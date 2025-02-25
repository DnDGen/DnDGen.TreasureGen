using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Selectors.Percentiles;
using DnDGen.TreasureGen.Selectors.Selections;
using DnDGen.TreasureGen.Tables;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.TreasureGen.Generators.Items.Magical
{
    internal class SpecialAbilitiesGenerator : ISpecialAbilitiesGenerator
    {
        private const int MaxBonus = 10;

        private readonly ICollectionSelector collectionsSelector;
        private readonly ICollectionDataSelector<DamageDataSelection> damageDataSelector;
        private readonly ICollectionDataSelector<SpecialAbilityDataSelection> specialAbilityDataSelector;
        private readonly ITreasurePercentileSelector percentileSelector;
        private readonly ISpellGenerator spellGenerator;

        public const string BonusSpecialAbility = "BonusSpecialAbility";

        public SpecialAbilitiesGenerator(
            ICollectionSelector collectionsSelector,
            ITreasurePercentileSelector percentileSelector,
            ICollectionDataSelector<SpecialAbilityDataSelection> specialAbilityDataSelector,
            ICollectionDataSelector<DamageDataSelection> damageDataSelector,
            ISpellGenerator spellGenerator)
        {
            this.collectionsSelector = collectionsSelector;
            this.percentileSelector = percentileSelector;
            this.specialAbilityDataSelector = specialAbilityDataSelector;
            this.damageDataSelector = damageDataSelector;
            this.spellGenerator = spellGenerator;
        }

        public IEnumerable<SpecialAbility> GenerateFor(Item targetItem, string power, int quantity)
        {
            if (targetItem.Magic.Bonus <= 0 || quantity <= 0)
                return [];

            var tableNames = GetTableNames(targetItem, power);
            var bonusSum = targetItem.Magic.Bonus;
            var availableAbilities = GetAvailableAbilities(targetItem, tableNames, bonusSum);
            var abilities = new List<SpecialAbility>();

            while (quantity > 0 && availableAbilities.Count > 0)
            {
                if (CanHaveAllAvailableAbilities(quantity, bonusSum, availableAbilities))
                {
                    var strongestAbilities = GetStrongestAvailableAbilities(availableAbilities);
                    var duplicates = abilities.Where(a => strongestAbilities.Any(sA => sA.BaseName == a.BaseName));
                    abilities = abilities.Except(duplicates).ToList();

                    abilities.AddRange(strongestAbilities);
                    availableAbilities.Clear();
                    continue;
                }

                var ability = GenerateAbilityFrom(availableAbilities, tableNames);
                if (ability.Name == BonusSpecialAbility)
                {
                    quantity++;
                    continue;
                }

                if (abilities.Any(a => a.BaseName == ability.BaseName))
                {
                    var previousAbility = abilities.First(a => a.BaseName == ability.BaseName);
                    bonusSum -= previousAbility.BonusEquivalent;
                    abilities.Remove(previousAbility);
                }

                quantity--;
                bonusSum += ability.BonusEquivalent;
                abilities.Add(ability);
                availableAbilities.Remove(ability);

                var weakerAbilities = availableAbilities.Where(a => a.BaseName == ability.BaseName && a.Power < ability.Power);
                availableAbilities = availableAbilities.Except(weakerAbilities).ToList();

                var tooStrongAbilities = availableAbilities.Where(a => a.BonusEquivalent + bonusSum > 10);
                availableAbilities = availableAbilities.Except(tooStrongAbilities).ToList();
            }

            return abilities;
        }

        private IEnumerable<string> GetTableNames(Item targetItem, string power)
        {
            var tableNames = new List<string>();

            if (targetItem.Attributes.Contains(AttributeConstants.Melee))
            {
                var tableName = TableNameConstants.Percentiles.POWERATTRIBUTESpecialAbilities(power, AttributeConstants.Melee);
                tableNames.Add(tableName);
            }

            if (targetItem.Attributes.Contains(AttributeConstants.Ranged))
            {
                var tableName = TableNameConstants.Percentiles.POWERATTRIBUTESpecialAbilities(power, AttributeConstants.Ranged);
                tableNames.Add(tableName);
            }

            if (targetItem.Attributes.Contains(AttributeConstants.Shield))
            {
                var tableName = TableNameConstants.Percentiles.POWERATTRIBUTESpecialAbilities(power, AttributeConstants.Shield);
                tableNames.Add(tableName);
            }
            else if (targetItem.ItemType == ItemTypeConstants.Armor)
            {
                var tableName = TableNameConstants.Percentiles.POWERATTRIBUTESpecialAbilities(power, ItemTypeConstants.Armor);
                tableNames.Add(tableName);
            }

            return tableNames;
        }

        private List<SpecialAbility> GetAvailableAbilities(Item targetItem, IEnumerable<string> tableNames, int bonus)
        {
            var availableAbilities = new List<SpecialAbility>();
            var weapon = targetItem as Weapon;

            foreach (var tableName in tableNames)
            {
                var abilityNames = percentileSelector.SelectAllFrom(Config.Name, tableName);

                foreach (var abilityName in abilityNames)
                {
                    if (abilityName == BonusSpecialAbility)
                        continue;

                    var ability = GetSpecialAbility(abilityName, weapon?.CriticalMultiplier ?? string.Empty);
                    if (ability.RequirementsMet(targetItem) && bonus + ability.BonusEquivalent <= 10)
                        availableAbilities.Add(ability);
                }
            }

            return availableAbilities;
        }

        private SpecialAbility GetSpecialAbility(string abilityName, string criticalMultiplier)
        {
            var ability = new SpecialAbility();
            var abilitySelection = specialAbilityDataSelector.SelectOneFrom(Config.Name, TableNameConstants.Collections.SpecialAbilityData, abilityName);

            ability.Name = abilityName;
            ability.BaseName = abilitySelection.BaseName;
            ability.AttributeRequirements = collectionsSelector.SelectFrom(
                Config.Name,
                TableNameConstants.Collections.SpecialAbilityAttributeRequirements,
                ability.BaseName);
            ability.BonusEquivalent = abilitySelection.BonusEquivalent;
            ability.Power = abilitySelection.Power;

            var damagesData = damageDataSelector.SelectFrom(Config.Name, TableNameConstants.Collections.AbilityDamages, abilityName);
            ability.Damages.AddRange(damagesData.Select(Damage.From));

            if (!string.IsNullOrEmpty(criticalMultiplier))
            {
                var critDamagesData = damageDataSelector.SelectFrom(Config.Name, TableNameConstants.Collections.AbilityDamages, abilityName + criticalMultiplier);
                ability.CriticalDamages.AddRange(critDamagesData.Select(Damage.From));
            }

            return ability;
        }

        private bool CanHaveAllAvailableAbilities(int quantity, int bonusSum, IEnumerable<SpecialAbility> availableAbilities)
        {
            return quantity >= availableAbilities.Count() && availableAbilities.Sum(a => a.BonusEquivalent) + bonusSum <= MaxBonus;
        }

        private IEnumerable<SpecialAbility> GetStrongestAvailableAbilities(IEnumerable<SpecialAbility> availableAbilities)
        {
            var strongestAbilities = new List<SpecialAbility>();

            foreach (var ability in availableAbilities)
            {
                if (IsCustomSpecialAbility(ability.Name))
                {
                    strongestAbilities.Add(ability);
                    continue;
                }

                var alreadyAdded = strongestAbilities.Any(a => a.BaseName == ability.BaseName && a.Power == ability.Power && a.Name == ability.Name);
                if (alreadyAdded)
                    continue;

                var max = availableAbilities.Where(a => a.BaseName == ability.BaseName).Max(a => a.Power);

                if (ability.Power == max)
                    strongestAbilities.Add(ability);
            }

            return strongestAbilities;
        }

        private bool IsCustomSpecialAbility(string abilityName)
        {
            return !specialAbilityDataSelector.IsCollection(Config.Name, TableNameConstants.Collections.SpecialAbilityData, abilityName);
        }

        private SpecialAbility GenerateAbilityFrom(IEnumerable<SpecialAbility> availableAbilities, IEnumerable<string> tableNames)
        {
            var abilityName = string.Empty;

            do
            {
                var tableName = collectionsSelector.SelectRandomFrom(tableNames);

                abilityName = percentileSelector.SelectFrom(Config.Name, tableName);

                if (abilityName == BonusSpecialAbility)
                    return new SpecialAbility { Name = abilityName };
            } while (!availableAbilities.Any(a => a.Name == abilityName));

            return availableAbilities.First(a => a.Name == abilityName);
        }

        public IEnumerable<SpecialAbility> GenerateFor(IEnumerable<SpecialAbility> abilityPrototypes, string criticalMultiplier)
        {
            var abilities = new List<SpecialAbility>();

            foreach (var abilityPrototype in abilityPrototypes)
            {
                if (IsCustomSpecialAbility(abilityPrototype.Name))
                {
                    abilities.Add(abilityPrototype);
                    continue;
                }

                var ability = GetSpecialAbility(abilityPrototype.Name, criticalMultiplier);
                abilities.Add(ability);
            }

            var strongestAbilities = GetStrongestAvailableAbilities(abilities);
            return strongestAbilities;
        }

        public Weapon ApplyAbilitiesToWeapon(Weapon weapon)
        {
            if (weapon.Magic.SpecialAbilities.Any(a => a.Name == SpecialAbilityConstants.SpellStoring))
            {
                var shouldStoreSpell = percentileSelector.SelectFrom<bool>(Config.Name, TableNameConstants.Percentiles.SpellStoringContainsSpell);

                if (shouldStoreSpell)
                {
                    var spellType = spellGenerator.GenerateType();
                    var level = spellGenerator.GenerateLevel(PowerConstants.Minor);
                    var spell = spellGenerator.Generate(spellType, level);

                    weapon.Contents.Add(spell);
                }
            }

            foreach (var specialAbility in weapon.Magic.SpecialAbilities)
            {
                var weaponDamages = GetWeaponDamages(specialAbility.Damages, weapon.Damages[0].Type);
                weapon.Damages.AddRange(weaponDamages);

                var weaponCritDamages = GetWeaponDamages(specialAbility.CriticalDamages, weapon.CriticalDamages[0].Type);
                weapon.CriticalDamages.AddRange(weaponCritDamages);

                if (specialAbility.Name == SpecialAbilityConstants.Keen)
                {
                    weapon.ThreatRange *= 2;
                }
            }

            if (weapon.IsDoubleWeapon && weapon.SecondaryHasAbilities)
            {
                var secondaryAbilities = GenerateFor(weapon.Magic.SpecialAbilities, weapon.SecondaryCriticalMultiplier);

                foreach (var specialAbility in secondaryAbilities)
                {
                    var weaponDamages = GetWeaponDamages(specialAbility.Damages, weapon.SecondaryDamages[0].Type);
                    weapon.SecondaryDamages.AddRange(weaponDamages);

                    var weaponCritDamages = GetWeaponDamages(specialAbility.CriticalDamages, weapon.SecondaryCriticalDamages[0].Type);
                    weapon.SecondaryCriticalDamages.AddRange(weaponCritDamages);
                }
            }

            return weapon;
        }

        private IEnumerable<Damage> GetWeaponDamages(IEnumerable<Damage> abilityDamages, string weaponDamageType)
        {
            if (!abilityDamages.Any())
                return [];

            var damages = abilityDamages.Select(d => d.Clone()).ToArray();
            foreach (var damage in damages)
            {
                if (string.IsNullOrEmpty(damage.Type))
                {
                    damage.Type = weaponDamageType;
                }
            }

            return damages;
        }
    }
}