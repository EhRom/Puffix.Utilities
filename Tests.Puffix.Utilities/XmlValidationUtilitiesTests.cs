using Puffix.Utilities;
using Puffix.Utilities.Exceptions.XmlUtilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using Xunit;

namespace Tests.Puffix.Utilities
{
    /// <summary>
    /// Test class for the XML validation helper.
    /// </summary>
    public class XmlValidationUtilitiesTests
    {
        /// <summary>
        /// Path for the XSD resource.
        /// </summary>
        private const string SAMPLE_XSD_RESOURCE_PATH = "Tests.Puffix.Utilities.Resources.TestSchema.xsd";

        /// <summary>
        /// Path for the linked XSD resource.
        /// </summary>
        private const string SAMPLE_LINKED_XSD_RESOURCE_PATH = "Tests.Puffix.Utilities.Resources.LinkedTestSchema.xsd";

        /// <summary>
        /// Path for the first invalid XSD resource.
        /// </summary>
        private const string INVALID_SAMPLE1_XSD_RESOURCE_PATH = "Tests.Puffix.Utilities.Resources.InvalidTestSchema1.xsd";

        /// <summary>
        /// Path for the second invalid XSD resource.
        /// </summary>
        private const string INVALID_SAMPLE2_XSD_RESOURCE_PATH = "Tests.Puffix.Utilities.Resources.InvalidTestSchema2.xsd";

        /// <summary>
        /// Path for the XML sample resource.
        /// </summary>
        private const string SAMPLE_XML_RESOURCE_PATH = "Tests.Puffix.Utilities.Resources.ValidXml.xml";

        /// <summary>
        /// Path for the XML sample resource with linked schema.
        /// </summary>
        private const string SAMPLE_LINKED_XML_RESOURCE_PATH = "Tests.Puffix.Utilities.Resources.ValidLinkedXml.xml";

        /// <summary>
        /// Path for the first XML invalid sample.
        /// </summary>
        private const string INVALID_SAMPLE1_XML_RESOURCE_PATH = "Tests.Puffix.Utilities.Resources.InvalidXml1.xml";

        /// <summary>
        /// Path for the second XML invalid sample.
        /// </summary>
        private const string INVALID_SAMPLE2_XML_RESOURCE_PATH = "Tests.Puffix.Utilities.Resources.InvalidXml2.xml";

        /// <summary>
        /// Test the XSD load method.
        /// </summary>
        [Fact]
        public void LoadSchemaSetTest()
        {
            // Load resources.
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            using Stream xsdStream = currentAssembly.GetManifestResourceStream(SAMPLE_XSD_RESOURCE_PATH);

            // Test load XML Schema Set.
            var schemaSet = XmlValidationUtilities.LoadXmlSchemaSet(xsdStream);

            // Assert result.
            Assert.NotNull(schemaSet);
            Assert.True(schemaSet.IsCompiled);
            Assert.Equal(1, schemaSet.Count);
        }

        /// <summary>
        /// Test the "try" XSD load method.
        /// </summary>
        [Fact]
        public void TryLoadSchemaSetTest()
        {
            // Load resources.
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            using Stream xsdStream = currentAssembly.GetManifestResourceStream(SAMPLE_XSD_RESOURCE_PATH);

            // Test load XML Schema Set.
            bool isValid = XmlValidationUtilities.TryLoadXmlSchemaSet(new[] { xsdStream }, out var schemaSet, out var errors);

            // Assert result.
            Assert.True(isValid);
            Assert.Null(errors);
            Assert.NotNull(schemaSet);
            Assert.True(schemaSet.IsCompiled);
            Assert.Equal(1, schemaSet.Count);
        }

        /// <summary>
        /// Test the XSD load method with a linked schema.
        /// </summary>
        [Fact]
        public void LoadSchemaSetWithLinkedSchemaTest()
        {
            // Load resources.
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            using Stream linkedXsdStream = currentAssembly.GetManifestResourceStream(SAMPLE_LINKED_XSD_RESOURCE_PATH);
            using Stream xsdStream = currentAssembly.GetManifestResourceStream(SAMPLE_XSD_RESOURCE_PATH);

            // Test load XML Schema Set.
            var schemaSet = XmlValidationUtilities.LoadXmlSchemaSet(xsdStream, linkedXsdStream);

            // Assert result.
            Assert.NotNull(schemaSet);
            Assert.True(schemaSet.IsCompiled);
            Assert.Equal(2, schemaSet.Count);
        }

