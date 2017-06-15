using System;
using System.Data.SQLite;
using System.Diagnostics.CodeAnalysis;

using RI.Framework.Utilities;




namespace RI.Framework.Data.SQLite.Functions
{
	/// <summary>
	///     Implements an SQLite function which checks whether a string contains a match of an other string.
	/// </summary>
	/// <remarks>
	///     <para>
	///         <see cref="StringExtensions" />.<see cref="StringExtensions.Contains(string,string,StringComparison)" /> with <see cref="StringComparison.OrdinalIgnoreCase" /> is used to match the input and the pattern.
	///     </para>
	///     <para>
	///         The SQL name of the function is <c> match </c>.
	///     </para>
	/// </remarks>
	/// <example>
	///     <code language="sql">
	/// <![CDATA[
	/// column MATCH 'pattern'
	/// match('pattern', column)
	/// ]]>
	/// </code>
	/// </example>
	[SQLiteFunction("match", 2, FunctionType.Scalar)]
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	public sealed class MatchSQLiteFunction : SQLiteFunction
	{
		#region Static Methods

		/// <summary>
		///     Registers the function.
		/// </summary>
		public static void RegisterGlobal ()
		{
			SQLiteFunction.RegisterFunction(typeof(MatchSQLiteFunction));
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

			return ((string)input).Contains((string)pattern, StringComparison.OrdinalIgnoreCase);
		}

		#endregion
	}
}
