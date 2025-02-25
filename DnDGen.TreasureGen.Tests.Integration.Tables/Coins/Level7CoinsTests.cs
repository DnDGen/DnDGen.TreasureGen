using DnDGen.TreasureGen.Coins;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Coins
{
    [TestFixture]
    public class Level7CoinsTests : TypeAndAmountPercentileTests
    {
        protected override string tableName => TableNameConstants.Percentiles.LevelXCoins(7);

        [Test]
        public override void ReplacementStringsAreValid()
        {
            AssertReplacementStringsAreValid();
        }

        [TestCase("", AmountConstants.Range0, 1, 11)]
        [TestCase(CoinConstants.Copper, AmountConstants.Range1d10x10000, 12, 18)]
        [TestCase(CoinConstants.Silver, AmountConstants.Range1d12x1000, 19, 35)]
        [TestCase(CoinConstants.Gold, AmountConstants.Range2d6x100, 36, 93)]
        [TestCase(CoinConstants.Platinum, AmountConstants.Range3d4x10, 94, 100)]
        public void Level7CoinsPercentile(string type, string amount, int lower, int upper)
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