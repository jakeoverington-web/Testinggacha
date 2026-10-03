"""Our combat rules: stat sheet, effect library, keyword list, role stat templates and power costs.
Written to Assets/_Game/Data/combat.json by build.py. Values are starting points for tuning."""

# ---------- Stats (real-time model) ----------
STATS = [
    # id, name, what it does, default
    ("hp", "Health", "Hero falls at 0.", None),
    ("atk", "Attack", "Scales damage, healing and shields.", None),
    ("def", "Defence", "Damage taken x K/(K+DEF), K = 300 at level 1.", None),
    ("atkSpd", "Attack speed", "Basic attacks per second.", None),
    ("moveSpd", "Move speed", "Metres per second on the field (field is ~10 m wide).", 3.0),
    ("range", "Range", "Attack reach in metres. Melee 1, ranged 4-6.", None),
    ("critRate", "Crit chance", "Chance a hit crits.", 0.05),
    ("critDmg", "Crit damage", "Crit multiplier. Base 150%.", 1.5),
    ("haste", "Haste", "Skill cooldowns / (1 + haste/100). Also speeds energy gain.", 0),
    ("energyRegen", "Energy gain", "Bonus % to energy gained from hits.", 0),
    ("accuracy", "Accuracy", "Cancels the target's dodge.", 0),
    ("dodge", "Dodge", "Chance to avoid a basic attack completely (skills can't be dodged).", 0),
    ("block", "Block", "Chance to halve a hit's damage.", 0),
    ("effectHit", "Effect hit", "Raises the chance debuffs land.", 0),
    ("effectRes", "Effect resist", "Chance to resist a debuff = effectRes - effectHit, never below 15% (Summoners War floor) unless the skill says 'cannot be resisted'.", 0.15),
    ("lifesteal", "Lifesteal", "Heals for % of damage dealt.", 0),
    ("healPower", "Healing power", "Bonus % to heals and shields the hero gives.", 0),
    ("healRecv", "Healing received", "Bonus % to heals and shields the hero receives.", 0),
]

ENERGY = {
    "max": 100,
    "basicHit": 10,        # AFK Arena: normal attacks give 70-120 of 1000
    "perPctHpLost": 0.5,   # being hit: +0.5 energy per 1% max HP lost
    "kill": 20,            # AFK Arena: 200 of 1000 on a kill
    "startEnergy": 0,
    "note": "Ultimate fires automatically at 100 (manual tap when Auto is off). Energy carries over between waves.",
}

# ---------- Effect library (two strength tiers like Raid: Shadow Legends) ----------
EFFECTS = {
    # buffs
    "ATK up": "+25% / +50% attack",
    "DEF up": "+30% / +60% defence",
    "Haste up": "+30 haste (skills recharge faster)",
    "Attack speed up": "+20% / +40% attack speed",
    "Crit up": "+15% / +30% crit chance",
    "Shield": "Absorbs damage; amount from caster ATK or max HP; shields stack (AFK Arena), capped at 50% of max HP",
    "Unkillable": "HP can't drop below 1 (max 4s)",
    "Immunity": "Ignores new debuffs",
    "Control immunity": "Ignores stun, sleep, root, fear, charm, silence, knock-up",
    "Untargetable": "Enemies can't select the hero (stealth breaks on attacking unless stated)",
    "Regen": "Heals % max HP per second",
    "Lifesteal up": "+15% / +30% lifesteal",
    "Counter": "Hits back for 75% when struck in melee",
    "Reflect": "Returns a % of damage taken to the attacker",
    "Revive": "Returns at 30-40% HP; once per hero per battle",
    "Skill shield": "Blocks the next skill or ultimate hit completely",
    # debuffs
    "DEF down": "-30% / -60% defence",
    "ATK down": "-25% / -50% attack",
    "Weaken": "Takes +15% / +25% damage from everything",
    "Mark": "Takes +15% damage; marked enemies are preferred targets",
    "Slow": "-30% move and attack speed",
    "Blind": "Basic attacks miss 50%",
    "Heal reduction": "-50% / -100% healing received",
    "Block buffs": "Can't receive buffs",
    "Silence": "Can't use Skill 1 / Skill 2 (ultimate still works)",
    "Burn": "Fire damage over time from caster ATK; stacks up to 3",
    "Bleed": "Damage over time from caster ATK; doubled while the target moves",
    "Poison": "2% / 4% of max HP per second",
    "Curse": "Takes extra damage equal to a % of damage dealt to their allies",
    "Soaked": "Ocean status: -15% fire damage taken, +25% lightning damage taken; feeds Ocean skills",
    "Energy drain": "Removes energy",
    # hard control (all capped at 2.5s; see diminishing returns)
    "Stun": "Can't act",
    "Sleep": "Can't act; breaks when damaged; sleeping targets take +30% crit chance against them",
    "Root": "Can't move; can still attack in range",
    "Fear": "Runs away from the caster",
    "Charm": "Attacks own allies",
    "Taunt": "Must attack the taunter",
    "Knock-up": "Airborne for 0.75s; counts as Airborne for payoffs",
    "Knockback / pull": "Moves the target; pulled targets count as Grouped or Isolated for payoffs",
}

