using DnDGen.TreasureGen.Generators.Items.Magical;
using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Selectors.Percentiles;
using DnDGen.TreasureGen.Tables;
using Moq;
using NUnit.Framework;
using System;
using System.Linq;

namespace DnDGen.TreasureGen.Tests.Unit.Generators.Items.Magical
{
    [TestFixture]
    public class WandGeneratorTests
    {
        private MagicalItemGenerator wandGenerator;
        private Mock<ITreasurePercentileSelector> mockPercentileSelector;
        private Mock<IChargesGenerator> mockChargesGenerator;
        private string power;
        private ItemVerifier itemVerifier;

        [SetUp]
        public void Setup()
        {
            mockPercentileSelector = new Mock<ITreasurePercentileSelector>();
            mockChargesGenerator = new Mock<IChargesGenerator>();
            wandGenerator = new WandGenerator(mockPercentileSelector.Object, mockChargesGenerator.Object);
            power = "power";
            itemVerifier = new ItemVerifier();
        }

        [Test]
        public void GenerateWand()
        {
            var wand = wandGenerator.GenerateRandom(power);

            Assert.That(wand.Name, Does.StartWith("Wand of "));
            Assert.That(wand.BaseNames.Single(), Is.EqualTo(ItemTypeConstants.Wand));
            Assert.That(wand.ItemType, Is.EqualTo(ItemTypeConstants.Wand));
            Assert.That(wand.IsMagical, Is.True);
            Assert.That(wand.Attributes, Contains.Item(AttributeConstants.Charged));
            Assert.That(wand.Attributes, Contains.Item(AttributeConstants.OneTimeUse));
            Assert.That(wand.Quantity, Is.EqualTo(1));
            Assert.That(wand.Contents, Is.Empty);
        }

        [Test]
        public void GetWandSpellFromSelector()
        {
            var tableName = TableNameConstants.Percentiles.POWERITEMTYPEs(power, ItemTypeConstants.Wand);
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, tableName)).Returns("wand spell");

            var wand = wandGenerator.GenerateRandom(power);
            Assert.That(wand.Name, Is.EqualTo("Wand of wand spell"));
        }

        [Test]
        public void GetChargesFromGenerator()
        {
            var tableName = TableNameConstants.Percentiles.POWERITEMTYPEs(power, ItemTypeConstants.Wand);
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, tableName)).Returns("wand spell");
            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Wand, "Wand of wand spell")).Returns(9266);

            var wand = wandGenerator.GenerateRandom(power);
            Assert.That(wand.Magic.Charges, Is.EqualTo(9266));
        }

        [Test]
        public void GenerateCustomWand()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);

            var wand = wandGenerator.Generate(template);
            itemVerifier.AssertMagicalItemFromTemplate(wand, template);
            Assert.That(wand.Name, Is.EqualTo(name));
            Assert.That(wand.BaseNames.Single(), Is.EqualTo(ItemTypeConstants.Wand));
            Assert.That(wand.ItemType, Is.EqualTo(ItemTypeConstants.Wand));
            Assert.That(wand.IsMagical, Is.True);
            Assert.That(wand.Attributes, Contains.Item(AttributeConstants.Charged));
            Assert.That(wand.Attributes, Contains.Item(AttributeConstants.OneTimeUse));
            Assert.That(wand.Quantity, Is.EqualTo(1));
            Assert.That(wand.Contents, Is.Empty);
        }

        [Test]
        public void GenerateRandomCustomWand()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);

            var wand = wandGenerator.Generate(template, true);
            itemVerifier.AssertMagicalItemFromTemplate(wand, template);
            Assert.That(wand.Name, Is.EqualTo(name));
            Assert.That(wand.BaseNames.Single(), Is.EqualTo(ItemTypeConstants.Wand));
            Assert.That(wand.ItemType, Is.EqualTo(ItemTypeConstants.Wand));
            Assert.That(wand.IsMagical, Is.True);
            Assert.That(wand.Attributes, Contains.Item(AttributeConstants.Charged));
            Assert.That(wand.Attributes, Contains.Item(AttributeConstants.OneTimeUse));
            Assert.That(wand.Quantity, Is.EqualTo(1));
            Assert.That(wand.Contents, Is.Empty);
        }

        [Test]
        public void GenerateFromName()
        {
            var tableName = TableNameConstants.Percentiles.POWERITEMTYPEs(power, ItemTypeConstants.Wand);
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(Config.Name, tableName))
                .Returns("wrong spell")
                .Returns("spell")
                .Returns("other spell");

            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Wand, "Wand of wrong spell")).Returns(666);
            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Wand, "Wand of spell")).Returns(9266);
            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Wand, "Wand of other spell")).Returns(90210);

            var wand = wandGenerator.Generate(power, "Wand of spell");
            Assert.That(wand.Name, Is.EqualTo("Wand of spell"));
            Assert.That(wand.BaseNames.Single(), Is.EqualTo(ItemTypeConstants.Wand));
            Assert.That(wand.ItemType, Is.EqualTo(ItemTypeConstants.Wand));
            Assert.That(wand.IsMagical, Is.True);
            Assert.That(wand.Attributes, Contains.Item(AttributeConstants.Charged));
            Assert.That(wand.Attributes, Contains.Item(AttributeConstants.OneTimeUse));
            Assert.That(wand.Quantity, Is.EqualTo(1));
            Assert.That(wand.Contents, Is.Empty);
            Assert.That(wand.Magic.Charges, Is.EqualTo(9266));
        }

        [Test]
        public void GenerateFromName_WithTraits()
        {
            var tableName = TableNameConstants.Percentiles.POWERITEMTYPEs(power, ItemTypeConstants.Wand);
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(Config.Name, tableName))
                .Returns("wrong spell")
                .Returns("spell")
                .Returns("other spell");

            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Wand, "Wand of wrong spell")).Returns(666);
            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Wand, "Wand of spell")).Returns(9266);
            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Wand, "Wand of other spell")).Returns(90210);

            var wand = wandGenerator.Generate(power, "Wand of spell", "trait 1", "trait 2");
            Assert.That(wand.Name, Is.EqualTo("Wand of spell"));
            Assert.That(wand.BaseNames.Single(), Is.EqualTo(ItemTypeConstants.Wand));
            Assert.That(wand.ItemType, Is.EqualTo(ItemTypeConstants.Wand));
            Assert.That(wand.IsMagical, Is.True);
            Assert.That(wand.Attributes, Contains.Item(AttributeConstants.Charged));
            Assert.That(wand.Attributes, Contains.Item(AttributeConstants.OneTimeUse));
            Assert.That(wand.Quantity, Is.EqualTo(1));
            Assert.That(wand.Contents, Is.Empty);
            Assert.That(wand.Magic.Charges, Is.EqualTo(9266));
            Assert.That(wand.Traits, Has.Count.EqualTo(2)
                .And.Contains("trait 1")
                .And.Contains("trait 2"));
        }
    }
}