namespace MarqSpec.Client.Tradovate.Api.Models;

/// <summary>
/// A Tradovate user.
/// </summary>
public sealed record User
{
    /// <summary>Gets the user id.</summary>
    public long? Id { get; init; }

    /// <summary>Gets the login name.</summary>
    public required string Name { get; init; }

    /// <summary>Gets the last-update timestamp (UTC).</summary>
    public required DateTimeOffset Timestamp { get; init; }

    /// <summary>Gets the email address.</summary>
    public required string Email { get; init; }

    /// <summary>Gets the user status.</summary>
    public required UserStatus Status { get; init; }

    /// <summary>Gets a value indicating whether the user is classified professional.</summary>
    public required bool Professional { get; init; }

    /// <summary>Gets the organization id, when present.</summary>
    public long? OrganizationId { get; init; }

    /// <summary>Gets the linked live user id, when present.</summary>
    public long? LinkedUserId { get; init; }
}
