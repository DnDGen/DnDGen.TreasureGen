using DnDGen.Infrastructure.Models;
using DnDGen.Infrastructure.Selectors.Percentiles;
using DnDGen.TreasureGen.Generators.Items.Mundane;
using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Mundane;
using DnDGen.TreasureGen.Tables;
using Moq;
using NUnit.Framework;
using System;
using System.Linq;

namespace DnDGen.TreasureGen.Tests.Unit.Generators.Items.Mundane
{
    [TestFixture]
    public class AlchemicalItemGeneratorTests
    {
        private MundaneItemGenerator alchemicalItemGenerator;
        private Mock<IPercentileTypeAndAmountSelector> mockTypeAndAmountPercentileSelector;
        private TypeAndAmountDataSelection selection;
        private ItemVerifier itemVerifier;

        [SetUp]
        public void Setup()
        {
            mockTypeAndAmountPercentileSelector = new Mock<IPercentileTypeAndAmountSelector>();
            alchemicalItemGenerator = new AlchemicalItemGenerator(mockTypeAndAmountPercentileSelector.Object);
            selection = new TypeAndAmountDataSelection();
            itemVerifier = new ItemVerifier();
        }

        [Test]
        public void GenerateAlchemicalItem()
        {
            selection.Type = "alchemical item";
            selection.AmountAsDouble = 9266;
            mockTypeAndAmountPercentileSelector
                .Setup(p => p.SelectFrom(Config.Name, TableNameConstants.Percentiles.AlchemicalItems))
                .Returns(selection);
            mockTypeAndAmountPercentileSelector
                .Setup(p => p.SelectAllFrom(Config.Name, TableNameConstants.Percentiles.AlchemicalItems))
                .Returns(
                [
                    new TypeAndAmountDataSelection { Type = "wrong item", AmountAsDouble = 666 },
                    selection
                ]);

            var item = alchemicalItemGenerator.GenerateRandom();
            Assert.That(item.Name, Is.EqualTo(selection.Type));
            Assert.That(item.BaseNames.Single(), Is.EqualTo(selection.Type));
            Assert.That(item.Quantity, Is.EqualTo(9266));
            Assert.That(item.ItemType, Is.EqualTo(ItemTypeConstants.AlchemicalItem));
            Assert.That(item.Attributes, Is.Empty);
            Assert.That(item.IsMagical, Is.False);
        }

        [Test]
        public void GenerateCustomAlchemicalItem()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);

