namespace MarqSpec.Client.Tradovate.Api.Models;

/// <summary>
/// A Tradovate <c>placeOSO</c> result.
/// </summary>
public sealed record PlaceOsoResult
{
    /// <summary>Gets the failure reason. Anything other than <see cref="Models.FailureReason.Success"/> is a rejected place.</summary>
    public FailureReason? FailureReason { get; init; }

    /// <summary>Gets the failure text, when present.</summary>
    public string? FailureText { get; init; }

    /// <summary>Gets the primary order id, when the place succeeded.</summary>
    public long? OrderId { get; init; }

    /// <summary>Gets the first bracket order id, when present.</summary>
    public long? Oso1Id { get; init; }

    /// <summary>Gets the second bracket order id, when present.</summary>
    public long? Oso2Id { get; init; }
}
