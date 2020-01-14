using Puffix.Utilities;
using Puffix.Utilities.Exceptions.XmlUtilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using Tests.Puffix.Utilities.Resources;
using Xunit;

namespace Tests.Puffix.Utilities
{
    /// <summary>
    /// Test class for the XML files and document helper.
    /// </summary>
    public class XmlUtilitiesTests
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

        #region Serialization / Deserialization

        /// <summary>
        /// Test deserialize XML Document.
        /// </summary>
        [Fact]
        public void DeserializeXmlTest()
        {
            // Load resources.
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            using Stream xmlStream = currentAssembly.GetManifestResourceStream(SAMPLE_XML_RESOURCE_PATH);

            // Load XML
            XmlDocument xml = new XmlDocument();
            xml.Load(xmlStream);

            // Test deserialization.
            var result = XmlUtilities.Deserialize<IssuesContainer>(xml);

            // Assert result.
            AssertIssuesContainer(result);
        }

        /// <summary>
        /// Test deserialize null XML Document.
        /// </summary>
        [Fact]
        public void NullObjectDeserializeTest()
        {
            const string expectedErrorMessage = "The XML document is not set.";

            XmlDocument xml = null;

            // Test deserialization.
            var error = Assert.Throws<NullXmlDocumentException>(() => XmlUtilities.Deserialize<IssuesContainer>(xml));

            // Assert result.
            Assert.NotNull(error);
            Assert.Equal(expectedErrorMessage, error.Message);
        }

        /// <summary>
        /// Test deserialize invalid XML Document.
        /// </summary>
        [Fact]
        public void InvalidDocument1DeserializeTest()
        {
            const string expectedOuterErrorMessage = "An error occured while deserializing a XML document or data.";
            const string expectedErrorMessage = "There is an error in the XML document.";
            const string expectedInnerErrorMessage = "Input string was not in a correct format.";

            // Load resources.
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            using Stream xmlStream = currentAssembly.GetManifestResourceStream(INVALID_SAMPLE1_XML_RESOURCE_PATH);

            // Load XML
            XmlDocument xml = new XmlDocument();
            xml.Load(xmlStream);

            // Test deserialization.
            var error = Assert.Throws<DeserializeException>(() => XmlUtilities.Deserialize<IssuesContainer>(xml));

            // Assert result.
            Assert.NotNull(error);
            Assert.NotNull(error.InnerException);
            Assert.NotNull(error.InnerException.InnerException);
            Assert.IsType<InvalidOperationException>(error.InnerException);
            Assert.IsType<FormatException>(error.InnerException.InnerException);
            Assert.Equal(expectedOuterErrorMessage, error.Message);
            Assert.Equal(expectedErrorMessage, error.InnerException.Message);
            Assert.Equal(expectedInnerErrorMessage, error.InnerException.InnerException.Message);
        }

        /// <summary>
        /// Test deserialize invalid XML Document.
        /// </summary>
        [Fact]
        public void InvalidDocument2DeserializeTest()
        {
            const string expectedOuterErrorMessage = "An error occured while deserializing a XML document or data.";
            const string expectedErrorMessage = "There is an error in the XML document.";
            const string expectedInnerErrorMessage = "<issueContainer xmlns='urn:sonarqube.org:2019:8.1'> was not expected.";

            // Load resources.
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            using Stream xmlStream = currentAssembly.GetManifestResourceStream(INVALID_SAMPLE2_XML_RESOURCE_PATH);

            // Load XML
            XmlDocument xml = new XmlDocument();
            xml.Load(xmlStream);

            // Test deserialization.
            var error = Assert.Throws<DeserializeException>(() => XmlUtilities.Deserialize<IssuesContainer>(xml));

            // Assert result.
            Assert.NotNull(error);
            Assert.NotNull(error.InnerException);
            Assert.NotNull(error.InnerException.InnerException);
            Assert.IsType<InvalidOperationException>(error.InnerException);
            Assert.IsType<InvalidOperationException>(error.InnerException.InnerException);
            Assert.Equal(expectedOuterErrorMessage, error.Message);
            Assert.Equal(expectedErrorMessage, error.InnerException.Message);
            Assert.Equal(expectedInnerErrorMessage, error.InnerException.InnerException.Message);
        }

