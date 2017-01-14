namespace RI.Framework.Services.Logging
{
	/// <summary>
	/// Interface implemented by types which supports logging through <see cref="ILogService"/> and also allows some controll over its logging behaviour.
	/// </summary>
	public interface ISupportsLoggingExtended : ISupportsLogging
	{
		/// <summary>
		/// Gets or sets whether logging is enabled for the implementing type.
		/// </summary>
		/// <value>
		/// true if the implementing type is currently logging, false otherwise.
		/// </value>
		/// <remarks>
		/// <note type="implement">
		/// The default value, true or false, depends on the actual type which implements <see cref="ISupportsLoggingExtended"/>
		/// </note>
		/// </remarks>
		bool LoggingEnabled { get; set; }
	}
}
