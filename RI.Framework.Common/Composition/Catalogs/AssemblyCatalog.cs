﻿using System;
using System.Collections.Generic;
using System.Reflection;

using RI.Framework.Collections.DirectLinq;




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
	public class AssemblyCatalog : CompositionCatalog
	{
		#region Static Methods

		private static IEnumerable<Type> GetAssemblyTypes (IEnumerable<Assembly> assemblies)
		{
			List<Type> types = new List<Type>();

			if (assemblies != null)
			{
				foreach (Assembly assembly in assemblies)
				{
					if (assembly != null)
					{
						types.AddRange(assembly.GetTypes());
					}
				}
			}

			return types;
		}

		#endregion




		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="AssemblyCatalog" />.
		/// </summary>
		/// <param name="assemblies"> The sequence of assemblies whose types are used for composition. </param>
		/// <remarks>
		///     <para>
		///         <paramref name="assemblies" /> is enumerated exactly once.
		///     </para>
		/// </remarks>
		public AssemblyCatalog (IEnumerable<Assembly> assemblies)
		{
			if (assemblies != null)
			{
				foreach (Assembly assembly in assemblies)
				{
					if (assembly != null)
					{
						foreach (Type type in assembly.GetTypes())
						{
							if (CompositionContainer.ValidateExportType(type))
							{
								bool privateExport = CompositionContainer.IsExportPrivate(type).GetValueOrDefault(false);
								HashSet<string> names = CompositionContainer.GetExportsOfType(type, false);
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
		/// <param name="assemblies"> The array of assemblies whose types are used for composition. </param>
		public AssemblyCatalog (params Assembly[] assemblies)
			: this((IEnumerable<Assembly>)assemblies)
		{
		}

		#endregion
	}
}
