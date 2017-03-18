using System;
using System.Collections.Generic;
using RI.Framework.Collections.Linq;

namespace RI.Framework.Mathematic
{
	/// <summary>
	/// Implements running values.
	/// </summary>
	/// <remarks>
	/// <para>
	/// So-called &quot;running values&quot; are values which are calculated based on a history of previous values.
	/// The history of previous values has a certain size (or capacity) and all values in that history are used to calculate the running values.
	/// </para>
	/// <para>
	/// The number of values in the history (<see cref="Count"/>) starts at zero.
	/// It is increased with each added value, but does not exceed <see cref="Capacity"/>.
	/// When a new value is added and the history already contains <see cref="Capacity"/> number of values, the oldest value is removed from the history and does no longer contribute to the calculation of the running values.
	/// </para>
	/// <para>
	/// <see cref="RunningValues"/> is useful in scenarios where performance is critical and a stable algorithmic complexity of approximately O(1) is required, regardless of the size of the history.
	/// Use <see cref="StatisticValues"/> or <see cref="Statistics"/> if a higher precision or more calculated values are required (but unfortunately with worse performance).
	/// </para>
	/// <para>
	/// <see cref="RunningValues"/> can be used for discrete or continuous running values.
	/// Their usage can be mixed, using <see cref="Add(float)"/> or <see cref="Add(float,float)"/> with a timestep of 1.0f for discrete values or <see cref="Add(float,float)"/> for continuous values.
	/// The running values are always calculated using the weighted value, which is the value multiplied with the corresponding timestep.
	/// </para>
	/// </remarks>
	public sealed class RunningValues
	{
		/// <summary>
		/// Creates a new instance of <see cref="RunningValues"/>.
		/// </summary>
		/// <param name="capacity">The capacity of the history used to calculate the running values.</param>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than 1.</exception>
		public RunningValues (int capacity)
		{
			this.Reset(capacity);
		}

		private float[] Values;

		private float[] Timesteps;

		private int Index;

		/// <summary>
		/// Gets the capacity of the history.
		/// </summary>
		/// <value>
		/// The capacity of the history.
		/// </value>
		public int Capacity { get; private set; }

		/// <summary>
		/// Gets the number of values currently in the history.
		/// </summary>
		/// <value>
		/// The number of values currently in the history.
		/// </value>
		public int Count { get; private set; }

		/// <summary>
		/// The value which was added last to the history.
		/// </summary>
		public float Last;

		/// <summary>
		/// The difference between <see cref="Last"/> and the value added before <see cref="Last"/>.
		/// </summary>
		/// <remarks>
		/// <para>
		/// A positive number means that <see cref="Last"/> is greater than the value added before <see cref="Last"/>.
		/// </para>
		/// </remarks>
		public float Difference;

		/// <summary>
		/// The sum of all values in the history.
		/// </summary>
		public float Sum;

		/// <summary>
		/// The average of all values in the history.
		/// </summary>
		public float Average;

		/// <summary>
		/// The sum of all timesteps in the history.
		/// </summary>
		public float Duration;

		private void ReplaceOldest (float value, float timestep, out float removedValue, out float removedTimestep)
		{
			removedValue = this.Values[this.Index];
			removedTimestep = this.Timesteps[this.Index];

			this.Values[this.Index] = value;
			this.Timesteps[this.Index] = timestep;

			if (this.Count < this.Capacity)
			{
				this.Index++;
				this.Count++;

				removedValue = 0.0f;
				removedTimestep = 0.0f;
			}

			this.Index = (this.Index + 1) % this.Capacity;
		}

		/// <summary>
		/// Gets all values currently in the history.
		/// </summary>
		/// <returns>
		/// The sequence of values currently in the history.
		/// An empty sequence is returned if there are no values in the history.
		/// </returns>
		/// <remarks>
		/// <para>
		/// The values are enumerated in the order they were added to the history.
		/// </para>
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
		/// Gets all timesteps currently in the history.
		/// </summary>
		/// <returns>
		/// The sequence of timesteps currently in the history.
		/// An empty sequence is returned if there are no timesteps in the history.
		/// </returns>
		/// <remarks>
		/// <para>
		/// The timesteps are enumerated in the order they were added to the history.
		/// </para>
		/// </remarks>
		public IEnumerable<float> GetTimesteps()
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
		/// Resets all values and clears the history.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The capacity of the history is not changed.
		/// </para>
		/// </remarks>
		public void Reset()
		{
			this.Reset(this.Capacity);
		}

		/// <summary>
		/// Resets all values and clears the history.
		/// </summary>
		/// <param name="capacity">The new capacity of the history.</param>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than 1.</exception>
		public void Reset(int capacity)
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

			this.Last = 0.0f;
			this.Difference = 0.0f;
			this.Sum = 0.0f;
			this.Average = 0.0f;
			this.Duration = 0.0f;
		}

		/// <summary>
		/// Adds a new value to the history, using a timestep of 1.0f, and recalculates the values.
		/// </summary>
		/// <param name="value">The value to add.</param>
		public void Add(float value)
		{
			this.Add(value, 1.0f);
		}

		/// <summary>
		/// Adds a new value to the history, using a given timestep, and recalculates the values.
		/// </summary>
		/// <param name="value">The value to add.</param>
		/// <param name="timestep">The timestep for the value.</param>
		public void Add(float value, float timestep)
		{
			float removedValue;
			float removedTimestep;

			this.ReplaceOldest(value, timestep, out removedValue, out removedTimestep);

			float weightedValue = value * timestep;
			float weightedRemovedValue = removedValue * removedTimestep;

			float last = this.Last;
			float sum = (this.Sum - weightedRemovedValue) + weightedValue;
			float duration = (this.Duration - removedTimestep) + timestep;

			this.Last = weightedValue;
			this.Difference = weightedValue - last;
			this.Sum = sum;
			this.Average = sum / duration;
			this.Duration = duration;
		}

		/// <summary>
		/// Gets the statistics for all values in the history, taking into account the timesteps.
		/// </summary>
		/// <returns>
		/// The statistics for all values and timesteps in the history.
		/// </returns>
		public StatisticValues GetStatistics()
		{
			return new StatisticValues(this.GetHistory().Select(x => (double)x), this.GetTimesteps().Select(x => (double)x));
		}
	}
}