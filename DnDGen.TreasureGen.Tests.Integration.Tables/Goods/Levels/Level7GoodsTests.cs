using DnDGen.TreasureGen.Goods;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Goods.Levels
{
    [TestFixture]
    public class Level7GoodsTests : TypeAndAmountPercentileTests
    {
        protected override string tableName => TableNameConstants.Percentiles.LevelXGoods(7);

        [Test]
        public override void ReplacementStringsAreValid()
        {
            AssertReplacementStringsAreValid();
        }

        [TestCase("", AmountConstants.Range0, 1, 48)]
        [TestCase(GoodsConstants.Gem, AmountConstants.Range1d4, 49, 88)]
        [TestCase(GoodsConstants.Art, AmountConstants.Range1d4, 89, 100)]
        public void Level7GoodsPercentile(string type, string amount, int lower, int upper)
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