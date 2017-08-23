using System.Data.Common;

using RI.Framework.IO.Paths;
using RI.Framework.Utilities.Logging;

namespace RI.Framework.Data.Database.Backup
{
	/// <summary>
	/// Defines the interface for database backup creators.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Database backup creators are used to create backups of databases and, optionally, restore them.
	/// </para>
	/// <para>
	/// Database backup creators are used by database managers (<see cref="IDatabaseManager"/> implementations).
	/// Do not use database backup creators directly but rather configure to use them through configuration (<see cref="IDatabaseManagerConfiguration"/>.<see cref="IDatabaseManagerConfiguration.BackupCreator"/>).
	/// </para>
	/// <para>
	/// Implementations of <see cref="IDatabaseBackupCreator"/> are always specific for a particular type of database (or particular implementation of <see cref="IDatabaseManager"/> respectively).
	/// </para>
	/// <para>
	/// Database backup creators are optional.
	/// If not configured, backup and restore are not available / not supported.
	/// </para>
	/// <para>
	/// A database backup creator must lways support creating backup.
	/// But support for restore is optional, indicated through <see cref="SupportsRestore"/>.
	/// </para>
	/// </remarks>
	public interface IDatabaseBackupCreator : ILogSource
	{
		/// <summary>
		/// Gets whether this database backup creator requires a script locator.
		/// </summary>
		/// <value>
		/// true if a script locator is required, false otherwise.
		/// </value>
		bool RequiresScriptLocator { get; }

		/// <summary>
		/// Gets whether this database backup creator supports restore.
		/// </summary>
		/// <value>
		/// true if restore is supported, false otherwise.
		/// </value>
		bool SupportsRestore { get; }

		/// <summary>
		/// Creates a backup of a database to a specific file.
		/// </summary>
		/// <param name="manager">The used database manager representing the database.</param>
		/// <param name="backupFile">The file to which the backup is written.</param>
		/// <returns>
		/// true if the backup was successful, false otherwise.
		/// Details must be written to the log.
		/// </returns>
		bool Backup (IDatabaseManager manager, FilePath backupFile);

		/// <summary>
		/// Restores a backup of a database from a specific file.
		/// </summary>
		/// <param name="manager">The used database manager representing the database.</param>
		/// <param name="backupFile">The file from which the backup is restored.</param>
		/// <returns>
		/// true if the restore was successful, false otherwise.
		/// Details must be written to the log.
		/// </returns>
		bool Restore (IDatabaseManager manager, FilePath backupFile);
	}

	/// <inheritdoc cref="IDatabaseBackupCreator"/>
	/// <typeparam name="TConnection">The database connection type, subclass of <see cref="DbConnection"/>.</typeparam>
	/// <typeparam name="TConnectionStringBuilder">The connection string builder type, subclass of <see cref="DbConnectionStringBuilder"/>.</typeparam>
	/// <typeparam name="TManager">The type of the database manager which is using this backup creator.</typeparam>
	public interface IDatabaseBackupCreator<TConnection, TConnectionStringBuilder, in TManager> : IDatabaseBackupCreator
		where TConnection : DbConnection
		where TConnectionStringBuilder : DbConnectionStringBuilder
		where TManager : IDatabaseManager<TConnection, TConnectionStringBuilder, TManager>
	{
		/// <inheritdoc cref="IDatabaseBackupCreator.Backup"/>
		bool Backup (TManager manager, FilePath backupFile);

		/// <inheritdoc cref="IDatabaseBackupCreator.Restore"/>
		bool Restore (TManager manager, FilePath backupFile);
	}
}
