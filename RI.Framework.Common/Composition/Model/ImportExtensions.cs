using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using RI.Framework.Collections;




namespace RI.Framework.Composition.Model
{
	/// <summary>
	///     Defines extension methods used to access the actual imported values when using <see cref="Import" /> and <see cref="ImportPropertyAttribute" />.
	/// </summary>
	/// <remarks>
	///     <para>
	///         The extension methods defined in this class can be used even if the corresponding <see cref="Import" /> property is null (e.g. before the imports were resolved for the first time).
	///     </para>
	///     <para>
	///         For high-performance access to imported values, it is recommended to retrieve them into an array (using the <see cref="ToArray{T}" /> method) by utilizing the <see cref="IImporting" /> interface to get informed after imports have been resolved or updated.
	///     </para>
	/// </remarks>
	[SuppressMessage ("ReSharper", "UnusedMember.Global")]
	[SuppressMessage ("ReSharper", "MemberCanBePrivate.Global")]
	public static class ImportExtensions
	{
		#region Static Methods

		/// <summary>
		///     Gets all imported values as an array.
		/// </summary>
		/// <typeparam name="T"> The type of the values. </typeparam>
		/// <param name="import"> The import proxy. </param>
		/// <returns>
		///     The array which contains all imported values of type <typeparamref name="T" />.
		///     The array is empty if no values were imported or no value is of type <typeparamref name="T" />.
		/// </returns>
		public static T[] ToArray <T> (this Import import) where T : class
		{
			return import.ToList<T>().ToArray<T>();
		}

		/// <summary>
		///     Gets all imported values as a list.
		/// </summary>
		/// <typeparam name="T"> The type of the values. </typeparam>
		/// <param name="import"> The import proxy. </param>
		/// <returns>
		///     The list which contains all imported values of type <typeparamref name="T" />.
		///     The list is empty if no values were imported or no value is of type <typeparamref name="T" />.
		/// </returns>
		public static List<T> ToList <T> (this Import import) where T : class
		{
			if (import == null)
			{
				return new List<T>();
			}

			object[] instances = import.Instances;

			if (instances == null)
			{
				return new List<T>();
			}

			List<T> list = new List<T>(instances.Length);
			for (int i1 = 0; i1 < instances.Length; i1++)
			{
				object instance = instances[i1];
				T value = instance as T;
				if (value != null)
				{
					list.Add(value);
				}
			}
			return list;
		}

		/// <summary>
		///     Gets the first imported value or null if no value has been imported.
		/// </summary>
		/// <typeparam name="T"> The type of the value. </typeparam>
		/// <param name="import"> The import proxy. </param>
		/// <returns>
		///     The first imported value of type <typeparamref name="T" /> or null if no value has been imported or no value is of type <typeparamref name="T" />.
		/// </returns>
		/// <remarks>
		///     <para>
		///         If multiple values of type <typeparamref name="T" /> were imported, one of them is returned but it is undefined which one.
		///     </para>
		/// </remarks>
		public static T Value <T> (this Import import) where T : class
		{
			object[] instances = import?.Instances;

			if (instances == null)
			{
				return null;
			}

			for (int i1 = 0; i1 < instances.Length; i1++)
			{
				object instance = instances[i1];
				T value = instance as T;
				if (value != null)
				{
					return value;
				}
			}

			return null;
		}

		/// <summary>
		///     Gets the sequence of all imported values.
		/// </summary>
		/// <typeparam name="T"> The type of the values. </typeparam>
		/// <param name="import"> The import proxy. </param>
		/// <returns>
		///     The sequence which contains all imported values of type <typeparamref name="T" />.
		///     The sequence contains no elements if no values were imported or no value is of type <typeparamref name="T" />.
		/// </returns>
		public static IEnumerable<T> Values <T> (this Import import) where T : class
		{
			object[] instances = import?.Instances;

			if (instances == null)
			{
				yield break;
			}

			for (int i1 = 0; i1 < instances.Length; i1++)
			{
				object instance = instances[i1];
				T value = instance as T;
				if (value != null)
				{
					yield return value;
				}
			}
		}

		#endregion
	}
}
