using DnDGen.RollGen;
using DnDGen.TreasureGen.Generators.Items.Magical;
using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using Moq;
using NUnit.Framework;
using System;
using System.Linq;

namespace DnDGen.TreasureGen.Tests.Unit.Generators.Items.Magical
{
    [TestFixture]
    public class ScrollGeneratorTests
    {
        private MagicalItemGenerator scrollGenerator;
        private Mock<ISpellGenerator> mockSpellGenerator;
        private Mock<Dice> mockDice;
        private ItemVerifier itemVerifier;

        [SetUp]
        public void Setup()
        {
            mockSpellGenerator = new Mock<ISpellGenerator>();
            mockDice = new Mock<Dice>();
            scrollGenerator = new ScrollGenerator(mockDice.Object, mockSpellGenerator.Object);

            itemVerifier = new ItemVerifier();

            mockDice.Setup(d => d.Roll(1).d(3).AsSum<int>()).Returns(1);
        }

        [Test]
        public void GenerateScroll()
        {
            var scroll = scrollGenerator.GenerateRandom(PowerConstants.Minor);
            AssertScroll(scroll);
        }

        private void AssertScroll(Item scroll, string name = ItemTypeConstants.Scroll)
        {
            Assert.That(scroll.ItemType, Is.EqualTo(ItemTypeConstants.Scroll));
            Assert.That(scroll.Name, Is.EqualTo(name));
            Assert.That(scroll.IsMagical, Is.True);
            Assert.That(scroll.Attributes.Single(), Is.EqualTo(AttributeConstants.OneTimeUse));
            Assert.That(scroll.Quantity, Is.EqualTo(1));
            Assert.That(scroll.BaseNames.Single(), Is.EqualTo(ItemTypeConstants.Scroll));
        }

        [Test]
        public void GetTypeAndLevelFromSpellGenerator()
        {
            mockSpellGenerator.Setup(g => g.GenerateType()).Returns("spell type");
            mockSpellGenerator.Setup(g => g.GenerateLevel(PowerConstants.Minor)).Returns(9266);
            mockSpellGenerator.Setup(g => g.Generate("spell type", 9266)).Returns("spell");

            var scroll = scrollGenerator.GenerateRandom(PowerConstants.Minor);
            AssertScroll(scroll);
            Assert.That(scroll.Traits, Contains.Item("spell type"));
            Assert.That(scroll.Contents, Contains.Item("spell (9266)"));
        }

        [Test]
        public void MinorScrollsHave1d3Spells()
        {
            mockDice.Setup(d => d.Roll(1).d(3).AsSum<int>()).Returns(9266);

            var scroll = scrollGenerator.GenerateRandom(PowerConstants.Minor);
            AssertScroll(scroll);
            Assert.That(scroll.Contents.Count, Is.EqualTo(9266));
        }

        [Test]
        public void MediumScrollsHave1d4Spells()
        {
            mockDice.Setup(d => d.Roll(1).d(4).AsSum<int>()).Returns(9266);

            var scroll = scrollGenerator.GenerateRandom(PowerConstants.Medium);
            AssertScroll(scroll);
            Assert.That(scroll.Contents.Count, Is.EqualTo(9266));
        }

        [Test]
        public void MajorScrollsHave1d6Spells()
        {
            mockDice.Setup(d => d.Roll(1).d(6).AsSum<int>()).Returns(9266);

            var scroll = scrollGenerator.GenerateRandom(PowerConstants.Major);
            AssertScroll(scroll);
            Assert.That(scroll.Contents.Count, Is.EqualTo(9266));
        }

        [Test]
        public void UnknownPowerThrowsError()
        {
            Assert.That(() => scrollGenerator.GenerateRandom("power"), Throws.ArgumentException);
        }

        [Test]
        public void GenerateSpellTypeOnce()
        {
            mockDice.Setup(d => d.Roll(1).d(3).AsSum<int>()).Returns(9266);

            var scroll = scrollGenerator.GenerateRandom(PowerConstants.Minor);
            AssertScroll(scroll);
            mockSpellGenerator.Verify(g => g.GenerateType(), Times.Once);
        }

