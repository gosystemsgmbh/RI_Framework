using System;
using System.Collections.Generic;

using RI.Framework.Collections.DirectLinq;




namespace RI.Framework.Collections
{
	/// <summary>
	///     Provides utility/extension methods for the <see cref="HashSet{T}" /> type.
	/// </summary>
	public static class HashSetExtensions
	{
		#region Static Methods

		/// <summary>
		///     Adds multiple items to a hash set.
		/// </summary>
		/// <typeparam name="T"> The type of the items in <paramref name="hashSet" />. </typeparam>
		/// <param name="hashSet"> The hash set. </param>
		/// <param name="items"> The sequence of items to add to the hash set. </param>
		/// <returns>
		///     The number of items added to the hash set.
		/// </returns>
		/// <remarks>
		///     <para>
		///         <paramref name="items" /> is enumerated exactly once.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="hashSet" /> or <paramref name="items" /> is null. </exception>
		public static int AddRange <T> (this HashSet<T> hashSet, IEnumerable<T> items)
		{
			if (hashSet == null)
			{
				throw new ArgumentNullException(nameof(hashSet));
			}

			if (items == null)
			{
				throw new ArgumentNullException(nameof(items));
			}

			int count = hashSet.Count;
			hashSet.UnionWith(items.AsEnumerable().ToList());
			return hashSet.Count - count;
		}

		/// <summary>
		///     Removes multiple items from a hash set.
		/// </summary>
		/// <typeparam name="T"> The type of the items in <paramref name="hashSet" />. </typeparam>
		/// <param name="hashSet"> The hash set. </param>
		/// <param name="items"> The sequence of items to remove from the hash set. </param>
		/// <returns>
		///     The number of items removed from the hash set.
		/// </returns>
		/// <remarks>
		///     <para>
		///         <paramref name="items" /> is enumerated exactly once.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="hashSet" /> or <paramref name="items" /> is null. </exception>
		public static int RemoveRange <T> (this HashSet<T> hashSet, IEnumerable<T> items)
		{
			if (hashSet == null)
			{
				throw new ArgumentNullException(nameof(hashSet));
			}

			if (items == null)
			{
				throw new ArgumentNullException(nameof(items));
			}

			int count = hashSet.Count;
			hashSet.ExceptWith(items.AsEnumerable().ToList());
			return count - hashSet.Count;
		}

		#endregion
	}
}
