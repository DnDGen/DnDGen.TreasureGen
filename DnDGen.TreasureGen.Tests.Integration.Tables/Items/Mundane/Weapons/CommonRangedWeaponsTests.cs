using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Mundane.Weapons
{
    [TestFixture]
    public class CommonRangedWeaponsTests : PercentileTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Percentiles.WEAPONTYPEWeapons("CommonRanged"); }
        }

        [Test]
        public override void ReplacementStringsAreValid()
        {
            AssertReplacementStringsAreValid();
        }

        [TestCase(WeaponConstants.Arrow, 1, 5)]
        [TestCase(WeaponConstants.CrossbowBolt, 6, 8)]
        [TestCase(WeaponConstants.SlingBullet, 9, 10)]
        [TestCase(WeaponConstants.ThrowingAxe, 11, 15)]
        [TestCase(WeaponConstants.HeavyCrossbow, 16, 25)]
        [TestCase(WeaponConstants.LightCrossbow, 26, 35)]
        [TestCase(WeaponConstants.Dart, 36, 39)]
        [TestCase(WeaponConstants.Javelin, 40, 41)]
        [TestCase(WeaponConstants.Shortbow, 42, 46)]
        [TestCase(WeaponConstants.CompositeShortbow_StrengthPlus0, 47, 51)]
        [TestCase(WeaponConstants.CompositeShortbow_StrengthPlus1, 52, 56)]
        [TestCase(WeaponConstants.CompositeShortbow_StrengthPlus2, 57, 61)]
        [TestCase(WeaponConstants.Sling, 62, 65)]
        [TestCase(WeaponConstants.Longbow, 66, 75)]
        [TestCase(WeaponConstants.CompositeLongbow_StrengthPlus0, 76, 80)]
        [TestCase(WeaponConstants.CompositeLongbow_StrengthPlus1, 81, 85)]
        [TestCase(WeaponConstants.CompositeLongbow_StrengthPlus2, 86, 90)]
        [TestCase(WeaponConstants.CompositeLongbow_StrengthPlus3, 91, 95)]
        [TestCase(WeaponConstants.CompositeLongbow_StrengthPlus4, 96, 100)]
        public void Percentile(string content, int lower, int upper)
        {
            base.AssertPercentile(content, lower, upper);
        }

        [Test]
        public override void TableIsComplete()
        {
            AssertTableIsComplete();
        }
    }
}