using System;

using RI.Framework.Services.Logging.Writers;
using RI.Framework.Utilities;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Services
{
	/// <summary>
	///     Describes the hosting context of an application started by a bootstrapper.
	/// </summary>
	public class HostContext : ICloneable<HostContext>, ICloneable
	{
		#region Instance Properties/Indexer

		/// <summary>
		///     Gets or sets an additional logger which is provided directly by the hosting environment.
		/// </summary>
		/// <value>
		///     An additional logger which is provided directly by the hosting environment or null if no such is available.
		/// </value>
		public ILogWriter Logger { get; set; } = null;

		/// <summary>
		/// Gets or sets the instance ID of the application provided by the hosting environment.
		/// </summary>
		/// <value>
		/// The instance ID of the application provided by the hosting environment or null if the hosting environment does not specify an instance ID (e.g. only one instance is run).
		/// </value>
		public string InstanceId { get; set; } = null;

		#endregion




		#region Interface: ICloneable<HostContext>

		/// <inheritdoc />
		public virtual HostContext Clone ()
		{
			HostContext clone = new HostContext();
			clone.Logger = this.Logger?.CloneOrSelf();
			clone.InstanceId = this.InstanceId;
			return clone;
		}

		/// <inheritdoc />
		object ICloneable.Clone ()
		{
			return this.Clone();
		}

		#endregion
	}
}
