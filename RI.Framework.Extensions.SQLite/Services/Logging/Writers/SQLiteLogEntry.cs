using System;

using RI.Framework.Services.Logging.Readers;
using RI.Framework.Utilities.Logging;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Services.Logging.Writers
{
    /// <summary>
    ///     Represents a single log entry in a SQLite log database.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    [Serializable]
    public sealed class SQLiteLogEntry : ICloneable<SQLiteLogEntry>, ICloneable
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="SQLiteLogEntry" />.
        /// </summary>
        public SQLiteLogEntry ()
            : this(null)
        {
        }

        /// <summary>
        ///     Creates a new instance of <see cref="SQLiteLogEntry" />.
        /// </summary>
        /// <param name="logFileEntry"> The log file entry from which the content is copied (can be null). </param>
        public SQLiteLogEntry (LogFileEntry logFileEntry)
        {
            if (logFileEntry != null)
            {
                this.Timestamp = logFileEntry.Timestamp;
                this.ThreadId = logFileEntry.ThreadId;
                this.Severity = logFileEntry.Severity;
                this.Source = logFileEntry.Source;
                this.Message = logFileEntry.Message;
            }
        }

        #endregion




        #region Instance Properties/Indexer

        /// <summary>
        ///     Gets or sets the actual log message.
        /// </summary>
        /// <value>
        ///     The actual log message.
        /// </value>
        public string Message { get; set; }

        /// <summary>
        ///     Gets or sets the session this log entry belongs to.
        /// </summary>
        /// <value>
        ///     The session this log entry belongs to.
        /// </value>
        public string Session { get; set; }

        /// <summary>
        ///     Gets or sets the severity.
        /// </summary>
        /// <value>
        ///     The severity.
        /// </value>
        public LogLevel Severity { get; set; }

        /// <summary>
        ///     Gets or sets the source.
        /// </summary>
        /// <value>
        ///     The source.
        /// </value>
        public string Source { get; set; }

        /// <summary>
        ///     Gets or sets the thread ID.
        /// </summary>
        /// <value>
        ///     The thread ID.
        /// </value>
        public int ThreadId { get; set; }

        /// <summary>
        ///     Gets or sets the timestamp of the log entry.
        /// </summary>
        /// <value>
        ///     The timestamp of the log entry.
        /// </value>
        public DateTime Timestamp { get; set; }

        #endregion




        #region Interface: ICloneable<SQLiteLogEntry>

        /// <inheritdoc />
        public SQLiteLogEntry Clone ()
        {
            SQLiteLogEntry clone = new SQLiteLogEntry();
            clone.Timestamp = this.Timestamp;
            clone.ThreadId = this.ThreadId;
            clone.Severity = this.Severity;
            clone.Source = this.Source;
            clone.Message = this.Message;
            clone.Session = this.Session;
            return clone;
        }

        /// <inheritdoc />
        object ICloneable.Clone ()
        {
            return this.Clone();
        }

        #endregion
    }
}
