using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;




namespace RI.Framework.Utilities.Threading
{
	/// <summary>
	///     Combines a <see cref="HeavyThread" /> and a <see cref="ThreadDispatcher" /> which is run inside the <see cref="HeavyThread" />.
	/// </summary>
	/// <remarks>
	///     <para>
	///         See <see cref="HeavyThread" /> and <see cref="ThreadDispatcher" /> for more details.
	///     </para>
	/// </remarks>
	/// <threadsafety static="true" instance="true" />
	public sealed class HeavyThreadDispatcher : HeavyThread, IThreadDispatcher
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="HeavyThreadDispatcher" />.
		/// </summary>
		public HeavyThreadDispatcher ()
		{
			this.DispatcherExceptionHandlerDelegate = this.DispatcherExceptionHandler;

			this.DefaultPriority = ThreadDispatcher.DefaultPriorityValue;
			this.CatchExceptions = false;
			this.FinishPendingDelegatesOnShutdown = false;
			this.ThreadName = this.GetType().Name;
			this.IsBackgroundThread = true;

			Thread currentThread = Thread.CurrentThread;
			this.ThreadPriority = currentThread.Priority;
			this.ThreadCulture = currentThread.CurrentCulture;
			this.ThreadUICulture = currentThread.CurrentUICulture;

			this.StartedEvent = null;
			this.DispatcherInternal = null;
		}

		#endregion




		#region Instance Fields

		private bool _catchExceptions;
		private int _defaultPriority;
		private bool _finishPendingDelegatesOnShutdown;
		private bool _isBackgroundThread;
		private CultureInfo _threadCulture;
		private string _threadName;

