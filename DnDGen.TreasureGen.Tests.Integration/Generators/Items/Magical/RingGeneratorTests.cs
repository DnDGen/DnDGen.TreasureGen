using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Tests.Unit.Generators.Items;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Generators.Items.Magical
{
    [TestFixture]
    public class RingGeneratorTests : IntegrationTests
    {
        private MagicalItemGenerator ringGenerator;
        private ItemVerifier itemVerifier;

        [SetUp]
        public void Setup()
        {
            itemVerifier = new ItemVerifier();
            ringGenerator = GetNewInstanceOf<MagicalItemGenerator>(ItemTypeConstants.Ring);
        }

        [TestCaseSource(typeof(ItemPowerTestData), nameof(ItemPowerTestData.Rings))]
        public void GenerateRing(string itemName, string power)
        {
            var item = ringGenerator.Generate(power, itemName);
            itemVerifier.AssertItem(item);
        }
    }
}
