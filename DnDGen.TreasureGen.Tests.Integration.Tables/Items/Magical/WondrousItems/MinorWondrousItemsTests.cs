using NUnit.Framework;
using DnDGen.TreasureGen.Tables;
using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.WondrousItems
{
    [TestFixture]
    public class MinorWondrousItemsTests : TypeAndAmountPercentileTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Percentiles.POWERITEMTYPEs(PowerConstants.Minor, ItemTypeConstants.WondrousItem); }
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

        [TestCase(WondrousItemConstants.QuaalsFeatherToken_Anchor, 0, 1)]
        [TestCase(WondrousItemConstants.UniversalSolvent, 0, 2)]
        [TestCase(WondrousItemConstants.ElixerOfLove, 0, 3)]
        [TestCase(WondrousItemConstants.UnguentOfTimelessness, 0, 4)]
        [TestCase(WondrousItemConstants.QuaalsFeatherToken_Fan, 0, 5)]
        [TestCase(WondrousItemConstants.DustOfTracelessness, 0, 6)]
        [TestCase(WondrousItemConstants.ElixerOfHiding, 0, 7)]
        [TestCase(WondrousItemConstants.ElixerOfSneaking, 0, 8)]
        [TestCase(WondrousItemConstants.ElixerOfSwimming, 0, 9)]
        [TestCase(WondrousItemConstants.ElixerOfVision, 0, 10)]
        [TestCase(WondrousItemConstants.Silversheen, 0, 11)]
        [TestCase(WondrousItemConstants.QuaalsFeatherToken_Bird, 0, 12)]
        [TestCase(WondrousItemConstants.QuaalsFeatherToken_Tree, 0, 13)]
        [TestCase(WondrousItemConstants.QuaalsFeatherToken_SwanBoat, 0, 14)]
        [TestCase(WondrousItemConstants.ElixerOfTruth, 0, 15)]
        [TestCase(WondrousItemConstants.QuaalsFeatherToken_Whip, 0, 16)]
        [TestCase(WondrousItemConstants.DustOfDryness, 0, 17)]
        [TestCase(WondrousItemConstants.BagOfTricks_Gray, 0, 18)]
        [TestCase(WondrousItemConstants.HandOfTheMage, 0, 19)]
        [TestCase(WondrousItemConstants.BracersOfArmor, 1, 20)]
        [TestCase(WondrousItemConstants.CloakOfResistance, 1, 21)]
        [TestCase(WondrousItemConstants.PearlOfPower_1stLevel, 0, 22)]
        [TestCase(WondrousItemConstants.PhylacteryOfFaithfulness, 0, 23)]
        [TestCase(WondrousItemConstants.SalveOfSlipperiness, 0, 24)]
        [TestCase(WondrousItemConstants.ElixerOfFireBreath, 0, 25)]
        [TestCase(WondrousItemConstants.PipesOfTheSewers, 0, 26)]
        [TestCase(WondrousItemConstants.DustOfIllusion, 0, 27)]
        [TestCase(WondrousItemConstants.GogglesOfMinuteSeeing, 0, 28)]
        [TestCase(WondrousItemConstants.BroochOfShielding, 0, 29)]
        [TestCase(WondrousItemConstants.NecklaceOfFireballs_I, 0, 30)]
        [TestCase(WondrousItemConstants.DustOfAppearance, 0, 31)]
        [TestCase(WondrousItemConstants.HatOfDisguise, 0, 32)]
        [TestCase(WondrousItemConstants.PipesOfSounding, 0, 33)]
        [TestCase(WondrousItemConstants.QuiverOfEhlonna, 0, 34)]
        [TestCase(WondrousItemConstants.AmuletOfNaturalArmor, 1, 35)]
        [TestCase(WondrousItemConstants.HewardsHandyHaversack, 0, 36)]
        [TestCase(WondrousItemConstants.HornOfFog, 0, 37)]
        [TestCase(WondrousItemConstants.ElementalGem, 0, 38)]
        [TestCase(WondrousItemConstants.RobeOfBones, 0, 39)]
        [TestCase(WondrousItemConstants.SovereignGlue, 0, 40)]
        [TestCase(WondrousItemConstants.BagOfHolding_I, 0, 41)]
        [TestCase(WondrousItemConstants.BootsOfElvenkind, 0, 42)]
        [TestCase(WondrousItemConstants.BootsOfTheWinterlands, 0, 43)]
        [TestCase(WondrousItemConstants.CandleOfTruth, 0, 44)]
        [TestCase(WondrousItemConstants.CloakOfElvenkind, 0, 45)]
        [TestCase(WondrousItemConstants.EyesOfTheEagle, 0, 46)]
        [TestCase(WondrousItemConstants.GolembaneScarab, 0, 47)]
        [TestCase(WondrousItemConstants.NecklaceOfFireballs_II, 0, 48)]
        [TestCase(WondrousItemConstants.StoneOfAlarm, 0, 49)]
        [TestCase(WondrousItemConstants.BagOfTricks_Rust, 0, 50)]
        [TestCase(WondrousItemConstants.BeadOfForce, 0, 51)]
        [TestCase(WondrousItemConstants.ChimeOfOpening, 0, 52)]
        [TestCase(WondrousItemConstants.HorseshoesOfSpeed, 0, 53)]
        [TestCase(WondrousItemConstants.RopeOfClimbing, 0, 54)]
        [TestCase(WondrousItemConstants.DustOfDisappearance, 0, 55)]
        [TestCase(WondrousItemConstants.LensOfDetection, 0, 56)]
        [TestCase(WondrousItemConstants.DruidsVestments, 0, 57)]
        [TestCase(WondrousItemConstants.FigurineOfWondrousPower_SilverRaven, 0, 58)]
        [TestCase(WondrousItemConstants.AmuletOfHealth, 2, 59)]
        [TestCase(WondrousItemConstants.BracersOfArmor, 2, 60)]
        [TestCase(WondrousItemConstants.CloakOfCharisma, 2, 61)]
        [TestCase(WondrousItemConstants.CloakOfResistance, 2, 62)]
        [TestCase(WondrousItemConstants.GauntletsOfOgrePower, 0, 63)]
        [TestCase(WondrousItemConstants.GlovesOfArrowSnaring, 0, 64)]
        [TestCase(WondrousItemConstants.GlovesOfDexterity, 2, 65)]
        [TestCase(WondrousItemConstants.HeadbandOfIntellect, 2, 66)]
        [TestCase(WondrousItemConstants.IounStone_ClearSpindle, 0, 67)]
        [TestCase(WondrousItemConstants.KeoghtomsOintment, 0, 68)]
        [TestCase(WondrousItemConstants.NolzursMarvelousPigments, 0, 69)]
        [TestCase(WondrousItemConstants.PearlOfPower_2ndLevel, 0, 70)]
        [TestCase(WondrousItemConstants.PeriaptOfWisdom, 2, 71)]
        [TestCase(WondrousItemConstants.StoneSalve, 0, 72)]
        [TestCase(WondrousItemConstants.NecklaceOfFireballs_III, 0, 73)]
        [TestCase(WondrousItemConstants.CircletOfPersuasion, 0, 74)]
        [TestCase(WondrousItemConstants.SlippersOfSpiderClimbing, 0, 75)]
        [TestCase(WondrousItemConstants.IncenseOfMeditation, 0, 76)]
        [TestCase(WondrousItemConstants.BagOfHolding_II, 0, 77)]
        [TestCase(WondrousItemConstants.BracersOfArchery_Lesser, 0, 78)]
        [TestCase(WondrousItemConstants.IounStone_DustyRosePrism, 0, 79)]
        [TestCase(WondrousItemConstants.HelmOfComprehendLanguagesAndReadMagic, 0, 80)]
        [TestCase(WondrousItemConstants.VestOfEscape, 0, 81)]
        [TestCase(WondrousItemConstants.EversmokingBottle, 0, 82)]
        [TestCase(WondrousItemConstants.MurlyndsSpoon, 0, 83)]
        [TestCase(WondrousItemConstants.NecklaceOfFireballs_IV, 0, 84)]
        [TestCase(WondrousItemConstants.BootsOfStridingAndSpringing, 0, 85)]
        [TestCase(WondrousItemConstants.WindFan, 0, 86)]
        [TestCase(WondrousItemConstants.AmuletOfMightyFists, 1, 87)]
        [TestCase(WondrousItemConstants.HorseshoesOfAZephyr, 0, 88)]
        [TestCase(WondrousItemConstants.PipesOfHaunting, 0, 89)]
        [TestCase(WondrousItemConstants.NecklaceOfFireballs_V, 0, 90)]
        [TestCase(WondrousItemConstants.GlovesOfSwimmingAndClimbing, 0, 91)]
        [TestCase(WondrousItemConstants.BagOfTricks_Tan, 0, 92)]
        [TestCase(WondrousItemConstants.CircletOfBlasting_Minor, 0, 93)]
        [TestCase(WondrousItemConstants.HornOfGoodnessEvil, 0, 94)]
        [TestCase(WondrousItemConstants.RobeOfUsefulItems, 0, 95)]
        [TestCase(WondrousItemConstants.FoldingBoat, 0, 96)]
        [TestCase(WondrousItemConstants.CloakOfTheMantaRay, 0, 97)]
        [TestCase(WondrousItemConstants.BottleOfAir, 0, 98)]
        [TestCase(WondrousItemConstants.BagOfHolding_III, 0, 99)]
        [TestCase(WondrousItemConstants.PeriaptOfHealth, 0, 100)]
        public void TypeAndAmountPercentile(string type, int amount, int roll)
        {
            base.AssertTypeAndAmountPercentile(type, amount, roll);
        }
    }
}