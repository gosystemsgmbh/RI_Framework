using System.Data.SQLite;
using System.Globalization;




namespace RI.Framework.Data.SQLite
{
	/// <summary>
	///     Implements an SQLite collation which performs trimmed, case-insensitive string comparison.
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
	public sealed class TrimmedCaseInsensitiveCultureCollation : SQLiteFunction
	{
		#region Static Properties/Indexer

		/// <summary>
		///     Gets the culture used for the string comparison when <see cref="TrimmedCaseInsensitiveCultureCollation" /> is used.
		/// </summary>
		/// <value>
		///     The culture used for the string comparison when <see cref="TrimmedCaseInsensitiveCultureCollation" /> is used.
		/// </value>
		public static CultureInfo Culture { get; private set; }

		#endregion




		#region Static Methods

		/// <summary>
		///     Registers the collation with the specified <see cref="CultureInfo" />.
		/// </summary>
		/// <param name="culture"> The <see cref="CultureInfo" /> used for the string comparison by this collation or null if <see cref="CultureInfo.InvariantCulture" /> is to be used. </param>
		public static void Register (CultureInfo culture)
		{
			TrimmedCaseInsensitiveCultureCollation.Culture = culture ?? CultureInfo.InvariantCulture;
			SQLiteFunction.RegisterFunction(typeof(TrimmedCaseInsensitiveCultureCollation));
		}

		#endregion




		#region Overrides

		/// <inheritdoc />
		public override int Compare (string param1, string param2)
		{
			return string.Compare(param1.Trim(), param2.Trim(), TrimmedCaseInsensitiveCultureCollation.Culture, CompareOptions.IgnoreCase);
		}

		#endregion
	}
}
