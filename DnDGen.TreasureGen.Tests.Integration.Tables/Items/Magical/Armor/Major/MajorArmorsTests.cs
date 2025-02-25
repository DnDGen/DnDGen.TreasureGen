using DnDGen.TreasureGen.Generators.Items.Magical;
using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Armor.Major
{
    [TestFixture]
    public class MajorArmorsTests : TypeAndAmountPercentileTests
    {
        protected override string tableName => TableNameConstants.Percentiles.POWERITEMTYPEs(PowerConstants.Major, ItemTypeConstants.Armor);

        [Test]
        public override void ReplacementStringsAreValid()
        {
            AssertReplacementStringsAreValid();
        }

        [TestCase(AttributeConstants.Shield, 3, 1, 8)]
        [TestCase(ItemTypeConstants.Armor, 3, 9, 16)]
        [TestCase(AttributeConstants.Shield, 4, 17, 27)]
        [TestCase(ItemTypeConstants.Armor, 4, 28, 38)]
        [TestCase(AttributeConstants.Shield, 5, 39, 49)]
        [TestCase(ItemTypeConstants.Armor, 5, 50, 57)]
        [TestCase(ItemTypeConstants.Armor, MagicalArmorGenerator.SpecificBonus, 58, 60)]
        [TestCase(AttributeConstants.Shield, MagicalArmorGenerator.SpecificBonus, 61, 63)]
        [TestCase(MagicalArmorGenerator.SpecialAbility, 0, 64, 100)]
        public void MajorArmorsPercentile(string type, int amount, int lower, int upper)
        {
            AssertTypeAndAmountPercentile(type, amount, lower, upper);
        }

        [Test]
        public override void TableIsComplete()
        {
            AssertTableIsComplete();
        }
    }
}