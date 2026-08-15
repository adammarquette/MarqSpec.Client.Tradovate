namespace MarqSpec.Client.Tradovate.Api.Models;

/// <summary>
/// Response from <c>GET /auth/me</c>.
/// </summary>
public sealed record AuthMe
{
    /// <summary>Gets non-empty error text when the request failed.</summary>
    public string? ErrorText { get; init; }

    /// <summary>Gets the user id.</summary>
    public long? UserId { get; init; }

    /// <summary>Gets the login name.</summary>
    public string? Name { get; init; }

    /// <summary>Gets the full name.</summary>
    public string? FullName { get; init; }

    /// <summary>Gets the email address.</summary>
    public string? Email { get; init; }

    /// <summary>Gets a value indicating whether the email is verified.</summary>
    public bool? EmailVerified { get; init; }

    /// <summary>Gets a value indicating whether the account is a trial.</summary>
    public bool? IsTrial { get; init; }
}
