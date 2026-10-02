# Niche report v3 (battle rules v1 + dive timing + hpScale 1.5, 2026-10-02)

Balance target (decisions row 29): every hero is the best pick somewhere; no hero is the best pick everywhere. Uplift = points a hero adds to her team's result versus a random same-role replacement, same opponent and seed. DOMINANT = overall ≥ +10 and above peers in ≥ 11 of 13 situations. NO NICHE = never more than +2 above peers in any situation. Reproduce: `tools/csharp/test.sh -Main Gacha.Tests.Tools.NicheReport 150 1`.

Changes since v2: all heroes have 1.5x HP (battle.json hpScale): average fight 20 s -> 31 s, timeouts 2% -> 5.5%. Flags at the extremes barely moved, so the outliers are kit numbers. Borderline NO NICHE heroes (-4 to -9) are within reach of noise and of the meta shifting once the dominant heroes change.

_234000 battles in 277s; 150 paired samples per hero per situation. Noise (one standard error): about ±4 points per cell, ±1.0 for a hero's overall uplift._

## Flags

- **Isaura** (arcane dps): DOMINANT. Overall +23 points; above her peers in 13 of 13 situations; best vs nature, worst vs control.
- **Solenne** (high dps): DOMINANT. Overall +20 points; above her peers in 13 of 13 situations; best vs dark, worst vs burst.
- **Ilyra** (high dps): DOMINANT. Overall +16 points; above her peers in 12 of 13 situations; best vs nature, worst own race.
- **Cassia** (high tank): DOMINANT. Overall +11 points; above her peers in 13 of 13 situations; best vs ocean, worst full package.
- **Coralie** (ocean tank): DOMINANT. Overall +11 points; above her peers in 12 of 13 situations; best vs ocean, worst full package.
- **Seren** (arcane support): NO NICHE. Overall -4 points; above her peers in 0 of 13 situations; best vs sustain, worst vs arcane.
- **Odette** (ocean healer): NO NICHE. Overall -4 points; above her peers in 0 of 13 situations; best full package, worst vs nature.
- **Hartwen** (nature tank): NO NICHE. Overall -5 points; above her peers in 0 of 13 situations; best vs ocean, worst random.
- **Zephyra** (nature support): NO NICHE. Overall -6 points; above her peers in 0 of 13 situations; best with partner, worst vs arcane.
- **Tempra** (arcane support): NO NICHE. Overall -6 points; above her peers in 0 of 13 situations; best random, worst own race.
- **Thalassa** (ocean tank): NO NICHE. Overall -7 points; above her peers in 0 of 13 situations; best vs dark, worst vs arcane.
- **Runa** (arcane tank): NO NICHE. Overall -7 points; above her peers in 0 of 13 situations; best vs nature, worst vs sustain.
- **Rosalind** (high support): NO NICHE. Overall -7 points; above her peers in 0 of 13 situations; best vs ocean, worst vs tanky.
- **Nerissa** (ocean dps): NO NICHE. Overall -7 points; above her peers in 0 of 13 situations; best vs tanky, worst vs dark.
- **Noctelle** (dark support): NO NICHE. Overall -8 points; above her peers in 0 of 13 situations; best vs ocean, worst vs dark.
- **Briar** (nature tank): NO NICHE. Overall -8 points; above her peers in 0 of 13 situations; best full package, worst random.
- **Elara** (high healer): NO NICHE. Overall -8 points; above her peers in 0 of 13 situations; best vs burst, worst vs ocean.
- **Wren** (nature support): NO NICHE. Overall -8 points; above her peers in 0 of 13 situations; best vs dark, worst own race.
- **Lucienne** (high dps): NO NICHE. Overall -9 points; above her peers in 0 of 13 situations; best vs control, worst own race.
- **Isolde** (high tank): NO NICHE. Overall -9 points; above her peers in 0 of 13 situations; best full package, worst vs nature.
- **Maelis** (dark dps): NO NICHE. Overall -11 points; above her peers in 0 of 13 situations; best vs burst, worst vs arcane.
- **Vesper** (dark healer): NO NICHE. Overall -12 points; above her peers in 0 of 13 situations; best own race, worst vs arcane.
- **Nyx** (dark dps): NO NICHE. Overall -17 points; above her peers in 0 of 13 situations; best vs tanky, worst own race.
- **Venna** (nature dps): NO NICHE. Overall -19 points; above her peers in 0 of 13 situations; best vs control, worst full package.
- **Caelith** (arcane dps): NO NICHE. Overall -23 points; above her peers in 0 of 13 situations; best vs control, worst own race.

