# Documentation index

> **Authoritative for:** the concept → component → spec map for this repo. Start here; do not re-derive
> context from the file tree alone.

This library is a **Tradovate-native** client. Venue-neutrality lives in trading-copilot's
`TradovateVenue : ITradingVenue` adapter, not here.
[trading-copilot#41](https://github.com/adammarquette/trading-copilot/issues/41).

## Docs

| Doc | Authoritative for |
|---|---|
| [README.md](../README.md) | Quick-start, layout, branching, how to consume the package |
| [PRD.md](../PRD.md) | Product requirements, user stories, safety rules, out of scope |
| [AGENTS.md](../AGENTS.md) | Agent contract, coding standards, test-first, client-side safety rules |
| [CONTRIBUTING.md](../CONTRIBUTING.md) | Branch ladder, Conventional Commits, same-PR docs rule |
| [LICENSE](../LICENSE) | MIT licence |

## ID namespaces

| Prefix | Owner | Meaning |
|---|---|---|
| US-* | [PRD.md](../PRD.md) User Stories | Numbered user stories (1–5) |
| trading-copilot#41 | trading-copilot tracker | The issue that opened this client |

## Concept → component → spec

| Concept | Implemented in | Specified by |
|---|---|---|
| Host-selected environment | `TradovateOptions`, `ITradovateApiClient.ConfiguredHost` | PRD Technical Requirements; AGENTS safety rules |
| Dual-token auth | `AuthenticationService` (`accessToken` + `mdAccessToken`) | PRD US-1; swagger `AccessTokenRequest` / `AccessTokenResponse` |
| REST commands + `failureReason` | `TradovateApiClient`, `CommandResultGuard` | PRD "failed call must never deserialize into a success" |
| Never-retry place/liquidate | `CommandEndpoints`, `TradovateRetryHandler` | AGENTS safety rules; trading-copilot#41 |
| Dual sockets + frames | `ITradovateWebSocketClient`, `FrameProtocol` | PRD US-5; official C# sample frame format |
| Historical bars | `ITradovateWebSocketClient.GetHistoricalBarsAsync` (MD socket) | PRD US-3; no REST bar endpoint |
| Product units | `Product.ValuePerPoint`, `Product.TickSize` | AGENTS "check the units" |
| Absent ≠ zero quotes | `Quote.BidSize` / `AskSize` nullable | AGENTS safety rules |

## Protocol oracle

[tradovate/example-api-csharp-trading](https://github.com/tradovate/example-api-csharp-trading)
`swagger/services.swagger.yaml` — name endpoints and DTOs from it. Do not swagger-codegen into this library.

Current Partner hosts (sample hosts are stale):

- REST demo: `https://demo.tradovateapi.com/v1`
- REST live: `https://live.tradovateapi.com/v1`
- Trading socket: `wss://{demo\|live}.tradovateapi.com/v1/websocket`
- Market-data socket: `wss://md.tradovateapi.com/v1/websocket`
