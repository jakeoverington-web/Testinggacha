# Phase 2: Hero Progression Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Owned heroes with Contract-slot levels and sync, 10 star ranks with fodder and Sigils, free resets, gear (4 slots, 13 types, 4 rarities, 5 sets, random drops), ability ranks in battle, the Heroes screens and a dev panel, checked by the phase 2 economy gate.

**Architecture:** All rules in Core (Progression, Gear, Campaign) with Edit Mode tests; battle gains per-hero stat bonuses and ability ranks (defaults keep the golden hash); save v2 holds the collection, slots and gear. UI adds a tab bar, Heroes tab, hero detail and a dev-only panel.

**Tech Stack:** C# 9 Core + NUnit runner, Unity 6.6 UI Toolkit, Unity CLI, Python roster tools.

**Spec:** `docs/superpowers/specs/2026-10-03-phase-2-hero-progression-design.md`

## Global Constraints

- Hard rules 1-8. Seeded RNG for the starter roll and every drop. No pop-ups; dev panel only in `UNITY_EDITOR || DEVELOPMENT_BUILD`.
- Golden hash unchanged when no gear, no set and all ability ranks are 1; any intended change gets a new hash comment (CLAUDE.md gotcha).
- Numbers from the spec: level caps 60,70,80,90,100,120,140,160,180,200; star costs table; gear main stat 10/20/30/40% (Boots 10/20/30/40 haste) x (1 + 5% x upgrade), upgrades +0..+20 at 1,000 x rank x level gold; rarities unlock after the ch 5/10/15 bosses; drop odds 50/30/15/5 renormalised; idle gear 1/2/3/4 at 50/35/12/3% x hours/24; chapter-boss first clear = 1 piece; set and type values from the spec tables.
- Level costs: progression.json breakpoints from the plan doc (XP and gold per level at 20, 40, 60, 80, 100, 130, 160, 200; Starlight at 20..180), log-linear between breakpoints.

## Review Focus

1. Swapping a hero into a slot whose level is above her star cap → she shows the cap; the slot keeps its level for a later hero. Task 2.
2. Feeding a hero that is in a Contract slot or equipped with gear → refused, not silently lost. Task 3.
3. Reset after spending Sigils and spare heroes → refund returns copies, gold and fodder as Sigils; total value never grows. Task 3.
4. Phase 1 save loaded → migrates to a seeded starter team without crashing; campaign progress kept. Task 6.
5. Gear equipped on two heroes at once → impossible (an item has one owner). Task 5.

---

### Task 1: Data — progression costs, star table, gear catalogue, maxStars 10

Files: `Data/progression.json` (add `levelCaps`, `levelCost` breakpoints, `starlight` breakpoints, `starCosts` rows), create `Data/gear.json` (slots, types with extra stats, rarities with main %, sets with 2/4-piece bonuses, unlock chapters, odds, idle table, upgrade cost), `tools/roster/build.py` (`maxStars` 10) then `python build.py`; Core loaders `Progression/LevelCosts.cs`, `Progression/StarCosts.cs`, `Gear/GearCatalog.cs`; tests `Tests/ProgressionDataTests.cs`.
- [ ] Failing tests: caps per star; XP and gold at each breakpoint equal the plan table; cumulative XP to 200 within 1% of 14M; star rows total 8 copies, 15 fodder, 4.1M gold; gear catalogue has 4 slots, 13 types, 4 rarities, 5 sets; heroes.json maxStars = 10 for all 60.
- [ ] Implement; `python tools/roster/check.py` clean and heroes.json diff only maxStars; full suite green. Commit.

### Task 2: Collection, Contract slots, levels and sync (Core)

Files: `Progression/Collection.cs` (owned hero: id, stars, copies; Sigils per race; Contract slots: 5 x {hero or "", level}); tests `Tests/CollectionTests.cs`.
- Produces: `Collection.Owns/Add/Copies`; `int Level(string hero)` (slot hero: min(slot level, cap); others: lowest slot level); `bool LevelUp(int slot, Wallet)` spends XP/gold/Starlight, refuses at the hero's cap or when short; `void Contract(int slot, string hero)` (free, no cooldown); `ResetSlot(int slot, Wallet)` full refund.
- [ ] Failing tests incl. Review Focus 1, sync to lowest slot, refusing past cap, refund equals spend. Implement; green; commit.

### Task 3: Stars, fodder, Sigils, resets (Core)

