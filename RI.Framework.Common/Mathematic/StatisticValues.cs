using System;
using System.Collections.Generic;

using RI.Framework.Collections.DirectLinq;
using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Mathematic
{
	/// <summary>
	///     Contains statistic values for a sequence of numbers.
	/// </summary>
	/// <remarks>
	///     <note type="note">
	///         For performance sensitive scenarios, consider using <see cref="RunningValues" />.
	///     </note>
	/// </remarks>
	[Serializable]
	public struct StatisticValues : ICloneable<StatisticValues>, ICloneable
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="StatisticValues" />.
		/// </summary>
		/// <param name="values"> The sequence of numbers the statistics are calculated from. </param>
		/// <remarks>
		///     <para>
		///         <paramref name="values" /> is enumerated only once.
		///     </para>
		///     <para>
		///         As no timestep is provided, the timestep is implicitly assumed to be 1.0.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="values" /> is null. </exception>
		public StatisticValues (IEnumerable<double> values)
			: this(values, null, 1.0)
		{
			if (values == null)
			{
				throw new ArgumentNullException(nameof(values));
			}
		}

		/// <summary>
		///     Creates a new instance of <see cref="StatisticValues" />.
		/// </summary>
		/// <param name="values"> The sequence of numbers the statistics are calculated from. </param>
		/// <param name="fixedTimestep"> The fixed timestep which is used for all values. </param>
		/// <remarks>
		///     <para>
		///         <paramref name="values" /> is enumerated only once.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="values" /> is null. </exception>
		/// <exception cref="NotFiniteArgumentException"> <paramref name="fixedTimestep" /> is NaN or infinity. </exception>
		public StatisticValues (IEnumerable<double> values, double fixedTimestep)
			: this(values, null, fixedTimestep)
		{
			if (values == null)
			{
				throw new ArgumentNullException(nameof(values));
			}

			if (fixedTimestep.IsNanOrInfinity())
			{
				throw new NotFiniteArgumentException(nameof(fixedTimestep));
			}
		}

		/// <summary>
		///     Creates a new instance of <see cref="Statistics" />.
		/// </summary>
		/// <param name="values"> The sequence of numbers the statistics are calculated from. </param>
		/// <param name="timesteps"> The sequence of timesteps for each value. </param>
		/// <remarks>
		///     <para>
		///         <paramref name="values" /> is enumerated only once.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="values" /> or <paramref name="timesteps" /> is null. </exception>
		/// <exception cref="ArgumentException"> The number of timesteps does not match the number of values. </exception>
		public StatisticValues (IEnumerable<double> values, IEnumerable<double> timesteps)
			: this(values, timesteps, double.NaN)
		{
			if (values == null)
			{
				throw new ArgumentNullException(nameof(values));
			}

			if (timesteps == null)
			{
				throw new ArgumentNullException(nameof(timesteps));
			}
		}

		private StatisticValues (IEnumerable<double> values, IEnumerable<double> timesteps, double fixedTimestep)
		{
			Func<double, int, double[]> arrayCreator = (a, b) =>
			{
				double[] array = new double[b];
				for (int i1 = 0; i1 < b; i1++)
				{
					array[i1] = a;
				}
				return array;
			};

			double[] valueArray = values.ToArray();
			double[] timestepArray = timesteps?.ToArray() ?? arrayCreator(fixedTimestep, valueArray.Length);

			if (valueArray.Length != timestepArray.Length)
			{
				throw new ArgumentException("The number of timesteps does not match the number of values.", nameof(timesteps));
			}

			this.Values = valueArray;
			this.Timesteps = timestepArray;

			this.WeightedValues = new double[this.Values.Length];
			this.Count = 0;
			this.Duration = 0.0;
			this.Sum = 0.0;
			this.SquareSum = 0.0;

			this.Min = double.MaxValue;
			this.Max = double.MinValue;

			for (int i1 = 0; i1 < this.Values.Length; i1++)
			{
				double value = this.Values[i1];
				double timestep = this.Timesteps[i1];
				double weightedValue = value * timestep;

				this.WeightedValues[i1] = weightedValue;
				this.Count++;
				this.Duration += timestep;
				this.Sum += weightedValue;
				this.SquareSum += Math.Pow(weightedValue, 2.0);

				if (this.Min > weightedValue)
				{
					this.Min = weightedValue;
				}

				if (this.Max < weightedValue)
				{
					this.Max = weightedValue;
				}
			}

			this.SortedValues = (double[])this.Values.Clone();
			Array.Sort(this.SortedValues);

			this.WeightedSortedValues = (double[])this.WeightedValues.Clone();
			Array.Sort(this.WeightedSortedValues);

			if (this.Count == 0)
			{
				this.Min = double.NaN;
				this.Max = double.NaN;

				this.Rms = 0.0;
				this.ArithmeticMean = 0.0;
				this.Sigma = 0.0;
			}
			else
			{
				this.Rms = Math.Sqrt(this.SquareSum / this.Duration);
				this.ArithmeticMean = this.Sum / this.Duration;

				double sigmaTemp = 0.0;
				foreach (double value in this.WeightedValues)
				{
					sigmaTemp += Math.Pow(value - this.ArithmeticMean, 2.0);
				}
				this.Sigma = Math.Sqrt(sigmaTemp / this.Duration);
			}

			if (this.Count < 2)
			{
				this.Median = double.NaN;
			}
			else if ((this.Count % 2) == 0)
			{
				this.Median = ((this.WeightedSortedValues[this.Count / 2]) + (this.WeightedSortedValues[(this.Count / 2) - 1])) / 2.0;
			}
			else
			{
				this.Median = this.WeightedSortedValues[this.Count / 2];
			}
		}

		#endregion




		#region Instance Fields

		/// <summary>
		///     The arithmetic mean or average of all values.
		/// </summary>
		public double ArithmeticMean;

		/// <summary>
		///     The number of values.
		/// </summary>
		public int Count;

		/// <summary>
		///     The sum of all timesteps.
		/// </summary>
		public double Duration;

		/// <summary>
		///     The largest value of all values.
		/// </summary>
		/// <remarks>
		///     <note type="note">
		///         This value is <see cref="double.NaN" /> after one of the parameterized constructors is used with an empty sequence of numbers.
		///     </note>
		/// </remarks>
		public double Max;

		/// <summary>
		///     The median of all values.
		/// </summary>
		/// <remarks>
		///     <note type="note">
		///         This value is <see cref="double.NaN" /> after one of the parameterized constructors is used with a sequence of numbers of less than two numbers.
		///     </note>
		/// </remarks>
		public double Median;

		/// <summary>
		///     The smallest value of all values.
		/// </summary>
		/// <remarks>
		///     <note type="note">
		///         This value is <see cref="double.NaN" /> after one of the parameterized constructors is used with an empty sequence of numbers.
		///     </note>
		/// </remarks>
		public double Min;

		/// <summary>
		///     The root-mean-square (RMS) of all values.
		/// </summary>
		public double Rms;

		/// <summary>
		///     The standard deviation of all values.
		/// </summary>
		public double Sigma;

		/// <summary>
		///     All values, sorted by their numeric value.
		/// </summary>
		public double[] SortedValues;

		/// <summary>
		///     The sum of all squared values (first squared, then summed).
		/// </summary>
		public double SquareSum;

		/// <summary>
		///     The sum of all values.
		/// </summary>
		public double Sum;

		/// <summary>
		///     The timesteps of each value.
		/// </summary>
		public double[] Timesteps;

		/// <summary>
		///     All values.
		/// </summary>
		public double[] Values;

		/// <summary>
		///     All values, multiplied by their corresponding timestep and then sorted by their numeric value.
		/// </summary>
		public double[] WeightedSortedValues;

		/// <summary>
		///     All values, multiplied by their corresponding timestep.
		/// </summary>
		public double[] WeightedValues;

		#endregion




		#region Interface: ICloneable<StatisticValues>

		/// <inheritdoc />
		public StatisticValues Clone ()
		{
			StatisticValues clone = new StatisticValues();

			clone.Sum = this.Sum;
			clone.SquareSum = this.SquareSum;
			clone.Min = this.Min;
			clone.Max = this.Max;
			clone.Rms = this.Rms;
			clone.ArithmeticMean = this.ArithmeticMean;
			clone.Sigma = this.Sigma;
			clone.Median = this.Median;
			clone.Values = (double[])this.Values?.Clone();
			clone.SortedValues = (double[])this.SortedValues?.Clone();
			clone.WeightedValues = (double[])this.WeightedValues?.Clone();
			clone.WeightedSortedValues = (double[])this.WeightedSortedValues?.Clone();

			clone.Count = this.Count;
			clone.Duration = this.Duration;
			clone.Timesteps = (double[])this.Timesteps?.Clone();

			return clone;
		}

		/// <inheritdoc />
		object ICloneable.Clone ()
		{
			return this.Clone();
		}

		#endregion
	}
}
