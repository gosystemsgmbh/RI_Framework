using System.Data.SqlClient;

namespace RI.Framework.Data.Database
{
	/// <summary>
	///     Implements a database manager for SQL Server databases.
	/// </summary>
	/// <remarks>
	///     <para>
	///         See <see cref="IDatabaseManager" /> for more details.
	///     </para>
	/// </remarks>
	public sealed class SqlServerDatabaseManager : DatabaseManager<SqlConnection, SqlTransaction, SqlConnectionStringBuilder, SqlServerDatabaseManager, SqlServerDatabaseManagerConfiguration>
	{
		/// <inheritdoc />
		protected override bool SupportsConnectionTrackingImpl => true;

		/// <inheritdoc />
		protected override bool SupportsReadOnlyImpl => false;

		/// <inheritdoc />
		protected override bool SupportsScriptsImpl => true;

		/// <inheritdoc />
		protected override bool SupportsUpgradeImpl => true;

		/// <inheritdoc />
		protected override bool SupportsCleanupImpl => true;

		/// <inheritdoc />
		protected override bool SupportsBackupImpl => false;

		/// <inheritdoc />
		protected override bool SupportsRestoreImpl => false;

		/// <inheritdoc />
		protected override SqlConnection CreateConnectionImpl (bool readOnly)
		{
			SqlConnectionStringBuilder connectionString = new SqlConnectionStringBuilder(this.Configuration.ConnectionString.ConnectionString);

			SqlConnection connection = new SqlConnection(connectionString.ConnectionString);
			connection.Open();

			return connection;
		}

		/// <inheritdoc />
		protected override IDatabaseProcessingStep<SqlConnection, SqlTransaction, SqlConnectionStringBuilder, SqlServerDatabaseManager, SqlServerDatabaseManagerConfiguration> CreateProcessingStepImpl ()
		{
			return new SqlServerDatabaseProcessingStep();
		}
	}
}
