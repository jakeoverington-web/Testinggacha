# Interaction audit v2 (after roster pass 9, 2026-10-03)

Looks for things that multiply unexpectedly. Reproduce: `tools/csharp/test.sh -Main Gacha.Tests.Tools.InteractionAudit 3000`.

**Verdict: synergy multiplies more, as intended; nothing runaway.** Median partner x1.04 (v1: x1.01); 26 of 1,416 pairs above x1.5 (v1: 9), one above x2 (Astraea + Vaela: Vaela pulls enemies together and Astraea pays off grouped enemies). Biggest single hit stack x5.4 (Kaida's wolf on a weakened, rooted tank: wolves' double damage, a crit, and 1.8x damage taken), v1 x3.3. One damage dealer with her best four helpers: x1.7 to x3.5 (v1: up to x2.6); the top is Nerissa with Pelagia, Siora, Seren and Wren, the owner-accepted "one carry, four supports" case.

_Pairs: 1770 x 2 seeds vs the dummy line; real battles: 3000._


## A. Partners that multiply a hero's own damage (her damage with the partner ÷ her damage alone)

Only heroes who deal at least 3,000 damage alone in 30 s, so idle-alone artifacts are excluded.

| Hero | Partner | Her damage alone | With partner | Multiplier | Biggest hit stack in the pair |
| --- | --- | --- | --- | --- | --- |
| Astraea | Vaela | 7523 | 16210 | x2.15 | astraea basic: x2.49 = ATK buffs 1.12 · passives 1.35 · race 1.00 · crit 1.50 · taken 1.10 · vs shield 1.00 (then DEF 0.71, block 1.00) |
| Sangrael | Hartwen | 3036 | 6064 | x2.00 | hartwen basic: x1.80 = ATK buffs 1.00 · passives 1.00 · race 1.00 · crit 1.50 · taken 1.20 · vs shield 1.00 (then DEF 0.71, block 1.00) |
| Ysra | Pelagia | 6951 | 12964 | x1.87 | ysra s1: x3.29 = ATK buffs 1.12 · passives 1.40 · race 1.00 · crit 1.50 · taken 1.40 · vs shield 1.00 (then DEF 0.71, block 1.00) |
| Halcyra | Marisol | 9426 | 17179 | x1.82 | halcyra ult: x2.10 = ATK buffs 1.12 · passives 1.00 · race 1.00 · crit 1.50 · taken 1.25 · vs shield 1.00 (then DEF 0.71, block 1.00) |
| Ilyra | Hartwen | 10328 | 18698 | x1.81 | ilyra basic: x2.07 = ATK buffs 1.00 · passives 1.15 · race 1.00 · crit 1.50 · taken 1.20 · vs shield 1.00 (then DEF 0.71, block 1.00) |
| Astraea | Draxa | 7523 | 13379 | x1.78 | astraea basic: x2.27 = ATK buffs 1.12 · passives 1.35 · race 1.00 · crit 1.50 · taken 1.00 · vs shield 1.00 (then DEF 0.71, block 1.00) |
| Ilyra | Draxa | 10328 | 17701 | x1.71 | ilyra basic: x1.98 = ATK buffs 1.00 · passives 1.10 · race 1.00 · crit 1.50 · taken 1.20 · vs shield 1.00 (then DEF 0.71, block 1.00) |
| Ilyra | Vaela | 10328 | 17684 | x1.71 | ilyra ult: x1.82 = ATK buffs 1.00 · passives 1.10 · race 1.00 · crit 1.50 · taken 1.10 · vs shield 1.00 (then DEF 0.71, block 1.00) |
| Sangrael | Vaela | 3036 | 4984 | x1.64 | sangrael basic: x1.65 = ATK buffs 1.00 · passives 1.00 · race 1.00 · crit 1.50 · taken 1.10 · vs shield 1.00 (then DEF 0.71, block 1.00) |
| Astraea | Hartwen | 7523 | 12338 | x1.64 | astraea basic: x2.43 = ATK buffs 1.00 · passives 1.35 · race 1.00 · crit 1.50 · taken 1.20 · vs shield 1.00 (then DEF 0.71, block 1.00) |
| Ilyra | Thalassa | 10328 | 16916 | x1.64 | thalassa basic: x1.80 = ATK buffs 1.00 · passives 1.00 · race 1.00 · crit 1.50 · taken 1.20 · vs shield 1.00 (then DEF 0.71, block 1.00) |
| Ysra | Marisol | 6951 | 11373 | x1.64 | ysra s1: x2.94 = ATK buffs 1.12 · passives 1.40 · race 1.00 · crit 1.50 · taken 1.25 · vs shield 1.00 (then DEF 0.71, block 1.00) |
| Solenne | Pelagia | 6799 | 11120 | x1.64 | solenne basic: x2.44 = ATK buffs 1.25 · passives 1.00 · race 1.00 · crit 1.50 · taken 1.30 · vs shield 1.00 (then DEF 0.71, block 1.00) |
| Astraea | Thalassa | 7523 | 12262 | x1.63 | astraea basic: x2.03 = ATK buffs 1.00 · passives 1.35 · race 1.00 · crit 1.50 · taken 1.00 · vs shield 1.00 (then DEF 0.71, block 1.00) |
| Ysra | Halcyra | 6951 | 11255 | x1.62 | ysra ult: x2.94 = ATK buffs 1.12 · passives 1.40 · race 1.00 · crit 1.50 · taken 1.25 · vs shield 1.00 (then DEF 0.71, block 1.00) |
| Nyx | Noctelle | 6575 | 10643 | x1.62 | nyx basic: x3.49 = ATK buffs 1.12 · passives 1.30 · race 1.00 · crit 2.40 · taken 1.00 · vs shield 1.00 (then DEF 0.71, block 1.00) |
| Sangrael | Thalassa | 3036 | 4846 | x1.60 | thalassa basic: x1.50 = ATK buffs 1.00 · passives 1.00 · race 1.00 · crit 1.50 · taken 1.00 · vs shield 1.00 (then DEF 0.71, block 1.00) |
| Lucienne | Vaela | 6011 | 9584 | x1.59 | lucienne basic: x1.65 = ATK buffs 1.00 · passives 1.00 · race 1.00 · crit 1.50 · taken 1.10 · vs shield 1.00 (then DEF 0.71, block 1.00) |
| Astraea | Corvina | 7523 | 11861 | x1.58 | astraea basic: x2.03 = ATK buffs 1.00 · passives 1.35 · race 1.00 · crit 1.50 · taken 1.00 · vs shield 1.00 (then DEF 0.71, block 1.00) |
| Sylwen | Hartwen | 8746 | 13610 | x1.56 | sylwen basic: x2.02 = ATK buffs 1.12 · passives 1.00 · race 1.00 · crit 1.50 · taken 1.20 · vs shield 1.00 (then DEF 0.71, block 1.00) |

