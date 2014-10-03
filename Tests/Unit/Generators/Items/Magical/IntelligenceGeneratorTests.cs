﻿using System;
using System.Collections.Generic;
using D20Dice;
using EquipmentGen.Common.Items;
using EquipmentGen.Generators.Interfaces.Items.Magical;
using EquipmentGen.Generators.Items.Magical;
using EquipmentGen.Selectors.Interfaces;
using EquipmentGen.Selectors.Interfaces.Objects;
using EquipmentGen.Tables.Interfaces;
using Moq;
using NUnit.Framework;

namespace EquipmentGen.Tests.Unit.Generators.Items.Magical
{
    [TestFixture]
    public class IntelligenceGeneratorTests
    {
        private IIntelligenceGenerator intelligenceGenerator;
        private Mock<IDice> mockDice;
        private Mock<IPercentileSelector> mockPercentileSelector;
        private Mock<IAttributesSelector> mockAttributesSelector;
        private Mock<IIntelligenceAttributesSelector> mockIntelligenceAttributesSelector;
        private Mock<IBooleanPercentileSelector> mockBooleanPercentileSelector;
        private List<String> attributes;
        private IntelligenceAttributesResult intelligenceAttributesResult;
        private Item item;
        private String itemType;

        [SetUp]
        public void Setup()
        {
            mockDice = new Mock<IDice>();
            mockPercentileSelector = new Mock<IPercentileSelector>();
            mockAttributesSelector = new Mock<IAttributesSelector>();
            mockIntelligenceAttributesSelector = new Mock<IIntelligenceAttributesSelector>();
            mockBooleanPercentileSelector = new Mock<IBooleanPercentileSelector>();
            intelligenceAttributesResult = new IntelligenceAttributesResult();
            attributes = new List<String>();
            item = new Item();
            itemType = "item type";

            var fillerValues = new[] { "0" };
            mockAttributesSelector.Setup(s => s.SelectFrom(It.IsAny<String>(), It.IsAny<String>())).Returns(fillerValues);
            mockDice.Setup(d => d.Roll(1).d4()).Returns(4);
            mockPercentileSelector.Setup(s => s.SelectFrom(TableNameConstants.Percentiles.Set.IntelligenceStrongStats)).Returns("10");
            mockIntelligenceAttributesSelector.Setup(s => s.SelectFrom(TableNameConstants.Attributes.Set.IntelligenceAttributes, It.IsAny<String>())).Returns(intelligenceAttributesResult);
            mockPercentileSelector.Setup(s => s.SelectFrom(TableNameConstants.Percentiles.Set.IntelligenceAlignments)).Returns(String.Empty);

            intelligenceGenerator = new IntelligenceGenerator(mockDice.Object, mockPercentileSelector.Object,
                mockAttributesSelector.Object, mockIntelligenceAttributesSelector.Object, mockBooleanPercentileSelector.Object);
        }

        [Test]
        public void DetermineIntelligentFromBooleanSelector()
        {
            var tableName = String.Format(TableNameConstants.Percentiles.Formattable.IsITEMTYPEIntelligent, itemType);
            mockBooleanPercentileSelector.Setup(s => s.SelectFrom(tableName)).Returns(true);

            var isIntelligent = intelligenceGenerator.IsIntelligent(itemType, attributes, true);
            Assert.That(isIntelligent, Is.True);
        }

        [Test]
        public void BagOfDevouringIsNotIntelligent()
        {
            Assert.Fail();
        }

        [Test]
        public void DetermineNotIntelligentFromBooleanSelector()
        {
            var tableName = String.Format(TableNameConstants.Percentiles.Formattable.IsITEMTYPEIntelligent, itemType);
            mockBooleanPercentileSelector.Setup(s => s.SelectFrom(tableName)).Returns(false);

            var isIntelligent = intelligenceGenerator.IsIntelligent(itemType, attributes, true);
            Assert.That(isIntelligent, Is.False);
        }

        [Test]
        public void DetermineMeleeIntelligence()
        {
            attributes.Add(AttributeConstants.Melee);
            var tableName = String.Format(TableNameConstants.Percentiles.Formattable.IsITEMTYPEIntelligent, AttributeConstants.Melee);
            mockBooleanPercentileSelector.Setup(s => s.SelectFrom(tableName)).Returns(true);

            var isIntelligent = intelligenceGenerator.IsIntelligent(itemType, attributes, true);
            Assert.That(isIntelligent, Is.True);
        }

