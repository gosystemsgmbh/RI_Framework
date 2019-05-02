using System.Data.SQLite;
using System.Diagnostics.CodeAnalysis;

using RI.Framework.Utilities;




namespace RI.Framework.Data.SQLite.Collations
{
    /// <summary>
    ///     Implements an SQLite collation which performs trimmed, case-sensitive string comparison using the current culture.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This collation uses <see cref="StringComparerEx.TrimmedCurrentCulture" />
    ///     </para>
    ///     <para>
    ///         The SQL name of the collation is <c> TrimmedCurrentCulture </c>.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    [SQLiteFunction(FuncType = FunctionType.Collation, Name = "TrimmedCurrentCulture")]
    public sealed class TrimmedCurrentCultureSQLiteCollation : SQLiteFunction
    {
        #region Static Methods

        /// <summary>
        ///     Registers the collation.
        /// </summary>
        public static void RegisterGlobal ()
        {
            SQLiteFunction.RegisterFunction(typeof(TrimmedCurrentCultureSQLiteCollation));
        }

        #endregion




        #region Overrides

        /// <inheritdoc />
        public override int Compare (string param1, string param2)
        {
            return StringComparerEx.TrimmedCurrentCulture.Compare(param1, param2);
        }

        #endregion
    }
}
