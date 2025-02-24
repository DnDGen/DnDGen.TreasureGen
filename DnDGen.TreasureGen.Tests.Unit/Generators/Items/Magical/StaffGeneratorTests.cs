using DnDGen.Infrastructure.Factories;
using DnDGen.Infrastructure.Models;
using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.Infrastructure.Selectors.Percentiles;
using DnDGen.TreasureGen.Generators.Items.Magical;
using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Items.Mundane;
using DnDGen.TreasureGen.Tables;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.TreasureGen.Tests.Unit.Generators.Items.Magical
{
    [TestFixture]
    public class StaffGeneratorTests
    {
        private MagicalItemGenerator staffGenerator;
        private Mock<IChargesGenerator> mockChargesGenerator;
        private Mock<IPercentileTypeAndAmountSelector> mockTypeAndAmountPercentileSelector;
        private Mock<ICollectionSelector> mockCollectionsSelector;
        private string power;
        private ItemVerifier itemVerifier;
        private Mock<ISpecialAbilitiesGenerator> mockSpecialAbilitiesGenerator;
        private Mock<MundaneItemGenerator> mockMundaneWeaponGenerator;
        private TypeAndAmountDataSelection selection;

        [SetUp]
        public void Setup()
        {
            mockTypeAndAmountPercentileSelector = new Mock<IPercentileTypeAndAmountSelector>();
            mockChargesGenerator = new Mock<IChargesGenerator>();
            mockCollectionsSelector = new Mock<ICollectionSelector>();
            mockSpecialAbilitiesGenerator = new Mock<ISpecialAbilitiesGenerator>();
            mockMundaneWeaponGenerator = new Mock<MundaneItemGenerator>();
            var mockJustInTimeFactory = new Mock<JustInTimeFactory>();

            mockJustInTimeFactory.Setup(f => f.Build<MundaneItemGenerator>(ItemTypeConstants.Weapon)).Returns(mockMundaneWeaponGenerator.Object);

            staffGenerator = new StaffGenerator(
                mockTypeAndAmountPercentileSelector.Object,
                mockChargesGenerator.Object,
                mockCollectionsSelector.Object,
                mockSpecialAbilitiesGenerator.Object,
                mockJustInTimeFactory.Object);
            power = "power";
            itemVerifier = new ItemVerifier();
            selection = new TypeAndAmountDataSelection
            {
                Type = "staffiness",
                AmountAsDouble = 90210
            };

            var tableName = TableNameConstants.Percentiles.POWERITEMTYPEs(power, ItemTypeConstants.Staff);
            mockTypeAndAmountPercentileSelector.Setup(s => s.SelectFrom(Config.Name, tableName)).Returns(selection);
            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Staff, selection.Type)).Returns(9266);

            mockSpecialAbilitiesGenerator.Setup(p => p.ApplyAbilitiesToWeapon(It.IsAny<Weapon>())).Returns((Weapon w) => w);
        }

        [Test]
        public void GenerateRandom_GenerateStaff()
        {
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, ItemTypeConstants.Staff))
                .Returns(new[] { "wrong power", power, "other power" });

            var staff = staffGenerator.GenerateRandom(power);
            Assert.That(staff.Name, Is.EqualTo("staffiness"));
            Assert.That(staff.ItemType, Is.EqualTo(ItemTypeConstants.Staff));
            Assert.That(staff.Attributes, Contains.Item(AttributeConstants.Charged));
            Assert.That(staff.Attributes, Contains.Item(AttributeConstants.OneTimeUse));
            Assert.That(staff.Attributes.Count(), Is.EqualTo(2));
            Assert.That(staff.Magic.Bonus, Is.EqualTo(90210));
            Assert.That(staff, Is.Not.InstanceOf<Weapon>());
            Assert.That(staff.Traits, Is.All.Not.EqualTo(TraitConstants.Masterwork));
            Assert.That(staff.Magic.Charges, Is.EqualTo(9266));
        }

        [Test]
        public void GenerateRandom_AdjustMinorPowerStaves()
        {
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, ItemTypeConstants.Staff))
                .Returns(new[] { power, "other power", "wrong power" });

            var staff = staffGenerator.GenerateRandom(PowerConstants.Minor);
            Assert.That(staff.Name, Is.EqualTo("staffiness"));
            Assert.That(staff.ItemType, Is.EqualTo(ItemTypeConstants.Staff));
            Assert.That(staff.Attributes, Contains.Item(AttributeConstants.Charged));
            Assert.That(staff.Attributes, Contains.Item(AttributeConstants.OneTimeUse));
            Assert.That(staff.Attributes.Count(), Is.EqualTo(2));
            Assert.That(staff.Magic.Bonus, Is.EqualTo(90210));
            Assert.That(staff, Is.Not.InstanceOf<Weapon>());
            Assert.That(staff.Traits, Is.All.Not.EqualTo(TraitConstants.Masterwork));
            Assert.That(staff.Magic.Charges, Is.EqualTo(9266));
        }

        [Test]
        public void GenerateRandom_GetBaseNames()
        {
            var baseNames = new[] { "base name", "other base name" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.ItemGroups, selection.Type)).Returns(baseNames);

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, ItemTypeConstants.Staff))
                .Returns(new[] { "wrong power", power, "other power" });

            var staff = staffGenerator.GenerateRandom(power);
            Assert.That(staff.BaseNames, Is.EqualTo(baseNames));
        }

        [Test]
        public void GenerateRandom_GetStaffThatIsAlsoWeapon()
        {
            var baseNames = new[] { "base name", "other base name", WeaponConstants.Dagger };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.ItemGroups, selection.Type)).Returns(baseNames);

            var mundaneWeapon = itemVerifier.CreateRandomWeaponTemplate(WeaponConstants.Dagger);
            mockMundaneWeaponGenerator.Setup(g => g.Generate(WeaponConstants.Dagger)).Returns(mundaneWeapon);

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, ItemTypeConstants.Staff))
                .Returns(new[] { "wrong power", power, "other power" });

            var staff = staffGenerator.GenerateRandom(power);
            Assert.That(staff.ItemType, Is.EqualTo(ItemTypeConstants.Staff));
            Assert.That(staff.Attributes, Is.EquivalentTo(mundaneWeapon.Attributes.Union(new[] { AttributeConstants.Charged })));
            Assert.That(staff.Magic.Bonus, Is.EqualTo(90210));
            Assert.That(staff.BaseNames, Is.EqualTo(baseNames));
            Assert.That(staff.Magic.Charges, Is.EqualTo(9266));
            Assert.That(staff, Is.InstanceOf<Weapon>());

            var weapon = staff as Weapon;
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
        public void GenerateRandom_GetStaffThatIsAlsoDoubleWeapon()
        {
            var baseNames = new[] { "base name", "other base name", WeaponConstants.Quarterstaff };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.ItemGroups, selection.Type)).Returns(baseNames);

            var mundaneWeapon = itemVerifier.CreateRandomWeaponTemplate(WeaponConstants.Quarterstaff);
            mundaneWeapon.Attributes = mundaneWeapon.Attributes.Union(new[] { AttributeConstants.DoubleWeapon });

            mockMundaneWeaponGenerator.Setup(g => g.Generate(WeaponConstants.Quarterstaff)).Returns(mundaneWeapon);

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, ItemTypeConstants.Staff))
                .Returns(new[] { "wrong power", power, "other power" });

            var staff = staffGenerator.GenerateRandom(power);
            Assert.That(staff.ItemType, Is.EqualTo(ItemTypeConstants.Staff));
            Assert.That(staff.Attributes, Is.EquivalentTo(mundaneWeapon.Attributes.Union(new[] { AttributeConstants.Charged })));
            Assert.That(staff.Magic.Bonus, Is.EqualTo(90210));
            Assert.That(staff.BaseNames, Is.EqualTo(baseNames));
            Assert.That(staff.Magic.Charges, Is.EqualTo(9266));
            Assert.That(staff, Is.InstanceOf<Weapon>());

            var weapon = staff as Weapon;
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
        public void GenerateCustomStaff()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);
            var specialAbilityNames = template.Magic.SpecialAbilities.Select(a => a.Name);

            var abilities = new[]
            {
                new SpecialAbility { Name = specialAbilityNames.First() },
                new SpecialAbility { Name = specialAbilityNames.Last() }
            };

            mockSpecialAbilitiesGenerator.Setup(p => p.GenerateFor(template.Magic.SpecialAbilities, string.Empty)).Returns(abilities);

            var baseNames = new[] { "base name", "other base name" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.ItemGroups, name)).Returns(baseNames);

            var staff = staffGenerator.Generate(template);
            itemVerifier.AssertMagicalItemFromTemplate(staff, template);
            Assert.That(staff.ItemType, Is.EqualTo(ItemTypeConstants.Staff));
            Assert.That(staff.Attributes, Contains.Item(AttributeConstants.Charged));
            Assert.That(staff.Attributes, Contains.Item(AttributeConstants.OneTimeUse));
            Assert.That(staff.Attributes.Count(), Is.EqualTo(2));
            Assert.That(staff.Quantity, Is.EqualTo(1));
            Assert.That(staff.BaseNames, Is.EquivalentTo(baseNames));
            Assert.That(staff, Is.Not.InstanceOf<Weapon>());
            Assert.That(staff.Traits, Is.All.Not.EqualTo(TraitConstants.Masterwork));
        }

        [Test]
        public void GenerateCustomStaffThatIsAlsoAWeapon()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);
            template.Traits.Clear();

            var baseNames = new[] { "base name", "other base name", WeaponConstants.Dagger };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.ItemGroups, name)).Returns(baseNames);

            var mundaneWeapon = itemVerifier.CreateRandomWeaponTemplate(WeaponConstants.Dagger);
            mundaneWeapon.Traits.Clear();
            mundaneWeapon.Quantity = 1;

            mockMundaneWeaponGenerator
                .Setup(g => g.Generate(WeaponConstants.Dagger))
                .Returns(mundaneWeapon);

            var specialAbilityNames = template.Magic.SpecialAbilities.Select(a => a.Name);

            var abilities = new[]
            {
                new SpecialAbility { Name = specialAbilityNames.First() },
                new SpecialAbility { Name = specialAbilityNames.Last() }
            };

            mockSpecialAbilitiesGenerator.Setup(p => p.GenerateFor(template.Magic.SpecialAbilities, mundaneWeapon.CriticalMultiplier)).Returns(abilities);
            mockSpecialAbilitiesGenerator.Setup(g => g.ApplyAbilitiesToWeapon(It.Is<Weapon>(w => w.Magic.SpecialAbilities == abilities))).Returns((Weapon w) => w);

            var staff = staffGenerator.Generate(template);
            itemVerifier.AssertMagicalItemFromTemplate(staff, template);
            Assert.That(staff.ItemType, Is.EqualTo(ItemTypeConstants.Staff));
            Assert.That(staff.Attributes, Is.EquivalentTo(mundaneWeapon.Attributes.Union(new[] { AttributeConstants.Charged })));
            Assert.That(staff.Attributes, Is.All.Not.EqualTo(AttributeConstants.OneTimeUse));
            Assert.That(staff.Quantity, Is.EqualTo(1));
            Assert.That(staff.Magic.SpecialAbilities, Is.EquivalentTo(abilities));
            Assert.That(staff.BaseNames, Is.EqualTo(baseNames));
            Assert.That(staff, Is.InstanceOf<Weapon>());

            var weapon = staff as Weapon;
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
        public void GenerateCustomStaffThatIsAlsoADoubleWeapon()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);
            template.Traits.Clear();

            var baseNames = new[] { "base name", "other base name", WeaponConstants.Quarterstaff };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.ItemGroups, name)).Returns(baseNames);

            var mundaneWeapon = itemVerifier.CreateRandomWeaponTemplate(WeaponConstants.Quarterstaff);
            mundaneWeapon.Traits.Clear();
            mundaneWeapon.Quantity = 1;
            mundaneWeapon.Attributes = mundaneWeapon.Attributes.Union([AttributeConstants.DoubleWeapon]);

            mockMundaneWeaponGenerator
                .Setup(g => g.Generate(WeaponConstants.Quarterstaff))
                .Returns(mundaneWeapon);

            var specialAbilityNames = template.Magic.SpecialAbilities.Select(a => a.Name);

            var abilities = new[]
            {
                new SpecialAbility { Name = specialAbilityNames.First() },
                new SpecialAbility { Name = specialAbilityNames.Last() }
            };

            mockSpecialAbilitiesGenerator.Setup(p => p.GenerateFor(template.Magic.SpecialAbilities, mundaneWeapon.CriticalMultiplier)).Returns(abilities);
            mockSpecialAbilitiesGenerator.Setup(g => g.ApplyAbilitiesToWeapon(It.Is<Weapon>(w => w.Magic.SpecialAbilities == abilities))).Returns((Weapon w) => w);

            var staff = staffGenerator.Generate(template);
            itemVerifier.AssertMagicalItemFromTemplate(staff, template);
            Assert.That(staff.ItemType, Is.EqualTo(ItemTypeConstants.Staff));
            Assert.That(staff.Attributes, Is.EquivalentTo(mundaneWeapon.Attributes.Union([AttributeConstants.Charged])));
            Assert.That(staff.Attributes, Is.All.Not.EqualTo(AttributeConstants.OneTimeUse));
            Assert.That(staff.Quantity, Is.EqualTo(1));
            Assert.That(staff.Magic.SpecialAbilities, Is.EquivalentTo(abilities));
            Assert.That(staff.BaseNames, Is.EqualTo(baseNames));
            Assert.That(staff, Is.InstanceOf<Weapon>());

            var weapon = staff as Weapon;
            Assert.That(weapon.Attributes, Is.SupersetOf(mundaneWeapon.Attributes));
            Assert.That(weapon.CriticalMultiplier, Is.EqualTo(mundaneWeapon.CriticalMultiplier));
            Assert.That(weapon.Damages.Select(d => d.Summary), Is.EqualTo(mundaneWeapon.Damages.Select(d => d.Summary)));
            Assert.That(weapon.CriticalDamages.Select(d => d.Summary), Is.EqualTo(mundaneWeapon.CriticalDamages.Select(d => d.Summary)));
            Assert.That(weapon.Size, Is.EqualTo(mundaneWeapon.Size));
            Assert.That(weapon.ThreatRangeSummary, Is.EqualTo(mundaneWeapon.ThreatRangeSummary));
            Assert.That(weapon.Traits, Contains.Item(TraitConstants.Masterwork).And.Count.EqualTo(1));
            Assert.That(weapon.IsDoubleWeapon, Is.True);
            Assert.That(weapon.SecondaryMagicBonus, Is.Positive.And.EqualTo(template.Magic.Bonus));
            Assert.That(weapon.SecondaryHasAbilities, Is.True);
        }

        [Test]
        public void GenerateCustomStaffThatIsAlsoAWeapon_WithTraits()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);
            var baseNames = new[] { "base name", "other base name", WeaponConstants.Dagger };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.ItemGroups, name)).Returns(baseNames);

            var mundaneWeapon = itemVerifier.CreateRandomWeaponTemplate(WeaponConstants.Dagger);
            mundaneWeapon.Traits.Clear();
            mundaneWeapon.Quantity = 1;

            mockMundaneWeaponGenerator
                .Setup(g => g.Generate(WeaponConstants.Dagger, template.Traits.First(), template.Traits.Last()))
                .Returns(mundaneWeapon);

            var specialAbilityNames = template.Magic.SpecialAbilities.Select(a => a.Name);

            var abilities = new[]
            {
                new SpecialAbility { Name = specialAbilityNames.First() },
                new SpecialAbility { Name = specialAbilityNames.Last() }
            };

            mockSpecialAbilitiesGenerator.Setup(p => p.GenerateFor(template.Magic.SpecialAbilities, mundaneWeapon.CriticalMultiplier)).Returns(abilities);
            mockSpecialAbilitiesGenerator.Setup(g => g.ApplyAbilitiesToWeapon(It.Is<Weapon>(w => w.Magic.SpecialAbilities == abilities))).Returns((Weapon w) => w);

            var staff = staffGenerator.Generate(template);
            itemVerifier.AssertMagicalItemFromTemplate(staff, template);
            Assert.That(staff.ItemType, Is.EqualTo(ItemTypeConstants.Staff));
            Assert.That(staff.Attributes, Is.EquivalentTo(mundaneWeapon.Attributes.Union(new[] { AttributeConstants.Charged })));
            Assert.That(staff.Attributes, Is.All.Not.EqualTo(AttributeConstants.OneTimeUse));
            Assert.That(staff.Quantity, Is.EqualTo(1));
            Assert.That(staff.Magic.SpecialAbilities, Is.EquivalentTo(abilities));
            Assert.That(staff.BaseNames, Is.EqualTo(baseNames));
            Assert.That(staff, Is.InstanceOf<Weapon>());

            var weapon = staff as Weapon;
            Assert.That(weapon.Attributes, Is.SupersetOf(mundaneWeapon.Attributes));
            Assert.That(weapon.CriticalMultiplier, Is.EqualTo(mundaneWeapon.CriticalMultiplier));
            Assert.That(weapon.Damages.Select(d => d.Summary), Is.EqualTo(mundaneWeapon.Damages.Select(d => d.Summary)));
            Assert.That(weapon.CriticalDamages.Select(d => d.Summary), Is.EqualTo(mundaneWeapon.CriticalDamages.Select(d => d.Summary)));
            Assert.That(weapon.Size, Is.EqualTo(mundaneWeapon.Size));
            Assert.That(weapon.ThreatRangeSummary, Is.EqualTo(mundaneWeapon.ThreatRangeSummary));
            Assert.That(weapon.Traits, Contains.Item(TraitConstants.Masterwork)
                .And.Count.EqualTo(3)
                .And.Count.EqualTo(template.Traits.Count + 1)
                .And.SupersetOf(template.Traits));
        }

        [Test]
        public void GenerateRandomCustomStaff()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);
            var specialAbilityNames = template.Magic.SpecialAbilities.Select(a => a.Name);

            var abilities = new[]
            {
                new SpecialAbility { Name = specialAbilityNames.First() },
                new SpecialAbility { Name = specialAbilityNames.Last() }
            };

            mockSpecialAbilitiesGenerator.Setup(p => p.GenerateFor(template.Magic.SpecialAbilities, string.Empty)).Returns(abilities);
            mockSpecialAbilitiesGenerator.Setup(g => g.ApplyAbilitiesToWeapon(It.Is<Weapon>(w => w.Magic.SpecialAbilities == abilities))).Returns((Weapon w) => w);

            var baseNames = new[] { "base name", "other base name" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.ItemGroups, name)).Returns(baseNames);

            var staff = staffGenerator.Generate(template, true);
            itemVerifier.AssertMagicalItemFromTemplate(staff, template);
            Assert.That(staff.ItemType, Is.EqualTo(ItemTypeConstants.Staff));
            Assert.That(staff.Attributes, Contains.Item(AttributeConstants.Charged));
            Assert.That(staff.Attributes, Contains.Item(AttributeConstants.OneTimeUse));
            Assert.That(staff.Attributes.Count(), Is.EqualTo(2));
            Assert.That(staff.Quantity, Is.EqualTo(1));
            Assert.That(staff.BaseNames, Is.EqualTo(baseNames));
            Assert.That(staff, Is.Not.InstanceOf<Weapon>());
        }

        [TestCase(StaffConstants.Abjuration)]
        [TestCase(StaffConstants.Charming)]
        [TestCase(StaffConstants.Conjuration)]
        [TestCase(StaffConstants.Defense)]
        [TestCase(StaffConstants.Divination)]
        [TestCase(StaffConstants.EarthAndStone)]
        [TestCase(StaffConstants.Enchantment)]
        [TestCase(StaffConstants.Evocation)]
        [TestCase(StaffConstants.Fire)]
        [TestCase(StaffConstants.Frost)]
        [TestCase(StaffConstants.Healing)]
        [TestCase(StaffConstants.Illumination)]
        [TestCase(StaffConstants.Illusion)]
        [TestCase(StaffConstants.Life)]
        [TestCase(StaffConstants.Necromancy)]
        [TestCase(StaffConstants.Passage)]
        [TestCase(StaffConstants.Power)]
        [TestCase(StaffConstants.SizeAlteration)]
        [TestCase(StaffConstants.SwarmingInsects)]
        [TestCase(StaffConstants.Transmutation)]
        [TestCase(StaffConstants.Woodlands)]
        public void GenerateFromName(string staffName)
        {
            var tableName = TableNameConstants.Percentiles.POWERITEMTYPEs(power, ItemTypeConstants.Staff);
            mockTypeAndAmountPercentileSelector
                .Setup(s => s.SelectAllFrom(Config.Name, tableName))
                .Returns(
                [
                    new TypeAndAmountDataSelection { Type = "wrong staff", AmountAsDouble = 666 },
                    new TypeAndAmountDataSelection { Type = staffName, AmountAsDouble = 42 },
                    new TypeAndAmountDataSelection { Type = "other staff", AmountAsDouble = 600 },
                ]);

            var baseNames = new[] { "base name", "other base name" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.ItemGroups, staffName)).Returns(baseNames);

            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Staff, "wrong staff")).Returns(666);
            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Staff, staffName)).Returns(9266);
            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Staff, "other staff")).Returns(90210);

            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(It.IsAny<IEnumerable<TypeAndAmountDataSelection>>()))
                .Returns((IEnumerable<TypeAndAmountDataSelection> c) => c.Last());

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, staffName))
                .Returns(new[] { "wrong power", power, "other power" });

            var staff = staffGenerator.Generate(power, staffName);
            Assert.That(staff.Name, Is.EqualTo(staffName));
            Assert.That(staff.ItemType, Is.EqualTo(ItemTypeConstants.Staff));
            Assert.That(staff.Attributes, Contains.Item(AttributeConstants.Charged));
            Assert.That(staff.Attributes, Contains.Item(AttributeConstants.OneTimeUse));
            Assert.That(staff.Attributes.Count(), Is.EqualTo(2));
            Assert.That(staff.Magic.Bonus, Is.EqualTo(42));
            Assert.That(staff.Magic.Charges, Is.EqualTo(9266));
            Assert.That(staff.BaseNames, Is.EqualTo(baseNames));
            Assert.That(staff, Is.Not.InstanceOf<Weapon>());
        }

        [Test]
        public void GenerateFromName_WithTraits()
        {
            var tableName = TableNameConstants.Percentiles.POWERITEMTYPEs(power, ItemTypeConstants.Staff);
            mockTypeAndAmountPercentileSelector
                .Setup(s => s.SelectAllFrom(Config.Name, tableName))
                .Returns(
                [
                    new TypeAndAmountDataSelection { Type = "wrong staff", AmountAsDouble = 666 },
                    new TypeAndAmountDataSelection { Type = StaffConstants.Abjuration, AmountAsDouble = 42 },
                    new TypeAndAmountDataSelection { Type = "other staff", AmountAsDouble = 600 },
                ]);

            var baseNames = new[] { "base name", "other base name" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.ItemGroups, StaffConstants.Abjuration)).Returns(baseNames);

            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Staff, "wrong staff")).Returns(666);
            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Staff, StaffConstants.Abjuration)).Returns(9266);
            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Staff, "other staff")).Returns(90210);

            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(It.IsAny<IEnumerable<TypeAndAmountDataSelection>>()))
                .Returns((IEnumerable<TypeAndAmountDataSelection> c) => c.Last());

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, StaffConstants.Abjuration))
                .Returns(new[] { "wrong power", power, "other power" });

            var staff = staffGenerator.Generate(power, StaffConstants.Abjuration, "trait 1", "trait 2");
            Assert.That(staff.Name, Is.EqualTo(StaffConstants.Abjuration));
            Assert.That(staff.ItemType, Is.EqualTo(ItemTypeConstants.Staff));
            Assert.That(staff.Attributes, Contains.Item(AttributeConstants.Charged));
            Assert.That(staff.Attributes, Contains.Item(AttributeConstants.OneTimeUse));
            Assert.That(staff.Attributes.Count(), Is.EqualTo(2));
            Assert.That(staff.Magic.Bonus, Is.EqualTo(42));
            Assert.That(staff.Magic.Charges, Is.EqualTo(9266));
            Assert.That(staff.BaseNames, Is.EqualTo(baseNames));
            Assert.That(staff, Is.Not.InstanceOf<Weapon>());
            Assert.That(staff.Traits, Has.Count.EqualTo(2)
                .And.Contains("trait 1")
                .And.Contains("trait 2"));
        }

        [Test]
        public void GenerateStaffAsWeaponFromName()
        {
            var tableName = TableNameConstants.Percentiles.POWERITEMTYPEs(power, ItemTypeConstants.Staff);
            mockTypeAndAmountPercentileSelector
                .Setup(s => s.SelectAllFrom(Config.Name, tableName))
                .Returns(new[]
                {
                    new TypeAndAmountDataSelection { Type = "wrong staff", AmountAsDouble = 666 },
                    new TypeAndAmountDataSelection { Type = StaffConstants.Abjuration, AmountAsDouble = 42 },
                    new TypeAndAmountDataSelection { Type = "other staff", AmountAsDouble = 600 },
                });

            var baseNames = new[] { "base name", "other base name", WeaponConstants.Quarterstaff };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.ItemGroups, StaffConstants.Abjuration)).Returns(baseNames);

            var quarterstaff = itemVerifier.CreateRandomWeaponTemplate(WeaponConstants.Quarterstaff);
            mockMundaneWeaponGenerator.Setup(g => g.Generate(WeaponConstants.Quarterstaff)).Returns(quarterstaff);

            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Staff, "wrong staff")).Returns(666);
            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Staff, StaffConstants.Abjuration)).Returns(9266);
            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Staff, "other staff")).Returns(90210);

            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(It.IsAny<IEnumerable<TypeAndAmountDataSelection>>()))
                .Returns((IEnumerable<TypeAndAmountDataSelection> c) => c.Last());

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, StaffConstants.Abjuration))
                .Returns(new[] { "wrong power", power, "other power" });

            var staff = staffGenerator.Generate(power, StaffConstants.Abjuration);
            Assert.That(staff.Name, Is.EqualTo(StaffConstants.Abjuration));
            Assert.That(staff.ItemType, Is.EqualTo(ItemTypeConstants.Staff));
            Assert.That(staff.Attributes, Is.EquivalentTo(quarterstaff.Attributes.Union(new[] { AttributeConstants.Charged })));
            Assert.That(staff.Attributes, Is.All.Not.EqualTo(AttributeConstants.OneTimeUse));
            Assert.That(staff.Magic.Bonus, Is.EqualTo(42));
            Assert.That(staff.Magic.Charges, Is.EqualTo(9266));
            Assert.That(staff.BaseNames, Is.EqualTo(baseNames));
            Assert.That(staff, Is.InstanceOf<Weapon>());

            var weapon = staff as Weapon;
            Assert.That(weapon.Attributes, Is.SupersetOf(quarterstaff.Attributes));
            Assert.That(weapon.CriticalMultiplier, Is.EqualTo(quarterstaff.CriticalMultiplier));
            Assert.That(weapon.Damages, Is.EqualTo(quarterstaff.Damages));
            Assert.That(weapon.CriticalDamages, Is.EqualTo(quarterstaff.CriticalDamages));
            Assert.That(weapon.Size, Is.EqualTo(quarterstaff.Size));
            Assert.That(weapon.ThreatRangeSummary, Is.EqualTo(quarterstaff.ThreatRangeSummary));
            Assert.That(weapon.Traits, Contains.Item(TraitConstants.Masterwork));
            Assert.That(weapon.IsDoubleWeapon, Is.False);
            Assert.That(weapon.SecondaryMagicBonus, Is.Zero);
            Assert.That(weapon.SecondaryHasAbilities, Is.False);
        }

        [Test]
        public void GenerateStaffAsDoubleWeaponFromName()
        {
            var tableName = TableNameConstants.Percentiles.POWERITEMTYPEs(power, ItemTypeConstants.Staff);
            mockTypeAndAmountPercentileSelector
                .Setup(s => s.SelectAllFrom(Config.Name, tableName))
                .Returns(new[]
                {
                    new TypeAndAmountDataSelection { Type = "wrong staff", AmountAsDouble = 666 },
                    new TypeAndAmountDataSelection { Type = StaffConstants.Woodlands, AmountAsDouble = 42 },
                    new TypeAndAmountDataSelection { Type = "other staff", AmountAsDouble = 600 },
                });

            var baseNames = new[] { "base name", "other base name", WeaponConstants.Quarterstaff };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.ItemGroups, StaffConstants.Woodlands)).Returns(baseNames);

            var quarterstaff = itemVerifier.CreateRandomWeaponTemplate(WeaponConstants.Quarterstaff);
            quarterstaff.Attributes = quarterstaff.Attributes.Union(new[] { AttributeConstants.DoubleWeapon });

            mockMundaneWeaponGenerator.Setup(g => g.Generate(WeaponConstants.Quarterstaff)).Returns(quarterstaff);

            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Staff, "wrong staff")).Returns(666);
            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Staff, StaffConstants.Woodlands)).Returns(9266);
            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Staff, "other staff")).Returns(90210);

            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(It.IsAny<IEnumerable<TypeAndAmountDataSelection>>()))
                .Returns((IEnumerable<TypeAndAmountDataSelection> c) => c.Last());

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, StaffConstants.Woodlands))
                .Returns(new[] { "wrong power", power, "other power" });

            var staff = staffGenerator.Generate(power, StaffConstants.Woodlands);
            Assert.That(staff.Name, Is.EqualTo(StaffConstants.Woodlands));
            Assert.That(staff.ItemType, Is.EqualTo(ItemTypeConstants.Staff));
            Assert.That(staff.Attributes, Is.EquivalentTo(quarterstaff.Attributes.Union(new[] { AttributeConstants.Charged })));
            Assert.That(staff.Attributes, Is.All.Not.EqualTo(AttributeConstants.OneTimeUse));
            Assert.That(staff.Magic.Bonus, Is.EqualTo(42));
            Assert.That(staff.Magic.Charges, Is.EqualTo(9266));
            Assert.That(staff.BaseNames, Is.EqualTo(baseNames));
            Assert.That(staff, Is.InstanceOf<Weapon>());

            var weapon = staff as Weapon;
            Assert.That(weapon.Attributes, Is.SupersetOf(quarterstaff.Attributes));
            Assert.That(weapon.CriticalMultiplier, Is.EqualTo(quarterstaff.CriticalMultiplier));
            Assert.That(weapon.Damages, Is.EqualTo(quarterstaff.Damages));
            Assert.That(weapon.CriticalDamages, Is.EqualTo(quarterstaff.CriticalDamages));
            Assert.That(weapon.Size, Is.EqualTo(quarterstaff.Size));
            Assert.That(weapon.ThreatRangeSummary, Is.EqualTo(quarterstaff.ThreatRangeSummary));
            Assert.That(weapon.Traits, Contains.Item(TraitConstants.Masterwork));
            Assert.That(weapon.IsDoubleWeapon, Is.True);
            Assert.That(weapon.SecondaryMagicBonus, Is.EqualTo(42));
            Assert.That(weapon.SecondaryHasAbilities, Is.True);
        }

        [Test]
        public void GenerateStaffAsWeaponFromName_WithTraits()
        {
            var tableName = TableNameConstants.Percentiles.POWERITEMTYPEs(power, ItemTypeConstants.Staff);
            mockTypeAndAmountPercentileSelector
                .Setup(s => s.SelectAllFrom(Config.Name, tableName))
                .Returns(new[]
                {
                    new TypeAndAmountDataSelection { Type = "wrong staff", AmountAsDouble = 666 },
                    new TypeAndAmountDataSelection { Type = StaffConstants.Abjuration, AmountAsDouble = 42 },
                    new TypeAndAmountDataSelection { Type = "other staff", AmountAsDouble = 600 },
                });

            var baseNames = new[] { "base name", "other base name", WeaponConstants.Quarterstaff };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.ItemGroups, StaffConstants.Abjuration)).Returns(baseNames);

            var quarterstaff = itemVerifier.CreateRandomWeaponTemplate(WeaponConstants.Quarterstaff);
            quarterstaff.Traits.Clear();

            mockMundaneWeaponGenerator.Setup(g => g.Generate(WeaponConstants.Quarterstaff, "trait 1", "trait 2")).Returns(quarterstaff);

            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Staff, "wrong staff")).Returns(666);
            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Staff, StaffConstants.Abjuration)).Returns(9266);
            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Staff, "other staff")).Returns(90210);

            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(It.IsAny<IEnumerable<TypeAndAmountDataSelection>>()))
                .Returns((IEnumerable<TypeAndAmountDataSelection> c) => c.Last());

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, StaffConstants.Abjuration))
                .Returns(new[] { "wrong power", power, "other power" });

            var staff = staffGenerator.Generate(power, StaffConstants.Abjuration, "trait 1", "trait 2");
            Assert.That(staff.Name, Is.EqualTo(StaffConstants.Abjuration));
            Assert.That(staff.ItemType, Is.EqualTo(ItemTypeConstants.Staff));
            Assert.That(staff.Attributes, Is.EquivalentTo(quarterstaff.Attributes.Union(new[] { AttributeConstants.Charged })));
            Assert.That(staff.Attributes, Is.All.Not.EqualTo(AttributeConstants.OneTimeUse));
            Assert.That(staff.Magic.Bonus, Is.EqualTo(42));
            Assert.That(staff.Magic.Charges, Is.EqualTo(9266));
            Assert.That(staff.BaseNames, Is.EqualTo(baseNames));
            Assert.That(staff, Is.InstanceOf<Weapon>());
            Assert.That(staff.Traits, Has.Count.EqualTo(3)
                .And.Contains("trait 1")
                .And.Contains("trait 2")
                .And.Contains(TraitConstants.Masterwork));

            var weapon = staff as Weapon;
            Assert.That(weapon.Attributes, Is.SupersetOf(quarterstaff.Attributes));
            Assert.That(weapon.CriticalMultiplier, Is.EqualTo(quarterstaff.CriticalMultiplier));
            Assert.That(weapon.Damages, Is.EqualTo(quarterstaff.Damages));
            Assert.That(weapon.CriticalDamages, Is.EqualTo(quarterstaff.CriticalDamages));
            Assert.That(weapon.Size, Is.EqualTo(quarterstaff.Size));
            Assert.That(weapon.ThreatRangeSummary, Is.EqualTo(quarterstaff.ThreatRangeSummary));
        }

        [Test]
        public void GenerateStaffAsWeaponFromBaseName()
        {
            var tableName = TableNameConstants.Percentiles.POWERITEMTYPEs(power, ItemTypeConstants.Staff);
            mockTypeAndAmountPercentileSelector
                .Setup(s => s.SelectAllFrom(Config.Name, tableName))
                .Returns(new[]
                {
                    new TypeAndAmountDataSelection { Type = "wrong staff", AmountAsDouble = 666 },
                    new TypeAndAmountDataSelection { Type = StaffConstants.Abjuration, AmountAsDouble = 42 },
                    new TypeAndAmountDataSelection { Type = "other staff", AmountAsDouble = 600 },
                });

            var baseNames = new[] { "base name", "other base name", WeaponConstants.Quarterstaff };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.ItemGroups, StaffConstants.Abjuration)).Returns(baseNames);

            var quarterstaff = itemVerifier.CreateRandomWeaponTemplate(WeaponConstants.Quarterstaff);
            mockMundaneWeaponGenerator.Setup(g => g.Generate(WeaponConstants.Quarterstaff)).Returns(quarterstaff);

            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Staff, "wrong staff")).Returns(666);
            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Staff, StaffConstants.Abjuration)).Returns(9266);
            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Staff, "other staff")).Returns(90210);

            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(It.IsAny<IEnumerable<TypeAndAmountDataSelection>>()))
                .Returns((IEnumerable<TypeAndAmountDataSelection> c) => c.Last());

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, StaffConstants.Abjuration))
                .Returns(new[] { "wrong power", power, "other power" });

            var staffs = StaffConstants.GetAllStaffs();
            mockCollectionsSelector
                .Setup(s => s.FindCollectionOf(
                    Config.Name,
                    TableNameConstants.Collections.ItemGroups,
                    WeaponConstants.Quarterstaff,
                    It.Is<string[]>(cn => cn.IsEquivalent(staffs))))
                .Returns(StaffConstants.Abjuration);

            var staff = staffGenerator.Generate(power, WeaponConstants.Quarterstaff);
            Assert.That(staff.Name, Is.EqualTo(StaffConstants.Abjuration));
            Assert.That(staff.ItemType, Is.EqualTo(ItemTypeConstants.Staff));
            Assert.That(staff.Attributes, Is.EquivalentTo(quarterstaff.Attributes.Union(new[] { AttributeConstants.Charged })));
            Assert.That(staff.Attributes, Is.All.Not.EqualTo(AttributeConstants.OneTimeUse));
            Assert.That(staff.Magic.Bonus, Is.EqualTo(42));
            Assert.That(staff.Magic.Charges, Is.EqualTo(9266));
            Assert.That(staff.BaseNames, Is.EqualTo(baseNames));
            Assert.That(staff, Is.InstanceOf<Weapon>());

            var weapon = staff as Weapon;
            Assert.That(weapon.Attributes, Is.SupersetOf(quarterstaff.Attributes));
            Assert.That(weapon.CriticalMultiplier, Is.EqualTo(quarterstaff.CriticalMultiplier));
            Assert.That(weapon.Damages, Is.EqualTo(quarterstaff.Damages));
            Assert.That(weapon.CriticalDamages, Is.EqualTo(quarterstaff.CriticalDamages));
            Assert.That(weapon.Size, Is.EqualTo(quarterstaff.Size));
            Assert.That(weapon.ThreatRangeSummary, Is.EqualTo(quarterstaff.ThreatRangeSummary));
            Assert.That(weapon.Traits, Contains.Item(TraitConstants.Masterwork));
        }

        [Test]
        public void MinorPowerFromNameAdjustsPower()
        {
            var tableName = TableNameConstants.Percentiles.POWERITEMTYPEs(power, ItemTypeConstants.Staff);
            mockTypeAndAmountPercentileSelector
                .Setup(s => s.SelectAllFrom(Config.Name, tableName))
                .Returns(new[]
                {
                    new TypeAndAmountDataSelection { Type = "wrong staff", AmountAsDouble = 666 },
                    new TypeAndAmountDataSelection { Type = StaffConstants.Abjuration, AmountAsDouble = 42 },
                    new TypeAndAmountDataSelection { Type = "other staff", AmountAsDouble = 600 },
                });

            var baseNames = new[] { "base name", "other base name" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.ItemGroups, StaffConstants.Abjuration)).Returns(baseNames);

            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Staff, "wrong staff")).Returns(666);
            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Staff, StaffConstants.Abjuration)).Returns(9266);
            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Staff, "other staff")).Returns(90210);

            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(It.IsAny<IEnumerable<TypeAndAmountDataSelection>>()))
                .Returns((IEnumerable<TypeAndAmountDataSelection> c) => c.Last());

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, StaffConstants.Abjuration))
                .Returns(new[] { power, "wrong power" });

            var staff = staffGenerator.Generate(PowerConstants.Minor, StaffConstants.Abjuration);
            Assert.That(staff.Name, Is.EqualTo(StaffConstants.Abjuration));
            Assert.That(staff.ItemType, Is.EqualTo(ItemTypeConstants.Staff));
            Assert.That(staff.Attributes, Contains.Item(AttributeConstants.Charged));
            Assert.That(staff.Attributes, Contains.Item(AttributeConstants.OneTimeUse));
            Assert.That(staff.Attributes.Count(), Is.EqualTo(2));
            Assert.That(staff.Magic.Bonus, Is.EqualTo(42));
            Assert.That(staff.Magic.Charges, Is.EqualTo(9266));
            Assert.That(staff.BaseNames, Is.EqualTo(baseNames));
            Assert.That(staff, Is.Not.InstanceOf<Weapon>());
        }

        [Test]
        public void GenerateFromName_MultipleOfPower()
        {
            var tableName = TableNameConstants.Percentiles.POWERITEMTYPEs(power, ItemTypeConstants.Staff);
            mockTypeAndAmountPercentileSelector
                .Setup(s => s.SelectAllFrom(Config.Name, tableName))
                .Returns(new[]
                {
                    new TypeAndAmountDataSelection { Type = "wrong staff", AmountAsDouble = 666 },
                    new TypeAndAmountDataSelection { Type = StaffConstants.Abjuration, AmountAsDouble = 42 },
                    new TypeAndAmountDataSelection { Type = StaffConstants.Abjuration, AmountAsDouble = 600 },
                });

            var baseNames = new[] { "base name", "other base name" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.ItemGroups, StaffConstants.Abjuration)).Returns(baseNames);

            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Staff, "wrong staff")).Returns(666);
            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Staff, StaffConstants.Abjuration)).Returns(9266);
            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Staff, "other staff")).Returns(90210);

            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(It.IsAny<IEnumerable<TypeAndAmountDataSelection>>()))
                .Returns((IEnumerable<TypeAndAmountDataSelection> c) => c.Last());

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, StaffConstants.Abjuration))
                .Returns(new[] { "wrong power", power, "other power" });

            var staff = staffGenerator.Generate(power, StaffConstants.Abjuration);
            Assert.That(staff.Name, Is.EqualTo(StaffConstants.Abjuration));
            Assert.That(staff.ItemType, Is.EqualTo(ItemTypeConstants.Staff));
            Assert.That(staff.Attributes, Contains.Item(AttributeConstants.Charged));
            Assert.That(staff.Attributes, Contains.Item(AttributeConstants.OneTimeUse));
            Assert.That(staff.Attributes.Count(), Is.EqualTo(2));
            Assert.That(staff.Magic.Bonus, Is.EqualTo(600));
            Assert.That(staff.Magic.Charges, Is.EqualTo(9266));
            Assert.That(staff.BaseNames, Is.EqualTo(baseNames));
            Assert.That(staff, Is.Not.InstanceOf<Weapon>());
        }

        [Test]
        public void GenerateFromName_NoneOfPower()
        {
            var tableName = TableNameConstants.Percentiles.POWERITEMTYPEs(power, ItemTypeConstants.Staff);
            mockTypeAndAmountPercentileSelector
                .Setup(s => s.SelectAllFrom(Config.Name, tableName))
                .Returns(new[]
                {
                    new TypeAndAmountDataSelection { Type = "wrong staff", AmountAsDouble = 666 },
                    new TypeAndAmountDataSelection { Type = StaffConstants.Abjuration, AmountAsDouble = 42 },
                    new TypeAndAmountDataSelection { Type = "other staff", AmountAsDouble = 600 },
                });

            var baseNames = new[] { "base name", "other base name" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.ItemGroups, StaffConstants.Abjuration)).Returns(baseNames);

            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Staff, "wrong staff")).Returns(666);
            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Staff, StaffConstants.Abjuration)).Returns(9266);
            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Staff, "other staff")).Returns(90210);

            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(It.IsAny<IEnumerable<TypeAndAmountDataSelection>>()))
                .Returns((IEnumerable<TypeAndAmountDataSelection> c) => c.Last());

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, StaffConstants.Abjuration))
                .Returns(new[] { power, "wrong power" });

            var staff = staffGenerator.Generate("other power", StaffConstants.Abjuration);
            Assert.That(staff.Name, Is.EqualTo(StaffConstants.Abjuration));
            Assert.That(staff.ItemType, Is.EqualTo(ItemTypeConstants.Staff));
            Assert.That(staff.Attributes, Contains.Item(AttributeConstants.Charged));
            Assert.That(staff.Attributes, Contains.Item(AttributeConstants.OneTimeUse));
            Assert.That(staff.Attributes.Count(), Is.EqualTo(2));
            Assert.That(staff.Magic.Bonus, Is.EqualTo(42));
            Assert.That(staff.Magic.Charges, Is.EqualTo(9266));
            Assert.That(staff.BaseNames, Is.EqualTo(baseNames));
            Assert.That(staff, Is.Not.InstanceOf<Weapon>());
        }
    }
}