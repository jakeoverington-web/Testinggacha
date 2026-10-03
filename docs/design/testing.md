# Testing method (battle and other Core rules)

Goal: every rule change is checked in seconds, without opening Unity, and the same tests still run in Unity's Test Runner.

## The loop

| Step | Command | Time |
| --- | --- | --- |
| Run everything | `tools/csharp/test.sh` (633 tests) | ~3 s |
| Run one area | `tools/csharp/test.sh Energy` (name filter) | ~1.5 s |
| Quick balance sweep | `tools/csharp/test.sh -Main Gacha.Tests.Tools.BalanceSweep 1000 1` (2,000 random battles) | ~5 s |
| Damage line (30 s vs dummies) | `tools/csharp/test.sh -Main Gacha.Tests.Tools.TrainingReport` | ~3 s |
| Hero probe (real battles) | `tools/csharp/test.sh -Main Gacha.Tests.Tools.HeroProbe nyx caelith` | ~5 s |
| Interaction audit | `tools/csharp/test.sh -Main Gacha.Tests.Tools.InteractionAudit 3000` (pairs, biggest hits, 1 + 4 ceiling) | ~15 s |
| Ablation (value of each kit part) | `tools/csharp/test.sh -Main Gacha.Tests.Tools.Ablation 300 1` | ~2 min |
| What-if variant | `tools/csharp/test.sh -Main Gacha.Tests.Tools.Variant caelith ai=nearest` | ~10 s |
| Synergy report (packages, top teams, round robin) | `tools/csharp/test.sh -Main Gacha.Tests.Tools.SynergyReport 300 32` | ~2-3 min |
| Package probe (why a package wins: uptime, payoff vs base; `null` = fake packages) | `tools/csharp/test.sh -Main Gacha.Tests.Tools.PackageProbe 200` | ~3 min |
| Niche report | `tools/csharp/test.sh -Main Gacha.Tests.Tools.NicheReport 150 1` (234,000 paired battles) | ~3 min |
| Campaign stages (writes Data/stages.json; sim-checks every stage's enemy level) | `tools/csharp/test.sh -Main Gacha.Tests.Tools.StageGen 1 80` (seed, battles per check); after a power-formula change only: `StageGen repower` (keeps levels) | ~9 min / 2 s |
| Campaign gate (reference teams play chapters 1-5 in order) | `tools/csharp/test.sh -Main Gacha.Tests.Tools.CampaignGate 5 15` (chapters, tries per team per stage); report: campaign-gate-v1.md | ~10 s |
| Economy gate (phase 2: idle hours to level 200 vs 700 h) | `tools/csharp/test.sh -Main Gacha.Tests.Tools.EconomySim` (args: target, xpMult, clearMult what-ifs); report: economy-gate-v1.md | ~2 s |
| Final check | Unity → Window → General → Test Runner → EditMode → Run All (verified 2026-10-03: 633 passed in 2.7 s on Unity 6000.6.4f1, golden replay hash identical) | on a PC |
| Final check from a terminal (Editor open, `com.unity.pipeline` installed) | `unity command run_tests --mode editor --timeout 300 --result-only` (verified 2026-10-03: 633 passed, CLI 1.0.0-beta.12, Pipeline 0.8.0-exp.1) | on the owner's PC |

Setup on a new machine: install PowerShell 7 (any OS; on a fresh cloud workspace, unpack Microsoft's official `powershell-7.x-linux-x64.tar.gz` release into `/opt/pwsh`). Nothing else: no .NET SDK, no NuGet.

`test.sh` compiles `Scripts/Core` + `Tests` with the C# 9 compiler bundled in PowerShell 7 (same language version as Unity) and runs the tests in memory. It needs `pwsh` (set `PWSH=` if it is not on PATH). Tests may only use the NUnit subset in `tools/csharp/NUnitShim.cs`; add to the shim (with the real NUnit signature) rather than to a test.

## Five kinds of test, cheapest first

| Kind | What it proves | How it is written | Example |
| --- | --- | --- | --- |
| 1. Rule tests | One formula or rule gives the exact number | Hand-written, tiny lab fight, assert on numbers | DEF 300 halves damage; counter race deals +10% |
| 2. Kit tests | Every ability of all 60 heroes runs and does what its tags say | **Generated from heroes.json** (TestCaseSource over hero ids): no per-hero code | Halcyra's Static Mark applies Soaked; every ultimate fires within 60 s |
| 2b. Training ground | Numbers match the text | Every hero vs neutral dummies (DEF 0, no race, crit and passives off): auto attacks = 100% ATK at her attack speed; every % ATK, % max HP, seconds, % and energy in each ability's text shows up in what it did; passive numbers match their text and bonuses apply at their value | Vesper's "heal the team for 60%" was split across the team; fixed to each ally |
| 3. Invariant tests | Nothing impossible ever happens | Hundreds of seeded random 5v5 battles, checks after every tick | HP in [0, max]; energy in [0, 100]; shield ≤ 50% max HP; control ≤ 2.5 s; battle ends by the time limit |
| 4. Golden replays | A rule change didn't silently change outcomes | Fixed teams + seed; event-log hash stored in the test | Changing a formula fails the hash on purpose; update the hash in the same commit |
| 5. Niche report | Every hero is the best pick somewhere and none everywhere (a report, not pass/fail) | Paired battles: the team with her vs the same team with a random same-role replacement, in 13 situations (vs each race, vs tanky/sustain/burst/control, with a synergy partner, with a full package of partners, own-race team) | Flags DOMINANT and NO NICHE heroes; role yardsticks per role |

## Rules that keep it fast

- **Assert on the event log, not on internals.** The battle emits structured events (cast, damage, heal, shield, status on/off, energy, death). Tests query the log, so they survive refactors.
- **Lab builder, not setup code.** `Lab.Seed(1).Ally("halcyra").Enemy(Lab.Dummy).Run(10)` builds a fight in one line. Dummies have fixed stats and never act unless asked.
- **Seeds are fixed.** Every random roll goes through `Gacha.Core.Rng`; a failing seed is a reproducible bug.
- **Kits are data, so kit tests are data.** Adding hero 61 adds its tests automatically. A new op or status must be added to the validator, or the data test fails.
- **One failing test first, then the code** for each new rule (TDD); run the filter for that area while working, everything before committing.
- **Whole suite under 10 s.** If it grows past that, shrink random-battle counts in invariant tests (the sweep is where volume belongs).
