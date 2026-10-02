"""Writes heroes.json + combat.json into the repo, and markdown sections for the compendium doc."""
import json, collections, os
from combat_rules import STATS, ENERGY, EFFECTS, CONTROL_RULES, KEYWORDS, TEMPLATES, COST, STYLE_MODS
from revisions import roster_at, PASSES
import kits
from check import stats as hero_stats  # noqa (check.py runs on import; fine for build)

FINAL = max(PASSES)
R = roster_at(FINAL)
import pathlib
REPO = str(pathlib.Path(__file__).resolve().parents[2] / "Assets/_Game/Data") + "/"
KW = dict(KEYWORDS, Drained="enemy has had energy stolen")
CORE_ORDER = ["high", "dark", "nature", "ocean", "arcane"]
CORE_NAME = {"high": "Lumarin (High)", "dark": "Noctyr (Dark)", "nature": "Verdani (Nature)", "ocean": "Thalyri (Ocean)", "arcane": "Aethari (Arcane)"}
ROLE_ORDER = ["tank", "dps", "healer", "support"]
ROLE_NAME = {"tank": "Tank", "dps": "DPS", "healer": "Healer", "support": "Support"}

appliers = collections.defaultdict(set); payers = collections.defaultdict(set)
for h in R:
    for k in h["applies"]: appliers[k].add(h["id"])
    for k in h["payoffs"]: payers[k].add(h["id"])
name = {h["id"]: h["name"] for h in R}
partners = collections.defaultdict(dict)
for k in payers:
    for a in appliers[k]:
        for b in payers[k]:
            if a != b:
                partners[a].setdefault(b, set()).add(k); partners[b].setdefault(a, set()).add(k)

def clean_stats(st):
    out = {}
    for k, v in st.items():
        k2 = "def" if k == "def_" else k
        out[k2] = round(v, 3) if isinstance(v, float) else v
    return out

# ---------- heroes.json ----------
heroes = []
for h in sorted(R, key=lambda h: (CORE_ORDER.index(h["core"]), ROLE_ORDER.index(h["role"]), h["name"])):
    base = dict(moveSpd=3.0, critRate=0.05, critDmg=1.5, effectRes=0.15, haste=0, energyRegen=0, accuracy=0, dodge=0, block=0,
                effectHit=0, lifesteal=0, healPower=0, healRecv=0)
    tpl = TEMPLATES[(h["role"], h["range"])] if (h["role"], h["range"]) in TEMPLATES else TEMPLATES[(h["role"], "ranged")]
    full = dict(base); full.update(tpl)
    for k, v in STYLE_MODS.get(h["style"], {}).items():
        full[k] = full.get(k, 0) + v
    st = clean_stats(full)
    heroes.append({
        "id": h["id"], "name": h["name"], "core": h["core"], "subrace": h["subrace"], "role": h["role"], "style": h["style"],
        "range": h["range"], "targeting": h["targeting"], "movement": h["movement"],
        "stats": st,
        "skills": {
            "ultimate": {"name": h["ultimate"]["name"], "text": h["ultimate"]["text"], "tags": h["ultimate"]["tags"]},
            "skill1": {"name": h["skill1"]["name"], "cooldown": h["skill1"]["cd"], "text": h["skill1"]["text"], "tags": h["skill1"]["tags"]},
            "skill2": {"name": h["skill2"]["name"], "cooldown": h["skill2"]["cd"], "text": h["skill2"]["text"], "tags": h["skill2"]["tags"]},
            "passive": {"name": h["passive"]["name"], "text": h["passive"]["text"], "tags": h["passive"]["tags"]},
        },
        "applies": h["applies"], "payoffs": h["payoffs"],
        "synergy": sorted([{"hero": b, "via": sorted(ks)} for b, ks in partners[h["id"]].items()], key=lambda x: x["hero"]),
        "art": {"look": h["look"], "fullbody": f"hero_{h['id']}_fullbody", "card": f"hero_{h['id']}_card", "icon": f"hero_{h['id']}_icon"},
        "maxStars": 5, "bond": None,
    })
    kits.apply(heroes[-1])
json.dump({"_schema": "hero v3 — 60 heroes; ids are permanent; text values are tuning starting points. Generated from the Hero Compendium build (pass %d)." % FINAL,
           "heroes": heroes}, open(REPO + "heroes.json", "w"), indent=1, ensure_ascii=False)

combat = {"_schema": "combat v1 — stat sheet, energy, effect library, control rules, synergy keywords, role templates. Starting values for tuning.",
          "stats": [{"id": i, "name": n, "rule": r, "default": d} for i, n, r, d in STATS],
          "energy": ENERGY, "effects": EFFECTS, "control": CONTROL_RULES, "keywords": KW,
          "roleTemplates": {f"{a}/{b}": clean_stats(v) for (a, b), v in TEMPLATES.items()},
          "styleMods": {k: clean_stats(v) for k, v in STYLE_MODS.items()},
          "budgetCost": clean_stats(COST)}
