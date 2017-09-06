using System;
using System.Collections.Generic;
using System.IO;

using RI.Framework.Composition.Model;
using RI.Framework.IO.Paths;
using RI.Framework.Services.Backup.Storages;
using RI.Framework.Utilities.Exceptions;




namespace RI.Framework.Services.Backup
{
	/// <summary>
	///     Defines the interface for a backup service.
	/// </summary>
	/// <remarks>
	///     <para>
	///         A backup service is used to backup and restore application-specific data.
	///     </para>
	/// <para>
	/// A single backup, which contains all or some application data and can be restored, is represented by a backup set (<see cref="IBackupSet"/>).
	/// Therefore, a backup set is considered an independent &quot;snapshot&quot; of the applications data at a specified point in time.
	/// </para>
	/// <para>
	/// Each backup set has one or more inclusions (<see cref="IBackupInclusion"/>).
	/// An inclusion is a single content in a backup set which is backed-up and restored independently from the other inclusions.
	/// An inclusion might be non-restorable, for example log files which are usually not restored but can be part of a backup.
	/// So a backup set can contain individual parts of an applications data as independent inclusions (e.g. database, log files, settings, etc., each an independent inclusion handled by different, independent parts of the application).
	/// </para>
	/// <para>
	/// Backup storages (<see cref="IBackupStorage"/>) are used to physically store the backup sets.
	/// If multiple backup storages are available, creating a backup only creates physical data and a backup set for the first backup storage which is not read-only.
	/// </para>
	/// <para>
	/// Because backing-up and restoring data heavily depends on the data itself and how it is accessed, the backup service itself does not create copies of the data which is backed up.
	/// Instead, the backup service provides all the infrastructure to allow individual application parts, which require backup functionality, to implement their own backup and restore procedure.
	/// This is implemented using backup-aware objects (<see cref="IBackupAware"/>).
	/// Anything which requires backup functionality implements <see cref="IBackupAware"/> and makes itself known to the backup service.
	/// The backup service then calls the corresponding methods of the backup-aware objects during backup and restore.
	/// See <see cref="IBackupAware"/> for more details.
	/// </para>
	/// </remarks>
	[Export]
	public interface IBackupService
	{
		/// <summary>
		///     Gets all currently available backup storages.
		/// </summary>
		/// <value>
		///     All currently available backup storages.
		/// </value>
		/// <remarks>
		///     <note type="implement">
		///         The value of this property must never be null.
		///     </note>
		/// </remarks>
		IEnumerable<IBackupStorage> Storages { get; }

		/// <summary>
		///     Gets all currently available backup-aware objects.
		/// </summary>
		/// <value>
		///     All currently available backup-aware objects.
		/// </value>
		/// <remarks>
		///     <note type="implement">
		///         The value of this property must never be null.
		///     </note>
		/// </remarks>
		IEnumerable<IBackupAware> Awares { get; }

		/// <summary>
		///     Adds a backup storage.
		/// </summary>
		/// <param name="backupStorage"> The backup storage to add. </param>
		/// <remarks>
		///     <note type="implement">
		///         Specifying an already added backup storage should have no effect.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="backupStorage" /> is null. </exception>
		void AddStorage(IBackupStorage backupStorage);

		/// <summary>
		///     Adds a backup-aware object.
		/// </summary>
		/// <param name="backupAware"> The backup-aware object to add. </param>
		/// <remarks>
		///     <note type="implement">
		///         Specifying an already added backup-aware object should have no effect.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="backupAware" /> is null. </exception>
		void AddAware(IBackupAware backupAware);

		/// <summary>
		///     Removes a backup storage.
		/// </summary>
		/// <param name="backupStorage"> The backup storage to remove. </param>
		/// <remarks>
		///     <note type="implement">
		///         Specifying an already removed backup storage should have no effect.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="backupStorage" /> is null. </exception>
		void RemoveStorage(IBackupStorage backupStorage);

		/// <summary>
		///     Removes a backup-aware object.
		/// </summary>
		/// <param name="backupAware"> The backup-aware object to remove. </param>
		/// <remarks>
		///     <note type="implement">
		///         Specifying an already removed backup-aware object should have no effect.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="backupAware" /> is null. </exception>
		void RemoveAware(IBackupAware backupAware);

