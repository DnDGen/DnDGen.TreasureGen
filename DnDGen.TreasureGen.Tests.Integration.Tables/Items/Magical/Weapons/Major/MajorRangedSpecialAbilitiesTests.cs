using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Weapons.Major
{
    [TestFixture]
    public class MajorRangedSpecialAbilitiesTests : PercentileTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Percentiles.POWERATTRIBUTESpecialAbilities(PowerConstants.Major, AttributeConstants.Ranged); }
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

        [TestCase(SpecialAbilityConstants.DESIGNATEDFOEbane, 1, 4)]
        [TestCase(SpecialAbilityConstants.Distance, 5, 8)]
        [TestCase(SpecialAbilityConstants.Flaming, 9, 12)]
        [TestCase(SpecialAbilityConstants.Frost, 13, 16)]
        [TestCase(SpecialAbilityConstants.Returning, 17, 21)]
        [TestCase(SpecialAbilityConstants.Shock, 22, 25)]
        [TestCase(SpecialAbilityConstants.Seeking, 26, 27)]
        [TestCase(SpecialAbilityConstants.Thundering, 28, 29)]
        [TestCase(SpecialAbilityConstants.Anarchic, 30, 34)]
        [TestCase(SpecialAbilityConstants.Axiomatic, 35, 39)]
        [TestCase(SpecialAbilityConstants.FlamingBurst, 40, 49)]
        [TestCase(SpecialAbilityConstants.Holy, 50, 54)]
        [TestCase(SpecialAbilityConstants.IcyBurst, 55, 64)]
        [TestCase(SpecialAbilityConstants.ShockingBurst, 65, 74)]
        [TestCase(SpecialAbilityConstants.Unholy, 75, 79)]
        [TestCase(SpecialAbilityConstants.Speed, 80, 84)]
        [TestCase(SpecialAbilityConstants.BrilliantEnergy, 85, 90)]
        [TestCase("BonusSpecialAbility", 91, 100)]
        public void Percentile(string content, int lower, int upper)
        {
            base.AssertPercentile(content, lower, upper);
        }
    }
}