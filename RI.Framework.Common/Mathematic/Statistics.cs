using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Mathematic
{
	/// <summary>
	/// Contains statistics about a sequence of numbers.
	/// </summary>
	public struct Statistics : ICloneable<Statistics>, ICloneable
	{
		/// <summary>
		/// Creates a new instance of <see cref="Statistics"/>.
		/// </summary>
		/// <param name="values">The sequence of numbers of which the statistics are calculated.</param>
		/// <remarks>
		/// <para>
		/// <paramref name="values"/> is only enumerated once.
		/// </para>
		/// <para>
		/// If the sequence <paramref name="values"/> contains no values, the statistics are initialized with the following values:
		/// <see cref="Count"/>, <see cref="Sum"/>, <see cref="SquareSum"/>, <see cref="RMS"/>, <see cref="Average"/>, <see cref="Sigma"/> are set to zero.
		/// <see cref="Min"/>, <see cref="Max"/> are set to <see cref="double.NaN"/>.
		/// <see cref="Values"/>, <see cref="SortedValues"/> are set to an empty array.
		/// </para>
		/// <para>
		/// If the sequence <paramref name="values"/> contains less than two values, the statistics are initialized with the following values:
		/// <see cref="Median"/> is set to <see cref="double.NaN"/>.
		/// </para>
		/// <note type="note">
		/// This constructor takes some time to execute because of the various calculations (approx. O(n log n)).
		/// For performance sensitive scenarios, consider using <see cref="RunningValuesBase{T}"/> or its derived classes respectively.
		/// </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"><paramref name="values"/> is null.</exception>
		public Statistics (IEnumerable<double> values)
		{
			if (values == null)
			{
				throw new ArgumentNullException(nameof(values));
			}

			this.Values = values.ToArray();
			this.SortedValues = (double[])this.Values.Clone();
			Array.Sort(this.SortedValues);

			this.Count = 0;
			this.Sum = 0.0;
			this.SquareSum = 0.0;

			this.Min = double.MaxValue;
			this.Max = double.MinValue;

			foreach (double value in this.Values)
			{
				this.Count++;
				this.Sum += value;
				this.SquareSum += Math.Pow(value, 2.0);

				if (this.Min > value)
				{
					this.Min = value;
				}

				if (this.Max < value)
				{
					this.Max = value;
				}
			}

			if (this.Count == 0)
			{
				this.Min = double.NaN;
				this.Max = double.NaN;
				this.RMS = 0.0;
				this.Average = 0.0;
				this.Sigma = 0.0;
			}
			else
			{
				this.RMS = Math.Sqrt(this.SquareSum / this.Count);
				this.Average = this.Sum / this.Count;

				double sigmaTemp = 0.0;
				foreach (double value in this.Values)
				{
					sigmaTemp += Math.Pow(value - this.Average, 2.0);
				}
				this.Sigma = Math.Sqrt(sigmaTemp / this.Count);
			}

			if (this.Count < 2)
			{
				this.Median = double.NaN;
			}
			else if ((this.Count % 2) == 0)
			{
				this.Median = ((this.SortedValues[this.Count / 2]) + (this.SortedValues[(this.Count / 2) - 1])) / 2.0;
			}
			else
			{
				this.Median = this.SortedValues[this.Count / 2];
			}
		}

		/// <summary>
		/// The number of values.
		/// </summary>
		public int Count;

		/// <summary>
		/// The sum of all values.
		/// </summary>
		public double Sum;

		/// <summary>
		/// The sum of all values squared.
		/// </summary>
		public double SquareSum;

		/// <summary>
		/// The smallest value of all values.
		/// </summary>
		public double Min;

		/// <summary>
		/// The largest value of all values.
		/// </summary>
		public double Max;

		/// <summary>
		/// The root-mean-square (RMS) of all values.
		/// </summary>
		[SuppressMessage("ReSharper", "InconsistentNaming")]
		public double RMS;

		/// <summary>
		/// The average of all values.
		/// </summary>
		public double Average;

		/// <summary>
		/// The standard deviation of all values.
		/// </summary>
		public double Sigma;

		/// <summary>
		/// The median of all values.
		/// </summary>
		public double Median;

		/// <summary>
		/// All values, sorted by their numeric value.
		/// </summary>
		public double[] SortedValues;

		/// <summary>
		/// All values.
		/// </summary>
		public double[] Values;

		/// <inheritdoc />
		public Statistics Clone ()
		{
			Statistics clone = new Statistics();
			clone.Count = this.Count;
			clone.Sum = this.Sum;
			clone.SquareSum = this.SquareSum;
			clone.Min = this.Min;
			clone.Max = this.Max;
			clone.RMS = this.RMS;
			clone.Average = this.Average;
			clone.Sigma = this.Sigma;
			clone.Median = this.Median;
			clone.SortedValues = (double[])this.SortedValues.Clone();
			clone.Values = (double[])this.Values.Clone();
			return clone;
		}

		/// <inheritdoc />
		object ICloneable.Clone ()
		{
			return this.Clone();
		}
	}
}