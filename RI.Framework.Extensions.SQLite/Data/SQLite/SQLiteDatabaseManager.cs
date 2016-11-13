using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;

using RI.Framework.Data.SQLite.Collations;
using RI.Framework.Data.SQLite.Functions;
using RI.Framework.IO.Paths;
using RI.Framework.Services.Logging;
using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;




namespace RI.Framework.Data.SQLite
{
	/// <summary>
	/// Implements a database manager for SQLite databases.
	/// </summary>
	/// <remarks>
	/// <para>
	/// <see cref="IDatabaseManager"/> for more details.
	/// </para>
	/// </remarks>
	// TODO: Property for ID, created timestamp, created version
	// TODO: Property for database history
	public sealed class SQLiteDatabaseManager : IDatabaseManager
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates anew instance of <see cref="SQLiteDatabaseManager" />.
		/// </summary>
		/// <param name="connectionString"> The connection string of the SQLite database file. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="connectionString" /> is null. </exception>
		/// <exception cref="InvalidPathArgumentException"> <paramref name="connectionString" /> contains wildcards. </exception>
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
		///     Creates anew instance of <see cref="SQLiteDatabaseManager" />.
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

			//TODO: Register per connection using event or using configuration type
			this.RegisterCollations();
			this.RegisterFunctions();
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

		public List<string> CleanupScriptChain { get; set; }

		/// <inheritdoc cref="IDatabaseManager.ConnectionStringBuilder" />
		public SQLiteConnectionStringBuilder ConnectionStringBuilder { get; private set; }

		/// <summary>
		///     Gets or sets the file path of the SQLite database file.
		/// </summary>
		/// <value>
		///     The file path of the SQLite database file.
		/// </value>
		/// <remarks>
		///     <note type="note">
		///         Changes to the database file only takes effect when calling <see cref="InitializeDatabase" />.
		///     </note>
		/// </remarks>
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

		public List<string> UpgradeScriptChain { get; private set; }

		public List<string> VersionScriptChain { get; private set; }

		private SQLiteConnectionStringBuilder CachedConnectionStringBuilder { get; set; }

		#endregion




		#region Instance Events

		public event Func<string, DateTime, string> PrepareScript;

		#endregion




		#region Instance Methods

		/// <inheritdoc cref="IDatabaseManager.CreateConnection" />
		public SQLiteConnection CreateConnection (bool readOnly)
		{
			if (this.State != DatabaseState.Ready)
			{
				throw new InvalidOperationException("Cannot create connection when in state " + this.State + ".");
			}

			this.CachedConnectionStringBuilder.ReadOnly = readOnly;
			return new SQLiteConnection(this.CachedConnectionStringBuilder.ConnectionString);
		}

		private void Log (LogLevel severity, string format, params object[] args)
		{
			LogLocator.Log(severity, this.GetType().Name, format, args);
		}

		private string OnPrepareScript (string script, DateTime now)
		{
			Func<string, DateTime, string> handler = this.PrepareScript;
			if (handler != null)
			{
				return handler(script, now);
			}
			return script;
		}

		private void RegisterCollations ()
		{
			TrimmedCaseInsensitiveInvariantSQLiteCollation.Register();
			TrimmedCaseInsensitiveCurrentSQLiteCollation.Register();
		}

		private void RegisterFunctions ()
		{
			//TODO: ToNullIfEmptyOrNullFunction
			IsNullOrEmptySQLiteFunction.Register();
			RegularExpressionSQLiteFunction.Register();
		}

