# Niche report v7 (after roster pass 9, 2026-10-03)

Balance target (decisions row 29). Reproduce: `tools/csharp/test.sh -Main Gacha.Tests.Tools.NicheReport 150 1`.

Change since v6: roster pass 9 (stronger payoffs, team-wide amplifiers, stronger DEF-down/weaken tier, 3 s control window, carry trims, weak-hero lifts, race team bonus). **Outside the ±8 band: 1 hero (Venna -9, within noise), down from 14.** Outside ±5: 22. Lowest: Venna -9, Vesper -8, Halcyra -7, Isolde -7, Nyx -7. Highest: Vaela +8, Siora +8, Calypso +8, Fenna +7, Elowen +7, Cassia +7.

_234000 battles in 225s; 150 paired samples per hero per situation. Noise (one standard error): about ±3 points per cell, ±1.0 for a hero's overall uplift._

## Flags

- **Odette** (ocean healer): NO NICHE. Overall -3 points; above her peers in 0 of 13 situations; best vs ocean, worst random.
- **Amarante** (high support): NO NICHE. Overall -5 points; above her peers in 0 of 13 situations; best vs arcane, worst vs tanky.
- **Rosalind** (high support): NO NICHE. Overall -6 points; above her peers in 0 of 13 situations; best vs nature, worst vs tanky.
- **Nerissa** (ocean dps): NO NICHE. Overall -6 points; above her peers in 0 of 13 situations; best vs tanky, worst own race.
- **Wren** (nature support): NO NICHE. Overall -6 points; above her peers in 0 of 13 situations; best vs high, worst vs tanky.
- **Briar** (nature tank): NO NICHE. Overall -6 points; above her peers in 0 of 13 situations; best full package, worst vs burst.
- **Runa** (arcane tank): NO NICHE. Overall -6 points; above her peers in 0 of 13 situations; best full package, worst own race.
- **Halcyra** (ocean dps): NO NICHE. Overall -7 points; above her peers in 0 of 13 situations; best vs arcane, worst vs control.
- **Nyx** (dark dps): NO NICHE. Overall -7 points; above her peers in 0 of 13 situations; best vs tanky, worst vs high.
- **Isolde** (high tank): NO NICHE. Overall -7 points; above her peers in 0 of 13 situations; best vs control, worst vs sustain.
- **Vesper** (dark healer): NO NICHE. Overall -8 points; above her peers in 0 of 13 situations; best vs control, worst vs high.

## Tanks

| Hero | Race | Uplift overall | Best situation | Worst situation | Damage soaked share / survives | Verdict |
| --- | --- | --- | --- | --- | --- | --- |
| Vaela | arcane | +8 | vs arcane (+15) | vs tanky (+2) | 30% / 46% | niche |
| Cassia | high | +7 | vs control (+12) | vs nature (+1) | 33% / 42% | niche |
| Sangrael | dark | +6 | own race (+21) | vs arcane (-1) | 31% / 43% | niche |
| Coralie | ocean | +5 | own race (+16) | vs control (-2) | 31% / 45% | niche |
| Valeria | high | +4 | vs tanky (+8) | vs control (0) | 30% / 43% | niche |
| Draxa | arcane | +2 | vs sustain (+7) | own race (-5) | 35% / 46% | niche |
| Eldrith | nature | -1 | with partner (+3) | random (-12) | 41% / 43% | niche |
| Corvina | dark | -2 | own race (+7) | vs high (-6) | 43% / 38% | niche |
| Hartwen | nature | -3 | own race (+5) | random (-12) | 28% / 35% | niche |
| Thalassa | ocean | -4 | own race (+6) | with partner (-15) | 26% / 34% | niche |
| Briar | nature | -6 | full package (+1) | vs burst (-17) | 25% / 35% | NO NICHE |
| Runa | arcane | -6 | full package (-1) | own race (-18) | 29% / 35% | NO NICHE |
| Isolde | high | -7 | vs control (-1) | vs sustain (-13) | 28% / 34% | NO NICHE |

## Dpss

