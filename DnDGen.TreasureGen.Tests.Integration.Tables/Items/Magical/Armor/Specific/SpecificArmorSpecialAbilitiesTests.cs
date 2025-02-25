using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Armor.Specific
{
    [TestFixture]
    public class SpecificArmorSpecialAbilitiesTests : CollectionsTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Collections.SpecificITEMTYPESpecialAbilities(ItemTypeConstants.Armor); }
        }

        [TestCase(ArmorConstants.BandedMailOfLuck)]
        [TestCase(ArmorConstants.BreastplateOfCommand)]
        [TestCase(ArmorConstants.CelestialArmor)]
        [TestCase(ArmorConstants.DemonArmor)]
        [TestCase(ArmorConstants.DwarvenPlate)]
        [TestCase(ArmorConstants.ElvenChain)]
        [TestCase(ArmorConstants.FullPlateOfSpeed)]
        [TestCase(ArmorConstants.PlateArmorOfTheDeep)]
        [TestCase(ArmorConstants.RhinoHide)]
        [TestCase(ArmorConstants.ChainShirt)]
        [TestCase(ArmorConstants.FullPlate)]
        [TestCase(ArmorConstants.Breastplate)]
        [TestCase(ArmorConstants.ArmorOfRage)]
        [TestCase(ArmorConstants.ArmorOfArrowAttraction)]
        public void SpecificArmorSpecialAbilities(string name, params string[] attributes)
        {
            base.AssertCollection(name, attributes);
        }
    }
}