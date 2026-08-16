namespace MarqSpec.Client.Tradovate.Exceptions;

/// <summary>
/// Exception thrown when Tradovate returns HTTP 429.
/// </summary>
public sealed class TradovateRateLimitException : TradovateApiException
{
    /// <summary>
    /// Gets the server-supplied retry delay, when present.
    /// </summary>
    public TimeSpan? RetryAfter { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TradovateRateLimitException"/> class.
    /// </summary>
    /// <param name="message">The error message. Must never include credentials or tokens.</param>
    /// <param name="retryAfter">The server-supplied retry delay, when present.</param>
    public TradovateRateLimitException(string message, TimeSpan? retryAfter = null)
        : base(message, StatusCodes.TooManyRequests)
    {
        RetryAfter = retryAfter;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TradovateRateLimitException"/> class with an inner exception.
    /// </summary>
    /// <param name="message">The error message. Must never include credentials or tokens.</param>
    /// <param name="innerException">The inner exception.</param>
    /// <param name="retryAfter">The server-supplied retry delay, when present.</param>
    public TradovateRateLimitException(string message, Exception innerException, TimeSpan? retryAfter = null)
        : base(message, StatusCodes.TooManyRequests, innerException)
    {
        RetryAfter = retryAfter;
    }

    private static class StatusCodes
    {
        public const int TooManyRequests = 429;
    }
}
