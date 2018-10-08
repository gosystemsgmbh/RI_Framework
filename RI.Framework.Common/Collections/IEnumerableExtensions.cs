using System;
using System.Collections.Generic;

using RI.Framework.Utilities.Comparison;




namespace RI.Framework.Collections
{
	/// <summary>
	///     Provides utility/extension methods for the <see cref="IEnumerable{T}" /> type and its implementations.
	/// </summary>
	/// <remarks>
	///     <para>
	///         <note type="important">
	///             The complexity stated for the operations provided by this class are under the assumption that enumerating an <see cref="IEnumerable{T}" /> has a complexity of O(n) where n is the number of elements in the sequence.
	///         </note>
	///     </para>
	/// </remarks>
	public static class IEnumerableExtensions
	{
		#region Static Methods

		/// <summary>
		///     Converts two sequences to a set which only contains elements which are not in both sequences.
		///     Comparison is done using the default equality comparison.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of <paramref name="first" /> and <paramref name="second" />. </typeparam>
		/// <param name="first"> The first sequence whose elements are only added to the resulting set if they are not in <paramref name="second" />. </param>
		/// <param name="second"> The second sequence whose elements are only added to the resulting set if they are not in <paramref name="first" />. </param>
		/// <returns>
		///     The set which contains all elements from both <paramref name="first" /> and <paramref name="second" /> which only are in either one of them but not both.
		///     The set is empty if both sequences are empty or if they only contain elements which are in both sequences.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This is a O(n*m) operation where n is the number of elements in <paramref name="first" /> and m is the number of elements in <paramref name="second" />.
		///     </para>
		///     <para>
		///         <paramref name="first" /> and <paramref name="second" /> are each enumerated exactly once.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="first" /> or <paramref name="second" /> is null. </exception>
		public static HashSet<T> Exclusive <T> (this IEnumerable<T> first, IEnumerable<T> second)
		{
			return first.Exclusive(second, EqualityComparer<T>.Default.Equals);
		}

		/// <summary>
		///     Converts two sequences to a set which only contains elements which are not in both sequences.
		///     Comparison is done using the specified equality comparer.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of <paramref name="first" /> and <paramref name="second" />. </typeparam>
		/// <param name="first"> The first sequence whose elements are only added to the resulting set if they are not in <paramref name="second" />. </param>
		/// <param name="second"> The second sequence whose elements are only added to the resulting set if they are not in <paramref name="first" />. </param>
		/// <param name="comparer"> The equality comparer used to compare the elements in the sequences to look for matches. </param>
		/// <returns>
		///     The set which contains all elements from both <paramref name="first" /> and <paramref name="second" /> which only are in either one of them but not both.
		///     The set is empty if both sequences are empty or if they only contain elements which are in both sequences.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This is a O(n*m) operation where n is the number of elements in <paramref name="first" /> and m is the number of elements in <paramref name="second" />.
		///     </para>
		///     <para>
		///         <paramref name="first" /> and <paramref name="second" /> are each enumerated exactly once.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="first" />, <paramref name="second" />, or <paramref name="comparer" /> is null. </exception>
		public static HashSet<T> Exclusive <T> (this IEnumerable<T> first, IEnumerable<T> second, IEqualityComparer<T> comparer)
		{
			if (first == null)
			{
				throw new ArgumentNullException(nameof(first));
			}

			if (second == null)
			{
				throw new ArgumentNullException(nameof(second));
			}

			if (comparer == null)
			{
				throw new ArgumentNullException(nameof(comparer));
			}

			return first.Exclusive(second, comparer.Equals);
		}

		/// <summary>
		///     Converts two sequences to a set which only contains elements which are not in both sequences.
		///     Comparison is done using the specified function.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of <paramref name="first" /> and <paramref name="second" />. </typeparam>
		/// <param name="first"> The first sequence whose elements are only added to the resulting set if they are not in <paramref name="second" />. </param>
		/// <param name="second"> The second sequence whose elements are only added to the resulting set if they are not in <paramref name="first" />. </param>
		/// <param name="comparer"> The function used to compare the elements in the sequences to look for matches. </param>
		/// <returns>
		///     The set which contains all elements from both <paramref name="first" /> and <paramref name="second" /> which only are in either one of them but not both.
		///     The set is empty if both sequences are empty or if they only contain elements which are in both sequences.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This is a O(n*m) operation where n is the number of elements in <paramref name="first" /> and m is the number of elements in <paramref name="second" />.
		///     </para>
		///     <para>
		///         <paramref name="first" /> and <paramref name="second" /> are each enumerated exactly once.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="first" />, <paramref name="second" />, or <paramref name="comparer" /> is null. </exception>
		public static HashSet<T> Exclusive <T> (this IEnumerable<T> first, IEnumerable<T> second, Func<T, T, bool> comparer)
		{
			if (first == null)
			{
				throw new ArgumentNullException(nameof(first));
			}

			if (second == null)
			{
				throw new ArgumentNullException(nameof(second));
			}

			if (comparer == null)
			{
				throw new ArgumentNullException(nameof(comparer));
			}

			HashSet<T> result = new HashSet<T>(first, new EqualityComparison<T>(comparer));
			result.SymmetricExceptWith(second);
			return result;
		}

