using System;

using RI.Framework.Composition.Model;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Services.Logging
{
	/// <summary>
	///     Defines the interface for a log writer.
	/// </summary>
	/// <remarks>
	///     <para>
	///         A log writer acts as an actual target for writing log messages as produced by an <see cref="ILogService" />.
	///     </para>
	/// </remarks>
	[Export]
	public interface ILogWriter : ISynchronizable
	{
		/// <summary>
		///     Gets or sets the used log filter.
		/// </summary>
		/// <value>
		///     The used log filter or null if no log filter is used.
		/// </value>
		/// <remarks>
		///     <para>
		///         The log filter applied here filters all messages passed to this log writer, independently of other log filters of other log writers.
		///     </para>
		/// </remarks>
		ILogFilter Filter { get; set; }

		/// <summary>
		///     Performs a cleanup of old log messages.
		/// </summary>
		/// <param name="retentionDate"> The date and time from which all older log messages are to be cleaned up. </param>
		/// <remarks>
		///     <note type="implement">
		///         If the implemented log writer does not support cleanup of old log messages, this method should do simply nothing.
		///     </note>
		/// </remarks>
		void Cleanup (DateTime retentionDate);

		/// <summary>
		///     Writes a log message.
		/// </summary>
		/// <param name="timestamp"> The timestamp the log message is associated with. </param>
		/// <param name="threadId"> The threadId the log message is associated with. </param>
		/// <param name="severity"> The severity of the log message. </param>
		/// <param name="source"> The source of the log message. </param>
		/// <param name="message"> The log message. </param>
		/// <remarks>
		///     <para>
		///         The <paramref name="threadId" /> is not necessarily a technical relevant ID and its meaning is undefined.
		///         It is only used to distinguish log messages coming from different threads.
		///     </para>
		/// </remarks>
		void Log (DateTime timestamp, int threadId, LogLevel severity, string source, string message);
	}
}
