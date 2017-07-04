using System;




namespace RI.Framework.Threading
{
	/// <summary>
	///     Describes the mode of a <see cref="ThreadDispatcherTimer" />.
	/// </summary>
	[Serializable]
	public enum ThreadDispatcherTimerMode
	{
		/// <summary>
		///     The timer is only executed once after the specified interval.
		/// </summary>
		OneShot = 0,

		/// <summary>
		///     The timer is executed repeatedly in the specified interval.
		/// </summary>
		Continuous = 1,
	}
}
