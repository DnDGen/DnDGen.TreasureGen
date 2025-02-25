using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Scrolls.Arcane
{
    [TestFixture]
    public class Level8ArcaneSpellsTests : PercentileTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Percentiles.LevelXSPELLTYPESpells(8, "Arcane"); }
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

        [TestCase("Antipathy", 1, 2)]
        [TestCase("Bigby's Clenched Fist", 3, 5)]
        [TestCase("Binding", 6, 8)]
        [TestCase("Mass Charm Monster", 9, 12)]
        [TestCase("Create Greater Undead", 14, 16)]
        [TestCase("Demand", 17, 19)]
        [TestCase("Dimensional Lock", 20, 22)]
        [TestCase("Discern Location", 23, 26)]
        [TestCase("Horrid Wilting", 27, 29)]
        [TestCase("Incendiary Cloud", 30, 32)]
        [TestCase("Iron Body", 33, 35)]
        [TestCase("Maze", 36, 38)]
        [TestCase("Mind Blank", 39, 41)]
        [TestCase("Moment of Prescience", 42, 44)]
        [TestCase("Otiluke's Telekinetic Sphere", 45, 48)]
        [TestCase("Otto's Irresistible Dance", 49, 51)]
        [TestCase("Greater Planar Binding", 52, 54)]
        [TestCase("Polar Ray", 55, 57)]
        [TestCase("Polymorph Any Object", 58, 60)]
        [TestCase("Power Word Stun", 61, 63)]
        [TestCase("Prismatic Wall", 64, 66)]
        [TestCase("Protection from Spells", 67, 70)]
        [TestCase("Greater Prying Eyes", 71, 73)]
        [TestCase("Scintillating Pattern", 74, 76)]
        [TestCase("Screen", 77, 78)]
        [TestCase("Greater Shadow Evocation", 79, 81)]
        [TestCase("Greater Shout", 82, 84)]
        [TestCase("Summon Monster VIII", 85, 87)]
        [TestCase("Sunburst", 88, 90)]
        [TestCase("Sympathy", 93, 94)]
        [TestCase("Temporal Stasis", 95, 98)]
        [TestCase("Trap the Soul", 99, 100)]
        public void Level8ArcaneSpellsPercentile(string content, int lower, int upper)
        {
            AssertPercentile(content, lower, upper);
        }

        [TestCase("Clone", 13)]
        [TestCase("Symbol of Death", 91)]
        [TestCase("Symbol of Insanity", 92)]
        public void Level8ArcaneSpellsPercentile(string content, int roll)
        {
            AssertPercentile(content, roll);
        }
    }
}