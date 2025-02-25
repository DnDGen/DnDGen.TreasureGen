using DnDGen.Infrastructure.Factories;
using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.Infrastructure.Selectors.Percentiles;
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
        private readonly IPercentileTypeAndAmountSelector typeAndAmountPercentileSelector;
        private readonly ICollectionSelector collectionsSelector;
        private readonly IChargesGenerator chargesGenerator;
        private readonly ITreasurePercentileSelector percentileSelector;
        private readonly ISpellGenerator spellGenerator;
        private readonly ISpecialAbilitiesGenerator specialAbilitiesGenerator;
        private readonly JustInTimeFactory justInTimeFactory;
        private readonly IReplacementSelector replacementSelector;

        public SpecificGearGenerator(IPercentileTypeAndAmountSelector typeAndAmountPercentileSelector,
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
                var hasSpell = percentileSelector.SelectFrom<bool>(Config.Name, TableNameConstants.Percentiles.CastersShieldContainsSpell);

                if (hasSpell)
                {
                    var spellType = percentileSelector.SelectFrom(Config.Name, TableNameConstants.Percentiles.CastersShieldSpellTypes);
                    var spellLevel = spellGenerator.GenerateLevel(PowerConstants.Medium);
                    var spell = spellGenerator.Generate(spellType, spellLevel);
                    var formattedSpell = $"{spell} ({spellType}, {spellLevel})";
                    gear.Contents.Add(formattedSpell);
                }
            }

            var templateName = gear.Name;
            gear.Name = replacementSelector.SelectSingle(templateName);
            gear.Magic.SpecialAbilities = GetSpecialAbilities(specificGearType, templateName, prototype.Magic.SpecialAbilities, string.Empty, true);

            var tableName = TableNameConstants.Collections.SpecificITEMTYPEAttributes(specificGearType);
            gear.Attributes = collectionsSelector.SelectFrom(Config.Name, tableName, templateName);

            tableName = TableNameConstants.Collections.SpecificITEMTYPETraits(specificGearType);
            var traits = collectionsSelector.SelectFrom(Config.Name, tableName, templateName);

            foreach (var trait in traits)
                gear.Traits.Add(trait);

            if (gear.Attributes.Contains(AttributeConstants.Charged))
                gear.Magic.Charges = chargesGenerator.GenerateFor(specificGearType, templateName);

            if (gear.Name == WeaponConstants.SlayingArrow || gear.Name == WeaponConstants.GreaterSlayingArrow)
            {
                var designatedFoe = collectionsSelector.SelectRandomFrom(
                    Config.Name,
                    TableNameConstants.Collections.ReplacementStrings,
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
            var armor = mundaneArmorGenerator.Generate(name, [.. gear.Traits]) as Armor;

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
            var weapon = mundaneWeaponGenerator.Generate(name, [.. gear.Traits]) as Weapon;

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

            weapon.Magic.SpecialAbilities = GetSpecialAbilities(
                ItemTypeConstants.Weapon,
                gear.Name,
                weapon.Magic.SpecialAbilities,
                weapon.CriticalMultiplier,
                false);
            weapon = specialAbilitiesGenerator.ApplyAbilitiesToWeapon(weapon);

            return weapon;
        }

        private IEnumerable<SpecialAbility> GetSpecialAbilities(
            string specificGearType,
            string name,
            IEnumerable<SpecialAbility> templateSpecialAbilities,
            string criticalMultiplier,
            bool addDefaultAbilities)
        {
            var abilityPrototypes = templateSpecialAbilities;

            if (addDefaultAbilities)
            {
                var tableName = TableNameConstants.Collections.SpecificITEMTYPESpecialAbilities(specificGearType);
                var abilityNames = collectionsSelector.SelectFrom(Config.Name, tableName, name);
                abilityPrototypes = abilityNames.Select(n => new SpecialAbility { Name = n }).Union(templateSpecialAbilities);
            }

            var abilities = specialAbilitiesGenerator.GenerateFor(abilityPrototypes, criticalMultiplier);

            return abilities;
        }

        public Item GenerateFrom(Item template)
        {
            var gear = template.Clone();

            var specificGearType = GetSpecificGearType(gear.Name);
            gear.ItemType = GetItemType(specificGearType);
            gear.BaseNames = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Collections.ItemGroups, gear.Name);

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
            var possiblePowers = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, name);
            var adjustedPower = PowerHelper.AdjustPower(power, possiblePowers);

            var tableName = TableNameConstants.Percentiles.POWERSpecificITEMTYPEs(adjustedPower, specificGearType);
            var selections = typeAndAmountPercentileSelector.SelectAllFrom(Config.Name, tableName);
            selections = selections.Where(s => NameMatchesWithReplacements(name, s.Type));

            var selection = collectionsSelector.SelectRandomFrom(selections);

            var gear = new Item
            {
                Name = selection.Type,
                BaseNames = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Collections.ItemGroups, selection.Type),
                ItemType = GetItemType(specificGearType)
            };
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
            var tableName = TableNameConstants.Percentiles.POWERSpecificITEMTYPEs(power, specificGearType);
            var selection = typeAndAmountPercentileSelector.SelectFrom(Config.Name, tableName);

            return selection.Type;
        }

        public string GenerateNameFrom(string power, string specificGearType, string baseType)
        {
            var tableName = TableNameConstants.Percentiles.POWERSpecificITEMTYPEs(power, specificGearType);
            var selections = typeAndAmountPercentileSelector.SelectAllFrom(Config.Name, tableName);

            var names = new List<string>();

            foreach (var selection in selections)
            {
                var baseNames = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Collections.ItemGroups, selection.Type);
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
            return gearType switch
            {
                ItemTypeConstants.Weapon => WeaponConstants.GetAllSpecific(),
                ItemTypeConstants.Armor => ArmorConstants.GetAllSpecificArmors(),
                AttributeConstants.Shield => ArmorConstants.GetAllSpecificShields(),
                _ => throw new ArgumentException($"{gearType} is not a valid specific gear type"),
            };
        }

        public bool CanBeSpecific(string power, string specificGearType, string itemName)
        {
            if (IsSpecific(specificGearType, itemName))
                return true;

            var tableName = TableNameConstants.Percentiles.POWERSpecificITEMTYPEs(power, specificGearType);
            var powerSelections = typeAndAmountPercentileSelector.SelectAllFrom(Config.Name, tableName);

            foreach (var selection in powerSelections)
            {
                var baseNames = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Collections.ItemGroups, selection.Type);
                if (baseNames.Contains(itemName))
                    return true;
            }

            return false;
        }
    }
}