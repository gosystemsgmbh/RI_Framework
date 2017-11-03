using System;

using RI.Framework.Bus.Connections;
using RI.Framework.Bus.Internals;
using RI.Framework.Composition.Model;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Bus.Routers
{
	/// <summary>
	///     Defines the interface for a bus router.
	/// </summary>
	/// <remarks>
	///     <para>
	///         See <see cref="IBus" /> for more details about message busses.
	///     </para>
	///     <para>
	///         This interface is part of the actual bus implementation and not intended to be used by the bus users.
	///     </para>
	/// </remarks>
	/// <threadsafety static="true" instance="true" />
	[Export]
	public interface IBusRouter : ISynchronizable
	{
		/// <summary>
		///     Determines whether a message is to be forwarded to remote busses through connections.
		/// </summary>
		/// <param name="message"> The message. </param>
		/// <returns>
		///     true if the message is to be forwarded to remote busses through connections, false otherwise.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="message" /> is null. </exception>
		bool ForwardToGlobal (MessageItem message);

		/// <summary>
		///     Determines whether a message is to be forwarded to local registered receivers.
		/// </summary>
		/// <param name="message"> The message. </param>
		/// <returns>
		///     true if the message is to be forwarded to local registered receivers, false otherwise.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="message" /> is null. </exception>
		bool ForwardToLocal (MessageItem message);

		/// <summary>
		///     Initializes the router when the bus starts.
		/// </summary>
		/// <param name="dependencyResolver"> The dependency resolver which can be used to get instances of required types. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="dependencyResolver" /> is null. </exception>
		void Initialize (IDependencyResolver dependencyResolver);

		/// <summary>
		///     Makes the router aware of a message which was received from a local bus.
		/// </summary>
		/// <param name="message"> The received message. </param>
		void ReceivedFromLocal (MessageItem message);

		/// <summary>
		///     Makes the router aware of a message which was received from a remote bus through a particular connection.
		/// </summary>
		/// <param name="message"> The received message. </param>
		/// <param name="connection"> The connection through the message was received. </param>
		void ReceivedFromRemote (MessageItem message, IBusConnection connection);

		/// <summary>
		///     Tests whether a receiver should receive a message.
		/// </summary>
		/// <param name="message"> The message. </param>
		/// <param name="receiver"> The receiver registration to test. </param>
		/// <returns>
		///     true if the specified receiver should receive the message, false otherwise.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="message" /> or <paramref name="receiver" /> is null. </exception>
		bool ShouldReceive (MessageItem message, ReceiverRegistrationItem receiver);

		/// <summary>
		///     Tests whether a connection should be used to send a message to a remote bus.
		/// </summary>
		/// <param name="message"> The message. </param>
		/// <param name="connection"> The bus connection to test. </param>
		/// <returns>
		///     true if the specified connection should be used to send the message, false otherwise.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="message" /> or <paramref name="connection" /> is null. </exception>
		bool ShouldSend (MessageItem message, IBusConnection connection);

		/// <summary>
		///     Unloads the router when the bus stops.
		/// </summary>
		void Unload ();
	}
}
