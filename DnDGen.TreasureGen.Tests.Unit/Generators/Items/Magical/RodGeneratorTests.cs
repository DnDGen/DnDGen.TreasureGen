using DnDGen.Infrastructure.Factories;
using DnDGen.Infrastructure.Models;
using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.Infrastructure.Selectors.Percentiles;
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
    public class RodGeneratorTests
    {
        private MagicalItemGenerator rodGenerator;
        private Mock<IPercentileTypeAndAmountSelector> mockTypeAndAmountPercentileSelector;
        private Mock<ICollectionSelector> mockCollectionsSelector;
        private Mock<IChargesGenerator> mockChargesGenerator;
        private Mock<ITreasurePercentileSelector> mockPercentileSelector;
        private TypeAndAmountDataSelection selection;
        private string power;
        private ItemVerifier itemVerifier;
        private Mock<ISpecialAbilitiesGenerator> mockSpecialAbilitiesGenerator;
        private Mock<MundaneItemGenerator> mockMundaneWeaponGenerator;

        [SetUp]
        public void Setup()
        {
            mockTypeAndAmountPercentileSelector = new Mock<IPercentileTypeAndAmountSelector>();
            mockCollectionsSelector = new Mock<ICollectionSelector>();
            mockChargesGenerator = new Mock<IChargesGenerator>();
            selection = new TypeAndAmountDataSelection();
            mockPercentileSelector = new Mock<ITreasurePercentileSelector>();
            mockSpecialAbilitiesGenerator = new Mock<ISpecialAbilitiesGenerator>();
            mockMundaneWeaponGenerator = new Mock<MundaneItemGenerator>();
            var mockJustInTimeFactory = new Mock<JustInTimeFactory>();

            mockJustInTimeFactory.Setup(f => f.Build<MundaneItemGenerator>(ItemTypeConstants.Weapon)).Returns(mockMundaneWeaponGenerator.Object);

            rodGenerator = new RodGenerator(mockTypeAndAmountPercentileSelector.Object,
                mockCollectionsSelector.Object,
                mockChargesGenerator.Object,
                mockPercentileSelector.Object,
                mockSpecialAbilitiesGenerator.Object,
                mockJustInTimeFactory.Object);
            itemVerifier = new ItemVerifier();

            selection.Type = "rod of ability";
            selection.AmountAsDouble = 9266;
            power = "power";

            var tableName = TableNameConstants.Percentiles.POWERITEMTYPEs(power, ItemTypeConstants.Rod);
            mockTypeAndAmountPercentileSelector.Setup(s => s.SelectFrom(Config.Name, tableName)).Returns(selection);

            mockSpecialAbilitiesGenerator.Setup(p => p.ApplyAbilitiesToWeapon(It.IsAny<Weapon>())).Returns((Weapon w) => w);
        }

        [Test]
        public void GenerateRandom_GenerateRod()
        {
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, ItemTypeConstants.Rod))
                .Returns(new[] { "wrong power", power, "other power" });

            var rod = rodGenerator.GenerateRandom(power);
            Assert.That(rod.ItemType, Is.EqualTo(ItemTypeConstants.Rod));
            Assert.That(rod.IsMagical, Is.True);
            Assert.That(rod.Name, Is.EqualTo(selection.Type));
            Assert.That(rod.Magic.Bonus, Is.EqualTo(selection.Amount));
            Assert.That(rod, Is.Not.InstanceOf<Weapon>());
        }

        [Test]
        public void GenerateRandom_GetBaseNames()
        {
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, ItemTypeConstants.Rod))
                .Returns(new[] { "wrong power", power, "other power" });

            var baseNames = new[] { "base name", "other base name" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.ItemGroups, selection.Type)).Returns(baseNames);

            var rod = rodGenerator.GenerateRandom(power);
            Assert.That(rod.BaseNames, Is.EqualTo(baseNames));
        }

        [Test]
        public void GenerateRandom_MinorPowerIsAdjusted()
        {
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, ItemTypeConstants.Rod))
                .Returns([power, "other power", "wrong power"]);

            var rod = rodGenerator.GenerateRandom(PowerConstants.Minor);
            Assert.That(rod.ItemType, Is.EqualTo(ItemTypeConstants.Rod));
            Assert.That(rod.IsMagical, Is.True);
            Assert.That(rod.Name, Is.EqualTo(selection.Type));
            Assert.That(rod.Magic.Bonus, Is.EqualTo(selection.Amount));
            Assert.That(rod, Is.Not.InstanceOf<Weapon>());
        }

        [Test]
        public void GenerateRandom_GetAttributesFromSelector()
        {
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, ItemTypeConstants.Rod))
                .Returns(["wrong power", power, "other power"]);

            var attributes = new[] { "attribute 1", "attribute 2" };
            var tableName = TableNameConstants.Collections.ITEMTYPEAttributes(ItemTypeConstants.Rod);
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, tableName, selection.Type)).Returns(attributes);

            var rod = rodGenerator.GenerateRandom(power);
            Assert.That(rod.Attributes, Is.EqualTo(attributes));
        }

        [Test]
        public void GenerateRandom_GetChargesIfCharged()
        {
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, ItemTypeConstants.Rod))
                .Returns(["wrong power", power, "other power"]);

            var attributes = new[] { AttributeConstants.Charged };
            var tableName = TableNameConstants.Collections.ITEMTYPEAttributes(ItemTypeConstants.Rod);
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, tableName, selection.Type)).Returns(attributes);
            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Rod, selection.Type)).Returns(90210);

            var rod = rodGenerator.GenerateRandom(power);
            Assert.That(rod.Magic.Charges, Is.EqualTo(90210));
        }

        [Test]
        public void GenerateRandom_DoNotGetChargesIfNotCharged()
        {
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, ItemTypeConstants.Rod))
                .Returns(new[] { "wrong power", power, "other power" });

            var attributes = new[] { "new attribute" };
            var tableName = TableNameConstants.Collections.ITEMTYPEAttributes(ItemTypeConstants.Rod);
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, tableName, selection.Type)).Returns(attributes);
            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Rod, selection.Type)).Returns(90210);

            var rod = rodGenerator.GenerateRandom(power);
            Assert.That(rod.Magic.Charges, Is.EqualTo(0));
        }

        [Test]
        public void GenerateRandom_RodOfAbsorptionContainsLevelsIfSelectorSaysSo()
        {
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, ItemTypeConstants.Rod))
                .Returns(new[] { "wrong power", power, "other power" });

            selection.Type = RodConstants.Absorption;
            var attributes = new[] { AttributeConstants.Charged };
            var tableName = TableNameConstants.Collections.ITEMTYPEAttributes(ItemTypeConstants.Rod);
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, tableName, selection.Type)).Returns(attributes);
            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Rod, selection.Type)).Returns(42);
            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Rod, RodConstants.Absorption_Full)).Returns(50);
            mockPercentileSelector.Setup(s => s.SelectFrom<bool>(Config.Name, TableNameConstants.Percentiles.RodOfAbsorptionContainsSpellLevels)).Returns(true);

            var rod = rodGenerator.GenerateRandom(power);
            Assert.That(rod.Magic.Charges, Is.EqualTo(42));
            Assert.That(rod.Contents, Contains.Item("4 spell levels"));
            Assert.That(rod.Contents.Count, Is.EqualTo(1));
        }

        [Test]
        public void GenerateRandom_RodOfAbsorptionDoesNotContainLevelsIfSelectorSaysSo()
        {
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, ItemTypeConstants.Rod))
                .Returns(new[] { "wrong power", power, "other power" });

            selection.Type = RodConstants.Absorption;
            var attributes = new[] { AttributeConstants.Charged };
            var tableName = TableNameConstants.Collections.ITEMTYPEAttributes(ItemTypeConstants.Rod);
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, tableName, selection.Type)).Returns(attributes);
            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Rod, selection.Type)).Returns(42);
            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Rod, RodConstants.Absorption_Full)).Returns(50);
            mockPercentileSelector.Setup(s => s.SelectFrom<bool>(Config.Name, TableNameConstants.Percentiles.RodOfAbsorptionContainsSpellLevels)).Returns(false);

            var rod = rodGenerator.GenerateRandom(power);
            Assert.That(rod.Magic.Charges, Is.EqualTo(42));
            Assert.That(rod.Contents, Is.Empty);
        }

        [Test]
        public void GenerateRandom_GetRodThatIsAlsoWeapon()
        {
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, ItemTypeConstants.Rod))
                .Returns(new[] { "wrong power", power, "other power" });

            var baseNames = new[] { "base name", "other base name", WeaponConstants.LightMace };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.ItemGroups, selection.Type)).Returns(baseNames);

            var attributes = new[] { "attribute 1", "attribute 2" };
            var tableName = TableNameConstants.Collections.ITEMTYPEAttributes(ItemTypeConstants.Rod);
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, tableName, selection.Type)).Returns(attributes);

            var mundaneWeapon = itemVerifier.CreateRandomWeaponTemplate(WeaponConstants.LightMace);
            mundaneWeapon.Traits.Clear();

            mockMundaneWeaponGenerator
                .Setup(g => g.Generate(WeaponConstants.LightMace))
                .Returns(mundaneWeapon);

            var rod = rodGenerator.GenerateRandom(power);
            Assert.That(rod.ItemType, Is.EqualTo(ItemTypeConstants.Rod));
            Assert.That(rod.IsMagical, Is.True);
            Assert.That(rod.Name, Is.EqualTo(selection.Type));
            Assert.That(rod.Magic.Bonus, Is.EqualTo(selection.Amount));
            Assert.That(rod.BaseNames, Is.EqualTo(baseNames));
            Assert.That(rod, Is.InstanceOf<Weapon>());

            var weapon = rod as Weapon;
            Assert.That(weapon.Attributes, Is.SupersetOf(mundaneWeapon.Attributes));
            Assert.That(weapon.CriticalMultiplier, Is.EqualTo(mundaneWeapon.CriticalMultiplier));
            Assert.That(weapon.Damages, Is.EqualTo(mundaneWeapon.Damages));
            Assert.That(weapon.CriticalDamages, Is.EqualTo(mundaneWeapon.CriticalDamages));
            Assert.That(weapon.Size, Is.EqualTo(mundaneWeapon.Size));
            Assert.That(weapon.ThreatRangeSummary, Is.EqualTo(mundaneWeapon.ThreatRangeSummary));
            Assert.That(weapon.Traits, Contains.Item(TraitConstants.Masterwork));
            Assert.That(weapon.IsDoubleWeapon, Is.False);
            Assert.That(weapon.SecondaryMagicBonus, Is.Zero);
            Assert.That(weapon.SecondaryHasAbilities, Is.False);
        }

        [Test]
        public void GenerateRandom_GetRodThatIsAlsoDoubleWeapon()
        {
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, ItemTypeConstants.Rod))
                .Returns(new[] { "wrong power", power, "other power" });

            var baseNames = new[] { "base name", "other base name", WeaponConstants.DireFlail };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.ItemGroups, selection.Type)).Returns(baseNames);

            var attributes = new[] { "attribute 1", "attribute 2" };
            var tableName = TableNameConstants.Collections.ITEMTYPEAttributes(ItemTypeConstants.Rod);
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, tableName, selection.Type)).Returns(attributes);

            var mundaneWeapon = itemVerifier.CreateRandomWeaponTemplate(WeaponConstants.DireFlail);
            mundaneWeapon.Traits.Clear();
            mundaneWeapon.Attributes = mundaneWeapon.Attributes.Union(new[] { AttributeConstants.DoubleWeapon });

            mockMundaneWeaponGenerator
                .Setup(g => g.Generate(WeaponConstants.DireFlail))
                .Returns(mundaneWeapon);

            var rod = rodGenerator.GenerateRandom(power);
            Assert.That(rod.ItemType, Is.EqualTo(ItemTypeConstants.Rod));
            Assert.That(rod.IsMagical, Is.True);
            Assert.That(rod.Name, Is.EqualTo(selection.Type));
            Assert.That(rod.Magic.Bonus, Is.EqualTo(selection.Amount));
            Assert.That(rod.BaseNames, Is.EqualTo(baseNames));
            Assert.That(rod, Is.InstanceOf<Weapon>());

            var weapon = rod as Weapon;
            Assert.That(weapon.Attributes, Is.SupersetOf(mundaneWeapon.Attributes));
            Assert.That(weapon.CriticalMultiplier, Is.EqualTo(mundaneWeapon.CriticalMultiplier));
            Assert.That(weapon.Damages, Is.EqualTo(mundaneWeapon.Damages));
            Assert.That(weapon.CriticalDamages, Is.EqualTo(mundaneWeapon.CriticalDamages));
            Assert.That(weapon.Size, Is.EqualTo(mundaneWeapon.Size));
            Assert.That(weapon.ThreatRangeSummary, Is.EqualTo(mundaneWeapon.ThreatRangeSummary));
            Assert.That(weapon.Traits, Contains.Item(TraitConstants.Masterwork));
            Assert.That(weapon.IsDoubleWeapon, Is.True);
            Assert.That(weapon.SecondaryMagicBonus, Is.EqualTo(selection.Amount));
            Assert.That(weapon.SecondaryHasAbilities, Is.True);
        }

        [Test]
        public void GenerateCustomRod()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);
            var specialAbilityNames = template.Magic.SpecialAbilities.Select(a => a.Name);

            var attributes = new[] { "attribute 1", "attribute 2" };
            var tableName = TableNameConstants.Collections.ITEMTYPEAttributes(ItemTypeConstants.Rod);
            mockCollectionsSelector.Setup(p => p.SelectFrom(Config.Name, tableName, name)).Returns(attributes);

            var abilities = new[]
            {
                new SpecialAbility { Name = specialAbilityNames.First() },
                new SpecialAbility { Name = specialAbilityNames.Last() }
            };

            mockSpecialAbilitiesGenerator.Setup(p => p.GenerateFor(template.Magic.SpecialAbilities, string.Empty)).Returns(abilities);

            var baseNames = new[] { "base name", "other base name" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.ItemGroups, name)).Returns(baseNames);

            tableName = TableNameConstants.Percentiles.POWERITEMTYPEs(PowerConstants.Medium, ItemTypeConstants.Rod);
            mockTypeAndAmountPercentileSelector.Setup(s => s.SelectAllFrom(Config.Name, tableName)).Returns(
            [
                new TypeAndAmountDataSelection { AmountAsDouble = 9266, Type = "other rod" },
                new TypeAndAmountDataSelection { AmountAsDouble = 0, Type = name },
            ]);

            var rod = rodGenerator.Generate(template);
            itemVerifier.AssertMagicalItemFromTemplate(rod, template);
            Assert.That(rod.Attributes, Is.EquivalentTo(attributes));
            Assert.That(rod.IsMagical, Is.True);
            Assert.That(rod.ItemType, Is.EqualTo(ItemTypeConstants.Rod));
            Assert.That(rod.Quantity, Is.EqualTo(1));
            Assert.That(rod.BaseNames, Is.EqualTo(baseNames));
            Assert.That(rod.Magic.Bonus, Is.EqualTo(0));
        }

        [Test]
        public void GenerateCustomRodWithBonus()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);
            var specialAbilityNames = template.Magic.SpecialAbilities.Select(a => a.Name);

            var attributes = new[] { "attribute 1", "attribute 2" };
            var tableName = TableNameConstants.Collections.ITEMTYPEAttributes(ItemTypeConstants.Rod);
            mockCollectionsSelector.Setup(p => p.SelectFrom(Config.Name, tableName, name)).Returns(attributes);

            var abilities = new[]
            {
                new SpecialAbility { Name = specialAbilityNames.First() },
                new SpecialAbility { Name = specialAbilityNames.Last() }
            };

            mockSpecialAbilitiesGenerator.Setup(p => p.GenerateFor(template.Magic.SpecialAbilities, string.Empty)).Returns(abilities);

            var baseNames = new[] { "base name", "other base name" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.ItemGroups, name)).Returns(baseNames);
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.ItemGroups, name)).Returns(baseNames);

            tableName = TableNameConstants.Percentiles.POWERITEMTYPEs(PowerConstants.Medium, ItemTypeConstants.Rod);
            mockTypeAndAmountPercentileSelector.Setup(s => s.SelectAllFrom(Config.Name, tableName)).Returns(
            [
                new TypeAndAmountDataSelection { AmountAsDouble = 90210, Type = "other rod" },
                new TypeAndAmountDataSelection { AmountAsDouble = 9266, Type = name },
            ]);

            var rod = rodGenerator.Generate(template);
            itemVerifier.AssertMagicalItemFromTemplate(rod, template);
            Assert.That(rod.Attributes, Is.EquivalentTo(attributes));
            Assert.That(rod.IsMagical, Is.True);
            Assert.That(rod.ItemType, Is.EqualTo(ItemTypeConstants.Rod));
            Assert.That(rod.Quantity, Is.EqualTo(1));
            Assert.That(rod.BaseNames, Is.EqualTo(baseNames));
            Assert.That(rod.Magic.Bonus, Is.EqualTo(9266));
        }

        [Test]
        public void GenerateCustomRodThatCanBeAWeapon()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);
            template.Traits.Clear();

            var specialAbilityNames = template.Magic.SpecialAbilities.Select(a => a.Name);

            var attributes = new[] { "attribute 1", "attribute 2" };
            var tableName = TableNameConstants.Collections.ITEMTYPEAttributes(ItemTypeConstants.Rod);
            mockCollectionsSelector.Setup(p => p.SelectFrom(Config.Name, tableName, name)).Returns(attributes);

            var baseNames = new[] { "base name", "other base name", WeaponConstants.Club };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.ItemGroups, name)).Returns(baseNames);

            tableName = TableNameConstants.Percentiles.POWERITEMTYPEs(PowerConstants.Medium, ItemTypeConstants.Rod);
            mockTypeAndAmountPercentileSelector.Setup(s => s.SelectAllFrom(Config.Name, tableName)).Returns(
            [
                new TypeAndAmountDataSelection { AmountAsDouble = 90210, Type = "other rod" },
                new TypeAndAmountDataSelection { AmountAsDouble = 9266, Type = name },
            ]);

            var mundaneWeapon = itemVerifier.CreateRandomWeaponTemplate(WeaponConstants.Club);
            mundaneWeapon.Traits.Clear();
            mundaneWeapon.Quantity = 1;

            mockMundaneWeaponGenerator
                .Setup(g => g.Generate(WeaponConstants.Club))
                .Returns(mundaneWeapon);

            var abilities = new[]
            {
                new SpecialAbility { Name = specialAbilityNames.First() },
                new SpecialAbility { Name = specialAbilityNames.Last() }
            };

            mockSpecialAbilitiesGenerator.Setup(p => p.GenerateFor(template.Magic.SpecialAbilities, mundaneWeapon.CriticalMultiplier)).Returns(abilities);

            var rod = rodGenerator.Generate(template);
            itemVerifier.AssertMagicalItemFromTemplate(rod, template);
            Assert.That(rod.Attributes, Is.SupersetOf(attributes));
            Assert.That(rod.IsMagical, Is.True);
            Assert.That(rod.ItemType, Is.EqualTo(ItemTypeConstants.Rod));
            Assert.That(rod.Quantity, Is.EqualTo(1));
            Assert.That(rod.Magic.SpecialAbilities, Is.EqualTo(abilities));
            Assert.That(rod.BaseNames, Is.EqualTo(baseNames));
            Assert.That(rod.Magic.Bonus, Is.EqualTo(9266));
            Assert.That(rod, Is.InstanceOf<Weapon>());

            var weapon = rod as Weapon;
            Assert.That(weapon.Attributes, Is.SupersetOf(mundaneWeapon.Attributes));
            Assert.That(weapon.CriticalMultiplier, Is.EqualTo(mundaneWeapon.CriticalMultiplier));
            Assert.That(weapon.Damages.Select(d => d.Summary), Is.EqualTo(mundaneWeapon.Damages.Select(d => d.Summary)));
            Assert.That(weapon.CriticalDamages.Select(d => d.Summary), Is.EqualTo(mundaneWeapon.CriticalDamages.Select(d => d.Summary)));
            Assert.That(weapon.Size, Is.EqualTo(mundaneWeapon.Size));
            Assert.That(weapon.ThreatRangeSummary, Is.EqualTo(mundaneWeapon.ThreatRangeSummary));
            Assert.That(weapon.Traits, Contains.Item(TraitConstants.Masterwork).And.Count.EqualTo(1));
            Assert.That(weapon.IsDoubleWeapon, Is.False);
            Assert.That(weapon.SecondaryMagicBonus, Is.Zero);
            Assert.That(weapon.SecondaryHasAbilities, Is.False);
        }

        [Test]
        public void GenerateCustomRodThatCanBeADoubleWeapon()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);
            template.Traits.Clear();

            var specialAbilityNames = template.Magic.SpecialAbilities.Select(a => a.Name);

            var attributes = new[] { "attribute 1", "attribute 2" };
            var tableName = TableNameConstants.Collections.ITEMTYPEAttributes(ItemTypeConstants.Rod);
            mockCollectionsSelector.Setup(p => p.SelectFrom(Config.Name, tableName, name)).Returns(attributes);

            var baseNames = new[] { "base name", "other base name", WeaponConstants.DireFlail };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.ItemGroups, name)).Returns(baseNames);

            tableName = TableNameConstants.Percentiles.POWERITEMTYPEs(PowerConstants.Medium, ItemTypeConstants.Rod);
            mockTypeAndAmountPercentileSelector.Setup(s => s.SelectAllFrom(Config.Name, tableName)).Returns(
            [
                new TypeAndAmountDataSelection { AmountAsDouble = 90210, Type = "other rod" },
                new TypeAndAmountDataSelection { AmountAsDouble = 9266, Type = name },
            ]);

            var mundaneWeapon = itemVerifier.CreateRandomWeaponTemplate(WeaponConstants.DireFlail);
            mundaneWeapon.Traits.Clear();
            mundaneWeapon.Quantity = 1;
            mundaneWeapon.Attributes = mundaneWeapon.Attributes.Union([AttributeConstants.DoubleWeapon]);

            mockMundaneWeaponGenerator
                .Setup(g => g.Generate(WeaponConstants.DireFlail))
                .Returns(mundaneWeapon);

            var abilities = new[]
            {
                new SpecialAbility { Name = specialAbilityNames.First() },
                new SpecialAbility { Name = specialAbilityNames.Last() }
            };

            mockSpecialAbilitiesGenerator.Setup(p => p.GenerateFor(template.Magic.SpecialAbilities, mundaneWeapon.CriticalMultiplier)).Returns(abilities);

            var rod = rodGenerator.Generate(template);
            itemVerifier.AssertMagicalItemFromTemplate(rod, template);
            Assert.That(rod.Attributes, Is.SupersetOf(attributes));
            Assert.That(rod.IsMagical, Is.True);
            Assert.That(rod.ItemType, Is.EqualTo(ItemTypeConstants.Rod));
            Assert.That(rod.Quantity, Is.EqualTo(1));
            Assert.That(rod.Magic.SpecialAbilities, Is.EqualTo(abilities));
            Assert.That(rod.BaseNames, Is.EqualTo(baseNames));
            Assert.That(rod.Magic.Bonus, Is.EqualTo(9266));
            Assert.That(rod, Is.InstanceOf<Weapon>());

            var weapon = rod as Weapon;
            Assert.That(weapon.Attributes, Is.SupersetOf(mundaneWeapon.Attributes));
            Assert.That(weapon.CriticalMultiplier, Is.EqualTo(mundaneWeapon.CriticalMultiplier));
            Assert.That(weapon.Damages.Select(d => d.Summary), Is.EqualTo(mundaneWeapon.Damages.Select(d => d.Summary)));
            Assert.That(weapon.CriticalDamages.Select(d => d.Summary), Is.EqualTo(mundaneWeapon.CriticalDamages.Select(d => d.Summary)));
            Assert.That(weapon.Size, Is.EqualTo(mundaneWeapon.Size));
            Assert.That(weapon.ThreatRangeSummary, Is.EqualTo(mundaneWeapon.ThreatRangeSummary));
            Assert.That(weapon.Traits, Contains.Item(TraitConstants.Masterwork).And.Count.EqualTo(1));
            Assert.That(weapon.IsDoubleWeapon, Is.True);
            Assert.That(weapon.SecondaryMagicBonus, Is.EqualTo(9266));
            Assert.That(weapon.SecondaryHasAbilities, Is.True);
        }

        [Test]
        public void GenerateCustomRodThatCanBeADoubleWeapon_WithAbilityDamages()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);
            template.Traits.Clear();

            var specialAbilityNames = template.Magic.SpecialAbilities.Select(a => a.Name);

            var attributes = new[] { "attribute 1", "attribute 2" };
            var tableName = TableNameConstants.Collections.ITEMTYPEAttributes(ItemTypeConstants.Rod);
            mockCollectionsSelector.Setup(p => p.SelectFrom(Config.Name, tableName, name)).Returns(attributes);

            var baseNames = new[] { "base name", "other base name", WeaponConstants.DireFlail };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.ItemGroups, name)).Returns(baseNames);

            tableName = TableNameConstants.Percentiles.POWERITEMTYPEs(PowerConstants.Medium, ItemTypeConstants.Rod);
            mockTypeAndAmountPercentileSelector.Setup(s => s.SelectAllFrom(Config.Name, tableName)).Returns(
            [
                new TypeAndAmountDataSelection { AmountAsDouble = 90210, Type = "other rod" },
                new TypeAndAmountDataSelection { AmountAsDouble = 9266, Type = name },
            ]);

            var mundaneWeapon = itemVerifier.CreateRandomWeaponTemplate(WeaponConstants.DireFlail);
            mundaneWeapon.Traits.Clear();
            mundaneWeapon.Quantity = 1;
            mundaneWeapon.Attributes = mundaneWeapon.Attributes.Union([AttributeConstants.DoubleWeapon]);
            mundaneWeapon.SecondaryDamages.Add(new Damage { Roll = "some", Type = "secondary" });
            mundaneWeapon.SecondaryCriticalDamages.Add(new Damage { Roll = "several", Type = "secondary" });

            mockMundaneWeaponGenerator
                .Setup(g => g.Generate(WeaponConstants.DireFlail))
                .Returns(mundaneWeapon);

            var abilities = new[]
            {
                new SpecialAbility { Name = specialAbilityNames.First() },
                new SpecialAbility
                {
                    Name = specialAbilityNames.Last(),
                    Damages = [new Damage { Roll = "some more", Type = "physical" }],
                    CriticalDamages = [new Damage { Roll = "even more", Type = "chemical" }]
                }
            };

            mockSpecialAbilitiesGenerator.Setup(p => p.GenerateFor(template.Magic.SpecialAbilities, mundaneWeapon.CriticalMultiplier)).Returns(abilities);

            var weaponWithAbilities = new Weapon
            {
                Name = "my weapon with abilities with damages"
            };
            weaponWithAbilities.Magic.SpecialAbilities = abilities;
            mockSpecialAbilitiesGenerator.Setup(p => p.ApplyAbilitiesToWeapon(mundaneWeapon)).Returns(weaponWithAbilities);

            var rod = rodGenerator.Generate(template);
            Assert.That(rod.Name, Is.EqualTo("my weapon with abilities with damages"));
            Assert.That(rod.Magic.SpecialAbilities, Is.EqualTo(abilities));
        }

        [Test]
        public void GenerateCustomRodThatCanBeAWeapon_WithTraits()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);
            var specialAbilityNames = template.Magic.SpecialAbilities.Select(a => a.Name);

            var attributes = new[] { "attribute 1", "attribute 2" };
            var tableName = TableNameConstants.Collections.ITEMTYPEAttributes(ItemTypeConstants.Rod);
            mockCollectionsSelector.Setup(p => p.SelectFrom(Config.Name, tableName, name)).Returns(attributes);

            var baseNames = new[] { "base name", "other base name", WeaponConstants.Club };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.ItemGroups, name)).Returns(baseNames);

            tableName = TableNameConstants.Percentiles.POWERITEMTYPEs(PowerConstants.Medium, ItemTypeConstants.Rod);
            mockTypeAndAmountPercentileSelector.Setup(s => s.SelectAllFrom(Config.Name, tableName)).Returns(
            [
                new TypeAndAmountDataSelection { AmountAsDouble = 90210, Type = "other rod" },
                new TypeAndAmountDataSelection { AmountAsDouble = 9266, Type = name },
            ]);

            var mundaneWeapon = itemVerifier.CreateRandomWeaponTemplate(WeaponConstants.Club);
            mundaneWeapon.Traits.Clear();
            mundaneWeapon.Quantity = 1;

            mockMundaneWeaponGenerator
                .Setup(g => g.Generate(WeaponConstants.Club, template.Traits.First(), template.Traits.Last()))
                .Returns(mundaneWeapon);

            var abilities = new[]
            {
                new SpecialAbility { Name = specialAbilityNames.First() },
                new SpecialAbility { Name = specialAbilityNames.Last() }
            };

            mockSpecialAbilitiesGenerator.Setup(p => p.GenerateFor(template.Magic.SpecialAbilities, mundaneWeapon.CriticalMultiplier)).Returns(abilities);

            var rod = rodGenerator.Generate(template);
            itemVerifier.AssertMagicalItemFromTemplate(rod, template);
            Assert.That(rod.Attributes, Is.SupersetOf(attributes));
            Assert.That(rod.IsMagical, Is.True);
            Assert.That(rod.ItemType, Is.EqualTo(ItemTypeConstants.Rod));
            Assert.That(rod.Quantity, Is.EqualTo(1));
            Assert.That(rod.Magic.SpecialAbilities, Is.EqualTo(abilities));
            Assert.That(rod.BaseNames, Is.EqualTo(baseNames));
            Assert.That(rod.Magic.Bonus, Is.EqualTo(9266));
            Assert.That(rod, Is.InstanceOf<Weapon>());

            var weapon = rod as Weapon;
            Assert.That(weapon.Attributes, Is.SupersetOf(mundaneWeapon.Attributes));
            Assert.That(weapon.CriticalMultiplier, Is.EqualTo(mundaneWeapon.CriticalMultiplier));
            Assert.That(weapon.Damages.Select(d => d.Summary), Is.EqualTo(mundaneWeapon.Damages.Select(d => d.Summary)));
            Assert.That(weapon.CriticalDamages.Select(d => d.Summary), Is.EqualTo(mundaneWeapon.CriticalDamages.Select(d => d.Summary)));
            Assert.That(weapon.Size, Is.EqualTo(mundaneWeapon.Size));
            Assert.That(weapon.ThreatRangeSummary, Is.EqualTo(mundaneWeapon.ThreatRangeSummary));
            Assert.That(weapon.Traits, Contains.Item(TraitConstants.Masterwork)
                .And.SupersetOf(template.Traits)
                .And.Count.EqualTo(3)
                .And.Count.EqualTo(template.Traits.Count + 1));
        }

        [Test]
        public void GenerateRandomCustomRod()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);
            var specialAbilityNames = template.Magic.SpecialAbilities.Select(a => a.Name);

            var attributes = new[] { "attribute 1", "attribute 2" };
            var tableName = TableNameConstants.Collections.ITEMTYPEAttributes(ItemTypeConstants.Rod);
            mockCollectionsSelector.Setup(p => p.SelectFrom(Config.Name, tableName, name)).Returns(attributes);

            var abilities = new[]
            {
                new SpecialAbility { Name = specialAbilityNames.First() },
                new SpecialAbility { Name = specialAbilityNames.Last() }
            };

            mockSpecialAbilitiesGenerator.Setup(p => p.GenerateFor(template.Magic.SpecialAbilities, string.Empty)).Returns(abilities);

            var baseNames = new[] { "base name", "other base name" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.ItemGroups, name)).Returns(baseNames);

            tableName = TableNameConstants.Percentiles.POWERITEMTYPEs(PowerConstants.Medium, ItemTypeConstants.Rod);
            mockTypeAndAmountPercentileSelector.Setup(s => s.SelectAllFrom(Config.Name, tableName)).Returns(
            [
                new TypeAndAmountDataSelection { AmountAsDouble = 90210, Type = "other rod" },
                new TypeAndAmountDataSelection { AmountAsDouble = 9266, Type = name },
            ]);

            var rod = rodGenerator.Generate(template, true);
            itemVerifier.AssertMagicalItemFromTemplate(rod, template);
            Assert.That(rod.Attributes, Is.EquivalentTo(attributes));
            Assert.That(rod.IsMagical, Is.True);
            Assert.That(rod.ItemType, Is.EqualTo(ItemTypeConstants.Rod));
            Assert.That(rod.Quantity, Is.EqualTo(1));
            Assert.That(rod.BaseNames, Is.EqualTo(baseNames));
            Assert.That(rod.Magic.Bonus, Is.EqualTo(9266));
        }

        [Test]
        public void GenerateOneTimeUseCustomRod()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);
            var specialAbilityNames = template.Magic.SpecialAbilities.Select(a => a.Name);

            var attributes = new[] { "attribute 1", "attribute 2", AttributeConstants.OneTimeUse };
            var tableName = TableNameConstants.Collections.ITEMTYPEAttributes(ItemTypeConstants.Rod);
            mockCollectionsSelector.Setup(p => p.SelectFrom(Config.Name, tableName, name)).Returns(attributes);

            var abilities = new[]
            {
                new SpecialAbility { Name = specialAbilityNames.First() },
                new SpecialAbility { Name = specialAbilityNames.Last() }
            };

            mockSpecialAbilitiesGenerator.Setup(p => p.GenerateFor(template.Magic.SpecialAbilities, string.Empty)).Returns(abilities);

            var baseNames = new[] { "base name", "other base name" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.ItemGroups, name)).Returns(baseNames);

            tableName = TableNameConstants.Percentiles.POWERITEMTYPEs(PowerConstants.Medium, ItemTypeConstants.Rod);
            mockTypeAndAmountPercentileSelector.Setup(s => s.SelectAllFrom(Config.Name, tableName)).Returns(
            [
                new TypeAndAmountDataSelection { AmountAsDouble = 90210, Type = "other rod" },
                new TypeAndAmountDataSelection { AmountAsDouble = 0, Type = name },
            ]);

            var rod = rodGenerator.Generate(template);
            itemVerifier.AssertMagicalItemFromTemplate(rod, template);
            Assert.That(rod.Attributes, Is.EquivalentTo(attributes));
            Assert.That(rod.IsMagical, Is.True);
            Assert.That(rod.ItemType, Is.EqualTo(ItemTypeConstants.Rod));
            Assert.That(rod.Quantity, Is.EqualTo(1));
            Assert.That(rod.BaseNames, Is.EqualTo(baseNames));
            Assert.That(rod.Magic.Bonus, Is.EqualTo(0));
        }

        [TestCase(RodConstants.Absorption)]
        [TestCase(RodConstants.Alertness)]
        [TestCase(RodConstants.MetalAndMineralDetection)]
        [TestCase(RodConstants.Metamagic_Empower)]
        [TestCase(RodConstants.Metamagic_Enlarge)]
        [TestCase(RodConstants.Metamagic_Extend)]
        [TestCase(RodConstants.Metamagic_Maximize)]
        [TestCase(RodConstants.Metamagic_Quicken)]
        [TestCase(RodConstants.Metamagic_Silent)]
        [TestCase(RodConstants.Metamagic_Empower_Lesser)]
        [TestCase(RodConstants.Metamagic_Enlarge_Lesser)]
        [TestCase(RodConstants.Metamagic_Extend_Lesser)]
        [TestCase(RodConstants.Metamagic_Maximize_Lesser)]
        [TestCase(RodConstants.Metamagic_Quicken_Lesser)]
        [TestCase(RodConstants.Metamagic_Silent_Lesser)]
        [TestCase(RodConstants.Metamagic_Empower_Greater)]
        [TestCase(RodConstants.Metamagic_Enlarge_Greater)]
        [TestCase(RodConstants.Metamagic_Extend_Greater)]
        [TestCase(RodConstants.Metamagic_Maximize_Greater)]
        [TestCase(RodConstants.Metamagic_Quicken_Greater)]
        [TestCase(RodConstants.Metamagic_Silent_Greater)]
        [TestCase(RodConstants.LordlyMight)]
        [TestCase(RodConstants.Cancellation)]
        [TestCase(RodConstants.EnemyDetection)]
        [TestCase(RodConstants.Flailing)]
        [TestCase(RodConstants.FlameExtinguishing)]
        [TestCase(RodConstants.ImmovableRod)]
        [TestCase(RodConstants.Negation)]
        [TestCase(RodConstants.Python)]
        [TestCase(RodConstants.Rulership)]
        [TestCase(RodConstants.Security)]
        [TestCase(RodConstants.Splendor)]
        [TestCase(RodConstants.ThunderAndLightning)]
        [TestCase(RodConstants.Viper)]
        [TestCase(RodConstants.Withering)]
        [TestCase(RodConstants.Wonder)]
        public void GenerateFromName(string rodName)
        {
            var tableName = TableNameConstants.Percentiles.POWERITEMTYPEs(power, ItemTypeConstants.Rod);
            mockTypeAndAmountPercentileSelector
                .Setup(s => s.SelectAllFrom(Config.Name, tableName))
                .Returns(
                [
                    new TypeAndAmountDataSelection { Type = "wrong rod", AmountAsDouble = 9266 },
                    new TypeAndAmountDataSelection { Type = rodName, AmountAsDouble = 90210 },
                    new TypeAndAmountDataSelection { Type = "other rod", AmountAsDouble = 42 },
                ]);

            var baseNames = new[] { "base name", "other base name" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.ItemGroups, rodName)).Returns(baseNames);

            var attributes = new[] { "attribute 1", "attribute 2" };
            tableName = TableNameConstants.Collections.ITEMTYPEAttributes(ItemTypeConstants.Rod);
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, tableName, rodName)).Returns(attributes);

            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(It.IsAny<IEnumerable<TypeAndAmountDataSelection>>()))
                .Returns((IEnumerable<TypeAndAmountDataSelection> c) => c.Last());

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, rodName))
                .Returns(new[] { "wrong power", power, "other power" });

            var rod = rodGenerator.Generate(power, rodName);
            Assert.That(rod.ItemType, Is.EqualTo(ItemTypeConstants.Rod));
            Assert.That(rod.IsMagical, Is.True);
            Assert.That(rod.Name, Is.EqualTo(rodName));
            Assert.That(rod.BaseNames, Is.EqualTo(baseNames));
            Assert.That(rod.Magic.Bonus, Is.EqualTo(90210));
            Assert.That(rod.Attributes, Is.EqualTo(attributes));
            Assert.That(rod, Is.Not.InstanceOf<Weapon>());
        }

        [Test]
        public void GenerateFromName_WithTraits()
        {
            var tableName = TableNameConstants.Percentiles.POWERITEMTYPEs(power, ItemTypeConstants.Rod);
            mockTypeAndAmountPercentileSelector
                .Setup(s => s.SelectAllFrom(Config.Name, tableName))
                .Returns(
                [
                    new TypeAndAmountDataSelection { Type = "wrong rod", AmountAsDouble = 9266 },
                    new TypeAndAmountDataSelection { Type = RodConstants.Alertness, AmountAsDouble = 90210 },
                    new TypeAndAmountDataSelection { Type = "other rod", AmountAsDouble = 42 },
                ]);

            var baseNames = new[] { "base name", "other base name" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.ItemGroups, RodConstants.Alertness)).Returns(baseNames);

            var attributes = new[] { "attribute 1", "attribute 2" };
            tableName = TableNameConstants.Collections.ITEMTYPEAttributes(ItemTypeConstants.Rod);
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, tableName, RodConstants.Alertness)).Returns(attributes);

            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(It.IsAny<IEnumerable<TypeAndAmountDataSelection>>()))
                .Returns((IEnumerable<TypeAndAmountDataSelection> c) => c.Last());

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, RodConstants.Alertness))
                .Returns(new[] { "wrong power", power, "other power" });

            var rod = rodGenerator.Generate(power, RodConstants.Alertness, "trait 1", "trait 2");
            Assert.That(rod.ItemType, Is.EqualTo(ItemTypeConstants.Rod));
            Assert.That(rod.IsMagical, Is.True);
            Assert.That(rod.Name, Is.EqualTo(RodConstants.Alertness));
            Assert.That(rod.BaseNames, Is.EqualTo(baseNames));
            Assert.That(rod.Magic.Bonus, Is.EqualTo(90210));
            Assert.That(rod.Attributes, Is.EqualTo(attributes));
            Assert.That(rod, Is.Not.InstanceOf<Weapon>());
            Assert.That(rod.Traits, Has.Count.EqualTo(2)
                .And.Contains("trait 1")
                .And.Contains("trait 2"));
        }

        [Test]
        public void GenerateFromBaseName()
        {
            var tableName = TableNameConstants.Percentiles.POWERITEMTYPEs(power, ItemTypeConstants.Rod);
            mockTypeAndAmountPercentileSelector
                .Setup(s => s.SelectAllFrom(Config.Name, tableName))
                .Returns(
                [
                    new TypeAndAmountDataSelection { Type = "wrong rod", AmountAsDouble = 9266 },
                    new TypeAndAmountDataSelection { Type = "rod", AmountAsDouble = 90210 },
                    new TypeAndAmountDataSelection { Type = "other rod", AmountAsDouble = 42 },
                ]);

            var baseNames = new[] { "base name", "other base name" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.ItemGroups, "rod")).Returns(baseNames);

            var attributes = new[] { "attribute 1", "attribute 2" };
            tableName = TableNameConstants.Collections.ITEMTYPEAttributes(ItemTypeConstants.Rod);
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, tableName, "rod")).Returns(attributes);

            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(It.IsAny<IEnumerable<TypeAndAmountDataSelection>>()))
                .Returns((IEnumerable<TypeAndAmountDataSelection> c) => c.Last());

            var rods = RodConstants.GetAllRods();
            mockCollectionsSelector
                .Setup(s => s.FindCollectionOf(
                    Config.Name,
                    TableNameConstants.Collections.ItemGroups,
                    "base name",
                    It.Is<string[]>(cn => cn.IsEquivalent(rods))))
                .Returns("rod");

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, "rod"))
                .Returns(new[] { "wrong power", power, "other power" });

            var rod = rodGenerator.Generate(power, "base name");
            Assert.That(rod.ItemType, Is.EqualTo(ItemTypeConstants.Rod));
            Assert.That(rod.IsMagical, Is.True);
            Assert.That(rod.Name, Is.EqualTo("rod"));
            Assert.That(rod.BaseNames, Is.EqualTo(baseNames));
            Assert.That(rod.Magic.Bonus, Is.EqualTo(90210));
            Assert.That(rod.Attributes, Is.EqualTo(attributes));
            Assert.That(rod, Is.Not.InstanceOf<Weapon>());
        }

        [Test]
        public void GenerateAsWeaponFromName()
        {
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, RodConstants.Alertness))
                .Returns(["wrong power", power, "other power"]);

            var tableName = TableNameConstants.Percentiles.POWERITEMTYPEs(power, ItemTypeConstants.Rod);
            mockTypeAndAmountPercentileSelector
                .Setup(s => s.SelectAllFrom(Config.Name, tableName))
                .Returns(
                [
                    new TypeAndAmountDataSelection { Type = "wrong rod", AmountAsDouble = 9266 },
                    new TypeAndAmountDataSelection { Type = RodConstants.Alertness, AmountAsDouble = 90210 },
                    new TypeAndAmountDataSelection { Type = "other rod", AmountAsDouble = 42 },
                ]);

            var baseNames = new[] { WeaponConstants.LightMace, "other base name" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.ItemGroups, RodConstants.Alertness)).Returns(baseNames);

            var attributes = new[] { "attribute 1", "attribute 2" };
            tableName = TableNameConstants.Collections.ITEMTYPEAttributes(ItemTypeConstants.Rod);
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, tableName, RodConstants.Alertness)).Returns(attributes);

            var mundaneWeapon = itemVerifier.CreateRandomWeaponTemplate(WeaponConstants.LightMace);
            mockMundaneWeaponGenerator
                .Setup(g => g.Generate(WeaponConstants.LightMace))
                .Returns(mundaneWeapon);

            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(It.IsAny<IEnumerable<TypeAndAmountDataSelection>>()))
                .Returns((IEnumerable<TypeAndAmountDataSelection> c) => c.Last());

            var rod = rodGenerator.Generate(power, RodConstants.Alertness);
            Assert.That(rod.ItemType, Is.EqualTo(ItemTypeConstants.Rod));
            Assert.That(rod.IsMagical, Is.True);
            Assert.That(rod.Name, Is.EqualTo(RodConstants.Alertness));
            Assert.That(rod.BaseNames, Is.EqualTo(baseNames));
            Assert.That(rod.Magic.Bonus, Is.EqualTo(90210));
            Assert.That(rod.Attributes, Is.SupersetOf(attributes));
            Assert.That(rod, Is.InstanceOf<Weapon>());

            var weapon = rod as Weapon;
            Assert.That(weapon.Attributes, Is.SupersetOf(mundaneWeapon.Attributes));
            Assert.That(weapon.CriticalMultiplier, Is.EqualTo(mundaneWeapon.CriticalMultiplier));
            Assert.That(weapon.Damages, Is.EqualTo(mundaneWeapon.Damages));
            Assert.That(weapon.CriticalDamages, Is.EqualTo(mundaneWeapon.CriticalDamages));
            Assert.That(weapon.Size, Is.EqualTo(mundaneWeapon.Size));
            Assert.That(weapon.ThreatRangeSummary, Is.EqualTo(mundaneWeapon.ThreatRangeSummary));
            Assert.That(weapon.Traits, Contains.Item(TraitConstants.Masterwork));
            Assert.That(weapon.IsDoubleWeapon, Is.False);
            Assert.That(weapon.SecondaryMagicBonus, Is.Zero);
            Assert.That(weapon.SecondaryHasAbilities, Is.False);
        }

        [Test]
        public void GenerateAsDoubleWeaponFromName()
        {
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, RodConstants.Flailing))
                .Returns(["wrong power", power, "other power"]);

            var tableName = TableNameConstants.Percentiles.POWERITEMTYPEs(power, ItemTypeConstants.Rod);
            mockTypeAndAmountPercentileSelector
                .Setup(s => s.SelectAllFrom(Config.Name, tableName))
                .Returns(
                [
                    new TypeAndAmountDataSelection { Type = "wrong rod", AmountAsDouble = 9266 },
                    new TypeAndAmountDataSelection { Type = RodConstants.Flailing, AmountAsDouble = 90210 },
                    new TypeAndAmountDataSelection { Type = "other rod", AmountAsDouble = 42 },
                ]);

            var baseNames = new[] { WeaponConstants.DireFlail, "other base name" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.ItemGroups, RodConstants.Flailing)).Returns(baseNames);

            var attributes = new[] { "attribute 1", "attribute 2" };
            tableName = TableNameConstants.Collections.ITEMTYPEAttributes(ItemTypeConstants.Rod);
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, tableName, RodConstants.Flailing)).Returns(attributes);

            var mundaneWeapon = itemVerifier.CreateRandomWeaponTemplate(WeaponConstants.DireFlail);
            mundaneWeapon.Attributes = mundaneWeapon.Attributes.Union([AttributeConstants.DoubleWeapon]);

            mockMundaneWeaponGenerator
                .Setup(g => g.Generate(WeaponConstants.DireFlail))
                .Returns(mundaneWeapon);

            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(It.IsAny<IEnumerable<TypeAndAmountDataSelection>>()))
                .Returns((IEnumerable<TypeAndAmountDataSelection> c) => c.Last());

            var rod = rodGenerator.Generate(power, RodConstants.Flailing);
            Assert.That(rod.ItemType, Is.EqualTo(ItemTypeConstants.Rod));
            Assert.That(rod.IsMagical, Is.True);
            Assert.That(rod.Name, Is.EqualTo(RodConstants.Flailing));
            Assert.That(rod.BaseNames, Is.EqualTo(baseNames));
            Assert.That(rod.Magic.Bonus, Is.EqualTo(90210));
            Assert.That(rod.Attributes, Is.SupersetOf(attributes));
            Assert.That(rod, Is.InstanceOf<Weapon>());

            var weapon = rod as Weapon;
            Assert.That(weapon.Attributes, Is.SupersetOf(mundaneWeapon.Attributes));
            Assert.That(weapon.CriticalMultiplier, Is.EqualTo(mundaneWeapon.CriticalMultiplier));
            Assert.That(weapon.Damages, Is.EqualTo(mundaneWeapon.Damages));
            Assert.That(weapon.CriticalDamages, Is.EqualTo(mundaneWeapon.CriticalDamages));
            Assert.That(weapon.Size, Is.EqualTo(mundaneWeapon.Size));
            Assert.That(weapon.ThreatRangeSummary, Is.EqualTo(mundaneWeapon.ThreatRangeSummary));
            Assert.That(weapon.Traits, Contains.Item(TraitConstants.Masterwork));
            Assert.That(weapon.IsDoubleWeapon, Is.True);
            Assert.That(weapon.SecondaryMagicBonus, Is.EqualTo(90210));
            Assert.That(weapon.SecondaryHasAbilities, Is.True);
        }

        [Test]
        public void GenerateAsWeaponFromName_WithTraits()
        {
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, RodConstants.Alertness))
                .Returns(["wrong power", power, "other power"]);

            var tableName = TableNameConstants.Percentiles.POWERITEMTYPEs(power, ItemTypeConstants.Rod);
            mockTypeAndAmountPercentileSelector
                .Setup(s => s.SelectAllFrom(Config.Name, tableName))
                .Returns(
                [
                    new TypeAndAmountDataSelection { Type = "wrong rod", AmountAsDouble = 9266 },
                    new TypeAndAmountDataSelection { Type = RodConstants.Alertness, AmountAsDouble = 90210 },
                    new TypeAndAmountDataSelection { Type = "other rod", AmountAsDouble = 42 },
                ]);

            var baseNames = new[] { WeaponConstants.LightMace, "other base name" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.ItemGroups, RodConstants.Alertness)).Returns(baseNames);

            var attributes = new[] { "attribute 1", "attribute 2" };
            tableName = TableNameConstants.Collections.ITEMTYPEAttributes(ItemTypeConstants.Rod);
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, tableName, RodConstants.Alertness)).Returns(attributes);

            var mundaneWeapon = itemVerifier.CreateRandomWeaponTemplate(WeaponConstants.LightMace);
            mundaneWeapon.Traits.Clear();
            mundaneWeapon.Traits.Add("trait 1");
            mundaneWeapon.Traits.Add("trait 2");

            mockMundaneWeaponGenerator.Setup(g => g.Generate(WeaponConstants.LightMace, "trait 1", "trait 2")).Returns(mundaneWeapon);

            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(It.IsAny<IEnumerable<TypeAndAmountDataSelection>>()))
                .Returns((IEnumerable<TypeAndAmountDataSelection> c) => c.Last());

            var rod = rodGenerator.Generate(power, RodConstants.Alertness, "trait 1", "trait 2");
            Assert.That(rod.ItemType, Is.EqualTo(ItemTypeConstants.Rod));
            Assert.That(rod.IsMagical, Is.True);
            Assert.That(rod.Name, Is.EqualTo(RodConstants.Alertness));
            Assert.That(rod.BaseNames, Is.EqualTo(baseNames));
            Assert.That(rod.Magic.Bonus, Is.EqualTo(90210));
            Assert.That(rod.Attributes, Is.SupersetOf(attributes));
            Assert.That(rod, Is.InstanceOf<Weapon>());
            Assert.That(rod.Traits, Has.Count.EqualTo(3)
                .And.Contains("trait 1")
                .And.Contains("trait 2")
                .And.Contains(TraitConstants.Masterwork));

            var weapon = rod as Weapon;
            Assert.That(weapon.Attributes, Is.SupersetOf(mundaneWeapon.Attributes));
            Assert.That(weapon.CriticalMultiplier, Is.EqualTo(mundaneWeapon.CriticalMultiplier));
            Assert.That(weapon.Damages, Is.EqualTo(mundaneWeapon.Damages));
            Assert.That(weapon.CriticalDamages, Is.EqualTo(mundaneWeapon.CriticalDamages));
            Assert.That(weapon.Size, Is.EqualTo(mundaneWeapon.Size));
            Assert.That(weapon.ThreatRangeSummary, Is.EqualTo(mundaneWeapon.ThreatRangeSummary));
        }

        [Test]
        public void MinorPowerFromNameAdjustsPower()
        {
            var tableName = TableNameConstants.Percentiles.POWERITEMTYPEs(power, ItemTypeConstants.Rod);
            mockTypeAndAmountPercentileSelector
                .Setup(s => s.SelectAllFrom(Config.Name, tableName))
                .Returns(
                [
                    new TypeAndAmountDataSelection { Type = "wrong rod", AmountAsDouble = 9266 },
                    new TypeAndAmountDataSelection { Type = RodConstants.Alertness, AmountAsDouble = 90210 },
                    new TypeAndAmountDataSelection { Type = "other rod", AmountAsDouble = 42 },
                ]);

            var baseNames = new[] { "base name", "other base name" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.ItemGroups, RodConstants.Alertness)).Returns(baseNames);

            var attributes = new[] { "attribute 1", "attribute 2" };
            tableName = TableNameConstants.Collections.ITEMTYPEAttributes(ItemTypeConstants.Rod);
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, tableName, RodConstants.Alertness)).Returns(attributes);

            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(It.IsAny<IEnumerable<TypeAndAmountDataSelection>>()))
                .Returns((IEnumerable<TypeAndAmountDataSelection> c) => c.Last());

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, RodConstants.Alertness))
                .Returns([power, "wrong power"]);

            var rod = rodGenerator.Generate(PowerConstants.Minor, RodConstants.Alertness);
            Assert.That(rod.ItemType, Is.EqualTo(ItemTypeConstants.Rod));
            Assert.That(rod.IsMagical, Is.True);
            Assert.That(rod.Name, Is.EqualTo(RodConstants.Alertness));
            Assert.That(rod.BaseNames, Is.EqualTo(baseNames));
            Assert.That(rod.Magic.Bonus, Is.EqualTo(90210));
            Assert.That(rod.Attributes, Is.EqualTo(attributes));
            Assert.That(rod, Is.Not.InstanceOf<Weapon>());
        }

        [Test]
        public void GenerateFromName_MultipleOfPower()
        {
            var tableName = TableNameConstants.Percentiles.POWERITEMTYPEs(power, ItemTypeConstants.Rod);
            mockTypeAndAmountPercentileSelector
                .Setup(s => s.SelectAllFrom(Config.Name, tableName))
                .Returns(
                [
                    new TypeAndAmountDataSelection { Type = "wrong rod", AmountAsDouble = 9266 },
                    new TypeAndAmountDataSelection { Type = RodConstants.Alertness, AmountAsDouble = 90210 },
                    new TypeAndAmountDataSelection { Type = RodConstants.Alertness, AmountAsDouble = 42 },
                ]);

            var baseNames = new[] { "base name", "other base name" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.ItemGroups, RodConstants.Alertness)).Returns(baseNames);

            var attributes = new[] { "attribute 1", "attribute 2" };
            tableName = TableNameConstants.Collections.ITEMTYPEAttributes(ItemTypeConstants.Rod);
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, tableName, RodConstants.Alertness)).Returns(attributes);

            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(It.IsAny<IEnumerable<TypeAndAmountDataSelection>>()))
                .Returns((IEnumerable<TypeAndAmountDataSelection> c) => c.Last());

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, RodConstants.Alertness))
                .Returns(["wrong power", power, "other power"]);

            var rod = rodGenerator.Generate(power, RodConstants.Alertness);
            Assert.That(rod.ItemType, Is.EqualTo(ItemTypeConstants.Rod));
            Assert.That(rod.IsMagical, Is.True);
            Assert.That(rod.Name, Is.EqualTo(RodConstants.Alertness));
            Assert.That(rod.BaseNames, Is.EqualTo(baseNames));
            Assert.That(rod.Magic.Bonus, Is.EqualTo(42));
            Assert.That(rod.Attributes, Is.EqualTo(attributes));
            Assert.That(rod, Is.Not.InstanceOf<Weapon>());
        }

        [Test]
        public void GenerateFromName_NoneOfPower()
        {
            var tableName = TableNameConstants.Percentiles.POWERITEMTYPEs(power, ItemTypeConstants.Rod);
            mockTypeAndAmountPercentileSelector
                .Setup(s => s.SelectAllFrom(Config.Name, tableName))
                .Returns(
                [
                    new TypeAndAmountDataSelection { Type = "wrong rod", AmountAsDouble = 9266 },
                    new TypeAndAmountDataSelection { Type = RodConstants.Alertness, AmountAsDouble = 90210 },
                    new TypeAndAmountDataSelection { Type = "other rod", AmountAsDouble = 42 },
                ]);

            var baseNames = new[] { "base name", "other base name" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.ItemGroups, RodConstants.Alertness)).Returns(baseNames);

            var attributes = new[] { "attribute 1", "attribute 2" };
            tableName = TableNameConstants.Collections.ITEMTYPEAttributes(ItemTypeConstants.Rod);
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, tableName, RodConstants.Alertness)).Returns(attributes);

            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(It.IsAny<IEnumerable<TypeAndAmountDataSelection>>()))
                .Returns((IEnumerable<TypeAndAmountDataSelection> c) => c.Last());

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, RodConstants.Alertness))
                .Returns([power, "wrong power"]);

            var rod = rodGenerator.Generate("other power", RodConstants.Alertness);
            Assert.That(rod.ItemType, Is.EqualTo(ItemTypeConstants.Rod));
            Assert.That(rod.IsMagical, Is.True);
            Assert.That(rod.Name, Is.EqualTo(RodConstants.Alertness));
            Assert.That(rod.BaseNames, Is.EqualTo(baseNames));
            Assert.That(rod.Magic.Bonus, Is.EqualTo(90210));
            Assert.That(rod.Attributes, Is.EqualTo(attributes));
            Assert.That(rod, Is.Not.InstanceOf<Weapon>());
        }
    }
}