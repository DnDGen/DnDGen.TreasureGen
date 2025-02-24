using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Tests.Unit.Generators.Items;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Generators.Items.Magical
{
    [TestFixture]
    public class WandGeneratorTests : IntegrationTests
    {
        private MagicalItemGenerator wandGenerator;
        private ItemVerifier itemVerifier;

        [SetUp]
        public void Setup()
        {
            itemVerifier = new ItemVerifier();
            wandGenerator = GetNewInstanceOf<MagicalItemGenerator>(ItemTypeConstants.Wand);
        }

        [TestCase("whatever", PowerConstants.Minor)]
        [TestCase("whatever", PowerConstants.Medium)]
        [TestCase("whatever", PowerConstants.Major)]
        public void GenerateWand(string itemName, string power)
        {
            var item = wandGenerator.Generate(power, itemName);
            itemVerifier.AssertItem(item);
        }
    }
}
