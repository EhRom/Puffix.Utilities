using Puffix.Utilities.Exceptions.XmlUtilities;
using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Puffix.Utilities
{
    /// <summary>
    /// Utilities for XML document and files.
    /// </summary>
    /// <typeparam name="ObjectTypeT">Type of the objects to process. Must be a class.</typeparam>
    public static class XmlUtilities<ObjectTypeT>
        where ObjectTypeT : class
    {
        /// <summary>
        /// Deserialize XML document.
        /// </summary>
        /// <param name="xml">XML document to deserialize.</param>
        /// <returns>Deserialized object.</returns>
        public static ObjectTypeT Deserialize(XmlDocument xml)
        {
            if (xml?.DocumentElement == null)
                throw new NullXmlDocumentException();

            try
            {
                using XmlNodeReader reader = new XmlNodeReader(xml.DocumentElement);
                XmlSerializer serializer = new XmlSerializer(typeof(ObjectTypeT));
                return (ObjectTypeT)serializer.Deserialize(reader);
            }
            catch (Exception error)
            {
                throw new DeserializeException(error);
            }
        }

        /// <summary>
        /// Deserialize XML data (byte array).
        /// </summary>
        /// <param name="xmlData">XML document to deserialize.</param>
        /// <returns>Deserialized object.</returns>
        public static ObjectTypeT Deserialize(byte[] xmlData)
        {
            if (xmlData == null)
                throw new NullXmlDataException();

            try
            {
                using MemoryStream stream = new MemoryStream(xmlData);
                XmlSerializer serializer = new XmlSerializer(typeof(ObjectTypeT));
                return (ObjectTypeT)serializer.Deserialize(stream);
            }
            catch (Exception error)
            {
                throw new DeserializeException(error);
            }
        }
    }
}
