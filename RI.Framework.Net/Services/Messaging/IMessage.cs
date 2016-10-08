using System;
using System.Collections.Generic;

using RI.Framework.Utilities.Exceptions;




namespace RI.Framework.Services.Messaging
{
	/// <summary>
	///     Defines the interface for a message.
	/// </summary>
	/// <remarks>
	///     <para>
	///         The message interface defines only the basic message properties.
	///         The actual way of transporting data is defined by the concrete message type.
	///     </para>
	/// </remarks>
	public interface IMessage
	{
		/// <summary>
		///     Gets the unique ID of the message.
		/// </summary>
		/// <value>
		///     The unique ID of the message.
		/// </value>
		/// <remarks>
		///     <para>
		///         Each message has a unique ID, even those of the same type/name.
		///     </para>
		/// </remarks>
		Guid Id { get; }

		/// <summary>
		///     Gets the name of the message.
		/// </summary>
		/// <value>
		///     The name of the message.
		/// </value>
		/// <remarks>
		///     <para>
		///         Each message is of a particular type which is identified by its name.
		///     </para>
		///     <note type="implement">
		///         This property must never be null.
		///     </note>
		/// </remarks>
		string Name { get; }

		/// <summary>
		///     Deletes message data.
		/// </summary>
		/// <param name="name"> The name of the message data. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="name" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="name" /> is an empty string. </exception>
		void DeleteData (string name);

		/// <summary>
		///     Gets message data.
		/// </summary>
		/// <param name="name"> The name of the message data. </param>
		/// <returns>
		///     The message data.
		/// </returns>
		/// <remarks>
		///     <note type="note">
		///         Do not use <see cref="GetData" /> to check whether message data exists by checking the return value for null as null might be a valid value.
		///         Use <see cref="HasData" /> instead.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="name" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="name" /> is an empty string. </exception>
		/// <exception cref="KeyNotFoundException"> The message data with the name <paramref name="name" /> does not exist. </exception>
		object GetData (string name);

		/// <summary>
		///     Checks whether message data exists.
		/// </summary>
		/// <param name="name"> The name of the message data. </param>
		/// <returns>
		///     true if the message data exists, false otherwise.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="name" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="name" /> is an empty string. </exception>
		bool HasData (string name);

		/// <summary>
		///     Sets message data.
		/// </summary>
		/// <param name="name"> The name of the message data. </param>
		/// <param name="value"> The message data. </param>
		/// <remarks>
		///     <note type="note">
		///         <see cref="SetData" /> overwrites existing message data of the same name.
		///     </note>
		///     <note type="note">
		///         <see cref="SetData" /> does not delete message data, even if null is used for <paramref name="value" />, as null can be a valid value.
		///         Use <see cref="DeleteData" /> instead.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="name" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="name" /> is an empty string. </exception>
		void SetData (string name, object value);
	}
}