        /// <summary>
        /// Test deserialize null XML Document element.
        /// </summary>
        [Fact]
        public void NullElementDeserializeTest()
        {
            const string expectedErrorMessage = "The XML document is not set.";

            XmlDocument xml = new XmlDocument();

            // Test deserialization.
            var error = Assert.Throws<NullXmlDocumentException>(() => XmlUtilities.Deserialize<IssuesContainer>(xml));

            // Assert result.
            Assert.NotNull(error);
            Assert.Equal(expectedErrorMessage, error.Message);
        }

        /// <summary>
        /// Test deserialize XML data.
        /// </summary>
        [Fact]
        public async Task DeserializeDataTest()
        {
            // Load resources.
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            using Stream stream = currentAssembly.GetManifestResourceStream(SAMPLE_XML_RESOURCE_PATH);
            using MemoryStream xmlStream = new MemoryStream();

            // Load XML
            await stream.CopyToAsync(xmlStream);
            byte[] xmlData = xmlStream.ToArray();

            // Test deserialization.
            var result = XmlUtilities.Deserialize<IssuesContainer>(xmlData);

            // Assert result.
            AssertIssuesContainer(result);
        }

        /// <summary>
        /// Test deserialize invalid XML Document.
        /// </summary>
        [Fact]
        public async Task InvalidData1DeserializeTest()
        {
            const string expectedOuterErrorMessage = "An error occured while deserializing a XML document or data.";
            const string expectedErrorMessage = "There is an error in XML document (22, 5).";
            const string expectedInnerErrorMessage = "Input string was not in a correct format.";

            // Load resources.
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            using Stream stream = currentAssembly.GetManifestResourceStream(INVALID_SAMPLE1_XML_RESOURCE_PATH);
            using MemoryStream xmlStream = new MemoryStream();

            // Load XML
            await stream.CopyToAsync(xmlStream);
            byte[] xmlData = xmlStream.ToArray();

            // Test deserialization.
            var error = Assert.Throws<DeserializeException>(() => XmlUtilities.Deserialize<IssuesContainer>(xmlData));

            // Assert result.
            Assert.NotNull(error);
            Assert.NotNull(error.InnerException);
            Assert.NotNull(error.InnerException.InnerException);
            Assert.IsType<InvalidOperationException>(error.InnerException);
            Assert.IsType<FormatException>(error.InnerException.InnerException);
            Assert.Equal(expectedOuterErrorMessage, error.Message);
            Assert.Equal(expectedErrorMessage, error.InnerException.Message);
            Assert.Equal(expectedInnerErrorMessage, error.InnerException.InnerException.Message);
        }

        /// <summary>
        /// Test deserialize invalid XML Document.
        /// </summary>
        [Fact]
        public async Task InvalidData2DeserializeTest()
        {
            const string expectedOuterErrorMessage = "An error occured while deserializing a XML document or data.";
            const string expectedErrorMessage = "There is an error in XML document (2, 2).";
            const string expectedInnerErrorMessage = "<issueContainer xmlns='urn:sonarqube.org:2019:8.1'> was not expected.";

            // Load resources.
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            using Stream stream = currentAssembly.GetManifestResourceStream(INVALID_SAMPLE2_XML_RESOURCE_PATH);
            using MemoryStream xmlStream = new MemoryStream();

            // Load XML
            await stream.CopyToAsync(xmlStream);
            byte[] xmlData = xmlStream.ToArray();

            // Test deserialization.
            var error = Assert.Throws<DeserializeException>(() => XmlUtilities.Deserialize<IssuesContainer>(xmlData));

            // Assert result.
            Assert.NotNull(error);
            Assert.NotNull(error.InnerException);
            Assert.NotNull(error.InnerException.InnerException);
            Assert.IsType<InvalidOperationException>(error.InnerException);
            Assert.IsType<InvalidOperationException>(error.InnerException.InnerException);
            Assert.Equal(expectedOuterErrorMessage, error.Message);
            Assert.Equal(expectedErrorMessage, error.InnerException.Message);
            Assert.Equal(expectedInnerErrorMessage, error.InnerException.InnerException.Message);
        }

