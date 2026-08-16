namespace MarqSpec.Client.Tradovate.Api.Models;

/// <summary>
/// A Tradovate <c>/order/modifyorder</c> request.
/// </summary>
public sealed record ModifyOrder
{
    /// <summary>Gets the order id to modify.</summary>
    public required long OrderId { get; init; }

    /// <summary>Gets the client order id.</summary>
    public string? ClOrdId { get; init; }

    /// <summary>Gets the new quantity.</summary>
    public required int OrderQty { get; init; }

    /// <summary>Gets the new order type.</summary>
    public required OrderType OrderType { get; init; }

    /// <summary>Gets the new limit price, when applicable.</summary>
    public decimal? Price { get; init; }

    /// <summary>Gets the new stop price, when applicable.</summary>
    public decimal? StopPrice { get; init; }

    /// <summary>Gets the displayed quantity, when applicable.</summary>
    public int? MaxShow { get; init; }

    /// <summary>Gets the peg difference, when applicable.</summary>
    public decimal? PegDifference { get; init; }

    /// <summary>Gets the time-in-force.</summary>
    public TimeInForce? TimeInForce { get; init; }

    /// <summary>Gets the expire time for GTD, when applicable.</summary>
    public DateTimeOffset? ExpireTime { get; init; }

    /// <summary>Gets free-text attached to the modify.</summary>
    public string? Text { get; init; }

    /// <summary>Gets the activation time, when applicable.</summary>
    public DateTimeOffset? ActivationTime { get; init; }

    /// <summary>Gets custom tag 50.</summary>
    public string? CustomTag50 { get; init; }

    /// <summary>Gets a value indicating whether the modify is automated.</summary>
    public bool? IsAutomated { get; init; }
}
