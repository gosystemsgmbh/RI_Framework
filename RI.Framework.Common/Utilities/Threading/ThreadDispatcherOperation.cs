using System;
using System.Threading;

using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Utilities.Threading
{
	/// <summary>
	/// Used to track <see cref="ThreadDispatcher"/> operations or the processing of enqueued delegates respectively.
	/// </summary>
	/// <remarks>
	/// <para>
	/// See <see cref="ThreadDispatcher"/> for more information.
	/// </para>
	/// </remarks>
	public sealed class ThreadDispatcherOperation : ISynchronizable
	{
		private ThreadDispatcherOperationState _state;
		private object _result;
		private Exception _exception;

		private object SyncRoot { get; set; }
		private ThreadDispatcher Dispatcher { get; set; }
		private Delegate Action { get; set; }
		private object[] Parameters { get; set; }
		private ManualResetEvent OperationDone { get; set; }

		/// <inheritdoc />
		bool ISynchronizable.IsSynchronized => true;

		/// <inheritdoc />
		object ISynchronizable.SyncRoot => this.SyncRoot;

		internal ThreadDispatcherOperation(ThreadDispatcher dispatcher, Delegate action, object[] parameters)
		{
			this.SyncRoot = new object();

			this.Dispatcher = dispatcher;
			this.Action = action;
			this.Parameters = parameters;

			this.State = ThreadDispatcherOperationState.Waiting;
			this.Result = null;
			this.Exception = null;
			this.OperationDone = new ManualResetEvent(false);
		}

		/// <summary>
		///     Garbage collects this instance of <see cref="ThreadDispatcherOperation" />.
		/// </summary>
		~ThreadDispatcherOperation()
		{
			this.OperationDone?.Close();
			this.OperationDone = null;
		}

		internal void Execute()
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
			try
			{
				result = this.Action.DynamicInvoke(this.Parameters);
			}
			catch (Exception exception)
			{
				this.Exception = exception;
			}

			lock (this.SyncRoot)
			{
				this.State = this.Exception == null ? ThreadDispatcherOperationState.Finished : ThreadDispatcherOperationState.Exception;
				this.Result = result;
				this.OperationDone.Set();
			}
		}

		/// <summary>
		/// Gets whether the dispatcher operation has finished processing.
		/// </summary>
		/// <value>
		/// true if <see cref="State"/> is anything else than <see cref="ThreadDispatcherOperationState.Waiting"/>, false if <see cref="State"/> is <see cref="ThreadDispatcherOperationState.Waiting"/>.
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
		/// Gets the current state of the dispatcher operation.
		/// </summary>
		/// <value>
		/// The current state of the dispatcher operation.
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
		/// Gets the exception which occurred during execution of the delegate associated with this dispatcher operation.
		/// </summary>
		/// <value>
		/// The exception which occurred during execution or null if no exception was thrown or the operation was not yet processed.
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
		/// Gets the value returned by the delegate associated with this dispatcher operation.
		/// </summary>
		/// <value>
		/// The value returned by the delegate associated with this dispatcher operation or null if the delegate has no return value or the operation was not yet processed.
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
		/// Waits indefinitely for the dispatcher operation to finish processing.
		/// </summary>
		public void Wait()
		{
			this.Wait(Timeout.Infinite);
		}

		/// <summary>
		/// Waits a specified amount of time for the dispatcher operation to finish processing.
		/// </summary>
		/// <param name="timeout">The maximum time to wait for the dispatcher operation to finish processing before the method returns.</param>
		/// <returns>
		/// true if the dispatcher operation finished processing within the specified timeout, false otherwise.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="timeout"/> is negative.</exception>
		public bool Wait(TimeSpan timeout)
		{
			return this.Wait((int)timeout.TotalMilliseconds);
		}

		/// <summary>
		/// Waits a specified amount of time for the dispatcher operation to finish processing.
		/// </summary>
		/// <param name="milliseconds">The maximum time in milliseconds to wait for the dispatcher operation to finish processing before the method returns.</param>
		/// <returns>
		/// true if the dispatcher operation finished processing within the specified timeout, false otherwise.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="milliseconds"/> is negative.</exception>
		public bool Wait(int milliseconds)
		{
			if ((milliseconds < 0) && (milliseconds != Timeout.Infinite))
			{
				throw new ArgumentOutOfRangeException(nameof(milliseconds));
			}

			return this.OperationDone.WaitOne(milliseconds);
		}

		/// <summary>
		/// Cancels the processing of the dispatcher operation.
		/// </summary>
		/// <returns>
		/// true if the operation could be canceled, false otherwise.
		/// </returns>
		/// <remarks>
		/// <para>
		/// A dispatcher operation can only be canceled if its is still pending <see cref="State"/> is <see cref="ThreadDispatcherOperationState.Waiting"/>).
		/// </para>
		/// </remarks>
		public bool Cancel()
		{
			lock (this.SyncRoot)
			{
				if (this.State != ThreadDispatcherOperationState.Waiting)
				{
					return false;
				}

				this.State = ThreadDispatcherOperationState.Canceled;
				this.Result = null;
				this.OperationDone.Set();

				return true;
			}
		}
	}
}