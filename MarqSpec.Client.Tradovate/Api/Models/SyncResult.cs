namespace MarqSpec.Client.Tradovate.Api.Models;

/// <summary>
/// Snapshot returned by trading-socket <c>user/syncrequest</c>.
/// </summary>
public sealed record SyncResult
{
    /// <summary>Gets accounts in the snapshot.</summary>
    public IReadOnlyList<Account> Accounts { get; init; } = [];

    /// <summary>Gets users in the snapshot.</summary>
    public IReadOnlyList<User> Users { get; init; } = [];

    /// <summary>Gets cash balances in the snapshot.</summary>
    public IReadOnlyList<CashBalance> CashBalances { get; init; } = [];

    /// <summary>Gets contracts in the snapshot.</summary>
    public IReadOnlyList<Contract> Contracts { get; init; } = [];

    /// <summary>Gets products in the snapshot.</summary>
    public IReadOnlyList<Product> Products { get; init; } = [];

    /// <summary>Gets positions in the snapshot.</summary>
    public IReadOnlyList<Position> Positions { get; init; } = [];

    /// <summary>Gets orders in the snapshot.</summary>
    public IReadOnlyList<Order> Orders { get; init; } = [];

    /// <summary>Gets fills in the snapshot.</summary>
    public IReadOnlyList<Fill> Fills { get; init; } = [];
}
