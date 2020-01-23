﻿using DnDGen.Infrastructure.Generators;
using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.RollGen;
using DnDGen.TreasureGen.Generators.Items.Magical;
using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Items.Mundane;
using DnDGen.TreasureGen.Selectors.Percentiles;
using DnDGen.TreasureGen.Tables;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.TreasureGen.Tests.Unit.Generators.Items.Magical
{
    [TestFixture]
    public class CurseGeneratorTests
    {
        private ICurseGenerator curseGenerator;
        private Mock<Dice> mockDice;
        private Mock<ITreasurePercentileSelector> mockPercentileSelector;
        private Mock<ICollectionSelector> mockCollectionsSelector;
        private Mock<MundaneItemGenerator> mockMundaneArmorGenerator;
        private Mock<MundaneItemGenerator> mockMundaneWeaponGenerator;
        private ItemVerifier itemVerifier;
        private Dictionary<string, IEnumerable<string>> itemGroups;

        [SetUp]
        public void Setup()
        {
            mockDice = new Mock<Dice>();
            mockPercentileSelector = new Mock<ITreasurePercentileSelector>();
            mockCollectionsSelector = new Mock<ICollectionSelector>();
            mockMundaneArmorGenerator = new Mock<MundaneItemGenerator>();
            mockMundaneWeaponGenerator = new Mock<MundaneItemGenerator>();

            var mockJustInTimeFactory = new Mock<JustInTimeFactory>();
            mockJustInTimeFactory.Setup(f => f.Build<MundaneItemGenerator>(ItemTypeConstants.Armor)).Returns(mockMundaneArmorGenerator.Object);
            mockJustInTimeFactory.Setup(f => f.Build<MundaneItemGenerator>(ItemTypeConstants.Weapon)).Returns(mockMundaneWeaponGenerator.Object);

            curseGenerator = new CurseGenerator(mockDice.Object, mockPercentileSelector.Object, mockCollectionsSelector.Object, mockJustInTimeFactory.Object);

            itemVerifier = new ItemVerifier();
            itemGroups = new Dictionary<string, IEnumerable<string>>();

            itemGroups["random item"] = new[] { "base name", "other base name" };

            mockCollectionsSelector.Setup(s => s.SelectAllFrom(TableNameConstants.Collections.Set.ItemGroups)).Returns(itemGroups);
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, It.IsAny<string>())).Returns((string table, string name) => itemGroups[name]);

            var count = 0;
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(It.IsAny<IEnumerable<string>>())).Returns((IEnumerable<string> c) => c.ElementAt(count++ % c.Count()));
        }

        [Test]
        public void NotCursedIfNoMagic()
        {
            mockPercentileSelector.Setup(s => s.SelectFrom<bool>(TableNameConstants.Percentiles.Set.IsItemCursed)).Returns(true);
            var cursed = curseGenerator.HasCurse(false);
            Assert.That(cursed, Is.False);
        }

        [Test]
        public void NotCursedIfSelectorSaySo()
        {
            mockPercentileSelector.Setup(s => s.SelectFrom<bool>(TableNameConstants.Percentiles.Set.IsItemCursed)).Returns(false);
            var cursed = curseGenerator.HasCurse(true);
            Assert.That(cursed, Is.False);
        }

        [Test]
        public void CursedIfSelectorSaysSo()
        {
            mockPercentileSelector.Setup(s => s.SelectFrom<bool>(TableNameConstants.Percentiles.Set.IsItemCursed)).Returns(true);
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
            mockDice.Setup(d => d.Roll(1).d(3).AsSum<int>()).Returns(1);

            var curse = curseGenerator.GenerateCurse();
            Assert.That(curse, Is.EqualTo("Intermittent Functioning (Unreliable)"));
        }

        [Test]
        public void IfIntermittentFunctioning_2OnD3IsDependent()
        {
            mockPercentileSelector.Setup(s => s.SelectFrom(TableNameConstants.Percentiles.Set.Curses)).Returns("Intermittent Functioning");
            mockDice.Setup(d => d.Roll(1).d(3).AsSum<int>()).Returns(2);
            mockPercentileSelector.Setup(s => s.SelectFrom(TableNameConstants.Percentiles.Set.CursedDependentSituations)).Returns("situation");

            var curse = curseGenerator.GenerateCurse();
            Assert.That(curse, Is.EqualTo("Intermittent Functioning (Dependent: situation)"));
        }

        [Test]
        public void IfIntermittentFunctioning_3OnD3IsUncontrolled()
        {
            mockPercentileSelector.Setup(s => s.SelectFrom(TableNameConstants.Percentiles.Set.Curses)).Returns("Intermittent Functioning");
            mockDice.Setup(d => d.Roll(1).d(3).AsSum<int>()).Returns(3);

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

            itemGroups["specific cursed item"] = new[] { "base name", "other base name" };

            var cursedItem = curseGenerator.Generate();
            Assert.That(cursedItem.Name, Is.EqualTo("specific cursed item"));
            Assert.That(cursedItem.BaseNames, Is.EquivalentTo(itemGroups["specific cursed item"]));
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

            itemGroups["specific cursed item"] = new[] { "base name" };

            var mundaneArmor = itemVerifier.CreateRandomArmorTemplate("base name");
            mundaneArmor.Size = Guid.NewGuid().ToString();
            mockMundaneArmorGenerator.Setup(g => g.GenerateFrom(It.Is<Item>(i => i.Name == "base name"), false)).Returns(mundaneArmor);

            var cursedItem = curseGenerator.Generate();
            Assert.That(cursedItem.Name, Is.EqualTo("specific cursed item"));
            Assert.That(cursedItem.BaseNames, Is.EquivalentTo(itemGroups["specific cursed item"]));
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

            itemGroups["specific cursed item"] = new[] { "base name", "other base name" };

            var mundaneWeapon = itemVerifier.CreateRandomWeaponTemplate("base name");
            mundaneWeapon.Size = Guid.NewGuid().ToString();
            mundaneWeapon.Quantity = 9266;
            mockMundaneWeaponGenerator.Setup(g => g.GenerateFrom(It.Is<Item>(i => i.Name == "base name"), false)).Returns(mundaneWeapon);

            var cursedItem = curseGenerator.Generate();
            Assert.That(cursedItem.Name, Is.EqualTo("specific cursed item"));
            Assert.That(cursedItem.BaseNames, Is.EquivalentTo(itemGroups["specific cursed item"]));
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

            itemGroups[CurseConstants.SpecificCursedItem] = new[] { "other cursed item", name };

            var isCursed = curseGenerator.IsSpecificCursedItem(template);
            Assert.That(isCursed, Is.True);
        }

        [Test]
        public void TemplateHasNoCurse()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);

            itemGroups[CurseConstants.SpecificCursedItem] = new[] { "other cursed item", "cursed item" };

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

            itemGroups[name] = new[] { "base name", "other base name" };

            var mundaneArmor = itemVerifier.CreateRandomArmorTemplate("base name");
            mundaneArmor.Size = Guid.NewGuid().ToString();
            mockMundaneArmorGenerator.Setup(g => g.GenerateFrom(It.Is<Item>(i => i.Name == "base name"), false)).Returns(mundaneArmor);

            var cursedItem = curseGenerator.GenerateFrom(template);
            itemVerifier.AssertMagicalItemFromTemplate(cursedItem, template);
            Assert.That(cursedItem.Name, Is.EqualTo(name));
            Assert.That(cursedItem.BaseNames, Is.EquivalentTo(itemGroups[name]));
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

            itemGroups[name] = new[] { "base name", "other base name" };

            var mundaneWeapon = itemVerifier.CreateRandomWeaponTemplate("base name");
            mundaneWeapon.Size = Guid.NewGuid().ToString();
            mundaneWeapon.Quantity = 9266;
            mockMundaneWeaponGenerator.Setup(g => g.GenerateFrom(It.Is<Item>(i => i.Name == "base name"), false)).Returns(mundaneWeapon);

            var cursedItem = curseGenerator.GenerateFrom(template);
            itemVerifier.AssertMagicalItemFromTemplate(cursedItem, template);
            Assert.That(cursedItem.Name, Is.EqualTo(name));
            Assert.That(cursedItem.BaseNames, Is.EquivalentTo(itemGroups[name]));
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

            itemGroups[name] = new[] { "base name", "other base name" };

            var cursedItem = curseGenerator.GenerateFrom(template);
            itemVerifier.AssertMagicalItemFromTemplate(cursedItem, template);
            Assert.That(cursedItem.Name, Is.EqualTo(name));
            Assert.That(cursedItem.BaseNames, Is.EquivalentTo(itemGroups[name]));
            Assert.That(cursedItem.IsMagical, Is.True);
            Assert.That(cursedItem.ItemType, Is.EqualTo("item type"));
            Assert.That(cursedItem.Attributes, Is.EquivalentTo(attributes));
            Assert.That(cursedItem.Magic.Curse, Is.EqualTo(CurseConstants.SpecificCursedItem));
            Assert.That(cursedItem.Magic.SpecialAbilities, Is.Empty);
            Assert.That(cursedItem, Is.Not.InstanceOf<Armor>());
            Assert.That(cursedItem, Is.Not.InstanceOf<Weapon>());
        }

        [Test]
        public void GenerateSpecificFromName()
        {
            var itemType = new[] { "item type" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.SpecificCursedItemItemTypes, "specific cursed item")).Returns(itemType);

            var attributes = new[] { "attribute 1", "attribute 2" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.SpecificCursedItemAttributes, "specific cursed item")).Returns(attributes);

            itemGroups["specific cursed item"] = new[] { "base name", "other base name" };
            itemGroups[CurseConstants.SpecificCursedItem] = new[] { "wrong specific cursed item", "specific cursed item", "other specific cursed item" };

            var cursedItem = curseGenerator.Generate("specific cursed item");
            Assert.That(cursedItem.Name, Is.EqualTo("specific cursed item"));
            Assert.That(cursedItem.BaseNames, Is.EquivalentTo(itemGroups["specific cursed item"]));
            Assert.That(cursedItem.IsMagical, Is.True);
            Assert.That(cursedItem.Magic.Curse, Is.EqualTo(CurseConstants.SpecificCursedItem));
            Assert.That(cursedItem.ItemType, Is.EqualTo("item type"));
            Assert.That(cursedItem.Attributes, Is.EquivalentTo(attributes));
            Assert.That(cursedItem, Is.Not.InstanceOf<Armor>());
            Assert.That(cursedItem, Is.Not.InstanceOf<Weapon>());
        }

        [Test]
        public void GenerateSpecificFromBaseName()
        {
            var itemType = new[] { "item type" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.SpecificCursedItemItemTypes, "specific cursed item")).Returns(itemType);

            var attributes = new[] { "attribute 1", "attribute 2" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.SpecificCursedItemAttributes, "specific cursed item")).Returns(attributes);

            itemGroups["wrong specific cursed item"] = new[] { "wrong base name", "other base name" };
            itemGroups["other specific cursed item"] = new[] { "other base name" };
            itemGroups["specific cursed item"] = new[] { "base name", "other base name" };
            itemGroups[CurseConstants.SpecificCursedItem] = new[] { "wrong specific cursed item", "specific cursed item", "other specific cursed item" };

            var cursedItem = curseGenerator.Generate("base name");
            Assert.That(cursedItem.Name, Is.EqualTo("specific cursed item"));
            Assert.That(cursedItem.BaseNames, Is.EquivalentTo(itemGroups["specific cursed item"]));
            Assert.That(cursedItem.IsMagical, Is.True);
            Assert.That(cursedItem.Magic.Curse, Is.EqualTo(CurseConstants.SpecificCursedItem));
            Assert.That(cursedItem.ItemType, Is.EqualTo("item type"));
            Assert.That(cursedItem.Attributes, Is.EquivalentTo(attributes));
            Assert.That(cursedItem, Is.Not.InstanceOf<Armor>());
            Assert.That(cursedItem, Is.Not.InstanceOf<Weapon>());
        }

        [Test]
        public void GenerateSpecificFromAnyBaseName()
        {
            var itemType = new[] { "item type" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.SpecificCursedItemItemTypes, "specific cursed item")).Returns(itemType);

            var attributes = new[] { "attribute 1", "attribute 2" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.SpecificCursedItemAttributes, "specific cursed item")).Returns(attributes);

            itemGroups["wrong specific cursed item"] = new[] { "wrong base name", "other base name" };
            itemGroups["other specific cursed item"] = new[] { "base name" };
            itemGroups["specific cursed item"] = new[] { "base name", "other base name" };
            itemGroups[CurseConstants.SpecificCursedItem] = new[] { "wrong specific cursed item", "other specific cursed item", "specific cursed item" };

            var cursedItem = curseGenerator.Generate("base name");
            Assert.That(cursedItem.Name, Is.EqualTo("other specific cursed item"));
            Assert.That(cursedItem.BaseNames, Is.EquivalentTo(itemGroups["other specific cursed item"]));
            Assert.That(cursedItem.IsMagical, Is.True);
            Assert.That(cursedItem.Magic.Curse, Is.EqualTo(CurseConstants.SpecificCursedItem));
            Assert.That(cursedItem.ItemType, Is.EqualTo("item type"));
            Assert.That(cursedItem.Attributes, Is.EquivalentTo(attributes));
            Assert.That(cursedItem, Is.Not.InstanceOf<Armor>());
            Assert.That(cursedItem, Is.Not.InstanceOf<Weapon>());
        }

        [Test]
        public void GenerateSpecificArmorFromName()
        {
            var itemType = new[] { "item type" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.SpecificCursedItemItemTypes, "specific cursed item")).Returns(new[] { ItemTypeConstants.Armor });

            var attributes = new[] { "attribute 1", "attribute 2" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.SpecificCursedItemAttributes, "specific cursed item")).Returns(attributes);

            itemGroups["specific cursed item"] = new[] { "base name", "other base name" };
            itemGroups[CurseConstants.SpecificCursedItem] = new[] { "wrong specific cursed item", "specific cursed item", "other specific cursed item" };

            var mundaneArmor = itemVerifier.CreateRandomArmorTemplate("base name");
            mundaneArmor.Size = Guid.NewGuid().ToString();
            mockMundaneArmorGenerator.Setup(g => g.GenerateFrom(It.Is<Item>(i => i.Name == "base name"), false)).Returns(mundaneArmor);

            var cursedItem = curseGenerator.Generate("specific cursed item");
            Assert.That(cursedItem.Name, Is.EqualTo("specific cursed item"));
            Assert.That(cursedItem.BaseNames, Is.EquivalentTo(itemGroups["specific cursed item"]));
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
        public void GenerateSpecificWeaponFromName()
        {
            var itemType = new[] { "item type" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.SpecificCursedItemItemTypes, "specific cursed item")).Returns(new[] { ItemTypeConstants.Weapon });

            var attributes = new[] { "attribute 1", "attribute 2" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.SpecificCursedItemAttributes, "specific cursed item")).Returns(attributes);

            itemGroups["specific cursed item"] = new[] { "base name", "other base name" };
            itemGroups[CurseConstants.SpecificCursedItem] = new[] { "wrong specific cursed item", "specific cursed item", "other specific cursed item" };

            var mundaneWeapon = itemVerifier.CreateRandomWeaponTemplate("base name");
            mundaneWeapon.Size = Guid.NewGuid().ToString();
            mundaneWeapon.Quantity = 9266;
            mockMundaneWeaponGenerator.Setup(g => g.GenerateFrom(It.Is<Item>(i => i.Name == "base name"), false)).Returns(mundaneWeapon);

            var cursedItem = curseGenerator.Generate("specific cursed item");
            Assert.That(cursedItem.Name, Is.EqualTo("specific cursed item"));
            Assert.That(cursedItem.BaseNames, Is.EquivalentTo(itemGroups["specific cursed item"]));
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
        public void DoNotGenerateSpecificFromName_IfNotSpecific_AndBaseNameDoesNotMatch()
        {
            var itemType = new[] { "item type" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.SpecificCursedItemItemTypes, "specific cursed item")).Returns(itemType);
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.SpecificCursedItemItemTypes, "other specific cursed item")).Returns(itemType);

            var attributes = new[] { "attribute 1", "attribute 2" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.SpecificCursedItemAttributes, "specific cursed item")).Returns(attributes);

            itemGroups["wrong specific cursed item"] = new[] { "wrong base name", "other base name" };
            itemGroups["other specific cursed item"] = new[] { "other base name" };
            itemGroups["specific cursed item"] = new[] { "base name", "other base name" };
            itemGroups[CurseConstants.SpecificCursedItem] = new[] { "specific cursed item", "other specific cursed item" };

            var cursedItem = curseGenerator.Generate("wrong specific cursed item");
            Assert.That(cursedItem, Is.Null);
        }

        [Test]
        public void IsSpecificCursedItem_ReturnsTrue()
        {
            itemGroups[CurseConstants.SpecificCursedItem] = new[] { "specific cursed item", "other specific cursed item" };

            var isSpecificCursed = curseGenerator.IsSpecificCursedItem("specific cursed item");
            Assert.That(isSpecificCursed, Is.True);
        }

        [Test]
        public void IsSpecificCursedItem_ReturnsFalse()
        {
            itemGroups[CurseConstants.SpecificCursedItem] = new[] { "specific cursed item", "other specific cursed item" };

            var isSpecificCursed = curseGenerator.IsSpecificCursedItem("wrong cursed item");
            Assert.That(isSpecificCursed, Is.False);
        }

        [Test]
        public void CanBeSpecificCursedItem_ReturnsTrue_IsSpecific()
        {
            itemGroups[CurseConstants.SpecificCursedItem] = new[] { "specific cursed item", "other specific cursed item" };

            var canBeSpecificCursed = curseGenerator.CanBeSpecificCursedItem("specific cursed item");
            Assert.That(canBeSpecificCursed, Is.True);
        }

        [Test]
        public void CanBeSpecificCursedItem_ReturnsTrue_BaseName()
        {
            itemGroups["other specific cursed item"] = new[] { "other base name" };
            itemGroups["specific cursed item"] = new[] { "base name", "other base name" };
            itemGroups[CurseConstants.SpecificCursedItem] = new[] { "specific cursed item", "other specific cursed item" };

            var canBeSpecificCursed = curseGenerator.CanBeSpecificCursedItem("base name");
            Assert.That(canBeSpecificCursed, Is.True);
        }

        [Test]
        public void CanBeSpecificCursedItem_ReturnsFalse()
        {
            itemGroups["other specific cursed item"] = new[] { "other base name" };
            itemGroups["specific cursed item"] = new[] { "base name", "other base name" };
            itemGroups[CurseConstants.SpecificCursedItem] = new[] { "specific cursed item", "other specific cursed item" };

            var canBeSpecificCursed = curseGenerator.CanBeSpecificCursedItem("wrong name");
            Assert.That(canBeSpecificCursed, Is.False);
        }
    }
}