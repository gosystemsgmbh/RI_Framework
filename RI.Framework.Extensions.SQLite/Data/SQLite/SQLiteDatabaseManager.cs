using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;

using RI.Framework.Data.Database;
using RI.Framework.Data.SQLite.Collations;
using RI.Framework.Data.SQLite.Functions;
using RI.Framework.IO.Paths;
using RI.Framework.Services.Logging;
using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;




namespace RI.Framework.Data.SQLite
{
	/// <summary>
	///     Implements a database manager for SQLite databases.
	/// </summary>
	/// <remarks>
	///     <para>
	///         See <see cref="IDatabaseManager" /> for more details.
	///     </para>
	/// </remarks>
	/// TODO: Implement connection tracking
	/// TODO: First prepare scripts, then execute
	/// TODO: Wrap script execution in transaction
	public sealed class SQLiteDatabaseManager : IDatabaseManager
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="SQLiteDatabaseManager" />.
		/// </summary>
		/// <param name="connectionString"> The connection string of the SQLite database. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="connectionString" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="connectionString" /> is an empty string. </exception>
		public SQLiteDatabaseManager (string connectionString)
			: this()
		{
			if (connectionString == null)
			{
				throw new ArgumentNullException(nameof(connectionString));
			}

			if (connectionString.IsEmpty())
			{
				throw new EmptyStringArgumentException(nameof(connectionString));
			}

			this.ConnectionString = connectionString;
		}

		/// <summary>
		///     Creates a new instance of <see cref="SQLiteDatabaseManager" />.
		/// </summary>
		/// <param name="databaseFile"> The file path of the SQLite database file. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="databaseFile" /> is null. </exception>
		/// <exception cref="InvalidPathArgumentException"> <paramref name="databaseFile" /> contains wildcards. </exception>
		public SQLiteDatabaseManager (FilePath databaseFile)
			: this()
		{
			if (databaseFile == null)
			{
				throw new ArgumentNullException(nameof(databaseFile));
			}

			if (databaseFile.HasWildcards)
			{
				throw new InvalidPathArgumentException(nameof(databaseFile));
			}

			this.DatabaseFile = databaseFile;
		}

		private SQLiteDatabaseManager ()
		{
			this.ConnectionStringBuilder = new SQLiteConnectionStringBuilder();
			this.VersionScriptChain = new List<string>();
			this.UpgradeScriptChain = new List<string>();
			this.CleanupScriptChain = new List<string>();

			this.SetState(DatabaseState.Uninitialized);
		}

		/// <summary>
		///     Garbage collects this instance of <see cref="SQLiteDatabaseManager" />.
		/// </summary>
		~SQLiteDatabaseManager ()
		{
			this.UnloadDatabase();
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		/// Gets the cleanup script chain.
		/// </summary>
		/// <value>
		/// The cleanup script chain.
		/// </value>
		/// <remarks>
		/// <para>
		/// TODO: Explanation
		/// </para>
		/// </remarks>
		public List<string> CleanupScriptChain { get; private set; }

		/// <inheritdoc cref="IDatabaseManager.ConnectionStringBuilder" />
		public SQLiteConnectionStringBuilder ConnectionStringBuilder { get; private set; }

		/// <summary>
		/// Gets or sets the file path of the SQLite database file.
		/// </summary>
		/// <value>
		/// The file path of the SQLite database file.
		/// </value>
		/// <exception cref="ArgumentNullException"> <paramref name="value" /> is null. </exception>
		/// <exception cref="InvalidPathArgumentException"> <paramref name="value" /> contains wildcards. </exception>
		public FilePath DatabaseFile
		{
			get
			{
				return this.ConnectionStringBuilder.DataSource;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException(nameof(value));
				}

				if (value.HasWildcards)
				{
					throw new InvalidPathArgumentException(nameof(value));
				}

				this.ConnectionStringBuilder.DataSource = value;
			}
		}

		/// <summary>
		/// Gets the upgrade script chain.
		/// </summary>
		/// <value>
		/// The upgrade script chain.
		/// </value>
		/// <remarks>
		/// <para>
		/// TODO: Explanation
		/// </para>
		/// </remarks>
		public List<string> UpgradeScriptChain { get; private set; }