        /// <summary>
        /// Test deserialize null XML data.
        /// </summary>
        [Fact]
        public void NullDataDeserializeTest()
        {
            const string expectedErrorMessage = "The XML data are not set.";

            byte[] xmlData = null;

            // Test deserialization.
            var error = Assert.Throws<NullXmlDataException>(() => XmlUtilities.Deserialize<IssuesContainer>(xmlData));

            // Assert result.
            Assert.NotNull(error);
            Assert.Equal(expectedErrorMessage, error.Message);
        }

        /// <summary>
        /// Test deserialize empty XML data.
        /// </summary>
        [Fact]
        public void EmptyDataDeserializeTest()
        {
            const string expectedOuterErrorMessage = "An error occured while deserializing a XML document or data.";
            const string expectedErrorMessage = "There is an error in XML document (0, 0).";
            const string expectedInnerErrorMessage = "Root element is missing.";

            byte[] xmlData = new byte[0];

            // Test deserialization.
            var error = Assert.Throws<DeserializeException>(() => XmlUtilities.Deserialize<IssuesContainer>(xmlData));

            // Assert result.
            Assert.NotNull(error);
            Assert.NotNull(error.InnerException);
            Assert.NotNull(error.InnerException.InnerException);
            Assert.IsType<InvalidOperationException>(error.InnerException);
            Assert.IsType<XmlException>(error.InnerException.InnerException);
            Assert.Equal(expectedOuterErrorMessage, error.Message);
            Assert.Equal(expectedErrorMessage, error.InnerException.Message);
            Assert.Equal(expectedInnerErrorMessage, error.InnerException.InnerException.Message);
        }

        /// <summary>
        /// Assert IssuesContainer result.
        /// </summary>
        /// <param name="result">Test result to assert.</param>
        private void AssertIssuesContainer(IssuesContainer result)
        {
            const int expectedIssueCount = 2;
            const string expectedEngineId = "1";
            const string expectedRuleId1 = "A440";
            const string expectedRuleId2 = "A442";
            const Severity expectedSeverity1 = Severity.Minor;
            const Severity expectedSeverity2 = Severity.Major;
            const string expectedPrimaryLocationMessage1 = "Test 1";
            const string expectedPrimaryLocationMessage2 = "Test 2";
            const string expectedPrimaryFilePath1 = @"C:\Test1.xml";
            const string expectedPrimaryFilePath2 = @"C:\Test2.xml";
            const int expectedEffortInMinutes1 = 90;
            const int expectedEffortInMinutes2 = 1;

            Assert.NotNull(result);
            Assert.NotNull(result.Issues);
            Assert.Equal(expectedIssueCount, result.Issues.Count);

            var issue1 = result.Issues[0];
            Assert.NotNull(issue1);
            Assert.Equal(expectedEngineId, issue1.EngineId);
            Assert.Equal(expectedRuleId1, issue1.RuleId);
            Assert.True(issue1.SeveritySpecified);
            Assert.Equal(expectedSeverity1, issue1.Severity);
            Assert.NotNull(issue1.PrimaryLocation);
            Assert.Equal(expectedPrimaryLocationMessage1, issue1.PrimaryLocation.Message);
            Assert.Equal(expectedPrimaryFilePath1, issue1.PrimaryLocation.FilePath);
            Assert.True(issue1.EffortMinutesSpecified);
            Assert.Equal(expectedEffortInMinutes1, issue1.EffortMinutes);

            var issue2 = result.Issues[1];
            Assert.NotNull(issue2);
            Assert.Equal(expectedEngineId, issue2.EngineId);
            Assert.Equal(expectedRuleId2, issue2.RuleId);
            Assert.True(issue1.SeveritySpecified);
            Assert.Equal(expectedSeverity2, issue2.Severity);
            Assert.NotNull(issue2.PrimaryLocation);
            Assert.Equal(expectedPrimaryLocationMessage2, issue2.PrimaryLocation.Message);
            Assert.Equal(expectedPrimaryFilePath2, issue2.PrimaryLocation.FilePath);
            Assert.True(issue2.EffortMinutesSpecified);
            Assert.Equal(expectedEffortInMinutes2, issue2.EffortMinutes);
        }

