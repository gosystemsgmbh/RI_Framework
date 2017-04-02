using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using RI.Framework.Collections.Comparison;
using RI.Framework.Utilities.Comparison;




namespace RI.Framework.Collections.DirectLinq
{
	/// <summary>
	///     Provides a LINQ replacement with utility/extension methods for the <see cref="IEnumerable{T}" /> type and its implementations.
	/// </summary>
	/// <remarks>
	///         <para>
	/// <see cref="DirectLinqExtensions"/> provides a LINQ replacement which uses non-deferred execution and does not depend on reflection.
	/// Therefore, the LINQ query is executed at the time a <see cref="DirectLinqExtensions"/> method is called, not at the time its result is eventually enumerated.
	///         </para>
	/// <para>
	/// Normal LINQ query expressions, such as <c>from x in values where x.IsActive select x</c>, can still be used.
	/// Just replace the namespace <see cref="System.Linq"/> (<c>using System.Linq;</c>) with <see cref="RI.Framework.Collections.DirectLinq"/> (<c>using RI.Framework.Collections.DirectLinq;</c>).
	/// </para>
	///         <note type="important">
	///             Not all LINQ features are implemented in <see cref="DirectLinqExtensions"/>.
	/// The main missing functions are: Grouping, Joining, Ordering, Aggregating.
	///         </note>
	///         <note type="important">
	///             The complexity stated for the operations provided by this class are under the assumption that enumerating an <see cref="IEnumerable{T}" /> has a complexity of O(n) where n is the number of elements in the sequence.
	///         </note>
	/// </remarks>
	public static class DirectLinqExtensions
	{
		#region Static Methods

		/// <summary>
		///     Determines whether all elements of a sequence satisfy a specified condition.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of <paramref name="enumerable" />. </typeparam>
		/// <param name="enumerable"> The sequence which contains the elements. </param>
		/// <param name="condition"> The function which tests each element for a condition, providing the element itself. </param>
		/// <returns>
		///     true if all elements satisfy the condition, false otherwise.
		///     The return value is false if the sequence contains no elements.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This is a O(n) operation where n is the number of elements in the sequence.
		///     </para>
		///     <para>
		///         <paramref name="enumerable" /> is enumerated only once and only up to the element which first does not satisfy the condition.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="enumerable" /> or <paramref name="condition" /> is null. </exception>
		public static bool All <T> (this IEnumerable<T> enumerable, Func<T, bool> condition)
		{
			return enumerable.All((i, e) => condition(e));
		}

		/// <summary>
		///     Determines whether all elements of a sequence satisfy a specified condition.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of <paramref name="enumerable" />. </typeparam>
		/// <param name="enumerable"> The sequence which contains the elements. </param>
		/// <param name="condition"> The function which tests each element for a condition, providing the elements index and the element itself. </param>
		/// <returns>
		///     true if all elements satisfy the condition, false otherwise.
		///     The return value is false if the sequence contains no elements.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This is a O(n) operation where n is the number of elements in the sequence.
		///     </para>
		///     <para>
		///         <paramref name="enumerable" /> is enumerated only once and only up to the element which first does not satisfy the condition.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="enumerable" /> or <paramref name="condition" /> is null. </exception>
		public static bool All <T> (this IEnumerable<T> enumerable, Func<int, T, bool> condition)
		{
			if (enumerable == null)
			{
				throw new ArgumentNullException(nameof(enumerable));
			}

			if (condition == null)
			{
				throw new ArgumentNullException(nameof(condition));
			}

			bool result = false;
			int count = 0;
			foreach (T item in enumerable)
			{
				result = true;
				if (!condition(count, item))
				{
					return false;
				}
				count++;
			}
			return result;
		}

		/// <summary>
		///     Determines whether a sequence consists of only the specified element.
		///     Comparison is done using the default equality comparison.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of <paramref name="enumerable" />. </typeparam>
		/// <param name="enumerable"> The sequence which contains the elements. </param>
		/// <param name="item"> The element the sequence should only consist of. </param>
		/// <returns>
		///     true if the sequence consists only of the specified element, false otherwise.
		///     The return value is false if the sequence contains no elements.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This is a O(n) operation where n is the number of elements in the sequence.
		///     </para>
		///     <para>
		///         <paramref name="enumerable" /> is enumerated only once and only up to the element which first does not match the specified element.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="enumerable" /> is null. </exception>
		public static bool All <T> (this IEnumerable<T> enumerable, T item)
		{
			return enumerable.All(item, EqualityComparer<T>.Default.Equals);
		}

		/// <summary>
		///     Determines whether a sequence consists of only the specified element.
		///     Comparison is done using the specified equality comparer.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of <paramref name="enumerable" />. </typeparam>
		/// <param name="enumerable"> The sequence which contains the elements. </param>
		/// <param name="item"> The element the sequence should only consist of. </param>
		/// <param name="comparer"> The equality comparer used to compare the specified element and the elements in the sequence to look for a match. </param>
		/// <returns>
		///     true if the sequence consists only of the specified element, false otherwise.
		///     The return value is false if the sequence contains no elements.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This is a O(n) operation where n is the number of elements in the sequence.
		///     </para>
		///     <para>
		///         <paramref name="enumerable" /> is enumerated only once and only up to the element which first does not match the specified element.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="enumerable" /> or <paramref name="comparer" /> is null. </exception>
		public static bool All <T> (this IEnumerable<T> enumerable, T item, IEqualityComparer<T> comparer)
		{
			if (enumerable == null)
			{
				throw new ArgumentNullException(nameof(enumerable));
			}

			if (comparer == null)
			{
				throw new ArgumentNullException(nameof(comparer));
			}

			return enumerable.All(item, comparer.Equals);
		}

		/// <summary>
		///     Determines whether a sequence consists of only the specified element.
		///     Comparison is done using the specified function.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of <paramref name="enumerable" />. </typeparam>
		/// <param name="enumerable"> The sequence which contains the elements. </param>
		/// <param name="item"> The element the sequence should only consist of. </param>
		/// <param name="comparer"> The function used to compare the specified element and the elements in the sequence to look for a match. </param>
		/// <returns>
		///     true if the sequence consists only of the specified element, false otherwise.
		///     The return value is false if the sequence contains no elements.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This is a O(n) operation where n is the number of elements in the sequence.
		///     </para>
		///     <para>
		///         <paramref name="enumerable" /> is enumerated only once and only up to the element which first does not match the specified element.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="enumerable" /> or <paramref name="comparer" /> is null. </exception>
		public static bool All <T> (this IEnumerable<T> enumerable, T item, Func<T, T, bool> comparer)
		{
			if (enumerable == null)
			{
				throw new ArgumentNullException(nameof(enumerable));
			}

			if (comparer == null)
			{
				throw new ArgumentNullException(nameof(comparer));
			}

			bool result = false;
			foreach (T currentItem in enumerable)
			{
				result = true;
				if (!comparer(currentItem, item))
				{
					return false;
				}
			}
			return result;
		}

		/// <summary>
		///     Determines whether a sequence has any elements.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of <paramref name="enumerable" />. </typeparam>
		/// <param name="enumerable"> The sequence which contains the elements. </param>
		/// <returns>
		///     true if the sequence contains one or more elements, false otherwise.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This is a O(1) operation.
		///     </para>
		///     <para>
		///         <paramref name="enumerable" /> is enumerated only once and only up to the first element.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="enumerable" /> is null. </exception>
		[SuppressMessage ("ReSharper", "UnusedVariable")]
		public static bool Any <T> (this IEnumerable<T> enumerable)
		{
			if (enumerable == null)
			{
				throw new ArgumentNullException(nameof(enumerable));
			}

			foreach (T item in enumerable)
			{
				return true;
			}
			return false;
		}

		/// <summary>
		///     Determines whether a sequence has any elements which satisfy a specified condition.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of <paramref name="enumerable" />. </typeparam>
		/// <param name="enumerable"> The sequence which contains the elements. </param>
		/// <param name="condition"> The function which tests each element for a condition, providing the element itself. </param>
		/// <returns>
		///     true if the sequence contains one or more elements which satisfy the condition, false otherwise.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This is a O(n) operation where n is the number of elements in the sequence.
		///     </para>
		///     <para>
		///         <paramref name="enumerable" /> is enumerated only once and only up to the first element which satisfies the condition.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="enumerable" /> or <paramref name="condition" /> is null. </exception>
		public static bool Any <T> (this IEnumerable<T> enumerable, Func<T, bool> condition)
		{
			return enumerable.Any((i, e) => condition(e));
		}

		/// <summary>
		///     Determines whether a sequence has any elements which satisfy a specified condition.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of <paramref name="enumerable" />. </typeparam>
		/// <param name="enumerable"> The sequence which contains the elements. </param>
		/// <param name="condition"> The function which tests each element for a condition, providing the elements index and the element itself. </param>
		/// <returns>
		///     true if the sequence contains one or more elements which satisfy the condition, false otherwise.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This is a O(n) operation where n is the number of elements in the sequence.
		///     </para>
		///     <para>
		///         <paramref name="enumerable" /> is enumerated only once and only up to the first element which satisfies the condition.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="enumerable" /> or <paramref name="condition" /> is null. </exception>
		public static bool Any <T> (this IEnumerable<T> enumerable, Func<int, T, bool> condition)
		{
			if (enumerable == null)
			{
				throw new ArgumentNullException(nameof(enumerable));
			}

			if (condition == null)
			{
				throw new ArgumentNullException(nameof(condition));
			}

			int count = 0;
			foreach (T item in enumerable)
			{
				if (condition(count, item))
				{
					return true;
				}
				count++;
			}
			return false;
		}

		/// <summary>
		///     Determines whether a sequence has any elements at a specified index.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of <paramref name="enumerable" />. </typeparam>
		/// <param name="enumerable"> The sequence which contains the elements. </param>
		/// <param name="index"> The index at which an element is expected. </param>
		/// <returns>
		///     true if the sequence contains an element at the specified index, false otherwise.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This is a O(n) operation where n is the number of elements in the sequence.
		///     </para>
		///     <para>
		///         <paramref name="enumerable" /> is enumerated only once and only up to the specified index.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="enumerable" /> is null. </exception>
		/// <exception cref="ArgumentOutOfRangeException"> <paramref name="index" /> is less than zero. </exception>
		[SuppressMessage ("ReSharper", "UnusedVariable")]
		public static bool Any <T> (this IEnumerable<T> enumerable, int index)
		{
			if (enumerable == null)
			{
				throw new ArgumentNullException(nameof(enumerable));
			}

			if (index < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(index));
			}

			int count = 0;
			foreach (T item in enumerable)
			{
				if (count == index)
				{
					return true;
				}
				count++;
			}
			return false;
		}

		/// <summary>
		///     Converts any instance implementing <see cref="IEnumerable{T}" /> to an explicit <see cref="IEnumerable{T}" />.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of <paramref name="enumerable" />. </typeparam>
		/// <param name="enumerable"> The instance implementing <see cref="IEnumerable{T}" />. </param>
		/// <returns>
		///     The instance as explicit <see cref="IEnumerable{T}" />.
		/// </returns>
		/// <remarks>
		///     <para>
		///         A conversion to an explicit <see cref="IEnumerable{T}" /> can be useful in cases where the utility/extension methods of <see cref="IEnumerable{T}" /> shall be used instead of the ones implemented by the instance itself.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="enumerable" /> is null. </exception>
		public static IEnumerable<T> AsEnumerable <T> (this IEnumerable<T> enumerable)
		{
			if (enumerable == null)
			{
				throw new ArgumentNullException(nameof(enumerable));
			}

			return enumerable;
		}

