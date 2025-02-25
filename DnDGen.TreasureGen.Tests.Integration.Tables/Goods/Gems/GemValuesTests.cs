using DnDGen.TreasureGen.Goods;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Goods.Gems
{
    [TestFixture]
    public class GemValuesTests : TypeAndAmountPercentileTests
    {
        protected override string tableName => TableNameConstants.Percentiles.GOODTYPEValues(GoodsConstants.Gem);

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

        [TestCase("4d4", AmountConstants.Range4d4, 1, 25)]
        [TestCase("2d4*10", AmountConstants.Range2d4x10, 26, 50)]
        [TestCase("4d4*10", AmountConstants.Range4d4x10, 51, 70)]
        [TestCase("2d4*100", AmountConstants.Range2d4x100, 71, 90)]
        [TestCase("4d4*100", AmountConstants.Range4d4x100, 91, 99)]
        [TestCase("2d4*1000", AmountConstants.Range2d4x1000, 100, 100)]
        public void GemValuePercentile(string type, string value, int lower, int upper)
        {
            AssertTypeAndAmountPercentile(type, value, lower, upper);
        }
    }
}