using System;
using System.Collections;
using System.Collections.Generic;

using RI.Framework.Collections;
using RI.Framework.Collections.DirectLinq;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Composition.Catalogs
{
	/// <summary>
	///     Implements a composition catalog which contains other composition catalogs.
	/// </summary>
	/// <remarks>
	///     <para>
	///         Composition catalogs can be added and removed dynamically.
	///         A recomposition is triggered whenever the contained catalogs change.
	///     </para>
	///     <para>
	///         In addition, the <see cref="CompositionCatalogItem" />s which are collected from all the contained catalogs can be filtered, using <see cref="FilterExport" /> and <see cref="Filter" />.
	///     </para>
	///     <para>
	///         See <see cref="CompositionCatalog" /> for more details about composition catalogs.
	///     </para>
	/// </remarks>
	/// <threadsafety static="true" instance="true" />
	public class AggregateCatalog : CompositionCatalog, ICollection<CompositionCatalog>, ICollection
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="AggregateCatalog" />.
		/// </summary>
		public AggregateCatalog ()
			: this((IEnumerable<CompositionCatalog>)null)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="AggregateCatalog" />.
		/// </summary>
		/// <param name="catalogs"> The sequence of catalogs which are aggregated. </param>
		/// <remarks>
		///     <para>
		///         <paramref name="catalogs" /> is enumerated exactly once.
		///     </para>
		/// </remarks>
		public AggregateCatalog (IEnumerable<CompositionCatalog> catalogs)
		{
			this.ItemsVersion = 0;
			this.Catalogs = new HashSet<CompositionCatalog>();
			this.CatalogRecomposeRequestHandler = this.HandleCatalogRecomposeRequest;

			if (catalogs != null)
			{
				foreach (CompositionCatalog catalog in catalogs)
				{
					this.Add(catalog);
				}
			}
		}

		/// <summary>
		///     Creates a new instance of <see cref="AggregateCatalog" />.
		/// </summary>
		/// <param name="catalogs"> The array of catalogs which are aggregated. </param>
		public AggregateCatalog (params CompositionCatalog[] catalogs)
			: this((IEnumerable<CompositionCatalog>)catalogs)
		{
		}

		#endregion




		#region Instance Properties/Indexer

		private EventHandler CatalogRecomposeRequestHandler { get; }

		private HashSet<CompositionCatalog> Catalogs { get; }

		private int ItemsVersion { get; set; }

		#endregion




		#region Instance Events

		/// <summary>
		///     Raised when a <see cref="CompositionCatalogItem" /> needs to be filtered.
		/// </summary>
		/// <remarks>
		///     <para>
		///         See <see cref="FilterExport" /> for more details.
		///     </para>
		/// </remarks>
		public event EventHandler<AggregateCatalogFilterEventArgs> Filter;

		#endregion




		#region Instance Methods

		private void HandleCatalogRecomposeRequest (object sender, EventArgs e)
		{
			this.RequestRecompose();
		}

		#endregion




		#region Virtuals

		/// <summary>
		///     Called for each <see cref="CompositionCatalogItem" /> aggregated by this catalog to filter exports.
		/// </summary>
		/// <param name="name"> The export name which is filtered. </param>
		/// <param name="item"> The catalog item which is filtered. </param>
		/// <returns>
		///     true if the export is used, false if it is not used.
		/// </returns>
		/// <remarks>
		///     <para>
		///         <see cref="FilterExport" /> is called during <see cref="UpdateItems" /> for each found <see cref="CompositionCatalogItem" /> in the aggregated <see cref="CompositionCatalog" />s.
		///     </para>
		///     <para>
		///         The default implementation raises <see cref="Filter" /> or, if <see cref="Filter" /> has no event handler attached, returns true.
		///     </para>
		/// </remarks>
		protected virtual bool FilterExport (string name, CompositionCatalogItem item)
		{
			EventHandler<AggregateCatalogFilterEventArgs> handler = this.Filter;
			if (handler != null)
			{
				AggregateCatalogFilterEventArgs args = new AggregateCatalogFilterEventArgs(name, item);
				handler(this, args);
				return args.Result;
			}
			return true;
		}

		#endregion




		#region Overrides

		/// <inheritdoc />
		protected internal sealed override void UpdateItems ()
		{
			base.UpdateItems();

			//In case you wonder what this loop is for: it ensures reentrancy (preservation of the manipulated state / the final results) when UpdateItems is called concurrently from multiple threads
			bool cont = true;
			while (cont)
			{
				int itemsVersion;
				List<CompositionCatalog> catalogs = new List<CompositionCatalog>();

				lock (this.SyncRoot)
				{
					this.ItemsVersion += 1;

					itemsVersion = this.ItemsVersion;
					catalogs.AddRange(this.Catalogs);
				}

				Dictionary<string, List<CompositionCatalogItem>> items = new Dictionary<string, List<CompositionCatalogItem>>(CompositionContainer.NameComparer);

				foreach (CompositionCatalog catalog in catalogs)
				{
					catalog.UpdateItems();
				}

				foreach (CompositionCatalog catalog in catalogs)
				{
					foreach (KeyValuePair<string, List<CompositionCatalogItem>> item in catalog.GetItemsSnapshot())
					{
						foreach (CompositionCatalogItem value in item.Value)
						{
							if (!this.FilterExport(item.Key, value))
							{
								continue;
							}

							if (!items.ContainsKey(item.Key))
							{
								items.Add(item.Key, new List<CompositionCatalogItem>());
							}

							if (!items[item.Key].Any(x => x.Type == value.Type))
							{
								items[item.Key].Add(value);
							}
						}
					}
				}

				lock (this.SyncRoot)
				{
					this.Items.Clear();
					this.Items.AddRange(items);

					cont = this.ItemsVersion != itemsVersion;
				}
			}
		}

		#endregion




		#region Interface: ICollection

		/// <inheritdoc />
		bool ICollection.IsSynchronized => ((ISynchronizable)this).IsSynchronized;

		/// <inheritdoc />
		object ICollection.SyncRoot => ((ISynchronizable)this).SyncRoot;

		/// <inheritdoc />
		void ICollection.CopyTo (Array array, int index)
		{
			lock (this.SyncRoot)
			{
				int i1 = 0;
				foreach (CompositionCatalog item in this)
				{
					array.SetValue(item, index + i1);
					i1++;
				}
			}
		}

		#endregion




		#region Interface: ICollection<CompositionCatalog>

		/// <inheritdoc />
		public int Count
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this.Catalogs.Count;
				}
			}
		}

		/// <inheritdoc />
		bool ICollection<CompositionCatalog>.IsReadOnly => false;

		/// <inheritdoc />
		public void Add (CompositionCatalog item)
		{
			if (item == null)
			{
				throw new ArgumentNullException(nameof(item));
			}

			lock (this.SyncRoot)
			{
				if (this.Catalogs.Add(item))
				{
					item.RecomposeRequested += this.CatalogRecomposeRequestHandler;
				}
				else
				{
					return;
				}
			}

			this.RequestRecompose();
		}

		/// <inheritdoc />
		public void Clear ()
		{
			int count;
			lock (this.SyncRoot)
			{
				count = this.Catalogs.Count;

				foreach (CompositionCatalog catalog in this.Catalogs)
				{
					catalog.RecomposeRequested -= this.CatalogRecomposeRequestHandler;
				}

				this.Catalogs.Clear();
			}

			if (count > 0)
			{
				this.RequestRecompose();
			}
		}

		/// <inheritdoc />
		public bool Contains (CompositionCatalog item)
		{
			lock (this.SyncRoot)
			{
				return this.Catalogs.Contains(item);
			}
		}

		/// <inheritdoc />
		void ICollection<CompositionCatalog>.CopyTo (CompositionCatalog[] array, int arrayIndex)
		{
			lock (this.SyncRoot)
			{
				this.Catalogs.CopyTo(array, arrayIndex);
			}
		}

		/// <inheritdoc />
		IEnumerator IEnumerable.GetEnumerator ()
		{
			return this.GetEnumerator();
		}

		/// <inheritdoc />
		public IEnumerator<CompositionCatalog> GetEnumerator ()
		{
			lock (this.SyncRoot)
			{
				return this.Catalogs.GetEnumerator();
			}
		}

		/// <inheritdoc />
		public bool Remove (CompositionCatalog item)
		{
			if (item == null)
			{
				throw new ArgumentNullException(nameof(item));
			}

			bool result;
			lock (this.SyncRoot)
			{
				result = this.Catalogs.Remove(item);
				item.RecomposeRequested -= this.CatalogRecomposeRequestHandler;
			}

			if (result)
			{
				this.RequestRecompose();
			}

			return result;
		}

		#endregion
	}
}
