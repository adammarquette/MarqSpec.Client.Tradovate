using System.Net;
using System.Text;
using FluentAssertions;
using MarqSpec.Client.Tradovate.Api.Models;
using MarqSpec.Client.Tradovate.Api.Rest;
using MarqSpec.Client.Tradovate.Internal;
using MarqSpec.Client.Tradovate.Serialization;
using Refit;

namespace MarqSpec.Client.Tradovate.Tests.Internal;

public sealed class TradovateHttpTests
{
    [Theory]
    [InlineData("https://demo.tradovateapi.com/v1")]
    [InlineData("https://demo.tradovateapi.com/v1/")]
    public void CreateRestBaseAddress_ShouldEndWithSlash_WhenHostIsDocumentedDemo(string restBaseUrl)
    {
        Uri address = TradovateHttp.CreateRestBaseAddress(restBaseUrl);

        address.AbsoluteUri.Should().Be("https://demo.tradovateapi.com/v1/");
    }

    [Fact]
    public async Task BasePathHandler_ShouldRestoreV1_WhenAbsolutePathRouteDropsIt()
    {
        var capturing = new CapturingHandler();
        var handler = new TradovateBasePathHandler(TradovateHttp.CreateRestBaseAddress("https://demo.tradovateapi.com/v1"))
        {
            InnerHandler = capturing,
        };
        using var invoker = new HttpMessageInvoker(handler);

        await invoker.SendAsync(
            new HttpRequestMessage(HttpMethod.Post, "https://demo.tradovateapi.com/order/placeorder"),
            CancellationToken.None);
        capturing.LastRequestUri.Should().Be("https://demo.tradovateapi.com/v1/order/placeorder");

        await invoker.SendAsync(
            new HttpRequestMessage(HttpMethod.Get, "https://demo.tradovateapi.com/account/item?id=9"),
            CancellationToken.None);
        capturing.LastRequestUri.Should().Be("https://demo.tradovateapi.com/v1/account/item?id=9");
    }

    [Fact]
    public void ApplyBasePath_ShouldCollapseDoubleSlash_WhenRefitConcatenatesBaseAndRoute()
    {
        Uri combined = TradovateBasePathHandler.ApplyBasePath(
            new Uri("https://demo.tradovateapi.com/v1//order/placeorder"),
            "/v1");

        combined.Should().Be(new Uri("https://demo.tradovateapi.com/v1/order/placeorder"));
    }

    [Theory]
    [InlineData("https://demo.tradovateapi.com/v1")]
    [InlineData("https://demo.tradovateapi.com/v1/")]
    public async Task RefitRoutes_ShouldKeepV1Path_WhenRestBaseUrlIsDocumentedHost(string restBaseUrl)
    {
        var capturing = new CapturingHandler();
        var handler = new TradovateBasePathHandler(TradovateHttp.CreateRestBaseAddress(restBaseUrl))
        {
            InnerHandler = capturing,
        };
        using var http = new HttpClient(handler)
        {
            BaseAddress = TradovateHttp.CreateRestBaseAddress(restBaseUrl),
        };
        ITradovateRestApi rest = RestService.For<ITradovateRestApi>(http, new RefitSettings
        {
            ContentSerializer = new SystemTextJsonContentSerializer(TradovateJson.Options),
        });
        ITradovateAuthApi auth = RestService.For<ITradovateAuthApi>(http, new RefitSettings
        {
            ContentSerializer = new SystemTextJsonContentSerializer(TradovateJson.Options),
        });

        await rest.PlaceOrderAsync(new PlaceOrder
        {
            AccountSpec = "DEMO",
            Action = OrderAction.Buy,
            Symbol = "ESM24",
            OrderQty = 1,
            OrderType = OrderType.Market,
            IsAutomated = true,
        });
        capturing.LastRequestUri.Should().Be("https://demo.tradovateapi.com/v1/order/placeorder");

        await auth.RequestAccessTokenAsync(new AccessTokenRequest
        {
            Name = "demo-user",
            Password = "demo-pass",
        });
        capturing.LastRequestUri.Should().Be("https://demo.tradovateapi.com/v1/auth/accesstokenrequest");
    }

    private sealed class CapturingHandler : HttpMessageHandler
    {
        public Uri? LastRequestUri { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LastRequestUri = request.RequestUri;
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{}", Encoding.UTF8, "application/json"),
            });
        }
    }
}
