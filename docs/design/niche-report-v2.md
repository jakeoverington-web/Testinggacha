# Niche report v2 (battle rules v1 + dive timing, 2026-10-02)

Balance target (decisions row 29): every hero is the best pick somewhere; no hero is the best pick everywhere. Uplift = points a hero adds to her team's result versus a random same-role replacement, same opponent and seed. DOMINANT = overall ≥ +10 and above peers in ≥ 11 of 13 situations. NO NICHE = never more than +2 above peers in any situation. Reproduce: `tools/csharp/test.sh -Main Gacha.Tests.Tools.NicheReport 150 1`.

Changes since v1: Vesper's ultimate heals each ally (was split across the team); divers (Nyx, Nerissa, Ophira, Lucienne) wait for their front line; new situation: full package (up to three synergy partners).

_234000 battles in 165s; 150 paired samples per hero per situation. Noise (one standard error): about ±4 points per cell, ±1.0 for a hero's overall uplift._

## Flags

- **Isaura** (arcane dps): DOMINANT. Overall +22 points; above her peers in 13 of 13 situations; best with partner, worst vs control.
- **Solenne** (high dps): DOMINANT. Overall +17 points; above her peers in 13 of 13 situations; best vs dark, worst vs ocean.
- **Calypso** (ocean dps): DOMINANT. Overall +13 points; above her peers in 13 of 13 situations; best vs burst, worst vs dark.
- **Astraea** (arcane dps): DOMINANT. Overall +12 points; above her peers in 12 of 13 situations; best vs nature, worst vs ocean.
- **Fenna** (nature support): DOMINANT. Overall +11 points; above her peers in 12 of 13 situations; best full package, worst vs dark.
- **Ysra** (ocean dps): DOMINANT. Overall +10 points; above her peers in 11 of 13 situations; best vs dark, worst vs nature.
- **Marisol** (ocean healer): NO NICHE. Overall -4 points; above her peers in 0 of 13 situations; best vs high, worst vs sustain.
- **Amarante** (high support): NO NICHE. Overall -5 points; above her peers in 0 of 13 situations; best vs burst, worst vs nature.
- **Thalassa** (ocean tank): NO NICHE. Overall -6 points; above her peers in 0 of 13 situations; best vs sustain, worst own race.
- **Briar** (nature tank): NO NICHE. Overall -6 points; above her peers in 0 of 13 situations; best vs arcane, worst vs ocean.
- **Rosalind** (high support): NO NICHE. Overall -7 points; above her peers in 0 of 13 situations; best vs high, worst random.
- **Zephyra** (nature support): NO NICHE. Overall -7 points; above her peers in 0 of 13 situations; best own race, worst vs burst.
- **Elara** (high healer): NO NICHE. Overall -8 points; above her peers in 0 of 13 situations; best vs dark, worst vs high.
- **Nerissa** (ocean dps): NO NICHE. Overall -9 points; above her peers in 0 of 13 situations; best vs high, worst own race.
- **Lucienne** (high dps): NO NICHE. Overall -9 points; above her peers in 0 of 13 situations; best vs control, worst vs arcane.
- **Isolde** (high tank): NO NICHE. Overall -10 points; above her peers in 0 of 13 situations; best full package, worst vs dark.
- **Vesper** (dark healer): NO NICHE. Overall -11 points; above her peers in 0 of 13 situations; best full package, worst vs high.
- **Nyx** (dark dps): NO NICHE. Overall -19 points; above her peers in 0 of 13 situations; best vs tanky, worst vs sustain.
- **Caelith** (arcane dps): NO NICHE. Overall -22 points; above her peers in 0 of 13 situations; best with partner, worst own race.
- **Venna** (nature dps): NO NICHE. Overall -22 points; above her peers in 0 of 13 situations; best vs control, worst full package.

## Tanks

