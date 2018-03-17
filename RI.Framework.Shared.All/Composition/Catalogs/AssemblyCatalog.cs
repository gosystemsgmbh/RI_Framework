using System;
using System.Collections.Generic;
using System.Reflection;

using RI.Framework.Collections.DirectLinq;
using RI.Framework.Composition.Model;
using RI.Framework.Utilities.Logging;




namespace RI.Framework.Composition.Catalogs
{
	/// <summary>
	///     Implements a composition catalog which contains assemblies.
	/// </summary>
	/// <remarks>
	///     <para>
	///         The types of the assemblies are used for type exporting (see <see cref="CompositionContainer" /> for more details).
	///     </para>
	///     <para>
	///         See <see cref="CompositionCatalog" /> for more details about composition catalogs.
	///     </para>
	/// </remarks>
	/// <threadsafety static="true" instance="true" />
	public class AssemblyCatalog : CompositionCatalog
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="AssemblyCatalog" />.
		/// </summary>
		/// <param name="assemblies"> The sequence of assemblies whose types are used for composition. </param>
		/// <remarks>
		///     <para>
		///         <paramref name="assemblies" /> is enumerated exactly once.
		///     </para>
		///     <para>
		///         true is used for <see cref="ExportAllTypes" />.
		///     </para>
		/// </remarks>
		public AssemblyCatalog (IEnumerable<Assembly> assemblies)
			: this(true, assemblies)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="AssemblyCatalog" />.
		/// </summary>
		/// <param name="assemblies"> The array of assemblies whose types are used for composition. </param>
		/// <remarks>
		///     <para>
		///         true is used for <see cref="ExportAllTypes" />.
		///     </para>
		/// </remarks>
		public AssemblyCatalog (params Assembly[] assemblies)
			: this(true, assemblies)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="AssemblyCatalog" />.
		/// </summary>
		/// <param name="exportAllTypes"> Specifies whether all types should be exported (see <see cref="ExportAllTypes" /> for details). </param>
		/// <param name="assemblies"> The sequence of assemblies whose types are used for composition. </param>
		/// <remarks>
		///     <para>
		///         <paramref name="assemblies" /> is enumerated exactly once.
		///     </para>
		/// </remarks>
		public AssemblyCatalog (bool exportAllTypes, IEnumerable<Assembly> assemblies)
		{
			this.ExportAllTypes = exportAllTypes;

			if (assemblies != null)
			{
				foreach (Assembly assembly in assemblies)
				{
					if (assembly != null)
					{
						AssemblyName assemblyName = assembly.GetName();
						this.Log(LogLevel.Debug, "Loading assembly: {0}", assemblyName.FullName);
						foreach (Type type in assembly.GetTypes())
						{
							if (CompositionContainer.ValidateExportType(type))
							{
								bool privateExport = CompositionContainer.IsExportPrivate(type).GetValueOrDefault(false);
								HashSet<string> names = CompositionContainer.GetExportsOfType(type, this.ExportAllTypes);
								foreach (string name in names)
								{
									if (!this.Items.ContainsKey(name))
									{
										this.Items.Add(name, new List<CompositionCatalogItem>());
									}

									if (!this.Items[name].Any(x => x.Type == type))
									{
										this.Items[name].Add(new CompositionCatalogItem(name, type, privateExport));
									}
								}
							}
						}
					}
				}
			}
		}

		/// <summary>
		///     Creates a new instance of <see cref="AssemblyCatalog" />.
		/// </summary>
		/// <param name="exportAllTypes"> Specifies whether all types should be exported (see <see cref="ExportAllTypes" /> for details). </param>
		/// <param name="assemblies"> The array of assemblies whose types are used for composition. </param>
		public AssemblyCatalog (bool exportAllTypes, params Assembly[] assemblies)
			: this(exportAllTypes, (IEnumerable<Assembly>)assemblies)
		{
		}

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

		#endregion
	}
}
