using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Levels
{
    [TestFixture]
    public class Level12ItemsTests : TypeAndAmountPercentileTests
    {
        protected override string tableName => TableNameConstants.Percentiles.LevelXItems(12);

        [Test]
        public override void ReplacementStringsAreValid()
        {
            AssertReplacementStringsAreValid();
        }

        [TestCase(PowerConstants.Mundane, AmountConstants.Range1d8, 1, 27)]
        [TestCase(PowerConstants.Minor, AmountConstants.Range1d6, 28, 82)]
        [TestCase(PowerConstants.Medium, AmountConstants.Range1, 83, 97)]
        [TestCase(PowerConstants.Major, AmountConstants.Range1, 98, 100)]
        public void Level12ItemsPercentile(string type, string amount, int lower, int upper)
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