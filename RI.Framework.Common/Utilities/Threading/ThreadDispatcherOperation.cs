using System;
using System.Threading;




namespace RI.Framework.Utilities.Threading
{
	//TODO: ISynchronizable
	//TODO: Destructor
	//TODO: Add priority
	//TODO: Add timeout
	public sealed class ThreadDispatcherOperation
	{
		private ThreadDispatcherOperationState _state;
		private object _result;
		private Exception _exception;

		private object SyncRoot { get; set; }
		private ThreadDispatcher Dispatcher { get; set; }
		private Delegate Action { get; set; }
		private object[] Parameters { get; set; }
		private ManualResetEvent OperationDone { get; set; }

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

		public void Wait()
		{
			this.Wait(Timeout.Infinite);
		}

		public bool Wait(TimeSpan timeout)
		{
			return this.Wait((int)timeout.TotalMilliseconds);
		}

		public bool Wait(int milliseconds)
		{
			if ((milliseconds < 0) && (milliseconds != Timeout.Infinite))
			{
				throw new ArgumentOutOfRangeException(nameof(milliseconds));
			}

			return this.OperationDone.WaitOne(milliseconds);
		}

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