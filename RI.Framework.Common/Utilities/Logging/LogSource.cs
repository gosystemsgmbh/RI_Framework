using RI.Framework.Services.Logging;

namespace RI.Framework.Utilities.Logging
{
	/// <summary>
	/// Provides a default implementation for <see cref="ILogSource"/>.
	/// </summary>
	public abstract class LogSource : ILogSource
	{
		/// <inheritdoc />
		public bool LoggingEnabled { get; set; } = true;

		/// <inheritdoc />
		public ILogger Logger { get; set; } = LogLocator.Logger;
	}
}
