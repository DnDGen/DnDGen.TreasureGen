using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Armor.Major
{
    [TestFixture]
    public class MajorSpecificArmorsTests : TypeAndAmountPercentileTests
    {
        protected override string tableName => TableNameConstants.Percentiles.POWERSpecificITEMTYPEs(PowerConstants.Major, ItemTypeConstants.Armor);

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

        [TestCase(ArmorConstants.Breastplate, 0, 1, 10)]
        [TestCase(ArmorConstants.DwarvenPlate, 0, 11, 20)]
        [TestCase(ArmorConstants.BandedMailOfLuck, 3, 21, 32)]
        [TestCase(ArmorConstants.CelestialArmor, 3, 33, 50)]
        [TestCase(ArmorConstants.PlateArmorOfTheDeep, 1, 51, 60)]
        [TestCase(ArmorConstants.BreastplateOfCommand, 2, 61, 75)]
        [TestCase(ArmorConstants.FullPlateOfSpeed, 1, 76, 90)]
        [TestCase(ArmorConstants.DemonArmor, 4, 91, 100)]
        public void MajorSpecificArmorsPercentilePercentile(string type, int amount, int lower, int upper)
        {
            AssertTypeAndAmountPercentile(type, amount, lower, upper);
        }
    }
}