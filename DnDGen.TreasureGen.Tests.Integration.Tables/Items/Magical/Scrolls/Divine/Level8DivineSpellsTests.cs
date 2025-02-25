using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Scrolls.Divine
{
    [TestFixture]
    public class Level8DivineSpellsTests : PercentileTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Percentiles.LevelXSPELLTYPESpells(8, "Divine"); }
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

        [TestCase("Animal Shapes", 1, 4)]
        [TestCase("Antimagic Field", 5, 10)]
        [TestCase("Cloak of Chaos", 11, 13)]
        [TestCase("Control Plants", 14, 17)]
        [TestCase("Create Greater Undead", 18, 20)]
        [TestCase("Mass Cure Critical Wounds", 21, 27)]
        [TestCase("Dimensional Lock", 28, 32)]
        [TestCase("Discern Location", 33, 36)]
        [TestCase("Earthquake", 37, 41)]
        [TestCase("Finger of Death", 42, 45)]
        [TestCase("Firestorm", 46, 49)]
        [TestCase("Holy Aura", 50, 52)]
        [TestCase("Mass Inflict Critical Wounds", 53, 56)]
        [TestCase("Greater Planar Ally", 57, 60)]
        [TestCase("Repel Metal or Stone", 61, 65)]
        [TestCase("Reverse Gravity", 66, 69)]
        [TestCase("Shield of Law", 70, 72)]
        [TestCase("Greater Spell Immunity", 73, 76)]
        [TestCase("Summon Monster VIII", 77, 80)]
        [TestCase("Summon Nature's Ally VIII", 81, 84)]
        [TestCase("Sunburst", 85, 89)]
        [TestCase("Symbol of Death", 90, 91)]
        [TestCase("Symbol of Insanity", 92, 93)]
        [TestCase("Unholy Aura", 94, 96)]
        [TestCase("Whirlwind", 97, 100)]
        public void Level8DivineSpellsPercentile(string content, int lower, int upper)
        {
            AssertPercentile(content, lower, upper);
        }
    }
}