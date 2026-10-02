# Ablation report v2 (after roster pass 8, 2026-10-03)

What each part of a kit is worth: paired battles with the full hero vs the same hero with one part switched off. Reproduce: `tools/csharp/test.sh -Main Gacha.Tests.Tools.Ablation 300 1`.

Pass 8 effect: the misaimed skills are now worth something: Lunaith's Drowse −4.5 → +6.0, Rosalind's Knighting −4.2 → +2.7, Tempra's Stasis −4.2 → +0.5, Briar's Root Hold −2.5 → +2.7, Zephyra's Dream Dust −0.7 → +3.2. Skill 1 for damage dealers rose from 4.9 to 7.4 points on average; auto attacks still carry the most (they also fill the energy bar), 25.0 → 23.9.

_108000 battles; 300 paired samples per hero; noise about ±1.8 points per cell (one standard error)._

Value = points of team result lost when that part is switched off. **Bold** = more than 2.5x noise away from the role average.

## Tanks

| Hero | Ultimate | Skill 1 | Skill 2 | Passive | Auto attacks |
| --- | --- | --- | --- | --- | --- |
| *role average* | *4.9* | *3.2* | *2.0* | *1.4* | *8.4* |
| Valeria | **14.0** | 1.0 | 2.3 | **6.3** | **18.3** |
| Cassia | 2.7 | **12.7** | 2.7 | 2.2 | **16.8** |
| Coralie | 6.3 | 5.2 | 5.0 | 2.3 | 8.7 |
| Draxa | **9.5** | 2.5 | -1.7 | **0.0** | 13.7 |
| Corvina | 4.5 | 4.5 | 3.8 | 0.8 | 7.5 |
| Vaela | 7.3 | -1.0 | 2.5 | 0.2 | 11.2 |
| Hartwen | 1.7 | 2.7 | 1.8 | 3.8 | 8.0 |
| Sangrael | 4.0 | 5.2 | 1.8 | 0.7 | 5.7 |
| Isolde | **1.8** | 5.2 | 5.8 | 1.0 | **2.8** |
| Eldrith | 7.8 | **0.3** | -0.3 | **0.0** | 7.7 |
| Briar | **1.7** | 0.7 | 2.7 | 1.3 | **2.8** |
| Runa | 3.2 | 1.7 | -1.2 | **-1.0** | 4.5 |
| Thalassa | **-0.5** | 1.5 | 0.8 | 0.5 | **2.2** |

## Dpss

| Hero | Ultimate | Skill 1 | Skill 2 | Passive | Auto attacks |
| --- | --- | --- | --- | --- | --- |
| *role average* | *11.3* | *7.4* | *2.4* | *3.3* | *23.9* |
| Isaura | **24.5** | 11.7 | 4.7 | 6.5 | **36.0** |
| Ravenna | **21.7** | 11.3 | 2.2 | **12.0** | **31.8** |
| Calypso | **16.8** | 9.0 | 1.0 | **14.3** | **32.5** |
| Maelis | 9.2 | **19.0** | 2.8 | **8.7** | **31.7** |
| Zaria | **18.3** | 9.3 | 6.3 | 3.5 | **31.3** |
| Astraea | 16.5 | 8.7 | 1.8 | 6.0 | 29.2 |
| Ilyra | 14.0 | **18.7** | -0.5 | 4.3 | 22.0 |
| Ysra | 12.8 | 8.2 | -1.3 | 1.3 | 30.0 |
| Halcyra | 14.5 | 5.8 | 2.5 | 5.0 | 22.3 |
| Caelith | 7.8 | 10.3 | 1.5 | **0.0** | 29.8 |
| Nyx | 9.3 | 4.2 | 6.5 | **0.3** | 19.2 |
| Solenne | 8.3 | **1.8** | **6.5** | **-0.2** | 22.8 |
| Rhiannon | 9.2 | 4.7 | 3.2 | 5.3 | **16.8** |
| Ophira | 7.8 | 5.3 | 2.7 | **-0.2** | 23.2 |
| Kaida | 7.7 | 8.8 | 0.8 | **1.2** | **15.7** |
| Sylwen | 7.7 | **-1.0** | 5.7 | **0.0** | 18.7 |
| Sable | 7.8 | 3.8 | 0.0 | **0.0** | 18.3 |
| Venna | **2.7** | **2.2** | 3.5 | **-0.8** | 20.0 |
| Lucienne | **6.5** | 3.3 | -2.0 | **0.5** | **15.7** |
| Nerissa | **2.5** | 2.5 | -0.8 | **-1.0** | **11.2** |

## Healers

| Hero | Ultimate | Skill 1 | Skill 2 | Passive | Auto attacks |
| --- | --- | --- | --- | --- | --- |
| *role average* | *10.5* | *11.2* | *3.8* | *2.3* | *14.6* |
| Lunaith | **16.8** | **18.0** | 6.0 | **7.2** | 21.0 |
| Siora | **16.2** | **18.8** | 5.7 | 6.3 | 19.2 |
| Aurelle | 13.5 | 16.8 | 5.7 | 0.3 | 17.0 |
| Selene | 10.0 | 14.0 | 8.7 | 1.3 | 14.7 |
| Thessaly | 11.2 | 15.8 | 0.0 | 2.7 | 16.0 |
| Mireille | 10.2 | 8.0 | 6.3 | 1.7 | 18.3 |
| Odette | 11.7 | 11.7 | 1.0 | 1.8 | 15.7 |
| Marisol | 9.8 | 9.8 | 2.5 | **0.5** | 11.0 |
| Elara | 7.5 | 6.7 | 3.3 | 2.2 | 10.8 |
| Nimue | **4.7** | 7.8 | 1.5 | 0.8 | 10.5 |
| Mordessa | 6.5 | 6.8 | 1.5 | 2.3 | **8.0** |
| Vesper | 8.5 | **-0.2** | 2.8 | **0.3** | 12.7 |

## Supports

| Hero | Ultimate | Skill 1 | Skill 2 | Passive | Auto attacks |
| --- | --- | --- | --- | --- | --- |
| *role average* | *4.9* | *2.8* | *1.4* | *2.6* | *12.7* |
| Seravelle | 3.8 | 7.5 | **10.0** | **9.0** | 12.3 |
| Elowen | **15.5** | 0.2 | -1.2 | 6.2 | 17.7 |
| Liora | 4.5 | 7.7 | 5.0 | 2.3 | 16.7 |
| Lorelei | 4.7 | 4.2 | -1.2 | 4.7 | **23.0** |
| Velisande | 8.3 | 2.3 | 0.8 | 1.3 | 15.2 |
| Ondine | 5.7 | 3.3 | -1.2 | **0.3** | **19.3** |
| Fenna | 4.2 | 2.8 | 2.5 | 3.3 | 11.7 |
| Tempra | 4.0 | 0.3 | 0.5 | 4.7 | 11.3 |
| Pelagia | 7.3 | 1.8 | 1.0 | **0.0** | 10.3 |
| Rosalind | **2.0** | 2.7 | 2.2 | 1.5 | 11.2 |
| Zephyra | **2.3** | 3.2 | 3.0 | 0.8 | 8.8 |
| Amarante | 4.3 | 0.0 | -0.3 | 3.3 | 10.7 |
| Noctelle | 3.2 | 3.3 | 1.8 | **0.2** | **7.3** |
| Wren | 2.7 | 3.3 | -0.7 | 1.2 | 7.5 |
| Seren | **1.0** | -0.3 | -1.7 | **-0.3** | 7.5 |
