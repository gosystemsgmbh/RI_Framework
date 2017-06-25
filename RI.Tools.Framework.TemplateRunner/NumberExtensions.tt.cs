
#if !TEMPLATE_RUNNER

using System;
using System.Diagnostics.CodeAnalysis;

// ReSharper disable RedundantCast

namespace RI.Framework.Mathematic
{
	/// <summary>
	///     Provides utility/extension methods for numerical types.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Numerical types are: <see cref="sbyte"/>, <see cref="byte"/>, <see cref="short"/>, <see cref="ushort"/>, <see cref="int"/>, <see cref="uint"/>, <see cref="long"/>, <see cref="ulong"/>, <see cref="float"/>, <see cref="double"/>, <see cref="decimal"/>.
	/// </para>
	/// </remarks>
	public static class NumberExtensions
	{

		/// <summary>
		/// Clamps a value between an inclusive minimum and maximum value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="min">The lowest possible value.</param>
		/// <param name="max">The highest possible value.</param>
		/// <returns>
		/// The clamped value.
		/// </returns>
		public static sbyte Clamp (this sbyte value, sbyte min, sbyte max)
		{
			if(value < min)
			{
				return min;
			}
			if(value > max)
			{
				return max;
			}
			return value;
		}

		/// <summary>
		/// Clamps a value to an inclusive minimum value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="min">The lowest possible value.</param>
		/// <returns>
		/// The clamped value.
		/// </returns>
		public static sbyte ClampMin (this sbyte value, sbyte min)
		{
			if(value < min)
			{
				return min;
			}
			return value;
		}

		/// <summary>
		/// Clamps a value to an inclusive maximum value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="max">The highest possible value.</param>
		/// <returns>
		/// The clamped value.
		/// </returns>
		public static sbyte ClampMax (this sbyte value, sbyte max)
		{
			if(value > max)
			{
				return max;
			}
			return value;
		}

		/// <summary>
		/// Quantizes a value to the nearest value of a multiple.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="multiple">The multiple.</param>
		/// <returns>
		/// The quantized value.
		/// </returns>
		/// <remarks>
		/// <para>
		/// <see cref="MidpointRounding.ToEven"/> is used for <see cref="MidpointRounding"/>.
		/// </para>
		/// </remarks>
		public static sbyte Quantize (this sbyte value, sbyte multiple) => (sbyte)(Math.Round((double)value / (double)multiple) * (double)multiple);

		/// <summary>
		/// Quantizes a value to the nearest value of a multiple.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="multiple">The multiple.</param>
		/// <param name="rounding">The kind of rounding to use.</param>
		/// <returns>
		/// The quantized value.
		/// </returns>
		public static sbyte Quantize (this sbyte value, sbyte multiple, MidpointRounding rounding) => (sbyte)(Math.Round((double)value / (double)multiple, rounding) * (double)multiple);

		/// <summary>
		/// Adds a number to a value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="addition">The number to add.</param>
		/// <returns>
		/// The result of: value + addition.
		/// </returns>
		public static sbyte Add (this sbyte value, sbyte addition) => (sbyte)(value + addition);

		/// <summary>
		/// Subtracts a number from a value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="subtract">The number to subtract.</param>
		/// <returns>
		/// The result of: value - subtract.
		/// </returns>
		public static sbyte Subtract (this sbyte value, sbyte subtract) => (sbyte)(value - subtract);

		/// <summary>
		/// Subtracts a value from a number.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="subtractFrom">The number to subtract from.</param>
		/// <returns>
		/// The result of: subtractFrom - value.
		/// </returns>
		public static sbyte SubtractFrom (this sbyte value, sbyte subtractFrom) => (sbyte)(subtractFrom - value);

		/// <summary>
		/// Multiplies a value by a factor.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="factor">The factor to multiply with.</param>
		/// <returns>
		/// The result of: value * factor.
		/// </returns>
		public static sbyte Multiply (this sbyte value, sbyte factor) => (sbyte)(value * factor);

		/// <summary>
		/// Clamps a value between an inclusive minimum and maximum value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="min">The lowest possible value.</param>
		/// <param name="max">The highest possible value.</param>
		/// <returns>
		/// The clamped value.
		/// </returns>
		public static byte Clamp (this byte value, byte min, byte max)
		{
			if(value < min)
			{
				return min;
			}
			if(value > max)
			{
				return max;
			}
			return value;
		}

		/// <summary>
		/// Clamps a value to an inclusive minimum value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="min">The lowest possible value.</param>
		/// <returns>
		/// The clamped value.
		/// </returns>
		public static byte ClampMin (this byte value, byte min)
		{
			if(value < min)
			{
				return min;
			}
			return value;
		}

		/// <summary>
		/// Clamps a value to an inclusive maximum value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="max">The highest possible value.</param>
		/// <returns>
		/// The clamped value.
		/// </returns>
		public static byte ClampMax (this byte value, byte max)
		{
			if(value > max)
			{
				return max;
			}
			return value;
		}

		/// <summary>
		/// Quantizes a value to the nearest value of a multiple.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="multiple">The multiple.</param>
		/// <returns>
		/// The quantized value.
		/// </returns>
		/// <remarks>
		/// <para>
		/// <see cref="MidpointRounding.ToEven"/> is used for <see cref="MidpointRounding"/>.
		/// </para>
		/// </remarks>
		public static byte Quantize (this byte value, byte multiple) => (byte)(Math.Round((double)value / (double)multiple) * (double)multiple);

		/// <summary>
		/// Quantizes a value to the nearest value of a multiple.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="multiple">The multiple.</param>
		/// <param name="rounding">The kind of rounding to use.</param>
		/// <returns>
		/// The quantized value.
		/// </returns>
		public static byte Quantize (this byte value, byte multiple, MidpointRounding rounding) => (byte)(Math.Round((double)value / (double)multiple, rounding) * (double)multiple);

		/// <summary>
		/// Adds a number to a value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="addition">The number to add.</param>
		/// <returns>
		/// The result of: value + addition.
		/// </returns>
		public static byte Add (this byte value, byte addition) => (byte)(value + addition);

		/// <summary>
		/// Subtracts a number from a value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="subtract">The number to subtract.</param>
		/// <returns>
		/// The result of: value - subtract.
		/// </returns>
		public static byte Subtract (this byte value, byte subtract) => (byte)(value - subtract);

		/// <summary>
		/// Subtracts a value from a number.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="subtractFrom">The number to subtract from.</param>
		/// <returns>
		/// The result of: subtractFrom - value.
		/// </returns>
		public static byte SubtractFrom (this byte value, byte subtractFrom) => (byte)(subtractFrom - value);

		/// <summary>
		/// Multiplies a value by a factor.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="factor">The factor to multiply with.</param>
		/// <returns>
		/// The result of: value * factor.
		/// </returns>
		public static byte Multiply (this byte value, byte factor) => (byte)(value * factor);

		/// <summary>
		/// Clamps a value between an inclusive minimum and maximum value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="min">The lowest possible value.</param>
		/// <param name="max">The highest possible value.</param>
		/// <returns>
		/// The clamped value.
		/// </returns>
		public static short Clamp (this short value, short min, short max)
		{
			if(value < min)
			{
				return min;
			}
			if(value > max)
			{
				return max;
			}
			return value;
		}

		/// <summary>
		/// Clamps a value to an inclusive minimum value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="min">The lowest possible value.</param>
		/// <returns>
		/// The clamped value.
		/// </returns>
		public static short ClampMin (this short value, short min)
		{
			if(value < min)
			{
				return min;
			}
			return value;
		}

		/// <summary>
		/// Clamps a value to an inclusive maximum value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="max">The highest possible value.</param>
		/// <returns>
		/// The clamped value.
		/// </returns>
		public static short ClampMax (this short value, short max)
		{
			if(value > max)
			{
				return max;
			}
			return value;
		}

		/// <summary>
		/// Quantizes a value to the nearest value of a multiple.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="multiple">The multiple.</param>
		/// <returns>
		/// The quantized value.
		/// </returns>
		/// <remarks>
		/// <para>
		/// <see cref="MidpointRounding.ToEven"/> is used for <see cref="MidpointRounding"/>.
		/// </para>
		/// </remarks>
		public static short Quantize (this short value, short multiple) => (short)(Math.Round((double)value / (double)multiple) * (double)multiple);