        /// <summary>
        /// Test serialize object in XML Document.
        /// </summary>
        [Fact]
        public void SerializeXmlTest()
        {
            IssuesContainer actualContainer = BuildIssuesContainer();

            // Load resources.
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            using Stream xmlStream = currentAssembly.GetManifestResourceStream(SAMPLE_XML_RESOURCE_PATH);

            // Load XML
            XmlDocument xml = new XmlDocument();
            xml.Load(xmlStream);

            // Test deserialization.
            var result = XmlUtilities.Serialize(actualContainer);

            

            //// Assert result.
            //AssertIssuesContainer(result);
        }

        ///// <summary>
        ///// Test deserialize null XML Document.
        ///// </summary>
        //[Fact]
        //public void NullObjectDeserializeTest()
        //{
        //    const string expectedErrorMessage = "The XML document is not set.";

        //    XmlDocument xml = null;

        //    // Test deserialization.
        //    var error = Assert.Throws<NullXmlDocumentException>(() => XmlUtilities.Deserialize<IssuesContainer>(xml));

        //    // Assert result.
        //    Assert.NotNull(error);
        //    Assert.Equal(expectedErrorMessage, error.Message);
        //}

        ///// <summary>
        ///// Test deserialize invalid XML Document.
        ///// </summary>
        //[Fact]
        //public void InvalidDocument1DeserializeTest()
        //{
        //    const string expectedOuterErrorMessage = "An error occured while deserializing a XML document or data.";
        //    const string expectedErrorMessage = "There is an error in the XML document.";
        //    const string expectedInnerErrorMessage = "Input string was not in a correct format.";

        //    // Load resources.
        //    Assembly currentAssembly = Assembly.GetExecutingAssembly();
        //    using Stream xmlStream = currentAssembly.GetManifestResourceStream(INVALID_SAMPLE1_XML_RESOURCE_PATH);

        //    // Load XML
        //    XmlDocument xml = new XmlDocument();
        //    xml.Load(xmlStream);

        //    // Test deserialization.
        //    var error = Assert.Throws<DeserializeException>(() => XmlUtilities.Deserialize<IssuesContainer>(xml));

        //    // Assert result.
        //    Assert.NotNull(error);
        //    Assert.NotNull(error.InnerException);
        //    Assert.NotNull(error.InnerException.InnerException);
        //    Assert.IsType<InvalidOperationException>(error.InnerException);
        //    Assert.IsType<FormatException>(error.InnerException.InnerException);
        //    Assert.Equal(expectedOuterErrorMessage, error.Message);
        //    Assert.Equal(expectedErrorMessage, error.InnerException.Message);
        //    Assert.Equal(expectedInnerErrorMessage, error.InnerException.InnerException.Message);
        //}

        ///// <summary>
        ///// Test deserialize invalid XML Document.
        ///// </summary>
        //[Fact]
        //public void InvalidDocument2DeserializeTest()
        //{
        //    const string expectedOuterErrorMessage = "An error occured while deserializing a XML document or data.";
        //    const string expectedErrorMessage = "There is an error in the XML document.";
        //    const string expectedInnerErrorMessage = "<issueContainer xmlns='urn:sonarqube.org:2019:8.1'> was not expected.";

