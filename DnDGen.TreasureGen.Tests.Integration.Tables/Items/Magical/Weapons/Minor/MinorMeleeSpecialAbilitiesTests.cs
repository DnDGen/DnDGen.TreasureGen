using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Weapons.Minor
{
    [TestFixture]
    public class MinorMeleeSpecialAbilitiesTests : PercentileTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Percentiles.POWERATTRIBUTESpecialAbilities(PowerConstants.Minor, AttributeConstants.Melee); }
        }

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

        [TestCase(SpecialAbilityConstants.DESIGNATEDFOEbane, 1, 10)]
        [TestCase(SpecialAbilityConstants.Defending, 11, 17)]
        [TestCase(SpecialAbilityConstants.Flaming, 18, 27)]
        [TestCase(SpecialAbilityConstants.Frost, 28, 37)]
        [TestCase(SpecialAbilityConstants.Shock, 38, 47)]
        [TestCase(SpecialAbilityConstants.GhostTouchWeapon, 48, 56)]
        [TestCase(SpecialAbilityConstants.Keen, 57, 67)]
        [TestCase(SpecialAbilityConstants.KiFocus, 68, 71)]
        [TestCase(SpecialAbilityConstants.Merciful, 72, 75)]
        [TestCase(SpecialAbilityConstants.MightyCleaving, 76, 82)]
        [TestCase(SpecialAbilityConstants.SpellStoring, 83, 87)]
        [TestCase(SpecialAbilityConstants.Throwing, 88, 91)]
        [TestCase(SpecialAbilityConstants.Thundering, 92, 95)]
        [TestCase(SpecialAbilityConstants.Vicious, 96, 99)]
        public void Percentile(string content, int lower, int upper)
        {
            base.AssertPercentile(content, lower, upper);
        }

        [TestCase("BonusSpecialAbility", 100)]
        public void Percentile(string content, int roll)
        {
            base.AssertPercentile(content, roll);
        }
    }
}