using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;

using RI.Framework.Composition.Model;
using RI.Framework.IO.Paths;
using RI.Framework.Services.Logging.Filters;
using RI.Framework.Services.Logging.Readers;
using RI.Framework.Utilities.Exceptions;
using RI.Framework.Utilities.Logging;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Services.Logging.Writers
{
	/// <summary>
	///     Implements a log writer which writes to a specified text log file.
	/// </summary>
	/// <remarks>
	///     <para>
	///         A maximum log file size can be specified.
	///         If no maximum size is specified, the log file has no maximum size and will grow indefinitely.
	///     </para>
	///     <para>
	///         If a maximum size is specified and when this maximum size is reached, the log file &quot; rolls over&quot; and begins to overwrite the existing entries without clearing the file.
	///     </para>
	///     <note type="note">
	///         The maximum log file size is used as an approximate value, the log file might still exceed the size by hundreds of bytes.
	///     </note>
	///     <para>
	///         See <see cref="ILogWriter" /> for more details.
	///     </para>
	/// </remarks>
	/// <threadsafety static="true" instance="true" />
	[Export]
	public sealed class FileLogWriter : ILogWriter, IDisposable, ILogSource
	{
		#region Constants

		/// <summary>
		///     The default text encoding which is used to write the log files.
		/// </summary>
		/// <remarks>
		///     <para>
		///         The default text encoding is UTF-8.
		///     </para>
		/// </remarks>
		public static readonly Encoding DefaultEncoding = Encoding.UTF8;

		#endregion




		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="FileLogWriter" />.
		/// </summary>
		/// <param name="file"> The log file. </param>
		/// <param name="append"> Specifies whether log entries are appended to existing entries (true) or the file is cleared and overwritten (false). </param>
		/// <remarks>
		///     <para>
		///         The default encoding <see cref="DefaultEncoding" /> is used as the text encoding to write the log file.
		///     </para>
		///     <para>
		///         The log file has no maximum size and will grow indefinitely.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="file" /> is null. </exception>
		/// <exception cref="InvalidPathArgumentException"> <paramref name="file" /> contains wildcards. </exception>
		public FileLogWriter (FilePath file, bool append)
			: this(file, append, null, null)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="FileLogWriter" />.
		/// </summary>
		/// <param name="file"> The log file. </param>
		/// <param name="append"> Specifies whether log entries are appended to existing entries (true) or the file is cleared and overwritten (false). </param>
		/// <param name="maxSize"> The maximum size of the log file in bytes. Can be null or zero to indicate no maximum. </param>
		/// <param name="encoding"> The text encoding which is used to write the log file (can be null to use <see cref="DefaultEncoding" />). </param>
		/// <exception cref="ArgumentNullException"> <paramref name="file" /> is null. </exception>
		/// <exception cref="InvalidPathArgumentException"> <paramref name="file" /> contains wildcards. </exception>
		/// <exception cref="ArgumentOutOfRangeException"> <paramref name="maxSize" /> is less than zero. </exception>
		public FileLogWriter (FilePath file, bool append, long? maxSize, Encoding encoding)
		{
			if (file == null)
			{
				throw new ArgumentNullException(nameof(file));
			}

			if (file.HasWildcards)
			{
				throw new InvalidPathArgumentException(nameof(file), "Wildcards are not allowed.");
			}

			if (maxSize.HasValue)
			{
				if (maxSize < 0)
				{
					throw new ArgumentOutOfRangeException(nameof(maxSize));
				}
				if (maxSize == 0)
				{
					maxSize = null;
				}
			}

			this.SyncRoot = new object();

			this.File = file;
			this.Append = append;
			this.MaxSize = maxSize;
			this.Encoding = encoding ?? DirectoryLogWriter.DefaultEncoding;

			this.Formatter = null;
			this.RolledOver = false;

			this.Stream = null;
			this.Writer = null;

			this.Initialize(true);
		}

		/// <summary>
		///     Garbage collects this instance of <see cref="FileLogWriter" />.
		/// </summary>
		~FileLogWriter ()
		{
			this.Dispose(false);
		}

		#endregion




		#region Instance Fields

		private ILogFilter _filter;

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets whether log entries are appended to existing entries or the file is cleared and overwritten.
		/// </summary>
		/// <value>
		///     true if log entries are appended to existing entries, false otherwise.
		/// </value>
		public bool Append { get; }

		/// <summary>
		///     Gets the used text encoding which is used to write log files.
		/// </summary>
		/// <value>
		///     The used text encoding which is used to write log files.
		/// </value>
		public Encoding Encoding { get; }

		/// <summary>
		///     Gets the log file.
		/// </summary>
		/// <value>
		///     The log file.
		/// </value>
		public FilePath File { get; }

		/// <summary>
		///     Gets the maximum size of the log file in bytes.
		/// </summary>
		/// <value>
		///     The maximum log file size in bytes or null if no maximum is specified.
		/// </value>
		public long? MaxSize { get; }

		private LogFileFormatter Formatter { get; set; }

		private bool RolledOver { get; set; }

		private FileStream Stream { get; set; }

		private object SyncRoot { get; }

		private StreamWriter Writer { get; set; }

		#endregion




		#region Instance Methods

		/// <summary>
		///     Closes this log writer and all used underlying streams.
		/// </summary>
		/// <remarks>
		///     <para>
		///         After the log writer is closed, all calls to <see cref="Log" /> do not have any effect but do not fail.
		///     </para>
		/// </remarks>
		public void Close ()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		[SuppressMessage("ReSharper", "UnusedParameter.Local")]
		[SuppressMessage("ReSharper", "EmptyGeneralCatchClause")]
		private void Dispose (bool disposing)
		{
			lock (this.SyncRoot)
			{
				if (this.Writer != null)
				{
					try
					{
						this.Writer.Flush();
					}
					catch
					{
					}

					try
					{
						this.Writer.Close();
					}
					catch
					{
					}

					this.Writer = null;
				}

				if (this.Stream != null)
				{
					try
					{
						this.Stream.Flush();
					}
					catch
					{
					}

					try
					{
						this.Stream.Close();
					}
					catch
					{
					}

					this.Stream = null;
				}
			}
		}

		private void Initialize (bool construction)
		{
			bool isRollover = (!construction) && ((this.MaxSize.HasValue && (this.Stream != null)) ? (this.Stream.Length > this.MaxSize.Value) : false);
			bool requiresInitialize = construction || isRollover;

			if (!requiresInitialize)
			{
				return;
			}

			if (!isRollover)
			{
				this.Dispose(true);

				this.RolledOver = false;

				this.File.Directory.Create();

				bool success = false;
				try
				{
					this.Stream = new FileStream(this.File, this.Append ? FileMode.OpenOrCreate : FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
					this.Writer = new StreamWriter(this.Stream, this.Encoding);
					success = true;
				}
				finally
				{
					if (!success)
					{
						this.Writer?.Close();
						this.Writer = null;

						this.Stream?.Close();
						this.Stream = null;
					}
				}
			}

			this.Formatter = this.Formatter ?? new LogFileFormatter();
			this.Formatter.Reset();

			if (isRollover)
			{
				this.RolledOver = true;

				this.Writer.Flush();
				this.Stream.Flush();

				this.Stream.Seek(0, SeekOrigin.Begin);
			}
		}

		#endregion




		#region Interface: IDisposable

		/// <inheritdoc />
		void IDisposable.Dispose ()
		{
			this.Close();
		}

		#endregion




		#region Interface: ILogWriter

		/// <inheritdoc />
		public ILogFilter Filter
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this._filter;
				}
			}
			set
			{
				lock (this.SyncRoot)
				{
					this._filter = value;
				}
			}
		}

		/// <inheritdoc />
		bool ISynchronizable.IsSynchronized => true;

		/// <inheritdoc />
		object ISynchronizable.SyncRoot => this.SyncRoot;

		/// <inheritdoc />
		void ILogWriter.Cleanup (DateTime retentionDate)
		{
		}

		/// <inheritdoc />
		[SuppressMessage("ReSharper", "EmptyGeneralCatchClause")]
		public void Log (DateTime timestamp, int threadId, LogLevel severity, string source, string message)
		{
			ILogFilter filter = this.Filter;
			if (filter != null)
			{
				if (!filter.Filter(timestamp, threadId, severity, source))
				{
					return;
				}
			}

			lock (this.SyncRoot)
			{
				if ((this.Writer == null) || (this.Stream == null))
				{
					return;
				}

				try
				{
					this.Initialize(false);

					this.Formatter.Write(this.Writer, timestamp, threadId, severity, source, message);

					this.Writer.Flush();
#if PLATFORM_NETFX
					this.Stream.Flush(true);
#endif
#if PLATFORM_UNITY
					this.Stream.Flush();
#endif

					if (this.RolledOver)
					{
						string newLine = Environment.NewLine;
						int moved = LogFileReader.MoveToNextEntryInStream(this.Stream, newLine.Length * 2);
						this.Stream.Seek(moved * -1, SeekOrigin.Current);
						string padding = newLine + string.Empty.PadRight(moved - (newLine.Length * 2), ' ') + newLine;

						this.Writer.Write(padding);

						this.Writer.Flush();
#if PLATFORM_NETFX
						this.Stream.Flush(true);
#endif
#if PLATFORM_UNITY
						this.Stream.Flush();
#endif
					}
				}
				catch
				{
				}
			}
		}

		#endregion
	}
}
