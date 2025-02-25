using DnDGen.Infrastructure.Models;
using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.Infrastructure.Selectors.Percentiles;
using DnDGen.TreasureGen.Generators.Goods;
using DnDGen.TreasureGen.Goods;
using DnDGen.TreasureGen.Tables;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DnDGen.TreasureGen.Tests.Unit.Generators.Goods
{
    [TestFixture]
    public class GoodsGeneratorTests
    {
        private Mock<IPercentileTypeAndAmountSelector> mockTypeAndAmountPercentileSelector;
        private Mock<ICollectionSelector> mockCollectionsSelector;
        private IGoodsGenerator generator;

        private TypeAndAmountDataSelection selection;
        private TypeAndAmountDataSelection valueSelection;
        private List<string> descriptions;

        [SetUp]
        public void Setup()
        {
            mockTypeAndAmountPercentileSelector = new Mock<IPercentileTypeAndAmountSelector>();
            mockCollectionsSelector = new Mock<ICollectionSelector>();
            generator = new GoodsGenerator(mockTypeAndAmountPercentileSelector.Object, mockCollectionsSelector.Object);
            selection = new TypeAndAmountDataSelection();
            valueSelection = new TypeAndAmountDataSelection();

            selection.Type = "type";
            selection.AmountAsDouble = 2;
            valueSelection.Type = "first value";
            valueSelection.AmountAsDouble = 9266;
            mockTypeAndAmountPercentileSelector.Setup(p => p.SelectFrom(Config.Name, It.IsAny<string>())).Returns(selection);

            var tableName = TableNameConstants.Percentiles.GOODTYPEValues(selection.Type);
            mockTypeAndAmountPercentileSelector.Setup(p => p.SelectFrom(Config.Name, tableName)).Returns(valueSelection);

            descriptions = ["description 1", "description 2"];
            tableName = TableNameConstants.Collections.GOODTYPEDescriptions(selection.Type);

            var count = 0;
            object testLock = new();
            mockCollectionsSelector
                .Setup(p => p.SelectRandomFrom(Config.Name, tableName, It.IsAny<string>()))
                .Returns(() =>
                {
                    lock (testLock)
                    {
                        return descriptions[count++ % descriptions.Count];
                    }
                });
        }

        [Test]
        public void GenerateAtLevel_ThrowsException_LevelTooLow()
        {
            Assert.That(() => generator.GenerateAtLevel(LevelLimits.Minimum - 1),
                Throws.ArgumentException.With.Message.EqualTo($"Level 0 is not a valid level for treasure generation"));
        }

        [TestCase(LevelLimits.Minimum)]
        [TestCase(LevelLimits.Minimum + 1)]
        [TestCase(10)]
        [TestCase(20)]
        [TestCase(LevelLimits.Maximum_Standard - 1)]
        [TestCase(LevelLimits.Maximum_Standard)]
        public void GenerateAtLevel_GoodsAreGenerated(int level)
        {
            var goods = generator.GenerateAtLevel(level);
            Assert.That(goods, Is.Not.Null);

            var tableName = TableNameConstants.Percentiles.LevelXGoods(level);
            mockTypeAndAmountPercentileSelector.Verify(p => p.SelectFrom(Config.Name, tableName), Times.Once);
        }

        [TestCase(LevelLimits.Maximum_Standard + 1)]
        [TestCase(LevelLimits.Maximum_Epic - 1)]
        [TestCase(LevelLimits.Maximum_Epic)]
        [TestCase(LevelLimits.Maximum_Epic + 1)]
        [TestCase(42)]
        [TestCase(9266)]
        public void GenerateAtLevel_GoodsAreGenerated_HighLevel(int level)
        {
            var goods = generator.GenerateAtLevel(level);
            Assert.That(goods, Is.Not.Null);

            var tableName = TableNameConstants.Percentiles.LevelXGoods(LevelLimits.Maximum_Standard);
            mockTypeAndAmountPercentileSelector.Verify(p => p.SelectFrom(Config.Name, tableName), Times.Once);
        }

        [Test]
        public void GenerateAtLevel_GetTypeAndAmountFromSelector()
        {
            var tableName = TableNameConstants.Percentiles.LevelXGoods(1);
            generator.GenerateAtLevel(1);
            mockTypeAndAmountPercentileSelector.Verify(p => p.SelectFrom(Config.Name, tableName), Times.Once);
        }

        [Test]
        public void GenerateAtLevel_EmptyGoodsIfNoGoodType()
        {
            selection.Type = string.Empty;
            selection.AmountAsDouble = 0;

            var goods = generator.GenerateAtLevel(1);
            Assert.That(goods, Is.Empty);
        }

        [Test]
        public void GenerateAtLevel_ReturnsNumberOfGoods()
        {
            selection.AmountAsDouble = 9266;
            var goods = generator.GenerateAtLevel(1);
            Assert.That(goods.Count(), Is.EqualTo(9266));
        }

        [Test]
        public void GenerateAtLevel_ValueDeterminedByValueResult()
        {
            var secondValueResult = new TypeAndAmountDataSelection
            {
                Type = "second value",
                AmountAsDouble = 90210
            };

            var tableName = TableNameConstants.Percentiles.GOODTYPEValues(selection.Type);
            mockTypeAndAmountPercentileSelector.SetupSequence(p => p.SelectFrom(Config.Name, tableName)).Returns(valueSelection).Returns(secondValueResult);

            var goods = generator.GenerateAtLevel(1);
            var firstGood = goods.First();
            var secondGood = goods.Last();

            Assert.That(firstGood.ValueInGold, Is.EqualTo(9266));
            Assert.That(secondGood.ValueInGold, Is.EqualTo(90210));
        }

        [Test]
        public void GenerateAtLevel_DescriptionDeterminedByValueResult()
        {
            var goods = generator.GenerateAtLevel(1);
            var firstGood = goods.First();
            var secondGood = goods.Last();

            Assert.That(firstGood.Description, Is.EqualTo("description 1"));
            Assert.That(secondGood.Description, Is.EqualTo("description 2"));
        }

        [Test]
        public async Task GenerateAtLevelAsync_GoodsAreGenerated()
        {
            var goods = await generator.GenerateAtLevelAsync(1);
            Assert.That(goods, Is.Not.Null);
        }

        [Test]
        public async Task GenerateAtLevelAsync_GetTypeAndAmountFromSelector()
        {
            var tableName = TableNameConstants.Percentiles.LevelXGoods(1);
            await generator.GenerateAtLevelAsync(1);
            mockTypeAndAmountPercentileSelector.Verify(p => p.SelectFrom(Config.Name, tableName), Times.Once);
        }

        [Test]
        public async Task GenerateAtLevelAsync_EmptyGoodsIfNoGoodType()
        {
            selection.Type = string.Empty;
            selection.AmountAsDouble = 0;

            var goods = await generator.GenerateAtLevelAsync(1);
            Assert.That(goods, Is.Empty);
        }

        [Test]
        public async Task GenerateAtLevelAsync_ReturnsNumberOfGoods()
        {
            selection.AmountAsDouble = 9266;
            var goods = await generator.GenerateAtLevelAsync(1);
            Assert.That(goods.Count(), Is.EqualTo(9266));
        }

        [Test]
        public async Task GenerateAtLevelAsync_ValueDeterminedByValueResult()
        {
            var secondValueResult = new TypeAndAmountDataSelection
            {
                Type = "second value",
                AmountAsDouble = 90210
            };

            var tableName = TableNameConstants.Percentiles.GOODTYPEValues(selection.Type);
            mockTypeAndAmountPercentileSelector.SetupSequence(p => p.SelectFrom(Config.Name, tableName)).Returns(valueSelection).Returns(secondValueResult);

            var goods = await generator.GenerateAtLevelAsync(1);
            var firstGood = goods.First();
            var secondGood = goods.Last();

            Assert.That(firstGood.ValueInGold, Is.EqualTo(9266).Or.EqualTo(90210));
            Assert.That(secondGood.ValueInGold, Is.EqualTo(9266).Or.EqualTo(90210));
            Assert.That(firstGood.ValueInGold, Is.Not.EqualTo(secondGood.ValueInGold));
        }

        [Test]
        public async Task GenerateAtLevelAsync_DescriptionDeterminedByValueResult()
        {
            var goods = await generator.GenerateAtLevelAsync(1);
            var descriptions = goods.Select(g => g.Description);

            Assert.That(descriptions, Contains.Item("description 1")
                .And.Contains("description 2"));
        }
    }
}