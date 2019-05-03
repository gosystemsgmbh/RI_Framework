using System.Data.SQLite;

using RI.Framework.Utilities;




namespace RI.Framework.Data.SQLite.Collations
{
    /// <summary>
    ///     Implements an SQLite collation which performs case-insensitive string comparison using the current culture.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This collation uses <see cref="StringComparerEx.CurrentCultureIgnoreCase" />
    ///     </para>
    ///     <para>
    ///         The SQL name of the collation is <c> CurrentCultureIgnoreCase </c>.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    [SQLiteFunction(FuncType = FunctionType.Collation, Name = "CurrentCultureIgnoreCase")]
    public sealed class CurrentCultureIgnoreCaseSQLiteCollation : SQLiteFunction
    {
        #region Static Methods

        /// <summary>
        ///     Registers the collation.
        /// </summary>
        public static void RegisterGlobal ()
        {
            SQLiteFunction.RegisterFunction(typeof(CurrentCultureIgnoreCaseSQLiteCollation));
        }

        #endregion




        #region Overrides

        /// <inheritdoc />
        public override int Compare (string param1, string param2)
        {
            return StringComparerEx.CurrentCultureIgnoreCase.Compare(param1, param2);
        }

        #endregion
    }
}
