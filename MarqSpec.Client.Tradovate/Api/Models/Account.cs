namespace MarqSpec.Client.Tradovate.Api.Models;

/// <summary>
/// A Tradovate account as returned by <c>/account/list</c> and <c>/account/item</c>.
/// </summary>
public sealed record Account
{
    /// <summary>Gets the integer account id.</summary>
    public long? Id { get; init; }

    /// <summary>Gets the account name (spec).</summary>
    public required string Name { get; init; }

    /// <summary>Gets the owning user id.</summary>
    public required long UserId { get; init; }

    /// <summary>Gets the account type.</summary>
    public required AccountType AccountType { get; init; }

    /// <summary>Gets a value indicating whether the account is active.</summary>
    public required bool Active { get; init; }

    /// <summary>Gets the clearing house id.</summary>
    public required long ClearingHouseId { get; init; }

    /// <summary>Gets the risk category id.</summary>
    public required long RiskCategoryId { get; init; }

    /// <summary>Gets the auto-liquidation profile id.</summary>
    public required long AutoLiqProfileId { get; init; }

    /// <summary>Gets the margin account type.</summary>
    public required MarginAccountType MarginAccountType { get; init; }

    /// <summary>Gets the legal status.</summary>
    public required LegalStatus LegalStatus { get; init; }

    /// <summary>Gets a value indicating whether the account is read-only.</summary>
    public bool? Readonly { get; init; }
}
