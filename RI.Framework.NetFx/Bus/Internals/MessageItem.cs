using System;
using System.Text;

using RI.Framework.Bus.Routers;
using RI.Framework.Utilities;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Bus.Internals
{
	/// <summary>
	///     Stores a single message.
	/// </summary>
	[Serializable]
	public sealed class MessageItem : ICloneable<MessageItem>, ICloneable
	{
		#region Instance Fields

		private object _payload;

		private object _routingInfo;

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets or sets the address of the message.
		/// </summary>
		/// <value>
		///     The address of the message or null if no address is used.
		/// </value>
		public string Address { get; set; }

		/// <summary>
		///     Gets or sets whether the message was received from a remote bus.
		/// </summary>
		/// <value>
		///     true if the message was received from a romote bus through a connection, false otherwise.
		/// </value>
		public bool FromGlobal { get; set; }

		/// <summary>
		///     Gets or sets the unique ID of the message.
		/// </summary>
		/// <value>
		///     The unique ID of the message.
		/// </value>
		public Guid Id { get; set; }

		/// <summary>
		///     Gets or sets whether the message is a broadcast message.
		/// </summary>
		/// <value>
		///     true if the message is a broadcast message, false otherwise.
		/// </value>
		public bool IsBroadcast { get; set; }

		/// <summary>
		///     Gets or sets the payload of the message.
		/// </summary>
		/// <value>
		///     The payload of the message or null if the message does not have a payload.
		/// </value>
		public object Payload
		{
			get
			{
				return this._payload;
			}
			set
			{
				this._payload = value;
				this.PayloadAssembly = value?.GetType().Assembly.GetName().Name;
				this.PayloadType = value?.GetType().FullName;
			}
		}

		/// <summary>
		///     Gets the simple name of the assembly in which the used payload type is defined.
		/// </summary>
		/// <value>
		///     The simple name of the assembly in which the used payload type is defined or null if the message does not have a payload.
		/// </value>
		public string PayloadAssembly { get; private set; }

		/// <summary>
		///     Gets the full name of the payload type.
		/// </summary>
		/// <value>
		///     The full name of the payload type or null if the message does not have a payload.
		/// </value>
		public string PayloadType { get; private set; }

		/// <summary>
		///     Gets or sets the unique ID of the message this message responds to.
		/// </summary>
		/// <value>
		///     The unique ID of the message this message responds to.
		/// </value>
		public Guid? ResponseTo { get; set; }

		/// <summary>
		///     Gets or sets routing information of the message.
		/// </summary>
		/// <value>
		///     The routing information of the message or null if the message does not have routing information.
		/// </value>
		/// <remarks>
		///     <para>
		///         Routing information is optional and only used by <see cref="IBusRouter" />s in cases where they need to exchange routing information.
		///     </para>
		/// </remarks>
		public object RoutingInfo
		{
			get
			{
				return this._routingInfo;
			}
			set
			{
				this._routingInfo = value;
				this.RoutingInfoAssembly = value?.GetType().Assembly.GetName().Name;
				this.RoutingInfoType = value?.GetType().FullName;
			}
		}

		/// <summary>
		///     Gets the simple name of the assembly in which the used routing information type is defined.
		/// </summary>
		/// <value>
		///     The simple name of the assembly in which the used routing information type is defined or null if the message does not have routing information.
		/// </value>
		public string RoutingInfoAssembly { get; private set; }

		/// <summary>
		///     Gets the full name of the routing information type.
		/// </summary>
		/// <value>
		///     The full name of the routing information type or null if the message does not have routing information.
		/// </value>
		public string RoutingInfoType { get; private set; }

		/// <summary>
		///     Gets or sets the UTC timestamp when the message was sent.
		/// </summary>
		/// <value>
		///     The UTC timestamp when the message was sent.
		/// </value>
		public DateTime Sent { get; set; }

		/// <summary>
		///     Gets or sets the timeout of the message.
		/// </summary>
		/// <value>
		///     The timeout of the message in milliseconds.
		/// </value>
		public int Timeout { get; set; }

		/// <summary>
		///     Gets or sets whether the message is intended to be sent globally.
		/// </summary>
		/// <value>
		///     true if the message is intended to be sent globally, false otherwise.
		/// </value>
		public bool ToGlobal { get; set; }

		#endregion




		#region Overrides

		/// <inheritdoc />
		public override string ToString ()
		{
			StringBuilder sb = new StringBuilder();

			sb.Append("Id=");
			sb.Append(this.Id.ToString("N").ToUpperInvariant());
			sb.Append("; ResponseTo=");
			sb.Append(this.ResponseTo?.ToString("N").ToUpperInvariant() ?? "[null]");
			sb.Append("; Address=");
			sb.Append(this.Address ?? "[null]");
			sb.Append("; Payload=");
			sb.Append(this.Payload?.GetType().FullName ?? "[null]");
			sb.Append("; ToGlobal=");
			sb.Append(this.ToGlobal);
			sb.Append("; FromGlobal=");
			sb.Append(this.FromGlobal);
			sb.Append("; IsBroadcast=");
			sb.Append(this.IsBroadcast);
			sb.Append("; Timeout=");
			sb.Append(this.Timeout);
			sb.Append("; Sent=");
			sb.Append(this.Sent.ToSortableString('-'));
			sb.Append("; RoutingInfo=");
			sb.Append(this.RoutingInfo?.ToString() ?? "[null]");

			return sb.ToString();
		}

		#endregion




		#region Interface: ICloneable<MessageItem>

		/// <inheritdoc />
		public MessageItem Clone ()
		{
			MessageItem clone = new MessageItem();
			clone.Address = this.Address;
			clone.FromGlobal = this.FromGlobal;
			clone.Id = this.Id;
			clone.IsBroadcast = this.IsBroadcast;
			clone.Payload = this.Payload;
			clone.ResponseTo = this.ResponseTo;
			clone.RoutingInfo = this.RoutingInfo;
			clone.Sent = this.Sent;
			clone.Timeout = this.Timeout;
			clone.ToGlobal = this.ToGlobal;
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
