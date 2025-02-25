using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical
{
    [TestFixture]
    public class MajorItemsTests : PercentileTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Percentiles.POWERItems(PowerConstants.Major); }
        }

        [Test]
        public override void ReplacementStringsAreValid()
        {
            AssertReplacementStringsAreValid();
        }

        [TestCase(ItemTypeConstants.Armor, 1, 10)]
        [TestCase(ItemTypeConstants.Weapon, 11, 20)]
        [TestCase(ItemTypeConstants.Potion, 21, 25)]
        [TestCase(ItemTypeConstants.Ring, 26, 35)]
        [TestCase(ItemTypeConstants.Rod, 36, 45)]
        [TestCase(ItemTypeConstants.Scroll, 46, 55)]
        [TestCase(ItemTypeConstants.Staff, 56, 75)]
        [TestCase(ItemTypeConstants.Wand, 76, 80)]
        [TestCase(ItemTypeConstants.WondrousItem, 81, 100)]
        public void MajorItemsPercentile(string content, int lower, int upper)
        {
            AssertPercentile(content, lower, upper);
        }

        [Test]
        public override void TableIsComplete()
        {
            AssertTableIsComplete();
        }
    }
}