using RI.Framework.Services.Logging;




namespace RI.Framework.Utilities.Logging
{
    /// <summary>
    ///     Defines the interface for types which create log messages using an <see cref="ILogger"/>.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         <see cref="ILogSource" /> only defines the configuration properties for the logging.
    ///         The actual logging is performed by extension methods implemented in <see cref="ILogSourceExtensions" />.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    public interface ILogSource
    {
        /// <summary>
        ///     Gets or sets the minimum severity required for log messages to be logged by this instance.
        /// </summary>
        /// <value>
        ///     The minimum severity required for log messages to be logged by this instance.
        /// </value>
        /// <remarks>
        ///     <note type="implement">
        ///         Unless noted otherwise, the default value shall be <see cref="LogLevel.Debug" />.
        ///     </note>
        /// </remarks>
        LogLevel LogFilter { get; set; }

        /// <summary>
        ///     Gets or sets the used logger for this instance.
        /// </summary>
        /// <value>
        ///     The used logger for this instance or null if no logger is used.
        /// </value>
        /// <remarks>
        ///     <note type="implement">
        ///         Unless noted otherwise, the default value shall be <see cref="LogLocator" />.<see cref="LogLocator.Logger" />.
        ///     </note>
        /// </remarks>
        ILogger Logger { get; set; }

        /// <summary>
        ///     Gets or sets whether the logging for this instance is enabled or not.
        /// </summary>
        /// <value>
        ///     true if the logging for this instance is enabled, false otherwise.
        /// </value>
        /// <remarks>
        ///     <note type="implement">
        ///         Unless noted otherwise, the default value shall be true.
        ///     </note>
        /// </remarks>
        bool LoggingEnabled { get; set; }
    }
}
