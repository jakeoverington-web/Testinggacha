# Niche report v8 (after roster pass 10, 2026-10-03)

Reproduce: `tools/csharp/test.sh -Main Gacha.Tests.Tools.NicheReport 150 1`.

**All 60 heroes within the ±8 band** (v7: one outside). Outside ±5: 21. Lowest: Wren -8, Isolde, Kaida, Rosalind, Seren, Vesper -7. Highest: Siora +8, Calypso, Cassia, Elowen, Ravenna, Sangrael +7.

_234000 battles in 228s; 150 paired samples per hero per situation. Noise (one standard error): about ±4 points per cell, ±1.0 for a hero's overall uplift._

## Flags

- **Elara** (high healer): NO NICHE. Overall -2 points; above her peers in 0 of 13 situations; best full package, worst vs arcane.
- **Marisol** (ocean healer): NO NICHE. Overall -4 points; above her peers in 0 of 13 situations; best vs dark, worst vs burst.
- **Lucienne** (high dps): NO NICHE. Overall -4 points; above her peers in 0 of 13 situations; best vs control, worst with partner.
- **Amarante** (high support): NO NICHE. Overall -4 points; above her peers in 0 of 13 situations; best own race, worst with partner.
- **Odette** (ocean healer): NO NICHE. Overall -5 points; above her peers in 0 of 13 situations; best full package, worst vs sustain.
- **Mordessa** (dark healer): NO NICHE. Overall -5 points; above her peers in 0 of 13 situations; best vs arcane, worst vs sustain.
- **Nyx** (dark dps): NO NICHE. Overall -5 points; above her peers in 0 of 13 situations; best vs nature, worst vs high.
- **Halcyra** (ocean dps): NO NICHE. Overall -6 points; above her peers in 0 of 13 situations; best vs arcane, worst vs sustain.
- **Zephyra** (nature support): NO NICHE. Overall -6 points; above her peers in 0 of 13 situations; best vs tanky, worst random.
- **Nerissa** (ocean dps): NO NICHE. Overall -6 points; above her peers in 0 of 13 situations; best vs ocean, worst vs sustain.
- **Seren** (arcane support): NO NICHE. Overall -7 points; above her peers in 0 of 13 situations; best vs sustain, worst own race.
- **Rosalind** (high support): NO NICHE. Overall -7 points; above her peers in 0 of 13 situations; best vs ocean, worst vs sustain.
- **Vesper** (dark healer): NO NICHE. Overall -7 points; above her peers in 0 of 13 situations; best own race, worst full package.
- **Wren** (nature support): NO NICHE. Overall -8 points; above her peers in 0 of 13 situations; best vs high, worst with partner.

## Tanks

| Hero | Race | Uplift overall | Best situation | Worst situation | Damage soaked share / survives | Verdict |
| --- | --- | --- | --- | --- | --- | --- |
| Cassia | high | +7 | full package (+18) | vs arcane (+1) | 33% / 42% | niche |
| Sangrael | dark | +7 | own race (+22) | vs arcane (-2) | 31% / 44% | niche |
| Vaela | arcane | +6 | own race (+13) | vs dark (-1) | 30% / 44% | niche |
| Coralie | ocean | +5 | own race (+13) | vs burst (-1) | 32% / 47% | niche |
| Valeria | high | +5 | vs burst (+8) | vs arcane (-1) | 31% / 44% | niche |
| Draxa | arcane | +2 | vs control (+9) | vs dark (-5) | 35% / 46% | niche |
| Corvina | dark | -1 | own race (+10) | with partner (-8) | 43% / 42% | niche |
| Eldrith | nature | -2 | own race (+3) | vs dark (-8) | 42% / 44% | niche |
| Hartwen | nature | -3 | own race (+9) | vs tanky (-10) | 29% / 35% | niche |
| Runa | arcane | -4 | vs arcane (+5) | vs dark (-11) | 31% / 37% | niche |
| Briar | nature | -5 | full package (+2) | vs burst (-16) | 27% / 37% | niche |
| Thalassa | ocean | -6 | own race (+7) | random (-14) | 27% / 33% | niche |
| Isolde | high | -7 | full package (+2) | vs burst (-15) | 28% / 35% | niche |

