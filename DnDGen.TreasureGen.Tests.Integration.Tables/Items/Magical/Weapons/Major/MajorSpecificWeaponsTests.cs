using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Weapons.Major
{
    [TestFixture]
    public class MajorSpecificWeaponsTests : TypeAndAmountPercentileTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Percentiles.POWERSpecificITEMTYPEs(PowerConstants.Major, ItemTypeConstants.Weapon); }
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

        [TestCase(WeaponConstants.AssassinsDagger, 2, 1, 4)]
        [TestCase(WeaponConstants.ShiftersSorrow, 1, 5, 7)]
        [TestCase(WeaponConstants.TridentOfFishCommand, 1, 8, 9)]
        [TestCase(WeaponConstants.FlameTongue, 1, 10, 13)]
        [TestCase(WeaponConstants.LuckBlade0, 2, 14, 17)]
        [TestCase(WeaponConstants.SwordOfSubtlety, 1, 18, 24)]
        [TestCase(WeaponConstants.SwordOfThePlanes, 1, 25, 31)]
        [TestCase(WeaponConstants.NineLivesStealer, 2, 32, 37)]
        [TestCase(WeaponConstants.SwordOfLifeStealing, 2, 38, 42)]
        [TestCase(WeaponConstants.Oathbow, 2, 43, 46)]
        [TestCase(WeaponConstants.MaceOfTerror, 2, 47, 51)]
        [TestCase(WeaponConstants.LifeDrinker, 1, 52, 57)]
        [TestCase(WeaponConstants.SylvanScimitar, 3, 58, 62)]
        [TestCase(WeaponConstants.RapierOfPuncturing, 2, 63, 67)]
        [TestCase(WeaponConstants.SunBlade, 2, 68, 73)]
        [TestCase(WeaponConstants.FrostBrand, 3, 74, 79)]
        [TestCase(WeaponConstants.DwarvenThrower, 2, 80, 84)]
        [TestCase(WeaponConstants.LuckBlade1, 2, 85, 91)]
        [TestCase(WeaponConstants.MaceOfSmiting, 3, 92, 95)]
        [TestCase(WeaponConstants.LuckBlade2, 2, 96, 97)]
        [TestCase(WeaponConstants.HolyAvenger, 2, 98, 99)]
        public void TypeAndAmountPercentile(string type, int amount, int lower, int upper)
        {
            AssertTypeAndAmountPercentile(type, amount, lower, upper);
        }

        [TestCase(WeaponConstants.LuckBlade3, 2, 100)]
        public void TypeAndAmountPercentile(string type, int amount, int roll)
        {
            AssertTypeAndAmountPercentile(type, amount, roll);
        }
    }
}