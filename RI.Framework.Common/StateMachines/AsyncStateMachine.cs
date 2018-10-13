using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using RI.Framework.Collections;
using RI.Framework.StateMachines.Caches;
using RI.Framework.StateMachines.Dispatchers;
using RI.Framework.StateMachines.Resolvers;
using RI.Framework.StateMachines.States;




namespace RI.Framework.StateMachines
{
	/// <summary>
	///     Extends a state machine with asynchronous functionality.
	/// </summary>
	/// <remarks>
	///     <para>
	///         See <see cref="StateMachine" /> for more details about state machines.
	///     </para>
	///     <note type="important">
	///         Some virtual methods are called from within locks to <see cref="StateMachine.SyncRoot" />.
	///         Be careful in inheriting classes when calling outside code from those methods (e.g. through events, callbacks, or other virtual methods) to not produce deadlocks!
	///     </note>
	/// </remarks>
	/// <threadsafety static="true" instance="true" />
	public class AsyncStateMachine : StateMachine
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="AsyncStateMachine" />.
		/// </summary>
		/// <param name="configuration"> The state machines configuration. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="configuration" /> is null. </exception>
		/// <exception cref="ArgumentException"> <paramref name="configuration" /> does not specify a <see cref="IStateDispatcher" />, <see cref="IStateResolver" />, or <see cref="IStateCache" />. </exception>
		public AsyncStateMachine (StateMachineConfiguration configuration)
			: base(configuration)
		{
			this.TransitionTasks = new List<Tuple<Type, TaskCompletionSource<object>>>();
			this.SignalTasks = new List<Tuple<object, TaskCompletionSource<object>>>();
			this.UpdateTasks = new List<Tuple<Type, TaskCompletionSource<object>>>();
		}

		#endregion




		#region Instance Properties/Indexer

		private List<Tuple<object, TaskCompletionSource<object>>> SignalTasks { get; }

		private List<Tuple<Type, TaskCompletionSource<object>>> TransitionTasks { get; }

		private List<Tuple<Type, TaskCompletionSource<object>>> UpdateTasks { get; }

		#endregion




		#region Instance Methods

		/// <summary>
		///     Sends a signal to the current state.
		/// </summary>
		/// <typeparam name="TSignal"> The type of the signal. </typeparam>
		/// <param name="signal"> The signal. </param>
		/// <returns>
		///     true if the signal was executed within the specified timeout, false otherwise.
		/// </returns>
		public async Task<bool> SignalAsync <TSignal> (TSignal signal)
		{
			return await this.SignalAsync((object)signal).ConfigureAwait(false);
		}

		/// <summary>
		///     Sends a signal to the current state.
		/// </summary>
		/// <param name="signal"> The signal. </param>
		/// <returns>
		///     true if the signal was executed within the specified timeout, false otherwise.
		/// </returns>
		public async Task<bool> SignalAsync (object signal)
		{
			Task<bool> waitTask;
			lock (this.SyncRoot)
			{
				waitTask = this.WaitForSignalAsync(signal, Timeout.Infinite, CancellationToken.None);
				this.SignalInternal(signal);
			}

			return await waitTask.ConfigureAwait(false);
		}

		/// <summary>
		///     Initiates a transition to another state.
		/// </summary>
		/// <typeparam name="TState"> The type of state to transition to. </typeparam>
		/// <returns>
		///     true if the transition was executed within the specified timeout, false otherwise.
		/// </returns>
		/// <remarks>
		///     <para>
		///         The method waits for the execution of the transition, even if the current state is already the same as the requested state.
		///     </para>
		/// </remarks>
		/// <exception cref="TaskCanceledException"> The state transition was aborted. </exception>
		public async Task<bool> TransientAsync <TState> ()
			where TState : IState
		{
			return await this.TransientAsync(typeof(TState)).ConfigureAwait(false);
		}

		/// <summary>
		///     Initiates a transition to another state.
		/// </summary>
		/// <param name="state"> The type of state to transition to. </param>
		/// <returns>
		///     true if the transition was executed within the specified timeout, false otherwise.
		/// </returns>
		/// <remarks>
		///     <para>
		///         The method waits for the execution of the transition, even if the current state is already the same as the requested state.
		///     </para>
		/// </remarks>
		/// <exception cref="TaskCanceledException"> The state transition was aborted. </exception>
		public async Task<bool> TransientAsync (Type state)
		{
			IState nextState = this.Resolve(state, true);

			Task<bool> waitTask;
			lock (this.SyncRoot)
			{
				waitTask = this.WaitForTransientAsync(state, Timeout.Infinite, CancellationToken.None);
				this.TransientInternal(nextState, new object[0]);
			}

			return await waitTask.ConfigureAwait(false);
		}

