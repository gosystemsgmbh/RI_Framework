using System;
using System.Data.SQLite;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;




namespace RI.Framework.Data.SQLite.Functions
{
	/// <summary>
	///     Implements an SQLite function which checks whether a string matches with a regular expression.
	/// </summary>
	/// <remarks>
	///     <para>
	///         <see cref="Regex" />.<see cref="Regex.IsMatch(string,string)" /> is used to match the input and the pattern.
	///     </para>
	///     <para>
	///         The SQL name of the function is <c> regexp </c>.
	///     </para>
	/// </remarks>
	/// <example>
	///     <code language="sql">
	/// <![CDATA[
	/// column REGEXP 'pattern'
	/// regexp('pattern', column)
	/// ]]>
	/// </code>
	/// </example>
	[SQLiteFunction("regexp", 2, FunctionType.Scalar)]
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	public sealed class RegularExpressionSQLiteFunction : SQLiteFunction
	{
		#region Static Methods

		/// <summary>
		///     Registers the function.
		/// </summary>
		public static void RegisterGlobal ()
		{
			SQLiteFunction.RegisterFunction(typeof(RegularExpressionSQLiteFunction));
		}

		#endregion




		#region Overrides

		/// <inheritdoc />
		public override object Invoke (object[] args)
		{
			if (args == null)
			{
				return false;
			}

			if (args.Length != 2)
			{
				return false;
			}

			object pattern = args[0];
			object input = args[1];

			if ((pattern == null) || (pattern == DBNull.Value))
			{
				return false;
			}

			if (!(pattern is string))
			{
				return false;
			}

			if ((input == null) || (input == DBNull.Value))
			{
				input = string.Empty;
			}

			if (!(input is string))
			{
				input = input.ToString();
			}

			return Regex.IsMatch((string)input, (string)pattern);
		}

		#endregion
	}
}
