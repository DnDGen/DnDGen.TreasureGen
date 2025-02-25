using DnDGen.TreasureGen.Generators.Items.Magical;
using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Tables;
using Moq;
using NUnit.Framework;
using System;

namespace DnDGen.TreasureGen.Tests.Unit.Generators.Items.Magical
{
    [TestFixture]
    public class MagicalItemGeneratorCurseDecoratorTests
    {
        private MagicalItemGenerator decorator;
        private Mock<ICurseGenerator> mockCurseGenerator;
        private Mock<MagicalItemGenerator> mockInnerGenerator;
        private Item innerItem;
        private ItemVerifier itemVerifier;

        [SetUp]
        public void Setup()
        {
            mockInnerGenerator = new Mock<MagicalItemGenerator>();
            mockCurseGenerator = new Mock<ICurseGenerator>();
            decorator = new MagicalItemGeneratorCurseDecorator(mockInnerGenerator.Object, mockCurseGenerator.Object);
            innerItem = new Item();
            itemVerifier = new ItemVerifier();

            innerItem.ItemType = "item type";
            innerItem.Name = "item name";

            mockInnerGenerator.Setup(g => g.GenerateRandom("power")).Returns(innerItem);
        }

        [Test]
        public void GetItemFromInnerGenerator()
        {
            var item = decorator.GenerateRandom("power");
            Assert.That(item, Is.EqualTo(innerItem));
        }

        [Test]
        public void DoNotGetCurseIfNotCursed()
        {
            mockCurseGenerator.Setup(g => g.HasCurse(innerItem)).Returns(false);
            mockCurseGenerator.Setup(g => g.GenerateCurse()).Returns("cursed");

            var item = decorator.GenerateRandom("power");
            Assert.That(item.Magic.Curse, Is.Empty);
        }

        [Test]
        public void GetCurseIfCursed()
        {
            mockCurseGenerator.Setup(g => g.HasCurse(innerItem)).Returns(true);
            mockCurseGenerator.Setup(g => g.GenerateCurse()).Returns("cursed");

            var item = decorator.GenerateRandom("power");
            Assert.That(item.Magic.Curse, Is.EqualTo("cursed"));
        }

        [Test]
        public void GetSpecificCursedItem_CanBeSpecific()
        {
            var specificCursedItem = new Item();
            mockCurseGenerator.Setup(g => g.HasCurse(innerItem)).Returns(true);
            mockCurseGenerator.Setup(g => g.ItemTypeCanBeSpecificCursedItem(innerItem.ItemType)).Returns(true);
            mockCurseGenerator.Setup(g => g.GenerateCurse()).Returns(TableNameConstants.Percentiles.SpecificCursedItems);
            mockCurseGenerator.Setup(g => g.GenerateSpecificCursedItem(innerItem.ItemType)).Returns(specificCursedItem);

            var item = decorator.GenerateRandom("power");
            Assert.That(item, Is.EqualTo(specificCursedItem));
        }

        [Test]
        public void DoNotGetSpecificCursedItem_CannotBeSpecific()
        {
            var specificCursedItem = new Item();
            mockCurseGenerator.Setup(g => g.HasCurse(innerItem)).Returns(true);
            mockCurseGenerator.Setup(g => g.ItemTypeCanBeSpecificCursedItem(innerItem.ItemType)).Returns(false);
            mockCurseGenerator.SetupSequence(g => g.GenerateCurse())
                .Returns(TableNameConstants.Percentiles.SpecificCursedItems)
                .Returns(TableNameConstants.Percentiles.SpecificCursedItems)
                .Returns("cursed");

            var item = decorator.GenerateRandom("power");
            Assert.That(item, Is.EqualTo(innerItem));
            Assert.That(item.Magic.Curse, Is.EqualTo("cursed"));
        }

        [Test]
        public void DecorateCustomItem()
        {
            var template = new Item();

            mockInnerGenerator.Setup(g => g.Generate(template, true)).Returns(innerItem);
            mockCurseGenerator.Setup(g => g.HasCurse(innerItem)).Returns(true);
            mockCurseGenerator.Setup(g => g.GenerateCurse()).Returns("cursed");

            var decoratedItem = decorator.Generate(template, allowRandomDecoration: true);
            Assert.That(decoratedItem, Is.Not.EqualTo(template));
            Assert.That(decoratedItem, Is.EqualTo(innerItem));
            Assert.That(decoratedItem.Magic.Curse, Is.EqualTo("cursed"));
        }

        [Test]
        public void DoNotDecorateCustomItem()
        {
            var template = new Item();

            mockInnerGenerator.Setup(g => g.Generate(template, false)).Returns(innerItem);
            mockCurseGenerator.Setup(g => g.HasCurse(innerItem)).Returns(true);
            mockCurseGenerator.Setup(g => g.GenerateCurse()).Returns("cursed");

            var decoratedItem = decorator.Generate(template);
            Assert.That(decoratedItem, Is.Not.EqualTo(template));
            Assert.That(decoratedItem, Is.EqualTo(innerItem));
            Assert.That(decoratedItem.Magic.Curse, Is.Empty);
        }

