using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;

using RI.Framework.Composition.Model;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Services.Messaging.Dispatchers
{
    /// <summary>
    ///     Defines the interface for a message dispatcher.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         A message dispatcher is used by a <see cref="IMessageService" /> to actually enqueue and deliver the messages to the receivers.
    ///     </para>
    ///     <note type="important">
    ///         A message dispatcher is not required to capture <see cref="ExecutionContext" />, <see cref="SynchronizationContext"/>, or <see cref="CultureInfo" />.
    ///         The thread to which the state machine operation is dispatched can define the used execution context, synchronization context, and thread culture.
    ///         Therefore, the actual behaviour depends on a <see cref="IMessageDispatcher" />s implementation.
    ///     </note>
    ///     <note type="important">
    ///         The priority a message is dispatched with, if applicable, depends on an <see cref="IMessageDispatcher" />s implementation.
    ///     </note>
    /// </remarks>
    /// <threadsafety static="true" instance="true" />
    [Export]
    [Obsolete(MessageService.ObsoleteMessage, false)]
    public interface IMessageDispatcher : ISynchronizable
    {
        /// <summary>
        ///     Asynchronously delivers a message.
        /// </summary>
        /// <param name="receivers"> The sequence of receivers. </param>
        /// <param name="message"> The message to deliver. </param>
        /// <param name="messageService"> The message service used to deliver the message. </param>
        /// <param name="deliveredCallback"> The callback to be called by the dispatcher when all receivers have received the message. Can be null if no callback is required. </param>
        /// <remarks>
        ///     <note type="implement">
        ///         This method can return before the message is delivered to all receivers.
        ///     </note>
        ///     <note type="note">
        ///         Do not call this method directly, it is intended to be called from an <see cref="IMessageService" /> implementation.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="receivers" />, <paramref name="message" />, or <paramref name="messageService" /> is null. </exception>
        void Post (List<IMessageReceiver> receivers, Message message, IMessageService messageService, Action<Message> deliveredCallback);
    }
}