        [Test]
        public void DetermineRangedIntelligence()
        {
            attributes.Add(AttributeConstants.Ranged);
            var tableName = String.Format(TableNameConstants.Percentiles.Formattable.IsITEMTYPEIntelligent, AttributeConstants.Ranged);
            mockBooleanPercentileSelector.Setup(s => s.SelectFrom(tableName)).Returns(true);

            var isIntelligent = intelligenceGenerator.IsIntelligent(itemType, attributes, true);
            Assert.That(isIntelligent, Is.True);
        }

        [Test]
        public void DetermineRangedAndMeleeIntelligence()
        {
            attributes.Add(AttributeConstants.Melee);
            attributes.Add(AttributeConstants.Ranged);
            var tableName = String.Format(TableNameConstants.Percentiles.Formattable.IsITEMTYPEIntelligent, AttributeConstants.Melee);
            mockBooleanPercentileSelector.Setup(s => s.SelectFrom(tableName)).Returns(true);

            var isIntelligent = intelligenceGenerator.IsIntelligent(itemType, attributes, true);
            Assert.That(isIntelligent, Is.True);
        }

        [Test]
        public void AmmunitionIsNotIntelligent()
        {
            attributes.Add(AttributeConstants.Ammunition);
            var tableName = String.Format(TableNameConstants.Percentiles.Formattable.IsITEMTYPEIntelligent, itemType);
            mockBooleanPercentileSelector.Setup(s => s.SelectFrom(tableName)).Returns(true);

            var isIntelligent = intelligenceGenerator.IsIntelligent(itemType, attributes, true);
            Assert.That(isIntelligent, Is.False);
        }

        [Test]
        public void OneTimeUseItemsAreNotIntelligent()
        {
            attributes.Add(AttributeConstants.OneTimeUse);
            var tableName = String.Format(TableNameConstants.Percentiles.Formattable.IsITEMTYPEIntelligent, itemType);
            mockBooleanPercentileSelector.Setup(s => s.SelectFrom(tableName)).Returns(true);

            var isIntelligent = intelligenceGenerator.IsIntelligent(itemType, attributes, true);
            Assert.That(isIntelligent, Is.False);
        }

        [Test]
        public void NonMagicalItemsAreNotIntelligent()
        {
            var tableName = String.Format(TableNameConstants.Percentiles.Formattable.IsITEMTYPEIntelligent, itemType);
            mockBooleanPercentileSelector.Setup(s => s.SelectFrom(tableName)).Returns(true);

            var isIntelligent = intelligenceGenerator.IsIntelligent(itemType, attributes, false);
            Assert.That(isIntelligent, Is.False);
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
            mockDice.Setup(d => d.Roll(1).d3()).Returns(1);
            mockPercentileSelector.Setup(s => s.SelectFrom(TableNameConstants.Percentiles.Set.IntelligenceStrongStats)).Returns("42");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.CharismaStat, Is.EqualTo(10));
            Assert.That(intelligence.IntelligenceStat, Is.EqualTo(42));
            Assert.That(intelligence.WisdomStat, Is.EqualTo(42));
        }

