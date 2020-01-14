﻿using NUnit.Framework;
using DnDGen.TreasureGen.Tables;
using DnDGen.TreasureGen.Items;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Armor
{
    [TestFixture]
    public class SpecificArmorsAttributesTests : CollectionsTests
    {
        protected override string tableName
        {
            get { return string.Format(TableNameConstants.Collections.Formattable.SpecificITEMTYPEAttributes, ItemTypeConstants.Armor); }
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
            base.Collections(name, attributes);
        }

        [TestCase(ArmorConstants.ArmorOfRage)]
        [TestCase(ArmorConstants.ArmorOfArrowAttraction)]
        public void SpecificCursedArmorMatchesAttributes(string item)
        {
            var specificCursedAttributes = CollectionMapper.Map(TableNameConstants.Collections.Set.SpecificCursedItemAttributes);
            var specificAttributes = GetCollection(item);

            Assert.That(specificAttributes, Is.EquivalentTo(specificCursedAttributes[item]));
        }
    }
}