using MarqSpec.Client.Tradovate.Api.Models;

namespace MarqSpec.Client.Tradovate;

/// <summary>
/// Tradovate-native REST client. Auth is internal; consumers just call the methods.
/// </summary>
/// <remarks>
/// This interface speaks Tradovate's vocabulary. It does not implement or mimic
/// <c>IProjectXApiClient</c>. Historical bars are on <see cref="WebSocket.ITradovateWebSocketClient"/>
/// because Tradovate has no REST bar endpoint.
/// </remarks>
public interface ITradovateApiClient
{
    /// <summary>
    /// Gets the configured REST base URL so a consumer can derive practice-versus-live.
    /// </summary>
    string ConfiguredHost { get; }

    /// <summary>
    /// Lists accounts for the authenticated user.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The accounts Tradovate returns.</returns>
    Task<IReadOnlyList<Account>> GetAccountsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets one account by integer id.
    /// </summary>
    /// <param name="accountId">The account id.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The account, or <see langword="null"/> if not found.</returns>
    Task<Account?> GetAccountAsync(long accountId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists cash balances for the authenticated user.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The cash balances Tradovate returns.</returns>
    Task<IReadOnlyList<CashBalance>> GetCashBalancesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists users visible to the authenticated login.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The users Tradovate returns.</returns>
    Task<IReadOnlyList<User>> GetUsersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets one user by integer id.
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The user, or <see langword="null"/> if not found.</returns>
    Task<User?> GetUserAsync(long userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds a contract by symbol (e.g. <c>ESM24</c>).
    /// </summary>
    /// <param name="name">The contract name.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The contract, or <see langword="null"/> if not found.</returns>
    Task<Contract?> FindContractAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a contract by integer id.
    /// </summary>
    /// <param name="contractId">The contract id.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The contract, or <see langword="null"/> if not found.</returns>
    Task<Contract?> GetContractAsync(long contractId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Suggests contracts matching a partial name.
    /// </summary>
    /// <param name="text">The suggestion text.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>Matching contracts.</returns>
    Task<IReadOnlyList<Contract>> SuggestContractsAsync(string text, CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds a product by name (e.g. <c>ES</c>).
    /// </summary>
    /// <param name="name">The product name.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The product, or <see langword="null"/> if not found.</returns>
    Task<Product?> FindProductAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a product by integer id.
    /// </summary>
    /// <param name="productId">The product id.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The product, or <see langword="null"/> if not found.</returns>
    Task<Product?> GetProductAsync(long productId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a contract maturity by integer id.
    /// </summary>
    /// <param name="maturityId">The maturity id.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The maturity, or <see langword="null"/> if not found.</returns>
    Task<ContractMaturity?> GetContractMaturityAsync(long maturityId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Places an order. Never retried. Throws when <c>failureReason</c> is not Success.
    /// </summary>
    /// <param name="request">The place-order request.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The place result.</returns>
    Task<PlaceOrderResult> PlaceOrderAsync(PlaceOrder request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Places an OSO (order-sends-order) bracket. Never retried. Throws when <c>failureReason</c> is not Success.
    /// </summary>
    /// <param name="request">The place-OSO request.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The place result.</returns>
    Task<PlaceOsoResult> PlaceOsoAsync(PlaceOso request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Places an OCO (one-cancels-other). Never retried. Throws when <c>failureReason</c> is not Success.
    /// </summary>
    /// <param name="request">The place-OCO request.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The place result.</returns>
    Task<PlaceOcoResult> PlaceOcoAsync(PlaceOco request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Modifies an existing order. Retried on 429 / 5xx / transport only. Throws when <c>failureReason</c> is not Success or when <c>commandId</c> is absent.
    /// </summary>
    /// <param name="request">The modify request.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The command result.</returns>
    Task<CommandResult> ModifyOrderAsync(ModifyOrder request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cancels an existing order. Retried on 429 / 5xx / transport only. Throws when <c>failureReason</c> is not Success or when <c>commandId</c> is absent.
    /// </summary>
    /// <param name="request">The cancel request.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The command result.</returns>
    Task<CommandResult> CancelOrderAsync(CancelOrder request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists orders for the authenticated user.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The orders Tradovate returns.</returns>
    Task<IReadOnlyList<Order>> GetOrdersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets one order by integer id.
    /// </summary>
    /// <param name="orderId">The order id.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The order, or <see langword="null"/> if not found.</returns>
    Task<Order?> GetOrderAsync(long orderId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists positions for the authenticated user. <see cref="Position.NetPos"/> keeps its sign.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The positions Tradovate returns.</returns>
    Task<IReadOnlyList<Position>> GetPositionsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Liquidates a position. Never retried. Throws when <c>failureReason</c> is not Success or when <c>commandId</c> is absent.
    /// </summary>
    /// <param name="request">The liquidate request.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The command result.</returns>
    Task<CommandResult> LiquidatePositionAsync(LiquidatePosition request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists fills for the authenticated user.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The fills Tradovate returns.</returns>
    Task<IReadOnlyList<Fill>> GetFillsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks whether the API is responsive by calling <c>/auth/me</c>.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns><see langword="true"/> when <c>/auth/me</c> succeeds.</returns>
    Task<bool> PingAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the authenticated user from <c>/auth/me</c>.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The current-user payload.</returns>
    Task<AuthMe> GetAuthMeAsync(CancellationToken cancellationToken = default);
}
