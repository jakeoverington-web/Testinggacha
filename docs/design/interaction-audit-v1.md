# Interaction audit v1 (after roster pass 7, 2026-10-02)

Looks for things that multiply unexpectedly. Reproduce: `tools/csharp/test.sh -Main Gacha.Tests.Tools.InteractionAudit 3000` (about 15 s).

**Verdict: no broken multiplication found.** The median partner leaves a hero's own damage unchanged (x1.01); 9 of 2,006 hero-partner combinations exceed x1.5 and one exceeds x2 (Draxa + Hartwen: Hartwen groups every enemy and Draxa's dragon breath is an area auto attack; a natural combo, and Draxa still deals less than any damage dealer). The largest single hit stack in 3,000 real battles is x3.3 over the ability's own value, built from modest legitimate factors. One damage dealer with her best four helpers reaches x1.2 to x2.6 of her own damage (top: Halcyra with Marisol, Elowen, Seren, Tempra: the intended Soaked synergy). No partner doubles any hero's ultimates; the most ultimates in one battle (13-16, tanks) happen in rare 90-second stalemates.

_Pairs: 1770 x 2 seeds vs the dummy line; real battles: 3000._


## A. Partners that multiply a hero's own damage (her damage with the partner ÷ her damage alone)

Only heroes who deal at least 3,000 damage alone in 30 s, so idle-alone artifacts are excluded.

| Hero | Partner | Her damage alone | With partner | Multiplier | Biggest hit stack in the pair |
| --- | --- | --- | --- | --- | --- |
| Draxa | Hartwen | 3216 | 7444 | x2.31 | draxa basic: x1.50 = ATK buffs 1.00 · passives 1.00 · race 1.00 · crit 1.50 · taken 1.00 · vs shield 1.00 (then DEF 0.71, block 1.00) |
| Draxa | Thalassa | 3216 | 6100 | x1.90 | draxa basic: x1.50 = ATK buffs 1.00 · passives 1.00 · race 1.00 · crit 1.50 · taken 1.00 · vs shield 1.00 (then DEF 0.71, block 1.00) |
| Draxa | Zephyra | 3216 | 5565 | x1.73 | zephyra basic: x1.50 = ATK buffs 1.00 · passives 1.00 · race 1.00 · crit 1.50 · taken 1.00 · vs shield 1.00 (then DEF 0.71, block 1.00) |
| Draxa | Seravelle | 3216 | 5487 | x1.71 | draxa basic: x1.88 = ATK buffs 1.25 · passives 1.00 · race 1.00 · crit 1.50 · taken 1.00 · vs shield 1.00 (then DEF 0.71, block 1.00) |
| Sangrael | Hartwen | 3554 | 6000 | x1.69 | sangrael basic: x1.50 = ATK buffs 1.00 · passives 1.00 · race 1.00 · crit 1.50 · taken 1.00 · vs shield 1.00 (then DEF 0.71, block 1.00) |
| Draxa | Pelagia | 3216 | 5416 | x1.68 | draxa basic: x1.50 = ATK buffs 1.00 · passives 1.00 · race 1.00 · crit 1.50 · taken 1.00 · vs shield 1.00 (then DEF 0.71, block 1.00) |
| Halcyra | Marisol | 10842 | 18105 | x1.67 | halcyra ult: x1.88 = ATK buffs 1.00 · passives 1.00 · race 1.00 · crit 1.50 · taken 1.25 · vs shield 1.00 (then DEF 0.71, block 1.00) |
| Draxa | Siora | 3216 | 5218 | x1.62 | draxa basic: x1.50 = ATK buffs 1.00 · passives 1.00 · race 1.00 · crit 1.50 · taken 1.00 · vs shield 1.00 (then DEF 0.71, block 1.00) |
| Draxa | Solenne | 3216 | 5014 | x1.56 | solenne basic: x2.16 = ATK buffs 1.25 · passives 1.00 · race 1.00 · crit 1.50 · taken 1.15 · vs shield 1.00 (then DEF 0.71, block 1.00) |
| Ilyra | Hartwen | 12697 | 18947 | x1.49 | ilyra basic: x1.73 = ATK buffs 1.00 · passives 1.15 · race 1.00 · crit 1.50 · taken 1.00 · vs shield 1.00 (then DEF 0.71, block 1.00) |
| Astraea | Vaela | 9240 | 13764 | x1.49 | astraea basic: x1.80 = ATK buffs 1.00 · passives 1.20 · race 1.00 · crit 1.50 · taken 1.00 · vs shield 1.00 (then DEF 0.71, block 1.00) |
| Ilyra | Vaela | 12697 | 18756 | x1.48 | ilyra ult: x1.65 = ATK buffs 1.00 · passives 1.10 · race 1.00 · crit 1.50 · taken 1.00 · vs shield 1.00 (then DEF 0.71, block 1.00) |
| Draxa | Vaela | 3216 | 4729 | x1.47 | draxa basic: x1.50 = ATK buffs 1.00 · passives 1.00 · race 1.00 · crit 1.50 · taken 1.00 · vs shield 1.00 (then DEF 0.71, block 1.00) |
| Draxa | Thessaly | 3216 | 4696 | x1.46 | draxa basic: x1.50 = ATK buffs 1.00 · passives 1.00 · race 1.00 · crit 1.50 · taken 1.00 · vs shield 1.00 (then DEF 0.71, block 1.00) |
| Halcyra | Pelagia | 10842 | 15787 | x1.46 | halcyra ult: x1.88 = ATK buffs 1.00 · passives 1.00 · race 1.00 · crit 1.50 · taken 1.25 · vs shield 1.00 (then DEF 0.71, block 1.00) |
| Ilyra | Thalassa | 12697 | 18407 | x1.45 | ilyra basic: x1.65 = ATK buffs 1.00 · passives 1.10 · race 1.00 · crit 1.50 · taken 1.00 · vs shield 1.00 (then DEF 0.71, block 1.00) |
| Draxa | Corvina | 3216 | 4640 | x1.44 | draxa basic: x1.50 = ATK buffs 1.00 · passives 1.00 · race 1.00 · crit 1.50 · taken 1.00 · vs shield 1.00 (then DEF 0.71, block 1.00) |
| Lucienne | Hartwen | 5473 | 7859 | x1.44 | lucienne basic: x1.90 = ATK buffs 1.00 · passives 1.00 · race 1.00 · crit 1.90 · taken 1.00 · vs shield 1.00 (then DEF 0.71, block 1.00) |
| Sangrael | Vaela | 3554 | 5069 | x1.43 | sangrael basic: x1.50 = ATK buffs 1.00 · passives 1.00 · race 1.00 · crit 1.50 · taken 1.00 · vs shield 1.00 (then DEF 0.71, block 1.00) |
| Astraea | Corvina | 9240 | 13132 | x1.42 | astraea s1: x1.80 = ATK buffs 1.00 · passives 1.20 · race 1.00 · crit 1.50 · taken 1.00 · vs shield 1.00 (then DEF 0.71, block 1.00) |

