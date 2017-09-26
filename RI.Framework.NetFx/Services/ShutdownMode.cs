using System;




namespace RI.Framework.Services
{
	/// <summary>
	///     Describes the desired shutdown mode returned by a bootstrapper.
	/// </summary>
	[Serializable]
	public enum ShutdownMode
	{
		/// <summary>
		///     Exits the application.
		/// </summary>
		ExitApplication = 1,

		/// <summary>
		///     Exits the application and runs a script.
		/// </summary>
		ExitApplicationAndRunScript = 2,

		/// <summary>
		///     Restarts the application.
		/// </summary>
		RestartApplication = 3,

		/// <summary>
		///     Exits the application and reboots the machine.
		/// </summary>
		RebootMachine = 4,

		/// <summary>
		///     Exits the application and shuts down the machine.
		/// </summary>
		ShutdownMachine = 5,
	}
}
