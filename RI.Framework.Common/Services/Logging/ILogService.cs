using System;
using System.Collections.Generic;

using RI.Framework.Composition.Model;
using RI.Framework.Services.Logging.Filters;
using RI.Framework.Services.Logging.Writers;
using RI.Framework.Utilities.Logging;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Services.Logging
{
    /// <summary>
    ///     Defines the interface for a logging service.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         A logging service provides logging to one or more targets, represented as <see cref="ILogWriter" />s.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="true" instance="true" />
    /// TODO: Implement DictionaryLogFilter
    /// TODO: Support for thread name
    [Export]
    public interface ILogService : ILogger, ISynchronizable
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
