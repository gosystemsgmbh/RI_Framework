using System;

using RI.Framework.Bus.Connections;
using RI.Framework.Bus.Internals;
using RI.Framework.ComponentModel;
using RI.Framework.Composition.Model;
using RI.Framework.Services.Logging;
using RI.Framework.Utilities.Logging;
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
	[Export]
	public class DefaultBusRouter : IBusRouter
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




		#region Virtuals

		/// <summary>
		///     Compares two types whether they are the same, in the context of payload type and receiver registration.
		/// </summary>
		/// <param name="source"> The type to convert from or the payload respectively. </param>
		/// <param name="target"> The type to convert to or the receiver type respectively. </param>
		/// <param name="inheritanceTolerant"> Specifies whether compatible, assignable types are also considered equal. </param>
		/// <returns>
		///     true if the two types are considered equal, false otherwise.
		/// </returns>
		protected virtual bool CompareTypes (Type source, Type target, bool inheritanceTolerant)
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
		public LogLevel LogFilter { get; set; } = LogLevel.Debug;

		/// <inheritdoc />
		public ILogger Logger { get; set; } = LogLocator.Logger;

		/// <inheritdoc />
		public bool LoggingEnabled { get; set; } = true;

		/// <inheritdoc />
		public object SyncRoot { get; }

		/// <inheritdoc />
		public virtual bool ForwardToGlobal (MessageItem message) => message.ToGlobal && (!message.FromGlobal);

		/// <inheritdoc />
		public virtual bool ForwardToLocal (MessageItem message) => !message.ResponseTo.HasValue;

		/// <inheritdoc />
		public virtual void Initialize (IDependencyResolver dependencyResolver)
		{
		}

		/// <inheritdoc />
		public virtual void ReceivedFromLocal (MessageItem message)
		{
		}

		/// <inheritdoc />
		public virtual void ReceivedFromRemote (MessageItem message, IBusConnection connection)
		{
		}

		/// <inheritdoc />
		public virtual bool ShouldReceive (MessageItem message, ReceiverRegistrationItem receiver)
		{
			bool addressMatch = (receiver.ReceiverRegistration.Address == null) || string.Equals(message.Address, receiver.ReceiverRegistration.Address, StringComparison.OrdinalIgnoreCase);
			bool typeMatch = (receiver.ReceiverRegistration.PayloadType == null) || this.CompareTypes(message.Payload?.GetType(), receiver.ReceiverRegistration.PayloadType, receiver.ReceiverRegistration.IncludeCompatiblePayloadTypes);
			return addressMatch && typeMatch;
		}

		/// <inheritdoc />
		public virtual bool ShouldSend (MessageItem message, IBusConnection connection) => true;

		/// <inheritdoc />
		public virtual void Unload ()
		{
		}

		#endregion
	}
}
