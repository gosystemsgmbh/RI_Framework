using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

using RI.Framework.Data.SqlServer;
using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;
using RI.Framework.Utilities.Logging;

namespace RI.Framework.Data.Database.Versioning
{
	/// <summary>
	/// Implements a database version detector for SQL Server databases.
	/// </summary>
	/// <remarks>
	/// <para>
	/// <see cref="SqlServerDatabaseVersionDetector"/> uses a custom SQL script which is loaded through a script locator using its script name.
	/// </para>
	/// <para>
	/// The script must return a scalar value which indicates the current version of the database.
	/// The script must return -1 to indicate when the database is damaged or in an invalid state or 0 to indicate that the database does not yet exist and needs to be created.
	/// </para>
	/// <para>
	/// The version detection fails if the script contains more than one batch.
	/// </para>
	/// </remarks>
	public sealed class SqlServerDatabaseVersionDetector : DatabaseVersionDetector<SqlConnection, SqlTransaction, SqlConnectionStringBuilder, SqlServerDatabaseManager, SqlServerDatabaseManagerConfiguration>
	{
		/// <summary>
		/// Creates a new instance of <see cref="SqlServerDatabaseVersionDetector"/>.
		/// </summary>
		/// <param name="scriptName">The name of the script which performs the version detection.</param>
		/// <exception cref="ArgumentNullException"><paramref name="scriptName"/> is null.</exception>
		/// <exception cref="EmptyStringArgumentException"><paramref name="scriptName"/> is an empty string.</exception>
		public SqlServerDatabaseVersionDetector(string scriptName)
		{
			if (scriptName == null)
			{
				throw new ArgumentNullException(nameof(scriptName));
			}

			if (scriptName.IsNullOrEmptyOrWhitespace())
			{
				throw new EmptyStringArgumentException(nameof(scriptName));
			}

			this.ScriptName = scriptName;
		}

		/// <summary>
		/// Gets the name of the script which performs the version detection.
		/// </summary>
		/// <value>
		/// The name of the script which performs the version detection.
		/// </value>
		public string ScriptName { get; }

		/// <inheritdoc />
		public override bool Detect(SqlServerDatabaseManager manager, out DatabaseState? state, out int version)
		{
			if (manager == null)
			{
				throw new ArgumentNullException(nameof(manager));
			}

			state = null;
			version = -1;

			try
			{
				SqlConnectionStringBuilder connectionString = new SqlConnectionStringBuilder(manager.Configuration.ConnectionString.ConnectionString);

				List<string> batches = manager.GetScriptBatch(this.ScriptName, true);
				if (batches == null)
				{
					throw new Exception("Batch retrieval failed for script: " + (this.ScriptName ?? "[null]"));
				}
				if (batches.Count != 1)
				{
					throw new Exception("Not exactly one batch in script: " + (this.ScriptName ?? "[null]"));
				}

				using (SqlConnection connection = new SqlConnection(connectionString.ConnectionString))
				{
					connection.Open();

					using (SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.Serializable))
					{
						using (SqlCommand command = new SqlCommand(batches[0], connection, transaction))
						{
							object value = command.ExecuteScalar();
							version = value.Int32FromSqlServerResult() ?? -1;
						}

						transaction.Rollback();
					}
				}

				return true;
			}
			catch (Exception exception)
			{
				this.Log(LogLevel.Error, "SQL Server database version detection failed:{0}{1}", Environment.NewLine, exception.ToDetailedString());
				return false;
			}
		}

		/// <inheritdoc />
		public override bool RequiresScriptLocator => true;
	}
}
