using System;
using System.Data.SQLite;
using System.Globalization;




namespace RI.Framework.Data.SQLite.Collations
{
	/// <summary>
	///     Implements an SQLite collation which performs trimmed, case-insensitive string comparison using a specified culture.
	/// </summary>
	/// <remarks>
	///     <para>
	///         The string comparison done by this collation first trimms both strings to compare (means: removing all leading and trailing whitespaces) and then performs a case-insensitive comparison using the specified <see cref="CultureInfo" />.
	///     </para>
	///     <para>
	///         The SQL name of the collation is <c> TRIMCICULTURE </c>.
	///     </para>
	/// </remarks>
	[SQLiteFunction (FuncType = FunctionType.Collation, Name = "TRIMCICULTURE")]
	public sealed class TrimmedCaseInsensitiveCultureSQLiteCollation : SQLiteFunction
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="TrimmedCaseInsensitiveCultureSQLiteCollation" />.
		/// </summary>
		/// <param name="culture"> The culture used for the string comparison. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="culture" /> is null. </exception>
		public TrimmedCaseInsensitiveCultureSQLiteCollation (CultureInfo culture)
		{
			if (culture == null)
			{
				throw new ArgumentNullException(nameof(culture));
			}

			this.Culture = culture;
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets the culture used for the string comparison.
		/// </summary>
		/// <value>
		///     The culture used for the string comparison.
		/// </value>
		public CultureInfo Culture { get; private set; }

		#endregion




		#region Overrides

		/// <inheritdoc />
		public override int Compare (string param1, string param2)
		{
			return string.Compare(param1?.Trim(), param2?.Trim(), this.Culture, CompareOptions.IgnoreCase);
		}

		#endregion
	}
}
