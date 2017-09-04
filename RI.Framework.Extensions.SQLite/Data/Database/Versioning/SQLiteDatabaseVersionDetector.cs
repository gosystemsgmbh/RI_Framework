﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics.CodeAnalysis;

using RI.Framework.Data.SQLite;
using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;
using RI.Framework.Utilities.Logging;

namespace RI.Framework.Data.Database.Versioning
{
	/// <summary>
	/// Implements a database version detector for SQLite databases.
	/// </summary>
	/// <remarks>
	/// <para>
	/// <see cref="SQLiteDatabaseVersionDetector"/> uses a custom SQL script which is loaded through a script locator using its script name.
	/// </para>
	/// <para>
	/// The script must return a scalar value which indicates the current version of the database.
	/// The script must return -1 to indicate when the database is damaged or in an invalid state or 0 to indicate that the database does not yet exist and needs to be created.
	/// </para>
	/// <para>
	/// The version detection fails if the script contains more than one batch.
	/// </para>
	/// </remarks>
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	public sealed class SQLiteDatabaseVersionDetector : DatabaseVersionDetector<SQLiteConnection, SQLiteTransaction, SQLiteConnectionStringBuilder, SQLiteDatabaseManager, SQLiteDatabaseManagerConfiguration>
	{
		/// <summary>
		/// Creates a new instance of <see cref="SQLiteDatabaseVersionDetector"/>.
		/// </summary>
		/// <param name="scriptName">The name of the script which performs the version detection.</param>
		/// <exception cref="ArgumentNullException"><paramref name="scriptName"/> is null.</exception>
		/// <exception cref="EmptyStringArgumentException"><paramref name="scriptName"/> is an empty string.</exception>
		public SQLiteDatabaseVersionDetector(string scriptName)
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
		public override bool Detect (SQLiteDatabaseManager manager, out DatabaseState? state, out int version)
		{
			if (manager == null)
			{
				throw new ArgumentNullException(nameof(manager));
			}

			state = null;
			version = -1;

			try
			{
				SQLiteConnectionStringBuilder connectionString = new SQLiteConnectionStringBuilder(manager.Configuration.ConnectionString.ConnectionString);
				connectionString.ReadOnly = true;

				List<string> batches = manager.GetScriptBatch(this.ScriptName, true);
				if (batches == null)
				{
					throw new Exception("Batch retrieval failed for script: " + (this.ScriptName ?? "[null]"));
				}
				if (batches.Count != 1)
				{
					throw new Exception("Not exactly one batch in script: " + (this.ScriptName ?? "[null]"));
				}

				using (SQLiteConnection connection = new SQLiteConnection(connectionString.ConnectionString))
				{
					connection.Open();

					using (SQLiteTransaction transaction = connection.BeginTransaction(IsolationLevel.Serializable))
					{
						using (SQLiteCommand command = new SQLiteCommand(batches[0], connection, transaction))
						{
							object value = command.ExecuteScalar();
							version = value.Int32FromSQLiteResult() ?? -1;
						}

						transaction?.Rollback();
					}
				}

				return true;
			}
			catch (Exception exception)
			{
				this.Log(LogLevel.Error, "SQLite database version detection failed:{0}{1}", Environment.NewLine, exception.ToDetailedString());
				return false;
			}
		}

		/// <inheritdoc />
		public override bool RequiresScriptLocator => true;
	}
}
