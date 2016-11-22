using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using RI.Framework.Collections;
using RI.Framework.Collections.Linq;
using RI.Framework.Services.Logging;
using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;




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
	///         Assembly which were loaded cannot be unloaded.
	///         Similarly, assembly which failed to load will not be attempted to be loaded again.
	///     </note>
	/// </remarks>
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
		/// <remarks>
		///     <para>
		///         The default file pattern <see cref="DefaultFilePattern" /> is used and search is performed non-recursive.
		///     </para>
		/// </remarks>
		public DirectoryCatalog (string directoryPath)
			: this(directoryPath, DirectoryCatalog.DefaultFilePattern, false)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="DirectoryCatalog" />.
		/// </summary>
		/// <param name="directoryPath"> The directory which is searched for assemblies. </param>
		/// <param name="filePattern"> The file pattern which is used to search for assemblies. </param>
		/// <param name="recursive"> Specifies whether assemblies are searched recursive (including subdirectories) or not. </param>
		public DirectoryCatalog (string directoryPath, string filePattern, bool recursive)
		{
			if (directoryPath == null)
			{
				throw new ArgumentNullException(nameof(directoryPath));
			}

			if (directoryPath.IsEmpty())
			{
				throw new EmptyStringArgumentException(nameof(directoryPath));
			}

			if (filePattern == null)
			{
				throw new ArgumentNullException(nameof(filePattern));
			}

			if (filePattern.IsEmpty())
			{
				throw new EmptyStringArgumentException(nameof(filePattern));
			}

			this.DirectoryPath = directoryPath;
			this.FilePattern = filePattern;
			this.Recursive = recursive;

			this.LoadedFiles = new HashSet<string>(StringComparerEx.InvariantCultureIgnoreCase);
			this.FailedFiles = new HashSet<string>(this.LoadedFiles.Comparer);
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets the directory which is searched for assemblies.
		/// </summary>
		/// <value>
		///     The directory which is searched for assemblies.
		/// </value>
		public string DirectoryPath { get; private set; }

		/// <summary>
		///     Gets the file pattern which is used to search for assemblies.
		/// </summary>
		/// <value>
		///     The file pattern which is used to search for assemblies.
		/// </value>
		public string FilePattern { get; private set; }

		/// <summary>
		///     Gets whether assemblies are searched recursive (including subdirectories) or not.
		/// </summary>
		/// <value>
		///     true if subdirectories of <see cref="DirectoryPath" /> are searched for assemblies, false otherwise.
		/// </value>
		public bool Recursive { get; private set; }

		private HashSet<string> FailedFiles { get; set; }

		private HashSet<string> LoadedFiles { get; set; }

		#endregion




		#region Instance Methods

		/// <summary>
		///     Gets all the assembly files which failed to load.
		/// </summary>
		/// <returns>
		///     The array with the assembly files which failed to load.
		///     The array is empty if there are no assembly files which failed to load.
		/// </returns>
		public string[] GetFailedFiles ()
		{
			return this.FailedFiles.ToArray();
		}

		/// <summary>
		///     Gets all the successfully loaded assembly files.
		/// </summary>
		/// <returns>
		///     The array with the successfully loaded assembly files.
		///     The array is empty if there are no successfully loaded assembly files.
		/// </returns>
		public string[] GetLoadedFiles ()
		{
			return this.LoadedFiles.ToArray();
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

			HashSet<string> allFiles = new HashSet<string>(Directory.GetFiles(this.DirectoryPath, this.FilePattern, this.Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly), this.LoadedFiles.Comparer);
			HashSet<string> newFiles = allFiles.Except(this.LoadedFiles, this.LoadedFiles.Comparer).Except(this.FailedFiles, this.LoadedFiles.Comparer);
			HashSet<string> suceededFiles = new HashSet<string>(this.LoadedFiles.Comparer);
			HashSet<string> failedFiles = new HashSet<string>(this.LoadedFiles.Comparer);

			foreach (string newFile in newFiles)
			{
				this.Log(LogLevel.Debug, "Trying to load assembly: {0}", newFile);

				try
				{
					Assembly assembly = Assembly.LoadFrom(newFile);
					Type[] types = assembly.GetTypes();
					foreach (Type type in types)
					{
						if (CompositionContainer.ValidateExportType(type))
						{
							HashSet<string> names = CompositionContainer.GetExportsOfType(type);
							foreach (string name in names)
							{
								if (!this.Items.ContainsKey(name))
								{
									this.Items.Add(name, new List<CompositionCatalogItem>());
								}

								if (!this.Items[name].Any(x => x.Type == type))
								{
									this.Items[name].Add(new CompositionCatalogItem(name, type));
								}
							}
						}
					}
					suceededFiles.Add(newFile);
				}
				catch (Exception exception)
				{
					this.Log(LogLevel.Warning, "Load assembly failed: {0}{1}{2}", newFile, Environment.NewLine, exception.ToDetailedString());

					failedFiles.Add(newFile);
				}
			}

			this.LoadedFiles.AddRange(suceededFiles);
			this.FailedFiles.AddRange(failedFiles);
		}

		#endregion
	}
}
