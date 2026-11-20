using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;

namespace DnDGen.TreasureGen.Generators.Items.Magical
{
    internal class MagicalItemGeneratorIntelligenceDecorator(MagicalItemGenerator innerGenerator, IIntelligenceGenerator intelligenceGenerator) : MagicalItemGenerator
    {
        public Item Generate(Item template, bool allowRandomDecoration = false)
        {
            var item = innerGenerator.Generate(template, allowRandomDecoration);
            var canBeIntelligent = intelligenceGenerator.CanBeIntelligent(item.Attributes, item.IsMagical);
            var isIntelligent = intelligenceGenerator.IsIntelligent(item.ItemType, item.Attributes, item.IsMagical);

            if (allowRandomDecoration && isIntelligent)
            {
                item.Magic.Intelligence = intelligenceGenerator.GenerateFor(item);
            }
            else if (!canBeIntelligent)
            {
                item.Magic.Intelligence = new Intelligence();
            }

            return item;
        }

        public Item GenerateRandom(string power)
        {
            var item = innerGenerator.GenerateRandom(power);

            if (intelligenceGenerator.IsIntelligent(item.ItemType, item.Attributes, item.IsMagical))
                item.Magic.Intelligence = intelligenceGenerator.GenerateFor(item);

            return item;
        }

        public Item Generate(string power, string itemName, params string[] traits)
        {
            var item = innerGenerator.Generate(power, itemName, traits);

            if (intelligenceGenerator.IsIntelligent(item.ItemType, item.Attributes, item.IsMagical))
                item.Magic.Intelligence = intelligenceGenerator.GenerateFor(item);

            return item;
        }
    }
}