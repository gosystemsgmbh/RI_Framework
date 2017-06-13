using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using RI.Framework.Collections;




namespace RI.Framework.StateMachines
{
	/// <summary>
	///     Extends a state machine with asynchronous functionality.
	/// </summary>
	/// <remarks>
	///     <para>
	///         See <see cref="StateMachine" /> for more details about state machines.
	///     </para>
	/// </remarks>
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
			this.UpdateTasks = new List<TaskCompletionSource<object>>();
		}

		#endregion




		#region Instance Properties/Indexer

		private List<Tuple<object, TaskCompletionSource<object>>> SignalTasks { get; set; }

		private List<Tuple<Type, TaskCompletionSource<object>>> TransitionTasks { get; set; }

		private List<TaskCompletionSource<object>> UpdateTasks { get; set; }

		#endregion




		#region Instance Methods

		/// <summary>
		///     Sends a signal to the current state.
		/// </summary>
		/// <typeparam name="TSignal"> The type of the signal. </typeparam>
		/// <param name="signal"> The signal. </param>
		/// <param name="timeout"> The timeout in milliseconds the method should wait for the signal to be sent. </param>
		/// <returns>
		///     true if the signal was executed within the specified timeout, false otherwise.
		/// </returns>
		public async Task<bool> SignalAsync <TSignal> (TSignal signal, int timeout)
		{
			return await this.SignalAsync((object)signal, timeout);
		}

		/// <summary>
		///     Sends a signal to the current state.
		/// </summary>
		/// <param name="signal"> The signal. </param>
		/// <param name="timeout"> The timeout in milliseconds the method should wait for the signal to be sent. </param>
		/// <returns>
		///     true if the signal was executed within the specified timeout, false otherwise.
		/// </returns>
		public async Task<bool> SignalAsync (object signal, int timeout)
		{
			Task<bool> waitTask;
			lock (this.SyncRoot)
			{
				this.Signal(signal);
				waitTask = this.WaitForSignalAsync(signal, timeout);
			}

			return await waitTask;
		}

		/// <summary>
		///     Initiates a transition to another state.
		/// </summary>
		/// <typeparam name="TState"> The type of state to transition to. </typeparam>
		/// <param name="timeout"> The timeout in milliseconds the method should wait for the transition to be finished. </param>
		/// <returns>
		///     true if the transition was executed within the specified timeout, false otherwise.
		/// </returns>
		/// <remarks>
		///     <para>
		///         The method waits for the execution of the transition, even if the current state is already the same as the requested state.
		///     </para>
		/// </remarks>
		/// <exception cref="TaskCanceledException"> The state transition was aborted. </exception>
		public async Task<bool> TransientAsync <TState> (int timeout)
			where TState : IState
		{
			return await this.TransientAsync(typeof(TState), timeout);
		}

		/// <summary>
		///     Initiates a transition to another state.
		/// </summary>
		/// <param name="state"> The type of state to transition to. </param>
		/// <param name="timeout"> The timeout in milliseconds the method should wait for the transition to be finished. </param>
		/// <returns>
		///     true if the transition was executed within the specified timeout, false otherwise.
		/// </returns>
		/// <remarks>
		///     <para>
		///         The method waits for the execution of the transition, even if the current state is already the same as the requested state.
		///     </para>
		/// </remarks>
		/// <exception cref="TaskCanceledException"> The state transition was aborted. </exception>
		public async Task<bool> TransientAsync (Type state, int timeout)
		{
			Task<bool> waitTask;
			lock (this.SyncRoot)
			{
				this.Transient(state);
				waitTask = this.WaitForStateAsyncInternal(state, timeout, true, CancellationToken.None);
			}

			return await waitTask;
		}

		/// <summary>
		///     Updates the current state.
		/// </summary>
		/// <param name="timeout"> The timeout in milliseconds the method should wait for the update to be executed. </param>
		/// <returns>
		///     true if the update was executed within the specified timeout, false otherwise.
		/// </returns>
		public async Task<bool> UpdateAsync (int timeout)
		{
			Task<bool> waitTask;
			lock (this.SyncRoot)
			{
				this.Update();
				waitTask = this.WaitForUpdateAsync(timeout);
			}

			return await waitTask;
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
			return await this.WaitForSignalAsync(signal, timeout, CancellationToken.None);
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
			return await this.WaitForSignalAsync(signal, timeout, CancellationToken.None);
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
			return await this.WaitForSignalAsync((object)signal, timeout, cancellationToken);
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
			TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();

			lock (this.SyncRoot)
			{
				this.SignalTasks.Add(new Tuple<object, TaskCompletionSource<object>>(signal, tcs));
			}

			Task signalTask = tcs.Task;
			Task timeoutTask = Task.Delay(timeout, cancellationToken);

			Task completed = await Task.WhenAny(signalTask, timeoutTask);
			return completed != timeoutTask;
		}

		/// <summary>
		///     Waits until a specified state becomes the current/active state.
		/// </summary>
		/// <typeparam name="TState"> The type of state to wait for. </typeparam>
		/// <param name="timeout"> The timeout in milliseconds the method should wait for the state. </param>
		/// <returns>
		///     true if the specified state became active within the specified timeout, false otherwise.
		/// </returns>
		/// <remarks>
		///     <para>
		///         The method does not wait and returns immediately if the current state is the requested state.
		///     </para>
		/// </remarks>
		/// <exception cref="TaskCanceledException"> The state transition was aborted. </exception>
		public async Task<bool> WaitForStateAsync <TState> (int timeout)
			where TState : IState
		{
			return await this.WaitForStateAsync<TState>(timeout, CancellationToken.None);
		}

		/// <summary>
		///     Waits until a specified state becomes the current/active state.
		/// </summary>
		/// <param name="state"> The type of state to wait for. </param>
		/// <param name="timeout"> The timeout in milliseconds the method should wait for the state. </param>
		/// <returns>
		///     true if the specified state became active within the specified timeout, false otherwise.
		/// </returns>
		/// <remarks>
		///     <para>
		///         The method does not wait and returns immediately if the current state is the requested state.
		///     </para>
		/// </remarks>
		/// <exception cref="TaskCanceledException"> The state transition was aborted. </exception>
		public async Task<bool> WaitForStateAsync (Type state, int timeout)
		{
			return await this.WaitForStateAsync(state, timeout, CancellationToken.None);
		}

		/// <summary>
		///     Waits until a specified state becomes the current/active state.
		/// </summary>
		/// <typeparam name="TState"> The type of state to wait for. </typeparam>
		/// <param name="timeout"> The timeout in milliseconds the method should wait for the state. </param>
		/// <param name="cancellationToken"> The token which can be used to cancel the wait. </param>
		/// <returns>
		///     true if the specified state became active within the specified timeout, false otherwise.
		/// </returns>
		/// <remarks>
		///     <para>
		///         The method does not wait and returns immediately if the current state is the requested state.
		///     </para>
		/// </remarks>
		/// <exception cref="TaskCanceledException"> The state transition was aborted. </exception>
		public async Task<bool> WaitForStateAsync <TState> (int timeout, CancellationToken cancellationToken)
			where TState : IState
		{
			return await this.WaitForStateAsyncInternal(typeof(TState), timeout, false, cancellationToken);
		}

		/// <summary>
		///     Waits until a specified state becomes the current/active state.
		/// </summary>
		/// <param name="state"> The type of state to wait for. </param>
		/// <param name="timeout"> The timeout in milliseconds the method should wait for the state. </param>
		/// <param name="cancellationToken"> The token which can be used to cancel the wait. </param>
		/// <returns>
		///     true if the specified state became active within the specified timeout, false otherwise.
		/// </returns>
		/// <remarks>
		///     <para>
		///         The method does not wait and returns immediately if the current state is the requested state.
		///     </para>
		/// </remarks>
		/// <exception cref="TaskCanceledException"> The state transition was aborted. </exception>
		public async Task<bool> WaitForStateAsync (Type state, int timeout, CancellationToken cancellationToken)
		{
			return await this.WaitForStateAsyncInternal(state, timeout, false, cancellationToken);
		}

		/// <summary>
		///     Waits until the state has been updated.
		/// </summary>
		/// <param name="timeout"> The timeout in milliseconds the method should wait for the update to be executed. </param>
		/// <returns>
		///     true if the update was executed within the specified timeout, false otherwise.
		/// </returns>
		public async Task<bool> WaitForUpdateAsync (int timeout)
		{
			return await this.WaitForUpdateAsync(timeout, CancellationToken.None);
		}

		/// <summary>
		///     Waits until the state has been updated.
		/// </summary>
		/// <param name="timeout"> The timeout in milliseconds the method should wait for the update to be executed. </param>
		/// <param name="cancellationToken"> The token which can be used to cancel the wait. </param>
		/// <returns>
		///     true if the update was executed within the specified timeout, false otherwise.
		/// </returns>
		public async Task<bool> WaitForUpdateAsync (int timeout, CancellationToken cancellationToken)
		{
			TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();

			lock (this.SyncRoot)
			{
				this.UpdateTasks.Add(tcs);
			}

			Task updateTask = tcs.Task;
			Task timeoutTask = Task.Delay(timeout, cancellationToken);

			Task completed = await Task.WhenAny(updateTask, timeoutTask);
			return completed != timeoutTask;
		}

		private async Task<bool> WaitForStateAsyncInternal (Type state, int timeout, bool ignoreCurrentState, CancellationToken cancellationToken)
		{
			TaskCompletionSource<object> tcs;

			lock (this.SyncRoot)
			{
				if ((!ignoreCurrentState) && (this.State?.GetType() == state))
				{
					return true;
				}

				tcs = new TaskCompletionSource<object>();
				this.TransitionTasks.Add(new Tuple<Type, TaskCompletionSource<object>>(state, tcs));
			}

			Task transitionTask = tcs.Task;
			Task timeoutTask = Task.Delay(timeout, cancellationToken);

			Task completed = await Task.WhenAny(transitionTask, timeoutTask);
			return completed != timeoutTask;
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
				task.Item2.SetResult(true);
			}
		}

		/// <inheritdoc />
		protected override void OnAfterSignal (StateSignalInfo signalInfo)
		{
			base.OnAfterSignal(signalInfo);

			List<Tuple<object, TaskCompletionSource<object>>> tasks = this.SignalTasks.RemoveWhere(x => object.ReferenceEquals(x.Item1, signalInfo.Signal));
			foreach (Tuple<object, TaskCompletionSource<object>> task in tasks)
			{
				task.Item2.SetResult(true);
			}
		}

		/// <inheritdoc />
		protected override void OnAfterUpdate (StateUpdateInfo updateInfo)
		{
			base.OnAfterUpdate(updateInfo);

			foreach (TaskCompletionSource<object> task in this.UpdateTasks)
			{
				task.SetResult(true);
			}

			this.UpdateTasks.Clear();
		}

		/// <inheritdoc />
		protected override void OnTransitionAborted (StateTransientInfo transientInfo)
		{
			base.OnTransitionAborted(transientInfo);

			List<Tuple<Type, TaskCompletionSource<object>>> tasks = this.TransitionTasks.RemoveWhere(x => x.Item1 == transientInfo.NextState?.GetType());
			foreach (Tuple<Type, TaskCompletionSource<object>> task in tasks)
			{
				task.Item2.SetCanceled();
			}
		}

		#endregion
	}
}
