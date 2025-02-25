using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Staves
{
    [TestFixture]
    public class MajorStaffsTests : TypeAndAmountPercentileTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Percentiles.POWERITEMTYPEs(PowerConstants.Major, ItemTypeConstants.Staff); }
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

        [TestCase(1, 3, StaffConstants.Charming, 0)]
        [TestCase(4, 9, StaffConstants.Fire, 0)]
        [TestCase(10, 11, StaffConstants.SwarmingInsects, 0)]
        [TestCase(12, 17, StaffConstants.Healing, 0)]
        [TestCase(18, 19, StaffConstants.SizeAlteration, 0)]
        [TestCase(20, 24, StaffConstants.Illumination, 0)]
        [TestCase(25, 31, StaffConstants.Frost, 0)]
        [TestCase(32, 38, StaffConstants.Defense, 0)]
        [TestCase(39, 41, StaffConstants.Abjuration, 0)]
        [TestCase(44, 48, StaffConstants.Conjuration, 0)]
        [TestCase(49, 53, StaffConstants.Enchantment, 0)]
        [TestCase(54, 58, StaffConstants.Evocation, 0)]
        [TestCase(59, 63, StaffConstants.Illusion, 0)]
        [TestCase(64, 68, StaffConstants.Necromancy, 0)]
        [TestCase(69, 73, StaffConstants.Transmutation, 0)]
        [TestCase(74, 77, StaffConstants.Divination, 0)]
        [TestCase(78, 82, StaffConstants.EarthAndStone, 0)]
        [TestCase(83, 87, StaffConstants.Woodlands, 2)]
        [TestCase(88, 92, StaffConstants.Life, 0)]
        [TestCase(93, 97, StaffConstants.Passage, 0)]
        [TestCase(98, 100, StaffConstants.Power, 2)]
        public void TypeAndAmountPercentile(int lower, int upper, string type, int amount)
        {
            base.AssertTypeAndAmountPercentile(type, amount, lower, upper);
        }
    }
}