using DnDGen.TreasureGen.Coins;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Coins
{
    [TestFixture]
    public class Level2CoinsTests : TypeAndAmountPercentileTests
    {
        protected override string tableName => TableNameConstants.Percentiles.LevelXCoins(2);

        [Test]
        public override void ReplacementStringsAreValid()
        {
            AssertReplacementStringsAreValid();
        }

        [TestCase("", AmountConstants.Range0, 1, 13)]
        [TestCase(CoinConstants.Copper, AmountConstants.Range1d10x1000, 14, 23)]
        [TestCase(CoinConstants.Silver, AmountConstants.Range2d10x100, 24, 43)]
        [TestCase(CoinConstants.Gold, AmountConstants.Range4d10x10, 44, 95)]
        [TestCase(CoinConstants.Platinum, AmountConstants.Range2d8x10, 96, 100)]
        public void Level2CoinsPercentile(string type, string amount, int lower, int upper)
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