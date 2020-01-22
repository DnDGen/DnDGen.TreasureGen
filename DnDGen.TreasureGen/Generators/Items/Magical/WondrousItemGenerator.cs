﻿using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.RollGen;
using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Selectors.Percentiles;
using DnDGen.TreasureGen.Tables;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.TreasureGen.Generators.Items.Magical
{
    internal class WondrousItemGenerator : MagicalItemGenerator
    {
        private readonly ITreasurePercentileSelector percentileSelector;
        private readonly ICollectionSelector collectionsSelector;
        private readonly IChargesGenerator chargesGenerator;
        private readonly Dice dice;
        private readonly ISpellGenerator spellGenerator;
        private readonly ITypeAndAmountPercentileSelector typeAndAmountPercentileSelector;

        public WondrousItemGenerator(
            ITreasurePercentileSelector percentileSelector,
            ICollectionSelector collectionsSelector,
            IChargesGenerator chargesGenerator,
            Dice dice,
            ISpellGenerator spellGenerator,
            ITypeAndAmountPercentileSelector typeAndAmountPercentileSelector)
        {
            this.percentileSelector = percentileSelector;
            this.collectionsSelector = collectionsSelector;
            this.chargesGenerator = chargesGenerator;
            this.dice = dice;
            this.spellGenerator = spellGenerator;
            this.typeAndAmountPercentileSelector = typeAndAmountPercentileSelector;
        }

        public Item GenerateFrom(string power)
        {
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, power, ItemTypeConstants.WondrousItem);
            var result = typeAndAmountPercentileSelector.SelectFrom(tableName);

            var item = BuildWondrousItem(result.Type);
            item.Magic.Bonus = result.Amount;

            return item;
        }

        public Item GenerateFrom(string power, string itemName)
        {
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, power, ItemTypeConstants.WondrousItem);
            var results = typeAndAmountPercentileSelector.SelectAllFrom(tableName);
            var matches = results.Where(r => r.Type == itemName);

            if (!matches.Any())
            {
                throw new ArgumentException($"{itemName} is not a valid {power} Wondrous Item");
            }

            var result = collectionsSelector.SelectRandomFrom(matches);
            var item = BuildWondrousItem(itemName);
            item.Magic.Bonus = result.Amount;

            return item;
        }

        private Item BuildWondrousItem(string name)
        {
            var item = new Item();
            item.Name = name;
            item.BaseNames = new[] { name };
            item.IsMagical = true;
            item.ItemType = ItemTypeConstants.WondrousItem;

            var tableName = string.Format(TableNameConstants.Collections.Formattable.ITEMTYPEAttributes, item.ItemType);
            item.Attributes = collectionsSelector.SelectFrom(tableName, name);

            if (item.Attributes.Contains(AttributeConstants.Charged))
                item.Magic.Charges = chargesGenerator.GenerateFor(item.ItemType, name);

            var trait = GetTraitFor(name);
            if (!string.IsNullOrEmpty(trait))
                item.Traits.Add(trait);

            var contents = GetContentsFor(name);
            item.Contents.AddRange(contents);

            return item;
        }

        private string GetTraitFor(string name)
        {
            switch (name)
            {
                case WondrousItemConstants.HornOfValhalla: return percentileSelector.SelectFrom(TableNameConstants.Percentiles.Set.HornOfValhallaTypes);
                case WondrousItemConstants.CandleOfInvocation: return percentileSelector.SelectFrom(TableNameConstants.Percentiles.Set.IntelligenceAlignments);
                case WondrousItemConstants.RobeOfTheArchmagi: return percentileSelector.SelectFrom(TableNameConstants.Percentiles.Set.RobeOfTheArchmagiColors);
                default: return string.Empty;
            }
        }

        private IEnumerable<string> GetContentsFor(string name)
        {
            switch (name)
            {
                case WondrousItemConstants.IronFlask: return GetIronFlaskContents();
                case WondrousItemConstants.RobeOfUsefulItems: return GetRobeOfUsefulItemsItems();
                case WondrousItemConstants.CubicGate: return GeneratePlanesForCubicGate();
            }

            if (ItemHasPartialContents(name))
                return GetPartialContents(name);

            return Enumerable.Empty<string>();
        }

        private IEnumerable<string> GetIronFlaskContents()
        {
            var contents = percentileSelector.SelectFrom(TableNameConstants.Percentiles.Set.IronFlaskContents);

            if (string.IsNullOrEmpty(contents))
                return Enumerable.Empty<string>();

            if (contents == TableNameConstants.Percentiles.Set.BalorOrPitFiend)
                contents = percentileSelector.SelectFrom(TableNameConstants.Percentiles.Set.BalorOrPitFiend);

            return new[] { contents };
        }

        private IEnumerable<string> GetRobeOfUsefulItemsItems()
        {
            var baseItems = collectionsSelector.SelectFrom(TableNameConstants.Collections.Set.WondrousItemContents, WondrousItemConstants.RobeOfUsefulItems);
            var extraItems = GenerateExtraItemsInRobeOfUsefulItems();

            //INFO: Can't do Union because it will deduplicate the allowed duplicate items
            var items = new List<string>();
            items.AddRange(baseItems);
            items.AddRange(extraItems);

            return items;
        }

        private IEnumerable<string> GetPartialContents(string name)
        {
            var quantity = chargesGenerator.GenerateFor(ItemTypeConstants.WondrousItem, name);
            var fullContents = collectionsSelector.SelectFrom(TableNameConstants.Collections.Set.WondrousItemContents, name).ToList();

            if (quantity >= fullContents.Count)
                return fullContents;

            var contents = new List<string>();

            while (quantity-- > 0)
            {
                var randomContents = collectionsSelector.SelectRandomFrom(fullContents);

                contents.Add(randomContents);
                fullContents.Remove(randomContents);
            }

            return contents;
        }

        private bool ItemHasPartialContents(string name)
        {
            if (name == WondrousItemConstants.RobeOfBones)
                return true;

            if (name.StartsWith(WondrousItemConstants.NecklaceOfFireballs))
                return true;

            if (name == WondrousItemConstants.DeckOfIllusions)
                return true;

            return false;
        }

        private IEnumerable<string> GenerateExtraItemsInRobeOfUsefulItems()
        {
            var extraItems = new List<string>();
            var quantity = dice.Roll(4).d4().AsSum();

            while (quantity-- > 0)
            {
                var item = percentileSelector.SelectFrom(TableNameConstants.Percentiles.Set.RobeOfUsefulItemsExtraItems);

                if (item == ItemTypeConstants.Scroll)
                {
                    var spellType = spellGenerator.GenerateType();
                    var level = spellGenerator.GenerateLevel(PowerConstants.Minor);
                    var spell = spellGenerator.Generate(spellType, level);

                    item = string.Format("{0} scroll of {1} ({2})", spellType, spell, level);
                }

                extraItems.Add(item);
            }

            return extraItems;
        }

        private IEnumerable<string> GeneratePlanesForCubicGate()
        {
            var planes = new HashSet<string>();
            planes.Add("Material Plane");

            while (planes.Count < 6)
            {
                var plane = percentileSelector.SelectFrom(TableNameConstants.Percentiles.Set.Planes);
                planes.Add(plane);
            }

            return planes;
        }

        public Item GenerateFrom(Item template, bool allowRandomDecoration = false)
        {
            var item = template.Clone();
            item.IsMagical = true;
            item.BaseNames = new[] { item.Name };

            var tableName = string.Format(TableNameConstants.Collections.Formattable.ITEMTYPEAttributes, ItemTypeConstants.WondrousItem);
            item.Attributes = collectionsSelector.SelectFrom(tableName, item.Name);
            item.ItemType = ItemTypeConstants.WondrousItem;

            return item.SmartClone();
        }
    }
}