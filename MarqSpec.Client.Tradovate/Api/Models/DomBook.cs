namespace MarqSpec.Client.Tradovate.Api.Models;

/// <summary>
/// A Tradovate depth-of-market book snapshot or update.
/// </summary>
public sealed record DomBook
{
    /// <summary>Gets the contract symbol or id.</summary>
    public string? Contract { get; init; }

    /// <summary>Gets the numeric contract id, when present.</summary>
    public long? ContractId { get; init; }

    /// <summary>Gets the timestamp (UTC), when present.</summary>
    public DateTimeOffset? Timestamp { get; init; }

    /// <summary>Gets bid levels, best bid first when the server orders them that way.</summary>
    public IReadOnlyList<DomLevel> Bids { get; init; } = [];

    /// <summary>Gets ask levels, best ask first when the server orders them that way.</summary>
    public IReadOnlyList<DomLevel> Asks { get; init; } = [];
}
