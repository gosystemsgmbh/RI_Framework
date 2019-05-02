using System;
using System.Data.SQLite;

using RI.Framework.Utilities;




namespace RI.Framework.Data.SQLite.Functions
{
    /// <summary>
    ///     Implements an SQLite function which returns an empty string if a value is a string which is null or empty or consists only of whitespaces.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The SQL name of the function is <c> toemptyifemptyornull </c>.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    /// <example>
    ///     <code language="sql">
    /// <![CDATA[
    /// toemptyifnulloremptyorwhitespace(NULL)
    /// toemptyifnulloremptyorwhitespace(' ')
    /// toemptyifnulloremptyorwhitespace(column)
    /// ]]>
    /// </code>
    /// </example>
    [SQLiteFunction("toemptyifnulloremptyorwhitespace", 1, FunctionType.Scalar)]
    public sealed class ToEmptyIfNullOrEmptyOrWhitespaceSQLiteFunction : SQLiteFunction
    {
        #region Static Methods

        /// <summary>
        ///     Registers the function.
        /// </summary>
        public static void RegisterGlobal ()
        {
            SQLiteFunction.RegisterFunction(typeof(ToEmptyIfNullOrEmptyOrWhitespaceSQLiteFunction));
        }

        #endregion




        #region Overrides

        /// <inheritdoc />
        public override object Invoke (object[] args)
        {
            if (args == null)
            {
                return string.Empty;
            }

            if (args.Length != 1)
            {
                return string.Empty;
            }

            object arg = args[0];

            if (arg == null)
            {
                return string.Empty;
            }

            if (arg == DBNull.Value)
            {
                return string.Empty;
            }

            if (!(arg is string))
            {
                return arg;
            }

            return ((string)arg).IsNullOrEmptyOrWhitespace() ? string.Empty : arg;
        }

        #endregion
    }
}
