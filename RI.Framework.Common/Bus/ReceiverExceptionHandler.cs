using System;




namespace RI.Framework.Bus
{
    /// <summary>
    ///     Delegate used to handle exceptions thrown by receiver registrations.
    /// </summary>
    /// <param name="address"> The address of the processed message or null if no address is used. </param>
    /// <param name="payload"> The payload of the processed message or null if no payload is used. </param>
    /// <param name="exception"> The exception which occurred in the receiver during processing of the message. This can be modified by the delegate. </param>
    /// <param name="forwardException"> Specifies whether the exception should be forwarded to the sender. This can be modified by the delegate. </param>
    /// <returns>
    ///     The response to return to the sender or null if no response is to be returned.
    /// </returns>
    /// <remarks>
    ///     <para>
    ///         See <see cref="IBus" /> for more details about exception handling and forwarding.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="true" instance="true" />
    /// TODO: BEFORE RELEASE
    /// TODO: Clarify dependency of return value and forwardException
    /// TODO: Change signature to: bool ReceiverExceptionHandler (ReceiverRegistration receiverRegistration, string address, object payload, ref Exception exception, ref object response)
    public delegate object ReceiverExceptionHandler (string address, object payload, ref Exception exception, ref bool forwardException);
}
