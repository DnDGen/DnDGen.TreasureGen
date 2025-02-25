using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Armor.Specific
{
    [TestFixture]
    public class SpecificShieldAttributesTests : CollectionsTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Collections.SpecificITEMTYPEAttributes(AttributeConstants.Shield); }
        }

        [TestCase(ArmorConstants.AbsorbingShield,
            AttributeConstants.Shield,
            AttributeConstants.Specific,
            AttributeConstants.Metal)]
        [TestCase(ArmorConstants.Buckler,
            AttributeConstants.Shield,
            AttributeConstants.Specific,
            AttributeConstants.Wood)]
        [TestCase(ArmorConstants.HeavyWoodenShield,
            AttributeConstants.Shield,
            AttributeConstants.Specific,
            AttributeConstants.Wood)]
        [TestCase(ArmorConstants.HeavySteelShield,
            AttributeConstants.Shield,
            AttributeConstants.Specific,
            AttributeConstants.Metal)]
        [TestCase(ArmorConstants.CastersShield,
            AttributeConstants.Shield,
            AttributeConstants.Specific,
            AttributeConstants.Wood)]
        [TestCase(ArmorConstants.SpinedShield,
            AttributeConstants.Shield,
            AttributeConstants.Specific,
            AttributeConstants.Metal)]
        [TestCase(ArmorConstants.LionsShield,
            AttributeConstants.Shield,
            AttributeConstants.Specific,
            AttributeConstants.Metal)]
        [TestCase(ArmorConstants.WingedShield,
            AttributeConstants.Shield,
            AttributeConstants.Specific,
            AttributeConstants.Wood)]
        public void SpecificShieldAttributes(string name, params string[] attributes)
        {
            base.AssertCollection(name, attributes);
        }
    }
}