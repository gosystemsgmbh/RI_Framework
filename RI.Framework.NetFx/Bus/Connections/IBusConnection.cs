using System;
using System.Collections.Generic;

using RI.Framework.Bus.Internals;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Bus.Connections
{
	/// <summary>
	///     Defines the interface for a bus connection.
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
	public interface IBusConnection : ISynchronizable
	{
		/// <summary>
		///     Gets an explanatory message what and why the connection is broken.
		/// </summary>
		/// <value>
		///     An explanatory message what and why the connection is broken or null if the connection is not broken.
		/// </value>
		string BrokenMessage { get; }

		/// <summary>
		///     Gets whether this connection is broken.
		/// </summary>
		/// <value>
		///     true if this connection is broken, false otherwise.
		/// </value>
		bool IsBroken { get; }

		/// <summary>
		///     Dequeues all messages which have been received by this connection.
		/// </summary>
		/// <param name="messages"> The list to which all dequeued messages are added. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="messages" /> is null. </exception>
		void DequeueMessages (List<MessageItem> messages);

		/// <summary>
		///     Initializes the connection when the bus starts.
		/// </summary>
		/// <param name="dependencyResolver"> The dependency resolver which can be used to get instances of required types. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="dependencyResolver" /> is null. </exception>
		void Initialize (IDependencyResolver dependencyResolver);

		/// <summary>
		///     Sends a message through this connection.
		/// </summary>
		/// <param name="message"> The message to send. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="message" /> is null. </exception>
		void SendMessage (MessageItem message);

		/// <summary>
		///     Unloads the connection when the bus stops.
		/// </summary>
		void Unload ();
	}
}
