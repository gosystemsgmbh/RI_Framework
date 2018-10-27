using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using RI.Framework.Collections;
using RI.Framework.Collections.DirectLinq;
using RI.Framework.Composition;
using RI.Framework.Composition.Model;
using RI.Framework.Services.Messaging.Dispatchers;
using RI.Framework.Utilities.Logging;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Services.Messaging
{
    /// <summary>
    ///     Implements a default messaging service which is suitable for most scenarios.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This message service manages <see cref="IMessageDispatcher" />s  and <see cref="IMessageReceiver" />s from two sources.
    ///         One are the explicitly specified dispatchers and receivers added through <see cref="AddDispatcher" /> and <see cref="AddReceiver" />.
    ///         The second is a <see cref="CompositionContainer" /> if this <see cref="MessageService" /> is added as an export (the dispatchers and receivers are then imported through composition).
    ///         <see cref="Dispatchers" /> gives the sequence containing all message dispatchers from all sources and <see cref="Receivers" /> gives the sequence containing all message receivers from all sources.
    ///     </para>
    ///     <para>
    ///         See <see cref="IMessageService" /> for more details.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="true" instance="true" />
    [Export]
    [Obsolete(MessageService.ObsoleteMessage, false)]
    public sealed class MessageService : LogSource, IMessageService, IImporting
    {
        /// <summary>
        /// The message used for the <see cref="ObsoleteAttribute"/>.
        /// </summary>
        public const string ObsoleteMessage = "The message service is obsolete. Use the message bus instead (RI.Framework.Bus.*).";


        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="MessageService" />.
        /// </summary>
        public MessageService ()
        {
            this.SyncRoot = new object();

            this.DispatchersUpdated = new List<IMessageDispatcher>();
            this.ReceiversUpdated = new List<IMessageReceiver>();

            this.DispatchersManual = new List<IMessageDispatcher>();
            this.ReceiversManual = new List<IMessageReceiver>();

            this.ReceiverCopy = new List<IMessageReceiver>();
        }

        #endregion




        #region Instance Properties/Indexer

        [Import(typeof(IMessageDispatcher), Recomposable = true)]
        private Import DispatchersImported { get; set; }

        private List<IMessageDispatcher> DispatchersManual { get; }

        private List<IMessageDispatcher> DispatchersUpdated { get; set; }

        private List<IMessageReceiver> ReceiverCopy { get; set; }

        [Import(typeof(IMessageReceiver), Recomposable = true)]
        private Import ReceiversImported { get; set; }

        private List<IMessageReceiver> ReceiversManual { get; }

        private List<IMessageReceiver> ReceiversUpdated { get; set; }

        #endregion




        #region Instance Methods

        private void UpdateDispatchers ()
        {
            this.Log(LogLevel.Debug, "Updating dispatchers");

            HashSet<IMessageDispatcher> currentDispatchers = new HashSet<IMessageDispatcher>(this.Dispatchers);
            HashSet<IMessageDispatcher> lastDispatchers = new HashSet<IMessageDispatcher>(this.DispatchersUpdated);

            HashSet<IMessageDispatcher> newDispatchers = currentDispatchers.Except(lastDispatchers);
            HashSet<IMessageDispatcher> oldDispatchers = lastDispatchers.Except(currentDispatchers);

            this.DispatchersUpdated.Clear();
            this.DispatchersUpdated.AddRange(currentDispatchers);

            foreach (IMessageDispatcher dispatcher in newDispatchers)
            {
                this.Log(LogLevel.Debug, "Dispatcher added: {0}", dispatcher.GetType().Name);
            }

            foreach (IMessageDispatcher dispatcher in oldDispatchers)
            {
                this.Log(LogLevel.Debug, "Dispatcher removed: {0}", dispatcher.GetType().Name);
            }
        }

        private void UpdateReceivers ()
        {
            this.Log(LogLevel.Debug, "Updating receivers");

            HashSet<IMessageReceiver> currentReceivers = new HashSet<IMessageReceiver>(this.Receivers);
            HashSet<IMessageReceiver> lastReceivers = new HashSet<IMessageReceiver>(this.ReceiversUpdated);

            HashSet<IMessageReceiver> newReceivers = currentReceivers.Except(lastReceivers);
            HashSet<IMessageReceiver> oldReceivers = lastReceivers.Except(currentReceivers);

            this.ReceiversUpdated.Clear();
            this.ReceiversUpdated.AddRange(currentReceivers);

            foreach (IMessageReceiver receiver in newReceivers)
            {
                this.Log(LogLevel.Debug, "Receiver added: {0}", receiver.GetType().Name);
            }

            foreach (IMessageReceiver receiver in oldReceivers)
            {
                this.Log(LogLevel.Debug, "Receiver removed: {0}", receiver.GetType().Name);
            }

            this.ReceiverCopy = new List<IMessageReceiver>(this.ReceiversUpdated);
        }

        #endregion




        #region Interface: IImporting

        /// <inheritdoc />
        void IImporting.ImportsResolved (CompositionFlags composition, bool updated)
        {
            if (updated)
            {
                lock (this.SyncRoot)
                {
                    this.UpdateDispatchers();
                    this.UpdateReceivers();
                }
            }
        }

        /// <inheritdoc />
        void IImporting.ImportsResolving (CompositionFlags composition)
        {
        }

        #endregion




        #region Interface: IMessageService

        /// <inheritdoc />
        public IEnumerable<IMessageDispatcher> Dispatchers
        {
            get
            {
                lock (this.SyncRoot)
                {
                    List<IMessageDispatcher> dispatchers = new List<IMessageDispatcher>();
                    dispatchers.AddRange(this.DispatchersManual);
                    dispatchers.AddRange(this.DispatchersImported.Values<IMessageDispatcher>());
                    return dispatchers;
                }
            }
        }

        /// <inheritdoc />
        bool ISynchronizable.IsSynchronized => true;

        /// <inheritdoc />
        public IEnumerable<IMessageReceiver> Receivers
        {
            get
            {
                lock (this.SyncRoot)
                {
                    List<IMessageReceiver> receivers = new List<IMessageReceiver>();
                    receivers.AddRange(this.ReceiversManual);
                    receivers.AddRange(this.ReceiversImported.Values<IMessageReceiver>());
                    return receivers;
                }
            }
        }

        /// <inheritdoc />
        public object SyncRoot { get; }

        /// <inheritdoc />
        public void AddDispatcher (IMessageDispatcher messageDispatcher)
        {
            if (messageDispatcher == null)
            {
                throw new ArgumentNullException(nameof(messageDispatcher));
            }

            lock (this.SyncRoot)
            {
                if (this.DispatchersManual.Contains(messageDispatcher))
                {
                    return;
                }

                this.DispatchersManual.Add(messageDispatcher);

                this.UpdateDispatchers();
            }
        }

        /// <inheritdoc />
        public void AddReceiver (IMessageReceiver messageReceiver)
        {
            if (messageReceiver == null)
            {
                throw new ArgumentNullException(nameof(messageReceiver));
            }

            lock (this.SyncRoot)
            {
                if (this.ReceiversManual.Contains(messageReceiver))
                {
                    return;
                }

                this.ReceiversManual.Add(messageReceiver);

                this.UpdateReceivers();
            }
        }

        /// <inheritdoc />
        public void Post (Message message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            lock (this.SyncRoot)
            {
                foreach (IMessageDispatcher dispatcher in this.DispatchersUpdated)
                {
                    dispatcher.Post(this.ReceiverCopy, message, this, null);
                }
            }
        }

        /// <inheritdoc />
        public void RemoveDispatcher (IMessageDispatcher messageDispatcher)
        {
            if (messageDispatcher == null)
            {
                throw new ArgumentNullException(nameof(messageDispatcher));
            }

            lock (this.SyncRoot)
            {
                if (!this.DispatchersManual.Contains(messageDispatcher))
                {
                    return;
                }

                this.DispatchersManual.RemoveAll(messageDispatcher);

                this.UpdateDispatchers();
            }
        }

        /// <inheritdoc />
        public void RemoveReceiver (IMessageReceiver messageReceiver)
        {
            if (messageReceiver == null)
            {
                throw new ArgumentNullException(nameof(messageReceiver));
            }

            lock (this.SyncRoot)
            {
                if (!this.ReceiversManual.Contains(messageReceiver))
                {
                    return;
                }

                this.ReceiversManual.RemoveAll(messageReceiver);

                this.UpdateReceivers();
            }
        }

        /// <inheritdoc />
        public Task Send (Message message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            TaskCompletionSource<Message> tcs = new TaskCompletionSource<Message>(TaskCreationOptions.RunContinuationsAsynchronously);

            lock (this.SyncRoot)
            {
                foreach (IMessageDispatcher dispatcher in this.DispatchersUpdated)
                {
                    dispatcher.Post(this.ReceiverCopy, message, this, x => tcs.SetResult(x));
                }
            }

            return tcs.Task;
        }

        #endregion
    }
}
