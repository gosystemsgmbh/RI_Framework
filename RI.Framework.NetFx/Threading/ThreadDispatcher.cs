using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

using RI.Framework.Collections;
using RI.Framework.Collections.Generic;
using RI.Framework.Utilities.ObjectModel;

namespace RI.Framework.Threading
{
	/// <summary>
	///     Implements a delegate dispatcher which can be run on any thread.
	/// </summary>
	/// <remarks>
	///     <para>
	///         A <see cref="ThreadDispatcher" /> provides a queue for delegates, filled through <see cref="Send(Delegate,object[])" />/<see cref="Send(int,Delegate,object[])" /> and <see cref="Post(Delegate,object[])" />/<see cref="Post(int,System.Delegate,object[])" />, which is processed on the thread where <see cref="Run" /> is called (<see cref="Run" /> blocks while executing the queue until <see cref="Shutdown" /> is called).
	///     </para>
	///     <para>
	///         The delegates are executed in the order they are added to the queue through <see cref="Send(Delegate,object[])" />/<see cref="Send(int,Delegate,object[])" /> or <see cref="Post(Delegate,object[])" />/<see cref="Post(int,System.Delegate,object[])" />.
	///         When all delegates are executed, or the queue is empty respectively, <see cref="ThreadDispatcher" /> waits for new delegates to process.
	///     </para>
	///     <para>
	///         During <see cref="Run" />, the current <see cref="SynchronizationContext" /> is replaced by an instance of <see cref="ThreadDispatcherSynchronizationContext" /> and restored afterwards.
	///     </para>
	///     <note type="important">
	///         Whether <see cref="ExecutionContext"/> and/or <see cref="CultureInfo"/> flows, depends on the used <see cref="ThreadDispatcherOptions"/>.
	///     </note>
	/// </remarks>
	/// <threadsafety static="true" instance="true" />
	public sealed class ThreadDispatcher : IThreadDispatcher
	{
		#region Constants

		/// <summary>
		///     The default value for <see cref="DefaultPriority" /> if it is not explicitly set.
		/// </summary>
		/// <remarks>
		///     <para>
		///         The default value is <c> int.MaxValue / 2 </c>.
		///     </para>
		/// </remarks>
		public const int DefaultPriorityValue = int.MaxValue / 2;

		/// <summary>
		///     The default value for <see cref="DefaultOptions" /> if it is not explicitly set.
		/// </summary>
		/// <remarks>
		///     <para>
		///         The default value is <see cref="ThreadDispatcherOptions.None"/>.
		///     </para>
		/// </remarks>
		public const ThreadDispatcherOptions DefaultOptionsValue = ThreadDispatcherOptions.None;

		#endregion




		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="ThreadDispatcher" />.
		/// </summary>
		public ThreadDispatcher ()
		{
			this.SyncRoot = new object();

			this.CatchExceptions = false;
			this.DefaultPriority = ThreadDispatcher.DefaultPriorityValue;
			this.DefaultOptions = ThreadDispatcher.DefaultOptionsValue;
			this.ShutdownMode = ThreadDispatcherShutdownMode.None;

			this.Thread = null;
			this.Queue = null;
			this.Posted = null;
			this.IdleSignals = null;
			this.CurrentPriority = null;
			this.CurrentOptions = null;
			this.CurrentOperation = null;

			this.Scheduler = new ThreadDispatcherTaskScheduler(this);
			this.Context = new ThreadDispatcherSynchronizationContext(this);

			this.PreRunQueue = new PriorityQueue<ThreadDispatcherOperation>();
			this.Finished = new ManualResetEvent(false);
			this.FinishedSignals = new List<TaskCompletionSource<object>>();
		}

		/// <summary>
		///     Garbage collects this instance of <see cref="ThreadDispatcher" />.
		/// </summary>
		~ThreadDispatcher ()
		{
			((IDisposable)this).Dispose();

			this.FinishedSignals?.ForEach(x => x.TrySetResult(null));
			this.FinishedSignals?.Clear();

			this.Finished?.Close();
		}

		#endregion




		#region Instance Fields

		private TaskScheduler _scheduler;
		private SynchronizationContext _context;

