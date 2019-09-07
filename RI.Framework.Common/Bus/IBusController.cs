using System;
using System.Collections.Generic;

using RI.Framework.Bus.Connections;
using RI.Framework.Bus.Internals;
using RI.Framework.Bus.Pipelines;
using RI.Framework.Composition.Model;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Bus
{
    /// <summary>
    ///     Provides control of the bus events and operations as presented to the bus user (by <see cref="IBus"/>) to bus infrastructure (such as <see cref="IBusPipeline"/>).
    /// </summary>
    /// <remarks>
    ///     See <see cref="IBus" /> for more details.
    /// </remarks>
    /// <threadsafety static="true" instance="true" />
    [Export]
    public interface IBusController : ISynchronizable
    {
        /// <summary>
        ///     Raises the <see cref="IBus.ConnectionBroken" /> event.
        /// </summary>
        /// <param name="connection"> The connection. </param>
        /// <remarks>
        ///     <note type="note">
        ///         This method is not intended to be used by the message bus user.
        ///         It is merely provided to allow <see cref="IBusPipeline" /> implementations to raise the corresponding event.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="connection" /> is null. </exception>
        void RaiseConnectionBroken (IBusConnection connection);

        /// <summary>
        ///     Raises the <see cref="IBus.ProcessingException" /> event.
        /// </summary>
        /// <param name="message"> The message. </param>
        /// <param name="result"> The result to respond to the sender. </param>
        /// <param name="exception"> Specifies and returns the exception. </param>
        /// <param name="forwardException"> Specifies and returns whether the exception should be forwarded to the sender. </param>
        /// <returns>
        ///     The result to respond to the sender.
        /// </returns>
        /// <remarks>
        ///     <note type="note">
        ///         This method is not intended to be used by the message bus user.
        ///         It is merely provided to allow <see cref="IBusPipeline" /> implementations to raise the corresponding event.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="message" /> or <paramref name="exception" /> is null. </exception>
        /// TODO: BEFORE RELEASE
        /// TODO: Clarify dependency of return value and forwardException
        /// TODO: Change signature to: bool ReceiverExceptionHandler (ReceiverRegistration receiverRegistration, string address, object payload, ref Exception exception, ref object response)
        object RaiseProcessingException (MessageItem message, object result, ref Exception exception, ref bool forwardException);

        /// <summary>
        ///     Raises the <see cref="IBus.ReceivingRequest" /> event.
        /// </summary>
        /// <param name="message"> The message. </param>
        /// <remarks>
        ///     <note type="note">
        ///         This method is not intended to be used by the message bus user.
        ///         It is merely provided to allow <see cref="IBusPipeline" /> implementations to raise the corresponding event.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="message" /> is null. </exception>
        void RaiseReceivingRequest (MessageItem message);

        /// <summary>
        ///     Raises the <see cref="IBus.ReceivingResponse" /> event.
        /// </summary>
        /// <param name="message"> The message. </param>
        /// <remarks>
        ///     <note type="note">
        ///         This method is not intended to be used by the message bus user.
        ///         It is merely provided to allow <see cref="IBusPipeline" /> implementations to raise the corresponding event.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="message" /> is null. </exception>
        void RaiseReceivingResponse (MessageItem message);

        /// <summary>
        ///     Raises the <see cref="IBus.SendingRequest" /> event.
        /// </summary>
        /// <param name="message"> The message. </param>
        /// <remarks>
        ///     <note type="note">
        ///         This method is not intended to be used by the message bus user.
        ///         It is merely provided to allow <see cref="IBusPipeline" /> implementations to raise the corresponding event.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="message" /> is null. </exception>
        void RaiseSendingRequest (MessageItem message);

        /// <summary>
        ///     Raises the <see cref="IBus.SendingResponse" /> event.
        /// </summary>
        /// <param name="message"> The message. </param>
        /// <remarks>
        ///     <note type="note">
        ///         This method is not intended to be used by the message bus user.
        ///         It is merely provided to allow <see cref="IBusPipeline" /> implementations to raise the corresponding event.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="message" /> is null. </exception>
        void RaiseSendingResponse (MessageItem message);

        /// <summary>
        ///     Signals that new work is pending (e.g. a message has been received through a bus connection) which must be processed by the bus processing pipeline.
        /// </summary>
        /// <remarks>
        ///     <note type="note">
        ///         This method is not intended to be used by the message bus user.
        ///         It is merely provided to allow <see cref="IBusPipeline" /> implementations to raise the corresponding event.
        ///     </note>
        /// </remarks>
        /// <exception cref="InvalidOperationException"> The bus is not started. </exception>
        void SignalWorkAvailable ();

        /// <summary>
        ///     Gets the list of registered receivers managed by the bus.
        /// </summary>
        /// <value>
        ///     The list of registered receivers.
        /// </value>
        /// <remarks>
        ///     <note type="note">
        ///         Do not use this property directly, it is intended to be used from bus and bus pipeline implementations.
        ///         Use <see cref="IBus.Register" /> and <see cref="IBus.Unregister" /> for dealing with receiver registrations.
        ///     </note>
        ///     <note type="important">
        ///         The returned list is not a copy and access to it needs to be synchronized using <see cref="ISynchronizable.SyncRoot" />.
        ///     </note>
        ///     <note type="implement">
        ///         This property must never return null.
        ///     </note>
        /// </remarks>
        List<ReceiverRegistrationItem> ReceiveRegistrations { get; }

        /// <summary>
        ///     Gets the list of enqueued send operations managed by the bus.
        /// </summary>
        /// <value>
        ///     The list of enqueued send operations.
        /// </value>
        /// <remarks>
        ///     <note type="note">
        ///         Do not use this property directly, it is intended to be used from bus and bus pipeline implementations.
        ///         Use <see cref="IBus.Enqueue" /> for dealing with send operations.
        ///     </note>
        ///     <note type="important">
        ///         The returned list is not a copy and access to it needs to be synchronized using <see cref="ISynchronizable.SyncRoot" />.
        ///     </note>
        ///     <note type="implement">
        ///         This property must never return null.
        ///     </note>
        /// </remarks>
        List<SendOperationItem> SendOperations { get; }
    }
}