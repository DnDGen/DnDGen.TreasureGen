using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Armor.Minor
{
    [TestFixture]
    public class MinorSpecificShieldsTests : TypeAndAmountPercentileTests
    {
        protected override string tableName => TableNameConstants.Percentiles.POWERSpecificITEMTYPEs(PowerConstants.Minor, AttributeConstants.Shield);

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

        [TestCase(ArmorConstants.Buckler, 0, 1, 30)]
        [TestCase(ArmorConstants.HeavyWoodenShield, 0, 31, 80)]
        [TestCase(ArmorConstants.HeavySteelShield, 0, 81, 95)]
        [TestCase(ArmorConstants.CastersShield, 1, 96, 100)]
        public void MinorSpecificShieldsPercentile(string type, int amount, int lower, int upper)
        {
            AssertTypeAndAmountPercentile(type, amount, lower, upper);
        }
    }
}