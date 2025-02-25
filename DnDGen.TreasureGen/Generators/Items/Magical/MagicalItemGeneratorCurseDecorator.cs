using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Tables;

namespace DnDGen.TreasureGen.Generators.Items.Magical
{
    internal class MagicalItemGeneratorCurseDecorator : MagicalItemGenerator
    {
        private readonly MagicalItemGenerator innerGenerator;
        private readonly ICurseGenerator curseGenerator;

        public MagicalItemGeneratorCurseDecorator(MagicalItemGenerator innerGenerator, ICurseGenerator curseGenerator)
        {
            this.innerGenerator = innerGenerator;
            this.curseGenerator = curseGenerator;
        }

        public Item GenerateRandom(string power)
        {
            var item = innerGenerator.GenerateRandom(power);

            if (curseGenerator.HasCurse(item))
            {
                var canBeSpecific = curseGenerator.ItemTypeCanBeSpecificCursedItem(item.ItemType);

                do item.Magic.Curse = curseGenerator.GenerateCurse();
                while (item.Magic.Curse == TableNameConstants.Percentiles.SpecificCursedItems && !canBeSpecific);

                if (item.Magic.Curse == TableNameConstants.Percentiles.SpecificCursedItems && canBeSpecific)
                {
                    return curseGenerator.GenerateSpecificCursedItem(item.ItemType);
                }
            }

            return item;
        }

        public Item Generate(string power, string itemName, params string[] traits)
        {
            if (curseGenerator.IsSpecificCursedItem(itemName))
                return curseGenerator.Generate(itemName, traits);

            var item = innerGenerator.Generate(power, itemName, traits);

            if (curseGenerator.HasCurse(item))
            {
                var canBeSpecific = curseGenerator.CanBeSpecificCursedItem(itemName);

                do item.Magic.Curse = curseGenerator.GenerateCurse();
                while (item.Magic.Curse == TableNameConstants.Percentiles.SpecificCursedItems && !canBeSpecific);

                if (item.Magic.Curse == TableNameConstants.Percentiles.SpecificCursedItems && canBeSpecific)
                {
                    var cursedItem = curseGenerator.Generate(itemName, traits);
                    return cursedItem;
                }
            }

            return item;
        }

        public Item Generate(Item template, bool allowRandomDecoration = false)
        {
            if (curseGenerator.IsSpecificCursedItem(template))
                return curseGenerator.Generate(template);

            var item = innerGenerator.Generate(template, allowRandomDecoration);

            if (allowRandomDecoration && curseGenerator.HasCurse(item))
            {
                do item.Magic.Curse = curseGenerator.GenerateCurse();
                while (item.Magic.Curse == TableNameConstants.Percentiles.SpecificCursedItems);
            }

            return item;
        }
    }
}