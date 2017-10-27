using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics.CodeAnalysis;

using RI.Framework.Composition.Model;
using RI.Framework.IO.Files;
using RI.Framework.IO.Paths;
using RI.Framework.Services.Logging.Filters;
using RI.Framework.Services.Logging.Readers;
using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;
using RI.Framework.Utilities.Logging;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Services.Logging.Writers
{
	/// <summary>
	///     Implements a log writer which writes log entries to an SQLite database.
	/// </summary>
	/// <remarks>
	///     <para>
	///         See <see cref="ILogWriter" /> for more details.
	///     </para>
	/// </remarks>
	/// <threadsafety static="true" instance="true" />
	[Export]
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	public sealed class SQLiteLogWriter : LogSource, ILogWriter, IDisposable
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="SQLiteLogWriter" />.
		/// </summary>
		/// <param name="session"> The current session identification or null if not used. </param>
		/// <param name="dbFile"> The SQLite database file the log entries are written to. </param>
		/// <remarks>
		///     <para>
		///         The default configuration is used.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="dbFile" /> is null. </exception>
		/// <exception cref="InvalidPathArgumentException"> <paramref name="dbFile" /> contains wildcards. </exception>
		public SQLiteLogWriter (string session, FilePath dbFile)
			: this(session, dbFile, null)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="SQLiteLogWriter" />.
		/// </summary>
		/// <param name="session"> The current session identification or null if not used. </param>
		/// <param name="dbFile"> The SQLite database file the log entries are written to. </param>
		/// <param name="configuration"> The used configuration or null to use the default configuration. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="dbFile" /> is null. </exception>
		/// <exception cref="InvalidPathArgumentException"> <paramref name="dbFile" /> contains wildcards. </exception>
		public SQLiteLogWriter (string session, FilePath dbFile, SQLiteLogConfiguration configuration)
		{
			if (dbFile == null)
			{
				throw new ArgumentNullException(nameof(dbFile));
			}

			if (dbFile.HasWildcards)
			{
				throw new InvalidPathArgumentException(nameof(dbFile), "Wildcards are not allowed.");
			}

			this.Initialize(session, dbFile, null, configuration);
		}

		/// <summary>
		///     Creates a new instance of <see cref="SQLiteLogWriter" />.
		/// </summary>
		/// <param name="session"> The current session identification or null if not used. </param>
		/// <param name="dbConnection"> The SQLite connection to the database the log entries are written to. </param>
		/// <remarks>
		///     <para>
		///         The default configuration is used.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="dbConnection" /> is null. </exception>
		public SQLiteLogWriter (string session, SQLiteConnection dbConnection)
			: this(session, dbConnection, null)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="SQLiteLogWriter" />.
		/// </summary>
		/// <param name="session"> The current session identification or null if not used. </param>
		/// <param name="dbConnection"> The SQLite connection to the database the log entries are written to. </param>
		/// <param name="configuration"> The used configuration or null to use the default configuration. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="dbConnection" /> is null. </exception>
		public SQLiteLogWriter (string session, SQLiteConnection dbConnection, SQLiteLogConfiguration configuration)
		{
			if (dbConnection == null)
			{
				throw new ArgumentNullException(nameof(dbConnection));
			}

			this.Initialize(session, null, dbConnection, configuration);
		}

		/// <summary>
		///     Garbage collects this instance of <see cref="SQLiteLogWriter" />.
		/// </summary>
		~SQLiteLogWriter ()
		{
			this.Dispose(false);
		}

		#endregion




		#region Instance Fields

		private ILogFilter _filter;

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets the used database file.
		/// </summary>
		/// <value>
		///     The used database file.
		/// </value>
		public FilePath DbFile { get; private set; }

		/// <summary>
		///     Gets the current session identification.
		/// </summary>
		/// <value>
		///     The current session identification.
		/// </value>
		public string Session { get; private set; }

		private SQLiteLogConfiguration DbConfiguration { get; set; }
		private SQLiteConnection DbConnection { get; set; }
		private string InsertEntryCommandString { get; set; }

		private object SyncRoot { get; set; }

		#endregion




		#region Instance Methods

		/// <summary>
		///     Closes this log writer and all used underlying connections.
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
				if (this.DbConnection != null)
				{
					try
					{
						this.DbConnection.Close();
					}
					catch
					{
					}

					this.DbConnection = null;
				}
			}
		}

		private void Initialize (string session, FilePath dbFile, SQLiteConnection dbConnection, SQLiteLogConfiguration configuration)
		{
			this.SyncRoot = new object();

			this.Session = session.ToNullIfNullOrEmptyOrWhitespace();
			this.DbConfiguration = configuration ?? new SQLiteLogConfiguration();

			if (dbFile != null)
			{
				this.DbFile = dbFile;
				this.DbConnection = this.DbConfiguration.CreateConnection(dbFile);
			}
			else
			{
				this.DbConnection = dbConnection;
				this.DbFile = dbConnection.DataSource;
			}

			if (this.DbConnection.State != ConnectionState.Open)
			{
				this.DbConnection.Open();
			}

			string createTableCommandString = this.DbConfiguration.BuildCreateTableCommand();
			string createIndicesCommandString = this.DbConfiguration.BuildCreateIndexCommand();

			this.InsertEntryCommandString = this.DbConfiguration.BuildInsertEntryCommand();

			using (SQLiteCommand createTableCommand = new SQLiteCommand(createTableCommandString, this.DbConnection))
			{
				createTableCommand.ExecuteNonQuery();
			}

			if (createIndicesCommandString != null)
			{
				using (SQLiteCommand createIndicesCommand = new SQLiteCommand(createIndicesCommandString, this.DbConnection))
				{
					createIndicesCommand.ExecuteNonQuery();
				}
			}
		}

		/// <summary>
		/// Creates a copy of the current log database which can be safely accessed.
		/// </summary>
		/// <returns>
		/// The copy of the current log database as a temporary file or null if the log writer is disposed.
		/// </returns>
		public TemporaryFile CreateCopyOfCurrentDatabase()
		{
			lock (this.SyncRoot)
			{
				if (this.DbConnection == null)
				{
					return null;
				}

				if (this.DbConnection.State != ConnectionState.Open)
				{
					return null;
				}

				TemporaryFile tempFile = null;
				bool success = false;
				try
				{
					tempFile = new TemporaryFile(".logdb");

					SQLiteConnectionStringBuilder targetConnectionString = new SQLiteConnectionStringBuilder();
					targetConnectionString.DataSource = tempFile.File.PathResolved;

					using (SQLiteConnection target = new SQLiteConnection(targetConnectionString.ConnectionString))
					{
						target.Open();

						string sourceDatabaseName = "main";
						string targetDatabaseName = "main";

						this.DbConnection.BackupDatabase(target, targetDatabaseName, sourceDatabaseName, -1, null, 10);
					}

					success = true;
					return tempFile;
				}
				finally
				{
					if (!success)
					{
						tempFile?.Delete();
					}
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
				if (this.DbConnection == null)
				{
					return;
				}

				if (this.DbConnection.State != ConnectionState.Open)
				{
					return;
				}

				this.Log(LogLevel.Information, "Cleaning up old log entries: {0}", this.DbFile);

				try
				{
					using (SQLiteCommand cleanupCommand = this.DbConfiguration.BuildCleanupCommand(retentionDate, this.DbConnection, null))
					{
						cleanupCommand.ExecuteNonQuery();
					}
				}
				catch (Exception exception)
				{
					this.Log(LogLevel.Warning, "Could not cleanup log entries: {0}", exception.ToDetailedString());
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
				if (this.DbConnection == null)
				{
					return;
				}

				if (this.DbConnection.State != ConnectionState.Open)
				{
					return;
				}

				try
				{
					using (SQLiteCommand insertCommand = new SQLiteCommand(this.InsertEntryCommandString, this.DbConnection))
					{
						LogFileEntry entry = new LogFileEntry();
						entry.Timestamp = timestamp;
						entry.ThreadId = threadId;
						entry.Severity = severity;
						entry.Source = source;
						entry.Message = message;
						entry.Session = this.Session;

						Dictionary<string, object> parameters = this.DbConfiguration.BuildInsertEntryParameters(null, entry);
						foreach (KeyValuePair<string, object> param in parameters)
						{
							insertCommand.Parameters.AddWithValue(param.Key, param.Value);
						}
						insertCommand.ExecuteNonQuery();
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
