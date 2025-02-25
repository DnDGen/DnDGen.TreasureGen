using NUnit.Framework;
using DnDGen.TreasureGen.Tables;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.WondrousItems
{
    [TestFixture]
    public class RobeOfTheArchmagiColorsTests : PercentileTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Percentiles.RobeOfTheArchmagiColors; }
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

        [TestCase("White (Good)", 1, 45)]
        [TestCase("Gray (Neutral)", 46, 75)]
        [TestCase("Black (Evil)", 76, 100)]
        public void Percentile(string content, int lower, int upper)
        {
            base.AssertPercentile(content, lower, upper);
        }
    }
}