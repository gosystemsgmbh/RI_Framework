using System;




namespace RI.Framework.Bus
{
	/// <summary>
	///     Delegate used to handle exceptions thrown by receivers when processing messages.
	/// </summary>
	/// <param name="address"> The address of the processed message or null if no address is used. </param>
	/// <param name="payload"> The payload of the processed message or null if no payload is used. </param>
	/// <param name="exception"> The exception which was thrown by the receiver during processing of the message. </param>
	/// <param name="forwardException"> Specifies and returns whether the exception should be forwarded to the sender. </param>
	/// <returns>
	///     The response to return to the sender or null if no response is to be returned.
	/// </returns>
	/// <remarks>
	///     <para>
	///         See <see cref="IBus" /> for more details about exception handling and forwarding.
	///     </para>
	/// </remarks>
	public delegate object ReceiverExceptionHandler (string address, object payload, Exception exception, ref bool forwardException);
}
