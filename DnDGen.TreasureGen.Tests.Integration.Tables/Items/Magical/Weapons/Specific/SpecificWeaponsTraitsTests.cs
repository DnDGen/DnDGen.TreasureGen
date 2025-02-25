using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Magical.Weapons.Specific
{
    [TestFixture]
    public class SpecificWeaponsTraitsTests : CollectionsTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Collections.SpecificITEMTYPETraits(ItemTypeConstants.Weapon); }
        }

        [TestCase(WeaponConstants.SleepArrow)]
        [TestCase(WeaponConstants.ScreamingBolt)]
        [TestCase(WeaponConstants.Dagger_Silver, TraitConstants.SpecialMaterials.AlchemicalSilver)]
        [TestCase(WeaponConstants.Longsword, TraitConstants.Masterwork, TraitConstants.SpecialMaterials.ColdIron)]
        [TestCase(WeaponConstants.JavelinOfLightning)]
        [TestCase(WeaponConstants.SlayingArrow)]
        [TestCase(WeaponConstants.Dagger_Adamantine, TraitConstants.SpecialMaterials.Adamantine)]
        [TestCase(WeaponConstants.Battleaxe_Adamantine, TraitConstants.SpecialMaterials.Adamantine)]
        [TestCase(WeaponConstants.GreaterSlayingArrow)]
        [TestCase(WeaponConstants.Shatterspike)]
        [TestCase(WeaponConstants.DaggerOfVenom)]
        [TestCase(WeaponConstants.TridentOfWarning)]
        [TestCase(WeaponConstants.AssassinsDagger)]
        [TestCase(WeaponConstants.ShiftersSorrow, TraitConstants.SpecialMaterials.AlchemicalSilver)]
        [TestCase(WeaponConstants.TridentOfFishCommand)]
        [TestCase(WeaponConstants.FlameTongue)]
        [TestCase(WeaponConstants.LuckBlade0)]
        [TestCase(WeaponConstants.SwordOfSubtlety)]
        [TestCase(WeaponConstants.SwordOfThePlanes)]
        [TestCase(WeaponConstants.NineLivesStealer)]
        [TestCase(WeaponConstants.SwordOfLifeStealing)]
        [TestCase(WeaponConstants.Oathbow)]
        [TestCase(WeaponConstants.MaceOfTerror)]
        [TestCase(WeaponConstants.LifeDrinker)]
        [TestCase(WeaponConstants.SylvanScimitar)]
        [TestCase(WeaponConstants.RapierOfPuncturing)]
        [TestCase(WeaponConstants.SunBlade)]
        [TestCase(WeaponConstants.FrostBrand)]
        [TestCase(WeaponConstants.DwarvenThrower)]
        [TestCase(WeaponConstants.LuckBlade1)]
        [TestCase(WeaponConstants.MaceOfSmiting, TraitConstants.SpecialMaterials.Adamantine)]
        [TestCase(WeaponConstants.LuckBlade2)]
        [TestCase(WeaponConstants.HolyAvenger, TraitConstants.SpecialMaterials.ColdIron)]
        [TestCase(WeaponConstants.LuckBlade3)]
        [TestCase(WeaponConstants.LuckBlade)]
        [TestCase(WeaponConstants.CursedBackbiterSpear)]
        [TestCase(WeaponConstants.CursedMinus2Sword)]
        [TestCase(WeaponConstants.BerserkingSword)]
        [TestCase(WeaponConstants.NetOfSnaring)]
        [TestCase(WeaponConstants.MaceOfBlood)]
        public void SpecificWeaponTraits(string name, params string[] attributes)
        {
            base.AssertCollection(name, attributes);
        }
    }
}