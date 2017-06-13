using System;
using System.Data.SQLite;
using System.Diagnostics.CodeAnalysis;

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
	[SuppressMessage("ReSharper", "InconsistentNaming")]
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

			return ((string)arg).IsNullOrEmptyOrWhitespace();
		}

		#endregion
	}
}
