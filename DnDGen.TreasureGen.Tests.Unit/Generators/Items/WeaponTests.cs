using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using NUnit.Framework;
using System;
using System.Linq;

namespace DnDGen.TreasureGen.Tests.Unit.Generators.Items
{
    [TestFixture]
    public class WeaponTests
    {
        private Weapon weapon;
        private ItemVerifier itemVerifier;

        [SetUp]
        public void Setup()
        {
            weapon = new Weapon();
            itemVerifier = new ItemVerifier();
        }

        [Test]
        public void WeaponInitialized()
        {
            Assert.That(weapon.Ammunition, Is.Empty);
            Assert.That(weapon.Damages, Is.Empty);
            Assert.That(weapon.DamageRoll, Is.Empty);
            Assert.That(weapon.DamageSummary, Is.Empty);
            Assert.That(weapon.SecondaryDamages, Is.Empty);
            Assert.That(weapon.SecondaryDamageRoll, Is.Empty);
            Assert.That(weapon.SecondaryDamageSummary, Is.Empty);
            Assert.That(weapon.Size, Is.Empty);
            Assert.That(weapon.ThreatRangeSummary, Is.EqualTo("20"));
            Assert.That(weapon.ThreatRange, Is.Zero);
            Assert.That(weapon.CriticalDamages, Is.Empty);
            Assert.That(weapon.CriticalDamageRoll, Is.Empty);
            Assert.That(weapon.CriticalDamageSummary, Is.Empty);
            Assert.That(weapon.SecondaryCriticalDamages, Is.Empty);
            Assert.That(weapon.SecondaryCriticalDamageRoll, Is.Empty);
            Assert.That(weapon.SecondaryCriticalDamageSummary, Is.Empty);
            Assert.That(weapon.ItemType, Is.Empty); //INFO: Weapon could be a rod or staff
            Assert.That(weapon.CombatTypes, Is.Empty);
            Assert.That(weapon.IsDoubleWeapon, Is.False);
            Assert.That(weapon.CriticalMultiplier, Is.Empty);
            Assert.That(weapon.SecondaryCriticalMultiplier, Is.Empty);
            Assert.That(weapon.SecondaryMagicBonus, Is.Zero);
            Assert.That(weapon.SecondaryHasAbilities, Is.False);
        }

        [Test]
        public void WeaponIsItem()
        {
            Assert.That(weapon, Is.InstanceOf<Item>());
        }

        [Test]
        public void WeaponCanBeUsedAsWeaponOrWeapon()
        {
            Assert.That(weapon.CanBeUsedAsWeaponOrArmor, Is.True);
        }

        [Test]
        public void DamageRoll_NoDamages()
        {
            weapon.Damages.Clear();
            Assert.That(weapon.DamageRoll, Is.Empty);
        }

        [Test]
        public void DamageRoll_NoDamages_WithMagicBonus()
        {
            weapon.Damages.Clear();
            weapon.Magic.Bonus = 1337;
            weapon.SecondaryMagicBonus = 666;

            Assert.That(weapon.DamageRoll, Is.EqualTo("1337"));
        }

        [Test]
        public void DamageRoll_WithDamage()
        {
            weapon.Damages.Add(new Damage { Roll = "9266d90210", Type = "emotional" });
            Assert.That(weapon.DamageRoll, Is.EqualTo("9266d90210"));
        }

        [Test]
        public void DamageRoll_WithDamage_WithMagicBonus()
        {
            weapon.Damages.Add(new Damage { Roll = "9266d90210", Type = "emotional" });
            weapon.Magic.Bonus = 1337;
            weapon.SecondaryMagicBonus = 666;

            Assert.That(weapon.DamageRoll, Is.EqualTo("9266d90210+1337"));
        }

        [Test]
        public void DamageRoll_WithDamages()
        {
            weapon.Damages.Add(new Damage { Roll = "9266d90210", Type = "emotional" });
            weapon.Damages.Add(new Damage { Roll = "42d600", Type = "spiritual" });

            Assert.That(weapon.DamageRoll, Is.EqualTo("9266d90210+42d600"));
        }

        [Test]
        public void DamageRoll_WithDamages_WithMagicBonus()
        {
            weapon.Damages.Add(new Damage { Roll = "9266d90210", Type = "emotional" });
            weapon.Damages.Add(new Damage { Roll = "42d600", Type = "spiritual" });
            weapon.Magic.Bonus = 1337;
            weapon.SecondaryMagicBonus = 666;

            Assert.That(weapon.DamageRoll, Is.EqualTo("9266d90210+1337+42d600"));
        }

        [Test]
        public void DamageSummary_NoDamages()
        {
            weapon.Damages.Clear();
            Assert.That(weapon.DamageSummary, Is.Empty);
        }

        [Test]
        public void DamageSummary_NoDamages_WithMagicBonus()
        {
            weapon.Damages.Clear();
            weapon.Magic.Bonus = 1337;
            weapon.SecondaryMagicBonus = 666;

            Assert.That(weapon.DamageSummary, Is.EqualTo("1337"));
        }

        [Test]
        public void DamageSummary_WithDamage()
        {
            weapon.Damages.Add(new Damage { Roll = "9266d90210", Type = "emotional" });
            Assert.That(weapon.DamageSummary, Is.EqualTo("9266d90210 emotional"));
        }

        [Test]
        public void DamageSummary_WithDamage_WithMagicBonus()
        {
            weapon.Damages.Add(new Damage { Roll = "9266d90210", Type = "emotional" });
            weapon.Magic.Bonus = 1337;
            weapon.SecondaryMagicBonus = 666;

            Assert.That(weapon.DamageSummary, Is.EqualTo("9266d90210+1337 emotional"));
        }

        [Test]
        public void DamageSummary_WithDamages()
        {
            weapon.Damages.Add(new Damage { Roll = "9266d90210", Type = "emotional" });
            weapon.Damages.Add(new Damage { Roll = "42d600", Type = "spiritual" });

            Assert.That(weapon.DamageSummary, Is.EqualTo("9266d90210 emotional + 42d600 spiritual"));
        }

