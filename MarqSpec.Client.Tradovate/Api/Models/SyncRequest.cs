namespace MarqSpec.Client.Tradovate.Api.Models;

/// <summary>
/// Body for trading-socket <c>user/syncrequest</c>.
/// </summary>
public sealed record SyncRequest
{
    /// <summary>Gets the user ids to sync. Typically the authenticated user.</summary>
    public required IReadOnlyList<long> Users { get; init; }
}
