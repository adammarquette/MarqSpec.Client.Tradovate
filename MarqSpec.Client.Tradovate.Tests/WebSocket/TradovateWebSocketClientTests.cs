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

    private static TradovateWebSocketClient CreateClient(IAuthenticationService auth, FakeWebSocketTransport transport)
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
                HeartbeatInterval = TimeSpan.FromHours(1),
                ServerSilenceTimeout = TimeSpan.FromHours(1),
            },
        });

        return new TradovateWebSocketClient(
            auth,
            options,
            A.Fake<ILogger<TradovateWebSocketClient>>(),
            new FakeWebSocketTransportFactory(transport));
    }
}
