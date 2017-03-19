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
	/// TODO: Exception handling mode
	public sealed class ThreadDispatcher : ISynchronizable
	{
		private ThreadDispatcherShutdownMode _shutdownMode;

		private object SyncRoot { get; set; }
		private Thread Thread { get; set; }
		private Queue<ThreadDispatcherOperation> Queue { get; set; }
		private ManualResetEvent Posted { get; set; }

		/// <inheritdoc />
		bool ISynchronizable.IsSynchronized => true;

		/// <inheritdoc />
		object ISynchronizable.SyncRoot => this.SyncRoot;

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

		/// <summary>
		/// Gets whether the dispatcher is running.
		/// </summary>
		/// <value>
		/// true if the dispatcher is running, false otherwise.
		/// </value>
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

		/// <summary>
		///     Determines whether the caller of this function is executed inside the dispatchers thread (where <see cref="Run"/> is executed) or not.
		/// </summary>
		/// <returns>
		///     true if the caller of this function is executed inside this thread, false otherwise or if the dispatcher is not running.
		/// </returns>
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

		/// <summary>
		/// Gets the active shutdown mode.
		/// </summary>
		/// <value>
		/// <see cref="ThreadDispatcherShutdownMode.None"/> if the dispatcher is not running or is not being shut down, <see cref="ThreadDispatcherShutdownMode.FinishPending"/> if the dispatcher is shutting down and all already pending delegates are processed before the shutdown completes, <see cref="ThreadDispatcherShutdownMode.DiscardPending"/> if the dispatcher is shutting down and all already pending delegates are discarded.
		/// </value>
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

		/// <summary>
		/// Enqueues a delegate to the dispatchers queue and does not wait for its execution.
		/// </summary>
		/// <param name="action">The delegate.</param>
		/// <param name="parameters">Optional parameters of the delagate.</param>
		/// <returns>
		/// The dispatcher operation object which can be used to track the execution of the enqueued delegate.
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="action"/> is null.</exception>
		/// <exception cref="InvalidOperationException">The dispatcher is not running or is being shut down.</exception>
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

		/// <summary>
		/// Enqueues a delegate to the dispatchers queue and waits for its execution to be completed.
		/// </summary>
		/// <param name="action">The delegate.</param>
		/// <param name="parameters">Optional parameters of the delagate.</param>
		/// <returns>
		/// The return value of the executed delegate or null if the delegate has no return value. 
		/// </returns>
		/// <remarks>
		/// <para>
		/// <see cref="Send"/> blocks until all previously enqueued delegates were processed.
		/// </para>
		/// <para>
		/// <see cref="Send"/> can be called from the dispatchers thread.
		/// Therefore, <see cref="Send"/> calls can be cascaded.
		/// </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"><paramref name="action"/> is null.</exception>
		/// <exception cref="InvalidOperationException">The dispatcher is not running or is being shut down.</exception>
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

		/// <summary>
		/// Stops processing the delegate queue.
		/// </summary>
		/// <param name="finishPendingDelegates">Specifies whether already pending delegates should be processed before the dispatcher is shut down.</param>
		/// <exception cref="InvalidOperationException">The dispatcher is not running or it is already being shut down.</exception>
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