namespace RI.Framework.Utilities
{
	/// <summary>
	///     Provides utility/extension methods for number types.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Numerical types are: <see cref="byte"/>, <see cref="sbyte"/>, <see cref="short"/>, <see cref="ushort"/>, <see cref="int"/>, <see cref="uint"/>, <see cref="long"/>, <see cref="ulong"/>, <see cref="float"/>, <see cref="double"/>, <see cref="decimal"/>.
	/// </para>
	/// </remarks>
	public static class NumberExtensions
	{
		/// <summary>
		/// Clamps a value between a minimum and maximum value.
		/// </summary>
		/// <param name="value">The value to clamp.</param>
		/// <param name="min">The lowest allowed value.</param>
		/// <param name="max">The highest allowed value.</param>
		/// <returns>
		/// The clamped value.
		/// </returns>
		public static byte Clamp(this byte value, byte min, byte max)
		{
			if (value < min)
			{
				return min;
			}
			else if (value > max)
			{
				return max;
			}
			else
			{
				return value;
			}
		}

		/// <summary>
		/// Clamps a value to a minimum.
		/// </summary>
		/// <param name="value">The value to clamp.</param>
		/// <param name="min">The lowest allowed value.</param>
		/// <returns>
		/// The clamped value.
		/// </returns>
		public static byte Min(this byte value, byte min)
		{
			if (value < min)
			{
				return min;
			}
			else
			{
				return value;
			}
		}

		/// <summary>
		/// Clamps a value to a maximum.
		/// </summary>
		/// <param name="value">The value to clamp.</param>
		/// <param name="max">The highest allowed value.</param>
		/// <returns>
		/// The clamped value.
		/// </returns>
		public static byte Max(this byte value, byte max)
		{
			if (value > max)
			{
				return max;
			}
			else
			{
				return value;
			}
		}




		/// <summary>
		/// Clamps a value between a minimum and maximum value.
		/// </summary>
		/// <param name="value">The value to clamp.</param>
		/// <param name="min">The lowest allowed value.</param>
		/// <param name="max">The highest allowed value.</param>
		/// <returns>
		/// The clamped value.
		/// </returns>
		public static sbyte Clamp(this sbyte value, sbyte min, sbyte max)
		{
			if (value < min)
			{
				return min;
			}
			else if (value > max)
			{
				return max;
			}
			else
			{
				return value;
			}
		}

		/// <summary>
		/// Clamps a value to a minimum.
		/// </summary>
		/// <param name="value">The value to clamp.</param>
		/// <param name="min">The lowest allowed value.</param>
		/// <returns>
		/// The clamped value.
		/// </returns>
		public static sbyte Min(this sbyte value, sbyte min)
		{
			if (value < min)
			{
				return min;
			}
			else
			{
				return value;
			}
		}

		/// <summary>
		/// Clamps a value to a maximum.
		/// </summary>
		/// <param name="value">The value to clamp.</param>
		/// <param name="max">The highest allowed value.</param>
		/// <returns>
		/// The clamped value.
		/// </returns>
		public static sbyte Max(this sbyte value, sbyte max)
		{
			if (value > max)
			{
				return max;
			}
			else
			{
				return value;
			}
		}




		/// <summary>
		/// Clamps a value between a minimum and maximum value.
		/// </summary>
		/// <param name="value">The value to clamp.</param>
		/// <param name="min">The lowest allowed value.</param>
		/// <param name="max">The highest allowed value.</param>
		/// <returns>
		/// The clamped value.
		/// </returns>
		public static short Clamp(this short value, short min, short max)
		{
			if (value < min)
			{
				return min;
			}
			else if (value > max)
			{
				return max;
			}
			else
			{
				return value;
			}
		}

		/// <summary>
		/// Clamps a value to a minimum.
		/// </summary>
		/// <param name="value">The value to clamp.</param>
		/// <param name="min">The lowest allowed value.</param>
		/// <returns>
		/// The clamped value.
		/// </returns>
		public static short Min(this short value, short min)
		{
			if (value < min)
			{
				return min;
			}
			else
			{
				return value;
			}
		}

		/// <summary>
		/// Clamps a value to a maximum.
		/// </summary>
		/// <param name="value">The value to clamp.</param>
		/// <param name="max">The highest allowed value.</param>
		/// <returns>
		/// The clamped value.
		/// </returns>
		public static short Max(this short value, short max)
		{
			if (value > max)
			{
				return max;
			}
			else
			{
				return value;
			}
		}




		/// <summary>
		/// Clamps a value between a minimum and maximum value.
		/// </summary>
		/// <param name="value">The value to clamp.</param>
		/// <param name="min">The lowest allowed value.</param>
		/// <param name="max">The highest allowed value.</param>
		/// <returns>
		/// The clamped value.
		/// </returns>
		public static ushort Clamp(this ushort value, ushort min, ushort max)
		{
			if (value < min)
			{
				return min;
			}
			else if (value > max)
			{
				return max;
			}
			else
			{
				return value;
			}
		}

