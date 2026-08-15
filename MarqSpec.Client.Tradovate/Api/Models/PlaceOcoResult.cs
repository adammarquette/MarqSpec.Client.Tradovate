namespace MarqSpec.Client.Tradovate.Api.Models;

/// <summary>
/// A Tradovate <c>placeOCO</c> result.
/// </summary>
public sealed record PlaceOcoResult
{
    /// <summary>Gets the failure reason. Anything other than <see cref="Models.FailureReason.Success"/> is a rejected place.</summary>
    public FailureReason? FailureReason { get; init; }

    /// <summary>Gets the failure text, when present.</summary>
    public string? FailureText { get; init; }

    /// <summary>Gets the first-leg order id, when the place succeeded.</summary>
    public long? OrderId { get; init; }

    /// <summary>Gets the OCO group id, when present.</summary>
    public long? OcoId { get; init; }
}
