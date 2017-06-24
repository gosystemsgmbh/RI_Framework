using System;
using System.Collections.Generic;

using RI.Framework.Composition.Model;




namespace RI.Framework.Services.Logging
{
	/// <summary>
	///     Defines the interface for a logging service.
	/// </summary>
	/// <remarks>
	///     <para>
	///         A logging service provides logging to one or more targets, represented as <see cref="ILogWriter" />s.
	///     </para>
	/// <note type="implement">
	/// Note that logging is usually to be used from any thread.
	/// Therefore, the logging service must be partially thread-safe, at least for the actual log and cleanup operations.
	/// However, logging service implementations can rely upon the thread-safety of <see cref="ILogWriter"/>.
	/// </note>
	/// </remarks>
	[Export]
	public interface ILogService
	{
		/// <summary>
		///     Gets or sets the used global log filter.
		/// </summary>
		/// <value>
		///     The used log filter or null if no log filter is used globally.
		/// </value>
		/// <remarks>
		///     <para>
		///         The log filter applied here filters globally over all messages handled by this log service, regardless of the used log writers.
		///     </para>
		/// </remarks>
		ILogFilter Filter { get; set; }

		/// <summary>
		///     Gets all currently available log writers.
		/// </summary>
		/// <value>
		///     All currently available log writers.
		/// </value>
		/// <remarks>
		///     <note type="implement">
		///         The value of this property must never be null.
		///     </note>
		/// </remarks>
		IEnumerable<ILogWriter> Writers { get; }

		/// <summary>
		///     Adds a log writer and starts using it for all subsequent logging.
		/// </summary>
		/// <param name="logWriter"> The log writer to add. </param>
		/// <remarks>
		///     <note type="implement">
		///         Specifying an already added log writer should have no effect.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="logWriter" /> is null. </exception>
		void AddWriter (ILogWriter logWriter);

		/// <summary>
		///     Performs a cleanup of old log messages.
		/// </summary>
		/// <param name="retentionDate"> The date and time from which all older log messages are to be cleaned up. </param>
		/// <remarks>
		///     <note type="note">
		///         The actual cleanup and whether it is possible at all depends on the individual <see cref="ILogWriter" />.
		///     </note>
		/// </remarks>
		void Cleanup (DateTime retentionDate);

		/// <summary>
		///     Performs a cleanup of old log messages.
		/// </summary>
		/// <param name="retentionTime"> The time span of messages from now into the past which are to be kept. </param>
		/// <remarks>
		///     <note type="note">
		///         The actual cleanup and whether it is possible at all depends on the individual <see cref="ILogWriter" />.
		///     </note>
		/// </remarks>
		void Cleanup (TimeSpan retentionTime);

		/// <summary>
		///     Logs a message.
		/// </summary>
		/// <param name="severity"> The severity of the message. </param>
		/// <param name="source"> The source of the message. </param>
		/// <param name="format"> The message. </param>
		/// <param name="args"> The arguments which will be expanded into the message (comparable to <see cref="string.Format(string, object[])" />). </param>
		void Log (LogLevel severity, string source, string format, params object[] args);

		/// <summary>
		///     Logs a message.
		/// </summary>
		/// <param name="timestamp"> The timestamp the log message is associated with. </param>
		/// <param name="threadId"> The threadId the log message is associated with. </param>
		/// <param name="severity"> The severity of the message. </param>
		/// <param name="source"> The source of the message. </param>
		/// <param name="format"> The message. </param>
		/// <param name="args"> The arguments which will be expanded into the message (comparable to <see cref="string.Format(string, object[])" />). </param>
		/// <remarks>
		///     <para>
		///         The <paramref name="threadId" /> is not necessarily a technical relevant ID and its meaning is undefined.
		///         It is only used to distinguish log messages coming from different threads.
		///     </para>
		/// </remarks>
		void Log (DateTime timestamp, int threadId, LogLevel severity, string source, string format, params object[] args);

		/// <summary>
		///     Removes a log writer and stops using it for all subsequent logging.
		/// </summary>
		/// <param name="logWriter"> The log writer to remove. </param>
		/// <remarks>
		///     <note type="implement">
		///         Specifying an already removed log writer should have no effect.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="logWriter" /> is null. </exception>
		void RemoveWriter (ILogWriter logWriter);
	}
}