CONTROL_RULES = {
    "maxDuration": 2.5,
    "diminishing": "After a hero is hit by hard control, further hard control on them lasts 50% as long for 4s, then 0% (immune) for 2s. Stops endless lock teams.",
    "priority": "Charm > Fear > Stun > Sleep > Root (AFK Arena style: only the strongest applies).",
    "cleansable": "Every debuff and control effect can be cleansed unless marked otherwise.",
}

# ---------- Synergy keywords: states that one hero applies and another pays off ----------
KEYWORDS = {
    "Burn": "enemy is burning", "Bleed": "enemy is bleeding", "Poison": "enemy is poisoned", "Curse": "enemy is cursed",
    "Mark": "enemy is marked", "Soaked": "enemy is soaked", "Slow": "enemy is slowed", "Blind": "enemy is blinded",
    "Rooted": "enemy is rooted", "Asleep": "enemy is asleep", "Stunned": "enemy is stunned", "Feared": "enemy is feared",
    "Charmed": "enemy is charmed", "Airborne": "enemy is knocked up", "Grouped": "enemies are pulled together",
    "Isolated": "an enemy is pulled away from its team", "Weakened": "enemy takes more damage or has lower DEF",
    "Taunted": "enemies are forced onto one ally", "Shielded": "allies have shields", "Stealth": "allies are hidden, or left stealth within the last 3s",
    "Energy": "allies gain energy", "Cooldown": "allies' skills recharge faster", "Lifesteal": "allies have lifesteal",
    "Buffed": "allies carry buffs", "Summon": "allied summons are on the field", "Kill": "an enemy falls",
    "LowHP": "an enemy is below a health threshold", "Dispel": "enemy buffs are removed",
    # Pass 10 packages (merged keywords)
    "Hindered": "enemy is slowed, rooted, stunned or knocked up (control counts for 3s after it ends)",
    "Dread": "enemy is asleep, feared or charmed (counts for 3s after it ends)",
    "Exposed": "enemy is marked, weakened, DEF-lowered or cursed",
    "Wounds": "enemy is bleeding or poisoned",
    "Disrupted": "enemy is blinded, silenced or drained (had energy taken in the last 4s)",
    "Gathered": "enemies are pulled together or taunted",
    "Tempo": "allies gain energy or their skills recharge faster",
    "Guarded": "allies carry shields or buffs",
}

# ---------- Role stat templates (level 1) ----------
TEMPLATES = {
    ("tank", "melee"):    dict(hp=2400, atk=120, def_=180, atkSpd=0.80, range=1, critRate=0.05, block=0.10, effectRes=0.25),
    ("dps", "melee"):     dict(hp=1550, atk=255, def_=95,  atkSpd=1.00, range=1, critRate=0.10),
    ("dps", "ranged"):    dict(hp=1250, atk=275, def_=70,  atkSpd=0.90, range=5, critRate=0.10),
    ("healer", "ranged"): dict(hp=1400, atk=170, def_=90,  atkSpd=0.80, range=4, healPower=0.10),
    ("support", "ranged"):dict(hp=1600, atk=175, def_=110, atkSpd=0.85, range=4, effectHit=0.10),
    ("support", "melee"): dict(hp=1800, atk=175, def_=130, atkSpd=0.85, range=1, effectHit=0.10),
}

# Power cost per unit of each stat. Used only to check every hero in a role has the same stat budget.
COST = dict(hp=1.0, atk=6.0, def_=5.0, atkSpd=600, critRate=1500, critDmg=600, block=2000, dodge=2000,
            effectHit=800, effectRes=600, haste=12, energyRegen=900, lifesteal=2500, healPower=1500,
            healRecv=800, moveSpd=120, accuracy=600, range=40)

