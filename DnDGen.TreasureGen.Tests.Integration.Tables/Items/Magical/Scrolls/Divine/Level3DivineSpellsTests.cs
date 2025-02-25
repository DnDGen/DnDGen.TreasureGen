using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Scrolls.Divine
{
    [TestFixture]
    public class Level3DivineSpellsTests : PercentileTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Percentiles.LevelXSPELLTYPESpells(3, "Divine"); }
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

        [TestCase("Animate Dead", 1, 2)]
        [TestCase("Bestow Curse", 3, 4)]
        [TestCase("Blindness/Deafness", 5, 6)]
        [TestCase("Call Lightning", 7, 8)]
        [TestCase("Contagion", 9, 10)]
        [TestCase("Continual Flame", 11, 12)]
        [TestCase("Create Food and Water", 13, 14)]
        [TestCase("Cure Serious Wounds", 15, 18)]
        [TestCase("Daylight", 20, 21)]
        [TestCase("Deeper Darkness", 22, 23)]
        [TestCase("Diminish Plants", 24, 25)]
        [TestCase("Dispel Magic", 26, 27)]
        [TestCase("Dominate Animal", 28, 29)]
        [TestCase("Glyph of Warding", 30, 31)]
        [TestCase("Helping Hand", 33, 34)]
        [TestCase("Inflict Serious Wounds", 35, 36)]
        [TestCase("Invisibility Purge", 37, 38)]
        [TestCase("Locate Object", 39, 40)]
        [TestCase("Magic Circle Against Chaos/Evil/Good/Law", 41, 46)]
        [TestCase("Greater Magic Fang", 47, 48)]
        [TestCase("Magic Vestment", 49, 50)]
        [TestCase("Meld into Stone", 51, 52)]
        [TestCase("Neutralize Poison", 53, 55)]
        [TestCase("Obscure Object", 56, 57)]
        [TestCase("Plant Growth", 58, 59)]
        [TestCase("Prayer", 60, 62)]
        [TestCase("Protection from Energy", 63, 64)]
        [TestCase("Quench", 65, 66)]
        [TestCase("Remove Blindness/Deafness", 67, 69)]
        [TestCase("Remove Curse", 70, 71)]
        [TestCase("Remove Disease", 72, 73)]
        [TestCase("Searing Light", 74, 76)]
        [TestCase("Sleet Storm", 77, 78)]
        [TestCase("Snare", 79, 80)]
        [TestCase("Speak with Dead", 81, 83)]
        [TestCase("Speak with Plants", 84, 85)]
        [TestCase("Spike Growth", 86, 87)]
        [TestCase("Stone Shape", 88, 89)]
        [TestCase("Summon Monster III", 90, 91)]
        [TestCase("Summon Nature's Ally III", 92, 93)]
        [TestCase("Water Breathing", 94, 96)]
        [TestCase("Water Walk", 97, 98)]
        [TestCase("Wind Wall", 99, 100)]
        public void Level3DivineSpellsPercentile(string content, int lower, int upper)
        {
            AssertPercentile(content, lower, upper);
        }

        [TestCase("Darkvision", 19)]
        [TestCase("Heal Mount", 32)]
        public void Level3DivineSpellsPercentile(string content, int roll)
        {
            AssertPercentile(content, roll);
        }
    }
}