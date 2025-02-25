using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Armor
{
    [TestFixture]
    public class ArmorTraitsTests : PercentileTests
    {
        protected override string tableName => TableNameConstants.Percentiles.ITEMTYPETraits(ItemTypeConstants.Armor);

        [Test]
        public override void ReplacementStringsAreValid()
        {
            AssertReplacementStringsAreValid();
        }

        [TestCase(TraitConstants.Markings, 1, 30)]
        [TestCase("", 31, 100)]
        public void ArmorTraitsPercentile(string content, int lower, int upper)
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