"""Executable kits: what every ability actually does in the battle sim.

build.py merges this into heroes.json: each skill gets "ops", the passive gets "mods"/"triggers"/"flags",
and each hero gets "ai" (enemy target, ally target, movement). The Core battle engine reads only this data.

Numbers that the kit text leaves open ("Heals one ally", "Burning area") use the defaults below;
all of them are tuning starting points. Vocabulary (ops, selectors, statuses, conditions) is documented
in docs/design/battle-rules.md and validated by the C# data tests.
"""

# ---------- helpers ----------
def op(name, **kw):
    d = {"op": name}; d.update(kw); return d

def dmg(m, to="target", **kw): return op("dmg", m=m, to=to, **kw)
def heal(to="ally", m=None, pct=None, **kw):
    d = op("heal", to=to, **kw)
    if m is not None: d["m"] = m
    if pct is not None: d["pct"] = pct
    return d
def shield(to="self", m=None, pct=None, pctTarget=None, **kw):
    d = op("shield", to=to, **kw)
    for k, v in (("m", m), ("pct", pct), ("pctTarget", pctTarget)):
        if v is not None: d[k] = v
    return d
def st(s, to="target", dur=None, v=None, **kw):
    d = op("status", s=s, to=to, **kw)
    if dur is not None: d["dur"] = dur
    if v is not None: d["v"] = v
    return d
def energy(v, to="self", **kw): return op("energy", v=v, to=to, **kw)
def cleanse(to="ally", n=99): return op("cleanse", to=to, n=n)
def dispel(to="target", n=99, **kw): return op("dispel", to=to, n=n, **kw)
def move(kind, to, dist=0.0): return op("move", kind=kind, to=to, dist=dist)
def teleport(to, where="behind"): return op("teleport", to=to, where=where)
def zone(dur, r, at, every, ops): return op("zone", dur=dur, r=r, at=at, every=every, ops=ops)
def summon(unit, n=1, dur=None, **kw):
    d = op("summon", unit=unit, n=n, **kw)
    if dur is not None: d["dur"] = dur
    return d
def mod(k, v, when=None, scope="self", **kw):
    d = {"k": k, "v": v, "scope": scope}
    if when: d["if"] = when
    d.update(kw); return d
def on(event, *ops, when=None, icd=None):
    d = {"on": event, "ops": list(ops)}
    if when: d["if"] = when
    if icd: d["icd"] = icd
    return d
def P(mods=(), triggers=(), flags=()):
    return {"mods": list(mods), "triggers": list(triggers), "flags": list(flags)}

# Pass 10 package conditions (merged keywords; see combat_rules.KEYWORDS)
HINDERED = "tgt:slow|root+3|stun+3|airborne+3"
DREAD = "tgt:sleep+3|fear+3|charm+3"
EXPOSED = "tgt:mark|weaken|def_down|curse|feeding_curse"
WOUNDS = "tgt:bleed|poison"
DISRUPTED = "tgt:blind|silence|drained"

# Defaults for numbers the kit text leaves open
HEAL = 2.0          # single-target heal, x ATK
SHIELD_PCT = 0.15   # self shield, x own max HP
BURN = 0.3          # burn per second, x caster ATK (stacks to 3)
BLEED = 0.25        # bleed per second, x caster ATK (doubled while moving)
POISON = 0.02       # poison per second, x target max HP
SLOW = 0.3
BLIND = 0.5

