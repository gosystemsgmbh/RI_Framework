﻿using System.Data.Common;

using RI.Framework.Utilities.Exceptions;
using RI.Framework.Utilities.Logging;




namespace RI.Framework.Data.Database.Backup
{
	/// <summary>
	///     Defines the interface for database backup creators.
	/// </summary>
	/// <remarks>
	///     <para>
	///         Database backup creators are used to create backups of databases and, optionally, restore them.
	///     </para>
	///     <para>
	///         Database backup creators are used by database managers (<see cref="IDatabaseManager" /> implementations).
	///         Do not use database backup creators directly but rather configure to use them through configuration (<see cref="IDatabaseManagerConfiguration" />.<see cref="IDatabaseManagerConfiguration.BackupCreator" />).
	///     </para>
	///     <para>
	///         Implementations of <see cref="IDatabaseBackupCreator" /> are always specific for a particular type of database (or particular implementation of <see cref="IDatabaseManager" /> respectively).
	///     </para>
	///     <para>
	///         Database backup creators are optional.
	///         If not configured, backup and restore are not available / not supported.
	///     </para>
	///     <para>
	///         A database backup creator must lways support creating backup.
	///         But support for restore is optional, indicated through <see cref="SupportsRestore" />.
	///     </para>
	/// </remarks>
	public interface IDatabaseBackupCreator : ILogSource
	{
		/// <summary>
		///     Gets whether this database backup creator requires a script locator.
		/// </summary>
		/// <value>
		///     true if a script locator is required, false otherwise.
		/// </value>
		bool RequiresScriptLocator { get; }

		/// <summary>
		///     Gets whether this database backup creator supports restore.
		/// </summary>
		/// <value>
		///     true if restore is supported, false otherwise.
		/// </value>
		bool SupportsRestore { get; }

		/// <summary>
		///     Creates a backup of a database to a specific file.
		/// </summary>
		/// <param name="manager"> The used database manager representing the database. </param>
		/// <param name="backupTarget"> The backup creator specific object which describes the backup target. </param>
		/// <returns>
		///     true if the backup was successful, false otherwise.
		///     Details must be written to the log.
		/// </returns>
		/// <exception cref="InvalidTypeArgumentException"> <paramref name="backupTarget" /> is of a type which is not supported by the used backup creator. </exception>
		bool Backup (IDatabaseManager manager, object backupTarget);

		/// <summary>
		///     Restores a backup of a database from a specific file.
		/// </summary>
		/// <param name="manager"> The used database manager representing the database. </param>
		/// <param name="backupSource"> The backup creator specific object which describes the backup source. </param>
		/// <returns>
		///     true if the restore was successful, false otherwise.
		///     Details must be written to the log.
		/// </returns>
		/// <exception cref="InvalidTypeArgumentException"> <paramref name="backupSource" /> is of a type which is not supported by the used backup creator. </exception>
		bool Restore (IDatabaseManager manager, object backupSource);
	}

	/// <inheritdoc cref="IDatabaseBackupCreator" />
	/// <typeparam name="TConnection"> The database connection type, subclass of <see cref="DbConnection" />. </typeparam>
	/// <typeparam name="TTransaction"> The database transaction type, subclass of <see cref="DbTransaction" />. </typeparam>
	/// <typeparam name="TConnectionStringBuilder"> The connection string builder type, subclass of <see cref="DbConnectionStringBuilder" />. </typeparam>
	/// <typeparam name="TManager"> The type of the database manager. </typeparam>
	/// <typeparam name="TConfiguration"> The type of database configuration. </typeparam>
	public interface IDatabaseBackupCreator <TConnection, TTransaction, TConnectionStringBuilder, in TManager, TConfiguration> : IDatabaseBackupCreator
		where TConnection : DbConnection
		where TTransaction : DbTransaction
		where TConnectionStringBuilder : DbConnectionStringBuilder
		where TManager : IDatabaseManager<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration>
		where TConfiguration : class, IDatabaseManagerConfiguration<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration>, new()
	{
		/// <inheritdoc cref="IDatabaseBackupCreator.Backup" />
		bool Backup (TManager manager, object backupTarget);

		/// <inheritdoc cref="IDatabaseBackupCreator.Restore" />
		bool Restore (TManager manager, object backupTarget);
	}
}
