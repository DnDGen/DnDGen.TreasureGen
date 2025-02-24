using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Mundane;
using DnDGen.TreasureGen.Tests.Unit.Generators.Items;
using NUnit.Framework;
using System.Linq;

namespace DnDGen.TreasureGen.Tests.Integration.Generators.Items.Mundane
{
    [TestFixture]
    public class MundaneWeaponGeneratorTests : IntegrationTests
    {
        private ItemVerifier itemVerifier;
        private MundaneItemGenerator weaponGenerator;

        [SetUp]
        public void Setup()
        {
            itemVerifier = new ItemVerifier();
            weaponGenerator = GetNewInstanceOf<MundaneItemGenerator>(ItemTypeConstants.Weapon);
        }

        [TestCaseSource(typeof(ItemTestData), nameof(ItemTestData.WeaponsNoSpecific))]
        public void GenerateWeapon(string itemName)
        {
            var item = weaponGenerator.Generate(itemName);
            itemVerifier.AssertItem(item);
        }

        [TestCase(WeaponConstants.Longsword, TraitConstants.Sizes.Colossal)]
        [TestCase(WeaponConstants.Longsword, TraitConstants.Sizes.Gargantuan)]
        [TestCase(WeaponConstants.Longsword, TraitConstants.Sizes.Huge)]
        [TestCase(WeaponConstants.Longsword, TraitConstants.Sizes.Large)]
        [TestCase(WeaponConstants.Longsword, TraitConstants.Sizes.Medium)]
        [TestCase(WeaponConstants.Longsword, TraitConstants.Sizes.Small)]
        [TestCase(WeaponConstants.Longsword, TraitConstants.Sizes.Tiny)]
        public void GenerateWeaponOfSize(string itemName, string size)
        {
            var item = weaponGenerator.Generate(itemName, "my trait", size);
            itemVerifier.AssertItem(item);
            Assert.That(item, Is.InstanceOf<Weapon>());

            var weapon = item as Weapon;
            Assert.That(weapon.Size, Is.EqualTo(size));
            Assert.That(weapon.Traits, Contains.Item("my trait")
                .And.Not.Contains(size));
        }

        [TestCase(WeaponConstants.Longsword, TraitConstants.Sizes.Colossal)]
        [TestCase(WeaponConstants.Longsword, TraitConstants.Sizes.Gargantuan)]
        [TestCase(WeaponConstants.Longsword, TraitConstants.Sizes.Huge)]
        [TestCase(WeaponConstants.Longsword, TraitConstants.Sizes.Large)]
        [TestCase(WeaponConstants.Longsword, TraitConstants.Sizes.Medium)]
        [TestCase(WeaponConstants.Longsword, TraitConstants.Sizes.Small)]
        [TestCase(WeaponConstants.Longsword, TraitConstants.Sizes.Tiny)]
        public void GenerateWeaponOfSize_FromTemplate(string itemName, string size)
        {
            var template = itemVerifier.CreateRandomWeaponTemplate(itemName);
            template.Traits.Add(size);
            template.Size = string.Empty;

            var item = weaponGenerator.Generate(template);
            itemVerifier.AssertItem(item);
            Assert.That(item, Is.InstanceOf<Weapon>(), item.Summary);

            var weapon = item as Weapon;
            Assert.That(weapon.Size, Is.EqualTo(size), weapon.Summary);
            Assert.That(weapon.Traits, Does.Not.Contain(size)
                .And.SupersetOf(template.Traits.Take(2)), weapon.Summary);
        }

        [TestCase(WeaponConstants.CompositeLongbow, WeaponConstants.Arrow)]
        [TestCase(WeaponConstants.CompositeLongbow_StrengthPlus0, WeaponConstants.Arrow)]
        [TestCase(WeaponConstants.CompositeLongbow_StrengthPlus1, WeaponConstants.Arrow)]
        [TestCase(WeaponConstants.CompositeLongbow_StrengthPlus2, WeaponConstants.Arrow)]
        [TestCase(WeaponConstants.CompositeLongbow_StrengthPlus3, WeaponConstants.Arrow)]
        [TestCase(WeaponConstants.CompositeLongbow_StrengthPlus4, WeaponConstants.Arrow)]
        [TestCase(WeaponConstants.CompositeShortbow, WeaponConstants.Arrow)]
        [TestCase(WeaponConstants.CompositeShortbow_StrengthPlus0, WeaponConstants.Arrow)]
        [TestCase(WeaponConstants.CompositeShortbow_StrengthPlus1, WeaponConstants.Arrow)]
        [TestCase(WeaponConstants.CompositeShortbow_StrengthPlus2, WeaponConstants.Arrow)]
        [TestCase(WeaponConstants.HandCrossbow, WeaponConstants.CrossbowBolt)]
        [TestCase(WeaponConstants.HeavyCrossbow, WeaponConstants.CrossbowBolt)]
        [TestCase(WeaponConstants.HeavyRepeatingCrossbow, WeaponConstants.CrossbowBolt)]
        [TestCase(WeaponConstants.LightCrossbow, WeaponConstants.CrossbowBolt)]
        [TestCase(WeaponConstants.LightRepeatingCrossbow, WeaponConstants.CrossbowBolt)]
        [TestCase(WeaponConstants.Longbow, WeaponConstants.Arrow)]
        [TestCase(WeaponConstants.Shortbow, WeaponConstants.Arrow)]
        [TestCase(WeaponConstants.Sling, WeaponConstants.SlingBullet)]
        public void GenerateWeaponWithAmmunition(string weaponName, string ammunition)
        {
            var item = weaponGenerator.Generate(weaponName);
            itemVerifier.AssertItem(item);
            Assert.That(item, Is.InstanceOf<Weapon>(), item.Summary);

            var weapon = item as Weapon;
            Assert.That(weapon.Ammunition, Is.EqualTo(ammunition), item.Summary);
        }

