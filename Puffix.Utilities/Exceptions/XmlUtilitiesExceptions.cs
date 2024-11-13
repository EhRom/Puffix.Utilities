using Puffix.Exceptions;
using Puffix.Exceptions.Basic;
using System;
using System.Collections.Generic;

namespace Puffix.Utilities.Exceptions
{
    /// <summary>
    /// Base class for exeption management for the class <c>XmlUtilities.</c>.
    /// </summary>
	/// <remarks>The error message patterns are stored in the file XmlUtilitiesExceptionsResources.resx. The key is the exception class name.</remarks>
    public abstract class XmlUtilitiesExceptions : BaseException
    {
        /// <summary>
        /// Logger.
        /// </summary>
        private static readonly ILog log = EmptyLog.GetLogger(typeof(XmlUtilitiesExceptions));

        /// <summary>
        /// Constructor for basic exceptions.
        /// </summary>
        /// <param name="exceptionType">Child exception type.</param>
		/// <param name="messageParams">Error message parameters.</param>
        protected XmlUtilitiesExceptions(Type exceptionType, params object[] messageParams)
            : base(typeof(XmlUtilitiesExceptionsResources), exceptionType, log, messageParams)
        { }

        /// <summary>
        /// Constructor for encapsulated exceptions.
        /// </summary>
        /// <param name="exceptionType">Child exception type.</param>
		/// <param name="innerException">Inner exception.</param>
		/// <param name="messageParams">Error message parameters.</param>
        protected XmlUtilitiesExceptions(Type exceptionType, Exception innerException, params object[] messageParams)
            : base(typeof(XmlUtilitiesExceptionsResources), exceptionType, log, innerException, messageParams)
        { }
    }
}

namespace Puffix.Utilities.Exceptions.XmlUtilities
{
    /// <summary>
    /// Error when the XML document is not set.
    /// </summary>
    public sealed class NullXmlDocumentException : XmlUtilitiesExceptions
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public NullXmlDocumentException()
            : base(typeof(NullXmlDocumentException))
        { }
    }

    /// <summary>
    /// Error when the XML data is not set.
    /// </summary>
    public sealed class NullXmlDataException : XmlUtilitiesExceptions
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public NullXmlDataException()
            : base(typeof(NullXmlDataException))
        { }
    }

    /// <summary>
    /// Error while deserializing a XML document or data.
    /// </summary>
    public sealed class DeserializeException : XmlUtilitiesExceptions
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="innerException">Inner error.</param>
        public DeserializeException(Exception innerException)
            : base(typeof(DeserializeException), innerException)
        { }
    }

    /// <summary>
    /// Error when the object to serialize is not set.
    /// </summary>
    public sealed class NullObjectToSerializeException : XmlUtilitiesExceptions
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public NullObjectToSerializeException()
            : base(typeof(NullObjectToSerializeException))
        { }
    }

    /// <summary>
    /// Error while serializing an object.
    /// </summary>
    public sealed class SerializeException : XmlUtilitiesExceptions
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="innerException">Inner error.</param>
        public SerializeException(Exception innerException)
            : base(typeof(SerializeException), innerException)
        { }
    }

    /// <summary>
    /// Error when loading a XML schema set.
    /// </summary>
    public sealed class LoadingSchemaSetException : XmlUtilitiesExceptions
    {
        /// <summary>
        /// Load and validation errors.
        /// </summary>
        public IReadOnlyCollection<Exception> ValidationErrors { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public LoadingSchemaSetException(IReadOnlyCollection<Exception> validationErrors)
            : base(typeof(LoadingSchemaSetException), validationErrors?.Count)
        {
            ValidationErrors = validationErrors;
        }
    }

    /// <summary>
    /// Error when validating a XML document.
    /// </summary>
    public sealed class XmlValidationException : XmlUtilitiesExceptions
    {
        /// <summary>
        /// Load and validation errors.
        /// </summary>
        public IReadOnlyCollection<Exception> ValidationErrors { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public XmlValidationException(IReadOnlyCollection<Exception> validationErrors)
            : base(typeof(XmlValidationException), validationErrors?.Count)
        {
            ValidationErrors = validationErrors;
        }
    }

    /// <summary>
    /// Error while comparing two XML documents.
    /// </summary>
    public sealed class CompareException : XmlUtilitiesExceptions
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="innerException">Inner error.</param>
        public CompareException(Exception innerException)
            : base(typeof(CompareException), innerException)
        { }
    }
}
