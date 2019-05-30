using System;




namespace RI.Framework.Collections.ObjectModel
{
    /// <summary>
    ///     Implements a simple pool which supports events for taking and returning.
    /// </summary>
    /// <typeparam name="T"> The type of objects which can be stored and recycled by the pool. </typeparam>
    /// <remarks>
    ///     <para>
    ///         See <see cref="PoolBase{T}" /> for more details.
    ///     </para>
    ///     <para>
    ///         This pool implementation supports <see cref="IPoolAware" />.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    /// <example>
    ///     <code language="cs">
    /// <![CDATA[
    /// // create a pool with initially one free item
    /// var pool = new Pool<MyObject>(1);
    /// 
    /// // register an event to initialize all items which will be created by the pool
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
        /// <remarks>
        ///     <note type="note">
        ///         For performance reasons, the event handler is not of type <see cref="EventHandler" />, therefore not using <see cref="EventArgs" />.
        ///         To identify the <see cref="Pool{T}" /> which is the event source, the <see cref="Pool{T}" /> instance is provided as the firts parameter and the affected item as the second parameter.
        ///     </note>
        /// </remarks>
        public event Action<Pool<T>, T> Created;

        /// <summary>
        ///     Raised when a free item is removed from the pool.
        /// </summary>
        /// <remarks>
        ///     <note type="note">
        ///         For performance reasons, the event handler is not of type <see cref="EventHandler" />, therefore not using <see cref="EventArgs" />.
        ///         To identify the <see cref="Pool{T}" /> which is the event source, the <see cref="Pool{T}" /> instance is provided as the firts parameter and the affected item as the second parameter.
        ///     </note>
        /// </remarks>
        public event Action<Pool<T>, T> Removed;

        /// <summary>
        ///     Raised when an item is returned to the pool.
        /// </summary>
        /// <remarks>
        ///     <note type="note">
        ///         For performance reasons, the event handler is not of type <see cref="EventHandler" />, therefore not using <see cref="EventArgs" />.
        ///         To identify the <see cref="Pool{T}" /> which is the event source, the <see cref="Pool{T}" /> instance is provided as the firts parameter and the affected item as the second parameter.
        ///     </note>
        /// </remarks>
        public event Action<Pool<T>, T> Returned;

        /// <summary>
        ///     Raised when an item is taken from the pool.
        /// </summary>
        /// <remarks>
        ///     <note type="note">
        ///         For performance reasons, the event handler is not of type <see cref="EventHandler" />, therefore not using <see cref="EventArgs" />.
        ///         To identify the <see cref="Pool{T}" /> which is the event source, the <see cref="Pool{T}" /> instance is provided as the firts parameter and the affected item as the second parameter.
        ///     </note>
        /// </remarks>
        public event Action<Pool<T>, T> Taking;

        #endregion




        #region Overrides

        /// <inheritdoc />
        protected override T Create () => new T();

        /// <inheritdoc />
        protected override void OnCreated (T item)
        {
            base.OnCreated(item);
            Action<Pool<T>, T> handler = this.Created;
            handler?.Invoke(this, item);
        }

        /// <inheritdoc />
        protected override void OnRemoved (T item)
        {
            base.OnRemoved(item);
            Action<Pool<T>, T> handler = this.Removed;
            handler?.Invoke(this, item);
        }

        /// <inheritdoc />
        protected override void OnReturned (T item)
        {
            base.OnReturned(item);
            Action<Pool<T>, T> handler = this.Returned;
            handler?.Invoke(this, item);
        }

        /// <inheritdoc />
        protected override void OnTaking (T item)
        {
            base.OnTaking(item);
            Action<Pool<T>, T> handler = this.Taking;
            handler?.Invoke(this, item);
        }

        #endregion
    }
}
