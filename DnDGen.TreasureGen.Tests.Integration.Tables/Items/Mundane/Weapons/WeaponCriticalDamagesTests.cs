using DnDGen.Infrastructure.Helpers;
using DnDGen.Infrastructure.Selectors.Collections;
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
    public class WeaponCriticalDamagesTests : CollectionsTests
    {
        protected override string tableName => TableNameConstants.Collections.WeaponCriticalDamages;

        private Dictionary<string, List<string>> weaponCriticalDamages;
        private ICollectionDataSelector<WeaponDataSelection> weaponDataSelector;
        private ICollectionDataSelector<DamageDataSelection> damageDataSelector;
        private string[] noDamage;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            weaponCriticalDamages = GetWeaponCriticalDamages();
            noDamage =
            [
                WeaponConstants.CrossbowBolt,
                WeaponConstants.Arrow,
                WeaponConstants.SlingBullet,
                WeaponConstants.Net,
            ];
        }

        [SetUp]
        public void Setup()
        {
            weaponDataSelector = GetNewInstanceOf<ICollectionDataSelector<WeaponDataSelection>>();
            damageDataSelector = GetNewInstanceOf<ICollectionDataSelector<DamageDataSelection>>();
        }

        [Test]
        public void AllKeysArePresent()
        {
            var weapons = WeaponConstants.GetAllWeapons(false, false);
            var sizes = TraitConstants.Sizes.GetAll();
            var expectedKeys = weapons.SelectMany(w => sizes.Select(s => w + s));

            Assert.That(weaponCriticalDamages.Keys, Is.EquivalentTo(expectedKeys));
            AssertCollection(table.Keys, expectedKeys);
        }

        [TestCaseSource(nameof(Weapons))]
        public void WeaponCriticalDamages(string weapon)
        {
            var sizes = TraitConstants.Sizes.GetAll();
            var weaponData = weaponDataSelector.SelectOneFrom(Config.Name, TableNameConstants.Collections.WeaponData, weapon);

            foreach (var size in sizes)
            {
                var key = weapon + size;
                AssertBludgeoningWeaponsMatchConstants(weapon, key);
                AssertPiercingWeaponsMatchConstants(weapon, key);
                AssertSlashingWeaponsMatchConstants(weapon, key);
                AssertDoubleWeaponsHaveMultipleDamages(weapon, key);
                AssertWeaponCriticalDamagesHaveCorrectMultiplier(weapon, key, weaponData);

                Assert.That(weaponCriticalDamages, Contains.Key(key));
                AssertCollection(key, [.. weaponCriticalDamages[key]]);
            }
        }

        private void AssertBludgeoningWeaponsMatchConstants(string weapon, string key)
        {
            var bludgeoning = WeaponConstants.GetAllBludgeoning(false, false);
            var isBludgeoning = bludgeoning.Contains(weapon);
            var hasBludgeoning = weaponCriticalDamages[key].Any(d => d.Contains(AttributeConstants.DamageTypes.Bludgeoning));

            Assert.That(hasBludgeoning, Is.EqualTo(isBludgeoning), $"TEST DATA: {key}");
        }

        private void AssertPiercingWeaponsMatchConstants(string weapon, string key)
        {
            var piercing = WeaponConstants.GetAllPiercing(false, false);
            var isPiercing = piercing.Contains(weapon);
            var hasPiercing = weaponCriticalDamages[key].Any(d => d.Contains(AttributeConstants.DamageTypes.Piercing));

            Assert.That(hasPiercing, Is.EqualTo(isPiercing), $"TEST DATA: {key}");
        }

        private void AssertSlashingWeaponsMatchConstants(string weapon, string key)
        {
            var slashing = WeaponConstants.GetAllSlashing(false, false);
            var isSlashing = slashing.Contains(weapon);
            var hasSlashing = weaponCriticalDamages[key].Any(d => d.Contains(AttributeConstants.DamageTypes.Slashing));

            Assert.That(hasSlashing, Is.EqualTo(isSlashing), $"TEST DATA: {key}");
        }

        private void AssertDoubleWeaponsHaveMultipleDamages(string weapon, string key)
        {
            var doubleWeapons = WeaponConstants.GetAllDouble(false, false);
            var isDouble = doubleWeapons.Contains(weapon);
            var count = isDouble ? 2 : 1;
            Assert.That(weaponCriticalDamages[key].Count, Is.EqualTo(count), $"TEST DATA: {key}");
        }

        private void AssertWeaponCriticalDamagesHaveCorrectMultiplier(string weapon, string key, WeaponDataSelection weaponData)
        {
            var normalDamages = damageDataSelector.SelectFrom(Config.Name, TableNameConstants.Collections.WeaponDamages, key).ToArray();
            var criticalDamages = weaponCriticalDamages[key].Select(DataHelper.Parse<DamageDataSelection>).ToArray();

            Assert.That(normalDamages, Has.Length.EqualTo(criticalDamages.Length), key);
            Assert.That(normalDamages, Has.Length.EqualTo(1).Or.Length.EqualTo(2), key);
            Assert.That(criticalDamages, Has.Length.EqualTo(1).Or.Length.EqualTo(2), key);

            AssertCriticalMultiplier(weapon, key, normalDamages[0], criticalDamages[0], weaponData.CriticalMultiplier);
            if (normalDamages.Length == 1)
                return;

            AssertCriticalMultiplier(weapon, key, normalDamages[1], criticalDamages[1], weaponData.SecondaryCriticalMultiplier);
        }

        private void AssertCriticalMultiplier(string weapon, string key, DamageDataSelection normal, DamageDataSelection critical, string criticalMultiplier)
        {
            Assert.That(critical.Type, Is.EqualTo(normal.Type), weapon);
            Assert.That(normal.Condition, Is.Empty, weapon);
            Assert.That(critical.Condition, Is.Empty, weapon);

            (var normalQuantity, var normalDie) = SplitDieRoll(normal.Roll);
            (var criticalQuantity, var criticalDie) = SplitDieRoll(critical.Roll);

            if (noDamage.Contains(weapon))
            {
                Assert.That(criticalDie, Is.EqualTo(normalDie).And.EqualTo(1), $"TEST DATA: {key}: {criticalMultiplier}");
                Assert.That(criticalQuantity, Is.EqualTo(normalQuantity).And.Zero, $"TEST DATA: {key}: {criticalMultiplier}");
                return;
            }

            var multiplier = Convert.ToInt32(criticalMultiplier.Substring(1, 1));

            Assert.That(multiplier, Is.EqualTo(2).Or.EqualTo(3).Or.EqualTo(4), $"TEST DATA: {key}: {criticalMultiplier}");
            Assert.That(criticalDie, Is.EqualTo(normalDie), $"TEST DATA: {key}: {criticalMultiplier}");
            Assert.That(criticalQuantity, Is.EqualTo(Math.Max(normalQuantity * multiplier, 1)), $"TEST DATA: {key}: {criticalMultiplier}");
        }

        private (int Quantity, int Die) SplitDieRoll(string roll)
        {
            var sections = roll.Split('d');
            var quantity = Convert.ToInt32(sections[0]);
            var die = 1;
            if (sections.Length > 1)
                die = Convert.ToInt32(sections[1]);

            return (quantity, die);
        }

        private Dictionary<string, List<string>> GetWeaponCriticalDamages()
        {
            var damages = new Dictionary<string, List<string>>
            {
                [WeaponConstants.Dagger + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d2", Type = "Piercing or Slashing" })],
                [WeaponConstants.Dagger + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d3", Type = "Piercing or Slashing" })],
                [WeaponConstants.Dagger + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d4", Type = "Piercing or Slashing" })],
                [WeaponConstants.Dagger + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = "Piercing or Slashing" })],
                [WeaponConstants.Dagger + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d8", Type = "Piercing or Slashing" })],
                [WeaponConstants.Dagger + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = "Piercing or Slashing" })],
                [WeaponConstants.Dagger + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = "Piercing or Slashing" })],
                [WeaponConstants.Greataxe + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d8", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Greataxe + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d10", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Greataxe + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d12", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Greataxe + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "9d6", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Greataxe + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "12d6", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Greataxe + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "18d6", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Greataxe + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "24d6", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Greatsword + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d8", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Greatsword + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d10", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Greatsword + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Greatsword + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Greatsword + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "8d6", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Greatsword + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "12d6", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Greatsword + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "16d6", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Kama + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d3", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Kama + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d4", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Kama + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Kama + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d8", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Kama + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Kama + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Kama + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "8d6", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.LightMace + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d3", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.LightMace + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d4", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.LightMace + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.LightMace + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d8", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.LightMace + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.LightMace + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.LightMace + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "8d6", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.Longsword + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d4", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Longsword + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Longsword + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d8", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Longsword + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Longsword + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Longsword + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "8d6", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Longsword + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "12d6", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.HeavyMace + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d4", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.HeavyMace + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.HeavyMace + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d8", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.HeavyMace + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.HeavyMace + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.HeavyMace + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "8d6", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.HeavyMace + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "12d6", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.Quarterstaff + TraitConstants.Sizes.Tiny] = [
                    DataHelper.Parse(new DamageDataSelection { Roll = "2d3", Type = AttributeConstants.DamageTypes.Bludgeoning }),
                    DataHelper.Parse(new DamageDataSelection { Roll = "2d3", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.Quarterstaff + TraitConstants.Sizes.Small] = [
                    DataHelper.Parse(new DamageDataSelection { Roll = "2d4", Type = AttributeConstants.DamageTypes.Bludgeoning }),
                    DataHelper.Parse(new DamageDataSelection { Roll = "2d4", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.Quarterstaff + TraitConstants.Sizes.Medium] = [
                    DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = AttributeConstants.DamageTypes.Bludgeoning }),
                    DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.Quarterstaff + TraitConstants.Sizes.Large] = [
                    DataHelper.Parse(new DamageDataSelection { Roll = "2d8", Type = AttributeConstants.DamageTypes.Bludgeoning }),
                    DataHelper.Parse(new DamageDataSelection { Roll = "2d8", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.Quarterstaff + TraitConstants.Sizes.Huge] = [
                    DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = AttributeConstants.DamageTypes.Bludgeoning }),
                    DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.Quarterstaff + TraitConstants.Sizes.Gargantuan] = [
                    DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = AttributeConstants.DamageTypes.Bludgeoning }),
                    DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.Quarterstaff + TraitConstants.Sizes.Colossal] = [
                    DataHelper.Parse(new DamageDataSelection { Roll = "8d6", Type = AttributeConstants.DamageTypes.Bludgeoning }),
                    DataHelper.Parse(new DamageDataSelection { Roll = "8d6", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.BastardSword + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.BastardSword + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d8", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.BastardSword + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d10", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.BastardSword + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d8", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.BastardSword + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d8", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.BastardSword + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "8d8", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.BastardSword + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "12d8", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Nunchaku + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d3", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.Nunchaku + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d4", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.Nunchaku + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.Nunchaku + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d8", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.Nunchaku + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.Nunchaku + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.Nunchaku + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "8d6", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.Rapier + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d3", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Rapier + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d4", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Rapier + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Rapier + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d8", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Rapier + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Rapier + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Rapier + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "8d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Scimitar + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d3", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Scimitar + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d4", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Scimitar + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Scimitar + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d8", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Scimitar + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Scimitar + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Scimitar + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "8d6", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Shortspear + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d3", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Shortspear + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d4", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Shortspear + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Shortspear + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d8", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Shortspear + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Shortspear + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Shortspear + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "8d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Siangham + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d3", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Siangham + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d4", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Siangham + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Siangham + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d8", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Siangham + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Siangham + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Siangham + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "8d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.ShortSword + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d3", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.ShortSword + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d4", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.ShortSword + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.ShortSword + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d8", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.ShortSword + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.ShortSword + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.ShortSword + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "8d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.DwarvenWaraxe + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d6", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.DwarvenWaraxe + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d8", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.DwarvenWaraxe + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d10", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.DwarvenWaraxe + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d8", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.DwarvenWaraxe + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "9d8", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.DwarvenWaraxe + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "12d8", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.DwarvenWaraxe + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "18d8", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.OrcDoubleAxe + TraitConstants.Sizes.Tiny] = [
                    DataHelper.Parse(new DamageDataSelection { Roll = "3d4", Type = AttributeConstants.DamageTypes.Slashing }),
                    DataHelper.Parse(new DamageDataSelection { Roll = "3d4", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.OrcDoubleAxe + TraitConstants.Sizes.Small] = [
                    DataHelper.Parse(new DamageDataSelection { Roll = "3d6", Type = AttributeConstants.DamageTypes.Slashing }),
                    DataHelper.Parse(new DamageDataSelection { Roll = "3d6", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.OrcDoubleAxe + TraitConstants.Sizes.Medium] = [
                    DataHelper.Parse(new DamageDataSelection { Roll = "3d8", Type = AttributeConstants.DamageTypes.Slashing }),
                    DataHelper.Parse(new DamageDataSelection { Roll = "3d8", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.OrcDoubleAxe + TraitConstants.Sizes.Large] = [
                    DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = AttributeConstants.DamageTypes.Slashing }),
                    DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.OrcDoubleAxe + TraitConstants.Sizes.Huge] = [
                    DataHelper.Parse(new DamageDataSelection { Roll = "9d6", Type = AttributeConstants.DamageTypes.Slashing }),
                    DataHelper.Parse(new DamageDataSelection { Roll = "9d6", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.OrcDoubleAxe + TraitConstants.Sizes.Gargantuan] = [
                    DataHelper.Parse(new DamageDataSelection { Roll = "12d6", Type = AttributeConstants.DamageTypes.Slashing }),
                    DataHelper.Parse(new DamageDataSelection { Roll = "12d6", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.OrcDoubleAxe + TraitConstants.Sizes.Colossal] = [
                    DataHelper.Parse(new DamageDataSelection { Roll = "18d6", Type = AttributeConstants.DamageTypes.Slashing }),
                    DataHelper.Parse(new DamageDataSelection { Roll = "18d6", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Battleaxe + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d4", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Battleaxe + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d6", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Battleaxe + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d8", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Battleaxe + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Battleaxe + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "9d6", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Battleaxe + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "12d6", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Battleaxe + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "18d6", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.SpikedChain + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d4", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.SpikedChain + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.SpikedChain + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d4", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.SpikedChain + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.SpikedChain + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.SpikedChain + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "8d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.SpikedChain + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "12d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Club + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d3", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.Club + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d4", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.Club + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.Club + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d8", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.Club + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.Club + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.Club + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "8d6", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.HandCrossbow + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d2", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.HandCrossbow + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d3", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.HandCrossbow + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d4", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.HandCrossbow + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.HandCrossbow + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d8", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.HandCrossbow + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.HandCrossbow + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.HeavyRepeatingCrossbow + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.HeavyRepeatingCrossbow + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d8", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.HeavyRepeatingCrossbow + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d10", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.HeavyRepeatingCrossbow + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d8", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.HeavyRepeatingCrossbow + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d8", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.HeavyRepeatingCrossbow + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "8d8", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.HeavyRepeatingCrossbow + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "12d8", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.LightRepeatingCrossbow + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d4", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.LightRepeatingCrossbow + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.LightRepeatingCrossbow + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d8", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.LightRepeatingCrossbow + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.LightRepeatingCrossbow + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.LightRepeatingCrossbow + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "8d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.LightRepeatingCrossbow + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "12d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.PunchingDagger + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d2", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.PunchingDagger + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d3", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.PunchingDagger + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d4", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.PunchingDagger + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.PunchingDagger + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d8", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.PunchingDagger + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.PunchingDagger + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "9d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Falchion + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d4", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Falchion + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Falchion + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d4", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Falchion + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Falchion + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Falchion + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "8d6", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Falchion + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "12d6", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.DireFlail + TraitConstants.Sizes.Tiny] = [
                    DataHelper.Parse(new DamageDataSelection { Roll = "2d4", Type = AttributeConstants.DamageTypes.Bludgeoning }),
                    DataHelper.Parse(new DamageDataSelection { Roll = "2d4", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.DireFlail + TraitConstants.Sizes.Small] = [
                    DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = AttributeConstants.DamageTypes.Bludgeoning }),
                    DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.DireFlail + TraitConstants.Sizes.Medium] = [
                    DataHelper.Parse(new DamageDataSelection { Roll = "2d8", Type = AttributeConstants.DamageTypes.Bludgeoning }),
                    DataHelper.Parse(new DamageDataSelection { Roll = "2d8", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.DireFlail + TraitConstants.Sizes.Large] = [
                    DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = AttributeConstants.DamageTypes.Bludgeoning }),
                    DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.DireFlail + TraitConstants.Sizes.Huge] = [
                    DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = AttributeConstants.DamageTypes.Bludgeoning }),
                    DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.DireFlail + TraitConstants.Sizes.Gargantuan] = [
                    DataHelper.Parse(new DamageDataSelection { Roll = "8d6", Type = AttributeConstants.DamageTypes.Bludgeoning }),
                    DataHelper.Parse(new DamageDataSelection { Roll = "8d6", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.DireFlail + TraitConstants.Sizes.Colossal] = [
                    DataHelper.Parse(new DamageDataSelection { Roll = "12d6", Type = AttributeConstants.DamageTypes.Bludgeoning }),
                    DataHelper.Parse(new DamageDataSelection { Roll = "12d6", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.HeavyFlail + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.HeavyFlail + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d8", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.HeavyFlail + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d10", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.HeavyFlail + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d8", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.HeavyFlail + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d8", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.HeavyFlail + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "8d8", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.HeavyFlail + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "12d8", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.Flail + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d4", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.Flail + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.Flail + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d8", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.Flail + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.Flail + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.Flail + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "8d6", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.Flail + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "12d6", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.Gauntlet + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "2", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.Gauntlet + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d2", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.Gauntlet + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d3", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.Gauntlet + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d4", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.Gauntlet + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.Gauntlet + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d8", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.Gauntlet + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.SpikedGauntlet + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d2", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.SpikedGauntlet + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d3", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.SpikedGauntlet + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d4", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.SpikedGauntlet + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.SpikedGauntlet + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d8", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.SpikedGauntlet + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.SpikedGauntlet + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Glaive + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d6", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Glaive + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d8", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Glaive + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d10", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Glaive + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d8", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Glaive + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "9d8", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Glaive + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "12d8", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Glaive + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "18d8", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Greatclub + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.Greatclub + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d8", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.Greatclub + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d10", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.Greatclub + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d8", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.Greatclub + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d8", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.Greatclub + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "8d8", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.Greatclub + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "12d8", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.Guisarme + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d4", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Guisarme + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d6", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Guisarme + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d8", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Guisarme + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Guisarme + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "9d6", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Guisarme + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "12d6", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Guisarme + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "18d6", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Halberd + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d6", Type = "Piercing or Slashing" })],
                [WeaponConstants.Halberd + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d8", Type = "Piercing or Slashing" })],
                [WeaponConstants.Halberd + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d10", Type = "Piercing or Slashing" })],
                [WeaponConstants.Halberd + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d8", Type = "Piercing or Slashing" })],
                [WeaponConstants.Halberd + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "9d8", Type = "Piercing or Slashing" })],
                [WeaponConstants.Halberd + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "12d8", Type = "Piercing or Slashing" })],
                [WeaponConstants.Halberd + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "18d8", Type = "Piercing or Slashing" })],
                [WeaponConstants.Spear + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d4", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Spear + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Spear + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d8", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Spear + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Spear + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "9d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Spear + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "12d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Spear + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "18d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.GnomeHookedHammer + TraitConstants.Sizes.Tiny] = [
                    DataHelper.Parse(new DamageDataSelection { Roll = "3d4", Type = AttributeConstants.DamageTypes.Bludgeoning }),
                    DataHelper.Parse(new DamageDataSelection { Roll = "4d3", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.GnomeHookedHammer + TraitConstants.Sizes.Small] = [
                    DataHelper.Parse(new DamageDataSelection { Roll = "3d6", Type = AttributeConstants.DamageTypes.Bludgeoning }),
                    DataHelper.Parse(new DamageDataSelection { Roll = "4d4", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.GnomeHookedHammer + TraitConstants.Sizes.Medium] = [
                    DataHelper.Parse(new DamageDataSelection { Roll = "3d8", Type = AttributeConstants.DamageTypes.Bludgeoning }),
                    DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.GnomeHookedHammer + TraitConstants.Sizes.Large] = [
                    DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = AttributeConstants.DamageTypes.Bludgeoning }),
                    DataHelper.Parse(new DamageDataSelection { Roll = "4d8", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.GnomeHookedHammer + TraitConstants.Sizes.Huge] = [
                    DataHelper.Parse(new DamageDataSelection { Roll = "9d6", Type = AttributeConstants.DamageTypes.Bludgeoning }),
                    DataHelper.Parse(new DamageDataSelection { Roll = "8d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.GnomeHookedHammer + TraitConstants.Sizes.Gargantuan] = [
                    DataHelper.Parse(new DamageDataSelection { Roll = "12d6", Type = AttributeConstants.DamageTypes.Bludgeoning }),
                    DataHelper.Parse(new DamageDataSelection { Roll = "12d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.GnomeHookedHammer + TraitConstants.Sizes.Colossal] = [
                    DataHelper.Parse(new DamageDataSelection { Roll = "18d6", Type = AttributeConstants.DamageTypes.Bludgeoning }),
                    DataHelper.Parse(new DamageDataSelection { Roll = "16d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.LightHammer + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d2", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.LightHammer + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d3", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.LightHammer + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d4", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.LightHammer + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.LightHammer + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d8", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.LightHammer + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.LightHammer + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.Handaxe + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d3", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Handaxe + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d4", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Handaxe + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d6", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Handaxe + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d8", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Handaxe + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Handaxe + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "9d6", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Handaxe + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "12d6", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Kukri + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d2", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Kukri + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d3", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Kukri + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d4", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Kukri + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Kukri + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d8", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Kukri + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Kukri + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Lance + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d4", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Lance + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Lance + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d8", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Lance + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Lance + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "9d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Lance + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "12d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Lance + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "18d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Longspear + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d4", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Longspear + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Longspear + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d8", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Longspear + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Longspear + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "9d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Longspear + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "12d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Longspear + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "18d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Morningstar + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d4", Type = "Bludgeoning and Piercing" })],
                [WeaponConstants.Morningstar + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = "Bludgeoning and Piercing" })],
                [WeaponConstants.Morningstar + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d8", Type = "Bludgeoning and Piercing" })],
                [WeaponConstants.Morningstar + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = "Bludgeoning and Piercing" })],
                [WeaponConstants.Morningstar + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = "Bludgeoning and Piercing" })],
                [WeaponConstants.Morningstar + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "8d6", Type = "Bludgeoning and Piercing" })],
                [WeaponConstants.Morningstar + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "12d6", Type = "Bludgeoning and Piercing" })],
                [WeaponConstants.Net + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "0", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.Net + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "0", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.Net + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "0", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.Net + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "0", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.Net + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "0", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.Net + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "0", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.Net + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "0", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.HeavyPick + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d3", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.HeavyPick + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d4", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.HeavyPick + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.HeavyPick + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d8", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.HeavyPick + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "8d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.HeavyPick + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "12d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.HeavyPick + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "16d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.LightPick + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d2", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.LightPick + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d3", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.LightPick + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d4", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.LightPick + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.LightPick + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d8", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.LightPick + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "8d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.LightPick + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "12d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Sai + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d2", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.Sai + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d3", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.Sai + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d4", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.Sai + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.Sai + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d8", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.Sai + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.Sai + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.Bolas + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d2", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.Bolas + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d3", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.Bolas + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d4", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.Bolas + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.Bolas + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d8", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.Bolas + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.Bolas + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.Ranseur + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d4", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Ranseur + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Ranseur + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d4", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Ranseur + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Ranseur + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "9d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Ranseur + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "12d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Ranseur + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "18d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Sap + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d3", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.Sap + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d4", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.Sap + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.Sap + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d8", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.Sap + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.Sap + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.Sap + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "8d6", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.Scythe + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d4", Type = "Piercing or Slashing" })],
                [WeaponConstants.Scythe + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = "Piercing or Slashing" })],
                [WeaponConstants.Scythe + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "8d4", Type = "Piercing or Slashing" })],
                [WeaponConstants.Scythe + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "8d6", Type = "Piercing or Slashing" })],
                [WeaponConstants.Scythe + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "12d6", Type = "Piercing or Slashing" })],
                [WeaponConstants.Scythe + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "16d6", Type = "Piercing or Slashing" })],
                [WeaponConstants.Scythe + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "24d6", Type = "Piercing or Slashing" })],
                [WeaponConstants.Shuriken + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "1", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Shuriken + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "2", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Shuriken + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d2", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Shuriken + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d3", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Shuriken + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d4", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Shuriken + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Shuriken + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d8", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Sickle + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d3", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Sickle + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d4", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Sickle + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Sickle + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d8", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Sickle + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Sickle + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Sickle + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "8d6", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.TwoBladedSword + TraitConstants.Sizes.Tiny] = [
                    DataHelper.Parse(new DamageDataSelection { Roll = "2d4", Type = AttributeConstants.DamageTypes.Slashing }),
                    DataHelper.Parse(new DamageDataSelection { Roll = "2d4", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.TwoBladedSword + TraitConstants.Sizes.Small] = [
                    DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = AttributeConstants.DamageTypes.Slashing }),
                    DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.TwoBladedSword + TraitConstants.Sizes.Medium] = [
                    DataHelper.Parse(new DamageDataSelection { Roll = "2d8", Type = AttributeConstants.DamageTypes.Slashing }),
                    DataHelper.Parse(new DamageDataSelection { Roll = "2d8", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.TwoBladedSword + TraitConstants.Sizes.Large] = [
                    DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = AttributeConstants.DamageTypes.Slashing }),
                    DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.TwoBladedSword + TraitConstants.Sizes.Huge] = [
                    DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = AttributeConstants.DamageTypes.Slashing }),
                    DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.TwoBladedSword + TraitConstants.Sizes.Gargantuan] = [
                    DataHelper.Parse(new DamageDataSelection { Roll = "8d6", Type = AttributeConstants.DamageTypes.Slashing }),
                    DataHelper.Parse(new DamageDataSelection { Roll = "8d6", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.TwoBladedSword + TraitConstants.Sizes.Colossal] = [
                    DataHelper.Parse(new DamageDataSelection { Roll = "12d6", Type = AttributeConstants.DamageTypes.Slashing }),
                    DataHelper.Parse(new DamageDataSelection { Roll = "12d6", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Trident + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d4", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Trident + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Trident + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d8", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Trident + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Trident + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Trident + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "8d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Trident + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "12d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.DwarvenUrgrosh + TraitConstants.Sizes.Tiny] = [
                    DataHelper.Parse(new DamageDataSelection { Roll = "3d4", Type = AttributeConstants.DamageTypes.Slashing }),
                    DataHelper.Parse(new DamageDataSelection { Roll = "3d3", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.DwarvenUrgrosh + TraitConstants.Sizes.Small] = [
                    DataHelper.Parse(new DamageDataSelection { Roll = "3d6", Type = AttributeConstants.DamageTypes.Slashing }),
                    DataHelper.Parse(new DamageDataSelection { Roll = "3d4", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.DwarvenUrgrosh + TraitConstants.Sizes.Medium] = [
                    DataHelper.Parse(new DamageDataSelection { Roll = "3d8", Type = AttributeConstants.DamageTypes.Slashing }),
                    DataHelper.Parse(new DamageDataSelection { Roll = "3d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.DwarvenUrgrosh + TraitConstants.Sizes.Large] = [
                    DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = AttributeConstants.DamageTypes.Slashing }),
                    DataHelper.Parse(new DamageDataSelection { Roll = "3d8", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.DwarvenUrgrosh + TraitConstants.Sizes.Huge] = [
                    DataHelper.Parse(new DamageDataSelection { Roll = "9d6", Type = AttributeConstants.DamageTypes.Slashing }),
                    DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.DwarvenUrgrosh + TraitConstants.Sizes.Gargantuan] = [
                    DataHelper.Parse(new DamageDataSelection { Roll = "12d6", Type = AttributeConstants.DamageTypes.Slashing }),
                    DataHelper.Parse(new DamageDataSelection { Roll = "9d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.DwarvenUrgrosh + TraitConstants.Sizes.Colossal] = [
                    DataHelper.Parse(new DamageDataSelection { Roll = "18d6", Type = AttributeConstants.DamageTypes.Slashing }),
                    DataHelper.Parse(new DamageDataSelection { Roll = "12d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Warhammer + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d4", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.Warhammer + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d6", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.Warhammer + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d8", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.Warhammer + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.Warhammer + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "9d6", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.Warhammer + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "12d6", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.Warhammer + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "18d6", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.Whip + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "2", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Whip + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d2", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Whip + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d3", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Whip + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d4", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Whip + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Whip + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d8", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.Whip + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.ThrowingAxe + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d3", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.ThrowingAxe + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d4", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.ThrowingAxe + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.ThrowingAxe + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d8", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.ThrowingAxe + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.ThrowingAxe + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.ThrowingAxe + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "8d6", Type = AttributeConstants.DamageTypes.Slashing })],
                [WeaponConstants.HeavyCrossbow + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.HeavyCrossbow + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d8", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.HeavyCrossbow + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d10", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.HeavyCrossbow + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d8", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.HeavyCrossbow + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d8", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.HeavyCrossbow + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "8d8", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.HeavyCrossbow + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "12d8", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.LightCrossbow + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d4", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.LightCrossbow + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.LightCrossbow + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d8", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.LightCrossbow + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.LightCrossbow + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.LightCrossbow + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "8d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.LightCrossbow + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "12d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Dart + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d2", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Dart + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d3", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Dart + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d4", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Dart + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Dart + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d8", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Dart + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Dart + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Javelin + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d3", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Javelin + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d4", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Javelin + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Javelin + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d8", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Javelin + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Javelin + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Javelin + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "8d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Shortbow + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d3", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Shortbow + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d4", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Shortbow + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Shortbow + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d8", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Shortbow + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Shortbow + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "9d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Shortbow + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "12d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.CompositeShortbow + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d3", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.CompositeShortbow + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d4", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.CompositeShortbow + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.CompositeShortbow + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d8", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.CompositeShortbow + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.CompositeShortbow + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "9d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.CompositeShortbow + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "12d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Sling + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d2", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.Sling + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d3", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.Sling + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d4", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.Sling + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.Sling + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d8", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.Sling + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d6", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.Sling + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.Longbow + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d4", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Longbow + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Longbow + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d8", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Longbow + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Longbow + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "9d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Longbow + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "12d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Longbow + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "18d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.CompositeLongbow + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d4", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.CompositeLongbow + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.CompositeLongbow + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "3d8", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.CompositeLongbow + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.CompositeLongbow + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "9d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.CompositeLongbow + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "12d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.CompositeLongbow + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "18d6", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Arrow + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "0", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Arrow + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "0", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Arrow + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "0", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Arrow + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "0", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Arrow + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "0", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Arrow + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "0", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.Arrow + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "0", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.CrossbowBolt + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "0", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.CrossbowBolt + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "0", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.CrossbowBolt + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "0", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.CrossbowBolt + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "0", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.CrossbowBolt + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "0", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.CrossbowBolt + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "0", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.CrossbowBolt + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "0", Type = AttributeConstants.DamageTypes.Piercing })],
                [WeaponConstants.SlingBullet + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "0", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.SlingBullet + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "0", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.SlingBullet + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "0", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.SlingBullet + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "0", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.SlingBullet + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "0", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.SlingBullet + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "0", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.SlingBullet + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "0", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.PincerStaff + TraitConstants.Sizes.Tiny] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d6", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.PincerStaff + TraitConstants.Sizes.Small] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d8", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.PincerStaff + TraitConstants.Sizes.Medium] = [DataHelper.Parse(new DamageDataSelection { Roll = "2d10", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.PincerStaff + TraitConstants.Sizes.Large] = [DataHelper.Parse(new DamageDataSelection { Roll = "4d8", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.PincerStaff + TraitConstants.Sizes.Huge] = [DataHelper.Parse(new DamageDataSelection { Roll = "6d8", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.PincerStaff + TraitConstants.Sizes.Gargantuan] = [DataHelper.Parse(new DamageDataSelection { Roll = "8d8", Type = AttributeConstants.DamageTypes.Bludgeoning })],
                [WeaponConstants.PincerStaff + TraitConstants.Sizes.Colossal] = [DataHelper.Parse(new DamageDataSelection { Roll = "12d8", Type = AttributeConstants.DamageTypes.Bludgeoning })],
            };

            return damages;
        }

        public static IEnumerable Weapons => WeaponConstants.GetAllWeapons(false, false).Select(w => new TestCaseData(w));
    }
}