		/// <summary>
		/// Quantizes a value to the nearest value of a multiple.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="multiple">The multiple.</param>
		/// <param name="rounding">The kind of rounding to use.</param>
		/// <returns>
		/// The quantized value.
		/// </returns>
		public static short Quantize (this short value, short multiple, MidpointRounding rounding) => (short)(Math.Round((double)value / (double)multiple, rounding) * (double)multiple);

		/// <summary>
		/// Adds a number to a value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="addition">The number to add.</param>
		/// <returns>
		/// The result of: value + addition.
		/// </returns>
		public static short Add (this short value, short addition) => (short)(value + addition);

		/// <summary>
		/// Subtracts a number from a value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="subtract">The number to subtract.</param>
		/// <returns>
		/// The result of: value - subtract.
		/// </returns>
		public static short Subtract (this short value, short subtract) => (short)(value - subtract);

		/// <summary>
		/// Subtracts a value from a number.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="subtractFrom">The number to subtract from.</param>
		/// <returns>
		/// The result of: subtractFrom - value.
		/// </returns>
		public static short SubtractFrom (this short value, short subtractFrom) => (short)(subtractFrom - value);

		/// <summary>
		/// Multiplies a value by a factor.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="factor">The factor to multiply with.</param>
		/// <returns>
		/// The result of: value * factor.
		/// </returns>
		public static short Multiply (this short value, short factor) => (short)(value * factor);

		/// <summary>
		/// Clamps a value between an inclusive minimum and maximum value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="min">The lowest possible value.</param>
		/// <param name="max">The highest possible value.</param>
		/// <returns>
		/// The clamped value.
		/// </returns>
		public static ushort Clamp (this ushort value, ushort min, ushort max)
		{
			if(value < min)
			{
				return min;
			}
			if(value > max)
			{
				return max;
			}
			return value;
		}

		/// <summary>
		/// Clamps a value to an inclusive minimum value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="min">The lowest possible value.</param>
		/// <returns>
		/// The clamped value.
		/// </returns>
		public static ushort ClampMin (this ushort value, ushort min)
		{
			if(value < min)
			{
				return min;
			}
			return value;
		}

		/// <summary>
		/// Clamps a value to an inclusive maximum value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="max">The highest possible value.</param>
		/// <returns>
		/// The clamped value.
		/// </returns>
		public static ushort ClampMax (this ushort value, ushort max)
		{
			if(value > max)
			{
				return max;
			}
			return value;
		}

		/// <summary>
		/// Quantizes a value to the nearest value of a multiple.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="multiple">The multiple.</param>
		/// <returns>
		/// The quantized value.
		/// </returns>
		/// <remarks>
		/// <para>
		/// <see cref="MidpointRounding.ToEven"/> is used for <see cref="MidpointRounding"/>.
		/// </para>
		/// </remarks>
		public static ushort Quantize (this ushort value, ushort multiple) => (ushort)(Math.Round((double)value / (double)multiple) * (double)multiple);

		/// <summary>
		/// Quantizes a value to the nearest value of a multiple.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="multiple">The multiple.</param>
		/// <param name="rounding">The kind of rounding to use.</param>
		/// <returns>
		/// The quantized value.
		/// </returns>
		public static ushort Quantize (this ushort value, ushort multiple, MidpointRounding rounding) => (ushort)(Math.Round((double)value / (double)multiple, rounding) * (double)multiple);

		/// <summary>
		/// Adds a number to a value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="addition">The number to add.</param>
		/// <returns>
		/// The result of: value + addition.
		/// </returns>
		public static ushort Add (this ushort value, ushort addition) => (ushort)(value + addition);

		/// <summary>
		/// Subtracts a number from a value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="subtract">The number to subtract.</param>
		/// <returns>
		/// The result of: value - subtract.
		/// </returns>
		public static ushort Subtract (this ushort value, ushort subtract) => (ushort)(value - subtract);

		/// <summary>
		/// Subtracts a value from a number.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="subtractFrom">The number to subtract from.</param>
		/// <returns>
		/// The result of: subtractFrom - value.
		/// </returns>
		public static ushort SubtractFrom (this ushort value, ushort subtractFrom) => (ushort)(subtractFrom - value);

		/// <summary>
		/// Multiplies a value by a factor.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="factor">The factor to multiply with.</param>
		/// <returns>
		/// The result of: value * factor.
		/// </returns>
		public static ushort Multiply (this ushort value, ushort factor) => (ushort)(value * factor);

		/// <summary>
		/// Clamps a value between an inclusive minimum and maximum value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="min">The lowest possible value.</param>
		/// <param name="max">The highest possible value.</param>
		/// <returns>
		/// The clamped value.
		/// </returns>
		public static int Clamp (this int value, int min, int max)
		{
			if(value < min)
			{
				return min;
			}
			if(value > max)
			{
				return max;
			}
			return value;
		}

		/// <summary>
		/// Clamps a value to an inclusive minimum value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="min">The lowest possible value.</param>
		/// <returns>
		/// The clamped value.
		/// </returns>
		public static int ClampMin (this int value, int min)
		{
			if(value < min)
			{
				return min;
			}
			return value;
		}

		/// <summary>
		/// Clamps a value to an inclusive maximum value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="max">The highest possible value.</param>
		/// <returns>
		/// The clamped value.
		/// </returns>
		public static int ClampMax (this int value, int max)
		{
			if(value > max)
			{
				return max;
			}
			return value;
		}

		/// <summary>
		/// Quantizes a value to the nearest value of a multiple.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="multiple">The multiple.</param>
		/// <returns>
		/// The quantized value.
		/// </returns>
		/// <remarks>
		/// <para>
		/// <see cref="MidpointRounding.ToEven"/> is used for <see cref="MidpointRounding"/>.
		/// </para>
		/// </remarks>
		public static int Quantize (this int value, int multiple) => (int)(Math.Round((double)value / (double)multiple) * (double)multiple);

		/// <summary>
		/// Quantizes a value to the nearest value of a multiple.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="multiple">The multiple.</param>
		/// <param name="rounding">The kind of rounding to use.</param>
		/// <returns>
		/// The quantized value.
		/// </returns>
		public static int Quantize (this int value, int multiple, MidpointRounding rounding) => (int)(Math.Round((double)value / (double)multiple, rounding) * (double)multiple);

		/// <summary>
		/// Adds a number to a value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="addition">The number to add.</param>
		/// <returns>
		/// The result of: value + addition.
		/// </returns>
		public static int Add (this int value, int addition) => (int)(value + addition);

		/// <summary>
		/// Subtracts a number from a value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="subtract">The number to subtract.</param>
		/// <returns>
		/// The result of: value - subtract.
		/// </returns>
		public static int Subtract (this int value, int subtract) => (int)(value - subtract);

		/// <summary>
		/// Subtracts a value from a number.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="subtractFrom">The number to subtract from.</param>
		/// <returns>
		/// The result of: subtractFrom - value.
		/// </returns>
		public static int SubtractFrom (this int value, int subtractFrom) => (int)(subtractFrom - value);

		/// <summary>
		/// Multiplies a value by a factor.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="factor">The factor to multiply with.</param>
		/// <returns>
		/// The result of: value * factor.
		/// </returns>
		public static int Multiply (this int value, int factor) => (int)(value * factor);

		/// <summary>
		/// Clamps a value between an inclusive minimum and maximum value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="min">The lowest possible value.</param>
		/// <param name="max">The highest possible value.</param>
		/// <returns>
		/// The clamped value.
		/// </returns>
		public static uint Clamp (this uint value, uint min, uint max)
		{
			if(value < min)
			{
				return min;
			}
			if(value > max)
			{
				return max;
			}
			return value;
		}

		/// <summary>
		/// Clamps a value to an inclusive minimum value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="min">The lowest possible value.</param>
		/// <returns>
		/// The clamped value.
		/// </returns>
		public static uint ClampMin (this uint value, uint min)
		{
			if(value < min)
			{
				return min;
			}
			return value;
		}

		/// <summary>
		/// Clamps a value to an inclusive maximum value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="max">The highest possible value.</param>
		/// <returns>
		/// The clamped value.
		/// </returns>
		public static uint ClampMax (this uint value, uint max)
		{
			if(value > max)
			{
				return max;
			}
			return value;
		}

