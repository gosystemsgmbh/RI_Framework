using System;
using System.Collections.Generic;
using System.Text;

using RI.Framework.Collections.Linq;
using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;




namespace RI.Framework.Mathematic
{
	/// <summary>
	/// Type to convert and store roman numbers.
	/// </summary>
	/// <remarks>
	/// <para>
	/// You can either use <see cref="RomanNumber"/> to store a roman number by creating instances or use its static methods to convert from/to roman numbers.
	/// </para>
	/// <para>
	/// See <see href="https://en.wikipedia.org/wiki/Roman_numerals"> https://en.wikipedia.org/wiki/Roman_numerals </see> for details about roman numbers.
	/// </para>
	/// <note type="note">
	/// Only the uppercase letters I, V, X, L, C, D, M are supported, lowercase letters of those characters ar considered invalid (as is any other character).
	/// </note>
	/// </remarks>
	public struct RomanNumber : IEquatable<RomanNumber>, IComparable<RomanNumber>, IComparable
	{
		private static Dictionary<char, int> ValuesToDecimal { get; set; }

		private static Dictionary<int, char> ValuesToRoman { get; set; }

		private static Dictionary<string, int> SpecialCasesToDecimal { get; set; }

		private static Dictionary<int, string> SpecialCasesToRoman { get; set; }

		static RomanNumber ()
		{
			RomanNumber.ValuesToDecimal = new Dictionary<char, int>();
			RomanNumber.ValuesToDecimal.Add('I', 1);
			RomanNumber.ValuesToDecimal.Add('V', 5);
			RomanNumber.ValuesToDecimal.Add('X', 10);
			RomanNumber.ValuesToDecimal.Add('L', 50);
			RomanNumber.ValuesToDecimal.Add('C', 100);
			RomanNumber.ValuesToDecimal.Add('D', 500);
			RomanNumber.ValuesToDecimal.Add('M', 1000);

			RomanNumber.ValuesToRoman = new Dictionary<int, char>();
			RomanNumber.ValuesToRoman.Add(1, 'I');
			RomanNumber.ValuesToRoman.Add(5, 'V');
			RomanNumber.ValuesToRoman.Add(10, 'X');
			RomanNumber.ValuesToRoman.Add(50, 'L');
			RomanNumber.ValuesToRoman.Add(100, 'C');
			RomanNumber.ValuesToRoman.Add(500, 'D');
			RomanNumber.ValuesToRoman.Add(1000, 'M');

			RomanNumber.SpecialCasesToDecimal = new Dictionary<string, int>(StringComparerEx.Ordinal);
			RomanNumber.SpecialCasesToDecimal.Add("IV", 4);
			RomanNumber.SpecialCasesToDecimal.Add("IX", 9);
			RomanNumber.SpecialCasesToDecimal.Add("XL", 40);
			RomanNumber.SpecialCasesToDecimal.Add("XC", 90);
			RomanNumber.SpecialCasesToDecimal.Add("CD", 400);
			RomanNumber.SpecialCasesToDecimal.Add("CM", 900);

			RomanNumber.SpecialCasesToRoman = new Dictionary<int, string>();
			RomanNumber.SpecialCasesToRoman.Add(4, "IV");
			RomanNumber.SpecialCasesToRoman.Add(9, "IX");
			RomanNumber.SpecialCasesToRoman.Add(40, "XL");
			RomanNumber.SpecialCasesToRoman.Add(90, "XC");
			RomanNumber.SpecialCasesToRoman.Add(400, "CD");
			RomanNumber.SpecialCasesToRoman.Add(900, "CM");
		}

		/// <summary>
		/// Converts a decimal number into a roman number.
		/// </summary>
		/// <param name="value">The decimal number.</param>
		/// <returns>
		/// The roman number as a string.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> is zero.</exception>
		public static string DecimalToRoman (int value)
		{
			if (value == 0)
			{
				throw new ArgumentOutOfRangeException(nameof(value));
			}

			List<int> values = RomanNumber.ValuesToRoman.Keys.ToList();
			values.Sort();
			values.Reverse();

			List<int> specialCases = RomanNumber.SpecialCasesToRoman.Keys.ToList();
			values.Sort();
			values.Reverse();

			StringBuilder sb = new StringBuilder();
			int remaining = value;

			if (value < 0)
			{
				sb.Append('-');
				remaining = Math.Abs(value);
			}

			//TODO: Implement
			throw new NotImplementedException();

			return sb.ToString();
		}

