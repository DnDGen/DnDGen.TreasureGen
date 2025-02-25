using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.Infrastructure.Selectors.Percentiles;
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
        private readonly IPercentileTypeAndAmountSelector typeAndAmountPercentileSelector;

        public WondrousItemGenerator(
            ITreasurePercentileSelector percentileSelector,
            ICollectionSelector collectionsSelector,
            IChargesGenerator chargesGenerator,
            Dice dice,
            ISpellGenerator spellGenerator,
            IPercentileTypeAndAmountSelector typeAndAmountPercentileSelector)
        {
            this.percentileSelector = percentileSelector;
            this.collectionsSelector = collectionsSelector;
            this.chargesGenerator = chargesGenerator;
            this.dice = dice;
            this.spellGenerator = spellGenerator;
            this.typeAndAmountPercentileSelector = typeAndAmountPercentileSelector;
        }

        public Item GenerateRandom(string power)
        {
            var tableName = TableNameConstants.Percentiles.POWERITEMTYPEs(power, ItemTypeConstants.WondrousItem);
            var result = typeAndAmountPercentileSelector.SelectFrom(Config.Name, tableName);

            var item = BuildWondrousItem(result.Type);
            item.Magic.Bonus = result.Amount;

            return item;
        }

        public Item Generate(string power, string itemName, params string[] traits)
        {
            var possiblePowers = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, itemName);
            var adjustedPower = PowerHelper.AdjustPower(power, possiblePowers);

            var tableName = TableNameConstants.Percentiles.POWERITEMTYPEs(adjustedPower, ItemTypeConstants.WondrousItem);
            var results = typeAndAmountPercentileSelector.SelectAllFrom(Config.Name, tableName);
            var matches = results.Where(r => r.Type == itemName);

            var result = collectionsSelector.SelectRandomFrom(matches);
            var item = BuildWondrousItem(itemName, traits);
            item.Magic.Bonus = result.Amount;

            return item;
        }

        private Item BuildWondrousItem(string name, params string[] traits)
        {
            var item = new Item
            {
                Name = name,
                BaseNames = [name],
                IsMagical = true,
                ItemType = ItemTypeConstants.WondrousItem,
                Traits = new HashSet<string>(traits)
            };

            var tableName = TableNameConstants.Collections.ITEMTYPEAttributes(item.ItemType);
            item.Attributes = collectionsSelector.SelectFrom(Config.Name, tableName, name);

            if (item.Attributes.Contains(AttributeConstants.Charged))
                item.Magic.Charges = chargesGenerator.GenerateFor(item.ItemType, name);

            if (!item.Traits.Any())
            {
                var trait = GetTraitFor(name);
                if (!string.IsNullOrEmpty(trait))
                    item.Traits.Add(trait);
            }

            var contents = GetContentsFor(name);
            item.Contents.AddRange(contents);

            return item;
        }

        private string GetTraitFor(string name)
        {
            switch (name)
            {
                case WondrousItemConstants.HornOfValhalla: return percentileSelector.SelectFrom(Config.Name, TableNameConstants.Percentiles.HornOfValhallaTypes);
                case WondrousItemConstants.CandleOfInvocation: return percentileSelector.SelectFrom(Config.Name, TableNameConstants.Percentiles.IntelligenceAlignments);
                case WondrousItemConstants.RobeOfTheArchmagi: return percentileSelector.SelectFrom(Config.Name, TableNameConstants.Percentiles.RobeOfTheArchmagiColors);
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

            return [];
        }

        private IEnumerable<string> GetIronFlaskContents()
        {
            var contents = percentileSelector.SelectFrom(Config.Name, TableNameConstants.Percentiles.IronFlaskContents);

            if (string.IsNullOrEmpty(contents))
                return [];

            if (contents == TableNameConstants.Percentiles.BalorOrPitFiend)
                contents = percentileSelector.SelectFrom(Config.Name, TableNameConstants.Percentiles.BalorOrPitFiend);

            return [contents];
        }

        private IEnumerable<string> GetRobeOfUsefulItemsItems()
        {
            var baseItems = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Collections.WondrousItemContents, WondrousItemConstants.RobeOfUsefulItems);
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
            var fullContents = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Collections.WondrousItemContents, name).ToList();

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
                var item = percentileSelector.SelectFrom(Config.Name, TableNameConstants.Percentiles.RobeOfUsefulItemsExtraItems);

                if (item == ItemTypeConstants.Scroll)
                {
                    var spellType = spellGenerator.GenerateType();
                    var level = spellGenerator.GenerateLevel(PowerConstants.Minor);
                    var spell = spellGenerator.Generate(spellType, level);

                    item = $"{spellType} scroll of {spell} ({level})";
                }

                extraItems.Add(item);
            }

            return extraItems;
        }

        private IEnumerable<string> GeneratePlanesForCubicGate()
        {
            var planes = new HashSet<string>
            {
                "Material Plane"
            };

            while (planes.Count < 6)
            {
                var plane = percentileSelector.SelectFrom(Config.Name, TableNameConstants.Percentiles.Planes);
                planes.Add(plane);
            }

            return planes;
        }

        public Item Generate(Item template, bool allowRandomDecoration = false)
        {
            var item = template.Clone();
            item.IsMagical = true;
            item.BaseNames = [item.Name];

            var tableName = TableNameConstants.Collections.ITEMTYPEAttributes(ItemTypeConstants.WondrousItem);
            item.Attributes = collectionsSelector.SelectFrom(Config.Name, tableName, item.Name);
            item.ItemType = ItemTypeConstants.WondrousItem;

            return item.SmartClone();
        }
    }
}