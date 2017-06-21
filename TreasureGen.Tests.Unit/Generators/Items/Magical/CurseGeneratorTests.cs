﻿using Moq;
using NUnit.Framework;
using RollGen;
using System;
using TreasureGen.Domain.Generators.Items.Magical;
using TreasureGen.Domain.Selectors.Collections;
using TreasureGen.Domain.Selectors.Percentiles;
using TreasureGen.Domain.Tables;
using TreasureGen.Items;
using TreasureGen.Items.Magical;
using TreasureGen.Items.Mundane;

namespace TreasureGen.Tests.Unit.Generators.Items.Magical
{
    [TestFixture]
    public class CurseGeneratorTests
    {
        private ICurseGenerator curseGenerator;
        private Mock<Dice> mockDice;
        private Mock<IPercentileSelector> mockPercentileSelector;
        private Mock<IBooleanPercentileSelector> mockBooleanPercentileSelector;
        private Mock<ICollectionsSelector> mockCollectionsSelector;
        private Mock<MundaneItemGenerator> mockMundaneArmorGenerator;
        private Mock<MundaneItemGenerator> mockMundaneWeaponGenerator;
        private ItemVerifier itemVerifier;

        [SetUp]
        public void Setup()
        {
            mockDice = new Mock<Dice>();
            mockPercentileSelector = new Mock<IPercentileSelector>();
            mockBooleanPercentileSelector = new Mock<IBooleanPercentileSelector>();
            mockCollectionsSelector = new Mock<ICollectionsSelector>();
            var generator = new ConfigurableIterativeGenerator(5);
            mockMundaneArmorGenerator = new Mock<MundaneItemGenerator>();
            mockMundaneWeaponGenerator = new Mock<MundaneItemGenerator>();

            var mockMundaneGeneratorFactory = new Mock<IMundaneItemGeneratorFactory>();
            mockMundaneGeneratorFactory.Setup(f => f.CreateGeneratorOf(ItemTypeConstants.Armor)).Returns(mockMundaneArmorGenerator.Object);
            mockMundaneGeneratorFactory.Setup(f => f.CreateGeneratorOf(ItemTypeConstants.Weapon)).Returns(mockMundaneWeaponGenerator.Object);

            curseGenerator = new CurseGenerator(mockDice.Object, mockPercentileSelector.Object, mockBooleanPercentileSelector.Object, mockCollectionsSelector.Object, generator, mockMundaneGeneratorFactory.Object);

            itemVerifier = new ItemVerifier();
        }

        [Test]
        public void NotCursedIfNoMagic()
        {
            mockBooleanPercentileSelector.Setup(s => s.SelectFrom(TableNameConstants.Percentiles.Set.IsItemCursed)).Returns(true);
            var cursed = curseGenerator.HasCurse(false);
            Assert.That(cursed, Is.False);
        }

        [Test]
        public void NotCursedIfSelectorSaySo()
        {
            mockBooleanPercentileSelector.Setup(s => s.SelectFrom(TableNameConstants.Percentiles.Set.IsItemCursed)).Returns(false);
            var cursed = curseGenerator.HasCurse(true);
            Assert.That(cursed, Is.False);
        }

        [Test]
        public void CursedIfSelectorSaysSo()
        {
            mockBooleanPercentileSelector.Setup(s => s.SelectFrom(TableNameConstants.Percentiles.Set.IsItemCursed)).Returns(true);
            var cursed = curseGenerator.HasCurse(true);
            Assert.That(cursed, Is.True);
        }

        [Test]
        public void GenerateCurseGetsFromPercentileSelector()
        {
            mockPercentileSelector.Setup(s => s.SelectFrom(TableNameConstants.Percentiles.Set.Curses)).Returns("curse");

            var curse = curseGenerator.GenerateCurse();
            Assert.That(curse, Is.EqualTo("curse"));
        }

        [Test]
        public void IfIntermittentFunctioning_1OnD3IsUnreliable()
        {
            mockPercentileSelector.Setup(s => s.SelectFrom(TableNameConstants.Percentiles.Set.Curses)).Returns("Intermittent Functioning");
            mockDice.Setup(d => d.Roll(1).d(3).AsSum()).Returns(1);

            var curse = curseGenerator.GenerateCurse();
            Assert.That(curse, Is.EqualTo("Intermittent Functioning (Unreliable)"));
        }

