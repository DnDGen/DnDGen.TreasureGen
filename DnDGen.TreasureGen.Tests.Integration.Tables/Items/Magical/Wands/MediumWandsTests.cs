using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Wands
{
    [TestFixture]
    public class MediumWandsTests : PercentileTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Percentiles.POWERITEMTYPEs(PowerConstants.Medium, ItemTypeConstants.Wand); }
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

        [TestCase("Magic Missile (5th)", 1, 3)]
        [TestCase("Bear's Endurance", 4, 7)]
        [TestCase("Bull's Strength", 8, 11)]
        [TestCase("Cat's Grace", 12, 15)]
        [TestCase("Cure Moderate Wounds", 16, 20)]
        [TestCase("Darkness", 21, 22)]
        [TestCase("Daze Monster", 23, 24)]
        [TestCase("Delay Poison", 25, 27)]
        [TestCase("Eagle's Splendor", 28, 31)]
        [TestCase("False Life", 32, 33)]
        [TestCase("Fox's Cunning", 34, 37)]
        [TestCase("Invisibility", 40, 42)]
        [TestCase("Knock", 43, 44)]
        [TestCase("Melf's Acid Arrow", 46, 47)]
        [TestCase("Mirror Image", 48, 49)]
        [TestCase("Owl's Wisdom", 50, 53)]
        [TestCase("Silence", 55, 56)]
        [TestCase("Web", 58, 59)]
        [TestCase("Magic Missile (7th)", 60, 62)]
        [TestCase("Magic Missile (9th)", 63, 64)]
        [TestCase("Call Lightning (5th)", 65, 67)]
        [TestCase("Contagion", 69, 70)]
        [TestCase("Cure Serious Wounds", 71, 74)]
        [TestCase("Dispel Magic", 75, 77)]
        [TestCase("Fireball (5th)", 78, 81)]
        [TestCase("Keen Edge", 82, 83)]
        [TestCase("Lightning Bolt (5th)", 84, 87)]
        [TestCase("Major Image", 88, 89)]
        [TestCase("Slow", 90, 91)]
        [TestCase("Suggestion", 92, 94)]
        [TestCase("Summon Monster III", 95, 97)]
        public void Percentile(string content, int lower, int upper)
        {
            base.AssertPercentile(content, lower, upper);
        }

        [TestCase("Ghoul Touch", 38)]
        [TestCase("Hold Person", 39)]
        [TestCase("Levitate", 45)]
        [TestCase("Shatter", 54)]
        [TestCase("Summon Monster II", 57)]
        [TestCase("Heightened Charm Person (3rd-level spell)", 68)]
        [TestCase("Fireball (6th)", 98)]
        [TestCase("Lightning Bolt (6th)", 99)]
        [TestCase("Searing Light (6th)", 100)]
        public void Percentile(string content, int roll)
        {
            base.AssertPercentile(content, roll);
        }
    }
}