        [Test]
        [Repeat(100)]
        public void BUG_GenerateWeapon_NetHasQuantityOf1()
        {
            var item = weaponGenerator.Generate(WeaponConstants.Net);
            itemVerifier.AssertItem(item);
            Assert.That(item.NameMatches(WeaponConstants.Net), Is.True, item.Summary);
            Assert.That(item.Quantity, Is.EqualTo(1), item.Summary);
        }

        [TestCase(WeaponConstants.Arrow)]
        [TestCase(WeaponConstants.CrossbowBolt)]
        [TestCase(WeaponConstants.SlingBullet)]
        [TestCase(WeaponConstants.Shuriken)]
        public void BUG_AmmunitionWithQuantityGreaterThan1Happens(string ammo)
        {
            var iterations = 10;
            var quantity = 1;

            while (iterations-- > 0 && quantity == 1)
            {
                var ammunition = weaponGenerator.Generate(ammo);
                itemVerifier.AssertItem(ammunition);
                Assert.That(ammunition.NameMatches(ammo), Is.True, ammunition.Summary);
                Assert.That(ammunition.Attributes, Contains.Item(AttributeConstants.Ammunition), ammunition.Summary);

                quantity = ammunition.Quantity;
            }

            Assert.That(quantity, Is.InRange(2, 50));
        }

        [TestCase(WeaponConstants.Dart)]
        [TestCase(WeaponConstants.Javelin)]
        [TestCase(WeaponConstants.Bolas)]
        [TestCase(WeaponConstants.Shuriken)]
        public void BUG_ThrownRangedWeaponWithQuantityGreaterThan1Happens(string name)
        {
            var iterations = 10;
            var quantity = 1;

            while (iterations-- > 0 && quantity == 1)
            {
                var thrownWeapon = weaponGenerator.Generate(name);
                itemVerifier.AssertItem(thrownWeapon);
                Assert.That(thrownWeapon.NameMatches(name), Is.True, thrownWeapon.Summary);
                Assert.That(thrownWeapon.Attributes, Contains.Item(AttributeConstants.Thrown)
                    .And.Contains(AttributeConstants.Ranged)
                    .And.Not.Contains(AttributeConstants.Melee), thrownWeapon.Summary);

                quantity = thrownWeapon.Quantity;
            }

            var topRange = name == WeaponConstants.Shuriken ? 50 : 20;
            Assert.That(quantity, Is.InRange(2, topRange));
        }

        [Test]
        public void BUG_ShurikenWithQuantityGreaterThan20Happens()
        {
            var iterations = 100;
            var quantity = 1;

            while (iterations-- > 0 && quantity <= 20)
            {
                var shuriken = weaponGenerator.Generate(WeaponConstants.Shuriken);
                itemVerifier.AssertItem(shuriken);
                Assert.That(shuriken.NameMatches(WeaponConstants.Shuriken), Is.True, shuriken.Summary);
                Assert.That(shuriken.Attributes, Contains.Item(AttributeConstants.Thrown)
                    .And.Contains(AttributeConstants.Ranged)
                    .And.Contains(AttributeConstants.Ammunition)
                    .And.Not.Contains(AttributeConstants.Melee), shuriken.Summary);

                quantity = shuriken.Quantity;
            }

            Assert.That(quantity, Is.InRange(21, 50));
        }

        //INFO: This would include weapons that are mostly melee, but can be thrown, such as daggers
        [TestCase(WeaponConstants.Dagger)]
        [TestCase(WeaponConstants.Club)]
        [TestCase(WeaponConstants.Shortspear)]
        [TestCase(WeaponConstants.Spear)]
        [TestCase(WeaponConstants.ThrowingAxe)]
        [TestCase(WeaponConstants.LightHammer)]
        [TestCase(WeaponConstants.Trident)]
        [TestCase(WeaponConstants.Sai)]
        [Repeat(100)]
        public void BUG_ThrownMeleeWeaponDoesNotGetQuantityGreaterThan1(string name)
        {
            var thrownWeapon = weaponGenerator.Generate(name);

            itemVerifier.AssertItem(thrownWeapon);
            Assert.That(thrownWeapon.Attributes, Contains.Item(AttributeConstants.Thrown)
                .And.Contains(AttributeConstants.Ranged)
                .And.Contains(AttributeConstants.Melee), thrownWeapon.Summary);
            Assert.That(thrownWeapon.Quantity, Is.EqualTo(1), thrownWeapon.Summary);
        }
    }
}
