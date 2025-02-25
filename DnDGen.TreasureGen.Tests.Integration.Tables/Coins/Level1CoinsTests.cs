using DnDGen.TreasureGen.Coins;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Coins
{
    [TestFixture]
    public class Level1CoinsTests : TypeAndAmountPercentileTests
    {
        protected override string tableName => TableNameConstants.Percentiles.LevelXCoins(1);

        [Test]
        public override void ReplacementStringsAreValid()
        {
            AssertReplacementStringsAreValid();
        }

        [TestCase("", AmountConstants.Range0, 1, 14)]
        [TestCase(CoinConstants.Copper, AmountConstants.Range1d6x1000, 15, 29)]
        [TestCase(CoinConstants.Silver, AmountConstants.Range1d8x100, 30, 52)]
        [TestCase(CoinConstants.Gold, AmountConstants.Range2d8x10, 53, 95)]
        [TestCase(CoinConstants.Platinum, AmountConstants.Range1d4x10, 96, 100)]
        public void Level1CoinsPercentile(string type, string amount, int lower, int upper)
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