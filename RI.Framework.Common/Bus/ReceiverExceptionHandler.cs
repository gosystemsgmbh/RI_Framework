using System;




namespace RI.Framework.Bus
{
    /// <summary>
    ///     Delegate used to handle exceptions thrown by receiver registrations.
    /// </summary>
    /// <param name="receiverRegistration"> The receiver registration which threw the exception. </param>
    /// <param name="exception"> The exception which occurred during the receiver registration processing. This can be modified by the delegate. </param>
    /// <param name="forwardException"> Specifies whether the exception should be forwarded to the sender. This can be modified by the delegate. </param>
    /// <param name="response"> The used response to the sender in case the exception is handled. This can be modified by the delegate. </param>
    /// <returns>
    ///     true if the exception was handled, false otherwise.
    /// </returns>
    /// <remarks>
    ///     <para>
    ///         If the exception was handled, the exception will not be visible to the sender and the response will be <paramref name="response"/>.
    ///     </para>
    ///     <para>
    ///         See <see cref="IBus" /> for more details about exception handling and forwarding.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="true" instance="true" />
    public delegate bool ReceiverExceptionHandler (ReceiverRegistration receiverRegistration, ref Exception exception, ref bool forwardException, ref object response);
}
