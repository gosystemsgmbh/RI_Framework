using System;

using RI.Framework.Composition.Model;




namespace RI.Framework.Services.Messaging
{
	/// <summary>
	///     The interface required for message receivers.
	/// </summary>
	/// <remarks>
	///     <para>
	///         Each message is delivered once by a <see cref="IMessageService" /> and the used <see cref="IMessageDispatcher" />s to all available message receivers.
	///     </para>
	/// </remarks>
	[Export]
	public interface IMessageReceiver
	{
		/// <summary>
		///     Called for each received message.
		/// </summary>
		/// <param name="message"> The received message. </param>
		/// <remarks>
		///     <note type="implement">
		///         The actual message delivery is done one receiver at a time.
		///         The delivery of the same message to the next receiver can only be made after <see cref="ReceiveMessage" /> returned.
		///         Therefore, long-running tasks should be avoided in <see cref="ReceiveMessage" />.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="message" /> is null. </exception>
		void ReceiveMessage (IMessage message);
	}
}
