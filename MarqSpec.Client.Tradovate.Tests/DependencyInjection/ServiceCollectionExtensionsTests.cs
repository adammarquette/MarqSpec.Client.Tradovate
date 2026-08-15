using FluentAssertions;
using MarqSpec.Client.Tradovate.Configuration;
using MarqSpec.Client.Tradovate.DependencyInjection;
using MarqSpec.Client.Tradovate.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MarqSpec.Client.Tradovate.Tests.DependencyInjection;

public sealed class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddTradovateApiClient_ShouldThrowOnResolve_WhenHostIsMissing()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Tradovate:Username"] = "demo-user",
                ["Tradovate:Password"] = "demo-pass",
            })
            .Build();

        services.AddTradovateApiClient(configuration);
        ServiceProvider provider = services.BuildServiceProvider();

        Action act = () => provider.GetRequiredService<ITradovateApiClient>();

        act.Should().Throw<InvalidOperationException>().WithMessage("*RestBaseUrl*");
    }

    [Fact]
    public async Task AddTradovateApiClient_ShouldRegisterClients_WhenHostIsConfigured()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddTradovateApiClient(options =>
        {
            options.Username = "demo-user";
            options.Password = "demo-pass";
            options.RestBaseUrl = "https://demo.tradovateapi.com/v1";
            options.TradingSocketUrl = "wss://demo.tradovateapi.com/v1/websocket";
            options.MarketDataSocketUrl = "wss://md.tradovateapi.com/v1/websocket";
        });

        await using ServiceProvider provider = services.BuildServiceProvider();

        provider.GetRequiredService<ITradovateApiClient>().ConfiguredHost.Should().Be("https://demo.tradovateapi.com/v1");
        provider.GetRequiredService<ITradovateWebSocketClient>().Should().NotBeNull();
    }
}
