using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Curses
{
    [TestFixture]
    public class SpecificCursedItemsTests : PercentileTests
    {
        protected override string tableName => TableNameConstants.Percentiles.SpecificCursedItems;

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

        [TestCase(WondrousItemConstants.IncenseOfObsession, 1, 5)]
        [TestCase(RingConstants.Clumsiness, 6, 15)]
        [TestCase(WondrousItemConstants.AmuletOfInescapableLocation, 16, 20)]
        [TestCase(WondrousItemConstants.StoneOfWeight_Loadstone, 21, 25)]
        [TestCase(WondrousItemConstants.BracersOfDefenselessness, 26, 30)]
        [TestCase(WondrousItemConstants.GauntletsOfFumbling, 31, 35)]
        [TestCase(WeaponConstants.CursedMinus2Sword, 36, 40)]
        [TestCase(ArmorConstants.ArmorOfRage, 41, 43)]
        [TestCase(WondrousItemConstants.MedallionOfThoughtProjection, 44, 46)]
        [TestCase(WondrousItemConstants.FlaskOfCurses, 47, 52)]
        [TestCase(WondrousItemConstants.DustOfSneezingAndChoking, 53, 54)]
        [TestCase(WondrousItemConstants.HelmOfOppositeAlignment, 55, 55)]
        [TestCase(PotionConstants.Poison, 56, 60)]
        [TestCase(WondrousItemConstants.BroomOfAnimatedAttack, 61, 61)]
        [TestCase(WondrousItemConstants.RobeOfPowerlessness, 62, 63)]
        [TestCase(WondrousItemConstants.VacousGrimoire, 64, 64)]
        [TestCase(WeaponConstants.CursedBackbiterSpear, 65, 68)]
        [TestCase(ArmorConstants.ArmorOfArrowAttraction, 69, 70)]
        [TestCase(WeaponConstants.NetOfSnaring, 71, 72)]
        [TestCase(WondrousItemConstants.BagOfDevouring, 73, 75)]
        [TestCase(WeaponConstants.MaceOfBlood, 76, 80)]
        [TestCase(WondrousItemConstants.RobeOfVermin, 81, 85)]
        [TestCase(WondrousItemConstants.PeriaptOfFoulRotting, 86, 88)]
        [TestCase(WeaponConstants.BerserkingSword, 89, 92)]
        [TestCase(WondrousItemConstants.BootsOfDancing, 93, 96)]
        [TestCase(WondrousItemConstants.CrystalBall_Hypnosis, 97, 97)]
        [TestCase(WondrousItemConstants.NecklaceOfStrangulation, 98, 98)]
        [TestCase(WondrousItemConstants.CloakOfPoisonousness, 99, 99)]
        [TestCase(WondrousItemConstants.ScarabOfDeath, 100, 100)]
        public void SpecificCursedItemsPercentile(string content, int lower, int upper)
        {
            AssertPercentile(content, lower, upper);
        }
    }
}