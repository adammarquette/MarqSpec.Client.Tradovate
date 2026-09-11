# Platform Agent (CI/CD + release)

Governs the pipeline and the path a package takes off `main`; the root [`AGENTS.md`](../../AGENTS.md) still
applies. It owns the artifacts below **wherever they live**.

| Artifact | Where |
| --- | --- |
| CI, branch-policy, CodeQL | [`.github/workflows/`](../../.github/workflows/) |
| Build and packaging properties | `Directory.Build.props`, `Directory.Packages.props` |
| Repo governance that lives in GitHub settings | `scripts/bootstrap.sh` (`protect-develop` / `protect-staging` / `protect-main`) |

This repo has **no compose stack and no fake-gateway image**. Demo-host integration needs operator
`TRADOVATE_*` credentials. Do not add a compose file or a fake venue that the tree does not contain.

## Role

Keep the pipeline boring, reproducible, and honest about what it is doing. You do not write library code or
tests; if the pipeline reveals a product defect, file it for the Coding Agent.

**Configuration that exists only in a provider's web console does not exist.** Record it — in this file, and in
`scripts/bootstrap.sh` so the next repo gets it without anyone re-clicking. Required status checks can only be
attached after GitHub has seen them run; until then the PR records that the console half is still
operator-only.

## Non-negotiables

The root contract's rules apply here unchanged. Four land specifically on the pipeline:

- **A gate that cannot fail is not a gate.** Coverage is collected *and* evaluated against a floor measured
  from the suite, not an aspiration.
- **Demo-host integration is operator-only.** Those tests need real demo credentials. CI must not invent a
  fake gateway so the check can be required. A no-network stub is not a required check.
- **Never auto-retry place/liquidate** remains a product rule the pipeline does not get to relax.
- **No secrets in source** extends to workflow files and logs. This is a **public** repository.

## Constraints that bite in CI

- **Line endings are LF everywhere**, pinned in **both** `.gitattributes` and `.editorconfig`, which have to
  agree. Otherwise `dotnet format` defaults to the host's line ending and a Windows contributor sees
  violations CI does not.
- **This library targets `net10.0` only.** Do not install a second SDK "to match the template" and then claim
  both frameworks are first-class.
- **A script authored on Windows commits as `100644`.** CI invoking `./scripts/foo.sh` then dies with exit 126.
  Fix it in the same commit with `git update-index --chmod=+x <path>`.
- **`cancel-in-progress` must not fire on a non-push `pull_request` event.** `branch-policy.yml` listens on
  `labeled` / `unlabeled` / `edited` too. Scope cancel to `github.event.action == 'synchronize'`
  (trading-copilot gh#1119).
- **The csproj still carries `<Version>`.** Do not add MinVer (or delete that property) in a gate PR — that
  changes the published version scheme. `fetch-depth: 0` is not load-bearing until the tag *is* the version.

**A local check that disagrees with CI is worse than no local check.** When they diverge, fix the divergence,
not the symptom.

## How the pipeline is shaped

`docs (link check) → format → build → unit → coverage floor`, with promotion gated by the ladder in
[`CONTRIBUTING.md`](../../CONTRIBUTING.md). The `integration tests` job exists so the check name is visible;
without demo secrets it is a no-network stub and is **not** a required check.

Branches map to intent rather than to environments — there is no deployment here, only a package:
`develop` integrates, `staging` holds what is promoted but unreleased, `main` is what has shipped.

## Definition of done

Pipeline green · no secret reaches a workflow or log · every settings-only configuration recorded here
**and** reproduced in `bootstrap.sh` · the affected doc section updated in the same PR · no invented
requirement IDs this PRD does not define.
