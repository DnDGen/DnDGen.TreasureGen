using DnDGen.TreasureGen.Coins;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Coins
{
    [TestFixture]
    public class Level8CoinsTests : TypeAndAmountPercentileTests
    {
        protected override string tableName => TableNameConstants.Percentiles.LevelXCoins(8);

        [Test]
        public override void ReplacementStringsAreValid()
        {
            AssertReplacementStringsAreValid();
        }

        [TestCase("", AmountConstants.Range0, 1, 10)]
        [TestCase(CoinConstants.Copper, AmountConstants.Range1d12x10000, 11, 15)]
        [TestCase(CoinConstants.Silver, AmountConstants.Range2d6x1000, 16, 29)]
        [TestCase(CoinConstants.Gold, AmountConstants.Range2d8x100, 30, 87)]
        [TestCase(CoinConstants.Platinum, AmountConstants.Range3d6x10, 88, 100)]
        public void Level8CoinsPercentile(string type, string amount, int lower, int upper)
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