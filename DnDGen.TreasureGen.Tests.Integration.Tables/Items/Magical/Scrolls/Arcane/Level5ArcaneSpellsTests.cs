using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Scrolls.Arcane
{
    [TestFixture]
    public class Level5ArcaneSpellsTests : PercentileTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Percentiles.LevelXSPELLTYPESpells(5, "Arcane"); }
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

        [TestCase("Animal Growth", 1, 2)]
        [TestCase("Baleful Polymorph", 3, 5)]
        [TestCase("Bigby's Interposing Hand", 6, 7)]
        [TestCase("Blight", 8, 9)]
        [TestCase("Break Enchantment", 10, 12)]
        [TestCase("Cloudkill", 13, 14)]
        [TestCase("Cone of Cold", 15, 17)]
        [TestCase("Contact Other Plane", 18, 19)]
        [TestCase("Dismissal", 21, 23)]
        [TestCase("Greater Dispel Magic", 24, 26)]
        [TestCase("Dominate Person", 27, 28)]
        [TestCase("Fabricate", 30, 31)]
        [TestCase("False Vision", 32, 33)]
        [TestCase("Feeblemind", 34, 35)]
        [TestCase("Hold Monster", 36, 39)]
        [TestCase("Major Creation", 42, 43)]
        [TestCase("Mind Fog", 44, 45)]
        [TestCase("Mirage Arcana", 46, 47)]
        [TestCase("Mordenkainen's Faithful Hound", 48, 49)]
        [TestCase("Mordenkainen's Private Sanctum", 50, 51)]
        [TestCase("Nightmare", 52, 53)]
        [TestCase("Overland Flight", 54, 57)]
        [TestCase("Passwall", 58, 60)]
        [TestCase("Persistent Image", 62, 63)]
        [TestCase("Lesser Planar Binding", 64, 65)]
        [TestCase("Prying Eyes", 66, 67)]
        [TestCase("Rary's Telepathic Bond", 68, 69)]
        [TestCase("Seeming", 70, 71)]
        [TestCase("Sending", 72, 74)]
        [TestCase("Shadow Evocation", 75, 76)]
        [TestCase("Summon Monster V", 78, 79)]
        [TestCase("Telekinesis", 82, 83)]
        [TestCase("Teleport", 84, 88)]
        [TestCase("Transmute Mud to Rock", 89, 90)]
        [TestCase("Transmute Rock to Mud", 91, 92)]
        [TestCase("Wall of Force", 93, 95)]
        [TestCase("Wall of Stone", 96, 98)]
        [TestCase("Waves of Fatigue", 99, 100)]
        public void Level5ArcaneSpellsPercentile(string content, int lower, int upper)
        {
            AssertPercentile(content, lower, upper);
        }

        [TestCase("Mass Cure Light Wounds", 20)]
        [TestCase("Dream", 29)]
        [TestCase("Leomund's Secret Chest", 40)]
        [TestCase("Magic Jar", 41)]
        [TestCase("Permanency", 61)]
        [TestCase("Song of Discord", 77)]
        [TestCase("Symbol of Pain", 80)]
        [TestCase("Symbol of Sleep", 81)]
        public void Level5ArcaneSpellsPercentile(string content, int roll)
        {
            AssertPercentile(content, roll);
        }
    }
}