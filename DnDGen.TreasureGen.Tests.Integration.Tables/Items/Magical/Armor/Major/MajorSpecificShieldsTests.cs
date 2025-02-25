using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Armor.Major
{
    [TestFixture]
    public class MajorSpecificShieldsTests : TypeAndAmountPercentileTests
    {
        protected override string tableName => TableNameConstants.Percentiles.POWERSpecificITEMTYPEs(PowerConstants.Major, AttributeConstants.Shield);

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

        [TestCase(ArmorConstants.CastersShield, 1, 1, 20)]
        [TestCase(ArmorConstants.SpinedShield, 1, 21, 40)]
        [TestCase(ArmorConstants.LionsShield, 2, 41, 60)]
        [TestCase(ArmorConstants.WingedShield, 3, 61, 90)]
        [TestCase(ArmorConstants.AbsorbingShield, 1, 91, 100)]
        public void MajorSpecificShieldsPercentile(string type, int amount, int lower, int upper)
        {
            AssertTypeAndAmountPercentile(type, amount, lower, upper);
        }
    }
}