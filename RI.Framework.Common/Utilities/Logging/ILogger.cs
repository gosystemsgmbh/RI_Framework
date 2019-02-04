using System;
using System.Threading;




namespace RI.Framework.Utilities.Logging
{
    /// <summary>
    ///     Defines the interface for a logger which persists log messages.
    /// </summary>
    /// <threadsafety static="true" instance="true" />
    public interface ILogger
    {
        /// <summary>
        ///     Logs a message.
        /// </summary>
        /// <param name="severity"> The severity of the message. </param>
        /// <param name="source"> The source of the message. </param>
        /// <param name="format"> The message format. </param>
        /// <param name="args"> The arguments which will be expanded into <paramref name="format"/>. </param>
        /// <remarks>
        ///     <note type="implement">
        ///         The logger implementation should determine timestamp, thread ID, and thread name on its own, compared to <see cref="Log(DateTime,int,string,LogLevel,string,string,object[])"/>.
        ///     </note>
        /// </remarks>
        void Log (LogLevel severity, string source, string format, params object[] args);

        /// <summary>
        ///     Logs a message.
        /// </summary>
        /// <param name="timestamp"> The timestamp the log message is associated with. </param>
        /// <param name="threadId"> The thread ID the log message is associated with. </param>
        /// <param name="threadName"> The thread name the log message is associated with or null if the thread name is not provided. </param>
        /// <param name="severity"> The severity of the message. </param>
        /// <param name="source"> The source of the message. </param>
        /// <param name="format"> The message format. </param>
        /// <param name="args"> The arguments which will be expanded into <paramref name="format"/>. </param>
        /// <remarks>
        ///     <para>
        ///         The <paramref name="threadId" /> and <paramref name="threadName"/> are not necessarily technical relevant and their meaning is undefined.
        ///         They are only used to distinguish log messages coming from different threads.
        ///         Usually, it is the value of <see cref="Thread.ManagedThreadId" /> and <see cref="Thread.Name"/> respectively.
        ///     </para>
        /// </remarks>
        void Log (DateTime timestamp, int threadId, string threadName, LogLevel severity, string source, string format, params object[] args);
    }
}
