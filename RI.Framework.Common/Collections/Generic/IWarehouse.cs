using System;
using System.Collections;
using System.Collections.Generic;

using RI.Framework.Utilities.ObjectModel;

namespace RI.Framework.Collections.Generic
{
	/// <summary>
	///     Defines a generic interface for &quot;warehouses&quot; which can be used to efficiently store very large amounts of items.
	/// </summary>
	/// <typeparam name="T"> The type of items stored in the warehouse. </typeparam>
	/// <remarks>
	///     <para>
	///         A warehouse uses bays to store the items.
	///         One item is stored in one bay and one bay is used to store one item.
	///         <see cref="Storage" /> is the array which contains all the bays.
	///     </para>
	///     <para>
	///         Each bay is identified by a number, provided and used by <see cref="Reserve" /> and <see cref="Release" />.
	///         That number can be used to directly access the bays by using it as an index for the <see cref="Storage" /> array.
	///         Zero is always used as an equivalent of &quot;invalid&quot; or &quot;null&quot;.
	///     </para>
	///     <para>
	///         This allows to use <see cref="int" />s instead of object references for the items.
	///         It also allows to provide a very efficient way to allocate and deallocate collection storage for items.
	///     </para>
	///     <para>
	///         An <see cref="IWarehouse{T}" /> only manages the bays and provides the storage.
	///         The items themselves must be managed by whatever uses the <see cref="IWarehouse{T}" />.
	///         <see cref="IWarehouse{T}" /> never touches the contents of <see cref="Storage" />, except for enumerating them as provided by <see cref="IEnumerable{T}"/>
	///     </para>
	/// </remarks>
	public interface IWarehouse <T> : ICollection, IEnumerable<T>, IEnumerable, ISynchronizable
	{
		/// <summary>
		///     Gets the amount of free bays.
		/// </summary>
		/// <value>
		///     The amount of free bays.
		/// </value>
		int Free { get; }

		/// <summary>
		///     Gets the total amount of bays.
		/// </summary>
		/// <value>
		///     The total amount of bays.
		/// </value>
		int Size { get; }

		/// <summary>
		///     Gets the array containing all the bays.
		/// </summary>
		/// <value>
		///     The array containing all the bays.
		/// </value>
		/// <remarks>
		///     <note type="important">
		///         Do not use the length of the array to determine the total amount of bays, always use the <see cref="Size" /> property.
		///     </note>
		/// </remarks>
		T[] Storage { get; }

		/// <summary>
		///     Releases a bay.
		/// </summary>
		/// <param name="bay"> The bay to be released. </param>
		/// <exception cref="ArgumentOutOfRangeException"> <paramref name="bay" /> is either less than one or larger than the storage size. </exception>
		void Release (int bay);

		/// <summary>
		///     Determines and reserves the next free bay.
		/// </summary>
		/// <returns>
		///     The next free bay or zero if no more free bays are available.
		/// </returns>
		int Reserve ();
	}
}
