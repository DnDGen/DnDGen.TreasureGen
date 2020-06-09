﻿using DnDGen.Infrastructure.Generators;
using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.TreasureGen.Generators.Items;
using DnDGen.TreasureGen.Generators.Items.Magical;
using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Items.Mundane;
using DnDGen.TreasureGen.Selectors.Percentiles;
using DnDGen.TreasureGen.Selectors.Selections;
using DnDGen.TreasureGen.Tables;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.TreasureGen.Tests.Unit.Generators.Items
{
    [TestFixture]
    public class SpecificGearGeneratorTests
    {
        private ISpecificGearGenerator specificGearGenerator;
        private Mock<ITypeAndAmountPercentileSelector> mockTypeAndAmountPercentileSelector;
        private Mock<ICollectionSelector> mockCollectionsSelector;
        private Mock<ISpecialAbilitiesGenerator> mockSpecialAbilitiesGenerator;
        private Mock<IChargesGenerator> mockChargesGenerator;
        private Mock<ISpellGenerator> mockSpellGenerator;
        private Mock<ITreasurePercentileSelector> mockPercentileSelector;
        private Mock<MundaneItemGenerator> mockMundaneArmorGenerator;
        private Mock<MundaneItemGenerator> mockMundaneWeaponGenerator;
        private Mock<IReplacementSelector> mockReplacementSelector;
        private TypeAndAmountSelection selection;
        private string power;
        private string gearType;
        private ItemVerifier itemVerifier;
        private List<string> baseNames;
        private Armor mundaneArmor;
        private Weapon mundaneWeapon;
        private Item template;

        [SetUp]
        public void Setup()
        {
            mockTypeAndAmountPercentileSelector = new Mock<ITypeAndAmountPercentileSelector>();
            mockCollectionsSelector = new Mock<ICollectionSelector>();
            mockSpecialAbilitiesGenerator = new Mock<ISpecialAbilitiesGenerator>();
            mockChargesGenerator = new Mock<IChargesGenerator>();
            mockSpellGenerator = new Mock<ISpellGenerator>();
            mockPercentileSelector = new Mock<ITreasurePercentileSelector>();
            mockMundaneArmorGenerator = new Mock<MundaneItemGenerator>();
            mockMundaneWeaponGenerator = new Mock<MundaneItemGenerator>();
            var mockJustInTimeFactory = new Mock<JustInTimeFactory>();
            mockReplacementSelector = new Mock<IReplacementSelector>();

            mockJustInTimeFactory.Setup(f => f.Build<MundaneItemGenerator>(ItemTypeConstants.Armor)).Returns(mockMundaneArmorGenerator.Object);
            mockJustInTimeFactory.Setup(f => f.Build<MundaneItemGenerator>(ItemTypeConstants.Weapon)).Returns(mockMundaneWeaponGenerator.Object);

            specificGearGenerator = new SpecificGearGenerator(
                mockTypeAndAmountPercentileSelector.Object,
                mockCollectionsSelector.Object,
                mockChargesGenerator.Object,
                mockPercentileSelector.Object,
                mockSpellGenerator.Object,
                mockSpecialAbilitiesGenerator.Object,
                mockJustInTimeFactory.Object,
                mockReplacementSelector.Object);

            selection = new TypeAndAmountSelection();
            itemVerifier = new ItemVerifier();
            baseNames = new List<string> { "base name" };
            mundaneArmor = itemVerifier.CreateRandomArmorTemplate(baseNames[0]);
            mundaneWeapon = itemVerifier.CreateRandomWeaponTemplate(baseNames[0]);
            template = new Item();

            mundaneArmor.Contents.Clear();
            mundaneWeapon.Contents.Clear();

            power = "power";
            gearType = "gear type";

            selection.Type = ArmorConstants.BandedMailOfLuck;
            selection.Amount = 9266;

            template.Magic.Bonus = selection.Amount;
            template.Name = selection.Type;

            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERSpecificITEMTYPEs, power, gearType);
            mockTypeAndAmountPercentileSelector.Setup(s => s.SelectFrom(tableName)).Returns(selection);

            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, It.IsAny<string>())).Returns((string table, string name) => new[] { name });
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, selection.Type)).Returns(baseNames);

            mockMundaneArmorGenerator.Setup(g => g.Generate(It.IsAny<string>(), It.IsAny<string[]>())).Returns(new Armor());
            mockMundaneWeaponGenerator.Setup(g => g.Generate(It.IsAny<string>(), It.IsAny<string[]>())).Returns(new Weapon());

            mockMundaneArmorGenerator.Setup(g => g.Generate(baseNames[0], It.IsAny<string[]>())).Returns(mundaneArmor);
            mockMundaneWeaponGenerator.Setup(g => g.Generate(baseNames[0], It.IsAny<string[]>())).Returns(mundaneWeapon);

            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(It.IsAny<IEnumerable<TypeAndAmountSelection>>()))
                .Returns((IEnumerable<TypeAndAmountSelection> ss) => ss.Last());

            mockReplacementSelector
                .Setup(s => s.SelectSingle(It.IsAny<string>()))
                .Returns((string s) => s);
        }

        [Test]
        public void ReturnPrototype()
        {
            selection.Type = WeaponConstants.HolyAvenger;

            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERSpecificITEMTYPEs, power, gearType);
            mockTypeAndAmountPercentileSelector
                .Setup(s => s.SelectAllFrom(tableName))
                .Returns(new[]
                {
                    new TypeAndAmountSelection { Type = "another item name", Amount = 90210 },
                    selection
                });

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.PowerGroups, WeaponConstants.HolyAvenger))
                .Returns(new[] { "wrong power", power, "other power" });

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, WeaponConstants.HolyAvenger))
                .Returns(baseNames);

            var prototype = specificGearGenerator.GeneratePrototypeFrom(power, gearType, WeaponConstants.HolyAvenger);
            Assert.That(prototype, Is.Not.Null);
            Assert.That(prototype.BaseNames, Is.EqualTo(baseNames));
            Assert.That(prototype.Name, Is.EqualTo(WeaponConstants.HolyAvenger));
            Assert.That(prototype.Magic.Bonus, Is.EqualTo(9266));
            Assert.That(prototype.IsMagical, Is.True);
        }

        [TestCase(ItemTypeConstants.Armor, ItemTypeConstants.Armor)]
        [TestCase(AttributeConstants.Shield, ItemTypeConstants.Armor)]
        [TestCase(ItemTypeConstants.Weapon, ItemTypeConstants.Weapon)]
        public void CorrectItemType(string gearType, string itemType)
        {
            selection.Type = "item name";

            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERSpecificITEMTYPEs, power, gearType);
            mockTypeAndAmountPercentileSelector
                .Setup(s => s.SelectAllFrom(tableName))
                .Returns(new[]
                {
                    new TypeAndAmountSelection { Type = "another item name", Amount = 9266 },
                    selection
                });

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.PowerGroups, "item name"))
                .Returns(new[] { "wrong power", power, "other power" });

            var prototype = specificGearGenerator.GeneratePrototypeFrom(power, gearType, "item name");
            Assert.That(prototype.ItemType, Is.EqualTo(itemType));
        }

        [Test]
        public void ReturnMundanePrototype()
        {
            selection.Amount = 0;
            selection.Type = WeaponConstants.Battleaxe_Adamantine;

            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERSpecificITEMTYPEs, power, gearType);
            mockTypeAndAmountPercentileSelector
                .Setup(s => s.SelectAllFrom(tableName))
                .Returns(new[]
                {
                    new TypeAndAmountSelection { Type = "another item name", Amount = 9266 },
                    selection
                });

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.PowerGroups, WeaponConstants.Battleaxe_Adamantine))
                .Returns(new[] { "wrong power", power, "other power" });

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, WeaponConstants.Battleaxe_Adamantine))
                .Returns(baseNames);

            var prototype = specificGearGenerator.GeneratePrototypeFrom(power, gearType, WeaponConstants.Battleaxe_Adamantine);
            Assert.That(prototype, Is.Not.Null);
            Assert.That(prototype.BaseNames, Is.EqualTo(baseNames));
            Assert.That(prototype.Name, Is.EqualTo(WeaponConstants.Battleaxe_Adamantine));
            Assert.That(prototype.Magic.Bonus, Is.Zero);
            Assert.That(prototype.IsMagical, Is.False);
        }

        [Test]
        public void MagicGearIsMasterwork()
        {
            var gear = specificGearGenerator.GenerateFrom(template);
            Assert.That(gear.IsMagical, Is.True);
            Assert.That(gear.Magic.Bonus, Is.EqualTo(9266));
            Assert.That(gear.Traits, Contains.Item(TraitConstants.Masterwork));
        }

        [Test]
        public void SpecificallyMasterworkMagicGearIsMasterwork()
        {
            var traits = new[] { "trait 1", "trait 2", TraitConstants.Masterwork };
            var tableName = string.Format(TableNameConstants.Collections.Formattable.SpecificITEMTYPETraits, gearType);
            mockCollectionsSelector.Setup(s => s.SelectFrom(tableName, selection.Type)).Returns(traits);

            var gear = specificGearGenerator.GenerateFrom(template);
            Assert.That(gear.IsMagical, Is.True);
            Assert.That(gear.Magic.Bonus, Is.EqualTo(9266));
            Assert.That(gear.Traits, Contains.Item(TraitConstants.Masterwork));
        }

        [Test]
        public void NonMagicGearIsNotMasterwork()
        {
            template.Magic.Bonus = 0;

            var traits = new[] { "trait 1", "trait 2" };
            var tableName = string.Format(TableNameConstants.Collections.Formattable.SpecificITEMTYPETraits, gearType);
            mockCollectionsSelector.Setup(s => s.SelectFrom(tableName, selection.Type)).Returns(traits);

            var gear = specificGearGenerator.GenerateFrom(template);
            Assert.That(gear.IsMagical, Is.False);
            Assert.That(gear.Traits, Is.All.Not.EqualTo(TraitConstants.Masterwork));
        }

        [Test]
        public void NonMagicSpecificallyMasterworkGearIsMasterwork()
        {
            template.Magic.Bonus = 0;

            var traits = new[] { "trait 1", "trait 2", TraitConstants.Masterwork };
            var tableName = string.Format(TableNameConstants.Collections.Formattable.SpecificITEMTYPETraits, ItemTypeConstants.Armor);
            mockCollectionsSelector.Setup(s => s.SelectFrom(tableName, template.Name)).Returns(traits);

            var gear = specificGearGenerator.GenerateFrom(template);
            Assert.That(gear.IsMagical, Is.False);
            Assert.That(gear.Traits, Contains.Item(TraitConstants.Masterwork));
        }

        [Test]
        public void GetAttributesFromSelector()
        {
            var attributes = new[] { "attribute 1", "attribute 2" };
            var tableName = string.Format(TableNameConstants.Collections.Formattable.SpecificITEMTYPEAttributes, ItemTypeConstants.Armor);
            mockCollectionsSelector.Setup(s => s.SelectFrom(tableName, selection.Type)).Returns(attributes);

            var gear = specificGearGenerator.GenerateFrom(template);
            Assert.That(gear.Attributes, Is.EqualTo(attributes));
        }

        [Test]
        public void GetTraitsFromSelector()
        {
            var traits = new[] { "trait 1", "trait 2" };
            var tableName = string.Format(TableNameConstants.Collections.Formattable.SpecificITEMTYPETraits, ItemTypeConstants.Armor);
            mockCollectionsSelector.Setup(s => s.SelectFrom(tableName, selection.Type)).Returns(traits);

            var gear = specificGearGenerator.GenerateFrom(template);
            Assert.That(gear.Traits, Is.SupersetOf(traits));
        }

        [Test]
        public void GetSpecificArmor()
        {
            template.ItemType = ItemTypeConstants.Armor;

            var attributes = new[] { "attribute 1", "attribute 2" };
            var tableName = string.Format(TableNameConstants.Collections.Formattable.SpecificITEMTYPEAttributes, ItemTypeConstants.Armor);
            mockCollectionsSelector.Setup(s => s.SelectFrom(tableName, template.Name)).Returns(attributes);

            var traits = new[] { "trait 1", "trait 2" };
            tableName = string.Format(TableNameConstants.Collections.Formattable.SpecificITEMTYPETraits, ItemTypeConstants.Armor);
            mockCollectionsSelector.Setup(s => s.SelectFrom(tableName, template.Name)).Returns(traits);

            var specialAbilityNames = new[] { "ability 1", "ability 2" };
            tableName = string.Format(TableNameConstants.Collections.Formattable.SpecificITEMTYPESpecialAbilities, ItemTypeConstants.Armor);
            mockCollectionsSelector.Setup(s => s.SelectFrom(tableName, template.Name)).Returns(specialAbilityNames);

            var specialAbilities = new[]
            {
                new SpecialAbility(),
                new SpecialAbility()
            };

            mockSpecialAbilitiesGenerator.Setup(s => s.GenerateFor(
                It.Is<IEnumerable<SpecialAbility>>(aa =>
                    aa.First().Name == "ability 1"
                    && aa.Last().Name == "ability 2")
                )
            ).Returns(specialAbilities);

            var gear = specificGearGenerator.GenerateFrom(template);
            Assert.That(gear.Name, Is.EqualTo(selection.Type));
            Assert.That(gear.BaseNames, Is.EquivalentTo(baseNames));
            Assert.That(gear.ItemType, Is.EqualTo(ItemTypeConstants.Armor));
            Assert.That(gear.Attributes, Is.EqualTo(attributes));
            Assert.That(gear.Traits, Is.SupersetOf(traits));
            Assert.That(gear.Magic.Bonus, Is.EqualTo(selection.Amount));
            Assert.That(gear.Magic.SpecialAbilities, Is.EqualTo(specialAbilities));
            Assert.That(gear, Is.InstanceOf<Armor>());

            var armor = gear as Armor;
            Assert.That(armor.ArmorBonus, Is.EqualTo(mundaneArmor.ArmorBonus));
            Assert.That(armor.ArmorCheckPenalty, Is.EqualTo(mundaneArmor.ArmorCheckPenalty));
            Assert.That(armor.MaxDexterityBonus, Is.EqualTo(mundaneArmor.MaxDexterityBonus));
            Assert.That(armor.Size, Is.EqualTo(mundaneArmor.Size));
        }

        [Test]
        public void GetSpecificWeapon()
        {
            template.Name = WeaponConstants.HolyAvenger;

            var attributes = new[] { "attribute 1", "attribute 2" };
            var tableName = string.Format(TableNameConstants.Collections.Formattable.SpecificITEMTYPEAttributes, ItemTypeConstants.Weapon);
            mockCollectionsSelector.Setup(s => s.SelectFrom(tableName, template.Name)).Returns(attributes);

            var traits = new[] { "trait 1", "trait 2" };
            tableName = string.Format(TableNameConstants.Collections.Formattable.SpecificITEMTYPETraits, ItemTypeConstants.Weapon);
            mockCollectionsSelector.Setup(s => s.SelectFrom(tableName, template.Name)).Returns(traits);

            var specialAbilityNames = new[] { "ability 1", "ability 2" };
            tableName = string.Format(TableNameConstants.Collections.Formattable.SpecificITEMTYPESpecialAbilities, ItemTypeConstants.Weapon);
            mockCollectionsSelector.Setup(s => s.SelectFrom(tableName, template.Name)).Returns(specialAbilityNames);

            var specialAbilities = new[]
            {
                new SpecialAbility(),
                new SpecialAbility()
            };

            mockSpecialAbilitiesGenerator.Setup(s => s.GenerateFor(
                It.Is<IEnumerable<SpecialAbility>>(aa =>
                    aa.First().Name == "ability 1"
                    && aa.Last().Name == "ability 2")
                )
            ).Returns(specialAbilities);

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, template.Name))
                .Returns(baseNames);

            var gear = specificGearGenerator.GenerateFrom(template);
            Assert.That(gear, Is.Not.EqualTo(template));
            Assert.That(gear.Name, Is.EqualTo(template.Name));
            Assert.That(gear.BaseNames, Is.EquivalentTo(baseNames));
            Assert.That(gear.ItemType, Is.EqualTo(ItemTypeConstants.Weapon));
            Assert.That(gear.Attributes, Is.EqualTo(attributes));
            Assert.That(gear.Traits, Is.SupersetOf(traits));
            Assert.That(gear.Magic.Bonus, Is.EqualTo(selection.Amount));
            Assert.That(gear.Magic.SpecialAbilities, Is.EqualTo(specialAbilities));
            Assert.That(gear, Is.InstanceOf<Weapon>());

            var weapon = gear as Weapon;
            Assert.That(weapon.CriticalMultiplier, Is.EqualTo(mundaneWeapon.CriticalMultiplier));
            Assert.That(weapon.Damage, Is.EqualTo(mundaneWeapon.Damage));
            Assert.That(weapon.DamageType, Is.EqualTo(mundaneWeapon.DamageType));
            Assert.That(weapon.Size, Is.EqualTo(mundaneWeapon.Size));
            Assert.That(weapon.ThreatRange, Is.EqualTo(mundaneWeapon.ThreatRange));
            Assert.That(weapon.Quantity, Is.EqualTo(mundaneWeapon.Quantity));
            Assert.That(mundaneWeapon.Quantity, Is.AtLeast(2));
        }

        [Test]
        public void GetSpecificArmor_WithTraits()
        {
            template.ItemType = ItemTypeConstants.Armor;
            template.Traits.Add("my trait");
            template.Traits.Add("my other trait");

            var attributes = new[] { "attribute 1", "attribute 2" };
            var tableName = string.Format(TableNameConstants.Collections.Formattable.SpecificITEMTYPEAttributes, ItemTypeConstants.Armor);
            mockCollectionsSelector.Setup(s => s.SelectFrom(tableName, template.Name)).Returns(attributes);

            var traits = new[] { "trait 1", "trait 2" };
            tableName = string.Format(TableNameConstants.Collections.Formattable.SpecificITEMTYPETraits, ItemTypeConstants.Armor);
            mockCollectionsSelector.Setup(s => s.SelectFrom(tableName, template.Name)).Returns(traits);

            var specialAbilityNames = new[] { "ability 1", "ability 2" };
            tableName = string.Format(TableNameConstants.Collections.Formattable.SpecificITEMTYPESpecialAbilities, ItemTypeConstants.Armor);
            mockCollectionsSelector.Setup(s => s.SelectFrom(tableName, template.Name)).Returns(specialAbilityNames);

            var specialAbilities = new[]
            {
                new SpecialAbility(),
                new SpecialAbility()
            };

            mockSpecialAbilitiesGenerator.Setup(s => s.GenerateFor(
                It.Is<IEnumerable<SpecialAbility>>(aa =>
                    aa.First().Name == "ability 1"
                    && aa.Last().Name == "ability 2")
                )
            ).Returns(specialAbilities);

            var gear = specificGearGenerator.GenerateFrom(template);
            Assert.That(gear.Name, Is.EqualTo(selection.Type));
            Assert.That(gear.BaseNames, Is.EquivalentTo(baseNames));
            Assert.That(gear.ItemType, Is.EqualTo(ItemTypeConstants.Armor));
            Assert.That(gear.Attributes, Is.EqualTo(attributes));
            Assert.That(gear.Traits, Is.SupersetOf(traits)
                .And.Contains("my trait")
                .And.Contains("my other trait"));
            Assert.That(gear.Magic.Bonus, Is.EqualTo(selection.Amount));
            Assert.That(gear.Magic.SpecialAbilities, Is.EqualTo(specialAbilities));
            Assert.That(gear, Is.InstanceOf<Armor>());

            var armor = gear as Armor;
            Assert.That(armor.ArmorBonus, Is.EqualTo(mundaneArmor.ArmorBonus));
            Assert.That(armor.ArmorCheckPenalty, Is.EqualTo(mundaneArmor.ArmorCheckPenalty));
            Assert.That(armor.MaxDexterityBonus, Is.EqualTo(mundaneArmor.MaxDexterityBonus));
            Assert.That(armor.Size, Is.EqualTo(mundaneArmor.Size));
        }

        [TestCase(TraitConstants.Sizes.Colossal)]
        [TestCase(TraitConstants.Sizes.Gargantuan)]
        [TestCase(TraitConstants.Sizes.Huge)]
        [TestCase(TraitConstants.Sizes.Large)]
        [TestCase(TraitConstants.Sizes.Medium)]
        [TestCase(TraitConstants.Sizes.Small)]
        [TestCase(TraitConstants.Sizes.Tiny)]
        public void GetSpecificArmor_WithTraitsAndSize(string size)
        {
            template.Name = ArmorConstants.ArmorOfRage;
            template.ItemType = ItemTypeConstants.Armor;
            template.Traits.Add("my trait");
            template.Traits.Add(size);

            mundaneArmor.Size = size;

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, template.Name))
                .Returns(baseNames);

            var attributes = new[] { "attribute 1", "attribute 2" };
            var tableName = string.Format(TableNameConstants.Collections.Formattable.SpecificITEMTYPEAttributes, ItemTypeConstants.Armor);
            mockCollectionsSelector.Setup(s => s.SelectFrom(tableName, template.Name)).Returns(attributes);

            var traits = new[] { "trait 1", "trait 2" };
            tableName = string.Format(TableNameConstants.Collections.Formattable.SpecificITEMTYPETraits, ItemTypeConstants.Armor);
            mockCollectionsSelector.Setup(s => s.SelectFrom(tableName, template.Name)).Returns(traits);

            var specialAbilityNames = new[] { "ability 1", "ability 2" };
            tableName = string.Format(TableNameConstants.Collections.Formattable.SpecificITEMTYPESpecialAbilities, ItemTypeConstants.Armor);
            mockCollectionsSelector.Setup(s => s.SelectFrom(tableName, template.Name)).Returns(specialAbilityNames);

            var specialAbilities = new[]
            {
                new SpecialAbility(),
                new SpecialAbility()
            };

            mockSpecialAbilitiesGenerator.Setup(s => s.GenerateFor(
                It.Is<IEnumerable<SpecialAbility>>(aa =>
                    aa.First().Name == "ability 1"
                    && aa.Last().Name == "ability 2")
                )
            ).Returns(specialAbilities);

            var gear = specificGearGenerator.GenerateFrom(template);
            Assert.That(gear.Name, Is.EqualTo(template.Name));
            Assert.That(gear.BaseNames, Is.EquivalentTo(baseNames));
            Assert.That(gear.ItemType, Is.EqualTo(ItemTypeConstants.Armor));
            Assert.That(gear.Attributes, Is.EqualTo(attributes));
            Assert.That(gear.Traits, Is.SupersetOf(traits)
                .And.Contains("my trait")
                .And.Not.Contains(size));
            Assert.That(gear.Magic.Bonus, Is.EqualTo(selection.Amount));
            Assert.That(gear.Magic.SpecialAbilities, Is.EqualTo(specialAbilities));
            Assert.That(gear, Is.InstanceOf<Armor>());

            var armor = gear as Armor;
            Assert.That(armor.ArmorBonus, Is.EqualTo(mundaneArmor.ArmorBonus));
            Assert.That(armor.ArmorCheckPenalty, Is.EqualTo(mundaneArmor.ArmorCheckPenalty));
            Assert.That(armor.MaxDexterityBonus, Is.EqualTo(mundaneArmor.MaxDexterityBonus));
            Assert.That(armor.Size, Is.EqualTo(mundaneArmor.Size).And.EqualTo(size));
        }

        [Test]
        public void GetSpecificWeapon_WithTraits()
        {
            template.Name = WeaponConstants.HolyAvenger;
            template.Traits.Add("my trait");
            template.Traits.Add("my other trait");

            var attributes = new[] { "attribute 1", "attribute 2" };
            var tableName = string.Format(TableNameConstants.Collections.Formattable.SpecificITEMTYPEAttributes, ItemTypeConstants.Weapon);
            mockCollectionsSelector.Setup(s => s.SelectFrom(tableName, template.Name)).Returns(attributes);

            var traits = new[] { "trait 1", "trait 2" };
            tableName = string.Format(TableNameConstants.Collections.Formattable.SpecificITEMTYPETraits, ItemTypeConstants.Weapon);
            mockCollectionsSelector.Setup(s => s.SelectFrom(tableName, template.Name)).Returns(traits);

            var specialAbilityNames = new[] { "ability 1", "ability 2" };
            tableName = string.Format(TableNameConstants.Collections.Formattable.SpecificITEMTYPESpecialAbilities, ItemTypeConstants.Weapon);
            mockCollectionsSelector.Setup(s => s.SelectFrom(tableName, template.Name)).Returns(specialAbilityNames);

            var specialAbilities = new[]
            {
                new SpecialAbility(),
                new SpecialAbility()
            };

            mockSpecialAbilitiesGenerator.Setup(s => s.GenerateFor(
                It.Is<IEnumerable<SpecialAbility>>(aa =>
                    aa.First().Name == "ability 1"
                    && aa.Last().Name == "ability 2")
                )
            ).Returns(specialAbilities);

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, template.Name))
                .Returns(baseNames);

            var gear = specificGearGenerator.GenerateFrom(template);
            Assert.That(gear, Is.Not.EqualTo(template));
            Assert.That(gear.Name, Is.EqualTo(template.Name));
            Assert.That(gear.BaseNames, Is.EquivalentTo(baseNames));
            Assert.That(gear.ItemType, Is.EqualTo(ItemTypeConstants.Weapon));
            Assert.That(gear.Attributes, Is.EqualTo(attributes));
            Assert.That(gear.Traits, Is.SupersetOf(traits)
                .And.Contains("my trait")
                .And.Contains("my other trait"));
            Assert.That(gear.Magic.Bonus, Is.EqualTo(selection.Amount));
            Assert.That(gear.Magic.SpecialAbilities, Is.EqualTo(specialAbilities));
            Assert.That(gear, Is.InstanceOf<Weapon>());

            var weapon = gear as Weapon;
            Assert.That(weapon.CriticalMultiplier, Is.EqualTo(mundaneWeapon.CriticalMultiplier));
            Assert.That(weapon.Damage, Is.EqualTo(mundaneWeapon.Damage));
            Assert.That(weapon.DamageType, Is.EqualTo(mundaneWeapon.DamageType));
            Assert.That(weapon.Size, Is.EqualTo(mundaneWeapon.Size));
            Assert.That(weapon.ThreatRange, Is.EqualTo(mundaneWeapon.ThreatRange));
            Assert.That(weapon.Quantity, Is.EqualTo(mundaneWeapon.Quantity));
            Assert.That(mundaneWeapon.Quantity, Is.AtLeast(2));
        }

        [TestCase(TraitConstants.Sizes.Colossal)]
        [TestCase(TraitConstants.Sizes.Gargantuan)]
        [TestCase(TraitConstants.Sizes.Huge)]
        [TestCase(TraitConstants.Sizes.Large)]
        [TestCase(TraitConstants.Sizes.Medium)]
        [TestCase(TraitConstants.Sizes.Small)]
        [TestCase(TraitConstants.Sizes.Tiny)]
        public void GetSpecificWeapon_WithTraitsAndSize(string size)
        {
            template.Name = WeaponConstants.HolyAvenger;
            template.Traits.Add("my trait");
            template.Traits.Add(size);

            mundaneWeapon.Size = size;

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, template.Name))
                .Returns(baseNames);

            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, ItemTypeConstants.Armor)).Returns(() => new[] { "wrong name" });
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, ItemTypeConstants.Weapon)).Returns(() => new[] { template.Name });

            var attributes = new[] { "attribute 1", "attribute 2" };
            var tableName = string.Format(TableNameConstants.Collections.Formattable.SpecificITEMTYPEAttributes, ItemTypeConstants.Weapon);
            mockCollectionsSelector.Setup(s => s.SelectFrom(tableName, template.Name)).Returns(attributes);

            var traits = new[] { "trait 1", "trait 2" };
            tableName = string.Format(TableNameConstants.Collections.Formattable.SpecificITEMTYPETraits, ItemTypeConstants.Weapon);
            mockCollectionsSelector.Setup(s => s.SelectFrom(tableName, template.Name)).Returns(traits);

            var specialAbilityNames = new[] { "ability 1", "ability 2" };
            tableName = string.Format(TableNameConstants.Collections.Formattable.SpecificITEMTYPESpecialAbilities, ItemTypeConstants.Weapon);
            mockCollectionsSelector.Setup(s => s.SelectFrom(tableName, template.Name)).Returns(specialAbilityNames);

            var specialAbilities = new[]
            {
                new SpecialAbility(),
                new SpecialAbility()
            };

            mockSpecialAbilitiesGenerator.Setup(s => s.GenerateFor(
                It.Is<IEnumerable<SpecialAbility>>(aa =>
                    aa.First().Name == "ability 1"
                    && aa.Last().Name == "ability 2")
                )
            ).Returns(specialAbilities);

            var gear = specificGearGenerator.GenerateFrom(template);
            Assert.That(gear, Is.Not.EqualTo(template));
            Assert.That(gear.Name, Is.EqualTo(template.Name));
            Assert.That(gear.BaseNames, Is.EquivalentTo(baseNames));
            Assert.That(gear.ItemType, Is.EqualTo(ItemTypeConstants.Weapon));
            Assert.That(gear.Attributes, Is.EqualTo(attributes));
            Assert.That(gear.Traits, Is.SupersetOf(traits)
                .And.Contains("my trait")
                .And.Not.Contains(size));
            Assert.That(gear.Magic.Bonus, Is.EqualTo(selection.Amount));
            Assert.That(gear.Magic.SpecialAbilities, Is.EqualTo(specialAbilities));
            Assert.That(gear, Is.InstanceOf<Weapon>());

            var weapon = gear as Weapon;
            Assert.That(weapon.CriticalMultiplier, Is.EqualTo(mundaneWeapon.CriticalMultiplier));
            Assert.That(weapon.Damage, Is.EqualTo(mundaneWeapon.Damage));
            Assert.That(weapon.DamageType, Is.EqualTo(mundaneWeapon.DamageType));
            Assert.That(weapon.Size, Is.EqualTo(mundaneWeapon.Size).And.EqualTo(size));
            Assert.That(weapon.ThreatRange, Is.EqualTo(mundaneWeapon.ThreatRange));
            Assert.That(weapon.Quantity, Is.EqualTo(mundaneWeapon.Quantity).And.AtLeast(2));
        }

        [Test]
        public void GetSpecificWeaponFromWeapon()
        {
            var weaponTemplate = new Weapon();
            template.CloneInto(weaponTemplate);
            weaponTemplate.Name = WeaponConstants.HolyAvenger;

            var attributes = new[] { "attribute 1", "attribute 2" };
            var tableName = string.Format(TableNameConstants.Collections.Formattable.SpecificITEMTYPEAttributes, ItemTypeConstants.Weapon);
            mockCollectionsSelector.Setup(s => s.SelectFrom(tableName, weaponTemplate.Name)).Returns(attributes);

            var traits = new[] { "trait 1", "trait 2" };
            tableName = string.Format(TableNameConstants.Collections.Formattable.SpecificITEMTYPETraits, ItemTypeConstants.Weapon);
            mockCollectionsSelector.Setup(s => s.SelectFrom(tableName, weaponTemplate.Name)).Returns(traits);

            var specialAbilityNames = new[] { "ability 1", "ability 2" };
            tableName = string.Format(TableNameConstants.Collections.Formattable.SpecificITEMTYPESpecialAbilities, ItemTypeConstants.Weapon);
            mockCollectionsSelector.Setup(s => s.SelectFrom(tableName, weaponTemplate.Name)).Returns(specialAbilityNames);

            var specialAbilities = new[]
            {
                new SpecialAbility(),
                new SpecialAbility()
            };

            mockSpecialAbilitiesGenerator.Setup(s => s.GenerateFor(
                It.Is<IEnumerable<SpecialAbility>>(aa =>
                    aa.First().Name == "ability 1"
                    && aa.Last().Name == "ability 2")
                )
            ).Returns(specialAbilities);

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, weaponTemplate.Name))
                .Returns(baseNames);

            var gear = specificGearGenerator.GenerateFrom(weaponTemplate);
            Assert.That(gear, Is.Not.EqualTo(weaponTemplate));
            Assert.That(gear, Is.Not.EqualTo(template));
            Assert.That(gear.Name, Is.EqualTo(weaponTemplate.Name));
            Assert.That(gear.BaseNames, Is.EquivalentTo(baseNames));
            Assert.That(gear.ItemType, Is.EqualTo(ItemTypeConstants.Weapon));
            Assert.That(gear.Attributes, Is.EqualTo(attributes));
            Assert.That(gear.Traits, Is.SupersetOf(traits));
            Assert.That(gear.Magic.Bonus, Is.EqualTo(selection.Amount));
            Assert.That(gear.Magic.SpecialAbilities, Is.EqualTo(specialAbilities));
            Assert.That(gear, Is.InstanceOf<Weapon>());

            var weapon = gear as Weapon;
            Assert.That(weapon.CriticalMultiplier, Is.EqualTo(mundaneWeapon.CriticalMultiplier));
            Assert.That(weapon.Damage, Is.EqualTo(mundaneWeapon.Damage));
            Assert.That(weapon.DamageType, Is.EqualTo(mundaneWeapon.DamageType));
            Assert.That(weapon.Size, Is.EqualTo(mundaneWeapon.Size));
            Assert.That(weapon.ThreatRange, Is.EqualTo(mundaneWeapon.ThreatRange));
            Assert.That(weapon.Quantity, Is.EqualTo(mundaneWeapon.Quantity));
            Assert.That(mundaneWeapon.Quantity, Is.AtLeast(2));
        }

        [Test]
        public void JavelinOfLightningIsMagical()
        {
            template.Name = WeaponConstants.JavelinOfLightning;

            var gear = specificGearGenerator.GenerateFrom(template);
            Assert.That(gear.IsMagical, Is.True);
        }

        [Test]
        public void GetSpecialAbilities()
        {
            var specialAbilityNames = new[] { "ability 1", "ability 2" };
            var tableName = string.Format(TableNameConstants.Collections.Formattable.SpecificITEMTYPESpecialAbilities, ItemTypeConstants.Armor);
            mockCollectionsSelector.Setup(s => s.SelectFrom(tableName, template.Name)).Returns(specialAbilityNames);

            var specialAbilities = new[]
            {
                new SpecialAbility(),
                new SpecialAbility()
            };

            mockSpecialAbilitiesGenerator.Setup(s => s.GenerateFor(
                It.Is<IEnumerable<SpecialAbility>>(aa =>
                    aa.First().Name == "ability 1"
                    && aa.Last().Name == "ability 2")
                )
            ).Returns(specialAbilities);

            var gear = specificGearGenerator.GenerateFrom(template);
            Assert.That(gear.Magic.SpecialAbilities, Is.EqualTo(specialAbilities));
        }

        [Test]
        public void IfCharged_GetChargesFromGenerator()
        {
            var attributes = new[] { AttributeConstants.Charged };
            var tableName = string.Format(TableNameConstants.Collections.Formattable.SpecificITEMTYPEAttributes, ItemTypeConstants.Armor);
            mockCollectionsSelector.Setup(s => s.SelectFrom(tableName, template.Name)).Returns(attributes);
            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Armor, template.Name)).Returns(9266);

            var gear = specificGearGenerator.GenerateFrom(template);
            Assert.That(gear.Magic.Charges, Is.EqualTo(9266));
        }

        [Test]
        public void IfNotCharged_DoNotGetChargesFromGenerator()
        {
            var attributes = new[] { "not charged" };
            var tableName = string.Format(TableNameConstants.Collections.Formattable.SpecificITEMTYPEAttributes, ItemTypeConstants.Armor);
            mockCollectionsSelector.Setup(s => s.SelectFrom(tableName, template.Name)).Returns(attributes);
            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Armor, template.Name)).Returns(9266);

            var gear = specificGearGenerator.GenerateFrom(template);
            Assert.That(gear.Magic.Charges, Is.EqualTo(0));
        }

        [TestCase(WeaponConstants.Dagger_Silver, WeaponConstants.Dagger)]
        [TestCase(WeaponConstants.Dagger_Adamantine, WeaponConstants.Dagger)]
        [TestCase(WeaponConstants.Battleaxe_Adamantine, WeaponConstants.Battleaxe)]
        [TestCase(WeaponConstants.LuckBlade0, WeaponConstants.LuckBlade)]
        [TestCase(WeaponConstants.LuckBlade1, WeaponConstants.LuckBlade)]
        [TestCase(WeaponConstants.LuckBlade2, WeaponConstants.LuckBlade)]
        [TestCase(WeaponConstants.LuckBlade3, WeaponConstants.LuckBlade)]
        public void SpecificGearIsRenamed(string originalName, string newName)
        {
            template.Name = originalName;
            template.ItemType = ItemTypeConstants.Weapon;

            mockReplacementSelector.Setup(s => s.SelectSingle(originalName)).Returns(newName);

            var gear = specificGearGenerator.GenerateFrom(template);
            Assert.That(gear.Name, Is.EqualTo(newName));
        }

        [Test]
        public void CastersShieldHasNoSpellIfSelectorSaysSo()
        {
            template.Name = ArmorConstants.CastersShield;

            mockPercentileSelector.Setup(s => s.SelectFrom(TableNameConstants.Percentiles.Set.CastersShieldSpellTypes)).Returns("spell type");
            mockSpellGenerator.Setup(g => g.GenerateLevel(PowerConstants.Medium)).Returns(42);
            mockSpellGenerator.Setup(g => g.Generate("spell type", 42)).Returns("spell");
            mockPercentileSelector.Setup(s => s.SelectFrom<bool>(TableNameConstants.Percentiles.Set.CastersShieldContainsSpell)).Returns(false);
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, selection.Type)).Returns(baseNames);

            var gear = specificGearGenerator.GenerateFrom(template);
            Assert.That(gear.Name, Is.EqualTo(ArmorConstants.CastersShield));
            Assert.That(gear.Contents, Is.Empty);
        }

        [Test]
        public void CastersShieldHasSpellIfSelectorSaysSo()
        {
            template.Name = ArmorConstants.CastersShield;

            mockPercentileSelector.Setup(s => s.SelectFrom(TableNameConstants.Percentiles.Set.CastersShieldSpellTypes)).Returns("spell type");
            mockSpellGenerator.Setup(g => g.GenerateLevel(PowerConstants.Medium)).Returns(42);
            mockSpellGenerator.Setup(g => g.Generate("spell type", 42)).Returns("spell");
            mockPercentileSelector.Setup(s => s.SelectFrom<bool>(TableNameConstants.Percentiles.Set.CastersShieldContainsSpell)).Returns(true);
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, selection.Type)).Returns(baseNames);

            var gear = specificGearGenerator.GenerateFrom(template);
            Assert.That(gear.Name, Is.EqualTo(ArmorConstants.CastersShield));
            Assert.That(gear.Contents, Contains.Item("spell (spell type, 42)"));
        }

        [Test]
        public void SlayingArrowHasDesignatedFoe()
        {
            template.Name = WeaponConstants.SlayingArrow;
            template.ItemType = ItemTypeConstants.Weapon;

            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(TableNameConstants.Collections.Set.ReplacementStrings, ReplacementStringConstants.DesignatedFoe))
                .Returns("foe");

            var gear = specificGearGenerator.GenerateFrom(template);
            Assert.That(gear.Name, Is.EqualTo(WeaponConstants.SlayingArrow));
            Assert.That(gear.Traits, Contains.Item("Designated Foe: foe"));
        }

        [Test]
        public void GreaterSlayingArrowHasDesignatedFoe()
        {
            template.Name = WeaponConstants.GreaterSlayingArrow;
            template.ItemType = ItemTypeConstants.Weapon;

            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(TableNameConstants.Collections.Set.ReplacementStrings, ReplacementStringConstants.DesignatedFoe))
                .Returns("foe");

            var gear = specificGearGenerator.GenerateFrom(template);
            Assert.That(gear.Name, Is.EqualTo(WeaponConstants.GreaterSlayingArrow));
            Assert.That(gear.Traits, Contains.Item("Designated Foe: foe"));
        }

        [TestCase(ArmorConstants.AbsorbingShield)]
        [TestCase(ArmorConstants.CastersShield)]
        [TestCase(ArmorConstants.LionsShield)]
        [TestCase(ArmorConstants.SpinedShield)]
        [TestCase(ArmorConstants.WingedShield)]
        [TestCase(ArmorConstants.ArmorOfArrowAttraction)]
        [TestCase(ArmorConstants.ArmorOfRage)]
        [TestCase(ArmorConstants.BandedMailOfLuck)]
        [TestCase(ArmorConstants.BreastplateOfCommand)]
        [TestCase(ArmorConstants.CelestialArmor)]
        [TestCase(ArmorConstants.DemonArmor)]
        [TestCase(ArmorConstants.DwarvenPlate)]
        [TestCase(ArmorConstants.ElvenChain)]
        [TestCase(ArmorConstants.FullPlateOfSpeed)]
        [TestCase(ArmorConstants.PlateArmorOfTheDeep)]
        [TestCase(ArmorConstants.RhinoHide)]
        [TestCase(WeaponConstants.AssassinsDagger)]
        [TestCase(WeaponConstants.Battleaxe_Adamantine)]
        [TestCase(WeaponConstants.BerserkingSword)]
        [TestCase(WeaponConstants.CursedBackbiterSpear)]
        [TestCase(WeaponConstants.CursedMinus2Sword)]
        [TestCase(WeaponConstants.DaggerOfVenom)]
        [TestCase(WeaponConstants.Dagger_Adamantine)]
        [TestCase(WeaponConstants.Dagger_Silver)]
        [TestCase(WeaponConstants.DwarvenThrower)]
        [TestCase(WeaponConstants.FlameTongue)]
        [TestCase(WeaponConstants.FrostBrand)]
        [TestCase(WeaponConstants.GreaterSlayingArrow)]
        [TestCase(WeaponConstants.HolyAvenger)]
        [TestCase(WeaponConstants.JavelinOfLightning)]
        [TestCase(WeaponConstants.LifeDrinker)]
        [TestCase(WeaponConstants.LuckBlade)]
        [TestCase(WeaponConstants.LuckBlade0)]
        [TestCase(WeaponConstants.LuckBlade1)]
        [TestCase(WeaponConstants.LuckBlade2)]
        [TestCase(WeaponConstants.LuckBlade3)]
        [TestCase(WeaponConstants.MaceOfBlood)]
        [TestCase(WeaponConstants.MaceOfSmiting)]
        [TestCase(WeaponConstants.MaceOfTerror)]
        [TestCase(WeaponConstants.NetOfSnaring)]
        [TestCase(WeaponConstants.NineLivesStealer)]
        [TestCase(WeaponConstants.Oathbow)]
        [TestCase(WeaponConstants.RapierOfPuncturing)]
        [TestCase(WeaponConstants.ScreamingBolt)]
        [TestCase(WeaponConstants.Shatterspike)]
        [TestCase(WeaponConstants.ShiftersSorrow)]
        [TestCase(WeaponConstants.SlayingArrow)]
        [TestCase(WeaponConstants.SleepArrow)]
        [TestCase(WeaponConstants.SunBlade)]
        [TestCase(WeaponConstants.SwordOfLifeStealing)]
        [TestCase(WeaponConstants.SwordOfSubtlety)]
        [TestCase(WeaponConstants.SwordOfThePlanes)]
        [TestCase(WeaponConstants.SylvanScimitar)]
        [TestCase(WeaponConstants.TridentOfFishCommand)]
        [TestCase(WeaponConstants.TridentOfWarning)]
        public void TemplateIsSpecific(string name)
        {
            var template = itemVerifier.CreateRandomTemplate(name);

            var specificItems = new[] { "other item", name };
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, AttributeConstants.Specific)).Returns(specificItems);

            var isSpecific = specificGearGenerator.IsSpecific(template);
            Assert.That(isSpecific, Is.True);
        }

        [Test]
        public void TemplateIsNotSpecific()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);

            var specificItems = new[] { "other item", "item" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, AttributeConstants.Specific)).Returns(specificItems);

            var isSpecific = specificGearGenerator.IsSpecific(template);
            Assert.That(isSpecific, Is.False);
        }

        [TestCase(WeaponConstants.Battleaxe_Adamantine, WeaponConstants.Battleaxe)]
        [TestCase(WeaponConstants.Dagger_Adamantine, WeaponConstants.Dagger)]
        [TestCase(WeaponConstants.Dagger_Silver, WeaponConstants.Dagger)]
        [TestCase(WeaponConstants.LuckBlade, WeaponConstants.LuckBlade)]
        [TestCase(WeaponConstants.LuckBlade0, WeaponConstants.LuckBlade)]
        [TestCase(WeaponConstants.LuckBlade1, WeaponConstants.LuckBlade)]
        [TestCase(WeaponConstants.LuckBlade2, WeaponConstants.LuckBlade)]
        [TestCase(WeaponConstants.LuckBlade3, WeaponConstants.LuckBlade)]
        public void BUG_TemplateIsSpecific_ChangedName(string oldName, params string[] newNames)
        {
            var template = itemVerifier.CreateRandomTemplate(oldName);

            var specificItems = new[]
            {
                "other item",
                WeaponConstants.Dagger_Silver,
                WeaponConstants.LuckBlade,
                WeaponConstants.Dagger_Adamantine,
                WeaponConstants.Battleaxe_Adamantine,
                "wrong item"
            };
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, AttributeConstants.Specific)).Returns(specificItems);

            mockReplacementSelector
                .Setup(s => s.SelectAll(It.IsAny<string>(), It.IsAny<bool>()))
                .Returns((string s, bool allow) => new[] { s });

            mockReplacementSelector
                .Setup(s => s.SelectAll(oldName, true))
                .Returns(newNames);

            var isSpecific = specificGearGenerator.IsSpecific(template);
            Assert.That(isSpecific, Is.True);
        }

        [TestCase(WeaponConstants.Dagger)]
        public void BUG_TemplateIsNotSpecific_ChangedName(string oldName)
        {
            var template = itemVerifier.CreateRandomTemplate(oldName);

            var specificItems = new[] { "other item", "item" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, AttributeConstants.Specific)).Returns(specificItems);

            var isSpecific = specificGearGenerator.IsSpecific(template);
            Assert.That(isSpecific, Is.False);
        }

        [Test]
        public void GenerateCustomSpecificShield()
        {
            var template = itemVerifier.CreateRandomTemplate(ArmorConstants.AbsorbingShield);

            var attributes = new[] { "attribute 1", "attribute 2" };
            var tableName = string.Format(TableNameConstants.Collections.Formattable.SpecificITEMTYPEAttributes, AttributeConstants.Shield);
            mockCollectionsSelector.Setup(s => s.SelectFrom(tableName, ArmorConstants.AbsorbingShield)).Returns(attributes);

            var traits = new[] { "trait 1", "trait 2" };
            tableName = string.Format(TableNameConstants.Collections.Formattable.SpecificITEMTYPETraits, AttributeConstants.Shield);
            mockCollectionsSelector.Setup(s => s.SelectFrom(tableName, ArmorConstants.AbsorbingShield)).Returns(traits);

            var specialAbilityNames = new[] { "ability 1", "ability 2" };
            tableName = string.Format(TableNameConstants.Collections.Formattable.SpecificITEMTYPESpecialAbilities, AttributeConstants.Shield);
            mockCollectionsSelector.Setup(s => s.SelectFrom(tableName, ArmorConstants.AbsorbingShield)).Returns(specialAbilityNames);

            var specialAbilities = new[]
            {
                new SpecialAbility { Name = template.Magic.SpecialAbilities.First().Name },
                new SpecialAbility { Name = template.Magic.SpecialAbilities.Last().Name }
            };

            mockSpecialAbilitiesGenerator.Setup(s =>
                s.GenerateFor(
                    It.Is<IEnumerable<SpecialAbility>>(aa =>
                        aa.First().Name == "ability 1"
                        && aa.Last().Name == "ability 2"
                    )
                )
            ).Returns(specialAbilities);

            mockMundaneArmorGenerator.Setup(g => g.Generate(It.Is<Item>(i => i.Name == ArmorConstants.AbsorbingShield), false)).Returns(mundaneArmor);
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, ArmorConstants.AbsorbingShield)).Returns(baseNames);

            var gear = specificGearGenerator.GenerateFrom(template);
            itemVerifier.AssertMagicalItemFromTemplate(gear, template);
            Assert.That(gear.ItemType, Is.EqualTo(ItemTypeConstants.Armor));
            Assert.That(gear.Attributes, Is.EqualTo(attributes));
            Assert.That(gear.Traits, Is.SupersetOf(traits));
            Assert.That(gear.Traits, Is.SupersetOf(template.Traits));
            Assert.That(gear.Magic.SpecialAbilities, Is.EqualTo(specialAbilities));
            Assert.That(gear.BaseNames, Is.EquivalentTo(baseNames));
        }

        [Test]
        public void GenerateCustomSpecificArmor()
        {
            var template = itemVerifier.CreateRandomTemplate(ArmorConstants.ArmorOfRage);

            var attributes = new[] { "attribute 1", "attribute 2" };
            var tableName = string.Format(TableNameConstants.Collections.Formattable.SpecificITEMTYPEAttributes, ItemTypeConstants.Armor);
            mockCollectionsSelector.Setup(s => s.SelectFrom(tableName, ArmorConstants.ArmorOfRage)).Returns(attributes);

            var traits = new[] { "trait 1", "trait 2" };
            tableName = string.Format(TableNameConstants.Collections.Formattable.SpecificITEMTYPETraits, ItemTypeConstants.Armor);
            mockCollectionsSelector.Setup(s => s.SelectFrom(tableName, ArmorConstants.ArmorOfRage)).Returns(traits);

            var specialAbilityNames = new[] { "ability 1", "ability 2" };
            tableName = string.Format(TableNameConstants.Collections.Formattable.SpecificITEMTYPESpecialAbilities, ItemTypeConstants.Armor);
            mockCollectionsSelector.Setup(s => s.SelectFrom(tableName, ArmorConstants.ArmorOfRage)).Returns(specialAbilityNames);

            var specialAbilities = new[]
            {
                new SpecialAbility { Name = template.Magic.SpecialAbilities.First().Name },
                new SpecialAbility { Name = template.Magic.SpecialAbilities.Last().Name }
            };

            mockSpecialAbilitiesGenerator.Setup(s =>
                s.GenerateFor(
                    It.Is<IEnumerable<SpecialAbility>>(aa =>
                        aa.First().Name == "ability 1"
                        && aa.Last().Name == "ability 2"
                    )
                )
            ).Returns(specialAbilities);

            mockMundaneArmorGenerator.Setup(g => g.Generate(It.Is<Item>(i => i.Name == ArmorConstants.ArmorOfRage), false)).Returns(mundaneArmor);
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, ArmorConstants.ArmorOfRage)).Returns(baseNames);

            var gear = specificGearGenerator.GenerateFrom(template);
            itemVerifier.AssertMagicalItemFromTemplate(gear, template);
            Assert.That(gear.ItemType, Is.EqualTo(ItemTypeConstants.Armor));
            Assert.That(gear.Attributes, Is.EqualTo(attributes));
            Assert.That(gear.Traits, Is.SupersetOf(traits));
            Assert.That(gear.Traits, Is.SupersetOf(template.Traits));
            Assert.That(gear.Magic.SpecialAbilities, Is.EqualTo(specialAbilities));
            Assert.That(gear.BaseNames, Is.EquivalentTo(baseNames));
            Assert.That(gear, Is.InstanceOf<Armor>());

            var armor = gear as Armor;
            Assert.That(armor.ArmorBonus, Is.EqualTo(mundaneArmor.ArmorBonus));
            Assert.That(armor.ArmorCheckPenalty, Is.EqualTo(mundaneArmor.ArmorCheckPenalty));
            Assert.That(armor.MaxDexterityBonus, Is.EqualTo(mundaneArmor.MaxDexterityBonus));
            Assert.That(armor.Size, Is.EqualTo(mundaneArmor.Size));
        }

        [Test]
        public void GenerateCustomSpecificWeapon()
        {
            var template = itemVerifier.CreateRandomTemplate(WeaponConstants.HolyAvenger);

            var attributes = new[] { "attribute 1", "attribute 2" };
            var tableName = string.Format(TableNameConstants.Collections.Formattable.SpecificITEMTYPEAttributes, ItemTypeConstants.Weapon);
            mockCollectionsSelector.Setup(s => s.SelectFrom(tableName, WeaponConstants.HolyAvenger)).Returns(attributes);

            var traits = new[] { "trait 1", "trait 2" };
            tableName = string.Format(TableNameConstants.Collections.Formattable.SpecificITEMTYPETraits, ItemTypeConstants.Weapon);
            mockCollectionsSelector.Setup(s => s.SelectFrom(tableName, WeaponConstants.HolyAvenger)).Returns(traits);

            var specialAbilityNames = new[] { "ability 1", "ability 2" };
            tableName = string.Format(TableNameConstants.Collections.Formattable.SpecificITEMTYPESpecialAbilities, ItemTypeConstants.Weapon);
            mockCollectionsSelector.Setup(s => s.SelectFrom(tableName, WeaponConstants.HolyAvenger)).Returns(specialAbilityNames);

            var specialAbilities = new[]
            {
                new SpecialAbility { Name = template.Magic.SpecialAbilities.First().Name },
                new SpecialAbility { Name = template.Magic.SpecialAbilities.Last().Name }
            };

            mockSpecialAbilitiesGenerator.Setup(s =>
                s.GenerateFor(
                    It.Is<IEnumerable<SpecialAbility>>(aa =>
                        aa.First().Name == "ability 1"
                        && aa.Last().Name == "ability 2"
                    )
                )
            ).Returns(specialAbilities);

            mockMundaneWeaponGenerator.Setup(g => g.Generate(It.Is<Item>(i => i.Name == WeaponConstants.HolyAvenger), false)).Returns(mundaneWeapon);
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, WeaponConstants.HolyAvenger)).Returns(baseNames);

            var gear = specificGearGenerator.GenerateFrom(template);
            itemVerifier.AssertMagicalItemFromTemplate(gear, template);
            Assert.That(gear.ItemType, Is.EqualTo(ItemTypeConstants.Weapon));
            Assert.That(gear.Attributes, Is.EqualTo(attributes));
            Assert.That(gear.Traits, Is.SupersetOf(traits));
            Assert.That(gear.Traits, Is.SupersetOf(template.Traits));
            Assert.That(gear.Magic.SpecialAbilities, Is.EqualTo(specialAbilities));
            Assert.That(gear.BaseNames, Is.EquivalentTo(baseNames));
            Assert.That(gear, Is.InstanceOf<Weapon>());

            var weapon = gear as Weapon;
            Assert.That(weapon.CriticalMultiplier, Is.EqualTo(mundaneWeapon.CriticalMultiplier));
            Assert.That(weapon.Damage, Is.EqualTo(mundaneWeapon.Damage));
            Assert.That(weapon.DamageType, Is.EqualTo(mundaneWeapon.DamageType));
            Assert.That(weapon.Size, Is.EqualTo(mundaneWeapon.Size));
            Assert.That(weapon.ThreatRange, Is.EqualTo(mundaneWeapon.ThreatRange));
            Assert.That(weapon.Quantity, Is.EqualTo(mundaneWeapon.Quantity).And.AtLeast(2));
        }

        [Test]
        public void GenerateCustomSpecificAmmunition()
        {
            var template = itemVerifier.CreateRandomTemplate(WeaponConstants.ScreamingBolt);

            var attributes = new[] { "attribute 1", "attribute 2", AttributeConstants.Ammunition };
            var tableName = string.Format(TableNameConstants.Collections.Formattable.SpecificITEMTYPEAttributes, ItemTypeConstants.Weapon);
            mockCollectionsSelector.Setup(s => s.SelectFrom(tableName, WeaponConstants.ScreamingBolt)).Returns(attributes);

            var traits = new[] { "trait 1", "trait 2" };
            tableName = string.Format(TableNameConstants.Collections.Formattable.SpecificITEMTYPETraits, ItemTypeConstants.Weapon);
            mockCollectionsSelector.Setup(s => s.SelectFrom(tableName, WeaponConstants.ScreamingBolt)).Returns(traits);

            var specialAbilityNames = new[] { "ability 1", "ability 2" };
            tableName = string.Format(TableNameConstants.Collections.Formattable.SpecificITEMTYPESpecialAbilities, ItemTypeConstants.Weapon);
            mockCollectionsSelector.Setup(s => s.SelectFrom(tableName, WeaponConstants.ScreamingBolt)).Returns(specialAbilityNames);

            var specialAbilities = new[]
            {
                new SpecialAbility { Name = template.Magic.SpecialAbilities.First().Name },
                new SpecialAbility { Name = template.Magic.SpecialAbilities.Last().Name }
            };

            mockSpecialAbilitiesGenerator.Setup(s =>
                s.GenerateFor(
                    It.Is<IEnumerable<SpecialAbility>>(aa =>
                        aa.First().Name == "ability 1"
                        && aa.Last().Name == "ability 2"
                    )
                )
            ).Returns(specialAbilities);

            mockMundaneWeaponGenerator.Setup(g => g.Generate(It.Is<Item>(i => i.Name == WeaponConstants.ScreamingBolt), false)).Returns(mundaneWeapon);
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, WeaponConstants.ScreamingBolt)).Returns(baseNames);

            var gear = specificGearGenerator.GenerateFrom(template);
            Assert.That(gear.ItemType, Is.EqualTo(ItemTypeConstants.Weapon));
            Assert.That(gear.Attributes, Is.EqualTo(attributes));
            Assert.That(gear.Traits, Is.SupersetOf(traits));
            Assert.That(gear.Traits, Is.SupersetOf(template.Traits));
            Assert.That(gear.Magic.SpecialAbilities, Is.EqualTo(specialAbilities));
            Assert.That(gear.BaseNames, Is.EquivalentTo(baseNames));
            Assert.That(gear, Is.InstanceOf<Weapon>());
            Assert.That(gear.Magic.Intelligence.Ego, Is.EqualTo(0));

            var weapon = gear as Weapon;
            Assert.That(weapon.CriticalMultiplier, Is.EqualTo(mundaneWeapon.CriticalMultiplier));
            Assert.That(weapon.Damage, Is.EqualTo(mundaneWeapon.Damage));
            Assert.That(weapon.DamageType, Is.EqualTo(mundaneWeapon.DamageType));
            Assert.That(weapon.Size, Is.EqualTo(mundaneWeapon.Size));
            Assert.That(weapon.ThreatRange, Is.EqualTo(mundaneWeapon.ThreatRange));
            Assert.That(weapon.Quantity, Is.EqualTo(mundaneWeapon.Quantity));
            Assert.That(mundaneWeapon.Quantity, Is.AtLeast(2));
        }

        [Test]
        public void GenerateCustomSpecificOneTimeUseWeapon()
        {
            var template = itemVerifier.CreateRandomTemplate(WeaponConstants.JavelinOfLightning);

            var attributes = new[] { "attribute 1", "attribute 2", AttributeConstants.OneTimeUse };
            var tableName = string.Format(TableNameConstants.Collections.Formattable.SpecificITEMTYPEAttributes, ItemTypeConstants.Weapon);
            mockCollectionsSelector.Setup(s => s.SelectFrom(tableName, WeaponConstants.JavelinOfLightning)).Returns(attributes);

            var traits = new[] { "trait 1", "trait 2" };
            tableName = string.Format(TableNameConstants.Collections.Formattable.SpecificITEMTYPETraits, ItemTypeConstants.Weapon);
            mockCollectionsSelector.Setup(s => s.SelectFrom(tableName, WeaponConstants.JavelinOfLightning)).Returns(traits);

            var specialAbilityNames = new[] { "ability 1", "ability 2" };
            tableName = string.Format(TableNameConstants.Collections.Formattable.SpecificITEMTYPESpecialAbilities, ItemTypeConstants.Weapon);
            mockCollectionsSelector.Setup(s => s.SelectFrom(tableName, WeaponConstants.JavelinOfLightning)).Returns(specialAbilityNames);

            var specialAbilities = new[]
            {
                new SpecialAbility { Name = template.Magic.SpecialAbilities.First().Name },
                new SpecialAbility { Name = template.Magic.SpecialAbilities.Last().Name }
            };

            mockSpecialAbilitiesGenerator.Setup(s =>
                s.GenerateFor(
                    It.Is<IEnumerable<SpecialAbility>>(aa =>
                        aa.First().Name == "ability 1"
                        && aa.Last().Name == "ability 2"
                    )
                )
            ).Returns(specialAbilities);

            mockMundaneWeaponGenerator.Setup(g => g.Generate(It.Is<Item>(i => i.Name == WeaponConstants.JavelinOfLightning), false)).Returns(mundaneWeapon);
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, WeaponConstants.JavelinOfLightning)).Returns(baseNames);

            var gear = specificGearGenerator.GenerateFrom(template);
            Assert.That(gear.ItemType, Is.EqualTo(ItemTypeConstants.Weapon));
            Assert.That(gear.Attributes, Is.EqualTo(attributes));
            Assert.That(gear.Traits, Is.SupersetOf(traits));
            Assert.That(gear.Traits, Is.SupersetOf(template.Traits));
            Assert.That(gear.Magic.SpecialAbilities, Is.EqualTo(specialAbilities));
            Assert.That(gear.BaseNames, Is.EquivalentTo(baseNames));
            Assert.That(gear, Is.InstanceOf<Weapon>());
            Assert.That(gear.Magic.Intelligence.Ego, Is.EqualTo(0));

            var weapon = gear as Weapon;
            Assert.That(weapon.CriticalMultiplier, Is.EqualTo(mundaneWeapon.CriticalMultiplier));
            Assert.That(weapon.Damage, Is.EqualTo(mundaneWeapon.Damage));
            Assert.That(weapon.DamageType, Is.EqualTo(mundaneWeapon.DamageType));
            Assert.That(weapon.Size, Is.EqualTo(mundaneWeapon.Size));
            Assert.That(weapon.ThreatRange, Is.EqualTo(mundaneWeapon.ThreatRange));
            Assert.That(weapon.Quantity, Is.EqualTo(mundaneWeapon.Quantity));
            Assert.That(mundaneWeapon.Quantity, Is.AtLeast(2));
        }

        [TestCase(AttributeConstants.Shield, ArmorConstants.AbsorbingShield)]
        [TestCase(AttributeConstants.Shield, ArmorConstants.CastersShield)]
        [TestCase(AttributeConstants.Shield, ArmorConstants.LionsShield)]
        [TestCase(AttributeConstants.Shield, ArmorConstants.SpinedShield)]
        [TestCase(AttributeConstants.Shield, ArmorConstants.WingedShield)]
        [TestCase(ItemTypeConstants.Armor, ArmorConstants.ArmorOfArrowAttraction)]
        [TestCase(ItemTypeConstants.Armor, ArmorConstants.ArmorOfRage)]
        [TestCase(ItemTypeConstants.Armor, ArmorConstants.BandedMailOfLuck)]
        [TestCase(ItemTypeConstants.Armor, ArmorConstants.BreastplateOfCommand)]
        [TestCase(ItemTypeConstants.Armor, ArmorConstants.CelestialArmor)]
        [TestCase(ItemTypeConstants.Armor, ArmorConstants.DemonArmor)]
        [TestCase(ItemTypeConstants.Armor, ArmorConstants.DwarvenPlate)]
        [TestCase(ItemTypeConstants.Armor, ArmorConstants.ElvenChain)]
        [TestCase(ItemTypeConstants.Armor, ArmorConstants.FullPlateOfSpeed)]
        [TestCase(ItemTypeConstants.Armor, ArmorConstants.PlateArmorOfTheDeep)]
        [TestCase(ItemTypeConstants.Armor, ArmorConstants.RhinoHide)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.AssassinsDagger)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.Battleaxe_Adamantine)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.BerserkingSword)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.CursedBackbiterSpear)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.CursedMinus2Sword)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.DaggerOfVenom)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.Dagger_Adamantine)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.Dagger_Silver)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.DwarvenThrower)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.FlameTongue)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.FrostBrand)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.GreaterSlayingArrow)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.HolyAvenger)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.JavelinOfLightning)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.LifeDrinker)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.LuckBlade)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.LuckBlade0)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.LuckBlade1)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.LuckBlade2)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.LuckBlade3)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.MaceOfBlood)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.MaceOfSmiting)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.MaceOfTerror)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.NetOfSnaring)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.NineLivesStealer)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.Oathbow)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.RapierOfPuncturing)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.ScreamingBolt)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.Shatterspike)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.ShiftersSorrow)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.SlayingArrow)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.SleepArrow)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.SunBlade)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.SwordOfLifeStealing)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.SwordOfSubtlety)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.SwordOfThePlanes)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.SylvanScimitar)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.TridentOfFishCommand)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.TridentOfWarning)]
        public void NameIsSpecific_ReturnsTrue(string gearType, string gear)
        {
            var isSpecific = specificGearGenerator.IsSpecific(gearType, gear);
            Assert.That(isSpecific, Is.True);
        }

        [TestCase(AttributeConstants.Shield, ArmorConstants.Buckler)]
        [TestCase(AttributeConstants.Shield, ArmorConstants.HeavySteelShield)]
        [TestCase(AttributeConstants.Shield, ArmorConstants.HeavyWoodenShield)]
        [TestCase(AttributeConstants.Shield, ArmorConstants.LightSteelShield)]
        [TestCase(AttributeConstants.Shield, ArmorConstants.LightWoodenShield)]
        [TestCase(AttributeConstants.Shield, ArmorConstants.TowerShield)]
        [TestCase(ItemTypeConstants.Armor, ArmorConstants.BandedMail)]
        [TestCase(ItemTypeConstants.Armor, ArmorConstants.Breastplate)]
        [TestCase(ItemTypeConstants.Armor, ArmorConstants.Chainmail)]
        [TestCase(ItemTypeConstants.Armor, ArmorConstants.ChainShirt)]
        [TestCase(ItemTypeConstants.Armor, ArmorConstants.FullPlate)]
        [TestCase(ItemTypeConstants.Armor, ArmorConstants.HalfPlate)]
        [TestCase(ItemTypeConstants.Armor, ArmorConstants.HideArmor)]
        [TestCase(ItemTypeConstants.Armor, ArmorConstants.LeatherArmor)]
        [TestCase(ItemTypeConstants.Armor, ArmorConstants.PaddedArmor)]
        [TestCase(ItemTypeConstants.Armor, ArmorConstants.ScaleMail)]
        [TestCase(ItemTypeConstants.Armor, ArmorConstants.SplintMail)]
        [TestCase(ItemTypeConstants.Armor, ArmorConstants.StuddedLeatherArmor)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.Arrow)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.Battleaxe)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.Bolas)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.Club)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.CompositeLongbow)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.CompositeLongbow_StrengthPlus0)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.CompositeLongbow_StrengthPlus1)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.CompositeLongbow_StrengthPlus2)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.CompositeLongbow_StrengthPlus3)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.CompositeLongbow_StrengthPlus4)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.CompositeShortbow)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.CompositeShortbow_StrengthPlus0)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.CompositeShortbow_StrengthPlus1)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.CompositeShortbow_StrengthPlus2)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.CrossbowBolt)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.Dagger)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.Dart)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.DireFlail)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.DwarvenUrgrosh)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.DwarvenWaraxe)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.Falchion)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.Flail)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.Gauntlet)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.Glaive)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.GnomeHookedHammer)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.Greataxe)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.Greatclub)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.Greatsword)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.Guisarme)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.Halberd)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.Handaxe)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.HandCrossbow)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.HeavyCrossbow)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.HeavyFlail)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.HeavyMace)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.HeavyPick)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.HeavyRepeatingCrossbow)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.Javelin)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.Kama)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.Kukri)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.Lance)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.LightCrossbow)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.LightHammer)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.LightMace)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.LightPick)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.LightRepeatingCrossbow)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.Longbow)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.Longspear)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.Longsword)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.Morningstar)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.Net)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.Nunchaku)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.OrcDoubleAxe)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.PincerStaff)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.PunchingDagger)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.Quarterstaff)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.Ranseur)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.Rapier)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.Sai)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.Sap)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.Scimitar)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.Scythe)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.Shortbow)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.Shortspear)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.ShortSword)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.Shuriken)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.Siangham)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.Sickle)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.Sling)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.SlingBullet)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.Spear)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.SpikedChain)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.SpikedGauntlet)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.ThrowingAxe)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.Trident)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.Warhammer)]
        [TestCase(ItemTypeConstants.Weapon, WeaponConstants.Whip)]
        public void NameIsSpecific_ReturnsFalse_NotSpecific(string gearType, string gear)
        {
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, AttributeConstants.Specific)).
                Returns(new[] { "other item name", "wrong item name" });
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, gearType)).
                Returns(new[] { "another item name", gear });

            var isSpecific = specificGearGenerator.IsSpecific(gearType, gear);
            Assert.That(isSpecific, Is.False);
        }

        [Test]
        public void NameIsSpecific_ReturnsFalse_NotAValidType()
        {
            Assert.That(() => specificGearGenerator.IsSpecific("gear type", "item name"),
                Throws.ArgumentException.With.Message.EqualTo("gear type is not a valid specific gear type"));
        }

        [TestCase(ItemTypeConstants.Weapon, ArmorConstants.AbsorbingShield)]
        [TestCase(ItemTypeConstants.Weapon, ArmorConstants.ArmorOfRage)]
        [TestCase(ItemTypeConstants.Armor, WeaponConstants.AssassinsDagger)]
        [TestCase(ItemTypeConstants.Armor, ArmorConstants.AbsorbingShield)]
        [TestCase(AttributeConstants.Shield, ArmorConstants.ArmorOfRage)]
        [TestCase(AttributeConstants.Shield, WeaponConstants.AssassinsDagger)]
        public void NameIsSpecific_ReturnsFalse_NotOfType(string gearType, string gear)
        {
            var isSpecific = specificGearGenerator.IsSpecific(gearType, gear);
            Assert.That(isSpecific, Is.False);
        }

        [TestCase(WeaponConstants.Battleaxe_Adamantine, WeaponConstants.Battleaxe)]
        [TestCase(WeaponConstants.Dagger_Adamantine, WeaponConstants.Dagger)]
        [TestCase(WeaponConstants.Dagger_Silver, WeaponConstants.Dagger)]
        [TestCase(WeaponConstants.LuckBlade, WeaponConstants.LuckBlade)]
        [TestCase(WeaponConstants.LuckBlade0, WeaponConstants.LuckBlade)]
        [TestCase(WeaponConstants.LuckBlade1, WeaponConstants.LuckBlade)]
        [TestCase(WeaponConstants.LuckBlade2, WeaponConstants.LuckBlade)]
        [TestCase(WeaponConstants.LuckBlade3, WeaponConstants.LuckBlade)]
        public void BUG_NameIsSpecific_ReturnsTrue_Renamed(string oldName, string newName)
        {
            mockReplacementSelector
                .Setup(s => s.SelectAll(It.IsAny<string>(), It.IsAny<bool>()))
                .Returns((string s, bool allow) => new[] { s });

            mockReplacementSelector
                .Setup(s => s.SelectAll(oldName, true))
                .Returns(new[] { newName });

            var isSpecific = specificGearGenerator.IsSpecific(ItemTypeConstants.Weapon, oldName);
            Assert.That(isSpecific, Is.True);
        }

        [TestCase(WeaponConstants.Battleaxe, WeaponConstants.Battleaxe_Adamantine)]
        [TestCase(WeaponConstants.Dagger, WeaponConstants.Dagger_Adamantine)]
        [TestCase(WeaponConstants.Dagger, WeaponConstants.Dagger_Silver)]
        public void BUG_NameIsSpecific_ReturnsFalse_Renamed(string generalName, string specificName)
        {
            mockReplacementSelector
                .Setup(s => s.SelectAll(It.IsAny<string>(), It.IsAny<bool>()))
                .Returns((string s, bool allow) => new[] { s });

            mockReplacementSelector
                .Setup(s => s.SelectAll(specificName, true))
                .Returns(new[] { generalName });

            var isSpecific = specificGearGenerator.IsSpecific(ItemTypeConstants.Weapon, generalName);
            Assert.That(isSpecific, Is.False);
        }

        [Test]
        public void GenerateChargesForRenamedItem()
        {
            var attributes = new[] { AttributeConstants.Charged };
            var tableName = string.Format(TableNameConstants.Collections.Formattable.SpecificITEMTYPEAttributes, ItemTypeConstants.Weapon);
            mockCollectionsSelector.Setup(s => s.SelectFrom(tableName, WeaponConstants.LuckBlade3)).Returns(attributes);

            var item = new Item();
            item.Name = WeaponConstants.LuckBlade3;

            mockReplacementSelector
                .Setup(s => s.SelectSingle(WeaponConstants.LuckBlade3))
                .Returns(WeaponConstants.LuckBlade);

            mockChargesGenerator
                .Setup(g => g.GenerateFor(ItemTypeConstants.Weapon, WeaponConstants.LuckBlade3))
                .Returns(3);

            var prototype = specificGearGenerator.GenerateFrom(item);
            Assert.That(prototype, Is.Not.Null);
            Assert.That(prototype.Name, Is.EqualTo(WeaponConstants.LuckBlade));
            Assert.That(prototype.Magic.Charges, Is.EqualTo(3));
        }

        [Test]
        public void BUG_GeneratePrototypeOfChangedName()
        {
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, AttributeConstants.Specific)).
                Returns(new[] { "other item name", "item name", WeaponConstants.LuckBlade, WeaponConstants.Dagger_Silver });
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, "gear type")).
                Returns(new[] { "another item name", "item name", WeaponConstants.LuckBlade, WeaponConstants.Dagger_Silver });

            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERSpecificITEMTYPEs, "power", "gear type");
            var selections = new[]
            {
                new TypeAndAmountSelection { Type = "other item name", Amount = 9266 },
                new TypeAndAmountSelection { Type = "item name", Amount = 90210 },
                new TypeAndAmountSelection { Type = WeaponConstants.LuckBlade0, Amount = 0 },
                new TypeAndAmountSelection { Type = WeaponConstants.Dagger_Silver, Amount = 600 },
            };
            mockTypeAndAmountPercentileSelector
                .Setup(s => s.SelectAllFrom(tableName))
                .Returns(selections);

            mockReplacementSelector
                .Setup(s => s.SelectAll(It.IsAny<string>(), It.IsAny<bool>()))
                .Returns((string s, bool allow) => new[] { s });

            mockReplacementSelector
                .Setup(s => s.SelectAll(WeaponConstants.LuckBlade0, true))
                .Returns(new[] { WeaponConstants.LuckBlade });

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.PowerGroups, WeaponConstants.LuckBlade))
                .Returns(new[] { "wrong power", "power", "other power" });

            var prototype = specificGearGenerator.GeneratePrototypeFrom("power", "gear type", WeaponConstants.LuckBlade);
            Assert.That(prototype, Is.Not.Null);
            Assert.That(prototype.Name, Is.EqualTo(WeaponConstants.LuckBlade0));
        }

        [Test]
        public void BUG_GeneratePrototypeOfRandomChangedName()
        {
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, AttributeConstants.Specific)).
                Returns(new[] { "other item name", "item name", WeaponConstants.LuckBlade, WeaponConstants.Dagger_Silver });
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, "gear type")).
                Returns(new[] { "another item name", "item name", WeaponConstants.LuckBlade, WeaponConstants.Dagger_Silver });

            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERSpecificITEMTYPEs, "power", "gear type");
            var selections = new[]
            {
                new TypeAndAmountSelection { Type = "other item name", Amount = 9266 },
                new TypeAndAmountSelection { Type = "item name", Amount = 90210 },
                new TypeAndAmountSelection { Type = WeaponConstants.LuckBlade1, Amount = 1 },
                new TypeAndAmountSelection { Type = WeaponConstants.LuckBlade2, Amount = 2 },
                new TypeAndAmountSelection { Type = WeaponConstants.LuckBlade3, Amount = 3 },
                new TypeAndAmountSelection { Type = WeaponConstants.Dagger_Silver, Amount = 600 },
            };
            mockTypeAndAmountPercentileSelector
                .Setup(s => s.SelectAllFrom(tableName))
                .Returns(selections);

            mockReplacementSelector
                .Setup(s => s.SelectAll(It.IsAny<string>(), It.IsAny<bool>()))
                .Returns((string s, bool allow) => new[] { s });

            mockReplacementSelector
                .Setup(s => s.SelectAll(WeaponConstants.LuckBlade3, true))
                .Returns(new[] { WeaponConstants.LuckBlade });

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.PowerGroups, WeaponConstants.LuckBlade))
                .Returns(new[] { "wrong power", "power", "other power" });

            var prototype = specificGearGenerator.GeneratePrototypeFrom("power", "gear type", WeaponConstants.LuckBlade);
            Assert.That(prototype, Is.Not.Null);
            Assert.That(prototype.Name, Is.EqualTo(WeaponConstants.LuckBlade3));
        }

        [Test]
        public void CanBeSpecific_ReturnsTrue_IsSpecific()
        {
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERSpecificITEMTYPEs, "power", "gear type");
            mockTypeAndAmountPercentileSelector
                .Setup(s => s.SelectAllFrom(tableName))
                .Returns(new[]
                {
                    new TypeAndAmountSelection { Type = "other item name", Amount = 9266 },
                    new TypeAndAmountSelection { Type = "item name", Amount = 90210 },
                });

            var canBeSpecific = specificGearGenerator.CanBeSpecific("power", ItemTypeConstants.Weapon, WeaponConstants.SunBlade);
            Assert.That(canBeSpecific, Is.True);
        }

        [TestCase(ItemTypeConstants.Weapon)]
        [TestCase(ItemTypeConstants.Armor)]
        [TestCase(AttributeConstants.Shield)]
        public void CanBeSpecific_ReturnsTrue_BaseNameMatches(string gearType)
        {
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERSpecificITEMTYPEs, "power", gearType);
            mockTypeAndAmountPercentileSelector
                .Setup(s => s.SelectAllFrom(tableName))
                .Returns(new[]
                {
                    new TypeAndAmountSelection { Type = "another item name", Amount = 9266 },
                    new TypeAndAmountSelection { Type = "wrong item name", Amount = 90210 },
                });

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, "wrong item name")).
                Returns(new[] { "base name", "wrong base name" });
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, "another item name")).
                Returns(new[] { "base name", "item name" });

            var canBeSpecific = specificGearGenerator.CanBeSpecific("power", gearType, "item name");
            Assert.That(canBeSpecific, Is.True);
        }

        [Test]
        public void CanBeSpecific_ReturnsFalse()
        {
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERSpecificITEMTYPEs, "power", "gear type");
            mockTypeAndAmountPercentileSelector
                .Setup(s => s.SelectAllFrom(tableName))
                .Returns(new[]
                {
                    new TypeAndAmountSelection { Type = "another item name", Amount = 9266 },
                    new TypeAndAmountSelection { Type = "wrong item name", Amount = 90210 },
                });

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, "wrong item name")).
                Returns(new[] { "base name", "wrong base name" });
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, "another item name")).
                Returns(new[] { "base name", "another base name" });

            var canBeSpecific = specificGearGenerator.CanBeSpecific("power", ItemTypeConstants.Weapon, "item name");
            Assert.That(canBeSpecific, Is.False);
        }

        [Test]
        public void GenerateNameFrom_1Option()
        {
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERSpecificITEMTYPEs, "power", "gear type");
            mockTypeAndAmountPercentileSelector
                .Setup(s => s.SelectAllFrom(tableName))
                .Returns(new[]
                {
                    new TypeAndAmountSelection { Type = "another item name", Amount = 9266 },
                    new TypeAndAmountSelection { Type = "item name", Amount = 90210 },
                });
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, "another item name")).
                Returns(new[] { "other base name", "wrong base name" });
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, "item name")).
                Returns(new[] { "other base name", "base name" });

            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(It.IsAny<IEnumerable<string>>()))
                .Returns((IEnumerable<string> c) => c.Last());

            var name = specificGearGenerator.GenerateNameFrom("power", "gear type", "base name");
            Assert.That(name, Is.EqualTo("item name"));
        }

        [Test]
        public void GenerateNameFrom_2Options_SelectsRandomly()
        {
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERSpecificITEMTYPEs, "power", "gear type");
            mockTypeAndAmountPercentileSelector
                .Setup(s => s.SelectAllFrom(tableName))
                .Returns(new[]
                {
                    new TypeAndAmountSelection { Type = "item name", Amount = 90210 },
                    new TypeAndAmountSelection { Type = "another item name", Amount = 9266 },
                });
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, "another item name")).
                Returns(new[] { "base name", "wrong base name" });
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, "item name")).
                Returns(new[] { "other base name", "base name" });

            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(It.IsAny<IEnumerable<string>>()))
                .Returns((IEnumerable<string> c) => c.Last());

            var name = specificGearGenerator.GenerateNameFrom("power", "gear type", "base name");
            Assert.That(name, Is.EqualTo("another item name"));
        }

        [Test]
        public void GenerateNameFrom_ThrowsArgumentException_WhenCannotBeSpecific()
        {
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERSpecificITEMTYPEs, "power", "gear type");
            mockTypeAndAmountPercentileSelector
                .Setup(s => s.SelectAllFrom(tableName))
                .Returns(new[]
                {
                    new TypeAndAmountSelection { Type = "another item name", Amount = 9266 },
                    new TypeAndAmountSelection { Type = "item name", Amount = 90210 },
                });
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, "another item name")).
                Returns(new[] { "other base name", "wrong base name" });
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, "item name")).
                Returns(new[] { "other base name", "wrong base name" });

            Assert.That(() => specificGearGenerator.GenerateNameFrom("power", "gear type", "base name"),
                Throws.ArgumentException.With.Message.EqualTo($"No power specific gear type has base type base name"));
        }

        [Test]
        public void GenerateRandomNameFrom_ReturnsRandomName()
        {
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERSpecificITEMTYPEs, "power", "gear type");
            mockTypeAndAmountPercentileSelector
                .Setup(s => s.SelectFrom(tableName))
                .Returns(new TypeAndAmountSelection { Type = "item name", Amount = 90210 });

            var name = specificGearGenerator.GenerateRandomNameFrom("power", "gear type");
            Assert.That(name, Is.EqualTo("item name"));
        }
    }
}