﻿using System;




namespace RI.Framework.Bus
{
    /// <summary>
    ///     Delegate used to handle exceptions thrown by send operations.
    /// </summary>
    /// <param name="sendOperation"> The send operation which resulted in an exception. </param>
    /// <param name="exception"> The exception which occurred during the send operation. </param>
    /// <param name="response"> The used response of the send operation in case the exception is handled. This can be modified by the delegate. </param>
    /// <returns>
    ///     true if the exception was handled, false otherwise.
    /// </returns>
    /// <remarks>
    ///     <para>
    ///         If the exception was handled, the exception will not be visible to the send operation issuer and the response of the send operation will be <paramref name="response"/>.
    ///     </para>
    ///     <para>
    ///         See <see cref="IBus" /> for more details about exception handling and forwarding.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="true" instance="true" />
    /// TODO: Use this
    public delegate bool SendExceptionHandler (SendOperation sendOperation, Exception exception, ref object response);
}