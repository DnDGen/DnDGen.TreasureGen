using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Tests.Unit.Generators.Items;
using NUnit.Framework;
using System.Linq;

namespace DnDGen.TreasureGen.Tests.Integration.Generators.Items.Magical
{
    [TestFixture]
    public class WondrousItemGeneratorTests : IntegrationTests
    {
        private MagicalItemGenerator wondrousItemGenerator;
        private ItemVerifier itemVerifier;

        [SetUp]
        public void Setup()
        {
            itemVerifier = new ItemVerifier();
            wondrousItemGenerator = GetNewInstanceOf<MagicalItemGenerator>(ItemTypeConstants.WondrousItem);
        }

        [TestCaseSource(typeof(ItemPowerTestData), nameof(ItemPowerTestData.WondrousItems))]
        public void GenerateWondrousItem(string itemName, string power)
        {
            var item = wondrousItemGenerator.Generate(power, itemName);
            itemVerifier.AssertItem(item);
        }

        [Test]
        [Repeat(100)]
        public void BUG_CubicGateDoesNotDuplicateMaterialPlane()
        {
            var cubicGate = wondrousItemGenerator.Generate(PowerConstants.Major, WondrousItemConstants.CubicGate);
            Assert.That(cubicGate.ItemType, Is.EqualTo(ItemTypeConstants.WondrousItem));
            Assert.That(cubicGate.Name, Is.EqualTo(WondrousItemConstants.CubicGate));
            Assert.That(cubicGate.Contents, Is.Unique);
            Assert.That(cubicGate.Contents.Select(p => p.ToLower()), Is.Unique);
        }
    }
}
