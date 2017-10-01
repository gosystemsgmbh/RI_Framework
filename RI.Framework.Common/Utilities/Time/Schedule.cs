using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;

using RI.Framework.Utilities.ObjectModel;

namespace RI.Framework.Utilities.Time
{
	/// <summary>
	/// Type to store schedule information about occurences of an event.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The term &quot;event&quot; in the context of a <see cref="Schedule"/> is not referring to an actual event of a .NET type but merely means &quot;events&quot; in its generic meaning that something occurs or happens which can be scheduled using the <see cref="Schedule"/> type.
	/// </para>
	/// <note type="important">
	/// <see cref="Schedule"/> exclusively uses UTC time and dates to avoid all sort of problems with daylight saving time (DST) and timezones.
	/// Therefore, be aware that all <see cref="DateTime"/>s exposed by <see cref="Schedule"/> are UTC timestamps.
	/// Consequently, all <see cref="DateTime"/>s passed to <see cref="Schedule"/> are converted to UTC timestamps (using <see cref="DateTime.ToUniversalTime"/>).
	/// </note>
	/// </remarks>
	[Serializable]
	public struct Schedule : IEquatable<Schedule>, ICloneable<Schedule>, ICloneable
	{
		private const char DateTimeSeparator = '-';

		private const char TimeSpanSeparator = '-';

		private const char StringSeparator = '-';

		/// <summary>
		/// Creates a schedule which schedules an event to occur once in the future.
		/// </summary>
		/// <param name="delay">The delay from now.</param>
		/// <returns>
		/// The schedule.
		/// </returns>
		public static Schedule Once(TimeSpan delay) => Schedule.Once(DateTime.UtcNow.Add(delay));

		/// <summary>
		/// Creates a schedule which schedules an event to occur once at a specified date and time.
		/// </summary>
		/// <param name="start">The date and time.</param>
		/// <returns>
		/// The schedule.
		/// </returns>
		public static Schedule Once(DateTime start)
		{
			Schedule schedule = new Schedule();
			schedule.Type = ScheduleType.Once;
			schedule.Start = start.ToUniversalTime();
			schedule.Stop = null;
			schedule.Interval = null;
			return schedule;
		}

		/// <summary>
		/// Creates a schedule which schedules an event to occur once per day at a specified time of day.
		/// </summary>
		/// <param name="start">The time of day.</param>
		/// <returns>
		/// The schedule.
		/// </returns>
		/// <remarks>
		/// <note type="note">
		/// The date part of <paramref name="start"/> is ignored.
		/// </note>
		/// </remarks>
		public static Schedule Daily(DateTime start)
		{
			Schedule schedule = new Schedule();
			schedule.Type = ScheduleType.Daily;
			schedule.Start = start.ToUniversalTime();
			schedule.Stop = null;
			schedule.Interval = null;
			return schedule;
		}

		/// <summary>
		/// Creates a schedule which schedules an event to occur once per day within a given time frame.
		/// </summary>
		/// <param name="start">The earliest time of day the event can occur.</param>
		/// <param name="stop">The latest time of day the event can occur.</param>
		/// <returns>
		/// The schedule.
		/// </returns>
		/// <remarks>
		/// <note type="note">
		/// The date part of <paramref name="start"/> is ignored.
		/// </note>
		/// </remarks>
		public static Schedule Daily(DateTime start, DateTime stop)
		{
			Schedule schedule = new Schedule();
			schedule.Type = ScheduleType.Daily;
			schedule.Start = start.ToUniversalTime();
			schedule.Stop = stop.ToUniversalTime();
			schedule.Interval = null;
			return schedule;
		}

		/// <summary>
		/// Creates a schedule which schedules an event to occur repeatedly.
		/// </summary>
		/// <param name="interval">The interval between two events.</param>
		/// <returns>
		/// The schedule.
		/// </returns>
		/// <remarks>
		/// <para>
		/// The first occurence of the event is considered to be now plus <paramref name="interval"/>.
		/// </para>
		/// </remarks>
		public static Schedule Repeated(TimeSpan interval)
		{
			Schedule schedule = new Schedule();
			schedule.Type = ScheduleType.Repeated;
			schedule.Start = DateTime.UtcNow.Add(interval);
			schedule.Stop = null;
			schedule.Interval = interval;
			return schedule;
		}

