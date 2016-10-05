using System;
using System.Collections.Generic;

using RI.Framework.Composition.Model;




namespace RI.Framework.Services.Messaging
{
	/// <summary>
	///     Defines the interface for a message dispatcher.
	/// </summary>
	/// <remarks>
	///     A message dispatcher is used by a <see cref="IMessageService" /> to actually enqueue and deliver the messages to the receivers.
	/// </remarks>
	[Export]
	public interface IMessageDispatcher
	{
		/// <summary>
		///     Asynchronously delivers a message.
		/// </summary>
		/// <param name="receivers"> The sequence of receivers. </param>
		/// <param name="message"> The message to deliver. </param>
		/// <remarks>
		///     <note type="implement">
		///         This method must not return until the message is delivered to all receivers.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="receivers" /> or <paramref name="message" /> is null. </exception>
		void Post (IEnumerable<IMessageReceiver> receivers, IMessage message);

		/// <summary>
		///     Synchronously delivers a message.
		/// </summary>
		/// <param name="receivers"> The sequence of receivers. </param>
		/// <param name="message"> The message to deliver. </param>
		/// <remarks>
		///     <note type="implement">
		///         This method must return immediately.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="receivers" /> or <paramref name="message" /> is null. </exception>
		void Send (IEnumerable<IMessageReceiver> receivers, IMessage message);
	}
}