## Dpss

| Hero | Race | Uplift overall | Best situation | Worst situation | Team damage share | Verdict |
| --- | --- | --- | --- | --- | --- | --- |
| Calypso | ocean | +7 | with partner (+17) | vs sustain (-3) | 28% | niche |
| Ravenna | dark | +7 | vs sustain (+21) | vs dark (-2) | 43% | niche |
| Solenne | high | +6 | vs sustain (+16) | with partner (-3) | 27% | niche |
| Ilyra | high | +6 | full package (+14) | vs arcane (-5) | 44% | niche |
| Ophira | high | +5 | vs ocean (+14) | random (-4) | 26% | niche |
| Maelis | dark | +3 | vs burst (+16) | own race (-5) | 31% | niche |
| Ysra | ocean | +3 | vs burst (+13) | with partner (-7) | 31% | niche |
| Zaria | arcane | +2 | with partner (+13) | vs tanky (-11) | 39% | niche |
| Isaura | arcane | +2 | full package (+11) | vs nature (-4) | 31% | niche |
| Astraea | arcane | +2 | vs burst (+8) | vs ocean (-7) | 37% | niche |
| Sable | dark | 0 | own race (+9) | vs dark (-6) | 31% | niche |
| Rhiannon | nature | 0 | own race (+16) | vs nature (-7) | 38% | niche |
| Sylwen | nature | -1 | vs arcane (+6) | vs burst (-6) | 34% | niche |
| Caelith | arcane | -3 | vs high (+5) | own race (-8) | 24% | niche |
| Lucienne | high | -4 | vs control (+2) | with partner (-11) | 34% | NO NICHE |
| Nyx | dark | -5 | vs nature (+2) | vs high (-13) | 26% | NO NICHE |
| Halcyra | ocean | -6 | vs arcane (-1) | vs sustain (-11) | 35% | NO NICHE |
| Venna | nature | -6 | vs tanky (+10) | vs sustain (-17) | 34% | niche |
| Nerissa | ocean | -6 | vs ocean (0) | vs sustain (-12) | 25% | NO NICHE |
| Kaida | nature | -7 | vs nature (+2) | with partner (-12) | 32% | niche |

## Healers

| Hero | Race | Uplift overall | Best situation | Worst situation | Heal / clutch heal per battle | Verdict |
| --- | --- | --- | --- | --- | --- | --- |
| Siora | ocean | +8 | vs tanky (+14) | vs ocean (+2) | 3824 / 675 | niche |
| Aurelle | high | +3 | vs control (+6) | vs nature (0) | 4898 / 960 | niche |
| Nimue | nature | +3 | vs sustain (+9) | full package (-7) | 1660 / 575 | niche |
| Selene | arcane | +3 | vs dark (+6) | with partner (-3) | 3391 / 1596 | niche |
| Mireille | nature | +3 | full package (+12) | vs high (-6) | 4660 / 1280 | niche |
| Lunaith | arcane | +1 | full package (+5) | own race (-3) | 3539 / 737 | niche |
| Thessaly | dark | +1 | own race (+10) | with partner (-5) | 3080 / 745 | niche |
| Elara | high | -2 | full package (+2) | vs arcane (-7) | 3830 / 818 | NO NICHE |
| Marisol | ocean | -4 | vs dark (+2) | vs burst (-15) | 3126 / 653 | NO NICHE |
| Odette | ocean | -5 | full package (+1) | vs sustain (-11) | 777 / 247 | NO NICHE |
| Mordessa | dark | -5 | vs arcane (+1) | vs sustain (-8) | 2756 / 619 | NO NICHE |
| Vesper | dark | -7 | own race (-1) | full package (-14) | 2307 / 609 | NO NICHE |

