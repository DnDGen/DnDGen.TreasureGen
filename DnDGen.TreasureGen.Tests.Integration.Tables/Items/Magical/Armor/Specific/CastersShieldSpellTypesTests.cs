using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Armor.Specific
{
    [TestFixture]
    public class CastersShieldSpellTypesTests : PercentileTests
    {
        protected override string tableName => TableNameConstants.Percentiles.CastersShieldSpellTypes;

        [Test]
        public override void ReplacementStringsAreValid()
        {
            AssertReplacementStringsAreValid();
        }

        [TestCase("Divine", 1, 80)]
        [TestCase("Arcane", 81, 100)]
        public void CasterShieldSpellTypesPercentile(string content, int lower, int upper)
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