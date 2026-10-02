# Balance sweep v1 (battle rules v1, 2026-10-02)

Random 5v5 teams, 1,000 matches played both ways (2,000 battles). Win rate of a hero = share of her battles won (timeouts count half). Reproduce: `tools/csharp/test.sh -Main Gacha.Tests.Tools.BalanceSweep 1000 1`. Kit numbers are untuned starting points.

2000 battles in 3.6s; avg 19.6s; timeouts 2.3%

| Hero | Race | Role | Win % | Dmg/battle | Heal/battle | Flag |
| --- | --- | --- | --- | --- | --- | --- |
| Solenne | high | dps | 74.1 | 3250 | 6 | STRONG |
| Ysra | ocean | dps | 71.7 | 3600 | 25 | STRONG |
| Calypso | ocean | dps | 68.6 | 2930 | 404 | STRONG |
| Isaura | arcane | dps | 68.3 | 3437 | 23 | STRONG |
| Ilyra | high | dps | 66.4 | 3879 | 20 | STRONG |
| Ravenna | dark | dps | 65.7 | 3273 | 181 | STRONG |
| Selene | arcane | healer | 64.3 | 1458 | 2217 | STRONG |
| Astraea | arcane | dps | 63.1 | 3415 | 24 | STRONG |
| Cassia | high | tank | 62.8 | 2428 | 587 | STRONG |
| Sable | dark | dps | 61.3 | 2908 | 20 | STRONG |
| Zaria | arcane | dps | 61.3 | 3460 | 16 | STRONG |
| Sylwen | nature | dps | 60.2 | 3521 | 21 | STRONG |
| Aurelle | high | healer | 60.0 | 1494 | 3220 | STRONG |
| Siora | ocean | healer | 59.2 | 1448 | 2385 |  |
| Ophira | high | dps | 57.8 | 2087 | 25 |  |
| Thessaly | dark | healer | 57.4 | 1378 | 2000 |  |
| Mireille | nature | healer | 57.1 | 2110 | 2521 |  |
| Rhiannon | nature | dps | 56.9 | 3147 | 17 |  |
| Halcyra | ocean | dps | 55.7 | 3395 | 28 |  |
| Nimue | nature | healer | 54.6 | 1497 | 1354 |  |
| Nerissa | ocean | dps | 54.4 | 1960 | 18 |  |
| Sangrael | dark | tank | 52.7 | 2343 | 554 |  |
| Kaida | nature | dps | 52.3 | 2469 | 14 |  |
| Lunaith | arcane | healer | 52.0 | 1546 | 2425 |  |
| Elara | high | healer | 51.8 | 1384 | 1364 |  |
| Marisol | ocean | healer | 51.6 | 1439 | 2074 |  |
| Vaela | arcane | tank | 50.5 | 1108 | 9 |  |
| Coralie | ocean | tank | 50.3 | 894 | 8 |  |
| Mordessa | dark | healer | 50.1 | 1370 | 2117 |  |
| Lucienne | high | dps | 50.0 | 2080 | 33 |  |
| Lorelei | ocean | support | 49.4 | 1282 | 17 |  |
| Fenna | nature | support | 48.0 | 1804 | 12 |  |
| Valeria | high | tank | 47.5 | 783 | 8 |  |
| Eldrith | nature | tank | 46.8 | 976 | 2112 |  |
| Odette | ocean | healer | 46.7 | 1595 | 673 |  |
| Corvina | dark | tank | 46.4 | 865 | 13 |  |
| Seravelle | high | support | 45.4 | 1099 | 7 |  |
| Ondine | ocean | support | 45.0 | 1256 | 10 |  |
| Hartwen | nature | tank | 44.3 | 1060 | 750 |  |
| Liora | dark | support | 43.7 | 1834 | 15 |  |
| Draxa | arcane | tank | 43.5 | 2122 | 13 |  |
| Pelagia | ocean | support | 43.4 | 1331 | 19 |  |
| Zephyra | nature | support | 42.8 | 1266 | 7 |  |
| Elowen | arcane | support | 42.7 | 1127 | 8 |  |
| Briar | nature | tank | 40.7 | 964 | 7 |  |
| Runa | arcane | tank | 40.3 | 1176 | 10 |  |
| Velisande | dark | support | 39.8 | 1264 | 59 | WEAK |
| Thalassa | ocean | tank | 39.5 | 1006 | 12 | WEAK |
| Wren | nature | support | 39.0 | 1469 | 11 | WEAK |
| Venna | nature | dps | 39.0 | 2207 | 8 | WEAK |
| Nyx | dark | dps | 38.8 | 1291 | 11 | WEAK |
| Tempra | arcane | support | 37.7 | 1116 | 8 | WEAK |
| Amarante | high | support | 37.6 | 1393 | 8 | WEAK |
| Seren | arcane | support | 37.6 | 1161 | 5 | WEAK |
| Rosalind | high | support | 37.3 | 1167 | 19 | WEAK |
| Vesper | dark | healer | 37.2 | 1946 | 505 | WEAK |
| Noctelle | dark | support | 35.0 | 1210 | 8 | WEAK |
| Maelis | dark | dps | 33.7 | 2921 | 13 | WEAK |
| Isolde | high | tank | 32.9 | 774 | 18 | WEAK |
| Caelith | arcane | dps | 30.7 | 2020 | 14 | WEAK |

| core | Win % |
| --- | --- |
| arcane | 49.2 |
| dark | 46.8 |
| high | 52.4 |
| nature | 48.7 |
| ocean | 53.0 |

| role | Win % |
| --- | --- |
| dps | 56.6 |
| healer | 53.4 |
| support | 41.6 |
| tank | 46.3 |

