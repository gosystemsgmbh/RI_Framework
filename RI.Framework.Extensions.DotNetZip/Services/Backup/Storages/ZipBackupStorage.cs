using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Ionic.Zip;

using RI.Framework.Collections;
using RI.Framework.Composition.Model;
using RI.Framework.IO.Files;
using RI.Framework.IO.INI;
using RI.Framework.IO.Paths;
using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;
using RI.Framework.Utilities.Logging;
using RI.Framework.Utilities.ObjectModel;

using DirectLinqExtensions = RI.Framework.Collections.DirectLinq.DirectLinqExtensions;




namespace RI.Framework.Services.Backup.Storages
{
	/// <summary>
	///     Implements a backup storage which uses ZIP files in a specified directory.
	/// </summary>
	/// <remarks>
	///     <para>
	///         Each ZIP file in the specified directory corresponds to one backup set (<see cref="ZipBackupSet" />).
	///         Each file in a ZIP file corresponds to a resolved stream.
	///         Subdirectories in ZIP files (backup sets) are not processed.
	///     </para>
	///     <para>
	///         A special file (<see cref="ZipBackupSet.BackupFileName" />) is expected in each ZIP file (backup set).
	///         It contains descriptions of the corresponding backup set and maps the files to their stream names.
	///     </para>
	///     <para>
	///         See <see cref="IBackupStorage" /> for more details.
	///     </para>
	/// </remarks>
	[Export]
	public sealed class ZipBackupStorage : LogSource, IBackupStorage
	{
		#region Constants

		/// <summary>
		///     The default file pattern which is used to search for ZIP files.
		/// </summary>
		/// <remarks>
		///     <para>
		///         The default file pattern is <c> *.zip </c>.
		///     </para>
		/// </remarks>
		public const string DefaultFilePattern = "*.zip";

		/// <summary>
		///     The default text encoding which is used for reading text files.
		/// </summary>
		/// <remarks>
		///     <para>
		///         The default text encoding is UTF-8.
		///     </para>
		/// </remarks>
		public static readonly Encoding DefaultEncoding = Encoding.UTF8;

		#endregion




		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="ZipBackupStorage" />.
		/// </summary>
		/// <param name="directory"> The directory which contains the backup set ZIP files. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="directory" /> is null. </exception>
		/// <exception cref="InvalidOperationException"> <paramref name="directory" /> is not a real usable directory. </exception>
		/// <remarks>
		///     <para>
		///         The default encoding <see cref="DefaultEncoding" /> is used as the text encoding.
		///     </para>
		///     <para>
		///         The default file pattern <see cref="DefaultFilePattern" /> is used and search is performed non-recursive.
		///     </para>
		/// </remarks>
		public ZipBackupStorage (DirectoryPath directory)
			: this(directory, null, null, false)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="ZipBackupStorage" />.
		/// </summary>
		/// <param name="directory"> The directory which contains the backup set ZIP files. </param>
		/// <param name="fileEncoding"> The text encoding used for reading text files (can be null to use <see cref="DefaultEncoding" />). </param>
		/// <param name="filePattern"> The file pattern which is used to search for ZIP files (can be null to use <see cref="DefaultFilePattern" />). </param>
		/// <param name="recursive"> Specifies whether ZIP files are searched recursive (including subdirectories) or not. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="directory" /> is null. </exception>
		/// <exception cref="InvalidOperationException"> <paramref name="directory" /> is not a real usable directory. </exception>
		/// <exception cref="InvalidPathArgumentException"> <paramref name="filePattern" /> is an empty string. </exception>
		public ZipBackupStorage (DirectoryPath directory, Encoding fileEncoding, string filePattern, bool recursive)
		{
			if (directory == null)
			{
				throw new ArgumentNullException(nameof(directory));
			}

			if (!directory.IsRealDirectory)
			{
				throw new InvalidPathArgumentException(nameof(directory));
			}

			if (filePattern != null)
			{
				if (filePattern.IsEmptyOrWhitespace())
				{
					throw new EmptyStringArgumentException(nameof(filePattern));
				}
			}

			this.SyncRoot = new object();
			this.IsInitialized = false;

			this.Directory = directory;
			this.FileEncoding = fileEncoding ?? ZipBackupStorage.DefaultEncoding;
			this.FilePattern = filePattern ?? ZipBackupStorage.DefaultFilePattern;
			this.Recursive = recursive;

			this.Sets = new Dictionary<FilePath, ZipBackupSet>();
		}

