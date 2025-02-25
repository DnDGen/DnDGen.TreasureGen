using DnDGen.Infrastructure.Models;
using DnDGen.Infrastructure.Selectors.Percentiles;
using DnDGen.TreasureGen.Selectors.Percentiles;
using Moq;
using NUnit.Framework;
using System.Linq;

namespace DnDGen.TreasureGen.Tests.Unit.Selectors.Percentiles
{
    [TestFixture]
    public class ReplacePercentileTypeAndAmountSelectorDecoratorTests
    {
        private ITreasurePercentileTypeAndAmountSelector decorator;
        private Mock<IPercentileTypeAndAmountSelector> mockInnerSelector;
        private Mock<IReplacementSelector> mockReplacementSelector;

        [SetUp]
        public void Setup()
        {
            mockInnerSelector = new Mock<IPercentileTypeAndAmountSelector>();
            mockReplacementSelector = new Mock<IReplacementSelector>();

            decorator = new PercentileTypeAndAmountSelectorStringReplacementDecorator(mockInnerSelector.Object, mockReplacementSelector.Object);
        }

        [Test]
        public void DecoratorSelectsFromInnerSelector()
        {
            var selection = new TypeAndAmountDataSelection { Type = "my type", Roll = "my roll", AmountAsDouble = 926.6 };

            mockInnerSelector.Setup(s => s.SelectFrom(Config.Name, "table")).Returns(selection);
            mockReplacementSelector.Setup(s => s.SelectRandom("my type")).Returns("my replacement");

            var result = decorator.SelectFrom(Config.Name, "table");
            Assert.That(result, Is.EqualTo(selection));
            Assert.That(result.Type, Is.EqualTo("my replacement"));
            Assert.That(result.Roll, Is.EqualTo("my roll"));
            Assert.That(result.AmountAsDouble, Is.EqualTo(926.6));
        }

        [Test]
        public void DecoratorSelectsAllFromInnerSelector()
        {
            var selections = new[]
            {
                new TypeAndAmountDataSelection { Type = "my type", Roll = "my roll", AmountAsDouble = 926.6 },
                new TypeAndAmountDataSelection { Type = "my other type", Roll = "my other roll", AmountAsDouble = 902.1 },
            };
            var replacements = new[] { "third", "fourth" };
            mockInnerSelector.Setup(s => s.SelectAllFrom(Config.Name, "table")).Returns(selections);
            mockReplacementSelector.Setup(s => s.SelectAll("my type", false)).Returns(["first", "second"]);
            mockReplacementSelector.Setup(s => s.SelectAll("my other type", false)).Returns(["third", "fourth"]);

            var results = decorator.SelectAllFrom(Config.Name, "table").ToArray();
            Assert.That(results.Length, Is.EqualTo(4));
            Assert.That(results[0].Type, Is.EqualTo("first"));
            Assert.That(results[0].Roll, Is.EqualTo("my roll"));
            Assert.That(results[0].AmountAsDouble, Is.EqualTo(926.6));
            Assert.That(results[1].Type, Is.EqualTo("second"));
            Assert.That(results[1].Roll, Is.EqualTo("my roll"));
            Assert.That(results[1].AmountAsDouble, Is.EqualTo(926.6));
            Assert.That(results[2].Type, Is.EqualTo("third"));
            Assert.That(results[2].Roll, Is.EqualTo("my other roll"));
            Assert.That(results[2].AmountAsDouble, Is.EqualTo(902.1));
            Assert.That(results[3].Type, Is.EqualTo("fourth"));
            Assert.That(results[3].Roll, Is.EqualTo("my other roll"));
            Assert.That(results[3].AmountAsDouble, Is.EqualTo(902.1));
        }
    }
}