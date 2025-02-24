using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Selectors.Percentiles;
using DnDGen.TreasureGen.Tables;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.TreasureGen.Generators.Items.Magical
{
    internal class RingGenerator : MagicalItemGenerator
    {
        private readonly ICollectionSelector collectionsSelector;
        private readonly ISpellGenerator spellGenerator;
        private readonly IChargesGenerator chargesGenerator;
        private readonly ITreasurePercentileTypeAndAmountSelector typeAndAmountPercentileSelector;
        private readonly IReplacementSelector replacementSelector;

        public RingGenerator(
            ICollectionSelector collectionsSelector,
            ISpellGenerator spellGenerator,
            IChargesGenerator chargesGenerator,
            ITreasurePercentileTypeAndAmountSelector typeAndAmountPercentileSelector,
            IReplacementSelector replacementSelector)
        {
            this.collectionsSelector = collectionsSelector;
            this.spellGenerator = spellGenerator;
            this.chargesGenerator = chargesGenerator;
            this.typeAndAmountPercentileSelector = typeAndAmountPercentileSelector;
            this.replacementSelector = replacementSelector;
        }

        public Item GenerateRandom(string power)
        {
            var result = typeAndAmountPercentileSelector.SelectFrom(Config.Name, TableNameConstants.Percentiles.POWERITEMTYPEs(power, ItemTypeConstants.Ring));

            var ring = BuildRing(result.Type, power);
            ring.Magic.Bonus = result.Amount;

            return ring;
        }

        public Item Generate(string power, string itemName, params string[] traits)
        {
            var possiblePowers = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, itemName);
            var adjustedPower = PowerHelper.AdjustPower(power, possiblePowers);

            var tableName = TableNameConstants.Percentiles.POWERITEMTYPEs(adjustedPower, ItemTypeConstants.Ring);
            var results = typeAndAmountPercentileSelector.SelectAllFrom(Config.Name, tableName);
            var matches = results.Where(r => NameMatches(r.Type, itemName));

            var result = collectionsSelector.SelectRandomFrom(matches);

            var ring = BuildRing(result.Type, power, traits);
            ring.Magic.Bonus = result.Amount;

            return ring;
        }

        private bool NameMatches(string source, string target)
        {
            var sourceReplacements = replacementSelector.SelectAll(source);
            var targetReplacements = replacementSelector.SelectAll(target);

            return source == target
                || sourceReplacements.Any(s => s == target)
                || targetReplacements.Any(t => t == source);
        }

        private Item BuildRing(string name, string power, params string[] traits)
        {
            var ring = new Item
            {
                Name = name,
                BaseNames = [name],
                IsMagical = true,
                ItemType = ItemTypeConstants.Ring,
                Traits = new HashSet<string>(traits)
            };

            ring.Attributes = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Collections.ITEMTYPEAttributes(ring.ItemType), name);

            if (ring.Attributes.Contains(AttributeConstants.Charged))
                ring.Magic.Charges = chargesGenerator.GenerateFor(ItemTypeConstants.Ring, name);

            if (ring.Name == RingConstants.Counterspells)
            {
                var level = spellGenerator.GenerateLevel(power);
                if (level <= 6)
                {
                    var type = spellGenerator.GenerateType();
                    var spell = spellGenerator.Generate(type, level);
                    ring.Contents.Add(spell);
                }
            }
            else if (ring.Name == RingConstants.SpellStoring_Minor)
            {
                var spells = GenerateSpells(power, 3);
                ring.Contents.AddRange(spells);
            }
            else if (ring.Name == RingConstants.SpellStoring_Major)
            {
                var spells = GenerateSpells(power, 10);
                ring.Contents.AddRange(spells);
            }
            else if (ring.Name == RingConstants.SpellStoring)
            {
                var spells = GenerateSpells(power, 5);
                ring.Contents.AddRange(spells);
            }

            return ring;
        }

        private IEnumerable<string> GenerateSpells(string power, int levelCap)
        {
            var level = spellGenerator.GenerateLevel(power);
            var levelSum = level;
            var spells = new List<string>();

            while (levelSum <= levelCap)
            {
                var type = spellGenerator.GenerateType();
                var spell = spellGenerator.Generate(type, level);
                var formattedSpell = $"{spell} ({type}, {level})";
                spells.Add(formattedSpell);

                level = spellGenerator.GenerateLevel(power);
                levelSum += level;
            }

            return spells;
        }

        public Item Generate(Item template, bool allowRandomDecoration = false)
        {
            var ring = template.Clone();
            ring.BaseNames = [ring.Name];
            ring.ItemType = ItemTypeConstants.Ring;
            ring.Quantity = 1;
            ring.IsMagical = true;
            ring.Attributes = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Collections.ITEMTYPEAttributes(ItemTypeConstants.Ring), ring.Name);

            return ring.SmartClone();
        }
    }
}