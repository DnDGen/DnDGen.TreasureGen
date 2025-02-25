using DnDGen.Infrastructure.Factories;
using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.Infrastructure.Selectors.Percentiles;
using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Items.Mundane;
using DnDGen.TreasureGen.Tables;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.TreasureGen.Generators.Items.Magical
{
    internal class StaffGenerator : MagicalItemGenerator
    {
        private readonly IPercentileTypeAndAmountSelector typeAndAmountPercentileSelector;
        private readonly IChargesGenerator chargesGenerator;
        private readonly ICollectionSelector collectionsSelector;
        private readonly ISpecialAbilitiesGenerator specialAbilitiesGenerator;
        private readonly JustInTimeFactory justInTimeFactory;

        public StaffGenerator(IPercentileTypeAndAmountSelector typeAndAmountPercentileSelector,
            IChargesGenerator chargesGenerator,
            ICollectionSelector collectionsSelector,
            ISpecialAbilitiesGenerator specialAbilitiesGenerator,
            JustInTimeFactory justInTimeFactory)
        {
            this.typeAndAmountPercentileSelector = typeAndAmountPercentileSelector;
            this.chargesGenerator = chargesGenerator;
            this.collectionsSelector = collectionsSelector;
            this.specialAbilitiesGenerator = specialAbilitiesGenerator;
            this.justInTimeFactory = justInTimeFactory;
        }

        public Item GenerateRandom(string power)
        {
            var rodPowers = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, ItemTypeConstants.Staff);
            var adjustedPower = PowerHelper.AdjustPower(power, rodPowers);

            var tablename = TableNameConstants.Percentiles.POWERITEMTYPEs(adjustedPower, ItemTypeConstants.Staff);
            var selection = typeAndAmountPercentileSelector.SelectFrom(Config.Name, tablename);

            return GenerateStaff(selection.Type, selection.Amount);
        }

        private Item GenerateStaff(string name, int bonus, params string[] traits)
        {
            var staff = new Item
            {
                Name = name
            };
            staff.Magic.Bonus = bonus;
            staff.Traits = new HashSet<string>(traits);

            staff = BuildStaff(staff);
            staff.Magic.Charges = chargesGenerator.GenerateFor(staff.ItemType, name);

            return staff;
        }

        public Item Generate(string power, string itemName, params string[] traits)
        {
            var staffName = GetStaffName(itemName);
            var powers = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, staffName);
            var adjustedPower = PowerHelper.AdjustPower(power, powers);

            var tablename = TableNameConstants.Percentiles.POWERITEMTYPEs(adjustedPower, ItemTypeConstants.Staff);
            var selections = typeAndAmountPercentileSelector.SelectAllFrom(Config.Name, tablename);
            var matches = selections.Where(s => s.Type == staffName).ToList();

            var selection = collectionsSelector.SelectRandomFrom(matches);
            return GenerateStaff(selection.Type, selection.Amount, traits);
        }

        private string GetStaffName(string itemName)
        {
            var staffs = StaffConstants.GetAllStaffs();
            if (staffs.Contains(itemName))
                return itemName;

            var staffFromBaseName = collectionsSelector.FindCollectionOf(Config.Name, TableNameConstants.Collections.ItemGroups, itemName, staffs.ToArray());

            return staffFromBaseName;
        }

        private Item BuildStaff(Item staff)
        {
            staff.ItemType = ItemTypeConstants.Staff;
            staff.Attributes = [AttributeConstants.Charged, AttributeConstants.OneTimeUse];
            staff.Quantity = 1;
            staff.IsMagical = true;
            staff.BaseNames = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Collections.ItemGroups, staff.Name);

            staff = GetWeapon(staff);

            return staff;
        }

        private Item GetWeapon(Item staff)
        {
            var weapons = WeaponConstants.GetAllMelee(false, false);
            if (!weapons.Intersect(staff.BaseNames).Any())
                return staff;

            var weaponName = weapons.Intersect(staff.BaseNames).First();

            var weaponGenerator = justInTimeFactory.Build<MundaneItemGenerator>(ItemTypeConstants.Weapon);
            var weapon = weaponGenerator.Generate(weaponName, [.. staff.Traits]) as Weapon;

            staff.Attributes = staff.Attributes.Union(weapon.Attributes).Except([AttributeConstants.OneTimeUse]);
            staff.CloneInto(weapon);

            if (weapon.IsMagical)
                weapon.Traits.Add(TraitConstants.Masterwork);

            if (weapon.IsDoubleWeapon)
            {
                weapon.SecondaryHasAbilities = true;
                weapon.SecondaryMagicBonus = staff.Magic.Bonus;
            }

            weapon = specialAbilitiesGenerator.ApplyAbilitiesToWeapon(weapon);

            return weapon;
        }

        public Item Generate(Item template, bool allowRandomDecoration = false)
        {
            var staff = template.Clone();

            staff.Magic.Intelligence = template.Magic.Intelligence.Clone();

            var baseNames = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Collections.ItemGroups, staff.Name);
            var weapons = WeaponConstants.GetAllMelee(false, false);
            var weaponBaseNames = weapons.Intersect(baseNames);
            if (weaponBaseNames.Any())
            {
                var weaponName = weaponBaseNames.First();
                var mundaneWeaponGenerator = justInTimeFactory.Build<MundaneItemGenerator>(ItemTypeConstants.Weapon);
                var mundaneWeapon = mundaneWeaponGenerator.Generate(weaponName, [.. staff.Traits]) as Weapon;

                staff.Magic.SpecialAbilities = specialAbilitiesGenerator.GenerateFor(staff.Magic.SpecialAbilities, mundaneWeapon.CriticalMultiplier);
            }

            staff = BuildStaff(staff);

            return staff.SmartClone();
        }
    }
}