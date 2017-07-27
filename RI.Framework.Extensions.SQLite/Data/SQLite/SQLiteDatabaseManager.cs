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
using RI.Framework.Services.Logging;
using RI.Framework.Utilities;
using RI.Framework.Utilities.Logging;

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
		/// <param name="configuration"> The database configuration. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="configuration" /> is null. </exception>
		public SQLiteDatabaseManager (SQLiteDatabaseConfiguration configuration)
		{
			if (configuration == null)
			{
				throw new ArgumentNullException(nameof(configuration));
			}

			this.Configuration = configuration;
			this.CachedConfiguration = null;

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

		/// <inheritdoc cref="IDatabaseManager.Configuration" />
		public SQLiteDatabaseConfiguration Configuration { get; }

		private SQLiteDatabaseConfiguration CachedConfiguration { get; set; }

		private List<SQLiteConnection> Connections { get; set; }

		private StateChangeEventHandler ConnectionStateChangedHandler { get; set; }

		#endregion




		#region Instance Methods

		/// <inheritdoc cref="IDatabaseManager.CreateConnection" />
		public SQLiteConnection CreateConnection (bool readOnly)
		{
			if (this.CurrentState != DatabaseState.Ready)
			{
				throw new InvalidOperationException("Cannot create SQLite connection when in state \"" + this.CurrentState + "\".");
			}

			if (readOnly && (!this.SupportsReadOnlyConnections))
			{
				throw new NotSupportedException("Read-only connections are not supported.");
			}

			return this.CreateConnectionInternal(readOnly);
		}

		private void CloseConnectionInternal ()
		{
			this.Log(LogLevel.Debug, "Closing connections: [{0}]", this);

			foreach (SQLiteConnection connection in this.Connections.ToList())
			{
				connection.Close();
				connection.Dispose();
			}
		}


		private void ConnectionStateChangedMethod (object sender, StateChangeEventArgs e)
		{
			SQLiteConnection connection = (SQLiteConnection)sender;

			connection.StateChange -= this.ConnectionStateChangedHandler;

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
			SQLiteConnectionStringBuilder connectionString = new SQLiteConnectionStringBuilder(this.CachedConfiguration.ConnectionString.ConnectionString);
			connectionString.ReadOnly = readOnly;

			SQLiteConnection connection = new SQLiteConnection(connectionString.ConnectionString);
			connection.Open();

			this.RegisterCollations(connection);
			this.RegisterFunctions(connection);

			connection.StateChange += this.ConnectionStateChangedHandler;
			this.Connections.Add(connection);

			this.ConnectionCreated?.Invoke(this, new DatabaseConnectionCreatedEventArgs(connection, readOnly));

			return connection;
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

			connection.BindFunction(new ToEmptyIfNullOrEmptyOrWhitespaceSQLiteFunction());
			connection.BindFunction(new ToNullIfNullOrEmptyOrWhitespaceSQLiteFunction());

			connection.BindFunction(new IsNullOrEmptyOrWhitespaceSQLiteFunction());

			connection.BindFunction(new MatchSQLiteFunction());
			connection.BindFunction(new RegularExpressionSQLiteFunction());
		}

		private void SetState (DatabaseState newState)
		{
			DatabaseState oldState = this.CurrentState;
			this.CurrentState = newState;

			this.Log(LogLevel.Debug, "Setting database state: {0} -> {1} @ [{2}]", oldState, newState, this);

			if (newState != DatabaseState.Ready)
			{
				this.CloseConnectionInternal();
			}

			if (newState == DatabaseState.Uninitialized)
			{
				this.CurrentState = DatabaseState.Uninitialized;
				this.InitialState = DatabaseState.Uninitialized;

				this.InitialVersion = 0;
				this.CurrentVersion = 0;

				this.MinVersion = 0;
				this.MaxVersion = 0;

				this.CachedConfiguration = null;
			}

			if (oldState != newState)
			{
				this.StateChanged?.Invoke(this, new DatabaseStateChangedEventArgs(oldState, newState));
			}
		}

		#endregion




		#region Overrides

		/// <inheritdoc />
		public override string ToString ()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}; State={1}; Version={2}; MinVersion={3}; MaxVersion={4}; Connections={5}; ConnectionString=[{6}]", this.GetType().Name, this.CurrentState, this.CurrentVersion, this.MinVersion, this.MaxVersion, this.Connections.Count, this.CachedConfiguration?.ConnectionString?.ConnectionString ?? "[null]");
		}

		#endregion




		#region Interface: IDatabaseManager

		/// <inheritdoc />
		DatabaseConfiguration IDatabaseManager.Configuration => this.Configuration;

		/// <inheritdoc />
		public DatabaseState CurrentState { get; private set; }

		/// <inheritdoc />
		public int CurrentVersion { get; private set; }

		/// <inheritdoc />
		public DatabaseState InitialState { get; private set; }

		/// <inheritdoc />
		public int InitialVersion { get; private set; }

		/// <inheritdoc />
		public int MaxVersion { get; private set; }

		/// <inheritdoc />
		public int MinVersion { get; private set; }

		/// <inheritdoc />
		public bool SupportsConnectionTracking => true;

		/// <inheritdoc />
		public bool SupportsReadOnlyConnections => true;

		/// <inheritdoc />
		public event EventHandler<DatabaseConnectionCreatedEventArgs> ConnectionCreated;

		/// <inheritdoc />
		public event EventHandler<DatabaseStateChangedEventArgs> StateChanged;

		/// <inheritdoc />
		public void CleanupDatabase ()
		{
			if (this.CurrentState != DatabaseState.Ready)
			{
				throw new InvalidOperationException("Cannot cleanup SQLite database when in state \"" + this.CurrentState + "\".");
			}

			this.Log(LogLevel.Information, "Beginning cleanup of database: [{0}]", this);
			try
			{
				using (SQLiteConnection connection = this.CreateConnectionInternal(false))
				{
					using (SQLiteTransaction transaction = connection.BeginTransaction(IsolationLevel.Serializable))
					{
						this.CachedConfiguration.CleanupProvider.Cleanup(this.CurrentVersion, connection, transaction, this.CachedConfiguration.ScriptLocator);

						transaction.Commit();
					}
				}
			}
			catch (Exception exception)
			{
				this.Log(LogLevel.Error, "Exception while cleaning up database: [{0}]{1}{2}", this, Environment.NewLine, exception.ToDetailedString());
				this.SetState(DatabaseState.DamagedOrInvalid);
				this.CurrentVersion = 0;
			}
			this.Log(LogLevel.Information, "Finished cleanup of database: [{0}]", this);
		}

		/// <inheritdoc />
		public void CloseConnections ()
		{
			if (this.CurrentState != DatabaseState.Ready)
			{
				throw new InvalidOperationException("Cannot close SQLite connections when in state \"" + this.CurrentState + "\".");
			}

			if (!this.SupportsConnectionTracking)
			{
				throw new NotSupportedException("Connection tracking is not supported.");
			}

			this.CloseConnectionInternal();
		}

		/// <inheritdoc />
		DbConnection IDatabaseManager.CreateConnection (bool readOnly) => this.CreateConnection(readOnly);

		/// <inheritdoc />
		public void Dispose () => this.UnloadDatabase();


		/// <inheritdoc />
		public void InitializeDatabase ()
		{
			if (this.Configuration.DatabaseFile == null)
			{
				throw new InvalidDatabaseConfigurationException("No database file configured.");
			}

			if (!this.Configuration.DatabaseFile.IsRealFile)
			{
				throw new InvalidDatabaseConfigurationException("Configured database file is not a real file.");
			}

			if (this.Configuration.ScriptLocator == null)
			{
				throw new InvalidDatabaseConfigurationException("No script locator configured.");
			}

			if (this.Configuration.VersionDetector == null)
			{
				throw new InvalidDatabaseConfigurationException("No version detector configured.");
			}

			if (this.Configuration.UpgradeProvider == null)
			{
				throw new InvalidDatabaseConfigurationException("No upgrade provider configured.");
			}

			if (this.Configuration.CleanupProvider == null)
			{
				throw new InvalidDatabaseConfigurationException("No cleanup provider configured.");
			}

			this.CachedConfiguration = this.Configuration.Clone();

			this.Log(LogLevel.Debug, "Initializing database: [{0}]", this);

			if (this.CurrentState != DatabaseState.Uninitialized)
			{
				this.UnloadDatabase();
			}

			this.CurrentState = DatabaseState.Uninitialized;
			this.InitialState = DatabaseState.Uninitialized;

			this.InitialVersion = 0;
			this.CurrentVersion = 0;

			this.MinVersion = 0;
			this.MaxVersion = 0;

			bool databaseFileExists = this.CachedConfiguration.DatabaseFile.Exists;

			this.Log(LogLevel.Debug, "Beginning version detection of database: [{0}]", this);
			if (!databaseFileExists)
			{
				this.CurrentVersion = 0;
			}
			else
			{
				try
				{
					using (SQLiteConnection connection = this.CreateConnectionInternal(true))
					{
						int detectedVersion = this.CachedConfiguration.VersionDetector.DetectCurrentVersion(connection, this.CachedConfiguration.ScriptLocator);
						if (detectedVersion == -1)
						{
							this.SetState(DatabaseState.DamagedOrInvalid);
							this.CurrentVersion = 0;
						}
						else
						{
							this.CurrentVersion = detectedVersion;
						}
					}
				}
				catch (Exception exception)
				{
					this.Log(LogLevel.Error, "Exception while detecting database version: [{0}]{1}{2}", this, Environment.NewLine, exception.ToDetailedString());
					this.SetState(DatabaseState.DamagedOrInvalid);
					this.CurrentVersion = 0;
				}
			}
			this.Log(LogLevel.Debug, "Finished version detection of database: [{0}]", this);

			if (this.CurrentState == DatabaseState.Uninitialized)
			{
				this.MinVersion = this.CachedConfiguration.UpgradeProvider.GetMinVersion();
				this.MaxVersion = this.CachedConfiguration.UpgradeProvider.GetMaxVersion();

				if (!databaseFileExists)
				{
					this.SetState(DatabaseState.New);
				}
				else if (this.CurrentVersion == 0)
				{
					this.SetState(DatabaseState.New);
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
			this.InitialState = this.CurrentState;

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
			if ((this.CurrentState != DatabaseState.New) && (this.CurrentState != DatabaseState.Old))
			{
				throw new InvalidOperationException("Cannot upgrade SQLite database when in state \"" + this.CurrentState + "\".");
			}

			bool databaseFileCreated = this.CachedConfiguration.DatabaseFile.CreateIfNotExist();
			if (databaseFileCreated)
			{
				this.Log(LogLevel.Information, "Database file created: {0} @ [{1}]", this.CachedConfiguration.DatabaseFile, this);
			}

			int startVersion = this.CurrentVersion;

			this.Log(LogLevel.Information, "Beginning upgrade of database: {0} -> {1} @ [{2}]", startVersion, this.MaxVersion, this);
			try
			{
				using (SQLiteConnection connection = this.CreateConnectionInternal(false))
				{
					using (SQLiteTransaction transaction = connection.BeginTransaction(IsolationLevel.Serializable))
					{
						while (true)
						{
							int currentVersion = this.CachedConfiguration.VersionDetector.DetectCurrentVersion(connection, this.CachedConfiguration.ScriptLocator);
							if (currentVersion >= this.MaxVersion)
							{
								this.CurrentVersion = currentVersion;
								transaction.Commit();
								break;
							}

							this.Log(LogLevel.Information, "Upgrading database: {0} @ [{1}]", currentVersion, this);

							this.CachedConfiguration.UpgradeProvider.Upgrade(currentVersion, connection, transaction, this.CachedConfiguration.ScriptLocator);
						}
					}
				}
			}
			catch (Exception exception)
			{
				this.Log(LogLevel.Error, "Exception while upgrading database: [{0}]{1}{2}", this, Environment.NewLine, exception.ToDetailedString());
				this.SetState(DatabaseState.DamagedOrInvalid);
				this.CurrentVersion = 0;
			}
			this.Log(LogLevel.Information, "Finished upgrade of database: {0} -> {1} @ [{2}]", startVersion, this.MaxVersion, this);

			if ((this.CurrentState == DatabaseState.New) || (this.CurrentState == DatabaseState.Old))
			{
				this.SetState(DatabaseState.Ready);
			}
		}

		#endregion
	}
}
