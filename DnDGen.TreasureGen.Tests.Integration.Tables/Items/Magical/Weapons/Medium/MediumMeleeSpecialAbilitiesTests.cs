using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Weapons.Medium
{
    [TestFixture]
    public class MediumMeleeSpecialAbilitiesTests : PercentileTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Percentiles.POWERATTRIBUTESpecialAbilities(PowerConstants.Medium, AttributeConstants.Melee); }
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

        [TestCase(SpecialAbilityConstants.DESIGNATEDFOEbane, 1, 6)]
        [TestCase(SpecialAbilityConstants.Defending, 7, 12)]
        [TestCase(SpecialAbilityConstants.Flaming, 13, 19)]
        [TestCase(SpecialAbilityConstants.Frost, 20, 26)]
        [TestCase(SpecialAbilityConstants.Shock, 27, 33)]
        [TestCase(SpecialAbilityConstants.GhostTouchWeapon, 34, 38)]
        [TestCase(SpecialAbilityConstants.Keen, 39, 44)]
        [TestCase(SpecialAbilityConstants.KiFocus, 45, 48)]
        [TestCase(SpecialAbilityConstants.Merciful, 49, 50)]
        [TestCase(SpecialAbilityConstants.MightyCleaving, 51, 54)]
        [TestCase(SpecialAbilityConstants.SpellStoring, 55, 59)]
        [TestCase(SpecialAbilityConstants.Throwing, 60, 63)]
        [TestCase(SpecialAbilityConstants.Thundering, 64, 65)]
        [TestCase(SpecialAbilityConstants.Vicious, 66, 69)]
        [TestCase(SpecialAbilityConstants.Anarchic, 70, 72)]
        [TestCase(SpecialAbilityConstants.Axiomatic, 73, 75)]
        [TestCase(SpecialAbilityConstants.Disruption, 76, 78)]
        [TestCase(SpecialAbilityConstants.FlamingBurst, 79, 81)]
        [TestCase(SpecialAbilityConstants.IcyBurst, 82, 84)]
        [TestCase(SpecialAbilityConstants.Holy, 85, 87)]
        [TestCase(SpecialAbilityConstants.ShockingBurst, 88, 90)]
        [TestCase(SpecialAbilityConstants.Unholy, 91, 93)]
        [TestCase(SpecialAbilityConstants.Wounding, 94, 95)]
        [TestCase("BonusSpecialAbility", 96, 100)]
        public void Percentile(string content, int lower, int upper)
        {
            base.AssertPercentile(content, lower, upper);
        }
    }
}