﻿using System.Collections.Generic;
using TreasureGen.Domain.Selectors.Collections;
using TreasureGen.Domain.Selectors.Percentiles;
using TreasureGen.Domain.Tables;
using TreasureGen.Items;
using TreasureGen.Items.Magical;
using TreasureGen.Items.Mundane;

namespace TreasureGen.Domain.Generators.Items
{
    internal class ItemsGenerator : IItemsGenerator
    {
        private ITypeAndAmountPercentileSelector typeAndAmountPercentileSelector;
        private IPercentileSelector percentileSelector;
        private IMundaneItemGeneratorFactory mundaneItemGeneratorFactory;
        private IMagicalItemGeneratorFactory magicalItemGeneratorFactory;
        private IRangeDataSelector rangeDataSelector;

        public ItemsGenerator(ITypeAndAmountPercentileSelector typeAndAmountPercentileSelector, IMundaneItemGeneratorFactory mundaneItemGeneratorFactory, IPercentileSelector percentileSelector, IMagicalItemGeneratorFactory magicalItemGeneratorFactory, IRangeDataSelector rangeDataSelector)
        {
            this.typeAndAmountPercentileSelector = typeAndAmountPercentileSelector;
            this.mundaneItemGeneratorFactory = mundaneItemGeneratorFactory;
            this.percentileSelector = percentileSelector;
            this.magicalItemGeneratorFactory = magicalItemGeneratorFactory;
            this.rangeDataSelector = rangeDataSelector;
        }

        public IEnumerable<Item> GenerateAtLevel(int level)
        {
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.LevelXItems, level);
            var result = typeAndAmountPercentileSelector.SelectFrom(tableName);
            var items = new List<Item>();

            while (result.Amount-- > 0)
            {
                var item = GenerateAtPower(result.Type);
                items.Add(item);
            }

            var epicItems = GetEpicItems(level);
            items.AddRange(epicItems);

            return items;
        }

        private IEnumerable<Item> GetEpicItems(int level)
        {
            var epicItems = new List<Item>();
            var majorItemQuantity = rangeDataSelector.SelectFrom(TableNameConstants.Collections.Set.EpicItems, level.ToString()).Minimum;

            while (majorItemQuantity-- > 0)
            {
                var epicItem = GenerateAtPower(PowerConstants.Major);
                epicItems.Add(epicItem);
            }

            return epicItems;
        }

        private Item GenerateAtPower(string power)
        {
            if (power == PowerConstants.Mundane)
                return GenerateMundaneItem();

            return GenerateMagicalItemAtPower(power);
        }

        private Item GenerateMundaneItem()
        {
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERItems, PowerConstants.Mundane);
            var itemType = percentileSelector.SelectFrom(tableName);
            var generator = mundaneItemGeneratorFactory.CreateGeneratorOf(itemType);

            return generator.Generate();
        }

        private Item GenerateMagicalItemAtPower(string power)
        {
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERItems, power);
            var itemType = percentileSelector.SelectFrom(tableName);
            var magicalItemGenerator = magicalItemGeneratorFactory.CreateGeneratorOf(itemType);

            return magicalItemGenerator.GenerateAtPower(power);
        }
    }
}