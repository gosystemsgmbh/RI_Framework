using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;

using RI.Framework.Bus.Internals;
using RI.Framework.Utilities.Exceptions;
using RI.Framework.Utilities.ObjectModel;

namespace RI.Framework.Bus.Serializers
{
	/// <summary>
	/// Defines the interface for a bus serializer.
	/// </summary>
	/// <remarks>
	/// See <see cref="IBus"/> for more details about message busses.
	/// </remarks>
	public interface IBusSerializer
	{
		/// <summary>
		/// Initializes the serializer when the bus starts.
		/// </summary>
		/// <param name="dependencyResolver">The dependency resolver which can be used to get instances of required types.</param>
		/// <exception cref="ArgumentNullException"><paramref name="dependencyResolver"/> is null.</exception>
		void Initialize(IDependencyResolver dependencyResolver);

		/// <summary>
		/// Unloads the serializer when the bus stops.
		/// </summary>
		void Unload();

		/// <summary>
		/// Deserializes a message item from a string.
		/// </summary>
		/// <param name="data">The string.</param>
		/// <returns>
		/// The deserialized message item
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="data"/> is null.</exception>
		/// <exception cref="EmptyStringArgumentException"><paramref name="data"/> is an empty string.</exception>
		/// <exception cref="SerializationException">The deserialization failed.</exception>
		MessageItem DeserializeFromString (string data);

		/// <summary>
		/// Deserializes a message item from a stream.
		/// </summary>
		/// <param name="stream">The stream.</param>
		/// <param name="encoding">The used encoding, if necessary, or null if the serializer should decide which encoding to use.</param>
		/// <returns>
		/// The deserialized message item
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="stream"/> is null.</exception>
		/// <exception cref="NotReadableStreamArgumentException"><paramref name="stream"/> is a stream which cannot be read.</exception>
		/// <exception cref="SerializationException">The deserialization failed.</exception>
		MessageItem DeserializeFromStream (Stream stream, Encoding encoding);

		/// <summary>
		/// Serializes a message item to a string.
		/// </summary>
		/// <param name="messageItem">The message item.</param>
		/// <returns>
		/// The serialized message item as a string.
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="messageItem"/> is null.</exception>
		/// <exception cref="SerializationException">The serialization failed.</exception>
		string SerializeToString (MessageItem messageItem);

		/// <summary>
		/// Serializes a message item to a stream.
		/// </summary>
		/// <param name="messageItem">The message item.</param>
		/// <param name="stream">The stream.</param>
		/// <param name="encoding">The used encoding, if necessary, or null if the serializer should decide which encoding to use.</param>
		/// <exception cref="ArgumentNullException"><paramref name="messageItem"/> or <paramref name="stream"/> is null.</exception>
		/// <exception cref="NotWriteableStreamArgumentException"><paramref name="stream"/> is a stream which is not writeable.</exception>
		/// <exception cref="SerializationException">The serialization failed.</exception>
		void SerializeToStream (MessageItem messageItem, Stream stream, Encoding encoding);
	}
}
