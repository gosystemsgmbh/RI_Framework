using System;
using System.Collections.Generic;

using RI.Framework.Services.Logging;

namespace RI.Framework.Bus
{
	internal sealed class BusNode : IBusNode, ILogSource
	{
		public BusNode (Guid id, IEnumerable<IBusMessageDispatcher> dispatchers, IEnumerable<IBusConnection> connections)
		{
			if (dispatchers == null)
			{
				throw new ArgumentNullException(nameof(dispatchers));
			}

			if (connections == null)
			{
				throw new ArgumentNullException(nameof(connections));
			}

			this.Id = id;

			this.Dispatchers = new List<IBusMessageDispatcher>(dispatchers);
			this.Connections = new List<IBusConnection>(connections);

			this.LocalSubscriptions = new Dictionary<Type, HashSet<IBusEndpoint>>();
		}

		public Guid Id { get; private set; }

		private List<IBusMessageDispatcher> Dispatchers { get; set; }

		private List<IBusConnection> Connections { get; set; }

		private Dictionary<Type, HashSet<IBusEndpoint>> LocalSubscriptions { get; set; }

		public void Start ()
		{
			this.LocalSubscriptions.Clear();

			foreach (IBusConnection connection in this.Connections)
			{
				connection.Connect();
			}
		}

		public void Stop ()
		{
			foreach (IBusConnection connection in this.Connections)
			{
				connection.Connect();
			}

			this.LocalSubscriptions.Clear();
		}

		public void Subscribe (IBusEndpoint busEndpoint, Type messageType)
		{
			if (!this.LocalSubscriptions.ContainsKey(messageType))
			{
				this.LocalSubscriptions.Add(messageType, new HashSet<IBusEndpoint>());
			}

			this.LocalSubscriptions[messageType].Add(busEndpoint);

			foreach (IBusConnection connection in this.Connections)
			{
				connection.AddSubscription(this, busEndpoint, messageType);
			}
		}

		public void Unsubscribe (IBusEndpoint busEndpoint, Type messageType)
		{
			foreach (IBusConnection connection in this.Connections)
			{
				connection.RemoveSubscription(this, busEndpoint, messageType);
			}

			if (!this.LocalSubscriptions.ContainsKey(messageType))
			{
				this.LocalSubscriptions.Add(messageType, new HashSet<IBusEndpoint>());
			}

			this.LocalSubscriptions[messageType].Remove(busEndpoint);
		}

		public IBusMessageTransmission Post (IBusEndpoint busEndpoint, Type messageType, object message)
		{
			throw new NotImplementedException();
		}
	}
}