		/// <summary>
		/// Clamps a value to a minimum.
		/// </summary>
		/// <param name="value">The value to clamp.</param>
		/// <param name="min">The lowest allowed value.</param>
		/// <returns>
		/// The clamped value.
		/// </returns>
		public static ushort Min(this ushort value, ushort min)
		{
			if (value < min)
			{
				return min;
			}
			else
			{
				return value;
			}
		}

		/// <summary>
		/// Clamps a value to a maximum.
		/// </summary>
		/// <param name="value">The value to clamp.</param>
		/// <param name="max">The highest allowed value.</param>
		/// <returns>
		/// The clamped value.
		/// </returns>
		public static ushort Max(this ushort value, ushort max)
		{
			if (value > max)
			{
				return max;
			}
			else
			{
				return value;
			}
		}




		/// <summary>
		/// Clamps a value between a minimum and maximum value.
		/// </summary>
		/// <param name="value">The value to clamp.</param>
		/// <param name="min">The lowest allowed value.</param>
		/// <param name="max">The highest allowed value.</param>
		/// <returns>
		/// The clamped value.
		/// </returns>
		public static int Clamp(this int value, int min, int max)
		{
			if (value < min)
			{
				return min;
			}
			else if (value > max)
			{
				return max;
			}
			else
			{
				return value;
			}
		}

		/// <summary>
		/// Clamps a value to a minimum.
		/// </summary>
		/// <param name="value">The value to clamp.</param>
		/// <param name="min">The lowest allowed value.</param>
		/// <returns>
		/// The clamped value.
		/// </returns>
		public static int Min(this int value, int min)
		{
			if (value < min)
			{
				return min;
			}
			else
			{
				return value;
			}
		}

		/// <summary>
		/// Clamps a value to a maximum.
		/// </summary>
		/// <param name="value">The value to clamp.</param>
		/// <param name="max">The highest allowed value.</param>
		/// <returns>
		/// The clamped value.
		/// </returns>
		public static int Max(this int value, int max)
		{
			if (value > max)
			{
				return max;
			}
			else
			{
				return value;
			}
		}




		/// <summary>
		/// Clamps a value between a minimum and maximum value.
		/// </summary>
		/// <param name="value">The value to clamp.</param>
		/// <param name="min">The lowest allowed value.</param>
		/// <param name="max">The highest allowed value.</param>
		/// <returns>
		/// The clamped value.
		/// </returns>
		public static uint Clamp(this uint value, uint min, uint max)
		{
			if (value < min)
			{
				return min;
			}
			else if (value > max)
			{
				return max;
			}
			else
			{
				return value;
			}
		}

		/// <summary>
		/// Clamps a value to a minimum.
		/// </summary>
		/// <param name="value">The value to clamp.</param>
		/// <param name="min">The lowest allowed value.</param>
		/// <returns>
		/// The clamped value.
		/// </returns>
		public static uint Min(this uint value, uint min)
		{
			if (value < min)
			{
				return min;
			}
			else
			{
				return value;
			}
		}

		/// <summary>
		/// Clamps a value to a maximum.
		/// </summary>
		/// <param name="value">The value to clamp.</param>
		/// <param name="max">The highest allowed value.</param>
		/// <returns>
		/// The clamped value.
		/// </returns>
		public static uint Max(this uint value, uint max)
		{
			if (value > max)
			{
				return max;
			}
			else
			{
				return value;
			}
		}




		/// <summary>
		/// Clamps a value between a minimum and maximum value.
		/// </summary>
		/// <param name="value">The value to clamp.</param>
		/// <param name="min">The lowest allowed value.</param>
		/// <param name="max">The highest allowed value.</param>
		/// <returns>
		/// The clamped value.
		/// </returns>
		public static long Clamp(this long value, long min, long max)
		{
			if (value < min)
			{
				return min;
			}
			else if (value > max)
			{
				return max;
			}
			else
			{
				return value;
			}
		}

		/// <summary>
		/// Clamps a value to a minimum.
		/// </summary>
		/// <param name="value">The value to clamp.</param>
		/// <param name="min">The lowest allowed value.</param>
		/// <returns>
		/// The clamped value.
		/// </returns>
		public static long Min(this long value, long min)
		{
			if (value < min)
			{
				return min;
			}
			else
			{
				return value;
			}
		}

		/// <summary>
		/// Clamps a value to a maximum.
		/// </summary>
		/// <param name="value">The value to clamp.</param>
		/// <param name="max">The highest allowed value.</param>
		/// <returns>
		/// The clamped value.
		/// </returns>
		public static long Max(this long value, long max)
		{
			if (value > max)
			{
				return max;
			}
			else
			{
				return value;
			}
		}




		/// <summary>
		/// Clamps a value between a minimum and maximum value.
		/// </summary>
		/// <param name="value">The value to clamp.</param>
		/// <param name="min">The lowest allowed value.</param>
		/// <param name="max">The highest allowed value.</param>
		/// <returns>
		/// The clamped value.
		/// </returns>
		public static ulong Clamp(this ulong value, ulong min, ulong max)
		{
			if (value < min)
			{
				return min;
			}
			else if (value > max)
			{
				return max;
			}
			else
			{
				return value;
			}
		}

