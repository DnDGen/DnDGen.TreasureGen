using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical
{
    [TestFixture]
    public class SpecialAbilityAttributeRequirementsTests : CollectionsTests
    {
        protected override string tableName => TableNameConstants.Collections.SpecialAbilityAttributeRequirements;

        [TestCase(SpecialAbilityConstants.Glamered)]
        [TestCase(SpecialAbilityConstants.Fortification)]
        [TestCase(SpecialAbilityConstants.Slick)]
        [TestCase(SpecialAbilityConstants.Shadow)]
        [TestCase(SpecialAbilityConstants.SilentMoves)]
        [TestCase(SpecialAbilityConstants.SpellResistance)]
        [TestCase(SpecialAbilityConstants.AcidResistance)]
        [TestCase(SpecialAbilityConstants.ColdResistance)]
        [TestCase(SpecialAbilityConstants.ElectricityResistance)]
        [TestCase(SpecialAbilityConstants.FireResistance)]
        [TestCase(SpecialAbilityConstants.SonicResistance)]
        [TestCase(SpecialAbilityConstants.GhostTouch)]
        [TestCase(SpecialAbilityConstants.Invulnerability)]
        [TestCase(SpecialAbilityConstants.Wild)]
        [TestCase(SpecialAbilityConstants.Etherealness)]
        [TestCase(SpecialAbilityConstants.UndeadControlling)]
        [TestCase(SpecialAbilityConstants.ArrowCatching,
            AttributeConstants.Shield)]
        [TestCase(SpecialAbilityConstants.Bashing,
            AttributeConstants.Shield,
            "!" + AttributeConstants.TowerShield)]
        [TestCase(SpecialAbilityConstants.Blinding,
            AttributeConstants.Shield)]
        [TestCase(SpecialAbilityConstants.ArrowDeflection,
            AttributeConstants.Shield)]
        [TestCase(SpecialAbilityConstants.Animated,
            AttributeConstants.Shield)]
        [TestCase(SpecialAbilityConstants.Reflecting,
            AttributeConstants.Shield)]
        [TestCase(SpecialAbilityConstants.Bane)]
        [TestCase(SpecialAbilityConstants.Shapeshifterbane)]
        [TestCase(SpecialAbilityConstants.Distance,
            AttributeConstants.Ranged)]
        [TestCase(SpecialAbilityConstants.Disruption,
            AttributeConstants.Melee,
            AttributeConstants.DamageTypes.Bludgeoning)]
        [TestCase(SpecialAbilityConstants.Flaming)]
        [TestCase(SpecialAbilityConstants.FlamingBurst)]
        [TestCase(SpecialAbilityConstants.Frost)]
        [TestCase(SpecialAbilityConstants.IcyBurst)]
        [TestCase(SpecialAbilityConstants.Merciful)]
        [TestCase(SpecialAbilityConstants.Returning,
            AttributeConstants.Ranged,
            AttributeConstants.Thrown)]
        [TestCase(SpecialAbilityConstants.Shock)]
        [TestCase(SpecialAbilityConstants.ShockingBurst)]
        [TestCase(SpecialAbilityConstants.Seeking,
            AttributeConstants.Ranged)]
        [TestCase(SpecialAbilityConstants.Thundering)]
        [TestCase(SpecialAbilityConstants.Anarchic)]
        [TestCase(SpecialAbilityConstants.Axiomatic)]
        [TestCase(SpecialAbilityConstants.Holy)]
        [TestCase(SpecialAbilityConstants.Unholy)]
        [TestCase(SpecialAbilityConstants.Speed)]
        [TestCase(SpecialAbilityConstants.BrilliantEnergy)]
        [TestCase(SpecialAbilityConstants.Keen,
            AttributeConstants.Melee,
            AttributeConstants.DamageTypes.Piercing + "/" + AttributeConstants.DamageTypes.Slashing)]
        [TestCase(SpecialAbilityConstants.KiFocus,
            AttributeConstants.Melee)]
        [TestCase(SpecialAbilityConstants.MightyCleaving,
            AttributeConstants.Melee)]
        [TestCase(SpecialAbilityConstants.SpellStoring,
            AttributeConstants.Melee)]
        [TestCase(SpecialAbilityConstants.Throwing,
            AttributeConstants.Melee)]
        [TestCase(SpecialAbilityConstants.Vicious,
            AttributeConstants.Melee)]
        [TestCase(SpecialAbilityConstants.Defending,
            AttributeConstants.Melee)]
        [TestCase(SpecialAbilityConstants.Wounding,
            AttributeConstants.Melee)]
        [TestCase(SpecialAbilityConstants.Dancing,
            AttributeConstants.Melee)]
        [TestCase(SpecialAbilityConstants.Vorpal,
            AttributeConstants.Melee,
            AttributeConstants.DamageTypes.Slashing)]
        [TestCase(SpecialAbilityConstants.HeavyFortification)]
        [TestCase(SpecialAbilityConstants.LightFortification)]
        [TestCase(SpecialAbilityConstants.ModerateFortification)]
        [TestCase(SpecialAbilityConstants.ImprovedSlick)]
        [TestCase(SpecialAbilityConstants.ImprovedShadow)]
        [TestCase(SpecialAbilityConstants.ImprovedSilentMoves)]
        [TestCase(SpecialAbilityConstants.GreaterSlick)]
        [TestCase(SpecialAbilityConstants.GreaterShadow)]
        [TestCase(SpecialAbilityConstants.GreaterSilentMoves)]
        [TestCase(SpecialAbilityConstants.SpellResistance13)]
        [TestCase(SpecialAbilityConstants.SpellResistance15)]
        [TestCase(SpecialAbilityConstants.SpellResistance17)]
        [TestCase(SpecialAbilityConstants.SpellResistance19)]
        [TestCase(SpecialAbilityConstants.ImprovedAcidResistance)]
        [TestCase(SpecialAbilityConstants.ImprovedColdResistance)]
        [TestCase(SpecialAbilityConstants.ImprovedElectricityResistance)]
        [TestCase(SpecialAbilityConstants.ImprovedFireResistance)]
        [TestCase(SpecialAbilityConstants.ImprovedSonicResistance)]
        [TestCase(SpecialAbilityConstants.GreaterAcidResistance)]
        [TestCase(SpecialAbilityConstants.GreaterColdResistance)]
        [TestCase(SpecialAbilityConstants.GreaterElectricityResistance)]
        [TestCase(SpecialAbilityConstants.GreaterFireResistance)]
        [TestCase(SpecialAbilityConstants.GreaterSonicResistance)]
        [TestCase(SpecialAbilityConstants.GhostTouchArmor)]
        [TestCase(SpecialAbilityConstants.GhostTouchWeapon)]
        [TestCase(SpecialAbilityConstants.Aberrationbane)]
        [TestCase(SpecialAbilityConstants.Animalbane)]
        [TestCase(SpecialAbilityConstants.Constructbane)]
        [TestCase(SpecialAbilityConstants.Dragonbane)]
        [TestCase(SpecialAbilityConstants.Elementalbane)]
        [TestCase(SpecialAbilityConstants.Feybane)]
        [TestCase(SpecialAbilityConstants.Giantbane)]
        [TestCase(SpecialAbilityConstants.AquaticHumanoidbane)]
        [TestCase(SpecialAbilityConstants.Elfbane)]
        [TestCase(SpecialAbilityConstants.Humanbane)]
        [TestCase(SpecialAbilityConstants.Dwarfbane)]
        [TestCase(SpecialAbilityConstants.Gnomebane)]
        [TestCase(SpecialAbilityConstants.Gnollbane)]
        [TestCase(SpecialAbilityConstants.Halflingbane)]
        [TestCase(SpecialAbilityConstants.AirOutsiderbane)]
        [TestCase(SpecialAbilityConstants.ChaoticOutsiderbane)]
        [TestCase(SpecialAbilityConstants.EarthOutsiderbane)]
        [TestCase(SpecialAbilityConstants.EvilOutsiderbane)]
        [TestCase(SpecialAbilityConstants.FireOutsiderbane)]
        [TestCase(SpecialAbilityConstants.GoodOutsiderbane)]
        [TestCase(SpecialAbilityConstants.LawfulOutsiderbane)]
        [TestCase(SpecialAbilityConstants.WaterOutsiderbane)]
        [TestCase(SpecialAbilityConstants.MonstrousHumanoidbane)]
        [TestCase(SpecialAbilityConstants.ReptilianHumanoidbane)]
        [TestCase(SpecialAbilityConstants.MagicalBeastbane)]
        [TestCase(SpecialAbilityConstants.Oozebane)]
        [TestCase(SpecialAbilityConstants.Undeadbane)]
        [TestCase(SpecialAbilityConstants.Verminbane)]
        [TestCase(SpecialAbilityConstants.Goblinoidbane)]
        [TestCase(SpecialAbilityConstants.Orcbane)]
        [TestCase(SpecialAbilityConstants.Plantbane)]
        [TestCase(SpecialAbilityConstants.DESIGNATEDFOEbane)]
        public void SpecialAbilityRequirements(string name, params string[] attributes)
        {
            base.AssertCollection(name, attributes);
        }

        [Test]
        public void AllAbilitiesPresentInTable()
        {
            var abilities = SpecialAbilityConstants.GetAllAbilities(true);
            AssertCollection(table.Keys, abilities);
        }
    }
}