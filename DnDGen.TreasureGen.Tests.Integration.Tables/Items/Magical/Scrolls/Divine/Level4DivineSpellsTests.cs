using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Scrolls.Divine
{
    [TestFixture]
    public class Level4DivineSpellsTests : PercentileTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Percentiles.LevelXSPELLTYPESpells(4, "Divine"); }
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

        [TestCase("Air Walk", 1, 5)]
        [TestCase("Antiplant Shell", 6, 7)]
        [TestCase("Blight", 8, 9)]
        [TestCase("Break Enchantment", 10, 11)]
        [TestCase("Command Plants", 12, 13)]
        [TestCase("Control Water", 14, 15)]
        [TestCase("Cure Critical Wounds", 16, 21)]
        [TestCase("Death Ward", 22, 26)]
        [TestCase("Dimensional Anchor", 27, 31)]
        [TestCase("Discern Lies", 32, 34)]
        [TestCase("Dismissal", 35, 37)]
        [TestCase("Divination", 38, 39)]
        [TestCase("Divine Power", 40, 42)]
        [TestCase("Freedom of Movement", 43, 47)]
        [TestCase("Giant Vermin", 48, 49)]
        [TestCase("Holy Sword", 50, 51)]
        [TestCase("Imbue with Spell Ability", 52, 54)]
        [TestCase("Inflict Critical Wounds", 55, 57)]
        [TestCase("Greater Magic Weapon", 58, 60)]
        [TestCase("Nondetection", 61, 62)]
        [TestCase("Lesser Planar Ally", 63, 64)]
        [TestCase("Poison", 65, 67)]
        [TestCase("Reincarnate", 68, 69)]
        [TestCase("Repel Vermin", 70, 71)]
        [TestCase("Restoration", 72, 76)]
        [TestCase("Rusting Grasp", 77, 78)]
        [TestCase("Sending", 79, 81)]
        [TestCase("Spell Immunity", 82, 85)]
        [TestCase("Spike Stones", 86, 87)]
        [TestCase("Summon Monster IV", 88, 90)]
        [TestCase("Summon Nature's Ally IV", 91, 93)]
        [TestCase("Tongues", 94, 98)]
        [TestCase("Tree Stride", 99, 100)]
        public void Level4DivineSpellsPercentile(string content, int lower, int upper)
        {
            AssertPercentile(content, lower, upper);
        }
    }
}