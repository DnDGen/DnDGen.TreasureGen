using NUnit.Framework;
using DnDGen.TreasureGen.Tables;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Weapons
{
    [TestFixture]
    public class MagicalWeaponTypesTests : PercentileTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Percentiles.MagicalWeaponTypes; }
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

        [TestCase("CommonMelee", 1, 70)]
        [TestCase("Uncommon", 71, 80)]
        [TestCase("CommonRanged", 81, 100)]
        public void Percentile(string content, int lower, int upper)
        {
            base.AssertPercentile(content, lower, upper);
        }
    }
}