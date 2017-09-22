using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using RI.Framework.Utilities.Exceptions;

namespace RI.Framework.Utilities.Serialization
{
	/// <summary>
	///     Provides utility/extension methods for the <see cref="SerializationInfo" /> type.
	/// </summary>
	public static class SerializationInfoExtensions
	{
		/// <summary>
		/// Checks whether a value is available.
		/// </summary>
		/// <param name="info"> The serialization data. </param>
		/// <param name="name"> The name of the value. </param>
		/// <returns>
		/// true if the value exists, false otherwise.
		/// </returns>
		/// <remarks>
		/// <note type= "note">
		/// Comparison is done case-insensitive.
		/// </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"><paramref name="info"/> or <paramref name="name"/> is null.</exception>
		/// <exception cref="EmptyStringArgumentException"><paramref name="name"/> is an empty string.</exception>
		public static bool HasValue (this SerializationInfo info, string name)
		{
			if (info == null)
			{
				throw new ArgumentNullException(nameof(info));
			}

			if (name == null)
			{
				throw new ArgumentNullException(nameof(name));
			}

			if (name.IsNullOrEmptyOrWhitespace())
			{
				throw new EmptyStringArgumentException(nameof(name));
			}

			SerializationInfoEnumerator values = info.GetEnumerator();
			while (values.MoveNext())
			{
				if (string.Equals(values.Current.Name, name, StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Checks whether multiple values are available.
		/// </summary>
		/// <param name="info"> The serialization data. </param>
		/// <param name="names"> The names of the values. </param>
		/// <returns>
		/// true if all values exists, false otherwise.
		/// </returns>
		/// <remarks>
		/// <note type= "note">
		/// Comparison is done case-insensitive.
		/// </note>
		/// <para>
		/// <paramref name="names"/> is enumerated only once.
		/// </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"><paramref name="info"/> or <paramref name="names"/> is null.</exception>
		/// <exception cref="EmptyStringArgumentException"><paramref name="names"/> contains an empty string.</exception>
		public static bool HasValues (this SerializationInfo info, IEnumerable<string> names)
		{
			if (info == null)
			{
				throw new ArgumentNullException(nameof(info));
			}

			if (names == null)
			{
				throw new ArgumentNullException(nameof(names));
			}

			HashSet<string> nameList = new HashSet<string>(names, StringComparerEx.OrdinalIgnoreCase);

			if (nameList.Count == 0)
			{
				return true;
			}

			foreach (string name in nameList)
			{
				if (name.IsNullOrEmptyOrWhitespace())
				{
					throw new EmptyStringArgumentException(nameof(names));
				}
			}

			SerializationInfoEnumerator values = info.GetEnumerator();
			while (values.MoveNext())
			{
				nameList.Remove(values.Name);
			}

			return nameList.Count == 0;
		}

		/// <summary>
		/// Checks whether multiple values are available.
		/// </summary>
		/// <param name="info"> The serialization data. </param>
		/// <param name="names"> The names of the values. </param>
		/// <returns>
		/// true if all values exists, false otherwise.
		/// </returns>
		/// <remarks>
		/// <note type= "note">
		/// Comparison is done case-insensitive.
		/// </note>
		/// <para>
		/// <paramref name="names"/> is enumerated only once.
		/// </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"><paramref name="info"/> or <paramref name="names"/> is null.</exception>
		/// <exception cref="EmptyStringArgumentException"><paramref name="names"/> contains an empty string.</exception>
		public static bool HasValues(this SerializationInfo info, params string[] names) => info.HasValues((IEnumerable<string>)names);
	}
}
