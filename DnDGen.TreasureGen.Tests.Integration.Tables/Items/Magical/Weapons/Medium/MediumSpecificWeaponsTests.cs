using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Weapons.Medium
{
    [TestFixture]
    public class MediumSpecificWeaponsTests : TypeAndAmountPercentileTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Percentiles.POWERSpecificITEMTYPEs(PowerConstants.Medium, ItemTypeConstants.Weapon); }
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

        [TestCase(WeaponConstants.JavelinOfLightning, 0, 1, 9)]
        [TestCase(WeaponConstants.SlayingArrow, 1, 10, 15)]
        [TestCase(WeaponConstants.Dagger_Adamantine, 0, 16, 24)]
        [TestCase(WeaponConstants.Battleaxe_Adamantine, 0, 25, 33)]
        [TestCase(WeaponConstants.GreaterSlayingArrow, 1, 34, 37)]
        [TestCase(WeaponConstants.Shatterspike, 1, 38, 40)]
        [TestCase(WeaponConstants.DaggerOfVenom, 1, 41, 46)]
        [TestCase(WeaponConstants.TridentOfWarning, 2, 47, 51)]
        [TestCase(WeaponConstants.AssassinsDagger, 2, 52, 57)]
        [TestCase(WeaponConstants.ShiftersSorrow, 1, 58, 62)]
        [TestCase(WeaponConstants.TridentOfFishCommand, 1, 63, 66)]
        [TestCase(WeaponConstants.FlameTongue, 1, 67, 74)]
        [TestCase(WeaponConstants.LuckBlade0, 2, 75, 79)]
        [TestCase(WeaponConstants.SwordOfSubtlety, 1, 80, 86)]
        [TestCase(WeaponConstants.SwordOfThePlanes, 1, 87, 91)]
        [TestCase(WeaponConstants.NineLivesStealer, 2, 92, 95)]
        [TestCase(WeaponConstants.SwordOfLifeStealing, 2, 96, 98)]
        [TestCase(WeaponConstants.Oathbow, 2, 99, 100)]
        public void TypeAndAmountPercentile(string type, int amount, int lower, int upper)
        {
            base.AssertTypeAndAmountPercentile(type, amount, lower, upper);
        }
    }
}