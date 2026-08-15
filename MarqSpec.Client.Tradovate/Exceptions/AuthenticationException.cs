namespace MarqSpec.Client.Tradovate.Exceptions;

/// <summary>
/// Exception thrown when Tradovate authentication fails.
/// </summary>
/// <remarks>
/// Messages must never include credentials, passwords, or tokens.
/// </remarks>
public sealed class AuthenticationException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AuthenticationException"/> class.
    /// </summary>
    public AuthenticationException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthenticationException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message. Must never include credentials or tokens.</param>
    public AuthenticationException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthenticationException"/> class with a message and inner exception.
    /// </summary>
    /// <param name="message">The error message. Must never include credentials or tokens.</param>
    /// <param name="innerException">The inner exception.</param>
    public AuthenticationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
