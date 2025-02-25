using DnDGen.TreasureGen.Coins;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Coins
{
    [TestFixture]
    public class Level6CoinsTests : TypeAndAmountPercentileTests
    {
        protected override string tableName => TableNameConstants.Percentiles.LevelXCoins(6);

        [Test]
        public override void ReplacementStringsAreValid()
        {
            AssertReplacementStringsAreValid();
        }

        [TestCase("", AmountConstants.Range0, 1, 10)]
        [TestCase(CoinConstants.Copper, AmountConstants.Range1d6x10000, 11, 18)]
        [TestCase(CoinConstants.Silver, AmountConstants.Range1d8x1000, 19, 37)]
        [TestCase(CoinConstants.Gold, AmountConstants.Range1d10x100, 38, 95)]
        [TestCase(CoinConstants.Platinum, AmountConstants.Range1d12x10, 96, 100)]
        public void Level6CoinsPercentile(string type, string amount, int lower, int upper)
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