using System;
using System.Collections.Generic;




namespace RI.Framework.Mathematic
{
	/// <summary>
	/// Base class for classes which calculate running values.
	/// </summary>
	/// <typeparam name="T">The type of the calculated running values.</typeparam>
	/// <remarks>
	/// <para>
	/// So-called &quot;running values&quot; are values which are calculated based on a history of previous values.
	/// The history of previous values has a certain size (or capacity) and all values in that history are used to calculate the running values.
	/// </para>
	/// <para>
	/// The number of values in the history (<see cref="Count"/>) starts at zero. it is increased with each added value, but does not exceed <see cref="Capacity"/>.
	/// When a new value is added and the history already contains <see cref="Capacity"/> number of values, the oldest value is removed from the history and does no longer contribute to the calculation of the running values.
	/// </para>
	/// <para>
	/// The following values are updated with each added value: <see cref="Last"/>, <see cref="Difference"/>, <see cref="Sum"/>, <see cref="Average"/>, <see cref="AverageDelayed"/>, <see cref="MinAll"/>, <see cref="MaxAll"/>.
	/// </para>
	/// <para>
	/// <see cref="RunningValuesBase{T}"/> or its derived classes respectively are useful in scenarios where performance is critical and a stable algorithmic complexity of approximately O(1) is required, regardless of the size of the history.
	/// </para>
	/// <para>
	/// Classes which implement calculation of running values are: <see cref="RunningFloat"/>, <see cref="RunningDouble"/>, <see cref="RunningInt32"/>.
	/// </para>
	/// </remarks>
	public abstract class RunningValuesBase<T>
	{
		internal RunningValuesBase (int capacity, bool initializeWithZero)
		{
			if (capacity < 1)
			{
				throw new ArgumentOutOfRangeException(nameof(capacity));
			}

			this.Capacity = capacity;
			this.InitializeWithZero = initializeWithZero;
		}

		/// <summary>
		/// Gets the capacity of the history.
		/// </summary>
		/// <value>
		/// The capacity of the history.
		/// </value>
		public int Capacity { get; private set; }

		/// <summary>
		/// Gets whether the values are to be initialized with zero by default.
		/// </summary>
		/// <value>
		/// true if the values are to be initialized with zero by default, false otherwise.
		/// </value>
		public bool InitializeWithZero { get; private set; }

		private T[] Values;

		private int Index;

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
		public T Last;

		/// <summary>
		/// The difference between <see cref="Last"/> and the value added before <see cref="Last"/>.
		/// </summary>
		/// <remarks>
		/// <para>
		/// A positive number means that <see cref="Last"/> is greater than the value added before <see cref="Last"/>.
		/// </para>
		/// </remarks>
		public T Difference;

		/// <summary>
		/// The sum of all values in the history.
		/// </summary>
		public T Sum;

		/// <summary>
		/// The average of all values in the history.
		/// </summary>
		/// <remarks>
		/// <para>
		/// In contrast to <see cref="AverageDelayed"/>, <see cref="Average"/> is only calculated with the values currently in the history (number of <see cref="Count"/> values).
		/// </para>
		/// </remarks>
		public T Average;

		/// <summary>
		/// The average of the entire history.
		/// </summary>
		/// <remarks>
		/// <para>
		/// In contrast to <see cref="Average"/>, <see cref="AverageDelayed"/> is always calculated with the number of <see cref="Capacity"/> values, using zero for the number of values between <see cref="Count"/> and <see cref="Capacity"/>.
		/// Therefore, <see cref="AverageDelayed"/> can be used to get the &quot;slow-starting&quot; average which starts near zero and approaches <see cref="Average"/>, reaching <see cref="Average"/> as soon as the entire history contains values (means: <see cref="Count"/> is equal to <see cref="Capacity"/>).
		/// </para>
		/// </remarks>
		public T AverageDelayed;

		/// <summary>
		/// The smallest value of all values previously added to the history.
		/// </summary>
		/// <remarks>
		/// <note type="important">
		/// <see cref="MinAll"/> is using all values ever added to the history since the instance was constructed or reset, not only the values in the history.
		/// </note>
		/// </remarks>
		public T MinAll;

		/// <summary>
		/// The largest value of all values previously added to the history.
		/// </summary>
		/// <remarks>
		/// <note type="important">
		/// <see cref="MaxAll"/> is using all values ever added to the history since the instance was constructed or reset, not only the values in the history.
		/// </note>
		/// </remarks>
		public T MaxAll;

		/// <summary>
		/// Replaces the oldest value in the history with a new value.
		/// </summary>
		/// <param name="value">The newest value to add to the history.</param>
		/// <returns>
		/// The oldest value which was returned.
		/// </returns>
		protected T ReplaceOldest (T value)
		{
			T removedValue = this.Values[this.Index];
			this.Values[this.Index] = value;

			if (this.Count < this.Capacity)
			{
				this.Index++;
				this.Count++;
				return default(T);
			}

			this.Index = (this.Index + 1) % this.Capacity;
			return removedValue;
		}

		/// <summary>
		/// Resets all values and clears the history.
		/// </summary>
		/// <param name="capacity">The new capacity of the history.</param>
		/// <param name="initializeWithZero">Specifies whether the values are initialized with zero (true) or <see cref="Double.NaN"/> (false).</param>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than 1.</exception>
		protected void ResetInternal (int capacity, bool initializeWithZero)
		{
			if (capacity < 1)
			{
				throw new ArgumentOutOfRangeException(nameof(capacity));
			}

			this.Capacity = capacity;
			this.InitializeWithZero = initializeWithZero;

			this.Values = new T[capacity];
			this.Index = 0;
			this.Count = 0;
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
		public IEnumerable<T> GetHistory ()
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
		/// Calculates statistics about all values still in the history.
		/// </summary>
		/// <returns>
		/// The statistics about all values still in the history.
		/// </returns>
		public abstract Statistics GetStatistics ();
	}
}