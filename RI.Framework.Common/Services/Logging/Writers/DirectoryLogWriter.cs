using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;

using RI.Framework.Composition.Model;
using RI.Framework.IO.Paths;
using RI.Framework.Services.Logging.Filters;
using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;
using RI.Framework.Utilities.Logging;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Services.Logging.Writers
{
	/// <summary>
	///     Implements a log writer which writes text log files into a specified directory.
	/// </summary>
	/// <remarks>
	///     <para>
	///         The specified directory (the common log directory; <see cref="CommonDirectory" />) contains subdirectories where each instance of <see cref="DirectoryLogWriter" /> is associated with one such subdirectory and where the current log directory (<see cref="CurrentDirectory" />) of this <see cref="DirectoryLogWriter" /> instance is such a subdirectory.
	///         The name of the current log directory corresponds to the specified timestamp or to the timestamp at the time the <see cref="DirectoryLogWriter" /> was instantiated (depending on the used constructor).
	///         The name of the current log directory is in the format yyyy-MM-dd-HH-mm-ss-fff or <c> 2016-02-01-14-30-50-333 </c> for example.
	///     </para>
	///     <para>
	///         Depending on the value of <see cref="SeparateDays" />, a new current log directory is created and subsequently used when a new day starts, separating the log files by days.
	///         In such cases, the current directory has all its time parts set to zero (e.g. 2016-02-02-00-00-00-000) so a rollover can be easily recognized by the names of the subdirectories in the common log directory.
	///     </para>
	///     <para>
	///         See <see cref="ILogWriter" /> for more details.
	///     </para>
	/// </remarks>
	/// <threadsafety static="true" instance="true" />
	[Export]
	public sealed class DirectoryLogWriter : LogSource, ILogWriter, IDisposable
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
		/// <param name="separateDays"> Indicates whether each day has its own current log directory and current log file (true) or whether all log entries should be written in only one file (false). </param>
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
		/// <exception cref="ArgumentNullException"> <paramref name="directory" /> is null. </exception>
		/// <exception cref="InvalidPathArgumentException"> <paramref name="directory" /> contains wildcards. </exception>
		public DirectoryLogWriter (DirectoryPath directory, bool separateDays)
			: this(directory, separateDays, null, null, DateTime.Now)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="DirectoryLogWriter" />.
		/// </summary>
		/// <param name="directory"> The common log directory. </param>
		/// <param name="separateDays"> Indicates whether each day has its own current log directory and current log file (true) or all log entries should be written in only one log file (false). </param>
		/// <param name="fileName"> The file name of the text log file in the current log directory (can be null to use <see cref="DefaultFileName" />). </param>
		/// <param name="encoding"> The text encoding which is used to write the log files (can be null to use <see cref="DefaultEncoding" />). </param>
		/// <param name="timestamp"> The timestamp which is used as the name for the current log directory. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="directory" /> is null. </exception>
		/// <exception cref="InvalidPathArgumentException"> <paramref name="directory" /> contains wildcards or <paramref name="fileName" /> is not a valid file name. </exception>
		public DirectoryLogWriter (DirectoryPath directory, bool separateDays, string fileName, Encoding encoding, DateTime timestamp)
		{
			if (directory == null)
			{
				throw new ArgumentNullException(nameof(directory));
			}

			if (directory.HasWildcards)
			{
				throw new InvalidPathArgumentException(nameof(directory), "Wildcards are not allowed.");
			}

			FilePath fileNamePath;
			try
			{
				fileNamePath = new FilePath(fileName ?? DirectoryLogWriter.DefaultFileName, false, true, PathProperties.GetSystemType());
			}
			catch (InvalidPathArgumentException)
			{
				throw new InvalidPathArgumentException(nameof(fileName), "Invalid file name.");
			}

			this.SyncRoot = new object();

			this.CommonDirectory = directory;
			this.SeparateDays = separateDays;
			this.FileName = fileNamePath;
			this.Encoding = encoding ?? DirectoryLogWriter.DefaultEncoding;
			this.InitialTimestamp = timestamp;

			this.Formatter = null;
			this.CurrentTimestamp = timestamp;
			this.CurrentDirectory = null;
			this.CurrentFile = null;

			this.CurrentStream = null;
			this.CurrentWriter = null;

			this.InitializeCurrent(true);
		}

		/// <summary>
		///     Garbage collects this instance of <see cref="DirectoryLogWriter" />.
		/// </summary>
		~DirectoryLogWriter ()
		{
			this.Dispose(false);
		}

		#endregion




		#region Instance Fields

		private DirectoryPath _currentDirectory;

		private FilePath _currentFile;

		private DateTime _currentTimestamp;

		private ILogFilter _filter;

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets the common log directory which contains the current log directory (<see cref="CurrentDirectory" />).
		/// </summary>
		/// <value>
		///     The common log directory.
		/// </value>
		public DirectoryPath CommonDirectory { get; }

		/// <summary>
		///     Gets the current log directory which is a subdirectory of the common log directory (<see cref="CommonDirectory" />).
		/// </summary>
		/// <value>
		///     The current log directory.
		/// </value>
		public DirectoryPath CurrentDirectory
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this._currentDirectory;
				}
			}
			private set
			{
				lock (this.SyncRoot)
				{
					this._currentDirectory = value;
				}
			}
		}

		/// <summary>
		///     Gets the current log file in the current log directory (<see cref="CurrentDirectory" />).
		/// </summary>
		/// <value>
		///     The current log file.
		/// </value>
		public FilePath CurrentFile
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this._currentFile;
				}
			}
			private set
			{
				lock (this.SyncRoot)
				{
					this._currentFile = value;
				}
			}
		}

		/// <summary>
		///     Gets the timestamp which is used as the name for the current log directory.
		/// </summary>
		/// <value>
		///     The timestamp which is used as the name for the current log directory.
		/// </value>
		public DateTime CurrentTimestamp
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this._currentTimestamp;
				}
			}
			private set
			{
				lock (this.SyncRoot)
				{
					this._currentTimestamp = value;
				}
			}
		}

		/// <summary>
		///     Gets the used text encoding which is used to write log files.
		/// </summary>
		/// <value>
		///     The used text encoding which is used to write log files.
		/// </value>
		public Encoding Encoding { get; }

		/// <summary>
		///     Gets the file name of the text log file in the current log directory.
		/// </summary>
		/// <value>
		///     The file name of the text log file in the current log directory.
		/// </value>
		public FilePath FileName { get; }

		/// <summary>
		///     Gets the initial timestamp this <see cref="DirectoryLogWriter" /> was created with.
		/// </summary>
		/// <value>
		///     The initial timestamp this <see cref="DirectoryLogWriter" /> was created with.
		/// </value>
		public DateTime InitialTimestamp { get; }

		/// <summary>
		///     Gets whether each day has its own current log directory and current log file or all log entries should be written in only one log file.
		/// </summary>
		/// <value>
		///     true if each day has its own current log directory and current log file, false if all log entries should be written in only one log file.
		/// </value>
		public bool SeparateDays { get; }

		private FileStream CurrentStream { get; set; }

		private StreamWriter CurrentWriter { get; set; }

		private LogFileFormatter Formatter { get; set; }

		private object SyncRoot { get; }

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

				if (this.CurrentStream != null)
				{
					try
					{
						this.CurrentStream.Flush();
					}
					catch
					{
					}

					try
					{
						this.CurrentStream.Close();
					}
					catch
					{
					}

					this.CurrentStream = null;
				}
			}
		}

		private void InitializeCurrent (bool construction)
		{
			DateTime now = DateTime.Now;

			bool requiresInitialize = construction || (this.CurrentTimestamp.Date != now.Date);
			bool isSeparation = !construction;

			if (!requiresInitialize)
			{
				return;
			}

			this.Dispose(true);

			this.Formatter = this.Formatter ?? new LogFileFormatter();
			this.Formatter.Reset();

			this.CurrentTimestamp = isSeparation ? now.Date : this.InitialTimestamp;
			this.CurrentDirectory = this.CommonDirectory.AppendDirectory(this.CurrentTimestamp.ToSortableString('-'));
			this.CurrentFile = this.CurrentDirectory.AppendFile(this.FileName);

			this.CommonDirectory.Create();
			this.CurrentDirectory.Create();

			bool success = false;
			try
			{
				this.CurrentStream = new FileStream(this.CurrentFile, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
				this.CurrentWriter = new StreamWriter(this.CurrentStream, this.Encoding);
				success = true;
			}
			finally
			{
				if (!success)
				{
					this.CurrentWriter?.Close();
					this.CurrentWriter = null;

					this.CurrentStream?.Close();
					this.CurrentStream = null;
				}
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
		public void Cleanup (DateTime retentionDate)
		{
			lock (this.SyncRoot)
			{
				if ((this.CurrentWriter == null) || (this.CurrentStream == null))
				{
					return;
				}

				List<DirectoryPath> directories = this.CommonDirectory.GetSubdirectories(false, false);
				foreach (DirectoryPath directory in directories)
				{
					if (directory == this.CurrentDirectory)
					{
						continue;
					}
					DateTime? timestamp = directory.DirectoryName.ToDateTimeFromSortable('-');
					if (!timestamp.HasValue)
					{
						continue;
					}
					if (timestamp.Value == this.CurrentTimestamp)
					{
						continue;
					}
					if (timestamp >= retentionDate)
					{
						continue;
					}

					this.Log(LogLevel.Information, "Cleaning up old log directory: {0}", directory);

					try
					{
						Directory.Delete(directory, true);
					}
					catch (Exception exception)
					{
						this.Log(LogLevel.Warning, "Could not cleanup log directory: {0}", exception.ToDetailedString());
					}
				}
			}
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
				if ((this.CurrentWriter == null) || (this.CurrentStream == null))
				{
					return;
				}

				try
				{
					this.InitializeCurrent(false);

					this.Formatter.Write(this.CurrentWriter, timestamp, threadId, severity, source, message);

					this.CurrentWriter.Flush();
#if PLATFORM_NETFX
					this.CurrentStream.Flush(true);
#endif
#if PLATFORM_UNITY
					this.CurrentStream.Flush();
#endif
				}
				catch
				{
				}
			}
		}

		#endregion
	}
}
