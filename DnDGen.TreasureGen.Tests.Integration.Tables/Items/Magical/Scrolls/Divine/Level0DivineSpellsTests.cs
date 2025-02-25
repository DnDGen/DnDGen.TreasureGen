using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Scrolls.Divine
{
    [TestFixture]
    public class Level0DivineSpellsTests : PercentileTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Percentiles.LevelXSPELLTYPESpells(0, "Divine"); }
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

        [TestCase("Create Water", 1, 7)]
        [TestCase("Cure Minor Wounds", 8, 14)]
        [TestCase("Detect Magic", 15, 22)]
        [TestCase("Detect Poison", 23, 29)]
        [TestCase("Flare", 30, 36)]
        [TestCase("Guidance", 37, 43)]
        [TestCase("Inflict Minor Wounds", 44, 50)]
        [TestCase("Know Direction", 51, 57)]
        [TestCase("Light", 58, 65)]
        [TestCase("Mending", 66, 72)]
        [TestCase("Purify Food and Drink", 73, 79)]
        [TestCase("Read Magic", 80, 86)]
        [TestCase("Resistance", 87, 93)]
        [TestCase("Virtue", 94, 100)]
        public void Level0DivineSpellsPercentile(string content, int lower, int upper)
        {
            AssertPercentile(content, lower, upper);
        }
    }
}