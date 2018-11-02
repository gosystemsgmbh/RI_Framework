using System;
using System.Globalization;

using RI.Framework.Composition.Model;
using RI.Framework.Services.Logging.Filters;
using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;
using RI.Framework.Utilities.Logging;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Services.Logging.Writers
{
    /// <summary>
    ///     Implements a log writer which forwards log messages as a string to a callback.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         See <see cref="ILogWriter" /> for more details.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="true" instance="true" />
    /// TODO: Use specialized delegates for callbacks
    [Export]
    public sealed class StringCallbackLogWriter : ILogWriter
    {
        #region Constants

        /// <summary>
        ///     The default format string.
        /// </summary>
        /// <value>
        ///     The default format string.
        /// </value>
        /// <remarks>
        ///     <para>
        ///         The default format string is <c> [{1}] [{2:D2}] [{5}] [{6}] {7} </c>.
        ///         See <see cref="FormatString" /> for more details.
        ///     </para>
        /// </remarks>
        public const string DefaultFormatString = "[{1}] [{2:D2}] [{5}] [{6}] {7}";

        #endregion




        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="StringCallbackLogWriter" />.
        /// </summary>
        /// <param name="logCallback"> The required log callback. </param>
        /// <remarks>
        ///     <para>
        ///         No cleanup callback is used.
        ///     </para>
        ///     <para>
        ///         <see cref="DefaultFormatString" /> is used as the format string.
        ///         See <see cref="FormatString" /> for more details.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="logCallback" /> is null. </exception>
        public StringCallbackLogWriter (Action<string> logCallback)
            : this(logCallback, null, null)
        {
        }

        /// <summary>
        ///     Creates a new instance of <see cref="StringCallbackLogWriter" />.
        /// </summary>
        /// <param name="logCallback"> The required log callback. </param>
        /// <param name="cleanupCallback"> The optional cleanup callback. Can be null if not used. </param>
        /// <param name="formatString"> The optional format string to format the log message. Can be null to use <see cref="DefaultFormatString" />. See <see cref="FormatString" /> for more details. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="logCallback" /> is null. </exception>
        /// <exception cref="EmptyStringArgumentException"> <paramref name="formatString" /> is an empty string. </exception>
        public StringCallbackLogWriter (Action<string> logCallback, Action<DateTime> cleanupCallback, string formatString)
        {
            this.SyncRoot = new object();

            if (logCallback == null)
            {
                throw new ArgumentNullException(nameof(logCallback));
            }

            if (formatString != null)
            {
                if (formatString.IsNullOrEmptyOrWhitespace())
                {
                    throw new EmptyStringArgumentException(nameof(formatString));
                }
            }

            this.LogCallback = logCallback;
            this.CleanupCallback = cleanupCallback;
            this.FormatString = formatString ?? StringCallbackLogWriter.DefaultFormatString;
        }

        #endregion




        #region Instance Fields

        private ILogFilter _filter;

        private string _formatString;

        #endregion




        #region Instance Properties/Indexer

        /// <summary>
        ///     Gets the cleanup callback which is called for each cleanup.
        /// </summary>
        /// <value>
        ///     The cleanup callback which is called for each cleanup.
        /// </value>
        public Action<DateTime> CleanupCallback { get; }

        /// <summary>
        ///     Gets or sets the format string used to format the log message.
        /// </summary>
        /// <value>
        ///     The format string used to format the log message.
        /// </value>
        /// <exception cref="ArgumentNullException"> <paramref name="value" /> is null. </exception>
        /// <exception cref="EmptyStringArgumentException"> <paramref name="value" /> is an empty string. </exception>
        /// <remarks>
        ///     <para>
        ///         The format string is given several arguments (not all have to be used):
        ///         {0}: The timestamp of the log entry as a <see cref="DateTime" />.
        ///         {1}: The timestamp of the log entry as a sortable <see cref="string" />.
        ///         {2}: The thread ID as an <see cref="int" />.
        ///         {3}: The severity as an <see cref="int" />.
        ///         {4}: The severity as a <see cref="string" />.
        ///         {5}: The first character of {4} as an uppercase <see cref="string" />.
        ///         {6}: The source as a <see cref="string" />.
        ///         {7}: The actual log message as a <see cref="string" />.
        ///     </para>
        /// </remarks>
        public string FormatString
        {
            get
            {
                lock (this.SyncRoot)
                {
                    return this._formatString;
                }
            }
            set
            {
                lock (this.SyncRoot)
                {
                    if (value == null)
                    {
                        throw new ArgumentNullException(nameof(value));
                    }

                    if (value.IsNullOrEmptyOrWhitespace())
                    {
                        throw new EmptyStringArgumentException(nameof(value));
                    }

                    this._formatString = value;
                }
            }
        }

        /// <summary>
        ///     Gets the log callback which is called for each log entry.
        /// </summary>
        /// <value>
        ///     The log callback which is called for each log entry.
        /// </value>
        public Action<string> LogCallback { get; }

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
        public object SyncRoot { get; }

        /// <inheritdoc />
        public void Cleanup (DateTime retentionDate)
        {
            this.CleanupCallback?.Invoke(retentionDate);
        }

        /// <inheritdoc />
        public void Log (DateTime timestamp, int threadId, LogLevel severity, string source, string message)
        {
            ILogFilter filter = this.Filter;
            if (filter != null)
            {
                if (!filter.Filter(timestamp, threadId, severity, source))
                {
                    return;
                }
            }

            source = source ?? "[null]";
            message = message ?? string.Empty;

            string messageToLog = string.Format(CultureInfo.InvariantCulture, this.FormatString, timestamp, timestamp.ToSortableString('-'), threadId, (int)severity, severity.ToString(), severity.ToString()[0].ToString(), source, message);
            this.LogCallback(messageToLog);
        }

        #endregion
    }
}