        //    // Load resources.
        //    Assembly currentAssembly = Assembly.GetExecutingAssembly();
        //    using Stream xmlStream = currentAssembly.GetManifestResourceStream(INVALID_SAMPLE2_XML_RESOURCE_PATH);

        //    // Load XML
        //    XmlDocument xml = new XmlDocument();
        //    xml.Load(xmlStream);

        //    // Test deserialization.
        //    var error = Assert.Throws<DeserializeException>(() => XmlUtilities.Deserialize<IssuesContainer>(xml));

        //    // Assert result.
        //    Assert.NotNull(error);
        //    Assert.NotNull(error.InnerException);
        //    Assert.NotNull(error.InnerException.InnerException);
        //    Assert.IsType<InvalidOperationException>(error.InnerException);
        //    Assert.IsType<InvalidOperationException>(error.InnerException.InnerException);
        //    Assert.Equal(expectedOuterErrorMessage, error.Message);
        //    Assert.Equal(expectedErrorMessage, error.InnerException.Message);
        //    Assert.Equal(expectedInnerErrorMessage, error.InnerException.InnerException.Message);
        //}

        ///// <summary>
        ///// Test deserialize null XML Document element.
        ///// </summary>
        //[Fact]
        //public void NullElementDeserializeTest()
        //{
        //    const string expectedErrorMessage = "The XML document is not set.";

        //    XmlDocument xml = new XmlDocument();

        //    // Test deserialization.
        //    var error = Assert.Throws<NullXmlDocumentException>(() => XmlUtilities.Deserialize<IssuesContainer>(xml));

        //    // Assert result.
        //    Assert.NotNull(error);
        //    Assert.Equal(expectedErrorMessage, error.Message);
        //}

        ///// <summary>
        ///// Test deserialize XML data.
        ///// </summary>
        //[Fact]
        //public async Task DeserializeDataTest()
        //{
        //    // Load resources.
        //    Assembly currentAssembly = Assembly.GetExecutingAssembly();
        //    using Stream stream = currentAssembly.GetManifestResourceStream(SAMPLE_XML_RESOURCE_PATH);
        //    using MemoryStream xmlStream = new MemoryStream();

        //    // Load XML
        //    await stream.CopyToAsync(xmlStream);
        //    byte[] xmlData = xmlStream.ToArray();

        //    // Test deserialization.
        //    var result = XmlUtilities.Deserialize<IssuesContainer>(xmlData);

        //    // Assert result.
        //    AssertIssuesContainer(result);
        //}

        ///// <summary>
        ///// Test deserialize invalid XML Document.
        ///// </summary>
        //[Fact]
        //public async Task InvalidData1DeserializeTest()
        //{
        //    const string expectedOuterErrorMessage = "An error occured while deserializing a XML document or data.";
        //    const string expectedErrorMessage = "There is an error in XML document (22, 5).";
        //    const string expectedInnerErrorMessage = "Input string was not in a correct format.";

        //    // Load resources.
        //    Assembly currentAssembly = Assembly.GetExecutingAssembly();
        //    using Stream stream = currentAssembly.GetManifestResourceStream(INVALID_SAMPLE1_XML_RESOURCE_PATH);
        //    using MemoryStream xmlStream = new MemoryStream();

        //    // Load XML
        //    await stream.CopyToAsync(xmlStream);
        //    byte[] xmlData = xmlStream.ToArray();

        //    // Test deserialization.
        //    var error = Assert.Throws<DeserializeException>(() => XmlUtilities.Deserialize<IssuesContainer>(xmlData));

