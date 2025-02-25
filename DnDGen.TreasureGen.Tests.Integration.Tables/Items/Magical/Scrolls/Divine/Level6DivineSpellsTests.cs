using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Scrolls.Divine
{
    [TestFixture]
    public class Level6DivineSpellsTests : PercentileTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Percentiles.LevelXSPELLTYPESpells(6, "Divine"); }
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

        [TestCase("Animate Objects", 1, 3)]
        [TestCase("Antilife Shell", 4, 6)]
        [TestCase("Banishment", 7, 9)]
        [TestCase("Mass Bear's Endurance", 10, 13)]
        [TestCase("Blade Barrier", 14, 16)]
        [TestCase("Mass Bull's Strength", 17, 20)]
        [TestCase("Mass Cat's Grace", 21, 24)]
        [TestCase("Mass Cure Moderate Wounds", 26, 29)]
        [TestCase("Greater Dispel Magic", 30, 33)]
        [TestCase("Mass Eagle's Splendor", 34, 37)]
        [TestCase("Find the Path", 38, 40)]
        [TestCase("Fire Seeds", 41, 43)]
        [TestCase("Harm", 47, 49)]
        [TestCase("Heal", 50, 52)]
        [TestCase("Heroes' Feast", 53, 55)]
        [TestCase("Mass Inflict Moderate Wounds", 56, 58)]
        [TestCase("Ironwood", 59, 61)]
        [TestCase("Move Earth", 63, 65)]
        [TestCase("Mass Owl's Wisdom", 66, 69)]
        [TestCase("Planar Ally", 70, 71)]
        [TestCase("Repel Wood", 72, 74)]
        [TestCase("Spellstaff", 75, 77)]
        [TestCase("Stone Tell", 78, 80)]
        [TestCase("Summon Monster VI", 81, 83)]
        [TestCase("Summon Nature's Ally VI", 84, 86)]
        [TestCase("Transport via Plants", 89, 91)]
        [TestCase("Undeath to Death", 92, 94)]
        [TestCase("Windwalk", 95, 97)]
        [TestCase("Word of Recall", 98, 100)]
        public void Level6DivineSpellsPercentile(string content, int lower, int upper)
        {
            AssertPercentile(content, lower, upper);
        }

        [TestCase("Create Undead", 25)]
        [TestCase("Forbiddance", 44)]
        [TestCase("Geas/Quest", 45)]
        [TestCase("Greater Glyph of Warding", 46)]
        [TestCase("Liveoak", 62)]
        [TestCase("Symbol of Fear", 87)]
        [TestCase("Symbol of Persuasion", 88)]
        public void Level6DivineSpellsPercentile(string content, int roll)
        {
            AssertPercentile(content, roll);
        }
    }
}