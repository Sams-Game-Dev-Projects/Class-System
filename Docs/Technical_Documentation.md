Technical Documentation

Architecture Overview
- Data (ScriptableObjects):
  - `Combat/DamageType.cs`: Designer‑defined damage elements (Fire, Ice, etc.).
  - `Items/WeaponType.cs`: Designer‑defined weapon categories (Sword, Bow, etc.).
  - `Core/StatBlock.cs`: Per‑class stat curves via entries (base + per level).
  - `Core/LevelingProfile.cs`: Per‑class XP requirements per level (with curve extrapolation).
  - `Items/Weapon.cs`: Multi‑type dice damage (base + dice per DamageType) and cooldown.
  - `Items/Armor.cs`: Armor Class and per‑DamageType resistances.
  - `Classes/PlayerClass.cs`: Class composition: stats, XP profile, allowed weapon types, known spells.
  - `Spells/Spell.cs`: Spell config: effect kind, element, costs, targeting (with `hitLayers`), visuals, and continuous behavior.
- Runtime (MonoBehaviours):
  - `Runtime/CharacterStats.cs`: Leveling, current HP/MP, mana regen, and damage/healing application.
  - `Runtime/EquipmentManager.cs`: Equipped weapon/armor, AC assembly, resistance queries, class proficiency checks.
  - `Runtime/Attacker.cs`: Weapon attacks, to‑hit calculation, and damage application.
  - `Runtime/SpellCaster.cs`: Casting pipeline (cooldown, mana, self/projectile delivery) and projectile visibility fallback.
  - `Spells/SpellProjectile.cs`: Mover + 2D collision for projectile spells; ignores caster and respects `hitLayers`.
  - `Spells/TimedEffect.cs`: Over‑time (DoT/HoT) execution helper.
  - `Demo/KeyboardDemo.cs`: Minimal input harness for quick testing.
  - `Demo/AwardXpToContributorsOnDeath.cs`: Tracks damage contributors and awards XP on death.
  - `Demo/DespawnOnDeath.cs`: Destroys an entity when its `CharacterStats` fires `OnDied`.

Stats and Leveling
- Stat source: `PlayerClass.statBlock` (ScriptableObject) with `StatEntry(stat, base, perLevel)`.
- Effective value at level L: `base + perLevel * (L - 1)`.
- Health and Mana caps are read from `StatType.Health` and `StatType.Mana` respectively.
- Mana regeneration uses `StatType.ManaRegen` in units per second (handled every Update).
- XP to next level is provided by `PlayerClass.levelingProfile.GetXpToNextLevel(currentLevel)`.
  - If the list is too short, an `AnimationCurve` extrapolates requirements.
- Level up: `CharacterStats.AddExperience` loops to handle multi‑level gains, restoring HP/MP.
 - Events:
   - `OnDamaged(float amount)` — raised on final damage applied after resistances.
   - `OnDamagedBy(float amount, Object source)` — same as above, including the originating source object for contribution tracking.
   - `OnHealed(float amount)`, `OnDied()` — raised on heal and when HP reaches 0 respectively.

Weapons, Attacks, Hit Chance
- Weapon damage model (per entry): `baseAmount + sum(dice.count d dice.sides)` per `DamageType`.
- Attacks roll to hit using a d20 check in `CombatResolver.RollToHit(attackRating, armorClass)`:
  - `d20 + attackRating >= 10 + armorClass`; natural 20 always hits, natural 1 always misses.
  - Default `attackRating` is `Dexterity` at current level (can be adjusted to taste).
- On hit, `Attacker` rolls weapon damage to produce a `DamageBundle` and calls `IDamageable.ReceiveDamage`.

Armor, Armor Class, Resistances
- Total Armor Class = `Defense (stat)` + `Armor.armorClass`.
- Resistances are additive per `DamageType`: final factor = `1 - sum(reduction)` clamped to [0.1, 1.9].
  - Example: 0.2 = 20% less damage, -0.3 = 30% more damage.
- `CharacterStats.ReceiveDamage` applies resistances and subtracts HP; death event is raised at 0 HP.

Spell System
- Spell has an `element` (`DamageType`), cost, cooldown, effect kind (Damage/Heal), and delivery mode.
- Targeting options:
  - Self: applies effect to caster immediately or over time.
  - Projectile: spawns `SpellProjectile` which moves directionally and applies effect on collision.
