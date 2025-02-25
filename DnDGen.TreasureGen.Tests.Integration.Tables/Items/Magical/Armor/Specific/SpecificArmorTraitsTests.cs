using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Armor.Specific
{
    [TestFixture]
    public class SpecificArmorTraitsTests : CollectionsTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Collections.SpecificITEMTYPETraits(ItemTypeConstants.Armor); }
        }

        [TestCase(ArmorConstants.BandedMailOfLuck)]
        [TestCase(ArmorConstants.BreastplateOfCommand)]
        [TestCase(ArmorConstants.CelestialArmor)]
        [TestCase(ArmorConstants.DemonArmor)]
        [TestCase(ArmorConstants.DwarvenPlate, TraitConstants.SpecialMaterials.Adamantine)]
        [TestCase(ArmorConstants.ElvenChain, TraitConstants.SpecialMaterials.Mithral)]
        [TestCase(ArmorConstants.FullPlateOfSpeed, TraitConstants.SpecialMaterials.Mithral)]
        [TestCase(ArmorConstants.PlateArmorOfTheDeep)]
        [TestCase(ArmorConstants.RhinoHide)]
        [TestCase(ArmorConstants.ArmorOfRage)]
        [TestCase(ArmorConstants.ArmorOfArrowAttraction)]
        [TestCase(ArmorConstants.ChainShirt, TraitConstants.SpecialMaterials.Mithral)]
        [TestCase(ArmorConstants.FullPlate, TraitConstants.SpecialMaterials.Dragonhide)]
        [TestCase(ArmorConstants.Breastplate, TraitConstants.SpecialMaterials.Adamantine)]
        public void SpecificArmorTraits(string name, params string[] attributes)
        {
            base.AssertCollection(name, attributes);
        }
    }
}