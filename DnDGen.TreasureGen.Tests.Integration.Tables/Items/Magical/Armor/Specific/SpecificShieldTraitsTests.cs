using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Armor.Specific
{
    [TestFixture]
    public class SpecificShieldTraitsTests : CollectionsTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Collections.SpecificITEMTYPETraits(AttributeConstants.Shield); }
        }

        [TestCase(ArmorConstants.AbsorbingShield)]
        [TestCase(ArmorConstants.CastersShield)]
        [TestCase(ArmorConstants.Buckler, TraitConstants.SpecialMaterials.Darkwood)]
        [TestCase(ArmorConstants.HeavySteelShield, TraitConstants.SpecialMaterials.Mithral)]
        [TestCase(ArmorConstants.HeavyWoodenShield, TraitConstants.SpecialMaterials.Darkwood)]
        [TestCase(ArmorConstants.LionsShield)]
        [TestCase(ArmorConstants.SpinedShield)]
        [TestCase(ArmorConstants.WingedShield)]
        public void Collection(string name, params string[] attributes)
        {
            base.AssertCollection(name, attributes);
        }
    }
}