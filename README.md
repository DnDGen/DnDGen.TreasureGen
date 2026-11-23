# TreasureGen

Generate random Dungeons & Dragons treasure, equipment, and items

[![Build Status](https://github.com/DnDGen/DnDGen.TreasureGen/actions/workflows/release.yml/badge.svg?branch=main)](https://github.com/DnDGen/DnDGen.TreasureGen/actions/workflows/release.yml?query=branch%3Amain) [![Theme Song](https://img.shields.io/badge/theme_song-Suno-ff69b4)](https://suno.com/s/1srFPYgfd6VuVpzh)

TreasureGen generates treasure for the Dungeons & Dragons 3.5 system: coins, goods (gems and art), and items (alchemical items, armor, weapons, tools, potions, rings, rods, scrolls, staffs, wands, and wondrous items), including magical bonuses, special abilities, curses, and intelligent items.

### Use

To use TreasureGen, simply generate from the generator you wish to use. Levels start at 1 (`LevelLimits.Minimum`), run to 20 for standard treasure (`LevelLimits.Maximum_Standard`), and to 30 for epic treasure (`LevelLimits.Maximum_Epic`). Item powers are Mundane, Minor, Medium, and Major (`PowerConstants`).

```C#
var treasure = treasureGenerator.GenerateAtLevel(6);
var coin = coinGenerator.GenerateAtLevel(6);
var goods = goodsGenerator.GenerateAtLevel(6);
var items = itemsGenerator.GenerateRandomAtLevel(6);
```

An item of a given type comes from the generator named for that type, so ask for it by its `ItemTypeConstants` name. Mundane items come from the Alchemical Item, Armor, Tool, and Weapon generators; magical items come from the Armor, Potion, Ring, Rod, Scroll, Staff, Wand, Weapon, and Wondrous Item generators.

```C#
var mundaneArmor = mundaneArmorGenerator.GenerateRandom();
var magicalWeapon = magicalWeaponGenerator.GenerateRandom(PowerConstants.Major);
```

To get a specific item, name it, with any traits (such as a special material), using the constants in the Constants table below.

```C#
var sword = mundaneWeaponGenerator.Generate(WeaponConstants.Longsword, TraitConstants.Masterwork);
```

Every generator that can take a while has an `Async` version, and `GenerateAtLevelAsync` generates the coins, goods, and items of a treasure at the same time.

### Getting the Generators

You can obtain generators via the Ninject module. Because the generators are very complex and are decorated in various ways, there is not a (recommended) way to build these generators manually. **Note:** This will also load the dependencies for TreasureGen, including RollGen and Infrastructure.

```C#
var kernel = new StandardKernel();
TreasureGenModuleLoader.LoadModules(kernel);

var treasureGenerator = kernel.Get<ITreasureGenerator>();
var mundaneArmorGenerator = kernel.Get<MundaneItemGenerator>(ItemTypeConstants.Armor);
var magicalWeaponGenerator = kernel.Get<MagicalItemGenerator>(ItemTypeConstants.Weapon);
```

Your particular syntax for how the Ninject injection should work will depend on your project (class library, web site, etc.)

### Installing TreasureGen

The project is on [Nuget](https://www.nuget.org/packages/DnDGen.TreasureGen). Install via the NuGet Package Manager.

    PM > Install-Package DnDGen.TreasureGen

Version 13 and later need .NET 10.

## Reference

Members are written as signatures, with types, parameter names, and defaults. `Async` methods return a `Task` of the same result as the method they are named for.

### Generators

#### ITreasureGenerator

Generates a whole treasure: coins, goods, and items. A level below `LevelLimits.Minimum` is an invalid level.

| Method | Returns |
|---|---|
| `GenerateAtLevel(int level)` | The `Treasure` for that level |
| `GenerateAtLevelAsync(int level)` | The same, generating its parts at the same time |

#### ICoinGenerator

| Method | Returns |
|---|---|
| `GenerateAtLevel(int level)` | The `Coin` for that level |

#### IGoodsGenerator

| Method | Returns |
|---|---|
| `GenerateAtLevel(int level)` | The `Good`s, gems and art, for that level |
| `GenerateAtLevelAsync(int level)` | The same, asynchronously |

#### IItemsGenerator

| Method | Returns |
|---|---|
| `GenerateRandomAtLevel(int level)` | The random `Item`s for that level |
| `GenerateRandomAtLevelAsync(int level)` | The same, asynchronously |
| `GenerateAtLevel(int level, string itemType, string itemName, params string[] traits)` | The named `Item` of an `ItemTypeConstants` type, with the traits |
| `GenerateAtLevelAsync(int level, string itemType, string itemName, params string[] traits)` | The same, asynchronously |

#### MundaneItemGenerator

Get one for each type of mundane item: Alchemical Item, Armor, Tool, and Weapon.

| Method | Returns |
|---|---|
| `GenerateRandom()` | A random mundane `Item` of the type |
| `Generate(string itemName, params string[] traits)` | The named `Item`, with the traits |
| `Generate(Item template, bool allowRandomDecoration = false)` | An `Item` built from the template |

#### MagicalItemGenerator

Get one for each type of magical item: Armor, Potion, Ring, Rod, Scroll, Staff, Wand, Weapon, and Wondrous Item.

| Method | Returns |
|---|---|
| `GenerateRandom(string power)` | A random `Item` of the `PowerConstants` power |
| `Generate(string power, string itemName, params string[] traits)` | The named `Item` of that power, with the traits |
| `Generate(Item template, bool allowRandomDecoration = false)` | An `Item` built from the template |

#### TreasureGenModuleLoader

| Constructor |
|---|
| `TreasureGenModuleLoader()` |

| Method | Description |
|---|---|
| `LoadModules(IKernel kernel)` | Loads the TreasureGen modules, and those of its dependencies, into the kernel |

#### WhenExtensions

Ninject helper that TreasureGen uses to decorate its generators.

| Method | Description |
|---|---|
| `WhenInjectedInto<T>(IBindingWhenSyntax<T> syntax, Type parentType, string name)` | Binds only when injected into the parent type for the named binding |

### Treasure

#### Treasure

| Constructor |
|---|
| `Treasure()` |

| Property | Description |
|---|---|
| `Coin` | The coins |
| `Goods` | The gems and art |
| `Items` | The items |
| `IsAny` | Whether there are any coins, goods, or items |

#### Coin

| Constructor |
|---|
| `Coin()` |

| Property | Description |
|---|---|
| `Currency` | The currency, from `CoinConstants` |
| `Quantity` | How many coins |

#### Good

| Constructor |
|---|
| `Good()` |

| Property | Description |
|---|---|
| `Description` | What the gem or art is |
| `ValueInGold` | Its value, in gold pieces |

#### LevelLimits

| Property | Description |
|---|---|
| `Minimum` | The lowest level, 1 |
| `Maximum_Standard` | The highest standard level, 20 |
| `Maximum_Epic` | The highest epic level, 30 |

### Items

#### Item

| Constructor |
|---|
| `Item()` |

| Method | Description |
|---|---|
| `CloneInto(Item target)` | Copies everything, magic included, into the target |
| `Clone()` | A copy of everything, magic included |
| `SmartClone()` | A copy that keeps only the magic that suits the item |
| `SmartCloneInto(Item target)` | Copies the same into the target |
| `MundaneClone()` | A copy without any magic |
| `MundaneCloneInto(Item target)` | Copies the same into the target |
| `NameMatches(string name)` | Whether the name, or one of the base names, is the name |

| Property | Description |
|---|---|
| `Name` | The item's name |
| `BaseNames` | The names it is a kind of, such as the weapon a specific weapon is |
| `ItemType` | Its `ItemTypeConstants` type |
| `Traits` | Its traits, such as special materials |
| `Attributes` | Its `AttributeConstants` attributes |
| `Magic` | Its magic |
| `Quantity` | How many there are |
| `Contents` | What it holds, such as the spells on a scroll |
| `Summary` | Its name with its materials, bonus, and special abilities |
| `IsMagical` | Whether it is magical |
| `CanBeUsedAsWeaponOrArmor` | Whether it can be used as a weapon or armor |

#### Armor

An `Item` that is armor or a shield.

| Constructor |
|---|
| `Armor()` |

| Method | Description |
|---|---|
| `Clone()` | As `Item`, keeping the armor details |
| `MundaneClone()` | As `Item`, keeping the armor details |
| `MundaneCloneInto(Item target)` | As `Item`, keeping the armor details |
| `SmartClone()` | As `Item`, keeping the armor details |

| Property | Description |
|---|---|
| `ArmorBonus` | The armor bonus |
| `Size` | The size, from `TraitConstants.Sizes` |
| `ArmorCheckPenalty` | The armor check penalty |
| `MaxDexterityBonus` | The maximum Dexterity bonus |
| `TotalArmorBonus` | The armor bonus with the magical bonus |
| `TotalArmorCheckPenalty` | The armor check penalty with its materials |
| `TotalMaxDexterityBonus` | The maximum Dexterity bonus with its materials |

#### Weapon

An `Item` that is a weapon.

| Constructor |
|---|
| `Weapon()` |

| Method | Description |
|---|---|
| `Clone()` | As `Item`, keeping the weapon details |
| `MundaneClone()` | As `Item`, keeping the weapon details |
| `MundaneCloneInto(Item target)` | As `Item`, keeping the weapon details |
| `CloneInto(Item target)` | As `Item`, keeping the weapon details |
| `SmartClone()` | As `Item`, keeping the weapon details |

| Property | Description |
|---|---|
| `Ammunition` | The ammunition it fires |
| `Damages` | Its damage |
| `DamageRoll` | The damage roll, with the magical bonus |
| `DamageSummary` | The damage, written out |
| `SecondaryDamages` | The damage of a double weapon's other end |
| `SecondaryDamageRoll` | The other end's damage roll |
| `SecondaryDamageSummary` | The other end's damage, written out |
| `CriticalDamages` | Its damage on a critical hit |
| `CriticalDamageRoll` | The critical roll |
| `CriticalDamageSummary` | The critical damage, written out |
| `CriticalMultiplier` | The critical multiplier |
| `SecondaryCriticalDamages` | The other end's critical damage |
| `SecondaryCriticalDamageRoll` | The other end's critical roll |
| `SecondaryCriticalDamageSummary` | The other end's critical damage, written out |
| `SecondaryCriticalMultiplier` | The other end's critical multiplier |
| `Size` | The size, from `TraitConstants.Sizes` |
| `ThreatRangeSummary` | The threat range, such as 19-20 |
| `ThreatRange` | How many extra numbers threaten |
| `IsDoubleWeapon` | Whether it is a double weapon |
| `SecondaryMagicBonus` | The other end's magical bonus |
| `SecondaryHasAbilities` | Whether the other end has the special abilities |
| `CanBeUsedAsWeaponOrArmor` | Always true |
| `CombatTypes` | Whether it is melee, ranged, or both |

#### Damage

| Constructor |
|---|
| `Damage()` |

| Method | Description |
|---|---|
| `Clone()` | A copy |

| Property | Description |
|---|---|
| `Roll` | The roll |
| `Type` | The damage type, from `AttributeConstants.DamageTypes` |
| `Summary` | The roll, type, and condition, written out |
| `Condition` | When the damage applies, if only sometimes |
| `IsConditional` | Whether there is a condition |

### Magic

#### Magic

| Constructor |
|---|
| `Magic()` |

| Property | Description |
|---|---|
| `Bonus` | The magical bonus |
| `Charges` | The charges left |
| `SpecialAbilities` | The special abilities |
| `Curse` | The curse, from `CurseConstants`, if cursed |
| `Intelligence` | The item's mind, if it has one |

#### SpecialAbility

| Constructor |
|---|
| `SpecialAbility()` |

| Method | Description |
|---|---|
| `RequirementsMet(Item targetItem)` | Whether the item can have the ability |

| Property | Description |
|---|---|
| `Name` | The name |
| `BaseName` | The name without alternate spellings |
| `Power` | The power of the ability |
| `AttributeRequirements` | The attributes an item needs |
| `BonusEquivalent` | How much of a bonus the ability counts for |
| `Damages` | The extra damage |
| `CriticalDamages` | The extra damage on a critical hit |

#### Intelligence

| Constructor |
|---|
| `Intelligence()` |

| Method | Description |
|---|---|
| `Clone()` | A copy |

| Property | Description |
|---|---|
| `IntelligenceStat` | Intelligence |
| `WisdomStat` | Wisdom |
| `CharismaStat` | Charisma |
| `Powers` | The powers |
| `SpecialPurpose` | The special purpose |
| `DedicatedPower` | The dedicated power |
| `Ego` | The ego |
| `Communication` | How it communicates |
| `Languages` | The languages it speaks |
| `Senses` | What it senses |
| `Alignment` | Its alignment |
| `Personality` | Its personality |

### Constants

The `*Constants` classes name things as strings, spelled as they read in the books, such as `"Wondrous Item"` or `"Amulet of Health"`. This table lists the classes, not each constant. A class's methods and properties are listed below it.

| Constants | Names |
|---|---|
| `CoinConstants` | The currencies |
| `GoodsConstants` | The kinds of goods |
| `ItemTypeConstants` | The types of item |
| `PowerConstants` | The powers of item |
| `AttributeConstants` | The attributes of items |
| `AttributeConstants.DamageTypes` | The damage types |
| `TraitConstants` | The traits an item can have |
| `TraitConstants.Sizes` | The sizes |
| `TraitConstants.SpecialMaterials` | The special materials |
| `ArmorConstants` | Armor and shields, with ways to list them |
| `WeaponConstants` | Weapons, with ways to list them |
| `AlchemicalItemConstants` | Alchemical items |
| `ToolConstants` | Tools |
| `PotionConstants` | Potions |
| `RingConstants` | Rings |
| `RodConstants` | Rods |
| `StaffConstants` | Staffs |
| `WondrousItemConstants` | Wondrous items |
| `SpecialAbilityConstants` | Special abilities of armor and weapons |
| `AlignmentConstants` | Alignments |
| `CurseConstants` | Curses |
| `CurseConstants.Dependent` | What a curse can depend on |
| `CurseConstants.Drawbacks` | The drawbacks a curse can have |

#### ArmorConstants

| Method | Returns |
|---|---|
| `GetAllArmorsAndShields(bool includeSpecific)` | Every armor and shield |
| `GetAllShields(bool includeSpecific)` | Every shield |
| `GetAllArmors(bool includeSpecific)` | Every armor |
| `GetAllLightArmors(bool includeSpecific)` | Every light armor |
| `GetAllMediumArmors(bool includeSpecific)` | Every medium armor |
| `GetAllHeavyArmors(bool includeSpecific)` | Every heavy armor |
| `GetAllSpecificArmorsAndShields()` | Every specific armor and shield |
| `GetAllSpecificArmors()` | Every specific armor |
| `GetAllSpecificShields()` | Every specific shield |

#### WeaponConstants

| Method | Returns |
|---|---|
| `GetAllWeapons(bool includeSpecific, bool includeTemplates)` | Every weapon |
| `GetAllMelee(bool includeSpecific, bool includeTemplates)` | Every melee weapon |
| `GetAllRanged(bool includeSpecific, bool includeTemplates, bool includeMelee)` | Every ranged weapon, and the melee weapons that can be thrown or fired if asked |
| `GetAllLightMelee(bool includeSpecific, bool includeTemplates)` | Every light melee weapon |
| `GetAllOneHandedMelee(bool includeSpecific, bool includeTemplates)` | Every one-handed melee weapon |
| `GetAllTwoHandedMelee(bool includeSpecific, bool includeTemplates)` | Every two-handed melee weapon |
| `GetAllSimple(bool includeSpecific, bool includeTemplates)` | Every simple weapon |
| `GetAllMartial(bool includeSpecific, bool includeTemplates)` | Every martial weapon |
| `GetAllExotic(bool includeSpecific, bool includeTemplates)` | Every exotic weapon |
| `GetAllDouble(bool includeSpecific, bool includeTemplates)` | Every double weapon |
| `GetAllReach(bool includeSpecific, bool includeTemplates)` | Every reach weapon |
| `GetAllProjectile(bool includeSpecific, bool includeTemplates)` | Every projectile weapon |
| `GetAllAmmunition(bool includeSpecific, bool includeTemplates)` | Every kind of ammunition |
| `GetAllThrown(bool includeSpecific, bool includeTemplates)` | Every thrown weapon |
| `GetAllBludgeoning(bool includeSpecific, bool includeTemplates)` | Every bludgeoning weapon |
| `GetAllPiercing(bool includeSpecific, bool includeTemplates)` | Every piercing weapon |
| `GetAllSlashing(bool includeSpecific, bool includeTemplates)` | Every slashing weapon |
| `GetAllSpecific()` | Every specific weapon |
| `GetAllTemplates()` | Every weapon template |

#### TraitConstants.Sizes

| Method | Returns |
|---|---|
| `GetAll()` | Every size |

#### TraitConstants.SpecialMaterials

| Method | Returns |
|---|---|
| `GetAll()` | Every special material |

#### AlignmentConstants

| Method | Returns |
|---|---|
| `GetAllAlignments()` | Every alignment |
| `GetAllPartialAlignments()` | Every alignment, and the parts of them |

| Property | Description |
|---|---|
| `LawfulGood` | Lawful Good |
| `NeutralGood` | Neutral Good |
| `ChaoticGood` | Chaotic Good |
| `LawfulNeutral` | Lawful Neutral |
| `TrueNeutral` | True Neutral |
| `ChaoticNeutral` | Chaotic Neutral |
| `LawfulEvil` | Lawful Evil |
| `NeutralEvil` | Neutral Evil |
| `ChaoticEvil` | Chaotic Evil |

#### AlchemicalItemConstants

| Method | Returns |
|---|---|
| `GetAllAlchemicalItems()` | Every alchemical item |

#### ToolConstants

| Method | Returns |
|---|---|
| `GetAllTools()` | Every tool |

#### PotionConstants

| Method | Returns |
|---|---|
| `GetAllPotions(bool includeTemplates)` | Every potion |

#### RingConstants

| Method | Returns |
|---|---|
| `GetAllRings()` | Every ring |

#### RodConstants

| Method | Returns |
|---|---|
| `GetAllRods()` | Every rod |

#### StaffConstants

| Method | Returns |
|---|---|
| `GetAllStaffs()` | Every staff |

#### WondrousItemConstants

| Method | Returns |
|---|---|
| `GetAllWondrousItems()` | Every wondrous item |

#### SpecialAbilityConstants

| Method | Returns |
|---|---|
| `GetAllAbilities(bool withAlternateNames)` | Every special ability |

## Theme Song

DnDGen.TreasureGen has its own theme song: [Haul Away -DnDGen.TreasureGen Theme-](https://suno.com/s/1srFPYgfd6VuVpzh), a pirate sea shanty for the treasure hunt.
It follows this README, so a change here may need the song updated too.
See the song's [lyrics, notes, and update procedure](https://github.com/DnDGen/DnDGen.TreasureGen/blob/main/docs/theme-song/README.md).
