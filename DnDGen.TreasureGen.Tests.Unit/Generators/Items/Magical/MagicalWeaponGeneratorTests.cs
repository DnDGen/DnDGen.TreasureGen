using DnDGen.Infrastructure.Factories;
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
    public class MagicalWeaponGeneratorTests
    {
        private MagicalItemGenerator magicalWeaponGenerator;
        private Mock<ITreasurePercentileSelector> mockPercentileSelector;
        private Mock<ICollectionSelector> mockCollectionsSelector;
        private Mock<ISpecialAbilitiesGenerator> mockSpecialAbilitiesGenerator;
        private Mock<ISpecificGearGenerator> mockSpecificGearGenerator;
        private Mock<MundaneItemGenerator> mockMundaneWeaponGenerator;
        private string power;
        private ItemVerifier itemVerifier;
        private Weapon mundaneWeapon;

        [SetUp]
        public void Setup()
        {
            mockPercentileSelector = new Mock<ITreasurePercentileSelector>();
            mockCollectionsSelector = new Mock<ICollectionSelector>();
            mockSpecialAbilitiesGenerator = new Mock<ISpecialAbilitiesGenerator>();
            mockSpecificGearGenerator = new Mock<ISpecificGearGenerator>();
            mockMundaneWeaponGenerator = new Mock<MundaneItemGenerator>();
            var mockJustInTimeFactory = new Mock<JustInTimeFactory>();
            mockJustInTimeFactory.Setup(f => f.Build<MundaneItemGenerator>(ItemTypeConstants.Weapon)).Returns(mockMundaneWeaponGenerator.Object);

            magicalWeaponGenerator = new MagicalWeaponGenerator(
                mockCollectionsSelector.Object,
                mockPercentileSelector.Object,
                mockSpecialAbilitiesGenerator.Object,
                mockSpecificGearGenerator.Object,
                mockJustInTimeFactory.Object);

            itemVerifier = new ItemVerifier();

            power = "power";
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.MagicalWeaponTypes)).Returns("weapon type");
            var tableName = TableNameConstants.Percentiles.WEAPONTYPEWeapons("weapon type");
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, tableName)).Returns("weapon name");

            mundaneWeapon = new Weapon
            {
                Name = "weapon name",
                Quantity = 600,
                Ammunition = "ammo",
                CriticalMultiplier = "crit"
            };
            mundaneWeapon.Damages.Add(new Damage { Roll = "hurty mchurtface", Type = "spiritual" });
            mundaneWeapon.CriticalDamages.Add(new Damage { Roll = "hurty mcSUPERhurtface", Type = "spiritual" });
            mundaneWeapon.Size = "enormous";
            mundaneWeapon.ThreatRange = 96;
            mockMundaneWeaponGenerator.Setup(g => g.Generate(It.IsAny<string>())).Returns(new Weapon());
            mockMundaneWeaponGenerator.Setup(g => g.Generate(It.IsAny<Item>(), It.IsAny<bool>())).Returns(new Weapon());
            mockMundaneWeaponGenerator.Setup(g => g.Generate(It.Is<Item>(i => i.NameMatches("weapon name")), true)).Returns(mundaneWeapon);

            tableName = TableNameConstants.Percentiles.POWERITEMTYPEs(power, ItemTypeConstants.Weapon);
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, tableName)).Returns("9266");
            mockPercentileSelector.Setup(s => s.SelectAllFrom(Config.Name, tableName)).Returns(["9266", "90210", "42", MagicalWeaponGenerator.SpecialAbility, MagicalWeaponGenerator.SpecificWeapon]);

            mockSpecialAbilitiesGenerator.Setup(p => p.ApplyAbilitiesToWeapon(It.IsAny<Weapon>())).Returns((Weapon w) => w);
        }

        [Test]
        public void GenerateWeapon()
        {
            var item = magicalWeaponGenerator.GenerateRandom(power);
            Assert.That(item, Is.EqualTo(mundaneWeapon));
            Assert.That(item.Magic.Bonus, Is.EqualTo(9266));
            Assert.That(item.Quantity, Is.EqualTo(600));
            Assert.That(item.Traits, Contains.Item(TraitConstants.Masterwork));

            var weapon = item as Weapon;
            Assert.That(weapon.Ammunition, Is.EqualTo("ammo"));
            Assert.That(weapon.CriticalMultiplier, Is.EqualTo("crit"));
            Assert.That(weapon.Damages, Has.Count.EqualTo(1));
            Assert.That(weapon.Damages[0].Roll, Is.EqualTo("hurty mchurtface"));
            Assert.That(weapon.Damages[0].Type, Is.EqualTo("spiritual"));
            Assert.That(weapon.CriticalDamages, Has.Count.EqualTo(1));
            Assert.That(weapon.CriticalDamages[0].Roll, Is.EqualTo("hurty mcSUPERhurtface"));
            Assert.That(weapon.CriticalDamages[0].Type, Is.EqualTo("spiritual"));
            Assert.That(weapon.SecondaryCriticalMultiplier, Is.Empty);
            Assert.That(weapon.SecondaryDamages, Is.Empty);
            Assert.That(weapon.SecondaryCriticalDamages, Is.Empty);
            Assert.That(weapon.SecondaryMagicBonus, Is.Zero);
            Assert.That(weapon.SecondaryHasAbilities, Is.False);
            Assert.That(weapon.Size, Is.EqualTo("enormous"));
            Assert.That(weapon.ThreatRange, Is.EqualTo(96));
        }

        [Test]
        public void GenerateWeapon_DoubleWeapon_OtherHeadHasSameEnhancement()
        {
            mundaneWeapon.Attributes = new[] { "attribute 1", AttributeConstants.DoubleWeapon, "attribute 2" };
            mundaneWeapon.SecondaryCriticalMultiplier = "other crit";
            mundaneWeapon.SecondaryDamages.Add(new Damage { Roll = "jab", Type = "punch" });
            mundaneWeapon.SecondaryCriticalDamages.Add(new Damage { Roll = "cross", Type = "punch" });

            mockPercentileSelector.Setup(s => s.SelectFrom(.5)).Returns(true);

            var item = magicalWeaponGenerator.GenerateRandom(power);
            Assert.That(item, Is.EqualTo(mundaneWeapon));
            Assert.That(item.Magic.Bonus, Is.EqualTo(9266));
            Assert.That(item.Quantity, Is.EqualTo(600));
            Assert.That(item.Traits, Contains.Item(TraitConstants.Masterwork));

            var weapon = item as Weapon;
            Assert.That(weapon.Ammunition, Is.EqualTo("ammo"));
            Assert.That(weapon.CriticalMultiplier, Is.EqualTo("crit"));
            Assert.That(weapon.Damages, Has.Count.EqualTo(1));
            Assert.That(weapon.Damages[0].Roll, Is.EqualTo("hurty mchurtface"));
            Assert.That(weapon.Damages[0].Type, Is.EqualTo("spiritual"));
            Assert.That(weapon.CriticalDamages, Has.Count.EqualTo(1));
            Assert.That(weapon.CriticalDamages[0].Roll, Is.EqualTo("hurty mcSUPERhurtface"));
            Assert.That(weapon.CriticalDamages[0].Type, Is.EqualTo("spiritual"));
            Assert.That(weapon.SecondaryCriticalMultiplier, Is.EqualTo("other crit"));
            Assert.That(weapon.SecondaryDamages, Has.Count.EqualTo(1));
            Assert.That(weapon.SecondaryDamages[0].Roll, Is.EqualTo("jab"));
            Assert.That(weapon.SecondaryDamages[0].Type, Is.EqualTo("punch"));
            Assert.That(weapon.SecondaryCriticalDamages, Has.Count.EqualTo(1));
            Assert.That(weapon.SecondaryCriticalDamages[0].Roll, Is.EqualTo("cross"));
            Assert.That(weapon.SecondaryCriticalDamages[0].Type, Is.EqualTo("punch"));
            Assert.That(weapon.SecondaryMagicBonus, Is.EqualTo(9266));
            Assert.That(weapon.SecondaryHasAbilities, Is.True);
            Assert.That(weapon.Size, Is.EqualTo("enormous"));
            Assert.That(weapon.ThreatRange, Is.EqualTo(96));
        }

        [Test]
        public void GenerateWeapon_DoubleWeapon_OtherHeadHasLesserEnhancement()
        {
            mundaneWeapon.Attributes = new[] { "attribute 1", AttributeConstants.DoubleWeapon, "attribute 2" };
            mundaneWeapon.SecondaryCriticalMultiplier = "other crit";
            mundaneWeapon.SecondaryDamages.Add(new Damage { Roll = "jab", Type = "punch" });
            mundaneWeapon.SecondaryCriticalDamages.Add(new Damage { Roll = "cross", Type = "punch" });

            mockPercentileSelector.Setup(s => s.SelectFrom(.5)).Returns(false);

            var item = magicalWeaponGenerator.GenerateRandom(power);
            Assert.That(item, Is.EqualTo(mundaneWeapon));
            Assert.That(item.Magic.Bonus, Is.EqualTo(9266));
            Assert.That(item.Quantity, Is.EqualTo(600));
            Assert.That(item.Traits, Contains.Item(TraitConstants.Masterwork));

            var weapon = item as Weapon;
            Assert.That(weapon.Ammunition, Is.EqualTo("ammo"));
            Assert.That(weapon.CriticalMultiplier, Is.EqualTo("crit"));
            Assert.That(weapon.Damages, Has.Count.EqualTo(1));
            Assert.That(weapon.Damages[0].Roll, Is.EqualTo("hurty mchurtface"));
            Assert.That(weapon.Damages[0].Type, Is.EqualTo("spiritual"));
            Assert.That(weapon.CriticalDamages, Has.Count.EqualTo(1));
            Assert.That(weapon.CriticalDamages[0].Roll, Is.EqualTo("hurty mcSUPERhurtface"));
            Assert.That(weapon.CriticalDamages[0].Type, Is.EqualTo("spiritual"));
            Assert.That(weapon.SecondaryCriticalMultiplier, Is.EqualTo("other crit"));
            Assert.That(weapon.SecondaryDamages, Has.Count.EqualTo(1));
            Assert.That(weapon.SecondaryDamages[0].Roll, Is.EqualTo("jab"));
            Assert.That(weapon.SecondaryDamages[0].Type, Is.EqualTo("punch"));
            Assert.That(weapon.SecondaryCriticalDamages, Has.Count.EqualTo(1));
            Assert.That(weapon.SecondaryCriticalDamages[0].Roll, Is.EqualTo("cross"));
            Assert.That(weapon.SecondaryCriticalDamages[0].Type, Is.EqualTo("punch"));
            Assert.That(weapon.SecondaryMagicBonus, Is.EqualTo(9265));
            Assert.That(weapon.SecondaryHasAbilities, Is.False);
            Assert.That(weapon.Size, Is.EqualTo("enormous"));
            Assert.That(weapon.ThreatRange, Is.EqualTo(96));
        }

        [Test]
        public void GetSpecificWeaponFromGenerator()
        {
            var tableName = TableNameConstants.Percentiles.POWERITEMTYPEs(power, ItemTypeConstants.Weapon);
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, tableName)).Returns(MagicalWeaponGenerator.SpecificWeapon);

            var specificWeapon = new Weapon();
            specificWeapon.Name = Guid.NewGuid().ToString();
            specificWeapon.BaseNames = new[] { "base name 1", "base name 2" };

            mockSpecificGearGenerator.Setup(g => g.CanBeSpecific(power, ItemTypeConstants.Weapon, "weapon name")).Returns(true);
            mockSpecificGearGenerator.Setup(g => g.GenerateNameFrom(power, ItemTypeConstants.Weapon, "weapon name")).Returns(specificWeapon.Name);
            mockSpecificGearGenerator.Setup(g => g.GeneratePrototypeFrom(power, ItemTypeConstants.Weapon, specificWeapon.Name)).Returns(specificWeapon);
            mockSpecificGearGenerator.Setup(g => g.IsSpecific(It.Is<Item>(i => i.Name == specificWeapon.Name))).Returns(true);
            mockSpecificGearGenerator.Setup(g => g.GenerateFrom(It.Is<Item>(i => i.Name == specificWeapon.Name))).Returns(specificWeapon);

            mockMundaneWeaponGenerator.Setup(g => g.Generate("base name 1")).Returns(mundaneWeapon);

            var weapon = magicalWeaponGenerator.GenerateRandom(power);
            Assert.That(weapon, Is.EqualTo(specificWeapon));
        }

        [Test]
        public void BUG_GetSpecificWeaponFromGenerator_HasQuantity()
        {
            var tableName = TableNameConstants.Percentiles.POWERITEMTYPEs(power, ItemTypeConstants.Weapon);
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, tableName)).Returns(MagicalWeaponGenerator.SpecificWeapon);

            var specificWeapon = new Weapon();
            specificWeapon.Name = Guid.NewGuid().ToString();
            specificWeapon.BaseNames = new[] { "base name 1", "base name 2" };

            mockSpecificGearGenerator.Setup(g => g.CanBeSpecific(power, ItemTypeConstants.Weapon, "weapon name")).Returns(true);
            mockSpecificGearGenerator.Setup(g => g.GenerateNameFrom(power, ItemTypeConstants.Weapon, "weapon name")).Returns(specificWeapon.Name);
            mockSpecificGearGenerator.Setup(g => g.GeneratePrototypeFrom(power, ItemTypeConstants.Weapon, specificWeapon.Name)).Returns(specificWeapon);
            mockSpecificGearGenerator.Setup(g => g.IsSpecific(It.Is<Item>(i => i.Name == specificWeapon.Name))).Returns(true);
            mockSpecificGearGenerator.Setup(g => g.GenerateFrom(It.Is<Item>(i => i.Name == specificWeapon.Name))).Returns(specificWeapon);

            var weaponForQuantity = new Weapon();
            weaponForQuantity.Name = "base name 1";
            weaponForQuantity.Quantity = 1336;
            mockMundaneWeaponGenerator.Setup(g => g.Generate("base name 1")).Returns(weaponForQuantity);

            var weapon = magicalWeaponGenerator.GenerateRandom(power);
            Assert.That(weapon, Is.EqualTo(specificWeapon));
            Assert.That(weapon.Quantity, Is.EqualTo(1336));
        }

        [Test]
        public void GetSpecialAbilitiesFromGenerator()
        {
            var tableName = TableNameConstants.Percentiles.POWERITEMTYPEs(power, ItemTypeConstants.Weapon);
            mockPercentileSelector.SetupSequence(p => p.SelectFrom(Config.Name, tableName)).Returns(MagicalWeaponGenerator.SpecialAbility).Returns(MagicalWeaponGenerator.SpecialAbility).Returns("9266");

            var abilities = new[] { new SpecialAbility() };
            mockSpecialAbilitiesGenerator.Setup(p => p.GenerateFor(mundaneWeapon, power, 2)).Returns(abilities);

            var weaponWithAbilities = new Weapon();
            weaponWithAbilities.Magic.SpecialAbilities = abilities;
            mockSpecialAbilitiesGenerator.Setup(g => g.ApplyAbilitiesToWeapon(mundaneWeapon)).Returns(weaponWithAbilities);

            var weapon = magicalWeaponGenerator.GenerateRandom(power);
            Assert.That(weapon, Is.EqualTo(weaponWithAbilities));
            Assert.That(weapon.Magic.SpecialAbilities, Is.EqualTo(abilities));
        }

        [Test]
        public void GenerateCustomWeapon()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);

            var specialAbilityNames = template.Magic.SpecialAbilities.Select(a => a.Name);
            var abilities = new[]
            {
                new SpecialAbility { Name = specialAbilityNames.First() },
                new SpecialAbility { Name = specialAbilityNames.Last() }
            };

            var random = itemVerifier.CreateRandomWeaponTemplate(name);
            random.Quantity = 1337;
            random.Attributes = ["type 1", "type 2"];

            mockSpecialAbilitiesGenerator.Setup(p => p.GenerateFor(template.Magic.SpecialAbilities, random.CriticalMultiplier)).Returns(abilities);

            var templateMundaneWeapon = random.MundaneClone();
            mockMundaneWeaponGenerator.Setup(g => g.Generate(It.Is<Item>(i => i.Name == name), false)).Returns(templateMundaneWeapon);

            var item = magicalWeaponGenerator.Generate(template);
            Assert.That(item, Is.EqualTo(templateMundaneWeapon));
            Assert.That(item.Quantity, Is.EqualTo(1337));
            Assert.That(item.Magic.Bonus, Is.EqualTo(template.Magic.Bonus));
            Assert.That(item.Magic.Charges, Is.EqualTo(template.Magic.Charges));
            Assert.That(item.Magic.Curse, Is.EqualTo(template.Magic.Curse));
            Assert.That(item.Magic.Intelligence.Ego, Is.EqualTo(template.Magic.Intelligence.Ego));
            Assert.That(item.Magic.Intelligence.Ego, Is.Positive);
            Assert.That(item.Magic.SpecialAbilities, Is.EquivalentTo(abilities));

            //INFO: Custom magic weapons should be masterwork
            Assert.That(item.Traits, Contains.Item(TraitConstants.Masterwork));

            var weapon = item as Weapon;
            Assert.That(weapon.Attributes, Is.SupersetOf(random.Attributes));
            Assert.That(weapon.CriticalMultiplier, Is.EqualTo(random.CriticalMultiplier));
            Assert.That(weapon.Damages.Select(d => d.Summary), Is.EqualTo(
                random.Damages.Select(d => d.Summary)));
            Assert.That(weapon.CriticalDamages.Select(d => d.Summary), Is.EqualTo(
                random.CriticalDamages.Select(d => d.Summary)));
            Assert.That(weapon.Size, Is.EqualTo(random.Size));
            Assert.That(weapon.ThreatRangeSummary, Is.EqualTo(random.ThreatRangeSummary));
            Assert.That(weapon.IsDoubleWeapon, Is.False);
            Assert.That(weapon.SecondaryMagicBonus, Is.Zero);
            Assert.That(weapon.SecondaryHasAbilities, Is.False);
            Assert.That(weapon.SecondaryDamages, Is.Empty);
            Assert.That(weapon.SecondaryCriticalDamages, Is.Empty);
        }

        [Test]
        public void GenerateCustomWeapon_WithAbilityDamages()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);

            var random = itemVerifier.CreateRandomWeaponTemplate(name);
            random.Quantity = 1337;
            random.Attributes = ["type 1", "type 2"];

            var templateMundaneWeapon = new Weapon();
            random.CloneInto(templateMundaneWeapon);
            mockMundaneWeaponGenerator.Setup(g => g.Generate(It.Is<Item>(i => i.Name == name), false)).Returns(templateMundaneWeapon);

            var specialAbilityNames = template.Magic.SpecialAbilities.Select(a => a.Name);
            var abilities = new[]
            {
                new SpecialAbility { Name = specialAbilityNames.First() },
                new SpecialAbility
                {
                    Name = specialAbilityNames.Last(),
                    Damages = [new() { Roll = "some more", Type = "physical" }],
                    CriticalDamages = [new() { Roll = "even more", Type = "chemical" }],
                }
            };

            mockSpecialAbilitiesGenerator.Setup(p => p.GenerateFor(template.Magic.SpecialAbilities, random.CriticalMultiplier)).Returns(abilities);

            var weaponWithAbilities = new Weapon();
            mockSpecialAbilitiesGenerator
                .Setup(g => g.ApplyAbilitiesToWeapon(It.Is<Weapon>(w => w.Magic.SpecialAbilities == abilities)))
                .Returns(weaponWithAbilities);

            var item = magicalWeaponGenerator.Generate(template);
            Assert.That(item, Is.EqualTo(weaponWithAbilities));
        }

        [Test]
        public void GenerateCustomWeapon_DoubleWeapon()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);

            var specialAbilityNames = template.Magic.SpecialAbilities.Select(a => a.Name);
            var abilities = new[]
            {
                new SpecialAbility { Name = specialAbilityNames.First() },
                new SpecialAbility { Name = specialAbilityNames.Last() }
            };

            var random = itemVerifier.CreateRandomWeaponTemplate(name);
            random.Quantity = 1337;
            random.Attributes = ["type 1", AttributeConstants.DoubleWeapon, "type 2"];
            random.SecondaryDamages.Add(new Damage { Roll = "a touch", Type = "secondary" });
            random.SecondaryCriticalDamages.Add(new Damage { Roll = "a lot", Type = "secondary" });
            random.SecondaryCriticalMultiplier = "sevenfold";

            mockSpecialAbilitiesGenerator.Setup(p => p.GenerateFor(template.Magic.SpecialAbilities, random.CriticalMultiplier)).Returns(abilities);

            var templateMundaneWeapon = random.MundaneClone();
            mockMundaneWeaponGenerator.Setup(g => g.Generate(It.Is<Item>(i => i.Name == name), false)).Returns(templateMundaneWeapon);

            var item = magicalWeaponGenerator.Generate(template);
            Assert.That(item, Is.EqualTo(templateMundaneWeapon));
            Assert.That(item.Quantity, Is.EqualTo(1337));
            Assert.That(item.Magic.Bonus, Is.EqualTo(template.Magic.Bonus));
            Assert.That(item.Magic.Charges, Is.EqualTo(template.Magic.Charges));
            Assert.That(item.Magic.Curse, Is.EqualTo(template.Magic.Curse));
            Assert.That(item.Magic.Intelligence.Ego, Is.EqualTo(template.Magic.Intelligence.Ego));
            Assert.That(item.Magic.Intelligence.Ego, Is.Positive);
            Assert.That(item.Magic.SpecialAbilities, Is.EquivalentTo(abilities));

            //INFO: Custom magic weapons should be masterwork
            Assert.That(item.Traits, Contains.Item(TraitConstants.Masterwork));

            var weapon = item as Weapon;
            Assert.That(weapon.Attributes, Is.SupersetOf(random.Attributes));
            Assert.That(weapon.CriticalMultiplier, Is.EqualTo(random.CriticalMultiplier));
            Assert.That(weapon.Damages.Select(d => d.Summary), Is.EqualTo(
                random.Damages.Select(d => d.Summary)));
            Assert.That(weapon.CriticalDamages.Select(d => d.Summary), Is.EqualTo(
                random.CriticalDamages.Select(d => d.Summary)));
            Assert.That(weapon.Size, Is.EqualTo(random.Size));
            Assert.That(weapon.ThreatRangeSummary, Is.EqualTo(random.ThreatRangeSummary));
            Assert.That(weapon.IsDoubleWeapon, Is.True);
            Assert.That(weapon.SecondaryHasAbilities, Is.True);
            Assert.That(weapon.SecondaryMagicBonus, Is.Positive.And.EqualTo(template.Magic.Bonus));
            Assert.That(weapon.SecondaryDamages.Select(d => d.Summary), Is.Not.Empty.And.EqualTo(
                random.SecondaryDamages.Select(d => d.Summary)));
            Assert.That(weapon.SecondaryCriticalDamages.Select(d => d.Summary), Is.Not.Empty.And.EqualTo(
                random.SecondaryCriticalDamages.Select(d => d.Summary)));
        }

        [Test]
        public void GenerateCustomWeapon_DoubleWeapon_WithAbilityDamages()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);

            var templateMundaneWeapon = itemVerifier.CreateRandomWeaponTemplate(name);
            templateMundaneWeapon.Quantity = 1337;
            templateMundaneWeapon.Attributes = ["type 1", AttributeConstants.DoubleWeapon, "type 2"];
            templateMundaneWeapon.SecondaryDamages.Add(new Damage { Roll = "a touch", Type = "secondary" });
            templateMundaneWeapon.SecondaryCriticalDamages.Add(new Damage { Roll = "a lot", Type = "secondary" });
            templateMundaneWeapon.SecondaryCriticalMultiplier = "sevenfold";

            mockMundaneWeaponGenerator.Setup(g => g.Generate(It.Is<Item>(i => i.Name == name), false)).Returns(templateMundaneWeapon);

            var specialAbilityNames = template.Magic.SpecialAbilities.Select(a => a.Name);
            var abilities = new[]
            {
                new SpecialAbility { Name = specialAbilityNames.First() },
                new SpecialAbility
                {
                    Name = specialAbilityNames.Last(),
                    Damages = [new() { Roll = "some more", Type = "physical" }],
                    CriticalDamages = [new() { Roll = "even more", Type = "chemical" }],
                }
            };

            mockSpecialAbilitiesGenerator.Setup(p => p.GenerateFor(template.Magic.SpecialAbilities, templateMundaneWeapon.CriticalMultiplier)).Returns(abilities);

            var weaponWithAbilities = new Weapon();
            mockSpecialAbilitiesGenerator
                .Setup(g => g.ApplyAbilitiesToWeapon(It.Is<Weapon>(w => w.Magic.SpecialAbilities == abilities)))
                .Returns(weaponWithAbilities);

            var item = magicalWeaponGenerator.Generate(template);
            Assert.That(item, Is.EqualTo(weaponWithAbilities));
        }

        [Test]
        public void GenerateRandomCustomWeapon()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);

            var specialAbilityNames = template.Magic.SpecialAbilities.Select(a => a.Name);
            var abilities = new[]
            {
                new SpecialAbility { Name = specialAbilityNames.First() },
                new SpecialAbility { Name = specialAbilityNames.Last() }
            };

            var templateMundaneWeapon = new Weapon
            {
                Name = name,
                Quantity = 1337,
                Attributes = ["type 1", "type 2"],
                CriticalMultiplier = "my crit",
            };
            mockMundaneWeaponGenerator.Setup(g => g.Generate(It.Is<Item>(i => i.Name == name), true)).Returns(templateMundaneWeapon);

            mockSpecialAbilitiesGenerator.Setup(p => p.GenerateFor(template.Magic.SpecialAbilities, templateMundaneWeapon.CriticalMultiplier)).Returns(abilities);

            var weapon = magicalWeaponGenerator.Generate(template, true);
            Assert.That(weapon, Is.EqualTo(templateMundaneWeapon));
            Assert.That(weapon.Quantity, Is.EqualTo(1337));
            Assert.That(weapon.Magic.Bonus, Is.EqualTo(template.Magic.Bonus));
            Assert.That(weapon.Magic.Charges, Is.EqualTo(template.Magic.Charges));
            Assert.That(weapon.Magic.Curse, Is.EqualTo(template.Magic.Curse));
            Assert.That(weapon.Magic.Intelligence.Ego, Is.EqualTo(template.Magic.Intelligence.Ego));
            Assert.That(weapon.Magic.Intelligence.Ego, Is.Positive);
            Assert.That(weapon.Magic.SpecialAbilities, Is.EquivalentTo(abilities));

            //INFO: Custom magic weapons should be masterwork
            Assert.That(weapon.Traits, Contains.Item(TraitConstants.Masterwork));
        }

        [Test]
        public void GenerateCustomAmmunition()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);

            var specialAbilityNames = template.Magic.SpecialAbilities.Select(a => a.Name);
            var abilities = new[]
            {
                new SpecialAbility { Name = specialAbilityNames.First() },
                new SpecialAbility { Name = specialAbilityNames.Last() }
            };

            var templateMundaneWeapon = new Weapon
            {
                Name = name,
                Quantity = 1337,
                Attributes = ["type 1", "type 2", AttributeConstants.Ammunition],
                CriticalMultiplier = "my crit",
            };
            mockMundaneWeaponGenerator.Setup(g => g.Generate(It.Is<Item>(i => i.Name == name), false)).Returns(templateMundaneWeapon);

            mockSpecialAbilitiesGenerator.Setup(p => p.GenerateFor(template.Magic.SpecialAbilities, "my crit")).Returns(abilities);

            var weapon = magicalWeaponGenerator.Generate(template);
            Assert.That(weapon, Is.EqualTo(templateMundaneWeapon));
            Assert.That(weapon.Quantity, Is.EqualTo(1337));
            Assert.That(weapon.Magic.Bonus, Is.EqualTo(template.Magic.Bonus));
            Assert.That(weapon.Magic.Charges, Is.EqualTo(template.Magic.Charges));
            Assert.That(weapon.Magic.Curse, Is.EqualTo(template.Magic.Curse));
            Assert.That(weapon.Magic.Intelligence, Is.Not.EqualTo(template.Magic.Intelligence));
            Assert.That(weapon.Magic.Intelligence.Ego, Is.EqualTo(0));
            Assert.That(weapon.Magic.SpecialAbilities, Is.EquivalentTo(abilities));

            //INFO: Custom magic weapons should be masterwork
            Assert.That(weapon.Traits, Contains.Item(TraitConstants.Masterwork));
        }

        [Test]
        public void GenerateSpecificCustomWeapon()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);

            var specificWeapon = itemVerifier.CreateRandomWeaponTemplate(name);
            specificWeapon.ItemType = ItemTypeConstants.Weapon;
            specificWeapon.Magic.SpecialAbilities = [new SpecialAbility(), new SpecialAbility()];
            specificWeapon.Attributes = ["type 1", AttributeConstants.Specific, "type 2"];

            mockSpecificGearGenerator.Setup(g => g.GenerateFrom(It.Is<Item>(i => i.Name == name))).Returns(specificWeapon);
            mockSpecificGearGenerator.Setup(g => g.IsSpecific(It.Is<Item>(i => i.Name == name))).Returns(true);

            specificWeapon.BaseNames = ["base name 1", "base name 2"];
            mockMundaneWeaponGenerator.Setup(g => g.Generate("base name 1")).Returns(mundaneWeapon);

            var weapon = magicalWeaponGenerator.Generate(template, true);
            Assert.That(weapon.Name, Is.EqualTo(specificWeapon.Name));
            Assert.That(weapon.BaseNames, Is.EquivalentTo(specificWeapon.BaseNames));
            Assert.That(weapon.Quantity, Is.EqualTo(specificWeapon.Quantity));
            Assert.That(weapon.Magic.SpecialAbilities, Is.EquivalentTo(specificWeapon.Magic.SpecialAbilities));
            Assert.That(weapon.Attributes, Is.EquivalentTo(specificWeapon.Attributes));
        }

        [Test]
        public void GenerateSpecificCustomWeapon_WithSetAbilities()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);

            var specificWeapon = itemVerifier.CreateRandomWeaponTemplate(name);
            specificWeapon.ItemType = ItemTypeConstants.Weapon;
            specificWeapon.Attributes = ["type 1", AttributeConstants.Specific, "type 2"];
            var specificAbilities = specificWeapon.Magic.SpecialAbilities.ToArray();

            mockSpecificGearGenerator.Setup(g => g.GenerateFrom(It.Is<Item>(i => i.Name == name))).Returns(specificWeapon);
            mockSpecificGearGenerator.Setup(g => g.IsSpecific(It.Is<Item>(i => i.Name == name))).Returns(true);

            var wrongAbilities = template.Magic.SpecialAbilities.Union(specificWeapon.Magic.SpecialAbilities).ToArray();
            mockSpecialAbilitiesGenerator.Setup(g => g.GenerateFor(template.Magic.SpecialAbilities, It.IsAny<string>())).Returns(wrongAbilities);

            specificWeapon.BaseNames = ["base name 1", "base name 2"];
            mockMundaneWeaponGenerator.Setup(g => g.Generate("base name 1")).Returns(mundaneWeapon);

            var weapon = magicalWeaponGenerator.Generate(template, true);
            Assert.That(weapon.Name, Is.EqualTo(specificWeapon.Name));
            Assert.That(weapon.BaseNames, Is.EquivalentTo(specificWeapon.BaseNames));
            Assert.That(weapon.Quantity, Is.EqualTo(specificWeapon.Quantity));
            Assert.That(weapon.Magic.SpecialAbilities, Is.EquivalentTo(specificAbilities)
                .And.Not.EquivalentTo(wrongAbilities));
            Assert.That(weapon.Attributes, Is.EquivalentTo(specificWeapon.Attributes));
        }

        [Test]
        public void GenerateSpecificCustomWeapon_WithSetAbilities_WhenGeneratedNameIsNotSpecific()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);

            var specificWeapon = itemVerifier.CreateRandomWeaponTemplate("mundane name");
            specificWeapon.ItemType = ItemTypeConstants.Weapon;
            specificWeapon.Attributes = ["type 1", AttributeConstants.Specific, "type 2"];
            var specificAbilities = specificWeapon.Magic.SpecialAbilities.ToArray();

            mockSpecificGearGenerator.Setup(g => g.GenerateFrom(It.Is<Item>(i => i.Name == name))).Returns(specificWeapon);
            mockSpecificGearGenerator.Setup(g => g.IsSpecific(It.Is<Item>(i => i.Name == name))).Returns(true);
            mockSpecificGearGenerator.Setup(g => g.IsSpecific(It.Is<Item>(i => i.Name == "mundane name"))).Returns(false);

            var wrongAbilities = template.Magic.SpecialAbilities.Union(specificWeapon.Magic.SpecialAbilities).ToArray();
            mockSpecialAbilitiesGenerator.Setup(g => g.GenerateFor(template.Magic.SpecialAbilities, It.IsAny<string>())).Returns(wrongAbilities);

            specificWeapon.BaseNames = ["base name 1", "base name 2"];
            mockMundaneWeaponGenerator.Setup(g => g.Generate("base name 1")).Returns(mundaneWeapon);

            var weapon = magicalWeaponGenerator.Generate(template, true);
            Assert.That(weapon.Name, Is.EqualTo(specificWeapon.Name));
            Assert.That(weapon.BaseNames, Is.EquivalentTo(specificWeapon.BaseNames));
            Assert.That(weapon.Quantity, Is.EqualTo(specificWeapon.Quantity));
            Assert.That(weapon.Magic.SpecialAbilities, Is.EquivalentTo(specificAbilities)
                .And.Not.EquivalentTo(wrongAbilities));
            Assert.That(weapon.Attributes, Is.EquivalentTo(specificWeapon.Attributes));
        }

        [Test]
        public void BUG_GenerateSpecificCustomWeapon_WithQuantity()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);

            var specificWeapon = itemVerifier.CreateRandomWeaponTemplate(name);
            specificWeapon.ItemType = ItemTypeConstants.Weapon;
            specificWeapon.Magic.SpecialAbilities = new[] { new SpecialAbility(), new SpecialAbility() };
            specificWeapon.Attributes = new[] { "type 1", AttributeConstants.Specific, "type 2" };
            specificWeapon.BaseNames = new[] { "base name 1", "base name 2" };

            mockSpecificGearGenerator.Setup(g => g.GenerateFrom(It.Is<Item>(i => i.Name == name))).Returns(specificWeapon);
            mockSpecificGearGenerator.Setup(g => g.IsSpecific(It.Is<Item>(i => i.Name == name))).Returns(true);

            var weaponForQuantity = new Weapon();
            weaponForQuantity.Name = "base name 1";
            weaponForQuantity.Quantity = 1336;
            mockMundaneWeaponGenerator.Setup(g => g.Generate("base name 1")).Returns(weaponForQuantity);

            var weapon = magicalWeaponGenerator.Generate(template, true);
            Assert.That(weapon.Name, Is.EqualTo(specificWeapon.Name));
            Assert.That(weapon.BaseNames, Is.EquivalentTo(specificWeapon.BaseNames));
            Assert.That(weapon.Quantity, Is.EqualTo(specificWeapon.Quantity));
            Assert.That(weapon.Magic.SpecialAbilities, Is.EquivalentTo(specificWeapon.Magic.SpecialAbilities));
            Assert.That(weapon.Attributes, Is.EquivalentTo(specificWeapon.Attributes));
            Assert.That(weapon.Quantity, Is.EqualTo(1336));
        }

        [Test]
        public void GenerateFromName()
        {
            var tableName = TableNameConstants.Percentiles.POWERITEMTYPEs(power, ItemTypeConstants.Weapon);
            mockPercentileSelector
                .SetupSequence(p => p.SelectFrom(Config.Name, tableName))
                .Returns(MagicalWeaponGenerator.SpecialAbility)
                .Returns(MagicalWeaponGenerator.SpecialAbility)
                .Returns("9266");

            var abilities = new[] { new SpecialAbility() };
            mockSpecialAbilitiesGenerator.Setup(p => p.GenerateFor(It.IsAny<Item>(), power, 2)).Returns(abilities);

            var weapon = magicalWeaponGenerator.Generate(power, "weapon name");
            Assert.That(weapon.Name, Is.EqualTo("weapon name"));
            Assert.That(weapon.Quantity, Is.EqualTo(600));
            Assert.That(weapon.Magic.Bonus, Is.EqualTo(9266));
            Assert.That(weapon.Magic.SpecialAbilities, Is.EqualTo(abilities));
            Assert.That(weapon, Is.EqualTo(mundaneWeapon));
        }

        [Test]
        public void GenerateFromName_WithTraits()
        {
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.MagicalWeaponTypes))
                .Returns("weapon type");

            var tableName = TableNameConstants.Percentiles.WEAPONTYPEWeapons("weapon type");
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, tableName)).Returns("weapon name");

            tableName = TableNameConstants.Percentiles.POWERITEMTYPEs(power, ItemTypeConstants.Weapon);
            mockPercentileSelector
                .SetupSequence(p => p.SelectFrom(Config.Name, tableName))
                .Returns(MagicalWeaponGenerator.SpecialAbility)
                .Returns(MagicalWeaponGenerator.SpecialAbility)
                .Returns("9266");

            var abilities = new[] { new SpecialAbility() };
            mockSpecialAbilitiesGenerator.Setup(p => p.GenerateFor(It.IsAny<Item>(), power, 2)).Returns(abilities);

            mockMundaneWeaponGenerator
                .Setup(g => g.Generate(It.Is<Item>(i => i.NameMatches("weapon name")), true))
                .Returns((Item t, bool d) => new Weapon
                {
                    Name = t.Name,
                    Traits = t.Traits,
                    BaseNames = new[] { "mundane base name" },
                });

            var weapon = magicalWeaponGenerator.Generate(power, "weapon name", "trait 1", "trait 2");
            Assert.That(weapon.Name, Is.EqualTo("weapon name"));
            Assert.That(weapon.BaseNames, Contains.Item("mundane base name"));
            Assert.That(weapon.BaseNames.Count(), Is.EqualTo(1));
            Assert.That(weapon.Magic.Bonus, Is.EqualTo(9266));
            Assert.That(weapon.Magic.SpecialAbilities, Is.EqualTo(abilities));
            Assert.That(weapon.Traits, Has.Count.EqualTo(3)
                .And.Contains("trait 1")
                .And.Contains("trait 2")
                .And.Contains(TraitConstants.Masterwork));
        }

        [Test]
        public void GenerateSpecificFromName()
        {
            var tableName = TableNameConstants.Percentiles.POWERITEMTYPEs(power, ItemTypeConstants.Weapon);
            mockPercentileSelector.SetupSequence(p => p.SelectFrom(Config.Name, tableName)).Returns(MagicalWeaponGenerator.SpecialAbility).Returns(MagicalWeaponGenerator.SpecialAbility).Returns(MagicalWeaponGenerator.SpecificWeapon);

            var specificWeapon = new Weapon
            {
                Name = "specific weapon name",
                BaseNames = ["base name", "other specific base name"]
            };
            mockSpecificGearGenerator.Setup(g => g.IsSpecific(ItemTypeConstants.Weapon, "specific weapon name")).Returns(true);
            mockSpecificGearGenerator.Setup(g => g.IsSpecific(It.Is<Item>(i => i.Name == "specific weapon name"))).Returns(true);
            mockSpecificGearGenerator.Setup(g => g.GeneratePrototypeFrom(power, ItemTypeConstants.Weapon, "specific weapon name")).Returns(specificWeapon);
            mockSpecificGearGenerator.Setup(g => g.GenerateFrom(It.Is<Item>(i => i.Name == "specific weapon name"))).Returns(specificWeapon);

            mockMundaneWeaponGenerator.Setup(g => g.Generate("base name")).Returns(mundaneWeapon);

            var weapon = magicalWeaponGenerator.Generate(power, "specific weapon name");
            Assert.That(weapon.Name, Is.EqualTo("specific weapon name"));
            Assert.That(weapon, Is.EqualTo(specificWeapon));
        }

        [Test]
        public void GenerateSpecificFromName_WithTraits()
        {
            var tableName = TableNameConstants.Percentiles.POWERITEMTYPEs(power, ItemTypeConstants.Weapon);
            mockPercentileSelector.SetupSequence(p => p.SelectFrom(Config.Name, tableName)).Returns(MagicalWeaponGenerator.SpecialAbility).Returns(MagicalWeaponGenerator.SpecialAbility).Returns(MagicalWeaponGenerator.SpecificWeapon);

            var specificWeapon = new Weapon();
            specificWeapon.Name = "specific weapon name";
            specificWeapon.BaseNames = new[] { "base name", "other specific base name" };
            mockSpecificGearGenerator.Setup(g => g.IsSpecific(ItemTypeConstants.Weapon, "specific weapon name")).Returns(true);
            mockSpecificGearGenerator.Setup(g => g.IsSpecific(It.Is<Item>(i => i.Name == "specific weapon name"))).Returns(true);
            mockSpecificGearGenerator
                .Setup(g => g.GeneratePrototypeFrom(power, ItemTypeConstants.Weapon, "specific weapon name", "trait 1", "trait 2"))
                .Returns(specificWeapon);
            mockSpecificGearGenerator.Setup(g => g.GenerateFrom(It.Is<Item>(i => i.Name == "specific weapon name"))).Returns(specificWeapon);

            mockMundaneWeaponGenerator.Setup(g => g.Generate("base name")).Returns(mundaneWeapon);

            var weapon = magicalWeaponGenerator.Generate(power, "specific weapon name", "trait 1", "trait 2");
            Assert.That(weapon.Name, Is.EqualTo("specific weapon name"));
            Assert.That(weapon, Is.EqualTo(specificWeapon));
        }

        [Test]
        public void BUG_GenerateSpecificFromName_WithQuantity()
        {
            var tableName = TableNameConstants.Percentiles.POWERITEMTYPEs(power, ItemTypeConstants.Weapon);
            mockPercentileSelector.SetupSequence(p => p.SelectFrom(Config.Name, tableName)).Returns(MagicalWeaponGenerator.SpecialAbility).Returns(MagicalWeaponGenerator.SpecialAbility).Returns(MagicalWeaponGenerator.SpecificWeapon);

            var specificWeapon = new Weapon();
            specificWeapon.Name = "specific weapon name";
            specificWeapon.BaseNames = new[] { "base name", "other specific base name" };

            mockSpecificGearGenerator.Setup(g => g.IsSpecific(ItemTypeConstants.Weapon, "specific weapon name")).Returns(true);
            mockSpecificGearGenerator.Setup(g => g.IsSpecific(It.Is<Item>(i => i.Name == "specific weapon name"))).Returns(true);
            mockSpecificGearGenerator.Setup(g => g.GeneratePrototypeFrom(power, ItemTypeConstants.Weapon, "specific weapon name")).Returns(specificWeapon);
            mockSpecificGearGenerator.Setup(g => g.GenerateFrom(It.Is<Item>(i => i.Name == "specific weapon name"))).Returns(specificWeapon);

            var weaponForQuantity = new Weapon();
            weaponForQuantity.Name = "base name";
            weaponForQuantity.Quantity = 1336;
            mockMundaneWeaponGenerator.Setup(g => g.Generate("base name")).Returns(weaponForQuantity);

            var weapon = magicalWeaponGenerator.Generate(power, "specific weapon name");
            Assert.That(weapon.Name, Is.EqualTo("specific weapon name"));
            Assert.That(weapon, Is.EqualTo(specificWeapon));
            Assert.That(weapon.Quantity, Is.EqualTo(1336));
        }

        [Test]
        public void GenerateSpecificFromBaseName()
        {
            var tableName = TableNameConstants.Percentiles.POWERITEMTYPEs(power, ItemTypeConstants.Weapon);
            mockPercentileSelector.SetupSequence(p => p.SelectFrom(Config.Name, tableName)).Returns(MagicalWeaponGenerator.SpecialAbility).Returns(MagicalWeaponGenerator.SpecialAbility).Returns(MagicalWeaponGenerator.SpecificWeapon);

            var specificWeapon = new Weapon();
            specificWeapon.Name = "specific weapon name";
            specificWeapon.BaseNames = new[] { "base name", "other specific base name" };

            mockSpecificGearGenerator.Setup(g => g.IsSpecific(ItemTypeConstants.Weapon, "specific weapon name")).Returns(true);
            mockSpecificGearGenerator.Setup(g => g.IsSpecific(It.Is<Item>(i => i.Name == "specific weapon name"))).Returns(true);
            mockSpecificGearGenerator.Setup(g => g.CanBeSpecific(power, ItemTypeConstants.Weapon, "base name")).Returns(true);
            mockSpecificGearGenerator.Setup(g => g.GenerateNameFrom(power, ItemTypeConstants.Weapon, "base name")).Returns("specific weapon name");
            mockSpecificGearGenerator.Setup(g => g.GeneratePrototypeFrom(power, ItemTypeConstants.Weapon, "specific weapon name")).Returns(specificWeapon);
            mockSpecificGearGenerator.Setup(g => g.GenerateFrom(It.Is<Item>(i => i.Name == "specific weapon name"))).Returns(specificWeapon);

            mockMundaneWeaponGenerator.Setup(g => g.Generate("base name")).Returns(mundaneWeapon);

            var weapon = magicalWeaponGenerator.Generate(power, "base name");
            Assert.That(weapon.Name, Is.EqualTo("specific weapon name"));
            Assert.That(weapon, Is.EqualTo(specificWeapon));
        }

        [Test]
        public void GenerateSpecificWithSpecialAbilitiesFromName()
        {
            var tableName = TableNameConstants.Percentiles.POWERITEMTYPEs(power, ItemTypeConstants.Weapon);
            mockPercentileSelector.SetupSequence(p => p.SelectFrom(Config.Name, tableName)).Returns(MagicalWeaponGenerator.SpecialAbility).Returns(MagicalWeaponGenerator.SpecialAbility).Returns("9266");

            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.MagicalWeaponTypes)).Returns("wrong weapon type");

            tableName = TableNameConstants.Percentiles.WEAPONTYPEWeapons("wrong weapon type");
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, tableName)).Returns("wrong weapon name");

            tableName = TableNameConstants.Percentiles.WEAPONTYPEWeapons("weapon type");
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, tableName)).Returns("weapon name");

            tableName = TableNameConstants.Percentiles.WEAPONTYPEWeapons("other weapon type");
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, tableName)).Returns("other weapon name");

            var specificWeapon = new Weapon();
            specificWeapon.Name = "specific weapon name";
            specificWeapon.BaseNames = new[] { "base name", "other specific base name" };
            specificWeapon.ItemType = ItemTypeConstants.Weapon;

            var abilities = new[] { new SpecialAbility(), new SpecialAbility() };
            specificWeapon.Magic.SpecialAbilities = abilities;

            mockSpecificGearGenerator.Setup(g => g.GeneratePrototypeFrom(power, ItemTypeConstants.Weapon, "specific weapon name")).Returns(specificWeapon);
            mockSpecificGearGenerator.Setup(g => g.GenerateFrom(It.Is<Item>(i => i.Name == "specific weapon name"))).Returns(specificWeapon);
            mockSpecificGearGenerator.Setup(g => g.IsSpecific(It.Is<Item>(i => i.Name == "specific weapon name"))).Returns(true);

            mockMundaneWeaponGenerator.Setup(g => g.Generate("base name")).Returns(mundaneWeapon);

            var weapon = magicalWeaponGenerator.Generate(power, "specific weapon name");
            Assert.That(weapon.Name, Is.EqualTo("specific weapon name"));
            Assert.That(weapon.Quantity, Is.EqualTo(600));
            Assert.That(weapon.BaseNames, Is.EquivalentTo(specificWeapon.BaseNames));
            Assert.That(weapon.Magic.SpecialAbilities, Is.EquivalentTo(abilities));
        }

        [Test]
        public void GenerateSpecificWithSpecialAbilitiesFromBaseName()
        {
            var tableName = TableNameConstants.Percentiles.POWERITEMTYPEs(power, ItemTypeConstants.Weapon);
            mockPercentileSelector.SetupSequence(p => p.SelectFrom(Config.Name, tableName)).Returns(MagicalWeaponGenerator.SpecialAbility).Returns(MagicalWeaponGenerator.SpecialAbility).Returns(MagicalWeaponGenerator.SpecificWeapon);

            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.MagicalWeaponTypes)).Returns("wrong weapon type");

            tableName = TableNameConstants.Percentiles.WEAPONTYPEWeapons("wrong weapon type");
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, tableName)).Returns("wrong weapon name");

            tableName = TableNameConstants.Percentiles.WEAPONTYPEWeapons("weapon type");
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, tableName)).Returns("weapon name");

            tableName = TableNameConstants.Percentiles.WEAPONTYPEWeapons("other weapon type");
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, tableName)).Returns("other weapon name");

            var specificWeapon = new Weapon();
            specificWeapon.Name = "specific weapon name";
            specificWeapon.BaseNames = new[] { "base name", "other specific base name" };
            specificWeapon.ItemType = ItemTypeConstants.Weapon;

            var abilities = new[] { new SpecialAbility(), new SpecialAbility() };
            specificWeapon.Magic.SpecialAbilities = abilities;

            mockSpecificGearGenerator.Setup(g => g.IsSpecific(ItemTypeConstants.Weapon, "specific weapon name")).Returns(true);
            mockSpecificGearGenerator.Setup(g => g.IsSpecific(It.Is<Item>(i => i.Name == "specific weapon name"))).Returns(true);
            mockSpecificGearGenerator.Setup(g => g.CanBeSpecific(power, ItemTypeConstants.Weapon, "base name")).Returns(true);
            mockSpecificGearGenerator.Setup(g => g.GenerateNameFrom(power, ItemTypeConstants.Weapon, "base name")).Returns("specific weapon name");
            mockSpecificGearGenerator.Setup(g => g.GeneratePrototypeFrom(power, ItemTypeConstants.Weapon, "specific weapon name")).Returns(specificWeapon);
            mockSpecificGearGenerator.Setup(g => g.GenerateFrom(It.Is<Item>(i => i.Name == "specific weapon name"))).Returns(specificWeapon);

            mockMundaneWeaponGenerator.Setup(g => g.Generate("base name")).Returns(mundaneWeapon);

            var weapon = magicalWeaponGenerator.Generate(power, "base name");
            Assert.That(weapon.Name, Is.EqualTo("specific weapon name"));
            Assert.That(weapon.Quantity, Is.EqualTo(600));
            Assert.That(weapon.BaseNames, Is.EquivalentTo(specificWeapon.BaseNames));
            Assert.That(weapon.Magic.SpecialAbilities, Is.EquivalentTo(abilities));
        }

        [Test]
        public void BUG_GenerateSpecificFromBaseName_WithQuantity()
        {
            var tableName = TableNameConstants.Percentiles.POWERITEMTYPEs(power, ItemTypeConstants.Weapon);
            mockPercentileSelector.SetupSequence(p => p.SelectFrom(Config.Name, tableName)).Returns(MagicalWeaponGenerator.SpecialAbility).Returns(MagicalWeaponGenerator.SpecialAbility).Returns(MagicalWeaponGenerator.SpecificWeapon);

            var specificWeapon = new Weapon();
            specificWeapon.Name = "specific weapon name";
            specificWeapon.BaseNames = new[] { "base name", "other specific base name" };

            mockSpecificGearGenerator.Setup(g => g.IsSpecific(ItemTypeConstants.Weapon, "specific weapon name")).Returns(true);
            mockSpecificGearGenerator.Setup(g => g.IsSpecific(It.Is<Item>(i => i.Name == "specific weapon name"))).Returns(true);
            mockSpecificGearGenerator.Setup(g => g.CanBeSpecific(power, ItemTypeConstants.Weapon, "base name")).Returns(true);
            mockSpecificGearGenerator.Setup(g => g.GenerateNameFrom(power, ItemTypeConstants.Weapon, "base name")).Returns("specific weapon name");
            mockSpecificGearGenerator.Setup(g => g.GeneratePrototypeFrom(power, ItemTypeConstants.Weapon, "specific weapon name")).Returns(specificWeapon);
            mockSpecificGearGenerator.Setup(g => g.GenerateFrom(It.Is<Item>(i => i.Name == "specific weapon name"))).Returns(specificWeapon);

            var weaponForQuantity = new Weapon();
            weaponForQuantity.Name = "base name";
            weaponForQuantity.Quantity = 1336;
            mockMundaneWeaponGenerator.Setup(g => g.Generate("base name")).Returns(weaponForQuantity);

            var weapon = magicalWeaponGenerator.Generate(power, "base name");
            Assert.That(weapon.Name, Is.EqualTo("specific weapon name"));
            Assert.That(weapon, Is.EqualTo(specificWeapon));
            Assert.That(weapon.Quantity, Is.EqualTo(1336));
        }
    }
}