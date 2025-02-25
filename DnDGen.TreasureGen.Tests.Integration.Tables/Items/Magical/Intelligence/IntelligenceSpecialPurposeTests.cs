using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Intelligence
{
    [TestFixture]
    public class IntelligenceSpecialPurposeTests : PercentileTests
    {
        protected override string tableName => TableNameConstants.Percentiles.IntelligenceSpecialPurposes;

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

        [TestCase("Defeat/slay diametrically opposed alignment", 1, 20)]
        [TestCase("Defeat/slay arcane spellcasters (including spellcasting monsters and those that use spell-like abilities)", 21, 30)]
        [TestCase("Defeat/slay divine spellcasters (including divine entities and servitors)", 31, 40)]
        [TestCase("Defeat/slay nonspellcasters", 41, 50)]
        [TestCase("Defeat/slay DESIGNATEDFOE", 51, 60)]
        [TestCase("Defend DESIGNATEDFOE", 61, 70)]
        [TestCase("Defeat/slay the servants of a specific deity", 71, 80)]
        [TestCase("Defend the servants of a specific deity", 81, 90)]
        [TestCase("Defeat/slay all (other than the item and the wielder)", 91, 95)]
        [TestCase("Defend arcane spellcasters", 96, 97)]
        [TestCase("Defend divine spellcasters", 98, 99)]
        [TestCase("Defend nonspellcasters", 100, 100)]
        public void IntelligenceSpecificPurposePercentile(string content, int lower, int upper)
        {
            AssertPercentile(content, lower, upper);
        }
    }
}