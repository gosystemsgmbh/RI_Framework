using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Threading;

using RI.Framework.Composition.Model;
using RI.Framework.Services.Logging.Filters;
using RI.Framework.Threading.Dispatcher;
using RI.Framework.Utilities.Logging;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Services.Logging.Writers
{
    /// <summary>
    ///     Implements a log writer which forwards log messages to another <see cref="ILogWriter"/> in a background thread.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         <see cref="BackgroundLogWriter" /> can be used to improve performance when creating a lot of log messages by offloading the actual log message writing into a background thread.
    ///     </para>
    ///     <note type="important">
    ///         Because persistence of the log messages is done in a background thread, it is possible that log messages are lost in cases of crashes or unhandled exceptions.
    ///     </note>
    ///     <para>
    ///         See <see cref="ILogWriter" /> for more details.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="true" instance="true" />
    [Export]
    public sealed class BackgroundLogWriter : LogSource, ILogWriter, IDisposable
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="BackgroundLogWriter" />.
        /// </summary>
        /// <param name="writer"> The log writer which is used in the background thread. </param>
        /// <remarks>
        ///     <para>
        ///         <paramref name="writer"/> is owned and disposed by this instance.
        ///     </para>
        ///     <para>
        ///         <see cref="ThreadPriority.Normal" /> is used as the background thread priority.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="writer" /> is null. </exception>
        public BackgroundLogWriter(ILogWriter writer)
            : this(writer, true, ThreadPriority.Normal)
        {
        }

        /// <summary>
        ///     Creates a new instance of <see cref="BackgroundLogWriter" />.
        /// </summary>
        /// <param name="writer"> The log writer which is used in the background thread. </param>
        /// <param name="ownWriter"> Specifies whether the used log writer is owned and disposed by this instance. </param>
        /// <param name="priority"> The used priority of the background thread. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="writer" /> is null. </exception>
        public BackgroundLogWriter(ILogWriter writer, bool ownWriter, ThreadPriority priority)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            this.SyncRoot = new object();

            this.Writer = writer;
            this.OwnWriter = ownWriter;
            this.Priority = priority;

            this.Dispatcher = new HeavyThreadDispatcher();
            this.Dispatcher.CatchExceptions = true;
            this.Dispatcher.DefaultOptions = ThreadDispatcherOptions.None;
            this.Dispatcher.DefaultPriority = 0;
            this.Dispatcher.FinishPendingDelegatesOnShutdown = true;
            this.Dispatcher.IsBackgroundThread = true;
            this.Dispatcher.ThreadCulture = CultureInfo.InvariantCulture;
            this.Dispatcher.ThreadUICulture = CultureInfo.InvariantCulture;
            this.Dispatcher.ThreadName = this.GetType().Name;
            this.Dispatcher.ThreadPriority = this.Priority;
            this.Dispatcher.WatchdogTimeout = null;

            this.Dispatcher.Start();
        }

        /// <summary>
        ///     Garbage collects this instance of <see cref="BackgroundLogWriter" />.
        /// </summary>
        ~BackgroundLogWriter()
        {
            this.Dispose(false);
        }

        #endregion




        #region Instance Fields

        private ILogFilter _filter;

        #endregion




        #region Instance Properties/Indexer

        /// <summary>
        ///     Gets the log writer which is used in the background thread.
        /// </summary>
        /// <value>
        ///     The log writer which is used in the background thread.
        /// </value>
        public ILogWriter Writer { get; }

        /// <summary>
        ///     Gets the used priority of the background thread.
        /// </summary>
        /// <value>
        ///     The used priority of the background thread.
        /// </value>
        public ThreadPriority Priority { get; }

        /// <summary>
        ///     Gets whether the used log writer is owned and disposed by this instance.
        /// </summary>
        /// <value>
        ///     true if the used log writer is owned and disposed by this instance, false otherwise.
        /// </value>
        public bool OwnWriter { get; }

        private HeavyThreadDispatcher Dispatcher { get; set; }

        private object SyncRoot { get; }

        #endregion




        #region Instance Methods

        /// <summary>
        ///     Stops the background thread and forwarding log messages.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         After the log writer is stopped, all calls to <see cref="Log" /> do not have any effect but do not fail.
        ///     </para>
        /// </remarks>
        public void Stop ()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        [SuppressMessage("ReSharper", "UnusedParameter.Local")]
        [SuppressMessage("ReSharper", "EmptyGeneralCatchClause")]
        private void Dispose(bool disposing)
        {
            lock (this.SyncRoot)
            {
                if (this.Dispatcher != null)
                {
                    try
                    {
                        this.Dispatcher.Stop();
                    }
                    catch
                    {
                    }

                    this.Dispatcher = null;
                }

                if (this.OwnWriter)
                {
                    try
                    {
                        (this.Writer as IDisposable)?.Dispose();
                    }
                    catch
                    {
                    }
                }
            }
        }

        #endregion




        #region Interface: IDisposable

        /// <inheritdoc />
        void IDisposable.Dispose()
        {
            this.Stop();
        }

        #endregion




        #region Interface: ILogWriter

        /// <inheritdoc />
        public ILogFilter Filter
        {
            get
            {
                lock (this.SyncRoot)
                {
                    return this._filter;
                }
            }
            set
            {
                lock (this.SyncRoot)
                {
                    this._filter = value;
                }
            }
        }

        /// <inheritdoc />
        bool ISynchronizable.IsSynchronized => true;

        /// <inheritdoc />
        object ISynchronizable.SyncRoot => this.SyncRoot;

        /// <inheritdoc />
        public void Cleanup(DateTime retentionDate)
        {
            lock (this.SyncRoot)
            {
                if (this.Dispatcher == null)
                {
                    return;
                }

                this.Writer.Cleanup(retentionDate);
            }
        }

        /// <inheritdoc />
        [SuppressMessage("ReSharper", "EmptyGeneralCatchClause")]
        public void Log(DateTime timestamp, int threadId, LogLevel severity, string source, string message)
        {
            ILogFilter filter = this.Filter;
            if (filter != null)
            {
                if (!filter.Filter(timestamp, threadId, severity, source))
                {
                    return;
                }
            }

            lock (this.SyncRoot)
            {
                if (this.Dispatcher == null)
                {
                    return;
                }

                try
                {
                    this.Writer.Log(timestamp, threadId, severity, source, message);
                }
                catch
                {
                }
            }
        }

        #endregion
    }
}
