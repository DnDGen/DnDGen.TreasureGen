using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Scrolls.Divine
{
    [TestFixture]
    public class Level2DivineSpellsTests : PercentileTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Percentiles.LevelXSPELLTYPESpells(2, "Divine"); }
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

        [TestCase("Animal Messenger", 1)]
        [TestCase("Animal Trance", 2)]
        [TestCase("Chill Metal", 18)]
        [TestCase("Death Knell", 27)]
        [TestCase("Fire Trap", 40)]
        [TestCase("Gentle Repose", 47)]
        [TestCase("Gust of Wind", 48)]
        [TestCase("Heat Metal", 49)]
        [TestCase("Reduce Animal", 62)]
        [TestCase("Snare", 77)]
        [TestCase("Soften Earth and Stone", 78)]
        [TestCase("Speak with Plants", 81)]
        [TestCase("Status", 86)]
        [TestCase("Tree Shape", 93)]
        [TestCase("Wood Shape", 98)]
        public void Level2DivineSpellsPercentile(string content, int roll)
        {
            AssertPercentile(content, roll);
        }

        [TestCase("Augury", 3, 4)]
        [TestCase("Barkskin", 5, 6)]
        [TestCase("Bear's Endurance", 7, 9)]
        [TestCase("Bull's Strength", 10, 12)]
        [TestCase("Calm Emotions", 13, 14)]
        [TestCase("Cat's Grace", 15, 17)]
        [TestCase("Consecrate", 19, 20)]
        [TestCase("Cure Moderate Wounds", 21, 24)]
        [TestCase("Darkness", 25, 26)]
        [TestCase("Delay Poison", 28, 30)]
        [TestCase("Desecrate", 31, 32)]
        [TestCase("Eagle's Splendor", 33, 35)]
        [TestCase("Enthrall", 36, 37)]
        [TestCase("Find Traps", 38, 39)]
        [TestCase("Flame Blade", 41, 42)]
        [TestCase("Flaming Sphere", 43, 44)]
        [TestCase("Fog Cloud", 45, 46)]
        [TestCase("Hold Animal", 50, 51)]
        [TestCase("Hold Person", 52, 54)]
        [TestCase("Inflict Moderate Wounds", 55, 56)]
        [TestCase("Make Whole", 57, 58)]
        [TestCase("Owl's Wisdom", 59, 61)]
        [TestCase("Remove Paralysis", 63, 64)]
        [TestCase("Resist Energy", 65, 67)]
        [TestCase("Lesser Restoration", 68, 70)]
        [TestCase("Shatter", 71, 72)]
        [TestCase("Shield Other", 73, 74)]
        [TestCase("Silence", 75, 76)]
        [TestCase("Sound Burst", 79, 80)]
        [TestCase("Spider Climb", 82, 83)]
        [TestCase("Spiritual Weapon", 84, 85)]
        [TestCase("Summon Monster II", 87, 88)]
        [TestCase("Summon Nature's Ally II", 89, 90)]
        [TestCase("Summon Swarm", 91, 92)]
        [TestCase("Undetectable Alignment", 94, 95)]
        [TestCase("Warp Wood", 96, 97)]
        [TestCase("Zone of Truth", 99, 100)]
        public void Level2DivineSpellsPercentile(string content, int lower, int upper)
        {
            AssertPercentile(content, lower, upper);
        }
    }
}