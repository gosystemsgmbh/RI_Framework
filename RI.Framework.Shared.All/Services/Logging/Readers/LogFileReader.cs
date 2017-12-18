﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;

using RI.Framework.IO.Paths;
using RI.Framework.Services.Logging.Writers;
using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;
using RI.Framework.Utilities.Logging;




namespace RI.Framework.Services.Logging.Readers
{
	/// <summary>
	///     Implements a log file reader which can read log files generated by <see cref="FileLogWriter" /> and <see cref="DirectoryLogWriter" />.
	/// </summary>
	public sealed class LogFileReader : IDisposable
	{
		#region Constants

		/// <summary>
		///     The default text encoding which is used to read the log files.
		/// </summary>
		/// <remarks>
		///     <para>
		///         The default text encoding is UTF-8.
		///     </para>
		/// </remarks>
		public static readonly Encoding DefaultEncoding = Encoding.UTF8;

		#endregion




		#region Static Methods

		internal static int MoveToNextEntryInStream (Stream stream, int minMove)
		{
			if (stream == null)
			{
				throw new ArgumentNullException(nameof(stream));
			}

			if (!stream.CanRead)
			{
				throw new NotReadableStreamArgumentException(nameof(stream));
			}

			if (!stream.CanSeek)
			{
				throw new NotSeekableStreamArgumentException(nameof(stream));
			}

			if (!stream.CanWrite)
			{
				throw new NotWriteableStreamArgumentException(nameof(stream));
			}

			if (minMove < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(minMove));
			}

			int read = 0;
			int stage = 0;
			while (true)
			{
				int current = stream.ReadByte();
				if (current == -1)
				{
					if (read >= minMove)
					{
						return read;
					}
					else
					{
						stream.SetLength(stream.Length + (minMove - read));
						return minMove;
					}
				}

				read++;

				switch (stage)
				{
					case 0:
						if ((char)current == '\n')
						{
							stage = 1;
						}
						break;

					case 1:
						if ((char)current == '#')
						{
							if (read >= minMove)
							{
								stream.Seek(-1, SeekOrigin.Current);
								return read - 1;
							}
							else
							{
								stage = 0;
							}
						}
						else
						{
							stage = 0;
						}
						break;
				}
			}
		}

		#endregion




		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="LogFileReader" />.
		/// </summary>
		/// <param name="file"> The log file. </param>
		/// <remarks>
		///     <para>
		///         The default encoding <see cref="DefaultEncoding" /> is used as the text encoding to read the log file.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="file" /> is null. </exception>
		/// <exception cref="InvalidPathArgumentException"> <paramref name="file" /> contains wildcards. </exception>
		/// <exception cref="FileNotFoundException"> The log file as specified by <paramref name="file" /> does not exist. </exception>
		public LogFileReader (FilePath file)
			: this(file, null)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="LogFileReader" />.
		/// </summary>
		/// <param name="file"> The log file. </param>
		/// <param name="encoding"> The text encoding which is used to read the log file (can be null to use <see cref="DefaultEncoding" />). </param>
		/// <exception cref="ArgumentNullException"> <paramref name="file" /> is null. </exception>
		/// <exception cref="InvalidPathArgumentException"> <paramref name="file" /> contains wildcards. </exception>
		/// <exception cref="FileNotFoundException"> The log file as specified by <paramref name="file" /> does not exist. </exception>
		public LogFileReader (FilePath file, Encoding encoding)
		{
			if (file == null)
			{
				throw new ArgumentNullException(nameof(file));
			}

			if (file.HasWildcards)
			{
				throw new InvalidPathArgumentException(nameof(file), "Wildcards are not allowed.");
			}

			if (!file.Exists)
			{
				throw new FileNotFoundException("The log file does not exist: " + file + ".", file);
			}

			this.File = file;
			this.Encoding = encoding ?? LogFileReader.DefaultEncoding;

			this.CurrentLineNumber = 0;
			this.CurrentValid = true;
			this.CurrentEntry = null;

			this.Reader = new StreamReader(this.File, this.Encoding);
			this.Buffer = null;
		}

		/// <summary>
		///     Garbage collects this instance of <see cref="LogFileReader" />.
		/// </summary>
		~LogFileReader ()
		{
			this.Dispose(false);
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets the current log entry.
		/// </summary>
		/// <value>
		///     The current log entry or null if the current line is invalid.
		/// </value>
		/// <remarks>
		///     <para>
		///         Before the first call to <see cref="ReadNext" />, this property is null.
		///     </para>
		///     <para>
		///         This property keeps its last value even if <see cref="ReadNext" /> returns false.
		///     </para>
		/// </remarks>
		public LogFileEntry CurrentEntry { get; private set; }

		/// <summary>
		///     Gets the current line number.
		/// </summary>
		/// <value>
		///     The current line number.
		/// </value>
		/// <remarks>
		///     <para>
		///         Before the first call to <see cref="ReadNext" />, this property is zero.
		///     </para>
		///     <para>
		///         This property keeps its last value even if <see cref="ReadNext" /> returns false.
		///     </para>
		/// </remarks>
		public int CurrentLineNumber { get; private set; }

		/// <summary>
		///     Gets whether the current line is valid.
		/// </summary>
		/// <value>
		///     true if the current line is valid, false otherwise.
		/// </value>
		/// <remarks>
		///     <para>
		///         Before the first call to <see cref="ReadNext" />, this property is true.
		///     </para>
		///     <para>
		///         This property keeps its last value even if <see cref="ReadNext" /> returns false.
		///     </para>
		/// </remarks>
		public bool CurrentValid { get; private set; }

		/// <summary>
		///     Gets the encoding which is used to read the log file.
		/// </summary>
		/// <value>
		///     The encoding to read the log file.
		/// </value>
		public Encoding Encoding { get; private set; }

		/// <summary>
		///     Gets the used log file.
		/// </summary>
		/// <value>
		///     The used log file.
		/// </value>
		public FilePath File { get; private set; }

		private string Buffer { get; set; }

		private StreamReader Reader { get; set; }

		#endregion




		#region Instance Methods

		/// <summary>
		///     Closes this log file reader and all used underlying streams.
		/// </summary>
		public void Close ()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}


