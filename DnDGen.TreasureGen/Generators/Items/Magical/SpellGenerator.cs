using DnDGen.TreasureGen.Selectors.Percentiles;
using DnDGen.TreasureGen.Tables;

namespace DnDGen.TreasureGen.Generators.Items.Magical
{
    internal class SpellGenerator(ITreasurePercentileSelector percentileSelector) : ISpellGenerator
    {
        public string GenerateType()
        {
            return percentileSelector.SelectFrom(Config.Name, TableNameConstants.Percentiles.SpellTypes);
        }

        public int GenerateLevel(string power)
        {
            var tableName = TableNameConstants.Percentiles.POWERSpellLevels(power);
            var level = percentileSelector.SelectFrom<int>(Config.Name, tableName);
            return level;
        }

        public string Generate(string spellType, int level)
        {
            var tableName = TableNameConstants.Percentiles.LevelXSPELLTYPESpells(level, spellType);
            return percentileSelector.SelectFrom(Config.Name, tableName);
        }
    }
}