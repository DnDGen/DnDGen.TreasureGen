using NUnit.Framework;
using DnDGen.TreasureGen.Tables;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.WondrousItems
{
    [TestFixture]
    public class HornOfValhallaTypesTests : PercentileTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Percentiles.HornOfValhallaTypes; }
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

        [TestCase("Silver", 1, 40)]
        [TestCase("Brass", 41, 75)]
        [TestCase("Bronze", 76, 90)]
        [TestCase("Iron", 91, 100)]
        public void Percentile(string content, int lower, int upper)
        {
            base.AssertPercentile(content, lower, upper);
        }
    }
}