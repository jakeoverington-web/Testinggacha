# Niche report v1 (battle rules v1, 2026-10-02)

Balance target (decisions row 29): every hero is the best pick somewhere; no hero is the best pick everywhere. Uplift = points a hero adds to her team's result versus a random same-role replacement, same opponent and seed. DOMINANT = overall ≥ +10 and above peers in ≥ 10 of 12 situations. NO NICHE = never more than +2 above peers in any situation. Reproduce: `tools/csharp/test.sh -Main Gacha.Tests.Tools.NicheReport 150 1`.

_216000 battles in 156s; 150 paired samples per hero per situation. Noise (one standard error): about ±4 points per cell, ±1.1 for a hero's overall uplift._

## Flags

- **Isaura** (arcane dps): DOMINANT. Overall +22 points; above her peers in 12 of 12 situations; best vs tanky, worst vs control.
- **Solenne** (high dps): DOMINANT. Overall +17 points; above her peers in 12 of 12 situations; best vs tanky, worst vs burst.
- **Ilyra** (high dps): DOMINANT. Overall +16 points; above her peers in 11 of 12 situations; best vs nature, worst vs control.
- **Calypso** (ocean dps): DOMINANT. Overall +14 points; above her peers in 12 of 12 situations; best vs burst, worst vs high.
- **Cassia** (high tank): DOMINANT. Overall +11 points; above her peers in 11 of 12 situations; best with partner, worst vs nature.
- **Astraea** (arcane dps): DOMINANT. Overall +11 points; above her peers in 11 of 12 situations; best vs high, worst vs ocean.
- **Fenna** (nature support): DOMINANT. Overall +11 points; above her peers in 12 of 12 situations; best vs high, worst vs dark.
- **Marisol** (ocean healer): NO NICHE. Overall -3 points; above her peers in 0 of 12 situations; best vs dark, worst vs burst.
- **Noctelle** (dark support): NO NICHE. Overall -4 points; above her peers in 0 of 12 situations; best vs dark, worst vs nature.
- **Seren** (arcane support): NO NICHE. Overall -5 points; above her peers in 0 of 12 situations; best vs burst, worst vs high.
- **Briar** (nature tank): NO NICHE. Overall -6 points; above her peers in 0 of 12 situations; best vs dark, worst vs ocean.
- **Thalassa** (ocean tank): NO NICHE. Overall -6 points; above her peers in 0 of 12 situations; best vs sustain, worst own race.
- **Rosalind** (high support): NO NICHE. Overall -6 points; above her peers in 0 of 12 situations; best vs high, worst vs tanky.
- **Zephyra** (nature support): NO NICHE. Overall -7 points; above her peers in 0 of 12 situations; best vs dark, worst vs burst.
- **Amarante** (high support): NO NICHE. Overall -7 points; above her peers in 0 of 12 situations; best vs burst, worst vs dark.
- **Kaida** (nature dps): NO NICHE. Overall -8 points; above her peers in 0 of 12 situations; best vs arcane, worst vs dark.
- **Wren** (nature support): NO NICHE. Overall -8 points; above her peers in 0 of 12 situations; best vs arcane, worst vs tanky.
- **Elara** (high healer): NO NICHE. Overall -9 points; above her peers in 0 of 12 situations; best vs sustain, worst vs high.
- **Maelis** (dark dps): NO NICHE. Overall -9 points; above her peers in 0 of 12 situations; best vs nature, worst vs dark.
- **Lucienne** (high dps): NO NICHE. Overall -9 points; above her peers in 0 of 12 situations; best vs ocean, worst vs nature.
- **Isolde** (high tank): NO NICHE. Overall -12 points; above her peers in 0 of 12 situations; best vs tanky, worst vs dark.
- **Nerissa** (ocean dps): NO NICHE. Overall -13 points; above her peers in 0 of 12 situations; best vs high, worst vs sustain.
- **Vesper** (dark healer): NO NICHE. Overall -15 points; above her peers in 0 of 12 situations; best random, worst vs ocean.
- **Venna** (nature dps): NO NICHE. Overall -21 points; above her peers in 0 of 12 situations; best vs control, worst own race.
- **Caelith** (arcane dps): NO NICHE. Overall -21 points; above her peers in 0 of 12 situations; best vs high, worst vs sustain.
- **Nyx** (dark dps): NO NICHE. Overall -22 points; above her peers in 0 of 12 situations; best vs tanky, worst vs sustain.

