# Phase 2: Hero progression — design

Status: approved in conversation by the owner, 2026-10-03.
Builds on: decisions rows 5-7, 12, 31-34; Game Modes and Progression Plan (Claude Docs: Leveling, Star ranks and fodder, Resources); the genre comparison (owner agreed the 7 points, 2026-10-03); Idle RPG UI Reference Notes (Omniheroes hero screens).

## Brief (the 7 points)

1. Level sync from the top 5, no cooldown. 2. Visible tiers (gold → diamond stars) and free resets. 3. Few copies to max (8, not 27). 4. Same-race fodder plus Race Sigils so stars do not depend only on pulls. 5. Stars unlock ability ranks. 6. No layer stacking: phase 2 is levels, stars and gear only (no talents, runes, accessories). 7. Gear kept small: 4 slots, plain upgrades, no substats or rerolling; one fixed extra stat per named piece type (owner, 2026-10-03).

## Owned heroes and start

- `Collection`: per owned hero: stars (1-10), spare copies (duplicates not yet spent). Stored in the save.
- **Starter team (owner):** a new save rolls 5 random heroes at 1★ from the save seed (hard rule 4): exactly 1 tank and 1 healer guaranteed, the other 3 from any role, no duplicates.
- **Dev panel (owner):** hidden, developer-only, stripped from release builds (`#if DEVELOPMENT_BUILD || UNITY_EDITOR`); grants heroes, copies, Race Sigils, gold, Hero XP, Starlight and gear. Not a pop-up: it opens only when tapped (row 15).

## Levels (rows 7, 31)