		/// <summary>
		/// Tries to convert a roman number into a decimal number.
		/// </summary>
		/// <param name="value">The roman number as a string.</param>
		/// <returns>
		/// The decimal number or null if <paramref name="value"/> is not a vlid roman number.
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
		public static int? RomanToDecimal (string value)
		{
			if (value == null)
			{
				throw new ArgumentNullException(nameof(value));
			}

			if (value.IsEmptyOrWhitespace())
			{
				return null;
			}

			int start = 0;
			int sum = 0;
			int factor = 1;

			if (value[0] == '-')
			{
				start = 1;
				factor = -1;
			}
			else if (value[0] == '+')
			{
				start = 1;
				factor = 1;
			}

			for (int i1 = start; i1 < value.Length; i1++)
			{
				if (i1 < (value.Length - 1))
				{
					string specialCaseCandidate = value.Substring(i1, 2);
					if (RomanNumber.SpecialCasesToDecimal.ContainsKey(specialCaseCandidate))
					{
						sum += RomanNumber.SpecialCasesToDecimal[specialCaseCandidate];
						i1++;
						continue;
					}
				}

				char currentCharacter = value[i1];
				if (RomanNumber.ValuesToDecimal.ContainsKey(currentCharacter))
				{
					sum += RomanNumber.ValuesToDecimal[currentCharacter];
					continue;
				}

				return null;
			}

			return sum * factor;
		}

		/// <summary>
		/// Parses a string as a roman number.
		/// </summary>
		/// <param name="str">The string to parse.</param>
		/// <returns>
		/// The roman number.
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="str"/> is null.</exception>
		/// <exception cref="FormatException"><paramref name="str"/> is not a valid roman number.</exception>
		public static RomanNumber Parse (string str)
		{
			if (str == null)
			{
				throw new ArgumentNullException(nameof(str));
			}

			RomanNumber value;
			if (!RomanNumber.TryParse(str, out value))
			{
				throw new FormatException("\"" + str + "\" is not a valid roman number.");
			}
			return value;
		}

		/// <summary>
		/// Tries to parse a string as a roman number.
		/// </summary>
		/// <param name="str">The string to parse.</param>
		/// <param name="value">The parsed roman number.</param>
		/// <returns>
		/// true if <paramref name="str"/> was a valid roman number, false otherwise.
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="str"/> is null.</exception>
		public static bool TryParse (string str, out RomanNumber value)
		{
			if (str == null)
			{
				throw new ArgumentNullException(nameof(str));
			}

			int? decimalValue = RomanNumber.RomanToDecimal(str);
			if (decimalValue.HasValue)
			{
				value = new RomanNumber(decimalValue.Value);
				return true;
			}
			value = new RomanNumber();
			return false;
		}

		/// <summary>
		/// Creates a new instance of <see cref="RomanNumber"/>.
		/// </summary>
		/// <param name="decimalValue">The number as a decimal number.</param>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="decimalValue"/> is zero.</exception>
		public RomanNumber (int decimalValue)
		{
			if (decimalValue == 0)
			{
				throw new ArgumentOutOfRangeException(nameof(decimalValue));
			}

			this.DecimalValue = decimalValue;
			this._romanValue = null;
		}

		private string _romanValue;

		/// <summary>
		/// Gets the value as a decimal value.
		/// </summary>
		/// <value>
		/// The value as a decimal value.
		/// </value>
		public int DecimalValue { get; }

		/// <summary>
		/// Gets the value as a roman value.
		/// </summary>
		/// <value>
		/// The value as a roman value.
		/// </value>
		public string RomanValue
		{
			get
			{
				if (this._romanValue == null)
				{
					this._romanValue = RomanNumber.DecimalToRoman(this.DecimalValue);
				}
				return this._romanValue;
			}
		}

		/// <inheritdoc />
		public override string ToString ()
		{
			return this.RomanValue;
		}

