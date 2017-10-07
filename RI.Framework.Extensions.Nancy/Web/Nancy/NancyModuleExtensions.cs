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

			const string itemName = "RequestedBinaryData-0537A029-D356-45F9-B821-4471C8B97030";

			module.Context.Items.Add(itemName, data);

			Response response = new StreamResponse(() =>
			{
				byte[] requestedBinaryData = (byte[])module.Context.Items[itemName];
				Stream stream = new MemoryStream(requestedBinaryData, false);
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
		///     The response.
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

			const string assemblyItemName = "RequestedEmbeddedAssembly-EC99F0E8-A8F3-4113-B456-423A11C5418B";
			const string fileItemName = "RequestedEmbeddedFile-24BED48D-D5DF-49AD-95E5-3CBFB3818B62";

			module.Context.Items.Add(assemblyItemName, assembly);
			module.Context.Items.Add(fileItemName, file);

			Response response = new StreamResponse(() =>
			{
				Assembly requestedEmbeddedAssembly = (Assembly)module.Context.Items[assemblyItemName];
				string requestedEmbeddedFile = (string)module.Context.Items[fileItemName];
				Stream stream = requestedEmbeddedAssembly.GetEmbeddedFileStream(requestedEmbeddedFile);
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
		///     The response.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="module" /> or <paramref name="file" /> is null. </exception>
		/// <exception cref="InvalidPathArgumentException"> <paramref name="file" /> is an invalid file path. </exception>
		/// <exception cref="FileNotFoundException"> <paramref name="file" /> does not exist. </exception>
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

			if (file.IsRealFile)
			{
				throw new InvalidPathArgumentException(nameof(file));
			}

			if (!file.Exists)
			{
				throw new FileNotFoundException("Physical file not found.", file);
			}

			const string itemName = "RequestedPhysicalFile-56C6FEF5-324F-4718-B62F-CFACC4EF4ACC";

			module.Context.Items.Add(itemName, file);

			Response response = new StreamResponse(() =>
			{
				FilePath requestedPhysicalFile = (FilePath)module.Context.Items[itemName];
				Stream stream = File.OpenRead(requestedPhysicalFile);
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

			const string itemName = "RequestedBinaryStream-56B94DE3-EA5E-4930-9169-F969F7CD0664";

			module.Context.Items.Add(itemName, stream);

			Response response = new StreamResponse(() =>
			{
				Stream requestedBinaryStream = (Stream)module.Context.Items[itemName];
				return requestedBinaryStream;
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