# ---------- AI: targeting text -> (enemy target, ally target); movement text -> movement mode ----------
ENEMY_TARGET = {
    "Nearest": "nearest", "Highest ATK": "highest_atk", "Highest ATK enemy": "highest_atk",
    "Enemy carry (highest ATK)": "highest_atk", "Densest enemy group": "densest", "Enemy healer first": "healer_first",
    "Enemy nearest the weakest ally": "near_weakest_ally", "Enemy with the most energy": "most_energy",
    "Highest-HP enemy": "highest_hp", "Isolated enemy, else carry": "isolated_else_carry", "Lowest-HP enemy": "lowest_hp",
    "Lowest-HP back-row enemy": "lowest_hp_back", "Most-buffed enemy": "most_buffed",
    "Nearest bleeding enemy, else nearest": "nearest_with:bleed", "Nearest blinded enemy, else nearest": "nearest_with:blind",
    "Random": "random", "Rooted enemy, else nearest": "nearest_with:root",
    "Soaked enemy, else highest ATK": "with_else_highest_atk:soaked", "Her beast's target": "nearest",
}
ALLY_TARGET = {
    "Lowest-HP ally": "lowest_hp", "Most-debuffed ally": "most_debuffed", "Ally who lost the most HP recently": "lost_most_recent",
    "Follows the ally closest to a full ultimate": "closest_to_ult", "Follows the ally with the longest cooldowns": "longest_cooldowns",
    "Follows the assassin": "assassin", "Follows the fastest ally": "fastest", "Follows the main damage dealer": "highest_atk",
    "Follows the strongest ally": "highest_atk",
}
MOVE = {
    "Holds range": "kite", "Holds long range": "kite", "Mid range": "kite", "Stays back": "kite", "Stays mid-field": "follow",
    "Stays out of sight": "follow", "Stays behind the carry": "follow", "Flits around": "follow", "Rides above the back line": "kite",
    "Stands beside the weakest ally": "guard",
    # divers wait for their front line to engage, then go for the back line
    "Dives": "dive", "Flies over the front line": "dive", "Glides above the field": "dive", "Dashes in and duels": "dive",
}

def ai_for(h):
    t = h["targeting"]
    enemy = ENEMY_TARGET.get(t, "nearest")
    ally = ALLY_TARGET.get(t, "lowest_hp" if h["role"] == "healer" else "self")
    mv = MOVE.get(h["movement"], "advance")
    if mv == "follow" and ally == "self": mv = "kite"
    return {"enemy": enemy, "ally": ally, "move": mv}

# ---------- the 60 kits ----------
K = {}

# Lumarin (High)
K["cassia"] = dict(
    ult=[zone(5, 2.5, "self", 1, [dmg(0.25, "zone_enemies")])],
    s1=[dmg(1.4, drain=0.5)],
    s2=[dispel("target")],
    p=P(mods=[mod("lifesteal_heal", 0.3)], triggers=[on("dispel_done", shield("self", pct=0.05), )]))
K["isolde"] = dict(
    ult=[st("redirect", "allies_except_self", 4, 0.5), st("dr", "self", 4, 0.3)],
    s1=[st("redirect", "weakest_ally_other", 5, 0.25)],
    s2=[dmg(1.5), cleanse("self")],
    p=P(mods=[mod("heal_in", 0.5)]))
K["valeria"] = dict(
    ult=[shield("allies", pct=0.2), st("taunt", "enemies", 3)],
    s1=[st("stun", "nearest_enemy", 1)],
    s2=[st("def_up", "allies_near_self:3", 5, 0.3)],
    p=P(mods=[mod("dmg_in", -0.1, scope="allies_behind")]))
K["ilyra"] = dict(
    ult=[dmg(1.7, "enemies", elem="fire"), st("revive_ready", "self", 10, 0.3)],
    s1=[zone(3, 2.0, "target", 1, [st("burn", "zone_enemies", 3, BURN)])],
    s2=[move("push", "enemies_near_self:2.5", 2.0)],
    p=P(mods=[mod("dmg_out", 0.05, perStack="burn")], triggers=[on("energy_from_ally", st("atk_up", "self", 4, 0.2), icd=4)]))
K["lucienne"] = dict(
    ult=[dmg(3.2, "line")],
    s1=[st("counter", "self", 3, 0.75), shield("self", pct=0.20)],
    s2=[st("blind", "nearest_enemy", 1.5, BLIND)],
    p=P(mods=[mod("crit_dmg", 0.6, DISRUPTED + "|slow|root+3|stun+3|airborne+3")]))
K["ophira"] = dict(
    ult=[dmg(2.7, "highest_atk_enemy"), st("airborne", "enemies_near_target:2")],
    s1=[dmg(1.6)],
    s2=[st("untargetable", "self", 1), shield("self", pct=0.15)],
    p=P(mods=[mod("dmg_out", 0.3, HINDERED)]))
K["solenne"] = dict(
    ult=[dmg(2.4, "target_row")],
    s1=[st("mark", "target", 6, 0.15)],
    s2=[st("blind", "cone", 2, BLIND), st("atk_up", "allies_near_self:4", 4, 0.25)],
    p=P(mods=[mod("dmg_out", 0.25, EXPOSED)], triggers=[on("crit", st("blind", "evt", 2, BLIND), when="tgt:mark")]))
