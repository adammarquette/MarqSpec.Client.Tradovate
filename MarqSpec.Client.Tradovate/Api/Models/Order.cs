namespace MarqSpec.Client.Tradovate.Api.Models;

/// <summary>
/// A Tradovate order entity.
/// </summary>
public sealed record Order
{
    /// <summary>Gets the order id.</summary>
    public long? Id { get; init; }

    /// <summary>Gets the account id.</summary>
    public required long AccountId { get; init; }

    /// <summary>Gets the contract id, when present.</summary>
    public long? ContractId { get; init; }

    /// <summary>Gets the create timestamp (UTC).</summary>
    public required DateTimeOffset Timestamp { get; init; }

    /// <summary>Gets the order action.</summary>
    public required OrderAction Action { get; init; }

    /// <summary>Gets the order status.</summary>
    public required OrderStatus OrdStatus { get; init; }

    /// <summary>Gets a value indicating whether the order was placed as admin.</summary>
    public required bool Admin { get; init; }

    /// <summary>Gets the OCO group id, when present.</summary>
    public long? OcoId { get; init; }

    /// <summary>Gets the parent order id, when present.</summary>
    public long? ParentId { get; init; }

    /// <summary>Gets the linked order id, when present.</summary>
    public long? LinkedId { get; init; }
}
