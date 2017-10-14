using RI.Framework.Services.Logging.Writers;




namespace RI.Framework.Services
{
	/// <summary>
	///     Describes the hosting context of an application started by a bootstrapper.
	/// </summary>
	public class HostContext
	{
		#region Instance Properties/Indexer

		/// <summary>
		///     Gets or sets the instance ID of the application provided by the hosting environment.
		/// </summary>
		/// <value>
		///     The instance ID of the application provided by the hosting environment or null if the hosting environment does not specify an instance ID (e.g. only one instance is run).
		/// </value>
		public string InstanceId { get; set; } = null;

		/// <summary>
		///     Gets or sets whether the application runs as a service or daemon.
		/// </summary>
		/// <value>
		///     true if the application runs as a service or daemon, false otherwise.
		/// </value>
		public bool IsService { get; set; } = false;

		/// <summary>
		///     Gets or sets an additional logger which is provided directly by the hosting environment.
		/// </summary>
		/// <value>
		///     An additional logger which is provided directly by the hosting environment or null if no such is available.
		/// </value>
		public ILogWriter Logger { get; set; } = null;

		#endregion
	}
}
