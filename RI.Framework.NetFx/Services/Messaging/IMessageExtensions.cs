using System;

using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;




namespace RI.Framework.Services.Messaging
{
	/// <summary>
	///     Provides utility/extension methods for the <see cref="IMessage" /> type.
	/// </summary>
	public static class IMessageExtensions
	{
		#region Static Methods

		/// <summary>
		///     Gets message data or a default value if the message does not contain the specified data.
		/// </summary>
		/// <param name="message"> The message. </param>
		/// <param name="name"> The name of the message data. </param>
		/// <param name="defaultValue"> The default value. </param>
		/// <returns>
		///     The message data or <paramref name="defaultValue" /> if the message does not have <paramref name="name" />.
		/// </returns>
		/// <remarks>
		///     <note type="note">
		///         Do not use <see cref="GetDataOrDefault" /> to check whether message data exists by checking the return value for the default value as the default value might be a valid value.
		///         Use <see cref="IMessage.HasData" /> instead.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="message" /> or <paramref name="name" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="name" /> is an empty string. </exception>
		public static object GetDataOrDefault (this IMessage message, string name, object defaultValue)
		{
			if (message == null)
			{
				throw new ArgumentNullException(nameof(message));
			}

			if (name == null)
			{
				throw new ArgumentNullException(nameof(name));
			}

			if (name.IsEmptyOrWhitespace())
			{
				throw new EmptyStringArgumentException(nameof(name));
			}

			return message.HasData(name) ? message.GetData(name) : defaultValue;
		}

		#endregion
	}
}
