﻿using DnDGen.TreasureGen.Coins;
using DnDGen.TreasureGen.Generators.Coins;
using DnDGen.TreasureGen.Selectors.Percentiles;
using DnDGen.TreasureGen.Selectors.Selections;
using DnDGen.TreasureGen.Tables;
using Moq;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Unit.Generators.Coins
{
    [TestFixture]
    public class CoinGeneratorTests
    {
        private Mock<ITypeAndAmountPercentileSelector> mockTypeAndAmountPercentileSelector;
        private ICoinGenerator generator;

        private TypeAndAmountSelection selection;

        [SetUp]
        public void Setup()
        {
            mockTypeAndAmountPercentileSelector = new Mock<ITypeAndAmountPercentileSelector>();
            generator = new CoinGenerator(mockTypeAndAmountPercentileSelector.Object);
            selection = new TypeAndAmountSelection();

            selection.Type = "coin type";
            selection.Amount = 9266;
            mockTypeAndAmountPercentileSelector.Setup(p => p.SelectFrom(It.IsAny<string>())).Returns(selection);

        }

        [Test]
        public void GenerateAtLevel_ThrowsException_LevelTooLow()
        {
            Assert.That(() => generator.GenerateAtLevel(LevelLimits.Minimum - 1),
                Throws.ArgumentException.With.Message.EqualTo($"Level 0 is not a valid level for treasure generation"));
        }

        [Test]
        public void GenerateAtLevel_ThrowsException_LevelTooHigh()
        {
            Assert.That(() => generator.GenerateAtLevel(LevelLimits.Maximum + 1),
                Throws.ArgumentException.With.Message.EqualTo($"Level 101 is not a valid level for treasure generation"));
        }

        [TestCase(LevelLimits.Minimum)]
        [TestCase(LevelLimits.Minimum + 1)]
        [TestCase(10)]
        [TestCase(20)]
        [TestCase(30)]
        [TestCase(42)]
        [TestCase(LevelLimits.Maximum - 1)]
        [TestCase(LevelLimits.Maximum)]
        public void ReturnCoinFromSelector(int level)
        {
            var tableName = TableNameConstants.Percentiles.LevelXCoins, level);
            generator.GenerateAtLevel(level);
            mockTypeAndAmountPercentileSelector.Verify(p => p.SelectFrom(tableName), Times.Once);
        }

        [Test]
        public void CoinIsEmptyIfResultIsEmpty()
        {
            selection.Type = string.Empty;

            var coin = generator.GenerateAtLevel(1);
            Assert.That(coin.Currency, Is.EqualTo(string.Empty));
            Assert.That(coin.Quantity, Is.EqualTo(0));
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