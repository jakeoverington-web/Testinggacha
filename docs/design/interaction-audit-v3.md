# Interaction audit v3 (after roster pass 10, 2026-10-03)

Reproduce: `tools/csharp/test.sh -Main Gacha.Tests.Tools.InteractionAudit 3000`.

**Verdict: nothing runaway.** Median partner x1.04; 28 of 1,416 pairs above x1.5, one above x2. Biggest single hit stack x4.95 (Kaida's wolf on a weakened, rooted tank; v2 x5.4). One damage dealer with her best four helpers: up to x4.05 (Halcyra with Marisol, Ondine, Tempra and Elowen: Soaked plus Tempo), the owner-accepted "one carry, four supports" case; Halcyra is -6 in real battles (niche report v8).

_Pairs: 1770 x 2 seeds vs the dummy line; real battles: 3000._


## A. Partners that multiply a hero's own damage (her damage with the partner ÷ her damage alone)

Only heroes who deal at least 3,000 damage alone in 30 s, so idle-alone artifacts are excluded.

| Hero | Partner | Her damage alone | With partner | Multiplier | Biggest hit stack in the pair |
| --- | --- | --- | --- | --- | --- |
| Astraea | Vaela | 7523 | 15615 | x2.08 | astraea basic: x2.40 = ATK buffs 1.12 · passives 1.30 · race 1.00 · crit 1.50 · taken 1.10 · vs shield 1.00 (then DEF 0.71, block 1.00) |
| Sangrael | Hartwen | 3036 | 5898 | x1.94 | hartwen basic: x1.73 = ATK buffs 1.00 · passives 1.00 · race 1.00 · crit 1.50 · taken 1.15 · vs shield 1.00 (then DEF 0.71, block 1.00) |
| Ysra | Pelagia | 6951 | 12710 | x1.83 | ysra s1: x3.18 = ATK buffs 1.12 · passives 1.40 · race 1.00 · crit 1.50 · taken 1.35 · vs shield 1.00 (then DEF 0.71, block 1.00) |
| Halcyra | Marisol | 9426 | 17179 | x1.82 | halcyra ult: x2.10 = ATK buffs 1.12 · passives 1.00 · race 1.00 · crit 1.50 · taken 1.25 · vs shield 1.00 (then DEF 0.71, block 1.00) |
| Ilyra | Hartwen | 10328 | 18339 | x1.78 | ilyra basic: x1.98 = ATK buffs 1.00 · passives 1.15 · race 1.00 · crit 1.50 · taken 1.15 · vs shield 1.00 (then DEF 0.71, block 1.00) |
| Ilyra | Draxa | 10328 | 17701 | x1.71 | ilyra basic: x1.98 = ATK buffs 1.00 · passives 1.10 · race 1.00 · crit 1.50 · taken 1.20 · vs shield 1.00 (then DEF 0.71, block 1.00) |
| Astraea | Draxa | 7523 | 12889 | x1.71 | astraea basic: x2.18 = ATK buffs 1.12 · passives 1.30 · race 1.00 · crit 1.50 · taken 1.00 · vs shield 1.00 (then DEF 0.71, block 1.00) |
| Ilyra | Vaela | 10328 | 17684 | x1.71 | ilyra ult: x1.82 = ATK buffs 1.00 · passives 1.10 · race 1.00 · crit 1.50 · taken 1.10 · vs shield 1.00 (then DEF 0.71, block 1.00) |
| Isaura | Zaria | 3419 | 5683 | x1.66 | zaria s1: x2.35 = ATK buffs 1.12 · passives 1.40 · race 1.00 · crit 1.50 · taken 1.00 · vs shield 1.00 (then DEF 0.71, block 1.00) |
| Sangrael | Vaela | 3036 | 4984 | x1.64 | sangrael basic: x1.65 = ATK buffs 1.00 · passives 1.00 · race 1.00 · crit 1.50 · taken 1.10 · vs shield 1.00 (then DEF 0.71, block 1.00) |
| Ysra | Marisol | 6951 | 11373 | x1.64 | ysra s1: x2.94 = ATK buffs 1.12 · passives 1.40 · race 1.00 · crit 1.50 · taken 1.25 · vs shield 1.00 (then DEF 0.71, block 1.00) |
| Ysra | Halcyra | 6951 | 11255 | x1.62 | ysra ult: x2.94 = ATK buffs 1.12 · passives 1.40 · race 1.00 · crit 1.50 · taken 1.25 · vs shield 1.00 (then DEF 0.71, block 1.00) |
| Nyx | Noctelle | 6575 | 10643 | x1.62 | nyx basic: x3.49 = ATK buffs 1.12 · passives 1.30 · race 1.00 · crit 2.40 · taken 1.00 · vs shield 1.00 (then DEF 0.71, block 1.00) |
| Ilyra | Thalassa | 10328 | 16556 | x1.60 | ilyra basic: x1.73 = ATK buffs 1.00 · passives 1.15 · race 1.00 · crit 1.50 · taken 1.00 · vs shield 1.00 (then DEF 0.71, block 1.00) |
| Lucienne | Vaela | 6011 | 9584 | x1.59 | lucienne basic: x1.65 = ATK buffs 1.00 · passives 1.00 · race 1.00 · crit 1.50 · taken 1.10 · vs shield 1.00 (then DEF 0.71, block 1.00) |
| Sangrael | Thalassa | 3036 | 4827 | x1.59 | thalassa basic: x1.50 = ATK buffs 1.00 · passives 1.00 · race 1.00 · crit 1.50 · taken 1.00 · vs shield 1.00 (then DEF 0.71, block 1.00) |
| Astraea | Thalassa | 7523 | 11796 | x1.57 | astraea basic: x1.95 = ATK buffs 1.00 · passives 1.30 · race 1.00 · crit 1.50 · taken 1.00 · vs shield 1.00 (then DEF 0.71, block 1.00) |
| Nerissa | Thalassa | 8426 | 13206 | x1.57 | nerissa basic: x1.68 = ATK buffs 1.12 · passives 1.00 · race 1.00 · crit 1.50 · taken 1.00 · vs shield 1.00 (then DEF 0.71, block 1.00) |
| Astraea | Hartwen | 7523 | 11679 | x1.55 | astraea basic: x2.24 = ATK buffs 1.00 · passives 1.30 · race 1.00 · crit 1.50 · taken 1.15 · vs shield 1.00 (then DEF 0.71, block 1.00) |
| Solenne | Pelagia | 7763 | 12005 | x1.55 | solenne basic: x2.93 = ATK buffs 1.25 · passives 1.25 · race 1.00 · crit 1.50 · taken 1.25 · vs shield 1.00 (then DEF 0.71, block 1.00) |

Median partner multiplier x1.04; above x1.5: 28 of 1416; above x2: 1.

Loop check (ultimates in 30 s, solo → best pair):

- Sable: 2 alone → 4 with Elowen
- Kaida: 2 alone → 4 with Elowen
- Rhiannon: 2 alone → 4 with Elowen
- Calypso: 2 alone → 4 with Elowen
- Nerissa: 2 alone → 4 with Pelagia

## B. Biggest hit stacks in real battles

Amplification = everything stacked on top of the ability's own % at base ATK (ATK buffs × passives × race × crit × damage taken × vs shield), before DEF and block.

| Hero | Hits | Typical (median) | 99th percentile | Max |
| --- | --- | --- | --- | --- |
| nyx | 5618 | x1.10 | x3.14 | x4.42 |
| wolf | 8870 | x1.00 | x3.00 | x4.95 |
| solenne | 7515 | x1.25 | x2.70 | x3.83 |
| ysra | 9405 | x1.12 | x2.59 | x3.70 |
| lucienne | 7408 | x1.10 | x2.52 | x3.56 |
| noctelle | 6521 | x1.10 | x2.40 | x3.22 |
| isaura | 9314 | x1.30 | x2.40 | x3.24 |
| astraea | 5932 | x1.30 | x2.36 | x3.90 |
| calypso | 8262 | x1.20 | x2.34 | x3.41 |
| caelith | 9691 | x1.05 | x2.29 | x4.32 |
| wren | 7790 | x1.10 | x2.25 | x3.23 |
| sable | 16109 | x1.25 | x2.22 | x3.77 |
| ophira | 6425 | x1.10 | x2.21 | x3.60 |
| kaida | 7923 | x1.15 | x2.18 | x3.13 |
| seren | 15027 | x1.05 | x2.15 | x3.19 |

Top 10 single hits:

- Kaida's wolf basic (Basic) → Corvina: x4.95 = ATK buffs 1.00 · passives 2.00 · race 1.00 · crit 1.50 · taken 1.65 · vs shield 1.00 (then DEF 0.56, block 1.00). Attacker buffs [], target debuffs [weaken,root]
- Kaida's wolf basic (Basic) → Valeria: x4.50 = ATK buffs 1.00 · passives 2.00 · race 1.00 · crit 1.50 · taken 1.50 · vs shield 1.00 (then DEF 0.63, block 0.50). Attacker buffs [], target debuffs [weaken,slow]
- Nyx basic (Basic) → Sylwen: x4.42 = ATK buffs 1.12 · passives 1.30 · race 1.10 · crit 2.40 · taken 1.15 · vs shield 1.00 (then DEF 0.81, block 1.00). Attacker buffs [stealth], target debuffs [drained]
- Kaida's wolf basic (Basic) → Corvina: x4.35 = ATK buffs 1.00 · passives 2.00 · race 1.00 · crit 1.50 · taken 1.45 · vs shield 1.00 (then DEF 0.63, block 1.00). Attacker buffs [], target debuffs [blind]
- Caelith basic (Basic) → Corvina: x4.32 = ATK buffs 1.00 · passives 1.30 · race 1.00 · crit 1.65 · taken 1.55 · vs shield 1.30 (then DEF 0.68, block 1.00). Attacker buffs [], target debuffs [mark,star_mark,weaken,burn,def_down]
- Caelith basic (Basic) → Eldrith: x4.12 = ATK buffs 1.25 · passives 1.30 · race 1.00 · crit 1.50 · taken 1.30 · vs shield 1.30 (then DEF 0.56, block 1.00). Attacker buffs [atk_up], target debuffs [mark,slow,blind]
- Kaida's wolf basic (Basic) → Ondine: x3.96 = ATK buffs 1.00 · passives 2.00 · race 1.10 · crit 1.50 · taken 1.20 · vs shield 1.00 (then DEF 0.80, block 1.00). Attacker buffs [], target debuffs [soaked,def_down]
- Kaida's wolf basic (Basic) → Ondine: x3.96 = ATK buffs 1.00 · passives 2.00 · race 1.10 · crit 1.50 · taken 1.20 · vs shield 1.00 (then DEF 0.80, block 1.00). Attacker buffs [], target debuffs [soaked,def_down]
- Nyx basic (Basic) → Hartwen: x3.93 = ATK buffs 1.12 · passives 1.00 · race 1.10 · crit 2.55 · taken 1.25 · vs shield 1.00 (then DEF 0.65, block 1.00). Attacker buffs [crit_up,projectile_block,lifesteal_up,stealth], target debuffs [mark,star_mark,taunt]
- Caelith s1 (Skill) → Valeria: x3.90 = ATK buffs 1.00 · passives 1.30 · race 1.10 · crit 1.50 · taken 1.40 · vs shield 1.30 (then DEF 0.56, block 1.00). Attacker buffs [], target debuffs [weaken]

Most ultimates by one hero in one battle: 17 (Elowen). Most events in one 0.1 s tick: 72.

## C. Expected ceiling: one damage dealer's own damage with the best four helpers (greedy pick vs the dummy line)

| Damage dealer | Alone (30 s) | Best four helpers | With them | Multiplier |
| --- | --- | --- | --- | --- |
| Halcyra | 9426 | Marisol, Ondine, Tempra, Elowen | 38198 | x4.05 |
| Nerissa | 8426 | Ondine, Seren, Elowen, Pelagia | 31483 | x3.74 |
| Nyx | 6575 | Noctelle, Seren, Elowen, Wren | 20500 | x3.12 |
| Ravenna | 15043 | Seren, Wren, Elowen, Seravelle | 43959 | x2.92 |
| Sable | 10656 | Seren, Elowen, Wren, Seravelle | 28716 | x2.69 |
| Kaida | 9092 | Siora, Seren, Wren, Seravelle | 23617 | x2.60 |
| Ysra | 6951 | Marisol, Pelagia, Elowen, Tempra | 17567 | x2.53 |
| Solenne | 7763 | Pelagia, Amarante, Wren, Ondine | 19468 | x2.51 |
| Lucienne | 6011 | Noctelle, Seren, Elowen, Wren | 15066 | x2.51 |
| Astraea | 7523 | Pelagia, Siora, Wren, Ondine | 18820 | x2.50 |
| Rhiannon | 10567 | Seren, Elowen, Rosalind, Wren | 25794 | x2.44 |
| Ophira | 7861 | Seren, Ondine, Zephyra, Noctelle | 17827 | x2.27 |
| Calypso | 6598 | Elowen, Seren, Velisande, Zephyra | 14880 | x2.26 |
| Isaura | 3419 | Ondine, Amarante, Seravelle, Wren | 7397 | x2.16 |
| Caelith | 7056 | Elowen, Wren, Ondine, Zephyra | 15151 | x2.15 |
| Sylwen | 8746 | Pelagia, Ondine, Siora, Wren | 16444 | x1.88 |
| Zaria | 13915 | Elowen, Tempra, Pelagia, Ondine | 25840 | x1.86 |
| Venna | 13313 | Pelagia, Wren, Siora, Ondine | 22840 | x1.72 |
| Ilyra | 10328 | Zephyra, Pelagia, Wren, Vesper | 17508 | x1.70 |
| Maelis | 11909 | Noctelle, Wren, Thessaly, Siora | 17337 | x1.46 |
