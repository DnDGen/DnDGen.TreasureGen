using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Rods
{
    [TestFixture]
    public class RodAttributesTests : CollectionsTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Collections.ITEMTYPEAttributes(ItemTypeConstants.Rod); }
        }

        [TestCase(RodConstants.Metamagic_Enlarge_Lesser)]
        [TestCase(RodConstants.Metamagic_Extend_Lesser)]
        [TestCase(RodConstants.Metamagic_Silent_Lesser)]
        [TestCase(RodConstants.Metamagic_Empower_Lesser)]
        [TestCase(RodConstants.Metamagic_Maximize_Lesser)]
        [TestCase(RodConstants.Metamagic_Quicken_Lesser)]
        [TestCase(RodConstants.Metamagic_Enlarge)]
        [TestCase(RodConstants.Metamagic_Extend)]
        [TestCase(RodConstants.Metamagic_Silent)]
        [TestCase(RodConstants.Metamagic_Empower)]
        [TestCase(RodConstants.Metamagic_Maximize)]
        [TestCase(RodConstants.Metamagic_Quicken)]
        [TestCase(RodConstants.Metamagic_Enlarge_Greater)]
        [TestCase(RodConstants.Metamagic_Extend_Greater)]
        [TestCase(RodConstants.Metamagic_Silent_Greater)]
        [TestCase(RodConstants.Metamagic_Empower_Greater)]
        [TestCase(RodConstants.Metamagic_Maximize_Greater)]
        [TestCase(RodConstants.Metamagic_Quicken_Greater)]
        [TestCase(RodConstants.ImmovableRod)]
        [TestCase(RodConstants.MetalAndMineralDetection)]
        [TestCase(RodConstants.Cancellation)]
        [TestCase(RodConstants.Wonder)]
        [TestCase(RodConstants.Python,
            AttributeConstants.DoubleWeapon,
            AttributeConstants.Simple,
            AttributeConstants.Melee,
            AttributeConstants.Wood,
            AttributeConstants.TwoHanded,
            AttributeConstants.Specific)]
        [TestCase(RodConstants.Viper,
            AttributeConstants.Simple,
            AttributeConstants.Melee,
            AttributeConstants.Metal,
            AttributeConstants.OneHanded,
            AttributeConstants.Specific)]
        [TestCase(RodConstants.FlameExtinguishing)]
        [TestCase(RodConstants.EnemyDetection)]
        [TestCase(RodConstants.Splendor)]
        [TestCase(RodConstants.Withering,
            AttributeConstants.Simple,
            AttributeConstants.Melee,
            AttributeConstants.Metal,
            AttributeConstants.Light,
            AttributeConstants.Specific)]
        [TestCase(RodConstants.ThunderAndLightning,
            AttributeConstants.Simple,
            AttributeConstants.Melee,
            AttributeConstants.Metal,
            AttributeConstants.Light,
            AttributeConstants.Specific)]
        [TestCase(RodConstants.Negation)]
        [TestCase(RodConstants.Flailing,
            AttributeConstants.DoubleWeapon,
            AttributeConstants.Melee,
            AttributeConstants.Metal,
            AttributeConstants.Specific,
            AttributeConstants.TwoHanded,
            AttributeConstants.Exotic)]
        [TestCase(RodConstants.Absorption,
            AttributeConstants.Charged,
            AttributeConstants.OneTimeUse)]
        [TestCase(RodConstants.Rulership,
            AttributeConstants.Charged,
            AttributeConstants.OneTimeUse)]
        [TestCase(RodConstants.Security)]
        [TestCase(RodConstants.LordlyMight,
            AttributeConstants.Simple,
            AttributeConstants.Melee,
            AttributeConstants.Metal,
            AttributeConstants.Light,
            AttributeConstants.Specific)]
        [TestCase(RodConstants.Alertness,
            AttributeConstants.Simple,
            AttributeConstants.Melee,
            AttributeConstants.Metal,
            AttributeConstants.Light,
            AttributeConstants.Specific)]
        public void RodAttributes(string name, params string[] attributes)
        {
            base.AssertCollection(name, attributes);
        }

        [TestCase(RodConstants.Alertness, WeaponConstants.LightMace)]
        [TestCase(RodConstants.Flailing, WeaponConstants.DireFlail)]
        [TestCase(RodConstants.LordlyMight, WeaponConstants.LightMace)]
        [TestCase(RodConstants.Python, WeaponConstants.Quarterstaff)]
        [TestCase(RodConstants.ThunderAndLightning, WeaponConstants.LightMace)]
        [TestCase(RodConstants.Viper, WeaponConstants.HeavyMace)]
        [TestCase(RodConstants.Withering, WeaponConstants.LightMace)]
        public void AttributesMatchWeapon(string rod, string weapon)
        {
            var rodAttributes = table[rod];

            var weaponAttributesTableName = TableNameConstants.Collections.ITEMTYPEAttributes(ItemTypeConstants.Weapon);
            var weaponAttributesTable = CollectionMapper.Map(Name, weaponAttributesTableName);
            var weaponAttributes = weaponAttributesTable[weapon];

            Assert.That(rodAttributes, Is.SupersetOf(weaponAttributes));
        }
    }
}