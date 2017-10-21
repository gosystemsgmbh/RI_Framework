using System;
using System.Collections.Generic;

using RI.Framework.Bus.Internals;
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
	public sealed class DefaultBusConnectionManager : IBusConnectionManager
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
				this.Connections.Clear();
				this.Connections.AddRange(dependencyResolver.GetInstances<IBusConnection>());
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
				this.Connections?.Clear();
			}
		}

		#endregion
	}
}
