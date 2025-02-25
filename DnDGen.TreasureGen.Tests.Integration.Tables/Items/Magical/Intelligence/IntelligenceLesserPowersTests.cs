using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Intelligence
{
    [TestFixture]
    public class IntelligenceLesserPowersTests : PercentileTests
    {
        protected override string tableName => TableNameConstants.Percentiles.IntelligenceLesserPowers;

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

        [TestCase("Item can Bless its allies 3/day", 1, 5)]
        [TestCase("Item can use Faerie Fire 3/day", 6, 10)]
        [TestCase("Item can cast Minor Image 1/day", 11, 13)]
        [TestCase("Item has Deathwatch continually active", 14, 20)]
        [TestCase("Item can use Detect Magic at will", 21, 25)]
        [TestCase("Item has 10 ranks in Intimidate", 26, 31)]
        [TestCase("Item has 10 ranks in Decipher Script", 32, 33)]
        [TestCase("Item has 10 ranks in Knowledge (KNOWLEDGECATEGORY)", 34, 36)]
        [TestCase("Item has 10 ranks in Search", 37, 40)]
        [TestCase("Item has 10 ranks in Spot", 41, 45)]
        [TestCase("Item has 10 ranks in Listen", 46, 50)]
        [TestCase("Item has 10 ranks in Spellcraft", 51, 54)]
        [TestCase("Item has 10 ranks in Sense Motive", 55, 60)]
        [TestCase("Item has 10 ranks in Bluff", 61, 66)]
        [TestCase("Item has 10 ranks in Diplomacy", 67, 72)]
        [TestCase("Item can cast Major Image 1/day", 73, 77)]
        [TestCase("Item can cast Darkness 3/day", 78, 80)]
        [TestCase("Item can use Hold Person on an enemy 3/day", 81, 83)]
        [TestCase("Item can activate Zone of Truth 3/day", 84, 86)]
        [TestCase("Item can use Daze Monster 3/day", 87, 89)]
        [TestCase("Item can use Locate Object 3/day", 90, 95)]
        [TestCase("Item can use Cure Moderate Wounds (2d8+3) on wielder 3/day", 96, 100)]
        public void IntelligenceLesserPowersPercentile(string content, int lower, int upper)
        {
            AssertPercentile(content, lower, upper);
        }
    }
}