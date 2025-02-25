using DnDGen.TreasureGen.Selectors.Selections;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Unit.Selectors.Selections
{
    [TestFixture]
    internal class DamageDataSelectionTests
    {
        private DamageDataSelection selection;

        [SetUp]
        public void Setup()
        {
            selection = new DamageDataSelection();
        }

        [Test]
        public void PropertiesHaveDefaultValues()
        {
            Assert.That(selection.Type, Is.Empty);
            Assert.That(selection.Roll, Is.Empty);
            Assert.That(selection.Condition, Is.Empty);
        }

        [Test]
        public void SectionCountIs3()
        {
            Assert.That(selection.SectionCount, Is.EqualTo(3));
        }

        [Test]
        public void Map_FromString_ReturnsSelection()
        {
            var data = new string[selection.SectionCount];
            data[DataIndexConstants.Weapon.DamageData.ConditionIndex] = "my condition";
            data[DataIndexConstants.Weapon.DamageData.RollIndex] = "my roll";
            data[DataIndexConstants.Weapon.DamageData.TypeIndex] = "my type";

            var newSelection = DamageDataSelection.Map(data);
            Assert.That(newSelection, Is.Not.Null);
            Assert.That(newSelection.Condition, Is.EqualTo("my condition"));
            Assert.That(newSelection.Roll, Is.EqualTo("my roll"));
            Assert.That(newSelection.Type, Is.EqualTo("my type"));
        }

        [Test]
        public void Map_FromSelection_ReturnsString()
        {
            var selection = new DamageDataSelection { Condition = "my condition", Roll = "my roll", Type = "my type" };
            var rawData = DamageDataSelection.Map(selection);
            Assert.That(rawData.Length, Is.EqualTo(selection.SectionCount));
            Assert.That(rawData[DataIndexConstants.Weapon.DamageData.ConditionIndex], Is.EqualTo("my condition"));
            Assert.That(rawData[DataIndexConstants.Weapon.DamageData.RollIndex], Is.EqualTo("my roll"));
            Assert.That(rawData[DataIndexConstants.Weapon.DamageData.TypeIndex], Is.EqualTo("my type"));
        }

        [Test]
        public void MapTo_ReturnsSelection()
        {
            var data = new string[selection.SectionCount];
            data[DataIndexConstants.Weapon.DamageData.ConditionIndex] = "my condition";
            data[DataIndexConstants.Weapon.DamageData.RollIndex] = "my roll";
            data[DataIndexConstants.Weapon.DamageData.TypeIndex] = "my type";

            var newSelection = selection.MapTo(data);
            Assert.That(newSelection, Is.Not.Null);
            Assert.That(newSelection.Condition, Is.EqualTo("my condition"));
            Assert.That(newSelection.Roll, Is.EqualTo("my roll"));
            Assert.That(newSelection.Type, Is.EqualTo("my type"));
        }

        [Test]
        public void MapFrom_ReturnsString()
        {
            var selection = new DamageDataSelection { Condition = "my condition", Roll = "my roll", Type = "my type" };
            var rawData = selection.MapFrom(selection);
            Assert.That(rawData.Length, Is.EqualTo(selection.SectionCount));
            Assert.That(rawData[DataIndexConstants.Weapon.DamageData.ConditionIndex], Is.EqualTo("my condition"));
            Assert.That(rawData[DataIndexConstants.Weapon.DamageData.RollIndex], Is.EqualTo("my roll"));
            Assert.That(rawData[DataIndexConstants.Weapon.DamageData.TypeIndex], Is.EqualTo("my type"));
        }
    }
}
