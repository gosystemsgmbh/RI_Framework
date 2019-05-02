using System.Data.SQLite;
using System.Diagnostics.CodeAnalysis;

using RI.Framework.Utilities;




namespace RI.Framework.Data.SQLite.Collations
{
    /// <summary>
    ///     Implements an SQLite collation which performs case-insensitive ordinal string comparison.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This collation uses <see cref="StringComparerEx.OrdinalIgnoreCase" />
    ///     </para>
    ///     <para>
    ///         The SQL name of the collation is <c> OrdinalIgnoreCase </c>.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    [SQLiteFunction(FuncType = FunctionType.Collation, Name = "OrdinalIgnoreCase")]
    public sealed class OrdinalIgnoreCaseSQLiteCollation : SQLiteFunction
    {
        #region Static Methods

        /// <summary>
        ///     Registers the collation.
        /// </summary>
        public static void RegisterGlobal ()
        {
            SQLiteFunction.RegisterFunction(typeof(OrdinalIgnoreCaseSQLiteCollation));
        }

        #endregion




        #region Overrides

        /// <inheritdoc />
        public override int Compare (string param1, string param2)
        {
            return StringComparerEx.OrdinalIgnoreCase.Compare(param1, param2);
        }

        #endregion
    }
}
