using System;




namespace RI.Framework.Services
{
	/// <summary>
	///     Describes the current state of a bootstrapper.
	/// </summary>
	[Serializable]
	public enum BootstrapperState
	{
		/// <summary>
		///     The bootstrapper is unitialized.
		/// </summary>
		Uninitialized = 0,

		/// <summary>
		///     The bootstrapping is in progress.
		/// </summary>
		Bootstrapping = 1,

		/// <summary>
		///     The bootstrapping is finished.
		/// </summary>
		Running = 2,

		/// <summary>
		///     The shutdown is in progress.
		/// </summary>
		ShuttingDown = 3,

		/// <summary>
		///     The shutdown is finished.
		/// </summary>
		ShutDown = 4,
	}
}
