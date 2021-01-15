using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

using RI.Framework.Utilities;




namespace RI.Framework.Threading.Dispatcher
{
    /// <summary>
    ///     Combines a <see cref="HeavyThread" /> and a <see cref="ThreadDispatcher" /> which is run inside a dedicated thread provided by <see cref="HeavyThread" />.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         See <see cref="HeavyThread" /> and <see cref="ThreadDispatcher" /> for more details.
    ///     </para>
    ///     <note type="important">
    ///         Some virtual methods are called from within locks to <see cref="HeavyThread.SyncRoot" />.
    ///         Be careful in inheriting classes when calling outside code from those methods (e.g. through events, callbacks, or other virtual methods) to not produce deadlocks!
    ///     </note>
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
            this.DispatcherWatchdogHandlerDelegate = this.DispatcherWatchdogHandler;

            this.CatchExceptions = false;
            this.DefaultPriority = ThreadDispatcher.DefaultPriorityValue;
            this.DefaultOptions = ThreadDispatcher.DefaultOptionsValue;
            this.WatchdogTimeout = null;

            this.FinishPendingDelegatesOnShutdown = false;
            this.IsBackgroundThread = true;
            this.ThreadName = this.GetType().Name;

            Thread currentThread = Thread.CurrentThread;
            this.ThreadPriority = currentThread.Priority;
            this.ThreadCulture = currentThread.CurrentCulture;
            this.ThreadUICulture = currentThread.CurrentUICulture;