| Hero | Race | Uplift overall | Best situation | Worst situation | Damage soaked share / survives | Verdict |
| --- | --- | --- | --- | --- | --- | --- |
| Cassia | high | +9 | with partner (+19) | vs nature (-3) | 35% / 49% | niche |
| Sangrael | dark | +8 | own race (+14) | vs arcane (+1) | 32% / 51% | niche |
| Coralie | ocean | +7 | vs sustain (+11) | full package (+0) | 32% / 54% | niche |
| Valeria | high | +6 | vs burst (+18) | vs arcane (+1) | 31% / 46% | niche |
| Draxa | arcane | +4 | vs tanky (+9) | vs ocean (-4) | 34% / 51% | niche |
| Vaela | arcane | +2 | vs dark (+5) | vs burst (-3) | 32% / 51% | niche |
| Corvina | dark | 0 | vs tanky (+8) | vs ocean (-7) | 44% / 48% | niche |
| Eldrith | nature | -2 | vs nature (+5) | vs arcane (-9) | 40% / 47% | niche |
| Hartwen | nature | -2 | vs arcane (+4) | vs dark (-13) | 34% / 41% | niche |
| Runa | arcane | -4 | random (+3) | vs sustain (-12) | 29% / 42% | niche |
| Thalassa | ocean | -6 | vs sustain (-2) | own race (-10) | 28% / 41% | NO NICHE |
| Briar | nature | -6 | vs arcane (-2) | vs ocean (-14) | 27% / 37% | NO NICHE |
| Isolde | high | -10 | full package (-1) | vs dark (-17) | 27% / 36% | NO NICHE |

## Dpss

| Hero | Race | Uplift overall | Best situation | Worst situation | Team damage share | Verdict |
| --- | --- | --- | --- | --- | --- | --- |
| Isaura | arcane | +22 | with partner (+32) | vs control (+5) | 41% | DOMINANT |
| Solenne | high | +17 | vs dark (+28) | vs ocean (+5) | 30% | DOMINANT |
| Ilyra | high | +15 | vs nature (+38) | vs arcane (+1) | 47% | niche |
| Calypso | ocean | +13 | vs burst (+25) | vs dark (+6) | 30% | DOMINANT |
| Astraea | arcane | +12 | vs nature (+23) | vs ocean (-4) | 39% | DOMINANT |
| Ysra | ocean | +10 | vs dark (+21) | vs nature (-3) | 33% | DOMINANT |
| Ravenna | dark | +7 | own race (+17) | vs tanky (0) | 37% | niche |
| Rhiannon | nature | +5 | own race (+27) | with partner (-10) | 37% | niche |
| Zaria | arcane | +5 | vs nature (+17) | vs tanky (0) | 39% | niche |
| Halcyra | ocean | +2 | vs dark (+8) | own race (-5) | 36% | niche |
| Ophira | high | +1 | vs arcane (+12) | vs nature (-7) | 24% | niche |
| Sable | dark | 0 | vs control (+5) | vs arcane (-11) | 29% | niche |
| Sylwen | nature | -1 | own race (+6) | vs sustain (-6) | 33% | niche |
| Kaida | nature | -7 | own race (+4) | vs dark (-16) | 27% | niche |
| Maelis | dark | -8 | own race (+10) | vs high (-19) | 37% | niche |
| Nerissa | ocean | -9 | vs high (-2) | own race (-17) | 21% | NO NICHE |
| Lucienne | high | -9 | vs control (+0) | vs arcane (-16) | 28% | NO NICHE |
| Nyx | dark | -19 | vs tanky (-5) | vs sustain (-33) | 17% | NO NICHE |
| Caelith | arcane | -22 | with partner (-15) | own race (-36) | 28% | NO NICHE |
| Venna | nature | -22 | vs control (-10) | full package (-35) | 28% | NO NICHE |

## Healers

