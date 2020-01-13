using Puffix.Utilities.Exceptions.XmlUtilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Schema;

namespace Puffix.Utilities
{
    /// <summary>
    /// Utilities for XML validation.
    /// </summary>
    public sealed class XmlValidationUtilities
    {
        /// <summary>
        /// Collection of the validation erros.
        /// </summary>
        private readonly List<Exception> validationErrors;

        /// <summary>
        /// Constructor.
        /// </summary>
        private XmlValidationUtilities()
        {
            validationErrors = new List<Exception>();
        }

        /// <summary>
        /// Load XSD schema set.
        /// </summary>
        /// <param name="schemasStreamCollection">Collection of the streams chich embed the XSD.</param>
        /// <returns>Loaded and validated XML Schema set.</returns>
        public static XmlSchemaSet LoadXmlSchemaSet(params Stream[] schemasStreamCollection)
        {
            XmlValidationUtilities utilities = new XmlValidationUtilities();

            // Load and validate the collection of the XSD (streams).
            if (!utilities.LoadXmlSchemaSet(schemasStreamCollection, out XmlSchemaSet xmlSchemaSet))
                throw new LoadingSchemaSetException(ObjectSerializer.DeepClone(utilities.validationErrors));

            return xmlSchemaSet;
        }

        /// <summary>
        /// Try to load a XSD schema set.
        /// </summary>
        /// <param name="schemasStreamCollection">Collection of the streams chich embed the XSD collection.</param>
        /// <param name="xmlSchemaSet">Shema set (output parameter).</param>
        /// <param name="errors">Error encoutered while loading and validating the XSD collection (output parameter).</param>
        /// <returns>Indicates whether the schema set is loaded and validated or not.</returns>
        public static bool TryLoadXmlSchemaSet(IEnumerable<Stream> schemasStreamCollection, out XmlSchemaSet xmlSchemaSet, out LoadingSchemaSetException errors)
        {
            XmlValidationUtilities utilities = new XmlValidationUtilities();

            // Load and validate the collection of the XSD (streams).
            bool isValid = utilities.LoadXmlSchemaSet(schemasStreamCollection, out xmlSchemaSet);

            // Encapsulate load and validation errors.
            errors = isValid ? null : new LoadingSchemaSetException(ObjectSerializer.DeepClone(utilities.validationErrors));

            return isValid;
        }

        /// <summary>
        /// Load XSD schema set.
        /// </summary>
        /// <param name="schemasStreamCollection">Collection of the streams chich embed the XSD.</param>
        /// <param name="xmlSchemaSet">Shema set (output parameter).</param>
        /// <returns>Indicates whether the schema set is loaded and validated or not.</returns>
        private bool LoadXmlSchemaSet(IEnumerable<Stream> schemasStreamCollection, out XmlSchemaSet xmlSchemaSet)
        {
            // Initialize the schema set, and register the handler to catch validation errors.
            xmlSchemaSet = new XmlSchemaSet();
            xmlSchemaSet.ValidationEventHandler += XmlValidationCallback;

            // Create EventHandler to catch loading schema errors.
            ValidationEventHandler schemaValidationEventHandler = XmlValidationCallback;

            foreach (Stream schemaStream in schemasStreamCollection)
            {
                // Load the XSD and add to the schema set if the load is fine.
                if (LoadXsd(schemaStream, schemaValidationEventHandler, out XmlSchema schema))
                    xmlSchemaSet.Add(schema);
            }

            // If error are encountered  while loading schema, the schemas are not compiled.
            if (validationErrors.Count > 0)
                return false;

            // Compile the shcema set to detect errors inside schemas.
            try
            {
                xmlSchemaSet.Compile();
            }
            catch (Exception error)
            {
                validationErrors.Add(error);
            }

            // Indicates whether errors were encoutered while validating the schema set.
            return validationErrors.Count == 0;
        }

        /// <summary>
        /// Load XML Schema (XSD).
        /// </summary>
        /// <param name="schemaStream">Stream which embeds the schema..</param>
        /// <param name="schemaValidationEventHandler">Handler for loading errors.</param>
        /// <param name="schema">XML Schema (output parameter)</param>
        /// <returns>Indicates whether the schema is loaded or not.</returns>
        private bool LoadXsd(Stream schemaStream, ValidationEventHandler schemaValidationEventHandler, out XmlSchema schema)
        {
            try
            {
                // Load the schema.
                schema = XmlSchema.Read(schemaStream, schemaValidationEventHandler);
            }
            catch (Exception error)
            {
                // Some errors must be caught, because all are not handled by the validation handler. The error is added to the list.
                validationErrors.Add(error);
                schema = null;
            }

            // Indicates whether errors were encoutered while loading the schema.
            return validationErrors.Count == 0;
        }

        /// <summary>
        /// Valiate the content of one XML document.
        /// </summary>
        /// <param name="xmlDocument">XML docment.</param>
        /// <param name="xmlSchemaSet">XSD set for the validation.</param>
        /// <param name="throwError">Indicates whethhr an error is thrown when the document is not valid (default: false).</param>
        /// <returns>Indicate whether the document is valid or not.</returns>
        public static bool ValidateXml(XmlDocument xmlDocument, XmlSchemaSet xmlSchemaSet, bool throwError = false)
        {
            XmlValidationUtilities utilities = new XmlValidationUtilities();

            // Validate the XML document.
            bool isValid = utilities.ValidateXml(xmlDocument, xmlSchemaSet);

            // Throw error if needed.
            if (!isValid && throwError)
                throw new XmlValidationException(ObjectSerializer.DeepClone(utilities.validationErrors));

            return isValid;
        }

        /// <summary>
        /// Validate the content of one XML document.
        /// </summary>
        /// <param name="xmlDocument">XML docment.</param>
        /// <param name="xmlSchemaSet">XSD set for the validation.</param>
        /// <param name="errors">Error encoutered while validating the XML (output parameter).</param>
        /// <returns>Indicate whether the document is valid or not.</returns>
        public static bool TryValidateXml(XmlDocument xmlDocument, XmlSchemaSet xmlSchemaSet, out XmlValidationException errors)
        {
            XmlValidationUtilities utilities = new XmlValidationUtilities();
            
            // Validate the XML document.
            bool isValid = utilities.ValidateXml(xmlDocument, xmlSchemaSet);

            // Encapsulate validation errors.
            errors = isValid ? null : new XmlValidationException(ObjectSerializer.DeepClone(utilities.validationErrors));

            return isValid;
        }

        /// <summary>
        /// Try to validate the content of one XML document.
        /// </summary>
        /// <param name="xmlDocument">XML docment.</param>
        /// <param name="xmlSchemaSet">XSD set for the validation.</param>
        /// <returns>Indicate whether the document is valid or not.</returns>
        private bool ValidateXml(XmlDocument xmlDocument, XmlSchemaSet xmlSchemaSet)
        {
            // Specify the schema set of the XML document.
            xmlDocument.Schemas = null;
            xmlDocument.Schemas.Add(xmlSchemaSet);

            // Validate the XML document. Validation errors are handled by the callback.
            xmlDocument.Validate(XmlValidationCallback);

            return validationErrors.Count == 0;
        }

        /// <summary>
        /// Callback to handle errors when loading XSD shemas or validating XML data.
        /// </summary>
        /// <param name="sender">Emetteur.</param>
        /// <param name="e">Arguments.</param>
        private void XmlValidationCallback(object sender, ValidationEventArgs e)
        {
            // Si l'evenement est du à une erreur, on specifie l'erreur.
            if (e.Severity == XmlSeverityType.Error)
                validationErrors.Add(e.Exception);
        }
    }
}