            this.Dispatcher = null;
            this.DispatcherStartOperation = null;
        }

        #endregion




        #region Instance Fields

        private bool _catchExceptions;
        private ThreadDispatcherOptions _defaultOptions;
        private int _defaultPriority;
        private TimeSpan? _watchdogTimeout;

        private bool _finishPendingDelegatesOnShutdown;
        private bool _isBackgroundThread;
        private string _threadName;

        private ThreadPriority _threadPriority;
        private CultureInfo _threadCulture;
        private CultureInfo _threadUICulture;

        #endregion




        #region Instance Properties/Indexer

        /// <summary>
        ///     Gets or sets whether pending delegates should be processed when shutting down.
        /// </summary>
        /// <value>
        ///     true if pending delegates should be processed during shutdown, false if pending delegates should be discarded during shutdown.
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
        /// <remarks>
        ///     <para>
        ///         The default value is true.
        ///     </para>
        /// </remarks>
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
        /// <remarks>
        ///     <para>
        ///         The default value is the formatting culture of the thread this instance was created on.
        ///     </para>
        /// </remarks>
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
        /// <remarks>
        ///     <para>
        ///         The default value is the name of this type.
        ///     </para>
        /// </remarks>
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
        /// <remarks>
        ///     <para>
        ///         The default value is the priority of the thread this instance was created on.
        ///     </para>
        /// </remarks>
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
        /// <remarks>
        ///     <para>
        ///         The default value is the UI culture of the thread this instance was created on.
        ///     </para>
        /// </remarks>
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

        internal ThreadDispatcher Dispatcher { get; private set; }

        private EventHandler<ThreadDispatcherExceptionEventArgs> DispatcherExceptionHandlerDelegate { get; }

        private ThreadDispatcherOperation DispatcherStartOperation { get; set; }

        private EventHandler<ThreadDispatcherWatchdogEventArgs> DispatcherWatchdogHandlerDelegate { get; }

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
            this.VerifyNotFromThread(nameof(this.Stop));
            this.FinishPendingDelegatesOnShutdown = finishPendingDelegates;
            this.Stop();
        }

        private Task BeginShutdownInternal (bool finishPendingDelegates)
        {
            StopThread stopThread = new StopThread(this, finishPendingDelegates);
            stopThread.Start();
            return stopThread.CompletionTask;
        }

        private void DispatcherExceptionHandler (object sender, ThreadDispatcherExceptionEventArgs args)
        {
            this.OnException(args.Exception, args.CanContinue);
        }

        private void DispatcherWatchdogHandler (object sender, ThreadDispatcherWatchdogEventArgs args)
        {
            this.Watchdog?.Invoke(this, args);
        }

        private void GetRidOfDispatcher ()
        {
            lock (this.SyncRoot)
            {
                if (this.DispatcherStartOperation != null)
                {
                    lock (this.DispatcherStartOperation.SyncRoot)
                    {
                        this.DispatcherStartOperation.CancelHard();
                        this.DispatcherStartOperation = null;
                    }
                }

                if (this.Dispatcher != null)
                {
                    lock (this.Dispatcher.SyncRoot)
                    {
                        this.Dispatcher.Exception -= this.DispatcherExceptionHandlerDelegate;
                        this.Dispatcher.Watchdog -= this.DispatcherWatchdogHandlerDelegate;

                        ((IDisposable)this.Dispatcher).Dispose();
                        this.Dispatcher = null;
                    }
                }
            }
        }

        private void VerifyRunningDispatcher ()
        {
            this.VerifyRunning();

            if (!(this.Dispatcher?.IsRunning ?? false))
            {
                throw new InvalidOperationException(this.GetType().Name + " is not running.");
            }
        }

        #endregion




        #region Overrides

        /// <inheritdoc />
        protected override void OnBegin ()
        {
            base.OnBegin();

            lock (this.SyncRoot)
            {
                this.GetRidOfDispatcher();

                bool catchExceptions = this.CatchExceptions;
                int defaultPriority = this.DefaultPriority;
                ThreadDispatcherOptions defaultOptions = this.DefaultOptions;
                TimeSpan? watchdogTimeout = this.WatchdogTimeout;

                this.Dispatcher = new ThreadDispatcher();
                this.Dispatcher.Exception += this.DispatcherExceptionHandlerDelegate;
                this.Dispatcher.Watchdog += this.DispatcherWatchdogHandlerDelegate;
                this.Dispatcher.CatchExceptions = catchExceptions;
                this.Dispatcher.DefaultPriority = defaultPriority;
                this.Dispatcher.DefaultOptions = defaultOptions;
                this.Dispatcher.WatchdogTimeout = watchdogTimeout;

                this.DispatcherStartOperation = this.Dispatcher.Post(0, ThreadDispatcherOptions.None, new Action(() => { }));
            }
        }

        /// <inheritdoc />
        protected override void OnStopped ()
        {
            base.OnStopped();

            this.GetRidOfDispatcher();
        }

        /// <inheritdoc />
        protected override void OnEnd ()
        {
            base.OnEnd();

            this.GetRidOfDispatcher();
        }

        /// <inheritdoc />
        protected override void OnException (Exception exception, bool canContinue)
        {
            base.OnException(exception, canContinue);

            this.Exception?.Invoke(this, new ThreadDispatcherExceptionEventArgs(exception, canContinue));
        }

        /// <inheritdoc />
        protected override void OnRun ()
        {
            base.OnRun();

            this.Dispatcher.Run();
        }

        /// <inheritdoc />
        protected override void OnStarted (bool withLock)
        {
            base.OnStarted(withLock);

            if (withLock)
            {
                return;
            }

            if (!this.DispatcherStartOperation.Wait(this.Timeout))
            {
                throw new TimeoutException(this.GetType().Name + " failed to start (timeout while waiting for dispatcher processing).");
            }
        }

        /// <inheritdoc />
        protected override void OnStarting()
        {
            base.OnStarting();

            this.Thread.Name = this.ThreadName;
            this.Thread.Priority = this.ThreadPriority;
            this.Thread.IsBackground = this.IsBackgroundThread;
            this.Thread.CurrentCulture = this.ThreadCulture;
            this.Thread.CurrentUICulture = this.ThreadUICulture;
        }

        /// <inheritdoc />
        protected override void OnStopping ()
        {
            if (this.Dispatcher?.IsRunning ?? false)
            {
                this.Dispatcher?.BeginShutdown(this.FinishPendingDelegatesOnShutdown);
            }

            base.OnStopping();
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
                    //TODO: Should this not affect the HeavyThread too?!?!
                    this._catchExceptions = value;

                    if (this.Dispatcher != null)
                    {
                        this.Dispatcher.CatchExceptions = value;
                    }
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
                if (value == ThreadDispatcherOptions.Default)
                {
                    throw new ArgumentException("Invalid default thread dispatcher option", nameof(value));
                }

                lock (this.SyncRoot)
                {
                    this._defaultOptions = value;

                    if (this.Dispatcher != null)
                    {
                        this.Dispatcher.DefaultOptions = value;
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

                    if (this.Dispatcher != null)
                    {
                        this.Dispatcher.DefaultPriority = value;
                    }
                }
            }
        }

        bool ISynchronizeInvoke.InvokeRequired => ((ISynchronizeInvoke)this.Dispatcher)?.InvokeRequired ?? false;

        /// <inheritdoc />
        public bool IsShuttingDown
        {
            get
            {
                lock (this.SyncRoot)
                {
                    return this.Dispatcher?.IsShuttingDown ?? false;
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
                    return this.Dispatcher?.ShutdownMode ?? ThreadDispatcherShutdownMode.None;
                }
            }
        }

        /// <inheritdoc />
        public TimeSpan? WatchdogTimeout
        {
            get
            {
                lock (this.SyncRoot)
                {
                    return this._watchdogTimeout;
                }
            }
            set
            {
                if (value.GetValueOrDefault(TimeSpan.Zero).IsNegative())
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                lock (this.SyncRoot)
                {
                    this._watchdogTimeout = value;

                    if (this.Dispatcher != null)
                    {
                        this.Dispatcher.WatchdogTimeout = value;
                    }
                }
            }
        }

        /// <inheritdoc />
        public event EventHandler<ThreadDispatcherExceptionEventArgs> Exception;

        /// <inheritdoc />
        public event EventHandler<ThreadDispatcherWatchdogEventArgs> Watchdog;

        /// <inheritdoc />
        public bool AddKeepAlive (object obj)
        {
            lock (this.SyncRoot)
            {
                return this.Dispatcher?.AddKeepAlive(obj) ?? false;
            }
        }

        /// <inheritdoc />
        IAsyncResult ISynchronizeInvoke.BeginInvoke (Delegate method, object[] args)
        {
            ThreadDispatcher dispatcher;
            lock (this.SyncRoot)
            {
                this.VerifyRunningDispatcher();
                dispatcher = this.Dispatcher;
            }

            return ((ISynchronizeInvoke)dispatcher).BeginInvoke(method, args);
        }

        /// <inheritdoc />
        [SuppressMessage("ReSharper", "UnusedVariable")]
        public void BeginShutdown (bool finishPendingDelegates)
        {
            lock (this.SyncRoot)
            {
                this.VerifyRunningDispatcher();

                this.BeginShutdownInternal(finishPendingDelegates);
            }
        }

        /// <inheritdoc />
        public void DoProcessing ()
        {
            ThreadDispatcher dispatcher;
            lock (this.SyncRoot)
            {
                this.VerifyRunningDispatcher();
                dispatcher = this.Dispatcher;
            }

            dispatcher.DoProcessing();
        }

        /// <inheritdoc />
        public void DoProcessing (int priority)
        {
            ThreadDispatcher dispatcher;
            lock (this.SyncRoot)
            {
                this.VerifyRunningDispatcher();
                dispatcher = this.Dispatcher;
            }

            dispatcher.DoProcessing(priority);
        }

        /// <inheritdoc />
        public Task DoProcessingAsync ()
        {
            ThreadDispatcher dispatcher;
            lock (this.SyncRoot)
            {
                this.VerifyRunningDispatcher();
                dispatcher = this.Dispatcher;
            }

            return dispatcher.DoProcessingAsync();
        }

        /// <inheritdoc />
        public Task DoProcessingAsync (int priority)
        {
            ThreadDispatcher dispatcher;
            lock (this.SyncRoot)
            {
                this.VerifyRunningDispatcher();
                dispatcher = this.Dispatcher;
            }

            return dispatcher.DoProcessingAsync(priority);
        }

        /// <inheritdoc />
        object ISynchronizeInvoke.EndInvoke (IAsyncResult result)
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            ThreadDispatcher dispatcher;
            lock (this.SyncRoot)
            {
                this.VerifyRunningDispatcher();
                dispatcher = this.Dispatcher;
            }

            return ((ISynchronizeInvoke)dispatcher).EndInvoke(result);
        }

        /// <inheritdoc />
        public ThreadDispatcherOptions? GetCurrentOptions ()
        {
            lock (this.SyncRoot)
            {
                return this.Dispatcher?.GetCurrentOptions();
            }
        }

        /// <inheritdoc />
        public int? GetCurrentPriority ()
        {
            lock (this.SyncRoot)
            {
                return this.Dispatcher?.GetCurrentPriority();
            }
        }

        /// <inheritdoc />
        object ISynchronizeInvoke.Invoke (Delegate method, object[] args)
        {
            ThreadDispatcher dispatcher;
            lock (this.SyncRoot)
            {
                this.VerifyRunningDispatcher();
                dispatcher = this.Dispatcher;
            }

            return ((ISynchronizeInvoke)dispatcher).Invoke(method, args);
        }

        /// <inheritdoc />
        public ThreadDispatcherOperation Post (Delegate action, params object[] parameters)
        {
            ThreadDispatcher dispatcher;
            lock (this.SyncRoot)
            {
                this.VerifyRunningDispatcher();
                dispatcher = this.Dispatcher;
            }

            return dispatcher.Post(action, parameters);
        }

        /// <inheritdoc />
        public ThreadDispatcherOperation Post (int priority, Delegate action, params object[] parameters)
        {
            ThreadDispatcher dispatcher;
            lock (this.SyncRoot)
            {
                this.VerifyRunningDispatcher();
                dispatcher = this.Dispatcher;
            }

            return dispatcher.Post(priority, action, parameters);
        }

        /// <inheritdoc />
        public ThreadDispatcherOperation Post (int priority, ThreadDispatcherOptions options, Delegate action, params object[] parameters)
        {
            ThreadDispatcher dispatcher;
            lock (this.SyncRoot)
            {
                this.VerifyRunningDispatcher();
                dispatcher = this.Dispatcher;
            }

            return dispatcher.Post(priority, options, action, parameters);
        }

        /// <inheritdoc />
        public ThreadDispatcherOperation Post (ThreadDispatcherExecutionContext context, int priority, ThreadDispatcherOptions options, Delegate action, params object[] parameters)
        {
            ThreadDispatcher dispatcher;
            lock (this.SyncRoot)
            {
                this.VerifyRunningDispatcher();
                dispatcher = this.Dispatcher;
            }

            return dispatcher.Post(context, priority, options, action, parameters);
        }

        /// <inheritdoc />
        public bool RemoveKeepAlive (object obj)
        {
            lock (this.SyncRoot)
            {
                return this.Dispatcher?.RemoveKeepAlive(obj) ?? false;
            }
        }

        /// <inheritdoc />
        public object Send (Delegate action, params object[] parameters)
        {
            ThreadDispatcher dispatcher;
            lock (this.SyncRoot)
            {
                this.VerifyRunningDispatcher();
                dispatcher = this.Dispatcher;
            }

            return dispatcher.Send(action, parameters);
        }

        /// <inheritdoc />
        public object Send (int priority, Delegate action, params object[] parameters)
        {
            ThreadDispatcher dispatcher;
            lock (this.SyncRoot)
            {
                this.VerifyRunningDispatcher();
                dispatcher = this.Dispatcher;
            }

            return dispatcher.Send(priority, action, parameters);
        }

        /// <inheritdoc />
        public object Send (int priority, ThreadDispatcherOptions options, Delegate action, params object[] parameters)
        {
            ThreadDispatcher dispatcher;
            lock (this.SyncRoot)
            {
                this.VerifyRunningDispatcher();
                dispatcher = this.Dispatcher;
            }

            return dispatcher.Send(priority, options, action, parameters);
        }

        /// <inheritdoc />
        public Task<object> SendAsync (Delegate action, params object[] parameters)
        {
            ThreadDispatcher dispatcher;
            lock (this.SyncRoot)
            {
                this.VerifyRunningDispatcher();
                dispatcher = this.Dispatcher;
            }

            return dispatcher.SendAsync(action, parameters);
        }

        /// <inheritdoc />
        public Task<object> SendAsync (int priority, Delegate action, params object[] parameters)
        {
            ThreadDispatcher dispatcher;
            lock (this.SyncRoot)
            {
                this.VerifyRunningDispatcher();
                dispatcher = this.Dispatcher;
            }

            return dispatcher.SendAsync(priority, action, parameters);
        }

        /// <inheritdoc />
        public Task<object> SendAsync (int priority, ThreadDispatcherOptions options, Delegate action, params object[] parameters)
        {
            ThreadDispatcher dispatcher;
            lock (this.SyncRoot)
            {
                this.VerifyRunningDispatcher();
                dispatcher = this.Dispatcher;
            }

            return dispatcher.SendAsync(priority, options, action, parameters);
        }

        /// <inheritdoc />
        public void Shutdown (bool finishPendingDelegates)
        {
            Task shutdownTask;

            lock (this.SyncRoot)
            {
                this.VerifyRunningDispatcher();
                this.VerifyNotFromThread(nameof(this.Shutdown));

                shutdownTask = this.BeginShutdownInternal(finishPendingDelegates);
            }

            shutdownTask.Wait();
        }

        /// <inheritdoc />
        public Task ShutdownAsync (bool finishPendingDelegates)
        {
            Task shutdownTask;

            lock (this.SyncRoot)
            {
                this.VerifyRunningDispatcher();
                this.VerifyNotFromThread(nameof(this.ShutdownAsync));

                shutdownTask = this.BeginShutdownInternal(finishPendingDelegates);
            }

            return shutdownTask;
        }

        #endregion




        private sealed class StopThread : HeavyThread
        {
            public HeavyThreadDispatcher HeavyThreadDispatcher { get; }

            public bool FinishPendingDelegates { get; }

            private TaskCompletionSource<object> CompletionSource { get; }

            public Task CompletionTask { get; }

            public StopThread (HeavyThreadDispatcher heavyThreadDispatcher, bool finishPendingDelegates)
            {
                if (heavyThreadDispatcher == null)
                {
                    throw new ArgumentNullException(nameof(heavyThreadDispatcher));
                }

                this.HeavyThreadDispatcher = heavyThreadDispatcher;
                this.FinishPendingDelegates = finishPendingDelegates;

                this.CompletionSource = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
                this.CompletionTask = this.CompletionSource.Task;
            }

            protected override void OnStarting ()
            {
                base.OnStarting();

                this.Thread.Name = "STOP_" + this.HeavyThreadDispatcher.ThreadName;
                this.Thread.Priority = this.HeavyThreadDispatcher.ThreadPriority;
                this.Thread.IsBackground = this.HeavyThreadDispatcher.IsBackgroundThread;
                this.Thread.CurrentCulture = this.HeavyThreadDispatcher.ThreadCulture;
                this.Thread.CurrentUICulture = this.HeavyThreadDispatcher.ThreadUICulture;
            }

            protected override void OnRun ()
            {
                base.OnRun();

                this.HeavyThreadDispatcher.Stop(this.FinishPendingDelegates);

                this.CompletionSource.SetResult(null);
            }
        }
    }
}
