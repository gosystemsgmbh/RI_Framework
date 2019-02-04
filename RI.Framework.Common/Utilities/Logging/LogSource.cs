using RI.Framework.Services.Logging;




namespace RI.Framework.Utilities.Logging
{
    /// <summary>
    ///     Provides a default implementation for <see cref="ILogSource" />.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    /// TODO: Rename to LogSourceBase
    public abstract class LogSource : ILogSource
    {
        #region Interface: ILogSource
  
        /// <inheritdoc />
        public LogLevel LogFilter { get; set; } = LogLevel.Debug;

        /// <inheritdoc />
        public ILogger Logger { get; set; } = LogLocator.Logger;

        /// <inheritdoc />
        public bool LoggingEnabled { get; set; } = true;

        #endregion
    }
}