| Hero | Race | Uplift overall | Best situation | Worst situation | Heal / clutch heal per battle | Verdict |
| --- | --- | --- | --- | --- | --- | --- |
| Aurelle | high | +9 | own race (+14) | full package (+5) | 3838 / 883 | niche |
| Siora | ocean | +7 | vs arcane (+12) | random (+3) | 2557 / 569 | niche |
| Selene | arcane | +4 | vs dark (+12) | with partner (-2) | 2408 / 1151 | niche |
| Nimue | nature | +4 | vs ocean (+14) | vs dark (-3) | 1226 / 448 | niche |
| Mireille | nature | +3 | vs sustain (+10) | vs high (-5) | 2958 / 961 | niche |
| Thessaly | dark | +0 | own race (+6) | vs sustain (-4) | 2102 / 603 | niche |
| Lunaith | arcane | -1 | random (+6) | vs nature (-9) | 2298 / 643 | niche |
| Mordessa | dark | -3 | full package (+4) | vs dark (-9) | 1996 / 516 | niche |
| Odette | ocean | -4 | vs dark (+3) | vs burst (-11) | 621 / 224 | niche |
| Marisol | ocean | -4 | vs high (+2) | vs sustain (-9) | 2069 / 522 | NO NICHE |
| Elara | high | -8 | vs dark (-3) | vs high (-14) | 1549 / 409 | NO NICHE |
| Vesper | dark | -11 | full package (-6) | vs high (-15) | 1020 / 297 | NO NICHE |

## Supports

| Hero | Race | Uplift overall | Best situation | Worst situation | Effects applied per battle | Verdict |
| --- | --- | --- | --- | --- | --- | --- |
| Fenna | nature | +11 | full package (+20) | vs dark (+1) | 11.9 | DOMINANT |
| Elowen | arcane | +8 | vs arcane (+14) | vs ocean (+3) | 9.2 | niche |
| Lorelei | ocean | +6 | full package (+15) | vs nature (-3) | 2.2 | niche |
| Liora | dark | +6 | with partner (+11) | vs nature (-2) | 7.2 | niche |
| Seravelle | high | +6 | random (+12) | vs nature (+1) | 5.1 | niche |
| Ondine | ocean | +4 | full package (+12) | vs ocean (-5) | 6.1 | niche |
| Velisande | dark | +3 | vs nature (+11) | vs ocean (-17) | 11.0 | niche |
| Tempra | arcane | -2 | full package (+7) | own race (-10) | 1.6 | niche |
| Pelagia | ocean | -3 | vs dark (+7) | own race (-16) | 6.9 | niche |
| Seren | arcane | -3 | vs nature (+3) | vs dark (-9) | 7.9 | niche |
| Noctelle | dark | -3 | vs tanky (+3) | vs high (-9) | 10.1 | niche |
| Amarante | high | -5 | vs burst (+0) | vs nature (-11) | 20.0 | NO NICHE |
| Rosalind | high | -7 | vs high (+0) | random (-12) | 7.0 | NO NICHE |
| Zephyra | nature | -7 | own race (+1) | vs burst (-14) | 4.4 | NO NICHE |
| Wren | nature | -8 | vs arcane (+2) | vs tanky (-16) | 3.7 | niche |

## Uplift by situation (points versus a random same-role replacement)

