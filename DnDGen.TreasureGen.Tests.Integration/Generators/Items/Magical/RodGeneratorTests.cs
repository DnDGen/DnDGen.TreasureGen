using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Tests.Unit.Generators.Items;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Generators.Items.Magical
{
    [TestFixture]
    public class RodGeneratorTests : IntegrationTests
    {
        private MagicalItemGenerator rodGenerator;
        private ItemVerifier itemVerifier;

        [SetUp]
        public void Setup()
        {
            itemVerifier = new ItemVerifier();
            rodGenerator = GetNewInstanceOf<MagicalItemGenerator>(ItemTypeConstants.Rod);
        }

        [TestCaseSource(typeof(ItemPowerTestData), nameof(ItemPowerTestData.Rods))]
        public void GenerateRod(string itemName, string power)
        {
            var item = rodGenerator.Generate(power, itemName);
            itemVerifier.AssertItem(item);
        }

        [TestCase(RodConstants.Python, PowerConstants.Medium, TraitConstants.Sizes.Colossal)]
        [TestCase(RodConstants.Python, PowerConstants.Medium, TraitConstants.Sizes.Gargantuan)]
        [TestCase(RodConstants.Python, PowerConstants.Medium, TraitConstants.Sizes.Huge)]
        [TestCase(RodConstants.Python, PowerConstants.Medium, TraitConstants.Sizes.Large)]
        [TestCase(RodConstants.Python, PowerConstants.Medium, TraitConstants.Sizes.Medium)]
        [TestCase(RodConstants.Python, PowerConstants.Medium, TraitConstants.Sizes.Small)]
        [TestCase(RodConstants.Python, PowerConstants.Medium, TraitConstants.Sizes.Tiny)]
        [TestCase(RodConstants.Python, PowerConstants.Major, TraitConstants.Sizes.Colossal)]
        [TestCase(RodConstants.Python, PowerConstants.Major, TraitConstants.Sizes.Gargantuan)]
        [TestCase(RodConstants.Python, PowerConstants.Major, TraitConstants.Sizes.Huge)]
        [TestCase(RodConstants.Python, PowerConstants.Major, TraitConstants.Sizes.Large)]
        [TestCase(RodConstants.Python, PowerConstants.Major, TraitConstants.Sizes.Medium)]
        [TestCase(RodConstants.Python, PowerConstants.Major, TraitConstants.Sizes.Small)]
        [TestCase(RodConstants.Python, PowerConstants.Major, TraitConstants.Sizes.Tiny)]
        public void GenerateRodOfSize(string itemName, string power, string size)
        {
            var item = rodGenerator.Generate(power, itemName, "my trait", size);
            itemVerifier.AssertItem(item);
            Assert.That(item, Is.InstanceOf<Weapon>());

            var weapon = item as Weapon;
            Assert.That(weapon.Size, Is.EqualTo(size));
            Assert.That(weapon.Magic.Bonus, Is.Positive);
        }
    }
}
