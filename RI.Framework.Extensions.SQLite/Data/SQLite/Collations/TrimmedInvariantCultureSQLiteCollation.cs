using System.Data.SQLite;

using RI.Framework.Utilities;




namespace RI.Framework.Data.SQLite.Collations
{
	/// <summary>
	///     Implements an SQLite collation which performs trimmed, case-sensitive string comparison using the invariant culture.
	/// </summary>
	/// <remarks>
	///     <para>
	///         This collation uses <see cref="StringComparerEx.TrimmedInvariantCulture" />
	///     </para>
	///     <para>
	///         The SQL name of the collation is <c> TrimmedInvariantCulture </c>.
	///     </para>
	/// </remarks>
	[SQLiteFunction (FuncType = FunctionType.Collation, Name = "TrimmedInvariantCulture")]
	public sealed class TrimmedInvariantCultureSQLiteCollation : SQLiteFunction
	{
		#region Static Methods

		/// <summary>
		///     Registers the collation.
		/// </summary>
		public static void RegisterGlobal ()
		{
			SQLiteFunction.RegisterFunction(typeof(TrimmedInvariantCultureSQLiteCollation));
		}

		#endregion




		#region Overrides

		/// <inheritdoc />
		public override int Compare (string param1, string param2)
		{
			return StringComparerEx.TrimmedInvariantCulture.Compare(param1, param2);
		}

		#endregion
	}
}
