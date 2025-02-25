using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Tests.Unit.Generators.Items;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Generators.Items.Magical
{
    [TestFixture]
    public class ScrollGeneratorTests : IntegrationTests
    {
        private MagicalItemGenerator scrollGenerator;
        private ItemVerifier itemVerifier;

        [SetUp]
        public void Setup()
        {
            itemVerifier = new ItemVerifier();
            scrollGenerator = GetNewInstanceOf<MagicalItemGenerator>(ItemTypeConstants.Scroll);
        }

        [TestCase(ItemTypeConstants.Scroll, PowerConstants.Minor)]
        [TestCase(ItemTypeConstants.Scroll, PowerConstants.Medium)]
        [TestCase(ItemTypeConstants.Scroll, PowerConstants.Major)]
        [TestCase("whatever", PowerConstants.Minor)]
        [TestCase("whatever", PowerConstants.Medium)]
        [TestCase("whatever", PowerConstants.Major)]
        public void GenerateScroll(string itemName, string power)
        {
            var item = scrollGenerator.Generate(power, itemName);
            itemVerifier.AssertItem(item);
        }
    }
}
