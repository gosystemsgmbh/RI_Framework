using System.Data.SQLite;
using System.Diagnostics.CodeAnalysis;

using RI.Framework.Utilities;




namespace RI.Framework.Data.SQLite.Collations
{
	/// <summary>
	///     Implements an SQLite collation which performs trimmed, case-insensitive string comparison using the current culture.
	/// </summary>
	/// <remarks>
	///     <para>
	///         This collation uses <see cref="StringComparerEx.TrimmedCurrentCultureIgnoreCase" />
	///     </para>
	///     <para>
	///         The SQL name of the collation is <c> TrimmedCurrentCultureIgnoreCase </c>.
	///     </para>
	/// </remarks>
	[SQLiteFunction (FuncType = FunctionType.Collation, Name = "TrimmedCurrentCultureIgnoreCase")]
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	public sealed class TrimmedCurrentCultureIgnoreCaseSQLiteCollation : SQLiteFunction
	{
		#region Static Methods

		/// <summary>
		///     Registers the collation.
		/// </summary>
		public static void RegisterGlobal ()
		{
			SQLiteFunction.RegisterFunction(typeof(TrimmedCurrentCultureIgnoreCaseSQLiteCollation));
		}

		#endregion




		#region Overrides

		/// <inheritdoc />
		public override int Compare (string param1, string param2)
		{
			return StringComparerEx.TrimmedCurrentCultureIgnoreCase.Compare(param1, param2);
		}

		#endregion
	}
}
