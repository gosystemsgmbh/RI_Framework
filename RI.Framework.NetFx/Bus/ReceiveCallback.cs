namespace RI.Framework.Bus
{
	/// <summary>
	/// The callback delegate used for receiving messages.
	/// </summary>
	/// <param name="address">The address the message was sent to.</param>
	/// <param name="payload">The payload or null if no payload was sent.</param>
	/// <returns>
	/// The response or null if no response is to be sent.
	/// </returns>
	public delegate object ReceiveCallback (string address, object payload);

	/// <summary>
	/// The callback delegate used for receiving messages.
	/// </summary>
	/// <typeparam name="TPayload">The type of the payload.</typeparam>
	/// <typeparam name="TResponse">The type of the response.</typeparam>
	/// <param name="address">The address the message was sent to.</param>
	/// <param name="payload">The payload or null if no payload was sent.</param>
	/// <returns>
	/// The response or null if no response is to be sent.
	/// </returns>
	public delegate TResponse ReceiveCallback<in TPayload, out TResponse> (string address, TPayload payload)
		where TPayload : class
		where TResponse : class;
}
