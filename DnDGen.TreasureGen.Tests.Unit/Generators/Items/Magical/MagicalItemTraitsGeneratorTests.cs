using DnDGen.TreasureGen.Generators.Items.Magical;
using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Selectors.Percentiles;
using DnDGen.TreasureGen.Tables;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.TreasureGen.Tests.Unit.Generators.Items.Magical
{
    [TestFixture]
    public class MagicalItemTraitsGeneratorTests
    {
        private IMagicalItemTraitsGenerator generator;
        private Mock<ITreasurePercentileSelector> mockPercentileSelector;
        private List<string> attributes;
        private string itemType;

        [SetUp]
        public void Setup()
        {
            mockPercentileSelector = new Mock<ITreasurePercentileSelector>();
            generator = new MagicalItemTraitsGenerator(mockPercentileSelector.Object);
            attributes = [];
            itemType = "item type";
        }

        [Test]
        public void GenerateTraits()
        {
            var traits = generator.GenerateFor(itemType, attributes);
            Assert.That(traits, Is.Not.Null);
        }

        [Test]
        public void GetTraitsFromPercentileSelector()
        {
            var tableName = TableNameConstants.Percentiles.ITEMTYPETraits("item type");
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, tableName)).Returns("trait");
            var traits = generator.GenerateFor(itemType, attributes);
            Assert.That(traits, Contains.Item("trait"));
            Assert.That(traits.Count(), Is.EqualTo(1));
        }

        [Test]
        public void IMeleefWeapon_GetMeleeTraits()
        {
            attributes.Add(AttributeConstants.Melee);
            var tableName = TableNameConstants.Percentiles.ITEMTYPETraits(AttributeConstants.Melee);
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, tableName)).Returns("trait");

            var traits = generator.GenerateFor(itemType, attributes);
            Assert.That(traits, Contains.Item("trait"));
            Assert.That(traits.Count(), Is.EqualTo(1));
        }

        [Test]
        public void IfRangedWeapon_GetRangedTraits()
        {
            attributes.Add(AttributeConstants.Ranged);
            var tableName = TableNameConstants.Percentiles.ITEMTYPETraits(AttributeConstants.Ranged);
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, tableName)).Returns("trait");

            var traits = generator.GenerateFor(itemType, attributes);
            Assert.That(traits, Contains.Item("trait"));
            Assert.That(traits.Count(), Is.EqualTo(1));
        }

        [Test]
        public void IfMeleeAndRangedWeapon_GetMeleeTraits()
        {
            attributes.Add(AttributeConstants.Melee);
            attributes.Add(AttributeConstants.Ranged);
            var tableName = TableNameConstants.Percentiles.ITEMTYPETraits(AttributeConstants.Melee);
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, tableName)).Returns("trait");

            var traits = generator.GenerateFor(itemType, attributes);
            Assert.That(traits, Contains.Item("trait"));
            Assert.That(traits.Count(), Is.EqualTo(1));
        }

        [Test]
        public void SplitsCommaDelimitedTraits()
        {
            var tableName = TableNameConstants.Percentiles.ITEMTYPETraits("item type");
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, tableName)).Returns("trait,other trait");

            var traits = generator.GenerateFor(itemType, attributes);
            Assert.That(traits, Contains.Item("trait"));
            Assert.That(traits, Contains.Item("other trait"));
            Assert.That(traits.Count(), Is.EqualTo(2));
        }

        [Test]
        public void DoNotAddEmptyTrait()
        {
            var tableName = TableNameConstants.Percentiles.ITEMTYPETraits("item type");
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, tableName)).Returns(string.Empty);

            var traits = generator.GenerateFor(itemType, attributes);
            Assert.That(traits, Is.Empty);
        }
    }
}