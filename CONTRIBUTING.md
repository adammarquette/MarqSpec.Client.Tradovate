# Contributing

How we work in this repo. The **authoritative product requirements** live in [`PRD.md`](PRD.md). Agent
rules live in [`AGENTS.md`](AGENTS.md).

## Branching model

**All new work branches off `develop`** and PRs back into it — `develop` is the sole integration branch.
Changes then promote up a one-way ladder, and **each step has exactly one allowed source**:

| Target | Allowed source | Exception |
|---|---|---|
| `develop` | any `feature` / `bug` branch | — |
| `staging` | **`develop` only** | state the reason in the PR |
| `main` | **`staging` only** | **none** |

Never branch off `main`, and never PR into it from anything but `staging`.

## Branch naming

```
<type>/<work-item-id>_<title>
```

- **`<type>`** — one of **`feature`**, **`bug`**, or **`hotfix`**.
- **`<work-item-id>`** — the tracking GitHub issue number (this library's work traces to
  [trading-copilot#41](https://github.com/adammarquette/trading-copilot/issues/41)).
- **`<title>`** — a short, kebab-case summary.

Example: `feature/41_tradovate-client`.

## Commits

[Conventional Commits](https://www.conventionalcommits.org/). AI-authored changes carry **both** trailers:

```
Assisted-by: Cursor Grok 4.6
Co-Authored-By: Cursor Grok 4.6 <cursor@cursor.com>
```

Commit *type* drives SemVer (`feat` → minor, `fix` → patch, `BREAKING CHANGE` → major).

## Same-PR docs

A PR whose change touches documented behavior updates [`README.md`](README.md) and the affected
[`PRD.md`](PRD.md) section **in the same PR**. Docs drift is a defect, not a follow-up.

## Before you open a PR

```bash
dotnet format MarqSpec.Client.Tradovate.slnx --verify-no-changes
dotnet test --filter "Category!=Integration"
```

PRs target `develop` and cite the tracking issue (`Closes` / `Related to`
[trading-copilot#41](https://github.com/adammarquette/trading-copilot/issues/41)).
