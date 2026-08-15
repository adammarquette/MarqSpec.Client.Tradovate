namespace MarqSpec.Client.Tradovate.Api.Models;

/// <summary>
/// Body for <c>POST /auth/accesstokenrequest</c>.
/// </summary>
public sealed record AccessTokenRequest
{
    /// <summary>Gets the login name.</summary>
    public required string Name { get; init; }

    /// <summary>Gets the password.</summary>
    public required string Password { get; init; }

    /// <summary>Gets the application id.</summary>
    public string? AppId { get; init; }

    /// <summary>Gets the application version.</summary>
    public string? AppVersion { get; init; }

    /// <summary>Gets the device id.</summary>
    public string? DeviceId { get; init; }

    /// <summary>Gets the OAuth client id.</summary>
    public string? Cid { get; init; }

    /// <summary>Gets the OAuth client secret.</summary>
    public string? Sec { get; init; }
}
