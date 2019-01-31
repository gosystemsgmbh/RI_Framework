using System;
using System.Collections.Generic;

using RI.Framework.Utilities.Comparison;




namespace RI.Framework.Utilities
{
    /// <summary>
    ///     Implements several often used string comparison operations and provides a base class for customized string comparison.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Half of the static properties which provide predefined <see cref="StringComparerEx" /> instances are just there to wrap <see cref="StringComparer" /> while the other ones are equivalents which provide trimmed comparison.
    ///         Trimmed comparison means that the strings are trimmed before they are compared, using <see cref="string.Trim()" />.
    ///         Trimmed comparison might be usefull to compare data coming from the user or a data source.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    public class StringComparerEx : StringComparer
	{
		#region Static Properties/Indexer

		/// <summary>
		///     Gets the string comparer used for case-sensitive comparison using the current culture.
		/// </summary>
		/// <value>
		///     The string comparer.
		/// </value>
		public new static StringComparerEx CurrentCulture => new StringComparerEx(StringComparer.CurrentCulture, StringComparer.CurrentCulture);

		/// <summary>
		///     Gets the string comparer used for case-insensitive comparison using the current culture.
		/// </summary>
		/// <value>
		///     The string comparer.
		/// </value>
		public new static StringComparerEx CurrentCultureIgnoreCase => new StringComparerEx(StringComparer.CurrentCultureIgnoreCase, StringComparer.CurrentCultureIgnoreCase);

		/// <summary>
		///     Gets the string comparer used for case-sensitive comparison using the invariant culture.
		/// </summary>
		/// <value>
		///     The string comparer.
		/// </value>
		public new static StringComparerEx InvariantCulture { get; } = new StringComparerEx(StringComparer.InvariantCulture, StringComparer.InvariantCulture);

		/// <summary>
		///     Gets the string comparer used for case-insensitive comparison using the invariant culture.
		/// </summary>
		/// <value>
		///     The string comparer.
		/// </value>
		public new static StringComparerEx InvariantCultureIgnoreCase { get; } = new StringComparerEx(StringComparer.InvariantCultureIgnoreCase, StringComparer.InvariantCultureIgnoreCase);

		/// <summary>
		///     Gets the string comparer used for case-sensitive ordinal comparison.
		/// </summary>
		/// <value>
		///     The string comparer.
		/// </value>
		public new static StringComparerEx Ordinal { get; } = new StringComparerEx(StringComparer.Ordinal, StringComparer.Ordinal);

		/// <summary>
		///     Gets the string comparer used for case-insensitive ordinal comparison.
		/// </summary>
		/// <value>
		///     The string comparer.
		/// </value>
		public new static StringComparerEx OrdinalIgnoreCase { get; } = new StringComparerEx(StringComparer.OrdinalIgnoreCase, StringComparer.OrdinalIgnoreCase);

		/// <summary>
		///     Gets the string comparer used for case-sensitive, trimmed comparison using the current culture.
		/// </summary>
		/// <value>
		///     The string comparer.
		/// </value>
		public static StringComparerEx TrimmedCurrentCulture => new StringComparerEx(new EqualityComparison<string>((x, y) => StringComparerEx.CurrentCulture.Equals(x?.Trim(), y?.Trim())), new OrderComparison<string>((x, y) => StringComparerEx.CurrentCulture.Compare(x?.Trim(), y?.Trim())));

		/// <summary>
		///     Gets the string comparer used for case-insensitive, trimmed comparison using the current culture.
		/// </summary>
		/// <value>
		///     The string comparer.
		/// </value>
		public static StringComparerEx TrimmedCurrentCultureIgnoreCase => new StringComparerEx(new EqualityComparison<string>((x, y) => StringComparerEx.CurrentCultureIgnoreCase.Equals(x?.Trim(), y?.Trim())), new OrderComparison<string>((x, y) => StringComparerEx.CurrentCultureIgnoreCase.Compare(x?.Trim(), y?.Trim())));

		/// <summary>
		///     Gets the string comparer used for case-sensitive, trimmed comparison using the invariant culture.
		/// </summary>
		/// <value>
		///     The string comparer.
		/// </value>
		public static StringComparerEx TrimmedInvariantCulture { get; } = new StringComparerEx(new EqualityComparison<string>((x, y) => StringComparerEx.InvariantCulture.Equals(x?.Trim(), y?.Trim())), new OrderComparison<string>((x, y) => StringComparerEx.InvariantCulture.Compare(x?.Trim(), y?.Trim())));

		/// <summary>
		///     Gets the string comparer used for case-insensitive, trimmed comparison using the invariant culture.
		/// </summary>
		/// <value>
		///     The string comparer.
		/// </value>
		public static StringComparerEx TrimmedInvariantCultureIgnoreCase { get; } = new StringComparerEx(new EqualityComparison<string>((x, y) => StringComparerEx.InvariantCultureIgnoreCase.Equals(x?.Trim(), y?.Trim())), new OrderComparison<string>((x, y) => StringComparerEx.InvariantCultureIgnoreCase.Compare(x?.Trim(), y?.Trim())));

		/// <summary>
		///     Gets the string comparer used for case-sensitive, trimmed ordinal comparison.
		/// </summary>
		/// <value>
		///     The string comparer.
		/// </value>
		public static StringComparerEx TrimmedOrdinal { get; } = new StringComparerEx(new EqualityComparison<string>((x, y) => StringComparerEx.Ordinal.Equals(x?.Trim(), y?.Trim())), new OrderComparison<string>((x, y) => StringComparerEx.Ordinal.Compare(x?.Trim(), y?.Trim())));

		/// <summary>
		///     Gets the string comparer used for case-insensitive, trimmed ordinal comparison.
		/// </summary>
		/// <value>
		///     The string comparer.
		/// </value>
		public static StringComparerEx TrimmedOrdinalIgnoreCase { get; } = new StringComparerEx(new EqualityComparison<string>((x, y) => StringComparerEx.OrdinalIgnoreCase.Equals(x?.Trim(), y?.Trim())), new OrderComparison<string>((x, y) => StringComparerEx.OrdinalIgnoreCase.Compare(x?.Trim(), y?.Trim())));

		#endregion




		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="StringComparerEx" />.
		/// </summary>
		/// <param name="equalityComparer"> The equality comparer used to compare two strings. </param>
		/// <param name="orderComparer"> The order comparer used to compare two strings. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="equalityComparer" /> or <paramref name="orderComparer" /> is null. </exception>
		public StringComparerEx (IEqualityComparer<string> equalityComparer, IComparer<string> orderComparer)
		{
			if (equalityComparer == null)
			{
				throw new ArgumentNullException(nameof(equalityComparer));
			}

			if (orderComparer == null)
			{
				throw new ArgumentNullException(nameof(orderComparer));
			}

			this.EqualityComparer = equalityComparer;
			this.OrderComparer = orderComparer;
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets the equality comparer used to compare two strings.
		/// </summary>
		/// <value>
		///     The equality comparer used to compare two strings.
		/// </value>
		public IEqualityComparer<string> EqualityComparer { get; }

		/// <summary>
		///     Gets the order comparer used to compare two strings.
		/// </summary>
		/// <value>
		///     The order comparer used to compare two strings.
		/// </value>
		public IComparer<string> OrderComparer { get; }

		#endregion




		#region Overrides

		/// <inheritdoc />
		public override int Compare (string x, string y)
		{
			return this.OrderComparer.Compare(x, y);
		}

		/// <inheritdoc />
		public override bool Equals (string x, string y)
		{
			return this.EqualityComparer.Equals(x, y);
		}

		/// <inheritdoc />
		public override bool Equals (object obj)
		{
			StringComparerEx other = obj as StringComparerEx;
			if (other == null)
			{
				return false;
			}

			return this.EqualityComparer.Equals(other.EqualityComparer) && this.OrderComparer.Equals(other.OrderComparer);
		}

		/// <inheritdoc />
		public override int GetHashCode (string obj)
		{
			return this.EqualityComparer.GetHashCode(obj);
		}

		/// <inheritdoc />
		public override int GetHashCode ()
		{
			long hashCode = 0;
			hashCode += this.EqualityComparer.GetHashCode();
			hashCode += this.OrderComparer.GetHashCode();
			return (int)(hashCode / 2);
		}

		#endregion
	}
}
