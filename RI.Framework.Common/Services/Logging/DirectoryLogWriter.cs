using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text;

using RI.Framework.Composition.Model;
using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;




namespace RI.Framework.Services.Logging
{
	/// <summary>
	///     Implements a log writer which writes text log files into a specified directory.
	/// </summary>
	/// <remarks>
	///     <para>
	///         The specified directory (the common log directory) contains subdirectories where each instance of <see cref="DirectoryLogWriter" /> is associated with one such subdirectory and where the current log directory of this <see cref="DirectoryLogWriter" /> instance is such a subdirectory.
	///         The name of the current log directory corresponds to the specified timestamp or to the timestamp at the time the <see cref="DirectoryLogWriter" /> was instantiated (depending on the used constructor).
	///         The name of the current log directory is in the format yyyy-MM-dd-HH-mm-ss-fff or <c> 2016-02-01-14-30-50-333 </c> for example.
	///     </para>
	/// </remarks>
	[Export]
	public sealed class DirectoryLogWriter : ILogWriter,
	                                         IDisposable
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

		/// <summary>
		///     The default file name for the log files in the current log directories.
		/// </summary>
		/// <remarks>
		///     <para>
		///         The default file name is <c> Log.txt </c>.
		///     </para>
		/// </remarks>
		public static readonly string DefaultFileName = "Log.txt";

		#endregion




		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="DirectoryLogWriter" />.
		/// </summary>
		/// <param name="directory"> The common log directory. </param>
		/// <remarks>
		///     <para>
		///         The default file name <see cref="DefaultFileName" /> is used as the file name of the text log file in the current log directory.
		///     </para>
		///     <para>
		///         The default encoding <see cref="DefaultEncoding" /> is used as the text encoding to write the log files.
		///     </para>
		///     <para>
		///         <see cref="DateTime.Now" /> is used as the timestamp for the current log directory.
		///     </para>
		/// </remarks>
		public DirectoryLogWriter (string directory)
			: this(directory, null, null, DateTime.Now)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="DirectoryLogWriter" />.
		/// </summary>
		/// <param name="directory"> The common log directory. </param>
		/// <param name="fileName"> The file name of the text log file in the current log directory. </param>
		/// <param name="encoding"> The text encoding which is used to write the log files. </param>
		/// <param name="timestamp"> The timestamp which is used as the name for the current log directory. </param>
		public DirectoryLogWriter (string directory, string fileName, Encoding encoding, DateTime timestamp)
		{
			if (directory == null)
			{
				throw new ArgumentNullException(nameof(directory));
			}

			if (directory.IsEmpty())
			{
				throw new EmptyStringArgumentException(nameof(directory));
			}

			this.Encoding = encoding ?? DirectoryLogWriter.DefaultEncoding;
			this.Timestamp = timestamp;
			this.CommonDirectory = directory;

			this.SyncRoot = new object();
			this.CurrentLengths = new int[]
			{
				0, 0, 0, 0, 0, 0,
			};

			this.CurrentDirectory = Path.Combine(this.CommonDirectory, timestamp.ToSortableString('-'));

			Directory.CreateDirectory(this.CommonDirectory);
			Directory.CreateDirectory(this.CurrentDirectory);

			this.CurrentFile = Path.Combine(this.CurrentDirectory, fileName ?? DirectoryLogWriter.DefaultFileName);

			this.CurrentWriter = new StreamWriter(this.CurrentFile, false, this.Encoding);
		}

