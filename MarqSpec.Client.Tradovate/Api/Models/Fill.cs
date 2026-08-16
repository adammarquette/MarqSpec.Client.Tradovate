namespace MarqSpec.Client.Tradovate.Api.Models;

/// <summary>
/// A Tradovate fill (execution).
/// </summary>
public sealed record Fill
{
    /// <summary>Gets the fill id.</summary>
    public long? Id { get; init; }

    /// <summary>Gets the order id.</summary>
    public required long OrderId { get; init; }

    /// <summary>Gets the contract id.</summary>
    public required long ContractId { get; init; }

    /// <summary>Gets the fill timestamp (UTC).</summary>
    public required DateTimeOffset Timestamp { get; init; }

    /// <summary>Gets the trade date.</summary>
    public required TradeDate TradeDate { get; init; }

    /// <summary>Gets the fill action.</summary>
    public required OrderAction Action { get; init; }

    /// <summary>Gets the fill quantity.</summary>
    public required int Qty { get; init; }

    /// <summary>Gets the fill price.</summary>
    public required decimal Price { get; init; }

    /// <summary>Gets a value indicating whether the fill is active.</summary>
    public required bool Active { get; init; }

    /// <summary>Gets the finally-paired quantity.</summary>
    public required int FinallyPaired { get; init; }
}
