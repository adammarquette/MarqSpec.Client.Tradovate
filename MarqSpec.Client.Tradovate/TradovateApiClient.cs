using MarqSpec.Client.Tradovate.Api.Models;
using MarqSpec.Client.Tradovate.Api.Rest;
using MarqSpec.Client.Tradovate.Configuration;
using MarqSpec.Client.Tradovate.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Refit;

namespace MarqSpec.Client.Tradovate;

/// <summary>
/// Default <see cref="ITradovateApiClient"/> implementation over Refit.
/// </summary>
public sealed class TradovateApiClient : ITradovateApiClient
{
    private readonly ITradovateRestApi _restApi;
    private readonly ILogger<TradovateApiClient> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="TradovateApiClient"/> class.
    /// </summary>
    /// <param name="restApi">The Refit REST API.</param>
    /// <param name="options">The Tradovate options. Host is required.</param>
    /// <param name="logger">The logger. Must never receive credentials or tokens.</param>
    internal TradovateApiClient(
        ITradovateRestApi restApi,
        IOptions<TradovateOptions> options,
        ILogger<TradovateApiClient> logger)
    {
        _restApi = restApi;
        _logger = logger;
        TradovateOptions value = options.Value;
        value.Validate();
        ConfiguredHost = value.RestBaseUrl;
    }

    /// <inheritdoc />
    public string ConfiguredHost { get; }

    /// <inheritdoc />
    public Task<IReadOnlyList<Account>> GetAccountsAsync(CancellationToken cancellationToken = default)
    {
        return ExecuteAsync(() => _restApi.ListAccountsAsync(cancellationToken), "account/list");
    }

