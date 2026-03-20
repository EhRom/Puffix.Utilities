using System;

namespace Puffix.Utilities.Exceptions;

public class XmlValidationError(string message, string errorType, string stackTrace, string source, string helpLink, XmlValidationError innerError = null)
{
    public string Message { get; init; } = message;

    public string ErrorType { get; init; } = errorType;

    public string StackTrace { get; init; } = stackTrace;

    public string Source { get; init; } = source;

    public string HelpLink { get; init; } = helpLink;

    public XmlValidationError InnerError { get; init; } = innerError;

    public static XmlValidationError CreateNew(Exception error)
    {
        XmlValidationError innerError = error.InnerException is not null ? CreateNew(error.InnerException) : null;

        return new XmlValidationError(
                error.Message,
                error.GetType().FullName ?? "System.Exception",
                error.StackTrace,
                error.Source,
                error.HelpLink,
                innerError
            );
    }
}
