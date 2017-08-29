using System;
using System.Data.SQLite;

using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;
using RI.Framework.Utilities.Logging;

namespace RI.Framework.Data.Database.Cleanup
{
	public sealed class SQLiteDatabaseCleanupProcessor : DatabaseCleanupProcessor<SQLiteConnection, SQLiteConnectionStringBuilder, SQLiteDatabaseManager>
	{
		/// <summary>
		/// The default cleanup script used when no script name is specified.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The default cleanup script uses <c>VACUUM</c>, <c>ANALYZE</c>, and <c>REINDEX</c>..
		/// </para>
		/// </remarks>
		public const string DefaultCleanupScript = "VACUUM;\r\nGO\r\nANALYZE;\r\nGO\r\nREINDEX;\r\nGO\r\n";

		/// <summary>
		/// Creates a new instance of <see cref="SQLiteDatabaseCleanupProcessor"/>.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The default cleanup script is used (<see cref="DefaultCleanupScript"/>).
		/// </para>
		/// </remarks>
		public SQLiteDatabaseCleanupProcessor ()
		{
			this.ScriptName = null;
		}

		/// <summary>
		/// Creates a new instance of <see cref="SQLiteDatabaseCleanupProcessor"/>.
		/// </summary>
		/// <param name="scriptName">The name of the script which performs the cleanup.</param>
		/// <exception cref="ArgumentNullException"><paramref name="scriptName"/> is null.</exception>
		/// <exception cref="EmptyStringArgumentException"><paramref name="scriptName"/> is an empty string.</exception>
		public SQLiteDatabaseCleanupProcessor (string scriptName)
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
				using (SQLiteConnection connection = manager.CreateConnection(false, false))
				{
					
				}

					this.Log(LogLevel.Information, "Beginning SQLite database backup: [{0}] -> [{1}]", sourceConnectionString.ConnectionString, targetConnectionString.ConnectionString);

				using (SQLiteConnection source = new SQLiteConnection(sourceConnectionString.ConnectionString))
				{
					using (SQLiteConnection target = new SQLiteConnection(targetConnectionString.ConnectionString))
					{
						source.Open();
						target.Open();

						source.BackupDatabase(target, "main", "main", -1, null, 10);
					}
				}

				this.Log(LogLevel.Information, "Finished SQLite database backup: [{0}] -> [{1}]", sourceConnectionString.ConnectionString, targetConnectionString.ConnectionString);

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
