using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Reflection;




namespace RI.Framework.Data.SQLite
{
    /// <summary>
    ///     Provides utility/extension methods for the <see cref="SQLiteConnection" /> type.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    /// TODO: Cache results of FindFrameworkFunctions
    public static class SQLiteConnectionExtensions
    {
        #region Static Methods

        /// <summary>
        ///     Binds all SQLite collations provided by this framework to a connection.
        /// </summary>
        /// <param name="connection"> The connection the collations are bound to. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="connection" /> is null. </exception>
        public static void BindFrameworkCollations (this SQLiteConnection connection)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            List<SQLiteFunction> functions = SQLiteConnectionExtensions.FindFrameworkFunctions(x => x.FuncType == FunctionType.Collation);
            functions.ForEach(connection.BindFunction);
        }

        /// <summary>
        ///     Binds all SQLite functions provided by this framework to a connection.
        /// </summary>
        /// <param name="connection"> The connection the functions are bound to. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="connection" /> is null. </exception>
        public static void BindFrameworkFunctions (this SQLiteConnection connection)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            List<SQLiteFunction> functions = SQLiteConnectionExtensions.FindFrameworkFunctions(x => (x.FuncType == FunctionType.Scalar) || (x.FuncType == FunctionType.Aggregate));
            functions.ForEach(connection.BindFunction);
        }

        /// <summary>
        ///     Binds an instance of a <see cref="SQLiteFunction" /> to a connection.
        /// </summary>
        /// <param name="connection"> The connection the function is bound to. </param>
        /// <param name="function"> The function to bind. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="connection" /> or <paramref name="function" /> is null. </exception>
        /// <exception cref="ArgumentException">The type of <paramref name="function"/> does not have <see cref="SQLiteFunctionAttribute"/> applied.</exception>
        public static void BindFunction (this SQLiteConnection connection, SQLiteFunction function)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (function == null)
            {
                throw new ArgumentNullException(nameof(function));
            }

            SQLiteFunctionAttribute attribute = function.GetType().GetCustomAttributes(typeof(SQLiteFunctionAttribute), true).OfType<SQLiteFunctionAttribute>().FirstOrDefault();
            if (attribute == null)
            {
                throw new ArgumentException("The specified SQLite function does not have " + nameof(SQLiteFunctionAttribute) + " applied.", nameof(function));
            }

            connection.BindFunction(attribute, function);
        }

        private static List<SQLiteFunction> FindFrameworkFunctions (Predicate<SQLiteFunctionAttribute> attributePredicate)
        {
            bool Predicate (Type type)
            {
                SQLiteFunctionAttribute attribute = type.GetCustomAttribute<SQLiteFunctionAttribute>();
                if (attribute == null)
                {
                    return false;
                }

                return attributePredicate(attribute);
            }

            List<SQLiteFunction> functions = (from x in Assembly.GetExecutingAssembly().GetTypes() where typeof(SQLiteFunction).IsAssignableFrom(x) && (!x.IsAbstract) && Predicate(x) select (SQLiteFunction)Activator.CreateInstance(x)).ToList();
            return functions;
        }

        #endregion
    }
}
