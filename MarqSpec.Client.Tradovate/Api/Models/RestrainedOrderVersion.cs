namespace MarqSpec.Client.Tradovate.Api.Models;

/// <summary>
/// A restrained child order used as the OCO other-side or an OSO bracket.
/// </summary>
public sealed record RestrainedOrderVersion
{
    /// <summary>Gets the child action.</summary>
    public required OrderAction Action { get; init; }

    /// <summary>Gets the client order id.</summary>
    public string? ClOrdId { get; init; }

    /// <summary>Gets the child order type.</summary>
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

    /// <summary>Gets free-text attached to the child order.</summary>
    public string? Text { get; init; }
}
