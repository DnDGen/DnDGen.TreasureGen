using DnDGen.TreasureGen.Generators.Items.Magical;
using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Armor.Minor
{
    [TestFixture]
    public class MinorArmorsTests : TypeAndAmountPercentileTests
    {
        protected override string tableName => TableNameConstants.Percentiles.POWERITEMTYPEs(PowerConstants.Minor, ItemTypeConstants.Armor);

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

        [TestCase(AttributeConstants.Shield, 1, 1, 60)]
        [TestCase(ItemTypeConstants.Armor, 1, 61, 80)]
        [TestCase(AttributeConstants.Shield, 2, 81, 85)]
        [TestCase(ItemTypeConstants.Armor, 2, 86, 87)]
        [TestCase(ItemTypeConstants.Armor, MagicalArmorGenerator.SpecificBonus, 88, 89)]
        [TestCase(AttributeConstants.Shield, MagicalArmorGenerator.SpecificBonus, 90, 91)]
        [TestCase(MagicalArmorGenerator.SpecialAbility, 0, 92, 100)]
        public void MinorArmorsPercentile(string type, int amount, int lower, int upper)
        {
            AssertTypeAndAmountPercentile(type, amount, lower, upper);
        }
    }
}