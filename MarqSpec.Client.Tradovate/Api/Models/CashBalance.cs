namespace MarqSpec.Client.Tradovate.Api.Models;

/// <summary>
/// A Tradovate cash balance snapshot.
/// </summary>
public sealed record CashBalance
{
    /// <summary>Gets the cash-balance id.</summary>
    public long? Id { get; init; }

    /// <summary>Gets the account id.</summary>
    public required long AccountId { get; init; }

    /// <summary>Gets the snapshot timestamp (UTC).</summary>
    public required DateTimeOffset Timestamp { get; init; }

    /// <summary>Gets the trade date.</summary>
    public required TradeDate TradeDate { get; init; }

    /// <summary>Gets the currency id.</summary>
    public required long CurrencyId { get; init; }

    /// <summary>Gets the cash amount.</summary>
    public required decimal Amount { get; init; }

    /// <summary>Gets realized P&amp;L, when present.</summary>
    public decimal? RealizedPnL { get; init; }

    /// <summary>Gets week realized P&amp;L, when present.</summary>
    public decimal? WeekRealizedPnL { get; init; }
}