		#endregion




		#region Instance Fields

		private bool _isInitialized;

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets the directory which contains the backup set ZIP files.
		/// </summary>
		/// <value>
		///     The directory which contains the backup set ZIP files.
		/// </value>
		public DirectoryPath Directory { get; }

		/// <summary>
		///     Gets the text encoding for reading text files.
		/// </summary>
		/// <value>
		///     The text encoding for reading text files.
		/// </value>
		public Encoding FileEncoding { get; }

		/// <summary>
		///     Gets the file pattern which is used to search for ZIP files.
		/// </summary>
		/// <value>
		///     The file pattern which is used to search for ZIP files.
		/// </value>
		public string FilePattern { get; }

		/// <summary>
		///     Gets whether ZIP files are searched recursive (including subdirectories) or not.
		/// </summary>
		/// <value>
		///     true if subdirectories of <see cref="DirectoryPath" /> are searched for ZIP files, false otherwise.
		/// </value>
		public bool Recursive { get; }

		private Dictionary<FilePath, ZipBackupSet> Sets { get; }

		#endregion




		#region Instance Methods

		private void UpdateSets (bool unload)
		{
			HashSet<FilePath> currentFiles = unload ? new HashSet<FilePath>() : new HashSet<FilePath>(this.Directory.GetFiles(false, this.Recursive, this.FilePattern));
			HashSet<FilePath> lastFiles = new HashSet<FilePath>(this.Sets.Keys);

			HashSet<FilePath> newFiles = DirectLinqExtensions.Except(currentFiles, lastFiles);
			HashSet<FilePath> oldFiles = DirectLinqExtensions.Except(lastFiles, currentFiles);

			foreach (FilePath file in newFiles)
			{
				this.Log(LogLevel.Debug, "ZIP file added: {0}", file);
				ZipBackupSet set = new ZipBackupSet(file, this);
				this.Sets.Add(file, set);
			}

			foreach (FilePath file in oldFiles)
			{
				this.Log(LogLevel.Debug, "ZIP file removed: {0}", file);
				this.Sets.Remove(file);
			}

			foreach (FilePath file in currentFiles)
			{
				ZipBackupSet set = this.Sets[file];
				set.Prepare();
				if (!set.IsValid.GetValueOrDefault(false))
				{
					this.Log(LogLevel.Error, "Unable to use ZIP file as ZIP backup set: {0}", file);
				}
			}

			this.Sets.RemoveWhere(x => !x.Value.IsValid.GetValueOrDefault(false));
		}

		#endregion




		#region Interface: IBackupStorage