		/// <summary>
		///     Initiates a transition to another state.
		/// </summary>
		/// <typeparam name="TState"> The type of state to transition to. </typeparam>
		/// <param name="parameters"> Specifies optional transition parameters passed to the next state. </param>
		/// <returns>
		///     true if the transition was executed within the specified timeout, false otherwise.
		/// </returns>
		/// <remarks>
		///     <para>
		///         The method waits for the execution of the transition, even if the current state is already the same as the requested state.
		///     </para>
		/// </remarks>
		/// <exception cref="TaskCanceledException"> The state transition was aborted. </exception>
		public async Task<bool> TransientAsync <TState> (params object[] parameters)
			where TState : IState
		{
			return await this.TransientAsync(typeof(TState), parameters).ConfigureAwait(false);
		}

		/// <summary>
		///     Initiates a transition to another state.
		/// </summary>
		/// <param name="state"> The type of state to transition to. </param>
		/// <param name="parameters"> Specifies optional transition parameters passed to the next state. </param>
		/// <returns>
		///     true if the transition was executed within the specified timeout, false otherwise.
		/// </returns>
		/// <remarks>
		///     <para>
		///         The method waits for the execution of the transition, even if the current state is already the same as the requested state.
		///     </para>
		/// </remarks>
		/// <exception cref="TaskCanceledException"> The state transition was aborted. </exception>
		public async Task<bool> TransientAsync (Type state, params object[] parameters)
		{
			IState nextState = this.Resolve(state, true);

			Task<bool> waitTask;
			lock (this.SyncRoot)
			{
				waitTask = this.WaitForTransientAsync(state, Timeout.Infinite, CancellationToken.None);
				this.TransientInternal(nextState, parameters ?? new object[0]);
			}

			return await waitTask.ConfigureAwait(false);
		}

		/// <summary>
		///     Updates the current state.
		/// </summary>
		/// <returns>
		///     true if the update was executed within the specified timeout, false otherwise.
		/// </returns>
		public async Task<bool> UpdateAsync ()
		{
			Task<bool> waitTask;
			lock (this.SyncRoot)
			{
				waitTask = this.WaitForUpdateAsync(Timeout.Infinite, CancellationToken.None);
				this.UpdateInternal(0);
			}

			return await waitTask.ConfigureAwait(false);
		}

		/// <summary>
		///     Waits until a specified signal has been processed.
		/// </summary>
		/// <typeparam name="TSignal"> The type of state to wait for. </typeparam>
		/// <param name="signal"> The signal to wait for. </param>
		/// <param name="timeout"> The timeout in milliseconds the method should wait for the signal. </param>
		/// <returns>
		///     true if the signal was executed within the specified timeout, false otherwise.
		/// </returns>
		public async Task<bool> WaitForSignalAsync <TSignal> (TSignal signal, int timeout)
		{
			return await this.WaitForSignalAsync(signal, timeout, CancellationToken.None).ConfigureAwait(false);
		}

		/// <summary>
		///     Waits until a specified signal has been processed.
		/// </summary>
		/// <param name="signal"> The signal to wait for. </param>
		/// <param name="timeout"> The timeout in milliseconds the method should wait for the signal. </param>
		/// <returns>
		///     true if the signal was executed within the specified timeout, false otherwise.
		/// </returns>
		public async Task<bool> WaitForSignalAsync (object signal, int timeout)
		{
			return await this.WaitForSignalAsync(signal, timeout, CancellationToken.None).ConfigureAwait(false);
		}

		/// <summary>
		///     Waits until a specified signal has been processed.
		/// </summary>
		/// <typeparam name="TSignal"> The type of state to wait for. </typeparam>
		/// <param name="signal"> The signal to wait for. </param>
		/// <param name="timeout"> The timeout in milliseconds the method should wait for the signal. </param>
		/// <param name="cancellationToken"> The token which can be used to cancel the wait. </param>
		/// <returns>
		///     true if the signal was executed within the specified timeout, false otherwise.
		/// </returns>
		public async Task<bool> WaitForSignalAsync <TSignal> (TSignal signal, int timeout, CancellationToken cancellationToken)
		{
			return await this.WaitForSignalAsync((object)signal, timeout, cancellationToken).ConfigureAwait(false);
		}

