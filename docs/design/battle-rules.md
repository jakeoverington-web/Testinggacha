# Battle rules v1 (as built)

Code: `Assets/_Game/Scripts/Core/Battle/` (engine-free). Tuning: `Data/battle.json`, `combat.json`, `races.json`. Kits: `tools/roster/kits.py` → `heroes.json` (`ops`, passive `mods`/`triggers`/`flags`, `ai`). Tests: `docs/design/testing.md`.

## Loop

| Rule | Value |
| --- | --- |
| Timestep | 0.1 s; every unit acts every tick; order alternates teams and flips each tick |
| Hero HP | Every hero's HP x 1.2 in battle (`hpScale`), for fights of about 30 s |
| Auto attacks | 75% ATK by default (`basicAttackPct`); specialists 100%, casters 60% (`basicPct` per hero) |
| Time limit | 90 s; a timeout counts as a loss for the attacker (team A) |
| Field | 16 m × 6 m, allies left, enemies right; formation = grid cells (front col x = 2.5 m, back x = 5 m) |
| Each tick, per unit | Ultimate at 100 energy → Skill 1 → Skill 2 → basic attack if the target is in range → else move |
| Skills | Start at 50% cooldown; 0.1 s cast pause after any cast; Haste divides cooldowns |
| "Worth casting" | An ability waits until its first effect has a target: heals need an ally under 95%, cleanses a debuff, dispels a buff, revives a fallen ally; plant-yourself skills need an enemy in reach |
| Aim | Sleep and stasis go on the highest-ATK enemy no ally is hitting (damage would wake or shield the focus target); Rosalind's Knighting goes to the ally most enemies are attacking |
| Energy | Basic hit +10, 0.5 per 1% HP lost, kill +20 (scaled by Energy gain and half of Haste); gifts and drains are flat |
| End | A side loses when all its heroes are down (summons don't count) |

## One hit, in order

1. Stasis or projectile wall absorbs it → 2. basic attacks: blind miss, then dodge − accuracy → 3. skill shield / one-hit shield
4. × (1 + outgoing passives) × (1 − damage down) × 1.10 if the attacker's race beats the target's → 5. crit roll (+30% vs sleeping) × crit damage
6. × K/(K + DEF), K = 300 (not for DoTs) → 7. × (1 + weaken + mark − DR + incoming passives ± Soaked: fire −15%, lightning +25%)
8. block halves → 9. shared damage (Isolde, Thessaly) → 10. shields absorb, then HP; Unkillable keeps 1 HP
11. after: lifesteal, reflect / thorns / counter, curse spread, wake from sleep, stealth breaks, on-hit passives

## Statuses and control

| Rule | Value |
| --- | --- |
| Resist | Every enemy debuff: chance = max(15%, resist − effect hit) |
| Hard control | Stasis > Charm > Fear > Stun > Airborne > Sleep > Root; only one at a time, the stronger wins; max 2.5 s |
| Diminishing returns | 1st control full; 2nd within 4 s half; any more inside the 6 s window resisted |
| Control immunity | Blocks stun, sleep, root, fear, charm, silence, knock-up (Lunaith's is sleep and stun only) |
| Stacking | Burn, bleed, poison stack to 3; other statuses refresh to the stronger value and longer time |
| Shields | Stack, capped at 50% max HP (trimmed if max HP shrinks), last 10 s, not dispellable |
| Revive | Once per hero per battle |
| Payoff window | A condition written `stun+3` is true while the status is on and for 3 s after it ends; used for control and stealth payoffs (control lasts 1-2.5 s and sleep breaks on the first hit) |
| Race team bonus | 3 of one race + 2 of another: +12% ATK and HP; 5 of one race: +20% (races.json `teamBonus`) |

## AI

Enemy target keys (from each hero's targeting text): nearest, highest_atk, densest, healer_first, near_weakest_ally, most_energy, highest_hp, isolated_else_carry, lowest_hp, lowest_hp_back, most_buffed, nearest_with:status, with_else_highest_atk:status, random (sticky). Taunt overrides; marked enemies are preferred by everyone; charmed units attack their own side.

Movement: advance (melee), dive (Nyx, Nerissa, Ophira, Lucienne: wait just behind the front line until an ally is in melee, she is hit, or 3 s pass), kite (stop at range), follow (stand behind the followed ally), guard (Isolde: beside the weakest ally). Units on a team nudge apart; nobody body-blocks.

## Interpretations awaiting review

These fill gaps the kit text leaves open; all are data in `kits.py` or `battle.json`.

| Topic | Choice |
| --- | --- |
| Unnamed numbers | Heal 200% ATK, self shield 15% max HP, burn 30% ATK/s, bleed 25% ATK/s, poison 2% max HP/s, slow 30%, blind 50% |
| Untargetable vs stealth | Untargetable dodges everything; stealth only stops being picked (area effects still hit) |
| Projectiles (Coralie, Vaela walls) | Basic attacks and single-target skills from ranged heroes |
| Summons | Wolves 30% HP / 40% ATK for 10 s, double damage to enemies rooted or knocked up (or in the last 3 s); decoy 20% HP, taunts enemies within 3 m; Rhiannon's stag is untargetable, 50% ATK, leaves with her |
| Small kit reads | Halcyra's Static Mark hits for 80% lightning; Vesper's ultimate heals each ally (not split) by the % of damage in its text; Seren's Star Map gives 5 energy per ally hit; Aurelle revives the first ally to fall in 8 s; Mireille's after-regen starts when Bloomfall ends |
