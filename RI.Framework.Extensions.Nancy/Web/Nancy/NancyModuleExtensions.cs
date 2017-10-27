using System;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;

using Nancy;
using Nancy.Responses;

using RI.Framework.IO.Paths;
using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;
using RI.Framework.Utilities.Reflection;




namespace RI.Framework.Web.Nancy
{
	/// <summary>
	///     Provides utility/extension methods for the <see cref="NancyModule" /> type.
	/// </summary>
	public static class NancyModuleExtensions
	{
		#region Static Methods

		/// <summary>
		///     Creates a response delivering binary data from a byte array.
		/// </summary>
		/// <param name="module"> The Nancy module. </param>
		/// <param name="data"> The byte array. </param>
		/// <param name="statusCode"> The optional HTTP status code of the response. The default value is OK. </param>
		/// <returns>
		///     The response.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="module" /> or <paramref name="data" /> is null. </exception>
		public static Response BinaryResponse (this NancyModule module, byte[] data, HttpStatusCode statusCode = HttpStatusCode.OK)
		{
			if (module == null)
			{
				throw new ArgumentNullException(nameof(module));
			}

			if (data == null)
			{
				throw new ArgumentNullException(nameof(data));
			}

			Response response = new StreamResponse(() =>
			{
				Stream stream = new MemoryStream(data, false);
				return stream;
			}, "application/octet-stream");
			response.StatusCode = statusCode;
			return response;
		}

		/// <summary>
		///     Creates a stream response delivering an embedded file.
		/// </summary>
		/// <param name="module"> The Nancy module. </param>
		/// <param name="assembly"> The assembly qhich contains the emdedded file. </param>
		/// <param name="file"> The name of the embedded file. </param>
		/// <param name="statusCode"> The optional HTTP status code of the response. The default value is OK. </param>
		/// <returns>
		///     The response or null if the embedded file does not exist.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="module" />, <paramref name="assembly" />, or <paramref name="file" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="file" /> is an empty string. </exception>
		public static Response EmbeddedFileResponse (this NancyModule module, Assembly assembly, string file, HttpStatusCode statusCode = HttpStatusCode.OK)
		{
			if (module == null)
			{
				throw new ArgumentNullException(nameof(module));
			}

			if (assembly == null)
			{
				throw new ArgumentNullException(nameof(assembly));
			}

			if (file == null)
			{
				throw new ArgumentNullException(nameof(file));
			}

			if (file.IsNullOrEmptyOrWhitespace())
			{
				throw new EmptyStringArgumentException(nameof(file));
			}

			using (Stream stream = assembly.GetEmbeddedFileStream(file))
			{
				if (stream == null)
				{
					return null;
				}
			}

			Response response = new StreamResponse(() =>
			{
				Stream stream = assembly.GetEmbeddedFileStream(file);
				return stream;
			}, MimeTypes.GetMimeType(file));
			response.StatusCode = statusCode;
			return response;
		}

		/// <summary>
		///     Creates a JSON response by serializing a .NET object as a JSON string.
		/// </summary>
		/// <typeparam name="T"> The type of the object to serialize. </typeparam>
		/// <param name="module"> The Nancy module. </param>
		/// <param name="obj"> The object to serialize. </param>
		/// <param name="statusCode"> The optional HTTP status code of the response. The default value is OK. </param>
		/// <returns>
		///     The response.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="module" /> or <paramref name="obj" /> is null. </exception>
		public static Response JsonResponse <T> (this NancyModule module, T obj, HttpStatusCode statusCode = HttpStatusCode.OK)
		{
			if (module == null)
			{
				throw new ArgumentNullException(nameof(module));
			}

			if (obj == null)
			{
				throw new ArgumentNullException(nameof(obj));
			}

			Response response = module.Response.AsJson(obj, statusCode);
			return response;
		}

		/// <summary>
		///     reates a stream response delivering a physical file.
		/// </summary>
		/// <param name="module"> The Nancy module. </param>
		/// <param name="file"> The path to the physical file. </param>
		/// <param name="statusCode"> The optional HTTP status code of the response. The default value is OK. </param>
		/// <returns>
		///     The response or null if the physical file does not exist.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="module" /> or <paramref name="file" /> is null. </exception>
		/// <exception cref="InvalidPathArgumentException"> <paramref name="file" /> is an invalid file path. </exception>
		public static Response PhysicalFileResponse (this NancyModule module, FilePath file, HttpStatusCode statusCode = HttpStatusCode.OK)
		{
			if (module == null)
			{
				throw new ArgumentNullException(nameof(module));
			}

			if (file == null)
			{
				throw new ArgumentNullException(nameof(file));
			}

			if (!file.IsRealFile)
			{
				throw new InvalidPathArgumentException(nameof(file));
			}

			if (!file.Exists)
			{
				return null;
			}

			Response response = new StreamResponse(() =>
			{
				Stream stream = File.OpenRead(file);
				return stream;
			}, MimeTypes.GetMimeType(file));
			response.StatusCode = statusCode;
			return response;
		}

