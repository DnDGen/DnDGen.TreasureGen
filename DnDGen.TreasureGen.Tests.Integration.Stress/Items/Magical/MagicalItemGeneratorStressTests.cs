using DnDGen.RollGen;
using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.TreasureGen.Tests.Integration.Stress.Items.Magical
{
    [TestFixture]
    public abstract class MagicalItemGeneratorStressTests : ItemStressTests
    {
        protected abstract string itemType { get; }
        protected abstract bool allowMinor { get; }

        protected IEnumerable<string> materials;
        protected MagicalItemGenerator magicalItemGenerator;
        private IEnumerable<string> specialAbilities;
        private Dice dice;

        [SetUp]
        public void MundaneItemGeneratorStressSetup()
        {
            magicalItemGenerator = GetNewInstanceOf<MagicalItemGenerator>(itemType);
            dice = GetNewInstanceOf<Dice>();

            materials = TraitConstants.SpecialMaterials.GetAll();
            specialAbilities = SpecialAbilityConstants.GetAllAbilities(false);
        }

        protected void GenerateAndAssertItem()
        {
            var item = GenerateItem(i => i.ItemType == itemType);
            AssertItem(item);
        }

        protected override Item GenerateItem()
        {
            var power = GetNewPower(allowMinor);
            return magicalItemGenerator.GenerateRandom(power);
        }

        private Item GetRandomTemplate(string name)
        {
            var template = ItemVerifier.CreateRandomTemplate(name);

            var abilitiesCount = dice.Roll().d10().AsSum();
            var abilityNames = new HashSet<string>();

            while (abilityNames.Count < abilitiesCount)
            {
                var abilityName = collectionSelector.SelectRandomFrom(specialAbilities);
                abilityNames.Add(abilityName);
            }

            template.Magic.SpecialAbilities = abilityNames.Select(n => new SpecialAbility { Name = n });

            return template;
        }

        protected void GenerateAndAssertCustomItem()
        {
            var name = GetRandomName();
            var template = GetRandomTemplate(name);
            var allowDecoration = dice.Roll().d2().AsTrueOrFalse();

            var item = magicalItemGenerator.Generate(template, allowDecoration);
            AssertItem(item);
            Assert.That(item.ItemType, Is.EqualTo(itemType));

            if (!allowDecoration)
                ItemVerifier.AssertMagicalItemFromTemplate(item, template);
        }

        protected void GenerateAndAssertItemFromName()
        {
            var name = GetRandomName();
            var item = GenerateItemFromName(name);

            AssertItem(item);
            Assert.That(item.ItemType, Is.EqualTo(itemType));
        }

        protected override Item GenerateItemFromName(string name, string power = null)
        {
            power ??= GetNewPower(allowMinor);

            return magicalItemGenerator.Generate(power, name);
        }
    }
}