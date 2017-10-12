﻿using System.Data.SqlClient;




namespace RI.Framework.Data.Database.Upgrading
{
	/// <summary>
	///     Implements an assembly version upgrade step extractor for SQL Server databases.
	/// </summary>
	public sealed class SQLiteAssemblyResourceVersionUpgraderUtility : AssemblyResourceVersionUpgraderUtility<SqlServerDatabaseVersionUpgradeStep, SqlConnection, SqlTransaction, SqlConnectionStringBuilder, SqlServerDatabaseManager, SqlServerDatabaseManagerConfiguration>
	{
		#region Overrides

		/// <inheritdoc />
		protected override SqlServerDatabaseVersionUpgradeStep CreateProcessingStep (int sourceVersion, string resourceName)
		{
			SqlServerDatabaseVersionUpgradeStep upgradeStep = new SqlServerDatabaseVersionUpgradeStep(sourceVersion);
			if (resourceName != null)
			{
				upgradeStep.AddScript(resourceName, DatabaseProcessingStepTransactionRequirement.Required);
			}
			return upgradeStep;
		}

		#endregion
	}
}