K["aurelle"] = dict(
    ult=[heal("allies", pct=0.3), st("revive_guard", "self", 8, 0.3)],
    s1=[heal("ally", m=2.2)],
    s2=[heal("weakest_allies:2", m=1.2, over=4)],
    p=P(triggers=[on("overheal", shield("evt", fromEvent=1.0))]))
K["elara"] = dict(
    ult=[heal("allies", pct=0.25), st("cc_immunity", "allies", 4)],
    s1=[heal("weakest_ally", m=1.6, over=4)],
    s2=[cleanse("ally")],
    p=P(mods=[mod("heal_out", 0.4, "tgt.buffed|tgt.shielded")], triggers=[on("heal_given", st("halo", "evt", 6, 0.05))]))
K["amarante"] = dict(
    ult=[dispel("enemies"), st("silence", "enemies", 2)],
    s1=[st("def_down", "target", 5, 0.5)],
    s2=[st("stun", "target", 1)],
    p=P(mods=[mod("dmg_in", 0.1, EXPOSED, scope="enemies")], triggers=[on("debuff_applied", st("dmg_down", "evt", 4, 0.1))]))
K["rosalind"] = dict(
    ult=[st("def_up", "allies", 6, 0.3), cleanse("allies", 1)],
    s1=[st("def_up", "most_targeted_ally", 5, 0.3)],
    s2=[dispel("most_buffed_enemy", 1, steal=True)],
    p=P(mods=[mod("dmg_out", 0.06, scope="allies", perBuff=5)]))
K["seravelle"] = dict(
    ult=[st("atk_up", "allies", 6, 0.25)],
    s1=[cleanse("ally")],
    s2=[st("untargetable", "ally", 1.5)],
    p=P(triggers=[on("skill_cast", energy(8, "allies_except_self"))]))

# Noctyr (Dark)
K["corvina"] = dict(
    ult=[st("unkillable", "self", 4), st("taunt", "enemies", 3)],
    s1=[shield("self", pct=0.2)],
    s2=[move("pull_to_caster", "backrow_enemy")],
    p=P(triggers=[on("ally_death", shield("self", pct=0.2))]))
K["sangrael"] = dict(
    ult=[dmg(1.2, "enemies_near_self:2.5", drain=1.0)],
    s1=[st("taunt", "enemies_near_self:3", 2), st("lifesteal_up", "allies_near_self:3", 4, 0.15)],
    s2=[dmg(1.0, "enemies_near_self:2.5"), st("bleed", "enemies_near_self:2.5", 4, BLEED)],
    p=P(mods=[mod("dmg_in", -0.25, "self.hp<0.3")]))
K["maelis"] = dict(
    ult=[st("burn", "enemies", 6, 0.35)],
    s1=[dmg(1.7), st("curse", "target", 5, 0.2)],
    s2=[st("blind", "target", 2, BLIND)],
    p=P(mods=[mod("dmg_out", 0.35, "tgt:burn|curse|feeding_curse")]))
K["nyx"] = dict(
    ult=[teleport("weakest_enemy"), dmg(3.0, "weakest_enemy", execute=0.25), st("stealth", "self", 2)],
    s1=[st("stealth", "self", 3), heal("self", pct=0.15)],
    s2=[teleport("target"), st("sure_crit", "self", 5)],
    p=P(mods=[mod("crit_dmg", 0.6, "src:stealth+3|sure_crit|" + DREAD)], flags=["prefer_marked"]))
K["ravenna"] = dict(
    ult=[dmg(0.6, "random_enemy_near_self:2.5", hits=6, perHitTarget=True, then=[st("bleed", "hit", 4, BLEED)])],
    s1=[dmg(1.4), st("bleed", "target", 4, BLEED)],
    s2=[st("aspd_up", "self", 4, 0.3)],
    p=P(mods=[mod("dmg_out", 0.35, WOUNDS)], triggers=[on("hit", heal("self", pct=0.03), when=WOUNDS, icd=0.5)]))
K["sable"] = dict(
    ult=[dmg(1.5, "enemies_near_self:3"), op("execute", to="enemies", below=0.25, killEnergy=50)],
    s1=[dmg(1.5, "front_row")],
    s2=[dmg(0.5, "weakest_enemy"), st("bleed", "weakest_enemy", 4, 0.3)],
    p=P(mods=[mod("dmg_out", 0.35, WOUNDS)], triggers=[on("enemy_death", energy(15))], flags=["no_revive_on_kill"]))