        /// <summary>
        /// Test the "try" XSD load method with a linked schema.
        /// </summary>
        [Fact]
        public void TryLoadSchemaSetWithLinkedSchemaTest()
        {
            // Load resources.
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            using Stream linkedXsdStream = currentAssembly.GetManifestResourceStream(SAMPLE_LINKED_XSD_RESOURCE_PATH);
            using Stream xsdStream = currentAssembly.GetManifestResourceStream(SAMPLE_XSD_RESOURCE_PATH);

            // Test load XML Schema Set.
            bool isValid = XmlValidationUtilities.TryLoadXmlSchemaSet(new[] { xsdStream, linkedXsdStream }, out var schemaSet, out var errors);

            // Assert result.
            Assert.True(isValid);
            Assert.Null(errors);
            Assert.NotNull(schemaSet);
            Assert.True(schemaSet.IsCompiled);
            Assert.Equal(2, schemaSet.Count);
        }

        /// <summary>
        /// Test the XSD load method with invalid XSD.
        /// </summary>
        [Fact]
        public void InvalidLoadSchemaSetTest1()
        {
            const string expectedErrorMessage = "The 'xs:restriction' start tag on line 7 position 6 does not match the end tag of 'xs:simpleType'. Line 8, position 5.";

            // Load resources.
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            using Stream xsdStream = currentAssembly.GetManifestResourceStream(INVALID_SAMPLE1_XSD_RESOURCE_PATH);

            // Test load XML Schema Set.
            var error = Assert.Throws<LoadingSchemaSetException>(() => XmlValidationUtilities.LoadXmlSchemaSet(xsdStream));

            // Assert result.
            Assert.NotNull(error.ValidationErrors);
            Assert.Equal(1, error.ValidationErrors.Count);
            Assert.Equal(expectedErrorMessage, error.ValidationErrors.First().Message);
        }

        /// <summary>
        /// Test the "try" XSD load method with invalid XSD.
        /// </summary>
        [Fact]
        public void InvalidTryLoadSchemaSetTest1()
        {
            const string expectedErrorMessage = "The 'xs:restriction' start tag on line 7 position 6 does not match the end tag of 'xs:simpleType'. Line 8, position 5.";

            // Load resources.
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            using Stream xsdStream = currentAssembly.GetManifestResourceStream(INVALID_SAMPLE1_XSD_RESOURCE_PATH);

            // Test load XML Schema Set.
            bool isValid = XmlValidationUtilities.TryLoadXmlSchemaSet(new[] { xsdStream }, out var schemaSet, out var error);

            // Assert result.
            Assert.False(isValid);
            Assert.NotNull(schemaSet);
            Assert.Equal(0, schemaSet.Count);
            Assert.NotNull(error);
            Assert.NotNull(error.ValidationErrors);
            Assert.Equal(1, error.ValidationErrors.Count);
            Assert.Equal(expectedErrorMessage, error.ValidationErrors.First().Message);
        }

        /// <summary>
        /// Test the XSD load method with invalid XSD.
        /// </summary>
        [Fact]
        public void InvalidLoadSchemaSetTest2()
        {
            const string expectedFirstErrorMessage = "Type 'urn:sonarqube.org:2019:8.1:SeverityType' is not declared.";
            const string expectedSecondErrorMessage = "Type 'urn:sonarqube.org:2019:8.1:IssueTypeType' is not declared.";
            const string expectedThridErrorMessage = "Type 'urn:sonarqube.org:2019:8.1:IssueTypeType' is not declared.";

            // Load resources.
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            using Stream xsdStream = currentAssembly.GetManifestResourceStream(INVALID_SAMPLE2_XSD_RESOURCE_PATH);

            // Test load XML Schema Set.
            var error = Assert.Throws<LoadingSchemaSetException>(() => XmlValidationUtilities.LoadXmlSchemaSet(xsdStream));

            // Assert result.
            Assert.NotNull(error.ValidationErrors);
            Assert.Equal(3, error.ValidationErrors.Count);

            var errors = new List<Exception>(error.ValidationErrors);

            Assert.Equal(expectedFirstErrorMessage, errors[0].Message);
            Assert.Equal(expectedSecondErrorMessage, errors[1].Message);
            Assert.Equal(expectedThridErrorMessage, errors[2].Message);
        }

