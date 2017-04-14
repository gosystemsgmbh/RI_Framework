using System;
using System.Data.SQLite;
using System.Diagnostics.CodeAnalysis;

using RI.Framework.Utilities;




namespace RI.Framework.Data.SQLite.Functions
{
	/// <summary>
	///     Implements an SQLite function which returns null if a value is an empty string or null.
	/// </summary>
	/// <remarks>
	///     <para>
	///         A string is considered empty if it is NULL or consists only of whitespaces.
	///     </para>
	///     <para>
	///         The SQL name of the function is <c> tonullifemptyornull </c>.
	///     </para>
	/// </remarks>
	/// <example>
	///     <code language="sql">
	/// <![CDATA[
	/// tonullifemptyornull(NULL)
	/// tonullifemptyornull(' ')
	/// tonullifemptyornull(column)
	/// ]]>
	/// </code>
	/// </example>
	[SQLiteFunction("tonullifemptyornull", 1, FunctionType.Scalar)]
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	public class ToNullIfEmptyOrNullSQLiteFunction : SQLiteFunction
	{
		#region Static Methods

		/// <summary>
		///     Registers the function.
		/// </summary>
		public static void RegisterGlobal ()
		{
			SQLiteFunction.RegisterFunction(typeof(ToNullIfEmptyOrNullSQLiteFunction));
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

			return ((string)arg).IsNullOrEmptyOrWhitespace() ? null : arg;
		}

		#endregion
	}
}
