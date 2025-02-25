using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Selectors.Percentiles;
using DnDGen.TreasureGen.Tables;
using System.Collections.Generic;

namespace DnDGen.TreasureGen.Generators.Items.Magical
{
    internal class WandGenerator : MagicalItemGenerator
    {
        private readonly ITreasurePercentileSelector percentileSelector;
        private readonly IChargesGenerator chargesGenerator;

        public WandGenerator(ITreasurePercentileSelector percentileSelector, IChargesGenerator chargesGenerator)
        {
            this.percentileSelector = percentileSelector;
            this.chargesGenerator = chargesGenerator;
        }

        public Item GenerateRandom(string power)
        {
            var tablename = TableNameConstants.Percentiles.POWERITEMTYPEs(power, ItemTypeConstants.Wand);
            var spell = percentileSelector.SelectFrom(Config.Name, tablename);
            var name = $"Wand of {spell}";

            return GenerateWand(name);
        }

        private Item GenerateWand(string name, params string[] traits)
        {
            var wand = new Item();
            wand.ItemType = ItemTypeConstants.Wand;
            wand.IsMagical = true;
            wand.Name = name;
            wand.Magic.Charges = chargesGenerator.GenerateFor(ItemTypeConstants.Wand, name);
            wand.BaseNames = new[] { ItemTypeConstants.Wand };
            wand.Attributes = new[] { AttributeConstants.Charged, AttributeConstants.OneTimeUse };
            wand.Traits = new HashSet<string>(traits);

            return wand;
        }

        public Item Generate(string power, string itemName, params string[] traits)
        {
            return GenerateWand(itemName, traits);
        }

        public Item Generate(Item template, bool allowRandomDecoration = false)
        {
            var wand = template.Clone();
            wand.IsMagical = true;
            wand.Quantity = 1;
            wand.BaseNames = [ItemTypeConstants.Wand];
            wand.Attributes = [AttributeConstants.Charged, AttributeConstants.OneTimeUse];
            wand.ItemType = ItemTypeConstants.Wand;

            return wand.SmartClone();
        }
    }
}