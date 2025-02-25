using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Levels
{
    [TestFixture]
    public class Level15ItemsTests : TypeAndAmountPercentileTests
    {
        protected override string tableName => TableNameConstants.Percentiles.LevelXItems(15);

        [Test]
        public override void ReplacementStringsAreValid()
        {
            AssertReplacementStringsAreValid();
        }

        [TestCase(PowerConstants.Mundane, AmountConstants.Range1d12, 1, 11)]
        [TestCase(PowerConstants.Minor, AmountConstants.Range1d10, 12, 46)]
        [TestCase(PowerConstants.Medium, AmountConstants.Range1, 47, 90)]
        [TestCase(PowerConstants.Major, AmountConstants.Range1, 91, 100)]
        public void Level15ItemsPercentile(string type, string amount, int lower, int upper)
        {
            AssertTypeAndAmountPercentile(type, amount, lower, upper);
        }

        [Test]
        public override void TableIsComplete()
        {
            AssertTableIsComplete();
        }
    }
}