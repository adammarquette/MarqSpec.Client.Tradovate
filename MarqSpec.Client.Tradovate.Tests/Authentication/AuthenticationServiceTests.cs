using FakeItEasy;
using FluentAssertions;
using MarqSpec.Client.Tradovate.Api.Models;
using MarqSpec.Client.Tradovate.Api.Rest;
using MarqSpec.Client.Tradovate.Authentication;
using MarqSpec.Client.Tradovate.Configuration;
using MarqSpec.Client.Tradovate.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Refit;

namespace MarqSpec.Client.Tradovate.Tests.Authentication;

public sealed class AuthenticationServiceTests
{
    [Fact]
    public async Task GetAccessTokenAsync_ShouldAcquireBothTokens_WhenNoneAreCached()
    {
        ITradovateAuthApi api = A.Fake<ITradovateAuthApi>();
        A.CallTo(() => api.RequestAccessTokenAsync(A<AccessTokenRequest>._, A<CancellationToken>._))
            .Returns(new AccessTokenResponse
            {
                AccessToken = "trade-token",
                MdAccessToken = "md-token",
                ExpirationTime = DateTimeOffset.UtcNow.AddMinutes(90),
                UserId = 42,
            });

        AuthenticationService service = CreateService(api);

        string access = await service.GetAccessTokenAsync();
        string md = await service.GetMarketDataAccessTokenAsync();
        long? userId = await service.GetUserIdAsync();

        access.Should().Be("trade-token");
        md.Should().Be("md-token");
        userId.Should().Be(42);
        A.CallTo(() => api.RequestAccessTokenAsync(A<AccessTokenRequest>._, A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => api.RenewAccessTokenAsync(A<string>._, A<CancellationToken>._))
            .MustNotHaveHappened();
    }

    [Fact]
    public async Task GetMarketDataAccessTokenAsync_ShouldFallBackToAccessToken_WhenMdTokenIsOmitted()
    {
        ITradovateAuthApi api = A.Fake<ITradovateAuthApi>();
        A.CallTo(() => api.RequestAccessTokenAsync(A<AccessTokenRequest>._, A<CancellationToken>._))
            .Returns(new AccessTokenResponse
            {
                AccessToken = "trade-token",
                ExpirationTime = DateTimeOffset.UtcNow.AddMinutes(90),
            });

        AuthenticationService service = CreateService(api);

        string md = await service.GetMarketDataAccessTokenAsync();

        md.Should().Be("trade-token");
    }

    [Fact]
    public async Task GetAccessTokenAsync_ShouldRenew_WhenInsideLeadTimeWindow()
    {
        ITradovateAuthApi api = A.Fake<ITradovateAuthApi>();
        A.CallTo(() => api.RequestAccessTokenAsync(A<AccessTokenRequest>._, A<CancellationToken>._))
            .Returns(new AccessTokenResponse
            {
                AccessToken = "old-token",
                ExpirationTime = DateTimeOffset.UtcNow.AddMinutes(10),
            });
        A.CallTo(() => api.RenewAccessTokenAsync(A<string>._, A<CancellationToken>._))
            .Returns(new AccessTokenResponse
            {
                AccessToken = "new-token",
                MdAccessToken = "new-md",
                ExpirationTime = DateTimeOffset.UtcNow.AddMinutes(90),
            });

        AuthenticationService service = CreateService(api);
        await service.GetAccessTokenAsync();
        string renewed = await service.GetAccessTokenAsync();

        renewed.Should().Be("new-token");
        A.CallTo(() => api.RenewAccessTokenAsync("Bearer old-token", A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task GetAccessTokenAsync_ShouldThrowAuthenticationExceptionWithoutSecrets_WhenServerRejects()
    {
        ITradovateAuthApi api = A.Fake<ITradovateAuthApi>();
        A.CallTo(() => api.RequestAccessTokenAsync(A<AccessTokenRequest>._, A<CancellationToken>._))
            .Returns(new AccessTokenResponse { ErrorText = "bad credentials for demo-user" });

        AuthenticationService service = CreateService(api);

        Func<Task> act = () => service.GetAccessTokenAsync();

        AuthenticationException ex = (await act.Should().ThrowAsync<AuthenticationException>()).Which;
        ex.Message.Should().NotContain("demo-pass");
        ex.Message.Should().NotContain("demo-user");
        ex.Message.Should().NotContain("trade-token");
        ex.Message.Should().NotContain("md-token");
    }

    [Fact]
    public async Task GetAccessTokenAsync_ShouldMapApiExceptionWithoutLoggingSecrets()
    {
        ITradovateAuthApi api = A.Fake<ITradovateAuthApi>();
        var apiException = await ApiException.Create(
            new HttpRequestMessage(HttpMethod.Post, "https://demo.tradovateapi.com/v1/auth/accesstokenrequest"),
            HttpMethod.Post,
            new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized)
            {
                Content = new StringContent("password=demo-pass"),
            },
            new RefitSettings());
        A.CallTo(() => api.RequestAccessTokenAsync(A<AccessTokenRequest>._, A<CancellationToken>._))
            .Throws(apiException);

        AuthenticationService service = CreateService(api);

        Func<Task> act = () => service.GetAccessTokenAsync();

        AuthenticationException ex = (await act.Should().ThrowAsync<AuthenticationException>()).Which;
        ex.Message.Should().Contain("401");
        ex.Message.Should().NotContain("demo-pass");
    }

    private static AuthenticationService CreateService(ITradovateAuthApi api)
    {
        var options = Options.Create(new TradovateOptions
        {
            Username = "demo-user",
            Password = "demo-pass",
            RestBaseUrl = "https://demo.tradovateapi.com/v1",
            TradingSocketUrl = "wss://demo.tradovateapi.com/v1/websocket",
            MarketDataSocketUrl = "wss://md.tradovateapi.com/v1/websocket",
            TokenRenewalLeadTime = TimeSpan.FromMinutes(15),
        });

        return new AuthenticationService(api, options, A.Fake<ILogger<AuthenticationService>>());
    }
}
