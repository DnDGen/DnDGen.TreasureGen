﻿using DnDGen.Core.Selectors.Collections;
using Moq;
using NUnit.Framework;
using System;
using System.Linq;
using TreasureGen.Domain.Generators.Items.Magical;
using TreasureGen.Domain.Selectors.Percentiles;
using TreasureGen.Domain.Selectors.Selections;
using TreasureGen.Domain.Tables;
using TreasureGen.Items;
using TreasureGen.Items.Magical;

namespace TreasureGen.Tests.Unit.Generators.Items.Magical
{
    [TestFixture]
    public class RingGeneratorTests
    {
        private MagicalItemGenerator ringGenerator;
        private Mock<ITypeAndAmountPercentileSelector> mockTypeAndAmountPercentileSelector;
        private Mock<ITreasurePercentileSelector> mockPercentileSelector;
        private Mock<ICollectionsSelector> mockCollectionsSelector;
        private Mock<IChargesGenerator> mockChargesGenerator;
        private Mock<ISpellGenerator> mockSpellGenerator;
        private TypeAndAmountSelection selection;
        private string power;
        private ItemVerifier itemVerifier;

        [SetUp]
        public void Setup()
        {
            mockCollectionsSelector = new Mock<ICollectionsSelector>();
            mockTypeAndAmountPercentileSelector = new Mock<ITypeAndAmountPercentileSelector>();
            mockChargesGenerator = new Mock<IChargesGenerator>();
            mockSpellGenerator = new Mock<ISpellGenerator>();
            mockPercentileSelector = new Mock<ITreasurePercentileSelector>();
            selection = new TypeAndAmountSelection();
            var generator = new IterativeGeneratorWithoutLogging(5);
            ringGenerator = new RingGenerator(mockPercentileSelector.Object, mockCollectionsSelector.Object, mockSpellGenerator.Object, mockChargesGenerator.Object, mockTypeAndAmountPercentileSelector.Object, generator);
            power = "power";
            itemVerifier = new ItemVerifier();

            selection.Amount = 9266;
            selection.Type = "ring of ability";

            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, power, ItemTypeConstants.Ring);
            mockTypeAndAmountPercentileSelector.Setup(p => p.SelectFrom(tableName)).Returns(selection);
        }

        [Test]
        public void GenerateRing()
        {
            var ring = ringGenerator.GenerateAtPower(power);
            Assert.That(ring.Name, Is.EqualTo("ring of ability"));
            Assert.That(ring.BaseNames.Single(), Is.EqualTo("ring of ability"));
            Assert.That(ring.IsMagical, Is.True);
            Assert.That(ring.ItemType, Is.EqualTo(ItemTypeConstants.Ring));
            Assert.That(ring.Magic.Bonus, Is.EqualTo(9266));
        }

        [Test]
        public void GetAttributesFromSelector()
        {
            var attributes = new[] { "attribute 1", "attribute 2" };
            var tableName = string.Format(TableNameConstants.Collections.Formattable.ITEMTYPEAttributes, ItemTypeConstants.Ring);
            mockCollectionsSelector.Setup(p => p.SelectFrom(tableName, selection.Type)).Returns(attributes);

            var ring = ringGenerator.GenerateAtPower(power);
            Assert.That(ring.Attributes, Is.EqualTo(attributes));
        }