		/// <summary>
		/// Creates a schedule which schedules an event to occur repeatedly once per day at a specified time of day.
		/// </summary>
		/// <param name="interval">The interval between two events.</param>
		/// <param name="start">The time of day.</param>
		/// <returns></returns>
		public static Schedule Repeated(TimeSpan interval, DateTime start)
		{
			Schedule schedule = new Schedule();
			schedule.Type = ScheduleType.Repeated;
			schedule.Start = start.ToUniversalTime();
			schedule.Stop = null;
			schedule.Interval = interval;
			return schedule;
		}

		/// <summary>
		/// Creates a schedule which schedules an event to occur repeatedly once per day within a given time frame.
		/// </summary>
		/// <param name="interval">The interval between two events.</param>
		/// <param name="start">The earliest time of day the event can occur.</param>
		/// <param name="stop">The latest time of day the event can occur.</param>
		/// <returns></returns>
		public static Schedule Repeated(TimeSpan interval, DateTime start, DateTime stop)
		{
			Schedule schedule = new Schedule();
			schedule.Type = ScheduleType.Repeated;
			schedule.Start = start.ToUniversalTime();
			schedule.Stop = stop.ToUniversalTime();
			schedule.Interval = interval;
			return schedule;
		}

		/// <summary>
		/// Gets the type of the schedule.
		/// </summary>
		/// <value>
		/// The type of the schedule.
		/// </value>
		public ScheduleType Type { get; private set; }

		/// <summary>
		/// Gets the information when an event occurs for the first time.
		/// </summary>
		/// <value>
		/// The information when an event occurs for a first time.
		/// </value>
		public DateTime Start { get; private set; }

		/// <summary>
		/// Gets the information when an event occurs for the last time.
		/// </summary>
		/// <value>
		/// The information when an event occurs for the last time or null if that information is not available.
		/// </value>
		public DateTime? Stop { get; private set; }

		/// <summary>
		/// Gets the information about the interval between two events.
		/// </summary>
		/// <value>
		/// The information about the interval between two events or null if that information is not available.
		/// </value>
		public TimeSpan? Interval { get; private set; }

		/// <summary>
		/// Checks whether an event is due according to this schedule, given the current time and the events last occurence.
		/// </summary>
		/// <param name="now">The current date and time.</param>
		/// <param name="lastOccurence">The date and time when the event occured the last time or null if the event never occured before.</param>
		/// <returns>
		/// true if the event is due, false otherwise.
		/// </returns>
		public bool IsDue (DateTime now, DateTime? lastOccurence)
		{
			TimeSpan overdue;
			return this.IsDue(now, lastOccurence, out overdue);
		}

		/// <summary>
		/// Checks whether an event is due according to this schedule, given the current time and the events last occurence.
		/// </summary>
		/// <param name="now">The current date and time.</param>
		/// <param name="lastOccurence">The date and time when the event occured the last time or null if the event never occured before.</param>
		/// <param name="dueTime">Returns the amount of time the event is due. See remarks for details.</param>
		/// <returns>
		/// true if the event is due, false otherwise.
		/// </returns>
		/// <remarks>
		/// <para>
		/// <paramref name="dueTime"/> returns a positive value if the event is overdue (this method returns true), stating how long ago the event should have occurred, relative to <paramref name="now"/> (as UTC).
		/// <paramref name="dueTime"/> returns a negative value if the event is not due now but in the future (this method returns false), stating the time to when the event should occur the next time, relative to <paramref name="now"/> (as UTC).
		/// <paramref name="dueTime"/> returns null if the event is not due now and not in the future (this method returns false).
		/// </para>
		/// <para>
		/// Under very rare circumstances, <paramref name="dueTime"/> can return zero, which is treated the same as the event is (over)due.
		/// </para>
		/// </remarks>
		public bool IsDue (DateTime now, DateTime? lastOccurence, out TimeSpan? dueTime)
		{
			now = now.ToUniversalTime();
			lastOccurence = lastOccurence?.ToUniversalTime();

			dueTime = null;

			switch (this.Type)
			{
				case ScheduleType.Once:
					break;

				case ScheduleType.Daily:
					break;

				case ScheduleType.Repeated:
					break;
			}

			//TODO: Implement
			throw new NotImplementedException();
		}

