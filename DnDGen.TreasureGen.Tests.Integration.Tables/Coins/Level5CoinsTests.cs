using DnDGen.TreasureGen.Coins;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Coins
{
    [TestFixture]
    public class Level5CoinsTests : TypeAndAmountPercentileTests
    {
        protected override string tableName => TableNameConstants.Percentiles.LevelXCoins(5);

        [Test]
        public override void ReplacementStringsAreValid()
        {
            AssertReplacementStringsAreValid();
        }

        [TestCase("", AmountConstants.Range0, 1, 10)]
        [TestCase(CoinConstants.Copper, AmountConstants.Range1d4x10000, 11, 19)]
        [TestCase(CoinConstants.Silver, AmountConstants.Range1d6x1000, 20, 38)]
        [TestCase(CoinConstants.Gold, AmountConstants.Range1d8x100, 39, 95)]
        [TestCase(CoinConstants.Platinum, AmountConstants.Range1d10x10, 96, 100)]
        public void Level5CoinsPercentile(string type, string amount, int lower, int upper)
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