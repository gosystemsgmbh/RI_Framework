using System;
using System.Data.SQLite;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;

using RI.Framework.IO.Paths;
using RI.Framework.Services.Logging;
using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;




namespace RI.Framework.Data.SQLite
{
	/// <summary>
	///     Represents an SQLite database file and provides database file management functionality not provided by ADO.NET or Entity Framework.
	/// </summary>
	/// <remarks>
	///     <note type="note">
	///         An instance of <see cref="SQLiteFile" /> can be reused by changing <see cref="ConnectionStringBuilder" />, <see cref="ConnectionString" />, <see cref="File" />, and/or <see cref="VersionDetector" />.
	///         However, changes made while the SQLite file is initialized or open will not be automatically applied, closing and re-initializing/re-opening is necessary.
	///     </note>
	///     <note type="important">
	///         <see cref="SQLiteFile" /> is intended to have exactly one long-living connection (<see cref="Connection" />) as long as the SQLite file is open.
	///         Furthermore, it is intended for single-threading use, so <see cref="SQLiteFile" /> and its created connection can only be used from the thread which created the instance of <see cref="SQLiteFile" />.
	///         If the database connection (<see cref="Connection" />) must be used on another thread, use <see cref="SQLiteConnection.Clone" /> to clone the database connection.
	///         However, any cloned connection is not managed nor tracked by <see cref="SQLiteFile" /> and might therefore lead to conflicts if the cloned database connection lives longer than the one managed by <see cref="SQLiteFile" />.
	///     </note>
	/// </remarks>
	public sealed class SQLiteFile : IDisposable
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="SQLiteFile" />.
		/// </summary>
		public SQLiteFile ()
			: this(new SQLiteConnectionStringBuilder())
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="SQLiteFile" />.
		/// </summary>
		/// <param name="file"> The path to the database file. </param>
		/// <exception cref="InvalidPathArgumentException"> <paramref name="file" /> is not null and contains wildcards. </exception>
		public SQLiteFile (FilePath file)
			: this(new SQLiteConnectionStringBuilder
			       {
				       DataSource = file
			       })
		{
			if (file != null)
			{
				if (file.HasWildcards)
				{
					throw new InvalidPathArgumentException(nameof(file));
				}
			}
		}

		/// <summary>
		///     Creates a new instance of <see cref="SQLiteFile" />.
		/// </summary>
		/// <param name="connectionString"> The database connection string. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="connectionString" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="connectionString" /> is an empty string. </exception>
		public SQLiteFile (string connectionString)
			: this(new SQLiteConnectionStringBuilder(connectionString))
		{
			if (connectionString == null)
			{
				throw new ArgumentNullException(nameof(connectionString));
			}

			if (connectionString.IsEmpty())
			{
				throw new EmptyStringArgumentException(nameof(connectionString));
			}
		}

		/// <summary>
		///     Creates a new instance of <see cref="SQLiteFile" />.
		/// </summary>
		/// <param name="connectionStringBuilder"> The database connection string builder. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="connectionStringBuilder" /> is null. </exception>
		public SQLiteFile (SQLiteConnectionStringBuilder connectionStringBuilder)
		{
			if (connectionStringBuilder == null)
			{
				throw new ArgumentNullException(nameof(connectionStringBuilder));
			}

			this.ConnectionStringBuilder = connectionStringBuilder;

			this.EnableLogging = false;
			this.State = SQLiteFileState.Closed;
			this.UsedFile = null;
			this.UsedConnectionString = null;
			this.Version = null;
			this.Connection = null;

			GC.KeepAlive(this);
		}

		/// <summary>
		///     Garbage collects this instance of <see cref="SQLiteFile" />.
		/// </summary>
		~SQLiteFile ()
		{
			this.Dispose(false);
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets the current database connection.
		/// </summary>
		/// <value>
		///     The current database connection or null if the SQLite file is not open.
		/// </value>
		public SQLiteConnection Connection { get; private set; }

		/// <summary>
		///     Gets or sets the current database connection string.
		/// </summary>
		/// <value>
		///     The current database connection string.
		/// </value>
		public string ConnectionString
		{
			get
			{
				return this.ConnectionStringBuilder.ConnectionString;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException(nameof(value));
				}

				if (value.IsEmpty())
				{
					throw new EmptyStringArgumentException(nameof(value));
				}

				this.ConnectionStringBuilder.ConnectionString = value;
			}
		}

		/// <summary>
		///     Gets the current database connection string builder.
		/// </summary>
		/// <value>
		///     The current database connection string builder.
		/// </value>
		public SQLiteConnectionStringBuilder ConnectionStringBuilder { get; private set; }

