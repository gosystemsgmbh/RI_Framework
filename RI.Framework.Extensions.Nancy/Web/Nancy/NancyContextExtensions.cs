using System;
using System.Globalization;
using System.Linq;

using Nancy;

using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;




namespace RI.Framework.Web.Nancy
{
    /// <summary>
    ///     Provides utility/extension methods for the <see cref="NancyContext" /> type.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    public static class NancyContextExtensions
    {
        #region Static Methods

        /// <summary>
        ///     Logs the current state of a context.
        /// </summary>
        /// <param name="message"> The log message. </param>
        /// <param name="context"> The context whose current state information is appended to the log message. </param>
        /// <param name="options"> Context logging options. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="context" /> or <paramref name="message" /> is null. </exception>
        /// <exception cref="EmptyStringArgumentException"> <paramref name="message" /> is an empty string. </exception>
        public static string CreateContextLog (this NancyContext context, string message, NancyContextLogOptions options)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (message.IsNullOrEmptyOrWhitespace())
            {
                throw new EmptyStringArgumentException(nameof(message));
            }

            const string timeMeasurementItemName = "TimeMeasurement-399059CD-BAB9-4E96-8678-1A9CB0C3D8BA";

            bool startTimeMeasurement = (options & NancyContextLogOptions.StartTimeMeasurement) == NancyContextLogOptions.StartTimeMeasurement;
            bool measureTime = (options & NancyContextLogOptions.MeasureTime) == NancyContextLogOptions.MeasureTime;
            bool includeResponse = ((options & NancyContextLogOptions.IncludeResponse) == NancyContextLogOptions.IncludeResponse) && (context.Response != null);

            string method = context.Request.Method;
            string url = context.Request.Url;
            string userHostAddress = context.Request.UserHostAddress;
            string requestHeaders = context.Request.Headers.SelectMany(x => x.Value.Select(y => x.Key + "=" + y)).Join("; ");

            string statusCode = string.Empty;
            string contentType = string.Empty;
            string user = string.Empty;
            string responseHeaders = string.Empty;
            if (includeResponse)
            {
                statusCode = context.Response.StatusCode.ToString();
                contentType = context.Response.ContentType;
                user = context.CurrentUser?.UserName == null ? "(anonymous)" : (context.CurrentUser.UserName + (context.CurrentUser.Claims == null ? string.Empty : " " + context.CurrentUser.Claims.Join(";"))).Trim();
                responseHeaders = context.Response.Headers.Select(x => x.Key + "=" + x.Value).Join("; ");
            }

            DateTime now = DateTime.UtcNow;

            DateTime? startTime = context.Items.ContainsKey(timeMeasurementItemName) ? (DateTime)context.Items[timeMeasurementItemName] : (DateTime?)null;
            int? processingTime = (startTime.HasValue && measureTime) ? (int)now.Subtract(startTime.Value).TotalMilliseconds : (int?)null;

            string format = message;

            if (!includeResponse)
            {
                format = format + "; {0} [{1}] FROM [{2}] HEADERS [{3}]";
            }
            else
            {
                format = format + "; {0} [{1}] FROM [{2}] WITH [{4}] AS [{5}] FOR [{6}] HEADERS [{7}]";
            }

            if (processingTime.HasValue)
            {
                format = format + " WITHIN [{8} ms]";
            }

            if (startTimeMeasurement)
            {
                context.Items.Remove(timeMeasurementItemName);
                context.Items.Add(timeMeasurementItemName, now);
            }

            string log = string.Format(CultureInfo.InvariantCulture, format, method, url, userHostAddress, requestHeaders, statusCode, contentType, user, responseHeaders, processingTime);
            return log;
        }

        #endregion
    }
}
