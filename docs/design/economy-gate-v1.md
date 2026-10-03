# Economy gate v1 (phase 2)

Gate (plan doc gate 2): the five Contract slots reach level 200 within 20% of the pacing target, about 700 idle hours (a month of daily collection). **Result: passed, 696 h (29 days), -1%.**

Model: a player collects a full idle chest once a day, clears every stage their level allows (stage about level x 3, the campaign's expected-level curve), takes first-clear rewards, and spends everything on levelling all five slots together. Resource pacing only; star caps are assumed to keep up (copies arrive with summoning in phase 3). Command: `tools/csharp/test.sh -Main Gacha.Tests.Tools.EconomySim`.

```
| Day | Idle hours | Level | Highest stage | Expected level there | Gold/h |
| --- | --- | --- | --- | --- | --- |
| 5 | 120 | 119 | 358 | 119 | 80,694 |
| 10 | 240 | 155 | 451 | 150 | 161,392 |
| 15 | 360 | 171 | 505 | 168 | 241,368 |
| 20 | 480 | 183 | 544 | 181 | 322,792 |
| 25 | 600 | 193 | 574 | 191 | 403,677 |
| 29 | 696 | 200 | 595 | 198 | 472,075 |

Level 200 after 696 idle hours (29 days); target 700 h; -1% off. GATE PASSED
Left over: 128,927,671 gold, 1,736,734 Hero XP, 160,094 Starlight (stars and gear also spend gold).
```

Findings:

- First run: 504 h (21 days, -28%). The plan's 700 h counted idle income only; 600 first clears worth an hour of idle loot each sped levelling up, and Hero XP was the only real limit. Owner's pick (2026-10-03): level XP costs x1.4 (progression.json), which lands at 696 h. Alternatives tried: no XP from first clears (768 h), XP x1.25 with first clears halved (744 h).
- Gold piles up: about 129M left at level 200, against about 20.5M for five heroes' stars and up to about 17M to take 20 Legendary pieces to +20. Gold will need a bigger sink (or smaller income) once summoning and modes exist; worth revisiting with the phase 3 economy.