		/// <summary>
		/// Determines whether all backup-aware objects are in a state which allows creating a backup using all inclusions.
		/// </summary>
		/// <param name="includeNonRestorables">Specifies whether inclusions which can not be restored are also included in the backup.</param>
		/// <returns>
		/// true if a full backup can be created, false otherwise.
		/// </returns>
		bool CanDoFullBackup (bool includeNonRestorables);

		/// <summary>
		/// Determines whether all backup-aware objects are in a state which allows creating a backup.
		/// </summary>
		/// <param name="inclusions">The inclusions to be included in the backup. Can be null to include all available inclusions.</param>
		/// <returns>
		/// true if a backup can be created, false otherwise.
		/// </returns>
		/// <exception cref="ArgumentException"><paramref name="inclusions"/> is an empty sequence.</exception>
		bool CanDoBackup (IEnumerable<IBackupInclusion> inclusions);

		/// <summary>
		/// Determines whether all backup-aware objects are in a state which allows restoring a backup using all inclusions.
		/// </summary>
		/// <param name="backupSet">The backup set to restore.</param>
		/// <returns>
		/// true if a full backup can be restored, false otherwise.
		/// </returns>
		bool CanDoFullRestore(IBackupSet backupSet);

		/// <summary>
		/// Determines whether all backup-aware objects are in a state which allows restoring a backup.
		/// </summary>
		/// <param name="backupSet">The backup set to restore.</param>
		/// <param name="inclusions">The inclusions to be included in the restore. Can be null to include all available inclusions.</param>
		/// <returns>
		/// true if a backup can be restored, false otherwise.
		/// </returns>
		/// <exception cref="ArgumentException"><paramref name="inclusions"/> is an empty sequence.</exception>
		bool CanDoRestore(IBackupSet backupSet, IEnumerable<IBackupInclusion> inclusions);

		/// <summary>
		/// Creates a new backup.
		/// </summary>
		/// <param name="name">The name of the backup. Can be null.</param>
		/// <param name="inclusions">The inclusions to be included in the backup. Can be null to include all available inclusions.</param>
		/// <returns>
		/// The created and stored backup set or null if the backup could not be created (one or more of <see cref="IBackupAware"/> returned false during <see cref="IBackupAware.BeginBackup"/>).
		/// </returns>
		/// <remarks>
		/// <para>
		/// <see cref="DateTime.UtcNow"/> is used as the timestamp.
		/// </para>
		/// </remarks>
		/// <exception cref="EmptyStringArgumentException"><paramref name="name"/> is an empty string.</exception>
		/// <exception cref="ArgumentException"><paramref name="inclusions"/> is an empty sequence.</exception>
		IBackupSet CreateBackup (string name, IEnumerable<IBackupInclusion> inclusions);

		/// <summary>
		/// Creates a new backup.
		/// </summary>
		/// <param name="name">The name of the backup. Can be null.</param>
		/// <param name="timestamp">The timestamp of the backup.</param>
		/// <param name="inclusions">The inclusions to be included in the backup. Can be null to include all available inclusions.</param>
		/// <returns>
		/// The created and stored backup set or null if the backup could not be created (one or more of <see cref="IBackupAware"/> returned false during <see cref="IBackupAware.BeginBackup"/>).
		/// </returns>
		/// <exception cref="EmptyStringArgumentException"><paramref name="name"/> is an empty string.</exception>
		/// <exception cref="ArgumentException"><paramref name="inclusions"/> is an empty sequence.</exception>
		IBackupSet CreateBackup (string name, DateTime timestamp, IEnumerable<IBackupInclusion> inclusions);

		/// <summary>
		/// Creates a full backup which contains all inclusions.
		/// </summary>
		/// <param name="name">The name of the backup. Can be null.</param>
		/// <param name="includeNonRestorables">Specifies whether inclusions which can not be restored are also included in the backup.</param>
		/// <returns>
		/// The created and stored backup set or null if the backup could not be created (one or more of <see cref="IBackupAware"/> returned false during <see cref="IBackupAware.BeginBackup"/>).
		/// </returns>
		/// <remarks>
		/// <para>
		/// <see cref="DateTime.UtcNow"/> is used as the timestamp.
		/// </para>
		/// </remarks>
		/// <exception cref="EmptyStringArgumentException"><paramref name="name"/> is an empty string.</exception>
		IBackupSet CreateFullBackup (string name, bool includeNonRestorables);

