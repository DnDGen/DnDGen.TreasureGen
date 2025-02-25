using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Scrolls.Arcane
{
    [TestFixture]
    public class Level7ArcaneSpellsTests : PercentileTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Percentiles.LevelXSPELLTYPESpells(7, "Arcane"); }
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

        [TestCase("Greater Arcane Sight", 1, 3)]
        [TestCase("Banishment", 4, 7)]
        [TestCase("Bigby's Grasping Hand", 8, 10)]
        [TestCase("Control Undead", 11, 13)]
        [TestCase("Control Weather", 14, 16)]
        [TestCase("Delayed Blast Fireball", 17, 19)]
        [TestCase("Drawmij's Instant Summons", 20, 21)]
        [TestCase("Ethereal Jaunt", 22, 25)]
        [TestCase("Finger of Death", 26, 28)]
        [TestCase("Forcecage", 29, 31)]
        [TestCase("Mass Hold Person", 32, 35)]
        [TestCase("Insanity", 36, 38)]
        [TestCase("Mass Invisibility", 39, 42)]
        [TestCase("Mordenkainen's Magnificent Mansion", 44, 45)]
        [TestCase("Mordenkainen's Sword", 46, 48)]
        [TestCase("Phase Door", 49, 51)]
        [TestCase("Plane Shift", 52, 54)]
        [TestCase("Power Word Blind", 55, 57)]
        [TestCase("Prismatic Spray", 58, 61)]
        [TestCase("Project Image", 62, 64)]
        [TestCase("Reverse Gravity", 65, 67)]
        [TestCase("Greater Scrying", 68, 70)]
        [TestCase("Sequester", 71, 73)]
        [TestCase("Greater Shadow Conjuration", 74, 76)]
        [TestCase("Spell Turning", 78, 80)]
        [TestCase("Statue", 81, 82)]
        [TestCase("Summon Monster VII", 83, 85)]
        [TestCase("Teleport Object", 88, 90)]
        [TestCase("Greater Teleport", 91, 95)]
        [TestCase("Vision", 96, 97)]
        [TestCase("Waves of Exhaustion", 98, 100)]
        public void Level7ArcaneSpellsPercentile(string content, int lower, int upper)
        {
            AssertPercentile(content, lower, upper);
        }

        [TestCase("Limited Wish", 43)]
        [TestCase("Simulacrum", 77)]
        [TestCase("Symbol of Stunning", 86)]
        [TestCase("Symbol of Weakness", 87)]
        public void Level7ArcaneSpellsPercentile(string content, int roll)
        {
            AssertPercentile(content, roll);
        }
    }
}