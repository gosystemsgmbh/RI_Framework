using System;
using System.Linq;




namespace RI.Framework.Mathematic
{
	/// <summary>
	/// Calculates running values using <see cref="float"/>.
	/// </summary>
	/// <remarks>
	/// <para>
	/// See <see cref="RunningValuesBase{T}"/> for more information.
	/// </para>
	/// </remarks>
	public sealed class RunningFloat : RunningValuesBase<float>
	{
		/// <summary>
		/// Creates a new instance of <see cref="RunningFloat"/>.
		/// </summary>
		/// <param name="capacity">The capacity of the history used to calculate the running values.</param>
		/// <remarks>
		/// <para>
		/// Using this constructor, all values are initialized with <see cref="float.NaN"/>.
		/// </para>
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than 1.</exception>
		public RunningFloat(int capacity)
			: this(capacity, false)
		{
		}

		/// <summary>
		/// Creates a new instance of <see cref="RunningFloat"/>.
		/// </summary>
		/// <param name="capacity">The capacity of the history used to calculate the running values.</param>
		/// <param name="initializeWithZero">Specifies whether the values are initialized with zero (true) or <see cref="Single.NaN"/> (false).</param>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than 1.</exception>
		public RunningFloat (int capacity, bool initializeWithZero)
			: base(capacity, initializeWithZero)
		{
			this.Reset();
		}

		/// <summary>
		/// Resets all values and clears the history.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The values are initialized based on the used constructor and its parameters.
		/// </para>
		/// <para>
		/// The capacity of the history is not changed.
		/// </para>
		/// </remarks>
		public void Reset()
		{
			this.Reset(this.Capacity, this.InitializeWithZero);
		}

		/// <summary>
		/// Resets all values and clears the history.
		/// </summary>
		/// <param name="capacity">The new capacity of the history.</param>
		/// <remarks>
		/// <para>
		/// The values are initialized based on the used constructor and its parameters.
		/// </para>
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than 1.</exception>
		public void Reset(int capacity)
		{
			this.Reset(capacity, this.InitializeWithZero);
		}

		/// <summary>
		/// Resets all values and clears the history.
		/// </summary>
		/// <param name="initializeWithZero">Specifies whether the values are initialized with zero (true) or <see cref="Single.NaN"/> (false).</param>
		/// <remarks>
		/// <para>
		/// The capacity of the history is not changed.
		/// </para>
		/// </remarks>
		public void Reset(bool initializeWithZero)
		{
			this.Reset(this.Capacity, initializeWithZero);
		}

		/// <summary>
		/// Resets all values and clears the history.
		/// </summary>
		/// <param name="capacity">The new capacity of the history.</param>
		/// <param name="initializeWithZero">Specifies whether the values are initialized with zero (true) or <see cref="Single.NaN"/> (false).</param>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than 1.</exception>
		public void Reset (int capacity, bool initializeWithZero)
		{
			this.ResetInternal(capacity, initializeWithZero);

			float initValue = initializeWithZero ? 0.0f : float.NaN;

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
		public void Add (float value)
		{
			if (this.Count == 0)
			{
				this.Sum = 0.0f;
				this.MinAll = float.MaxValue;
				this.MaxAll = float.MinValue;
			}

			float removedValue = this.ReplaceOldest(value);

			float lastValue = this.Last;
			float sum = (this.Sum - removedValue) + value;
			float count = this.Count;
			float capacity = this.Capacity;

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
		public override Statistics GetStatistics ()
		{
			return new Statistics(this.GetHistory().Select(x => (double)x));
		}
	}
}