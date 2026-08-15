namespace MarqSpec.Client.Tradovate.Api.Models;

/// <summary>
/// A Tradovate market-data quote.
/// </summary>
/// <remarks>
/// Bid/ask sizes are nullable. Absent size is not zero — leave it null.
/// reference: documentation/INDEX.md (Absent ≠ zero quotes)
/// </remarks>
public sealed record Quote
{
    /// <summary>Gets the contract symbol or id the quote belongs to.</summary>
    public string? Contract { get; init; }

    /// <summary>Gets the numeric contract id, when present.</summary>
    public long? ContractId { get; init; }

    /// <summary>Gets the timestamp (UTC), when present.</summary>
    public DateTimeOffset? Timestamp { get; init; }

    /// <summary>Gets the bid price, when present.</summary>
    public decimal? BidPrice { get; init; }

    /// <summary>Gets the bid size. Null when the socket omitted it — not zero.</summary>
    public int? BidSize { get; init; }

    /// <summary>Gets the ask price, when present.</summary>
    public decimal? AskPrice { get; init; }

    /// <summary>Gets the ask size. Null when the socket omitted it — not zero.</summary>
    public int? AskSize { get; init; }

    /// <summary>Gets the last trade price, when present.</summary>
    public decimal? LastPrice { get; init; }

    /// <summary>Gets the last trade size, when present.</summary>
    public int? LastSize { get; init; }
}
