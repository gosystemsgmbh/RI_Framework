using System;
using System.Diagnostics;
using System.Text;

using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;
using RI.Framework.Utilities.Logging;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Services.Logging.Adapters
{
    /// <summary>
    ///     Implements a trace listener which forwards trace messages to a <see cref="ILogService" />.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         <see cref="LogServiceTraceListener" /> is a trace listener which can be used with <see cref="Trace" /> to forward trace messages written to <see cref="Trace" /> to a <see cref="ILogService" />.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="true" instance="true" />
    public sealed class LogServiceTraceListener : TraceListener, ISynchronizable
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="LogServiceTraceListener" />.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         <see cref="LogLocator" /> is used to forward the trace messages.
        ///     </para>
        /// </remarks>
        public LogServiceTraceListener ()
            : this(null)
        {
        }

        /// <summary>
        ///     Creates a new instance of <see cref="LogServiceTraceListener" />.
        /// </summary>
        /// <param name="logService"> The log service to which the trace messages are forwarded or null if <see cref="LogLocator" /> should be used. </param>
        public LogServiceTraceListener (ILogService logService)
        {
            this.SyncRoot = new object();

            this.LogService = logService;

            //TODO: Move to constant & allow null for Source property to use default from constant
            this.Source = "TRACE";
            this.Buffer = null;

            this.TraceOutputOptions = TraceOptions.None;
            this.Filter = null;
            this.NeedIndent = false;
            this.Name = this.GetType().Name;
        }

        #endregion




        #region Instance Fields

        private string _source;

        #endregion




        #region Instance Properties/Indexer

        /// <summary>
        ///     Gets the log service to which the trace messages are forwarded or null if <see cref="LogLocator" /> should be used.
        /// </summary>
        /// <value>
        ///     The log service to which the trace messages are forwarded or null if <see cref="LogLocator" /> should be used.
        /// </value>
        public ILogService LogService { get; }

        /// <summary>
        ///     Gets or sets the source which is used with the logging service.
        /// </summary>
        /// <value>
        ///     The source which is used with the logging service.
        /// </value>
        /// <remarks>
        ///     <para>
        ///         The default value is <c> TRACE </c>.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="value" /> is null. </exception>
        /// <exception cref="EmptyStringArgumentException"> <paramref name="value" /> is an empty string. </exception>
        public string Source
        {
            get
            {
                lock (this.SyncRoot)
                {
                    return this._source;
                }
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                if (value.IsNullOrEmptyOrWhitespace())
                {
                    throw new EmptyStringArgumentException(nameof(value));
                }

                lock (this.SyncRoot)
                {
                    this._source = value;
                }
            }
        }

        private string Buffer { get; set; }

        #endregion




        #region Instance Methods

        private void WriteTrace (string message)
        {
            lock (this.SyncRoot)
            {
                StringBuilder logMessageBuilder = new StringBuilder();

                if (this.Buffer != null)
                {
                    logMessageBuilder.Append(this.Buffer);
                    this.Buffer = null;
                }

                logMessageBuilder.Append(message);

                string logMessageString = logMessageBuilder.ToString();
                int lastIndex = logMessageString.LastIndexOf("\n", StringComparison.Ordinal);

                string logMessage = null;
                string remaining = null;

                if (lastIndex == -1)
                {
                    remaining = logMessageString;
                }
                else if (lastIndex == (logMessageString.Length - 1))
                {
                    logMessage = logMessageString.TrimEnd('\r', '\n');
                }
                else
                {
                    logMessage = logMessageString.Substring(0, lastIndex).TrimEnd('\r', '\n');
                    remaining = logMessageString.Substring(lastIndex + 1);
                }

                this.Buffer = remaining;

                if (logMessage != null)
                {
                    if (this.LogService == null)
                    {
                        LogLocator.Log(LogLevel.Debug, this.Source, logMessage);
                    }
                    else
                    {
                        this.LogService.Log(LogLevel.Debug, this.Source, logMessage);
                    }
                }
            }
        }

        private void WriteTraceLine (string message)
        {
            this.WriteTrace(message + Environment.NewLine);
        }

        #endregion




        #region Overrides

        /// <inheritdoc />
        public override bool IsThreadSafe => true;

        /// <inheritdoc />
        public override void Write (string message)
        {
            this.WriteTrace(message);
        }

        /// <inheritdoc />
        public override void WriteLine (string message)
        {
            this.WriteTraceLine(message);
        }

        #endregion




        #region Interface: ISynchronizable

        /// <inheritdoc />
        public bool IsSynchronized => true;

        /// <inheritdoc />
        public object SyncRoot { get; }

        #endregion
    }
}
