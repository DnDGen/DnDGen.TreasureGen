using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.Infrastructure.Selectors.Percentiles;
using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Selectors.Percentiles;
using DnDGen.TreasureGen.Tables;
using System.Linq;

namespace DnDGen.TreasureGen.Generators.Items.Magical
{
    internal class PotionGenerator : MagicalItemGenerator
    {
        private readonly IPercentileTypeAndAmountSelector typeAndAmountPercentileSelector;
        private readonly ICollectionSelector collectionSelector;
        private readonly IReplacementSelector replacementSelector;

        public PotionGenerator(
            IPercentileTypeAndAmountSelector typeAndAmountPercentileSelector,
            ICollectionSelector collectionSelector,
            IReplacementSelector replacementSelector)
        {
            this.typeAndAmountPercentileSelector = typeAndAmountPercentileSelector;
            this.collectionSelector = collectionSelector;
            this.replacementSelector = replacementSelector;
        }

        public Item GenerateRandom(string power)
        {
            var result = typeAndAmountPercentileSelector.SelectFrom(Config.Name, TableNameConstants.Percentiles.POWERITEMTYPEs(power, ItemTypeConstants.Potion));

            return GeneratePotion(result.Type, result.Amount);
        }

        private Item GeneratePotion(string itemName, int bonus, params string[] traits)
        {
            var potion = new Item
            {
                Name = itemName,
                BaseNames = [itemName],
                ItemType = ItemTypeConstants.Potion
            };
            potion.Magic.Bonus = bonus;
            potion.IsMagical = true;
            potion.Attributes = [AttributeConstants.OneTimeUse];
            potion.Traits = [.. traits];

            return potion;
        }

        public Item Generate(string power, string itemName, params string[] traits)
        {
            var possiblePowers = collectionSelector.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, itemName);
            var adjustedPower = PowerHelper.AdjustPower(power, possiblePowers);

            var results = typeAndAmountPercentileSelector.SelectAllFrom(Config.Name, TableNameConstants.Percentiles.POWERITEMTYPEs(adjustedPower, ItemTypeConstants.Potion));
            var matches = results.Where(r => NameMatches(itemName, r.Type));
            var result = collectionSelector.SelectRandomFrom(matches);
            var potionName = GetName(itemName, result.Type);

            return GeneratePotion(potionName, result.Amount, traits);
        }

        private bool NameMatches(string source, string target)
        {
            var sourceReplacements = replacementSelector.SelectAll(source);
            var targetReplacements = replacementSelector.SelectAll(target);

            return source == target
                || sourceReplacements.Any(s => s == target)
                || targetReplacements.Any(t => t == source);
        }

        private string GetName(string source, string target)
        {
            var targetReplacements = replacementSelector.SelectAll(target);

            if (source == target || targetReplacements.Contains(source))
                return source;

            return target;
        }

        public Item Generate(Item template, bool allowRandomDecoration = false)
        {
            var potion = template.Clone();
            potion.BaseNames = [potion.Name];
            potion.ItemType = ItemTypeConstants.Potion;
            potion.Attributes = [AttributeConstants.OneTimeUse];
            potion.IsMagical = true;
            potion.Quantity = 1;

            return potion.SmartClone();
        }
    }
}