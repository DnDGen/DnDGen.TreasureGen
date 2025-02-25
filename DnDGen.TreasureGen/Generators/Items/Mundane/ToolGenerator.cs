using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Mundane;
using DnDGen.TreasureGen.Selectors.Percentiles;
using DnDGen.TreasureGen.Tables;
using System.Collections.Generic;

namespace DnDGen.TreasureGen.Generators.Items.Mundane
{
    internal class ToolGenerator : MundaneItemGenerator
    {
        private readonly ITreasurePercentileSelector percentileSelector;

        public ToolGenerator(ITreasurePercentileSelector percentileSelector)
        {
            this.percentileSelector = percentileSelector;
        }

        public Item GenerateRandom()
        {
            var name = percentileSelector.SelectFrom(Config.Name, TableNameConstants.Percentiles.Tools);
            return Generate(name);
        }

        public Item Generate(string itemName, params string[] traits)
        {
            var tool = new Item
            {
                Name = itemName,
                ItemType = ItemTypeConstants.Tool,
                BaseNames = [itemName],
                Traits = new HashSet<string>(traits)
            };

            return tool;
        }

        public Item Generate(Item template, bool allowRandomDecoration = false)
        {
            var tool = template.MundaneClone();
            tool.ItemType = ItemTypeConstants.Tool;
            tool.Quantity = 1;
            tool.BaseNames = [tool.Name];
            tool.Attributes = [];

            return tool;
        }
    }
}