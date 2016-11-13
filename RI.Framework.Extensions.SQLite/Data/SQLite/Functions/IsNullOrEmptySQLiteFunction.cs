using System;
using System.Data.SQLite;

using RI.Framework.Utilities;




namespace RI.Framework.Data.SQLite.Functions
{
	/// <summary>
	///     Implements an SQLite function which checks whether a string is empty or not.
	/// </summary>
	/// <remarks>
	///     <para>
	///         A string is considered empty if it is NULL or consists only of whitespaces.
	///     </para>
	///     <para>
	///         The SQL name of the function is <c> isnullorempty </c>.
	///     </para>
	/// </remarks>
	/// <example>
	///     <code language="sql">
	/// <![CDATA[
	/// isnullorempty(NULL)
	/// isnullorempty(' ')
	/// isnullorempty(column)
	/// ]]>
	/// </code>
	/// </example>
	[SQLiteFunction ("isnullorempty", 1, FunctionType.Scalar)]
	public sealed class IsNullOrEmptySQLiteFunction : SQLiteFunction
	{
		#region Static Methods

		/// <summary>
		///     Registers the function.
		/// </summary>
		public static void Register ()
		{
			SQLiteFunction.RegisterFunction(typeof(IsNullOrEmptySQLiteFunction));
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
				return false;
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
				return false;
			}

			return ((string)arg).IsNullOrEmpty();
		}

		#endregion
	}
}
