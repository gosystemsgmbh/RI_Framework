using System;

using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;




namespace RI.Framework.Composition.Catalogs
{
	/// <summary>
	///     Event arguments for the <see cref="AggregateCatalog" />.<see cref="AggregateCatalog.Filter" /> event.
	/// </summary>
	[Serializable]
	public sealed class AggregateCatalogFilterEventArgs : EventArgs
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="AggregateCatalogFilterEventArgs" />.
		/// </summary>
		/// <param name="name"> The export name which is filtered. </param>
		/// <param name="item"> The catalog item which is filtered. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="name" /> or <paramref name="item" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="name" /> is an empty string. </exception>
		public AggregateCatalogFilterEventArgs (string name, CompositionCatalogItem item)
		{
			if (name == null)
			{
				throw new ArgumentNullException(nameof(name));
			}

			if (name.IsNullOrEmptyOrWhitespace())
			{
				throw new EmptyStringArgumentException(nameof(name));
			}

			if (item == null)
			{
				throw new ArgumentNullException(nameof(item));
			}

			this.Name = name;
			this.Item = item;

			this.Result = true;
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets the catalog item which is filtered.
		/// </summary>
		/// <value>
		///     The catalog item which is filtered.
		/// </value>
		public CompositionCatalogItem Item { get; }

		/// <summary>
		///     Gets the export name which is filtered.
		/// </summary>
		/// <value>
		///     The export name which is filtered.
		/// </value>
		public string Name { get; }

		/// <summary>
		///     Gets or sets the filter result.
		/// </summary>
		/// <value>
		///     true if the export is used, false otherwise.
		/// </value>
		/// <remarks>
		///     <para>
		///         The default value is true.
		///     </para>
		/// </remarks>
		public bool Result { get; set; }

		#endregion
	}
}
