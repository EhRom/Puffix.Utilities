using Puffix.Utilities.Exceptions;
using Puffix.Utilities.Exceptions.XmlUtilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Puffix.Utilities;

/// <summary>
/// Utilities for XML document and files.
/// </summary>
public sealed class XmlUtilities
{
    ///// <summary>
    ///// Collection of the validation erros.
    ///// </summary>
    //private readonly ICollection<Exception> validationErrors;

    ///// <summary>
    ///// Constructor.
    ///// </summary>
    //private XmlUtilities()
    //{
    //    validationErrors = [];
    //}

    #region Serialization / Deserialization

    /// <summary>
    /// Deserialize XML document.
    /// </summary>
    /// <typeparam name="ObjectTypeT">Type of the objects to process. Must be a class.</typeparam>
    /// <param name="xml">XML document to deserialize.</param>
    /// <returns>Deserialized object.</returns>
    public static ObjectTypeT Deserialize<ObjectTypeT>(XmlDocument xml)
        where ObjectTypeT : class
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
    /// <typeparam name="ObjectTypeT">Type of the objects to process. Must be a class.</typeparam>
    /// <param name="xmlData">XML document to deserialize.</param>
    /// <returns>Deserialized object.</returns>
    public static ObjectTypeT Deserialize<ObjectTypeT>(byte[] xmlData)
        where ObjectTypeT : class
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

    /// <summary>
    /// Serialize an object as XML Document.
    /// </summary>
    /// <typeparam name="ObjectTypeT">Type of the objects to process. Must be a class.</typeparam>
    /// <param name="objectToSerialize">Object to serialize.</param>
    /// <param name="encoding">Encoding for the serialized data.</param>        
    /// <param name="indented">Indicates whetehr the XML is idented or not.</param>
    /// <returns>The XML representation (XML document) of the object.</returns>
    public static XmlDocument Serialize<ObjectTypeT>(ObjectTypeT objectToSerialize, Encoding encoding = null, bool indented = false)
        where ObjectTypeT : class
    {
        try
        {
            // Serialize the object.
            using Stream stream = SerializeAsStream(objectToSerialize, encoding, indented);

            // Load the XML content into a document..
            XmlDocument xml = new XmlDocument();
            xml.Load(stream);

            return xml;
        }
        catch (XmlUtilitiesExceptions)
        {
            throw;
        }
        catch (Exception error)
        {
            throw new SerializeException(error);
        }
    }

    /// <summary>
    /// Serialize an object as byte array.
    /// </summary>
    /// <typeparam name="ObjectTypeT">Type of the objects to process. Must be a class.</typeparam>
    /// <param name="objectToSerialize">Object to serialize.</param>
    /// <param name="encoding">Encoding for the serialized data.</param>        
    /// <param name="indented">Indicates whetehr the XML is idented or not.</param>
    /// <returns>Byte array which contains the XML representation of the object.</returns>
    public static byte[] SerializeAsByteArray<ObjectTypeT>(ObjectTypeT objectToSerialize, Encoding encoding = null, bool indented = false)
        where ObjectTypeT : class
    {
        try
        {
            using MemoryStream stream = SerializeAsStream(objectToSerialize, encoding, indented) as MemoryStream;

            byte[] xmlContent = stream.ToArray();

            return xmlContent;
        }
        catch (XmlUtilitiesExceptions)
        {
            throw;
        }
        catch (Exception error)
        {
            throw new SerializeException(error);
        }
    }

