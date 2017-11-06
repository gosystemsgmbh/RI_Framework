using System;

using RI.Framework.Bus.Internals;

namespace RI.Framework.Bus
{
	/// <summary>
	///     Event arguments for message-related <see cref="IBus" /> events.
	/// </summary>
	[Serializable]
	public sealed class BusMessageEventArgs : EventArgs
	{
		/// <summary>
		/// Creates a new instance of <see cref="BusMessageEventArgs"/>.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <exception cref="ArgumentNullException"><paramref name="message"/> is null.</exception>
		public BusMessageEventArgs (MessageItem message)
		{
			if (message == null)
			{
				throw new ArgumentNullException(nameof(message));
			}

			this.Message = message;
		}

		/// <summary>
		/// Gets the message.
		/// </summary>
		/// <value>
		/// The message.
		/// </value>
		public MessageItem Message { get; }
	}
}
