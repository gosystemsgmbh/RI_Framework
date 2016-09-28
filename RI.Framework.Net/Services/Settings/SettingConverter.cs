using System;
using System.Globalization;

using RI.Framework.Collections;
using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;




namespace RI.Framework.Services.Settings
{
	/// <summary>
	///     Implements a default setting converter which can convert to and from the basic types used in .NET.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The types supported by this seting converter are:
	/// <see cref="bool"/>, <see cref="char"/>, <see cref="string"/>, <see cref="sbyte"/>, <see cref="byte"/>, <see cref="short"/>, <see cref="ushort"/>, <see cref="int"/>, <see cref="uint"/>, <see cref="long"/>, <see cref="ulong"/>, <see cref="float"/>, <see cref="double"/>, <see cref="decimal"/>, <see cref="DateTime"/>, <see cref="TimeSpan"/>, <see cref="Guid"/>, <see cref="Version"/>, and enumerations (<see cref="Enum"/>)
	/// </para>
	/// </remarks>
	public sealed class SettingConverter : ISettingConverter
	{
		private readonly Type[] SupportedTypes =
		{
			typeof(bool),
			typeof(char),
			typeof(string),
			typeof(sbyte),
			typeof(byte),
			typeof(short),
			typeof(ushort),
			typeof(int),
			typeof(uint),
			typeof(long),
			typeof(ulong),
			typeof(float),
			typeof(double),
			typeof(decimal),
			typeof(DateTime),
			typeof(TimeSpan),
			typeof(Guid),
			typeof(Version),
			typeof(Enum),
		};

		/// <inheritdoc />
		public bool CanConvert (Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			if (type.IsEnum)
			{
				return true;
			}

			return this.SupportedTypes.Contains(type);
		}

		/// <inheritdoc />
		public string ConvertFrom (Type type, object value)
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			if (value == null)
			{
				throw new ArgumentNullException(nameof(value));
			}

			if ((type != value.GetType()) && (type.IsEnum && (!value.GetType().IsEnum)))
			{
				throw new InvalidTypeArgumentException(nameof(value));
			}

			if (type.IsEnum)
			{
				return Convert.ToInt32(value).ToString("D", CultureInfo.InvariantCulture);
			}
			if (type == typeof(bool))
			{
				return ((bool)value).ToString(CultureInfo.InvariantCulture);
			}
			if (type == typeof(char))
			{
				return ((char)value).ToString();
			}
			if (type == typeof(string))
			{
				return (string)value;
			}
			if (type == typeof(sbyte))
			{
				return ((sbyte)value).ToString("D", CultureInfo.InvariantCulture);
			}
			if (type == typeof(byte))
			{
				return ((byte)value).ToString("D", CultureInfo.InvariantCulture);
			}
			if (type == typeof(short))
			{
				return ((short)value).ToString("D", CultureInfo.InvariantCulture);
			}
			if (type == typeof(ushort))
			{
				return ((ushort)value).ToString("D", CultureInfo.InvariantCulture);
			}
			if (type == typeof(int))
			{
				return ((int)value).ToString("D", CultureInfo.InvariantCulture);
			}
			if (type == typeof(uint))
			{
				return ((uint)value).ToString("D", CultureInfo.InvariantCulture);
			}
			if (type == typeof(long))
			{
				return ((long)value).ToString("D", CultureInfo.InvariantCulture);
			}
			if (type == typeof(ulong))
			{
				return ((ulong)value).ToString("D", CultureInfo.InvariantCulture);
			}
			if (type == typeof(float))
			{
				return ((float)value).ToString("F", CultureInfo.InvariantCulture);
			}
			if (type == typeof(double))
			{
				return ((double)value).ToString("F", CultureInfo.InvariantCulture);
			}
			if (type == typeof(decimal))
			{
				return ((decimal)value).ToString("F", CultureInfo.InvariantCulture);
			}
			if (type == typeof(DateTime))
			{
				return ((DateTime)value).ToSortableString();
			}
			if (type == typeof(TimeSpan))
			{
				return ((TimeSpan)value).ToSortableString();
			}
			if (type == typeof(Guid))
			{
				return ((Guid)value).ToString("N", CultureInfo.InvariantCulture).ToUpperInvariant();
			}
			if (type == typeof(Version))
			{
				return ((Version)value).ToString(4);
			}

			throw new InvalidTypeArgumentException(nameof(value));
		}

		/// <inheritdoc />
		public object ConvertTo (Type type, string value)
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			if (value == null)
			{
				throw new ArgumentNullException(nameof(value));
			}

			object finalValue = null;

			if (type.IsEnum)
			{
				finalValue = value.ToEnum(type);
			}
			else if (type == typeof(bool))
			{
				bool? tempValue = value.ToBoolean();
				finalValue = tempValue.HasValue ? tempValue.Value : (object)null;
			}
			else if (type == typeof(char))
			{
				//TODO
			}
			else if (type == typeof(string))
			{
			}
			else if (type == typeof(sbyte))
			{
			}
			else if (type == typeof(byte))
			{
			}
			else if (type == typeof(short))
			{
			}
			else if (type == typeof(ushort))
			{
			}
			else if (type == typeof(int))
			{
			}
			else if (type == typeof(uint))
			{
			}
			else if (type == typeof(long))
			{
			}
			else if (type == typeof(ulong))
			{
			}
			else if (type == typeof(float))
			{
			}
			else if (type == typeof(double))
			{
			}
			else if (type == typeof(decimal))
			{
			}
			else if (type == typeof(DateTime))
			{
			}
			else if (type == typeof(TimeSpan))
			{
			}
			else if (type == typeof(Guid))
			{
			}
			else if (type == typeof(Version))
			{
			}
			else
			{
				throw new InvalidTypeArgumentException(nameof(type));
			}

			if (finalValue == null)
			{
				throw new FormatException();
			}
			
			return finalValue;
		}
	}
}