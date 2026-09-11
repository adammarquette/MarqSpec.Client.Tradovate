<!--
  Open against `develop`. A Tradovate PR cannot close a trading-copilot issue. Use a fully-qualified
  `Related to adammarquette/trading-copilot#N`. A local issue uses a PLAIN `Closes #N`.
-->

Related to adammarquette/trading-copilot#

## What changed and why

<!-- The why, not a restatement of the diff. A reviewer reads this to know what question the change answers. -->

## How it was verified

<!-- What you ran, and what it proved. "Tests pass" is not a verification; say which tests and what they cover. -->

- [ ] `dotnet format --verify-no-changes` clean
- [ ] `dotnet build -c Release` clean, warnings-as-errors on `net10.0` only
- [ ] Unit tests green (no network)
- [ ] Demo-host integration: ran with credentials **or** recorded as operator-only (this tree has no fake gateway)

## Checklist

- [ ] **Test-first** — the new test failed before the implementation; a bug fix reproduces the bug first
- [ ] **Docs in lockstep** — the affected section of the PRD or README is updated *in this PR*
- [ ] **No secrets** — nothing logged, nothing tracked, no credential-shaped value in a committed file
- [ ] **Commits** are Conventional and carry both `Assisted-by:` and `Co-Authored-By:` trailers if AI-authored
- [ ] History is curated into units of work (this repo rebase-merges; squash is disabled)

## Order path

- [ ] Nothing non-idempotent became retryable. `placeOrder` / `placeOSO` / `placeOCO` / `liquidatePosition` are still excluded.
- [ ] A rejected command still throws (`failureReason`, or an ambiguous 200 with no `orderId` / `commandId`).
- [ ] Host is still required — no default, no live fallback.

## Public surface

- [ ] No breaking change to the public API — **or** a major version bump, because
      trading-copilot compiles against this assembly directly.
