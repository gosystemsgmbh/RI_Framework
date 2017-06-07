using System;
using System.Collections.Generic;

using RI.Framework.Composition.Model;

namespace RI.Framework.Bus
{
	[Export]
	public interface IBusService
	{
		/// <summary>
		/// Gets the local node of the bus which can be used for sending and receiving.
		/// </summary>
		/// <value>
		/// The local node of the bus which can be used for sending and receiving.
		/// </value>
		IBusNode LocalNode { get; }

		/// <summary>
		///     Gets all currently available message dispatchers.
		/// </summary>
		/// <value>
		///     All currently available message dispatchers.
		/// </value>
		/// <remarks>
		///     <note type="implement">
		///         The value of this property must never be null.
		///     </note>
		/// </remarks>
		IEnumerable<IBusMessageDispatcher> Dispatchers { get; }

		/// <summary>
		///     Gets all currently available bus connections.
		/// </summary>
		/// <value>
		///     All currently available bus connections.
		/// </value>
		/// <remarks>
		///     <note type="implement">
		///         The value of this property must never be null.
		///     </note>
		/// </remarks>
		IEnumerable<IBusConnection> Connections { get; }

		/// <summary>
		///     Adds a message dispatcher.
		/// </summary>
		/// <param name="messageDispatcher"> The message dispatcher to add. </param>
		/// <remarks>
		///     <note type="implement">
		///         Specifying an already added message dispatcher should have no effect.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="messageDispatcher" /> is null. </exception>
		void AddDispatcher(IBusMessageDispatcher messageDispatcher);

		/// <summary>
		///     Adds a bus connection.
		/// </summary>
		/// <param name="busConnection"> The bus connection to add. </param>
		/// <remarks>
		///     <note type="implement">
		///         Specifying an already added bus connection should have no effect.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="busConnection" /> is null. </exception>
		void AddConnection(IBusConnection busConnection);

		/// <summary>
		///     Starts the service and connects to the bus using the used bus connections.
		/// </summary>
		/// <param name="localNodeId">The ID of the local node.</param>
		/// <remarks>
		/// <note type="implement">
		/// Changes to the available dispatchers and connections must not have an effect after the bus was started.
		/// Starting the bus must take a snapshot of the available dispatchers (<see cref="Dispatchers"/>) and connections (<see cref="Connections"/>).
		/// </note>
		/// </remarks>
		/// <exception cref="InvalidOperationException"> The bus is already started. </exception>
		void Start (Guid localNodeId);

		/// <summary>
		///     Stops the service and disconnects to the bus.
		/// </summary>
		void Stop ();

		/// <summary>
		///     Removes a message dispatcher.
		/// </summary>
		/// <param name="messageDispatcher"> The message dispatcher to remove. </param>
		/// <remarks>
		///     <note type="implement">
		///         Specifying an already removed message dispatcher should have no effect.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="messageDispatcher" /> is null. </exception>
		void RemoveDispatcher(IBusMessageDispatcher messageDispatcher);

		/// <summary>
		///     Removes a bus connection.
		/// </summary>
		/// <param name="busConnection"> The bus connection to remove. </param>
		/// <remarks>
		///     <note type="implement">
		///         Specifying an already removed bus connection should have no effect.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="busConnection" /> is null. </exception>
		void RemoveConnection(IBusConnection busConnection);
	}
}
