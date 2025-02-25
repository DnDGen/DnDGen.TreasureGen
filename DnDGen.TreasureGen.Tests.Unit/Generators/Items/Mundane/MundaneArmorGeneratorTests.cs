using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.TreasureGen.Generators.Items.Mundane;
using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Mundane;
using DnDGen.TreasureGen.Selectors.Percentiles;
using DnDGen.TreasureGen.Selectors.Selections;
using DnDGen.TreasureGen.Tables;
using Moq;
using NUnit.Framework;
using System;

namespace DnDGen.TreasureGen.Tests.Unit.Generators.Items.Mundane
{
    [TestFixture]
    public class MundaneArmorGeneratorTests
    {
        private MundaneItemGenerator mundaneArmorGenerator;
        private Mock<ITreasurePercentileSelector> mockPercentileSelector;
        private Mock<ICollectionSelector> mockCollectionsSelector;
        private Mock<ICollectionDataSelector<ArmorDataSelection>> mockArmorDataSelector;
        private ItemVerifier itemVerifier;
        private ArmorDataSelection armorSelection;

        [SetUp]
        public void Setup()
        {
            mockPercentileSelector = new Mock<ITreasurePercentileSelector>();
            mockCollectionsSelector = new Mock<ICollectionSelector>();
            mockArmorDataSelector = new Mock<ICollectionDataSelector<ArmorDataSelection>>();
            mundaneArmorGenerator = new MundaneArmorGenerator(mockPercentileSelector.Object, mockCollectionsSelector.Object, mockArmorDataSelector.Object);
            itemVerifier = new ItemVerifier();
            armorSelection = new ArmorDataSelection();

            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.MundaneArmors)).Returns("armor type");
            mockArmorDataSelector.Setup(s => s.SelectOneFrom(Config.Name, TableNameConstants.Collections.ArmorData, "armor type")).Returns(armorSelection);
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, TableNameConstants.Percentiles.MundaneGearSizes)).Returns("size");
        }

        [Test]
        public void ReturnArmor()
        {
            var armor = mundaneArmorGenerator.GenerateRandom();
            Assert.That(armor, Is.Not.Null);
            Assert.That(armor, Is.InstanceOf<Armor>());
        }

        [Test]
        public void GetArmorTypeFromPercentileSelector()
        {
            var armor = mundaneArmorGenerator.GenerateRandom();
            Assert.That(armor.Name, Is.EqualTo("armor type"));
        }

        [Test]
        public void GetArmorBaseNames()
        {
            var baseNames = new[] { "base name", "other base name" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.ItemGroups, "armor type")).Returns(baseNames);

            var armor = mundaneArmorGenerator.GenerateRandom();
            Assert.That(armor.BaseNames, Is.EqualTo(baseNames));
        }

        [Test]
        public void GetArmorBonus()
        {
            armorSelection.ArmorBonus = 9266;

            var item = mundaneArmorGenerator.GenerateRandom();
            var armor = item as Armor;

            Assert.That(armor.ArmorBonus, Is.EqualTo(9266));
        }

        [Test]
        public void GetShieldArmorBonus()
        {
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, TableNameConstants.Percentiles.MundaneArmors)).Returns(AttributeConstants.Shield);
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, TableNameConstants.Percentiles.MundaneShields)).Returns("big shield");
            mockArmorDataSelector.Setup(s => s.SelectOneFrom(Config.Name, TableNameConstants.Collections.ArmorData, "big shield")).Returns(armorSelection);

            armorSelection.ArmorBonus = 9266;

            var item = mundaneArmorGenerator.GenerateRandom();
            var armor = item as Armor;

            Assert.That(armor.Name, Is.EqualTo("big shield"));
            Assert.That(armor.ArmorBonus, Is.EqualTo(9266));
        }

        [Test]
        public void GetArmorCheckPenalty()
        {
            armorSelection.ArmorCheckPenalty = -9266;

            var item = mundaneArmorGenerator.GenerateRandom();
            var armor = item as Armor;

            Assert.That(armor.ArmorCheckPenalty, Is.EqualTo(-9266));
        }

        [Test]
        public void GetShieldArmorCheckPenalty()
        {
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, TableNameConstants.Percentiles.MundaneArmors)).Returns(AttributeConstants.Shield);
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, TableNameConstants.Percentiles.MundaneShields)).Returns("big shield");
            mockArmorDataSelector.Setup(s => s.SelectOneFrom(Config.Name, TableNameConstants.Collections.ArmorData, "big shield")).Returns(armorSelection);

            armorSelection.ArmorCheckPenalty = -9266;

            var item = mundaneArmorGenerator.GenerateRandom();
            var armor = item as Armor;

            Assert.That(armor.Name, Is.EqualTo("big shield"));
            Assert.That(armor.ArmorCheckPenalty, Is.EqualTo(-9266));
        }

        [Test]
        public void GetMaxDexterityBonus()
        {
            armorSelection.MaxDexterityBonus = 9266;

            var item = mundaneArmorGenerator.GenerateRandom();
            var armor = item as Armor;

            Assert.That(armor.MaxDexterityBonus, Is.EqualTo(9266));
        }

        [Test]
        public void GetShieldMaxDexterityBonus()
        {
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, TableNameConstants.Percentiles.MundaneArmors)).Returns(AttributeConstants.Shield);
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, TableNameConstants.Percentiles.MundaneShields)).Returns("big shield");
            mockArmorDataSelector.Setup(s => s.SelectOneFrom(Config.Name, TableNameConstants.Collections.ArmorData, "big shield")).Returns(armorSelection);

            armorSelection.MaxDexterityBonus = 9266;

            var item = mundaneArmorGenerator.GenerateRandom();
            var armor = item as Armor;

            Assert.That(armor.Name, Is.EqualTo("big shield"));
            Assert.That(armor.MaxDexterityBonus, Is.EqualTo(9266));
        }

        [Test]
        public void SetMasterworkTraitIfMasterwork()
        {
            mockPercentileSelector.Setup(p => p.SelectFrom<bool>(Config.Name, TableNameConstants.Percentiles.IsMasterwork)).Returns(true);

            var armor = mundaneArmorGenerator.GenerateRandom();
            Assert.That(armor.Traits, Contains.Item(TraitConstants.Masterwork));
        }

        [Test]
        public void DoNotSetMasterworkTraitIfNotMasterwork()
        {
            mockPercentileSelector.Setup(p => p.SelectFrom<bool>(Config.Name, TableNameConstants.Percentiles.IsMasterwork)).Returns(false);

            var armor = mundaneArmorGenerator.GenerateRandom();
            Assert.That(armor.Traits, Is.All.Not.EqualTo(TraitConstants.Masterwork));
        }

        [Test]
        public void GetShieldTypeIfResultIsShield()
        {
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, TableNameConstants.Percentiles.MundaneArmors)).Returns(AttributeConstants.Shield);
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, TableNameConstants.Percentiles.MundaneShields)).Returns("big shield");
            mockArmorDataSelector.Setup(s => s.SelectOneFrom(Config.Name, TableNameConstants.Collections.ArmorData, "big shield")).Returns(armorSelection);

            var shield = mundaneArmorGenerator.GenerateRandom();
            Assert.That(shield.Name, Is.EqualTo("big shield"));
        }

        [Test]
        public void GetShieldBaseNames()
        {
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, TableNameConstants.Percentiles.MundaneArmors)).Returns(AttributeConstants.Shield);
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, TableNameConstants.Percentiles.MundaneShields)).Returns("big shield");
            mockArmorDataSelector.Setup(s => s.SelectOneFrom(Config.Name, TableNameConstants.Collections.ArmorData, "big shield")).Returns(armorSelection);

            var baseNames = new[] { "base name", "other base name" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.ItemGroups, "big shield")).Returns(baseNames);

            var shield = mundaneArmorGenerator.GenerateRandom();
            Assert.That(shield.BaseNames, Is.EqualTo(baseNames));
        }

        [Test]
        public void GetAttributesFromSelector()
        {
            var attributes = new[] { "attribute 1", "attribute 2" };
            var tableName = TableNameConstants.Collections.ITEMTYPEAttributes(ItemTypeConstants.Armor);
            mockCollectionsSelector.Setup(p => p.SelectFrom(Config.Name, tableName, "armor type")).Returns(attributes);

            var armor = mundaneArmorGenerator.GenerateRandom();
            Assert.That(armor.Attributes, Is.EqualTo(attributes));
        }

        [Test]
        public void GenerateSizeFromPercentileSelector()
        {

            var item = mundaneArmorGenerator.GenerateRandom();
            var armor = item as Armor;

            Assert.That(armor.Traits, Is.All.Not.EqualTo("size"));
            Assert.That(armor.Size, Is.EqualTo("size"));
        }

        [Test]
        public void GetAttributesForMundaneShields()
        {
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, TableNameConstants.Percentiles.MundaneArmors)).Returns(AttributeConstants.Shield);
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, TableNameConstants.Percentiles.MundaneShields)).Returns("big shield");
            mockArmorDataSelector.Setup(s => s.SelectOneFrom(Config.Name, TableNameConstants.Collections.ArmorData, "big shield")).Returns(armorSelection);

            var attributes = new[] { "attribute 1", "attribute 2" };
            var tableName = TableNameConstants.Collections.ITEMTYPEAttributes(ItemTypeConstants.Armor);
            mockCollectionsSelector.Setup(p => p.SelectFrom(Config.Name, tableName, "big shield")).Returns(attributes);

            var armor = mundaneArmorGenerator.GenerateRandom();
            Assert.That(armor.Name, Is.EqualTo("big shield"));
            Assert.That(armor.Attributes, Is.EqualTo(attributes));
        }

        [Test]
        public void GetSizesForMundaneShields()
        {
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, TableNameConstants.Percentiles.MundaneArmors)).Returns(AttributeConstants.Shield);
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, TableNameConstants.Percentiles.MundaneShields)).Returns("big shield");
            mockArmorDataSelector.Setup(s => s.SelectOneFrom(Config.Name, TableNameConstants.Collections.ArmorData, "big shield")).Returns(armorSelection);

            var item = mundaneArmorGenerator.GenerateRandom();
            var armor = item as Armor;

            Assert.That(armor.Name, Is.EqualTo("big shield"));
            Assert.That(armor.Traits, Is.All.Not.EqualTo("size"));
            Assert.That(armor.Size, Is.EqualTo("size"));
        }

        [Test]
        public void GetMasterworkMundaneShield()
        {
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, TableNameConstants.Percentiles.MundaneArmors)).Returns(AttributeConstants.Shield);
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, TableNameConstants.Percentiles.MundaneShields)).Returns("big shield");
            mockPercentileSelector.Setup(p => p.SelectFrom<bool>(Config.Name, TableNameConstants.Percentiles.IsMasterwork)).Returns(true);
            mockArmorDataSelector.Setup(s => s.SelectOneFrom(Config.Name, TableNameConstants.Collections.ArmorData, "big shield")).Returns(armorSelection);

            var armor = mundaneArmorGenerator.GenerateRandom();
            Assert.That(armor.Name, Is.EqualTo("big shield"));
            Assert.That(armor.Traits, Contains.Item(TraitConstants.Masterwork));
        }

        [Test]
        public void DoNotGetMasterworkMundaneShield()
        {
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, TableNameConstants.Percentiles.MundaneArmors)).Returns(AttributeConstants.Shield);
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, TableNameConstants.Percentiles.MundaneShields)).Returns("big shield");
            mockPercentileSelector.Setup(p => p.SelectFrom<bool>(Config.Name, TableNameConstants.Percentiles.IsMasterwork)).Returns(false);
            mockArmorDataSelector.Setup(s => s.SelectOneFrom(Config.Name, TableNameConstants.Collections.ArmorData, "big shield")).Returns(armorSelection);

            var armor = mundaneArmorGenerator.GenerateRandom();
            Assert.That(armor.Name, Is.EqualTo("big shield"));
            Assert.That(armor.Traits, Is.Not.Contains(TraitConstants.Masterwork));
        }

        [Test]
        public void GenerateCustomMundaneArmor()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomArmorTemplate(name);

            var attributes = new[] { "attribute 1", "attribute 2" };
            var tableName = TableNameConstants.Collections.ITEMTYPEAttributes(ItemTypeConstants.Armor);
            mockCollectionsSelector.Setup(p => p.SelectFrom(Config.Name, tableName, name)).Returns(attributes);

            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, TableNameConstants.Percentiles.MundaneGearSizes)).Returns("size");
            mockPercentileSelector.Setup(p => p.SelectFrom<bool>(Config.Name, TableNameConstants.Percentiles.IsMasterwork)).Returns(true);

            var baseNames = new[] { "base name", "other base name" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.ItemGroups, name)).Returns(baseNames);

            armorSelection.ArmorBonus = 9266;
            armorSelection.ArmorCheckPenalty = -90210;
            armorSelection.MaxDexterityBonus = 42;
            mockArmorDataSelector.Setup(s => s.SelectOneFrom(Config.Name, TableNameConstants.Collections.ArmorData, name)).Returns(armorSelection);

            var item = mundaneArmorGenerator.Generate(template);
            var armor = item as Armor;

            itemVerifier.AssertMundaneItemFromTemplate(armor, template);
            Assert.That(armor.ItemType, Is.EqualTo(ItemTypeConstants.Armor));
            Assert.That(armor.Attributes, Is.EquivalentTo(attributes));
            Assert.That(armor.Traits, Is.All.Not.EqualTo("size"));
            Assert.That(armor.Size, Is.EqualTo("size"));
            Assert.That(armor.Traits, Is.All.Not.EqualTo(TraitConstants.Masterwork));
            Assert.That(armor.Quantity, Is.EqualTo(1));
            Assert.That(armor.BaseNames, Is.EqualTo(baseNames));
            Assert.That(armor.ArmorBonus, Is.EqualTo(9266));
            Assert.That(armor.ArmorCheckPenalty, Is.EqualTo(-90210));
            Assert.That(armor.MaxDexterityBonus, Is.EqualTo(42));
        }

        [Test]
        public void GenerateCustomMundaneArmorFromNonArmorTemplate()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);

            var attributes = new[] { "attribute 1", "attribute 2" };
            var tableName = TableNameConstants.Collections.ITEMTYPEAttributes(ItemTypeConstants.Armor);
            mockCollectionsSelector.Setup(p => p.SelectFrom(Config.Name, tableName, name)).Returns(attributes);

            mockPercentileSelector.Setup(p => p.SelectFrom<bool>(Config.Name, TableNameConstants.Percentiles.IsMasterwork)).Returns(true);

            var baseNames = new[] { "base name", "other base name" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.ItemGroups, name)).Returns(baseNames);

            armorSelection.ArmorBonus = 9266;
            armorSelection.ArmorCheckPenalty = -90210;
            armorSelection.MaxDexterityBonus = 42;
            mockArmorDataSelector.Setup(s => s.SelectOneFrom(Config.Name, TableNameConstants.Collections.ArmorData, name)).Returns(armorSelection);

            var item = mundaneArmorGenerator.Generate(template);
            var armor = item as Armor;

            itemVerifier.AssertMundaneItemFromTemplate(armor, template);
            Assert.That(armor.ItemType, Is.EqualTo(ItemTypeConstants.Armor));
            Assert.That(armor.Attributes, Is.EquivalentTo(attributes));
            Assert.That(armor.Traits, Is.All.Not.EqualTo("size"));
            Assert.That(armor.Size, Is.EqualTo("size"));
            Assert.That(armor.Traits, Is.All.Not.EqualTo(TraitConstants.Masterwork));
            Assert.That(armor.Quantity, Is.EqualTo(1));
            Assert.That(armor.BaseNames, Is.EqualTo(baseNames));
            Assert.That(armor.ArmorBonus, Is.EqualTo(9266));
            Assert.That(armor.ArmorCheckPenalty, Is.EqualTo(-90210));
            Assert.That(armor.MaxDexterityBonus, Is.EqualTo(42));
        }

        [Test]
        public void GenerateRandomCustomMundaneArmor()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomArmorTemplate(name);

            var attributes = new[] { "attribute 1", "attribute 2" };
            var tableName = TableNameConstants.Collections.ITEMTYPEAttributes(ItemTypeConstants.Armor);
            mockCollectionsSelector.Setup(p => p.SelectFrom(Config.Name, tableName, name)).Returns(attributes);

            mockPercentileSelector.Setup(p => p.SelectFrom<bool>(Config.Name, TableNameConstants.Percentiles.IsMasterwork)).Returns(true);

            var baseNames = new[] { "base name", "other base name" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.ItemGroups, name)).Returns(baseNames);

            armorSelection.ArmorBonus = 9266;
            armorSelection.ArmorCheckPenalty = -90210;
            armorSelection.MaxDexterityBonus = 42;
            mockArmorDataSelector.Setup(s => s.SelectOneFrom(Config.Name, TableNameConstants.Collections.ArmorData, name)).Returns(armorSelection);

            var item = mundaneArmorGenerator.Generate(template, true);
            var armor = item as Armor;

            itemVerifier.AssertMundaneItemFromTemplate(armor, template);
            Assert.That(armor.ItemType, Is.EqualTo(ItemTypeConstants.Armor));
            Assert.That(armor.Attributes, Is.EquivalentTo(attributes));
            Assert.That(armor.Traits, Is.All.Not.EqualTo("size"));
            Assert.That(armor.Size, Is.EqualTo("size"));
            Assert.That(armor.Traits, Contains.Item(TraitConstants.Masterwork));
            Assert.That(armor.Quantity, Is.EqualTo(1));
            Assert.That(armor.BaseNames, Is.EqualTo(baseNames));
            Assert.That(armor.ArmorBonus, Is.EqualTo(9266));
            Assert.That(armor.ArmorCheckPenalty, Is.EqualTo(-90210));
            Assert.That(armor.MaxDexterityBonus, Is.EqualTo(42));
        }

        [Test]
        public void DoNotAddSizeToCustomItemIfItAlreadyHasOne()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomArmorTemplate(name);
            template.Traits.Add("custom size");

            var attributes = new[] { "attribute 1", "attribute 2" };
            var tableName = TableNameConstants.Collections.ITEMTYPEAttributes(ItemTypeConstants.Armor);
            mockCollectionsSelector.Setup(p => p.SelectFrom(Config.Name, tableName, name)).Returns(attributes);

            var sizes = new[] { "size", "custom size", "other size" };
            mockPercentileSelector.Setup(s => s.SelectAllFrom(Config.Name, TableNameConstants.Percentiles.MundaneGearSizes)).Returns(sizes);
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, TableNameConstants.Percentiles.MundaneGearSizes)).Returns("other size");
            mockPercentileSelector.Setup(p => p.SelectFrom<bool>(Config.Name, TableNameConstants.Percentiles.IsMasterwork)).Returns(true);

            var baseNames = new[] { "base name", "other base name" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.ItemGroups, name)).Returns(baseNames);

            armorSelection.ArmorBonus = 9266;
            armorSelection.ArmorCheckPenalty = -90210;
            armorSelection.MaxDexterityBonus = 42;
            mockArmorDataSelector.Setup(s => s.SelectOneFrom(Config.Name, TableNameConstants.Collections.ArmorData, name)).Returns(armorSelection);

            var item = mundaneArmorGenerator.Generate(template);
            var armor = item as Armor;

            itemVerifier.AssertMundaneItemFromTemplate(armor, template);
            Assert.That(armor.ItemType, Is.EqualTo(ItemTypeConstants.Armor));
            Assert.That(armor.Attributes, Is.EquivalentTo(attributes));
            Assert.That(armor.Traits, Is.All.Not.EqualTo("size"));
            Assert.That(armor.Traits, Is.All.Not.EqualTo("custom size"));
            Assert.That(armor.Traits, Is.All.Not.EqualTo("other size"));
            Assert.That(armor.Size, Is.EqualTo("custom size"));
            Assert.That(armor.Traits, Is.All.Not.EqualTo(TraitConstants.Masterwork));
            Assert.That(armor.Quantity, Is.EqualTo(1));
            Assert.That(armor.BaseNames, Is.EqualTo(baseNames));
            Assert.That(armor.ArmorBonus, Is.EqualTo(9266));
            Assert.That(armor.ArmorCheckPenalty, Is.EqualTo(-90210));
            Assert.That(armor.MaxDexterityBonus, Is.EqualTo(42));
        }

        [Test]
        public void DoNotAddSizeToCustomArmorIfItAlreadyHasOne()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomArmorTemplate(name);
            template.Size = "custom size";

            var attributes = new[] { "attribute 1", "attribute 2" };
            var tableName = TableNameConstants.Collections.ITEMTYPEAttributes(ItemTypeConstants.Armor);
            mockCollectionsSelector.Setup(p => p.SelectFrom(Config.Name, tableName, name)).Returns(attributes);

            var sizes = new[] { "size", "custom size", "other size" };
            mockPercentileSelector.Setup(s => s.SelectAllFrom(Config.Name, TableNameConstants.Percentiles.MundaneGearSizes)).Returns(sizes);
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, TableNameConstants.Percentiles.MundaneGearSizes)).Returns("other size");
            mockPercentileSelector.Setup(p => p.SelectFrom<bool>(Config.Name, TableNameConstants.Percentiles.IsMasterwork)).Returns(true);

            var baseNames = new[] { "base name", "other base name" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.ItemGroups, name)).Returns(baseNames);

            armorSelection.ArmorBonus = 9266;
            armorSelection.ArmorCheckPenalty = -90210;
            armorSelection.MaxDexterityBonus = 42;
            mockArmorDataSelector.Setup(s => s.SelectOneFrom(Config.Name, TableNameConstants.Collections.ArmorData, name)).Returns(armorSelection);

            var item = mundaneArmorGenerator.Generate(template);
            var armor = item as Armor;

            itemVerifier.AssertMundaneItemFromTemplate(armor, template);
            Assert.That(armor.ItemType, Is.EqualTo(ItemTypeConstants.Armor));
            Assert.That(armor.Attributes, Is.EquivalentTo(attributes));
            Assert.That(armor.Traits, Is.All.Not.EqualTo("size"));
            Assert.That(armor.Traits, Is.All.Not.EqualTo("custom size"));
            Assert.That(armor.Traits, Is.All.Not.EqualTo("other size"));
            Assert.That(armor.Size, Is.EqualTo("custom size"));
            Assert.That(armor.Traits, Is.All.Not.EqualTo(TraitConstants.Masterwork));
            Assert.That(armor.Quantity, Is.EqualTo(1));
            Assert.That(armor.BaseNames, Is.EqualTo(baseNames));
            Assert.That(armor.ArmorBonus, Is.EqualTo(9266));
            Assert.That(armor.ArmorCheckPenalty, Is.EqualTo(-90210));
            Assert.That(armor.MaxDexterityBonus, Is.EqualTo(42));
        }

        [Test]
        public void GenerateFromName()
        {
            mockPercentileSelector.SetupSequence(p => p.SelectFrom(Config.Name, TableNameConstants.Percentiles.MundaneArmors))
                .Returns("wrong armor")
                .Returns("armor")
                .Returns("other armor");

            var baseNames = new[] { "base name", "other base name" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.ItemGroups, "armor")).Returns(baseNames);

            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, TableNameConstants.Percentiles.MundaneGearSizes)).Returns("size");
            mockPercentileSelector.Setup(p => p.SelectFrom<bool>(Config.Name, TableNameConstants.Percentiles.IsMasterwork)).Returns(false);

            var attributes = new[] { "attribute 1", "attribute 2" };
            var tableName = TableNameConstants.Collections.ITEMTYPEAttributes(ItemTypeConstants.Armor);
            mockCollectionsSelector.Setup(p => p.SelectFrom(Config.Name, tableName, "armor")).Returns(attributes);

            armorSelection.ArmorBonus = 9266;
            armorSelection.ArmorCheckPenalty = -90210;
            armorSelection.MaxDexterityBonus = 42;
            mockArmorDataSelector.Setup(s => s.SelectOneFrom(Config.Name, TableNameConstants.Collections.ArmorData, "armor")).Returns(armorSelection);
            mockArmorDataSelector.Setup(s => s.SelectOneFrom(Config.Name, TableNameConstants.Collections.ArmorData, "wrong armor")).Returns(new ArmorDataSelection());

            var item = mundaneArmorGenerator.Generate("armor");
            var armor = item as Armor;

            Assert.That(armor, Is.Not.Null);
            Assert.That(armor.Name, Is.EqualTo("armor"));
            Assert.That(armor.BaseNames, Is.EqualTo(baseNames));
            Assert.That(armor.Traits, Is.All.Not.EqualTo(TraitConstants.Masterwork));
            Assert.That(armor.Traits, Is.All.Not.EqualTo("size"));
            Assert.That(armor.Size, Is.EqualTo("size"));
            Assert.That(armor.Attributes, Is.EqualTo(attributes));
            Assert.That(armor.ArmorBonus, Is.EqualTo(9266));
            Assert.That(armor.ArmorCheckPenalty, Is.EqualTo(-90210));
            Assert.That(armor.MaxDexterityBonus, Is.EqualTo(42));
        }

        [Test]
        public void GenerateMasterworkFromName()
        {
            mockPercentileSelector.SetupSequence(p => p.SelectFrom(Config.Name, TableNameConstants.Percentiles.MundaneArmors))
                .Returns("wrong armor")
                .Returns("armor")
                .Returns("other armor");

            var baseNames = new[] { "base name", "other base name" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.ItemGroups, "armor")).Returns(baseNames);

            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, TableNameConstants.Percentiles.MundaneGearSizes)).Returns("size");
            mockPercentileSelector.Setup(p => p.SelectFrom<bool>(Config.Name, TableNameConstants.Percentiles.IsMasterwork)).Returns(true);

            var attributes = new[] { "attribute 1", "attribute 2" };
            var tableName = TableNameConstants.Collections.ITEMTYPEAttributes(ItemTypeConstants.Armor);
            mockCollectionsSelector.Setup(p => p.SelectFrom(Config.Name, tableName, "armor")).Returns(attributes);

            armorSelection.ArmorBonus = 9266;
            armorSelection.ArmorCheckPenalty = -90210;
            armorSelection.MaxDexterityBonus = 42;
            mockArmorDataSelector.Setup(s => s.SelectOneFrom(Config.Name, TableNameConstants.Collections.ArmorData, "armor")).Returns(armorSelection);
            mockArmorDataSelector.Setup(s => s.SelectOneFrom(Config.Name, TableNameConstants.Collections.ArmorData, "wrong armor")).Returns(new ArmorDataSelection());

            var item = mundaneArmorGenerator.Generate("armor");
            var armor = item as Armor;

            Assert.That(armor, Is.Not.Null);
            Assert.That(armor.Name, Is.EqualTo("armor"));
            Assert.That(armor.BaseNames, Is.EqualTo(baseNames));
            Assert.That(armor.Traits, Contains.Item(TraitConstants.Masterwork));
            Assert.That(armor.Traits, Is.All.Not.EqualTo("size"));
            Assert.That(armor.Size, Is.EqualTo("size"));
            Assert.That(armor.Attributes, Is.EqualTo(attributes));
            Assert.That(armor.ArmorBonus, Is.EqualTo(9266));
            Assert.That(armor.ArmorCheckPenalty, Is.EqualTo(-90210));
            Assert.That(armor.MaxDexterityBonus, Is.EqualTo(42));
        }

        [Test]
        public void GenerateShieldFromName()
        {
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, TableNameConstants.Percentiles.MundaneArmors)).Returns(AttributeConstants.Shield);
            mockPercentileSelector.SetupSequence(p => p.SelectFrom(Config.Name, TableNameConstants.Percentiles.MundaneShields))
                .Returns("wrong shield")
                .Returns("shield")
                .Returns("other shield");

            var baseNames = new[] { "base name", "other base name" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.ItemGroups, "shield")).Returns(baseNames);

            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, TableNameConstants.Percentiles.MundaneGearSizes)).Returns("size");
            mockPercentileSelector.Setup(p => p.SelectFrom<bool>(Config.Name, TableNameConstants.Percentiles.IsMasterwork)).Returns(false);

            var attributes = new[] { "attribute 1", "attribute 2" };
            var tableName = TableNameConstants.Collections.ITEMTYPEAttributes(ItemTypeConstants.Armor);
            mockCollectionsSelector.Setup(p => p.SelectFrom(Config.Name, tableName, "shield")).Returns(attributes);

            armorSelection.ArmorBonus = 9266;
            armorSelection.ArmorCheckPenalty = -90210;
            armorSelection.MaxDexterityBonus = 42;
            mockArmorDataSelector.Setup(s => s.SelectOneFrom(Config.Name, TableNameConstants.Collections.ArmorData, "shield")).Returns(armorSelection);
            mockArmorDataSelector.Setup(s => s.SelectOneFrom(Config.Name, TableNameConstants.Collections.ArmorData, "wrong shield")).Returns(new ArmorDataSelection());

            var item = mundaneArmorGenerator.Generate("shield");
            var armor = item as Armor;

            Assert.That(armor, Is.Not.Null);
            Assert.That(armor.Name, Is.EqualTo("shield"));
            Assert.That(armor.BaseNames, Is.EqualTo(baseNames));
            Assert.That(armor.Traits, Is.All.Not.EqualTo(TraitConstants.Masterwork));
            Assert.That(armor.Traits, Is.All.Not.EqualTo("size"));
            Assert.That(armor.Size, Is.EqualTo("size"));
            Assert.That(armor.Attributes, Is.EqualTo(attributes));
            Assert.That(armor.ArmorBonus, Is.EqualTo(9266));
            Assert.That(armor.ArmorCheckPenalty, Is.EqualTo(-90210));
            Assert.That(armor.MaxDexterityBonus, Is.EqualTo(42));
        }

        [Test]
        public void GenerateMasterworkShieldFromName()
        {
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, TableNameConstants.Percentiles.MundaneArmors)).Returns(AttributeConstants.Shield);
            mockPercentileSelector.SetupSequence(p => p.SelectFrom(Config.Name, TableNameConstants.Percentiles.MundaneShields))
                .Returns("wrong shield")
                .Returns("shield")
                .Returns("other shield");

            var baseNames = new[] { "base name", "other base name" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.ItemGroups, "shield")).Returns(baseNames);

            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, TableNameConstants.Percentiles.MundaneGearSizes)).Returns("size");
            mockPercentileSelector.Setup(p => p.SelectFrom<bool>(Config.Name, TableNameConstants.Percentiles.IsMasterwork)).Returns(true);

            var attributes = new[] { "attribute 1", "attribute 2" };
            var tableName = TableNameConstants.Collections.ITEMTYPEAttributes(ItemTypeConstants.Armor);
            mockCollectionsSelector.Setup(p => p.SelectFrom(Config.Name, tableName, "shield")).Returns(attributes);

            armorSelection.ArmorBonus = 9266;
            armorSelection.ArmorCheckPenalty = -90210;
            armorSelection.MaxDexterityBonus = 42;
            mockArmorDataSelector.Setup(s => s.SelectOneFrom(Config.Name, TableNameConstants.Collections.ArmorData, "shield")).Returns(armorSelection);
            mockArmorDataSelector.Setup(s => s.SelectOneFrom(Config.Name, TableNameConstants.Collections.ArmorData, "wrong shield")).Returns(new ArmorDataSelection());

            var item = mundaneArmorGenerator.Generate("shield");
            var armor = item as Armor;

            Assert.That(armor, Is.Not.Null);
            Assert.That(armor.Name, Is.EqualTo("shield"));
            Assert.That(armor.BaseNames, Is.EqualTo(baseNames));
            Assert.That(armor.Traits, Contains.Item(TraitConstants.Masterwork));
            Assert.That(armor.Traits, Is.All.Not.EqualTo("size"));
            Assert.That(armor.Size, Is.EqualTo("size"));
            Assert.That(armor.Attributes, Is.EqualTo(attributes));
            Assert.That(armor.ArmorBonus, Is.EqualTo(9266));
            Assert.That(armor.ArmorCheckPenalty, Is.EqualTo(-90210));
            Assert.That(armor.MaxDexterityBonus, Is.EqualTo(42));
        }

        [Test]
        public void GenerateFromNameWithTraits()
        {
            mockPercentileSelector.SetupSequence(p => p.SelectFrom(Config.Name, TableNameConstants.Percentiles.MundaneArmors))
                .Returns("wrong armor")
                .Returns("armor")
                .Returns("other armor");

            var baseNames = new[] { "base name", "other base name" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.ItemGroups, "armor")).Returns(baseNames);

            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, TableNameConstants.Percentiles.MundaneGearSizes)).Returns("size");
            mockPercentileSelector.Setup(p => p.SelectFrom<bool>(Config.Name, TableNameConstants.Percentiles.IsMasterwork)).Returns(false);

            var attributes = new[] { "attribute 1", "attribute 2" };
            var tableName = TableNameConstants.Collections.ITEMTYPEAttributes(ItemTypeConstants.Armor);
            mockCollectionsSelector.Setup(p => p.SelectFrom(Config.Name, tableName, "armor")).Returns(attributes);

            armorSelection.ArmorBonus = 9266;
            armorSelection.ArmorCheckPenalty = -90210;
            armorSelection.MaxDexterityBonus = 42;
            mockArmorDataSelector.Setup(s => s.SelectOneFrom(Config.Name, TableNameConstants.Collections.ArmorData, "armor")).Returns(armorSelection);
            mockArmorDataSelector.Setup(s => s.SelectOneFrom(Config.Name, TableNameConstants.Collections.ArmorData, "wrong armor")).Returns(new ArmorDataSelection());

            var item = mundaneArmorGenerator.Generate("armor", "my trait", "my other trait");
            var armor = item as Armor;

            Assert.That(armor, Is.Not.Null);
            Assert.That(armor.Name, Is.EqualTo("armor"));
            Assert.That(armor.BaseNames, Is.EqualTo(baseNames));
            Assert.That(armor.Traits, Is.All.Not.EqualTo(TraitConstants.Masterwork));
            Assert.That(armor.Traits, Is.All.Not.EqualTo("size"));
            Assert.That(armor.Traits, Contains.Item("my trait"));
            Assert.That(armor.Traits, Contains.Item("my other trait"));
            Assert.That(armor.Size, Is.EqualTo("size"));
            Assert.That(armor.Attributes, Is.EqualTo(attributes));
            Assert.That(armor.ArmorBonus, Is.EqualTo(9266));
            Assert.That(armor.ArmorCheckPenalty, Is.EqualTo(-90210));
            Assert.That(armor.MaxDexterityBonus, Is.EqualTo(42));
        }

        [Test]
        public void GenerateFromNameWithDuplicateTraits()
        {
            mockPercentileSelector.SetupSequence(p => p.SelectFrom(Config.Name, TableNameConstants.Percentiles.MundaneArmors))
                .Returns("wrong armor")
                .Returns("armor")
                .Returns("other armor");

            var baseNames = new[] { "base name", "other base name" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.ItemGroups, "armor")).Returns(baseNames);

            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, TableNameConstants.Percentiles.MundaneGearSizes)).Returns("size");
            mockPercentileSelector.Setup(p => p.SelectFrom<bool>(Config.Name, TableNameConstants.Percentiles.IsMasterwork)).Returns(false);

            var attributes = new[] { "attribute 1", "attribute 2" };
            var tableName = TableNameConstants.Collections.ITEMTYPEAttributes(ItemTypeConstants.Armor);
            mockCollectionsSelector.Setup(p => p.SelectFrom(Config.Name, tableName, "armor")).Returns(attributes);

            armorSelection.ArmorBonus = 9266;
            armorSelection.ArmorCheckPenalty = -90210;
            armorSelection.MaxDexterityBonus = 42;
            mockArmorDataSelector.Setup(s => s.SelectOneFrom(Config.Name, TableNameConstants.Collections.ArmorData, "armor")).Returns(armorSelection);
            mockArmorDataSelector.Setup(s => s.SelectOneFrom(Config.Name, TableNameConstants.Collections.ArmorData, "wrong armor")).Returns(new ArmorDataSelection());

            var item = mundaneArmorGenerator.Generate("armor", "my trait", "my trait");
            var armor = item as Armor;

            Assert.That(armor, Is.Not.Null);
            Assert.That(armor.Name, Is.EqualTo("armor"));
            Assert.That(armor.BaseNames, Is.EqualTo(baseNames));
            Assert.That(armor.Traits, Is.All.Not.EqualTo(TraitConstants.Masterwork));
            Assert.That(armor.Traits, Is.All.Not.EqualTo("size"));
            Assert.That(armor.Traits, Contains.Item("my trait"));
            Assert.That(armor.Size, Is.EqualTo("size"));
            Assert.That(armor.Attributes, Is.EqualTo(attributes));
            Assert.That(armor.ArmorBonus, Is.EqualTo(9266));
            Assert.That(armor.ArmorCheckPenalty, Is.EqualTo(-90210));
            Assert.That(armor.MaxDexterityBonus, Is.EqualTo(42));
        }

        [Test]
        public void GenerateFromNameWithSize()
        {
            mockPercentileSelector.SetupSequence(p => p.SelectFrom(Config.Name, TableNameConstants.Percentiles.MundaneArmors))
                .Returns("wrong armor")
                .Returns("armor")
                .Returns("other armor");

            var sizes = new[] { "size", "custom size", "other size" };
            mockPercentileSelector.Setup(s => s.SelectAllFrom(Config.Name, TableNameConstants.Percentiles.MundaneGearSizes)).Returns(sizes);

            var baseNames = new[] { "base name", "other base name" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.ItemGroups, "armor")).Returns(baseNames);

            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, TableNameConstants.Percentiles.MundaneGearSizes)).Returns("wrong size");
            mockPercentileSelector.Setup(p => p.SelectFrom<bool>(Config.Name, TableNameConstants.Percentiles.IsMasterwork)).Returns(false);

            var attributes = new[] { "attribute 1", "attribute 2" };
            var tableName = TableNameConstants.Collections.ITEMTYPEAttributes(ItemTypeConstants.Armor);
            mockCollectionsSelector.Setup(p => p.SelectFrom(Config.Name, tableName, "armor")).Returns(attributes);

            armorSelection.ArmorBonus = 9266;
            armorSelection.ArmorCheckPenalty = -90210;
            armorSelection.MaxDexterityBonus = 42;
            mockArmorDataSelector.Setup(s => s.SelectOneFrom(Config.Name, TableNameConstants.Collections.ArmorData, "armor")).Returns(armorSelection);
            mockArmorDataSelector.Setup(s => s.SelectOneFrom(Config.Name, TableNameConstants.Collections.ArmorData, "wrong armor")).Returns(new ArmorDataSelection());

            var item = mundaneArmorGenerator.Generate("armor", "size");
            var armor = item as Armor;

            Assert.That(armor, Is.Not.Null);
            Assert.That(armor.Name, Is.EqualTo("armor"));
            Assert.That(armor.BaseNames, Is.EqualTo(baseNames));
            Assert.That(armor.Traits, Is.All.Not.EqualTo(TraitConstants.Masterwork));
            Assert.That(armor.Traits, Is.All.Not.EqualTo("size"));
            Assert.That(armor.Size, Is.EqualTo("size"));
            Assert.That(armor.Attributes, Is.EqualTo(attributes));
            Assert.That(armor.ArmorBonus, Is.EqualTo(9266));
            Assert.That(armor.ArmorCheckPenalty, Is.EqualTo(-90210));
            Assert.That(armor.MaxDexterityBonus, Is.EqualTo(42));
        }
    }
}