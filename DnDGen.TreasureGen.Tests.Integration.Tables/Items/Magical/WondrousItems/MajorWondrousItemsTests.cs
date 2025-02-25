using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.WondrousItems
{
    [TestFixture]
    public class MajorWondrousItemsTests : TypeAndAmountPercentileTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Percentiles.POWERITEMTYPEs(PowerConstants.Major, ItemTypeConstants.WondrousItem); }
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

        [TestCase(WondrousItemConstants.DimensionalShackles, 0, 1)]
        [TestCase(WondrousItemConstants.FigurineOfWondrousPower_ObsidianSteed, 0, 2)]
        [TestCase(WondrousItemConstants.DrumsOfPanic, 0, 3)]
        [TestCase(WondrousItemConstants.IounStone_OrangePrism, 0, 4)]
        [TestCase(WondrousItemConstants.IounStone_PaleGreenPrism, 0, 5)]
        [TestCase(WondrousItemConstants.LanternOfRevealing, 0, 6)]
        [TestCase(WondrousItemConstants.RobeOfBlending, 0, 7)]
        [TestCase(WondrousItemConstants.AmuletOfNaturalArmor, 4, 8)]
        [TestCase(WondrousItemConstants.AmuletOfProofAgainstDetectionAndLocation, 0, 9)]
        [TestCase(WondrousItemConstants.CarpetOfFlying_5x10, 0, 10)]
        [TestCase(WondrousItemConstants.GolemManual_Iron, 0, 11)]
        [TestCase(WondrousItemConstants.AmuletOfHealth, 6, 12)]
        [TestCase(WondrousItemConstants.BeltOfGiantStrength, 6, 13)]
        [TestCase(WondrousItemConstants.BracersOfArmor, 6, 14)]
        [TestCase(WondrousItemConstants.CloakOfCharisma, 6, 15)]
        [TestCase(WondrousItemConstants.GlovesOfDexterity, 6, 16)]
        [TestCase(WondrousItemConstants.HeadbandOfIntellect, 6, 17)]
        [TestCase(WondrousItemConstants.IounStone_VibrantPurplePrism, 0, 18)]
        [TestCase(WondrousItemConstants.PearlOfPower_6thLevel, 0, 19)]
        [TestCase(WondrousItemConstants.PeriaptOfWisdom, 6, 20)]
        [TestCase(WondrousItemConstants.ScarabOfProtection, 0, 21)]
        [TestCase(WondrousItemConstants.IounStone_LavenderAndGreenEllipsoid, 0, 22)]
        [TestCase(WondrousItemConstants.RingGates, 0, 23)]
        [TestCase(WondrousItemConstants.CrystalBall, 0, 24)]
        [TestCase(WondrousItemConstants.GolemManual_Stone_Greater, 0, 25)]
        [TestCase(WondrousItemConstants.OrbOfStorms, 0, 26)]
        [TestCase(WondrousItemConstants.BootsOfTeleportation, 0, 27)]
        [TestCase(WondrousItemConstants.BracersOfArmor, 7, 28)]
        [TestCase(WondrousItemConstants.PearlOfPower_7thLevel, 0, 29)]
        [TestCase(WondrousItemConstants.AmuletOfNaturalArmor, 5, 30)]
        [TestCase(WondrousItemConstants.CloakOfDisplacement_Major, 0, 31)]
        [TestCase(WondrousItemConstants.CrystalBall_SeeInvisibility, 0, 32)]
        [TestCase(WondrousItemConstants.HornOfValhalla, 0, 33)]
        [TestCase(WondrousItemConstants.CrystalBall_DetectThoughts, 0, 34)]
        [TestCase(WondrousItemConstants.CarpetOfFlying_6x9, 0, 35)]
        [TestCase(WondrousItemConstants.AmuletOfMightyFists, 3, 36)]
        [TestCase(WondrousItemConstants.WingsOfFlying, 0, 37)]
        [TestCase(WondrousItemConstants.CloakOfEtherealness, 0, 38)]
        [TestCase(WondrousItemConstants.DaernsInstantFortress, 0, 39)]
        [TestCase(WondrousItemConstants.ManualOfBodilyHealth, 2, 40)]
        [TestCase(WondrousItemConstants.ManualOfGainfulExercise, 2, 41)]
        [TestCase(WondrousItemConstants.ManualOfQuicknessInAction, 2, 42)]
        [TestCase(WondrousItemConstants.TomeOfClearThought, 2, 43)]
        [TestCase(WondrousItemConstants.TomeOfLeadershipAndInfluence, 2, 44)]
        [TestCase(WondrousItemConstants.TomeOfUnderstanding, 2, 45)]
        [TestCase(WondrousItemConstants.EyesOfCharming, 0, 46)]
        [TestCase(WondrousItemConstants.RobeOfStars, 0, 47)]
        [TestCase(WondrousItemConstants.CarpetOfFlying_10x10, 0, 48)]
        [TestCase(WondrousItemConstants.Darkskull, 0, 49)]
        [TestCase(WondrousItemConstants.CubeOfForce, 0, 50)]
        [TestCase(WondrousItemConstants.BracersOfArmor, 8, 51)]
        [TestCase(WondrousItemConstants.PearlOfPower_8thLevel, 0, 52)]
        [TestCase(WondrousItemConstants.CrystalBall_Telepathy, 0, 53)]
        [TestCase(WondrousItemConstants.HornOfBlasting_Greater, 0, 54)]
        [TestCase(WondrousItemConstants.PearlOfPower_TwoSpells, 0, 55)]
        [TestCase(WondrousItemConstants.HelmOfTeleportation, 0, 56)]
        [TestCase(WondrousItemConstants.GemOfSeeing, 0, 57)]
        [TestCase(WondrousItemConstants.RobeOfTheArchmagi, 0, 58)]
        [TestCase(WondrousItemConstants.MantleOfFaith, 0, 59)]
        [TestCase(WondrousItemConstants.CrystalBall_TrueSeeing, 0, 60)]
        [TestCase(WondrousItemConstants.PearlOfPower_9thLevel, 0, 61)]
        [TestCase(WondrousItemConstants.WellOfManyWorlds, 0, 62)]
        [TestCase(WondrousItemConstants.ManualOfBodilyHealth, 3, 63)]
        [TestCase(WondrousItemConstants.ManualOfGainfulExercise, 3, 64)]
        [TestCase(WondrousItemConstants.ManualOfQuicknessInAction, 3, 65)]
        [TestCase(WondrousItemConstants.TomeOfClearThought, 3, 66)]
        [TestCase(WondrousItemConstants.TomeOfLeadershipAndInfluence, 3, 67)]
        [TestCase(WondrousItemConstants.TomeOfUnderstanding, 3, 68)]
        [TestCase(WondrousItemConstants.ApparatusOfKwalish, 0, 69)]
        [TestCase(WondrousItemConstants.MantleOfSpellResistance, 0, 70)]
        [TestCase(WondrousItemConstants.MirrorOfOpposition, 0, 71)]
        [TestCase(WondrousItemConstants.StrandOfPrayerBeads_Greater, 0, 72)]
        [TestCase(WondrousItemConstants.AmuletOfMightyFists, 4, 73)]
        [TestCase(WondrousItemConstants.EyesOfPetrification, 0, 74)]
        [TestCase(WondrousItemConstants.BowlOfCommandingWaterElementals, 0, 75)]
        [TestCase(WondrousItemConstants.BrazierOfCommandingFireElementals, 0, 76)]
        [TestCase(WondrousItemConstants.CenserOfControllingAirElementals, 0, 77)]
        [TestCase(WondrousItemConstants.StoneOfControllingEarthElementals, 0, 78)]
        [TestCase(WondrousItemConstants.ManualOfBodilyHealth, 4, 79)]
        [TestCase(WondrousItemConstants.ManualOfGainfulExercise, 4, 80)]
        [TestCase(WondrousItemConstants.ManualOfQuicknessInAction, 4, 81)]
        [TestCase(WondrousItemConstants.TomeOfClearThought, 4, 82)]
        [TestCase(WondrousItemConstants.TomeOfLeadershipAndInfluence, 4, 83)]
        [TestCase(WondrousItemConstants.TomeOfUnderstanding, 4, 84)]
        [TestCase(WondrousItemConstants.AmuletOfThePlanes, 0, 85)]
        [TestCase(WondrousItemConstants.RobeOfEyes, 0, 86)]
        [TestCase(WondrousItemConstants.HelmOfBrilliance, 0, 87)]
        [TestCase(WondrousItemConstants.ManualOfBodilyHealth, 5, 88)]
        [TestCase(WondrousItemConstants.ManualOfGainfulExercise, 5, 89)]
        [TestCase(WondrousItemConstants.ManualOfQuicknessInAction, 5, 90)]
        [TestCase(WondrousItemConstants.TomeOfClearThought, 5, 91)]
        [TestCase(WondrousItemConstants.TomeOfLeadershipAndInfluence, 5, 92)]
        [TestCase(WondrousItemConstants.TomeOfUnderstanding, 5, 93)]
        [TestCase(WondrousItemConstants.EfreetiBottle, 0, 94)]
        [TestCase(WondrousItemConstants.AmuletOfMightyFists, 5, 95)]
        [TestCase(WondrousItemConstants.ChaosDiamond, 0, 96)]
        [TestCase(WondrousItemConstants.CubicGate, 0, 97)]
        [TestCase(WondrousItemConstants.IronFlask, 0, 98)]
        [TestCase(WondrousItemConstants.MirrorOfMentalProwess, 0, 99)]
        [TestCase(WondrousItemConstants.MirrorOfLifeTrapping, 0, 100)]
        public void TypeAndAmountPercentile(string type, int amount, int roll)
        {
            base.AssertTypeAndAmountPercentile(type, amount, roll);
        }
    }
}