		/// <summary>
		///     Hard-casts each element of a sequence into a new list.
		///     An exception is thrown if an element is incompatible to the target type and cannot be converted.
		/// </summary>
		/// <typeparam name="T"> The target type the elements are casted into. </typeparam>
		/// <param name="enumerable"> The sequence which contains the elements. </param>
		/// <returns>
		///     The list which contains the casted elements in the order they were enumerated.
		///     The list is empty if the sequence contains no elements.
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
		/// <exception cref="InvalidCastException"> At least one element cannot be casted into <typeparamref name="T" />. </exception>
		public static List<T> Cast <T> (this IEnumerable enumerable)
		{
			if (enumerable == null)
			{
				throw new ArgumentNullException(nameof(enumerable));
			}

			List<T> items = new List<T>();
			foreach (object item in enumerable)
			{
				items.Add((T)item);
			}
			return items;
		}

		/// <summary>
		///     Concatenates all elements of two sequences into a new list.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of <paramref name="first" /> and <paramref name="second" />. </typeparam>
		/// <param name="first"> The first sequence (whose elements appear at the beginning of the resulting list). </param>
		/// <param name="second"> The second sequence (whose elements appear at the end of the resulting list). </param>
		/// <returns>
		///     The list which contains all the elements of both sequences in the order they were enumerated.
		///     The list is empty if both sequences are empty.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This is a O(n+m) operation where n is the number of elements in <paramref name="first" /> and m is the number of elements in <paramref name="second" />.
		///     </para>
		///     <para>
		///         <paramref name="first" /> and <paramref name="second" /> are each enumerated exactly once.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="first" /> or <paramref name="second" /> is null. </exception>
		public static List<T> Concat <T> (this IEnumerable<T> first, IEnumerable<T> second)
		{
			if (first == null)
			{
				throw new ArgumentNullException(nameof(first));
			}

			if (second == null)
			{
				throw new ArgumentNullException(nameof(second));
			}

			List<T> items = new List<T>();
			foreach (T item in first)
			{
				items.Add(item);
			}
			foreach (T item in second)
			{
				items.Add(item);
			}
			return items;
		}

		/// <summary>
		///     Determines whether a sequence contains a specified element.
		///     Comparison is done using the default equality comparison.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of <paramref name="enumerable" />. </typeparam>
		/// <param name="enumerable"> The sequence which contains the elements. </param>
		/// <param name="item"> The element to find in the sequence. </param>
		/// <returns>
		///     true if the sequence contains the item at least once, false otherwise.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This is a O(n) operation where n is the number of elements in the sequence.
		///     </para>
		///     <para>
		///         <paramref name="enumerable" /> is enumerated only once and only up to the element which first matches the specified element.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="enumerable" /> is null. </exception>
		public static bool Contains <T> (this IEnumerable<T> enumerable, T item)
		{
			return enumerable.Contains(item, EqualityComparer<T>.Default.Equals);
		}

		/// <summary>
		///     Determines whether a sequence contains a specified element.
		///     Comparison is done using the specified equality comparer.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of <paramref name="enumerable" />. </typeparam>
		/// <param name="enumerable"> The sequence which contains the elements. </param>
		/// <param name="item"> The element to find in the sequence. </param>
		/// <param name="comparer"> The equality comparer used to compare the specified element and the elements in the sequence to look for a match. </param>
		/// <returns>
		///     true if the sequence contains the element at least once, false otherwise.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This is a O(n) operation where n is the number of elements in the sequence.
		///     </para>
		///     <para>
		///         <paramref name="enumerable" /> is enumerated only once and only up to the element which first matches the specified element.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="enumerable" /> or <paramref name="comparer" /> is null. </exception>
		public static bool Contains <T> (this IEnumerable<T> enumerable, T item, IEqualityComparer<T> comparer)
		{
			if (enumerable == null)
			{
				throw new ArgumentNullException(nameof(enumerable));
			}

			if (comparer == null)
			{
				throw new ArgumentNullException(nameof(comparer));
			}

			return enumerable.Contains(item, comparer.Equals);
		}

		/// <summary>
		///     Determines whether a sequence contains a specified element.
		///     Comparison is done using the specified function.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of <paramref name="enumerable" />. </typeparam>
		/// <param name="enumerable"> The sequence which contains the elements. </param>
		/// <param name="item"> The element to find in the sequence. </param>
		/// <param name="comparer"> The function used to compare the specified element and the elements in the sequence to look for a match. </param>
		/// <returns>
		///     true if the sequence contains the element at least once, false otherwise.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This is a O(n) operation where n is the number of elements in the sequence.
		///     </para>
		///     <para>
		///         <paramref name="enumerable" /> is enumerated only once and only up to the element which first matches the specified element.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="enumerable" /> or <paramref name="comparer" /> is null. </exception>
		public static bool Contains <T> (this IEnumerable<T> enumerable, T item, Func<T, T, bool> comparer)
		{
			if (enumerable == null)
			{
				throw new ArgumentNullException(nameof(enumerable));
			}

			if (comparer == null)
			{
				throw new ArgumentNullException(nameof(comparer));
			}

			foreach (T currentItem in enumerable)
			{
				if (comparer(currentItem, item))
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		///     Determines how many elements are in a sequence.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of <paramref name="enumerable" />. </typeparam>
		/// <param name="enumerable"> The sequence which contains the elements. </param>
		/// <returns>
		///     The amount of elements in the sequence.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This is a O(n) operation where n is the number of elements in the sequence.
		///     </para>
		///     <para>
		///         <paramref name="enumerable" /> is enumerated exactly once .
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="enumerable" /> is null. </exception>
		[SuppressMessage ("ReSharper", "UnusedVariable")]
		public static int Count <T> (this IEnumerable<T> enumerable)
		{
			if (enumerable == null)
			{
				throw new ArgumentNullException(nameof(enumerable));
			}

			int count = 0;
			foreach (T item in enumerable)
			{
				count++;
			}
			return count;
		}

		/// <summary>
		///     Determines how many elements in a sequence satisfy a specified condition.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of <paramref name="enumerable" />. </typeparam>
		/// <param name="enumerable"> The sequence which contains the elements. </param>
		/// <param name="condition"> The function which tests each element for a condition, providing the element itself. </param>
		/// <returns>
		///     The amount of elements in the sequence which satisfy the condition.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This is a O(n) operation where n is the number of elements in the sequence.
		///     </para>
		///     <para>
		///         <paramref name="enumerable" /> is enumerated exactly once .
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="enumerable" /> or <paramref name="condition" /> is null. </exception>
		public static int Count <T> (this IEnumerable<T> enumerable, Func<T, bool> condition)
		{
			return enumerable.Count((i, e) => condition(e));
		}

		/// <summary>
		///     Determines how many elements in a sequence satisfy a specified condition.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of <paramref name="enumerable" />. </typeparam>
		/// <param name="enumerable"> The sequence which contains the elements. </param>
		/// <param name="condition"> The function which tests each element for a condition, providing the elements index and the element itself. </param>
		/// <returns>
		///     The amount of elements in the sequence which satisfy the condition.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This is a O(n) operation where n is the number of elements in the sequence.
		///     </para>
		///     <para>
		///         <paramref name="enumerable" /> is enumerated exactly once .
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="enumerable" /> or <paramref name="condition" /> is null. </exception>
		public static int Count <T> (this IEnumerable<T> enumerable, Func<int, T, bool> condition)
		{
			if (enumerable == null)
			{
				throw new ArgumentNullException(nameof(enumerable));
			}

			if (condition == null)
			{
				throw new ArgumentNullException(nameof(condition));
			}

			int count = 0;
			int currentIndex = 0;
			foreach (T item in enumerable)
			{
				if (condition(currentIndex, item))
				{
					count++;
				}
				currentIndex++;
			}
			return count;
		}

		/// <summary>
		///     Determines how many elements are located at and after a specified index.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of <paramref name="enumerable" />. </typeparam>
		/// <param name="enumerable"> The sequence which contains the elements. </param>
		/// <param name="index"> The index at which counting begins. </param>
		/// <returns>
		///     The amount of elements in the sequence starting at the specified index.
		///     The return value is zero if the specified index is outside the length of the sequence.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This is a O(n) operation where n is the number of elements in the sequence.
		///     </para>
		///     <para>
		///         <paramref name="enumerable" /> is enumerated exactly once .
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="enumerable" /> is null. </exception>
		/// <exception cref="ArgumentOutOfRangeException"> <paramref name="index" /> is less than zero. </exception>
		[SuppressMessage ("ReSharper", "UnusedVariable")]
		public static int Count <T> (this IEnumerable<T> enumerable, int index)
		{
			if (enumerable == null)
			{
				throw new ArgumentNullException(nameof(enumerable));
			}

			if (index < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(index));
			}

			int count = 0;
			int currentIndex = 0;
			foreach (T item in enumerable)
			{
				if (currentIndex >= index)
				{
					count++;
				}
				currentIndex++;
			}
			return count;
		}

		/// <summary>
		///     Converts a sequence to a set, removing all duplicates.
		///     Comparison is done using the default equality comparison.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of <paramref name="enumerable" />. </typeparam>
		/// <param name="enumerable"> The sequence which contains the elements. </param>
		/// <returns>
		///     The set containing the elements of the sequence without duplicates.
		///     The set is empty if the sequence is empty.
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
		public static HashSet<T> Distinct <T> (this IEnumerable<T> enumerable)
		{
			return enumerable.Distinct(EqualityComparer<T>.Default.Equals);
		}

		/// <summary>
		///     Converts a sequence to a set, removing all duplicates.
		///     Comparison is done using the specified equality comparer.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of <paramref name="enumerable" />. </typeparam>
		/// <param name="enumerable"> The sequence which contains the elements. </param>
		/// <param name="comparer"> The equality comparer used to compare the elements in the sequence to look for duplicates. </param>
		/// <returns>
		///     The set containing the elements of the sequence without duplicates.
		///     The set is empty if the sequence is empty.
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
		public static HashSet<T> Distinct <T> (this IEnumerable<T> enumerable, IEqualityComparer<T> comparer)
		{
			if (enumerable == null)
			{
				throw new ArgumentNullException(nameof(enumerable));
			}

			if (comparer == null)
			{
				throw new ArgumentNullException(nameof(comparer));
			}

			return enumerable.Distinct(comparer.Equals);
		}

		/// <summary>
		///     Converts a sequence to a set, removing all duplicates.
		///     Comparison is done using the specified function.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of <paramref name="enumerable" />. </typeparam>
		/// <param name="enumerable"> The sequence which contains the elements. </param>
		/// <param name="comparer"> The function used to compare the elements in the sequence to look for duplicates. </param>
		/// <returns>
		///     The set containing the elements of the sequence without duplicates.
		///     The set is empty if the sequence is empty.
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
		public static HashSet<T> Distinct <T> (this IEnumerable<T> enumerable, Func<T, T, bool> comparer)
		{
			if (enumerable == null)
			{
				throw new ArgumentNullException(nameof(enumerable));
			}

			if (comparer == null)
			{
				throw new ArgumentNullException(nameof(comparer));
			}

			HashSet<T> result = new HashSet<T>(enumerable, new EqualityComparison<T>(comparer));
			return result;
		}

		/// <summary>
		///     Gets the element at a specified index in a sequence.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of <paramref name="enumerable" />. </typeparam>
		/// <param name="enumerable"> The sequence which contains the elements. </param>
		/// <param name="index"> The index of the required element. </param>
		/// <returns>
		///     The element at the specified index.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This is a O(n) operation where n is the number of elements in the sequence.
		///     </para>
		///     <para>
		///         <paramref name="enumerable" /> is enumerated only once and only up to the specified index.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="enumerable" /> is null. </exception>
		/// <exception cref="ArgumentOutOfRangeException"> <paramref name="index" /> is less than zero. </exception>
		/// <exception cref="InvalidOperationException"> The specified index is outside the length of the sequence. </exception>
		public static T ElementAt <T> (this IEnumerable<T> enumerable, int index)
		{
			if (enumerable == null)
			{
				throw new ArgumentNullException(nameof(enumerable));
			}

			if (index < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(index));
			}

			int count = 0;
			foreach (T item in enumerable)
			{
				if (count == index)
				{
					return item;
				}
				count++;
			}

			throw new InvalidOperationException("The specified index is outside the length of the sequence.");
		}

