using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;




namespace RI.Framework.Collections.Virtualization
{
	/// <summary>
	///     Implements a list which uses data virtualization to load data (the items in the collection) on demand.
	/// </summary>
	/// <typeparam name="T"> The type of items virtualized. </typeparam>
	/// <remarks>
	///     <para>
	///         <see cref="VirtualizationCollection{T}" /> uses an <see cref="IItemsProvider{T}" /> to load items on-demand.
	///         That means that items are only loaded when they are actually requested through <see cref="VirtualizationCollection{T}" /> (e.g. by using the collections indexer property).
	///     </para>
	///     <para>
	///         Items are loaded in pages which size can be specified when cosntructing <see cref="VirtualizationCollection{T}" />.
	///         The loaded pages will then stay in the cache for a specified amount of time.
	///     </para>
	///     <note type="note">
	///         If the used <see cref="IItemsProvider{T}" /> uses <see cref="IItemsProvider{T}.ItemsChanged" />, the cache will be cleared.
	///     </note>
	/// </remarks>
	public sealed class VirtualizationCollection <T> : IList<T>, ICollection<T>, IEnumerable<T>, IList, ICollection, IEnumerable, IDisposable, IItemsProvider<T>
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="VirtualizationCollection{T}" />
		/// </summary>
		/// <param name="pageSize"> The page size. </param>
		/// <param name="cacheTime"> The time in milliseconds pages stay in the cache. </param>
		/// <param name="itemsProvider"> The provider which is used to load the items as needed. </param>
		/// <exception cref="ArgumentOutOfRangeException"> <paramref name="pageSize" /> is less than 1 or <paramref name="cacheTime" /> is negative. </exception>
		/// <exception cref="ArgumentNullException"> <paramref name="itemsProvider" /> is null. </exception>
		public VirtualizationCollection (int pageSize, TimeSpan? cacheTime, IItemsProvider<T> itemsProvider)
			: this(pageSize, cacheTime == null ? 0 : (int)cacheTime.Value.TotalMilliseconds, itemsProvider)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="VirtualizationCollection{T}" />
		/// </summary>
		/// <param name="pageSize"> The page size. </param>
		/// <param name="cacheTime"> The time in milliseconds pages stay in the cache. </param>
		/// <param name="itemsProvider"> The provider which is used to load the items as needed. </param>
		/// <exception cref="ArgumentOutOfRangeException"> <paramref name="pageSize" /> is less than 1 or <paramref name="cacheTime" /> is negative. </exception>
		/// <exception cref="ArgumentNullException"> <paramref name="itemsProvider" /> is null. </exception>
		public VirtualizationCollection (int pageSize, int cacheTime, IItemsProvider<T> itemsProvider)
		{
			if (pageSize < 1)
			{
				throw new ArgumentOutOfRangeException(nameof(pageSize));
			}

			if (cacheTime < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(cacheTime));
			}

			if (itemsProvider == null)
			{
				throw new ArgumentNullException(nameof(itemsProvider));
			}

			this.PageSize = pageSize;
			this.CacheTime = cacheTime;
			this.ItemsProvider = itemsProvider;

			this.SyncRoot = new object();
			this.Cache = new PageCollection();
			this.ItemsChangedHandler = this.ItemsChangedMethod;