		/// <summary>
		///     Executes a specified action on each element of a sequence.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of <paramref name="enumerable" />. </typeparam>
		/// <param name="enumerable"> The sequence which contains the elements. </param>
		/// <param name="action"> The action to execute for each element, providing the element itself. </param>
		/// <returns>
		///     The number of processed elements.
		///     Zero if the sequence is empty.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This is a O(n) operation where n is the number of elements in the sequence.
		///     </para>
		///     <para>
		///         <paramref name="enumerable" /> is enumerated exactly once.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="enumerable" /> or <paramref name="action" /> is null. </exception>
		public static int ForEach <T> (this IEnumerable<T> enumerable, Action<T> action)
		{
			return enumerable.ForEach((i, e) => action(e));
		}

		/// <summary>
		///     Executes a specified action on each element of a sequence.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of <paramref name="enumerable" />. </typeparam>
		/// <param name="enumerable"> The sequence which contains the elements. </param>
		/// <param name="action"> The action to execute for each element, providing the elements index and the element itself. </param>
		/// <returns>
		///     The number of processed elements.
		///     Zero if the sequence is empty.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This is a O(n) operation where n is the number of elements in the sequence.
		///     </para>
		///     <para>
		///         <paramref name="enumerable" /> is enumerated exactly once.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="enumerable" /> or <paramref name="action" /> is null. </exception>
		public static int ForEach <T> (this IEnumerable<T> enumerable, Action<int, T> action)
		{
			if (enumerable == null)
			{
				throw new ArgumentNullException(nameof(enumerable));
			}

			if (action == null)
			{
				throw new ArgumentNullException(nameof(action));
			}

			int count = 0;
			foreach (T item in enumerable)
			{
				action(count, item);
				count++;
			}
			return count;
		}

		/// <summary>
		///     Determines how many times a specified element is in the sequence.
		///     Comparison is done using the default equality comparison.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of <paramref name="enumerable" />. </typeparam>
		/// <param name="enumerable"> The sequence which contains the elements. </param>
		/// <param name="item"> The element to count in the sequence. </param>
		/// <returns>
		///     The number of times the specified element is in the sequence.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This is a O(n) operation where n is the number of elements in the sequence.
		///     </para>
		///     <para>
		///         <paramref name="enumerable" /> is enumerated exactly once.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="enumerable" /> is null. </exception>
		public static int Same <T> (this IEnumerable<T> enumerable, T item)
		{
			return enumerable.Same(item, EqualityComparer<T>.Default.Equals);
		}

		/// <summary>
		///     Determines how many times a specified element is in the sequence.
		///     Comparison is done using the specified equality comparer.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of <paramref name="enumerable" />. </typeparam>
		/// <param name="enumerable"> The sequence which contains the elements. </param>
		/// <param name="item"> The element to count in the sequence. </param>
		/// <param name="comparer"> The equality comparer used to compare the specified element and the elements in the sequence to look for a match. </param>
		/// <returns>
		///     The number of times the specified element is in the sequence.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This is a O(n) operation where n is the number of elements in the sequence.
		///     </para>
		///     <para>
		///         <paramref name="enumerable" /> is enumerated exactly once.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="enumerable" /> or <paramref name="comparer" /> is null. </exception>
		public static int Same <T> (this IEnumerable<T> enumerable, T item, IEqualityComparer<T> comparer)
		{
			if (enumerable == null)
			{
				throw new ArgumentNullException(nameof(enumerable));
			}

			if (comparer == null)
			{
				throw new ArgumentNullException(nameof(comparer));
			}

			return enumerable.Same(item, comparer.Equals);
		}

