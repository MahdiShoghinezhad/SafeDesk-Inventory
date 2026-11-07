# Engineering decisions — Mahdi Shoghinezhad

This note documents the engineering decisions I made for the SafeDesk Inventory portfolio prototype. My priority was to make the design understandable and reviewable, not to claim exhaustive wallet detection.

## ADR-001 — Inspect installation metadata, not wallet state

**Context.** An application can observe that certain software is installed without knowing anything about a user's accounts. These are fundamentally different data classes.

**Decision.** I rely on standard Windows uninstall records and published Chromium extension package IDs. I never enumerate wallet data files or browser local-storage databases.

**Tradeoff.** Detection is intentionally incomplete: portable applications, registry omissions, stale package folders and modified browsers can produce misses or false positives. For this use case I prefer a transparent false-negative/false-positive discussion to invasive inspection.

## ADR-002 — Require explicit consent and preserve demo/live provenance

**Decision.** The app starts with synthetic examples. A live scan requires a deliberate button click and a separate consent confirmation. The UI clearly switches to `LIVE — THIS PC` only after a completed scan.

**Why.** I want neither automatic scanning nor the possibility of mistaking a polished demo for observed user data.

## ADR-003 — Keep Windows-specific work out of core logic

**Decision.** I separate the WPF view, cross-platform core types/export and Windows adapter into three projects. I verify the cross-platform core with a dependency-free executable test harness.

**Tradeoff.** This is smaller than a typical enterprise test solution and does not yet include fixture-driven Windows adapter integration tests. In a contract project I would add those after agreeing on supported environments and installation variants.

## ADR-004 — Treat inventory values as untrusted input

**Decision.** The CSV exporter quotes fields, doubles quotation marks and neutralizes potential spreadsheet formula entries. I do not interpolate registry values into executable commands, file lookups or network destinations.

**Why.** Local registry display names and version strings are still input from software installers, not trusted constants.

## ADR-005 — Never infer precision that the source lacks

**Decision.** Missing install dates and versions are shown as `Not available`. For browser extensions I do not infer install dates from filesystem timestamps; these are not reliable evidence of installation time. I never infer activation status from a package folder.

## ADR-006 — Make validation reproducible

**Decision.** I provide exact build commands, a synthetic walkthrough, automated core checks and a GitHub Actions Windows build/publish workflow. I keep separate status labels for code written, CI passing and manual Windows acceptance.

**Next validation gate.** A successful Windows CI run followed by manual review on Windows 10/11. Until then, this is a code-complete portfolio *prototype*, not a verified release.

## Production extensions I would scope before starting

1. Define the exact supported browsers, software catalog and Windows variants.
2. Implement testable Windows adapters with injected registry/filesystem abstractions and fixtures.
3. Improve publisher verification and distinguish uninstall entries from confirmed installations.
4. Add accessibility review, signed release packaging and reproducible versioning.
5. Agree on privacy reviews, reviewable milestone deliverables and explicit acceptance tests.

I would **not** add seed phrase, private key, balance or account extraction; those capabilities violate the scope of this inventory application.
