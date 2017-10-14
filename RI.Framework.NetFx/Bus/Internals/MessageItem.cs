using System;




namespace RI.Framework.Bus.Internals
{
	/// <summary>
	///     Stores a single message.
	/// </summary>
	[Serializable]
	public sealed class MessageItem
	{
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
		public object Payload { get; set; }

		/// <summary>
		///     Gets or sets the unique ID of the message this message responds to.
		/// </summary>
		/// <value>
		///     The unique ID of the message this message responds to.
		/// </value>
		public Guid? ResponseTo { get; set; }

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
	}
}