		/// <summary>
		///     Gets the element at a specified index in a sequence or the default value if the index does not exist.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of <paramref name="enumerable" />. </typeparam>
		/// <param name="enumerable"> The sequence which contains the elements. </param>
		/// <param name="index"> The index of the required element. </param>
		/// <returns>
		///     The element at the specified index.
		///     If the specified index does not exist, the default value for <typeparamref name="T" /> is returned.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This is a O(n) operation where n is the number of elements in the sequence.
		///     </para>
		///     <para>
		///         <paramref name="enumerable" /> is enumerated only once and only up to the specified index.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="enumerable" /> is null. </exception>
		/// <exception cref="ArgumentOutOfRangeException"> <paramref name="index" /> is less than zero. </exception>
		public static T ElementAtOrDefault <T> (this IEnumerable<T> enumerable, int index)
		{
			return enumerable.ElementAtOrDefault(index, default(T));
		}

		/// <summary>
		///     Gets the element at a specified index in a sequence or a default value if the index does not exist.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of <paramref name="enumerable" />. </typeparam>
		/// <param name="enumerable"> The sequence which contains the elements. </param>
		/// <param name="index"> The index of the required element. </param>
		/// <param name="defaultValue"> The default value to return if the specified index does not exist. </param>
		/// <returns>
		///     The element at the specified index.
		///     If the specified index does not exist, the value of <paramref name="defaultValue" /> is returned.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This is a O(n) operation where n is the number of elements in the sequence.
		///     </para>
		///     <para>
		///         <paramref name="enumerable" /> is enumerated only once and only up to the specified index.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="enumerable" /> is null. </exception>
		/// <exception cref="ArgumentOutOfRangeException"> <paramref name="index" /> is less than zero. </exception>
		public static T ElementAtOrDefault <T> (this IEnumerable<T> enumerable, int index, T defaultValue)
		{
			if (enumerable == null)
			{
				throw new ArgumentNullException(nameof(enumerable));
			}

			if (index < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(index));
			}

			int count = 0;
			foreach (T item in enumerable)
			{
				if (count == index)
				{
					return item;
				}
				count++;
			}