## Supports

| Hero | Race | Uplift overall | Best situation | Worst situation | Effects applied per battle | Verdict |
| --- | --- | --- | --- | --- | --- | --- |
| Elowen | arcane | +7 | vs tanky (+17) | vs dark (-2) | 13.1 | niche |
| Fenna | nature | +6 | with partner (+15) | vs arcane (-1) | 16.1 | niche |
| Lorelei | ocean | +5 | with partner (+12) | vs tanky (-2) | 14.7 | niche |
| Ondine | ocean | +5 | full package (+17) | vs sustain (-1) | 8.0 | niche |
| Liora | dark | +5 | with partner (+14) | random (-6) | 11.6 | niche |
| Tempra | arcane | +4 | full package (+14) | vs high (-2) | 2.1 | niche |
| Seravelle | high | +4 | own race (+11) | vs arcane (-2) | 7.4 | niche |
| Velisande | dark | +1 | with partner (+7) | vs ocean (-5) | 14.9 | niche |
| Noctelle | dark | -2 | full package (+4) | vs burst (-8) | 13.3 | niche |
| Pelagia | ocean | -3 | vs control (+6) | own race (-16) | 9.3 | niche |
| Amarante | high | -4 | own race (+1) | with partner (-11) | 27.7 | NO NICHE |
| Zephyra | nature | -6 | vs tanky (-2) | random (-16) | 6.7 | NO NICHE |
| Seren | arcane | -7 | vs sustain (0) | own race (-15) | 10.6 | NO NICHE |
| Rosalind | high | -7 | vs ocean (+1) | vs sustain (-16) | 10.0 | NO NICHE |
| Wren | nature | -8 | vs high (+2) | with partner (-22) | 4.8 | NO NICHE |

## Uplift by situation (points versus a random same-role replacement)

