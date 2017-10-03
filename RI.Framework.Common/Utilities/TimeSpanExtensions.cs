using System;
using System.Globalization;
using System.Text;




namespace RI.Framework.Utilities
{
	/// <summary>
	///     Provides utility/extension methods for the <see cref="TimeSpan" /> type.
	/// </summary>
	public static class TimeSpanExtensions
	{
		#region Static Methods

		/// <summary>
		///     Converts a time span into a sortable string.
		/// </summary>
		/// <param name="timeSpan"> The time span. </param>
		/// <returns>
		///     The time span as a sortable string in the following format %dhhmmssfff with no separator between the units.
		///     Example: <c> 1143050333 </c>
		/// </returns>
		public static string ToSortableString (this TimeSpan timeSpan)
		{
			return timeSpan.ToSortableString(null);
		}

		/// <summary>
		///     Converts a time span into a sortable string.
		/// </summary>
		/// <param name="timeSpan"> The time span. </param>
		/// <param name="separator"> The separator between each unit of the time span. </param>
		/// <returns>
		///     The time span as a sortable string in the following format %d-hh-mm-ss-fff where the dash is the used separator.
		///     Example: <c> 1_14_30_50_333 </c> when used with the underscore as a separator.
		/// </returns>
		public static string ToSortableString (this TimeSpan timeSpan, char separator)
		{
			return timeSpan.ToSortableString(new string(separator, 1));
		}

		/// <summary>
		///     Converts a time span into a sortable string.
		/// </summary>
		/// <param name="timeSpan"> The time span. </param>
		/// <param name="separator"> The separator between each unit of the time span. Can be null to use no separator. </param>
		/// <returns>
		///     The time span as a sortable string in the following format %d-hh-mm-ss-fff where the dash is the used separator.
		///     Example: <c> 1_14_30_50_333 </c> when used with the underscore as a separator.
		/// </returns>
		public static string ToSortableString (this TimeSpan timeSpan, string separator)
		{
			separator = separator ?? string.Empty;

			StringBuilder dateTimeString = new StringBuilder(20);

			dateTimeString.Append(timeSpan.Days.ToString("D", CultureInfo.InvariantCulture));
			dateTimeString.Append(separator);
			dateTimeString.Append(timeSpan.Hours.ToString("D2", CultureInfo.InvariantCulture));
			dateTimeString.Append(separator);
			dateTimeString.Append(timeSpan.Minutes.ToString("D2", CultureInfo.InvariantCulture));
			dateTimeString.Append(separator);
			dateTimeString.Append(timeSpan.Seconds.ToString("D2", CultureInfo.InvariantCulture));
			dateTimeString.Append(separator);
			dateTimeString.Append(timeSpan.Milliseconds.ToString("D3", CultureInfo.InvariantCulture));

			return dateTimeString.ToString();
		}

		/// <summary>
		/// Determines whether a time span is negative.
		/// </summary>
		/// <param name="timeSpan">The time span.</param>
		/// <returns>
		/// true if the time span is negative, false otherwise.
		/// </returns>
		/// <remarks>
		/// <note type="note">
		/// Zero is considered not negative.
		/// </note>
		/// </remarks>
		public static bool IsNegative (this TimeSpan timeSpan) => timeSpan.Ticks < 0;

		/// <summary>
		/// Determines whether a time span is positive.
		/// </summary>
		/// <param name="timeSpan">The time span.</param>
		/// <returns>
		/// true if the time span is positive, false otherwise.
		/// </returns>
		/// <remarks>
		/// <note type="note">
		/// Zero is considered positive.
		/// </note>
		/// </remarks>
		public static bool IsPositive(this TimeSpan timeSpan) => timeSpan.Ticks >= 0;

		/// <summary>
		/// Determines whether a time span is zero.
		/// </summary>
		/// <param name="timeSpan">The time span.</param>
		/// <returns>
		/// true if the time span is zero, false otherwise.
		/// </returns>
		public static bool IsZero(this TimeSpan timeSpan) => timeSpan.Ticks == 0;

		#endregion
	}
}
