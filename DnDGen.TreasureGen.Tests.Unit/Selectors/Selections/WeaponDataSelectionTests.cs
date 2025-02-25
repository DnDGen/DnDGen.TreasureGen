using DnDGen.TreasureGen.Selectors.Selections;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Unit.Selectors.Selections
{
    [TestFixture]
    internal class WeaponDataSelectionTests
    {
        private WeaponDataSelection selection;

        [SetUp]
        public void Setup()
        {
            selection = new WeaponDataSelection();
        }

        [Test]
        public void SectionCountIs4()
        {
            Assert.That(selection.SectionCount, Is.EqualTo(4));
        }

        [Test]
        public void Map_FromString_ReturnsSelection()
        {
            var data = new string[selection.SectionCount];
            data[DataIndexConstants.Weapon.ThreatRange] = "9266";
            data[DataIndexConstants.Weapon.Ammunition] = "my ammo";
            data[DataIndexConstants.Weapon.CriticalMultiplier] = "over 9000";
            data[DataIndexConstants.Weapon.SecondaryCriticalMultiplier] = "around 300";

            var newSelection = WeaponDataSelection.Map(data);
            Assert.That(newSelection, Is.Not.Null);
            Assert.That(newSelection.ThreatRange, Is.EqualTo(9266));
            Assert.That(newSelection.Ammunition, Is.EqualTo("my ammo"));
            Assert.That(newSelection.CriticalMultiplier, Is.EqualTo("over 9000"));
            Assert.That(newSelection.SecondaryCriticalMultiplier, Is.EqualTo("around 300"));
        }

        [Test]
        public void Map_FromSelection_ReturnsString()
        {
            var selection = new WeaponDataSelection
            {
                ThreatRange = 9266,
                Ammunition = "my ammo",
                CriticalMultiplier = "over 9000",
                SecondaryCriticalMultiplier = "around 300"
            };
            var rawData = WeaponDataSelection.Map(selection);
            Assert.That(rawData.Length, Is.EqualTo(selection.SectionCount));
            Assert.That(rawData[DataIndexConstants.Weapon.ThreatRange], Is.EqualTo("9266"));
            Assert.That(rawData[DataIndexConstants.Weapon.Ammunition], Is.EqualTo("my ammo"));
            Assert.That(rawData[DataIndexConstants.Weapon.CriticalMultiplier], Is.EqualTo("over 9000"));
            Assert.That(rawData[DataIndexConstants.Weapon.SecondaryCriticalMultiplier], Is.EqualTo("around 300"));
        }

        [Test]
        public void MapTo_ReturnsSelection()
        {
            var data = new string[selection.SectionCount];
            data[DataIndexConstants.Weapon.ThreatRange] = "9266";
            data[DataIndexConstants.Weapon.Ammunition] = "my ammo";
            data[DataIndexConstants.Weapon.CriticalMultiplier] = "over 9000";
            data[DataIndexConstants.Weapon.SecondaryCriticalMultiplier] = "around 300";

            var newSelection = selection.MapTo(data);
            Assert.That(newSelection, Is.Not.Null);
            Assert.That(newSelection.ThreatRange, Is.EqualTo(9266));
            Assert.That(newSelection.Ammunition, Is.EqualTo("my ammo"));
            Assert.That(newSelection.CriticalMultiplier, Is.EqualTo("over 9000"));
            Assert.That(newSelection.SecondaryCriticalMultiplier, Is.EqualTo("around 300"));
        }

        [Test]
        public void MapFrom_ReturnsString()
        {
            var selection = new WeaponDataSelection
            {
                ThreatRange = 9266,
                Ammunition = "my ammo",
                CriticalMultiplier = "over 9000",
                SecondaryCriticalMultiplier = "around 300"
            };
            var rawData = selection.MapFrom(selection);
            Assert.That(rawData.Length, Is.EqualTo(selection.SectionCount));
            Assert.That(rawData[DataIndexConstants.Weapon.ThreatRange], Is.EqualTo("9266"));
            Assert.That(rawData[DataIndexConstants.Weapon.Ammunition], Is.EqualTo("my ammo"));
            Assert.That(rawData[DataIndexConstants.Weapon.CriticalMultiplier], Is.EqualTo("over 9000"));
            Assert.That(rawData[DataIndexConstants.Weapon.SecondaryCriticalMultiplier], Is.EqualTo("around 300"));
        }
    }
}