        [Test]
        public void GetChargesIfCharged()
        {
            var attributes = new[] { AttributeConstants.Charged };
            var tableName = string.Format(TableNameConstants.Collections.Formattable.ITEMTYPEAttributes, ItemTypeConstants.Ring);
            mockCollectionsSelector.Setup(p => p.SelectFrom(tableName, selection.Type)).Returns(attributes);
            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Ring, selection.Type)).Returns(9266);

            var ring = ringGenerator.GenerateAtPower(power);
            Assert.That(ring.Magic.Charges, Is.EqualTo(9266));
        }

        [Test]
        public void DoNotGetChargesIfNotCharged()
        {
            var attributes = new[] { "new attribute" };
            var tableName = string.Format(TableNameConstants.Collections.Formattable.ITEMTYPEAttributes, ItemTypeConstants.Ring);
            mockCollectionsSelector.Setup(p => p.SelectFrom(tableName, selection.Type)).Returns(attributes);
            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Ring, selection.Type)).Returns(9266);

            var ring = ringGenerator.GenerateAtPower(power);
            Assert.That(ring.Magic.Charges, Is.EqualTo(0));
        }

        [Test]
        public void MinorSpellStoringHasSpell()
        {
            selection.Type = RingConstants.SpellStoring_Minor;
            mockSpellGenerator.Setup(g => g.GenerateType()).Returns("spell type");
            mockSpellGenerator.Setup(g => g.GenerateLevel(power)).Returns(2);
            mockSpellGenerator.Setup(g => g.Generate("spell type", 2)).Returns("spell");

            var ring = ringGenerator.GenerateAtPower(power);
            Assert.That(ring.Name, Is.EqualTo(RingConstants.SpellStoring_Minor));
            Assert.That(ring.Contents, Contains.Item("spell (spell type, 2)"));
            Assert.That(ring.Contents.Count, Is.EqualTo(1));
        }

        [Test]
        public void MinorSpellStoringHasAtMost3SpellLevels()
        {
            selection.Type = RingConstants.SpellStoring_Minor;
            mockSpellGenerator.Setup(g => g.GenerateType()).Returns("spell type");
            mockSpellGenerator.Setup(g => g.GenerateLevel(power)).Returns(1);
            mockSpellGenerator.SetupSequence(g => g.Generate("spell type", 1)).Returns("spell 1").Returns("spell 2").Returns("spell 3").Returns("spell 4");

            var ring = ringGenerator.GenerateAtPower(power);
            Assert.That(ring.Name, Is.EqualTo(RingConstants.SpellStoring_Minor));
            Assert.That(ring.Contents, Contains.Item("spell 1 (spell type, 1)"));
            Assert.That(ring.Contents, Contains.Item("spell 2 (spell type, 1)"));
            Assert.That(ring.Contents, Contains.Item("spell 3 (spell type, 1)"));
            Assert.That(ring.Contents.Count, Is.EqualTo(3));
            mockSpellGenerator.Verify(g => g.GenerateType(), Times.Exactly(3));
            mockSpellGenerator.Verify(g => g.GenerateLevel(power), Times.AtLeast(3));
        }

        [Test]
        public void MinorSpellStoringAllowsDuplicateSpells()
        {
            selection.Type = RingConstants.SpellStoring_Minor;
            mockSpellGenerator.Setup(g => g.GenerateType()).Returns("spell type");
            mockSpellGenerator.Setup(g => g.GenerateLevel(power)).Returns(1);
            mockSpellGenerator.Setup(g => g.Generate("spell type", 1)).Returns("spell");

            var ring = ringGenerator.GenerateAtPower(power);
            Assert.That(ring.Name, Is.EqualTo(RingConstants.SpellStoring_Minor));
            Assert.That(ring.Contents, Contains.Item("spell (spell type, 1)"));
            Assert.That(ring.Contents.Count, Is.EqualTo(3));
            Assert.That(ring.Contents.Distinct().Count(), Is.EqualTo(1));
        }

        [Test]
        public void SpellStoringHasSpell()
        {
            selection.Type = RingConstants.SpellStoring;
            mockSpellGenerator.Setup(g => g.GenerateType()).Returns("spell type");
            mockSpellGenerator.Setup(g => g.GenerateLevel(power)).Returns(3);
            mockSpellGenerator.Setup(g => g.Generate("spell type", 3)).Returns("spell");

            var ring = ringGenerator.GenerateAtPower(power);
            Assert.That(ring.Name, Is.EqualTo(RingConstants.SpellStoring));
            Assert.That(ring.Contents, Contains.Item("spell (spell type, 3)"));
            Assert.That(ring.Contents.Count, Is.EqualTo(1));
        }

        [Test]
        public void SpellStoringHasAtMost5SpellLevels()
        {
            selection.Type = RingConstants.SpellStoring;
            mockSpellGenerator.Setup(g => g.GenerateType()).Returns("spell type");
            mockSpellGenerator.Setup(g => g.GenerateLevel(power)).Returns(1);
            mockSpellGenerator.SetupSequence(g => g.Generate("spell type", 1)).Returns("spell 1").Returns("spell 2")
                .Returns("spell 3").Returns("spell 4").Returns("spell 5").Returns("spell 6");

            var ring = ringGenerator.GenerateAtPower(power);
            Assert.That(ring.Name, Is.EqualTo(RingConstants.SpellStoring));
            Assert.That(ring.Contents, Contains.Item("spell 1 (spell type, 1)"));
            Assert.That(ring.Contents, Contains.Item("spell 2 (spell type, 1)"));
            Assert.That(ring.Contents, Contains.Item("spell 3 (spell type, 1)"));
            Assert.That(ring.Contents, Contains.Item("spell 4 (spell type, 1)"));
            Assert.That(ring.Contents, Contains.Item("spell 5 (spell type, 1)"));
            Assert.That(ring.Contents.Count, Is.EqualTo(5));
            mockSpellGenerator.Verify(g => g.GenerateType(), Times.Exactly(5));
            mockSpellGenerator.Verify(g => g.GenerateLevel(power), Times.AtLeast(5));
        }

        [Test]
        public void SpellStoringAllowsDuplicateSpells()
        {
            selection.Type = RingConstants.SpellStoring;
            mockSpellGenerator.Setup(g => g.GenerateType()).Returns("spell type");
            mockSpellGenerator.Setup(g => g.GenerateLevel(power)).Returns(1);
            mockSpellGenerator.Setup(g => g.Generate("spell type", 1)).Returns("spell");

            var ring = ringGenerator.GenerateAtPower(power);
            Assert.That(ring.Name, Is.EqualTo(RingConstants.SpellStoring));
            Assert.That(ring.Contents, Contains.Item("spell (spell type, 1)"));
            Assert.That(ring.Contents.Count, Is.EqualTo(5));
            Assert.That(ring.Contents.Distinct().Count(), Is.EqualTo(1));
        }

        [Test]
        public void MajorSpellStoringHasSpell()
        {
            selection.Type = RingConstants.SpellStoring_Major;
            mockSpellGenerator.Setup(g => g.GenerateType()).Returns("spell type");
            mockSpellGenerator.Setup(g => g.GenerateLevel(power)).Returns(6);
            mockSpellGenerator.Setup(g => g.Generate("spell type", 6)).Returns("spell");

            var ring = ringGenerator.GenerateAtPower(power);
            Assert.That(ring.Name, Is.EqualTo(RingConstants.SpellStoring_Major));
            Assert.That(ring.Contents, Contains.Item("spell (spell type, 6)"));
            Assert.That(ring.Contents.Count, Is.EqualTo(1));
        }

        [Test]
        public void MajorSpellStoringHasAtMost10SpellLevels()
        {
            selection.Type = RingConstants.SpellStoring_Major;
            mockSpellGenerator.Setup(g => g.GenerateType()).Returns("spell type");
            mockSpellGenerator.Setup(g => g.GenerateLevel(power)).Returns(1);
            mockSpellGenerator.SetupSequence(g => g.Generate("spell type", 1)).Returns("spell 1").Returns("spell 2")
                .Returns("spell 3").Returns("spell 4").Returns("spell 5").Returns("spell 6").Returns("spell 7").Returns("spell 8")
                .Returns("spell 9").Returns("spell 10").Returns("spell 11");

            var ring = ringGenerator.GenerateAtPower(power);
            Assert.That(ring.Name, Is.EqualTo(RingConstants.SpellStoring_Major));
            Assert.That(ring.Contents, Contains.Item("spell 1 (spell type, 1)"));
            Assert.That(ring.Contents, Contains.Item("spell 2 (spell type, 1)"));
            Assert.That(ring.Contents, Contains.Item("spell 3 (spell type, 1)"));
            Assert.That(ring.Contents, Contains.Item("spell 4 (spell type, 1)"));
            Assert.That(ring.Contents, Contains.Item("spell 5 (spell type, 1)"));
            Assert.That(ring.Contents, Contains.Item("spell 6 (spell type, 1)"));
            Assert.That(ring.Contents, Contains.Item("spell 7 (spell type, 1)"));
            Assert.That(ring.Contents, Contains.Item("spell 8 (spell type, 1)"));
            Assert.That(ring.Contents, Contains.Item("spell 9 (spell type, 1)"));
            Assert.That(ring.Contents, Contains.Item("spell 10 (spell type, 1)"));
            Assert.That(ring.Contents.Count, Is.EqualTo(10));
            mockSpellGenerator.Verify(g => g.GenerateType(), Times.Exactly(10));
            mockSpellGenerator.Verify(g => g.GenerateLevel(power), Times.AtLeast(10));
        }

        [Test]
        public void MajorSpellStoringAllowsDuplicateSpells()
        {
            selection.Type = RingConstants.SpellStoring_Major;
            mockSpellGenerator.Setup(g => g.GenerateType()).Returns("spell type");
            mockSpellGenerator.Setup(g => g.GenerateLevel(power)).Returns(1);
            mockSpellGenerator.Setup(g => g.Generate("spell type", 1)).Returns("spell");

            var ring = ringGenerator.GenerateAtPower(power);
            Assert.That(ring.Name, Is.EqualTo(RingConstants.SpellStoring_Major));
            Assert.That(ring.Contents, Contains.Item("spell (spell type, 1)"));
            Assert.That(ring.Contents.Count, Is.EqualTo(10));
            Assert.That(ring.Contents.Distinct().Count(), Is.EqualTo(1));
        }

        [Test]
        public void CounterspellsHasSpell()
        {
            selection.Type = RingConstants.Counterspells;
            mockSpellGenerator.Setup(g => g.GenerateType()).Returns("spell type");
            mockSpellGenerator.Setup(g => g.GenerateLevel(power)).Returns(4);
            mockSpellGenerator.Setup(g => g.Generate("spell type", 4)).Returns("spell");

            var ring = ringGenerator.GenerateAtPower(power);
            Assert.That(ring.Name, Is.EqualTo(RingConstants.Counterspells));
            Assert.That(ring.Contents, Contains.Item("spell"));
            Assert.That(ring.Contents.Count, Is.EqualTo(1));
        }

        [Test]
        public void CounterspellsHasAtMost1Spell()
        {
            selection.Type = RingConstants.Counterspells;
            mockSpellGenerator.Setup(g => g.GenerateType()).Returns("spell type");
            mockSpellGenerator.Setup(g => g.GenerateLevel(power)).Returns(1);
            mockSpellGenerator.SetupSequence(g => g.Generate("spell type", 1)).Returns("spell 1").Returns("spell 2");

            var ring = ringGenerator.GenerateAtPower(power);
            Assert.That(ring.Name, Is.EqualTo(RingConstants.Counterspells));
            Assert.That(ring.Contents, Contains.Item("spell 1"));
            Assert.That(ring.Contents.Count, Is.EqualTo(1));
            mockSpellGenerator.Verify(g => g.GenerateType(), Times.Exactly(1));
            mockSpellGenerator.Verify(g => g.GenerateLevel(power), Times.Exactly(1));
        }

        [Test]
        public void CounterspellsHasSpellOfAtMostSixthLevel()
        {
            selection.Type = RingConstants.Counterspells;
            mockSpellGenerator.Setup(g => g.GenerateType()).Returns("spell type");
            mockSpellGenerator.Setup(g => g.GenerateLevel(power)).Returns(6);
            mockSpellGenerator.Setup(g => g.Generate("spell type", 6)).Returns("spell");

            var ring = ringGenerator.GenerateAtPower(power);
            Assert.That(ring.Name, Is.EqualTo(RingConstants.Counterspells));
            Assert.That(ring.Contents, Contains.Item("spell"));
            Assert.That(ring.Contents.Count, Is.EqualTo(1));
        }

        [Test]
        public void CounterspellsHasNoSpellHigherThanSixthLevel()
        {
            selection.Type = RingConstants.Counterspells;
            mockSpellGenerator.Setup(g => g.GenerateType()).Returns("spell type");
            mockSpellGenerator.Setup(g => g.GenerateLevel(power)).Returns(7);
            mockSpellGenerator.Setup(g => g.Generate("spell type", 7)).Returns("spell");

            var ring = ringGenerator.GenerateAtPower(power);
            Assert.That(ring.Name, Is.EqualTo(RingConstants.Counterspells));
            Assert.That(ring.Contents, Is.Empty);
        }

        [Test]
        public void GenerateCustomRing()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);

            var attributes = new[] { "attribute 1", "attribute 2" };
            var tableName = string.Format(TableNameConstants.Collections.Formattable.ITEMTYPEAttributes, ItemTypeConstants.Ring);
            mockCollectionsSelector.Setup(p => p.SelectFrom(tableName, name)).Returns(attributes);

            var ring = ringGenerator.Generate(template);
            itemVerifier.AssertMagicalItemFromTemplate(ring, template);
            Assert.That(ring.BaseNames.Single(), Is.EqualTo(name));
            Assert.That(ring.Attributes, Is.EquivalentTo(attributes));
            Assert.That(ring.IsMagical, Is.True);
            Assert.That(ring.ItemType, Is.EqualTo(ItemTypeConstants.Ring));
            Assert.That(ring.Quantity, Is.EqualTo(1));
        }

        [Test]
        public void GenerateRandomCustomRing()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);

            var attributes = new[] { "attribute 1", "attribute 2" };
            var tableName = string.Format(TableNameConstants.Collections.Formattable.ITEMTYPEAttributes, ItemTypeConstants.Ring);
            mockCollectionsSelector.Setup(p => p.SelectFrom(tableName, name)).Returns(attributes);

            var ring = ringGenerator.Generate(template, true);
            itemVerifier.AssertMagicalItemFromTemplate(ring, template);
            Assert.That(ring.BaseNames.Single(), Is.EqualTo(name));
            Assert.That(ring.Attributes, Is.EquivalentTo(attributes));
            Assert.That(ring.IsMagical, Is.True);
            Assert.That(ring.ItemType, Is.EqualTo(ItemTypeConstants.Ring));
            Assert.That(ring.Quantity, Is.EqualTo(1));
        }

        [Test]
        public void GenerateOneTimeUseRing()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);

            var attributes = new[] { "attribute 1", "attribute 2", AttributeConstants.OneTimeUse };
            var tableName = string.Format(TableNameConstants.Collections.Formattable.ITEMTYPEAttributes, ItemTypeConstants.Ring);
            mockCollectionsSelector.Setup(p => p.SelectFrom(tableName, name)).Returns(attributes);

            var ring = ringGenerator.Generate(template);
            itemVerifier.AssertMagicalItemFromTemplate(ring, template);
            Assert.That(ring.BaseNames.Single(), Is.EqualTo(name));
            Assert.That(ring.Attributes, Is.EquivalentTo(attributes));
            Assert.That(ring.IsMagical, Is.True);
            Assert.That(ring.ItemType, Is.EqualTo(ItemTypeConstants.Ring));
            Assert.That(ring.Quantity, Is.EqualTo(1));
        }

        [Test]
        public void GenerateFromSubset()
        {
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, power, ItemTypeConstants.Ring);
            mockTypeAndAmountPercentileSelector.SetupSequence(p => p.SelectFrom(tableName))
                .Returns(new TypeAndAmountSelection { Type = "wrong ring", Amount = 666 })
                .Returns(new TypeAndAmountSelection { Type = "ring", Amount = 9266 })
                .Returns(new TypeAndAmountSelection { Type = "other ring", Amount = 90210 });

            var attributes = new[] { "attribute 1", "attribute 2" };
            tableName = string.Format(TableNameConstants.Collections.Formattable.ITEMTYPEAttributes, ItemTypeConstants.Ring);
            mockCollectionsSelector.Setup(p => p.SelectFrom(tableName, "ring")).Returns(attributes);

            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Ring, It.IsAny<string>())).Returns(666);

            var subset = new[] { "other ring", "ring" };

            var ring = ringGenerator.GenerateFromSubset(power, subset);
            Assert.That(ring.Name, Is.EqualTo("ring"));
            Assert.That(ring.BaseNames.Single(), Is.EqualTo("ring"));
            Assert.That(ring.IsMagical, Is.True);
            Assert.That(ring.ItemType, Is.EqualTo(ItemTypeConstants.Ring));
            Assert.That(ring.Magic.Bonus, Is.EqualTo(9266));
            Assert.That(ring.Magic.Charges, Is.EqualTo(0));
            Assert.That(ring.Attributes, Is.EqualTo(attributes));
        }

        [Test]
        public void GenerateChargedFromSubset()
        {
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, power, ItemTypeConstants.Ring);
            mockTypeAndAmountPercentileSelector.SetupSequence(p => p.SelectFrom(tableName))
                .Returns(new TypeAndAmountSelection { Type = "wrong ring", Amount = 666 })
                .Returns(new TypeAndAmountSelection { Type = "ring", Amount = 9266 })
                .Returns(new TypeAndAmountSelection { Type = "other ring", Amount = 90210 });

            var attributes = new[] { "attribute 1", "attribute 2", AttributeConstants.Charged };
            tableName = string.Format(TableNameConstants.Collections.Formattable.ITEMTYPEAttributes, ItemTypeConstants.Ring);
            mockCollectionsSelector.Setup(p => p.SelectFrom(tableName, "ring")).Returns(attributes);

            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Ring, "ring")).Returns(42);

            var subset = new[] { "other ring", "ring" };

            var ring = ringGenerator.GenerateFromSubset(power, subset);
            Assert.That(ring.Name, Is.EqualTo("ring"));
            Assert.That(ring.BaseNames.Single(), Is.EqualTo("ring"));
            Assert.That(ring.IsMagical, Is.True);
            Assert.That(ring.ItemType, Is.EqualTo(ItemTypeConstants.Ring));
            Assert.That(ring.Magic.Bonus, Is.EqualTo(9266));
            Assert.That(ring.Magic.Charges, Is.EqualTo(42));
            Assert.That(ring.Attributes, Is.EqualTo(attributes));
        }

        [TestCase(RingConstants.SpellStoring_Minor, 3)]
        [TestCase(RingConstants.SpellStoring, 5)]
        [TestCase(RingConstants.SpellStoring_Major, 10)]
        [TestCase(RingConstants.Counterspells, 1)]
        public void GenerateRingWithSpellFromSubset(string name, int totalSpells)
        {
            mockSpellGenerator.Setup(g => g.GenerateType()).Returns("spell type");
            mockSpellGenerator.Setup(g => g.GenerateLevel(power)).Returns(1);
            mockSpellGenerator.Setup(g => g.Generate("spell type", 1)).Returns("spell");

            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, power, ItemTypeConstants.Ring);
            mockTypeAndAmountPercentileSelector.SetupSequence(p => p.SelectFrom(tableName))
                .Returns(new TypeAndAmountSelection { Type = "wrong ring", Amount = 666 })
                .Returns(new TypeAndAmountSelection { Type = name, Amount = 9266 })
                .Returns(new TypeAndAmountSelection { Type = "other ring", Amount = 90210 });

            var attributes = new[] { "attribute 1", "attribute 2" };
            tableName = string.Format(TableNameConstants.Collections.Formattable.ITEMTYPEAttributes, ItemTypeConstants.Ring);
            mockCollectionsSelector.Setup(p => p.SelectFrom(tableName, name)).Returns(attributes);

            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Ring, It.IsAny<string>())).Returns(666);

            var subset = new[] { "other ring", name };

            var ring = ringGenerator.GenerateFromSubset(power, subset);
            Assert.That(ring.Name, Is.EqualTo(name));
            Assert.That(ring.BaseNames.Single(), Is.EqualTo(name));
            Assert.That(ring.IsMagical, Is.True);
            Assert.That(ring.ItemType, Is.EqualTo(ItemTypeConstants.Ring));
            Assert.That(ring.Magic.Bonus, Is.EqualTo(9266));
            Assert.That(ring.Magic.Charges, Is.EqualTo(0));
            Assert.That(ring.Attributes, Is.EqualTo(attributes));
            Assert.That(ring.Contents, Contains.Item("spell (spell type, 1)").Or.Contain("spell"));
            Assert.That(ring.Contents.Count, Is.EqualTo(totalSpells));
        }

        [Test]
        public void GenerateDefaultFromSubset()
        {
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, power, ItemTypeConstants.Ring);
            mockTypeAndAmountPercentileSelector.Setup(p => p.SelectFrom(tableName)).Returns(new TypeAndAmountSelection { Type = "wrong ring", Amount = 666 });
            mockTypeAndAmountPercentileSelector.Setup(p => p.SelectAllFrom(tableName)).Returns(new[]
            {
                new TypeAndAmountSelection { Type = "wrong ring", Amount = 666 },
                new TypeAndAmountSelection { Type = "ring", Amount = 9266 },
                new TypeAndAmountSelection { Type = "other ring", Amount = 90210 },
            });

            var attributes = new[] { "attribute 1", "attribute 2" };
            tableName = string.Format(TableNameConstants.Collections.Formattable.ITEMTYPEAttributes, ItemTypeConstants.Ring);
            mockCollectionsSelector.Setup(p => p.SelectFrom(tableName, "ring")).Returns(attributes);

            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Ring, It.IsAny<string>())).Returns(666);

            var subset = new[] { "other ring", "ring" };
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(subset)).Returns(subset.Last());

            var ring = ringGenerator.GenerateFromSubset(power, subset);
            Assert.That(ring.Name, Is.EqualTo("ring"));
            Assert.That(ring.BaseNames.Single(), Is.EqualTo("ring"));
            Assert.That(ring.IsMagical, Is.True);
            Assert.That(ring.ItemType, Is.EqualTo(ItemTypeConstants.Ring));
            Assert.That(ring.Magic.Bonus, Is.EqualTo(9266));
            Assert.That(ring.Magic.Charges, Is.EqualTo(0));
            Assert.That(ring.Attributes, Is.EqualTo(attributes));
        }

        [Test]
        public void GenerateDefaultChargedFromSubset()
        {
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, power, ItemTypeConstants.Ring);
            mockTypeAndAmountPercentileSelector.Setup(p => p.SelectFrom(tableName)).Returns(new TypeAndAmountSelection { Type = "wrong ring", Amount = 666 });
            mockTypeAndAmountPercentileSelector.Setup(p => p.SelectAllFrom(tableName)).Returns(new[]
            {
                new TypeAndAmountSelection { Type = "wrong ring", Amount = 666 },
                new TypeAndAmountSelection { Type = "ring", Amount = 9266 },
                new TypeAndAmountSelection { Type = "other ring", Amount = 90210 },
            });

            var attributes = new[] { "attribute 1", "attribute 2", AttributeConstants.Charged };
            tableName = string.Format(TableNameConstants.Collections.Formattable.ITEMTYPEAttributes, ItemTypeConstants.Ring);
            mockCollectionsSelector.Setup(p => p.SelectFrom(tableName, "ring")).Returns(attributes);

            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Ring, "ring")).Returns(42);

            var subset = new[] { "other ring", "ring" };
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(subset)).Returns(subset.Last());

            var ring = ringGenerator.GenerateFromSubset(power, subset);
            Assert.That(ring.Name, Is.EqualTo("ring"));
            Assert.That(ring.BaseNames.Single(), Is.EqualTo("ring"));
            Assert.That(ring.IsMagical, Is.True);
            Assert.That(ring.ItemType, Is.EqualTo(ItemTypeConstants.Ring));
            Assert.That(ring.Magic.Bonus, Is.EqualTo(9266));
            Assert.That(ring.Magic.Charges, Is.EqualTo(42));
            Assert.That(ring.Attributes, Is.EqualTo(attributes));
        }

        [TestCase(RingConstants.SpellStoring_Minor, 3)]
        [TestCase(RingConstants.SpellStoring, 5)]
        [TestCase(RingConstants.SpellStoring_Major, 10)]
        [TestCase(RingConstants.Counterspells, 1)]
        public void GenerateDefaultRingWithSpellFromSubset(string name, int totalSpells)
        {
            mockSpellGenerator.Setup(g => g.GenerateType()).Returns("spell type");
            mockSpellGenerator.Setup(g => g.GenerateLevel(power)).Returns(1);
            mockSpellGenerator.Setup(g => g.Generate("spell type", 1)).Returns("spell");

            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, power, ItemTypeConstants.Ring);
            mockTypeAndAmountPercentileSelector.Setup(p => p.SelectFrom(tableName)).Returns(new TypeAndAmountSelection { Type = "wrong ring", Amount = 666 });
            mockTypeAndAmountPercentileSelector.Setup(p => p.SelectAllFrom(tableName)).Returns(new[]
            {
                new TypeAndAmountSelection { Type = "wrong ring", Amount = 666 },
                new TypeAndAmountSelection { Type = name, Amount = 9266 },
                new TypeAndAmountSelection { Type = "other ring", Amount = 90210 },
            });

            var attributes = new[] { "attribute 1", "attribute 2" };
            tableName = string.Format(TableNameConstants.Collections.Formattable.ITEMTYPEAttributes, ItemTypeConstants.Ring);
            mockCollectionsSelector.Setup(p => p.SelectFrom(tableName, name)).Returns(attributes);

            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.Ring, It.IsAny<string>())).Returns(666);

            var subset = new[] { "other ring", name };
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(subset)).Returns(subset.Last());

            var ring = ringGenerator.GenerateFromSubset(power, subset);
            Assert.That(ring.Name, Is.EqualTo(name));
            Assert.That(ring.BaseNames.Single(), Is.EqualTo(name));
            Assert.That(ring.IsMagical, Is.True);
            Assert.That(ring.ItemType, Is.EqualTo(ItemTypeConstants.Ring));
            Assert.That(ring.Magic.Bonus, Is.EqualTo(9266));
            Assert.That(ring.Magic.Charges, Is.EqualTo(0));
            Assert.That(ring.Attributes, Is.EqualTo(attributes));
            Assert.That(ring.Contents, Contains.Item("spell (spell type, 1)").Or.Contain("spell"));
            Assert.That(ring.Contents.Count, Is.EqualTo(totalSpells));
        }

        [Test]
        public void GenerateDefaultFromSubsetWithDifferentPower()
        {
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, power, ItemTypeConstants.Ring);
            mockTypeAndAmountPercentileSelector.Setup(s => s.SelectFrom(tableName)).Returns(new TypeAndAmountSelection { Type = "wrong ring", Amount = 9266 });
            mockTypeAndAmountPercentileSelector.Setup(s => s.SelectAllFrom(tableName)).Returns(new[]
            {
                new TypeAndAmountSelection { Amount = 666, Type = "wrong ring" },
                new TypeAndAmountSelection { Amount = 42, Type = "other ring" },
            });

            tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, PowerConstants.Minor, ItemTypeConstants.Ring);
            mockTypeAndAmountPercentileSelector.Setup(s => s.SelectAllFrom(tableName)).Returns(new[]
            {
                new TypeAndAmountSelection { Amount = 666, Type = "wrong ring" },
            });

            tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, PowerConstants.Medium, ItemTypeConstants.Ring);
            mockTypeAndAmountPercentileSelector.Setup(s => s.SelectAllFrom(tableName)).Returns(new[]
            {
                new TypeAndAmountSelection { Amount = 42, Type = "other ring" },
            });

            tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, PowerConstants.Major, ItemTypeConstants.Ring);
            mockTypeAndAmountPercentileSelector.Setup(s => s.SelectAllFrom(tableName)).Returns(new[]
            {
                new TypeAndAmountSelection { Amount = 90210, Type = "ring" },
            });

            var attributes = new[] { "attribute 1", "attribute 2" };
            tableName = string.Format(TableNameConstants.Collections.Formattable.ITEMTYPEAttributes, ItemTypeConstants.Ring);
            mockCollectionsSelector.Setup(p => p.SelectFrom(tableName, "ring")).Returns(attributes);

            var subset = new[] { "other ring", "ring" };
            mockCollectionsSelector.Setup(s => s.SelectRandomFrom(subset)).Returns(subset.Last());

            var ring = ringGenerator.GenerateFromSubset(power, subset);
            Assert.That(ring.Name, Is.EqualTo("ring"));
            Assert.That(ring.BaseNames.Single(), Is.EqualTo("ring"));
            Assert.That(ring.IsMagical, Is.True);
            Assert.That(ring.ItemType, Is.EqualTo(ItemTypeConstants.Ring));
            Assert.That(ring.Magic.Bonus, Is.EqualTo(90210));
            Assert.That(ring.Magic.Charges, Is.EqualTo(0));
            Assert.That(ring.Attributes, Is.EqualTo(attributes));
        }
    }
}