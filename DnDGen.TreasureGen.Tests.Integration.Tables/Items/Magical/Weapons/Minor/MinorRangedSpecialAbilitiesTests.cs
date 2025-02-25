using NUnit.Framework;
using DnDGen.TreasureGen.Tables;
using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Weapons.Minor
{
    [TestFixture]
    public class MinorRangedSpecialAbilitiesTests : PercentileTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Percentiles.POWERATTRIBUTESpecialAbilities(PowerConstants.Minor, AttributeConstants.Ranged); }
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

        [TestCase(SpecialAbilityConstants.DESIGNATEDFOEbane, 1, 12)]
        [TestCase(SpecialAbilityConstants.Distance, 13, 25)]
        [TestCase(SpecialAbilityConstants.Flaming, 26, 40)]
        [TestCase(SpecialAbilityConstants.Frost, 41, 55)]
        [TestCase(SpecialAbilityConstants.Merciful, 56, 60)]
        [TestCase(SpecialAbilityConstants.Returning, 61, 68)]
        [TestCase(SpecialAbilityConstants.Shock, 69, 83)]
        [TestCase(SpecialAbilityConstants.Seeking, 84, 93)]
        [TestCase(SpecialAbilityConstants.Thundering, 94, 99)]
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