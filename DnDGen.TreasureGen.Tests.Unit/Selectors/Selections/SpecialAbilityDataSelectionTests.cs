using DnDGen.TreasureGen.Selectors.Selections;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Unit.Selectors.Selections
{
    [TestFixture]
    internal class SpecialAbilityDataSelectionTests
    {
        private SpecialAbilityDataSelection selection;

        [SetUp]
        public void Setup()
        {
            selection = new SpecialAbilityDataSelection();
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
            data[DataIndexConstants.SpecialAbility.BaseName] = "my special ability";
            data[DataIndexConstants.SpecialAbility.BonusEquivalent] = "9266";
            data[DataIndexConstants.SpecialAbility.Power] = "42";

            var newSelection = SpecialAbilityDataSelection.Map(data);
            Assert.That(newSelection, Is.Not.Null);
            Assert.That(newSelection.BaseName, Is.EqualTo("my special ability"));
            Assert.That(newSelection.BonusEquivalent, Is.EqualTo(9266));
            Assert.That(newSelection.Power, Is.EqualTo(42));
        }

        [Test]
        public void Map_FromSelection_ReturnsString()
        {
            var selection = new SpecialAbilityDataSelection { BaseName = "my special ability", BonusEquivalent = 9266, Power = 42 };
            var rawData = SpecialAbilityDataSelection.Map(selection);
            Assert.That(rawData.Length, Is.EqualTo(selection.SectionCount));
            Assert.That(rawData[DataIndexConstants.SpecialAbility.BaseName], Is.EqualTo("my special ability"));
            Assert.That(rawData[DataIndexConstants.SpecialAbility.BonusEquivalent], Is.EqualTo("9266"));
            Assert.That(rawData[DataIndexConstants.SpecialAbility.Power], Is.EqualTo("42"));
        }

        [Test]
        public void MapTo_ReturnsSelection()
        {
            var data = new string[selection.SectionCount];
            data[DataIndexConstants.SpecialAbility.BaseName] = "my special ability";
            data[DataIndexConstants.SpecialAbility.BonusEquivalent] = "9266";
            data[DataIndexConstants.SpecialAbility.Power] = "42";

            var newSelection = selection.MapTo(data);
            Assert.That(newSelection, Is.Not.Null);
            Assert.That(newSelection.BaseName, Is.EqualTo("my special ability"));
            Assert.That(newSelection.BonusEquivalent, Is.EqualTo(9266));
            Assert.That(newSelection.Power, Is.EqualTo(42));
        }

        [Test]
        public void MapFrom_ReturnsString()
        {
            var selection = new SpecialAbilityDataSelection { BaseName = "my special ability", BonusEquivalent = 9266, Power = 42 };
            var rawData = selection.MapFrom(selection);
            Assert.That(rawData.Length, Is.EqualTo(selection.SectionCount));
            Assert.That(rawData[DataIndexConstants.SpecialAbility.BaseName], Is.EqualTo("my special ability"));
            Assert.That(rawData[DataIndexConstants.SpecialAbility.BonusEquivalent], Is.EqualTo("9266"));
            Assert.That(rawData[DataIndexConstants.SpecialAbility.Power], Is.EqualTo("42"));
        }
    }
}
