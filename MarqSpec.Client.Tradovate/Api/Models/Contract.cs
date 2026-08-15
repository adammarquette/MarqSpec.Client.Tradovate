namespace MarqSpec.Client.Tradovate.Api.Models;

/// <summary>
/// A Tradovate contract (e.g. <c>ESM24</c>).
/// </summary>
public sealed record Contract
{
    /// <summary>Gets the integer contract id.</summary>
    public long? Id { get; init; }

    /// <summary>Gets the contract name / symbol.</summary>
    public required string Name { get; init; }

    /// <summary>Gets the contract maturity id.</summary>
    public required long ContractMaturityId { get; init; }
}