json.dump(combat, open(REPO + "combat.json", "w"), indent=1, ensure_ascii=False)

# ---------- markdown for the doc ----------
os.makedirs("md", exist_ok=True)
def esc(s): return s.replace("|", "/")
for c in CORE_ORDER:
    rows = [h for h in heroes if h["core"] == c]
    beats = {"high": "Noctyr", "dark": "Verdani", "nature": "Thalyri", "ocean": "Aethari", "arcane": "Lumarin"}[c]
    lines = [f"### {CORE_NAME[c]} · beats {beats}", "",
             "| Hero | Role and style | Targets and moves | Ultimate | Skill 1 | Skill 2 | Passive | Best partners |",
             "| --- | --- | --- | --- | --- | --- | --- | --- |"]
    for h in sorted(rows, key=lambda h: (ROLE_ORDER.index(h["role"]), h["name"])):
        s = h["skills"]
        syn = sorted(h["synergy"], key=lambda x: (-len(x["via"]), x["hero"]))[:3]
        syn_txt = "; ".join(f"{name[x['hero']]} ({', '.join(x['via'])})" for x in syn)
        lines.append(f"| **{h['name']}** ({h['subrace'].title()}) | {ROLE_NAME[h['role']]} · {h['style']} | {esc(h['targeting'])}; {esc(h['movement']).lower()} | "
                     f"**{s['ultimate']['name']}**: {esc(s['ultimate']['text'])} | **{s['skill1']['name']}** ({s['skill1']['cooldown']}s): {esc(s['skill1']['text'])} | "
                     f"**{s['skill2']['name']}** ({s['skill2']['cooldown']}s): {esc(s['skill2']['text'])} | **{s['passive']['name']}**: {esc(s['passive']['text'])} | {syn_txt} |")
    open(f"md/core_{c}.md", "w").write("\n".join(lines))

# keyword table
kl = ["| Keyword | Means | Applied by | Paid off by |", "| --- | --- | --- | --- |"]
for k in sorted(set(appliers) | set(payers)):
    if not payers[k]: continue
    kl.append(f"| {k} | {KW.get(k, '')} | {', '.join(sorted(name[x] for x in appliers[k]))} | {', '.join(sorted(name[x] for x in payers[k]))} |")
open("md/keywords.md", "w").write("\n".join(kl))

# revision log summary
counts = {}
import subprocess
for p in range(0, FINAL + 1):
    r = json.load(open(f"report_pass{p}.json")) if os.path.exists(f"report_pass{p}.json") else None
    counts[p] = r["issue_count"] if r else None
rl = ["| Pass | Focus | Edits | Issues found after |", "| --- | --- | --- | --- |",
      f"| 0 | First draft of 60 heroes | n/a | {counts[0]} |",
      f"| 1 | Synergy holes, power bands, look-alike kits, race identity, counters | {len(PASSES[1])} | {counts[1]} |",
      f"| 2 | Remaining outliers | {len(PASSES[2])} | {counts[2]} |",
      f"| 3 | Manual read: damage numbers, duplicate buffs, control caps, wording | {len(PASSES[3])} | {counts[3]} |",
      f"| 4 | Synergy fairness: no must-pick hub | {len(PASSES[4])} | {counts[4]} |", f"| 5 | Final read of the published tables: duplicate effect, naming clashes | {len(PASSES[5])} | {counts[5]} |",
      f"| 6 | Tag audit by the executable kits (battle tests) | {len(PASSES[6])} | {counts[6]} |"]
open("md/revlog.md", "w").write("\n".join(rl))

# role/race summary
rc = collections.Counter((h["core"], h["role"]) for h in heroes)
sl = ["| Race | Tank | DPS | Healer | Support | Total |", "| --- | --- | --- | --- | --- | --- |"]
for c in CORE_ORDER:
    sl.append(f"| {CORE_NAME[c]} | " + " | ".join(str(rc[(c, r)]) for r in ROLE_ORDER) + f" | {sum(rc[(c, r)] for r in ROLE_ORDER)} |")
tot = collections.Counter(h["role"] for h in heroes)
sl.append("| **All** | " + " | ".join(f"**{tot[r]}**" for r in ROLE_ORDER) + f" | **{len(heroes)}** |")
open("md/summary.md", "w").write("\n".join(sl))
pp = {name[k]: len(v) for k, v in partners.items()}
print("built", len(heroes), "heroes; partners min/max", min(pp.values()), max(pp.values()))
