using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Selectors.Percentiles;
using DnDGen.TreasureGen.Tables;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.TreasureGen.Generators.Items.Magical
{
    internal class MagicalItemTraitsGenerator : IMagicalItemTraitsGenerator
    {
        private readonly ITreasurePercentileSelector percentileSelector;

        public MagicalItemTraitsGenerator(ITreasurePercentileSelector percentileSelector)
        {
            this.percentileSelector = percentileSelector;
        }

        public IEnumerable<string> GenerateFor(string itemType, IEnumerable<string> attributes)
        {
            var tableName = GetTableName(itemType, attributes);
            var result = percentileSelector.SelectFrom(Config.Name, tableName);

            if (string.IsNullOrEmpty(result))
                return [];

            return result.Split(',');
        }

        private string GetTableName(string itemType, IEnumerable<string> attributes)
        {
            if (attributes.Contains(AttributeConstants.Melee))
                return TableNameConstants.Percentiles.ITEMTYPETraits(AttributeConstants.Melee);

            if (attributes.Contains(AttributeConstants.Ranged))
                return TableNameConstants.Percentiles.ITEMTYPETraits(AttributeConstants.Ranged);

            return TableNameConstants.Percentiles.ITEMTYPETraits(itemType);
        }
    }
}