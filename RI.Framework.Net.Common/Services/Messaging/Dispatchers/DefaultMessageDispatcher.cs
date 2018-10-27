using System;
using System.Collections.Generic;

using RI.Framework.Composition.Model;
using RI.Framework.Threading;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Services.Messaging.Dispatchers
{
    /// <summary>
    ///     Implements a message dispatcher which uses <see cref="DispatchCapture" />.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         See <see cref="IMessageDispatcher" /> for more details.
    ///     </para>
    ///     <para>
    ///         Messages are dispatched using <see cref="DispatchCapture.Execute" />.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="true" instance="true" />
    [Export]
    [Obsolete(MessageService.ObsoleteMessage, false)]
    public sealed class DefaultMessageDispatcher : IMessageDispatcher
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="DefaultMessageDispatcher" />.
        /// </summary>
        public DefaultMessageDispatcher ()
        {
            this.SyncRoot = new object();
        }

        #endregion




        #region Interface: IMessageDispatcher

        /// <inheritdoc />
        bool ISynchronizable.IsSynchronized => true;

        /// <inheritdoc />
        public object SyncRoot { get; }

        /// <inheritdoc />
        public void Post (List<IMessageReceiver> receivers, Message message, IMessageService messageService, Action<Message> deliveredCallback)
        {
            if (receivers == null)
            {
                throw new ArgumentNullException(nameof(receivers));
            }

            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            lock (this.SyncRoot)
            {
                DispatchCapture capture = new DispatchCapture(new Action<List<IMessageReceiver>, Message, IMessageService, Action<Message>>((r, m, s, c) =>
                {
                    foreach (IMessageReceiver receiver in r)
                    {
                        receiver.ReceiveMessage(m, s);
                    }
                    c?.Invoke(m);
                }), receivers, message, messageService, deliveredCallback);

                capture.Execute();
            }
        }

        #endregion
    }
}
