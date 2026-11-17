using DnDGen.RollGen;
using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using System;

namespace DnDGen.TreasureGen.Generators.Items.Magical
{
    internal class ScrollGenerator(Dice dice, ISpellGenerator spellGenerator) : MagicalItemGenerator
    {
        public Item Generate(Item template, bool allowRandomDecoration = false)
        {
            var scroll = template.Clone();
            scroll.IsMagical = true;
            scroll.Quantity = 1;
            scroll.ItemType = ItemTypeConstants.Scroll;
            scroll.Attributes = [AttributeConstants.OneTimeUse];
            scroll.BaseNames = [ItemTypeConstants.Scroll];

            return scroll.SmartClone();
        }

        public Item GenerateRandom(string power)
        {
            var spellType = spellGenerator.GenerateType();
            var scroll = new Item
            {
                Name = ItemTypeConstants.Scroll,
                BaseNames = [ItemTypeConstants.Scroll],
                ItemType = ItemTypeConstants.Scroll,
                IsMagical = true,
                Attributes = [AttributeConstants.OneTimeUse]
            };
            scroll.Traits.Add(spellType);

            var quantity = GetQuantity(power);
            while (quantity-- > 0)
            {
                var level = spellGenerator.GenerateLevel(power);
                var spell = spellGenerator.Generate(spellType, level);
                var spellWithLevel = $"{spell} ({level})";
                scroll.Contents.Add(spellWithLevel);
            }

            return scroll;
        }

        public Item Generate(string power, string itemName, params string[] traits)
        {
            var scroll = GenerateRandom(power);
            scroll.Name = itemName;

            foreach (var trait in traits)
                scroll.Traits.Add(trait);

            return scroll;
        }

        private int GetQuantity(string power)
        {
            return power switch
            {
                PowerConstants.Minor => dice.Roll().d3().AsSum(),
                PowerConstants.Medium => dice.Roll().d4().AsSum(),
                PowerConstants.Major => dice.Roll().d6().AsSum(),
                _ => throw new ArgumentException($"Cannot generate scroll of power '{power}'"),
            };
        }
    }
}