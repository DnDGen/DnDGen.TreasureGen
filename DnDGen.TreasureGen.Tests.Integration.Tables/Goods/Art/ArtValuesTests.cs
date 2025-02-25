using DnDGen.TreasureGen.Goods;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Goods.Art
{
    [TestFixture]
    public class ArtValuesTests : TypeAndAmountPercentileTests
    {
        protected override string tableName => TableNameConstants.Percentiles.GOODTYPEValues(GoodsConstants.Art);

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

        [TestCase("1d10*10", AmountConstants.Range1d10x10, 1, 10)]
        [TestCase("3d6*10", AmountConstants.Range3d6x10, 11, 25)]
        [TestCase("1d6*100", AmountConstants.Range1d6x100, 26, 40)]
        [TestCase("1d10*100", AmountConstants.Range1d10x100, 41, 50)]
        [TestCase("2d6*100", AmountConstants.Range2d6x100, 51, 60)]
        [TestCase("3d6*100", AmountConstants.Range3d6x100, 61, 70)]
        [TestCase("4d6*100", AmountConstants.Range4d6x100, 71, 80)]
        [TestCase("5d6*100", AmountConstants.Range5d6x100, 81, 85)]
        [TestCase("1d4*1000", AmountConstants.Range1d4x1000, 86, 90)]
        [TestCase("1d6*1000", AmountConstants.Range1d6x1000, 91, 95)]
        [TestCase("2d4*1000", AmountConstants.Range2d4x1000, 96, 99)]
        [TestCase("2d6*1000", AmountConstants.Range2d6x1000, 100, 100)]
        public void ArtValuePercentile(string type, string value, int lower, int upper)
        {
            AssertTypeAndAmountPercentile(type, value, lower, upper);
        }
    }
}