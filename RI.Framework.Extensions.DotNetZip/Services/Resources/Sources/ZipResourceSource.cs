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

namespace RI.Framework.Services.Resources.Sources
{
	/// <summary>
	///     Implements a resource source which reads from ZIP files in a specified directory.
	/// </summary>
	/// <remarks>
	///     <para>
	///         Each ZIP file in the specified directory corresponds to one resource set (<see cref="ZipResourceSet" />).
	///         Each file in a ZIP file is read and loaded, if the file extension is known.
	///         Subdirectories in ZIP files (resource sets) are not processed.
	///     </para>
	///     <para>
	///         A special file (<see cref="ZipResourceSet.SettingsFileName" />) is expected in each ZIP file (resource set).
	///         It contains descriptions and settings of the corresponding resource set.
	///     </para>
	///     <para>
	///         See <see cref="IResourceSource" /> for more details.
	///     </para>
	/// </remarks>
	[Export]
	public sealed class ZipResourceSource : LogSource, IResourceSource
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

		/// <summary>
		///     The default file pattern which is used to search for ZIP files.
		/// </summary>
		/// <remarks>
		///     <para>
		///         The default file pattern is <c> *.zip </c>.
		///     </para>
		/// </remarks>
		public const string DefaultFilePattern = "*.zip";

		#endregion




		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="ZipResourceSource" />.
		/// </summary>
		/// <param name="directory"> The directory which contains the resource set ZIP files. </param>
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
		public ZipResourceSource(DirectoryPath directory)
			: this(directory, null, null, false)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="ZipResourceSource" />.
		/// </summary>
		/// <param name="directory"> The directory which contains the resource set ZIP files. </param>
		/// <param name="fileEncoding"> The text encoding used for reading text files (can be null to use <see cref="DefaultEncoding" />). </param>
		/// <param name="filePattern"> The file pattern which is used to search for ZIP files (can be null to use <see cref="DefaultFilePattern" />). </param>
		/// <param name="recursive"> Specifies whether ZIP files are searched recursive (including subdirectories) or not. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="directory" /> is null. </exception>
		/// <exception cref="InvalidOperationException"> <paramref name="directory" /> is not a real usable directory. </exception>
		/// <exception cref="InvalidPathArgumentException"> <paramref name="filePattern" /> is an empty string. </exception>
		public ZipResourceSource(DirectoryPath directory, Encoding fileEncoding, string filePattern, bool recursive)
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

			this.Directory = directory;
			this.FileEncoding = fileEncoding ?? ZipResourceSource.DefaultEncoding;
			this.FilePattern = filePattern ?? ZipResourceSource.DefaultFilePattern;
			this.Recursive = recursive;

			this.Sets = new Dictionary<FilePath, ZipResourceSet>();
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

		internal List<IResourceConverter> Converters { get; private set; }

		private Dictionary<FilePath, ZipResourceSet> Sets { get; }

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
				ZipResourceSet set = new ZipResourceSet(file, this);
				this.Sets.Add(file, set);
			}

			foreach (FilePath file in oldFiles)
			{
				this.Log(LogLevel.Debug, "ZIP file removed: {0}", file);
				this.Sets[file].Unload();
				this.Sets.Remove(file);
			}

			foreach (FilePath file in currentFiles)
			{
				ZipResourceSet set = this.Sets[file];
				set.Prepare();
				if (!set.IsValid.GetValueOrDefault(false))
				{
					this.Log(LogLevel.Error, "Unable to use ZIP file as ZIP resource set: {0}", file);
				}
			}

			this.Sets.RemoveWhere(x => !x.Value.IsValid.GetValueOrDefault(false));
		}

		#endregion




		#region Interface: IResourceSource

		/// <inheritdoc />
		public List<IResourceSet> GetAvailableSets () => this.Sets.Values.Cast<IResourceSet>().ToList();

		/// <inheritdoc />
		public bool IsInitialized { get; private set; }

		/// <inheritdoc />
		void IResourceSource.Initialize (IEnumerable<IResourceConverter> converters)
		{
			if (converters == null)
			{
				throw new ArgumentNullException(nameof(converters));
			}

			this.Converters = converters.ToList();

			this.Log(LogLevel.Debug, "Initializing ZIP resource source: {0}", this.Directory);

			this.UpdateSets(false);

			this.IsInitialized = true;
		}

		/// <inheritdoc />
		void IResourceSource.Unload ()
		{
			this.Log(LogLevel.Debug, "Unloading ZIP resource source: {0}", this.Directory);

			this.UpdateSets(true);

			this.IsInitialized = false;
		}

		/// <inheritdoc />
		public void UpdateConverters (IEnumerable<IResourceConverter> converters)
		{
			if (converters == null)
			{
				throw new ArgumentNullException(nameof(converters));
			}

			this.Converters = converters.ToList();

			this.UpdateSets(!this.IsInitialized);
		}

		#endregion
	}
}
