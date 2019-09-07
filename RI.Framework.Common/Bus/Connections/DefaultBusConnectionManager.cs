using System;
using System.Collections.Generic;

using RI.Framework.Bus.Internals;
using RI.Framework.ComponentModel;
using RI.Framework.Composition.Model;
using RI.Framework.Utilities.Logging;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Bus.Connections
{
    /// <summary>
    ///     Implements a default bus connection manager which is suitable for most scenarios.
    /// </summary>
    /// <remarks>
    ///     See <see cref="IBusConnectionManager" /> for more details.
    /// </remarks>
    /// <threadsafety static="true" instance="true" />
    [Export]
    public sealed class DefaultBusConnectionManager : LogSource, IBusConnectionManager
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="DefaultBusConnectionManager" />.
        /// </summary>
        public DefaultBusConnectionManager ()
        {
            this.SyncRoot = new object();

            this.Connections = new List<IBusConnection>();
        }

        #endregion




        #region Instance Properties/Indexer

        private List<IBusConnection> Connections { get; }

        #endregion




        #region Interface: IBusConnectionManager

        /// <inheritdoc />
        IReadOnlyList<IBusConnection> IBusConnectionManager.Connections
        {
            get
            {
                lock (this.SyncRoot)
                {
                    return this.Connections;
                }
            }
        }

        /// <inheritdoc />
        bool ISynchronizable.IsSynchronized => true;

        /// <inheritdoc />
        public object SyncRoot { get; }

        /// <inheritdoc />
        public void DequeueMessages (List<Tuple<MessageItem, IBusConnection>> messages)
        {
            lock (this.SyncRoot)
            {
                List<MessageItem> connectionMessages = new List<MessageItem>();
                foreach (IBusConnection connection in this.Connections)
                {
                    connectionMessages.Clear();
                    connection.DequeueMessages(connectionMessages);
                    foreach (MessageItem message in connectionMessages)
                    {
                        this.Log(LogLevel.Debug, "Receiving: Connection=[{0}], Message=[{1}]", connection, message);
                        messages.Add(new Tuple<MessageItem, IBusConnection>(message, connection));
                    }
                }
            }
        }

        /// <inheritdoc />
        public void Initialize (IDependencyResolver dependencyResolver)
        {
            lock (this.SyncRoot)
            {
                this.Log(LogLevel.Debug, "Initializing");

                this.Connections.Clear();
                this.Connections.AddRange(dependencyResolver.GetInstances<IBusConnection>());

                this.Connections.ForEach(x =>
                {
                    this.Log(LogLevel.Debug, "Initializing Connection: {0}", x);
                    x.Initialize(dependencyResolver);
                });
            }
        }

        /// <inheritdoc />
        public void SendMessage (MessageItem message, IBusConnection connection)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            this.Log(LogLevel.Debug, "Sending: Connection=[{0}], Message=[{1}]", connection, message);

            connection.SendMessage(message);
        }

        /// <inheritdoc />
        public void SendMessage (MessageItem message, IEnumerable<IBusConnection> connections)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (connections == null)
            {
                throw new ArgumentNullException(nameof(connections));
            }

            foreach (IBusConnection connection in connections)
            {
                this.SendMessage(message, connection);
            }
        }

        /// <inheritdoc />
        public void SendMessage (MessageItem message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            lock (this.SyncRoot)
            {
                foreach (IBusConnection connection in this.Connections)
                {
                    this.SendMessage(message, connection);
                }
            }
        }

        /// <inheritdoc />
        public void Unload ()
        {
            lock (this.SyncRoot)
            {
                this.Log(LogLevel.Debug, "Unloading");

                this.Connections?.Clear();
            }
        }

        #endregion
    }
}
