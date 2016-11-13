using System.Data.SQLite;
using System.Globalization;




namespace RI.Framework.Data.SQLite.Collations
{
	/// <summary>
	///     Implements an SQLite collation which performs trimmed, case-insensitive string comparison using the current culture.
	/// </summary>
	/// <remarks>
	///     <para>
	///         The string comparison done by this collation first trimms both strings to compare (means: removing all leading and trailing whitespaces) and then performs a case-insensitive comparison using <see cref="CultureInfo.CurrentCulture" />.
	///     </para>
	///     <para>
	///         The SQL name of the collation is <c> TRIMCICURRENT </c>.
	///     </para>
	/// </remarks>
	[SQLiteFunction (FuncType = FunctionType.Collation, Name = "TRIMCICURRENT")]
	public sealed class TrimmedCaseInsensitiveCurrentSQLiteCollation : SQLiteFunction
	{
		#region Static Methods

		/// <summary>
		///     Registers the collation.
		/// </summary>
		public static void RegisterGlobal ()
		{
			SQLiteFunction.RegisterFunction(typeof(TrimmedCaseInsensitiveCurrentSQLiteCollation));
		}

		#endregion




		#region Overrides

		/// <inheritdoc />
		public override int Compare (string param1, string param2)
		{
			return string.Compare(param1?.Trim(), param2?.Trim(), CultureInfo.CurrentCulture, CompareOptions.IgnoreCase);
		}

		#endregion
	}
}
