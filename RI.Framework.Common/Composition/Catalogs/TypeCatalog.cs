using System;
using System.Collections.Generic;

using RI.Framework.Collections.Linq;




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
		/// </remarks>
		public TypeCatalog (IEnumerable<Type> types)
		{
			if (types != null)
			{
				foreach (Type type in types)
				{
					if (type != null)
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
				}
			}
		}

		/// <summary>
		///     Creates a new instance of <see cref="TypeCatalog" />.
		/// </summary>
		/// <param name="types"> The array of types which are used for composition. </param>
		public TypeCatalog (params Type[] types)
			: this((IEnumerable<Type>)types)
		{
		}

		#endregion
	}
}
