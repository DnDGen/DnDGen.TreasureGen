using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Scrolls.Arcane
{
    [TestFixture]
    public class Level4ArcaneSpellsTests : PercentileTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Percentiles.LevelXSPELLTYPESpells(4, "Arcane"); }
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

        [TestCase("Animate Dead", 1, 2)]
        [TestCase("Arcane Eye", 3, 5)]
        [TestCase("Bestow Curse", 6, 7)]
        [TestCase("Charm Monster", 8, 10)]
        [TestCase("Confusion", 11, 13)]
        [TestCase("Contagion", 14, 15)]
        [TestCase("Crushing Despair", 16, 17)]
        [TestCase("Dimension Door", 20, 23)]
        [TestCase("Dimensional Anchor", 24, 26)]
        [TestCase("Enervation", 27, 28)]
        [TestCase("Mass Enlarge Person", 29, 30)]
        [TestCase("Evard's Black Tentacles", 31, 32)]
        [TestCase("Fear", 33, 34)]
        [TestCase("Fire Shield", 35, 37)]
        [TestCase("Fire Trap", 38, 39)]
        [TestCase("Freedom of Movement", 40, 42)]
        [TestCase("Lesser Globe of Invulnerability", 44, 46)]
        [TestCase("Hallucinatory Terrain", 47, 48)]
        [TestCase("Ice Storm", 49, 50)]
        [TestCase("Illusory Wall", 51, 52)]
        [TestCase("Greater Invisibility", 53, 55)]
        [TestCase("Leomund's Secure Shelter", 56, 57)]
        [TestCase("Minor Creation", 59, 60)]
        [TestCase("Otiluke's Resilient Sphere", 63, 64)]
        [TestCase("Phantasmal Killer", 65, 66)]
        [TestCase("Polymorph", 67, 68)]
        [TestCase("Rainbow Pattern", 69, 70)]
        [TestCase("Mass Reduce Person", 72, 73)]
        [TestCase("Remove Curse", 74, 76)]
        [TestCase("Scrying", 78, 79)]
        [TestCase("Shadow Conjuration", 80, 81)]
        [TestCase("Shout", 82, 83)]
        [TestCase("Solid Fog", 84, 85)]
        [TestCase("Stone Shape", 87, 88)]
        [TestCase("Stoneskin", 89, 91)]
        [TestCase("Summon Monster IV", 92, 93)]
        [TestCase("Wall of Fire", 94, 96)]
        [TestCase("Wall of Ice", 97, 99)]
        public void Level4ArcaneSpellsPercentile(string content, int lower, int upper)
        {
            AssertPercentile(content, lower, upper);
        }

        [TestCase("Cure Critical Wounds", 18)]
        [TestCase("Detect Scrying", 19)]
        [TestCase("Lesser Geas", 43)]
        [TestCase("Locate Creature", 58)]
        [TestCase("Modify Memory", 61)]
        [TestCase("Neutralize Poison", 62)]
        [TestCase("Rary's Mnemonic Enhancer", 71)]
        [TestCase("Repel Vermin", 77)]
        [TestCase("Speak with Plants", 86)]
        [TestCase("Zone of Silence", 100)]
        public void Level4ArcaneSpellsPercentile(string content, int roll)
        {
            AssertPercentile(content, roll);
        }
    }
}