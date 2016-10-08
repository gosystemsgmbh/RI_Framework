using System;




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
		///     Gets message data.
		/// </summary>
		/// <param name="name"> The name of the message data. </param>
		/// <returns>
		///     The message data or null if the specified message data does not exist.
		/// </returns>
		object GetData (string name);
	}
}
