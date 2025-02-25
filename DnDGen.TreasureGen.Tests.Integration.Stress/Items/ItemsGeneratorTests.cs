using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Items.Mundane;
using DnDGen.TreasureGen.Tests.Unit.Generators.Items;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DnDGen.TreasureGen.Tests.Integration.Stress.Items
{
    [TestFixture]
    public class ItemsGeneratorTests : StressTests
    {
        private IItemsGenerator itemsGenerator;
        private ItemVerifier itemVerifier;

        [SetUp]
        public void Setup()
        {
            itemsGenerator = GetNewInstanceOf<IItemsGenerator>();
            itemVerifier = new ItemVerifier();
        }

        [Test]
        public void StressRandomItems()
        {
            stressor.Stress(GenerateAndAssertRandomItems);
        }

        private void GenerateAndAssertRandomItems()
        {
            var level = GetNewLevel();
            var items = itemsGenerator.GenerateRandomAtLevel(level);

            Assert.That(items, Is.Not.Null);

            if (level > 20)
                Assert.That(items, Is.Not.Empty, $"Level {level}");

            foreach (var item in items)
                itemVerifier.AssertItem(item);
        }

        [Test]
        public async Task StressRandomItemsAsync()
        {
            await stressor.StressAsync(GenerateAndAssertRandomItemsAsync);
        }

        private async Task GenerateAndAssertRandomItemsAsync()
        {
            var level = GetNewLevel();
            var items = await itemsGenerator.GenerateRandomAtLevelAsync(level);

            Assert.That(items, Is.Not.Null);

            if (level > 20)
                Assert.That(items, Is.Not.Empty, $"Level {level}");

            foreach (var item in items)
                itemVerifier.AssertItem(item);
        }

        [Test]
        public void StressNamedItemAtLevel()
        {
            stressor.Stress(GenerateAndAssertNamedItemAtLevelItems);
        }

        private void GenerateAndAssertNamedItemAtLevelItems()
        {
            var level = GetNewLevel();
            var itemType = GetRandomItemType();
            var itemName = GetRandomItemName(itemType);

            var item = itemsGenerator.GenerateAtLevel(level, itemType, itemName);
            itemVerifier.AssertItem(item);
        }

        private string GetRandomItemType()
        {
            var itemTypes = new[]
            {
                ItemTypeConstants.AlchemicalItem,
                ItemTypeConstants.Armor,
                ItemTypeConstants.Potion,
                ItemTypeConstants.Ring,
                ItemTypeConstants.Rod,
                ItemTypeConstants.Scroll,
                ItemTypeConstants.Staff,
                ItemTypeConstants.Tool,
                ItemTypeConstants.Wand,
                ItemTypeConstants.Weapon,
                ItemTypeConstants.WondrousItem,
            };

            var itemType = collectionSelector.SelectRandomFrom(itemTypes);
            return itemType;
        }

        private string GetRandomItemName(string itemType)
        {
            var itemNames = GetItemNames(itemType);
            var itemName = collectionSelector.SelectRandomFrom(itemNames);
            return itemName;
        }

        private IEnumerable<string> GetItemNames(string itemType)
        {
            return itemType switch
            {
                ItemTypeConstants.AlchemicalItem => AlchemicalItemConstants.GetAllAlchemicalItems(),
                ItemTypeConstants.Armor => ArmorConstants.GetAllArmorsAndShields(true),
                ItemTypeConstants.Potion => PotionConstants.GetAllPotions(true),
                ItemTypeConstants.Ring => RingConstants.GetAllRings(),
                ItemTypeConstants.Rod => RodConstants.GetAllRods(),
                ItemTypeConstants.Scroll => [$"Scroll {Guid.NewGuid()}"],
                ItemTypeConstants.Staff => StaffConstants.GetAllStaffs(),
                ItemTypeConstants.Tool => ToolConstants.GetAllTools(),
                ItemTypeConstants.Wand => [$"Wand {Guid.NewGuid()}"],
                ItemTypeConstants.Weapon => WeaponConstants.GetAllWeapons(true, true),
                ItemTypeConstants.WondrousItem => WondrousItemConstants.GetAllWondrousItems(),
                _ => throw new ArgumentException($"{itemType} is not a valid Item Type"),
            };
        }

        [Test]
        public async Task StressNamedItemAtLevelAsync()
        {
            await stressor.StressAsync(GenerateAndAssertNamedItemAtLevelAsync);
        }

        private async Task GenerateAndAssertNamedItemAtLevelAsync()
        {
            var level = GetNewLevel();
            var itemType = GetRandomItemType();
            var itemName = GetRandomItemName(itemType);

            var item = await itemsGenerator.GenerateAtLevelAsync(level, itemType, itemName);
            itemVerifier.AssertItem(item);
        }
    }
}