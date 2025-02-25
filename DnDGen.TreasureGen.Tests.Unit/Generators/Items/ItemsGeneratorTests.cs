using DnDGen.Infrastructure.Factories;
using DnDGen.Infrastructure.Models;
using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.Infrastructure.Selectors.Percentiles;
using DnDGen.TreasureGen.Generators.Items;
using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Items.Mundane;
using DnDGen.TreasureGen.Selectors.Percentiles;
using DnDGen.TreasureGen.Tables;
using Moq;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace DnDGen.TreasureGen.Tests.Unit.Generators.Items
{
    [TestFixture]
    public class ItemsGeneratorTests
    {
        private Mock<IPercentileTypeAndAmountSelector> mockTypeAndAmountPercentileSelector;
        private Mock<ICollectionTypeAndAmountSelector> mockTypeAndAmountCollectionSelector;
        private Mock<JustInTimeFactory> mockJustInTimeFactory;
        private Mock<MundaneItemGenerator> mockMundaneItemGenerator;
        private Mock<ITreasurePercentileSelector> mockPercentileSelector;
        private Mock<MagicalItemGenerator> mockMagicalItemGenerator;
        private IItemsGenerator itemsGenerator;
        private TypeAndAmountDataSelection percentileSelection;
        private TypeAndAmountDataSelection extraSelection;
        private Mock<ICollectionSelector> mockCollectionSelector;

        [SetUp]
        public void Setup()
        {
            mockJustInTimeFactory = new Mock<JustInTimeFactory>();
            mockPercentileSelector = new Mock<ITreasurePercentileSelector>();
            mockMagicalItemGenerator = new Mock<MagicalItemGenerator>();
            mockTypeAndAmountPercentileSelector = new Mock<IPercentileTypeAndAmountSelector>();
            mockTypeAndAmountCollectionSelector = new Mock<ICollectionTypeAndAmountSelector>();
            mockMundaneItemGenerator = new Mock<MundaneItemGenerator>();
            mockCollectionSelector = new Mock<ICollectionSelector>();

            itemsGenerator = new ItemsGenerator(
                mockTypeAndAmountPercentileSelector.Object,
                mockJustInTimeFactory.Object,
                mockPercentileSelector.Object,
                mockCollectionSelector.Object,
                mockTypeAndAmountCollectionSelector.Object);

            percentileSelection = new TypeAndAmountDataSelection
            {
                Type = "power",
                AmountAsDouble = 42
            };
            extraSelection = new TypeAndAmountDataSelection
            {
                Type = string.Empty,
                AmountAsDouble = 0
            };
            mockTypeAndAmountPercentileSelector.Setup(p => p.SelectFrom(Config.Name, It.IsAny<string>())).Returns(percentileSelection);
            mockTypeAndAmountCollectionSelector.Setup(p => p.SelectOneFrom(Config.Name, It.IsAny<string>(), It.IsAny<string>())).Returns(extraSelection);
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, It.IsAny<string>())).Returns(ItemTypeConstants.WondrousItem);

            var dummyMagicalMock = new Mock<MagicalItemGenerator>();
            dummyMagicalMock.Setup(m => m.GenerateRandom(It.IsAny<string>())).Returns(() => new Item { Name = "magical item" });
            mockJustInTimeFactory.Setup(f => f.Build<MagicalItemGenerator>(It.IsAny<string>())).Returns(dummyMagicalMock.Object);

            var dummyMundaneMock = new Mock<MundaneItemGenerator>();
            dummyMundaneMock.Setup(m => m.GenerateRandom()).Returns(() => new Item { Name = "mundane item" });
            mockJustInTimeFactory.Setup(f => f.Build<MundaneItemGenerator>(It.IsAny<string>())).Returns(dummyMundaneMock.Object);
        }

        [Test]
        public void GenerateRandomAtLevel_ThrowsException_LevelTooLow()
        {
            Assert.That(() => itemsGenerator.GenerateRandomAtLevel(LevelLimits.Minimum - 1),
                Throws.ArgumentException.With.Message.EqualTo("Level 0 is not a valid level for treasure generation"));
        }

        [TestCase(LevelLimits.Minimum)]
        [TestCase(LevelLimits.Minimum + 1)]
        [TestCase(10)]
        [TestCase(LevelLimits.Maximum_Standard - 1)]
        [TestCase(LevelLimits.Maximum_Standard)]
        public void GenerateRandomAtLevel_ItemsAreGenerated(int level)
        {
            var items = itemsGenerator.GenerateRandomAtLevel(level);
            Assert.That(items, Is.Not.Null);
            var expectedTableName = TableNameConstants.Percentiles.LevelXItems(level);
            mockTypeAndAmountPercentileSelector.Verify(p => p.SelectFrom(Config.Name, expectedTableName), Times.Once);
        }

        [TestCase(LevelLimits.Maximum_Standard + 1)]
        [TestCase(9266)]
        public void GenerateRandomAtLevel_ItemsAreGenerated_StandardMax(int level)
        {
            var items = itemsGenerator.GenerateRandomAtLevel(level);
            Assert.That(items, Is.Not.Null);
            var expectedTableName = TableNameConstants.Percentiles.LevelXItems(LevelLimits.Maximum_Standard);
            mockTypeAndAmountPercentileSelector.Verify(p => p.SelectFrom(Config.Name, expectedTableName), Times.Once);
        }

        [TestCase(LevelLimits.Minimum)]
        [TestCase(LevelLimits.Minimum + 1)]
        [TestCase(10)]
        [TestCase(LevelLimits.Maximum_Standard - 1)]
        [TestCase(LevelLimits.Maximum_Standard)]
        [TestCase(LevelLimits.Maximum_Standard + 1)]
        [TestCase(LevelLimits.Maximum_Epic - 1)]
        [TestCase(LevelLimits.Maximum_Epic)]
        public void GenerateRandomAtLevel_BonusItemsAreGenerated(int level)
        {
            var items = itemsGenerator.GenerateRandomAtLevel(level);
            Assert.That(items, Is.Not.Null);
            mockTypeAndAmountCollectionSelector.Verify(p => p.SelectOneFrom(Config.Name, TableNameConstants.Collections.ExtraItems, level.ToString()), Times.Once);
        }

        [TestCase(LevelLimits.Maximum_Epic + 1)]
        [TestCase(9266)]
        public void GenerateRandomAtLevel_BonusItemsAreGenerated_EpicMax(int level)
        {
            var items = itemsGenerator.GenerateRandomAtLevel(level);
            Assert.That(items, Is.Not.Null); mockTypeAndAmountCollectionSelector.Verify(p => p.SelectOneFrom(Config.Name, TableNameConstants.Collections.ExtraItems, LevelLimits.Maximum_Epic.ToString()), Times.Once);
        }

        [Test]
        public void GenerateRandomAtLevel_GetAmountFromRoll()
        {
            var items = itemsGenerator.GenerateRandomAtLevel(1);
            Assert.That(items.Count(), Is.EqualTo(42));
        }

        [Test]
        public void GenerateRandomAtLevel_ReturnItems()
        {
            percentileSelection.Type = PowerConstants.Mundane;
            percentileSelection.AmountAsDouble = 2;
            extraSelection.Type = string.Empty;
            extraSelection.AmountAsDouble = 0;

            var firstItem = new Item();
            var secondItem = new Item();
            mockJustInTimeFactory.Setup(f => f.Build<MundaneItemGenerator>(It.IsAny<string>())).Returns(mockMundaneItemGenerator.Object);
            mockMundaneItemGenerator.SetupSequence(f => f.GenerateRandom()).Returns(firstItem).Returns(secondItem);

            var items = itemsGenerator.GenerateRandomAtLevel(1);
            Assert.That(items, Contains.Item(firstItem));
            Assert.That(items, Contains.Item(secondItem));
            Assert.That(items.Count(), Is.EqualTo(2));
        }

        [Test]
        public void GenerateRandomAtLevel_ReturnBonusItems()
        {
            percentileSelection.Type = string.Empty;
            percentileSelection.AmountAsDouble = 0;
            extraSelection.Type = PowerConstants.Mundane;
            extraSelection.AmountAsDouble = 2;

            var firstItem = new Item();
            var secondItem = new Item();
            mockJustInTimeFactory.Setup(f => f.Build<MundaneItemGenerator>(It.IsAny<string>())).Returns(mockMundaneItemGenerator.Object);
            mockMundaneItemGenerator.SetupSequence(f => f.GenerateRandom()).Returns(firstItem).Returns(secondItem);

            var items = itemsGenerator.GenerateRandomAtLevel(1);
            Assert.That(items, Contains.Item(firstItem));
            Assert.That(items, Contains.Item(secondItem));
            Assert.That(items.Count(), Is.EqualTo(2));
        }

        [Test]
        public void GenerateRandomAtLevel_ReturnItemsAndBonusItems()
        {
            percentileSelection.Type = PowerConstants.Mundane;
            percentileSelection.AmountAsDouble = 2;
            extraSelection.Type = "power";
            extraSelection.AmountAsDouble = 3;

            var itemCount = 0;
            mockJustInTimeFactory.Setup(f => f.Build<MundaneItemGenerator>(It.IsAny<string>())).Returns(mockMundaneItemGenerator.Object);
            mockMundaneItemGenerator.Setup(f => f.GenerateRandom()).Returns(() => new Item { Name = $"mundane item {itemCount++}" });

            var expectedTableName = TableNameConstants.Percentiles.POWERItems(percentileSelection.Type);
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, expectedTableName)).Returns("magic item type");
            mockJustInTimeFactory.Setup(f => f.Build<MagicalItemGenerator>("magic item type")).Returns(mockMagicalItemGenerator.Object);
            mockMagicalItemGenerator.Setup(g => g.GenerateRandom(percentileSelection.Type)).Returns(() => new Item { Name = $"magical item {itemCount++}" });

            var items = itemsGenerator.GenerateRandomAtLevel(1);
            Assert.That(items, Is.All.Not.Null);
            Assert.That(items.Count(), Is.EqualTo(5));
        }

        [Test]
        public void GenerateRandomAtLevel_IfSelectorReturnsEmptyResult_ItemsGeneratorReturnsEmptyEnumerable()
        {
            percentileSelection.Type = string.Empty;
            percentileSelection.AmountAsDouble = 0;
            extraSelection.Type = string.Empty;
            extraSelection.AmountAsDouble = 0;

            var items = itemsGenerator.GenerateRandomAtLevel(1);
            Assert.That(items, Is.Empty);
        }

        [Test]
        public void GenerateRandomAtLevel_GetMundaneItems()
        {
            percentileSelection.Type = PowerConstants.Mundane;
            percentileSelection.AmountAsDouble = 1;
            extraSelection.Type = string.Empty;
            extraSelection.AmountAsDouble = 0;

            var mundaneItem = new Item();
            var expectedTableName = TableNameConstants.Percentiles.POWERItems(percentileSelection.Type);
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, expectedTableName)).Returns("mundane item type");
            mockJustInTimeFactory.Setup(f => f.Build<MundaneItemGenerator>("mundane item type")).Returns(mockMundaneItemGenerator.Object);
            mockMundaneItemGenerator.Setup(g => g.GenerateRandom()).Returns(mundaneItem);

            var items = itemsGenerator.GenerateRandomAtLevel(1);
            Assert.That(items.Single(), Is.EqualTo(mundaneItem));
        }

        [Test]
        public void GenerateRandomAtLevel_GetMagicalItems()
        {
            percentileSelection.Type = "power";
            percentileSelection.AmountAsDouble = 1;
            extraSelection.Type = string.Empty;
            extraSelection.AmountAsDouble = 0;

            var magicalItem = new Item();
            var expectedTableName = TableNameConstants.Percentiles.POWERItems(percentileSelection.Type);
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, expectedTableName)).Returns("magic item type");
            mockJustInTimeFactory.Setup(f => f.Build<MagicalItemGenerator>("magic item type")).Returns(mockMagicalItemGenerator.Object);
            mockMagicalItemGenerator.Setup(g => g.GenerateRandom(percentileSelection.Type)).Returns(magicalItem);

            var items = itemsGenerator.GenerateRandomAtLevel(1);
            Assert.That(items.Single(), Is.EqualTo(magicalItem));
        }

        [Test]
        public async Task GenerateRandomAtLevelAsync_ItemsAreGenerated()
        {
            var items = await itemsGenerator.GenerateRandomAtLevelAsync(1);
            Assert.That(items, Is.Not.Null);
        }

        [Test]
        public async Task GenerateRandomAtLevelAsync_GetItemTypeFromSelector()
        {
            await itemsGenerator.GenerateRandomAtLevelAsync(9);
            var expectedTableName = TableNameConstants.Percentiles.LevelXItems(9);
            mockTypeAndAmountPercentileSelector.Verify(p => p.SelectFrom(Config.Name, expectedTableName), Times.Once);
        }

        [Test]
        public async Task GenerateRandomAtLevelAsync_GetAmountFromRoll()
        {
            var items = await itemsGenerator.GenerateRandomAtLevelAsync(1);
            Assert.That(items.Count(), Is.EqualTo(42));
        }

        [Test]
        public async Task GenerateRandomAtLevelAsync_ReturnItems()
        {
            percentileSelection.Type = PowerConstants.Mundane;
            percentileSelection.AmountAsDouble = 2;
            extraSelection.Type = string.Empty;
            extraSelection.AmountAsDouble = 0;

            var firstItem = new Item();
            var secondItem = new Item();
            mockJustInTimeFactory.Setup(f => f.Build<MundaneItemGenerator>(It.IsAny<string>())).Returns(mockMundaneItemGenerator.Object);
            mockMundaneItemGenerator
                .SetupSequence(f => f.GenerateRandom())
                .Returns(firstItem)
                .Returns(secondItem);

            var items = await itemsGenerator.GenerateRandomAtLevelAsync(1);
            Assert.That(items, Contains.Item(firstItem));
            Assert.That(items, Contains.Item(secondItem));
            Assert.That(items.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task GenerateRandomAtLevelAsync_ReturnBonusItems()
        {
            percentileSelection.Type = string.Empty;
            percentileSelection.AmountAsDouble = 0;
            extraSelection.Type = PowerConstants.Mundane;
            extraSelection.AmountAsDouble = 2;

            var firstItem = new Item();
            var secondItem = new Item();
            mockJustInTimeFactory.Setup(f => f.Build<MundaneItemGenerator>(It.IsAny<string>())).Returns(mockMundaneItemGenerator.Object);
            mockMundaneItemGenerator
                .SetupSequence(f => f.GenerateRandom())
                .Returns(firstItem)
                .Returns(secondItem);

            var items = await itemsGenerator.GenerateRandomAtLevelAsync(1);
            Assert.That(items, Contains.Item(firstItem));
            Assert.That(items, Contains.Item(secondItem));
            Assert.That(items.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task GenerateRandomAtLevelAsync_ReturnItemsAndBonusItems()
        {
            percentileSelection.Type = PowerConstants.Mundane;
            percentileSelection.AmountAsDouble = 2;
            extraSelection.Type = "power";
            extraSelection.AmountAsDouble = 3;

            var itemCount = 0;
            mockJustInTimeFactory.Setup(f => f.Build<MundaneItemGenerator>(It.IsAny<string>())).Returns(mockMundaneItemGenerator.Object);
            mockMundaneItemGenerator.Setup(f => f.GenerateRandom()).Returns(() => new Item { Name = $"mundane item {itemCount++}" });

            var expectedTableName = TableNameConstants.Percentiles.POWERItems(percentileSelection.Type);
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, expectedTableName)).Returns("magic item type");
            mockJustInTimeFactory.Setup(f => f.Build<MagicalItemGenerator>("magic item type")).Returns(mockMagicalItemGenerator.Object);
            mockMagicalItemGenerator.Setup(g => g.GenerateRandom(percentileSelection.Type)).Returns(() => new Item { Name = $"magical item {itemCount++}" });

            var items = await itemsGenerator.GenerateRandomAtLevelAsync(1);
            Assert.That(items, Is.All.Not.Null);
            Assert.That(items.Count(), Is.EqualTo(5));
        }

        [Test]
        public async Task GenerateRandomAtLevelAsync_IfSelectorReturnsEmptyResult_ItemsGeneratorReturnsEmptyEnumerable()
        {
            percentileSelection.Type = string.Empty;
            percentileSelection.AmountAsDouble = 0;
            extraSelection.Type = string.Empty;
            extraSelection.AmountAsDouble = 0;

            var items = await itemsGenerator.GenerateRandomAtLevelAsync(1);
            Assert.That(items, Is.Empty);
        }

        [Test]
        public async Task GenerateRandomAtLevelAsync_GetMundaneItems()
        {
            percentileSelection.Type = PowerConstants.Mundane;
            percentileSelection.AmountAsDouble = 1;
            extraSelection.Type = string.Empty;
            extraSelection.AmountAsDouble = 0;

            var mundaneItem = new Item();
            var expectedTableName = TableNameConstants.Percentiles.POWERItems(percentileSelection.Type);
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, expectedTableName)).Returns("mundane item type");
            mockJustInTimeFactory.Setup(f => f.Build<MundaneItemGenerator>("mundane item type")).Returns(mockMundaneItemGenerator.Object);
            mockMundaneItemGenerator.Setup(g => g.GenerateRandom()).Returns(mundaneItem);

            var items = await itemsGenerator.GenerateRandomAtLevelAsync(1);
            Assert.That(items.Single(), Is.EqualTo(mundaneItem));
        }

        [Test]
        public async Task GenerateRandomAtLevelAsync_GetMagicalItems()
        {
            percentileSelection.Type = "power";
            percentileSelection.AmountAsDouble = 1;
            extraSelection.Type = string.Empty;
            extraSelection.AmountAsDouble = 0;

            var magicalItem = new Item();
            var expectedTableName = TableNameConstants.Percentiles.POWERItems(percentileSelection.Type);
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, expectedTableName)).Returns("magic item type");
            mockJustInTimeFactory.Setup(f => f.Build<MagicalItemGenerator>("magic item type")).Returns(mockMagicalItemGenerator.Object);
            mockMagicalItemGenerator.Setup(g => g.GenerateRandom(percentileSelection.Type)).Returns(magicalItem);

            var items = await itemsGenerator.GenerateRandomAtLevelAsync(1);
            Assert.That(items.Single(), Is.EqualTo(magicalItem));
        }

        [Test]
        public void GenerateAtLevel_Named_GetMundaneItem()
        {
            percentileSelection.Type = PowerConstants.Mundane;
            extraSelection.Type = string.Empty;
            extraSelection.AmountAsDouble = 0;

            mockCollectionSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, "item name"))
                .Returns(new[] { PowerConstants.Mundane, "power", "more power", "wrong power" });

            var firstItem = new Item();
            mockJustInTimeFactory.Setup(f => f.Build<MundaneItemGenerator>("item type")).Returns(mockMundaneItemGenerator.Object);
            mockMundaneItemGenerator.Setup(f => f.Generate("item name")).Returns(firstItem);

            var item = itemsGenerator.GenerateAtLevel(1, "item type", "item name");
            Assert.That(item, Is.EqualTo(firstItem));
        }

        [Test]
        public void BUG_GenerateAtLevel_Named_GetMundaneItem_WhenNoPowerSpecified()
        {
            percentileSelection.Type = string.Empty;
            extraSelection.Type = string.Empty;
            extraSelection.AmountAsDouble = 0;

            mockCollectionSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, "item name"))
                .Returns(new[] { PowerConstants.Mundane, "power", "more power", "wrong power" });

            var firstItem = new Item();
            mockJustInTimeFactory.Setup(f => f.Build<MundaneItemGenerator>("item type")).Returns(mockMundaneItemGenerator.Object);
            mockMundaneItemGenerator.Setup(f => f.Generate("item name")).Returns(firstItem);

            var item = itemsGenerator.GenerateAtLevel(1, "item type", "item name");
            Assert.That(item, Is.EqualTo(firstItem));
        }

        [Test]
        public void BUG_GenerateAtLevel_Named_GetSpecificItem_WhenMundaneSpecified()
        {
            percentileSelection.Type = PowerConstants.Mundane;
            extraSelection.Type = string.Empty;
            extraSelection.AmountAsDouble = 0;

            mockCollectionSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, "specific item"))
                .Returns(new[] { "power", "more power", "wrong power" });

            var magicalItem = new Item();
            mockJustInTimeFactory.Setup(f => f.Build<MagicalItemGenerator>("item type")).Returns(mockMagicalItemGenerator.Object);
            mockMagicalItemGenerator.Setup(f => f.Generate("power", "specific item")).Returns(magicalItem);

            var item = itemsGenerator.GenerateAtLevel(1, "item type", "specific item");
            Assert.That(item, Is.EqualTo(magicalItem));
        }

        [Test]
        public void BUG_GenerateAtLevel_Named_GetSpecificItem_WhenNoneSpecified()
        {
            percentileSelection.Type = string.Empty;
            extraSelection.Type = string.Empty;
            extraSelection.AmountAsDouble = 0;

            mockCollectionSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, "specific item"))
                .Returns(new[] { "power", "more power", "wrong power" });

            var magicalItem = new Item();
            mockJustInTimeFactory.Setup(f => f.Build<MagicalItemGenerator>("item type")).Returns(mockMagicalItemGenerator.Object);
            mockMagicalItemGenerator.Setup(f => f.Generate("power", "specific item")).Returns(magicalItem);

            var item = itemsGenerator.GenerateAtLevel(1, "item type", "specific item");
            Assert.That(item, Is.EqualTo(magicalItem));
        }

        [Test]
        public void BUG_GenerateAtLevel_Named_GetMagicalItem_WhenNoPowerSpecified()
        {
            percentileSelection.Type = string.Empty;
            extraSelection.Type = string.Empty;
            extraSelection.AmountAsDouble = 0;

            mockCollectionSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, "item name"))
                .Returns(new[] { "power", "more power", "wrong power" });

            var magicalItem = new Item();
            mockJustInTimeFactory.Setup(f => f.Build<MagicalItemGenerator>("item type")).Returns(mockMagicalItemGenerator.Object);
            mockMagicalItemGenerator.Setup(g => g.Generate("power", "item name")).Returns(magicalItem);

            var item = itemsGenerator.GenerateAtLevel(1, "item type", "item name");
            Assert.That(item, Is.EqualTo(magicalItem));
        }

        [Test]
        public void BUG_GenerateAtLevel_Named_GetMundaneItem_WhenPowerIsMagical_ButItemTypeIsMundane()
        {
            percentileSelection.Type = "power";
            extraSelection.Type = string.Empty;
            extraSelection.AmountAsDouble = 0;

            mockCollectionSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, "item name"))
                .Returns(new[] { PowerConstants.Mundane });

            var firstItem = new Item();
            mockJustInTimeFactory.Setup(f => f.Build<MundaneItemGenerator>("item type")).Returns(mockMundaneItemGenerator.Object);
            mockMundaneItemGenerator.Setup(f => f.Generate("item name")).Returns(firstItem);

            var item = itemsGenerator.GenerateAtLevel(1, "item type", "item name");
            Assert.That(item, Is.EqualTo(firstItem));
        }

        [Test]
        public void BUG_GenerateAtLevel_Named_GetMagicalItem_WhenPowerIsMundane_ButItemTypeIsMagical()
        {
            percentileSelection.Type = PowerConstants.Mundane;
            extraSelection.Type = string.Empty;
            extraSelection.AmountAsDouble = 0;

            mockCollectionSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, "item name"))
                .Returns(new[] { "power", "more power", "wrong power" });

            var magicalItem = new Item();
            mockJustInTimeFactory.Setup(f => f.Build<MagicalItemGenerator>("item type")).Returns(mockMagicalItemGenerator.Object);
            mockMagicalItemGenerator.Setup(g => g.Generate("power", "item name")).Returns(magicalItem);

            var item = itemsGenerator.GenerateAtLevel(1, "item type", "item name");
            Assert.That(item, Is.EqualTo(magicalItem));
        }

        [Test]
        public void BUG_GenerateAtLevel_Named_GetMagicalItem_WhenPowerIsMagical_ButItemTypeIsHigherMagical()
        {
            percentileSelection.Type = "power";
            extraSelection.Type = string.Empty;
            extraSelection.AmountAsDouble = 0;

            mockCollectionSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, "item name"))
                .Returns(new[] { "more power", "wrong power" });

            var magicalItem = new Item();
            mockJustInTimeFactory.Setup(f => f.Build<MagicalItemGenerator>("item type")).Returns(mockMagicalItemGenerator.Object);
            mockMagicalItemGenerator.Setup(g => g.Generate("more power", "item name")).Returns(magicalItem);

            var item = itemsGenerator.GenerateAtLevel(1, "item type", "item name");
            Assert.That(item, Is.EqualTo(magicalItem));
        }

        [Test]
        public void GenerateAtLevel_Named_GetMundaneItem_WithTraits()
        {
            percentileSelection.Type = PowerConstants.Mundane;
            extraSelection.Type = string.Empty;
            extraSelection.AmountAsDouble = 0;

            mockCollectionSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, "item name"))
                .Returns(new[] { PowerConstants.Mundane, "power", "more power", "wrong power" });

            var firstItem = new Item();
            mockJustInTimeFactory.Setup(f => f.Build<MundaneItemGenerator>("item type")).Returns(mockMundaneItemGenerator.Object);
            mockMundaneItemGenerator.Setup(f => f.Generate("item name", "trait 1", "trait 2")).Returns(firstItem);

            var item = itemsGenerator.GenerateAtLevel(1, "item type", "item name", "trait 1", "trait 2");
            Assert.That(item, Is.EqualTo(firstItem));
        }

        [Test]
        public void GenerateAtLevel_Named_GetMagicalItem()
        {
            percentileSelection.Type = "power";
            extraSelection.Type = string.Empty;
            extraSelection.AmountAsDouble = 0;

            mockCollectionSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, "item name"))
                .Returns(new[] { PowerConstants.Mundane, "power", "more power", "wrong power" });

            var magicalItem = new Item();
            mockJustInTimeFactory.Setup(f => f.Build<MagicalItemGenerator>("item type")).Returns(mockMagicalItemGenerator.Object);
            mockMagicalItemGenerator.Setup(g => g.Generate("power", "item name")).Returns(magicalItem);

            var item = itemsGenerator.GenerateAtLevel(1, "item type", "item name");
            Assert.That(item, Is.EqualTo(magicalItem));
        }

        [Test]
        public void GenerateAtLevel_Named_GetMagicalItem_WithTraits()
        {
            percentileSelection.Type = "power";
            extraSelection.Type = string.Empty;
            extraSelection.AmountAsDouble = 0;

            mockCollectionSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, "item name"))
                .Returns(new[] { PowerConstants.Mundane, "power", "more power", "wrong power" });

            var magicalItem = new Item();
            mockJustInTimeFactory.Setup(f => f.Build<MagicalItemGenerator>("item type")).Returns(mockMagicalItemGenerator.Object);
            mockMagicalItemGenerator.Setup(g => g.Generate("power", "item name", "trait 1", "trait 2")).Returns(magicalItem);

            var item = itemsGenerator.GenerateAtLevel(1, "item type", "item name", "trait 1", "trait 2");
            Assert.That(item, Is.EqualTo(magicalItem));
        }

        [Test]
        public void BUG_GenerateAtLevel_Named_GetMagicalItem_ScrollWithCustomName()
        {
            percentileSelection.Type = "power";
            extraSelection.Type = string.Empty;
            extraSelection.AmountAsDouble = 0;

            mockCollectionSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, ItemTypeConstants.Scroll))
                .Returns(new[] { PowerConstants.Mundane, "power", "more power", "wrong power" });

            var magicalItem = new Item();
            mockJustInTimeFactory.Setup(f => f.Build<MagicalItemGenerator>(ItemTypeConstants.Scroll)).Returns(mockMagicalItemGenerator.Object);
            mockMagicalItemGenerator.Setup(g => g.Generate("power", "item name")).Returns(magicalItem);

            var item = itemsGenerator.GenerateAtLevel(1, ItemTypeConstants.Scroll, "item name");
            Assert.That(item, Is.EqualTo(magicalItem));
        }

        [Test]
        public void BUG_GenerateAtLevel_Named_GetMagicalItem_WandWithCustomName()
        {
            percentileSelection.Type = "power";
            extraSelection.Type = string.Empty;
            extraSelection.AmountAsDouble = 0;

            mockCollectionSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, ItemTypeConstants.Wand))
                .Returns(new[] { PowerConstants.Mundane, "power", "more power", "wrong power" });

            var magicalItem = new Item();
            mockJustInTimeFactory.Setup(f => f.Build<MagicalItemGenerator>(ItemTypeConstants.Wand)).Returns(mockMagicalItemGenerator.Object);
            mockMagicalItemGenerator.Setup(g => g.Generate("power", "item name")).Returns(magicalItem);

            var item = itemsGenerator.GenerateAtLevel(1, ItemTypeConstants.Wand, "item name");
            Assert.That(item, Is.EqualTo(magicalItem));
        }

        [Test]
        public void GenerateAtLevel_Named_GetPowerFromSelector()
        {
            mockCollectionSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, "item name"))
                .Returns(new[] { PowerConstants.Mundane, "power", "more power", "wrong power" });

            itemsGenerator.GenerateAtLevel(96, "item type", "item name");
            var expectedTableName = TableNameConstants.Percentiles.LevelXItems(20);
            mockTypeAndAmountPercentileSelector.Verify(p => p.SelectFrom(Config.Name, expectedTableName), Times.Once);
        }

        [Test]
        public void GenerateAtLevel_Named_ReturnItem()
        {
            percentileSelection.Type = PowerConstants.Mundane;
            percentileSelection.AmountAsDouble = 2;
            extraSelection.Type = string.Empty;
            extraSelection.AmountAsDouble = 0;

            mockCollectionSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, "item name"))
                .Returns(new[] { PowerConstants.Mundane, "power", "more power", "wrong power" });

            var generatedItem = new Item();
            mockJustInTimeFactory
                .Setup(f => f.Build<MundaneItemGenerator>("item type"))
                .Returns(mockMundaneItemGenerator.Object);
            mockMundaneItemGenerator
                .Setup(f => f.Generate("item name"))
                .Returns(generatedItem);

            var item = itemsGenerator.GenerateAtLevel(1, "item type", "item name");
            Assert.That(item, Is.EqualTo(generatedItem));
        }

        [Test]
        public async Task GenerateAtLevelAsync_Named_GetPowerFromSelector()
        {
            mockCollectionSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, "item name"))
                .Returns(new[] { PowerConstants.Mundane, "power", "more power", "wrong power" });

            await itemsGenerator.GenerateAtLevelAsync(96, "item type", "item name");
            var expectedTableName = TableNameConstants.Percentiles.LevelXItems(20);
            mockTypeAndAmountPercentileSelector.Verify(p => p.SelectFrom(Config.Name, expectedTableName), Times.Once);
        }

        [Test]
        public async Task GenerateAtLevelAsync_Named_ReturnItem()
        {
            percentileSelection.Type = PowerConstants.Mundane;
            percentileSelection.AmountAsDouble = 2;
            extraSelection.Type = string.Empty;
            extraSelection.AmountAsDouble = 0;

            mockCollectionSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, "item name"))
                .Returns(new[] { PowerConstants.Mundane, "power", "more power", "wrong power" });

            var generatedItem = new Item();
            mockJustInTimeFactory
                .Setup(f => f.Build<MundaneItemGenerator>("item type"))
                .Returns(mockMundaneItemGenerator.Object);
            mockMundaneItemGenerator
                .Setup(f => f.Generate("item name"))
                .Returns(generatedItem);

            var item = await itemsGenerator.GenerateAtLevelAsync(1, "item type", "item name");
            Assert.That(item, Is.EqualTo(generatedItem));
        }

        [Test]
        public async Task GenerateAtLevelAsync_Named_GetMundaneItem()
        {
            percentileSelection.Type = PowerConstants.Mundane;
            percentileSelection.AmountAsDouble = 1;
            extraSelection.Type = string.Empty;
            extraSelection.AmountAsDouble = 0;

            mockCollectionSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, "item name"))
                .Returns(new[] { PowerConstants.Mundane, "power", "more power", "wrong power" });

            var mundaneItem = new Item();
            mockJustInTimeFactory.Setup(f => f.Build<MundaneItemGenerator>("item type")).Returns(mockMundaneItemGenerator.Object);
            mockMundaneItemGenerator.Setup(g => g.Generate("item name")).Returns(mundaneItem);

            var item = await itemsGenerator.GenerateAtLevelAsync(1, "item type", "item name");
            Assert.That(item, Is.EqualTo(mundaneItem));
        }

        [Test]
        public async Task BUG_GenerateAtLevelAsync_Named_GetMundaneItem_WithNoPower()
        {
            percentileSelection.Type = string.Empty;
            percentileSelection.AmountAsDouble = 1;
            extraSelection.Type = string.Empty;
            extraSelection.AmountAsDouble = 0;

            mockCollectionSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, "item name"))
                .Returns(new[] { PowerConstants.Mundane, "power", "more power", "wrong power" });

            var mundaneItem = new Item();
            mockJustInTimeFactory.Setup(f => f.Build<MundaneItemGenerator>("item type")).Returns(mockMundaneItemGenerator.Object);
            mockMundaneItemGenerator.Setup(g => g.Generate("item name")).Returns(mundaneItem);

            var item = await itemsGenerator.GenerateAtLevelAsync(1, "item type", "item name");
            Assert.That(item, Is.EqualTo(mundaneItem));
        }

        [Test]
        public async Task BUG_GenerateAtLevelAsync_Named_GetMagicalItem_WithNoPower()
        {
            percentileSelection.Type = string.Empty;
            percentileSelection.AmountAsDouble = 1;
            extraSelection.Type = string.Empty;
            extraSelection.AmountAsDouble = 0;

            mockCollectionSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, "item name"))
                .Returns(new[] { "power", "more power", "wrong power" });

            var magicalItem = new Item();
            mockJustInTimeFactory.Setup(f => f.Build<MagicalItemGenerator>("item type")).Returns(mockMagicalItemGenerator.Object);
            mockMagicalItemGenerator.Setup(g => g.Generate("power", "item name")).Returns(magicalItem);

            var item = await itemsGenerator.GenerateAtLevelAsync(1, "item type", "item name");
            Assert.That(item, Is.EqualTo(magicalItem));
        }

        [Test]
        public async Task BUG_GenerateAtLevelAsync_Named_GetMundaneItem_WhenPowerIsMagical_ButItemTypeIsMundane()
        {
            percentileSelection.Type = "power";
            extraSelection.Type = string.Empty;
            extraSelection.AmountAsDouble = 0;

            mockCollectionSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, "item name"))
                .Returns(new[] { PowerConstants.Mundane });

            var firstItem = new Item();
            mockJustInTimeFactory.Setup(f => f.Build<MundaneItemGenerator>("item type")).Returns(mockMundaneItemGenerator.Object);
            mockMundaneItemGenerator.Setup(f => f.Generate("item name")).Returns(firstItem);

            var item = await itemsGenerator.GenerateAtLevelAsync(1, "item type", "item name");
            Assert.That(item, Is.EqualTo(firstItem));
        }

        [Test]
        public async Task BUG_GenerateAtLevelAsync_Named_GetMagicalItem_WhenPowerIsMundane_ButItemTypeIsMagical()
        {
            percentileSelection.Type = PowerConstants.Mundane;
            extraSelection.Type = string.Empty;
            extraSelection.AmountAsDouble = 0;

            mockCollectionSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, "item name"))
                .Returns(new[] { "power", "more power", "wrong power" });

            var magicalItem = new Item();
            mockJustInTimeFactory.Setup(f => f.Build<MagicalItemGenerator>("item type")).Returns(mockMagicalItemGenerator.Object);
            mockMagicalItemGenerator.Setup(g => g.Generate("power", "item name")).Returns(magicalItem);

            var item = await itemsGenerator.GenerateAtLevelAsync(1, "item type", "item name");
            Assert.That(item, Is.EqualTo(magicalItem));
        }

        [Test]
        public async Task BUG_GenerateAtLevelAsync_Named_GetMagicalItem_WhenPowerIsMagical_ButItemTypeIsHigherMagical()
        {
            percentileSelection.Type = "power";
            extraSelection.Type = string.Empty;
            extraSelection.AmountAsDouble = 0;

            mockCollectionSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, "item name"))
                .Returns(new[] { "more power", "wrong power" });

            var magicalItem = new Item();
            mockJustInTimeFactory.Setup(f => f.Build<MagicalItemGenerator>("item type")).Returns(mockMagicalItemGenerator.Object);
            mockMagicalItemGenerator.Setup(g => g.Generate("more power", "item name")).Returns(magicalItem);

            var item = await itemsGenerator.GenerateAtLevelAsync(1, "item type", "item name");
            Assert.That(item, Is.EqualTo(magicalItem));
        }

        [Test]
        public async Task GenerateAtLevelAsync_Named_GetMagicalItem()
        {
            percentileSelection.Type = "power";
            percentileSelection.AmountAsDouble = 1;
            extraSelection.Type = string.Empty;
            extraSelection.AmountAsDouble = 0;

            mockCollectionSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, "item name"))
                .Returns(new[] { PowerConstants.Mundane, "power", "more power", "wrong power" });

            var magicalItem = new Item();
            mockJustInTimeFactory
                .Setup(f => f.Build<MagicalItemGenerator>("item type"))
                .Returns(mockMagicalItemGenerator.Object);
            mockMagicalItemGenerator
                .Setup(g => g.Generate(percentileSelection.Type, "item name"))
                .Returns(magicalItem);

            var item = await itemsGenerator.GenerateAtLevelAsync(1, "item type", "item name");
            Assert.That(item, Is.EqualTo(magicalItem));
        }
    }
}