using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;




namespace RI.Framework.Utilities.Text
{
	/// <summary>
	///     Implements a comparer which uses natural alphanumeric string sorting.
	/// </summary>
	/// <remarks>
	///     <para>
	///         Natural alphanumeric sorting is a more sophisticated string sorting algorithm which respects the natural way humans look at strings, especially if they contain numbers.
	///     </para>
	///     <para>
	///         The following list of strings is used as an example: Test100xyz, Test200abc, Test99abc.
	///         Traditional sorting would sort this list as shown, putting Test99abc at the end (because the first four characters are the same an then 9 is greater than 2).
	///         With natural alphanumeric sorting, the list is sorted as: Test99abc, Test100xyz, Test200abc (because the algorithm builds chunks and compares those instead of charcater-by-character).
	///     </para>
	///     <para>
	///         Because this kind of sorting depends on the used culture, the used culture must be specified.
	///     </para>
	/// </remarks>
	public sealed class AlphaNumComparer : IComparer<string>
	{
		#region Static Methods

		/// <summary>
		///     Creates a natural alphanumeric comparer for the current thread culture (case matters; strings are not trimmed before comparison).
		/// </summary>
		/// <returns>
		///     The comparer.
		/// </returns>
		public static AlphaNumComparer CurrentCulture () => new AlphaNumComparer(CultureInfo.CurrentCulture, false, false);

		/// <summary>
		///     Creates a natural alphanumeric comparer for the current thread culture (case is ignored; strings are not trimmed before comparison).
		/// </summary>
		/// <returns>
		///     The comparer.
		/// </returns>
		public static AlphaNumComparer CurrentCultureIgnoreCase () => new AlphaNumComparer(CultureInfo.CurrentCulture, true, false);

		/// <summary>
		///     Creates a natural alphanumeric comparer for the invariant culture (case matters; strings are not trimmed before comparison).
		/// </summary>
		/// <returns>
		///     The comparer.
		/// </returns>
		public static AlphaNumComparer InvariantCulture () => new AlphaNumComparer(CultureInfo.InvariantCulture, false, false);

		/// <summary>
		///     Creates a natural alphanumeric comparer for the invariant culture (case is ignored; strings are not trimmed before comparison).
		/// </summary>
		/// <returns>
		///     The comparer.
		/// </returns>
		public static AlphaNumComparer InvariantCultureIgnoreCase () => new AlphaNumComparer(CultureInfo.InvariantCulture, true, false);

		/// <summary>
		///     Creates a natural alphanumeric comparer for the current thread culture (case matters; strings are trimmed before comparison).
		/// </summary>
		/// <returns>
		///     The comparer.
		/// </returns>
		public static AlphaNumComparer TrimmedCurrentCulture () => new AlphaNumComparer(CultureInfo.CurrentCulture, false, true);

		/// <summary>
		///     Creates a natural alphanumeric comparer for the current thread culture (case is ignored; strings are trimmed before comparison).
		/// </summary>
		/// <returns>
		///     The comparer.
		/// </returns>
		public static AlphaNumComparer TrimmedCurrentCultureIgnoreCase () => new AlphaNumComparer(CultureInfo.CurrentCulture, true, true);

		/// <summary>
		///     Creates a natural alphanumeric comparer for the invariant culture (case matters; strings are trimmed before comparison).
		/// </summary>
		/// <returns>
		///     The comparer.
		/// </returns>
		public static AlphaNumComparer TrimmedInvariantCulture () => new AlphaNumComparer(CultureInfo.InvariantCulture, false, true);

		/// <summary>
		///     Creates a natural alphanumeric comparer for the invariant culture (case is ignored; strings are trimmed before comparison).
		/// </summary>
		/// <returns>
		///     The comparer.
		/// </returns>
		public static AlphaNumComparer TrimmedInvariantCultureIgnoreCase () => new AlphaNumComparer(CultureInfo.InvariantCulture, true, true);

