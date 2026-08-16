using FakeItEasy;
using FluentAssertions;
using MarqSpec.Client.Tradovate.Api.Models;
using MarqSpec.Client.Tradovate.Authentication;
using MarqSpec.Client.Tradovate.Configuration;
using MarqSpec.Client.Tradovate.WebSocket;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MarqSpec.Client.Tradovate.Tests.WebSocket;

public sealed class TradovateWebSocketClientTests
{
    [Fact]
    public async Task ConnectTradingAsync_ShouldAuthorizeWithAccessToken_WhenOpenFrameArrives()
    {
        var transport = new FakeWebSocketTransport();
        transport.Enqueue("o");
        transport.Enqueue("""a[{"i":1,"s":200,"d":{}}]""");
        IAuthenticationService auth = A.Fake<IAuthenticationService>();
        A.CallTo(() => auth.GetAccessTokenAsync(A<CancellationToken>._)).Returns("trade-token");

        await using TradovateWebSocketClient client = CreateClient(auth, transport);
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        await client.ConnectTradingAsync(timeout.Token);

        client.TradingState.Should().Be(ConnectionState.Connected);
        transport.Sent.Should().Contain(frame => frame.StartsWith("authorize\n", StringComparison.Ordinal) && frame.EndsWith("trade-token", StringComparison.Ordinal));
        transport.Sent.Should().NotContain(frame => frame.Contains("md-token", StringComparison.Ordinal));
    }

    [Fact]
    public async Task ConnectMarketDataAsync_ShouldAuthorizeWithMdToken()
    {
        var transport = new FakeWebSocketTransport();
        transport.Enqueue("o");
        transport.Enqueue("""a[{"i":1,"s":200,"d":{}}]""");
        IAuthenticationService auth = A.Fake<IAuthenticationService>();
        A.CallTo(() => auth.GetMarketDataAccessTokenAsync(A<CancellationToken>._)).Returns("md-token");

        await using TradovateWebSocketClient client = CreateClient(auth, transport);
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        await client.ConnectMarketDataAsync(timeout.Token);

        client.MarketDataState.Should().Be(ConnectionState.Connected);
        transport.Sent.Should().Contain(frame => frame.EndsWith("md-token", StringComparison.Ordinal));
    }

    [Fact]
    public async Task SyncRequestAsync_ShouldReturnSnapshot_WhenServerReplies()
    {
        var transport = new FakeWebSocketTransport();
        transport.Enqueue("o");
        transport.Enqueue("""a[{"i":1,"s":200,"d":{}}]""");
        transport.Enqueue("""a[{"i":2,"s":200,"d":{"accounts":[{"name":"DEMO","userId":1,"accountType":"Customer","active":true,"clearingHouseId":1,"riskCategoryId":1,"autoLiqProfileId":1,"marginAccountType":"Speculator","legalStatus":"Individual"}]}}]""");
        IAuthenticationService auth = A.Fake<IAuthenticationService>();
        A.CallTo(() => auth.GetAccessTokenAsync(A<CancellationToken>._)).Returns("trade-token");

        await using TradovateWebSocketClient client = CreateClient(auth, transport);
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        await client.ConnectTradingAsync(timeout.Token);
        SyncResult sync = await client.SyncRequestAsync(new SyncRequest { Users = [1] }, timeout.Token);

        sync.Accounts.Should().ContainSingle().Which.Name.Should().Be("DEMO");
        transport.Sent.Should().Contain(frame => frame.StartsWith("user/syncrequest\n", StringComparison.Ordinal));
    }

    [Fact]
    public async Task SubscribeQuoteAsync_ShouldSendOneSubscribePerContract()
    {
        var transport = new FakeWebSocketTransport();
        transport.Enqueue("o");
        transport.Enqueue("""a[{"i":1,"s":200,"d":{}}]""");
        transport.Enqueue("""a[{"i":2,"s":200,"d":{}}]""");
        IAuthenticationService auth = A.Fake<IAuthenticationService>();
        A.CallTo(() => auth.GetMarketDataAccessTokenAsync(A<CancellationToken>._)).Returns("md-token");

        await using TradovateWebSocketClient client = CreateClient(auth, transport);
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        await client.ConnectMarketDataAsync(timeout.Token);
        await client.SubscribeQuoteAsync("ESM24", timeout.Token);

        transport.Sent.Should().Contain(frame => frame.StartsWith("md/subscribeQuote\n", StringComparison.Ordinal) && frame.Contains("ESM24", StringComparison.Ordinal));
    }