    /// <summary>
    /// Serialize an object as stream.
    /// </summary>
    /// <typeparam name="ObjectTypeT">Type of the objects to process. Must be a class.</typeparam>
    /// <param name="objectToSerialize">Object to serialize.</param>
    /// <param name="encoding">Encoding for the serialized data.</param>        
    /// <param name="indented">Indicates whetehr the XML is idented or not.</param>
    /// <returns>Stream which contains the XML representation of the object.</returns>
    public static Stream SerializeAsStream<ObjectTypeT>(ObjectTypeT objectToSerialize, Encoding encoding = null, bool indented = false)
        where ObjectTypeT : class
    {
        if (objectToSerialize == null)
            throw new NullObjectToSerializeException();

        try
        {
            // Set the writer settings.
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings
            {
                Indent = indented,
                Encoding = encoding ?? Encoding.Unicode
            };

            // Initialize the stream. The caller should manage the stream lifecycle.
            MemoryStream stream = new MemoryStream();

            // Initialize the writter and the serialization.
            using XmlWriter writer = XmlWriter.Create(stream, xmlWriterSettings);
            XmlSerializer serializer = new XmlSerializer(typeof(ObjectTypeT));

            // Serialize the data.
            serializer.Serialize(writer, objectToSerialize);
            writer.Flush();

            // Rewind stream.
            stream.Seek(0, SeekOrigin.Begin);

            return stream;
        }
        catch (Exception error)
        {
            throw new SerializeException(error);
        }
    }

    /// <summary>
    /// Serialize an object as XML Document (asynchornous method).
    /// </summary>
    /// <typeparam name="ObjectTypeT">Type of the objects to process. Must be a class.</typeparam>
    /// <param name="objectToSerialize">Object to serialize.</param>
    /// <param name="encoding">Encoding for the serialized data.</param>        
    /// <param name="indented">Indicates whetehr the XML is idented or not.</param>
    /// <returns>The XML representation (XML document) of the object.</returns>
    public async static Task<XmlDocument> SerializeAsync<ObjectTypeT>(ObjectTypeT objectToSerialize, Encoding encoding = null, bool indented = false)
        where ObjectTypeT : class
    {
        try
        {
            // Serialize the object.
            using Stream stream = await SerializeAsStreamAsync(objectToSerialize, encoding, indented);

            // Load the XML content into a document.
            XmlDocument xml = new XmlDocument();
            xml.Load(stream);

            return xml;
        }
        catch (XmlUtilitiesExceptions)
        {
            throw;
        }
        catch (Exception error)
        {
            throw new SerializeException(error);
        }
    }

    /// <summary>
    /// Serialize an object as byte array (asynchornous method).
    /// </summary>
    /// <typeparam name="ObjectTypeT">Type of the objects to process. Must be a class.</typeparam>
    /// <param name="objectToSerialize">Object to serialize.</param>
    /// <param name="encoding">Encoding for the serialized data.</param>        
    /// <param name="indented">Indicates whetehr the XML is idented or not.</param>
    /// <returns>Byte array which contains the XML representation of the object.</returns>
    public async static Task<byte[]> SerializeAsByteArrayAsync<ObjectTypeT>(ObjectTypeT objectToSerialize, Encoding encoding = null, bool indented = false)
        where ObjectTypeT : class
    {
        try
        {
            using MemoryStream stream = await SerializeAsStreamAsync(objectToSerialize, encoding, indented) as MemoryStream;

            byte[] xmlContent = stream.ToArray();

            return xmlContent;
        }
        catch (XmlUtilitiesExceptions)
        {
            throw;
        }
        catch (Exception error)
        {
            throw new SerializeException(error);
        }
    }

    /// <summary>
    /// Serialize an object as stream (asynchornous method).
    /// </summary>
    /// <typeparam name="ObjectTypeT">Type of the objects to process. Must be a class.</typeparam>
    /// <param name="objectToSerialize">Object to serialize.</param>
    /// <param name="encoding">Encoding for the serialized data.</param>        
    /// <param name="indented">Indicates whetehr the XML is idented or not.</param>
    /// <returns>Stream which contains the XML representation of the object.</returns>
    public async static Task<Stream> SerializeAsStreamAsync<ObjectTypeT>(ObjectTypeT objectToSerialize, Encoding encoding = null, bool indented = false)
        where ObjectTypeT : class
    {
        if (objectToSerialize == null)
            throw new NullObjectToSerializeException();

        try
        {
            // Set the writer settings.
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings
            {
                Indent = indented,
                Encoding = encoding ?? Encoding.Unicode,
                Async = true,
            };

            // Initialize the stream. The caller should manage the stream lifecycle.
            var stream = new MemoryStream();

            // Initialize the writter and the serialization.
            using XmlWriter writer = XmlWriter.Create(stream, xmlWriterSettings);
            XmlSerializer serializer = new XmlSerializer(typeof(ObjectTypeT));

            // Serialize the data.
            serializer.Serialize(writer, objectToSerialize);
            await writer.FlushAsync();

            // Rewind stream.
            stream.Seek(0, SeekOrigin.Begin);

            return stream;
        }
        catch (Exception error)
        {
            throw new SerializeException(error);
        }
    }

