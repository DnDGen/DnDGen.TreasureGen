using DnDGen.Infrastructure.Models;
using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.RollGen;
using DnDGen.TreasureGen.Generators.Items.Magical;
using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Selectors.Percentiles;
using DnDGen.TreasureGen.Tables;
using Moq;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Unit.Generators.Items.Magical
{
    [TestFixture]
    public class ChargesGeneratorTests
    {
        private IChargesGenerator generator;
        private Mock<Dice> mockDice;
        private Mock<ICollectionTypeAndAmountSelector> mockTypeAndAmountSelector;
        private Mock<ITreasurePercentileSelector> mockPercentileSelector;

        [SetUp]
        public void Setup()
        {
            mockDice = new Mock<Dice>();
            mockTypeAndAmountSelector = new Mock<ICollectionTypeAndAmountSelector>();
            mockPercentileSelector = new Mock<ITreasurePercentileSelector>();
            generator = new ChargesGenerator(mockDice.Object, mockTypeAndAmountSelector.Object, mockPercentileSelector.Object);
        }

        [TestCase(ItemTypeConstants.Wand, 1, 1)]
        [TestCase(ItemTypeConstants.Wand, 2, 1)]
        [TestCase(ItemTypeConstants.Wand, 3, 1)]
        [TestCase(ItemTypeConstants.Wand, 4, 2)]
        [TestCase(ItemTypeConstants.Wand, 5, 2)]
        [TestCase(ItemTypeConstants.Wand, 6, 3)]
        [TestCase(ItemTypeConstants.Wand, 7, 3)]
        [TestCase(ItemTypeConstants.Wand, 8, 4)]
        [TestCase(ItemTypeConstants.Wand, 9, 4)]
        [TestCase(ItemTypeConstants.Wand, 10, 5)]
        [TestCase(ItemTypeConstants.Wand, 11, 5)]
        [TestCase(ItemTypeConstants.Wand, 12, 6)]
        [TestCase(ItemTypeConstants.Wand, 13, 6)]
        [TestCase(ItemTypeConstants.Wand, 14, 7)]
        [TestCase(ItemTypeConstants.Wand, 15, 7)]
        [TestCase(ItemTypeConstants.Wand, 16, 8)]
        [TestCase(ItemTypeConstants.Wand, 17, 8)]
        [TestCase(ItemTypeConstants.Wand, 18, 9)]
        [TestCase(ItemTypeConstants.Wand, 19, 9)]
        [TestCase(ItemTypeConstants.Wand, 20, 10)]
        [TestCase(ItemTypeConstants.Wand, 21, 10)]
        [TestCase(ItemTypeConstants.Wand, 22, 11)]
        [TestCase(ItemTypeConstants.Wand, 23, 11)]
        [TestCase(ItemTypeConstants.Wand, 24, 12)]
        [TestCase(ItemTypeConstants.Wand, 25, 12)]
        [TestCase(ItemTypeConstants.Wand, 26, 13)]
        [TestCase(ItemTypeConstants.Wand, 27, 13)]
        [TestCase(ItemTypeConstants.Wand, 28, 14)]
        [TestCase(ItemTypeConstants.Wand, 29, 14)]
        [TestCase(ItemTypeConstants.Wand, 30, 15)]
        [TestCase(ItemTypeConstants.Wand, 31, 15)]
        [TestCase(ItemTypeConstants.Wand, 32, 16)]
        [TestCase(ItemTypeConstants.Wand, 33, 16)]
        [TestCase(ItemTypeConstants.Wand, 34, 17)]
        [TestCase(ItemTypeConstants.Wand, 35, 17)]
        [TestCase(ItemTypeConstants.Wand, 36, 18)]
        [TestCase(ItemTypeConstants.Wand, 37, 18)]
        [TestCase(ItemTypeConstants.Wand, 38, 19)]
        [TestCase(ItemTypeConstants.Wand, 39, 19)]
        [TestCase(ItemTypeConstants.Wand, 40, 20)]
        [TestCase(ItemTypeConstants.Wand, 41, 20)]
        [TestCase(ItemTypeConstants.Wand, 42, 21)]
        [TestCase(ItemTypeConstants.Wand, 43, 21)]
        [TestCase(ItemTypeConstants.Wand, 44, 22)]
        [TestCase(ItemTypeConstants.Wand, 45, 22)]
        [TestCase(ItemTypeConstants.Wand, 46, 23)]
        [TestCase(ItemTypeConstants.Wand, 47, 23)]
        [TestCase(ItemTypeConstants.Wand, 48, 24)]
        [TestCase(ItemTypeConstants.Wand, 49, 24)]
        [TestCase(ItemTypeConstants.Wand, 50, 25)]
        [TestCase(ItemTypeConstants.Wand, 51, 25)]
        [TestCase(ItemTypeConstants.Wand, 52, 26)]
        [TestCase(ItemTypeConstants.Wand, 53, 26)]
        [TestCase(ItemTypeConstants.Wand, 54, 27)]
        [TestCase(ItemTypeConstants.Wand, 55, 27)]
        [TestCase(ItemTypeConstants.Wand, 56, 28)]
        [TestCase(ItemTypeConstants.Wand, 57, 28)]
        [TestCase(ItemTypeConstants.Wand, 58, 29)]
        [TestCase(ItemTypeConstants.Wand, 59, 29)]
        [TestCase(ItemTypeConstants.Wand, 60, 30)]
        [TestCase(ItemTypeConstants.Wand, 61, 30)]
        [TestCase(ItemTypeConstants.Wand, 62, 31)]
        [TestCase(ItemTypeConstants.Wand, 63, 31)]
        [TestCase(ItemTypeConstants.Wand, 64, 32)]
        [TestCase(ItemTypeConstants.Wand, 65, 32)]
        [TestCase(ItemTypeConstants.Wand, 66, 33)]
        [TestCase(ItemTypeConstants.Wand, 67, 33)]
        [TestCase(ItemTypeConstants.Wand, 68, 34)]
        [TestCase(ItemTypeConstants.Wand, 69, 34)]
        [TestCase(ItemTypeConstants.Wand, 70, 35)]
        [TestCase(ItemTypeConstants.Wand, 71, 35)]
        [TestCase(ItemTypeConstants.Wand, 72, 36)]
        [TestCase(ItemTypeConstants.Wand, 73, 36)]
        [TestCase(ItemTypeConstants.Wand, 74, 37)]
        [TestCase(ItemTypeConstants.Wand, 75, 37)]
        [TestCase(ItemTypeConstants.Wand, 76, 38)]
        [TestCase(ItemTypeConstants.Wand, 77, 38)]
        [TestCase(ItemTypeConstants.Wand, 78, 39)]
        [TestCase(ItemTypeConstants.Wand, 79, 39)]
        [TestCase(ItemTypeConstants.Wand, 80, 40)]
        [TestCase(ItemTypeConstants.Wand, 81, 40)]
        [TestCase(ItemTypeConstants.Wand, 82, 41)]
        [TestCase(ItemTypeConstants.Wand, 83, 41)]
        [TestCase(ItemTypeConstants.Wand, 84, 42)]
        [TestCase(ItemTypeConstants.Wand, 85, 42)]
        [TestCase(ItemTypeConstants.Wand, 86, 43)]
        [TestCase(ItemTypeConstants.Wand, 87, 43)]
        [TestCase(ItemTypeConstants.Wand, 88, 44)]
        [TestCase(ItemTypeConstants.Wand, 89, 44)]
        [TestCase(ItemTypeConstants.Wand, 90, 45)]
        [TestCase(ItemTypeConstants.Wand, 91, 45)]
        [TestCase(ItemTypeConstants.Wand, 92, 46)]
        [TestCase(ItemTypeConstants.Wand, 93, 46)]
        [TestCase(ItemTypeConstants.Wand, 94, 47)]
        [TestCase(ItemTypeConstants.Wand, 95, 47)]
        [TestCase(ItemTypeConstants.Wand, 96, 48)]
        [TestCase(ItemTypeConstants.Wand, 97, 48)]
        [TestCase(ItemTypeConstants.Wand, 98, 49)]
        [TestCase(ItemTypeConstants.Wand, 99, 49)]
        [TestCase(ItemTypeConstants.Wand, 100, 50)]
        [TestCase(ItemTypeConstants.Staff, 1, 1)]
        [TestCase(ItemTypeConstants.Staff, 2, 1)]
        [TestCase(ItemTypeConstants.Staff, 3, 1)]
        [TestCase(ItemTypeConstants.Staff, 4, 2)]
        [TestCase(ItemTypeConstants.Staff, 5, 2)]
        [TestCase(ItemTypeConstants.Staff, 6, 3)]
        [TestCase(ItemTypeConstants.Staff, 7, 3)]
        [TestCase(ItemTypeConstants.Staff, 8, 4)]
        [TestCase(ItemTypeConstants.Staff, 9, 4)]
        [TestCase(ItemTypeConstants.Staff, 10, 5)]
        [TestCase(ItemTypeConstants.Staff, 11, 5)]
        [TestCase(ItemTypeConstants.Staff, 12, 6)]
        [TestCase(ItemTypeConstants.Staff, 13, 6)]
        [TestCase(ItemTypeConstants.Staff, 14, 7)]
        [TestCase(ItemTypeConstants.Staff, 15, 7)]
        [TestCase(ItemTypeConstants.Staff, 16, 8)]
        [TestCase(ItemTypeConstants.Staff, 17, 8)]
        [TestCase(ItemTypeConstants.Staff, 18, 9)]
        [TestCase(ItemTypeConstants.Staff, 19, 9)]
        [TestCase(ItemTypeConstants.Staff, 20, 10)]
        [TestCase(ItemTypeConstants.Staff, 21, 10)]
        [TestCase(ItemTypeConstants.Staff, 22, 11)]
        [TestCase(ItemTypeConstants.Staff, 23, 11)]
        [TestCase(ItemTypeConstants.Staff, 24, 12)]
        [TestCase(ItemTypeConstants.Staff, 25, 12)]
        [TestCase(ItemTypeConstants.Staff, 26, 13)]
        [TestCase(ItemTypeConstants.Staff, 27, 13)]
        [TestCase(ItemTypeConstants.Staff, 28, 14)]
        [TestCase(ItemTypeConstants.Staff, 29, 14)]
        [TestCase(ItemTypeConstants.Staff, 30, 15)]
        [TestCase(ItemTypeConstants.Staff, 31, 15)]
        [TestCase(ItemTypeConstants.Staff, 32, 16)]
        [TestCase(ItemTypeConstants.Staff, 33, 16)]
        [TestCase(ItemTypeConstants.Staff, 34, 17)]
        [TestCase(ItemTypeConstants.Staff, 35, 17)]
        [TestCase(ItemTypeConstants.Staff, 36, 18)]
        [TestCase(ItemTypeConstants.Staff, 37, 18)]
        [TestCase(ItemTypeConstants.Staff, 38, 19)]
        [TestCase(ItemTypeConstants.Staff, 39, 19)]
        [TestCase(ItemTypeConstants.Staff, 40, 20)]
        [TestCase(ItemTypeConstants.Staff, 41, 20)]
        [TestCase(ItemTypeConstants.Staff, 42, 21)]
        [TestCase(ItemTypeConstants.Staff, 43, 21)]
        [TestCase(ItemTypeConstants.Staff, 44, 22)]
        [TestCase(ItemTypeConstants.Staff, 45, 22)]
        [TestCase(ItemTypeConstants.Staff, 46, 23)]
        [TestCase(ItemTypeConstants.Staff, 47, 23)]
        [TestCase(ItemTypeConstants.Staff, 48, 24)]
        [TestCase(ItemTypeConstants.Staff, 49, 24)]
        [TestCase(ItemTypeConstants.Staff, 50, 25)]
        [TestCase(ItemTypeConstants.Staff, 51, 25)]
        [TestCase(ItemTypeConstants.Staff, 52, 26)]
        [TestCase(ItemTypeConstants.Staff, 53, 26)]
        [TestCase(ItemTypeConstants.Staff, 54, 27)]
        [TestCase(ItemTypeConstants.Staff, 55, 27)]
        [TestCase(ItemTypeConstants.Staff, 56, 28)]
        [TestCase(ItemTypeConstants.Staff, 57, 28)]
        [TestCase(ItemTypeConstants.Staff, 58, 29)]
        [TestCase(ItemTypeConstants.Staff, 59, 29)]
        [TestCase(ItemTypeConstants.Staff, 60, 30)]
        [TestCase(ItemTypeConstants.Staff, 61, 30)]
        [TestCase(ItemTypeConstants.Staff, 62, 31)]
        [TestCase(ItemTypeConstants.Staff, 63, 31)]
        [TestCase(ItemTypeConstants.Staff, 64, 32)]
        [TestCase(ItemTypeConstants.Staff, 65, 32)]
        [TestCase(ItemTypeConstants.Staff, 66, 33)]
        [TestCase(ItemTypeConstants.Staff, 67, 33)]
        [TestCase(ItemTypeConstants.Staff, 68, 34)]
        [TestCase(ItemTypeConstants.Staff, 69, 34)]
        [TestCase(ItemTypeConstants.Staff, 70, 35)]
        [TestCase(ItemTypeConstants.Staff, 71, 35)]
        [TestCase(ItemTypeConstants.Staff, 72, 36)]
        [TestCase(ItemTypeConstants.Staff, 73, 36)]
        [TestCase(ItemTypeConstants.Staff, 74, 37)]
        [TestCase(ItemTypeConstants.Staff, 75, 37)]
        [TestCase(ItemTypeConstants.Staff, 76, 38)]
        [TestCase(ItemTypeConstants.Staff, 77, 38)]
        [TestCase(ItemTypeConstants.Staff, 78, 39)]
        [TestCase(ItemTypeConstants.Staff, 79, 39)]
        [TestCase(ItemTypeConstants.Staff, 80, 40)]
        [TestCase(ItemTypeConstants.Staff, 81, 40)]
        [TestCase(ItemTypeConstants.Staff, 82, 41)]
        [TestCase(ItemTypeConstants.Staff, 83, 41)]
        [TestCase(ItemTypeConstants.Staff, 84, 42)]
        [TestCase(ItemTypeConstants.Staff, 85, 42)]
        [TestCase(ItemTypeConstants.Staff, 86, 43)]
        [TestCase(ItemTypeConstants.Staff, 87, 43)]
        [TestCase(ItemTypeConstants.Staff, 88, 44)]
        [TestCase(ItemTypeConstants.Staff, 89, 44)]
        [TestCase(ItemTypeConstants.Staff, 90, 45)]
        [TestCase(ItemTypeConstants.Staff, 91, 45)]
        [TestCase(ItemTypeConstants.Staff, 92, 46)]
        [TestCase(ItemTypeConstants.Staff, 93, 46)]
        [TestCase(ItemTypeConstants.Staff, 94, 47)]
        [TestCase(ItemTypeConstants.Staff, 95, 47)]
        [TestCase(ItemTypeConstants.Staff, 96, 48)]
        [TestCase(ItemTypeConstants.Staff, 97, 48)]
        [TestCase(ItemTypeConstants.Staff, 98, 49)]
        [TestCase(ItemTypeConstants.Staff, 99, 49)]
        [TestCase(ItemTypeConstants.Staff, 100, 50)]
        public void ChargePercentileRoll(string itemType, int roll, int quantity)
        {
            mockDice.Setup(d => d.Roll(1).d(100).AsSum<int>()).Returns(roll);
            var charges = generator.GenerateFor(ItemTypeConstants.Staff, string.Empty);
            Assert.That(charges, Is.EqualTo(quantity));
        }

        [Test]
        public void GetMinAndMaxForNamedItemsFromAttributesSelector()
        {
            var result = new TypeAndAmountDataSelection
            {
                AmountAsDouble = 9266
            };

            mockTypeAndAmountSelector.Setup(s => s.SelectOneFrom(Config.Name, TableNameConstants.Collections.ChargeLimits, "name")).Returns(result);

            var charges = generator.GenerateFor(string.Empty, "name");
            Assert.That(charges, Is.EqualTo(9266));
        }

        [Test]
        public void GenerateFullDeckOfIllusions()
        {
            var result = new TypeAndAmountDataSelection
            {
                AmountAsDouble = 9
            };
            var fullResult = new TypeAndAmountDataSelection
            {
                AmountAsDouble = 34
            };

            mockTypeAndAmountSelector
                .Setup(s => s.SelectOneFrom(Config.Name, TableNameConstants.Collections.ChargeLimits, WondrousItemConstants.DeckOfIllusions))
                .Returns(result);
            mockTypeAndAmountSelector
                .Setup(s => s.SelectOneFrom(Config.Name, TableNameConstants.Collections.ChargeLimits, WondrousItemConstants.DeckOfIllusions_Full))
                .Returns(fullResult);

            mockPercentileSelector.Setup(s => s.SelectFrom<bool>(Config.Name, TableNameConstants.Percentiles.IsDeckOfIllusionsFullyCharged)).Returns(true);

            var charges = generator.GenerateFor(string.Empty, WondrousItemConstants.DeckOfIllusions);
            Assert.That(charges, Is.EqualTo(34));
        }

        [Test]
        public void GeneratePartiallyFullDeckOfIllusions()
        {
            var result = new TypeAndAmountDataSelection
            {
                AmountAsDouble = 9
            };
            var fullResult = new TypeAndAmountDataSelection
            {
                AmountAsDouble = 34
            };

            mockTypeAndAmountSelector
                .Setup(s => s.SelectOneFrom(Config.Name, TableNameConstants.Collections.ChargeLimits, WondrousItemConstants.DeckOfIllusions))
                .Returns(result);
            mockTypeAndAmountSelector
                .Setup(s => s.SelectOneFrom(Config.Name, TableNameConstants.Collections.ChargeLimits, WondrousItemConstants.DeckOfIllusions_Full))
                .Returns(fullResult);

            mockPercentileSelector.Setup(s => s.SelectFrom<bool>(Config.Name, TableNameConstants.Percentiles.IsDeckOfIllusionsFullyCharged)).Returns(false);

            var charges = generator.GenerateFor(string.Empty, WondrousItemConstants.DeckOfIllusions);
            Assert.That(charges, Is.EqualTo(9));
        }
    }
}