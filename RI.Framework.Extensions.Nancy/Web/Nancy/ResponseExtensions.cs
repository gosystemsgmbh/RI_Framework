using System;
using System.Globalization;
using System.Linq;
using System.Text;

using Nancy;




namespace RI.Framework.Web.Nancy
{
	/// <summary>
	///     Provides utility/extension methods for the <see cref="Response" /> type.
	/// </summary>
	public static class ResponseExtensions
	{
		#region Constants

		private const string EncodingContentTypeParameter = "charset=";

		private static readonly string[] SupportedEncodingContentTypes = new[] {"text/plain", "text/html", "text/css", "application/xml", "application/xhtml+xml", "application/javascript", "application/json", "application/base64",};

		#endregion




		#region Static Methods

		/// <summary>
		///     Adds last modified and thus change check information to a response.
		/// </summary>
		/// <param name="response"> The response. </param>
		/// <param name="timestamp"> The timestamp of the last change ot the requetsed resource or null if the current date and time is used. </param>
		/// <returns>
		///     The response.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="response" /> is null. </exception>
		public static Response WithChangeCheck (this Response response, DateTime? timestamp = null)
		{
			if (response == null)
			{
				throw new ArgumentNullException(nameof(response));
			}

			timestamp = timestamp?.ToUniversalTime() ?? DateTime.UtcNow;

			response.Headers["ETag"] = timestamp.Value.Ticks.ToString("x", CultureInfo.InvariantCulture);
			response.Headers["Last-Modified"] = timestamp.Value.ToString("R", CultureInfo.InvariantCulture);
			return response;
		}

		/// <summary>
		///     Adds encoding information to a response.
		/// </summary>
		/// <param name="response"> The response. </param>
		/// <param name="encoding"> The encoding. </param>
		/// <returns>
		///     The response.
		/// </returns>
		/// <remarks>
		///     <para>
		///         <see cref="WithEncoding" /> detects whether the response is a text response and is missing its charset content type parameter.
		///         It then adds the charset content type parameter based on the specified encoding.
		///     </para>
		///     <para>
		///         The following content types are supported (others are ignored and left unchanged):
		///         <c> text/plain </c>, <c> text/html </c>, <c> text/css </c>, <c> application/xml </c>, <c> application/xhtml+xml </c>, <c> application/javascript </c>, <c> application/json </c>, <c> application/base64 </c>.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="response" /> or <paramref name="encoding" /> is null. </exception>
		public static Response WithEncoding (this Response response, Encoding encoding)
		{
			if (response == null)
			{
				throw new ArgumentNullException(nameof(response));
			}

			if (encoding == null)
			{
				throw new ArgumentNullException(nameof(encoding));
			}

			string contentType = response.ContentType;
			if (!ResponseExtensions.SupportedEncodingContentTypes.Any(x => contentType.StartsWith(x, StringComparison.OrdinalIgnoreCase)))
			{
				return response;
			}

			if (contentType.IndexOf(ResponseExtensions.EncodingContentTypeParameter, StringComparison.OrdinalIgnoreCase) != -1)
			{
				return response;
			}

			response.ContentType = contentType + "; " + ResponseExtensions.EncodingContentTypeParameter + encoding.WebName;
			return response;
		}

		#endregion
	}
}
