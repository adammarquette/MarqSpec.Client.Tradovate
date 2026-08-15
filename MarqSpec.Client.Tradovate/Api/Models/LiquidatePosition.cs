namespace MarqSpec.Client.Tradovate.Api.Models;

/// <summary>
/// A Tradovate <c>/order/liquidateposition</c> request.
/// </summary>
public sealed record LiquidatePosition
{
    /// <summary>Gets the account id.</summary>
    public required long AccountId { get; init; }

    /// <summary>Gets the contract id.</summary>
    public required long ContractId { get; init; }

    /// <summary>Gets a value indicating whether this is an admin liquidation.</summary>
    public required bool Admin { get; init; }

    /// <summary>Gets custom tag 50.</summary>
    public string? CustomTag50 { get; init; }
}