		/// <summary>
		///     Gets or sets whether this instance of <see cref="SQLiteFile" /> uses logging or not.
		/// </summary>
		/// <value>
		///     true if logging is used, false otherwise.
		/// </value>
		/// <remarks>
		///     <para>
		///         The default value is false.
		///     </para>
		///     <para>
		///         <see cref="LogLocator" /> is used for logging.
		///     </para>
		/// </remarks>
		public bool EnableLogging { get; private set; }

		/// <summary>
		///     Gets or sets the current database file.
		/// </summary>
		/// <value>
		///     The current database file.
		/// </value>
		public FilePath File
		{
			get
			{
				string dataSource = this.ConnectionStringBuilder.DataSource;
				return dataSource == null ? null : new FilePath(dataSource);
			}
			set
			{
				if (value != null)
				{
					if (value.HasWildcards)
					{
						throw new InvalidPathArgumentException(nameof(value));
					}
				}

				this.ConnectionStringBuilder.DataSource = value;
			}
		}

		/// <summary>
		///     Gets the current state of the SQLite file.
		/// </summary>
		/// <value>
		///     The current state of the SQLite file.
		/// </value>
		public SQLiteFileState State { get; private set; }

		/// <summary>
		///     Gets the detected version of the database.
		/// </summary>
		/// <value>
		///     The detected version of the database or null if no version was detected.
		/// </value>
		/// <remarks>
		///     <para>
		///         The version of the database is detected on calling <see cref="Initialize" /> if <see cref="VersionDetector" /> is set.
		///         If no version detector is set or the version cannot be determined, the value is reset to null by <see cref="Initialize" />.
		///     </para>
		///     <para>
		///         A database version of zero can be used to indicate a new or empty database.
		///     </para>
		/// </remarks>
		public int? Version { get; private set; }

		/// <summary>
		///     Gets or sets the used version detector to detect the version of the database.
		/// </summary>
		/// <value>
		///     The used version detector to detect the version of the database.
		/// </value>
		/// <remarks>
		///     <para>
		///         See <see cref="Version" /> for more details.
		///     </para>
		/// </remarks>
		public ISQLiteFileVersionDetector VersionDetector { get; set; }

		private string UsedConnectionString { get; set; }

		private FilePath UsedFile { get; set; }

		#endregion




		#region Instance Methods

		/// <summary>
		///     Closes the database connection and the SQLite file.
		/// </summary>
		public void Close ()
		{
			this.Dispose(true);
		}

		/// <summary>
		///     Initializes the SQLite file.
		/// </summary>
		/// <exception cref="InvalidOperationException"> The SQLite file is not closed. </exception>
		public void Initialize ()
		{
			if (this.State != SQLiteFileState.Closed)
			{
				throw new InvalidOperationException("SQLite file not closed.");
			}

			this.UsedFile = this.File;
			this.UsedConnectionString = this.ConnectionString;
			this.Version = null;

			this.Log("Initializing SQLite file: {0}; {1}", this.UsedFile, this.UsedConnectionString);

			bool newFile = this.File.CreateIfNotExist();
			this.Log(newFile ? "Used new SQLite file: {0}" : "Used existing SQLite file: {0}", this.File);

			this.Log("Used SQLite file version detector: {0}", this.VersionDetector?.GetType().Name ?? "[null]");
			if (this.VersionDetector != null)
			{
				SQLiteConnectionStringBuilder temporaryBuilder = new SQLiteConnectionStringBuilder(this.UsedConnectionString);
				temporaryBuilder.ReadOnly = true;
				using (SQLiteConnection temporaryConnection = new SQLiteConnection(temporaryBuilder.ConnectionString))
				{
					temporaryConnection.Open();
					this.Version = this.VersionDetector.DetectVersion(temporaryConnection, newFile, this.File);
					temporaryConnection.Close();
				}
			}
			this.Log("Detected SQLite file version: {0}", this.Version?.ToString("D", CultureInfo.InvariantCulture) ?? "[null]");

			this.Connection = null;

			this.State = SQLiteFileState.Initialized;
		}