		/// <summary>
		/// Quantizes a value to the nearest value of a multiple.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="multiple">The multiple.</param>
		/// <returns>
		/// The quantized value.
		/// </returns>
		/// <remarks>
		/// <para>
		/// <see cref="MidpointRounding.ToEven"/> is used for <see cref="MidpointRounding"/>.
		/// </para>
		/// </remarks>
		public static uint Quantize (this uint value, uint multiple) => (uint)(Math.Round((double)value / (double)multiple) * (double)multiple);

		/// <summary>
		/// Quantizes a value to the nearest value of a multiple.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="multiple">The multiple.</param>
		/// <param name="rounding">The kind of rounding to use.</param>
		/// <returns>
		/// The quantized value.
		/// </returns>
		public static uint Quantize (this uint value, uint multiple, MidpointRounding rounding) => (uint)(Math.Round((double)value / (double)multiple, rounding) * (double)multiple);

		/// <summary>
		/// Adds a number to a value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="addition">The number to add.</param>
		/// <returns>
		/// The result of: value + addition.
		/// </returns>
		public static uint Add (this uint value, uint addition) => (uint)(value + addition);

		/// <summary>
		/// Subtracts a number from a value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="subtract">The number to subtract.</param>
		/// <returns>
		/// The result of: value - subtract.
		/// </returns>
		public static uint Subtract (this uint value, uint subtract) => (uint)(value - subtract);

		/// <summary>
		/// Subtracts a value from a number.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="subtractFrom">The number to subtract from.</param>
		/// <returns>
		/// The result of: subtractFrom - value.
		/// </returns>
		public static uint SubtractFrom (this uint value, uint subtractFrom) => (uint)(subtractFrom - value);

		/// <summary>
		/// Multiplies a value by a factor.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="factor">The factor to multiply with.</param>
		/// <returns>
		/// The result of: value * factor.
		/// </returns>
		public static uint Multiply (this uint value, uint factor) => (uint)(value * factor);

		/// <summary>
		/// Clamps a value between an inclusive minimum and maximum value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="min">The lowest possible value.</param>
		/// <param name="max">The highest possible value.</param>
		/// <returns>
		/// The clamped value.
		/// </returns>
		public static long Clamp (this long value, long min, long max)
		{
			if(value < min)
			{
				return min;
			}
			if(value > max)
			{
				return max;
			}
			return value;
		}

		/// <summary>
		/// Clamps a value to an inclusive minimum value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="min">The lowest possible value.</param>
		/// <returns>
		/// The clamped value.
		/// </returns>
		public static long ClampMin (this long value, long min)
		{
			if(value < min)
			{
				return min;
			}
			return value;
		}

		/// <summary>
		/// Clamps a value to an inclusive maximum value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="max">The highest possible value.</param>
		/// <returns>
		/// The clamped value.
		/// </returns>
		public static long ClampMax (this long value, long max)
		{
			if(value > max)
			{
				return max;
			}
			return value;
		}

		/// <summary>
		/// Quantizes a value to the nearest value of a multiple.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="multiple">The multiple.</param>
		/// <returns>
		/// The quantized value.
		/// </returns>
		/// <remarks>
		/// <para>
		/// <see cref="MidpointRounding.ToEven"/> is used for <see cref="MidpointRounding"/>.
		/// </para>
		/// </remarks>
		public static long Quantize (this long value, long multiple) => (long)(Math.Round((double)value / (double)multiple) * (double)multiple);

		/// <summary>
		/// Quantizes a value to the nearest value of a multiple.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="multiple">The multiple.</param>
		/// <param name="rounding">The kind of rounding to use.</param>
		/// <returns>
		/// The quantized value.
		/// </returns>
		public static long Quantize (this long value, long multiple, MidpointRounding rounding) => (long)(Math.Round((double)value / (double)multiple, rounding) * (double)multiple);

		/// <summary>
		/// Adds a number to a value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="addition">The number to add.</param>
		/// <returns>
		/// The result of: value + addition.
		/// </returns>
		public static long Add (this long value, long addition) => (long)(value + addition);

		/// <summary>
		/// Subtracts a number from a value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="subtract">The number to subtract.</param>
		/// <returns>
		/// The result of: value - subtract.
		/// </returns>
		public static long Subtract (this long value, long subtract) => (long)(value - subtract);

		/// <summary>
		/// Subtracts a value from a number.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="subtractFrom">The number to subtract from.</param>
		/// <returns>
		/// The result of: subtractFrom - value.
		/// </returns>
		public static long SubtractFrom (this long value, long subtractFrom) => (long)(subtractFrom - value);

		/// <summary>
		/// Multiplies a value by a factor.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="factor">The factor to multiply with.</param>
		/// <returns>
		/// The result of: value * factor.
		/// </returns>
		public static long Multiply (this long value, long factor) => (long)(value * factor);

		/// <summary>
		/// Clamps a value between an inclusive minimum and maximum value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="min">The lowest possible value.</param>
		/// <param name="max">The highest possible value.</param>
		/// <returns>
		/// The clamped value.
		/// </returns>
		public static ulong Clamp (this ulong value, ulong min, ulong max)
		{
			if(value < min)
			{
				return min;
			}
			if(value > max)
			{
				return max;
			}
			return value;
		}

		/// <summary>
		/// Clamps a value to an inclusive minimum value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="min">The lowest possible value.</param>
		/// <returns>
		/// The clamped value.
		/// </returns>
		public static ulong ClampMin (this ulong value, ulong min)
		{
			if(value < min)
			{
				return min;
			}
			return value;
		}

		/// <summary>
		/// Clamps a value to an inclusive maximum value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="max">The highest possible value.</param>
		/// <returns>
		/// The clamped value.
		/// </returns>
		public static ulong ClampMax (this ulong value, ulong max)
		{
			if(value > max)
			{
				return max;
			}
			return value;
		}

		/// <summary>
		/// Quantizes a value to the nearest value of a multiple.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="multiple">The multiple.</param>
		/// <returns>
		/// The quantized value.
		/// </returns>
		/// <remarks>
		/// <para>
		/// <see cref="MidpointRounding.ToEven"/> is used for <see cref="MidpointRounding"/>.
		/// </para>
		/// </remarks>
		public static ulong Quantize (this ulong value, ulong multiple) => (ulong)(Math.Round((double)value / (double)multiple) * (double)multiple);

		/// <summary>
		/// Quantizes a value to the nearest value of a multiple.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="multiple">The multiple.</param>
		/// <param name="rounding">The kind of rounding to use.</param>
		/// <returns>
		/// The quantized value.
		/// </returns>
		public static ulong Quantize (this ulong value, ulong multiple, MidpointRounding rounding) => (ulong)(Math.Round((double)value / (double)multiple, rounding) * (double)multiple);

		/// <summary>
		/// Adds a number to a value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="addition">The number to add.</param>
		/// <returns>
		/// The result of: value + addition.
		/// </returns>
		public static ulong Add (this ulong value, ulong addition) => (ulong)(value + addition);

		/// <summary>
		/// Subtracts a number from a value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="subtract">The number to subtract.</param>
		/// <returns>
		/// The result of: value - subtract.
		/// </returns>
		public static ulong Subtract (this ulong value, ulong subtract) => (ulong)(value - subtract);

		/// <summary>
		/// Subtracts a value from a number.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="subtractFrom">The number to subtract from.</param>
		/// <returns>
		/// The result of: subtractFrom - value.
		/// </returns>
		public static ulong SubtractFrom (this ulong value, ulong subtractFrom) => (ulong)(subtractFrom - value);

		/// <summary>
		/// Multiplies a value by a factor.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="factor">The factor to multiply with.</param>
		/// <returns>
		/// The result of: value * factor.
		/// </returns>
		public static ulong Multiply (this ulong value, ulong factor) => (ulong)(value * factor);

		/// <summary>
		/// Clamps a value between an inclusive minimum and maximum value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="min">The lowest possible value.</param>
		/// <param name="max">The highest possible value.</param>
		/// <returns>
		/// The clamped value.
		/// </returns>
		public static float Clamp (this float value, float min, float max)
		{
			if(value < min)
			{
				return min;
			}
			if(value > max)
			{
				return max;
			}
			return value;
		}