        /// <summary>
        /// Test the "try" XSD load method with invalid XSD.
        /// </summary>
        [Fact]
        public void InvalidTryLoadSchemaSetTest2()
        {
            const string expectedFirstErrorMessage = "Type 'urn:sonarqube.org:2019:8.1:SeverityType' is not declared.";
            const string expectedSecondErrorMessage = "Type 'urn:sonarqube.org:2019:8.1:IssueTypeType' is not declared.";
            const string expectedThridErrorMessage = "Type 'urn:sonarqube.org:2019:8.1:IssueTypeType' is not declared.";

            // Load resources.
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            using Stream xsdStream = currentAssembly.GetManifestResourceStream(INVALID_SAMPLE2_XSD_RESOURCE_PATH);

            // Test load XML Schema Set.
            bool isValid = XmlValidationUtilities.TryLoadXmlSchemaSet(new[] { xsdStream }, out var schemaSet, out var error);

            // Assert result.
            Assert.False(isValid);
            Assert.NotNull(schemaSet);
            Assert.Equal(1, schemaSet.Count);
            Assert.NotNull(error);
            Assert.NotNull(error.ValidationErrors);
            Assert.Equal(3, error.ValidationErrors.Count);

            var errors = new List<Exception>(error.ValidationErrors);

            Assert.Equal(expectedFirstErrorMessage, errors[0].Message);
            Assert.Equal(expectedSecondErrorMessage, errors[1].Message);
            Assert.Equal(expectedThridErrorMessage, errors[2].Message);
        }

        /// <summary>
        /// Test the validate XML method.
        /// </summary>
        [Fact]
        public void ValidateXmlTest()
        {
            // Load resources.
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            using Stream xmlStream = currentAssembly.GetManifestResourceStream(SAMPLE_XML_RESOURCE_PATH);
            using Stream xsdStream = currentAssembly.GetManifestResourceStream(SAMPLE_XSD_RESOURCE_PATH);

            // Load Schema set.
            var schemaSet = XmlValidationUtilities.LoadXmlSchemaSet(xsdStream);

            // Load XML
            XmlDocument xml = new XmlDocument();
            xml.Load(xmlStream);

            // Test XML validation.
            bool isValid = XmlValidationUtilities.ValidateXml(xml, schemaSet);

            // Assert result.
            Assert.True(isValid);
        }

        /// <summary>
        /// Test the validate XML method.
        /// </summary>
        [Fact]
        public void ValidateXmlWithThownErrorTest()
        {
            // Load resources.
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            using Stream xmlStream = currentAssembly.GetManifestResourceStream(SAMPLE_XML_RESOURCE_PATH);
            using Stream xsdStream = currentAssembly.GetManifestResourceStream(SAMPLE_XSD_RESOURCE_PATH);

            // Load Schema set.
            var schemaSet = XmlValidationUtilities.LoadXmlSchemaSet(xsdStream);

            // Load XML
            XmlDocument xml = new XmlDocument();
            xml.Load(xmlStream);

            // Test XML validation.
            bool isValid = XmlValidationUtilities.ValidateXml(xml, schemaSet, true);

            // Assert result.
            Assert.True(isValid);
        }

        /// <summary>
        /// Test the "try" validate XML method.
        /// </summary>
        [Fact]
        public void TryValidateXmlTest()
        {
            // Load resources.
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            using Stream xmlStream = currentAssembly.GetManifestResourceStream(SAMPLE_XML_RESOURCE_PATH);
            using Stream xsdStream = currentAssembly.GetManifestResourceStream(SAMPLE_XSD_RESOURCE_PATH);

            // Load Schema set.
            var schemaSet = XmlValidationUtilities.LoadXmlSchemaSet(xsdStream);

            // Load XML
            XmlDocument xml = new XmlDocument();
            xml.Load(xmlStream);

            // Test XML validation.
            bool isValid = XmlValidationUtilities.TryValidateXml(xml, schemaSet, out var error);

            // Assert result.
            Assert.True(isValid);
        }

