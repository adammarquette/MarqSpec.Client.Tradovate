# Review checklist — MarqSpec.Client.Tradovate

What to weigh when reviewing a change here. This file is the **substantive** checklist; the
[Code Reviewer contract](../documentation/agents/code-reviewer.md) owns *how* to report, and points here for
*what* to look for. It stays at this path because GitHub's Copilot reviewer reads it.

## Lead with the worst failure this repo can have

> Can this change place an order twice, treat a rejected command as success, or send to the live host when the
> caller configured demo?

This is an **execution** client. A duplicated `placeOrder` / `placeOSO` / `placeOCO` / `liquidatePosition`, a
`failureReason` that deserialized as success, or a silent live fallback is the blast radius — not a leaked
news token. Everything else is downstream of that.

## Order path

- **Never auto-retry** `placeOrder` / `placeOSO` / `placeOCO` / `liquidatePosition`. Reads, cancel, and modify
  retry on 429 / 5xx / transport only. Anything added to a retry set needs a stated reason why resending is safe.
- **Failure can hide in a 200.** `failureReason` is checked on every command. A rejected place/liquidate throws.
  An ambiguous 200 (no `failureReason` and no confirming `orderId` / `commandId`) is also a failure — fail closed.
- Tradovate's order endpoints are `/order/placeOrder` (and OSO/OCO/liquidate). This is **not** ProjectX;
  `POST /api/Order/place` is a different venue and must not appear as if it were this client's contract.

## Host is the environment

- `RestBaseUrl` and both socket URLs are required. No default, no live fallback.
- Practice-vs-live is the host, not an account name and not a request flag.
- A "demo" configuration that silently reaches `live.tradovateapi.com` is a finding.

## Auth and secrets

- Dual tokens (`accessToken`, `mdAccessToken`) never appear in logs, exception messages, or `ToString()` on
  options types.
- REST authorization is `Bearer {accessToken}`. Do not put either token in a query string.
- A tracked `appsettings.json` with a credential-shaped key is a finding regardless of whether the value is a
  placeholder.

## Fail-closed, not fail-open

- A missing host or credential fails at construction / first use with a clear error, not a live default.
- A `catch` that swallows and returns a default is a fail-open. Rate limits surface as
  `TradovateRateLimitException`.
- Absent ≠ zero: a quote with no bid/ask size stays null.

## Money and units

- Prices and sizes on the public surface are **`decimal`**. A `float` or `double` on such a path is a finding.
- Expose product `valuePerPoint` and `tickSize` as Tradovate returns them. Do not assume ProjectX's
  `PointValue = TickValue / TickSize`.
- Timestamps are UTC on the wire.

## Conventions

- Must compile clean on `net10.0` only, with warnings-as-errors. Do not add a second TFM "to match a sibling."
- `CancellationToken` on every public async method, threaded all the way down.
- XML docs on every public member — `GenerateDocumentationFile` is on, so a gap is a build error.
- Fluent LINQ, never query-comprehension syntax.

## Tests

- **Test-first.** A new public method arriving without a test that failed first is a process finding.
- Bug fixes are **regression-first**.
- Unit tests stub the transport and touch no network. `FakeWebSocketTransport` is a test double, not a venue.
- Integration tests are **demo host only** and need `TRADOVATE_*` credentials. This tree has **no fake gateway**.
  A new test that needs the live host is a finding. A skip whose condition can never become false is dead weight
  pretending to be coverage.

## Traceability

Every PR cites its issue with a plain `Closes #N` or a fully-qualified
`Related to adammarquette/trading-copilot#N` — a backticked keyword does not bind. Behavior, API or
configuration changes update the matching PRD section and the README **in the same PR**. Stale documentation is
a finding. This PRD does not define a local `R-#` / ADR catalog; do not invent one in comments.
