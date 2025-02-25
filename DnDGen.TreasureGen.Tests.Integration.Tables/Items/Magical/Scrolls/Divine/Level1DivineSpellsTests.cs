using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Scrolls.Divine
{
    [TestFixture]
    public class Level1DivineSpellsTests : PercentileTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Percentiles.LevelXSPELLTYPESpells(1, "Divine"); }
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

        [TestCase("Alarm", 1, 1)]
        [TestCase("Bane", 2, 3)]
        [TestCase("Bless", 4, 6)]
        [TestCase("Bless Water", 7, 9)]
        [TestCase("Bless Weapon", 10, 10)]
        [TestCase("Calm Animals", 11, 12)]
        [TestCase("Cause Fear", 13, 14)]
        [TestCase("Charm Animal", 15, 16)]
        [TestCase("Command", 17, 19)]
        [TestCase("Comprehend Languages", 20, 21)]
        [TestCase("Cure Light Wounds", 22, 26)]
        [TestCase("Curse Water", 27, 28)]
        [TestCase("Deathwatch", 29, 30)]
        [TestCase("Detect Animals or Plants", 31, 32)]
        [TestCase("Detect Chaos/Evil/Good/Law", 33, 35)]
        [TestCase("Detect Snares and Pits", 36, 37)]
        [TestCase("Detect Undead", 38, 39)]
        [TestCase("Divine Favor", 40, 41)]
        [TestCase("Doom", 42, 43)]
        [TestCase("Endure Elements", 44, 48)]
        [TestCase("Entangle", 49, 50)]
        [TestCase("Entropic Shield", 51, 52)]
        [TestCase("Faerie Fire", 53, 54)]
        [TestCase("Goodberry", 55, 56)]
        [TestCase("Hide from Animals", 57, 58)]
        [TestCase("Hide from Undead", 59, 60)]
        [TestCase("Inflict Light Wounds", 61, 62)]
        [TestCase("Jump", 63, 64)]
        [TestCase("Longstrider", 65, 66)]
        [TestCase("Magic Fang", 67, 68)]
        [TestCase("Magic Stone", 69, 72)]
        [TestCase("Magic Weapon", 73, 74)]
        [TestCase("Obscuring Mist", 75, 78)]
        [TestCase("Pass Without Trace", 79, 80)]
        [TestCase("Produce Flame", 81, 82)]
        [TestCase("Protection from Chaos/Evil/Good/Law", 83, 86)]
        [TestCase("Remove Fear", 87, 88)]
        [TestCase("Sanctuary", 89, 90)]
        [TestCase("Shield of Faith", 91, 92)]
        [TestCase("Shillelagh", 93, 94)]
        [TestCase("Speak with Animals", 95, 96)]
        [TestCase("Summon Monster I", 97, 98)]
        [TestCase("Summon Nature's Ally I", 99, 100)]
        public void Level1DivineSpellsPercentile(string content, int lower, int upper)
        {
            AssertPercentile(content, lower, upper);
        }
    }
}