			this.ItemsProvider.ItemsChanged += this.ItemsChangedHandler;
		}

		/// <summary>
		///     Garbage collects this instance of <see cref="VirtualizationCollection{T}" />.
		/// </summary>
		~VirtualizationCollection ()
		{
			this.Dispose();
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets the time in milliseconds pages stay in the cache.
		/// </summary>
		/// <value>
		///     The time in milliseconds pages stay in the cache.
		/// </value>
		public int CacheTime { get; private set; }

		/// <summary>
		///     Gets the page size.
		/// </summary>
		/// <value>
		///     The page size.
		/// </value>
		public int PageSize { get; private set; }

		private PageCollection Cache { get; set; }

		private EventHandler ItemsChangedHandler { get; set; }

		private IItemsProvider<T> ItemsProvider { get; set; }

		private object SyncRoot { get; set; }

		#endregion




		#region Instance Events

		/// <summary>
		///     Raised when the used <see cref="IItemsProvider{T}" /> signalled that items have changed.
		/// </summary>
		public event EventHandler ItemsChanged;

		private event EventHandler ProviderItemsChanged;

		#endregion




		#region Instance Methods

		/// <summary>
		///     Clears the cache by removing all cached pages.
		/// </summary>
		public void ClearCache ()
		{
			this.VerifyNotDisposed();
			this.Cache.Clear();
		}

		private void CleanupCache ()
		{
			DateTime now = DateTime.UtcNow;
			this.Cache.RemoveWhere(x => now.Subtract(x.Timestamp).TotalMilliseconds > this.CacheTime);
		}

		private void ItemsChangedMethod (object sender, EventArgs args)
		{
			this.ClearCache();

			this.ItemsChanged?.Invoke(this, EventArgs.Empty);
			this.ProviderItemsChanged?.Invoke(this, EventArgs.Empty);
		}

		private Page LoadPage (int pageIndex, bool temporary)
		{
			Page page = null;
			bool load = false;

			if ((!temporary) && this.Cache.Contains(pageIndex))
			{
				page = this.Cache[pageIndex];
				load = false;
			}
			else
			{
				page = new Page(pageIndex);
				load = true;
			}

			if (load)
			{
				int start = pageIndex * this.PageSize;
				int count = this.PageSize;

				IEnumerable<T> enumerator = this.ItemsProvider.GetItems(start, count);

				if (enumerator == null)
				{
					page = null;
				}
				else
				{
					page.Items.Clear();
					page.Items.AddRange(enumerator);
					if (page.Items.Count == 0)
					{
						page = null;
					}
				}
			}

			if (!temporary)
			{
				this.Cache.Remove(pageIndex);

				if (page != null)
				{
					this.Cache.Add(page);
				}
			}

			return page;
		}

		private void ThrowReadOnlyException ()
		{
			throw new NotSupportedException("A virtualization collection is read-only.");
		}

		private void VerifyNotDisposed ()
		{
			if (this.ItemsProvider == null)
			{
				throw new ObjectDisposedException(this.GetType().Name);
			}
		}

		#endregion




		#region Interface: IDisposable

		/// <inheritdoc />
		public void Dispose ()
		{
			this.ItemsProvider.ItemsChanged -= this.ItemsChangedHandler;
			this.ItemsProvider = null;
		}

		#endregion




		#region Interface: IItemsProvider<T>

		/// <inheritdoc />
		event EventHandler IItemsProvider<T>.ItemsChanged
		{
			add
			{
				this.ProviderItemsChanged += value;
			}
			remove
			{
				this.ProviderItemsChanged -= value;
			}
		}

		/// <inheritdoc />
		int IItemsProvider<T>.GetCount ()
		{
			return this.Count;
		}

		/// <inheritdoc />
		IEnumerable<T> IItemsProvider<T>.GetItems (int start, int count)
		{
			for (int i1 = start; i1 < (start + count); i1++)
			{
				yield return this[i1];
			}
		}

		/// <inheritdoc />
		int IItemsProvider<T>.Search (T item)
		{
			return this.IndexOf(item);
		}

		#endregion




		#region Interface: IList

		/// <inheritdoc />
		bool IList.IsFixedSize
		{
			get
			{
				return false;
			}
		}

		/// <inheritdoc />
		bool ICollection.IsSynchronized
		{
			get
			{
				return false;
			}
		}

		/// <inheritdoc />
		object ICollection.SyncRoot
		{
			get
			{
				return this.SyncRoot;
			}
		}

		/// <inheritdoc />
		object IList.this [int index]
		{
			get
			{
				return this[index];
			}
			set
			{
				this[index] = (T)value;
			}
		}

		/// <inheritdoc />
		int IList.Add (object value)
		{
			((IList<T>)this).Add((T)value);
			return ((IList)this).IndexOf(value);
		}

		/// <inheritdoc />
		void IList.Clear ()
		{
			((IList<T>)this).Clear();
		}

		/// <inheritdoc />
		bool IList.Contains (object value)
		{
			return this.Contains((T)value);
		}

		/// <inheritdoc />
		void ICollection.CopyTo (Array array, int index)
		{
			this.CopyTo((T[])array, index);
		}

		/// <inheritdoc />
		int IList.IndexOf (object value)
		{
			return this.IndexOf((T)value);
		}

		/// <inheritdoc />
		void IList.Insert (int index, object value)
		{
			((IList<T>)this).Insert(index, (T)value);
		}

		/// <inheritdoc />
		void IList.Remove (object value)
		{
			((IList<T>)this).Remove((T)value);
		}

		/// <inheritdoc />
		void IList.RemoveAt (int index)
		{
			((IList<T>)this).RemoveAt(index);
		}

		#endregion




		#region Interface: IList<T>

		/// <inheritdoc />
		public int Count
		{
			get
			{
				this.VerifyNotDisposed();
				this.CleanupCache();

				return this.ItemsProvider.GetCount();
			}
		}

		/// <inheritdoc />
		public bool IsReadOnly
		{
			get
			{
				return true;
			}
		}

		/// <inheritdoc />
		[SuppressMessage ("ReSharper", "ValueParameterNotUsed")]
		public T this [int index]
		{
			get
			{
				this.VerifyNotDisposed();
				this.CleanupCache();

				if (index < 0)
				{
					throw new IndexOutOfRangeException();
				}

				int pageIndex = index / this.PageSize;
				int pageOffset = index % this.PageSize;

				this.LoadPage(pageIndex + 1, false);
				if (pageIndex > 0)
				{
					this.LoadPage(pageIndex - 1, false);
				}

				Page page = this.LoadPage(pageIndex, false);
				if (page == null)
				{
					throw new IndexOutOfRangeException();
				}

				if (pageOffset >= page.Items.Count)
				{
					throw new IndexOutOfRangeException();
				}

				return page.Items[pageOffset];
			}
			set
			{
				this.ThrowReadOnlyException();
			}
		}

		/// <inheritdoc />
		void ICollection<T>.Add (T item)
		{
			this.ThrowReadOnlyException();
		}

		/// <inheritdoc />
		void ICollection<T>.Clear ()
		{
			this.ThrowReadOnlyException();
		}

		/// <inheritdoc />
		public bool Contains (T item)
		{
			this.VerifyNotDisposed();
			this.CleanupCache();

			return this.ItemsProvider.Search(item) != -1;
		}

		/// <inheritdoc />
		public void CopyTo (T[] array, int arrayIndex)
		{
			this.VerifyNotDisposed();
			this.CleanupCache();

			foreach (T item in this)
			{
				array[arrayIndex] = item;
				arrayIndex++;
			}
		}

		/// <inheritdoc />
		public IEnumerator<T> GetEnumerator ()
		{
			this.VerifyNotDisposed();
			this.CleanupCache();

			int pageIndex = 0;
			while (true)
			{
				Page page = this.LoadPage(pageIndex, true);

				if (page == null)
				{
					yield break;
				}

				foreach (T item in page.Items)
				{
					yield return item;
				}

				pageIndex++;
			}
		}

		/// <inheritdoc />
		IEnumerator IEnumerable.GetEnumerator ()
		{
			return this.GetEnumerator();
		}

		/// <inheritdoc />
		public int IndexOf (T item)
		{
			this.VerifyNotDisposed();
			this.CleanupCache();

			return this.ItemsProvider.Search(item);
		}

		/// <inheritdoc />
		void IList<T>.Insert (int index, T item)
		{
			this.ThrowReadOnlyException();
		}

		/// <inheritdoc />
		bool ICollection<T>.Remove (T item)
		{
			this.ThrowReadOnlyException();
			return false;
		}

		/// <inheritdoc />
		void IList<T>.RemoveAt (int index)
		{
			this.ThrowReadOnlyException();
		}

		#endregion




		#region Type: Page

		private sealed class Page
		{
			#region Instance Constructor/Destructor

			public Page (int pageIndex)
			{
				if (pageIndex < 0)
				{
					throw new ArgumentOutOfRangeException(nameof(pageIndex));
				}

				this.PageIndex = pageIndex;

				this.Items = new List<T>();
				this.Timestamp = DateTime.UtcNow;
			}

			#endregion




			#region Instance Properties/Indexer

			public List<T> Items { get; private set; }

			public int PageIndex { get; private set; }

			public DateTime Timestamp { get; private set; }

			#endregion




			#region Instance Methods

			public void ResetTtl ()
			{
				this.Timestamp = DateTime.UtcNow;
			}

			#endregion
		}

		#endregion




		#region Type: PageCollection

		private sealed class PageCollection : KeyedCollection<int, Page>
		{
			#region Overrides

			protected override int GetKeyForItem (Page item)
			{
				return item.PageIndex;
			}

			#endregion
		}

		#endregion
	}
}