		/// <summary>
		/// Creates a full backup which contains all inclusions.
		/// </summary>
		/// <param name="name">The name of the backup. Can be null.</param>
		/// <param name="timestamp">The timestamp of the backup.</param>
		/// <param name="includeNonRestorables">Specifies whether inclusions which can not be restored are also included in the backup.</param>
		/// <returns>
		/// The created and stored backup set or null if the backup could not be created (one or more of <see cref="IBackupAware"/> returned false during <see cref="IBackupAware.BeginBackup"/>).
		/// </returns>
		/// <exception cref="EmptyStringArgumentException"><paramref name="name"/> is an empty string.</exception>
		IBackupSet CreateFullBackup (string name, DateTime timestamp, bool includeNonRestorables);

		/// <summary>
		/// Restores an existing backup.
		/// </summary>
		/// <param name="backupSet">The backup set to restore.</param>
		/// <param name="inclusions">The inclusions to restore. Can be null to restore all available inclusions.</param>
		/// <returns>
		/// true if the backup was restored, false if one or more of <see cref="IBackupAware"/> returned false during <see cref="IBackupAware.BeginRestore"/>
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="backupSet"/> is null.</exception>
		/// <exception cref="ArgumentException"><paramref name="inclusions"/> is an empty sequence.</exception>
		/// <exception cref="InvalidOperationException"><paramref name="backupSet"/> does not support restore.</exception>
		bool RestoreBackup (IBackupSet backupSet, IEnumerable<IBackupInclusion> inclusions);

		/// <summary>
		/// Restores an existing backup with all inclusions.
		/// </summary>
		/// <param name="backupSet">The backup set to restore.</param>
		/// <returns>
		/// true if the backup was restored, false if one or more of <see cref="IBackupAware"/> returned false during <see cref="IBackupAware.BeginRestore"/>
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="backupSet"/> is null.</exception>
		/// <exception cref="InvalidOperationException"><paramref name="backupSet"/> does not support restore.</exception>
		bool RestoreFullBackup (IBackupSet backupSet);

		/// <summary>
		/// Deletes a backup from the storage.
		/// </summary>
		/// <param name="backupSet">The backup set to delete.</param>
		/// <exception cref="ArgumentNullException"><paramref name="backupSet"/> is null.</exception>
		void DeleteBackup (IBackupSet backupSet);

		/// <summary>
		/// Imports a backup set from an existing backup file.
		/// </summary>
		/// <param name="file">The existing backup file to import.</param>
		/// <returns>
		/// The backup set created and associated with the imported backup file or null if no backup storage could import the file.
		/// </returns>
		/// <remarks>
		/// <para>
		/// Each available backup storage is tried for importing the file, using <see cref="IBackupStorage.TryImportBackupFromFile"/>, returning the backup set of the first backup storage which does not return null.
		/// </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"><paramref name="file"/> is null.</exception>
		/// <exception cref="InvalidPathArgumentException"><paramref name="file"/> is not a valid file path.</exception>
		/// <exception cref="FileNotFoundException"><paramref name="file"/> does not exist.</exception>
		IBackupSet ImportBackupFromFile (FilePath file);

		/// <summary>
		/// Creates an inclusion.
		/// </summary>
		/// <param name="id">The ID of the inclusion.</param>
		/// <param name="resourceKey">The resource key. Can be null.</param>
		/// <param name="supportsRestore">Specifies whether the inclusion supports restore or not.</param>
		/// <returns>
		/// The newly created inclusion.
		/// </returns>
		/// <exception cref="EmptyStringArgumentException"><paramref name="resourceKey"/> is an empty string.</exception>
		IBackupInclusion CreateInclusion (Guid id, string resourceKey, bool supportsRestore);

		/// <summary>
		/// Gets all available backups found in the backup storages.
		/// </summary>
		/// <returns>
		/// The list of available backup sets.
		/// An empty list is returned if no backups are available.
		/// </returns>
		List<IBackupSet> GetAvailableBackups ();

		/// <summary>
		/// Gets all available inclusions queried from the backup-aware objects.
		/// </summary>
		/// <returns>
		/// The list of available inclusions.
		/// An empty list is returned if no inclusions are available.
		/// </returns>
		List<IBackupInclusion> GetAvailableInclusions ();
	}
}
