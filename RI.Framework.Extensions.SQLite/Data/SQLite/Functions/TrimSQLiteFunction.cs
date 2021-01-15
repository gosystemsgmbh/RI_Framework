using System;
using System.Data.SQLite;




namespace RI.Framework.Data.SQLite.Functions
{
    /// <summary>
    ///     Implements an SQLite function which returns a trimmed string.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         A a trimmed string has all its leading and trailing whitespaces removed.
    ///     </para>
    ///     <para>
    ///         The SQL name of the function is <c> trim </c>.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    /// <example>
    ///     <code language="sql">
    /// <![CDATA[
    /// trim(NULL)
    /// trim(' ')
    /// trim(column)
    /// ]]>
    /// </code>
    /// </example>
    [SQLiteFunction("trim", 1, FunctionType.Scalar)]
    public sealed class TrimSQLiteFunction : SQLiteFunction
    {
        #region Static Methods

        /// <summary>
        ///     Registers the function.
        /// </summary>
        public static void RegisterGlobal ()
        {
            SQLiteFunction.RegisterFunction(typeof(TrimSQLiteFunction));
        }

        #endregion




        #region Overrides

        /// <inheritdoc />
        public override object Invoke (object[] args)
        {
            if (args == null)
            {
                return null;
            }

            if (args.Length != 1)
            {
                return null;
            }

            object arg = args[0];

            if (arg == null)
            {
                return null;
            }

            if (arg == DBNull.Value)
            {
                return null;
            }

            if (!(arg is string))
            {
                return arg;
            }

            return ((string)arg).Trim();
        }

        #endregion
    }
}
