﻿using System.Collections.Generic;
using System.Linq;
using TreasureGen.Domain.Items.Mundane;
using TreasureGen.Items;
using TreasureGen.Items.Magical;

namespace TreasureGen.Domain.Generators.Items.Magical
{
    internal class MagicalItemGeneratorSpecialMaterialDecorator : MagicalItemGenerator
    {
        private readonly MagicalItemGenerator innerGenerator;
        private readonly ISpecialMaterialGenerator specialMaterialGenerator;
        private readonly IEnumerable<string> masterworkMaterials;

        public MagicalItemGeneratorSpecialMaterialDecorator(MagicalItemGenerator innerGenerator, ISpecialMaterialGenerator specialMaterialGenerator)
        {
            this.innerGenerator = innerGenerator;
            this.specialMaterialGenerator = specialMaterialGenerator;

            masterworkMaterials = new[]
            {
                TraitConstants.SpecialMaterials.Adamantine,
                TraitConstants.SpecialMaterials.Darkwood,
                TraitConstants.SpecialMaterials.Dragonhide,
                TraitConstants.SpecialMaterials.Mithral,
            };
        }

        public Item Generate(Item template, bool allowRandomDecoration = false)
        {
            var item = innerGenerator.Generate(template, allowRandomDecoration);

            item = AddSpecialMaterials(item, allowRandomDecoration);

            if (item.Traits.Intersect(masterworkMaterials).Any())
                item.Traits.Add(TraitConstants.Masterwork);

            return item;
        }

        private Item AddSpecialMaterials(Item item, bool allowMaterials)
        {
            while (allowMaterials && specialMaterialGenerator.CanHaveSpecialMaterial(item.ItemType, item.Attributes, item.Traits))
            {
                var material = specialMaterialGenerator.GenerateFor(item.ItemType, item.Attributes, item.Traits);
                item.Traits.Add(material);

                if (material == TraitConstants.SpecialMaterials.Dragonhide)
                {
                    var metalAndWood = new[] { AttributeConstants.Metal, AttributeConstants.Wood };
                    item.Attributes = item.Attributes.Except(metalAndWood);
                }
            }

            if (item.Traits.Intersect(masterworkMaterials).Any())
                item.Traits.Add(TraitConstants.Masterwork);

            return item;
        }

        public Item GenerateAtPower(string power)
        {
            var item = innerGenerator.GenerateAtPower(power);

            if (item.Magic.Curse == CurseConstants.SpecificCursedItem)
                return item;

            item = AddSpecialMaterials(item, true);

            return item;
        }

        public Item GenerateFromSubset(string power, IEnumerable<string> subset)
        {
            var item = innerGenerator.GenerateFromSubset(power, subset);

            if (item.Magic.Curse == CurseConstants.SpecificCursedItem)
                return item;

            item = AddSpecialMaterials(item, true);

            return item;
        }
    }
}