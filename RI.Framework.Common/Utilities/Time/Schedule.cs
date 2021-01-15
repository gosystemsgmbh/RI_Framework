using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

using RI.Framework.Collections;
using RI.Framework.Collections.Comparison;
using RI.Framework.Collections.DirectLinq;
using RI.Framework.Utilities.Exceptions;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Utilities.Time
{
    /// <summary>
    ///     Type to store schedule information about occurences of an event.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The term &quot;event&quot; in the context of a <see cref="Schedule" /> is not referring to an actual event of a .NET type but merely means &quot;events&quot; in its generic meaning that something occurs or happens which can be scheduled using the <see cref="Schedule" /> type.
    ///     </para>
    ///     <note type="important">
    ///         <see cref="Schedule" /> exclusively uses UTC time and dates to avoid all sort of problems with daylight saving time (DST) and timezones.
    ///         Therefore, be aware that all <see cref="DateTime" />s exposed by <see cref="Schedule" /> are UTC timestamps.
    ///         Consequently, all <see cref="DateTime" />s passed to <see cref="Schedule" /> are converted to UTC timestamps (using <see cref="DateTime.ToUniversalTime" />).
    ///     </note>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    /// TODO: Add support for skipping (e.g. each n-th event)
    /// TODO: Add IComparable
    [Serializable]
    public struct Schedule : IEquatable<Schedule>, ICloneable<Schedule>, ICloneable, IFormattable
    {
        #region Constants

        private const char StringSeparator = ',';

        private const char TimeOfDaySeparator = '-';

        private const char TimestampSeparator = '-';

        #endregion




        #region Static Methods

        /// <summary>
        ///     Creates a schedule which schedules an event to occur once per day at a specified time of day.
        /// </summary>
        /// <param name="start"> The time of day. </param>
        /// <returns>
        ///     The schedule.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"> <paramref name="start" /> is not a valid time of day. </exception>
        public static Schedule Daily (TimeSpan start)
        {
            if (!start.IsValidTimeOfDay())
            {
                throw new ArgumentOutOfRangeException(nameof(start));
            }

            Schedule schedule = new Schedule();
            schedule.Type = ScheduleType.Daily;
            schedule.Timestamp = null;
            schedule.Start = start;
            schedule.Stop = null;
            schedule.WeekDays = null;
            return schedule;
        }

        /// <summary>
        ///     Creates a schedule which schedules an event to occur once per day within a given time frame.
        /// </summary>
        /// <param name="start"> The earliest time of day the event can occur. </param>
        /// <param name="stop"> The latest time of day the event can occur. </param>
        /// <returns>
        ///     The schedule.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"> <paramref name="start" /> or <paramref name="stop" /> is not a valid time of day. </exception>
        public static Schedule Daily (TimeSpan start, TimeSpan stop)
        {
            if (!start.IsValidTimeOfDay())
            {
                throw new ArgumentOutOfRangeException(nameof(start));
            }

            if (!stop.IsValidTimeOfDay())
            {
                throw new ArgumentOutOfRangeException(nameof(stop));
            }

            Schedule schedule = new Schedule();
            schedule.Type = ScheduleType.Daily;
            schedule.Timestamp = null;
            schedule.Start = start;
            schedule.Stop = stop;
            schedule.WeekDays = null;
            return schedule;
        }

        /// <summary>
        ///     Creates a schedule which is due now.
        /// </summary>
        /// <returns>
        ///     The schedule.
        /// </returns>
        public static Schedule Once () => Schedule.Once(DateTime.UtcNow);

        /// <summary>
        ///     Creates a schedule which schedules an event to occur once relative to now.
        /// </summary>
        /// <param name="delay"> The delay from now. </param>
        /// <returns>
        ///     The schedule.
        /// </returns>
        public static Schedule Once (TimeSpan delay) => Schedule.Once(DateTime.UtcNow.Add(delay));

        /// <summary>
        ///     Creates a schedule which schedules an event to occur once at a specified date and time.
        /// </summary>
        /// <param name="timestamp"> The date and time. </param>
        /// <returns>
        ///     The schedule.
        /// </returns>
        public static Schedule Once (DateTime timestamp)
        {
            Schedule schedule = new Schedule();
            schedule.Type = ScheduleType.Once;
            schedule.Timestamp = timestamp.ToUniversalTime();
            schedule.Start = null;
            schedule.Stop = null;
            schedule.WeekDays = null;
            return schedule;
        }

        /// <summary>
        ///     Parses a string as a schedule.
        /// </summary>
        /// <param name="str"> The string to parse. </param>
        /// <returns>
        ///     The schedule.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         The threads current culture is used as format provider.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="str" /> is null. </exception>
        /// <exception cref="FormatException"> <paramref name="str" /> is not a valid schedule. </exception>
        public static Schedule Parse (string str) => Schedule.Parse(str, null);

        /// <summary>
        ///     Parses a string as a schedule.
        /// </summary>
        /// <param name="str"> The string to parse. </param>
        /// <param name="formatProvider"> The used format provider or null to use the threads current culture. </param>
        /// <returns>
        ///     The schedule.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="str" /> is null. </exception>
        /// <exception cref="FormatException"> <paramref name="str" /> is not a valid schedule. </exception>
        public static Schedule Parse (string str, IFormatProvider formatProvider)
        {
            Schedule value;
            if (!Schedule.TryParse(str, formatProvider, out value))
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
        /// <remarks>
        ///     <para>
        ///         The threads current culture is used as format provider.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="str" /> is null. </exception>
        public static bool TryParse (string str, out Schedule value) => Schedule.TryParse(str, null, out value);

        /// <summary>
        ///     Tries to parse a string as a schedule.
        /// </summary>
        /// <param name="str"> The string to parse. </param>
        /// <param name="formatProvider"> The used format provider or null to use the threads current culture. </param>
        /// <param name="value"> The parsed schedule. </param>
        /// <returns>
        ///     true if <paramref name="str" /> was a valid schedule, false otherwise.
        /// </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="str" /> is null. </exception>
        public static bool TryParse (string str, IFormatProvider formatProvider, out Schedule value)
        {
            if (str == null)
            {
                throw new ArgumentNullException(nameof(str));
            }

            formatProvider = formatProvider ?? CultureInfo.CurrentCulture;
            DateTimeFormatInfo formatInfo = (DateTimeFormatInfo)formatProvider.GetFormat(typeof(DateTimeFormatInfo)) ?? CultureInfo.CurrentCulture.DateTimeFormat;

            value = new Schedule();

            string[] pieces = str.Split(Schedule.StringSeparator);
            if ((pieces.Length < 2) || (pieces.Length > 5))
            {
                return false;
            }

            ScheduleType? type = Schedule.DecodeType(pieces[0]);
            if (!type.HasValue)
            {
                return false;
            }

            if (type.Value == ScheduleType.Once)
            {
                if (pieces.Length == 2)
                {
                    DateTime? timestamp = Schedule.DecodeTimestamp(pieces[1], Schedule.TimestampSeparator);
                    if (!timestamp.HasValue)
                    {
                        return false;
                    }

                    value = Schedule.Once(timestamp.Value);
                    return true;
                }

                return false;
            }

            if (type.Value == ScheduleType.Daily)
            {
                if (pieces.Length == 2)
                {
                    TimeSpan? start = Schedule.DecodeTimeOfDay(pieces[1], Schedule.TimeOfDaySeparator);
                    if (!start.HasValue)
                    {
                        return false;
                    }

                    value = Schedule.Daily(start.Value);
                    return true;
                }

                if (pieces.Length == 3)
                {
                    TimeSpan? start = Schedule.DecodeTimeOfDay(pieces[1], Schedule.TimeOfDaySeparator);
                    if (!start.HasValue)
                    {
                        return false;
                    }

                    TimeSpan? stop = Schedule.DecodeTimeOfDay(pieces[2], Schedule.TimeOfDaySeparator);
                    if (!stop.HasValue)
                    {
                        return false;
                    }

                    value = Schedule.Daily(start.Value, stop.Value);
                    return true;
                }

                return false;
            }

            if (type.Value == ScheduleType.Weekly)
            {
                if (pieces.Length == 2)
                {
                    List<DayOfWeek> weekDays = Schedule.DecodeWeekDays(pieces[1], formatInfo);
                    if (weekDays == null)
                    {
                        return false;
                    }

                    value = Schedule.Weekly(weekDays);
                    return true;
                }

                if (pieces.Length == 3)
                {
                    List<DayOfWeek> weekDays = Schedule.DecodeWeekDays(pieces[1], formatInfo);
                    if (weekDays == null)
                    {
                        return false;
                    }

                    TimeSpan? start = Schedule.DecodeTimeOfDay(pieces[2], Schedule.TimeOfDaySeparator);
                    if (!start.HasValue)
                    {
                        return false;
                    }

                    value = Schedule.Weekly(weekDays, start.Value);
                    return true;
                }

                if (pieces.Length == 4)
                {
                    List<DayOfWeek> weekDays = Schedule.DecodeWeekDays(pieces[1], formatInfo);
                    if (weekDays == null)
                    {
                        return false;
                    }

                    TimeSpan? start = Schedule.DecodeTimeOfDay(pieces[2], Schedule.TimeOfDaySeparator);
                    if (!start.HasValue)
                    {
                        return false;
                    }

                    TimeSpan? stop = Schedule.DecodeTimeOfDay(pieces[3], Schedule.TimeOfDaySeparator);
                    if (!stop.HasValue)
                    {
                        return false;
                    }

                    value = Schedule.Weekly(weekDays, start.Value, stop.Value);
                    return true;
                }

                return false;
            }

            return false;
        }

        /// <summary>
        ///     Creates a schedule which schedules an event to occur weekly.
        /// </summary>
        /// <param name="weekDays"> The sequence of weekdays on which the event should occur. </param>
        /// <returns>
        ///     The schedule.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         <paramref name="weekDays" /> is enumerated only once.
        ///     </para>
        /// </remarks>
        public static Schedule Weekly (IEnumerable<DayOfWeek> weekDays)
        {
            Schedule schedule = new Schedule();
            schedule.Type = ScheduleType.Weekly;
            schedule.Timestamp = null;
            schedule.Start = null;
            schedule.Stop = null;
            schedule.WeekDays = new ReadOnlyCollection<DayOfWeek>(new List<DayOfWeek>(weekDays));
            return schedule;
        }

        /// <summary>
        ///     Creates a schedule which schedules an event to occur weekly at a specified time of day.
        /// </summary>
        /// <param name="weekDays"> The sequence of weekdays on which the event should occur. </param>
        /// <param name="start"> The time of day. </param>
        /// <returns>
        ///     The schedule.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         <paramref name="weekDays" /> is enumerated only once.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException"> <paramref name="start" /> is not a valid time of day. </exception>
        public static Schedule Weekly (IEnumerable<DayOfWeek> weekDays, TimeSpan start)
        {
            if (!start.IsValidTimeOfDay())
            {
                throw new ArgumentOutOfRangeException(nameof(start));
            }

            Schedule schedule = new Schedule();
            schedule.Type = ScheduleType.Weekly;
            schedule.Timestamp = null;
            schedule.Start = start;
            schedule.Stop = null;
            schedule.WeekDays = new ReadOnlyCollection<DayOfWeek>(new List<DayOfWeek>(weekDays));
            return schedule;
        }

        /// <summary>
        ///     Creates a schedule which schedules an event to occur weekly within a given time frame.
        /// </summary>
        /// <param name="weekDays"> The sequence of weekdays on which the event should occur. </param>
        /// <param name="start"> The earliest time of day the event can occur. </param>
        /// <param name="stop"> The latest time of day the event can occur. </param>
        /// <returns>
        ///     The schedule.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         <paramref name="weekDays" /> is enumerated only once.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException"> <paramref name="start" /> or <paramref name="stop" /> is not a valid time of day. </exception>
        public static Schedule Weekly (IEnumerable<DayOfWeek> weekDays, TimeSpan start, TimeSpan stop)
        {
            if (!start.IsValidTimeOfDay())
            {
                throw new ArgumentOutOfRangeException(nameof(start));
            }

            if (!stop.IsValidTimeOfDay())
            {
                throw new ArgumentOutOfRangeException(nameof(stop));
            }

            Schedule schedule = new Schedule();
            schedule.Type = ScheduleType.Weekly;
            schedule.Timestamp = null;
            schedule.Start = start;
            schedule.Stop = stop;
            schedule.WeekDays = new ReadOnlyCollection<DayOfWeek>(new List<DayOfWeek>(weekDays));
            return schedule;
        }

        private static TimeSpan? DecodeTimeOfDay (string str, char separator)
        {
            string[] pieces = str.Split(separator);
            if ((pieces.Length < 3) && (pieces.Length > 4))
            {
                return null;
            }

            int? hours = pieces[0].ToInt32Invariant();
            int? minutes = pieces[1].ToInt32Invariant();
            int? seconds = pieces[2].ToInt32Invariant();
            int? milliseconds = pieces.Length >= 4 ? pieces[1].ToInt32Invariant() : 0;

            if ((hours == null) || (minutes == null) || (seconds == null) || (milliseconds == null))
            {
                return null;
            }

            TimeSpan timeSpan = new TimeSpan(0, hours.Value, minutes.Value, seconds.Value, milliseconds.Value);
            if (!timeSpan.IsValidTimeOfDay())
            {
                return null;
            }

            return timeSpan;
        }

        private static DateTime? DecodeTimestamp (string str, char separator)
        {
            return str.ToDateTimeFromSortable(separator);
        }

        private static ScheduleType? DecodeType (string str)
        {
            if (str.Length != 1)
            {
                return null;
            }

            foreach (string name in Enum.GetNames(typeof(ScheduleType)))
            {
                if (name[0] == str[0])
                {
                    return (ScheduleType)Enum.Parse(typeof(ScheduleType), name);
                }
            }

            return null;
        }

        private static List<DayOfWeek> DecodeWeekDays (string str, DateTimeFormatInfo formatInfo)
        {
            List<DayOfWeek> candidates = Schedule.GetSortedWeekDays(formatInfo);
            if (str.Length != candidates.Count)
            {
                return null;
            }

            List<DayOfWeek> weekDays = new List<DayOfWeek>();
            for (int i1 = 0; i1 < candidates.Count; i1++)
            {
                if (str[i1] == '1')
                {
                    weekDays.Add(candidates[i1]);
                }
                else if (str[i1] != '0')
                {
                    return null;
                }
            }
            return weekDays;
        }

        private static string EncodeTimeOfDay (TimeSpan timeSpan, char separator)
        {
            StringBuilder str = new StringBuilder(20);

            str.Append(timeSpan.Hours.ToString("D2", CultureInfo.InvariantCulture));
            str.Append(separator);
            str.Append(timeSpan.Minutes.ToString("D2", CultureInfo.InvariantCulture));
            str.Append(separator);
            str.Append(timeSpan.Seconds.ToString("D2", CultureInfo.InvariantCulture));
            str.Append(separator);
            str.Append(timeSpan.Milliseconds.ToString("D3", CultureInfo.InvariantCulture));

            return str.ToString();
        }

        private static string EncodeTimestamp (DateTime timestamp, char separator)
        {
            return timestamp.ToSortableString(separator);
        }

        private static string EncodeType (ScheduleType type)
        {
            return new string(type.ToString()[0], 1);
        }

        private static string EncodeWeekDays (IList<DayOfWeek> weekDays, DateTimeFormatInfo formatInfo)
        {
            StringBuilder str = new StringBuilder();

            List<DayOfWeek> candidates = Schedule.GetSortedWeekDays(formatInfo);
            foreach (DayOfWeek candidate in candidates)
            {
                str.Append(weekDays.Contains(candidate) ? '1' : '0');
            }

            return str.ToString();
        }

        private static void GetDayWindow (DateTime now, TimeSpan todStart, TimeSpan todStop, out DateTime start, out DateTime stop, out DateTime nextStart)
        {
            bool stopIsOnNextDay = todStop < todStart;
            TimeSpan duration = stopIsOnNextDay ? TimeSpan.FromHours(24).Subtract(todStart).Add(todStop) : todStop.Subtract(todStart);

            DateTime today = now.Date;
            DateTime tomorrow = now.AddDays(1).Date;

            DateTime todayStop = today.Add(todStop);
            DateTime tomorrowStop = tomorrow.Add(todStop);

            DateTime windowEnd = now <= todayStop ? todayStop : tomorrowStop;

            start = windowEnd.Subtract(duration);
            stop = windowEnd;

            nextStart = start.AddDays(1);
        }

        private static int GetNextPossibleWeekDayOffset (IList<DayOfWeek> weekDays, DateTime date, bool ignoreFirstResult)
        {
            List<DayOfWeek> candidates = ((DayOfWeek[])Enum.GetValues(typeof(DayOfWeek))).ToList();
            candidates.Sort();
            int weekDayIndex = candidates.IndexOf(date.DayOfWeek);
            List<DayOfWeek> daysToMove = new List<DayOfWeek>();
            for (int i1 = 0; i1 < weekDayIndex; i1++)
            {
                daysToMove.Add(candidates[0]);
                candidates.RemoveAt(0);
            }
            candidates.AddRange(daysToMove);
            for (int i1 = 0; i1 < candidates.Count; i1++)
            {
                if (weekDays.Contains(candidates[i1]))
                {
                    if (!ignoreFirstResult)
                    {
                        return i1;
                    }
                    ignoreFirstResult = false;
                }
            }
            throw new InvalidStateOrExecutionPathException();
        }

        private static List<DayOfWeek> GetSortedWeekDays (DateTimeFormatInfo formatInfo)
        {
            List<DayOfWeek> weekDays = ((DayOfWeek[])Enum.GetValues(typeof(DayOfWeek))).ToList();
            weekDays.Sort();
            int firstDayIndex = weekDays.IndexOf(formatInfo.FirstDayOfWeek);
            List<DayOfWeek> daysToMove = new List<DayOfWeek>();
            for (int i1 = 0; i1 < firstDayIndex; i1++)
            {
                daysToMove.Add(weekDays[0]);
                weekDays.RemoveAt(0);
            }
            weekDays.AddRange(daysToMove);
            return weekDays;
        }

        #endregion




        #region Instance Properties/Indexer

        /// <summary>
        ///     Gets the earliest time of day when an event can occur.
        /// </summary>
        /// <value>
        ///     The earliest time of day when an event can occur or null if that information is not available/used.
        /// </value>
        public TimeSpan? Start { get; private set; }

        /// <summary>
        ///     Gets the latest time of day when an event can occur.
        /// </summary>
        /// <value>
        ///     The latest time of day when an event can occur or null if that information is not available/used.
        /// </value>
        public TimeSpan? Stop { get; private set; }

        /// <summary>
        ///     Gets the timestamp when an event occurs.
        /// </summary>
        /// <value>
        ///     The timestamp when an event occurs or null if that information is not available/used.
        /// </value>
        public DateTime? Timestamp { get; private set; }

        /// <summary>
        ///     Gets the type of the schedule.
        /// </summary>
        /// <value>
        ///     The type of the schedule.
        /// </value>
        public ScheduleType Type { get; private set; }

        /// <summary>
        ///     Gets the read-only list of weekdays on which the event should occur.
        /// </summary>
        /// <value>
        ///     The read-only list of weekdays on which the event should occur or null if that information is not available/used.
        /// </value>
        /// TODO: Replace type (ReadOnlyCollection is not serializable)
        public ReadOnlyCollection<DayOfWeek> WeekDays { get; private set; }

        #endregion




        #region Instance Methods

        /// <summary>
        ///     Checks whether an event is due according to this schedule, given the current time and the assumption that the event never occurred before.
        /// </summary>
        /// <param name="now"> The current date and time. </param>
        /// <returns>
        ///     true if the event is due, false otherwise.
        /// </returns>
        public bool IsDue (DateTime now)
        {
            return this.IsDue(now, null, out _);
        }

        /// <summary>
        ///     Checks whether an event is due according to this schedule, given the current time and the events last occurence.
        /// </summary>
        /// <param name="now"> The current date and time. </param>
        /// <param name="lastOccurence"> The date and time when the event occured the last time or null if the event never occured before. </param>
        /// <returns>
        ///     true if the event is due, false otherwise.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"> <paramref name="lastOccurence" /> comes after <paramref name="now." /> </exception>
        public bool IsDue (DateTime now, DateTime? lastOccurence)
        {
            return this.IsDue(now, lastOccurence, out _);
        }

        /// <summary>
        ///     Checks whether an event is due according to this schedule, given the current time and the events last occurence.
        /// </summary>
        /// <param name="now"> The current date and time. </param>
        /// <param name="lastOccurence"> The date and time when the event occured the last time or null if the event never occured before. </param>
        /// <param name="dueTime"> Returns the amount of time the event is due. See remarks for details. </param>
        /// <returns>
        ///     true if the event is due, false otherwise.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         <paramref name="dueTime" /> returns a positive value if the event is overdue (this method returns true), stating how long ago the event should have occurred, relative to <paramref name="now" /> (as UTC).
        ///         <paramref name="dueTime" /> returns a negative value if the event is not due now but in the future (this method returns false), stating the time to when the event should occur the next time, relative to <paramref name="now" /> (as UTC).
        ///         <paramref name="dueTime" /> returns null if the event is not due now and not in the future (this method returns false).
        ///     </para>
        ///     <para>
        ///         Under very rare circumstances, <paramref name="dueTime" /> can return zero, which is the same as returning a positive value for <paramref name="dueTime" />.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException"> <paramref name="lastOccurence" /> comes after <paramref name="now." /> </exception>
        public bool IsDue (DateTime now, DateTime? lastOccurence, out TimeSpan? dueTime)
        {
            now = now.ToUniversalTime();
            lastOccurence = lastOccurence?.ToUniversalTime();

            if (lastOccurence.HasValue)
            {
                if (lastOccurence.Value > now)
                {
                    throw new ArgumentOutOfRangeException(nameof(lastOccurence));
                }
            }

            switch (this.Type)
            {
                default:
                {
                    throw new InvalidStateOrExecutionPathException();
                }

                case ScheduleType.Once:
                {
                    if (!this.Timestamp.HasValue)
                    {
                        throw new InvalidStateOrExecutionPathException();
                    }

                    if (lastOccurence.HasValue)
                    {
                        dueTime = null;
                        return false;
                    }

                    dueTime = now.Subtract(this.Timestamp.Value);
                    return dueTime.Value.IsPositive();
                }

                case ScheduleType.Daily:
                {
                    if (!this.Start.HasValue)
                    {
                        throw new InvalidStateOrExecutionPathException();
                    }

                    DateTime start;
                    DateTime stop;
                    DateTime nextStart;
                    Schedule.GetDayWindow(now, this.Start.Value, this.Stop.GetValueOrDefault(TimeSpanExtensions.LatestValidTimeOfDay), out start, out stop, out nextStart);

                    if (lastOccurence.HasValue && (lastOccurence.Value >= start) && (lastOccurence.Value <= stop))
                    {
                        dueTime = now.Subtract(nextStart);
                        return false;
                    }

                    if ((now >= start) && (now <= stop))
                    {
                        dueTime = now.Subtract(start);
                        return true;
                    }

                    if (now < start)
                    {
                        dueTime = now.Subtract(start);
                        return false;
                    }

                    if (now > stop)
                    {
                        dueTime = now.Subtract(nextStart);
                        return false;
                    }

                    throw new InvalidStateOrExecutionPathException();
                }

                case ScheduleType.Weekly:
                {
                    if (this.WeekDays == null)
                    {
                        throw new InvalidStateOrExecutionPathException();
                    }

                    if (this.WeekDays.Count == 0)
                    {
                        dueTime = null;
                        return false;
                    }

                    DateTime start;
                    DateTime stop;
                    DateTime nextStart;
                    this.GetWeekWindow(now, this.WeekDays, this.Start.GetValueOrDefault(TimeSpanExtensions.EarliestValidTimeOfDay), this.Stop.GetValueOrDefault(TimeSpanExtensions.LatestValidTimeOfDay), out start, out stop, out nextStart);

                    if (lastOccurence.HasValue && (lastOccurence.Value >= start) && (lastOccurence.Value <= stop))
                    {
                        dueTime = now.Subtract(nextStart);
                        return false;
                    }

                    if ((now >= start) && (now <= stop))
                    {
                        dueTime = now.Subtract(start);
                        return true;
                    }

                    if (now < start)
                    {
                        dueTime = now.Subtract(start);
                        return false;
                    }

                    if (now > stop)
                    {
                        dueTime = now.Subtract(nextStart);
                        return false;
                    }

                    throw new InvalidStateOrExecutionPathException();
                }
            }
        }

        private void GetWeekWindow (DateTime now, IList<DayOfWeek> weekDays, TimeSpan todStart, TimeSpan todStop, out DateTime start, out DateTime stop, out DateTime nextStart)
        {
            start = DateTime.MinValue;
            stop = DateTime.MaxValue;

            bool stopIsOnNextDay = todStop < todStart;
            TimeSpan duration = stopIsOnNextDay ? TimeSpan.FromHours(24).Subtract(todStart).Add(todStop) : todStop.Subtract(todStart);

            bool found = false;
            for (int i1 = 0; i1 < 6; i1++)
            {
                DateTime today = now.AddDays(i1).Date;
                DateTime tomorrow = now.AddDays(i1 + 1).Date;

                DateTime todayStop = today.Add(todStop);
                DateTime tomorrowStop = tomorrow.Add(todStop);

                DateTime windowEnd = now <= todayStop ? todayStop : tomorrowStop;

                start = windowEnd.Subtract(duration);
                stop = windowEnd;

                if (weekDays.Contains(start.DayOfWeek))
                {
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                throw new InvalidStateOrExecutionPathException();
            }

            int nextPossibleWeekDayOffset = Schedule.GetNextPossibleWeekDayOffset(weekDays, start, true);
            nextStart = start.AddDays(nextPossibleWeekDayOffset);
        }

        #endregion




        #region Overrides

        /// <inheritdoc />
        public override bool Equals (object obj)
        {
            return obj is Schedule ? this.Equals((Schedule)obj) : false;
        }

        /// <inheritdoc />
        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
        public override int GetHashCode ()
        {
            long count = 4;
            long hashCode = this.Type.GetHashCode();
            hashCode += this.Timestamp?.GetHashCode() ?? 0;
            hashCode += this.Start?.GetHashCode() ?? 0;
            hashCode += this.Stop?.GetHashCode() ?? 0;
            this.WeekDays?.ForEach(x =>
            {
                hashCode += (int)x;
                count++;
            });
            return (int)(hashCode / count);
        }

        /// <inheritdoc />
        public override string ToString () => this.ToString(null, null);

        #endregion




        #region Interface: ICloneable<Schedule>

        /// <inheritdoc />
        public Schedule Clone ()
        {
            Schedule clone = new Schedule();
            clone.Type = this.Type;
            clone.Timestamp = this.Timestamp;
            clone.Start = this.Start;
            clone.Stop = this.Stop;
            clone.WeekDays = new ReadOnlyCollection<DayOfWeek>(this.WeekDays);
            return clone;
        }

        /// <inheritdoc />
        object ICloneable.Clone ()
        {
            return this.Clone();
        }

        #endregion




        #region Interface: IEquatable<Schedule>

        /// <inheritdoc />
        public bool Equals (Schedule other)
        {
            if (this.Type != other.Type)
            {
                return false;
            }

            if (this.Timestamp != other.Timestamp)
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

            if (!CollectionComparer<DayOfWeek>.DefaultIgnoreOrder.Equals(this.WeekDays, other.WeekDays))
            {
                return false;
            }

            return true;
        }

        #endregion




        #region Interface: IFormattable

        /// <inheritdoc />
        public string ToString (string format, IFormatProvider formatProvider)
        {
            formatProvider = formatProvider ?? CultureInfo.CurrentCulture;
            DateTimeFormatInfo formatInfo = (DateTimeFormatInfo)formatProvider.GetFormat(typeof(DateTimeFormatInfo)) ?? CultureInfo.CurrentCulture.DateTimeFormat;

            StringBuilder sb = new StringBuilder(100);
            sb.Append(Schedule.EncodeType(this.Type));
            if (this.Timestamp.HasValue)
            {
                sb.Append(Schedule.StringSeparator);
                sb.Append(Schedule.EncodeTimestamp(this.Timestamp.Value, Schedule.TimestampSeparator));
            }
            if (this.Start.HasValue)
            {
                sb.Append(Schedule.StringSeparator);
                sb.Append(Schedule.EncodeTimeOfDay(this.Start.Value, Schedule.TimeOfDaySeparator));
            }
            if (this.Stop.HasValue)
            {
                sb.Append(Schedule.StringSeparator);
                sb.Append(Schedule.EncodeTimeOfDay(this.Stop.Value, Schedule.TimeOfDaySeparator));
            }
            if (this.WeekDays != null)
            {
                sb.Append(Schedule.StringSeparator);
                sb.Append(Schedule.EncodeWeekDays(this.WeekDays, formatInfo));
            }
            return sb.ToString();
        }

        #endregion
    }
}
