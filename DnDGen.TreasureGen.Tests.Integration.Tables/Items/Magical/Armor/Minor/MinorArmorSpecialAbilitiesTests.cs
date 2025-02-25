using DnDGen.TreasureGen.Generators.Items.Magical;
using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Armor.Minor
{
    [TestFixture]
    public class MinorArmorSpecialAbilitiesTests : PercentileTests
    {
        protected override string tableName => TableNameConstants.Percentiles.POWERATTRIBUTESpecialAbilities(PowerConstants.Minor, ItemTypeConstants.Armor);
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

        [TestCase(SpecialAbilityConstants.Glamered, 1, 25)]
        [TestCase(SpecialAbilityConstants.LightFortification, 26, 32)]
        [TestCase(SpecialAbilityConstants.Slick, 33, 52)]
        [TestCase(SpecialAbilityConstants.Shadow, 53, 72)]
        [TestCase(SpecialAbilityConstants.SilentMoves, 73, 92)]
        [TestCase(SpecialAbilityConstants.SpellResistance13, 93, 96)]
        [TestCase(SpecialAbilityConstants.ImprovedSlick, 97, 97)]
        [TestCase(SpecialAbilityConstants.ImprovedShadow, 98, 98)]
        [TestCase(SpecialAbilityConstants.ImprovedSilentMoves, 99, 99)]
        [TestCase(SpecialAbilitiesGenerator.BonusSpecialAbility, 100, 100)]
        public void MinorArmorSpecialAbilitiesPercentile(string content, int lower, int upper)
        {
            AssertPercentile(content, lower, upper);
        }
    }
}