using System;
using System.Collections.Generic;
using System.Text;

using RI.Framework.Collections;
using RI.Framework.Collections.DirectLinq;
using RI.Framework.Composition.Model;
using RI.Framework.IO.Paths;
using RI.Framework.Services.Logging;
using RI.Framework.Utilities.Exceptions;




namespace RI.Framework.Services.Resources.Sources
{
	/// <summary>
	///     Implements a resource source which reads from a specified directory
	/// </summary>
	/// <remarks>
	///     <para>
	///         Each subdirectory in the specified directory corresponds to one resource set (<see cref="DirectoryResourceSet" />).
	///         Each file in a subdirectory is read and loaded, if the file extension is known.
	///         Subdirectories of subdirectories (resource sets) are not processed.
	///     </para>
	///     <para>
	///         The following file extensions are known and will be loaded into raw resource values:
	///     </para>
	///     <para>
	///         A special file (<see cref="DirectoryResourceSet.SettingsFileName" />) is expected in each subdirectory (resource set).
	///         It contains descriptions and settings of the corresponding resource set.
	///     </para>
	///     <para>
	///         See <see cref="IResourceSource" /> for more details.
	///     </para>
	/// </remarks>
	[Export]
	public sealed class DirectoryResourceSource : IResourceSource
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

		#endregion




		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="DirectoryResourceSource" />.
		/// </summary>
		/// <param name="directory"> The directory which contains the resource set subdirectories. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="directory" /> is null. </exception>
		/// <exception cref="InvalidPathArgumentException"> <paramref name="directory" /> contains wildcards. </exception>
		/// <remarks>
		///     <para>
		///         The default encoding <see cref="DefaultEncoding" /> is used as the text encoding for the INI file.
		///     </para>
		/// </remarks>
		public DirectoryResourceSource (DirectoryPath directory)
			: this(directory, null)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="DirectoryResourceSource" />.
		/// </summary>
		/// <param name="directory"> The directory which contains the resource set subdirectories. </param>
		/// <param name="fileEncoding"> The text encoding used for reading text files. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="directory" /> is null. </exception>
		/// <exception cref="InvalidPathArgumentException"> <paramref name="directory" /> contains wildcards. </exception>
		public DirectoryResourceSource (DirectoryPath directory, Encoding fileEncoding)
		{
			if (directory == null)
			{
				throw new ArgumentNullException(nameof(directory));
			}

			if (directory.HasWildcards)
			{
				throw new InvalidPathArgumentException(nameof(directory));
			}

			this.Directory = directory;
			this.FileEncoding = fileEncoding ?? DirectoryResourceSource.DefaultEncoding;

			this.Sets = new Dictionary<DirectoryPath, DirectoryResourceSet>();
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets the currently used resource converters of the associated <see cref="IResourceService" />.
		/// </summary>
		/// <value>
		///     The currently used resource converters of the associated <see cref="IResourceService" />.
		/// </value>
		public IEnumerable<IResourceConverter> Converters { get; private set; }

		/// <summary>
		///     Gets the directory which contains the resource set subdirectories.
		/// </summary>
		/// <value>
		///     The directory which contains the resource set subdirectories.
		/// </value>
		public DirectoryPath Directory { get; private set; }

		/// <summary>
		///     Gets the text encoding for reading text files.
		/// </summary>
		/// <value>
		///     The text encoding for reading text files.
		/// </value>
		public Encoding FileEncoding { get; private set; }

		private Dictionary<DirectoryPath, DirectoryResourceSet> Sets { get; set; }

		#endregion




		#region Instance Methods

		private void Log (LogLevel severity, string format, params object[] args)
		{
			LogLocator.Log(severity, this.GetType().Name, format, args);
		}

		private void UpdateSets (bool unload)
		{
			HashSet<DirectoryPath> currentDirectories = unload ? new HashSet<DirectoryPath>() : new HashSet<DirectoryPath>(this.Directory.GetSubdirectories(false, false));
			HashSet<DirectoryPath> lastDirectories = new HashSet<DirectoryPath>(this.Sets.Keys);

			HashSet<DirectoryPath> newDirectories = DirectLinqExtensions.Except(currentDirectories, lastDirectories);
			HashSet<DirectoryPath> oldDirectories = DirectLinqExtensions.Except(lastDirectories, currentDirectories);

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
		public IEnumerable<IResourceSet> AvailableSets => this.Sets.Values;

		/// <inheritdoc />
		public bool IsInitialized { get; private set; }

		/// <inheritdoc />
		void IResourceSource.Initialize (IEnumerable<IResourceConverter> converters)
		{
			if (converters == null)
			{
				throw new ArgumentNullException(nameof(converters));
			}

			this.Converters = converters;

			this.Log(LogLevel.Debug, "Initializing directory resource source: {0}", this.Directory);

			this.UpdateSets(false);

			this.IsInitialized = true;
		}

		/// <inheritdoc />
		void IResourceSource.Unload ()
		{
			this.Log(LogLevel.Debug, "Unloading directory resource source: {0}", this.Directory);

			this.UpdateSets(true);

			this.IsInitialized = false;
		}

		/// <inheritdoc />
		public void UpdateAvailable ()
		{
			this.Log(LogLevel.Debug, "Updating directory resource source: {0}", this.Directory);

			this.UpdateSets(false);
		}

		#endregion
	}
}