        [Test]
        public void DamageSummary_WithDamages_WithMagicBonus()
        {
            weapon.Damages.Add(new Damage { Roll = "9266d90210", Type = "emotional" });
            weapon.Damages.Add(new Damage { Roll = "42d600", Type = "spiritual" });
            weapon.Magic.Bonus = 1337;
            weapon.SecondaryMagicBonus = 666;

            Assert.That(weapon.DamageSummary, Is.EqualTo("9266d90210+1337 emotional + 42d600 spiritual"));
        }

        [Test]
        public void DamageSummary_WithDamages_WithMagicBonus_Conditional()
        {
            weapon.Damages.Add(new Damage { Roll = "9266d90210", Type = "emotional" });
            weapon.Damages.Add(new Damage { Roll = "42d600", Type = "spiritual", Condition = "only sometimes" });
            weapon.Magic.Bonus = 1337;
            weapon.SecondaryMagicBonus = 666;

            Assert.That(weapon.DamageSummary, Is.EqualTo("9266d90210+1337 emotional + 42d600 spiritual (only sometimes)"));
        }

        [Test]
        public void CriticalDamageRoll_NoDamages()
        {
            weapon.CriticalDamages.Clear();
            Assert.That(weapon.CriticalDamageRoll, Is.Empty);
        }

        [Test]
        public void CriticalDamageRoll_NoDamages_WithMagicBonus()
        {
            weapon.CriticalDamages.Clear();
            weapon.Magic.Bonus = 1337;
            weapon.SecondaryMagicBonus = 666;

            Assert.That(weapon.CriticalDamageRoll, Is.EqualTo("1337"));
        }

        [Test]
        public void CriticalDamageRoll_WithDamage()
        {
            weapon.CriticalDamages.Add(new Damage { Roll = "9266d90210", Type = "emotional" });
            Assert.That(weapon.CriticalDamageRoll, Is.EqualTo("9266d90210"));
        }

        [Test]
        public void CriticalDamageRoll_WithDamage_WithMagicBonus()
        {
            weapon.CriticalDamages.Add(new Damage { Roll = "9266d90210", Type = "emotional" });
            weapon.Magic.Bonus = 1337;
            weapon.SecondaryMagicBonus = 666;

            Assert.That(weapon.CriticalDamageRoll, Is.EqualTo("9266d90210+1337"));
        }

        [Test]
        public void CriticalDamageRoll_WithDamages()
        {
            weapon.CriticalDamages.Add(new Damage { Roll = "9266d90210", Type = "emotional" });
            weapon.CriticalDamages.Add(new Damage { Roll = "42d600", Type = "spiritual" });

            Assert.That(weapon.CriticalDamageRoll, Is.EqualTo("9266d90210+42d600"));
        }

        [Test]
        public void CriticalDamageRoll_WithDamages_WithMagicBonus()
        {
            weapon.CriticalDamages.Add(new Damage { Roll = "9266d90210", Type = "emotional" });
            weapon.CriticalDamages.Add(new Damage { Roll = "42d600", Type = "spiritual" });
            weapon.Magic.Bonus = 1337;
            weapon.SecondaryMagicBonus = 666;

            Assert.That(weapon.CriticalDamageRoll, Is.EqualTo("9266d90210+1337+42d600"));
        }

        [Test]
        public void CriticalDamageSummary_NoDamages()
        {
            weapon.CriticalDamages.Clear();
            Assert.That(weapon.CriticalDamageSummary, Is.Empty);
        }

        [Test]
        public void CriticalDamageSummary_NoDamages_WithMagicBonus()
        {
            weapon.CriticalDamages.Clear();
            weapon.Magic.Bonus = 1337;
            weapon.SecondaryMagicBonus = 666;

            Assert.That(weapon.CriticalDamageSummary, Is.EqualTo("1337"));
        }

        [Test]
        public void CriticalDamageSummary_WithDamage()
        {
            weapon.CriticalDamages.Add(new Damage { Roll = "9266d90210", Type = "emotional" });
            Assert.That(weapon.CriticalDamageSummary, Is.EqualTo("9266d90210 emotional"));
        }

        [Test]
        public void CriticalDamageSummary_WithDamage_WithMagicBonus()
        {
            weapon.CriticalDamages.Add(new Damage { Roll = "9266d90210", Type = "emotional" });
            weapon.Magic.Bonus = 1337;
            weapon.SecondaryMagicBonus = 666;

            Assert.That(weapon.CriticalDamageSummary, Is.EqualTo("9266d90210+1337 emotional"));
        }

        [Test]
        public void CriticalDamageSummary_WithDamages()
        {
            weapon.CriticalDamages.Add(new Damage { Roll = "9266d90210", Type = "emotional" });
            weapon.CriticalDamages.Add(new Damage { Roll = "42d600", Type = "spiritual" });

            Assert.That(weapon.CriticalDamageSummary, Is.EqualTo("9266d90210 emotional + 42d600 spiritual"));
        }

        [Test]
        public void CriticalDamageSummary_WithDamages_WithMagicBonus()
        {
            weapon.CriticalDamages.Add(new Damage { Roll = "9266d90210", Type = "emotional" });
            weapon.CriticalDamages.Add(new Damage { Roll = "42d600", Type = "spiritual" });
            weapon.Magic.Bonus = 1337;
            weapon.SecondaryMagicBonus = 666;

            Assert.That(weapon.CriticalDamageSummary, Is.EqualTo("9266d90210+1337 emotional + 42d600 spiritual"));
        }

        [Test]
        public void SecondaryDamageRoll_NoDamages()
        {
            weapon.SecondaryDamages.Clear();
            Assert.That(weapon.SecondaryDamageRoll, Is.Empty);
        }

        [Test]
        public void SecondaryDamageRoll_NoDamages_WithMagicBonus()
        {
            weapon.SecondaryDamages.Clear();
            weapon.Magic.Bonus = 666;
            weapon.SecondaryMagicBonus = 1337;

            Assert.That(weapon.SecondaryDamageRoll, Is.EqualTo("1337"));
        }

        [Test]
        public void SecondaryDamageRoll_WithDamage()
        {
            weapon.SecondaryDamages.Add(new Damage { Roll = "9266d90210", Type = "emotional" });
            Assert.That(weapon.SecondaryDamageRoll, Is.EqualTo("9266d90210"));
        }

