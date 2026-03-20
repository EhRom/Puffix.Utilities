using NUnit.Framework;
using Puffix.Utilities;
using Puffix.Utilities.Exceptions.XmlUtilities;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Tests.Puffix.Utilities.Resources;

namespace Tests.Puffix.Utilities;

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

    #region Serialization / Deserialization

    /// <summary>
    /// Test deserialize XML Document.
    /// </summary>
    [Test]
    public void DeserializeXmlTest()
    {
        // Load resources.
        Assembly currentAssembly = Assembly.GetExecutingAssembly();
        using Stream xmlStream = currentAssembly.GetManifestResourceStream(SAMPLE_XML_RESOURCE_PATH);

        // Load XML
        XmlDocument xml = new XmlDocument();
        xml.Load(xmlStream);

        // Test deserialization.
        IssuesContainer result = XmlUtilities.Deserialize<IssuesContainer>(xml);

        // Assert result.
        AssertIssuesContainer(result);
    }

    /// <summary>
    /// Test deserialize null XML Document.
    /// </summary>
    [Test]
    public void NullObjectDeserializeTest()
    {
        const string expectedErrorMessage = "The XML document is not set.";

        XmlDocument xml = null;

        // Test deserialization.
        NullXmlDocumentException error = Assert.Throws<NullXmlDocumentException>(() => XmlUtilities.Deserialize<IssuesContainer>(xml));

        // Assert result.

        using (Assert.EnterMultipleScope())
        {
            Assert.That(error, Is.Not.Null);
            Assert.That(error.Message, Is.EqualTo(expectedErrorMessage));
        }
    }

    /// <summary>
    /// Test deserialize invalid XML Document.
    /// </summary>
    [Test]
    public void InvalidDocument1DeserializeTest()
    {
        const string expectedOuterErrorMessage = "An error occured while deserializing a XML document or data.";
        const string expectedErrorMessage = "There is an error in the XML document.";
        const string expectedInnerErrorMessage = "The input string 'AH AH, c'est une chaîne de caractères.' was not in a correct format.";

        // Load resources.
        Assembly currentAssembly = Assembly.GetExecutingAssembly();
        using Stream xmlStream = currentAssembly.GetManifestResourceStream(INVALID_SAMPLE1_XML_RESOURCE_PATH);

        // Load XML
        XmlDocument xml = new XmlDocument();
        xml.Load(xmlStream);

        // Test deserialization.
        DeserializeException error = Assert.Throws<DeserializeException>(() => XmlUtilities.Deserialize<IssuesContainer>(xml));

        // Assert result.
        using (Assert.EnterMultipleScope())
        {
            Assert.That(error, Is.Not.Null);
            Assert.That(error.Message, Is.EqualTo(expectedOuterErrorMessage));

            Assert.That(error.InnerException, Is.Not.Null);
            Assert.That(error.InnerException, Is.InstanceOf<InvalidOperationException>());
            Assert.That(error.InnerException.Message, Is.EqualTo(expectedErrorMessage));

            Assert.That(error.InnerException.InnerException, Is.Not.Null);
            Assert.That(error.InnerException.InnerException, Is.InstanceOf<FormatException>());
            Assert.That(error.InnerException.InnerException.Message, Is.EqualTo(expectedInnerErrorMessage));
        }
    }

    /// <summary>
    /// Test deserialize invalid XML Document.
    /// </summary>
    [Test]
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
        DeserializeException error = Assert.Throws<DeserializeException>(() => XmlUtilities.Deserialize<IssuesContainer>(xml));

        // Assert result.
        using (Assert.EnterMultipleScope())
        {
            Assert.That(error, Is.Not.Null);
            Assert.That(error.Message, Is.EqualTo(expectedOuterErrorMessage));

            Assert.That(error.InnerException, Is.Not.Null);
            Assert.That(error.InnerException, Is.InstanceOf<InvalidOperationException>());
            Assert.That(error.InnerException.Message, Is.EqualTo(expectedErrorMessage));

            Assert.That(error.InnerException.InnerException, Is.Not.Null);
            Assert.That(error.InnerException.InnerException, Is.InstanceOf<InvalidOperationException>());
            Assert.That(error.InnerException.InnerException.Message, Is.EqualTo(expectedInnerErrorMessage));
        }
    }

    /// <summary>
    /// Test deserialize null XML Document element.
    /// </summary>
    [Test]
    public void NullElementDeserializeTest()
    {
        const string expectedErrorMessage = "The XML document is not set.";

        XmlDocument xml = new XmlDocument();

        // Test deserialization.
        NullXmlDocumentException error = Assert.Throws<NullXmlDocumentException>(() => XmlUtilities.Deserialize<IssuesContainer>(xml));

        // Assert result.
        using (Assert.EnterMultipleScope())
        {
            Assert.That(error, Is.Not.Null);
            Assert.That(error.Message, Is.EqualTo(expectedErrorMessage));
        }
    }

    /// <summary>
    /// Test deserialize XML data.
    /// </summary>
    [Test]
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
        IssuesContainer result = XmlUtilities.Deserialize<IssuesContainer>(xmlData);

        // Assert result.
        AssertIssuesContainer(result);
    }

    /// <summary>
    /// Test deserialize invalid XML Document.
    /// </summary>
    [Test]
    public async Task InvalidData1DeserializeTest()
    {
        const string expectedOuterErrorMessage = "An error occured while deserializing a XML document or data.";
        const string expectedErrorMessage = "There is an error in XML document (22, 5).";
        const string expectedInnerErrorMessage = "The input string 'AH AH, c'est une chaîne de caractères.' was not in a correct format."; // "Input string was not in a correct format.";

        // Load resources.
        Assembly currentAssembly = Assembly.GetExecutingAssembly();
        using Stream stream = currentAssembly.GetManifestResourceStream(INVALID_SAMPLE1_XML_RESOURCE_PATH);
        using MemoryStream xmlStream = new MemoryStream();

        // Load XML
        await stream.CopyToAsync(xmlStream);
        byte[] xmlData = xmlStream.ToArray();



        // Test deserialization.
        DeserializeException error = Assert.Throws<DeserializeException>(() => XmlUtilities.Deserialize<IssuesContainer>(xmlData));

        // Assert result.
        using (Assert.EnterMultipleScope())
        {
            Assert.That(error, Is.Not.Null);
            Assert.That(error.Message, Is.EqualTo(expectedOuterErrorMessage));

            Assert.That(error.InnerException, Is.Not.Null);
            Assert.That(error.InnerException, Is.InstanceOf<InvalidOperationException>());
            Assert.That(error.InnerException.Message, Is.EqualTo(expectedErrorMessage));

            Assert.That(error.InnerException.InnerException, Is.Not.Null);
            Assert.That(error.InnerException.InnerException, Is.InstanceOf<FormatException>());
            Assert.That(error.InnerException.InnerException.Message, Is.EqualTo(expectedInnerErrorMessage));
        }
    }

    /// <summary>
    /// Test deserialize invalid XML Document.
    /// </summary>
    [Test]
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
        DeserializeException error = Assert.Throws<DeserializeException>(() => XmlUtilities.Deserialize<IssuesContainer>(xmlData));

        // Assert result.
        using (Assert.EnterMultipleScope())
        {
            Assert.That(error, Is.Not.Null);
            Assert.That(error.Message, Is.EqualTo(expectedOuterErrorMessage));

            Assert.That(error.InnerException, Is.Not.Null);
            Assert.That(error.InnerException, Is.InstanceOf<InvalidOperationException>());
            Assert.That(error.InnerException.Message, Is.EqualTo(expectedErrorMessage));

            Assert.That(error.InnerException.InnerException, Is.Not.Null);
            Assert.That(error.InnerException.InnerException, Is.InstanceOf<InvalidOperationException>());
            Assert.That(error.InnerException.InnerException.Message, Is.EqualTo(expectedInnerErrorMessage));
        }
    }

    /// <summary>
    /// Test deserialize null XML data.
    /// </summary>
    [Test]
    public void NullDataDeserializeTest()
    {
        const string expectedErrorMessage = "The XML data are not set.";

        byte[] xmlData = null;

        // Test deserialization.
        NullXmlDataException error = Assert.Throws<NullXmlDataException>(() => XmlUtilities.Deserialize<IssuesContainer>(xmlData));

        // Assert result.
        using (Assert.EnterMultipleScope())
        {
            Assert.That(error, Is.Not.Null);
            Assert.That(error.Message, Is.EqualTo(expectedErrorMessage));
        }
    }

    /// <summary>
    /// Test deserialize empty XML data.
    /// </summary>
    [Test]
    public void EmptyDataDeserializeTest()
    {
        const string expectedOuterErrorMessage = "An error occured while deserializing a XML document or data.";
        const string expectedErrorMessage = "There is an error in XML document (0, 0).";
        const string expectedInnerErrorMessage = "Root element is missing.";

        byte[] xmlData = new byte[0];

        // Test deserialization.
        DeserializeException error = Assert.Throws<DeserializeException>(() => XmlUtilities.Deserialize<IssuesContainer>(xmlData));

        // Assert result.
        using (Assert.EnterMultipleScope())
        {
            Assert.That(error, Is.Not.Null);
            Assert.That(error.Message, Is.EqualTo(expectedOuterErrorMessage));

            Assert.That(error.InnerException, Is.Not.Null);
            Assert.That(error.InnerException, Is.InstanceOf<InvalidOperationException>());
            Assert.That(error.InnerException.Message, Is.EqualTo(expectedErrorMessage));

            Assert.That(error.InnerException.InnerException, Is.Not.Null);
            Assert.That(error.InnerException.InnerException, Is.InstanceOf<XmlException>());
            Assert.That(error.InnerException.InnerException.Message, Is.EqualTo(expectedInnerErrorMessage));
        }
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

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Issues, Is.Not.Null);
            Assert.That(result.Issues.Count, Is.EqualTo(expectedIssueCount));

            Issue issue1 = result.Issues[0];
            Assert.That(issue1, Is.Not.Null);
            Assert.That(issue1.EngineId, Is.EqualTo(expectedEngineId));
            Assert.That(issue1.RuleId, Is.EqualTo(expectedRuleId1));
            Assert.That(issue1.SeveritySpecified, Is.True);
            Assert.That(issue1.Severity, Is.EqualTo(expectedSeverity1));
            Assert.That(issue1.PrimaryLocation, Is.Not.Null);
            Assert.That(issue1.PrimaryLocation.Message, Is.EqualTo(expectedPrimaryLocationMessage1));
            Assert.That(issue1.PrimaryLocation.FilePath, Is.EqualTo(expectedPrimaryFilePath1));
            Assert.That(issue1.EffortMinutesSpecified, Is.True);
            Assert.That(issue1.EffortMinutes, Is.EqualTo(expectedEffortInMinutes1));

            Issue issue2 = result.Issues[1];
            Assert.That(issue2, Is.Not.Null);
            Assert.That(issue2.EngineId, Is.EqualTo(expectedEngineId));
            Assert.That(issue2.RuleId, Is.EqualTo(expectedRuleId2));
            Assert.That(issue2.SeveritySpecified, Is.True);
            Assert.That(issue2.Severity, Is.EqualTo(expectedSeverity2));
            Assert.That(issue2.PrimaryLocation, Is.Not.Null);
            Assert.That(issue2.PrimaryLocation.Message, Is.EqualTo(expectedPrimaryLocationMessage2));
            Assert.That(issue2.PrimaryLocation.FilePath, Is.EqualTo(expectedPrimaryFilePath2));
            Assert.That(issue2.EffortMinutesSpecified, Is.True);
            Assert.That(issue2.EffortMinutes, Is.EqualTo(expectedEffortInMinutes2));
        }
    }

    /// <summary>
    /// Test serialize object in XML Document.
    /// </summary>
    [Test]
    public void SerializeXmlTest()
    {
        // Load the resources & the expected XML Document
        Assembly currentAssembly = Assembly.GetExecutingAssembly();
        using Stream xmlStream = currentAssembly.GetManifestResourceStream(SAMPLE_XML_RESOURCE_PATH);
        XmlDocument expectedXmlDocument = new XmlDocument();
        expectedXmlDocument.Load(xmlStream);

        // Load the data to serialize
        IssuesContainer actualContainer = BuildIssuesContainer();

        // Test deserialization.
        XmlDocument result = XmlUtilities.Serialize(actualContainer, Encoding.UTF8);

        // Check the result.
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(XmlUtilities.Compare(expectedXmlDocument, result), Is.True);
        }
    }

    /// <summary>
    /// Test serialize object as byte array.
    /// </summary>
    [Test]
    public void SerializeXmlAsByteArrayTest()
    {
        // Load the resources & the expected XML Document
        Assembly currentAssembly = Assembly.GetExecutingAssembly();
        using Stream xmlStream = currentAssembly.GetManifestResourceStream(SAMPLE_XML_RESOURCE_PATH);
        XmlDocument expectedXmlDocument = new XmlDocument();
        expectedXmlDocument.Load(xmlStream);

        // Load the data to serialize
        IssuesContainer actualContainer = BuildIssuesContainer();

        // Test deserialization.
        byte[] result = XmlUtilities.SerializeAsByteArray(actualContainer, Encoding.UTF8, true);

        // Check the result.
        using (Assert.EnterMultipleScope())
        {
            // Load the result into a XML Document.
            using MemoryStream actualStream = new MemoryStream(result);
            XmlDocument actualXmlDocument = new XmlDocument();
            actualXmlDocument.Load(actualStream);


            Assert.That(result, Is.Not.Null);
            Assert.That(XmlUtilities.Compare(expectedXmlDocument, actualXmlDocument), Is.True);
        }
    }

    /// <summary>
    /// Test serialize object as stream.
    /// </summary>
    [Test]
    public void SerializeXmlAsStreamTest()
    {
        // Load the resources & the expected XML Document
        Assembly currentAssembly = Assembly.GetExecutingAssembly();
        using Stream xmlStream = currentAssembly.GetManifestResourceStream(SAMPLE_XML_RESOURCE_PATH);
        XmlDocument expectedXmlDocument = new XmlDocument();
        expectedXmlDocument.Load(xmlStream);

        // Load the data to serialize
        IssuesContainer actualContainer = BuildIssuesContainer();

        // Test deserialization.
        using Stream result = XmlUtilities.SerializeAsStream(actualContainer, Encoding.UTF8, false);

        // Check the result.
        using (Assert.EnterMultipleScope())
        {
            // Load the result into a XML Document.
            XmlDocument actualXmlDocument = new XmlDocument();
            actualXmlDocument.Load(result);


            Assert.That(result, Is.Not.Null);
            Assert.That(XmlUtilities.Compare(expectedXmlDocument, actualXmlDocument), Is.True);
        }
    }

    /// <summary>
    /// Test serialize object in XML Document (async).
    /// </summary>
    [Test]
    public async Task SerializeXmlAsyncTest()
    {
        // Load resources.
        Assembly currentAssembly = Assembly.GetExecutingAssembly();
        using Stream xmlStream = currentAssembly.GetManifestResourceStream(SAMPLE_XML_RESOURCE_PATH);

        // Load XML
        XmlDocument expectedXmlDocument = new XmlDocument();
        expectedXmlDocument.Load(xmlStream);

        // Load the data to serialize
        IssuesContainer actualContainer = BuildIssuesContainer();

        // Test deserialization.
        XmlDocument result = await XmlUtilities.SerializeAsync(actualContainer, Encoding.UTF8);

        // Check the result.
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(XmlUtilities.Compare(expectedXmlDocument, result), Is.True);
        }
    }

    /// <summary>
    /// Test serialize object in byte array (async).
    /// </summary>
    [Test]
    public async Task SerializeXmlAsByteArrayAsyncTest()
    {
        // Load the resources & the expected XML Document
        Assembly currentAssembly = Assembly.GetExecutingAssembly();
        using Stream xmlStream = currentAssembly.GetManifestResourceStream(SAMPLE_XML_RESOURCE_PATH);
        XmlDocument expectedXmlDocument = new XmlDocument();
        expectedXmlDocument.Load(xmlStream);

        // Load the data to serialize
        IssuesContainer actualContainer = BuildIssuesContainer();

        // Test deserialization.
        byte[] result = await XmlUtilities.SerializeAsByteArrayAsync(actualContainer, Encoding.UTF8);

        // Check the result.
        using (Assert.EnterMultipleScope())
        {
            // Load the result into a XML Document.
            using MemoryStream actualStream = new MemoryStream(result);
            XmlDocument actualXmlDocument = new XmlDocument();
            actualXmlDocument.Load(actualStream);


            Assert.That(result, Is.Not.Null);
            Assert.That(XmlUtilities.Compare(expectedXmlDocument, actualXmlDocument), Is.True);
        }
    }

    /// <summary>
    /// Test serialize object in stream (async).
    /// </summary>
    [Test]
    public async Task SerializeXmlAsStreamAsyncTest()
    {
        // Load the resources & the expected XML Document
        Assembly currentAssembly = Assembly.GetExecutingAssembly();
        using Stream xmlStream = currentAssembly.GetManifestResourceStream(SAMPLE_XML_RESOURCE_PATH);
        XmlDocument expectedXmlDocument = new XmlDocument();
        expectedXmlDocument.Load(xmlStream);

        // Load the data to serialize
        IssuesContainer actualContainer = BuildIssuesContainer();

        // Test deserialization.
        using Stream result = await XmlUtilities.SerializeAsStreamAsync(actualContainer, Encoding.UTF8);

        // Check the result.
        using (Assert.EnterMultipleScope())
        {
            // Load the result into a XML Document.
            XmlDocument actualXmlDocument = new XmlDocument();
            actualXmlDocument.Load(result);


            Assert.That(result, Is.Not.Null);
            Assert.That(XmlUtilities.Compare(expectedXmlDocument, actualXmlDocument), Is.True);
        }
    }

    /// <summary>
    /// Test serialize null object in XML Document.
    /// </summary>
    [Test]
    public void SerializeNullXmlTest()
    {
        const string expectedErrorMessage = "The object to serialize is not set.";

        // Test deserialization.
        NullObjectToSerializeException error = Assert.Throws<NullObjectToSerializeException>(() => XmlUtilities.Serialize<IssuesContainer>(null));

        // Check the result.
        using (Assert.EnterMultipleScope())
        {
            Assert.That(error, Is.Not.Null);
            Assert.That(error.Message, Is.EqualTo(expectedErrorMessage));
        }
    }

    /// <summary>
    /// Test serialize null object as byte array.
    /// </summary>
    [Test]
    public void SerializeNullXmlAsByteArrayTest()
    {
        const string expectedErrorMessage = "The object to serialize is not set.";

        // Test deserialization.
        NullObjectToSerializeException error = Assert.Throws<NullObjectToSerializeException>(() => XmlUtilities.SerializeAsByteArray<IssuesContainer>(null));

        // Check the result.
        using (Assert.EnterMultipleScope())
        {
            Assert.That(error, Is.Not.Null);
            Assert.That(error.Message, Is.EqualTo(expectedErrorMessage));
        }
    }

    /// <summary>
    /// Test serialize null object as stream.
    /// </summary>
    [Test]
    public void SerializeNullXmlAsStreamTest()
    {
        const string expectedErrorMessage = "The object to serialize is not set.";

        // Test deserialization.
        NullObjectToSerializeException error = Assert.Throws<NullObjectToSerializeException>(() => XmlUtilities.SerializeAsStream<IssuesContainer>(null));

        // Check the result.
        using (Assert.EnterMultipleScope())
        {
            Assert.That(error, Is.Not.Null);
            Assert.That(error.Message, Is.EqualTo(expectedErrorMessage));
        }
    }

    /// <summary>
    /// Test serialize null object in XML Document (async).
    /// </summary>
    [Test]
    public async Task SerializeNullXmlAsyncTest()
    {
        const string expectedErrorMessage = "The object to serialize is not set.";

        // Test deserialization.
        NullObjectToSerializeException error = Assert.ThrowsAsync<NullObjectToSerializeException>(async () => await XmlUtilities.SerializeAsync<IssuesContainer>(null));

        // Check the result.
        using (Assert.EnterMultipleScope())
        {
            Assert.That(error, Is.Not.Null);
            Assert.That(error.Message, Is.EqualTo(expectedErrorMessage));
        }
    }

    /// <summary>
    /// Test serialize null object in byte array (async).
    /// </summary>
    [Test]
    public async Task SerializeNullXmlAsByteArrayAsyncTest()
    {
        const string expectedErrorMessage = "The object to serialize is not set.";

        // Test deserialization.
        NullObjectToSerializeException error = Assert.ThrowsAsync<NullObjectToSerializeException>(async () => await XmlUtilities.SerializeAsByteArrayAsync<IssuesContainer>(null));

        // Check the result.
        using (Assert.EnterMultipleScope())
        {
            Assert.That(error, Is.Not.Null);
            Assert.That(error.Message, Is.EqualTo(expectedErrorMessage));
        }
    }

    /// <summary>
    /// Test serialize null object in stream (async).
    /// </summary>
    [Test]
    public async Task SerializeNullXmlAsStreamAsyncTest()
    {
        const string expectedErrorMessage = "The object to serialize is not set.";

        // Test deserialization.
        NullObjectToSerializeException error = Assert.ThrowsAsync<NullObjectToSerializeException>(async () => await XmlUtilities.SerializeAsStreamAsync<IssuesContainer>(null));

        // Check the result.
        using (Assert.EnterMultipleScope())
        {
            Assert.That(error, Is.Not.Null);
            Assert.That(error.Message, Is.EqualTo(expectedErrorMessage));
        }
    }

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
}
