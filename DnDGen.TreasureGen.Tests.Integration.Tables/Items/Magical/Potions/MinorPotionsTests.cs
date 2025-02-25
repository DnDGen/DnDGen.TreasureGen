using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Potions
{
    [TestFixture]
    public class MinorPotionsTests : TypeAndAmountPercentileTests
    {
        protected override string tableName => TableNameConstants.Percentiles.POWERITEMTYPEs(PowerConstants.Minor, ItemTypeConstants.Potion);

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

        [TestCase(PotionConstants.CureLightWounds, 0, 1, 10)]
        [TestCase(PotionConstants.EndureElements, 0, 11, 13)]
        [TestCase(PotionConstants.HideFromAnimals, 0, 14, 15)]
        [TestCase(PotionConstants.HideFromUndead, 0, 16, 17)]
        [TestCase(PotionConstants.Jump, 0, 18, 19)]
        [TestCase(PotionConstants.MageArmor, 0, 20, 22)]
        [TestCase(PotionConstants.MagicFang, 0, 23, 25)]
        [TestCase(PotionConstants.MagicWeapon, 0, 27, 29)]
        [TestCase(PotionConstants.ProtectionFromPARTIALALIGNMENT, 0, 31, 32)]
        [TestCase(PotionConstants.RemoveFear, 0, 33, 34)]
        [TestCase(PotionConstants.ShieldOfFaith, 2, 36, 38)]
        [TestCase(PotionConstants.BlessWeapon, 0, 40, 41)]
        [TestCase(PotionConstants.EnlargePerson, 0, 42, 44)]
        [TestCase(PotionConstants.Aid, 0, 46, 47)]
        [TestCase(PotionConstants.Barkskin, 2, 48, 50)]
        [TestCase(PotionConstants.BearsEndurance, 0, 51, 53)]
        [TestCase(PotionConstants.Blur, 0, 54, 56)]
        [TestCase(PotionConstants.BullsStrength, 0, 57, 59)]
        [TestCase(PotionConstants.CatsGrace, 0, 60, 62)]
        [TestCase(PotionConstants.CureModerateWounds, 0, 63, 67)]
        [TestCase(PotionConstants.Darkvision, 0, 69, 71)]
        [TestCase(PotionConstants.DelayPoison, 0, 72, 74)]
        [TestCase(PotionConstants.EaglesSplendor, 0, 75, 76)]
        [TestCase(PotionConstants.FoxsCunning, 0, 77, 78)]
        [TestCase(PotionConstants.Invisibility_Potion, 0, 79, 80)]
        [TestCase(PotionConstants.Restoration_Lesser, 0, 82, 84)]
        [TestCase(PotionConstants.OwlsWisdom, 0, 88, 89)]
        [TestCase(PotionConstants.ProtectionFromArrows_10, 0, 90, 91)]
        [TestCase(PotionConstants.RemoveParalysis, 0, 92, 93)]
        [TestCase(PotionConstants.ResistENERGY_10, 0, 94, 96)]
        [TestCase(PotionConstants.SpiderClimb, 0, 98, 99)]
        public void MinorPotionPercentile(string name, int amount, int lower, int upper)
        {
            AssertTypeAndAmountPercentile(name, amount, lower, upper);
        }

        [TestCase(PotionConstants.MagicStone, 0, 26)]
        [TestCase(PotionConstants.PassWithoutTrace, 0, 30)]
        [TestCase(PotionConstants.Sanctuary, 0, 35)]
        [TestCase(PotionConstants.Shillelagh, 0, 39)]
        [TestCase(PotionConstants.ReducePerson, 0, 45)]
        [TestCase(PotionConstants.Darkness, 0, 68)]
        [TestCase(PotionConstants.Invisibility_Oil, 0, 81)]
        [TestCase(PotionConstants.Levitate_Potion, 0, 85)]
        [TestCase(PotionConstants.Levitate_Oil, 0, 86)]
        [TestCase(PotionConstants.Misdirection, 0, 87)]
        [TestCase(PotionConstants.ShieldOfFaith, 3, 97)]
        [TestCase(PotionConstants.UndetectableAlignment, 0, 100)]
        public void MinorPotionPercentile(string name, int amount, int roll)
        {
            AssertTypeAndAmountPercentile(name, amount, roll);
        }
    }
}