		/// <summary>
		///     Parses a string as a schedule.
		/// </summary>
		/// <param name="str"> The string to parse. </param>
		/// <returns>
		///     The schedule.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="str" /> is null. </exception>
		/// <exception cref="FormatException"> <paramref name="str" /> is not a valid schedule. </exception>
		public static Schedule Parse(string str)
		{
			Schedule value;
			if (!Schedule.TryParse(str, out value))
			{
				throw new FormatException("\"" + str + "\" is not a valid schedule.");
			}
			return value;
		}

		/// <summary>
		///     Tries to parse a string as a schedule.
		/// </summary>
		/// <param name="str"> The string to parse. </param>
		/// <param name="value"> The parsed schedule. </param>
		/// <returns>
		///     true if <paramref name="str" /> was a valid schedule, false otherwise.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="str" /> is null. </exception>
		public static bool TryParse(string str, out Schedule value)
		{
			if (str == null)
			{
				throw new ArgumentNullException(nameof(str));
			}

			value = new Schedule();

			string[] pieces = str.Split(Schedule.StringSeparator);
			if ((pieces.Length < 2) || (pieces.Length > 4))
			{
				return false;
			}

			string type = pieces[0].ToUpperInvariant();
			if (type.Length != 1)
			{
				return false;
			}

			DateTime? start = pieces[1].ToDateTimeFromSortable(Schedule.DateTimeSeparator);
			if (!start.HasValue)
			{
				return false;
			}

			if (type[0] == nameof(ScheduleType.Once)[0])
			{
				if (pieces.Length != 2)
				{
					return false;
				}

				value = Schedule.Once(start.Value);
				return true;
			}

			if (type[0] == nameof(ScheduleType.Daily)[0])
			{
				if (pieces.Length == 2)
				{
					value = Schedule.Daily(start.Value);
					return true;
				}

				if (pieces.Length == 3)
				{
					DateTime? stop = pieces[2].ToDateTimeFromSortable(Schedule.DateTimeSeparator);
					if (!stop.HasValue)
					{
						return false;
					}

					value = Schedule.Daily(start.Value, stop.Value);
					return true;
				}

				return false;
			}

			if (type[0] == nameof(ScheduleType.Repeated)[0])
			{
				if (pieces.Length == 3)
				{
					TimeSpan? interval = pieces[2].ToTimeSpanFromSortable(Schedule.TimeSpanSeparator);
					if (!interval.HasValue)
					{
						return false;
					}

					value = Schedule.Repeated(interval.Value, start.Value);
					return true;
				}

				if (pieces.Length == 4)
				{
					DateTime? stop = pieces[2].ToDateTimeFromSortable(Schedule.DateTimeSeparator);
					if (!stop.HasValue)
					{
						return false;
					}

					TimeSpan? interval = pieces[3].ToTimeSpanFromSortable(Schedule.TimeSpanSeparator);
					if (!interval.HasValue)
					{
						return false;
					}

					value = Schedule.Repeated(interval.Value, start.Value, stop.Value);
					return true;
				}

				return false;
			}

			return false;
		}

		/// <inheritdoc />
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder(70);
			sb.Append(this.Type.ToString()[0]);
			sb.Append(Schedule.StringSeparator);
			sb.Append(this.Start.ToSortableString(Schedule.DateTimeSeparator));
			if (this.Stop.HasValue)
			{
				sb.Append(Schedule.StringSeparator);
				sb.Append(this.Stop.Value.ToSortableString(Schedule.DateTimeSeparator));
			}
			if (this.Interval.HasValue)
			{
				sb.Append(Schedule.StringSeparator);
				sb.Append(this.Interval.Value.ToSortableString(Schedule.TimeSpanSeparator));
			}
			return sb.ToString();
		}

		/// <inheritdoc />
		[SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
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
}
