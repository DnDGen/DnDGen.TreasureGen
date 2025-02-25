using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.RollGen;
using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Selectors.Percentiles;
using DnDGen.TreasureGen.Tables;
using System;

namespace DnDGen.TreasureGen.Generators.Items.Magical
{
    internal class ChargesGenerator : IChargesGenerator
    {
        private readonly Dice dice;
        private readonly ICollectionTypeAndAmountSelector typeAndAmountSelector;
        private readonly ITreasurePercentileSelector percentileSelector;

        public ChargesGenerator(Dice dice, ICollectionTypeAndAmountSelector typeAndAmountSelector, ITreasurePercentileSelector percentileSelector)
        {
            this.dice = dice;
            this.typeAndAmountSelector = typeAndAmountSelector;
            this.percentileSelector = percentileSelector;
        }

        public int GenerateFor(string itemType, string name)
        {
            if (itemType == ItemTypeConstants.Wand || itemType == ItemTypeConstants.Staff)
                return PercentileCharges();

            if (name == WondrousItemConstants.DeckOfIllusions)
            {
                var isFullyCharged = percentileSelector.SelectFrom<bool>(Config.Name, TableNameConstants.Percentiles.IsDeckOfIllusionsFullyCharged);

                if (isFullyCharged)
                    name = WondrousItemConstants.DeckOfIllusions_Full;
            }

            var result = typeAndAmountSelector.SelectOneFrom(Config.Name, TableNameConstants.Collections.ChargeLimits, name);
            return result.Amount;
        }

        private int PercentileCharges()
        {
            var roll = dice.Roll().Percentile().AsSum();
            return Math.Max(roll / 2, 1);
        }
    }
}