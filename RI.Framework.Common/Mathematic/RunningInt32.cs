using System;
using System.Linq;




namespace RI.Framework.Mathematic
{
	/// <summary>
	/// Calculates running values using <see cref="int"/>.
	/// </summary>
	/// <remarks>
	/// <para>
	/// See <see cref="RunningValuesBase{T}"/> for more information.
	/// </para>
	/// </remarks>
	public sealed class RunningInt32 : RunningValuesBase<int>
	{
		/// <summary>
		/// Creates a new instance of <see cref="RunningInt32"/>.
		/// </summary>
		/// <param name="capacity">The capacity of the history used to calculate the running values.</param>
		/// <remarks>
		/// <para>
		/// All values are initialized with zero.
		/// </para>
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than 1.</exception>
		public RunningInt32(int capacity)
			: base(capacity, true)
		{
			this.Reset();
		}

		/// <summary>
		/// Resets all values and clears the history.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The values are initialized with zero.
		/// </para>
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
		/// <remarks>
		/// <para>
		/// The values are initialized with zero.
		/// </para>
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than 1.</exception>
		public void Reset(int capacity)
		{
			this.ResetInternal(capacity, true);

			int initValue = 0;

			this.Last = initValue;
			this.Difference = initValue;
			this.Sum = initValue;
			this.Average = initValue;
			this.AverageDelayed = initValue;
			this.MinAll = initValue;
			this.MaxAll = initValue;
		}

		/// <summary>
		/// Adds a new value to the history and recalculates the values.
		/// </summary>
		/// <param name="value">The value to add.</param>
		public void Add(int value)
		{
			if (this.Count == 0)
			{
				this.Sum = 0;
				this.MinAll = int.MaxValue;
				this.MaxAll = int.MinValue;
			}

			int removedValue = this.ReplaceOldest(value);

			int lastValue = this.Last;
			int sum = (this.Sum - removedValue) + value;
			int count = this.Count;
			int capacity = this.Capacity;

			this.Last = value;
			this.Difference = value - lastValue;
			this.Sum = sum;
			this.Average = sum / count;
			this.AverageDelayed = sum / capacity;

			if (this.MinAll > value)
			{
				this.MinAll = value;
			}

			if (this.MaxAll < value)
			{
				this.MaxAll = value;
			}
		}

		/// <inheritdoc />
		public override Statistics GetStatistics()
		{
			return new Statistics(this.GetHistory().Select(x => (double)x));
		}
	}
}