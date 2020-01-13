using Puffix.Utilities;
using Puffix.Utilities.Exceptions.XmlUtilities;
using System;
using System.IO;
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
        /// Path for the XML sample resource.
        /// </summary>
        private const string SAMPLE_XML_RESOURCE_PATH = "Tests.Puffix.Utilities.Resources.ValidXml.xml";

        /// <summary>
        /// Path for the first XML invalid sample.
        /// </summary>
        private const string INVALID_SAMPLE1_XML_RESOURCE_PATH = "Tests.Puffix.Utilities.Resources.InvalidXml1.xml";

        /// <summary>
        /// Path for the second XML invalid sample.
        /// </summary>
        private const string INVALID_SAMPLE2_XML_RESOURCE_PATH = "Tests.Puffix.Utilities.Resources.InvalidXml2.xml";

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
            var result = XmlUtilities<IssuesContainer>.Deserialize(xml);

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
            var error = Assert.Throws<NullXmlDocumentException>(() => XmlUtilities<IssuesContainer>.Deserialize(xml));

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
            var error = Assert.Throws<DeserializeException>(() => XmlUtilities<IssuesContainer>.Deserialize(xml));

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
            var error = Assert.Throws<DeserializeException>(() => XmlUtilities<IssuesContainer>.Deserialize(xml));

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
            var error = Assert.Throws<NullXmlDocumentException>(() => XmlUtilities<IssuesContainer>.Deserialize(xml));

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
            var result = XmlUtilities<IssuesContainer>.Deserialize(xmlData);

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
            var error = Assert.Throws<DeserializeException>(() => XmlUtilities<IssuesContainer>.Deserialize(xmlData));

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
            var error = Assert.Throws<DeserializeException>(() => XmlUtilities<IssuesContainer>.Deserialize(xmlData));

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
            var error = Assert.Throws<NullXmlDataException>(() => XmlUtilities<IssuesContainer>.Deserialize(xmlData));

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
            var error = Assert.Throws<DeserializeException>(() => XmlUtilities<IssuesContainer>.Deserialize(xmlData));
            
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
        /// <param name="result"></param>
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
    }
}