		/// <inheritdoc />
		public bool IsInitialized
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this._isInitialized;
				}
			}
			private set
			{
				lock (this.SyncRoot)
				{
					this._isInitialized = value;
				}
			}
		}

		/// <inheritdoc />
		public bool IsReadOnly => false;

		/// <inheritdoc />
		bool ISynchronizable.IsSynchronized => true;

		/// <inheritdoc />
		public object SyncRoot { get; }

		/// <inheritdoc />
		public void Cleanup (DateTime retentionDate)
		{
			lock (this.SyncRoot)
			{
				List<IBackupSet> sets = this.GetAvailableSets();
				foreach (IBackupSet set in sets)
				{
					if (set.Timestamp.Date < retentionDate.Date)
					{
						this.TryDeleteBackup(set);
					}
				}

				this.UpdateSets(!this.IsInitialized);
			}
		}

		/// <inheritdoc />
		public void EndBackup (IBackupSet backupSet)
		{
			if (backupSet == null)
			{
				throw new ArgumentNullException(nameof(backupSet));
			}

			ZipBackupSet usedSet = backupSet as ZipBackupSet;
			if (usedSet == null)
			{
				throw new ArgumentException("The backup set to end backup was not created by this backup storage.", nameof(backupSet));
			}

			lock (this.SyncRoot)
			{
				List<IBackupSet> sets = this.GetAvailableSets();
				if (!sets.Contains(backupSet))
				{
					throw new ArgumentException("The backup set to end backup was not created by this backup storage.", nameof(backupSet));
				}

				using (ZipFile zipFile = ZipFile.Read(usedSet.File))
				{
					foreach (KeyValuePair<Guid, Tuple<TemporaryFile, FileStream>> tempFile in usedSet.TemporaryFiles)
					{
						tempFile.Value.Item2.Flush(true);
						tempFile.Value.Item2.Position = 0;

						zipFile.AddEntry(tempFile.Key.ToString("N").ToUpperInvariant(), tempFile.Value.Item2);
					}

					zipFile.Save();
				}

				foreach (KeyValuePair<Guid, Tuple<TemporaryFile, FileStream>> tempFile in usedSet.TemporaryFiles)
				{
					tempFile.Value.Item2.Close();
					tempFile.Value.Item1.Delete();
				}

				usedSet.TemporaryFiles.Clear();

				this.UpdateSets(!this.IsInitialized);
			}
		}

		/// <inheritdoc />
		public void EndRestore (IBackupSet backupSet)
		{
			if (backupSet == null)
			{
				throw new ArgumentNullException(nameof(backupSet));
			}

			ZipBackupSet usedSet = backupSet as ZipBackupSet;
			if (usedSet == null)
			{
				throw new ArgumentException("The backup set to end restore was not created by this backup storage.", nameof(backupSet));
			}

			lock (this.SyncRoot)
			{
				List<IBackupSet> sets = this.GetAvailableSets();
				if (!sets.Contains(backupSet))
				{
					throw new ArgumentException("The backup set to end restore was not created by this backup storage.", nameof(backupSet));
				}

				foreach (KeyValuePair<Guid, Tuple<TemporaryFile, FileStream>> tempFile in usedSet.TemporaryFiles)
				{
					tempFile.Value.Item2.Close();
					tempFile.Value.Item1.Delete();
				}

				usedSet.TemporaryFiles.Clear();
			}
		}

		/// <inheritdoc />
		public List<IBackupSet> GetAvailableSets ()
		{
			lock (this.SyncRoot)
			{
				return this.Sets.Values.Cast<IBackupSet>().ToList();
			}
		}

		/// <inheritdoc />
		void IBackupStorage.Initialize ()
		{
			lock (this.SyncRoot)
			{
				this.Log(LogLevel.Debug, "Initializing ZIP backup storage: {0}", this.Directory);

				this.UpdateSets(false);

				this.IsInitialized = true;
			}
		}

		/// <inheritdoc />
		public bool TryBeginBackup (string name, DateTime timestamp, IEnumerable<IBackupInclusion> inclusions, out IBackupSet backupSet, out Func<Guid, Stream> streamResolver)
		{
			backupSet = null;
			streamResolver = null;

			if (name != null)
			{
				if (name.IsNullOrEmptyOrWhitespace())
				{
					throw new EmptyStringArgumentException(nameof(name));
				}
			}

			if (inclusions == null)
			{
				throw new ArgumentNullException(nameof(inclusions));
			}

			lock (this.SyncRoot)
			{
				IniDocument iniDocument = new IniDocument();
				iniDocument.AddValue(nameof(IBackupSet.Name), name);
				iniDocument.AddValue(nameof(IBackupSet.Timestamp), timestamp.ToSortableString('-'));
				foreach (IBackupInclusion inclusion in inclusions)
				{
					iniDocument.AddSectionHeader(ZipBackupSet.InclusionSectionName);
					iniDocument.AddValue(nameof(IBackupInclusion.Id), inclusion.Id.ToString("N").ToUpperInvariant());
					iniDocument.AddValue(nameof(IBackupInclusion.ResourceKey), inclusion.ResourceKey);
					iniDocument.AddValue(nameof(IBackupInclusion.SupportsRestore), inclusion.SupportsRestore.ToString());
					inclusion.Streams.ForEach(x => iniDocument.AddValue(nameof(IBackupInclusion.Streams), x.ToString("N").ToUpperInvariant()));
					inclusion.Tags.ForEach(x => iniDocument.AddValue(x.Key, x.Value));
				}

				FilePath path = this.Directory.AppendFile(timestamp.ToSortableString('-') + "_" + Guid.NewGuid().ToString("N").ToUpperInvariant());
				using (ZipFile zipFile = new ZipFile(path))
				{
					string backupFileContent = iniDocument.AsString();
					byte[] backupFileData = this.FileEncoding.GetBytes(backupFileContent);
					zipFile.AddEntry(ZipBackupSet.BackupFileName, backupFileData);
					zipFile.Save();
				}

				this.UpdateSets(!this.IsInitialized);

				ZipBackupSet usedSet = (from x in this.GetAvailableSets().Cast<ZipBackupSet>() where x.File.Equals(path) select x).FirstOrDefault();
				if (usedSet == null)
				{
					this.Log(LogLevel.Warning, "Failed to prepare new backup file: {0}", path);
					path.Delete();
					return false;
				}

				Func<Guid, Stream> usedStreamResolver = g =>
				{
					TemporaryFile tempFile = new TemporaryFile();
					FileStream tempStream = new FileStream(tempFile.File, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
					usedSet.TemporaryFiles.Add(g, new Tuple<TemporaryFile, FileStream>(tempFile, tempStream));
					return tempStream;
				};

				backupSet = usedSet;
				streamResolver = usedStreamResolver;

				return true;
			}
		}

		/// <inheritdoc />
		public bool TryBeginRestore (IBackupSet backupSet, IEnumerable<IBackupInclusion> inclusions, out Func<Guid, Stream> streamResolver)
		{
			streamResolver = null;

			if (backupSet == null)
			{
				throw new ArgumentNullException(nameof(backupSet));
			}

			if (inclusions == null)
			{
				throw new ArgumentNullException(nameof(inclusions));
			}

			lock (this.SyncRoot)
			{
				List<IBackupSet> sets = this.GetAvailableSets();
				if (!sets.Contains(backupSet))
				{
					return false;
				}

				ZipBackupSet usedSet = (ZipBackupSet)backupSet;

				Func<Guid, Stream> usedStreamResolver = g =>
				{
					string fileName = g.ToString("N").ToUpperInvariant();
					using (ZipFile zipFile = ZipFile.Read(usedSet.File))
					{
						if (!zipFile.ContainsEntry(fileName))
						{
							return null;
						}

						TemporaryFile tempFile = new TemporaryFile();
						FileStream tempStream = new FileStream(tempFile.File, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
						zipFile[fileName].Extract(tempStream);
						tempStream.Flush(true);
						tempStream.Position = 0;
						usedSet.TemporaryFiles.Add(g, new Tuple<TemporaryFile, FileStream>(tempFile, tempStream));
						return tempStream;
					}
				};

				streamResolver = usedStreamResolver;

				return true;
			}
		}

		/// <inheritdoc />
		public bool TryDeleteBackup (IBackupSet backupSet)
		{
			if (backupSet == null)
			{
				throw new ArgumentNullException(nameof(backupSet));
			}

			lock (this.SyncRoot)
			{
				ZipBackupSet set = backupSet as ZipBackupSet;
				if (set == null)
				{
					return false;
				}

				List<IBackupSet> sets = this.GetAvailableSets();
				if (!sets.Contains(set))
				{
					return false;
				}

				set.File.Delete();

				this.UpdateSets(!this.IsInitialized);

				return true;
			}
		}

		/// <inheritdoc />
		public IBackupSet TryImportBackupFromFile (FilePath file)
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
				FilePath targetFile = this.Directory.AppendFile(file.FileName);
				if (targetFile.Exists)
				{
					this.Log(LogLevel.Warning, "Backup file already exists: {0}", targetFile);
					return null;
				}

				if (!file.Copy(targetFile, false))
				{
					this.Log(LogLevel.Warning, "Cannot copy backup file: {0} -> {1}", file, targetFile);
					return null;
				}

				try
				{
					using (ZipFile zipFile = ZipFile.Read(targetFile))
					{
						DirectLinqExtensions.ToArray(zipFile);
					}
				}
				catch (Exception exception)
				{
					this.Log(LogLevel.Debug, "Cannot import backup file: {0}{1}{2}", targetFile, Environment.NewLine, exception.ToDetailedString());
					targetFile.Delete();
					return null;
				}

				this.UpdateSets(!this.IsInitialized);

				IBackupSet backupSet = (from x in this.GetAvailableSets().Cast<ZipBackupSet>() where x.File.Equals(targetFile) select x).FirstOrDefault();
				if (backupSet == null)
				{
					this.Log(LogLevel.Warning, "Failed to prepare imported backup file: {0}", targetFile);
					targetFile.Delete();
					return null;
				}

				return backupSet;
			}
		}

		/// <inheritdoc />
		void IBackupStorage.Unload ()
		{
			lock (this.SyncRoot)
			{
				this.Log(LogLevel.Debug, "Unloading ZIP backup storage: {0}", this.Directory);

				this.UpdateSets(true);

				this.IsInitialized = false;
			}
		}

		#endregion
	}
}
