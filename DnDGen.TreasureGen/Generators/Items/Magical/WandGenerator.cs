using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Selectors.Percentiles;
using DnDGen.TreasureGen.Tables;

namespace DnDGen.TreasureGen.Generators.Items.Magical
{
    internal class WandGenerator(ITreasurePercentileSelector percentileSelector, IChargesGenerator chargesGenerator) : MagicalItemGenerator
    {
        public Item GenerateRandom(string power)
        {
            var tablename = TableNameConstants.Percentiles.POWERITEMTYPEs(power, ItemTypeConstants.Wand);
            var spell = percentileSelector.SelectFrom(Config.Name, tablename);
            var name = $"Wand of {spell}";

            return GenerateWand(name);
        }

        private Item GenerateWand(string name, params string[] traits)
        {
            var wand = new Item
            {
                ItemType = ItemTypeConstants.Wand,
                IsMagical = true,
                Name = name
            };
            wand.Magic.Charges = chargesGenerator.GenerateFor(ItemTypeConstants.Wand, name);
            wand.BaseNames = [ItemTypeConstants.Wand];
            wand.Attributes = [AttributeConstants.Charged, AttributeConstants.OneTimeUse];
            wand.Traits = [.. traits];

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