using System;
using System.Collections;
using System.Threading;




namespace RI.Framework.Utilities.Windows
{
	//TODO: IThreadDispatcher

	//TODO: ISynchronizable
	//TODO: Destructor
	//TODO: Use priority queue
	//TODO: Add timeout functionality
	public sealed class WindowsMessageLoop
	{
		private WindowsMessageLoopShutdownMode _shutdownMode;

		private object SyncRoot { get; set; }
		private Thread Thread { get; set; }
		private uint ThreadId { get; set; }
		private Hashtable Queue { get; set; }
		private int LastInvokeHandle { get; set; }

		public WindowsMessageLoop()
		{
			this.SyncRoot = new object();

			this.Thread = null;
			this.ThreadId = 0;
			this.Queue = null;
			this.LastInvokeHandle = 0;

			this.ShutdownMode = WindowsMessageLoopShutdownMode.None;
		}

		public void Run ()
		{
			try
			{
				lock (this.SyncRoot)
				{
					this.VerifyNotRunning();

					this.Thread = Thread.CurrentThread;
					this.ThreadId = WindowsMessage.GetCurrentThreadId();
					this.Queue = new Hashtable();
					this.LastInvokeHandle = 0;
					this.ShutdownMode = WindowsMessageLoopShutdownMode.None;
				}

				while (true)
				{
					WindowsMessage message = WindowsMessage.GetMessage();

					message = message.Translate();
					message.Dispatch();

					//TODO: Events etc.
					//TODO: Observe shutdown mode
				}
			}
			finally
			{
				lock (this.SyncRoot)
				{
					this.Queue?.Clear();

					this.Thread = null;
					this.ThreadId = 0;
					this.Queue = null;
					this.LastInvokeHandle = 0;
				}
			}
		}

		private void VerifyRunning()
		{
			if (!this.IsRunning)
			{
				throw new InvalidOperationException(nameof(WindowsMessageLoop) + " is not running.");
			}
		}

		private void VerifyNotRunning()
		{
			if (this.IsRunning)
			{
				throw new InvalidOperationException(nameof(WindowsMessageLoop) + " is already running.");
			}
		}

		private void VerifyShuttingDown()
		{
			if (this.ShutdownMode == WindowsMessageLoopShutdownMode.None)
			{
				throw new InvalidOperationException(nameof(WindowsMessageLoop) + " is not shutting down.");
			}
		}

		private void VerifyNotShuttingDown()
		{
			if (this.ShutdownMode != WindowsMessageLoopShutdownMode.None)
			{
				throw new InvalidOperationException(nameof(WindowsMessageLoop) + " is already shutting down.");
			}
		}

		public bool IsRunning
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this.Thread != null;
				}
			}
		}

		public bool IsInThread
		{
			get
			{
				lock (this.SyncRoot)
				{
					this.VerifyRunning();

					return this.Thread.ManagedThreadId == Thread.CurrentThread.ManagedThreadId;
				}
			}
		}

		internal uint NativeThreadId
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this.ThreadId;
				}
			}
		}

		public WindowsMessageLoopShutdownMode ShutdownMode
		{
			get
			{
				lock (this.SyncRoot)
				{
					this.VerifyRunning();

					return this._shutdownMode;
				}
			}
			private set
			{
				lock (this.SyncRoot)
				{
					this._shutdownMode = value;
				}
			}
		}

		public WindowsMessageLoopOperation Post (Delegate action, params object[] parameters)
		{
			if (action == null)
			{
				throw new ArgumentNullException(nameof(action));
			}

			parameters = parameters ?? new object[0];

			lock (this.SyncRoot)
			{
				this.VerifyRunning();
				this.VerifyNotShuttingDown();

				this.LastInvokeHandle++;
				int handle = this.LastInvokeHandle;
				WindowsMessageLoopOperation operation = new WindowsMessageLoopOperation(this, handle, action, parameters);
				this.Queue.Add(handle, operation);
				return operation;
			}
		}

		public object Send (Delegate action, params object[] parameters)
		{
			if (action == null)
			{
				throw new ArgumentNullException(nameof(action));
			}

			parameters = parameters ?? new object[0];

			bool isInThread;
			WindowsMessageLoopOperation operation;

			lock (this.SyncRoot)
			{
				this.VerifyRunning();
				this.VerifyNotShuttingDown();

				isInThread = this.IsInThread;
				operation = this.Post(action, parameters);
			}

			if (isInThread)
			{
				//TODO
				//this.ExecuteFrame(operation);
			}
			else
			{
				operation.Wait();
			}

			return operation.Result;
		}

		public void Shutdown (bool finishPendingMessages)
		{
			lock (this.SyncRoot)
			{
				this.VerifyRunning();
				this.VerifyNotShuttingDown();

				this.ShutdownMode = finishPendingMessages ? WindowsMessageLoopShutdownMode.FinishPending : WindowsMessageLoopShutdownMode.DiscardPending;
				//TODO
			}
		}
	}

	//TODO: ISynchronizable
	//TODO: Destructor
	//TODO: Add priority
	//TODO: Add timeout
}