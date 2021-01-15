using System;
using System.Collections.Generic;



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
    /// <threadsafety static="false" instance="false" />
    /// TODO: ToListList
    /// TODO: ToListSet
    public static class IEnumerableExtensions
    {
        #region Static Methods

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
        public static IEnumerable<T> AsEnumerable<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException(nameof(enumerable));
            }

            return enumerable;
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
