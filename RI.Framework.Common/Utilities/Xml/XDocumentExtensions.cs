﻿using System;
using System.Xml;
using System.Xml.Linq;




namespace RI.Framework.Utilities.Xml
{
    /// <summary>
    ///     Provides utility/extension methods for the <see cref="XDocument" /> type.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    public static class XDocumentExtensions
    {
        #region Static Methods

        /// <summary>
        ///     Converts a <see cref="XDocument" /> to a <see cref="XmlDocument" />.
        /// </summary>
        /// <param name="xDocument"> The <see cref="XDocument" /> to convert. </param>
        /// <returns> The converted <see cref="XmlDocument" />. </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="xDocument" /> is null. </exception>
        public static XmlDocument ToXmlDocument (this XDocument xDocument)
        {
            if (xDocument == null)
            {
                throw new ArgumentNullException(nameof(xDocument));
            }

            XmlDocument xmlDocument = new XmlDocument();
            using (XmlReader xmlReader = xDocument.CreateReader())
            {
                xmlDocument.Load(xmlReader);
            }
            return xmlDocument;
        }

        #endregion
    }
}
