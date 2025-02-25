using DnDGen.TreasureGen.Coins;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Coins
{
    [TestFixture]
    public class Level9CoinsTests : TypeAndAmountPercentileTests
    {
        protected override string tableName => TableNameConstants.Percentiles.LevelXCoins(9);

        [Test]
        public override void ReplacementStringsAreValid()
        {
            AssertReplacementStringsAreValid();
        }

        [TestCase("", AmountConstants.Range0, 1, 10)]
        [TestCase(CoinConstants.Copper, AmountConstants.Range2d6x10000, 11, 15)]
        [TestCase(CoinConstants.Silver, AmountConstants.Range2d8x1000, 16, 29)]
        [TestCase(CoinConstants.Gold, AmountConstants.Range5d4x100, 30, 85)]
        [TestCase(CoinConstants.Platinum, AmountConstants.Range2d12x10, 86, 100)]
        public void Level9CoinsPercentile(string type, string amount, int lower, int upper)
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