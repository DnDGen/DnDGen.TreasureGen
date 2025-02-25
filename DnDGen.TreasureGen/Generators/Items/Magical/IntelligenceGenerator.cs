using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.RollGen;
using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Selectors.Percentiles;
using DnDGen.TreasureGen.Selectors.Selections;
using DnDGen.TreasureGen.Tables;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.TreasureGen.Generators.Items.Magical
{
    internal class IntelligenceGenerator : IIntelligenceGenerator
    {
        private readonly Dice dice;
        private readonly ITreasurePercentileSelector percentileSelector;
        private readonly ICollectionSelector collectionsSelector;
        private readonly ICollectionDataSelector<IntelligenceDataSelection> intelligenceDataSelector;
        private readonly ICollectionTypeAndAmountSelector typeAndAmountSelector;

        private const int MaxGreaterPowers = 3;

        public IntelligenceGenerator(
            Dice dice,
            ITreasurePercentileSelector percentileSelector,
            ICollectionSelector collectionsSelector,
            ICollectionDataSelector<IntelligenceDataSelection> intelligenceDataSelector,
            ICollectionTypeAndAmountSelector typeAndAmountSelector)
        {
            this.dice = dice;
            this.percentileSelector = percentileSelector;
            this.collectionsSelector = collectionsSelector;
            this.intelligenceDataSelector = intelligenceDataSelector;
            this.typeAndAmountSelector = typeAndAmountSelector;
        }

        public bool CanBeIntelligent(IEnumerable<string> attributes, bool isMagical)
        {
            return isMagical
                && !attributes.Contains(AttributeConstants.OneTimeUse)
                && !attributes.Contains(AttributeConstants.Ammunition);
        }

        public bool IsIntelligent(string itemType, IEnumerable<string> attributes, bool isMagical)
        {
            if (!CanBeIntelligent(attributes, isMagical))
                return false;

            if (attributes.Contains(AttributeConstants.Melee))
                itemType = AttributeConstants.Melee;
            else if (attributes.Contains(AttributeConstants.Ranged))
                itemType = AttributeConstants.Ranged;

            var threshold = typeAndAmountSelector.SelectOneFrom(Config.Name, TableNameConstants.Collections.IsIntelligent, itemType);
            return percentileSelector.SelectFrom(threshold.Amount);
        }

        public Intelligence GenerateFor(Item item)
        {
            var highStatResult = percentileSelector.SelectFrom(Config.Name, TableNameConstants.Percentiles.IntelligenceStrongStats);
            var highStat = Convert.ToInt32(highStatResult);

            var intelligence = new Intelligence();
            intelligence.Ego += highStat - 10 - highStat % 2;
            intelligence.Ego += item.Magic.Bonus;

            foreach (var ability in item.Magic.SpecialAbilities)
                intelligence.Ego += ability.BonusEquivalent;

            intelligence.CharismaStat = highStat;
            intelligence.IntelligenceStat = highStat;
            intelligence.WisdomStat = highStat;

            switch (dice.Roll().d3().AsSum())
            {
                case 1: intelligence.CharismaStat = 10; break;
                case 2: intelligence.IntelligenceStat = 10; break;
                case 3: intelligence.WisdomStat = 10; break;
            }

            intelligence.Communication = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Collections.IntelligenceCommunication, highStatResult);

            if (intelligence.Communication.Contains("Speech"))
                intelligence.Languages = GenerateLanguages(intelligence.IntelligenceStat);

            intelligence.Ego += BoostEgoByCommunication(intelligence.Communication, "Read");
            intelligence.Ego += BoostEgoByCommunication(intelligence.Communication, "Read magic");
            intelligence.Ego += BoostEgoByCommunication(intelligence.Communication, "Telepathy");

            var intelligenceAttributesResult = intelligenceDataSelector.SelectOneFrom(Config.Name, TableNameConstants.Collections.IntelligenceData, highStatResult);
            intelligence.Senses = intelligenceAttributesResult.Senses;

            var lesserPowers = GeneratePowers(TableNameConstants.Percentiles.IntelligenceLesserPowers, intelligenceAttributesResult.LesserPowersCount);
            intelligence.Ego += lesserPowers.Count;
            intelligence.Powers.AddRange(lesserPowers);

            var greaterPowers = GeneratePowers(TableNameConstants.Percentiles.IntelligenceGreaterPowers, intelligenceAttributesResult.GreaterPowersCount);
            var threshold = MaxGreaterPowers + 2 - intelligenceAttributesResult.GreaterPowersCount;
            var hasSpecialPurpose = dice.Roll().d(MaxGreaterPowers + 1).AsTrueOrFalse(threshold);

            if (greaterPowers.Any() && hasSpecialPurpose)
            {
                greaterPowers.RemoveAt(greaterPowers.Count - 1);
                intelligence.SpecialPurpose = percentileSelector.SelectFrom(Config.Name, TableNameConstants.Percentiles.IntelligenceSpecialPurposes);
                intelligence.DedicatedPower = percentileSelector.SelectFrom(Config.Name, TableNameConstants.Percentiles.IntelligenceDedicatedPowers);
                intelligence.Ego += 4;
            }

            intelligence.Ego += greaterPowers.Count * 2;
            intelligence.Powers.AddRange(greaterPowers);
            intelligence.Alignment = GetAlignment(item);
            intelligence.Personality = percentileSelector.SelectFrom(Config.Name, TableNameConstants.Percentiles.PersonalityTraits);

            return intelligence;
        }

        private int BoostEgoByCommunication(IEnumerable<string> communication, string communicationType)
        {
            var containsCommunicationType = communication.Contains(communicationType);
            return Convert.ToInt32(containsCommunicationType);
        }

        private List<string> GenerateLanguages(int intelligenceStat)
        {
            var modifier = (intelligenceStat - 10) / 2;
            var languages = GetNonDuplicateList(TableNameConstants.Percentiles.Languages, modifier);
            languages.Add("Common");

            return languages;
        }

        private List<string> GeneratePowers(string tableName, int count) => GetNonDuplicateList(tableName, count);

        private List<string> GetNonDuplicateList(string tableName, int quantity)
        {
            var list = new List<string>();

            while (list.Count < quantity)
            {
                var result = percentileSelector.SelectFrom(Config.Name, tableName);

                if (result.Equals("Common", StringComparison.InvariantCultureIgnoreCase))
                    continue;

                if (list.Contains(result))
                    continue;

                list.Add(result);
            }

            return list;
        }

        private string GetAlignment(Item item)
        {
            string alignment;
            var abilityNames = item.Magic.SpecialAbilities.Select(a => a.Name);
            var specificAlignmentRequirement = GetSpecificAlignmentRequirement(item);

            do alignment = percentileSelector.SelectFrom(Config.Name, TableNameConstants.Percentiles.IntelligenceAlignments);
            while (!AlignmentIsAllowed(alignment, abilityNames, specificAlignmentRequirement));

            return alignment;
        }

        private string GetSpecificAlignmentRequirement(Item item)
        {
            var itemsWithSpecificAlignments = collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Collections.ItemAlignmentRequirements, "Items");

            if (itemsWithSpecificAlignments.Contains(item.Name))
                return collectionsSelector.SelectFrom(Config.Name, TableNameConstants.Collections.ItemAlignmentRequirements, item.Name).Single();

            var alignments = AlignmentConstants.GetAllAlignments();
            var requirements = alignments.Where(a => item.Traits.Any(t => t.Contains(a)));

            if (requirements.Any())
                //INFO: If there is more than 1 alignment requirement in the traits, then there is a problem
                return requirements.Single();

            alignments = AlignmentConstants.GetAllPartialAlignments();
            requirements = alignments.Where(a => item.Traits.Any(t => t.Contains(a)));

            if (requirements.Any())
                //INFO: If there is more than 1 partial alignment requirement in the traits, then there is a problem
                //i.e., they either conflict or should have been a full alignment requirement
                return requirements.Single();

            return string.Empty;
        }

        private bool AlignmentIsAllowed(string alignment, IEnumerable<string> abilityNames, string specificAlignmentRequirement)
        {
            if (alignment == AlignmentConstants.TrueNeutral)
                return true;

            if (abilityNames.Contains(SpecialAbilityConstants.Anarchic) && alignment.StartsWith(AlignmentConstants.Lawful))
                return false;

            if (abilityNames.Contains(SpecialAbilityConstants.Axiomatic) && alignment.StartsWith(AlignmentConstants.Chaotic))
                return false;

            if (abilityNames.Contains(SpecialAbilityConstants.Holy) && alignment.EndsWith(AlignmentConstants.Evil))
                return false;

            if (abilityNames.Contains(SpecialAbilityConstants.Unholy) && alignment.EndsWith(AlignmentConstants.Good))
                return false;

            return alignment == specificAlignmentRequirement || PartialAlignmentRequirementMet(alignment, specificAlignmentRequirement);
        }

        private bool PartialAlignmentRequirementMet(string alignment, string specificAlignmentRequirement)
        {
            if (specificAlignmentRequirement == AlignmentConstants.Neutral)
                return alignment.EndsWith(specificAlignmentRequirement);

            return alignment.StartsWith(specificAlignmentRequirement) || alignment.EndsWith(specificAlignmentRequirement);
        }
    }
}