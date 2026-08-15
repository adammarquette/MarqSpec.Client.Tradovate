Marquette Specifications

# Tradovate API Client

## Executive Summary
The Tradovate API Client is a .NET library providing integration with the Tradovate REST and WebSocket APIs.
It gives developers a typed, async interface for account discovery, contract lookup, order placement and
management, position handling, and real-time market/account streaming.

**Target Audience**: .NET developers building trading applications against Tradovate-hosted accounts —
including prop-firm accounts (Apex, Take Profit Trader, TradeDay, …) that run on the Tradovate platform.

**Key Success Criteria**:
- Enable developers to integrate with the Tradovate API in under 30 minutes
- Provide production-ready reliability with auto-reconnection, dual-token refresh, and retry logic
- Deliver real-time market data with minimal latency over Tradovate's frame-based socket protocol

## Relationship to `MarqSpec.Client.ProjectX` — read this first
This library is a **sibling** of `MarqSpec.Client.ProjectX`, **not a reimplementation of its interface.**

- It is **parallel in shape and convention**: DI extension method, Options pattern, `ILogger<T>`,
  `CancellationToken` on every async method, Refit-based REST, XML docs on all public members, same test stack.
- It is **deliberately different in signatures**: it speaks *Tradovate's* vocabulary (`accountId` as `int`,
  `contractId`, `symbol`, OSO/OCO brackets, `liquidatePosition`), because Tradovate's auth, transport, symbology,
  and environment model genuinely differ from ProjectX's.

Forcing this client to implement `IProjectXApiClient` would bake ProjectX's transport assumptions — SignalR
hubs, single JWT, a per-account `simulated` flag — into a platform that shares none of them. That is the
precise leak the consuming platform's venue abstraction (PRD `R-17`) exists to prevent.

**Venue-neutrality is the consumer's job, not this library's.** In `trading-copilot`, the adapter project
`MarqSpec.TradingCopilot.Integration.Tradovate` translates this client into the venue-neutral `ITradingVenue`
seam. *That* is where the interface is identical to ProjectX's. See `trading-copilot` gh#41.

## User Stories
1. As a developer, I want to authenticate with the Tradovate API using my credentials, configurable via
   environment variables or a configuration file.
   - **Acceptance Criteria:**
     - Credentials configurable via environment variables (`TRADOVATE_USERNAME`, `TRADOVATE_PASSWORD`,
       `TRADOVATE_APP_ID`, `TRADOVATE_APP_VERSION`, `TRADOVATE_DEVICE_ID`, `TRADOVATE_CID`, `TRADOVATE_SECRET`)
     - Credentials configurable via `appsettings.json` through the Options pattern
     - Both `accessToken` and `mdAccessToken` are acquired and tracked independently
     - Tokens auto-renew ahead of the 90-minute expiry without interrupting in-flight calls
     - Authentication failures return clear errors; credentials never appear in logs or exception messages

2. As a developer, I want to discover the accounts behind my login and read their state.
   - **Acceptance Criteria:**
     - Methods to list accounts, users, and cash balances
     - Practice-vs-live is determined by the **host** the client is configured against, and surfaced explicitly
     - Responses deserialize into typed C# models

3. As a developer, I want to look up contracts and retrieve historical price data.
   - **Acceptance Criteria:**
     - Methods to find contracts by symbol and by ID, and to resolve contract maturity/product metadata
     - Historical bar retrieval with configurable interval and time range
     - Errors handled gracefully with meaningful messages

4. As a developer, I want to place and manage orders, including bracket orders.
   - **Acceptance Criteria:**
     - Methods for `placeOrder`, `placeOSO`, `placeOCO`, `modifyOrder`, `cancelOrder`
     - `liquidatePosition` is supported as a first-class operation (the primitive auto-flatten depends on)
     - Order and position queries return typed models
     - Rate-limit responses (`429`) are surfaced distinctly from other failures

5. As a developer, I want to stream real-time market data and account updates over WebSocket.
   - **Acceptance Criteria:**
     - The client manages **two sockets** — trading/account and market data — independently
     - Frame protocol (`o` open, `a[…]` payload, `h` heartbeat, `c` close) is fully encapsulated; consumers
       never see raw frames
     - Request/response correlation via `requestId` is handled internally
     - Client heartbeats are emitted on the required interval; a stalled server connection is detected and
       reconnected automatically
     - Real-time updates surface as typed events (Observer pattern), mirroring the ProjectX client's ergonomics

