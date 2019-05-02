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
	/// TODO: Add IMessageObject
	[Serializable]
	public sealed class MessageItem : ICloneable<MessageItem>, ICloneable
	{
		#region Static Methods

		/// <summary>
		///     Creates a useful display string from an exception object as transported by <see cref="Exception" />.
		/// </summary>
		/// <param name="exception"> The exception object. </param>
		/// <param name="detailed"> Specifies whether a detailed exception message is to be created, using <see cref="ExceptionExtensions.ToDetailedString(System.Exception)" />, if <paramref name="exception" /> is or inherits from <see cref="System.Exception" />. </param>
		/// <returns>
		///     A display string representing the exception object.
		///     The return value is never null, even if <paramref name="exception" /> is null.
		/// </returns>
		public static string CreateExceptionMessage (object exception, bool detailed)
		{
			if (exception != null)
			{
				Exception ex = exception as Exception;
				if (ex != null)
				{
					return ex.GetType().Name + ": " + (detailed ? ex.ToDetailedString() : ex.Message);
				}
				else
				{
					return exception.GetType().Name + ": " + exception;
				}
			}
			else
			{
				return "null";
			}
		}

		#endregion




		#region Instance Fields

		private object _exception;

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
		///     Gets or sets the exception of the message processing.
		/// </summary>
		/// <value>
		///     The exception of the message processing or null if the message processing did not throw an exception or exception forwarding was not enabled.
		/// </value>
		/// <remarks>
		///     <note type="note">
		///         <see cref="Exception" /> is of type <see cref="object" /> instead of <see cref="System.Exception" /> to allow bus connections to substitute the exception with another object, e.g. for serialization/deserialization reasons.
		///     </note>
		/// </remarks>
		public object Exception
		{
			get
			{
				return this._exception;
			}
			set
			{
				this._exception = value;
				this.ExceptionAssembly = value?.GetType().Assembly.GetName().Name;
				this.ExceptionType = value?.GetType().FullName;
			}
		}

		/// <summary>
		///     Gets the simple name of the assembly in which the used message processing exception type is defined.
		/// </summary>
		/// <value>
		///     The simple name of the assembly in which the used message processing exception type is defined or null if the message does not have an exception.
		/// </value>
		public string ExceptionAssembly { get; private set; }

		/// <summary>
		///     Gets or sets whether exception forwarding to the sender is used when the message is processed by a receiver.
		/// </summary>
		/// <value>
		///     true if exception forwarding is used, false otherwise.
		/// </value>
		public bool ExceptionForwarding { get; set; }

		/// <summary>
		///     Gets the full name of the message processing exception type.
		/// </summary>
		/// <value>
		///     The full name of the message processing exception type or null if the message does not have an exception.
		/// </value>
		public string ExceptionType { get; private set; }

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
			sb.Append("; Exception=[");
			sb.Append(MessageItem.CreateExceptionMessage(this.Exception, false));
			sb.Append("]; RoutingInfo=");
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
			clone.Exception = this.Exception;
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
