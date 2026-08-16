namespace MarqSpec.Client.Tradovate.Api.Models;

/// <summary>
/// A request for historical or streaming bars on the market-data socket.
/// </summary>
/// <remarks>
/// Tradovate has no REST bar endpoint. History is retrieved via <c>md/getChart</c>.
/// </remarks>
public sealed record ChartRequest
{
    /// <summary>Gets the contract symbol (e.g. <c>ESM24</c>), when resolving by name.</summary>
    public string? Symbol { get; init; }

    /// <summary>Gets the numeric contract id, when resolving by id.</summary>
    public long? ContractId { get; init; }

    /// <summary>Gets the bar underlying type.</summary>
    public required ChartUnderlyingType UnderlyingType { get; init; }

    /// <summary>Gets the element size (e.g. 1 or 5 for minute bars).</summary>
    public required int ElementSize { get; init; }

    /// <summary>Gets the closest (newest) timestamp of the range, when bounding by time.</summary>
    public DateTimeOffset? ClosestTimestamp { get; init; }

    /// <summary>Gets the farthest (oldest) timestamp of the range, when bounding by time.</summary>
    public DateTimeOffset? AsFarAsTimestamp { get; init; }

    /// <summary>Gets the maximum number of bars to return, when bounding by count.</summary>
    public int? AsMuchAsElements { get; init; }
}
