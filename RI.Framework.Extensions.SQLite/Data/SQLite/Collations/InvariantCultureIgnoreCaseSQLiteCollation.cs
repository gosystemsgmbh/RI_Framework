using System.Data.SQLite;
using System.Diagnostics.CodeAnalysis;

using RI.Framework.Utilities;




namespace RI.Framework.Data.SQLite.Collations
{
	/// <summary>
	///     Implements an SQLite collation which performs case-insensitive string comparison using the invariant culture.
	/// </summary>
	/// <remarks>
	///     <para>
	///         This collation uses <see cref="StringComparerEx.InvariantCultureIgnoreCase" />
	///     </para>
	///     <para>
	///         The SQL name of the collation is <c> InvariantCultureIgnoreCase </c>.
	///     </para>
	/// </remarks>
	[SQLiteFunction(FuncType = FunctionType.Collation, Name = "InvariantCultureIgnoreCase")]
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	public sealed class InvariantCultureIgnoreCaseSQLiteCollation : SQLiteFunction
	{
		#region Static Methods

		/// <summary>
		///     Registers the collation.
		/// </summary>
		public static void RegisterGlobal ()
		{
			SQLiteFunction.RegisterFunction(typeof(InvariantCultureIgnoreCaseSQLiteCollation));
		}

		#endregion




		#region Overrides

		/// <inheritdoc />
		public override int Compare (string param1, string param2)
		{
			return StringComparerEx.InvariantCultureIgnoreCase.Compare(param1, param2);
		}

		#endregion
	}
}
