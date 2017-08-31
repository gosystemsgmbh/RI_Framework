using System;
using System.Collections.Generic;

using RI.Framework.Collections;
using RI.Framework.Collections.DirectLinq;
using RI.Framework.Composition.Model;
using RI.Framework.IO.Paths;
using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;
using RI.Framework.Utilities.Logging;
using RI.Framework.Utilities.Runtime;

namespace RI.Framework.Composition.Catalogs
{
	/// <summary>
	///     Implements a composition catalog which searches a directory for assembly files and includes them in the catalog for composition.
	/// </summary>
	/// <remarks>
	///     <para>
	///         The types of the assemblies found in the directory are used for type exporting (see <see cref="CompositionContainer" /> for more details).
	///     </para>
	///     <para>
	///         See <see cref="CompositionCatalog" /> for more details about composition catalogs.
	///     </para>
	///     <note type="note">
	///         Changes in the specified directory are not detected automatically.
	///         Changes must be applied by calling <see cref="Reload" />.
	///     </note>
	///     <note type="note">
	///         Assemblies which were loaded cannot be unloaded.
	///         Similarly, assemblies which failed to load will not be attempted to be loaded again.
	///     </note>
	/// </remarks>
	/// <threadsafety static="true" instance="true" />
	public sealed class DirectoryCatalog : CompositionCatalog
	{
		#region Constants

		/// <summary>
		///     The default file pattern which is used to search for assembly files.
		/// </summary>
		/// <remarks>
		///     <para>
		///         The default file pattern is <c> *.dll </c>.
		///     </para>
		/// </remarks>
		public const string DefaultFilePattern = "*.dll";