## Tanks

| Hero | Race | Uplift overall | Best situation | Worst situation | Damage soaked share / survives | Verdict |
| --- | --- | --- | --- | --- | --- | --- |
| Cassia | high | +11 | with partner (+18) | vs nature (+2) | 35% / 52% | DOMINANT |
| Sangrael | dark | +7 | vs nature (+11) | vs sustain (+1) | 33% / 51% | niche |
| Coralie | ocean | +6 | vs sustain (+11) | random (+1) | 33% / 56% | niche |
| Valeria | high | +5 | vs burst (+14) | with partner (-2) | 32% / 49% | niche |
| Draxa | arcane | +3 | vs tanky (+12) | vs ocean (-7) | 34% / 51% | niche |
| Vaela | arcane | +2 | vs dark (+5) | vs high (-3) | 32% / 50% | niche |
| Corvina | dark | +0 | vs tanky (+8) | vs ocean (-6) | 45% / 48% | niche |
| Hartwen | nature | -2 | own race (+8) | vs dark (-8) | 34% / 42% | niche |
| Eldrith | nature | -2 | vs nature (+5) | vs tanky (-7) | 41% / 49% | niche |
| Runa | arcane | -4 | vs arcane (+3) | vs sustain (-11) | 31% / 43% | niche |
| Briar | nature | -6 | vs dark (+1) | vs ocean (-14) | 28% / 40% | NO NICHE |
| Thalassa | ocean | -6 | vs sustain (+1) | own race (-11) | 28% / 39% | NO NICHE |
| Isolde | high | -12 | vs tanky (+0) | vs dark (-21) | 28% / 36% | NO NICHE |

## Dpss

| Hero | Race | Uplift overall | Best situation | Worst situation | Team damage share | Verdict |
| --- | --- | --- | --- | --- | --- | --- |
| Isaura | arcane | +22 | vs tanky (+30) | vs control (+10) | 41% | DOMINANT |
| Solenne | high | +17 | vs tanky (+29) | vs burst (+5) | 31% | DOMINANT |
| Ilyra | high | +16 | vs nature (+36) | vs control (+2) | 47% | DOMINANT |
| Calypso | ocean | +14 | vs burst (+26) | vs high (+6) | 29% | DOMINANT |
| Astraea | arcane | +11 | vs high (+21) | vs ocean (-1) | 39% | DOMINANT |
| Ysra | ocean | +10 | vs arcane (+20) | vs nature (-1) | 33% | niche |
| Ravenna | dark | +6 | own race (+18) | vs tanky (-2) | 37% | niche |
| Zaria | arcane | +5 | vs nature (+15) | vs tanky (-2) | 39% | niche |
| Rhiannon | nature | +5 | own race (+21) | vs nature (-7) | 37% | niche |
| Halcyra | ocean | +3 | vs dark (+10) | vs ocean (-4) | 37% | niche |
| Sylwen | nature | +1 | vs tanky (+8) | vs high (-2) | 33% | niche |
| Sable | dark | -1 | vs control (+5) | vs arcane (-11) | 29% | niche |
| Ophira | high | -6 | vs burst (+6) | vs nature (-18) | 21% | niche |
| Kaida | nature | -8 | vs arcane (+1) | vs dark (-15) | 27% | NO NICHE |
| Maelis | dark | -9 | vs nature (+2) | vs dark (-16) | 37% | NO NICHE |
| Lucienne | high | -9 | vs ocean (+2) | vs nature (-18) | 26% | NO NICHE |
| Nerissa | ocean | -13 | vs high (-7) | vs sustain (-21) | 20% | NO NICHE |
| Venna | nature | -21 | vs control (-13) | own race (-28) | 29% | NO NICHE |
| Caelith | arcane | -21 | vs high (-14) | vs sustain (-29) | 28% | NO NICHE |
| Nyx | dark | -22 | vs tanky (-8) | vs sustain (-32) | 16% | NO NICHE |

