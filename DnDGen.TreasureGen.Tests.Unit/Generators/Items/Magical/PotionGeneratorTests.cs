﻿using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.TreasureGen.Generators.Items.Magical;
using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Selectors.Percentiles;
using DnDGen.TreasureGen.Selectors.Selections;
using DnDGen.TreasureGen.Tables;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.TreasureGen.Tests.Unit.Generators.Items.Magical
{
    [TestFixture]
    public class PotionGeneratorTests
    {
        private MagicalItemGenerator potionGenerator;
        private Mock<ITypeAndAmountPercentileSelector> mockTypeAndAmountPercentileSelector;
        private string power;
        private ItemVerifier itemVerifier;
        private Mock<ICollectionSelector> mockCollectionsSelector;
        private Mock<IReplacementSelector> mockReplacementSelector;

        [SetUp]
        public void Setup()
        {
            mockTypeAndAmountPercentileSelector = new Mock<ITypeAndAmountPercentileSelector>();
            mockCollectionsSelector = new Mock<ICollectionSelector>();
            mockReplacementSelector = new Mock<IReplacementSelector>();
            potionGenerator = new PotionGenerator(
                mockTypeAndAmountPercentileSelector.Object,
                mockCollectionsSelector.Object,
                mockReplacementSelector.Object);
            itemVerifier = new ItemVerifier();

            var result = new TypeAndAmountSelection();
            result.Amount = 9266;
            result.Type = "potion";
            power = "power";

            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, power, ItemTypeConstants.Potion);
            mockTypeAndAmountPercentileSelector.Setup(s => s.SelectFrom(tableName)).Returns(result);
        }

        [Test]
        public void GeneratePotion()
        {
            var potion = potionGenerator.GenerateRandom(power);
            Assert.That(potion.Attributes, Contains.Item(AttributeConstants.OneTimeUse));
            Assert.That(potion.IsMagical, Is.True);
            Assert.That(potion.Name, Is.EqualTo("potion"));
            Assert.That(potion.BaseNames.Single(), Is.EqualTo("potion"));
            Assert.That(potion.Magic.Bonus, Is.EqualTo(9266));
            Assert.That(potion.Quantity, Is.EqualTo(1));
            Assert.That(potion.ItemType, Is.EqualTo(ItemTypeConstants.Potion));
        }

        [Test]
        public void GenerateCustomPotion()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);

            var potion = potionGenerator.Generate(template);
            itemVerifier.AssertMagicalItemFromTemplate(potion, template);
            Assert.That(potion.BaseNames.Single(), Is.EqualTo(name));
            Assert.That(potion.Attributes, Contains.Item(AttributeConstants.OneTimeUse));
            Assert.That(potion.IsMagical, Is.True);
            Assert.That(potion.ItemType, Is.EqualTo(ItemTypeConstants.Potion));
            Assert.That(potion.Quantity, Is.EqualTo(1));
        }

        [Test]
        public void GenerateRandomCustomPotion()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);

            var potion = potionGenerator.Generate(template, true);
            itemVerifier.AssertMagicalItemFromTemplate(potion, template);
            Assert.That(potion.BaseNames.Single(), Is.EqualTo(name));
            Assert.That(potion.Attributes, Contains.Item(AttributeConstants.OneTimeUse));
            Assert.That(potion.IsMagical, Is.True);
            Assert.That(potion.ItemType, Is.EqualTo(ItemTypeConstants.Potion));
            Assert.That(potion.Quantity, Is.EqualTo(1));
        }

        [Test]
        public void GenerateFromName()
        {
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, power, ItemTypeConstants.Potion);
            mockTypeAndAmountPercentileSelector.Setup(s => s.SelectAllFrom(tableName)).Returns(new[]
            {
                new TypeAndAmountSelection { Amount = 666, Type = "wrong potion" },
                new TypeAndAmountSelection { Amount = 90210, Type = "potion" },
                new TypeAndAmountSelection { Amount = 42, Type = "other potion" },
            });

            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(It.IsAny<IEnumerable<TypeAndAmountSelection>>()))
                .Returns((IEnumerable<TypeAndAmountSelection> c) => c.Last());

            var potion = potionGenerator.Generate(power, "potion");
            Assert.That(potion.Attributes, Contains.Item(AttributeConstants.OneTimeUse));
            Assert.That(potion.IsMagical, Is.True);
            Assert.That(potion.Name, Is.EqualTo("potion"));
            Assert.That(potion.BaseNames.Single(), Is.EqualTo("potion"));
            Assert.That(potion.Magic.Bonus, Is.EqualTo(90210));
            Assert.That(potion.Quantity, Is.EqualTo(1));
            Assert.That(potion.ItemType, Is.EqualTo(ItemTypeConstants.Potion));
        }

        [Test]
        public void GenerateFromName_WithTraits()
        {
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, power, ItemTypeConstants.Potion);
            mockTypeAndAmountPercentileSelector.Setup(s => s.SelectAllFrom(tableName)).Returns(new[]
            {
                new TypeAndAmountSelection { Amount = 666, Type = "wrong potion" },
                new TypeAndAmountSelection { Amount = 90210, Type = "potion" },
                new TypeAndAmountSelection { Amount = 42, Type = "other potion" },
            });

            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(It.IsAny<IEnumerable<TypeAndAmountSelection>>()))
                .Returns((IEnumerable<TypeAndAmountSelection> c) => c.Last());

            var potion = potionGenerator.Generate(power, "potion", "trait 1", "trait 2");
            Assert.That(potion.Attributes, Contains.Item(AttributeConstants.OneTimeUse));
            Assert.That(potion.IsMagical, Is.True);
            Assert.That(potion.Name, Is.EqualTo("potion"));
            Assert.That(potion.BaseNames.Single(), Is.EqualTo("potion"));
            Assert.That(potion.Magic.Bonus, Is.EqualTo(90210));
            Assert.That(potion.Quantity, Is.EqualTo(1));
            Assert.That(potion.ItemType, Is.EqualTo(ItemTypeConstants.Potion));
            Assert.That(potion.Traits, Has.Count.EqualTo(2)
                .And.Contains("trait 1")
                .And.Contains("trait 2"));
        }

        [Test]
        public void GenerateFromName_MultipleOfPower()
        {
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, power, ItemTypeConstants.Potion);
            mockTypeAndAmountPercentileSelector.Setup(s => s.SelectAllFrom(tableName)).Returns(new[]
            {
                new TypeAndAmountSelection { Amount = 666, Type = "wrong potion" },
                new TypeAndAmountSelection { Amount = 90210, Type = "potion" },
                new TypeAndAmountSelection { Amount = 42, Type = "potion" },
            });

            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(It.IsAny<IEnumerable<TypeAndAmountSelection>>()))
                .Returns((IEnumerable<TypeAndAmountSelection> c) => c.Last());

            var potion = potionGenerator.Generate(power, "potion");
            Assert.That(potion.Attributes, Contains.Item(AttributeConstants.OneTimeUse));
            Assert.That(potion.IsMagical, Is.True);
            Assert.That(potion.Name, Is.EqualTo("potion"));
            Assert.That(potion.BaseNames.Single(), Is.EqualTo("potion"));
            Assert.That(potion.Magic.Bonus, Is.EqualTo(42));
            Assert.That(potion.Quantity, Is.EqualTo(1));
            Assert.That(potion.ItemType, Is.EqualTo(ItemTypeConstants.Potion));
        }

        [Test]
        public void GenerateFromName_NoneOfPower()
        {
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, power, ItemTypeConstants.Potion);
            mockTypeAndAmountPercentileSelector.Setup(s => s.SelectAllFrom(tableName)).Returns(new[]
            {
                new TypeAndAmountSelection { Amount = 666, Type = "wrong potion" },
                new TypeAndAmountSelection { Amount = 42, Type = "other potion" },
            });

            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(It.IsAny<IEnumerable<TypeAndAmountSelection>>()))
                .Returns((IEnumerable<TypeAndAmountSelection> c) => c.Last());

            Assert.That(() => potionGenerator.Generate(power, "potion"),
                Throws.ArgumentException.With.Message.EqualTo("potion is not a valid power Potion"));
        }

        [Test]
        public void BUG_GenerateFromName_NeedsReplacement()
        {
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, power, ItemTypeConstants.Potion);
            mockTypeAndAmountPercentileSelector.Setup(s => s.SelectAllFrom(tableName)).Returns(new[]
            {
                new TypeAndAmountSelection { Amount = 666, Type = "wrong potion" },
                new TypeAndAmountSelection { Amount = 90210, Type = "potion" },
                new TypeAndAmountSelection { Amount = 42, Type = "other potion" },
            });

            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(It.IsAny<IEnumerable<TypeAndAmountSelection>>()))
                .Returns((IEnumerable<TypeAndAmountSelection> c) => c.Last());

            mockReplacementSelector
                .Setup(s => s.SelectAll("needs replacement"))
                .Returns(new[]
                {
                    "other wrong potion",
                    "potion",
                });

            var potion = potionGenerator.Generate(power, "needs replacement");
            Assert.That(potion.Attributes, Contains.Item(AttributeConstants.OneTimeUse));
            Assert.That(potion.IsMagical, Is.True);
            Assert.That(potion.Name, Is.EqualTo("potion"));
            Assert.That(potion.BaseNames.Single(), Is.EqualTo("potion"));
            Assert.That(potion.Magic.Bonus, Is.EqualTo(90210));
            Assert.That(potion.Quantity, Is.EqualTo(1));
            Assert.That(potion.ItemType, Is.EqualTo(ItemTypeConstants.Potion));
        }

        [Test]
        public void IsItemOfPower_ReturnsFalse()
        {
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, power, ItemTypeConstants.Potion);
            mockTypeAndAmountPercentileSelector.Setup(s => s.SelectAllFrom(tableName)).Returns(new[]
            {
                new TypeAndAmountSelection { Amount = 666, Type = "wrong potion" },
                new TypeAndAmountSelection { Amount = 42, Type = "other potion" },
            });

            var isOfPower = potionGenerator.IsItemOfPower("potion", power);
            Assert.That(isOfPower, Is.False);
        }

        [Test]
        public void IsItemOfPower_ReturnsTrue()
        {
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, power, ItemTypeConstants.Potion);
            mockTypeAndAmountPercentileSelector.Setup(s => s.SelectAllFrom(tableName)).Returns(new[]
            {
                new TypeAndAmountSelection { Amount = 666, Type = "wrong potion" },
                new TypeAndAmountSelection { Amount = 90210, Type = "potion" },
                new TypeAndAmountSelection { Amount = 42, Type = "other potion" },
            });

            var isOfPower = potionGenerator.IsItemOfPower("potion", power);
            Assert.That(isOfPower, Is.True);
        }

        [Test]
        public void BUG_IsItemOfPower_ReturnsTrue_WithReplacement()
        {
            var tableName = string.Format(TableNameConstants.Percentiles.Formattable.POWERITEMTYPEs, power, ItemTypeConstants.Potion);
            mockTypeAndAmountPercentileSelector.Setup(s => s.SelectAllFrom(tableName)).Returns(new[]
            {
                new TypeAndAmountSelection { Amount = 666, Type = "wrong potion" },
                new TypeAndAmountSelection { Amount = 90210, Type = "potion" },
                new TypeAndAmountSelection { Amount = 42, Type = "other potion" },
            });

            mockReplacementSelector
                .Setup(s => s.SelectAll("needs replacement"))
                .Returns(new[]
                {
                    "other wrong potion",
                    "potion",
                });

            var isOfPower = potionGenerator.IsItemOfPower("needs replacement", power);
            Assert.That(isOfPower, Is.True);
        }
    }
}