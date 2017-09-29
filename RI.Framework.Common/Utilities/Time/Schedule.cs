using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using RI.Framework.Utilities.ObjectModel;

namespace RI.Framework.Utilities.Time
{
	[Serializable]
	public struct Schedule : IEquatable<Schedule>, ICloneable<Schedule>, ICloneable
	{
		public static Schedule Once(TimeSpan delay) => Schedule.Once(DateTime.UtcNow.Add(delay));

		public static Schedule Once(DateTime start)
		{
			Schedule schedule = new Schedule();
			schedule.Type = ScheduleType.Once;
			schedule.Start = start;
			schedule.Stop = null;
			schedule.Interval = null;
			return schedule;
		}

		public static Schedule Daily(DateTime start)
		{
			Schedule schedule = new Schedule();
			schedule.Type = ScheduleType.Daily;
			schedule.Start = start;
			schedule.Stop = null;
			schedule.Interval = null;
			return schedule;
		}

		public static Schedule Daily(DateTime start, DateTime stop)
		{
			Schedule schedule = new Schedule();
			schedule.Type = ScheduleType.Daily;
			schedule.Start = start;
			schedule.Stop = stop;
			schedule.Interval = null;
			return schedule;
		}

		public static Schedule Repeated(TimeSpan interval)
		{
			Schedule schedule = new Schedule();
			schedule.Type = ScheduleType.Repeated;
			schedule.Start = DateTime.UtcNow;
			schedule.Stop = null;
			schedule.Interval = interval;
			return schedule;
		}

		public static Schedule Repeated(TimeSpan interval, DateTime start)
		{
			Schedule schedule = new Schedule();
			schedule.Type = ScheduleType.Repeated;
			schedule.Start = start;
			schedule.Stop = null;
			schedule.Interval = interval;
			return schedule;
		}

		public static Schedule Repeated(TimeSpan interval, DateTime start, DateTime stop)
		{
			Schedule schedule = new Schedule();
			schedule.Type = ScheduleType.Repeated;
			schedule.Start = start;
			schedule.Stop = stop;
			schedule.Interval = interval;
			return schedule;
		}

		public ScheduleType Type { get; private set; }

		public DateTime Start { get; private set; }

		public DateTime? Stop { get; private set; }

		public TimeSpan? Interval { get; private set; }

		public bool IsDue(DateTime now, DateTime? lastRun)
		{
			throw new NotImplementedException();
		}

		public static Schedule Parse(string str)
		{
			Schedule value;
			if (!Schedule.TryParse(str, out value))
			{
				throw new FormatException("\"" + str + "\" is not a valid schedule.");
			}
			return value;
		}

		public static bool TryParse(string str, out Schedule schedule)
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc />
		public override string ToString()
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc />
		public override int GetHashCode()
		{
			long hashCode = this.Type.GetHashCode();
			hashCode += this.Start.GetHashCode();
			hashCode += this.Stop?.GetHashCode() ?? 0;
			hashCode += this.Interval?.GetHashCode() ?? 0;
			return (int)(hashCode / 4);
		}

		/// <inheritdoc />
		public override bool Equals(object obj)
		{
			return obj is Schedule ? this.Equals((Schedule)obj) : false;
		}

		/// <inheritdoc />
		public bool Equals(Schedule other)
		{
			if (this.Type != other.Type)
			{
				return false;
			}

			if (this.Start != other.Start)
			{
				return false;
			}

			if (this.Stop != other.Stop)
			{
				return false;
			}

			if (this.Interval != other.Interval)
			{
				return false;
			}

			return true;
		}

		/// <inheritdoc />
		public Schedule Clone()
		{
			Schedule clone = new Schedule();
			clone.Type = this.Type;
			clone.Start = this.Start;
			clone.Stop = this.Stop;
			clone.Interval = this.Interval;
			return clone;
		}

		/// <inheritdoc />
		object ICloneable.Clone()
		{
			return this.Clone();
		}

		/// <summary>
		///     Compares two <see cref="Schedule" />s for inequality.
		/// </summary>
		/// <param name="x"> The first value. </param>
		/// <param name="y"> The second value. </param>
		/// <returns>
		///     false if both values are equal, true otherwise.
		/// </returns>
		public static bool operator !=(Schedule x, Schedule y)
		{
			return !(x == y);
		}

		/// <summary>
		///     Compares two <see cref="Schedule" />s for equality.
		/// </summary>
		/// <param name="x"> The first value. </param>
		/// <param name="y"> The second value. </param>
		/// <returns>
		///     true if both values are equal, false otherwise.
		/// </returns>
		public static bool operator ==(Schedule x, Schedule y)
		{
			return x.Equals(y);
		}
	}

	[Serializable]
	public enum ScheduleType
	{
		Once = 0,

		Daily = 1,

		Repeated = 2,
	}
}
