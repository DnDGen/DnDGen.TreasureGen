using DnDGen.Infrastructure.Helpers;
using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Selectors.Selections;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;
using System.Linq;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Mundane.Weapons
{
    [TestFixture]
    public class WeaponDataTests : CollectionsTests
    {
        protected override string tableName => TableNameConstants.Collections.WeaponData;

        [TestCase(WeaponConstants.Gauntlet, 1, "x2", "", "")]
        [TestCase(WeaponConstants.Dagger, 2, "x2", "", "")]
        [TestCase(WeaponConstants.PunchingDagger, 1, "x3", "", "")]
        [TestCase(WeaponConstants.SpikedGauntlet, 1, "x2", "", "")]
        [TestCase(WeaponConstants.LightMace, 1, "x2", "", "")]
        [TestCase(WeaponConstants.Sickle, 1, "x2", "", "")]
        [TestCase(WeaponConstants.Club, 1, "x2", "", "")]
        [TestCase(WeaponConstants.HeavyMace, 1, "x2", "", "")]
        [TestCase(WeaponConstants.Morningstar, 1, "x2", "", "")]
        [TestCase(WeaponConstants.Shortspear, 1, "x2", "", "")]
        [TestCase(WeaponConstants.Longspear, 1, "x3", "", "")]
        [TestCase(WeaponConstants.Quarterstaff, 1, "x2", "", "x2")]
        [TestCase(WeaponConstants.Spear, 1, "x3", "", "")]
        [TestCase(WeaponConstants.HeavyCrossbow, 2, "x2", WeaponConstants.CrossbowBolt, "")]
        [TestCase(WeaponConstants.LightCrossbow, 2, "x2", WeaponConstants.CrossbowBolt, "")]
        [TestCase(WeaponConstants.Dart, 1, "x2", "", "")]
        [TestCase(WeaponConstants.Javelin, 1, "x2", "", "")]
        [TestCase(WeaponConstants.Sling, 1, "x2", WeaponConstants.SlingBullet, "")]
        [TestCase(WeaponConstants.ThrowingAxe, 1, "x2", "", "")]
        [TestCase(WeaponConstants.LightHammer, 1, "x2", "", "")]
        [TestCase(WeaponConstants.Handaxe, 1, "x3", "", "")]
        [TestCase(WeaponConstants.Kukri, 3, "x2", "", "")]
        [TestCase(WeaponConstants.LightPick, 1, "x4", "", "")]
        [TestCase(WeaponConstants.Sap, 1, "x2", "", "")]
        [TestCase(WeaponConstants.ShortSword, 2, "x2", "", "")]
        [TestCase(WeaponConstants.Battleaxe, 1, "x3", "", "")]
        [TestCase(WeaponConstants.Flail, 1, "x2", "", "")]
        [TestCase(WeaponConstants.Longsword, 2, "x2", "", "")]
        [TestCase(WeaponConstants.HeavyPick, 1, "x4", "", "")]
        [TestCase(WeaponConstants.Rapier, 3, "x2", "", "")]
        [TestCase(WeaponConstants.Scimitar, 3, "x2", "", "")]
        [TestCase(WeaponConstants.Trident, 1, "x2", "", "")]
        [TestCase(WeaponConstants.Warhammer, 1, "x3", "", "")]
        [TestCase(WeaponConstants.Falchion, 3, "x2", "", "")]
        [TestCase(WeaponConstants.Glaive, 1, "x3", "", "")]
        [TestCase(WeaponConstants.Greataxe, 1, "x3", "", "")]
        [TestCase(WeaponConstants.Greatclub, 1, "x2", "", "")]
        [TestCase(WeaponConstants.HeavyFlail, 2, "x2", "", "")]
        [TestCase(WeaponConstants.Greatsword, 2, "x2", "", "")]
        [TestCase(WeaponConstants.Guisarme, 1, "x3", "", "")]
        [TestCase(WeaponConstants.Halberd, 1, "x3", "", "")]
        [TestCase(WeaponConstants.Lance, 1, "x3", "", "")]
        [TestCase(WeaponConstants.Ranseur, 1, "x3", "", "")]
        [TestCase(WeaponConstants.Scythe, 1, "x4", "", "")]
        [TestCase(WeaponConstants.Longbow, 1, "x3", WeaponConstants.Arrow, "")]
        [TestCase(WeaponConstants.CompositeLongbow, 1, "x3", WeaponConstants.Arrow, "")]
        [TestCase(WeaponConstants.Shortbow, 1, "x3", WeaponConstants.Arrow, "")]
        [TestCase(WeaponConstants.CompositeShortbow, 1, "x3", WeaponConstants.Arrow, "")]
        [TestCase(WeaponConstants.Kama, 1, "x2", "", "")]
        [TestCase(WeaponConstants.Nunchaku, 1, "x2", "", "")]
        [TestCase(WeaponConstants.Sai, 1, "x2", "", "")]
        [TestCase(WeaponConstants.Siangham, 1, "x2", "", "")]
        [TestCase(WeaponConstants.BastardSword, 2, "x2", "", "")]
        [TestCase(WeaponConstants.DwarvenWaraxe, 1, "x3", "", "")]
        [TestCase(WeaponConstants.Whip, 1, "x2", "", "")]
        [TestCase(WeaponConstants.OrcDoubleAxe, 1, "x3", "", "x3")]
        [TestCase(WeaponConstants.SpikedChain, 1, "x2", "", "")]
        [TestCase(WeaponConstants.DireFlail, 1, "x2", "", "x2")]
        [TestCase(WeaponConstants.GnomeHookedHammer, 1, "x3", "", "x4")]
        [TestCase(WeaponConstants.TwoBladedSword, 2, "x2", "", "x2")]
        [TestCase(WeaponConstants.DwarvenUrgrosh, 1, "x3", "", "x3")]
        [TestCase(WeaponConstants.Bolas, 1, "x2", "", "")]
        [TestCase(WeaponConstants.HandCrossbow, 2, "x2", WeaponConstants.CrossbowBolt, "")]
        [TestCase(WeaponConstants.HeavyRepeatingCrossbow, 2, "x2", WeaponConstants.CrossbowBolt, "")]
        [TestCase(WeaponConstants.LightRepeatingCrossbow, 2, "x2", WeaponConstants.CrossbowBolt, "")]
        [TestCase(WeaponConstants.Net, 1, "x2", "", "")]
        [TestCase(WeaponConstants.Shuriken, 1, "x2", "", "")]
        [TestCase(WeaponConstants.Arrow, 1, "x3", "", "")]
        [TestCase(WeaponConstants.CrossbowBolt, 2, "x2", "", "")]
        [TestCase(WeaponConstants.SlingBullet, 1, "x2", "", "")]
        [TestCase(WeaponConstants.PincerStaff, 1, "x2", "", "")]
        public void WeaponData(string weapon, int threatRange, string criticalMultiplier, string ammunition, string secondaryCriticalMultiplier)
        {
            Assert.That(criticalMultiplier, Is.EqualTo("x2").Or.EqualTo("x3").Or.EqualTo("x4"));

            var doubleWeapons = WeaponConstants.GetAllDouble(false, false);
            if (doubleWeapons.Contains(weapon))
            {
                Assert.That(secondaryCriticalMultiplier, Is.EqualTo("x2").Or.EqualTo("x3").Or.EqualTo("x4"));
            }
            else
            {
                Assert.That(secondaryCriticalMultiplier, Is.Empty);
            }

            var data = DataHelper.Parse(new WeaponDataSelection
            {
                CriticalMultiplier = criticalMultiplier,
                SecondaryCriticalMultiplier = secondaryCriticalMultiplier,
                ThreatRange = threatRange,
                Ammunition = ammunition,
            });
            AssertOrderedCollections(weapon, data);
        }

        [Test]
        public void AllWeaponsAreInTable()
        {
            var weapons = WeaponConstants.GetAllWeapons(false, false);
            AssertCollection(table.Keys, weapons);
        }
    }
}