        [Test]
        public void SecondaryDamageRoll_WithDamage_WithMagicBonus()
        {
            weapon.SecondaryDamages.Add(new Damage { Roll = "9266d90210", Type = "emotional" });
            weapon.Magic.Bonus = 666;
            weapon.SecondaryMagicBonus = 1337;

            Assert.That(weapon.SecondaryDamageRoll, Is.EqualTo("9266d90210+1337"));
        }

        [Test]
        public void SecondaryDamageRoll_WithDamages()
        {
            weapon.SecondaryDamages.Add(new Damage { Roll = "9266d90210", Type = "emotional" });
            weapon.SecondaryDamages.Add(new Damage { Roll = "42d600", Type = "spiritual" });

            Assert.That(weapon.SecondaryDamageRoll, Is.EqualTo("9266d90210+42d600"));
        }

        [Test]
        public void SecondaryDamageRoll_WithDamages_WithMagicBonus()
        {
            weapon.SecondaryDamages.Add(new Damage { Roll = "9266d90210", Type = "emotional" });
            weapon.SecondaryDamages.Add(new Damage { Roll = "42d600", Type = "spiritual" });
            weapon.Magic.Bonus = 666;
            weapon.SecondaryMagicBonus = 1337;

            Assert.That(weapon.SecondaryDamageRoll, Is.EqualTo("9266d90210+1337+42d600"));
        }

        [Test]
        public void SecondaryDamageSummary_NoDamages()
        {
            weapon.SecondaryDamages.Clear();
            Assert.That(weapon.SecondaryDamageSummary, Is.Empty);
        }

        [Test]
        public void SecondaryDamageSummary_NoDamages_WithMagicBonus()
        {
            weapon.SecondaryDamages.Clear();
            weapon.Magic.Bonus = 666;
            weapon.SecondaryMagicBonus = 1337;

            Assert.That(weapon.SecondaryDamageSummary, Is.EqualTo("1337"));
        }

        [Test]
        public void SecondaryDamageSummary_WithDamage()
        {
            weapon.SecondaryDamages.Add(new Damage { Roll = "9266d90210", Type = "emotional" });
            Assert.That(weapon.SecondaryDamageSummary, Is.EqualTo("9266d90210 emotional"));
        }

        [Test]
        public void SecondaryDamageSummary_WithDamage_WithMagicBonus()
        {
            weapon.SecondaryDamages.Add(new Damage { Roll = "9266d90210", Type = "emotional" });
            weapon.Magic.Bonus = 666;
            weapon.SecondaryMagicBonus = 1337;

            Assert.That(weapon.SecondaryDamageSummary, Is.EqualTo("9266d90210+1337 emotional"));
        }

        [Test]
        public void SecondaryDamageSummary_WithDamages()
        {
            weapon.SecondaryDamages.Add(new Damage { Roll = "9266d90210", Type = "emotional" });
            weapon.SecondaryDamages.Add(new Damage { Roll = "42d600", Type = "spiritual" });

            Assert.That(weapon.SecondaryDamageSummary, Is.EqualTo("9266d90210 emotional + 42d600 spiritual"));
        }

        [Test]
        public void SecondaryDamageSummary_WithDamages_WithMagicBonus()
        {
            weapon.SecondaryDamages.Add(new Damage { Roll = "9266d90210", Type = "emotional" });
            weapon.SecondaryDamages.Add(new Damage { Roll = "42d600", Type = "spiritual" });
            weapon.Magic.Bonus = 666;
            weapon.SecondaryMagicBonus = 1337;

            Assert.That(weapon.SecondaryDamageSummary, Is.EqualTo("9266d90210+1337 emotional + 42d600 spiritual"));
        }

        [Test]
        public void SecondaryCriticalDamageRoll_NoDamages()
        {
            weapon.SecondaryCriticalDamages.Clear();
            Assert.That(weapon.SecondaryCriticalDamageRoll, Is.Empty);
        }

        [Test]
        public void SecondaryCriticalDamageRoll_NoDamages_WithMagicBonus()
        {
            weapon.SecondaryCriticalDamages.Clear();
            weapon.Magic.Bonus = 666;
            weapon.SecondaryMagicBonus = 1337;

            Assert.That(weapon.SecondaryCriticalDamageRoll, Is.EqualTo("1337"));
        }

        [Test]
        public void SecondaryCriticalDamageRoll_WithDamage()
        {
            weapon.SecondaryCriticalDamages.Add(new Damage { Roll = "9266d90210", Type = "emotional" });
            Assert.That(weapon.SecondaryCriticalDamageRoll, Is.EqualTo("9266d90210"));
        }

        [Test]
        public void SecondaryCriticalDamageRoll_WithDamage_WithMagicBonus()
        {
            weapon.SecondaryCriticalDamages.Add(new Damage { Roll = "9266d90210", Type = "emotional" });
            weapon.Magic.Bonus = 666;
            weapon.SecondaryMagicBonus = 1337;

            Assert.That(weapon.SecondaryCriticalDamageRoll, Is.EqualTo("9266d90210+1337"));
        }

        [Test]
        public void SecondaryCriticalDamageRoll_WithDamages()
        {
            weapon.SecondaryCriticalDamages.Add(new Damage { Roll = "9266d90210", Type = "emotional" });
            weapon.SecondaryCriticalDamages.Add(new Damage { Roll = "42d600", Type = "spiritual" });

            Assert.That(weapon.SecondaryCriticalDamageRoll, Is.EqualTo("9266d90210+42d600"));
        }

        [Test]
        public void SecondaryCriticalDamageRoll_WithDamages_WithMagicBonus()
        {
            weapon.SecondaryCriticalDamages.Add(new Damage { Roll = "9266d90210", Type = "emotional" });
            weapon.SecondaryCriticalDamages.Add(new Damage { Roll = "42d600", Type = "spiritual" });
            weapon.Magic.Bonus = 666;
            weapon.SecondaryMagicBonus = 1337;

            Assert.That(weapon.SecondaryCriticalDamageRoll, Is.EqualTo("9266d90210+1337+42d600"));
        }

