using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics.CodeAnalysis;

using RI.Framework.Data.Database.Scripts;
using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;
using RI.Framework.Utilities.Logging;

namespace RI.Framework.Data.Database.Cleanup
{
	/// <summary>
	/// Implements a database cleanup processor for SQLite databases.
	/// </summary>
	/// <remarks>
	/// <para>
	/// <see cref="SQLiteDatabaseCleanupProcessor"/> can be used with either a default SQLite cleanup script (<see cref="DefaultCleanupScript"/>) or with a custom script which is loaded through a script locator using its script name.
	/// <see cref="RequiresScriptLocator"/> returns false if the default cleanup script is used, true if a custom script is used.
	/// </para>
	/// </remarks>
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	public sealed class SQLiteDatabaseCleanupProcessor : DatabaseCleanupProcessor<SQLiteConnection, SQLiteConnectionStringBuilder, SQLiteDatabaseManager>
	{
		/// <summary>
		/// The default cleanup script used when no script name is specified.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The default cleanup script uses <c>VACUUM</c>, <c>ANALYZE</c>, and <c>REINDEX</c>, each executed as a single command.
		/// </para>
		/// </remarks>
		public const string DefaultCleanupScript = "VACUUM;" + DatabaseScriptLocator.DefaultBatchSeparator + "ANALYZE;" + DatabaseScriptLocator.DefaultBatchSeparator + "REINDEX;" + DatabaseScriptLocator.DefaultBatchSeparator;

		/// <summary>
		/// Creates a new instance of <see cref="SQLiteDatabaseCleanupProcessor"/>.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The default cleanup script is used (<see cref="DefaultCleanupScript"/>).
		/// </para>
		/// </remarks>
		public SQLiteDatabaseCleanupProcessor ()
			: this(null)
		{
		}

		/// <summary>
		/// Creates a new instance of <see cref="SQLiteDatabaseCleanupProcessor"/>.
		/// </summary>
		/// <param name="scriptName">The name of the script which performs the cleanup or null if the default cleanup script is used (<see cref="DefaultCleanupScript"/>).</param>
		/// <exception cref="EmptyStringArgumentException"><paramref name="scriptName"/> is an empty string.</exception>
		public SQLiteDatabaseCleanupProcessor (string scriptName)
		{
			if (scriptName != null)
			{
				if (scriptName.IsNullOrEmptyOrWhitespace())
				{
					throw new EmptyStringArgumentException(nameof(scriptName));
				}
			}

			this.ScriptName = scriptName;
		}

		/// <summary>
		/// Gets the name of the script which performs the cleanup.
		/// </summary>
		/// <value>
		/// The name of the script which performs the cleanup or null if the default cleanup script is used.
		/// </value>
		public string ScriptName { get; }

		/// <inheritdoc />
		public override bool Cleanup (SQLiteDatabaseManager manager)
		{
			if (manager == null)
			{
				throw new ArgumentNullException(nameof(manager));
			}

			try
			{
				string connectionString = manager.Configuration.ConnectionString.ConnectionString;

				this.Log(LogLevel.Information, "Beginning SQLite database cleanup: Script=[{0}]; Connection=[{1}]", this.ScriptName ?? "null", connectionString);

				List<string> batches = this.ScriptName == null ? DatabaseScriptLocator.SplitBatches(SQLiteDatabaseCleanupProcessor.DefaultCleanupScript, DatabaseScriptLocator.DefaultBatchSeparator) : manager.GetScriptBatch(this.ScriptName, true);
				if (batches == null)
				{
					throw new Exception("Batch retrieval failed for script: " + (this.ScriptName ?? "[null]"));
				}

				using (SQLiteConnection connection = manager.CreateConnection(false, false))
				{
					foreach (string batch in batches)
					{
						using (SQLiteCommand command = new SQLiteCommand(batch, connection))
						{
							this.Log(LogLevel.Debug, "Executing SQLite database cleanup batch: {0}", batch);
							command.ExecuteNonQuery();
						}
					}
				}

				this.Log(LogLevel.Information, "Finished SQLite database cleanup: Script=[{0}]; Connection=[{1}]", this.ScriptName ?? "null", connectionString);

				return true;
			}
			catch (Exception exception)
			{
				this.Log(LogLevel.Error, "SQLite database cleanup failed:{0}{1}", Environment.NewLine, exception.ToDetailedString());
				return false;
			}
		}

		/// <inheritdoc />
		public override bool RequiresScriptLocator => this.ScriptName != null;
	}
}
