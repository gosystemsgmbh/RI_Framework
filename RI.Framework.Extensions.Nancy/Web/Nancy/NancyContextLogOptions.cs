using System;




namespace RI.Framework.Web.Nancy
{
	/// <summary>
	///     Defines options for logging the current state of a context.
	/// </summary>
	[Serializable]
	[Flags]
	public enum NancyContextLogOptions
	{
		/// <summary>
		///     No options.
		/// </summary>
		None = 0x00,

		/// <summary>
		///     Start or restart time measurements.
		/// </summary>
		StartTimeMeasurement = 0x01,

		/// <summary>
		///     Measure time since last time measurement start (<see cref="StartTimeMeasurement" />) and append to log message.
		/// </summary>
		MeasureTime = 0x02,

		/// <summary>
		///     Include response information in the log message.
		/// </summary>
		IncludeResponse = 0x04,
	}
}
