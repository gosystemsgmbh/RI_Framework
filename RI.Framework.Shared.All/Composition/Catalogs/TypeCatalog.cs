using System;
using System.Collections.Generic;

using RI.Framework.Collections.DirectLinq;
using RI.Framework.Utilities.Logging;
using RI.Framework.Composition.Model;




namespace RI.Framework.Composition.Catalogs
{
	/// <summary>
	///     Implements a composition catalog which contains types.
	/// </summary>
	/// <remarks>
	///     <para>
	///         The types are used for type exporting (see <see cref="CompositionContainer" /> for more details).
	///     </para>
	///     <para>
	///         See <see cref="CompositionCatalog" /> for more details about composition catalogs.
	///     </para>
	/// </remarks>
	/// <threadsafety static="true" instance="true" />
	public class TypeCatalog : CompositionCatalog
	{
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="TypeCatalog" />.
        /// </summary>
        /// <param name="types"> The sequence of types which are used for composition. </param>
        /// <remarks>
        ///     <para>
        ///         <paramref name="types" /> is enumerated exactly once.
        ///     </para>
		///     <para>
		///         true is used for <see cref="ExportAllTypes" />.
		///     </para>
        /// </remarks>
        public TypeCatalog(IEnumerable<Type> types)
            : this(true, types)
        {
        }

        /// <summary>
        ///     Creates a new instance of <see cref="TypeCatalog" />.
        /// </summary>
        /// <param name="types"> The array of types which are used for composition. </param>
		/// <remarks>
		///     <para>
		///         true is used for <see cref="ExportAllTypes" />.
		///     </para>
		/// </remarks>
        public TypeCatalog(params Type[] types)
            : this(true, types)
        {
        }

        /// <summary>
        ///     Creates a new instance of <see cref="TypeCatalog" />.
        /// </summary>
        /// <param name="exportAllTypes"> Specifies whether all types should be exported (see <see cref="ExportAllTypes" /> for details). </param>
        /// <param name="types"> The sequence of types which are used for composition. </param>
        /// <remarks>
        ///     <para>
        ///         <paramref name="types" /> is enumerated exactly once.
        ///     </para>
        /// </remarks>
        public TypeCatalog (bool exportAllTypes, IEnumerable<Type> types)
		{
            this.ExportAllTypes = exportAllTypes;

            if (types != null)
			{
				foreach (Type type in types)
				{
					if (type != null)
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
						else
						{
							this.Log(LogLevel.Warning, "{0} is not a valid type for exporting.", type.FullName);
						}
					}
				}
			}
		}

        /// <summary>
        ///     Creates a new instance of <see cref="TypeCatalog" />.
        /// </summary>
        /// <param name="exportAllTypes"> Specifies whether all types should be exported (see <see cref="ExportAllTypes" /> for details). </param>
        /// <param name="types"> The array of types which are used for composition. </param>
        public TypeCatalog (bool exportAllTypes, params Type[] types)
			: this(exportAllTypes, (IEnumerable<Type>)types)
		{
        }

        /// <summary>
		///     Gets whether all types should be exported.
		/// </summary>
		/// <value>
		///     true if all types should be exported, false otherwise.
		/// </value>
		/// <remarks>
		///     <para>
		///         If all types are exported, the exports will consist of all non-abstract, non-static types, even those without an <see cref="ExportAttribute" />.
		///     </para>
		/// </remarks>
		public bool ExportAllTypes { get; }

        #endregion
    }
}
