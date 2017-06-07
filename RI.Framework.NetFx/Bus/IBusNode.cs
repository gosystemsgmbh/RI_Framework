using System;

namespace RI.Framework.Bus
{
	public interface IBusNode
	{
		/// <summary>
		/// Subscribes a bus endpoint to a specific type of messages.
		/// </summary>
		/// <param name="busEndpoint">The bus endpoint.</param>
		/// <param name="messageType">The message type.</param>
		/// <exception cref="ArgumentNullException"><paramref name="busEndpoint"/> or <paramref name="messageType"/> is null.</exception>
		void Subscribe(IBusEndpoint busEndpoint, Type messageType);

		/// <summary>
		/// Unsubscribes a bus endpoint from a specific type of messages.
		/// </summary>
		/// <param name="busEndpoint">The bus endpoint.</param>
		/// <param name="messageType">The message type.</param>
		/// <exception cref="ArgumentNullException"><paramref name="busEndpoint"/> or <paramref name="messageType"/> is null.</exception>
		void Unsubscribe(IBusEndpoint busEndpoint, Type messageType);

		/// <summary>
		/// Posts a message and sends it to all subscribed bus endpoints.
		/// </summary>
		/// <param name="busEndpoint">The sender.</param>
		/// <param name="messageType">The message type.</param>
		/// <param name="message">The message.</param>
		/// <returns>
		/// The message transmission which can be used for any follow-up on the sent message.
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="busEndpoint"/>, <paramref name="messageType"/>, or <paramref name="message"/> is null.</exception>
		IBusMessageTransmission Post(IBusEndpoint busEndpoint, Type messageType, object message);

		/// <summary>
		/// Gets the unique ID of this node.
		/// </summary>
		/// <value>
		/// The unique ID of this node.
		/// </value>
		Guid Id { get; }
	}
}
