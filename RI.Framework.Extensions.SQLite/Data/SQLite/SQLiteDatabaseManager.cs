using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

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
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	public sealed class SQLiteDatabaseManager : IDatabaseManager, ILogSource
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

			if (connectionString.IsEmptyOrWhitespace())
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

			this.Connections = new List<SQLiteConnection>();
			this.ConnectionStateChangedHandler = this.ConnectionStateChangedMethod;

			this.SetState(DatabaseState.Uninitialized);
		}

		/// <summary>
		///     Garbage collects this instance of <see cref="SQLiteDatabaseManager" />.
		/// </summary>
		~SQLiteDatabaseManager ()
		{
			this.Dispose();
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets the cleanup script chain.
		/// </summary>
		/// <value>
		///     The cleanup script chain.
		/// </value>
		/// <remarks>
		///     <para>
		///         This list contains all SQL scripts which are executed during <see cref="CleanupDatabase" />.
		///     </para>
		///     <para>
		///         The scripts are executed in the order as they are in the list.
		///     </para>
		///     <para>
		///         null values and empty strings are ignored and the list can be empty.
		///     </para>
		/// </remarks>
		public List<string> CleanupScriptChain { get; private set; }

		/// <inheritdoc cref="IDatabaseManager.ConnectionStringBuilder" />
		public SQLiteConnectionStringBuilder ConnectionStringBuilder { get; private set; }

		/// <summary>
		///     Gets or sets the file path of the SQLite database file.
		/// </summary>
		/// <value>
		///     The file path of the SQLite database file.
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
		///     Gets the upgrade script chain.
		/// </summary>
		/// <value>
		///     The upgrade script chain.
		/// </value>
		/// <remarks>
		///     <para>
		///         This list contains all SQL scripts which are executed during <see cref="UpgradeDatabase" /> to upgrade the database to the latest known/supported version (which is <see cref="MaxVersion" />).
		///         Furthermore, this list is used to detect <see cref="MinVersion" /> and <see cref="MaxVersion" />.
		///     </para>
		///     <para>
		///         The scripts are executed in the order as they are in the list.
		///     </para>
		///     <para>
		///         The list can contain null values but not empty strings and can be empty.
		///     </para>
		///     <para>
		///         If the list is empty, only the version which will be detected using <see cref="VersionScriptChain" /> is supported (and <see cref="MinVersion" /> and <see cref="MaxVersion" /> will have the same value as <see cref="CurrentVersion" />).
		///     </para>
		///     <para>
		///         To specify that a particular version of the database can be upgraded to its next version, an upgrade script must be placed at the list index of the version from which can be upgraded.
		///         For example, if upgrade from version n to n+1 shall be supported, the corresponding script which performs the upgrade from version n to n+1 must be placed at the list index n.
		///         Upgrades are only possible step-by-step or by a version number increment of one respectively.
		///         The index zero is used to initialize a new database to its first version.
		///     </para>
		/// </remarks>
		public List<string> UpgradeScriptChain { get; private set; }

		/// <summary>
		///     Gets the version script chain.
		/// </summary>
		/// <value>
		///     The version script chain.
		/// </value>
		/// <remarks>
		///     <para>
		///         This list contains all SQL scripts which are executed during <see cref="InitializeDatabase" /> to detect the current version of the database.
		///     </para>
		///     <para>
		///         The scripts are executed in the order as they are in the list.
		///     </para>
		///     <para>
		///         The list cannot contain null values or empty strings and must contain at least one script.
		///     </para>
		///     <para>
		///         Each script must return a scalar value.
		///         When any script returns a scalar value of zero, the script chain is aborted and the database is considered <see cref="DatabaseState.DamagedOrInvalid" />.
		///         Therefore, the last script must return the actual version of the database (or zero) while previous scripts can be used to check for validity (non-zero if valid, zero if invalid).
		///     </para>
		/// </remarks>
		public List<string> VersionScriptChain { get; private set; }

		private List<string> CachedCleanupScriptChain { get; set; }

		private SQLiteConnectionStringBuilder CachedConnectionString { get; set; }

		private FilePath CachedDatabaseFile { get; set; }

		private List<string> CachedUpgradeScriptChain { get; set; }

		private List<string> CachedVersionScriptChain { get; set; }

		private List<SQLiteConnection> Connections { get; set; }

		private StateChangeEventHandler ConnectionStateChangedHandler { get; set; }

		#endregion




		#region Instance Methods

		/// <inheritdoc cref="IDatabaseManager.CreateConnection" />
		public SQLiteConnection CreateConnection (bool readOnly)
		{
			if (this.State != DatabaseState.Ready)
			{
				throw new InvalidOperationException("Cannot create SQLite connection when in state \"" + this.State + "\".");
			}

			return this.CreateConnectionInternal(readOnly);
		}

		private void ConnectionStateChangedMethod (object sender, StateChangeEventArgs e)
		{
			SQLiteConnection connection = (SQLiteConnection)sender;

			switch (e.CurrentState)
			{
				case ConnectionState.Broken:
				case ConnectionState.Closed:
				{
					this.Connections.Remove(connection);
					break;
				}
			}
		}

		private SQLiteConnection CreateConnectionInternal (bool readOnly)
		{
			SQLiteConnectionStringBuilder connectionString = new SQLiteConnectionStringBuilder(this.CachedConnectionString.ConnectionString);
			connectionString.ReadOnly = readOnly;

			SQLiteConnection connection = new SQLiteConnection(connectionString.ConnectionString);
			connection.Open();

			this.RegisterCollations(connection);
			this.RegisterFunctions(connection);

			connection.StateChange += this.ConnectionStateChangedHandler;
			this.Connections.Add(connection);

			EventHandler<DatabaseConnectionCreatedEventArgs> handler = this.ConnectionCreated;
			if (handler != null)
			{
				handler(this, new DatabaseConnectionCreatedEventArgs(connection, readOnly));
			}

			return connection;
		}

		private void OnExecuteScript (string script)
		{
			this.Log(LogLevel.Debug, "Executing database script: [{0}]{1}{2}", this, Environment.NewLine, script ?? "[null]");

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
			this.Log(LogLevel.Debug, "Preparing database script: [{0}]{1}{2}", this, Environment.NewLine, script ?? "[null]");

			EventHandler<DatabaseScriptEventArgs> handler = this.PrepareScript;
			if (handler != null)
			{
				DatabaseScriptEventArgs eventArgs = new DatabaseScriptEventArgs();
				eventArgs.Script = script;
				handler(this, eventArgs);
				script = eventArgs.Script;
			}
			return script.ToNullIfNullOrEmptyOrWhitespace();
		}

		private void RegisterCollations (SQLiteConnection connection)
		{
			connection.BindFunction(new CurrentCultureIgnoreCaseSQLiteCollation());
			connection.BindFunction(new CurrentCultureSQLiteCollation());
			connection.BindFunction(new InvariantCultureIgnoreCaseSQLiteCollation());
			connection.BindFunction(new InvariantCultureSQLiteCollation());
			connection.BindFunction(new OrdinalIgnoreCaseSQLiteCollation());
			connection.BindFunction(new OrdinalSQLiteCollation());

			connection.BindFunction(new TrimmedCurrentCultureIgnoreCaseSQLiteCollation());
			connection.BindFunction(new TrimmedCurrentCultureSQLiteCollation());
			connection.BindFunction(new TrimmedInvariantCultureIgnoreCaseSQLiteCollation());
			connection.BindFunction(new TrimmedInvariantCultureSQLiteCollation());
			connection.BindFunction(new TrimmedOrdinalIgnoreCaseSQLiteCollation());
			connection.BindFunction(new TrimmedOrdinalSQLiteCollation());
		}

		private void RegisterFunctions (SQLiteConnection connection)
		{
			connection.BindFunction(new TrimSQLiteFunction());
			connection.BindFunction(new ToEmptyIfEmptyOrNullSQLiteFunction());
			connection.BindFunction(new ToNullIfEmptyOrNullSQLiteFunction());

			connection.BindFunction(new IsNullOrEmptySQLiteFunction());

			connection.BindFunction(new RegularExpressionSQLiteFunction());
			connection.BindFunction(new MatchSQLiteFunction());
		}

		private void SetState (DatabaseState state)
		{
			this.Log(LogLevel.Debug, "Setting database state: {0} -> {1} @ [{2}]", this.State, state, this);

			if ((this.State == DatabaseState.Ready) && (state == DatabaseState.Uninitialized))
			{
				this.CloseConnections();
			}

			DatabaseState oldState = this.State;

			this.State = state;

			if (this.State == DatabaseState.Uninitialized)
			{
				this.InitialVersion = 0;
				this.InitialNew = false;
				this.InitialOld = false;

				this.CurrentVersion = 0;

				this.MinVersion = 0;
				this.MaxVersion = 0;

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




		#region Overrides

		/// <inheritdoc />
		public override string ToString ()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}; State={1}; CurrentVersion={2}; MinVersion={3}; MaxVersion={4}; Connections={5}; ConnectionString=[{6}]", this.GetType().Name, this.State, this.CurrentVersion, this.MinVersion, this.MaxVersion, this.Connections.Count, this.CachedConnectionString == null ? this.ConnectionString : this.CachedConnectionString.ConnectionString);
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

				if (value.IsEmptyOrWhitespace())
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
		public bool InitialNew { get; private set; }

		/// <inheritdoc />
		public bool InitialOld { get; private set; }

		/// <inheritdoc />
		public int InitialVersion { get; private set; }

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
				throw new InvalidOperationException("Cannot cleanup SQLite database when in state \"" + this.State + "\".");
			}

			this.Log(LogLevel.Information, "Beginning cleanup database: [{0}]", this);
			try
			{
				using (SQLiteConnection connection = this.CreateConnectionInternal(false))
				{
					using (SQLiteTransaction transaction = connection.BeginTransaction(IsolationLevel.Serializable))
					{
						foreach (string commandText in this.CachedCleanupScriptChain)
						{
							if (commandText.IsNullOrEmptyOrWhitespace())
							{
								continue;
							}

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

						transaction.Commit();
					}
				}
			}
			catch (SQLiteException exception)
			{
				this.Log(LogLevel.Error, "Exception while cleanup database: [{0}]{1}{2}", this, Environment.NewLine, exception.ToDetailedString());
				this.SetState(DatabaseState.DamagedOrInvalid);
				this.CurrentVersion = 0;
			}
			this.Log(LogLevel.Information, "Finished cleanup database: [{0}]", this);
		}

		/// <inheritdoc />
		public void CloseConnections ()
		{
			if (this.State != DatabaseState.Ready)
			{
				throw new InvalidOperationException("Cannot close SQLite connections when in state \"" + this.State + "\".");
			}

			this.Log(LogLevel.Debug, "Closing connections: [{0}]", this);

			foreach (SQLiteConnection connection in this.Connections.ToList())
			{
				connection.Close();
				connection.Dispose();
			}
		}

		/// <inheritdoc />
		DbConnection IDatabaseManager.CreateConnection (bool readOnly) => this.CreateConnection(readOnly);

		/// <inheritdoc />
		public void Dispose () => this.UnloadDatabase();

		/// <inheritdoc />
		public void InitializeDatabase ()
		{
			bool databaseFileExists = this.DatabaseFile.Exists;

			if (this.VersionScriptChain.Any(x => x == null))
			{
				throw new InvalidOperationException("Script in SQLite version script chain is null.");
			}

			if (this.VersionScriptChain.Any(x => x.IsEmptyOrWhitespace()))
			{
				throw new InvalidOperationException("Script in SQLite version script chain is empty.");
			}

			if (this.VersionScriptChain.Count == 0)
			{
				throw new InvalidOperationException("SQLite version script chain has no scripts.");
			}

			if (this.UpgradeScriptChain.Any(x => x.IsEmptyOrWhitespace()))
			{
				throw new InvalidOperationException("Script in SQLite upgrade script chain is empty.");
			}

			int firstUpgradeIndex = this.UpgradeScriptChain.FindIndex(x => x != null);
			int lastUpgradeIndex = this.UpgradeScriptChain.FindLastIndex(x => x != null);
			if (firstUpgradeIndex != lastUpgradeIndex)
			{
				for (int i1 = firstUpgradeIndex; i1 <= lastUpgradeIndex; i1++)
				{
					if (this.UpgradeScriptChain[i1] == null)
					{
						throw new InvalidOperationException("Script in SQLite upgrade script chain is null.");
					}
				}
			}

			if (this.State != DatabaseState.Uninitialized)
			{
				this.UnloadDatabase();
			}

			this.InitialVersion = 0;
			this.InitialNew = false;
			this.InitialOld = false;

			this.CurrentVersion = 0;

			this.MinVersion = 0;
			this.MaxVersion = 0;

			this.CachedConnectionString = new SQLiteConnectionStringBuilder(this.ConnectionString);
			this.CachedDatabaseFile = this.DatabaseFile.Clone();

			this.CachedVersionScriptChain = new List<string>(this.VersionScriptChain);
			this.CachedUpgradeScriptChain = new List<string>(this.UpgradeScriptChain);
			this.CachedCleanupScriptChain = new List<string>(this.CleanupScriptChain);

			this.Log(LogLevel.Debug, "Beginning version detection of database: [{0}]", this);
			if (!databaseFileExists)
			{
				this.CurrentVersion = 0;
			}
			else
			{
				try
				{
					using (SQLiteConnection connection = this.CreateConnectionInternal(false))
					{
						using (SQLiteTransaction transaction = connection.BeginTransaction(IsolationLevel.Serializable))
						{
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

									this.Log(LogLevel.Debug, "Database version detection result: {0} @ [{1}]", scalarResult ?? "[null]", this);

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

							transaction.Rollback();
							this.CurrentVersion = result;
						}
					}
				}
				catch (SQLiteException exception)
				{
					this.Log(LogLevel.Error, "Exception while version detection of database: [{0}]{1}{2}", this, Environment.NewLine, exception.ToDetailedString());
					this.SetState(DatabaseState.DamagedOrInvalid);
					this.CurrentVersion = 0;
				}
			}
			this.Log(LogLevel.Debug, "Finished version detection of database: [{0}]", this);

			if (this.State == DatabaseState.Uninitialized)
			{
				if ((firstUpgradeIndex != -1) && (lastUpgradeIndex != -1))
				{
					this.MinVersion = firstUpgradeIndex;
					this.MaxVersion = lastUpgradeIndex + 1;
				}
				else
				{
					this.MinVersion = this.CurrentVersion;
					this.MaxVersion = this.CurrentVersion;
				}

				if (!databaseFileExists)
				{
					this.SetState(DatabaseState.New);
				}
				else if (this.CurrentVersion == 0)
				{
					this.SetState(DatabaseState.DamagedOrInvalid);
				}
				else if (this.CurrentVersion < this.MinVersion)
				{
					this.SetState(DatabaseState.TooOld);
				}
				else if (this.CurrentVersion > this.MaxVersion)
				{
					this.SetState(DatabaseState.TooNew);
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

			this.InitialVersion = this.CurrentVersion;
			this.InitialNew = this.State == DatabaseState.New;
			this.InitialOld = this.State == DatabaseState.Old;

			this.Log(LogLevel.Debug, "Database initialized: [{0}]", this);
		}

		/// <inheritdoc />
		public void UnloadDatabase ()
		{
			this.Log(LogLevel.Debug, "Unloading database: [{0}]", this);

			this.SetState(DatabaseState.Uninitialized);
		}

		/// <inheritdoc />
		public void UpgradeDatabase ()
		{
			if ((this.State != DatabaseState.New) && (this.State != DatabaseState.Old))
			{
				throw new InvalidOperationException("Cannot upgrade SQLite database when in state \"" + this.State + "\".");
			}

			bool databaseFileCreated = this.CachedDatabaseFile.CreateIfNotExist();
			if (databaseFileCreated)
			{
				this.Log(LogLevel.Information, "Database file created: {0} @ [{1}]", this.CachedDatabaseFile, this);
			}

			int startVersion = this.CurrentVersion;

			this.Log(LogLevel.Information, "Beginning upgrade database: {0} -> {1} @ [{2}]", startVersion, this.MaxVersion, this);
			try
			{
				using (SQLiteConnection connection = this.CreateConnectionInternal(false))
				{
					using (SQLiteTransaction transaction = connection.BeginTransaction(IsolationLevel.Serializable))
					{
						for (int i1 = this.CurrentVersion; i1 < this.MaxVersion; i1++)
						{
							this.Log(LogLevel.Information, "Upgrading database: {0} -> {1} @ [{2}]", i1, i1 + 1, this);

							string commandText = this.CachedUpgradeScriptChain[i1];
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

						transaction.Commit();
						this.CurrentVersion = this.MaxVersion;
					}
				}
			}
			catch (SQLiteException exception)
			{
				this.Log(LogLevel.Error, "Exception while upgrade database: [{0}]{1}{2}", this, Environment.NewLine, exception.ToDetailedString());
				this.SetState(DatabaseState.DamagedOrInvalid);
				this.CurrentVersion = 0;
			}
			this.Log(LogLevel.Information, "Finished upgrade database: {0} -> {1} @ [{2}]", startVersion, this.MaxVersion, this);

			if ((this.State == DatabaseState.New) || (this.State == DatabaseState.Old))
			{
				this.SetState(DatabaseState.Ready);
			}
		}

		#endregion
	}
}
