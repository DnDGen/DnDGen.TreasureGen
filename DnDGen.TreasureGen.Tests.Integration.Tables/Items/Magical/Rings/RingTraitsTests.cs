using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Rings
{
    [TestFixture]
    public class RingTraitsTests : PercentileTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Percentiles.ITEMTYPETraits(ItemTypeConstants.Ring); }
        }

        [Test]
        public override void ReplacementStringsAreValid()
        {
            AssertReplacementStringsAreValid();
        }

        [TestCase(TraitConstants.Markings, 1, 30)]
        [TestCase("", 31, 100)]
        public void RingTraitsPercentile(string content, int lower, int upper)
        {
            AssertPercentile(content, lower, upper);
        }

        [Test]
        public override void TableIsComplete()
        {
            AssertTableIsComplete();
        }
    }
}