using DnDGen.TreasureGen.Coins;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Coins
{
    [TestFixture]
    public class Level19CoinsTests : TypeAndAmountPercentileTests
    {
        protected override string tableName => TableNameConstants.Percentiles.LevelXCoins(19);

        [Test]
        public override void ReplacementStringsAreValid()
        {
            AssertReplacementStringsAreValid();
        }

        [TestCase("", AmountConstants.Range0, 1, 2)]
        [TestCase(CoinConstants.Gold, AmountConstants.Range3d8x1000, 3, 65)]
        [TestCase(CoinConstants.Platinum, AmountConstants.Range3d10x100, 66, 100)]
        public void Level19CoinsPercentile(string type, string amount, int lower, int upper)
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