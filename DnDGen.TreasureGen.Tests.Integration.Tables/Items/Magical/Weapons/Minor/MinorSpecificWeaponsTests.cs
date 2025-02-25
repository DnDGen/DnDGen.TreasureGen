using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Weapons.Minor
{
    [TestFixture]
    public class MinorSpecificWeaponsTests : TypeAndAmountPercentileTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Percentiles.POWERSpecificITEMTYPEs(PowerConstants.Minor, ItemTypeConstants.Weapon); }
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

        [TestCase(WeaponConstants.SleepArrow, 1, 1, 15)]
        [TestCase(WeaponConstants.ScreamingBolt, 2, 16, 25)]
        [TestCase(WeaponConstants.Dagger_Silver, 0, 26, 45)]
        [TestCase(WeaponConstants.Longsword, 0, 46, 65)]
        [TestCase(WeaponConstants.JavelinOfLightning, 0, 66, 75)]
        [TestCase(WeaponConstants.SlayingArrow, 1, 76, 80)]
        [TestCase(WeaponConstants.Dagger_Adamantine, 0, 81, 90)]
        [TestCase(WeaponConstants.Battleaxe_Adamantine, 0, 91, 100)]
        public void TypeAndAmountPercentile(string type, int amount, int lower, int upper)
        {
            base.AssertTypeAndAmountPercentile(type, amount, lower, upper);
        }
    }
}