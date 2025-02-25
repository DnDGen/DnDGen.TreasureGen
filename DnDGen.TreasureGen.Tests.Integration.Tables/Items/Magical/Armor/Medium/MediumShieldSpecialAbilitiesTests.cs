using DnDGen.TreasureGen.Generators.Items.Magical;
using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Armor.Medium
{
    [TestFixture]
    public class MediumShieldSpecialAbilitiesTests : PercentileTests
    {
        protected override string tableName => TableNameConstants.Percentiles.POWERATTRIBUTESpecialAbilities(PowerConstants.Medium, AttributeConstants.Shield);

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

        [TestCase(SpecialAbilityConstants.ArrowCatching, 1, 10)]
        [TestCase(SpecialAbilityConstants.Bashing, 11, 20)]
        [TestCase(SpecialAbilityConstants.Blinding, 21, 25)]
        [TestCase(SpecialAbilityConstants.LightFortification, 26, 40)]
        [TestCase(SpecialAbilityConstants.ArrowDeflection, 41, 50)]
        [TestCase(SpecialAbilityConstants.Animated, 51, 57)]
        [TestCase(SpecialAbilityConstants.SpellResistance13, 58, 59)]
        [TestCase(SpecialAbilityConstants.AcidResistance, 60, 63)]
        [TestCase(SpecialAbilityConstants.ColdResistance, 64, 67)]
        [TestCase(SpecialAbilityConstants.ElectricityResistance, 68, 71)]
        [TestCase(SpecialAbilityConstants.FireResistance, 72, 75)]
        [TestCase(SpecialAbilityConstants.SonicResistance, 76, 79)]
        [TestCase(SpecialAbilityConstants.GhostTouchArmor, 80, 85)]
        [TestCase(SpecialAbilityConstants.ModerateFortification, 86, 95)]
        [TestCase(SpecialAbilityConstants.SpellResistance15, 96, 98)]
        [TestCase(SpecialAbilityConstants.Wild, 99, 99)]
        [TestCase(SpecialAbilitiesGenerator.BonusSpecialAbility, 100, 100)]
        public void MediumShieldSpecialAbilitiesPercentile(string content, int lower, int upper)
        {
            AssertPercentile(content, lower, upper);
        }
    }
}