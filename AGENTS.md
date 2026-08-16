# AGENTS.md — MarqSpec.Client.Tradovate (root)

Instructions for AI coding agents working in this repository. **Nested `AGENTS.md` files take precedence for
their subtree.** This root file holds the rules that apply everywhere.

## What this repo is

A **Tradovate-native** .NET client (`MarqSpec.Client.Tradovate`) — a faithful wrapper over Tradovate's REST and
frame-based WebSocket APIs. It is a sibling of `MarqSpec.Client.ProjectX` in **convention only** (DI, Options,
`ILogger<T>`, `CancellationToken`, Refit, XML docs, test stack). It must **not** implement `IProjectXApiClient`
or copy that interface's wire vocabulary.

Venue-neutrality is the **adapter's** job in [trading-copilot](https://github.com/adammarquette/trading-copilot)
(`TradovateVenue : ITradingVenue`). Tracking issue:
[trading-copilot#41](https://github.com/adammarquette/trading-copilot/issues/41).

## Source of truth (read before coding)

**Start at [`README.md`](README.md), then [`documentation/INDEX.md`](documentation/INDEX.md) — the wiki's front
door.** [`PRD.md`](PRD.md) owns product requirements. The official protocol oracle is
[tradovate/example-api-csharp-trading](https://github.com/tradovate/example-api-csharp-trading)
(`swagger/services.swagger.yaml`) — do **not** swagger-codegen it into this library.

## Universal rules (all agents, everywhere)

- **Every capability traces to a real user need / use case.** If it doesn't, don't build it.
- **The front-door index stays live.** Adding or renaming a doc, component, ID, or use case → update its
  `INDEX.md` row in the same change.
- **Contracts are the source of truth.** External calls conform to Tradovate's API, not ProjectX's.
- **Comments are terse.** Inline comments are one short line, only for non-obvious *why*. A comment that
  points at a doc or issue must be prefixed `reference:`.
- **No secrets in source.** Credentials and both tokens never appear in logs or exception messages.
- **Synthetic/demo data only — never real secrets or PII** anywhere: code, tests, fixtures, logs, telemetry.
- **Structured logging** via `ILogger` message templates, never interpolated log strings.
- **Commits:** Conventional Commits; add both an `Assisted-by:` and a `Co-Authored-By:` trailer when authored
  by an AI agent.
- **No orphaned PRs.** Every PR references a tracking issue (`Closes #N` / `Related to #N`). This repo's
  work traces to [trading-copilot#41](https://github.com/adammarquette/trading-copilot/issues/41).

## Safety rules (client-side, from #41)

- **Host is the environment.** Required `RestBaseUrl` / socket URLs, no default, no live fallback. Surface the
  configured host. Never infer practice from an account name.
- **Failure can hide in a 200.** Check `failureReason` on every command. A rejected place/liquidate must throw.
  An ambiguous 200 (no `failureReason` and no confirming `orderId` / `commandId`) is also a failure — fail closed.
- **Absent ≠ zero.** If a quote has no bid/ask size, leave it null.
- **Check the units.** Expose product `valuePerPoint` and `tickSize` as Tradovate returns them. Do not assume
  ProjectX's `PointValue = TickValue / TickSize`.
- **Never auto-retry** `placeOrder` / `placeOSO` / `placeOCO` / `liquidatePosition`. Retry reads, cancel, and
  modify on 429 / 5xx / transport only. Surface 429 distinctly.

## Standards

**.NET 10 (LTS), C# latest, net10.0 only.** File-scoped namespaces (build **error** if violated), nullable on,
warnings-as-errors, `sealed` by default, one public type per file, immutability (`record` / `required` /
`init`), `decimal` for prices/sizes on the public surface, `DateTime.UtcNow` never `DateTime.Now`, constructor
DI only, async all the way with `CancellationToken`, fluent LINQ only, exhaustive `switch` on enums,
`System.Text.Json` (no Newtonsoft).

## Test-first (mandatory)

- Write the **failing unit test before** the implementation. No new public method without a failing test first;
  bug fixes are regression-first.
- Name: `MethodUnderTest_Should{ExpectedBehavior}_When{condition}`.
- Every test guards a named failure mode. FakeItEasy, no I/O / network in unit tests.
- Integration tests `Category=Integration`, **demo host only**, credentials from env.

## Build / test

- Build: `dotnet build MarqSpec.Client.Tradovate.slnx`
- Unit tests: `dotnet test --filter "Category!=Integration"`
- Before opening a PR: `dotnet format --verify-no-changes` + unit tests green.

## Branching

All new work branches off `develop` and PRs back into it. See [`CONTRIBUTING.md`](CONTRIBUTING.md).
