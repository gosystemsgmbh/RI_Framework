using System.Data.Common;

using RI.Framework.Data.Database.Backup;
using RI.Framework.Data.Database.Cleanup;
using RI.Framework.Data.Database.Scripts;
using RI.Framework.Data.Database.Upgrading;
using RI.Framework.Data.Database.Versioning;
using RI.Framework.Utilities.Logging;

namespace RI.Framework.Data.Database
{
	/// <summary>
	/// Implements a base class for database manager configurations.
	/// </summary>
	/// <typeparam name="TConnection">The database connection type, subclass of <see cref="DbConnection"/>.</typeparam>
	/// <typeparam name="TConnectionStringBuilder">The connection string builder type, subclass of <see cref="DbConnectionStringBuilder"/>.</typeparam>
	/// <typeparam name="TManager">The type of the database manager which is using this configuration.</typeparam>
	/// <remarks>
	/// <para>
	/// It is recommended that database manager configuration implementations use this base class as it already implements most of the logic which is database-independent.
	/// </para>
	/// <para>
	/// See <see cref="IDatabaseManagerConfiguration"/> for more details.
	/// </para>
	/// </remarks>
	public abstract class DatabaseManagerConfiguration<TConnection, TConnectionStringBuilder, TManager> : IDatabaseManagerConfiguration<TConnection, TConnectionStringBuilder, TManager>
		where TConnection : DbConnection
		where TConnectionStringBuilder : DbConnectionStringBuilder
		where TManager : IDatabaseManager<TConnection, TConnectionStringBuilder, TManager>
	{
		private IDatabaseVersionDetector<TConnection, TConnectionStringBuilder, TManager> _versionDetector;

		/// <inheritdoc />
		public IDatabaseVersionDetector<TConnection, TConnectionStringBuilder, TManager> VersionDetector
		{
			get
			{
				return this._versionDetector;
			}
			set
			{
				this._versionDetector = value;

				this.InheritLogger();
			}
		}

		private IDatabaseVersionUpgrader<TConnection, TConnectionStringBuilder, TManager> _versionUpgrader;

		/// <inheritdoc />
		public IDatabaseVersionUpgrader<TConnection, TConnectionStringBuilder, TManager> VersionUpgrader
		{
			get
			{
				return this._versionUpgrader;
			}
			set
			{
				this._versionUpgrader = value;

				this.InheritLogger();
			}
		}

		private IDatabaseBackupCreator<TConnection, TConnectionStringBuilder, TManager> _backupCreator;

		/// <inheritdoc />
		public IDatabaseBackupCreator<TConnection, TConnectionStringBuilder, TManager> BackupCreator
		{
			get
			{
				return this._backupCreator;
			}
			set
			{
				this._backupCreator = value;

				this.InheritLogger();
			}
		}

		private IDatabaseCleanupProcessor<TConnection, TConnectionStringBuilder, TManager> _cleanupProcessor;

		/// <inheritdoc />
		public IDatabaseCleanupProcessor<TConnection, TConnectionStringBuilder, TManager> CleanupProcessor
		{
			get
			{
				return this._cleanupProcessor;
			}
			set
			{
				this._cleanupProcessor = value;

				this.InheritLogger();
			}
		}

		private IDatabaseScriptLocator _scriptLocator;

		/// <inheritdoc />
		public IDatabaseScriptLocator ScriptLocator
		{
			get
			{
				return this._scriptLocator;
			}
			set
			{
				this._scriptLocator = value;

				this.InheritLogger();
			}
		}

		private TConnectionStringBuilder _connectionString;

		/// <inheritdoc />
		public TConnectionStringBuilder ConnectionString
		{
			get
			{
				return this._connectionString;
			}
			set
			{
				this._connectionString = value;
			}
		}

		/// <inheritdoc />
		IDatabaseVersionDetector IDatabaseManagerConfiguration.VersionDetector => this.VersionDetector;

		/// <inheritdoc />
		IDatabaseVersionUpgrader IDatabaseManagerConfiguration.VersionUpgrader => this.VersionUpgrader;

		/// <inheritdoc />
		IDatabaseBackupCreator IDatabaseManagerConfiguration.BackupCreator => this.BackupCreator;

		/// <inheritdoc />
		IDatabaseCleanupProcessor IDatabaseManagerConfiguration.CleanupProcessor => this.CleanupProcessor;

		/// <inheritdoc />
		DbConnectionStringBuilder IDatabaseManagerConfiguration.ConnectionString => this.ConnectionString;

		private bool _loggingEnabled;

		/// <inheritdoc />
		public bool LoggingEnabled
		{
			get
			{
				return this._loggingEnabled;
			}
			set
			{
				this._loggingEnabled = value;

				this.InheritLogger();
			}
		}

		private ILogger _logger;

		/// <inheritdoc />
		public ILogger Logger
		{
			get
			{
				return this._logger;
			}
			set
			{
				this._logger = value;

				this.InheritLogger();
			}
		}

		/// <inheritdoc cref="IDatabaseManagerConfiguration.InheritLogger"/>
		protected virtual void InheritLogger ()
		{
			if (this.VersionDetector != null)
			{
				this.VersionDetector.Logger = this.Logger;
				this.VersionDetector.LoggingEnabled = this.LoggingEnabled;
			}

			if (this.VersionUpgrader != null)
			{
				this.VersionUpgrader.Logger = this.Logger;
				this.VersionUpgrader.LoggingEnabled = this.LoggingEnabled;
			}

			if (this.BackupCreator != null)
			{
				this.BackupCreator.Logger = this.Logger;
				this.BackupCreator.LoggingEnabled = this.LoggingEnabled;
			}

			if (this.CleanupProcessor != null)
			{
				this.CleanupProcessor.Logger = this.Logger;
				this.CleanupProcessor.LoggingEnabled = this.LoggingEnabled;
			}

			if (this.ScriptLocator != null)
			{
				this.ScriptLocator.Logger = this.Logger;
				this.ScriptLocator.LoggingEnabled = this.LoggingEnabled;
			}
		}

		/// <inheritdoc />
		void IDatabaseManagerConfiguration.InheritLogger()
		{
			this.InheritLogger();
		}

		/// <inheritdoc cref="IDatabaseManagerConfiguration.VerifyConfiguration"/>
		protected virtual void VerifyConfiguration()
		{
			if (this.ConnectionString == null)
			{
				throw new InvalidDatabaseConfigurationException("No connection string configured.");
			}

			if (this.ScriptLocator == null)
			{
				if (this.VersionDetector?.RequiresScriptLocator ?? false)
				{
					throw new InvalidDatabaseConfigurationException("Version detector (" + this.VersionDetector.GetType().Name + ") requires script locator but none is configured.");
				}

				if (this.VersionUpgrader?.RequiresScriptLocator ?? false)
				{
					throw new InvalidDatabaseConfigurationException("Version upgrader (" + this.VersionUpgrader.GetType().Name + ") requires script locator but none is configured.");
				}

				if (this.BackupCreator?.RequiresScriptLocator ?? false)
				{
					throw new InvalidDatabaseConfigurationException("Backup creator (" + this.BackupCreator.GetType().Name + ") requires script locator but none is configured.");
				}

				if (this.CleanupProcessor?.RequiresScriptLocator ?? false)
				{
					throw new InvalidDatabaseConfigurationException("Cleanup processor (" + this.CleanupProcessor.GetType().Name + ") requires script locator but none is configured.");
				}
			}

			if (this.VersionDetector == null)
			{
				throw new InvalidDatabaseConfigurationException("No version detector configured.");
			}

			if (this.VersionUpgrader != null)
			{
				if (this.VersionUpgrader.MinVersion < 0)
				{
					throw new InvalidDatabaseConfigurationException("Invalid version upgrader minimum version.");
				}

				if (this.VersionUpgrader.MaxVersion < this.VersionUpgrader.MinVersion)
				{
					throw new InvalidDatabaseConfigurationException("Invalid version upgrader maximum version.");
				}
			}
		}

		/// <inheritdoc />
		void IDatabaseManagerConfiguration.VerifyConfiguration()
		{
			this.VerifyConfiguration();
		}
	}
}
