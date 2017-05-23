using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;




namespace RI.Framework.Utilities.Threading
{
	/// <summary>
	///     Implements a timer which can be used with <see cref="IThreadDispatcher" />.
	/// </summary>
	/// <remarks>
	///     <para>
	///         <see cref="ThreadDispatcherTimer" /> enqueues a delegate to the specified dispatchers queue (using <see cref="IThreadDispatcher.Post(int,Delegate,object[])" />) in a specified interval.
	///     </para>
	///     <para>
	///         The interval is awaited before the timer is executed for the first time. Afterwards, the delegate is posted to the dispatcher in the specified interval.
	///     </para>
	///     <para>
	///         The timer is initially stopped and needs to be started explicitly using <see cref="Start" />.
	///     </para>
	///     <note type="note">
	///         <see cref="ThreadDispatcherTimer" /> is relatively heavy-weighted as it uses its own thread (one per instance) to deliver the delegate to the dispatcher.
	///     </note>
	/// </remarks>
	public sealed class ThreadDispatcherTimer : IDisposable
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="ThreadDispatcherTimer" />.
		/// </summary>
		/// <param name="dispatcher"> The used dispatcher. </param>
		/// <param name="mode"> The timer mode. </param>
		/// <param name="priority">The priority.</param>
		/// <param name="interval"> The interval between executions in milliseconds. </param>
		/// <param name="action"> The delegate. </param>
		/// <param name="parameters"> Optional parameters of the delagate. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="dispatcher" /> or <paramref name="action" /> is null. </exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="priority"/> is less than zero.</exception>
		public ThreadDispatcherTimer (IThreadDispatcher dispatcher, ThreadDispatcherTimerMode mode, int priority, int interval, Delegate action, params object[] parameters)
			: this(dispatcher, mode, priority, TimeSpan.FromMilliseconds(interval), action, parameters)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="ThreadDispatcherTimer" />.
		/// </summary>
		/// <param name="dispatcher"> The used dispatcher. </param>
		/// <param name="mode"> The timer mode. </param>
		/// <param name="priority">The priority.</param>
		/// <param name="interval"> The interval between executions. </param>
		/// <param name="action"> The delegate. </param>
		/// <param name="parameters"> Optional parameters of the delagate. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="dispatcher" /> or <paramref name="action" /> is null. </exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="priority"/> is less than zero.</exception>
		public ThreadDispatcherTimer (IThreadDispatcher dispatcher, ThreadDispatcherTimerMode mode, int priority, TimeSpan interval, Delegate action, params object[] parameters)
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
			this.Interval = interval;
			this.Action = action;
			this.Parameters = parameters;
		}

		/// <summary>
		///     Garbage collects this instance of <see cref="ThreadDispatcherTimer" />.
		/// </summary>
		~ThreadDispatcherTimer ()
		{
			this.Dispose(false);
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets the used delegate.
		/// </summary>
		/// <value>
		///     The used delegate.
		/// </value>
		public Delegate Action { get; private set; }

		/// <summary>
		///     Gets the used dispatcher.
		/// </summary>
		/// <value>
		///     The used dispatcher.
		/// </value>
		public IThreadDispatcher Dispatcher { get; private set; }

		/// <summary>
		///     Gets the used interval.
		/// </summary>
		/// <value>
		///     The used interval.
		/// </value>
		public TimeSpan Interval { get; private set; }

		/// <summary>
		///     Gets the timer mode.
		/// </summary>
		/// <value>
		///     The timer mode.
		/// </value>
		public ThreadDispatcherTimerMode Mode { get; private set; }

		/// <summary>
		///     Gets the optional parameters of the delagate.
		/// </summary>
		/// <value>
		///     The optional parameters of the delagate.
		/// </value>
		public object[] Parameters { get; private set; }

		/// <summary>
		/// Gets the priority.
		/// </summary>
		/// <value>
		/// The priority.
		/// </value>
		public int Priority { get; private set; }

		private object SyncRoot { get; set; }

		private Thread TimerThread { get; set; }

		#endregion




		#region Instance Methods

		/// <summary>
		///     Starts the timer.
		/// </summary>
		public void Start ()
		{
			lock (this.SyncRoot)
			{
				if (this.TimerThread != null)
				{
					return;
				}

				this.TimerThread = new Thread(() =>
				{
					bool cont;
					do
					{
						Thread.Sleep(this.Interval);
						lock (this.SyncRoot)
						{
							lock (this.Dispatcher.SyncRoot)
							{
								if (this.Dispatcher.IsRunning)
								{
									this.Dispatcher.Post(this.Priority, this.Action, this.Parameters);
								}
							}
							cont = this.Mode == ThreadDispatcherTimerMode.Continuous;
						}
					}
					while (cont);

					lock (this.SyncRoot)
					{
						this.TimerThread = null;
					}
				});
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
				if (this.TimerThread == null)
				{
					return;
				}

				try
				{
					this.TimerThread.Abort();
				}
				catch
				{
				}
				this.TimerThread = null;
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
	}
}
