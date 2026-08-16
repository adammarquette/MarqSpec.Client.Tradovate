using System.Collections.Concurrent;
using System.Net;
using System.Text.Json;
using MarqSpec.Client.Tradovate.Api.Models;
using MarqSpec.Client.Tradovate.Authentication;
using MarqSpec.Client.Tradovate.Configuration;
using MarqSpec.Client.Tradovate.Exceptions;
using MarqSpec.Client.Tradovate.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MarqSpec.Client.Tradovate.WebSocket;

/// <summary>
/// Dual-socket Tradovate client. Frame protocol, heartbeats, and request correlation stay encapsulated.
/// </summary>
public sealed class TradovateWebSocketClient : ITradovateWebSocketClient
{
    private readonly IAuthenticationService _authentication;
    private readonly IWebSocketTransportFactory _transportFactory;
    private readonly TradovateOptions _options;
    private readonly ILogger<TradovateWebSocketClient> _logger;
    private readonly SocketConnection _trading;
    private readonly SocketConnection _marketData;
    private readonly ConcurrentDictionary<string, byte> _quoteSubscriptions = new(StringComparer.Ordinal);
    private readonly ConcurrentDictionary<string, byte> _domSubscriptions = new(StringComparer.Ordinal);
    private readonly ConcurrentDictionary<string, ChartRequest> _chartSubscriptions = new(StringComparer.Ordinal);

    /// <summary>
    /// Initializes a new instance of the <see cref="TradovateWebSocketClient"/> class.
    /// </summary>
    /// <param name="authentication">The dual-token auth service.</param>
    /// <param name="options">The Tradovate options. Socket URLs are required.</param>
    /// <param name="logger">The logger. Must never receive tokens.</param>
    public TradovateWebSocketClient(
        IAuthenticationService authentication,
        IOptions<TradovateOptions> options,
        ILogger<TradovateWebSocketClient> logger)
        : this(authentication, options, logger, new ClientWebSocketTransportFactory())
    {
    }

    internal TradovateWebSocketClient(
        IAuthenticationService authentication,
        IOptions<TradovateOptions> options,
        ILogger<TradovateWebSocketClient> logger,
        IWebSocketTransportFactory transportFactory)
    {
        _authentication = authentication;
        _options = options.Value;
        _logger = logger;
        _transportFactory = transportFactory;
        _options.Validate();
        _trading = new SocketConnection(this, isTrading: true);
        _marketData = new SocketConnection(this, isTrading: false);
    }

    /// <inheritdoc />
    public ConnectionState TradingState => _trading.State;

    /// <inheritdoc />
    public ConnectionState MarketDataState => _marketData.State;

    /// <inheritdoc />
    public event EventHandler<ConnectionStatusChange>? ConnectionStatusChanged;

    /// <inheritdoc />
    public event EventHandler<WebSocketMessageFailedEventArgs>? MessageSendFailed;

    /// <inheritdoc />
    public event EventHandler<SyncResult>? SyncCompleted;

    /// <inheritdoc />
    public event EventHandler<EntityPropsEvent>? EntityReceived;

    /// <inheritdoc />
    public event EventHandler<Order>? OrderReceived;

    /// <inheritdoc />
    public event EventHandler<Position>? PositionReceived;

    /// <inheritdoc />
    public event EventHandler<Fill>? FillReceived;

    /// <inheritdoc />
    public event EventHandler<CashBalance>? CashBalanceReceived;

    /// <inheritdoc />
    public event EventHandler<Quote>? QuoteReceived;

    /// <inheritdoc />
    public event EventHandler<DomBook>? DomReceived;

    /// <inheritdoc />
    public event EventHandler<IReadOnlyList<ChartBar>>? ChartBarsReceived;

    /// <inheritdoc />
    public Task ConnectTradingAsync(CancellationToken cancellationToken = default)
    {
        return _trading.ConnectAsync(new Uri(_options.TradingSocketUrl), cancellationToken);
    }

    /// <inheritdoc />
    public Task ConnectMarketDataAsync(CancellationToken cancellationToken = default)
    {
        return _marketData.ConnectAsync(new Uri(_options.MarketDataSocketUrl), cancellationToken);
    }

