using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;

namespace DnDGen.TreasureGen.Generators.Items.Magical
{
    internal class MagicalItemGeneratorTraitsDecorator(MagicalItemGenerator innerGenerator, IMagicalItemTraitsGenerator traitsGenerator) : MagicalItemGenerator
    {
        public Item Generate(Item template, bool allowRandomDecoration = false)
        {
            var item = innerGenerator.Generate(template, allowRandomDecoration);

            if (allowRandomDecoration)
            {
                item = AddTraits(item);
            }

            return item;
        }

        private Item AddTraits(Item item)
        {
            var traits = traitsGenerator.GenerateFor(item.ItemType, item.Attributes);

            foreach (var trait in traits)
                item.Traits.Add(trait);

            return item;
        }

        public Item GenerateRandom(string power)
        {
            var item = innerGenerator.GenerateRandom(power);
            if (!item.IsMagical)
                return item;

            item = AddTraits(item);

            return item;
        }

        public Item Generate(string power, string itemName, params string[] traits)
        {
            var item = innerGenerator.Generate(power, itemName, traits);
            if (!item.IsMagical)
                return item;

            item = AddTraits(item);

            return item;
        }
    }
}