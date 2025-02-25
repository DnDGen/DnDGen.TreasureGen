using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Mundane
{
    [TestFixture]
    public class SpecialMaterialsTests : CollectionsTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Collections.SpecialMaterials; }
        }

        [TestCase(TraitConstants.SpecialMaterials.Adamantine, AttributeConstants.Metal)]
        [TestCase(TraitConstants.SpecialMaterials.AlchemicalSilver,
            AttributeConstants.Metal,
            ItemTypeConstants.Weapon)]
        [TestCase(TraitConstants.SpecialMaterials.ColdIron,
            AttributeConstants.Metal,
            ItemTypeConstants.Weapon)]
        [TestCase(TraitConstants.SpecialMaterials.Darkwood, AttributeConstants.Wood)]
        [TestCase(TraitConstants.SpecialMaterials.Dragonhide, ItemTypeConstants.Armor)]
        [TestCase(TraitConstants.SpecialMaterials.Mithral, AttributeConstants.Metal)]
        [TestCase(TraitConstants.Masterwork,
            TraitConstants.SpecialMaterials.Adamantine,
            TraitConstants.SpecialMaterials.Darkwood,
            TraitConstants.SpecialMaterials.Dragonhide,
            TraitConstants.SpecialMaterials.Mithral)]
        public void Collection(string name, params string[] attributes)
        {
            base.AssertCollection(name, attributes);
        }

        [Test]
        public void TableContainsAllSpecialMaterials()
        {
            var materials = TraitConstants.SpecialMaterials.GetAll();
            var table = CollectionMapper.Map(Name, tableName);
            Assert.That(table.Keys, Is.SupersetOf(materials));
        }
    }
}