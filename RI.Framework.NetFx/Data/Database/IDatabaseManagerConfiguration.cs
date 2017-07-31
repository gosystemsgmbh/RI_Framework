using System.Data.Common;

using RI.Framework.Data.Database.Backup;
using RI.Framework.Data.Database.Cleanup;
using RI.Framework.Data.Database.Upgrading;
using RI.Framework.Data.Database.Versioning;
using RI.Framework.Utilities.Logging;

namespace RI.Framework.Data.Database
{
	public interface IDatabaseManagerConfiguration : ILogSource
	{
		DbConnectionStringBuilder ConnectionString { get; }

		IDatabaseVersionDetector VersionDetector { get; }

		IDatabaseVersionUpgrader VersionUpgrader { get; }

		IDatabaseBackupCreator BackupCreator { get; }

		IDatabaseCleanupProcessor CleanupProcessor { get; }

		void InheritLogger ();

		void VerifyConfiguration ();
	}

	public interface IDatabaseManagerConfiguration<TConnection, TConnectionStringBuilder, TManager> : IDatabaseManagerConfiguration
		where TConnection : DbConnection
		where TConnectionStringBuilder : DbConnectionStringBuilder
		where TManager : IDatabaseManager<TConnection, TConnectionStringBuilder, TManager>
	{
		new TConnectionStringBuilder ConnectionString { get; set; }

		new IDatabaseVersionDetector<TConnection, TConnectionStringBuilder, TManager> VersionDetector { get; set; }

		new IDatabaseVersionUpgrader<TConnection, TConnectionStringBuilder, TManager> VersionUpgrader { get; set; }

		new IDatabaseBackupCreator<TConnection, TConnectionStringBuilder, TManager> BackupCreator { get; set; }

		new IDatabaseCleanupProcessor<TConnection, TConnectionStringBuilder, TManager> CleanupProcessor { get; set; }
	}
}
