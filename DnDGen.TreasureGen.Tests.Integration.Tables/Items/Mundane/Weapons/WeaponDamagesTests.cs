using DnDGen.Infrastructure.Helpers;
using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Selectors.Selections;
using DnDGen.TreasureGen.Tables;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items.Mundane.Weapons
{
    [TestFixture]
    public class WeaponDamagesTests : CollectionsTests
    {
        protected override string tableName => TableNameConstants.Collections.WeaponDamages;

        private Dictionary<string, List<string>> weaponDamages;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            weaponDamages = GetWeaponDamages();
        }

        [Test]
        public void AllKeysArePresent()
        {
            var weapons = WeaponConstants.GetAllWeapons(false, false);
            var sizes = TraitConstants.Sizes.GetAll();
            var expectedKeys = weapons.SelectMany(w => sizes.Select(s => w + s));

            Assert.That(weaponDamages.Keys, Is.EquivalentTo(expectedKeys));
            AssertCollection(table.Keys, expectedKeys);
        }

        [TestCaseSource(nameof(Weapons))]
        public void WeaponDamages(string weapon)
        {
            var sizes = TraitConstants.Sizes.GetAll();

            foreach (var size in sizes)
            {
                var key = weapon + size;
                AssertBludgeoningWeaponsMatchConstants(weapon, key);
                AssertPiercingWeaponsMatchConstants(weapon, key);
                AssertSlashingWeaponsMatchConstants(weapon, key);
                AssertDoubleWeaponsHaveMultipleDamages(weapon, key);

                Assert.That(weaponDamages, Contains.Key(key));
                AssertCollection(key, [.. weaponDamages[key]]);
            }
        }

        private void AssertBludgeoningWeaponsMatchConstants(string weapon, string key)
        {
            var bludgeoning = WeaponConstants.GetAllBludgeoning(false, false);
            var isBludgeoning = bludgeoning.Contains(weapon);
            var hasBludgeoning = weaponDamages[key].Any(d => d.Contains(AttributeConstants.DamageTypes.Bludgeoning));

            Assert.That(hasBludgeoning, Is.EqualTo(isBludgeoning), weapon);
        }

        private void AssertPiercingWeaponsMatchConstants(string weapon, string key)
        {
            var piercing = WeaponConstants.GetAllPiercing(false, false);
            var isPiercing = piercing.Contains(weapon);
            var hasPiercing = weaponDamages[key].Any(d => d.Contains(AttributeConstants.DamageTypes.Piercing));

            Assert.That(hasPiercing, Is.EqualTo(isPiercing), weapon);
        }

        private void AssertSlashingWeaponsMatchConstants(string weapon, string key)
        {
            var slashing = WeaponConstants.GetAllSlashing(false, false);
            var isSlashing = slashing.Contains(weapon);
            var hasSlashing = weaponDamages[key].Any(d => d.Contains(AttributeConstants.DamageTypes.Slashing));

            Assert.That(hasSlashing, Is.EqualTo(isSlashing), weapon);
        }

        private void AssertDoubleWeaponsHaveMultipleDamages(string weapon, string key)
        {
            var doubleWeapons = WeaponConstants.GetAllDouble(false, false);
            var isDouble = doubleWeapons.Contains(weapon);
            var count = isDouble ? 2 : 1;
            Assert.That(weaponDamages[key].Count, Is.EqualTo(count));
        }

        private Dictionary<string, List<string>> GetWeaponDamages()
        {
            var damages = new Dictionary<string, List<string>>
            {
                [WeaponConstants.Dagger + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d2", Type = "Piercing or Slashing" })],
                [WeaponConstants.Dagger + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d3", Type = "Piercing or Slashing" })],
                [WeaponConstants.Dagger + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d4", Type = "Piercing or Slashing" })],
                [WeaponConstants.Dagger + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d6", Type = "Piercing or Slashing" })],
                [WeaponConstants.Dagger + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d8", Type = "Piercing or Slashing" })],
                [WeaponConstants.Dagger + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = "Piercing or Slashing" })],
                [WeaponConstants.Dagger + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d6", Type = "Piercing or Slashing" })],
                [WeaponConstants.Greataxe + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d8", Type = "Slashing" })],
                [WeaponConstants.Greataxe + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d10", Type = "Slashing" })],
                [WeaponConstants.Greataxe + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d12", Type = "Slashing" })],
                [WeaponConstants.Greataxe + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d6", Type = "Slashing" })],
                [WeaponConstants.Greataxe + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = "Slashing" })],
                [WeaponConstants.Greataxe + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = "Slashing" })],
                [WeaponConstants.Greataxe + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "8d6", Type = "Slashing" })],
                [WeaponConstants.Greatsword + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d8", Type = "Slashing" })],
                [WeaponConstants.Greatsword + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d10", Type = "Slashing" })],
                [WeaponConstants.Greatsword + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = "Slashing" })],
                [WeaponConstants.Greatsword + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d6", Type = "Slashing" })],
                [WeaponConstants.Greatsword + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = "Slashing" })],
                [WeaponConstants.Greatsword + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = "Slashing" })],
                [WeaponConstants.Greatsword + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "8d6", Type = "Slashing" })],
                [WeaponConstants.Kama + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d3", Type = "Slashing" })],
                [WeaponConstants.Kama + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d4", Type = "Slashing" })],
                [WeaponConstants.Kama + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d6", Type = "Slashing" })],
                [WeaponConstants.Kama + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d8", Type = "Slashing" })],
                [WeaponConstants.Kama + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = "Slashing" })],
                [WeaponConstants.Kama + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d6", Type = "Slashing" })],
                [WeaponConstants.Kama + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = "Slashing" })],
                [WeaponConstants.Longsword + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d4", Type = "Slashing" })],
                [WeaponConstants.Longsword + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d6", Type = "Slashing" })],
                [WeaponConstants.Longsword + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d8", Type = "Slashing" })],
                [WeaponConstants.Longsword + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = "Slashing" })],
                [WeaponConstants.Longsword + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d6", Type = "Slashing" })],
                [WeaponConstants.Longsword + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = "Slashing" })],
                [WeaponConstants.Longsword + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = "Slashing" })],
                [WeaponConstants.HeavyMace + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d4", Type = "Bludgeoning" })],
                [WeaponConstants.HeavyMace + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d6", Type = "Bludgeoning" })],
                [WeaponConstants.HeavyMace + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d8", Type = "Bludgeoning" })],
                [WeaponConstants.HeavyMace + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = "Bludgeoning" })],
                [WeaponConstants.HeavyMace + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d6", Type = "Bludgeoning" })],
                [WeaponConstants.HeavyMace + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = "Bludgeoning" })],
                [WeaponConstants.HeavyMace + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = "Bludgeoning" })],
                [WeaponConstants.LightMace + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d3", Type = "Bludgeoning" })],
                [WeaponConstants.LightMace + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d4", Type = "Bludgeoning" })],
                [WeaponConstants.LightMace + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d6", Type = "Bludgeoning" })],
                [WeaponConstants.LightMace + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d8", Type = "Bludgeoning" })],
                [WeaponConstants.LightMace + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = "Bludgeoning" })],
                [WeaponConstants.LightMace + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d6", Type = "Bludgeoning" })],
                [WeaponConstants.LightMace + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = "Bludgeoning" })],
                [WeaponConstants.Nunchaku + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d3", Type = "Bludgeoning" })],
                [WeaponConstants.Nunchaku + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d4", Type = "Bludgeoning" })],
                [WeaponConstants.Nunchaku + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d6", Type = "Bludgeoning" })],
                [WeaponConstants.Nunchaku + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d8", Type = "Bludgeoning" })],
                [WeaponConstants.Nunchaku + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = "Bludgeoning" })],
                [WeaponConstants.Nunchaku + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d6", Type = "Bludgeoning" })],
                [WeaponConstants.Nunchaku + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = "Bludgeoning" })],
                [WeaponConstants.Quarterstaff + TraitConstants.Sizes.Tiny] = [
                    DataHelper.Parse(new DamageDataSelection { Roll = "1d3", Type = "Bludgeoning" }),
                    DataHelper.Parse(new DamageDataSelection { Roll = "1d3", Type = "Bludgeoning" })],
                [WeaponConstants.Quarterstaff + TraitConstants.Sizes.Small] = [
                    DataHelper.Parse(new DamageDataSelection { Roll = "1d4", Type = "Bludgeoning" }),
                    DataHelper.Parse(new DamageDataSelection { Roll = "1d4", Type = "Bludgeoning" })],
                [WeaponConstants.Quarterstaff + TraitConstants.Sizes.Medium] = [
                    DataHelper.Parse(new DamageDataSelection { Roll = "1d6", Type = "Bludgeoning" }),
                    DataHelper.Parse(new DamageDataSelection { Roll = "1d6", Type = "Bludgeoning" })],
                [WeaponConstants.Quarterstaff + TraitConstants.Sizes.Large] = [
                    DataHelper.Parse(new DamageDataSelection { Roll = "1d8", Type = "Bludgeoning" }),
                    DataHelper.Parse(new DamageDataSelection { Roll = "1d8", Type = "Bludgeoning" })],
                [WeaponConstants.Quarterstaff + TraitConstants.Sizes.Huge] = [
                    DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = "Bludgeoning" }),
                    DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = "Bludgeoning" })],
                [WeaponConstants.Quarterstaff + TraitConstants.Sizes.Gargantuan] = [
                    DataHelper.Parse(new DamageDataSelection { Roll = "3d6", Type = "Bludgeoning" }),
                    DataHelper.Parse(new DamageDataSelection { Roll = "3d6", Type = "Bludgeoning" })],
                [WeaponConstants.Quarterstaff + TraitConstants.Sizes.Colossal] = [
                    DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = "Bludgeoning" }),
                    DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = "Bludgeoning" })],
                [WeaponConstants.Scimitar + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d3", Type = "Piercing" })],
                [WeaponConstants.Scimitar + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d4", Type = "Piercing" })],
                [WeaponConstants.Scimitar + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d6", Type = "Piercing" })],
                [WeaponConstants.Scimitar + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d8", Type = "Piercing" })],
                [WeaponConstants.Scimitar + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = "Piercing" })],
                [WeaponConstants.Scimitar + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d6", Type = "Piercing" })],
                [WeaponConstants.Scimitar + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = "Piercing" })],
                [WeaponConstants.Rapier + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d3", Type = "Piercing" })],
                [WeaponConstants.Rapier + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d4", Type = "Piercing" })],
                [WeaponConstants.Rapier + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d6", Type = "Piercing" })],
                [WeaponConstants.Rapier + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d8", Type = "Piercing" })],
                [WeaponConstants.Rapier + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = "Piercing" })],
                [WeaponConstants.Rapier + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d6", Type = "Piercing" })],
                [WeaponConstants.Rapier + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = "Piercing" })],
                [WeaponConstants.Shortspear + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d3", Type = "Piercing" })],
                [WeaponConstants.Shortspear + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d4", Type = "Piercing" })],
                [WeaponConstants.Shortspear + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d6", Type = "Piercing" })],
                [WeaponConstants.Shortspear + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d8", Type = "Piercing" })],
                [WeaponConstants.Shortspear + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = "Piercing" })],
                [WeaponConstants.Shortspear + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d6", Type = "Piercing" })],
                [WeaponConstants.Shortspear + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = "Piercing" })],
                [WeaponConstants.Siangham + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d3", Type = "Piercing" })],
                [WeaponConstants.Siangham + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d4", Type = "Piercing" })],
                [WeaponConstants.Siangham + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d6", Type = "Piercing" })],
                [WeaponConstants.Siangham + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d8", Type = "Piercing" })],
                [WeaponConstants.Siangham + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = "Piercing" })],
                [WeaponConstants.Siangham + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d6", Type = "Piercing" })],
                [WeaponConstants.Siangham + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = "Piercing" })],
                [WeaponConstants.ShortSword + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d3", Type = "Piercing" })],
                [WeaponConstants.ShortSword + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d4", Type = "Piercing" })],
                [WeaponConstants.ShortSword + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d6", Type = "Piercing" })],
                [WeaponConstants.ShortSword + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d8", Type = "Piercing" })],
                [WeaponConstants.ShortSword + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = "Piercing" })],
                [WeaponConstants.ShortSword + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d6", Type = "Piercing" })],
                [WeaponConstants.ShortSword + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = "Piercing" })],
                [WeaponConstants.BastardSword + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d6", Type = "Slashing" })],
                [WeaponConstants.BastardSword + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d8", Type = "Slashing" })],
                [WeaponConstants.BastardSword + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d10", Type = "Slashing" })],
                [WeaponConstants.BastardSword + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d8", Type = "Slashing" })],
                [WeaponConstants.BastardSword + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d8", Type = "Slashing" })],
                [WeaponConstants.BastardSword + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d8", Type = "Slashing" })],
                [WeaponConstants.BastardSword + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d8", Type = "Slashing" })],
                [WeaponConstants.DwarvenWaraxe + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d6", Type = "Slashing" })],
                [WeaponConstants.DwarvenWaraxe + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d8", Type = "Slashing" })],
                [WeaponConstants.DwarvenWaraxe + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d10", Type = "Slashing" })],
                [WeaponConstants.DwarvenWaraxe + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d8", Type = "Slashing" })],
                [WeaponConstants.DwarvenWaraxe + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d8", Type = "Slashing" })],
                [WeaponConstants.DwarvenWaraxe + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d8", Type = "Slashing" })],
                [WeaponConstants.DwarvenWaraxe + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d8", Type = "Slashing" })],
                [WeaponConstants.OrcDoubleAxe + TraitConstants.Sizes.Tiny] = [
                    DataHelper.Parse(new DamageDataSelection { Roll = "1d4", Type = "Slashing" }),
                    DataHelper.Parse(new DamageDataSelection { Roll = "1d4", Type = "Slashing" })],
                [WeaponConstants.OrcDoubleAxe + TraitConstants.Sizes.Small] = [
                    DataHelper.Parse(new DamageDataSelection { Roll = "1d6", Type = "Slashing" }),
                    DataHelper.Parse(new DamageDataSelection { Roll = "1d6", Type = "Slashing" })],
                [WeaponConstants.OrcDoubleAxe + TraitConstants.Sizes.Medium] = [
                    DataHelper.Parse(new DamageDataSelection { Roll = "1d8", Type = "Slashing" }),
                    DataHelper.Parse(new DamageDataSelection { Roll = "1d8", Type = "Slashing" })],
                [WeaponConstants.OrcDoubleAxe + TraitConstants.Sizes.Large] = [
                    DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = "Slashing" }),
                    DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = "Slashing" })],
                [WeaponConstants.OrcDoubleAxe + TraitConstants.Sizes.Huge] = [
                    DataHelper.Parse(new DamageDataSelection { Roll = "3d6", Type = "Slashing" }),
                    DataHelper.Parse(new DamageDataSelection { Roll = "3d6", Type = "Slashing" })],
                [WeaponConstants.OrcDoubleAxe + TraitConstants.Sizes.Gargantuan] = [
                    DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = "Slashing" }),
                    DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = "Slashing" })],
                [WeaponConstants.OrcDoubleAxe + TraitConstants.Sizes.Colossal] = [
                    DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = "Slashing" }),
                    DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = "Slashing" })],
                [WeaponConstants.Battleaxe + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d4", Type = "Slashing" })],
                [WeaponConstants.Battleaxe + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d6", Type = "Slashing" })],
                [WeaponConstants.Battleaxe + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d8", Type = "Slashing" })],
                [WeaponConstants.Battleaxe + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = "Slashing" })],
                [WeaponConstants.Battleaxe + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d6", Type = "Slashing" })],
                [WeaponConstants.Battleaxe + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = "Slashing" })],
                [WeaponConstants.Battleaxe + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = "Slashing" })],
                [WeaponConstants.SpikedChain + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d4", Type = "Piercing" })],
                [WeaponConstants.SpikedChain + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d6", Type = "Piercing" })],
                [WeaponConstants.SpikedChain + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d4", Type = "Piercing" })],
                [WeaponConstants.SpikedChain + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = "Piercing" })],
                [WeaponConstants.SpikedChain + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d6", Type = "Piercing" })],
                [WeaponConstants.SpikedChain + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = "Piercing" })],
                [WeaponConstants.SpikedChain + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = "Piercing" })],
                [WeaponConstants.Club + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d3", Type = "Bludgeoning" })],
                [WeaponConstants.Club + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d4", Type = "Bludgeoning" })],
                [WeaponConstants.Club + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d6", Type = "Bludgeoning" })],
                [WeaponConstants.Club + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d8", Type = "Bludgeoning" })],
                [WeaponConstants.Club + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = "Bludgeoning" })],
                [WeaponConstants.Club + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d6", Type = "Bludgeoning" })],
                [WeaponConstants.Club + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = "Bludgeoning" })],
                [WeaponConstants.HandCrossbow + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d2", Type = "Piercing" })],
                [WeaponConstants.HandCrossbow + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d3", Type = "Piercing" })],
                [WeaponConstants.HandCrossbow + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d4", Type = "Piercing" })],
                [WeaponConstants.HandCrossbow + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d6", Type = "Piercing" })],
                [WeaponConstants.HandCrossbow + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d8", Type = "Piercing" })],
                [WeaponConstants.HandCrossbow + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = "Piercing" })],
                [WeaponConstants.HandCrossbow + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d6", Type = "Piercing" })],
                [WeaponConstants.HeavyRepeatingCrossbow + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d6", Type = "Piercing" })],
                [WeaponConstants.HeavyRepeatingCrossbow + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d8", Type = "Piercing" })],
                [WeaponConstants.HeavyRepeatingCrossbow + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d10", Type = "Piercing" })],
                [WeaponConstants.HeavyRepeatingCrossbow + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d8", Type = "Piercing" })],
                [WeaponConstants.HeavyRepeatingCrossbow + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d8", Type = "Piercing" })],
                [WeaponConstants.HeavyRepeatingCrossbow + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d8", Type = "Piercing" })],
                [WeaponConstants.HeavyRepeatingCrossbow + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d8", Type = "Piercing" })],
                [WeaponConstants.LightRepeatingCrossbow + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d4", Type = "Piercing" })],
                [WeaponConstants.LightRepeatingCrossbow + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d6", Type = "Piercing" })],
                [WeaponConstants.LightRepeatingCrossbow + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d8", Type = "Piercing" })],
                [WeaponConstants.LightRepeatingCrossbow + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = "Piercing" })],
                [WeaponConstants.LightRepeatingCrossbow + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d6", Type = "Piercing" })],
                [WeaponConstants.LightRepeatingCrossbow + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = "Piercing" })],
                [WeaponConstants.LightRepeatingCrossbow + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = "Piercing" })],
                [WeaponConstants.PunchingDagger + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d2", Type = "Piercing" })],
                [WeaponConstants.PunchingDagger + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d3", Type = "Piercing" })],
                [WeaponConstants.PunchingDagger + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d4", Type = "Piercing" })],
                [WeaponConstants.PunchingDagger + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d6", Type = "Piercing" })],
                [WeaponConstants.PunchingDagger + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d8", Type = "Piercing" })],
                [WeaponConstants.PunchingDagger + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = "Piercing" })],
                [WeaponConstants.PunchingDagger + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d6", Type = "Piercing" })],
                [WeaponConstants.Falchion + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d4", Type = "Slashing" })],
                [WeaponConstants.Falchion + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d6", Type = "Slashing" })],
                [WeaponConstants.Falchion + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d4", Type = "Slashing" })],
                [WeaponConstants.Falchion + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = "Slashing" })],
                [WeaponConstants.Falchion + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d6", Type = "Slashing" })],
                [WeaponConstants.Falchion + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = "Slashing" })],
                [WeaponConstants.Falchion + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = "Slashing" })],
                [WeaponConstants.DireFlail + TraitConstants.Sizes.Tiny] = [
                    DataHelper.Parse(new DamageDataSelection { Roll = "1d4", Type = "Bludgeoning" }),
                    DataHelper.Parse(new DamageDataSelection { Roll = "1d4", Type = "Bludgeoning" })],
                [WeaponConstants.DireFlail + TraitConstants.Sizes.Small] = [
                    DataHelper.Parse(new DamageDataSelection { Roll = "1d6", Type = "Bludgeoning" }),
                    DataHelper.Parse(new DamageDataSelection { Roll = "1d6", Type = "Bludgeoning" })],
                [WeaponConstants.DireFlail + TraitConstants.Sizes.Medium] = [
                    DataHelper.Parse(new DamageDataSelection { Roll = "1d8", Type = "Bludgeoning" }),
                    DataHelper.Parse(new DamageDataSelection { Roll = "1d8", Type = "Bludgeoning" })],
                [WeaponConstants.DireFlail + TraitConstants.Sizes.Large] = [
                    DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = "Bludgeoning" }),
                    DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = "Bludgeoning" })],
                [WeaponConstants.DireFlail + TraitConstants.Sizes.Huge] = [
                    DataHelper.Parse(new DamageDataSelection { Roll = "3d6", Type = "Bludgeoning" }),
                    DataHelper.Parse(new DamageDataSelection { Roll = "3d6", Type = "Bludgeoning" })],
                [WeaponConstants.DireFlail + TraitConstants.Sizes.Gargantuan] = [
                    DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = "Bludgeoning" }),
                    DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = "Bludgeoning" })],
                [WeaponConstants.DireFlail + TraitConstants.Sizes.Colossal] = [
                    DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = "Bludgeoning" }),
                    DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = "Bludgeoning" })],
                [WeaponConstants.HeavyFlail + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d6", Type = "Bludgeoning" })],
                [WeaponConstants.HeavyFlail + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d8", Type = "Bludgeoning" })],
                [WeaponConstants.HeavyFlail + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d10", Type = "Bludgeoning" })],
                [WeaponConstants.HeavyFlail + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d8", Type = "Bludgeoning" })],
                [WeaponConstants.HeavyFlail + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d8", Type = "Bludgeoning" })],
                [WeaponConstants.HeavyFlail + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d8", Type = "Bludgeoning" })],
                [WeaponConstants.HeavyFlail + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d8", Type = "Bludgeoning" })],
                [WeaponConstants.Flail + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d4", Type = "Bludgeoning" })],
                [WeaponConstants.Flail + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d6", Type = "Bludgeoning" })],
                [WeaponConstants.Flail + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d8", Type = "Bludgeoning" })],
                [WeaponConstants.Flail + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = "Bludgeoning" })],
                [WeaponConstants.Flail + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d6", Type = "Bludgeoning" })],
                [WeaponConstants.Flail + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = "Bludgeoning" })],
                [WeaponConstants.Flail + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = "Bludgeoning" })],
                [WeaponConstants.Gauntlet + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "1", Type = "Bludgeoning" })],
                [WeaponConstants.Gauntlet + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d2", Type = "Bludgeoning" })],
                [WeaponConstants.Gauntlet + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d3", Type = "Bludgeoning" })],
                [WeaponConstants.Gauntlet + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d4", Type = "Bludgeoning" })],
                [WeaponConstants.Gauntlet + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d6", Type = "Bludgeoning" })],
                [WeaponConstants.Gauntlet + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d8", Type = "Bludgeoning" })],
                [WeaponConstants.Gauntlet + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = "Bludgeoning" })],
                [WeaponConstants.SpikedGauntlet + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d2", Type = "Piercing" })],
                [WeaponConstants.SpikedGauntlet + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d3", Type = "Piercing" })],
                [WeaponConstants.SpikedGauntlet + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d4", Type = "Piercing" })],
                [WeaponConstants.SpikedGauntlet + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d6", Type = "Piercing" })],
                [WeaponConstants.SpikedGauntlet + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d8", Type = "Piercing" })],
                [WeaponConstants.SpikedGauntlet + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = "Piercing" })],
                [WeaponConstants.SpikedGauntlet + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d6", Type = "Piercing" })],
                [WeaponConstants.Glaive + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d6", Type = "Slashing" })],
                [WeaponConstants.Glaive + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d8", Type = "Slashing" })],
                [WeaponConstants.Glaive + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d10", Type = "Slashing" })],
                [WeaponConstants.Glaive + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d8", Type = "Slashing" })],
                [WeaponConstants.Glaive + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d8", Type = "Slashing" })],
                [WeaponConstants.Glaive + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d8", Type = "Slashing" })],
                [WeaponConstants.Glaive + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d8", Type = "Slashing" })],
                [WeaponConstants.Greatclub + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d6", Type = "Bludgeoning" })],
                [WeaponConstants.Greatclub + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d8", Type = "Bludgeoning" })],
                [WeaponConstants.Greatclub + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d10", Type = "Bludgeoning" })],
                [WeaponConstants.Greatclub + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d8", Type = "Bludgeoning" })],
                [WeaponConstants.Greatclub + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d8", Type = "Bludgeoning" })],
                [WeaponConstants.Greatclub + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d8", Type = "Bludgeoning" })],
                [WeaponConstants.Greatclub + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d8", Type = "Bludgeoning" })],
                [WeaponConstants.Guisarme + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d4", Type = "Slashing" })],
                [WeaponConstants.Guisarme + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d6", Type = "Slashing" })],
                [WeaponConstants.Guisarme + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d8", Type = "Slashing" })],
                [WeaponConstants.Guisarme + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = "Slashing" })],
                [WeaponConstants.Guisarme + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d6", Type = "Slashing" })],
                [WeaponConstants.Guisarme + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = "Slashing" })],
                [WeaponConstants.Guisarme + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = "Slashing" })],
                [WeaponConstants.Halberd + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d6", Type = "Piercing or Slashing" })],
                [WeaponConstants.Halberd + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d8", Type = "Piercing or Slashing" })],
                [WeaponConstants.Halberd + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d10", Type = "Piercing or Slashing" })],
                [WeaponConstants.Halberd + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d8", Type = "Piercing or Slashing" })],
                [WeaponConstants.Halberd + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d8", Type = "Piercing or Slashing" })],
                [WeaponConstants.Halberd + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d8", Type = "Piercing or Slashing" })],
                [WeaponConstants.Halberd + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d8", Type = "Piercing or Slashing" })],
                [WeaponConstants.Spear + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d4", Type = "Piercing" })],
                [WeaponConstants.Spear + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d6", Type = "Piercing" })],
                [WeaponConstants.Spear + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d8", Type = "Piercing" })],
                [WeaponConstants.Spear + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = "Piercing" })],
                [WeaponConstants.Spear + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d6", Type = "Piercing" })],
                [WeaponConstants.Spear + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = "Piercing" })],
                [WeaponConstants.Spear + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = "Piercing" })],
                [WeaponConstants.GnomeHookedHammer + TraitConstants.Sizes.Tiny] = [
                    DataHelper.Parse(new DamageDataSelection { Roll = "1d4", Type = "Bludgeoning" }),
                    DataHelper.Parse(new DamageDataSelection { Roll = "1d3", Type = "Piercing" })],
                [WeaponConstants.GnomeHookedHammer + TraitConstants.Sizes.Small] = [
                    DataHelper.Parse(new DamageDataSelection { Roll = "1d6", Type = "Bludgeoning" }),
                    DataHelper.Parse(new DamageDataSelection { Roll = "1d4", Type = "Piercing" })],
                [WeaponConstants.GnomeHookedHammer + TraitConstants.Sizes.Medium] = [
                    DataHelper.Parse(new DamageDataSelection { Roll = "1d8", Type = "Bludgeoning" }),
                    DataHelper.Parse(new DamageDataSelection { Roll = "1d6", Type = "Piercing" })],
                [WeaponConstants.GnomeHookedHammer + TraitConstants.Sizes.Large] = [
                    DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = "Bludgeoning" }),
                    DataHelper.Parse(new DamageDataSelection { Roll = "1d8", Type = "Piercing" })],
                [WeaponConstants.GnomeHookedHammer + TraitConstants.Sizes.Huge] = [
                    DataHelper.Parse(new DamageDataSelection { Roll = "3d6", Type = "Bludgeoning" }),
                    DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = "Piercing" })],
                [WeaponConstants.GnomeHookedHammer + TraitConstants.Sizes.Gargantuan] = [
                    DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = "Bludgeoning" }),
                    DataHelper.Parse(new DamageDataSelection { Roll = "3d6", Type = "Piercing" })],
                [WeaponConstants.GnomeHookedHammer + TraitConstants.Sizes.Colossal] = [
                    DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = "Bludgeoning" }),
                    DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = "Piercing" })],
                [WeaponConstants.LightHammer + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d2", Type = "Bludgeoning" })],
                [WeaponConstants.LightHammer + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d3", Type = "Bludgeoning" })],
                [WeaponConstants.LightHammer + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d4", Type = "Bludgeoning" })],
                [WeaponConstants.LightHammer + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d6", Type = "Bludgeoning" })],
                [WeaponConstants.LightHammer + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d8", Type = "Bludgeoning" })],
                [WeaponConstants.LightHammer + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = "Bludgeoning" })],
                [WeaponConstants.LightHammer + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d6", Type = "Bludgeoning" })],
                [WeaponConstants.Handaxe + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d3", Type = "Slashing" })],
                [WeaponConstants.Handaxe + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d4", Type = "Slashing" })],
                [WeaponConstants.Handaxe + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d6", Type = "Slashing" })],
                [WeaponConstants.Handaxe + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d8", Type = "Slashing" })],
                [WeaponConstants.Handaxe + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = "Slashing" })],
                [WeaponConstants.Handaxe + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d6", Type = "Slashing" })],
                [WeaponConstants.Handaxe + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = "Slashing" })],
                [WeaponConstants.Kukri + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d2", Type = "Slashing" })],
                [WeaponConstants.Kukri + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d3", Type = "Slashing" })],
                [WeaponConstants.Kukri + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d4", Type = "Slashing" })],
                [WeaponConstants.Kukri + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d6", Type = "Slashing" })],
                [WeaponConstants.Kukri + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d8", Type = "Slashing" })],
                [WeaponConstants.Kukri + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = "Slashing" })],
                [WeaponConstants.Kukri + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d6", Type = "Slashing" })],
                [WeaponConstants.Lance + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d4", Type = "Piercing" })],
                [WeaponConstants.Lance + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d6", Type = "Piercing" })],
                [WeaponConstants.Lance + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d8", Type = "Piercing" })],
                [WeaponConstants.Lance + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = "Piercing" })],
                [WeaponConstants.Lance + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d6", Type = "Piercing" })],
                [WeaponConstants.Lance + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = "Piercing" })],
                [WeaponConstants.Lance + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = "Piercing" })],
                [WeaponConstants.Longspear + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d4", Type = "Piercing" })],
                [WeaponConstants.Longspear + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d6", Type = "Piercing" })],
                [WeaponConstants.Longspear + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d8", Type = "Piercing" })],
                [WeaponConstants.Longspear + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = "Piercing" })],
                [WeaponConstants.Longspear + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d6", Type = "Piercing" })],
                [WeaponConstants.Longspear + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = "Piercing" })],
                [WeaponConstants.Longspear + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = "Piercing" })],
                [WeaponConstants.Morningstar + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d4", Type = "Bludgeoning and Piercing" })],
                [WeaponConstants.Morningstar + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d6", Type = "Bludgeoning and Piercing" })],
                [WeaponConstants.Morningstar + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d8", Type = "Bludgeoning and Piercing" })],
                [WeaponConstants.Morningstar + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = "Bludgeoning and Piercing" })],
                [WeaponConstants.Morningstar + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d6", Type = "Bludgeoning and Piercing" })],
                [WeaponConstants.Morningstar + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = "Bludgeoning and Piercing" })],
                [WeaponConstants.Morningstar + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = "Bludgeoning and Piercing" })],
                [WeaponConstants.Net + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "0", Type = "Bludgeoning" })],
                [WeaponConstants.Net + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "0", Type = "Bludgeoning" })],
                [WeaponConstants.Net + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "0", Type = "Bludgeoning" })],
                [WeaponConstants.Net + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "0", Type = "Bludgeoning" })],
                [WeaponConstants.Net + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "0", Type = "Bludgeoning" })],
                [WeaponConstants.Net + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "0", Type = "Bludgeoning" })],
                [WeaponConstants.Net + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "0", Type = "Bludgeoning" })],
                [WeaponConstants.HeavyPick + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d3", Type = "Piercing" })],
                [WeaponConstants.HeavyPick + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d4", Type = "Piercing" })],
                [WeaponConstants.HeavyPick + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d6", Type = "Piercing" })],
                [WeaponConstants.HeavyPick + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d8", Type = "Piercing" })],
                [WeaponConstants.HeavyPick + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = "Piercing" })],
                [WeaponConstants.HeavyPick + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d6", Type = "Piercing" })],
                [WeaponConstants.HeavyPick + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = "Piercing" })],
                [WeaponConstants.LightPick + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d2", Type = "Piercing" })],
                [WeaponConstants.LightPick + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d3", Type = "Piercing" })],
                [WeaponConstants.LightPick + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d4", Type = "Piercing" })],
                [WeaponConstants.LightPick + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d6", Type = "Piercing" })],
                [WeaponConstants.LightPick + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d8", Type = "Piercing" })],
                [WeaponConstants.LightPick + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = "Piercing" })],
                [WeaponConstants.LightPick + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d6", Type = "Piercing" })],
                [WeaponConstants.Sai + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d2", Type = "Bludgeoning" })],
                [WeaponConstants.Sai + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d3", Type = "Bludgeoning" })],
                [WeaponConstants.Sai + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d4", Type = "Bludgeoning" })],
                [WeaponConstants.Sai + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d6", Type = "Bludgeoning" })],
                [WeaponConstants.Sai + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d8", Type = "Bludgeoning" })],
                [WeaponConstants.Sai + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = "Bludgeoning" })],
                [WeaponConstants.Sai + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d6", Type = "Bludgeoning" })],
                [WeaponConstants.Bolas + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d2", Type = "Bludgeoning" })],
                [WeaponConstants.Bolas + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d3", Type = "Bludgeoning" })],
                [WeaponConstants.Bolas + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d4", Type = "Bludgeoning" })],
                [WeaponConstants.Bolas + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d6", Type = "Bludgeoning" })],
                [WeaponConstants.Bolas + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d8", Type = "Bludgeoning" })],
                [WeaponConstants.Bolas + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = "Bludgeoning" })],
                [WeaponConstants.Bolas + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d6", Type = "Bludgeoning" })],
                [WeaponConstants.Ranseur + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d4", Type = "Piercing" })],
                [WeaponConstants.Ranseur + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d6", Type = "Piercing" })],
                [WeaponConstants.Ranseur + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d4", Type = "Piercing" })],
                [WeaponConstants.Ranseur + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = "Piercing" })],
                [WeaponConstants.Ranseur + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d6", Type = "Piercing" })],
                [WeaponConstants.Ranseur + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = "Piercing" })],
                [WeaponConstants.Ranseur + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = "Piercing" })],
                [WeaponConstants.Sap + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d3", Type = "Bludgeoning" })],
                [WeaponConstants.Sap + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d4", Type = "Bludgeoning" })],
                [WeaponConstants.Sap + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d6", Type = "Bludgeoning" })],
                [WeaponConstants.Sap + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d8", Type = "Bludgeoning" })],
                [WeaponConstants.Sap + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = "Bludgeoning" })],
                [WeaponConstants.Sap + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d6", Type = "Bludgeoning" })],
                [WeaponConstants.Sap + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = "Bludgeoning" })],
                [WeaponConstants.Scythe + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d4", Type = "Piercing or Slashing" })],
                [WeaponConstants.Scythe + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d6", Type = "Piercing or Slashing" })],
                [WeaponConstants.Scythe + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d4", Type = "Piercing or Slashing" })],
                [WeaponConstants.Scythe + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = "Piercing or Slashing" })],
                [WeaponConstants.Scythe + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d6", Type = "Piercing or Slashing" })],
                [WeaponConstants.Scythe + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = "Piercing or Slashing" })],
                [WeaponConstants.Scythe + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = "Piercing or Slashing" })],
                [WeaponConstants.Shuriken + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "0", Type = "Piercing" })],
                [WeaponConstants.Shuriken + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "1", Type = "Piercing" })],
                [WeaponConstants.Shuriken + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d2", Type = "Piercing" })],
                [WeaponConstants.Shuriken + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d3", Type = "Piercing" })],
                [WeaponConstants.Shuriken + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d4", Type = "Piercing" })],
                [WeaponConstants.Shuriken + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d6", Type = "Piercing" })],
                [WeaponConstants.Shuriken + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d8", Type = "Piercing" })],
                [WeaponConstants.Sickle + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d3", Type = "Slashing" })],
                [WeaponConstants.Sickle + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d4", Type = "Slashing" })],
                [WeaponConstants.Sickle + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d6", Type = "Slashing" })],
                [WeaponConstants.Sickle + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d8", Type = "Slashing" })],
                [WeaponConstants.Sickle + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = "Slashing" })],
                [WeaponConstants.Sickle + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d6", Type = "Slashing" })],
                [WeaponConstants.Sickle + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = "Slashing" })],
                [WeaponConstants.TwoBladedSword + TraitConstants.Sizes.Tiny] = [
                    DataHelper.Parse(new DamageDataSelection { Roll = "1d4", Type = "Slashing" }),
                    DataHelper.Parse(new DamageDataSelection { Roll = "1d4", Type = "Slashing" })],
                [WeaponConstants.TwoBladedSword + TraitConstants.Sizes.Small] = [
                    DataHelper.Parse(new DamageDataSelection { Roll = "1d6", Type = "Slashing" }),
                    DataHelper.Parse(new DamageDataSelection { Roll = "1d6", Type = "Slashing" })],
                [WeaponConstants.TwoBladedSword + TraitConstants.Sizes.Medium] = [
                    DataHelper.Parse(new DamageDataSelection { Roll = "1d8", Type = "Slashing" }),
                    DataHelper.Parse(new DamageDataSelection { Roll = "1d8", Type = "Slashing" })],
                [WeaponConstants.TwoBladedSword + TraitConstants.Sizes.Large] = [
                    DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = "Slashing" }),
                    DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = "Slashing" })],
                [WeaponConstants.TwoBladedSword + TraitConstants.Sizes.Huge] = [
                    DataHelper.Parse(new DamageDataSelection { Roll = "3d6", Type = "Slashing" }),
                    DataHelper.Parse(new DamageDataSelection { Roll = "3d6", Type = "Slashing" })],
                [WeaponConstants.TwoBladedSword + TraitConstants.Sizes.Gargantuan] = [
                    DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = "Slashing" }),
                    DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = "Slashing" })],
                [WeaponConstants.TwoBladedSword + TraitConstants.Sizes.Colossal] = [
                    DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = "Slashing" }),
                    DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = "Slashing" })],
                [WeaponConstants.Trident + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d4", Type = "Piercing" })],
                [WeaponConstants.Trident + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d6", Type = "Piercing" })],
                [WeaponConstants.Trident + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d8", Type = "Piercing" })],
                [WeaponConstants.Trident + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = "Piercing" })],
                [WeaponConstants.Trident + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d6", Type = "Piercing" })],
                [WeaponConstants.Trident + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = "Piercing" })],
                [WeaponConstants.Trident + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = "Piercing" })],
                [WeaponConstants.DwarvenUrgrosh + TraitConstants.Sizes.Tiny] = [
                    DataHelper.Parse(new DamageDataSelection { Roll = "1d4", Type = "Slashing" }),
                    DataHelper.Parse(new DamageDataSelection { Roll = "1d3", Type = "Piercing" })],
                [WeaponConstants.DwarvenUrgrosh + TraitConstants.Sizes.Small] = [
                    DataHelper.Parse(new DamageDataSelection { Roll = "1d6", Type = "Slashing" }),
                    DataHelper.Parse(new DamageDataSelection { Roll = "1d4", Type = "Piercing" })],
                [WeaponConstants.DwarvenUrgrosh + TraitConstants.Sizes.Medium] = [
                    DataHelper.Parse(new DamageDataSelection { Roll = "1d8", Type = "Slashing" }),
                    DataHelper.Parse(new DamageDataSelection { Roll = "1d6", Type = "Piercing" })],
                [WeaponConstants.DwarvenUrgrosh + TraitConstants.Sizes.Large] = [
                    DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = "Slashing" }),
                    DataHelper.Parse(new DamageDataSelection { Roll = "1d8", Type = "Piercing" })],
                [WeaponConstants.DwarvenUrgrosh + TraitConstants.Sizes.Huge] = [
                    DataHelper.Parse(new DamageDataSelection { Roll = "3d6", Type = "Slashing" }),
                    DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = "Piercing" })],
                [WeaponConstants.DwarvenUrgrosh + TraitConstants.Sizes.Gargantuan] = [
                    DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = "Slashing" }),
                    DataHelper.Parse(new DamageDataSelection { Roll = "3d6", Type = "Piercing" })],
                [WeaponConstants.DwarvenUrgrosh + TraitConstants.Sizes.Colossal] = [
                    DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = "Slashing" }),
                    DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = "Piercing" })],
                [WeaponConstants.Warhammer + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d4", Type = "Bludgeoning" })],
                [WeaponConstants.Warhammer + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d6", Type = "Bludgeoning" })],
                [WeaponConstants.Warhammer + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d8", Type = "Bludgeoning" })],
                [WeaponConstants.Warhammer + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = "Bludgeoning" })],
                [WeaponConstants.Warhammer + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d6", Type = "Bludgeoning" })],
                [WeaponConstants.Warhammer + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = "Bludgeoning" })],
                [WeaponConstants.Warhammer + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = "Bludgeoning" })],
                [WeaponConstants.Whip + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "1", Type = "Slashing" })],
                [WeaponConstants.Whip + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d2", Type = "Slashing" })],
                [WeaponConstants.Whip + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d3", Type = "Slashing" })],
                [WeaponConstants.Whip + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d4", Type = "Slashing" })],
                [WeaponConstants.Whip + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d6", Type = "Slashing" })],
                [WeaponConstants.Whip + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d8", Type = "Slashing" })],
                [WeaponConstants.Whip + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = "Slashing" })],
                [WeaponConstants.ThrowingAxe + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d3", Type = "Slashing" })],
                [WeaponConstants.ThrowingAxe + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d4", Type = "Slashing" })],
                [WeaponConstants.ThrowingAxe + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d6", Type = "Slashing" })],
                [WeaponConstants.ThrowingAxe + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d8", Type = "Slashing" })],
                [WeaponConstants.ThrowingAxe + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = "Slashing" })],
                [WeaponConstants.ThrowingAxe + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d6", Type = "Slashing" })],
                [WeaponConstants.ThrowingAxe + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = "Slashing" })],
                [WeaponConstants.HeavyCrossbow + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d6", Type = "Piercing" })],
                [WeaponConstants.HeavyCrossbow + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d8", Type = "Piercing" })],
                [WeaponConstants.HeavyCrossbow + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d10", Type = "Piercing" })],
                [WeaponConstants.HeavyCrossbow + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d8", Type = "Piercing" })],
                [WeaponConstants.HeavyCrossbow + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d8", Type = "Piercing" })],
                [WeaponConstants.HeavyCrossbow + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d8", Type = "Piercing" })],
                [WeaponConstants.HeavyCrossbow + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d8", Type = "Piercing" })],
                [WeaponConstants.LightCrossbow + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d4", Type = "Piercing" })],
                [WeaponConstants.LightCrossbow + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d6", Type = "Piercing" })],
                [WeaponConstants.LightCrossbow + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d8", Type = "Piercing" })],
                [WeaponConstants.LightCrossbow + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = "Piercing" })],
                [WeaponConstants.LightCrossbow + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d6", Type = "Piercing" })],
                [WeaponConstants.LightCrossbow + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = "Piercing" })],
                [WeaponConstants.LightCrossbow + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = "Piercing" })],
                [WeaponConstants.Dart + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d2", Type = "Piercing" })],
                [WeaponConstants.Dart + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d3", Type = "Piercing" })],
                [WeaponConstants.Dart + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d4", Type = "Piercing" })],
                [WeaponConstants.Dart + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d6", Type = "Piercing" })],
                [WeaponConstants.Dart + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d8", Type = "Piercing" })],
                [WeaponConstants.Dart + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = "Piercing" })],
                [WeaponConstants.Dart + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d6", Type = "Piercing" })],
                [WeaponConstants.Javelin + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d3", Type = "Piercing" })],
                [WeaponConstants.Javelin + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d4", Type = "Piercing" })],
                [WeaponConstants.Javelin + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d6", Type = "Piercing" })],
                [WeaponConstants.Javelin + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d8", Type = "Piercing" })],
                [WeaponConstants.Javelin + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = "Piercing" })],
                [WeaponConstants.Javelin + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d6", Type = "Piercing" })],
                [WeaponConstants.Javelin + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = "Piercing" })],
                [WeaponConstants.Shortbow + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d3", Type = "Piercing" })],
                [WeaponConstants.Shortbow + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d4", Type = "Piercing" })],
                [WeaponConstants.Shortbow + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d6", Type = "Piercing" })],
                [WeaponConstants.Shortbow + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d8", Type = "Piercing" })],
                [WeaponConstants.Shortbow + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = "Piercing" })],
                [WeaponConstants.Shortbow + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d6", Type = "Piercing" })],
                [WeaponConstants.Shortbow + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = "Piercing" })],
                [WeaponConstants.CompositeShortbow + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d3", Type = "Piercing" })],
                [WeaponConstants.CompositeShortbow + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d4", Type = "Piercing" })],
                [WeaponConstants.CompositeShortbow + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d6", Type = "Piercing" })],
                [WeaponConstants.CompositeShortbow + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d8", Type = "Piercing" })],
                [WeaponConstants.CompositeShortbow + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = "Piercing" })],
                [WeaponConstants.CompositeShortbow + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d6", Type = "Piercing" })],
                [WeaponConstants.CompositeShortbow + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = "Piercing" })],
                [WeaponConstants.Sling + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d2", Type = "Bludgeoning" })],
                [WeaponConstants.Sling + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d3", Type = "Bludgeoning" })],
                [WeaponConstants.Sling + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d4", Type = "Bludgeoning" })],
                [WeaponConstants.Sling + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d6", Type = "Bludgeoning" })],
                [WeaponConstants.Sling + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d8", Type = "Bludgeoning" })],
                [WeaponConstants.Sling + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = "Bludgeoning" })],
                [WeaponConstants.Sling + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d6", Type = "Bludgeoning" })],
                [WeaponConstants.PincerStaff + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d6", Type = "Bludgeoning" })],
                [WeaponConstants.PincerStaff + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d8", Type = "Bludgeoning" })],
                [WeaponConstants.PincerStaff + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d10", Type = "Bludgeoning" })],
                [WeaponConstants.PincerStaff + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d8", Type = "Bludgeoning" })],
                [WeaponConstants.PincerStaff + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d8", Type = "Bludgeoning" })],
                [WeaponConstants.PincerStaff + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d8", Type = "Bludgeoning" })],
                [WeaponConstants.PincerStaff + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d8", Type = "Bludgeoning" })],
                [WeaponConstants.SlingBullet + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "0", Type = "Bludgeoning" })],
                [WeaponConstants.SlingBullet + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "0", Type = "Bludgeoning" })],
                [WeaponConstants.SlingBullet + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "0", Type = "Bludgeoning" })],
                [WeaponConstants.SlingBullet + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "0", Type = "Bludgeoning" })],
                [WeaponConstants.SlingBullet + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "0", Type = "Bludgeoning" })],
                [WeaponConstants.SlingBullet + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "0", Type = "Bludgeoning" })],
                [WeaponConstants.SlingBullet + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "0", Type = "Bludgeoning" })],
                [WeaponConstants.Arrow + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "0", Type = "Piercing" })],
                [WeaponConstants.Arrow + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "0", Type = "Piercing" })],
                [WeaponConstants.Arrow + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "0", Type = "Piercing" })],
                [WeaponConstants.Arrow + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "0", Type = "Piercing" })],
                [WeaponConstants.Arrow + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "0", Type = "Piercing" })],
                [WeaponConstants.Arrow + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "0", Type = "Piercing" })],
                [WeaponConstants.Arrow + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "0", Type = "Piercing" })],
                [WeaponConstants.CrossbowBolt + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "0", Type = "Piercing" })],
                [WeaponConstants.CrossbowBolt + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "0", Type = "Piercing" })],
                [WeaponConstants.CrossbowBolt + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "0", Type = "Piercing" })],
                [WeaponConstants.CrossbowBolt + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "0", Type = "Piercing" })],
                [WeaponConstants.CrossbowBolt + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "0", Type = "Piercing" })],
                [WeaponConstants.CrossbowBolt + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "0", Type = "Piercing" })],
                [WeaponConstants.CrossbowBolt + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "0", Type = "Piercing" })],
                [WeaponConstants.CompositeLongbow + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d4", Type = "Piercing" })],
                [WeaponConstants.CompositeLongbow + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d6", Type = "Piercing" })],
                [WeaponConstants.CompositeLongbow + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d8", Type = "Piercing" })],
                [WeaponConstants.CompositeLongbow + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = "Piercing" })],
                [WeaponConstants.CompositeLongbow + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d6", Type = "Piercing" })],
                [WeaponConstants.CompositeLongbow + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = "Piercing" })],
                [WeaponConstants.CompositeLongbow + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = "Piercing" })],
                [WeaponConstants.Longbow + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d4", Type = "Piercing" })],
                [WeaponConstants.Longbow + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d6", Type = "Piercing" })],
                [WeaponConstants.Longbow + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "1d8", Type = "Piercing" })],
                [WeaponConstants.Longbow + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = "Piercing" })],
                [WeaponConstants.Longbow + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d6", Type = "Piercing" })],
                [WeaponConstants.Longbow + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = "Piercing" })],
                [WeaponConstants.Longbow + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = "Piercing" })],
            };

            return damages;
        }

        public static IEnumerable Weapons => WeaponConstants.GetAllWeapons(false, false).Select(w => new TestCaseData(w));
    }
}
