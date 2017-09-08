using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;

using RI.Framework.Composition.Model;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Services.Messaging.Dispatchers
{
	/// <summary>
	///     Defines the interface for a message dispatcher.
	/// </summary>
	/// <remarks>
	///     <para>
	///         A message dispatcher is used by a <see cref="IMessageService" /> to actually enqueue and deliver the messages to the receivers.
	///     </para>
	///     <note type="important">
	///         A message dispatcher is not intended to flow <see cref="ExecutionContext" /> or <see cref="CultureInfo" />.
	///         The thread to which the message delivery is dispatched defines the used execution context and thread culture.
	///         Therefore, the actual behaviour regarding execution context and thread culture depends on a <see cref="IMessageDispatcher" />s implementation.
	///     </note>
	///     <note type="important">
	///         The priority a message is dispatched with, if applicable, depends on a <see cref="IMessageDispatcher" />s implementation.
	///     </note>
	/// </remarks>
	/// <threadsafety static="true" instance="true" />
	[Export]
	public interface IMessageDispatcher : ISynchronizable
	{
		/// <summary>
		///     Asynchronously delivers a message.
		/// </summary>
		/// <param name="receivers"> The sequence of receivers. </param>
		/// <param name="message"> The message to deliver. </param>
		/// <param name="messageService"> The message service used to deliver the message. </param>
		/// <param name="deliveredCallback"> The callback to be called by the dispatcher when all receivers have received the message. Can be null if no callback is required. </param>
		/// <remarks>
		///     <note type="implement">
		///         This method must not return until the message is delivered to all receivers.
		///     </note>
		/// <note type="note">
		/// Do not call this method directly, it is intended to be called from an <see cref="IMessageService"/> implementation.
		/// </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="receivers" />, <paramref name="message" />, or <paramref name="messageService" /> is null. </exception>
		void Post (IEnumerable<IMessageReceiver> receivers, IMessage message, IMessageService messageService, Action<IMessage> deliveredCallback);
	}
}
