using System;
using System.Collections.Generic;

using RI.Framework.ComponentModel;
using RI.Framework.Services.Logging.Filters;
using RI.Framework.Services.Logging.Writers;
using RI.Framework.Utilities.Logging;




namespace RI.Framework.Services.Logging
{
    /// <summary>
    ///     Provides a centralized and global logging provider.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         <see cref="LogLocator" /> is merely a convenience utility as it uses <see cref="ServiceLocator" /> to retrieve and use a <see cref="ILogService" />.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="true" instance="true" />
    /// TODO: Do not use locator types
    public static class LogLocator
    {
        #region Static Constructor/Destructor

        static LogLocator ()
        {
            LogLocator.Logger = new LogLocatorLogger();
        }

        #endregion




        #region Static Properties/Indexer

        /// <inheritdoc cref="ILogService.Filter" />
        public static ILogFilter Filter => LogLocator.Service?.Filter;

        /// <summary>
        ///     Gets whether a logging service is available and can be used by <see cref="LogLocator" />.
        /// </summary>
        /// <value>
        ///     true if a logging service is available and can be used by <see cref="LogLocator" />, false otherwise.
        /// </value>
        public static bool IsAvailable => LogLocator.Service != null;

        /// <summary>
        ///     Gets a logger which uses <see cref="LogLocator" />.
        /// </summary>
        /// <value>
        ///     A logger which uses <see cref="LogLocator" />.
        /// </value>
        public static ILogger Logger { get; }

        /// <summary>
        ///     Gets the available logging service.
        /// </summary>
        /// <value>
        ///     The available logging service or null if no logging service is available.
        /// </value>
        public static ILogService Service => ServiceLocator.GetInstance<ILogService>();

        /// <inheritdoc cref="ILogService.Writers" />
        public static IEnumerable<ILogWriter> Writers => LogLocator.Service?.Writers ?? new ILogWriter[0];

        #endregion




        #region Static Methods

        /// <inheritdoc cref="ILogService.Cleanup(DateTime)" />
        public static void Cleanup (DateTime retentionDate) => LogLocator.Service?.Cleanup(retentionDate);

        /// <inheritdoc cref="ILogService.Cleanup(TimeSpan)" />
        public static void Cleanup (TimeSpan retentionTime) => LogLocator.Service?.Cleanup(retentionTime);

        /// <inheritdoc cref="M:ILogger.Log(LogLevel,string,string,object[])" />
        public static void Log (LogLevel severity, string source, string format, params object[] args) => LogLocator.Service?.Log(severity, source, format, args);

        /// <inheritdoc cref="M:ILogger.Log(DateTime,int,LogLevel,string,string,object[])" />
        public static void Log (DateTime timestamp, int threadId, string threadName, LogLevel severity, string source, string format, params object[] args) => LogLocator.Service?.Log(timestamp, threadId, threadName, severity, source, format, args);

        #endregion




        #region Type: LogLocatorLogger

        private sealed class LogLocatorLogger : ILogger
        {
            #region Interface: ILogger

            void ILogger.Log (LogLevel severity, string source, string format, params object[] args)
            {
                LogLocator.Log(severity, source, format, args);
            }

            void ILogger.Log (DateTime timestamp, int threadId, string threadName, LogLevel severity, string source, string format, params object[] args)
            {
                LogLocator.Log(timestamp, threadId, threadName, severity, source, format, args);
            }

            #endregion
        }

        #endregion
    }
}