## Technical Requirements
- Implemented in C#, targeting **.NET 10 only** (net10.0; no net8 multi-target — matches trading-copilot and Finnhub)
- Asynchronous throughout; all public async methods accept `CancellationToken`
- Configuration via the Options pattern, supporting environment variables and configuration files
- **Dual-token lifecycle** is a first-class concern: `accessToken` (trading/account) and `mdAccessToken`
  (market data) are acquired together, tracked separately, and renewed before expiry
- **Environment is host-selected**: demo and live are different base URLs, not a request flag. The host must be
  configured explicitly with **no default and no fallback**, and surfaced on the client for assertion.
  Consumers derive practice-vs-live from it, so an ambiguous host is a safety defect rather than a convenience:
  a "demo" configuration that silently reaches the live host risks real money (R-14)
- Thread-safe; supports concurrent API calls
- **A failed call must never deserialize into a success.** ProjectX reports failure *in the payload*
  (`success=false` on an otherwise healthy `200`). Confirm whether Tradovate does the same and, if so, check it
  on every call and throw. A rejected order that looks placed is the highest-severity bug this library can ship.
- Rate limiting handled with automatic retry and exponential backoff (Polly), respecting Tradovate's rolling
  request budget
- The WebSocket layer must encapsulate the frame protocol, heartbeat cadence, `requestId` correlation, and
  reconnection — none of which should be visible to consumers
- Failed WebSocket messages queued and retried, or reported to observers
- Comprehensive unit tests; XML documentation on all public members
- All classes, structs, and enums in separate files
- File-scoped namespaces (build error if violated); `sealed` by default; `decimal` for prices/sizes on the public surface
- `dotnet format --verify-no-changes` in CI — style drift fails the build

## Technology Stack and Dependencies
- C# with .NET 10 (net10.0 only)
- Refit (minimum 7.0.0) — REST
- `System.Net.WebSockets.ClientWebSocket` — Tradovate uses a bespoke frame protocol over raw WebSocket, **not**
  SignalR; there is no SignalR dependency in this client
- Polly (minimum 8.6.5) — transient faults, retries, rate-limit backoff
- `Microsoft.Extensions.Options` — configuration
- `Microsoft.Extensions.Logging.Abstractions` — `ILogger<T>` throughout
- xUnit (minimum 2.9.0), FakeItEasy (minimum 8.0.0), FluentAssertions (`[6.12.0,8.0.0)` — license cap)

## External Interfaces
- **REST API**
  - Demo base URL: `https://demo.tradovateapi.com/v1`
  - Live base URL: `https://live.tradovateapi.com/v1`
  - Authentication: `POST /auth/accessTokenRequest` → `{ accessToken, mdAccessToken, expirationTime }`
  - Token renewal: `/auth/renewAccessToken`; tokens live ~90 minutes, renew ~15 minutes before expiry
  - Authorization: `Bearer {accessToken}` header
  - Rate limit: ~5,000 requests per rolling 60-minute window; exceeding returns `429`
  - Endpoint groups: `/auth`, `/account`, `/user`, `/cashBalance`, `/contract`, `/product`, `/order`,
    `/position`, `/fill`, `/md`
  - Order operations: `/order/placeOrder`, `/order/placeOSO`, `/order/placeOCO`, `/order/modifyOrder`,
    `/order/cancelOrder`, `/order/liquidatePosition`
- **WebSocket — trading/account**: `wss://{demo|live}.tradovateapi.com/v1/websocket`
- **WebSocket — market data**: `wss://md.tradovateapi.com/v1/websocket` (authenticated with `mdAccessToken`)
  - Protocol: bespoke frame format — `o` (open), `a[…]` (JSON array payload), `h` (heartbeat), `c` (close)
  - Message format: `endpoint\nrequestId\n\nbody`
  - Handshake: on `o`, send `authorize\n{requestId}\n\n{accessToken}`
  - Client heartbeat: empty frame `[]` on a ~2.5s cadence; server silence beyond ~10s indicates a dead
    connection and must trigger reconnection

