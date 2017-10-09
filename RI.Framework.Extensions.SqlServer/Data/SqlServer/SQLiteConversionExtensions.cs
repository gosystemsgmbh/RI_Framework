using System;
using System.Diagnostics.CodeAnalysis;

using RI.Framework.Utilities;

namespace RI.Framework.Data.SqlServer
{
	/// <summary>
	///     Provides utility/extension methods for objects returned by SQL Server queries.
	/// </summary>
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	public static class SqlServerConversionExtensions
	{
		#region Static Methods

		/// <summary>
		///     Attempts to convert a value from a SQL Server query result to <see cref="int" />.
		/// </summary>
		/// <param name="value"> The value to convert. </param>
		/// <returns>
		///     The converted value or null if <paramref name="value" /> is null, <see cref="DBNull" />, or can not be converted to <see cref="int" />.
		/// </returns>
		public static int? Int32FromSqlServerResult (this object value)
		{
			if (value == null)
			{
				return null;
			}

			if (value is DBNull)
			{
				return null;
			}

			if (value is sbyte)
			{
				return (sbyte)value;
			}

			if (value is byte)
			{
				return (byte)value;
			}

			if (value is short)
			{
				return (short)value;
			}

			if (value is ushort)
			{
				return (ushort)value;
			}

			if (value is int)
			{
				return (int)value;
			}

			if (value is uint)
			{
				uint testValue = (uint)value;
				return (testValue > int.MaxValue) ? (int?)null : (int)testValue;
			}

			if (value is long)
			{
				long testValue = (long)value;
				return (testValue < int.MinValue) || (testValue > int.MaxValue) ? (int?)null : (int)testValue;
			}

			if (value is ulong)
			{
				ulong testValue = (ulong)value;
				return (testValue > int.MaxValue) ? (int?)null : (int)testValue;
			}

			if (value is float)
			{
				float testValue = (float)value;
				return (testValue < int.MinValue) || (testValue > int.MaxValue) ? (int?)null : (int)testValue;
			}

			if (value is double)
			{
				double testValue = (double)value;
				return (testValue < int.MinValue) || (testValue > int.MaxValue) ? (int?)null : (int)testValue;
			}

			if (value is decimal)
			{
				decimal testValue = (decimal)value;
				return (testValue < int.MinValue) || (testValue > int.MaxValue) ? (int?)null : (int)testValue;
			}

			if (value is string)
			{
				return ((string)value).ToInt32Invariant();
			}

			return null;
		}

		/// <summary>
		///     Attempts to convert a value from a SQL Server query result to <see cref="long" />.
		/// </summary>
		/// <param name="value"> The value to convert. </param>
		/// <returns>
		///     The converted value or null if <paramref name="value" /> is null, <see cref="DBNull" />, or can not be converted to <see cref="long" />.
		/// </returns>
		public static long? Int64FromSqlServerResult (this object value)
		{
			if (value == null)
			{
				return null;
			}

			if (value is DBNull)
			{
				return null;
			}

			if (value is sbyte)
			{
				return (sbyte)value;
			}

			if (value is byte)
			{
				return (byte)value;
			}

			if (value is short)
			{
				return (short)value;
			}

			if (value is ushort)
			{
				return (ushort)value;
			}

			if (value is int)
			{
				return (int)value;
			}

			if (value is uint)
			{
				return (uint)value;
			}

			if (value is long)
			{
				return (long)value;
			}

			if (value is ulong)
			{
				ulong testValue = (ulong)value;
				return (testValue > long.MaxValue) ? (long?)null : (long)testValue;
			}

			if (value is float)
			{
				float testValue = (float)value;
				return (testValue < long.MinValue) || (testValue > long.MaxValue) ? (long?)null : (long)testValue;
			}

			if (value is double)
			{
				double testValue = (double)value;
				return (testValue < long.MinValue) || (testValue > long.MaxValue) ? (long?)null : (long)testValue;
			}

			if (value is decimal)
			{
				decimal testValue = (decimal)value;
				return (testValue < long.MinValue) || (testValue > long.MaxValue) ? (long?)null : (long)testValue;
			}

			if (value is string)
			{
				return ((string)value).ToInt64Invariant();
			}

			return null;
		}

		#endregion
	}
}
