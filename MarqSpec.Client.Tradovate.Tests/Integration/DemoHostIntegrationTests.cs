using FluentAssertions;
using MarqSpec.Client.Tradovate.Configuration;
using MarqSpec.Client.Tradovate.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace MarqSpec.Client.Tradovate.Tests.Integration;

[Trait("Category", "Integration")]
public sealed class DemoHostIntegrationTests
{
    [Fact]
    public async Task GetAccountsAsync_ShouldListAccounts_WhenDemoCredentialsArePresent()
    {
        if (!TryCreateClient(out ITradovateApiClient? client, out _))
        {
            // Stub: demo credentials are optional. CI unit runs never hit the network.
            return;
        }

        client!.ConfiguredHost.Should().StartWith("https://demo.tradovateapi.com", "integration tests are demo-only");

        var accounts = await client.GetAccountsAsync();

        accounts.Should().NotBeNull();
    }

    [Fact]
    public async Task PingAsync_ShouldSucceed_WhenDemoCredentialsArePresent()
    {
        if (!TryCreateClient(out ITradovateApiClient? client, out _))
        {
            return;
        }

        bool ok = await client!.PingAsync();

        ok.Should().BeTrue();
    }

    private static bool TryCreateClient(out ITradovateApiClient? client, out string? skipReason)
    {
        string? username = Environment.GetEnvironmentVariable("TRADOVATE_USERNAME");
        string? password = Environment.GetEnvironmentVariable("TRADOVATE_PASSWORD");
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            client = null;
            skipReason = "TRADOVATE_USERNAME / TRADOVATE_PASSWORD are not set.";
            return false;
        }

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddTradovateApiClient(options =>
        {
            options.Username = username;
            options.Password = password;
            options.AppId = Environment.GetEnvironmentVariable("TRADOVATE_APP_ID");
            options.AppVersion = Environment.GetEnvironmentVariable("TRADOVATE_APP_VERSION");
            options.Cid = Environment.GetEnvironmentVariable("TRADOVATE_CID");
            options.Secret = Environment.GetEnvironmentVariable("TRADOVATE_SECRET");
            options.RestBaseUrl = "https://demo.tradovateapi.com/v1";
            options.TradingSocketUrl = "wss://demo.tradovateapi.com/v1/websocket";
            options.MarketDataSocketUrl = "wss://md.tradovateapi.com/v1/websocket";
        });

        client = services.BuildServiceProvider().GetRequiredService<ITradovateApiClient>();
        skipReason = null;
        return true;
    }
}
