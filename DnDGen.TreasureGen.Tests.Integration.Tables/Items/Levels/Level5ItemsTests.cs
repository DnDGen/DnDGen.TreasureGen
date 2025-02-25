using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Levels
{
    [TestFixture]
    public class Level5ItemsTests : TypeAndAmountPercentileTests
    {
        protected override string tableName => TableNameConstants.Percentiles.LevelXItems(5);

        [Test]
        public override void ReplacementStringsAreValid()
        {
            AssertReplacementStringsAreValid();
        }

        [TestCase("", AmountConstants.Range0, 1, 57)]
        [TestCase(PowerConstants.Mundane, AmountConstants.Range1d4, 58, 67)]
        [TestCase(PowerConstants.Minor, AmountConstants.Range1d3, 68, 100)]
        public void Level5ItemsPercentile(string type, string amount, int lower, int upper)
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