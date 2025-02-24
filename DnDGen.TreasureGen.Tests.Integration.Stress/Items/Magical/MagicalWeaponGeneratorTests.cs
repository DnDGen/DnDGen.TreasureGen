using DnDGen.TreasureGen.Items;
using NUnit.Framework;
using System.Collections.Generic;

namespace DnDGen.TreasureGen.Tests.Integration.Stress.Items.Magical
{
    [TestFixture]
    public class MagicalWeaponGeneratorTests : MagicalItemGeneratorStressTests
    {
        protected override bool allowMinor
        {
            get { return true; }
        }

        protected override string itemType
        {
            get { return ItemTypeConstants.Weapon; }
        }

        [Test]
        public void StressWeapon()
        {
            stressor.Stress(GenerateAndAssertItem);
        }

        protected override void MakeSpecificAssertionsAgainst(Item item)
        {
            Assert.That(item.Name, Is.Not.Empty);
            Assert.That(item.Attributes, Is.Not.Empty, item.Name);
            Assert.That(item.Attributes, Contains.Item(AttributeConstants.Simple)
                .Or.Contains(AttributeConstants.Martial)
                .Or.Contains(AttributeConstants.Exotic), item.Name);
            Assert.That(item.Attributes, Contains.Item(AttributeConstants.Light)
                .Or.Contains(AttributeConstants.OneHanded)
                .Or.Contains(AttributeConstants.TwoHanded)
                .Or.Contains(AttributeConstants.Ranged), item.Name);
            Assert.That(item.Attributes, Contains.Item(AttributeConstants.Melee).Or.Contains(AttributeConstants.Ranged), item.Name);
            Assert.That(item.Contents, Is.Not.Null, item.Name);
            Assert.That(item.ItemType, Is.EqualTo(ItemTypeConstants.Weapon), item.Name);
            Assert.That(item.Traits, Is.Not.Null, item.Name);
            Assert.That(item.Magic.Charges, Is.Not.Negative, item.Name);
            Assert.That(item.Magic.SpecialAbilities, Is.Not.Null, item.Name);
            Assert.That(item, Is.InstanceOf<Weapon>(), item.Name);

            var weapon = item as Weapon;
            Assert.That(weapon.BaseNames, Is.Not.Empty, item.Name);
            Assert.That(weapon.CriticalMultiplier, Is.Not.Empty, item.Name);
            Assert.That(weapon.Damages, Is.Not.Empty, item.Name);
            Assert.That(weapon.CriticalDamages, Is.Not.Empty, item.Name);

            foreach (var damage in weapon.Damages)
            {
                Assert.That(damage.Roll, Is.Not.Empty, item.Name);
                Assert.That(damage.Type, Is.Not.Empty, item.Name);
            }

            Assert.That(weapon.Damages[0].Type, Contains.Substring(AttributeConstants.DamageTypes.Bludgeoning)
                .Or.Contains(AttributeConstants.DamageTypes.Piercing)
                .Or.Contains(AttributeConstants.DamageTypes.Slashing), item.Name);

            foreach (var damage in weapon.CriticalDamages)
            {
                Assert.That(damage.Roll, Is.Not.Empty, item.Name);
                Assert.That(damage.Type, Is.Not.Empty, item.Name);
            }

            Assert.That(weapon.CriticalDamages[0].Type, Contains.Substring(AttributeConstants.DamageTypes.Bludgeoning)
                .Or.Contains(AttributeConstants.DamageTypes.Piercing)
                .Or.Contains(AttributeConstants.DamageTypes.Slashing), item.Name);

            Assert.That(weapon.Size, Is.Not.Empty, item.Name);
            Assert.That(weapon.ThreatRangeSummary, Is.Not.Empty, item.Name);
        }

        protected override IEnumerable<string> GetItemNames()
        {
            return WeaponConstants.GetAllWeapons(true, true);
        }

        [Test]
        public void StressCustomWeapon()
        {
            stressor.Stress(GenerateAndAssertCustomItem);
        }

        [Test]
        public void StressWeaponFromName()
        {
            stressor.Stress(GenerateAndAssertItemFromName);
        }
    }
}