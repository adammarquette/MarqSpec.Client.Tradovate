# MarqSpec.Client.Tradovate

A .NET client library for the Tradovate REST and WebSocket APIs.

> **Status: scaffolding.** No implementation yet — this repo currently holds the requirements only. Start at
> [`PRD.md`](PRD.md); the layout below is the plan, not the present.

## What this is

A typed, async .NET client for Tradovate, covering account discovery, contract lookup, order placement and
management, position handling, and real-time market/account streaming.

It is the **sibling** of [`MarqSpec.Client.ProjectX`](https://github.com/adammarquette/MarqSpec.Client.ProjectX)
— parallel in shape and convention, deliberately different in signatures. See *Relationship to
`MarqSpec.Client.ProjectX`* in [`PRD.md`](PRD.md) for why the two clients must **not** share a public interface,
and where the venue-neutral symmetry actually lives.

**Tracking issue:** [`adammarquette/trading-copilot#41`](https://github.com/adammarquette/trading-copilot/issues/41)

## Planned layout

| Project | Purpose |
|---|---|
| `MarqSpec.Client.Tradovate/` | The library — REST client, WebSocket client, models, DI helpers |
| `MarqSpec.Client.Tradovate.Tests/` | Unit and integration tests |
| `MarqSpec.Client.Tradovate.Samples/` | Console sample(s) demonstrating REST + real-time usage |

Target frameworks: **.NET 10.0** (primary), **.NET 8.0** (multi-target, for parity with the ProjectX client).

## Key design constraints

Three properties of Tradovate's API drive the design, and each differs from ProjectX:

- **Dual-token auth.** `accessToken` (trading/account) and `mdAccessToken` (market data) are acquired together,
  tracked separately, and renewed ahead of a ~90-minute expiry.
- **Host-selected environment.** Demo and live are different base URLs, not a request flag. The host is
  configured explicitly with no default and no fallback — a "demo" configuration that silently reaches the live
  host risks real money.
- **Two sockets, bespoke framing.** Trading and market data are separate WebSocket connections speaking a frame
  protocol (`o` / `a[…]` / `h` / `c`) with client-driven heartbeats and manual `requestId` correlation — not
  SignalR. All of it is encapsulated; consumers never see a raw frame.

## Development

```bash
dotnet build
dotnet test --filter "Category!=Integration"
```

Integration tests run against the **demo host only** and require credentials supplied via environment:

```bash
dotnet test --filter "Category=Integration"
```

### Coding standards

- Target .NET 10; C# latest, nullable enabled, warnings-as-errors, file-scoped namespaces
- All public async methods accept `CancellationToken`
- `ILogger<T>` for logging — credentials and tokens are never logged
- XML documentation on all public members
- One class, struct, or enum per file
- Queries in fluent/method syntax — never LINQ query-comprehension
- Conventional Commits; `Assisted-by:` trailer on AI-authored changes

### Branching

`develop` → `staging` → `main`. Branch off `develop`; never branch off or PR directly into `main`.
Branch names: `<type>/<work-item-id>_<title>` (`feature` | `bug` | `hotfix`).

## Consuming this library

It is consumed by [`trading-copilot`](https://github.com/adammarquette/trading-copilot) as a git submodule under
`external/`, behind the venue adapter `MarqSpec.TradingCopilot.Integration.Tradovate`. The adapter — not this
library — is what implements the venue-neutral `ITradingVenue` seam (PRD `R-17`).

## License

MIT.
