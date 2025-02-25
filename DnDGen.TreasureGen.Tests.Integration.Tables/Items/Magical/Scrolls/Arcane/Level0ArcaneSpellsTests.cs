using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Scrolls.Arcane
{
    [TestFixture]
    public class Level0ArcaneSpellsTests : PercentileTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Percentiles.LevelXSPELLTYPESpells(0, "Arcane"); }
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

        [TestCase("Acid Splash", 1, 4)]
        [TestCase("Arcane Mark", 5, 8)]
        [TestCase("Dancing Lights", 9, 13)]
        [TestCase("Daze", 14, 17)]
        [TestCase("Detect Magic", 18, 24)]
        [TestCase("Detect Poison", 25, 28)]
        [TestCase("Disrupt Undead", 29, 32)]
        [TestCase("Flare", 33, 37)]
        [TestCase("Ghost Sound", 38, 42)]
        [TestCase("Know Direction", 43, 44)]
        [TestCase("Light", 45, 50)]
        [TestCase("Lullaby", 51, 52)]
        [TestCase("Mage Hand", 53, 57)]
        [TestCase("Mending", 58, 62)]
        [TestCase("Message", 63, 67)]
        [TestCase("Open/Close", 68, 72)]
        [TestCase("Prestidigitation", 73, 77)]
        [TestCase("Ray of Frost", 78, 81)]
        [TestCase("Read Magic", 82, 87)]
        [TestCase("Resistance", 88, 94)]
        [TestCase("Summon Instrument", 95, 96)]
        [TestCase("Touch of Fatigue", 97, 100)]
        public void Level0ArcaneSpellsPercentile(string content, int lower, int upper)
        {
            AssertPercentile(content, lower, upper);
        }
    }
}