# Towers, Boss mode and mode map — design

Status: approved in conversation by the owner, 2026-10-03. Mode map and mode screens are placeholder cards; the visual design comes later.
Builds on: decisions rows 35-38; Game Modes and Progression Plan (Claude Docs); pre-battle and battle redesign spec (same date). Build order unchanged: phase 2, phase 3, then these (phase 4).
References: Idle RPG UI Reference Notes (Claude Docs), Omniheroes Lost City.

## Mode map (placeholder)

- Bottom tab bar: **Campaign** and **Modes**.
- Modes tab: a list of large cards: Endless Stair, Race towers (opens the 5 towers), Boss mode, Rivals. Each card: name, one progress line ("Floor 37", "Best: difficulty 3"), lock badge "Unlocks at ch N" (row 35), a dot when a chest is unclaimed. No pop-ups (row 15). Painted map is phase 5.

## Towers (row 38)

- **Endless Stair** (any heroes) and **5 race towers** (that race's 12 heroes only). Endless, no daily limits.
- **Flights:** floors come in groups of 10; one screen per flight (placeholder: a vertical list of 10 floor cards with 3 star pips each). A star bar per flight with chests at 10, 20 and 30 stars.
- **Floor goals (bonus stars only; owner 2026-10-03):** a win clears the floor and unlocks the next; stars never gate progress. Goal 1 is always "win". Goals 2 and 3 come from a fair pool:
  - all 5 survive / no hero falls;
  - win under N seconds;
  - 3+ heroes of one race (12 per race);
  - 2+ carriers of a package with at least 8 carriers (so never Lifesteal, Stealth or Burn).
  On many floors one goal is team-agnostic (survival or time), so any decent team earns stars. Replaying a cleared floor can earn missed stars; first-clear rewards pay once.
- **Guardians and themes:** every 10th floor is a guardian; in race towers it is the race that counters that tower (row 38). Each flight has a synergy theme. Every 50th floor keeps the rule-floor hook (no implementation).
- **Difficulty:** floors are fixed data (`Data/towers.json`, ids `stair_f001`, `lumarin_f001` ... stable). Floor *f* expects the level and stars of campaign stage 3×*f* (towers keep pace with the campaign). Floors 1-100 per tower are generated and sim-checked by the StageGen method (best of 4 reference teams, 50-70% win band); later floors add about 4% enemy power per floor.
- **Auto climb:** fights the next floor with Auto-Battle after each win, stops on a loss (same as campaign auto, row 21). No sweep or skip.
- **Rewards (plan doc):** gold on first clear; Race Sigils every 10 floors in race towers; Fallen Stars every 25th floor; Hourglasses at milestones; star chests per flight.

## Boss mode (row 37)

- 5 bosses, always open, no attempt limit. 90 s fight: your team against the boss and its adds; the boss fights back and a wipe ends the fight early. Score = total damage dealt.
- Each difficulty has 10 damage marks about 1.6x apart; rewards pay the first time a mark is passed (gold and Starlight each mark, Fallen Stars every 5th, an Hourglass every 10th); passing the 10th raises the boss one difficulty, endlessly.
- **Each boss rewards a different synergy package** (each package has 11-21 carriers, so no boss needs specific heroes). Drafts from the World Bible; names and looks are open to change before phase 4:

| Homeland | Boss (draft) | Mechanic | Rewards |
|---|---|---|---|
| Gilded Spire (Lumarin) | The Saint of Mercy: a Hollow templar colossus made of those given "the Mercy" | Gold shields on itself in phases | Exposed (DEF-down, marks) |
| Ashmourne (Noctyr) | The Ash Drinker: a starless thing that learned to drink the living | Drains your HP to heal itself | Disrupted (silence, blind, Drained) |
| Elderwild (Verdani) | The Weeping Willow: a tree of elves bound so they could not turn Hollow | Root adds shield the trunk | Gathered (pull, taunt) |
| Drowned Reach (Thalyri) | The Tithe: what the Mother Below sends back | Floods the field; long tidal casts | Hindered (slow, stun, root) |
| Unlit Observatory (Aethari) | The Unwritten: a spell that rewrote itself | Erases your buffs every few seconds | Tempo (energy, haste) |

- Built as the reusable **special encounter** (row 11 hook): boss = data (HP, phases at damage marks, abilities written in the same ops as hero kits, adds as summons). Chapter bosses and later events reuse it.
- **Screens (placeholder cards):** boss select (5 race-coloured cards: difficulty, 10-mark bar, next reward) → shared pre-battle → battle with a damage meter climbing through the marks instead of a boss HP bar → result (damage, marks passed, rewards).
- Tone (World Bible): show aftermath, not gore; no pure villains — each boss is grief made monstrous, not evil for its own sake.

## Testing

Floor goals evaluated in Core from the battle log, with tests per goal type; fairness test: every generated team goal is satisfiable by at least 8 heroes. Tower and boss data validated like stages.json. A gate tool plays floors 1-100 of each tower and every boss to difficulty 3 in the sim (plan doc gate 3: every mode played start to finish).

## Out of scope

Rivals (row 36, unchanged; gets its card only), painted mode map and art, rule floors (hook only), events.

## Decisions log changes (when built)

Row 38 gains: floor star goals (bonus only, fair pool), flights of 10 with star chests, difficulty tied to campaign stage 3×floor, auto climb. Row 37 gains: one package per boss, the five drafted bosses (once the owner confirms names). New row: Campaign / Modes tab bar with placeholder mode cards.
