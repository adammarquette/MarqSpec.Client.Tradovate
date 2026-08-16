using System.Net;
using FluentAssertions;
using MarqSpec.Client.Tradovate.Configuration;
using MarqSpec.Client.Tradovate.Internal;

namespace MarqSpec.Client.Tradovate.Tests.Internal;

public sealed class TradovateRetryHandlerTests
{
    [Fact]
    public async Task SendAsync_ShouldNotRetry_WhenPlaceOrderReturns500()
    {
        var inner = new CountingHandler(HttpStatusCode.InternalServerError);
        var handler = new TradovateRetryHandler(FastRetry()) { InnerHandler = inner };
        using var invoker = new HttpMessageInvoker(handler);

        HttpResponseMessage response = await invoker.SendAsync(
            new HttpRequestMessage(HttpMethod.Post, "https://demo.tradovateapi.com/v1/order/placeorder"),
            CancellationToken.None);

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        inner.Calls.Should().Be(1, "placeOrder must never be auto-retried");
    }

    [Fact]
    public async Task SendAsync_ShouldRetry_WhenAccountListReturns500()
    {
        var inner = new CountingHandler(HttpStatusCode.InternalServerError);
        var handler = new TradovateRetryHandler(FastRetry()) { InnerHandler = inner };
        using var invoker = new HttpMessageInvoker(handler);

        HttpResponseMessage response = await invoker.SendAsync(
            new HttpRequestMessage(HttpMethod.Get, "https://demo.tradovateapi.com/v1/account/list"),
            CancellationToken.None);

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        inner.Calls.Should().Be(3);
    }

    [Theory]
    [InlineData("https://demo.tradovateapi.com/v1/order/placeoso")]
    [InlineData("https://demo.tradovateapi.com/v1/order/placeoco")]
    [InlineData("https://demo.tradovateapi.com/v1/order/liquidateposition")]
    public async Task SendAsync_ShouldNotRetry_WhenCommandEndpointReturns500(string url)
    {
        var inner = new CountingHandler(HttpStatusCode.InternalServerError);
        var handler = new TradovateRetryHandler(FastRetry()) { InnerHandler = inner };
        using var invoker = new HttpMessageInvoker(handler);

        HttpResponseMessage response = await invoker.SendAsync(
            new HttpRequestMessage(HttpMethod.Post, url),
            CancellationToken.None);

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        inner.Calls.Should().Be(1, "place/liquidate must never be auto-retried");
    }

    [Fact]
    public async Task SendAsync_ShouldNotRetry_WhenPlaceOrderThrowsTransportError()
    {
        var inner = new ThrowingHandler();
        var handler = new TradovateRetryHandler(FastRetry()) { InnerHandler = inner };
        using var invoker = new HttpMessageInvoker(handler);

        Func<Task> act = () => invoker.SendAsync(
            new HttpRequestMessage(HttpMethod.Post, "https://demo.tradovateapi.com/v1/order/placeorder"),
            CancellationToken.None);

        await act.Should().ThrowAsync<HttpRequestException>();
        inner.Calls.Should().Be(1, "a transport fault on placeOrder must not be retried");
    }

    [Fact]
    public async Task SendAsync_ShouldRetryCancel_WhenServerReturns429()
    {
        var inner = new CountingHandler(HttpStatusCode.TooManyRequests);
        var handler = new TradovateRetryHandler(FastRetry()) { InnerHandler = inner };
        using var invoker = new HttpMessageInvoker(handler);

        HttpResponseMessage response = await invoker.SendAsync(
            new HttpRequestMessage(HttpMethod.Post, "https://demo.tradovateapi.com/v1/order/cancelorder"),
            CancellationToken.None);

        response.StatusCode.Should().Be(HttpStatusCode.TooManyRequests);
        inner.Calls.Should().BeGreaterThan(1);
    }

    private static RetryOptions FastRetry()
    {
        return new RetryOptions
        {
            MaxRetryAttempts = 2,
            BaseDelay = TimeSpan.FromMilliseconds(1),
            MaxDelay = TimeSpan.FromMilliseconds(2),
        };
    }

    private sealed class CountingHandler : HttpMessageHandler
    {
        private readonly HttpStatusCode _status;

        public CountingHandler(HttpStatusCode status)
        {
            _status = status;
        }

        public int Calls { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Calls++;
            return Task.FromResult(new HttpResponseMessage(_status));
        }
    }

    private sealed class ThrowingHandler : HttpMessageHandler
    {
        public int Calls { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Calls++;
            throw new HttpRequestException("transport failed");
        }
    }
}