## Healers

| Hero | Race | Uplift overall | Best situation | Worst situation | Heal / clutch heal per battle | Verdict |
| --- | --- | --- | --- | --- | --- | --- |
| Aurelle | high | +10 | own race (+15) | random (+5) | 3669 / 859 | niche |
| Siora | ocean | +8 | vs arcane (+14) | vs high (+3) | 2561 / 556 | niche |
| Selene | arcane | +5 | vs dark (+10) | with partner (-3) | 2293 / 1100 | niche |
| Nimue | nature | +4 | vs ocean (+12) | vs dark (-2) | 1195 / 448 | niche |
| Mireille | nature | +2 | vs arcane (+8) | vs high (-3) | 2513 / 905 | niche |
| Thessaly | dark | +1 | own race (+8) | vs tanky (-6) | 2137 / 632 | niche |
| Lunaith | arcane | +0 | random (+12) | vs nature (-7) | 2326 / 640 | niche |
| Odette | ocean | -3 | vs control (+5) | vs burst (-8) | 596 / 224 | niche |
| Marisol | ocean | -3 | vs dark (+1) | vs burst (-8) | 2117 / 521 | NO NICHE |
| Mordessa | dark | -4 | vs nature (+3) | vs sustain (-9) | 1932 / 505 | niche |
| Elara | high | -9 | vs sustain (-1) | vs high (-15) | 1468 / 409 | NO NICHE |
| Vesper | dark | -15 | random (-12) | vs ocean (-20) | 448 / 122 | NO NICHE |

## Supports

| Hero | Race | Uplift overall | Best situation | Worst situation | Effects applied per battle | Verdict |
| --- | --- | --- | --- | --- | --- | --- |
| Fenna | nature | +11 | vs high (+16) | vs dark (+3) | 12.2 | DOMINANT |
| Elowen | arcane | +9 | vs tanky (+15) | vs control (+3) | 9.0 | niche |
| Liora | dark | +7 | vs burst (+14) | vs nature (-2) | 7.5 | niche |
| Lorelei | ocean | +6 | vs burst (+17) | vs nature (-4) | 2.3 | niche |
| Seravelle | high | +5 | random (+12) | vs dark (-2) | 5.0 | niche |
| Ondine | ocean | +3 | vs burst (+9) | vs ocean (-4) | 6.4 | niche |
| Velisande | dark | +2 | vs tanky (+12) | vs ocean (-13) | 11.0 | niche |
| Tempra | arcane | -2 | random (+5) | vs tanky (-6) | 1.7 | niche |
| Noctelle | dark | -4 | vs dark (+1) | vs nature (-14) | 10.2 | NO NICHE |
| Pelagia | ocean | -4 | vs control (+3) | own race (-15) | 6.9 | niche |
| Seren | arcane | -5 | vs burst (+0) | vs high (-11) | 8.0 | NO NICHE |
| Rosalind | high | -6 | vs high (-+0) | vs tanky (-12) | 7.0 | NO NICHE |
| Zephyra | nature | -7 | vs dark (-2) | vs burst (-16) | 4.5 | NO NICHE |
| Amarante | high | -7 | vs burst (-1) | vs dark (-11) | 20.4 | NO NICHE |
| Wren | nature | -8 | vs arcane (+2) | vs tanky (-18) | 3.8 | NO NICHE |

## Uplift by situation (points versus a random same-role replacement)

