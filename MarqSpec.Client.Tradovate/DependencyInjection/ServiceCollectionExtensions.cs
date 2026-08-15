using MarqSpec.Client.Tradovate.Api.Rest;
using MarqSpec.Client.Tradovate.Authentication;
using MarqSpec.Client.Tradovate.Configuration;
using MarqSpec.Client.Tradovate.Internal;
using MarqSpec.Client.Tradovate.Serialization;
using MarqSpec.Client.Tradovate.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Refit;

namespace MarqSpec.Client.Tradovate.DependencyInjection;

/// <summary>
/// Extension methods for registering the Tradovate client.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Tradovate REST and WebSocket clients. Host URLs are required; there is no default.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration root that contains a <c>Tradovate</c> section.</param>
    /// <returns>The same service collection.</returns>
    public static IServiceCollection AddTradovateApiClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<TradovateOptions>(options =>
        {
            configuration.GetSection(TradovateOptions.SectionName).Bind(options);
            OverlayEnvironment(options);
        });

        return AddTradovateApiClient(services);
    }

    /// <summary>
    /// Adds Tradovate REST and WebSocket clients using a previously configured <see cref="TradovateOptions"/> registration.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">A callback that sets required host URLs and credentials.</param>
    /// <returns>The same service collection.</returns>
    public static IServiceCollection AddTradovateApiClient(this IServiceCollection services, Action<TradovateOptions> configure)
    {
        services.Configure(configure);
        return AddTradovateApiClient(services);
    }

    private static IServiceCollection AddTradovateApiClient(IServiceCollection services)
    {
        var refitSettings = new RefitSettings
        {
            ContentSerializer = new SystemTextJsonContentSerializer(TradovateJson.Options),
        };

        services.AddSingleton<IAuthenticationService>(sp =>
        {
            ITradovateAuthApi authApi = RestService.For<ITradovateAuthApi>(CreateAuthHttpClient(sp), refitSettings);
            return new AuthenticationService(
                authApi,
                sp.GetRequiredService<IOptions<TradovateOptions>>(),
                sp.GetRequiredService<ILogger<AuthenticationService>>());
        });

        services.AddRefitClient<ITradovateRestApi>(refitSettings)
            .ConfigureHttpClient((sp, client) =>
            {
                TradovateOptions options = sp.GetRequiredService<IOptions<TradovateOptions>>().Value;
                options.Validate();
                client.BaseAddress = new Uri(options.RestBaseUrl);
            })
            .AddHttpMessageHandler(sp => new AuthenticationHandler(sp.GetRequiredService<IAuthenticationService>()))
            .AddHttpMessageHandler(sp => new TradovateRetryHandler(sp.GetRequiredService<IOptions<TradovateOptions>>().Value.Retry));

        services.AddSingleton<ITradovateApiClient>(sp => new TradovateApiClient(
            sp.GetRequiredService<ITradovateRestApi>(),
            sp.GetRequiredService<IOptions<TradovateOptions>>(),
            sp.GetRequiredService<ILogger<TradovateApiClient>>()));

        services.AddSingleton<ITradovateWebSocketClient, TradovateWebSocketClient>();

        return services;
    }

    private static HttpClient CreateAuthHttpClient(IServiceProvider services)
    {
        TradovateOptions options = services.GetRequiredService<IOptions<TradovateOptions>>().Value;
        options.Validate();
        var client = new HttpClient
        {
            BaseAddress = new Uri(options.RestBaseUrl),
        };
        return client;
    }

    private static void OverlayEnvironment(TradovateOptions options)
    {
        Overlay(Environment.GetEnvironmentVariable("TRADOVATE_USERNAME"), value => options.Username = value);
        Overlay(Environment.GetEnvironmentVariable("TRADOVATE_PASSWORD"), value => options.Password = value);
        Overlay(Environment.GetEnvironmentVariable("TRADOVATE_APP_ID"), value => options.AppId = value);
        Overlay(Environment.GetEnvironmentVariable("TRADOVATE_APP_VERSION"), value => options.AppVersion = value);
        Overlay(Environment.GetEnvironmentVariable("TRADOVATE_DEVICE_ID"), value => options.DeviceId = value);
        Overlay(Environment.GetEnvironmentVariable("TRADOVATE_CID"), value => options.Cid = value);
        Overlay(Environment.GetEnvironmentVariable("TRADOVATE_SECRET"), value => options.Secret = value);
        Overlay(Environment.GetEnvironmentVariable("TRADOVATE_REST_BASE_URL"), value => options.RestBaseUrl = value);
        Overlay(Environment.GetEnvironmentVariable("TRADOVATE_TRADING_SOCKET_URL"), value => options.TradingSocketUrl = value);
        Overlay(Environment.GetEnvironmentVariable("TRADOVATE_MARKET_DATA_SOCKET_URL"), value => options.MarketDataSocketUrl = value);
    }

    private static void Overlay(string? value, Action<string> assign)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            assign(value);
        }
    }
}
