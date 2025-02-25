using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Mundane.Weapons
{
    [TestFixture]
    public class WeaponAttributesTests : CollectionsTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Collections.ITEMTYPEAttributes(ItemTypeConstants.Weapon); }
        }

        [TestCase(WeaponConstants.Dagger,
            AttributeConstants.Metal,
            AttributeConstants.Simple,
            AttributeConstants.Melee,
            AttributeConstants.Ranged,
            AttributeConstants.Light,
            AttributeConstants.Thrown)]
        [TestCase(WeaponConstants.Greataxe,
            AttributeConstants.Metal,
            AttributeConstants.Martial,
            AttributeConstants.Melee,
            AttributeConstants.TwoHanded)]
        [TestCase(WeaponConstants.Greatsword,
            AttributeConstants.Metal,
            AttributeConstants.Martial,
            AttributeConstants.Melee,
            AttributeConstants.TwoHanded)]
        [TestCase(WeaponConstants.Kama,
            AttributeConstants.Wood,
            AttributeConstants.Metal,
            AttributeConstants.Exotic,
            AttributeConstants.Light,
            AttributeConstants.Melee)]
        [TestCase(WeaponConstants.Longsword,
            AttributeConstants.Metal,
            AttributeConstants.Martial,
            AttributeConstants.OneHanded,
            AttributeConstants.Melee)]
        [TestCase(WeaponConstants.LightMace,
            AttributeConstants.Metal,
            AttributeConstants.Simple,
            AttributeConstants.Light,
            AttributeConstants.Melee)]
        [TestCase(WeaponConstants.HeavyMace,
            AttributeConstants.Metal,
            AttributeConstants.Simple,
            AttributeConstants.OneHanded,
            AttributeConstants.Melee)]
        [TestCase(WeaponConstants.Nunchaku,
            AttributeConstants.Wood,
            AttributeConstants.Exotic,
            AttributeConstants.Light,
            AttributeConstants.Melee)]
        [TestCase(WeaponConstants.Quarterstaff,
            AttributeConstants.Wood,
            AttributeConstants.Simple,
            AttributeConstants.DoubleWeapon,
            AttributeConstants.Melee,
            AttributeConstants.TwoHanded)]
        [TestCase(WeaponConstants.Rapier,
            AttributeConstants.Metal,
            AttributeConstants.Martial,
            AttributeConstants.OneHanded,
            AttributeConstants.Melee)]
        [TestCase(WeaponConstants.Scimitar,
            AttributeConstants.Metal,
            AttributeConstants.Martial,
            AttributeConstants.OneHanded,
            AttributeConstants.Melee)]
        [TestCase(WeaponConstants.Shortspear,
            AttributeConstants.Wood,
            AttributeConstants.Metal,
            AttributeConstants.Simple,
            AttributeConstants.Melee,
            AttributeConstants.Ranged,
            AttributeConstants.OneHanded,
            AttributeConstants.Thrown)]
        [TestCase(WeaponConstants.Siangham,
            AttributeConstants.Metal,
            AttributeConstants.Exotic,
            AttributeConstants.Light,
            AttributeConstants.Melee)]
        [TestCase(WeaponConstants.BastardSword,
            AttributeConstants.Metal,
            AttributeConstants.Exotic,
            AttributeConstants.OneHanded,
            AttributeConstants.TwoHanded,
            AttributeConstants.Melee)]
        [TestCase(WeaponConstants.ShortSword,
            AttributeConstants.Metal,
            AttributeConstants.Martial,
            AttributeConstants.Light,
            AttributeConstants.Melee)]
        [TestCase(WeaponConstants.DwarvenWaraxe,
            AttributeConstants.Metal,
            AttributeConstants.Exotic,
            AttributeConstants.OneHanded,
            AttributeConstants.Melee)]
        [TestCase(WeaponConstants.OrcDoubleAxe,
            AttributeConstants.Metal,
            AttributeConstants.Exotic,
            AttributeConstants.DoubleWeapon,
            AttributeConstants.Melee,
            AttributeConstants.TwoHanded)]
        [TestCase(WeaponConstants.Battleaxe,
            AttributeConstants.Metal,
            AttributeConstants.Martial,
            AttributeConstants.OneHanded,
            AttributeConstants.Melee)]
        [TestCase(WeaponConstants.SpikedChain,
            AttributeConstants.Metal,
            AttributeConstants.Exotic,
            AttributeConstants.Melee,
            AttributeConstants.Reach,
            AttributeConstants.TwoHanded)]
        [TestCase(WeaponConstants.Club,
            AttributeConstants.Wood,
            AttributeConstants.Simple,
            AttributeConstants.Melee,
            AttributeConstants.Ranged,
            AttributeConstants.OneHanded,
            AttributeConstants.Thrown)]
        [TestCase(WeaponConstants.HandCrossbow,
            AttributeConstants.Metal,
            AttributeConstants.Exotic,
            AttributeConstants.Projectile,
            AttributeConstants.Ranged)]
        [TestCase(WeaponConstants.HeavyRepeatingCrossbow,
            AttributeConstants.Metal,
            AttributeConstants.Exotic,
            AttributeConstants.Ranged,
            AttributeConstants.Projectile)]
        [TestCase(WeaponConstants.LightRepeatingCrossbow,
            AttributeConstants.Metal,
            AttributeConstants.Exotic,
            AttributeConstants.Ranged,
            AttributeConstants.Projectile)]
        [TestCase(WeaponConstants.PunchingDagger,
            AttributeConstants.Metal,
            AttributeConstants.Simple,
            AttributeConstants.Light,
            AttributeConstants.Melee)]
        [TestCase(WeaponConstants.Falchion,
            AttributeConstants.Metal,
            AttributeConstants.Martial,
            AttributeConstants.Melee,
            AttributeConstants.TwoHanded)]
        [TestCase(WeaponConstants.DireFlail,
            AttributeConstants.Metal,
            AttributeConstants.Exotic,
            AttributeConstants.Melee,
            AttributeConstants.DoubleWeapon,
            AttributeConstants.TwoHanded)]
        [TestCase(WeaponConstants.HeavyFlail,
            AttributeConstants.Metal,
            AttributeConstants.Martial,
            AttributeConstants.Melee,
            AttributeConstants.TwoHanded)]
        [TestCase(WeaponConstants.Flail,
            AttributeConstants.Metal,
            AttributeConstants.Martial,
            AttributeConstants.OneHanded,
            AttributeConstants.Melee)]
        [TestCase(WeaponConstants.Gauntlet,
            AttributeConstants.Metal,
            AttributeConstants.Simple,
            AttributeConstants.Light,
            AttributeConstants.Melee)]
        [TestCase(WeaponConstants.SpikedGauntlet,
            AttributeConstants.Metal,
            AttributeConstants.Simple,
            AttributeConstants.Light,
            AttributeConstants.Melee)]
        [TestCase(WeaponConstants.Glaive,
            AttributeConstants.Metal,
            AttributeConstants.Martial,
            AttributeConstants.Melee,
            AttributeConstants.Reach,
            AttributeConstants.TwoHanded)]
        [TestCase(WeaponConstants.Greatclub,
            AttributeConstants.Wood,
            AttributeConstants.Martial,
            AttributeConstants.Melee,
            AttributeConstants.TwoHanded)]
        [TestCase(WeaponConstants.Guisarme,
            AttributeConstants.Metal,
            AttributeConstants.Martial,
            AttributeConstants.Melee,
            AttributeConstants.Reach,
            AttributeConstants.TwoHanded)]
        [TestCase(WeaponConstants.Halberd,
            AttributeConstants.Metal,
            AttributeConstants.Martial,
            AttributeConstants.Melee,
            AttributeConstants.TwoHanded)]
        [TestCase(WeaponConstants.Spear,
            AttributeConstants.Metal,
            AttributeConstants.Wood,
            AttributeConstants.Simple,
            AttributeConstants.Ranged,
            AttributeConstants.Thrown,
            AttributeConstants.Melee,
            AttributeConstants.TwoHanded)]
        [TestCase(WeaponConstants.GnomeHookedHammer,
            AttributeConstants.Metal,
            AttributeConstants.Exotic,
            AttributeConstants.Melee,
            AttributeConstants.DoubleWeapon,
            AttributeConstants.TwoHanded)]
        [TestCase(WeaponConstants.LightHammer,
            AttributeConstants.Metal,
            AttributeConstants.Martial,
            AttributeConstants.Melee,
            AttributeConstants.Ranged,
            AttributeConstants.Light,
            AttributeConstants.Thrown)]
        [TestCase(WeaponConstants.Handaxe,
            AttributeConstants.Metal,
            AttributeConstants.Martial,
            AttributeConstants.Light,
            AttributeConstants.Melee)]
        [TestCase(WeaponConstants.Kukri,
            AttributeConstants.Metal,
            AttributeConstants.Martial,
            AttributeConstants.Light,
            AttributeConstants.Melee)]
        [TestCase(WeaponConstants.Lance,
            AttributeConstants.Wood,
            AttributeConstants.Metal,
            AttributeConstants.Martial,
            AttributeConstants.Melee,
            AttributeConstants.Reach,
            AttributeConstants.TwoHanded)]
        [TestCase(WeaponConstants.Longspear,
            AttributeConstants.Wood,
            AttributeConstants.Metal,
            AttributeConstants.Simple,
            AttributeConstants.Melee,
            AttributeConstants.Reach,
            AttributeConstants.TwoHanded)]
        [TestCase(WeaponConstants.Morningstar,
            AttributeConstants.Metal,
            AttributeConstants.Simple,
            AttributeConstants.OneHanded,
            AttributeConstants.Melee)]
        [TestCase(WeaponConstants.Net,
            AttributeConstants.Exotic,
            AttributeConstants.Ranged,
            AttributeConstants.Thrown)]
        [TestCase(WeaponConstants.HeavyPick,
            AttributeConstants.Metal,
            AttributeConstants.Martial,
            AttributeConstants.OneHanded,
            AttributeConstants.Melee)]
        [TestCase(WeaponConstants.LightPick,
            AttributeConstants.Metal,
            AttributeConstants.Martial,
            AttributeConstants.Light,
            AttributeConstants.Melee)]
        [TestCase(WeaponConstants.Ranseur,
            AttributeConstants.Wood,
            AttributeConstants.Metal,
            AttributeConstants.Martial,
            AttributeConstants.Melee,
            AttributeConstants.Reach,
            AttributeConstants.TwoHanded)]
        [TestCase(WeaponConstants.Sap,
            AttributeConstants.Martial,
            AttributeConstants.Light,
            AttributeConstants.Melee)]
        [TestCase(WeaponConstants.Scythe,
            AttributeConstants.Wood,
            AttributeConstants.Metal,
            AttributeConstants.Martial,
            AttributeConstants.Melee,
            AttributeConstants.TwoHanded)]
        [TestCase(WeaponConstants.Shuriken,
            AttributeConstants.Metal,
            AttributeConstants.Exotic,
            AttributeConstants.Ranged,
            AttributeConstants.Ammunition,
            AttributeConstants.Thrown)]
        [TestCase(WeaponConstants.Sickle,
            AttributeConstants.Metal,
            AttributeConstants.Simple,
            AttributeConstants.Light,
            AttributeConstants.Melee)]
        [TestCase(WeaponConstants.TwoBladedSword,
            AttributeConstants.Metal,
            AttributeConstants.Exotic,
            AttributeConstants.Melee,
            AttributeConstants.DoubleWeapon,
            AttributeConstants.TwoHanded)]
        [TestCase(WeaponConstants.Trident,
            AttributeConstants.Metal,
            AttributeConstants.Martial,
            AttributeConstants.Melee,
            AttributeConstants.Ranged,
            AttributeConstants.OneHanded,
            AttributeConstants.Thrown)]
        [TestCase(WeaponConstants.DwarvenUrgrosh,
            AttributeConstants.Metal,
            AttributeConstants.Exotic,
            AttributeConstants.Melee,
            AttributeConstants.DoubleWeapon,
            AttributeConstants.TwoHanded)]
        [TestCase(WeaponConstants.Warhammer,
            AttributeConstants.Metal,
            AttributeConstants.Martial,
            AttributeConstants.OneHanded,
            AttributeConstants.Melee)]
        [TestCase(WeaponConstants.Whip,
            AttributeConstants.Exotic,
            AttributeConstants.Reach,
            AttributeConstants.OneHanded,
            AttributeConstants.Melee)]
        [TestCase(WeaponConstants.ThrowingAxe,
            AttributeConstants.Martial,
            AttributeConstants.Ranged,
            AttributeConstants.Melee,
            AttributeConstants.Metal,
            AttributeConstants.Light,
            AttributeConstants.Thrown)]
        [TestCase(WeaponConstants.HeavyCrossbow,
            AttributeConstants.Simple,
            AttributeConstants.Ranged,
            AttributeConstants.Wood,
            AttributeConstants.Projectile)]
        [TestCase(WeaponConstants.LightCrossbow,
            AttributeConstants.Simple,
            AttributeConstants.Ranged,
            AttributeConstants.Wood,
            AttributeConstants.Projectile)]
        [TestCase(WeaponConstants.Dart,
            AttributeConstants.Simple,
            AttributeConstants.Ranged,
            AttributeConstants.Metal,
            AttributeConstants.Thrown)]
        [TestCase(WeaponConstants.Javelin,
            AttributeConstants.Simple,
            AttributeConstants.Ranged,
            AttributeConstants.Metal,
            AttributeConstants.Wood,
            AttributeConstants.Thrown)]
        [TestCase(WeaponConstants.Shortbow,
            AttributeConstants.Martial,
            AttributeConstants.Ranged,
            AttributeConstants.Wood,
            AttributeConstants.Projectile)]
        [TestCase(WeaponConstants.CompositeShortbow,
            AttributeConstants.Martial,
            AttributeConstants.Ranged,
            AttributeConstants.Wood,
            AttributeConstants.Projectile)]
        [TestCase(WeaponConstants.Sling,
            AttributeConstants.Simple,
            AttributeConstants.Ranged,
            AttributeConstants.Projectile)]
        [TestCase(WeaponConstants.Longbow,
            AttributeConstants.Martial,
            AttributeConstants.Ranged,
            AttributeConstants.Wood,
            AttributeConstants.Projectile)]
        [TestCase(WeaponConstants.CompositeLongbow,
            AttributeConstants.Martial,
            AttributeConstants.Ranged,
            AttributeConstants.Wood,
            AttributeConstants.Projectile)]
        [TestCase(WeaponConstants.Arrow,
            AttributeConstants.Martial,
            AttributeConstants.Ranged,
            AttributeConstants.Ammunition,
            AttributeConstants.Metal,
            AttributeConstants.Wood)]
        [TestCase(WeaponConstants.CrossbowBolt,
            AttributeConstants.Simple,
            AttributeConstants.Ranged,
            AttributeConstants.Ammunition,
            AttributeConstants.Metal)]
        [TestCase(WeaponConstants.SlingBullet,
            AttributeConstants.Simple,
            AttributeConstants.Ranged,
            AttributeConstants.Ammunition,
            AttributeConstants.Metal)]
        [TestCase(WeaponConstants.Sai,
            AttributeConstants.Exotic,
            AttributeConstants.Melee,
            AttributeConstants.Ranged,
            AttributeConstants.Thrown,
            AttributeConstants.Light,
            AttributeConstants.Metal)]
        [TestCase(WeaponConstants.Bolas,
            AttributeConstants.Exotic,
            AttributeConstants.Ranged,
            AttributeConstants.Thrown)]
        [TestCase(WeaponConstants.PincerStaff,
            AttributeConstants.Metal,
            AttributeConstants.Exotic,
            AttributeConstants.Melee,
            AttributeConstants.Reach,
            AttributeConstants.TwoHanded)]
        public void WeaponAttributes(string name, params string[] attributes)
        {
            base.AssertCollection(name, attributes);
        }

        [Test]
        public void AllWeaponsAreInTable()
        {
            var weapons = WeaponConstants.GetAllWeapons(false, false);
            AssertCollection(table.Keys, weapons);
        }

        [Test]
        public void MeleeWeaponsMatchConstants()
        {
            var melee = WeaponConstants.GetAllMelee(false, false);
            VerifyAttribute(melee, AttributeConstants.Melee);
        }

        private void VerifyAttribute(IEnumerable<string> weaponsWith, string attribute)
        {
            var entriesWith = table.Where(kvp => kvp.Value.Contains(attribute));
            var entriesWithout = table.Except(entriesWith);

            var weapons = WeaponConstants.GetAllWeapons(false, false);
            var weaponsWithout = weapons.Except(weaponsWith);

            AssertCollection(entriesWith.Select(kvp => kvp.Key), weaponsWith);
            AssertCollection(entriesWithout.Select(kvp => kvp.Key), weaponsWithout);
        }

        [Test]
        public void RangedWeaponsMatchConstants()
        {
            var ranged = WeaponConstants.GetAllRanged(false, false, true);
            VerifyAttribute(ranged, AttributeConstants.Ranged);
        }

        [Test]
        public void SimpleWeaponsMatchConstants()
        {
            var simple = WeaponConstants.GetAllSimple(false, false);
            VerifyAttribute(simple, AttributeConstants.Simple);
        }

        [Test]
        public void MartialWeaponsMatchConstants()
        {
            var martial = WeaponConstants.GetAllMartial(false, false);
            VerifyAttribute(martial, AttributeConstants.Martial);
        }

        [Test]
        public void ExoticWeaponsMatchConstants()
        {
            var exotic = WeaponConstants.GetAllExotic(false, false);
            VerifyAttribute(exotic, AttributeConstants.Exotic);
        }

        [Test]
        public void LightWeaponsMatchConstants()
        {
            var light = WeaponConstants.GetAllLightMelee(false, false);
            VerifyAttribute(light, AttributeConstants.Light);
        }

        [Test]
        public void OneHandedWeaponsMatchConstants()
        {
            var oneHanded = WeaponConstants.GetAllOneHandedMelee(false, false);
            VerifyAttribute(oneHanded, AttributeConstants.OneHanded);
        }

        [Test]
        public void TwoHandedWeaponsMatchConstants()
        {
            var twoHanded = WeaponConstants.GetAllTwoHandedMelee(false, false);
            VerifyAttribute(twoHanded, AttributeConstants.TwoHanded);
        }

        [Test]
        public void DoubleWeaponsMatchConstants()
        {
            var doubleWeapons = WeaponConstants.GetAllDouble(false, false);
            VerifyAttribute(doubleWeapons, AttributeConstants.DoubleWeapon);
        }

        [Test]
        public void ReachWeaponsMatchConstants()
        {
            var reach = WeaponConstants.GetAllReach(false, false);
            VerifyAttribute(reach, AttributeConstants.Reach);
        }

        [Test]
        public void ThrownWeaponsMatchConstants()
        {
            var thrown = WeaponConstants.GetAllThrown(false, false);
            VerifyAttribute(thrown, AttributeConstants.Thrown);
        }

        [Test]
        public void ProjectileWeaponsMatchConstants()
        {
            var projectiles = WeaponConstants.GetAllProjectile(false, false);
            VerifyAttribute(projectiles, AttributeConstants.Projectile);
        }

        [Test]
        public void AmmunitionWeaponsMatchConstants()
        {
            var ammunitions = WeaponConstants.GetAllAmmunition(false, false);
            VerifyAttribute(ammunitions, AttributeConstants.Ammunition);
        }
    }
}