Median partner multiplier x1.01; above x1.5: 9 of 2006; above x2: 1.

Loop check (ultimates in 30 s, solo → best pair):

- No hero doubles her ultimates from any single partner.

## B. Biggest hit stacks in real battles

Amplification = everything stacked on top of the ability's own % at base ATK (ATK buffs × passives × race × crit × damage taken × vs shield), before DEF and block.

| Hero | Hits | Typical (median) | 99th percentile | Max |
| --- | --- | --- | --- | --- |
| nyx | 5177 | x1.00 | x2.48 | x3.03 |
| astraea | 6939 | x1.20 | x2.20 | x3.30 |
| ysra | 10441 | x1.10 | x2.16 | x3.22 |
| solenne | 8311 | x1.15 | x2.10 | x2.89 |
| calypso | 8728 | x1.10 | x2.06 | x2.96 |
| isaura | 10673 | x1.25 | x2.06 | x2.93 |
| seravelle | 6814 | x1.03 | x2.00 | x2.91 |
| rhiannon | 9429 | x1.10 | x2.00 | x2.44 |
| wolf | 10136 | x1.00 | x2.00 | x3.20 |
| seren | 16977 | x1.00 | x2.00 | x2.65 |
| caelith | 8834 | x1.00 | x1.98 | x3.33 |
| noctelle | 7941 | x1.00 | x1.91 | x2.70 |
| lucienne | 7899 | x1.00 | x1.90 | x2.73 |
| kaida | 8613 | x1.10 | x1.90 | x2.53 |
| halcyra | 14061 | x1.00 | x1.88 | x3.00 |

Top 10 single hits:

