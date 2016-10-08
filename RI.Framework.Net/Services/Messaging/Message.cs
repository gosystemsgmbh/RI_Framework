using System;
using System.Collections.Generic;

using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;




namespace RI.Framework.Services.Messaging
{
	/// <summary>
	///     Implements default message which uses a dictionary to transport the messages data.
	/// </summary>
	/// <remarks>
	///     <para>
	///         The dictionary which stortes the transported data, <see cref="Data" />, uses <see cref="StringComparer" />.<see cref="StringComparer.InvariantCultureIgnoreCase" />.
	///     </para>
	///     <para>
	///         See <see cref="IMessage" /> for more details.
	///     </para>
	/// </remarks>
	/// TODO: Fix null handling in Get methods
	public class Message : IMessage
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="MessageService" />.
		/// </summary>
		/// <param name="name"> The name of the message. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="name" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="name" /> is an empty string. </exception>
		public Message (string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException(nameof(name));
			}

			if (name.IsEmpty())
			{
				throw new EmptyStringArgumentException(nameof(name));
			}

			this.Name = name;

			this.Id = Guid.NewGuid();
			this.Data = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets the data dictionary which contains the messages data.
		/// </summary>
		/// <value>
		///     The data dictionary which contains the messages data.
		/// </value>
		public Dictionary<string, object> Data { get; private set; }

		#endregion




		#region Instance Methods

		/// <summary>
		///     Gets the first data item from the data dictionary which is of the specified type.
		/// </summary>
		/// <typeparam name="T"> The type of the data item. </typeparam>
		/// <returns>
		///     The data item.
		/// </returns>
		/// <exception cref="KeyNotFoundException"> No data item of the specified type was found. </exception>
		public T Get <T> ()
		{
			foreach (KeyValuePair<string, object> data in this.Data)
			{
				if (data.Value is T)
				{
					return (T)data.Value;
				}
			}

			throw new KeyNotFoundException();
		}

		/// <summary>
		///     Gets the data item with the specified name from the data dictionary.
		/// </summary>
		/// <typeparam name="T"> The type the data item should be converted to. </typeparam>
		/// <param name="name"> The name of the data item. </param>
		/// <returns>
		///     The data item.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="name" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="name" /> is an empty string. </exception>
		/// <exception cref="KeyNotFoundException"> No data item of the specified name was found. </exception>
		/// <exception cref="InvalidCastException"> The data item was found but could not be converted to type <typeparamref name="T" />. </exception>
		public T Get <T> (string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException(nameof(name));
			}

			if (name.IsEmpty())
			{
				throw new EmptyStringArgumentException(nameof(name));
			}

			if (!this.Data.ContainsKey(name))
			{
				throw new KeyNotFoundException();
			}

			object value = this.Data[name];

			if (!(value is T))
			{
				throw new InvalidCastException();
			}

			return (T)value;
		}

		#endregion




		#region Interface: IMessage

		/// <inheritdoc />
		public Guid Id { get; }

		/// <inheritdoc />
		public string Name { get; }

		/// <inheritdoc />
		object IMessage.GetData(string name)
		{
			return this.Data.ContainsKey(name) ? this.Data[name] : null;
		}

		#endregion
	}
}
