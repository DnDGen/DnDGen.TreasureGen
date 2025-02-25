using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Scrolls.Arcane
{
    [TestFixture]
    public class Level9ArcaneSpellsTests : PercentileTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Percentiles.LevelXSPELLTYPESpells(9, "Arcane"); }
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

        [TestCase("Astral Projection", 1, 3)]
        [TestCase("Bigby's Crushing Hand", 4, 7)]
        [TestCase("Dominate Monster", 8, 12)]
        [TestCase("Energy Drain", 13, 16)]
        [TestCase("Etherealness", 17, 21)]
        [TestCase("Foresight", 22, 25)]
        [TestCase("Freedom", 26, 31)]
        [TestCase("Gate", 32, 36)]
        [TestCase("Mass Hold Monster", 37, 40)]
        [TestCase("Imprisonment", 41, 44)]
        [TestCase("Meteor Swarm", 45, 49)]
        [TestCase("Mordenkainen's Disjunction", 50, 53)]
        [TestCase("Power Word Kill", 54, 58)]
        [TestCase("Prismatic Sphere", 59, 62)]
        [TestCase("Refuge", 63, 66)]
        [TestCase("Shades", 67, 70)]
        [TestCase("Shapechange", 71, 76)]
        [TestCase("Soul Bind", 77, 79)]
        [TestCase("Summon Monster IX", 80, 83)]
        [TestCase("Teleportation Circle", 84, 86)]
        [TestCase("Time Stop", 87, 91)]
        [TestCase("Wail of the Banshee", 92, 95)]
        [TestCase("Weird", 96, 99)]
        [TestCase("Wish", 100, 100)]
        public void Level9ArcaneSpellsPercentile(string content, int lower, int upper)
        {
            AssertPercentile(content, lower, upper);
        }
    }
}