    [Fact]
    public async Task SyncRequestAsync_ShouldThrow_WhenSocketDisconnectsBeforeReply()
    {
        var transport = new FakeWebSocketTransport();
        transport.Enqueue("o");
        transport.Enqueue("""a[{"i":1,"s":200,"d":{}}]""");
        IAuthenticationService auth = A.Fake<IAuthenticationService>();
        A.CallTo(() => auth.GetAccessTokenAsync(A<CancellationToken>._)).Returns("trade-token");

        await using TradovateWebSocketClient client = CreateClient(auth, transport);
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        await client.ConnectTradingAsync(timeout.Token);

        Task<SyncResult> sync = client.SyncRequestAsync(new SyncRequest { Users = [1] }, timeout.Token);
        await WaitUntilAsync(() => transport.Sent.Any(frame => frame.StartsWith("user/syncrequest\n", StringComparison.Ordinal)), timeout.Token);
        await client.DisconnectTradingAsync(timeout.Token);

        await sync.Awaiting(task => task).Should().ThrowAsync<IOException>().WithMessage("*disconnected*");
    }

    [Fact]
    public async Task ConnectTradingAsync_ShouldLeaveDisconnected_WhenTransportConnectThrows()
    {
        var transport = new FakeWebSocketTransport
        {
            ConnectFault = new IOException("refused"),
        };
        IAuthenticationService auth = A.Fake<IAuthenticationService>();
        A.CallTo(() => auth.GetAccessTokenAsync(A<CancellationToken>._)).Returns("trade-token");

        await using TradovateWebSocketClient client = CreateClient(auth, transport);
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(5));

        await client.Awaiting(c => c.ConnectTradingAsync(timeout.Token)).Should().ThrowAsync<IOException>();
        client.TradingState.Should().Be(ConnectionState.Disconnected);
    }

    [Fact]
    public async Task HeartbeatLoop_ShouldReconnect_WhenHeartbeatSendFails()
    {
        var first = new FakeWebSocketTransport
        {
            SendFault = message => message == FrameProtocol.HeartbeatBody ? new IOException("send failed") : null,
        };
        first.Enqueue("o");
        first.Enqueue("""a[{"i":1,"s":200,"d":{}}]""");

        var second = new FakeWebSocketTransport();
        second.Enqueue("o");
        second.Enqueue("""a[{"i":2,"s":200,"d":{}}]""");

        IAuthenticationService auth = A.Fake<IAuthenticationService>();
        A.CallTo(() => auth.GetAccessTokenAsync(A<CancellationToken>._)).Returns("trade-token");
        var factory = new FakeWebSocketTransportFactory(first, second);

        await using TradovateWebSocketClient client = CreateClient(
            auth,
            factory,
            heartbeatInterval: TimeSpan.FromMilliseconds(20),
            serverSilenceTimeout: TimeSpan.FromHours(1));
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        await client.ConnectTradingAsync(timeout.Token);

        await WaitUntilAsync(() => factory.Created >= 2, timeout.Token);
        await WaitUntilAsync(() => client.TradingState == ConnectionState.Connected, timeout.Token);

        factory.Created.Should().BeGreaterThanOrEqualTo(2);
        client.TradingState.Should().Be(ConnectionState.Connected);
    }

    private static TradovateWebSocketClient CreateClient(IAuthenticationService auth, FakeWebSocketTransport transport)
    {
        return CreateClient(auth, new FakeWebSocketTransportFactory(transport));
    }

    private static async Task WaitUntilAsync(Func<bool> condition, CancellationToken cancellationToken)
    {
        while (!condition())
        {
            await Task.Delay(10, cancellationToken);
        }
    }

    private static TradovateWebSocketClient CreateClient(
        IAuthenticationService auth,
        FakeWebSocketTransportFactory factory,
        TimeSpan? heartbeatInterval = null,
        TimeSpan? serverSilenceTimeout = null)
    {
        var options = Options.Create(new TradovateOptions
        {
            Username = "demo-user",
            Password = "demo-pass",
            RestBaseUrl = "https://demo.tradovateapi.com/v1",
            TradingSocketUrl = "wss://demo.tradovateapi.com/v1/websocket",
            MarketDataSocketUrl = "wss://md.tradovateapi.com/v1/websocket",
            WebSocket = new WebSocketOptions
            {
                HeartbeatInterval = heartbeatInterval ?? TimeSpan.FromHours(1),
                ServerSilenceTimeout = serverSilenceTimeout ?? TimeSpan.FromHours(1),
            },
        });

        return new TradovateWebSocketClient(
            auth,
            options,
            A.Fake<ILogger<TradovateWebSocketClient>>(),
            factory);
    }
}