K["mordessa"] = dict(
    ult=[st("lifesteal_up", "allies", 6, 0.3), heal("allies", pct=0.1)],
    s1=[op("spend_hp", pct=0.1), heal("weakest_ally_other", pct=0.25)],
    s2=[st("bond_heal", "weakest_allies:2", 6, 0.2)],
    p=P(triggers=[on("ally_lifesteal", energy(3), icd=0.5)]))
K["thessaly"] = dict(
    ult=[heal("allies", pct=0.2), st("stealth", "allies", 2)],
    s1=[heal("ally", m=HEAL), st("def_up", "ally", 4, 0.3)],
    s2=[st("share", "weakest_allies:2", 5, 0.5)],
    p=P(triggers=[on("heal_given", energy(5, "evt"))]))
K["vesper"] = dict(
    ult=[dmg(1.4, "enemies", healTeam=0.8)],
    s1=[st("feeding_curse", "target", 5, 0.30)],
    s2=[st("hit_shield", "weakest_ally", 8)],
    p=P(mods=[mod("heal_out", 0.4, "any_enemy:bleed|poison")]))
K["liora"] = dict(
    ult=[st("burn", "enemies", 4, BURN), st("def_down", "enemies", 4, 0.2)],
    s1=[energy(-20, "highest_atk_enemy")],
    s2=[st("blind", "back_row", 4, 0.2)],
    p=P(mods=[mod("heal_in", -0.3, "tgt:burn", scope="enemies"), mod("dmg_in", 0.15, DISRUPTED, scope="enemies")]))
K["noctelle"] = dict(
    ult=[st("stealth", "allies", 2), st("sure_crit", "allies", 4)],
    s1=[st("stealth", "ally", 1.5)],
    s2=[st("fear", "target", 1.5)],
    p=P(mods=[mod("dmg_out", 0.3, "src:stealth+3", scope="allies")]))
K["velisande"] = dict(
    ult=[st("fear", "enemies", 2)],
    s1=[st("lifesteal_up", "allies", 5, 0.15)],
    s2=[st("antiheal", "target", 5, 0.5)],
    p=P(mods=[mod("dmg_in", 0.15, DREAD, scope="enemies")]))

# Verdani (Nature)
K["briar"] = dict(
    ult=[st("reflect", "self", 4, 0.3), st("dr", "self", 4, 0.3)],
    s1=[st("taunt", "target", 2), st("bleed", "target", 4, 0.2)],
    s2=[st("planted", "self", 3), st("def_up", "self", 3, 0.6)],
    p=P(mods=[mod("reflect_melee", 0.15), mod("dmg_in", 0.2, WOUNDS, scope="enemies")]))
K["eldrith"] = dict(
    ult=[st("taunt", "enemies", 5), st("regen", "self", 5, 0.10), st("planted", "self", 5)],
    s1=[st("def_up", "self", 5, 0.3)],
    s2=[st("root", "nearest_enemies:2", 1.5)],
    p=P(mods=[mod("heal_in", 0.25)]))
K["hartwen"] = dict(
    ult=[move("pull_to_center", "enemies"), st("root", "enemies", 2)],
    s1=[dmg(1.0), st("airborne", "target")],
    s2=[st("root", "nearest_enemy", 1.5), st("taunt", "nearest_enemy", 1.5)],
    p=P(mods=[mod("dmg_in", 0.15, HINDERED, scope="enemies")]))
K["kaida"] = dict(
    ult=[summon("wolf", 2, 10)],
    s1=[teleport("backrow_enemy"), dmg(1.8, "backrow_enemy")],
    s2=[st("aspd_up", "self_and_summons", 5, 0.2)],
    p=P(mods=[mod("dmg_out", 0.15, "self.beast"), mod("dmg_out", 0.3, HINDERED)]))
K["rhiannon"] = dict(
    ult=[dmg(2.6, "line"), st("airborne", "line")],
    s1=[dmg(1.8)],
    s2=[st("atk_up", "self_and_summons", 5, 0.25)],
    p=P(triggers=[on("battle_start", summon("stag", 1)), on("energy_from_ally", dmg(0.8, "target"), icd=2)]))
K["sylwen"] = dict(
    ult=[dmg(1.6, "weakest_enemies:3", ignoreDef=0.3)],
    s1=[st("root", "target", 1)],
    s2=[dmg(1.2, "enemies_near_target:2")],
    p=P(mods=[mod("crit_rate", 0.25, HINDERED + "|mark|weaken|def_down|curse|feeding_curse")], flags=["see_stealth"]))
