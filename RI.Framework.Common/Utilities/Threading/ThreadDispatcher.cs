using System;
using System.Collections.Generic;
using System.Threading;




namespace RI.Framework.Utilities.Threading
{
	//TODO: IThreadDispatcher
	//TODO: ThreadDispatcherTimer
	//TODO: MultiThreadDispatcher
	//TODO: HeavyThreadDispatcher

	//TODO: ISynchronizable
	//TODO: Destructor
	//TODO: Use priority queue
	//TODO: Add timeout functionality
	public sealed class ThreadDispatcher
	{
		private ThreadDispatcherShutdownMode _shutdownMode;

		private object SyncRoot { get; set; }
		private Thread Thread { get; set; }
		private Queue<ThreadDispatcherOperation> Queue { get; set; }
		private ManualResetEvent Posted { get; set; }


		public ThreadDispatcher()
		{
			this.SyncRoot = new object();

			this.Thread = null;
			this.Queue = null;
			this.Posted = null;
			
			this.ShutdownMode = ThreadDispatcherShutdownMode.None;
		}

		public void Run()
		{
			try
			{
				lock (this.SyncRoot)
				{
					this.VerifyNotRunning();

					this.Thread = Thread.CurrentThread;
					this.Queue = new Queue<ThreadDispatcherOperation>();
					this.Posted = new ManualResetEvent(false);
					this.ShutdownMode = ThreadDispatcherShutdownMode.None;
				}

				this.ExecuteFrame(null);
			}
			finally
			{
				lock (this.SyncRoot)
				{
					this.Posted?.Close();
					this.Queue?.Clear();

					this.Posted = null;
					this.Queue = null;
					this.Thread = null;
				}
			}
		}

		private void ExecuteFrame (ThreadDispatcherOperation returnTrigger)
		{
			while (true)
			{
				this.Posted.WaitOne();

				lock (this.SyncRoot)
				{
					this.Posted.Reset();
				}

				while (true)
				{
					ThreadDispatcherOperation operation = null;

					lock (this.SyncRoot)
					{
						if (this.ShutdownMode == ThreadDispatcherShutdownMode.DiscardPending)
						{
							foreach (ThreadDispatcherOperation operationToCancel in this.Queue)
							{
								if (operationToCancel.State == ThreadDispatcherOperationState.Waiting)
								{
									operationToCancel.Cancel();
								}
							}
							this.Queue.Clear();
							return;
						}

						if ((this.ShutdownMode == ThreadDispatcherShutdownMode.FinishPending) && (this.Queue.Count == 0))
						{
							return;
						}

						if (this.Queue.Count > 0)
						{
							operation = this.Queue.Dequeue();
						}
					}

					if (operation == null)
					{
						break;
					}

					operation.Execute();

					//TODO: Exception handling

					if (object.ReferenceEquals(operation, returnTrigger))
					{
						return;
					}
				}
			}
		}

		private void VerifyRunning()
		{
			if (!this.IsRunning)
			{
				throw new InvalidOperationException(nameof(ThreadDispatcher) + " is not running.");
			}
		}

		private void VerifyNotRunning()
		{
			if (this.IsRunning)
			{
				throw new InvalidOperationException(nameof(ThreadDispatcher) + " is already running.");
			}
		}

		private void VerifyShuttingDown()
		{
			if (this.ShutdownMode == ThreadDispatcherShutdownMode.None)
			{
				throw new InvalidOperationException(nameof(ThreadDispatcher) + " is not shutting down.");
			}
		}

		private void VerifyNotShuttingDown()
		{
			if (this.ShutdownMode != ThreadDispatcherShutdownMode.None)
			{
				throw new InvalidOperationException(nameof(ThreadDispatcher) + " is already shutting down.");
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

		public ThreadDispatcherShutdownMode ShutdownMode
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

		public ThreadDispatcherOperation Post (Delegate action, params object[] parameters)
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

				ThreadDispatcherOperation operation = new ThreadDispatcherOperation(this, action, parameters);
				this.Queue.Enqueue(operation);
				this.Posted.Set();
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
			ThreadDispatcherOperation operation;

			lock (this.SyncRoot)
			{
				this.VerifyRunning();
				this.VerifyNotShuttingDown();

				isInThread = this.IsInThread;
				operation = this.Post(action, parameters);
			}

			if (isInThread)
			{
				this.ExecuteFrame(operation);
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

				this.ShutdownMode = finishPendingMessages ? ThreadDispatcherShutdownMode.FinishPending : ThreadDispatcherShutdownMode.DiscardPending;
				this.Posted.Set();
			}
		}
	}
}