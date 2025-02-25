using DnDGen.Infrastructure.Factories;
using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.RollGen;
using DnDGen.TreasureGen.Generators.Items.Magical;
using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Items.Mundane;
using DnDGen.TreasureGen.Selectors.Percentiles;
using DnDGen.TreasureGen.Tables;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
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

            mockCollectionsSelector.Setup(s => s.SelectAllFrom(Config.Name, TableNameConstants.Collections.ItemGroups)).Returns(itemGroups);
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.ItemGroups, It.IsAny<string>()))
                .Returns((string assembly, string table, string name) => itemGroups[name]);

            var count = 0;
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(It.IsAny<IEnumerable<string>>())).Returns((IEnumerable<string> c) => c.ElementAt(count++ % c.Count()));
        }

        [Test]
        public void NotCursedIfNoMagic()
        {
            var item = new Item();
            item.IsMagical = false;

            mockPercentileSelector.Setup(s => s.SelectFrom<bool>(Config.Name, TableNameConstants.Percentiles.IsItemCursed)).Returns(true);

            var cursed = curseGenerator.HasCurse(item);
            Assert.That(cursed, Is.False);
        }

        [Test]
        public void NotCursedIfSelectorSaySo()
        {
            var item = new Item();
            item.IsMagical = true;

            mockPercentileSelector.Setup(s => s.SelectFrom<bool>(Config.Name, TableNameConstants.Percentiles.IsItemCursed)).Returns(false);

            var cursed = curseGenerator.HasCurse(item);
            Assert.That(cursed, Is.False);
        }

        [Test]
        public void CursedIfSelectorSaysSo()
        {
            var item = new Item();
            item.IsMagical = true;

            mockPercentileSelector.Setup(s => s.SelectFrom<bool>(Config.Name, TableNameConstants.Percentiles.IsItemCursed)).Returns(true);

            var cursed = curseGenerator.HasCurse(item);
            Assert.That(cursed, Is.True);
        }

        [Test]
        public void GenerateCurseGetsFromPercentileSelector()
        {
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.Curses)).Returns("curse");

            var curse = curseGenerator.GenerateCurse();
            Assert.That(curse, Is.EqualTo("curse"));
        }

        [Test]
        public void IfIntermittentFunctioning_1OnD3IsUnreliable()
        {
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.Curses)).Returns("Intermittent Functioning");
            mockDice.Setup(d => d.Roll(1).d(3).AsSum<int>()).Returns(1);

            var curse = curseGenerator.GenerateCurse();
            Assert.That(curse, Is.EqualTo("Intermittent Functioning (Unreliable)"));
        }

        [Test]
        public void IfIntermittentFunctioning_2OnD3IsDependent()
        {
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.Curses)).Returns("Intermittent Functioning");
            mockDice.Setup(d => d.Roll(1).d(3).AsSum<int>()).Returns(2);
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.CursedDependentSituations)).Returns("situation");

            var curse = curseGenerator.GenerateCurse();
            Assert.That(curse, Is.EqualTo("Intermittent Functioning (Dependent: situation)"));
        }

        [Test]
        public void IfIntermittentFunctioning_3OnD3IsUncontrolled()
        {
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.Curses)).Returns("Intermittent Functioning");
            mockDice.Setup(d => d.Roll(1).d(3).AsSum<int>()).Returns(3);

            var curse = curseGenerator.GenerateCurse();
            Assert.That(curse, Is.EqualTo("Intermittent Functioning (Uncontrolled)"));
        }

        [Test]
        public void IfDrawback_GetDrawback()
        {
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.Curses)).Returns("Drawback");
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.CurseDrawbacks)).Returns("cursed drawback");

            var curse = curseGenerator.GenerateCurse();
            Assert.That(curse, Is.EqualTo("cursed drawback"));
        }

        [Test]
        public void GetSpecificCursedItem()
        {
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.SpecificCursedItems)).Returns("specific cursed item");

            var itemType = new[] { "item type" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemItemTypes, "specific cursed item")).Returns(itemType);

            var attributes = new[] { "attribute 1", "attribute 2" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemAttributes, "specific cursed item")).Returns(attributes);

            itemGroups[CurseConstants.SpecificCursedItem] = new[] { "wrong specific cursed item", "specific cursed item", "other specific cursed item" };
            itemGroups["specific cursed item"] = new[] { "base name", "other base name" };
            itemGroups["wrong specific cursed item"] = new[] { "wrong base name", "other base name" };
            itemGroups["other specific cursed item"] = new[] { "other base name" };

            var cursedItem = curseGenerator.GenerateRandom();
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
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.SpecificCursedItems)).Returns("specific cursed item");

            var itemType = new[] { ItemTypeConstants.Armor };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemItemTypes, "specific cursed item")).Returns(itemType);

            var attributes = new[] { "attribute 1", "attribute 2" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemAttributes, "specific cursed item")).Returns(attributes);

            itemGroups[CurseConstants.SpecificCursedItem] = new[] { "wrong specific cursed item", "specific cursed item", "other specific cursed item" };
            itemGroups["specific cursed item"] = new[] { "base name" };
            itemGroups["wrong specific cursed item"] = new[] { "wrong base name", "other base name" };
            itemGroups["other specific cursed item"] = new[] { "other base name" };

            var mundaneArmor = itemVerifier.CreateRandomArmorTemplate("base name");
            mundaneArmor.Size = Guid.NewGuid().ToString();
            mockMundaneArmorGenerator.Setup(g => g.Generate("base name")).Returns(mundaneArmor);

            var cursedItem = curseGenerator.GenerateRandom();
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
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.SpecificCursedItems)).Returns("specific cursed item");

            var itemType = new[] { ItemTypeConstants.Weapon };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemItemTypes, "specific cursed item")).Returns(itemType);

            var attributes = new[] { "attribute 1", "attribute 2" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemAttributes, "specific cursed item")).Returns(attributes);

            itemGroups[CurseConstants.SpecificCursedItem] = new[] { "wrong specific cursed item", "specific cursed item", "other specific cursed item" };
            itemGroups["specific cursed item"] = new[] { "base name", "other base name" };
            itemGroups["wrong specific cursed item"] = new[] { "wrong base name", "other base name" };
            itemGroups["other specific cursed item"] = new[] { "other base name" };

            var mundaneWeapon = itemVerifier.CreateRandomWeaponTemplate("base name");
            mundaneWeapon.Size = Guid.NewGuid().ToString();
            mundaneWeapon.Quantity = 9266;
            mockMundaneWeaponGenerator.Setup(g => g.Generate("base name")).Returns(mundaneWeapon);

            var cursedItem = curseGenerator.GenerateRandom();
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
            Assert.That(weapon.Damages, Is.EqualTo(mundaneWeapon.Damages));
            Assert.That(weapon.CriticalDamages, Is.EqualTo(mundaneWeapon.CriticalDamages));
            Assert.That(weapon.ThreatRangeSummary, Is.EqualTo(mundaneWeapon.ThreatRangeSummary));

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
            template.Traits.Clear();

            var itemType = new[] { "item type" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemItemTypes, name)).Returns(itemType);

            var attributes = new[] { "attribute 1", "attribute 2" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemAttributes, name)).Returns(attributes);

            var baseNames = new[] { "base name", "other base name" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.ItemGroups, name)).Returns(baseNames);

            var cursedItem = curseGenerator.Generate(template);
            itemVerifier.AssertMagicalItemFromTemplate(cursedItem, template);
            Assert.That(cursedItem.Name, Is.EqualTo(name));
            Assert.That(cursedItem.BaseNames, Is.EquivalentTo(baseNames));
            Assert.That(cursedItem.IsMagical, Is.True);
            Assert.That(cursedItem.ItemType, Is.EqualTo("item type"));
            Assert.That(cursedItem.Attributes, Is.EquivalentTo(attributes));
            Assert.That(cursedItem.Magic.Curse, Is.EqualTo(CurseConstants.SpecificCursedItem));
            Assert.That(cursedItem, Is.Not.InstanceOf<Armor>()
                .And.Not.InstanceOf<Weapon>());
        }

        [Test]
        public void GenerateCustomSpecificCursedItem_WithTraits()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);

            var itemType = new[] { "item type" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemItemTypes, name)).Returns(itemType);

            var attributes = new[] { "attribute 1", "attribute 2" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemAttributes, name)).Returns(attributes);

            var baseNames = new[] { "base name", "other base name" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.ItemGroups, name)).Returns(baseNames);

            var cursedItem = curseGenerator.Generate(template);
            itemVerifier.AssertMagicalItemFromTemplate(cursedItem, template);
            Assert.That(cursedItem.Name, Is.EqualTo(name));
            Assert.That(cursedItem.BaseNames, Is.EquivalentTo(baseNames));
            Assert.That(cursedItem.IsMagical, Is.True);
            Assert.That(cursedItem.ItemType, Is.EqualTo("item type"));
            Assert.That(cursedItem.Attributes, Is.EquivalentTo(attributes));
            Assert.That(cursedItem.Magic.Curse, Is.EqualTo(CurseConstants.SpecificCursedItem));
            Assert.That(cursedItem, Is.Not.InstanceOf<Armor>()
                .And.Not.InstanceOf<Weapon>());
            Assert.That(cursedItem.Traits, Has.Count.EqualTo(2)
                .And.SupersetOf(template.Traits));
        }

        [Test]
        public void GenerateCustomSpecificCursedArmor()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);
            template.Traits.Clear();

            var itemType = new[] { ItemTypeConstants.Armor };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemItemTypes, name)).Returns(itemType);

            var attributes = new[] { "attribute 1", "attribute 2" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemAttributes, name)).Returns(attributes);

            itemGroups[name] = new[] { "base name", "other base name" };

            var mundaneArmor = itemVerifier.CreateRandomArmorTemplate("base name");
            mundaneArmor.Size = Guid.NewGuid().ToString();
            mockMundaneArmorGenerator.Setup(g => g.Generate("base name")).Returns(mundaneArmor);

            var cursedItem = curseGenerator.Generate(template);
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
        public void GenerateCustomSpecificCursedArmor_WithTraits()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);

            var itemType = new[] { ItemTypeConstants.Armor };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemItemTypes, name)).Returns(itemType);

            var attributes = new[] { "attribute 1", "attribute 2" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemAttributes, name)).Returns(attributes);

            itemGroups[name] = new[] { "base name", "other base name" };

            var mundaneArmor = itemVerifier.CreateRandomArmorTemplate("base name");
            mundaneArmor.Traits.Clear();

            mundaneArmor.Size = Guid.NewGuid().ToString();
            mockMundaneArmorGenerator
                .Setup(g => g.Generate("base name", template.Traits.First(), template.Traits.Last()))
                .Returns(mundaneArmor);

            var cursedItem = curseGenerator.Generate(template);
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
            Assert.That(armor.Traits, Contains.Item(TraitConstants.Masterwork)
                .And.SupersetOf(template.Traits)
                .And.Count.EqualTo(3));
        }

        [TestCase(TraitConstants.Sizes.Colossal)]
        [TestCase(TraitConstants.Sizes.Gargantuan)]
        [TestCase(TraitConstants.Sizes.Huge)]
        [TestCase(TraitConstants.Sizes.Large)]
        [TestCase(TraitConstants.Sizes.Medium)]
        [TestCase(TraitConstants.Sizes.Small)]
        [TestCase(TraitConstants.Sizes.Tiny)]
        public void GenerateCustomSpecificCursedArmor_WithTraitsAndSize(string size)
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);
            template.Traits.Add(size);

            var itemType = new[] { ItemTypeConstants.Armor };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemItemTypes, name)).Returns(itemType);

            var attributes = new[] { "attribute 1", "attribute 2" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemAttributes, name)).Returns(attributes);

            itemGroups[name] = new[] { "base name", "other base name" };

            var mundaneArmor = itemVerifier.CreateRandomArmorTemplate("base name");
            mundaneArmor.Traits = new HashSet<string>(template.Traits.Take(2));
            mundaneArmor.Size = size;

            mockMundaneArmorGenerator
                .Setup(g => g.Generate(
                    "base name",
                    It.Is<string[]>(ss => ss.IsEquivalent(template.Traits))))
                .Returns(mundaneArmor);

            var cursedItem = curseGenerator.Generate(template);
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
            Assert.That(armor.Size, Is.EqualTo(mundaneArmor.Size).And.EqualTo(size));

            //INFO: Because all specific cursed items are magical, they are also all masterwork
            Assert.That(armor.Traits, Contains.Item(TraitConstants.Masterwork)
                .And.SupersetOf(template.Traits.Take(2))
                .And.Not.Contains(size)
                .And.Count.EqualTo(3));
        }

        [Test]
        public void GenerateCustomSpecificCursedWeapon()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);
            template.Traits.Clear();

            var itemType = new[] { ItemTypeConstants.Weapon };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemItemTypes, name)).Returns(itemType);

            var attributes = new[] { "attribute 1", "attribute 2" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemAttributes, name)).Returns(attributes);

            itemGroups[name] = new[] { "base name", "other base name" };

            var mundaneWeapon = itemVerifier.CreateRandomWeaponTemplate("base name");
            mundaneWeapon.Size = Guid.NewGuid().ToString();
            mundaneWeapon.Quantity = 9266;
            mockMundaneWeaponGenerator.Setup(g => g.Generate("base name")).Returns(mundaneWeapon);

            var cursedItem = curseGenerator.Generate(template);
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
            Assert.That(weapon.Damages, Is.EqualTo(mundaneWeapon.Damages));
            Assert.That(weapon.CriticalDamages, Is.EqualTo(mundaneWeapon.CriticalDamages));
            Assert.That(weapon.ThreatRangeSummary, Is.EqualTo(mundaneWeapon.ThreatRangeSummary));

            //INFO: Because all specific cursed items are magical, they are also all masterwork
            Assert.That(weapon.Traits, Contains.Item(TraitConstants.Masterwork));
        }

        [Test]
        public void GenerateCustomSpecificCursedWeapon_WithTraits()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);

            var itemType = new[] { ItemTypeConstants.Weapon };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemItemTypes, name)).Returns(itemType);

            var attributes = new[] { "attribute 1", "attribute 2" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemAttributes, name)).Returns(attributes);

            itemGroups[name] = new[] { "base name", "other base name" };

            var mundaneWeapon = itemVerifier.CreateRandomWeaponTemplate("base name");
            mundaneWeapon.Size = Guid.NewGuid().ToString();
            mundaneWeapon.Quantity = 9266;
            mundaneWeapon.Traits.Clear();

            mockMundaneWeaponGenerator
                .Setup(g => g.Generate("base name", template.Traits.First(), template.Traits.Last()))
                .Returns(mundaneWeapon);

            var cursedItem = curseGenerator.Generate(template);
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
            Assert.That(weapon.Damages, Is.EqualTo(mundaneWeapon.Damages));
            Assert.That(weapon.CriticalDamages, Is.EqualTo(mundaneWeapon.CriticalDamages));
            Assert.That(weapon.ThreatRangeSummary, Is.EqualTo(mundaneWeapon.ThreatRangeSummary));

            //INFO: Because all specific cursed items are magical, they are also all masterwork
            Assert.That(weapon.Traits, Contains.Item(TraitConstants.Masterwork)
                .And.SupersetOf(template.Traits)
                .And.Count.EqualTo(3));
        }

        [TestCase(TraitConstants.Sizes.Colossal)]
        [TestCase(TraitConstants.Sizes.Gargantuan)]
        [TestCase(TraitConstants.Sizes.Huge)]
        [TestCase(TraitConstants.Sizes.Large)]
        [TestCase(TraitConstants.Sizes.Medium)]
        [TestCase(TraitConstants.Sizes.Small)]
        [TestCase(TraitConstants.Sizes.Tiny)]
        public void GenerateCustomSpecificCursedWeapon_WithTraitsAndSize(string size)
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);
            template.Traits.Add(size);

            var itemType = new[] { ItemTypeConstants.Weapon };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemItemTypes, name)).Returns(itemType);

            var attributes = new[] { "attribute 1", "attribute 2" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemAttributes, name)).Returns(attributes);

            itemGroups[name] = new[] { "base name", "other base name" };

            var mundaneWeapon = itemVerifier.CreateRandomWeaponTemplate("base name");
            mundaneWeapon.Size = size;
            mundaneWeapon.Quantity = 9266;
            mundaneWeapon.Traits = new HashSet<string>(template.Traits.Take(2));

            mockMundaneWeaponGenerator
                .Setup(g => g.Generate(
                    "base name",
                    It.Is<string[]>(ss => ss.IsEquivalent(template.Traits))))
                .Returns(mundaneWeapon);

            var cursedItem = curseGenerator.Generate(template);
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
            Assert.That(weapon.Size, Is.EqualTo(mundaneWeapon.Size).And.EqualTo(size));
            Assert.That(weapon.CriticalMultiplier, Is.EqualTo(mundaneWeapon.CriticalMultiplier));
            Assert.That(weapon.Damages, Is.EqualTo(mundaneWeapon.Damages));
            Assert.That(weapon.CriticalDamages, Is.EqualTo(mundaneWeapon.CriticalDamages));
            Assert.That(weapon.ThreatRangeSummary, Is.EqualTo(mundaneWeapon.ThreatRangeSummary));

            //INFO: Because all specific cursed items are magical, they are also all masterwork
            Assert.That(weapon.Traits, Contains.Item(TraitConstants.Masterwork)
                .And.SupersetOf(template.Traits.Take(2))
                .And.Not.Contains(size)
                .And.Count.EqualTo(3));
        }

        [Test]
        public void GenerateCustomSpecificCursedItemWithNoSpecialAbilities()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);

            var itemType = new[] { "item type" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemItemTypes, name)).Returns(itemType);

            var attributes = new[] { "attribute 1", "attribute 2" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemAttributes, name)).Returns(attributes);

            itemGroups[name] = new[] { "base name", "other base name" };

            var cursedItem = curseGenerator.Generate(template);
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
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemItemTypes, "specific cursed item")).Returns(itemType);

            var attributes = new[] { "attribute 1", "attribute 2" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemAttributes, "specific cursed item")).Returns(attributes);

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
        public void GenerateSpecificFromName_WithTraits()
        {
            var itemType = new[] { "item type" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemItemTypes, "specific cursed item")).Returns(itemType);

            var attributes = new[] { "attribute 1", "attribute 2" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemAttributes, "specific cursed item")).Returns(attributes);

            itemGroups["specific cursed item"] = new[] { "base name", "other base name" };
            itemGroups[CurseConstants.SpecificCursedItem] = new[] { "wrong specific cursed item", "specific cursed item", "other specific cursed item" };

            var cursedItem = curseGenerator.Generate("specific cursed item", "trait 1", "trait 2");
            Assert.That(cursedItem.Name, Is.EqualTo("specific cursed item"));
            Assert.That(cursedItem.BaseNames, Is.EquivalentTo(itemGroups["specific cursed item"]));
            Assert.That(cursedItem.IsMagical, Is.True);
            Assert.That(cursedItem.Magic.Curse, Is.EqualTo(CurseConstants.SpecificCursedItem));
            Assert.That(cursedItem.ItemType, Is.EqualTo("item type"));
            Assert.That(cursedItem.Attributes, Is.EquivalentTo(attributes));
            Assert.That(cursedItem, Is.Not.InstanceOf<Armor>()
                .And.Not.InstanceOf<Weapon>());
            Assert.That(cursedItem.Traits, Contains.Item("trait 1")
                .And.Contains("trait 2")
                .And.Count.EqualTo(2));
        }

        [Test]
        public void GenerateSpecificFromBaseName()
        {
            var itemType = new[] { "item type" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemItemTypes, "specific cursed item")).Returns(itemType);

            var attributes = new[] { "attribute 1", "attribute 2" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemAttributes, "specific cursed item")).Returns(attributes);

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
            Assert.That(cursedItem, Is.Not.InstanceOf<Armor>()
                .And.Not.InstanceOf<Weapon>());
        }

        [Test]
        public void GenerateSpecificFromBaseName_WithTraits()
        {
            var itemType = new[] { "item type" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemItemTypes, "specific cursed item")).Returns(itemType);

            var attributes = new[] { "attribute 1", "attribute 2" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemAttributes, "specific cursed item")).Returns(attributes);

            itemGroups["wrong specific cursed item"] = new[] { "wrong base name", "other base name" };
            itemGroups["other specific cursed item"] = new[] { "other base name" };
            itemGroups["specific cursed item"] = new[] { "base name", "other base name" };
            itemGroups[CurseConstants.SpecificCursedItem] = new[] { "wrong specific cursed item", "specific cursed item", "other specific cursed item" };

            var cursedItem = curseGenerator.Generate("base name", "trait 1", "trait 2");
            Assert.That(cursedItem.Name, Is.EqualTo("specific cursed item"));
            Assert.That(cursedItem.BaseNames, Is.EquivalentTo(itemGroups["specific cursed item"]));
            Assert.That(cursedItem.IsMagical, Is.True);
            Assert.That(cursedItem.Magic.Curse, Is.EqualTo(CurseConstants.SpecificCursedItem));
            Assert.That(cursedItem.ItemType, Is.EqualTo("item type"));
            Assert.That(cursedItem.Attributes, Is.EquivalentTo(attributes));
            Assert.That(cursedItem, Is.Not.InstanceOf<Armor>()
                .And.Not.InstanceOf<Weapon>());
            Assert.That(cursedItem.Traits, Contains.Item("trait 1")
                .And.Contains("trait 2")
                .And.Count.EqualTo(2));
        }

        [Test]
        public void GenerateSpecificFromAnyBaseName()
        {
            var itemType = new[] { "item type" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemItemTypes, "specific cursed item")).Returns(itemType);

            var otherItemType = new[] { "other item type" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemItemTypes, "other specific cursed item")).Returns(otherItemType);

            var attributes = new[] { "attribute 1", "attribute 2" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemAttributes, "specific cursed item")).Returns(attributes);

            var otherAttributes = new[] { "other attribute 1", "other attribute 2" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemAttributes, "other specific cursed item")).Returns(otherAttributes);

            itemGroups["wrong specific cursed item"] = new[] { "wrong base name", "other base name" };
            itemGroups["other specific cursed item"] = new[] { "base name" };
            itemGroups["specific cursed item"] = new[] { "base name", "other base name" };
            itemGroups[CurseConstants.SpecificCursedItem] = new[] { "wrong specific cursed item", "other specific cursed item", "specific cursed item" };

            var cursedItem = curseGenerator.Generate("base name");
            Assert.That(cursedItem.Name, Is.EqualTo("other specific cursed item"));
            Assert.That(cursedItem.BaseNames, Is.EquivalentTo(itemGroups["other specific cursed item"]));
            Assert.That(cursedItem.IsMagical, Is.True);
            Assert.That(cursedItem.Magic.Curse, Is.EqualTo(CurseConstants.SpecificCursedItem));
            Assert.That(cursedItem.ItemType, Is.EqualTo("other item type"));
            Assert.That(cursedItem.Attributes, Is.EquivalentTo(otherAttributes));
            Assert.That(cursedItem, Is.Not.InstanceOf<Armor>());
            Assert.That(cursedItem, Is.Not.InstanceOf<Weapon>());
        }

        [Test]
        public void GenerateSpecificArmorFromName()
        {
            var itemType = new[] { "item type" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemItemTypes, "specific cursed item")).Returns(new[] { ItemTypeConstants.Armor });

            var attributes = new[] { "attribute 1", "attribute 2" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemAttributes, "specific cursed item")).Returns(attributes);

            itemGroups["specific cursed item"] = new[] { "base name", "other base name" };
            itemGroups[CurseConstants.SpecificCursedItem] = new[] { "wrong specific cursed item", "specific cursed item", "other specific cursed item" };

            var mundaneArmor = itemVerifier.CreateRandomArmorTemplate("base name");
            mundaneArmor.Size = Guid.NewGuid().ToString();
            mockMundaneArmorGenerator.Setup(g => g.Generate("base name")).Returns(mundaneArmor);

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
        public void GenerateSpecificArmorFromName_WithTraits()
        {
            var itemType = new[] { "item type" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemItemTypes, "specific cursed item")).Returns(new[] { ItemTypeConstants.Armor });

            var attributes = new[] { "attribute 1", "attribute 2" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemAttributes, "specific cursed item")).Returns(attributes);

            itemGroups["specific cursed item"] = new[] { "base name", "other base name" };
            itemGroups[CurseConstants.SpecificCursedItem] = new[] { "wrong specific cursed item", "specific cursed item", "other specific cursed item" };

            var mundaneArmor = itemVerifier.CreateRandomArmorTemplate("base name");
            mundaneArmor.Size = Guid.NewGuid().ToString();
            mockMundaneArmorGenerator.Setup(g => g.Generate("base name", "trait 1", "trait 2")).Returns(mundaneArmor);

            var cursedItem = curseGenerator.Generate("specific cursed item", "trait 1", "trait 2");
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
            Assert.That(armor.Traits, Contains.Item(TraitConstants.Masterwork)
                .And.Contains("trait 1")
                .And.Contains("trait 2"));
        }

        [Test]
        public void GenerateSpecificArmorFromName_WithTraitsAndSize()
        {
            var itemType = new[] { "item type" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemItemTypes, "specific cursed item")).Returns(new[] { ItemTypeConstants.Armor });

            var attributes = new[] { "attribute 1", "attribute 2" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemAttributes, "specific cursed item")).Returns(attributes);

            itemGroups["specific cursed item"] = new[] { "base name", "other base name" };
            itemGroups[CurseConstants.SpecificCursedItem] = new[] { "wrong specific cursed item", "specific cursed item", "other specific cursed item" };

            var mundaneArmor = itemVerifier.CreateRandomArmorTemplate("base name");
            mundaneArmor.Size = "my size";
            mundaneArmor.Traits.Add("trait 1");

            mockMundaneArmorGenerator.Setup(g => g.Generate("base name", "trait 1", "my size")).Returns(mundaneArmor);

            var cursedItem = curseGenerator.Generate("specific cursed item", "trait 1", "my size");
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
            Assert.That(armor.Size, Is.EqualTo(mundaneArmor.Size).And.EqualTo("my size"));

            //INFO: Because all specific cursed items are magical, they are also all masterwork
            Assert.That(armor.Traits, Contains.Item(TraitConstants.Masterwork)
                .And.Contains("trait 1")
                .And.Not.Contains("my sizew"));
        }

        [Test]
        public void GenerateSpecificArmorFromBaseName()
        {
            var itemType = new[] { "item type" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemItemTypes, "specific cursed item")).Returns(new[] { ItemTypeConstants.Armor });

            var attributes = new[] { "attribute 1", "attribute 2" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemAttributes, "specific cursed item")).Returns(attributes);

            itemGroups["specific cursed item"] = new[] { "base name", "other base name" };
            itemGroups[CurseConstants.SpecificCursedItem] = new[] { "wrong specific cursed item", "specific cursed item", "other specific cursed item" };
            itemGroups["wrong specific cursed item"] = new[] { "wrong base name", "other base name" };
            itemGroups["other specific cursed item"] = new[] { "other base name" };
            itemGroups["another specific cursed item"] = new[] { "another base name", "other base name" };

            var mundaneArmor = itemVerifier.CreateRandomArmorTemplate("base name");
            mundaneArmor.Size = Guid.NewGuid().ToString();
            mockMundaneArmorGenerator.Setup(g => g.Generate("base name")).Returns(mundaneArmor);

            var cursedItem = curseGenerator.Generate("base name");
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
        public void GenerateSpecificArmorFromBaseName_WithTraits()
        {
            var itemType = new[] { "item type" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemItemTypes, "specific cursed item")).Returns(new[] { ItemTypeConstants.Armor });

            var attributes = new[] { "attribute 1", "attribute 2" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemAttributes, "specific cursed item")).Returns(attributes);

            itemGroups["specific cursed item"] = new[] { "base name", "other base name" };
            itemGroups[CurseConstants.SpecificCursedItem] = new[] { "wrong specific cursed item", "specific cursed item", "other specific cursed item" };
            itemGroups["wrong specific cursed item"] = new[] { "wrong base name", "other base name" };
            itemGroups["other specific cursed item"] = new[] { "other base name" };
            itemGroups["another specific cursed item"] = new[] { "another base name", "other base name" };

            var mundaneArmor = itemVerifier.CreateRandomArmorTemplate("base name");
            mundaneArmor.Size = Guid.NewGuid().ToString();
            mockMundaneArmorGenerator.Setup(g => g.Generate("base name", "trait 1", "trait 2")).Returns(mundaneArmor);

            var cursedItem = curseGenerator.Generate("base name", "trait 1", "trait 2");
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
            Assert.That(armor.Traits, Contains.Item(TraitConstants.Masterwork)
                .And.Contains("trait 1")
                .And.Contains("trait 2"));
        }

        [Test]
        public void GenerateSpecificArmorFromBaseName_WithTraitsAndSize()
        {
            var itemType = new[] { "item type" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemItemTypes, "specific cursed item")).Returns(new[] { ItemTypeConstants.Armor });

            var attributes = new[] { "attribute 1", "attribute 2" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemAttributes, "specific cursed item")).Returns(attributes);

            itemGroups["specific cursed item"] = new[] { "base name", "other base name" };
            itemGroups[CurseConstants.SpecificCursedItem] = new[] { "wrong specific cursed item", "specific cursed item", "other specific cursed item" };
            itemGroups["wrong specific cursed item"] = new[] { "wrong base name", "other base name" };
            itemGroups["other specific cursed item"] = new[] { "other base name" };
            itemGroups["another specific cursed item"] = new[] { "another base name", "other base name" };

            var mundaneArmor = itemVerifier.CreateRandomArmorTemplate("base name");
            mundaneArmor.Size = "my size";
            mundaneArmor.Traits.Add("trait 1");

            mockMundaneArmorGenerator.Setup(g => g.Generate("base name", "trait 1", "my size")).Returns(mundaneArmor);

            var cursedItem = curseGenerator.Generate("base name", "trait 1", "my size");
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
            Assert.That(armor.Size, Is.EqualTo(mundaneArmor.Size).And.EqualTo("my size"));

            //INFO: Because all specific cursed items are magical, they are also all masterwork
            Assert.That(armor.Traits, Contains.Item(TraitConstants.Masterwork)
                .And.Contains("trait 1")
                .And.Not.Contains("my size"));
        }

        [Test]
        public void GenerateSpecificWeaponFromName()
        {
            var itemType = new[] { "item type" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemItemTypes, "specific cursed item")).Returns(new[] { ItemTypeConstants.Weapon });

            var attributes = new[] { "attribute 1", "attribute 2" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemAttributes, "specific cursed item")).Returns(attributes);

            itemGroups["specific cursed item"] = new[] { "base name", "other base name" };
            itemGroups[CurseConstants.SpecificCursedItem] = new[] { "wrong specific cursed item", "specific cursed item", "other specific cursed item" };

            var mundaneWeapon = itemVerifier.CreateRandomWeaponTemplate("base name");
            mundaneWeapon.Size = Guid.NewGuid().ToString();
            mundaneWeapon.Quantity = 9266;
            mockMundaneWeaponGenerator.Setup(g => g.Generate("base name")).Returns(mundaneWeapon);

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
            Assert.That(weapon.Damages, Is.EqualTo(mundaneWeapon.Damages));
            Assert.That(weapon.CriticalDamages, Is.EqualTo(mundaneWeapon.CriticalDamages));
            Assert.That(weapon.ThreatRangeSummary, Is.EqualTo(mundaneWeapon.ThreatRangeSummary));

            //INFO: Because all specific cursed items are magical, they are also all masterwork
            Assert.That(weapon.Traits, Contains.Item(TraitConstants.Masterwork));
        }

        [Test]
        public void GenerateSpecificWeaponFromName_WithTraits()
        {
            var itemType = new[] { "item type" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemItemTypes, "specific cursed item")).Returns(new[] { ItemTypeConstants.Weapon });

            var attributes = new[] { "attribute 1", "attribute 2" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemAttributes, "specific cursed item")).Returns(attributes);

            itemGroups["specific cursed item"] = new[] { "base name", "other base name" };
            itemGroups[CurseConstants.SpecificCursedItem] = new[] { "wrong specific cursed item", "specific cursed item", "other specific cursed item" };

            var mundaneWeapon = itemVerifier.CreateRandomWeaponTemplate("base name");
            mundaneWeapon.Size = Guid.NewGuid().ToString();
            mundaneWeapon.Quantity = 9266;
            mockMundaneWeaponGenerator.Setup(g => g.Generate("base name", "trait 1", "trait 2")).Returns(mundaneWeapon);

            var cursedItem = curseGenerator.Generate("specific cursed item", "trait 1", "trait 2");
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
            Assert.That(weapon.Damages, Is.EqualTo(mundaneWeapon.Damages));
            Assert.That(weapon.CriticalDamages, Is.EqualTo(mundaneWeapon.CriticalDamages));
            Assert.That(weapon.ThreatRangeSummary, Is.EqualTo(mundaneWeapon.ThreatRangeSummary));

            //INFO: Because all specific cursed items are magical, they are also all masterwork
            Assert.That(weapon.Traits, Contains.Item(TraitConstants.Masterwork)
                .And.Contains("trait 1")
                .And.Contains("trait 2"));
        }

        [Test]
        public void GenerateSpecificWeaponFromName_WithTraitsAndSize()
        {
            var itemType = new[] { "item type" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemItemTypes, "specific cursed item")).Returns(new[] { ItemTypeConstants.Weapon });

            var attributes = new[] { "attribute 1", "attribute 2" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemAttributes, "specific cursed item")).Returns(attributes);

            itemGroups["specific cursed item"] = new[] { "base name", "other base name" };
            itemGroups[CurseConstants.SpecificCursedItem] = new[] { "wrong specific cursed item", "specific cursed item", "other specific cursed item" };

            var mundaneWeapon = itemVerifier.CreateRandomWeaponTemplate("base name");
            mundaneWeapon.Size = "my size";
            mundaneWeapon.Quantity = 9266;
            mundaneWeapon.Traits.Add("trait 1");

            mockMundaneWeaponGenerator.Setup(g => g.Generate("base name", "trait 1", "my size")).Returns(mundaneWeapon);

            var cursedItem = curseGenerator.Generate("specific cursed item", "trait 1", "my size");
            Assert.That(cursedItem.Name, Is.EqualTo("specific cursed item"));
            Assert.That(cursedItem.BaseNames, Is.EquivalentTo(itemGroups["specific cursed item"]));
            Assert.That(cursedItem.Quantity, Is.EqualTo(9266));
            Assert.That(cursedItem.IsMagical, Is.True);
            Assert.That(cursedItem.Magic.Curse, Is.EqualTo(CurseConstants.SpecificCursedItem));
            Assert.That(cursedItem.ItemType, Is.EqualTo(ItemTypeConstants.Weapon));
            Assert.That(cursedItem.Attributes, Is.EquivalentTo(attributes));
            Assert.That(cursedItem, Is.InstanceOf<Weapon>());

            var weapon = cursedItem as Weapon;
            Assert.That(weapon.Size, Is.EqualTo(mundaneWeapon.Size).And.EqualTo("my size"));
            Assert.That(weapon.CriticalMultiplier, Is.EqualTo(mundaneWeapon.CriticalMultiplier));
            Assert.That(weapon.Damages, Is.EqualTo(mundaneWeapon.Damages));
            Assert.That(weapon.CriticalDamages, Is.EqualTo(mundaneWeapon.CriticalDamages));
            Assert.That(weapon.ThreatRangeSummary, Is.EqualTo(mundaneWeapon.ThreatRangeSummary));

            //INFO: Because all specific cursed items are magical, they are also all masterwork
            Assert.That(weapon.Traits, Contains.Item(TraitConstants.Masterwork)
                .And.Contains("trait 1")
                .And.Not.Contains("my size"));
        }

        [Test]
        public void GenerateSpecificWeaponFromBaseName()
        {
            var itemType = new[] { "item type" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemItemTypes, "specific cursed item")).Returns(new[] { ItemTypeConstants.Weapon });

            var attributes = new[] { "attribute 1", "attribute 2" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemAttributes, "specific cursed item")).Returns(attributes);

            itemGroups["specific cursed item"] = new[] { "base name", "other base name" };
            itemGroups["wrong specific cursed item"] = new[] { "wrong base name", "other base name" };
            itemGroups["other specific cursed item"] = new[] { "other base name" };
            itemGroups[CurseConstants.SpecificCursedItem] = new[] { "wrong specific cursed item", "specific cursed item", "other specific cursed item" };

            var mundaneWeapon = itemVerifier.CreateRandomWeaponTemplate("base name");
            mundaneWeapon.Size = Guid.NewGuid().ToString();
            mundaneWeapon.Quantity = 9266;
            mockMundaneWeaponGenerator.Setup(g => g.Generate("base name")).Returns(mundaneWeapon);

            var cursedItem = curseGenerator.Generate("base name");
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
            Assert.That(weapon.Damages, Is.EqualTo(mundaneWeapon.Damages));
            Assert.That(weapon.CriticalDamages, Is.EqualTo(mundaneWeapon.CriticalDamages));
            Assert.That(weapon.ThreatRangeSummary, Is.EqualTo(mundaneWeapon.ThreatRangeSummary));

            //INFO: Because all specific cursed items are magical, they are also all masterwork
            Assert.That(weapon.Traits, Contains.Item(TraitConstants.Masterwork));
        }

        [Test]
        public void GenerateSpecificWeaponFromBaseName_WithTraits()
        {
            var itemType = new[] { "item type" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemItemTypes, "specific cursed item")).Returns(new[] { ItemTypeConstants.Weapon });

            var attributes = new[] { "attribute 1", "attribute 2" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemAttributes, "specific cursed item")).Returns(attributes);

            itemGroups["specific cursed item"] = new[] { "base name", "other base name" };
            itemGroups["wrong specific cursed item"] = new[] { "wrong base name", "other base name" };
            itemGroups["other specific cursed item"] = new[] { "other base name" };
            itemGroups[CurseConstants.SpecificCursedItem] = new[] { "wrong specific cursed item", "specific cursed item", "other specific cursed item" };

            var mundaneWeapon = itemVerifier.CreateRandomWeaponTemplate("base name");
            mundaneWeapon.Size = Guid.NewGuid().ToString();
            mundaneWeapon.Quantity = 9266;
            mockMundaneWeaponGenerator.Setup(g => g.Generate("base name", "trait 1", "trait 2")).Returns(mundaneWeapon);

            var cursedItem = curseGenerator.Generate("base name", "trait 1", "trait 2");
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
            Assert.That(weapon.Damages, Is.EqualTo(mundaneWeapon.Damages));
            Assert.That(weapon.CriticalDamages, Is.EqualTo(mundaneWeapon.CriticalDamages));
            Assert.That(weapon.ThreatRangeSummary, Is.EqualTo(mundaneWeapon.ThreatRangeSummary));

            //INFO: Because all specific cursed items are magical, they are also all masterwork
            Assert.That(weapon.Traits, Contains.Item(TraitConstants.Masterwork)
                .And.Contains("trait 1")
                .And.Contains("trait 2"));
        }

        [Test]
        public void GenerateSpecificWeaponFromBaseName_WithTraitsAndSize()
        {
            var itemType = new[] { "item type" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemItemTypes, "specific cursed item")).Returns(new[] { ItemTypeConstants.Weapon });

            var attributes = new[] { "attribute 1", "attribute 2" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemAttributes, "specific cursed item")).Returns(attributes);

            itemGroups["specific cursed item"] = new[] { "base name", "other base name" };
            itemGroups["wrong specific cursed item"] = new[] { "wrong base name", "other base name" };
            itemGroups["other specific cursed item"] = new[] { "other base name" };
            itemGroups[CurseConstants.SpecificCursedItem] = new[] { "wrong specific cursed item", "specific cursed item", "other specific cursed item" };

            var mundaneWeapon = itemVerifier.CreateRandomWeaponTemplate("base name");
            mundaneWeapon.Size = "my size";
            mundaneWeapon.Quantity = 9266;
            mundaneWeapon.Traits.Add("trait 1");

            mockMundaneWeaponGenerator.Setup(g => g.Generate("base name", "trait 1", "my size")).Returns(mundaneWeapon);

            var cursedItem = curseGenerator.Generate("base name", "trait 1", "my size");
            Assert.That(cursedItem.Name, Is.EqualTo("specific cursed item"));
            Assert.That(cursedItem.BaseNames, Is.EquivalentTo(itemGroups["specific cursed item"]));
            Assert.That(cursedItem.Quantity, Is.EqualTo(9266));
            Assert.That(cursedItem.IsMagical, Is.True);
            Assert.That(cursedItem.Magic.Curse, Is.EqualTo(CurseConstants.SpecificCursedItem));
            Assert.That(cursedItem.ItemType, Is.EqualTo(ItemTypeConstants.Weapon));
            Assert.That(cursedItem.Attributes, Is.EquivalentTo(attributes));
            Assert.That(cursedItem, Is.InstanceOf<Weapon>());

            var weapon = cursedItem as Weapon;
            Assert.That(weapon.Size, Is.EqualTo(mundaneWeapon.Size).And.EqualTo("my size"));
            Assert.That(weapon.CriticalMultiplier, Is.EqualTo(mundaneWeapon.CriticalMultiplier));
            Assert.That(weapon.Damages, Is.EqualTo(mundaneWeapon.Damages));
            Assert.That(weapon.CriticalDamages, Is.EqualTo(mundaneWeapon.CriticalDamages));
            Assert.That(weapon.ThreatRangeSummary, Is.EqualTo(mundaneWeapon.ThreatRangeSummary));

            //INFO: Because all specific cursed items are magical, they are also all masterwork
            Assert.That(weapon.Traits, Contains.Item(TraitConstants.Masterwork)
                .And.Contains("trait 1")
                .And.Not.Contains("my size"));
        }

        [Test]
        public void DoNotGenerateSpecificFromName_IfNotSpecific_AndBaseNameDoesNotMatch()
        {
            var itemType = new[] { "item type" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemItemTypes, "specific cursed item")).Returns(itemType);
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemItemTypes, "other specific cursed item")).Returns(itemType);

            var attributes = new[] { "attribute 1", "attribute 2" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemAttributes, "specific cursed item")).Returns(attributes);

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

        [Test]
        public void GenerateSpecificFromItemType()
        {
            var itemTypes = new Dictionary<string, IEnumerable<string>>();
            itemTypes["specific cursed item"] = new[] { "item type" };
            itemTypes["other specific cursed item"] = new[] { "other item type", "item type" };
            itemTypes["wrong specific cursed item"] = new[] { "wrong item type" };

            mockCollectionsSelector
                .Setup(s => s.SelectAllFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemItemTypes))
                .Returns(itemTypes);
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemItemTypes, It.IsAny<string>()))
                .Returns((string a, string t, string n) => itemTypes[n]);

            var attributes = new[] { "attribute 1", "attribute 2" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemAttributes, "specific cursed item")).Returns(attributes);

            itemGroups["specific cursed item"] = new[] { "base name", "other base name" };
            itemGroups[CurseConstants.SpecificCursedItem] = new[] { "wrong specific cursed item", "specific cursed item", "other specific cursed item" };

            var cursedItem = curseGenerator.GenerateSpecificCursedItem("item type");
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
        public void GenerateSpecificFromAnyItemType()
        {
            var itemTypes = new Dictionary<string, IEnumerable<string>>();
            itemTypes["other specific cursed item"] = new[] { "item type" };
            itemTypes["specific cursed item"] = new[] { "item type" };
            itemTypes["wrong specific cursed item"] = new[] { "wrong item type" };

            mockCollectionsSelector
                .Setup(s => s.SelectAllFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemItemTypes))
                .Returns(itemTypes);
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemItemTypes, It.IsAny<string>()))
                .Returns((string a, string t, string n) => itemTypes[n]);

            var attributes = new[] { "attribute 1", "attribute 2" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemAttributes, "specific cursed item")).Returns(attributes);

            var otherAttributes = new[] { "other attribute 1", "other attribute 2" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemAttributes, "other specific cursed item")).Returns(otherAttributes);

            itemGroups["wrong specific cursed item"] = new[] { "wrong base name", "other base name" };
            itemGroups["other specific cursed item"] = new[] { "base name" };
            itemGroups["specific cursed item"] = new[] { "base name", "other base name" };
            itemGroups[CurseConstants.SpecificCursedItem] = new[] { "wrong specific cursed item", "other specific cursed item", "specific cursed item" };

            var cursedItem = curseGenerator.GenerateSpecificCursedItem("item type");
            Assert.That(cursedItem.Name, Is.EqualTo("other specific cursed item"));
            Assert.That(cursedItem.BaseNames, Is.EquivalentTo(itemGroups["other specific cursed item"]));
            Assert.That(cursedItem.IsMagical, Is.True);
            Assert.That(cursedItem.Magic.Curse, Is.EqualTo(CurseConstants.SpecificCursedItem));
            Assert.That(cursedItem.ItemType, Is.EqualTo("item type"));
            Assert.That(cursedItem.Attributes, Is.EquivalentTo(otherAttributes));
            Assert.That(cursedItem, Is.Not.InstanceOf<Armor>());
            Assert.That(cursedItem, Is.Not.InstanceOf<Weapon>());
        }

        [Test]
        public void GenerateSpecificArmorFromItemType()
        {
            var itemTypes = new Dictionary<string, IEnumerable<string>>();
            itemTypes["other specific cursed item"] = new[] { "other item type", "item type" };
            itemTypes["specific cursed item"] = new[] { ItemTypeConstants.Armor };
            itemTypes["wrong specific cursed item"] = new[] { "wrong item type" };

            mockCollectionsSelector
                .Setup(s => s.SelectAllFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemItemTypes))
                .Returns(itemTypes);
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemItemTypes, It.IsAny<string>()))
                .Returns((string a, string t, string n) => itemTypes[n]);

            var attributes = new[] { "attribute 1", "attribute 2" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemAttributes, "specific cursed item")).Returns(attributes);

            itemGroups["specific cursed item"] = new[] { "base name", "other base name" };
            itemGroups[CurseConstants.SpecificCursedItem] = new[] { "wrong specific cursed item", "specific cursed item", "other specific cursed item" };

            var mundaneArmor = itemVerifier.CreateRandomArmorTemplate("base name");
            mundaneArmor.Size = Guid.NewGuid().ToString();
            mockMundaneArmorGenerator.Setup(g => g.Generate("base name")).Returns(mundaneArmor);

            var cursedItem = curseGenerator.GenerateSpecificCursedItem(ItemTypeConstants.Armor);
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
        public void GenerateSpecificWeaponFromItemType()
        {
            var itemTypes = new Dictionary<string, IEnumerable<string>>();
            itemTypes["other specific cursed item"] = new[] { "other item type", "item type" };
            itemTypes["specific cursed item"] = new[] { ItemTypeConstants.Weapon };
            itemTypes["wrong specific cursed item"] = new[] { "wrong item type" };

            mockCollectionsSelector
                .Setup(s => s.SelectAllFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemItemTypes))
                .Returns(itemTypes);
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemItemTypes, It.IsAny<string>()))
                .Returns((string a, string t, string n) => itemTypes[n]);

            var attributes = new[] { "attribute 1", "attribute 2" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemAttributes, "specific cursed item")).Returns(attributes);

            itemGroups["specific cursed item"] = new[] { "base name", "other base name" };
            itemGroups[CurseConstants.SpecificCursedItem] = new[] { "wrong specific cursed item", "specific cursed item", "other specific cursed item" };

            var mundaneWeapon = itemVerifier.CreateRandomWeaponTemplate("base name");
            mundaneWeapon.Size = Guid.NewGuid().ToString();
            mundaneWeapon.Quantity = 9266;
            mockMundaneWeaponGenerator.Setup(g => g.Generate("base name")).Returns(mundaneWeapon);

            var cursedItem = curseGenerator.GenerateSpecificCursedItem(ItemTypeConstants.Weapon);
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
            Assert.That(weapon.Damages, Is.EqualTo(mundaneWeapon.Damages));
            Assert.That(weapon.CriticalDamages, Is.EqualTo(mundaneWeapon.CriticalDamages));
            Assert.That(weapon.ThreatRangeSummary, Is.EqualTo(mundaneWeapon.ThreatRangeSummary));

            //INFO: Because all specific cursed items are magical, they are also all masterwork
            Assert.That(weapon.Traits, Contains.Item(TraitConstants.Masterwork));
        }

        [Test]
        public void DoNotGenerateSpecificFromName_IfItemTypeDoesNotMatch()
        {
            var itemTypes = new Dictionary<string, IEnumerable<string>>();
            itemTypes["other specific cursed item"] = new[] { "other item type", "item type" };
            itemTypes["specific cursed item"] = new[] { "item type" };

            mockCollectionsSelector
                .Setup(s => s.SelectAllFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemItemTypes))
                .Returns(itemTypes);
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemItemTypes, It.IsAny<string>()))
                .Returns((string a, string t, string n) => itemTypes[n]);

            var attributes = new[] { "attribute 1", "attribute 2" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemAttributes, "specific cursed item")).Returns(attributes);

            itemGroups["wrong specific cursed item"] = new[] { "wrong base name", "other base name" };
            itemGroups["other specific cursed item"] = new[] { "other base name" };
            itemGroups["specific cursed item"] = new[] { "base name", "other base name" };
            itemGroups[CurseConstants.SpecificCursedItem] = new[] { "specific cursed item", "other specific cursed item" };

            var cursedItem = curseGenerator.GenerateSpecificCursedItem("wrong item type");
            Assert.That(cursedItem, Is.Null);
        }

        [Test]
        public void ItemTypeCanBeSpecificCursedItem_ReturnsTrue()
        {
            var itemTypes = new Dictionary<string, IEnumerable<string>>();
            itemTypes["other specific cursed item"] = new[] { "other item type", "item type" };
            itemTypes["specific cursed item"] = new[] { "item type" };

            mockCollectionsSelector
                .Setup(s => s.SelectAllFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemItemTypes))
                .Returns(itemTypes);
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemItemTypes, It.IsAny<string>()))
                .Returns((string a, string t, string n) => itemTypes[n]);

            var canBeSpecificCursed = curseGenerator.ItemTypeCanBeSpecificCursedItem("item type");
            Assert.That(canBeSpecificCursed, Is.True);
        }

        [Test]
        public void ItemTypeCanBeSpecificCursedItem_ReturnsFalse()
        {
            var itemTypes = new Dictionary<string, IEnumerable<string>>();
            itemTypes["other specific cursed item"] = new[] { "other item type", "item type" };
            itemTypes["specific cursed item"] = new[] { "item type" };

            mockCollectionsSelector
                .Setup(s => s.SelectAllFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemItemTypes))
                .Returns(itemTypes);
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecificCursedItemItemTypes, It.IsAny<string>()))
                .Returns((string a, string t, string n) => itemTypes[n]);

            var canBeSpecificCursed = curseGenerator.ItemTypeCanBeSpecificCursedItem("wrong item type");
            Assert.That(canBeSpecificCursed, Is.False);
        }
    }
}