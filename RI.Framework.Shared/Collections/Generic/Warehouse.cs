using System;




namespace RI.Framework.Collections.Generic
{
	/// <summary>
	///     Implements a simple warehouse.
	/// </summary>
	/// <typeparam name="T"> The type of items stored in the warehouse. </typeparam>
	/// <remarks>
	///     <para>
	///         A <see cref="Warehouse{T}" /> only manages the bays and provides the storage.
	///         The items themselves must be managed by whatever uses the <see cref="Warehouse{T}" />.
	///         <see cref="Warehouse{T}" /> never touches the contents of <see cref="Storage" />.
	///     </para>
	/// </remarks>
	/// <example>
	///     <para>
	///         The following example shows how a <see cref="Warehouse{T}" /> can be used to store items:
	///     </para>
	///     <code language="cs">
	/// <![CDATA[
	/// // create a new warehouse and get its storage
	/// var warehouse = new Warehouse<MyObject>(100000);
	/// var storage = warehouse.Storage;
	/// 
	/// // reserve a bay
	/// var bay = warehouse.Reserve();
	/// 
	/// // store an item in the bay
	/// storage[bay] = new MyObject();
	/// 
	/// //...do something with the warehouse and its bays wherever and as long as necessary...
	/// 
	/// //release the bay when it is no longer used
	/// warehouse.Release(bay);
	/// ]]>
	/// </code>
	/// </example>
	public sealed class Warehouse <T> : IWarehouse<T>
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="Warehouse{T}" />.
		/// </summary>
		/// <param name="size"> The total amount of bays. </param>
		/// <exception cref="ArgumentOutOfRangeException"> <paramref name="size" /> is less than one. </exception>
		public Warehouse (int size)
		{
			if (size < 1)
			{
				throw new ArgumentOutOfRangeException(nameof(size));
			}

			this.Size = size;
			this.Storage = new T[size + 1];

			this.Bays = new int[size];
			this.BayIndex = size - 1;

			for (int i1 = 0; i1 < size; i1++)
			{
				this.Bays[i1] = size - i1;
			}
		}

		#endregion




		#region Instance Fields

		private int BayIndex;

		private readonly int[] Bays;

		#endregion




		#region Interface: IWarehouse<T>

		/// <inheritdoc />
		public int Free
		{
			get
			{
				return this.BayIndex + 1;
			}
		}

		/// <inheritdoc />
		public int Size { get; private set; }

		/// <inheritdoc />
		public T[] Storage { get; private set; }

		/// <inheritdoc />
		/// <remarks>
		///     <para>
		///         This is a O(1) operation.
		///     </para>
		///     <note type="important">
		///         For performance reasons, <see cref="Release" /> does not check whether a bay was already released.
		///         If a bay is released which is already released, unpredictable behaviour occurs.
		///     </note>
		/// </remarks>
		public void Release (int bay)
		{
			if (( bay < 1 ) || ( bay > this.Bays.Length ))
			{
				throw new ArgumentOutOfRangeException(nameof(bay));
			}

			this.BayIndex++;
			this.Bays[this.BayIndex] = bay;
		}

		/// <inheritdoc />
		/// <remarks>
		///     <para>
		///         This is a O(1) operation.
		///     </para>
		/// </remarks>
		public int Reserve ()
		{
			if (this.BayIndex == -1)
			{
				return 0;
			}

			int bay = this.Bays[this.BayIndex];
			this.BayIndex--;
			return bay;
		}

		#endregion
	}
}
