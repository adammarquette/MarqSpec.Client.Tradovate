namespace MarqSpec.Client.Tradovate.Api.Models;

/// <summary>
/// A Tradovate <c>placeOrder</c> result.
/// </summary>
public sealed record PlaceOrderResult
{
    /// <summary>Gets the failure reason. Anything other than <see cref="Models.FailureReason.Success"/> is a rejected place.</summary>
    public FailureReason? FailureReason { get; init; }

    /// <summary>Gets the failure text, when present.</summary>
    public string? FailureText { get; init; }

    /// <summary>Gets the new order id, when the place succeeded.</summary>
    public long? OrderId { get; init; }
}