        [Test]
        public void SecondaryCriticalDamageSummary_NoDamages()
        {
            weapon.SecondaryCriticalDamages.Clear();
            Assert.That(weapon.SecondaryCriticalDamageSummary, Is.Empty);
        }

        [Test]
        public void SecondaryCriticalDamageSummary_NoDamages_WithMagicBonus()
        {
            weapon.SecondaryCriticalDamages.Clear();
            weapon.Magic.Bonus = 666;
            weapon.SecondaryMagicBonus = 1337;

            Assert.That(weapon.SecondaryCriticalDamageSummary, Is.EqualTo("1337"));
        }

        [Test]
        public void SecondaryCriticalDamageSummary_WithDamage()
        {
            weapon.SecondaryCriticalDamages.Add(new Damage { Roll = "9266d90210", Type = "emotional" });
            Assert.That(weapon.SecondaryCriticalDamageSummary, Is.EqualTo("9266d90210 emotional"));
        }

        [Test]
        public void SecondaryCriticalDamageSummary_WithDamage_WithMagicBonus()
        {
            weapon.SecondaryCriticalDamages.Add(new Damage { Roll = "9266d90210", Type = "emotional" });
            weapon.Magic.Bonus = 666;
            weapon.SecondaryMagicBonus = 1337;

            Assert.That(weapon.SecondaryCriticalDamageSummary, Is.EqualTo("9266d90210+1337 emotional"));
        }

        [Test]
        public void SecondaryCriticalDamageSummary_WithDamages()
        {
            weapon.SecondaryCriticalDamages.Add(new Damage { Roll = "9266d90210", Type = "emotional" });
            weapon.SecondaryCriticalDamages.Add(new Damage { Roll = "42d600", Type = "spiritual" });

            Assert.That(weapon.SecondaryCriticalDamageSummary, Is.EqualTo("9266d90210 emotional + 42d600 spiritual"));
        }

        [Test]
        public void SecondaryCriticalDamageSummary_WithDamages_WithMagicBonus()
        {
            weapon.SecondaryCriticalDamages.Add(new Damage { Roll = "9266d90210", Type = "emotional" });
            weapon.SecondaryCriticalDamages.Add(new Damage { Roll = "42d600", Type = "spiritual" });
            weapon.Magic.Bonus = 666;
            weapon.SecondaryMagicBonus = 1337;

            Assert.That(weapon.SecondaryCriticalDamageSummary, Is.EqualTo("9266d90210+1337 emotional + 42d600 spiritual"));
        }

        [Test]
        public void CloneIsSuccessful()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomWeaponTemplate(name);

            template.Ammunition = "nerf bullets";
            template.Damages.Add(new Damage { Roll = "a ton", Type = "stabbing" });
            template.Damages.Add(new Damage { Roll = "a bit", Type = "emotional" });
            template.CriticalDamages.Add(new Damage { Roll = "a ton more", Type = "stabbing" });
            template.CriticalDamages.Add(new Damage { Roll = "a bit more", Type = "spiritual" });
            template.SecondaryDamages.Add(new Damage { Roll = "some", Type = "whacking" });
            template.SecondaryDamages.Add(new Damage { Roll = "partial", Type = "mental" });
            template.SecondaryCriticalDamages.Add(new Damage { Roll = "some more", Type = "whacking" });
            template.SecondaryCriticalDamages.Add(new Damage { Roll = "partial more", Type = "psychic" });
            template.Size = "massive";
            template.SecondaryMagicBonus = 90210;
            template.SecondaryHasAbilities = true;
            template.SecondaryCriticalMultiplier = "sevenfold";

            var clone = template.Clone();
            Assert.That(clone, Is.Not.EqualTo(template));
            Assert.That(clone.Name, Is.EqualTo(template.Name));

            Assert.That(clone.Attributes, Is.Not.Empty.And.EquivalentTo(template.Attributes));
            Assert.That(clone.BaseNames, Is.Not.Empty.And.EquivalentTo(template.BaseNames));
            Assert.That(clone.Contents, Is.Not.Empty.And.EquivalentTo(template.Contents));
            Assert.That(clone.Magic.Bonus, Is.Not.Zero.And.EqualTo(template.Magic.Bonus));
            Assert.That(clone.Magic.Charges, Is.Not.Zero.And.EqualTo(template.Magic.Charges));
            Assert.That(clone.Magic.Curse, Is.Not.Empty.And.EqualTo(template.Magic.Curse));
            Assert.That(clone.Magic.Intelligence.Alignment, Is.Not.Empty.And.EqualTo(template.Magic.Intelligence.Alignment));
            Assert.That(clone.Magic.Intelligence.CharismaStat, Is.Not.Zero.And.EqualTo(template.Magic.Intelligence.CharismaStat));
            Assert.That(clone.Magic.Intelligence.Communication, Is.Not.Empty.And.EquivalentTo(template.Magic.Intelligence.Communication));
            Assert.That(clone.Magic.Intelligence.DedicatedPower, Is.Not.Empty.And.EqualTo(template.Magic.Intelligence.DedicatedPower));
            Assert.That(clone.Magic.Intelligence.Ego, Is.Not.Zero.And.EqualTo(template.Magic.Intelligence.Ego));
            Assert.That(clone.Magic.Intelligence.IntelligenceStat, Is.Not.Zero.And.EqualTo(template.Magic.Intelligence.IntelligenceStat));
            Assert.That(clone.Magic.Intelligence.Languages, Is.Not.Empty.And.EquivalentTo(template.Magic.Intelligence.Languages));
            Assert.That(clone.Magic.Intelligence.Personality, Is.Not.Empty.And.EqualTo(template.Magic.Intelligence.Personality));
            Assert.That(clone.Magic.Intelligence.Powers, Is.Not.Empty.And.EquivalentTo(template.Magic.Intelligence.Powers));
            Assert.That(clone.Magic.Intelligence.Senses, Is.Not.Empty.And.EqualTo(template.Magic.Intelligence.Senses));
            Assert.That(clone.Magic.Intelligence.SpecialPurpose, Is.Not.Empty.And.EqualTo(template.Magic.Intelligence.SpecialPurpose));
            Assert.That(clone.Magic.Intelligence.WisdomStat, Is.Not.Zero.And.EqualTo(template.Magic.Intelligence.WisdomStat));
            Assert.That(clone.Magic.SpecialAbilities, Is.Not.Empty.And.EquivalentTo(template.Magic.SpecialAbilities));
            Assert.That(clone.Traits, Is.SupersetOf(template.Traits));

