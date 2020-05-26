﻿using DnDGen.Infrastructure.Generators;
using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.TreasureGen.Generators.Items;
using DnDGen.TreasureGen.Generators.Items.Magical;
using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Items.Mundane;
using DnDGen.TreasureGen.Selectors.Percentiles;
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
        private Mock<ITreasurePercentileSelector> mockPercentileSelector;
        private Mock<ICollectionSelector> mockCollectionsSelector;
        private Mock<ISpecialAbilitiesGenerator> mockSpecialAbilitiesGenerator;
        private Mock<ISpecificGearGenerator> mockSpecificGearGenerator;
        private Mock<MundaneItemGenerator> mockMundaneArmorGenerator;
        private Mock<JustInTimeFactory> mockJustInTimeFactory;
        private ItemVerifier itemVerifier;

        [SetUp]
        public void Setup()
        {
            mockPercentileSelector = new Mock<ITreasurePercentileSelector>();
            mockCollectionsSelector = new Mock<ICollectionSelector>();
            mockSpecialAbilitiesGenerator = new Mock<ISpecialAbilitiesGenerator>();
            mockSpecificGearGenerator = new Mock<ISpecificGearGenerator>();
            mockMundaneArmorGenerator = new Mock<MundaneItemGenerator>();
            mockJustInTimeFactory = new Mock<JustInTimeFactory>();

            mockJustInTimeFactory.Setup(f => f.Build<MundaneItemGenerator>(ItemTypeConstants.Armor)).Returns(mockMundaneArmorGenerator.Object);

            magicalArmorGenerator = new MagicalArmorGenerator(
                mockPercentileSelector.Object,
                mockCollectionsSelector.Object,
                mockSpecialAbilitiesGenerator.Object,
                mockSpecificGearGenerator.Object,
                mockJustInTimeFactory.Object);

            itemVerifier = new ItemVerifier();
        }

        [Test]
        public void GetArmor()
        {
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERArmorTypes, "power", ItemTypeConstants.Armor);
            mockPercentileSelector.Setup(s => s.SelectFrom(tableName)).Returns("armor type");

            tableName = string.Format(TableNameConstants.Percentiles.Formattable.ARMORTYPETypes, "armor type");
            mockPercentileSelector.Setup(s => s.SelectFrom(tableName)).Returns("armor name");

            tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, "power", ItemTypeConstants.Armor);
            mockPercentileSelector
                .SetupSequence(s => s.SelectFrom(tableName))
                .Returns(9266.ToString());

            var mundaneArmor = new Armor();
            mundaneArmor.Name = "armor name";
            mockMundaneArmorGenerator.Setup(g => g.Generate(It.Is<Item>(i => i.NameMatches("armor name")), false)).Returns(mundaneArmor);

            var armor = magicalArmorGenerator.GenerateRandom("power");
            Assert.That(armor, Is.EqualTo(mundaneArmor));
            Assert.That(armor.Name, Is.EqualTo("armor name"));
            Assert.That(armor.Magic.Bonus, Is.EqualTo(9266));
            Assert.That(armor.Traits, Contains.Item(TraitConstants.Masterwork));
        }

        [Test]
        public void GetSpecialAbilitiesFromGenerator()
        {
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERArmorTypes, "power", ItemTypeConstants.Armor);
            mockPercentileSelector.Setup(s => s.SelectFrom(tableName)).Returns("armor type");

            tableName = string.Format(TableNameConstants.Percentiles.Formattable.ARMORTYPETypes, "armor type");
            mockPercentileSelector.Setup(s => s.SelectFrom(tableName)).Returns("armor name");

            tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, "power", ItemTypeConstants.Armor);
            mockPercentileSelector
                .SetupSequence(p => p.SelectFrom(tableName))
                .Returns("SpecialAbility")
                .Returns("SpecialAbility")
                .Returns(9266.ToString());

            var mundaneArmor = new Armor();
            mundaneArmor.Name = "armor name";
            mockMundaneArmorGenerator.Setup(g => g.Generate(It.Is<Item>(i => i.NameMatches("armor name")), false)).Returns(mundaneArmor);

            var abilities = new[] { new SpecialAbility() };
            mockSpecialAbilitiesGenerator.Setup(p => p.GenerateFor(mundaneArmor, "power", 2)).Returns(abilities);

            var armor = magicalArmorGenerator.GenerateRandom("power");
            Assert.That(armor.Magic.SpecialAbilities, Is.EqualTo(abilities));
        }

        [Test]
        public void GetSpecificArmorsFromGenerator()
        {
            var specificArmor = new Armor();
            specificArmor.Name = "specific armor";

            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERArmorTypes, "power", ItemTypeConstants.Armor);
            mockPercentileSelector.Setup(s => s.SelectFrom(tableName)).Returns("armor type");

            tableName = string.Format(TableNameConstants.Percentiles.Formattable.ARMORTYPETypes, "armor type");
            mockPercentileSelector.Setup(s => s.SelectFrom(tableName)).Returns("armor name");

            tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, "power", ItemTypeConstants.Armor);
            mockPercentileSelector
                .SetupSequence(s => s.SelectFrom(tableName))
                .Returns("SpecialAbility")
                .Returns("SpecialAbility")
                .Returns(ItemTypeConstants.Armor);

            mockSpecificGearGenerator.Setup(g => g.CanBeSpecific("power", "armor type", "armor name")).Returns(true);

            mockSpecificGearGenerator.Setup(g => g.GenerateNameFrom("power", "armor type", "armor name")).Returns("specific armor");
            mockSpecificGearGenerator.Setup(g => g.GeneratePrototypeFrom("power", "armor type", "specific armor")).Returns(specificArmor);
            mockSpecificGearGenerator.Setup(g => g.IsSpecific(It.Is<Item>(i => i.NameMatches("specific armor")))).Returns(true);
            mockSpecificGearGenerator.Setup(g => g.GenerateFrom(It.Is<Item>(i => i.NameMatches("specific armor")))).Returns(specificArmor);

            var armor = magicalArmorGenerator.GenerateRandom("power");
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
            mockMundaneArmorGenerator.Setup(g => g.Generate(It.Is<Item>(i => i.NameMatches(name)), false)).Returns(templateMundaneArmor);

            var armor = magicalArmorGenerator.Generate(template);
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
            mockMundaneArmorGenerator.Setup(g => g.Generate(It.Is<Item>(i => i.NameMatches(name)), false)).Returns(templateMundaneArmor);

            var armor = magicalArmorGenerator.Generate(template, true);
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

            var armor = magicalArmorGenerator.Generate(template, true);
            Assert.That(armor.Quantity, Is.EqualTo(1));
            Assert.That(armor.Name, Is.EqualTo(specificArmor.Name));
            Assert.That(armor.BaseNames, Is.EquivalentTo(specificArmor.BaseNames));
            Assert.That(armor.ItemType, Is.EqualTo(ItemTypeConstants.Armor));
            Assert.That(armor.Magic.SpecialAbilities, Is.EquivalentTo(specificArmor.Magic.SpecialAbilities));
        }

        [Test]
        public void GenerateFromName_Armor()
        {
            var tableName = string.Format(TableNameConstants.Collections.Formattable.ITEMTYPEAttributes, ItemTypeConstants.Armor);
            mockCollectionsSelector.Setup(p => p.SelectFrom(tableName, "armor name")).Returns(new[] { "attribute", "not shield attribute" });

            tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, "power", ItemTypeConstants.Armor);
            mockPercentileSelector
                .SetupSequence(s => s.SelectFrom(tableName))
                .Returns("SpecialAbility")
                .Returns("SpecialAbility")
                .Returns(9266.ToString());

            var abilities = new[] { new SpecialAbility() };
            mockSpecialAbilitiesGenerator.Setup(p => p.GenerateFor(It.IsAny<Item>(), "power", 2)).Returns(abilities);
            mockMundaneArmorGenerator.Setup(g => g.Generate(It.IsAny<Item>(), false)).Returns((Item template, bool decorate) => new Armor { Name = template.Name, BaseNames = new[] { "from mundane" } });

            var armor = magicalArmorGenerator.Generate("power", "armor name");
            Assert.That(armor.Name, Is.EqualTo("armor name"));
            Assert.That(armor.BaseNames, Contains.Item("from mundane"));
            Assert.That(armor.Magic.Bonus, Is.EqualTo(9266));
            Assert.That(armor.Magic.SpecialAbilities, Is.EquivalentTo(abilities));
        }

        [Test]
        public void GenerateFromName_Armor_WithTraits()
        {
            var tableName = string.Format(TableNameConstants.Collections.Formattable.ITEMTYPEAttributes, ItemTypeConstants.Armor);
            mockCollectionsSelector.Setup(p => p.SelectFrom(tableName, "armor name")).Returns(new[] { "attribute", "not shield attribute" });

            tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, "power", ItemTypeConstants.Armor);
            mockPercentileSelector
                .SetupSequence(s => s.SelectFrom(tableName))
                .Returns("SpecialAbility")
                .Returns("SpecialAbility")
                .Returns(9266.ToString());

            var abilities = new[] { new SpecialAbility() };
            mockSpecialAbilitiesGenerator.Setup(p => p.GenerateFor(It.IsAny<Item>(), "power", 2)).Returns(abilities);
            mockMundaneArmorGenerator
                .Setup(g => g.Generate(It.IsAny<Item>(), false))
                .Returns((Item template, bool decorate) => new Armor
                {
                    Name = template.Name,
                    BaseNames = new[] { "from mundane" },
                    Traits = template.Traits,
                    Size = "armor size"
                });

            var armor = magicalArmorGenerator.Generate("power", "armor name", "trait 1", "trait 2");
            Assert.That(armor.Name, Is.EqualTo("armor name"));
            Assert.That(armor.BaseNames, Contains.Item("from mundane"));
            Assert.That(armor.Magic.Bonus, Is.EqualTo(9266));
            Assert.That(armor.Magic.SpecialAbilities, Is.EquivalentTo(abilities));
            Assert.That(armor.Traits, Has.Count.EqualTo(3)
                .And.Contains("trait 1")
                .And.Contains("trait 2")
                .And.Contains(TraitConstants.Masterwork));
            Assert.That(armor, Is.InstanceOf<Armor>());
            Assert.That((armor as Armor).Size, Is.EqualTo("armor size"));
        }

        [Test]
        public void GenerateFromName_Shield()
        {
            var tableName = string.Format(TableNameConstants.Collections.Formattable.ITEMTYPEAttributes, ItemTypeConstants.Armor);
            mockCollectionsSelector.Setup(p => p.SelectFrom(tableName, "shield name")).Returns(new[] { "attribute", AttributeConstants.Shield });

            tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, "power", ItemTypeConstants.Armor);
            mockPercentileSelector
                .SetupSequence(s => s.SelectFrom(tableName))
                .Returns("SpecialAbility")
                .Returns("SpecialAbility")
                .Returns(9266.ToString());

            var abilities = new[] { new SpecialAbility() };
            mockSpecialAbilitiesGenerator.Setup(p => p.GenerateFor(It.IsAny<Item>(), "power", 2)).Returns(abilities);
            mockMundaneArmorGenerator.Setup(g => g.Generate(It.IsAny<Item>(), false)).Returns((Item template, bool decorate) => new Armor { Name = template.Name, BaseNames = new[] { "from mundane" } });

            var armor = magicalArmorGenerator.Generate("power", "shield name");
            Assert.That(armor.Name, Is.EqualTo("shield name"));
            Assert.That(armor.BaseNames, Contains.Item("from mundane"));
            Assert.That(armor.Magic.Bonus, Is.EqualTo(9266));
            Assert.That(armor.Magic.SpecialAbilities, Is.EquivalentTo(abilities));
        }

        [Test]
        public void GenerateFromName_Shield_WithTraits()
        {
            var tableName = string.Format(TableNameConstants.Collections.Formattable.ITEMTYPEAttributes, ItemTypeConstants.Armor);
            mockCollectionsSelector.Setup(p => p.SelectFrom(tableName, "shield name")).Returns(new[] { "attribute", AttributeConstants.Shield });

            tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, "power", ItemTypeConstants.Armor);
            mockPercentileSelector
                .SetupSequence(s => s.SelectFrom(tableName))
                .Returns("SpecialAbility")
                .Returns("SpecialAbility")
                .Returns(9266.ToString());

            var abilities = new[] { new SpecialAbility() };
            mockSpecialAbilitiesGenerator.Setup(p => p.GenerateFor(It.IsAny<Item>(), "power", 2)).Returns(abilities);
            mockMundaneArmorGenerator
                .Setup(g => g.Generate(It.IsAny<Item>(), false))
                .Returns((Item template, bool decorate) => new Armor
                {
                    Name = template.Name,
                    BaseNames = new[] { "from mundane" },
                    Traits = template.Traits,
                    Size = "shield size"
                });

            var armor = magicalArmorGenerator.Generate("power", "shield name", "trait 1", "trait 2");
            Assert.That(armor.Name, Is.EqualTo("shield name"));
            Assert.That(armor.BaseNames, Contains.Item("from mundane"));
            Assert.That(armor.Magic.Bonus, Is.EqualTo(9266));
            Assert.That(armor.Magic.SpecialAbilities, Is.EquivalentTo(abilities));
            Assert.That(armor.Traits, Has.Count.EqualTo(3)
                .And.Contains("trait 1")
                .And.Contains("trait 2")
                .And.Contains(TraitConstants.Masterwork));
            Assert.That(armor, Is.InstanceOf<Armor>());
            Assert.That((armor as Armor).Size, Is.EqualTo("shield size"));
        }

        [Test]
        public void GenerateSpecificFromName_Armor()
        {
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, "power", ItemTypeConstants.Armor);
            mockPercentileSelector
                .SetupSequence(s => s.SelectFrom(tableName))
                .Returns("SpecialAbility")
                .Returns("SpecialAbility")
                .Returns(ItemTypeConstants.Armor);

            tableName = string.Format(TableNameConstants.Percentiles.Formattable.ARMORTYPETypes, "wrong armor type");
            mockPercentileSelector.Setup(p => p.SelectFrom(tableName)).Returns("wrong armor name");

            tableName = string.Format(TableNameConstants.Percentiles.Formattable.ARMORTYPETypes, "other armor type");
            mockPercentileSelector.Setup(p => p.SelectFrom(tableName)).Returns("other armor name");

            mockMundaneArmorGenerator.Setup(g => g.Generate(It.IsAny<Item>(), false)).Returns((Item template, bool decorate) => new Armor { Name = template.Name, BaseNames = new[] { "from mundane" } });

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

            mockSpecificGearGenerator.Setup(g => g.IsSpecific("power", ItemTypeConstants.Armor, "specific armor")).Returns(true);
            mockSpecificGearGenerator.Setup(g => g.GeneratePrototypeFrom("power", ItemTypeConstants.Armor, "specific armor")).Returns(specificArmor);
            mockSpecificGearGenerator.Setup(g => g.GenerateFrom(It.Is<Item>(i => i.Name == specificArmor.Name))).Returns(specificArmor);
            mockSpecificGearGenerator.Setup(g => g.IsSpecific(It.Is<Item>(i => i.Name == specificArmor.Name))).Returns(true);

            var armor = magicalArmorGenerator.Generate("power", "specific armor");
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
        public void GenerateSpecificFromName_Armor_WithTraits()
        {
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, "power", ItemTypeConstants.Armor);
            mockPercentileSelector
                .SetupSequence(s => s.SelectFrom(tableName))
                .Returns("SpecialAbility")
                .Returns("SpecialAbility")
                .Returns(ItemTypeConstants.Armor);

            tableName = string.Format(TableNameConstants.Percentiles.Formattable.ARMORTYPETypes, "wrong armor type");
            mockPercentileSelector.Setup(p => p.SelectFrom(tableName)).Returns("wrong armor name");

            tableName = string.Format(TableNameConstants.Percentiles.Formattable.ARMORTYPETypes, "other armor type");
            mockPercentileSelector.Setup(p => p.SelectFrom(tableName)).Returns("other armor name");

            mockMundaneArmorGenerator.Setup(g => g.Generate(It.IsAny<Item>(), false)).Returns((Item template, bool decorate) => new Armor { Name = template.Name, BaseNames = new[] { "from mundane" } });

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
            specificArmor.Traits.Add("trait 1");
            specificArmor.Traits.Add("trait 2");
            specificArmor.Size = "specific armor size";

            mockSpecificGearGenerator.Setup(g => g.IsSpecific("power", ItemTypeConstants.Armor, "specific armor")).Returns(true);
            mockSpecificGearGenerator.Setup(g => g.GeneratePrototypeFrom("power", ItemTypeConstants.Armor, "specific armor", "trait 1", "trait 2")).Returns(specificArmor);
            mockSpecificGearGenerator.Setup(g => g.GenerateFrom(It.Is<Item>(i => i.Name == specificArmor.Name))).Returns(specificArmor);
            mockSpecificGearGenerator.Setup(g => g.IsSpecific(It.Is<Item>(i => i.Name == specificArmor.Name))).Returns(true);

            var armor = magicalArmorGenerator.Generate("power", "specific armor", "trait 1", "trait 2");
            Assert.That(armor, Is.EqualTo(specificArmor));
            Assert.That(armor.Name, Is.EqualTo("specific armor"));
            Assert.That(armor.BaseNames, Is.EqualTo(baseNames));
            Assert.That(armor.ItemType, Is.EqualTo(ItemTypeConstants.Armor));
            Assert.That(armor.Quantity, Is.EqualTo(1));
            Assert.That(armor.Magic.Bonus, Is.EqualTo(42));
            Assert.That(armor.Attributes, Is.EqualTo(attributes));
            Assert.That(armor.Magic.SpecialAbilities, Is.EquivalentTo(abilities));
            Assert.That(armor.Traits, Has.Count.EqualTo(2).And.Contains("trait 1").And.Contains("trait 2"));
            Assert.That(armor, Is.InstanceOf<Armor>());
            Assert.That((armor as Armor).Size, Is.EqualTo("specific armor size"));
        }

        [Test]
        public void GenerateSpecificFromName_Shield()
        {
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, "power", ItemTypeConstants.Armor);
            mockPercentileSelector
                .SetupSequence(s => s.SelectFrom(tableName))
                .Returns("SpecialAbility")
                .Returns("SpecialAbility")
                .Returns(ItemTypeConstants.Armor);

            tableName = string.Format(TableNameConstants.Percentiles.Formattable.ARMORTYPETypes, "wrong armor type");
            mockPercentileSelector.Setup(p => p.SelectFrom(tableName)).Returns("wrong armor name");

            tableName = string.Format(TableNameConstants.Percentiles.Formattable.ARMORTYPETypes, "other armor type");
            mockPercentileSelector.Setup(p => p.SelectFrom(tableName)).Returns("other armor name");

            mockMundaneArmorGenerator.Setup(g => g.Generate(It.IsAny<Item>(), false)).Returns((Item template, bool decorate) => new Armor { Name = template.Name, BaseNames = new[] { "from mundane" } });

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

            mockSpecificGearGenerator.Setup(g => g.IsSpecific("power", AttributeConstants.Shield, "specific shield")).Returns(true);
            mockSpecificGearGenerator.Setup(g => g.GeneratePrototypeFrom("power", AttributeConstants.Shield, "specific shield")).Returns(specificArmor);
            mockSpecificGearGenerator.Setup(g => g.GenerateFrom(It.Is<Item>(i => i.Name == specificArmor.Name))).Returns(specificArmor);
            mockSpecificGearGenerator.Setup(g => g.IsSpecific(It.Is<Item>(i => i.Name == specificArmor.Name))).Returns(true);

            var armor = magicalArmorGenerator.Generate("power", "specific shield");
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
        public void GenerateSpecificFromName_Shield_WithTraits()
        {
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, "power", ItemTypeConstants.Armor);
            mockPercentileSelector
                .SetupSequence(s => s.SelectFrom(tableName))
                .Returns("SpecialAbility")
                .Returns("SpecialAbility")
                .Returns(ItemTypeConstants.Armor);

            tableName = string.Format(TableNameConstants.Percentiles.Formattable.ARMORTYPETypes, "wrong armor type");
            mockPercentileSelector.Setup(p => p.SelectFrom(tableName)).Returns("wrong armor name");

            tableName = string.Format(TableNameConstants.Percentiles.Formattable.ARMORTYPETypes, "other armor type");
            mockPercentileSelector.Setup(p => p.SelectFrom(tableName)).Returns("other armor name");

            mockMundaneArmorGenerator.Setup(g => g.Generate(It.IsAny<Item>(), false)).Returns((Item template, bool decorate) => new Armor { Name = template.Name, BaseNames = new[] { "from mundane" } });

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
            specificArmor.Traits.Add("trait 1");
            specificArmor.Traits.Add("trait 2");
            specificArmor.Size = "specific shield size";

            mockSpecificGearGenerator.Setup(g => g.IsSpecific("power", AttributeConstants.Shield, "specific shield")).Returns(true);
            mockSpecificGearGenerator.Setup(g => g.GeneratePrototypeFrom("power", AttributeConstants.Shield, "specific shield", "trait 1", "trait 2")).Returns(specificArmor);
            mockSpecificGearGenerator.Setup(g => g.GenerateFrom(It.Is<Item>(i => i.Name == specificArmor.Name))).Returns(specificArmor);
            mockSpecificGearGenerator.Setup(g => g.IsSpecific(It.Is<Item>(i => i.Name == specificArmor.Name))).Returns(true);

            var armor = magicalArmorGenerator.Generate("power", "specific shield", "trait 1", "trait 2");
            Assert.That(armor, Is.EqualTo(specificArmor));
            Assert.That(armor.Name, Is.EqualTo("specific shield"));
            Assert.That(armor.BaseNames, Is.EqualTo(baseNames));
            Assert.That(armor.ItemType, Is.EqualTo(ItemTypeConstants.Armor));
            Assert.That(armor.Quantity, Is.EqualTo(1));
            Assert.That(armor.Magic.Bonus, Is.EqualTo(42));
            Assert.That(armor.Attributes, Is.EqualTo(attributes));
            Assert.That(armor.Magic.SpecialAbilities, Is.EquivalentTo(abilities));
            Assert.That(armor.Traits, Has.Count.EqualTo(2).And.Contains("trait 1").And.Contains("trait 2"));
            Assert.That(armor, Is.InstanceOf<Armor>());
            Assert.That((armor as Armor).Size, Is.EqualTo("specific shield size"));
        }

        [Test]
        public void GenerateSpecificFromBaseName_Armor()
        {
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, "power", ItemTypeConstants.Armor);
            mockPercentileSelector
                .SetupSequence(s => s.SelectFrom(tableName))
                .Returns("SpecialAbility")
                .Returns("SpecialAbility")
                .Returns(ItemTypeConstants.Armor);

            tableName = string.Format(TableNameConstants.Collections.Formattable.ITEMTYPEAttributes, ItemTypeConstants.Armor);
            mockCollectionsSelector.Setup(p => p.SelectFrom(tableName, "armor name")).Returns(new[] { "attribute", "other attribute" });

            tableName = string.Format(TableNameConstants.Percentiles.Formattable.ARMORTYPETypes, ItemTypeConstants.Armor);
            mockPercentileSelector.Setup(p => p.SelectFrom(tableName)).Returns("armor name");

            var baseNames = new[] { "from mundane", "base name" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, "armor name")).Returns(baseNames);
            mockMundaneArmorGenerator.Setup(g => g.Generate(It.IsAny<Item>(), false)).Returns((Item template, bool decorate) => new Armor { Name = template.Name, BaseNames = baseNames });

            var attributes = new[] { "type 1", "type 2" };
            var abilities = new[] { new SpecialAbility(), new SpecialAbility() };

            var specificArmor = new Armor();
            specificArmor.Name = "specific armor";
            specificArmor.BaseNames = baseNames;
            specificArmor.ItemType = ItemTypeConstants.Armor;
            specificArmor.Attributes = attributes;
            specificArmor.Magic.Bonus = 42;
            specificArmor.Magic.SpecialAbilities = abilities;

            mockSpecificGearGenerator.Setup(g => g.CanBeSpecific("power", ItemTypeConstants.Armor, "base name")).Returns(true);
            mockSpecificGearGenerator.Setup(g => g.IsSpecific("power", ItemTypeConstants.Armor, "specific armor")).Returns(true);
            mockSpecificGearGenerator.Setup(g => g.GenerateNameFrom("power", ItemTypeConstants.Armor, "base name")).Returns("specific armor");
            mockSpecificGearGenerator.Setup(g => g.GeneratePrototypeFrom("power", ItemTypeConstants.Armor, "specific armor")).Returns(specificArmor);
            mockSpecificGearGenerator.Setup(g => g.IsSpecific(It.Is<Item>(i => i.Name == specificArmor.Name))).Returns(true);
            mockSpecificGearGenerator.Setup(g => g.GenerateFrom(It.Is<Item>(i => i.Name == specificArmor.Name))).Returns(specificArmor);

            var armor = magicalArmorGenerator.Generate("power", "base name");
            Assert.That(armor.Name, Is.EqualTo("specific armor"));
            Assert.That(armor.BaseNames, Contains.Item("from mundane").And.Contains("base name"));
            Assert.That(armor.Magic.Bonus, Is.EqualTo(42));
            Assert.That(armor.Magic.SpecialAbilities, Is.EquivalentTo(abilities));
        }

        [Test]
        public void GenerateSpecificFromBaseName_Shield()
        {
            var tableName = string.Format(TableNameConstants.Collections.Formattable.ITEMTYPEAttributes, ItemTypeConstants.Armor);
            mockCollectionsSelector.Setup(p => p.SelectFrom(tableName, "base name")).Returns(new[] { "attribute", AttributeConstants.Shield });

            tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, "power", ItemTypeConstants.Armor);
            mockPercentileSelector
                .SetupSequence(s => s.SelectFrom(tableName))
                .Returns("SpecialAbility")
                .Returns("SpecialAbility")
                .Returns(ItemTypeConstants.Armor);

            tableName = string.Format(TableNameConstants.Percentiles.Formattable.ARMORTYPETypes, AttributeConstants.Shield);
            mockPercentileSelector.Setup(p => p.SelectFrom(tableName)).Returns("armor name");

            var baseNames = new[] { "from mundane", "base name" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, "armor name")).Returns(baseNames);
            mockMundaneArmorGenerator.Setup(g => g.Generate(It.IsAny<Item>(), false)).Returns((Item template, bool decorate) => new Armor { Name = template.Name, BaseNames = baseNames });

            var attributes = new[] { "type 1", "type 2" };
            var abilities = new[] { new SpecialAbility(), new SpecialAbility() };

            var specificArmor = new Armor();
            specificArmor.Name = "specific shield";
            specificArmor.BaseNames = baseNames;
            specificArmor.ItemType = ItemTypeConstants.Armor;
            specificArmor.Attributes = attributes;
            specificArmor.Magic.Bonus = 42;
            specificArmor.Magic.SpecialAbilities = abilities;

            mockSpecificGearGenerator.Setup(g => g.CanBeSpecific("power", AttributeConstants.Shield, "base name")).Returns(true);
            mockSpecificGearGenerator.Setup(g => g.IsSpecific("power", AttributeConstants.Shield, "specific shield")).Returns(true);
            mockSpecificGearGenerator.Setup(g => g.GenerateNameFrom("power", AttributeConstants.Shield, "base name")).Returns("specific shield");
            mockSpecificGearGenerator.Setup(g => g.GeneratePrototypeFrom("power", AttributeConstants.Shield, "specific shield")).Returns(specificArmor);
            mockSpecificGearGenerator.Setup(g => g.IsSpecific(It.Is<Item>(i => i.Name == specificArmor.Name))).Returns(true);
            mockSpecificGearGenerator.Setup(g => g.GenerateFrom(It.Is<Item>(i => i.Name == specificArmor.Name))).Returns(specificArmor);

            var armor = magicalArmorGenerator.Generate("power", "base name");
            Assert.That(armor.Name, Is.EqualTo("specific shield"));
            Assert.That(armor.BaseNames, Contains.Item("from mundane").And.Contains("base name"));
            Assert.That(armor.Magic.Bonus, Is.EqualTo(42));
            Assert.That(armor.Magic.SpecialAbilities, Is.EquivalentTo(abilities));
        }

        [Test]
        public void IsItemOfPower_ReturnsTrue_SpecificArmor()
        {
            mockSpecificGearGenerator.Setup(g => g.IsSpecific(ItemTypeConstants.Armor, "item name")).Returns(true);
            mockSpecificGearGenerator.Setup(g => g.IsSpecific("power", ItemTypeConstants.Armor, "item name")).Returns(true);

            var isOfPower = magicalArmorGenerator.IsItemOfPower("item name", "power");
            Assert.That(isOfPower, Is.True);
        }

        [Test]
        public void IsItemOfPower_ReturnsFalse_SpecificArmor()
        {
            mockSpecificGearGenerator.Setup(g => g.IsSpecific(ItemTypeConstants.Armor, "item name")).Returns(true);
            mockSpecificGearGenerator.Setup(g => g.IsSpecific("power", ItemTypeConstants.Armor, "item name")).Returns(false);

            var isOfPower = magicalArmorGenerator.IsItemOfPower("item name", "power");
            Assert.That(isOfPower, Is.False);
        }

        [Test]
        public void IsItemOfPower_ReturnsTrue_SpecificShield()
        {
            mockSpecificGearGenerator.Setup(g => g.IsSpecific(AttributeConstants.Shield, "item name")).Returns(true);
            mockSpecificGearGenerator.Setup(g => g.IsSpecific("power", AttributeConstants.Shield, "item name")).Returns(true);

            var isOfPower = magicalArmorGenerator.IsItemOfPower("item name", "power");
            Assert.That(isOfPower, Is.True);
        }

        [Test]
        public void BUG_IsItemOfPower_ReturnsTrue_SpecificShield_AlsoArmor()
        {
            mockSpecificGearGenerator.Setup(g => g.IsSpecific(ItemTypeConstants.Armor, "item name")).Returns(true);
            mockSpecificGearGenerator.Setup(g => g.IsSpecific("power", ItemTypeConstants.Armor, "item name")).Returns(false);
            mockSpecificGearGenerator.Setup(g => g.IsSpecific(AttributeConstants.Shield, "item name")).Returns(true);
            mockSpecificGearGenerator.Setup(g => g.IsSpecific("power", AttributeConstants.Shield, "item name")).Returns(true);

            var isOfPower = magicalArmorGenerator.IsItemOfPower("item name", "power");
            Assert.That(isOfPower, Is.True);
        }

        [Test]
        public void IsItemOfPower_ReturnsFalse_SpecificShield()
        {
            mockSpecificGearGenerator.Setup(g => g.IsSpecific(AttributeConstants.Shield, "item name")).Returns(true);
            mockSpecificGearGenerator.Setup(g => g.IsSpecific("power", AttributeConstants.Shield, "item name")).Returns(false);

            var isOfPower = magicalArmorGenerator.IsItemOfPower("item name", "power");
            Assert.That(isOfPower, Is.False);
        }

        [Test]
        public void IsItemOfPower_ReturnsTrue_NotSpecific()
        {
            mockSpecificGearGenerator.Setup(g => g.IsSpecific(ItemTypeConstants.Armor, "item name")).Returns(false);
            mockSpecificGearGenerator.Setup(g => g.IsSpecific(AttributeConstants.Shield, "item name")).Returns(false);

            var tableName = string.Format(TableNameConstants.Collections.Formattable.ITEMTYPEAttributes, ItemTypeConstants.Armor);
            mockCollectionsSelector
                .Setup(s => s.IsCollection(tableName, "item name"))
                .Returns(true);

            var isOfPower = magicalArmorGenerator.IsItemOfPower("item name", "power");
            Assert.That(isOfPower, Is.True);
        }

        [Test]
        public void IsItemOfPower_ReturnsFalse_NotSpecific()
        {
            mockSpecificGearGenerator.Setup(g => g.IsSpecific(ItemTypeConstants.Armor, "item name")).Returns(false);
            mockSpecificGearGenerator.Setup(g => g.IsSpecific(AttributeConstants.Shield, "item name")).Returns(false);

            var tableName = string.Format(TableNameConstants.Collections.Formattable.ITEMTYPEAttributes, ItemTypeConstants.Armor);
            mockCollectionsSelector
                .Setup(s => s.IsCollection(tableName, "item name"))
                .Returns(false);

            var isOfPower = magicalArmorGenerator.IsItemOfPower("item name", "power");
            Assert.That(isOfPower, Is.False);
        }
    }
}