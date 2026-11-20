using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.RollGen;
using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Mundane;
using DnDGen.TreasureGen.Selectors.Collections;
using DnDGen.TreasureGen.Selectors.Percentiles;
using DnDGen.TreasureGen.Tables;
using System;
using System.Linq;

namespace DnDGen.TreasureGen.Generators.Items.Mundane
{
    internal class MundaneWeaponGenerator(
        ITreasurePercentileSelector percentileSelector,
        ICollectionSelector collectionsSelector,
        Dice dice,
        IWeaponDataSelector weaponDataSelector,
        IReplacementSelector replacementSelector) : MundaneItemGenerator
    {
        public Item GenerateRandom()
        {
            var name = GetRandomName();
            return Generate(name);
        }

        public Item Generate(string itemName, params string[] traits)
        {
            var weapon = GeneratePrototype(itemName);
            weapon.Traits = [.. traits];

            weapon = GenerateFromPrototype(weapon, true);

            return weapon;
        }

        private string GetRandomName()
        {
            var type = percentileSelector.SelectFrom(Config.Name, TableNameConstants.Percentiles.MundaneWeaponTypes);
            var tableName = TableNameConstants.Percentiles.WEAPONTYPEWeapons(type);
            var weaponName = percentileSelector.SelectFrom(Config.Name, tableName);

            return weaponName;
        }

        private Weapon GeneratePrototype(string name)
        {
            var weapon = new Weapon
            {
                Name = name,
                BaseNames = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Collections.ItemGroups, name),
                Quantity = 0
            };

            return weapon;
        }

        private Weapon GenerateFromPrototype(Weapon prototype, bool allowDecoration)
        {
            var weapon = SetWeaponAttributes(prototype);
            weapon.Quantity = GetQuantity(weapon);

            if (allowDecoration)
            {
                var isMasterwork = percentileSelector.SelectFrom<bool>(Config.Name, TableNameConstants.Percentiles.IsMasterwork);
                if (isMasterwork)
                    weapon.Traits.Add(TraitConstants.Masterwork);
            }

            return weapon;
        }

        private Weapon SetWeaponAttributes(Weapon weapon)
        {
            if (string.IsNullOrEmpty(weapon.Name))
                throw new ArgumentException("Weapon name cannot be empty - it must be filled before calling this method");

            weapon.Size = GetSize(weapon);
            weapon.Traits.Remove(weapon.Size);
            weapon.ItemType = ItemTypeConstants.Weapon;

            if (NameMatches(weapon.Name, WeaponConstants.CompositeLongbow)
                || NameMatches(weapon.Name, WeaponConstants.CompositeShortbow))
            {
                var oldName = weapon.Name;
                weapon.Name = replacementSelector.SelectSingle(oldName);
                var compositeStrengthBonus = GetCompositeBowBonus(oldName);

                if (!string.IsNullOrEmpty(compositeStrengthBonus))
                {
                    weapon.Traits.Add(compositeStrengthBonus);
                }
                else if (!weapon.Traits.Any(t => t.Contains("Strength")))
                {
                    var isLongbow = NameMatches(weapon.Name, WeaponConstants.CompositeLongbow);
                    var allCommonRangedWeapons = percentileSelector.SelectAllFrom(Config.Name, TableNameConstants.Percentiles.WEAPONTYPEWeapons("CommonRanged"));
                    var compositeBows = allCommonRangedWeapons
                        .Where(w => isLongbow ? NameMatches(w, WeaponConstants.CompositeLongbow) : NameMatches(w, WeaponConstants.CompositeShortbow));
                    var randomCompositeBow = collectionsSelector.SelectRandomFrom(compositeBows);

                    compositeStrengthBonus = GetCompositeBowBonus(randomCompositeBow);
                    weapon.Traits.Add(compositeStrengthBonus);
                }
            }

            var tableName = TableNameConstants.Collections.ITEMTYPEAttributes(ItemTypeConstants.Weapon);
            weapon.Attributes = collectionsSelector.SelectFrom(Config.Name, tableName, weapon.Name);

            var weaponSelection = weaponDataSelector.Select(weapon.Name, weapon.Size);
            if (weapon.Damages.Count == 0)
                weapon.Damages.Add(Damage.From(weaponSelection.Damages[0]));

            if (weapon.CriticalDamages.Count == 0)
                weapon.CriticalDamages.Add(Damage.From(weaponSelection.CriticalDamages[0]));

            if (weapon.IsDoubleWeapon && weapon.SecondaryDamages.Count == 0)
                weapon.SecondaryDamages.Add(Damage.From(weaponSelection.Damages[1]));

            if (weapon.IsDoubleWeapon && weapon.SecondaryCriticalDamages.Count == 0)
                weapon.SecondaryCriticalDamages.Add(Damage.From(weaponSelection.CriticalDamages[1]));

            weapon.ThreatRange = weaponSelection.ThreatRange;
            weapon.Ammunition = weaponSelection.Ammunition;
            weapon.CriticalMultiplier = weaponSelection.CriticalMultiplier;

            if (weapon.IsDoubleWeapon && string.IsNullOrEmpty(weapon.SecondaryCriticalMultiplier))
                weapon.SecondaryCriticalMultiplier = weaponSelection.SecondaryCriticalMultiplier;

            return weapon;
        }

        private bool NameMatches(string source, string target)
        {
            var sourceReplacement = replacementSelector.SelectSingle(source);
            var targetReplacement = replacementSelector.SelectSingle(target);

            return source == target
                || sourceReplacement == target
                || targetReplacement == source;
        }

        private int GetQuantity(Weapon weapon)
        {
            if (weapon.Quantity > 0)
                return weapon.Quantity;

            if (weapon.NameMatches(WeaponConstants.Net))
                return 1;

            if (weapon.Attributes.Contains(AttributeConstants.Ammunition))
            {
                var roll = dice.Roll().Percentile().AsSum();
                return Math.Max(1, roll / 2);
            }

            if (weapon.Attributes.Contains(AttributeConstants.Thrown) && !weapon.Attributes.Contains(AttributeConstants.Melee))
                return dice.Roll().d20().AsSum();

            return 1;
        }

        private static string GetCompositeBowBonus(string weaponName)
        {
            var compositeBonusStartIndex = weaponName.IndexOf('+');
            if (compositeBonusStartIndex == -1)
                return string.Empty;

            var compositeBonus = weaponName.Substring(compositeBonusStartIndex, 2);
            return $"Allows up to {compositeBonus} Strength bonus on damage";
        }

        public Item Generate(Item template, bool allowRandomDecoration = false)
        {
            template.BaseNames = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Collections.ItemGroups, template.Name);

            var weapon = new Weapon();

            if (template is Weapon)
                weapon = template.MundaneClone() as Weapon;
            else
                template.MundaneCloneInto(weapon);

            weapon = GenerateFromPrototype(weapon, allowRandomDecoration);

            return weapon;
        }

        private string GetSize(Weapon template)
        {
            if (!string.IsNullOrEmpty(template.Size))
                return template.Size;

            if (template.Traits.Count == 0)
                return GetRandomSize();

            var allSizes = percentileSelector.SelectAllFrom(Config.Name, TableNameConstants.Percentiles.MundaneGearSizes);
            var sizeTraits = template.Traits.Intersect(allSizes);

            if (sizeTraits.Any())
                return sizeTraits.Single();

            return GetRandomSize();
        }

        private string GetRandomSize()
        {
            return percentileSelector.SelectFrom(Config.Name, TableNameConstants.Percentiles.MundaneGearSizes);
        }
    }
}