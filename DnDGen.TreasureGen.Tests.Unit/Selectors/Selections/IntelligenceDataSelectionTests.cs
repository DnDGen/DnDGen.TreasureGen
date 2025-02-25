using DnDGen.TreasureGen.Selectors.Selections;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Unit.Selectors.Selections
{
    [TestFixture]
    internal class IntelligenceDataSelectionTests
    {
        private IntelligenceDataSelection selection;

        [SetUp]
        public void Setup()
        {
            selection = new IntelligenceDataSelection();
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
            data[DataIndexConstants.Intelligence.Senses] = "my senses";
            data[DataIndexConstants.Intelligence.LesserPowersCount] = "9266";
            data[DataIndexConstants.Intelligence.GreaterPowersCount] = "42";

            var newSelection = IntelligenceDataSelection.Map(data);
            Assert.That(newSelection, Is.Not.Null);
            Assert.That(newSelection.Senses, Is.EqualTo("my senses"));
            Assert.That(newSelection.LesserPowersCount, Is.EqualTo(9266));
            Assert.That(newSelection.GreaterPowersCount, Is.EqualTo(42));
        }

        [Test]
        public void Map_FromSelection_ReturnsString()
        {
            var selection = new IntelligenceDataSelection { Senses = "my senses", LesserPowersCount = 9266, GreaterPowersCount = 42 };
            var rawData = IntelligenceDataSelection.Map(selection);
            Assert.That(rawData.Length, Is.EqualTo(selection.SectionCount));
            Assert.That(rawData[DataIndexConstants.Intelligence.Senses], Is.EqualTo("my senses"));
            Assert.That(rawData[DataIndexConstants.Intelligence.LesserPowersCount], Is.EqualTo("9266"));
            Assert.That(rawData[DataIndexConstants.Intelligence.GreaterPowersCount], Is.EqualTo("42"));
        }

        [Test]
        public void MapTo_ReturnsSelection()
        {
            var data = new string[selection.SectionCount];
            data[DataIndexConstants.Intelligence.Senses] = "my senses";
            data[DataIndexConstants.Intelligence.LesserPowersCount] = "9266";
            data[DataIndexConstants.Intelligence.GreaterPowersCount] = "42";

            var newSelection = selection.MapTo(data);
            Assert.That(newSelection, Is.Not.Null);
            Assert.That(newSelection.Senses, Is.EqualTo("my senses"));
            Assert.That(newSelection.LesserPowersCount, Is.EqualTo(9266));
            Assert.That(newSelection.GreaterPowersCount, Is.EqualTo(42));
        }

        [Test]
        public void MapFrom_ReturnsString()
        {
            var selection = new IntelligenceDataSelection { Senses = "my senses", LesserPowersCount = 9266, GreaterPowersCount = 42 };
            var rawData = selection.MapFrom(selection);
            Assert.That(rawData.Length, Is.EqualTo(selection.SectionCount));
            Assert.That(rawData[DataIndexConstants.Intelligence.Senses], Is.EqualTo("my senses"));
            Assert.That(rawData[DataIndexConstants.Intelligence.LesserPowersCount], Is.EqualTo("9266"));
            Assert.That(rawData[DataIndexConstants.Intelligence.GreaterPowersCount], Is.EqualTo("42"));
        }
    }
}
