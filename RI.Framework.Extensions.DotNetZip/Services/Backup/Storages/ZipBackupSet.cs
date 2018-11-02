using System;
using System.Collections.Generic;
using System.IO;

using Ionic.Zip;

using RI.Framework.Collections;
using RI.Framework.Collections.DirectLinq;
using RI.Framework.IO.Files;
using RI.Framework.IO.INI;
using RI.Framework.IO.Paths;
using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;
using RI.Framework.Utilities.Logging;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Services.Backup.Storages
{
    /// <summary>
    ///     Implements a backup set associated with a ZIP file of a <see cref="ZipBackupStorage" />.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         See <see cref="IBackupSet" /> and <see cref="ZipBackupStorage" /> for more details.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="true" instance="true" />
    public sealed class ZipBackupSet : LogSource, IBackupSet
    {
        #region Constants

        /// <summary>
        ///     The file name of the backup description file.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         The value is <c> [Backup].ini </c>.
        ///     </para>
        /// </remarks>
        public const string BackupFileName = "[Backup].ini";

        internal const string InclusionSectionName = "Inclusions";

        #endregion




        #region Instance Constructor/Destructor

        internal ZipBackupSet (FilePath file, ZipBackupStorage storage)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (storage == null)
            {
                throw new ArgumentNullException(nameof(storage));
            }

            this.SyncRoot = new object();

            this.File = file;
            this.Storage = storage;

            this.BackupFile = ZipBackupSet.BackupFileName;
            this.IsValid = null;

            this.Name = null;
            this.SizeInBytes = null;
            this.Timestamp = DateTime.MinValue;
            this.Inclusions = new List<BackupInclusion>();
            this.Streams = new Dictionary<Guid, FilePath>();

            this.TemporaryFiles = new Dictionary<Guid, Tuple<TemporaryFile, FileStream>>();
        }

        #endregion




        #region Instance Fields

        private bool? _isValid;

        private string _name;

        private long? _sizeInBytes;

        private DateTime _timestamp;

        #endregion




        #region Instance Properties/Indexer

        /// <summary>
        ///     Gets the backup description file path inside the ZIP file of this backup set.
        /// </summary>
        /// <value>
        ///     The backup description file path inside the ZIP file of this backup set.
        /// </value>
        public FilePath BackupFile { get; }

        /// <summary>
        ///     Gets the ZIP file of this backup set.
        /// </summary>
        /// <value>
        ///     The ZIP file of this backup set.
        /// </value>
        public FilePath File { get; }

        internal bool? IsValid
        {
            get
            {
                lock (this.SyncRoot)
                {
                    return this._isValid;
                }
            }
            private set
            {
                lock (this.SyncRoot)
                {
                    this._isValid = value;
                }
            }
        }

        internal ZipBackupStorage Storage { get; }

        internal Dictionary<Guid, Tuple<TemporaryFile, FileStream>> TemporaryFiles { get; }

        private List<BackupInclusion> Inclusions { get; }

        private Dictionary<Guid, FilePath> Streams { get; }

        #endregion




        #region Instance Methods

        internal Dictionary<Guid, FilePath> GetStreams ()
        {
            lock (this.SyncRoot)
            {
                return new Dictionary<Guid, FilePath>(this.Streams, this.Streams.Comparer);
            }
        }

        internal void Prepare ()
        {
            lock (this.SyncRoot)
            {
                if (this.IsValid.HasValue)
                {
                    return;
                }

                this.Log(LogLevel.Debug, "Preparing ZIP file backup set: {0}", this.File);

                this.Name = null;
                this.SizeInBytes = null;
                this.Timestamp = DateTime.MinValue;

                this.Inclusions.Clear();

                if (!this.File.Exists)
                {
                    this.Log(LogLevel.Error, "ZIP file does no longer exist: {0}", this.File);
                    this.IsValid = false;
                    return;
                }

                ZipFile zipFile = null;

                try
                {
                    try
                    {
                        zipFile = ZipFile.Read(this.File);
                    }
                    catch (Exception exception)
                    {
                        this.Log(LogLevel.Error, "Failed to open ZIP file on prepare: {0}{1}{2}", this.File, Environment.NewLine, exception.ToDetailedString());
                        this.IsValid = false;
                        return;
                    }

                    if (!zipFile.ContainsEntry(this.BackupFile))
                    {
                        this.Log(LogLevel.Error, "Backup description file does not exist in ZIP file: {0}", this.File);
                        this.IsValid = false;
                        return;
                    }

                    IniDocument iniDocument = new IniDocument();
                    try
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            zipFile[this.BackupFile].Extract(ms);

                            ms.Flush();
                            ms.Position = 0;

                            using (StreamReader sr = new StreamReader(ms, this.Storage.FileEncoding))
                            {
                                string content = sr.ReadToEnd();
                                iniDocument.Load(content);
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        this.Log(LogLevel.Error, "Backup description file is not a valid INI file: {0} @ {1}{2}{3}", this.BackupFile, this.File, Environment.NewLine, exception.ToDetailedString());
                        this.IsValid = false;
                        return;
                    }

                    this.SizeInBytes = this.File.Size;

                    Dictionary<string, string> settings = iniDocument.GetSection(null);

                    string timestampKey = nameof(this.Timestamp);
                    string nameKey = nameof(this.Name);

                    if (settings.ContainsKey(timestampKey))
                    {
                        string value = settings[timestampKey];
                        DateTime? candidate = value.ToDateTimeFromSortable('-');
                        if (candidate.HasValue)
                        {
                            this.Log(LogLevel.Debug, "Backup description value: {0}={1} @ {2}", timestampKey, value, this.File);
                            this.Timestamp = candidate.Value;
                        }
                        else
                        {
                            this.Log(LogLevel.Error, "Invalid backup description value in backup description file: {0}={1} @ {2}", timestampKey, value, this.File);
                            this.IsValid = false;
                            return;
                        }
                    }
                    else
                    {
                        this.Log(LogLevel.Error, "Missing required backup description value in backup description file: {0} @ {1}", timestampKey, this.File);
                        this.IsValid = false;
                        return;
                    }

                    if (settings.ContainsKey(nameKey))
                    {
                        string value = settings[nameKey];
                        if (!value.IsEmptyOrWhitespace())
                        {
                            this.Log(LogLevel.Debug, "Backup description value: {0}={1} @ {2}", nameKey, value, this.File);
                            this.Name = value;
                        }
                        else
                        {
                            this.Log(LogLevel.Warning, "Invalid backup description value in backup description file: {0}={1} @ {2}", nameKey, value, this.File);
                        }
                    }
                    else
                    {
                        this.Log(LogLevel.Warning, "Missing recommended backup description value in backup description file: {0} @ {1}", nameKey, this.File);
                    }

                    string idKey = nameof(BackupInclusion.Id);
                    string resourceKeyKey = nameof(BackupInclusion.ResourceKey);
                    string supportsRestoreKey = nameof(BackupInclusion.SupportsRestore);
                    string streamsKey = nameof(BackupInclusion.Streams);

                    List<Dictionary<string, List<string>>> inclusions = iniDocument.GetSectionsAll(ZipBackupSet.InclusionSectionName);
                    foreach (Dictionary<string, List<string>> inclusion in inclusions)
                    {
                        Guid id;
                        string resourceKey = null;
                        bool supportsRestore;

                        List<Guid> streams = null;
                        Dictionary<string, string> tags;

                        if (inclusion.ContainsKey(idKey))
                        {
                            string value = inclusion[idKey][0];
                            Guid? candidate = value.ToGuid();
                            if (candidate.HasValue)
                            {
                                this.Log(LogLevel.Debug, "Backup inclusion value: {0}={1} @ {2}", idKey, value, this.File);
                                id = candidate.Value;
                            }
                            else
                            {
                                this.Log(LogLevel.Error, "Invalid backup inclusion value: {0}={1} @ {2}", idKey, value, this.File);
                                this.IsValid = false;
                                return;
                            }
                        }
                        else
                        {
                            this.Log(LogLevel.Error, "Missing backup inclusion value: {0} @ {1}", idKey, this.File);
                            this.IsValid = false;
                            return;
                        }

                        if (inclusion.ContainsKey(resourceKeyKey))
                        {
                            string value = inclusion[resourceKeyKey][0];
                            if (!value.IsNullOrEmptyOrWhitespace())
                            {
                                this.Log(LogLevel.Debug, "Backup inclusion value: {0}={1} @ {2}", resourceKeyKey, value, this.File);
                                resourceKey = value;
                            }
                            else
                            {
                                this.Log(LogLevel.Warning, "Invalid backup inclusion value: {0}={1} @ {2}", resourceKeyKey, value, this.File);
                            }
                        }
                        else
                        {
                            this.Log(LogLevel.Warning, "Missing backup inclusion value: {0} @ {1}", resourceKeyKey, this.File);
                        }

                        if (inclusion.ContainsKey(supportsRestoreKey))
                        {
                            string value = inclusion[supportsRestoreKey][0];
                            bool? candidate = value.ToBoolean();
                            if (candidate.HasValue)
                            {
                                this.Log(LogLevel.Debug, "Backup inclusion value: {0}={1} @ {2}", supportsRestoreKey, value, this.File);
                                supportsRestore = candidate.Value;
                            }
                            else
                            {
                                this.Log(LogLevel.Error, "Invalid backup inclusion value: {0}={1} @ {2}", supportsRestoreKey, value, this.File);
                                this.IsValid = false;
                                return;
                            }
                        }
                        else
                        {
                            this.Log(LogLevel.Error, "Missing backup inclusion value: {0} @ {1}", supportsRestoreKey, this.File);
                            this.IsValid = false;
                            return;
                        }

                        if (inclusion.ContainsKey(streamsKey))
                        {
                            streams = new List<Guid>();
                            List<string> values = inclusion[streamsKey];
                            foreach (string value in values)
                            {
                                Guid? candidate = value.ToGuid();
                                if (candidate.HasValue)
                                {
                                    this.Log(LogLevel.Debug, "Backup inclusion stream value: {0}={1} @ {2}", streamsKey, value, this.File);
                                    streams.Add(candidate.Value);
                                }
                                else
                                {
                                    this.Log(LogLevel.Error, "Invalid backup inclusion stream value: {0}={1} @ {2}", streamsKey, value, this.File);
                                    this.IsValid = false;
                                    return;
                                }
                            }
                        }

                        if (streams == null)
                        {
                            streams = new List<Guid>();
                        }

                        inclusion.Remove(idKey);
                        inclusion.Remove(resourceKeyKey);
                        inclusion.Remove(supportsRestoreKey);
                        inclusion.Remove(streamsKey);

                        tags = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
                        foreach (KeyValuePair<string, List<string>> tag in inclusion)
                        {
                            tags.Add(tag.Key, tag.Value[0]);
                        }

                        BackupInclusion inclusionObj = new BackupInclusion(id, resourceKey, supportsRestore);
                        inclusionObj.Streams.AddRange(streams);
                        inclusionObj.Tags.AddRange(tags);

                        this.Inclusions.Add(inclusionObj);
                    }

                    ZipEntry[] entries = zipFile.ToArray();

                    List<FilePath> files = (from x in entries where !x.IsDirectory select new FilePath(x.FileName)).ToList();
                    files.Remove(this.BackupFile);

                    foreach (FilePath file in files)
                    {
                        string fileName = file.FileNameWithoutExtension;
                        Guid? candidate = fileName.ToGuid();
                        if (candidate.HasValue)
                        {
                            this.Log(LogLevel.Debug, "Backup stream name: {0} @ {1}", fileName, this.File);
                            this.Streams.Add(candidate.Value, file);
                        }
                        else
                        {
                            this.Log(LogLevel.Error, "Invalid backup stream name: {0} @ {1}", fileName, this.File);
                            this.IsValid = false;
                            return;
                        }
                    }
                }
                finally
                {
                    zipFile?.Dispose();
                }

                this.IsValid = true;
            }
        }

        #endregion




        #region Overrides

        /// <inheritdoc />
        public override bool Equals (object obj) => this.Equals(obj as IBackupSet);

        /// <inheritdoc />
        public override int GetHashCode () => this.File.GetHashCode();

        #endregion




        #region Interface: IBackupSet

        /// <inheritdoc />
        public bool CanExportToFile => true;

        /// <inheritdoc />
        public bool CanRestore
        {
            get
            {
                lock (this.SyncRoot)
                {
                    if (this.Inclusions.Count == 0)
                    {
                        return false;
                    }

                    return this.Inclusions.Any(x => x.SupportsRestore);
                }
            }
        }

        /// <inheritdoc />
        bool ISynchronizable.IsSynchronized => true;

        /// <inheritdoc />
        public string Name
        {
            get
            {
                lock (this.SyncRoot)
                {
                    return this._name;
                }
            }
            private set
            {
                lock (this.SyncRoot)
                {
                    this._name = value;
                }
            }
        }

        /// <inheritdoc />
        public long? SizeInBytes
        {
            get
            {
                lock (this.SyncRoot)
                {
                    return this._sizeInBytes;
                }
            }
            private set
            {
                lock (this.SyncRoot)
                {
                    this._sizeInBytes = value;
                }
            }
        }

        /// <inheritdoc />
        public object SyncRoot { get; }

        /// <inheritdoc />
        public DateTime Timestamp
        {
            get
            {
                lock (this.SyncRoot)
                {
                    return this._timestamp;
                }
            }
            private set
            {
                lock (this.SyncRoot)
                {
                    this._timestamp = value;
                }
            }
        }

        /// <inheritdoc />
        public bool Equals (IBackupSet other)
        {
            if (other == null)
            {
                return false;
            }

            ZipBackupSet other2 = other as ZipBackupSet;
            if (other2 == null)
            {
                return false;
            }

            return this.File.Equals(other2.File);
        }

        /// <inheritdoc />
        public void ExportToFile (FilePath targetFile)
        {
            if (targetFile == null)
            {
                throw new ArgumentNullException(nameof(targetFile));
            }

            if (!targetFile.IsRealFile)
            {
                throw new InvalidPathArgumentException(nameof(targetFile));
            }

            lock (this.SyncRoot)
            {
                this.File.Copy(targetFile, true);
            }
        }

        /// <inheritdoc />
        public List<BackupInclusion> GetInclusions ()
        {
            lock (this.SyncRoot)
            {
                return new List<BackupInclusion>(this.Inclusions);
            }
        }

        #endregion
    }
}
