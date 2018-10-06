using System;
using System.Collections.Generic;
using System.Text;

using RI.Framework.Collections;
using RI.Framework.Collections.DirectLinq;
using RI.Framework.Composition.Model;
using RI.Framework.IO.Paths;
using RI.Framework.Services.Resources.Converters;
using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;
using RI.Framework.Utilities.Logging;
using RI.Framework.Utilities.ObjectModel;


namespace RI.Framework.Services.Resources.Sources
{
    /// <summary>
    ///     Implements a resource source which reads from a specified directory.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Each subdirectory in the specified directory corresponds to one resource set (<see cref="DirectoryResourceSet" />).
    ///         Each file in a subdirectory is read and loaded, if the file extension is known.
    ///         Subdirectories of subdirectories are not processed.
    ///     </para>
    ///     <para>
    ///         A special file (<see cref="DirectoryResourceSet.SettingsFileName" />) is expected in each subdirectory (resource set).
    ///         It contains descriptions and settings of the corresponding resource set.
    ///     </para>
    ///     <para>
    ///         See <see cref="IResourceSource" /> for more details.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="true" instance="true" />
    [Export]
    public sealed class DirectoryResourceSource : LogSource, IResourceSource
    {
        #region Constants

        /// <summary>
        ///     The default text encoding which is used for reading text files.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         The default text encoding is UTF-8.
        ///     </para>
        /// </remarks>
        public static readonly Encoding DefaultEncoding = Encoding.UTF8;

        private bool _isInitialized;

        #endregion




        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="DirectoryResourceSource" />.
        /// </summary>
        /// <param name="directory"> The directory which contains the resource set subdirectories. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="directory" /> is null. </exception>
        /// <exception cref="InvalidOperationException"> <paramref name="directory" /> is not a real usable directory. </exception>
        /// <remarks>
        ///     <para>
        ///         The default encoding <see cref="DefaultEncoding" /> is used as the text encoding.
        ///     </para>
        ///     <para>
        ///         No files are ignored.
        ///     </para>
        /// </remarks>
        public DirectoryResourceSource (DirectoryPath directory)
            : this(directory, null, false, null)
        {
        }

        /// <summary>
        ///     Creates a new instance of <see cref="DirectoryResourceSource" />.
        /// </summary>
        /// <param name="directory"> The directory which contains the resource set subdirectories. </param>
        /// <param name="fileEncoding"> The text encoding used for reading text files (can be null to use <see cref="DefaultEncoding" />). </param>
        /// <param name="recursive"> Specifies whether subdirectories are searched recursive for resource files or not. </param>
        /// <param name="ignoredExtensions"> A sequence of file extensions which are completely ignored (can be null to not ignore any files). </param>
        /// <exception cref="ArgumentNullException"> <paramref name="directory" /> is null. </exception>
        /// <exception cref="InvalidOperationException"> <paramref name="directory" /> is not a real usable directory. </exception>
        public DirectoryResourceSource (DirectoryPath directory, Encoding fileEncoding, bool recursive, IEnumerable<string> ignoredExtensions)
        {
            if (directory == null)
            {
                throw new ArgumentNullException(nameof(directory));
            }

            if (!directory.IsRealDirectory)
            {
                throw new InvalidPathArgumentException(nameof(directory));
            }

            this.SyncRoot = new object();

            this.Directory = directory;
            this.FileEncoding = fileEncoding ?? DirectoryResourceSource.DefaultEncoding;
            this.Recursive = recursive;

            this.IgnoredExtensionsInternal = new HashSet<string>((ignoredExtensions ?? new string[0]).Select(x => x.TrimStart('.')), StringComparerEx.InvariantCultureIgnoreCase);

            this.Sets = new Dictionary<DirectoryPath, DirectoryResourceSet>();
        }

        /// <summary>
        ///     Creates a new instance of <see cref="DirectoryResourceSource" />.
        /// </summary>
        /// <param name="directory"> The directory which contains the resource set subdirectories. </param>
        /// <param name="fileEncoding"> The text encoding used for reading text files (can be null to use <see cref="DefaultEncoding" />). </param>
        /// <param name="recursive"> Specifies whether subdirectories are searched recursive for resource files or not. </param>
        /// <param name="ignoredExtensions"> A sequence of file extensions which are completely ignored (can be null to not ignore any files). </param>
        /// <exception cref="ArgumentNullException"> <paramref name="directory" /> is null. </exception>
        /// <exception cref="InvalidOperationException"> <paramref name="directory" /> is not a real usable directory. </exception>
        public DirectoryResourceSource (DirectoryPath directory, Encoding fileEncoding, bool recursive, params string[] ignoredExtensions)
            : this(directory, fileEncoding, recursive, (IEnumerable<string>)ignoredExtensions)
        {
        }