## Tanks

| Hero | Race | Uplift overall | Best situation | Worst situation | Damage soaked share / survives | Verdict |
| --- | --- | --- | --- | --- | --- | --- |
| Cassia | high | +11 | vs ocean (+25) | full package (+6) | 35% / 54% | DOMINANT |
| Coralie | ocean | +11 | vs ocean (+19) | full package (+2) | 33% / 61% | DOMINANT |
| Valeria | high | +7 | vs dark (+14) | with partner (0) | 32% / 53% | niche |
| Sangrael | dark | +4 | vs ocean (+11) | vs arcane (-7) | 32% / 51% | niche |
| Vaela | arcane | +4 | vs nature (+9) | vs burst (-2) | 33% / 55% | niche |
| Eldrith | nature | +2 | vs nature (+12) | vs control (-7) | 44% / 57% | niche |
| Draxa | arcane | -1 | full package (+7) | vs ocean (-7) | 37% / 52% | niche |
| Corvina | dark | -1 | vs tanky (+6) | own race (-9) | 44% / 50% | niche |
| Hartwen | nature | -5 | vs ocean (+1) | random (-11) | 35% / 43% | NO NICHE |
| Thalassa | ocean | -7 | vs dark (-1) | vs arcane (-14) | 28% / 44% | NO NICHE |
| Runa | arcane | -7 | vs nature (-3) | vs sustain (-13) | 30% / 43% | NO NICHE |
| Briar | nature | -8 | full package (-1) | random (-13) | 26% / 38% | NO NICHE |
| Isolde | high | -9 | full package (+1) | vs nature (-18) | 27% / 41% | NO NICHE |

## Dpss

| Hero | Race | Uplift overall | Best situation | Worst situation | Team damage share | Verdict |
| --- | --- | --- | --- | --- | --- | --- |
| Isaura | arcane | +23 | vs nature (+33) | vs control (+8) | 39% | DOMINANT |
| Solenne | high | +20 | vs dark (+30) | vs burst (+8) | 30% | DOMINANT |
| Ilyra | high | +16 | vs nature (+32) | own race (0) | 47% | DOMINANT |
| Calypso | ocean | +9 | vs ocean (+17) | vs high (0) | 29% | niche |
| Ysra | ocean | +9 | vs burst (+20) | vs nature (-3) | 33% | niche |
| Ravenna | dark | +7 | own race (+24) | vs tanky (-3) | 40% | niche |
| Ophira | high | +6 | vs ocean (+15) | vs nature (-3) | 26% | niche |
| Astraea | arcane | +4 | vs high (+18) | vs ocean (-8) | 37% | niche |
| Zaria | arcane | +4 | vs burst (+12) | vs arcane (-3) | 39% | niche |
| Sable | dark | +3 | vs dark (+9) | vs nature (-4) | 30% | niche |
| Rhiannon | nature | +1 | vs sustain (+11) | vs nature (-10) | 36% | niche |
| Halcyra | ocean | -1 | with partner (+10) | own race (-9) | 37% | niche |
| Sylwen | nature | -2 | random (+7) | vs control (-11) | 31% | niche |
| Kaida | nature | -5 | full package (+4) | vs dark (-15) | 29% | niche |
| Nerissa | ocean | -7 | vs tanky (-2) | vs dark (-13) | 22% | NO NICHE |
| Lucienne | high | -9 | vs control (+1) | own race (-19) | 29% | NO NICHE |
| Maelis | dark | -11 | vs burst (-6) | vs arcane (-22) | 38% | NO NICHE |
| Nyx | dark | -17 | vs tanky (-10) | own race (-26) | 18% | NO NICHE |
| Venna | nature | -19 | vs control (-5) | full package (-28) | 32% | NO NICHE |
| Caelith | arcane | -23 | vs control (-11) | own race (-38) | 28% | NO NICHE |

## Healers

