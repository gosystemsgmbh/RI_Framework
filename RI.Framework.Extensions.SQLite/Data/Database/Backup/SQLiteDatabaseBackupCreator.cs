using System;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics.CodeAnalysis;
using System.IO;

using RI.Framework.IO.Files;
using RI.Framework.IO.Paths;
using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;
using RI.Framework.Utilities.Logging;

namespace RI.Framework.Data.Database.Backup
{
	/// <summary>
	/// Implements a database backup creator for SQLite databases.
	/// </summary>
	/// <remarks>
	/// <para>
	/// <see cref="SQLiteDatabaseBackupCreator"/> performs a backup using the SQLite online backup API.
	/// It also supports additional pre-processing and post-processing steps which are executed on the source database.
	/// </para>
	/// <para>
	/// <see cref="SQLiteDatabaseBackupCreator"/> supports the following types for backup targets and sources:
	/// <see cref="FilePath"/> (backup to/from SQLite database file using this file path),
	/// <see cref="string"/> or <see cref="SQLiteConnectionStringBuilder"/> (backup to/from SQLite database using this connection string),
	/// <see cref="SQLiteConnection"/> (backup to/from SQLite database using this connection),
	/// <see cref="Stream"/> (backup to/from a stream which contains a SQLite database).
	/// </para>
	/// <para>
	/// <see cref="SQLiteDatabaseBackupCreator"/> does not support restore.
	/// </para>
	/// </remarks>
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	public sealed class SQLiteDatabaseBackupCreator : DatabaseBackupCreator<SQLiteConnection, SQLiteTransaction, SQLiteConnectionStringBuilder, SQLiteDatabaseManager, SQLiteDatabaseManagerConfiguration>
	{
		/// <summary>
		/// Creates a new instance of <see cref="SQLiteDatabaseBackupCreator"/>.
		/// </summary>
		/// <remarks>
		/// <para>
		/// No pre-processing or post-processing is used.
		/// </para>
		/// </remarks>
		public SQLiteDatabaseBackupCreator()
			: this(null, null)
		{
		}

		/// <summary>
		/// Creates a new instance of <see cref="SQLiteDatabaseBackupCreator"/>.
		/// </summary>
		/// <param name="preprocessingStep">The pre-processing step executed before the backup is created or null if no pre-processing step is used.</param>
		/// <param name="postprocessingStep">The post-processing step executed after the backup was created or null if no post-processing step is used.</param>
		public SQLiteDatabaseBackupCreator(SQLiteDatabaseProcessingStep preprocessingStep, SQLiteDatabaseProcessingStep postprocessingStep)
		{
			this.PreprocessingStep = preprocessingStep;
			this.PostprocessingStep = postprocessingStep;
		}

		/// <summary>
		/// Gets the used pre-processing step.
		/// </summary>
		/// <value>
		/// The used pre-processing step or null if no pre-processing is used.
		/// </value>
		public SQLiteDatabaseProcessingStep PreprocessingStep { get; }

		/// <summary>
		/// Gets the used post-processing step.
		/// </summary>
		/// <value>
		/// The used post-processing step or null if no post-processing is used.
		/// </value>
		public SQLiteDatabaseProcessingStep PostprocessingStep { get; }

		/// <inheritdoc />
		public override bool RequiresScriptLocator => (this.PreprocessingStep?.RequiresScriptLocator ?? false) || (this.PostprocessingStep?.RequiresScriptLocator ?? false);

		/// <inheritdoc />
		public override bool SupportsRestore => false;

		/// <inheritdoc />
		public override bool Backup (SQLiteDatabaseManager manager, object backupTarget)
		{
			if (manager == null)
			{
				throw new ArgumentNullException(nameof(manager));
			}

			if (backupTarget == null)
			{
				throw new ArgumentNullException(nameof(backupTarget));
			}

			FilePath backupFile = backupTarget as FilePath;
			string connectionString = backupTarget as string;
			SQLiteConnectionStringBuilder connectionStringBuilder = backupTarget as SQLiteConnectionStringBuilder;
			SQLiteConnection connection = backupTarget as SQLiteConnection;
			Stream stream = backupTarget as Stream;

			if ((backupFile == null) && (connectionString == null) && (connectionStringBuilder == null) && (connection == null) && (stream == null))
			{
				throw new InvalidTypeArgumentException(nameof(backupTarget), nameof(SQLiteDatabaseBackupCreator) + " does not support " + backupTarget.GetType().Name + " as backup target.");
			}

			try
			{
				TemporaryFile tempFile = null;

				try
				{
					SQLiteConnectionStringBuilder sourceConnectionString = new SQLiteConnectionStringBuilder(manager.Configuration.ConnectionString.ConnectionString);

					SQLiteConnectionStringBuilder targetConnectionString = connectionStringBuilder ?? new SQLiteConnectionStringBuilder(connectionString ?? manager.Configuration.ConnectionString.ConnectionString);
					if (backupFile != null)
					{
						targetConnectionString.DataSource = backupFile.PathResolved;
					}
					if (stream != null)
					{
						tempFile = new TemporaryFile();
						targetConnectionString.DataSource = tempFile.File.PathResolved;
					}

					this.Log(LogLevel.Information, "Beginning SQLite database backup: Source=[{0}]; Target=[{1}]", sourceConnectionString.ConnectionString, backupTarget);

					using (SQLiteConnection source = new SQLiteConnection(sourceConnectionString.ConnectionString))
					{
						source.Open();

						if (this.PreprocessingStep != null)
						{
							using (SQLiteTransaction transaction = this.PreprocessingStep.RequiresTransaction ? source.BeginTransaction(IsolationLevel.Serializable) : null)
							{
								this.PreprocessingStep.Execute(manager, source, transaction);
								transaction?.Commit();
							}
						}

						using (SQLiteConnection target = connection ?? new SQLiteConnection(targetConnectionString.ConnectionString))
						{
							if (target.State != ConnectionState.Open)
							{
								target.Open();
							}

							string sourceDatabaseName = "main";
							string targetDatabaseName = "main";

							this.Log(LogLevel.Debug, "Executing SQLite database backup: {0} -> {1}", sourceDatabaseName, targetDatabaseName);
							source.BackupDatabase(target, targetDatabaseName, sourceDatabaseName, -1, null, 10);
						}

						if (stream != null)
						{
							using (FileStream fs = new FileStream(tempFile.File, FileMode.Open, FileAccess.Read, FileShare.Read))
							{
								fs.CopyTo(stream);
							}
						}

						if (this.PostprocessingStep != null)
						{
							using (SQLiteTransaction transaction = this.PostprocessingStep.RequiresTransaction ? source.BeginTransaction(IsolationLevel.Serializable) : null)
							{
								this.PostprocessingStep.Execute(manager, source, transaction);
								transaction?.Commit();
							}
						}
					}

					this.Log(LogLevel.Information, "Finished SQLite database backup: Source=[{0}]; Target=[{1}]", sourceConnectionString.ConnectionString, backupTarget);

					return true;
				}
				finally
				{
					tempFile?.Delete();
				}
			}
			catch (Exception exception)
			{
				this.Log(LogLevel.Error, "SQLite database backup failed:{0}{1}", Environment.NewLine, exception.ToDetailedString());
				return false;
			}
		}

		/// <inheritdoc />
		public override bool Restore (SQLiteDatabaseManager manager, object backupSource)
		{
			throw new NotSupportedException(nameof(SQLiteDatabaseBackupCreator) + "does not support restore.");
		}
	}
}