		/// <summary>
		/// Clamps a value to a minimum.
		/// </summary>
		/// <param name="value">The value to clamp.</param>
		/// <param name="min">The lowest allowed value.</param>
		/// <returns>
		/// The clamped value.
		/// </returns>
		public static ulong Min(this ulong value, ulong min)
		{
			if (value < min)
			{
				return min;
			}
			else
			{
				return value;
			}
		}

		/// <summary>
		/// Clamps a value to a maximum.
		/// </summary>
		/// <param name="value">The value to clamp.</param>
		/// <param name="max">The highest allowed value.</param>
		/// <returns>
		/// The clamped value.
		/// </returns>
		public static ulong Max(this ulong value, ulong max)
		{
			if (value > max)
			{
				return max;
			}
			else
			{
				return value;
			}
		}




		/// <summary>
		/// Clamps a value between a minimum and maximum value.
		/// </summary>
		/// <param name="value">The value to clamp.</param>
		/// <param name="min">The lowest allowed value.</param>
		/// <param name="max">The highest allowed value.</param>
		/// <returns>
		/// The clamped value.
		/// </returns>
		public static float Clamp(this float value, float min, float max)
		{
			if (value < min)
			{
				return min;
			}
			else if (value > max)
			{
				return max;
			}
			else
			{
				return value;
			}
		}

		/// <summary>
		/// Clamps a value to a minimum.
		/// </summary>
		/// <param name="value">The value to clamp.</param>
		/// <param name="min">The lowest allowed value.</param>
		/// <returns>
		/// The clamped value.
		/// </returns>
		public static float Min(this float value, float min)
		{
			if (value < min)
			{
				return min;
			}
			else
			{
				return value;
			}
		}

		/// <summary>
		/// Clamps a value to a maximum.
		/// </summary>
		/// <param name="value">The value to clamp.</param>
		/// <param name="max">The highest allowed value.</param>
		/// <returns>
		/// The clamped value.
		/// </returns>
		public static float Max(this float value, float max)
		{
			if (value > max)
			{
				return max;
			}
			else
			{
				return value;
			}
		}




		/// <summary>
		/// Clamps a value between a minimum and maximum value.
		/// </summary>
		/// <param name="value">The value to clamp.</param>
		/// <param name="min">The lowest allowed value.</param>
		/// <param name="max">The highest allowed value.</param>
		/// <returns>
		/// The clamped value.
		/// </returns>
		public static double Clamp(this double value, double min, double max)
		{
			if (value < min)
			{
				return min;
			}
			else if (value > max)
			{
				return max;
			}
			else
			{
				return value;
			}
		}

		/// <summary>
		/// Clamps a value to a minimum.
		/// </summary>
		/// <param name="value">The value to clamp.</param>
		/// <param name="min">The lowest allowed value.</param>
		/// <returns>
		/// The clamped value.
		/// </returns>
		public static double Min(this double value, double min)
		{
			if (value < min)
			{
				return min;
			}
			else
			{
				return value;
			}
		}

		/// <summary>
		/// Clamps a value to a maximum.
		/// </summary>
		/// <param name="value">The value to clamp.</param>
		/// <param name="max">The highest allowed value.</param>
		/// <returns>
		/// The clamped value.
		/// </returns>
		public static double Max(this double value, double max)
		{
			if (value > max)
			{
				return max;
			}
			else
			{
				return value;
			}
		}




		/// <summary>
		/// Clamps a value between a minimum and maximum value.
		/// </summary>
		/// <param name="value">The value to clamp.</param>
		/// <param name="min">The lowest allowed value.</param>
		/// <param name="max">The highest allowed value.</param>
		/// <returns>
		/// The clamped value.
		/// </returns>
		public static decimal Clamp(this decimal value, decimal min, decimal max)
		{
			if (value < min)
			{
				return min;
			}
			else if (value > max)
			{
				return max;
			}
			else
			{
				return value;
			}
		}

		/// <summary>
		/// Clamps a value to a minimum.
		/// </summary>
		/// <param name="value">The value to clamp.</param>
		/// <param name="min">The lowest allowed value.</param>
		/// <returns>
		/// The clamped value.
		/// </returns>
		public static decimal Min(this decimal value, decimal min)
		{
			if (value < min)
			{
				return min;
			}
			else
			{
				return value;
			}
		}

		/// <summary>
		/// Clamps a value to a maximum.
		/// </summary>
		/// <param name="value">The value to clamp.</param>
		/// <param name="max">The highest allowed value.</param>
		/// <returns>
		/// The clamped value.
		/// </returns>
		public static decimal Max(this decimal value, decimal max)
		{
			if (value > max)
			{
				return max;
			}
			else
			{
				return value;
			}
		}
	}
}
