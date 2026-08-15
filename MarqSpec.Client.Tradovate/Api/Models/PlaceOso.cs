namespace MarqSpec.Client.Tradovate.Api.Models;

/// <summary>
/// A Tradovate <c>/order/placeoso</c> request (order-sends-order brackets).
/// </summary>
public sealed record PlaceOso
{
    /// <summary>Gets the account spec (name), when targeting by name.</summary>
    public string? AccountSpec { get; init; }

    /// <summary>Gets the integer account id, when targeting by id.</summary>
    public long? AccountId { get; init; }

    /// <summary>Gets the client order id.</summary>
    public string? ClOrdId { get; init; }

    /// <summary>Gets the primary order action.</summary>
    public required OrderAction Action { get; init; }

    /// <summary>Gets the contract symbol.</summary>
    public required string Symbol { get; init; }

    /// <summary>Gets the order quantity.</summary>
    public required int OrderQty { get; init; }

    /// <summary>Gets the primary order type.</summary>
    public required OrderType OrderType { get; init; }

    /// <summary>Gets the limit price, when applicable.</summary>
    public decimal? Price { get; init; }

    /// <summary>Gets the stop price, when applicable.</summary>
    public decimal? StopPrice { get; init; }

    /// <summary>Gets the displayed quantity, when applicable.</summary>
    public int? MaxShow { get; init; }

    /// <summary>Gets the peg difference, when applicable.</summary>
    public decimal? PegDifference { get; init; }

    /// <summary>Gets the time-in-force.</summary>
    public TimeInForce? TimeInForce { get; init; }

    /// <summary>Gets the expire time for GTD, when applicable.</summary>
    public DateTimeOffset? ExpireTime { get; init; }

    /// <summary>Gets free-text attached to the order.</summary>
    public string? Text { get; init; }

    /// <summary>Gets the activation time, when applicable.</summary>
    public DateTimeOffset? ActivationTime { get; init; }

    /// <summary>Gets custom tag 50.</summary>
    public string? CustomTag50 { get; init; }

    /// <summary>Gets a value indicating whether the order is automated.</summary>
    public bool? IsAutomated { get; init; }

    /// <summary>Gets the first bracket (required).</summary>
    public required RestrainedOrderVersion Bracket1 { get; init; }

    /// <summary>Gets the second bracket, when placing a two-sided OSO.</summary>
    public RestrainedOrderVersion? Bracket2 { get; init; }
}
