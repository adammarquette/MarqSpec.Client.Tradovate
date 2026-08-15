namespace MarqSpec.Client.Tradovate.Api.Models;

/// <summary>
/// Response from <c>/auth/accesstokenrequest</c> and <c>/auth/renewaccesstoken</c>.
/// </summary>
public sealed record AccessTokenResponse
{
    /// <summary>Gets non-empty error text when the request failed.</summary>
    public string? ErrorText { get; init; }

    /// <summary>Gets the trading/account access token.</summary>
    public string? AccessToken { get; init; }

    /// <summary>
    /// Gets the market-data access token when the server supplies one.
    /// Absent on some account types; callers may fall back to <see cref="AccessToken"/>.
    /// </summary>
    public string? MdAccessToken { get; init; }

    /// <summary>Gets the token expiration time (UTC).</summary>
    public DateTimeOffset? ExpirationTime { get; init; }

    /// <summary>Gets the password expiration time, when present.</summary>
    public DateTimeOffset? PasswordExpirationTime { get; init; }

    /// <summary>Gets the user status, when present.</summary>
    public UserStatus? UserStatus { get; init; }

    /// <summary>Gets the authenticated user id.</summary>
    public long? UserId { get; init; }

    /// <summary>Gets the authenticated user name.</summary>
    public string? Name { get; init; }

    /// <summary>Gets a value indicating whether the user has a live entitlement.</summary>
    public bool? HasLive { get; init; }
}
