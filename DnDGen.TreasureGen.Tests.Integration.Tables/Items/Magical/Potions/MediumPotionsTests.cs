using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Potions
{
    [TestFixture]
    public class MediumPotionsTests : TypeAndAmountPercentileTests
    {
        protected override string tableName => TableNameConstants.Percentiles.POWERITEMTYPEs(PowerConstants.Medium, ItemTypeConstants.Potion);

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

        [TestCase(PotionConstants.BlessWeapon, 0, 1, 2)]
        [TestCase(PotionConstants.EnlargePerson, 0, 3, 4)]
        [TestCase(PotionConstants.BearsEndurance, 0, 8, 10)]
        [TestCase(PotionConstants.Blur, 0, 11, 13)]
        [TestCase(PotionConstants.BullsStrength, 0, 14, 16)]
        [TestCase(PotionConstants.CatsGrace, 0, 17, 19)]
        [TestCase(PotionConstants.CureModerateWounds, 0, 20, 27)]
        [TestCase(PotionConstants.Darkvision, 0, 29, 30)]
        [TestCase(PotionConstants.EaglesSplendor, 0, 32, 33)]
        [TestCase(PotionConstants.FoxsCunning, 0, 34, 35)]
        [TestCase(PotionConstants.OwlsWisdom, 0, 41, 42)]
        [TestCase(PotionConstants.ResistENERGY_10, 0, 45, 46)]
        [TestCase(PotionConstants.ShieldOfFaith, 3, 47, 48)]
        [TestCase(PotionConstants.ResistENERGY_20, 0, 53, 55)]
        [TestCase(PotionConstants.CureSeriousWounds, 0, 56, 60)]
        [TestCase(PotionConstants.Displacement, 0, 62, 64)]
        [TestCase(PotionConstants.Fly, 0, 66, 68)]
        [TestCase(PotionConstants.MagicFang_Greater, 1, 70, 71)]
        [TestCase(PotionConstants.MagicWeapon_Greater, 1, 72, 73)]
        [TestCase(PotionConstants.Haste, 0, 74, 75)]
        [TestCase(PotionConstants.Heroism, 0, 76, 78)]
        [TestCase(PotionConstants.KeenEdge, 0, 79, 80)]
        [TestCase(PotionConstants.MagicVestment, 1, 82, 83)]
        [TestCase(PotionConstants.NeutralizePoison, 0, 84, 86)]
        [TestCase(PotionConstants.Nondetection, 0, 87, 88)]
        [TestCase(PotionConstants.ProtectionFromENERGY, 0, 89, 91)]
        [TestCase(PotionConstants.Rage, 0, 92, 93)]
        [TestCase(PotionConstants.WaterBreathing, 0, 98, 99)]
        public void MediumPotionPercentile(string name, int amount, int lower, int upper)
        {
            AssertTypeAndAmountPercentile(name, amount, lower, upper);
        }

        [TestCase(PotionConstants.ReducePerson, 0, 5)]
        [TestCase(PotionConstants.Aid, 0, 6)]
        [TestCase(PotionConstants.Barkskin, 2, 7)]
        [TestCase(PotionConstants.Darkness, 0, 28)]
        [TestCase(PotionConstants.DelayPoison, 0, 31)]
        [TestCase(PotionConstants.Invisibility_Potion, 0, 36)]
        [TestCase(PotionConstants.Invisibility_Oil, 0, 37)]
        [TestCase(PotionConstants.Restoration_Lesser, 0, 38)]
        [TestCase(PotionConstants.Levitate_Potion, 0, 39)]
        [TestCase(PotionConstants.Misdirection, 0, 40)]
        [TestCase(PotionConstants.ProtectionFromArrows_10, 0, 43)]
        [TestCase(PotionConstants.RemoveParalysis, 0, 44)]
        [TestCase(PotionConstants.SpiderClimb, 0, 49)]
        [TestCase(PotionConstants.UndetectableAlignment, 0, 50)]
        [TestCase(PotionConstants.Barkskin, 3, 51)]
        [TestCase(PotionConstants.ShieldOfFaith, 4, 52)]
        [TestCase(PotionConstants.Daylight, 0, 61)]
        [TestCase(PotionConstants.FlameArrow, 0, 65)]
        [TestCase(PotionConstants.GaseousForm, 0, 69)]
        [TestCase(PotionConstants.MagicCircleAgainstPARTIALALIGNMENT, 0, 81)]
        [TestCase(PotionConstants.RemoveBlindnessDeafness, 0, 94)]
        [TestCase(PotionConstants.RemoveCurse, 0, 95)]
        [TestCase(PotionConstants.RemoveDisease, 0, 96)]
        [TestCase(PotionConstants.Tongues, 0, 97)]
        [TestCase(PotionConstants.WaterWalk, 0, 100)]
        public void MediumPotionPercentile(string name, int amount, int roll)
        {
            AssertTypeAndAmountPercentile(name, amount, roll);
        }
    }
}