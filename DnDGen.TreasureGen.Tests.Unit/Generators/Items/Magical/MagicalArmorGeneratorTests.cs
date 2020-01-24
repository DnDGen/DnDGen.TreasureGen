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
using System.Linq;

namespace DnDGen.TreasureGen.Tests.Unit.Generators.Items.Magical
{
    [TestFixture]
    public class MagicalArmorGeneratorTests
    {
        private MagicalItemGenerator magicalArmorGenerator;
        private Mock<ITypeAndAmountPercentileSelector> mockTypeAndAmountPercentileSelector;
        private Mock<ITreasurePercentileSelector> mockPercentileSelector;
        private Mock<ICollectionSelector> mockCollectionsSelector;
        private Mock<ISpecialAbilitiesGenerator> mockSpecialAbilitiesGenerator;
        private Mock<ISpecificGearGenerator> mockSpecificGearGenerator;
        private Mock<MundaneItemGenerator> mockMundaneArmorGenerator;
        private Mock<JustInTimeFactory> mockJustInTimeFactory;
        private TypeAndAmountSelection selection;
        private string power;
        private ItemVerifier itemVerifier;
        private Armor mundaneArmor;

        [SetUp]
        public void Setup()
        {
            mockPercentileSelector = new Mock<ITreasurePercentileSelector>();
            mockCollectionsSelector = new Mock<ICollectionSelector>();
            mockSpecialAbilitiesGenerator = new Mock<ISpecialAbilitiesGenerator>();
            mockSpecificGearGenerator = new Mock<ISpecificGearGenerator>();
            mockTypeAndAmountPercentileSelector = new Mock<ITypeAndAmountPercentileSelector>();
            mockMundaneArmorGenerator = new Mock<MundaneItemGenerator>();
            mockJustInTimeFactory = new Mock<JustInTimeFactory>();

            mockJustInTimeFactory.Setup(f => f.Build<MundaneItemGenerator>(ItemTypeConstants.Armor)).Returns(mockMundaneArmorGenerator.Object);

            magicalArmorGenerator = new MagicalArmorGenerator(
                mockTypeAndAmountPercentileSelector.Object,
                mockPercentileSelector.Object,
                mockCollectionsSelector.Object,
                mockSpecialAbilitiesGenerator.Object,
                mockSpecificGearGenerator.Object,
                mockJustInTimeFactory.Object);

            itemVerifier = new ItemVerifier();

            selection = new TypeAndAmountSelection();
            selection.Type = "armor type";
            selection.Amount = 9266;
            power = "power";

            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, power, ItemTypeConstants.Armor);
            mockTypeAndAmountPercentileSelector.Setup(p => p.SelectFrom(tableName)).Returns(selection);

            tableName = string.Format(TableNameConstants.Percentiles.Formattable.ARMORTYPETypes, selection.Type);
            mockPercentileSelector.Setup(p => p.SelectFrom(tableName)).Returns("armor name");

            mundaneArmor = new Armor();
            mundaneArmor.Name = "armor name";
            mockMundaneArmorGenerator.Setup(g => g.GenerateFrom(It.Is<Item>(i => i.NameMatches("armor name")), false)).Returns(mundaneArmor);
        }

        [Test]
        public void GetArmor()
        {
            var armor = magicalArmorGenerator.GenerateFrom(power);
            Assert.That(armor, Is.EqualTo(mundaneArmor));
        }

        [Test]
        public void GetBonusFromSelector()
        {
            var armor = magicalArmorGenerator.GenerateFrom(power);
            Assert.That(armor.Magic.Bonus, Is.EqualTo(9266));
        }

        [Test]
        public void MagicArmorIsMasterwork()
        {
            var armor = magicalArmorGenerator.GenerateFrom(power);
            Assert.That(armor.Traits, Contains.Item(TraitConstants.Masterwork));
        }

