using Puffix.Exceptions;
using Puffix.Exceptions.Basic;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Puffix.Utilities.Exceptions
{
    /// <summary>
    /// Base class for exeption management for the class <c>XmlUtilities.</c>.
    /// </summary>
	/// <remarks>The error message patterns are stored in the file XmlUtilitiesExceptionsResources.resx. The key is the exception class name.</remarks>
    [Serializable]
#pragma warning disable S3376
    public abstract class XmlUtilitiesExceptions : BaseException
#pragma warning restore S3376
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

        #region Exception serialization.
        /// <summary>
        /// Constructor for the serialization (DO NOT MODIFY).
        /// </summary>
        /// <param name="info">Serialization information.</param>
        /// <param name="context">Context.</param>
        protected XmlUtilitiesExceptions(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
        #endregion
    }
}

namespace Puffix.Utilities.Exceptions.XmlUtilities
{
    /// <summary>
    /// Error when the XML document is not set.
    /// </summary>
    [Serializable]
    public sealed class NullXmlDocumentException : XmlUtilitiesExceptions
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public NullXmlDocumentException()
            : base(typeof(NullXmlDocumentException))
        { }

        #region Exception serialization.
        /// <summary>
        /// Constructor for the serialization (DO NOT MODIFY).
        /// </summary>
        /// <param name="info">Serialization information.</param>
        /// <param name="context">Context.</param>
        private NullXmlDocumentException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
        #endregion
    }

    /// <summary>
    /// Error when the XML data is not set.
    /// </summary>
    [Serializable]
    public sealed class NullXmlDataException : XmlUtilitiesExceptions
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public NullXmlDataException()
            : base(typeof(NullXmlDataException))
        { }

        #region Exception serialization.
        /// <summary>
        /// Constructor for the serialization (DO NOT MODIFY).
        /// </summary>
        /// <param name="info">Serialization information.</param>
        /// <param name="context">Context.</param>
        private NullXmlDataException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
        #endregion
    }

    /// <summary>
    /// Error while deserializing a XML document or data.
    /// </summary>
    [Serializable]
    public sealed class DeserializeException : XmlUtilitiesExceptions
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="innerException">Inner error.</param>
        public DeserializeException(Exception innerException)
            : base(typeof(DeserializeException), innerException)
        { }

        #region Exception serialization.
        /// <summary>
        /// Constructor for the serialization (DO NOT MODIFY).
        /// </summary>
        /// <param name="info">Serialization information.</param>
        /// <param name="context">Context.</param>
        private DeserializeException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
        #endregion
    }

    /// <summary>
    /// Error when loading a XML schema set.
    /// </summary>
    [Serializable]
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

        #region Exception serialization.
        /// <summary>
        /// Constructor for the serialization (DO NOT MODIFY).
        /// </summary>
        /// <param name="info">Serialization information.</param>
        /// <param name="context">Context.</param>
        private LoadingSchemaSetException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            ValidationErrors = info.GetValue(nameof(ValidationErrors), typeof(List<Exception>)) as IReadOnlyCollection<Exception>;
        }

        /// <summary>
        /// Serialize the class elements (technical, DO NOT MODIFY).
        /// </summary>
        /// <param name="info">Serialization information.</param>
        /// <param name="context">Streaming context.</param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));

            // Ajout des champs à sérialiser.
            info.AddValue(nameof(ValidationErrors), ValidationErrors);

            // Appel de la méthode de base.
            base.GetObjectData(info, context);
        }
        #endregion
    }

    /// <summary>
    /// Error when validating a XML document.
    /// </summary>
    [Serializable]
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

        #region Exception serialization.
        /// <summary>
        /// Constructor for the serialization (DO NOT MODIFY).
        /// </summary>
        /// <param name="info">Serialization information.</param>
        /// <param name="context">Context.</param>
        private XmlValidationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            ValidationErrors = info.GetValue(nameof(ValidationErrors), typeof(List<Exception>)) as IReadOnlyCollection<Exception>;
        }

        /// <summary>
        /// Serialize the class elements (technical, DO NOT MODIFY).
        /// </summary>
        /// <param name="info">Serialization information.</param>
        /// <param name="context">Streaming context.</param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));

            // Ajout des champs à sérialiser.
            info.AddValue(nameof(ValidationErrors), ValidationErrors);

            // Appel de la méthode de base.
            base.GetObjectData(info, context);
        }
        #endregion
    }
}
