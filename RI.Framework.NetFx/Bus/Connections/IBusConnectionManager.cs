using System;
using System.Collections.Generic;

using RI.Framework.Bus.Internals;
using RI.Framework.Utilities.ObjectModel;

namespace RI.Framework.Bus.Connections
{
	/// <summary>
	/// Defines the interface for a bus connection manager.
	/// </summary>
	/// <remarks>
	/// See <see cref="IBus"/> for more details about message busses.
	/// </remarks>
	public interface IBusConnectionManager : ISynchronizable
	{
		/// <summary>
		/// Initializes the connection manager when the bus starts.
		/// </summary>
		/// <param name="dependencyResolver">The dependency resolver which can be used to get instances of required types.</param>
		/// <exception cref="ArgumentNullException"><paramref name="dependencyResolver"/> is null.</exception>
		void Initialize(IDependencyResolver dependencyResolver);

		/// <summary>
		/// Unloads the connection manager when the bus stops.
		/// </summary>
		void Unload();

		/// <summary>
		/// Gets the list of managed connections.
		/// </summary>
		/// <value>
		/// The list of managed connections.
		/// </value>
		IReadOnlyList<IBusConnection> Connections { get; }

		/// <summary>
		/// Dequeues all messages which have been received from all managed connections since the last call to <see cref="DequeueMessages"/>.
		/// </summary>
		/// <param name="messages">The to which all dequeued messages are added.</param>
		/// <exception cref="ArgumentNullException"><paramref name="messages"/> is null.</exception>
		void DequeueMessages (List<Tuple<MessageItem, IBusConnection>> messages);

		/// <summary>
		/// Sends a message through the specified connections.
		/// </summary>
		/// <param name="message">The message to send.</param>
		/// <param name="connections">The sequence of connections through which the message is sent.</param>
		/// <remarks>
		/// <para>
		/// <paramref name="connections"/> is enumerated only once.
		/// </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"><paramref name="message"/> or <paramref name="connections"/> is null.</exception>
		void SendMessage (MessageItem message, IEnumerable<IBusConnection> connections);

		/// <summary>
		/// Sends a message through the specified connection.
		/// </summary>
		/// <param name="message">The message to send.</param>
		/// <param name="connection">The connection through which the message is sent.</param>
		/// <exception cref="ArgumentNullException"><paramref name="message"/> or <paramref name="connection"/> is null.</exception>
		void SendMessage(MessageItem message, IBusConnection connection);

		/// <summary>
		/// Sends a message through all managed connections
		/// </summary>
		/// <param name="message">The message to send.</param>
		/// <exception cref="ArgumentNullException"><paramref name="message"/> is null.</exception>
		void SendMessage (MessageItem message);
	}
}