| Hero | random | vs high | vs dark | vs nature | vs ocean | vs arcane | vs tanky | vs sustain | vs burst | vs control | with partner | full package | own race |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| Astraea | +7 | +8 | +1 | +1 | -7 | +3 | -3 | +2 | +8 | 0 | -6 | +5 | +2 |
| Caelith | 0 | +5 | -4 | -1 | -7 | -7 | 0 | -2 | -2 | -1 | -5 | -6 | -8 |
| Isaura | +2 | -1 | +4 | -4 | +1 | +2 | +5 | -1 | +8 | -1 | +5 | +11 | -4 |
| Zaria | +7 | +1 | +7 | +7 | -3 | -3 | -11 | +2 | +4 | 0 | +13 | +3 | 0 |
| Lunaith | +4 | -1 | +2 | 0 | +2 | 0 | +1 | +2 | -1 | 0 | +3 | +5 | -3 |
| Selene | +6 | +4 | +6 | -1 | +2 | +3 | 0 | +4 | +6 | +6 | -3 | +5 | -3 |
| Elowen | +3 | +2 | -2 | +12 | +5 | +7 | +17 | +10 | +9 | +5 | +5 | +6 | +8 |
| Seren | -7 | -5 | -3 | -6 | 0 | -5 | -11 | 0 | -5 | -11 | -7 | -9 | -15 |
| Tempra | +7 | -2 | +2 | +6 | +4 | +3 | +2 | +4 | +7 | +3 | +7 | +14 | +3 |
| Draxa | +7 | 0 | -5 | +7 | -1 | -1 | +3 | +2 | +2 | +9 | -2 | +6 | -4 |
| Runa | -4 | -2 | -11 | -5 | -5 | +5 | -4 | -6 | -4 | -2 | -1 | -3 | -9 |
| Vaela | +6 | +3 | -1 | +12 | +11 | +1 | +4 | +10 | +9 | +3 | +3 | +6 | +13 |
| Maelis | +5 | -3 | +2 | +7 | -1 | +1 | +4 | 0 | +16 | +8 | +4 | 0 | -5 |
| Nyx | -4 | -13 | -6 | +2 | -3 | -1 | -2 | -10 | -5 | -13 | +2 | +2 | -9 |
| Ravenna | +12 | -2 | -2 | +3 | +5 | +6 | -1 | +21 | 0 | +6 | +19 | +14 | +10 |
| Sable | +3 | 0 | -6 | +6 | +6 | -4 | -4 | -3 | -3 | +1 | -5 | +2 | +9 |
| Mordessa | -5 | -3 | -6 | -7 | -1 | +1 | -7 | -8 | -3 | -7 | -6 | -8 | 0 |
| Thessaly | +1 | -4 | +2 | -3 | +3 | 0 | -1 | +2 | -2 | +3 | -5 | +4 | +10 |
| Vesper | -8 | -10 | -8 | -6 | -7 | -5 | -10 | -10 | -5 | -3 | -9 | -14 | -1 |
| Liora | -6 | +2 | +1 | +1 | +4 | +2 | +7 | +7 | +11 | +3 | +14 | +9 | +6 |
| Noctelle | -8 | -2 | -4 | 0 | +3 | -3 | +1 | -4 | -8 | -4 | -1 | +4 | +2 |
| Velisande | 0 | 0 | -3 | -2 | -5 | +3 | 0 | +5 | +2 | 0 | +7 | +5 | -2 |
| Corvina | -3 | -1 | 0 | +7 | -6 | -1 | -2 | -2 | -2 | -5 | -8 | +2 | +10 |
| Sangrael | +4 | +8 | +5 | +5 | +2 | -2 | +4 | +3 | +10 | +2 | +6 | +16 | +22 |
| Ilyra | +5 | +5 | +11 | +12 | +5 | -5 | +6 | +1 | +4 | +5 | +11 | +14 | +1 |
| Lucienne | -4 | +1 | -5 | -9 | -2 | -6 | -3 | -3 | -5 | +2 | -11 | -6 | -2 |
| Ophira | -4 | +4 | +6 | +5 | +14 | 0 | +8 | +2 | +9 | -1 | +2 | +5 | +11 |
| Solenne | +1 | +5 | +12 | +10 | +1 | 0 | +11 | +16 | +4 | +12 | -3 | +10 | -3 |
| Aurelle | +6 | +5 | +1 | 0 | +3 | +2 | +1 | +4 | +2 | +6 | +5 | +1 | +3 |
| Elara | -2 | -4 | -6 | +1 | 0 | -7 | -2 | -2 | -6 | -4 | +2 | +2 | -4 |
| Amarante | -5 | -1 | 0 | -1 | -5 | 0 | -7 | -1 | -9 | -11 | -11 | -6 | +1 |
| Rosalind | -10 | -8 | -4 | -1 | +1 | -5 | -8 | -16 | -8 | -13 | -11 | -2 | -7 |
| Seravelle | +10 | 0 | +5 | +8 | +3 | -2 | +6 | +3 | +2 | +5 | -1 | +1 | +11 |
| Cassia | +12 | +3 | +10 | +5 | +3 | +1 | +7 | +2 | +12 | +6 | +7 | +18 | +6 |
| Isolde | -7 | -4 | -7 | -12 | -5 | -9 | -8 | -10 | -15 | -1 | +1 | +2 | -12 |
| Valeria | +6 | +6 | +7 | +7 | +6 | -1 | +7 | +1 | +8 | +5 | 0 | +3 | +5 |
| Kaida | -11 | 0 | -10 | +2 | -5 | -8 | -10 | -2 | -8 | -8 | -12 | -10 | -1 |
| Rhiannon | -3 | -4 | -5 | -7 | -3 | 0 | -7 | -5 | +9 | +4 | -4 | +9 | +16 |
| Sylwen | +5 | +1 | -2 | -1 | -2 | +6 | -1 | +2 | -6 | -4 | 0 | -3 | -5 |
| Venna | -12 | -6 | -7 | -1 | -8 | +2 | +10 | -17 | -16 | -2 | -5 | -6 | -9 |
| Mireille | +8 | -6 | -5 | +3 | +7 | +3 | +4 | +10 | +2 | +2 | -1 | +12 | -4 |
| Nimue | +5 | +4 | +2 | +5 | +6 | +8 | -2 | +9 | +2 | +3 | 0 | -7 | +3 |
| Fenna | +6 | +4 | +2 | +5 | +3 | -1 | +5 | +5 | +14 | +9 | +15 | +8 | +6 |
| Wren | -10 | +2 | -6 | -3 | -8 | -10 | -15 | -10 | -11 | -6 | -22 | -4 | -3 |
| Zephyra | -16 | -3 | -6 | -9 | -4 | -2 | -2 | -6 | -5 | -5 | -3 | -10 | -6 |
| Briar | -8 | -6 | -2 | -5 | -6 | -8 | -5 | -5 | -16 | -5 | -1 | +2 | +1 |
| Eldrith | -5 | +2 | -8 | 0 | -1 | +3 | +3 | -3 | -5 | 0 | -6 | -4 | +3 |
| Hartwen | -8 | +1 | -2 | -9 | 0 | -7 | -10 | -3 | -3 | -4 | -2 | -2 | +9 |
| Calypso | +9 | +3 | +6 | +2 | +16 | +11 | +9 | -3 | +14 | +6 | +17 | +2 | +4 |
| Halcyra | -4 | -8 | -4 | -2 | -3 | -1 | -7 | -11 | -11 | -11 | -3 | -4 | -6 |
| Nerissa | -10 | -5 | -9 | -8 | 0 | -9 | -2 | -12 | -7 | -4 | -6 | -4 | -7 |
| Ysra | 0 | +3 | 0 | -6 | +4 | +4 | +8 | 0 | +13 | +5 | -7 | +1 | +10 |
| Marisol | -2 | -3 | +2 | -4 | -9 | +2 | 0 | -6 | -15 | -1 | -1 | -4 | -3 |
| Odette | -8 | -3 | -5 | -9 | -3 | -4 | -9 | -11 | -3 | -2 | -2 | +1 | -2 |
| Siora | +7 | +6 | +9 | +3 | +2 | +6 | +14 | +9 | +8 | +6 | +13 | +13 | +6 |
| Lorelei | +10 | -2 | +8 | +2 | +3 | +6 | -2 | -2 | +9 | +7 | +12 | +11 | +7 |
| Ondine | +2 | +2 | +5 | +1 | 0 | 0 | +8 | -1 | +13 | +9 | +6 | +17 | +6 |
| Pelagia | -7 | -6 | +1 | -3 | -5 | +4 | -7 | +1 | -8 | +6 | -2 | +1 | -16 |
| Coralie | +7 | +4 | +2 | 0 | +6 | +6 | +6 | +7 | -1 | +5 | +5 | +9 | +13 |
| Thalassa | -14 | -7 | -11 | -9 | -3 | -4 | -7 | -7 | -8 | -5 | -4 | -2 | +7 |

## Races

| Race | Win % of its heroes in single-race teams | Uplift of its heroes vs each race (high / dark / nature / ocean / arcane) |
| --- | --- | --- |
| high | +1 uplift | +1 / +2 / +2 / +2 / -3 |
| dark | +4 uplift | -2 / -2 / +1 / 0 / 0 |
| nature | +1 uplift | -1 / -4 / -2 / -2 / -1 |
| ocean | +2 uplift | -1 / 0 / -3 / +1 / +2 |
| arcane | -2 uplift | +1 / 0 / +2 / 0 / +1 |
