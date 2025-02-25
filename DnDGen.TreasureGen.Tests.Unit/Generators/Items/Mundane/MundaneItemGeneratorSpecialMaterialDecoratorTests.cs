using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.TreasureGen.Generators.Items.Mundane;
using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Mundane;
using DnDGen.TreasureGen.Tables;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Collections.Generic;

namespace DnDGen.TreasureGen.Tests.Unit.Generators.Items.Mundane
{
    [TestFixture]
    public class MudnaneItemGeneratorSpecialMaterialDecoratorTests
    {
        private MundaneItemGenerator decorator;
        private Mock<ISpecialMaterialGenerator> mockMaterialGenerator;
        private Mock<MundaneItemGenerator> mockInnerGenerator;
        private Item item;
        private Mock<ICollectionSelector> mockCollectionsSelector;
        private List<string> masterworkMaterials;

        [SetUp]
        public void Setup()
        {
            mockMaterialGenerator = new Mock<ISpecialMaterialGenerator>();
            mockInnerGenerator = new Mock<MundaneItemGenerator>();
            mockCollectionsSelector = new Mock<ICollectionSelector>();
            decorator = new MundaneItemGeneratorSpecialMaterialDecorator(mockInnerGenerator.Object, mockMaterialGenerator.Object, mockCollectionsSelector.Object);

            item = new Item();
            masterworkMaterials = new List<string>();

            mockInnerGenerator.Setup(g => g.GenerateRandom()).Returns(item);
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecialMaterials, TraitConstants.Masterwork))
                .Returns(masterworkMaterials);
        }

        [Test]
        public void GetItemFromInnerGenerator()
        {
            var decoratedItem = decorator.GenerateRandom();
            Assert.That(decoratedItem, Is.EqualTo(item));
        }

        [Test]
        public void DoNotGetSpecialMaterial()
        {
            mockMaterialGenerator.Setup(g => g.CanHaveSpecialMaterial(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>()))
                .Returns(false);
            mockMaterialGenerator.Setup(g => g.GenerateFor(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>()))
                .Returns("special material");

            var decoratedItem = decorator.GenerateRandom();
            Assert.That(decoratedItem.Traits, Is.Not.Contains("special material"));
            Assert.That(decoratedItem.Traits.Count, Is.EqualTo(0));
        }

        [Test]
        public void GetSpecialMaterial()
        {
            mockMaterialGenerator.SetupSequence(g => g.CanHaveSpecialMaterial(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>()))
                .Returns(true).Returns(false);
            mockMaterialGenerator.Setup(g => g.GenerateFor(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>()))
                .Returns("special material");

            var decoratedItem = decorator.GenerateRandom();
            Assert.That(decoratedItem.Traits, Contains.Item("special material"));
            Assert.That(decoratedItem.Traits.Count, Is.EqualTo(1));
        }

        [Test]
        public void GetMultipleSpecialMaterials()
        {
            mockMaterialGenerator.SetupSequence(g => g.CanHaveSpecialMaterial(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>()))
                .Returns(true).Returns(true).Returns(false);
            mockMaterialGenerator.SetupSequence(g => g.GenerateFor(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>()))
                .Returns("special material 1").Returns("special material 2").Returns("special material 3");

            var decoratedItem = decorator.GenerateRandom();
            Assert.That(decoratedItem.Traits, Contains.Item("special material 1"));
            Assert.That(decoratedItem.Traits, Contains.Item("special material 2"));
            Assert.That(decoratedItem.Traits.Count, Is.EqualTo(2));
        }

        [Test]
        public void DragonhideIsNotMetal()
        {
            item.Attributes = new[] { AttributeConstants.Metal };

            mockMaterialGenerator.SetupSequence(g => g.CanHaveSpecialMaterial(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>()))
                .Returns(true).Returns(false);
            mockMaterialGenerator.Setup(g => g.GenerateFor(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>()))
                .Returns(TraitConstants.SpecialMaterials.Dragonhide);

            var decoratedItem = decorator.GenerateRandom();
            Assert.That(decoratedItem.Traits, Contains.Item(TraitConstants.SpecialMaterials.Dragonhide));
            Assert.That(decoratedItem.Attributes, Is.All.Not.EqualTo(AttributeConstants.Metal));
        }

        [Test]
        public void DragonhideIsNotWood()
        {
            item.Attributes = new[] { AttributeConstants.Wood };

            mockMaterialGenerator.SetupSequence(g => g.CanHaveSpecialMaterial(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>()))
                .Returns(true).Returns(false);
            mockMaterialGenerator.Setup(g => g.GenerateFor(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>()))
                .Returns(TraitConstants.SpecialMaterials.Dragonhide);

            var decoratedItem = decorator.GenerateRandom();
            Assert.That(decoratedItem.Traits, Contains.Item(TraitConstants.SpecialMaterials.Dragonhide));
            Assert.That(decoratedItem.Attributes, Is.All.Not.EqualTo(AttributeConstants.Wood));
        }

        [Test]
        public void DecorateCustomItem()
        {
            var template = new Item();

            mockInnerGenerator.Setup(g => g.Generate(template, true)).Returns(item);
            mockMaterialGenerator.SetupSequence(g => g.CanHaveSpecialMaterial(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>()))
                .Returns(true).Returns(false);
            mockMaterialGenerator.Setup(g => g.GenerateFor(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>()))
                .Returns("special material");

            var decoratedItem = decorator.Generate(template, true);
            Assert.That(decoratedItem, Is.Not.EqualTo(template));
            Assert.That(decoratedItem, Is.EqualTo(item));
            Assert.That(decoratedItem.Traits, Contains.Item("special material"));
            Assert.That(decoratedItem.Traits.Count, Is.EqualTo(1));
        }

        [Test]
        public void DoNotDecorateCustomItem()
        {
            var template = new Item();

            mockInnerGenerator.Setup(g => g.Generate(template, false)).Returns(item);
            mockMaterialGenerator.SetupSequence(g => g.CanHaveSpecialMaterial(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>()))
                .Returns(true).Returns(false);
            mockMaterialGenerator.Setup(g => g.GenerateFor(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>()))
                .Returns("special material");

            var decoratedItem = decorator.Generate(template);
            Assert.That(decoratedItem, Is.Not.SameAs(template));
            Assert.That(JsonConvert.SerializeObject(decoratedItem), Is.EqualTo(JsonConvert.SerializeObject(template)));
            Assert.That(decoratedItem, Is.EqualTo(item));
            Assert.That(decoratedItem.Traits, Is.Empty);
        }

        [Test]
        public void GenerateFromName()
        {
            mockMaterialGenerator.Setup(g => g.CanHaveSpecialMaterial(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>())).Returns(false);
            mockMaterialGenerator.Setup(g => g.GenerateFor(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>())).Returns("special material");

            mockInnerGenerator.Setup(g => g.Generate("item name")).Returns(item);

            var decoratedItem = decorator.Generate("item name");
            Assert.That(decoratedItem, Is.EqualTo(item));
            Assert.That(decoratedItem.Traits, Is.Not.Contains("special material"));
            Assert.That(decoratedItem.Traits.Count, Is.EqualTo(0));
        }

        [Test]
        public void GetSpecialMaterialFromName()
        {
            mockMaterialGenerator.SetupSequence(g => g.CanHaveSpecialMaterial(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>()))
                .Returns(true).Returns(false);
            mockMaterialGenerator.Setup(g => g.GenerateFor(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>()))
                .Returns("special material");

            mockInnerGenerator.Setup(g => g.Generate("item name")).Returns(item);

            var decoratedItem = decorator.Generate("item name");
            Assert.That(decoratedItem.Traits, Contains.Item("special material"));
            Assert.That(decoratedItem.Traits.Count, Is.EqualTo(1));
        }

        [Test]
        public void GetMultipleSpecialMaterialsFromName()
        {
            mockMaterialGenerator.SetupSequence(g => g.CanHaveSpecialMaterial(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>()))
                .Returns(true).Returns(true).Returns(false);
            mockMaterialGenerator.SetupSequence(g => g.GenerateFor(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>()))
                .Returns("special material 1").Returns("special material 2").Returns("special material 3");

            mockInnerGenerator.Setup(g => g.Generate("item name")).Returns(item);

            var decoratedItem = decorator.Generate("item name");
            Assert.That(decoratedItem.Traits, Contains.Item("special material 1"));
            Assert.That(decoratedItem.Traits, Contains.Item("special material 2"));
            Assert.That(decoratedItem.Traits.Count, Is.EqualTo(2));
        }

        [Test]
        public void DragonhideFromNameIsNotMetal()
        {
            item.Attributes = new[] { AttributeConstants.Metal };

            mockMaterialGenerator.SetupSequence(g => g.CanHaveSpecialMaterial(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>()))
                .Returns(true).Returns(false);
            mockMaterialGenerator.Setup(g => g.GenerateFor(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>()))
                .Returns(TraitConstants.SpecialMaterials.Dragonhide);

            mockInnerGenerator.Setup(g => g.Generate("item name")).Returns(item);

            var decoratedItem = decorator.Generate("item name");
            Assert.That(decoratedItem.Traits, Contains.Item(TraitConstants.SpecialMaterials.Dragonhide));
            Assert.That(decoratedItem.Attributes, Is.All.Not.EqualTo(AttributeConstants.Metal));
        }

        [Test]
        public void DragonhideFromNameIsNotWood()
        {
            item.Attributes = new[] { AttributeConstants.Wood };

            mockMaterialGenerator.SetupSequence(g => g.CanHaveSpecialMaterial(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>()))
                .Returns(true).Returns(false);
            mockMaterialGenerator.Setup(g => g.GenerateFor(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>()))
                .Returns(TraitConstants.SpecialMaterials.Dragonhide);

            mockInnerGenerator.Setup(g => g.Generate("item name")).Returns(item);

            var decoratedItem = decorator.Generate("item name");
            Assert.That(decoratedItem.Traits, Contains.Item(TraitConstants.SpecialMaterials.Dragonhide));
            Assert.That(decoratedItem.Attributes, Is.All.Not.EqualTo(AttributeConstants.Wood));
        }

        [Test]
        public void SpecialMaterialIsMasterwork()
        {
            masterworkMaterials.Add("special material");

            mockMaterialGenerator.SetupSequence(g => g.CanHaveSpecialMaterial(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>()))
                .Returns(true)
                .Returns(false);
            mockMaterialGenerator.Setup(g => g.GenerateFor(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>())).Returns("special material");

            var decoratedItem = decorator.GenerateRandom();
            Assert.That(decoratedItem.Traits, Contains.Item("special material"));
            Assert.That(decoratedItem.Traits, Contains.Item(TraitConstants.Masterwork));
        }

        [Test]
        public void SpecialMaterialFromItemIsMasterwork()
        {
            masterworkMaterials.Add("special material");

            mockMaterialGenerator.Setup(g => g.CanHaveSpecialMaterial(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>())).Returns(false);
            item.Traits.Add("special material");

            var decoratedItem = decorator.GenerateRandom();
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

            var decoratedItem = decorator.GenerateRandom();
            Assert.That(decoratedItem.Traits, Contains.Item("special material"));
            Assert.That(decoratedItem.Traits, Is.All.Not.EqualTo(TraitConstants.Masterwork));
        }

        [Test]
        public void SpecialMaterialFromItemIsNotMasterwork()
        {
            mockMaterialGenerator.Setup(g => g.CanHaveSpecialMaterial(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>())).Returns(false);
            item.Traits.Add("special material");

            var decoratedItem = decorator.GenerateRandom();
            Assert.That(decoratedItem.Traits, Contains.Item("special material"));
            Assert.That(decoratedItem.Traits, Is.All.Not.EqualTo(TraitConstants.Masterwork));
        }
    }
}