        [Test]
        public void GetNameFromPercentileSelector()
        {
            var armor = magicalArmorGenerator.GenerateFrom(power);
            Assert.That(armor.Name, Is.EqualTo("armor name"));
        }

        [Test]
        public void GetSpecialAbilitiesFromGenerator()
        {
            var abilityResult = new TypeAndAmountSelection();
            abilityResult.Type = "SpecialAbility";
            abilityResult.Amount = 1;

            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, power, ItemTypeConstants.Armor);
            mockTypeAndAmountPercentileSelector.SetupSequence(p => p.SelectFrom(tableName)).Returns(abilityResult).Returns(abilityResult).Returns(selection);

            var abilities = new[] { new SpecialAbility() };
            mockSpecialAbilitiesGenerator.Setup(p => p.GenerateFor(mundaneArmor, power, 2)).Returns(abilities);

            var armor = magicalArmorGenerator.GenerateFrom(power);
            Assert.That(armor.Magic.SpecialAbilities, Is.EqualTo(abilities));
        }

        [Test]
        public void GetSpecificArmorsFromGenerator()
        {
            selection.Amount = 0;

            var specificArmor = new Armor();
            specificArmor.Name = "specific armor";

            mockSpecificGearGenerator.Setup(g => g.GenerateRandomNameFrom(power, selection.Type)).Returns("specific armor");
            mockSpecificGearGenerator.Setup(g => g.GeneratePrototypeFrom(power, selection.Type, "specific armor")).Returns(specificArmor);
            mockSpecificGearGenerator.Setup(g => g.GenerateFrom(It.Is<Item>(i => i.NameMatches("specific armor")))).Returns(specificArmor);
            mockSpecificGearGenerator.Setup(g => g.IsSpecific(It.Is<Item>(i => i.NameMatches("specific armor")))).Returns(true);

            var armor = magicalArmorGenerator.GenerateFrom(power);
            Assert.That(armor, Is.EqualTo(specificArmor));
            mockSpecialAbilitiesGenerator.Verify(g => g.GenerateFor(It.IsAny<Item>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never);
        }

        [Test]
        public void GenerateCustomArmor()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);

            var specialAbilityNames = template.Magic.SpecialAbilities.Select(a => a.Name);
            var abilities = new[]
            {
                new SpecialAbility { Name = specialAbilityNames.First() },
                new SpecialAbility { Name = specialAbilityNames.Last() }
            };

            mockSpecialAbilitiesGenerator.Setup(p => p.GenerateFor(template.Magic.SpecialAbilities)).Returns(abilities);

            var templateMundaneArmor = new Armor();
            templateMundaneArmor.Name = name;
            mockMundaneArmorGenerator.Setup(g => g.GenerateFrom(It.Is<Item>(i => i.NameMatches(name)), false)).Returns(templateMundaneArmor);

            var armor = magicalArmorGenerator.GenerateFrom(template);
            Assert.That(armor, Is.EqualTo(templateMundaneArmor));
            Assert.That(armor.Quantity, Is.EqualTo(1));
            Assert.That(armor.Magic.Bonus, Is.EqualTo(template.Magic.Bonus));
            Assert.That(armor.Magic.Charges, Is.EqualTo(template.Magic.Charges));
            Assert.That(armor.Magic.Curse, Is.EqualTo(template.Magic.Curse));
            Assert.That(armor.Magic.Intelligence.Ego, Is.EqualTo(template.Magic.Intelligence.Ego));
            Assert.That(armor.Magic.SpecialAbilities, Is.EquivalentTo(abilities));

            //INFO: Custom magic armor should be masterwork
            Assert.That(armor.Traits, Contains.Item(TraitConstants.Masterwork));
        }

        [Test]
        public void GenerateRandomCustomArmor()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);
            var specialAbilityNames = template.Magic.SpecialAbilities.Select(a => a.Name);

            var abilities = new[]
            {
                new SpecialAbility { Name = specialAbilityNames.First() },
                new SpecialAbility { Name = specialAbilityNames.Last() }
            };

            mockSpecialAbilitiesGenerator.Setup(p => p.GenerateFor(template.Magic.SpecialAbilities)).Returns(abilities);

            var templateMundaneArmor = new Armor();
            templateMundaneArmor.Name = name;
            mockMundaneArmorGenerator.Setup(g => g.GenerateFrom(It.Is<Item>(i => i.NameMatches(name)), false)).Returns(templateMundaneArmor);

            var armor = magicalArmorGenerator.GenerateFrom(template, true);
            Assert.That(armor, Is.EqualTo(templateMundaneArmor));
            Assert.That(armor.Quantity, Is.EqualTo(1));
            Assert.That(armor.Magic.Bonus, Is.EqualTo(template.Magic.Bonus));
            Assert.That(armor.Magic.Charges, Is.EqualTo(template.Magic.Charges));
            Assert.That(armor.Magic.Curse, Is.EqualTo(template.Magic.Curse));
            Assert.That(armor.Magic.Intelligence.Ego, Is.EqualTo(template.Magic.Intelligence.Ego));
            Assert.That(armor.Magic.SpecialAbilities, Is.EquivalentTo(abilities));

            //INFO: Custom magic armor should be masterwork
            Assert.That(armor.Traits, Contains.Item(TraitConstants.Masterwork));
        }

        [Test]
        public void GenerateSpecificCustomArmor()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);
            var specialAbilityNames = template.Magic.SpecialAbilities.Select(a => a.Name);

            var tableName = string.Format(TableNameConstants.Collections.Formattable.ITEMTYPEAttributes, ItemTypeConstants.Armor);
            mockCollectionsSelector.Setup(p => p.SelectFrom(tableName, name)).Throws<ArgumentException>();

            var abilities = new[]
            {
                new SpecialAbility { Name = specialAbilityNames.First() },
                new SpecialAbility { Name = specialAbilityNames.Last() }
            };

            mockSpecialAbilitiesGenerator.Setup(p => p.GenerateFor(template.Magic.SpecialAbilities)).Returns(abilities);

            var specificArmor = itemVerifier.CreateRandomArmorTemplate(name);
            specificArmor.ItemType = ItemTypeConstants.Armor;
            mockSpecificGearGenerator.Setup(g => g.GenerateFrom(It.Is<Item>(i => i.NameMatches(name)))).Returns(specificArmor);
            mockSpecificGearGenerator.Setup(g => g.IsSpecific(It.Is<Item>(i => i.NameMatches(name)))).Returns(true);

            var armor = magicalArmorGenerator.GenerateFrom(template, true);
            Assert.That(armor.Quantity, Is.EqualTo(1));
            Assert.That(armor.Name, Is.EqualTo(specificArmor.Name));
            Assert.That(armor.BaseNames, Is.EquivalentTo(specificArmor.BaseNames));
            Assert.That(armor.ItemType, Is.EqualTo(ItemTypeConstants.Armor));
            Assert.That(armor.Magic.SpecialAbilities, Is.EquivalentTo(specificArmor.Magic.SpecialAbilities));
        }

        [Test]
        public void GenerateFromName_Armor()
        {
            var abilityResult = new TypeAndAmountSelection();
            abilityResult.Type = "SpecialAbility";
            abilityResult.Amount = 1;

            var tableName = string.Format(TableNameConstants.Collections.Formattable.ITEMTYPEAttributes, ItemTypeConstants.Armor);
            mockCollectionsSelector.Setup(p => p.SelectFrom(tableName, "armor name")).Returns(new[] { "attribute", "not shield attribute" });

            tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, power, ItemTypeConstants.Armor);
            mockTypeAndAmountPercentileSelector.SetupSequence(p => p.SelectFrom(tableName))
                .Returns(abilityResult)
                .Returns(new TypeAndAmountSelection { Type = ItemTypeConstants.Armor, Amount = 9266 })
                .Returns(abilityResult)
                .Returns(abilityResult)
                .Returns(new TypeAndAmountSelection { Type = "whatever", Amount = 9266 }); ;

            var abilities = new[] { new SpecialAbility() };
            mockSpecialAbilitiesGenerator.Setup(p => p.GenerateFor(It.IsAny<Item>(), power, 2)).Returns(abilities);
            mockMundaneArmorGenerator.Setup(g => g.GenerateFrom(It.IsAny<Item>(), false)).Returns((Item template, bool decorate) => new Armor { Name = template.Name, BaseNames = new[] { "from mundane" } });

            var armor = magicalArmorGenerator.GenerateFrom(power, "armor name");
            Assert.That(armor.Name, Is.EqualTo("armor name"));
            Assert.That(armor.BaseNames, Contains.Item("from mundane"));
            Assert.That(armor.Magic.Bonus, Is.EqualTo(9266));
            Assert.That(armor.Magic.SpecialAbilities, Is.EquivalentTo(abilities));
        }

        [Test]
        public void GenerateFromName_Shield()
        {
            var abilityResult = new TypeAndAmountSelection();
            abilityResult.Type = "SpecialAbility";
            abilityResult.Amount = 1;

            var tableName = string.Format(TableNameConstants.Collections.Formattable.ITEMTYPEAttributes, ItemTypeConstants.Armor);
            mockCollectionsSelector.Setup(p => p.SelectFrom(tableName, "armor name")).Returns(new[] { "attribute", AttributeConstants.Shield });

            tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, power, ItemTypeConstants.Armor);
            mockTypeAndAmountPercentileSelector.SetupSequence(p => p.SelectFrom(tableName))
                .Returns(abilityResult)
                .Returns(new TypeAndAmountSelection { Type = AttributeConstants.Shield, Amount = 9266 })
                .Returns(abilityResult)
                .Returns(abilityResult)
                .Returns(new TypeAndAmountSelection { Type = "whatever", Amount = 9266 }); ;

            var abilities = new[] { new SpecialAbility() };
            mockSpecialAbilitiesGenerator.Setup(p => p.GenerateFor(It.IsAny<Item>(), power, 2)).Returns(abilities);
            mockMundaneArmorGenerator.Setup(g => g.GenerateFrom(It.IsAny<Item>(), false)).Returns((Item template, bool decorate) => new Armor { Name = template.Name, BaseNames = new[] { "from mundane" } });

            var armor = magicalArmorGenerator.GenerateFrom(power, "armor name");
            Assert.That(armor.Name, Is.EqualTo("armor name"));
            Assert.That(armor.BaseNames, Contains.Item("from mundane"));
            Assert.That(armor.Magic.Bonus, Is.EqualTo(9266));
            Assert.That(armor.Magic.SpecialAbilities, Is.EquivalentTo(abilities));
        }

        [Test]
        public void GenerateSpecificFromName_Armor()
        {
            var abilityResult = new TypeAndAmountSelection();
            abilityResult.Type = "SpecialAbility";
            abilityResult.Amount = 1;

            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, power, ItemTypeConstants.Armor);
            mockTypeAndAmountPercentileSelector.SetupSequence(p => p.SelectFrom(tableName))
                .Returns(abilityResult).Returns(new TypeAndAmountSelection { Type = "wrong armor type", Amount = 666 })
                .Returns(abilityResult).Returns(abilityResult).Returns(new TypeAndAmountSelection { Type = "armor type", Amount = 0 })
                .Returns(abilityResult).Returns(abilityResult).Returns(abilityResult).Returns(new TypeAndAmountSelection { Type = "other armor type", Amount = 90210 });

            tableName = string.Format(TableNameConstants.Percentiles.Formattable.ARMORTYPETypes, "wrong armor type");
            mockPercentileSelector.Setup(p => p.SelectFrom(tableName)).Returns("wrong armor name");

            tableName = string.Format(TableNameConstants.Percentiles.Formattable.ARMORTYPETypes, "other armor type");
            mockPercentileSelector.Setup(p => p.SelectFrom(tableName)).Returns("other armor name");

            mockMundaneArmorGenerator.Setup(g => g.GenerateFrom(It.IsAny<Item>(), false)).Returns((Item template, bool decorate) => new Armor { Name = template.Name, BaseNames = new[] { "from mundane" } });

            var baseNames = new[] { "base name", "other base name" };
            var attributes = new[] { "type 1", "type 2" };
            var abilities = new[] { new SpecialAbility(), new SpecialAbility() };

            var specificArmor = new Armor();
            specificArmor.Name = "specific armor";
            specificArmor.BaseNames = baseNames;
            specificArmor.ItemType = ItemTypeConstants.Armor;
            specificArmor.Attributes = attributes;
            specificArmor.Magic.Bonus = 42;
            specificArmor.Magic.SpecialAbilities = abilities;

            mockSpecificGearGenerator.Setup(g => g.GeneratePrototypeFrom(power, "armor type", "specific armor")).Returns(specificArmor);
            mockSpecificGearGenerator.Setup(g => g.GenerateFrom(It.Is<Item>(i => i.Name == specificArmor.Name))).Returns(specificArmor);
            mockSpecificGearGenerator.Setup(g => g.IsSpecific(It.Is<Item>(i => i.Name == specificArmor.Name))).Returns(true);
            mockSpecificGearGenerator.Setup(g => g.IsSpecific(ItemTypeConstants.Armor, "specific armor")).Returns(true);

            var armor = magicalArmorGenerator.GenerateFrom(power, "specific armor");
            Assert.That(armor, Is.EqualTo(specificArmor));
            Assert.That(armor.Name, Is.EqualTo("specific armor"));
            Assert.That(armor.BaseNames, Is.EqualTo(baseNames));
            Assert.That(armor.ItemType, Is.EqualTo(ItemTypeConstants.Armor));
            Assert.That(armor.Quantity, Is.EqualTo(1));
            Assert.That(armor.Magic.Bonus, Is.EqualTo(42));
            Assert.That(armor.Attributes, Is.EqualTo(attributes));
            Assert.That(armor.Magic.SpecialAbilities, Is.EquivalentTo(abilities));
        }

        [Test]
        public void GenerateSpecificFromName_Shield()
        {
            var abilityResult = new TypeAndAmountSelection();
            abilityResult.Type = "SpecialAbility";
            abilityResult.Amount = 1;

            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, power, ItemTypeConstants.Armor);
            mockTypeAndAmountPercentileSelector.SetupSequence(p => p.SelectFrom(tableName))
                .Returns(abilityResult).Returns(new TypeAndAmountSelection { Type = "wrong armor type", Amount = 666 })
                .Returns(abilityResult).Returns(abilityResult).Returns(new TypeAndAmountSelection { Type = "armor type", Amount = 0 })
                .Returns(abilityResult).Returns(abilityResult).Returns(abilityResult).Returns(new TypeAndAmountSelection { Type = "other armor type", Amount = 90210 });

            tableName = string.Format(TableNameConstants.Percentiles.Formattable.ARMORTYPETypes, "wrong armor type");
            mockPercentileSelector.Setup(p => p.SelectFrom(tableName)).Returns("wrong armor name");

            tableName = string.Format(TableNameConstants.Percentiles.Formattable.ARMORTYPETypes, "other armor type");
            mockPercentileSelector.Setup(p => p.SelectFrom(tableName)).Returns("other armor name");

            mockMundaneArmorGenerator.Setup(g => g.GenerateFrom(It.IsAny<Item>(), false)).Returns((Item template, bool decorate) => new Armor { Name = template.Name, BaseNames = new[] { "from mundane" } });

            var baseNames = new[] { "base name", "other base name" };
            var attributes = new[] { "type 1", "type 2" };
            var abilities = new[] { new SpecialAbility(), new SpecialAbility() };

            var specificArmor = new Armor();
            specificArmor.Name = "specific shield";
            specificArmor.BaseNames = baseNames;
            specificArmor.ItemType = ItemTypeConstants.Armor;
            specificArmor.Attributes = attributes;
            specificArmor.Magic.Bonus = 42;
            specificArmor.Magic.SpecialAbilities = abilities;

            mockSpecificGearGenerator.Setup(g => g.GeneratePrototypeFrom(power, "armor type", "specific shield")).Returns(specificArmor);
            mockSpecificGearGenerator.Setup(g => g.GenerateFrom(It.Is<Item>(i => i.Name == specificArmor.Name))).Returns(specificArmor);
            mockSpecificGearGenerator.Setup(g => g.IsSpecific(It.Is<Item>(i => i.Name == specificArmor.Name))).Returns(true);
            mockSpecificGearGenerator.Setup(g => g.IsSpecific(AttributeConstants.Shield, "specific shield")).Returns(true);

            var armor = magicalArmorGenerator.GenerateFrom(power, "specific shield");
            Assert.That(armor, Is.EqualTo(specificArmor));
            Assert.That(armor.Name, Is.EqualTo("specific shield"));
            Assert.That(armor.BaseNames, Is.EqualTo(baseNames));
            Assert.That(armor.ItemType, Is.EqualTo(ItemTypeConstants.Armor));
            Assert.That(armor.Quantity, Is.EqualTo(1));
            Assert.That(armor.Magic.Bonus, Is.EqualTo(42));
            Assert.That(armor.Attributes, Is.EqualTo(attributes));
            Assert.That(armor.Magic.SpecialAbilities, Is.EquivalentTo(abilities));
        }

        [Test]
        public void GenerateSpecificFromBaseName_Armor()
        {
            var abilityResult = new TypeAndAmountSelection();
            abilityResult.Type = "SpecialAbility";
            abilityResult.Amount = 1;

            var tableName = string.Format(TableNameConstants.Collections.Formattable.ITEMTYPEAttributes, ItemTypeConstants.Armor);
            mockCollectionsSelector.Setup(p => p.SelectFrom(tableName, "armor name")).Returns(new[] { "attribute", "other attribute" });

            tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, power, ItemTypeConstants.Armor);
            mockTypeAndAmountPercentileSelector.SetupSequence(p => p.SelectFrom(tableName))
                .Returns(abilityResult)
                .Returns(abilityResult)
                .Returns(new TypeAndAmountSelection { Type = ItemTypeConstants.Armor, Amount = 9266 });

            tableName = string.Format(TableNameConstants.Percentiles.Formattable.ARMORTYPETypes, ItemTypeConstants.Armor);
            mockPercentileSelector.Setup(p => p.SelectFrom(tableName)).Returns("armor name");

            var baseNames = new[] { "from mundane", "base name" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, "armor name")).Returns(baseNames);
            mockMundaneArmorGenerator.Setup(g => g.GenerateFrom(It.IsAny<Item>(), false)).Returns((Item template, bool decorate) => new Armor { Name = template.Name, BaseNames = baseNames });

            var attributes = new[] { "type 1", "type 2" };
            var abilities = new[] { new SpecialAbility(), new SpecialAbility() };

            var specificArmor = new Armor();
            specificArmor.Name = "specific armor";
            specificArmor.BaseNames = baseNames;
            specificArmor.ItemType = ItemTypeConstants.Armor;
            specificArmor.Attributes = attributes;
            specificArmor.Magic.Bonus = 42;
            specificArmor.Magic.SpecialAbilities = abilities;

            mockSpecificGearGenerator.Setup(g => g.GeneratePrototypeFrom(power, "armor type", "specific armor")).Returns(specificArmor);
            mockSpecificGearGenerator.Setup(g => g.GenerateFrom(It.Is<Item>(i => i.Name == specificArmor.Name))).Returns(specificArmor);
            mockSpecificGearGenerator.Setup(g => g.IsSpecific(It.Is<Item>(i => i.Name == specificArmor.Name))).Returns(true);
            mockSpecificGearGenerator.Setup(g => g.IsSpecific(ItemTypeConstants.Armor, "specific armor")).Returns(true);

            var armor = magicalArmorGenerator.GenerateFrom(power, "base name");
            Assert.That(armor.Name, Is.EqualTo("specific armor"));
            Assert.That(armor.BaseNames, Contains.Item("from mundane"));
            Assert.That(armor.BaseNames, Contains.Item("base name"));
            Assert.That(armor.Magic.Bonus, Is.EqualTo(9266));
            Assert.That(armor.Magic.SpecialAbilities, Is.EquivalentTo(abilities));
        }

        [Test]
        public void GenerateSpecificFromBaseName_Shield()
        {
            var abilityResult = new TypeAndAmountSelection();
            abilityResult.Type = "SpecialAbility";
            abilityResult.Amount = 1;

            var tableName = string.Format(TableNameConstants.Collections.Formattable.ITEMTYPEAttributes, ItemTypeConstants.Armor);
            mockCollectionsSelector.Setup(p => p.SelectFrom(tableName, "armor name")).Returns(new[] { "attribute", AttributeConstants.Shield });

            tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, power, ItemTypeConstants.Armor);
            mockTypeAndAmountPercentileSelector.SetupSequence(p => p.SelectFrom(tableName))
                .Returns(abilityResult).Returns(abilityResult).Returns(new TypeAndAmountSelection { Type = AttributeConstants.Shield, Amount = 9266 });

            tableName = string.Format(TableNameConstants.Percentiles.Formattable.ARMORTYPETypes, AttributeConstants.Shield);
            mockPercentileSelector.Setup(p => p.SelectFrom(tableName)).Returns("armor name");

            var baseNames = new[] { "from mundane", "base name" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, "armor name")).Returns(baseNames);
            mockMundaneArmorGenerator.Setup(g => g.GenerateFrom(It.IsAny<Item>(), false)).Returns((Item template, bool decorate) => new Armor { Name = template.Name, BaseNames = baseNames });

            var attributes = new[] { "type 1", "type 2" };
            var abilities = new[] { new SpecialAbility(), new SpecialAbility() };

            var specificArmor = new Armor();
            specificArmor.Name = "specific shield";
            specificArmor.BaseNames = baseNames;
            specificArmor.ItemType = ItemTypeConstants.Armor;
            specificArmor.Attributes = attributes;
            specificArmor.Magic.Bonus = 42;
            specificArmor.Magic.SpecialAbilities = abilities;

            mockSpecificGearGenerator.Setup(g => g.GeneratePrototypeFrom(power, "armor type", "specific shield")).Returns(specificArmor);
            mockSpecificGearGenerator.Setup(g => g.GenerateFrom(It.Is<Item>(i => i.Name == specificArmor.Name))).Returns(specificArmor);
            mockSpecificGearGenerator.Setup(g => g.IsSpecific(It.Is<Item>(i => i.Name == specificArmor.Name))).Returns(true);
            mockSpecificGearGenerator.Setup(g => g.IsSpecific(AttributeConstants.Shield, "specific shield")).Returns(true);

            var armor = magicalArmorGenerator.GenerateFrom(power, "base name");
            Assert.That(armor.Name, Is.EqualTo("specific shield"));
            Assert.That(armor.BaseNames, Contains.Item("from mundane"));
            Assert.That(armor.BaseNames, Contains.Item("base name"));
            Assert.That(armor.Magic.Bonus, Is.EqualTo(9266));
            Assert.That(armor.Magic.SpecialAbilities, Is.EquivalentTo(abilities));
        }
    }
}