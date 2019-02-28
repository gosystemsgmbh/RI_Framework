using System;
using System.Collections.Generic;
using System.Threading;

using RI.Framework.Composition.Model;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Services.Messaging.Dispatchers
{
    /// <summary>
    ///     Implements a message dispatcher which uses <see cref="SynchronizationContext" />.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Messages are dispatched using <see cref="System.Threading.SynchronizationContext.Post(SendOrPostCallback,object)" />.
    ///     </para>
    ///     <para>
    ///         See <see cref="IMessageDispatcher" /> for more details.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="true" instance="true" />
    [Export]
    [Obsolete(MessageService.ObsoleteMessage, false)]
    public sealed class SynchronizationContextMessageDispatcher : IMessageDispatcher
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="SynchronizationContextMessageDispatcher" />.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         The synchronization context which is the current context when this instance is created will be used, using <see cref="System.Threading.SynchronizationContext.Current" />.
        ///     </para>
        /// </remarks>
        /// <exception cref="InvalidOperationException"> There is no current synchronization context available. </exception>
        public SynchronizationContextMessageDispatcher ()
        {
            SynchronizationContext context = SynchronizationContext.Current;
            if (context == null)
            {
                throw new InvalidOperationException("No current synchronization context available.");
            }

            this.SyncRoot = new object();
            this.SynchronizationContext = context;
        }

        /// <summary>
        ///     Creates a new instance of <see cref="SynchronizationContextMessageDispatcher" />.
        /// </summary>
        /// <param name="synchronizationContext"> The used synchronization context. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="synchronizationContext" /> is null. </exception>
        public SynchronizationContextMessageDispatcher (SynchronizationContext synchronizationContext)
        {
            if (synchronizationContext == null)
            {
                throw new ArgumentNullException(nameof(synchronizationContext));
            }

            this.SyncRoot = new object();
            this.SynchronizationContext = synchronizationContext;
        }

        #endregion




        #region Instance Properties/Indexer

        /// <summary>
        ///     Gets the used synchronization context.
        /// </summary>
        /// <value>
        ///     The used synchronization context.
        /// </value>
        public SynchronizationContext SynchronizationContext { get; }

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
                this.SynchronizationContext.Post(_ =>
                {
                    foreach (IMessageReceiver receiver in receivers)
                    {
                        receiver.ReceiveMessage(message, messageService);
                    }
                    deliveredCallback?.Invoke(message);
                }, null);
            }
        }

        #endregion
    }
}