        [Test]
        public void IfIntermittentFunctioning_2OnD3IsDependent()
        {
            mockPercentileSelector.Setup(s => s.SelectFrom(TableNameConstants.Percentiles.Set.Curses)).Returns("Intermittent Functioning");
            mockDice.Setup(d => d.Roll(1).d(3).AsSum()).Returns(2);
            mockPercentileSelector.Setup(s => s.SelectFrom(TableNameConstants.Percentiles.Set.CursedDependentSituations)).Returns("situation");

            var curse = curseGenerator.GenerateCurse();
            Assert.That(curse, Is.EqualTo("Intermittent Functioning (Dependent: situation)"));
        }

        [Test]
        public void IfIntermittentFunctioning_3OnD3IsUncontrolled()
        {
            mockPercentileSelector.Setup(s => s.SelectFrom(TableNameConstants.Percentiles.Set.Curses)).Returns("Intermittent Functioning");
            mockDice.Setup(d => d.Roll(1).d(3).AsSum()).Returns(3);

            var curse = curseGenerator.GenerateCurse();
            Assert.That(curse, Is.EqualTo("Intermittent Functioning (Uncontrolled)"));
        }

        [Test]
        public void IfDrawback_GetDrawback()
        {
            mockPercentileSelector.Setup(s => s.SelectFrom(TableNameConstants.Percentiles.Set.Curses)).Returns("Drawback");
            mockPercentileSelector.Setup(s => s.SelectFrom(TableNameConstants.Percentiles.Set.CurseDrawbacks)).Returns("cursed drawback");

            var curse = curseGenerator.GenerateCurse();
            Assert.That(curse, Is.EqualTo("cursed drawback"));
        }

        [Test]
        public void GetSpecificCursedItem()
        {
            mockPercentileSelector.Setup(s => s.SelectFrom(TableNameConstants.Percentiles.Set.SpecificCursedItems)).Returns("specific cursed item");

            var itemType = new[] { "item type" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.SpecificCursedItemItemTypes, "specific cursed item")).Returns(itemType);

            var attributes = new[] { "attribute 1", "attribute 2" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.SpecificCursedItemAttributes, "specific cursed item")).Returns(attributes);

