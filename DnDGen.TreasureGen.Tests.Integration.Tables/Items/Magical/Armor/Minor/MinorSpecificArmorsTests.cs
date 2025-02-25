using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Armor.Minor
{
    [TestFixture]
    public class MinorSpecificArmorsTests : TypeAndAmountPercentileTests
    {
        protected override string tableName => TableNameConstants.Percentiles.POWERSpecificITEMTYPEs(PowerConstants.Minor, ItemTypeConstants.Armor);

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

        [TestCase(ArmorConstants.ChainShirt, 0, 1, 50)]
        [TestCase(ArmorConstants.FullPlate, 0, 51, 80)]
        [TestCase(ArmorConstants.ElvenChain, 0, 81, 100)]
        public void MinorSpecificArmorsPercentile(string type, int amount, int lower, int upper)
        {
            AssertTypeAndAmountPercentile(type, amount, lower, upper);
        }
    }
}