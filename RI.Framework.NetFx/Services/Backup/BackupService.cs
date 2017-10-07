using System;
using System.Collections.Generic;
using System.IO;

using RI.Framework.Collections;
using RI.Framework.Collections.DirectLinq;
using RI.Framework.Composition;
using RI.Framework.Composition.Model;
using RI.Framework.IO.Paths;
using RI.Framework.Services.Backup.Storages;
using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;
using RI.Framework.Utilities.Logging;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Services.Backup
{
	/// <summary>
	///     Implements a default backup service which is suitable for most scenarios.
	/// </summary>
	/// <remarks>
	///     <para>
	///         This backup service manages <see cref="IBackupStorage" />s  and <see cref="IBackupAware" />s from two sources.
	///         One are the explicitly specified storages and awares added through <see cref="AddStorage" /> and <see cref="AddAware" />.
	///         The second is a <see cref="CompositionContainer" /> if this <see cref="BackupService" /> is added as an export (the storages and awares are then imported through composition).
	///         <see cref="Storages" /> gives the sequence containing all backup storages from all sources and <see cref="Awares" /> gives the sequence containing all backup-aware objects from all sources.
	///     </para>
	///     <para>
	///         See <see cref="IBackupService" /> for more details.
	///     </para>
	/// </remarks>
	/// <threadsafety static="true" instance="true" />
	[Export]
	public sealed class BackupService : LogSource, IBackupService, IImporting
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="BackupService" />.
		/// </summary>
		public BackupService ()
		{
			this.SyncRoot = new object();

			this.StoragesUpdated = new List<IBackupStorage>();
			this.AwaresUpdated = new List<IBackupAware>();

			this.StoragesManual = new List<IBackupStorage>();
			this.AwaresManual = new List<IBackupAware>();

			this.AwaresCopy = new List<IBackupAware>();
		}

		#endregion




		#region Instance Properties/Indexer

		private List<IBackupAware> AwaresCopy { get; set; }

		[Import(typeof(IBackupAware), Recomposable = true)]
		private Import AwaresImported { get; set; }

		private List<IBackupAware> AwaresManual { get; }

		private List<IBackupAware> AwaresUpdated { get; set; }

		[Import(typeof(IBackupStorage), Recomposable = true)]
		private Import StoragesImported { get; set; }

		private List<IBackupStorage> StoragesManual { get; }

		private List<IBackupStorage> StoragesUpdated { get; set; }

		#endregion




		#region Instance Methods

		private bool CreateBackupInternal (bool testOnly, string name, DateTime timestamp, List<IBackupInclusion> inclusions, out IBackupSet backupSet)
		{
			List<IBackupAware> awares;
			lock (this.SyncRoot)
			{
				awares = this.AwaresCopy;
			}

			bool canDo = true;

			foreach (IBackupAware aware in awares)
			{
				if (!aware.BeginBackup(inclusions, this))
				{
					canDo = false;
				}
			}

			bool willDo = canDo && (!testOnly);
			IBackupStorage usedStorage = null;
			IBackupSet usedSet = null;
			Func<Guid, Stream> streamResolver = null;

			if (willDo)
			{
				lock (this.SyncRoot)
				{
					foreach (IBackupStorage storage in this.StoragesUpdated)
					{
						if (storage.TryBeginBackup(name, timestamp, inclusions, out usedSet, out streamResolver))
						{
							usedStorage = storage;
							break;
						}
					}

					if (usedStorage == null)
					{
						willDo = false;
					}
				}
			}

			if (willDo)
			{
				foreach (IBackupAware aware in awares)
				{
					aware.Backup(streamResolver, inclusions, this);
				}
			}

			if (usedStorage != null)
			{
				lock (this.SyncRoot)
				{
					usedStorage.EndBackup(usedSet);
				}
			}

			foreach (IBackupAware aware in awares)
			{
				aware.EndBackup(willDo, this);
			}

			backupSet = usedSet;
			return willDo || (canDo && testOnly);
		}

		private bool RestoreBackupInternal (bool testOnly, IBackupSet backupSet, List<IBackupInclusion> inclusions)
		{
			List<IBackupAware> awares;
			lock (this.SyncRoot)
			{
				awares = this.AwaresCopy;
			}

			bool canDo = true;

			foreach (IBackupAware aware in awares)
			{
				if (!aware.BeginRestore(inclusions, this))
				{
					canDo = false;
				}
			}

			bool willDo = canDo && (!testOnly);
			IBackupStorage usedStorage = null;
			Func<Guid, Stream> streamResolver = null;

			if (willDo)
			{
				lock (this.SyncRoot)
				{
					foreach (IBackupStorage storage in this.StoragesUpdated)
					{
						if (storage.TryBeginRestore(backupSet, inclusions, out streamResolver))
						{
							usedStorage = storage;
							break;
						}
					}

					if (usedStorage == null)
					{
						willDo = false;
					}
				}
			}

			if (willDo)
			{
				foreach (IBackupAware aware in awares)
				{
					aware.Restore(streamResolver, inclusions, this);
				}
			}

			if (usedStorage != null)
			{
				lock (this.SyncRoot)
				{
					usedStorage.EndRestore(backupSet);
				}
			}

			foreach (IBackupAware aware in awares)
			{
				aware.EndRestore(willDo, this);
			}

			return willDo || (canDo && testOnly);
		}

		private void UpdateAwares ()
		{
			this.Log(LogLevel.Debug, "Updating awares");

			HashSet<IBackupAware> currentAwares = new HashSet<IBackupAware>(this.Awares);
			HashSet<IBackupAware> lastAwares = new HashSet<IBackupAware>(this.AwaresUpdated);

			HashSet<IBackupAware> newAwares = currentAwares.Except(lastAwares);
			HashSet<IBackupAware> oldAwares = lastAwares.Except(currentAwares);

			this.AwaresUpdated.Clear();
			this.AwaresUpdated.AddRange(currentAwares);

			foreach (IBackupAware aware in newAwares)
			{
				this.Log(LogLevel.Debug, "Aware added: {0}", aware.GetType().Name);
			}

			foreach (IBackupAware aware in oldAwares)
			{
				this.Log(LogLevel.Debug, "Aware removed: {0}", aware.GetType().Name);
			}

			this.AwaresCopy = new List<IBackupAware>(this.AwaresUpdated);
		}

		private void UpdateStorages ()
		{
			this.Log(LogLevel.Debug, "Updating storages");

			HashSet<IBackupStorage> currentStorages = new HashSet<IBackupStorage>(this.Storages);
			HashSet<IBackupStorage> lastStorages = new HashSet<IBackupStorage>(this.StoragesUpdated);

			HashSet<IBackupStorage> newStorages = currentStorages.Except(lastStorages);
			HashSet<IBackupStorage> oldStorages = lastStorages.Except(currentStorages);

			this.StoragesUpdated.Clear();
			this.StoragesUpdated.AddRange(currentStorages);

			foreach (IBackupStorage storage in newStorages)
			{
				this.Log(LogLevel.Debug, "Storage added: {0}", storage.GetType().Name);
			}

			foreach (IBackupStorage storage in oldStorages)
			{
				storage.Unload();

				this.Log(LogLevel.Debug, "Storage removed: {0}", storage.GetType().Name);
			}

			foreach (IBackupStorage storage in currentStorages)
			{
				if (!storage.IsInitialized)
				{
					storage.Initialize();
				}
			}
		}

		#endregion




		#region Interface: IBackupService

		/// <inheritdoc />
		public IEnumerable<IBackupAware> Awares
		{
			get
			{
				lock (this.SyncRoot)
				{
					List<IBackupAware> awares = new List<IBackupAware>();
					awares.AddRange(this.AwaresManual);
					awares.AddRange(this.AwaresImported.Values<IBackupAware>());
					return awares;
				}
			}
		}

		/// <inheritdoc />
		bool ISynchronizable.IsSynchronized => true;

		/// <inheritdoc />
		public IEnumerable<IBackupStorage> Storages
		{
			get
			{
				lock (this.SyncRoot)
				{
					List<IBackupStorage> storages = new List<IBackupStorage>();
					storages.AddRange(this.StoragesManual);
					storages.AddRange(this.StoragesImported.Values<IBackupStorage>());
					return storages;
				}
			}
		}

		/// <inheritdoc />
		public object SyncRoot { get; }

		/// <inheritdoc />
		public void AddAware (IBackupAware backupAware)
		{
			if (backupAware == null)
			{
				throw new ArgumentNullException(nameof(backupAware));
			}

			lock (this.SyncRoot)
			{
				if (this.AwaresManual.Contains(backupAware))
				{
					return;
				}

				this.AwaresManual.Add(backupAware);

				this.UpdateAwares();
			}
		}

		/// <inheritdoc />
		public void AddStorage (IBackupStorage backupStorage)
		{
			if (backupStorage == null)
			{
				throw new ArgumentNullException(nameof(backupStorage));
			}

			lock (this.SyncRoot)
			{
				if (this.StoragesManual.Contains(backupStorage))
				{
					return;
				}

				this.StoragesManual.Add(backupStorage);

				this.UpdateStorages();
			}
		}

		/// <inheritdoc />
		public bool CanDoBackup (IEnumerable<IBackupInclusion> inclusions)
		{
			List<IBackupInclusion> inclusionList = inclusions?.ToList() ?? this.GetAvailableInclusions();
			if (inclusionList.Count == 0)
			{
				throw new ArgumentException("Inclusion sequence is empty.", nameof(inclusions));
			}

			IBackupSet backupSet;
			return this.CreateBackupInternal(true, null, DateTime.Now, inclusionList, out backupSet);
		}

		/// <inheritdoc />
		public bool CanDoFullBackup (bool includeNonRestorables) => this.CanDoBackup(from x in this.GetAvailableInclusions() where x.SupportsRestore || includeNonRestorables select x);

		/// <inheritdoc />
		public bool CanDoFullRestore (IBackupSet backupSet) => this.CanDoRestore(backupSet, null);

		/// <inheritdoc />
		public bool CanDoRestore (IBackupSet backupSet, IEnumerable<IBackupInclusion> inclusions)
		{
			if (backupSet == null)
			{
				throw new ArgumentNullException(nameof(backupSet));
			}

			if (!backupSet.CanRestore)
			{
				return false;
			}

			List<IBackupInclusion> inclusionList = inclusions?.ToList() ?? (from x in backupSet.GetInclusions() where x.SupportsRestore select x).ToList();
			if (inclusionList.Count == 0)
			{
				throw new ArgumentException("Inclusion sequence is empty.", nameof(inclusions));
			}

			foreach (IBackupInclusion inclusion in inclusionList)
			{
				if (!inclusion.SupportsRestore)
				{
					return false;
				}
			}

			return this.RestoreBackupInternal(true, backupSet, inclusionList);
		}

		/// <inheritdoc />
		public void Cleanup (DateTime retentionDate)
		{
			lock (this.SyncRoot)
			{
				foreach (IBackupStorage storage in this.StoragesUpdated)
				{
					storage.Cleanup(retentionDate);
				}
			}
		}

		/// <inheritdoc />
		public void Cleanup (TimeSpan retentionTime)
		{
			if (retentionTime.Ticks <= 0)
			{
				return;
			}

			this.Cleanup(DateTime.Now.Subtract(retentionTime));
		}

		/// <inheritdoc />
		public IBackupSet CreateBackup (string name, IEnumerable<IBackupInclusion> inclusions) => this.CreateBackup(name, DateTime.Now, inclusions);

		/// <inheritdoc />
		public IBackupSet CreateBackup (string name, DateTime timestamp, IEnumerable<IBackupInclusion> inclusions)
		{
			if (name != null)
			{
				if (name.IsNullOrEmptyOrWhitespace())
				{
					throw new EmptyStringArgumentException(nameof(name));
				}
			}

			List<IBackupInclusion> inclusionList = inclusions?.ToList() ?? this.GetAvailableInclusions();
			if (inclusionList.Count == 0)
			{
				throw new ArgumentException("Inclusion sequence is empty.", nameof(inclusions));
			}

			IBackupSet backupSet;
			if (this.CreateBackupInternal(false, name, timestamp, inclusionList, out backupSet))
			{
				return backupSet;
			}

			return null;
		}

		/// <inheritdoc />
		public IBackupSet CreateFullBackup (string name, bool includeNonRestorables) => this.CreateFullBackup(name, DateTime.Now, includeNonRestorables);

		/// <inheritdoc />
		public IBackupSet CreateFullBackup (string name, DateTime timestamp, bool includeNonRestorables) => this.CreateBackup(name, timestamp, from x in this.GetAvailableInclusions() where x.SupportsRestore || includeNonRestorables select x);

		/// <inheritdoc />
		public void DeleteBackup (IBackupSet backupSet)
		{
			if (backupSet == null)
			{
				throw new ArgumentNullException(nameof(backupSet));
			}

			lock (this.SyncRoot)
			{
				foreach (IBackupStorage storage in this.StoragesUpdated)
				{
					storage.TryDeleteBackup(backupSet);
				}
			}
		}

		/// <inheritdoc />
		public bool ExportBackupToFile (IBackupSet backupSet, FilePath file)
		{
			if (backupSet == null)
			{
				throw new ArgumentNullException(nameof(backupSet));
			}

			if (file == null)
			{
				throw new ArgumentNullException(nameof(file));
			}

			if (!file.IsRealFile)
			{
				throw new InvalidPathArgumentException(nameof(file));
			}

			file.Directory.Create();

			lock (this.SyncRoot)
			{
				if (!backupSet.CanExportToFile)
				{
					return false;
				}

				backupSet.ExportToFile(file);

				return true;
			}
		}

		/// <inheritdoc />
		public List<IBackupInclusion> GetAvailableInclusions ()
		{
			List<IBackupAware> awares;
			lock (this.SyncRoot)
			{
				awares = this.AwaresCopy;
			}

			List<IBackupInclusion> inclusions = new List<IBackupInclusion>();
			foreach (IBackupAware aware in awares)
			{
				aware.QueryBackupInclusions(inclusions, this);
			}
			return inclusions;
		}

		/// <inheritdoc />
		public List<IBackupSet> GetAvailableSets ()
		{
			lock (this.SyncRoot)
			{
				List<IBackupSet> sets = new List<IBackupSet>();
				foreach (IBackupStorage storage in this.StoragesUpdated)
				{
					sets.AddRange(storage.GetAvailableSets());
				}
				return sets;
			}
		}

		/// <inheritdoc />
		public IBackupSet ImportBackupFromFile (FilePath file)
		{
			if (file == null)
			{
				throw new ArgumentNullException(nameof(file));
			}

			if (!file.IsRealFile)
			{
				throw new InvalidPathArgumentException(nameof(file));
			}

			if (!file.Exists)
			{
				throw new FileNotFoundException("Backup import file not found.", file);
			}

			lock (this.SyncRoot)
			{
				foreach (IBackupStorage storage in this.StoragesUpdated)
				{
					IBackupSet set = storage.TryImportBackupFromFile(file);
					if (set != null)
					{
						return set;
					}
				}

				return null;
			}
		}

		/// <inheritdoc />
		public void RemoveAware (IBackupAware backupAware)
		{
			if (backupAware == null)
			{
				throw new ArgumentNullException(nameof(backupAware));
			}

			lock (this.SyncRoot)
			{
				if (!this.AwaresManual.Contains(backupAware))
				{
					return;
				}

				this.AwaresManual.RemoveAll(backupAware);

				this.UpdateAwares();
			}
		}

		/// <inheritdoc />
		public void RemoveStorage (IBackupStorage backupStorage)
		{
			if (backupStorage == null)
			{
				throw new ArgumentNullException(nameof(backupStorage));
			}

			lock (this.SyncRoot)
			{
				if (!this.StoragesManual.Contains(backupStorage))
				{
					return;
				}

				this.StoragesManual.RemoveAll(backupStorage);

				this.UpdateStorages();
			}
		}

		/// <inheritdoc />
		public bool RestoreBackup (IBackupSet backupSet, IEnumerable<IBackupInclusion> inclusions)
		{
			if (backupSet == null)
			{
				throw new ArgumentNullException(nameof(backupSet));
			}

			if (!backupSet.CanRestore)
			{
				throw new InvalidOperationException("The backup set does not support restore: " + backupSet);
			}

			List<IBackupInclusion> inclusionList = inclusions?.ToList() ?? (from x in backupSet.GetInclusions() where x.SupportsRestore select x).ToList();
			if (inclusionList.Count == 0)
			{
				throw new ArgumentException("Inclusion sequence is empty.", nameof(inclusions));
			}

			foreach (IBackupInclusion inclusion in inclusionList)
			{
				if (!inclusion.SupportsRestore)
				{
					throw new InvalidOperationException("The backup inclusion does not support restore: " + inclusion.Id);
				}
			}

			return this.RestoreBackupInternal(false, backupSet, inclusionList);
		}

		/// <inheritdoc />
		public bool RestoreFullBackup (IBackupSet backupSet) => this.RestoreBackup(backupSet, null);

		#endregion




		#region Interface: IImporting

		/// <inheritdoc />
		void IImporting.ImportsResolved (CompositionFlags composition, bool updated)
		{
			if (updated)
			{
				lock (this.SyncRoot)
				{
					this.UpdateStorages();
					this.UpdateAwares();
				}
			}
		}

		/// <inheritdoc />
		void IImporting.ImportsResolving (CompositionFlags composition)
		{
		}

		#endregion
	}
}
