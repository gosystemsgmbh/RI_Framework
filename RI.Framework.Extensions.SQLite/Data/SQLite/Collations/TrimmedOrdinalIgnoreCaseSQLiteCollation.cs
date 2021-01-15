using System.Data.SQLite;

using RI.Framework.Utilities;




namespace RI.Framework.Data.SQLite.Collations
{
    /// <summary>
    ///     Implements an SQLite collation which performs trimmed, case-insensitive ordinal string comparison.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This collation uses <see cref="StringComparerEx.TrimmedOrdinalIgnoreCase" />
    ///     </para>
    ///     <para>
    ///         The SQL name of the collation is <c> TrimmedOrdinalIgnoreCase </c>.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    [SQLiteFunction(FuncType = FunctionType.Collation, Name = "TrimmedOrdinalIgnoreCase")]
    public sealed class TrimmedOrdinalIgnoreCaseSQLiteCollation : SQLiteFunction
    {
        #region Static Methods

        /// <summary>
        ///     Registers the collation.
        /// </summary>
        public static void RegisterGlobal ()
        {
            SQLiteFunction.RegisterFunction(typeof(TrimmedOrdinalIgnoreCaseSQLiteCollation));
        }

        #endregion




        #region Overrides

        /// <inheritdoc />
        public override int Compare (string param1, string param2)
        {
            return StringComparerEx.TrimmedOrdinalIgnoreCase.Compare(param1, param2);
        }

        #endregion
    }
}
