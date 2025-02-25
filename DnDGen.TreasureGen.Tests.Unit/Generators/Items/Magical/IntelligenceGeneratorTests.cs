using DnDGen.Infrastructure.Models;
using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.RollGen;
using DnDGen.TreasureGen.Generators.Items.Magical;
using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Selectors.Percentiles;
using DnDGen.TreasureGen.Selectors.Selections;
using DnDGen.TreasureGen.Tables;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;

namespace DnDGen.TreasureGen.Tests.Unit.Generators.Items.Magical
{
    [TestFixture]
    public class IntelligenceGeneratorTests
    {
        private IIntelligenceGenerator intelligenceGenerator;
        private Mock<Dice> mockDice;
        private Mock<ITreasurePercentileSelector> mockPercentileSelector;
        private Mock<ICollectionSelector> mockCollectionSelector;
        private Mock<ICollectionTypeAndAmountSelector> mockCollectionTypeAndAmountSelector;
        private Mock<ICollectionDataSelector<IntelligenceDataSelection>> mockIntelligenceDataSelector;
        private List<string> attributes;
        private IntelligenceDataSelection intelligenceSelection;
        private Item item;
        private string itemType;

        [SetUp]
        public void Setup()
        {
            mockDice = new Mock<Dice>();
            mockPercentileSelector = new Mock<ITreasurePercentileSelector>();
            mockCollectionSelector = new Mock<ICollectionSelector>();
            mockCollectionTypeAndAmountSelector = new Mock<ICollectionTypeAndAmountSelector>();
            mockIntelligenceDataSelector = new Mock<ICollectionDataSelector<IntelligenceDataSelection>>();
            intelligenceSelection = new IntelligenceDataSelection();
            attributes = [];
            item = new Item();
            itemType = "item type";

            var fillerValues = new[] { "0" };
            mockCollectionSelector.Setup(s => s.SelectFrom(Config.Name, It.IsAny<string>(), It.IsAny<string>())).Returns(fillerValues);
            mockDice.Setup(d => d.Roll(1).d(4).AsSum<int>()).Returns(4);
            mockDice.Setup(d => d.Roll(1).d(3).AsSum<int>()).Returns(3);
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.IntelligenceStrongStats)).Returns("10");
            mockIntelligenceDataSelector.Setup(s => s.SelectOneFrom(Config.Name, TableNameConstants.Collections.IntelligenceData, It.IsAny<string>())).Returns(intelligenceSelection);
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.IntelligenceAlignments)).Returns(string.Empty);

            intelligenceGenerator = new IntelligenceGenerator(mockDice.Object, mockPercentileSelector.Object, mockCollectionSelector.Object, mockIntelligenceDataSelector.Object, mockCollectionTypeAndAmountSelector.Object);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void DetermineIntelligentFromSelector(bool expected)
        {
            mockCollectionTypeAndAmountSelector
                .Setup(s => s.SelectOneFrom(Config.Name, TableNameConstants.Collections.IsIntelligent, itemType))
                .Returns(new TypeAndAmountDataSelection { Type = itemType, AmountAsDouble = 92 });
            mockPercentileSelector
                .Setup(s => s.SelectFrom(92))
                .Returns(expected);

            var isIntelligent = intelligenceGenerator.IsIntelligent(itemType, attributes, true);
            Assert.That(isIntelligent, Is.EqualTo(expected));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void DetermineMeleeIntelligence(bool expected)
        {
            attributes.Add(AttributeConstants.Melee);
            mockCollectionTypeAndAmountSelector
                .Setup(s => s.SelectOneFrom(Config.Name, TableNameConstants.Collections.IsIntelligent, AttributeConstants.Melee))
                .Returns(new TypeAndAmountDataSelection { Type = AttributeConstants.Melee, AmountAsDouble = 92 });
            mockPercentileSelector
                .Setup(s => s.SelectFrom(92))
                .Returns(expected);

            var isIntelligent = intelligenceGenerator.IsIntelligent(itemType, attributes, true);
            Assert.That(isIntelligent, Is.EqualTo(expected));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void DetermineRangedIntelligence(bool expected)
        {
            attributes.Add(AttributeConstants.Ranged);
            mockCollectionTypeAndAmountSelector
                .Setup(s => s.SelectOneFrom(Config.Name, TableNameConstants.Collections.IsIntelligent, AttributeConstants.Ranged))
                .Returns(new TypeAndAmountDataSelection { Type = AttributeConstants.Ranged, AmountAsDouble = 92 });
            mockPercentileSelector
                .Setup(s => s.SelectFrom(92))
                .Returns(expected);

            var isIntelligent = intelligenceGenerator.IsIntelligent(itemType, attributes, true);
            Assert.That(isIntelligent, Is.EqualTo(expected));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void DetermineRangedAndMeleeIntelligence_AsMelee(bool expected)
        {
            attributes.Add(AttributeConstants.Melee);
            attributes.Add(AttributeConstants.Ranged);
            mockCollectionTypeAndAmountSelector
                .Setup(s => s.SelectOneFrom(Config.Name, TableNameConstants.Collections.IsIntelligent, AttributeConstants.Melee))
                .Returns(new TypeAndAmountDataSelection { Type = AttributeConstants.Melee, AmountAsDouble = 92 });
            mockPercentileSelector
                .Setup(s => s.SelectFrom(92))
                .Returns(expected);

            var isIntelligent = intelligenceGenerator.IsIntelligent(itemType, attributes, true);
            Assert.That(isIntelligent, Is.EqualTo(expected));
        }

        [Test]
        public void AmmunitionIsNotIntelligent()
        {
            attributes.Add(AttributeConstants.Ammunition);
            mockCollectionTypeAndAmountSelector
                .Setup(s => s.SelectOneFrom(Config.Name, TableNameConstants.Collections.IsIntelligent, itemType))
                .Returns(new TypeAndAmountDataSelection { Type = itemType, AmountAsDouble = 92 });
            mockPercentileSelector
                .Setup(s => s.SelectFrom(92))
                .Returns(true);

            var isIntelligent = intelligenceGenerator.IsIntelligent(itemType, attributes, true);
            Assert.That(isIntelligent, Is.False);
        }

        [Test]
        public void OneTimeUseItemsAreNotIntelligent()
        {
            attributes.Add(AttributeConstants.OneTimeUse);
            mockCollectionTypeAndAmountSelector
                .Setup(s => s.SelectOneFrom(Config.Name, TableNameConstants.Collections.IsIntelligent, itemType))
                .Returns(new TypeAndAmountDataSelection { Type = itemType, AmountAsDouble = 92 });
            mockPercentileSelector
                .Setup(s => s.SelectFrom(92))
                .Returns(true);

            var isIntelligent = intelligenceGenerator.IsIntelligent(itemType, attributes, true);
            Assert.That(isIntelligent, Is.False);
        }

        [Test]
        public void NonMagicalItemsAreNotIntelligent()
        {
            mockCollectionTypeAndAmountSelector
                .Setup(s => s.SelectOneFrom(Config.Name, TableNameConstants.Collections.IsIntelligent, itemType))
                .Returns(new TypeAndAmountDataSelection { Type = itemType, AmountAsDouble = 92 });
            mockPercentileSelector
                .Setup(s => s.SelectFrom(92))
                .Returns(true);

            var isIntelligent = intelligenceGenerator.IsIntelligent(itemType, attributes, false);
            Assert.That(isIntelligent, Is.False);
        }

        [Test]
        public void AmmunitionCannotBeIntelligent()
        {
            attributes.Add(AttributeConstants.Ammunition);

            var canBeIntelligent = intelligenceGenerator.CanBeIntelligent(attributes, true);
            Assert.That(canBeIntelligent, Is.False);
        }

        [Test]
        public void OneTimeUseItemsCannotBeIntelligent()
        {
            attributes.Add(AttributeConstants.OneTimeUse);

            var canBeIntelligent = intelligenceGenerator.CanBeIntelligent(attributes, true);
            Assert.That(canBeIntelligent, Is.False);
        }

        [Test]
        public void NonMagicalItemsCannotBeIntelligent()
        {
            var canBeIntelligent = intelligenceGenerator.CanBeIntelligent(attributes, false);
            Assert.That(canBeIntelligent, Is.False);
        }

        [Test]
        public void ReturnIntelligence()
        {
            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence, Is.Not.Null);
        }

        [Test]
        public void Roll1MeansCharismaIsWeakStat()
        {
            mockDice.Setup(d => d.Roll(1).d(3).AsSum<int>()).Returns(1);
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.IntelligenceStrongStats)).Returns("42");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.CharismaStat, Is.EqualTo(10));
            Assert.That(intelligence.IntelligenceStat, Is.EqualTo(42));
            Assert.That(intelligence.WisdomStat, Is.EqualTo(42));
        }

        [Test]
        public void Roll2MeansIntelligenceIsWeakStat()
        {
            mockDice.Setup(d => d.Roll(1).d(3).AsSum<int>()).Returns(2);
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.IntelligenceStrongStats)).Returns("42");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.CharismaStat, Is.EqualTo(42));
            Assert.That(intelligence.IntelligenceStat, Is.EqualTo(10));
            Assert.That(intelligence.WisdomStat, Is.EqualTo(42));
        }

        [Test]
        public void Roll3MeansWisdomIsWeakStat()
        {
            mockDice.Setup(d => d.Roll(1).d(3).AsSum<int>()).Returns(3);
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.IntelligenceStrongStats)).Returns("42");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.CharismaStat, Is.EqualTo(42));
            Assert.That(intelligence.IntelligenceStat, Is.EqualTo(42));
            Assert.That(intelligence.WisdomStat, Is.EqualTo(10));
        }

        [Test]
        public void GetCommunicationFromAttributesSelector()
        {
            var attributes = new[] { "talky" };
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.IntelligenceStrongStats)).Returns("9266");
            mockCollectionSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.IntelligenceCommunication, "9266")).Returns(attributes);

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Communication, Is.EqualTo(attributes));
        }

        [Test]
        public void GetLanguagesIfSpeech()
        {
            var attributes = new[] { "Speech" };
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.IntelligenceStrongStats)).Returns("10");
            mockCollectionSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.IntelligenceCommunication, "10")).Returns(attributes);

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Languages, Contains.Item("Common"));
        }

        [Test]
        public void GetNumberOfBonusLanguagesEqualToIntelligenceModifier()
        {
            var attributes = new[] { "Speech" };
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.IntelligenceStrongStats)).Returns("14");
            mockCollectionSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.IntelligenceCommunication, "14")).Returns(attributes);
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.Languages)).Returns("english").Returns("german");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Languages, Has.Count.EqualTo(3)
                .And.Contains("Common")
                .And.Contains("english")
                .And.Contains("german"));
        }

        [Test]
        public void DoNotHaveDuplicateLanguages()
        {
            var attributes = new[] { "Speech" };
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.IntelligenceStrongStats)).Returns("14");
            mockCollectionSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.IntelligenceCommunication, "14")).Returns(attributes);
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.Languages))
                .Returns("english").Returns("english").Returns("german");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Languages, Has.Count.EqualTo(3)
                .And.Contains("Common")
                .And.Contains("english")
                .And.Contains("german"));
        }

        [Test]
        public void GetSensesFromAttributesSelector()
        {
            intelligenceSelection.Senses = "sensy";
            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Senses, Is.EqualTo(intelligenceSelection.Senses));
        }

        [Test]
        public void GetLesserPowersFromAttributesSelector()
        {
            intelligenceSelection.LesserPowersCount = 2;
            var tableName = TableNameConstants.Percentiles.IntelligenceLesserPowers;
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(Config.Name, tableName)).Returns("power 1").Returns("power 2");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Powers, Has.Count.EqualTo(2));
        }

        [Test]
        public void CannotHaveDuplicateLesserPowers()
        {
            intelligenceSelection.LesserPowersCount = 2;
            var tableName = TableNameConstants.Percentiles.IntelligenceLesserPowers;
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(Config.Name, tableName)).Returns("power 1").Returns("power 1").Returns("power 2");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Powers, Has.Count.EqualTo(2)
                .And.Contains("power 1")
                .And.Contains("power 2"));
        }

        [Test]
        public void GetGreaterPowersFromAttributesSelector()
        {
            intelligenceSelection.GreaterPowersCount = 2;
            var tableName = TableNameConstants.Percentiles.IntelligenceGreaterPowers;
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(Config.Name, tableName)).Returns("power 1").Returns("power 2");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Powers, Has.Count.EqualTo(2));
        }

        [Test]
        public void CannotHaveDuplicateGreaterPowers()
        {
            intelligenceSelection.GreaterPowersCount = 2;
            var tableName = TableNameConstants.Percentiles.IntelligenceGreaterPowers;
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(Config.Name, tableName)).Returns("power 1").Returns("power 1").Returns("power 2");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Powers, Has.Count.EqualTo(2)
                .And.Contains("power 1")
                .And.Contains("power 2"));
        }

        [Test]
        public void ZeroGreaterPowerMeans0PercentChanceForSpecialPurpose()
        {
            intelligenceSelection.LesserPowersCount = 2;
            intelligenceSelection.GreaterPowersCount = 0;

            var lesserTableName = TableNameConstants.Percentiles.IntelligenceLesserPowers;
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(Config.Name, lesserTableName)).Returns("power 1").Returns("power 2");

            var greaterTableName = TableNameConstants.Percentiles.IntelligenceGreaterPowers;
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, greaterTableName)).Returns("greater power");

            mockDice.Setup(d => d.Roll(1).d(4).AsTrueOrFalse(5)).Returns(true);

            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.IntelligenceSpecialPurposes)).Returns("purpose");
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.IntelligenceDedicatedPowers)).Returns("dedicated power");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Powers, Has.Count.EqualTo(2)
                .And.Contains("power 1")
                .And.Contains("power 2"));
            Assert.That(intelligence.SpecialPurpose, Is.Empty);
            Assert.That(intelligence.DedicatedPower, Is.Empty);
        }

        [Test]
        public void OneGreaterPowerMeans25PercentChanceForSpecialPurpose()
        {
            intelligenceSelection.LesserPowersCount = 2;
            intelligenceSelection.GreaterPowersCount = 1;

            var lesserTableName = TableNameConstants.Percentiles.IntelligenceLesserPowers;
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(Config.Name, lesserTableName)).Returns("power 1").Returns("power 2");

            var greaterTableName = TableNameConstants.Percentiles.IntelligenceGreaterPowers;
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, greaterTableName)).Returns("greater power");

            mockDice.Setup(d => d.Roll(1).d(4).AsTrueOrFalse(4)).Returns(true);

            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.IntelligenceSpecialPurposes)).Returns("purpose");
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.IntelligenceDedicatedPowers)).Returns("dedicated power");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Powers, Has.Count.EqualTo(2)
                .And.Contains("power 1")
                .And.Contains("power 2"));
            Assert.That(intelligence.SpecialPurpose, Is.EqualTo("purpose"));
            Assert.That(intelligence.DedicatedPower, Is.EqualTo("dedicated power"));
        }

        [Test]
        public void OneGreaterPowerMeans75PercentChanceForGreaterPower()
        {
            intelligenceSelection.LesserPowersCount = 2;
            intelligenceSelection.GreaterPowersCount = 1;

            var lesserTableName = TableNameConstants.Percentiles.IntelligenceLesserPowers;
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(Config.Name, lesserTableName)).Returns("power 1").Returns("power 2");

            var greaterTableName = TableNameConstants.Percentiles.IntelligenceGreaterPowers;
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, greaterTableName)).Returns("greater power");

            mockDice.Setup(d => d.Roll(1).d(4).AsTrueOrFalse(4)).Returns(false);

            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.IntelligenceSpecialPurposes)).Returns("purpose");
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.IntelligenceDedicatedPowers)).Returns("dedicated power");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Powers, Has.Count.EqualTo(3)
                .And.Contains("power 1")
                .And.Contains("power 2")
                .And.Contains("greater power"));
            Assert.That(intelligence.SpecialPurpose, Is.Empty);
            Assert.That(intelligence.DedicatedPower, Is.Empty);
        }

        [Test]
        public void TwoGreaterPowerMeans50PercentChanceForSpecialPurpose()
        {
            intelligenceSelection.LesserPowersCount = 2;
            intelligenceSelection.GreaterPowersCount = 2;

            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.IntelligenceSpecialPurposes)).Returns("purpose");
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.IntelligenceDedicatedPowers)).Returns("dedicated power");

            var lesserTableName = TableNameConstants.Percentiles.IntelligenceLesserPowers;
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(Config.Name, lesserTableName)).Returns("power 1").Returns("power 2");

            var greaterTableName = TableNameConstants.Percentiles.IntelligenceGreaterPowers;
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(Config.Name, greaterTableName)).Returns("greater power 1").Returns("greater power 2");

            mockDice.Setup(d => d.Roll(1).d(4).AsTrueOrFalse(3)).Returns(true);

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Powers, Has.Count.EqualTo(3)
                .And.Contains("power 1")
                .And.Contains("power 2")
                .And.Contains("greater power 1"));
            Assert.That(intelligence.SpecialPurpose, Is.EqualTo("purpose"));
            Assert.That(intelligence.DedicatedPower, Is.EqualTo("dedicated power"));
        }

        [Test]
        public void TwoGreaterPowerMeans50PercentChanceForGreaterPower()
        {
            intelligenceSelection.LesserPowersCount = 2;
            intelligenceSelection.GreaterPowersCount = 2;

            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.IntelligenceSpecialPurposes)).Returns("purpose");
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.IntelligenceDedicatedPowers)).Returns("dedicated power");

            var lesserTableName = TableNameConstants.Percentiles.IntelligenceLesserPowers;
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(Config.Name, lesserTableName)).Returns("power 1").Returns("power 2");

            var greaterTableName = TableNameConstants.Percentiles.IntelligenceGreaterPowers;
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(Config.Name, greaterTableName)).Returns("greater power 1").Returns("greater power 2");

            mockDice.Setup(d => d.Roll(1).d(4).AsTrueOrFalse(3)).Returns(false);

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Powers, Has.Count.EqualTo(4)
                .And.Contains("power 1")
                .And.Contains("power 2")
                .And.Contains("greater power 1")
                .And.Contains("greater power 2"));
            Assert.That(intelligence.SpecialPurpose, Is.Empty);
            Assert.That(intelligence.DedicatedPower, Is.Empty);
        }

        [Test]
        public void ThreeGreaterPowerMeans75PercentChanceForSpecialPurpose()
        {
            intelligenceSelection.LesserPowersCount = 2;
            intelligenceSelection.GreaterPowersCount = 3;

            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.IntelligenceSpecialPurposes)).Returns("purpose");
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.IntelligenceDedicatedPowers)).Returns("dedicated power");

            var lesserTableName = TableNameConstants.Percentiles.IntelligenceLesserPowers;
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(Config.Name, lesserTableName)).Returns("power 1").Returns("power 2");

            var greaterTableName = TableNameConstants.Percentiles.IntelligenceGreaterPowers;
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(Config.Name, greaterTableName)).Returns("greater power 1").Returns("greater power 2").Returns("greater power 3");

            mockDice.Setup(d => d.Roll(1).d(4).AsTrueOrFalse(2)).Returns(true);

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Powers, Has.Count.EqualTo(4)
                .And.Contains("power 1")
                .And.Contains("power 2")
                .And.Contains("greater power 1")
                .And.Contains("greater power 2"));
            Assert.That(intelligence.SpecialPurpose, Is.EqualTo("purpose"));
            Assert.That(intelligence.DedicatedPower, Is.EqualTo("dedicated power"));
        }

        [Test]
        public void ThreeGreaterPowerMeans25PercentChanceForGreaterPower()
        {
            intelligenceSelection.LesserPowersCount = 2;
            intelligenceSelection.GreaterPowersCount = 3;

            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.IntelligenceSpecialPurposes)).Returns("purpose");
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.IntelligenceDedicatedPowers)).Returns("dedicated power");

            var lesserTableName = TableNameConstants.Percentiles.IntelligenceLesserPowers;
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(Config.Name, lesserTableName)).Returns("power 1").Returns("power 2");

            var greaterTableName = TableNameConstants.Percentiles.IntelligenceGreaterPowers;
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(Config.Name, greaterTableName)).Returns("greater power 1").Returns("greater power 2").Returns("greater power 3");

            mockDice.Setup(d => d.Roll(1).d(4).AsTrueOrFalse(2)).Returns(false);

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Powers, Has.Count.EqualTo(5)
                .And.Contains("power 1")
                .And.Contains("power 2")
                .And.Contains("greater power 1")
                .And.Contains("greater power 2")
                .And.Contains("greater power 3"));
            Assert.That(intelligence.SpecialPurpose, Is.Empty);
            Assert.That(intelligence.DedicatedPower, Is.Empty);
        }

        [Test]
        public void GetAlignmentFromPercentileSelector()
        {
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.IntelligenceAlignments)).Returns("alignment");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Alignment, Is.EqualTo("alignment"));
        }

        [TestCase(AlignmentConstants.Chaotic)]
        public void NonAxiomaticAlignments(string alignment)
        {
            var ability = new SpecialAbility { Name = SpecialAbilityConstants.Axiomatic };
            item.Magic.SpecialAbilities = new[] { ability };
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.IntelligenceAlignments))
                .Returns(alignment + " alignment").Returns("alignment");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Alignment, Is.EqualTo("alignment"));
        }

        [TestCase(AlignmentConstants.Neutral)]
        [TestCase(AlignmentConstants.Lawful)]
        [TestCase("True")]
        public void AxiomaticAlignments(string alignment)
        {
            var ability = new SpecialAbility { Name = SpecialAbilityConstants.Axiomatic };
            item.Magic.SpecialAbilities = new[] { ability };
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.IntelligenceAlignments))
                .Returns(alignment + " alignment").Returns("alignment");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Alignment, Is.EqualTo(alignment + " alignment"));
        }

        [TestCase(AlignmentConstants.Lawful)]
        public void NonAnarchicAlignments(string alignment)
        {
            var ability = new SpecialAbility { Name = SpecialAbilityConstants.Anarchic };
            item.Magic.SpecialAbilities = new[] { ability };
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.IntelligenceAlignments))
                .Returns(alignment + " alignment").Returns("alignment");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Alignment, Is.EqualTo("alignment"));
        }

        [TestCase(AlignmentConstants.Neutral)]
        [TestCase(AlignmentConstants.Chaotic)]
        [TestCase("True")]
        public void AnarchicAlignments(string alignment)
        {
            var ability = new SpecialAbility { Name = SpecialAbilityConstants.Anarchic };
            item.Magic.SpecialAbilities = new[] { ability };
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.IntelligenceAlignments))
                .Returns(alignment + " alignment").Returns("alignment");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Alignment, Is.EqualTo(alignment + " alignment"));
        }

        [TestCase(AlignmentConstants.Evil)]
        public void NonHolyAlignments(string alignment)
        {
            var ability = new SpecialAbility { Name = SpecialAbilityConstants.Holy };
            item.Magic.SpecialAbilities = new[] { ability };
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.IntelligenceAlignments))
                .Returns("alignment " + alignment).Returns("alignment");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Alignment, Is.EqualTo("alignment"));
        }

        [TestCase(AlignmentConstants.Neutral)]
        [TestCase(AlignmentConstants.Good)]
        public void HolyAlignments(string alignment)
        {
            var ability = new SpecialAbility { Name = SpecialAbilityConstants.Holy };
            item.Magic.SpecialAbilities = new[] { ability };
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.IntelligenceAlignments))
                .Returns("alignment " + alignment).Returns("alignment");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Alignment, Is.EqualTo("alignment " + alignment));
        }

        [TestCase(AlignmentConstants.Good)]
        public void NonUnholyAlignments(string alignment)
        {
            var ability = new SpecialAbility { Name = SpecialAbilityConstants.Unholy };
            item.Magic.SpecialAbilities = new[] { ability };
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.IntelligenceAlignments))
                .Returns("alignment " + alignment).Returns("alignment");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Alignment, Is.EqualTo("alignment"));
        }

        [TestCase(AlignmentConstants.Neutral)]
        [TestCase(AlignmentConstants.Evil)]
        public void UnholyAlignments(string alignment)
        {
            var ability = new SpecialAbility { Name = SpecialAbilityConstants.Unholy };
            item.Magic.SpecialAbilities = new[] { ability };
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.IntelligenceAlignments))
                .Returns("alignment " + alignment).Returns("alignment");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Alignment, Is.EqualTo("alignment " + alignment));
        }

        [Test]
        public void ItemWithSpecificAlignmentHasMatchingAlignment()
        {
            item.Name = "item name";
            mockCollectionSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.ItemAlignmentRequirements, "Items"))
                .Returns(new[] { item.Name, "other item name" });
            var alignment = "specific alignment";
            mockCollectionSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.ItemAlignmentRequirements, item.Name)).Returns(new[] { alignment });
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.IntelligenceAlignments))
                .Returns("alignment").Returns(alignment);

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Alignment, Is.EqualTo(alignment));
        }

        [Test]
        public void ItemWithNoSpecificAlignmentHasAnyAlignment()
        {
            item.Name = "item name";
            mockCollectionSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.ItemAlignmentRequirements, "Items")).Returns(new[] { "other item name" });
            mockCollectionSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.ItemAlignmentRequirements, item.Name)).Returns(new[] { "specific" });
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.IntelligenceAlignments))
                .Returns("alignment").Returns("specific alignment");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Alignment, Is.EqualTo("alignment"));
        }

        [Test]
        public void ItemWithSpecificAlignmentBeginningHasMatchingAlignment()
        {
            item.Name = "item name";
            mockCollectionSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.ItemAlignmentRequirements, "Items"))
                .Returns(new[] { item.Name, "other item name" });
            var alignment = "specific";
            mockCollectionSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.ItemAlignmentRequirements, item.Name)).Returns(new[] { alignment });
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.IntelligenceAlignments))
                .Returns("alignment").Returns("specific alignment");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Alignment, Is.EqualTo("specific alignment"));
        }

        [Test]
        public void ItemWithSpecificAlignmentEndingHasMatchingAlignment()
        {
            item.Name = "item name";
            mockCollectionSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.ItemAlignmentRequirements, "Items"))
                .Returns(new[] { item.Name, "other item name" });
            var alignment = "ending";
            mockCollectionSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.ItemAlignmentRequirements, item.Name)).Returns(new[] { alignment });
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.IntelligenceAlignments))
                .Returns("alignment").Returns("specific alignment ending");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Alignment, Is.EqualTo("specific alignment ending"));
        }

        [Test]
        public void ItemWithPartOfAlignmentAsTraitUsesThatAsAlignmentRequirement()
        {
            item.Traits.Add(AlignmentConstants.Good);
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.IntelligenceAlignments))
                .Returns("alignment Evil").Returns("alignment Good");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Alignment, Is.EqualTo("alignment Good"));
        }

        [Test]
        public void ItemWithPartOfAlignmentAsPartOfTraitUsesThatAsAlignmentRequirement()
        {
            var trait = string.Format("trait ({0})", AlignmentConstants.Good);
            item.Traits.Add(trait);
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.IntelligenceAlignments))
                .Returns("alignment Evil").Returns("alignment Good");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Alignment, Is.EqualTo("alignment Good"));
        }

        [Test]
        public void ItemWithAlignmentAsTraitUsesThatAsAlignmentRequirement()
        {
            item.Traits.Add(AlignmentConstants.ChaoticNeutral);
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.IntelligenceAlignments))
                .Returns("alignment").Returns(AlignmentConstants.ChaoticNeutral);

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Alignment, Is.EqualTo(AlignmentConstants.ChaoticNeutral));
        }

        [Test]
        public void ItemWithAlignmentAsPartOfTraitUsesThatAsAlignmentRequirement()
        {
            var trait = string.Format("trait ({0})", AlignmentConstants.ChaoticNeutral);
            item.Traits.Add(trait);
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.IntelligenceAlignments))
                .Returns("alignment").Returns(AlignmentConstants.ChaoticNeutral);

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Alignment, Is.EqualTo(AlignmentConstants.ChaoticNeutral));
        }

        [Test]
        public void OnlyAlignmentsEndingInNeutralMatchNeutralRequirement()
        {
            item.Traits.Add(AlignmentConstants.Neutral);
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.IntelligenceAlignments))
                .Returns(AlignmentConstants.NeutralEvil).Returns(AlignmentConstants.ChaoticNeutral);

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Alignment, Is.EqualTo(AlignmentConstants.ChaoticNeutral));
        }

        [Test]
        public void TrueNeutralSatisfiesAllRequirements()
        {
            item.Magic.SpecialAbilities = new[]
            {
                new SpecialAbility { Name = SpecialAbilityConstants.Unholy },
                new SpecialAbility { Name = SpecialAbilityConstants.Holy },
                new SpecialAbility { Name = SpecialAbilityConstants.Axiomatic },
                new SpecialAbility { Name = SpecialAbilityConstants.Anarchic }
            };

            item.Traits.Add(AlignmentConstants.ChaoticEvil);
            item.Traits.Add($"trait ({AlignmentConstants.Good})");

            mockCollectionSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.ItemAlignmentRequirements, item.Name)).Returns(new[] { "specific" });
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.IntelligenceAlignments))
                .Returns(AlignmentConstants.ChaoticEvil)
                .Returns(AlignmentConstants.ChaoticGood)
                .Returns(AlignmentConstants.ChaoticNeutral)
                .Returns(AlignmentConstants.LawfulEvil)
                .Returns(AlignmentConstants.LawfulGood)
                .Returns(AlignmentConstants.LawfulNeutral)
                .Returns(AlignmentConstants.NeutralEvil)
                .Returns(AlignmentConstants.NeutralGood)
                .Returns(AlignmentConstants.TrueNeutral);

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Alignment, Is.EqualTo(AlignmentConstants.TrueNeutral));
        }

        [Test]
        public void EgoIncludesMagicBonus()
        {
            item.Magic.Bonus = 9266;

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Ego, Is.EqualTo(9266));
        }

        [Test]
        public void EgoIncludesSpecialAbilityBonuses()
        {
            item.Magic.SpecialAbilities = new[]
            {
                new SpecialAbility { BonusEquivalent = 9200 },
                new SpecialAbility { BonusEquivalent = 66 }
            };

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Ego, Is.EqualTo(9266));
        }

        [Test]
        public void EgoIncludesLesserPowers()
        {
            intelligenceSelection.LesserPowersCount = 2;
            var tableName = TableNameConstants.Percentiles.IntelligenceLesserPowers;
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(Config.Name, tableName)).Returns("power 1").Returns("power 2");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Ego, Is.EqualTo(2));
        }

        [Test]
        public void EgoIncludesGreaterPowers()
        {
            intelligenceSelection.GreaterPowersCount = 2;
            var tableName = TableNameConstants.Percentiles.IntelligenceGreaterPowers;
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(Config.Name, tableName)).Returns("greater power 1").Returns("greater power 2");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Ego, Is.EqualTo(4));
        }

        [Test]
        public void EgoIncludesDedicatedPower()
        {
            intelligenceSelection.GreaterPowersCount = 1;
            var tableName = TableNameConstants.Percentiles.IntelligenceGreaterPowers;
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, tableName)).Returns("greater power");
            mockDice.Setup(d => d.Roll(1).d(4).AsTrueOrFalse(4)).Returns(true);

            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.IntelligenceSpecialPurposes)).Returns("purpose");
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.IntelligenceDedicatedPowers)).Returns("dedicated power");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Ego, Is.EqualTo(4));
        }

        [Test]
        public void EgoIncludesTelepathy()
        {
            var attributes = new[] { "Telepathy" };
            mockCollectionSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.IntelligenceCommunication, "10")).Returns(attributes);

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Ego, Is.EqualTo(1));
        }

        [Test]
        public void EgoIncludesReading()
        {
            var attributes = new[] { "Read" };
            mockCollectionSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.IntelligenceCommunication, "10")).Returns(attributes);

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Ego, Is.EqualTo(1));
        }

        [Test]
        public void EgoIncludesReadMagic()
        {
            var attributes = new[] { "Read magic" };
            mockCollectionSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.IntelligenceCommunication, "10")).Returns(attributes);

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Ego, Is.EqualTo(1));
        }

        [TestCase(10, 0)]
        [TestCase(11, 0)]
        [TestCase(12, 2)]
        [TestCase(13, 2)]
        [TestCase(14, 4)]
        [TestCase(15, 4)]
        [TestCase(16, 6)]
        [TestCase(17, 6)]
        [TestCase(18, 8)]
        [TestCase(19, 8)]
        [TestCase(20, 10)]
        public void EgoIncludesStatBonuses(int strongStat, int egoBonus)
        {
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.IntelligenceStrongStats)).Returns(strongStat.ToString());
            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Ego, Is.EqualTo(egoBonus));
        }

        [Test]
        public void EgoSumsAllFactors()
        {
            var communication = new[] { "Read", "Read magic", "Telepathy" };
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.IntelligenceStrongStats)).Returns("19");
            mockCollectionSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.IntelligenceCommunication, "19")).Returns(communication);
            intelligenceSelection.LesserPowersCount = 2;
            intelligenceSelection.GreaterPowersCount = 2;

            var tableName = TableNameConstants.Percentiles.IntelligenceGreaterPowers;
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(Config.Name, tableName)).Returns("greater power 1").Returns("greater power 2");
            mockDice.Setup(d => d.Roll(1).d(4).AsTrueOrFalse(3)).Returns(true);

            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.IntelligenceSpecialPurposes)).Returns("purpose");
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.IntelligenceDedicatedPowers)).Returns("dedicated power");

            tableName = TableNameConstants.Percentiles.IntelligenceLesserPowers;
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(Config.Name, tableName)).Returns("power 1").Returns("power 2");

            var ability = new SpecialAbility();
            ability.BonusEquivalent = 92;
            item.Magic.SpecialAbilities = new[] { ability };
            item.Magic.Bonus = 66;

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Ego, Is.EqualTo(177));
        }

        [Test]
        public void IntelligenceHasPersonality()
        {
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.PersonalityTraits)).Returns("personality");
            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Personality, Is.EqualTo("personality"));
        }
    }
}