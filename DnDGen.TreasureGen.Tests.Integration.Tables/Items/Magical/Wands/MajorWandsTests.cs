using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Wands
{
    [TestFixture]
    public class MajorWandsTests : PercentileTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Percentiles.POWERITEMTYPEs(PowerConstants.Major, ItemTypeConstants.Wand); }
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

        [TestCase("Magic Missile (7th)", 1, 2)]
        [TestCase("Magic Missile (9th)", 3, 5)]
        [TestCase("Call Lightning (5th)", 6, 7)]
        [TestCase("Contagion", 9, 10)]
        [TestCase("Cure Serious Wounds", 11, 13)]
        [TestCase("Dispel Magic", 14, 15)]
        [TestCase("Fireball (5th)", 16, 17)]
        [TestCase("Keen Edge", 18, 19)]
        [TestCase("Lightning Bolt (5th)", 20, 21)]
        [TestCase("Major Image", 22, 23)]
        [TestCase("Slow", 24, 25)]
        [TestCase("Suggestion", 26, 27)]
        [TestCase("Summon Monster III", 28, 29)]
        [TestCase("Fireball (6th)", 30, 31)]
        [TestCase("Lightning Bolt (6th)", 32, 33)]
        [TestCase("Searing Light (6th)", 34, 35)]
        [TestCase("Call Lightning (8th)", 36, 37)]
        [TestCase("Fireball (8th)", 38, 39)]
        [TestCase("Lightning Bolt (8th)", 40, 41)]
        [TestCase("Charm Monster", 42, 45)]
        [TestCase("Cure Critical Wounds", 46, 50)]
        [TestCase("Dimensional Anchor", 51, 52)]
        [TestCase("Fear", 53, 55)]
        [TestCase("Greater Invisibility", 56, 59)]
        [TestCase("Ice Storm", 61, 65)]
        [TestCase("Inflict Critical Wounds", 66, 68)]
        [TestCase("Neutralize Poison", 69, 72)]
        [TestCase("Poison", 73, 74)]
        [TestCase("Polymorph", 75, 77)]
        [TestCase("Summon Monster IV", 80, 82)]
        [TestCase("Wall of Fire", 83, 86)]
        [TestCase("Wall of Ice", 87, 90)]
        [TestCase("Restoration", 98, 99)]
        public void Percentile(string content, int lower, int upper)
        {
            base.AssertPercentile(content, lower, upper);
        }

        [TestCase("Heightened Charm Person (3rd-level spell)", 8)]
        [TestCase("Heightened Hold Person (4th-level spell)", 60)]
        [TestCase("Heightened Ray of Enfeeblement (4th-level spell)", 78)]
        [TestCase("Heightened Suggestion (4th-level spell)", 79)]
        [TestCase("Dispel Magic (10th)", 91)]
        [TestCase("Fireball (10th)", 92)]
        [TestCase("Lightning Bolt (10th)", 93)]
        [TestCase("Chaos Hammer (10th)", 94)]
        [TestCase("Holy Smite (8th)", 95)]
        [TestCase("Order's Wrath (8th)", 96)]
        [TestCase("Unholy Blight (8th)", 97)]
        [TestCase("Stoneskin", 100)]
        public void Percentile(string content, int roll)
        {
            base.AssertPercentile(content, roll);
        }
    }
}