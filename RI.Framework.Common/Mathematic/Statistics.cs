using System;
using System.Collections.Generic;

using RI.Framework.Collections.DirectLinq;
using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Mathematic
{
	/// <summary>
	/// Contains discrete and continuous statistics for a sequence of numbers.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Discrete statistics implicitly use a fixed timestep of 1.0 while the continuous statistics use weigthed values based on their timestep.
	/// The weighted value is the value multiplied by its corresponding timestep.
	/// </para>
	/// <para>
	/// Use <see cref="StatisticValues"/> if you only need discrete or continuous statistics but not both.
	/// </para>
	/// <remarks>
	/// <note type="note">
	/// For performance sensitive scenarios, consider using <see cref="RunningValues"/>.
	/// </note>
	/// </remarks>
	/// </remarks>
	public struct Statistics : ICloneable<Statistics>, ICloneable
	{
		/// <summary>
		/// Creates a new instance of <see cref="Statistics"/>.
		/// </summary>
		/// <param name="values">The sequence of numbers the statistics are calculated from. </param>
		/// <remarks>
		/// <para>
		/// <paramref name="values"/> is enumerated only once.
		/// </para>
		/// <para>
		/// As no timestep is provided, the timestep is implicitly assumed to be 1.0, leading to <see cref="Discrete"/> and <see cref="Continuous"/> containing the same values.
		/// </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"><paramref name="values"/> is null.</exception>
		public Statistics (IEnumerable<double> values)
		{
			if (values == null)
			{
				throw new ArgumentNullException(nameof(values));
			}

			this.Discrete = new StatisticValues(values);
			this.Continuous = this.Discrete.Clone();
		}

		/// <summary>
		/// Creates a new instance of <see cref="Statistics"/>.
		/// </summary>
		/// <param name="values">The sequence of numbers the statistics are calculated from. </param>
		/// <param name="fixedTimestep">The fixed timestep which is used for all values.</param>
		/// <remarks>
		/// <para>
		/// <paramref name="values"/> is enumerated only once.
		/// </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"><paramref name="values"/> is null.</exception>
		/// <exception cref="NotFiniteArgumentException"><paramref name="fixedTimestep"/> is NaN or infinity.</exception>
		public Statistics (IEnumerable<double> values, double fixedTimestep)
		{
			if (values == null)
			{
				throw new ArgumentNullException(nameof(values));
			}

			if (fixedTimestep.IsNanOrInfinity())
			{
				throw new NotFiniteArgumentException(nameof(fixedTimestep));
			}

			double[] valueArray = values.ToArray();

			this.Discrete = new StatisticValues(valueArray);
			this.Continuous = new StatisticValues(valueArray, fixedTimestep);
		}

		/// <summary>
		/// Creates a new instance of <see cref="Statistics"/>.
		/// </summary>
		/// <param name="values">The sequence of numbers the statistics are calculated from. </param>
		/// <param name="timesteps">The sequence of timesteps for each value.</param>
		/// <remarks>
		/// <para>
		/// <paramref name="values"/> is enumerated only once.
		/// </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"><paramref name="values"/> or <paramref name="timesteps"/> is null.</exception>
		/// <exception cref="ArgumentException">The number of timesteps does not match the number of values.</exception>
		public Statistics (IEnumerable<double> values, IEnumerable<double> timesteps)
		{
			if (values == null)
			{
				throw new ArgumentNullException(nameof(values));
			}

			if (timesteps == null)
			{
				throw new ArgumentNullException(nameof(timesteps));
			}

			double[] valueArray = values.ToArray();

			this.Discrete = new StatisticValues(valueArray);
			this.Continuous = new StatisticValues(valueArray, timesteps);
		}

		/// <summary>
		/// The computed discrete statistical values.
		/// </summary>
		public StatisticValues Discrete;

		/// <summary>
		/// The computed continuous statistical values.
		/// </summary>
		public StatisticValues Continuous;

		/// <inheritdoc />
		public Statistics Clone ()
		{
			Statistics clone = new Statistics();

			clone.Discrete = this.Discrete.Clone();
			clone.Continuous = this.Continuous.Clone();

			return clone;
		}

		/// <inheritdoc />
		object ICloneable.Clone ()
		{
			return this.Clone();
		}
	}
}