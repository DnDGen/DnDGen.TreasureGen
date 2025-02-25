using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Armor.Major
{
    [TestFixture]
    public class MajorShieldSpecialAbilitiesTests : PercentileTests
    {
        protected override string tableName => TableNameConstants.Percentiles.POWERATTRIBUTESpecialAbilities(PowerConstants.Major, AttributeConstants.Shield);

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

        [TestCase(SpecialAbilityConstants.ArrowCatching, 1, 5)]
        [TestCase(SpecialAbilityConstants.Bashing, 6, 8)]
        [TestCase(SpecialAbilityConstants.Blinding, 9, 10)]
        [TestCase(SpecialAbilityConstants.LightFortification, 11, 15)]
        [TestCase(SpecialAbilityConstants.ArrowDeflection, 16, 20)]
        [TestCase(SpecialAbilityConstants.Animated, 21, 25)]
        [TestCase(SpecialAbilityConstants.AcidResistance, 26, 28)]
        [TestCase(SpecialAbilityConstants.ColdResistance, 29, 31)]
        [TestCase(SpecialAbilityConstants.ElectricityResistance, 32, 34)]
        [TestCase(SpecialAbilityConstants.FireResistance, 35, 37)]
        [TestCase(SpecialAbilityConstants.SonicResistance, 38, 40)]
        [TestCase(SpecialAbilityConstants.GhostTouchArmor, 41, 46)]
        [TestCase(SpecialAbilityConstants.ModerateFortification, 47, 56)]
        [TestCase(SpecialAbilityConstants.SpellResistance15, 57, 58)]
        [TestCase(SpecialAbilityConstants.Wild, 59, 59)]
        [TestCase(SpecialAbilityConstants.ImprovedAcidResistance, 60, 64)]
        [TestCase(SpecialAbilityConstants.ImprovedColdResistance, 65, 69)]
        [TestCase(SpecialAbilityConstants.ImprovedElectricityResistance, 70, 74)]
        [TestCase(SpecialAbilityConstants.ImprovedFireResistance, 75, 79)]
        [TestCase(SpecialAbilityConstants.ImprovedSonicResistance, 80, 84)]
        [TestCase(SpecialAbilityConstants.SpellResistance17, 85, 86)]
        [TestCase(SpecialAbilityConstants.UndeadControlling, 87, 87)]
        [TestCase(SpecialAbilityConstants.HeavyFortification, 88, 91)]
        [TestCase(SpecialAbilityConstants.Reflecting, 92, 93)]
        [TestCase(SpecialAbilityConstants.SpellResistance19, 94, 94)]
        [TestCase(SpecialAbilityConstants.GreaterAcidResistance, 95, 95)]
        [TestCase(SpecialAbilityConstants.GreaterColdResistance, 96, 96)]
        [TestCase(SpecialAbilityConstants.GreaterElectricityResistance, 97, 97)]
        [TestCase(SpecialAbilityConstants.GreaterFireResistance, 98, 98)]
        [TestCase(SpecialAbilityConstants.GreaterSonicResistance, 99, 99)]
        [TestCase("BonusSpecialAbility", 100, 100)]
        public void MajorShieldSpecialAbilitiesPercentile(string content, int lower, int upper)
        {
            AssertPercentile(content, lower, upper);
        }
    }
}