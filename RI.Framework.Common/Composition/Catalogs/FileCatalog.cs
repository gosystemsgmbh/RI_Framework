using System;
using System.Collections.Generic;
using System.Reflection;

using RI.Framework.Collections.DirectLinq;
using RI.Framework.Composition.Model;
using RI.Framework.IO.Paths;
using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;
using RI.Framework.Utilities.Logging;

namespace RI.Framework.Composition.Catalogs
{
	/// <summary>
	///     Implements a composition catalog which loads a single assembly file and includes it in the catalog for composition.
	/// </summary>
	/// <remarks>
	///     <para>
	///         The types of the assembly file are used for type exporting (see <see cref="CompositionContainer" /> for more details).
	///     </para>
	///     <para>
	///         See <see cref="CompositionCatalog" /> for more details about composition catalogs.
	///     </para>
	///     <note type="note">
	///         The assembly file is only attempted to be loaded once.
	///         If the load fails, it will not be attempted again.
	///     </note>
	/// </remarks>
	/// <threadsafety static="true" instance="true" />
	public sealed class FileCatalog : CompositionCatalog
	{
		#region Static Methods

		internal static Dictionary<string, List<CompositionCatalogItem>> LoadAssemblyFile (FilePath file, bool exportAllTypes)
		{
			if (file == null)
			{
				throw new ArgumentNullException(nameof(file));
			}

			if (!file.IsRealFile)
			{
				throw new InvalidPathArgumentException(nameof(file));
			}

			Dictionary<string, List<CompositionCatalogItem>> items = new Dictionary<string, List<CompositionCatalogItem>>();
			Assembly assembly = Assembly.LoadFrom(file);
			Type[] types = assembly.GetTypes();
			foreach (Type type in types)
			{
				if (CompositionContainer.ValidateExportType(type))
				{
					bool privateExport = CompositionContainer.IsExportPrivate(type).GetValueOrDefault(false);
					HashSet<string> names = CompositionContainer.GetExportsOfType(type, exportAllTypes);
					foreach (string name in names)
					{
						if (!items.ContainsKey(name))
						{
							items.Add(name, new List<CompositionCatalogItem>());
						}

						if (!items[name].Any(x => x.Type == type))
						{
							items[name].Add(new CompositionCatalogItem(name, type, privateExport));
						}
					}
				}
			}
			return items;
		}

		#endregion




		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="FileCatalog" />.
		/// </summary>
		/// <param name="file"> The assembly file to load. </param>
		/// <param name="exportAllTypes"> Specifies whether all types should be exported (see <see cref="ExportAllTypes" /> for details). </param>
		/// <exception cref="ArgumentNullException"> <paramref name="file" /> is null. </exception>
		/// <exception cref="InvalidPathArgumentException"> <paramref name="file" /> is not a real usable file. </exception>
		public FileCatalog (FilePath file, bool exportAllTypes)
		{
			if (file == null)
			{
				throw new ArgumentNullException(nameof(file));
			}

			if (!file.IsRealFile)
			{
				throw new InvalidPathArgumentException(nameof(file));
			}

			this.File = file;
			this.ExportAllTypes = exportAllTypes;

			this.IsLoaded = false;
			this.Failed = false;
		}

		#endregion




		#region Instance Fields

		private bool _failed;

		#endregion




		#region Instance Properties/Indexer

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
		///     Indicates whether the assembly file was successfully loaded.
		/// </summary>
		/// <value>
		///     true if the assembly filefailed to load, false otherwise.
		/// </value>
		public bool Failed
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this._failed;
				}
			}
			private set
			{
				lock (this.SyncRoot)
				{
					this._failed = value;
				}
			}
		}

		/// <summary>
		///     Gets the used assembly file.
		/// </summary>
		/// <value>
		///     The used assembly file.
		/// </value>
		public FilePath File { get; }

		private bool IsLoaded { get; set; }

		#endregion




		#region Overrides

		/// <inheritdoc />
		protected internal override void UpdateItems ()
		{
			base.UpdateItems();

			lock (this.SyncRoot)
			{
				if (this.IsLoaded)
				{
					return;
				}

				this.IsLoaded = true;

				this.Log(LogLevel.Debug, "Trying to load assembly: {0}", this.File);

				try
				{
					Dictionary<string, List<CompositionCatalogItem>> items = FileCatalog.LoadAssemblyFile(this.File, this.ExportAllTypes);
					foreach (KeyValuePair<string, List<CompositionCatalogItem>> item in items)
					{
						this.Items.Add(item.Key, item.Value);
					}
					this.Failed = false;
				}
				catch (Exception exception)
				{
					this.Log(LogLevel.Error, "Load assembly failed: {0}{1}{2}", this.File, Environment.NewLine, exception.ToDetailedString());
					this.Failed = true;
				}
			}
		}

		#endregion
	}
}
