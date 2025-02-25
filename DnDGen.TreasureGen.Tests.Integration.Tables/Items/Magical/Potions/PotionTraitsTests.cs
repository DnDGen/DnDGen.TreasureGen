using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Potions
{
    [TestFixture]
    public class PotionTraitsTests : PercentileTests
    {
        protected override string tableName => TableNameConstants.Percentiles.ITEMTYPETraits(ItemTypeConstants.Potion);

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

        [TestCase("", 1, 100)]
        public void PotionTraitPercentile(string content, int lower, int upper)
        {
            AssertPercentile(content, lower, upper);
        }
    }
}