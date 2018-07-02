﻿using NUnit.Framework;
using TreasureGen.Tables;
using TreasureGen.Items;
using TreasureGen.Items.Magical;

namespace TreasureGen.Tests.Integration.Tables.Items.Magical.Armor.Major
{
    [TestFixture]
    public class MajorArmorSpecialAbilitiesTests : PercentileTests
    {
        protected override string tableName
        {
            get { return string.Format(TableNameConstants.Percentiles.Formattable.POWERATTRIBUTESpecialAbilities, PowerConstants.Major, ItemTypeConstants.Armor); }
        }

        [Test]
        public override void ReplacementStringsAreValid()
        {
            AssertReplacementStringsAreValid();
        }

        [TestCase(SpecialAbilityConstants.Glamered, 1, 3)]
        [TestCase(SpecialAbilityConstants.ImprovedSlick, 5, 7)]
        [TestCase(SpecialAbilityConstants.ImprovedShadow, 8, 10)]
        [TestCase(SpecialAbilityConstants.ImprovedSilentMoves, 11, 13)]
        [TestCase(SpecialAbilityConstants.AcidResistance, 14, 16)]
        [TestCase(SpecialAbilityConstants.ColdResistance, 17, 19)]
        [TestCase(SpecialAbilityConstants.ElectricityResistance, 20, 22)]
        [TestCase(SpecialAbilityConstants.FireResistance, 23, 25)]
        [TestCase(SpecialAbilityConstants.SonicResistance, 26, 28)]
        [TestCase(SpecialAbilityConstants.GhostTouchArmor, 29, 33)]
        [TestCase(SpecialAbilityConstants.Invulnerability, 34, 35)]
        [TestCase(SpecialAbilityConstants.ModerateFortification, 36, 40)]
        [TestCase(SpecialAbilityConstants.SpellResistance15, 41, 42)]
        [TestCase(SpecialAbilityConstants.GreaterSlick, 44, 48)]
        [TestCase(SpecialAbilityConstants.GreaterShadow, 49, 53)]
        [TestCase(SpecialAbilityConstants.GreaterSilentMoves, 54, 58)]
        [TestCase(SpecialAbilityConstants.ImprovedAcidResistance, 59, 63)]
        [TestCase(SpecialAbilityConstants.ImprovedColdResistance, 64, 68)]
        [TestCase(SpecialAbilityConstants.ImprovedElectricityResistance, 69, 73)]
        [TestCase(SpecialAbilityConstants.ImprovedFireResistance, 74, 78)]
        [TestCase(SpecialAbilityConstants.ImprovedSonicResistance, 79, 83)]
        [TestCase(SpecialAbilityConstants.SpellResistance17, 84, 88)]
        [TestCase(SpecialAbilityConstants.HeavyFortification, 91, 92)]
        [TestCase(SpecialAbilityConstants.SpellResistance19, 93, 94)]
        public override void Percentile(string content, int lower, int upper)
        {
            base.Percentile(content, lower, upper);
        }

        [TestCase(SpecialAbilityConstants.LightFortification, 4)]
        [TestCase(SpecialAbilityConstants.Wild, 43)]
        [TestCase(SpecialAbilityConstants.Etherealness, 89)]
        [TestCase(SpecialAbilityConstants.UndeadControlling, 90)]
        [TestCase(SpecialAbilityConstants.GreaterAcidResistance, 95)]
        [TestCase(SpecialAbilityConstants.GreaterColdResistance, 96)]
        [TestCase(SpecialAbilityConstants.GreaterElectricityResistance, 97)]
        [TestCase(SpecialAbilityConstants.GreaterFireResistance, 98)]
        [TestCase(SpecialAbilityConstants.GreaterSonicResistance, 99)]
        [TestCase("BonusSpecialAbility", 100)]
        public override void Percentile(string content, int roll)
        {
            base.Percentile(content, roll);
        }

        [Test]
        public override void TableIsComplete()
        {
            AssertTableIsComplete();
        }
    }
}