		private ThreadPriority _threadPriority;
		private CultureInfo _threadUICulture;

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets the used <see cref="ThreadDispatcher" />.
		/// </summary>
		/// <value>
		///     The used <see cref="ThreadDispatcher" /> or null if the thread is not running.
		/// </value>
		public ThreadDispatcher Dispatcher
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this.DispatcherInternal;
				}
			}
		}

		/// <summary>
		///     Gets or sets whether pending delegates should be processed when shutting down (<see cref="HeavyThread.Stop" />).
		/// </summary>
		/// <value>
		///     true if pending delegates should be processed during shutdown, false if pending delegates should be discarded when <see cref="HeavyThread.Stop" /> is called.
		/// </value>
		/// <remarks>
		///     <para>
		///         The default value is false.
		///     </para>
		/// </remarks>
		public bool FinishPendingDelegatesOnShutdown
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this._finishPendingDelegatesOnShutdown;
				}
			}
			set
			{
				lock (this.SyncRoot)
				{
					this._finishPendingDelegatesOnShutdown = value;
				}
			}
		}

		/// <summary>
		///     Gets or sets whether the thread is a background thread.
		/// </summary>
		/// <value>
		///     true if the thread is a background thread, false otherwise.
		/// </value>
		public bool IsBackgroundThread
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this._isBackgroundThread;
				}
			}
			set
			{
				lock (this.SyncRoot)
				{
					this._isBackgroundThread = value;

					if (this.Thread != null)
					{
						this.Thread.IsBackground = value;
					}
				}
			}
		}

		/// <summary>
		///     Gets or sets the formatting culture of the thread.
		/// </summary>
		/// <value>
		///     The formatting culture of the thread.
		/// </value>
		public CultureInfo ThreadCulture
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this._threadCulture;
				}
			}
			set
			{
				lock (this.SyncRoot)
				{
					this._threadCulture = value;

					if (this.Thread != null)
					{
						this.Thread.CurrentCulture = value;
					}
				}
			}
		}

		/// <summary>
		///     Gets or sets the name of the thread.
		/// </summary>
		/// <value>
		///     The name of the thread.
		/// </value>
		public string ThreadName
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this._threadName;
				}
			}
			set
			{
				lock (this.SyncRoot)
				{
					this._threadName = value;

					if (this.Thread != null)
					{
						this.Thread.Name = value;
					}
				}
			}
		}

		/// <summary>
		///     Gets or sets the priority of the thread.
		/// </summary>
		/// <value>
		///     The priority of the thread.
		/// </value>
		public ThreadPriority ThreadPriority
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this._threadPriority;
				}
			}
			set
			{
				lock (this.SyncRoot)
				{
					this._threadPriority = value;

					if (this.Thread != null)
					{
						this.Thread.Priority = value;
					}
				}
			}
		}

		/// <summary>
		///     Gets or sets the UI culture of the thread.
		/// </summary>
		/// <value>
		///     The UI culture of the thread.
		/// </value>
		[SuppressMessage("ReSharper", "InconsistentNaming")]
		public CultureInfo ThreadUICulture
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this._threadUICulture;
				}
			}
			set
			{
				lock (this.SyncRoot)
				{
					this._threadUICulture = value;

					if (this.Thread != null)
					{
						this.Thread.CurrentUICulture = value;
					}
				}
			}
		}

		private EventHandler<ThreadDispatcherExceptionEventArgs> DispatcherExceptionHandlerDelegate { get; set; }

		private ThreadDispatcher DispatcherInternal { get; set; }

		private ManualResetEvent StartedEvent { get; set; }

		#endregion




		#region Instance Methods

		/// <inheritdoc cref="HeavyThread.Stop" />
		/// <param name="finishPendingDelegates"> Specifies whether already pending delegates should be processed before the dispatcher is shut down. </param>
		/// <remarks>
		///     <para>
		///         <see cref="FinishPendingDelegatesOnShutdown" /> is set to the value of <paramref name="finishPendingDelegates" />.
		///     </para>
		/// </remarks>
		public void Stop (bool finishPendingDelegates)
		{
			this.FinishPendingDelegatesOnShutdown = finishPendingDelegates;

			this.Stop();
		}

		private void DispatcherExceptionHandler (object sender, ThreadDispatcherExceptionEventArgs args)
		{
			this.OnException(args.Exception, true, args.CanContinue);
		}

		private void OnException (Exception exception, bool canContinue)
		{
			this.Exception?.Invoke(this, new ThreadDispatcherExceptionEventArgs(exception, canContinue));
		}

		#endregion




		#region Overrides

		/// <inheritdoc />
		protected override void Dispose (bool disposing)
		{
			base.Dispose(disposing);

			if (this.DispatcherInternal != null)
			{
				this.DispatcherInternal.Exception -= this.DispatcherExceptionHandlerDelegate;
				this.DispatcherInternal = null;
			}

			if (this.StartedEvent != null)
			{
				this.StartedEvent.Dispose();
				this.StartedEvent = null;
			}
		}

		/// <inheritdoc />
		protected override void OnBegin ()
		{
			base.OnBegin();

			this.StartedEvent = new ManualResetEvent(false);

			this.DispatcherInternal = new ThreadDispatcher();
			this.DispatcherInternal.Exception += this.DispatcherExceptionHandlerDelegate;
			this.DispatcherInternal.CatchExceptions = this.CatchExceptions;
			this.DispatcherInternal.DefaultPriority = this.DefaultPriority;
		}

		/// <inheritdoc />
		protected override void OnException (Exception exception, bool running, bool canContinue)
		{
			base.OnException(exception, running, canContinue);

			this.OnException(exception, canContinue);
		}

		/// <inheritdoc />
		protected override void OnRun ()
		{
			base.OnRun();

			this.StartedEvent.Set();

			this.DispatcherInternal.Run();
		}

		/// <inheritdoc />
		protected override void OnStarted ()
		{
			base.OnStarted();

			bool started = this.StartedEvent.WaitOne(this.Timeout);

			if (started)
			{
				DateTime start = DateTime.UtcNow;
				while (!this.DispatcherInternal.IsRunning)
				{
					Thread.Sleep(1);
					if (DateTime.UtcNow.Subtract(start).TotalMilliseconds > this.Timeout)
					{
						started = false;
						break;
					}
				}
			}

			if (!started)
			{
				throw new TimeoutException("Timeout while waiting for dispatcher to start running.");
			}
		}

		/// <inheritdoc />
		protected override void OnStarting ()
		{
			base.OnStarting();

			this.Thread.Name = this.ThreadName;
			this.Thread.Priority = this.ThreadPriority;
			this.Thread.IsBackground = this.IsBackgroundThread;
			this.Thread.CurrentCulture = this.ThreadCulture;
			this.Thread.CurrentUICulture = this.ThreadUICulture;
		}

		/// <inheritdoc />
		protected override void OnStop ()
		{
			if (this.DispatcherInternal.IsRunning)
			{
				this.DispatcherInternal.Shutdown(this.FinishPendingDelegatesOnShutdown);
			}

			base.OnStop();
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
					if (this.DispatcherInternal != null)
					{
						this._catchExceptions = this.DispatcherInternal.CatchExceptions;
					}

					return this._catchExceptions;
				}
			}
			set
			{
				lock (this.SyncRoot)
				{
					this._catchExceptions = value;

					if (this.DispatcherInternal != null)
					{
						this.DispatcherInternal.CatchExceptions = value;
					}
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
					if (this.DispatcherInternal != null)
					{
						this._defaultPriority = this.DispatcherInternal.DefaultPriority;
					}

					return this._defaultPriority;
				}
			}
			set
			{
				lock (this.SyncRoot)
				{
					this._defaultPriority = value;

					if (this.DispatcherInternal != null)
					{
						this.DispatcherInternal.DefaultPriority = value;
					}
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
					return this.DispatcherInternal?.IsShuttingDown ?? false;
				}
			}
		}

		/// <inheritdoc />
		public ThreadDispatcherShutdownMode ShutdownMode
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this.DispatcherInternal?.ShutdownMode ?? ThreadDispatcherShutdownMode.None;
				}
			}
		}

		/// <inheritdoc />
		public event EventHandler<ThreadDispatcherExceptionEventArgs> Exception;

		/// <inheritdoc />
		public void DoProcessing ()
		{
			ThreadDispatcher dispatcher;
			lock (this.SyncRoot)
			{
				this.VerifyRunning();
				dispatcher = this.DispatcherInternal;
			}

			dispatcher.DoProcessing();
		}

		/// <inheritdoc />
		public Task DoProcessingAsync ()
		{
			ThreadDispatcher dispatcher;
			lock (this.SyncRoot)
			{
				this.VerifyRunning();
				dispatcher = this.DispatcherInternal;
			}

			return dispatcher.DoProcessingAsync();
		}

		/// <inheritdoc />
		public int? GetCurrentPriority ()
		{
			lock (this.SyncRoot)
			{
				return this.DispatcherInternal?.GetCurrentPriority();
			}
		}

		/// <inheritdoc />
		public ThreadDispatcherOperation Post (Delegate action, params object[] parameters)
		{
			ThreadDispatcher dispatcher;
			lock (this.SyncRoot)
			{
				this.VerifyRunning();
				dispatcher = this.DispatcherInternal;
			}

			return dispatcher.Post(action, parameters);
		}

		/// <inheritdoc />
		public ThreadDispatcherOperation Post (int priority, Delegate action, params object[] parameters)
		{
			ThreadDispatcher dispatcher;
			lock (this.SyncRoot)
			{
				this.VerifyRunning();
				dispatcher = this.DispatcherInternal;
			}

			return dispatcher.Post(priority, action, parameters);
		}

		/// <inheritdoc />
		public object Send (Delegate action, params object[] parameters)
		{
			ThreadDispatcher dispatcher;
			lock (this.SyncRoot)
			{
				this.VerifyRunning();
				dispatcher = this.DispatcherInternal;
			}

			return dispatcher.Send(action, parameters);
		}

		/// <inheritdoc />
		public object Send (int priority, Delegate action, params object[] parameters)
		{
			ThreadDispatcher dispatcher;
			lock (this.SyncRoot)
			{
				this.VerifyRunning();
				dispatcher = this.DispatcherInternal;
			}

			return dispatcher.Send(priority, action, parameters);
		}

		/// <inheritdoc />
		public Task<object> SendAsync (Delegate action, params object[] parameters)
		{
			ThreadDispatcher dispatcher;
			lock (this.SyncRoot)
			{
				this.VerifyRunning();
				dispatcher = this.DispatcherInternal;
			}

			return dispatcher.SendAsync(action, parameters);
		}

		/// <inheritdoc />
		public Task<object> SendAsync (int priority, Delegate action, params object[] parameters)
		{
			ThreadDispatcher dispatcher;
			lock (this.SyncRoot)
			{
				this.VerifyRunning();
				dispatcher = this.DispatcherInternal;
			}

			return dispatcher.SendAsync(priority, action, parameters);
		}

		/// <inheritdoc />
		public void Shutdown (bool finishPendingDelegates)
		{
			this.Stop(finishPendingDelegates);
		}

		/// <inheritdoc />
		public Task ShutdownAsync (bool finishPendingDelegates)
		{
			lock (this.SyncRoot)
			{
				return Task.Factory.StartNew(() => this.Shutdown(finishPendingDelegates));
			}
		}

		#endregion
	}
}
