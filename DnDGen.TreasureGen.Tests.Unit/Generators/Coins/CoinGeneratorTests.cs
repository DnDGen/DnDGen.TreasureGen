using DnDGen.Infrastructure.Models;
using DnDGen.Infrastructure.Selectors.Percentiles;
using DnDGen.TreasureGen.Coins;
using DnDGen.TreasureGen.Generators.Coins;
using DnDGen.TreasureGen.Tables;
using Moq;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Unit.Generators.Coins
{
    [TestFixture]
    public class CoinGeneratorTests
    {
        private Mock<IPercentileTypeAndAmountSelector> mockTypeAndAmountPercentileSelector;
        private ICoinGenerator generator;

        private TypeAndAmountDataSelection selection;

        [SetUp]
        public void Setup()
        {
            mockTypeAndAmountPercentileSelector = new Mock<IPercentileTypeAndAmountSelector>();
            generator = new CoinGenerator(mockTypeAndAmountPercentileSelector.Object);
            selection = new TypeAndAmountDataSelection
            {
                Type = "coin type",
                AmountAsDouble = 9266
            };
            mockTypeAndAmountPercentileSelector.Setup(p => p.SelectFrom(Config.Name, It.IsAny<string>())).Returns(selection);

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
        [TestCase(LevelLimits.Maximum_Standard - 1)]
        [TestCase(LevelLimits.Maximum_Standard)]
        public void ReturnCoinFromSelector(int level)
        {
            var tableName = TableNameConstants.Percentiles.LevelXCoins(level);
            generator.GenerateAtLevel(level);
            mockTypeAndAmountPercentileSelector.Verify(p => p.SelectFrom(Config.Name, tableName), Times.Once);
        }

        [TestCase(LevelLimits.Maximum_Standard + 1)]
        [TestCase(LevelLimits.Maximum_Epic - 1)]
        [TestCase(LevelLimits.Maximum_Epic)]
        [TestCase(LevelLimits.Maximum_Epic + 1)]
        [TestCase(42)]
        [TestCase(9266)]
        public void ReturnCoinFromSelector_HighLevel(int level)
        {
            var tableName = TableNameConstants.Percentiles.LevelXCoins(LevelLimits.Maximum_Standard);
            generator.GenerateAtLevel(level);
            mockTypeAndAmountPercentileSelector.Verify(p => p.SelectFrom(Config.Name, tableName), Times.Once);
        }

        [Test]
        public void CoinIsEmptyIfResultIsEmpty()
        {
            selection.Type = string.Empty;
            selection.AmountAsDouble = 0;

            var coin = generator.GenerateAtLevel(1);
            Assert.That(coin.Currency, Is.Empty);
            Assert.That(coin.Quantity, Is.Zero);
        }

        [Test]
        public void GetCurrencyAmount()
        {
            var coin = generator.GenerateAtLevel(1);
            Assert.That(coin.Currency, Is.EqualTo(selection.Type));
            Assert.That(coin.Quantity, Is.EqualTo(9266));
        }
    }
}