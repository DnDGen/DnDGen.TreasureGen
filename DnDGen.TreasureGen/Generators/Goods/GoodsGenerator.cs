using DnDGen.Infrastructure.Models;
using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.Infrastructure.Selectors.Percentiles;
using DnDGen.TreasureGen.Goods;
using DnDGen.TreasureGen.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DnDGen.TreasureGen.Generators.Goods
{
    internal class GoodsGenerator : IGoodsGenerator
    {
        private readonly IPercentileTypeAndAmountSelector typeAndAmountPercentileSelector;
        private readonly ICollectionSelector collectionSelector;

        public GoodsGenerator(IPercentileTypeAndAmountSelector typeAndAmountPercentileSelector, ICollectionSelector collectionSelector)
        {
            this.typeAndAmountPercentileSelector = typeAndAmountPercentileSelector;
            this.collectionSelector = collectionSelector;
        }

        public IEnumerable<Good> GenerateAtLevel(int level)
        {
            var validLevel = GetValidLevel(level);
            var typeAndAmountSelection = typeAndAmountPercentileSelector.SelectFrom(Config.Name, TableNameConstants.Percentiles.LevelXGoods(validLevel));
            var goods = new List<Good>();

            while (goods.Count < typeAndAmountSelection.Amount)
            {
                var good = GenerateGood(typeAndAmountSelection);
                goods.Add(good);
            }

            return goods;
        }

        private int GetValidLevel(int level)
        {
            if (level < LevelLimits.Minimum)
                throw new ArgumentException($"Level {level} is not a valid level for treasure generation");

            if (level > LevelLimits.Maximum_Standard)
                return LevelLimits.Maximum_Standard;

            return level;
        }

        private Good GenerateGood(TypeAndAmountDataSelection quantity)
        {
            var valueSelection = typeAndAmountPercentileSelector.SelectFrom(Config.Name, TableNameConstants.Percentiles.GOODTYPEValues(quantity.Type));

            var good = new Good
            {
                Description = collectionSelector.SelectRandomFrom(Config.Name, TableNameConstants.Collections.GOODTYPEDescriptions(quantity.Type), valueSelection.Type),
                ValueInGold = valueSelection.Amount
            };

            return good;
        }

        public async Task<IEnumerable<Good>> GenerateAtLevelAsync(int level)
        {
            var validLevel = GetValidLevel(level);
            var typeAndAmountSelection = typeAndAmountPercentileSelector.SelectFrom(Config.Name, TableNameConstants.Percentiles.LevelXGoods(validLevel));
            var tasks = new List<Task<Good>>();

            while (tasks.Count < typeAndAmountSelection.Amount)
            {
                var task = Task.Run(() => GenerateGood(typeAndAmountSelection));
                tasks.Add(task);
            }

            await Task.WhenAll(tasks);

            return tasks.Select(t => t.Result);
        }
    }
}