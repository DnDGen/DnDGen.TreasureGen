using DnDGen.Infrastructure.Factories;
using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.Infrastructure.Selectors.Percentiles;
using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Items.Mundane;
using DnDGen.TreasureGen.Selectors.Percentiles;
using DnDGen.TreasureGen.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DnDGen.TreasureGen.Generators.Items
{
    internal class ItemsGenerator : IItemsGenerator
    {
        private readonly IPercentileTypeAndAmountSelector typeAndAmountPercentileSelector;
        private readonly ITreasurePercentileSelector percentileSelector;
        private readonly JustInTimeFactory justInTimeFactory;
        private readonly ICollectionSelector collectionSelector;
        private readonly ICollectionTypeAndAmountSelector typeAndAmountCollectionSelector;

        public ItemsGenerator(
            IPercentileTypeAndAmountSelector typeAndAmountPercentileSelector,
            JustInTimeFactory justInTimeFactory,
            ITreasurePercentileSelector percentileSelector,
            ICollectionSelector collectionSelector,
            ICollectionTypeAndAmountSelector typeAndAmountCollectionSelector)
        {
            this.typeAndAmountPercentileSelector = typeAndAmountPercentileSelector;
            this.justInTimeFactory = justInTimeFactory;
            this.percentileSelector = percentileSelector;
            this.collectionSelector = collectionSelector;
            this.typeAndAmountCollectionSelector = typeAndAmountCollectionSelector;
        }

        public IEnumerable<Item> GenerateRandomAtLevel(int level)
        {
            var percentileLevel = GetValidPercentileLevel(level);
            var extraItemsLevel = GetValidExtraItemLevel(level);

            var percentileResult = typeAndAmountPercentileSelector.SelectFrom(Config.Name, TableNameConstants.Percentiles.LevelXItems(percentileLevel));
            var items = new List<Item>();

            while (items.Count < percentileResult.Amount)
            {
                var item = GenerateRandomAtPower(percentileResult.Type);
                items.Add(item);
            }

            var extraItemsResult = typeAndAmountCollectionSelector.SelectOneFrom(Config.Name, TableNameConstants.Collections.ExtraItems, extraItemsLevel.ToString());

            while (items.Count < percentileResult.Amount + extraItemsResult.Amount)
            {
                var item = GenerateRandomAtPower(extraItemsResult.Type);
                items.Add(item);
            }

            return items;
        }

        private int GetValidPercentileLevel(int level)
        {
            if (level < LevelLimits.Minimum)
                throw new ArgumentException($"Level {level} is not a valid level for treasure generation");

            if (level > LevelLimits.Maximum_Standard)
                return LevelLimits.Maximum_Standard;

            return level;
        }

        private int GetValidExtraItemLevel(int level)
        {
            if (level < LevelLimits.Minimum)
                throw new ArgumentException($"Level {level} is not a valid level for treasure generation");

            if (level > LevelLimits.Maximum_Epic)
                return LevelLimits.Maximum_Epic;

            return level;
        }

        public async Task<IEnumerable<Item>> GenerateRandomAtLevelAsync(int level)
        {
            var percentileLevel = GetValidPercentileLevel(level);
            var extraItemsLevel = GetValidExtraItemLevel(level);

            var percentileResult = typeAndAmountPercentileSelector.SelectFrom(Config.Name, TableNameConstants.Percentiles.LevelXItems(percentileLevel));
            var tasks = new List<Task<Item>>();

            while (tasks.Count < percentileResult.Amount)
            {
                var task = Task.Run(() => GenerateRandomAtPower(percentileResult.Type));
                tasks.Add(task);
            }

            var extraItemsResult = typeAndAmountCollectionSelector.SelectOneFrom(Config.Name, TableNameConstants.Collections.ExtraItems, extraItemsLevel.ToString());

            while (tasks.Count < percentileResult.Amount + extraItemsResult.Amount)
            {
                var task = Task.Run(() => GenerateRandomAtPower(extraItemsResult.Type));
                tasks.Add(task);
            }

            await Task.WhenAll(tasks);

            return tasks.Select(t => t.Result);
        }

        private Item GenerateRandomAtPower(string power)
        {
            if (power == PowerConstants.Mundane)
                return GenerateRandomMundaneItem();

            return GenerateRandomMagicalItemAtPower(power);
        }

        private Item GenerateRandomMundaneItem()
        {
            var itemType = percentileSelector.SelectFrom(Config.Name, TableNameConstants.Percentiles.POWERItems(PowerConstants.Mundane));

            return GenerateMundaneItem(itemType);
        }

        private Item GenerateRandomMagicalItemAtPower(string power)
        {
            var itemType = percentileSelector.SelectFrom(Config.Name, TableNameConstants.Percentiles.POWERItems(power));

            return GenerateMagicalItemAtPower(power, itemType);
        }

        public Item GenerateAtLevel(int level, string itemType, string itemName, params string[] traits)
        {
            var percentileLevel = GetValidPercentileLevel(level);
            var result = typeAndAmountPercentileSelector.SelectFrom(Config.Name, TableNameConstants.Percentiles.LevelXItems(percentileLevel));
            var powers = collectionSelector.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, itemType);

            if (itemType != ItemTypeConstants.Scroll && itemType != ItemTypeConstants.Wand)
                powers = collectionSelector.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, itemName);

            var power = PowerHelper.AdjustPower(result.Type, powers);
            if (power == PowerConstants.Mundane)
                return GenerateMundaneItem(itemType, itemName, traits);

            return GenerateMagicalItemAtPower(power, itemType, itemName, traits);
        }

        public async Task<Item> GenerateAtLevelAsync(int level, string itemType, string itemName, params string[] traits) =>
            await Task.Run(() => GenerateAtLevel(level, itemType, itemName, traits));

        private Item GenerateMundaneItem(string itemType, string itemName = null, params string[] traits)
        {
            var generator = justInTimeFactory.Build<MundaneItemGenerator>(itemType);

            if (string.IsNullOrEmpty(itemName))
                return generator.GenerateRandom();

            return generator.Generate(itemName, traits);
        }

        private Item GenerateMagicalItemAtPower(string power, string itemType, string itemName = null, params string[] traits)
        {
            var magicalItemGenerator = justInTimeFactory.Build<MagicalItemGenerator>(itemType);

            if (string.IsNullOrEmpty(itemName))
                return magicalItemGenerator.GenerateRandom(power);

            return magicalItemGenerator.Generate(power, itemName, traits);
        }
    }
}