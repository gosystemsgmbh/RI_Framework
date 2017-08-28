using System;
using System.Xml;
using System.Xml.Linq;

namespace RI.Framework.Utilities.Xml
{
	/// <summary>
	///     Provides utility/extension methods for the <see cref="XmlDocument" /> type.
	/// </summary>
	public static class XmlDocumentExtensions
	{
		/// <summary>
		/// Converts a <see cref="XmlDocument"/> to a <see cref="XDocument"/>.
		/// </summary>
		/// <param name="xmlDocument">The <see cref="XmlDocument"/> to convert.</param>
		/// <returns>The converted <see cref="XDocument"/>.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="xmlDocument"/> is null.</exception>
		public static XDocument ToXDocument(this XmlDocument xmlDocument)
		{
			if (xmlDocument == null)
			{
				throw new ArgumentNullException(nameof(xmlDocument));
			}

			using (XmlNodeReader nodeReader = new XmlNodeReader(xmlDocument))
			{
				nodeReader.MoveToContent();
				return XDocument.Load(nodeReader);
			}
		}
	}
}