        //    // Assert result.
        //    Assert.NotNull(error);
        //    Assert.NotNull(error.InnerException);
        //    Assert.NotNull(error.InnerException.InnerException);
        //    Assert.IsType<InvalidOperationException>(error.InnerException);
        //    Assert.IsType<FormatException>(error.InnerException.InnerException);
        //    Assert.Equal(expectedOuterErrorMessage, error.Message);
        //    Assert.Equal(expectedErrorMessage, error.InnerException.Message);
        //    Assert.Equal(expectedInnerErrorMessage, error.InnerException.InnerException.Message);
        //}

        ///// <summary>
        ///// Test deserialize invalid XML Document.
        ///// </summary>
        //[Fact]
        //public async Task InvalidData2DeserializeTest()
        //{
        //    const string expectedOuterErrorMessage = "An error occured while deserializing a XML document or data.";
        //    const string expectedErrorMessage = "There is an error in XML document (2, 2).";
        //    const string expectedInnerErrorMessage = "<issueContainer xmlns='urn:sonarqube.org:2019:8.1'> was not expected.";

        //    // Load resources.
        //    Assembly currentAssembly = Assembly.GetExecutingAssembly();
        //    using Stream stream = currentAssembly.GetManifestResourceStream(INVALID_SAMPLE2_XML_RESOURCE_PATH);
        //    using MemoryStream xmlStream = new MemoryStream();

        //    // Load XML
        //    await stream.CopyToAsync(xmlStream);
        //    byte[] xmlData = xmlStream.ToArray();

        //    // Test deserialization.
        //    var error = Assert.Throws<DeserializeException>(() => XmlUtilities.Deserialize<IssuesContainer>(xmlData));

        //    // Assert result.
        //    Assert.NotNull(error);
        //    Assert.NotNull(error.InnerException);
        //    Assert.NotNull(error.InnerException.InnerException);
        //    Assert.IsType<InvalidOperationException>(error.InnerException);
        //    Assert.IsType<InvalidOperationException>(error.InnerException.InnerException);
        //    Assert.Equal(expectedOuterErrorMessage, error.Message);
        //    Assert.Equal(expectedErrorMessage, error.InnerException.Message);
        //    Assert.Equal(expectedInnerErrorMessage, error.InnerException.InnerException.Message);
        //}

        ///// <summary>
        ///// Test deserialize null XML data.
        ///// </summary>
        //[Fact]
        //public void NullDataDeserializeTest()
        //{
        //    const string expectedErrorMessage = "The XML data are not set.";

        //    byte[] xmlData = null;

        //    // Test deserialization.
        //    var error = Assert.Throws<NullXmlDataException>(() => XmlUtilities.Deserialize<IssuesContainer>(xmlData));

        //    // Assert result.
        //    Assert.NotNull(error);
        //    Assert.Equal(expectedErrorMessage, error.Message);
        //}

        ///// <summary>
        ///// Test deserialize empty XML data.
        ///// </summary>
        //[Fact]
        //public void EmptyDataDeserializeTest()
        //{
        //    const string expectedOuterErrorMessage = "An error occured while deserializing a XML document or data.";
        //    const string expectedErrorMessage = "There is an error in XML document (0, 0).";
        //    const string expectedInnerErrorMessage = "Root element is missing.";

        //    byte[] xmlData = new byte[0];

        //    // Test deserialization.
        //    var error = Assert.Throws<DeserializeException>(() => XmlUtilities.Deserialize<IssuesContainer>(xmlData));

        //    // Assert result.
        //    Assert.NotNull(error);
        //    Assert.NotNull(error.InnerException);
        //    Assert.NotNull(error.InnerException.InnerException);
        //    Assert.IsType<InvalidOperationException>(error.InnerException);
        //    Assert.IsType<XmlException>(error.InnerException.InnerException);
        //    Assert.Equal(expectedOuterErrorMessage, error.Message);
        //    Assert.Equal(expectedErrorMessage, error.InnerException.Message);
        //    Assert.Equal(expectedInnerErrorMessage, error.InnerException.InnerException.Message);
        //}