		/// <summary>
		/// Gets the version script chain.
		/// </summary>
		/// <value>
		/// The version script chain.
		/// </value>
		/// <remarks>
		/// <para>
		/// TODO: Explanation
		/// </para>
		/// </remarks>
		public List<string> VersionScriptChain { get; private set; }

		private List<string> CachedCleanupScriptChain { get; set; }

		private SQLiteConnectionStringBuilder CachedConnectionString { get; set; }

		private FilePath CachedDatabaseFile { get; set; }

		private List<string> CachedUpgradeScriptChain { get; set; }

		private List<string> CachedVersionScriptChain { get; set; }

		#endregion




		#region Instance Methods

		/// <inheritdoc cref="IDatabaseManager.CreateConnection" />
		public SQLiteConnection CreateConnection (bool readOnly)
		{
			if (this.State != DatabaseState.Ready)
			{
				throw new InvalidOperationException("Cannot create connection when in state " + this.State + ".");
			}

			this.CachedConnectionString.ReadOnly = readOnly;

			SQLiteConnection connection = new SQLiteConnection(this.CachedConnectionString.ConnectionString);
			this.RegisterCollations(connection);
			this.RegisterFunctions(connection);

			EventHandler<DatabaseConnectionCreatedEventArgs> handler = this.ConnectionCreated;
			if (handler != null)
			{
				handler(this, new DatabaseConnectionCreatedEventArgs(connection, readOnly));
			}

			return connection;
		}

		private void Log (LogLevel severity, string format, params object[] args)
		{
			//TODO: Overhaul log
			LogLocator.Log(severity, this.GetType().Name, format, args);
		}

		private void OnExecuteScript (string script)
		{
			this.Log(LogLevel.Debug, "Executing database script: {0}{1}{2}", this.State == DatabaseState.Ready ? this.CachedConnectionString.ConnectionString : this.ConnectionStringBuilder.ConnectionString, Environment.NewLine, script ?? "[null]");

			EventHandler<DatabaseScriptEventArgs> handler = this.ExecuteScript;
			if (handler != null)
			{
				DatabaseScriptEventArgs eventArgs = new DatabaseScriptEventArgs();
				eventArgs.Script = script;
				handler(this, eventArgs);
			}
		}

		private string OnPrepareScript (string script)
		{
			EventHandler<DatabaseScriptEventArgs> handler = this.PrepareScript;
			if (handler != null)
			{
				DatabaseScriptEventArgs eventArgs = new DatabaseScriptEventArgs();
				eventArgs.Script = script;
				handler(this, eventArgs);
				script = eventArgs.Script;
			}
			return script.ToNullIfNullOrEmpty();
		}

		private void RegisterCollations (SQLiteConnection connection)
		{
			connection.BindFunction(new TrimmedCaseInsensitiveInvariantSQLiteCollation());
			connection.BindFunction(new TrimmedCaseInsensitiveCurrentSQLiteCollation());
		}

		private void RegisterFunctions (SQLiteConnection connection)
		{
			connection.BindFunction(new ToNullIfEmptyOrNullSQLiteFunction());
			connection.BindFunction(new IsNullOrEmptySQLiteFunction());
			connection.BindFunction(new RegularExpressionSQLiteFunction());
		}
		
		private void SetState (DatabaseState state)
		{
			this.Log(LogLevel.Information, "Setting database state: {0} @ {1}", state, state == DatabaseState.Ready ? this.CachedConnectionString.ConnectionString : this.ConnectionStringBuilder.ConnectionString);

			DatabaseState oldState = this.State;

			this.State = state;

			if (this.State == DatabaseState.Uninitialized)
			{
				this.CurrentVersion = 0;
				this.MinVersion = 0;
				this.MaxVersion = 0;
			}

			if (this.State != DatabaseState.Ready)
			{
				this.CachedConnectionString = null;
				this.CachedDatabaseFile = null;

				this.CachedVersionScriptChain = null;
				this.CachedUpgradeScriptChain = null;
				this.CachedCleanupScriptChain = null;
			}

			if (oldState != this.State)
			{
				EventHandler<DatabaseStateChangedEventArgs> handler = this.StateChanged;
				if (handler != null)
				{
					handler(this, new DatabaseStateChangedEventArgs(oldState, this.State));
				}
			}
		}

