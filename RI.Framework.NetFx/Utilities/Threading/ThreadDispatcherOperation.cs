using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Utilities.Threading
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

		internal ThreadDispatcherOperation (ThreadDispatcher dispatcher, Delegate action, object[] parameters)
		{
			this.SyncRoot = new object();

			this.Dispatcher = dispatcher;
			this.Action = action;
			this.Parameters = parameters;

			this.State = ThreadDispatcherOperationState.Waiting;
			this.Result = null;
			this.Exception = null;

			this.OperationDone = new ManualResetEvent(false);
			this.OperationDoneTask = new TaskCompletionSource<object>();
		}

		/// <summary>
		///     Garbage collects this instance of <see cref="ThreadDispatcherOperation" />.
		/// </summary>
		~ThreadDispatcherOperation ()
		{
			this.OperationDone?.Close();
			this.OperationDone = null;
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

		private Delegate Action { get; }

		private ThreadDispatcher Dispatcher { get; }

		private ManualResetEvent OperationDone { get; set; }
		private TaskCompletionSource<object> OperationDoneTask { get; set; }
		private object[] Parameters { get; }

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
			lock (this.SyncRoot)
			{
				if (this.State != ThreadDispatcherOperationState.Waiting)
				{
					return false;
				}

				this.State = ThreadDispatcherOperationState.Canceled;
				this.Exception = null;
				this.Result = null;

				this.OperationDone.Set();
				this.OperationDoneTask.SetCanceled();

				return true;
			}
		}

		/// <summary>
		///     Waits indefinitely for the dispatcher operation to finish processing.
		/// </summary>
		public void Wait ()
		{
			this.Wait(Timeout.Infinite);
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
			return this.Wait((int)timeout.TotalMilliseconds);
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
		public Task WaitAsync ()
		{
			return this.WaitAsync(Timeout.Infinite);
		}

		/// <summary>
		///     Waits indefinitely for the dispatcher operation to finish processing.
		/// </summary>
		/// <param name="cancellationToken"> The cancellation token which can be used to cancel the wait. </param>
		/// <returns>
		///     true if the dispatcher operation finished processing, false if the wait was cancelled.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="cancellationToken" /> is null. </exception>
		public Task<bool> WaitAsync (CancellationToken cancellationToken)
		{
			return this.WaitAsync(Timeout.Infinite, cancellationToken);
		}

		/// <summary>
		///     Waits a specified amount of time for the dispatcher operation to finish processing.
		/// </summary>
		/// <param name="timeout"> The maximum time to wait for the dispatcher operation to finish processing before the method returns. </param>
		/// <returns>
		///     true if the dispatcher operation finished processing within the specified timeout, false otherwise.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException"> <paramref name="timeout" /> is negative. </exception>
		public Task<bool> WaitAsync (TimeSpan timeout)
		{
			return this.WaitAsync((int)timeout.TotalMilliseconds);
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
		public Task<bool> WaitAsync (TimeSpan timeout, CancellationToken cancellationToken)
		{
			return this.WaitAsync((int)timeout.TotalMilliseconds, cancellationToken);
		}

		/// <summary>
		///     Waits a specified amount of time for the dispatcher operation to finish processing.
		/// </summary>
		/// <param name="milliseconds"> The maximum time in milliseconds to wait for the dispatcher operation to finish processing before the method returns. </param>
		/// <returns>
		///     true if the dispatcher operation finished processing within the specified timeout, false otherwise.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException"> <paramref name="milliseconds" /> is negative. </exception>
		public Task<bool> WaitAsync (int milliseconds)
		{
			return this.WaitAsync(milliseconds, CancellationToken.None);
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
		public Task<bool> WaitAsync (int milliseconds, CancellationToken cancellationToken)
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
				return Task.FromResult(true);
			}

			Task operationTask = this.OperationDoneTask.Task;
			Task timeoutTask = Task.Delay(milliseconds, cancellationToken);

			Task<Task> completed = Task.WhenAny(operationTask, timeoutTask);
			Task<bool> result = completed.ContinueWith((task, state) => object.ReferenceEquals(state, task.Result), operationTask);

			return result;
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
				result = this.Action.DynamicInvoke(this.Parameters);
			}
			else
			{
				try
				{
					result = this.Action.DynamicInvoke(this.Parameters);
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
					this.State = ThreadDispatcherOperationState.Finished;
					this.Exception = null;
					this.Result = result;

					this.OperationDone.Set();
					this.OperationDoneTask.SetResult(this.Result);
				}
				else
				{
					this.State = ThreadDispatcherOperationState.Exception;
					this.Exception = exception;
					this.Result = null;

					this.OperationDone.Set();
					this.OperationDoneTask.SetException(this.Exception);
				}
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
