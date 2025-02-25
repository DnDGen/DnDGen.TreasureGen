using DnDGen.TreasureGen.Items;
using NUnit.Framework;
using System.Linq;

namespace DnDGen.TreasureGen.Tests.Unit.Generators.Items
{
    [TestFixture]
    public class TraitConstantsTests
    {
        [TestCase(TraitConstants.Masterwork, "Masterwork")]
        [TestCase(TraitConstants.SpecialMaterials.Darkwood, "Darkwood")]
        [TestCase(TraitConstants.Sizes.Tiny, "Tiny")]
        [TestCase(TraitConstants.Sizes.Small, "Small")]
        [TestCase(TraitConstants.Sizes.Medium, "Medium")]
        [TestCase(TraitConstants.Sizes.Large, "Large")]
        [TestCase(TraitConstants.Sizes.Huge, "Huge")]
        [TestCase(TraitConstants.Sizes.Gargantuan, "Gargantuan")]
        [TestCase(TraitConstants.Sizes.Colossal, "Colossal")]
        [TestCase(TraitConstants.SpecialMaterials.Adamantine, "Adamantine")]
        [TestCase(TraitConstants.SpecialMaterials.Dragonhide, "Dragonhide")]
        [TestCase(TraitConstants.SpecialMaterials.ColdIron, "Cold iron")]
        [TestCase(TraitConstants.SpecialMaterials.Mithral, "Mithral")]
        [TestCase(TraitConstants.SpecialMaterials.AlchemicalSilver, "Alchemical silver")]
        [TestCase(TraitConstants.Markings, "Markings provide a clue to its function")]
        [TestCase(TraitConstants.ShedsLight, "Sheds light")]
        public void Constant(string constant, string value)
        {
            Assert.That(constant, Is.EqualTo(value));
        }

        [Test]
        public void AllSpecialMaterials()
        {
            var materials = TraitConstants.SpecialMaterials.GetAll();
            Assert.That(materials, Contains.Item(TraitConstants.SpecialMaterials.Mithral));
            Assert.That(materials, Contains.Item(TraitConstants.SpecialMaterials.Adamantine));
            Assert.That(materials, Contains.Item(TraitConstants.SpecialMaterials.AlchemicalSilver));
            Assert.That(materials, Contains.Item(TraitConstants.SpecialMaterials.Darkwood));
            Assert.That(materials, Contains.Item(TraitConstants.SpecialMaterials.ColdIron));
            Assert.That(materials, Contains.Item(TraitConstants.SpecialMaterials.Dragonhide));
            Assert.That(materials.Count(), Is.EqualTo(6));
        }

        [Test]
        public void AllSizes()
        {
            var sizes = TraitConstants.Sizes.GetAll();
            Assert.That(sizes, Contains.Item(TraitConstants.Sizes.Colossal));
            Assert.That(sizes, Contains.Item(TraitConstants.Sizes.Gargantuan));
            Assert.That(sizes, Contains.Item(TraitConstants.Sizes.Huge));
            Assert.That(sizes, Contains.Item(TraitConstants.Sizes.Large));
            Assert.That(sizes, Contains.Item(TraitConstants.Sizes.Medium));
            Assert.That(sizes, Contains.Item(TraitConstants.Sizes.Small));
            Assert.That(sizes, Contains.Item(TraitConstants.Sizes.Tiny));
            Assert.That(sizes.Count(), Is.EqualTo(7));
        }
    }
}