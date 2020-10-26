﻿using DnDGen.Infrastructure.Generators;
using DnDGen.Infrastructure.Selectors.Collections;
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

namespace DnDGen.TreasureGen.Tests.Unit.Generators.Items.Magical
{
    [TestFixture]
    public class StaffGeneratorTests
    {
        private MagicalItemGenerator staffGenerator;
        private Mock<IChargesGenerator> mockChargesGenerator;
        private Mock<ITypeAndAmountPercentileSelector> mockTypeAndAmountPercentileSelector;
        private Mock<ICollectionSelector> mockCollectionsSelector;
        private string power;
        private ItemVerifier itemVerifier;
        private Mock<ISpecialAbilitiesGenerator> mockSpecialAbilitiesGenerator;
        private Mock<MundaneItemGenerator> mockMundaneWeaponGenerator;
        private TypeAndAmountSelection selection;

        [SetUp]
        public void Setup()
        {
            mockTypeAndAmountPercentileSelector = new Mock<ITypeAndAmountPercentileSelector>();
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
            selection = new TypeAndAmountSelection();

            selection.Type = "staffiness";
            selection.Amount = 90210;

            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, power, ItemTypeConstants.Staff);
            mockTypeAndAmountPercentileSelector.Setup(s => s.SelectFrom(tableName)).Returns(selection);
            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Staff, selection.Type)).Returns(9266);
        }

        [Test]
        public void GenerateStaff()
        {
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.PowerGroups, ItemTypeConstants.Staff))
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
        public void AdjustMinorPowerStaves()
        {
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.PowerGroups, ItemTypeConstants.Staff))
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
        public void GetBaseNames()
        {
            var baseNames = new[] { "base name", "other base name" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, selection.Type)).Returns(baseNames);

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.PowerGroups, ItemTypeConstants.Staff))
                .Returns(new[] { "wrong power", power, "other power" });

            var staff = staffGenerator.GenerateRandom(power);
            Assert.That(staff.BaseNames, Is.EqualTo(baseNames));
        }

        [Test]
        public void GetStaffThatIsAlsoWeapon()
        {
            var baseNames = new[] { "base name", "other base name", WeaponConstants.Dagger };
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, selection.Type)).Returns(baseNames);

            var mundaneWeapon = itemVerifier.CreateRandomWeaponTemplate(WeaponConstants.Dagger);
            mockMundaneWeaponGenerator.Setup(g => g.Generate(WeaponConstants.Dagger)).Returns(mundaneWeapon);

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.PowerGroups, ItemTypeConstants.Staff))
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
            Assert.That(weapon.Damage, Is.EqualTo(mundaneWeapon.Damage));
            Assert.That(weapon.DamageType, Is.EqualTo(mundaneWeapon.DamageType));
            Assert.That(weapon.Size, Is.EqualTo(mundaneWeapon.Size));
            Assert.That(weapon.ThreatRangeDescription, Is.EqualTo(mundaneWeapon.ThreatRangeDescription));
            Assert.That(weapon.Traits, Contains.Item(TraitConstants.Masterwork));
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

            mockSpecialAbilitiesGenerator.Setup(p => p.GenerateFor(template.Magic.SpecialAbilities)).Returns(abilities);

            var baseNames = new[] { "base name", "other base name" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, name)).Returns(baseNames);

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

            var specialAbilityNames = template.Magic.SpecialAbilities.Select(a => a.Name);

            var abilities = new[]
            {
                new SpecialAbility { Name = specialAbilityNames.First() },
                new SpecialAbility { Name = specialAbilityNames.Last() }
            };

            mockSpecialAbilitiesGenerator.Setup(p => p.GenerateFor(template.Magic.SpecialAbilities)).Returns(abilities);
            var baseNames = new[] { "base name", "other base name", WeaponConstants.Dagger };
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, name)).Returns(baseNames);

            var mundaneWeapon = itemVerifier.CreateRandomWeaponTemplate(WeaponConstants.Dagger);
            mundaneWeapon.Traits.Clear();
            mundaneWeapon.Quantity = 1;

            mockMundaneWeaponGenerator
                .Setup(g => g.Generate(WeaponConstants.Dagger))
                .Returns(mundaneWeapon);

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
            Assert.That(weapon.Damage, Is.EqualTo(mundaneWeapon.Damage));
            Assert.That(weapon.DamageType, Is.EqualTo(mundaneWeapon.DamageType));
            Assert.That(weapon.Size, Is.EqualTo(mundaneWeapon.Size));
            Assert.That(weapon.ThreatRangeDescription, Is.EqualTo(mundaneWeapon.ThreatRangeDescription));
            Assert.That(weapon.Traits, Contains.Item(TraitConstants.Masterwork).And.Count.EqualTo(1));
        }

        [Test]
        public void GenerateCustomStaffThatIsAlsoAWeapon_WithTraits()
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
            var baseNames = new[] { "base name", "other base name", WeaponConstants.Dagger };
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, name)).Returns(baseNames);

            var mundaneWeapon = itemVerifier.CreateRandomWeaponTemplate(WeaponConstants.Dagger);
            mundaneWeapon.Traits.Clear();
            mundaneWeapon.Quantity = 1;

            mockMundaneWeaponGenerator
                .Setup(g => g.Generate(WeaponConstants.Dagger, template.Traits.First(), template.Traits.Last()))
                .Returns(mundaneWeapon);

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
            Assert.That(weapon.Damage, Is.EqualTo(mundaneWeapon.Damage));
            Assert.That(weapon.DamageType, Is.EqualTo(mundaneWeapon.DamageType));
            Assert.That(weapon.Size, Is.EqualTo(mundaneWeapon.Size));
            Assert.That(weapon.ThreatRangeDescription, Is.EqualTo(mundaneWeapon.ThreatRangeDescription));
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

            mockSpecialAbilitiesGenerator.Setup(p => p.GenerateFor(template.Magic.SpecialAbilities)).Returns(abilities);

            var baseNames = new[] { "base name", "other base name" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, name)).Returns(baseNames);

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
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, power, ItemTypeConstants.Staff);
            mockTypeAndAmountPercentileSelector
                .Setup(s => s.SelectAllFrom(tableName))
                .Returns(new[]
                {
                    new TypeAndAmountSelection { Type = "wrong staff", Amount = 666 },
                    new TypeAndAmountSelection { Type = staffName, Amount = 42 },
                    new TypeAndAmountSelection { Type = "other staff", Amount = 600 },
                });

            var baseNames = new[] { "base name", "other base name" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, staffName)).Returns(baseNames);

            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Staff, "wrong staff")).Returns(666);
            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Staff, staffName)).Returns(9266);
            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Staff, "other staff")).Returns(90210);

            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(It.IsAny<IEnumerable<TypeAndAmountSelection>>()))
                .Returns((IEnumerable<TypeAndAmountSelection> c) => c.Last());

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.PowerGroups, staffName))
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
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, power, ItemTypeConstants.Staff);
            mockTypeAndAmountPercentileSelector
                .Setup(s => s.SelectAllFrom(tableName))
                .Returns(new[]
                {
                    new TypeAndAmountSelection { Type = "wrong staff", Amount = 666 },
                    new TypeAndAmountSelection { Type = StaffConstants.Abjuration, Amount = 42 },
                    new TypeAndAmountSelection { Type = "other staff", Amount = 600 },
                });

            var baseNames = new[] { "base name", "other base name" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, StaffConstants.Abjuration)).Returns(baseNames);

            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Staff, "wrong staff")).Returns(666);
            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Staff, StaffConstants.Abjuration)).Returns(9266);
            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Staff, "other staff")).Returns(90210);

            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(It.IsAny<IEnumerable<TypeAndAmountSelection>>()))
                .Returns((IEnumerable<TypeAndAmountSelection> c) => c.Last());

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.PowerGroups, StaffConstants.Abjuration))
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
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, power, ItemTypeConstants.Staff);
            mockTypeAndAmountPercentileSelector
                .Setup(s => s.SelectAllFrom(tableName))
                .Returns(new[]
                {
                    new TypeAndAmountSelection { Type = "wrong staff", Amount = 666 },
                    new TypeAndAmountSelection { Type = StaffConstants.Abjuration, Amount = 42 },
                    new TypeAndAmountSelection { Type = "other staff", Amount = 600 },
                });

            var baseNames = new[] { "base name", "other base name", WeaponConstants.Quarterstaff };
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, StaffConstants.Abjuration)).Returns(baseNames);

            var quarterstaff = itemVerifier.CreateRandomWeaponTemplate(WeaponConstants.Quarterstaff);
            mockMundaneWeaponGenerator.Setup(g => g.Generate(WeaponConstants.Quarterstaff)).Returns(quarterstaff);

            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Staff, "wrong staff")).Returns(666);
            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Staff, StaffConstants.Abjuration)).Returns(9266);
            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Staff, "other staff")).Returns(90210);

            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(It.IsAny<IEnumerable<TypeAndAmountSelection>>()))
                .Returns((IEnumerable<TypeAndAmountSelection> c) => c.Last());

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.PowerGroups, StaffConstants.Abjuration))
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
            Assert.That(weapon.Damage, Is.EqualTo(quarterstaff.Damage));
            Assert.That(weapon.DamageType, Is.EqualTo(quarterstaff.DamageType));
            Assert.That(weapon.Size, Is.EqualTo(quarterstaff.Size));
            Assert.That(weapon.ThreatRangeDescription, Is.EqualTo(quarterstaff.ThreatRangeDescription));
            Assert.That(weapon.Traits, Contains.Item(TraitConstants.Masterwork));
        }

        [Test]
        public void GenerateStaffAsWeaponFromName_WithTraits()
        {
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, power, ItemTypeConstants.Staff);
            mockTypeAndAmountPercentileSelector
                .Setup(s => s.SelectAllFrom(tableName))
                .Returns(new[]
                {
                    new TypeAndAmountSelection { Type = "wrong staff", Amount = 666 },
                    new TypeAndAmountSelection { Type = StaffConstants.Abjuration, Amount = 42 },
                    new TypeAndAmountSelection { Type = "other staff", Amount = 600 },
                });

            var baseNames = new[] { "base name", "other base name", WeaponConstants.Quarterstaff };
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, StaffConstants.Abjuration)).Returns(baseNames);

            var quarterstaff = itemVerifier.CreateRandomWeaponTemplate(WeaponConstants.Quarterstaff);
            quarterstaff.Traits.Clear();

            mockMundaneWeaponGenerator.Setup(g => g.Generate(WeaponConstants.Quarterstaff, "trait 1", "trait 2")).Returns(quarterstaff);

            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Staff, "wrong staff")).Returns(666);
            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Staff, StaffConstants.Abjuration)).Returns(9266);
            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Staff, "other staff")).Returns(90210);

            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(It.IsAny<IEnumerable<TypeAndAmountSelection>>()))
                .Returns((IEnumerable<TypeAndAmountSelection> c) => c.Last());

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.PowerGroups, StaffConstants.Abjuration))
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
            Assert.That(weapon.Damage, Is.EqualTo(quarterstaff.Damage));
            Assert.That(weapon.DamageType, Is.EqualTo(quarterstaff.DamageType));
            Assert.That(weapon.Size, Is.EqualTo(quarterstaff.Size));
            Assert.That(weapon.ThreatRangeDescription, Is.EqualTo(quarterstaff.ThreatRangeDescription));
        }

        [Test]
        public void GenerateStaffAsWeaponFromBaseName()
        {
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, power, ItemTypeConstants.Staff);
            mockTypeAndAmountPercentileSelector
                .Setup(s => s.SelectAllFrom(tableName))
                .Returns(new[]
                {
                    new TypeAndAmountSelection { Type = "wrong staff", Amount = 666 },
                    new TypeAndAmountSelection { Type = StaffConstants.Abjuration, Amount = 42 },
                    new TypeAndAmountSelection { Type = "other staff", Amount = 600 },
                });

            var baseNames = new[] { "base name", "other base name", WeaponConstants.Quarterstaff };
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, StaffConstants.Abjuration)).Returns(baseNames);

            var quarterstaff = itemVerifier.CreateRandomWeaponTemplate(WeaponConstants.Quarterstaff);
            mockMundaneWeaponGenerator.Setup(g => g.Generate(WeaponConstants.Quarterstaff)).Returns(quarterstaff);

            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Staff, "wrong staff")).Returns(666);
            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Staff, StaffConstants.Abjuration)).Returns(9266);
            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Staff, "other staff")).Returns(90210);

            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(It.IsAny<IEnumerable<TypeAndAmountSelection>>()))
                .Returns((IEnumerable<TypeAndAmountSelection> c) => c.Last());

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.PowerGroups, StaffConstants.Abjuration))
                .Returns(new[] { "wrong power", power, "other power" });

            var staffs = StaffConstants.GetAllStaffs();
            mockCollectionsSelector
                .Setup(s => s.FindCollectionOf(
                    TableNameConstants.Collections.Set.ItemGroups,
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
            Assert.That(weapon.Damage, Is.EqualTo(quarterstaff.Damage));
            Assert.That(weapon.DamageType, Is.EqualTo(quarterstaff.DamageType));
            Assert.That(weapon.Size, Is.EqualTo(quarterstaff.Size));
            Assert.That(weapon.ThreatRangeDescription, Is.EqualTo(quarterstaff.ThreatRangeDescription));
            Assert.That(weapon.Traits, Contains.Item(TraitConstants.Masterwork));
        }

        [Test]
        public void MinorPowerFromNameAdjustsPower()
        {
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, power, ItemTypeConstants.Staff);
            mockTypeAndAmountPercentileSelector
                .Setup(s => s.SelectAllFrom(tableName))
                .Returns(new[]
                {
                    new TypeAndAmountSelection { Type = "wrong staff", Amount = 666 },
                    new TypeAndAmountSelection { Type = StaffConstants.Abjuration, Amount = 42 },
                    new TypeAndAmountSelection { Type = "other staff", Amount = 600 },
                });

            var baseNames = new[] { "base name", "other base name" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, StaffConstants.Abjuration)).Returns(baseNames);

            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Staff, "wrong staff")).Returns(666);
            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Staff, StaffConstants.Abjuration)).Returns(9266);
            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Staff, "other staff")).Returns(90210);

            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(It.IsAny<IEnumerable<TypeAndAmountSelection>>()))
                .Returns((IEnumerable<TypeAndAmountSelection> c) => c.Last());

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.PowerGroups, StaffConstants.Abjuration))
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
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, power, ItemTypeConstants.Staff);
            mockTypeAndAmountPercentileSelector
                .Setup(s => s.SelectAllFrom(tableName))
                .Returns(new[]
                {
                    new TypeAndAmountSelection { Type = "wrong staff", Amount = 666 },
                    new TypeAndAmountSelection { Type = StaffConstants.Abjuration, Amount = 42 },
                    new TypeAndAmountSelection { Type = StaffConstants.Abjuration, Amount = 600 },
                });

            var baseNames = new[] { "base name", "other base name" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, StaffConstants.Abjuration)).Returns(baseNames);

            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Staff, "wrong staff")).Returns(666);
            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Staff, StaffConstants.Abjuration)).Returns(9266);
            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Staff, "other staff")).Returns(90210);

            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(It.IsAny<IEnumerable<TypeAndAmountSelection>>()))
                .Returns((IEnumerable<TypeAndAmountSelection> c) => c.Last());

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.PowerGroups, StaffConstants.Abjuration))
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
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, power, ItemTypeConstants.Staff);
            mockTypeAndAmountPercentileSelector
                .Setup(s => s.SelectAllFrom(tableName))
                .Returns(new[]
                {
                    new TypeAndAmountSelection { Type = "wrong staff", Amount = 666 },
                    new TypeAndAmountSelection { Type = StaffConstants.Abjuration, Amount = 42 },
                    new TypeAndAmountSelection { Type = "other staff", Amount = 600 },
                });

            var baseNames = new[] { "base name", "other base name" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.ItemGroups, StaffConstants.Abjuration)).Returns(baseNames);

            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Staff, "wrong staff")).Returns(666);
            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Staff, StaffConstants.Abjuration)).Returns(9266);
            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Staff, "other staff")).Returns(90210);

            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(It.IsAny<IEnumerable<TypeAndAmountSelection>>()))
                .Returns((IEnumerable<TypeAndAmountSelection> c) => c.Last());

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(TableNameConstants.Collections.Set.PowerGroups, StaffConstants.Abjuration))
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