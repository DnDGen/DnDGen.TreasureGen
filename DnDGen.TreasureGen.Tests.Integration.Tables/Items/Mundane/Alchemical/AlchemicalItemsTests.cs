using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Mundane.Alchemical
{
    [TestFixture]
    public class AlchemicalItemsTests : TypeAndAmountPercentileTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Percentiles.AlchemicalItems; }
        }

        [Test]
        public override void ReplacementStringsAreValid()
        {
            AssertReplacementStringsAreValid();
        }

        [Test]
        public override void TableIsComplete()
        {
            AssertTableIsComplete();
        }

        [TestCase("Alchemist's fire", "1d4", 1, 12)]
        [TestCase("Acid", "2d4", 13, 24)]
        [TestCase("Smokestick", "1d4", 25, 36)]
        [TestCase("Holy water", "1d4", 37, 48)]
        [TestCase("Antitoxin", "1d4", 49, 62)]
        [TestCase("Everburning torch", "1", 63, 74)]
        [TestCase("Tanglefoot bag", "1d4", 75, 88)]
        [TestCase("Thunderstone", "1d4", 89, 100)]
        public void TypeAndAmountPercentile(string type, string amount, int lower, int upper)
        {
            AssertTypeAndAmountPercentile(type, amount, lower, upper);
        }
    }
}