        #endregion




        #region Instance Properties/Indexer

        /// <summary>
        ///     Gets the directory which contains the resource set subdirectories.
        /// </summary>
        /// <value>
        ///     The directory which contains the resource set subdirectories.
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
        ///     Gets the set of ignored file extensions.
        /// </summary>
        /// <value>
        ///     The set of ignored file extensions.
        /// </value>
        /// <remarks>
        ///     <note type="note">
        ///         The file extensions in the set have their leading dot removed.
        ///     </note>
        /// </remarks>
        public IEnumerable<string> IgnoredExtensions => this.IgnoredExtensionsInternal;

        /// <summary>
        ///     Gets whether subdirectories are searched recursive for resource files or not.
        /// </summary>
        /// <value>
        ///     true if subdirectories of <see cref="DirectoryPath" /> are searched for resource files, false otherwise.
        /// </value>
        public bool Recursive { get; }

        internal HashSet<string> IgnoredExtensionsInternal { get; }

        internal List<IResourceConverter> Converters { get; private set; }

        private Dictionary<DirectoryPath, DirectoryResourceSet> Sets { get; }

        #endregion




        #region Instance Methods

        private void UpdateSets (bool unload)
        {
            HashSet<DirectoryPath> currentDirectories = unload ? new HashSet<DirectoryPath>() : new HashSet<DirectoryPath>(this.Directory.GetSubdirectories(false, false));
            HashSet<DirectoryPath> lastDirectories = new HashSet<DirectoryPath>(this.Sets.Keys);

            HashSet<DirectoryPath> newDirectories = currentDirectories.Except(lastDirectories);
            HashSet<DirectoryPath> oldDirectories = lastDirectories.Except(currentDirectories);

            foreach (DirectoryPath directory in newDirectories)
            {
                this.Log(LogLevel.Debug, "Directory added: {0}", directory);
                DirectoryResourceSet set = new DirectoryResourceSet(directory, this);
                this.Sets.Add(directory, set);
            }

            foreach (DirectoryPath directory in oldDirectories)
            {
                this.Log(LogLevel.Debug, "Directory removed: {0}", directory);
                this.Sets[directory].Unload();
                this.Sets.Remove(directory);
            }

            foreach (DirectoryPath directory in currentDirectories)
            {
                DirectoryResourceSet set = this.Sets[directory];
                set.Prepare();
                if (!set.IsValid.GetValueOrDefault(false))
                {
                    this.Log(LogLevel.Error, "Unable to use directory as directory resource set: {0}", directory);
                }
            }

            this.Sets.RemoveWhere(x => !x.Value.IsValid.GetValueOrDefault(false));
        }

        #endregion




        #region Interface: IResourceSource

        /// <inheritdoc />
        bool ISynchronizable.IsSynchronized => true;

        /// <inheritdoc />
        public object SyncRoot { get; }

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
        List<IResourceSet> IResourceSource.GetAvailableSets ()
        {
            lock (this.SyncRoot)
            {
                return this.Sets.Values.Cast<IResourceSet>().ToList();
            }
        }

        /// <inheritdoc />
        void IResourceSource.Initialize (IEnumerable<IResourceConverter> converters)
        {
            if (converters == null)
            {
                throw new ArgumentNullException(nameof(converters));
            }

            lock (this.SyncRoot)
            {
                this.Converters = converters.ToList();

                this.Log(LogLevel.Debug, "Initializing directory resource source: {0}", this.Directory);

                this.UpdateSets(false);

                this.IsInitialized = true;
            }
        }

        /// <inheritdoc />
        void IResourceSource.Unload ()
        {
            lock (this.SyncRoot)
            {
                this.Log(LogLevel.Debug, "Unloading directory resource source: {0}", this.Directory);

                this.UpdateSets(true);

                this.IsInitialized = false;
            }
        }

        /// <inheritdoc />
        void IResourceSource.UpdateConverters (IEnumerable<IResourceConverter> converters)
        {
            if (converters == null)
            {
                throw new ArgumentNullException(nameof(converters));
            }

            lock (this.SyncRoot)
            {
                this.Converters = converters.ToList();

                this.UpdateSets(!this.IsInitialized);
            }
        }

        #endregion
    }
}
