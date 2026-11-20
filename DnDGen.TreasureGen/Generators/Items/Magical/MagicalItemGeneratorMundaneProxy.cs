using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using System;

namespace DnDGen.TreasureGen.Generators.Items.Magical
{
    internal class MagicalItemGeneratorMundaneProxy(MagicalItemGenerator innerGenerator) : MagicalItemGenerator
    {
        public Item Generate(Item template, bool allowRandomDecoration = false)
        {
            return innerGenerator.Generate(template, allowRandomDecoration);
        }

        public Item GenerateRandom(string power)
        {
            if (power == PowerConstants.Mundane)
                throw new ArgumentException("Cannot generate a magical item of mundane power");

            return innerGenerator.GenerateRandom(power);
        }

        public Item Generate(string power, string itemName, params string[] traits)
        {
            if (power == PowerConstants.Mundane)
                throw new ArgumentException("Cannot generate a magical item of mundane power");

            return innerGenerator.Generate(power, itemName, traits);
        }
    }
}