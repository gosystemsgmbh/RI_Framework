using System;
using System.Collections.Generic;
using System.Threading;

using RI.Framework.Composition.Model;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Services.Messaging.Dispatchers
{
    /// <summary>
    ///     Implements a message dispatcher which uses <see cref="ThreadPool" />.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Messages are dispatched using <see cref="ThreadPool.QueueUserWorkItem(WaitCallback)" />.
    ///     </para>
    ///     <para>
    ///         See <see cref="IMessageDispatcher" /> for more details.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="true" instance="true" />
    [Export]
    [Obsolete(MessageService.ObsoleteMessage, false)]
    public sealed class ThreadPoolMessageDispatcher : IMessageDispatcher
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="ThreadPoolMessageDispatcher" />.
        /// </summary>
        public ThreadPoolMessageDispatcher ()
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
                ThreadPool.QueueUserWorkItem(_ =>
                {
                    foreach (IMessageReceiver receiver in receivers)
                    {
                        receiver.ReceiveMessage(message, messageService);
                    }
                    deliveredCallback?.Invoke(message);
                });
            }
        }

        #endregion
    }
}