        [Test]
        public void Roll2MeansIntelligenceIsWeakStat()
        {
            mockDice.Setup(d => d.Roll(1).d3()).Returns(2);
            mockPercentileSelector.Setup(s => s.SelectFrom(TableNameConstants.Percentiles.Set.IntelligenceStrongStats)).Returns("42");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.CharismaStat, Is.EqualTo(42));
            Assert.That(intelligence.IntelligenceStat, Is.EqualTo(10));
            Assert.That(intelligence.WisdomStat, Is.EqualTo(42));
        }

        [Test]
        public void Roll3MeansWisdomIsWeakStat()
        {
            mockDice.Setup(d => d.Roll(1).d3()).Returns(3);
            mockPercentileSelector.Setup(s => s.SelectFrom(TableNameConstants.Percentiles.Set.IntelligenceStrongStats)).Returns("42");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.CharismaStat, Is.EqualTo(42));
            Assert.That(intelligence.IntelligenceStat, Is.EqualTo(42));
            Assert.That(intelligence.WisdomStat, Is.EqualTo(10));
        }

        [Test]
        public void GetCommunicationFromAttributesSelector()
        {
            var attributes = new[] { "talky" };
            mockPercentileSelector.Setup(s => s.SelectFrom(TableNameConstants.Percentiles.Set.IntelligenceStrongStats)).Returns("9266");
            mockAttributesSelector.Setup(s => s.SelectFrom(TableNameConstants.Attributes.Set.IntelligenceCommunication, "9266")).Returns(attributes);

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Communication, Is.EqualTo(attributes));
        }

        [Test]
        public void GetLanguagesIfSpeech()
        {
            var attributes = new[] { "Speech" };
            mockPercentileSelector.Setup(s => s.SelectFrom(TableNameConstants.Percentiles.Set.IntelligenceStrongStats)).Returns("10");
            mockAttributesSelector.Setup(s => s.SelectFrom(TableNameConstants.Attributes.Set.IntelligenceCommunication, "10")).Returns(attributes);

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Languages, Contains.Item("Common"));
        }

        [Test]
        public void GetNumberOfBonusLanguagesEqualToIntelligenceModifier()
        {
            var attributes = new[] { "Speech" };
            mockPercentileSelector.Setup(s => s.SelectFrom(TableNameConstants.Percentiles.Set.IntelligenceStrongStats)).Returns("14");
            mockAttributesSelector.Setup(s => s.SelectFrom(TableNameConstants.Attributes.Set.IntelligenceCommunication, "14")).Returns(attributes);
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(TableNameConstants.Percentiles.Set.Languages)).Returns("english").Returns("german");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Languages, Contains.Item("Common"));
            Assert.That(intelligence.Languages, Contains.Item("english"));
            Assert.That(intelligence.Languages, Contains.Item("german"));
            Assert.That(intelligence.Languages.Count, Is.EqualTo(3));
        }

        [Test]
        public void DoNotHaveDuplicateLanguages()
        {
            var attributes = new[] { "Speech" };
            mockPercentileSelector.Setup(s => s.SelectFrom(TableNameConstants.Percentiles.Set.IntelligenceStrongStats)).Returns("14");
            mockAttributesSelector.Setup(s => s.SelectFrom(TableNameConstants.Attributes.Set.IntelligenceCommunication, "14")).Returns(attributes);
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(TableNameConstants.Percentiles.Set.Languages))
                .Returns("english").Returns("english").Returns("german");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Languages, Contains.Item("Common"));
            Assert.That(intelligence.Languages, Contains.Item("english"));
            Assert.That(intelligence.Languages, Contains.Item("german"));
            Assert.That(intelligence.Languages.Count, Is.EqualTo(3));
        }

        [Test]
        public void GetSensesFromAttributesSelector()
        {
            intelligenceAttributesResult.Senses = "sensy";
            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Senses, Is.EqualTo(intelligenceAttributesResult.Senses));
        }

        [Test]
        public void GetLesserPowersFromAttributesSelector()
        {
            intelligenceAttributesResult.LesserPowersCount = 2;
            var tableName = String.Format(TableNameConstants.Percentiles.Formattable.IntelligencePOWERPowers, "Lesser");
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(tableName)).Returns("power 1").Returns("power 2");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Powers.Count, Is.EqualTo(2));
        }

        [Test]
        public void CannotHaveDuplicateLesserPowers()
        {
            intelligenceAttributesResult.LesserPowersCount = 2;
            var tableName = String.Format(TableNameConstants.Percentiles.Formattable.IntelligencePOWERPowers, "Lesser");
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(tableName)).Returns("power 1").Returns("power 1").Returns("power 2");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Powers, Contains.Item("power 1"));
            Assert.That(intelligence.Powers, Contains.Item("power 2"));
            Assert.That(intelligence.Powers.Count, Is.EqualTo(2));
        }

        [Test]
        public void GetGreaterPowersFromAttributesSelector()
        {
            intelligenceAttributesResult.GreaterPowersCount = 2;
            var tableName = String.Format(TableNameConstants.Percentiles.Formattable.IntelligencePOWERPowers, "Greater");
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(tableName)).Returns("power 1").Returns("power 2");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Powers.Count, Is.EqualTo(2));
        }

        [Test]
        public void CannotHaveDuplicateGreaterPowers()
        {
            intelligenceAttributesResult.GreaterPowersCount = 2;
            var tableName = String.Format(TableNameConstants.Percentiles.Formattable.IntelligencePOWERPowers, "Greater");
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(tableName)).Returns("power 1").Returns("power 1").Returns("power 2");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Powers, Contains.Item("power 1"));
            Assert.That(intelligence.Powers, Contains.Item("power 2"));
            Assert.That(intelligence.Powers.Count, Is.EqualTo(2));
        }

        [Test]
        public void OneGreaterPowerMeans25PercentChanceForSpecialPurpose()
        {
            intelligenceAttributesResult.GreaterPowersCount = 1;
            var tableName = String.Format(TableNameConstants.Percentiles.Formattable.IntelligencePOWERPowers, "Greater");
            mockPercentileSelector.Setup(s => s.SelectFrom(tableName)).Returns("greater power");
            mockDice.Setup(d => d.Roll(1).d4()).Returns(1);
            mockPercentileSelector.Setup(s => s.SelectFrom(TableNameConstants.Percentiles.Set.IntelligenceSpecialPurposes)).Returns("purpose");
            mockPercentileSelector.Setup(s => s.SelectFrom(TableNameConstants.Percentiles.Set.IntelligenceDedicatedPowers)).Returns("dedicated power");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Powers, Is.Empty);
            Assert.That(intelligence.SpecialPurpose, Is.EqualTo("purpose"));
            Assert.That(intelligence.DedicatedPower, Is.EqualTo("dedicated power"));
        }

        [Test]
        public void OneGreaterPowerMeans75PercentChanceForGreaterPower()
        {
            intelligenceAttributesResult.GreaterPowersCount = 1;
            var tableName = String.Format(TableNameConstants.Percentiles.Formattable.IntelligencePOWERPowers, "Greater");
            mockPercentileSelector.Setup(s => s.SelectFrom(tableName)).Returns("greater power");
            mockPercentileSelector.Setup(s => s.SelectFrom(TableNameConstants.Percentiles.Set.IntelligenceSpecialPurposes)).Returns("purpose");
            mockPercentileSelector.Setup(s => s.SelectFrom(TableNameConstants.Percentiles.Set.IntelligenceDedicatedPowers)).Returns("dedicated power");

            for (var roll = 4; roll > 1; roll--)
            {
                mockDice.Setup(d => d.Roll(1).d4()).Returns(roll);

                var intelligence = intelligenceGenerator.GenerateFor(item);
                Assert.That(intelligence.Powers, Contains.Item("greater power"));
                Assert.That(intelligence.Powers.Count, Is.EqualTo(1));
                Assert.That(intelligence.SpecialPurpose, Is.Empty);
                Assert.That(intelligence.DedicatedPower, Is.Empty);
            }
        }

        [Test]
        public void TwoGreaterPowerMeans50PercentChanceForSpecialPurpose()
        {
            intelligenceAttributesResult.GreaterPowersCount = 2;
            mockPercentileSelector.Setup(s => s.SelectFrom(TableNameConstants.Percentiles.Set.IntelligenceSpecialPurposes)).Returns("purpose");
            mockPercentileSelector.Setup(s => s.SelectFrom(TableNameConstants.Percentiles.Set.IntelligenceDedicatedPowers)).Returns("dedicated power");

            for (var roll = 2; roll > 0; roll--)
            {
                var tableName = String.Format(TableNameConstants.Percentiles.Formattable.IntelligencePOWERPowers, "Greater");
                mockPercentileSelector.SetupSequence(s => s.SelectFrom(tableName)).Returns("greater power 1").Returns("greater power 2");
                mockDice.Setup(d => d.Roll(1).d4()).Returns(roll);

                var intelligence = intelligenceGenerator.GenerateFor(item);
                Assert.That(intelligence.Powers, Contains.Item("greater power 1"));
                Assert.That(intelligence.Powers.Count, Is.EqualTo(1));
                Assert.That(intelligence.SpecialPurpose, Is.EqualTo("purpose"));
                Assert.That(intelligence.DedicatedPower, Is.EqualTo("dedicated power"));
            }
        }

        [Test]
        public void TwoGreaterPowerMeans50PercentChanceForGreaterPower()
        {
            intelligenceAttributesResult.GreaterPowersCount = 2;
            mockPercentileSelector.Setup(s => s.SelectFrom(TableNameConstants.Percentiles.Set.IntelligenceSpecialPurposes)).Returns("purpose");
            mockPercentileSelector.Setup(s => s.SelectFrom(TableNameConstants.Percentiles.Set.IntelligenceDedicatedPowers)).Returns("dedicated power");

            for (var roll = 4; roll > 2; roll--)
            {
                var tableName = String.Format(TableNameConstants.Percentiles.Formattable.IntelligencePOWERPowers, "Greater");
                mockPercentileSelector.SetupSequence(s => s.SelectFrom(tableName)).Returns("greater power 1").Returns("greater power 2");
                mockDice.Setup(d => d.Roll(1).d4()).Returns(roll);

                var intelligence = intelligenceGenerator.GenerateFor(item);
                Assert.That(intelligence.Powers, Contains.Item("greater power 1"));
                Assert.That(intelligence.Powers, Contains.Item("greater power 2"));
                Assert.That(intelligence.Powers.Count, Is.EqualTo(2));
                Assert.That(intelligence.SpecialPurpose, Is.Empty);
                Assert.That(intelligence.DedicatedPower, Is.Empty);
            }
        }

        [Test]
        public void ThreeGreaterPowerMeans75PercentChanceForSpecialPurpose()
        {
            intelligenceAttributesResult.GreaterPowersCount = 3;
            mockPercentileSelector.Setup(s => s.SelectFrom(TableNameConstants.Percentiles.Set.IntelligenceSpecialPurposes)).Returns("purpose");
            mockPercentileSelector.Setup(s => s.SelectFrom(TableNameConstants.Percentiles.Set.IntelligenceDedicatedPowers)).Returns("dedicated power");

            for (var roll = 3; roll > 0; roll--)
            {
                var tableName = String.Format(TableNameConstants.Percentiles.Formattable.IntelligencePOWERPowers, "Greater");
                mockPercentileSelector.SetupSequence(s => s.SelectFrom(tableName))
                    .Returns("greater power 1").Returns("greater power 2").Returns("greater power 3");

                mockDice.Setup(d => d.Roll(1).d4()).Returns(roll);

                var intelligence = intelligenceGenerator.GenerateFor(item);
                Assert.That(intelligence.Powers, Contains.Item("greater power 1"));
                Assert.That(intelligence.Powers, Contains.Item("greater power 2"));
                Assert.That(intelligence.Powers.Count, Is.EqualTo(2));
                Assert.That(intelligence.SpecialPurpose, Is.EqualTo("purpose"));
                Assert.That(intelligence.DedicatedPower, Is.EqualTo("dedicated power"));
            }
        }

        [Test]
        public void ThreeGreaterPowerMeans25PercentChanceForGreaterPower()
        {
            intelligenceAttributesResult.GreaterPowersCount = 3;
            mockPercentileSelector.Setup(s => s.SelectFrom(TableNameConstants.Percentiles.Set.IntelligenceSpecialPurposes)).Returns("purpose");
            mockPercentileSelector.Setup(s => s.SelectFrom(TableNameConstants.Percentiles.Set.IntelligenceDedicatedPowers)).Returns("dedicated power");

            for (var roll = 4; roll > 3; roll--)
            {
                var tableName = String.Format(TableNameConstants.Percentiles.Formattable.IntelligencePOWERPowers, "Greater");
                mockPercentileSelector.SetupSequence(s => s.SelectFrom(tableName))
                    .Returns("greater power 1").Returns("greater power 2").Returns("greater power 3");

                mockDice.Setup(d => d.Roll(1).d4()).Returns(roll);

                var intelligence = intelligenceGenerator.GenerateFor(item);
                Assert.That(intelligence.Powers, Contains.Item("greater power 1"));
                Assert.That(intelligence.Powers, Contains.Item("greater power 2"));
                Assert.That(intelligence.Powers, Contains.Item("greater power 3"));
                Assert.That(intelligence.Powers.Count, Is.EqualTo(3));
                Assert.That(intelligence.SpecialPurpose, Is.Empty);
                Assert.That(intelligence.DedicatedPower, Is.Empty);
            }
        }

        [Test]
        public void GetAlignmentFromPercentileSelector()
        {
            mockPercentileSelector.Setup(s => s.SelectFrom(TableNameConstants.Percentiles.Set.IntelligenceAlignments)).Returns("alignment");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Alignment, Is.EqualTo("alignment"));
        }

        [TestCase("Chaotic")]
        public void NonAxiomaticAlignments(String alignment)
        {
            var ability = new SpecialAbility { Name = SpecialAbilityConstants.Axiomatic };
            item.Magic.SpecialAbilities = new[] { ability };
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(TableNameConstants.Percentiles.Set.IntelligenceAlignments))
                .Returns(alignment + " alignment").Returns("alignment");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Alignment, Is.EqualTo("alignment"));
        }

        [TestCase("Neutral")]
        [TestCase("Lawful")]
        [TestCase("True")]
        public void AxiomaticAlignments(String alignment)
        {
            var ability = new SpecialAbility { Name = SpecialAbilityConstants.Axiomatic };
            item.Magic.SpecialAbilities = new[] { ability };
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(TableNameConstants.Percentiles.Set.IntelligenceAlignments))
                .Returns(alignment + " alignment").Returns("alignment");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Alignment, Is.EqualTo(alignment + " alignment"));
        }

        [TestCase("Lawful")]
        public void NonAnarchicAlignments(String alignment)
        {
            var ability = new SpecialAbility { Name = SpecialAbilityConstants.Anarchic };
            item.Magic.SpecialAbilities = new[] { ability };
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(TableNameConstants.Percentiles.Set.IntelligenceAlignments))
                .Returns(alignment + " alignment").Returns("alignment");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Alignment, Is.EqualTo("alignment"));
        }

        [TestCase("Neutral")]
        [TestCase("Chaotic")]
        [TestCase("True")]
        public void AnarchicAlignments(String alignment)
        {
            var ability = new SpecialAbility { Name = SpecialAbilityConstants.Anarchic };
            item.Magic.SpecialAbilities = new[] { ability };
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(TableNameConstants.Percentiles.Set.IntelligenceAlignments))
                .Returns(alignment + " alignment").Returns("alignment");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Alignment, Is.EqualTo(alignment + " alignment"));
        }

        [TestCase("Evil")]
        public void NonHolyAlignments(String alignment)
        {
            var ability = new SpecialAbility { Name = SpecialAbilityConstants.Holy };
            item.Magic.SpecialAbilities = new[] { ability };
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(TableNameConstants.Percentiles.Set.IntelligenceAlignments))
                .Returns("alignment " + alignment).Returns("alignment");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Alignment, Is.EqualTo("alignment"));
        }

        [TestCase("Neutral")]
        [TestCase("Good")]
        public void HolyAlignments(String alignment)
        {
            var ability = new SpecialAbility { Name = SpecialAbilityConstants.Holy };
            item.Magic.SpecialAbilities = new[] { ability };
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(TableNameConstants.Percentiles.Set.IntelligenceAlignments))
                .Returns("alignment " + alignment).Returns("alignment");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Alignment, Is.EqualTo("alignment " + alignment));
        }

        [TestCase("Good")]
        public void NonUnholyAlignments(String alignment)
        {
            var ability = new SpecialAbility { Name = SpecialAbilityConstants.Unholy };
            item.Magic.SpecialAbilities = new[] { ability };
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(TableNameConstants.Percentiles.Set.IntelligenceAlignments))
                .Returns("alignment " + alignment).Returns("alignment");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Alignment, Is.EqualTo("alignment"));
        }

        [TestCase("Neutral")]
        [TestCase("Evil")]
        public void UnholyAlignments(String alignment)
        {
            var ability = new SpecialAbility { Name = SpecialAbilityConstants.Unholy };
            item.Magic.SpecialAbilities = new[] { ability };
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(TableNameConstants.Percentiles.Set.IntelligenceAlignments))
                .Returns("alignment " + alignment).Returns("alignment");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Alignment, Is.EqualTo("alignment " + alignment));
        }

        [Test]
        public void ItemWithSpecificAlignmentHasMatchingAlignment()
        {
            item.Name = "item name";
            mockAttributesSelector.Setup(s => s.SelectFrom(TableNameConstants.Attributes.Set.ItemAlignmentRequirements, "Items"))
                .Returns(new[] { item.Name, "other item name" });
            var alignment = "specific alignment";
            mockAttributesSelector.Setup(s => s.SelectFrom(TableNameConstants.Attributes.Set.ItemAlignmentRequirements, item.Name)).Returns(new[] { alignment });
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(TableNameConstants.Percentiles.Set.IntelligenceAlignments))
                .Returns("alignment").Returns(alignment);

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Alignment, Is.EqualTo(alignment));
        }

        [Test]
        public void ItemWithNoSpecificAlignmentHasAnyAlignment()
        {
            item.Name = "item name";
            mockAttributesSelector.Setup(s => s.SelectFrom(TableNameConstants.Attributes.Set.ItemAlignmentRequirements, "Items")).Returns(new[] { "other item name" });
            mockAttributesSelector.Setup(s => s.SelectFrom(TableNameConstants.Attributes.Set.ItemAlignmentRequirements, item.Name)).Returns(new[] { "specific" });
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(TableNameConstants.Percentiles.Set.IntelligenceAlignments))
                .Returns("alignment").Returns("specific alignment");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Alignment, Is.EqualTo("alignment"));
        }

        [Test]
        public void ItemWithSpecificAlignmentBeginningHasMatchingAlignment()
        {
            item.Name = "item name";
            mockAttributesSelector.Setup(s => s.SelectFrom(TableNameConstants.Attributes.Set.ItemAlignmentRequirements, "Items"))
                .Returns(new[] { item.Name, "other item name" });
            var alignment = "specific";
            mockAttributesSelector.Setup(s => s.SelectFrom(TableNameConstants.Attributes.Set.ItemAlignmentRequirements, item.Name)).Returns(new[] { alignment });
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(TableNameConstants.Percentiles.Set.IntelligenceAlignments))
                .Returns("alignment").Returns("specific alignment");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Alignment, Is.EqualTo("specific alignment"));
        }

        [Test]
        public void ItemWithSpecificAlignmentEndingHasMatchingAlignment()
        {
            item.Name = "item name";
            mockAttributesSelector.Setup(s => s.SelectFrom(TableNameConstants.Attributes.Set.ItemAlignmentRequirements, "Items"))
                .Returns(new[] { item.Name, "other item name" });
            var alignment = "ending";
            mockAttributesSelector.Setup(s => s.SelectFrom(TableNameConstants.Attributes.Set.ItemAlignmentRequirements, item.Name)).Returns(new[] { alignment });
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(TableNameConstants.Percentiles.Set.IntelligenceAlignments))
                .Returns("alignment").Returns("specific alignment ending");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Alignment, Is.EqualTo("specific alignment ending"));
        }

        [Test]
        public void ItemWithAlignmentUsesThatAsAlignmentRequirement()
        {
            Assert.Fail();
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
            var ability = new SpecialAbility();
            ability.BonusEquivalent = 9266;
            item.Magic.SpecialAbilities = new[] { ability };

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Ego, Is.EqualTo(9266));
        }

        [Test]
        public void EgoIncludesLesserPowers()
        {
            intelligenceAttributesResult.LesserPowersCount = 2;
            var tableName = String.Format(TableNameConstants.Percentiles.Formattable.IntelligencePOWERPowers, "Lesser");
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(tableName)).Returns("power 1").Returns("power 2");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Ego, Is.EqualTo(2));
        }

        [Test]
        public void EgoIncludesGreaterPowers()
        {
            intelligenceAttributesResult.GreaterPowersCount = 2;
            var tableName = String.Format(TableNameConstants.Percentiles.Formattable.IntelligencePOWERPowers, "Greater");
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(tableName)).Returns("greater power 1").Returns("greater power 2");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Ego, Is.EqualTo(4));
        }

        [Test]
        public void EgoIncludesDedicatedPower()
        {
            intelligenceAttributesResult.GreaterPowersCount = 1;
            var tableName = String.Format(TableNameConstants.Percentiles.Formattable.IntelligencePOWERPowers, "Greater");
            mockPercentileSelector.Setup(s => s.SelectFrom(tableName)).Returns("greater power");
            mockDice.Setup(d => d.Roll(1).d4()).Returns(1);
            mockPercentileSelector.Setup(s => s.SelectFrom(TableNameConstants.Percentiles.Set.IntelligenceSpecialPurposes)).Returns("purpose");
            mockPercentileSelector.Setup(s => s.SelectFrom(TableNameConstants.Percentiles.Set.IntelligenceDedicatedPowers)).Returns("dedicated power");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Ego, Is.EqualTo(4));
        }

        [Test]
        public void EgoIncludesTelepathy()
        {
            var attributes = new[] { "Telepathy" };
            mockAttributesSelector.Setup(s => s.SelectFrom(TableNameConstants.Attributes.Set.IntelligenceCommunication, "10")).Returns(attributes);

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Ego, Is.EqualTo(1));
        }

        [Test]
        public void EgoIncludesReading()
        {
            var attributes = new[] { "Read" };
            mockAttributesSelector.Setup(s => s.SelectFrom(TableNameConstants.Attributes.Set.IntelligenceCommunication, "10")).Returns(attributes);

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Ego, Is.EqualTo(1));
        }

        [Test]
        public void EgoIncludesReadMagic()
        {
            var attributes = new[] { "Read magic" };
            mockAttributesSelector.Setup(s => s.SelectFrom(TableNameConstants.Attributes.Set.IntelligenceCommunication, "10")).Returns(attributes);

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Ego, Is.EqualTo(1));
        }

        [Test]
        public void EgoIncludesStatBonuses()
        {
            mockPercentileSelector.Setup(s => s.SelectFrom(TableNameConstants.Percentiles.Set.IntelligenceStrongStats)).Returns("19");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Ego, Is.EqualTo(8));
        }

        [Test]
        public void EgoSumsAllFactors()
        {
            var communication = new[] { "Read", "Read magic", "Telepathy" };
            mockPercentileSelector.Setup(s => s.SelectFrom(TableNameConstants.Percentiles.Set.IntelligenceStrongStats)).Returns("19");
            mockAttributesSelector.Setup(s => s.SelectFrom(TableNameConstants.Attributes.Set.IntelligenceCommunication, "19")).Returns(communication);
            intelligenceAttributesResult.LesserPowersCount = 2;
            intelligenceAttributesResult.GreaterPowersCount = 2;

            var tableName = String.Format(TableNameConstants.Percentiles.Formattable.IntelligencePOWERPowers, "Greater");
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(tableName)).Returns("greater power 1").Returns("greater power 2");
            mockDice.Setup(d => d.Roll(1).d4()).Returns(1);
            mockPercentileSelector.Setup(s => s.SelectFrom(TableNameConstants.Percentiles.Set.IntelligenceSpecialPurposes)).Returns("purpose");
            mockPercentileSelector.Setup(s => s.SelectFrom(TableNameConstants.Percentiles.Set.IntelligenceDedicatedPowers)).Returns("dedicated power");

            tableName = String.Format(TableNameConstants.Percentiles.Formattable.IntelligencePOWERPowers, "Lesser");
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(tableName)).Returns("power 1").Returns("power 2");

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
            mockPercentileSelector.Setup(s => s.SelectFrom(TableNameConstants.Percentiles.Set.PersonalityTraits)).Returns("personality");
            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Personality, Is.EqualTo("personality"));
        }

        [Test]
        public void ChooseCategoryForRanksInKnowledge()
        {
            intelligenceAttributesResult.LesserPowersCount = 1;
            var tableName = String.Format(TableNameConstants.Percentiles.Formattable.IntelligencePOWERPowers, "Lesser");
            mockPercentileSelector.Setup(s => s.SelectFrom(tableName)).Returns("this ability has Knowledge");
            mockPercentileSelector.Setup(s => s.SelectFrom(TableNameConstants.Percentiles.Set.KnowledgeCategories)).Returns("category");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Powers, Contains.Item("this ability has Knowledge (category)"));
        }

        [Test]
        public void CanHaveRanksInDifferentKnowledge()
        {
            intelligenceAttributesResult.LesserPowersCount = 2;
            var tableName = String.Format(TableNameConstants.Percentiles.Formattable.IntelligencePOWERPowers, "Lesser");
            mockPercentileSelector.Setup(s => s.SelectFrom(tableName)).Returns("this ability has Knowledge");
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(TableNameConstants.Percentiles.Set.KnowledgeCategories)).Returns("category").Returns("other category");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Powers, Contains.Item("this ability has Knowledge (category)"));
            Assert.That(intelligence.Powers, Contains.Item("this ability has Knowledge (other category)"));
            Assert.That(intelligence.Powers.Count, Is.EqualTo(2));
        }

        [Test]
        public void CannotHaveDuplicateKnowledgeCategories()
        {
            intelligenceAttributesResult.LesserPowersCount = 2;
            var tableName = String.Format(TableNameConstants.Percentiles.Formattable.IntelligencePOWERPowers, "Lesser");
            mockPercentileSelector.Setup(s => s.SelectFrom(tableName)).Returns("this ability has Knowledge");
            mockPercentileSelector.SetupSequence(s => s.SelectFrom(TableNameConstants.Percentiles.Set.KnowledgeCategories)).Returns("category").Returns("category").Returns("other category");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.Powers, Contains.Item("this ability has Knowledge (category)"));
            Assert.That(intelligence.Powers, Contains.Item("this ability has Knowledge (other category)"));
            Assert.That(intelligence.Powers.Count, Is.EqualTo(2));
        }

        [Test]
        public void DesignatedFoesDetermined()
        {
            intelligenceAttributesResult.GreaterPowersCount = 1;
            var tableName = String.Format(TableNameConstants.Percentiles.Formattable.IntelligencePOWERPowers, "Greater");
            mockPercentileSelector.Setup(s => s.SelectFrom(tableName)).Returns("greater power");
            mockDice.Setup(d => d.Roll(1).d4()).Returns(1);
            mockPercentileSelector.Setup(s => s.SelectFrom(TableNameConstants.Percentiles.Set.IntelligenceSpecialPurposes)).Returns("purpose has DESIGNATEDFOE");
            mockPercentileSelector.Setup(s => s.SelectFrom(TableNameConstants.Percentiles.Set.DesignatedFoes)).Returns("foe");

            var intelligence = intelligenceGenerator.GenerateFor(item);
            Assert.That(intelligence.SpecialPurpose, Is.EqualTo("purpose has foe"));
        }
    }
}