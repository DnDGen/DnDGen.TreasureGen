using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Scrolls.Arcane
{
    [TestFixture]
    public class Level6ArcaneSpellsTests : PercentileTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Percentiles.LevelXSPELLTYPESpells(6, "Arcane"); }
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

        [TestCase("Acid Fog", 1, 2)]
        [TestCase("Analyze Dweomer", 3, 5)]
        [TestCase("Antimagic Field", 7, 9)]
        [TestCase("Mass Bear's Endurance", 10, 12)]
        [TestCase("Bigby's Forceful Hand", 13, 14)]
        [TestCase("Mass Bull's Strength", 15, 17)]
        [TestCase("Mass Cat's Grace", 18, 20)]
        [TestCase("Chain Lightning", 21, 23)]
        [TestCase("Circle of Death", 24, 25)]
        [TestCase("Control Water", 27, 28)]
        [TestCase("Disintegrate", 31, 33)]
        [TestCase("Greater Dispel Magic", 34, 37)]
        [TestCase("Mass Eagle's Splendor", 38, 40)]
        [TestCase("Eyebite", 41, 42)]
        [TestCase("Flesh to Stone", 44, 45)]
        [TestCase("Mass Fox's Cunning", 46, 48)]
        [TestCase("Globe of Invulnerability", 50, 52)]
        [TestCase("Greater Heroism", 55, 56)]
        [TestCase("Mislead", 58, 59)]
        [TestCase("Move Earth", 61, 62)]
        [TestCase("Otiluke's Freezing Sphere", 63, 64)]
        [TestCase("Mass Owl's Wisdom", 65, 67)]
        [TestCase("Permanent Image", 68, 69)]
        [TestCase("Planar Binding", 70, 71)]
        [TestCase("Programmed Image", 72, 73)]
        [TestCase("Repulsion", 74, 75)]
        [TestCase("Shadow Walk", 76, 78)]
        [TestCase("Stone to Flesh", 79, 81)]
        [TestCase("Mass Suggestion", 82, 83)]
        [TestCase("Summon Monster VI", 84, 85)]
        [TestCase("Tenser's Transformation", 89, 90)]
        [TestCase("True Seeing", 91, 93)]
        [TestCase("Undeath to Death", 94, 95)]
        [TestCase("Veil", 96, 97)]
        [TestCase("Wall of Iron", 98, 100)]
        public void Level6ArcaneSpellsPercentile(string content, int lower, int upper)
        {
            AssertPercentile(content, lower, upper);
        }

        [TestCase("Animate Objects", 6)]
        [TestCase("Contingency", 26)]
        [TestCase("Create Undead", 29)]
        [TestCase("Mass Cure Moderate Wounds", 30)]
        [TestCase("Find the Path", 43)]
        [TestCase("Geas/Quest", 49)]
        [TestCase("Guards and Wards", 53)]
        [TestCase("Heroes' Feast", 54)]
        [TestCase("Legend Lore", 57)]
        [TestCase("Mordenkainen's Lucubration", 60)]
        [TestCase("Symbol of Fear", 86)]
        [TestCase("Symbol of Persuasion", 87)]
        [TestCase("Sympathetic Vibration", 88)]
        public void Level6ArcaneSpellsPercentile(string content, int roll)
        {
            AssertPercentile(content, roll);
        }
    }
}