| Hero | Race | Uplift overall | Best situation | Worst situation | Team damage share | Verdict |
| --- | --- | --- | --- | --- | --- | --- |
| Calypso | ocean | +8 | vs burst (+15) | vs high (-4) | 29% | niche |
| Ravenna | dark | +6 | full package (+25) | vs tanky (-10) | 43% | niche |
| Solenne | high | +5 | vs dark (+11) | own race (-7) | 26% | niche |
| Astraea | arcane | +5 | full package (+14) | vs ocean (-4) | 37% | niche |
| Ysra | ocean | +4 | vs burst (+13) | with partner (-8) | 32% | niche |
| Ophira | high | +4 | vs ocean (+14) | full package (-6) | 27% | niche |
| Ilyra | high | +3 | vs nature (+14) | own race (-4) | 42% | niche |
| Zaria | arcane | +3 | full package (+10) | vs tanky (-4) | 39% | niche |
| Maelis | dark | +2 | full package (+8) | vs ocean (-3) | 31% | niche |
| Isaura | arcane | +2 | vs burst (+10) | vs nature (-8) | 31% | niche |
| Sable | dark | -1 | vs ocean (+11) | vs tanky (-12) | 32% | niche |
| Sylwen | nature | -1 | vs arcane (+5) | vs burst (-9) | 35% | niche |
| Rhiannon | nature | -2 | own race (+16) | vs dark (-9) | 38% | niche |
| Caelith | arcane | -2 | vs high (+7) | own race (-15) | 24% | niche |
| Kaida | nature | -6 | vs nature (+5) | with partner (-13) | 31% | niche |
| Nerissa | ocean | -6 | vs tanky (+1) | own race (-11) | 25% | NO NICHE |
| Lucienne | high | -6 | vs high (+3) | with partner (-11) | 34% | niche |
| Halcyra | ocean | -7 | vs arcane (0) | vs control (-14) | 35% | NO NICHE |
| Nyx | dark | -7 | vs tanky (+1) | vs high (-19) | 26% | NO NICHE |
| Venna | nature | -9 | vs tanky (+5) | full package (-18) | 34% | niche |

## Healers

| Hero | Race | Uplift overall | Best situation | Worst situation | Heal / clutch heal per battle | Verdict |
| --- | --- | --- | --- | --- | --- | --- |
| Siora | ocean | +8 | full package (+22) | vs nature (+1) | 3873 / 687 | niche |
| Aurelle | high | +5 | vs high (+9) | vs ocean (-1) | 4885 / 982 | niche |
| Nimue | nature | +3 | vs nature (+8) | full package (-7) | 1687 / 600 | niche |
| Selene | arcane | +2 | own race (+6) | random (-2) | 3483 / 1628 | niche |
| Mireille | nature | +1 | full package (+6) | vs high (-6) | 4929 / 1314 | niche |
| Lunaith | arcane | 0 | with partner (+7) | vs control (-4) | 3587 / 761 | niche |
| Thessaly | dark | 0 | own race (+6) | vs sustain (-4) | 2967 / 722 | niche |
| Mordessa | dark | -3 | own race (+5) | vs sustain (-14) | 2746 / 610 | niche |
| Marisol | ocean | -3 | full package (+6) | vs burst (-10) | 3222 / 650 | niche |
| Odette | ocean | -3 | vs ocean (0) | random (-8) | 805 / 248 | NO NICHE |
| Elara | high | -3 | vs nature (+3) | vs high (-7) | 3813 / 809 | niche |
| Vesper | dark | -8 | vs control (-2) | vs high (-14) | 2259 / 574 | NO NICHE |

## Supports

| Hero | Race | Uplift overall | Best situation | Worst situation | Effects applied per battle | Verdict |
| --- | --- | --- | --- | --- | --- | --- |
| Elowen | arcane | +7 | vs tanky (+14) | full package (-2) | 13.0 | niche |
| Fenna | nature | +7 | own race (+11) | vs dark (0) | 16.1 | niche |
| Seravelle | high | +6 | own race (+12) | vs high (+2) | 7.3 | niche |
| Liora | dark | +5 | full package (+13) | vs dark (0) | 9.9 | niche |
| Lorelei | ocean | +4 | full package (+12) | vs nature (-3) | 3.0 | niche |
| Ondine | ocean | +4 | vs tanky (+11) | vs sustain (-4) | 7.9 | niche |
| Velisande | dark | +3 | vs tanky (+10) | vs burst (-5) | 14.5 | niche |
| Tempra | arcane | +2 | vs burst (+13) | vs sustain (-7) | 2.0 | niche |
| Noctelle | dark | -1 | own race (+6) | vs nature (-7) | 13.4 | niche |
| Pelagia | ocean | -3 | vs sustain (+7) | random (-12) | 9.4 | niche |
| Zephyra | nature | -4 | vs arcane (+2) | random (-9) | 7.0 | niche |
| Amarante | high | -5 | vs arcane (+2) | vs tanky (-10) | 27.9 | NO NICHE |
| Seren | arcane | -6 | vs ocean (+3) | own race (-13) | 10.5 | niche |
| Rosalind | high | -6 | vs nature (0) | vs tanky (-12) | 9.5 | NO NICHE |
| Wren | nature | -6 | vs high (+1) | vs tanky (-13) | 4.7 | NO NICHE |

## Uplift by situation (points versus a random same-role replacement)

