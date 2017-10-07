using System.Data.SqlClient;




namespace RI.Framework.Data.Database.Upgrading
{
	/// <summary>
	///     Implements an assembly version upgrade step configurators for SQL Server databases.
	/// </summary>
	public abstract class SqlServerAssemblyResourceVersionUpgraderStepConfigurator : AssemblyResourceVersionUpgraderStepConfigurator<SqlServerDatabaseVersionUpgradeStep, SqlConnection, SqlTransaction, SqlConnectionStringBuilder, SqlServerDatabaseManager, SqlServerDatabaseManagerConfiguration>
	{
	}
}
