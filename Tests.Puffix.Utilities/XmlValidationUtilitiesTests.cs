using NUnit.Framework;
using Puffix.Utilities;
using Puffix.Utilities.Exceptions;
using Puffix.Utilities.Exceptions.XmlUtilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;

namespace Tests.Puffix.Utilities;

/// <summary>
/// Test class for the XML files and document helper.
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

    #region Validation

    /// <summary>
    /// Test the XSD load method.
    /// </summary>
    [Test]
    public void LoadSchemaSetTest()
    {
        // Load resources.
        Assembly currentAssembly = Assembly.GetExecutingAssembly();
        using Stream xsdStream = currentAssembly.GetManifestResourceStream(SAMPLE_XSD_RESOURCE_PATH);

        // Test load XML Schema Set.
        XmlSchemaSet schemaSet = XmlValidationUtilities.LoadXmlSchemaSet(xsdStream);

        // Check the result.
        using (Assert.EnterMultipleScope())
        {
            Assert.That(schemaSet, Is.Not.Null);
            Assert.That(schemaSet.IsCompiled, Is.True);
            Assert.That(schemaSet.Count, Is.EqualTo(1));
        }
    }

    /// <summary>
    /// Test the "try" XSD load method.
    /// </summary>
    [Test]
    public void TryLoadSchemaSetTest()
    {
        // Load resources.
        Assembly currentAssembly = Assembly.GetExecutingAssembly();
        using Stream xsdStream = currentAssembly.GetManifestResourceStream(SAMPLE_XSD_RESOURCE_PATH);

        // Test load XML Schema Set.
        bool isValid = XmlValidationUtilities.TryLoadXmlSchemaSet([xsdStream], out XmlSchemaSet schemaSet, out LoadingSchemaSetException errors);

        // Check the result.
        using (Assert.EnterMultipleScope())
        {
            Assert.That(isValid, Is.True);
            Assert.That(errors, Is.Null);

            Assert.That(schemaSet, Is.Not.Null);
            Assert.That(schemaSet.IsCompiled, Is.True);
            Assert.That(schemaSet.Count, Is.EqualTo(1));
        }
    }

    /// <summary>
    /// Test the XSD load method with a linked schema.
    /// </summary>
    [Test]
    public void LoadSchemaSetWithLinkedSchemaTest()
    {
        // Load resources.
        Assembly currentAssembly = Assembly.GetExecutingAssembly();
        using Stream linkedXsdStream = currentAssembly.GetManifestResourceStream(SAMPLE_LINKED_XSD_RESOURCE_PATH);
        using Stream xsdStream = currentAssembly.GetManifestResourceStream(SAMPLE_XSD_RESOURCE_PATH);

        // Test load XML Schema Set.
        XmlSchemaSet schemaSet = XmlValidationUtilities.LoadXmlSchemaSet(xsdStream, linkedXsdStream);

        // Check the result.
        using (Assert.EnterMultipleScope())
        {
            Assert.That(schemaSet, Is.Not.Null);
            Assert.That(schemaSet.IsCompiled, Is.True);
            Assert.That(schemaSet.Count, Is.EqualTo(2));
        }
    }

    /// <summary>
    /// Test the "try" XSD load method with a linked schema.
    /// </summary>
    [Test]
    public void TryLoadSchemaSetWithLinkedSchemaTest()
    {
        // Load resources.
        Assembly currentAssembly = Assembly.GetExecutingAssembly();
        using Stream linkedXsdStream = currentAssembly.GetManifestResourceStream(SAMPLE_LINKED_XSD_RESOURCE_PATH);
        using Stream xsdStream = currentAssembly.GetManifestResourceStream(SAMPLE_XSD_RESOURCE_PATH);

        // Test load XML Schema Set.
        bool isValid = XmlValidationUtilities.TryLoadXmlSchemaSet([xsdStream, linkedXsdStream], out XmlSchemaSet schemaSet, out LoadingSchemaSetException errors);

        // Check the result.
        using (Assert.EnterMultipleScope())
        {
            Assert.That(isValid, Is.True);
            Assert.That(errors, Is.Null);

            Assert.That(schemaSet, Is.Not.Null);
            Assert.That(schemaSet.IsCompiled, Is.True);
            Assert.That(schemaSet.Count, Is.EqualTo(2));
        }
    }

    /// <summary>
    /// Test the XSD load method with invalid XSD.
    /// </summary>
    [Test]
    public void InvalidLoadSchemaSetTest1()
    {
        const string expectedErrorMessage = "The 'xs:restriction' start tag on line 7 position 6 does not match the end tag of 'xs:simpleType'. Line 8, position 5.";

        // Load resources.
        Assembly currentAssembly = Assembly.GetExecutingAssembly();
        using Stream xsdStream = currentAssembly.GetManifestResourceStream(INVALID_SAMPLE1_XSD_RESOURCE_PATH);

        // Test load XML Schema Set.
        LoadingSchemaSetException error = Assert.Throws<LoadingSchemaSetException>(() => XmlValidationUtilities.LoadXmlSchemaSet(xsdStream));

        // Check the result.
        using (Assert.EnterMultipleScope())
        {
            Assert.That(error, Is.Not.Null);
            Assert.That(error.ValidationErrors, Is.Not.Null);
            Assert.That(error.ValidationErrors.Count, Is.EqualTo(1));
            Assert.That(error.ValidationErrors.First().Message, Is.EqualTo(expectedErrorMessage));
        }
    }

    /// <summary>
    /// Test the "try" XSD load method with invalid XSD.
    /// </summary>
    [Test]
    public void InvalidTryLoadSchemaSetTest1()
    {
        const string expectedErrorMessage = "The 'xs:restriction' start tag on line 7 position 6 does not match the end tag of 'xs:simpleType'. Line 8, position 5.";

        // Load resources.
        Assembly currentAssembly = Assembly.GetExecutingAssembly();
        using Stream xsdStream = currentAssembly.GetManifestResourceStream(INVALID_SAMPLE1_XSD_RESOURCE_PATH);

        // Test load XML Schema Set.
        bool isValid = XmlValidationUtilities.TryLoadXmlSchemaSet([xsdStream], out XmlSchemaSet schemaSet, out LoadingSchemaSetException error);

        // Check the result.
        using (Assert.EnterMultipleScope())
        {
            Assert.That(isValid, Is.False);

            Assert.That(schemaSet, Is.Not.Null);
            Assert.That(schemaSet.Count, Is.EqualTo(0));

            Assert.That(error, Is.Not.Null);
            Assert.That(error.ValidationErrors, Is.Not.Null);
            Assert.That(error.ValidationErrors.Count, Is.EqualTo(1));
            Assert.That(error.ValidationErrors.First().Message, Is.EqualTo(expectedErrorMessage));
        }
    }

    /// <summary>
    /// Test the XSD load method with invalid XSD.
    /// </summary>
    [Test]
    public void InvalidLoadSchemaSetTest2()
    {
        const string expectedFirstErrorMessage = "Type 'urn:sonarqube.org:2019:8.1:SeverityType' is not declared.";
        const string expectedSecondErrorMessage = "Type 'urn:sonarqube.org:2019:8.1:IssueTypeType' is not declared.";
        const string expectedThridErrorMessage = "Type 'urn:sonarqube.org:2019:8.1:IssueTypeType' is not declared.";

        // Load resources.
        Assembly currentAssembly = Assembly.GetExecutingAssembly();
        using Stream xsdStream = currentAssembly.GetManifestResourceStream(INVALID_SAMPLE2_XSD_RESOURCE_PATH);

        // Test load XML Schema Set.
        LoadingSchemaSetException error = Assert.Throws<LoadingSchemaSetException>(() => XmlValidationUtilities.LoadXmlSchemaSet(xsdStream));

        // Check the result.
        using (Assert.EnterMultipleScope())
        {
            Assert.That(error, Is.Not.Null);
            Assert.That(error.ValidationErrors, Is.Not.Null);
            Assert.That(error.ValidationErrors.Count, Is.EqualTo(3));

            XmlValidationError[] errors = error.ValidationErrors.ToArray();
            Assert.That(errors[0].Message, Is.EqualTo(expectedFirstErrorMessage));
            Assert.That(errors[1].Message, Is.EqualTo(expectedSecondErrorMessage));
            Assert.That(errors[2].Message, Is.EqualTo(expectedThridErrorMessage));
        }
    }

    /// <summary>
    /// Test the "try" XSD load method with invalid XSD.
    /// </summary>
    [Test]
    public void InvalidTryLoadSchemaSetTest2()
    {
        const string expectedFirstErrorMessage = "Type 'urn:sonarqube.org:2019:8.1:SeverityType' is not declared.";
        const string expectedSecondErrorMessage = "Type 'urn:sonarqube.org:2019:8.1:IssueTypeType' is not declared.";
        const string expectedThridErrorMessage = "Type 'urn:sonarqube.org:2019:8.1:IssueTypeType' is not declared.";

        // Load resources.
        Assembly currentAssembly = Assembly.GetExecutingAssembly();
        using Stream xsdStream = currentAssembly.GetManifestResourceStream(INVALID_SAMPLE2_XSD_RESOURCE_PATH);

        // Test load XML Schema Set.
        bool isValid = XmlValidationUtilities.TryLoadXmlSchemaSet([xsdStream], out XmlSchemaSet schemaSet, out LoadingSchemaSetException error);

        // Check the result.
        using (Assert.EnterMultipleScope())
        {
            Assert.That(isValid, Is.False);

            Assert.That(schemaSet, Is.Not.Null);
            Assert.That(schemaSet.Count, Is.EqualTo(1));

            Assert.That(error, Is.Not.Null);
            Assert.That(error.ValidationErrors, Is.Not.Null);
            Assert.That(error.ValidationErrors.Count, Is.EqualTo(3));

            XmlValidationError[] errors = error.ValidationErrors.ToArray();
            Assert.That(errors[0].Message, Is.EqualTo(expectedFirstErrorMessage));
            Assert.That(errors[1].Message, Is.EqualTo(expectedSecondErrorMessage));
            Assert.That(errors[2].Message, Is.EqualTo(expectedThridErrorMessage));
        }
    }

    /// <summary>
    /// Test the validate XML method.
    /// </summary>
    [Test]
    public void ValidateXmlTest()
    {
        // Load resources.
        Assembly currentAssembly = Assembly.GetExecutingAssembly();
        using Stream xmlStream = currentAssembly.GetManifestResourceStream(SAMPLE_XML_RESOURCE_PATH);
        using Stream xsdStream = currentAssembly.GetManifestResourceStream(SAMPLE_XSD_RESOURCE_PATH);

        // Load Schema set.
        XmlSchemaSet schemaSet = XmlValidationUtilities.LoadXmlSchemaSet(xsdStream);

        // Load XML
        XmlDocument xml = new XmlDocument();
        xml.Load(xmlStream);

        // Test XML validation.
        bool isValid = XmlValidationUtilities.ValidateXml(xml, schemaSet);

        // Check the result.
        using (Assert.EnterMultipleScope())
        {
            Assert.That(isValid, Is.True);
        }
    }

    /// <summary>
    /// Test the validate XML method.
    /// </summary>
    [Test]
    public void ValidateXmlWithThownErrorTest()
    {
        // Load resources.
        Assembly currentAssembly = Assembly.GetExecutingAssembly();
        using Stream xmlStream = currentAssembly.GetManifestResourceStream(SAMPLE_XML_RESOURCE_PATH);
        using Stream xsdStream = currentAssembly.GetManifestResourceStream(SAMPLE_XSD_RESOURCE_PATH);

        // Load Schema set.
        XmlSchemaSet schemaSet = XmlValidationUtilities.LoadXmlSchemaSet(xsdStream);

        // Load XML
        XmlDocument xml = new XmlDocument();
        xml.Load(xmlStream);

        // Test XML validation.
        bool isValid = XmlValidationUtilities.ValidateXml(xml, schemaSet, true);

        // Check the result.
        using (Assert.EnterMultipleScope())
        {
            Assert.That(isValid, Is.True);
        }
    }

    /// <summary>
    /// Test the "try" validate XML method.
    /// </summary>
    [Test]
    public void TryValidateXmlTest()
    {
        // Load resources.
        Assembly currentAssembly = Assembly.GetExecutingAssembly();
        using Stream xmlStream = currentAssembly.GetManifestResourceStream(SAMPLE_XML_RESOURCE_PATH);
        using Stream xsdStream = currentAssembly.GetManifestResourceStream(SAMPLE_XSD_RESOURCE_PATH);

        // Load Schema set.
        XmlSchemaSet schemaSet = XmlValidationUtilities.LoadXmlSchemaSet(xsdStream);

        // Load XML
        XmlDocument xml = new XmlDocument();
        xml.Load(xmlStream);

        // Test XML validation.
        bool isValid = XmlValidationUtilities.TryValidateXml(xml, schemaSet, out XmlValidationException error);

        // Check the result.
        using (Assert.EnterMultipleScope())
        {
            Assert.That(isValid, Is.True);
        }
    }

    /// <summary>
    /// Test the validate linked XML method.
    /// </summary>
    [Test]
    public void ValidateLinkedXmlTest()
    {
        // Load resources.
        Assembly currentAssembly = Assembly.GetExecutingAssembly();
        using Stream xmlStream = currentAssembly.GetManifestResourceStream(SAMPLE_LINKED_XML_RESOURCE_PATH);
        using Stream linkedXsdStream = currentAssembly.GetManifestResourceStream(SAMPLE_LINKED_XSD_RESOURCE_PATH);
        using Stream xsdStream = currentAssembly.GetManifestResourceStream(SAMPLE_XSD_RESOURCE_PATH);

        // Load Schema set.
        XmlSchemaSet schemaSet = XmlValidationUtilities.LoadXmlSchemaSet(xsdStream, linkedXsdStream);

        // Load XML
        XmlDocument xml = new XmlDocument();
        xml.Load(xmlStream);

        // Test XML validation.
        bool isValid = XmlValidationUtilities.ValidateXml(xml, schemaSet);

        // Check the result.
        using (Assert.EnterMultipleScope())
        {
            Assert.That(isValid, Is.True);
        }
    }

    /// <summary>
    /// Test the validate linked XML method.
    /// </summary>
    [Test]
    public void ValidateLinkedXmlWithThownErrorTest()
    {
        // Load resources.
        Assembly currentAssembly = Assembly.GetExecutingAssembly();
        using Stream xmlStream = currentAssembly.GetManifestResourceStream(SAMPLE_LINKED_XML_RESOURCE_PATH);
        using Stream linkedXsdStream = currentAssembly.GetManifestResourceStream(SAMPLE_LINKED_XSD_RESOURCE_PATH);
        using Stream xsdStream = currentAssembly.GetManifestResourceStream(SAMPLE_XSD_RESOURCE_PATH);

        // Load Schema set.
        XmlSchemaSet schemaSet = XmlValidationUtilities.LoadXmlSchemaSet(xsdStream, linkedXsdStream);

        // Load XML
        XmlDocument xml = new XmlDocument();
        xml.Load(xmlStream);

        // Test XML validation.
        bool isValid = XmlValidationUtilities.ValidateXml(xml, schemaSet, true);

        // Check the result.
        using (Assert.EnterMultipleScope())
        {
            Assert.That(isValid, Is.True);
        }
    }

    /// <summary>
    /// Test the "try" validate linked XML method.
    /// </summary>
    [Test]
    public void TryValidateLinkedXmlTest()
    {
        // Load resources.
        Assembly currentAssembly = Assembly.GetExecutingAssembly();
        using Stream xmlStream = currentAssembly.GetManifestResourceStream(SAMPLE_LINKED_XML_RESOURCE_PATH);
        using Stream linkedXsdStream = currentAssembly.GetManifestResourceStream(SAMPLE_LINKED_XSD_RESOURCE_PATH);
        using Stream xsdStream = currentAssembly.GetManifestResourceStream(SAMPLE_XSD_RESOURCE_PATH);

        // Load Schema set.
        XmlSchemaSet schemaSet = XmlValidationUtilities.LoadXmlSchemaSet(xsdStream, linkedXsdStream);

        // Load XML
        XmlDocument xml = new XmlDocument();
        xml.Load(xmlStream);

        // Test XML validation.
        bool isValid = XmlValidationUtilities.TryValidateXml(xml, schemaSet, out XmlValidationException error);

        // Check the result.
        using (Assert.EnterMultipleScope())
        {
            Assert.That(isValid, Is.True);
        }
    }

    /// <summary>
    /// Test the validate linked XML method, with missing schema.
    /// </summary>
    [Test]
    public void ValidateLinkedXmllWithMissingSchemaTest()
    {
        // Load resources.
        Assembly currentAssembly = Assembly.GetExecutingAssembly();
        using Stream xmlStream = currentAssembly.GetManifestResourceStream(SAMPLE_LINKED_XML_RESOURCE_PATH);
        using Stream xsdStream = currentAssembly.GetManifestResourceStream(SAMPLE_XSD_RESOURCE_PATH);

        // Load Schema set.
        XmlSchemaSet schemaSet = XmlValidationUtilities.LoadXmlSchemaSet(xsdStream);

        // Load XML
        XmlDocument xml = new XmlDocument();
        xml.Load(xmlStream);

        // Test XML validation.
        bool isValid = XmlValidationUtilities.ValidateXml(xml, schemaSet);

        // Check the result.
        using (Assert.EnterMultipleScope())
        {
            Assert.That(isValid, Is.False);
        }
    }

    /// <summary>
    /// Test the validate linked XML method, with missing schema.
    /// </summary>
    [Test]
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
        XmlSchemaSet schemaSet = XmlValidationUtilities.LoadXmlSchemaSet(xsdStream);

        // Load XML
        XmlDocument xml = new XmlDocument();
        xml.Load(xmlStream);

        // Test XML validation.
        XmlValidationException error = Assert.Throws<XmlValidationException>(() => XmlValidationUtilities.ValidateXml(xml, schemaSet, true));

        // Check the result.
        using (Assert.EnterMultipleScope())
        {
            Assert.That(schemaSet, Is.Not.Null);
            Assert.That(schemaSet.Count, Is.EqualTo(1));

            Assert.That(error, Is.Not.Null);
            Assert.That(error.Message, Is.EqualTo(expectedErrorMessage));
            Assert.That(error.ValidationErrors, Is.Not.Null);
            Assert.That(error.ValidationErrors.Count, Is.EqualTo(expectedValidationErrorCount));

            Assert.That(error.ValidationErrors.First().Message, Is.EqualTo(expectedValidationErrorMessage));
        }
    }

    /// <summary>
    /// Test the "try" validate linked XML method, with missing schema.
    /// </summary>
    [Test]
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
        XmlSchemaSet schemaSet = XmlValidationUtilities.LoadXmlSchemaSet(xsdStream);

        // Load XML
        XmlDocument xml = new XmlDocument();
        xml.Load(xmlStream);

        // Test XML validation.
        bool isValid = XmlValidationUtilities.TryValidateXml(xml, schemaSet, out XmlValidationException error);

        // Check the result.
        using (Assert.EnterMultipleScope())
        {
            Assert.That(isValid, Is.False);

            Assert.That(schemaSet, Is.Not.Null);
            Assert.That(schemaSet.Count, Is.EqualTo(1));

            Assert.That(error, Is.Not.Null);
            Assert.That(error.Message, Is.EqualTo(expectedErrorMessage));
            Assert.That(error.ValidationErrors, Is.Not.Null);
            Assert.That(error.ValidationErrors.Count, Is.EqualTo(expectedValidationErrorCount));

            Assert.That(error.ValidationErrors.First().Message, Is.EqualTo(expectedValidationErrorMessage));
        }
    }

    /// <summary>
    /// Test the validate linked XML method, with missing schema.
    /// </summary>
    [Test]
    public void ValidateInvalidXml1Test()
    {
        // Load resources.
        Assembly currentAssembly = Assembly.GetExecutingAssembly();
        using Stream xmlStream = currentAssembly.GetManifestResourceStream(INVALID_SAMPLE1_XML_RESOURCE_PATH);
        using Stream xsdStream = currentAssembly.GetManifestResourceStream(SAMPLE_XSD_RESOURCE_PATH);

        // Load Schema set.
        XmlSchemaSet schemaSet = XmlValidationUtilities.LoadXmlSchemaSet(xsdStream);

        // Load XML
        XmlDocument xml = new XmlDocument();
        xml.Load(xmlStream);

        // Test XML validation.
        bool isValid = XmlValidationUtilities.ValidateXml(xml, schemaSet);

        // Check the result.
        using (Assert.EnterMultipleScope())
        {
            Assert.That(isValid, Is.False);
        }
    }

    /// <summary>
    /// Test the validate linked XML method, with missing schema.
    /// </summary>
    [Test]
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
        XmlSchemaSet schemaSet = XmlValidationUtilities.LoadXmlSchemaSet(xsdStream);
        // Load XML
        XmlDocument xml = new XmlDocument();
        xml.Load(xmlStream);

        // Test XML validation.
        XmlValidationException error = Assert.Throws<XmlValidationException>(() => XmlValidationUtilities.ValidateXml(xml, schemaSet, true));

        // Check the result.
        using (Assert.EnterMultipleScope())
        {
            Assert.That(schemaSet, Is.Not.Null);
            Assert.That(schemaSet.Count, Is.EqualTo(1));

            Assert.That(error, Is.Not.Null);
            Assert.That(error.Message, Is.EqualTo(expectedErrorMessage));
            Assert.That(error.ValidationErrors, Is.Not.Null);
            Assert.That(error.ValidationErrors.Count, Is.EqualTo(expectedValidationErrorCount));

            Assert.That(error.ValidationErrors.First().Message, Is.EqualTo(expectedValidationErrorMessage));
        }
    }

    /// <summary>
    /// Test the "try" validate linked XML method, with missing schema.
    /// </summary>
    [Test]
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
        XmlSchemaSet schemaSet = XmlValidationUtilities.LoadXmlSchemaSet(xsdStream);

        // Load XML
        XmlDocument xml = new XmlDocument();
        xml.Load(xmlStream);

        // Test XML validation.
        bool isValid = XmlValidationUtilities.TryValidateXml(xml, schemaSet, out XmlValidationException error);

        // Check the result.
        using (Assert.EnterMultipleScope())
        {
            Assert.That(isValid, Is.False);

            Assert.That(schemaSet, Is.Not.Null);
            Assert.That(schemaSet.Count, Is.EqualTo(1));

            Assert.That(error, Is.Not.Null);
            Assert.That(error.Message, Is.EqualTo(expectedErrorMessage));
            Assert.That(error.ValidationErrors, Is.Not.Null);
            Assert.That(error.ValidationErrors.Count, Is.EqualTo(expectedValidationErrorCount));

            Assert.That(error.ValidationErrors.First().Message, Is.EqualTo(expectedValidationErrorMessage));
        }
    }

    /// <summary>
    /// Test the validate linked XML method, with missing schema.
    /// </summary>
    [Test]
    public void ValidateInvalidXml2Test()
    {
        // Load resources.
        Assembly currentAssembly = Assembly.GetExecutingAssembly();
        using Stream xmlStream = currentAssembly.GetManifestResourceStream(INVALID_SAMPLE2_XML_RESOURCE_PATH);
        using Stream linkedXsdStream = currentAssembly.GetManifestResourceStream(SAMPLE_LINKED_XSD_RESOURCE_PATH);
        using Stream xsdStream = currentAssembly.GetManifestResourceStream(SAMPLE_XSD_RESOURCE_PATH);

        // Load Schema set.
        XmlSchemaSet schemaSet = XmlValidationUtilities.LoadXmlSchemaSet(xsdStream, linkedXsdStream);

        // Load XML
        XmlDocument xml = new XmlDocument();
        xml.Load(xmlStream);

        // Test XML validation.
        bool isValid = XmlValidationUtilities.ValidateXml(xml, schemaSet);

        // Check the result.
        using (Assert.EnterMultipleScope())
        {
            Assert.That(isValid, Is.False);
        }
    }

    /// <summary>
    /// Test the validate linked XML method, with missing schema.
    /// </summary>
    [Test]
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
        XmlSchemaSet schemaSet = XmlValidationUtilities.LoadXmlSchemaSet(xsdStream, linkedXsdStream);
        // Load XML
        XmlDocument xml = new XmlDocument();
        xml.Load(xmlStream);

        // Test XML validation.
        XmlValidationException error = Assert.Throws<XmlValidationException>(() => XmlValidationUtilities.ValidateXml(xml, schemaSet, true));

        // Check the result.
        using (Assert.EnterMultipleScope())
        {
            Assert.That(schemaSet, Is.Not.Null);
            Assert.That(schemaSet.Count, Is.EqualTo(2));

            Assert.That(error, Is.Not.Null);
            Assert.That(error.Message, Is.EqualTo(expectedErrorMessage));
            Assert.That(error.ValidationErrors, Is.Not.Null);
            Assert.That(error.ValidationErrors.Count, Is.EqualTo(expectedValidationErrorCount));


            XmlValidationError[] errors = error.ValidationErrors.ToArray();
            Assert.That(errors[0].Message, Is.EqualTo(expectedFirstValidationErrorMessage));
            Assert.That(errors[1].Message, Is.EqualTo(expectedSecondValidationErrorMessage));
        }
    }

    /// <summary>
    /// Test the "try" validate linked XML method, with missing schema.
    /// </summary>
    [Test]
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
        XmlSchemaSet schemaSet = XmlValidationUtilities.LoadXmlSchemaSet(xsdStream, linkedXsdStream);

        // Load XML
        XmlDocument xml = new XmlDocument();
        xml.Load(xmlStream);

        // Test XML validation.
        bool isValid = XmlValidationUtilities.TryValidateXml(xml, schemaSet, out XmlValidationException error);

        // Check the result.
        using (Assert.EnterMultipleScope())
        {
            Assert.That(isValid, Is.False);

            Assert.That(schemaSet, Is.Not.Null);
            Assert.That(schemaSet.Count, Is.EqualTo(2));

            Assert.That(error, Is.Not.Null);
            Assert.That(error.Message, Is.EqualTo(expectedErrorMessage));
            Assert.That(error.ValidationErrors, Is.Not.Null);
            Assert.That(error.ValidationErrors.Count, Is.EqualTo(expectedValidationErrorCount));


            XmlValidationError[] errors = error.ValidationErrors.ToArray();
            Assert.That(errors[0].Message, Is.EqualTo(expectedFirstValidationErrorMessage));
            Assert.That(errors[1].Message, Is.EqualTo(expectedSecondValidationErrorMessage));
        }
    }

    #endregion Validation
}