		/// <summary>
		/// Clamps a value to an inclusive minimum value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="min">The lowest possible value.</param>
		/// <returns>
		/// The clamped value.
		/// </returns>
		public static float ClampMin (this float value, float min)
		{
			if(value < min)
			{
				return min;
			}
			return value;
		}

		/// <summary>
		/// Clamps a value to an inclusive maximum value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="max">The highest possible value.</param>
		/// <returns>
		/// The clamped value.
		/// </returns>
		public static float ClampMax (this float value, float max)
		{
			if(value > max)
			{
				return max;
			}
			return value;
		}

		/// <summary>
		/// Quantizes a value to the nearest value of a multiple.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="multiple">The multiple.</param>
		/// <returns>
		/// The quantized value.
		/// </returns>
		/// <remarks>
		/// <para>
		/// <see cref="MidpointRounding.ToEven"/> is used for <see cref="MidpointRounding"/>.
		/// </para>
		/// </remarks>
		public static float Quantize (this float value, float multiple) => (float)(Math.Round((double)value / (double)multiple) * (double)multiple);

		/// <summary>
		/// Quantizes a value to the nearest value of a multiple.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="multiple">The multiple.</param>
		/// <param name="rounding">The kind of rounding to use.</param>
		/// <returns>
		/// The quantized value.
		/// </returns>
		public static float Quantize (this float value, float multiple, MidpointRounding rounding) => (float)(Math.Round((double)value / (double)multiple, rounding) * (double)multiple);

		/// <summary>
		/// Adds a number to a value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="addition">The number to add.</param>
		/// <returns>
		/// The result of: value + addition.
		/// </returns>
		public static float Add (this float value, float addition) => (float)(value + addition);

		/// <summary>
		/// Subtracts a number from a value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="subtract">The number to subtract.</param>
		/// <returns>
		/// The result of: value - subtract.
		/// </returns>
		public static float Subtract (this float value, float subtract) => (float)(value - subtract);

		/// <summary>
		/// Subtracts a value from a number.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="subtractFrom">The number to subtract from.</param>
		/// <returns>
		/// The result of: subtractFrom - value.
		/// </returns>
		public static float SubtractFrom (this float value, float subtractFrom) => (float)(subtractFrom - value);

		/// <summary>
		/// Multiplies a value by a factor.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="factor">The factor to multiply with.</param>
		/// <returns>
		/// The result of: value * factor.
		/// </returns>
		public static float Multiply (this float value, float factor) => (float)(value * factor);

		/// <summary>
		/// Clamps a value between an inclusive minimum and maximum value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="min">The lowest possible value.</param>
		/// <param name="max">The highest possible value.</param>
		/// <returns>
		/// The clamped value.
		/// </returns>
		public static double Clamp (this double value, double min, double max)
		{
			if(value < min)
			{
				return min;
			}
			if(value > max)
			{
				return max;
			}
			return value;
		}

		/// <summary>
		/// Clamps a value to an inclusive minimum value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="min">The lowest possible value.</param>
		/// <returns>
		/// The clamped value.
		/// </returns>
		public static double ClampMin (this double value, double min)
		{
			if(value < min)
			{
				return min;
			}
			return value;
		}

		/// <summary>
		/// Clamps a value to an inclusive maximum value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="max">The highest possible value.</param>
		/// <returns>
		/// The clamped value.
		/// </returns>
		public static double ClampMax (this double value, double max)
		{
			if(value > max)
			{
				return max;
			}
			return value;
		}

		/// <summary>
		/// Quantizes a value to the nearest value of a multiple.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="multiple">The multiple.</param>
		/// <returns>
		/// The quantized value.
		/// </returns>
		/// <remarks>
		/// <para>
		/// <see cref="MidpointRounding.ToEven"/> is used for <see cref="MidpointRounding"/>.
		/// </para>
		/// </remarks>
		public static double Quantize (this double value, double multiple) => (double)(Math.Round((double)value / (double)multiple) * (double)multiple);

		/// <summary>
		/// Quantizes a value to the nearest value of a multiple.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="multiple">The multiple.</param>
		/// <param name="rounding">The kind of rounding to use.</param>
		/// <returns>
		/// The quantized value.
		/// </returns>
		public static double Quantize (this double value, double multiple, MidpointRounding rounding) => (double)(Math.Round((double)value / (double)multiple, rounding) * (double)multiple);

		/// <summary>
		/// Adds a number to a value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="addition">The number to add.</param>
		/// <returns>
		/// The result of: value + addition.
		/// </returns>
		public static double Add (this double value, double addition) => (double)(value + addition);

		/// <summary>
		/// Subtracts a number from a value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="subtract">The number to subtract.</param>
		/// <returns>
		/// The result of: value - subtract.
		/// </returns>
		public static double Subtract (this double value, double subtract) => (double)(value - subtract);

		/// <summary>
		/// Subtracts a value from a number.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="subtractFrom">The number to subtract from.</param>
		/// <returns>
		/// The result of: subtractFrom - value.
		/// </returns>
		public static double SubtractFrom (this double value, double subtractFrom) => (double)(subtractFrom - value);

		/// <summary>
		/// Multiplies a value by a factor.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="factor">The factor to multiply with.</param>
		/// <returns>
		/// The result of: value * factor.
		/// </returns>
		public static double Multiply (this double value, double factor) => (double)(value * factor);

		/// <summary>
		/// Clamps a value between an inclusive minimum and maximum value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="min">The lowest possible value.</param>
		/// <param name="max">The highest possible value.</param>
		/// <returns>
		/// The clamped value.
		/// </returns>
		public static decimal Clamp (this decimal value, decimal min, decimal max)
		{
			if(value < min)
			{
				return min;
			}
			if(value > max)
			{
				return max;
			}
			return value;
		}

		/// <summary>
		/// Clamps a value to an inclusive minimum value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="min">The lowest possible value.</param>
		/// <returns>
		/// The clamped value.
		/// </returns>
		public static decimal ClampMin (this decimal value, decimal min)
		{
			if(value < min)
			{
				return min;
			}
			return value;
		}

		/// <summary>
		/// Clamps a value to an inclusive maximum value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="max">The highest possible value.</param>
		/// <returns>
		/// The clamped value.
		/// </returns>
		public static decimal ClampMax (this decimal value, decimal max)
		{
			if(value > max)
			{
				return max;
			}
			return value;
		}

		/// <summary>
		/// Quantizes a value to the nearest value of a multiple.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="multiple">The multiple.</param>
		/// <returns>
		/// The quantized value.
		/// </returns>
		/// <remarks>
		/// <para>
		/// <see cref="MidpointRounding.ToEven"/> is used for <see cref="MidpointRounding"/>.
		/// </para>
		/// </remarks>
		public static decimal Quantize (this decimal value, decimal multiple) => (decimal)(Math.Round((double)value / (double)multiple) * (double)multiple);

		/// <summary>
		/// Quantizes a value to the nearest value of a multiple.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="multiple">The multiple.</param>
		/// <param name="rounding">The kind of rounding to use.</param>
		/// <returns>
		/// The quantized value.
		/// </returns>
		public static decimal Quantize (this decimal value, decimal multiple, MidpointRounding rounding) => (decimal)(Math.Round((double)value / (double)multiple, rounding) * (double)multiple);

		/// <summary>
		/// Adds a number to a value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="addition">The number to add.</param>
		/// <returns>
		/// The result of: value + addition.
		/// </returns>
		public static decimal Add (this decimal value, decimal addition) => (decimal)(value + addition);

		/// <summary>
		/// Subtracts a number from a value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="subtract">The number to subtract.</param>
		/// <returns>
		/// The result of: value - subtract.
		/// </returns>
		public static decimal Subtract (this decimal value, decimal subtract) => (decimal)(value - subtract);

		/// <summary>
		/// Subtracts a value from a number.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="subtractFrom">The number to subtract from.</param>
		/// <returns>
		/// The result of: subtractFrom - value.
		/// </returns>
		public static decimal SubtractFrom (this decimal value, decimal subtractFrom) => (decimal)(subtractFrom - value);

		/// <summary>
		/// Multiplies a value by a factor.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="factor">The factor to multiply with.</param>
		/// <returns>
		/// The result of: value * factor.
		/// </returns>
		public static decimal Multiply (this decimal value, decimal factor) => (decimal)(value * factor);

