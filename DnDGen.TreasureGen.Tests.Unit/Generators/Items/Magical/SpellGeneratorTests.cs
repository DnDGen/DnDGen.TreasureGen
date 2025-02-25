using DnDGen.TreasureGen.Generators.Items.Magical;
using DnDGen.TreasureGen.Selectors.Percentiles;
using DnDGen.TreasureGen.Tables;
using Moq;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Unit.Generators.Items.Magical
{
    [TestFixture]
    public class SpellGeneratorTests
    {
        private ISpellGenerator generator;
        private Mock<ITreasurePercentileSelector> mockPercentileSelector;
        private string power;

        [SetUp]
        public void Setup()
        {
            mockPercentileSelector = new Mock<ITreasurePercentileSelector>();
            generator = new SpellGenerator(mockPercentileSelector.Object);
            power = "power";
        }

        [Test]
        public void ReturnSpellLevel()
        {
            var tableName = TableNameConstants.Percentiles.POWERSpellLevels(power);
            mockPercentileSelector.Setup(p => p.SelectFrom<int>(Config.Name, tableName)).Returns(9266);

            var level = generator.GenerateLevel(power);
            Assert.That(level, Is.EqualTo(9266));
        }

        [Test]
        public void ReturnSpellType()
        {
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, TableNameConstants.Percentiles.SpellTypes)).Returns("spell type");
            var type = generator.GenerateType();
            Assert.That(type, Is.EqualTo("spell type"));
        }

        [Test]
        public void ReturnSpell()
        {
            var tableName = TableNameConstants.Percentiles.LevelXSPELLTYPESpells(9266, "spell type");
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, tableName)).Returns("this is my spell");

            var spell = generator.Generate("spell type", 9266);
            Assert.That(spell, Is.EqualTo("this is my spell"));
        }
    }
}