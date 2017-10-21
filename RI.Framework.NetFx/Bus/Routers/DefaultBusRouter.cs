using System;

using RI.Framework.Bus.Connections;
using RI.Framework.Bus.Internals;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Bus.Routers
{
	/// <summary>
	///     Implements a default bus router which is suitable for most scenarios.
	/// </summary>
	/// <remarks>
	///     <para>
	///         See <see cref="IBusRouter" /> for more details.
	///     </para>
	/// </remarks>
	/// <threadsafety static="true" instance="true" />
	public sealed class DefaultBusRouter : IBusRouter
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="DefaultBusRouter" />.
		/// </summary>
		public DefaultBusRouter ()
		{
			this.SyncRoot = new object();
		}

		#endregion




		#region Instance Methods

		private bool CompareTypes (Type source, Type target, bool inheritanceTolerant)
		{
			if ((source == null) && (target == null))
			{
				return true;
			}

			if ((source == null) || (target == null))
			{
				return false;
			}

			if (!inheritanceTolerant)
			{
				return source == target;
			}

			return target.IsAssignableFrom(source);
		}

		#endregion




		#region Interface: IBusRouter

		/// <inheritdoc />
		bool ISynchronizable.IsSynchronized => true;

		/// <inheritdoc />
		public object SyncRoot { get; set; }

		/// <inheritdoc />
		public bool ForwardToGlobal (MessageItem message) => message.ToGlobal && (!message.FromGlobal);

		/// <inheritdoc />
		public bool ForwardToLocal (MessageItem message) => !message.ResponseTo.HasValue;

		/// <inheritdoc />
		public void Initialize (IDependencyResolver dependencyResolver)
		{
		}

		/// <inheritdoc />
		public void ReceivedFromLocal (MessageItem message)
		{
		}

		/// <inheritdoc />
		public void ReceivedFromRemote (MessageItem message, IBusConnection connection)
		{
		}

		/// <inheritdoc />
		public bool ShouldReceive (MessageItem message, ReceiverRegistrationItem receiver)
		{
			bool addressMatch = string.Equals(message.Address, receiver.ReceiverRegistration.Address, StringComparison.OrdinalIgnoreCase);
			bool typeMatch = this.CompareTypes(message.Payload?.GetType(), receiver.ReceiverRegistration.PayloadType, receiver.ReceiverRegistration.IncludeCompatiblePayloadTypes);
			return addressMatch && typeMatch;
		}

		/// <inheritdoc />
		public bool ShouldSend (MessageItem message, IBusConnection connection) => true;

		/// <inheritdoc />
		public void Unload ()
		{
		}

		#endregion
	}
}
