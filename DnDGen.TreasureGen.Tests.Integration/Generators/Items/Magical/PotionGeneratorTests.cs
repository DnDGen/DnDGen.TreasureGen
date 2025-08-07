using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Tests.Unit.Generators.Items;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Generators.Items.Magical
{
    [TestFixture]
    public class PotionGeneratorTests : IntegrationTests
    {
        private MagicalItemGenerator potionGenerator;
        private ItemVerifier itemVerifier;

        [SetUp]
        public void Setup()
        {
            itemVerifier = new ItemVerifier();
            potionGenerator = GetNewInstanceOf<MagicalItemGenerator>(ItemTypeConstants.Potion);
        }

        [TestCaseSource(typeof(ItemPowerTestData), nameof(ItemPowerTestData.Potions))]
        public void GeneratePotion(string itemName, string power)
        {
            var item = potionGenerator.Generate(power, itemName);
            itemVerifier.AssertItem(item);
        }

        [TestCase(PotionConstants.MagicCircleAgainstChaos, PowerConstants.Medium)]
        [TestCase(PotionConstants.MagicCircleAgainstEvil, PowerConstants.Medium)]
        [TestCase(PotionConstants.MagicCircleAgainstGood, PowerConstants.Medium)]
        [TestCase(PotionConstants.MagicCircleAgainstLaw, PowerConstants.Medium)]
        public void BUG_GeneratePotion_ReturnsNonTemplatesPotion(string itemName, string power)
        {
            var item = potionGenerator.Generate(power, itemName);
            itemVerifier.AssertItem(item);

            Assert.That(item.Name, Is.EqualTo(itemName));
        }
    }
}
