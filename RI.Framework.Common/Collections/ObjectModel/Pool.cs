using System;




namespace RI.Framework.Collections.ObjectModel
{
	/// <summary>
	///     Implements a simple pool which supports events for taking and returning.
	/// </summary>
	/// <typeparam name="T"> The type of objects which can be stored and recycled by the pool. </typeparam>
	/// <remarks>
	///     <para>
	///         This pool implementation supports <see cref="IPoolAware" />.
	///         See <see cref="PoolBase{T}" /> for more details.
	///     </para>
	/// </remarks>
	/// <example>
	///     <para>
	///         The following example shows how a <see cref="Pool{T}" /> can be used:
	///     </para>
	///     <code language="cs">
	/// <![CDATA[
	/// // create a pool with initially one free item
	/// var pool = new Pool<MyObject>(1);
	/// pool.Created += x => x.Initialize();
	/// 
	/// // get an item from the pool (the first one already exists as a free item in the pool)
	/// var item1 = pool.Take();
	/// 
	/// // get another item from the pool (the second now needs to be created by the pool)
	/// var item2 = pool.Take();
	/// 
	/// // ... do something ...
	/// 
	/// // return one of the items
	/// pool.Return(item2);
	/// 
	/// // ... do something ...
	/// 
	/// // get another item (the former item2 is recycled)
	/// var item3 = pool.Take();
	/// ]]>
	/// </code>
	/// </example>
	public sealed class Pool <T> : PoolBase<T>
		where T : new()
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="Pool{T}" />.
		/// </summary>
		public Pool ()
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="Pool{T}" />.
		/// </summary>
		/// <param name="capacity"> The initial capacity of free items in the pool. </param>
		/// <remarks>
		///     <para>
		///         <paramref name="capacity" /> is only a hint of the expected number of free items.
		///         No free items are created so the initial count of free items in the pool is zero, regardless of the value of <paramref name="capacity" />.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException"> <paramref name="capacity" /> is less than zero. </exception>
		public Pool (int capacity)
			: base(capacity)
		{
		}

		#endregion




		#region Instance Events

		/// <summary>
		///     Raised when a new item is created.
		/// </summary>
		public event Action<T> Created;

		/// <summary>
		///     Raised when a free item is removed from the pool.
		/// </summary>
		public event Action<T> Removed;

		/// <summary>
		///     Raised when an item is returned to the pool.
		/// </summary>
		public event Action<T> Returned;

		/// <summary>
		///     Raised when an item is taken from the pool.
		/// </summary>
		public event Action<T> Taking;

		#endregion




		#region Overrides

		/// <inheritdoc />
		protected override T Create ()
		{
			return new T();
		}

		/// <inheritdoc />
		protected override void OnCreated (T item)
		{
			base.OnCreated(item);
			Action<T> handler = this.Created;
			handler?.Invoke(item);
		}

		/// <inheritdoc />
		protected override void OnRemoved (T item)
		{
			base.OnRemoved(item);
			Action<T> handler = this.Removed;
			handler?.Invoke(item);
		}

		/// <inheritdoc />
		protected override void OnReturned (T item)
		{
			base.OnReturned(item);
			Action<T> handler = this.Returned;
			handler?.Invoke(item);
		}

		/// <inheritdoc />
		protected override void OnTaking (T item)
		{
			base.OnTaking(item);
			Action<T> handler = this.Taking;
			handler?.Invoke(item);
		}

		#endregion
	}
}
