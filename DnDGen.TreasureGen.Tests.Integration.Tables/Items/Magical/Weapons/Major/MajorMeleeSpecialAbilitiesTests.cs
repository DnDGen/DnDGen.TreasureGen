using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Weapons.Major
{
    [TestFixture]
    public class MajorMeleeSpecialAbilitiesTests : PercentileTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Percentiles.POWERATTRIBUTESpecialAbilities(PowerConstants.Major, AttributeConstants.Melee); }
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

        [TestCase(SpecialAbilityConstants.DESIGNATEDFOEbane, 1, 3)]
        [TestCase(SpecialAbilityConstants.Flaming, 4, 6)]
        [TestCase(SpecialAbilityConstants.Frost, 7, 9)]
        [TestCase(SpecialAbilityConstants.Shock, 10, 12)]
        [TestCase(SpecialAbilityConstants.GhostTouchWeapon, 13, 15)]
        [TestCase(SpecialAbilityConstants.KiFocus, 16, 19)]
        [TestCase(SpecialAbilityConstants.MightyCleaving, 20, 21)]
        [TestCase(SpecialAbilityConstants.SpellStoring, 22, 24)]
        [TestCase(SpecialAbilityConstants.Throwing, 25, 28)]
        [TestCase(SpecialAbilityConstants.Thundering, 29, 32)]
        [TestCase(SpecialAbilityConstants.Vicious, 33, 36)]
        [TestCase(SpecialAbilityConstants.Anarchic, 37, 41)]
        [TestCase(SpecialAbilityConstants.Axiomatic, 42, 46)]
        [TestCase(SpecialAbilityConstants.Disruption, 47, 49)]
        [TestCase(SpecialAbilityConstants.FlamingBurst, 50, 54)]
        [TestCase(SpecialAbilityConstants.IcyBurst, 55, 59)]
        [TestCase(SpecialAbilityConstants.Holy, 60, 64)]
        [TestCase(SpecialAbilityConstants.ShockingBurst, 65, 69)]
        [TestCase(SpecialAbilityConstants.Unholy, 70, 74)]
        [TestCase(SpecialAbilityConstants.Wounding, 75, 78)]
        [TestCase(SpecialAbilityConstants.Speed, 79, 83)]
        [TestCase(SpecialAbilityConstants.BrilliantEnergy, 84, 86)]
        [TestCase(SpecialAbilityConstants.Dancing, 87, 88)]
        [TestCase(SpecialAbilityConstants.Vorpal, 89, 90)]
        [TestCase("BonusSpecialAbility", 91, 100)]
        public void Percentile(string content, int lower, int upper)
        {
            base.AssertPercentile(content, lower, upper);
        }
    }
}