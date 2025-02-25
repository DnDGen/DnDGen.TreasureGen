using DnDGen.TreasureGen.Selectors.Selections;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Unit.Selectors.Selections
{
    [TestFixture]
    internal class ArmorDataSelectionTests
    {
        private ArmorDataSelection selection;

        [SetUp]
        public void Setup()
        {
            selection = new ArmorDataSelection();
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
            data[DataIndexConstants.Armor.ArmorCheckPenalty] = "-90210";
            data[DataIndexConstants.Armor.ArmorBonus] = "9266";
            data[DataIndexConstants.Armor.MaxDexterityBonus] = "42";

            var newSelection = ArmorDataSelection.Map(data);
            Assert.That(newSelection, Is.Not.Null);
            Assert.That(newSelection.ArmorBonus, Is.EqualTo(9266));
            Assert.That(newSelection.ArmorCheckPenalty, Is.EqualTo(-90210));
            Assert.That(newSelection.MaxDexterityBonus, Is.EqualTo(42));
        }

        [Test]
        public void Map_FromSelection_ReturnsString()
        {
            var selection = new ArmorDataSelection { ArmorBonus = 9266, ArmorCheckPenalty = -90210, MaxDexterityBonus = 42 };
            var rawData = ArmorDataSelection.Map(selection);
            Assert.That(rawData.Length, Is.EqualTo(selection.SectionCount));
            Assert.That(rawData[DataIndexConstants.Armor.ArmorBonus], Is.EqualTo("9266"));
            Assert.That(rawData[DataIndexConstants.Armor.ArmorCheckPenalty], Is.EqualTo("-90210"));
            Assert.That(rawData[DataIndexConstants.Armor.MaxDexterityBonus], Is.EqualTo("42"));
        }

        [Test]
        public void MapTo_ReturnsSelection()
        {
            var data = new string[selection.SectionCount];
            data[DataIndexConstants.Armor.ArmorCheckPenalty] = "-90210";
            data[DataIndexConstants.Armor.ArmorBonus] = "9266";
            data[DataIndexConstants.Armor.MaxDexterityBonus] = "42";

            var newSelection = selection.MapTo(data);
            Assert.That(newSelection, Is.Not.Null);
            Assert.That(newSelection.ArmorBonus, Is.EqualTo(9266));
            Assert.That(newSelection.ArmorCheckPenalty, Is.EqualTo(-90210));
            Assert.That(newSelection.MaxDexterityBonus, Is.EqualTo(42));
        }

        [Test]
        public void MapFrom_ReturnsString()
        {
            var selection = new ArmorDataSelection { ArmorBonus = 9266, ArmorCheckPenalty = -90210, MaxDexterityBonus = 42 };
            var rawData = selection.MapFrom(selection);
            Assert.That(rawData.Length, Is.EqualTo(selection.SectionCount));
            Assert.That(rawData[DataIndexConstants.Armor.ArmorBonus], Is.EqualTo("9266"));
            Assert.That(rawData[DataIndexConstants.Armor.ArmorCheckPenalty], Is.EqualTo("-90210"));
            Assert.That(rawData[DataIndexConstants.Armor.MaxDexterityBonus], Is.EqualTo("42"));
        }
    }
}