		/// <summary>
		/// Gets the absolute number of a value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The absolute number.
		/// </returns>
		public static sbyte Abs (this sbyte value) => value < 0 ? (sbyte)(-1 * value) : value;

		/// <summary>
		/// Gets the sign of a value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// 1 if the number is positive, 0 if the number is zero, -1 if the number is negative.
		/// </returns>
		[SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
		public static sbyte Sign (this sbyte value) => (sbyte)((value == 0) ? 0 : (value < 0 ? -1 : 1));

		/// <summary>
		/// Calculates the magnitude of a value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The magnitude.
		/// </returns>
		/// <remarks>
		/// <para>
		/// The magnitude expresses the order of magnitude of a number.
		/// Simply said, it calculates the number of significant digits on the left side of the decimal point.
		/// </para>
		/// <para>
		/// If <paramref name="value"/> is positive, the result is positive.
		/// If <paramref name="value"/> is negative, the result is negative.
		/// If <paramref name="value"/> is zero, the result is zero.
		/// </para>
		/// <para>
		/// Examples: 0 -> 0; 1 -> 1; -1 -> -1; 5 -> 1; -5 -> -1; 10 -> 2; 100 -> 3; 1234 -> 4; -1234 -> -4; 9999 -> 4; 10000 -> 5; 10001 -> 5
		/// </para>
		/// </remarks>
		[SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
		public static sbyte Magnitude (this sbyte value)
		{
			if(value == 0)
			{
				return 0;
			}

			return (sbyte)(Math.Floor(Math.Log10((double)Math.Abs(value)) + 1.0) * (double)Math.Sign(value));
		}

		/// <summary>
		/// Gets the absolute number of a value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The absolute number.
		/// </returns>
		public static short Abs (this short value) => value < 0 ? (short)(-1 * value) : value;

		/// <summary>
		/// Gets the sign of a value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// 1 if the number is positive, 0 if the number is zero, -1 if the number is negative.
		/// </returns>
		[SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
		public static short Sign (this short value) => (short)((value == 0) ? 0 : (value < 0 ? -1 : 1));

		/// <summary>
		/// Calculates the magnitude of a value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The magnitude.
		/// </returns>
		/// <remarks>
		/// <para>
		/// The magnitude expresses the order of magnitude of a number.
		/// Simply said, it calculates the number of significant digits on the left side of the decimal point.
		/// </para>
		/// <para>
		/// If <paramref name="value"/> is positive, the result is positive.
		/// If <paramref name="value"/> is negative, the result is negative.
		/// If <paramref name="value"/> is zero, the result is zero.
		/// </para>
		/// <para>
		/// Examples: 0 -> 0; 1 -> 1; -1 -> -1; 5 -> 1; -5 -> -1; 10 -> 2; 100 -> 3; 1234 -> 4; -1234 -> -4; 9999 -> 4; 10000 -> 5; 10001 -> 5
		/// </para>
		/// </remarks>
		[SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
		public static short Magnitude (this short value)
		{
			if(value == 0)
			{
				return 0;
			}

			return (short)(Math.Floor(Math.Log10((double)Math.Abs(value)) + 1.0) * (double)Math.Sign(value));
		}

		/// <summary>
		/// Gets the absolute number of a value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The absolute number.
		/// </returns>
		public static int Abs (this int value) => value < 0 ? (int)(-1 * value) : value;

		/// <summary>
		/// Gets the sign of a value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// 1 if the number is positive, 0 if the number is zero, -1 if the number is negative.
		/// </returns>
		[SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
		public static int Sign (this int value) => (int)((value == 0) ? 0 : (value < 0 ? -1 : 1));

		/// <summary>
		/// Calculates the magnitude of a value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The magnitude.
		/// </returns>
		/// <remarks>
		/// <para>
		/// The magnitude expresses the order of magnitude of a number.
		/// Simply said, it calculates the number of significant digits on the left side of the decimal point.
		/// </para>
		/// <para>
		/// If <paramref name="value"/> is positive, the result is positive.
		/// If <paramref name="value"/> is negative, the result is negative.
		/// If <paramref name="value"/> is zero, the result is zero.
		/// </para>
		/// <para>
		/// Examples: 0 -> 0; 1 -> 1; -1 -> -1; 5 -> 1; -5 -> -1; 10 -> 2; 100 -> 3; 1234 -> 4; -1234 -> -4; 9999 -> 4; 10000 -> 5; 10001 -> 5
		/// </para>
		/// </remarks>
		[SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
		public static int Magnitude (this int value)
		{
			if(value == 0)
			{
				return 0;
			}

			return (int)(Math.Floor(Math.Log10((double)Math.Abs(value)) + 1.0) * (double)Math.Sign(value));
		}

		/// <summary>
		/// Gets the absolute number of a value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The absolute number.
		/// </returns>
		public static long Abs (this long value) => value < 0 ? (long)(-1 * value) : value;

		/// <summary>
		/// Gets the sign of a value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// 1 if the number is positive, 0 if the number is zero, -1 if the number is negative.
		/// </returns>
		[SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
		public static long Sign (this long value) => (long)((value == 0) ? 0 : (value < 0 ? -1 : 1));

		/// <summary>
		/// Calculates the magnitude of a value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The magnitude.
		/// </returns>
		/// <remarks>
		/// <para>
		/// The magnitude expresses the order of magnitude of a number.
		/// Simply said, it calculates the number of significant digits on the left side of the decimal point.
		/// </para>
		/// <para>
		/// If <paramref name="value"/> is positive, the result is positive.
		/// If <paramref name="value"/> is negative, the result is negative.
		/// If <paramref name="value"/> is zero, the result is zero.
		/// </para>
		/// <para>
		/// Examples: 0 -> 0; 1 -> 1; -1 -> -1; 5 -> 1; -5 -> -1; 10 -> 2; 100 -> 3; 1234 -> 4; -1234 -> -4; 9999 -> 4; 10000 -> 5; 10001 -> 5
		/// </para>
		/// </remarks>
		[SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
		public static long Magnitude (this long value)
		{
			if(value == 0)
			{
				return 0;
			}

			return (long)(Math.Floor(Math.Log10((double)Math.Abs(value)) + 1.0) * (double)Math.Sign(value));
		}

		/// <summary>
		/// Gets the absolute number of a value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The absolute number.
		/// </returns>
		public static float Abs (this float value) => value < 0.0f ? (float)(-1.0f * value) : value;

		/// <summary>
		/// Gets the sign of a value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// 1 if the number is positive, 0 if the number is zero, -1 if the number is negative.
		/// </returns>
		[SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
		public static float Sign (this float value) => (float)((value == 0.0f) ? 0.0f : (value < 0.0f ? -1.0f : 1.0f));

		/// <summary>
		/// Calculates the magnitude of a value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The magnitude.
		/// </returns>
		/// <remarks>
		/// <para>
		/// The magnitude expresses the order of magnitude of a number.
		/// Simply said, it calculates the number of significant digits on the left side of the decimal point.
		/// </para>
		/// <para>
		/// If <paramref name="value"/> is positive, the result is positive.
		/// If <paramref name="value"/> is negative, the result is negative.
		/// If <paramref name="value"/> is zero, the result is zero.
		/// </para>
		/// <para>
		/// Examples: 0 -> 0; 1 -> 1; -1 -> -1; 5 -> 1; -5 -> -1; 10 -> 2; 100 -> 3; 1234 -> 4; -1234 -> -4; 9999 -> 4; 10000 -> 5; 10001 -> 5
		/// </para>
		/// </remarks>
		[SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
		public static float Magnitude (this float value)
		{
			if(value == 0.0f)
			{
				return 0.0f;
			}

			return (float)(Math.Floor(Math.Log10((double)Math.Abs(value)) + 1.0) * (double)Math.Sign(value));
		}

		/// <summary>
		/// Gets the absolute number of a value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The absolute number.
		/// </returns>
		public static double Abs (this double value) => value < 0.0 ? (double)(-1.0 * value) : value;

		/// <summary>
		/// Gets the sign of a value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// 1 if the number is positive, 0 if the number is zero, -1 if the number is negative.
		/// </returns>
		[SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
		public static double Sign (this double value) => (double)((value == 0.0) ? 0.0 : (value < 0.0 ? -1.0 : 1.0));

		/// <summary>
		/// Calculates the magnitude of a value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The magnitude.
		/// </returns>
		/// <remarks>
		/// <para>
		/// The magnitude expresses the order of magnitude of a number.
		/// Simply said, it calculates the number of significant digits on the left side of the decimal point.
		/// </para>
		/// <para>
		/// If <paramref name="value"/> is positive, the result is positive.
		/// If <paramref name="value"/> is negative, the result is negative.
		/// If <paramref name="value"/> is zero, the result is zero.
		/// </para>
		/// <para>
		/// Examples: 0 -> 0; 1 -> 1; -1 -> -1; 5 -> 1; -5 -> -1; 10 -> 2; 100 -> 3; 1234 -> 4; -1234 -> -4; 9999 -> 4; 10000 -> 5; 10001 -> 5
		/// </para>
		/// </remarks>
		[SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
		public static double Magnitude (this double value)
		{
			if(value == 0.0)
			{
				return 0.0;
			}

			return (double)(Math.Floor(Math.Log10((double)Math.Abs(value)) + 1.0) * (double)Math.Sign(value));
		}

		/// <summary>
		/// Gets the absolute number of a value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The absolute number.
		/// </returns>
		public static decimal Abs (this decimal value) => value < 0.0m ? (decimal)(-1.0m * value) : value;

		/// <summary>
		/// Gets the sign of a value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// 1 if the number is positive, 0 if the number is zero, -1 if the number is negative.
		/// </returns>
		[SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
		public static decimal Sign (this decimal value) => (decimal)((value == 0.0m) ? 0.0m : (value < 0.0m ? -1.0m : 1.0m));

		/// <summary>
		/// Calculates the magnitude of a value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The magnitude.
		/// </returns>
		/// <remarks>
		/// <para>
		/// The magnitude expresses the order of magnitude of a number.
		/// Simply said, it calculates the number of significant digits on the left side of the decimal point.
		/// </para>
		/// <para>
		/// If <paramref name="value"/> is positive, the result is positive.
		/// If <paramref name="value"/> is negative, the result is negative.
		/// If <paramref name="value"/> is zero, the result is zero.
		/// </para>
		/// <para>
		/// Examples: 0 -> 0; 1 -> 1; -1 -> -1; 5 -> 1; -5 -> -1; 10 -> 2; 100 -> 3; 1234 -> 4; -1234 -> -4; 9999 -> 4; 10000 -> 5; 10001 -> 5
		/// </para>
		/// </remarks>
		[SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
		public static decimal Magnitude (this decimal value)
		{
			if(value == 0.0m)
			{
				return 0.0m;
			}

			return (decimal)(Math.Floor(Math.Log10((double)Math.Abs(value)) + 1.0) * (double)Math.Sign(value));
		}

		/// <summary>
		/// Gets the remainder of a division.
		/// </summary>
		/// <param name="dividend">The dividend.</param>
		/// <param name="divisor">The divisor.</param>
		/// <returns>
		/// The remainder of  dividend / divisor.
		/// </returns>
		/// <remarks>
		/// <note type="important">
		/// This is not the same as simply applying the module operator.
		/// The remainder is calculated, allowing the operation for floating point values.
		/// </note>
		/// </remarks>
		public static float DivRem (this float dividend, float divisor)
		{
			float dividendAbs = dividend.Abs();
			float divisorAbs = divisor.Abs();
			float sign = dividend.Sign();
			return (dividendAbs - (divisorAbs * ((float)Math.Floor(dividendAbs / divisorAbs)))) * sign;
		}

		/// <summary>
		/// Determines whether a value is almost equal to another value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="other">The other value to compare with.</param>
		/// <returns>
		/// true if the value is within the default accuracy for equality, false otherwise.
		/// </returns>
		public static bool AlmostEqual (this float value, float other) => value.AlmostEqual(other, 5.96046447753906E-07f);

		/// <summary>
		/// Determines whether a value is almost equal to another value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="other">The other value to compare with.</param>
		/// <param name="accuracy">The accuracy within the two values are considered equal.</param>
		/// <returns>
		/// true if the value is within the specified accuracy for equality, false otherwise.
		/// </returns>
		public static bool AlmostEqual (this float value, float other, float accuracy) => Math.Abs(value - other) < (double)accuracy;

		/// <summary>
		/// Determines whether a value is almost zero.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// true if the value is within the default accuracy to zero, false otherwise.
		/// </returns>
		public static bool AlmostZero (this float value) => Math.Abs(value) < (double)5.96046447753906E-07f;

		/// <summary>
		/// Determines whether a value is almost zero.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="accuracy">The accuracy within the value is considered zero.</param>
		/// <returns>
		/// true if the value is within the specified accuracy to zero, false otherwise.
		/// </returns>
		public static bool AlmostZero (this float value, float accuracy) => Math.Abs(value) < (double)accuracy;

		/// <summary>
		/// Gets the smallest integer that is greater than or equal to a value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The smallest integer.
		/// </returns>
		public static float Ceiling (this float value) => (float)Math.Ceiling(value);

		/// <summary>
		/// Gets the largest integer less than or equal to a value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The largest integer.
		/// </returns>
		public static float Floor (this float value) => (float)Math.Floor(value);

		/// <summary>
		/// Gets the integer part of a value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The integer part.
		/// </returns>
		public static float Integer (this float value) => (float)Math.Truncate(value);

		/// <summary>
		/// Gets the fraction part of a value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The fraction part.
		/// </returns>
		public static float Fraction (this float value) => (float)(value - Math.Truncate(value));

		/// <summary>
		/// Rounds a value to the nearest integer.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The rounded value.
		/// </returns>
		/// <remarks>
		/// <para>
		/// <see cref="MidpointRounding.ToEven"/> is used for <see cref="MidpointRounding"/>.
		/// </para>
		/// </remarks>
		public static float RoundInteger (this float value) => (float)Math.Round(value);

		/// <summary>
		/// Rounds a value to the nearest integer.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="rounding">The kind of rounding to use.</param>
		/// <returns>
		/// The rounded value.
		/// </returns>
		public static float RoundInteger (this float value, MidpointRounding rounding) => (float)Math.Round(value, rounding);

		/// <summary>
		/// Rounds a value to a specified amount of fractional digits.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="digits">The number of digits.</param>
		/// <returns>
		/// The rounded value.
		/// </returns>
		/// <remarks>
		/// <para>
		/// <see cref="MidpointRounding.ToEven"/> is used for <see cref="MidpointRounding"/>.
		/// </para>
		/// </remarks>
		public static float RoundDigits (this float value, int digits) => (float)Math.Round(value, digits);

		/// <summary>
		/// Rounds a value to a specified amount of fractional digits.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="digits">The number of digits.</param>
		/// <param name="rounding">The kind of rounding to use.</param>
		/// <returns>
		/// The rounded value.
		/// </returns>
		public static float RoundDigits (this float value, int digits, MidpointRounding rounding) => (float)Math.Round(value, digits, rounding);

		/// <summary>
		/// Calculates value^power.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="power">The power.</param>
		/// <returns>
		/// The result.
		/// </returns>
		public static float Pow (this float value, double power) => (float)Math.Pow((double)value, power);

		/// <summary>
		/// Calculates value^2.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The result.
		/// </returns>
		public static float Pow2 (this float value) => value * value;

		/// <summary>
		/// Calculates value^3.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The result.
		/// </returns>
		public static float Pow3 (this float value) => value * value * value;

		/// <summary>
		/// Calculates value^10.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The result.
		/// </returns>
		public static float Pow10 (this float value) => (float)Math.Pow((double)value, 10.0);

		/// <summary>
		/// Calculates value^e.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The result.
		/// </returns>
		public static float PowE (this float value) => (float)Math.Pow((double)value, Math.E);

		/// <summary>
		/// Calculates expBase^value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="expBase">The base.</param>
		/// <returns>
		/// The result.
		/// </returns>
		public static float Exp (this float value, double expBase) => (float)Math.Pow(expBase, (double)value);

		/// <summary>
		/// Calculates 2^value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The result.
		/// </returns>
		public static float Exp2 (this float value) => (float)Math.Pow(2.0, (double)value);

		/// <summary>
		/// Calculates 3^value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The result.
		/// </returns>
		public static float Exp3 (this float value) => (float)Math.Pow(3.0, (double)value);

		/// <summary>
		/// Calculates 10^value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The result.
		/// </returns>
		public static float Exp10 (this float value) => (float)Math.Pow(10.0, (double)value);

		/// <summary>
		/// Calculates e^value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The result.
		/// </returns>
		public static float ExpE (this float value) => (float)Math.Exp((double)value);

		/// <summary>
		/// Calculates log[logBase](value).
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="logBase">The base.</param>
		/// <returns>
		/// The result.
		/// </returns>
		public static float Log (this float value, double logBase) => (float)Math.Log((double)value, logBase);

		/// <summary>
		/// Calculates log[2](value).
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The result.
		/// </returns>
		public static float Log2 (this float value) => (float)Math.Log((double)value, 2.0);

		/// <summary>
		/// Calculates log[3](value).
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The result.
		/// </returns>
		public static float Log3 (this float value) => (float)Math.Log((double)value, 3.0);

		/// <summary>
		/// Calculates log[10](value).
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The result.
		/// </returns>
		public static float Log10 (this float value) => (float)Math.Log10((double)value);

		/// <summary>
		/// Calculates log[e](value).
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The result.
		/// </returns>
		public static float LogE (this float value) => (float)Math.Log((double)value);

		/// <summary>
		/// Calculates the square-root of a value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The square-root.
		/// </returns>
		public static float Sqrt (this float value) => (float)Math.Sqrt((double)value);

		/// <summary>
		/// Calculates the n-th root of a value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="n">The root.</param>
		/// <returns>
		/// The n-th root.
		/// </returns>
		public static float Root (this float value, double n) => (float)Math.Pow((double)value, 1.0 / n);

		/// <summary>
		/// Calculates the Sine of a value in radians.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The result.
		/// </returns>
		public static float Sin (this float value) => (float)Math.Sin(value);

		/// <summary>
		/// Calculates the Cosine of a value in radians.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The result.
		/// </returns>
		public static float Cos (this float value) => (float)Math.Cos(value);

		/// <summary>
		/// Calculates the Tangent of a value in radians.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The result.
		/// </returns>
		public static float Tan (this float value) => (float)Math.Tan(value);

		/// <summary>
		/// Calculates the Cotangent of a value in radians.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The result.
		/// </returns>
		public static float Cot (this float value) => 1.0f / (float)Math.Tan(value);

		/// <summary>
		/// Calculates the Arc Sine of a value in radians.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The result.
		/// </returns>
		public static float Asin (this float value) => (float)Math.Asin(value);

		/// <summary>
		/// Calculates the Arc Cosine of a value in radians.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The result.
		/// </returns>
		public static float Acos (this float value) => (float)Math.Acos(value);

		/// <summary>
		/// Calculates the Arc Tangent of a value in radians.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The result.
		/// </returns>
		public static float Atan (this float value) => (float)Math.Atan(value);

		/// <summary>
		/// Calculates the Arc Cotangent of a value in radians.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The result.
		/// </returns>
		public static float Acot (this float value) => (float)Math.Tan(1.0f / value);

		/// <summary>
		/// Calculates the Hyperbolic Sine of a value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The result.
		/// </returns>
		public static float Sinh (this float value) => (float)Math.Sinh(value);

		/// <summary>
		/// Calculates the Hyperbolic Cosine of a value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The result.
		/// </returns>
		public static float Cosh (this float value) => (float)Math.Cosh(value);

		/// <summary>
		/// Calculates the Hyperbolic Tangent of a value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The result.
		/// </returns>
		public static float Tanh (this float value) => (float)Math.Tanh(value);

		/// <summary>
		/// Calculates the Hyperbolic Cotangent of a value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The result.
		/// </returns>
		public static float Coth (this float value)
		{
			double e1 = Math.Exp(value);
			double e2 = Math.Exp(-value);
			return (float)((e1 + e2) / (e1 - e2));
		}

		/// <summary>
		/// Converts a radian value to degrees.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The result.
		/// </returns>
		/// <remarks>
		/// <para>
		/// The result is not clamped to a single full circle value (-359...0...+359).
		/// Use <see cref="CircularClampDeg(float)" /> to clamp to a single full circle value.
		/// </para>
		/// </remarks>
		public static float ToDeg (this float value) => value * 57.295779513f;

		/// <summary>
		/// Converts a degree value to radians.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The result.
		/// </returns>
		/// <remarks>
		/// <para>
		/// The result is not clamped to a single full circle value (-2π...0...+2π).
		/// Use <see cref="CircularClampRad(float)" /> to clamp to a single full circle value.
		/// </para>
		/// </remarks>
		public static float ToRad (this float value) => value * 0.01745329252f;

		/// <summary>
		/// Clamps a degree value to a single full circle (-359...0...+359).
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The result.
		/// </returns>
		/// <remarks>
		/// <para>
		/// Examples: 0 -> 0; 350 -> 350; 360 -> 0; 370 -> 10; etc.
		/// </para>
		/// </remarks>
		public static float CircularClampDeg (this float value) => value.DivRem(360.0f);

		/// <summary>
		/// Clamps a radian value to a single full circle (-2π...0...+2π).
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The result.
		/// </returns>
		/// <remarks>
		/// <para>
		/// Examples: 0 -> 0; 1.5 -> 1.5; 2π -> 0; 3π -> π; etc.
		/// </para>
		/// </remarks>
		public static float CircularClampRad (this float value) => value.DivRem(2.0f*3.1415926535897932384626433832795028841971693993751f);

		/// <summary>
		/// Gets the remainder of a division.
		/// </summary>
		/// <param name="dividend">The dividend.</param>
		/// <param name="divisor">The divisor.</param>
		/// <returns>
		/// The remainder of  dividend / divisor.
		/// </returns>
		/// <remarks>
		/// <note type="important">
		/// This is not the same as simply applying the module operator.
		/// The remainder is calculated, allowing the operation for floating point values.
		/// </note>
		/// </remarks>
		public static double DivRem (this double dividend, double divisor)
		{
			double dividendAbs = dividend.Abs();
			double divisorAbs = divisor.Abs();
			double sign = dividend.Sign();
			return (dividendAbs - (divisorAbs * ((double)Math.Floor(dividendAbs / divisorAbs)))) * sign;
		}

		/// <summary>
		/// Determines whether a value is almost equal to another value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="other">The other value to compare with.</param>
		/// <returns>
		/// true if the value is within the default accuracy for equality, false otherwise.
		/// </returns>
		public static bool AlmostEqual (this double value, double other) => value.AlmostEqual(other, 1.11022302462516E-15);

		/// <summary>
		/// Determines whether a value is almost equal to another value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="other">The other value to compare with.</param>
		/// <param name="accuracy">The accuracy within the two values are considered equal.</param>
		/// <returns>
		/// true if the value is within the specified accuracy for equality, false otherwise.
		/// </returns>
		public static bool AlmostEqual (this double value, double other, double accuracy) => Math.Abs(value - other) < (double)accuracy;

		/// <summary>
		/// Determines whether a value is almost zero.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// true if the value is within the default accuracy to zero, false otherwise.
		/// </returns>
		public static bool AlmostZero (this double value) => Math.Abs(value) < (double)1.11022302462516E-15;

		/// <summary>
		/// Determines whether a value is almost zero.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="accuracy">The accuracy within the value is considered zero.</param>
		/// <returns>
		/// true if the value is within the specified accuracy to zero, false otherwise.
		/// </returns>
		public static bool AlmostZero (this double value, double accuracy) => Math.Abs(value) < (double)accuracy;

		/// <summary>
		/// Gets the smallest integer that is greater than or equal to a value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The smallest integer.
		/// </returns>
		public static double Ceiling (this double value) => (double)Math.Ceiling(value);

		/// <summary>
		/// Gets the largest integer less than or equal to a value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The largest integer.
		/// </returns>
		public static double Floor (this double value) => (double)Math.Floor(value);

		/// <summary>
		/// Gets the integer part of a value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The integer part.
		/// </returns>
		public static double Integer (this double value) => (double)Math.Truncate(value);

		/// <summary>
		/// Gets the fraction part of a value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The fraction part.
		/// </returns>
		public static double Fraction (this double value) => (double)(value - Math.Truncate(value));

		/// <summary>
		/// Rounds a value to the nearest integer.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The rounded value.
		/// </returns>
		/// <remarks>
		/// <para>
		/// <see cref="MidpointRounding.ToEven"/> is used for <see cref="MidpointRounding"/>.
		/// </para>
		/// </remarks>
		public static double RoundInteger (this double value) => (double)Math.Round(value);

		/// <summary>
		/// Rounds a value to the nearest integer.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="rounding">The kind of rounding to use.</param>
		/// <returns>
		/// The rounded value.
		/// </returns>
		public static double RoundInteger (this double value, MidpointRounding rounding) => (double)Math.Round(value, rounding);

		/// <summary>
		/// Rounds a value to a specified amount of fractional digits.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="digits">The number of digits.</param>
		/// <returns>
		/// The rounded value.
		/// </returns>
		/// <remarks>
		/// <para>
		/// <see cref="MidpointRounding.ToEven"/> is used for <see cref="MidpointRounding"/>.
		/// </para>
		/// </remarks>
		public static double RoundDigits (this double value, int digits) => (double)Math.Round(value, digits);

		/// <summary>
		/// Rounds a value to a specified amount of fractional digits.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="digits">The number of digits.</param>
		/// <param name="rounding">The kind of rounding to use.</param>
		/// <returns>
		/// The rounded value.
		/// </returns>
		public static double RoundDigits (this double value, int digits, MidpointRounding rounding) => (double)Math.Round(value, digits, rounding);

		/// <summary>
		/// Calculates value^power.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="power">The power.</param>
		/// <returns>
		/// The result.
		/// </returns>
		public static double Pow (this double value, double power) => (double)Math.Pow((double)value, power);

		/// <summary>
		/// Calculates value^2.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The result.
		/// </returns>
		public static double Pow2 (this double value) => value * value;

		/// <summary>
		/// Calculates value^3.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The result.
		/// </returns>
		public static double Pow3 (this double value) => value * value * value;

		/// <summary>
		/// Calculates value^10.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The result.
		/// </returns>
		public static double Pow10 (this double value) => (double)Math.Pow((double)value, 10.0);

		/// <summary>
		/// Calculates value^e.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The result.
		/// </returns>
		public static double PowE (this double value) => (double)Math.Pow((double)value, Math.E);

		/// <summary>
		/// Calculates expBase^value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="expBase">The base.</param>
		/// <returns>
		/// The result.
		/// </returns>
		public static double Exp (this double value, double expBase) => (double)Math.Pow(expBase, (double)value);

		/// <summary>
		/// Calculates 2^value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The result.
		/// </returns>
		public static double Exp2 (this double value) => (double)Math.Pow(2.0, (double)value);

		/// <summary>
		/// Calculates 3^value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The result.
		/// </returns>
		public static double Exp3 (this double value) => (double)Math.Pow(3.0, (double)value);

		/// <summary>
		/// Calculates 10^value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The result.
		/// </returns>
		public static double Exp10 (this double value) => (double)Math.Pow(10.0, (double)value);

		/// <summary>
		/// Calculates e^value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The result.
		/// </returns>
		public static double ExpE (this double value) => (double)Math.Exp((double)value);

		/// <summary>
		/// Calculates log[logBase](value).
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="logBase">The base.</param>
		/// <returns>
		/// The result.
		/// </returns>
		public static double Log (this double value, double logBase) => (double)Math.Log((double)value, logBase);

		/// <summary>
		/// Calculates log[2](value).
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The result.
		/// </returns>
		public static double Log2 (this double value) => (double)Math.Log((double)value, 2.0);

		/// <summary>
		/// Calculates log[3](value).
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The result.
		/// </returns>
		public static double Log3 (this double value) => (double)Math.Log((double)value, 3.0);

		/// <summary>
		/// Calculates log[10](value).
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The result.
		/// </returns>
		public static double Log10 (this double value) => (double)Math.Log10((double)value);

		/// <summary>
		/// Calculates log[e](value).
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The result.
		/// </returns>
		public static double LogE (this double value) => (double)Math.Log((double)value);

		/// <summary>
		/// Calculates the square-root of a value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The square-root.
		/// </returns>
		public static double Sqrt (this double value) => (double)Math.Sqrt((double)value);

		/// <summary>
		/// Calculates the n-th root of a value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="n">The root.</param>
		/// <returns>
		/// The n-th root.
		/// </returns>
		public static double Root (this double value, double n) => (double)Math.Pow((double)value, 1.0 / n);

		/// <summary>
		/// Calculates the Sine of a value in radians.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The result.
		/// </returns>
		public static double Sin (this double value) => (double)Math.Sin(value);

		/// <summary>
		/// Calculates the Cosine of a value in radians.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The result.
		/// </returns>
		public static double Cos (this double value) => (double)Math.Cos(value);

		/// <summary>
		/// Calculates the Tangent of a value in radians.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The result.
		/// </returns>
		public static double Tan (this double value) => (double)Math.Tan(value);

		/// <summary>
		/// Calculates the Cotangent of a value in radians.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The result.
		/// </returns>
		public static double Cot (this double value) => 1.0 / (double)Math.Tan(value);

		/// <summary>
		/// Calculates the Arc Sine of a value in radians.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The result.
		/// </returns>
		public static double Asin (this double value) => (double)Math.Asin(value);

		/// <summary>
		/// Calculates the Arc Cosine of a value in radians.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The result.
		/// </returns>
		public static double Acos (this double value) => (double)Math.Acos(value);

		/// <summary>
		/// Calculates the Arc Tangent of a value in radians.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The result.
		/// </returns>
		public static double Atan (this double value) => (double)Math.Atan(value);

		/// <summary>
		/// Calculates the Arc Cotangent of a value in radians.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The result.
		/// </returns>
		public static double Acot (this double value) => (double)Math.Tan(1.0 / value);

		/// <summary>
		/// Calculates the Hyperbolic Sine of a value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The result.
		/// </returns>
		public static double Sinh (this double value) => (double)Math.Sinh(value);

		/// <summary>
		/// Calculates the Hyperbolic Cosine of a value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The result.
		/// </returns>
		public static double Cosh (this double value) => (double)Math.Cosh(value);

		/// <summary>
		/// Calculates the Hyperbolic Tangent of a value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The result.
		/// </returns>
		public static double Tanh (this double value) => (double)Math.Tanh(value);

		/// <summary>
		/// Calculates the Hyperbolic Cotangent of a value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The result.
		/// </returns>
		public static double Coth (this double value)
		{
			double e1 = Math.Exp(value);
			double e2 = Math.Exp(-value);
			return (double)((e1 + e2) / (e1 - e2));
		}

		/// <summary>
		/// Converts a radian value to degrees.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The result.
		/// </returns>
		/// <remarks>
		/// <para>
		/// The result is not clamped to a single full circle value (-359...0...+359).
		/// Use <see cref="CircularClampDeg(double)" /> to clamp to a single full circle value.
		/// </para>
		/// </remarks>
		public static double ToDeg (this double value) => value * 57.295779513;

		/// <summary>
		/// Converts a degree value to radians.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The result.
		/// </returns>
		/// <remarks>
		/// <para>
		/// The result is not clamped to a single full circle value (-2π...0...+2π).
		/// Use <see cref="CircularClampRad(double)" /> to clamp to a single full circle value.
		/// </para>
		/// </remarks>
		public static double ToRad (this double value) => value * 0.01745329252;

		/// <summary>
		/// Clamps a degree value to a single full circle (-359...0...+359).
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The result.
		/// </returns>
		/// <remarks>
		/// <para>
		/// Examples: 0 -> 0; 350 -> 350; 360 -> 0; 370 -> 10; etc.
		/// </para>
		/// </remarks>
		public static double CircularClampDeg (this double value) => value.DivRem(360.0);

		/// <summary>
		/// Clamps a radian value to a single full circle (-2π...0...+2π).
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The result.
		/// </returns>
		/// <remarks>
		/// <para>
		/// Examples: 0 -> 0; 1.5 -> 1.5; 2π -> 0; 3π -> π; etc.
		/// </para>
		/// </remarks>
		public static double CircularClampRad (this double value) => value.DivRem(2.0*3.1415926535897932384626433832795028841971693993751);
	}
}

#endif
