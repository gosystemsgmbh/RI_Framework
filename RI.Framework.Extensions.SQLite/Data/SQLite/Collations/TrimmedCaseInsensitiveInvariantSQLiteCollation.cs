using System.Data.SQLite;
using System.Globalization;




namespace RI.Framework.Data.SQLite.Collations
{
	/// <summary>
	///     Implements an SQLite collation which performs trimmed, case-insensitive string comparison.
	/// </summary>
	/// <remarks>
	///     <para>
	///         The string comparison done by this collation first trimms both strings to compare (means: removing all leading and trailing whitespaces) and then performs a case-insensitive comparison using <see cref="CultureInfo.InvariantCulture" />.
	///     </para>
	///     <para>
	///         The SQL name of the collation is <c> TRIMCIINVARIANT </c>.
	///     </para>
	/// </remarks>
	[SQLiteFunction (FuncType = FunctionType.Collation, Name = "TRIMCIINVARIANT")]
	public sealed class TrimmedCaseInsensitiveInvariantSQLiteCollation : SQLiteFunction
	{
		#region Static Methods

		/// <summary>
		///     Registers the collation.
		/// </summary>
		public static void Register ()
		{
			SQLiteFunction.RegisterFunction(typeof(TrimmedCaseInsensitiveInvariantSQLiteCollation));
		}

		#endregion




		#region Overrides

		/// <inheritdoc />
		public override int Compare (string param1, string param2)
		{
			return string.Compare(param1.Trim(), param2.Trim(), CultureInfo.InvariantCulture, CompareOptions.IgnoreCase);
		}

		#endregion
	}
}
