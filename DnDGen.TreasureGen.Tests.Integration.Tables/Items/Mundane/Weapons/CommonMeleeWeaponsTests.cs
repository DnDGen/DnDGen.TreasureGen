using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Mundane.Weapons
{
    [TestFixture]
    public class CommonMeleeWeaponsTests : PercentileTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Percentiles.WEAPONTYPEWeapons("CommonMelee"); }
        }

        [Test]
        public override void ReplacementStringsAreValid()
        {
            AssertReplacementStringsAreValid();
        }

        [TestCase(WeaponConstants.Dagger, 1, 4)]
        [TestCase(WeaponConstants.Greataxe, 5, 14)]
        [TestCase(WeaponConstants.Greatsword, 15, 24)]
        [TestCase(WeaponConstants.Kama, 25, 28)]
        [TestCase(WeaponConstants.Longsword, 29, 41)]
        [TestCase(WeaponConstants.LightMace, 42, 45)]
        [TestCase(WeaponConstants.HeavyMace, 46, 50)]
        [TestCase(WeaponConstants.Nunchaku, 51, 54)]
        [TestCase(WeaponConstants.Quarterstaff, 55, 57)]
        [TestCase(WeaponConstants.Rapier, 58, 61)]
        [TestCase(WeaponConstants.Scimitar, 62, 66)]
        [TestCase(WeaponConstants.Shortspear, 67, 70)]
        [TestCase(WeaponConstants.Siangham, 71, 74)]
        [TestCase(WeaponConstants.BastardSword, 75, 84)]
        [TestCase(WeaponConstants.ShortSword, 85, 89)]
        [TestCase(WeaponConstants.DwarvenWaraxe, 90, 100)]
        public void Percentile(string content, int lower, int upper)
        {
            AssertPercentile(content, lower, upper);
        }

        [Test]
        public override void TableIsComplete()
        {
            AssertTableIsComplete();
        }
    }
}