		/// <summary>
		///     Determines how many times a specified element is in the sequence.
		///     Comparison is done using the specified function.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of <paramref name="enumerable" />. </typeparam>
		/// <param name="enumerable"> The sequence which contains the elements. </param>
		/// <param name="item"> The element to count in the sequence. </param>
		/// <param name="comparer"> The function used to compare the specified element and the elements in the sequence to look for a match. </param>
		/// <returns>
		///     The number of times the specified element is in the sequence.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This is a O(n) operation where n is the number of elements in the sequence.
		///     </para>
		///     <para>
		///         <paramref name="enumerable" /> is enumerated exactly once.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="enumerable" /> or <paramref name="comparer" /> is null. </exception>
		public static int Same <T> (this IEnumerable<T> enumerable, T item, Func<T, T, bool> comparer)
		{
			if (enumerable == null)
			{
				throw new ArgumentNullException(nameof(enumerable));
			}

			if (comparer == null)
			{
				throw new ArgumentNullException(nameof(comparer));
			}

			int count = 0;
			foreach (T currentItem in enumerable)
			{
				if (comparer(currentItem, item))
				{
					count++;
				}
			}
			return count;
		}

		/// <summary>
		///     Sorts the elements of a sequence into a new list.
		///     Comparison is done using the default order comparison.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of <paramref name="enumerable" />. </typeparam>
		/// <param name="enumerable"> The sequence which contains the elements. </param>
		/// <param name="reverseOrder"> Specifies whether the sorting is done in reverse order (that is, reversing the order as determined by the default order comparison). </param>
		/// <returns>
		///     The list which contains the elements of the sequence in sorted order.
		///     The list is empty if the sequence contains no elements.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This is a O(n^2) operation where n is the number of elements in the sequence.
		///     </para>
		///     <para>
		///         <paramref name="enumerable" /> is enumerated exactly once.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="enumerable" /> is null. </exception>
		public static List<T> Sort <T> (this IEnumerable<T> enumerable, bool reverseOrder)
		{
			return enumerable.Sort(reverseOrder, Comparer<T>.Default.Compare);
		}

		/// <summary>
		///     Sorts the elements of a sequence into a new list.
		///     Comparison is done using the specified order comparer.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of <paramref name="enumerable" />. </typeparam>
		/// <param name="enumerable"> The sequence which contains the elements. </param>
		/// <param name="reverseOrder"> Specifies whether the sorting is done in reverse order (that is, reversing the order as determined by <paramref name="comparer" />). </param>
		/// <param name="comparer"> The order comparer used to compare two elements. </param>
		/// <returns>
		///     The list which contains the elements of the sequence in sorted order.
		///     The list is empty if the sequence contains no elements.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This is a O(n^2) operation where n is the number of elements in the sequence.
		///     </para>
		///     <para>
		///         <paramref name="enumerable" /> is enumerated exactly once.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="enumerable" /> or <paramref name="comparer" /> is null. </exception>
		public static List<T> Sort <T> (this IEnumerable<T> enumerable, bool reverseOrder, IComparer<T> comparer)
		{
			if (enumerable == null)
			{
				throw new ArgumentNullException(nameof(enumerable));
			}

			if (comparer == null)
			{
				throw new ArgumentNullException(nameof(comparer));
			}

			return enumerable.Sort(reverseOrder, comparer.Compare);
		}

		/// <summary>
		///     Sorts the elements of a sequence into a new list.
		///     Comparison is done using the specified function.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of <paramref name="enumerable" />. </typeparam>
		/// <param name="enumerable"> The sequence which contains the elements. </param>
		/// <param name="reverseOrder"> Specifies whether the sorting is done in reverse order (that is, reversing the order as determined by <paramref name="comparer" />). </param>
		/// <param name="comparer"> The function used to compare two elements. </param>
		/// <returns>
		///     The list which contains the elements of the sequence in sorted order.
		///     The list is empty if the sequence contains no elements.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This is a O(n^2) operation where n is the number of elements in the sequence.
		///     </para>
		///     <para>
		///         <paramref name="enumerable" /> is enumerated exactly once.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="enumerable" /> or <paramref name="comparer" /> is null. </exception>
		public static List<T> Sort <T> (this IEnumerable<T> enumerable, bool reverseOrder, Func<T, T, int> comparer)
		{
			if (enumerable == null)
			{
				throw new ArgumentNullException(nameof(enumerable));
			}

			if (comparer == null)
			{
				throw new ArgumentNullException(nameof(comparer));
			}

			List<T> items = new List<T>(enumerable);
			items.Sort(new OrderComparison<T>(reverseOrder, comparer));
			return items;
		}

