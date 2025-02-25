using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Scrolls
{
    [TestFixture]
    public class MajorSpellLevelsTests : PercentileTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Percentiles.POWERSpellLevels(PowerConstants.Major); }
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

        [TestCase("4", 1, 5)]
        [TestCase("5", 6, 50)]
        [TestCase("6", 51, 70)]
        [TestCase("7", 71, 85)]
        [TestCase("8", 86, 95)]
        [TestCase("9", 96, 100)]
        public void Percentile(string content, int lower, int upper)
        {
            base.AssertPercentile(content, lower, upper);
        }
    }
}