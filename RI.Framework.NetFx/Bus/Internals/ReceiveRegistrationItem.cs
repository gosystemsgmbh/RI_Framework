using System;

namespace RI.Framework.Bus.Internals
{
	/// <summary>
	/// Holds a receiver registration as managed by a bus implementation.
	/// </summary>
	/// <remarks>
	/// <para>
	/// <see cref="ReceiverRegistrationItem"/> contains additional data which is only used internally by the bus implementation.
	/// </para>
	/// </remarks>
	public sealed class ReceiverRegistrationItem
	{
		/// <summary>
		/// Creates a new instance of <see cref="ReceiverRegistrationItem"/>.
		/// </summary>
		/// <param name="receiverRegistration">The receiver registration.</param>
		/// <exception cref="ArgumentNullException"><paramref name="receiverRegistration"/> is null.</exception>
		public ReceiverRegistrationItem(ReceiverRegistration receiverRegistration)
		{
			if (receiverRegistration == null)
			{
				throw new ArgumentNullException(nameof(receiverRegistration));
			}

			this.ReceiverRegistration = receiverRegistration;
		}

		/// <summary>
		/// Gets the receiver registration.
		/// </summary>
		/// <value>
		/// The receiver registration.
		/// </value>
		public ReceiverRegistration ReceiverRegistration { get; }
	}
}