using System;
using System.Threading;




namespace RI.Framework.Utilities.Logging
{
    /// <summary>
    ///     Provides extension methods for the <see cref="ILogSource" /> interface to add simple logging to types which implement <see cref="ILogSource" />.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    public static class ILogSourceExtensions
    {
        #region Static Methods

        /// <summary>
        ///     Logs a message.
        /// </summary>
        /// <param name="source"> The source of the message. </param>
        /// <param name="severity"> The severity of the message. </param>
        /// <param name="format"> The message format. </param>
        /// <param name="args"> The arguments which will be expanded into <paramref name="format"/>. </param>
        /// <remarks>
        ///     <note type="implement">
        ///         The logger implementation should determine timestamp, thread ID, and thread name on its own, compared to <see cref="Log(ILogSource,DateTime,int,string,LogLevel,string,object[])"/>.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is null.</exception>
        public static void Log (this ILogSource source, LogLevel severity, string format, params object[] args)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (format == null)
            {
                return;
            }

            if (!source.LoggingEnabled)
            {
                return;
            }

            if (source.Logger == null)
            {
                return;
            }

            if (severity < source.LogFilter)
            {
                return;
            }

            source.Logger.Log(severity, source.GetType().Name, format, args);
        }

        /// <summary>
        ///     Logs a message.
        /// </summary>
        /// <param name="source"> The source of the message. </param>
        /// <param name="timestamp"> The timestamp the log message is associated with. </param>
        /// <param name="threadId"> The thread ID the log message is associated with. </param>
        /// <param name="threadName"> The thread name the log message is associated with or null if the thread name is not provided. </param>
        /// <param name="severity"> The severity of the message. </param>
        /// <param name="format"> The message format. </param>
        /// <param name="args"> The arguments which will be expanded into <paramref name="format"/>. </param>
        /// <remarks>
        ///     <para>
        ///         The <paramref name="threadId" /> and <paramref name="threadName"/> are not necessarily technical relevant and their meaning is undefined.
        ///         They are only used to distinguish log messages coming from different threads.
        ///         Usually, it is the value of <see cref="Thread.ManagedThreadId" /> and <see cref="Thread.Name"/> respectively.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is null.</exception>
        public static void Log (this ILogSource source, DateTime timestamp, int threadId, string threadName, LogLevel severity, string format, params object[] args)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (format == null)
            {
                return;
            }

            if (!source.LoggingEnabled)
            {
                return;
            }

            if (source.Logger == null)
            {
                return;
            }

            if (severity < source.LogFilter)
            {
                return;
            }

            source.Logger.Log(timestamp, threadId, threadName, severity, source.GetType().Name, format, args);
        }

        #endregion
    }
}
