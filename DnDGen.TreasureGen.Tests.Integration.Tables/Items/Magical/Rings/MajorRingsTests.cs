using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Rings
{
    [TestFixture]
    public class MajorRingsTests : TypeAndAmountPercentileTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Percentiles.POWERITEMTYPEs(PowerConstants.Major, ItemTypeConstants.Ring); }
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

        [TestCase(RingConstants.ENERGYResistance_Minor, 0, 1, 2)]
        [TestCase(RingConstants.Protection, 3, 3, 7)]
        [TestCase(RingConstants.SpellStoring_Minor, 0, 8, 10)]
        [TestCase(RingConstants.Invisibility, 0, 11, 15)]
        [TestCase(RingConstants.Wizardry_I, 0, 16, 19)]
        [TestCase(RingConstants.Evasion, 0, 20, 25)]
        [TestCase(RingConstants.XRayVision, 0, 26, 28)]
        [TestCase(RingConstants.Blinking, 0, 29, 32)]
        [TestCase(RingConstants.ENERGYResistance_Major, 0, 33, 39)]
        [TestCase(RingConstants.Protection, 4, 40, 49)]
        [TestCase(RingConstants.Wizardry_II, 0, 50, 55)]
        [TestCase(RingConstants.FreedomOfMovement, 0, 56, 60)]
        [TestCase(RingConstants.ENERGYResistance_Greater, 0, 61, 63)]
        [TestCase(RingConstants.FriendShield, 0, 64, 65)]
        [TestCase(RingConstants.Protection, 5, 66, 70)]
        [TestCase(RingConstants.ShootingStars, 0, 71, 74)]
        [TestCase(RingConstants.SpellStoring, 0, 75, 79)]
        [TestCase(RingConstants.Wizardry_III, 0, 80, 83)]
        [TestCase(RingConstants.Telekinesis, 0, 84, 86)]
        [TestCase(RingConstants.Regeneration, 0, 87, 88)]
        [TestCase(RingConstants.ThreeWishes, 0, 89, 89)]
        [TestCase(RingConstants.SpellTurning, 0, 90, 92)]
        [TestCase(RingConstants.Wizardry_IV, 0, 93, 94)]
        [TestCase(RingConstants.DjinniCalling, 0, 95, 95)]
        [TestCase(RingConstants.ElementalCommand_Air, 0, 96, 96)]
        [TestCase(RingConstants.ElementalCommand_Earth, 0, 97, 97)]
        [TestCase(RingConstants.ElementalCommand_Fire, 0, 98, 98)]
        [TestCase(RingConstants.ElementalCommand_Water, 0, 99, 99)]
        [TestCase(RingConstants.SpellStoring_Major, 0, 100, 100)]
        public void MajorRingsPercentile(string type, int amount, int lower, int upper)
        {
            AssertTypeAndAmountPercentile(type, amount, lower, upper);
        }
    }
}