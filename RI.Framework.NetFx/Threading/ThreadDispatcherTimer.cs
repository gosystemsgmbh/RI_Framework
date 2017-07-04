using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Threading
{
	/// <summary>
	///     Implements a timer which can be used with <see cref="IThreadDispatcher" />.
	/// </summary>
	/// <remarks>
	///     <para>
	///         <see cref="ThreadDispatcherTimer" /> enqueues a delegate to the specified dispatchers queue (using <see cref="IThreadDispatcher.Post(int,ThreadDispatcherOptions,Delegate,object[])" />) in a specified interval.
	///     </para>
	///     <para>
	///         The interval is awaited before the timer is executed for the first time. Afterwards, the delegate is posted to the dispatcher in the specified interval.
	///         The interval is strictly observed.
	///         This means that if a previous interval has not yet finished executing the delegate, it is not dispatched for execution until the next interval.
	///     </para>
	///     <para>
	///         The timer is initially stopped and needs to be started explicitly using <see cref="Start" />.
	///     </para>
	///     <para>
	///         The associated dispatcher does not need to be started.
	///         However, the timer is stopped on an interval if the dispatcher is not running.
	///     </para>
	///     <note type="important">
	///         If an instance of <see cref="ThreadDispatcherTimer" /> is garbage collected before the delegate is executed, the delegate will not be executed as the timer is stopped and disposed when its finalizer is called.
	///         Therefore, to guarantee the execution of the delegate, you must hold on to the timer instance as long as you want the timer to work.
	///     </note>
	/// </remarks>
	/// <threadsafety static="true" instance="true" />
	public sealed class ThreadDispatcherTimer : IDisposable, ISynchronizable
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="ThreadDispatcherTimer" />.
		/// </summary>
		/// <param name="dispatcher"> The used dispatcher. </param>
		/// <param name="mode"> The timer mode. </param>
		/// <param name="priority"> The priority. </param>
		/// <param name="options"> The used execution options. </param>
		/// <param name="interval"> The interval between executions in milliseconds. </param>
		/// <param name="action"> The delegate. </param>
		/// <param name="parameters"> Optional parameters of the delagate. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="dispatcher" /> or <paramref name="action" /> is null. </exception>
		/// <exception cref="ArgumentOutOfRangeException"> <paramref name="priority" /> is less than zero. </exception>
		public ThreadDispatcherTimer (IThreadDispatcher dispatcher, ThreadDispatcherTimerMode mode, int priority, ThreadDispatcherOptions options, int interval, Delegate action, params object[] parameters)
			: this(dispatcher, mode, priority, options, TimeSpan.FromMilliseconds(interval), action, parameters)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="ThreadDispatcherTimer" />.
		/// </summary>
		/// <param name="dispatcher"> The used dispatcher. </param>
		/// <param name="mode"> The timer mode. </param>
		/// <param name="priority"> The priority. </param>
		/// <param name="options"> The used execution options. </param>
		/// <param name="interval"> The interval between executions. </param>
		/// <param name="action"> The delegate. </param>
		/// <param name="parameters"> Optional parameters of the delagate. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="dispatcher" /> or <paramref name="action" /> is null. </exception>
		/// <exception cref="ArgumentOutOfRangeException"> <paramref name="priority" /> is less than zero. </exception>
		public ThreadDispatcherTimer (IThreadDispatcher dispatcher, ThreadDispatcherTimerMode mode, int priority, ThreadDispatcherOptions options, TimeSpan interval, Delegate action, params object[] parameters)
		{
			if (dispatcher == null)
			{
				throw new ArgumentNullException(nameof(dispatcher));
			}

			if (priority < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(priority));
			}

			if (action == null)
			{
				throw new ArgumentNullException(nameof(action));
			}

			this.SyncRoot = new object();

			this.Dispatcher = dispatcher;
			this.Mode = mode;
			this.Priority = priority;
			this.Options = options;
			this.Interval = interval;
			this.Action = action;
			this.Parameters = parameters;

			this.Timer = null;
			this.PreviousOperation = null;

			//TODO: Use real
			ThreadDispatcherOperation.Capture();
		}

		/// <summary>
		///     Garbage collects this instance of <see cref="ThreadDispatcherTimer" />.
		/// </summary>
		~ThreadDispatcherTimer ()
		{
			this.Dispose(false);
		}

		#endregion




		#region Instance Fields

		private long _executionCount;

		private long _missCount;

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets the used delegate.
		/// </summary>
		/// <value>
		///     The used delegate.
		/// </value>
		public Delegate Action { get; }

		/// <summary>
		///     Gets the used dispatcher.
		/// </summary>
		/// <value>
		///     The used dispatcher.
		/// </value>
		public IThreadDispatcher Dispatcher { get; }

		/// <summary>
		///     Gets the number of times the delegate was executed.
		/// </summary>
		/// <value>
		///     The number of times the delegate was executed.
		/// </value>
		/// <remarks>
		///     <para>
		///         <see cref="ExecutionCount" /> is reset to zero whenever the timer is started after it was stopped.
		///     </para>
		/// </remarks>
		public long ExecutionCount
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this._executionCount;
				}
			}
			private set
			{
				lock (this.SyncRoot)
				{
					this._executionCount = value;
				}
			}
		}

		/// <summary>
		///     Gets the used interval.
		/// </summary>
		/// <value>
		///     The used interval.
		/// </value>
		public TimeSpan Interval { get; }

		/// <summary>
		///     Gets whether the timer is currently running.
		/// </summary>
		/// <value>
		///     true if the timer is running, false otherwise.
		/// </value>
		public bool IsRunning
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this.Timer != null;
				}
			}
		}


		/// <summary>
		///     Gets the number of times the delegate was not executed because the previous execution was not yet finished.
		/// </summary>
		/// <value>
		///     The number of times the delegate was not executed because the previous execution was not yet finished
		/// </value>
		/// <remarks>
		///     <para>
		///         <see cref="MissCount" /> is reset to zero whenever the timer is started after it was stopped.
		///     </para>
		/// </remarks>
		public long MissCount
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this._missCount;
				}
			}
			private set
			{
				lock (this.SyncRoot)
				{
					this._missCount = value;
				}
			}
		}

		/// <summary>
		///     Gets the timer mode.
		/// </summary>
		/// <value>
		///     The timer mode.
		/// </value>
		public ThreadDispatcherTimerMode Mode { get; }

		/// <summary>
		///     Gets the used execution options.
		/// </summary>
		/// <value>
		///     The used execution options.
		/// </value>
		public ThreadDispatcherOptions Options { get; }

		/// <summary>
		///     Gets the optional parameters of the delagate.
		/// </summary>
		/// <value>
		///     The optional parameters of the delagate.
		/// </value>
		public object[] Parameters { get; }

		/// <summary>
		///     Gets the priority.
		/// </summary>
		/// <value>
		///     The priority.
		/// </value>
		public int Priority { get; }

		private ThreadDispatcherOperation PreviousOperation { get; set; }

		private Timer Timer { get; set; }

		#endregion




		#region Instance Methods

		/// <summary>
		///     Starts the timer.
		/// </summary>
		public void Start ()
		{
			lock (this.SyncRoot)
			{
				if (this.Timer != null)
				{
					return;
				}

				GC.ReRegisterForFinalize(this);

				this.ExecutionCount = 0;
				this.MissCount = 0;

				this.Timer = new Timer(x =>
				{
					ThreadDispatcherTimer self = (ThreadDispatcherTimer)x;
					lock (self.SyncRoot)
					{
						if (self.Timer == null)
						{
							return;
						}

						if (self.PreviousOperation != null)
						{
							if (!self.PreviousOperation.IsDone)
							{
								self.MissCount++;
								return;
							}
						}

						bool isRunning;
						lock (self.Dispatcher.SyncRoot)
						{
							isRunning = self.Dispatcher.IsRunning;
							if (isRunning)
							{
								//TODO: Use captured context
								self.PreviousOperation = self.Dispatcher.Post(self.Priority, self.Options, self.Action, self.Parameters);
								self.ExecutionCount++;
							}
						}

						if ((self.Mode == ThreadDispatcherTimerMode.OneShot) || (!isRunning))
						{
							self.Stop();
						}
					}
				}, this, (int)this.Interval.TotalMilliseconds, this.Mode == ThreadDispatcherTimerMode.OneShot ? -1 : (int)this.Interval.TotalMilliseconds);
			}
		}

		/// <summary>
		///     Stops the timer.
		/// </summary>
		public void Stop ()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		[SuppressMessage("ReSharper", "UnusedParameter.Local")]
		[SuppressMessage("ReSharper", "EmptyGeneralCatchClause")]
		private void Dispose (bool disposing)
		{
			lock (this.SyncRoot)
			{
				this.Timer?.Dispose();
				this.Timer = null;
			}
		}

		#endregion




		#region Interface: IDisposable

		/// <inheritdoc />
		void IDisposable.Dispose ()
		{
			this.Stop();
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