		#endregion




		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="AlphaNumComparer" />.
		/// </summary>
		/// <param name="culture"> The used culture. </param>
		/// <param name="ignoreCase"> Specifies whether a characters case is ignored for comparison. </param>
		/// <param name="trimmed"> Specifies whether the strings are trimmed before the comparison. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="culture" /> is null. </exception>
		public AlphaNumComparer (CultureInfo culture, bool ignoreCase, bool trimmed, bool pureNumbers)
		{
			if (culture == null)
			{
				throw new ArgumentNullException(nameof(culture));
			}

			this.Culture = culture;
			this.IgnoreCase = ignoreCase;
			this.Trimmed = trimmed;
			this.PureNumbers = pureNumbers;

			this.NumberCharacters = new HashSet<string>(StringComparerEx.Ordinal);
			if (!this.PureNumbers)
			{
				this.NumberCharacters.Add(this.Culture.NumberFormat.NumberDecimalSeparator);
				this.NumberCharacters.Add(this.Culture.NumberFormat.PositiveSign);
				this.NumberCharacters.Add(this.Culture.NumberFormat.NegativeSign);
				this.NumberCharacters.Add(this.Culture.NumberFormat.NumberGroupSeparator);
			}
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets the used culture.
		/// </summary>
		/// <value>
		///     The used culture.
		/// </value>
		public CultureInfo Culture { get; }

		/// <summary>
		///     Gets whether a characters case is ignored for comparison.
		/// </summary>
		/// <value>
		///     true if lowercase and uppercase of the same characters are considered equal, false otherwise.
		/// </value>
		public bool IgnoreCase { get; }

		/// <summary>
		///     Gets whether the strings are trimmed before the comparison.
		/// </summary>
		/// <value>
		///     true if the strings are trimmed before comparison, false otherwise.
		/// </value>
		public bool Trimmed { get; }

		/// <summary>
		/// Gets whether numbers are only used when they are pure numbers.
		/// </summary>
		/// <value>
		/// true if numbers are only used when they are pure numbers, false otherwise.
		/// </value>
		public bool PureNumbers { get; }



		private HashSet<string> NumberCharacters { get; set; }

		#endregion




		#region Instance Methods

		private int CompareChunks (string x, string y)
		{
			NumberStyles numberCompareStyles = NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign | NumberStyles.AllowLeadingWhite | NumberStyles.AllowThousands | NumberStyles.AllowTrailingWhite;

			double xValue;
			double yValue;
			if (double.TryParse(x, numberCompareStyles, this.Culture, out xValue) && double.TryParse(y, numberCompareStyles, this.Culture, out yValue))
			{
				return xValue.CompareTo(yValue);
			}

			CompareInfo comparer = this.Culture.CompareInfo;
			int result = comparer.Compare(x, y, this.IgnoreCase ? CompareOptions.IgnoreCase : CompareOptions.None);
			return result;
		}

		private bool IsDigit (char chr)
		{
			return char.IsDigit(chr) || this.NumberCharacters.Contains(new string(chr, 1));
		}

		private int ReadWhileSameType (string str, int startIndex, StringBuilder sb)
		{
			bool isDigit = false;
			int readCount;
			for (readCount = startIndex; readCount < str.Length; readCount++)
			{
				char chr = str[readCount];
				if (readCount == 0)
				{
					isDigit = this.IsDigit(chr);
					sb.Append(chr);
				}
				else
				{
					if (isDigit != this.IsDigit(chr))
					{
						break;
					}
					sb.Append(chr);
				}
			}
			return readCount;
		}

		#endregion




		#region Interface: IComparer<string>

		/// <inheritdoc />
		public int Compare (string x, string y)
		{
			if ((x == null) && (y == null))
			{
				return 0;
			}

			if (x == null)
			{
				return -1;
			}

			if (y == null)
			{
				return 1;
			}

			if (this.Trimmed)
			{
				x = x.Trim();
				y = y.Trim();
			}

			if ((x.Length == 0) && (y.Length == 0))
			{
				return 0;
			}

			if (x.Length == 0)
			{
				return -1;
			}

			if (y.Length == 0)
			{
				return 1;
			}

			int xIndex = 0;
			int yIndex = 0;

			StringBuilder xChunk = new StringBuilder();
			StringBuilder yChunk = new StringBuilder();

			while ((xIndex < x.Length) && (yIndex < y.Length))
			{
				xChunk.Remove(0, xChunk.Length);
				yChunk.Remove(0, xChunk.Length);

				int xRead = this.ReadWhileSameType(x, xIndex, xChunk);
				int yRead = this.ReadWhileSameType(y, yIndex, yChunk);

				int result = this.CompareChunks(xChunk.ToString(), yChunk.ToString());
				if (result != 0)
				{
					return result;
				}

				xIndex += xRead;
				yIndex += yRead;
			}

			while (xIndex < x.Length)
			{
				xChunk.Append(x[xIndex]);
				xIndex++;
			}

			while (yIndex < y.Length)
			{
				yChunk.Append(y[yIndex]);
				yIndex++;
			}

			return this.CompareChunks(xChunk.ToString(), yChunk.ToString());
		}

		#endregion
	}
}
