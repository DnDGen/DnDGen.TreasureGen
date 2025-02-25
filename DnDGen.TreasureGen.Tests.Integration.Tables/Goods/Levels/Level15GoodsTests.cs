using DnDGen.TreasureGen.Goods;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Goods.Levels
{
    [TestFixture]
    public class Level15GoodsTests : TypeAndAmountPercentileTests
    {
        protected override string tableName => TableNameConstants.Percentiles.LevelXGoods(15);

        [Test]
        public override void ReplacementStringsAreValid()
        {
            AssertReplacementStringsAreValid();
        }

        [TestCase("", AmountConstants.Range0, 1, 9)]
        [TestCase(GoodsConstants.Gem, AmountConstants.Range2d10, 10, 65)]
        [TestCase(GoodsConstants.Art, AmountConstants.Range2d8, 66, 100)]
        public void Level15GoodsPercentile(string type, string amount, int lower, int upper)
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