            var baseNames = new[] { "base name", "other base name" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, "specific cursed item")).Returns(baseNames);

            var cursedItem = curseGenerator.Generate();
            Assert.That(cursedItem.Name, Is.EqualTo("specific cursed item"));
            Assert.That(cursedItem.BaseNames, Is.EquivalentTo(baseNames));
            Assert.That(cursedItem.IsMagical, Is.True);
            Assert.That(cursedItem.Magic.Curse, Is.EqualTo(CurseConstants.SpecificCursedItem));
            Assert.That(cursedItem.ItemType, Is.EqualTo("item type"));
            Assert.That(cursedItem.Attributes, Is.EquivalentTo(attributes));
            Assert.That(cursedItem, Is.Not.InstanceOf<Armor>());
            Assert.That(cursedItem, Is.Not.InstanceOf<Weapon>());
        }

        [Test]
        public void GetSpecificCursedArmor()
        {
            mockPercentileSelector.Setup(s => s.SelectFrom(TableNameConstants.Percentiles.Set.SpecificCursedItems)).Returns("specific cursed item");

            var itemType = new[] { ItemTypeConstants.Armor };
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.SpecificCursedItemItemTypes, "specific cursed item")).Returns(itemType);

            var attributes = new[] { "attribute 1", "attribute 2" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.SpecificCursedItemAttributes, "specific cursed item")).Returns(attributes);

            var baseNames = new[] { "base name" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, "specific cursed item")).Returns(baseNames);

            var mundaneArmor = itemVerifier.CreateRandomArmorTemplate("base name");
            mundaneArmor.Size = Guid.NewGuid().ToString();
            mockMundaneArmorGenerator.Setup(g => g.GenerateFrom(It.Is<Item>(i => i.Name == "base name"), false)).Returns(mundaneArmor);

            var cursedItem = curseGenerator.Generate();
            Assert.That(cursedItem.Name, Is.EqualTo("specific cursed item"));
            Assert.That(cursedItem.BaseNames, Is.EquivalentTo(baseNames));
            Assert.That(cursedItem.IsMagical, Is.True);
            Assert.That(cursedItem.Magic.Curse, Is.EqualTo(CurseConstants.SpecificCursedItem));
            Assert.That(cursedItem.ItemType, Is.EqualTo(ItemTypeConstants.Armor));
            Assert.That(cursedItem.Attributes, Is.EquivalentTo(attributes));
            Assert.That(cursedItem, Is.InstanceOf<Armor>());

            var armor = cursedItem as Armor;
            Assert.That(armor.ArmorBonus, Is.EqualTo(mundaneArmor.ArmorBonus));
            Assert.That(armor.ArmorCheckPenalty, Is.EqualTo(mundaneArmor.ArmorCheckPenalty));
            Assert.That(armor.MaxDexterityBonus, Is.EqualTo(mundaneArmor.MaxDexterityBonus));
            Assert.That(armor.Size, Is.EqualTo(mundaneArmor.Size));

            //INFO: Because all specific cursed items are magical, they are also all masterwork
            Assert.That(armor.Traits, Contains.Item(TraitConstants.Masterwork));
        }

        [Test]
        public void GetSpecificCursedWeapon()
        {
            mockPercentileSelector.Setup(s => s.SelectFrom(TableNameConstants.Percentiles.Set.SpecificCursedItems)).Returns("specific cursed item");

            var itemType = new[] { ItemTypeConstants.Weapon };
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.SpecificCursedItemItemTypes, "specific cursed item")).Returns(itemType);

            var attributes = new[] { "attribute 1", "attribute 2" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.SpecificCursedItemAttributes, "specific cursed item")).Returns(attributes);

            var baseNames = new[] { "base name" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, "specific cursed item")).Returns(baseNames);

            var mundaneWeapon = itemVerifier.CreateRandomWeaponTemplate("base name");
            mundaneWeapon.Size = Guid.NewGuid().ToString();
            mundaneWeapon.Quantity = 9266;
            mockMundaneWeaponGenerator.Setup(g => g.GenerateFrom(It.Is<Item>(i => i.Name == "base name"), false)).Returns(mundaneWeapon);

            var cursedItem = curseGenerator.Generate();
            Assert.That(cursedItem.Name, Is.EqualTo("specific cursed item"));
            Assert.That(cursedItem.BaseNames, Is.EquivalentTo(baseNames));
            Assert.That(cursedItem.Quantity, Is.EqualTo(9266));
            Assert.That(cursedItem.IsMagical, Is.True);
            Assert.That(cursedItem.Magic.Curse, Is.EqualTo(CurseConstants.SpecificCursedItem));
            Assert.That(cursedItem.ItemType, Is.EqualTo(ItemTypeConstants.Weapon));
            Assert.That(cursedItem.Attributes, Is.EquivalentTo(attributes));
            Assert.That(cursedItem, Is.InstanceOf<Weapon>());

            var weapon = cursedItem as Weapon;
            Assert.That(weapon.Size, Is.EqualTo(mundaneWeapon.Size));
            Assert.That(weapon.CriticalMultiplier, Is.EqualTo(mundaneWeapon.CriticalMultiplier));
            Assert.That(weapon.Damage, Is.EqualTo(mundaneWeapon.Damage));
            Assert.That(weapon.DamageType, Is.EqualTo(mundaneWeapon.DamageType));
            Assert.That(weapon.ThreatRange, Is.EqualTo(mundaneWeapon.ThreatRange));

            //INFO: Because all specific cursed items are magical, they are also all masterwork
            Assert.That(weapon.Traits, Contains.Item(TraitConstants.Masterwork));
        }

        [Test]
        public void TemplateHasCurse()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);

            var cursedItems = new[] { "other cursed item", name };
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, CurseConstants.SpecificCursedItem)).Returns(cursedItems);

            var isCursed = curseGenerator.IsSpecificCursedItem(template);
            Assert.That(isCursed, Is.True);
        }

        [Test]
        public void TemplateHasNoCurse()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);

            var cursedItems = new[] { "other cursed item", "cursed item" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, CurseConstants.SpecificCursedItem)).Returns(cursedItems);

            var isCursed = curseGenerator.IsSpecificCursedItem(template);
            Assert.That(isCursed, Is.False);
        }

        [Test]
        public void GenerateCustomSpecificCursedItem()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);

            var itemType = new[] { "item type" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.SpecificCursedItemItemTypes, name)).Returns(itemType);

            var attributes = new[] { "attribute 1", "attribute 2" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.SpecificCursedItemAttributes, name)).Returns(attributes);

            var baseNames = new[] { "base name", "other base name" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, name)).Returns(baseNames);

            var cursedItem = curseGenerator.GenerateFrom(template);
            itemVerifier.AssertMagicalItemFromTemplate(cursedItem, template);
            Assert.That(cursedItem.Name, Is.EqualTo(name));
            Assert.That(cursedItem.BaseNames, Is.EquivalentTo(baseNames));
            Assert.That(cursedItem.IsMagical, Is.True);
            Assert.That(cursedItem.ItemType, Is.EqualTo("item type"));
            Assert.That(cursedItem.Attributes, Is.EquivalentTo(attributes));
            Assert.That(cursedItem.Magic.Curse, Is.EqualTo(CurseConstants.SpecificCursedItem));
            Assert.That(cursedItem, Is.Not.InstanceOf<Armor>());
            Assert.That(cursedItem, Is.Not.InstanceOf<Weapon>());
        }

        [Test]
        public void GenerateCustomSpecificCursedArmor()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);

            var itemType = new[] { ItemTypeConstants.Armor };
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.SpecificCursedItemItemTypes, name)).Returns(itemType);

            var attributes = new[] { "attribute 1", "attribute 2" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.SpecificCursedItemAttributes, name)).Returns(attributes);

            var baseNames = new[] { "base name", "other base name" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, name)).Returns(baseNames);

            var mundaneArmor = itemVerifier.CreateRandomArmorTemplate("base name");
            mundaneArmor.Size = Guid.NewGuid().ToString();
            mockMundaneArmorGenerator.Setup(g => g.GenerateFrom(It.Is<Item>(i => i.Name == "base name"), false)).Returns(mundaneArmor);

            var cursedItem = curseGenerator.GenerateFrom(template);
            itemVerifier.AssertMagicalItemFromTemplate(cursedItem, template);
            Assert.That(cursedItem.Name, Is.EqualTo(name));
            Assert.That(cursedItem.BaseNames, Is.EquivalentTo(baseNames));
            Assert.That(cursedItem.IsMagical, Is.True);
            Assert.That(cursedItem.ItemType, Is.EqualTo(ItemTypeConstants.Armor));
            Assert.That(cursedItem.Attributes, Is.EquivalentTo(attributes));
            Assert.That(cursedItem.Magic.Curse, Is.EqualTo(CurseConstants.SpecificCursedItem));
            Assert.That(cursedItem, Is.InstanceOf<Armor>());

            var armor = cursedItem as Armor;
            Assert.That(armor.ArmorBonus, Is.EqualTo(mundaneArmor.ArmorBonus));
            Assert.That(armor.ArmorCheckPenalty, Is.EqualTo(mundaneArmor.ArmorCheckPenalty));
            Assert.That(armor.MaxDexterityBonus, Is.EqualTo(mundaneArmor.MaxDexterityBonus));
            Assert.That(armor.Size, Is.EqualTo(mundaneArmor.Size));

            //INFO: Because all specific cursed items are magical, they are also all masterwork
            Assert.That(armor.Traits, Contains.Item(TraitConstants.Masterwork));
        }

        [Test]
        public void GenerateCustomSpecificCursedWeapon()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);

            var itemType = new[] { ItemTypeConstants.Weapon };
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.SpecificCursedItemItemTypes, name)).Returns(itemType);

            var attributes = new[] { "attribute 1", "attribute 2" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.SpecificCursedItemAttributes, name)).Returns(attributes);

            var baseNames = new[] { "base name", "other base name" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, name)).Returns(baseNames);

            var mundaneWeapon = itemVerifier.CreateRandomWeaponTemplate("base name");
            mundaneWeapon.Size = Guid.NewGuid().ToString();
            mundaneWeapon.Quantity = 9266;
            mockMundaneWeaponGenerator.Setup(g => g.GenerateFrom(It.Is<Item>(i => i.Name == "base name"), false)).Returns(mundaneWeapon);

            var cursedItem = curseGenerator.GenerateFrom(template);
            itemVerifier.AssertMagicalItemFromTemplate(cursedItem, template);
            Assert.That(cursedItem.Name, Is.EqualTo(name));
            Assert.That(cursedItem.BaseNames, Is.EquivalentTo(baseNames));
            Assert.That(cursedItem.Quantity, Is.EqualTo(9266));
            Assert.That(cursedItem.IsMagical, Is.True);
            Assert.That(cursedItem.ItemType, Is.EqualTo(ItemTypeConstants.Weapon));
            Assert.That(cursedItem.Attributes, Is.EquivalentTo(attributes));
            Assert.That(cursedItem.Magic.Curse, Is.EqualTo(CurseConstants.SpecificCursedItem));
            Assert.That(cursedItem, Is.InstanceOf<Weapon>());

            var weapon = cursedItem as Weapon;
            Assert.That(weapon.Size, Is.EqualTo(mundaneWeapon.Size));
            Assert.That(weapon.CriticalMultiplier, Is.EqualTo(mundaneWeapon.CriticalMultiplier));
            Assert.That(weapon.Damage, Is.EqualTo(mundaneWeapon.Damage));
            Assert.That(weapon.DamageType, Is.EqualTo(mundaneWeapon.DamageType));
            Assert.That(weapon.ThreatRange, Is.EqualTo(mundaneWeapon.ThreatRange));

            //INFO: Because all specific cursed items are magical, they are also all masterwork
            Assert.That(weapon.Traits, Contains.Item(TraitConstants.Masterwork));
        }

        [Test]
        public void GenerateCustomSpecificCursedItemWithNoSpecialAbilities()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);

            var itemType = new[] { "item type" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.SpecificCursedItemItemTypes, name)).Returns(itemType);

            var attributes = new[] { "attribute 1", "attribute 2" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.SpecificCursedItemAttributes, name)).Returns(attributes);

            var baseNames = new[] { "base name", "other base name" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, name)).Returns(baseNames);

            var cursedItem = curseGenerator.GenerateFrom(template);
            itemVerifier.AssertMagicalItemFromTemplate(cursedItem, template);
            Assert.That(cursedItem.Name, Is.EqualTo(name));
            Assert.That(cursedItem.BaseNames, Is.EquivalentTo(baseNames));
            Assert.That(cursedItem.IsMagical, Is.True);
            Assert.That(cursedItem.ItemType, Is.EqualTo("item type"));
            Assert.That(cursedItem.Attributes, Is.EquivalentTo(attributes));
            Assert.That(cursedItem.Magic.Curse, Is.EqualTo(CurseConstants.SpecificCursedItem));
            Assert.That(cursedItem.Magic.SpecialAbilities, Is.Empty);
            Assert.That(cursedItem, Is.Not.InstanceOf<Armor>());
            Assert.That(cursedItem, Is.Not.InstanceOf<Weapon>());
        }

        [Test]
        public void GenerateSpecificFromSubset()
        {
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(TableNameConstants.Percentiles.Set.SpecificCursedItems))
                .Returns("wrong specific cursed item")
                .Returns("specific cursed item")
                .Returns("other specific cursed item");

            var itemType = new[] { "item type" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.SpecificCursedItemItemTypes, "wrong specific cursed item")).Returns(itemType);
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.SpecificCursedItemItemTypes, "specific cursed item")).Returns(itemType);
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.SpecificCursedItemItemTypes, "other specific cursed item")).Returns(itemType);

            var attributes = new[] { "attribute 1", "attribute 2" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.SpecificCursedItemAttributes, "specific cursed item")).Returns(attributes);

            var baseNames = new[] { "base name", "other base name" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, "specific cursed item")).Returns(baseNames);

            var subset = new[] { "specific cursed item", "other specific cursed item" };

            var cursedItem = curseGenerator.GenerateFrom(subset);
            Assert.That(cursedItem.Name, Is.EqualTo("specific cursed item"));
            Assert.That(cursedItem.BaseNames, Is.EquivalentTo(baseNames));
            Assert.That(cursedItem.IsMagical, Is.True);
            Assert.That(cursedItem.Magic.Curse, Is.EqualTo(CurseConstants.SpecificCursedItem));
            Assert.That(cursedItem.ItemType, Is.EqualTo("item type"));
            Assert.That(cursedItem.Attributes, Is.EquivalentTo(attributes));
            Assert.That(cursedItem, Is.Not.InstanceOf<Armor>());
            Assert.That(cursedItem, Is.Not.InstanceOf<Weapon>());
        }

        [Test]
        public void GenerateSpecificFromSubsetWithBaseName()
        {
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(TableNameConstants.Percentiles.Set.SpecificCursedItems))
                .Returns("wrong specific cursed item")
                .Returns("specific cursed item")
                .Returns("other specific cursed item");

            var itemType = new[] { "item type" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.SpecificCursedItemItemTypes, "wrong specific cursed item")).Returns(itemType);
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.SpecificCursedItemItemTypes, "specific cursed item")).Returns(itemType);
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.SpecificCursedItemItemTypes, "other specific cursed item")).Returns(itemType);

            var attributes = new[] { "attribute 1", "attribute 2" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.SpecificCursedItemAttributes, "specific cursed item")).Returns(attributes);

            var baseNames = new[] { "base name", "other base name" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, "specific cursed item")).Returns(baseNames);

            var subset = new[] { "base name", "other specific cursed item" };

            var cursedItem = curseGenerator.GenerateFrom(subset);
            Assert.That(cursedItem.Name, Is.EqualTo("specific cursed item"));
            Assert.That(cursedItem.BaseNames, Is.EquivalentTo(baseNames));
            Assert.That(cursedItem.IsMagical, Is.True);
            Assert.That(cursedItem.Magic.Curse, Is.EqualTo(CurseConstants.SpecificCursedItem));
            Assert.That(cursedItem.ItemType, Is.EqualTo("item type"));
            Assert.That(cursedItem.Attributes, Is.EquivalentTo(attributes));
            Assert.That(cursedItem, Is.Not.InstanceOf<Armor>());
            Assert.That(cursedItem, Is.Not.InstanceOf<Weapon>());
        }

        [Test]
        public void GenerateSpecificArmorFromSubset()
        {
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(TableNameConstants.Percentiles.Set.SpecificCursedItems))
                .Returns("wrong specific cursed item")
                .Returns("specific cursed item")
                .Returns("other specific cursed item");

            var itemType = new[] { "item type" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.SpecificCursedItemItemTypes, "wrong specific cursed item")).Returns(itemType);
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.SpecificCursedItemItemTypes, "specific cursed item")).Returns(new[] { ItemTypeConstants.Armor });
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.SpecificCursedItemItemTypes, "other specific cursed item")).Returns(itemType);

            var attributes = new[] { "attribute 1", "attribute 2" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.SpecificCursedItemAttributes, "specific cursed item")).Returns(attributes);

            var baseNames = new[] { "base name", "other base name" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, "specific cursed item")).Returns(baseNames);

            var subset = new[] { "specific cursed item", "other specific cursed item" };

            var mundaneArmor = itemVerifier.CreateRandomArmorTemplate("base name");
            mundaneArmor.Size = Guid.NewGuid().ToString();
            mockMundaneArmorGenerator.Setup(g => g.GenerateFrom(It.Is<Item>(i => i.Name == "base name"), false)).Returns(mundaneArmor);

            var cursedItem = curseGenerator.GenerateFrom(subset);
            Assert.That(cursedItem.Name, Is.EqualTo("specific cursed item"));
            Assert.That(cursedItem.BaseNames, Is.EquivalentTo(baseNames));
            Assert.That(cursedItem.IsMagical, Is.True);
            Assert.That(cursedItem.Magic.Curse, Is.EqualTo(CurseConstants.SpecificCursedItem));
            Assert.That(cursedItem.ItemType, Is.EqualTo(ItemTypeConstants.Armor));
            Assert.That(cursedItem.Attributes, Is.EquivalentTo(attributes));
            Assert.That(cursedItem, Is.InstanceOf<Armor>());

            var armor = cursedItem as Armor;
            Assert.That(armor.ArmorBonus, Is.EqualTo(mundaneArmor.ArmorBonus));
            Assert.That(armor.ArmorCheckPenalty, Is.EqualTo(mundaneArmor.ArmorCheckPenalty));
            Assert.That(armor.MaxDexterityBonus, Is.EqualTo(mundaneArmor.MaxDexterityBonus));
            Assert.That(armor.Size, Is.EqualTo(mundaneArmor.Size));

            //INFO: Because all specific cursed items are magical, they are also all masterwork
            Assert.That(armor.Traits, Contains.Item(TraitConstants.Masterwork));
        }

        [Test]
        public void GenerateSpecificWeaponFromSubset()
        {
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(TableNameConstants.Percentiles.Set.SpecificCursedItems))
                .Returns("wrong specific cursed item")
                .Returns("specific cursed item")
                .Returns("other specific cursed item");

            var itemType = new[] { "item type" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.SpecificCursedItemItemTypes, "wrong specific cursed item")).Returns(itemType);
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.SpecificCursedItemItemTypes, "specific cursed item")).Returns(new[] { ItemTypeConstants.Weapon });
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.SpecificCursedItemItemTypes, "other specific cursed item")).Returns(itemType);

            var attributes = new[] { "attribute 1", "attribute 2" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.SpecificCursedItemAttributes, "specific cursed item")).Returns(attributes);

            var baseNames = new[] { "base name", "other base name" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, "specific cursed item")).Returns(baseNames);

            var subset = new[] { "specific cursed item", "other specific cursed item" };

            var mundaneWeapon = itemVerifier.CreateRandomWeaponTemplate("base name");
            mundaneWeapon.Size = Guid.NewGuid().ToString();
            mundaneWeapon.Quantity = 9266;
            mockMundaneWeaponGenerator.Setup(g => g.GenerateFrom(It.Is<Item>(i => i.Name == "base name"), false)).Returns(mundaneWeapon);

            var cursedItem = curseGenerator.GenerateFrom(subset);
            Assert.That(cursedItem.Name, Is.EqualTo("specific cursed item"));
            Assert.That(cursedItem.BaseNames, Is.EquivalentTo(baseNames));
            Assert.That(cursedItem.Quantity, Is.EqualTo(9266));
            Assert.That(cursedItem.IsMagical, Is.True);
            Assert.That(cursedItem.Magic.Curse, Is.EqualTo(CurseConstants.SpecificCursedItem));
            Assert.That(cursedItem.ItemType, Is.EqualTo(ItemTypeConstants.Weapon));
            Assert.That(cursedItem.Attributes, Is.EquivalentTo(attributes));
            Assert.That(cursedItem, Is.InstanceOf<Weapon>());

            var weapon = cursedItem as Weapon;
            Assert.That(weapon.Size, Is.EqualTo(mundaneWeapon.Size));
            Assert.That(weapon.CriticalMultiplier, Is.EqualTo(mundaneWeapon.CriticalMultiplier));
            Assert.That(weapon.Damage, Is.EqualTo(mundaneWeapon.Damage));
            Assert.That(weapon.DamageType, Is.EqualTo(mundaneWeapon.DamageType));
            Assert.That(weapon.ThreatRange, Is.EqualTo(mundaneWeapon.ThreatRange));

            //INFO: Because all specific cursed items are magical, they are also all masterwork
            Assert.That(weapon.Traits, Contains.Item(TraitConstants.Masterwork));
        }

        [Test]
        public void GenerateDefaultSpecificFromSubset()
        {
            mockPercentileSelector.Setup(s => s.SelectFrom(TableNameConstants.Percentiles.Set.SpecificCursedItems)).Returns("wrong specific cursed item");

            var itemType = new[] { "item type" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.SpecificCursedItemItemTypes, "wrong specific cursed item")).Returns(itemType);
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.SpecificCursedItemItemTypes, "specific cursed item")).Returns(itemType);
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.SpecificCursedItemItemTypes, "other specific cursed item")).Returns(itemType);

            var attributes = new[] { "attribute 1", "attribute 2" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.SpecificCursedItemAttributes, "specific cursed item")).Returns(attributes);

            var baseNames = new[] { "base name", "other base name" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, "specific cursed item")).Returns(baseNames);

            var subset = new[] { "base name", "other specific cursed item" };

            var cursedItem = curseGenerator.GenerateFrom(subset);
            Assert.That(cursedItem, Is.Null);
        }
    }
}