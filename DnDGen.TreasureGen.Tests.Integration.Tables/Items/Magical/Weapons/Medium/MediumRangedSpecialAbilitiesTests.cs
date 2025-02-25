using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Weapons.Medium
{
    [TestFixture]
    public class MediumRangedSpecialAbilitiesTests : PercentileTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Percentiles.POWERATTRIBUTESpecialAbilities(PowerConstants.Medium, AttributeConstants.Ranged); }
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

        [TestCase(SpecialAbilityConstants.DESIGNATEDFOEbane, 1, 8)]
        [TestCase(SpecialAbilityConstants.Distance, 9, 16)]
        [TestCase(SpecialAbilityConstants.Flaming, 17, 28)]
        [TestCase(SpecialAbilityConstants.Frost, 29, 40)]
        [TestCase(SpecialAbilityConstants.Merciful, 41, 42)]
        [TestCase(SpecialAbilityConstants.Returning, 43, 47)]
        [TestCase(SpecialAbilityConstants.Shock, 48, 59)]
        [TestCase(SpecialAbilityConstants.Seeking, 60, 64)]
        [TestCase(SpecialAbilityConstants.Thundering, 65, 68)]
        [TestCase(SpecialAbilityConstants.Anarchic, 69, 71)]
        [TestCase(SpecialAbilityConstants.Axiomatic, 72, 74)]
        [TestCase(SpecialAbilityConstants.FlamingBurst, 75, 79)]
        [TestCase(SpecialAbilityConstants.Holy, 80, 82)]
        [TestCase(SpecialAbilityConstants.IcyBurst, 83, 87)]
        [TestCase(SpecialAbilityConstants.ShockingBurst, 88, 92)]
        [TestCase(SpecialAbilityConstants.Unholy, 93, 95)]
        [TestCase("BonusSpecialAbility", 96, 100)]
        public void Percentile(string content, int lower, int upper)
        {
            base.AssertPercentile(content, lower, upper);
        }
    }
}