            var cloneWeapon = clone as Weapon;
            Assert.That(cloneWeapon.Ammunition, Is.Not.Empty.And.EqualTo(template.Ammunition));
            Assert.That(cloneWeapon.DamageRoll, Is.Not.Empty.And.EqualTo(template.DamageRoll));
            Assert.That(cloneWeapon.DamageSummary, Is.Not.Empty.And.EqualTo(template.DamageSummary));
            Assert.That(cloneWeapon.CriticalDamageRoll, Is.Not.Empty.And.EqualTo(template.CriticalDamageRoll));
            Assert.That(cloneWeapon.CriticalDamageSummary, Is.Not.Empty.And.EqualTo(template.CriticalDamageSummary));
            Assert.That(cloneWeapon.SecondaryDamageRoll, Is.Not.Empty.And.EqualTo(template.SecondaryDamageRoll));
            Assert.That(cloneWeapon.SecondaryDamageSummary, Is.Not.Empty.And.EqualTo(template.SecondaryDamageSummary));
            Assert.That(cloneWeapon.SecondaryCriticalDamageRoll, Is.Not.Empty.And.EqualTo(template.SecondaryCriticalDamageRoll));
            Assert.That(cloneWeapon.SecondaryCriticalDamageSummary, Is.Not.Empty.And.EqualTo(template.SecondaryCriticalDamageSummary));
            Assert.That(cloneWeapon.Size, Is.Not.Empty.And.EqualTo(template.Size));
            Assert.That(cloneWeapon.ThreatRangeSummary, Is.Not.Empty.And.EqualTo(template.ThreatRangeSummary));
            Assert.That(cloneWeapon.ThreatRange, Is.Not.Zero.And.EqualTo(template.ThreatRange));
            Assert.That(cloneWeapon.CriticalMultiplier, Is.Not.Empty.And.EqualTo(template.CriticalMultiplier));
            Assert.That(cloneWeapon.SecondaryCriticalMultiplier, Is.Not.Empty.And.EqualTo(template.SecondaryCriticalMultiplier));
            Assert.That(cloneWeapon.SecondaryMagicBonus, Is.Not.Zero.And.EqualTo(template.SecondaryMagicBonus));
            Assert.That(cloneWeapon.SecondaryHasAbilities, Is.True.And.EqualTo(template.SecondaryHasAbilities));
        }

        [Test]
        public void MundaneCloneIsSuccessful()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomWeaponTemplate(name);
            template.ItemType = Guid.NewGuid().ToString();
            template.Magic.SpecialAbilities = new[]
            {
                new SpecialAbility { Name = Guid.NewGuid().ToString() },
                new SpecialAbility { Name = Guid.NewGuid().ToString() }
            };

            template.Attributes = new[] { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() };
            template.Damages.Add(new Damage { Roll = "a ton", Type = "stabbing" });
            template.Damages.Add(new Damage { Roll = "a bit", Type = "emotional" });
            template.CriticalDamages.Add(new Damage { Roll = "a ton more", Type = "stabbing" });
            template.CriticalDamages.Add(new Damage { Roll = "a bit more", Type = "spiritual" });
            template.SecondaryDamages.Add(new Damage { Roll = "some", Type = "whacking" });
            template.SecondaryDamages.Add(new Damage { Roll = "partial", Type = "mental" });
            template.SecondaryCriticalDamages.Add(new Damage { Roll = "some more", Type = "whacking" });
            template.SecondaryCriticalDamages.Add(new Damage { Roll = "partial more", Type = "psychic" });
            template.Size = "massive";
            template.Ammunition = "nerf bullets";
            template.SecondaryMagicBonus = 90210;
            template.SecondaryHasAbilities = true;
            template.SecondaryCriticalMultiplier = "sevenfold";

            var clone = template.MundaneClone();
            itemVerifier.AssertMundaneItemFromTemplate(clone, template);
            Assert.That(clone.ItemType, Is.EqualTo(template.ItemType));
            Assert.That(clone.Attributes, Is.EquivalentTo(template.Attributes));
            Assert.That(clone.Magic.SpecialAbilities, Is.Empty);