K["venna"] = dict(
    ult=[st("poison", "enemies", 4, 0.06)],
    s1=[st("poison", "target", 4, 0.03)],
    s2=[st("poison", "enemies_near_target:2", 3, 0.03), st("slow", "enemies_near_target:2", 3, SLOW)],
    p=P(triggers=[on("enemy_death", st("poison", "evt_nearest_ally", 4, POISON), when="tgt:poison")]))
K["mireille"] = dict(
    ult=[heal("allies", pct=0.4, over=8), st("regen", "allies", 4, 0.02, delay=8)],
    s1=[shield("ally", m=1.5), st("thorns", "ally", 8, 0.2)],
    s2=[st("seed", "ally", 12, 0.3)],
    p=P(mods=[mod("heal_out", 0.2, "tgt.hp<0.5")]))
K["nimue"] = dict(
    ult=[op("revive", to="dead_ally", pct=0.4)],
    s1=[heal("ally", m=1.6), st("regen", "ally", 4, 0.03)],
    s2=[shield("ally", m=1.8)],
    p=P(mods=[mod("dmg_in", -0.2, "tgt.hp<0.2", scope="allies")]))
K["fenna"] = dict(
    ult=[st("sleep", "enemies", 2)],
    s1=[summon("decoy", 1, 3, tauntR=3)],
    s2=[st("slow", "enemies_near_target:2", 3, SLOW), st("poison", "enemies_near_target:2", 3, POISON)],
    p=P(mods=[mod("dodge", 0.1, scope="allies_near:3")]))
K["wren"] = dict(
    ult=[st("weaken", "enemies_near_target:3", 5, 0.4)],
    s1=[st("root", "target", 2)],
    s2=[cleanse("most_debuffed_ally", 1)],
    p=P(mods=[mod("dmg_out", -0.15, "src:slow|root+3|stun+3|airborne+3", scope="enemies"), mod("dmg_in", 0.1, HINDERED, scope="enemies")]))
K["zephyra"] = dict(
    ult=[st("aspd_up", "allies", 5, 0.3)],
    s1=[st("sleep", "target", 2)],
    s2=[energy(20, "ally")],
    p=P(mods=[mod("crit_rate", 0.25, DREAD, scope="allies")]))

# Thalyri (Ocean)
K["coralie"] = dict(
    ult=[st("projectile_block", "allies", 2), st("soaked", "enemies_near_self:4", 4)],
    s1=[shield("self", pct=0.12), shield("nearest_ally", pct=0.12)],
    s2=[st("taunt", "enemies", 2)],
    p=P(mods=[mod("block", 0.2)]))
K["thalassa"] = dict(
    ult=[move("pull_to_caster", "front_row"), st("slow", "front_row", 3, SLOW), st("soaked", "front_row", 3)],
    s1=[st("stun", "target", 1)],
    s2=[move("pull_to_caster", "backrow_enemy")],
    p=P(mods=[mod("dmg_in", 0.2, "tgt.isolated", scope="enemies")]))
K["calypso"] = dict(
    ult=[move("pull_to_caster", "highest_atk_enemy"), dmg(3.0, "highest_atk_enemy", drain=0.5)],
    s1=[dmg(1.5)],
    s2=[st("charm", "target", 1.5)],
    p=P(mods=[mod("dmg_out", 0.3, "tgt.isolated|" + DREAD), mod("crit_rate", 1.0, "src:stealth")],
        triggers=[on("battle_start", st("stealth", "self", 3))]))
K["halcyra"] = dict(
    ult=[op("chain", m=1.4, bounces=6, to="target", elem="lightning", bonusFrom="tempest", soakedDouble=True)],
    s1=[dmg(0.8, elem="lightning"), st("soaked", "target", 4)],
    s2=[st("stun", "enemies_near_target:2", 0.5)],
    p=P(triggers=[on("crit", op("chain", m=0.5, bounces=1, to="evt", elem="lightning", skipFirst=True), icd=0.5),
                  on("skill_cast", op("counter", key="tempest", add=1))]))