- Caelith basic (Basic) → Valeria: x3.33 = ATK buffs 1.00 · passives 1.15 · race 1.10 · crit 1.50 · taken 1.35 · vs shield 1.30 (then DEF 0.56, block 1.00). Attacker buffs [], target debuffs [fear,weaken]
- Astraea ult (Ult) → Kaida's wolf: x3.30 = ATK buffs 1.00 · passives 2.20 · race 1.00 · crit 1.50 · taken 1.00 · vs shield 1.00 (then DEF 0.83, block 1.00). Attacker buffs [], target debuffs []
- Astraea basic (Basic) → Fenna's decoy: x3.30 = ATK buffs 1.00 · passives 2.20 · race 1.00 · crit 1.50 · taken 1.00 · vs shield 1.00 (then DEF 1.00, block 1.00). Attacker buffs [], target debuffs []
- Astraea ult (Ult) → Kaida's wolf: x3.30 = ATK buffs 1.00 · passives 2.20 · race 1.00 · crit 1.50 · taken 1.00 · vs shield 1.00 (then DEF 0.83, block 1.00). Attacker buffs [def_up], target debuffs []
- Astraea basic (Basic) → Fenna's decoy: x3.30 = ATK buffs 1.00 · passives 2.20 · race 1.00 · crit 1.50 · taken 1.00 · vs shield 1.00 (then DEF 1.00, block 1.00). Attacker buffs [def_up], target debuffs [taunt]
- Astraea ult (Ult) → Fenna's decoy: x3.30 = ATK buffs 1.00 · passives 2.20 · race 1.00 · crit 1.50 · taken 1.00 · vs shield 1.00 (then DEF 1.00, block 1.00). Attacker buffs [], target debuffs []
- Ysra s1 (Skill) → Draxa: x3.22 = ATK buffs 1.25 · passives 1.25 · race 1.10 · crit 1.50 · taken 1.25 · vs shield 1.00 (then DEF 0.63, block 1.00). Attacker buffs [atk_up], target debuffs [silence,soaked,charm]
- Kaida's wolf basic (Basic) → Eldrith: x3.20 = ATK buffs 1.00 · passives 2.00 · race 1.00 · crit 1.60 · taken 1.00 · vs shield 1.00 (then DEF 0.63, block 1.00). Attacker buffs [aspd_up], target debuffs [airborne]
- Ysra s1 (Skill) → Coralie: x3.16 = ATK buffs 1.25 · passives 1.25 · race 1.00 · crit 1.50 · taken 1.35 · vs shield 1.00 (then DEF 0.63, block 1.00). Attacker buffs [lifesteal_up,hot,atk_up], target debuffs [soaked,slow,fear]
- Ysra ult (Ult) → Kaida: x3.13 = ATK buffs 1.25 · passives 1.25 · race 1.00 · crit 1.60 · taken 1.25 · vs shield 1.00 (then DEF 0.76, block 1.00). Attacker buffs [atk_up], target debuffs [soaked]

Most ultimates by one hero in one battle: 16 (Cassia). Most events in one 0.1 s tick: 79.

## C. Expected ceiling: one damage dealer's own damage with the best four helpers (greedy pick vs the dummy line)

| Damage dealer | Alone (30 s) | Best four helpers | With them | Multiplier |
| --- | --- | --- | --- | --- |
| Halcyra | 10842 | Marisol, Elowen, Seren, Tempra | 28526 | x2.63 |
| Nerissa | 8039 | Seren, Pelagia, Siora, Seravelle | 20640 | x2.57 |
| Sable | 9795 | Seren, Tempra, Pelagia, Seravelle | 19807 | x2.02 |
| Ravenna | 13467 | Seren, Elowen, Seravelle, Rosalind | 26244 | x1.95 |
| Nyx | 6411 | Seren, Elowen, Siora, Seravelle | 12012 | x1.87 |
| Solenne | 7595 | Zephyra, Pelagia, Tempra, Marisol | 13918 | x1.83 |
| Lucienne | 5473 | Zephyra, Seravelle, Seren, Rosalind | 9772 | x1.79 |
| Calypso | 8001 | Seren, Elowen, Tempra, Noctelle | 14258 | x1.78 |
| Astraea | 9240 | Pelagia, Marisol, Wren, Lunaith | 16408 | x1.78 |
| Ophira | 7364 | Elowen, Seren, Seravelle, Marisol | 12978 | x1.76 |
| Kaida | 9108 | Seren, Ondine, Elowen, Seravelle | 15665 | x1.72 |
| Ysra | 8405 | Pelagia, Tempra, Marisol, Ondine | 14047 | x1.67 |
| Venna | 13496 | Thessaly, Elowen, Zephyra, Seren | 20924 | x1.55 |
| Rhiannon | 10742 | Seren, Zephyra, Lorelei, Wren | 16237 | x1.51 |
| Caelith | 10463 | Zephyra, Amarante, Seren, Rosalind | 15766 | x1.51 |
| Maelis | 15593 | Rosalind, Elowen, Pelagia, Wren | 23271 | x1.49 |
| Isaura | 5199 | Siora, Wren, Rosalind, Seren | 7443 | x1.43 |
| Zaria | 14336 | Zephyra, Seravelle, Tempra, Seren | 18994 | x1.32 |
| Sylwen | 7960 | Zephyra, Ondine, Seren, Rosalind | 10231 | x1.29 |
| Ilyra | 12697 | Wren, Velisande, Odette, Rosalind | 15742 | x1.24 |
done in 11s
