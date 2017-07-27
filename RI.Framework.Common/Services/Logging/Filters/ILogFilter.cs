using System;

using RI.Framework.Services.Logging.Writers;
using RI.Framework.Utilities.Logging;

namespace RI.Framework.Services.Logging.Filters
{
	/// <summary>
	///     Defines the interface for a log filter.
	/// </summary>
	/// <remarks>
	///     <para>
	///         A log filter can be used to filter log messages to only let certain messages reach the <see cref="ILogWriter" />s for actual logging.
	///     </para>
	///     <para>
	///         Log filters can be applied globally (to a <see cref="ILogService" />) or to specific log writers (<see cref="ILogWriter" />).
	///     </para>
	///     <note type="note">
	///         Log filters are not exported or imported to the log service or log writers, they must be applied explicitly.
	///     </note>
	/// </remarks>
	public interface ILogFilter
	{
		/// <summary>
		///     Determines whether a log entry is to be written or not.
		/// </summary>
		/// <param name="timestamp"> The timestamp the log message is associated with. </param>
		/// <param name="threadId"> The threadId the log message is associated with. </param>
		/// <param name="severity"> The severity of the message. </param>
		/// <param name="source"> The source of the message. </param>
		/// <returns>
		///     true if the log entry is to be written, false otherwise.
		/// </returns>
		bool Filter (DateTime timestamp, int threadId, LogLevel severity, string source);
	}
}
