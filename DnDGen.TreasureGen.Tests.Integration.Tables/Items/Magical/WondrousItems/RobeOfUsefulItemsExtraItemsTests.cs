using NUnit.Framework;
using DnDGen.TreasureGen.Tables;
using DnDGen.TreasureGen.Items;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.WondrousItems
{
    [TestFixture]
    public class RobeOfUsefulItemsExtraItemsTests : PercentileTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Percentiles.RobeOfUsefulItemsExtraItems; }
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

        [TestCase("Bag of 100 gold pieces", 1, 8)]
        [TestCase("Silver coffer (6 in, by 6 in, by 1 ft.) (500gp value)", 9, 15)]
        [TestCase("Iron door (up to 10 ft. wide and 10 ft. high and barred on one side - must be placed upright, attaches and hinges itself)", 16, 22)]
        [TestCase("10 gems (100gp value each)", 23, 30)]
        [TestCase("Wooden ladder (24 ft. long)", 31, 44)]
        [TestCase("Mule (with saddle bags)", 45, 51)]
        [TestCase("Open pit (10 ft. by 10 ft. by 10 ft.)", 52, 59)]
        [TestCase("Potion of Cure Serious Wounds", 60, 68)]
        [TestCase("Rowboat (12 ft. long)", 69, 75)]
        [TestCase(ItemTypeConstants.Scroll, 76, 83)]
        [TestCase("Pair of war dogs (treat as riding dogs)", 84, 90)]
        [TestCase("Window (2 ft. by 4 ft., up to 2 ft. deep)", 91, 96)]
        [TestCase("Portable ram", 97, 100)]
        public void Percentile(string content, int lower, int upper)
        {
            base.AssertPercentile(content, lower, upper);
        }
    }
}