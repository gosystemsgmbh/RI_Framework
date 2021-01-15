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
    /// TODO: Use TaskScheduler optionally provided by dispatcher
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
        ///     The task used to wait for finishing the signal processing.
        /// </returns>
        public Task SignalAsync <TSignal> (TSignal signal)
        {
            return this.SignalAsync((object)signal);
        }

        /// <summary>
        ///     Sends a signal to the current state.
        /// </summary>
        /// <param name="signal"> The signal. </param>
        /// <returns>
        ///     The task used to wait for finishing the signal processing.
        /// </returns>
        public Task SignalAsync (object signal)
        {
            Task<bool> waitTask;
            lock (this.SyncRoot)
            {
                waitTask = this.WaitForSignalAsync(signal, Timeout.Infinite, CancellationToken.None);
                this.SignalInternal(signal, new object[0]);
            }

            return waitTask;
        }

        /// <summary>
        ///     Sends a signal to the current state.
        /// </summary>
        /// <typeparam name="TSignal"> The type of the signal. </typeparam>
        /// <param name="signal"> The signal. </param>
        /// <param name="parameters"> Specifies optional signal parameters passed to the current state. </param>
        /// <returns>
        ///     The task used to wait for finishing the signal processing.
        /// </returns>
        public Task SignalAsync<TSignal>(TSignal signal, params object[] parameters)
        {
            return this.SignalAsync((object)signal, parameters);
        }

        /// <summary>
        ///     Sends a signal to the current state.
        /// </summary>
        /// <param name="signal"> The signal. </param>
        /// <param name="parameters"> Specifies optional signal parameters passed to the current state. </param>
        /// <returns>
        ///     The task used to wait for finishing the signal processing.
        /// </returns>
        public Task SignalAsync(object signal, params object[] parameters)
        {
            Task<bool> waitTask;
            lock (this.SyncRoot)
            {
                waitTask = this.WaitForSignalAsync(signal, Timeout.Infinite, CancellationToken.None);
                this.SignalInternal(signal, parameters);
            }

            return waitTask;
        }

        /// <summary>
        ///     Initiates a transition to another state.
        /// </summary>
        /// <typeparam name="TState"> The type of state to transition to. </typeparam>
        /// <returns>
        ///     The task used to wait for finishing the transition.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         The method waits for the execution of the transition, even if the current state is already the same as the requested state.
        ///     </para>
        /// </remarks>
        /// <exception cref="TaskCanceledException"> The state transition was aborted. </exception>
        public Task TransientAsync <TState> ()
            where TState : IState
        {
            return this.TransientAsync(typeof(TState));
        }

        /// <summary>
        ///     Initiates a transition to another state.
        /// </summary>
        /// <param name="state"> The type of state to transition to. </param>
        /// <returns>
        ///     The task used to wait for finishing the transition.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         The method waits for the execution of the transition, even if the current state is already the same as the requested state.
        ///     </para>
        /// </remarks>
        /// <exception cref="TaskCanceledException"> The state transition was aborted. </exception>
        public Task TransientAsync (Type state)
        {
            IState nextState = this.Resolve(state);

            Task<bool> waitTask;
            lock (this.SyncRoot)
            {
                waitTask = this.WaitForTransientAsync(state, Timeout.Infinite, CancellationToken.None);
                this.TransientInternal(nextState, new object[0]);
            }

            return waitTask;
        }

        /// <summary>
        ///     Initiates a transition to another state.
        /// </summary>
        /// <typeparam name="TState"> The type of state to transition to. </typeparam>
        /// <param name="parameters"> Specifies optional transition parameters passed to the next state. </param>
        /// <returns>
        ///     The task used to wait for finishing the transition.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         The method waits for the execution of the transition, even if the current state is already the same as the requested state.
        ///     </para>
        /// </remarks>
        /// <exception cref="TaskCanceledException"> The state transition was aborted. </exception>
        public Task TransientAsync <TState> (params object[] parameters)
            where TState : IState
        {
            return this.TransientAsync(typeof(TState), parameters);
        }

        /// <summary>
        ///     Initiates a transition to another state.
        /// </summary>
        /// <param name="state"> The type of state to transition to. </param>
        /// <param name="parameters"> Specifies optional transition parameters passed to the next state. </param>
        /// <returns>
        ///     The task used to wait for finishing the transition.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         The method waits for the execution of the transition, even if the current state is already the same as the requested state.
        ///     </para>
        /// </remarks>
        /// <exception cref="TaskCanceledException"> The state transition was aborted. </exception>
        public Task TransientAsync (Type state, params object[] parameters)
        {
            IState nextState = this.Resolve(state);

            Task<bool> waitTask;
            lock (this.SyncRoot)
            {
                waitTask = this.WaitForTransientAsync(state, Timeout.Infinite, CancellationToken.None);
                this.TransientInternal(nextState, parameters ?? new object[0]);
            }

            return waitTask;
        }

        /// <summary>
        ///     Updates the current state.
        /// </summary>
        /// <returns>
        ///     The task used to wait for finishing the update.
        /// </returns>
        public Task UpdateAsync ()
        {
            Task<bool> waitTask;
            lock (this.SyncRoot)
            {
                waitTask = this.WaitForUpdateAsync(Timeout.Infinite, CancellationToken.None);
                this.UpdateInternal(0);
            }

            return waitTask;
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
        public Task<bool> WaitForSignalAsync <TSignal> (TSignal signal, int timeout)
        {
            return this.WaitForSignalAsync(signal, timeout, CancellationToken.None);
        }

        /// <summary>
        ///     Waits until a specified signal has been processed.
        /// </summary>
        /// <param name="signal"> The signal to wait for. </param>
        /// <param name="timeout"> The timeout in milliseconds the method should wait for the signal. </param>
        /// <returns>
        ///     true if the signal was executed within the specified timeout, false otherwise.
        /// </returns>
        public Task<bool> WaitForSignalAsync (object signal, int timeout)
        {
            return this.WaitForSignalAsync(signal, timeout, CancellationToken.None);
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
        public Task<bool> WaitForSignalAsync <TSignal> (TSignal signal, int timeout, CancellationToken cancellationToken)
        {
            return this.WaitForSignalAsync((object)signal, timeout, cancellationToken);
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
        public Task<bool> WaitForSignalAsync (object signal, int timeout, CancellationToken cancellationToken)
        {
            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);

            lock (this.SyncRoot)
            {
                this.SignalTasks.Add(new Tuple<object, TaskCompletionSource<object>>(signal, tcs));
            }

            Task signalTask = tcs.Task;
            Task timeoutTask = Task.Delay(timeout, cancellationToken);

            Task<Task> completed = Task.WhenAny(signalTask, timeoutTask);
            return completed.ContinueWith(_ => object.ReferenceEquals(completed, signalTask), CancellationToken.None, TaskContinuationOptions.DenyChildAttach | TaskContinuationOptions.LazyCancellation | TaskContinuationOptions.RunContinuationsAsynchronously, TaskScheduler.Current);
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
        public Task<bool> WaitForTransientAsync <TState> (int timeout)
            where TState : IState
        {
            return this.WaitForTransientAsync<TState>(timeout, CancellationToken.None);
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
        public Task<bool> WaitForTransientAsync (Type state, int timeout)
        {
            return this.WaitForTransientAsync(state, timeout, CancellationToken.None);
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
        public Task<bool> WaitForTransientAsync <TState> (int timeout, CancellationToken cancellationToken)
            where TState : IState
        {
            return this.WaitForTransientAsync(typeof(TState), timeout, cancellationToken);
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
        public Task<bool> WaitForTransientAsync (Type state, int timeout, CancellationToken cancellationToken)
        {
            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);

            lock (this.SyncRoot)
            {
                this.TransitionTasks.Add(new Tuple<Type, TaskCompletionSource<object>>(state, tcs));
            }

            Task transitionTask = tcs.Task;
            Task timeoutTask = Task.Delay(timeout, cancellationToken);

            Task<Task> completed = Task.WhenAny(transitionTask, timeoutTask);
            return completed.ContinueWith(_ => object.ReferenceEquals(completed, transitionTask), CancellationToken.None, TaskContinuationOptions.DenyChildAttach | TaskContinuationOptions.LazyCancellation | TaskContinuationOptions.RunContinuationsAsynchronously, TaskScheduler.Current);
        }

        /// <summary>
        ///     Waits until the current state has been updated.
        /// </summary>
        /// <param name="timeout"> The timeout in milliseconds the method should wait for the update to be executed. </param>
        /// <returns>
        ///     true if the update was executed within the specified timeout, false otherwise.
        /// </returns>
        public Task<bool> WaitForUpdateAsync (int timeout)
        {
            return this.WaitForUpdateAsync(timeout, CancellationToken.None);
        }

        /// <summary>
        ///     Waits until the current state has been updated.
        /// </summary>
        /// <param name="timeout"> The timeout in milliseconds the method should wait for the update to be executed. </param>
        /// <param name="cancellationToken"> The token which can be used to cancel the wait. </param>
        /// <returns>
        ///     true if the update was executed within the specified timeout, false otherwise.
        /// </returns>
        public Task<bool> WaitForUpdateAsync (int timeout, CancellationToken cancellationToken)
        {
            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);

            lock (this.SyncRoot)
            {
                this.UpdateTasks.Add(new Tuple<Type, TaskCompletionSource<object>>(this.State?.GetType(), tcs));
            }

            Task updateTask = tcs.Task;
            Task timeoutTask = Task.Delay(timeout, cancellationToken);

            Task<Task> completed = Task.WhenAny(updateTask, timeoutTask);
            return completed.ContinueWith(_ => object.ReferenceEquals(completed, updateTask), CancellationToken.None, TaskContinuationOptions.DenyChildAttach | TaskContinuationOptions.LazyCancellation | TaskContinuationOptions.RunContinuationsAsynchronously, TaskScheduler.Current);
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

            List<Tuple<object, TaskCompletionSource<object>>> tasks = this.SignalTasks.RemoveWhere(x =>
            {
                if((x.Item1 == null) && (signalInfo.Signal == null))
                {
                    return true;
                }

                if ((x.Item1 == null) || (signalInfo.Signal == null))
                {
                    return false;
                }

                if (object.ReferenceEquals(x.Item1, signalInfo.Signal))
                {
                    return true;
                }

                if (x.Item1.GetType() != signalInfo.GetType())
                {
                    return false;
                }

                return x.Item1.Equals(signalInfo.Signal);
            });
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
