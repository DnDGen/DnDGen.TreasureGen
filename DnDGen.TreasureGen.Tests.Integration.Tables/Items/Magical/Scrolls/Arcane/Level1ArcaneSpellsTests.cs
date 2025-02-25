using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Scrolls.Arcane
{
    [TestFixture]
    public class Level1ArcaneSpellsTests : PercentileTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Percentiles.LevelXSPELLTYPESpells(1, "Arcane"); }
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

        [TestCase("Alarm", 1, 3)]
        [TestCase("Animate Rope", 4, 5)]
        [TestCase("Burning Hands", 6, 7)]
        [TestCase("Cause Fear", 8, 9)]
        [TestCase("Charm Person", 10, 12)]
        [TestCase("Chill Touch", 13, 14)]
        [TestCase("Color Spray", 15, 16)]
        [TestCase("Comprehend Languages", 17, 19)]
        [TestCase("Lesser Confusion", 20, 20)]
        [TestCase("Cure Light Wounds", 21, 21)]
        [TestCase("Detect Secret Doors", 22, 24)]
        [TestCase("Detect Undead", 25, 26)]
        [TestCase("Disguise Self", 27, 29)]
        [TestCase("Endure Elements", 30, 32)]
        [TestCase("Enlarge Person", 33, 35)]
        [TestCase("Erase", 36, 37)]
        [TestCase("Expeditious Retreat", 38, 40)]
        [TestCase("Feather Fall", 41, 41)]
        [TestCase("Grease", 42, 43)]
        [TestCase("Hold Portal", 44, 45)]
        [TestCase("Hypnotism", 46, 47)]
        [TestCase("Identify", 48, 49)]
        [TestCase("Jump", 50, 51)]
        [TestCase("Mage Armor", 52, 54)]
        [TestCase("Magic Missile", 55, 56)]
        [TestCase("Magic Weapon", 57, 59)]
        [TestCase("Mount", 60, 62)]
        [TestCase("Nystul's Magic Aura", 63, 64)]
        [TestCase("Obscuring Mist", 65, 66)]
        [TestCase("Protection from Chaos/Evil/Good/Law", 67, 74)]
        [TestCase("Ray of Enfeeblement", 75, 76)]
        [TestCase("Reduce Person", 77, 78)]
        [TestCase("Remove Fear", 79, 80)]
        [TestCase("Shield", 81, 82)]
        [TestCase("Shocking Grasp", 83, 84)]
        [TestCase("Silent Image", 85, 86)]
        [TestCase("Sleep", 87, 88)]
        [TestCase("Summon Monster I", 89, 90)]
        [TestCase("Tenser's Floating Disc", 91, 93)]
        [TestCase("True Strike", 94, 95)]
        [TestCase("Undetectable Alignment", 96, 96)]
        [TestCase("Unseen Servant", 97, 98)]
        [TestCase("Ventriloquism", 99, 100)]
        public void Level1ArcaneSpellsPercentile(string content, int lower, int upper)
        {
            AssertPercentile(content, lower, upper);
        }
    }
}