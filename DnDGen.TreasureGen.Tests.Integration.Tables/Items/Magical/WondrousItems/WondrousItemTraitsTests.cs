using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.WondrousItems
{
    [TestFixture]
    public class WondrousItemTraitsTests : PercentileTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Percentiles.ITEMTYPETraits(ItemTypeConstants.WondrousItem); }
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

        [TestCase(TraitConstants.Markings, 1, 30)]
        [TestCase("", 31, 100)]
        public void Percentile(string content, int lower, int upper)
        {
            AssertPercentile(content, lower, upper);
        }
    }
}