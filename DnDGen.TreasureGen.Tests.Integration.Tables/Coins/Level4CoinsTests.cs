using DnDGen.TreasureGen.Coins;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Coins
{
    [TestFixture]
    public class Level4CoinsTests : TypeAndAmountPercentileTests
    {
        protected override string tableName => TableNameConstants.Percentiles.LevelXCoins(4);

        [Test]
        public override void ReplacementStringsAreValid()
        {
            AssertReplacementStringsAreValid();
        }

        [TestCase("", AmountConstants.Range0, 1, 11)]
        [TestCase(CoinConstants.Copper, AmountConstants.Range3d10x1000, 12, 21)]
        [TestCase(CoinConstants.Silver, AmountConstants.Range4d12x1000, 22, 41)]
        [TestCase(CoinConstants.Gold, AmountConstants.Range1d6x100, 42, 95)]
        [TestCase(CoinConstants.Platinum, AmountConstants.Range1d8x10, 96, 100)]
        public void Level4CoinsPercentile(string type, string amount, int lower, int upper)
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