| Hero | random | vs high | vs dark | vs nature | vs ocean | vs arcane | vs tanky | vs sustain | vs burst | vs control | with partner | full package | own race |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| Astraea | +22 | +21 | +10 | +23 | -4 | +6 | +5 | +17 | +4 | +6 | +10 | +11 | +20 |
| Caelith | -23 | -23 | -26 | -22 | -19 | -24 | -18 | -23 | -21 | -17 | -15 | -16 | -36 |
| Isaura | +19 | +31 | +27 | +19 | +17 | +23 | +27 | +25 | +14 | +5 | +32 | +23 | +23 |
| Zaria | +4 | +12 | +6 | +17 | +4 | +3 | 0 | +2 | +5 | +3 | +4 | +3 | +4 |
| Lunaith | +6 | +1 | +2 | -9 | +3 | +1 | +2 | -9 | +1 | -2 | -3 | -2 | -7 |
| Selene | -1 | +8 | +12 | +3 | +8 | +5 | -2 | +6 | +6 | +5 | -2 | +5 | +1 |
| Elowen | +11 | +10 | +6 | +8 | +3 | +14 | +14 | +7 | +9 | +5 | +7 | +4 | +10 |
| Seren | -6 | +1 | -9 | +3 | -3 | -4 | -6 | -3 | +1 | 0 | -3 | -9 | -5 |
| Tempra | +2 | -4 | +1 | -6 | +1 | +1 | -5 | -5 | -4 | -6 | +2 | +7 | -10 |
| Draxa | +4 | +3 | +4 | +6 | -4 | +5 | +9 | +5 | +5 | +9 | +5 | +0 | +2 |
| Runa | +3 | -6 | -2 | -7 | 0 | +1 | -2 | -12 | -3 | -2 | -4 | -4 | -10 |
| Vaela | +1 | +0 | +5 | +5 | +4 | -1 | +4 | +3 | -3 | +4 | +2 | +0 | +5 |
| Maelis | -13 | -19 | -8 | +1 | -10 | -15 | -8 | -13 | -7 | -2 | -11 | -11 | +10 |
| Nyx | -13 | -22 | -29 | -24 | -8 | -15 | -5 | -33 | -19 | -18 | -23 | -18 | -24 |
| Ravenna | +6 | +9 | +9 | +10 | +6 | +6 | 0 | +11 | +3 | +6 | +2 | +7 | +17 |
| Sable | -1 | +1 | +4 | -4 | +3 | -11 | -3 | +2 | -5 | +5 | -1 | 0 | +4 |
| Mordessa | -4 | -2 | -9 | -1 | -6 | -7 | -7 | -7 | +0 | -2 | -6 | +4 | +3 |
| Thessaly | +3 | -2 | +0 | +2 | +2 | -3 | -4 | -4 | +5 | -3 | -2 | +1 | +6 |
| Vesper | -8 | -15 | -10 | -12 | -10 | -12 | -12 | -12 | -11 | -8 | -14 | -6 | -7 |
| Liora | +5 | +7 | +7 | -2 | +7 | +3 | +1 | +7 | +10 | +6 | +11 | +10 | +8 |
| Noctelle | -5 | -9 | +0 | -8 | +1 | -6 | +3 | -8 | +0 | -4 | -5 | -3 | +2 |
| Velisande | +7 | +10 | +6 | +11 | -17 | +2 | +11 | +4 | -2 | +1 | +2 | -1 | +4 |
| Corvina | +1 | -2 | +2 | +5 | -7 | +0 | +8 | -3 | +4 | -5 | +1 | -6 | -5 |
| Sangrael | +9 | +6 | +11 | +11 | +10 | +1 | +7 | +11 | +6 | +7 | +7 | +9 | +14 |
| Ilyra | +14 | +11 | +30 | +38 | +17 | +1 | +19 | +14 | +16 | +1 | +19 | +19 | +2 |
| Lucienne | -13 | -7 | -6 | -13 | -4 | -16 | -8 | -10 | -4 | +0 | -13 | -10 | -10 |
| Ophira | -4 | -4 | +3 | -7 | +4 | +12 | +7 | -6 | +8 | -2 | +1 | -2 | +1 |
| Solenne | +17 | +11 | +28 | +21 | +5 | +6 | +26 | +18 | +13 | +11 | +22 | +25 | +14 |
| Aurelle | +7 | +13 | +8 | +9 | +6 | +5 | +13 | +10 | +9 | +10 | +9 | +5 | +14 |
| Elara | -7 | -14 | -3 | -9 | -8 | -9 | -12 | -5 | -8 | -6 | -5 | -7 | -11 |
| Amarante | -2 | -2 | -6 | -11 | -4 | -9 | -6 | -2 | +0 | -7 | -11 | -10 | +0 |
| Rosalind | -12 | +0 | -3 | -4 | -3 | -9 | -12 | -11 | -5 | -3 | -7 | -8 | -10 |
| Seravelle | +12 | +5 | +4 | +1 | +5 | +5 | +6 | +3 | +9 | +3 | +9 | +3 | +9 |
| Cassia | +7 | +15 | +4 | -3 | +17 | +7 | +11 | +12 | +7 | +7 | +19 | +6 | +13 |
| Isolde | -12 | -11 | -17 | -16 | -11 | -8 | -5 | -16 | -10 | -7 | -12 | -1 | -11 |
| Valeria | +3 | +5 | +10 | +8 | +6 | +1 | +5 | +6 | +18 | +2 | +3 | +2 | +4 |
| Kaida | -9 | -2 | -16 | -3 | -6 | -1 | -10 | -6 | -11 | -11 | -12 | -2 | +4 |
| Rhiannon | +7 | +5 | -6 | -8 | +9 | +12 | -3 | +12 | +15 | +4 | -10 | +6 | +27 |
| Sylwen | +3 | +2 | -4 | -4 | +0 | +0 | -4 | -6 | 0 | +0 | -2 | -4 | +6 |
| Venna | -17 | -28 | -23 | -22 | -18 | -19 | -27 | -17 | -21 | -10 | -29 | -35 | -19 |
| Mireille | +2 | -5 | +4 | +2 | +3 | +8 | +1 | +10 | +3 | -1 | +4 | +9 | -1 |
| Nimue | +3 | +1 | -3 | +6 | +14 | +1 | -2 | +9 | +5 | +7 | +7 | -2 | -1 |
| Fenna | +9 | +14 | +1 | +16 | +7 | +9 | +14 | +10 | +13 | +15 | +5 | +20 | +11 |
| Wren | -4 | -4 | -11 | -13 | -5 | +2 | -16 | -8 | -14 | -7 | -9 | -6 | -14 |
| Zephyra | -7 | -5 | -8 | -11 | -8 | -10 | -2 | -12 | -14 | -1 | -1 | -9 | +1 |
| Briar | -4 | -4 | -3 | -7 | -14 | -2 | -5 | -6 | -7 | -4 | -12 | -3 | -4 |
| Eldrith | +2 | -6 | -5 | +5 | -4 | -9 | -4 | +1 | -4 | +0 | -5 | +0 | +5 |
| Hartwen | -3 | +3 | -13 | -3 | +0 | +4 | -2 | -2 | -7 | -3 | -6 | +4 | +0 |
| Calypso | +12 | +8 | +6 | +6 | +20 | +22 | +8 | +8 | +25 | +13 | +18 | +23 | +7 |
| Halcyra | +4 | +2 | +8 | +6 | -3 | +3 | +2 | +7 | -1 | -2 | +8 | -2 | -5 |
| Nerissa | -9 | -2 | -14 | -8 | -8 | -3 | -3 | -13 | -8 | -5 | -12 | -8 | -17 |
| Ysra | +9 | +15 | +21 | -3 | -1 | +19 | +15 | +3 | +16 | +7 | +13 | +15 | +6 |
| Marisol | -7 | +2 | +0 | -7 | -1 | -3 | -6 | -9 | -7 | -7 | -4 | +0 | -1 |
| Odette | -2 | -1 | +3 | -7 | -8 | -5 | -8 | -11 | -11 | +3 | -2 | +2 | -2 |
| Siora | +3 | +3 | +6 | +4 | +9 | +12 | +9 | +11 | +3 | +8 | +6 | +10 | +4 |
| Lorelei | +7 | +6 | +9 | -3 | +8 | +2 | +2 | +4 | +13 | +7 | +4 | +15 | +10 |
| Ondine | +1 | +3 | +5 | +1 | -5 | +3 | +8 | +6 | +6 | +3 | +9 | +12 | 0 |
| Pelagia | -2 | -3 | +7 | -13 | -4 | +0 | -5 | -4 | 0 | +1 | -5 | +3 | -16 |
| Coralie | +5 | +1 | +6 | +11 | +10 | +5 | +4 | +11 | +9 | +7 | +4 | +0 | +11 |
| Thalassa | -2 | -8 | -3 | -3 | -7 | -5 | -7 | -2 | -8 | -5 | -4 | -6 | -10 |

## Races

| Race | Win % of its heroes in single-race teams | Uplift of its heroes vs each race (high / dark / nature / ocean / arcane) |
| --- | --- | --- |
| high | +1 uplift | +2 / +4 / +1 / +3 / -1 |
| dark | +3 uplift | -3 / -1 / -1 / -3 / -5 |
| nature | +1 uplift | -2 / -7 / -3 / -2 / 0 |
| ocean | -1 uplift | +2 / +4 / -1 / +1 / +4 |
| arcane | 0 uplift | +5 / +3 / +3 / +1 / +3 |
