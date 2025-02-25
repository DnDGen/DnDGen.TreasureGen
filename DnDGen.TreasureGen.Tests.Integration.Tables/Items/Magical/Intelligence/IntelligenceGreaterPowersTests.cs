using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Intelligence
{
    [TestFixture]
    public class IntelligenceGreaterPowersTests : PercentileTests
    {
        protected override string tableName => TableNameConstants.Percentiles.IntelligenceGreaterPowers;

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

        [TestCase("Item can detect opposing alignment at will", 1, 6)]
        [TestCase("Item can Detect Undead at will", 7, 10)]
        [TestCase("Item can Cause Fear in an enemy at will", 11, 13)]
        [TestCase("Item can use Dimensional Anchor on a foe 1/day", 14, 18)]
        [TestCase("Item can use Dismissal on a foe 1/day", 19, 23)]
        [TestCase("Item can use Lesser Globe of Invulnerability 1/day", 24, 28)]
        [TestCase("Item can use Arcane Eye 1/day", 29, 33)]
        [TestCase("Item has continuous Detect Scrying effect", 34, 37)]
        [TestCase("Item creates Wall of Fire in a ring with the wielder at the center 1/day", 38, 41)]
        [TestCase("Item can use Quench on fires 3/day", 42, 45)]
        [TestCase("Item has Status effect, usable at will", 46, 50)]
        [TestCase("Item can use Gust of Wind 3/day", 51, 54)]
        [TestCase("Item can use Clairvoyance 3/day", 55, 59)]
        [TestCase("Item can create Magic Circle against opposing alignment at will", 60, 64)]
        [TestCase("Item can use Haste on its owner 3/day", 65, 68)]
        [TestCase("Item can create Daylight 3/day", 69, 73)]
        [TestCase("Item can create Deeper Darkness 3/day", 74, 76)]
        [TestCase("Item can use Invisibility Purge (30 ft. range) 3/day", 77, 80)]
        [TestCase("Item can use Slow on its enemies 3/day", 81, 85)]
        [TestCase("Item can Locate Creature 3/day", 86, 91)]
        [TestCase("Item can use Fear against foes 3/day", 92, 97)]
        [TestCase("Item can use Detect Thoughts at will", 98, 100)]
        public void IntelligenceGreaterPowersPercentile(string content, int lower, int upper)
        {
            AssertPercentile(content, lower, upper);
        }
    }
}