# Style modifiers: trades that keep the budget level (checked by check.py).
STYLE_MODS = {
    "Crusader":      dict(atk=+40, lifesteal=+0.08, def_=-40, block=-0.05),
    "Tidewall":      dict(hp=+150, block=+0.05, atk=-35, critRate=-0.05),
    "Sniper":        dict(critRate=+0.05, range=+1, hp=-115),
    "Pyromancer":    dict(atk=+20, atkSpd=-0.10, hp=-60),
    "Inkweaver":     dict(effectHit=+0.15, hp=-120),
    "Rainmaker":     dict(hp=+150, healPower=-0.10),
    "Guardian":      dict(hp=+150, block=+0.05, atk=-35, critRate=-0.05),
    "Bruiser":       dict(atk=+40, lifesteal=+0.08, def_=-40, block=-0.05),
    "Gatherer":      dict(hp=+100, moveSpd=+0.8, def_=-20, block=-0.05, atk=-15),
    "Disruptor":     dict(effectHit=+0.15, hp=-120),
    "Warden":        dict(effectRes=+0.15, def_=+10, hp=-140),
    "Undying":       dict(hp=+200, healRecv=+0.15, atk=-40, block=-0.05),
    "Thorns":        dict(def_=+40, atk=-33),
    "Shapeshifter":  dict(atk=+30, hp=-180),
    "Martyr":        dict(healRecv=+0.30, hp=-240),
    "Rune golem":    dict(effectRes=+0.20, hp=-120),
    "Heartwood":     dict(hp=+180, healRecv=+0.10, moveSpd=-0.6, atk=-30, block=-0.02),
    "Marksman":      dict(critRate=+0.05, range=+1, hp=-115),
    "Assassin":      dict(critDmg=+0.30, moveSpd=+1.0, hp=-300),
    "Nuker":         dict(atk=+20, atkSpd=-0.10, hp=-60),
    "Chain caster":  dict(haste=+10, atk=-20),
    "Summoner":      dict(hp=+100, atk=-17),
    "Warlock":       dict(effectHit=+0.15, atk=-20),
    "Diver":         dict(moveSpd=+1.0, dodge=+0.05, hp=-220),
    "Tank-breaker":  dict(accuracy=+0.20, atk=-20),
    "Duelist":       dict(dodge=+0.08, critRate=+0.03, hp=-205),
    "Lancer":        dict(moveSpd=+0.8, atk=+5, hp=-126),
    "Bleed duelist": dict(atkSpd=+0.15, atk=-15),
    "Reaper":        dict(critDmg=+0.20, hp=-120),
    "Beastmaster":   dict(hp=+100, atk=-17),
    "Poisoner":      dict(effectHit=+0.15, atk=-20),
    "Stormcaller":   dict(haste=+10, atk=-20),
    "Lurker":        dict(lifesteal=+0.05, hp=-125),
    "Mana thief":    dict(energyRegen=+0.10, atk=-15),
    "Dragonrider":   dict(atk=+20, atkSpd=-0.10, hp=-60),
    "Burst":         dict(healPower=+0.10, hp=-150),
    "Drain":         dict(lifesteal=+0.06, healPower=-0.10),
    "Regen":         dict(hp=+150, healPower=-0.10),
    "Shield":        dict(def_=+30, healPower=-0.10),
    "Rewind":        dict(haste=+12, healPower=-0.10),
    "Cleanser":      dict(effectRes=+0.25, healPower=-0.10),
    "Veil":          dict(dodge=+0.075, healPower=-0.10),
    "Reviver":       dict(hp=+150, healPower=-0.10),
    "Warder":        dict(effectRes=+0.25, healPower=-0.10),
    "Lifebinder":    dict(lifesteal=+0.06, healPower=-0.10),
    "Songweaver":    dict(haste=+12, healPower=-0.10),
    "Buffer":        dict(haste=+10, hp=-120),
    "Debuffer":      dict(effectHit=+0.15, hp=-120),
    "Controller":    dict(effectHit=+0.15, hp=-120),
    "Charmer":       dict(effectHit=+0.15, hp=-120),
    "Energy battery":dict(energyRegen=+0.13, hp=-120),
    "Dispeller":     dict(effectHit=+0.15, hp=-120),
    "Anti-heal":     dict(effectHit=+0.15, hp=-120),
    "Speed buffer":  dict(moveSpd=+1.0, hp=-120),
    "Crit buffer":   dict(critRate=+0.08, hp=-120),
    "Commander":     dict(def_=+24, hp=-120),
    "Shadowbinder":  dict(dodge=+0.06, hp=-120),
    "Hexer":         dict(effectHit=+0.15, hp=-120),
    "Tidecaller":    dict(effectHit=+0.15, hp=-120),
    "Chronomancer":  dict(haste=+10, hp=-120),
}
