using NUnit.Framework;
using DnDGen.TreasureGen.Tables;
using DnDGen.TreasureGen.Items;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Mundane
{
    [TestFixture]
    public class MundaneGearSizesTests : PercentileTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Percentiles.MundaneGearSizes; }
        }

        [Test]
        public override void ReplacementStringsAreValid()
        {
            AssertReplacementStringsAreValid();
        }

        [TestCase(TraitConstants.Sizes.Tiny, 1, 1)]
        [TestCase(TraitConstants.Sizes.Small, 2, 11)]
        [TestCase(TraitConstants.Sizes.Medium, 12, 87)]
        [TestCase(TraitConstants.Sizes.Large, 88, 97)]
        [TestCase(TraitConstants.Sizes.Huge, 98, 98)]
        [TestCase(TraitConstants.Sizes.Gargantuan, 99, 99)]
        [TestCase(TraitConstants.Sizes.Colossal, 100, 100)]
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