using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Armor.Medium
{
    [TestFixture]
    public class MediumSpecificArmorsTests : TypeAndAmountPercentileTests
    {
        protected override string tableName => TableNameConstants.Percentiles.POWERSpecificITEMTYPEs(PowerConstants.Medium, ItemTypeConstants.Armor);

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

        [TestCase(ArmorConstants.ChainShirt, 0, 1, 25)]
        [TestCase(ArmorConstants.FullPlate, 0, 26, 45)]
        [TestCase(ArmorConstants.ElvenChain, 0, 46, 57)]
        [TestCase(ArmorConstants.RhinoHide, 2, 58, 67)]
        [TestCase(ArmorConstants.Breastplate, 0, 68, 82)]
        [TestCase(ArmorConstants.DwarvenPlate, 0, 83, 97)]
        [TestCase(ArmorConstants.BandedMailOfLuck, 3, 98, 100)]
        public void MediumSpecificArmorsPercentile(string type, int amount, int lower, int upper)
        {
            AssertTypeAndAmountPercentile(type, amount, lower, upper);
        }
    }
}