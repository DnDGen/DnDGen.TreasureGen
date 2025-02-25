using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Scrolls.Arcane
{
    [TestFixture]
    public class Level2ArcaneSpellsTests : PercentileTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Percentiles.LevelXSPELLTYPESpells(2, "Arcane"); }
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

        [TestCase("Animal Messenger", 1, 1)]
        [TestCase("Animal Trance", 2, 2)]
        [TestCase("Arcane Lock", 3, 3)]
        [TestCase("Bear's Endurance", 4, 6)]
        [TestCase("Blindness/Deafness", 7, 8)]
        [TestCase("Blur", 9, 10)]
        [TestCase("Bull's Strength", 11, 13)]
        [TestCase("Calm Emotions", 14, 14)]
        [TestCase("Cat's Grace", 15, 17)]
        [TestCase("Command Undead", 18, 19)]
        [TestCase("Continual Flame", 20, 20)]
        [TestCase("Cure Moderate Wounds", 21, 21)]
        [TestCase("Darkness", 22, 22)]
        [TestCase("Darkvision", 23, 25)]
        [TestCase("Daze Monster", 26, 26)]
        [TestCase("Delay Poison", 27, 27)]
        [TestCase("Detect Thoughts", 28, 29)]
        [TestCase("Disguise Self", 30, 31)]
        [TestCase("Eagle's Splendor", 32, 34)]
        [TestCase("Enthrall", 35, 35)]
        [TestCase("False Life", 36, 37)]
        [TestCase("Flaming Sphere", 38, 39)]
        [TestCase("Fog Cloud", 40, 40)]
        [TestCase("Fox's Cunning", 41, 43)]
        [TestCase("Ghoul Touch", 44, 44)]
        [TestCase("Glitterdust", 45, 46)]
        [TestCase("Gust of Wind", 47, 47)]
        [TestCase("Hypnotic Pattern", 48, 49)]
        [TestCase("Invisibility", 50, 52)]
        [TestCase("Knock", 53, 55)]
        [TestCase("Leomund's Trap", 56, 56)]
        [TestCase("Levitate", 57, 58)]
        [TestCase("Locate Object", 59, 59)]
        [TestCase("Magic Mouth", 60, 60)]
        [TestCase("Melf's Acid Arrow", 61, 62)]
        [TestCase("Minor Image", 63, 63)]
        [TestCase("Mirror Image", 64, 65)]
        [TestCase("Misdirection", 66, 66)]
        [TestCase("Obscure Object", 67, 67)]
        [TestCase("Owl's Wisdom", 68, 70)]
        [TestCase("Protection from Arrows", 71, 73)]
        [TestCase("Pyrotechnics", 74, 75)]
        [TestCase("Resist Energy", 76, 78)]
        [TestCase("Rope Trick", 79, 79)]
        [TestCase("Scare", 80, 80)]
        [TestCase("Scorching Ray", 81, 82)]
        [TestCase("See Invisibility", 83, 85)]
        [TestCase("Shatter", 86, 86)]
        [TestCase("Silence", 87, 87)]
        [TestCase("Sound Burst", 88, 88)]
        [TestCase("Spectral Hand", 89, 89)]
        [TestCase("Spider Climb", 90, 91)]
        [TestCase("Summon Monster II", 92, 93)]
        [TestCase("Summon Swarm", 94, 95)]
        [TestCase("Tasha's Hideous Laughter", 96, 96)]
        [TestCase("Touch of Idiocy", 97, 97)]
        [TestCase("Web", 98, 99)]
        [TestCase("Whispering Wind", 100, 100)]
        public void Level2ArcaneSpellsPercentile(string content, int lower, int upper)
        {
            AssertPercentile(content, lower, upper);
        }
    }
}