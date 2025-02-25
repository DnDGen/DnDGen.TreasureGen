using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Items.Mundane;
using DnDGen.TreasureGen.Tables;
using System.Linq;

namespace DnDGen.TreasureGen.Generators.Items.Magical
{
    internal class MagicalItemGeneratorSpecialMaterialDecorator : MagicalItemGenerator
    {
        private readonly MagicalItemGenerator innerGenerator;
        private readonly ISpecialMaterialGenerator specialMaterialGenerator;
        private readonly ICollectionSelector collectionsSelector;

        public MagicalItemGeneratorSpecialMaterialDecorator(MagicalItemGenerator innerGenerator, ISpecialMaterialGenerator specialMaterialGenerator, ICollectionSelector collectionsSelector)
        {
            this.innerGenerator = innerGenerator;
            this.specialMaterialGenerator = specialMaterialGenerator;
            this.collectionsSelector = collectionsSelector;
        }

        public Item Generate(Item template, bool allowRandomDecoration = false)
        {
            var item = innerGenerator.Generate(template, allowRandomDecoration);

            item = AddSpecialMaterials(item, allowRandomDecoration);

            return item;
        }

        private Item AddSpecialMaterials(Item item, bool allowMaterials)
        {
            while (allowMaterials && specialMaterialGenerator.CanHaveSpecialMaterial(item.ItemType, item.Attributes, item.Traits))
            {
                var material = specialMaterialGenerator.GenerateFor(item.ItemType, item.Attributes, item.Traits);
                item.Traits.Add(material);
            }

            var masterworkMaterials = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Collections.SpecialMaterials, TraitConstants.Masterwork);
            if (item.Traits.Intersect(masterworkMaterials).Any())
                item.Traits.Add(TraitConstants.Masterwork);

            if (item.Traits.Contains(TraitConstants.SpecialMaterials.Dragonhide))
            {
                var metalAndWood = new[] { AttributeConstants.Metal, AttributeConstants.Wood };
                item.Attributes = item.Attributes.Except(metalAndWood);
            }

            return item;
        }

        public Item GenerateRandom(string power)
        {
            var item = innerGenerator.GenerateRandom(power);

            if (item.Magic.Curse == CurseConstants.SpecificCursedItem)
            {
                var specialMaterials = TraitConstants.SpecialMaterials.GetAll();
                foreach (var specialMaterial in specialMaterials)
                {
                    item.Traits.Remove(specialMaterial);
                }

                return item;
            }

            item = AddSpecialMaterials(item, true);

            return item;
        }

        public Item Generate(string power, string itemName, params string[] traits)
        {
            var item = innerGenerator.Generate(power, itemName, traits);

            if (item.Magic.Curse == CurseConstants.SpecificCursedItem)
            {
                var specialMaterials = TraitConstants.SpecialMaterials.GetAll();
                foreach (var specialMaterial in specialMaterials)
                {
                    item.Traits.Remove(specialMaterial);
                }

                return item;
            }

            item = AddSpecialMaterials(item, true);

            return item;
        }
    }
}