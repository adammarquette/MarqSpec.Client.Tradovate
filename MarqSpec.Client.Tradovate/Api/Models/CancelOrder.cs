namespace MarqSpec.Client.Tradovate.Api.Models;

/// <summary>
/// A Tradovate <c>/order/cancelorder</c> request.
/// </summary>
public sealed record CancelOrder
{
    /// <summary>Gets the order id to cancel.</summary>
    public required long OrderId { get; init; }

    /// <summary>Gets the client order id.</summary>
    public string? ClOrdId { get; init; }

    /// <summary>Gets the activation time, when applicable.</summary>
    public DateTimeOffset? ActivationTime { get; init; }

    /// <summary>Gets custom tag 50.</summary>
    public string? CustomTag50 { get; init; }

    /// <summary>Gets a value indicating whether the cancel is automated.</summary>
    public bool? IsAutomated { get; init; }
}
