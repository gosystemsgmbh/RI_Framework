using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;

using Newtonsoft.Json;

using RI.Framework.Bus.Internals;
using RI.Framework.IO.Streams;
using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;
using RI.Framework.Utilities.ObjectModel;

namespace RI.Framework.Bus.Serializers
{
	/// <summary>
	/// Implements a bus serializer which uses JSON.
	/// </summary>
	/// <remarks>
	/// See <see cref="IBusSerializer"/> for more details.
	/// </remarks>
	public sealed class JsonBusSerializer : IBusSerializer
	{
		/// <summary>
		/// The default encoding used for serializing/deserializing to/from streams.
		/// </summary>
		/// <value>
		/// The default encoding used for serializing/deserializing to/from streams.
		/// </value>
		public static readonly Encoding DefaultEncoding = Encoding.UTF8;


		/// <summary>
		///     Creates a new instance of <see cref="JsonBusSerializer" />.
		/// </summary>
		/// <remarks>
		///     <para>
		///         Default JSON serialization settings and <see cref="DefaultEncoding"/> are used.
		///     </para>
		/// </remarks>
		public JsonBusSerializer()
			: this(null, null)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="JsonBusSerializer" />.
		/// </summary>
		/// <param name="settings"> The used JSON serialization settings or null to use default settings. </param>
		/// <param name="encoding"> The used encoding for serializing/deserializing to/from streams or null to use the default encoding. </param>
		public JsonBusSerializer(JsonSerializerSettings settings, Encoding encoding)
		{
			this.Settings = settings;
			this.Encoding = encoding ?? JsonBusSerializer.DefaultEncoding;
			this.Serializer = this.Settings == null ? JsonSerializer.CreateDefault() : JsonSerializer.CreateDefault(this.Settings);
		}

		/// <summary>
		///     Gets the used JSON serialization settings.
		/// </summary>
		/// <value>
		///     The used JSON serialization settings or null if default settings are used.
		/// </value>
		public JsonSerializerSettings Settings { get; }

		/// <summary>
		///     Gets the used encoding when serializing/deserializing to/from streams.
		/// </summary>
		/// <value>
		///     The used encoding when serializing/deserializing to/from streams.
		/// </value>
		public Encoding Encoding { get; }

		private JsonSerializer Serializer { get; }

		/// <inheritdoc />
		public void Initialize (IDependencyResolver dependencyResolver)
		{
		}

		/// <inheritdoc />
		public void Unload ()
		{
		}

		/// <inheritdoc />
		public MessageItem DeserializeFromString (string data)
		{
			if (data == null)
			{
				throw new ArgumentNullException(nameof(data));
			}

			if (data.IsNullOrEmptyOrWhitespace())
			{
				throw new EmptyStringArgumentException(nameof(data));
			}

			try
			{
				return (MessageItem)JsonConvert.DeserializeObject(data, typeof(MessageItem), this.Settings);
			}
			catch (Exception exception)
			{
				throw new SerializationException("The message item deserialization from a JSON string failed.", exception);
			}
		}

		/// <inheritdoc />
		public MessageItem DeserializeFromStream (Stream stream, Encoding encoding)
		{
			if (stream == null)
			{
				throw new ArgumentNullException(nameof(stream));
			}

			if (!stream.CanRead)
			{
				throw new NotReadableStreamArgumentException(nameof(stream));
			}

			encoding = encoding ?? this.Encoding;

			UncloseableStream us = new UncloseableStream(stream);
			using (StreamReader sr = new StreamReader(us, encoding))
			{
				try
				{
					return (MessageItem)this.Serializer.Deserialize(sr, typeof(MessageItem));
				}
				catch (Exception exception)
				{
					throw new SerializationException("The message item deserialization from a JSON stream failed.", exception);
				}
			}
		}

		/// <inheritdoc />
		public string SerializeToString (MessageItem messageItem)
		{
			if (messageItem == null)
			{
				throw new ArgumentNullException(nameof(messageItem));
			}

			try
			{
				return JsonConvert.SerializeObject(messageItem, typeof(MessageItem), this.Settings);
			}
			catch (Exception exception)
			{
				throw new SerializationException("The message item serialization to a JSON string failed.", exception);
			}
		}

		/// <inheritdoc />
		public void SerializeToStream (MessageItem messageItem, Stream stream, Encoding encoding)
		{
			if (messageItem == null)
			{
				throw new ArgumentNullException(nameof(messageItem));
			}

			if (stream == null)
			{
				throw new ArgumentNullException(nameof(stream));
			}

			if (!stream.CanWrite)
			{
				throw new NotWriteableStreamArgumentException(nameof(stream));
			}

			encoding = encoding ?? this.Encoding;

			UncloseableStream us = new UncloseableStream(stream);
			using (StreamWriter sw = new StreamWriter(us, encoding))
			{
				try
				{
					this.Serializer.Serialize(sw, messageItem, typeof(MessageItem));
				}
				catch (Exception exception)
				{
					throw new SerializationException("The message item serialization to a JSON stream failed.", exception);
				}
			}
		}
	}
}