        /// <summary>
        /// Test the validate linked XML method.
        /// </summary>
        [Fact]
        public void ValidateLinkedXmlTest()
        {
            // Load resources.
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            using Stream xmlStream = currentAssembly.GetManifestResourceStream(SAMPLE_LINKED_XML_RESOURCE_PATH);
            using Stream linkedXsdStream = currentAssembly.GetManifestResourceStream(SAMPLE_LINKED_XSD_RESOURCE_PATH);
            using Stream xsdStream = currentAssembly.GetManifestResourceStream(SAMPLE_XSD_RESOURCE_PATH);

            // Load Schema set.
            var schemaSet = XmlValidationUtilities.LoadXmlSchemaSet(xsdStream, linkedXsdStream);

            // Load XML
            XmlDocument xml = new XmlDocument();
            xml.Load(xmlStream);

            // Test XML validation.
            bool isValid = XmlValidationUtilities.ValidateXml(xml, schemaSet);

            // Assert result.
            Assert.True(isValid);
        }

        /// <summary>
        /// Test the validate linked XML method.
        /// </summary>
        [Fact]
        public void ValidateLinkedXmlWithThownErrorTest()
        {
            // Load resources.
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            using Stream xmlStream = currentAssembly.GetManifestResourceStream(SAMPLE_LINKED_XML_RESOURCE_PATH);
            using Stream linkedXsdStream = currentAssembly.GetManifestResourceStream(SAMPLE_LINKED_XSD_RESOURCE_PATH);
            using Stream xsdStream = currentAssembly.GetManifestResourceStream(SAMPLE_XSD_RESOURCE_PATH);

            // Load Schema set.
            var schemaSet = XmlValidationUtilities.LoadXmlSchemaSet(xsdStream, linkedXsdStream);

            // Load XML
            XmlDocument xml = new XmlDocument();
            xml.Load(xmlStream);

            // Test XML validation.
            bool isValid = XmlValidationUtilities.ValidateXml(xml, schemaSet, true);

            // Assert result.
            Assert.True(isValid);
        }

        /// <summary>
        /// Test the "try" validate linked XML method.
        /// </summary>
        [Fact]
        public void TryValidateLinkedXmlTest()
        {
            // Load resources.
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            using Stream xmlStream = currentAssembly.GetManifestResourceStream(SAMPLE_LINKED_XML_RESOURCE_PATH);
            using Stream linkedXsdStream = currentAssembly.GetManifestResourceStream(SAMPLE_LINKED_XSD_RESOURCE_PATH);
            using Stream xsdStream = currentAssembly.GetManifestResourceStream(SAMPLE_XSD_RESOURCE_PATH);

            // Load Schema set.
            var schemaSet = XmlValidationUtilities.LoadXmlSchemaSet(xsdStream, linkedXsdStream);

            // Load XML
            XmlDocument xml = new XmlDocument();
            xml.Load(xmlStream);

            // Test XML validation.
            bool isValid = XmlValidationUtilities.TryValidateXml(xml, schemaSet, out var error);

            // Assert result.
            Assert.True(isValid);
        }

        /// <summary>
        /// Test the validate linked XML method, with missing schema.
        /// </summary>
        [Fact]
        public void ValidateLinkedXmllWithMissingSchemaTest()
        {
            // Load resources.
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            using Stream xmlStream = currentAssembly.GetManifestResourceStream(SAMPLE_LINKED_XML_RESOURCE_PATH);
            using Stream xsdStream = currentAssembly.GetManifestResourceStream(SAMPLE_XSD_RESOURCE_PATH);

            // Load Schema set.
            var schemaSet = XmlValidationUtilities.LoadXmlSchemaSet(xsdStream);

            // Load XML
            XmlDocument xml = new XmlDocument();
            xml.Load(xmlStream);

            // Test XML validation.
            bool isValid = XmlValidationUtilities.ValidateXml(xml, schemaSet);

            // Assert result.
            Assert.False(isValid);
        }

