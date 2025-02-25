using DnDGen.Infrastructure.Helpers;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Selectors.Selections;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical
{
    [TestFixture]
    public class SpecialAbilityDataTests : CollectionsTests
    {
        protected override string tableName => TableNameConstants.Collections.SpecialAbilityData;

        [Test]
        public void AllAbilitiesPresentInTable()
        {
            var abilities = SpecialAbilityConstants.GetAllAbilities(false);
            AssertCollection(table.Keys, abilities);
        }

        [TestCase(SpecialAbilityConstants.Glamered, 0, SpecialAbilityConstants.Glamered, 0)]
        [TestCase(SpecialAbilityConstants.AcidResistance, 0, SpecialAbilityConstants.AcidResistance, 1)]
        [TestCase(SpecialAbilityConstants.ImprovedAcidResistance, 0, SpecialAbilityConstants.AcidResistance, 2)]
        [TestCase(SpecialAbilityConstants.GreaterAcidResistance, 0, SpecialAbilityConstants.AcidResistance, 3)]
        [TestCase(SpecialAbilityConstants.ColdResistance, 0, SpecialAbilityConstants.ColdResistance, 1)]
        [TestCase(SpecialAbilityConstants.ImprovedColdResistance, 0, SpecialAbilityConstants.ColdResistance, 2)]
        [TestCase(SpecialAbilityConstants.GreaterColdResistance, 0, SpecialAbilityConstants.ColdResistance, 3)]
        [TestCase(SpecialAbilityConstants.ElectricityResistance, 0, SpecialAbilityConstants.ElectricityResistance, 1)]
        [TestCase(SpecialAbilityConstants.ImprovedElectricityResistance, 0, SpecialAbilityConstants.ElectricityResistance, 2)]
        [TestCase(SpecialAbilityConstants.GreaterElectricityResistance, 0, SpecialAbilityConstants.ElectricityResistance, 3)]
        [TestCase(SpecialAbilityConstants.FireResistance, 0, SpecialAbilityConstants.FireResistance, 1)]
        [TestCase(SpecialAbilityConstants.ImprovedFireResistance, 0, SpecialAbilityConstants.FireResistance, 2)]
        [TestCase(SpecialAbilityConstants.GreaterFireResistance, 0, SpecialAbilityConstants.FireResistance, 3)]
        [TestCase(SpecialAbilityConstants.SonicResistance, 0, SpecialAbilityConstants.SonicResistance, 1)]
        [TestCase(SpecialAbilityConstants.ImprovedSonicResistance, 0, SpecialAbilityConstants.SonicResistance, 2)]
        [TestCase(SpecialAbilityConstants.GreaterSonicResistance, 0, SpecialAbilityConstants.SonicResistance, 3)]
        [TestCase(SpecialAbilityConstants.Anarchic, 2, SpecialAbilityConstants.Anarchic, 0)]
        [TestCase(SpecialAbilityConstants.Axiomatic, 2, SpecialAbilityConstants.Axiomatic, 0)]
        [TestCase(SpecialAbilityConstants.Distance, 1, SpecialAbilityConstants.Distance, 0)]
        [TestCase(SpecialAbilityConstants.Flaming, 1, SpecialAbilityConstants.Flaming, 1)]
        [TestCase(SpecialAbilityConstants.Frost, 1, SpecialAbilityConstants.Frost, 1)]
        [TestCase(SpecialAbilityConstants.Merciful, 1, SpecialAbilityConstants.Merciful, 0)]
        [TestCase(SpecialAbilityConstants.Returning, 1, SpecialAbilityConstants.Returning, 0)]
        [TestCase(SpecialAbilityConstants.Shock, 1, SpecialAbilityConstants.Shock, 1)]
        [TestCase(SpecialAbilityConstants.Seeking, 1, SpecialAbilityConstants.Seeking, 0)]
        [TestCase(SpecialAbilityConstants.Thundering, 1, SpecialAbilityConstants.Thundering, 0)]
        [TestCase(SpecialAbilityConstants.FlamingBurst, 2, SpecialAbilityConstants.Flaming, 2)]
        [TestCase(SpecialAbilityConstants.Holy, 2, SpecialAbilityConstants.Holy, 0)]
        [TestCase(SpecialAbilityConstants.IcyBurst, 2, SpecialAbilityConstants.Frost, 2)]
        [TestCase(SpecialAbilityConstants.ShockingBurst, 2, SpecialAbilityConstants.Shock, 2)]
        [TestCase(SpecialAbilityConstants.Unholy, 2, SpecialAbilityConstants.Unholy, 0)]
        [TestCase(SpecialAbilityConstants.Speed, 3, SpecialAbilityConstants.Speed, 0)]
        [TestCase(SpecialAbilityConstants.BrilliantEnergy, 4, SpecialAbilityConstants.BrilliantEnergy, 0)]
        [TestCase(SpecialAbilityConstants.Defending, 1, SpecialAbilityConstants.Defending, 0)]
        [TestCase(SpecialAbilityConstants.GhostTouchWeapon, 1, SpecialAbilityConstants.GhostTouch, 0)]
        [TestCase(SpecialAbilityConstants.Keen, 1, SpecialAbilityConstants.Keen, 0)]
        [TestCase(SpecialAbilityConstants.KiFocus, 1, SpecialAbilityConstants.KiFocus, 0)]
        [TestCase(SpecialAbilityConstants.MightyCleaving, 1, SpecialAbilityConstants.MightyCleaving, 0)]
        [TestCase(SpecialAbilityConstants.SpellStoring, 1, SpecialAbilityConstants.SpellStoring, 0)]
        [TestCase(SpecialAbilityConstants.Throwing, 1, SpecialAbilityConstants.Throwing, 0)]
        [TestCase(SpecialAbilityConstants.Vicious, 1, SpecialAbilityConstants.Vicious, 0)]
        [TestCase(SpecialAbilityConstants.Disruption, 2, SpecialAbilityConstants.Disruption, 0)]
        [TestCase(SpecialAbilityConstants.Dancing, 4, SpecialAbilityConstants.Dancing, 0)]
        [TestCase(SpecialAbilityConstants.Vorpal, 5, SpecialAbilityConstants.Vorpal, 0)]
        [TestCase(SpecialAbilityConstants.Wounding, 2, SpecialAbilityConstants.Wounding, 0)]
        [TestCase(SpecialAbilityConstants.LightFortification, 1, SpecialAbilityConstants.Fortification, 1)]
        [TestCase(SpecialAbilityConstants.ModerateFortification, 3, SpecialAbilityConstants.Fortification, 2)]
        [TestCase(SpecialAbilityConstants.HeavyFortification, 5, SpecialAbilityConstants.Fortification, 3)]
        [TestCase(SpecialAbilityConstants.Slick, 0, SpecialAbilityConstants.Slick, 1)]
        [TestCase(SpecialAbilityConstants.Shadow, 0, SpecialAbilityConstants.Shadow, 1)]
        [TestCase(SpecialAbilityConstants.SilentMoves, 0, SpecialAbilityConstants.SilentMoves, 1)]
        [TestCase(SpecialAbilityConstants.ImprovedSlick, 0, SpecialAbilityConstants.Slick, 2)]
        [TestCase(SpecialAbilityConstants.ImprovedShadow, 0, SpecialAbilityConstants.Shadow, 2)]
        [TestCase(SpecialAbilityConstants.ImprovedSilentMoves, 0, SpecialAbilityConstants.SilentMoves, 2)]
        [TestCase(SpecialAbilityConstants.GreaterSlick, 0, SpecialAbilityConstants.Slick, 3)]
        [TestCase(SpecialAbilityConstants.GreaterShadow, 0, SpecialAbilityConstants.Shadow, 3)]
        [TestCase(SpecialAbilityConstants.GreaterSilentMoves, 0, SpecialAbilityConstants.SilentMoves, 3)]
        [TestCase(SpecialAbilityConstants.SpellResistance13, 2, SpecialAbilityConstants.SpellResistance, 13)]
        [TestCase(SpecialAbilityConstants.SpellResistance15, 3, SpecialAbilityConstants.SpellResistance, 15)]
        [TestCase(SpecialAbilityConstants.SpellResistance17, 4, SpecialAbilityConstants.SpellResistance, 17)]
        [TestCase(SpecialAbilityConstants.SpellResistance19, 5, SpecialAbilityConstants.SpellResistance, 19)]
        [TestCase(SpecialAbilityConstants.GhostTouchArmor, 3, SpecialAbilityConstants.GhostTouch, 0)]
        [TestCase(SpecialAbilityConstants.Invulnerability, 3, SpecialAbilityConstants.Invulnerability, 0)]
        [TestCase(SpecialAbilityConstants.Wild, 3, SpecialAbilityConstants.Wild, 0)]
        [TestCase(SpecialAbilityConstants.Etherealness, 0, SpecialAbilityConstants.Etherealness, 0)]
        [TestCase(SpecialAbilityConstants.UndeadControlling, 0, SpecialAbilityConstants.UndeadControlling, 0)]
        [TestCase(SpecialAbilityConstants.ArrowCatching, 1, SpecialAbilityConstants.ArrowCatching, 0)]
        [TestCase(SpecialAbilityConstants.ArrowDeflection, 2, SpecialAbilityConstants.ArrowDeflection, 0)]
        [TestCase(SpecialAbilityConstants.Bashing, 1, SpecialAbilityConstants.Bashing, 0)]
        [TestCase(SpecialAbilityConstants.Blinding, 1, SpecialAbilityConstants.Blinding, 0)]
        [TestCase(SpecialAbilityConstants.Animated, 2, SpecialAbilityConstants.Animated, 0)]
        [TestCase(SpecialAbilityConstants.Reflecting, 5, SpecialAbilityConstants.Reflecting, 0)]
        [TestCase(SpecialAbilityConstants.Aberrationbane, 1, SpecialAbilityConstants.Bane, 0)]
        [TestCase(SpecialAbilityConstants.Animalbane, 1, SpecialAbilityConstants.Bane, 0)]
        [TestCase(SpecialAbilityConstants.Constructbane, 1, SpecialAbilityConstants.Bane, 0)]
        [TestCase(SpecialAbilityConstants.Dragonbane, 1, SpecialAbilityConstants.Bane, 0)]
        [TestCase(SpecialAbilityConstants.Elementalbane, 1, SpecialAbilityConstants.Bane, 0)]
        [TestCase(SpecialAbilityConstants.Feybane, 1, SpecialAbilityConstants.Bane, 0)]
        [TestCase(SpecialAbilityConstants.Giantbane, 1, SpecialAbilityConstants.Bane, 0)]
        [TestCase(SpecialAbilityConstants.AquaticHumanoidbane, 1, SpecialAbilityConstants.Bane, 0)]
        [TestCase(SpecialAbilityConstants.Dwarfbane, 1, SpecialAbilityConstants.Bane, 0)]
        [TestCase(SpecialAbilityConstants.Elfbane, 1, SpecialAbilityConstants.Bane, 0)]
        [TestCase(SpecialAbilityConstants.Gnollbane, 1, SpecialAbilityConstants.Bane, 0)]
        [TestCase(SpecialAbilityConstants.Gnomebane, 1, SpecialAbilityConstants.Bane, 0)]
        [TestCase(SpecialAbilityConstants.Goblinoidbane, 1, SpecialAbilityConstants.Bane, 0)]
        [TestCase(SpecialAbilityConstants.Halflingbane, 1, SpecialAbilityConstants.Bane, 0)]
        [TestCase(SpecialAbilityConstants.Humanbane, 1, SpecialAbilityConstants.Bane, 0)]
        [TestCase(SpecialAbilityConstants.ReptilianHumanoidbane, 1, SpecialAbilityConstants.Bane, 0)]
        [TestCase(SpecialAbilityConstants.Orcbane, 1, SpecialAbilityConstants.Bane, 0)]
        [TestCase(SpecialAbilityConstants.MagicalBeastbane, 1, SpecialAbilityConstants.Bane, 0)]
        [TestCase(SpecialAbilityConstants.MonstrousHumanoidbane, 1, SpecialAbilityConstants.Bane, 0)]
        [TestCase(SpecialAbilityConstants.Oozebane, 1, SpecialAbilityConstants.Bane, 0)]
        [TestCase(SpecialAbilityConstants.AirOutsiderbane, 1, SpecialAbilityConstants.Bane, 0)]
        [TestCase(SpecialAbilityConstants.ChaoticOutsiderbane, 1, SpecialAbilityConstants.Bane, 0)]
        [TestCase(SpecialAbilityConstants.EarthOutsiderbane, 1, SpecialAbilityConstants.Bane, 0)]
        [TestCase(SpecialAbilityConstants.FireOutsiderbane, 1, SpecialAbilityConstants.Bane, 0)]
        [TestCase(SpecialAbilityConstants.LawfulOutsiderbane, 1, SpecialAbilityConstants.Bane, 0)]
        [TestCase(SpecialAbilityConstants.GoodOutsiderbane, 1, SpecialAbilityConstants.Bane, 0)]
        [TestCase(SpecialAbilityConstants.EvilOutsiderbane, 1, SpecialAbilityConstants.Bane, 0)]
        [TestCase(SpecialAbilityConstants.WaterOutsiderbane, 1, SpecialAbilityConstants.Bane, 0)]
        [TestCase(SpecialAbilityConstants.Plantbane, 1, SpecialAbilityConstants.Bane, 0)]
        [TestCase(SpecialAbilityConstants.Undeadbane, 1, SpecialAbilityConstants.Bane, 0)]
        [TestCase(SpecialAbilityConstants.Verminbane, 1, SpecialAbilityConstants.Bane, 0)]
        [TestCase(SpecialAbilityConstants.Shapeshifterbane, 1, SpecialAbilityConstants.Bane, 0)]
        public void SpecialAbilityData(string name, int bonusEquivalent, string baseName, int power)
        {
            var data = DataHelper.Parse(new SpecialAbilityDataSelection
            {
                BonusEquivalent = bonusEquivalent,
                BaseName = baseName,
                Power = power,
            });

            AssertCollection(name, data);
        }
    }
}