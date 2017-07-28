using RI.Framework.Services.Logging;

namespace RI.Framework.Utilities.Logging
{
	/// <summary>
	///     Defines the interface for types to implement if they are the source of log messages and need to utilize logging.
	/// </summary>
	/// <remarks>
	///     <para>
	///         <see cref="ILogSource"/> only defines the type/instance specific data and dependencies which are required for logging.
	/// The actual logging is performed by extension methods implemented in <see cref="ILogSourceExtensions"/>.
	///     </para>
	/// </remarks>
	public interface ILogSource
	{
		/// <summary>
		/// Gets or sets whether the logging for this instance is enabled or not.
		/// </summary>
		/// <value>
		/// true if the logging for this instance is enabled, false otherwise.
		/// </value>
		/// <remarks>
		/// <note type="implement">
		/// The default value is true.
		/// </note>
		/// </remarks>
		bool LoggingEnabled { get; set; }

		/// <summary>
		/// Gets or sets the used logger for this instance.
		/// </summary>
		/// <value>
		/// The used logger for this instance or null if no logger is used.
		/// </value>
		/// <remarks>
		/// <note type="implement">
		/// The default value is <see cref="LogLocator"/>.<see cref="LogLocator.Logger"/>.
		/// </note>
		/// </remarks>
		ILogger Logger { get; set; }
	}
}