| Hero | random | vs high | vs dark | vs nature | vs ocean | vs arcane | vs tanky | vs sustain | vs burst | vs control | with partner | own race |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| Astraea | +20 | +21 | +5 | +21 | -1 | +8 | +8 | +17 | +9 | +7 | +12 | +7 |
| Caelith | -25 | -14 | -26 | -21 | -20 | -25 | -16 | -29 | -21 | -21 | -15 | -24 |
| Isaura | +20 | +25 | +20 | +23 | +13 | +25 | +30 | +24 | +19 | +10 | +29 | +22 |
| Zaria | +4 | +5 | +10 | +15 | +3 | -1 | -2 | +1 | +8 | +4 | +6 | +3 |
| Lunaith | +12 | -1 | +2 | -7 | +0 | +1 | +2 | -6 | +3 | -3 | -1 | +1 |
| Selene | +4 | +7 | +10 | +8 | +4 | +5 | -1 | +6 | +2 | +4 | -3 | +8 |
| Elowen | +9 | +12 | +10 | +11 | +6 | +11 | +15 | +4 | +8 | +3 | +9 | +13 |
| Seren | -8 | -11 | -9 | -1 | -4 | -3 | -5 | -7 | +0 | -2 | -3 | -9 |
| Tempra | +5 | -4 | +2 | -5 | -3 | +1 | -6 | -3 | +0 | -3 | +1 | -5 |
| Draxa | +9 | +1 | +5 | +3 | -7 | +8 | +12 | -1 | -+0 | +6 | +6 | -5 |
| Runa | -1 | -4 | -2 | -5 | -6 | +3 | -4 | -11 | -7 | -1 | -3 | -7 |
| Vaela | +5 | -3 | +5 | +5 | +1 | -3 | +1 | +2 | +2 | +3 | +2 | +5 |
| Maelis | -8 | -13 | -16 | +2 | -11 | -15 | -12 | -15 | -5 | -3 | -7 | -1 |
| Nyx | -16 | -25 | -32 | -24 | -14 | -21 | -8 | -32 | -11 | -28 | -26 | -25 |
| Ravenna | +1 | +8 | +1 | +11 | +3 | +7 | -2 | +13 | -1 | +5 | +4 | +18 |
| Sable | -4 | -3 | +4 | -+0 | +3 | -11 | -5 | +1 | -3 | +5 | +4 | +2 |
| Mordessa | -5 | -1 | -8 | +3 | -4 | -6 | -8 | -9 | -3 | -6 | +1 | +2 |
| Thessaly | +4 | -1 | -1 | +7 | +1 | +3 | -6 | -3 | +3 | -3 | +5 | +8 |
| Vesper | -12 | -16 | -19 | -19 | -20 | -13 | -14 | -17 | -12 | -17 | -14 | -13 |
| Liora | +8 | +5 | +10 | -2 | +11 | +2 | +5 | +3 | +14 | +5 | +10 | +7 |
| Noctelle | -6 | -2 | +1 | -14 | +0 | -6 | -1 | -3 | -+0 | -5 | -2 | -9 |
| Velisande | +5 | +0 | +7 | +8 | -13 | +1 | +12 | +5 | -2 | -2 | +0 | +1 |
| Corvina | +2 | -3 | +5 | +5 | -6 | +0 | +8 | +0 | -+0 | -5 | -2 | -3 |
| Sangrael | +7 | +5 | +8 | +11 | +11 | +4 | +6 | +1 | +7 | +8 | +7 | +10 |
| Ilyra | +16 | +14 | +33 | +36 | +11 | +3 | +17 | +17 | +13 | +2 | +16 | +11 |
| Lucienne | -8 | -8 | -8 | -18 | +2 | -17 | -12 | -10 | -1 | -4 | -8 | -13 |
| Ophira | -11 | -8 | +0 | -18 | -3 | +0 | -3 | -9 | +6 | -11 | -6 | -13 |
| Solenne | +19 | +10 | +26 | +22 | +12 | +10 | +29 | +20 | +5 | +11 | +20 | +17 |
| Aurelle | +5 | +14 | +11 | +8 | +8 | +8 | +14 | +7 | +9 | +10 | +8 | +15 |
| Elara | -10 | -15 | -5 | -8 | -8 | -7 | -9 | -1 | -12 | -9 | -3 | -14 |
| Amarante | -6 | -2 | -11 | -9 | -5 | -7 | -6 | -6 | -1 | -10 | -9 | -8 |
| Rosalind | -9 | -+0 | -4 | -7 | -5 | -9 | -12 | -11 | -3 | -2 | -7 | -8 |
| Seravelle | +12 | +2 | -2 | +6 | +5 | +5 | +4 | +3 | +12 | +4 | +6 | +8 |
| Cassia | +10 | +13 | +13 | +2 | +14 | +9 | +11 | +11 | +12 | +11 | +18 | +12 |
| Isolde | -14 | -13 | -21 | -17 | -6 | -9 | +0 | -19 | -6 | -11 | -11 | -14 |
| Valeria | +2 | +4 | +8 | +5 | +5 | +0 | +7 | +5 | +14 | +6 | -2 | +1 |
| Kaida | -15 | +0 | -15 | -3 | -5 | +1 | -11 | -8 | -9 | -12 | -10 | -4 |
| Rhiannon | +6 | +6 | -4 | -7 | +7 | +13 | -7 | +12 | +8 | +3 | -2 | +21 |
| Sylwen | +4 | -2 | -2 | +3 | +1 | +3 | +8 | -2 | -+0 | +1 | -1 | +2 |
| Venna | -18 | -25 | -26 | -18 | -16 | -18 | -23 | -21 | -22 | -13 | -25 | -28 |
| Mireille | -2 | -3 | +0 | +8 | +2 | +8 | +2 | +7 | -1 | +0 | +1 | +1 |
| Nimue | +1 | +3 | -2 | +9 | +12 | +6 | +1 | +9 | +6 | +5 | +1 | -1 |
| Fenna | +13 | +16 | +3 | +15 | +11 | +12 | +11 | +8 | +13 | +11 | +6 | +9 |
| Wren | -5 | -5 | -9 | -9 | -8 | +2 | -18 | -9 | -6 | -7 | -12 | -8 |
| Zephyra | -7 | -5 | -2 | -12 | -7 | -10 | -4 | -10 | -16 | -2 | -3 | -3 |
| Briar | -2 | -7 | +1 | -8 | -14 | -1 | -5 | -6 | -6 | -4 | -10 | -5 |
| Eldrith | +5 | -1 | -4 | +5 | -2 | -6 | -7 | -+0 | -2 | -2 | -7 | +1 |
| Hartwen | +1 | -2 | -8 | -3 | +1 | +2 | -4 | -3 | -3 | -+0 | -6 | +8 |
| Calypso | +11 | +6 | +11 | +10 | +18 | +25 | +10 | +10 | +26 | +10 | +21 | +7 |
| Halcyra | +5 | +1 | +10 | +7 | -4 | +8 | +4 | +2 | +2 | -1 | +4 | +0 |
| Nerissa | -12 | -7 | -16 | -18 | -9 | -12 | -15 | -21 | -12 | -8 | -13 | -15 |
| Ysra | +8 | +16 | +18 | -1 | +0 | +20 | +17 | +0 | +16 | +9 | +12 | +6 |
| Marisol | -1 | +0 | +1 | -6 | -2 | +0 | -6 | -3 | -8 | -5 | -3 | -5 |
| Odette | +1 | +1 | -2 | -4 | -3 | -5 | -5 | -6 | -8 | +5 | -3 | -4 |
| Siora | +3 | +3 | +11 | +7 | +11 | +14 | +6 | +10 | +8 | +6 | +5 | +7 |
| Lorelei | +7 | +5 | +5 | -4 | +6 | +2 | +2 | +8 | +17 | +9 | +6 | +10 |
| Ondine | +0 | +7 | +5 | +1 | -4 | +4 | +4 | +4 | +9 | +4 | +8 | -2 |
| Pelagia | -3 | +0 | +1 | -13 | -5 | +1 | -8 | -4 | -7 | +3 | -3 | -15 |
| Coralie | +1 | +3 | +8 | +8 | +9 | +2 | +3 | +11 | +6 | +6 | +3 | +9 |
| Thalassa | -9 | -9 | -6 | -4 | -5 | -5 | -9 | +1 | -7 | -6 | -5 | -11 |

## Races

| Race | Win % of its heroes in single-race teams | Uplift of its heroes vs each race (high / dark / nature / ocean / arcane) |
| --- | --- | --- |
| high | -1 uplift | +1 / +3 / +0 / +3 / -1 |
| dark | -+0 uplift | -4 / -3 / -1 / -3 / -5 |
| nature | -+0 uplift | -2 / -6 / -2 / -2 / +1 |
| ocean | -1 uplift | +2 / +4 / -1 / +1 / +4 |
| arcane | +1 uplift | +3 / +3 / +4 / -1 / +3 |
