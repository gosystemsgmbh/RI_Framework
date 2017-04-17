using System;
using System.Collections.Generic;




namespace RI.Framework.Collections.ObjectModel
{
	/// <summary>
	///     Implements a base class which can be used for <see cref="IPool{T}" /> implementations.
	/// </summary>
	/// <typeparam name="T"> The type of objects which can be stored and recycled by the pool. </typeparam>
	/// <remarks>
	///     <para>
	///         All pools derived from <see cref="PoolBase{T}" /> support the <see cref="IPoolAware" /> interface.
	///         See <see cref="IPoolAware" /> for more details about support of <see cref="IPoolAware" />.
	///     </para>
	/// </remarks>
	public abstract class PoolBase <T> : IPool<T>
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="PoolBase{T}" />.
		/// </summary>
		protected PoolBase ()
		{
			this._freeItemsInternal = new List<T>();
		}

		/// <summary>
		///     Creates a new instance of <see cref="PoolBase{T}" />.
		/// </summary>
		/// <param name="capacity"> The initial capacity of free items in the pool. </param>
		/// <remarks>
		///     <para>
		///         <paramref name="capacity" /> is only a hint of the expected number of free items.
		///         No free items are created so the initial count of free items in the pool is zero, regardless of the value of <paramref name="capacity" />.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException"> <paramref name="capacity" /> is less than zero. </exception>
		protected PoolBase (int capacity)
		{
			if (capacity < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(capacity));
			}

			this._freeItemsInternal = new List<T>(capacity);
		}

		#endregion




		#region Instance Fields

		private readonly List<T> _freeItemsInternal;

		#endregion




		#region Instance Methods

		/// <summary>
		///     Returns an item to the pool as a free item so that it can be recycled by <see cref="IPool{T}.Take" />.
		/// </summary>
		/// <param name="item"> The item to return to the pool. </param>
		/// <returns>
		///     true if the item was returned, false if it was already returned.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This is a O(n) operation where n is the number of free items in the pool.
		///     </para>
		///     <note type="important">
		///         This return operation does check whether the item to be returned has already been returned to ensure consistency of the free and taken items.
		///         If a more performant return operation is required, use <see cref="Return(T)" /> instead.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="item" /> is null. </exception>
		public bool ReturnSafe (T item)
		{
			if (item == null)
			{
				throw new ArgumentNullException(nameof(item));
			}

			if (this.Contains(item))
			{
				return false;
			}

			this.Return(item);
			return true;
		}

		#endregion




		#region Abstracts

		/// <summary>
		///     Called when a new item needs to be created.
		/// </summary>
		/// <returns>
		///     The instance of the newly created item.
		/// </returns>
		protected abstract T Create ();

		#endregion




		#region Virtuals

		/// <summary>
		///     Called when an item is created.
		/// </summary>
		/// <param name="item"> The item which is created. </param>
		protected virtual void OnCreated (T item)
		{
			IPoolAware poolAware = item as IPoolAware;
			poolAware?.Created();
		}

		/// <summary>
		///     Called when a free item is removed from the pool.
		/// </summary>
		/// <param name="item"> The free item which is removed from the pool. </param>
		protected virtual void OnRemoved (T item)
		{
			IPoolAware poolAware = item as IPoolAware;
			poolAware?.Removed();
		}

		/// <summary>
		///     Called when an item is returned to the pool.
		/// </summary>
		/// <param name="item"> The item which is returned to the pool. </param>
		protected virtual void OnReturned (T item)
		{
			IPoolAware poolAware = item as IPoolAware;
			poolAware?.Returned();
		}

		/// <summary>
		///     Called when an item is taken from the pool.
		/// </summary>
		/// <param name="item"> The item which is taken from the pool. </param>
		protected virtual void OnTaking (T item)
		{
			IPoolAware poolAware = item as IPoolAware;
			poolAware?.Taking();
		}

		#endregion




		#region Interface: IPool<T>

		/// <inheritdoc />
		public int Count
		{
			get
			{
				return this._freeItemsInternal.Count;
			}
		}

		/// <inheritdoc />
		public IEnumerable<T> FreeItems
		{
			get
			{
				return this._freeItemsInternal;
			}
		}

		/// <inheritdoc />
		/// <remarks>
		///     <para>
		///         This is a O(n) operation where n is the number of free items in the pool.
		///     </para>
		/// </remarks>
		public void Clear ()
		{
			this.Reduce(0);
		}

		/// <inheritdoc />
		/// <remarks>
		///     <para>
		///         This is a O(n) operation where n is the number of free items in the pool.
		///     </para>
		/// </remarks>
		public bool Contains (T item)
		{
			if (item == null)
			{
				throw new ArgumentNullException(nameof(item));
			}

			return this._freeItemsInternal.Contains(item);
		}

		/// <inheritdoc />
		/// <remarks>
		///     <para>
		///         This is a O(n) operation where n is the number of items which need to be created.
		///     </para>
		/// </remarks>
		public int Ensure (int minItems)
		{
			if (minItems < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(minItems));
			}

			if (this._freeItemsInternal.Capacity < minItems)
			{
				this._freeItemsInternal.Capacity = minItems;
			}

			int count = 0;
			while (this._freeItemsInternal.Count < minItems)
			{
				T item = this.Create();
				this.OnCreated(item);
				this._freeItemsInternal.Add(item);
				this.OnReturned(item);
				count++;
			}
			return count;
		}

		/// <inheritdoc />
		/// <remarks>
		///     <para>
		///         This is a O(n) operation where n is the number of free items which need to be removed.
		///     </para>
		/// </remarks>
		public int Reduce (int maxItems)
		{
			if (maxItems < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(maxItems));
			}

			int count = 0;
			while (this._freeItemsInternal.Count > maxItems)
			{
				T item = this._freeItemsInternal[this._freeItemsInternal.Count - 1];
				this._freeItemsInternal.RemoveAt(this._freeItemsInternal.Count - 1);
				this.OnRemoved(item);
				count++;
			}
			return count;
		}

		/// <inheritdoc />
		/// <remarks>
		///     <para>
		///         This is a O(1) operation.
		///     </para>
		///     <note type="important">
		///         To increase performance, this return operation does not check whether the item to be returned has already been returned previously.
		///         Returning an item which is already been returned leads to unpredictable behaviour.
		///         If a safe return operation, checking whether an item has already been returned or not, at the cost of performance, is required, use <see cref="ReturnSafe(T)" /> instead.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="item" /> is null. </exception>
		public void Return (T item)
		{
			if (item == null)
			{
				throw new ArgumentNullException(nameof(item));
			}

			this._freeItemsInternal.Add(item);
			this.OnReturned(item);
		}

		/// <inheritdoc />
		/// <remarks>
		///     <para>
		///         This is a O(1) operation.
		///     </para>
		/// </remarks>
		public T Take ()
		{
			T item;

			if (this._freeItemsInternal.Count == 0)
			{
				item = this.Create();
				this.OnCreated(item);
			}
			else
			{
				item = this._freeItemsInternal[this._freeItemsInternal.Count - 1];
				this._freeItemsInternal.RemoveAt(this._freeItemsInternal.Count - 1);
			}

			this.OnTaking(item);

			return item;
		}

		#endregion
	}
}
