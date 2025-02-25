using DnDGen.Infrastructure.Models;
using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.Infrastructure.Selectors.Percentiles;
using DnDGen.RollGen;
using DnDGen.TreasureGen.Generators.Items.Magical;
using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Selectors.Percentiles;
using DnDGen.TreasureGen.Tables;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.TreasureGen.Tests.Unit.Generators.Items.Magical
{
    [TestFixture]
    public class WondrousItemGeneratorTests
    {
        private MagicalItemGenerator wondrousItemGenerator;
        private Mock<ITreasurePercentileSelector> mockPercentileSelector;
        private Mock<ICollectionSelector> mockCollectionsSelector;
        private Mock<IChargesGenerator> mockChargesGenerator;
        private Mock<Dice> mockDice;
        private Mock<ISpellGenerator> mockSpellGenerator;
        private Mock<IPercentileTypeAndAmountSelector> mockTypeAndAmountPercentileSelector;
        private TypeAndAmountDataSelection selection;
        private string power;
        private ItemVerifier itemVerifier;

        [SetUp]
        public void Setup()
        {
            mockCollectionsSelector = new Mock<ICollectionSelector>();
            mockChargesGenerator = new Mock<IChargesGenerator>();
            mockSpellGenerator = new Mock<ISpellGenerator>();
            mockPercentileSelector = new Mock<ITreasurePercentileSelector>();
            mockDice = new Mock<Dice>();
            mockTypeAndAmountPercentileSelector = new Mock<IPercentileTypeAndAmountSelector>();
            wondrousItemGenerator = new WondrousItemGenerator(
                mockPercentileSelector.Object,
                mockCollectionsSelector.Object,
                mockChargesGenerator.Object,
                mockDice.Object,
                mockSpellGenerator.Object,
                mockTypeAndAmountPercentileSelector.Object);

            itemVerifier = new ItemVerifier();
            selection = new TypeAndAmountDataSelection();

            power = "power";
            selection.Type = "wondrous item";
            selection.AmountAsDouble = 0;
            var tableName = TableNameConstants.Percentiles.POWERITEMTYPEs(power, ItemTypeConstants.WondrousItem);
            mockTypeAndAmountPercentileSelector.Setup(p => p.SelectFrom(Config.Name, tableName)).Returns(selection);
        }

        [Test]
        public void GenerateWondrousItem()
        {
            var item = wondrousItemGenerator.GenerateRandom(power);
            Assert.That(item.Name, Is.EqualTo(selection.Type));
            Assert.That(item.BaseNames.Single(), Is.EqualTo(selection.Type));
            Assert.That(item.ItemType, Is.EqualTo(ItemTypeConstants.WondrousItem));
            Assert.That(item.IsMagical, Is.True);
        }

        [Test]
        public void GetAttributesFromSelector()
        {
            var attributes = new[] { "type 1", "type 2" };
            var tableName = TableNameConstants.Collections.ITEMTYPEAttributes(ItemTypeConstants.WondrousItem);
            mockCollectionsSelector.Setup(p => p.SelectFrom(Config.Name, tableName, selection.Type)).Returns(attributes);

            var item = wondrousItemGenerator.GenerateRandom(power);
            Assert.That(item.Attributes, Is.EqualTo(attributes));
        }

        [Test]
        public void DoNotGetChargesIfNotCharged()
        {
            var attributes = new[] { "type 1", "type 2" };
            var tableName = TableNameConstants.Collections.ITEMTYPEAttributes(ItemTypeConstants.WondrousItem);
            mockCollectionsSelector.Setup(p => p.SelectFrom(Config.Name, tableName, selection.Type)).Returns(attributes);

            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.WondrousItem, "wondrous item")).Returns(9266);

            var item = wondrousItemGenerator.GenerateRandom(power);
            Assert.That(item.Magic.Charges, Is.EqualTo(0));
        }

        [Test]
        public void GetChargesIfCharged()
        {
            var attributes = new[] { AttributeConstants.Charged };
            var tableName = TableNameConstants.Collections.ITEMTYPEAttributes(ItemTypeConstants.WondrousItem);
            mockCollectionsSelector.Setup(p => p.SelectFrom(Config.Name, tableName, selection.Type)).Returns(attributes);

            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.WondrousItem, selection.Type)).Returns(9266);

            var item = wondrousItemGenerator.GenerateRandom(power);
            Assert.That(item.Magic.Charges, Is.EqualTo(9266));
        }

        [Test]
        public void GetBonus()
        {
            selection.AmountAsDouble = 90210;
            var item = wondrousItemGenerator.GenerateRandom(power);
            Assert.That(item.Magic.Bonus, Is.EqualTo(90210));
        }

        [Test]
        public void HornOfValhallaGetsType()
        {
            selection.Type = WondrousItemConstants.HornOfValhalla;
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, TableNameConstants.Percentiles.HornOfValhallaTypes)).Returns("metallic");

            var item = wondrousItemGenerator.GenerateRandom(power);
            Assert.That(item.Name, Is.EqualTo(WondrousItemConstants.HornOfValhalla));
            Assert.That(item.Traits, Contains.Item("metallic"));
            Assert.That(item.Traits.Count, Is.EqualTo(1));
        }

        [Test]
        public void IronFlaskContentsGenerated()
        {
            selection.Type = WondrousItemConstants.IronFlask;
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, TableNameConstants.Percentiles.IronFlaskContents)).Returns("contents");

            var item = wondrousItemGenerator.GenerateRandom(power);
            Assert.That(item.Name, Is.EqualTo(WondrousItemConstants.IronFlask));
            Assert.That(item.Contents, Contains.Item("contents"));
        }

        [Test]
        public void IronFlaskContentsDoNotContainEmptyString()
        {
            selection.Type = WondrousItemConstants.IronFlask;
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, TableNameConstants.Percentiles.IronFlaskContents)).Returns(string.Empty);

            var item = wondrousItemGenerator.GenerateRandom(power);
            Assert.That(item.Name, Is.EqualTo(WondrousItemConstants.IronFlask));
            Assert.That(item.Contents, Is.Empty);
        }

        [Test]
        public void IronFlaskOnlyContainsOneThing()
        {
            selection.Type = WondrousItemConstants.IronFlask;
            mockPercentileSelector.SetupSequence(p => p.SelectFrom(Config.Name, TableNameConstants.Percentiles.IronFlaskContents)).Returns("contents").Returns("more contents");

            var item = wondrousItemGenerator.GenerateRandom(power);
            Assert.That(item.Name, Is.EqualTo(WondrousItemConstants.IronFlask));
            Assert.That(item.Contents, Contains.Item("contents"));
            Assert.That(item.Contents.Count, Is.EqualTo(1));
        }

        [Test]
        public void IfBalorOrPitFiend_GetFromSelector()
        {
            selection.Type = WondrousItemConstants.IronFlask;
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, TableNameConstants.Percentiles.IronFlaskContents)).Returns(TableNameConstants.Percentiles.BalorOrPitFiend);
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, TableNameConstants.Percentiles.BalorOrPitFiend)).Returns("balor or pit fiend");

            var item = wondrousItemGenerator.GenerateRandom(power);
            Assert.That(item.Name, Is.EqualTo(WondrousItemConstants.IronFlask));
            Assert.That(item.Contents, Contains.Item("balor or pit fiend"));
        }

        [Test]
        public void RobeOfUsefulItemsBaseItemsAdded()
        {
            SetUpRoll(4, 4, 0);
            selection.Type = WondrousItemConstants.RobeOfUsefulItems;
            var items = new[] { "item 1", "item 1", "item 2", "item 2", "item 3", "item 3" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.WondrousItemContents, selection.Type)).Returns(items);

            var item = wondrousItemGenerator.GenerateRandom(power);
            Assert.That(item.Name, Is.EqualTo(selection.Type));
            Assert.That(item.Contents, Is.SupersetOf(items));
        }

        [Test]
        public void RobeOfUsefulItemsExtraItemsDetermined()
        {
            selection.Type = WondrousItemConstants.RobeOfUsefulItems;
            SetUpRoll(4, 4, 2);
            mockPercentileSelector.SetupSequence(p => p.SelectFrom(Config.Name, TableNameConstants.Percentiles.RobeOfUsefulItemsExtraItems)).Returns("item 1").Returns("item 2");

            var item = wondrousItemGenerator.GenerateRandom(power);
            Assert.That(item.Name, Is.EqualTo(selection.Type));
            Assert.That(item.Contents, Contains.Item("item 1"));
            Assert.That(item.Contents, Contains.Item("item 2"));
            Assert.That(item.Contents.Count, Is.EqualTo(2));
        }

        [Test]
        public void RobeOfUsefulItemsCanHaveDuplicateExtraItems()
        {
            selection.Type = WondrousItemConstants.RobeOfUsefulItems;
            SetUpRoll(4, 4, 2);
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, TableNameConstants.Percentiles.RobeOfUsefulItemsExtraItems)).Returns("item 1");

            var item = wondrousItemGenerator.GenerateRandom(power);
            Assert.That(item.Name, Is.EqualTo(selection.Type));
            Assert.That(item.Contents, Contains.Item("item 1"));
            Assert.That(item.Contents.Count(i => i == "item 1"), Is.EqualTo(2));
            Assert.That(item.Contents.Count, Is.EqualTo(2));
        }

        [Test]
        public void RobeOfUsefulItemsExtraItemsScrollDetermined()
        {
            selection.Type = WondrousItemConstants.RobeOfUsefulItems;
            SetUpRoll(4, 4, 1);
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, TableNameConstants.Percentiles.RobeOfUsefulItemsExtraItems)).Returns("Scroll");
            mockSpellGenerator.Setup(g => g.GenerateLevel(PowerConstants.Minor)).Returns(9266);
            mockSpellGenerator.Setup(g => g.GenerateType()).Returns("spell type");
            mockSpellGenerator.Setup(g => g.Generate("spell type", 9266)).Returns("spell");

            var item = wondrousItemGenerator.GenerateRandom(power);
            Assert.That(item.Name, Is.EqualTo(selection.Type));
            Assert.That(item.Contents, Contains.Item("spell type scroll of spell (9266)"));
        }

        [Test]
        public void CubicGateGetsPlanes()
        {
            selection.Type = WondrousItemConstants.CubicGate;
            mockPercentileSelector.SetupSequence(p => p.SelectFrom(Config.Name, TableNameConstants.Percentiles.Planes)).Returns("plane 1").Returns("plane 2").Returns("plane 3")
                .Returns("plane 4").Returns("plane 5");

            var item = wondrousItemGenerator.GenerateRandom(power);
            Assert.That(item.Name, Is.EqualTo(selection.Type));
            Assert.That(item.Contents, Contains.Item("Material Plane"));
            Assert.That(item.Contents, Contains.Item("plane 1"));
            Assert.That(item.Contents, Contains.Item("plane 2"));
            Assert.That(item.Contents, Contains.Item("plane 3"));
            Assert.That(item.Contents, Contains.Item("plane 4"));
            Assert.That(item.Contents, Contains.Item("plane 5"));
            Assert.That(item.Contents.Count, Is.EqualTo(6));
        }

        [Test]
        public void CubicGateGetsDistinctPlanes()
        {
            selection.Type = WondrousItemConstants.CubicGate;
            mockPercentileSelector.SetupSequence(p => p.SelectFrom(Config.Name, TableNameConstants.Percentiles.Planes)).Returns("plane 1").Returns("plane 1").Returns("plane 2")
                .Returns("plane 3").Returns("plane 4").Returns("plane 5");

            var item = wondrousItemGenerator.GenerateRandom(power);
            Assert.That(item.Name, Is.EqualTo(selection.Type));
            Assert.That(item.Contents, Contains.Item("Material Plane"));
            Assert.That(item.Contents, Contains.Item("plane 1"));
            Assert.That(item.Contents, Contains.Item("plane 2"));
            Assert.That(item.Contents, Contains.Item("plane 3"));
            Assert.That(item.Contents, Contains.Item("plane 4"));
            Assert.That(item.Contents, Contains.Item("plane 5"));
            Assert.That(item.Contents.Count, Is.EqualTo(6));
        }

        [Test]
        public void GetCardsForDeckOfIllusions()
        {
            selection.Type = WondrousItemConstants.DeckOfIllusions;
            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.WondrousItem, selection.Type)).Returns(3);
            var cards = new[] { "card 1", "card 2", "card 3", "card 4" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.WondrousItemContents, selection.Type)).Returns(cards);
            SetUpRandomSelections(cards, 0, 1, 3);

            var item = wondrousItemGenerator.GenerateRandom(power);
            Assert.That(item.Name, Is.EqualTo(selection.Type));
            Assert.That(item.Contents, Contains.Item("card 1"));
            Assert.That(item.Contents, Contains.Item("card 2"));
            Assert.That(item.Contents, Contains.Item("card 4"));
            Assert.That(item.Contents.Count, Is.EqualTo(3));
        }

        [Test]
        public void GetSpheresForNecklaceOfFireballs()
        {
            selection.Type = WondrousItemConstants.NecklaceOfFireballs + " type whatever";
            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.WondrousItem, selection.Type)).Returns(4);
            var spheres = new[] { "small sphere", "big sphere", "normal sphere", "normal sphere", "big sphere" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.WondrousItemContents, selection.Type)).Returns(spheres);
            SetUpRandomSelections(spheres, 0, 1, 2, 4);

            var item = wondrousItemGenerator.GenerateRandom(power);
            Assert.That(item.Name, Is.EqualTo(selection.Type));
            Assert.That(item.Contents, Contains.Item("small sphere"));
            Assert.That(item.Contents, Contains.Item("normal sphere"));
            Assert.That(item.Contents, Contains.Item("big sphere"));
            Assert.That(item.Contents.Count(c => c == "big sphere"), Is.EqualTo(2));
            Assert.That(item.Contents.Count, Is.EqualTo(4));
        }

        private void SetUpRoll(int quantity, int die, int result)
        {
            var mockPartial = new Mock<PartialRoll>();
            mockPartial.Setup(p => p.AsSum<int>()).Returns(result);
            mockDice.Setup(d => d.Roll(quantity).d(die)).Returns(mockPartial.Object);
        }

        [Test]
        public void RobeOfBonesItemsAdded()
        {
            selection.Type = WondrousItemConstants.RobeOfBones;
            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.WondrousItem, selection.Type)).Returns(4);
            var items = new[] { "undead 1", "undead 1", "undead 2", "undead 2", "undead 3", "undead 3" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.WondrousItemContents, selection.Type)).Returns(items);
            SetUpRandomSelections(items, 0, 1, 2, 4);

            var item = wondrousItemGenerator.GenerateRandom(power);
            Assert.That(item.Name, Is.EqualTo(selection.Type));
            Assert.That(item.Contents, Contains.Item("undead 1"));
            Assert.That(item.Contents.Count(c => c == "undead 1"), Is.EqualTo(2));
            Assert.That(item.Contents, Contains.Item("undead 2"));
            Assert.That(item.Contents, Contains.Item("undead 3"));
            Assert.That(item.Contents.Count, Is.EqualTo(4));
        }

        [Test]
        public void DoNotRollIfFullContents()
        {
            selection.Type = WondrousItemConstants.RobeOfBones;
            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.WondrousItem, selection.Type)).Returns(6);
            var items = new[] { "undead 1", "undead 1", "undead 2", "undead 2", "undead 3", "undead 3" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.WondrousItemContents, selection.Type)).Returns(items);

            var item = wondrousItemGenerator.GenerateRandom(power);
            Assert.That(item.Name, Is.EqualTo(selection.Type));
            Assert.That(item.Contents, Contains.Item("undead 1"));
            Assert.That(item.Contents.Count(c => c == "undead 1"), Is.EqualTo(2));
            Assert.That(item.Contents, Contains.Item("undead 2"));
            Assert.That(item.Contents.Count(c => c == "undead 2"), Is.EqualTo(2));
            Assert.That(item.Contents, Contains.Item("undead 3"));
            Assert.That(item.Contents.Count(c => c == "undead 3"), Is.EqualTo(2));
            Assert.That(item.Contents.Count, Is.EqualTo(6));
            mockDice.Verify(d => d.Roll(It.IsAny<int>()).d(It.IsAny<int>()), Times.Never);
        }

        [Test]
        public void CandleOfInvocationGetsAlignment()
        {
            selection.Type = WondrousItemConstants.CandleOfInvocation;
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.IntelligenceAlignments)).Returns("alignment");

            var item = wondrousItemGenerator.GenerateRandom(power);
            Assert.That(item.Name, Is.EqualTo(selection.Type));
            Assert.That(item.Traits, Contains.Item("alignment"));
            Assert.That(item.Traits.Count, Is.EqualTo(1));
        }

        [Test]
        public void RobeOfTheArchmagiGetsAlignment()
        {
            selection.Type = WondrousItemConstants.RobeOfTheArchmagi;
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.RobeOfTheArchmagiColors)).Returns("color (alignment)");

            var item = wondrousItemGenerator.GenerateRandom(power);
            Assert.That(item.Name, Is.EqualTo(selection.Type));
            Assert.That(item.Traits, Contains.Item("color (alignment)"));
            Assert.That(item.Traits.Count, Is.EqualTo(1));
        }

        [Test]
        public void GenerateCustomWondrousItem()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);

            var attributes = new[] { "type 1", "type 2" };
            var tableName = TableNameConstants.Collections.ITEMTYPEAttributes(ItemTypeConstants.WondrousItem);
            mockCollectionsSelector.Setup(p => p.SelectFrom(Config.Name, tableName, name)).Returns(attributes);

            var wondrousItem = wondrousItemGenerator.Generate(template);
            itemVerifier.AssertMagicalItemFromTemplate(wondrousItem, template);
            Assert.That(wondrousItem.Name, Is.EqualTo(name));
            Assert.That(wondrousItem.BaseNames.Single(), Is.EqualTo(name));
            Assert.That(wondrousItem.ItemType, Is.EqualTo(ItemTypeConstants.WondrousItem));
            Assert.That(wondrousItem.IsMagical, Is.True);
            Assert.That(wondrousItem.Attributes, Is.EqualTo(attributes));
        }

        [Test]
        public void GenerateRandomCustomWondrousItem()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);

            var attributes = new[] { "type 1", "type 2" };
            var tableName = TableNameConstants.Collections.ITEMTYPEAttributes(ItemTypeConstants.WondrousItem);
            mockCollectionsSelector.Setup(p => p.SelectFrom(Config.Name, tableName, name)).Returns(attributes);

            var wondrousItem = wondrousItemGenerator.Generate(template, true);
            itemVerifier.AssertMagicalItemFromTemplate(wondrousItem, template);
            Assert.That(wondrousItem.Name, Is.EqualTo(name));
            Assert.That(wondrousItem.BaseNames.Single(), Is.EqualTo(name));
            Assert.That(wondrousItem.ItemType, Is.EqualTo(ItemTypeConstants.WondrousItem));
            Assert.That(wondrousItem.IsMagical, Is.True);
            Assert.That(wondrousItem.Attributes, Is.EqualTo(attributes));
        }

        [Test]
        public void GenerateFromName()
        {
            var tableName = TableNameConstants.Percentiles.POWERITEMTYPEs(power, ItemTypeConstants.WondrousItem);
            var selections = new[]
            {
                new TypeAndAmountDataSelection { Type = "wrong wondrous item", AmountAsDouble = 666 },
                new TypeAndAmountDataSelection { Type = "wondrous item", AmountAsDouble = 9266 },
                new TypeAndAmountDataSelection { Type = "other wondrous item", AmountAsDouble = 90210 }
            };

            mockTypeAndAmountPercentileSelector.Setup(s => s.SelectAllFrom(Config.Name, tableName)).Returns(selections);

            var attributes = new[] { "type 1", "type 2" };
            tableName = TableNameConstants.Collections.ITEMTYPEAttributes(ItemTypeConstants.WondrousItem);
            mockCollectionsSelector.Setup(p => p.SelectFrom(Config.Name, tableName, "wondrous item")).Returns(attributes);

            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.WondrousItem, It.IsAny<string>())).Returns(666);

            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(It.IsAny<IEnumerable<TypeAndAmountDataSelection>>()))
                .Returns((IEnumerable<TypeAndAmountDataSelection> c) => c.Last());

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, "wondrous item"))
                .Returns(new[] { "wrong power", power, "other power" });

            var item = wondrousItemGenerator.Generate(power, "wondrous item");
            Assert.That(item.Name, Is.EqualTo("wondrous item"));
            Assert.That(item.BaseNames.Single(), Is.EqualTo("wondrous item"));
            Assert.That(item.ItemType, Is.EqualTo(ItemTypeConstants.WondrousItem));
            Assert.That(item.Attributes, Is.EqualTo(attributes));
            Assert.That(item.IsMagical, Is.True);
            Assert.That(item.Magic.Bonus, Is.EqualTo(9266));
            Assert.That(item.Magic.Charges, Is.Zero);
        }

        [Test]
        public void GenerateFromName_WithTraits()
        {
            var tableName = TableNameConstants.Percentiles.POWERITEMTYPEs(power, ItemTypeConstants.WondrousItem);
            var selections = new[]
            {
                new TypeAndAmountDataSelection { Type = "wrong wondrous item", AmountAsDouble = 666 },
                new TypeAndAmountDataSelection { Type = "wondrous item", AmountAsDouble = 9266 },
                new TypeAndAmountDataSelection { Type = "other wondrous item", AmountAsDouble = 90210 }
            };

            mockTypeAndAmountPercentileSelector.Setup(s => s.SelectAllFrom(Config.Name, tableName)).Returns(selections);

            var attributes = new[] { "type 1", "type 2" };
            tableName = TableNameConstants.Collections.ITEMTYPEAttributes(ItemTypeConstants.WondrousItem);
            mockCollectionsSelector.Setup(p => p.SelectFrom(Config.Name, tableName, "wondrous item")).Returns(attributes);

            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.WondrousItem, It.IsAny<string>())).Returns(666);

            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(It.IsAny<IEnumerable<TypeAndAmountDataSelection>>()))
                .Returns((IEnumerable<TypeAndAmountDataSelection> c) => c.Last());

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, "wondrous item"))
                .Returns(new[] { "wrong power", power, "other power" });

            var item = wondrousItemGenerator.Generate(power, "wondrous item", "trait 1", "trait 2");
            Assert.That(item.Name, Is.EqualTo("wondrous item"));
            Assert.That(item.BaseNames.Single(), Is.EqualTo("wondrous item"));
            Assert.That(item.ItemType, Is.EqualTo(ItemTypeConstants.WondrousItem));
            Assert.That(item.Attributes, Is.EqualTo(attributes));
            Assert.That(item.IsMagical, Is.True);
            Assert.That(item.Magic.Bonus, Is.EqualTo(9266));
            Assert.That(item.Magic.Charges, Is.Zero);
            Assert.That(item.Traits, Has.Count.EqualTo(2)
                .And.Contains("trait 1")
                .And.Contains("trait 2"));
        }

        [Test]
        public void GenerateFromName_MultipleOfPower()
        {
            var tableName = TableNameConstants.Percentiles.POWERITEMTYPEs(power, ItemTypeConstants.WondrousItem);
            var selections = new[]
            {
                new TypeAndAmountDataSelection { Type = "wrong wondrous item", AmountAsDouble = 666 },
                new TypeAndAmountDataSelection { Type = "wondrous item", AmountAsDouble = 9266 },
                new TypeAndAmountDataSelection { Type = "wondrous item", AmountAsDouble = 90210 }
            };

            mockTypeAndAmountPercentileSelector.Setup(s => s.SelectAllFrom(Config.Name, tableName)).Returns(selections);

            var attributes = new[] { "type 1", "type 2" };
            tableName = TableNameConstants.Collections.ITEMTYPEAttributes(ItemTypeConstants.WondrousItem);
            mockCollectionsSelector.Setup(p => p.SelectFrom(Config.Name, tableName, "wondrous item")).Returns(attributes);

            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.WondrousItem, It.IsAny<string>())).Returns(666);

            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(It.IsAny<IEnumerable<TypeAndAmountDataSelection>>()))
                .Returns((IEnumerable<TypeAndAmountDataSelection> c) => c.Last());

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, "wondrous item"))
                .Returns(new[] { "wrong power", power, "other power" });

            var item = wondrousItemGenerator.Generate(power, "wondrous item");
            Assert.That(item.Name, Is.EqualTo("wondrous item"));
            Assert.That(item.BaseNames.Single(), Is.EqualTo("wondrous item"));
            Assert.That(item.ItemType, Is.EqualTo(ItemTypeConstants.WondrousItem));
            Assert.That(item.Attributes, Is.EqualTo(attributes));
            Assert.That(item.IsMagical, Is.True);
            Assert.That(item.Magic.Bonus, Is.EqualTo(90210));
            Assert.That(item.Magic.Charges, Is.Zero);
        }

        [Test]
        public void GenerateFromName_NoneOfPower()
        {
            var tableName = TableNameConstants.Percentiles.POWERITEMTYPEs(power, ItemTypeConstants.WondrousItem);
            var selections = new[]
            {
                new TypeAndAmountDataSelection { Type = "wrong wondrous item", AmountAsDouble = 666 },
                new TypeAndAmountDataSelection { Type = "wondrous item", AmountAsDouble = 9266 },
                new TypeAndAmountDataSelection { Type = "other wondrous item", AmountAsDouble = 90210 }
            };

            mockTypeAndAmountPercentileSelector.Setup(s => s.SelectAllFrom(Config.Name, tableName)).Returns(selections);

            var attributes = new[] { "type 1", "type 2" };
            tableName = TableNameConstants.Collections.ITEMTYPEAttributes(ItemTypeConstants.WondrousItem);
            mockCollectionsSelector.Setup(p => p.SelectFrom(Config.Name, tableName, "wondrous item")).Returns(attributes);

            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.WondrousItem, It.IsAny<string>())).Returns(666);

            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(It.IsAny<IEnumerable<TypeAndAmountDataSelection>>()))
                .Returns((IEnumerable<TypeAndAmountDataSelection> c) => c.Last());

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, "wondrous item"))
                .Returns(new[] { power, "wrong power" });

            var item = wondrousItemGenerator.Generate("other power", "wondrous item");
            Assert.That(item.Name, Is.EqualTo("wondrous item"));
            Assert.That(item.BaseNames.Single(), Is.EqualTo("wondrous item"));
            Assert.That(item.ItemType, Is.EqualTo(ItemTypeConstants.WondrousItem));
            Assert.That(item.Attributes, Is.EqualTo(attributes));
            Assert.That(item.IsMagical, Is.True);
            Assert.That(item.Magic.Bonus, Is.EqualTo(9266));
            Assert.That(item.Magic.Charges, Is.Zero);
        }

        [Test]
        public void GenerateChargedFromName()
        {
            var tableName = TableNameConstants.Percentiles.POWERITEMTYPEs(power, ItemTypeConstants.WondrousItem);
            var selections = new[]
            {
                new TypeAndAmountDataSelection { Type = "wrong wondrous item", AmountAsDouble = 666 },
                new TypeAndAmountDataSelection { Type = "wondrous item", AmountAsDouble = 9266 },
                new TypeAndAmountDataSelection { Type = "other wondrous item", AmountAsDouble = 90210 }
            };

            mockTypeAndAmountPercentileSelector.Setup(s => s.SelectAllFrom(Config.Name, tableName)).Returns(selections);

            var attributes = new[] { "type 1", "type 2", AttributeConstants.Charged };
            tableName = TableNameConstants.Collections.ITEMTYPEAttributes(ItemTypeConstants.WondrousItem);
            mockCollectionsSelector.Setup(p => p.SelectFrom(Config.Name, tableName, "wondrous item")).Returns(attributes);

            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.WondrousItem, It.IsAny<string>())).Returns(42);

            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(It.IsAny<IEnumerable<TypeAndAmountDataSelection>>()))
                .Returns((IEnumerable<TypeAndAmountDataSelection> c) => c.Last());

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, "wondrous item"))
                .Returns(new[] { "wrong power", power, "other power" });

            var item = wondrousItemGenerator.Generate(power, "wondrous item");
            Assert.That(item.Name, Is.EqualTo("wondrous item"));
            Assert.That(item.BaseNames.Single(), Is.EqualTo("wondrous item"));
            Assert.That(item.ItemType, Is.EqualTo(ItemTypeConstants.WondrousItem));
            Assert.That(item.Attributes, Is.EqualTo(attributes));
            Assert.That(item.IsMagical, Is.True);
            Assert.That(item.Magic.Bonus, Is.EqualTo(9266));
            Assert.That(item.Magic.Charges, Is.EqualTo(42));
        }

        [Test]
        public void GenerateHornOfValhallaFromName()
        {
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, TableNameConstants.Percentiles.HornOfValhallaTypes)).Returns("metallic");

            var tableName = TableNameConstants.Percentiles.POWERITEMTYPEs(power, ItemTypeConstants.WondrousItem);
            var selections = new[]
            {
                new TypeAndAmountDataSelection { Type = "wrong wondrous item", AmountAsDouble = 666 },
                new TypeAndAmountDataSelection { Type = WondrousItemConstants.HornOfValhalla, AmountAsDouble = 9266 },
                new TypeAndAmountDataSelection { Type = "other wondrous item", AmountAsDouble = 90210 }
            };

            mockTypeAndAmountPercentileSelector.Setup(s => s.SelectAllFrom(Config.Name, tableName)).Returns(selections);

            var attributes = new[] { "type 1", "type 2" };
            tableName = TableNameConstants.Collections.ITEMTYPEAttributes(ItemTypeConstants.WondrousItem);
            mockCollectionsSelector.Setup(p => p.SelectFrom(Config.Name, tableName, WondrousItemConstants.HornOfValhalla)).Returns(attributes);

            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.WondrousItem, It.IsAny<string>())).Returns(666);

            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(It.IsAny<IEnumerable<TypeAndAmountDataSelection>>()))
                .Returns((IEnumerable<TypeAndAmountDataSelection> c) => c.Last());

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, WondrousItemConstants.HornOfValhalla))
                .Returns(new[] { "wrong power", power, "other power" });

            var item = wondrousItemGenerator.Generate(power, WondrousItemConstants.HornOfValhalla);
            Assert.That(item.Name, Is.EqualTo(WondrousItemConstants.HornOfValhalla));
            Assert.That(item.BaseNames.Single(), Is.EqualTo(WondrousItemConstants.HornOfValhalla));
            Assert.That(item.ItemType, Is.EqualTo(ItemTypeConstants.WondrousItem));
            Assert.That(item.Attributes, Is.EqualTo(attributes));
            Assert.That(item.IsMagical, Is.True);
            Assert.That(item.Magic.Bonus, Is.EqualTo(9266));
            Assert.That(item.Magic.Charges, Is.EqualTo(0));
            Assert.That(item.Traits, Contains.Item("metallic"));
            Assert.That(item.Traits.Count, Is.EqualTo(1));
        }

        [Test]
        public void GenerateHornOfValhallaFromName_WithTraits()
        {
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, TableNameConstants.Percentiles.HornOfValhallaTypes)).Returns("metallic");

            var tableName = TableNameConstants.Percentiles.POWERITEMTYPEs(power, ItemTypeConstants.WondrousItem);
            var selections = new[]
            {
                new TypeAndAmountDataSelection { Type = "wrong wondrous item", AmountAsDouble = 666 },
                new TypeAndAmountDataSelection { Type = WondrousItemConstants.HornOfValhalla, AmountAsDouble = 9266 },
                new TypeAndAmountDataSelection { Type = "other wondrous item", AmountAsDouble = 90210 }
            };

            mockTypeAndAmountPercentileSelector.Setup(s => s.SelectAllFrom(Config.Name, tableName)).Returns(selections);

            var attributes = new[] { "type 1", "type 2" };
            tableName = TableNameConstants.Collections.ITEMTYPEAttributes(ItemTypeConstants.WondrousItem);
            mockCollectionsSelector.Setup(p => p.SelectFrom(Config.Name, tableName, WondrousItemConstants.HornOfValhalla)).Returns(attributes);

            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.WondrousItem, It.IsAny<string>())).Returns(666);

            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(It.IsAny<IEnumerable<TypeAndAmountDataSelection>>()))
                .Returns((IEnumerable<TypeAndAmountDataSelection> c) => c.Last());

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, WondrousItemConstants.HornOfValhalla))
                .Returns(new[] { "wrong power", power, "other power" });

            var item = wondrousItemGenerator.Generate(power, WondrousItemConstants.HornOfValhalla, "ceramic");
            Assert.That(item.Name, Is.EqualTo(WondrousItemConstants.HornOfValhalla));
            Assert.That(item.BaseNames.Single(), Is.EqualTo(WondrousItemConstants.HornOfValhalla));
            Assert.That(item.ItemType, Is.EqualTo(ItemTypeConstants.WondrousItem));
            Assert.That(item.Attributes, Is.EqualTo(attributes));
            Assert.That(item.IsMagical, Is.True);
            Assert.That(item.Magic.Bonus, Is.EqualTo(9266));
            Assert.That(item.Magic.Charges, Is.EqualTo(0));
            Assert.That(item.Traits, Contains.Item("ceramic").And.Count.EqualTo(1));
        }

        [Test]
        public void GenerateIronFlaskFromName()
        {
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, TableNameConstants.Percentiles.IronFlaskContents)).Returns("contents");

            var tableName = TableNameConstants.Percentiles.POWERITEMTYPEs(power, ItemTypeConstants.WondrousItem);
            var selections = new[]
            {
                new TypeAndAmountDataSelection { Type = "wrong wondrous item", AmountAsDouble = 666 },
                new TypeAndAmountDataSelection { Type = WondrousItemConstants.IronFlask, AmountAsDouble = 9266 },
                new TypeAndAmountDataSelection { Type = "other wondrous item", AmountAsDouble = 90210 }
            };

            mockTypeAndAmountPercentileSelector.Setup(s => s.SelectAllFrom(Config.Name, tableName)).Returns(selections);

            var attributes = new[] { "type 1", "type 2" };
            tableName = TableNameConstants.Collections.ITEMTYPEAttributes(ItemTypeConstants.WondrousItem);
            mockCollectionsSelector.Setup(p => p.SelectFrom(Config.Name, tableName, WondrousItemConstants.IronFlask)).Returns(attributes);

            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.WondrousItem, It.IsAny<string>())).Returns(666);

            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(It.IsAny<IEnumerable<TypeAndAmountDataSelection>>()))
                .Returns((IEnumerable<TypeAndAmountDataSelection> c) => c.Last());

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, WondrousItemConstants.IronFlask))
                .Returns(new[] { "wrong power", power, "other power" });

            var item = wondrousItemGenerator.Generate(power, WondrousItemConstants.IronFlask);
            Assert.That(item.Name, Is.EqualTo(WondrousItemConstants.IronFlask));
            Assert.That(item.BaseNames.Single(), Is.EqualTo(WondrousItemConstants.IronFlask));
            Assert.That(item.ItemType, Is.EqualTo(ItemTypeConstants.WondrousItem));
            Assert.That(item.Attributes, Is.EqualTo(attributes));
            Assert.That(item.IsMagical, Is.True);
            Assert.That(item.Magic.Bonus, Is.EqualTo(9266));
            Assert.That(item.Magic.Charges, Is.EqualTo(0));
            Assert.That(item.Contents, Contains.Item("contents"));
        }

        [Test]
        public void GenerateRobeOfUsefulItemsFromName()
        {
            SetUpRoll(4, 4, 2);
            var items = new[] { "item 1", "item 1", "item 2", "item 2", "item 3", "item 3" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.WondrousItemContents, WondrousItemConstants.RobeOfUsefulItems)).Returns(items);
            mockPercentileSelector.SetupSequence(p => p.SelectFrom(Config.Name, TableNameConstants.Percentiles.RobeOfUsefulItemsExtraItems)).Returns("extra item 1").Returns("extra item 2");

            var tableName = TableNameConstants.Percentiles.POWERITEMTYPEs(power, ItemTypeConstants.WondrousItem);
            var selections = new[]
            {
                new TypeAndAmountDataSelection { Type = "wrong wondrous item", AmountAsDouble = 666 },
                new TypeAndAmountDataSelection { Type = WondrousItemConstants.RobeOfUsefulItems, AmountAsDouble = 9266 },
                new TypeAndAmountDataSelection { Type = "other wondrous item", AmountAsDouble = 90210 }
            };

            mockTypeAndAmountPercentileSelector.Setup(s => s.SelectAllFrom(Config.Name, tableName)).Returns(selections);

            var attributes = new[] { "type 1", "type 2" };
            tableName = TableNameConstants.Collections.ITEMTYPEAttributes(ItemTypeConstants.WondrousItem);
            mockCollectionsSelector.Setup(p => p.SelectFrom(Config.Name, tableName, WondrousItemConstants.RobeOfUsefulItems)).Returns(attributes);

            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.WondrousItem, It.IsAny<string>())).Returns(666);

            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(It.IsAny<IEnumerable<TypeAndAmountDataSelection>>()))
                .Returns((IEnumerable<TypeAndAmountDataSelection> c) => c.Last());

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, WondrousItemConstants.RobeOfUsefulItems))
                .Returns(new[] { "wrong power", power, "other power" });

            var item = wondrousItemGenerator.Generate(power, WondrousItemConstants.RobeOfUsefulItems);
            Assert.That(item.Name, Is.EqualTo(WondrousItemConstants.RobeOfUsefulItems));
            Assert.That(item.BaseNames.Single(), Is.EqualTo(WondrousItemConstants.RobeOfUsefulItems));
            Assert.That(item.ItemType, Is.EqualTo(ItemTypeConstants.WondrousItem));
            Assert.That(item.Attributes, Is.EqualTo(attributes));
            Assert.That(item.IsMagical, Is.True);
            Assert.That(item.Magic.Bonus, Is.EqualTo(9266));
            Assert.That(item.Magic.Charges, Is.EqualTo(0));
            Assert.That(item.Contents, Is.SupersetOf(items));
            Assert.That(item.Contents, Contains.Item("extra item 1"));
            Assert.That(item.Contents, Contains.Item("extra item 2"));
        }

        [Test]
        public void GenerateCubicGateFromName()
        {
            mockPercentileSelector.SetupSequence(p => p.SelectFrom(Config.Name, TableNameConstants.Percentiles.Planes))
                .Returns("plane 1")
                .Returns("plane 2")
                .Returns("plane 3")
                .Returns("plane 4")
                .Returns("plane 5");

            var tableName = TableNameConstants.Percentiles.POWERITEMTYPEs(power, ItemTypeConstants.WondrousItem);
            var selections = new[]
            {
                new TypeAndAmountDataSelection { Type = "wrong wondrous item", AmountAsDouble = 666 },
                new TypeAndAmountDataSelection { Type = WondrousItemConstants.CubicGate, AmountAsDouble = 9266 },
                new TypeAndAmountDataSelection { Type = "other wondrous item", AmountAsDouble = 90210 }
            };

            mockTypeAndAmountPercentileSelector.Setup(s => s.SelectAllFrom(Config.Name, tableName)).Returns(selections);

            var attributes = new[] { "type 1", "type 2" };
            tableName = TableNameConstants.Collections.ITEMTYPEAttributes(ItemTypeConstants.WondrousItem);
            mockCollectionsSelector.Setup(p => p.SelectFrom(Config.Name, tableName, WondrousItemConstants.CubicGate)).Returns(attributes);

            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.WondrousItem, It.IsAny<string>())).Returns(666);

            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(It.IsAny<IEnumerable<TypeAndAmountDataSelection>>()))
                .Returns((IEnumerable<TypeAndAmountDataSelection> c) => c.Last());

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, WondrousItemConstants.CubicGate))
                .Returns(new[] { "wrong power", power, "other power" });

            var item = wondrousItemGenerator.Generate(power, WondrousItemConstants.CubicGate);
            Assert.That(item.Name, Is.EqualTo(WondrousItemConstants.CubicGate));
            Assert.That(item.BaseNames.Single(), Is.EqualTo(WondrousItemConstants.CubicGate));
            Assert.That(item.ItemType, Is.EqualTo(ItemTypeConstants.WondrousItem));
            Assert.That(item.Attributes, Is.EqualTo(attributes));
            Assert.That(item.IsMagical, Is.True);
            Assert.That(item.Magic.Bonus, Is.EqualTo(9266));
            Assert.That(item.Magic.Charges, Is.EqualTo(0));
            Assert.That(item.Contents, Contains.Item("Material Plane"));
            Assert.That(item.Contents, Contains.Item("plane 1"));
            Assert.That(item.Contents, Contains.Item("plane 2"));
            Assert.That(item.Contents, Contains.Item("plane 3"));
            Assert.That(item.Contents, Contains.Item("plane 4"));
            Assert.That(item.Contents, Contains.Item("plane 5"));
            Assert.That(item.Contents.Count, Is.EqualTo(6));
        }

        [Test]
        public void GenerateDeckOfIllusionsFromName()
        {
            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.WondrousItem, WondrousItemConstants.DeckOfIllusions)).Returns(3);
            var cards = new[] { "card 1", "card 2", "card 3", "card 4" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.WondrousItemContents, WondrousItemConstants.DeckOfIllusions)).Returns(cards);
            SetUpRandomSelections(cards, 0, 1, 3);

            var tableName = TableNameConstants.Percentiles.POWERITEMTYPEs(power, ItemTypeConstants.WondrousItem);
            var selections = new[]
            {
                new TypeAndAmountDataSelection { Type = "wrong wondrous item", AmountAsDouble = 666 },
                new TypeAndAmountDataSelection { Type = WondrousItemConstants.DeckOfIllusions, AmountAsDouble = 9266 },
                new TypeAndAmountDataSelection { Type = "other wondrous item", AmountAsDouble = 90210 }
            };

            mockTypeAndAmountPercentileSelector.Setup(s => s.SelectAllFrom(Config.Name, tableName)).Returns(selections);

            var attributes = new[] { "type 1", "type 2" };
            tableName = TableNameConstants.Collections.ITEMTYPEAttributes(ItemTypeConstants.WondrousItem);
            mockCollectionsSelector.Setup(p => p.SelectFrom(Config.Name, tableName, WondrousItemConstants.DeckOfIllusions)).Returns(attributes);

            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(It.IsAny<IEnumerable<TypeAndAmountDataSelection>>()))
                .Returns((IEnumerable<TypeAndAmountDataSelection> c) => c.Last());

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, WondrousItemConstants.DeckOfIllusions))
                .Returns(new[] { "wrong power", power, "other power" });

            var item = wondrousItemGenerator.Generate(power, WondrousItemConstants.DeckOfIllusions);
            Assert.That(item.Name, Is.EqualTo(WondrousItemConstants.DeckOfIllusions));
            Assert.That(item.BaseNames.Single(), Is.EqualTo(WondrousItemConstants.DeckOfIllusions));
            Assert.That(item.ItemType, Is.EqualTo(ItemTypeConstants.WondrousItem));
            Assert.That(item.Attributes, Is.EqualTo(attributes));
            Assert.That(item.IsMagical, Is.True);
            Assert.That(item.Magic.Bonus, Is.EqualTo(9266));
            Assert.That(item.Magic.Charges, Is.EqualTo(0));
            Assert.That(item.Contents, Contains.Item("card 1"));
            Assert.That(item.Contents, Contains.Item("card 2"));
            Assert.That(item.Contents, Contains.Item("card 4"));
            Assert.That(item.Contents.Count, Is.EqualTo(3));
        }

        private void SetUpRandomSelections(string[] collection, params int[] selectedIndices)
        {
            for (var i = 0; i < selectedIndices.Length; i++)
            {
                var index = selectedIndices[i];
                var skipAmount = i;

                mockCollectionsSelector
                    .Setup(s =>
                        s.SelectRandomFrom(It.Is<IEnumerable<string>>(c =>
                            c.Count() == collection.Length - skipAmount))
                    ).Returns(collection[index]);
            }
        }

        [Test]
        public void GenerateNecklaceOfFireballsFromName()
        {
            var name = WondrousItemConstants.NecklaceOfFireballs + " type whatever";
            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.WondrousItem, name)).Returns(4);
            var spheres = new[] { "small sphere", "big sphere", "normal sphere", "normal sphere", "big sphere" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.WondrousItemContents, name)).Returns(spheres);
            SetUpRandomSelections(spheres, 0, 1, 2, 4);

            var tableName = TableNameConstants.Percentiles.POWERITEMTYPEs(power, ItemTypeConstants.WondrousItem);
            var selections = new[]
            {
                new TypeAndAmountDataSelection { Type = "wrong wondrous item", AmountAsDouble = 666 },
                new TypeAndAmountDataSelection { Type = name, AmountAsDouble = 9266 },
                new TypeAndAmountDataSelection { Type = "other wondrous item", AmountAsDouble = 90210 }
            };

            mockTypeAndAmountPercentileSelector.Setup(s => s.SelectAllFrom(Config.Name, tableName)).Returns(selections);

            var attributes = new[] { "type 1", "type 2" };
            tableName = TableNameConstants.Collections.ITEMTYPEAttributes(ItemTypeConstants.WondrousItem);
            mockCollectionsSelector.Setup(p => p.SelectFrom(Config.Name, tableName, name)).Returns(attributes);

            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(It.IsAny<IEnumerable<TypeAndAmountDataSelection>>()))
                .Returns((IEnumerable<TypeAndAmountDataSelection> c) => c.Last());

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, name))
                .Returns(new[] { "wrong power", power, "other power" });

            var item = wondrousItemGenerator.Generate(power, name);
            Assert.That(item.Name, Is.EqualTo(name));
            Assert.That(item.BaseNames.Single(), Is.EqualTo(name));
            Assert.That(item.ItemType, Is.EqualTo(ItemTypeConstants.WondrousItem));
            Assert.That(item.Attributes, Is.EqualTo(attributes));
            Assert.That(item.IsMagical, Is.True);
            Assert.That(item.Magic.Bonus, Is.EqualTo(9266));
            Assert.That(item.Magic.Charges, Is.EqualTo(0));
            Assert.That(item.Contents, Contains.Item("small sphere"));
            Assert.That(item.Contents, Contains.Item("normal sphere"));
            Assert.That(item.Contents, Contains.Item("big sphere"));
            Assert.That(item.Contents.Count(c => c == "big sphere"), Is.EqualTo(2));
            Assert.That(item.Contents.Count, Is.EqualTo(4));
        }

        [Test]
        public void GenerateRobeOfBonesFromName()
        {
            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.WondrousItem, WondrousItemConstants.RobeOfBones)).Returns(4);
            var items = new[] { "undead 1", "undead 1", "undead 2", "undead 2", "undead 3", "undead 3" };
            mockCollectionsSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.WondrousItemContents, WondrousItemConstants.RobeOfBones)).Returns(items);
            SetUpRandomSelections(items, 0, 1, 2, 4);

            var tableName = TableNameConstants.Percentiles.POWERITEMTYPEs(power, ItemTypeConstants.WondrousItem);
            var selections = new[]
            {
                new TypeAndAmountDataSelection { Type = "wrong wondrous item", AmountAsDouble = 666 },
                new TypeAndAmountDataSelection { Type = WondrousItemConstants.RobeOfBones, AmountAsDouble = 9266 },
                new TypeAndAmountDataSelection { Type = "other wondrous item", AmountAsDouble = 90210 }
            };

            mockTypeAndAmountPercentileSelector.Setup(s => s.SelectAllFrom(Config.Name, tableName)).Returns(selections);

            var attributes = new[] { "type 1", "type 2" };
            tableName = TableNameConstants.Collections.ITEMTYPEAttributes(ItemTypeConstants.WondrousItem);
            mockCollectionsSelector.Setup(p => p.SelectFrom(Config.Name, tableName, WondrousItemConstants.RobeOfBones)).Returns(attributes);

            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(It.IsAny<IEnumerable<TypeAndAmountDataSelection>>()))
                .Returns((IEnumerable<TypeAndAmountDataSelection> c) => c.Last());

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, WondrousItemConstants.RobeOfBones))
                .Returns(new[] { "wrong power", power, "other power" });

            var item = wondrousItemGenerator.Generate(power, WondrousItemConstants.RobeOfBones);
            Assert.That(item.Name, Is.EqualTo(WondrousItemConstants.RobeOfBones));
            Assert.That(item.BaseNames.Single(), Is.EqualTo(WondrousItemConstants.RobeOfBones));
            Assert.That(item.ItemType, Is.EqualTo(ItemTypeConstants.WondrousItem));
            Assert.That(item.Attributes, Is.EqualTo(attributes));
            Assert.That(item.IsMagical, Is.True);
            Assert.That(item.Magic.Bonus, Is.EqualTo(9266));
            Assert.That(item.Magic.Charges, Is.EqualTo(0));
            Assert.That(item.Contents, Contains.Item("undead 1"));
            Assert.That(item.Contents.Count(c => c == "undead 1"), Is.EqualTo(2));
            Assert.That(item.Contents, Contains.Item("undead 2"));
            Assert.That(item.Contents, Contains.Item("undead 3"));
            Assert.That(item.Contents.Count, Is.EqualTo(4));
        }

        [Test]
        public void GenerateCandleOfInvocationFromName()
        {
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.IntelligenceAlignments)).Returns("alignment");

            var tableName = TableNameConstants.Percentiles.POWERITEMTYPEs(power, ItemTypeConstants.WondrousItem);
            var selections = new[]
            {
                new TypeAndAmountDataSelection { Type = "wrong wondrous item", AmountAsDouble = 666 },
                new TypeAndAmountDataSelection { Type = WondrousItemConstants.CandleOfInvocation, AmountAsDouble = 9266 },
                new TypeAndAmountDataSelection { Type = "other wondrous item", AmountAsDouble = 90210 }
            };

            mockTypeAndAmountPercentileSelector.Setup(s => s.SelectAllFrom(Config.Name, tableName)).Returns(selections);

            var attributes = new[] { "type 1", "type 2" };
            tableName = TableNameConstants.Collections.ITEMTYPEAttributes(ItemTypeConstants.WondrousItem);
            mockCollectionsSelector.Setup(p => p.SelectFrom(Config.Name, tableName, WondrousItemConstants.CandleOfInvocation)).Returns(attributes);

            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.WondrousItem, It.IsAny<string>())).Returns(666);

            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(It.IsAny<IEnumerable<TypeAndAmountDataSelection>>()))
                .Returns((IEnumerable<TypeAndAmountDataSelection> c) => c.Last());

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, WondrousItemConstants.CandleOfInvocation))
                .Returns(new[] { "wrong power", power, "other power" });

            var item = wondrousItemGenerator.Generate(power, WondrousItemConstants.CandleOfInvocation);
            Assert.That(item.Name, Is.EqualTo(WondrousItemConstants.CandleOfInvocation));
            Assert.That(item.BaseNames.Single(), Is.EqualTo(WondrousItemConstants.CandleOfInvocation));
            Assert.That(item.ItemType, Is.EqualTo(ItemTypeConstants.WondrousItem));
            Assert.That(item.Attributes, Is.EqualTo(attributes));
            Assert.That(item.IsMagical, Is.True);
            Assert.That(item.Magic.Bonus, Is.EqualTo(9266));
            Assert.That(item.Magic.Charges, Is.EqualTo(0));
            Assert.That(item.Traits, Contains.Item("alignment"));
            Assert.That(item.Traits.Count, Is.EqualTo(1));
        }

        [Test]
        public void GenerateCandleOfInvocationFromName_WithTraits()
        {
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.IntelligenceAlignments)).Returns("alignment");

            var tableName = TableNameConstants.Percentiles.POWERITEMTYPEs(power, ItemTypeConstants.WondrousItem);
            var selections = new[]
            {
                new TypeAndAmountDataSelection { Type = "wrong wondrous item", AmountAsDouble = 666 },
                new TypeAndAmountDataSelection { Type = WondrousItemConstants.CandleOfInvocation, AmountAsDouble = 9266 },
                new TypeAndAmountDataSelection { Type = "other wondrous item", AmountAsDouble = 90210 }
            };

            mockTypeAndAmountPercentileSelector.Setup(s => s.SelectAllFrom(Config.Name, tableName)).Returns(selections);

            var attributes = new[] { "type 1", "type 2" };
            tableName = TableNameConstants.Collections.ITEMTYPEAttributes(ItemTypeConstants.WondrousItem);
            mockCollectionsSelector.Setup(p => p.SelectFrom(Config.Name, tableName, WondrousItemConstants.CandleOfInvocation)).Returns(attributes);

            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.WondrousItem, It.IsAny<string>())).Returns(666);

            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(It.IsAny<IEnumerable<TypeAndAmountDataSelection>>()))
                .Returns((IEnumerable<TypeAndAmountDataSelection> c) => c.Last());

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, WondrousItemConstants.CandleOfInvocation))
                .Returns(new[] { "wrong power", power, "other power" });

            var item = wondrousItemGenerator.Generate(power, WondrousItemConstants.CandleOfInvocation, "goody two-shoes");
            Assert.That(item.Name, Is.EqualTo(WondrousItemConstants.CandleOfInvocation));
            Assert.That(item.BaseNames.Single(), Is.EqualTo(WondrousItemConstants.CandleOfInvocation));
            Assert.That(item.ItemType, Is.EqualTo(ItemTypeConstants.WondrousItem));
            Assert.That(item.Attributes, Is.EqualTo(attributes));
            Assert.That(item.IsMagical, Is.True);
            Assert.That(item.Magic.Bonus, Is.EqualTo(9266));
            Assert.That(item.Magic.Charges, Is.EqualTo(0));
            Assert.That(item.Traits, Contains.Item("goody two-shoes").And.Count.EqualTo(1));
        }

        [Test]
        public void GenerateRobeOfTheArchmagiFromName()
        {
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.RobeOfTheArchmagiColors)).Returns("color (alignment)");

            var tableName = TableNameConstants.Percentiles.POWERITEMTYPEs(power, ItemTypeConstants.WondrousItem);
            var selections = new[]
            {
                new TypeAndAmountDataSelection { Type = "wrong wondrous item", AmountAsDouble = 666 },
                new TypeAndAmountDataSelection { Type = WondrousItemConstants.RobeOfTheArchmagi, AmountAsDouble = 9266 },
                new TypeAndAmountDataSelection { Type = "other wondrous item", AmountAsDouble = 90210 }
            };

            mockTypeAndAmountPercentileSelector.Setup(s => s.SelectAllFrom(Config.Name, tableName)).Returns(selections);

            var attributes = new[] { "type 1", "type 2" };
            tableName = TableNameConstants.Collections.ITEMTYPEAttributes(ItemTypeConstants.WondrousItem);
            mockCollectionsSelector.Setup(p => p.SelectFrom(Config.Name, tableName, WondrousItemConstants.RobeOfTheArchmagi)).Returns(attributes);

            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.WondrousItem, It.IsAny<string>())).Returns(666);

            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(It.IsAny<IEnumerable<TypeAndAmountDataSelection>>()))
                .Returns((IEnumerable<TypeAndAmountDataSelection> c) => c.Last());

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, WondrousItemConstants.RobeOfTheArchmagi))
                .Returns(new[] { "wrong power", power, "other power" });

            var item = wondrousItemGenerator.Generate(power, WondrousItemConstants.RobeOfTheArchmagi);
            Assert.That(item.Name, Is.EqualTo(WondrousItemConstants.RobeOfTheArchmagi));
            Assert.That(item.BaseNames.Single(), Is.EqualTo(WondrousItemConstants.RobeOfTheArchmagi));
            Assert.That(item.ItemType, Is.EqualTo(ItemTypeConstants.WondrousItem));
            Assert.That(item.Attributes, Is.EqualTo(attributes));
            Assert.That(item.IsMagical, Is.True);
            Assert.That(item.Magic.Bonus, Is.EqualTo(9266));
            Assert.That(item.Magic.Charges, Is.EqualTo(0));
            Assert.That(item.Traits, Contains.Item("color (alignment)"));
            Assert.That(item.Traits.Count, Is.EqualTo(1));
        }

        [Test]
        public void GenerateRobeOfTheArchmagiFromName_WithTraits()
        {
            mockPercentileSelector.Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Percentiles.RobeOfTheArchmagiColors)).Returns("color (alignment)");

            var tableName = TableNameConstants.Percentiles.POWERITEMTYPEs(power, ItemTypeConstants.WondrousItem);
            var selections = new[]
            {
                new TypeAndAmountDataSelection { Type = "wrong wondrous item", AmountAsDouble = 666 },
                new TypeAndAmountDataSelection { Type = WondrousItemConstants.RobeOfTheArchmagi, AmountAsDouble = 9266 },
                new TypeAndAmountDataSelection { Type = "other wondrous item", AmountAsDouble = 90210 }
            };

            mockTypeAndAmountPercentileSelector.Setup(s => s.SelectAllFrom(Config.Name, tableName)).Returns(selections);

            var attributes = new[] { "type 1", "type 2" };
            tableName = TableNameConstants.Collections.ITEMTYPEAttributes(ItemTypeConstants.WondrousItem);
            mockCollectionsSelector.Setup(p => p.SelectFrom(Config.Name, tableName, WondrousItemConstants.RobeOfTheArchmagi)).Returns(attributes);

            mockChargesGenerator.Setup(g => g.GenerateFor(ItemTypeConstants.WondrousItem, It.IsAny<string>())).Returns(666);

            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(It.IsAny<IEnumerable<TypeAndAmountDataSelection>>()))
                .Returns((IEnumerable<TypeAndAmountDataSelection> c) => c.Last());

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.PowerGroups, WondrousItemConstants.RobeOfTheArchmagi))
                .Returns(new[] { "wrong power", power, "other power" });

            var item = wondrousItemGenerator.Generate(power, WondrousItemConstants.RobeOfTheArchmagi, "plaid (superbad)");
            Assert.That(item.Name, Is.EqualTo(WondrousItemConstants.RobeOfTheArchmagi));
            Assert.That(item.BaseNames.Single(), Is.EqualTo(WondrousItemConstants.RobeOfTheArchmagi));
            Assert.That(item.ItemType, Is.EqualTo(ItemTypeConstants.WondrousItem));
            Assert.That(item.Attributes, Is.EqualTo(attributes));
            Assert.That(item.IsMagical, Is.True);
            Assert.That(item.Magic.Bonus, Is.EqualTo(9266));
            Assert.That(item.Magic.Charges, Is.EqualTo(0));
            Assert.That(item.Traits, Contains.Item("plaid (superbad)").And.Count.EqualTo(1));
        }
    }
}