﻿using NUnit.Framework;
using TreasureGen.Domain.Tables;
using TreasureGen.Items;

namespace TreasureGen.Tests.Integration.Tables.Items.Magical.Weapons.Specific
{
    [TestFixture]
    public class SpecificWeaponsTraitsTests : CollectionsTests
    {
        protected override string tableName
        {
            get { return string.Format(TableNameConstants.Collections.Formattable.SpecificITEMTYPETraits, ItemTypeConstants.Weapon); }
        }

        [TestCase(WeaponConstants.SleepArrow)]
        [TestCase(WeaponConstants.ScreamingBolt)]
        [TestCase(WeaponConstants.SilverDagger, TraitConstants.SpecialMaterials.AlchemicalSilver)]
        [TestCase(WeaponConstants.Longsword, TraitConstants.Masterwork, TraitConstants.SpecialMaterials.ColdIron)]
        [TestCase(WeaponConstants.JavelinOfLightning)]
        [TestCase(WeaponConstants.SlayingArrow)]
        [TestCase(WeaponConstants.Dagger, TraitConstants.SpecialMaterials.Adamantine)]
        [TestCase(WeaponConstants.Battleaxe, TraitConstants.SpecialMaterials.Adamantine)]
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
            base.Collections(name, attributes);
        }
    }
}