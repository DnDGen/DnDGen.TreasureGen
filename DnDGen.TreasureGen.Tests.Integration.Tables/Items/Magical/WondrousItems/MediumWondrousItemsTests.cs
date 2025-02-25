using NUnit.Framework;
using DnDGen.TreasureGen.Tables;
using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.WondrousItems
{
    [TestFixture]
    public class MediumWondrousItemsTests : TypeAndAmountPercentileTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Percentiles.POWERITEMTYPEs(PowerConstants.Medium, ItemTypeConstants.WondrousItem); }
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

        [TestCase(WondrousItemConstants.BootsOfLevitation, 0, 1)]
        [TestCase(WondrousItemConstants.HarpOfCharming, 0, 2)]
        [TestCase(WondrousItemConstants.AmuletOfNaturalArmor, 2, 3)]
        [TestCase(WondrousItemConstants.GolemManual_Flesh, 0, 4)]
        [TestCase(WondrousItemConstants.HandOfGlory, 0, 5)]
        [TestCase(WondrousItemConstants.IounStone_DeepRedSphere, 0, 6)]
        [TestCase(WondrousItemConstants.IounStone_IncandescentBlueSphere, 0, 7)]
        [TestCase(WondrousItemConstants.IounStone_PaleBlueRhomboid, 0, 8)]
        [TestCase(WondrousItemConstants.IounStone_PinkAndGreenSphere, 0, 9)]
        [TestCase(WondrousItemConstants.IounStone_PinkRhomboid, 0, 10)]
        [TestCase(WondrousItemConstants.IounStone_ScarletAndBlueSphere, 0, 11)]
        [TestCase(WondrousItemConstants.DeckOfIllusions, 0, 12)]
        [TestCase(WondrousItemConstants.NecklaceOfFireballs_VI, 0, 13)]
        [TestCase(WondrousItemConstants.CandleOfInvocation, 0, 14)]
        [TestCase(WondrousItemConstants.BracersOfArmor, 3, 15)]
        [TestCase(WondrousItemConstants.CloakOfResistance, 3, 16)]
        [TestCase(WondrousItemConstants.DecanterOfEndlessWater, 0, 17)]
        [TestCase(WondrousItemConstants.NecklaceOfAdaptation, 0, 18)]
        [TestCase(WondrousItemConstants.PearlOfPower_3rdLevel, 0, 19)]
        [TestCase(WondrousItemConstants.TalismanOfTheSphere, 0, 20)]
        [TestCase(WondrousItemConstants.FigurineOfWondrousPower_SerpentineOwl, 0, 21)]
        [TestCase(WondrousItemConstants.NecklaceOfFireballs_VII, 0, 22)]
        [TestCase(WondrousItemConstants.StrandOfPrayerBeads_Lesser, 0, 23)]
        [TestCase(WondrousItemConstants.BagOfHolding_IV, 0, 24)]
        [TestCase(WondrousItemConstants.FigurineOfWondrousPower_BronzeGriffon, 0, 25)]
        [TestCase(WondrousItemConstants.FigurineOfWondrousPower_EbonyFly, 0, 26)]
        [TestCase(WondrousItemConstants.GloveOfStoring, 0, 27)]
        [TestCase(WondrousItemConstants.IounStone_DarkBlueRhomboid, 0, 28)]
        [TestCase(WondrousItemConstants.StoneHorse_Courser, 0, 29)]
        [TestCase(WondrousItemConstants.CapeOfTheMountebank, 0, 30)]
        [TestCase(WondrousItemConstants.PhylacteryOfUndeadTurning, 0, 31)]
        [TestCase(WondrousItemConstants.GauntletOfRust, 0, 32)]
        [TestCase(WondrousItemConstants.BootsOfSpeed, 0, 33)]
        [TestCase(WondrousItemConstants.GogglesOfNight, 0, 34)]
        [TestCase(WondrousItemConstants.GolemManual_Clay, 0, 35)]
        [TestCase(WondrousItemConstants.MedallionOfThoughts, 0, 36)]
        [TestCase(WondrousItemConstants.PipesOfPain, 0, 37)]
        [TestCase(WondrousItemConstants.BoccobsBlessedBook, 0, 38)]
        [TestCase(WondrousItemConstants.MonksBelt, 0, 39)]
        [TestCase(WondrousItemConstants.GemOfBrightness, 0, 40)]
        [TestCase(WondrousItemConstants.LyreOfBuilding, 0, 41)]
        [TestCase(WondrousItemConstants.CloakOfArachnida, 0, 42)]
        [TestCase(WondrousItemConstants.StoneHorse_Destrier, 0, 43)]
        [TestCase(WondrousItemConstants.BeltOfDwarvenkind, 0, 44)]
        [TestCase(WondrousItemConstants.PeriaptOfWoundClosure, 0, 45)]
        [TestCase(WondrousItemConstants.HornOfTheTritons, 0, 46)]
        [TestCase(WondrousItemConstants.PearlOfTheSirines, 0, 47)]
        [TestCase(WondrousItemConstants.FigurineOfWondrousPower_OnyxDog, 0, 48)]
        [TestCase(WondrousItemConstants.AmuletOfHealth, 4, 49)]
        [TestCase(WondrousItemConstants.BeltOfGiantStrength, 4, 50)]
        [TestCase(WondrousItemConstants.WingedBoots, 0, 51)]
        [TestCase(WondrousItemConstants.BracersOfArmor, 4, 52)]
        [TestCase(WondrousItemConstants.CloakOfCharisma, 4, 53)]
        [TestCase(WondrousItemConstants.CloakOfResistance, 4, 54)]
        [TestCase(WondrousItemConstants.GlovesOfDexterity, 4, 55)]
        [TestCase(WondrousItemConstants.HeadbandOfIntellect, 4, 56)]
        [TestCase(WondrousItemConstants.PearlOfPower_4thLevel, 0, 57)]
        [TestCase(WondrousItemConstants.PeriaptOfWisdom, 4, 58)]
        [TestCase(WondrousItemConstants.ScabbardOfKeenEdges, 0, 59)]
        [TestCase(WondrousItemConstants.FigurineOfWondrousPower_GoldenLions, 0, 60)]
        [TestCase(WondrousItemConstants.ChimeOfInterruption, 0, 61)]
        [TestCase(WondrousItemConstants.BroomOfFlying, 0, 62)]
        [TestCase(WondrousItemConstants.FigurineOfWondrousPower_MarbleElephant, 0, 63)]
        [TestCase(WondrousItemConstants.AmuletOfNaturalArmor, 3, 64)]
        [TestCase(WondrousItemConstants.IounStone_IridescentSpindle, 0, 65)]
        [TestCase(WondrousItemConstants.BraceletOfFriends, 0, 66)]
        [TestCase(WondrousItemConstants.CarpetOfFlying_5x5, 0, 67)]
        [TestCase(WondrousItemConstants.HornOfBlasting, 0, 68)]
        [TestCase(WondrousItemConstants.IounStone_PaleLavenderEllipsoid, 0, 69)]
        [TestCase(WondrousItemConstants.IounStone_PearlyWhiteSpindle, 0, 70)]
        [TestCase(WondrousItemConstants.PortableHole, 0, 71)]
        [TestCase(WondrousItemConstants.StoneOfGoodLuck_Luckstone, 0, 72)]
        [TestCase(WondrousItemConstants.FigurineOfWondrousPower_IvoryGoats, 0, 73)]
        [TestCase(WondrousItemConstants.RopeOfEntanglement, 0, 74)]
        [TestCase(WondrousItemConstants.GolemManual_Stone, 0, 75)]
        [TestCase(WondrousItemConstants.MaskOfTheSkull, 0, 76)]
        [TestCase(WondrousItemConstants.MattockOfTheTitans, 0, 77)]
        [TestCase(WondrousItemConstants.CircletOfBlasting_Major, 0, 78)]
        [TestCase(WondrousItemConstants.AmuletOfMightyFists, 2, 79)]
        [TestCase(WondrousItemConstants.CloakOfDisplacement_Minor, 0, 80)]
        [TestCase(WondrousItemConstants.HelmOfUnderwaterAction, 0, 81)]
        [TestCase(WondrousItemConstants.BracersOfArchery_Greater, 0, 82)]
        [TestCase(WondrousItemConstants.BracersOfArmor, 5, 83)]
        [TestCase(WondrousItemConstants.CloakOfResistance, 5, 84)]
        [TestCase(WondrousItemConstants.EyesOfDoom, 0, 85)]
        [TestCase(WondrousItemConstants.PearlOfPower_5thLevel, 0, 86)]
        [TestCase(WondrousItemConstants.MaulOfTheTitans, 0, 87)]
        [TestCase(WondrousItemConstants.StrandOfPrayerBeads, 0, 88)]
        [TestCase(WondrousItemConstants.CloakOfTheBat, 0, 89)]
        [TestCase(WondrousItemConstants.IronBandsOfBilarro, 0, 90)]
        [TestCase(WondrousItemConstants.CubeOfFrostResistance, 0, 91)]
        [TestCase(WondrousItemConstants.HelmOfTelepathy, 0, 92)]
        [TestCase(WondrousItemConstants.PeriaptOfProofAgainstPoison, 0, 93)]
        [TestCase(WondrousItemConstants.RobeOfScintillatingColors, 0, 94)]
        [TestCase(WondrousItemConstants.ManualOfBodilyHealth, 1, 95)]
        [TestCase(WondrousItemConstants.ManualOfGainfulExercise, 1, 96)]
        [TestCase(WondrousItemConstants.ManualOfQuicknessInAction, 1, 97)]
        [TestCase(WondrousItemConstants.TomeOfClearThought, 1, 98)]
        [TestCase(WondrousItemConstants.TomeOfLeadershipAndInfluence, 1, 99)]
        [TestCase(WondrousItemConstants.TomeOfUnderstanding, 1, 100)]
        public void TypeAndAmountPercentile(string type, int amount, int roll)
        {
            base.AssertTypeAndAmountPercentile(type, amount, roll);
        }
    }
}