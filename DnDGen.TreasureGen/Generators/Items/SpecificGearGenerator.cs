﻿using DnDGen.Infrastructure.Generators;
using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.TreasureGen.Generators.Items.Magical;
using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Items.Mundane;
using DnDGen.TreasureGen.Selectors.Percentiles;
using DnDGen.TreasureGen.Tables;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.TreasureGen.Generators.Items
{
    internal class SpecificGearGenerator : ISpecificGearGenerator
    {
        private readonly ITypeAndAmountPercentileSelector typeAndAmountPercentileSelector;
        private readonly ICollectionSelector collectionsSelector;
        private readonly IChargesGenerator chargesGenerator;
        private readonly ITreasurePercentileSelector percentileSelector;
        private readonly ISpellGenerator spellGenerator;
        private readonly ISpecialAbilitiesGenerator specialAbilitiesGenerator;
        private readonly JustInTimeFactory justInTimeFactory;
        private readonly IReplacementSelector replacementSelector;

        public SpecificGearGenerator(ITypeAndAmountPercentileSelector typeAndAmountPercentileSelector,
            ICollectionSelector collectionsSelector,
            IChargesGenerator chargesGenerator,
            ITreasurePercentileSelector percentileSelector,
            ISpellGenerator spellGenerator,
            ISpecialAbilitiesGenerator specialAbilitiesGenerator,
            JustInTimeFactory justInTimeFactory,
            IReplacementSelector replacementSelector)
        {
            this.typeAndAmountPercentileSelector = typeAndAmountPercentileSelector;
            this.collectionsSelector = collectionsSelector;
            this.chargesGenerator = chargesGenerator;
            this.percentileSelector = percentileSelector;
            this.spellGenerator = spellGenerator;
            this.specialAbilitiesGenerator = specialAbilitiesGenerator;
            this.justInTimeFactory = justInTimeFactory;
            this.replacementSelector = replacementSelector;
        }

        private Item SetPrototypeAttributes(Item prototype, string specificGearType)
        {
            var gear = prototype.Clone();

            if (gear.Name == WeaponConstants.JavelinOfLightning)
            {
                gear.IsMagical = true;
            }
            else if (gear.Name == ArmorConstants.CastersShield)
            {
                var hasSpell = percentileSelector.SelectFrom<bool>(Config.Name, TableNameConstants.Percentiles.Set.CastersShieldContainsSpell);

                if (hasSpell)
                {
                    var spellType = percentileSelector.SelectFrom(Config.Name, TableNameConstants.Percentiles.Set.CastersShieldSpellTypes);
                    var spellLevel = spellGenerator.GenerateLevel(PowerConstants.Medium);
                    var spell = spellGenerator.Generate(spellType, spellLevel);
                    var formattedSpell = $"{spell} ({spellType}, {spellLevel})";
                    gear.Contents.Add(formattedSpell);
                }
            }

            var templateName = gear.Name;
            gear.Name = replacementSelector.SelectSingle(templateName);

            gear.Magic.SpecialAbilities = GetSpecialAbilities(specificGearType, templateName, prototype.Magic.SpecialAbilities);

            var tableName = string.Format(TableNameConstants.Collections.Formattable.SpecificITEMTYPEAttributes, specificGearType);
            gear.Attributes = collectionsSelector.SelectFrom(Config.Name, tableName, templateName);

            tableName = string.Format(TableNameConstants.Collections.Formattable.SpecificITEMTYPETraits, specificGearType);
            var traits = collectionsSelector.SelectFrom(Config.Name, tableName, templateName);

            foreach (var trait in traits)
                gear.Traits.Add(trait);

            if (gear.Attributes.Contains(AttributeConstants.Charged))
                gear.Magic.Charges = chargesGenerator.GenerateFor(specificGearType, templateName);

            if (gear.Name == WeaponConstants.SlayingArrow || gear.Name == WeaponConstants.GreaterSlayingArrow)
            {
                var designatedFoe = collectionsSelector.SelectRandomFrom(
                    Config.Name,
                    TableNameConstants.Collections.Set.ReplacementStrings,
                    ReplacementStringConstants.DesignatedFoe);
                var trait = $"Designated Foe: {designatedFoe}";
                gear.Traits.Add(trait);
            }

            if (gear.IsMagical)
                gear.Traits.Add(TraitConstants.Masterwork);

            if (gear.ItemType == ItemTypeConstants.Armor)
                return GetArmor(gear);

            if (gear.ItemType == ItemTypeConstants.Weapon)
                return GetWeapon(gear);

            if (gear.Quantity == 0)
                gear.Quantity = 1;

            return gear;
        }

        private Armor GetArmor(Item gear)
        {
            var name = gear.BaseNames.First();

            var mundaneArmorGenerator = justInTimeFactory.Build<MundaneItemGenerator>(ItemTypeConstants.Armor);
            var armor = mundaneArmorGenerator.Generate(name, gear.Traits.ToArray()) as Armor;

            gear.CloneInto(armor);

            if (armor.IsMagical)
                armor.Traits.Add(TraitConstants.Masterwork);

            armor.Traits.Remove(armor.Size);

            return armor;
        }

        private Weapon GetWeapon(Item gear)
        {
            var name = gear.BaseNames.First();

            var mundaneWeaponGenerator = justInTimeFactory.Build<MundaneItemGenerator>(ItemTypeConstants.Weapon);
            var weapon = mundaneWeaponGenerator.Generate(name, gear.Traits.ToArray()) as Weapon;

            gear.CloneInto(weapon);

            if (weapon.IsMagical)
                weapon.Traits.Add(TraitConstants.Masterwork);

            if (weapon.Attributes.Contains(AttributeConstants.Ammunition) || weapon.Attributes.Contains(AttributeConstants.OneTimeUse))
                weapon.Magic.Intelligence = new Intelligence();

            weapon.Traits.Remove(weapon.Size);

            if (weapon.IsDoubleWeapon)
            {
                weapon.SecondaryHasAbilities = true;
                weapon.SecondaryMagicBonus = weapon.Magic.Bonus;
            }

            foreach (var specialAbility in weapon.Magic.SpecialAbilities)
            {
                if (specialAbility.Damages.Any())
                {
                    var damages = specialAbility.Damages.Select(d => d.Clone()).ToArray();
                    var damageType = weapon.Damages[0].Type;

                    foreach (var damage in damages)
                    {
                        if (string.IsNullOrEmpty(damage.Type))
                        {
                            damage.Type = damageType;
                        }
                    }

                    weapon.Damages.AddRange(damages);

                    if (weapon.SecondaryHasAbilities)
                    {
                        var secondaryDamages = specialAbility.Damages.Select(d => d.Clone()).ToArray();
                        var secondaryDamageType = weapon.SecondaryDamages[0].Type;

                        foreach (var damage in secondaryDamages)
                        {
                            if (string.IsNullOrEmpty(damage.Type))
                            {
                                damage.Type = secondaryDamageType;
                            }
                        }

                        weapon.SecondaryDamages.AddRange(secondaryDamages);
                    }
                }

                if (specialAbility.CriticalDamages.Any())
                {
                    var damageType = weapon.CriticalDamages[0].Type;
                    foreach (var damage in specialAbility.CriticalDamages[weapon.CriticalMultiplier])
                    {
                        if (string.IsNullOrEmpty(damage.Type))
                        {
                            damage.Type = damageType;
                        }
                    }

                    weapon.CriticalDamages.AddRange(specialAbility.CriticalDamages[weapon.CriticalMultiplier]);

                    if (weapon.SecondaryHasAbilities)
                    {
                        var secondaryDamages = specialAbility.Damages.Select(d => d.Clone()).ToArray();
                        var secondaryDamageType = weapon.SecondaryCriticalDamages[0].Type;

                        foreach (var damage in specialAbility.CriticalDamages[weapon.SecondaryCriticalMultiplier])
                        {
                            if (string.IsNullOrEmpty(damage.Type))
                            {
                                damage.Type = secondaryDamageType;
                            }
                        }

                        weapon.SecondaryCriticalDamages.AddRange(specialAbility.CriticalDamages[weapon.SecondaryCriticalMultiplier]);
                    }
                }

                if (specialAbility.Name == SpecialAbilityConstants.Keen)
                {
                    weapon.ThreatRange *= 2;
                }
            }

            return weapon;
        }

        private IEnumerable<SpecialAbility> GetSpecialAbilities(string specificGearType, string name, IEnumerable<SpecialAbility> templateSpecialAbilities)
        {
            var tableName = string.Format(TableNameConstants.Collections.Formattable.SpecificITEMTYPESpecialAbilities, specificGearType);
            var abilityNames = collectionsSelector.SelectFrom(Config.Name, tableName, name);
            var abilityPrototypes = abilityNames.Select(n => new SpecialAbility { Name = n }).Union(templateSpecialAbilities);
            var abilities = specialAbilitiesGenerator.GenerateFor(abilityPrototypes);

            return abilities;
        }

        public Item GenerateFrom(Item template)
        {
            var gear = template.Clone();

            var specificGearType = GetSpecificGearType(gear.Name);
            gear.ItemType = GetItemType(specificGearType);
            gear.BaseNames = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Collections.Set.ItemGroups, gear.Name);

            gear = SetPrototypeAttributes(gear, specificGearType);

            return gear;
        }

        private string GetItemType(string specificGearType)
        {
            if (specificGearType == ItemTypeConstants.Weapon)
                return ItemTypeConstants.Weapon;

            return ItemTypeConstants.Armor;
        }

        private string GetSpecificGearType(string name)
        {
            var weapons = GetGear(ItemTypeConstants.Weapon);
            if (weapons.Contains(name))
                return ItemTypeConstants.Weapon;

            var armors = GetGear(ItemTypeConstants.Armor);
            if (armors.Contains(name))
                return ItemTypeConstants.Armor;

            var shields = GetGear(AttributeConstants.Shield);
            if (shields.Contains(name))
                return AttributeConstants.Shield;

            throw new ArgumentException($"{name} is not a valid specific item");
        }

        public bool IsSpecific(Item template)
        {
            return NameMatchesSpecific(template.Name);
        }

        public Item GeneratePrototypeFrom(string power, string specificGearType, string name, params string[] traits)
        {
            var possiblePowers = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Collections.Set.PowerGroups, name);
            var adjustedPower = PowerHelper.AdjustPower(power, possiblePowers);

            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERSpecificITEMTYPEs, adjustedPower, specificGearType);
            var selections = typeAndAmountPercentileSelector.SelectAllFrom(tableName);
            selections = selections.Where(s => NameMatchesWithReplacements(name, s.Type));

            var selection = collectionsSelector.SelectRandomFrom(selections);

            var gear = new Item();
            gear.Name = selection.Type;
            gear.BaseNames = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Collections.Set.ItemGroups, selection.Type);
            gear.ItemType = GetItemType(specificGearType);
            gear.Magic.Bonus = selection.Amount;
            gear.Quantity = 0;
            gear.Traits = new HashSet<string>(traits);

            return gear;
        }

        private bool NameMatchesSpecific(string name)
        {
            var weapons = GetGear(ItemTypeConstants.Weapon);
            var armors = GetGear(ItemTypeConstants.Armor);
            var shields = GetGear(AttributeConstants.Shield);

            var specificGear = weapons.Union(armors).Union(shields);

            return specificGear.Contains(name);
        }

        private bool NameMatchesWithReplacements(string source, string target)
        {
            var sourceReplacements = replacementSelector.SelectAll(source, true);
            var targetReplacements = replacementSelector.SelectAll(target, true);

            return source == target
                || sourceReplacements.Any(s => s == target)
                || targetReplacements.Any(t => t == source);
        }

        public string GenerateRandomNameFrom(string power, string specificGearType)
        {
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERSpecificITEMTYPEs, power, specificGearType);
            var selection = typeAndAmountPercentileSelector.SelectFrom(tableName);

            return selection.Type;
        }

        public string GenerateNameFrom(string power, string specificGearType, string baseType)
        {
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERSpecificITEMTYPEs, power, specificGearType);
            var selections = typeAndAmountPercentileSelector.SelectAllFrom(tableName);

            var names = new List<string>();

            foreach (var selection in selections)
            {
                var baseNames = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Collections.Set.ItemGroups, selection.Type);
                if (baseNames.Contains(baseType))
                    names.Add(selection.Type);
            }

            if (!names.Any())
            {
                throw new ArgumentException($"No {power} specific {specificGearType} has base type {baseType}");
            }

            var name = collectionsSelector.SelectRandomFrom(names);

            return name;
        }

        public bool IsSpecific(string specificGearType, string itemName)
        {
            var gearTypeCollection = GetGear(specificGearType);

            var isSpecific = NameMatchesSpecific(itemName);
            isSpecific &= gearTypeCollection.Any(i => NameMatchesWithReplacements(itemName, i));

            return isSpecific;
        }

        private IEnumerable<string> GetGear(string gearType)
        {
            switch (gearType)
            {
                case ItemTypeConstants.Weapon: return WeaponConstants.GetAllSpecific();
                case ItemTypeConstants.Armor: return ArmorConstants.GetAllSpecificArmors();
                case AttributeConstants.Shield: return ArmorConstants.GetAllSpecificShields();
                default: throw new ArgumentException($"{gearType} is not a valid specific gear type");
            }
        }

        public bool CanBeSpecific(string power, string specificGearType, string itemName)
        {
            if (IsSpecific(specificGearType, itemName))
                return true;

            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERSpecificITEMTYPEs, power, specificGearType);
            var powerSelections = typeAndAmountPercentileSelector.SelectAllFrom(tableName);

            foreach (var selection in powerSelections)
            {
                var baseNames = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Collections.Set.ItemGroups, selection.Type);
                if (baseNames.Contains(itemName))
                    return true;
            }

            return false;
        }
    }
}