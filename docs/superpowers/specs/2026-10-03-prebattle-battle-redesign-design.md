# Pre-battle and battle redesign — design

Status: approved in conversation by the owner, 2026-10-03 ("when we get it all working I'll know if something can be changed").
Builds on: phase 1c branch (`feature/phase-1c-campaign`). References: Idle RPG UI Reference Notes (Claude Docs), sections Mythic Heroes and Omniheroes Lost City.

## Goal

Every fight (campaign now; towers, Boss mode and Rivals later) goes map panel → pre-battle on the battlefield → live battle where you can tap ultimates or let them auto-cast. Placeholders only (flat colour + label); art in phase 5.

## Decisions (owner, 2026-10-03)

| # | Topic | Decision |
|---|---|---|
| 1 | Ultimates | Tap or auto. Auto-Battle: ultimates cast at full energy (today's behaviour). Battle (manual): a full-energy portrait expands and waits for a tap. Changes decisions row 1 to "idle auto-battle with optional manual ultimates" |
| 2 | Stage tap | Tapping a campaign stage opens a panel on the map: stage number, enemy lineup (hero, level, stars), Fight. Tapping another stage swaps the panel |
| 3 | Positions | Your 5 heroes stand in the 2-front / 3-back layout; tap a hero, then another slot, to swap. The 11 formations (row 2) stay a hook |
| 4 | Two autos | Pre-battle has two buttons: **Auto-Battle** and **Battle**. An AUTO toggle in the battle HUD switches mid-fight. Campaign auto-chaining (row 21) always uses Auto-Battle |
| 5 | Quick Deploy | Fills the 5 highest-power heroes, tanks placed in front |

## Screens

**Map stage panel.** Bottom panel over the campaign map; enemy tiles as on today's pre-battle; Fight opens pre-battle. Locked stages show no panel.

**Pre-battle.** Top: power totals, yours vs enemy. Middle: the battlefield as the side view (allies left, enemies right) with each unit standing at its start cell, name and level under it. Swapping: tap one of your units, then another slot (filled or empty). Below: roster strip of hero cards (race colour, name, role, level/stars) with race filter tabs (All + 5 races); tap a card to add to the first empty slot or remove. Bottom row: Back, Quick Deploy, Auto-Battle, Battle. Recommended power stays a hint, not a lock.

**Battle HUD.** Top: stage, timer, speed (1× / 2× / 4×), AUTO toggle, Skip. Field: units with HP and energy bars. Bottom: your 5 portraits with HP and energy bars. In manual mode, at full energy a portrait grows into a tall card (placeholder slot for the hero's alternate art); tapping it casts the ultimate. Skip runs the rest of the fight instantly (with auto ultimates from that point).

**Result.** Unchanged from phase 1c, plus Retry going back to pre-battle with the same positions.

## Rules (Core, engine-free, tested)

- `Battle` gains, per team, `ManualUltimates` (bool). When true, a full-energy hero does not auto-cast; the cast waits for an input.
- Input queue: `Battle.Queue(Input)` with kinds `CastUltimate(unitIndex)` and `SetAuto(team, bool)`. Inputs apply at the start of the next tick. Invalid inputs (dead unit, not enough energy, enemy unit) are ignored and not logged.
- Every applied input is logged as `(tick, kind, unit/flag)`. Replay = same seed + same setups + same input log → same hash (hard rule 4).
- With no inputs and auto on, behaviour and the golden hash are unchanged.
- Fight lifecycle splits: `CampaignState.StartFight(...)` returns a live `FightSession` (battle + seed + setups); the UI steps it in whole ticks; `CampaignState.FinishFight(session, now)` applies the result (first clear, chest settle, attempts) exactly as `Fight` does today. `Fight` remains as start + run-to-end + finish for tools and auto-chaining.
- Positions: `TeamSetup.Cells` (already a hook) is filled from the pre-battle slots; the last-used setup per mode (row 18) stores heroes and cells.
- Quick Deploy: pure function `QuickDeploy.Pick(GameData, owned heroes, level, stars) → 5 heroes + cells` (top 5 by `TeamPower.Hero`, tanks to front cells, ties by roster order).

## Testing

Edit Mode tests for: manual mode never auto-casts; queued cast fires next tick and only when legal; input log replays to the same hash; AUTO toggle mid-fight; no-input auto battle keeps the golden hash; StartFight/FinishFight equals Fight; Quick Deploy picks and placement; last-used cells round-trip through the save. UI checked in play mode via the Unity CLI (screens, tap-to-cast, Skip).

## Out of scope

Alternate ultimate art (phase 5), the 11 formations, star goals on campaign stages (towers get them; see the modes spec), sound.

## Decisions log changes (when built)

Row 1 → "Idle auto-battle with optional manual ultimates (tap a full-energy portrait; Auto-Battle casts them)". New row: pre-battle on the battlefield with slot swapping, Quick Deploy (top 5 by power), Auto-Battle / Battle buttons, map stage panel.
