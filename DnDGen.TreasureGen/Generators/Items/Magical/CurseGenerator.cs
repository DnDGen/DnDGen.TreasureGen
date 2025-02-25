using DnDGen.Infrastructure.Factories;
using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.RollGen;
using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Items.Mundane;
using DnDGen.TreasureGen.Selectors.Percentiles;
using DnDGen.TreasureGen.Tables;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.TreasureGen.Generators.Items.Magical
{
    internal class CurseGenerator : ICurseGenerator
    {
        private readonly Dice dice;
        private readonly ITreasurePercentileSelector percentileSelector;
        private readonly ICollectionSelector collectionsSelector;
        private readonly JustInTimeFactory justInTimeFactory;

        public CurseGenerator(Dice dice, ITreasurePercentileSelector percentileSelector, ICollectionSelector collectionsSelector, JustInTimeFactory justInTimeFactory)
        {
            this.dice = dice;
            this.percentileSelector = percentileSelector;
            this.collectionsSelector = collectionsSelector;
            this.justInTimeFactory = justInTimeFactory;
        }

        public bool HasCurse(Item item)
        {
            return item.IsMagical && percentileSelector.SelectFrom<bool>(Config.Name, TableNameConstants.Percentiles.IsItemCursed);
        }

        public string GenerateCurse()
        {
            var curse = percentileSelector.SelectFrom(Config.Name, TableNameConstants.Percentiles.Curses);

            if (curse == CurseConstants.Intermittent)
            {
                var intermittent = GetIntermittentFunctioning();
                return $"{curse} ({intermittent})";
            }

            if (curse == CurseConstants.Drawback)
                return percentileSelector.SelectFrom(Config.Name, TableNameConstants.Percentiles.CurseDrawbacks);

            return curse;
        }

        private string GetIntermittentFunctioning()
        {
            var roll = dice.Roll().d3().AsSum();

            if (roll == 1)
                return "Unreliable";

            if (roll == 3)
                return "Uncontrolled";

            var situation = percentileSelector.SelectFrom(Config.Name, TableNameConstants.Percentiles.CursedDependentSituations);
            return $"Dependent: {situation}";
        }

        public Item GenerateRandom()
        {
            var name = percentileSelector.SelectFrom(Config.Name, TableNameConstants.Percentiles.SpecificCursedItems);
            return Generate(name);
        }

        public Item Generate(string itemName, params string[] traits)
        {
            if (!CanBeSpecificCursedItem(itemName))
                return null;

            var cursedName = GetCursedName(itemName);
            var prototype = GeneratePrototype(cursedName);
            prototype.Traits = new HashSet<string>(traits);

            var specificCursedItem = GenerateFromPrototype(prototype);

            return specificCursedItem;
        }

        private string GetCursedName(string itemName)
        {
            if (IsSpecificCursedItem(itemName))
                return itemName;

            var cursedItems = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Collections.ItemGroups, CurseConstants.SpecificCursedItem);
            var cursedNames = new List<string>();

            foreach (var cursedName in cursedItems)
            {
                var baseNames = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Collections.ItemGroups, cursedName);
                if (baseNames.Contains(itemName))
                    cursedNames.Add(cursedName);
            }

            var name = collectionsSelector.SelectRandomFrom(cursedNames);

            return name;
        }

        private Item GeneratePrototype(string name)
        {
            var specificCursedItem = new Item
            {
                Name = name,
                BaseNames = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Collections.ItemGroups, name)
            };
            specificCursedItem.Magic.Curse = CurseConstants.SpecificCursedItem;

            return specificCursedItem;
        }

        private Item GenerateFromPrototype(Item prototype)
        {
            var specificCursedItem = prototype.Clone();
            specificCursedItem.ItemType = collectionsSelector
                .SelectFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemItemTypes, specificCursedItem.Name)
                .Single();
            specificCursedItem.Attributes = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemAttributes, specificCursedItem.Name);

            if (specificCursedItem.ItemType == ItemTypeConstants.Armor)
                return GetArmor(specificCursedItem);

            if (specificCursedItem.ItemType == ItemTypeConstants.Weapon)
                return GetWeapon(specificCursedItem);

            return specificCursedItem;
        }

        private Item GetArmor(Item cursedItem)
        {
            var name = cursedItem.BaseNames.First();

            var mundaneArmorGenerator = justInTimeFactory.Build<MundaneItemGenerator>(ItemTypeConstants.Armor);
            var armor = mundaneArmorGenerator.Generate(name, cursedItem.Traits.ToArray()) as Armor;

            cursedItem.CloneInto(armor);

            if (armor.IsMagical)
                armor.Traits.Add(TraitConstants.Masterwork);

            armor.Traits.Remove(armor.Size);

            return armor;
        }

        private Item GetWeapon(Item cursedItem)
        {
            var name = cursedItem.BaseNames.First();

            var mundaneWeaponGenerator = justInTimeFactory.Build<MundaneItemGenerator>(ItemTypeConstants.Weapon);
            var weapon = mundaneWeaponGenerator.Generate(name, cursedItem.Traits.ToArray()) as Weapon;

            cursedItem.Quantity = weapon.Quantity;
            cursedItem.CloneInto(weapon);

            if (weapon.IsMagical)
                weapon.Traits.Add(TraitConstants.Masterwork);

            weapon.Traits.Remove(weapon.Size);

            return weapon;
        }

        public bool IsSpecificCursedItem(Item template)
        {
            return IsSpecificCursedItem(template.Name);
        }

        public bool IsSpecificCursedItem(string itemName)
        {
            var cursedItems = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Collections.ItemGroups, CurseConstants.SpecificCursedItem);
            return cursedItems.Contains(itemName);
        }

        public bool CanBeSpecificCursedItem(string itemName)
        {
            if (IsSpecificCursedItem(itemName))
                return true;

            var cursedItems = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Collections.ItemGroups, CurseConstants.SpecificCursedItem);
            foreach (var item in cursedItems)
            {
                var baseNames = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Collections.ItemGroups, item);
                if (baseNames.Contains(itemName))
                    return true;
            }

            return false;
        }

        public bool ItemTypeCanBeSpecificCursedItem(string itemType)
        {
            var itemTypes = collectionsSelector.SelectAllFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemItemTypes);
            return itemTypes.Values.SelectMany(v => v).Contains(itemType);
        }

        public Item Generate(Item template, bool allowDecoration = false)
        {
            var prototype = template.SmartClone();
            prototype.BaseNames = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Collections.ItemGroups, prototype.Name);
            prototype.Magic.Curse = CurseConstants.SpecificCursedItem;
            prototype.Quantity = 1;
            prototype.Magic.SpecialAbilities = Enumerable.Empty<SpecialAbility>();

            var cursedItem = GenerateFromPrototype(prototype);

            return cursedItem;
        }

        public Item GenerateSpecificCursedItem(string itemType)
        {
            if (!ItemTypeCanBeSpecificCursedItem(itemType))
                return null;

            var cursedName = GetCursedNameFromItemType(itemType);
            var prototype = GeneratePrototype(cursedName);
            var specificCursedItem = GenerateFromPrototype(prototype);

            return specificCursedItem;
        }

        private string GetCursedNameFromItemType(string itemType)
        {
            var cursedItems = collectionsSelector.SelectAllFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemItemTypes);
            var cursedNames = cursedItems
                .Where(kvp => kvp.Value.Contains(itemType))
                .Select(kvp => kvp.Key);

            var name = collectionsSelector.SelectRandomFrom(cursedNames);

            return name;
        }
    }
}