﻿using System;
using System.Collections.Generic;

using RI.Framework.Composition.Model;
using RI.Framework.Services.Messaging.Dispatchers;




namespace RI.Framework.Services.Messaging
{
	/// <summary>
	///     Defines the interface for a messaging service.
	/// </summary>
	/// <remarks>
	///     <para>
	///         A messaging service is used to send/receive application-specific messages asynchronously to/from modules of an application.
	///     </para>
	///     <para>
	///         Messages are instances of <see cref="IMessage" /> which are sent using a messaging service.
	///         The message service uses message dispatchers (<see cref="IMessageDispatcher" />) to deliver the messages asynchronously to all known message receivers (<see cref="IMessageReceiver" />).
	///         The message dispatcher is responsible for achieving asynchronity.
	///     </para>
	///     <para>
	///         Asynchronous delivery of messages means that sending a message is quick (xxx returns immediately) and the messages are delivered, and therefore handled by their receivers, at a later time but in the correct order as they were sent.
	///         The meaning of &quot;at a later time&quot; depends on the application context and the used message dispatcher.
	///         For example, in GUI applications, the message dispatcher can enqueue the messages into the applications message loop.
	///     </para>
	///     <para>
	///         A message service is explicitly made to support sending from different threads.
	///         The receiving however depends on the used message dispatchers.
	///     </para>
	/// <note type="implement">
	/// Note that messaging is usually to be used from any thread.
	/// Therefore, the messaging service must be partially thread-safe, at least for the actual send and receive operations.
	/// However, messaging service implementations can rely upon the thread-safety of <see cref="IMessageDispatcher"/>.
	/// </note>
	/// </remarks>
	[Export]
	public interface IMessageService
	{
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
		IEnumerable<IMessageDispatcher> Dispatchers { get; }

		/// <summary>
		///     Gets all currently available message receivers.
		/// </summary>
		/// <value>
		///     All currently available message receivers.
		/// </value>
		/// <remarks>
		///     <note type="implement">
		///         The value of this property must never be null.
		///     </note>
		/// </remarks>
		IEnumerable<IMessageReceiver> Receivers { get; }

		/// <summary>
		///     Adds a message dispatcher and starts using it for all subsequent messages.
		/// </summary>
		/// <param name="messageDispatcher"> The message dispatcher to add. </param>
		/// <remarks>
		///     <note type="implement">
		///         Specifying an already added message dispatcher should have no effect.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="messageDispatcher" /> is null. </exception>
		void AddDispatcher (IMessageDispatcher messageDispatcher);

		/// <summary>
		///     Adds a message receiver and starts using it for all subsequent messages.
		/// </summary>
		/// <param name="messageReceiver"> The message receiver to add. </param>
		/// <remarks>
		///     <note type="implement">
		///         Specifying an already added message receiver should have no effect.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="messageReceiver" /> is null. </exception>
		void AddReceiver (IMessageReceiver messageReceiver);

		/// <summary>
		///     Sends a message asynchronously.
		/// </summary>
		/// <param name="message"> The message to send. </param>
		/// <remarks>
		///     <para>
		///         The message is delivered after all previously sent or posted messages but this method returns immediately.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="message" /> is null. </exception>
		void Post (IMessage message);

		/// <summary>
		///     Removes a message dispatcher and stops using it for all subsequent messages.
		/// </summary>
		/// <param name="messageDispatcher"> The message dispatcher to remove. </param>
		/// <remarks>
		///     <note type="implement">
		///         Specifying an already removed message dispatcher should have no effect.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="messageDispatcher" /> is null. </exception>
		void RemoveDispatcher (IMessageDispatcher messageDispatcher);

		/// <summary>
		///     Removes a message receiver and stops using it for all subsequent messages.
		/// </summary>
		/// <param name="messageReceiver"> The message receiver to remove. </param>
		/// <remarks>
		///     <note type="implement">
		///         Specifying an already removed message receiver should have no effect.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="messageReceiver" /> is null. </exception>
		void RemoveReceiver (IMessageReceiver messageReceiver);
	}
}