| Hero | Race | Uplift overall | Best situation | Worst situation | Heal / clutch heal per battle | Verdict |
| --- | --- | --- | --- | --- | --- | --- |
| Aurelle | high | +10 | vs burst (+14) | vs arcane (+6) | 7171 / 1281 | niche |
| Mireille | nature | +9 | vs nature (+19) | vs high (0) | 8311 / 1855 | niche |
| Siora | ocean | +9 | vs sustain (+14) | vs high (+3) | 5532 / 924 | niche |
| Selene | arcane | +3 | vs high (+9) | random (-4) | 4007 / 1796 | niche |
| Nimue | nature | +1 | vs high (+7) | own race (-13) | 2415 / 836 | niche |
| Thessaly | dark | +1 | random (+4) | vs high (-6) | 4312 / 1011 | niche |
| Lunaith | arcane | 0 | vs burst (+4) | vs nature (-5) | 5000 / 1100 | niche |
| Mordessa | dark | -1 | own race (+4) | vs tanky (-5) | 4443 / 881 | niche |
| Marisol | ocean | -2 | random (+3) | vs dark (-6) | 4804 / 930 | niche |
| Odette | ocean | -4 | full package (0) | vs nature (-12) | 990 / 308 | NO NICHE |
| Elara | high | -8 | vs burst (-4) | vs ocean (-13) | 3488 / 744 | NO NICHE |
| Vesper | dark | -12 | own race (-4) | vs arcane (-19) | 1889 / 422 | NO NICHE |

## Supports

| Hero | Race | Uplift overall | Best situation | Worst situation | Effects applied per battle | Verdict |
| --- | --- | --- | --- | --- | --- | --- |
| Fenna | nature | +9 | full package (+16) | vs ocean (+3) | 20.2 | niche |
| Elowen | arcane | +8 | vs arcane (+19) | with partner (-1) | 15.0 | niche |
| Seravelle | high | +7 | with partner (+15) | vs dark (-1) | 9.0 | niche |
| Lorelei | ocean | +6 | full package (+11) | vs control (+2) | 3.8 | niche |
| Velisande | dark | +5 | vs nature (+13) | vs ocean (-5) | 18.5 | niche |
| Liora | dark | +5 | with partner (+8) | vs arcane (-1) | 12.8 | niche |
| Ondine | ocean | +3 | with partner (+11) | vs nature (-4) | 10.4 | niche |
| Pelagia | ocean | -4 | vs nature (+5) | own race (-17) | 12.0 | niche |
| Seren | arcane | -4 | vs sustain (+1) | vs arcane (-10) | 13.3 | NO NICHE |
| Amarante | high | -5 | own race (+3) | vs burst (-10) | 35.2 | niche |
| Zephyra | nature | -6 | with partner (+2) | vs arcane (-14) | 8.1 | NO NICHE |
| Tempra | arcane | -6 | random (+1) | own race (-12) | 2.4 | NO NICHE |
| Rosalind | high | -7 | vs ocean (+2) | vs tanky (-12) | 12.2 | NO NICHE |
| Noctelle | dark | -8 | vs ocean (+1) | vs dark (-15) | 17.8 | NO NICHE |
| Wren | nature | -8 | vs dark (-3) | own race (-16) | 6.3 | NO NICHE |

## Uplift by situation (points versus a random same-role replacement)

