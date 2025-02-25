using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Wands
{
    [TestFixture]
    public class WandTraitsTests : PercentileTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Percentiles.ITEMTYPETraits(ItemTypeConstants.Wand); }
        }

        [Test]
        public override void ReplacementStringsAreValid()
        {
            AssertReplacementStringsAreValid();
        }

        [TestCase(TraitConstants.Markings, 1, 30)]
        [TestCase("", 31, 100)]
        public void Percentile(string content, int lower, int upper)
        {
            base.AssertPercentile(content, lower, upper);
        }

        [Test]
        public override void TableIsComplete()
        {
            AssertTableIsComplete();
        }
    }
}