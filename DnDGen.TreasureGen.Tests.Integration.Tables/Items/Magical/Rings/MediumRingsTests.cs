using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Rings
{
    [TestFixture]
    public class MediumRingsTests : TypeAndAmountPercentileTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Percentiles.POWERITEMTYPEs(PowerConstants.Medium, ItemTypeConstants.Ring); }
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

        [TestCase(RingConstants.Counterspells, 0, 1, 5)]
        [TestCase(RingConstants.MindShielding, 0, 6, 8)]
        [TestCase(RingConstants.Protection, 2, 9, 18)]
        [TestCase(RingConstants.ForceShield, 0, 19, 23)]
        [TestCase(RingConstants.Ram, 0, 24, 28)]
        [TestCase(RingConstants.Climbing_Improved, 0, 29, 34)]
        [TestCase(RingConstants.Jumping_Improved, 0, 35, 40)]
        [TestCase(RingConstants.Swimming_Improved, 0, 41, 46)]
        [TestCase(RingConstants.AnimalFriendship, 0, 47, 51)]
        [TestCase(RingConstants.ENERGYResistance_Minor, 0, 52, 56)]
        [TestCase(RingConstants.ChameleonPower, 0, 57, 61)]
        [TestCase(RingConstants.WaterWalking, 0, 62, 66)]
        [TestCase(RingConstants.Protection, 3, 67, 71)]
        [TestCase(RingConstants.SpellStoring_Minor, 0, 72, 76)]
        [TestCase(RingConstants.Invisibility, 0, 77, 81)]
        [TestCase(RingConstants.Wizardry_I, 0, 82, 85)]
        [TestCase(RingConstants.Evasion, 0, 86, 90)]
        [TestCase(RingConstants.XRayVision, 0, 91, 93)]
        [TestCase(RingConstants.Blinking, 0, 94, 97)]
        [TestCase(RingConstants.ENERGYResistance_Major, 0, 98, 100)]
        public void MediumRingsPercentile(string type, int amount, int lower, int upper)
        {
            AssertTypeAndAmountPercentile(type, amount, lower, upper);
        }
    }
}