            var cloneWeapon = clone as Weapon;
            Assert.That(cloneWeapon.Ammunition, Is.Not.Empty.And.EqualTo(template.Ammunition));
            Assert.That(cloneWeapon.DamageRoll, Is.Not.Empty.And.EqualTo(string.Join("+", template.Damages.Select(d => d.Roll))));
            Assert.That(cloneWeapon.DamageSummary, Is.Not.Empty.And.EqualTo(string.Join(" + ", template.Damages.Select(d => d.Summary))));
            Assert.That(cloneWeapon.CriticalDamageRoll, Is.Not.Empty.And.EqualTo(string.Join("+", template.CriticalDamages.Select(d => d.Roll))));
            Assert.That(cloneWeapon.CriticalDamageSummary, Is.Not.Empty.And.EqualTo(string.Join(" + ", template.CriticalDamages.Select(d => d.Summary))));
            Assert.That(cloneWeapon.SecondaryDamageRoll, Is.Not.Empty.And.EqualTo(string.Join("+", template.SecondaryDamages.Select(d => d.Roll))));
            Assert.That(cloneWeapon.SecondaryDamageSummary, Is.Not.Empty.And.EqualTo(string.Join(" + ", template.SecondaryDamages.Select(d => d.Summary))));
            Assert.That(cloneWeapon.SecondaryCriticalDamageRoll, Is.Not.Empty.And.EqualTo(string.Join("+", template.SecondaryCriticalDamages.Select(d => d.Roll))));
            Assert.That(cloneWeapon.SecondaryCriticalDamageSummary, Is.Not.Empty.And.EqualTo(string.Join(" + ", template.SecondaryCriticalDamages.Select(d => d.Summary))));
            Assert.That(cloneWeapon.Size, Is.Not.Empty.And.EqualTo(template.Size));
            Assert.That(cloneWeapon.ThreatRangeSummary, Is.Not.Empty.And.EqualTo(template.ThreatRangeSummary));
            Assert.That(cloneWeapon.ThreatRange, Is.Not.Zero.And.EqualTo(template.ThreatRange));
            Assert.That(cloneWeapon.CriticalMultiplier, Is.Not.Empty.And.EqualTo(template.CriticalMultiplier));
            Assert.That(cloneWeapon.SecondaryCriticalMultiplier, Is.Not.Empty.And.EqualTo(template.SecondaryCriticalMultiplier));
            Assert.That(cloneWeapon.SecondaryMagicBonus, Is.Zero);
            Assert.That(cloneWeapon.SecondaryHasAbilities, Is.False);
        }

        [Test]
        public void SmartCloneIsSuccessful()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomWeaponTemplate(name);
            template.ItemType = Guid.NewGuid().ToString();

            template.Attributes = new[] { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() };
            template.Damages.Add(new Damage { Roll = "a ton", Type = "stabbing" });
            template.Damages.Add(new Damage { Roll = "a bit", Type = "emotional" });
            template.CriticalDamages.Add(new Damage { Roll = "a ton more", Type = "stabbing" });
            template.CriticalDamages.Add(new Damage { Roll = "a bit more", Type = "spiritual" });
            template.SecondaryDamages.Add(new Damage { Roll = "some", Type = "whacking" });
            template.SecondaryDamages.Add(new Damage { Roll = "partial", Type = "mental" });
            template.SecondaryCriticalDamages.Add(new Damage { Roll = "some more", Type = "whacking" });
            template.SecondaryCriticalDamages.Add(new Damage { Roll = "partial more", Type = "psychic" });
            template.Ammunition = "nerf bullets";
            template.Size = "massive";
            template.SecondaryMagicBonus = 90210;
            template.SecondaryHasAbilities = true;
            template.SecondaryCriticalMultiplier = "sevenfold";

            var clone = template.SmartClone();
            itemVerifier.AssertMagicalItemFromTemplate(clone, template);
            Assert.That(clone.ItemType, Is.EqualTo(template.ItemType));
            Assert.That(clone.Attributes, Is.EquivalentTo(template.Attributes));

            var cloneWeapon = clone as Weapon;
            Assert.That(cloneWeapon.Ammunition, Is.Not.Empty.And.EqualTo(template.Ammunition));
            Assert.That(cloneWeapon.DamageRoll, Is.Not.Empty.And.EqualTo(template.DamageRoll));
            Assert.That(cloneWeapon.DamageSummary, Is.Not.Empty.And.EqualTo(template.DamageSummary));
            Assert.That(cloneWeapon.CriticalDamageRoll, Is.Not.Empty.And.EqualTo(template.CriticalDamageRoll));
            Assert.That(cloneWeapon.CriticalDamageSummary, Is.Not.Empty.And.EqualTo(template.CriticalDamageSummary));
            Assert.That(cloneWeapon.SecondaryDamageRoll, Is.Not.Empty.And.EqualTo(template.SecondaryDamageRoll));
            Assert.That(cloneWeapon.SecondaryDamageSummary, Is.Not.Empty.And.EqualTo(template.SecondaryDamageSummary));
            Assert.That(cloneWeapon.SecondaryCriticalDamageRoll, Is.Not.Empty.And.EqualTo(template.SecondaryCriticalDamageRoll));
            Assert.That(cloneWeapon.SecondaryCriticalDamageSummary, Is.Not.Empty.And.EqualTo(template.SecondaryCriticalDamageSummary));
            Assert.That(cloneWeapon.Size, Is.Not.Empty.And.EqualTo(template.Size));
            Assert.That(cloneWeapon.ThreatRangeSummary, Is.Not.Empty.And.EqualTo(template.ThreatRangeSummary));
            Assert.That(cloneWeapon.ThreatRange, Is.Not.Zero.And.EqualTo(template.ThreatRange));
            Assert.That(cloneWeapon.CriticalMultiplier, Is.Not.Empty.And.EqualTo(template.CriticalMultiplier));
            Assert.That(cloneWeapon.SecondaryCriticalMultiplier, Is.Not.Empty.And.EqualTo(template.SecondaryCriticalMultiplier));
            Assert.That(cloneWeapon.SecondaryMagicBonus, Is.Not.Zero.And.EqualTo(template.SecondaryMagicBonus));
            Assert.That(cloneWeapon.SecondaryHasAbilities, Is.True.And.EqualTo(template.SecondaryHasAbilities));
        }

        [Test]
        public void IsMelee()
        {
            weapon.Attributes = new[] { "some attribute", AttributeConstants.Melee };
            Assert.That(weapon.CombatTypes, Contains.Item(AttributeConstants.Melee));
            Assert.That(weapon.CombatTypes.Count, Is.EqualTo(1));
        }

        [Test]
        public void IsRanged()
        {
            weapon.Attributes = new[] { "some attribute", AttributeConstants.Ranged };
            Assert.That(weapon.CombatTypes, Contains.Item(AttributeConstants.Ranged));
            Assert.That(weapon.CombatTypes.Count, Is.EqualTo(1));
        }

        [Test]
        public void IsMeleeAndRanged()
        {
            weapon.Attributes = new[] { "some attribute", AttributeConstants.Melee, "other attribute", AttributeConstants.Ranged };
            Assert.That(weapon.CombatTypes, Contains.Item(AttributeConstants.Melee));
            Assert.That(weapon.CombatTypes, Contains.Item(AttributeConstants.Ranged));
            Assert.That(weapon.CombatTypes.Count, Is.EqualTo(2));
        }

        [Test]
        public void CloneIntoCopiesProperties()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomWeaponTemplate(name);

            template.Ammunition = "nerf bullets";
            template.Damages.Add(new Damage { Roll = "a ton", Type = "stabbing" });
            template.Damages.Add(new Damage { Roll = "a bit", Type = "emotional" });
            template.CriticalDamages.Add(new Damage { Roll = "a ton more", Type = "stabbing" });
            template.CriticalDamages.Add(new Damage { Roll = "a bit more", Type = "spiritual" });
            template.SecondaryDamages.Add(new Damage { Roll = "some", Type = "whacking" });
            template.SecondaryDamages.Add(new Damage { Roll = "partial", Type = "mental" });
            template.SecondaryCriticalDamages.Add(new Damage { Roll = "some more", Type = "whacking" });
            template.SecondaryCriticalDamages.Add(new Damage { Roll = "partial more", Type = "psychic" });
            template.Size = "massive";
            template.SecondaryMagicBonus = 90210;
            template.SecondaryHasAbilities = true;
            template.SecondaryCriticalMultiplier = "sevenfold";

            var clone = new Weapon();
            var newClone = template.CloneInto(clone);
            Assert.That(newClone, Is.EqualTo(clone));
            Assert.That(clone.Name, Is.EqualTo(template.Name));

            Assert.That(clone.Attributes, Is.EquivalentTo(template.Attributes));
            Assert.That(clone.BaseNames, Is.EquivalentTo(template.BaseNames));
            Assert.That(clone.Contents, Is.EquivalentTo(template.Contents));
            Assert.That(clone.Magic.Bonus, Is.EqualTo(template.Magic.Bonus));
            Assert.That(clone.Magic.Charges, Is.EqualTo(template.Magic.Charges));
            Assert.That(clone.Magic.Curse, Is.EqualTo(template.Magic.Curse));
            Assert.That(clone.Magic.Intelligence.Alignment, Is.EqualTo(template.Magic.Intelligence.Alignment));
            Assert.That(clone.Magic.Intelligence.CharismaStat, Is.EqualTo(template.Magic.Intelligence.CharismaStat));
            Assert.That(clone.Magic.Intelligence.Communication, Is.EquivalentTo(template.Magic.Intelligence.Communication));
            Assert.That(clone.Magic.Intelligence.DedicatedPower, Is.EqualTo(template.Magic.Intelligence.DedicatedPower));
            Assert.That(clone.Magic.Intelligence.Ego, Is.EqualTo(template.Magic.Intelligence.Ego));
            Assert.That(clone.Magic.Intelligence.IntelligenceStat, Is.EqualTo(template.Magic.Intelligence.IntelligenceStat));
            Assert.That(clone.Magic.Intelligence.Languages, Is.EquivalentTo(template.Magic.Intelligence.Languages));
            Assert.That(clone.Magic.Intelligence.Personality, Is.EqualTo(template.Magic.Intelligence.Personality));
            Assert.That(clone.Magic.Intelligence.Powers, Is.EquivalentTo(template.Magic.Intelligence.Powers));
            Assert.That(clone.Magic.Intelligence.Senses, Is.EqualTo(template.Magic.Intelligence.Senses));
            Assert.That(clone.Magic.Intelligence.SpecialPurpose, Is.EqualTo(template.Magic.Intelligence.SpecialPurpose));
            Assert.That(clone.Magic.Intelligence.WisdomStat, Is.EqualTo(template.Magic.Intelligence.WisdomStat));
            Assert.That(clone.Magic.SpecialAbilities, Is.EquivalentTo(template.Magic.SpecialAbilities));
            Assert.That(clone.Traits, Is.SupersetOf(template.Traits));

            Assert.That(clone.Ammunition, Is.Not.Empty.And.EqualTo(template.Ammunition));
            Assert.That(clone.DamageRoll, Is.Not.Empty.And.EqualTo(template.DamageRoll));
            Assert.That(clone.DamageSummary, Is.Not.Empty.And.EqualTo(template.DamageSummary));
            Assert.That(clone.CriticalDamageRoll, Is.Not.Empty.And.EqualTo(template.CriticalDamageRoll));
            Assert.That(clone.CriticalDamageSummary, Is.Not.Empty.And.EqualTo(template.CriticalDamageSummary));
            Assert.That(clone.SecondaryDamageRoll, Is.Not.Empty.And.EqualTo(template.SecondaryDamageRoll));
            Assert.That(clone.SecondaryDamageSummary, Is.Not.Empty.And.EqualTo(template.SecondaryDamageSummary));
            Assert.That(clone.SecondaryCriticalDamageRoll, Is.Not.Empty.And.EqualTo(template.SecondaryCriticalDamageRoll));
            Assert.That(clone.SecondaryCriticalDamageSummary, Is.Not.Empty.And.EqualTo(template.SecondaryCriticalDamageSummary));
            Assert.That(clone.Size, Is.Not.Empty.And.EqualTo(template.Size));
            Assert.That(clone.ThreatRangeSummary, Is.Not.Empty.And.EqualTo(template.ThreatRangeSummary));
            Assert.That(clone.ThreatRange, Is.Not.Zero.And.EqualTo(template.ThreatRange));
            Assert.That(clone.CriticalMultiplier, Is.Not.Empty.And.EqualTo(template.CriticalMultiplier));
            Assert.That(clone.SecondaryCriticalMultiplier, Is.Not.Empty.And.EqualTo(template.SecondaryCriticalMultiplier));
            Assert.That(clone.SecondaryMagicBonus, Is.Not.Zero.And.EqualTo(template.SecondaryMagicBonus));
            Assert.That(clone.SecondaryHasAbilities, Is.True.And.EqualTo(template.SecondaryHasAbilities));
        }

        [Test]
        public void BUG_CloneIntoPrefersNonemptyProperties()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomWeaponTemplate(name);

            template.Ammunition = string.Empty;
            template.Damages.Clear();
            template.CriticalDamages.Clear();
            template.SecondaryDamages.Clear();
            template.SecondaryCriticalDamages.Clear();
            template.Size = string.Empty;
            template.ThreatRange = 0;
            template.Quantity = 1;
            template.CriticalMultiplier = string.Empty;

            var clone = new Weapon();
            clone.Ammunition = "nerf bullets";
            clone.Damages.Add(new Damage { Roll = "a ton", Type = "stabbing" });
            clone.Damages.Add(new Damage { Roll = "a bit", Type = "emotional" });
            clone.CriticalDamages.Add(new Damage { Roll = "a ton more", Type = "stabbing" });
            clone.CriticalDamages.Add(new Damage { Roll = "a bit more", Type = "spiritual" });
            clone.SecondaryDamages.Add(new Damage { Roll = "some", Type = "whacking" });
            clone.SecondaryDamages.Add(new Damage { Roll = "partial", Type = "mental" });
            clone.SecondaryCriticalDamages.Add(new Damage { Roll = "some more", Type = "whacking" });
            clone.SecondaryCriticalDamages.Add(new Damage { Roll = "partial more", Type = "psychic" });
            clone.Size = "massive";
            clone.Quantity = 9266;
            clone.ThreatRange = 42;
            clone.CriticalMultiplier = "sevenfold";
            clone.SecondaryMagicBonus = 90210;
            clone.SecondaryHasAbilities = true;
            clone.SecondaryCriticalMultiplier = "threefold";

            var newClone = template.CloneInto(clone);
            Assert.That(newClone, Is.EqualTo(clone));
            Assert.That(clone.Name, Is.EqualTo(template.Name));

            Assert.That(clone.Attributes, Is.EquivalentTo(template.Attributes));
            Assert.That(clone.BaseNames, Is.EquivalentTo(template.BaseNames));
            Assert.That(clone.Contents, Is.EquivalentTo(template.Contents));
            Assert.That(clone.Magic.Bonus, Is.EqualTo(template.Magic.Bonus));
            Assert.That(clone.Magic.Charges, Is.EqualTo(template.Magic.Charges));
            Assert.That(clone.Magic.Curse, Is.EqualTo(template.Magic.Curse));
            Assert.That(clone.Magic.Intelligence.Alignment, Is.EqualTo(template.Magic.Intelligence.Alignment));
            Assert.That(clone.Magic.Intelligence.CharismaStat, Is.EqualTo(template.Magic.Intelligence.CharismaStat));
            Assert.That(clone.Magic.Intelligence.Communication, Is.EquivalentTo(template.Magic.Intelligence.Communication));
            Assert.That(clone.Magic.Intelligence.DedicatedPower, Is.EqualTo(template.Magic.Intelligence.DedicatedPower));
            Assert.That(clone.Magic.Intelligence.Ego, Is.EqualTo(template.Magic.Intelligence.Ego));
            Assert.That(clone.Magic.Intelligence.IntelligenceStat, Is.EqualTo(template.Magic.Intelligence.IntelligenceStat));
            Assert.That(clone.Magic.Intelligence.Languages, Is.EquivalentTo(template.Magic.Intelligence.Languages));
            Assert.That(clone.Magic.Intelligence.Personality, Is.EqualTo(template.Magic.Intelligence.Personality));
            Assert.That(clone.Magic.Intelligence.Powers, Is.EquivalentTo(template.Magic.Intelligence.Powers));
            Assert.That(clone.Magic.Intelligence.Senses, Is.EqualTo(template.Magic.Intelligence.Senses));
            Assert.That(clone.Magic.Intelligence.SpecialPurpose, Is.EqualTo(template.Magic.Intelligence.SpecialPurpose));
            Assert.That(clone.Magic.Intelligence.WisdomStat, Is.EqualTo(template.Magic.Intelligence.WisdomStat));
            Assert.That(clone.Magic.SpecialAbilities, Is.EquivalentTo(template.Magic.SpecialAbilities));
            Assert.That(clone.Traits, Is.SupersetOf(template.Traits));

            Assert.That(clone.Ammunition, Is.EqualTo("nerf bullets"));
            Assert.That(clone.DamageRoll, Is.EqualTo($"a ton+{template.Magic.Bonus}+a bit"));
            Assert.That(clone.DamageSummary, Is.EqualTo($"a ton+{template.Magic.Bonus} stabbing + a bit emotional"));
            Assert.That(clone.CriticalDamageRoll, Is.EqualTo($"a ton more+{template.Magic.Bonus}+a bit more"));
            Assert.That(clone.CriticalDamageSummary, Is.EqualTo($"a ton more+{template.Magic.Bonus} stabbing + a bit more spiritual"));
            Assert.That(clone.SecondaryDamageRoll, Is.EqualTo($"some+90210+partial"));
            Assert.That(clone.SecondaryDamageSummary, Is.EqualTo($"some+90210 whacking + partial mental"));
            Assert.That(clone.SecondaryCriticalDamageRoll, Is.EqualTo($"some more+90210+partial more"));
            Assert.That(clone.SecondaryCriticalDamageSummary, Is.EqualTo($"some more+90210 whacking + partial more psychic"));
            Assert.That(clone.Size, Is.EqualTo("massive"));
            Assert.That(clone.ThreatRange, Is.Not.Zero.And.EqualTo(42));
            Assert.That(clone.Quantity, Is.Not.Zero.And.EqualTo(9266));
            Assert.That(clone.CriticalMultiplier, Is.Not.Empty.And.EqualTo("sevenfold"));
            Assert.That(clone.SecondaryCriticalMultiplier, Is.Not.Empty.And.EqualTo("threefold"));
            Assert.That(clone.SecondaryMagicBonus, Is.Not.Zero.And.EqualTo(90210));
            Assert.That(clone.SecondaryHasAbilities, Is.True);
        }

        [TestCase(1, "20")]
        [TestCase(2, "19-20")]
        [TestCase(3, "18-20")]
        [TestCase(4, "17-20")]
        [TestCase(5, "16-20")]
        [TestCase(6, "15-20")]
        public void ThreatRangeSummary(int threatRange, string summary)
        {
            weapon.ThreatRange = threatRange;
            Assert.That(weapon.ThreatRangeSummary, Is.EqualTo(summary));
        }

        [Test]
        public void IsDoubleWeapon_IsNot()
        {
            weapon.Attributes = ["my attribute", "my other attribute"];
            Assert.That(weapon.IsDoubleWeapon, Is.False);
        }

        [Test]
        public void IsDoubleWeapon_Is()
        {
            weapon.Attributes = ["my attribute", AttributeConstants.DoubleWeapon, "my other attribute"];
            Assert.That(weapon.IsDoubleWeapon, Is.True);
        }
    }
}
