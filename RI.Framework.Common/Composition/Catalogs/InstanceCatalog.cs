using System.Collections.Generic;

using RI.Framework.Collections.Linq;




namespace RI.Framework.Composition.Catalogs
{
	/// <summary>
	///     Implements a composition catalog which contains objects.
	/// </summary>
	/// <remarks>
	///     <para>
	///         The objects are used for object exporting (see <see cref="CompositionContainer" /> for more details).
	///     </para>
	///     <para>
	///         See <see cref="CompositionCatalog" /> for more details about composition catalogs.
	///     </para>
	/// </remarks>
	public class InstanceCatalog : CompositionCatalog
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="InstanceCatalog" />.
		/// </summary>
		/// <param name="objects"> The sequence of objects which are used for composition. </param>
		/// <remarks>
		///     <para>
		///         <paramref name="objects" /> is enumerated exactly once.
		///     </para>
		/// </remarks>
		public InstanceCatalog (IEnumerable<object> objects)
		{
			if (objects != null)
			{
				foreach (object obj in objects)
				{
					if (obj != null)
					{
						if (CompositionContainer.ValidateExportInstance(obj))
						{
							HashSet<string> names = CompositionContainer.GetExportsOfType(obj.GetType());
							foreach (string name in names)
							{
								if (!this.Items.ContainsKey(name))
								{
									this.Items.Add(name, new List<CompositionCatalogItem>());
								}

								if (!this.Items[name].Any(x => object.ReferenceEquals(x.Value, obj)))
								{
									this.Items[name].Add(new CompositionCatalogItem(name, obj));
								}
							}
						}
					}
				}
			}
		}

		/// <summary>
		///     Creates a new instance of <see cref="InstanceCatalog" />.
		/// </summary>
		/// <param name="objects"> The array of objects which are used for composition. </param>
		public InstanceCatalog (params object[] objects)
			: this((IEnumerable<object>)objects)
		{
		}

		#endregion
	}
}
