﻿using System;
using System.Collections.Generic;

using RI.Framework.Collections.DirectLinq;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Mathematic.Statistics
{
	/// <summary>
	///     Implements running values.
	/// </summary>
	/// <remarks>
	///     <para>
	///         So-called &quot;running values&quot; are values which are calculated based on a history of previous values.
	///         The history of previous values has a certain size (or capacity) and all values in that history are used to calculate the running values.
	///     </para>
	///     <para>
	///         The number of values in the history (<see cref="Count" />) starts at zero.
	///         It is increased with each added value, but does not exceed <see cref="Capacity" />.
	///         When a new value is added and the history already contains <see cref="Capacity" /> number of values, the oldest value is removed from the history and does no longer contribute to the calculation of the running values.
	///     </para>
	///     <para>
	///         <see cref="RunningValues" /> is useful in scenarios where performance is critical and a stable algorithmic complexity of approximately O(1) is required, regardless of the size of the history.
	///         Use <see cref="StatisticValues" /> or <see cref="Statistics" /> if a higher precision or more calculated values are required (but unfortunately with worse performance).
	///     </para>
	///     <para>
	///         <see cref="RunningValues" /> can be used for discrete or continuous running values.
	///         Their usage can be mixed, using <see cref="Add(float)" /> or <see cref="Add(float,float)" /> with a timestep of 1.0f for discrete values or <see cref="Add(float,float)" /> for continuous values.
	///         The running values are always calculated using the weighted value, which is the value multiplied with the corresponding timestep.
	///     </para>
	/// </remarks>
	/// <example>
	///     <code language="cs">
	/// <![CDATA[
	/// // create running values
	/// var frameTime  = new RunningValues(1000);  // to calculate the average framerate
	/// var enemyKills = new RunningValues(1000);  // to calculate the average enemy kills per second
	/// 
	/// // ...
	/// 
	/// // update in each frame
	/// frameTime.Add(Time.deltaTime);
	/// enemyKills.Add(killCountSinceLastFrame, Time.deltaTime); // use the timestep to get kills per second independent of the framerate
	/// 
	/// // get averages over the last 1000 frames
	/// var framerate = 1.0f / frameTime.ArithmeticMean;
	/// var killsPerSecond = 1.0f / enemyKills.ArithmeticMean;
	/// 
	/// // reset kill counter
	/// killCountSinceLastFrame = 0;
	/// ]]>
	/// </code>
	/// </example>
	public sealed class RunningValues : ICloneable<RunningValues>, ICloneable
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="RunningValues" />.
		/// </summary>
		/// <param name="capacity"> The capacity of the history used to calculate the running values. </param>
		/// <exception cref="ArgumentOutOfRangeException"> <paramref name="capacity" /> is less than 1. </exception>
		public RunningValues (int capacity)
		{
			this.Reset(capacity);
		}

		/// <summary>
		///     Creates a new instance of <see cref="RunningValues" />.
		/// </summary>
		/// <param name="initialValues"> The initial values added to the history. </param>
		/// <remarks>
		///     <para>
		///         The initial values are added with a timestep of 1.0.
		///     </para>
		///     <para>
		///         The capacity is set to the number of values in <paramref name="initialValues" />.
		///         Therefore, the sequence must contain at least one value.
		///     </para>
		///     <para>
		///         <paramref name="initialValues" /> is enumerated only once.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="initialValues" /> is null. </exception>
		/// <exception cref="ArgumentException"> <paramref name="initialValues" /> is an empty sequence. </exception>
		public RunningValues (IEnumerable<float> initialValues)
		{
			if (initialValues == null)
			{
				throw new ArgumentNullException(nameof(initialValues));
			}

			List<float> values = initialValues.ToList();
			if (values.Count == 0)
			{
				throw new ArgumentException("The sequence of intial values is empty.", nameof(initialValues));
			}

			this.Reset(values.Count);

			for (int i1 = 0; i1 < values.Count; i1++)
			{
				float value = values[i1];
				this.Add(value);
			}
		}

		/// <summary>
		///     Creates a new instance of <see cref="RunningValues" />.
		/// </summary>
		/// <param name="initialValues"> The initial values added to the history. </param>
		/// <param name="initialTimesteps"> The timesteps values added to the history. </param>
		/// <remarks>
		///     <para>
		///         <paramref name="initialValues" /> and <paramref name="initialTimesteps" /> must contain the same number of values.
		///     </para>
		///     <para>
		///         The capacity is set to the number of values in <paramref name="initialValues" />.
		///         Therefore, the sequence must contain at least one value.
		///     </para>
		///     <para>
		///         <paramref name="initialValues" /> and <paramref name="initialTimesteps" /> are enumerated only once.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="initialValues" /> or <paramref name="initialTimesteps" /> is null. </exception>
		/// <exception cref="ArgumentException"> <paramref name="initialValues" /> or <paramref name="initialTimesteps" /> is an empty sequence or <paramref name="initialValues" /> and <paramref name="initialTimesteps" /> do not contain the same number of values. </exception>
		public RunningValues (IEnumerable<float> initialValues, IEnumerable<float> initialTimesteps)
		{
			if (initialValues == null)
			{
				throw new ArgumentNullException(nameof(initialValues));
			}

			if (initialTimesteps == null)
			{
				throw new ArgumentNullException(nameof(initialTimesteps));
			}

			List<float> values = initialValues.ToList();
			if (values.Count == 0)
			{
				throw new ArgumentException("The sequence of intial values is empty.", nameof(initialValues));
			}

			List<float> timesteps = initialTimesteps.ToList();
			if (timesteps.Count == 0)
			{
				throw new ArgumentException("The sequence of intial timesteps is empty.", nameof(initialValues));
			}

			if (values.Count != timesteps.Count)
			{
				throw new ArgumentException("The sequence of intial values and initial timesteps do not contain the same number of elements.", nameof(initialValues));
			}

			this.Reset(values.Count);

			for (int i1 = 0; i1 < values.Count; i1++)
			{
				float value = values[i1];
				float timestep = timesteps[i1];
				this.Add(value, timestep);
			}
		}

		/// <summary>
		///     Creates a new instance of <see cref="RunningValues" />.
		/// </summary>
		/// <param name="capacity"> The capacity of the history used to calculate the running values. </param>
		/// <param name="initialValues"> The initial values added to the history. </param>
		/// <remarks>
		///     <para>
		///         The initial values are added with a timestep of 1.0.
		///     </para>
		///     <para>
		///         <paramref name="initialValues" /> is enumerated only once.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException"> <paramref name="capacity" /> is less than 1. </exception>
		/// <exception cref="ArgumentNullException"> <paramref name="initialValues" /> is null. </exception>
		/// <exception cref="ArgumentException"> <paramref name="initialValues" /> is an empty sequence. </exception>
		public RunningValues (int capacity, IEnumerable<float> initialValues)
		{
			if (initialValues == null)
			{
				throw new ArgumentNullException(nameof(initialValues));
			}

			List<float> values = initialValues.ToList();
			if (values.Count == 0)
			{
				throw new ArgumentException("The sequence of intial values is empty.", nameof(initialValues));
			}

			this.Reset(capacity);

			foreach (float value in values)
			{
				this.Add(value);
			}
		}

		/// <summary>
		///     Creates a new instance of <see cref="RunningValues" />.
		/// </summary>
		/// <param name="capacity"> The capacity of the history used to calculate the running values. </param>
		/// <param name="initialValues"> The initial values added to the history. </param>
		/// <param name="initialTimesteps"> The timesteps values added to the history. </param>
		/// <remarks>
		///     <para>
		///         <paramref name="initialValues" /> and <paramref name="initialTimesteps" /> must contain the same number of values.
		///     </para>
		///     <para>
		///         <paramref name="initialValues" /> and <paramref name="initialTimesteps" /> are enumerated only once.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException"> <paramref name="capacity" /> is less than 1. </exception>
		/// <exception cref="ArgumentNullException"> <paramref name="initialValues" /> or <paramref name="initialTimesteps" /> is null. </exception>
		/// <exception cref="ArgumentException"> <paramref name="initialValues" /> or <paramref name="initialTimesteps" /> is an empty sequence or <paramref name="initialValues" /> and <paramref name="initialTimesteps" /> do not contain the same number of values. </exception>
		public RunningValues (int capacity, IEnumerable<float> initialValues, IEnumerable<float> initialTimesteps)
		{
			if (initialValues == null)
			{
				throw new ArgumentNullException(nameof(initialValues));
			}

			if (initialTimesteps == null)
			{
				throw new ArgumentNullException(nameof(initialTimesteps));
			}

			List<float> values = initialValues.ToList();
			if (values.Count == 0)
			{
				throw new ArgumentException("The sequence of intial values is empty.", nameof(initialValues));
			}

			List<float> timesteps = initialTimesteps.ToList();
			if (timesteps.Count == 0)
			{
				throw new ArgumentException("The sequence of intial timesteps is empty.", nameof(initialValues));
			}

			if (values.Count != timesteps.Count)
			{
				throw new ArgumentException("The sequence of intial values and initial timesteps do not contain the same number of elements.", nameof(initialValues));
			}

			this.Reset(capacity);

			for (int i1 = 0; i1 < values.Count; i1++)
			{
				float value = values[i1];
				float timestep = timesteps[i1];
				this.Add(value, timestep);
			}
		}

		#endregion




		#region Instance Fields

		/// <summary>
		///     The arithmetic mean or average of all values.
		/// </summary>
		public float ArithmeticMean;

		/// <summary>
		///     The difference between <see cref="Last" /> and the value added before <see cref="Last" />.
		/// </summary>
		/// <remarks>
		///     <para>
		///         A positive number means that <see cref="Last" /> is greater than the value added before <see cref="Last" />.
		///     </para>
		/// </remarks>
		public float Difference;

		/// <summary>
		///     The sum of all timesteps in the history.
		/// </summary>
		public float Duration;

		/// <summary>
		///     The value which was added last to the history.
		/// </summary>
		public float Last;

		/// <summary>
		///     The root-mean-square (RMS) of all values in the history.
		/// </summary>
		public double Rms;

		/// <summary>
		///     The sigma or standard deviation of all values in the history.
		/// </summary>
		public double Sigma;

		/// <summary>
		///     The sum of squared values (first squared, then summed) of all values in the history.
		/// </summary>
		public float SquareSum;

		/// <summary>
		///     The sum of all values in the history.
		/// </summary>
		public float Sum;

		/// <summary>
		///     The variance of all values in the history.
		/// </summary>
		public double Variance;

		private int Index;

		private float[] Timesteps;

		private float[] Values;

		private float VarianceDiff;

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets the capacity of the history.
		/// </summary>
		/// <value>
		///     The capacity of the history.
		/// </value>
		public int Capacity { get; private set; }

		/// <summary>
		///     Gets the number of values currently in the history.
		/// </summary>
		/// <value>
		///     The number of values currently in the history.
		/// </value>
		public int Count { get; private set; }

		#endregion




		#region Instance Methods

		/// <summary>
		///     Adds a new value to the history, using a timestep of 1.0f, and recalculates the values.
		/// </summary>
		/// <param name="value"> The value to add. </param>
		public void Add (float value)
		{
			this.Add(value, 1.0f);
		}

		/// <summary>
		///     Adds a new value to the history, using a given timestep, and recalculates the values.
		/// </summary>
		/// <param name="value"> The value to add. </param>
		/// <param name="timestep"> The timestep for the value. </param>
		public void Add (float value, float timestep)
		{
			float previousLast = this.Last;
			float previousAverage = this.ArithmeticMean;

			float removedValue;
			float removedTimestep;

			this.ReplaceOldest(value, timestep, out removedValue, out removedTimestep);

			float weightedValue = value * timestep;
			float weightedValueRemoved = removedValue * removedTimestep;

			float weightedSquaredValue = weightedValue * weightedValue;
			float weightedSquaredValueRemoved = weightedValueRemoved * weightedValueRemoved;

			float sum = (this.Sum - weightedValueRemoved) + weightedValue;
			float squareSum = (this.SquareSum - weightedSquaredValueRemoved) + weightedSquaredValue;
			float duration = (this.Duration - removedTimestep) + timestep;
			bool durationIsZero = duration.AlmostZero();
			float rms = durationIsZero ? 0.0f : (float)Math.Sqrt(squareSum / duration);
			float difference = weightedValue - previousLast;
			float average = durationIsZero ? 0.0f : (sum / duration);

			float weightedVarianceTemp = (float)Math.Sqrt(weightedValue - average);
			float weightedVarianceTempRemoved = (float)Math.Sqrt(weightedValueRemoved - previousAverage);
			float varianceDiff = (this.VarianceDiff - weightedVarianceTempRemoved) + weightedVarianceTemp;
			float variance = durationIsZero ? 0.0f : (varianceDiff / duration);
			float sigma = (float)Math.Sqrt(variance);

			this.Last = weightedValue;
			this.Sum = sum;
			this.SquareSum = squareSum;
			this.Duration = duration;
			this.Rms = rms;
			this.Difference = difference;
			this.ArithmeticMean = average;

			this.VarianceDiff = varianceDiff;
			this.Variance = variance;
			this.Sigma = sigma;
		}

		/// <summary>
		///     Gets all values currently in the history.
		/// </summary>
		/// <returns>
		///     The sequence of values currently in the history.
		///     An empty sequence is returned if there are no values in the history.
		/// </returns>
		/// <remarks>
		///     <para>
		///         The values are enumerated in the order they were added to the history.
		///     </para>
		/// </remarks>
		public IEnumerable<float> GetHistory ()
		{
			if (this.Count < this.Capacity)
			{
				for (int i1 = 0; i1 < this.Count; i1++)
				{
					yield return this.Values[i1];
				}
			}
			else
			{
				for (int i1 = 0; i1 < this.Count; i1++)
				{
					int index = (this.Index + i1) % this.Capacity;
					yield return this.Values[index];
				}
			}
		}

		/// <summary>
		///     Gets the statistics for all values in the history, taking into account the timesteps.
		/// </summary>
		/// <returns>
		///     The statistics for all values and timesteps in the history.
		/// </returns>
		public StatisticValues GetStatistics ()
		{
			return new StatisticValues(this.GetHistory().Select(x => (double)x), this.GetTimesteps().Select(x => (double)x));
		}

		/// <summary>
		///     Gets all timesteps currently in the history.
		/// </summary>
		/// <returns>
		///     The sequence of timesteps currently in the history.
		///     An empty sequence is returned if there are no timesteps in the history.
		/// </returns>
		/// <remarks>
		///     <para>
		///         The timesteps are enumerated in the order they were added to the history.
		///     </para>
		/// </remarks>
		public IEnumerable<float> GetTimesteps ()
		{
			if (this.Count < this.Capacity)
			{
				for (int i1 = 0; i1 < this.Count; i1++)
				{
					yield return this.Timesteps[i1];
				}
			}
			else
			{
				for (int i1 = 0; i1 < this.Count; i1++)
				{
					int index = (this.Index + i1) % this.Capacity;
					yield return this.Timesteps[index];
				}
			}
		}

		/// <summary>
		///     Resets all values and clears the history.
		/// </summary>
		/// <remarks>
		///     <para>
		///         The capacity of the history is not changed.
		///     </para>
		/// </remarks>
		public void Reset ()
		{
			this.Reset(this.Capacity);
		}

		/// <summary>
		///     Resets all values and clears the history.
		/// </summary>
		/// <param name="capacity"> The new capacity of the history. </param>
		/// <exception cref="ArgumentOutOfRangeException"> <paramref name="capacity" /> is less than 1. </exception>
		public void Reset (int capacity)
		{
			if (capacity < 1)
			{
				throw new ArgumentOutOfRangeException(nameof(capacity));
			}

			this.Capacity = capacity;
			this.Values = new float[capacity];
			this.Timesteps = new float[capacity];
			this.Index = 0;
			this.Count = 0;

			this.VarianceDiff = 0.0f;

			this.Last = 0.0f;
			this.Difference = 0.0f;
			this.Sum = 0.0f;
			this.SquareSum = 0.0f;
			this.ArithmeticMean = 0.0f;
			this.Duration = 0.0f;
			this.Rms = 0.0f;
			this.Variance = 0.0f;
			this.Sigma = 0.0f;
		}

		private void ReplaceOldest (float value, float timestep, out float removedValue, out float removedTimestep)
		{
			removedValue = this.Values[this.Index];
			removedTimestep = this.Timesteps[this.Index];

			this.Values[this.Index] = value;
			this.Timesteps[this.Index] = timestep;

			this.Index = (this.Index + 1) % this.Capacity;

			if (this.Count < this.Capacity)
			{
				this.Count++;

				removedValue = 0.0f;
				removedTimestep = 0.0f;
			}
		}

		#endregion




		#region Interface: ICloneable<RunningValues>

		/// <inheritdoc />
		public RunningValues Clone () => new RunningValues(this.GetHistory(), this.GetTimesteps());

		/// <inheritdoc />
		object ICloneable.Clone () => this.Clone();

		#endregion
	}
}
