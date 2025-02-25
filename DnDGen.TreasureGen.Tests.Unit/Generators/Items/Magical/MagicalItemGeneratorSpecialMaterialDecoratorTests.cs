using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.TreasureGen.Generators.Items.Magical;
using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Items.Mundane;
using DnDGen.TreasureGen.Tables;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Collections.Generic;

namespace DnDGen.TreasureGen.Tests.Unit.Generators.Items.Magical
{
    [TestFixture]
    public class MagicalItemGeneratorSpecialMaterialDecoratorTests
    {
        private MagicalItemGenerator decorator;
        private Mock<ISpecialMaterialGenerator> mockMaterialGenerator;
        private Mock<MagicalItemGenerator> mockInnerGenerator;
        private Item item;
        private Mock<ICollectionSelector> mockCollectionsSelector;
        private List<string> masterworkMaterials;

        [SetUp]
        public void Setup()
        {
            mockMaterialGenerator = new Mock<ISpecialMaterialGenerator>();
            mockInnerGenerator = new Mock<MagicalItemGenerator>();
            mockCollectionsSelector = new Mock<ICollectionSelector>();
            decorator = new MagicalItemGeneratorSpecialMaterialDecorator(mockInnerGenerator.Object, mockMaterialGenerator.Object, mockCollectionsSelector.Object);

            item = new Item();
            masterworkMaterials = new List<string>();

            mockInnerGenerator.Setup(g => g.GenerateRandom("power")).Returns(item);
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecialMaterials, TraitConstants.Masterwork))
                .Returns(masterworkMaterials);
        }

        [Test]
        public void GetItemFromInnerGenerator()
        {
            var decoratedItem = decorator.GenerateRandom("power");
            Assert.That(decoratedItem, Is.EqualTo(item));
        }

        [Test]
        public void SpecificCursedItemsDoNotHaveSpecialMaterials()
        {
            item.Magic.Curse = CurseConstants.SpecificCursedItem;

            mockMaterialGenerator.Setup(g => g.CanHaveSpecialMaterial(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>())).Returns(true);
            mockMaterialGenerator.Setup(g => g.GenerateFor(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>())).Returns("special material");

            var decoratedItem = decorator.GenerateRandom("power");
            Assert.That(decoratedItem.Traits, Is.Empty);
        }

        [TestCase(TraitConstants.SpecialMaterials.Adamantine)]
        [TestCase(TraitConstants.SpecialMaterials.AlchemicalSilver)]
        [TestCase(TraitConstants.SpecialMaterials.ColdIron)]
        [TestCase(TraitConstants.SpecialMaterials.Darkwood)]
        [TestCase(TraitConstants.SpecialMaterials.Dragonhide)]
        [TestCase(TraitConstants.SpecialMaterials.Mithral)]
        public void SpecificCursedItemsRemovesSpecialMaterialsFromName(string material)
        {
            item.Magic.Curse = CurseConstants.SpecificCursedItem;
            item.Traits.Add(material);
            item.Traits.Add("other trait");

            mockMaterialGenerator.Setup(g => g.CanHaveSpecialMaterial(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>())).Returns(true);
            mockMaterialGenerator.Setup(g => g.GenerateFor(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>())).Returns("special material");

            var decoratedItem = decorator.GenerateRandom("power");
            Assert.That(decoratedItem.Traits, Has.Count.EqualTo(1).And.Contains("other trait"));
        }

        [Test]
        public void DoNotGetSpecialMaterial()
        {
            mockMaterialGenerator.Setup(g => g.CanHaveSpecialMaterial(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>())).Returns(false);
            mockMaterialGenerator.Setup(g => g.GenerateFor(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>())).Returns("special material");

            var decoratedItem = decorator.GenerateRandom("power");
            Assert.That(decoratedItem.Traits, Is.Empty);
        }

        [Test]
        public void GetSpecialMaterial()
        {
            mockMaterialGenerator.SetupSequence(g => g.CanHaveSpecialMaterial(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>()))
                .Returns(true)
                .Returns(false);
            mockMaterialGenerator.Setup(g => g.GenerateFor(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>())).Returns("special material");

            var decoratedItem = decorator.GenerateRandom("power");
            Assert.That(decoratedItem.Traits, Contains.Item("special material"));
            Assert.That(decoratedItem.Traits.Count, Is.EqualTo(1));
        }

        [Test]
        public void GetMultipleSpecialMaterials()
        {
            mockMaterialGenerator.SetupSequence(g => g.CanHaveSpecialMaterial(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>()))
                .Returns(true)
                .Returns(true)
                .Returns(false);
            mockMaterialGenerator.SetupSequence(g => g.GenerateFor(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>()))
                .Returns("special material 1")
                .Returns("special material 2")
                .Returns("special material 3");

            var decoratedItem = decorator.GenerateRandom("power");
            Assert.That(decoratedItem.Traits, Contains.Item("special material 1"));
            Assert.That(decoratedItem.Traits, Contains.Item("special material 2"));
            Assert.That(decoratedItem.Traits.Count, Is.EqualTo(2));
        }

        [Test]
        public void DragonhideIsNotMetal()
        {
            item.Attributes = new[] { AttributeConstants.Metal };

            mockMaterialGenerator.SetupSequence(g => g.CanHaveSpecialMaterial(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>()))
                .Returns(true)
                .Returns(false);
            mockMaterialGenerator.Setup(g => g.GenerateFor(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>())).Returns(TraitConstants.SpecialMaterials.Dragonhide);

            var decoratedItem = decorator.GenerateRandom("power");
            Assert.That(decoratedItem.Traits, Contains.Item(TraitConstants.SpecialMaterials.Dragonhide));
            Assert.That(decoratedItem.Attributes, Is.All.Not.EqualTo(AttributeConstants.Metal));
        }

        [Test]
        public void DragonhideIsNotWood()
        {
            item.Attributes = new[] { AttributeConstants.Wood };

            mockMaterialGenerator.SetupSequence(g => g.CanHaveSpecialMaterial(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>()))
                .Returns(true)
                .Returns(false);
            mockMaterialGenerator.Setup(g => g.GenerateFor(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>())).Returns(TraitConstants.SpecialMaterials.Dragonhide);

            var decoratedItem = decorator.GenerateRandom("power");
            Assert.That(decoratedItem.Traits, Contains.Item(TraitConstants.SpecialMaterials.Dragonhide));
            Assert.That(decoratedItem.Attributes, Is.All.Not.EqualTo(AttributeConstants.Wood));
        }

        [Test]
        public void DecorateCustomItem()
        {
            var template = new Item();

            mockInnerGenerator.Setup(g => g.Generate(template, true)).Returns(item);
            mockMaterialGenerator.SetupSequence(g => g.CanHaveSpecialMaterial(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>()))
                .Returns(true)
                .Returns(false);
            mockMaterialGenerator.Setup(g => g.GenerateFor(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>())).Returns("special material");

            var decoratedItem = decorator.Generate(template, allowRandomDecoration: true);
            Assert.That(decoratedItem, Is.Not.EqualTo(template));
            Assert.That(decoratedItem, Is.EqualTo(item));
            Assert.That(decoratedItem.Traits, Contains.Item("special material"));
        }

        [Test]
        public void DecorateCustomItem_DragonhideIsNotWoodOrMetal()
        {
            var template = new Item();

            mockInnerGenerator.Setup(g => g.Generate(template, true)).Returns(item);
            mockMaterialGenerator.SetupSequence(g => g.CanHaveSpecialMaterial(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>()))
                .Returns(true)
                .Returns(false);
            mockMaterialGenerator.Setup(g => g.GenerateFor(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>())).Returns(TraitConstants.SpecialMaterials.Dragonhide);

            item.Attributes = new[] { "my attribute", AttributeConstants.Wood, AttributeConstants.Metal };

            var decoratedItem = decorator.Generate(template, allowRandomDecoration: true);
            Assert.That(decoratedItem, Is.Not.EqualTo(template));
            Assert.That(decoratedItem, Is.EqualTo(item));
            Assert.That(decoratedItem.Traits, Contains.Item(TraitConstants.SpecialMaterials.Dragonhide));
            Assert.That(decoratedItem.Attributes, Contains.Item("my attribute")
                .And.Not.Contains(AttributeConstants.Wood)
                .And.Not.Contains(AttributeConstants.Metal));
        }

        [Test]
        public void DoNotDecorateCustomItem()
        {
            var template = new Item();

            mockInnerGenerator.Setup(g => g.Generate(template, false)).Returns(item);
            mockMaterialGenerator.SetupSequence(g => g.CanHaveSpecialMaterial(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>()))
                .Returns(true)
                .Returns(false);
            mockMaterialGenerator.Setup(g => g.GenerateFor(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>())).Returns("special material");

            var decoratedItem = decorator.Generate(template);
            Assert.That(decoratedItem, Is.Not.SameAs(template));
            Assert.That(JsonConvert.SerializeObject(decoratedItem), Is.EqualTo(JsonConvert.SerializeObject(template)));
            Assert.That(decoratedItem, Is.EqualTo(item));
            Assert.That(decoratedItem.Traits, Is.Empty);
        }

        [Test]
        public void GenerateFromName()
        {
            var namedItem = new Item();
            mockInnerGenerator.Setup(g => g.Generate("power", "item name", "trait 1", "trait 2")).Returns(namedItem);

            var decoratedItem = decorator.Generate("power", "item name", "trait 1", "trait 2");
            Assert.That(decoratedItem, Is.EqualTo(namedItem));
        }

        [Test]
        public void SpecificCursedItemsDoNotHaveSpecialMaterialsFromName()
        {
            mockInnerGenerator.Setup(g => g.Generate("power", "item name", "trait 1", "trait 2")).Returns(item);

            item.Magic.Curse = CurseConstants.SpecificCursedItem;

            mockMaterialGenerator.Setup(g => g.CanHaveSpecialMaterial(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>())).Returns(true);
            mockMaterialGenerator.Setup(g => g.GenerateFor(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>())).Returns("special material");

            var decoratedItem = decorator.Generate("power", "item name", "trait 1", "trait 2");
            Assert.That(decoratedItem.Traits, Is.Empty);
        }

        [TestCase(TraitConstants.SpecialMaterials.Adamantine)]
        [TestCase(TraitConstants.SpecialMaterials.AlchemicalSilver)]
        [TestCase(TraitConstants.SpecialMaterials.ColdIron)]
        [TestCase(TraitConstants.SpecialMaterials.Darkwood)]
        [TestCase(TraitConstants.SpecialMaterials.Dragonhide)]
        [TestCase(TraitConstants.SpecialMaterials.Mithral)]
        public void SpecificCursedItemsRemovesSpecialMaterialsFromRandom(string material)
        {
            mockInnerGenerator.Setup(g => g.Generate("power", "item name", "trait 1", "trait 2")).Returns(item);

            item.Magic.Curse = CurseConstants.SpecificCursedItem;
            item.Traits.Add(material);
            item.Traits.Add("other trait");

            mockMaterialGenerator.Setup(g => g.CanHaveSpecialMaterial(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>())).Returns(true);
            mockMaterialGenerator.Setup(g => g.GenerateFor(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>())).Returns("special material");

            var decoratedItem = decorator.Generate("power", "item name", "trait 1", "trait 2");
            Assert.That(decoratedItem.Traits, Has.Count.EqualTo(1).And.Contains("other trait"));
        }

        [Test]
        public void DoNotGetSpecialMaterialFromName()
        {
            mockInnerGenerator.Setup(g => g.Generate("power", "item name", "trait 1", "trait 2")).Returns(item);

            mockMaterialGenerator.Setup(g => g.CanHaveSpecialMaterial(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>())).Returns(false);
            mockMaterialGenerator.Setup(g => g.GenerateFor(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>())).Returns("special material");

            var decoratedItem = decorator.Generate("power", "item name", "trait 1", "trait 2");
            Assert.That(decoratedItem.Traits, Is.Empty);
        }

        [Test]
        public void GetSpecialMaterialFromName()
        {
            mockInnerGenerator.Setup(g => g.Generate("power", "item name", "trait 1", "trait 2")).Returns(item);

            mockMaterialGenerator.SetupSequence(g => g.CanHaveSpecialMaterial(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>()))
                .Returns(true)
                .Returns(false);
            mockMaterialGenerator.Setup(g => g.GenerateFor(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>())).Returns("special material");

            var decoratedItem = decorator.Generate("power", "item name", "trait 1", "trait 2");
            Assert.That(decoratedItem.Traits, Contains.Item("special material"));
            Assert.That(decoratedItem.Traits.Count, Is.EqualTo(1));
        }

        [Test]
        public void GetMultipleSpecialMaterialsFromName()
        {
            mockInnerGenerator.Setup(g => g.Generate("power", "item name", "trait 1", "trait 2")).Returns(item);

            mockMaterialGenerator.SetupSequence(g => g.CanHaveSpecialMaterial(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>()))
                .Returns(true)
                .Returns(true)
                .Returns(false);
            mockMaterialGenerator.SetupSequence(g => g.GenerateFor(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>()))
                .Returns("special material 1")
                .Returns("special material 2")
                .Returns("special material 3");

            var decoratedItem = decorator.Generate("power", "item name", "trait 1", "trait 2");
            Assert.That(decoratedItem.Traits, Contains.Item("special material 1"));
            Assert.That(decoratedItem.Traits, Contains.Item("special material 2"));
            Assert.That(decoratedItem.Traits.Count, Is.EqualTo(2));
        }

        [Test]
        public void DragonhideFromNameIsNotMetal()
        {
            mockInnerGenerator.Setup(g => g.Generate("power", "item name", "trait 1", "trait 2")).Returns(item);

            item.Attributes = new[] { AttributeConstants.Metal };

            mockMaterialGenerator.SetupSequence(g => g.CanHaveSpecialMaterial(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>()))
                .Returns(true)
                .Returns(false);
            mockMaterialGenerator.Setup(g => g.GenerateFor(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>())).Returns(TraitConstants.SpecialMaterials.Dragonhide);

            var decoratedItem = decorator.Generate("power", "item name", "trait 1", "trait 2");
            Assert.That(decoratedItem.Traits, Contains.Item(TraitConstants.SpecialMaterials.Dragonhide));
            Assert.That(decoratedItem.Attributes, Is.All.Not.EqualTo(AttributeConstants.Metal));
        }

        [Test]
        public void DragonhideFromNameIsNotWood()
        {
            mockInnerGenerator.Setup(g => g.Generate("power", "item name", "trait 1", "trait 2")).Returns(item);

            item.Attributes = new[] { AttributeConstants.Wood };

            mockMaterialGenerator.SetupSequence(g => g.CanHaveSpecialMaterial(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>()))
                .Returns(true)
                .Returns(false);
            mockMaterialGenerator.Setup(g => g.GenerateFor(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>())).Returns(TraitConstants.SpecialMaterials.Dragonhide);

            var decoratedItem = decorator.Generate("power", "item name", "trait 1", "trait 2");
            Assert.That(decoratedItem.Traits, Contains.Item(TraitConstants.SpecialMaterials.Dragonhide));
            Assert.That(decoratedItem.Attributes, Is.All.Not.EqualTo(AttributeConstants.Wood));
        }

        [Test]
        public void DragonhideFromInnerNameIsNotWoodOrMetal()
        {
            mockInnerGenerator.Setup(g => g.Generate("power", "item name", "trait 1", "trait 2")).Returns(item);

            item.Attributes = new[] { "my attribute", AttributeConstants.Wood, AttributeConstants.Metal };
            item.Traits.Add(TraitConstants.SpecialMaterials.Dragonhide);

            mockMaterialGenerator
                .SetupSequence(g => g.CanHaveSpecialMaterial(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>()))
                .Returns(false);

            var decoratedItem = decorator.Generate("power", "item name", "trait 1", "trait 2");
            Assert.That(decoratedItem.Traits, Contains.Item(TraitConstants.SpecialMaterials.Dragonhide));
            Assert.That(decoratedItem.Attributes, Contains.Item("my attribute")
                .And.Not.Contains(AttributeConstants.Wood)
                .And.Not.Contains(AttributeConstants.Metal));
        }

        [Test]
        public void SpecialMaterialIsMasterwork()
        {
            masterworkMaterials.Add("special material");

            mockMaterialGenerator.SetupSequence(g => g.CanHaveSpecialMaterial(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>()))
                .Returns(true)
                .Returns(false);
            mockMaterialGenerator.Setup(g => g.GenerateFor(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>())).Returns("special material");

            var decoratedItem = decorator.GenerateRandom("power");
            Assert.That(decoratedItem.Traits, Contains.Item("special material"));
            Assert.That(decoratedItem.Traits, Contains.Item(TraitConstants.Masterwork));
        }

        [Test]
        public void SpecialMaterialFromItemIsMasterwork()
        {
            masterworkMaterials.Add("special material");

            mockMaterialGenerator.Setup(g => g.CanHaveSpecialMaterial(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>())).Returns(false);
            item.Traits.Add("special material");

            var decoratedItem = decorator.GenerateRandom("power");
            Assert.That(decoratedItem.Traits, Contains.Item("special material"));
            Assert.That(decoratedItem.Traits, Contains.Item(TraitConstants.Masterwork));
        }

        [Test]
        public void SpecialMaterialIsNotMasterwork()
        {
            mockMaterialGenerator.SetupSequence(g => g.CanHaveSpecialMaterial(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>()))
                .Returns(true)
                .Returns(false);
            mockMaterialGenerator.Setup(g => g.GenerateFor(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>())).Returns("special material");

            var decoratedItem = decorator.GenerateRandom("power");
            Assert.That(decoratedItem.Traits, Contains.Item("special material"));
            Assert.That(decoratedItem.Traits, Is.All.Not.EqualTo(TraitConstants.Masterwork));
        }

        [Test]
        public void SpecialMaterialFromItemIsNotMasterwork()
        {
            mockMaterialGenerator.Setup(g => g.CanHaveSpecialMaterial(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>())).Returns(false);
            item.Traits.Add("special material");

            var decoratedItem = decorator.GenerateRandom("power");
            Assert.That(decoratedItem.Traits, Contains.Item("special material"));
            Assert.That(decoratedItem.Traits, Is.All.Not.EqualTo(TraitConstants.Masterwork));
        }
    }
}