- Visuals: optional `castVfxPrefab` and `impactVfxPrefab` are instantiated if assigned.
- Instant vs Continuous:
  - Instant: one application using `baseAmount + dice`.
  - Continuous: repeated application every `tickInterval` for `duration` using same roll formula each tick.
- Designers can add new spell elements by creating new `DamageType` assets and selecting them in spells.
 - Collision and filtering:
   - `Spell.hitLayers` gates which layers a projectile will process; non‑matching layers are ignored.
   - Projectiles ignore the caster and their child colliders via `Physics2D.IgnoreCollision` plus runtime checks.

Class Proficiencies and Restrictions
- Each `PlayerClass` lists allowed `WeaponType`s.
- `EquipmentManager.TryEquipWeapon` enforces this whitelist. Disallowed attempts return `false` and an error string.

Integration Notes
- Namespaces are under `ClassSystem.*` to reduce collisions when importing into other projects.
- Components are plug‑and‑play; attach to existing character prefabs and assign assets.
- Projectile spells assume 2D; ensure projectile prefabs carry a 2D trigger collider to detect `IDamageable` targets.
- For visibility without a prefab, `SpellCaster` adds a `SpriteRenderer` and tries to load a demo sprite.
- Set meaningful layers for Player/Enemy/Obstacles and configure each Spell’s `hitLayers` accordingly.

Editor Utilities
- Tools > RPG > Generate Sample ClassObjects
  - Programmatically creates a sample content tree under `Assets/ClassObjects` (DamageTypes, WeaponTypes, Weapons, Armors, Spells, StatBlocks, PlayerClasses, LevelingProfiles) with conservative starting stats:
    - Knight Health: 20 + 10/level; Wizard Health: 12 + 6/level
  - Both classes share a single `LevelingProfile` but can diverge by swapping the reference.
- Tools > RPG > Art > Create Fireball Demo Prefab
  - Creates a basic Fireball sprite and `FireballProjectile.prefab` and assigns it to the `Fireball` spell if present.

Runtime XP Distribution
- Attach `Demo/AwardXpToContributorsOnDeath` to an enemy to award XP when it dies.
  - Tracks contributors using `CharacterStats.OnDamagedBy(amount, source)`.
  - If `proportionalToDamage` is true: each contributor receives `totalXp * (damageDealt / totalDamage)` (rounded).
  - Otherwise: each eligible contributor receives `floor(totalXp / contributorCount)`.
  - `minContribution` removes trivial hits from eligibility.
- Multiplayer: run the XP award on the authoritative instance and replicate XP/level state to clients per your netcode.

Extensibility
- Add new stats by extending `StatType` and adding `StatEntry` items in the `StatBlock` assets.
- Override hit or damage behavior by replacing `CombatResolver` or `Attacker` logic.
- Add new delivery types by extending `Spell.targeting` handling in `SpellCaster` and providing a new delivery component.

Key Files
- Assets/Scripts/Core/StatType.cs
- Assets/Scripts/Core/StatBlock.cs
- Assets/Scripts/Core/LevelingProfile.cs
- Assets/Scripts/Combat/DamageType.cs
- Assets/Scripts/Combat/DamagePackets.cs
- Assets/Scripts/Combat/CombatResolver.cs
- Assets/Scripts/Items/WeaponType.cs
- Assets/Scripts/Items/Weapon.cs
- Assets/Scripts/Items/Armor.cs
- Assets/Scripts/Classes/PlayerClass.cs
- Assets/Scripts/Runtime/EquipmentManager.cs
- Assets/Scripts/Runtime/CharacterStats.cs
- Assets/Scripts/Runtime/Attacker.cs
- Assets/Scripts/Runtime/SpellCaster.cs
- Assets/Scripts/Spells/Spell.cs
- Assets/Scripts/Spells/SpellProjectile.cs
- Assets/Scripts/Spells/TimedEffect.cs
- Assets/Scripts/Demo/KeyboardDemo.cs
- Assets/Scripts/Demo/AwardXpToContributorsOnDeath.cs
- Assets/Scripts/Demo/DespawnOnDeath.cs
