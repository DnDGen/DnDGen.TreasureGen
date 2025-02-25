using DnDGen.Infrastructure.Factories;
using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.Infrastructure.Selectors.Percentiles;
using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Items.Mundane;
using DnDGen.TreasureGen.Selectors.Percentiles;
using DnDGen.TreasureGen.Tables;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.TreasureGen.Generators.Items.Magical
{
    internal class MagicalArmorGenerator : MagicalItemGenerator
    {
        private readonly IPercentileTypeAndAmountSelector typeAndAmountPercentileSelector;
        private readonly ITreasurePercentileSelector percentileSelector;
        private readonly ICollectionSelector collectionsSelector;
        private readonly ISpecialAbilitiesGenerator specialAbilitiesGenerator;
        private readonly ISpecificGearGenerator specificGearGenerator;
        private readonly JustInTimeFactory justInTimeFactory;

        public const string SpecialAbility = "SpecialAbility";
        public const int SpecificBonus = -1;

        public MagicalArmorGenerator(
            ITreasurePercentileSelector percentileSelector,
            ICollectionSelector collectionsSelector,
            ISpecialAbilitiesGenerator specialAbilitiesGenerator,
            ISpecificGearGenerator specificGearGenerator,
            JustInTimeFactory justInTimeFactory,
            IPercentileTypeAndAmountSelector typeAndAmountPercentileSelector)
        {
            this.percentileSelector = percentileSelector;
            this.collectionsSelector = collectionsSelector;
            this.specialAbilitiesGenerator = specialAbilitiesGenerator;
            this.specificGearGenerator = specificGearGenerator;
            this.justInTimeFactory = justInTimeFactory;
            this.typeAndAmountPercentileSelector = typeAndAmountPercentileSelector;
        }

        public Item GenerateRandom(string power)
        {
            return GenerateArmor(power);
        }

        public Item Generate(string power, string itemName, params string[] traits)
        {
            var armorType = GetArmorType(itemName);
            var isSpecific = specificGearGenerator.IsSpecific(armorType, itemName);

            return GenerateArmor(power, itemName, armorType, isSpecific, traits);
        }

        private string GetArmorType(string itemName)
        {
            if (specificGearGenerator.IsSpecific(AttributeConstants.Shield, itemName))
            {
                return AttributeConstants.Shield;
            }

            if (specificGearGenerator.IsSpecific(ItemTypeConstants.Armor, itemName))
            {
                return ItemTypeConstants.Armor;
            }

            var tableName = TableNameConstants.Collections.ITEMTYPEAttributes(ItemTypeConstants.Armor);
            var attributes = collectionsSelector.SelectFrom(Config.Name, tableName, itemName);

            if (attributes.Contains(AttributeConstants.Shield))
            {
                return AttributeConstants.Shield;
            }

            return ItemTypeConstants.Armor;
        }

        private Item GenerateArmor(string power, string itemName = "", string armorType = "", bool isSpecific = false, params string[] traits)
        {
            var prototype = GeneratePrototype(power, itemName, armorType, isSpecific, traits);
            var armor = GenerateFromPrototype(prototype);

            if (!specificGearGenerator.IsSpecific(armor))
            {
                var abilityCount = armor.Magic.SpecialAbilities.Count();
                armor.Magic.SpecialAbilities = specialAbilitiesGenerator.GenerateFor(armor, power, abilityCount);
            }

            return armor;
        }

        private Armor GeneratePrototype(string power, string itemName, string armorType, bool isSpecific, params string[] traits)
        {
            var prototype = new Armor();

            if (isSpecific)
            {
                var specificItem = specificGearGenerator.GeneratePrototypeFrom(power, armorType, itemName, traits);
                specificItem.CloneInto(prototype);

                return prototype;
            }

            var tableName = TableNameConstants.Percentiles.POWERITEMTYPEs(power, ItemTypeConstants.Armor);
            var randomArmorType = string.Empty;
            var bonus = 0;
            var abilityCount = 0;

            do
            {
                var typeAndAmount = typeAndAmountPercentileSelector.SelectFrom(Config.Name, tableName);
                randomArmorType = typeAndAmount.Type;
                bonus = typeAndAmount.Amount;

                if (randomArmorType == SpecialAbility)
                    abilityCount++;
            }
            while (bonus == 0);

            prototype.Traits = new HashSet<string>(traits);
            var isRandomSpecific = string.IsNullOrEmpty(itemName) && bonus == SpecificBonus;
            var isRandomGeneral = string.IsNullOrEmpty(itemName) && bonus != SpecificBonus;
            var isSetSpecific = !string.IsNullOrEmpty(itemName) && bonus == SpecificBonus && specificGearGenerator.CanBeSpecific(power, armorType, itemName);

            if (isRandomSpecific)
            {
                itemName = specificGearGenerator.GenerateRandomNameFrom(power, randomArmorType);
                armorType = randomArmorType;
            }
            else if (isRandomGeneral)
            {
                itemName = percentileSelector.SelectFrom(Config.Name, TableNameConstants.Percentiles.ARMORTYPETypes(randomArmorType));
                armorType = randomArmorType;
            }
            else if (isSetSpecific)
            {
                itemName = specificGearGenerator.GenerateNameFrom(power, armorType, itemName);
            }

            if (isRandomSpecific || isSetSpecific)
            {
                var specificItem = specificGearGenerator.GeneratePrototypeFrom(power, armorType, itemName, traits);
                specificItem.CloneInto(prototype);

                return prototype;
            }

            prototype.Name = itemName;
            prototype.BaseNames = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Collections.ItemGroups, itemName);
            prototype.Magic.Bonus = bonus;
            prototype.Magic.SpecialAbilities = Enumerable.Repeat(new SpecialAbility(), abilityCount);

            return prototype;
        }

        private Armor GenerateFromPrototype(Armor prototype)
        {
            if (specificGearGenerator.IsSpecific(prototype))
            {
                var specificArmor = specificGearGenerator.GenerateFrom(prototype);
                specificArmor.Quantity = 1;

                return specificArmor as Armor;
            }

            var mundaneArmorGenerator = justInTimeFactory.Build<MundaneItemGenerator>(ItemTypeConstants.Armor);
            var armor = mundaneArmorGenerator.Generate(prototype);

            armor.Magic.Bonus = prototype.Magic.Bonus;
            armor.Magic.Charges = prototype.Magic.Charges;
            armor.Magic.Curse = prototype.Magic.Curse;
            armor.Magic.Intelligence = prototype.Magic.Intelligence;
            armor.Magic.SpecialAbilities = prototype.Magic.SpecialAbilities;

            if (armor.IsMagical)
                armor.Traits.Add(TraitConstants.Masterwork);

            return armor as Armor;
        }

        public Item Generate(Item template, bool allowRandomDecoration = false)
        {
            var armorTemplate = new Armor();
            template.CloneInto(armorTemplate);

            armorTemplate.Magic.SpecialAbilities = [];
            armorTemplate.BaseNames = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Collections.ItemGroups, armorTemplate.Name);

            var armor = GenerateFromPrototype(armorTemplate);
            armor.Magic.SpecialAbilities = specialAbilitiesGenerator.GenerateFor(template.Magic.SpecialAbilities, string.Empty);

            return armor;
        }
    }
}