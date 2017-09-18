using System;
using System.Collections.Generic;

using RI.Framework.IO.Paths;
using RI.Framework.Services.Backup.Storages;




namespace RI.Framework.Services.Backup
{
	/// <summary>
	///     Provides a centralized and global backup provider.
	/// </summary>
	/// <remarks>
	///     <para>
	///         <see cref="BackupLocator" /> is merely a convenience utility as it uses <see cref="ServiceLocator" /> to retrieve and use a <see cref="IBackupService" />.
	///     </para>
	/// </remarks>
	public static class BackupLocator
	{
		/// <summary>
		///     Gets whether a backup service is available and can be used by <see cref="BackupLocator" />.
		/// </summary>
		/// <value>
		///     true if a backup service is available and can be used by <see cref="BackupLocator" />, false otherwise.
		/// </value>
		public static bool IsAvailable => BackupLocator.Service != null;

		/// <summary>
		///     Gets the available backup service.
		/// </summary>
		/// <value>
		///     The backup service or null if no backup service is available.
		/// </value>
		public static IBackupService Service => ServiceLocator.GetInstance<IBackupService>();

		/// <inheritdoc cref="IBackupService.Storages" />
		public static IEnumerable<IBackupStorage> Storages => BackupLocator.Service?.Storages ?? new IBackupStorage[0];

		/// <inheritdoc cref="IBackupService.Awares" />
		public static IEnumerable<IBackupAware> Awares => BackupLocator.Service?.Awares ?? new IBackupAware[0];

		/// <inheritdoc cref="IBackupService.CanDoBackup" />
		public static bool CanDoBackup (IEnumerable<IBackupInclusion> inclusions) => BackupLocator.Service?.CanDoBackup(inclusions) ?? false;

		/// <inheritdoc cref="IBackupService.CanDoFullBackup" />
		public static bool CanDoFullBackup (bool includeNonRestorables) => BackupLocator.Service?.CanDoFullBackup(includeNonRestorables) ?? false;

		/// <inheritdoc cref="IBackupService.CanDoRestore" />
		public static bool CanDoRestore (IBackupSet backupSet, IEnumerable<IBackupInclusion> inclusions) => BackupLocator.Service?.CanDoRestore(backupSet, inclusions) ?? false;

		/// <inheritdoc cref="IBackupService.CanDoFullRestore" />
		public static bool CanDoFullRestore (IBackupSet backupSet) => BackupLocator.Service?.CanDoFullRestore(backupSet) ?? false;

		/// <inheritdoc cref="IBackupService.CreateBackup(string,IEnumerable{IBackupInclusion})" />
		public static IBackupSet CreateBackup (string name, IEnumerable<IBackupInclusion> inclusions) => BackupLocator.Service?.CreateBackup(name, inclusions);

		/// <inheritdoc cref="IBackupService.CreateBackup(string,DateTime,IEnumerable{IBackupInclusion})" />
		public static IBackupSet CreateBackup (string name, DateTime timestamp, IEnumerable<IBackupInclusion> inclusions) => BackupLocator.Service?.CreateBackup(name, timestamp, inclusions);

		/// <inheritdoc cref="IBackupService.CreateFullBackup(string,bool)" />
		public static IBackupSet CreateFullBackup (string name, bool includeNonRestorables) => BackupLocator.Service?.CreateFullBackup(name, includeNonRestorables);

		/// <inheritdoc cref="IBackupService.CreateFullBackup(string,DateTime,bool)" />
		public static IBackupSet CreateFullBackup (string name, DateTime timestamp, bool includeNonRestorables) => BackupLocator.Service?.CreateFullBackup(name, timestamp, includeNonRestorables);

		/// <inheritdoc cref="IBackupService.RestoreBackup" />
		public static bool RestoreBackup (IBackupSet backupSet, IEnumerable<IBackupInclusion> inclusions) => BackupLocator.Service?.RestoreBackup(backupSet, inclusions) ?? false;

		/// <inheritdoc cref="IBackupService.RestoreFullBackup" />
		public static bool RestoreFullBackup (IBackupSet backupSet) => BackupLocator.Service?.RestoreFullBackup(backupSet) ?? false;

		/// <inheritdoc cref="IBackupService.ImportBackupFromFile" />
		public static IBackupSet ImportBackupFromFile (FilePath file) => BackupLocator.Service?.ImportBackupFromFile(file);

		/// <inheritdoc cref="IBackupService.ExportBackupToFile" />
		public static bool ExportBackupToFile (IBackupSet backupSet, FilePath file) => BackupLocator.Service?.ExportBackupToFile(backupSet, file) ?? false;

		/// <inheritdoc cref="IBackupService.DeleteBackup" />
		public static void DeleteBackup (IBackupSet backupSet) => BackupLocator.Service?.DeleteBackup(backupSet);

		/// <inheritdoc cref="IBackupService.Cleanup(DateTime)" />
		public static void Cleanup (DateTime retentionDate) => BackupLocator.Service?.Cleanup(retentionDate);

		/// <inheritdoc cref="IBackupService.Cleanup(TimeSpan)" />
		public static void Cleanup (TimeSpan retentionTime) => BackupLocator.Service?.Cleanup(retentionTime);

		/// <inheritdoc cref="IBackupService.GetAvailableSets" />
		public static List<IBackupSet> GetAvailableSets () => BackupLocator.Service?.GetAvailableSets() ?? new List<IBackupSet>();

		/// <inheritdoc cref="IBackupService.GetAvailableInclusions" />
		public static List<IBackupInclusion> GetAvailableInclusions () => BackupLocator.Service?.GetAvailableInclusions() ?? new List<IBackupInclusion>();
	}
}
