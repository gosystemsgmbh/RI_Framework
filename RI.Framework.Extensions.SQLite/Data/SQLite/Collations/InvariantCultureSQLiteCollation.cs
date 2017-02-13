using System.Data.SQLite;
using System.Diagnostics.CodeAnalysis;

using RI.Framework.Utilities;




namespace RI.Framework.Data.SQLite.Collations
{
	/// <summary>
	///     Implements an SQLite collation which performs case-sensitive string comparison using the invariant culture.
	/// </summary>
	/// <remarks>
	///     <para>
	///         This collation uses <see cref="StringComparerEx.InvariantCulture" />
	///     </para>
	///     <para>
	///         The SQL name of the collation is <c> InvariantCulture </c>.
	///     </para>
	/// </remarks>
	[SQLiteFunction (FuncType = FunctionType.Collation, Name = "InvariantCulture")]
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	public sealed class InvariantCultureSQLiteCollation : SQLiteFunction
	{
		#region Static Methods

		/// <summary>
		///     Registers the collation.
		/// </summary>
		public static void RegisterGlobal ()
		{
			SQLiteFunction.RegisterFunction(typeof(InvariantCultureSQLiteCollation));
		}

		#endregion




		#region Overrides

		/// <inheritdoc />
		public override int Compare (string param1, string param2)
		{
			return StringComparerEx.InvariantCulture.Compare(param1, param2);
		}

		#endregion
	}
}
