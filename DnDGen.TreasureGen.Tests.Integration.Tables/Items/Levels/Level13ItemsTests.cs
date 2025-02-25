using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Levels
{
    [TestFixture]
    public class Level13ItemsTests : TypeAndAmountPercentileTests
    {
        protected override string tableName => TableNameConstants.Percentiles.LevelXItems(13);

        [Test]
        public override void ReplacementStringsAreValid()
        {
            AssertReplacementStringsAreValid();
        }

        [TestCase(PowerConstants.Mundane, AmountConstants.Range1d8, 1, 19)]
        [TestCase(PowerConstants.Minor, AmountConstants.Range1d6, 20, 73)]
        [TestCase(PowerConstants.Medium, AmountConstants.Range1, 74, 95)]
        [TestCase(PowerConstants.Major, AmountConstants.Range1, 96, 100)]
        public void Level13ItemsPercentile(string type, string amount, int lower, int upper)
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