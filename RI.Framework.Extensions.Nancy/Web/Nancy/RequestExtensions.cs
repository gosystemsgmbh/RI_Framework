using System;
using System.Globalization;
using System.Linq;

using Nancy;




namespace RI.Framework.Web.Nancy
{
	/// <summary>
	///     Provides utility/extension methods for the <see cref="Request" /> type.
	/// </summary>
	public static class RequestExtensions
	{
		#region Static Methods

		/// <summary>
		///     Checks whether a request requests a modified resource.
		/// </summary>
		/// <param name="request"> The request. </param>
		/// <param name="timestamp"> The timestamp of the last time the requested resource was modified. </param>
		/// <returns>
		///     true if the requested resource was modified and should be resend, false otherwise.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="request" /> is null. </exception>
		public static bool IsModified (this Request request, DateTime timestamp)
		{
			timestamp = timestamp.ToUniversalTime();
			string etag = timestamp.Ticks.ToString("x", CultureInfo.InvariantCulture);

			RequestHeaders requestHeaders = request.Headers;

			if (requestHeaders.IfNoneMatch.Contains(etag))
			{
				return false;
			}

			if (!requestHeaders.IfModifiedSince.HasValue)
			{
				return true;
			}

			return timestamp > requestHeaders.IfModifiedSince.Value.ToUniversalTime();
		}

		#endregion
	}
}
