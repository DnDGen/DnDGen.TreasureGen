using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Mundane.Weapons
{
    [TestFixture]
    public class UncommonWeaponsTests : PercentileTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Percentiles.WEAPONTYPEWeapons("Uncommon"); }
        }

        [Test]
        public override void ReplacementStringsAreValid()
        {
            AssertReplacementStringsAreValid();
        }

        [Test]
        public override void TableIsComplete()
        {
            AssertTableIsComplete();
        }

        [TestCase(WeaponConstants.OrcDoubleAxe, 1, 3)]
        [TestCase(WeaponConstants.Battleaxe, 4, 7)]
        [TestCase(WeaponConstants.SpikedChain, 8, 10)]
        [TestCase(WeaponConstants.Club, 11, 12)]
        [TestCase(WeaponConstants.HandCrossbow, 13, 16)]
        [TestCase(WeaponConstants.LightRepeatingCrossbow, 17, 18)]
        [TestCase(WeaponConstants.PunchingDagger, 20, 21)]
        [TestCase(WeaponConstants.Falchion, 22, 23)]
        [TestCase(WeaponConstants.DireFlail, 24, 26)]
        [TestCase(WeaponConstants.HeavyFlail, 27, 29)]
        [TestCase(WeaponConstants.Flail, 30, 33)]
        [TestCase(WeaponConstants.Sai, 34, 34)]
        [TestCase(WeaponConstants.Bolas, 35, 35)]
        [TestCase(WeaponConstants.Gauntlet, 36, 37)]
        [TestCase(WeaponConstants.SpikedGauntlet, 38, 39)]
        [TestCase(WeaponConstants.Glaive, 40, 41)]
        [TestCase(WeaponConstants.Greatclub, 42, 43)]
        [TestCase(WeaponConstants.Guisarme, 44, 45)]
        [TestCase(WeaponConstants.Halberd, 46, 48)]
        [TestCase(WeaponConstants.Spear, 49, 51)]
        [TestCase(WeaponConstants.GnomeHookedHammer, 52, 54)]
        [TestCase(WeaponConstants.LightHammer, 55, 56)]
        [TestCase(WeaponConstants.Handaxe, 57, 58)]
        [TestCase(WeaponConstants.Kukri, 59, 61)]
        [TestCase(WeaponConstants.Lance, 62, 64)]
        [TestCase(WeaponConstants.Longspear, 65, 67)]
        [TestCase(WeaponConstants.Morningstar, 68, 70)]
        [TestCase(WeaponConstants.Net, 71, 72)]
        [TestCase(WeaponConstants.HeavyPick, 73, 74)]
        [TestCase(WeaponConstants.LightPick, 75, 76)]
        [TestCase(WeaponConstants.Ranseur, 77, 78)]
        [TestCase(WeaponConstants.Sap, 79, 80)]
        [TestCase(WeaponConstants.Scythe, 81, 82)]
        [TestCase(WeaponConstants.Shuriken, 83, 84)]
        [TestCase(WeaponConstants.Sickle, 85, 86)]
        [TestCase(WeaponConstants.TwoBladedSword, 87, 89)]
        [TestCase(WeaponConstants.Trident, 90, 91)]
        [TestCase(WeaponConstants.DwarvenUrgrosh, 92, 94)]
        [TestCase(WeaponConstants.Warhammer, 95, 97)]
        [TestCase(WeaponConstants.Whip, 98, 100)]
        public void Percentile(string content, int lower, int upper)
        {
            base.AssertPercentile(content, lower, upper);
        }

        [TestCase(WeaponConstants.HeavyRepeatingCrossbow, 19)]
        public void Percentile(string content, int roll)
        {
            base.AssertPercentile(content, roll);
        }
    }
}