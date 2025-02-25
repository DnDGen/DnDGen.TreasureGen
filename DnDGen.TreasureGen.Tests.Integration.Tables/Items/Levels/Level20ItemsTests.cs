using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Levels
{
    [TestFixture]
    public class Level20ItemsTests : TypeAndAmountPercentileTests
    {
        protected override string tableName => TableNameConstants.Percentiles.LevelXItems(20);

        [Test]
        public override void ReplacementStringsAreValid()
        {
            AssertReplacementStringsAreValid();
        }

        [TestCase(PowerConstants.Minor, AmountConstants.Range2d4, 1, 25)]
        [TestCase(PowerConstants.Medium, AmountConstants.Range1d4, 26, 65)]
        [TestCase(PowerConstants.Major, AmountConstants.Range1d3, 66, 100)]
        public void Level20ItemsPercentile(string type, string amount, int lower, int upper)
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