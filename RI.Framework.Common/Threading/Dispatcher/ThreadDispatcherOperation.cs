using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Threading.Dispatcher
{
    /// <summary>
    ///     Used to track <see cref="IThreadDispatcher" /> operations or the processing of enqueued delegates respectively.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         See <see cref="IThreadDispatcher" /> for more information.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="true" instance="true" />
    public sealed class ThreadDispatcherOperation : ISynchronizable
    {
        #region Instance Constructor/Destructor

        internal ThreadDispatcherOperation (ThreadDispatcher dispatcher, ThreadDispatcherExecutionContext executionContext, int priority, ThreadDispatcherOptions options, Delegate action, object[] parameters)
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException(nameof(dispatcher));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            this.SyncRoot = new object();

            this.ExecutionContext = executionContext?.Clone();

            this.Dispatcher = dispatcher;
            this.Priority = priority;
            this.Options = options;
            this.Action = action;
            this.Parameters = parameters;

            this.Stage = 0;
            this.Task = null;

            this.State = ThreadDispatcherOperationState.Waiting;
            this.Result = null;
            this.Exception = null;

            this.OperationDone = new ManualResetEvent(false);
            this.OperationDoneTask = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);

            //TODO: Will this work if part of the pre-run queue?
            this.Dispatcher.AddKeepAlive(this);

            this.RunTimeMillisecondsInternal = 0.0;
            this.WatchdogTimeMillisecondsInternal = 0.0;
            this.WatchdogEventsInternal = 0;
        }

        /// <summary>
        ///     Garbage collects this instance of <see cref="ThreadDispatcherOperation" />.
        /// </summary>
        ~ThreadDispatcherOperation ()
        {
            this.Dispatcher?.RemoveKeepAlive(this);

            this.OperationDone?.Close();
            this.OperationDone = null;

            this.ExecutionContext?.Dispose();
            this.ExecutionContext = null;
        }

        #endregion




        #region Instance Fields

        private Exception _exception;
        private object _result;
        private ThreadDispatcherOperationState _state;

        #endregion




        #region Instance Properties/Indexer

        /// <summary>
        ///     Gets the delegate executed by this operation.
        /// </summary>
        /// <value>
        ///     The delegate executed by this operation.
        /// </value>
        public Delegate Action { get; }

        /// <summary>
        ///     Gets the exception which occurred during execution of the delegate associated with this dispatcher operation.
        /// </summary>
        /// <value>
        ///     The exception which occurred during execution or null if no exception was thrown or the operation was not yet processed.
        /// </value>
        public Exception Exception
        {
            get
            {
                lock (this.SyncRoot)
                {
                    return this._exception;
                }
            }
            private set
            {
                lock (this.SyncRoot)
                {
                    this._exception = value;
                }
            }
        }

        /// <summary>
        ///     Gets whether the dispatcher operation has finished processing.
        /// </summary>
        /// <value>
        ///     true if <see cref="State" /> is anything else than <see cref="ThreadDispatcherOperationState.Waiting" /> or <see cref="ThreadDispatcherOperationState.Executing" />, false if <see cref="State" /> is <see cref="ThreadDispatcherOperationState.Waiting" /> or <see cref="ThreadDispatcherOperationState.Executing" />.
        /// </value>
        public bool IsDone
        {
            get
            {
                lock (this.SyncRoot)
                {
                    return (this.State != ThreadDispatcherOperationState.Waiting) && (this.State != ThreadDispatcherOperationState.Executing);
                }
            }
        }

        /// <summary>
        ///     Gets the value returned by the delegate associated with this dispatcher operation.
        /// </summary>
        /// <value>
        ///     The value returned by the delegate associated with this dispatcher operation or null if the delegate has no return value or the operation was not yet processed.
        /// </value>
        public object Result
        {
            get
            {
                lock (this.SyncRoot)
                {
                    return this._result;
                }
            }
            private set
            {
                lock (this.SyncRoot)
                {
                    this._result = value;
                }
            }
        }

        /// <summary>
        ///     Gets the total time in milliseconds this dispatcher operation was executing within the dispatcher.
        /// </summary>
        /// <value>
        ///     The total time in milliseconds this dispatcher operation was executing within the dispatcher.
        /// </value>
        /// <remarks>
        ///     <para>
        ///         The executing time does only include the time the operation was using dispatcher resources, but does not include time which was spent in another thread (e.g. when the operation is a task which uses other threads).
        ///     </para>
        /// </remarks>
        public int RunTimeMilliseconds
        {
            get
            {
                lock (this.SyncRoot)
                {
                    return (int)this.RunTimeMillisecondsInternal;
                }
            }
        }

        /// <summary>
        ///     Gets the current state of the dispatcher operation.
        /// </summary>
        /// <value>
        ///     The current state of the dispatcher operation.
        /// </value>
        public ThreadDispatcherOperationState State
        {
            get
            {
                lock (this.SyncRoot)
                {
                    return this._state;
                }
            }
            private set
            {
                lock (this.SyncRoot)
                {
                    this._state = value;
                }
            }
        }

        /// <summary>
        ///     Gets the number of watchdog events for this operation.
        /// </summary>
        /// <value>
        ///     The number of watchdog events for this operation.
        /// </value>
        /// <remarks>
        ///     <para>
        ///         When a watchdog event occurs, this counter is already incremented, for example the first watchdog events has thsi property set to 1.
        ///     </para>
        /// </remarks>
        public int WatchdogEvents
        {
            get
            {
                lock (this.SyncRoot)
                {
                    return this.WatchdogEventsInternal;
                }
            }
        }

        /// <summary>
        ///     Gets the time in milliseconds this dispatcher operation was executing within the dispatcher since its last watchdog event.
        /// </summary>
        /// <value>
        ///     The time in milliseconds this dispatcher operation was executing within the dispatcher since its last watchdog event.
        /// </value>
        /// <remarks>
        ///     <para>
        ///         The executing time does only include the time the operation was using dispatcher resources, but does not include time which was spent in another thread (e.g. when the operation is a task which uses other threads).
        ///     </para>
        /// </remarks>
        public int WatchdogTimeMilliseconds
        {
            get
            {
                lock (this.SyncRoot)
                {
                    return (int)this.WatchdogTimeMillisecondsInternal;
                }
            }
        }

        internal ThreadDispatcher Dispatcher { get; }
        internal ThreadDispatcherExecutionContext ExecutionContext { get; set; }
        internal ThreadDispatcherOptions Options { get; }
        internal object[] Parameters { get; }
        internal int Priority { get; }
        internal double RunTimeMillisecondsInternal { get; set; }
        internal int WatchdogEventsInternal { get; set; }
        internal double WatchdogTimeMillisecondsInternal { get; set; }
        private ManualResetEvent OperationDone { get; set; }
        private TaskCompletionSource<object> OperationDoneTask { get; set; }
        private int Stage { get; set; }
        private Task Task { get; set; }

        #endregion




        #region Instance Methods

        /// <summary>
        ///     Cancels the processing of the dispatcher operation.
        /// </summary>
        /// <returns>
        ///     true if the operation could be canceled, false otherwise.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         A dispatcher operation can only be canceled if its is still pending (<see cref="State" /> is <see cref="ThreadDispatcherOperationState.Waiting" />).
        ///     </para>
        /// </remarks>
        public bool Cancel ()
        {
            return this.CancelInternal(false);
        }

        /// <summary>
        ///     Waits indefinitely for the dispatcher operation to finish processing.
        /// </summary>
        public void Wait ()
        {
            this.Wait(Timeout.Infinite, CancellationToken.None);
        }

        /// <summary>
        ///     Waits indefinitely for the dispatcher operation to finish processing.
        /// </summary>
        /// <param name="cancellationToken"> The cancellation token which can be used to cancel the wait. </param>
        /// <returns>
        ///     true if the dispatcher operation finished processing, false if the wait was cancelled.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="cancellationToken" /> is null. </exception>
        public bool Wait (CancellationToken cancellationToken)
        {
            return this.Wait(Timeout.Infinite, cancellationToken);
        }

        /// <summary>
        ///     Waits a specified amount of time for the dispatcher operation to finish processing.
        /// </summary>
        /// <param name="timeout"> The maximum time to wait for the dispatcher operation to finish processing before the method returns. </param>
        /// <returns>
        ///     true if the dispatcher operation finished processing within the specified timeout, false otherwise.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"> <paramref name="timeout" /> is negative. </exception>
        public bool Wait (TimeSpan timeout)
        {
            return this.Wait((int)timeout.TotalMilliseconds, CancellationToken.None);
        }

        /// <summary>
        ///     Waits a specified amount of time for the dispatcher operation to finish processing.
        /// </summary>
        /// <param name="timeout"> The maximum time to wait for the dispatcher operation to finish processing before the method returns. </param>
        /// <param name="cancellationToken"> The cancellation token which can be used to cancel the wait. </param>
        /// <returns>
        ///     true if the dispatcher operation finished processing within the specified timeout, false otherwise or if the wait was cancelled.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"> <paramref name="timeout" /> is negative. </exception>
        /// <exception cref="ArgumentNullException"> <paramref name="cancellationToken" /> is null. </exception>
        public bool Wait (TimeSpan timeout, CancellationToken cancellationToken)
        {
            return this.Wait((int)timeout.TotalMilliseconds, cancellationToken);
        }

        /// <summary>
        ///     Waits a specified amount of time for the dispatcher operation to finish processing.
        /// </summary>
        /// <param name="milliseconds"> The maximum time in milliseconds to wait for the dispatcher operation to finish processing before the method returns. </param>
        /// <returns>
        ///     true if the dispatcher operation finished processing within the specified timeout, false otherwise.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"> <paramref name="milliseconds" /> is negative. </exception>
        public bool Wait (int milliseconds)
        {
            return this.Wait(milliseconds, CancellationToken.None);
        }

        /// <summary>
        ///     Waits a specified amount of time for the dispatcher operation to finish processing.
        /// </summary>
        /// <param name="milliseconds"> The maximum time in milliseconds to wait for the dispatcher operation to finish processing before the method returns. </param>
        /// <param name="cancellationToken"> The cancellation token which can be used to cancel the wait. </param>
        /// <returns>
        ///     true if the dispatcher operation finished processing within the specified timeout, false otherwise or if the wait was cancelled.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"> <paramref name="milliseconds" /> is negative. </exception>
        /// <exception cref="ArgumentNullException"> <paramref name="cancellationToken" /> is null. </exception>
        /// TODO: This is not safe to use on the dispatcher thread...it blocks the dispatcher indefinitely...!
        public bool Wait (int milliseconds, CancellationToken cancellationToken)
        {
            if ((milliseconds < 0) && (milliseconds != Timeout.Infinite))
            {
                throw new ArgumentOutOfRangeException(nameof(milliseconds));
            }

            if (cancellationToken == null)
            {
                throw new ArgumentNullException(nameof(cancellationToken));
            }

            if (this.IsDone)
            {
                return true;
            }

            bool result = WaitHandle.WaitAny(new[] {cancellationToken.WaitHandle, this.OperationDone}, milliseconds) == 1;
            return result;
        }

        /// <summary>
        ///     Waits indefinitely for the dispatcher operation to finish processing.
        /// </summary>
        /// <returns>
        ///     The task which can be used to await the finish of the processing.
        /// </returns>
        public async Task WaitAsync ()
        {
            await this.WaitAsync(Timeout.Infinite, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        ///     Waits indefinitely for the dispatcher operation to finish processing.
        /// </summary>
        /// <param name="cancellationToken"> The cancellation token which can be used to cancel the wait. </param>
        /// <returns>
        ///     true if the dispatcher operation finished processing, false if the wait was cancelled.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="cancellationToken" /> is null. </exception>
        public async Task<bool> WaitAsync (CancellationToken cancellationToken)
        {
            return await this.WaitAsync(Timeout.Infinite, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        ///     Waits a specified amount of time for the dispatcher operation to finish processing.
        /// </summary>
        /// <param name="timeout"> The maximum time to wait for the dispatcher operation to finish processing before the method returns. </param>
        /// <returns>
        ///     true if the dispatcher operation finished processing within the specified timeout, false otherwise.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"> <paramref name="timeout" /> is negative. </exception>
        public async Task<bool> WaitAsync (TimeSpan timeout)
        {
            return await this.WaitAsync((int)timeout.TotalMilliseconds, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        ///     Waits a specified amount of time for the dispatcher operation to finish processing.
        /// </summary>
        /// <param name="timeout"> The maximum time to wait for the dispatcher operation to finish processing before the method returns. </param>
        /// <param name="cancellationToken"> The cancellation token which can be used to cancel the wait. </param>
        /// <returns>
        ///     true if the dispatcher operation finished processing within the specified timeout, false otherwise or if the wait was cancelled.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"> <paramref name="timeout" /> is negative. </exception>
        /// <exception cref="ArgumentNullException"> <paramref name="cancellationToken" /> is null. </exception>
        public async Task<bool> WaitAsync (TimeSpan timeout, CancellationToken cancellationToken)
        {
            return await this.WaitAsync((int)timeout.TotalMilliseconds, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        ///     Waits a specified amount of time for the dispatcher operation to finish processing.
        /// </summary>
        /// <param name="milliseconds"> The maximum time in milliseconds to wait for the dispatcher operation to finish processing before the method returns. </param>
        /// <returns>
        ///     true if the dispatcher operation finished processing within the specified timeout, false otherwise.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"> <paramref name="milliseconds" /> is negative. </exception>
        public async Task<bool> WaitAsync (int milliseconds)
        {
            return await this.WaitAsync(milliseconds, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        ///     Waits a specified amount of time for the dispatcher operation to finish processing.
        /// </summary>
        /// <param name="milliseconds"> The maximum time in milliseconds to wait for the dispatcher operation to finish processing before the method returns. </param>
        /// <param name="cancellationToken"> The cancellation token which can be used to cancel the wait. </param>
        /// <returns>
        ///     true if the dispatcher operation finished processing within the specified timeout, false otherwise or if the wait was cancelled.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"> <paramref name="milliseconds" /> is negative. </exception>
        /// <exception cref="ArgumentNullException"> <paramref name="cancellationToken" /> is null. </exception>
        public async Task<bool> WaitAsync (int milliseconds, CancellationToken cancellationToken)
        {
            if ((milliseconds < 0) && (milliseconds != Timeout.Infinite))
            {
                throw new ArgumentOutOfRangeException(nameof(milliseconds));
            }

            if (cancellationToken == null)
            {
                throw new ArgumentNullException(nameof(cancellationToken));
            }

            if (this.IsDone)
            {
                return true;
            }

            Task operationTask = this.OperationDoneTask.Task;
            Task timeoutTask = Task.Delay(milliseconds, cancellationToken);

            Task completed = await Task.WhenAny(operationTask, timeoutTask).ConfigureAwait(false);
            return object.ReferenceEquals(completed, operationTask);
        }

        internal bool CancelHard ()
        {
            return this.CancelInternal(true);
        }

        internal void Execute ()
        {
            lock (this.SyncRoot)
            {
                if ((this.State != ThreadDispatcherOperationState.Waiting) && (this.Stage == 0))
                {
                    return;
                }

                if ((this.State != ThreadDispatcherOperationState.Executing) && (this.Stage != 0))
                {
                    return;
                }

                this.State = ThreadDispatcherOperationState.Executing;
            }

            bool finished = false;
            object result = null;
            Exception exception;
            bool canceled = false;

            try
            {
                finished = this.ExecuteCore(out result, out exception, out canceled);
            }
            catch (ThreadAbortException)
            {
                throw;
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            lock (this.SyncRoot)
            {
                if (canceled)
                {
                    this.Exception = null;
                    this.Result = null;
                    this.State = ThreadDispatcherOperationState.Canceled;

                    this.Dispatcher.RemoveKeepAlive(this);

                    this.OperationDone.Set();
                    this.OperationDoneTask.TrySetCanceled();
                }
                else if (exception != null)
                {
                    this.Exception = exception;
                    this.Result = null;
                    this.State = ThreadDispatcherOperationState.Exception;

                    this.Dispatcher.RemoveKeepAlive(this);

                    this.OperationDone.Set();
                    this.OperationDoneTask.TrySetException(this.Exception);
                }
                else if (finished)
                {
                    this.Exception = null;
                    this.Result = result;
                    this.State = ThreadDispatcherOperationState.Finished;

                    this.Dispatcher.RemoveKeepAlive(this);

                    this.OperationDone.Set();
                    this.OperationDoneTask.TrySetResult(this.Result);
                }
            }
        }

        private bool CancelInternal (bool hard)
        {
            lock (this.SyncRoot)
            {
                if (hard)
                {
                    if ((this.State != ThreadDispatcherOperationState.Waiting) && (this.State != ThreadDispatcherOperationState.Executing))
                    {
                        return false;
                    }
                }
                else
                {
                    if (this.State != ThreadDispatcherOperationState.Waiting)
                    {
                        return false;
                    }
                }

                this.Exception = null;
                this.Result = null;
                this.State = this.State == ThreadDispatcherOperationState.Executing ? ThreadDispatcherOperationState.Aborted : ThreadDispatcherOperationState.Canceled;

                this.Dispatcher.RemoveKeepAlive(this);

                this.OperationDone.Set();
                this.OperationDoneTask.TrySetCanceled();

                return true;
            }
        }

        private void EvaluateTask (out object result, out Exception exception, out bool canceled)
        {
            result = null;
            exception = this.Task.Exception;
            canceled = this.Task.IsCanceled;

            if (this.Task.Status == TaskStatus.RanToCompletion)
            {
                //TODO: Cache this to improve performance
                Type taskType = this.Task.GetType();
                PropertyInfo resultProperty = taskType.GetProperty(nameof(Task<object>.Result), BindingFlags.Instance | BindingFlags.Public);
                MethodInfo resultGetter = resultProperty?.GetGetMethod(false);
                result = resultGetter?.Invoke(this.Task, null);
            }
        }

        private bool ExecuteCore (out object result, out Exception exception, out bool canceled)
        {
            if (this.Stage == 0)
            {
                Type returnType = this.Action.Method.ReturnType;
                bool isTask = typeof(Task).IsAssignableFrom(returnType);

                if (isTask)
                {
                    this.Task = (Task)this.ExecuteCoreAction();

                    if (this.Task.IsCompleted)
                    {
                        this.EvaluateTask(out result, out exception, out canceled);
                        return true;
                    }
                    else
                    {
                        this.Stage = 1;

                        this.Task.ContinueWith((t, s) => { this.Dispatcher.EnqueueOperation(this); }, null, CancellationToken.None, TaskContinuationOptions.DenyChildAttach | TaskContinuationOptions.LazyCancellation | TaskContinuationOptions.RunContinuationsAsynchronously, this.Dispatcher.Scheduler);

                        result = null;
                        exception = null;
                        canceled = false;
                        return false;
                    }
                }
                else
                {
                    result = this.ExecuteCoreAction();
                    exception = null;
                    canceled = false;
                    return true;
                }
            }

            if (this.Stage == 1)
            {
                this.EvaluateTask(out result, out exception, out canceled);
                return true;
            }

            throw new InvalidOperationException("Invalid stage: " + this.Stage);
        }

        private object ExecuteCoreAction ()
        {
            if ((this.ExecutionContext != null) && (this.Options != ThreadDispatcherOptions.None))
            {
                return this.ExecutionContext.Run(this.Options, this.Action, this.Parameters);
            }
            else
            {
                return this.Action.DynamicInvoke(this.Parameters);
            }
        }

        #endregion




        #region Interface: ISynchronizable

        /// <inheritdoc />
        bool ISynchronizable.IsSynchronized => true;

        /// <inheritdoc />
        public object SyncRoot { get; }

        #endregion
    }
}