        /// <summary>
        /// Test the validate linked XML method, with missing schema.
        /// </summary>
        [Fact]
        public void ValidateLinkedXmlWithThownErrorlAndMissingSchemaTest()
        {
            const string expectedErrorMessage = "Errors (1 errors) were encountered while validating a XML document.";
            const int expectedValidationErrorCount = 1;
            const string expectedValidationErrorMessage = "The 'urn:sonarqube.org:2019:8.1:issueContainer' element is not declared.";
            
            // Load resources.
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            using Stream xmlStream = currentAssembly.GetManifestResourceStream(SAMPLE_LINKED_XML_RESOURCE_PATH);
            using Stream xsdStream = currentAssembly.GetManifestResourceStream(SAMPLE_XSD_RESOURCE_PATH);

            // Load Schema set.
            var schemaSet = XmlValidationUtilities.LoadXmlSchemaSet(xsdStream);
            // Load XML
            XmlDocument xml = new XmlDocument();
            xml.Load(xmlStream);

            // Test XML validation.
            var error = Assert.Throws<XmlValidationException>(() => XmlValidationUtilities.ValidateXml(xml, schemaSet, true));
            
            // Assert result.
            Assert.NotNull(error);
            Assert.Equal(expectedErrorMessage, error.Message);
            Assert.NotNull(error.ValidationErrors);
            Assert.Equal(expectedValidationErrorCount, error.ValidationErrors.Count);
            Assert.Equal(expectedValidationErrorMessage, error.ValidationErrors.First().Message);
        }

        /// <summary>
        /// Test the "try" validate linked XML method, with missing schema.
        /// </summary>
        [Fact]
        public void TryValidateLinkedXmlWithMissingSchemaTest()
        {
            const string expectedErrorMessage = "Errors (1 errors) were encountered while validating a XML document.";
            const int expectedValidationErrorCount = 1;
            const string expectedValidationErrorMessage = "The 'urn:sonarqube.org:2019:8.1:issueContainer' element is not declared.";

            // Load resources.
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            using Stream xmlStream = currentAssembly.GetManifestResourceStream(SAMPLE_LINKED_XML_RESOURCE_PATH);
            using Stream xsdStream = currentAssembly.GetManifestResourceStream(SAMPLE_XSD_RESOURCE_PATH);

            // Load Schema set.
            var schemaSet = XmlValidationUtilities.LoadXmlSchemaSet(xsdStream);

            // Load XML
            XmlDocument xml = new XmlDocument();
            xml.Load(xmlStream);

            // Test XML validation.
            bool isValid = XmlValidationUtilities.TryValidateXml(xml, schemaSet, out var error);

            // Assert result.
            Assert.False(isValid);
            Assert.NotNull(error);
            Assert.Equal(expectedErrorMessage, error.Message);
            Assert.NotNull(error.ValidationErrors);
            Assert.Equal(expectedValidationErrorCount, error.ValidationErrors.Count);
            Assert.Equal(expectedValidationErrorMessage, error.ValidationErrors.First().Message);
        }

        /// <summary>
        /// Test the validate linked XML method, with missing schema.
        /// </summary>
        [Fact]
        public void ValidateInvalidXml1Test()
        {
            // Load resources.
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            using Stream xmlStream = currentAssembly.GetManifestResourceStream(INVALID_SAMPLE1_XML_RESOURCE_PATH);
            using Stream xsdStream = currentAssembly.GetManifestResourceStream(SAMPLE_XSD_RESOURCE_PATH);

            // Load Schema set.
            var schemaSet = XmlValidationUtilities.LoadXmlSchemaSet(xsdStream);

            // Load XML
            XmlDocument xml = new XmlDocument();
            xml.Load(xmlStream);

            // Test XML validation.
            bool isValid = XmlValidationUtilities.ValidateXml(xml, schemaSet);

            // Assert result.
            Assert.False(isValid);
        }