    /// <inheritdoc />
    public Task DisconnectTradingAsync(CancellationToken cancellationToken = default)
    {
        return _trading.DisconnectAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task DisconnectMarketDataAsync(CancellationToken cancellationToken = default)
    {
        return _marketData.DisconnectAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<SyncResult> SyncRequestAsync(SyncRequest request, CancellationToken cancellationToken = default)
    {
        string body = JsonSerializer.Serialize(request, TradovateJson.Options);
        FramePayload payload = await _trading.InvokeAsync("user/syncrequest", body, cancellationToken).ConfigureAwait(false);
        SyncResult result = payload.Data is { } data ? EntitySnapshotParser.ReadSync(data) : new SyncResult();
        SyncCompleted?.Invoke(this, result);
        return result;
    }

    /// <inheritdoc />
    public async Task SubscribeQuoteAsync(string symbolOrContractId, CancellationToken cancellationToken = default)
    {
        _quoteSubscriptions[symbolOrContractId] = 0;
        await InvokeSubscriptionAsync(_marketData, "md/subscribeQuote", symbolOrContractId, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task UnsubscribeQuoteAsync(string symbolOrContractId, CancellationToken cancellationToken = default)
    {
        _quoteSubscriptions.TryRemove(symbolOrContractId, out _);
        await InvokeSubscriptionAsync(_marketData, "md/unsubscribeQuote", symbolOrContractId, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task SubscribeDomAsync(string symbolOrContractId, CancellationToken cancellationToken = default)
    {
        _domSubscriptions[symbolOrContractId] = 0;
        await InvokeSubscriptionAsync(_marketData, "md/subscribeDOM", symbolOrContractId, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task UnsubscribeDomAsync(string symbolOrContractId, CancellationToken cancellationToken = default)
    {
        _domSubscriptions.TryRemove(symbolOrContractId, out _);
        await InvokeSubscriptionAsync(_marketData, "md/unsubscribeDOM", symbolOrContractId, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task SubscribeChartAsync(ChartRequest request, CancellationToken cancellationToken = default)
    {
        string key = ChartKey(request);
        _chartSubscriptions[key] = request;
        string body = SerializeChart(request);
        await _marketData.InvokeAsync("md/getChart", body, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ChartBar>> GetHistoricalBarsAsync(ChartRequest request, CancellationToken cancellationToken = default)
    {
        string body = SerializeChart(request);
        FramePayload payload = await _marketData.InvokeAsync("md/getChart", body, cancellationToken).ConfigureAwait(false);
        return payload.Data is { } data ? MarketDataParser.ReadBars(data) : [];
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        await _trading.DisposeAsync().ConfigureAwait(false);
        await _marketData.DisposeAsync().ConfigureAwait(false);
    }

    private async Task InvokeSubscriptionAsync(SocketConnection socket, string endpoint, string symbolOrContractId, CancellationToken cancellationToken)
    {
        string body = long.TryParse(symbolOrContractId, out long contractId)
            ? JsonSerializer.Serialize(new { contractId }, TradovateJson.LooseOptions)
            : JsonSerializer.Serialize(new { symbol = symbolOrContractId }, TradovateJson.LooseOptions);
        await socket.InvokeAsync(endpoint, body, cancellationToken).ConfigureAwait(false);
    }

    private static string ChartKey(ChartRequest request)
    {
        return $"{request.Symbol ?? request.ContractId?.ToString()}:{request.UnderlyingType}:{request.ElementSize}";
    }

    private static string SerializeChart(ChartRequest request)
    {
        var payload = new Dictionary<string, object?>();
        if (!string.IsNullOrWhiteSpace(request.Symbol))
        {
            payload["symbol"] = request.Symbol;
        }

        if (request.ContractId is { } contractId)
        {
            payload["contractId"] = contractId;
        }

        payload["chartDescription"] = new Dictionary<string, object?>
        {
            ["underlyingType"] = request.UnderlyingType.ToString(),
            ["elementSize"] = request.ElementSize,
            ["elementSizeUnit"] = "UnderlyingUnits",
        };

        var timeRange = new Dictionary<string, object?>();
        if (request.ClosestTimestamp is { } closest)
        {
            timeRange["closestTimestamp"] = closest.ToUniversalTime().ToString("O");
        }

        if (request.AsFarAsTimestamp is { } farthest)
        {
            timeRange["asFarAsTimestamp"] = farthest.ToUniversalTime().ToString("O");
        }

        if (request.AsMuchAsElements is { } count)
        {
            timeRange["asMuchAsElements"] = count;
        }

        payload["timeRange"] = timeRange;
        return JsonSerializer.Serialize(payload, TradovateJson.LooseOptions);
    }

    private async Task ReplaySubscriptionsAsync(CancellationToken cancellationToken)
    {
        foreach (string key in _quoteSubscriptions.Keys)
        {
            await InvokeSubscriptionAsync(_marketData, "md/subscribeQuote", key, cancellationToken).ConfigureAwait(false);
        }

        foreach (string key in _domSubscriptions.Keys)
        {
            await InvokeSubscriptionAsync(_marketData, "md/subscribeDOM", key, cancellationToken).ConfigureAwait(false);
        }

        foreach (ChartRequest request in _chartSubscriptions.Values)
        {
            await _marketData.InvokeAsync("md/getChart", SerializeChart(request), cancellationToken).ConfigureAwait(false);
        }
    }

    private void RaiseConnection(bool isTrading, ConnectionState previous, ConnectionState current)
    {
        ConnectionStatusChanged?.Invoke(this, new ConnectionStatusChange
        {
            IsTradingSocket = isTrading,
            Previous = previous,
            Current = current,
        });
    }

    private void RaiseSendFailed(string endpoint, string message, bool isTrading)
    {
        MessageSendFailed?.Invoke(this, new WebSocketMessageFailedEventArgs(endpoint, message, isTrading));
    }

    private void DispatchPayload(FramePayload payload, bool isTrading)
    {
        if (payload.Data is not { } data)
        {
            return;
        }

        if (string.Equals(payload.Event, "props", StringComparison.OrdinalIgnoreCase))
        {
            EntityPropsEvent? props = EntitySnapshotParser.ReadProps(data);
            if (props is null)
            {
                return;
            }

            EntityReceived?.Invoke(this, props);
            DispatchTypedEntity(props);
            return;
        }

        if (!isTrading)
        {
            foreach (Quote quote in MarketDataParser.ReadQuotes(data))
            {
                QuoteReceived?.Invoke(this, quote);
            }

            foreach (DomBook book in MarketDataParser.ReadDom(data))
            {
                DomReceived?.Invoke(this, book);
            }

            IReadOnlyList<ChartBar> bars = MarketDataParser.ReadBars(data);
            if (bars.Count > 0)
            {
                ChartBarsReceived?.Invoke(this, bars);
            }
        }
    }

    private void DispatchTypedEntity(EntityPropsEvent props)
    {
        switch (props.EntityType.ToLowerInvariant())
        {
            case "order":
                Order? order = EntitySnapshotParser.Deserialize<Order>(props.Entity);
                if (order is not null)
                {
                    OrderReceived?.Invoke(this, order);
                }

                break;
            case "position":
                Position? position = EntitySnapshotParser.Deserialize<Position>(props.Entity);
                if (position is not null)
                {
                    PositionReceived?.Invoke(this, position);
                }

                break;
            case "fill":
                Fill? fill = EntitySnapshotParser.Deserialize<Fill>(props.Entity);
                if (fill is not null)
                {
                    FillReceived?.Invoke(this, fill);
                }

                break;
            case "cashbalance":
                CashBalance? balance = EntitySnapshotParser.Deserialize<CashBalance>(props.Entity);
                if (balance is not null)
                {
                    CashBalanceReceived?.Invoke(this, balance);
                }

                break;
            default:
                break;
        }
    }

    private sealed class SocketConnection : IAsyncDisposable
    {
        private readonly TradovateWebSocketClient _owner;
        private readonly bool _isTrading;
        private readonly SemaphoreSlim _connectGate = new(1, 1);
        private readonly ConcurrentDictionary<int, TaskCompletionSource<FramePayload>> _pending = new();
        private readonly ConcurrentDictionary<int, FramePayload> _early = new();
        private int _nextRequestId;
        private IWebSocketTransport? _transport;
        private CancellationTokenSource? _loopCts;
        private Task? _receiveLoop;
        private Task? _heartbeatLoop;
        private DateTimeOffset _lastServerActivity = DateTimeOffset.UtcNow;
        private ConnectionState _state = ConnectionState.Disconnected;
        private bool _manualDisconnect;

        public SocketConnection(TradovateWebSocketClient owner, bool isTrading)
        {
            _owner = owner;
            _isTrading = isTrading;
        }

        public ConnectionState State => _state;

        public async Task ConnectAsync(Uri uri, CancellationToken cancellationToken)
        {
            await _connectGate.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                _manualDisconnect = false;
                await ConnectUnlockedAsync(uri, replay: false, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                _connectGate.Release();
            }
        }

        public async Task DisconnectAsync(CancellationToken cancellationToken)
        {
            _manualDisconnect = true;
            FailPending("The Tradovate WebSocket disconnected before the request completed.");
            await StopLoopsAsync().ConfigureAwait(false);
            if (_transport is not null)
            {
                await _transport.CloseAsync(cancellationToken).ConfigureAwait(false);
                await _transport.DisposeAsync().ConfigureAwait(false);
                _transport = null;
            }

            SetState(ConnectionState.Disconnected);
        }

        public async Task<FramePayload> InvokeAsync(string endpoint, string? body, CancellationToken cancellationToken)
        {
            if (_transport is null || !_transport.IsConnected)
            {
                throw new InvalidOperationException("The WebSocket is not connected.");
            }

            int requestId = Interlocked.Increment(ref _nextRequestId);
            var completion = new TaskCompletionSource<FramePayload>(TaskCreationOptions.RunContinuationsAsynchronously);
            _pending[requestId] = completion;
            if (_early.TryRemove(requestId, out FramePayload? early))
            {
                completion.TrySetResult(early);
            }
            string frame = FrameProtocol.FormatRequest(endpoint, requestId, queryParams: null, body);
            try
            {
                await _transport.SendAsync(frame, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _pending.TryRemove(requestId, out _);
                _owner.RaiseSendFailed(endpoint, "Failed to send a WebSocket frame.", _isTrading);
                _owner._logger.LogError(ex, "Failed to send {Endpoint} on the {Socket} socket", endpoint, _isTrading ? "trading" : "market-data");
                throw;
            }

            using CancellationTokenRegistration registration = cancellationToken.Register(() => completion.TrySetCanceled(cancellationToken));
            FramePayload payload = await completion.Task.ConfigureAwait(false);
            if (payload.Status is { } status && status >= 400)
            {
                if (status == (int)HttpStatusCode.TooManyRequests)
                {
                    throw new TradovateRateLimitException($"WebSocket {endpoint} returned status 429.");
                }

                throw new TradovateApiException($"WebSocket {endpoint} returned status {status}.", status);
            }

            return payload;
        }

        public async ValueTask DisposeAsync()
        {
            await DisconnectAsync(CancellationToken.None).ConfigureAwait(false);
            _connectGate.Dispose();
        }

        private async Task ConnectUnlockedAsync(Uri uri, bool replay, CancellationToken cancellationToken)
        {
            SetState(replay ? ConnectionState.Reconnecting : ConnectionState.Connecting);
            try
            {
                FailPending("The Tradovate WebSocket disconnected before the request completed.");
                await StopLoopsAsync().ConfigureAwait(false);
                if (_transport is not null)
                {
                    await _transport.DisposeAsync().ConfigureAwait(false);
                }

                _transport = _owner._transportFactory.Create();
                await _transport.ConnectAsync(uri, cancellationToken).ConfigureAwait(false);

                string? open = await _transport.ReceiveAsync(cancellationToken).ConfigureAwait(false);
                if (open is null || FrameProtocol.Parse(open).Kind != FrameKind.Open)
                {
                    throw new TradovateApiException("The WebSocket did not send an open frame.");
                }

                string token = _isTrading
                    ? await _owner._authentication.GetAccessTokenAsync(cancellationToken).ConfigureAwait(false)
                    : await _owner._authentication.GetMarketDataAccessTokenAsync(cancellationToken).ConfigureAwait(false);

                _lastServerActivity = DateTimeOffset.UtcNow;
                _loopCts = new CancellationTokenSource();
                _receiveLoop = Task.Run(() => ReceiveLoopAsync(_loopCts.Token), CancellationToken.None);
                _heartbeatLoop = Task.Run(() => HeartbeatLoopAsync(_loopCts.Token), CancellationToken.None);

                await InvokeAsync("authorize", token, cancellationToken).ConfigureAwait(false);
                SetState(ConnectionState.Connected);
                _owner._logger.LogInformation("Connected the {Socket} Tradovate socket", _isTrading ? "trading" : "market-data");

                if (replay && !_isTrading)
                {
                    await _owner.ReplaySubscriptionsAsync(cancellationToken).ConfigureAwait(false);
                }
            }
            catch
            {
                SetState(ConnectionState.Disconnected);
                throw;
            }
        }

        private async Task ReceiveLoopAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested && _transport is not null)
                {
                    string? raw = await _transport.ReceiveAsync(cancellationToken).ConfigureAwait(false);
                    if (raw is null)
                    {
                        RequestReconnect();
                        return;
                    }

                    _lastServerActivity = DateTimeOffset.UtcNow;
                    FrameParseResult parsed = FrameProtocol.Parse(raw);
                    switch (parsed.Kind)
                    {
                        case FrameKind.Heartbeat:
                        case FrameKind.Open:
                            break;
                        case FrameKind.Close:
                            RequestReconnect();
                            return;
                        case FrameKind.Payload:
                            foreach (FramePayload payload in parsed.Payloads)
                            {
                                if (payload.RequestId is { } id)
                                {
                                    if (_pending.TryRemove(id, out TaskCompletionSource<FramePayload>? completion))
                                    {
                                        completion.TrySetResult(payload);
                                    }
                                    else
                                    {
                                        _early[id] = payload;
                                    }
                                }

                                _owner.DispatchPayload(payload, _isTrading);
                            }

                            break;
                        case FrameKind.Unknown:
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                _owner._logger.LogError(ex, "The {Socket} socket receive loop failed", _isTrading ? "trading" : "market-data");
                RequestReconnect();
            }
        }

        private async Task HeartbeatLoopAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested && _transport is not null)
                {
                    await Task.Delay(_owner._options.WebSocket.HeartbeatInterval, cancellationToken).ConfigureAwait(false);
                    if (DateTimeOffset.UtcNow - _lastServerActivity > _owner._options.WebSocket.ServerSilenceTimeout)
                    {
                        _owner._logger.LogWarning("The {Socket} socket was silent too long; reconnecting", _isTrading ? "trading" : "market-data");
                        RequestReconnect();
                        return;
                    }

                    if (_transport.IsConnected)
                    {
                        await _transport.SendAsync(FrameProtocol.HeartbeatBody, cancellationToken).ConfigureAwait(false);
                    }
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                _owner._logger.LogError(ex, "The {Socket} socket heartbeat loop failed", _isTrading ? "trading" : "market-data");
                RequestReconnect();
            }
        }

        private void RequestReconnect()
        {
            if (_manualDisconnect)
            {
                return;
            }

            _ = Task.Run(ReconnectAsync);
        }

        private async Task ReconnectAsync()
        {
            if (_manualDisconnect)
            {
                return;
            }

            try
            {
                Uri uri = new(_isTrading ? _owner._options.TradingSocketUrl : _owner._options.MarketDataSocketUrl);
                await _connectGate.WaitAsync().ConfigureAwait(false);
                try
                {
                    if (_manualDisconnect)
                    {
                        return;
                    }

                    await ConnectUnlockedAsync(uri, replay: true, CancellationToken.None).ConfigureAwait(false);
                    if (_isTrading)
                    {
                        long? userId = await _owner._authentication.GetUserIdAsync().ConfigureAwait(false);
                        if (userId is { } id)
                        {
                            await _owner.SyncRequestAsync(new SyncRequest { Users = [id] }).ConfigureAwait(false);
                        }
                    }
                }
                finally
                {
                    _connectGate.Release();
                }
            }
            catch (Exception ex)
            {
                _owner._logger.LogError(ex, "Failed to reconnect the {Socket} socket", _isTrading ? "trading" : "market-data");
                SetState(ConnectionState.Disconnected);
            }
        }

        private async Task StopLoopsAsync()
        {
            if (_loopCts is not null)
            {
                await _loopCts.CancelAsync().ConfigureAwait(false);
            }

            if (_receiveLoop is not null)
            {
                try
                {
                    await _receiveLoop.ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                }
            }

            if (_heartbeatLoop is not null)
            {
                try
                {
                    await _heartbeatLoop.ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                }
            }

            _loopCts?.Dispose();
            _loopCts = null;
            _receiveLoop = null;
            _heartbeatLoop = null;
        }

        private void FailPending(string reason)
        {
            var exception = new IOException(reason);
            foreach (int id in _pending.Keys)
            {
                if (_pending.TryRemove(id, out TaskCompletionSource<FramePayload>? completion))
                {
                    completion.TrySetException(exception);
                }
            }

            _early.Clear();
        }

        private void SetState(ConnectionState state)
        {
            ConnectionState previous = _state;
            if (previous == state)
            {
                return;
            }

            _state = state;
            _owner.RaiseConnection(_isTrading, previous, state);
        }
    }
}