            var item = alchemicalItemGenerator.Generate(template);
            itemVerifier.AssertMundaneItemFromTemplate(item, template);
            Assert.That(item.BaseNames.Single(), Is.EqualTo(name));
            Assert.That(item.ItemType, Is.EqualTo(ItemTypeConstants.AlchemicalItem));
            Assert.That(item.Attributes, Is.Empty);
        }

        [Test]
        public void GenerateRandomCustomAlchemicalItem()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);

            var item = alchemicalItemGenerator.Generate(template, true);
            itemVerifier.AssertMundaneItemFromTemplate(item, template);
            Assert.That(item.BaseNames.Single(), Is.EqualTo(name));
            Assert.That(item.ItemType, Is.EqualTo(ItemTypeConstants.AlchemicalItem));
            Assert.That(item.Attributes, Is.Empty);
        }

        [Test]
        public void GenerateFromName()
        {
            selection.Type = "alchemical item";
            selection.AmountAsDouble = 9266;
            mockTypeAndAmountPercentileSelector
                .SetupSequence(p => p.SelectAllFrom(Config.Name, TableNameConstants.Percentiles.AlchemicalItems))
                .Returns(new[]
                {
                    new TypeAndAmountDataSelection { Type = "wrong alchemical item", AmountAsDouble = 666 },
                    selection,
                    new TypeAndAmountDataSelection { Type = "other alchemical item", AmountAsDouble = 42 }
                });

            var item = alchemicalItemGenerator.Generate("alchemical item");
            Assert.That(item.Name, Is.EqualTo("alchemical item"));
            Assert.That(item.BaseNames.Single(), Is.EqualTo("alchemical item"));
            Assert.That(item.Attributes, Is.Empty);
            Assert.That(item.ItemType, Is.EqualTo(ItemTypeConstants.AlchemicalItem));
            Assert.That(item.IsMagical, Is.False);
            Assert.That(item.Contents, Is.Empty);
            Assert.That(item.Quantity, Is.EqualTo(9266));
            Assert.That(item.Traits, Is.Empty);
        }

        [Test]
        public void GenerateFromName_IfNoneMatching_SetQuantityTo1()
        {
            mockTypeAndAmountPercentileSelector
                .SetupSequence(p => p.SelectAllFrom(Config.Name, TableNameConstants.Percentiles.AlchemicalItems))
                .Returns(new[]
                {
                    new TypeAndAmountDataSelection { Type = "wrong alchemical item", AmountAsDouble = 9266 },
                    new TypeAndAmountDataSelection { Type = "other alchemical item", AmountAsDouble = 42 }
                });

            var item = alchemicalItemGenerator.Generate("alchemical item");
            Assert.That(item.Name, Is.EqualTo("alchemical item"));
            Assert.That(item.BaseNames.Single(), Is.EqualTo("alchemical item"));
            Assert.That(item.Attributes, Is.Empty);
            Assert.That(item.ItemType, Is.EqualTo(ItemTypeConstants.AlchemicalItem));
            Assert.That(item.IsMagical, Is.False);
            Assert.That(item.Contents, Is.Empty);
            Assert.That(item.Quantity, Is.EqualTo(1));
            Assert.That(item.Traits, Is.Empty);
        }

        [Test]
        public void GenerateFromNameWithTraits()
        {
            selection.Type = "alchemical item";
            selection.AmountAsDouble = 9266;
            mockTypeAndAmountPercentileSelector
                .SetupSequence(p => p.SelectAllFrom(Config.Name, TableNameConstants.Percentiles.AlchemicalItems))
                .Returns(new[]
                {
                    new TypeAndAmountDataSelection { Type = "wrong alchemical item", AmountAsDouble = 666 },
                    selection,
                    new TypeAndAmountDataSelection { Type = "other alchemical item", AmountAsDouble = 42 }
                });

            var item = alchemicalItemGenerator.Generate("alchemical item", "my trait", "my other trait");
            Assert.That(item.Name, Is.EqualTo("alchemical item"));
            Assert.That(item.BaseNames.Single(), Is.EqualTo("alchemical item"));
            Assert.That(item.Attributes, Is.Empty);
            Assert.That(item.ItemType, Is.EqualTo(ItemTypeConstants.AlchemicalItem));
            Assert.That(item.IsMagical, Is.False);
            Assert.That(item.Contents, Is.Empty);
            Assert.That(item.Quantity, Is.EqualTo(9266));
            Assert.That(item.Traits, Has.Count.EqualTo(2)
                .And.Contains("my trait")
                .And.Contains("my other trait"));
        }

        [Test]
        public void GenerateFromNameWithDuplicateTraits()
        {
            selection.Type = "alchemical item";
            selection.AmountAsDouble = 9266;
            mockTypeAndAmountPercentileSelector
                .SetupSequence(p => p.SelectAllFrom(Config.Name, TableNameConstants.Percentiles.AlchemicalItems))
                .Returns(new[]
                {
                    new TypeAndAmountDataSelection { Type = "wrong alchemical item", AmountAsDouble = 666 },
                    selection,
                    new TypeAndAmountDataSelection { Type = "other alchemical item", AmountAsDouble = 42 }
                });

            var item = alchemicalItemGenerator.Generate("alchemical item", "my trait", "my trait");
            Assert.That(item.Name, Is.EqualTo("alchemical item"));
            Assert.That(item.BaseNames.Single(), Is.EqualTo("alchemical item"));
            Assert.That(item.Attributes, Is.Empty);
            Assert.That(item.ItemType, Is.EqualTo(ItemTypeConstants.AlchemicalItem));
            Assert.That(item.IsMagical, Is.False);
            Assert.That(item.Contents, Is.Empty);
            Assert.That(item.Quantity, Is.EqualTo(9266));
            Assert.That(item.Traits, Has.Count.EqualTo(1)
                .And.Contains("my trait"));
        }
    }
}