        /// <summary>
        /// Test the validate linked XML method, with missing schema.
        /// </summary>
        [Fact]
        public void ValidateInvalidXml1WithThownErrorTest()
        {
            const string expectedErrorMessage = "Errors (1 errors) were encountered while validating a XML document.";
            const int expectedValidationErrorCount = 1;
            const string expectedValidationErrorMessage = "The 'urn:sonarqube.org:2019:8.1:effortMinutes' element is invalid - The value 'AH AH, c'est une chaîne de caractères.' is invalid according to its datatype 'http://www.w3.org/2001/XMLSchema:int' - The string 'AH AH, c'est une chaîne de caractères.' is not a valid Int32 value.";

            // Load resources.
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            using Stream xmlStream = currentAssembly.GetManifestResourceStream(INVALID_SAMPLE1_XML_RESOURCE_PATH);
            using Stream xsdStream = currentAssembly.GetManifestResourceStream(SAMPLE_XSD_RESOURCE_PATH);

            // Load Schema set.
            var schemaSet = XmlValidationUtilities.LoadXmlSchemaSet(xsdStream);
            // Load XML
            XmlDocument xml = new XmlDocument();
            xml.Load(xmlStream);

            // Test XML validation.
            var error = Assert.Throws<XmlValidationException>(() => XmlValidationUtilities.ValidateXml(xml, schemaSet, true));

            // Assert result.
            Assert.NotNull(error);
            Assert.Equal(expectedErrorMessage, error.Message);
            Assert.NotNull(error.ValidationErrors);
            Assert.Equal(expectedValidationErrorCount, error.ValidationErrors.Count);
            Assert.Equal(expectedValidationErrorMessage, error.ValidationErrors.First().Message);
        }

        /// <summary>
        /// Test the "try" validate linked XML method, with missing schema.
        /// </summary>
        [Fact]
        public void TryValidateInvalidXml1Test()
        {
            const string expectedErrorMessage = "Errors (1 errors) were encountered while validating a XML document.";
            const int expectedValidationErrorCount = 1;
            const string expectedValidationErrorMessage = "The 'urn:sonarqube.org:2019:8.1:effortMinutes' element is invalid - The value 'AH AH, c'est une chaîne de caractères.' is invalid according to its datatype 'http://www.w3.org/2001/XMLSchema:int' - The string 'AH AH, c'est une chaîne de caractères.' is not a valid Int32 value.";

            // Load resources.
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            using Stream xmlStream = currentAssembly.GetManifestResourceStream(INVALID_SAMPLE1_XML_RESOURCE_PATH);
            using Stream xsdStream = currentAssembly.GetManifestResourceStream(SAMPLE_XSD_RESOURCE_PATH);

            // Load Schema set.
            var schemaSet = XmlValidationUtilities.LoadXmlSchemaSet(xsdStream);

            // Load XML
            XmlDocument xml = new XmlDocument();
            xml.Load(xmlStream);

            // Test XML validation.
            bool isValid = XmlValidationUtilities.TryValidateXml(xml, schemaSet, out var error);

            // Assert result.
            Assert.False(isValid);
            Assert.NotNull(error);
            Assert.Equal(expectedErrorMessage, error.Message);
            Assert.NotNull(error.ValidationErrors);
            Assert.Equal(expectedValidationErrorCount, error.ValidationErrors.Count);
            Assert.Equal(expectedValidationErrorMessage, error.ValidationErrors.First().Message);
        }

        /// <summary>
        /// Test the validate linked XML method, with missing schema.
        /// </summary>
        [Fact]
        public void ValidateInvalidXml2Test()
        {
            // Load resources.
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            using Stream xmlStream = currentAssembly.GetManifestResourceStream(INVALID_SAMPLE2_XML_RESOURCE_PATH);
            using Stream linkedXsdStream = currentAssembly.GetManifestResourceStream(SAMPLE_LINKED_XSD_RESOURCE_PATH);
            using Stream xsdStream = currentAssembly.GetManifestResourceStream(SAMPLE_XSD_RESOURCE_PATH);

            // Load Schema set.
            var schemaSet = XmlValidationUtilities.LoadXmlSchemaSet(xsdStream, linkedXsdStream);

            // Load XML
            XmlDocument xml = new XmlDocument();
            xml.Load(xmlStream);

            // Test XML validation.
            bool isValid = XmlValidationUtilities.ValidateXml(xml, schemaSet);

            // Assert result.
            Assert.False(isValid);
        }