    /// <inheritdoc />
    public Task<Account?> GetAccountAsync(long accountId, CancellationToken cancellationToken = default)
    {
        return ExecuteOptionalAsync(() => _restApi.GetAccountAsync(accountId, cancellationToken), "account/item");
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<CashBalance>> GetCashBalancesAsync(CancellationToken cancellationToken = default)
    {
        return ExecuteAsync(() => _restApi.ListCashBalancesAsync(cancellationToken), "cashBalance/list");
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<User>> GetUsersAsync(CancellationToken cancellationToken = default)
    {
        return ExecuteAsync(() => _restApi.ListUsersAsync(cancellationToken), "user/list");
    }

    /// <inheritdoc />
    public Task<User?> GetUserAsync(long userId, CancellationToken cancellationToken = default)
    {
        return ExecuteOptionalAsync(() => _restApi.GetUserAsync(userId, cancellationToken), "user/item");
    }

    /// <inheritdoc />
    public Task<Contract?> FindContractAsync(string name, CancellationToken cancellationToken = default)
    {
        return ExecuteOptionalAsync(() => _restApi.FindContractAsync(name, cancellationToken), "contract/find");
    }

    /// <inheritdoc />
    public Task<Contract?> GetContractAsync(long contractId, CancellationToken cancellationToken = default)
    {
        return ExecuteOptionalAsync(() => _restApi.GetContractAsync(contractId, cancellationToken), "contract/item");
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<Contract>> SuggestContractsAsync(string text, CancellationToken cancellationToken = default)
    {
        return ExecuteAsync(() => _restApi.SuggestContractsAsync(text, cancellationToken), "contract/suggest");
    }

    /// <inheritdoc />
    public Task<Product?> FindProductAsync(string name, CancellationToken cancellationToken = default)
    {
        return ExecuteOptionalAsync(() => _restApi.FindProductAsync(name, cancellationToken), "product/find");
    }

    /// <inheritdoc />
    public Task<Product?> GetProductAsync(long productId, CancellationToken cancellationToken = default)
    {
        return ExecuteOptionalAsync(() => _restApi.GetProductAsync(productId, cancellationToken), "product/item");
    }

    /// <inheritdoc />
    public Task<ContractMaturity?> GetContractMaturityAsync(long maturityId, CancellationToken cancellationToken = default)
    {
        return ExecuteOptionalAsync(() => _restApi.GetContractMaturityAsync(maturityId, cancellationToken), "contractMaturity/item");
    }

    /// <inheritdoc />
    public async Task<PlaceOrderResult> PlaceOrderAsync(PlaceOrder request, CancellationToken cancellationToken = default)
    {
        PlaceOrderResult result = await ExecuteAsync(() => _restApi.PlaceOrderAsync(request, cancellationToken), "order/placeorder").ConfigureAwait(false);
        CommandResultGuard.EnsureSuccess(result.FailureReason, result.FailureText, result.OrderId, "placeOrder", requireOrderId: true);
        return result;
    }

    /// <inheritdoc />
    public async Task<PlaceOsoResult> PlaceOsoAsync(PlaceOso request, CancellationToken cancellationToken = default)
    {
        PlaceOsoResult result = await ExecuteAsync(() => _restApi.PlaceOsoAsync(request, cancellationToken), "order/placeoso").ConfigureAwait(false);
        CommandResultGuard.EnsureSuccess(result.FailureReason, result.FailureText, result.OrderId, "placeOSO", requireOrderId: true);
        return result;
    }

    /// <inheritdoc />
    public async Task<PlaceOcoResult> PlaceOcoAsync(PlaceOco request, CancellationToken cancellationToken = default)
    {
        PlaceOcoResult result = await ExecuteAsync(() => _restApi.PlaceOcoAsync(request, cancellationToken), "order/placeoco").ConfigureAwait(false);
        CommandResultGuard.EnsureSuccess(result.FailureReason, result.FailureText, result.OrderId, "placeOCO", requireOrderId: true);
        return result;
    }

    /// <inheritdoc />
    public async Task<CommandResult> ModifyOrderAsync(ModifyOrder request, CancellationToken cancellationToken = default)
    {
        CommandResult result = await ExecuteAsync(() => _restApi.ModifyOrderAsync(request, cancellationToken), "order/modifyorder").ConfigureAwait(false);
        CommandResultGuard.EnsureSuccess(result.FailureReason, result.FailureText, orderId: null, "modifyOrder");
        return result;
    }

    /// <inheritdoc />
    public async Task<CommandResult> CancelOrderAsync(CancelOrder request, CancellationToken cancellationToken = default)
    {
        CommandResult result = await ExecuteAsync(() => _restApi.CancelOrderAsync(request, cancellationToken), "order/cancelorder").ConfigureAwait(false);
        CommandResultGuard.EnsureSuccess(result.FailureReason, result.FailureText, orderId: null, "cancelOrder");
        return result;
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<Order>> GetOrdersAsync(CancellationToken cancellationToken = default)
    {
        return ExecuteAsync(() => _restApi.ListOrdersAsync(cancellationToken), "order/list");
    }

    /// <inheritdoc />
    public Task<Order?> GetOrderAsync(long orderId, CancellationToken cancellationToken = default)
    {
        return ExecuteOptionalAsync(() => _restApi.GetOrderAsync(orderId, cancellationToken), "order/item");
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<Position>> GetPositionsAsync(CancellationToken cancellationToken = default)
    {
        return ExecuteAsync(() => _restApi.ListPositionsAsync(cancellationToken), "position/list");
    }

    /// <inheritdoc />
    public async Task<CommandResult> LiquidatePositionAsync(LiquidatePosition request, CancellationToken cancellationToken = default)
    {
        CommandResult result = await ExecuteAsync(() => _restApi.LiquidatePositionAsync(request, cancellationToken), "order/liquidateposition").ConfigureAwait(false);
        CommandResultGuard.EnsureSuccess(result.FailureReason, result.FailureText, orderId: null, "liquidatePosition");
        return result;
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<Fill>> GetFillsAsync(CancellationToken cancellationToken = default)
    {
        return ExecuteAsync(() => _restApi.ListFillsAsync(cancellationToken), "fill/list");
    }

    /// <inheritdoc />
    public async Task<bool> PingAsync(CancellationToken cancellationToken = default)
    {
        AuthMe me = await GetAuthMeAsync(cancellationToken).ConfigureAwait(false);
        return string.IsNullOrWhiteSpace(me.ErrorText);
    }

    /// <inheritdoc />
    public Task<AuthMe> GetAuthMeAsync(CancellationToken cancellationToken = default)
    {
        return ExecuteAsync(() => _restApi.MeAsync(cancellationToken), "auth/me");
    }

    private async Task<T> ExecuteAsync<T>(Func<Task<T>> operation, string endpoint)
    {
        try
        {
            _logger.LogDebug("Calling Tradovate REST {Endpoint}", endpoint);
            return await operation().ConfigureAwait(false);
        }
        catch (ApiException ex)
        {
            throw ApiExceptionMapper.Map(ex);
        }
    }

    private async Task<T?> ExecuteOptionalAsync<T>(Func<Task<T>> operation, string endpoint)
        where T : class
    {
        try
        {
            _logger.LogDebug("Calling Tradovate REST {Endpoint}", endpoint);
            return await operation().ConfigureAwait(false);
        }
        catch (ApiException ex) when (ApiExceptionMapper.IsNotFound(ex))
        {
            return null;
        }
        catch (ApiException ex)
        {
            throw ApiExceptionMapper.Map(ex);
        }
    }
}
