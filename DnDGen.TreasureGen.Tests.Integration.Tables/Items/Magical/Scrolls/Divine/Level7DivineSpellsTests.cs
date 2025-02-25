using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Scrolls.Divine
{
    [TestFixture]
    public class Level7DivineSpellsTests : PercentileTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Percentiles.LevelXSPELLTYPESpells(7, "Divine"); }
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

        [TestCase("Animate Plants", 1, 5)]
        [TestCase("Blasphemy", 6, 9)]
        [TestCase("Changestaff", 10, 14)]
        [TestCase("Control Weather", 15, 16)]
        [TestCase("Creeping Doom", 17, 21)]
        [TestCase("Mass Cure Serious Wounds", 22, 27)]
        [TestCase("Destruction", 28, 32)]
        [TestCase("Dictum", 33, 36)]
        [TestCase("Ethereal Jaunt", 37, 41)]
        [TestCase("Holy Word", 42, 45)]
        [TestCase("Mass Inflict Serious Wounds", 46, 50)]
        [TestCase("Refuge", 51, 55)]
        [TestCase("Regenerate", 56, 60)]
        [TestCase("Repulsion", 61, 65)]
        [TestCase("Greater Restoration", 66, 69)]
        [TestCase("Resurrection", 70, 71)]
        [TestCase("Greater Scrying", 72, 76)]
        [TestCase("Summon Monster VII", 77, 81)]
        [TestCase("Summon Nature's Ally VII", 82, 85)]
        [TestCase("Sunbeam", 86, 90)]
        [TestCase("Symbol of Stunning", 91, 91)]
        [TestCase("Symbol of Weakness", 92, 92)]
        [TestCase("Transmute Metal to Wood", 93, 97)]
        [TestCase("Word of Chaos", 98, 100)]
        public void Level7DivineSpellsPercentile(string content, int lower, int upper)
        {
            AssertPercentile(content, lower, upper);
        }
    }
}