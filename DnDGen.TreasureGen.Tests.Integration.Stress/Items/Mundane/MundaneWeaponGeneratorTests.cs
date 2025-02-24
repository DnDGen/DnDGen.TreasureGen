using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Mundane;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.TreasureGen.Tests.Integration.Stress.Items.Mundane
{
    [TestFixture]
    public class MundaneWeaponGeneratorTests : MundaneItemGeneratorStressTests
    {
        [SetUp]
        public void Setup()
        {
            mundaneItemGenerator = GetNewInstanceOf<MundaneItemGenerator>(ItemTypeConstants.Weapon);
        }

        [Test]
        public void StressWeapon()
        {
            stressor.Stress(GenerateAndAssertItem);
        }

        protected override void MakeSpecificAssertionsAgainst(Item item)
        {
            Assert.That(item.Name, Is.Not.Empty);
            Assert.That(item.ItemType, Is.EqualTo(ItemTypeConstants.Weapon), item.Name);
            Assert.That(item.Quantity, Is.Positive, item.Name);
            Assert.That(item.IsMagical, Is.False, item.Name);
            Assert.That(item.Attributes, Contains.Item(AttributeConstants.Simple)
                .Or.Contains(AttributeConstants.Martial)
                .Or.Contains(AttributeConstants.Exotic), item.Name);
            Assert.That(item.Attributes, Contains.Item(AttributeConstants.Light)
                .Or.Contains(AttributeConstants.OneHanded)
                .Or.Contains(AttributeConstants.TwoHanded)
                .Or.Contains(AttributeConstants.Ranged), item.Name);
            Assert.That(item.Attributes, Contains.Item(AttributeConstants.Melee)
                .Or.Contains(AttributeConstants.Ranged), item.Name);

            var sizes = TraitConstants.Sizes.GetAll();
            Assert.That(item.Traits.Intersect(sizes), Is.Empty);
            Assert.That(item, Is.InstanceOf<Weapon>(), item.Name);

            var weapon = item as Weapon;
            Assert.That(weapon.Size, Is.Not.Empty);
            Assert.That(sizes, Contains.Item(weapon.Size), item.Name);
        }

        protected override IEnumerable<string> GetItemNames()
        {
            return WeaponConstants.GetAllWeapons(false, false);
        }

        [Test]
        public void StressCustomWeapon()
        {
            stressor.Stress(GenerateAndAssertCustomItem);
        }

        [Test]
        public void StressMundaneWeaponFromName()
        {
            stressor.Stress(GenerateAndAssertItemFromName);
        }
    }
}