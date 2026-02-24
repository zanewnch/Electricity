# Documentation Guide

This guide explains the documentation practices used in this project and why they matter.

## Overview

This project uses industry-standard documentation formats practiced by professional engineering teams:

- **RFC** (Request for Comments) — Propose changes **before** implementation
- **ADR** (Architecture Decision Records) — Record decisions **after** they're made
- **C4 Model** — Visualize system architecture at multiple levels
- **OpenAPI** — Define API contracts in machine-readable format

The goal is to practice the same habits used in professional engineering teams, where documentation is treated as a first-class deliverable alongside code.

## Documentation Types

| Type | Purpose | When | Detailed Guide |
|------|---------|------|----------------|
| **RFC** | Propose and review engineering changes | Before implementation | [rfc/README.md](rfc/README.md) |
| **ADR** | Record architectural decisions | After decision is made | [adr/README.md](adr/README.md) |
| **Architecture** | Visualize system structure (C4 model) | During design | [architecture/README.md](architecture/README.md) |
| **OpenAPI** | Formal API contract | Before/during API development | [api/openapi.yaml](api/openapi.yaml) |

## Industry Adoption

These formats are widely used across the industry:

- **RFC:** Google, Meta, Rust, Sourcegraph
- **ADR:** Netflix, Spotify, ThoughtWorks
- **C4 Model:** Enterprise teams worldwide
- **OpenAPI:** Stripe, Twilio, most major API providers (Linux Foundation standard)

## Quick Reference

### RFC vs ADR

| | RFC | ADR |
|---|---|---|
| **When** | Before decision | After decision |
| **Goal** | Get feedback | Record history |
| **Question** | "Should we?" | "Why did we?" |

An RFC often **becomes** an ADR once accepted — the ADR is the final record of what the RFC concluded.

## Getting Started

1. **Proposing a feature?** → Write an [RFC](rfc/README.md)
2. **Made a decision?** → Document it in an [ADR](adr/README.md)
3. **Designing system structure?** → Create [C4 diagrams](architecture/README.md)
4. **Building an API?** → Define the contract in [OpenAPI](api/openapi.yaml)

## Why Document?

Documentation serves three critical purposes:

1. **Communication** — Share ideas and get feedback before writing code
2. **Decision History** — Understand why choices were made, not just what was built
3. **Onboarding** — Help future contributors (including future you) understand the system

> "Code tells you how. Documentation tells you why."