        /// <summary>
        /// Test the validate linked XML method, with missing schema.
        /// </summary>
        [Fact]
        public void ValidateInvalidXml2WithThownErrorTest()
        {
            const string expectedErrorMessage = "Errors (2 errors) were encountered while validating a XML document.";
            const int expectedValidationErrorCount = 2;
            const string expectedFirstValidationErrorMessage = "The element 'issue' in namespace 'urn:sonarqube.org:2019:8.1' has invalid child element 'badNodeName' in namespace 'urn:sonarqube.org:2019:8.1'. List of possible elements expected: 'engineId' in namespace 'urn:sonarqube.org:2019:8.1'.";
            const string expectedSecondValidationErrorMessage = "The element 'issue' in namespace 'urn:sonarqube.org:2019:8.1' has invalid child element 'badNodeName' in namespace 'urn:sonarqube.org:2019:8.1'. List of possible elements expected: 'engineId' in namespace 'urn:sonarqube.org:2019:8.1'.";

            // Load resources.
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            using Stream xmlStream = currentAssembly.GetManifestResourceStream(INVALID_SAMPLE2_XML_RESOURCE_PATH);
            using Stream linkedXsdStream = currentAssembly.GetManifestResourceStream(SAMPLE_LINKED_XSD_RESOURCE_PATH);
            using Stream xsdStream = currentAssembly.GetManifestResourceStream(SAMPLE_XSD_RESOURCE_PATH);

            // Load Schema set.
            var schemaSet = XmlValidationUtilities.LoadXmlSchemaSet(xsdStream, linkedXsdStream);
            // Load XML
            XmlDocument xml = new XmlDocument();
            xml.Load(xmlStream);

            // Test XML validation.
            var error = Assert.Throws<XmlValidationException>(() => XmlValidationUtilities.ValidateXml(xml, schemaSet, true));

            // Assert result.
            Assert.NotNull(error);
            Assert.Equal(expectedErrorMessage, error.Message);
            Assert.NotNull(error.ValidationErrors);
            Assert.Equal(expectedValidationErrorCount, error.ValidationErrors.Count);

            var errors = new List<Exception>(error.ValidationErrors);

            Assert.Equal(expectedFirstValidationErrorMessage, errors[0].Message);
            Assert.Equal(expectedSecondValidationErrorMessage, errors[1].Message);
        }

        /// <summary>
        /// Test the "try" validate linked XML method, with missing schema.
        /// </summary>
        [Fact]
        public void TryValidateInvalidXml2Test()
        {
            const string expectedErrorMessage = "Errors (2 errors) were encountered while validating a XML document.";
            const int expectedValidationErrorCount = 2;
            const string expectedFirstValidationErrorMessage = "The element 'issue' in namespace 'urn:sonarqube.org:2019:8.1' has invalid child element 'badNodeName' in namespace 'urn:sonarqube.org:2019:8.1'. List of possible elements expected: 'engineId' in namespace 'urn:sonarqube.org:2019:8.1'.";
            const string expectedSecondValidationErrorMessage = "The element 'issue' in namespace 'urn:sonarqube.org:2019:8.1' has invalid child element 'badNodeName' in namespace 'urn:sonarqube.org:2019:8.1'. List of possible elements expected: 'engineId' in namespace 'urn:sonarqube.org:2019:8.1'.";

            // Load resources.
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            using Stream xmlStream = currentAssembly.GetManifestResourceStream(INVALID_SAMPLE2_XML_RESOURCE_PATH);
            using Stream linkedXsdStream = currentAssembly.GetManifestResourceStream(SAMPLE_LINKED_XSD_RESOURCE_PATH);
            using Stream xsdStream = currentAssembly.GetManifestResourceStream(SAMPLE_XSD_RESOURCE_PATH);

            // Load Schema set.
            var schemaSet = XmlValidationUtilities.LoadXmlSchemaSet(xsdStream, linkedXsdStream);

            // Load XML
            XmlDocument xml = new XmlDocument();
            xml.Load(xmlStream);

            // Test XML validation.
            bool isValid = XmlValidationUtilities.TryValidateXml(xml, schemaSet, out var error);

            // Assert result.
            Assert.False(isValid);
            Assert.NotNull(error);
            Assert.Equal(expectedErrorMessage, error.Message);
            Assert.NotNull(error.ValidationErrors);
            Assert.Equal(expectedValidationErrorCount, error.ValidationErrors.Count);

            var errors = new List<Exception>(error.ValidationErrors);

            Assert.Equal(expectedFirstValidationErrorMessage, errors[0].Message);
            Assert.Equal(expectedSecondValidationErrorMessage, errors[1].Message);
        }
    }
}