		#endregion




		#region Interface: IDatabaseManager

		/// <inheritdoc />
		public string ConnectionString
		{
			get
			{
				return this.ConnectionStringBuilder.ConnectionString;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException(nameof(value));
				}

				if (value.IsEmpty())
				{
					throw new EmptyStringArgumentException(nameof(value));
				}

				this.ConnectionStringBuilder.ConnectionString = value;
			}
		}

		/// <inheritdoc />
		DbConnectionStringBuilder IDatabaseManager.ConnectionStringBuilder => this.ConnectionStringBuilder;

		/// <inheritdoc />
		public int CurrentVersion { get; private set; }

		/// <inheritdoc />
		public int MaxVersion { get; private set; }

		/// <inheritdoc />
		public int MinVersion { get; private set; }

		/// <inheritdoc />
		public DatabaseState State { get; private set; }

		/// <inheritdoc />
		public bool SupportsConnectionTracking => false;
		
		/// <inheritdoc />
		public bool SupportsReadOnlyConnections => true;

		/// <inheritdoc />
		public event EventHandler<DatabaseConnectionCreatedEventArgs> ConnectionCreated;

		/// <inheritdoc />
		public event EventHandler<DatabaseScriptEventArgs> ExecuteScript;

		/// <inheritdoc />
		public event EventHandler<DatabaseScriptEventArgs> PrepareScript;

		/// <inheritdoc />
		public event EventHandler<DatabaseStateChangedEventArgs> StateChanged;

		/// <inheritdoc />
		public void CleanupDatabase ()
		{
			if (this.State != DatabaseState.Ready)
			{
				throw new InvalidOperationException("Cannot cleanup database when in state " + this.State + ".");
			}

			this.Log(LogLevel.Information, "Cleaning up database");

			try
			{
				//TODO: Internal create connection
				using (SQLiteConnection connection = this.CreateConnection(true))
				{
					connection.Open();

					foreach (string commandText in this.CachedCleanupScriptChain)
					{
						string usedCommandText = this.OnPrepareScript(commandText);
						if (usedCommandText == null)
						{
							continue;
						}

						using (SQLiteCommand command = connection.CreateCommand())
						{
							command.CommandText = usedCommandText;
							command.CommandType = CommandType.Text;

							this.OnExecuteScript(usedCommandText);
							command.ExecuteScalar();
						}
					}

					connection.Close();
				}
			}
			catch (SQLiteException exception)
			{
				this.Log(LogLevel.Error, "SQLite exception while detecting database version: {0}", exception.ToDetailedString());
				this.SetState(DatabaseState.DamagedOrInvalid);
			}
		}
		
		/// <inheritdoc />
		DbConnection IDatabaseManager.CreateConnection (bool readOnly) => this.CreateConnection(readOnly);

		/// <inheritdoc />
		void IDisposable.Dispose ()
		{
			this.UnloadDatabase();
		}

		/// <inheritdoc />
		public void InitializeDatabase ()
		{
			bool databaseFileExists = this.CachedDatabaseFile.Exists;

			int firstUpgradeIndex = this.UpgradeScriptChain.FindIndex(x => x != null);
			int lastUpgradeIndex = this.UpgradeScriptChain.FindLastIndex(x => x != null);

			//TODO: Check and throw min/max versions
			//TODO: Check and throw version script chain (no nulls, min one)

			if (this.State != DatabaseState.Uninitialized)
			{
				this.UnloadDatabase();
			}

			this.CurrentVersion = 0;
			this.MinVersion = firstUpgradeIndex;
			this.MaxVersion = lastUpgradeIndex + 1;

			this.CachedConnectionString = new SQLiteConnectionStringBuilder(this.ConnectionString);
			this.CachedDatabaseFile = this.DatabaseFile.Clone();

			this.CachedVersionScriptChain = new List<string>(this.VersionScriptChain);
			this.CachedUpgradeScriptChain = new List<string>(this.UpgradeScriptChain);
			this.CachedCleanupScriptChain = new List<string>(this.CleanupScriptChain);

			this.Log(LogLevel.Debug, "Begin detecting database version");
			if (!databaseFileExists)
			{
				this.CurrentVersion = 0;
			}
			else
			{
				try
				{
					//TODO: Internal create connection
					using (SQLiteConnection connection = this.CreateConnection(true))
					{
						connection.Open();

						int result = 0;
						foreach (string commandText in this.CachedVersionScriptChain)
						{
							string usedCommandText = this.OnPrepareScript(commandText);
							if (usedCommandText == null)
							{
								continue;
							}

							using (SQLiteCommand command = connection.CreateCommand())
							{
								command.CommandText = usedCommandText;
								command.CommandType = CommandType.Text;

								this.OnExecuteScript(usedCommandText);
								object scalarResult = command.ExecuteScalar();

								if (scalarResult is int)
								{
									result = (int)scalarResult;
								}
								else if (scalarResult is string)
								{
									int? resultCandidate = ((string)scalarResult).ToInt32Invariant();
									if (resultCandidate.HasValue)
									{
										result = resultCandidate.Value;
									}
									else
									{
										result = 0;
									}
								}
								else
								{
									result = 0;
								}
							}

							if (result == 0)
							{
								break;
							}
						}
						this.CurrentVersion = result;

						connection.Close();
					}
				}
				catch (SQLiteException exception)
				{
					this.Log(LogLevel.Error, "SQLite exception while detecting database version: {0}", exception.ToDetailedString());
					this.SetState(DatabaseState.DamagedOrInvalid);
				}
			}
			this.Log(LogLevel.Debug, "Finished detecting database version: {0}", this.CurrentVersion);

			if (this.State == DatabaseState.Uninitialized)
			{
				if (this.CurrentVersion < this.MinVersion)
				{
					this.SetState(DatabaseState.TooOld);
				}
				else if (this.CurrentVersion > this.MaxVersion)
				{
					this.SetState(DatabaseState.TooNew);
				}
				else if (this.CurrentVersion == 0)
				{
					this.SetState(DatabaseState.New);
				}
				else if (this.CurrentVersion < this.MaxVersion)
				{
					this.SetState(DatabaseState.Old);
				}
				else
				{
					this.SetState(DatabaseState.Ready);
				}
			}
		}

		/// <inheritdoc />
		public void UnloadDatabase ()
		{
			this.SetState(DatabaseState.Uninitialized);
		}

		/// <inheritdoc />
		public void UpgradeDatabase ()
		{
			if ((this.State != DatabaseState.New) && (this.State != DatabaseState.Old))
			{
				throw new InvalidOperationException("Cannot upgrade database when in state " + this.State + ".");
			}

			bool databaseFileCreated = this.DatabaseFile.CreateIfNotExist();
			if (databaseFileCreated)
			{
				this.Log(LogLevel.Information, "Database file created because it did not exist: {0}", this.DatabaseFile);
			}

			this.Log(LogLevel.Debug, "Begin upgrading database version: {0} -> {1}", this.CurrentVersion, this.MaxVersion);
			try
			{
				//TODO: Internal create connection
				using (SQLiteConnection connection = this.CreateConnection(false))
				{
					connection.Open();

					for (int i1 = this.CurrentVersion; i1 < this.MaxVersion; i1++)
					{
						this.Log(LogLevel.Debug, "Upgrading database version: {0} -> {1}", i1, i1 + 1);

						string commandText = this.UpgradeScriptChain[i1];
						string usedCommandText = this.OnPrepareScript(commandText);
						if (usedCommandText == null)
						{
							continue;
						}

						using (SQLiteCommand command = connection.CreateCommand())
						{
							command.CommandText = usedCommandText;
							command.CommandType = CommandType.Text;

							this.OnExecuteScript(usedCommandText);
							command.ExecuteNonQuery();
						}
					}

					connection.Close();
				}
			}
			catch (SQLiteException exception)
			{
				this.Log(LogLevel.Error, "SQLite exception while upgrading database version: {0}", exception.ToDetailedString());
				this.SetState(DatabaseState.DamagedOrInvalid);
			}
			this.Log(LogLevel.Debug, "Finished upgrading database version: {0} -> {1}", this.CurrentVersion, this.MaxVersion);

			if ((this.State == DatabaseState.New) || (this.State == DatabaseState.Old))
			{
				this.SetState(DatabaseState.Ready);
			}
		}

		#endregion
	}
}