		/// <inheritdoc />
		public override int GetHashCode ()
		{
			return this.DecimalValue;
		}

		/// <inheritdoc />
		public override bool Equals(object obj)
		{
			if (obj is RomanNumber)
			{
				return this.Equals((RomanNumber)obj);
			}
			return false;
		}

		/// <inheritdoc />
		public bool Equals (RomanNumber other)
		{
			return this.DecimalValue == other.DecimalValue;
		}

		/// <inheritdoc />
		int IComparable.CompareTo(object obj)
		{
			if (obj is RomanNumber)
			{
				return this.CompareTo((RomanNumber)obj);
			}
			return 1;
		}

		/// <inheritdoc />
		public int CompareTo (RomanNumber other)
		{
			return this.DecimalValue.CompareTo(other.DecimalValue);
		}

		public static RomanNumber operator + (RomanNumber x, RomanNumber y)
		{
			return new RomanNumber(x.DecimalValue + y.DecimalValue);
		}

		public static RomanNumber operator -(RomanNumber x, RomanNumber y)
		{
			return new RomanNumber(x.DecimalValue + y.DecimalValue);
		}

		public static RomanNumber operator *(RomanNumber x, RomanNumber y)
		{
			return new RomanNumber(x.DecimalValue * y.DecimalValue);
		}

		public static RomanNumber operator /(RomanNumber x, RomanNumber y)
		{
			return new RomanNumber(x.DecimalValue / y.DecimalValue);
		}

		public static RomanNumber operator %(RomanNumber x, RomanNumber y)
		{
			return new RomanNumber(x.DecimalValue % y.DecimalValue);
		}

		public static RomanNumber operator &(RomanNumber x, RomanNumber y)
		{
			return new RomanNumber(x.DecimalValue & y.DecimalValue);
		}

		public static RomanNumber operator |(RomanNumber x, RomanNumber y)
		{
			return new RomanNumber(x.DecimalValue | y.DecimalValue);
		}

		public static RomanNumber operator ^(RomanNumber x, RomanNumber y)
		{
			return new RomanNumber(x.DecimalValue ^ y.DecimalValue);
		}

		public static RomanNumber operator <<(RomanNumber x, int y)
		{
			return new RomanNumber(x.DecimalValue << y);
		}

		public static RomanNumber operator >>(RomanNumber x, int y)
		{
			return new RomanNumber(x.DecimalValue >> y);
		}

		public static bool operator ==(RomanNumber x, RomanNumber y)
		{
			return x.DecimalValue == y.DecimalValue;
		}

		public static bool operator !=(RomanNumber x, RomanNumber y)
		{
			return x.DecimalValue != y.DecimalValue;
		}

		public static bool operator > (RomanNumber x, RomanNumber y)
		{
			return x.DecimalValue > y.DecimalValue;
		}

		public static bool operator <(RomanNumber x, RomanNumber y)
		{
			return x.DecimalValue < y.DecimalValue;
		}

		public static bool operator >=(RomanNumber x, RomanNumber y)
		{
			return x.DecimalValue >= y.DecimalValue;
		}

		public static bool operator <=(RomanNumber x, RomanNumber y)
		{
			return x.DecimalValue <= y.DecimalValue;
		}

		public static RomanNumber operator +(RomanNumber x)
		{
			return new RomanNumber(x.DecimalValue);
		}

		public static RomanNumber operator -(RomanNumber x)
		{
			return new RomanNumber(x.DecimalValue * -1);
		}

		public static RomanNumber operator ++(RomanNumber x)
		{
			return new RomanNumber(x.DecimalValue + 1);
		}

		public static RomanNumber operator --(RomanNumber x)
		{
			return new RomanNumber(x.DecimalValue - 1);
		}

		public static RomanNumber operator ~(RomanNumber x)
		{
			return new RomanNumber(~x.DecimalValue);
		}

		public static implicit operator RomanNumber (string value)
		{
			return RomanNumber.Parse(value);
		}

		public static implicit operator RomanNumber(int value)
		{
			return new RomanNumber(value);
		}

		public static implicit operator string(RomanNumber value)
		{
			return value.RomanValue;
		}

		public static implicit operator int(RomanNumber value)
		{
			return value.DecimalValue;
		}
	}
}