		/// <summary>
		///     Waits until a specified signal has been processed.
		/// </summary>
		/// <param name="signal"> The signal to wait for. </param>
		/// <param name="timeout"> The timeout in milliseconds the method should wait for the signal. </param>
		/// <param name="cancellationToken"> The token which can be used to cancel the wait. </param>
		/// <returns>
		///     true if the signal was executed within the specified timeout, false otherwise.
		/// </returns>
		public async Task<bool> WaitForSignalAsync (object signal, int timeout, CancellationToken cancellationToken)
		{
			TaskCompletionSource<object> tcs = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);

			lock (this.SyncRoot)
			{
				this.SignalTasks.Add(new Tuple<object, TaskCompletionSource<object>>(signal, tcs));
			}

			Task signalTask = tcs.Task;
			Task timeoutTask = Task.Delay(timeout, cancellationToken);

			Task completed = await Task.WhenAny(signalTask, timeoutTask).ConfigureAwait(false);
			return object.ReferenceEquals(completed, signalTask);
		}

		/// <summary>
		///     Waits until a transient to a specified state has been processed.
		/// </summary>
		/// <typeparam name="TState"> The type of state to wait for. </typeparam>
		/// <param name="timeout"> The timeout in milliseconds the method should wait for the state. </param>
		/// <returns>
		///     true if the specified state became active within the specified timeout, false otherwise.
		/// </returns>
		/// <remarks>
		///     <para>
		///         The method always waits for the specified transient to happen, even if the current state is the same state as to transient to.
		///     </para>
		/// </remarks>
		/// <exception cref="TaskCanceledException"> The state transition was aborted. </exception>
		public async Task<bool> WaitForTransientAsync <TState> (int timeout)
			where TState : IState
		{
			return await this.WaitForTransientAsync<TState>(timeout, CancellationToken.None).ConfigureAwait(false);
		}

		/// <summary>
		///     Waits until a transient to a specified state has been processed.
		/// </summary>
		/// <param name="state"> The type of state to wait for. </param>
		/// <param name="timeout"> The timeout in milliseconds the method should wait for the state. </param>
		/// <returns>
		///     true if the specified state became active within the specified timeout, false otherwise.
		/// </returns>
		/// <remarks>
		///     <para>
		///         The method always waits for the specified transient to happen, even if the current state is the same state as to transient to.
		///     </para>
		/// </remarks>
		/// <exception cref="TaskCanceledException"> The state transition was aborted. </exception>
		public async Task<bool> WaitForTransientAsync (Type state, int timeout)
		{
			return await this.WaitForTransientAsync(state, timeout, CancellationToken.None).ConfigureAwait(false);
		}

		/// <summary>
		///     Waits until a transient to a specified state has been processed.
		/// </summary>
		/// <typeparam name="TState"> The type of state to wait for. </typeparam>
		/// <param name="timeout"> The timeout in milliseconds the method should wait for the state. </param>
		/// <param name="cancellationToken"> The token which can be used to cancel the wait. </param>
		/// <returns>
		///     true if the specified state became active within the specified timeout, false otherwise.
		/// </returns>
		/// <remarks>
		///     <para>
		///         The method always waits for the specified transient to happen, even if the current state is the same state as to transient to.
		///     </para>
		/// </remarks>
		/// <exception cref="TaskCanceledException"> The state transition was aborted. </exception>
		public async Task<bool> WaitForTransientAsync <TState> (int timeout, CancellationToken cancellationToken)
			where TState : IState
		{
			return await this.WaitForTransientAsync(typeof(TState), timeout, cancellationToken).ConfigureAwait(false);
		}

		/// <summary>
		///     Waits until a transient to a specified state has been processed.
		/// </summary>
		/// <param name="state"> The type of state to wait for. </param>
		/// <param name="timeout"> The timeout in milliseconds the method should wait for the state. </param>
		/// <param name="cancellationToken"> The token which can be used to cancel the wait. </param>
		/// <returns>
		///     true if the specified state became active within the specified timeout, false otherwise.
		/// </returns>
		/// <remarks>
		///     <para>
		///         The method always waits for the specified transient to happen, even if the current state is the same state as to transient to.
		///     </para>
		/// </remarks>
		/// <exception cref="TaskCanceledException"> The state transition was aborted. </exception>
		public async Task<bool> WaitForTransientAsync (Type state, int timeout, CancellationToken cancellationToken)
		{
			TaskCompletionSource<object> tcs = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);

			lock (this.SyncRoot)
			{
				this.TransitionTasks.Add(new Tuple<Type, TaskCompletionSource<object>>(state, tcs));
			}

			Task transitionTask = tcs.Task;
			Task timeoutTask = Task.Delay(timeout, cancellationToken);

			Task completed = await Task.WhenAny(transitionTask, timeoutTask).ConfigureAwait(false);
			return object.ReferenceEquals(completed, transitionTask);
		}

		/// <summary>
		///     Waits until the current state has been updated.
		/// </summary>
		/// <param name="timeout"> The timeout in milliseconds the method should wait for the update to be executed. </param>
		/// <returns>
		///     true if the update was executed within the specified timeout, false otherwise.
		/// </returns>
		public async Task<bool> WaitForUpdateAsync (int timeout)
		{
			return await this.WaitForUpdateAsync(timeout, CancellationToken.None).ConfigureAwait(false);
		}

		/// <summary>
		///     Waits until the current state has been updated.
		/// </summary>
		/// <param name="timeout"> The timeout in milliseconds the method should wait for the update to be executed. </param>
		/// <param name="cancellationToken"> The token which can be used to cancel the wait. </param>
		/// <returns>
		///     true if the update was executed within the specified timeout, false otherwise.
		/// </returns>
		public async Task<bool> WaitForUpdateAsync (int timeout, CancellationToken cancellationToken)
		{
			TaskCompletionSource<object> tcs = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);

			lock (this.SyncRoot)
			{
				this.UpdateTasks.Add(new Tuple<Type, TaskCompletionSource<object>>(this.State?.GetType(), tcs));
			}

			Task updateTask = tcs.Task;
			Task timeoutTask = Task.Delay(timeout, cancellationToken);

			Task completed = await Task.WhenAny(updateTask, timeoutTask).ConfigureAwait(false);
			return object.ReferenceEquals(completed, updateTask);
		}

		#endregion




		#region Overrides

		/// <inheritdoc />
		protected override void OnAfterEnter (StateTransientInfo transientInfo)
		{
			base.OnAfterEnter(transientInfo);

			List<Tuple<Type, TaskCompletionSource<object>>> tasks = this.TransitionTasks.RemoveWhere(x => x.Item1 == transientInfo.NextState?.GetType());
			foreach (Tuple<Type, TaskCompletionSource<object>> task in tasks)
			{
				task.Item2.TrySetResult(true);
			}
		}

		/// <inheritdoc />
		protected override void OnAfterSignal (StateSignalInfo signalInfo)
		{
			base.OnAfterSignal(signalInfo);

			List<Tuple<object, TaskCompletionSource<object>>> tasks = this.SignalTasks.RemoveWhere(x => object.ReferenceEquals(x.Item1, signalInfo.Signal));
			foreach (Tuple<object, TaskCompletionSource<object>> task in tasks)
			{
				task.Item2.TrySetResult(true);
			}
		}

		/// <inheritdoc />
		protected override void OnAfterUpdate (StateUpdateInfo updateInfo)
		{
			base.OnAfterUpdate(updateInfo);

			List<Tuple<Type, TaskCompletionSource<object>>> tasks = this.UpdateTasks.RemoveWhere(x => x.Item1 == updateInfo.UpdateState?.GetType());
			foreach (Tuple<Type, TaskCompletionSource<object>> task in tasks)
			{
				task.Item2.TrySetResult(true);
			}
		}

		/// <inheritdoc />
		protected override void OnTransitionAborted (StateTransientInfo transientInfo)
		{
			base.OnTransitionAborted(transientInfo);

			List<Tuple<Type, TaskCompletionSource<object>>> tasks = this.TransitionTasks.RemoveWhere(x => x.Item1 == transientInfo.NextState?.GetType());
			foreach (Tuple<Type, TaskCompletionSource<object>> task in tasks)
			{
				task.Item2.TrySetCanceled();
			}
		}

		/// <inheritdoc />
		protected override void OnUpdateAborted (StateUpdateInfo updateInfo)
		{
			base.OnUpdateAborted(updateInfo);

			List<Tuple<Type, TaskCompletionSource<object>>> tasks = this.UpdateTasks.RemoveWhere(x => x.Item1 == updateInfo.UpdateState?.GetType());
			foreach (Tuple<Type, TaskCompletionSource<object>> task in tasks)
			{
				task.Item2.TrySetCanceled();
			}
		}

		#endregion
	}
}
