using System;
using System.Collections.Generic;

using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Services.Messaging
{
    /// <summary>
    ///     Implements the message object.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Each message is of a particular type which is identified by its <see cref="Name"/>.
    ///     </para>
    ///     <para>
    ///         Each message has a unique <see cref="Id"/>, even those of the same name.
    ///     </para>
    ///     <para>
    ///         The dictionary which stores the transported data, <see cref="Data" />, uses <see cref="StringComparerEx" />.<see cref="StringComparerEx.InvariantCultureIgnoreCase" />.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="true" instance="true" />
    [Obsolete(MessageService.ObsoleteMessage, false)]
    [Serializable]
    public sealed class Message : ISynchronizable, IEquatable<Message>
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="Message" />.
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

            if (name.IsEmptyOrWhitespace())
            {
                throw new EmptyStringArgumentException(nameof(name));
            }

            this.Name = name;

            this.SyncRoot = new object();
            this.Id = Guid.NewGuid();
            this.Data = new Dictionary<string, object>(StringComparerEx.InvariantCultureIgnoreCase);
        }

        #endregion




        #region Instance Properties/Indexer

        /// <summary>
        ///     Gets the data dictionary which contains the messages data.
        /// </summary>
        /// <value>
        ///     The data dictionary which contains the messages data.
        /// </value>
        public Dictionary<string, object> Data { get; }

        /// <summary>
        ///     Gets the unique ID of the message.
        /// </summary>
        /// <value>
        ///     The unique ID of the message.
        /// </value>
        public Guid Id { get; }

        /// <summary>
        ///     Gets the name of the message.
        /// </summary>
        /// <value>
        ///     The name of the message.
        /// </value>
        public string Name { get; }

        /// <inheritdoc />
        bool ISynchronizable.IsSynchronized => true;

        /// <inheritdoc />
        public object SyncRoot { get; }

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

            throw new KeyNotFoundException("No data item of type " + typeof(T).Name + " could be found.");
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

            if (name.IsEmptyOrWhitespace())
            {
                throw new EmptyStringArgumentException(nameof(name));
            }

            if (!this.Data.ContainsKey(name))
            {
                throw new KeyNotFoundException("No data item of name \"" + name + "\" could be found.");
            }

            object value = this.Data[name];

            if (!(value is T))
            {
                throw new InvalidCastException("A data item of name \"" + name + "\" was found but could not be converted to type " + typeof(T).Name);
            }

            return (T)value;
        }

        /// <summary>
        ///     Gets the first data item from the data dictionary which is of the specified type or a default value if the data item is not found.
        /// </summary>
        /// <typeparam name="T"> The type of the data item. </typeparam>
        /// <returns>
        ///     The data item or the default value for the type <typeparamref name="T" /> if the message does not have a data item of type <typeparamref name="T" />.
        /// </returns>
        public T GetOrDefault <T> ()
        {
            return this.GetOrDefault(default(T));
        }

        /// <summary>
        ///     Gets the first data item from the data dictionary which is of the specified type or a default value if the data item is not found.
        /// </summary>
        /// <typeparam name="T"> The type of the data item. </typeparam>
        /// <param name="defaultValue"> The default value. </param>
        /// <returns>
        ///     The data item or <paramref name="defaultValue" /> if the message does not have a data item of type <typeparamref name="T" />.
        /// </returns>
        public T GetOrDefault <T> (T defaultValue)
        {
            foreach (KeyValuePair<string, object> data in this.Data)
            {
                if (data.Value is T)
                {
                    return (T)data.Value;
                }
            }

            return defaultValue;
        }

        /// <summary>
        ///     Gets the data item with the specified name from the data dictionary or a default value if the data item is not found.
        /// </summary>
        /// <typeparam name="T"> The type the data item should be converted to. </typeparam>
        /// <param name="name"> The name of the data item. </param>
        /// <returns>
        ///     The data item or the default value for the type <typeparamref name="T" /> if the message does not have <paramref name="name" />.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="name" /> is null. </exception>
        /// <exception cref="EmptyStringArgumentException"> <paramref name="name" /> is an empty string. </exception>
        /// <exception cref="InvalidCastException"> The data item was found but could not be converted to type <typeparamref name="T" />. </exception>
        public T GetOrDefault <T> (string name)
        {
            return this.GetOrDefault(name, default(T));
        }

        /// <summary>
        ///     Gets the data item with the specified name from the data dictionary or a default value if the data item is not found.
        /// </summary>
        /// <typeparam name="T"> The type the data item should be converted to. </typeparam>
        /// <param name="name"> The name of the data item. </param>
        /// <param name="defaultValue"> The default value. </param>
        /// <returns>
        ///     The data item or <paramref name="defaultValue" /> if the message does not have <paramref name="name" />.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="name" /> is null. </exception>
        /// <exception cref="EmptyStringArgumentException"> <paramref name="name" /> is an empty string. </exception>
        /// <exception cref="InvalidCastException"> The data item was found but could not be converted to type <typeparamref name="T" />. </exception>
        public T GetOrDefault <T> (string name, T defaultValue)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (name.IsEmptyOrWhitespace())
            {
                throw new EmptyStringArgumentException(nameof(name));
            }

            if (!this.Data.ContainsKey(name))
            {
                return defaultValue;
            }

            object value = this.Data[name];

            if (!(value is T))
            {
                throw new InvalidCastException("A data item of name \"" + name + "\" was found but could not be converted to type " + typeof(T).Name);
            }

            return (T)value;
        }

        /// <summary>
        ///     Deletes message data.
        /// </summary>
        /// <param name="name"> The name of the message data. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="name" /> is null. </exception>
        /// <exception cref="EmptyStringArgumentException"> <paramref name="name" /> is an empty string. </exception>
        public void DeleteData(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (name.IsEmptyOrWhitespace())
            {
                throw new EmptyStringArgumentException(nameof(name));
            }

            this.Data.Remove(name);
        }

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
        public object GetData(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (name.IsEmptyOrWhitespace())
            {
                throw new EmptyStringArgumentException(nameof(name));
            }

            return this.Data[name];
        }

        /// <summary>
        ///     Checks whether message data exists.
        /// </summary>
        /// <param name="name"> The name of the message data. </param>
        /// <returns>
        ///     true if the message data exists, false otherwise.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="name" /> is null. </exception>
        /// <exception cref="EmptyStringArgumentException"> <paramref name="name" /> is an empty string. </exception>
        public bool HasData(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (name.IsEmptyOrWhitespace())
            {
                throw new EmptyStringArgumentException(nameof(name));
            }

            return this.Data.ContainsKey(name);
        }

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
        public void SetData(string name, object value)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (name.IsEmptyOrWhitespace())
            {
                throw new EmptyStringArgumentException(nameof(name));
            }

            if (this.Data.ContainsKey(name))
            {
                this.Data[name] = value;
            }
            else
            {
                this.Data.Add(name, value);
            }
        }

        #endregion




        #region Overrides

        /// <inheritdoc />
        public override bool Equals (object obj) => this.Equals(obj as Message);

        /// <inheritdoc />
        public override int GetHashCode () => this.Id.GetHashCode();

        #endregion




        #region Interface: IMessage


        /// <inheritdoc />
        public bool Equals (Message other)
        {
            if (other == null)
            {
                return false;
            }

            return this.Id.Equals(other.Id);
        }

        #endregion
    }
}
