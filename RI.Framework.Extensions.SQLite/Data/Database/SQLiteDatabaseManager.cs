using System.Data.SQLite;
using System.Diagnostics.CodeAnalysis;

using RI.Framework.Data.SQLite;
using RI.Framework.Data.SQLite.Collations;
using RI.Framework.Data.SQLite.Functions;
using RI.Framework.IO.Paths;




namespace RI.Framework.Data.Database
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
	public sealed class SQLiteDatabaseManager : DatabaseManager<SQLiteConnection, SQLiteTransaction, SQLiteConnectionStringBuilder, SQLiteDatabaseManager, SQLiteDatabaseManagerConfiguration>
	{
		#region Instance Properties/Indexer

		/// <summary>
		///     Gets the path to the database file.
		/// </summary>
		/// <value>
		///     The path to the database file.
		/// </value>
		public FilePath DatabaseFile => this.Configuration.DatabaseFile;

		#endregion




		#region Instance Methods

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

		#endregion




		#region Overrides

		/// <inheritdoc />
		protected override bool SupportsBackupImpl => true;

		/// <inheritdoc />
		protected override bool SupportsCleanupImpl => true;

		/// <inheritdoc />
		protected override bool SupportsConnectionTrackingImpl => true;

		/// <inheritdoc />
		protected override bool SupportsReadOnlyImpl => true;

		/// <inheritdoc />
		protected override bool SupportsRestoreImpl => true;

		/// <inheritdoc />
		protected override bool SupportsScriptsImpl => true;

		/// <inheritdoc />
		protected override bool SupportsUpgradeImpl => true;

		/// <inheritdoc />
		protected override SQLiteConnection CreateConnectionImpl (bool readOnly)
		{
			SQLiteConnectionStringBuilder connectionString = new SQLiteConnectionStringBuilder(this.Configuration.ConnectionString.ConnectionString);
			connectionString.ReadOnly = readOnly;

			SQLiteConnection connection = new SQLiteConnection(connectionString.ConnectionString);
			connection.Open();

			if (this.Configuration.RegisterDefaultCollations)
			{
				this.RegisterCollations(connection);
			}

			if (this.Configuration.RegisterDefaultFunctions)
			{
				this.RegisterFunctions(connection);
			}

			return connection;
		}

		/// <inheritdoc />
		protected override IDatabaseProcessingStep<SQLiteConnection, SQLiteTransaction, SQLiteConnectionStringBuilder, SQLiteDatabaseManager, SQLiteDatabaseManagerConfiguration> CreateProcessingStepImpl ()
		{
			return new SQLiteDatabaseProcessingStep();
		}

		/// <inheritdoc />
		protected override bool DetectStateAndVersionImpl (out DatabaseState? state, out int version)
		{
			if (!this.DatabaseFile.Exists)
			{
				state = null;
				version = 0;
				return true;
			}

			return base.DetectStateAndVersionImpl(out state, out version);
		}

		#endregion
	}
}
