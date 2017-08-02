using System.Data.Common;

using RI.Framework.Data.Database.Backup;
using RI.Framework.Data.Database.Cleanup;
using RI.Framework.Data.Database.Upgrading;
using RI.Framework.Data.Database.Versioning;
using RI.Framework.Utilities.Logging;

namespace RI.Framework.Data.Database
{
	public abstract class DatabaseManagerConfiguration<TConnection, TConnectionStringBuilder, TManager> : IDatabaseManagerConfiguration<TConnection, TConnectionStringBuilder, TManager>
		where TConnection : DbConnection
		where TConnectionStringBuilder : DbConnectionStringBuilder
		where TManager : IDatabaseManager<TConnection, TConnectionStringBuilder, TManager>
	{
		private IDatabaseVersionDetector<TConnection, TConnectionStringBuilder, TManager> _versionDetector;

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

		private TConnectionStringBuilder _connectionString;

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

		IDatabaseVersionDetector IDatabaseManagerConfiguration.VersionDetector => this.VersionDetector;

		IDatabaseVersionUpgrader IDatabaseManagerConfiguration.VersionUpgrader => this.VersionUpgrader;

		IDatabaseBackupCreator IDatabaseManagerConfiguration.BackupCreator => this.BackupCreator;

		IDatabaseCleanupProcessor IDatabaseManagerConfiguration.CleanupProcessor => this.CleanupProcessor;

		DbConnectionStringBuilder IDatabaseManagerConfiguration.ConnectionString => this.ConnectionString;

		private bool _loggingEnabled;

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
		}

		void IDatabaseManagerConfiguration.InheritLogger()
		{
			this.InheritLogger();
		}

		protected virtual void VerifyConfiguration()
		{
			if (this.ConnectionString == null)
			{
				throw new InvalidDatabaseConfigurationException("No connection string specified.");
			}

			if (this.VersionDetector == null)
			{
				throw new InvalidDatabaseConfigurationException("No version detector specified.");
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

		void IDatabaseManagerConfiguration.VerifyConfiguration()
		{
			this.VerifyConfiguration();
		}
	}
}