    #endregion Serialization / Deserialization

    #region Comparison

    /// <summary>
    /// Compare two XML documents.
    /// </summary>
    /// <param name="sourceXml">Source XML document.</param>
    /// <param name="targetXml">Target XML document.</param>
    /// <param name="nodesToEscape">Container (diotionary) for the nodes to skip for the comparison (Key: the node namespaece, Value: the list of the node names).</param>
    /// <returns>Indicates whether the documents match or not.</returns>
    public static bool Compare(XmlDocument sourceXml, XmlDocument targetXml, Dictionary<string, HashSet<string>> nodesToEscape = null)
    {
        if (sourceXml == null)
            throw new NullXmlDocumentException();
        if (targetXml == null)
            throw new NullXmlDocumentException();

        try
        {
            // Turns the XML documents into XDocument.
            XDocument source = ConvertToXDocument(sourceXml);
            XDocument target = ConvertToXDocument(targetXml);

            // Compare the documents.
            return CoreCompare(source, target, nodesToEscape);
        }
        catch (Exception error)
        {
            throw new CompareException(error);
        }
    }

    /// <summary>
    /// Compare two XML documents.
    /// </summary>
    /// <param name="sourceXml">Source XML document.</param>
    /// <param name="targetXml">Target XML document.</param>
    /// <param name="nodesToEscape">Container (diotionary) for the nodes to skip for the comparison (Key: the node namespaece, Value: the list of the node names).</param>
    /// <returns>Indicates whether the documents match or not.</returns>
    private static bool CoreCompare(XDocument source, XDocument target, Dictionary<string, HashSet<string>> nodesToEscape = null)
    {
        // Manage the nodes to skip.
        if (nodesToEscape != null && nodesToEscape.Count > 0)
        {
            // Turns the dictionary into a list.
            List<XName> nodesToEscapeCollection = [];
            foreach (var key in nodesToEscape.Keys)
            {
                nodesToEscapeCollection.AddRange(nodesToEscape[key].Select(name => XName.Get(name, key)));
            }

            // Neutralize the nodes to skip in the comparison: the value is set to the empty value.
            foreach (var nodeToEscape in nodesToEscapeCollection)
            {
                IEnumerable<XElement> sourceElementsToEscape = source.Descendants(nodeToEscape);
                IEnumerable<XElement> targetElementsToEscape = target.Descendants(nodeToEscape);

                foreach (XElement sourceElementToEscape in sourceElementsToEscape)
                {
                    sourceElementToEscape.Value = string.Empty;
                }

                foreach (XElement targetElementToEscape in targetElementsToEscape)
                {
                    targetElementToEscape.Value = string.Empty;
                }
            }
        }

        // Comapare the documents.
        return XNode.DeepEquals(source, target);
    }

    /// <summary>
    /// Convert a XML Document into a XDocument.
    /// </summary>
    /// <param name="xml">XML Document to convert.</param>
    /// <returns>Converted XDocument.</returns>
    public static XDocument ConvertToXDocument(XmlDocument xml)
    {
        using XmlNodeReader nodeReader = new XmlNodeReader(xml);
        return XDocument.Load(nodeReader);
    }

    #endregion Comparison
}
