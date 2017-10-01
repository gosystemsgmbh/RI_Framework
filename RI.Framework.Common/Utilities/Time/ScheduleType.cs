using System;




namespace RI.Framework.Utilities.Time
{
	/// <summary>
	/// Defines the schedule type of an event.
	/// </summary>
	[Serializable]
	public enum ScheduleType
	{
		/// <summary>
		/// The event is scheduled to occur only once.
		/// </summary>
		Once = 0,

		/// <summary>
		/// The event is scheduled to occur daily.
		/// </summary>
		Daily = 1,

		/// <summary>
		/// The event is scheduled to occur repeatedly.
		/// </summary>
		Repeated = 2,
	}
}