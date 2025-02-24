using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Tests.Unit.Generators.Items;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Generators.Items.Magical
{
    [TestFixture]
    public class StaffGeneratorTests : IntegrationTests
    {
        private MagicalItemGenerator staffGenerator;
        private ItemVerifier itemVerifier;

        [SetUp]
        public void Setup()
        {
            itemVerifier = new ItemVerifier();
            staffGenerator = GetNewInstanceOf<MagicalItemGenerator>(ItemTypeConstants.Staff);
        }

        [TestCaseSource(typeof(ItemPowerTestData), nameof(ItemPowerTestData.Staffs))]
        public void GenerateStaff(string itemName, string power)
        {
            var item = staffGenerator.Generate(power, itemName);
            itemVerifier.AssertItem(item);
        }

        [TestCase(StaffConstants.Woodlands, PowerConstants.Major, TraitConstants.Sizes.Gargantuan)]
        [TestCase(StaffConstants.Woodlands, PowerConstants.Major, TraitConstants.Sizes.Huge)]
        [TestCase(StaffConstants.Woodlands, PowerConstants.Major, TraitConstants.Sizes.Large)]
        [TestCase(StaffConstants.Woodlands, PowerConstants.Major, TraitConstants.Sizes.Medium)]
        [TestCase(StaffConstants.Woodlands, PowerConstants.Major, TraitConstants.Sizes.Small)]
        [TestCase(StaffConstants.Woodlands, PowerConstants.Major, TraitConstants.Sizes.Tiny)]
        public void GenerateWeaponOfSize(string itemName, string power, string size)
        {
            var item = staffGenerator.Generate(power, itemName, "my trait", size);
            itemVerifier.AssertItem(item);
            Assert.That(item, Is.InstanceOf<Weapon>());

            var weapon = item as Weapon;
            Assert.That(weapon.Size, Is.EqualTo(size));
            Assert.That(weapon.Magic.Bonus, Is.Positive);
        }
    }
}
