using DnDGen.TreasureGen.Generators.Items.Mundane;
using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Mundane;
using DnDGen.TreasureGen.Selectors.Percentiles;
using DnDGen.TreasureGen.Tables;
using Moq;
using NUnit.Framework;
using System;
using System.Linq;

namespace DnDGen.TreasureGen.Tests.Unit.Generators.Items.Mundane
{
    [TestFixture]
    public class ToolGeneratorTests
    {
        private MundaneItemGenerator toolGenerator;
        private Mock<ITreasurePercentileSelector> mockPercentileSelector;
        private ItemVerifier itemVerifier;

        [SetUp]
        public void Setup()
        {
            mockPercentileSelector = new Mock<ITreasurePercentileSelector>();
            toolGenerator = new ToolGenerator(mockPercentileSelector.Object);
            itemVerifier = new ItemVerifier();
        }

        [Test]
        public void GenerateTool()
        {
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, TableNameConstants.Percentiles.Tools)).Returns("tool");

            var tool = toolGenerator.GenerateRandom();
            Assert.That(tool.Name, Is.EqualTo("tool"));
            Assert.That(tool.BaseNames.Single(), Is.EqualTo("tool"));
            Assert.That(tool.Attributes, Is.Empty);
            Assert.That(tool.ItemType, Is.EqualTo(ItemTypeConstants.Tool));
            Assert.That(tool.IsMagical, Is.False);
            Assert.That(tool.Contents, Is.Empty);
            Assert.That(tool.Quantity, Is.EqualTo(1));
            Assert.That(tool.Traits, Is.Empty);
        }

        [Test]
        public void GenerateCustomTool()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);

            var tool = toolGenerator.Generate(template);
            itemVerifier.AssertMundaneItemFromTemplate(tool, template);
            Assert.That(tool.BaseNames.Single(), Is.EqualTo(name));
            Assert.That(tool.ItemType, Is.EqualTo(ItemTypeConstants.Tool));
            Assert.That(tool.Attributes, Is.Empty);
            Assert.That(tool.IsMagical, Is.False);
            Assert.That(tool.Quantity, Is.EqualTo(1));
        }

        [Test]
        public void GenerateRandomCustomTool()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);

            var tool = toolGenerator.Generate(template, true);
            itemVerifier.AssertMundaneItemFromTemplate(tool, template);
            Assert.That(tool.BaseNames.Single(), Is.EqualTo(name));
            Assert.That(tool.ItemType, Is.EqualTo(ItemTypeConstants.Tool));
            Assert.That(tool.Attributes, Is.Empty);
            Assert.That(tool.IsMagical, Is.False);
            Assert.That(tool.Quantity, Is.EqualTo(1));
        }

        [Test]
        public void GenerateFromName()
        {
            var tool = toolGenerator.Generate("tool");
            Assert.That(tool.Name, Is.EqualTo("tool"));
            Assert.That(tool.BaseNames.Single(), Is.EqualTo("tool"));
            Assert.That(tool.Attributes, Is.Empty);
            Assert.That(tool.ItemType, Is.EqualTo(ItemTypeConstants.Tool));
            Assert.That(tool.IsMagical, Is.False);
            Assert.That(tool.Contents, Is.Empty);
            Assert.That(tool.Quantity, Is.EqualTo(1));
            Assert.That(tool.Traits, Is.Empty);
        }

        [Test]
        public void GenerateFromNameWithTraits()
        {
            var tool = toolGenerator.Generate("tool", "my trait", "my other trait");
            Assert.That(tool.Name, Is.EqualTo("tool"));
            Assert.That(tool.BaseNames.Single(), Is.EqualTo("tool"));
            Assert.That(tool.Attributes, Is.Empty);
            Assert.That(tool.ItemType, Is.EqualTo(ItemTypeConstants.Tool));
            Assert.That(tool.IsMagical, Is.False);
            Assert.That(tool.Contents, Is.Empty);
            Assert.That(tool.Quantity, Is.EqualTo(1));
            Assert.That(tool.Traits, Contains.Item("my trait"));
            Assert.That(tool.Traits, Contains.Item("my other trait"));
            Assert.That(tool.Traits.Count, Is.EqualTo(2));
        }

        [Test]
        public void GenerateFromNameWithDuplicateTraits()
        {
            var tool = toolGenerator.Generate("tool", "my trait", "my trait");
            Assert.That(tool.Name, Is.EqualTo("tool"));
            Assert.That(tool.BaseNames.Single(), Is.EqualTo("tool"));
            Assert.That(tool.Attributes, Is.Empty);
            Assert.That(tool.ItemType, Is.EqualTo(ItemTypeConstants.Tool));
            Assert.That(tool.IsMagical, Is.False);
            Assert.That(tool.Contents, Is.Empty);
            Assert.That(tool.Quantity, Is.EqualTo(1));
            Assert.That(tool.Traits, Contains.Item("my trait"));
            Assert.That(tool.Traits.Count, Is.EqualTo(1));
        }
    }
}