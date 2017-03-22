using System;
using System.Collections.Generic;
using System.Threading;

using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Utilities.Threading
{
	/// <summary>
	/// Implements a delegate dispatcher which can be run on any thread.
	/// </summary>
	/// <remarks>
	/// <para>
	/// A <see cref="ThreadDispatcher"/> provides a queue for delegates, filled through <see cref="Send"/> and <see cref="Post"/>, which is processed on the thread where <see cref="Run"/> is called (<see cref="Run"/> blocks while executing the queue until <see cref="Shutdown"/> is called).
	/// </para>
	/// <para>
	/// The delegates are executed in the order they are added to the queue through <see cref="Send"/> or <see cref="Post"/>.
	/// When all delegates are executed, or the queue is empty respectively, <see cref="ThreadDispatcher"/> waits for new delegates to process.
	/// </para>
	/// </remarks>
	public sealed class ThreadDispatcher : IThreadDispatcher, ISynchronizable
	{
		private ThreadDispatcherShutdownMode _shutdownMode;
		private bool _catchExceptions;

		private object SyncRoot { get; set; }
		private Thread Thread { get; set; }
		private Queue<ThreadDispatcherOperation> Queue { get; set; }
		private ManualResetEvent Posted { get; set; }

		/// <inheritdoc />
		bool ISynchronizable.IsSynchronized => true;

		/// <inheritdoc />
		object ISynchronizable.SyncRoot => this.SyncRoot;

		/// <inheritdoc />
		public event EventHandler<ThreadDispatcherExceptionEventArgs> Exception;

		private void OnException (Exception exception, bool canContinue)
		{
			this.Exception?.Invoke(this, new ThreadDispatcherExceptionEventArgs(exception, canContinue));
		}

		/// <inheritdoc />
		public bool CatchExceptions
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this._catchExceptions;
				}
			}
			set
			{
				lock (this.SyncRoot)
				{
					this._catchExceptions = value;
				}
			}
		}

		/// <summary>
		/// Creates a new instance of <see cref="ThreadDispatcher"/>.
		/// </summary>
		public ThreadDispatcher()
		{
			this.SyncRoot = new object();

			this.Thread = null;
			this.Queue = null;
			this.Posted = null;
			
			this.ShutdownMode = ThreadDispatcherShutdownMode.None;

			this.CatchExceptions = false;
		}

		/// <summary>
		///     Garbage collects this instance of <see cref="ThreadDispatcher" />.
		/// </summary>
		~ThreadDispatcher ()
		{
			this.Posted?.Close();
			this.Posted = null;

			this.Queue?.Clear();
			this.Queue = null;
		}

		/// <summary>
		/// Processes the delegate queue or waits for new delegates until <see cref="Shutdown"/> is called.
		/// </summary>
		/// <exception cref="InvalidOperationException">The dispatcher is already running.</exception>
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

					if (operation.State == ThreadDispatcherOperationState.Exception)
					{
						bool canContinue = this.CatchExceptions;

						this.OnException(operation.Exception, canContinue);

						if (!canContinue)
						{
							throw new ThreadDispatcherException(operation.Exception);
						}
					}
					
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

		/// <inheritdoc />
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

		/// <inheritdoc />
		public bool IsInThread ()
		{
			lock (this.SyncRoot)
			{
				if (this.Thread == null)
				{
					return false;
				}

				return this.Thread.ManagedThreadId == Thread.CurrentThread.ManagedThreadId;
			}
		}

		/// <inheritdoc />
		public ThreadDispatcherShutdownMode ShutdownMode
		{
			get
			{
				lock (this.SyncRoot)
				{
					if (!this.IsRunning)
					{
						return ThreadDispatcherShutdownMode.None;
					}

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

		/// <inheritdoc />
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

		/// <inheritdoc />
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

				isInThread = this.IsInThread();
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

		/// <inheritdoc />
		public void DoProcessing ()
		{
			this.Send(new Action(() => { }));
		}

		/// <inheritdoc />
		public void Shutdown (bool finishPendingDelegates)
		{
			lock (this.SyncRoot)
			{
				this.VerifyRunning();
				this.VerifyNotShuttingDown();

				this.ShutdownMode = finishPendingDelegates ? ThreadDispatcherShutdownMode.FinishPending : ThreadDispatcherShutdownMode.DiscardPending;
				this.Posted.Set();
			}
		}
	}
}