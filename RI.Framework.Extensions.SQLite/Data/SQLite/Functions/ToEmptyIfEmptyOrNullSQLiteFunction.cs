using System;
using System.Data.SQLite;
using System.Diagnostics.CodeAnalysis;

using RI.Framework.Utilities;




namespace RI.Framework.Data.SQLite.Functions
{
	/// <summary>
	///     Implements an SQLite function which returns an empty string if a value is an empty string or null.
	/// </summary>
	/// <remarks>
	///     <para>
	///         A string is considered empty if it is NULL or consists only of whitespaces.
	///     </para>
	///     <para>
	///         The SQL name of the function is <c> toemptyifemptyornull </c>.
	///     </para>
	/// </remarks>
	/// <example>
	///     <code language="sql">
	/// <![CDATA[
	/// toemptyifemptyornull(NULL)
	/// toemptyifemptyornull(' ')
	/// toemptyifemptyornull(column)
	/// ]]>
	/// </code>
	/// </example>
	[SQLiteFunction ("toemptyifemptyornull", 1, FunctionType.Scalar)]
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	public class ToEmptyIfEmptyOrNullSQLiteFunction : SQLiteFunction
	{
		#region Static Methods

		/// <summary>
		///     Registers the function.
		/// </summary>
		public static void RegisterGlobal ()
		{
			SQLiteFunction.RegisterFunction(typeof(ToEmptyIfEmptyOrNullSQLiteFunction));
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