| Hero | random | vs high | vs dark | vs nature | vs ocean | vs arcane | vs tanky | vs sustain | vs burst | vs control | with partner | full package | own race |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| Astraea | +4 | +9 | +4 | +7 | -4 | +7 | -2 | +9 | +6 | -1 | +5 | +14 | +12 |
| Caelith | 0 | +7 | -3 | 0 | -5 | -2 | +1 | -4 | -1 | -7 | +4 | 0 | -15 |
| Isaura | -5 | +2 | +7 | -8 | -1 | +2 | +8 | -2 | +10 | +6 | +6 | -1 | +1 |
| Zaria | +5 | +5 | +5 | +8 | -2 | -2 | -4 | +8 | +1 | -1 | +2 | +10 | +2 |
| Lunaith | 0 | -1 | +1 | 0 | -3 | 0 | +3 | -3 | +6 | -4 | +7 | +1 | -4 |
| Selene | -2 | +2 | +3 | +4 | +5 | +3 | 0 | +1 | +4 | +3 | -1 | -1 | +6 |
| Elowen | +4 | +5 | -1 | +10 | +4 | +11 | +14 | +11 | +7 | +5 | +11 | -2 | +9 |
| Seren | -9 | -5 | -8 | -5 | +3 | -5 | -6 | +1 | -2 | -6 | -5 | -10 | -13 |
| Tempra | +6 | -1 | +1 | +4 | -2 | +3 | +2 | -7 | +13 | +1 | +8 | +3 | -3 |
| Draxa | +5 | +6 | 0 | +3 | -1 | 0 | +1 | +7 | +4 | +3 | -2 | +3 | -5 |
| Runa | -5 | -4 | -11 | -11 | -3 | -1 | -7 | -4 | -3 | -7 | -6 | -1 | -18 |
| Vaela | +14 | +7 | +7 | +12 | +10 | +15 | +2 | +6 | +8 | +3 | +10 | +2 | +13 |
| Maelis | +4 | -2 | +1 | +5 | -3 | 0 | 0 | +1 | +4 | +5 | +4 | +8 | +1 |
| Nyx | -7 | -19 | -10 | -4 | -4 | -1 | +1 | -5 | -6 | -15 | -2 | -9 | -12 |
| Ravenna | +10 | -1 | +3 | 0 | +6 | +4 | -10 | +21 | -8 | +10 | +8 | +25 | +11 |
| Sable | +2 | -2 | -6 | +1 | +11 | -2 | -12 | -1 | -3 | +3 | +7 | -6 | +1 |
| Mordessa | -7 | +1 | -6 | -2 | +1 | -2 | -5 | -14 | -3 | -8 | 0 | +1 | +5 |
| Thessaly | +1 | 0 | +2 | 0 | +2 | -2 | -4 | -4 | -3 | 0 | +1 | -2 | +6 |
| Vesper | -11 | -14 | -13 | -5 | -7 | -5 | -8 | -4 | -8 | -2 | -8 | -10 | -5 |
| Liora | +1 | +2 | 0 | +6 | +1 | +2 | +8 | +3 | +13 | +5 | +5 | +13 | +9 |
| Noctelle | -2 | -3 | -5 | -7 | +4 | -1 | -1 | -1 | -5 | -2 | +1 | +1 | +6 |
| Velisande | +9 | +7 | -1 | +5 | -3 | +5 | +10 | +7 | -5 | 0 | +1 | +3 | 0 |
| Corvina | -4 | -6 | -4 | +2 | +1 | -2 | +2 | -4 | -2 | -4 | -3 | -3 | +7 |
| Sangrael | +5 | +5 | +4 | +5 | +3 | -1 | +4 | +4 | +4 | +8 | +8 | +6 | +21 |
| Ilyra | 0 | +1 | +7 | +14 | +2 | 0 | +10 | +1 | -1 | +2 | +7 | 0 | -4 |
| Lucienne | -10 | +3 | -9 | -8 | -5 | -3 | -4 | -4 | -10 | -1 | -11 | -6 | -8 |
| Ophira | +2 | +2 | +3 | +5 | +14 | -2 | +11 | +2 | +5 | -1 | +7 | -6 | +4 |
| Solenne | 0 | +7 | +11 | +8 | +6 | -3 | +6 | +10 | +5 | +7 | +11 | +11 | -7 |
| Aurelle | +9 | +9 | +5 | +3 | -1 | +3 | +4 | +2 | +5 | +4 | +6 | +3 | +9 |
| Elara | 0 | -7 | -2 | +3 | +1 | -6 | -3 | -5 | -7 | -6 | -2 | -3 | -5 |
| Amarante | -6 | -4 | -5 | -1 | -7 | +2 | -10 | -5 | -9 | -7 | -8 | -1 | -4 |
| Rosalind | -5 | -9 | -3 | 0 | -1 | -3 | -12 | -10 | -7 | -3 | -8 | -5 | -6 |
| Seravelle | +9 | +2 | +5 | +3 | +5 | +3 | +2 | +6 | +11 | +6 | +9 | +3 | +12 |
| Cassia | +12 | +5 | +5 | +1 | +6 | +5 | +10 | +8 | +4 | +12 | +4 | +9 | +11 |
| Isolde | -10 | -3 | -7 | -11 | -7 | -7 | -4 | -13 | -11 | -1 | -8 | -4 | -8 |
| Valeria | +6 | +3 | +5 | +6 | +5 | +3 | +8 | +5 | +5 | 0 | +2 | +4 | +6 |
| Kaida | -9 | -3 | -10 | +5 | -5 | -3 | -13 | -5 | -10 | -7 | -13 | +5 | -5 |
| Rhiannon | -7 | -5 | -9 | -7 | 0 | 0 | -7 | -3 | +3 | +9 | -8 | -3 | +16 |
| Sylwen | +2 | 0 | -3 | +3 | +2 | +5 | +5 | -1 | -9 | +2 | -6 | -3 | -5 |
| Venna | -11 | -4 | -7 | -1 | -6 | -7 | +5 | -14 | -15 | -4 | -16 | -18 | -12 |
| Mireille | -1 | -6 | -1 | +5 | +2 | 0 | +3 | +5 | -2 | -3 | +4 | +6 | +1 |
| Nimue | +1 | +4 | +5 | +8 | +3 | +5 | +2 | +8 | 0 | 0 | +8 | -7 | +3 |
| Fenna | +11 | +6 | 0 | +10 | 0 | +1 | +7 | +8 | +11 | +5 | +9 | +6 | +11 |
| Wren | -7 | +1 | -4 | -2 | -11 | -2 | -13 | -8 | -5 | -3 | -12 | -1 | -7 |
| Zephyra | -9 | -1 | -8 | -4 | -5 | +2 | -5 | -9 | -4 | +2 | -7 | -3 | -3 |
| Briar | -6 | -6 | 0 | -4 | -4 | -5 | -5 | -8 | -17 | -8 | -7 | +1 | -8 |
| Eldrith | -12 | -2 | -7 | +1 | -2 | +2 | -1 | +1 | 0 | -3 | +3 | +1 | -1 |
| Hartwen | -12 | +2 | -4 | -10 | 0 | -4 | -8 | -4 | -2 | -6 | +2 | -1 | +5 |
| Calypso | +9 | -4 | +6 | +8 | +14 | +14 | +12 | +1 | +15 | +5 | +7 | +14 | +2 |
| Halcyra | -3 | -8 | -2 | -2 | -2 | 0 | -4 | -13 | -7 | -14 | -12 | -10 | -8 |
| Nerissa | -6 | -5 | -5 | -5 | -3 | -6 | +1 | -9 | -6 | -2 | -7 | -10 | -11 |
| Ysra | 0 | +6 | +9 | +1 | 0 | +8 | +10 | -6 | +13 | +5 | -8 | +3 | +9 |
| Marisol | -3 | -2 | 0 | -7 | -7 | +1 | -4 | -7 | -10 | -4 | -2 | +6 | 0 |
| Odette | -8 | 0 | -1 | -6 | 0 | -7 | -3 | -4 | -2 | 0 | -5 | 0 | -3 |
| Siora | +8 | +6 | +11 | +1 | +5 | +6 | +10 | +12 | +10 | +4 | +6 | +22 | +6 |
| Lorelei | +1 | 0 | +5 | -3 | +9 | +5 | +5 | +4 | +4 | +6 | +8 | +12 | +3 |
| Ondine | -2 | +3 | +6 | +4 | 0 | -1 | +11 | -4 | +8 | +8 | +5 | +9 | +6 |
| Pelagia | -12 | -4 | -1 | -5 | -8 | +2 | -7 | +7 | -8 | 0 | 0 | 0 | -9 |
| Coralie | -1 | +1 | +3 | +3 | +4 | +8 | +6 | +8 | +8 | -2 | +10 | +4 | +16 |
| Thalassa | -8 | -3 | -6 | -9 | +1 | -1 | -2 | -1 | -10 | -6 | -15 | -2 | +6 |

## Races

| Race | Win % of its heroes in single-race teams | Uplift of its heroes vs each race (high / dark / nature / ocean / arcane) |
| --- | --- | --- |
| high | 0 uplift | +1 / +1 / +2 / +1 / -1 |
| dark | +4 uplift | -3 / -3 / +1 / +1 / -1 |
| nature | 0 uplift | -1 / -4 / 0 / -2 / -1 |
| ocean | +1 uplift | -1 / +2 / -2 / +1 / +2 |
| arcane | -1 uplift | +3 / 0 / +2 / 0 / +2 |