		/// <summary>
		///     Reads the next log entry.
		/// </summary>
		/// <returns>
		///     true if another log entry was read, false if the end of the file was reached.
		/// </returns>
		/// <exception cref="ObjectDisposedException"> The log file reader has been closed/disposed. </exception>
		public bool ReadNext ()
		{
			this.VerifyNotClosed();

			string firstLine = null;
			List<string> subsequentLines = new List<string>();

			while (true)
			{
				string line = this.PeekLine();
				if (line == null)
				{
					this.ReadLine();
					break;
				}

				string trimmedLine = line.Trim();

				if (firstLine == null)
				{
					this.ReadLine();

					if (trimmedLine.StartsWith(LogFileFormatter.FirstLinePrefix, StringComparison.Ordinal))
					{
						firstLine = trimmedLine.Substring(1);
					}
					else
					{
						this.CurrentValid = false;
						this.CurrentEntry = null;
						return true;
					}
				}
				else
				{
					if (trimmedLine.StartsWith(LogFileFormatter.FirstLinePrefix, StringComparison.Ordinal))
					{
						break;
					}
					else if (trimmedLine.StartsWith(LogFileFormatter.SubsequentLinePrefix, StringComparison.Ordinal))
					{
						this.ReadLine();
						subsequentLines.Add(trimmedLine.Substring(1));
					}
					else
					{
						this.ReadLine();
						this.CurrentValid = false;
						this.CurrentEntry = null;
						return true;
					}
				}
			}

			if (firstLine == null)
			{
				return false;
			}

			List<string> headers = new List<string>();
			int lastPosition = 0;
			while (true)
			{
				int startIndex = firstLine.IndexOf("[", lastPosition, StringComparison.Ordinal);
				if (startIndex == -1)
				{
					break;
				}

				int stopIndex = firstLine.IndexOf("]", startIndex, StringComparison.Ordinal);
				if (startIndex == -1)
				{
					break;
				}

				lastPosition = stopIndex;

				string header = firstLine.Substring(startIndex + 1, stopIndex - (startIndex + 1));
				headers.Add(header);

				if (headers.Count == 4)
				{
					break;
				}
			}

			if (headers.Count != 4)
			{
				this.CurrentValid = false;
				this.CurrentEntry = null;
				return true;
			}

			int firstMessageStart = -1;
			int subsequentPadding = -1;
			for (int i1 = lastPosition + 1; i1 < firstLine.Length; i1++)
			{
				if (!char.IsWhiteSpace(firstLine, i1))
				{
					firstMessageStart = i1;
					subsequentPadding = i1;
					break;
				}
			}

			if ((firstMessageStart == -1) || (subsequentPadding == -1))
			{
				this.CurrentValid = false;
				this.CurrentEntry = null;
				return true;
			}

			DateTime? timestamp = headers[0].ToDateTimeFromSortable('-');
			int? threadId = headers[1].ToInt32Invariant();
			LogLevel? severity = headers[2].ToEnum<LogLevel>();
			string source = headers[3] ?? string.Empty;

			if ((!timestamp.HasValue) || (!threadId.HasValue) || (!severity.HasValue))
			{
				this.CurrentValid = false;
				this.CurrentEntry = null;
				return true;
			}

			StringBuilder message = new StringBuilder();
			message.AppendLine(firstLine.Substring(firstMessageStart));
			foreach (string subsequentLine in subsequentLines)
			{
				string trimmedSubsequentLine = subsequentLine.Substring(subsequentPadding);
				message.AppendLine(trimmedSubsequentLine);
			}

			LogFileEntry entry = new LogFileEntry();
			entry.Timestamp = timestamp.Value;
			entry.ThreadId = threadId.Value;
			entry.Severity = severity.Value;
			entry.Source = source;
			entry.Message = message.ToString();

			this.CurrentValid = true;
			this.CurrentEntry = entry;
			return true;
		}

		[SuppressMessage("ReSharper", "UnusedParameter.Local")]
		private void Dispose (bool disposing)
		{
			this.Reader?.Close();
			this.Reader = null;
		}

		private string PeekLine ()
		{
			if (this.Buffer != null)
			{
				return this.Buffer;
			}

			string line = this.Reader.ReadLine();
			this.Buffer = line;
			return line;
		}

		[SuppressMessage("ReSharper", "UnusedMethodReturnValue.Local")]
		private string ReadLine ()
		{
			this.CurrentLineNumber++;

			if (this.Buffer != null)
			{
				string line = this.Buffer;
				this.Buffer = null;
				return line;
			}

			string read = this.Reader.ReadLine();
			return read;
		}

		private void VerifyNotClosed ()
		{
			if (this.Reader == null)
			{
				throw new ObjectDisposedException(nameof(LogFileReader));
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
	}
}