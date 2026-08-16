using MarqSpec.Client.Tradovate.Api.Models;
using Refit;

namespace MarqSpec.Client.Tradovate.Api.Rest;

/// <summary>
/// Tradovate REST surface named from swagger/services.swagger.yaml.
/// </summary>
internal interface ITradovateRestApi
{
    [Get("/account/list")]
    Task<IReadOnlyList<Account>> ListAccountsAsync(CancellationToken cancellationToken = default);

    [Get("/account/item")]
    Task<Account> GetAccountAsync([Query] long id, CancellationToken cancellationToken = default);

    [Get("/cashBalance/list")]
    Task<IReadOnlyList<CashBalance>> ListCashBalancesAsync(CancellationToken cancellationToken = default);

    [Get("/user/list")]
    Task<IReadOnlyList<User>> ListUsersAsync(CancellationToken cancellationToken = default);

    [Get("/user/item")]
    Task<User> GetUserAsync([Query] long id, CancellationToken cancellationToken = default);

    [Get("/contract/find")]
    Task<Contract> FindContractAsync([Query] string name, CancellationToken cancellationToken = default);

    [Get("/contract/item")]
    Task<Contract> GetContractAsync([Query] long id, CancellationToken cancellationToken = default);

    [Get("/contract/suggest")]
    Task<IReadOnlyList<Contract>> SuggestContractsAsync([Query(Format = "t")] string t, CancellationToken cancellationToken = default);

    [Get("/product/find")]
    Task<Product> FindProductAsync([Query] string name, CancellationToken cancellationToken = default);

    [Get("/product/item")]
    Task<Product> GetProductAsync([Query] long id, CancellationToken cancellationToken = default);

    [Get("/contractMaturity/item")]
    Task<ContractMaturity> GetContractMaturityAsync([Query] long id, CancellationToken cancellationToken = default);

    [Post("/order/placeorder")]
    Task<PlaceOrderResult> PlaceOrderAsync([Body] PlaceOrder request, CancellationToken cancellationToken = default);

    [Post("/order/placeoso")]
    Task<PlaceOsoResult> PlaceOsoAsync([Body] PlaceOso request, CancellationToken cancellationToken = default);

    [Post("/order/placeoco")]
    Task<PlaceOcoResult> PlaceOcoAsync([Body] PlaceOco request, CancellationToken cancellationToken = default);

    [Post("/order/modifyorder")]
    Task<CommandResult> ModifyOrderAsync([Body] ModifyOrder request, CancellationToken cancellationToken = default);

    [Post("/order/cancelorder")]
    Task<CommandResult> CancelOrderAsync([Body] CancelOrder request, CancellationToken cancellationToken = default);

    [Post("/order/liquidateposition")]
    Task<CommandResult> LiquidatePositionAsync([Body] LiquidatePosition request, CancellationToken cancellationToken = default);

    [Get("/order/list")]
    Task<IReadOnlyList<Order>> ListOrdersAsync(CancellationToken cancellationToken = default);

    [Get("/order/item")]
    Task<Order> GetOrderAsync([Query] long id, CancellationToken cancellationToken = default);

    [Get("/position/list")]
    Task<IReadOnlyList<Position>> ListPositionsAsync(CancellationToken cancellationToken = default);

    [Get("/fill/list")]
    Task<IReadOnlyList<Fill>> ListFillsAsync(CancellationToken cancellationToken = default);

    [Get("/auth/me")]
    Task<AuthMe> MeAsync(CancellationToken cancellationToken = default);
}