        [Test]
        public void GenerateLevelAndSpellEachTime()
        {
            mockDice.Setup(d => d.Roll(1).d(3).AsSum<int>()).Returns(9266);

            var scroll = scrollGenerator.GenerateRandom(PowerConstants.Minor);
            AssertScroll(scroll);
            mockSpellGenerator.Verify(g => g.GenerateLevel(PowerConstants.Minor), Times.Exactly(9266));
            mockSpellGenerator.Verify(g => g.Generate(It.IsAny<string>(), It.IsAny<int>()), Times.Exactly(9266));
        }

        [Test]
        public void GenerateCustomScroll()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);

            var scroll = scrollGenerator.Generate(template);
            ItemVerifier.AssertMagicalItemFromTemplate(scroll, template);
            Assert.That(scroll.ItemType, Is.EqualTo(ItemTypeConstants.Scroll));
            Assert.That(scroll.IsMagical, Is.True);
            Assert.That(scroll.Attributes.Single(), Is.EqualTo(AttributeConstants.OneTimeUse));
            Assert.That(scroll.Quantity, Is.EqualTo(1));
            Assert.That(scroll.Magic.Bonus, Is.EqualTo(0));
            Assert.That(scroll.Magic.Charges, Is.EqualTo(0));
            Assert.That(scroll.BaseNames.Single(), Is.EqualTo(ItemTypeConstants.Scroll));
        }

        [Test]
        public void GenerateRandomCustomScroll()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);

            var scroll = scrollGenerator.Generate(template, true);
            ItemVerifier.AssertMagicalItemFromTemplate(scroll, template);
            Assert.That(scroll.ItemType, Is.EqualTo(ItemTypeConstants.Scroll));
            Assert.That(scroll.IsMagical, Is.True);
            Assert.That(scroll.Attributes.Single(), Is.EqualTo(AttributeConstants.OneTimeUse));
            Assert.That(scroll.Quantity, Is.EqualTo(1));
            Assert.That(scroll.Magic.Bonus, Is.EqualTo(0));
            Assert.That(scroll.Magic.Charges, Is.EqualTo(0));
            Assert.That(scroll.BaseNames.Single(), Is.EqualTo(ItemTypeConstants.Scroll));
        }

        [Test]
        public void GenerateFromName()
        {
            var scroll = scrollGenerator.Generate(PowerConstants.Minor, ItemTypeConstants.Scroll);
            AssertScroll(scroll);
        }

        [Test]
        public void GenerateFromName_WithTraits()
        {
            mockSpellGenerator.Setup(g => g.GenerateType()).Returns("spell type");
            mockSpellGenerator.Setup(g => g.GenerateLevel(PowerConstants.Minor)).Returns(9266);
            mockSpellGenerator.Setup(g => g.Generate("spell type", 9266)).Returns("spell");

            var scroll = scrollGenerator.Generate(PowerConstants.Minor, ItemTypeConstants.Scroll, "trait 1", "trait 2");
            AssertScroll(scroll);
            Assert.That(scroll.Traits, Has.Count.EqualTo(3)
                .And.Contains("trait 1")
                .And.Contains("trait 2")
                .And.Contains("spell type"));
        }

        [Test]
        public void GenerateFromNonScrollName()
        {
            var scroll = scrollGenerator.Generate(PowerConstants.Minor, "my special thing");
            AssertScroll(scroll, "my special thing");
        }

        [Test]
        public void GenerateFromNonScrollName_WithTraits()
        {
            mockSpellGenerator.Setup(g => g.GenerateType()).Returns("spell type");
            mockSpellGenerator.Setup(g => g.GenerateLevel(PowerConstants.Minor)).Returns(9266);
            mockSpellGenerator.Setup(g => g.Generate("spell type", 9266)).Returns("spell");

            var scroll = scrollGenerator.Generate(PowerConstants.Minor, "my special thing", "trait 1", "trait 2");
            AssertScroll(scroll, "my special thing");
            Assert.That(scroll.Traits, Has.Count.EqualTo(3)
                .And.Contains("trait 1")
                .And.Contains("trait 2")
                .And.Contains("spell type"));
        }
    }
}