using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Mundane;
using DnDGen.TreasureGen.Selectors.Percentiles;
using DnDGen.TreasureGen.Selectors.Selections;
using DnDGen.TreasureGen.Tables;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.TreasureGen.Generators.Items.Mundane
{
    internal class MundaneArmorGenerator : MundaneItemGenerator
    {
        private readonly ITreasurePercentileSelector percentileSelector;
        private readonly ICollectionSelector collectionsSelector;
        private readonly ICollectionDataSelector<ArmorDataSelection> armorDataSelector;

        public MundaneArmorGenerator(
            ITreasurePercentileSelector percentileSelector,
            ICollectionSelector collectionsSelector,
            ICollectionDataSelector<ArmorDataSelection> armorDataSelector)
        {
            this.percentileSelector = percentileSelector;
            this.collectionsSelector = collectionsSelector;
            this.armorDataSelector = armorDataSelector;
        }

        public Item GenerateRandom()
        {
            var name = GetRandomName();
            return Generate(name);
        }

        public Item Generate(string itemName, params string[] traits)
        {
            var armor = GeneratePrototype(itemName);
            armor.Traits = new HashSet<string>(traits);

            armor = GenerateFromPrototype(armor, true);

            return armor;
        }

        private Armor SetArmorAttributes(Armor armor)
        {
            armor.ItemType = ItemTypeConstants.Armor;
            armor.Quantity = 1;

            var tableName = TableNameConstants.Collections.ITEMTYPEAttributes(armor.ItemType);
            armor.Attributes = collectionsSelector.SelectFrom(Config.Name, tableName, armor.Name);

            armor.Size = GetSize(armor);
            armor.Traits.Remove(armor.Size);

            var armorSelection = armorDataSelector.SelectOneFrom(Config.Name, TableNameConstants.Collections.ArmorData, armor.Name);
            armor.ArmorBonus = armorSelection.ArmorBonus;
            armor.ArmorCheckPenalty = armorSelection.ArmorCheckPenalty;
            armor.MaxDexterityBonus = armorSelection.MaxDexterityBonus;

            return armor;
        }

        public Item Generate(Item template, bool allowRandomDecoration = false)
        {
            template.BaseNames = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Collections.ItemGroups, template.Name);

            var armor = new Armor();

            if (template is Armor)
                armor = template.MundaneClone() as Armor;
            else
                template.MundaneCloneInto(armor);

            armor = GenerateFromPrototype(armor, allowRandomDecoration);

            return armor;
        }

        private string GetSize(Armor template)
        {
            if (!string.IsNullOrEmpty(template.Size))
                return template.Size;

            if (!template.Traits.Any())
                return GetRandomSize();

            var allSizes = percentileSelector.SelectAllFrom(Config.Name, TableNameConstants.Percentiles.MundaneGearSizes);
            var sizeTraits = template.Traits.Intersect(allSizes);

            if (sizeTraits.Any())
                return sizeTraits.Single();

            return GetRandomSize();
        }

        private string GetRandomSize()
        {
            return percentileSelector.SelectFrom(Config.Name, TableNameConstants.Percentiles.MundaneGearSizes);
        }

        private Armor GenerateFromPrototype(Armor prototype, bool allowDecoration)
        {
            var armor = SetArmorAttributes(prototype);

            if (allowDecoration)
            {
                var isMasterwork = percentileSelector.SelectFrom<bool>(Config.Name, TableNameConstants.Percentiles.IsMasterwork);
                if (isMasterwork)
                    armor.Traits.Add(TraitConstants.Masterwork);
            }

            return armor;
        }

        private string GetRandomName()
        {
            var name = percentileSelector.SelectFrom(Config.Name, TableNameConstants.Percentiles.MundaneArmors);

            if (name == AttributeConstants.Shield)
                name = percentileSelector.SelectFrom(Config.Name, TableNameConstants.Percentiles.MundaneShields);

            return name;
        }

        private Armor GeneratePrototype(string itemName)
        {
            var armor = new Armor();
            armor.Name = itemName;
            armor.BaseNames = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Collections.ItemGroups, itemName);

            return armor;
        }
    }
}