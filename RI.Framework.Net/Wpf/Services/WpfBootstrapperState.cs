using System;




namespace RI.Framework.Wpf.Services
{
	/// <summary>
	///     Describes the current state of a WPF application bootstrapper.
	/// </summary>
	[Serializable]
	public enum WpfBootstrapperState
	{
		/// <summary>
		///     The bootstrapper is unitialized (<see cref="WpfBootstrapper.Run" /> was not called).
		/// </summary>
		Uninitialized = 0,

		/// <summary>
		///     The bootstrapping is in progress (<see cref="WpfBootstrapper.Run" /> was called but the application is not yet running).
		/// </summary>
		Bootstrapping = 1,

		/// <summary>
		///     The application is running.
		/// </summary>
		Running = 2,

		/// <summary>
		///     The shutdown is in progress (<see cref="WpfBootstrapper.Shutdown" /> was called).
		/// </summary>
		ShuttingDown = 3,

		/// <summary>
		///     The shutdown is finished (<see cref="WpfBootstrapper.Run" /> returned).
		/// </summary>
		ShutDown = 4,
	}
}