		/// <summary>
		///     Migrates the database version to the maximum target version supported by a migration chain.
		/// </summary>
		/// <param name="migrationChain"> The migration chain to use. </param>
		/// <param name="restoreFileOnFail"> Specifies whether a backup should be made to restore the database after a failed migration to its state before the migration. </param>
		/// <returns>
		///     true if the migration was successful, false otherwise.
		/// </returns>
		/// <remarks>
		///     <para>
		///         If <paramref name="restoreFileOnFail" /> is true, a temporary file is used as a backup copy, obtained through <see cref="FilePath" />.<see cref="FilePath.GetTemporaryFile" />.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="migrationChain" /> is null. </exception>
		/// <exception cref="ArgumentException"> <paramref name="migrationChain" /> is not a valid migration chain. </exception>
		/// <exception cref="InvalidOperationException"> The SQLite file is not initialized, the database version was not detected, or <paramref name="migrationChain" /> does not support the version of this database. </exception>
		/// <exception cref="FileNotFoundException"> The SQLite file was deleted after initialization. </exception>
		/// <exception cref="IOException"> Creation or restore of the backup copy failed. </exception>
		[SuppressMessage ("ReSharper", "PossibleInvalidOperationException")]
		public bool Migrate (SQLiteFileMigrationChain migrationChain, bool restoreFileOnFail)
		{
			if (migrationChain == null)
			{
				throw new ArgumentNullException(nameof(migrationChain));
			}

			if (!migrationChain.IsValid)
			{
				throw new ArgumentException("The SQLite file migration chain is invalid", nameof(migrationChain));
			}

			if (this.State != SQLiteFileState.Initialized)
			{
				throw new InvalidOperationException("SQLite file not initialized.");
			}

			if (!this.Version.HasValue)
			{
				throw new InvalidOperationException("SQLite database version not detected.");
			}

			if (this.Version.Value < migrationChain.MinSourceVersion.Value)
			{
				throw new InvalidOperationException("The SQLite file migration chain does not support this database version.");
			}

			if (!this.UsedFile.Exists)
			{
				throw new FileNotFoundException("SQLite file not found.", this.UsedFile);
			}

			this.Log("Migrating SQLite database: {0} -> {1} ({2}; {3}; {4})", this.Version.Value, migrationChain.MaxTargetVersion.Value, restoreFileOnFail ? "restore file on fail" : "do not restore file on fail", this.UsedFile, this.UsedConnectionString);

			bool result = false;
			FilePath backupFile = restoreFileOnFail ? FilePath.GetTemporaryFile() : null;
			try
			{
				if (backupFile != null)
				{
					this.Log("Creating backup of SQLite file before migration: {0} -> {1}", this.UsedFile, backupFile);
					if (!this.UsedFile.Copy(backupFile, true))
					{
						throw new IOException(string.Format("Failed to create backup of SQLite file: {0} -> {1}", this.UsedFile, backupFile));
					}
				}

				SQLiteConnectionStringBuilder temporaryBuilder = new SQLiteConnectionStringBuilder(this.UsedConnectionString);
				using (SQLiteConnection temporaryConnection = new SQLiteConnection(temporaryBuilder.ConnectionString))
				{
					temporaryConnection.Open();
					result = migrationChain.Execute(temporaryConnection);
					temporaryConnection.Close();
				}
			}
			finally
			{
				if ((backupFile != null) && backupFile.Exists)
				{
					if (!result)
					{
						this.Log("Restoring backup of SQLite file after failed migration: {0} -> {1}", backupFile, this.UsedFile);
						if (!backupFile.Copy(this.UsedFile, true))
						{
							throw new IOException(string.Format("Failed to restore backup of SQLite file: {0} -> {1}", backupFile, this.UsedFile));
						}
					}

					this.Log("Deleting backup of SQLite file after migration: {0}", backupFile);
					backupFile.Delete();
				}
			}

			this.Log("Migrated SQLite database: {0}", result ? "Success" : "Failed");

			return result;
		}

		/// <summary>
		///     Opens the SQLite file and the database connection.
		/// </summary>
		/// <exception cref="InvalidOperationException"> The SQLite file is not initialized. </exception>
		/// <exception cref="FileNotFoundException"> The SQLite file was deleted after initialization. </exception>
		public void Open ()
		{
			if (this.State != SQLiteFileState.Initialized)
			{
				throw new InvalidOperationException("SQLite file not initialized.");
			}

			if (!this.UsedFile.Exists)
			{
				throw new FileNotFoundException("SQLite file not found.", this.UsedFile);
			}

			this.Log("Opening SQLite file: {0}; {1}", this.UsedFile, this.UsedConnectionString);

			this.Connection = new SQLiteConnection(this.UsedConnectionString);

			this.State = SQLiteFileState.Open;
		}

		[SuppressMessage ("ReSharper", "UnusedParameter.Local")]
		private void Dispose (bool disposing)
		{
			this.State = SQLiteFileState.Closed;

			if (this.Connection != null)
			{
				this.Log("Closing SQLite file: {0}; {1}", this.UsedFile, this.UsedConnectionString);

				this.Connection.Close();
				this.Connection = null;
			}

			this.UsedFile = null;
			this.UsedConnectionString = null;
			this.Version = null;
		}

		private void Log (string format, params object[] args)
		{
			if (this.EnableLogging)
			{
				LogLocator.LogDebug(nameof(SQLiteFile), format, args);
			}
		}

		#endregion




		#region Interface: IDisposable

		/// <inheritdoc />
		void IDisposable.Dispose ()
		{
			this.Close();
		}

		#endregion
	}
}
