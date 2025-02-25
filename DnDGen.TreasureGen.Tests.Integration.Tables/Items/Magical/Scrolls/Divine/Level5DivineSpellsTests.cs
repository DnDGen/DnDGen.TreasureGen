using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Scrolls.Divine
{
    [TestFixture]
    public class Level5DivineSpellsTests : PercentileTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Percentiles.LevelXSPELLTYPESpells(5, "Divine"); }
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

        [TestCase("Animal Growth", 1, 3)]
        [TestCase("Atonement", 4, 5)]
        [TestCase("Baleful Polymorph", 7, 9)]
        [TestCase("Break Enchantment", 10, 13)]
        [TestCase("Call Lightning Storm", 14, 16)]
        [TestCase("Greater Command", 17, 20)]
        [TestCase("Control Winds", 23, 24)]
        [TestCase("Mass Cure Light Wounds", 25, 30)]
        [TestCase("Dispel Chaos/Evil/Good/Law", 31, 34)]
        [TestCase("Disrupting Weapon", 35, 38)]
        [TestCase("Flame Strike", 39, 41)]
        [TestCase("Hallow", 42, 43)]
        [TestCase("Ice Storm", 44, 46)]
        [TestCase("Mass Inflict Light Wounds", 47, 49)]
        [TestCase("Insect Plague", 50, 52)]
        [TestCase("Plane Shift", 54, 56)]
        [TestCase("Raise Dead", 57, 58)]
        [TestCase("Righteous Might", 59, 61)]
        [TestCase("Scrying", 62, 63)]
        [TestCase("Slay Living", 64, 66)]
        [TestCase("Spell Resistance", 67, 69)]
        [TestCase("Stoneskin", 70, 71)]
        [TestCase("Summon Monster V", 72, 74)]
        [TestCase("Summon Nature's Ally V", 75, 77)]
        [TestCase("Transmute Mud to Rock", 80, 82)]
        [TestCase("Transmute Rock to Mud", 83, 85)]
        [TestCase("True Seeing", 86, 89)]
        [TestCase("Unhallow", 90, 91)]
        [TestCase("Wall of Fire", 92, 94)]
        [TestCase("Wall of Stone", 95, 97)]
        [TestCase("Wall of Thorns", 98, 100)]
        public void Level5DivineSpellsPercentile(string content, int lower, int upper)
        {
            AssertPercentile(content, lower, upper);
        }

        [TestCase("Awaken", 6)]
        [TestCase("Commune", 21)]
        [TestCase("Commune with Nature", 22)]
        [TestCase("Mark of Justice", 53)]
        [TestCase("Symbol of Pain", 78)]
        [TestCase("Symbol of Sleep", 79)]
        public void Level5DivineSpellsPercentile(string content, int roll)
        {
            AssertPercentile(content, roll);
        }
    }
}