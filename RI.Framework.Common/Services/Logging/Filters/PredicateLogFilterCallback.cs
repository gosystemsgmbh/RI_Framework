using System;

using RI.Framework.Utilities.Logging;




namespace RI.Framework.Services.Logging.Filters
{
    /// <summary>
    ///     Defines the callback used by <see cref="PredicateLogFilter" /> as the predicate.
    /// </summary>
    /// <param name="timestamp"> The timestamp the log message is associated with. </param>
    /// <param name="threadId"> The threadId the log message is associated with. </param>
    /// <param name="severity"> The severity of the message. </param>
    /// <param name="source"> The source of the message. </param>
    /// <returns>
    ///     true if the log entry is to be written, false otherwise.
    /// </returns>
    /// <threadsafety static="false" instance="false" />
    public delegate bool PredicateLogFilterCallback (DateTime timestamp, int threadId, LogLevel severity, string source);
}
