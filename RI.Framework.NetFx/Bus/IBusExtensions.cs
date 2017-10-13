using System;

namespace RI.Framework.Bus
{
	/// <summary>
	///     Provides utility/extension methods for the <see cref="IBus" /> type.
	/// </summary>
	public static class IBusExtensions
	{
		/// <summary>
		/// Creates a new send operation which can be configured.
		/// </summary>
		/// <param name="bus">The bus.</param>
		/// <returns>
		/// The new send operation.
		/// </returns>
		/// <remarks>
		/// <note type="note">
		/// Although the name says &quot;Send&quot;, it does not send anything by itself.
		/// In fact, it only creates a new instance of <see cref="SendOperation"/>.
		/// This method exists purely to allow a fluent syntax.
		/// </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"><paramref name="bus"/> is null.</exception>
		public static SendOperation Send (this IBus bus)
		{
			if (bus == null)
			{
				throw new ArgumentNullException(nameof(bus));
			}

			return new SendOperation(bus);
		}

		/// <summary>
		/// Creates a new receiver registration which can be configured.
		/// </summary>
		/// <param name="bus">The bus.</param>
		/// <returns>
		/// The new receiver registration.
		/// </returns>
		/// <remarks>
		/// <note type="note">
		/// Although the name says &quot;Receive&quot;, it does not receive anything by itself.
		/// In fact, it only creates a new instance of <see cref="ReceiverRegistration"/>.
		/// This method exists purely to allow a fluent syntax.
		/// </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"><paramref name="bus"/> is null.</exception>
		public static ReceiverRegistration Receive (this IBus bus)
		{
			if (bus == null)
			{
				throw new ArgumentNullException(nameof(bus));
			}

			return new ReceiverRegistration(bus);
		}
	}
}
