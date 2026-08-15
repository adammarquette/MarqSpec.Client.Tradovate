# MarqSpec.Client.Tradovate

A typed, async .NET 10 client for the Tradovate REST and frame-based WebSocket APIs.

It is the **sibling** of [`MarqSpec.Client.ProjectX`](https://github.com/adammarquette/MarqSpec.Client.ProjectX)
— parallel in shape and convention, deliberately different in signatures. See *Relationship to
`MarqSpec.Client.ProjectX`* in [`PRD.md`](PRD.md) for why the two clients must **not** share a public interface,
and where the venue-neutral symmetry actually lives.

**Tracking issue:** [`adammarquette/trading-copilot#41`](https://github.com/adammarquette/trading-copilot/issues/41)

Docs index: [`documentation/INDEX.md`](documentation/INDEX.md).

## Quick start (demo host)

```csharp
services.AddTradovateApiClient(options =>
{
    options.Username = builder.Configuration["Tradovate:Username"]!;
    options.Password = builder.Configuration["Tradovate:Password"]!;
    options.AppId = "MyApp";
    options.AppVersion = "0.1.0";
    options.RestBaseUrl = "https://demo.tradovateapi.com/v1";
    options.TradingSocketUrl = "wss://demo.tradovateapi.com/v1/websocket";
    options.MarketDataSocketUrl = "wss://md.tradovateapi.com/v1/websocket";
});

ITradovateApiClient api = services.GetRequiredService<ITradovateApiClient>();
Console.WriteLine(api.ConfiguredHost); // derive practice-vs-live from the host

IReadOnlyList<Account> accounts = await api.GetAccountsAsync();

ITradovateWebSocketClient sockets = services.GetRequiredService<ITradovateWebSocketClient>();
sockets.QuoteReceived += (_, quote) => { /* bid/ask sizes may be null */ };
await sockets.ConnectTradingAsync();
await sockets.ConnectMarketDataAsync();
await sockets.SyncRequestAsync(new SyncRequest { Users = [userId] });
await sockets.SubscribeQuoteAsync("ESM24");
```

Credentials can also come from environment variables (`TRADOVATE_USERNAME`, `TRADOVATE_PASSWORD`,
`TRADOVATE_APP_ID`, `TRADOVATE_APP_VERSION`, `TRADOVATE_DEVICE_ID`, `TRADOVATE_CID`, `TRADOVATE_SECRET`,
plus `TRADOVATE_REST_BASE_URL` / `TRADOVATE_TRADING_SOCKET_URL` / `TRADOVATE_MARKET_DATA_SOCKET_URL`).

**Host is required.** There is no default and no live fallback. A "demo" configuration that silently
reaches the live host risks real money.

Historical bars are retrieved on the market-data socket (`ITradovateWebSocketClient.GetHistoricalBarsAsync`).
Tradovate has no REST bar endpoint.

## Layout

| Project | Purpose |
|---|---|
| `MarqSpec.Client.Tradovate/` | The library — REST client, WebSocket client, models, DI helpers |
| `MarqSpec.Client.Tradovate.Tests/` | Unit and demo-only integration tests |
| `MarqSpec.Client.Tradovate.Samples/` | Console sample: DI against **demo**, account list, both sockets |

Target framework: **.NET 10.0 only**.

## Key design constraints

- **Dual-token auth.** `accessToken` (trading/account) and `mdAccessToken` (market data) are acquired together,
  tracked separately, and renewed ~15 minutes ahead of a ~90-minute expiry.
- **Host-selected environment.** Demo and live are different base URLs, not a request flag.
- **Two sockets, bespoke framing.** Trading and market data are separate WebSocket connections speaking
  `{endpoint}\n{requestId}\n{query}\n{body}` with `o` / `a[…]` / `h` / `c` receive frames. Consumers never see a raw frame.
- **Failure can hide in a 200.** `failureReason` is checked on every command. A rejected place/liquidate throws.
- **Never auto-retry** `placeOrder` / `placeOSO` / `placeOCO` / `liquidatePosition`. Reads, cancel, and modify
  retry on 429 / 5xx / transport. HTTP 429 surfaces as `TradovateRateLimitException`.

## Development

```bash
dotnet build MarqSpec.Client.Tradovate.slnx
dotnet format MarqSpec.Client.Tradovate.slnx --verify-no-changes
dotnet test --filter "Category!=Integration"
```

Integration tests run against the **demo host only** and require credentials supplied via environment:

```bash
dotnet test --filter "Category=Integration"
```

The sample is also demo-only:

```bash
dotnet run --project MarqSpec.Client.Tradovate.Samples
```

### Coding standards

- Target .NET 10; C# latest, nullable enabled, warnings-as-errors, file-scoped namespaces
- All public async methods accept `CancellationToken`
- `ILogger<T>` for logging — credentials and tokens are never logged
- XML documentation on all public members
- One class, struct, or enum per file
- Queries in fluent/method syntax — never LINQ query-comprehension
- Conventional Commits; `Assisted-by:` and `Co-Authored-By:` trailers on AI-authored changes

### Branching

All new work branches off `develop` and PRs back into it — `develop` is the sole integration branch. Promotion
is a one-way ladder with exactly one allowed source per step:

| Target | Allowed source | Exception |
|---|---|---|
| `develop` | any `feature` / `bug` branch | — |
| `staging` | **`develop` only** | allowed with a stated, good reason recorded in the PR |
| `main` | **`staging` only** | **none** |

Never branch off `main`, and never PR into it from anything but `staging`. Branch names:
`<type>/<work-item-id>_<title>` (`feature` | `bug` | `hotfix`). See [`CONTRIBUTING.md`](CONTRIBUTING.md).

## Consuming this library

It is consumed by [`trading-copilot`](https://github.com/adammarquette/trading-copilot) as a git submodule under
`external/`, behind the venue adapter `MarqSpec.TradingCopilot.Integration.Tradovate`. The adapter — not this
library — is what implements the venue-neutral `ITradingVenue` seam (PRD `R-17`).

## License

MIT.
