using System;
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
		private static readonly string[] SupportedEncodingContentTypes = new[]
		{
			"text/plain",
			"text/html",
			"text/css",
			"application/xml",
			"application/xhtml+xml",
			"application/javascript",
			"application/json",
			"application/base64",
		};

		private const string EncodingContentTypeParameter = "charset=";

		/// <summary>
		/// Adds encoding information to a response.
		/// </summary>
		/// <param name="response"></param>
		/// <param name="encoding"></param>
		/// <returns></returns>
		/// <remarks>
		/// <para>
		/// <see cref="WithEncoding"/> detects whether the response is a text response and is missing its charset content type parameter.
		/// It then adds the charset content type parameter based on the specified encoding.
		/// </para>
		/// <para>
		/// The following content types are supported (others are ignored and left unchanged):
		/// <c>text/plain</c>, <c>text/html</c>, <c>text/css</c>, <c>application/xml</c>, <c>application/xhtml+xml</c>, <c>application/javascript</c>, <c>application/json</c>, <c>application/base64</c>.
		/// </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"><paramref name="response"/> or <paramref name="encoding"/> is null.</exception>
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
	}
}
