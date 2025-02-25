using DnDGen.Infrastructure.Selectors.Percentiles;
using DnDGen.TreasureGen.Coins;
using DnDGen.TreasureGen.Tables;
using System;

namespace DnDGen.TreasureGen.Generators.Coins
{
    internal class CoinGenerator : ICoinGenerator
    {
        private readonly IPercentileTypeAndAmountSelector typeAndAmountPercentileSelector;

        public CoinGenerator(IPercentileTypeAndAmountSelector typeAndAmountPercentileSelector)
        {
            this.typeAndAmountPercentileSelector = typeAndAmountPercentileSelector;
        }

        public Coin GenerateAtLevel(int level)
        {
            if (level < LevelLimits.Minimum)
                throw new ArgumentException($"Level {level} is not a valid level for treasure generation");

            if (level > LevelLimits.Maximum_Standard)
                level = LevelLimits.Maximum_Standard;

            var result = typeAndAmountPercentileSelector.SelectFrom(Config.Name, TableNameConstants.Percentiles.LevelXCoins(level));
            var coin = new Coin
            {
                Currency = result.Type,
                Quantity = result.Amount
            };

            return coin;
        }
    }
}