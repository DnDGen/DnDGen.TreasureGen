using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Armor
{
    [TestFixture]
    public class SpecificArmorsAttributesTests : CollectionsTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Collections.SpecificITEMTYPEAttributes(ItemTypeConstants.Armor); }
        }

        [TestCase(ArmorConstants.BandedMailOfLuck,
            AttributeConstants.Specific,
            AttributeConstants.Metal)]
        [TestCase(ArmorConstants.BreastplateOfCommand,
            AttributeConstants.Specific,
            AttributeConstants.Metal)]
        [TestCase(ArmorConstants.CelestialArmor,
            AttributeConstants.Specific,
            AttributeConstants.Metal)]
        [TestCase(ArmorConstants.DemonArmor,
            AttributeConstants.Specific,
            AttributeConstants.Metal)]
        [TestCase(ArmorConstants.DwarvenPlate,
            AttributeConstants.Specific,
            AttributeConstants.Metal)]
        [TestCase(ArmorConstants.ElvenChain,
            AttributeConstants.Specific,
            AttributeConstants.Metal)]
        [TestCase(ArmorConstants.FullPlateOfSpeed,
            AttributeConstants.Specific,
            AttributeConstants.Metal)]
        [TestCase(ArmorConstants.PlateArmorOfTheDeep,
            AttributeConstants.Specific,
            AttributeConstants.Metal)]
        [TestCase(ArmorConstants.RhinoHide,
            AttributeConstants.Specific)]
        [TestCase(ArmorConstants.ChainShirt,
            AttributeConstants.Specific,
            AttributeConstants.Metal)]
        [TestCase(ArmorConstants.FullPlate,
            AttributeConstants.Specific)]
        [TestCase(ArmorConstants.Breastplate,
            AttributeConstants.Specific,
            AttributeConstants.Metal)]
        [TestCase(ArmorConstants.ArmorOfRage,
            AttributeConstants.Metal,
            AttributeConstants.Specific)]
        [TestCase(ArmorConstants.ArmorOfArrowAttraction,
            AttributeConstants.Specific,
            AttributeConstants.Metal)]
        public void SpecificArmorAttributes(string name, params string[] attributes)
        {
            base.AssertCollection(name, attributes);
        }

        [TestCase(ArmorConstants.ArmorOfRage)]
        [TestCase(ArmorConstants.ArmorOfArrowAttraction)]
        public void SpecificCursedArmorMatchesAttributes(string item)
        {
            var specificCursedAttributes = CollectionMapper.Map(Name, TableNameConstants.Collections.SpecificCursedItemAttributes);
            Assert.That(table[item], Is.EquivalentTo(specificCursedAttributes[item]));
        }
    }
}