		private void SetState (DatabaseState state)
		{
			this.Log(LogLevel.Information, "Setting database state: {0} @ {1}", state, state == DatabaseState.Ready ? this.CachedConnectionStringBuilder.ConnectionString : this.ConnectionStringBuilder.ConnectionString);
			this.State = state;

			if (this.State == DatabaseState.Uninitialized)
			{
				this.CurrentVersion = -1;
				this.MinVersion = -1;
				this.MaxVersion = -1;
			}

			if (this.State != DatabaseState.Ready)
			{
				this.CachedConnectionStringBuilder = null;
			}

			//TODO: Create and dispose repository?
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
		public bool SupportsReadOnlyConnections => true;

		/// <inheritdoc />
		public void CleanupDatabase ()
		{
			if (this.State != DatabaseState.Ready)
			{
				throw new InvalidOperationException("Cannot cleanup database when in state " + this.State + ".");
			}

			this.Log(LogLevel.Information, "Cleaning up database: {0}", this.CachedConnectionStringBuilder.ConnectionString);

			//TODO: Cleanup database (from script file, Vacuum, Analyze, Reindex)
		}

		/// <inheritdoc />
		DbConnection IDatabaseManager.CreateConnection (bool readOnly)
		{
			return this.CreateConnection(readOnly);
		}

		/// <inheritdoc />
		void IDisposable.Dispose ()
		{
			this.UnloadDatabase();
		}

		/// <inheritdoc />
		public void InitializeDatabase ()
		{
			this.SetState(DatabaseState.Uninitialized);

			this.MaxVersion = this.UpgradeScriptChain.Count;

			bool databaseFileCreated = this.DatabaseFile.CreateIfNotExist();
			if (databaseFileCreated)
			{
				this.Log(LogLevel.Information, "Database file created because it did not exist: {0}", this.DatabaseFile);
			}

			//TODO: Detect database type

			this.Log(LogLevel.Debug, "Begin detecting database version");
			try
			{
				using (SQLiteConnection connection = this.CreateConnection(true))
				{
					connection.Open();
					if (databaseFileCreated)
					{
						this.CurrentVersion = 0;
					}
					else
					{
						int result = 0;
						DateTime now = DateTime.Now;
						//TODO: Move into script files
						List<string> commandChain = new List<string>
						{
							"",
							"",
							"",
							"",
							"",
							"",
							"",
							"",
							""
						};
						foreach (string commandText in commandChain)
						{
							using (SQLiteCommand command = connection.CreateCommand())
							{
								string usedCommandText = this.OnPrepareScript(commandText, now);
								command.CommandText = usedCommandText;
								command.CommandType = CommandType.Text;
								this.Log(LogLevel.Debug, "Executing SQL (detecting database version):{0}{1}", Environment.NewLine, usedCommandText);
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
					}
					connection.Close();
				}
			}
			catch (SQLiteException exception)
			{
				this.Log(LogLevel.Error, "SQLite exception while detecting database version: {0}", exception.ToDetailedString());
				this.SetState(DatabaseState.DamagedOrInvalid);
			}
			this.Log(LogLevel.Debug, "Finished detecting database version: {0}", this.CurrentVersion);

			if (this.State == DatabaseState.Uninitialized)
			{
				if (this.CurrentVersion == 0)
				{
					this.SetState(DatabaseState.New);
				}
				else if (this.CurrentVersion < this.MaxVersion)
				{
					this.SetState(DatabaseState.Old);
				}
				else if (this.CurrentVersion > this.MaxVersion)
				{
					this.SetState(DatabaseState.TooNew);
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

			this.Log(LogLevel.Debug, "Begin upgrading database version: {0} -> {1}", this.CurrentVersion, this.MaxVersion);
			try
			{
				//TODO: Create backup
				using (SQLiteConnection connection = this.CreateConnection(false))
				{
					connection.Open();
					DateTime now = DateTime.Now;
					for (int i1 = this.CurrentVersion; i1 < this.MaxVersion; i1++)
					{
						this.Log(LogLevel.Debug, "Upgrading database version: {0} -> {1}", i1, i1 + 1);
						using (SQLiteCommand command = connection.CreateCommand())
						{
							string commandText = this.UpgradeScriptChain[i1];
							string usedCommandText = this.OnPrepareScript(commandText, now);
							command.CommandText = usedCommandText;
							command.CommandType = CommandType.Text;
							this.Log(LogLevel.Debug, "Executing SQL (upgrading database version):{0}{1}", Environment.NewLine, usedCommandText);
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