		/// <summary>
		///     Creates a response delivering binary data from a stream.
		/// </summary>
		/// <param name="module"> The Nancy module. </param>
		/// <param name="stream"> The stream. </param>
		/// <param name="statusCode"> The optional HTTP status code of the response. The default value is OK. </param>
		/// <returns>
		///     The response.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="module" /> or <paramref name="stream" /> is null. </exception>
		public static Response StreamResponse (this NancyModule module, Stream stream, HttpStatusCode statusCode = HttpStatusCode.OK)
		{
			if (module == null)
			{
				throw new ArgumentNullException(nameof(module));
			}

			if (stream == null)
			{
				throw new ArgumentNullException(nameof(stream));
			}

			Response response = new StreamResponse(() =>
			{
				return stream;
			}, "application/octet-stream");
			response.StatusCode = statusCode;
			return response;
		}

		/// <summary>
		///     Creates a text response.
		/// </summary>
		/// <param name="module"> The Nancy module. </param>
		/// <param name="text"> The text response. </param>
		/// <param name="statusCode"> The optional HTTP status code of the response. The default value is OK. </param>
		/// <returns>
		///     The response.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="module" /> or <paramref name="text" /> is null. </exception>
		public static Response TextResponse (this NancyModule module, string text, HttpStatusCode statusCode = HttpStatusCode.OK)
		{
			if (module == null)
			{
				throw new ArgumentNullException(nameof(module));
			}

			if (text == null)
			{
				throw new ArgumentNullException(nameof(text));
			}

			Response response = module.Response.AsText(text);
			response.StatusCode = statusCode;
			return response;
		}

		/// <summary>
		///     Creates a XML response by serializing a .NET object as XML.
		/// </summary>
		/// <typeparam name="T"> The type of the object to serialize. </typeparam>
		/// <param name="module"> The Nancy module. </param>
		/// <param name="obj"> The object to serialize. </param>
		/// <param name="statusCode"> The optional HTTP status code of the response. The default value is OK. </param>
		/// <returns>
		///     The response.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="module" /> or <paramref name="obj" /> is null. </exception>
		public static Response XmlResponse <T> (this NancyModule module, T obj, HttpStatusCode statusCode = HttpStatusCode.OK)
		{
			if (module == null)
			{
				throw new ArgumentNullException(nameof(module));
			}

			if (obj == null)
			{
				throw new ArgumentNullException(nameof(obj));
			}

			Response response = module.Response.AsXml(obj);
			response.StatusCode = statusCode;
			return response;
		}

		/// <summary>
		///     Creates a XML response from an XML document.
		/// </summary>
		/// <param name="module"> The Nancy module. </param>
		/// <param name="doc"> The XML document. </param>
		/// <param name="statusCode"> The optional HTTP status code of the response. The default value is OK. </param>
		/// <returns>
		///     The response.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="module" /> or <paramref name="doc" /> is null. </exception>
		public static Response XmlResponse (this NancyModule module, XDocument doc, HttpStatusCode statusCode = HttpStatusCode.OK)
		{
			if (module == null)
			{
				throw new ArgumentNullException(nameof(module));
			}

			if (doc == null)
			{
				throw new ArgumentNullException(nameof(doc));
			}

			string xmlData = doc.ToString(SaveOptions.None);

			Response response = module.Response.AsText(xmlData, "application/xml");
			response.StatusCode = statusCode;
			return response;
		}

		/// <summary>
		///     Creates a XML response from an XML document.
		/// </summary>
		/// <param name="module"> The Nancy module. </param>
		/// <param name="doc"> The XML document. </param>
		/// <param name="statusCode"> The optional HTTP status code of the response. The default value is OK. </param>
		/// <returns>
		///     The response.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="module" /> or <paramref name="doc" /> is null. </exception>
		public static Response XmlResponse (this NancyModule module, XmlDocument doc, HttpStatusCode statusCode = HttpStatusCode.OK)
		{
			if (module == null)
			{
				throw new ArgumentNullException(nameof(module));
			}

			if (doc == null)
			{
				throw new ArgumentNullException(nameof(doc));
			}

			string xmlData;
			using (StringWriter sw = new StringWriter())
			{
				doc.Save(sw);
				sw.Flush();
				xmlData = sw.ToString();
			}

			Response response = module.Response.AsText(xmlData, "application/xml");
			response.StatusCode = statusCode;
			return response;
		}

		#endregion
	}
}
