﻿using System;
using TreasureGen.Common.Items;
using TreasureGen.Tables;
using NUnit.Framework;

namespace TreasureGen.Tests.Integration.Tables.Items.Magical.Curses
{
    [TestFixture]
    public class SpecificCursedItemItemTypesTests : AttributesTests
    {
        protected override String tableName
        {
            get { return TableNameConstants.Attributes.Set.SpecificCursedItemItemTypes; }
        }

        [TestCase(WondrousItemConstants.IncenseOfObsession, ItemTypeConstants.WondrousItem)]
        [TestCase(RingConstants.Clumsiness, ItemTypeConstants.Ring)]
        [TestCase(WondrousItemConstants.AmuletOfInescapableLocation, ItemTypeConstants.WondrousItem)]
        [TestCase(WondrousItemConstants.StoneOfWeight_Loadstone, ItemTypeConstants.WondrousItem)]
        [TestCase(WondrousItemConstants.BracersOfDefenselessness, ItemTypeConstants.WondrousItem)]
        [TestCase(WondrousItemConstants.GauntletsOfFumbling, ItemTypeConstants.WondrousItem)]
        [TestCase(WeaponConstants.CursedMinus2Sword, ItemTypeConstants.Weapon)]
        [TestCase(ArmorConstants.ArmorOfRage, ItemTypeConstants.Armor)]
        [TestCase(WondrousItemConstants.MedallionOfThoughtProjection, ItemTypeConstants.WondrousItem)]
        [TestCase(WondrousItemConstants.FlaskOfCurses, ItemTypeConstants.WondrousItem)]
        [TestCase(WondrousItemConstants.DustOfSneezingAndChoking, ItemTypeConstants.WondrousItem)]
        [TestCase(PotionConstants.Poison, ItemTypeConstants.Potion)]
        [TestCase(WondrousItemConstants.RobeOfPowerlessness, ItemTypeConstants.WondrousItem)]
        [TestCase(WeaponConstants.CursedBackbiterSpear, ItemTypeConstants.Weapon)]
        [TestCase(ArmorConstants.ArmorOfArrowAttraction, ItemTypeConstants.Armor)]
        [TestCase(WeaponConstants.NetOfSnaring, ItemTypeConstants.Weapon)]
        [TestCase(WondrousItemConstants.BagOfDevouring, ItemTypeConstants.LivingCreature)]
        [TestCase(WeaponConstants.MaceOfBlood, ItemTypeConstants.Weapon)]
        [TestCase(WondrousItemConstants.RobeOfVermin, ItemTypeConstants.WondrousItem)]
        [TestCase(WondrousItemConstants.PeriaptOfFoulRotting, ItemTypeConstants.WondrousItem)]
        [TestCase(WeaponConstants.BerserkingSword, ItemTypeConstants.Weapon)]
        [TestCase(WondrousItemConstants.BootsOfDancing, ItemTypeConstants.WondrousItem)]
        [TestCase(WondrousItemConstants.HelmOfOppositeAlignment, ItemTypeConstants.WondrousItem)]
        [TestCase(WondrousItemConstants.BroomOfAnimatedAttack, ItemTypeConstants.WondrousItem)]
        [TestCase(WondrousItemConstants.VacousGrimoire, ItemTypeConstants.WondrousItem)]
        [TestCase(WondrousItemConstants.CrystalBall_Hypnosis, ItemTypeConstants.WondrousItem)]
        [TestCase(WondrousItemConstants.NecklaceOfStrangulation, ItemTypeConstants.WondrousItem)]
        [TestCase(WondrousItemConstants.CloakOfPoisonousness, ItemTypeConstants.WondrousItem)]
        [TestCase(WondrousItemConstants.ScarabOfDeath, ItemTypeConstants.WondrousItem)]
        public override void Attributes(String name, params String[] attributes)
        {
            base.Attributes(name, attributes);
        }
    }
}