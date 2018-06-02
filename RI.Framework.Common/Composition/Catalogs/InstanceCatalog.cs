using System.Collections.Generic;

using RI.Framework.Collections.DirectLinq;
using RI.Framework.Composition.Model;
using RI.Framework.Utilities.Logging;




namespace RI.Framework.Composition.Catalogs
{
	/// <summary>
	///     Implements a composition catalog which contains instances.
	/// </summary>
	/// <remarks>
	///     <para>
	///         The objects are used for object exporting (see <see cref="CompositionContainer" /> for more details).
	///     </para>
	///     <para>
	///         See <see cref="CompositionCatalog" /> for more details about composition catalogs.
	///     </para>
	/// </remarks>
	/// <threadsafety static="true" instance="true" />
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
		///     <para>
		///         true is used for <see cref="ExportAllTypes" />.
		///     </para>
		/// </remarks>
		public InstanceCatalog (IEnumerable<object> objects)
			: this(true, objects)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="InstanceCatalog" />.
		/// </summary>
		/// <param name="objects"> The array of objects which are used for composition. </param>
		/// <remarks>
		///     <para>
		///         true is used for <see cref="ExportAllTypes" />.
		///     </para>
		/// </remarks>
		public InstanceCatalog (params object[] objects)
			: this(true, objects)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="InstanceCatalog" />.
		/// </summary>
		/// <param name="exportAllTypes"> Specifies whether all types should be exported (see <see cref="ExportAllTypes" /> for details). </param>
		/// <param name="objects"> The sequence of objects which are used for composition. </param>
		/// <remarks>
		///     <para>
		///         <paramref name="objects" /> is enumerated exactly once.
		///     </para>
		/// </remarks>
		public InstanceCatalog (bool exportAllTypes, IEnumerable<object> objects)
		{
			this.ExportAllTypes = exportAllTypes;

			if (objects != null)
			{
				foreach (object obj in objects)
				{
					if (obj != null)
					{
						if (CompositionContainer.ValidateExportInstance(obj))
						{
							HashSet<string> names = CompositionContainer.GetExportsOfType(obj.GetType(), this.ExportAllTypes);
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
						else
						{
							this.Log(LogLevel.Warning, "{0} is not a valid instance for exporting.", obj.GetType().FullName);
						}
					}
				}
			}
		}

		/// <summary>
		///     Creates a new instance of <see cref="InstanceCatalog" />.
		/// </summary>
		/// <param name="exportAllTypes"> Specifies whether all types should be exported (see <see cref="ExportAllTypes" /> for details). </param>
		/// <param name="objects"> The array of objects which are used for composition. </param>
		public InstanceCatalog (bool exportAllTypes, params object[] objects)
			: this(exportAllTypes, (IEnumerable<object>)objects)
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
		///         If all types are exported, the exports will consist of all non-abstract, non-static types, even those without an <see cref="ExportAttribute" />.
		///     </para>
		/// </remarks>
		public bool ExportAllTypes { get; }

		#endregion
	}
}
