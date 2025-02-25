using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Scrolls.Arcane
{
    [TestFixture]
    public class Level3ArcaneSpellsTests : PercentileTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Percentiles.LevelXSPELLTYPESpells(3, "Arcane"); }
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

        [TestCase("Arcane Sight", 1, 2)]
        [TestCase("Blink", 3, 4)]
        [TestCase("Clairaudience/Clairvoyance", 5, 6)]
        [TestCase("Daylight", 8, 10)]
        [TestCase("Deep Slumber", 11, 12)]
        [TestCase("Dispel Magic", 13, 15)]
        [TestCase("Displacement", 16, 17)]
        [TestCase("Fireball", 19, 20)]
        [TestCase("Flame Arrow", 21, 22)]
        [TestCase("Fly", 23, 25)]
        [TestCase("Gaseous Form", 26, 27)]
        [TestCase("Gentle Repose", 28, 29)]
        [TestCase("Halt Undead", 32, 33)]
        [TestCase("Haste", 34, 36)]
        [TestCase("Heroism", 37, 38)]
        [TestCase("Hold Person", 39, 40)]
        [TestCase("Invisibility Sphere", 42, 44)]
        [TestCase("Keen Edge", 45, 47)]
        [TestCase("Leomund's Tiny Hut", 48, 49)]
        [TestCase("Lightning Bolt", 50, 51)]
        [TestCase("Magic Circle Against Chaos/Evil/Good/Law", 52, 59)]
        [TestCase("Greater Magic Weapon", 60, 62)]
        [TestCase("Major Image", 63, 64)]
        [TestCase("Nondetection", 65, 66)]
        [TestCase("Phantom Steed", 67, 68)]
        [TestCase("Protection from Energy", 69, 71)]
        [TestCase("Rage", 72, 73)]
        [TestCase("Ray of Exhaustion", 74, 75)]
        [TestCase("Sleet Storm", 80, 81)]
        [TestCase("Slow", 82, 83)]
        [TestCase("Stinking Cloud", 85, 86)]
        [TestCase("Suggestion", 87, 88)]
        [TestCase("Summon Monster III", 89, 90)]
        [TestCase("Tongues", 91, 93)]
        [TestCase("Vampiric Touch", 94, 95)]
        [TestCase("Water Breathing", 96, 98)]
        [TestCase("Wind Wall", 99, 100)]
        public void Level3ArcaneSpellsPercentile(string content, int lower, int upper)
        {
            AssertPercentile(content, lower, upper);
        }

        [TestCase("Cure Serious Wounds", 7)]
        [TestCase("Explosive Runes", 18)]
        [TestCase("Glibness", 30)]
        [TestCase("Good Hope", 31)]
        [TestCase("Illusory Script", 41)]
        [TestCase("Sculpt Sound", 76)]
        [TestCase("Secret Page", 77)]
        [TestCase("Sepia Snake Sigil", 78)]
        [TestCase("Shrink Item", 79)]
        [TestCase("Speak with Animals", 84)]
        public void Level3ArcaneSpellsPercentile(string content, int roll)
        {
            AssertPercentile(content, roll);
        }
    }
}