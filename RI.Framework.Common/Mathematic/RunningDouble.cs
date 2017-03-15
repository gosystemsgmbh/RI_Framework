using System;




namespace RI.Framework.Mathematic
{
	/// <summary>
	/// Calculates running values using <see cref="double"/>.
	/// </summary>
	/// <remarks>
	/// <para>
	/// See <see cref="RunningValuesBase{T}"/> for more information.
	/// </para>
	/// </remarks>
	public sealed class RunningDouble : RunningValuesBase<double>
	{
		/// <summary>
		/// Creates a new instance of <see cref="RunningDouble"/>.
		/// </summary>
		/// <param name="capacity">The capacity of the history used to calculate the running values.</param>
		/// <remarks>
		/// <para>
		/// Using this constructor, all values are initialized with <see cref="double.NaN"/>.
		/// </para>
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than 1.</exception>
		public RunningDouble(int capacity)
			: this(capacity, false)
		{
		}

		/// <summary>
		/// Creates a new instance of <see cref="RunningDouble"/>.
		/// </summary>
		/// <param name="capacity">The capacity of the history used to calculate the running values.</param>
		/// <param name="initializeWithZero">Specifies whether the values are initialized with zero (true) or <see cref="Double.NaN"/> (false).</param>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than 1.</exception>
		public RunningDouble(int capacity, bool initializeWithZero)
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
		/// <param name="initializeWithZero">Specifies whether the values are initialized with zero (true) or <see cref="Double.NaN"/> (false).</param>
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
		/// <param name="initializeWithZero">Specifies whether the values are initialized with zero (true) or <see cref="Double.NaN"/> (false).</param>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than 1.</exception>
		public void Reset(int capacity, bool initializeWithZero)
		{
			this.ResetInternal(capacity, initializeWithZero);

			double initValue = initializeWithZero ? 0.0 : double.NaN;

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
		public void Add(double value)
		{
			if (this.Count == 0)
			{
				this.Sum = 0.0f;
				this.MinAll = double.MaxValue;
				this.MaxAll = double.MinValue;
			}

			double removedValue = this.ReplaceOldest(value);

			double lastValue = this.Last;
			double sum = (this.Sum - removedValue) + value;
			double count = this.Count;
			double capacity = this.Capacity;

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
			return new Statistics(this.GetHistory());
		}
	}
}