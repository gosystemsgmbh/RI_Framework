using System;

namespace RI.Framework.Bus
{
	public interface IBusEndpoint
	{
		/// <summary>
		/// Gets the unique ID of this endpoint.
		/// </summary>
		/// <value>
		/// The unique ID of this endpoint.
		/// </value>
		Guid Id { get; }

		/// <summary>
		/// Called on the endpoint when it receives a message it has subscribed to.
		/// </summary>
		/// <param name="message">The received message.</param>
		void Receive(IBusMessage message);
	}
}
