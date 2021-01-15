﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;

using RI.Framework.Data.Database.Cleanup;
using RI.Framework.Data.Database.Upgrading;
using RI.Framework.Utilities.Logging;




namespace RI.Framework.Data.Database
{
    /// <summary>
    ///     Implements a single SQL Server database processing step.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         <see cref="SqlServerDatabaseProcessingStep" /> is used by <see cref="SqlServerDatabaseCleanupProcessor" /> and <see cref="SqlServerDatabaseVersionUpgrader" />.
    ///     </para>
    ///     <para>
    ///         See <see cref="DatabaseProcessingStep{TConnection,TTransaction,TConnectionStringBuilder,TManager,TConfiguration}" /> for more details.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    public class SqlServerDatabaseProcessingStep : DatabaseProcessingStep<SqlConnection, SqlTransaction, SqlConnectionStringBuilder, SqlServerDatabaseManager, SqlServerDatabaseManagerConfiguration>
    {
        #region Overrides

        /// <inheritdoc />
        protected override void ExecuteBatchesImpl (List<string> batches, SqlServerDatabaseManager manager, SqlConnection connection, SqlTransaction transaction)
        {
            foreach (string batch in batches)
            {
                this.Log(LogLevel.Debug, "Executing SQL Server database processing command:{0}{1}", Environment.NewLine, batch);
                using (SqlCommand command = transaction == null ? new SqlCommand(batch, connection) : new SqlCommand(batch, connection, transaction))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        #endregion
    }
}
