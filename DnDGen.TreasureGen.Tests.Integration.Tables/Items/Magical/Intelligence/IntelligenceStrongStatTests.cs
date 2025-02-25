using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Intelligence
{
    [TestFixture]
    public class IntelligenceStrongStatTests : PercentileTests
    {
        protected override string tableName => TableNameConstants.Percentiles.IntelligenceStrongStats;

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

        [TestCase("12", 1, 34)]
        [TestCase("13", 35, 59)]
        [TestCase("14", 60, 79)]
        [TestCase("15", 80, 91)]
        [TestCase("16", 92, 97)]
        [TestCase("17", 98, 98)]
        [TestCase("18", 99, 99)]
        [TestCase("19", 100, 100)]
        public void IntelligenceStringStatPercentile(string content, int lower, int upper)
        {
            AssertPercentile(content, lower, upper);
        }
    }
}