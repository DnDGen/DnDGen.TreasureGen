using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Weapons.Specific
{
    [TestFixture]
    public class SpecificWeaponsSpecialAbilitiesTests : CollectionsTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Collections.SpecificITEMTYPESpecialAbilities(ItemTypeConstants.Weapon); }
        }

        [TestCase(WeaponConstants.SleepArrow)]
        [TestCase(WeaponConstants.ScreamingBolt)]
        [TestCase(WeaponConstants.Dagger_Silver)]
        [TestCase(WeaponConstants.Dagger_Adamantine)]
        [TestCase(WeaponConstants.Battleaxe_Adamantine)]
        [TestCase(WeaponConstants.Longsword)]
        [TestCase(WeaponConstants.JavelinOfLightning)]
        [TestCase(WeaponConstants.SlayingArrow)]
        [TestCase(WeaponConstants.GreaterSlayingArrow)]
        [TestCase(WeaponConstants.Shatterspike)]
        [TestCase(WeaponConstants.DaggerOfVenom)]
        [TestCase(WeaponConstants.TridentOfWarning)]
        [TestCase(WeaponConstants.AssassinsDagger)]
        [TestCase(WeaponConstants.ShiftersSorrow, SpecialAbilityConstants.Shapeshifterbane)]
        [TestCase(WeaponConstants.TridentOfFishCommand)]
        [TestCase(WeaponConstants.FlameTongue, SpecialAbilityConstants.FlamingBurst)]
        [TestCase(WeaponConstants.LuckBlade0)]
        [TestCase(WeaponConstants.SwordOfSubtlety)]
        [TestCase(WeaponConstants.SwordOfThePlanes)]
        [TestCase(WeaponConstants.NineLivesStealer)]
        [TestCase(WeaponConstants.SwordOfLifeStealing)]
        [TestCase(WeaponConstants.Oathbow)]
        [TestCase(WeaponConstants.MaceOfTerror)]
        [TestCase(WeaponConstants.LifeDrinker)]
        [TestCase(WeaponConstants.SylvanScimitar)]
        [TestCase(WeaponConstants.RapierOfPuncturing, SpecialAbilityConstants.Wounding)]
        [TestCase(WeaponConstants.SunBlade)]
        [TestCase(WeaponConstants.FrostBrand, SpecialAbilityConstants.Frost)]
        [TestCase(WeaponConstants.DwarvenThrower)]
        [TestCase(WeaponConstants.LuckBlade1)]
        [TestCase(WeaponConstants.MaceOfSmiting)]
        [TestCase(WeaponConstants.LuckBlade2)]
        [TestCase(WeaponConstants.HolyAvenger)]
        [TestCase(WeaponConstants.LuckBlade3)]
        [TestCase(WeaponConstants.LuckBlade)]
        [TestCase(WeaponConstants.CursedBackbiterSpear)]
        [TestCase(WeaponConstants.CursedMinus2Sword)]
        [TestCase(WeaponConstants.BerserkingSword)]
        [TestCase(WeaponConstants.NetOfSnaring)]
        [TestCase(WeaponConstants.MaceOfBlood)]
        public void SpecificWeaponSpecialAbilities(string name, params string[] attributes)
        {
            base.AssertCollection(name, attributes);
        }
    }
}