		/// <summary>
		///     Converts a sequence to a dictionary by deriving a key from each element.
		///     Each key can be assigned to one or more elements.
		///     Key equality is checked using the default equality comparer for the key type.
		/// </summary>
		/// <typeparam name="TIn"> The type of the elements of <paramref name="enumerable" />. </typeparam>
		/// <typeparam name="TKey"> The type of the derived keys in the dictionary. </typeparam>
		/// <typeparam name="TValue"> The type of the values in the dictionary. </typeparam>
		/// <param name="enumerable"> The sequence which contains the elements. </param>
		/// <param name="mapper"> The function which derives a key and a value for each element in the sequence. </param>
		/// <returns>
		///     The dictionary which contains the key-value-pairs where each value is a list of values derived from the elements which have the same key derived/assigned.
		///     The dictionary is empty if the sequence contains no elements.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This is a O(n^2) operation where n is the number of elements in the sequence.
		///     </para>
		///     <para>
		///         <paramref name="enumerable" /> is enumerated exactly once.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="enumerable" /> or <paramref name="mapper" /> is null. </exception>
		public static Dictionary<TKey, List<TValue>> ToDictionaryList <TIn, TKey, TValue> (this IEnumerable<TIn> enumerable, Func<TIn, KeyValuePair<TKey, TValue>> mapper)
		{
			if (enumerable == null)
			{
				throw new ArgumentNullException(nameof(enumerable));
			}

			if (mapper == null)
			{
				throw new ArgumentNullException(nameof(mapper));
			}

			return IEnumerableExtensions.ToDictionaryListInternal(enumerable, null, mapper);
		}

		/// <summary>
		///     Converts a sequence to a dictionary by deriving a key from each element.
		///     Each key can be assigned to one or more elements.
		///     Key equality is checked using the specified equality comparer for the key type.
		/// </summary>
		/// <typeparam name="TIn"> The type of the elements of <paramref name="enumerable" />. </typeparam>
		/// <typeparam name="TKey"> The type of the derived keys in the dictionary. </typeparam>
		/// <typeparam name="TValue"> The type of the values in the dictionary. </typeparam>
		/// <param name="enumerable"> The sequence which contains the elements. </param>
		/// <param name="keyComparer"> The equality comparer for the keys, used by the returned dictionary. </param>
		/// <param name="mapper"> The function which derives a key and a value for each element in the sequence. </param>
		/// <returns>
		///     The dictionary which contains the key-value-pairs where each value is a list of values derived from the elements which have the same key derived/assigned.
		///     The dictionary is empty if the sequence contains no elements.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This is a O(n^2) operation where n is the number of elements in the sequence.
		///     </para>
		///     <para>
		///         <paramref name="enumerable" /> is enumerated exactly once.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="enumerable" />, <paramref name="keyComparer" />, or <paramref name="mapper" /> is null. </exception>
		public static Dictionary<TKey, List<TValue>> ToDictionaryList <TIn, TKey, TValue> (this IEnumerable<TIn> enumerable, IEqualityComparer<TKey> keyComparer, Func<TIn, KeyValuePair<TKey, TValue>> mapper)
		{
			if (enumerable == null)
			{
				throw new ArgumentNullException(nameof(enumerable));
			}

			if (keyComparer == null)
			{
				throw new ArgumentNullException(nameof(keyComparer));
			}

			if (mapper == null)
			{
				throw new ArgumentNullException(nameof(mapper));
			}

			return IEnumerableExtensions.ToDictionaryListInternal(enumerable, keyComparer, mapper);
		}

		/// <summary>
		///     Converts a sequence to a dictionary by deriving a key from each element.
		///     Each key can be assigned to one or more elements.
		///     Key equality is checked using the default equality comparer for the key type.
		/// </summary>
		/// <typeparam name="TIn"> The type of the elements of <paramref name="enumerable" />. </typeparam>
		/// <typeparam name="TKey"> The type of the derived keys in the dictionary. </typeparam>
		/// <typeparam name="TValue"> The type of the values in the dictionary. </typeparam>
		/// <param name="enumerable"> The sequence which contains the elements. </param>
		/// <param name="mapper"> The function which derives a key and a value for each element in the sequence. </param>
		/// <returns>
		///     The dictionary which contains the key-value-pairs where each value is a set of values derived from the elements which have the same key derived/assigned.
		///     The dictionary is empty if the sequence contains no elements.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This is a O(n^2) operation where n is the number of elements in the sequence.
		///     </para>
		///     <para>
		///         <paramref name="enumerable" /> is enumerated exactly once.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="enumerable" /> or <paramref name="mapper" /> is null. </exception>
		public static Dictionary<TKey, HashSet<TValue>> ToDictionarySet <TIn, TKey, TValue> (this IEnumerable<TIn> enumerable, Func<TIn, KeyValuePair<TKey, TValue>> mapper)
		{
			if (enumerable == null)
			{
				throw new ArgumentNullException(nameof(enumerable));
			}

			if (mapper == null)
			{
				throw new ArgumentNullException(nameof(mapper));
			}

			return IEnumerableExtensions.ToDictionarySetInternal(enumerable, null, null, mapper);
		}

