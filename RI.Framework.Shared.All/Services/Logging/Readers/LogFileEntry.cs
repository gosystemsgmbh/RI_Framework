using System;

using RI.Framework.Utilities.Logging;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Services.Logging.Readers
{
	/// <summary>
	///     Represents a single entry in a log file read by <see cref="LogFileReader" />.
	/// </summary>
	public sealed class LogFileEntry : ICloneable<LogFileEntry>, ICloneable
	{
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




		#region Interface: ICloneable<LogFileEntry>

		/// <inheritdoc />
		public LogFileEntry Clone ()
		{
			LogFileEntry clone = new LogFileEntry();
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
