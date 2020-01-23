﻿using DnDGen.Infrastructure.Generators;
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

        public bool HasCurse(bool isMagical)
        {
            return isMagical && percentileSelector.SelectFrom<bool>(TableNameConstants.Percentiles.Set.IsItemCursed);
        }

        public string GenerateCurse()
        {
            var curse = percentileSelector.SelectFrom(TableNameConstants.Percentiles.Set.Curses);

            if (curse == CurseConstants.Intermittent)
                return string.Format("{0} ({1})", curse, GetIntermittentFunctioning());

            if (curse == CurseConstants.Drawback)
                return percentileSelector.SelectFrom(TableNameConstants.Percentiles.Set.CurseDrawbacks);

            return curse;
        }

        private string GetIntermittentFunctioning()
        {
            var roll = dice.Roll().d3().AsSum();

            if (roll == 1)
                return "Unreliable";

            if (roll == 3)
                return "Uncontrolled";

            var situation = percentileSelector.SelectFrom(TableNameConstants.Percentiles.Set.CursedDependentSituations);
            return $"Dependent: {situation}";
        }

        public Item Generate()
        {
            var name = percentileSelector.SelectFrom(TableNameConstants.Percentiles.Set.SpecificCursedItems);
            return Generate(name);
        }

        public Item Generate(string itemName)
        {
            if (!CanBeSpecificCursedItem(itemName))
                return null;

            var cursedName = GetCursedName(itemName);
            var prototype = GeneratePrototype(cursedName);
            var specificCursedItem = GenerateFromPrototype(prototype);

            return specificCursedItem;
        }

        private string GetCursedName(string itemName)
        {
            if (IsSpecificCursedItem(itemName))
                return itemName;

            var cursedItems = collectionsSelector.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, CurseConstants.SpecificCursedItem);
            var cursedNames = new List<string>();

            foreach (var cursedName in cursedItems)
            {
                var baseNames = collectionsSelector.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, cursedName);
                if (baseNames.Contains(itemName))
                    cursedNames.Add(cursedName);
            }

            var name = collectionsSelector.SelectRandomFrom(cursedNames);

            return name;
        }

        private Item GeneratePrototype(string name)
        {
            var specificCursedItem = new Item();
            specificCursedItem.Name = name;
            specificCursedItem.BaseNames = collectionsSelector.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, name);
            specificCursedItem.Magic.Curse = CurseConstants.SpecificCursedItem;

            return specificCursedItem;
        }

        private Item GenerateFromPrototype(Item prototype)
        {
            var specificCursedItem = prototype.Clone();
            specificCursedItem.ItemType = collectionsSelector.SelectFrom(TableNameConstants.Collections.Set.SpecificCursedItemItemTypes, specificCursedItem.Name).Single();
            specificCursedItem.Attributes = collectionsSelector.SelectFrom(TableNameConstants.Collections.Set.SpecificCursedItemAttributes, specificCursedItem.Name);

            if (specificCursedItem.ItemType == ItemTypeConstants.Armor)
                return GetArmor(specificCursedItem);

            if (specificCursedItem.ItemType == ItemTypeConstants.Weapon)
                return GetWeapon(specificCursedItem);

            return specificCursedItem;
        }

        private Item GetArmor(Item cursedItem)
        {
            var template = new Armor();
            template.Name = cursedItem.BaseNames.First();

            var mundaneArmorGenerator = justInTimeFactory.Build<MundaneItemGenerator>(ItemTypeConstants.Armor);
            var armor = mundaneArmorGenerator.GenerateFrom(template);

            cursedItem.CloneInto(armor);

            if (armor.IsMagical)
                armor.Traits.Add(TraitConstants.Masterwork);

            return armor;
        }

        private Item GetWeapon(Item cursedItem)
        {
            var template = new Weapon();
            template.Name = cursedItem.BaseNames.First();

            var mundaneWeaponGenerator = justInTimeFactory.Build<MundaneItemGenerator>(ItemTypeConstants.Weapon);
            var weapon = mundaneWeaponGenerator.GenerateFrom(template);

            cursedItem.Quantity = weapon.Quantity;
            cursedItem.CloneInto(weapon);

            if (weapon.IsMagical)
                weapon.Traits.Add(TraitConstants.Masterwork);

            return weapon;
        }

        public bool IsSpecificCursedItem(Item template)
        {
            return IsSpecificCursedItem(template.Name);
        }

        public bool IsSpecificCursedItem(string itemName)
        {
            var cursedItems = collectionsSelector.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, CurseConstants.SpecificCursedItem);
            return cursedItems.Contains(itemName);
        }

        public bool CanBeSpecificCursedItem(string itemName)
        {
            if (IsSpecificCursedItem(itemName))
                return true;

            var cursedItems = collectionsSelector.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, CurseConstants.SpecificCursedItem);
            foreach (var item in cursedItems)
            {
                var baseNames = collectionsSelector.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, item);
                if (baseNames.Contains(itemName))
                    return true;
            }

            return false;
        }

        public Item GenerateFrom(Item template, bool allowDecoration = false)
        {
            var prototype = template.SmartClone();
            prototype.BaseNames = collectionsSelector.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, prototype.Name);
            prototype.Magic.Curse = CurseConstants.SpecificCursedItem;
            prototype.Quantity = 1;
            prototype.Magic.SpecialAbilities = Enumerable.Empty<SpecialAbility>();

            var cursedItem = GenerateFromPrototype(prototype);

            return cursedItem;
        }
    }
}