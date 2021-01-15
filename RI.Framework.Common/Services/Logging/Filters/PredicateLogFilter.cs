using System;

using RI.Framework.Utilities.Logging;




namespace RI.Framework.Services.Logging.Filters
{
    /// <summary>
    ///     Implements a log filter based on a predicate which filters each message.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         See <see cref="ILogFilter" /> for more details.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    public class PredicateLogFilter : ILogFilter
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="PredicateLogFilter" />.
        /// </summary>
        /// <param name="predicate"> The used predicate. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="predicate" /> is null. </exception>
        public PredicateLogFilter (PredicateLogFilterCallback predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            this.Predicate = predicate;
        }

        #endregion




        #region Instance Properties/Indexer

        /// <summary>
        ///     Gets the used predicate.
        /// </summary>
        /// <value>
        ///     The used predicate.
        /// </value>
        public PredicateLogFilterCallback Predicate { get; }

        #endregion




        #region Interface: ILogFilter

        /// <inheritdoc />
        public bool Filter (DateTime timestamp, int threadId, LogLevel severity, string source)
        {
            return this.Predicate(timestamp, threadId, severity, source);
        }

        #endregion
    }
}
