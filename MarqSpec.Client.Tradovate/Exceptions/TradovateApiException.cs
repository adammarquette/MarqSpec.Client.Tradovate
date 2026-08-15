using MarqSpec.Client.Tradovate.Api.Models;

namespace MarqSpec.Client.Tradovate.Exceptions;

/// <summary>
/// Exception thrown when a Tradovate API request fails.
/// </summary>
public class TradovateApiException : Exception
{
    /// <summary>
    /// Gets the HTTP status code of the failed request, when one was received.
    /// </summary>
    public int? StatusCode { get; }

    /// <summary>
    /// Gets the Tradovate <c>failureReason</c> when the server reported failure inside a 200 payload.
    /// </summary>
    public FailureReason? FailureReason { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TradovateApiException"/> class.
    /// </summary>
    public TradovateApiException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TradovateApiException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message. Must never include credentials or tokens.</param>
    public TradovateApiException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TradovateApiException"/> class with a message and status code.
    /// </summary>
    /// <param name="message">The error message. Must never include credentials or tokens.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    public TradovateApiException(string message, int statusCode)
        : base(message)
    {
        StatusCode = statusCode;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TradovateApiException"/> class with a message, status code, and failure reason.
    /// </summary>
    /// <param name="message">The error message. Must never include credentials or tokens.</param>
    /// <param name="statusCode">The HTTP status code, when known.</param>
    /// <param name="failureReason">The Tradovate <c>failureReason</c>.</param>
    public TradovateApiException(string message, int? statusCode, FailureReason? failureReason)
        : base(message)
    {
        StatusCode = statusCode;
        FailureReason = failureReason;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TradovateApiException"/> class with a message and inner exception.
    /// </summary>
    /// <param name="message">The error message. Must never include credentials or tokens.</param>
    /// <param name="innerException">The inner exception.</param>
    public TradovateApiException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TradovateApiException"/> class with a message, status code, and inner exception.
    /// </summary>
    /// <param name="message">The error message. Must never include credentials or tokens.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="innerException">The inner exception.</param>
    public TradovateApiException(string message, int statusCode, Exception innerException)
        : base(message, innerException)
    {
        StatusCode = statusCode;
    }
}
