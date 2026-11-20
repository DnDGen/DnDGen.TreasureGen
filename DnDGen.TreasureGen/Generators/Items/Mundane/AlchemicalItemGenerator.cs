using DnDGen.Infrastructure.Models;
using DnDGen.Infrastructure.Selectors.Percentiles;
using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Mundane;
using DnDGen.TreasureGen.Tables;
using System.Linq;

namespace DnDGen.TreasureGen.Generators.Items.Mundane
{
    internal class AlchemicalItemGenerator(IPercentileTypeAndAmountSelector typeAndAmountPercentileSelector) : MundaneItemGenerator
    {
        public Item GenerateRandom()
        {
            var result = typeAndAmountPercentileSelector.SelectFrom(Config.Name, TableNameConstants.Percentiles.AlchemicalItems);

            return Generate(result.Type);
        }

        public Item Generate(string itemName, params string[] traits)
        {
            var selections = typeAndAmountPercentileSelector.SelectAllFrom(Config.Name, TableNameConstants.Percentiles.AlchemicalItems);
            var selection = selections.FirstOrDefault(s => s.Type == itemName) ?? new TypeAndAmountDataSelection
            {
                Type = itemName,
                AmountAsDouble = 1
            };

            var item = new Item
            {
                Name = itemName,
                Quantity = selection.Amount,
                ItemType = ItemTypeConstants.AlchemicalItem,
                BaseNames = [itemName],
                Traits = [.. traits]
            };

            return item;
        }

        public Item Generate(Item template, bool allowRandomDecoration = false)
        {
            var item = template.MundaneClone();
            item.ItemType = ItemTypeConstants.AlchemicalItem;
            item.BaseNames = [item.Name];
            item.Attributes = [];

            return item;
        }
    }
}