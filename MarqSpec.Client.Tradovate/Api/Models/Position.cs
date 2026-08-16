namespace MarqSpec.Client.Tradovate.Api.Models;

/// <summary>
/// A Tradovate position.
/// </summary>
/// <remarks>
/// <see cref="NetPos"/> is signed — do not drop the sign. Positive is long, negative is short.
/// </remarks>
public sealed record Position
{
    /// <summary>Gets the position id.</summary>
    public long? Id { get; init; }

    /// <summary>Gets the account id.</summary>
    public required long AccountId { get; init; }

    /// <summary>Gets the contract id.</summary>
    public required long ContractId { get; init; }

    /// <summary>Gets the last-update timestamp (UTC).</summary>
    public required DateTimeOffset Timestamp { get; init; }

    /// <summary>Gets the trade date.</summary>
    public required TradeDate TradeDate { get; init; }

    /// <summary>Gets the signed net quantity. Positive is long; negative is short.</summary>
    public required int NetPos { get; init; }

    /// <summary>Gets the net price, when present.</summary>
    public decimal? NetPrice { get; init; }

    /// <summary>Gets contracts bought.</summary>
    public required int Bought { get; init; }

    /// <summary>Gets the bought value.</summary>
    public required decimal BoughtValue { get; init; }

    /// <summary>Gets contracts sold.</summary>
    public required int Sold { get; init; }

    /// <summary>Gets the sold value.</summary>
    public required decimal SoldValue { get; init; }

    /// <summary>Gets the previous session net position (signed).</summary>
    public required int PrevPos { get; init; }

    /// <summary>Gets the previous session net price, when present.</summary>
    public decimal? PrevPrice { get; init; }
}
