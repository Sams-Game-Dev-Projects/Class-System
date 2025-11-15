Class System (Unity C# Module)

What’s included
- Designer‑friendly ScriptableObjects for Classes, Stats, Leveling, Weapons, Armor, Damage Types, Weapon Types, and Spells.
- Runtime systems for leveling, equipment restrictions, mana regen, attacks with hit rolls, resistances, and spells (self or projectile, instant or over time).

Start here
- See `Docs/Designer_Setup_Instructions.md` for step‑by‑step setup.
- For architecture details, see `Docs/Technical_Documentation.md`.

Importing into another game
- Copy `Assets/Scripts` (and optional `Docs`) into your project. All scripts are in `ClassSystem.*` namespaces.

Editor tools
- Tools > RPG > Generate Sample ClassObjects
  - Creates a complete set of sample assets under `Assets/ClassObjects` (DamageTypes, WeaponTypes, Weapons, Armors, Spells, StatBlocks, PlayerClasses, LevelingProfiles).
  - Conservative defaults: Knight Health 20 + 10/level, Wizard Health 12 + 6/level; shared Leveling Profile.
  - Run once to bootstrap content you can inspect and duplicate.
- Tools > RPG > Art > Create Fireball Demo Prefab
  - Generates a simple Fireball sprite and `FireballProjectile.prefab` in `Assets/Art/Demo/` and assigns it to the Fireball spell if present.

Minimal scene setup
- Add `CharacterStats`, `EquipmentManager`, `SpellCaster`, `Attacker`, and `KeyboardDemo` to a `Player` GameObject.
- Assign a `PlayerClass` from `Assets/ClassObjects/PlayerClasses` (e.g., WizardClass).
- Add a target with `CharacterStats` + 2D collider. Optionally add `AwardXpToContributorsOnDeath` and `DespawnOnDeath` to enemies.

Controls (with KeyboardDemo)
- Left mouse: weapon attack on object under the cursor
- Right mouse: cast the currently selected spell toward the cursor
- Q / E: cycle through known spells
