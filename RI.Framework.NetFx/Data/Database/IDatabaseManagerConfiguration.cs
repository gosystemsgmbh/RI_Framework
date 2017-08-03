using System.Data.Common;

using RI.Framework.Data.Database.Backup;
using RI.Framework.Data.Database.Cleanup;
using RI.Framework.Data.Database.Upgrading;
using RI.Framework.Data.Database.Versioning;
using RI.Framework.Utilities.Logging;

namespace RI.Framework.Data.Database
{
	/// <summary>
	/// Holds the database configuration used by a <see cref="IDatabaseManager"/>.
	/// </summary>
	/// <remarks>
	/// <para>
	/// See <see cref="IDatabaseManager"/> for more details about database managers.
	/// </para>
	/// </remarks>
	public interface IDatabaseManagerConfiguration : ILogSource
	{
		/// <summary>
		/// Gets or sets the connection string builder which is used to build the connection string to access the database.
		/// </summary>
		/// <value>
		/// The connection string builder which is used to build the connection string to access the database.
		/// </value>
		/// <remarks>
		/// <note type="important">
		/// This property is required and must be set before the database manager can be used.
		/// </note>
		/// </remarks>
		DbConnectionStringBuilder ConnectionString { get; }

		/// <summary>
		/// Gets or sets the version detector which is used with the database.
		/// </summary>
		/// <value>
		/// The version detector which is used with the database.
		/// </value>
		/// <remarks>
		/// <note type="important">
		/// This property is required and must be set before the database manager can be used.
		/// </note>
		/// </remarks>
		IDatabaseVersionDetector VersionDetector { get; }

		/// <summary>
		/// Gets or sets the version upgrader to use.
		/// </summary>
		/// <value>
		/// The version upgrader to use or null if upgrading is not required.
		/// </value>
		IDatabaseVersionUpgrader VersionUpgrader { get; }

		/// <summary>
		/// Gets or sets the backup creator to use.
		/// </summary>
		/// <value>
		/// The backup creator to use or null if backup and restore are not required.
		/// </value>
		IDatabaseBackupCreator BackupCreator { get; }

		/// <summary>
		/// Gets or sets the cleanup processor to use.
		/// </summary>
		/// <value>
		/// The cleanup processor to use or null if cleanup is not required.
		/// </value>
		IDatabaseCleanupProcessor CleanupProcessor { get; }

		/// <summary>
		/// Inherits the logger used by <see cref="IDatabaseManagerConfiguration"/> to <see cref="VersionDetector"/>, <see cref="VersionUpgrader"/>, <see cref="BackupCreator"/>, and <see cref="CleanupProcessor"/>.
		/// </summary>
		void InheritLogger ();

		/// <summary>
		/// Verifies the configuration.
		/// </summary>
		/// <exception cref="InvalidDatabaseConfigurationException">The database configuration is not valid.</exception>
		void VerifyConfiguration ();
	}

	/// <inheritdoc cref="IDatabaseManagerConfiguration"/>
	/// <typeparam name="TConnection">The database connection type, subclass of <see cref="DbConnection"/>.</typeparam>
	/// <typeparam name="TConnectionStringBuilder">The connection string builder type, subclass of <see cref="DbConnectionStringBuilder"/>.</typeparam>
	/// <typeparam name="TManager">The type of the database manager which is implementing this interface.</typeparam>
	public interface IDatabaseManagerConfiguration<TConnection, TConnectionStringBuilder, TManager> : IDatabaseManagerConfiguration
		where TConnection : DbConnection
		where TConnectionStringBuilder : DbConnectionStringBuilder
		where TManager : IDatabaseManager<TConnection, TConnectionStringBuilder, TManager>
	{
		/// <inheritdoc cref="IDatabaseManagerConfiguration.ConnectionString"/>
		new TConnectionStringBuilder ConnectionString { get; set; }

		/// <inheritdoc cref="IDatabaseManagerConfiguration.VersionDetector"/>
		new IDatabaseVersionDetector<TConnection, TConnectionStringBuilder, TManager> VersionDetector { get; set; }

		/// <inheritdoc cref="IDatabaseManagerConfiguration.VersionUpgrader"/>
		new IDatabaseVersionUpgrader<TConnection, TConnectionStringBuilder, TManager> VersionUpgrader { get; set; }

		/// <inheritdoc cref="IDatabaseManagerConfiguration.BackupCreator"/>
		new IDatabaseBackupCreator<TConnection, TConnectionStringBuilder, TManager> BackupCreator { get; set; }

		/// <inheritdoc cref="IDatabaseManagerConfiguration.CleanupProcessor"/>
		new IDatabaseCleanupProcessor<TConnection, TConnectionStringBuilder, TManager> CleanupProcessor { get; set; }
	}
}