		#endregion




		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="DirectoryCatalog" />.
		/// </summary>
		/// <param name="directoryPath"> The directory which is searched for assemblies. </param>
		/// <param name="exportAllTypes"> Specifies whether all types should be exported (see <see cref="ExportAllTypes" /> for details). </param>
		/// <param name="ignoreFrameworkFiles"> Specifies whether framework-provided files should be exported (see <see cref="IgnoreFrameworkFiles" /> for details). </param>
		/// <remarks>
		///     <para>
		///         The default file pattern <see cref="DefaultFilePattern" /> is used and search is performed non-recursive.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="directoryPath" /> is null. </exception>
		/// <exception cref="InvalidPathArgumentException"> <paramref name="directoryPath" /> is not a real usable directory. </exception>
		public DirectoryCatalog (DirectoryPath directoryPath, bool exportAllTypes, bool ignoreFrameworkFiles)
			: this(directoryPath, exportAllTypes, ignoreFrameworkFiles, DirectoryCatalog.DefaultFilePattern, false)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="DirectoryCatalog" />.
		/// </summary>
		/// <param name="directoryPath"> The directory which is searched for assemblies. </param>
		/// <param name="exportAllTypes"> Specifies whether all types should be exported (see <see cref="ExportAllTypes" /> for details). </param>
		/// <param name="ignoreFrameworkFiles"> Specifies whether framework-provided files should be exported (see <see cref="IgnoreFrameworkFiles" /> for details). </param>
		/// <param name="filePattern"> The file pattern which is used to search for assemblies (can be null to use <see cref="DefaultFilePattern" />). </param>
		/// <param name="recursive"> Specifies whether assemblies are searched recursive (including subdirectories) or not. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="directoryPath" /> is null. </exception>
		/// <exception cref="InvalidOperationException"> <paramref name="directoryPath" /> is not a real usable directory. </exception>
		/// <exception cref="InvalidPathArgumentException"> <paramref name="filePattern" /> is an empty string. </exception>
		public DirectoryCatalog (DirectoryPath directoryPath, bool exportAllTypes, bool ignoreFrameworkFiles, string filePattern, bool recursive)
		{
			if (directoryPath == null)
			{
				throw new ArgumentNullException(nameof(directoryPath));
			}

			if (!directoryPath.IsRealDirectory)
			{
				throw new InvalidPathArgumentException(nameof(directoryPath));
			}

			if (filePattern != null)
			{
				if (filePattern.IsEmptyOrWhitespace())
				{
					throw new EmptyStringArgumentException(nameof(filePattern));
				}
			}

			this.DirectoryPath = directoryPath;
			this.ExportAllTypes = exportAllTypes;
			this.IgnoreFrameworkFiles = ignoreFrameworkFiles;
			this.FilePattern = filePattern ?? DirectoryCatalog.DefaultFilePattern;
			this.Recursive = recursive;

			this.LoadedFiles = new HashSet<FilePath>();
			this.FailedFiles = new HashSet<FilePath>();
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets the directory which is searched for assemblies.
		/// </summary>
		/// <value>
		///     The directory which is searched for assemblies.
		/// </value>
		public DirectoryPath DirectoryPath { get; }

		/// <summary>
		///     Gets whether all types should be exported.
		/// </summary>
		/// <value>
		///     true if all types should be exported, false otherwise.
		/// </value>
		/// <remarks>
		///     <para>
		///         If all types are exported, the exports will consist of all public, non-abstract, non-static types, even those without an <see cref="ExportAttribute" />.
		///     </para>
		/// </remarks>
		public bool ExportAllTypes { get; }

		/// <summary>
		/// Gets whether files provided by the framework itself are ignored.
		/// </summary>
		/// <value>
		/// true if files provided by the framework itself are ignored, false otherwise.
		/// </value>
		/// <remarks>
		/// <para>
		/// If framework-provided files are ignored, their types are not exported by default.
		/// However, you can still export those types by explicit export them, e.g. through <see cref="AssemblyCatalog"/>, <see cref="TypeCatalog"/>, or <see cref="InstanceCatalog"/>.
		/// </para>
		/// </remarks>
		public bool IgnoreFrameworkFiles { get; }

		/// <summary>
		///     Indicates whether there were assembly files which could not be loaded.
		/// </summary>
		/// <value>
		///     true if there was at least one assembly file which could not be loaded, false otherwise.
		/// </value>
		/// <remarks>
		///     <para>
		///         The list of failed assembly files which could not be loaded can be retrieved using <see cref="GetFailedFiles" />.
		///     </para>
		/// </remarks>
		public bool Failed
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this.FailedFiles.Count > 0;
				}
			}
		}

		/// <summary>
		///     Gets the file pattern which is used to search for assemblies.
		/// </summary>
		/// <value>
		///     The file pattern which is used to search for assemblies.
		/// </value>
		public string FilePattern { get; }

		/// <summary>
		///     Gets whether assemblies are searched recursive (including subdirectories) or not.
		/// </summary>
		/// <value>
		///     true if subdirectories of <see cref="DirectoryPath" /> are searched for assemblies, false otherwise.
		/// </value>
		public bool Recursive { get; }

		private HashSet<FilePath> FailedFiles { get; set; }

		private HashSet<FilePath> LoadedFiles { get; set; }

		#endregion




		#region Instance Methods

		/// <summary>
		///     Gets all the assembly files which failed to load.
		/// </summary>
		/// <returns>
		///     The array with the assembly files which failed to load.
		///     The array is empty if there are no assembly files which failed to load.
		/// </returns>
		public FilePath[] GetFailedFiles ()
		{
			lock (this.SyncRoot)
			{
				return this.FailedFiles.ToArray();
			}
		}

		/// <summary>
		///     Gets all the successfully loaded assembly files.
		/// </summary>
		/// <returns>
		///     The array with the successfully loaded assembly files.
		///     The array is empty if there are no successfully loaded assembly files.
		/// </returns>
		public FilePath[] GetLoadedFiles ()
		{
			lock (this.SyncRoot)
			{
				return this.LoadedFiles.ToArray();
			}
		}

		/// <summary>
		///     Checks the associated directory for new assemblies and loads them.
		/// </summary>
		public void Reload ()
		{
			this.RequestRecompose();
		}

		#endregion




		#region Overrides

		/// <inheritdoc />
		protected internal override void UpdateItems ()
		{
			base.UpdateItems();

			lock (this.SyncRoot)
			{
				HashSet<FilePath> allFiles = new HashSet<FilePath>(this.DirectoryPath.GetFiles(false, this.Recursive, this.FilePattern), this.LoadedFiles.Comparer);
				HashSet<FilePath> newFiles = allFiles.Except(this.LoadedFiles, this.LoadedFiles.Comparer).Except(this.FailedFiles, this.LoadedFiles.Comparer);
				HashSet<FilePath> suceededFiles = new HashSet<FilePath>(this.LoadedFiles.Comparer);
				HashSet<FilePath> failedFiles = new HashSet<FilePath>(this.LoadedFiles.Comparer);

				foreach (FilePath newFile in newFiles)
				{
					this.Log(LogLevel.Debug, "Trying to load assembly: {0}", newFile);

					bool isFrameworkFile = FrameworkTypeUtility.IsFrameworkFile(newFile);
					if (isFrameworkFile && this.IgnoreFrameworkFiles)
					{
						this.Log(LogLevel.Debug, "Framework-provided file filtered: {0}", newFile);
						continue;
					}

					try
					{
						Dictionary<string, List<CompositionCatalogItem>> items = FileCatalog.LoadAssemblyFile(newFile, this.ExportAllTypes);
						foreach (KeyValuePair<string, List<CompositionCatalogItem>> item in items)
						{
							if (!this.Items.ContainsKey(item.Key))
							{
								this.Items.Add(item.Key, new List<CompositionCatalogItem>());
							}

							foreach (CompositionCatalogItem value in item.Value)
							{
								if (!this.Items[item.Key].Any(x => x.Type == value.Type))
								{
									this.Items[item.Key].Add(value);
								}
							}
						}

						suceededFiles.Add(newFile);
					}
					catch (Exception exception)
					{
						this.Log(LogLevel.Error, "Load assembly failed: {0}{1}{2}", newFile, Environment.NewLine, exception.ToDetailedString());

						failedFiles.Add(newFile);
					}
				}

				this.LoadedFiles.AddRange(suceededFiles);
				this.FailedFiles.AddRange(failedFiles);
			}
		}

		#endregion
	}
}
