using DnDGen.Infrastructure.Selectors.Percentiles;
using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Selectors.Percentiles;
using DnDGen.TreasureGen.Tables;
using DnDGen.TreasureGen.Tests.Integration.Generators.Items;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.TreasureGen.Tests.Integration.Tables.Items
{
    [TestFixture]
    public class PowerGroupsTests : CollectionsTests
    {
        private IPercentileTypeAndAmountSelector typeAndAmountPercentileSelector;
        private IReplacementSelector replacementSelector;
        private IPercentileSelector percentileSelector;

        protected override string tableName => TableNameConstants.Collections.PowerGroups;

        [SetUp]
        public void Setup()
        {
            typeAndAmountPercentileSelector = GetNewInstanceOf<IPercentileTypeAndAmountSelector>();
            replacementSelector = GetNewInstanceOf<IReplacementSelector>();
            percentileSelector = GetNewInstanceOf<IPercentileSelector>();
        }

        [TestCase(ItemTypeConstants.AlchemicalItem,
            PowerConstants.Mundane)]
        [TestCase(ItemTypeConstants.Armor,
            PowerConstants.Mundane,
            PowerConstants.Minor,
            PowerConstants.Medium,
            PowerConstants.Major)]
        [TestCase(ItemTypeConstants.Potion,
            PowerConstants.Minor,
            PowerConstants.Medium,
            PowerConstants.Major)]
        [TestCase(ItemTypeConstants.Ring,
            PowerConstants.Minor,
            PowerConstants.Medium,
            PowerConstants.Major)]
        [TestCase(ItemTypeConstants.Rod,
            PowerConstants.Medium,
            PowerConstants.Major)]
        [TestCase(ItemTypeConstants.Scroll,
            PowerConstants.Minor,
            PowerConstants.Medium,
            PowerConstants.Major)]
        [TestCase(ItemTypeConstants.Staff,
            PowerConstants.Medium,
            PowerConstants.Major)]
        [TestCase(ItemTypeConstants.Tool,
            PowerConstants.Mundane)]
        [TestCase(ItemTypeConstants.Wand,
            PowerConstants.Minor,
            PowerConstants.Medium,
            PowerConstants.Major)]
        [TestCase(ItemTypeConstants.Weapon,
            PowerConstants.Mundane,
            PowerConstants.Minor,
            PowerConstants.Medium,
            PowerConstants.Major)]
        [TestCase(ItemTypeConstants.WondrousItem,
            PowerConstants.Minor,
            PowerConstants.Medium,
            PowerConstants.Major)]
        public void PowerGroup(string name, params string[] powers)
        {
            base.AssertCollection(name, powers);
        }

        [TestCaseSource(typeof(ItemTestData), nameof(ItemTestData.AlchemicalItems))]
        public void AlchemicalItemPowerGroupsMatch(string itemName)
        {
            var powers = table[ItemTypeConstants.AlchemicalItem];
            base.AssertCollection(itemName, powers.ToArray());
        }

        [TestCaseSource(typeof(ItemTestData), nameof(ItemTestData.Armors))]
        public void ArmorPowerGroupsMatch(string itemName)
        {
            var possiblePowers = new List<string>();
            var powers = table[ItemTypeConstants.Armor];
            possiblePowers.AddRange(powers);

            var generalArmors = ArmorConstants.GetAllArmorsAndShields(false);
            var shields = ArmorConstants.GetAllShields(true);
            var cursed = percentileSelector.SelectAllFrom(Name, TableNameConstants.Percentiles.SpecificCursedItems);

            if (!generalArmors.Contains(itemName))
            {
                var gearType = shields.Contains(itemName) ? AttributeConstants.Shield : ItemTypeConstants.Armor;
                possiblePowers.Remove(PowerConstants.Mundane);

                if (!cursed.Contains(itemName))
                {
                    foreach (var power in powers.Except([PowerConstants.Mundane]))
                    {
                        var tableName = TableNameConstants.Percentiles.POWERSpecificITEMTYPEs(power, gearType);
                        var results = typeAndAmountPercentileSelector.SelectAllFrom(Config.Name, tableName);

                        if (!results.Any(r => NameMatchesWithReplacements(r.Type, itemName)))
                            possiblePowers.Remove(power);
                    }
                }
            }

            base.AssertCollection(itemName, [.. possiblePowers]);
        }

        [TestCaseSource(typeof(ItemTestData), nameof(ItemTestData.Potions))]
        public void PotionPowerGroupsMatch(string itemName)
        {
            var possiblePowers = new List<string>();
            var powers = table[ItemTypeConstants.Potion];
            var cursed = percentileSelector.SelectAllFrom(Name, TableNameConstants.Percentiles.SpecificCursedItems);

            foreach (var power in powers)
            {
                var tableName = TableNameConstants.Percentiles.POWERITEMTYPEs(power, ItemTypeConstants.Potion);
                var results = typeAndAmountPercentileSelector.SelectAllFrom(Config.Name, tableName);

                if (results.Any(r => NameMatchesWithReplacements(r.Type, itemName))
                    || cursed.Contains(itemName))
                {
                    possiblePowers.Add(power);
                }
            }

            base.AssertCollection(itemName, [.. possiblePowers]);
        }

        private bool NameMatchesWithReplacements(string source, string target)
        {
            var sourceReplacements = replacementSelector.SelectAll(source, true);
            var targetReplacements = replacementSelector.SelectAll(target, true);

            return source == target
                || sourceReplacements.Any(s => s == target)
                || targetReplacements.Any(t => t == source);
        }

        [TestCaseSource(typeof(ItemTestData), nameof(ItemTestData.Rings))]
        public void RingPowerGroupsMatch(string itemName)
        {
            var possiblePowers = new List<string>();
            var powers = table[ItemTypeConstants.Ring];
            var cursed = percentileSelector.SelectAllFrom(Name, TableNameConstants.Percentiles.SpecificCursedItems);

            foreach (var power in powers)
            {
                var tableName = TableNameConstants.Percentiles.POWERITEMTYPEs(power, ItemTypeConstants.Ring);
                var results = typeAndAmountPercentileSelector.SelectAllFrom(Config.Name, tableName);

                if (results.Any(r => NameMatchesWithReplacements(r.Type, itemName))
                    || cursed.Contains(itemName))
                    possiblePowers.Add(power);
            }

            base.AssertCollection(itemName, [.. possiblePowers]);
        }

        [TestCaseSource(typeof(ItemTestData), nameof(ItemTestData.Rods))]
        public void RodPowerGroupsMatch(string itemName)
        {
            var possiblePowers = new List<string>();
            var powers = table[ItemTypeConstants.Rod];
            var cursed = percentileSelector.SelectAllFrom(Name, TableNameConstants.Percentiles.SpecificCursedItems);

            foreach (var power in powers)
            {
                var tableName = TableNameConstants.Percentiles.POWERITEMTYPEs(power, ItemTypeConstants.Rod);
                var results = typeAndAmountPercentileSelector.SelectAllFrom(Config.Name, tableName);

                if (results.Any(r => NameMatchesWithReplacements(r.Type, itemName)))
                    possiblePowers.Add(power);
            }

            base.AssertCollection(itemName, [.. possiblePowers]);
        }

        [TestCaseSource(typeof(ItemTestData), nameof(ItemTestData.Staffs))]
        public void StaffPowerGroupsMatch(string itemName)
        {
            var possiblePowers = new List<string>();
            var powers = table[ItemTypeConstants.Staff];
            var cursed = percentileSelector.SelectAllFrom(Name, TableNameConstants.Percentiles.SpecificCursedItems);

            foreach (var power in powers)
            {
                var tableName = TableNameConstants.Percentiles.POWERITEMTYPEs(power, ItemTypeConstants.Staff);
                var results = typeAndAmountPercentileSelector.SelectAllFrom(Config.Name, tableName);

                if (results.Any(r => NameMatchesWithReplacements(r.Type, itemName)))
                    possiblePowers.Add(power);
            }

            base.AssertCollection(itemName, [.. possiblePowers]);
        }

        [TestCaseSource(typeof(ItemTestData), nameof(ItemTestData.Tools))]
        public void ToolPowerGroupsMatch(string itemName)
        {
            var powers = table[ItemTypeConstants.Tool];
            base.AssertCollection(itemName, powers.ToArray());
        }

        [TestCaseSource(typeof(ItemTestData), nameof(ItemTestData.Weapons))]
        public void WeaponPowerGroupsMatch(string itemName)
        {
            var possiblePowers = new List<string>();
            var powers = table[ItemTypeConstants.Weapon];
            possiblePowers.AddRange(powers);

            var generalWeapons = WeaponConstants.GetAllWeapons(false, true);
            var cursed = percentileSelector.SelectAllFrom(Name, TableNameConstants.Percentiles.SpecificCursedItems);

            if (!generalWeapons.Contains(itemName))
            {
                possiblePowers.Remove(PowerConstants.Mundane);

                if (!cursed.Contains(itemName))
                {
                    foreach (var power in powers.Except([PowerConstants.Mundane]))
                    {
                        var tableName = TableNameConstants.Percentiles.POWERSpecificITEMTYPEs(power, ItemTypeConstants.Weapon);
                        var results = typeAndAmountPercentileSelector.SelectAllFrom(Config.Name, tableName);

                        if (!results.Any(r => NameMatchesWithReplacements(r.Type, itemName)))
                            possiblePowers.Remove(power);
                    }
                }
            }

            base.AssertCollection(itemName, [.. possiblePowers]);
        }

        [TestCaseSource(typeof(ItemTestData), nameof(ItemTestData.WondrousItems))]
        public void WondrousItemPowerGroupsMatch(string itemName)
        {
            var possiblePowers = new List<string>();
            var powers = table[ItemTypeConstants.WondrousItem];
            var cursed = percentileSelector.SelectAllFrom(Name, TableNameConstants.Percentiles.SpecificCursedItems);

            foreach (var power in powers)
            {
                var tableName = TableNameConstants.Percentiles.POWERITEMTYPEs(power, ItemTypeConstants.WondrousItem);
                var results = typeAndAmountPercentileSelector.SelectAllFrom(Config.Name, tableName);

                if (results.Any(r => NameMatchesWithReplacements(r.Type, itemName))
                    || cursed.Contains(itemName))
                    possiblePowers.Add(power);
            }

            base.AssertCollection(itemName, [.. possiblePowers]);
        }
    }
}
