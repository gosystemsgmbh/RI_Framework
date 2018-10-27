using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

using Ionic.Zip;

using RI.Framework.Collections.DirectLinq;
using RI.Framework.IO.INI;
using RI.Framework.IO.Paths;
using RI.Framework.Services.Resources.Converters;
using RI.Framework.Services.Resources.Internals;
using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;
using RI.Framework.Utilities.Logging;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Services.Resources.Sources
{
    /// <summary>
    ///     Implements a resource set associated with a ZIP file of a <see cref="ZipResourceSource" />.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         See <see cref="IResourceSet" /> and <see cref="ZipResourceSource" /> for more details.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="true" instance="true" />
    public sealed class ZipResourceSet : LogSource, IResourceSet
    {
        #region Constants

        /// <summary>
        ///     The file name of the settings file.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         The value is <c> [Settings].ini </c>.
        ///     </para>
        /// </remarks>
        public const string SettingsFileName = "[Settings].ini";

        #endregion




        #region Instance Constructor/Destructor

        internal ZipResourceSet (FilePath file, ZipResourceSource source)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            this.SyncRoot = new object();

            this.Id = file.FileNameWithoutExtension;

            this.File = file;
            this.Source = source;

            this.SettingsFile = ZipResourceSet.SettingsFileName;
            this.IsValid = null;

            this.Name = null;
            this.Group = null;
            this.Selectable = false;
            this.AlwaysLoad = false;
            this.Priority = 0;
            this.Culture = null;
            this.Formatting = null;

            this.IsLoaded = false;
            this.IsLazyLoaded = false;

            this.Resources = new Dictionary<string, Tuple<FilePath, Loader>>(StringComparerEx.TrimmedInvariantCultureIgnoreCase);
        }

        #endregion




        #region Instance Fields

        private bool _isLazyLoaded;

        private bool _isLoaded;

        #endregion




        #region Instance Properties/Indexer

        /// <summary>
        ///     Gets the ZIP file of this resource set.
        /// </summary>
        /// <value>
        ///     The ZIP file of this resource set.
        /// </value>
        public FilePath File { get; }

        /// <summary>
        ///     Gets the settings file path inside the ZIP file of this resource set.
        /// </summary>
        /// <value>
        ///     The settings file path inside the ZIP file of this resource set.
        /// </value>
        public FilePath SettingsFile { get; }

        internal bool? IsValid { get; private set; }

        private List<IResourceConverter> Converters => this.Source.Converters;

        private Dictionary<string, Tuple<FilePath, Loader>> Resources { get; }

        private ZipResourceSource Source { get; }

        #endregion




        #region Instance Methods

        internal void Prepare ()
        {
            if (this.IsValid.HasValue)
            {
                return;
            }

            this.Log(LogLevel.Debug, "Preparing ZIP file resource set: {0}", this.File);

            this.Name = null;
            this.Group = null;
            this.Selectable = false;
            this.AlwaysLoad = false;
            this.Priority = 0;
            this.Culture = null;
            this.Formatting = null;

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

                if (!zipFile.ContainsEntry(this.SettingsFile))
                {
                    this.Log(LogLevel.Error, "Settings file does not exist in ZIP file: {0}", this.File);
                    this.IsValid = false;
                    return;
                }

                IniDocument iniDocument = new IniDocument();
                try
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        zipFile[this.SettingsFile].Extract(ms);

                        ms.Flush();
                        ms.Position = 0;

                        using (StreamReader sr = new StreamReader(ms, this.Source.FileEncoding))
                        {
                            string content = sr.ReadToEnd();
                            iniDocument.Load(content);
                        }
                    }
                }
                catch (Exception exception)
                {
                    this.Log(LogLevel.Error, "Settings file is not a valid INI file: {0} @ {1}{2}{3}", this.SettingsFile, this.File, Environment.NewLine, exception.ToDetailedString());
                    this.IsValid = false;
                    return;
                }

                Dictionary<string, string> settings = iniDocument.GetSection(null);

                string nameKey = nameof(this.Name);
                string groupKey = nameof(this.Group);
                string selectableKey = nameof(this.Selectable);
                string alwaysLoadKey = nameof(this.AlwaysLoad);
                string priorityKey = nameof(this.Priority);
                string uiCultureKey = nameof(this.Culture);
                string formattingCultureKey = nameof(this.Formatting);

                if (settings.ContainsKey(nameKey))
                {
                    string value = settings[nameKey];
                    if (!value.IsEmptyOrWhitespace())
                    {
                        this.Log(LogLevel.Debug, "Settings value: {0}={1} @ {2}", nameKey, value, this.File);
                        this.Name = value;
                    }
                    else
                    {
                        this.Log(LogLevel.Error, "Invalid settings value in settings file: {0}={1} @ {2}", nameKey, value, this.File);
                        this.IsValid = false;
                        return;
                    }
                }
                else
                {
                    this.Log(LogLevel.Error, "Missing required settings value in settings file: {0} @ {1}", nameKey, this.File);
                    this.IsValid = false;
                    return;
                }

                if (settings.ContainsKey(groupKey))
                {
                    string value = settings[groupKey];
                    if (!value.IsEmptyOrWhitespace())
                    {
                        this.Log(LogLevel.Debug, "Settings value: {0}={1} @ {2}", groupKey, value, this.File);
                        this.Group = value;
                    }
                    else
                    {
                        this.Log(LogLevel.Warning, "Invalid settings value in settings file: {0}={1} @ {2}", groupKey, value, this.File);
                    }
                }
                else
                {
                    this.Log(LogLevel.Warning, "Missing recommended settings value in settings file: {0} @ {1}", groupKey, this.File);
                }

                if (settings.ContainsKey(selectableKey))
                {
                    string value = settings[selectableKey];
                    bool? candidate = value.ToBoolean();
                    if (candidate.HasValue)
                    {
                        this.Log(LogLevel.Debug, "Settings value: {0}={1} @ {2}", selectableKey, value, this.File);
                        this.Selectable = candidate.Value;
                    }
                    else
                    {
                        this.Log(LogLevel.Warning, "Invalid settings value in settings file: {0}={1} @ {2}", selectableKey, value, this.File);
                    }
                }
                else
                {
                    this.Log(LogLevel.Warning, "Missing recommended settings value in settings file: {0} @ {1}", selectableKey, this.File);
                }

                if (settings.ContainsKey(alwaysLoadKey))
                {
                    string value = settings[alwaysLoadKey];
                    bool? candidate = value.ToBoolean();
                    if (candidate.HasValue)
                    {
                        this.Log(LogLevel.Debug, "Settings value: {0}={1} @ {2}", alwaysLoadKey, value, this.File);
                        this.AlwaysLoad = candidate.Value;
                    }
                    else
                    {
                        this.Log(LogLevel.Warning, "Invalid settings value in settings file: {0}={1} @ {2}", alwaysLoadKey, value, this.File);
                    }
                }
                else
                {
                    this.Log(LogLevel.Warning, "Missing recommended settings value in settings file: {0} @ {1}", alwaysLoadKey, this.File);
                }

                if (settings.ContainsKey(priorityKey))
                {
                    string value = settings[priorityKey];
                    int? candidate = value.ToInt32(NumberStyles.Integer, CultureInfo.InvariantCulture);
                    if (candidate.HasValue)
                    {
                        this.Log(LogLevel.Debug, "Settings value: {0}={1} @ {2}", priorityKey, value, this.File);
                        this.Priority = candidate.Value;
                    }
                    else
                    {
                        this.Log(LogLevel.Warning, "Invalid settings value in settings file: {0}={1} @ {2}", priorityKey, value, this.File);
                    }
                }
                else
                {
                    this.Log(LogLevel.Warning, "Missing recommended settings value in settings file: {0} @ {1}", priorityKey, this.File);
                }

                if (settings.ContainsKey(uiCultureKey))
                {
                    string value = settings[uiCultureKey];
                    CultureInfo candidate;
                    try
                    {
                        candidate = new CultureInfo(value, false);
                    }
                    catch (CultureNotFoundException)
                    {
                        candidate = null;
                    }

                    if (candidate != null)
                    {
                        this.Log(LogLevel.Debug, "Settings value: {0}={1} @ {2}", uiCultureKey, value, this.File);
                        this.Culture = candidate;
                    }
                    else
                    {
                        this.Log(LogLevel.Warning, "Invalid settings value in settings file: {0}={1} @ {2}", uiCultureKey, value, this.File);
                    }
                }
                else
                {
                    this.Log(LogLevel.Debug, "Missing optional settings value in settings file: {0} @ {1}", uiCultureKey, this.File);
                }

                if (settings.ContainsKey(formattingCultureKey))
                {
                    string value = settings[formattingCultureKey];
                    CultureInfo candidate;
                    try
                    {
                        candidate = new CultureInfo(value, false);
                    }
                    catch (CultureNotFoundException)
                    {
                        candidate = null;
                    }

                    if (candidate != null)
                    {
                        this.Log(LogLevel.Debug, "Settings value: {0}={1} @ {2}", formattingCultureKey, value, this.File);
                        this.Formatting = candidate;
                    }
                    else
                    {
                        this.Log(LogLevel.Warning, "Invalid settings value in settings file: {0}={1} @ {2}", formattingCultureKey, value, this.File);
                    }
                }
                else
                {
                    this.Log(LogLevel.Debug, "Missing optional settings value in settings file: {0} @ {1}", formattingCultureKey, this.File);
                }
            }
            finally
            {
                zipFile?.Dispose();
            }

            this.IsValid = true;
        }

        private IResourceConverter GetConverter (Type sourceType, Type targetType)
        {
            foreach (IResourceConverter converter in this.Converters)
            {
                if (converter.CanConvert(sourceType, targetType))
                {
                    return converter;
                }
            }

            return null;
        }

        private ResourceLoadingInfo GetLoadingInfo (string extension)
        {
            foreach (IResourceConverter converter in this.Converters)
            {
                ResourceLoadingInfo loadingInfo = converter.GetLoadingInfoFromFileExtension(extension);
                if (loadingInfo != null)
                {
                    return loadingInfo;
                }
            }

            return new ResourceLoadingInfo(ResourceLoadingType.Unknown, null);
        }

        private void Load ()
        {
            ZipFile zipFile = null;

            try
            {
                try
                {
                    zipFile = ZipFile.Read(this.File);
                }
                catch (Exception exception)
                {
                    this.Log(LogLevel.Error, "Failed to open ZIP file on load: {0}{1}{2}", this.File, Environment.NewLine, exception.ToDetailedString());
                    return;
                }

                ZipEntry[] entries = zipFile.ToArray();

                List<FilePath> existingFiles = (from x in entries where !x.IsDirectory select new FilePath(x.FileName)).ToList();
                existingFiles.Remove(this.SettingsFile);

                HashSet<FilePath> newFiles = existingFiles.Except(from x in this.Resources select x.Value.Item1);
                newFiles.RemoveWhere(x => this.Source.IgnoredExtensionsInternal.Contains(x.ExtensionWithoutDot));

                foreach (FilePath file in newFiles)
                {
                    string extension = file.ExtensionWithDot.ToUpperInvariant();
                    ResourceLoadingInfo loadingInfo = this.GetLoadingInfo(extension);

                    switch (loadingInfo.LoadingType)
                    {
                        default:
                        {
                            this.Log(LogLevel.Warning, "Unknown resource loading type: {0} @ {1} @ {2}", loadingInfo.LoadingType, file, this.File);
                            break;
                        }

                        case ResourceLoadingType.Binary:
                        {
                            string name = file.FileNameWithoutExtension;
                            ByteArrayLoader loader = new ByteArrayLoader(file, this.File, loadingInfo.ResourceType);
                            this.Resources.Add(name, new Tuple<FilePath, Loader>(file, loader));
                            this.Log(LogLevel.Debug, "Added binary resource file: {0} = {1} @ {2}", name, file, this.File);
                            break;
                        }

                        case ResourceLoadingType.Text:
                        {
                            string name = file.FileNameWithoutExtension;
                            StringLoader loader = new StringLoader(file, this.File, this.Source.FileEncoding, loadingInfo.ResourceType);
                            this.Resources.Add(name, new Tuple<FilePath, Loader>(file, loader));
                            this.Log(LogLevel.Debug, "Added text resource file: {0} = {1} @ {2}", name, file, this.File);
                            break;
                        }

                        case ResourceLoadingType.Unknown:
                        {
                            switch (extension)
                            {
                                default:
                                {
                                    this.Log(LogLevel.Warning, "Unknown resource file type: {0} @ {1}", file, this.File);
                                    break;
                                }

                                case ".INI":
                                {
                                    IniDocument iniDocument = new IniDocument();
                                    try
                                    {
                                        using (MemoryStream ms = new MemoryStream())
                                        {
                                            zipFile[file].Extract(ms);

                                            ms.Flush();
                                            ms.Position = 0;

                                            using (StreamReader sr = new StreamReader(ms, this.Source.FileEncoding))
                                            {
                                                string content = sr.ReadToEnd();
                                                iniDocument.Load(content);
                                            }
                                        }

                                        Dictionary<string, string> values = iniDocument.GetSection(null);
                                        foreach (KeyValuePair<string, string> value in values)
                                        {
                                            string name = value.Key;
                                            EagerLoader loader = new EagerLoader(value.Value);
                                            this.Resources.Add(name, new Tuple<FilePath, Loader>(file, loader));
                                        }

                                        this.Log(LogLevel.Debug, "Added INI resource file: {0} @ {1}", file, this.File);
                                    }
                                    catch (Exception exception)
                                    {
                                        this.Log(LogLevel.Error, "Invalid INI resource file: {0} @ {1}{2}{3}", file, this.File, Environment.NewLine, exception.ToDetailedString());
                                    }

                                    break;
                                }
                            }

                            break;
                        }
                    }
                }
            }
            finally
            {
                zipFile?.Dispose();
            }
        }

        #endregion




        #region Overrides

        /// <inheritdoc />
        public override bool Equals (object obj) => this.Equals(obj as IResourceSet);

        /// <inheritdoc />
        public override int GetHashCode () => this.File.GetHashCode();

        #endregion




        #region Interface: IResourceSet

        /// <inheritdoc />
        public bool AlwaysLoad { get; private set; }

        /// <inheritdoc />
        public CultureInfo Culture { get; private set; }

        /// <inheritdoc />
        public CultureInfo Formatting { get; private set; }

        /// <inheritdoc />
        public string Group { get; private set; }

        /// <inheritdoc />
        public string Id { get; }

        /// <inheritdoc />
        public bool IsLazyLoaded
        {
            get
            {
                lock (this.SyncRoot)
                {
                    return this._isLazyLoaded;
                }
            }
            private set
            {
                lock (this.SyncRoot)
                {
                    this._isLazyLoaded = value;
                }
            }
        }

        /// <inheritdoc />
        public bool IsLoaded
        {
            get
            {
                lock (this.SyncRoot)
                {
                    return this._isLoaded;
                }
            }
            private set
            {
                lock (this.SyncRoot)
                {
                    this._isLoaded = value;
                }
            }
        }

        /// <inheritdoc />
        bool ISynchronizable.IsSynchronized => true;

        /// <inheritdoc />
        public string Name { get; private set; }

        /// <inheritdoc />
        public int Priority { get; private set; }

        /// <inheritdoc />
        public bool Selectable { get; private set; }

        /// <inheritdoc />
        public object SyncRoot { get; }

        /// <inheritdoc />
        public int CompareTo (object obj) => this.CompareTo(obj as IResourceSet);

        /// <inheritdoc />
        public int CompareTo (IResourceSet other)
        {
            if (other == null)
            {
                return -1;
            }

            return this.Priority.CompareTo(other.Priority);
        }

        /// <inheritdoc />
        public bool Equals (IResourceSet other)
        {
            if (other == null)
            {
                return false;
            }

            ZipResourceSet other2 = other as ZipResourceSet;
            if (other2 == null)
            {
                return false;
            }

            return this.File.Equals(other2.File);
        }

        /// <inheritdoc />
        public HashSet<string> GetAvailableResources ()
        {
            lock (this.SyncRoot)
            {
                return new HashSet<string>(this.Resources.Keys, this.Resources.Comparer);
            }
        }

        /// <inheritdoc />
        public object GetRawValue (string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (name.IsEmptyOrWhitespace())
            {
                throw new EmptyStringArgumentException(nameof(name));
            }

            lock (this.SyncRoot)
            {
                if (!this.Resources.ContainsKey(name))
                {
                    return null;
                }

                Loader loader = this.Resources[name].Item2;
                object loadedValue = loader.Load();

                Type sourceType = loadedValue.GetType();
                Type targetType = loader.Type;

                IResourceConverter converter = this.GetConverter(sourceType, targetType);
                if (converter == null)
                {
                    return null;
                }

                object rawValue = converter.Convert(targetType, loadedValue);
                return rawValue;
            }
        }

        /// <inheritdoc />
        public bool Load (bool lazyLoad)
        {
            lock (this.SyncRoot)
            {
                this.Log(LogLevel.Debug, "Loading ZIP file resource set: {0}", this.File);

                this.IsLoaded = true;
                this.IsLazyLoaded = lazyLoad;

                this.Resources.Clear();

                this.Load();

                if (!lazyLoad)
                {
                    foreach (string resource in this.Resources.Keys)
                    {
                        this.GetRawValue(resource);
                    }
                }

                return lazyLoad;
            }
        }

        /// <inheritdoc />
        public void Unload ()
        {
            lock (this.SyncRoot)
            {
                this.Log(LogLevel.Debug, "Unloading ZIP file resource set: {0}", this.File);

                this.Resources.Clear();

                this.IsLoaded = false;
                this.IsLazyLoaded = false;
            }
        }

        #endregion




        #region Type: ByteArrayLoader

        private sealed class ByteArrayLoader : Loader
        {
            #region Instance Constructor/Destructor

            public ByteArrayLoader (FilePath file, FilePath zipFile, Type type)
            {
                if (file == null)
                {
                    throw new ArgumentNullException(nameof(file));
                }

                if (file.HasWildcards)
                {
                    throw new InvalidPathArgumentException(nameof(file));
                }

                if (zipFile == null)
                {
                    throw new ArgumentNullException(nameof(zipFile));
                }

                if (zipFile.HasWildcards)
                {
                    throw new InvalidPathArgumentException(nameof(zipFile));
                }

                if (type == null)
                {
                    throw new ArgumentNullException(nameof(type));
                }

                this.File = file;
                this.ZipFile = zipFile;
                this.Type = type;

                this.Data = null;
            }

            #endregion




            #region Instance Properties/Indexer

            private byte[] Data { get; set; }

            private FilePath File { get; }

            private FilePath ZipFile { get; }

            #endregion




            #region Overrides

            public override Type Type { get; }

            public override object Load ()
            {
                if (this.Data == null)
                {
                    using (ZipFile zipFile = Ionic.Zip.ZipFile.Read(this.File))
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            zipFile[this.File].Extract(ms);

                            ms.Flush();
                            ms.Position = 0;

                            this.Data = ms.ToArray();
                        }
                    }
                }

                return this.Data;
            }

            #endregion
        }

        #endregion




        #region Type: EagerLoader

        private sealed class EagerLoader : Loader
        {
            #region Instance Constructor/Destructor

            public EagerLoader (object value)
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                this.Value = value;
            }

            #endregion




            #region Instance Properties/Indexer

            private object Value { get; set; }

            #endregion




            #region Overrides

            public override Type Type => this.Value.GetType();

            public override object Load ()
            {
                return this.Value;
            }

            #endregion
        }

        #endregion




        #region Type: Loader

        private abstract class Loader
        {
            #region Abstracts

            public abstract Type Type { get; }

            public abstract object Load ();

            #endregion
        }

        #endregion




        #region Type: StringLoader

        private sealed class StringLoader : Loader
        {
            #region Instance Constructor/Destructor

            public StringLoader (FilePath file, FilePath zipFile, Encoding encoding, Type type)
            {
                if (file == null)
                {
                    throw new ArgumentNullException(nameof(file));
                }

                if (file.HasWildcards)
                {
                    throw new InvalidPathArgumentException(nameof(file));
                }

                if (zipFile == null)
                {
                    throw new ArgumentNullException(nameof(zipFile));
                }

                if (zipFile.HasWildcards)
                {
                    throw new InvalidPathArgumentException(nameof(zipFile));
                }

                if (encoding == null)
                {
                    throw new ArgumentNullException(nameof(encoding));
                }

                if (type == null)
                {
                    throw new ArgumentNullException(nameof(type));
                }

                this.File = file;
                this.ZipFile = zipFile;
                this.Encoding = encoding;
                this.Type = type;

                this.Data = null;
            }

            #endregion




            #region Instance Properties/Indexer

            private string Data { get; set; }

            private Encoding Encoding { get; }

            private FilePath File { get; }

            private FilePath ZipFile { get; }

            #endregion




            #region Overrides

            public override Type Type { get; }

            public override object Load ()
            {
                if (this.Data == null)
                {
                    using (ZipFile zipFile = Ionic.Zip.ZipFile.Read(this.File))
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            zipFile[this.File].Extract(ms);

                            ms.Flush();
                            ms.Position = 0;

                            using (StreamReader sr = new StreamReader(ms, this.Encoding))
                            {
                                this.Data = sr.ReadToEnd();
                            }
                        }
                    }
                }

                return this.Data;
            }

            #endregion
        }

        #endregion
    }
}