K["nerissa"] = dict(
    ult=[zone(3, 2.5, "target", 0.5, [move("pull_to_point", "zone_enemies", 0.5), dmg(0.35, "zone_enemies")])],
    s1=[teleport("backrow_enemy"), dmg(2.4, "backrow_enemy")],
    s2=[st("slow", "target", 3, SLOW)],
    p=P(triggers=[on("hit", energy(5), when=HINDERED, icd=0.5)]))
K["ysra"] = dict(
    ult=[dmg(4.5, "weakest_with:soaked|mark", elem="lightning")],
    s1=[dmg(1.7, elem="lightning")],
    s2=[st("soaked", "enemies_near_target:2", 4), st("slow", "enemies_near_target:2", 4, SLOW)],
    p=P(mods=[mod("dmg_out", 0.4, "tgt:soaked|mark")],
        triggers=[on("hit", op("chain", m=0.3, bounces=1, to="evt", elem="lightning", skipFirst=True, onlySoaked=True), when="tgt:soaked", icd=1)]))
K["marisol"] = dict(
    ult=[zone(6, 99, "self", 1, [heal("zone_allies", pct=0.04), st("soaked", "zone_enemies", 2)])],
    s1=[heal("ally", m=HEAL)],
    s2=[cleanse("most_debuffed_allies:2")],
    p=P(triggers=[on("overheal", energy(0, "evt", fromOverheal=0.5))]))
K["odette"] = dict(
    ult=[shield("allies", pctTarget=0.25)],
    s1=[heal("ally", m=1.2), shield("ally", m=1.0)],
    s2=[cleanse("allies")],
    p=P(mods=[mod("cc_immune", 1, "tgt.shielded", scope="allies")]))
K["siora"] = dict(
    ult=[zone(4, 99, "self", 1, [heal("zone_allies", pct=0.05), st("slow", "zone_enemies", 1.5, SLOW)])],
    s1=[heal("ally", m=HEAL)],
    s2=[heal("weakest_ally_other", lastHeal=0.5), cleanse("weakest_ally_other", 1)],
    p=P(triggers=[on("heal_given", st("aspd_up", "evt", 3, 0.15))]))
K["lorelei"] = dict(
    ult=[st("charm", "highest_atk_enemies:2", 2.5)],
    s1=[energy(-25, "target")],
    s2=[st("silence", "most_energy_enemy", 2)],
    p=P(triggers=[on("hit", energy(-5, "evt"), when="!tgt:soaked"), on("hit", energy(-10, "evt"), when="tgt:soaked")]))
K["ondine"] = dict(
    ult=[st("blind", "enemies", 3, BLIND)],
    s1=[st("stun", "target", 1)],
    s2=[st("soaked", "target", 5), st("def_down", "target", 5, 0.3)],
    p=P(mods=[mod("dmg_in", 0.2, "tgt:soaked|blind|silence|drained", scope="enemies")]))
K["pelagia"] = dict(
    ult=[st("soaked", "enemies", 4), move("push", "enemies", 2.0), energy(20, "allies")],
    s1=[st("soaked", "target", 3), st("slow", "target", 3, SLOW)],
    s2=[cleanse("most_debuffed_ally")],
    p=P(mods=[mod("aspd", -0.15, HINDERED, scope="enemies"), mod("dmg_in", 0.1, HINDERED, scope="enemies")]))

# Aethari (Arcane)
K["draxa"] = dict(
    ult=[st("dragonform", "self", 8, 0.5)],
    s1=[st("def_up", "self", 5, 0.3)],
    s2=[move("push", "enemies_near_self:2.5", 2.0)],
    p=P(mods=[mod("dmg_in", 0.2, "tgt:burn", scope="enemies")], flags=["immune:burn"]))
K["runa"] = dict(
    ult=[st("skill_shield", "allies", 10)],
    s1=[shield("self", pct=SHIELD_PCT)],
    s2=[st("silence", "nearest_enemy", 1.5)],
    p=P(mods=[mod("reflect_skill", 0.2)]))
K["vaela"] = dict(
    ult=[st("projectile_block", "allies", 2)],
    s1=[shield("self", pct=SHIELD_PCT), st("silence", "nearest_enemy", 1)],
    s2=[move("pull_to_caster", "enemies_near_self:4"), st("taunt", "enemies_near_self:4", 2)],
    p=P(mods=[mod("dmg_in", 0.1, "tgt.grouped|tgt:taunt", scope="enemies")]))