		/// <summary>
		///     Garbage collects this instance of <see cref="DirectoryLogWriter" />.
		/// </summary>
		~DirectoryLogWriter ()
		{
			this.Dispose(false);
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets the common log directory which contains the current log directory (<see cref="CurrentDirectory" />).
		/// </summary>
		/// <value>
		///     The common log directory.
		/// </value>
		public string CommonDirectory { get; private set; }

		/// <summary>
		///     Gets the current log directory which is a subdirectory of the common log directory (<see cref="CommonDirectory" />).
		/// </summary>
		/// <value>
		///     The current log directory.
		/// </value>
		public string CurrentDirectory { get; private set; }

		/// <summary>
		///     Gets the current log file in the current log directory (<see cref="CurrentDirectory" />).
		/// </summary>
		/// <value>
		///     The current log file.
		/// </value>
		public string CurrentFile { get; private set; }

		/// <summary>
		///     Gets the used text encoding which is used to write log files.
		/// </summary>
		/// <value>
		///     The used text encoding which is used to write log files.
		/// </value>
		public Encoding Encoding { get; private set; }

		/// <summary>
		///     Gets the timestamp which is used as the name for the current log directory.
		/// </summary>
		/// <value>
		///     The timestamp which is used as the name for the current log directory.
		/// </value>
		public DateTime Timestamp { get; private set; }

		private int[] CurrentLengths { get; set; }

		private StreamWriter CurrentWriter { get; set; }

		private object SyncRoot { get; set; }

		#endregion




		#region Instance Methods

		[SuppressMessage ("ReSharper", "UnusedParameter.Local")]
		private void Dispose (bool disposing)
		{
			lock (this.SyncRoot)
			{
				if (this.CurrentWriter != null)
				{
					try
					{
						this.CurrentWriter.Flush();
					}
					catch
					{
					}

					try
					{
						this.CurrentWriter.Close();
					}
					catch
					{
					}

					this.CurrentWriter = null;
				}
			}
		}

		#endregion




		#region Interface: IDisposable

		/// <inheritdoc />
		public void Dispose ()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion




		#region Interface: ILogWriter

		/// <inheritdoc />
		public void Cleanup (DateTime retentionDate)
		{
			string[] directories = Directory.GetDirectories(this.CommonDirectory, "*", SearchOption.TopDirectoryOnly);
			foreach (string directory in directories)
			{
				if (string.Equals(directory, this.CurrentDirectory, StringComparison.OrdinalIgnoreCase))
				{
					continue;
				}
				DateTime? timestamp = Path.GetDirectoryName(directory).ToDateTimeFromSortable('-');
				if (!timestamp.HasValue)
				{
					continue;
				}
				if (timestamp.Value == this.Timestamp)
				{
					continue;
				}
				if (timestamp >= retentionDate)
				{
					continue;
				}

				LogLocator.LogDebug(this.GetType().Name, "Cleaning up old log directory: {0}", directory);

				throw new NotImplementedException("TEST THIS FIRST !!!");
				//Directory.Delete(directory, true);
			}
		}

		/// <inheritdoc />
		public void Log (DateTime timestamp, int threadId, LogLevel severity, string source, string message)
		{
			lock (this.SyncRoot)
			{
				if (this.CurrentWriter != null)
				{
					try
					{
						message = message ?? string.Empty;
						int newLineIndex = message.IndexOf('\n');
						string firstLine = newLineIndex == -1 ? message.Trim() : message.Substring(0, newLineIndex).Trim();
						string[] subsequentLines = newLineIndex == -1 ? null : message.Substring(newLineIndex + 1).Trim().SplitLines(StringSplitOptions.None);

						string[] headers = new string[6];

						headers[0] = "#".PadRight(this.CurrentLengths[0], ' ');
						headers[1] = (" [" + timestamp.ToSortableString('-') + "]").PadRight(this.CurrentLengths[1], ' ');
						headers[2] = (" [" + threadId.ToString("D", CultureInfo.InvariantCulture) + "]").PadRight(this.CurrentLengths[2], ' ');
						headers[3] = (" [" + severity + "]").PadRight(this.CurrentLengths[3], ' ');
						headers[4] = (" [" + (source ?? "null") + "]").PadRight(this.CurrentLengths[4], ' ');
						headers[5] = (" ").PadRight(this.CurrentLengths[5], ' ');

						int headerLength = 0;

						for (int i1 = 0; i1 < headers.Length; i1++)
						{
							this.CurrentWriter.Write(headers[i1]);
							this.CurrentLengths[i1] = Math.Max(headers[i1].Length, this.CurrentLengths[i1]);
							headerLength += this.CurrentLengths[i1];
						}

						this.CurrentWriter.WriteLine(firstLine);

						if (subsequentLines != null)
						{
							foreach (string subsequentLine in subsequentLines)
							{
								if (!subsequentLine.IsEmpty())
								{
									this.CurrentWriter.Write(">".PadRight(headerLength, ' '));
									this.CurrentWriter.WriteLine(subsequentLine);
								}
							}
						}

						this.CurrentWriter.Flush();
					}
					catch
					{
					}
				}
			}
		}

		#endregion
	}
}
