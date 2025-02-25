using DnDGen.TreasureGen.Generators.Items.Magical;
using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Armor.Minor
{
    [TestFixture]
    public class MinorShieldSpecialAbilitiesTests : PercentileTests
    {
        protected override string tableName => TableNameConstants.Percentiles.POWERATTRIBUTESpecialAbilities(PowerConstants.Minor, AttributeConstants.Shield);

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

        [TestCase(SpecialAbilityConstants.ArrowCatching, 1, 20)]
        [TestCase(SpecialAbilityConstants.Bashing, 21, 40)]
        [TestCase(SpecialAbilityConstants.Blinding, 41, 50)]
        [TestCase(SpecialAbilityConstants.LightFortification, 51, 75)]
        [TestCase(SpecialAbilityConstants.ArrowDeflection, 76, 92)]
        [TestCase(SpecialAbilityConstants.Animated, 93, 97)]
        [TestCase(SpecialAbilityConstants.SpellResistance13, 98, 99)]
        [TestCase(SpecialAbilitiesGenerator.BonusSpecialAbility, 100, 100)]
        public void MinorShieldSpecialAbilitiesPercentile(string content, int lower, int upper)
        {
            AssertPercentile(content, lower, upper);
        }
    }
}