| Hero | random | vs high | vs dark | vs nature | vs ocean | vs arcane | vs tanky | vs sustain | vs burst | vs control | with partner | full package | own race |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| Astraea | +9 | +18 | +2 | +8 | -8 | -1 | -1 | +7 | -1 | +3 | +2 | +2 | +14 |
| Caelith | -24 | -18 | -27 | -27 | -27 | -30 | -23 | -28 | -17 | -11 | -15 | -14 | -38 |
| Isaura | +22 | +22 | +21 | +33 | +13 | +16 | +24 | +26 | +19 | +8 | +31 | +28 | +31 |
| Zaria | +2 | +9 | +7 | +4 | -2 | -3 | -2 | +5 | +12 | +5 | +8 | +3 | +4 |
| Lunaith | -5 | +1 | 0 | -5 | -1 | 0 | +3 | -4 | +4 | -1 | 0 | +1 | 0 |
| Selene | -4 | +9 | +7 | -2 | +2 | +3 | 0 | +3 | +9 | +4 | +4 | -1 | 0 |
| Elowen | +10 | +6 | +15 | +7 | +11 | +19 | +10 | +6 | +11 | +3 | -1 | +2 | +10 |
| Seren | -4 | -3 | -4 | -6 | -4 | -10 | -8 | +1 | -1 | 0 | -3 | -3 | -8 |
| Tempra | +1 | -8 | -2 | -7 | -7 | -5 | -8 | -6 | -9 | -3 | -4 | -8 | -12 |
| Draxa | -1 | -2 | +2 | -5 | -7 | -2 | -1 | 0 | +2 | +2 | -4 | +7 | +2 |
| Runa | -10 | -3 | -4 | -3 | -6 | -4 | -11 | -13 | -6 | -6 | -8 | -5 | -11 |
| Vaela | +1 | +1 | +8 | +9 | +1 | +7 | +4 | +6 | -2 | +2 | +2 | +2 | +6 |
| Maelis | -12 | -12 | -14 | -8 | -11 | -22 | -7 | -13 | -6 | -7 | -12 | -13 | -8 |
| Nyx | -11 | -19 | -22 | -18 | -12 | -17 | -10 | -22 | -14 | -19 | -19 | -11 | -26 |
| Ravenna | +5 | +6 | +10 | +12 | -2 | +7 | -3 | +17 | +4 | +6 | +5 | +4 | +24 |
| Sable | +7 | +5 | +9 | -4 | +5 | -3 | -2 | +8 | -1 | +1 | -1 | +5 | +9 |
| Mordessa | -4 | -1 | -5 | +1 | +1 | 0 | -5 | 0 | -2 | -1 | +2 | +2 | +4 |
| Thessaly | +4 | -6 | +2 | 0 | +3 | -1 | -2 | -5 | +3 | +1 | +2 | +1 | +4 |
| Vesper | -11 | -9 | -6 | -16 | -11 | -19 | -19 | -13 | -13 | -9 | -16 | -13 | -4 |
| Liora | +7 | +6 | +1 | 0 | +5 | -1 | +5 | +8 | +7 | +1 | +8 | +8 | +7 |
| Noctelle | -5 | -4 | -15 | -8 | +1 | -15 | 0 | -10 | -5 | -6 | -7 | -12 | -15 |
| Velisande | +5 | +2 | +8 | +13 | -5 | +1 | +12 | +3 | +3 | +3 | +8 | +9 | +2 |
| Corvina | -1 | -2 | -2 | -1 | -3 | +3 | +6 | -4 | +4 | +1 | 0 | 0 | -9 |
| Sangrael | +1 | +10 | +4 | +8 | +11 | -7 | +8 | -3 | +1 | +4 | +6 | +11 | +5 |
| Ilyra | +10 | +9 | +29 | +32 | +13 | +6 | +21 | +13 | +15 | +13 | +19 | +23 | 0 |
| Lucienne | -10 | -5 | -9 | -14 | -4 | -15 | -12 | -4 | -3 | +1 | -10 | -16 | -19 |
| Ophira | +9 | +1 | +4 | -3 | +15 | +9 | +8 | +3 | +12 | -1 | +8 | +10 | +5 |
| Solenne | +18 | +20 | +30 | +24 | +19 | +19 | +23 | +24 | +8 | +12 | +26 | +27 | +10 |
| Aurelle | +8 | +8 | +9 | +11 | +7 | +6 | +10 | +11 | +14 | +9 | +13 | +8 | +13 |
| Elara | -6 | -9 | -5 | -8 | -13 | -11 | -5 | -9 | -4 | -8 | -13 | -6 | -11 |
| Amarante | -8 | -4 | -8 | -5 | -7 | -3 | -6 | -6 | -10 | -6 | +1 | -7 | +3 |
| Rosalind | -6 | -8 | -7 | -6 | +2 | -7 | -12 | -8 | -9 | -4 | -8 | -12 | -4 |
| Seravelle | +5 | +3 | -1 | +9 | +6 | +1 | +10 | +7 | +13 | +6 | +15 | +3 | +13 |
| Cassia | +11 | +10 | +7 | +9 | +25 | +8 | +15 | +8 | +12 | +11 | +13 | +6 | +10 |
| Isolde | -8 | -3 | -4 | -18 | -10 | -6 | -5 | -17 | -18 | -13 | -9 | +1 | -13 |
| Valeria | +5 | +13 | +14 | +5 | +9 | +9 | +10 | +2 | +4 | +11 | 0 | +4 | +6 |
| Kaida | -9 | -13 | -15 | -2 | -6 | -5 | -5 | -3 | -11 | -5 | -3 | +4 | +2 |
| Rhiannon | +2 | +6 | -4 | -10 | +2 | +1 | -5 | +11 | -1 | +6 | 0 | +1 | +9 |
| Sylwen | +7 | +1 | -5 | -4 | -1 | -4 | -4 | -2 | -8 | -11 | -3 | 0 | +4 |
| Venna | -20 | -19 | -17 | -23 | -18 | -24 | -15 | -17 | -21 | -5 | -21 | -28 | -16 |
| Mireille | +7 | 0 | +8 | +19 | +13 | +14 | +11 | +10 | +6 | 0 | +2 | +13 | +13 |
| Nimue | 0 | +7 | -6 | +6 | +7 | +2 | -2 | +5 | +5 | +4 | +7 | -7 | -13 |
| Fenna | +7 | +13 | +9 | +15 | +3 | +9 | +10 | +6 | +13 | +5 | +8 | +16 | +8 |
| Wren | -7 | -4 | -3 | -11 | -12 | -12 | -13 | -9 | -11 | -5 | -3 | -4 | -16 |
| Zephyra | -11 | -8 | -1 | -6 | 0 | -14 | -5 | -5 | -9 | -7 | +2 | -9 | -4 |
| Briar | -13 | -8 | -8 | -10 | -8 | -4 | -8 | -11 | -10 | -8 | -10 | -1 | -10 |
| Eldrith | +7 | -3 | +1 | +12 | -6 | +3 | +4 | +1 | +3 | -7 | -1 | +3 | +9 |
| Hartwen | -11 | -9 | -10 | -9 | +1 | -2 | -3 | -2 | -7 | -5 | -7 | -6 | 0 |
| Calypso | +11 | 0 | +5 | +5 | +17 | +14 | +15 | +3 | +11 | +5 | +12 | +15 | +3 |
| Halcyra | -2 | -4 | -4 | +2 | -2 | +3 | -4 | 0 | -1 | -3 | +10 | +6 | -9 |
| Nerissa | -3 | -11 | -13 | -9 | -9 | -3 | -2 | -7 | -6 | -5 | -8 | -7 | -10 |
| Ysra | +9 | +8 | +15 | -3 | +6 | +12 | +15 | +2 | +20 | +12 | +3 | +5 | +8 |
| Marisol | +3 | -4 | -6 | -4 | -3 | +3 | -2 | -5 | -4 | -4 | +3 | -2 | -1 |
| Odette | -6 | -2 | 0 | -12 | -6 | -3 | -6 | -7 | -6 | -1 | -4 | 0 | -6 |
| Siora | +11 | +3 | +10 | +6 | +6 | +13 | +7 | +14 | +7 | +10 | +8 | +10 | +10 |
| Lorelei | +7 | +4 | +9 | +3 | +8 | +7 | +6 | +2 | +10 | +2 | +4 | +11 | +8 |
| Ondine | -4 | +5 | +5 | -4 | +1 | +1 | +5 | +9 | 0 | +2 | +11 | +10 | +3 |
| Pelagia | -6 | -5 | -3 | +5 | -8 | 0 | -5 | +1 | -6 | +2 | -4 | -3 | -17 |
| Coralie | +9 | +11 | +10 | +5 | +19 | +16 | +7 | +7 | +14 | +9 | +14 | +2 | +16 |
| Thalassa | -3 | -10 | -1 | -5 | -4 | -14 | -10 | -8 | -11 | -8 | -2 | -2 | -6 |

## Races

| Race | Win % of its heroes in single-race teams | Uplift of its heroes vs each race (high / dark / nature / ocean / arcane) |
| --- | --- | --- |
| high | +1 uplift | +3 / +5 / +3 / +5 / +1 |
| dark | -1 uplift | -2 / -3 / -2 / -1 / -6 |
| nature | -1 uplift | -3 / -4 / -2 / -2 / -3 |
| ocean | 0 uplift | 0 / +2 / -1 / +2 / +4 |
| arcane | 0 uplift | +3 / +2 / +1 / -3 / -1 |
