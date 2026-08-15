using MarqSpec.Client.Tradovate.Api.Models;

namespace MarqSpec.Client.Tradovate.WebSocket;

/// <summary>
/// Dual-socket Tradovate WebSocket client. Encapsulates the frame protocol; consumers never see <c>o</c>/<c>a</c>/<c>h</c>/<c>c</c>.
/// </summary>
public interface ITradovateWebSocketClient : IAsyncDisposable
{
    /// <summary>Gets the trading/account socket state.</summary>
    ConnectionState TradingState { get; }

    /// <summary>Gets the market-data socket state.</summary>
    ConnectionState MarketDataState { get; }

    /// <summary>Occurs when either socket changes connection state.</summary>
    event EventHandler<ConnectionStatusChange>? ConnectionStatusChanged;

    /// <summary>Occurs when a subscribe or invoke fails to send.</summary>
    event EventHandler<WebSocketMessageFailedEventArgs>? MessageSendFailed;

    /// <summary>Occurs after a successful <c>user/syncrequest</c> snapshot.</summary>
    event EventHandler<SyncResult>? SyncCompleted;

    /// <summary>Occurs when a trading-socket <c>props</c> entity event arrives.</summary>
    event EventHandler<EntityPropsEvent>? EntityReceived;

    /// <summary>Occurs when an order entity is created or updated.</summary>
    event EventHandler<Order>? OrderReceived;

    /// <summary>Occurs when a position entity is created or updated.</summary>
    event EventHandler<Position>? PositionReceived;

    /// <summary>Occurs when a fill entity is created or updated.</summary>
    event EventHandler<Fill>? FillReceived;

    /// <summary>Occurs when a cash-balance entity is created or updated.</summary>
    event EventHandler<CashBalance>? CashBalanceReceived;

    /// <summary>Occurs when a quote arrives on the market-data socket.</summary>
    event EventHandler<Quote>? QuoteReceived;

    /// <summary>Occurs when a DOM book arrives on the market-data socket.</summary>
    event EventHandler<DomBook>? DomReceived;

    /// <summary>Occurs when chart bars arrive on the market-data socket.</summary>
    event EventHandler<IReadOnlyList<ChartBar>>? ChartBarsReceived;

    /// <summary>
    /// Connects and authorizes the trading/account socket.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task ConnectTradingAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Connects and authorizes the market-data socket with <c>mdAccessToken</c>.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task ConnectMarketDataAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Disconnects the trading/account socket.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task DisconnectTradingAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Disconnects the market-data socket.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task DisconnectMarketDataAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends <c>user/syncrequest</c> on the trading socket and returns the snapshot.
    /// </summary>
    /// <param name="request">The sync request. Typically the authenticated user id.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The entity snapshot.</returns>
    Task<SyncResult> SyncRequestAsync(SyncRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Subscribes to quotes for one contract. One subscription per type per contract.
    /// </summary>
    /// <param name="symbolOrContractId">A symbol (e.g. <c>ESM24</c>) or numeric contract id.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task SubscribeQuoteAsync(string symbolOrContractId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Unsubscribes from quotes for one contract.
    /// </summary>
    /// <param name="symbolOrContractId">The same key passed to <see cref="SubscribeQuoteAsync"/>.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task UnsubscribeQuoteAsync(string symbolOrContractId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Subscribes to DOM for one contract. One subscription per type per contract.
    /// </summary>
    /// <param name="symbolOrContractId">A symbol or numeric contract id.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task SubscribeDomAsync(string symbolOrContractId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Unsubscribes from DOM for one contract.
    /// </summary>
    /// <param name="symbolOrContractId">The same key passed to <see cref="SubscribeDomAsync"/>.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task UnsubscribeDomAsync(string symbolOrContractId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Subscribes to streaming chart bars for one contract. One subscription per type per contract.
    /// </summary>
    /// <param name="request">The chart request.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task SubscribeChartAsync(ChartRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves historical bars via the market-data socket (<c>md/getChart</c>).
    /// </summary>
    /// <param name="request">The chart request.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The bars Tradovate returned.</returns>
    Task<IReadOnlyList<ChartBar>> GetHistoricalBarsAsync(ChartRequest request, CancellationToken cancellationToken = default);
}
