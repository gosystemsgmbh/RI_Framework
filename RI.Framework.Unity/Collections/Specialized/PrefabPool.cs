using System;

using RI.Framework.Collections.ObjectModel;

using UnityEngine;

using Object = UnityEngine.Object;




namespace RI.Framework.Collections.Specialized
{
	/// <summary>
	///     Implements a pool which creates items by instantiating from a prefab.
	/// </summary>
	/// <remarks>
	///     <para>
	///         The prefab can be any <c> GameObject </c>.
	///         <c> Object.Instantiate() </c> is used to create new items from the prefab.
	///         When a free item is removed from the pool, <c> Object.Destroy() </c> is used to dispose the item.
	///     </para>
	///     <para>
	///         The prefab itself is never used as an item or taken from the pool respectively, it is only used for instantiation.
	///     </para>
	///     <para>
	///         This pool implementation supports <see cref="IPoolAware" />.
	///     </para>
	///     <para>
	///         See <see cref="PoolBase{T}" /> for more details.
	///     </para>
	/// </remarks>
	/// <example>
	///     <code language="cs">
	/// <![CDATA[
	/// // create a pool with a prefab as prototype
	/// var pool = new PrefabPool(myPrefab);
	/// 
	/// // get some instantiated game objects from the prefab
	/// var item1 = pool.Take();
	/// var item2 = pool.Take();
	/// var item3 = pool.Take();
	/// 
	/// // ... do something ...
	/// 
	/// // return one of the game objects
	/// pool.Return(item2);
	/// 
	/// // ... do something ...
	/// 
	/// // get another game object (the former item2 is recycled)
	/// var item4 = pool.Take();
	/// ]]>
	/// </code>
	/// </example>
	public sealed class PrefabPool : PoolBase<GameObject>
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="PrefabPool" />.
		/// </summary>
		/// <param name="prefab"> The prefab the items of this pool are instantiated from. </param>
		/// <exception cref="System.ArgumentNullException"> <paramref name="prefab" /> is null. </exception>
		public PrefabPool (GameObject prefab)
		{
			if (prefab == null)
			{
				throw new ArgumentNullException(nameof(prefab));
			}

			this.Initialize(prefab);
		}

		/// <summary>
		///     Creates a new instance of <see cref="PrefabPool" />.
		/// </summary>
		/// <param name="prefab"> The prefab the items of this pool are instantiated from. </param>
		/// <param name="count"> The amount of initial free items in the pool. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="prefab" /> is null. </exception>
		/// <exception cref="ArgumentOutOfRangeException"> <paramref name="count" /> is less than zero. </exception>
		public PrefabPool (GameObject prefab, int count)
			: base(count)
		{
			if (prefab == null)
			{
				throw new ArgumentNullException(nameof(prefab));
			}

			if (count < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(count));
			}

			this.Initialize(prefab);

			this.Ensure(count);
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets or sets whether the items are activated when taken.
		/// </summary>
		/// <value>
		///     true if the items are activated when taken, false otherwise.
		/// </value>
		/// <remarks>
		///     <para>
		///         The default value is false.
		///     </para>
		///     <para>
		///         <c> GameObject.SetActive() </c> is used for activation.
		///     </para>
		/// </remarks>
		public bool AutoActivate { get; set; }

		/// <summary>
		///     Gets or sets whether the items are deactivated when returned.
		/// </summary>
		/// <value>
		///     true if the items are deactivated when taken, false otherwise.
		/// </value>
		/// <remarks>
		///     <para>
		///         The default value is false.
		///     </para>
		///     <para>
		///         <c> GameObject.SetActive() </c> is used for deactivation.
		///     </para>
		/// </remarks>
		public bool AutoDeactivate { get; set; }

		/// <summary>
		///     The original prefab from which all the items of this pool are instantiated from.
		/// </summary>
		/// <value>
		///     The original prefab as specified during construction of this instance.
		/// </value>
		/// <remarks>
		///     <note type="important">
		///         Be careful when manipulating the prefab.
		///         Already instantiated items are not affected, only items made after the manipulation.
		///     </note>
		/// </remarks>
		public GameObject Prefab { get; private set; }

		#endregion




		#region Instance Methods

		private void Initialize (GameObject prefab)
		{
			this.AutoActivate = false;
			this.AutoDeactivate = false;
			this.Prefab = prefab;
		}

		#endregion




		#region Overrides

		/// <inheritdoc />
		protected override GameObject Create ()
		{
			return Object.Instantiate(this.Prefab);
		}

		/// <inheritdoc />
		protected override void OnRemoved (GameObject item)
		{
			base.OnRemoved(item);
			Object.Destroy(item);
		}

		/// <inheritdoc />
		protected override void OnReturned (GameObject item)
		{
			base.OnReturned(item);
			if (this.AutoDeactivate)
			{
				item.SetActive(false);
			}
		}

		/// <inheritdoc />
		protected override void OnTaking (GameObject item)
		{
			base.OnTaking(item);
			if (this.AutoActivate)
			{
				item.SetActive(true);
			}
		}

		#endregion
	}
}
