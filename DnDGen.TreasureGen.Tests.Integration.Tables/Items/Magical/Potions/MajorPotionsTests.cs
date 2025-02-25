using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Potions
{
    [TestFixture]
    public class MajorPotionsTests : TypeAndAmountPercentileTests
    {
        protected override string tableName => TableNameConstants.Percentiles.POWERITEMTYPEs(PowerConstants.Major, ItemTypeConstants.Potion);

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

        [TestCase(1, 2, PotionConstants.Blur, 0)]
        [TestCase(3, 7, PotionConstants.CureModerateWounds, 0)]
        [TestCase(8, 9, PotionConstants.Darkvision, 0)]
        [TestCase(10, 10, PotionConstants.Invisibility_Potion, 0)]
        [TestCase(11, 11, PotionConstants.Invisibility_Oil, 0)]
        [TestCase(12, 12, PotionConstants.Restoration_Lesser, 0)]
        [TestCase(13, 13, PotionConstants.RemoveParalysis, 0)]
        [TestCase(14, 14, PotionConstants.ShieldOfFaith, 3)]
        [TestCase(15, 15, PotionConstants.UndetectableAlignment, 0)]
        [TestCase(16, 16, PotionConstants.Barkskin, 3)]
        [TestCase(17, 18, PotionConstants.ShieldOfFaith, 4)]
        [TestCase(19, 20, PotionConstants.ResistENERGY_20, 0)]
        [TestCase(21, 28, PotionConstants.CureSeriousWounds, 0)]
        [TestCase(29, 29, PotionConstants.Daylight, 0)]
        [TestCase(30, 32, PotionConstants.Displacement, 0)]
        [TestCase(33, 33, PotionConstants.FlameArrow, 0)]
        [TestCase(34, 38, PotionConstants.Fly, 0)]
        [TestCase(39, 39, PotionConstants.GaseousForm, 0)]
        [TestCase(40, 41, PotionConstants.Haste, 0)]
        [TestCase(42, 44, PotionConstants.Heroism, 0)]
        [TestCase(45, 46, PotionConstants.KeenEdge, 0)]
        [TestCase(47, 47, PotionConstants.MagicCircleAgainstPARTIALALIGNMENT, 0)]
        [TestCase(48, 50, PotionConstants.NeutralizePoison, 0)]
        [TestCase(51, 52, PotionConstants.Nondetection, 0)]
        [TestCase(53, 54, PotionConstants.ProtectionFromENERGY, 0)]
        [TestCase(55, 55, PotionConstants.Rage, 0)]
        [TestCase(56, 56, PotionConstants.RemoveBlindnessDeafness, 0)]
        [TestCase(57, 57, PotionConstants.RemoveCurse, 0)]
        [TestCase(58, 58, PotionConstants.RemoveDisease, 0)]
        [TestCase(59, 59, PotionConstants.Tongues, 0)]
        [TestCase(60, 60, PotionConstants.WaterBreathing, 0)]
        [TestCase(61, 61, PotionConstants.WaterWalk, 0)]
        [TestCase(62, 63, PotionConstants.Barkskin, 4)]
        [TestCase(64, 64, PotionConstants.ShieldOfFaith, 5)]
        [TestCase(65, 65, PotionConstants.GoodHope, 0)]
        [TestCase(66, 68, PotionConstants.ResistENERGY_30, 0)]
        [TestCase(69, 69, PotionConstants.Barkskin, 5)]
        [TestCase(70, 73, PotionConstants.MagicFang_Greater, 2)]
        [TestCase(74, 77, PotionConstants.MagicWeapon_Greater, 2)]
        [TestCase(78, 81, PotionConstants.MagicVestment, 2)]
        [TestCase(82, 82, PotionConstants.ProtectionFromArrows_15, 0)]
        [TestCase(83, 85, PotionConstants.MagicFang_Greater, 3)]
        [TestCase(86, 88, PotionConstants.MagicWeapon_Greater, 3)]
        [TestCase(89, 91, PotionConstants.MagicVestment, 3)]
        [TestCase(92, 93, PotionConstants.MagicFang_Greater, 4)]
        [TestCase(94, 95, PotionConstants.MagicWeapon_Greater, 4)]
        [TestCase(96, 97, PotionConstants.MagicVestment, 4)]
        [TestCase(98, 98, PotionConstants.MagicFang_Greater, 5)]
        [TestCase(99, 99, PotionConstants.MagicWeapon_Greater, 5)]
        [TestCase(100, 100, PotionConstants.MagicVestment, 5)]
        public void MajorPotionPercentile(int lower, int upper, string name, int amount)
        {
            AssertTypeAndAmountPercentile(name, amount, lower, upper);
        }
    }
}