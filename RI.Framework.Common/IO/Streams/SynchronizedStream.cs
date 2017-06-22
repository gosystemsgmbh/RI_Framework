using System;
using System.IO;

using RI.Framework.Utilities.ObjectModel;

namespace RI.Framework.IO.Streams
{
	/// <summary>
	/// Implements a stream which wraps another stream and synchronizes acces to it using a synchronization object.
	/// </summary>
	/// <remarks>
	/// <value>
	/// <see cref="SynchronizedStream"/> can be used to synchronize access to a stream from multiple threads.
	/// </value>
	/// <note type="important">
	/// Only the access to <see cref="Stream"/> members are synchronized.
	/// Repeated access (e.g. a <see cref="Seek"/> followed by a <see cref="Write"/>), which rely on to be of atomic nature, must be synchronized on a higher level, by the users of <see cref="SynchronizedStream"/>, using <see cref="SyncRoot"/>.
	/// </note>
	/// </remarks>
	public sealed class SynchronizedStream : Stream, ISynchronizable
	{
		/// <summary>
		/// Creates a new instance of <see cref="SynchronizedStream"/>.
		/// </summary>
		/// <param name="stream">The stream to wrap.</param>
		/// <remarks>
		/// <para>
		/// A new, dedicated synchronization object is created and used.
		/// </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"><paramref name="stream"/> is null.</exception>
		public SynchronizedStream(Stream stream)
			: this(stream, null)
		{
		}

		/// <summary>
		/// Creates a new instance of <see cref="SynchronizedStream"/>.
		/// </summary>
		/// <param name="stream">The stream to wrap.</param>
		/// <param name="syncRoot">The synchronization object to use. Can be null to create a new, dedicated synchronization object.</param>
		/// <exception cref="ArgumentNullException"><paramref name="stream"/> is null.</exception>
		public SynchronizedStream(Stream stream, object syncRoot)
		{
			if (stream == null)
			{
				throw new ArgumentNullException(nameof(stream));
			}

			this.BaseStream = stream;
			this.SyncRoot = syncRoot ?? new object();
		}

		/// <summary>
		///     Garbage collects this instance of <see cref="SynchronizedStream" />.
		/// </summary>
		~SynchronizedStream()
		{
			this.Close();
		}

		/// <summary>
		/// Gets the wrapped stream.
		/// </summary>
		/// <value>
		/// The wrapped stream.
		/// </value>
		public Stream BaseStream { get; }

		/// <inheritdoc />
		bool ISynchronizable.IsSynchronized => true;

		/// <inheritdoc />
		public object SyncRoot { get; }

		/// <inheritdoc />
		public override void Flush ()
		{
			lock (this.SyncRoot)
			{
				this.BaseStream.Flush();
			}
		}

		/// <inheritdoc />
		public override int Read (byte[] buffer, int offset, int count)
		{
			lock (this.SyncRoot)
			{
				return this.BaseStream.Read(buffer, offset, count);
			}
		}

		/// <inheritdoc />
		public override long Seek (long offset, SeekOrigin origin)
		{
			lock (this.SyncRoot)
			{
				return this.BaseStream.Seek(offset, origin);
			}
		}

		/// <inheritdoc />
		public override void SetLength (long value)
		{
			lock (this.SyncRoot)
			{
				this.BaseStream.SetLength(value);
			}
		}

		/// <inheritdoc />
		public override void Write (byte[] buffer, int offset, int count)
		{
			lock (this.SyncRoot)
			{
				this.BaseStream.Write(buffer, offset, count);
			}
		}

		/// <inheritdoc />
		public override bool CanRead
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this.BaseStream.CanRead;
				}
			}
		}

		/// <inheritdoc />
		public override bool CanSeek
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this.BaseStream.CanSeek;
				}
			}
		}

		/// <inheritdoc />
		public override bool CanWrite
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this.BaseStream.CanWrite;
				}
			}
		}

		/// <inheritdoc />
		public override long Length
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this.BaseStream.Length;
				}
			}
		}

		/// <inheritdoc />
		public override long Position
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this.BaseStream.Position;
				}
			}
			set
			{
				lock (this.SyncRoot)
				{
					this.BaseStream.Position = value;
				}
			}
		}

		/// <inheritdoc />
		protected override void Dispose (bool disposing)
		{
			lock (this.SyncRoot)
			{
				this.BaseStream.Close();
				base.Dispose(disposing);
			}
		}

		/// <inheritdoc />
		public override void Close ()
		{
			lock (this.SyncRoot)
			{
				this.BaseStream.Close();
				base.Close();
			}
		}

		/// <inheritdoc />
		public override IAsyncResult BeginRead (byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			lock (this.SyncRoot)
			{
				return this.BaseStream.BeginRead(buffer, offset, count, callback, state);
			}
		}

		/// <inheritdoc />
		public override IAsyncResult BeginWrite (byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			lock (this.SyncRoot)
			{
				return this.BaseStream.BeginWrite(buffer, offset, count, callback, state);
			}
		}

		/// <inheritdoc />
		public override int EndRead (IAsyncResult asyncResult)
		{
			lock (this.SyncRoot)
			{
				return this.BaseStream.EndRead(asyncResult);
			}
		}

		/// <inheritdoc />
		public override void EndWrite (IAsyncResult asyncResult)
		{
			lock (this.SyncRoot)
			{
				this.BaseStream.EndWrite(asyncResult);
			}
		}

		/// <inheritdoc />
		public override void WriteByte (byte value)
		{
			lock (this.SyncRoot)
			{
				this.BaseStream.WriteByte(value);
			}
		}

		/// <inheritdoc />
		public override int ReadByte ()
		{
			lock (this.SyncRoot)
			{
				return this.BaseStream.ReadByte();
			}
		}

		/// <inheritdoc />
		public override int WriteTimeout
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this.BaseStream.WriteTimeout;
				}
			}
			set
			{
				lock (this.SyncRoot)
				{
					this.BaseStream.WriteTimeout = value;
				}
			}
		}

		/// <inheritdoc />
		public override int ReadTimeout
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this.BaseStream.ReadTimeout;
				}
			}
			set
			{
				lock (this.SyncRoot)
				{
					this.BaseStream.ReadTimeout = value;
				}
			}
		}

		/// <inheritdoc />
		public override bool CanTimeout
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this.BaseStream.CanTimeout;
				}
			}
		}
	}
}
