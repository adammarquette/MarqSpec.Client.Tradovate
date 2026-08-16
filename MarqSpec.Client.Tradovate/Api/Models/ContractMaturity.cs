namespace MarqSpec.Client.Tradovate.Api.Models;

/// <summary>
/// A Tradovate contract maturity.
/// </summary>
public sealed record ContractMaturity
{
    /// <summary>Gets the maturity id.</summary>
    public long? Id { get; init; }

    /// <summary>Gets the product id.</summary>
    public required long ProductId { get; init; }

    /// <summary>Gets the expiration month (Tradovate packed month code).</summary>
    public required int ExpirationMonth { get; init; }

    /// <summary>Gets the expiration date (UTC).</summary>
    public required DateTimeOffset ExpirationDate { get; init; }

    /// <summary>Gets a value indicating whether this is the front month.</summary>
    public required bool IsFront { get; init; }

    /// <summary>Gets the first intent date, when present.</summary>
    public DateTimeOffset? FirstIntentDate { get; init; }

    /// <summary>Gets the underlying id, when present.</summary>
    public long? UnderlyingId { get; init; }
}