        [Test]
        public void DoNotGetSpecificCursedItemsForCustomItem()
        {
            var template = new Item();
            var specificCursedItem = new Item();

            mockInnerGenerator.Setup(g => g.Generate(template, true)).Returns(innerItem);
            mockCurseGenerator.Setup(g => g.HasCurse(innerItem)).Returns(true);
            mockCurseGenerator.SetupSequence(g => g.GenerateCurse())
                .Returns(TableNameConstants.Percentiles.SpecificCursedItems)
                .Returns(TableNameConstants.Percentiles.SpecificCursedItems)
                .Returns("cursed");
            mockCurseGenerator.Setup(g => g.GenerateRandom()).Returns(specificCursedItem);

            var decoratedItem = decorator.Generate(template, allowRandomDecoration: true);
            Assert.That(decoratedItem, Is.Not.EqualTo(template));
            Assert.That(decoratedItem, Is.Not.EqualTo(specificCursedItem));
            Assert.That(decoratedItem, Is.EqualTo(innerItem));
            Assert.That(decoratedItem.Magic.Curse, Is.EqualTo("cursed"));
        }

        [Test]
        public void GenerateSpecificCursedCustomItem()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);

            var cursedItem = itemVerifier.CreateRandomTemplate(name);
            mockCurseGenerator.Setup(g => g.IsSpecificCursedItem(template)).Returns(true);
            mockCurseGenerator.Setup(g => g.Generate(template, false)).Returns(cursedItem);

            var decoratedItem = decorator.Generate(template, allowRandomDecoration: true);
            Assert.That(decoratedItem, Is.EqualTo(cursedItem));
            mockInnerGenerator.Verify(g => g.Generate(It.IsAny<Item>(), It.IsAny<bool>()), Times.Never);
        }

        [Test]
        public void GenerateNoCurseFromName()
        {
            mockCurseGenerator.Setup(g => g.HasCurse(innerItem)).Returns(false);
            mockCurseGenerator.Setup(g => g.GenerateCurse()).Returns("cursed");

            mockInnerGenerator.Setup(g => g.Generate("power", "item name", "trait 1", "trait 2")).Returns(innerItem);

            var item = decorator.Generate("power", "item name", "trait 1", "trait 2");
            Assert.That(item, Is.EqualTo(innerItem));
            Assert.That(item.Magic.Curse, Is.Empty);
        }

        [Test]
        public void GenerateCurseFromName()
        {
            mockCurseGenerator.Setup(g => g.HasCurse(innerItem)).Returns(true);
            mockCurseGenerator.Setup(g => g.GenerateCurse()).Returns("cursed");

            mockInnerGenerator.Setup(g => g.Generate("power", "item name", "trait 1", "trait 2")).Returns(innerItem);

            var item = decorator.Generate("power", "item name", "trait 1", "trait 2");
            Assert.That(item, Is.EqualTo(innerItem));
            Assert.That(item.Magic.Curse, Is.EqualTo("cursed"));
        }

        [Test]
        public void GenerateSpecificFromName_CanBeSpecific()
        {
            mockInnerGenerator.Setup(g => g.Generate("power", "item name", "trait 1", "trait 2")).Returns(innerItem);

            var specificCursedItem = new Item();
            mockCurseGenerator.Setup(g => g.HasCurse(innerItem)).Returns(true);
            mockCurseGenerator.Setup(g => g.CanBeSpecificCursedItem("item name")).Returns(true);
            mockCurseGenerator.Setup(g => g.GenerateCurse()).Returns(TableNameConstants.Percentiles.SpecificCursedItems);
            mockCurseGenerator.Setup(g => g.Generate("item name", "trait 1", "trait 2")).Returns(specificCursedItem);

            var item = decorator.Generate("power", "item name", "trait 1", "trait 2");
            Assert.That(item, Is.EqualTo(specificCursedItem));
        }

        [Test]
        public void DoNotGenerateSpecificFromName_CannotBeSpecific()
        {
            mockInnerGenerator.Setup(g => g.Generate("power", "item name", "trait 1", "trait 2")).Returns(innerItem);

            var specificCursedItem = new Item();
            mockCurseGenerator.Setup(g => g.HasCurse(innerItem)).Returns(true);
            mockCurseGenerator.Setup(g => g.CanBeSpecificCursedItem("item name")).Returns(false);
            mockCurseGenerator.SetupSequence(g => g.GenerateCurse())
                .Returns(TableNameConstants.Percentiles.SpecificCursedItems)
                .Returns(TableNameConstants.Percentiles.SpecificCursedItems)
                .Returns("cursed");
            mockCurseGenerator.Setup(g => g.Generate("item name")).Returns(specificCursedItem);

            var item = decorator.Generate("power", "item name", "trait 1", "trait 2");
            Assert.That(item, Is.EqualTo(innerItem));
            Assert.That(item.Magic.Curse, Is.EqualTo("cursed"));
        }
    }
}