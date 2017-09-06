using System;
using System.Collections.Generic;
using System.IO;




namespace RI.Framework.Services.Backup
{
	/// <summary>
	/// Defines the interface for a backup-aware object.
	/// </summary>
	/// <remarks>
	/// <para>
	/// See <see cref="IBackupService"/> for more details.
	/// </para>
	/// <para>
	/// The backup procedure implemented by the backup service works as follows:
	/// <see cref="BeginBackup"/> is called for all backup-aware objects known to the backup service.
	/// If one or more <see cref="BeginBackup"/> calls return false, the backup is canceled and <see cref="EndBackup"/> is called for all backup-aware objects with the <c>performed</c> parameter set to false.
	/// If all <see cref="BeginBackup"/> calls return true, the backup is performed by calling <see cref="Backup"/> on all backup-aware objects, followed by calling <see cref="EndBackup"/> with the <c>performed</c> parameter set to true on all backup-aware objects.
	/// </para>
	/// <para>
	/// The restore procedure implemented by the backup service works as follows:
	/// <see cref="BeginRestore"/> is called for all backup-aware objects known to the backup service.
	/// If one or more <see cref="BeginRestore"/> calls return false, the restore is canceled and <see cref="EndRestore"/> is called for all backup-aware objects with the <c>performed</c> parameter set to false.
	/// If all <see cref="BeginRestore"/> calls return true, the restore is performed by calling <see cref="Restore"/> on all backup-aware objects, followed by calling <see cref="EndRestore"/> with the <c>performed</c> parameter set to true on all backup-aware objects.
	/// </para>
	/// <note type="important">
	/// <see cref="BeginBackup"/>/<see cref="EndBackup"/> and <see cref="BeginRestore"/>/<see cref="EndRestore"/> are also used to check whether backups/restores are possible (<see cref="IBackupService.CanDoBackup"/>, <see cref="IBackupService.CanDoFullBackup"/>, <see cref="IBackupService.CanDoRestore"/>, <see cref="IBackupService.CanDoFullRestore"/>).
	/// </note>
	/// </remarks>
	public interface IBackupAware
	{
		/// <summary>
		/// Queries a backup-aware object for all its supported inclusions.
		/// </summary>
		/// <param name="inclusions">The list of inclusions to be filled by the backup-aware object.</param>
		/// <param name="backupService">The backup service.</param>
		/// <remarks>
		/// <note type="implement">
		/// <paramref name="inclusions"/> is a list used to collect the inclusions for all backup-aware objects.
		/// You should only add to the list but not remove.
		/// </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"><paramref name="inclusions"/> is null.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="backupService"/> is null.</exception>
		void QueryBackupInclusions (IList<IBackupInclusion> inclusions, IBackupService backupService);

		/// <summary>
		/// Informs that a backup is about to be performed.
		/// </summary>
		/// <param name="inclusions">The inclusions to be backed-up.</param>
		/// <returns>
		/// true if the backup-aware object is in a state where a backup can be performed, false otherwise.
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="inclusions"/> is null.</exception>
		bool BeginBackup (IList<IBackupInclusion> inclusions);

		/// <summary>
		/// Performs the actual backup.
		/// </summary>
		/// <param name="streamResolver">The stream resolver used to store data in the bakcup set.</param>
		/// <param name="inclusions">The inclusions to be backed-up.</param>
		/// <exception cref="ArgumentNullException"><paramref name="streamResolver"/> or <paramref name="inclusions"/> is null.</exception>
		void Backup (Func<string, Stream> streamResolver, IList<IBackupInclusion> inclusions);

		/// <summary>
		/// Informs that a backup has been finished.
		/// </summary>
		/// <param name="performed">Indicates whether the backup was actually performed.</param>
		void EndBackup (bool performed);

		/// <summary>
		/// Informs that a restore is about to be performed.
		/// </summary>
		/// <param name="inclusions">The inclusions to be restored.</param>
		/// <returns>
		/// true if the backup-aware object is in a state where a restore can be performed, false otherwise.
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="inclusions"/> is null.</exception>
		bool BeginRestore (IList<IBackupInclusion> inclusions);

		/// <summary>
		/// Performs the actual restore.
		/// </summary>
		/// <param name="streamResolver">The stream resolver used to retrieve data from the backup set.</param>
		/// <param name="inclusions">The inclusions to be restored.</param>
		/// <exception cref="ArgumentNullException"><paramref name="streamResolver"/> or <paramref name="inclusions"/> is null.</exception>
		void Restore (Func<string, Stream> streamResolver, IList<IBackupInclusion> inclusions);

		/// <summary>
		/// Informs that a restore has been finished.
		/// </summary>
		/// <param name="performed">Indicates whether the restore was actually performed.</param>
		void EndRestore (bool performed);
	}
}