Files: `Progression/Stars.cs`; tests `Tests/StarTests.cs`.
- Produces: `StarUpResult Stars.CanRaise/Raise(Collection, hero, List<string> fodderHeroes, int sigils, Wallet)`; `Stars.Reset(Collection, hero, Wallet)` (refund copies, gold, fodder as Sigils of the hero's race).
- [ ] Failing tests: each rank's cost; wrong-race fodder refused; starred fodder refused; Contract or geared fodder refused (Review 2); Sigils count as fodder; reset refunds everything, value never grows (Review 3). Implement; green; commit.

### Task 4: Battle — stat bonuses, ability ranks, two new set effects

Files: `Battle/Battle.cs` (AddTeam), `Battle/Battle.Ops.cs`, `Battle/Battle.Combat.cs`, `Battle/Unit.cs`; create `Battle/StatBonus.cs`; tests `Tests/BonusAndRankTests.cs`.
- Produces: `StatBonus` (HpPct, AtkPct, DefPct, Haste, AtkSpdPct, MoveSpdPct, CritRate, CritDmg, Accuracy, Dodge, Block, EffectHit, EffectRes, Lifesteal, HealPower, HealRecv, EnergyRegen, ShieldRecv, StartEnergy); `TeamSetup.Bonuses` and `TeamSetup.Ranks` (per hero: ult, s1, s2, passive rank 1-3) — null = none. Rank 2/3 multiply ability magnitudes (damage and heal amounts, shields, status `v`) by 1.10 / 1.20, never durations or counts. ShieldRecv multiplies shields received; StartEnergy sets energy at battle start.
- [ ] Failing tests: each bonus lands on the unit; rank 2 deals 10% more on a fixed hit; durations unchanged; StartEnergy; ShieldRecv; golden hash unchanged with null bonuses/ranks. Implement; green; commit.

### Task 5: Gear items, sets, upgrades, Equip Best, drops (Core)

Files: `Gear/GearItem.cs`, `Gear/Inventory.cs`, `Gear/GearDrops.cs`; tests `Tests/GearTests.cs`.
- Produces: `GearItem { int Id; string Slot, Type, Set; int Rarity; int Upgrade; string EquippedOn; }`; `StatBonus Gear.BonusFor(hero, Inventory, Catalog)` (main + extra + 2/4-piece sets); `Upgrade(item, Wallet)`; `Salvage(item, Wallet)`; `EquipBest(hero)`; `Equip(item, hero)` moves it (Review 5); `GearDrops.Roll(Rng, int unlockedRarities)`; `GearDrops.IdleCount(Rng, double hours)`; rarity unlock from highest cleared chapter boss.
- [ ] Failing tests: main stat by rarity and upgrade; extra stat scaling; 2- and 4-piece sets; upgrade cost; renormalised odds (seeded counts within tolerance); idle count distribution and hours scaling; Equip Best; one owner per item. Implement; green; commit.

### Task 6: Campaign integration, starter team, save v2

Files: `Campaign/CampaignState.cs`, `Campaign/SaveGame.cs`, `Campaign/QuickDeploy.cs` (owned only); tests update `CampaignTests`, `StagesDataTests`, new `Tests/StarterTests.cs`.
- PlayerSetup uses real levels, stars (Scales), gear (Bonuses) and ranks; Fight refuses heroes not owned. Starter roll: 5 unique from seed, exactly 1 tank and 1 healer. Chapter-boss first clear drops 1 piece; idle collect rolls gear. Save version 2 (collection, slots, Sigils, inventory, drop counter); v1 saves migrate (keep progress, roll a starter team) (Review 4).
- [ ] Failing tests; implement; green; golden hash note if needed; commit.

### Task 7: Economy gate (phase 2 gate)

Files: `Tests/Tools/EconomySim.cs`, report `docs/design/economy-gate-v1.md`.
- Simulates a player collecting idle loot daily, clearing stages at the expected power curve and levelling the five slots; reports idle hours to level 200 vs the 700 h target (pass within 20%) and the level reached by stage vs the campaign's expected levels. If it fails, report to the owner before changing tuning. Re-run CampaignGate with a starter-team rule only if stage levels change.
- [ ] Implement; run; save report; commit.

### Task 8: Screens — tab bar, Heroes, hero detail, gear, dev panel

Files: `UI/Heroes/*`, `UI/HeroDetail/*`, `UI/Dev/*`, `Scripts/UI/HeroesController.cs`, `HeroDetailController.cs`, `DevPanelController.cs`, `UiRoot.cs`/`ScreenRouter.cs` (tab bar), `Editor/SceneBuilder.cs` (new assets).
- Heroes: 5 Contract slots with level and power on top; collection grid with race tabs and count. Detail: 1:2 placeholder, stars (gold/diamond, border at 10), stats, 4 gear slots, skills slide-out with ranks; Level Up, Stars, Gear (pick / upgrade / Equip Best / salvage), Reset, Contract. Dev panel button only in editor/dev builds. Pre-battle roster shows owned heroes only.
- [ ] Build, recompile, play-mode checks via CLI with captures; both suites green; commit.

### Task 9: Decisions and hand-off

- [ ] Decisions rows 6, 7, 31 updated, new row for starter team and dev panel, open question on gear slots closed; CLAUDE.md phase line. Screenshots to the owner. Commit.
