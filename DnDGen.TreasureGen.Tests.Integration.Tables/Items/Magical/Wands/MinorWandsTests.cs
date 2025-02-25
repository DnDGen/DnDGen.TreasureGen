using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Wands
{
    [TestFixture]
    public class MinorWandsTests : PercentileTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Percentiles.POWERITEMTYPEs(PowerConstants.Minor, ItemTypeConstants.Wand); }
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

        [TestCase("Detect Magic", 1, 2)]
        [TestCase("Light", 3, 4)]
        [TestCase("Burning Hands", 5, 7)]
        [TestCase("Charm Animal", 8, 10)]
        [TestCase("Charm Person", 11, 13)]
        [TestCase("Color Spray", 14, 16)]
        [TestCase("Cure Light Wounds", 17, 19)]
        [TestCase("Detect Secret Doors", 20, 22)]
        [TestCase("Enlarge Person", 23, 25)]
        [TestCase("Magic Missile (1st)", 26, 28)]
        [TestCase("Shocking Grasp", 29, 31)]
        [TestCase("Summon Monster I", 32, 34)]
        [TestCase("Magic Missile (3rd)", 35, 36)]
        [TestCase("Bear's Endurance", 38, 40)]
        [TestCase("Bull's Strength", 41, 43)]
        [TestCase("Cat's Grace", 44, 46)]
        [TestCase("Cure Moderate Wounds", 47, 49)]
        [TestCase("Darkness", 50, 51)]
        [TestCase("Daze Monster", 52, 54)]
        [TestCase("Delay Poison", 55, 57)]
        [TestCase("Eagle's Splendor", 58, 60)]
        [TestCase("False Life", 61, 63)]
        [TestCase("Fox's Cunning", 64, 66)]
        [TestCase("Ghoul Touch", 67, 68)]
        [TestCase("Hold Person", 69, 71)]
        [TestCase("Invisibility", 72, 74)]
        [TestCase("Knock", 75, 77)]
        [TestCase("Levitate", 78, 80)]
        [TestCase("Melf's Acid Arrow", 81, 83)]
        [TestCase("Mirror Image", 84, 86)]
        [TestCase("Owl's Wisdom", 87, 89)]
        [TestCase("Shatter", 90, 91)]
        [TestCase("Silence", 92, 94)]
        [TestCase("Summon Monster II", 95, 97)]
        [TestCase("Web", 98, 100)]
        public void Percentile(string content, int lower, int upper)
        {
            base.AssertPercentile(content, lower, upper);
        }

        [TestCase("Magic Missile (5th)", 37)]
        public void Percentile(string content, int roll)
        {
            base.AssertPercentile(content, roll);
        }
    }
}