Median partner multiplier x1.04; above x1.5: 26 of 1416; above x2: 1.

Loop check (ultimates in 30 s, solo → best pair):

- Sable: 2 alone → 4 with Elowen
- Kaida: 2 alone → 4 with Elowen
- Calypso: 2 alone → 4 with Elowen
- Nerissa: 2 alone → 4 with Seren

## B. Biggest hit stacks in real battles

Amplification = everything stacked on top of the ability's own % at base ATK (ATK buffs × passives × race × crit × damage taken × vs shield), before DEF and block.

| Hero | Hits | Typical (median) | 99th percentile | Max |
| --- | --- | --- | --- | --- |
| nyx | 5565 | x1.10 | x3.12 | x3.96 |
| wolf | 8733 | x1.00 | x2.64 | x5.40 |
| ysra | 9471 | x1.12 | x2.63 | x3.81 |
| astraea | 5998 | x1.35 | x2.48 | x4.41 |
| noctelle | 6479 | x1.10 | x2.44 | x3.43 |
| solenne | 7302 | x1.20 | x2.37 | x3.49 |
| calypso | 8044 | x1.20 | x2.35 | x4.50 |
| lucienne | 7655 | x1.10 | x2.35 | x3.68 |
| isaura | 9630 | x1.30 | x2.34 | x3.43 |
| ophira | 6444 | x1.10 | x2.31 | x3.47 |
| nerissa | 10151 | x1.10 | x2.31 | x3.70 |
| wren | 7593 | x1.12 | x2.28 | x3.51 |
| caelith | 9242 | x1.05 | x2.24 | x4.41 |
| kaida | 7817 | x1.15 | x2.22 | x3.32 |
| sable | 15448 | x1.24 | x2.21 | x4.54 |

Top 10 single hits:

