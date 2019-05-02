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
    ///         This collation uses <see cref="StringComparerEx.Ordinal" />
    ///     </para>
    ///     <para>
    ///         The SQL name of the collation is <c> Ordinal </c>.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    [SQLiteFunction(FuncType = FunctionType.Collation, Name = "Ordinal")]
    public sealed class OrdinalSQLiteCollation : SQLiteFunction
    {
        #region Static Methods

        /// <summary>
        ///     Registers the collation.
        /// </summary>
        public static void RegisterGlobal ()
        {
            SQLiteFunction.RegisterFunction(typeof(OrdinalSQLiteCollation));
        }

        #endregion




        #region Overrides

        /// <inheritdoc />
        public override int Compare (string param1, string param2)
        {
            return StringComparerEx.Ordinal.Compare(param1, param2);
        }

        #endregion
    }
}