        /// <summary>
        /// Build IssuesContainer.
        /// </summary>
        /// <param name="result"></param>
        private IssuesContainer BuildIssuesContainer()
        {
            const string actualEngineId = "1";
            const string actualRuleId1 = "A440";
            const string actualRuleId2 = "A442";
            const Severity actualSeverity1 = Severity.Minor;
            const Severity actualSeverity2 = Severity.Major;
            const string actualPrimaryLocationMessage1 = "Test 1";
            const string actualPrimaryLocationMessage2 = "Test 2";
            const string actualPrimaryFilePath1 = @"C:\Test1.xml";
            const string actualPrimaryFilePath2 = @"C:\Test2.xml";
            const int actualEffortInMinutes1 = 90;
            const int actualEffortInMinutes2 = 1;

            IssuesContainer container = new IssuesContainer();
            container.Issues.Add(new Issue
            {
                EngineId = actualEngineId,
                RuleId = actualRuleId1,
                Severity = actualSeverity1,
                SeveritySpecified = true,
                PrimaryLocation = new LocationType
                {
                    Message = actualPrimaryLocationMessage1,
                    FilePath = actualPrimaryFilePath1,
                },
                EffortMinutes = actualEffortInMinutes1,
                EffortMinutesSpecified = true
            });
            container.Issues.Add(new Issue
            {
                EngineId = actualEngineId,
                RuleId = actualRuleId2,
                Severity = actualSeverity2,
                SeveritySpecified = true,
                PrimaryLocation = new LocationType
                {
                    Message = actualPrimaryLocationMessage2,
                    FilePath = actualPrimaryFilePath2,
                },
                EffortMinutes = actualEffortInMinutes2,
                EffortMinutesSpecified = true
            });

            return container;
        }

        #endregion Serialization / Deserialization

        #region Validation

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
            var schemaSet = XmlUtilities.LoadXmlSchemaSet(xsdStream);

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
            bool isValid = XmlUtilities.TryLoadXmlSchemaSet(new[] { xsdStream }, out var schemaSet, out var errors);

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
            var schemaSet = XmlUtilities.LoadXmlSchemaSet(xsdStream, linkedXsdStream);

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
            bool isValid = XmlUtilities.TryLoadXmlSchemaSet(new[] { xsdStream, linkedXsdStream }, out var schemaSet, out var errors);

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
            var error = Assert.Throws<LoadingSchemaSetException>(() => XmlUtilities.LoadXmlSchemaSet(xsdStream));

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
            bool isValid = XmlUtilities.TryLoadXmlSchemaSet(new[] { xsdStream }, out var schemaSet, out var error);

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
            var error = Assert.Throws<LoadingSchemaSetException>(() => XmlUtilities.LoadXmlSchemaSet(xsdStream));

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
            bool isValid = XmlUtilities.TryLoadXmlSchemaSet(new[] { xsdStream }, out var schemaSet, out var error);

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
            var schemaSet = XmlUtilities.LoadXmlSchemaSet(xsdStream);

            // Load XML
            XmlDocument xml = new XmlDocument();
            xml.Load(xmlStream);

            // Test XML validation.
            bool isValid = XmlUtilities.ValidateXml(xml, schemaSet);

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
            var schemaSet = XmlUtilities.LoadXmlSchemaSet(xsdStream);

            // Load XML
            XmlDocument xml = new XmlDocument();
            xml.Load(xmlStream);

            // Test XML validation.
            bool isValid = XmlUtilities.ValidateXml(xml, schemaSet, true);

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
            var schemaSet = XmlUtilities.LoadXmlSchemaSet(xsdStream);

            // Load XML
            XmlDocument xml = new XmlDocument();
            xml.Load(xmlStream);

            // Test XML validation.
            bool isValid = XmlUtilities.TryValidateXml(xml, schemaSet, out var error);

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
            var schemaSet = XmlUtilities.LoadXmlSchemaSet(xsdStream, linkedXsdStream);