## Security Requirements
- Credentials and both tokens must never be logged or included in exception messages
- HTTPS enforced for all REST calls; WSS for all socket connections
- SSL certificates validated by default
- Sensitive configuration supports encryption at rest
- **Environment safety**: the configured host is surfaced on the client so a consumer can assert demo-vs-live
  before transmitting an order. The client must never infer or silently fall back to the live host.

## Non-Functional Requirements
- **Compatibility**: .NET 10 (net10.0 only)
- **Deployment**: NuGet package with symbol packages
- **Backward Compatibility**: SemVer 2.0
- **Memory Efficiency**: no leaks across long-running dual-socket operation
- **Graceful Degradation**: partial service outages handled; market-data socket loss must not take down the
  trading socket, and vice versa

## Metrics

### Development Metrics
- **Unit Test Coverage**: minimum 95% line and 90% branch on all public methods
- **Integration Test Coverage**: at least one integration test per public method against the **real API**
  (nothing mocked), configurable per environment. **The demo host only — never the live host**, in CI or
  locally. This library's test suite has no reason to reach a real-money venue; exercising the live host is a
  production concern for the consuming platform, under its own practice-vs-live policy
- **Code Quality**: SOLID; analyzer warnings at zero
- **Documentation Alignment**: XML comments match the published API documentation

### Performance Metrics
- **REST Latency**: p95 < 500ms, p99 < 1000ms under normal network conditions
- **WebSocket Throughput**: 1,000 events/second per stream without message loss
- **WebSocket Latency**: p99 < 100ms from server send to client callback invocation
- **Memory Stability**: no leaks during 24-hour continuous dual-socket operation

### Reliability Metrics
- **Auto-Reconnection**: reconnect within 5 seconds of disconnection, per socket, with re-authorization and
  subscription replay
- **Token Renewal**: zero auth-expiry-induced request failures during continuous 24-hour operation
- **API Success Rate**: >99.9% under normal conditions (excluding 4xx client errors)
- **Retry Resilience**: transient failures (5xx, network timeouts) retried up to 3 times

## Example Usage
See the README quick-start and `MarqSpec.Client.Tradovate.Samples`. The sample:

1. Registers DI against the **demo** host (`https://demo.tradovateapi.com/v1`) with no live fallback
2. Lists accounts and calls `GET /auth/me`
3. Connects both sockets, runs `user/syncrequest`, and listens for quotes / orders

Historical bars are retrieved via `ITradovateWebSocketClient.GetHistoricalBarsAsync` (`md/getChart`). There is no REST bar endpoint.

## Out of Scope
- UI components or visualization tools
- Backtesting or historical analysis
- Order execution algorithms or trading strategies
- Venue-neutral abstraction — that lives in the consumer's adapter, not here (see "Relationship to
  `MarqSpec.Client.ProjectX`")
- Risk enforcement of any kind — this library transmits; it does not decide

## Open questions
- **`Decide:` Partner API vs standard API.** Two distinct *documentation* surfaces exist, and neither is an API
  host — don't conflate them with the endpoints in *External Interfaces* above:
  - `api.tradovate.com` — docs for the **standard** API, which is served from `{demo|live}.tradovateapi.com/v1`
  - `partner.tradovate.com` — docs for a separate **Partner** API

  The frame-protocol detail above came from the Partner docs, which serve staging URLs under a NinjaTrader
  domain, so it may not describe the standard surface at all. Confirm which surface prop-firm accounts (Apex,
  TPT, TradeDay) actually authenticate against — and which hosts that implies — before fixing endpoints.
- **`Decide:` OSO/OCO semantics.** `placeOSO` and `placeOCO` are confirmed to exist but their exact request
  shape and bracket semantics need verification against live docs before the `BracketOrders` capability is
  claimed.
- **`Decide:` market-data entitlement.** Whether `mdAccessToken` grants real-time data on all prop-firm account
  types, or whether some require a separate market-data subscription.
- **`Decide:` rate-limit granularity.** The ~5,000/hour figure is widely reported but may vary by account type;
  confirm and encode the actual budget in the Polly policy.
