using System;
using System.Collections.Generic;

using RI.Framework.Bus.Internals;
using RI.Framework.Bus.Pipeline;
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
		public bool IsSynchronized { get; }

		public object SyncRoot { get; }

		public IReadOnlyList<IBusConnection> Connections { get; }

		public void DequeueMessages (List<Tuple<MessageItem, IBusConnection>> messages)
		{
			throw new NotImplementedException();
		}

		public void Initialize (IDependencyResolver dependencyResolver)
		{
			throw new NotImplementedException();
		}

		public void SendMessage (MessageItem message, IEnumerable<IBusConnection> connections)
		{
			throw new NotImplementedException();
		}

		public void SendMessage (MessageItem message, IBusConnection connection)
		{
			throw new NotImplementedException();
		}

		public void SendMessage (MessageItem message)
		{
			throw new NotImplementedException();
		}

		public void Unload ()
		{
			throw new NotImplementedException();
		}
	}
}
