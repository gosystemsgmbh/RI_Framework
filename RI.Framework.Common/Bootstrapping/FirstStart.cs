using System;




namespace RI.Framework.Bootstrapping
{
	/// <summary>
	///     Describes the if and how a application is started for the first time by a <see cref="Bootstrapper" />.
	/// </summary>
	[Serializable]
	[Flags]
	public enum FirstStart
	{
		/// <summary>
		///     The application is not started for the first time.
		/// </summary>
		Not = 0x00,

		/// <summary>
		///     The application is started for the first time on this machine.
		/// </summary>
		Machine = 0x01,

		/// <summary>
		///     The application is started for the first time for the current user.
		/// </summary>
		User = 0x02,

		/// <summary>
		///     The application is started for the first time using the current application version.
		///     Note that this first start indication is used for any change of application version upwards or downwards.
		/// </summary>
		Version = 0x04,

		/// <summary>
		///     Combination of <see cref="Machine" />, <see cref="User" />, <see cref="Version" />.
		/// </summary>
		All = FirstStart.Machine | FirstStart.User | FirstStart.Version,
	}
}
