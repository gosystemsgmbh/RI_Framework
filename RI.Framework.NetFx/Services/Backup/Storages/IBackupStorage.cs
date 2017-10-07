using System;
using System.Collections.Generic;
using System.IO;

using RI.Framework.IO.Paths;
using RI.Framework.Utilities.Exceptions;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Services.Backup.Storages
{
	/// <summary>
	///     Defines the interface for a backup storage.
	/// </summary>
	/// <remarks>
	///     <para>
	///         See <see cref="IBackupService" /> for more details.
	///     </para>
	/// </remarks>
	/// <threadsafety static="true" instance="true" />
	public interface IBackupStorage : ISynchronizable
	{
		/// <summary>
		///     Gets whether the backup storage is initialized or not.
		/// </summary>
		/// <value>
		///     true if the backup storage is initialized, false otherwise or after the backup storage was unloaded.
		/// </value>
		bool IsInitialized { get; }

		/// <summary>
		///     Gets whether the backup storage is read-only.
		/// </summary>
		/// <value>
		///     true if the backup storage is read-only, false otherwise.
		/// </value>
		bool IsReadOnly { get; }

		/// <summary>
		///     Performs a cleanup of old backups.
		/// </summary>
		/// <param name="retentionDate"> The date and time from which all older backups are to be cleaned up. </param>
		/// <remarks>
		///     <note type="implement">
		///         If the implemented backup storage does not support cleanup of old backups, this method should do simply nothing.
		///     </note>
		///     <note type="note">
		///         Do not call this method directly, it is intended to be called from an <see cref="IBackupService" /> implementation.
		///     </note>
		/// </remarks>
		void Cleanup (DateTime retentionDate);

		/// <summary>
		///     Finalizes and persists a backup set created by this backup storage.
		/// </summary>
		/// <param name="backupSet"> The backup set to finalize and persist, as returned by <see cref="TryBeginBackup" />. </param>
		/// <remarks>
		///     <note type="note">
		///         Do not call this method directly, it is intended to be called from an <see cref="IBackupService" /> implementation.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="backupSet" /> is null. </exception>
		/// <exception cref="ArgumentException"> <paramref name="backupSet" /> is not a set returned by <see cref="TryBeginBackup" /> of this backup storage. </exception>
		void EndBackup (IBackupSet backupSet);

		/// <summary>
		///     Finishes restore of a backup set restored by this backup storage.
		/// </summary>
		/// <param name="backupSet"> The backup set to finish restore, as returned by <see cref="TryBeginRestore" />. </param>
		/// <remarks>
		///     <note type="note">
		///         Do not call this method directly, it is intended to be called from an <see cref="IBackupService" /> implementation.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="backupSet" /> is null. </exception>
		/// <exception cref="ArgumentException"> <paramref name="backupSet" /> is not a set returned by <see cref="TryBeginRestore" /> of this backup storage. </exception>
		void EndRestore (IBackupSet backupSet);

		/// <summary>
		///     Gets all available backup sets.
		/// </summary>
		/// <returns>
		///     The list of available backup sets.
		///     An empty list is returned if no backup sets are available.
		/// </returns>
		/// <remarks>
		///     <note type="note">
		///         Do not call this method directly, it is intended to be called from an <see cref="IBackupService" /> implementation.
		///     </note>
		/// </remarks>
		List<IBackupSet> GetAvailableSets ();

		/// <summary>
		///     Initializes the backup storage.
		/// </summary>
		/// <remarks>
		///     <note type="note">
		///         Do not call this method directly, it is intended to be called from an <see cref="IBackupService" /> implementation.
		///     </note>
		/// </remarks>
		void Initialize ();

		/// <summary>
		///     Attempts to create a new backup set.
		/// </summary>
		/// <param name="name"> The name of the backup. Can be null. </param>
		/// <param name="timestamp"> The timestamp of the backup. </param>
		/// <param name="inclusions"> The inclusions to be included in the backup. </param>
		/// <param name="backupSet"> Returns the newly created backup set. </param>
		/// <param name="streamResolver"> Returns the stream resolver used to store data in the bakcup set. </param>
		/// <returns>
		///     true if this backup storage has started a new backup set, false otherwise.
		///     This is used by the backup service to determine which backup storage to use when creating a new backup.
		/// </returns>
		/// <remarks>
		///     <note type="note">
		///         Do not call this method directly, it is intended to be called from an <see cref="IBackupService" /> implementation.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="inclusions" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="name" /> is an empty string. </exception>
		/// <exception cref="ArgumentException"> <paramref name="inclusions" /> is an empty sequence. </exception>
		bool TryBeginBackup (string name, DateTime timestamp, IEnumerable<IBackupInclusion> inclusions, out IBackupSet backupSet, out Func<Guid, Stream> streamResolver);

		/// <summary>
		///     Attempts to begin restore of a backup set.
		/// </summary>
		/// <param name="backupSet"> The backup set to restore. </param>
		/// <param name="inclusions"> The inclusions to restore. </param>
		/// <param name="streamResolver"> Returns the stream resolver used to retrieve data from the backup set. </param>
		/// <returns>
		///     true if the backup set was managed by this backup storage and if this backup storage has started restore of the existing backup set, false otherwise.
		///     This is used by the backup service to determine which backup storage to use when restoring an existing backup.
		/// </returns>
		/// <remarks>
		///     <note type="note">
		///         Do not call this method directly, it is intended to be called from an <see cref="IBackupService" /> implementation.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="backupSet" /> or <paramref name="inclusions" /> is null. </exception>
		/// <exception cref="ArgumentException"> <paramref name="inclusions" /> is an empty sequence. </exception>
		bool TryBeginRestore (IBackupSet backupSet, IEnumerable<IBackupInclusion> inclusions, out Func<Guid, Stream> streamResolver);

		/// <summary>
		///     Attempts to delete a backup from the storage.
		/// </summary>
		/// <param name="backupSet"> The backup set to delete. </param>
		/// <returns>
		///     true if the backup set was managed by this backup storage and was deleted, false otherwise (where the backup set belongs to another backup storage).
		///     This is used by the backup service to determine which backup storage to use when deleting a backup.
		/// </returns>
		/// <remarks>
		///     <note type="note">
		///         Do not call this method directly, it is intended to be called from an <see cref="IBackupService" /> implementation.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="backupSet" /> is null. </exception>
		bool TryDeleteBackup (IBackupSet backupSet);

		/// <summary>
		///     Attempst to import a backup set from an existing backup file.
		/// </summary>
		/// <param name="file"> The existing backup file to import. </param>
		/// <returns>
		///     The backup set created and associated with the imported backup file or null if this backup storage could not import the file.
		///     This is used by the backup service to determine which backup storage to use for importing a file.
		/// </returns>
		/// <remarks>
		///     <note type="implement">
		///         Importing a backup file must also include copying the backup content into the appropriate storage where the backup set is associated to.
		///     </note>
		///     <note type="note">
		///         Do not call this method directly, it is intended to be called from an <see cref="IBackupService" /> implementation.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="file" /> is null. </exception>
		/// <exception cref="InvalidPathArgumentException"> <paramref name="file" /> is not a valid file path. </exception>
		/// <exception cref="FileNotFoundException"> <paramref name="file" /> does not exist. </exception>
		IBackupSet TryImportBackupFromFile (FilePath file);

		/// <summary>
		///     Unloads the backup storage.
		/// </summary>
		/// <remarks>
		///     <note type="note">
		///         Do not call this method directly, it is intended to be called from an <see cref="IBackupService" /> implementation.
		///     </note>
		/// </remarks>
		void Unload ();
	}
}
