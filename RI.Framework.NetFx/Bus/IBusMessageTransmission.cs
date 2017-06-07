using System;

namespace RI.Framework.Bus
{
	public interface IBusMessageTransmission
	{
		/// <summary>
		/// Gets the unique ID of this transmission.
		/// </summary>
		/// <value>
		/// The unique ID of this transmission.
		/// </value>
		Guid Id { get; }

		/// <summary>
		/// Gets the message.
		/// </summary>
		/// <value>
		/// The message.
		/// </value>
		IBusMessage Message { get; }
	}
}
