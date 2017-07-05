using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Threading
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
		#region Static Methods

		internal static void Capture ()
		{
			//TODO: Implement
		}

		#endregion




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

			this.Dispatcher = dispatcher;
			this.ExecutionContext = executionContext;
			this.Priority = priority;
			this.Options = options;
			this.Action = action;
			this.Parameters = parameters;

			this.State = ThreadDispatcherOperationState.Waiting;
			this.Result = null;
			this.Exception = null;

			this.OperationDone = new ManualResetEvent(false);
			this.OperationDoneTask = new TaskCompletionSource<object>();

			//TODO: Use real
			ThreadDispatcherOperation.Capture();
		}

		/// <summary>
		///     Garbage collects this instance of <see cref="ThreadDispatcherOperation" />.
		/// </summary>
		~ThreadDispatcherOperation ()
		{
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
		///     true if <see cref="State" /> is anything else than <see cref="ThreadDispatcherOperationState.Waiting" />, false if <see cref="State" /> is <see cref="ThreadDispatcherOperationState.Waiting" />.
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

		internal Delegate Action { get; }
		internal ThreadDispatcher Dispatcher { get; }
		internal ThreadDispatcherExecutionContext ExecutionContext { get; set; }
		internal ThreadDispatcherOptions Options { get; }
		internal object[] Parameters { get; }
		internal int Priority { get; }

		private ManualResetEvent OperationDone { get; set; }
		private TaskCompletionSource<object> OperationDoneTask { get; set; }

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
		///         A dispatcher operation can only be canceled if its is still pending <see cref="State" /> is <see cref="ThreadDispatcherOperationState.Waiting" />).
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
				if (this.State != ThreadDispatcherOperationState.Waiting)
				{
					return;
				}

				this.State = ThreadDispatcherOperationState.Executing;
			}

			object result = null;
			Exception exception = null;

			if (Debugger.IsAttached)
			{
				result = this.ExecuteCore();
			}
			else
			{
				try
				{
					result = this.ExecuteCore();
				}
				catch (ThreadAbortException)
				{
					throw;
				}
				catch (Exception ex)
				{
					exception = ex;
				}
			}

			lock (this.SyncRoot)
			{
				if (exception == null)
				{
					this.OperationDone.Set();
					this.OperationDoneTask.TrySetResult(this.Result);

					this.Exception = null;
					this.Result = result;
					this.State = ThreadDispatcherOperationState.Finished;
				}
				else
				{
					this.OperationDone.Set();
					this.OperationDoneTask.TrySetException(this.Exception);

					this.Exception = exception;
					this.Result = null;
					this.State = ThreadDispatcherOperationState.Exception;
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

				this.OperationDone.Set();
				this.OperationDoneTask.TrySetCanceled();

				this.Exception = null;
				this.Result = null;
				this.State = ThreadDispatcherOperationState.Canceled;

				return true;
			}
		}

		private object ExecuteCore ()
		{
			//TODO: We might want to provide an async root with continuations on the associated dispatcher
			//Use the following flags: DenyChildAttach | LazyCancellation | RunContinuationsAsynchronously
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