- Kaida's wolf basic (Basic) → Corvina: x5.40 = ATK buffs 1.00 · passives 2.00 · race 1.00 · crit 1.50 · taken 1.80 · vs shield 1.00 (then DEF 0.56, block 1.00). Attacker buffs [], target debuffs [weaken,root]
- Kaida's wolf basic (Basic) → Mireille: x4.80 = ATK buffs 1.00 · passives 2.00 · race 1.00 · crit 1.50 · taken 1.60 · vs shield 1.00 (then DEF 0.77, block 1.00). Attacker buffs [], target debuffs [weaken,root]
- Kaida's wolf basic (Basic) → Corvina: x4.80 = ATK buffs 1.00 · passives 2.00 · race 1.00 · crit 1.50 · taken 1.60 · vs shield 1.00 (then DEF 0.63, block 1.00). Attacker buffs [], target debuffs [weaken]
- Sable basic (Basic) → Eldrith: x4.54 = ATK buffs 1.00 · passives 1.35 · race 1.10 · crit 1.70 · taken 1.80 · vs shield 1.00 (then DEF 0.63, block 1.00). Attacker buffs [hot], target debuffs [weaken,bleed]
- Calypso basic (Basic) → Runa: x4.50 = ATK buffs 1.00 · passives 1.40 · race 1.10 · crit 1.50 · taken 1.95 · vs shield 1.00 (then DEF 0.63, block 1.00). Attacker buffs [lifesteal_up], target debuffs [weaken,mark]
- Kaida's wolf basic (Basic) → Corvina: x4.50 = ATK buffs 1.00 · passives 2.00 · race 1.00 · crit 1.50 · taken 1.50 · vs shield 1.00 (then DEF 0.63, block 1.00). Attacker buffs [], target debuffs [blind]
- Caelith ult (Ult) → Valeria: x4.41 = ATK buffs 1.25 · passives 1.30 · race 1.10 · crit 1.65 · taken 1.15 · vs shield 1.30 (then DEF 0.56, block 1.00). Attacker buffs [atk_up,crit_up], target debuffs [mark,star_mark]
- Astraea basic (Basic) → Kaida's wolf: x4.41 = ATK buffs 1.25 · passives 2.35 · race 1.00 · crit 1.50 · taken 1.00 · vs shield 1.00 (then DEF 0.83, block 1.00). Attacker buffs [atk_up,aspd_up], target debuffs []
- Astraea basic (Basic) → Kaida's wolf: x4.23 = ATK buffs 1.00 · passives 2.35 · race 1.00 · crit 1.50 · taken 1.20 · vs shield 1.00 (then DEF 0.83, block 1.00). Attacker buffs [lifesteal_up], target debuffs [fear]
- Kaida's wolf basic (Basic) → Corvina: x4.20 = ATK buffs 1.00 · passives 2.00 · race 1.00 · crit 1.50 · taken 1.40 · vs shield 1.00 (then DEF 0.63, block 1.00). Attacker buffs [], target debuffs []

Most ultimates by one hero in one battle: 19 (Elowen). Most events in one 0.1 s tick: 75.

## C. Expected ceiling: one damage dealer's own damage with the best four helpers (greedy pick vs the dummy line)

| Damage dealer | Alone (30 s) | Best four helpers | With them | Multiplier |
| --- | --- | --- | --- | --- |
| Nerissa | 9088 | Pelagia, Siora, Seren, Wren | 32085 | x3.53 |
| Nyx | 6575 | Noctelle, Seren, Elowen, Velisande | 22265 | x3.39 |
| Halcyra | 9426 | Marisol, Ondine, Elowen, Seren | 28940 | x3.07 |
| Ysra | 6951 | Pelagia, Ondine, Marisol, Siora | 20837 | x3.00 |
| Ravenna | 15043 | Seren, Wren, Elowen, Seravelle | 44870 | x2.98 |
| Astraea | 7523 | Pelagia, Elowen, Siora, Marisol | 22303 | x2.96 |
| Kaida | 9092 | Seren, Wren, Lunaith, Elowen | 26556 | x2.92 |
| Solenne | 6799 | Pelagia, Amarante, Wren, Ondine | 18769 | x2.76 |
| Lucienne | 6011 | Noctelle, Seren, Elowen, Wren | 14807 | x2.46 |
| Calypso | 6680 | Ondine, Pelagia, Seren, Tempra | 16396 | x2.45 |
| Sable | 10656 | Seren, Wren, Pelagia, Elowen | 23775 | x2.23 |
| Rhiannon | 10567 | Seren, Elowen, Ondine, Zephyra | 22557 | x2.13 |
| Caelith | 7056 | Elowen, Ondine, Pelagia, Tempra | 14396 | x2.04 |
| Isaura | 3720 | Wren, Lunaith, Liora, Ondine | 7578 | x2.04 |
| Sylwen | 8746 | Zephyra, Wren, Ondine, Pelagia | 16186 | x1.85 |
| Ophira | 8012 | Ondine, Seren, Pelagia, Elowen | 14025 | x1.75 |
| Zaria | 13915 | Elowen, Tempra, Wren, Zephyra | 24268 | x1.74 |
| Ilyra | 10328 | Amarante, Wren, Pelagia, Siora | 17774 | x1.72 |
| Venna | 13313 | Pelagia, Wren, Lunaith, Ondine | 22674 | x1.70 |
| Maelis | 11909 | Noctelle, Wren, Thessaly, Velisande | 18210 | x1.53 |
