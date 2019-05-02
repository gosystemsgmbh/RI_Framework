using System;
using System.Data.SQLite;

using RI.Framework.Utilities;




namespace RI.Framework.Data.SQLite.Functions
{
    /// <summary>
    ///     Implements an SQLite function which checks whether a string is null or empty or consists only of whitespaces.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The SQL name of the function is <c> isnulloremptyorwhitespace </c>.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    /// <example>
    ///     <code language="sql">
    /// <![CDATA[
    /// isnulloremptyorwhitespace(NULL)
    /// isnulloremptyorwhitespace(' ')
    /// isnulloremptyorwhitespace(column)
    /// ]]>
    /// </code>
    /// </example>
    [SQLiteFunction("isnulloremptyorwhitespace", 1, FunctionType.Scalar)]
    public sealed class IsNullOrEmptyOrWhitespaceSQLiteFunction : SQLiteFunction
    {
        #region Static Methods

        /// <summary>
        ///     Registers the function.
        /// </summary>
        public static void RegisterGlobal ()
        {
            SQLiteFunction.RegisterFunction(typeof(IsNullOrEmptyOrWhitespaceSQLiteFunction));
        }

        #endregion




        #region Overrides

        /// <inheritdoc />
        public override object Invoke (object[] args)
        {
            if (args == null)
            {
                return true;
            }

            if (args.Length != 1)
            {
                return true;
            }

            object arg = args[0];

            if (arg == null)
            {
                return true;
            }

            if (arg == DBNull.Value)
            {
                return true;
            }

            if (!(arg is string))
            {
                return true;
            }

            return ((string)arg).IsNullOrEmptyOrWhitespace();
        }

        #endregion
    }
}
