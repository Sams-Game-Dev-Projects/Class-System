Designer Setup Instructions

Overview
- This system lets designers create Classes, Weapons, Armor, Damage Types, Weapon Types, and Spells via right‑click Create menus. No code edits are required.

One‑Time Import
- Drag the `Assets/Scripts` folder into your Unity project (or import this project’s folder).

Create Your Data (right‑click in Project window)
- Damage Types: `Create > RPG > Damage Type` (e.g., Fire, Ice, Lightning).
- Weapon Types: `Create > RPG > Weapon Type` (e.g., Sword, Bow, Dagger, Staff).
- Leveling Profile: `Create > RPG > Leveling Profile` (set XP required per level).
- Stat Block: `Create > RPG > Stat Block`
  - Add entries for Health, Mana, Strength, Intelligence, Dexterity, Defense, ManaRegen.
  - Set Base Value (level 1) and Per Level increment for each.
- Spells: `Create > RPG > Spell`
  - Basics: Name, Icon, Element (a Damage Type), Mana Cost, Cooldown.
  - Effect: Choose Damage or Heal. Set Base Amount and optional dice (e.g., 2d6).
  - Continuous: Toggle if effect repeats; set Duration and Tick Interval.
  - Targeting:
    - Self: applies to the caster.
    - Projectile: optionally assign a Projectile Prefab (2D trigger collider recommended) and set Speed/Max Distance.
      - Hit Layers: choose which layers this spell can collide with (e.g., Enemy, Obstacles). The projectile automatically ignores the caster.
  - Visuals: optional Cast/Impact VFX.
- Weapons: `Create > RPG > Weapon`
  - Set Weapon Type.
  - Add one or more Damage entries. For each, choose a Damage Type, Base Amount, and optional Dice (e.g., count=2, sides=6 for 2d6). Repeat for multiple types (e.g., Fire + Lightning).
- Armor: `Create > RPG > Armor`
  - Set Armor Class (AC). Higher AC = harder to hit.
  - Add Resistances (per Damage Type). A value of 0.2 = 20% reduced damage; -0.3 = 30% extra damage (weakness).
- Player Class: `Create > RPG > Player Class`
  - Assign Leveling Profile and Stat Block.
  - Allowed Weapon Types: add all types this class can use; other weapon types will be blocked.
  - Known Spells: add the spells this class can cast.

Set Up a Scene
- Create an empty GameObject named `Player`.
- Add components: `CharacterStats`, `EquipmentManager`, `SpellCaster`, `Attacker`.
- In `CharacterStats`:
  - Assign your Player Class asset.
  - Set Level to 1 (or desired starting level).
- In `EquipmentManager`:
  - Drag your starting Weapon and Armor.
  - If a weapon isn’t allowed by the class, the system will block it and log a clear message. No extra setup needed.
- Optional: Add `KeyboardDemo` for testing controls:
  - Left click = weapon attack on object under mouse.
  - Right click = cast the selected spell toward the mouse.
  - Q / E = cycle through the class’s Known Spells.
- For projectile spells:
  - Ensure target objects have 2D colliders.
  - Set the Spell’s Hit Layers to include target layers (e.g., Enemy) and exclude the Player’s layer.

Layer Setup (recommended)
- Create named layers for clearer filtering:
  - Player, Enemy, Obstacles, Projectiles (optional)
- How to add layers:
  - Select any GameObject → top-right of Inspector → Layer → Add Layer… → fill empty slots with names above.
  - Then assign each GameObject’s Layer via the same Layer dropdown.
- Suggested assignments:
  - Player GameObject: `Player`
  - Enemy prefabs/instances: `Enemy`
  - Walls/ground/collision tiles: `Obstacles`
  - Projectile prefab(s): `Projectiles` (optional; any visible layer is fine)
- Configure each Spell’s Hit Layers:
  - Open the Spell asset (e.g., `ClassObjects/Spells/Fireball.asset`).
  - Set `Hit Layers` to include `Enemy` and `Obstacles`, and exclude `Player`.
  - Self-target spells (Heal Self, Regenerate) ignore Hit Layers.
- Optional Physics 2D Layer Collision Matrix:
  - Edit → Project Settings → Physics 2D → Layer Collision Matrix.
  - Uncheck pairs you never want to physically collide (e.g., `Projectiles` vs `Player` if you don’t use friendly fire).
  - Note: Projectiles automatically ignore the caster at runtime even if the matrix allows it.
- Camera visibility:
  - Ensure the Main Camera’s Culling Mask includes the layers you use (Player, Enemy, Projectiles, Obstacles).

Add Enemies (works with procedural spawns)
- Enemy prefab needs: `CharacterStats` (with a Player Class), `EquipmentManager` (optional), a 2D Collider, and optionally `Attacker`.
- To award XP on enemy death (multiplayer‑friendly): add `AwardXpToContributorsOnDeath` to the enemy prefab.
  - Set `Total Xp` (e.g., 100).
  - Choose `Proportional To Damage` (split by contribution) or turn off to split evenly.
  - `Min Contribution` can filter out trivial hits.
- To clean up on death: add `DespawnOnDeath` to the enemy prefab (destroys the GameObject when HP reaches zero).

How To Test Quickly
- Create `Player` and an enemy (e.g., `Dummy`) as above.
- Press Play and use mouse buttons (Q/E to switch spells). Kill the enemy to see XP awarded and despawn.

Tips
- New elements: create a Damage Type and use it in weapons/spells.
- New weapon categories: create a Weapon Type asset and add it to a class’s Allowed Weapon Types.
- Leveling: tune the Leveling Profile to change XP needed per level. On level‑up, HP/MP refill automatically.

Editor Tools (one-click sample content)
- Tools > RPG > Generate Sample ClassObjects
  - Creates a ready-to-use set of sample assets in `Assets/ClassObjects` with the same internal organization as `RPG`:
    - Damage Types: Fire, Ice, Lightning, Poison
    - Weapon Types: Sword, Dagger, Wand, Staff
    - Weapons: FlamingLaserSword, Dagger, BasicWand
    - Armors: BasicArmor, LeatherArmor
    - Spells: Fireball (instant, projectile), Poison Spray (continuous, projectile), Heal Self (instant), Regenerate Health (continuous)
    - Stat Blocks: WizardStatBlock (Health 12 + 6/level), KnightStatBlock (Health 20 + 10/level)
    - Player Classes: Wizard (Wand/Staff; Fireball, Heal Self, Regenerate), Knight (Sword/Dagger; Regenerate)
    - Leveling Profile: PlayerLvlProfile (shared by all classes)
  - Run this once to bootstrap your project with examples you can inspect and duplicate.
- Tools > RPG > Art > Create Fireball Demo Prefab
  - Creates a simple Fireball sprite and a `FireballProjectile.prefab` in `Assets/Art/Demo/`.
  - If the sample content exists, it auto-assigns the prefab to `ClassObjects/Spells/Fireball.asset`.
  - Use this to quickly visualize spell projectiles in your scenes.
