using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Armor.Specific
{
    [TestFixture]
    public class ShieldTypesTests : PercentileTests
    {
        protected override string tableName => TableNameConstants.Percentiles.ARMORTYPETypes(AttributeConstants.Shield);

        [Test]
        public override void ReplacementStringsAreValid()
        {
            AssertReplacementStringsAreValid();
        }

        [TestCase(ArmorConstants.Buckler, 1, 10)]
        [TestCase(ArmorConstants.LightWoodenShield, 11, 15)]
        [TestCase(ArmorConstants.LightSteelShield, 16, 20)]
        [TestCase(ArmorConstants.HeavyWoodenShield, 21, 30)]
        [TestCase(ArmorConstants.HeavySteelShield, 31, 95)]
        [TestCase(ArmorConstants.TowerShield, 96, 100)]
        public void ShieldTypesPercentile(string content, int lower, int upper)
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