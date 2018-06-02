using System;
using System.Threading;




namespace RI.Framework.Utilities.Logging
{
	/// <summary>
	///     Defines the interface for a logger which can be used for writing and/or persisting log messages.
	/// </summary>
	public interface ILogger
	{
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
		///         Usually, it is the value of <see cref="Thread.ManagedThreadId" />
		///     </para>
		/// </remarks>
		void Log (DateTime timestamp, int threadId, LogLevel severity, string source, string format, params object[] args);
	}
}