		/// <summary>
		///     Converts a sequence to a dictionary by deriving a key from each element.
		///     Each key can be assigned to one or more elements.
		///     Key equality is checked using the specified equality comparer for the key type.
		/// </summary>
		/// <typeparam name="TIn"> The type of the elements of <paramref name="enumerable" />. </typeparam>
		/// <typeparam name="TKey"> The type of the derived keys in the dictionary. </typeparam>
		/// <typeparam name="TValue"> The type of the values in the dictionary. </typeparam>
		/// <param name="enumerable"> The sequence which contains the elements. </param>
		/// <param name="keyComparer"> The equality comparer for the keys, used by the returned dictionary. </param>
		/// <param name="setComparer"> The equality comparer for the values, used by the sets in the returned dictionary. </param>
		/// <param name="mapper"> The function which derives a key and a value for each element in the sequence. </param>
		/// <returns>
		///     The dictionary which contains the key-value-pairs where each value is a set of values derived from the elements which have the same key derived/assigned.
		///     The dictionary is empty if the sequence contains no elements.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This is a O(n^2) operation where n is the number of elements in the sequence.
		///     </para>
		///     <para>
		///         <paramref name="enumerable" /> is enumerated exactly once.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="enumerable" />, <paramref name="keyComparer" />, <paramref name="setComparer" />, or <paramref name="mapper" /> is null. </exception>
		public static Dictionary<TKey, HashSet<TValue>> ToDictionarySet <TIn, TKey, TValue> (this IEnumerable<TIn> enumerable, IEqualityComparer<TKey> keyComparer, IEqualityComparer<TValue> setComparer, Func<TIn, KeyValuePair<TKey, TValue>> mapper)
		{
			if (enumerable == null)
			{
				throw new ArgumentNullException(nameof(enumerable));
			}

			if (keyComparer == null)
			{
				throw new ArgumentNullException(nameof(keyComparer));
			}

			if (setComparer == null)
			{
				throw new ArgumentNullException(nameof(setComparer));
			}

			if (mapper == null)
			{
				throw new ArgumentNullException(nameof(mapper));
			}

			return IEnumerableExtensions.ToDictionarySetInternal(enumerable, keyComparer, setComparer, mapper);
		}

		private static Dictionary<TKey, List<TValue>> ToDictionaryListInternal <TIn, TKey, TValue> (IEnumerable<TIn> enumerable, IEqualityComparer<TKey> keyComparer, Func<TIn, KeyValuePair<TKey, TValue>> mapper)
		{
			Dictionary<TKey, List<TValue>> dictionary = new Dictionary<TKey, List<TValue>>(keyComparer ?? EqualityComparer<TKey>.Default);
			foreach (TIn item in enumerable)
			{
				KeyValuePair<TKey, TValue> pair = mapper(item);
				if (!dictionary.ContainsKey(pair.Key))
				{
					dictionary.Add(pair.Key, new List<TValue>());
				}
				dictionary[pair.Key].Add(pair.Value);
			}
			return dictionary;
		}

		private static Dictionary<TKey, HashSet<TValue>> ToDictionarySetInternal <TIn, TKey, TValue> (IEnumerable<TIn> enumerable, IEqualityComparer<TKey> keyComparer, IEqualityComparer<TValue> setComparer, Func<TIn, KeyValuePair<TKey, TValue>> mapper)
		{
			setComparer = setComparer ?? EqualityComparer<TValue>.Default;
			Dictionary<TKey, HashSet<TValue>> dictionary = new Dictionary<TKey, HashSet<TValue>>(keyComparer ?? EqualityComparer<TKey>.Default);
			foreach (TIn item in enumerable)
			{
				KeyValuePair<TKey, TValue> pair = mapper(item);
				if (!dictionary.ContainsKey(pair.Key))
				{
					dictionary.Add(pair.Key, new HashSet<TValue>(setComparer));
				}
				dictionary[pair.Key].Add(pair.Value);
			}
			return dictionary;
		}

		#endregion
	}
}
