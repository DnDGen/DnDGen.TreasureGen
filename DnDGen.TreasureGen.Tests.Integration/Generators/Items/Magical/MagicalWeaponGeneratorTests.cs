using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Tests.Unit.Generators.Items;
using Ninject;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Generators.Items.Magical
{
    [TestFixture]
    public class MagicalWeaponGeneratorTests : IntegrationTests
    {
        [Inject, Named(ItemTypeConstants.Weapon)]
        public MagicalItemGenerator WeaponGenerator { get; set; }

        private ItemVerifier itemVerifier;

        [SetUp]
        public void Setup()
        {
            itemVerifier = new ItemVerifier();
        }

        [TestCaseSource(typeof(ItemPowerTestData), nameof(ItemPowerTestData.Weapons))]
        public void GenerateWeapon(string itemName, string power)
        {
            var item = WeaponGenerator.Generate(power, itemName);
            itemVerifier.AssertItem(item);
        }

        [TestCase(WeaponConstants.Longsword, PowerConstants.Minor, TraitConstants.Sizes.Colossal)]
        [TestCase(WeaponConstants.Longsword, PowerConstants.Minor, TraitConstants.Sizes.Gargantuan)]
        [TestCase(WeaponConstants.Longsword, PowerConstants.Minor, TraitConstants.Sizes.Huge)]
        [TestCase(WeaponConstants.Longsword, PowerConstants.Minor, TraitConstants.Sizes.Large)]
        [TestCase(WeaponConstants.Longsword, PowerConstants.Minor, TraitConstants.Sizes.Medium)]
        [TestCase(WeaponConstants.Longsword, PowerConstants.Minor, TraitConstants.Sizes.Small)]
        [TestCase(WeaponConstants.Longsword, PowerConstants.Minor, TraitConstants.Sizes.Tiny)]
        [TestCase(WeaponConstants.Longsword, PowerConstants.Medium, TraitConstants.Sizes.Colossal)]
        [TestCase(WeaponConstants.Longsword, PowerConstants.Medium, TraitConstants.Sizes.Gargantuan)]
        [TestCase(WeaponConstants.Longsword, PowerConstants.Medium, TraitConstants.Sizes.Huge)]
        [TestCase(WeaponConstants.Longsword, PowerConstants.Medium, TraitConstants.Sizes.Large)]
        [TestCase(WeaponConstants.Longsword, PowerConstants.Medium, TraitConstants.Sizes.Medium)]
        [TestCase(WeaponConstants.Longsword, PowerConstants.Medium, TraitConstants.Sizes.Small)]
        [TestCase(WeaponConstants.Longsword, PowerConstants.Medium, TraitConstants.Sizes.Tiny)]
        [TestCase(WeaponConstants.Longsword, PowerConstants.Major, TraitConstants.Sizes.Colossal)]
        [TestCase(WeaponConstants.Longsword, PowerConstants.Major, TraitConstants.Sizes.Gargantuan)]
        [TestCase(WeaponConstants.Longsword, PowerConstants.Major, TraitConstants.Sizes.Huge)]
        [TestCase(WeaponConstants.Longsword, PowerConstants.Major, TraitConstants.Sizes.Large)]
        [TestCase(WeaponConstants.Longsword, PowerConstants.Major, TraitConstants.Sizes.Medium)]
        [TestCase(WeaponConstants.Longsword, PowerConstants.Major, TraitConstants.Sizes.Small)]
        [TestCase(WeaponConstants.Longsword, PowerConstants.Major, TraitConstants.Sizes.Tiny)]
        public void GenerateWeaponOfSize(string itemName, string power, string size)
        {
            var item = WeaponGenerator.Generate(power, itemName, "my trait", size);
            itemVerifier.AssertItem(item);
            Assert.That(item, Is.InstanceOf<Weapon>());

            var weapon = item as Weapon;
            Assert.That(weapon.ItemType, Is.EqualTo(ItemTypeConstants.Weapon), $"{weapon.Name} {weapon.Magic.Curse}");
            Assert.That(weapon.NameMatches(itemName), Is.True, $"{weapon.Name} {weapon.Magic.Curse}");
            Assert.That(weapon.Size, Is.EqualTo(size), $"{weapon.Name} {weapon.Magic.Curse}");
            Assert.That(weapon.Traits, Does.Not.Contain(size)
                .And.Contains("my trait"), $"{weapon.Name} {weapon.Magic.Curse}");
        }

        [TestCase(PowerConstants.Minor, WeaponConstants.CompositeLongbow, WeaponConstants.Arrow)]
        [TestCase(PowerConstants.Minor, WeaponConstants.CompositeLongbow_StrengthPlus0, WeaponConstants.Arrow)]
        [TestCase(PowerConstants.Minor, WeaponConstants.CompositeLongbow_StrengthPlus1, WeaponConstants.Arrow)]
        [TestCase(PowerConstants.Minor, WeaponConstants.CompositeLongbow_StrengthPlus2, WeaponConstants.Arrow)]
        [TestCase(PowerConstants.Minor, WeaponConstants.CompositeLongbow_StrengthPlus3, WeaponConstants.Arrow)]
        [TestCase(PowerConstants.Minor, WeaponConstants.CompositeLongbow_StrengthPlus4, WeaponConstants.Arrow)]
        [TestCase(PowerConstants.Minor, WeaponConstants.CompositeShortbow, WeaponConstants.Arrow)]
        [TestCase(PowerConstants.Minor, WeaponConstants.CompositeShortbow_StrengthPlus0, WeaponConstants.Arrow)]
        [TestCase(PowerConstants.Minor, WeaponConstants.CompositeShortbow_StrengthPlus1, WeaponConstants.Arrow)]
        [TestCase(PowerConstants.Minor, WeaponConstants.CompositeShortbow_StrengthPlus2, WeaponConstants.Arrow)]
        [TestCase(PowerConstants.Minor, WeaponConstants.HandCrossbow, WeaponConstants.CrossbowBolt)]
        [TestCase(PowerConstants.Minor, WeaponConstants.HeavyCrossbow, WeaponConstants.CrossbowBolt)]
        [TestCase(PowerConstants.Minor, WeaponConstants.HeavyRepeatingCrossbow, WeaponConstants.CrossbowBolt)]
        [TestCase(PowerConstants.Minor, WeaponConstants.LightCrossbow, WeaponConstants.CrossbowBolt)]
        [TestCase(PowerConstants.Minor, WeaponConstants.LightRepeatingCrossbow, WeaponConstants.CrossbowBolt)]
        [TestCase(PowerConstants.Minor, WeaponConstants.Longbow, WeaponConstants.Arrow)]
        [TestCase(PowerConstants.Minor, WeaponConstants.Shortbow, WeaponConstants.Arrow)]
        [TestCase(PowerConstants.Minor, WeaponConstants.Sling, WeaponConstants.SlingBullet)]
        [TestCase(PowerConstants.Medium, WeaponConstants.CompositeLongbow, WeaponConstants.Arrow)]
        [TestCase(PowerConstants.Medium, WeaponConstants.CompositeLongbow_StrengthPlus0, WeaponConstants.Arrow)]
        [TestCase(PowerConstants.Medium, WeaponConstants.CompositeLongbow_StrengthPlus1, WeaponConstants.Arrow)]
        [TestCase(PowerConstants.Medium, WeaponConstants.CompositeLongbow_StrengthPlus2, WeaponConstants.Arrow)]
        [TestCase(PowerConstants.Medium, WeaponConstants.CompositeLongbow_StrengthPlus3, WeaponConstants.Arrow)]
        [TestCase(PowerConstants.Medium, WeaponConstants.CompositeLongbow_StrengthPlus4, WeaponConstants.Arrow)]
        [TestCase(PowerConstants.Medium, WeaponConstants.CompositeShortbow, WeaponConstants.Arrow)]
        [TestCase(PowerConstants.Medium, WeaponConstants.CompositeShortbow_StrengthPlus0, WeaponConstants.Arrow)]
        [TestCase(PowerConstants.Medium, WeaponConstants.CompositeShortbow_StrengthPlus1, WeaponConstants.Arrow)]
        [TestCase(PowerConstants.Medium, WeaponConstants.CompositeShortbow_StrengthPlus2, WeaponConstants.Arrow)]
        [TestCase(PowerConstants.Medium, WeaponConstants.HandCrossbow, WeaponConstants.CrossbowBolt)]
        [TestCase(PowerConstants.Medium, WeaponConstants.HeavyCrossbow, WeaponConstants.CrossbowBolt)]
        [TestCase(PowerConstants.Medium, WeaponConstants.HeavyRepeatingCrossbow, WeaponConstants.CrossbowBolt)]
        [TestCase(PowerConstants.Medium, WeaponConstants.LightCrossbow, WeaponConstants.CrossbowBolt)]
        [TestCase(PowerConstants.Medium, WeaponConstants.LightRepeatingCrossbow, WeaponConstants.CrossbowBolt)]
        [TestCase(PowerConstants.Medium, WeaponConstants.Longbow, WeaponConstants.Arrow)]
        [TestCase(PowerConstants.Medium, WeaponConstants.Shortbow, WeaponConstants.Arrow)]
        [TestCase(PowerConstants.Medium, WeaponConstants.Sling, WeaponConstants.SlingBullet)]
        [TestCase(PowerConstants.Major, WeaponConstants.CompositeLongbow, WeaponConstants.Arrow)]
        [TestCase(PowerConstants.Major, WeaponConstants.CompositeLongbow_StrengthPlus0, WeaponConstants.Arrow)]
        [TestCase(PowerConstants.Major, WeaponConstants.CompositeLongbow_StrengthPlus1, WeaponConstants.Arrow)]
        [TestCase(PowerConstants.Major, WeaponConstants.CompositeLongbow_StrengthPlus2, WeaponConstants.Arrow)]
        [TestCase(PowerConstants.Major, WeaponConstants.CompositeLongbow_StrengthPlus3, WeaponConstants.Arrow)]
        [TestCase(PowerConstants.Major, WeaponConstants.CompositeLongbow_StrengthPlus4, WeaponConstants.Arrow)]
        [TestCase(PowerConstants.Major, WeaponConstants.CompositeShortbow, WeaponConstants.Arrow)]
        [TestCase(PowerConstants.Major, WeaponConstants.CompositeShortbow_StrengthPlus0, WeaponConstants.Arrow)]
        [TestCase(PowerConstants.Major, WeaponConstants.CompositeShortbow_StrengthPlus1, WeaponConstants.Arrow)]
        [TestCase(PowerConstants.Major, WeaponConstants.CompositeShortbow_StrengthPlus2, WeaponConstants.Arrow)]
        [TestCase(PowerConstants.Major, WeaponConstants.HandCrossbow, WeaponConstants.CrossbowBolt)]
        [TestCase(PowerConstants.Major, WeaponConstants.HeavyCrossbow, WeaponConstants.CrossbowBolt)]
        [TestCase(PowerConstants.Major, WeaponConstants.HeavyRepeatingCrossbow, WeaponConstants.CrossbowBolt)]
        [TestCase(PowerConstants.Major, WeaponConstants.LightCrossbow, WeaponConstants.CrossbowBolt)]
        [TestCase(PowerConstants.Major, WeaponConstants.LightRepeatingCrossbow, WeaponConstants.CrossbowBolt)]
        [TestCase(PowerConstants.Major, WeaponConstants.Longbow, WeaponConstants.Arrow)]
        [TestCase(PowerConstants.Major, WeaponConstants.Shortbow, WeaponConstants.Arrow)]
        [TestCase(PowerConstants.Major, WeaponConstants.Sling, WeaponConstants.SlingBullet)]
        public void GenerateWeaponWithAmmunition(string power, string weaponName, string ammunition)
        {
            var item = WeaponGenerator.Generate(power, weaponName);
            itemVerifier.AssertItem(item);
            Assert.That(item, Is.InstanceOf<Weapon>(), item.Name);

            var weapon = item as Weapon;
            Assert.That(weapon.Ammunition, Is.EqualTo(ammunition), item.Name);
        }

        [TestCase(WeaponConstants.Quarterstaff)]
        [TestCase(WeaponConstants.TwoBladedSword)]
        [TestCase(WeaponConstants.OrcDoubleAxe)]
        [TestCase(WeaponConstants.GnomeHookedHammer)]
        [TestCase(WeaponConstants.DwarvenUrgrosh)]
        [TestCase(WeaponConstants.DireFlail)]
        [TestCase(WeaponConstants.ShiftersSorrow)]
        public void GenerateMagicalDoubleWeapon(string weaponName)
        {
            var item = WeaponGenerator.Generate(PowerConstants.Major, weaponName);
            itemVerifier.AssertItem(item);
            Assert.That(item.Attributes, Contains.Item(AttributeConstants.DoubleWeapon));
            Assert.That(item, Is.InstanceOf<Weapon>(), item.Name);

            var weapon = item as Weapon;
            Assert.That(weapon.IsDoubleWeapon, Is.True);
            Assert.That(weapon.SecondaryCriticalDamageSummary, Is.Not.Empty);
            Assert.That(weapon.SecondaryCriticalDamageRoll, Is.Not.Empty);
            Assert.That(weapon.SecondaryCriticalDamages, Is.Not.Empty);
            Assert.That(weapon.SecondaryCriticalMultiplier, Is.Not.Empty);
            Assert.That(weapon.SecondaryDamageSummary, Is.Not.Empty);
            Assert.That(weapon.SecondaryDamageRoll, Is.Not.Empty);
            Assert.That(weapon.SecondaryDamages, Is.Not.Empty);
            Assert.That(weapon.SecondaryMagicBonus, Is.Positive);
        }

        [TestCase(WeaponConstants.ShiftersSorrow)]
        public void GenerateSpecificMagicalDoubleWeapon(string weaponName)
        {
            var item = WeaponGenerator.Generate(PowerConstants.Major, weaponName);
            itemVerifier.AssertItem(item);
            Assert.That(item.Attributes, Contains.Item(AttributeConstants.DoubleWeapon));
            Assert.That(item, Is.InstanceOf<Weapon>(), item.Name);

            var weapon = item as Weapon;
            Assert.That(weapon.IsDoubleWeapon, Is.True);
            Assert.That(weapon.SecondaryCriticalDamageSummary, Is.Not.Empty);
            Assert.That(weapon.SecondaryCriticalDamageRoll, Is.Not.Empty);
            Assert.That(weapon.SecondaryCriticalDamages, Is.Not.Empty);
            Assert.That(weapon.SecondaryCriticalMultiplier, Is.Not.Empty);
            Assert.That(weapon.SecondaryDamageSummary, Is.Not.Empty);
            Assert.That(weapon.SecondaryDamageRoll, Is.Not.Empty);
            Assert.That(weapon.SecondaryDamages, Is.Not.Empty);
            Assert.That(weapon.SecondaryMagicBonus, Is.EqualTo(1).And.EqualTo(weapon.Magic.Bonus));
            Assert.That(weapon.SecondaryHasAbilities, Is.True);
        }

        [TestCase(WeaponConstants.FlameTongue)]
        [TestCase(WeaponConstants.FrostBrand)]
        public void GenerateSpecificMagicalWeaponWithExtraDamage(string weaponName)
        {
            var item = WeaponGenerator.Generate(PowerConstants.Major, weaponName);
            itemVerifier.AssertItem(item);
            Assert.That(item, Is.InstanceOf<Weapon>(), item.Name);

            var weapon = item as Weapon;
            Assert.That(weapon.Damages, Has.Count.GreaterThan(1), weapon.DamageSummary);
            Assert.That(weapon.CriticalDamages, Has.Count.GreaterThan(1), weapon.CriticalDamageSummary);
        }

        [TestCaseSource(typeof(ItemPowerTestData), nameof(ItemPowerTestData.SpecificWeapons))]
        public void BUG_GenerateWeapon_SpecificWeaponHasQuantity(string itemName, string power)
        {
            var item = WeaponGenerator.Generate(power, itemName);
            itemVerifier.AssertItem(item);
            Assert.That(item, Is.InstanceOf<Weapon>(), item.Name);
            Assert.That(item.Quantity, Is.AtLeast(1), item.Name);
        }
    }
}
