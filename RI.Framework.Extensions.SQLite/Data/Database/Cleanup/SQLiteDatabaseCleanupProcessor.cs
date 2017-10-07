using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics.CodeAnalysis;

using RI.Framework.Data.Database.Scripts;
using RI.Framework.Utilities;
using RI.Framework.Utilities.Logging;




namespace RI.Framework.Data.Database.Cleanup
{
	/// <summary>
	///     Implements a database cleanup processor for SQLite databases.
	/// </summary>
	/// <remarks>
	///     <para>
	///         <see cref="SQLiteDatabaseCleanupProcessor" /> can be used with either a default SQLite cleanup script (<see cref="DefaultCleanupScript" />) or with a custom processing step.
	///     </para>
	/// </remarks>
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	public sealed class SQLiteDatabaseCleanupProcessor : DatabaseCleanupProcessor<SQLiteConnection, SQLiteTransaction, SQLiteConnectionStringBuilder, SQLiteDatabaseManager, SQLiteDatabaseManagerConfiguration>
	{
		#region Constants

		/// <summary>
		///     The default cleanup script used when no custom processing step is specified.
		/// </summary>
		/// <remarks>
		///     <para>
		///         The default cleanup script uses <c> VACUUM </c>, <c> ANALYZE </c>, and <c> REINDEX </c>, each executed as a single command.
		///     </para>
		/// </remarks>
		public const string DefaultCleanupScript = "VACUUM;" + DatabaseScriptLocator.DefaultBatchSeparator + "ANALYZE;" + DatabaseScriptLocator.DefaultBatchSeparator + "REINDEX;" + DatabaseScriptLocator.DefaultBatchSeparator;

		#endregion




		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="SQLiteDatabaseCleanupProcessor" />.
		/// </summary>
		/// <remarks>
		///     <para>
		///         The default cleanup script is used (<see cref="DefaultCleanupScript" />).
		///     </para>
		/// </remarks>
		public SQLiteDatabaseCleanupProcessor ()
			: this(null)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="SQLiteDatabaseCleanupProcessor" />.
		/// </summary>
		/// <param name="cleanupStep"> The custom processing step which performs the cleanup or null if the default cleanup script is used (<see cref="DefaultCleanupScript" />). </param>
		public SQLiteDatabaseCleanupProcessor (SQLiteDatabaseProcessingStep cleanupStep)
		{
			this.CleanupStep = cleanupStep;
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets the custom processing step which performs the cleanup.
		/// </summary>
		/// <value>
		///     The custom processing step which performs the cleanup or null if the default cleanup script is used (<see cref="DefaultCleanupScript" />).
		/// </value>
		public SQLiteDatabaseProcessingStep CleanupStep { get; }

		#endregion




		#region Overrides

		/// <inheritdoc />
		public override bool RequiresScriptLocator => this.CleanupStep?.RequiresScriptLocator ?? false;

		/// <inheritdoc />
		public override bool Cleanup (SQLiteDatabaseManager manager)
		{
			if (manager == null)
			{
				throw new ArgumentNullException(nameof(manager));
			}

			try
			{
				SQLiteConnectionStringBuilder connectionString = new SQLiteConnectionStringBuilder(manager.Configuration.ConnectionString.ConnectionString);

				this.Log(LogLevel.Information, "Beginning SQLite database cleanup: Connection=[{0}]", connectionString.ConnectionString);

				using (SQLiteConnection connection = new SQLiteConnection(connectionString.ConnectionString))
				{
					connection.Open();

					SQLiteDatabaseProcessingStep cleanupStep = this.CleanupStep;
					if (cleanupStep == null)
					{
						List<string> batches = DatabaseScriptLocator.SplitBatches(SQLiteDatabaseCleanupProcessor.DefaultCleanupScript, DatabaseScriptLocator.DefaultBatchSeparator);
						cleanupStep = new SQLiteDatabaseProcessingStep();
						cleanupStep.AddBatches(batches);
					}

					using (SQLiteTransaction transaction = cleanupStep.RequiresTransaction ? connection.BeginTransaction(IsolationLevel.Serializable) : null)
					{
						cleanupStep.Execute(manager, connection, transaction);
						transaction?.Commit();
					}
				}

				this.Log(LogLevel.Information, "Finished SQLite database cleanup: Connection=[{0}]", connectionString.ConnectionString);

				return true;
			}
			catch (Exception exception)
			{
				this.Log(LogLevel.Error, "SQLite database cleanup failed:{0}{1}", Environment.NewLine, exception.ToDetailedString());
				return false;
			}
		}

		#endregion
	}
}