			return defaultValue;
		}

		/// <summary>
		///     Converts a sequence to a set which only contains elements not existing in another sequence.
		///     Comparison is done using the default equality comparison.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of <paramref name="enumerable" />. </typeparam>
		/// <param name="enumerable"> The sequence which contains the elements. </param>
		/// <param name="excluded"> The sequence whose elements are to be removed from the resulting set. </param>
		/// <returns>
		///     The set which contains all elements from <paramref name="enumerable" /> which are not in <paramref name="excluded" />.
		///     The set is empty if <paramref name="enumerable" /> is empty or all its elements are also in <paramref name="excluded" />.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This is a O(n*m) operation where n is the number of elements in <paramref name="enumerable" /> and m is the number of elements in <paramref name="excluded" />.
		///     </para>
		///     <para>
		///         <paramref name="enumerable" /> and <paramref name="excluded" /> are each enumerated exactly once.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="enumerable" /> or <paramref name="excluded" /> is null. </exception>
		public static HashSet<T> Except <T> (this IEnumerable<T> enumerable, IEnumerable<T> excluded)
		{
			return enumerable.Except(excluded, EqualityComparer<T>.Default.Equals);
		}

		/// <summary>
		///     Converts a sequence to a set which only contains elements not existing in another sequence.
		///     Comparison is done using the specified equality comparer.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of <paramref name="enumerable" />. </typeparam>
		/// <param name="enumerable"> The sequence which contains the elements. </param>
		/// <param name="excluded"> The sequence whose elements are to be removed from the resulting set. </param>
		/// <param name="comparer"> The equality comparer used to compare the elements in the sequences to look for matches. </param>
		/// <returns>
		///     The set which contains all elements from <paramref name="enumerable" /> which are not in <paramref name="excluded" />.
		///     The set is empty if <paramref name="enumerable" /> is empty or all its elements are also in <paramref name="excluded" />.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This is a O(n*m) operation where n is the number of elements in <paramref name="enumerable" /> and m is the number of elements in <paramref name="excluded" />.
		///     </para>
		///     <para>
		///         <paramref name="enumerable" /> and <paramref name="excluded" /> are each enumerated exactly once.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="enumerable" />, <paramref name="excluded" />, or <paramref name="comparer" /> is null. </exception>
		public static HashSet<T> Except <T> (this IEnumerable<T> enumerable, IEnumerable<T> excluded, IEqualityComparer<T> comparer)
		{
			if (enumerable == null)
			{
				throw new ArgumentNullException(nameof(enumerable));
			}

			if (excluded == null)
			{
				throw new ArgumentNullException(nameof(excluded));
			}

			if (comparer == null)
			{
				throw new ArgumentNullException(nameof(comparer));
			}

			return enumerable.Except(excluded, comparer.Equals);
		}

		/// <summary>
		///     Converts a sequence to a set which only contains elements not existing in another sequence.
		///     Comparison is done using the specified function.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of <paramref name="enumerable" />. </typeparam>
		/// <param name="enumerable"> The sequence which contains the elements. </param>
		/// <param name="excluded"> The sequence whose elements are to be removed from the resulting set. </param>
		/// <param name="comparer"> The function used to compare the elements in the sequences to look for matches. </param>
		/// <returns>
		///     The set which contains all elements from <paramref name="enumerable" /> which are not in <paramref name="excluded" />.
		///     The set is empty if <paramref name="enumerable" /> is empty or all its elements are also in <paramref name="excluded" />.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This is a O(n*m) operation where n is the number of elements in <paramref name="enumerable" /> and m is the number of elements in <paramref name="excluded" />.
		///     </para>
		///     <para>
		///         <paramref name="enumerable" /> and <paramref name="excluded" /> are each enumerated exactly once.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="enumerable" />, <paramref name="excluded" />, or <paramref name="comparer" /> is null. </exception>
		public static HashSet<T> Except <T> (this IEnumerable<T> enumerable, IEnumerable<T> excluded, Func<T, T, bool> comparer)
		{
			if (enumerable == null)
			{
				throw new ArgumentNullException(nameof(enumerable));
			}

			if (excluded == null)
			{
				throw new ArgumentNullException(nameof(excluded));
			}

			if (comparer == null)
			{
				throw new ArgumentNullException(nameof(comparer));
			}

			HashSet<T> result = new HashSet<T>(enumerable, new EqualityComparison<T>(comparer));
			result.ExceptWith(excluded);
			return result;
		}

		/// <summary>
		///     Gets the first element of a sequence.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of <paramref name="enumerable" />. </typeparam>
		/// <param name="enumerable"> The sequence which contains the elements. </param>
		/// <returns>
		///     The first element in the sequence.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This is a O(1) operation.
		///     </para>
		///     <para>
		///         <paramref name="enumerable" /> is enumerated only once and only up to the first element.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="enumerable" /> is null. </exception>
		/// <exception cref="InvalidOperationException"> The sequence is empty and thus cannot provide a first element. </exception>
		public static T First <T> (this IEnumerable<T> enumerable)
		{
			if (enumerable == null)
			{
				throw new ArgumentNullException(nameof(enumerable));
			}

			foreach (T item in enumerable)
			{
				return item;
			}

			throw new InvalidOperationException("The sequence is empty and thus cannot provide a first element.");
		}

		/// <summary>
		///     Gets the first element of a sequence which satisfies a specified condition.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of <paramref name="enumerable" />. </typeparam>
		/// <param name="enumerable"> The sequence which contains the elements. </param>
		/// <param name="condition"> The function which tests each element for a condition, providing the element itself. </param>
		/// <returns>
		///     The first element in the sequence which satisfies the specified condition.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This is a O(n) operation where n is the number of elements in the sequence.
		///     </para>
		///     <para>
		///         <paramref name="enumerable" /> is enumerated only once and only up to the first element which satisfies the condition.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="enumerable" /> or <paramref name="condition" /> is null. </exception>
		/// <exception cref="InvalidOperationException"> The sequence does not contain any element which satisfies the specified condition and thus cannot provide a first element. </exception>
		public static T First <T> (this IEnumerable<T> enumerable, Func<T, bool> condition)
		{
			return enumerable.First((i, e) => condition(e));
		}

		/// <summary>
		///     Gets the first element of a sequence which satisfies a specified condition.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of <paramref name="enumerable" />. </typeparam>
		/// <param name="enumerable"> The sequence which contains the elements. </param>
		/// <param name="condition"> The function which tests each element for a condition, providing the elements index and the element itself. </param>
		/// <returns>
		///     The first element in the sequence which satisfies the specified condition.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This is a O(n) operation where n is the number of elements in the sequence.
		///     </para>
		///     <para>
		///         <paramref name="enumerable" /> is enumerated only once and only up to the first element which satisfies the condition.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="enumerable" /> or <paramref name="condition" /> is null. </exception>
		/// <exception cref="InvalidOperationException"> The sequence does not contain any element which satisfies the specified condition and thus cannot provide a first element. </exception>
		public static T First <T> (this IEnumerable<T> enumerable, Func<int, T, bool> condition)
		{
			if (enumerable == null)
			{
				throw new ArgumentNullException(nameof(enumerable));
			}

			if (condition == null)
			{
				throw new ArgumentNullException(nameof(condition));
			}

			int count = 0;
			foreach (T item in enumerable)
			{
				if (condition(count, item))
				{
					return item;
				}
				count++;
			}

			throw new InvalidOperationException("The sequence does not contain any element which satisfies the specified condition and thus cannot provide a first element.");
		}

		/// <summary>
		///     Gets the first element of a sequence or the default value if the sequence is empty.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of <paramref name="enumerable" />. </typeparam>
		/// <param name="enumerable"> The sequence which contains the elements. </param>
		/// <returns>
		///     The first element of the sequence.
		///     If the sequence is empty, the default value for <typeparamref name="T" /> is returned.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This is a O(1) operation.
		///     </para>
		///     <para>
		///         <paramref name="enumerable" /> is enumerated only once and only up to the first element.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="enumerable" /> is null. </exception>
		public static T FirstOrDefault <T> (this IEnumerable<T> enumerable)
		{
			return enumerable.FirstOrDefault(default(T));
		}

		/// <summary>
		///     Gets the first element of a sequence or a default value if the sequence is empty.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of <paramref name="enumerable" />. </typeparam>
		/// <param name="enumerable"> The sequence which contains the elements. </param>
		/// <param name="defaultValue"> The default value to return if the sequence is empty. </param>
		/// <returns>
		///     The first element of the sequence.
		///     If the sequence is empty, the value of <paramref name="defaultValue" /> is returned.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This is a O(1) operation.
		///     </para>
		///     <para>
		///         <paramref name="enumerable" /> is enumerated only once and only up to the first element.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="enumerable" /> is null. </exception>
		public static T FirstOrDefault <T> (this IEnumerable<T> enumerable, T defaultValue)
		{
			if (enumerable == null)
			{
				throw new ArgumentNullException(nameof(enumerable));
			}

			foreach (T item in enumerable)
			{
				return item;
			}

			return defaultValue;
		}

		/// <summary>
		///     Gets the first element of a sequence which satisfies a specified condition or the default value if the sequence is empty.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of <paramref name="enumerable" />. </typeparam>
		/// <param name="enumerable"> The sequence which contains the elements. </param>
		/// <param name="condition"> The function which tests each element for a condition, providing the element itself. </param>
		/// <returns>
		///     The first element of the sequence which satisfies the specified condition.
		///     If no element satisfies the specified condition, the default value for <typeparamref name="T" /> is returned.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This is a O(n) operation where n is the number of elements in the sequence.
		///     </para>
		///     <para>
		///         <paramref name="enumerable" /> is enumerated only once and only up to the first element which satisfies the condition.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="enumerable" /> or <paramref name="condition" /> is null. </exception>
		public static T FirstOrDefault <T> (this IEnumerable<T> enumerable, Func<T, bool> condition)
		{
			return enumerable.FirstOrDefault((i, e) => condition(e));
		}

		/// <summary>
		///     Gets the first element of a sequence which satisfies a specified condition or the default value if the sequence is empty.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of <paramref name="enumerable" />. </typeparam>
		/// <param name="enumerable"> The sequence which contains the elements. </param>
		/// <param name="condition"> The function which tests each element for a condition, providing the elements index and the element itself. </param>
		/// <returns>
		///     The first element of the sequence which satisfies the specified condition.
		///     If no element satisfies the specified condition, the default value for <typeparamref name="T" /> is returned.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This is a O(n) operation where n is the number of elements in the sequence.
		///     </para>
		///     <para>
		///         <paramref name="enumerable" /> is enumerated only once and only up to the first element which satisfies the condition.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="enumerable" /> or <paramref name="condition" /> is null. </exception>
		public static T FirstOrDefault <T> (this IEnumerable<T> enumerable, Func<int, T, bool> condition)
		{
			return enumerable.FirstOrDefault(default(T), condition);
		}

		/// <summary>
		///     Gets the first element of a sequence which satisfies a specified condition or a default value if the sequence is empty.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of <paramref name="enumerable" />. </typeparam>
		/// <param name="enumerable"> The sequence which contains the elements. </param>
		/// <param name="defaultValue"> The default value to return if the sequence does not contain any element which satisfies the specified condition. </param>
		/// <param name="condition"> The function which tests each element for a condition, providing the element itself. </param>
		/// <returns>
		///     The first element of the sequence which satisfies the specified condition.
		///     If no element satisfies the specified condition, the value of <paramref name="defaultValue" /> is returned.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This is a O(n) operation where n is the number of elements in the sequence.
		///     </para>
		///     <para>
		///         <paramref name="enumerable" /> is enumerated only once and only up to the first element which satisfies the condition.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="enumerable" /> or <paramref name="condition" /> is null. </exception>
		public static T FirstOrDefault <T> (this IEnumerable<T> enumerable, T defaultValue, Func<T, bool> condition)
		{
			return enumerable.FirstOrDefault(defaultValue, (i, e) => condition(e));
		}

		/// <summary>
		///     Gets the first element of a sequence which satisfies a specified condition or a default value if the sequence is empty.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of <paramref name="enumerable" />. </typeparam>
		/// <param name="enumerable"> The sequence which contains the elements. </param>
		/// <param name="defaultValue"> The default value to return if the sequence does not contain any element which satisfies the specified condition. </param>
		/// <param name="condition"> The function which tests each element for a condition, providing the elements index and the element itself. </param>
		/// <returns>
		///     The first element of the sequence which satisfies the specified condition.
		///     If no element satisfies the specified condition, the value of <paramref name="defaultValue" /> is returned.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This is a O(n) operation where n is the number of elements in the sequence.
		///     </para>
		///     <para>
		///         <paramref name="enumerable" /> is enumerated only once and only up to the first element which satisfies the condition.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="enumerable" /> or <paramref name="condition" /> is null. </exception>
		public static T FirstOrDefault <T> (this IEnumerable<T> enumerable, T defaultValue, Func<int, T, bool> condition)
		{
			if (enumerable == null)
			{
				throw new ArgumentNullException(nameof(enumerable));
			}

			if (condition == null)
			{
				throw new ArgumentNullException(nameof(condition));
			}

			int count = 0;
			foreach (T item in enumerable)
			{
				if (condition(count, item))
				{
					return item;
				}
				count++;
			}

			return defaultValue;
		}

		/// <summary>
		///     Converts two sequences to a set which only contains elements which are in both sequences.
		///     Comparison is done using the default equality comparison.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of <paramref name="first" /> and <paramref name="second" />. </typeparam>
		/// <param name="first"> The first sequence whose elements are only added to the resulting set if they are also in <paramref name="second" />. </param>
		/// <param name="second"> The second sequence whose elements are only added to the resulting set if they are also in <paramref name="first" />. </param>
		/// <returns>
		///     The set which contains all elements from both <paramref name="first" /> and <paramref name="second" /> which are in both.
		///     The set is empty if one or both sequences are empty or if they only contain elements which are not in both sequences.
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
		public static HashSet<T> Intersect <T> (this IEnumerable<T> first, IEnumerable<T> second)
		{
			return first.Intersect(second, EqualityComparer<T>.Default.Equals);
		}

		/// <summary>
		///     Converts two sequences to a set which only contains elements which are in both sequences.
		///     Comparison is done using the specified equality comparer.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of <paramref name="first" /> and <paramref name="second" />. </typeparam>
		/// <param name="first"> The first sequence whose elements are only added to the resulting set if they are also in <paramref name="second" />. </param>
		/// <param name="second"> The second sequence whose elements are only added to the resulting set if they are also in <paramref name="first" />. </param>
		/// <param name="comparer"> The equality comparer used to compare the elements in the sequences to look for matches. </param>
		/// <returns>
		///     The set which contains all elements from both <paramref name="first" /> and <paramref name="second" /> which are in both.
		///     The set is empty if one or both sequences are empty or if they only contain elements which are not in both sequences.
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
		public static HashSet<T> Intersect <T> (this IEnumerable<T> first, IEnumerable<T> second, IEqualityComparer<T> comparer)
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

			return first.Intersect(second, comparer.Equals);
		}

		/// <summary>
		///     Converts two sequences to a set which only contains elements which are in both sequences.
		///     Comparison is done using the specified function.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of <paramref name="first" /> and <paramref name="second" />. </typeparam>
		/// <param name="first"> The first sequence whose elements are only added to the resulting set if they are also in <paramref name="second" />. </param>
		/// <param name="second"> The second sequence whose elements are only added to the resulting set if they are also in <paramref name="first" />. </param>
		/// <param name="comparer"> The function used to compare the elements in the sequences to look for matches. </param>
		/// <returns>
		///     The set which contains all elements from both <paramref name="first" /> and <paramref name="second" /> which are in both.
		///     The set is empty if one or both sequences are empty or if they only contain elements which are not in both sequences.
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
		public static HashSet<T> Intersect <T> (this IEnumerable<T> first, IEnumerable<T> second, Func<T, T, bool> comparer)
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
			result.IntersectWith(second);
			return result;
		}

		/// <summary>
		///     Gets the last element of a sequence.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of <paramref name="enumerable" />. </typeparam>
		/// <param name="enumerable"> The sequence which contains the elements. </param>
		/// <returns>
		///     The last element in the sequence.
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
		/// <exception cref="InvalidOperationException"> The sequence is empty and thus cannot provide a last element. </exception>
		public static T Last <T> (this IEnumerable<T> enumerable)
		{
			if (enumerable == null)
			{
				throw new ArgumentNullException(nameof(enumerable));
			}

			bool found = false;
			T last = default(T);
			foreach (T item in enumerable)
			{
				found = true;
				last = item;
			}
			if (found)
			{
				return last;
			}

			throw new InvalidOperationException("The sequence is empty and thus cannot provide a last element.");
		}

		/// <summary>
		///     Gets the last element of a sequence which satisfies a specified condition.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of <paramref name="enumerable" />. </typeparam>
		/// <param name="enumerable"> The sequence which contains the elements. </param>
		/// <param name="condition"> The function which tests each element for a condition, providing the element itself. </param>
		/// <returns>
		///     The last element in the sequence which satisfies the specified condition.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This is a O(n) operation where n is the number of elements in the sequence.
		///     </para>
		///     <para>
		///         <paramref name="enumerable" /> is enumerated exactly once.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="enumerable" /> or <paramref name="condition" /> is null. </exception>
		/// <exception cref="InvalidOperationException"> The sequence does not contain any element which satisfies the specified condition and thus cannot provide a last element. </exception>
		public static T Last <T> (this IEnumerable<T> enumerable, Func<T, bool> condition)
		{
			return enumerable.Last((i, e) => condition(e));
		}

		/// <summary>
		///     Gets the last element of a sequence which satisfies a specified condition.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of <paramref name="enumerable" />. </typeparam>
		/// <param name="enumerable"> The sequence which contains the elements. </param>
		/// <param name="condition"> The function which tests each element for a condition, providing the elements index and the element itself. </param>
		/// <returns>
		///     The last element in the sequence which satisfies the specified condition.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This is a O(n) operation where n is the number of elements in the sequence.
		///     </para>
		///     <para>
		///         <paramref name="enumerable" /> is enumerated exactly once.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="enumerable" /> or <paramref name="condition" /> is null. </exception>
		/// <exception cref="InvalidOperationException"> The sequence does not contain any element which satisfies the specified condition and thus cannot provide a last element. </exception>
		public static T Last <T> (this IEnumerable<T> enumerable, Func<int, T, bool> condition)
		{
			if (enumerable == null)
			{
				throw new ArgumentNullException(nameof(enumerable));
			}

			if (condition == null)
			{
				throw new ArgumentNullException(nameof(condition));
			}

			bool found = false;
			T last = default(T);
			int count = 0;
			foreach (T item in enumerable)
			{
				if (condition(count, item))
				{
					found = true;
					last = item;
				}
				count++;
			}
			if (found)
			{
				return last;
			}

			throw new InvalidOperationException("The sequence does not contain any element which satisfies the specified condition and thus cannot provide a last element.");
		}

		/// <summary>
		///     Gets the last element of a sequence or the default value if the sequence is empty.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of <paramref name="enumerable" />. </typeparam>
		/// <param name="enumerable"> The sequence which contains the elements. </param>
		/// <returns>
		///     The last element of the sequence.
		///     If the sequence is empty, the default value for <typeparamref name="T" /> is returned.
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
		public static T LastOrDefault <T> (this IEnumerable<T> enumerable)
		{
			return enumerable.LastOrDefault(default(T));
		}

		/// <summary>
		///     Gets the last element of a sequence or a default value if the sequence is empty.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of <paramref name="enumerable" />. </typeparam>
		/// <param name="enumerable"> The sequence which contains the elements. </param>
		/// <param name="defaultValue"> The default value to return if the sequence is empty. </param>
		/// <returns>
		///     The last element of the sequence.
		///     If the sequence is empty, the value of <paramref name="defaultValue" /> is returned.
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
		public static T LastOrDefault <T> (this IEnumerable<T> enumerable, T defaultValue)
		{
			if (enumerable == null)
			{
				throw new ArgumentNullException(nameof(enumerable));
			}

			T last = defaultValue;
			foreach (T item in enumerable)
			{
				last = item;
			}
			return last;
		}

		/// <summary>
		///     Gets the last element of a sequence which satisfies a specified condition or the default value if the sequence is empty.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of <paramref name="enumerable" />. </typeparam>
		/// <param name="enumerable"> The sequence which contains the elements. </param>
		/// <param name="condition"> The function which tests each element for a condition, providing the element itself. </param>
		/// <returns>
		///     The last element of the sequence which satisfies the specified condition.
		///     If no element satisfies the specified condition, the default value for <typeparamref name="T" /> is returned.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This is a O(n) operation where n is the number of elements in the sequence.
		///     </para>
		///     <para>
		///         <paramref name="enumerable" /> is enumerated exactly once.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="enumerable" /> or <paramref name="condition" /> is null. </exception>
		public static T LastOrDefault <T> (this IEnumerable<T> enumerable, Func<T, bool> condition)
		{
			return enumerable.LastOrDefault((i, e) => condition(e));
		}

		/// <summary>
		///     Gets the last element of a sequence which satisfies a specified condition or the default value if the sequence is empty.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of <paramref name="enumerable" />. </typeparam>
		/// <param name="enumerable"> The sequence which contains the elements. </param>
		/// <param name="condition"> The function which tests each element for a condition, providing the elements index and the element itself. </param>
		/// <returns>
		///     The last element of the sequence which satisfies the specified condition.
		///     If no element satisfies the specified condition, the default value for <typeparamref name="T" /> is returned.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This is a O(n) operation where n is the number of elements in the sequence.
		///     </para>
		///     <para>
		///         <paramref name="enumerable" /> is enumerated exactly once.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="enumerable" /> or <paramref name="condition" /> is null. </exception>
		public static T LastOrDefault <T> (this IEnumerable<T> enumerable, Func<int, T, bool> condition)
		{
			return enumerable.LastOrDefault(default(T), condition);
		}

		/// <summary>
		///     Gets the last element of a sequence which satisfies a specified condition or a default value if the sequence is empty.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of <paramref name="enumerable" />. </typeparam>
		/// <param name="enumerable"> The sequence which contains the elements. </param>
		/// <param name="defaultValue"> The default value to return if the sequence does not contain any element which satisfies the specified condition. </param>
		/// <param name="condition"> The function which tests each element for a condition, providing the element itself. </param>
		/// <returns>
		///     The last element of the sequence which satisfies the specified condition.
		///     If no element satisfies the specified condition, the value of <paramref name="defaultValue" /> is returned.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This is a O(n) operation where n is the number of elements in the sequence.
		///     </para>
		///     <para>
		///         <paramref name="enumerable" /> is enumerated exactly once.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="enumerable" /> or <paramref name="condition" /> is null. </exception>
		public static T LastOrDefault <T> (this IEnumerable<T> enumerable, T defaultValue, Func<T, bool> condition)
		{
			return enumerable.LastOrDefault(defaultValue, (i, e) => condition(e));
		}

		/// <summary>
		///     Gets the last element of a sequence which satisfies a specified condition or a default value if the sequence is empty.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of <paramref name="enumerable" />. </typeparam>
		/// <param name="enumerable"> The sequence which contains the elements. </param>
		/// <param name="defaultValue"> The default value to return if the sequence does not contain any element which satisfies the specified condition. </param>
		/// <param name="condition"> The function which tests each element for a condition, providing the elements index and the element itself. </param>
		/// <returns>
		///     The last element of the sequence which satisfies the specified condition.
		///     If no element satisfies the specified condition, the value of <paramref name="defaultValue" /> is returned.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This is a O(n) operation where n is the number of elements in the sequence.
		///     </para>
		///     <para>
		///         <paramref name="enumerable" /> is enumerated exactly once.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="enumerable" /> or <paramref name="condition" /> is null. </exception>
		public static T LastOrDefault <T> (this IEnumerable<T> enumerable, T defaultValue, Func<int, T, bool> condition)
		{
			if (enumerable == null)
			{
				throw new ArgumentNullException(nameof(enumerable));
			}

			if (condition == null)
			{
				throw new ArgumentNullException(nameof(condition));
			}

			T last = defaultValue;
			int count = 0;
			foreach (T item in enumerable)
			{
				if (condition(count, item))
				{
					last = item;
				}
				count++;
			}
			return last;
		}

		/// <summary>
		///     Soft-casts each element of a sequence into a new list.
		///     An element which is incompatible to the target type and cannot be converted is ignored.
		/// </summary>
		/// <typeparam name="T"> The type the elements are casted into. </typeparam>
		/// <param name="enumerable"> The sequence which contains the elements. </param>
		/// <returns>
		///     The list which contains the casted elements in the order they were enumerated.
		///     The list is empty if the sequence contains no elements or only incompatible elements which could not be casted.
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
		public static List<T> OfType <T> (this IEnumerable enumerable)
		{
			if (enumerable == null)
			{
				throw new ArgumentNullException(nameof(enumerable));
			}

			List<T> items = new List<T>();
			foreach (object item in enumerable)
			{
				if (item is T)
				{
					items.Add((T)item);
				}
			}
			return items;
		}

		/// <summary>
		///     Reverses the order of the elements of a sequence into a new list.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of <paramref name="enumerable" />. </typeparam>
		/// <param name="enumerable"> The sequence which contains the elements. </param>
		/// <returns>
		///     The list which contains the elements of the sequence in reverse order as they were enumerated.
		///     The list is empty if the sequence contains no elements.
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
		public static List<T> Reverse <T> (this IEnumerable<T> enumerable)
		{
			if (enumerable == null)
			{
				throw new ArgumentNullException(nameof(enumerable));
			}

			List<T> items = new List<T>(enumerable);
			items.Reverse();
			return items;
		}

		/// <summary>
		///     Selects and converts each element of a sequence into a new list using a specified function.
		/// </summary>
		/// <typeparam name="TIn"> The type of the elements of <paramref name="enumerable" />. </typeparam>
		/// <typeparam name="TOut"> The type of the elements in the resulting list, after conversion. </typeparam>
		/// <param name="enumerable"> The sequence which contains the elements. </param>
		/// <param name="selector"> The selector and conversion function which is called for each element, providing the element itself. </param>
		/// <returns>
		///     The list which contains all the values returned by <paramref name="selector" /> in the order they were enumerated.
		///     The list is empty if the sequence contains no elements.
		///     Note that the values returned by <paramref name="selector" /> are not filtered and added as-is.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This is a O(n) operation where n is the number of elements in the sequence.
		///     </para>
		///     <para>
		///         <paramref name="enumerable" /> is enumerated exactly once.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="enumerable" /> or <paramref name="selector" /> is null. </exception>
		public static List<TOut> Select <TIn, TOut> (this IEnumerable<TIn> enumerable, Func<TIn, TOut> selector)
		{
			return enumerable.Select((i, e) => selector(e));
		}

		/// <summary>
		///     Selects and converts each element of a sequence into a new list using a specified function.
		/// </summary>
		/// <typeparam name="TIn"> The type of the elements of <paramref name="enumerable" />. </typeparam>
		/// <typeparam name="TOut"> The type of the elements in the resulting list, after conversion. </typeparam>
		/// <param name="enumerable"> The sequence which contains the elements. </param>
		/// <param name="selector"> The selector and conversion function which is called for each element, providing the elements index and the element itself. </param>
		/// <returns>
		///     The list which contains all the values returned by <paramref name="selector" /> in the order they were enumerated.
		///     The list is empty if the sequence contains no elements.
		///     Note that the values returned by <paramref name="selector" /> are not filtered and added as-is.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This is a O(n) operation where n is the number of elements in the sequence.
		///     </para>
		///     <para>
		///         <paramref name="enumerable" /> is enumerated exactly once.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="enumerable" /> or <paramref name="selector" /> is null. </exception>
		public static List<TOut> Select <TIn, TOut> (this IEnumerable<TIn> enumerable, Func<int, TIn, TOut> selector)
		{
			if (enumerable == null)
			{
				throw new ArgumentNullException(nameof(enumerable));
			}

			if (selector == null)
			{
				throw new ArgumentNullException(nameof(selector));
			}

			List<TOut> results = new List<TOut>();
			int index = 0;
			foreach (TIn item in enumerable)
			{
				results.Add(selector(index, item));
				index++;
			}
			return results;
		}

		/// <summary>
		///     Selects and converts each element of a sequence into multiple values using a specified function.
		/// </summary>
		/// <typeparam name="TIn"> The type of the elements of <paramref name="enumerable" />. </typeparam>
		/// <typeparam name="TOut"> The type of the elements in the resulting list, after conversion. </typeparam>
		/// <param name="enumerable"> The sequence which contains the elements. </param>
		/// <param name="selector"> The selector and conversion function which is called for each element, providing the element itself, creating a sequence of zero, one, or multiple resulting values. </param>
		/// <returns>
		///     The list which contains all the values returned by <paramref name="selector" /> in the order they were enumerated.
		///     The list is empty if the sequence contains no elements or <paramref name="selector" /> only returns sequences with no elements.
		///     Note that the values returned by <paramref name="selector" /> are not filtered and added as-is.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This is a O(n) operation where n is the number of elements in the sequence.
		///     </para>
		///     <para>
		///         <paramref name="enumerable" /> is enumerated exactly once.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="enumerable" /> or <paramref name="selector" /> is null. </exception>
		public static List<TOut> SelectMany <TIn, TOut> (this IEnumerable<TIn> enumerable, Func<TIn, IEnumerable<TOut>> selector)
		{
			return enumerable.SelectMany((i, e) => selector(e));
		}

		/// <summary>
		///     Selects and converts each element of a sequence into multiple values using a specified function.
		/// </summary>
		/// <typeparam name="TIn"> The type of the elements of <paramref name="enumerable" />. </typeparam>
		/// <typeparam name="TOut"> The type of the elements in the resulting list, after conversion. </typeparam>
		/// <param name="enumerable"> The sequence which contains the elements. </param>
		/// <param name="selector"> The selector and conversion function which is called for each element, providing the elements index and the element itself, creating a sequence of zero, one, or multiple resulting values. </param>
		/// <returns>
		///     The list which contains all the values returned by <paramref name="selector" /> in the order they were enumerated.
		///     The list is empty if the sequence contains no elements or <paramref name="selector" /> only returns sequences with no elements.
		///     Note that the values returned by <paramref name="selector" /> are not filtered and added as-is.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This is a O(n) operation where n is the number of elements in the sequence.
		///     </para>
		///     <para>
		///         <paramref name="enumerable" /> is enumerated exactly once.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="enumerable" /> or <paramref name="selector" /> is null. </exception>
		public static List<TOut> SelectMany <TIn, TOut> (this IEnumerable<TIn> enumerable, Func<int, TIn, IEnumerable<TOut>> selector)
		{
			if (enumerable == null)
			{
				throw new ArgumentNullException(nameof(enumerable));
			}

			if (selector == null)
			{
				throw new ArgumentNullException(nameof(selector));
			}

			List<TOut> results = new List<TOut>();
			int index = 0;
			foreach (TIn item in enumerable)
			{
				results.AddRange(selector(index, item));
				index++;
			}
			return results;
		}

		/// <summary>
		///     Compares two sequences and determines whether they contain the same elements.
		///     Comparison is done using the default equality comparison.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of <paramref name="x" /> and <paramref name="y" />. </typeparam>
		/// <param name="x"> The first of the two sequences whose elements are compared against the elements of <paramref name="y" />. </param>
		/// <param name="y"> The second of the two sequences whose elements are compared against the elements of <paramref name="x" />. </param>
		/// <returns>
		///     true if both sequences are equal or contain the same elements respectively.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This is a O(n+m) operation, where n is the number of elements in <paramref name="x" /> and m is the number of elements in <paramref name="y" />.
		///     </para>
		///     <para>
		///         <paramref name="x" /> and <paramref name="y" /> are each enumerated exactly once.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="x" /> or <paramref name="y" /> is null. </exception>
		public static bool SequenceEqual <T> (this IEnumerable<T> x, IEnumerable<T> y)
		{
			if (x == null)
			{
				throw new ArgumentNullException(nameof(x));
			}

			if (y == null)
			{
				throw new ArgumentNullException(nameof(y));
			}

			return CollectionComparer<T>.Default.Equals(x, y);
		}

		/// <summary>
		///     Compares two sequences and determines whether they contain the same elements.
		///     Comparison is done using the default equality comparison.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of <paramref name="x" /> and <paramref name="y" />. </typeparam>
		/// <param name="x"> The first of the two sequences whose elements are compared against the elements of <paramref name="y" />. </param>
		/// <param name="y"> The second of the two sequences whose elements are compared against the elements of <paramref name="x" />. </param>
		/// <param name="options"> The options which specify comparison options. </param>
		/// <returns>
		///     true if both sequences are equal or contain the same elements respectively.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This is a O(n*m) operation if order is ignored or a O(n+m) operation if order is not ignored, where n is the number of elements in <paramref name="x" /> and m is the number of elements in <paramref name="y" />.
		///     </para>
		///     <para>
		///         <paramref name="x" /> and <paramref name="y" /> are each enumerated exactly once.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="x" /> or <paramref name="y" /> is null. </exception>
		public static bool SequenceEqual <T> (this IEnumerable<T> x, IEnumerable<T> y, CollectionComparerFlags options)
		{
			if (x == null)
			{
				throw new ArgumentNullException(nameof(x));
			}

			if (y == null)
			{
				throw new ArgumentNullException(nameof(y));
			}

			CollectionComparer<T> collectionComparer = new CollectionComparer<T>(options);
			return collectionComparer.Equals(x, y);
		}

		/// <summary>
		///     Compares two sequences and determines whether they contain the same elements.
		///     Comparison is done using the specified equality comparer.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of <paramref name="x" /> and <paramref name="y" />. </typeparam>
		/// <param name="x"> The first of the two sequences whose elements are compared against the elements of <paramref name="y" />. </param>
		/// <param name="y"> The second of the two sequences whose elements are compared against the elements of <paramref name="x" />. </param>
		/// <param name="comparer"> The equality comparer used to compare the elements in the sequences to look for matches. </param>
		/// <returns>
		///     true if both sequences are equal or contain the same elements respectively.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This is a O(n*m) operation if order is ignored or a O(n+m) operation if order is not ignored, where n is the number of elements in <paramref name="x" /> and m is the number of elements in <paramref name="y" />.
		///     </para>
		///     <para>
		///         <paramref name="x" /> and <paramref name="y" /> are each enumerated exactly once.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="x" />, <paramref name="y" />, or <paramref name="comparer" /> is null. </exception>
		public static bool SequenceEqual <T> (this IEnumerable<T> x, IEnumerable<T> y, IEqualityComparer<T> comparer)
		{
			if (x == null)
			{
				throw new ArgumentNullException(nameof(x));
			}

			if (y == null)
			{
				throw new ArgumentNullException(nameof(y));
			}

			if (comparer == null)
			{
				throw new ArgumentNullException(nameof(comparer));
			}

			CollectionComparer<T> collectionComparer = new CollectionComparer<T>(comparer);
			return collectionComparer.Equals(x, y);
		}

		/// <summary>
		///     Compares two sequences and determines whether they contain the same elements.
		///     Comparison is done using the specified function.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of <paramref name="x" /> and <paramref name="y" />. </typeparam>
		/// <param name="x"> The first of the two sequences whose elements are compared against the elements of <paramref name="y" />. </param>
		/// <param name="y"> The second of the two sequences whose elements are compared against the elements of <paramref name="x" />. </param>
		/// <param name="comparer"> The function used to compare the elements in the sequences to look for matches. </param>
		/// <returns>
		///     true if both sequences are equal or contain the same elements respectively.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This is a O(n*m) operation if order is ignored or a O(n+m) operation if order is not ignored, where n is the number of elements in <paramref name="x" /> and m is the number of elements in <paramref name="y" />.
		///     </para>
		///     <para>
		///         <paramref name="x" /> and <paramref name="y" /> are each enumerated exactly once.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="x" />, <paramref name="y" />, or <paramref name="comparer" /> is null. </exception>
		public static bool SequenceEqual <T> (this IEnumerable<T> x, IEnumerable<T> y, Func<T, T, bool> comparer)
		{
			if (x == null)
			{
				throw new ArgumentNullException(nameof(x));
			}

			if (y == null)
			{
				throw new ArgumentNullException(nameof(y));
			}

			if (comparer == null)
			{
				throw new ArgumentNullException(nameof(comparer));
			}

			CollectionComparer<T> collectionComparer = new CollectionComparer<T>(comparer);
			return collectionComparer.Equals(x, y);
		}

		/// <summary>
		///     Compares two sequences and determines whether they contain the same elements.
		///     Comparison is done using the specified equality comparer.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of <paramref name="x" /> and <paramref name="y" />. </typeparam>
		/// <param name="x"> The first of the two sequences whose elements are compared against the elements of <paramref name="y" />. </param>
		/// <param name="y"> The second of the two sequences whose elements are compared against the elements of <paramref name="x" />. </param>
		/// <param name="options"> The options which specify comparison options. </param>
		/// <param name="comparer"> The equality comparer used to compare the elements in the sequences to look for matches. </param>
		/// <returns>
		///     true if both sequences are equal or contain the same elements respectively.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This is a O(n*m) operation if order is ignored or a O(n+m) operation if order is not ignored, where n is the number of elements in <paramref name="x" /> and m is the number of elements in <paramref name="y" />.
		///     </para>
		///     <para>
		///         <paramref name="x" /> and <paramref name="y" /> are each enumerated exactly once.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="x" />, <paramref name="y" />, or <paramref name="comparer" /> is null. </exception>
		public static bool SequenceEqual <T> (this IEnumerable<T> x, IEnumerable<T> y, CollectionComparerFlags options, IEqualityComparer<T> comparer)
		{
			if (x == null)
			{
				throw new ArgumentNullException(nameof(x));
			}

			if (y == null)
			{
				throw new ArgumentNullException(nameof(y));
			}

			if (comparer == null)
			{
				throw new ArgumentNullException(nameof(comparer));
			}

			CollectionComparer<T> collectionComparer = new CollectionComparer<T>(options, comparer);
			return collectionComparer.Equals(x, y);
		}

		/// <summary>
		///     Compares two sequences and determines whether they contain the same elements.
		///     Comparison is done using the specified function.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of <paramref name="x" /> and <paramref name="y" />. </typeparam>
		/// <param name="x"> The first of the two sequences whose elements are compared against the elements of <paramref name="y" />. </param>
		/// <param name="y"> The second of the two sequences whose elements are compared against the elements of <paramref name="x" />. </param>
		/// <param name="options"> The options which specify comparison options. </param>
		/// <param name="comparer"> The function used to compare the elements in the sequences to look for matches. </param>
		/// <returns>
		///     true if both sequences are equal or contain the same elements respectively.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This is a O(n*m) operation if order is ignored or a O(n+m) operation if order is not ignored, where n is the number of elements in <paramref name="x" /> and m is the number of elements in <paramref name="y" />.
		///     </para>
		///     <para>
		///         <paramref name="x" /> and <paramref name="y" /> are each enumerated exactly once.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="x" />, <paramref name="y" />, or <paramref name="comparer" /> is null. </exception>
		public static bool SequenceEqual <T> (this IEnumerable<T> x, IEnumerable<T> y, CollectionComparerFlags options, Func<T, T, bool> comparer)
		{
			if (x == null)
			{
				throw new ArgumentNullException(nameof(x));
			}

			if (y == null)
			{
				throw new ArgumentNullException(nameof(y));
			}

			if (comparer == null)
			{
				throw new ArgumentNullException(nameof(comparer));
			}

			CollectionComparer<T> collectionComparer = new CollectionComparer<T>(options, comparer);
			return collectionComparer.Equals(x, y);
		}

		/// <summary>
		///     Omits a specified amount of elements from the beginning of a sequence and gets the remaining elements.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of <paramref name="enumerable" />. </typeparam>
		/// <param name="enumerable"> The sequence which contains the elements. </param>
		/// <param name="count"> The number of elements to omit from the beginning of the sequence. </param>
		/// <returns>
		///     The list which contains the remaining elements of the sequence in the order they were enumerated.
		///     The list is empty if the sequence contains no elements or the specified number of elements to omit is equal or larger than the length of the sequence.
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
		/// <exception cref="ArgumentOutOfRangeException"> <paramref name="count" /> is less than zero. </exception>
		public static List<T> Skip <T> (this IEnumerable<T> enumerable, int count)
		{
			if (enumerable == null)
			{
				throw new ArgumentNullException(nameof(enumerable));
			}

			if (count < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(count));
			}

			List<T> items = new List<T>();
			int currentIndex = 0;
			foreach (T item in enumerable)
			{
				if (currentIndex >= count)
				{
					items.Add(item);
				}
				currentIndex++;
			}
			return items;
		}

		/// <summary>
		///     Omits elements from the beginning of a sequence as long as a specified condition is satisfied and then gets the remaining elements.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of <paramref name="enumerable" />. </typeparam>
		/// <param name="enumerable"> The sequence which contains the elements. </param>
		/// <param name="condition"> The function which tests each element for a condition, providing the element itself. </param>
		/// <returns>
		///     The list which contains the remaining elements of the sequence.
		///     The list is empty if the sequence contains no elements or all elements satisfied the specified condition.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This is a O(n) operation where n is the number of elements in the sequence.
		///     </para>
		///     <para>
		///         <paramref name="enumerable" /> is enumerated exactly once.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="enumerable" /> or <paramref name="condition" /> is null. </exception>
		public static List<T> SkipWhile <T> (this IEnumerable<T> enumerable, Func<T, bool> condition)
		{
			return enumerable.SkipWhile((i, e) => condition(e));
		}

		/// <summary>
		///     Omits elements from the beginning of a sequence as long as a specified condition is satisfied and then gets the remaining elements.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of <paramref name="enumerable" />. </typeparam>
		/// <param name="enumerable"> The sequence which contains the elements. </param>
		/// <param name="condition"> The function which tests each element for a condition, providing the elements index and the element itself. </param>
		/// <returns>
		///     The list which contains the remaining elements of the sequence.
		///     The list is empty if the sequence contains no elements or all elements satisfied the specified condition.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This is a O(n) operation where n is the number of elements in the sequence.
		///     </para>
		///     <para>
		///         <paramref name="enumerable" /> is enumerated exactly once.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="enumerable" /> or <paramref name="condition" /> is null. </exception>
		public static List<T> SkipWhile <T> (this IEnumerable<T> enumerable, Func<int, T, bool> condition)
		{
			if (enumerable == null)
			{
				throw new ArgumentNullException(nameof(enumerable));
			}

			if (condition == null)
			{
				throw new ArgumentNullException(nameof(condition));
			}

			List<T> items = new List<T>();
			bool skipping = true;
			int index = 0;
			foreach (T item in enumerable)
			{
				if (skipping)
				{
					skipping = condition(index, item);
				}
				if (!skipping)
				{
					items.Add(item);
				}
				index++;
			}
			return items;
		}

		/// <summary>
		///     Gets a specified amount of elements from the beginning of a sequence and omits the remaining elements.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of <paramref name="enumerable" />. </typeparam>
		/// <param name="enumerable"> The sequence which contains the elements. </param>
		/// <param name="count"> The number of elements to get from the beginning of the sequence. </param>
		/// <returns>
		///     The list which contains the elements of the sequence at its beginning in the order they were enumerated.
		///     The list is empty if the sequence contains no elements or <paramref name="count" /> is zero.
		///     If the specified number of elements to take is larger than the length of the sequence, the list contains all elements of the sequence but not more.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This is a O(n) operation where n is the number of elements in the sequence.
		///     </para>
		///     <para>
		///         <paramref name="enumerable" /> is enumerated only once and only up to the number of elements specified by <paramref name="count" />.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="enumerable" /> is null. </exception>
		/// <exception cref="ArgumentOutOfRangeException"> <paramref name="count" /> is less than zero. </exception>
		public static List<T> Take <T> (this IEnumerable<T> enumerable, int count)
		{
			if (enumerable == null)
			{
				throw new ArgumentNullException(nameof(enumerable));
			}

			if (count < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(count));
			}

			List<T> items = new List<T>();
			int currentIndex = 0;
			foreach (T item in enumerable)
			{
				if (currentIndex >= count)
				{
					break;
				}
				items.Add(item);
				currentIndex++;
				if (currentIndex >= count)
				{
					break;
				}
			}
			return items;
		}

		/// <summary>
		///     Gets elements from the beginning of a sequence as long as a specified condition is satisfied and then omits the remaining elements.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of <paramref name="enumerable" />. </typeparam>
		/// <param name="enumerable"> The sequence which contains the elements. </param>
		/// <param name="condition"> The function which tests each element for a condition, providing the element itself. </param>
		/// <returns>
		///     The list which contains the elements of the sequence at its beginning.
		///     The list is empty if the sequence contains no elements or the first element did not satisfy the condition.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This is a O(n) operation where n is the number of elements in the sequence.
		///     </para>
		///     <para>
		///         <paramref name="enumerable" /> is enumerated only once and only up to the first element which does not satisfy the condition.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="enumerable" /> or <paramref name="condition" /> is null. </exception>
		public static List<T> TakeWhile <T> (this IEnumerable<T> enumerable, Func<T, bool> condition)
		{
			return enumerable.TakeWhile((i, e) => condition(e));
		}

		/// <summary>
		///     Gets elements from the beginning of a sequence as long as a specified condition is satisfied and then omits the remaining elements.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of <paramref name="enumerable" />. </typeparam>
		/// <param name="enumerable"> The sequence which contains the elements. </param>
		/// <param name="condition"> The function which tests each element for a condition, providing the elements index and the element itself. </param>
		/// <returns>
		///     The list which contains the elements of the sequence at its beginning.
		///     The list is empty if the sequence contains no elements or the first element did not satisfy the condition.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This is a O(n) operation where n is the number of elements in the sequence.
		///     </para>
		///     <para>
		///         <paramref name="enumerable" /> is enumerated only once and only up to the first element which does not satisfy the condition.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="enumerable" /> or <paramref name="condition" /> is null. </exception>
		public static List<T> TakeWhile <T> (this IEnumerable<T> enumerable, Func<int, T, bool> condition)
		{
			if (enumerable == null)
			{
				throw new ArgumentNullException(nameof(enumerable));
			}

			if (condition == null)
			{
				throw new ArgumentNullException(nameof(condition));
			}

			List<T> items = new List<T>();
			int index = 0;
			foreach (T item in enumerable)
			{
				if (condition(index, item))
				{
					items.Add(item);
				}
				else
				{
					break;
				}
				index++;
			}
			return items;
		}

		/// <summary>
		///     Converts a sequence to a new array.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of <paramref name="enumerable" />. </typeparam>
		/// <param name="enumerable"> The sequence which contains the elements. </param>
		/// <returns>
		///     An array which contains all elements of the sequence in the order they were enumerated.
		///     The array has a length of zero if the sequence contains no elements.
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
		public static T[] ToArray <T> (this IEnumerable<T> enumerable)
		{
			if (enumerable == null)
			{
				throw new ArgumentNullException(nameof(enumerable));
			}

			List<T> result = new List<T>(enumerable);
			return result.ToArray();
		}

		/// <summary>
		///     Converts a sequence to a new array, starting at a specified index.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of <paramref name="enumerable" />. </typeparam>
		/// <param name="enumerable"> The sequence which contains the elements. </param>
		/// <param name="index"> The index from which the elements are copied to the array. </param>
		/// <returns>
		///     An array which contains all elements of the sequence, starting at the specified index, in the order they were enumerated.
		///     The array has a length of zero if the sequence contains no elements or the specified index is outside the sequence.
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
		/// <exception cref="ArgumentOutOfRangeException"> <paramref name="index" /> is less than zero. </exception>
		public static T[] ToArray <T> (this IEnumerable<T> enumerable, int index)
		{
			if (enumerable == null)
			{
				throw new ArgumentNullException(nameof(enumerable));
			}

			if (index < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(index));
			}

			return DirectLinqExtensions.ToListInternal(enumerable, index, -1).ToArray();
		}

		/// <summary>
		///     Converts a sequence to a new array, starting at a specified index for a specified number of elements.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of <paramref name="enumerable" />. </typeparam>
		/// <param name="enumerable"> The sequence which contains the elements. </param>
		/// <param name="index"> The index from which the elements are copied to the array. </param>
		/// <param name="count"> The number of elements to copy to the array, starting at the specified index. </param>
		/// <returns>
		///     An array which contains the specified number of elements of the sequence, starting at the specified index, in the order they were enumerated.
		///     The array has a length of zero if the sequence contains no elements, the specified index is outside the sequence, or <paramref name="count" /> is zero.
		///     If the range specified by <paramref name="index" /> and <paramref name="count" /> reaches outside the sequence, the array stops at the last element of the sequence.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This is a O(n) operation where n is the number of elements in the sequence.
		///     </para>
		///     <para>
		///         <paramref name="enumerable" /> is enumerated only once and only up to the last element in the range specified by <paramref name="index" /> and <paramref name="count" />.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="enumerable" /> is null. </exception>
		/// <exception cref="ArgumentOutOfRangeException"> <paramref name="index" /> or <paramref name="count" /> is less than zero. </exception>
		public static T[] ToArray <T> (this IEnumerable<T> enumerable, int index, int count)
		{
			if (enumerable == null)
			{
				throw new ArgumentNullException(nameof(enumerable));
			}

			if (index < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(index));
			}

			if (count < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(count));
			}

			return DirectLinqExtensions.ToListInternal(enumerable, index, count).ToArray();
		}

		/// <summary>
		///     Converts a sequence to a dictionary by deriving a key from each element.
		///     Each key can only be assigned to one element.
		///     Key equality is checked using the default equality comparer for the key type.
		/// </summary>
		/// <typeparam name="TIn"> The type of the elements of <paramref name="enumerable" />. </typeparam>
		/// <typeparam name="TKey"> The type of the derived keys in the dictionary. </typeparam>
		/// <typeparam name="TValue"> The type of the values in the dictionary. </typeparam>
		/// <param name="enumerable"> The sequence which contains the elements. </param>
		/// <param name="mapper"> The function which derives a key and a value for each element in the sequence. </param>
		/// <returns>
		///     The dictionary which contains the key-value-pairs for each element in the sequence.
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
		/// <exception cref="InvalidOperationException"> The same key is derived for more than one element. </exception>
		public static Dictionary<TKey, TValue> ToDictionary <TIn, TKey, TValue> (this IEnumerable<TIn> enumerable, Func<TIn, KeyValuePair<TKey, TValue>> mapper)
		{
			if (enumerable == null)
			{
				throw new ArgumentNullException(nameof(enumerable));
			}

			if (mapper == null)
			{
				throw new ArgumentNullException(nameof(mapper));
			}

			return DirectLinqExtensions.ToDictionaryInternal(enumerable, null, mapper);
		}

		/// <summary>
		///     Converts a sequence to a dictionary by deriving a key from each element.
		///     Each key can only be assigned to one element.
		///     Key equality is checked using the specified equality comparer for the key type.
		/// </summary>
		/// <typeparam name="TIn"> The type of the elements of <paramref name="enumerable" />. </typeparam>
		/// <typeparam name="TKey"> The type of the derived keys in the dictionary. </typeparam>
		/// <typeparam name="TValue"> The type of the values in the dictionary. </typeparam>
		/// <param name="enumerable"> The sequence which contains the elements. </param>
		/// <param name="keyComparer"> The equality comparer for the keys, used by the returned dictionary. </param>
		/// <param name="mapper"> The function which derives a key and a value for each element in the sequence. </param>
		/// <returns>
		///     The dictionary which contains the key-value-pairs for each element in the sequence.
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
		/// <exception cref="InvalidOperationException"> The same key is derived for more than one element. </exception>
		public static Dictionary<TKey, TValue> ToDictionary <TIn, TKey, TValue> (this IEnumerable<TIn> enumerable, IEqualityComparer<TKey> keyComparer, Func<TIn, KeyValuePair<TKey, TValue>> mapper)
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

			return DirectLinqExtensions.ToDictionaryInternal(enumerable, keyComparer, mapper);
		}

		/// <summary>
		///     Converts a sequence to a new list.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of <paramref name="enumerable" />. </typeparam>
		/// <param name="enumerable"> The sequence which contains the elements. </param>
		/// <returns>
		///     A list which contains all elements of the sequence in the order they were enumerated.
		///     The list has a length of zero if the sequence contains no elements.
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
		public static List<T> ToList <T> (this IEnumerable<T> enumerable)
		{
			if (enumerable == null)
			{
				throw new ArgumentNullException(nameof(enumerable));
			}

			List<T> result = new List<T>(enumerable);
			return result;
		}

		/// <summary>
		///     Converts a non-generic sequence to a new list.
		/// </summary>
		/// <param name="enumerable"> The sequence which contains the elements. </param>
		/// <returns>
		///     A list which contains all elements of the sequence in the order they were enumerated.
		///     The list has a length of zero if the sequence contains no elements.
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
		public static List<object> ToList (this IEnumerable enumerable)
		{
			if (enumerable == null)
			{
				throw new ArgumentNullException(nameof(enumerable));
			}

			List<object> result = new List<object>();
			foreach (object item in enumerable)
			{
				result.Add(item);
			}
			return result;
		}

		/// <summary>
		///     Converts a sequence to a new list, starting at a specified index.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of <paramref name="enumerable" />. </typeparam>
		/// <param name="enumerable"> The sequence which contains the elements. </param>
		/// <param name="index"> The index from which the elements are copied to the list. </param>
		/// <returns>
		///     A list which contains all elements of the sequence, starting at the specified index, in the order they were enumerated.
		///     The list has a length of zero if the sequence contains no elements or the specified index is outside the sequence.
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
		/// <exception cref="ArgumentOutOfRangeException"> <paramref name="index" /> is less than zero. </exception>
		public static List<T> ToList <T> (this IEnumerable<T> enumerable, int index)
		{
			if (enumerable == null)
			{
				throw new ArgumentNullException(nameof(enumerable));
			}

			if (index < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(index));
			}

			return DirectLinqExtensions.ToListInternal(enumerable, index, -1);
		}

		/// <summary>
		///     Converts a sequence to a new list, starting at a specified index for a specified number of elements.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of <paramref name="enumerable" />. </typeparam>
		/// <param name="enumerable"> The sequence which contains the elements. </param>
		/// <param name="index"> The index from which the elements are copied to the list. </param>
		/// <param name="count"> The number of elements to copy to the list, starting at the specified index. </param>
		/// <returns>
		///     A list which contains the specified number of elements of the sequence in the order they were enumerated.
		///     The list has a length of zero if the sequence contains no elements, the specified index is outside the sequence, or <paramref name="count" /> is zero.
		///     If the range specified by <paramref name="index" /> and <paramref name="count" /> reaches outside the sequence, the list stops at the last element of the sequence.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This is a O(n) operation where n is the number of elements in the sequence.
		///     </para>
		///     <para>
		///         <paramref name="enumerable" /> is enumerated only once and only up to the last element in the range specified by <paramref name="index" /> and <paramref name="count" />.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="enumerable" /> is null. </exception>
		/// <exception cref="ArgumentOutOfRangeException"> <paramref name="index" /> or <paramref name="count" /> is less than zero. </exception>
		public static List<T> ToList <T> (this IEnumerable<T> enumerable, int index, int count)
		{
			if (enumerable == null)
			{
				throw new ArgumentNullException(nameof(enumerable));
			}

			if (index < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(index));
			}

			if (count < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(count));
			}

			return DirectLinqExtensions.ToListInternal(enumerable, index, count);
		}

		/// <summary>
		///     Converts two sequences to a set which contains all the elements of both sequences.
		///     Comparison is done using the default equality comparison.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of <paramref name="first" /> and <paramref name="second" />. </typeparam>
		/// <param name="first"> The first sequence whose elements are added to the resulting set. </param>
		/// <param name="second"> The second sequence whose elements are added to the resulting set. </param>
		/// <returns>
		///     The set which contains all elements from both <paramref name="first" /> and <paramref name="second" />.
		///     The set is empty if both sequences are empty.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This is a O((n+m)^2) operation where n is the number of elements in <paramref name="first" /> and m is the number of elements in <paramref name="second" />.
		///     </para>
		///     <para>
		///         <paramref name="first" /> and <paramref name="second" /> are each enumerated exactly once.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="first" /> or <paramref name="second" /> is null. </exception>
		public static HashSet<T> Union <T> (this IEnumerable<T> first, IEnumerable<T> second)
		{
			return first.Union(second, EqualityComparer<T>.Default.Equals);
		}

		/// <summary>
		///     Converts two sequences to a set which contains all the elements of both sequences.
		///     Comparison is done using the specified equality comparer.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of <paramref name="first" /> and <paramref name="second" />. </typeparam>
		/// <param name="first"> The first sequence whose elements are added to the resulting set. </param>
		/// <param name="second"> The second sequence whose elements are added to the resulting set. </param>
		/// <param name="comparer"> The equality comparer used to compare the elements in the sequences to look for matches. </param>
		/// <returns>
		///     The set which contains all elements from both <paramref name="first" /> and <paramref name="second" />.
		///     The set is empty if both sequences are empty.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This is a O((n+m)^2) operation where n is the number of elements in <paramref name="first" /> and m is the number of elements in <paramref name="second" />.
		///     </para>
		///     <para>
		///         <paramref name="first" /> and <paramref name="second" /> are each enumerated exactly once.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="first" />, <paramref name="second" />, or <paramref name="comparer" /> is null. </exception>
		public static HashSet<T> Union <T> (this IEnumerable<T> first, IEnumerable<T> second, IEqualityComparer<T> comparer)
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

			return first.Union(second, comparer.Equals);
		}

		/// <summary>
		///     Converts two sequences to a set which contains all the elements of both sequences.
		///     Comparison is done using the specified function.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of <paramref name="first" /> and <paramref name="second" />. </typeparam>
		/// <param name="first"> The first sequence whose elements are added to the resulting set. </param>
		/// <param name="second"> The second sequence whose elements are added to the resulting set. </param>
		/// <param name="comparer"> The equality comparer used to compare the elements in the sequences to look for matches. </param>
		/// <returns>
		///     The set which contains all elements from both <paramref name="first" /> and <paramref name="second" />.
		///     The set is empty if both sequences are empty.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This is a O((n+m)^2) operation where n is the number of elements in <paramref name="first" /> and m is the number of elements in <paramref name="second" />.
		///     </para>
		///     <para>
		///         <paramref name="first" /> and <paramref name="second" /> are each enumerated exactly once.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="first" />, <paramref name="second" />, or <paramref name="comparer" /> is null. </exception>
		public static HashSet<T> Union <T> (this IEnumerable<T> first, IEnumerable<T> second, Func<T, T, bool> comparer)
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
			result.UnionWith(second);
			return result;
		}

		/// <summary>
		///     Filters all elements of a sequence into a new list.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of <paramref name="enumerable" />. </typeparam>
		/// <param name="enumerable"> The sequence which contains the elements. </param>
		/// <param name="filter"> The function which filters each element, providing the element itself. </param>
		/// <returns>
		///     The list which contains all elements which passed the filter.
		///     The list is empty if the sequence contains no elements or no element passed the filter.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This is a O(n) operation where n is the number of elements in the sequence.
		///     </para>
		///     <para>
		///         <paramref name="enumerable" /> is enumerated exactly once.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="enumerable" /> or <paramref name="filter" /> is null. </exception>
		public static List<T> Where <T> (this IEnumerable<T> enumerable, Func<T, bool> filter)
		{
			return enumerable.Where((i, e) => filter(e));
		}

		/// <summary>
		///     Filters all elements of a sequence into a new list.
		/// </summary>
		/// <typeparam name="T"> The type of the elements of <paramref name="enumerable" />. </typeparam>
		/// <param name="enumerable"> The sequence which contains the elements. </param>
		/// <param name="filter"> The function which filters each element, providing the elements index and the element itself. </param>
		/// <returns>
		///     The list which contains all elements which passed the filter.
		///     The list is empty if the sequence contains no elements or no element passed the filter.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This is a O(n) operation where n is the number of elements in the sequence.
		///     </para>
		///     <para>
		///         <paramref name="enumerable" /> is enumerated exactly once.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="enumerable" /> or <paramref name="filter" /> is null. </exception>
		public static List<T> Where <T> (this IEnumerable<T> enumerable, Func<int, T, bool> filter)
		{
			if (enumerable == null)
			{
				throw new ArgumentNullException(nameof(enumerable));
			}

			if (filter == null)
			{
				throw new ArgumentNullException(nameof(filter));
			}

			List<T> results = new List<T>();
			int index = 0;
			foreach (T item in enumerable)
			{
				if (filter(index, item))
				{
					results.Add(item);
				}
				index++;
			}
			return results;
		}

		/// <summary>
		///     Zips together two sequences into a list by combining/converting two elements with the same index of each sequence into a resulting element.
		/// </summary>
		/// <typeparam name="TFirst"> The type of the elements of <paramref name="first" />. </typeparam>
		/// <typeparam name="TSecond"> The type of the elements of <paramref name="second" />. </typeparam>
		/// <typeparam name="TResult"> The type of the resulting elements in the list. </typeparam>
		/// <param name="first"> The first sequence whose elements are zipped with the corresponding elements of <paramref name="second" />. </param>
		/// <param name="second"> The second sequence whose elements are zipped with the corresponding elements of <paramref name="first" />. </param>
		/// <param name="zipper"> The function which combines, and optionally converts, two elements of the two sequences with the same index, providing the element itself. </param>
		/// <returns>
		///     The list which contains the resulting elements.
		///     The list is empty if either <paramref name="first" /> or <paramref name="second" /> contains no elements.
		///     The zipping stops when the end of either sequence is reached, adding no further elements to the list.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This is a O(n) operation where n is the number of elements in the sequence with lesser elements.
		///     </para>
		///     <para>
		///         <paramref name="first" /> and <paramref name="second" /> are each enumerated exactly once.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="first" />, <paramref name="second" />, or <paramref name="zipper" /> is null. </exception>
		public static List<TResult> Zip <TFirst, TSecond, TResult> (this IEnumerable<TFirst> first, IEnumerable<TSecond> second, Func<TFirst, TSecond, TResult> zipper)
		{
			return first.Zip(second, (i, a, b) => zipper(a, b));
		}

		/// <summary>
		///     Zips together two sequences into a list by combining/converting two elements with the same index of each sequence into a resulting element.
		/// </summary>
		/// <typeparam name="TFirst"> The type of the elements of <paramref name="first" />. </typeparam>
		/// <typeparam name="TSecond"> The type of the elements of <paramref name="second" />. </typeparam>
		/// <typeparam name="TResult"> The type of the resulting elements in the list. </typeparam>
		/// <param name="first"> The first sequence whose elements are zipped with the corresponding elements of <paramref name="second" />. </param>
		/// <param name="second"> The second sequence whose elements are zipped with the corresponding elements of <paramref name="first" />. </param>
		/// <param name="zipper"> The function which combines, and optionally converts, two elements of the two sequences with the same index, providing the elements index and the element itself. </param>
		/// <returns>
		///     The list which contains the resulting elements.
		///     The list is empty if either <paramref name="first" /> or <paramref name="second" /> contains no elements.
		///     The zipping stops when the end of either sequence is reached, adding no further elements to the list.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This is a O(n) operation where n is the number of elements in the sequence with lesser elements.
		///     </para>
		///     <para>
		///         <paramref name="first" /> and <paramref name="second" /> are each enumerated exactly once.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="first" />, <paramref name="second" />, or <paramref name="zipper" /> is null. </exception>
		public static List<TResult> Zip <TFirst, TSecond, TResult> (this IEnumerable<TFirst> first, IEnumerable<TSecond> second, Func<int, TFirst, TSecond, TResult> zipper)
		{
			if (first == null)
			{
				throw new ArgumentNullException(nameof(first));
			}

			if (second == null)
			{
				throw new ArgumentNullException(nameof(second));
			}

			if (zipper == null)
			{
				throw new ArgumentNullException(nameof(zipper));
			}

			List<TFirst> firstValues = new List<TFirst>(first);
			List<TSecond> secondValues = new List<TSecond>(second);
			List<TResult> results = new List<TResult>();
			int count = Math.Min(firstValues.Count, secondValues.Count);
			for (int i1 = 0; i1 < count; i1++)
			{
				results.Add(zipper(i1, firstValues[i1], secondValues[i1]));
			}
			return results;
		}

		private static Dictionary<TKey, TValue> ToDictionaryInternal <TIn, TKey, TValue> (IEnumerable<TIn> enumerable, IEqualityComparer<TKey> keyComparer, Func<TIn, KeyValuePair<TKey, TValue>> mapper)
		{
			Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>(keyComparer ?? EqualityComparer<TKey>.Default);
			foreach (TIn item in enumerable)
			{
				KeyValuePair<TKey, TValue> pair = mapper(item);
				try
				{
					dictionary.Add(pair.Key, pair.Value);
				}
				catch (ArgumentException)
				{
					throw new InvalidOperationException("The same key is derived for more than one element.");
				}
			}
			return dictionary;
		}

		private static List<T> ToListInternal <T> (IEnumerable<T> enumerable, int index, int count)
		{
			List<T> items = new List<T>();
			int currentIndex = 0;
			foreach (T item in enumerable)
			{
				if (currentIndex >= index)
				{
					if ((currentIndex >= (index + count)) && (count != -1))
					{
						break;
					}
					items.Add(item);
				}
				currentIndex++;
			}
			return items;
		}

		#endregion
	}
}
