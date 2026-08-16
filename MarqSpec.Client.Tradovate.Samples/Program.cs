using MarqSpec.Client.Tradovate;
using MarqSpec.Client.Tradovate.Api.Models;
using MarqSpec.Client.Tradovate.DependencyInjection;
using MarqSpec.Client.Tradovate.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.Logging.AddSimpleConsole(options => options.SingleLine = true);

string restBaseUrl = Environment.GetEnvironmentVariable("TRADOVATE_REST_BASE_URL") ?? "https://demo.tradovateapi.com/v1";
if (!restBaseUrl.Contains("demo.tradovateapi.com", StringComparison.OrdinalIgnoreCase))
{
    throw new InvalidOperationException("The sample is demo-only. Set TRADOVATE_REST_BASE_URL to https://demo.tradovateapi.com/v1.");
}

builder.Services.AddTradovateApiClient(options =>
{
    options.Username = Require("TRADOVATE_USERNAME");
    options.Password = Require("TRADOVATE_PASSWORD");
    options.AppId = Environment.GetEnvironmentVariable("TRADOVATE_APP_ID");
    options.AppVersion = Environment.GetEnvironmentVariable("TRADOVATE_APP_VERSION");
    options.DeviceId = Environment.GetEnvironmentVariable("TRADOVATE_DEVICE_ID");
    options.Cid = Environment.GetEnvironmentVariable("TRADOVATE_CID");
    options.Secret = Environment.GetEnvironmentVariable("TRADOVATE_SECRET");
    options.RestBaseUrl = restBaseUrl;
    options.TradingSocketUrl = Environment.GetEnvironmentVariable("TRADOVATE_TRADING_SOCKET_URL")
        ?? "wss://demo.tradovateapi.com/v1/websocket";
    options.MarketDataSocketUrl = Environment.GetEnvironmentVariable("TRADOVATE_MARKET_DATA_SOCKET_URL")
        ?? "wss://md.tradovateapi.com/v1/websocket";
});

using IHost host = builder.Build();
ITradovateApiClient api = host.Services.GetRequiredService<ITradovateApiClient>();
ITradovateWebSocketClient sockets = host.Services.GetRequiredService<ITradovateWebSocketClient>();

Console.WriteLine($"Configured host: {api.ConfiguredHost}");

IReadOnlyList<Account> accounts = await api.GetAccountsAsync();
Console.WriteLine($"Accounts: {accounts.Count}");
foreach (Account account in accounts)
{
    Console.WriteLine($"  {account.Id} {account.Name} active={account.Active}");
}

AuthMe me = await api.GetAuthMeAsync();
Console.WriteLine($"auth/me userId={me.UserId} name={me.Name}");

sockets.QuoteReceived += (_, quote) =>
    Console.WriteLine($"quote {quote.Contract} bid={quote.BidPrice} ask={quote.AskPrice}");
sockets.OrderReceived += (_, order) =>
    Console.WriteLine($"order {order.Id} {order.OrdStatus}");

await sockets.ConnectTradingAsync();
await sockets.ConnectMarketDataAsync();

if (me.UserId is { } userId)
{
    SyncResult sync = await sockets.SyncRequestAsync(new SyncRequest { Users = [userId] });
    Console.WriteLine($"sync positions={sync.Positions.Count} orders={sync.Orders.Count}");
}

Console.WriteLine("Both sockets connected. Press Enter to exit.");
Console.ReadLine();

await sockets.DisposeAsync();
return;

static string Require(string name)
{
    string? value = Environment.GetEnvironmentVariable(name);
    if (string.IsNullOrWhiteSpace(value))
    {
        throw new InvalidOperationException($"{name} is required.");
    }

    return value;
}
