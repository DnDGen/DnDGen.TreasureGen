using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Mundane;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Stress.Items.Mundane
{
    [TestFixture]
    public abstract class MundaneItemGeneratorStressTests : ItemStressTests
    {
        protected MundaneItemGenerator mundaneItemGenerator;

        protected void GenerateAndAssertItem()
        {
            var item = GenerateItem();
            AssertItem(item);
        }

        protected override Item GenerateItem()
        {
            return mundaneItemGenerator.GenerateRandom();
        }

        protected void GenerateAndAssertCustomItem()
        {
            var name = GetRandomName();
            var template = ItemVerifier.CreateRandomTemplate(name);

            var item = mundaneItemGenerator.Generate(template);
            AssertItem(item);
            Assert.That(item.Name, Is.EqualTo(name));
            ItemVerifier.AssertMundaneItemFromTemplate(item, template);
        }

        protected void GenerateAndAssertItemFromName()
        {
            var name = GetRandomName();
            var item = GenerateItemFromName(name);
            AssertItem(item);
            Assert.That(item.NameMatches(name), Is.True, $"{item.Name} ({string.Join(", ", item.BaseNames)}) from '{name}'");
        }

        protected override Item GenerateItemFromName(string name, string power = null)
        {
            return mundaneItemGenerator.Generate(name);
        }
    }
}