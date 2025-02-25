using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Intelligence
{
    [TestFixture]
    public class LanguagesTests : PercentileTests
    {
        protected override string tableName => TableNameConstants.Percentiles.Languages;

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

        [TestCase("Abyssal", 1, 5)]
        [TestCase("Aquan", 6, 10)]
        [TestCase("Auran", 11, 15)]
        [TestCase("Celestial", 16, 20)]
        [TestCase("Common", 21, 25)]
        [TestCase("Draconic", 26, 30)]
        [TestCase("Dwarven", 31, 35)]
        [TestCase("Elven", 36, 40)]
        [TestCase("Giant", 41, 45)]
        [TestCase("Gnome", 46, 50)]
        [TestCase("Goblin", 51, 55)]
        [TestCase("Gnoll", 56, 60)]
        [TestCase("Halfling", 61, 65)]
        [TestCase("Ignan", 66, 70)]
        [TestCase("Infernal", 71, 75)]
        [TestCase("Orc", 76, 80)]
        [TestCase("Sylvan", 81, 85)]
        [TestCase("Terran", 86, 90)]
        [TestCase("Undercommon", 91, 95)]
        [TestCase("Druidic", 96, 100)]
        public void LanguagesPercentile(string content, int lower, int upper)
        {
            AssertPercentile(content, lower, upper);
        }
    }
}