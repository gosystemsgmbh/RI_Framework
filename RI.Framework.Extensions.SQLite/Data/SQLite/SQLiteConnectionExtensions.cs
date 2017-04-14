using System;
using System.Data.SQLite;
using System.Diagnostics.CodeAnalysis;
using System.Linq;




namespace RI.Framework.Data.SQLite
{
	/// <summary>
	///     Provides utility/extension methods for the <see cref="SQLiteConnection" /> type.
	/// </summary>
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	public static class SQLiteConnectionExtensions
	{
		#region Static Methods

		/// <summary>
		///     Binds an instance of a <see cref="SQLiteFunction" /> to a connection.
		/// </summary>
		/// <param name="connection"> The connection the function is bound to. </param>
		/// <param name="function"> The function to bind. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="connection" /> or <paramref name="function" /> is null. </exception>
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

		#endregion
	}
}
