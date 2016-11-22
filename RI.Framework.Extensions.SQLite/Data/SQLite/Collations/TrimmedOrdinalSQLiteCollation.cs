using System.Data.SQLite;

using RI.Framework.Utilities;




namespace RI.Framework.Data.SQLite.Collations
{
	/// <summary>
	///     Implements an SQLite collation which performs trimmed, case-insensitive ordinal string comparison.
	/// </summary>
	/// <remarks>
	///     <para>
	///         This collation uses <see cref="StringComparerEx.TrimmedOrdinal" />
	///     </para>
	///     <para>
	///         The SQL name of the collation is <c> TrimmedOrdinal </c>.
	///     </para>
	/// </remarks>
	[SQLiteFunction (FuncType = FunctionType.Collation, Name = "TrimmedOrdinal")]
	public sealed class TrimmedOrdinalSQLiteCollation : SQLiteFunction
	{
		#region Static Methods

		/// <summary>
		///     Registers the collation.
		/// </summary>
		public static void RegisterGlobal ()
		{
			SQLiteFunction.RegisterFunction(typeof(TrimmedOrdinalSQLiteCollation));
		}

		#endregion




		#region Overrides

		/// <inheritdoc />
		public override int Compare (string param1, string param2)
		{
			return StringComparerEx.TrimmedOrdinal.Compare(param1, param2);
		}

		#endregion
	}
}
