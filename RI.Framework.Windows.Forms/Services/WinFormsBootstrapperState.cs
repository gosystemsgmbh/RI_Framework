using System;




namespace RI.Framework.Services
{
	/// <summary>
	///     Describes the current state of a Windows Forms application bootstrapper.
	/// </summary>
	[Serializable]
	public enum WinFormsBootstrapperState
	{
		/// <summary>
		///     The bootstrapper is unitialized (<see cref="WinFormsBootstrapper.Run" /> was not called).
		/// </summary>
		Uninitialized = 0,

		/// <summary>
		///     The bootstrapping is in progress (<see cref="WinFormsBootstrapper.Run" /> was called but the application is not yet running).
		/// </summary>
		Bootstrapping = 1,

		/// <summary>
		///     The application is running.
		/// </summary>
		Running = 2,

		/// <summary>
		///     The shutdown is in progress (<see cref="WinFormsBootstrapper.Shutdown" /> was called).
		/// </summary>
		ShuttingDown = 3,

		/// <summary>
		///     The shutdown is finished (<see cref="WinFormsBootstrapper.Run" /> returned).
		/// </summary>
		ShutDown = 4,
	}
}