K["astraea"] = dict(
    ult=[dmg(2.8, "enemies_near_target:2.5")],
    s1=[dmg(2.0)],
    s2=[st("airborne", "line")],
    p=P(mods=[mod("dmg_out", 0.3, "tgt.grouped|tgt:taunt"), mod("dmg_out", 1.0, "tgt.summon")]))
K["caelith"] = dict(
    ult=[dmg(0, pctMaxHp=0.15, hits=3)],
    s1=[dmg(1.7)],
    s2=[dispel("target", 1), st("silence", "target", 1)],
    p=P(mods=[mod("dmg_vs_shield", 0.3), mod("dmg_out", 0.3, EXPOSED)]))
K["isaura"] = dict(
    ult=[energy(-25, "enemies", dmgPerEnergy=0.05)],
    s1=[energy(-10, "target", steal=True)],
    s2=[teleport("self", "away"), st("untargetable", "self", 0.5)],
    p=P(mods=[mod("dmg_out", 0.3, DISRUPTED + "|sleep+3|fear+3|charm+3")]))
K["zaria"] = dict(
    ult=[dmg(2.6, "cone", elem="fire"), st("burn", "cone", 4, BURN)],
    s1=[dmg(1.7)],
    s2=[st("burn", "target_row", 4, BURN)],
    p=P(mods=[mod("dmg_out", 0.4, "tgt:burn")]))
K["lunaith"] = dict(
    ult=[heal("allies", pct=0.25), st("cc_immunity", "allies", 4, only=["sleep", "stun"])],
    s1=[heal("ally", m=HEAL)],
    s2=[st("sleep", "target", 2)],
    p=P(triggers=[on("heal_given", energy(10, "evt"))]))
K["selene"] = dict(
    ult=[op("rewind", to="allies", sec=3)],
    s1=[heal("ally", m=1.6), energy(10, "ally")],
    s2=[shield("weakest_ally", m=1.8, breakHeal=1.0)],
    p=P(triggers=[on("heal_given", cleanse("evt", 1))]))
K["elowen"] = dict(
    ult=[energy(25, "allies_except_self"), st("aspd_up", "allies", 5, 0.2)],
    s1=[st("slow", "target", 3, SLOW)],
    s2=[st("haste_up", "ally", 4, 30)],
    p=P(mods=[mod("energy_gain", 0.15, scope="allies")]))
K["seren"] = dict(
    ult=[st("crit_up", "allies", 6, 0.3)],
    s1=[st("mark", "target", 6, 0.15), st("star_mark", "target", 6, 5)],
    s2=[st("thread", "ally", 5, 0.3)],
    p=P(mods=[mod("crit_dmg", 0.15, scope="allies")]))
K["tempra"] = dict(
    ult=[op("cooldown", to="allies", reset=True)],
    s1=[op("cooldown", to="ally", advance=3)],
    s2=[st("stasis", "target", 1.5)],
    p=P(mods=[mod("haste", 15, scope="allies")]))

# Per-hero auto attack strength (fraction of ATK). Heroes not listed use battle.json basicAttackPct.
AUTO_PCT = {
    # auto-attack specialists: duelists, marksmen, reapers, beast fighters, divers
    **{h: 1.0 for h in ["ravenna", "lucienne", "kaida", "sable", "sylwen", "ophira", "nyx", "nerissa", "rhiannon"]},
    # casters: their power is meant to be their skills
    **{h: 0.6 for h in ["ilyra", "ysra", "astraea", "zaria", "halcyra", "isaura", "maelis", "caelith"]},
}

# Requirements: an ability waits (energy stays full / cooldown stays ready) until this is true.
REQUIRES = {("nimue", "ult"): "dead_ally"}


def apply(hero_json):
    """Merge executable kit data into one hero dict from heroes.json (in place)."""
    k = K[hero_json["id"]]
    sk = hero_json["skills"]
    sk["ultimate"]["ops"] = k["ult"]
    sk["skill1"]["ops"] = k["s1"]
    sk["skill2"]["ops"] = k["s2"]
    for key, name in (("ult", "ultimate"), ("s1", "skill1"), ("s2", "skill2")):
        if (hero_json["id"], key) in REQUIRES: sk[name]["requires"] = REQUIRES[(hero_json["id"], key)]
    sk["passive"].update(k["p"])
    hero_json["ai"] = ai_for(hero_json)
    if hero_json["id"] in AUTO_PCT: hero_json["basicPct"] = AUTO_PCT[hero_json["id"]]
