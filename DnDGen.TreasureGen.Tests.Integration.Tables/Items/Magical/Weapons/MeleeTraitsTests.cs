using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Weapons
{
    [TestFixture]
    public class MeleeTraitsTests : PercentileTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Percentiles.ITEMTYPETraits(AttributeConstants.Melee); }
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

        [TestCase(TraitConstants.ShedsLight, 1, 30)]
        [TestCase(TraitConstants.Markings, 31, 45)]
        [TestCase("", 46, 100)]
        public void Percentile(string content, int lower, int upper)
        {
            base.AssertPercentile(content, lower, upper);
        }
    }
}