		private bool _catchExceptions;
		private int _defaultPriority;
		private ThreadDispatcherOptions _defaultOptions;
		private ThreadDispatcherShutdownMode _shutdownMode;

		#endregion




		#region Instance Properties/Indexer

		internal TaskScheduler Scheduler
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this._scheduler;
				}
			}
			private set
			{
				lock (this.SyncRoot)
				{
					this._scheduler = value;
				}
			}
		}
		internal SynchronizationContext Context
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this._context;
				}
			}
			private set
			{
				lock (this.SyncRoot)
				{
					this._context = value;
				}
			}
		}

		private PriorityQueue<ThreadDispatcherOperation> PreRunQueue { get; set; }
		private ManualResetEvent Finished { get; set; }
		private List<TaskCompletionSource<object>> FinishedSignals { get; set; }

		private Thread Thread { get; set; }
		private PriorityQueue<ThreadDispatcherOperation> Queue { get; set; }
		private ManualResetEvent Posted { get; set; }
		private List<TaskCompletionSource<object>> IdleSignals { get; set; }
		private Stack<int> CurrentPriority { get; set; }
		private Stack<ThreadDispatcherOptions> CurrentOptions { get; set; }
		private Stack<ThreadDispatcherOperation> CurrentOperation { get; set; }

		#endregion




		#region Instance Methods

		/// <summary>
		///     Processes the delegate queue or waits for new delegates until <see cref="Shutdown" /> is called.
		/// </summary>
		/// <exception cref="InvalidOperationException"> The dispatcher is already running. </exception>
		/// <exception cref="ThreadDispatcherException"> The execution of a delegate has thrown an exception and <see cref="CatchExceptions" /> is false. </exception>
		public void Run ()
		{
			SynchronizationContext synchronizationContextBackup = SynchronizationContext.Current;

			try
			{
				lock (this.SyncRoot)
				{
					this.VerifyNotRunning();

					synchronizationContextBackup = SynchronizationContext.Current;

					this.Thread = Thread.CurrentThread;
					this.Queue = new PriorityQueue<ThreadDispatcherOperation>();
					this.Posted = new ManualResetEvent(this.PreRunQueue.Count > 0);
					this.IdleSignals = new List<TaskCompletionSource<object>>();
					this.CurrentPriority = new Stack<int>();
					this.CurrentOptions = new Stack<ThreadDispatcherOptions>();
					this.CurrentOperation = new Stack<ThreadDispatcherOperation>();

					this.ShutdownMode = ThreadDispatcherShutdownMode.None;
					this.PreRunQueue.MoveTo(this.Queue);

					SynchronizationContext.SetSynchronizationContext(this.Context);

					this.Finished.Reset();
					this.FinishedSignals.Clear();
				}

				this.ExecuteFrame(null);
			}
			finally
			{
				lock (this.SyncRoot)
				{
					this.CurrentOperation?.ForEach(x => x.CancelHard());
					this.Queue?.ForEach(x => x.CancelHard());
					this.IdleSignals?.ForEach(x => x.TrySetResult(null));

					SynchronizationContext.SetSynchronizationContext(synchronizationContextBackup);

					this.PreRunQueue?.Clear();
					this.ShutdownMode = ThreadDispatcherShutdownMode.None;

					this.CurrentOperation?.Clear();
					this.CurrentPriority?.Clear();
					this.CurrentOptions?.Clear();
					this.IdleSignals?.Clear();
					this.Posted?.Close();
					this.Queue?.Clear();

					this.CurrentOperation = null;
					this.CurrentPriority = null;
					this.CurrentOptions = null;
					this.IdleSignals = null;
					this.Posted = null;
					this.Queue = null;
					this.Thread = null;

					this.Finished.Set();
					this.FinishedSignals.ForEach(x => x.TrySetResult(null));
					this.FinishedSignals.Clear();
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
								operationToCancel.Cancel();
							}
							this.Queue.Clear();
							this.SignalIdle();
							return;
						}

						if ((this.ShutdownMode == ThreadDispatcherShutdownMode.FinishPending) && (this.Queue.Count == 0))
						{
							this.SignalIdle();
							return;
						}

						if (this.Queue.Count > 0)
						{
							operation = this.Queue.Dequeue();
							this.CurrentOperation.Push(operation);
							this.CurrentPriority.Push(operation.Priority);
							this.CurrentOptions.Push(operation.Options);
						}
						else
						{
							this.SignalIdle();
						}
					}

					if (operation == null)
					{
						break;
					}

					operation.Execute();

					bool catchExceptions;

					lock (this.SyncRoot)
					{
						this.CurrentOptions.Pop();
						this.CurrentPriority.Pop();
						this.CurrentOperation.Pop();

						catchExceptions = this.CatchExceptions;
					}

					if (operation.Exception != null)
					{
						this.OnException(operation.Exception, catchExceptions);

						if (!catchExceptions)
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

		private void OnException (Exception exception, bool canContinue)
		{
			this.Exception?.Invoke(this, new ThreadDispatcherExceptionEventArgs(exception, canContinue));
		}

		private void SignalIdle ()
		{
			this.IdleSignals?.ForEach(x => x.TrySetResult(null));
			this.IdleSignals?.Clear();
		}

		private void VerifyNotRunning ()
		{
			if (this.IsRunning)
			{
				throw new InvalidOperationException(nameof(ThreadDispatcher) + " is already running.");
			}
		}

		private void VerifyNotShuttingDown ()
		{
			if (this.ShutdownMode != ThreadDispatcherShutdownMode.None)
			{
				throw new InvalidOperationException(nameof(ThreadDispatcher) + " is already shutting down.");
			}
		}

		private void VerifyNotFromDispatcher(string methodName)
		{
			if (this.IsInThread())
			{
				throw new InvalidOperationException(methodName + " cannot be called from inside the dispatcher.");
			}
		}

		private void VerifyRunning ()
		{
			if (!this.IsRunning)
			{
				throw new InvalidOperationException(nameof(ThreadDispatcher) + " is not running.");
			}
		}

		private void EnqueueInternal (ThreadDispatcherOperation operation)
		{
			lock (this.SyncRoot)
			{
				this.VerifyRunning();

				this.Queue.Enqueue(operation, operation.Priority);
				this.Posted.Set();
			}
		}

		#endregion




		#region Interface: IThreadDispatcher

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

		/// <inheritdoc />
		public int DefaultPriority
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this._defaultPriority;
				}
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException(nameof(value));
				}

				lock (this.SyncRoot)
				{
					this._defaultPriority = value;
				}
			}
		}

		/// <inheritdoc />
		public ThreadDispatcherOptions DefaultOptions
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this._defaultOptions;
				}
			}
			set
			{
				lock (this.SyncRoot)
				{
					this._defaultOptions = value;
				}
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
		public bool IsShuttingDown
		{
			get
			{
				lock (this.SyncRoot)
				{
					if (this.Thread == null)
					{
						return false;
					}

					return this.ShutdownMode != ThreadDispatcherShutdownMode.None;
				}
			}
		}

		/// <inheritdoc />
		bool ISynchronizable.IsSynchronized => true;

		/// <inheritdoc />
		public ThreadDispatcherShutdownMode ShutdownMode
		{
			get
			{
				lock (this.SyncRoot)
				{
					if (this.Thread == null)
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
		public object SyncRoot { get; }

		/// <inheritdoc />
		public event EventHandler<ThreadDispatcherExceptionEventArgs> Exception;

		/// <inheritdoc />
		void IDisposable.Dispose ()
		{
			lock (this.SyncRoot)
			{
				if (this.IsRunning && (!this.IsShuttingDown))
				{
					this.BeginShutdown(false);
				}

				this.PreRunQueue?.ForEach(x => x.CancelHard());
				this.PreRunQueue?.Clear();
			}
		}

		/// <inheritdoc />
		public void DoProcessing ()
		{
			bool isInThread;
			ThreadDispatcherOperation operation = null;

			while (true)
			{
				lock (this.SyncRoot)
				{
					if (operation == null)
					{
						this.VerifyRunning();
					}
					else if (!this.IsRunning)
					{
						return;
					}

					if ((this.Queue.Count == 0) && (this.CurrentOperation.Count == 0))
					{
						return;
					}

					isInThread = this.IsInThread();
					operation = this.Post(0, new Action(() => { }));
				}

				if (isInThread)
				{
					this.ExecuteFrame(operation);
				}
				else
				{
					operation.Wait();
				}
			}
		}

		/// <inheritdoc />
		public async Task DoProcessingAsync ()
		{
			ThreadDispatcherOperation operation = null;

			while (true)
			{
				lock (this.SyncRoot)
				{
					if (operation == null)
					{
						this.VerifyRunning();
					}
					else if (!this.IsRunning)
					{
						return;
					}

					if ((this.Queue.Count == 0) && (this.CurrentOperation.Count == 0))
					{
						return;
					}

					operation = this.Post(0, new Action(() => { }));
				}

				await operation.WaitAsync().ConfigureAwait(false);
			}
		}

		/// <inheritdoc />
		public void DoProcessing(int priority)
		{
			if (priority < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(priority));
			}

			bool isInThread;
			ThreadDispatcherOperation operation = null;

			while (true)
			{
				lock (this.SyncRoot)
				{
					if (operation == null)
					{
						this.VerifyRunning();
					}
					else if (!this.IsRunning)
					{
						return;
					}

					if ((this.Queue.Count == 0) && (this.CurrentOperation.Count == 0))
					{
						return;
					}

					if (this.Queue.HighestPriority < priority)
					{
						return;
					}

					isInThread = this.IsInThread();
					operation = this.Post(priority, new Action(() => { }));
				}

				if (isInThread)
				{
					this.ExecuteFrame(operation);
				}
				else
				{
					operation.Wait();
				}
			}
		}

		/// <inheritdoc />
		public async Task DoProcessingAsync(int priority)
		{
			if (priority < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(priority));
			}

			ThreadDispatcherOperation operation = null;

			while (true)
			{
				lock (this.SyncRoot)
				{
					if (operation == null)
					{
						this.VerifyRunning();
					}
					else if (!this.IsRunning)
					{
						return;
					}

					if ((this.Queue.Count == 0) && (this.CurrentOperation.Count == 0))
					{
						return;
					}

					if (this.Queue.HighestPriority < priority)
					{
						return;
					}

					operation = this.Post(priority, new Action(() => { }));
				}

				await operation.WaitAsync().ConfigureAwait(false);
			}
		}

		/// <inheritdoc />
		public int? GetCurrentPriority ()
		{
			lock (this.SyncRoot)
			{
				if (!this.IsInThread())
				{
					return null;
				}

				if (this.CurrentPriority == null)
				{
					return null;
				}

				if (this.CurrentPriority.Count == 0)
				{
					return null;
				}

				return this.CurrentPriority.Peek();
			}
		}

		/// <inheritdoc />
		public ThreadDispatcherOptions? GetCurrentOptions()
		{
			lock (this.SyncRoot)
			{
				if (!this.IsInThread())
				{
					return null;
				}

				if (this.CurrentOptions == null)
				{
					return null;
				}

				if (this.CurrentOptions.Count == 0)
				{
					return null;
				}

				return this.CurrentOptions.Peek();
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
		public ThreadDispatcherOperation Post (Delegate action, params object[] parameters)
		{
			return this.Post(this.DefaultPriority, this.DefaultOptions, action, parameters);
		}

		/// <inheritdoc />
		public ThreadDispatcherOperation Post (int priority, Delegate action, params object[] parameters)
		{
			return this.Post(priority, this.DefaultOptions, action, parameters);
		}

		/// <inheritdoc />
		public ThreadDispatcherOperation Post(int priority, ThreadDispatcherOptions options, Delegate action, params object[] parameters)
		{
			if (priority < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(priority));
			}

			if (action == null)
			{
				throw new ArgumentNullException(nameof(action));
			}

			parameters = parameters ?? new object[0];

			lock (this.SyncRoot)
			{
				this.VerifyNotShuttingDown();

				ThreadDispatcherOperation operation = new ThreadDispatcherOperation(this, priority, options, action, parameters);

				if (this.IsRunning)
				{
					this.EnqueueInternal(operation);
				}
				else
				{
					this.PreRunQueue.Enqueue(operation, priority);
				}

				return operation;
			}
		}

		/// <inheritdoc />
		public object Send (Delegate action, params object[] parameters)
		{
			return this.Send(this.DefaultPriority, this.DefaultOptions, action, parameters);
		}

		/// <inheritdoc />
		public object Send (int priority, Delegate action, params object[] parameters)
		{
			return this.Send(priority, this.DefaultOptions, action, parameters);
		}

		/// <inheritdoc />
		public object Send(int priority, ThreadDispatcherOptions options, Delegate action, params object[] parameters)
		{
			if (priority < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(priority));
			}

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
				operation = this.Post(priority, options, action, parameters);
			}

			if (isInThread)
			{
				this.ExecuteFrame(operation);
			}
			else
			{
				operation.Wait();
			}

			if (operation.Exception != null)
			{
				throw new ThreadDispatcherException(operation.Exception);
			}

			if (operation.State == ThreadDispatcherOperationState.Canceled)
			{
				throw new OperationCanceledException();
			}

			return operation.Result;
		}

		/// <inheritdoc />
		public async Task<object> SendAsync (Delegate action, params object[] parameters)
		{
			return await this.SendAsync(this.DefaultPriority, this.DefaultOptions, action, parameters).ConfigureAwait(false);
		}

		/// <inheritdoc />
		public async Task<object> SendAsync (int priority, Delegate action, params object[] parameters)
		{
			return await this.SendAsync(priority, this.DefaultOptions, action, parameters).ConfigureAwait(false);
		}

		/// <inheritdoc />
		public async Task<object> SendAsync(int priority, ThreadDispatcherOptions options, Delegate action, params object[] parameters)
		{
			if (priority < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(priority));
			}

			if (action == null)
			{
				throw new ArgumentNullException(nameof(action));
			}

			parameters = parameters ?? new object[0];

			ThreadDispatcherOperation operation;

			lock (this.SyncRoot)
			{
				this.VerifyRunning();
				this.VerifyNotShuttingDown();

				operation = this.Post(priority, options, action, parameters);
			}

			try
			{
				await operation.WaitAsync().ConfigureAwait(false);
			}
			catch (TaskCanceledException)
			{
				throw new OperationCanceledException();
			}
			catch (Exception ex)
			{
				throw new ThreadDispatcherException(ex);
			}

			if (operation.Exception != null)
			{
				throw new ThreadDispatcherException(operation.Exception);
			}

			if (operation.State == ThreadDispatcherOperationState.Canceled)
			{
				throw new OperationCanceledException();
			}

			return operation.Result;
		}

		/// <inheritdoc />
		public void BeginShutdown (bool finishPendingDelegates)
		{
			lock (this.SyncRoot)
			{
				this.VerifyRunning();
				this.VerifyNotShuttingDown();

				this.ShutdownMode = finishPendingDelegates ? ThreadDispatcherShutdownMode.FinishPending : ThreadDispatcherShutdownMode.DiscardPending;
				this.Posted.Set();
			}
		}

		/// <inheritdoc />
		public void Shutdown (bool finishPendingDelegates)
		{
			ManualResetEvent finishedEvent;

			lock (this.SyncRoot)
			{
				this.VerifyRunning();
				this.VerifyNotShuttingDown();
				this.VerifyNotFromDispatcher(nameof(this.Shutdown));

				finishedEvent = this.Finished;

				this.BeginShutdown(finishPendingDelegates);
			}

			finishedEvent.WaitOne();
		}

		/// <inheritdoc />
		public async Task ShutdownAsync (bool finishPendingDelegates)
		{
			Task finishTask;

			lock (this.SyncRoot)
			{
				this.VerifyRunning();
				this.VerifyNotShuttingDown();
				this.VerifyNotFromDispatcher(nameof(this.ShutdownAsync));

				this.BeginShutdown(finishPendingDelegates);

				TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
				this.FinishedSignals.Add(tcs);
				finishTask = tcs.Task;
			}

			await finishTask.ConfigureAwait(false);
		}

		#endregion
	}
}