- **Five Contract slots** hold the top five. Each slot has its own level 1-200, raised with Hero XP and gold per the plan doc's table (level 200 = 14M XP and 21M gold per slot from level 1), plus Starlight at every 20th level (22,500 per slot in total). Costs live in `progression.json`.
- **The level belongs to the slot.** Putting a hero in a slot gives her the slot's level capped by her stars (1★-10★: 60, 70, 80, 90, 100, 120, 140, 160, 180, 200). Swapping is free, with no cooldown and nothing lost. A slot's level above its hero's cap is kept; it shows again when a higher-star hero sits there.
- **Everyone else syncs** to the lowest Contract slot's level, ignoring their own stars.
- The phase 1 stand-in (team at the stage's expected level) is removed: heroes fight at their real levels.

## Stars (rows 2, 3, 5, 31, 32)

Per-rank costs (plan doc 10-rank table; first draft, tuned by the economy sim):

| Star | Cost to reach | Gold | Level cap | Stats vs 1★ | Unlocks |
|---|---|---|---|---|---|
| 2 | 1 copy | 25,000 | 70 | x1.05 | Skill 1 rank 2 |
| 3 | 1 copy | 50,000 | 80 | x1.10 | Skill 2 rank 2 |
| 4 | 1 copy | 100,000 | 90 | x1.15 | Passive rank 2 |
| 5 | 2 copies | 200,000 | 100 | x1.20 | Ultimate rank 2 |
| 6 | 1 copy + 2 fodder | 300,000 | 120 | x1.30 | Skill 1 rank 3 |
| 7 | 1 copy + 3 fodder | 450,000 | 140 | x1.40 | Skill 2 rank 3 |
| 8 | 3 fodder | 650,000 | 160 | x1.50 | Passive rank 3 |
| 9 | 3 fodder | 1,000,000 | 180 | x1.60 | Ultimate rank 3 |
| 10 | 1 copy + 4 fodder | 1,325,000 | 200 | x1.70 | Diamond border |

- Totals: 8 copies, 15 fodder, 4.1M gold. 1-5★ gold, 6-10★ diamond (row 2).
- **Fodder** = a spare 1★ hero of the same race (consumed) or one Race Sigil of that race (row 32). A starred hero must be reset before she can be fed.
- **Ability ranks:** rank 2 raises that ability's numbers by 10%, rank 3 by 20% (plan doc); kit texts stay true at rank 1 and tests check every rank (training-ground rule).
- `heroes.json` `maxStars` goes from 5 to 10 for all 60 heroes.

## Resets (row 31)

Free, any time, full refund: stars back to 1★ returns copies, gold, and the fodder spent as Race Sigils of that hero's race. Removing a hero from a Contract slot costs nothing (the slot keeps its level). A slot can be reset to level 1 for a full XP/gold/Starlight refund.

## Gear (row 6; owner 2026-10-03)

- **4 slots, one fixed main stat each; every piece is a named type with one fixed extra stat (owner, 2026-10-03). No substats, no random rolls.**

| Slot | Main stat | Types and their fixed extra stat (value at Legendary; Uncommon/Rare/Epic give 1/4, 2/4, 3/4 of it) |
|---|---|---|
| Weapon | ATK | Keen: +15% crit rate · Brutal: +30% crit damage · Hexing: +20% effect hit · Radiant: +20% healing done |
| Helm | HP | Warded: +20% effect resist · Blessed: +20% healing received · Sighted: +15% accuracy |
| Armor | DEF | Guarding: +15% block · Evasive: +12% dodge · Leeching: +12% lifesteal |
| Boots | Haste (faster skill cooldowns) | Quick: +15% attack speed · Charged: +20% energy gain · Striding: +20% move speed |

- **4 rarities (owner, 2026-10-03): Uncommon, Rare, Epic, Legendary.** Main stat = 10% / 20% / 30% / 40% of the hero's stat (Boots: 10 / 20 / 30 / 40 haste), times (1 + 5% x upgrade level); upgrades +0 to +20 with gold (first draft: 1,000 x rarity rank x level gold per step, rank 1-4) raise the main stat only. Extra stat = the Legendary value x rank / 4. All first drafts, tuned by the sims.
- **Drops are random; rarities unlock by breakthrough (owner, 2026-10-03):** first clears and the idle chest drop pieces whose slot, type, set and rarity are random, through the shared seeded RNG (save seed + a drop counter, hard rule 4). Uncommon drops from the start; Rare unlocks after ch 5's boss, Epic after ch 10's, Legendary after ch 15's. Each drop picks among unlocked rarities with odds favouring the newest: newest 50%, next 30%, next 15%, oldest 5% (renormalised while fewer are unlocked: 2 unlocked = 62.5 / 37.5, 3 unlocked = 52.6 / 31.6 / 15.8). A piece's stats are never rolled: slot, type, set and rarity fully determine them.
- **5 sets** (owner: stat themes), 2-piece and 4-piece bonuses (first drafts):

| Set | 2-piece | 4-piece |
|---|---|---|
| Fury | +10% ATK | +15% crit rate |
| Bulwark | +10% DEF | shields received +20% |
| Swiftness | +10 haste | +20 haste |
| Vigor | +10% energy regen | start each battle with 30 energy |
| Renewal | +10% healing done | +15% healing received |

- **Equip Best** fills the 4 slots by main-stat value (ties: completing a 2- or 4-piece set). Gear is not hero-locked. Unwanted pieces salvage into gold.
- Gear and set bonuses count in battle and in power (power uses the final stats).

## Screens (placeholders; Omniheroes references)

- **Tab bar:** Campaign | Heroes (Modes arrives in phase 4).
- **Heroes tab:** the 5 Contract slots pinned at the top with their levels and power; below, the collection grid (race-coloured cards: name, stars, level) with race tabs (All + 5 races) and a count (owned / 60).
- **Hero detail:** full-body placeholder (1:2 slot) with name, gold/diamond stars, role and race; stats (HP, ATK, DEF, speed, crit); 4 gear slots; a skills list sliding out from the side with ability ranks; buttons: Level Up (Contract heroes), Stars (cost shown, greyed when short), Gear (pick, upgrade, Equip Best), Reset, Contract (place in a slot).

## Gate (plan doc gate 2)

An economy sim plays idle loot, first clears and levelling against the pacing table (about 700 h of idle income to take the five slots to 200) and passes within 20%. If real progression differs from the phase 1 expected-level curve, StageGen re-tunes stage levels and CampaignGate is re-run with the starter-team rule.

## Testing

Edit Mode tests: starter roll (seeded, 1 tank, 1 healer, 5 unique); level costs and caps; slot-level swap keeps level and respects caps; sync to the lowest slot; star costs per rank, fodder by race, Sigils, reset refunds everything; gear stat formula, upgrade costs, set bonuses (2/4-piece), Equip Best; save round-trip of collection, slots and gear; old saves (phase 1) migrate to a starter team. Battle: gear/set stats feed Battle through per-hero stat bonuses; golden hash unchanged when no gear or sets.

## Out of scope

Summoning and duplicates from pulls (phase 3), talents/runes/accessories (point 6), alternate art (phase 5).

## Decisions log changes (when built)

Row 6 gains: 4 slots (Weapon ATK, Helm HP, Armor DEF, Boots haste), each piece a named type with one fixed extra stat (13 types), tiers by chapter, +20 upgrades, no substats/rerolling, 4 rarities (Uncommon, Rare, Epic, Legendary) unlocked every 5 chapter bosses with odds favouring the newest, random seeded drops, 5 stat-theme sets. Row 7/31 gain: Contract slots own the level; sync to the lowest slot. New row: starter team (random, 1 tank + 1 healer) and dev panel. Open question "Gear slots per hero" closes.