            // Load XML
            XmlDocument xml = new XmlDocument();
            xml.Load(xmlStream);

            // Test XML validation.
            bool isValid = XmlUtilities.ValidateXml(xml, schemaSet);

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
            var schemaSet = XmlUtilities.LoadXmlSchemaSet(xsdStream, linkedXsdStream);

            // Load XML
            XmlDocument xml = new XmlDocument();
            xml.Load(xmlStream);

            // Test XML validation.
            bool isValid = XmlUtilities.ValidateXml(xml, schemaSet, true);

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
            var schemaSet = XmlUtilities.LoadXmlSchemaSet(xsdStream, linkedXsdStream);

            // Load XML
            XmlDocument xml = new XmlDocument();
            xml.Load(xmlStream);

            // Test XML validation.
            bool isValid = XmlUtilities.TryValidateXml(xml, schemaSet, out var error);

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
            var schemaSet = XmlUtilities.LoadXmlSchemaSet(xsdStream);

            // Load XML
            XmlDocument xml = new XmlDocument();
            xml.Load(xmlStream);

            // Test XML validation.
            bool isValid = XmlUtilities.ValidateXml(xml, schemaSet);

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
            var schemaSet = XmlUtilities.LoadXmlSchemaSet(xsdStream);
            // Load XML
            XmlDocument xml = new XmlDocument();
            xml.Load(xmlStream);

            // Test XML validation.
            var error = Assert.Throws<XmlValidationException>(() => XmlUtilities.ValidateXml(xml, schemaSet, true));

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
            var schemaSet = XmlUtilities.LoadXmlSchemaSet(xsdStream);

            // Load XML
            XmlDocument xml = new XmlDocument();
            xml.Load(xmlStream);

            // Test XML validation.
            bool isValid = XmlUtilities.TryValidateXml(xml, schemaSet, out var error);

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
            var schemaSet = XmlUtilities.LoadXmlSchemaSet(xsdStream);

            // Load XML
            XmlDocument xml = new XmlDocument();
            xml.Load(xmlStream);

            // Test XML validation.
            bool isValid = XmlUtilities.ValidateXml(xml, schemaSet);

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
            var schemaSet = XmlUtilities.LoadXmlSchemaSet(xsdStream);
            // Load XML
            XmlDocument xml = new XmlDocument();
            xml.Load(xmlStream);

            // Test XML validation.
            var error = Assert.Throws<XmlValidationException>(() => XmlUtilities.ValidateXml(xml, schemaSet, true));

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
            var schemaSet = XmlUtilities.LoadXmlSchemaSet(xsdStream);

            // Load XML
            XmlDocument xml = new XmlDocument();
            xml.Load(xmlStream);

            // Test XML validation.
            bool isValid = XmlUtilities.TryValidateXml(xml, schemaSet, out var error);

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
            var schemaSet = XmlUtilities.LoadXmlSchemaSet(xsdStream, linkedXsdStream);

            // Load XML
            XmlDocument xml = new XmlDocument();
            xml.Load(xmlStream);

            // Test XML validation.
            bool isValid = XmlUtilities.ValidateXml(xml, schemaSet);

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
            var schemaSet = XmlUtilities.LoadXmlSchemaSet(xsdStream, linkedXsdStream);
            // Load XML
            XmlDocument xml = new XmlDocument();
            xml.Load(xmlStream);

            // Test XML validation.
            var error = Assert.Throws<XmlValidationException>(() => XmlUtilities.ValidateXml(xml, schemaSet, true));

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
            var schemaSet = XmlUtilities.LoadXmlSchemaSet(xsdStream, linkedXsdStream);

            // Load XML
            XmlDocument xml = new XmlDocument();
            xml.Load(xmlStream);

            // Test XML validation.
            bool isValid = XmlUtilities.TryValidateXml(xml, schemaSet, out var error);

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

        #endregion Validation
    }
}
