using DnDGen.TreasureGen.Coins;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Coins
{
    [TestFixture]
    public class Level13CoinsTests : TypeAndAmountPercentileTests
    {
        protected override string tableName => TableNameConstants.Percentiles.LevelXCoins(13);

        [Test]
        public override void ReplacementStringsAreValid()
        {
            AssertReplacementStringsAreValid();
        }

        [TestCase("", AmountConstants.Range0, 1, 8)]
        [TestCase(CoinConstants.Gold, AmountConstants.Range1d4x1000, 9, 75)]
        [TestCase(CoinConstants.Platinum, AmountConstants.Range1d10x100, 76, 100)]
        public void Level13CoinsPercentile(string type, string amount, int lower, int upper)
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