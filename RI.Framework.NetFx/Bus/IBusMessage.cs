using System;

namespace RI.Framework.Bus
{
	public interface IBusMessage
	{
		/// <summary>
		/// Gets the unique ID of the transmission.
		/// </summary>
		/// <value>
		/// The unique ID of the transmission.
		/// </value>
		Guid Transmission { get; }

		/// <summary>
		/// Gets the unique ID of the senders endpoint.
		/// </summary>
		/// <value>
		/// The unique ID of the senders endpoint.
		/// </value>
		Guid SenderEndpoint { get; }

		/// <summary>
		/// Gets the unique ID of the senders node.
		/// </summary>
		/// <value>
		/// The unique ID of the senders node.
		/// </value>
		Guid SenderNode { get; }

		/// <summary>
		/// Gets the message type.
		/// </summary>
		/// <value>
		/// The message type.
		/// </value>
		Type Type { get; }

		/// <summary>
		/// Gets the message.
		/// </summary>
		/// <value>
		/// The message.
		/// </value>
		object Message { get; }
	}
}
