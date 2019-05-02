using System.Data.SQLite;
using System.Diagnostics.CodeAnalysis;

using RI.Framework.Utilities;




namespace RI.Framework.Data.SQLite.Collations
{
    /// <summary>
    ///     Implements an SQLite collation which performs trimmed, case-insensitive string comparison using the invariant culture.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This collation uses <see cref="StringComparerEx.TrimmedInvariantCultureIgnoreCase" />
    ///     </para>
    ///     <para>
    ///         The SQL name of the collation is <c> TrimmedInvariantCultureIgnoreCase </c>.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    [SQLiteFunction(FuncType = FunctionType.Collation, Name = "TrimmedInvariantCultureIgnoreCase")]
    public sealed class TrimmedInvariantCultureIgnoreCaseSQLiteCollation : SQLiteFunction
    {
        #region Static Methods

        /// <summary>
        ///     Registers the collation.
        /// </summary>
        public static void RegisterGlobal ()
        {
            SQLiteFunction.RegisterFunction(typeof(TrimmedInvariantCultureIgnoreCaseSQLiteCollation));
        }

        #endregion




        #region Overrides

        /// <inheritdoc />
        public override int Compare (string param1, string param2)
        {
            return StringComparerEx.TrimmedInvariantCultureIgnoreCase.Compare(param1, param2);
        }

        #endregion
    }
}
