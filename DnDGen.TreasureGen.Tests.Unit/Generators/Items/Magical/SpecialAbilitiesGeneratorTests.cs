using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.TreasureGen.Generators.Items.Magical;
using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Selectors.Percentiles;
using DnDGen.TreasureGen.Selectors.Selections;
using DnDGen.TreasureGen.Tables;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.TreasureGen.Tests.Unit.Generators.Items.Magical
{
    [TestFixture]
    public class SpecialAbilitiesGeneratorTests
    {
        private ISpecialAbilitiesGenerator specialAbilitiesGenerator;
        private Mock<ICollectionSelector> mockCollectionsSelector;
        private Mock<ITreasurePercentileSelector> mockPercentileSelector;
        private Mock<ICollectionDataSelector<SpecialAbilityDataSelection>> mockSpecialAbilityDataSelector;
        private Mock<ICollectionDataSelector<DamageDataSelection>> mockDamageDataSelector;
        private Mock<ISpellGenerator> mockSpellGenerator;

        private List<string> itemAttributes;
        private List<string> names;
        private string power;
        private Item item;

        [SetUp]
        public void Setup()
        {
            mockCollectionsSelector = new Mock<ICollectionSelector>();
            mockPercentileSelector = new Mock<ITreasurePercentileSelector>();
            mockSpecialAbilityDataSelector = new Mock<ICollectionDataSelector<SpecialAbilityDataSelection>>();
            mockDamageDataSelector = new Mock<ICollectionDataSelector<DamageDataSelection>>();
            mockSpellGenerator = new Mock<ISpellGenerator>();
            specialAbilitiesGenerator = new SpecialAbilitiesGenerator(
                mockCollectionsSelector.Object,
                mockPercentileSelector.Object,
                mockSpecialAbilityDataSelector.Object,
                mockDamageDataSelector.Object,
                mockSpellGenerator.Object);

            itemAttributes = [];
            names = [];
            power = "power";

            item = new Item();
            item.ItemType = ItemTypeConstants.Armor;
            item.Attributes = itemAttributes;
            item.Magic.Bonus = 1;

            mockPercentileSelector.Setup(p => p.SelectAllFrom(Config.Name, It.IsAny<string>())).Returns(names);

            var count = 0;

            mockCollectionsSelector
                .Setup(s => s.SelectRandomFrom(It.IsAny<IEnumerable<string>>()))
                .Returns((IEnumerable<string> ss) => ss.ElementAt(count++ % ss.Count()));

            mockDamageDataSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.AbilityDamages, It.IsAny<string>()))
                .Returns([]);
        }

        [Test]
        public void GenerateFor_ReturnEmptyIfBonusLessThanOne()
        {
            item.Magic.Bonus = 0;
            CreateSpecialAbility("name", "base name", 9, 266);

            var abilities = specialAbilitiesGenerator.GenerateFor(item, power, 1);
            Assert.That(abilities, Is.Empty);
            mockPercentileSelector.Verify(s => s.SelectAllFrom(Config.Name, It.IsAny<string>()), Times.Never);
            mockPercentileSelector.Verify(s => s.SelectFrom(Config.Name, It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void GenerateFor_ReturnEmptyIfQuantityIsNone()
        {
            CreateSpecialAbility("name", "base name", 9, 266);

            var abilities = specialAbilitiesGenerator.GenerateFor(item, power, 0);
            Assert.That(abilities, Is.Empty);
            mockPercentileSelector.Verify(s => s.SelectAllFrom(Config.Name, It.IsAny<string>()), Times.Never);
            mockPercentileSelector.Verify(s => s.SelectFrom(Config.Name, It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void GenerateFor_GetShieldAbilityIfShield()
        {
            itemAttributes.Add(AttributeConstants.Shield);
            CreateSpecialAbility("name", "base name", 9, 266);

            var abilities = specialAbilitiesGenerator.GenerateFor(item, power, 1);
            Assert.That(abilities, Is.Not.Empty);

            var tableName = TableNameConstants.Percentiles.POWERATTRIBUTESpecialAbilities(power, AttributeConstants.Shield);
            mockPercentileSelector.Verify(s => s.SelectAllFrom(Config.Name, tableName), Times.Once);
        }

        [Test]
        public void GenerateFor_GetMeleeAbilityIfMelee()
        {
            item.ItemType = ItemTypeConstants.Weapon;
            itemAttributes.Add(AttributeConstants.Melee);
            CreateSpecialAbility("name", "base name", 9, 266);

            var abilities = specialAbilitiesGenerator.GenerateFor(item, power, 1);
            Assert.That(abilities, Is.Not.Empty);

            var tableName = TableNameConstants.Percentiles.POWERATTRIBUTESpecialAbilities(power, AttributeConstants.Melee);
            mockPercentileSelector.Verify(s => s.SelectAllFrom(Config.Name, tableName), Times.Once);
        }

        [Test]
        public void GenerateFor_GetRangedAbilityIfRanged()
        {
            item.ItemType = ItemTypeConstants.Weapon;
            itemAttributes.Add(AttributeConstants.Ranged);
            CreateSpecialAbility("name", "base name", 9, 266);

            var abilities = specialAbilitiesGenerator.GenerateFor(item, power, 1);
            Assert.That(abilities, Is.Not.Empty);

            var tableName = TableNameConstants.Percentiles.POWERATTRIBUTESpecialAbilities(power, AttributeConstants.Ranged);
            mockPercentileSelector.Verify(s => s.SelectAllFrom(Config.Name, tableName), Times.Once);
        }

        [Test]
        public void GenerateFor_GetArmorAbilityIfArmor()
        {
            CreateSpecialAbility("name", "base name", 9, 266);

            var abilities = specialAbilitiesGenerator.GenerateFor(item, power, 1);
            Assert.That(abilities, Is.Not.Empty);

            var tableName = TableNameConstants.Percentiles.POWERATTRIBUTESpecialAbilities(power, ItemTypeConstants.Armor);
            mockPercentileSelector.Verify(s => s.SelectAllFrom(Config.Name, tableName), Times.Once);
        }

        [Test]
        public void GenerateFor_ReturnEmptyIfNoMatchingTableNames()
        {
            item.ItemType = "wrong item type";
            var abilities = specialAbilitiesGenerator.GenerateFor(item, power, 1);
            Assert.That(abilities, Is.Empty);
        }

        [Test]
        public void GenerateFor_SetAbilityByResult()
        {
            CreateSpecialAbility("name", "base name", 9, 266);
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, It.IsAny<string>())).Returns("name");

            var abilities = specialAbilitiesGenerator.GenerateFor(item, power, 1);
            Assert.That(abilities, Is.Not.Empty);

            var ability = abilities.First();
            Assert.That(ability.AttributeRequirements, Is.EqualTo(itemAttributes));
            Assert.That(ability.Name, Is.EqualTo("name"));
            Assert.That(ability.BaseName, Is.EqualTo("base name"));
            Assert.That(ability.Power, Is.EqualTo(266));
            Assert.That(ability.BonusEquivalent, Is.EqualTo(9));
            Assert.That(ability.Damages, Is.Empty);
            Assert.That(ability.CriticalDamages, Is.Empty);
            Assert.That(abilities.Count(), Is.EqualTo(1));
        }

        [Test]
        public void GenerateFor_GetAbilities()
        {
            CreateSpecialAbility("ability 1");
            CreateSpecialAbility("ability 2");
            CreateSpecialAbility("ability 3");
            mockPercentileSelector.SetupSequence(p => p.SelectFrom(Config.Name, It.IsAny<string>()))
                .Returns("ability 1")
                .Returns("ability 2")
                .Returns("ability 3");

            var abilities = specialAbilitiesGenerator.GenerateFor(item, power, 3);
            Assert.That(abilities, Is.Not.Empty);

            var names = abilities.Select(a => a.Name);
            Assert.That(names, Contains.Item("ability 1"));
            Assert.That(names, Contains.Item("ability 2"));
            Assert.That(names, Contains.Item("ability 3"));
            Assert.That(names.Count(), Is.EqualTo(3));
        }

        [Test]
        public void GenerateFor_SetDamage()
        {
            CreateSpecialAbility("name", "base name", 9, 266);
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, It.IsAny<string>())).Returns("name");

            mockDamageDataSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.AbilityDamages, "name"))
                .Returns([new DamageDataSelection { Type = "my damage type", Roll = "my roll" }]);

            var abilities = specialAbilitiesGenerator.GenerateFor(item, power, 1);
            Assert.That(abilities, Is.Not.Empty);

            var ability = abilities.First();
            Assert.That(ability.AttributeRequirements, Is.EqualTo(itemAttributes));
            Assert.That(ability.Name, Is.EqualTo("name"));
            Assert.That(ability.BaseName, Is.EqualTo("base name"));
            Assert.That(ability.Power, Is.EqualTo(266));
            Assert.That(ability.BonusEquivalent, Is.EqualTo(9));
            Assert.That(ability.Damages, Has.Count.EqualTo(1));
            Assert.That(ability.Damages[0].Roll, Is.EqualTo("my roll"));
            Assert.That(ability.Damages[0].Type, Is.EqualTo("my damage type"));
            Assert.That(ability.Damages[0].Condition, Is.Empty);
            Assert.That(ability.CriticalDamages, Is.Empty);
            Assert.That(abilities.Count(), Is.EqualTo(1));
        }

        [Test]
        public void GenerateFor_SetDamages()
        {
            CreateSpecialAbility("name", "base name", 9, 266);
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, It.IsAny<string>())).Returns("name");

            mockDamageDataSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.AbilityDamages, "name"))
                .Returns(
                [
                    new DamageDataSelection { Type = "my damage type", Roll = "my roll" },
                    new DamageDataSelection { Type = "my other damage type", Roll = "my other roll" },
                ]);

            var abilities = specialAbilitiesGenerator.GenerateFor(item, power, 1);
            Assert.That(abilities, Is.Not.Empty);

            var ability = abilities.First();
            Assert.That(ability.AttributeRequirements, Is.EqualTo(itemAttributes));
            Assert.That(ability.Name, Is.EqualTo("name"));
            Assert.That(ability.BaseName, Is.EqualTo("base name"));
            Assert.That(ability.Power, Is.EqualTo(266));
            Assert.That(ability.BonusEquivalent, Is.EqualTo(9));
            Assert.That(ability.Damages, Has.Count.EqualTo(2));
            Assert.That(ability.Damages[0].Roll, Is.EqualTo("my roll"));
            Assert.That(ability.Damages[0].Type, Is.EqualTo("my damage type"));
            Assert.That(ability.Damages[0].Condition, Is.Empty);
            Assert.That(ability.Damages[1].Roll, Is.EqualTo("my other roll"));
            Assert.That(ability.Damages[1].Type, Is.EqualTo("my other damage type"));
            Assert.That(ability.Damages[1].Condition, Is.Empty);
            Assert.That(ability.CriticalDamages, Is.Empty);
            Assert.That(abilities.Count(), Is.EqualTo(1));
        }

        [Test]
        public void GenerateFor_SetDamage_WithCondition()
        {
            CreateSpecialAbility("name", "base name", 9, 266);
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, It.IsAny<string>())).Returns("name");

            mockDamageDataSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.AbilityDamages, "name"))
                .Returns([new DamageDataSelection { Type = "my damage type", Roll = "my roll", Condition = "my condition" }]);

            var abilities = specialAbilitiesGenerator.GenerateFor(item, power, 1);
            Assert.That(abilities, Is.Not.Empty);

            var ability = abilities.First();
            Assert.That(ability.AttributeRequirements, Is.EqualTo(itemAttributes));
            Assert.That(ability.Name, Is.EqualTo("name"));
            Assert.That(ability.BaseName, Is.EqualTo("base name"));
            Assert.That(ability.Power, Is.EqualTo(266));
            Assert.That(ability.BonusEquivalent, Is.EqualTo(9));
            Assert.That(ability.Damages, Has.Count.EqualTo(1));
            Assert.That(ability.Damages[0].Roll, Is.EqualTo("my roll"));
            Assert.That(ability.Damages[0].Type, Is.EqualTo("my damage type"));
            Assert.That(ability.Damages[0].Condition, Is.EqualTo("my condition"));
            Assert.That(ability.CriticalDamages, Is.Empty);
            Assert.That(abilities.Count(), Is.EqualTo(1));
        }

        [Test]
        public void GenerateFor_SetDamages_WithCondition()
        {
            CreateSpecialAbility("name", "base name", 9, 266);
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, It.IsAny<string>())).Returns("name");

            mockDamageDataSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.AbilityDamages, "name"))
                .Returns(
                [
                    new DamageDataSelection { Type = "my damage type", Roll = "my roll", Condition = "my condition" },
                    new DamageDataSelection { Type = "my other damage type", Roll = "my other roll", Condition = "my other condition" },
                ]);

            var abilities = specialAbilitiesGenerator.GenerateFor(item, power, 1);
            Assert.That(abilities, Is.Not.Empty);

            var ability = abilities.First();
            Assert.That(ability.AttributeRequirements, Is.EqualTo(itemAttributes));
            Assert.That(ability.Name, Is.EqualTo("name"));
            Assert.That(ability.BaseName, Is.EqualTo("base name"));
            Assert.That(ability.Power, Is.EqualTo(266));
            Assert.That(ability.BonusEquivalent, Is.EqualTo(9));
            Assert.That(ability.Damages, Has.Count.EqualTo(2));
            Assert.That(ability.Damages[0].Roll, Is.EqualTo("my roll"));
            Assert.That(ability.Damages[0].Type, Is.EqualTo("my damage type"));
            Assert.That(ability.Damages[0].Condition, Is.EqualTo("my condition"));
            Assert.That(ability.Damages[1].Roll, Is.EqualTo("my other roll"));
            Assert.That(ability.Damages[1].Type, Is.EqualTo("my other damage type"));
            Assert.That(ability.Damages[1].Condition, Is.EqualTo("my other condition"));
            Assert.That(ability.CriticalDamages, Is.Empty);
            Assert.That(abilities.Count(), Is.EqualTo(1));
        }

        [Test]
        public void GenerateFor_SetCriticalDamageByMultiplier()
        {
            CreateSpecialAbility("name", "base name", 9, 266);
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, It.IsAny<string>())).Returns("name");

            mockDamageDataSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.AbilityDamages, "name"))
                .Returns([]);

            mockDamageDataSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.AbilityDamages, "namex90210"))
                .Returns([new DamageDataSelection { Type = "my damage type", Roll = "my roll" }]);

            var weapon = new Weapon
            {
                ItemType = ItemTypeConstants.Weapon,
                Name = "my weapon",
                CriticalMultiplier = "x90210",
                Attributes = [AttributeConstants.Melee]
            };
            weapon.Magic.Bonus = 1;

            var abilities = specialAbilitiesGenerator.GenerateFor(weapon, power, 1);
            Assert.That(abilities, Is.Not.Empty);

            var ability = abilities.First();
            Assert.That(ability.AttributeRequirements, Is.EqualTo(itemAttributes));
            Assert.That(ability.Name, Is.EqualTo("name"));
            Assert.That(ability.BaseName, Is.EqualTo("base name"));
            Assert.That(ability.Power, Is.EqualTo(266));
            Assert.That(ability.BonusEquivalent, Is.EqualTo(9));
            Assert.That(ability.Damages, Is.Empty);
            Assert.That(ability.CriticalDamages, Has.Count.EqualTo(1));
            Assert.That(ability.CriticalDamages[0].Roll, Is.EqualTo("my roll"));
            Assert.That(ability.CriticalDamages[0].Type, Is.EqualTo("my damage type"));
            Assert.That(ability.CriticalDamages[0].Condition, Is.Empty);
            Assert.That(abilities.Count(), Is.EqualTo(1));
        }

        [Test]
        public void GenerateFor_SetCriticalDamagesByMultiplier()
        {
            CreateSpecialAbility("name", "base name", 9, 266);
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, It.IsAny<string>())).Returns("name");

            mockDamageDataSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.AbilityDamages, "name"))
                .Returns([]);

            mockDamageDataSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.AbilityDamages, "namex90210"))
                .Returns(
                [
                    new DamageDataSelection { Type = "my damage type", Roll = "my roll" },
                    new DamageDataSelection { Type = "my damage type 2", Roll = "my roll 2" },
                ]);

            var weapon = new Weapon
            {
                ItemType = ItemTypeConstants.Weapon,
                Name = "my weapon",
                CriticalMultiplier = "x90210",
                Attributes = [AttributeConstants.Melee]
            };
            weapon.Magic.Bonus = 1;

            var abilities = specialAbilitiesGenerator.GenerateFor(weapon, power, 1);
            Assert.That(abilities, Is.Not.Empty);

            var ability = abilities.First();
            Assert.That(ability.AttributeRequirements, Is.EqualTo(itemAttributes));
            Assert.That(ability.Name, Is.EqualTo("name"));
            Assert.That(ability.BaseName, Is.EqualTo("base name"));
            Assert.That(ability.Power, Is.EqualTo(266));
            Assert.That(ability.BonusEquivalent, Is.EqualTo(9));
            Assert.That(ability.Damages, Is.Empty);
            Assert.That(ability.CriticalDamages, Has.Count.EqualTo(2));
            Assert.That(ability.CriticalDamages[0].Roll, Is.EqualTo("my roll"));
            Assert.That(ability.CriticalDamages[0].Type, Is.EqualTo("my damage type"));
            Assert.That(ability.CriticalDamages[0].Condition, Is.Empty);
            Assert.That(ability.CriticalDamages[1].Roll, Is.EqualTo("my roll 2"));
            Assert.That(ability.CriticalDamages[1].Type, Is.EqualTo("my damage type 2"));
            Assert.That(ability.CriticalDamages[1].Condition, Is.Empty);
            Assert.That(abilities.Count(), Is.EqualTo(1));
        }

        [Test]
        public void GenerateFor_SetCriticalDamageByMultiplier_WithCondition()
        {
            CreateSpecialAbility("name", "base name", 9, 266);
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, It.IsAny<string>())).Returns("name");

            mockDamageDataSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.AbilityDamages, "name"))
                .Returns([]);

            mockDamageDataSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.AbilityDamages, "namex90210"))
                .Returns([new DamageDataSelection { Type = "my damage type", Roll = "my roll", Condition = "my condition" }]);

            var weapon = new Weapon
            {
                ItemType = ItemTypeConstants.Weapon,
                Name = "my weapon",
                CriticalMultiplier = "x90210",
                Attributes = [AttributeConstants.Melee]
            };
            weapon.Magic.Bonus = 1;

            var abilities = specialAbilitiesGenerator.GenerateFor(weapon, power, 1);
            Assert.That(abilities, Is.Not.Empty);

            var ability = abilities.First();
            Assert.That(ability.AttributeRequirements, Is.EqualTo(itemAttributes));
            Assert.That(ability.Name, Is.EqualTo("name"));
            Assert.That(ability.BaseName, Is.EqualTo("base name"));
            Assert.That(ability.Power, Is.EqualTo(266));
            Assert.That(ability.BonusEquivalent, Is.EqualTo(9));
            Assert.That(ability.Damages, Is.Empty);
            Assert.That(ability.CriticalDamages, Has.Count.EqualTo(1));
            Assert.That(ability.CriticalDamages[0].Roll, Is.EqualTo("my roll"));
            Assert.That(ability.CriticalDamages[0].Type, Is.EqualTo("my damage type"));
            Assert.That(ability.CriticalDamages[0].Condition, Is.EqualTo("my condition"));
            Assert.That(abilities.Count(), Is.EqualTo(1));
        }

        [Test]
        public void GenerateFor_SetCriticalDamagesByMultiplier_WithCondition()
        {
            CreateSpecialAbility("name", "base name", 9, 266);
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, It.IsAny<string>())).Returns("name");

            mockDamageDataSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.AbilityDamages, "name"))
                .Returns([]);

            mockDamageDataSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.AbilityDamages, "namex90210"))
                .Returns(
                [
                    new DamageDataSelection { Type = "my damage type", Roll = "my roll", Condition = "my condition" },
                    new DamageDataSelection { Type = "my damage type 2", Roll = "my roll 2", Condition = "my condition 2" },
                ]);

            var weapon = new Weapon
            {
                ItemType = ItemTypeConstants.Weapon,
                Name = "my weapon",
                CriticalMultiplier = "x90210",
                Attributes = [AttributeConstants.Melee]
            };
            weapon.Magic.Bonus = 1;

            var abilities = specialAbilitiesGenerator.GenerateFor(weapon, power, 1);
            Assert.That(abilities, Is.Not.Empty);

            var ability = abilities.First();
            Assert.That(ability.AttributeRequirements, Is.EqualTo(itemAttributes));
            Assert.That(ability.Name, Is.EqualTo("name"));
            Assert.That(ability.BaseName, Is.EqualTo("base name"));
            Assert.That(ability.Power, Is.EqualTo(266));
            Assert.That(ability.BonusEquivalent, Is.EqualTo(9));
            Assert.That(ability.Damages, Is.Empty);
            Assert.That(ability.CriticalDamages, Has.Count.EqualTo(2));
            Assert.That(ability.CriticalDamages[0].Roll, Is.EqualTo("my roll"));
            Assert.That(ability.CriticalDamages[0].Type, Is.EqualTo("my damage type"));
            Assert.That(ability.CriticalDamages[0].Condition, Is.EqualTo("my condition"));
            Assert.That(ability.CriticalDamages[1].Roll, Is.EqualTo("my roll 2"));
            Assert.That(ability.CriticalDamages[1].Type, Is.EqualTo("my damage type 2"));
            Assert.That(ability.CriticalDamages[1].Condition, Is.EqualTo("my condition 2"));
            Assert.That(abilities.Count(), Is.EqualTo(1));
        }

        [Test]
        public void GenerateFor_SetDamageAndCriticalDamageByMultiplier()
        {
            CreateSpecialAbility("name", "base name", 9, 266);
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, It.IsAny<string>())).Returns("name");

            mockDamageDataSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.AbilityDamages, "name"))
                .Returns([new DamageDataSelection { Type = "my damage type", Roll = "my roll" }]);

            mockDamageDataSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.AbilityDamages, "namex90210"))
                .Returns([new DamageDataSelection { Type = "my other damage type", Roll = "my other roll" }]);

            var weapon = new Weapon
            {
                ItemType = ItemTypeConstants.Weapon,
                Name = "my weapon",
                CriticalMultiplier = "x90210",
                Attributes = [AttributeConstants.Melee]
            };
            weapon.Magic.Bonus = 1;

            var abilities = specialAbilitiesGenerator.GenerateFor(weapon, power, 1);
            Assert.That(abilities, Is.Not.Empty);

            var ability = abilities.First();
            Assert.That(ability.AttributeRequirements, Is.EqualTo(itemAttributes));
            Assert.That(ability.Name, Is.EqualTo("name"));
            Assert.That(ability.BaseName, Is.EqualTo("base name"));
            Assert.That(ability.Power, Is.EqualTo(266));
            Assert.That(ability.BonusEquivalent, Is.EqualTo(9));
            Assert.That(ability.Damages, Has.Count.EqualTo(1));
            Assert.That(ability.Damages[0].Roll, Is.EqualTo("my roll"));
            Assert.That(ability.Damages[0].Type, Is.EqualTo("my damage type"));
            Assert.That(ability.Damages[0].Condition, Is.Empty);
            Assert.That(ability.CriticalDamages, Has.Count.EqualTo(1));
            Assert.That(ability.CriticalDamages[0].Roll, Is.EqualTo("my other roll"));
            Assert.That(ability.CriticalDamages[0].Type, Is.EqualTo("my other damage type"));
            Assert.That(ability.CriticalDamages[0].Condition, Is.Empty);
            Assert.That(abilities.Count(), Is.EqualTo(1));
        }

        [Test]
        public void GenerateFor_SetDamagesAndCriticalDamagesByMultiplier_WithConditions()
        {
            CreateSpecialAbility("name", "base name", 9, 266);
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, It.IsAny<string>())).Returns("name");

            mockDamageDataSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.AbilityDamages, "name"))
                .Returns(
                [
                    new DamageDataSelection { Type = "my damage type", Roll = "my roll", Condition = "my condition" },
                    new DamageDataSelection { Type = "my damage type 2", Roll = "my roll 2", Condition = "my condition 2" },
                ]);

            mockDamageDataSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.AbilityDamages, "namex90210"))
                .Returns(
                [
                    new DamageDataSelection { Type = "my other damage type", Roll = "my other roll", Condition = "my other condition" },
                    new DamageDataSelection { Type = "my other damage type 2", Roll = "my other roll 2", Condition = "my other condition 2" },
                ]);

            var weapon = new Weapon
            {
                ItemType = ItemTypeConstants.Weapon,
                Name = "my weapon",
                CriticalMultiplier = "x90210",
                Attributes = [AttributeConstants.Melee]
            };
            weapon.Magic.Bonus = 1;

            var abilities = specialAbilitiesGenerator.GenerateFor(weapon, power, 1);
            Assert.That(abilities, Is.Not.Empty);

            var ability = abilities.First();
            Assert.That(ability.AttributeRequirements, Is.EqualTo(itemAttributes));
            Assert.That(ability.Name, Is.EqualTo("name"));
            Assert.That(ability.BaseName, Is.EqualTo("base name"));
            Assert.That(ability.Power, Is.EqualTo(266));
            Assert.That(ability.BonusEquivalent, Is.EqualTo(9));
            Assert.That(ability.Damages, Has.Count.EqualTo(2));
            Assert.That(ability.Damages[0].Roll, Is.EqualTo("my roll"));
            Assert.That(ability.Damages[0].Type, Is.EqualTo("my damage type"));
            Assert.That(ability.Damages[0].Condition, Is.EqualTo("my condition"));
            Assert.That(ability.Damages[1].Roll, Is.EqualTo("my roll 2"));
            Assert.That(ability.Damages[1].Type, Is.EqualTo("my damage type 2"));
            Assert.That(ability.Damages[1].Condition, Is.EqualTo("my condition 2"));
            Assert.That(ability.CriticalDamages, Has.Count.EqualTo(2));
            Assert.That(ability.CriticalDamages[0].Roll, Is.EqualTo("my other roll"));
            Assert.That(ability.CriticalDamages[0].Type, Is.EqualTo("my other damage type"));
            Assert.That(ability.CriticalDamages[0].Condition, Is.EqualTo("my other condition"));
            Assert.That(ability.CriticalDamages[1].Roll, Is.EqualTo("my other roll 2"));
            Assert.That(ability.CriticalDamages[1].Type, Is.EqualTo("my other damage type 2"));
            Assert.That(ability.CriticalDamages[1].Condition, Is.EqualTo("my other condition 2"));
            Assert.That(abilities.Count(), Is.EqualTo(1));
        }

        [Test]
        public void GenerateFor_WeaponsThatAreBothMeleeAndRangedGetRandomlyBetweenTables()
        {
            item.ItemType = ItemTypeConstants.Weapon;
            itemAttributes.Add(AttributeConstants.Melee);
            itemAttributes.Add(AttributeConstants.Ranged);

            var tableName = TableNameConstants.Percentiles.POWERATTRIBUTESpecialAbilities(power, AttributeConstants.Melee);
            mockPercentileSelector.Setup(p => p.SelectAllFrom(Config.Name, tableName)).Returns(["melee ability"]);
            tableName = TableNameConstants.Percentiles.POWERATTRIBUTESpecialAbilities(power, AttributeConstants.Ranged);
            mockPercentileSelector.Setup(p => p.SelectAllFrom(Config.Name, tableName)).Returns(["ranged ability"]);

            var meleeResult = new SpecialAbilityDataSelection
            {
                BaseName = "melee ability"
            };
            mockSpecialAbilityDataSelector
                .Setup(s => s.SelectOneFrom(Config.Name, TableNameConstants.Collections.SpecialAbilityData, meleeResult.BaseName))
                .Returns(meleeResult);
            mockCollectionsSelector
                .Setup(p => p.SelectFrom(Config.Name, TableNameConstants.Collections.SpecialAbilityAttributeRequirements, meleeResult.BaseName))
                .Returns(itemAttributes);

            var rangedResult = new SpecialAbilityDataSelection
            {
                BaseName = "ranged ability"
            };
            mockSpecialAbilityDataSelector
                .Setup(s => s.SelectOneFrom(Config.Name, TableNameConstants.Collections.SpecialAbilityData, rangedResult.BaseName))
                .Returns(rangedResult);
            mockCollectionsSelector
                .Setup(p => p.SelectFrom(Config.Name, TableNameConstants.Collections.SpecialAbilityAttributeRequirements, rangedResult.BaseName))
                .Returns(itemAttributes);

            var abilities = specialAbilitiesGenerator.GenerateFor(item, power, 2);
            Assert.That(abilities, Is.Not.Empty);

            var names = abilities.Select(a => a.Name);

            Assert.That(names, Contains.Item("melee ability"));
            Assert.That(names, Contains.Item("ranged ability"));
            Assert.That(names.Count(), Is.EqualTo(2));
        }

        [Test]
        public void GenerateFor_DoNotAllowAbilitiesAndMagicBonusToBeGreaterThan10()
        {
            item.Magic.Bonus = 9;

            CreateSpecialAbility("big ability", bonus: 2);
            CreateSpecialAbility("small ability", bonus: 1);

            mockPercentileSelector.SetupSequence(p => p.SelectFrom(Config.Name, It.IsAny<string>()))
                .Returns("big ability")
                .Returns("small ability");

            var abilities = specialAbilitiesGenerator.GenerateFor(item, power, 1);
            Assert.That(abilities, Is.Not.Empty);

            var names = abilities.Select(a => a.Name);
            Assert.That(names, Contains.Item("small ability"));
            Assert.That(names.Count(), Is.EqualTo(1));
        }

        [Test]
        public void GenerateFor_AccumulateSpecialAbilities()
        {
            item.Magic.Bonus = 5;

            CreateSpecialAbility("ability 1", bonus: 2);
            CreateSpecialAbility("ability 2", bonus: 2);
            CreateSpecialAbility("ability 3", bonus: 3);
            CreateSpecialAbility("ability 4", bonus: 0);

            mockPercentileSelector
                .SetupSequence(p => p.SelectFrom(Config.Name, It.IsAny<string>()))
                .Returns("ability 1")
                .Returns("ability 2")
                .Returns("ability 3")
                .Returns("ability 4");

            var abilities = specialAbilitiesGenerator.GenerateFor(item, power, 3);
            Assert.That(abilities, Is.Not.Empty);

            var names = abilities.Select(a => a.Name);
            Assert.That(names, Contains.Item("ability 1"));
            Assert.That(names, Contains.Item("ability 2"));
            Assert.That(names, Contains.Item("ability 4"));
            Assert.That(names.Count(), Is.EqualTo(3));
        }

        [Test]
        public void GenerateFor_ReplaceWeakWithStrong()
        {
            CreateSpecialAbility("weak ability", "ability", power: 1);
            CreateSpecialAbility("strong ability", "ability", power: 2);

            mockPercentileSelector.SetupSequence(p => p.SelectFrom(Config.Name, It.IsAny<string>()))
                .Returns("weak ability")
                .Returns("strong ability");

            mockSpecialAbilityDataSelector.Setup(s => s.IsCollection(Config.Name, TableNameConstants.Collections.SpecialAbilityData, "weak ability")).Returns(true);
            mockSpecialAbilityDataSelector.Setup(s => s.IsCollection(Config.Name, TableNameConstants.Collections.SpecialAbilityData, "strong ability")).Returns(true);

            var abilities = specialAbilitiesGenerator.GenerateFor(item, power, 2);
            Assert.That(abilities, Is.Not.Empty);

            var names = abilities.Select(a => a.Name);
            Assert.That(names, Contains.Item("strong ability"));
            Assert.That(names.Count(), Is.EqualTo(1));
        }

        [Test]
        public void GenerateFor_DoNotCompareStrengthForDissimilarCoreName()
        {
            CreateSpecialAbility("weak ability", power: 1);
            CreateSpecialAbility("strong ability", power: 2);
            mockPercentileSelector.SetupSequence(p => p.SelectFrom(Config.Name, It.IsAny<string>())).Returns("weak ability").Returns("strong ability");

            var abilities = specialAbilitiesGenerator.GenerateFor(item, power, 2);
            Assert.That(abilities, Is.Not.Empty);

            var names = abilities.Select(a => a.Name);
            Assert.That(names, Contains.Item("weak ability"));
            Assert.That(names, Contains.Item("strong ability"));
            Assert.That(names.Count(), Is.EqualTo(2));
        }

        [Test]
        public void GenerateFor_DoNotReplaceStrongWithWeak()
        {
            CreateSpecialAbility("strong ability", "ability", power: 2);
            CreateSpecialAbility("weak ability", "ability", power: 1);
            CreateSpecialAbility("other ability");
            mockPercentileSelector.SetupSequence(p => p.SelectFrom(Config.Name, It.IsAny<string>())).Returns("strong ability").Returns("weak ability")
                .Returns("other ability");

            var abilities = specialAbilitiesGenerator.GenerateFor(item, power, 2);
            Assert.That(abilities, Is.Not.Empty);

            var names = abilities.Select(a => a.Name);
            Assert.That(names, Contains.Item("strong ability"));
            Assert.That(names, Is.Not.Contains("weak ability"));
        }

        [Test]
        public void GenerateFor_SpecialAbilitiesMaxOutAtBonusOf10()
        {
            CreateSpecialAbility("ability 1", bonus: 2);
            CreateSpecialAbility("ability 2", bonus: 2);
            CreateSpecialAbility("ability 3", bonus: 3);
            CreateSpecialAbility("ability 4", bonus: 3);
            mockPercentileSelector.SetupSequence(p => p.SelectFrom(Config.Name, It.IsAny<string>())).Returns("ability 1").Returns("ability 2")
                .Returns("ability 3").Returns("ability 4");

            var abilities = specialAbilitiesGenerator.GenerateFor(item, power, 4);
            Assert.That(abilities, Is.Not.Empty);

            var names = abilities.Select(a => a.Name);
            Assert.That(names, Contains.Item("ability 1"));
            Assert.That(names, Contains.Item("ability 2"));
            Assert.That(names, Contains.Item("ability 3"));
            Assert.That(names.Count(), Is.EqualTo(3));
        }

        [Test]
        public void GenerateFor_CanGetAbilitiesWithBonusOf0WhileAtBonusOf10()
        {
            CreateSpecialAbility("ability 1", bonus: 9);
            CreateSpecialAbility("ability 2", bonus: 0);
            CreateSpecialAbility("ability 3", bonus: 3);
            mockPercentileSelector.SetupSequence(p => p.SelectFrom(Config.Name, It.IsAny<string>())).Returns("ability 1").Returns("ability 2")
                .Returns("ability 3");

            var abilities = specialAbilitiesGenerator.GenerateFor(item, power, 2);
            Assert.That(abilities, Is.Not.Empty);

            var names = abilities.Select(a => a.Name);
            Assert.That(names, Contains.Item("ability 1"));
            Assert.That(names, Contains.Item("ability 2"));
            Assert.That(names.Count(), Is.EqualTo(2));
        }

        [Test]
        public void GenerateFor_DuplicateAbilitiesCannotBeAdded()
        {
            CreateSpecialAbility("ability 1");
            CreateSpecialAbility("ability 2");
            CreateSpecialAbility("ability 3");
            mockPercentileSelector.SetupSequence(p => p.SelectFrom(Config.Name, It.IsAny<string>())).Returns("ability 1").Returns("ability 1")
                .Returns("ability 2");

            var abilities = specialAbilitiesGenerator.GenerateFor(item, power, 2);
            Assert.That(abilities, Is.Not.Empty);

            var names = abilities.Select(a => a.Name);
            Assert.That(names, Contains.Item("ability 1"));
            Assert.That(names, Contains.Item("ability 2"));
            Assert.That(names.Count(), Is.EqualTo(2));
        }

        [Test]
        public void GenerateFor_SpecialAbilitiesFilteredByAttributeRequirementsFromBaseName()
        {
            CreateSpecialAbility("ability 1", "base ability 1");
            CreateSpecialAbility("ability 2", "base ability 2");
            mockPercentileSelector.SetupSequence(p => p.SelectFrom(Config.Name, It.IsAny<string>())).Returns("ability 1").Returns("ability 2");

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecialAbilityAttributeRequirements, "base ability 1"))
                .Returns(new[] { "other type", "type 1" });
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecialAbilityAttributeRequirements, "base ability 2"))
                .Returns(itemAttributes);

            var abilities = specialAbilitiesGenerator.GenerateFor(item, power, 1);
            Assert.That(abilities, Is.Not.Empty);

            var names = abilities.Select(a => a.Name);
            Assert.That(names, Contains.Item("ability 2"));
            Assert.That(names.Count(), Is.EqualTo(1));
        }

        [Test]
        public void GenerateFor_SpecialAbilitiesFilteredByEitherAttributeRequirementsFromBaseName()
        {
            itemAttributes.Add("or");

            CreateSpecialAbility("ability 1", "base ability 1");
            CreateSpecialAbility("ability 2", "base ability 2");
            mockPercentileSelector.SetupSequence(p => p.SelectFrom(Config.Name, It.IsAny<string>())).Returns("ability 1").Returns("ability 2");

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecialAbilityAttributeRequirements, "base ability 1"))
                .Returns(new[] { "other/order/of/type", "type 1" });
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecialAbilityAttributeRequirements, "base ability 2"))
                .Returns(new[] { "either/or" });

            var abilities = specialAbilitiesGenerator.GenerateFor(item, power, 1);
            Assert.That(abilities, Is.Not.Empty);

            var names = abilities.Select(a => a.Name);
            Assert.That(names, Contains.Item("ability 2"));
            Assert.That(names.Count(), Is.EqualTo(1));
        }

        [Test]
        public void GenerateFor_ExtraAttributesDoNotMatter()
        {
            CreateSpecialAbility("ability");
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, It.IsAny<string>())).Returns("ability");
            itemAttributes.Add("type 1");
            item.Attributes = itemAttributes.Union(new[] { "other type" });

            var abilities = specialAbilitiesGenerator.GenerateFor(item, power, 1);
            Assert.That(abilities, Is.Not.Empty);

            var names = abilities.Select(a => a.Name);
            Assert.That(names, Contains.Item("ability"));
            Assert.That(names.Count(), Is.EqualTo(1));
        }

        [Test]
        public void GenerateFor_BonusSpecialAbilitiesAdded()
        {
            CreateSpecialAbility("ability 1", "base ability 1");
            CreateSpecialAbility("ability 2", "base ability 2");
            mockPercentileSelector.SetupSequence(p => p.SelectFrom(Config.Name, It.IsAny<string>()))
                .Returns("BonusSpecialAbility")
                .Returns("ability 1")
                .Returns("ability 2");

            var abilities = specialAbilitiesGenerator.GenerateFor(item, power, 1);
            Assert.That(abilities, Is.Not.Empty);

            var names = abilities.Select(a => a.Name);
            Assert.That(names, Contains.Item("ability 1"));
            Assert.That(names, Contains.Item("ability 2"));
            Assert.That(names.Count(), Is.EqualTo(2));
        }

        [Test]
        public void GenerateFor_StopIfAllPossibleAbilitiesAcquired()
        {
            CreateSpecialAbility("ability");
            mockPercentileSelector.Setup(p => p.SelectFrom(Config.Name, It.IsAny<string>())).Returns("ability");

            var abilities = specialAbilitiesGenerator.GenerateFor(item, power, 5);
            Assert.That(abilities, Is.Not.Empty);
            Assert.That(abilities.Count(), Is.EqualTo(1));
        }

        [Test]
        public void GenerateFor_CountSameCoreNameAsSameAbility()
        {
            CreateSpecialAbility("ability 1", "base name", power: 1);
            CreateSpecialAbility("ability 2", "base name", power: 2);
            mockPercentileSelector.SetupSequence(p => p.SelectFrom(Config.Name, It.IsAny<string>())).Returns("ability 1").Returns("ability 2");

            mockSpecialAbilityDataSelector.Setup(s => s.IsCollection(Config.Name, TableNameConstants.Collections.SpecialAbilityData, "ability 1")).Returns(true);
            mockSpecialAbilityDataSelector.Setup(s => s.IsCollection(Config.Name, TableNameConstants.Collections.SpecialAbilityData, "ability 2")).Returns(true);

            var abilities = specialAbilitiesGenerator.GenerateFor(item, power, 5);
            Assert.That(abilities, Is.Not.Empty);
            Assert.That(abilities.Count(), Is.EqualTo(1));
        }

        [Test]
        public void GenerateFor_ReturnEmptyIfNoCompatibleAbilities()
        {
            mockPercentileSelector.Setup(p => p.SelectAllFrom(Config.Name, It.IsAny<string>())).Returns(Enumerable.Empty<string>());
            var abilities = specialAbilitiesGenerator.GenerateFor(item, power, 1);
            Assert.That(abilities, Is.Empty);
        }

        [Test]
        public void GenerateFor_ReturnAllAbilitiesWithStrongestIfQuantityGreaterThanOrEqualToAllAvailableAbilities()
        {
            CreateSpecialAbility("ability 1");
            CreateSpecialAbility("ability 2");
            CreateSpecialAbility("ability 3", "ability", power: 1);
            CreateSpecialAbility("ability 4", "ability", power: 2);
            mockPercentileSelector.SetupSequence(p => p.SelectFrom(Config.Name, It.IsAny<string>())).Returns("ability 1").Returns("ability 2")
                .Returns("ability 3").Returns("ability 4");

            mockSpecialAbilityDataSelector.Setup(s => s.IsCollection(Config.Name, TableNameConstants.Collections.SpecialAbilityData, "ability 1")).Returns(true);
            mockSpecialAbilityDataSelector.Setup(s => s.IsCollection(Config.Name, TableNameConstants.Collections.SpecialAbilityData, "ability 2")).Returns(true);
            mockSpecialAbilityDataSelector.Setup(s => s.IsCollection(Config.Name, TableNameConstants.Collections.SpecialAbilityData, "ability 3")).Returns(true);
            mockSpecialAbilityDataSelector.Setup(s => s.IsCollection(Config.Name, TableNameConstants.Collections.SpecialAbilityData, "ability 4")).Returns(true);

            var abilities = specialAbilitiesGenerator.GenerateFor(item, power, 4);
            Assert.That(abilities, Is.Not.Empty);

            var names = abilities.Select(a => a.Name);
            Assert.That(names, Contains.Item("ability 1"));
            Assert.That(names, Contains.Item("ability 2"));
            Assert.That(names, Contains.Item("ability 4"));
            Assert.That(names.Count(), Is.EqualTo(3));
            mockPercentileSelector.Verify(p => p.SelectFrom(Config.Name, It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void GenerateFor_DoNotReturnAbilitiesWithBonusSumGreaterThan10()
        {
            CreateSpecialAbility("ability 1", bonus: 3);
            CreateSpecialAbility("ability 2", bonus: 3);
            CreateSpecialAbility("ability 3", bonus: 3);
            CreateSpecialAbility("ability 4", bonus: 3);
            mockPercentileSelector.SetupSequence(p => p.SelectFrom(Config.Name, It.IsAny<string>())).Returns("ability 1").Returns("ability 2")
                .Returns("ability 3").Returns("ability 4");

            var abilities = specialAbilitiesGenerator.GenerateFor(item, power, 5);
            Assert.That(abilities, Is.Not.Empty);

            var names = abilities.Select(a => a.Name);
            Assert.That(names, Contains.Item("ability 1"));
            Assert.That(names, Contains.Item("ability 2"));
            Assert.That(names, Contains.Item("ability 3"));
            Assert.That(names.Count(), Is.EqualTo(3));
            mockPercentileSelector.Verify(p => p.SelectFrom(Config.Name, It.IsAny<string>()), Times.Exactly(3));
        }

        [Test]
        public void GenerateFor_RemoveWeakerAbilitiesFromAvailableWhenStrongAdded()
        {
            CreateSpecialAbility("ability 1");
            CreateSpecialAbility("ability 2");
            CreateSpecialAbility("ability 3", "ability", power: 1);
            CreateSpecialAbility("ability 4", "ability", power: 2);
            mockPercentileSelector.SetupSequence(p => p.SelectFrom(Config.Name, It.IsAny<string>())).Returns("ability 4").Returns("ability 2")
                .Returns("ability 3").Returns("ability 1");

            var abilities = specialAbilitiesGenerator.GenerateFor(item, power, 3);
            Assert.That(abilities, Is.Not.Empty);

            var names = abilities.Select(a => a.Name);
            Assert.That(names, Contains.Item("ability 1"));
            Assert.That(names, Contains.Item("ability 2"));
            Assert.That(names, Contains.Item("ability 4"));
            Assert.That(names.Count(), Is.EqualTo(3));
            mockPercentileSelector.Verify(p => p.SelectFrom(Config.Name, It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void GenerateFor_WhenAddingAllAbilities_UpgradeAllWeakAbilities()
        {
            CreateSpecialAbility("ability 1");
            CreateSpecialAbility("ability 2");
            CreateSpecialAbility("ability 3", "ability", power: 1);
            CreateSpecialAbility("ability 4", "ability", power: 2);
            mockPercentileSelector.SetupSequence(p => p.SelectFrom(Config.Name, It.IsAny<string>()))
                .Returns("ability 3").Returns("BonusSpecialAbility")
                .Returns("ability 2").Returns("ability 1").Returns("ability 4");

            var abilities = specialAbilitiesGenerator.GenerateFor(item, power, 3);
            Assert.That(abilities, Is.Not.Empty);

            var names = abilities.Select(a => a.Name);
            Assert.That(names, Contains.Item("ability 1"));
            Assert.That(names, Contains.Item("ability 2"));
            Assert.That(names, Contains.Item("ability 4"));
            Assert.That(names.Count(), Is.EqualTo(3));
            mockPercentileSelector.Verify(p => p.SelectFrom(Config.Name, It.IsAny<string>()), Times.Exactly(2));
        }

        private void CreateSpecialAbility(string name, string baseName = "", int bonus = 0, int power = 0)
        {
            var result = new SpecialAbilityDataSelection();

            if (string.IsNullOrEmpty(baseName))
                result.BaseName = name;
            else
                result.BaseName = baseName;

            result.BonusEquivalent = bonus;
            result.Power = power;
            names.Add(name);

            mockSpecialAbilityDataSelector.Setup(s => s.SelectOneFrom(Config.Name, TableNameConstants.Collections.SpecialAbilityData, name)).Returns(result);
            mockCollectionsSelector
                .Setup(p => p.SelectFrom(Config.Name, TableNameConstants.Collections.SpecialAbilityAttributeRequirements, result.BaseName))
                .Returns(itemAttributes);
        }

        [Test]
        public void GenerateFor_Prototypes_CreateSetSpecialAbilities()
        {
            CreateSpecialAbility("ability 1", "base 1", 9266, 90210);
            CreateSpecialAbility("ability 2", "base 2", 42, 600);
            CreateSpecialAbility("ability 3", "base 3", 1337, 1234);

            var attributeRequirements = new List<IEnumerable<string>>
            {
                (["other type 1", "type 1"]),
                (["other type 2", "type 2"]),
                (["other type 3", "type 3"])
            };

            mockSpecialAbilityDataSelector.Setup(s => s.IsCollection(Config.Name, TableNameConstants.Collections.SpecialAbilityData, "ability 1")).Returns(true);
            mockSpecialAbilityDataSelector.Setup(s => s.IsCollection(Config.Name, TableNameConstants.Collections.SpecialAbilityData, "ability 2")).Returns(true);
            mockSpecialAbilityDataSelector.Setup(s => s.IsCollection(Config.Name, TableNameConstants.Collections.SpecialAbilityData, "ability 3")).Returns(true);

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecialAbilityAttributeRequirements, "base 1"))
                .Returns(attributeRequirements[0]);
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecialAbilityAttributeRequirements, "base 2"))
                .Returns(attributeRequirements[1]);
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecialAbilityAttributeRequirements, "base 3"))
                .Returns(attributeRequirements[2]);

            mockDamageDataSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.AbilityDamages, "ability 2"))
                .Returns(
                [
                    new DamageDataSelection { Type = "my damage type", Roll = "my roll" },
                ]);

            mockDamageDataSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.AbilityDamages, "ability 3x90210"))
                .Returns(
                [
                    new DamageDataSelection { Type = "my crit damage type", Roll = "my crit roll" },
                ]);

            var abilityPrototypes = new[]
            {
                new SpecialAbility { Name = "ability 1" },
                new SpecialAbility { Name = "ability 2" },
                new SpecialAbility { Name = "ability 3" },
            };

            var abilities = specialAbilitiesGenerator.GenerateFor(abilityPrototypes, "x90210");
            Assert.That(abilities, Is.Not.Empty);

            var abilityArray = abilities.ToArray();
            Assert.That(abilityArray[0].Name, Is.EqualTo("ability 1"));
            Assert.That(abilityArray[0].BaseName, Is.EqualTo("base 1"));
            Assert.That(abilityArray[0].AttributeRequirements, Is.EqualTo(attributeRequirements[0]));
            Assert.That(abilityArray[0].Power, Is.EqualTo(90210));
            Assert.That(abilityArray[0].BonusEquivalent, Is.EqualTo(9266));
            Assert.That(abilityArray[0].Damages, Is.Empty);
            Assert.That(abilityArray[0].CriticalDamages, Is.Empty);
            Assert.That(abilityArray[1].Name, Is.EqualTo("ability 2"));
            Assert.That(abilityArray[1].BaseName, Is.EqualTo("base 2"));
            Assert.That(abilityArray[1].AttributeRequirements, Is.EqualTo(attributeRequirements[1]));
            Assert.That(abilityArray[1].Power, Is.EqualTo(600));
            Assert.That(abilityArray[1].BonusEquivalent, Is.EqualTo(42));
            Assert.That(abilityArray[1].Damages, Has.Count.EqualTo(1));
            Assert.That(abilityArray[1].Damages[0].Roll, Is.EqualTo("my roll"));
            Assert.That(abilityArray[1].Damages[0].Type, Is.EqualTo("my damage type"));
            Assert.That(abilityArray[1].CriticalDamages, Is.Empty);
            Assert.That(abilityArray[2].Name, Is.EqualTo("ability 3"));
            Assert.That(abilityArray[2].BaseName, Is.EqualTo("base 3"));
            Assert.That(abilityArray[2].AttributeRequirements, Is.EqualTo(attributeRequirements[2]));
            Assert.That(abilityArray[2].Power, Is.EqualTo(1234));
            Assert.That(abilityArray[2].BonusEquivalent, Is.EqualTo(1337));
            Assert.That(abilityArray[2].Damages, Is.Empty);
            Assert.That(abilityArray[2].CriticalDamages, Has.Count.EqualTo(1));
            Assert.That(abilityArray[2].CriticalDamages[0].Roll, Is.EqualTo("my crit roll"));
            Assert.That(abilityArray[2].CriticalDamages[0].Type, Is.EqualTo("my crit damage type"));
            Assert.That(abilityArray.Length, Is.EqualTo(3));
        }

        [Test]
        public void GenerateFor_Prototypes_CreateSetSpecialAbilities_GetMostPowerful()
        {
            CreateSpecialAbility("ability 1", "base 1", 9266, 90210);
            CreateSpecialAbility("ability 2", "base 2", 42, 600);
            CreateSpecialAbility("ability 2.1", "base 2", 1337, 1336);
            CreateSpecialAbility("ability 3", "base 3", 96, 783);

            var attributeRequirements = new List<IEnumerable<string>>
            {
                (["other type 1", "type 1"]),
                (["other type 2", "type 2"]),
                (["other type 3", "type 3"])
            };

            mockSpecialAbilityDataSelector.Setup(s => s.IsCollection(Config.Name, TableNameConstants.Collections.SpecialAbilityData, "ability 1")).Returns(true);
            mockSpecialAbilityDataSelector.Setup(s => s.IsCollection(Config.Name, TableNameConstants.Collections.SpecialAbilityData, "ability 2")).Returns(true);
            mockSpecialAbilityDataSelector.Setup(s => s.IsCollection(Config.Name, TableNameConstants.Collections.SpecialAbilityData, "ability 2.1")).Returns(true);
            mockSpecialAbilityDataSelector.Setup(s => s.IsCollection(Config.Name, TableNameConstants.Collections.SpecialAbilityData, "ability 3")).Returns(true);

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecialAbilityAttributeRequirements, "base 1"))
                .Returns(attributeRequirements[0]);
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecialAbilityAttributeRequirements, "base 2"))
                .Returns(attributeRequirements[1]);
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecialAbilityAttributeRequirements, "base 3"))
                .Returns(attributeRequirements[2]);

            mockDamageDataSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.AbilityDamages, "ability 2"))
                .Returns(
                [
                    new DamageDataSelection { Type = "my damage type", Roll = "my roll" },
                ]);

            mockDamageDataSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.AbilityDamages, "ability 2.1"))
                .Returns(
                [
                    new DamageDataSelection { Type = "my higher damage type", Roll = "my higher roll" },
                ]);

            mockDamageDataSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.AbilityDamages, "ability 3x90210"))
                .Returns(
                [
                    new DamageDataSelection { Type = "my crit damage type", Roll = "my crit roll" },
                ]);

            var abilityPrototypes = new[]
            {
                new SpecialAbility { Name = "ability 1" },
                new SpecialAbility { Name = "ability 2" },
                new SpecialAbility { Name = "ability 2.1" },
                new SpecialAbility { Name = "ability 3" },
            };

            var abilities = specialAbilitiesGenerator.GenerateFor(abilityPrototypes, "x90210");
            Assert.That(abilities, Is.Not.Empty);

            var abilityArray = abilities.ToArray();
            Assert.That(abilityArray[0].Name, Is.EqualTo("ability 1"));
            Assert.That(abilityArray[0].BaseName, Is.EqualTo("base 1"));
            Assert.That(abilityArray[0].AttributeRequirements, Is.EqualTo(attributeRequirements[0]));
            Assert.That(abilityArray[0].Power, Is.EqualTo(90210));
            Assert.That(abilityArray[0].BonusEquivalent, Is.EqualTo(9266));
            Assert.That(abilityArray[0].Damages, Is.Empty);
            Assert.That(abilityArray[0].CriticalDamages, Is.Empty);
            Assert.That(abilityArray[1].Name, Is.EqualTo("ability 2.1"));
            Assert.That(abilityArray[1].BaseName, Is.EqualTo("base 2"));
            Assert.That(abilityArray[1].AttributeRequirements, Is.EqualTo(attributeRequirements[1]));
            Assert.That(abilityArray[1].Power, Is.EqualTo(1336));
            Assert.That(abilityArray[1].BonusEquivalent, Is.EqualTo(1337));
            Assert.That(abilityArray[1].Damages, Has.Count.EqualTo(1));
            Assert.That(abilityArray[1].Damages[0].Roll, Is.EqualTo("my higher roll"));
            Assert.That(abilityArray[1].Damages[0].Type, Is.EqualTo("my higher damage type"));
            Assert.That(abilityArray[1].CriticalDamages, Is.Empty);
            Assert.That(abilityArray[2].Name, Is.EqualTo("ability 3"));
            Assert.That(abilityArray[2].BaseName, Is.EqualTo("base 3"));
            Assert.That(abilityArray[2].AttributeRequirements, Is.EqualTo(attributeRequirements[2]));
            Assert.That(abilityArray[2].Power, Is.EqualTo(783));
            Assert.That(abilityArray[2].BonusEquivalent, Is.EqualTo(96));
            Assert.That(abilityArray[2].Damages, Is.Empty);
            Assert.That(abilityArray[2].CriticalDamages, Has.Count.EqualTo(1));
            Assert.That(abilityArray[2].CriticalDamages[0].Roll, Is.EqualTo("my crit roll"));
            Assert.That(abilityArray[2].CriticalDamages[0].Type, Is.EqualTo("my crit damage type"));
            Assert.That(abilityArray.Length, Is.EqualTo(3));
        }

        [Test]
        public void GenerateFor_Prototypes_CreateSetSpecialAbilities_RemoveDuplicates()
        {
            CreateSpecialAbility("ability 1", "base 1", 9266, 90210);
            CreateSpecialAbility("ability 2", "base 2", 42, 600);
            CreateSpecialAbility("ability 3", "base 3", 1337, 1234);

            var attributeRequirements = new List<IEnumerable<string>>
            {
                (["other type 1", "type 1"]),
                (["other type 2", "type 2"]),
                (["other type 3", "type 3"])
            };

            mockSpecialAbilityDataSelector.Setup(s => s.IsCollection(Config.Name, TableNameConstants.Collections.SpecialAbilityData, "ability 1")).Returns(true);
            mockSpecialAbilityDataSelector.Setup(s => s.IsCollection(Config.Name, TableNameConstants.Collections.SpecialAbilityData, "ability 2")).Returns(true);
            mockSpecialAbilityDataSelector.Setup(s => s.IsCollection(Config.Name, TableNameConstants.Collections.SpecialAbilityData, "ability 3")).Returns(true);

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecialAbilityAttributeRequirements, "base 1"))
                .Returns(attributeRequirements[0]);
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecialAbilityAttributeRequirements, "base 2"))
                .Returns(attributeRequirements[1]);
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecialAbilityAttributeRequirements, "base 3"))
                .Returns(attributeRequirements[2]);

            mockDamageDataSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.AbilityDamages, "ability 2"))
                .Returns(
                [
                    new DamageDataSelection { Type = "my damage type", Roll = "my roll" },
                ]);

            mockDamageDataSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.AbilityDamages, "ability 3x90210"))
                .Returns(
                [
                    new DamageDataSelection { Type = "my crit damage type", Roll = "my crit roll" },
                ]);

            var abilityPrototypes = new[]
            {
                new SpecialAbility { Name = "ability 1" },
                new SpecialAbility { Name = "ability 2" },
                new SpecialAbility { Name = "ability 2" },
                new SpecialAbility { Name = "ability 3" },
            };

            var abilities = specialAbilitiesGenerator.GenerateFor(abilityPrototypes, "x90210");
            Assert.That(abilities, Is.Not.Empty);

            var abilityArray = abilities.ToArray();
            Assert.That(abilityArray[0].Name, Is.EqualTo("ability 1"));
            Assert.That(abilityArray[0].BaseName, Is.EqualTo("base 1"));
            Assert.That(abilityArray[0].AttributeRequirements, Is.EqualTo(attributeRequirements[0]));
            Assert.That(abilityArray[0].Power, Is.EqualTo(90210));
            Assert.That(abilityArray[0].BonusEquivalent, Is.EqualTo(9266));
            Assert.That(abilityArray[0].Damages, Is.Empty);
            Assert.That(abilityArray[0].CriticalDamages, Is.Empty);
            Assert.That(abilityArray[1].Name, Is.EqualTo("ability 2"));
            Assert.That(abilityArray[1].BaseName, Is.EqualTo("base 2"));
            Assert.That(abilityArray[1].AttributeRequirements, Is.EqualTo(attributeRequirements[1]));
            Assert.That(abilityArray[1].Power, Is.EqualTo(600));
            Assert.That(abilityArray[1].BonusEquivalent, Is.EqualTo(42));
            Assert.That(abilityArray[1].Damages, Has.Count.EqualTo(1));
            Assert.That(abilityArray[1].Damages[0].Roll, Is.EqualTo("my roll"));
            Assert.That(abilityArray[1].Damages[0].Type, Is.EqualTo("my damage type"));
            Assert.That(abilityArray[1].CriticalDamages, Is.Empty);
            Assert.That(abilityArray[2].Name, Is.EqualTo("ability 3"));
            Assert.That(abilityArray[2].BaseName, Is.EqualTo("base 3"));
            Assert.That(abilityArray[2].AttributeRequirements, Is.EqualTo(attributeRequirements[2]));
            Assert.That(abilityArray[2].Power, Is.EqualTo(1234));
            Assert.That(abilityArray[2].BonusEquivalent, Is.EqualTo(1337));
            Assert.That(abilityArray[2].Damages, Is.Empty);
            Assert.That(abilityArray[2].CriticalDamages, Has.Count.EqualTo(1));
            Assert.That(abilityArray[2].CriticalDamages[0].Roll, Is.EqualTo("my crit roll"));
            Assert.That(abilityArray[2].CriticalDamages[0].Type, Is.EqualTo("my crit damage type"));
            Assert.That(abilityArray.Length, Is.EqualTo(3));
        }

        [Test]
        public void GenerateFor_Prototypes_CreateCustomSpecialAbilities()
        {
            CreateSpecialAbility("ability 1", "base 1", 9266, 90210);
            CreateSpecialAbility("ability 2", "base 2", 42, 600);
            CreateSpecialAbility("ability 3", "base 3", 1337, 1234);

            var attributeRequirements = new List<IEnumerable<string>>
            {
                (["other type 1", "type 1"]),
                (["other type 2", "type 2"]),
                (["other type 3", "type 3"])
            };

            mockSpecialAbilityDataSelector.Setup(s => s.IsCollection(Config.Name, TableNameConstants.Collections.SpecialAbilityData, "ability 1")).Returns(false);
            mockSpecialAbilityDataSelector.Setup(s => s.IsCollection(Config.Name, TableNameConstants.Collections.SpecialAbilityData, "ability 2")).Returns(false);
            mockSpecialAbilityDataSelector.Setup(s => s.IsCollection(Config.Name, TableNameConstants.Collections.SpecialAbilityData, "ability 3")).Returns(false);

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecialAbilityAttributeRequirements, "base 1"))
                .Returns(attributeRequirements[0]);
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecialAbilityAttributeRequirements, "base 2"))
                .Returns(attributeRequirements[1]);
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecialAbilityAttributeRequirements, "base 3"))
                .Returns(attributeRequirements[2]);

            mockDamageDataSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.AbilityDamages, "ability 2"))
                .Returns(
                [
                    new DamageDataSelection { Type = "my damage type", Roll = "my roll" },
                ]);

            mockDamageDataSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.AbilityDamages, "ability 3x90210"))
                .Returns(
                [
                    new DamageDataSelection { Type = "my crit damage type", Roll = "my crit roll" },
                ]);

            var abilityPrototypes = new[]
            {
                new SpecialAbility { Name = "ability 1", BaseName = "custom base 1", BonusEquivalent = 2345, Power = 9876 },
                new SpecialAbility { Name = "ability 2", BaseName = "custom base 2", BonusEquivalent = 3456, Power = 8765 },
                new SpecialAbility { Name = "ability 3", BaseName = "custom base 3", BonusEquivalent = 4567, Power = 7654 },
            };

            var abilities = specialAbilitiesGenerator.GenerateFor(abilityPrototypes, "x90210");
            Assert.That(abilities, Is.Not.Empty);

            var abilityArray = abilities.ToArray();
            Assert.That(abilityArray[0].AttributeRequirements, Is.Empty);
            Assert.That(abilityArray[0].Name, Is.EqualTo("ability 1"));
            Assert.That(abilityArray[0].BaseName, Is.EqualTo("custom base 1"));
            Assert.That(abilityArray[0].Power, Is.EqualTo(9876));
            Assert.That(abilityArray[0].BonusEquivalent, Is.EqualTo(2345));
            Assert.That(abilityArray[0].Damages, Is.Empty);
            Assert.That(abilityArray[0].CriticalDamages, Is.Empty);
            Assert.That(abilityArray[1].AttributeRequirements, Is.Empty);
            Assert.That(abilityArray[1].Name, Is.EqualTo("ability 2"));
            Assert.That(abilityArray[1].BaseName, Is.EqualTo("custom base 2"));
            Assert.That(abilityArray[1].Power, Is.EqualTo(8765));
            Assert.That(abilityArray[1].BonusEquivalent, Is.EqualTo(3456));
            Assert.That(abilityArray[1].Damages, Is.Empty);
            Assert.That(abilityArray[1].CriticalDamages, Is.Empty);
            Assert.That(abilityArray[2].AttributeRequirements, Is.Empty);
            Assert.That(abilityArray[2].Name, Is.EqualTo("ability 3"));
            Assert.That(abilityArray[2].BaseName, Is.EqualTo("custom base 3"));
            Assert.That(abilityArray[2].Power, Is.EqualTo(7654));
            Assert.That(abilityArray[2].BonusEquivalent, Is.EqualTo(4567));
            Assert.That(abilityArray[2].Damages, Is.Empty);
            Assert.That(abilityArray[2].CriticalDamages, Is.Empty);
            Assert.That(abilityArray.Length, Is.EqualTo(3));
        }

        [Test]
        public void GenerateFor_Prototypes_CreateSetAndCustomSpecialAbilities()
        {
            CreateSpecialAbility("ability 1", "base 1", 9266, 90210);
            CreateSpecialAbility("ability 2", "base 2", 42, 600);
            CreateSpecialAbility("ability 3", "base 3", 1337, 1234);

            var attributeRequirements = new List<IEnumerable<string>>
            {
                (["other type 1", "type 1"]),
                (["other type 2", "type 2"]),
                (["other type 3", "type 3"])
            };

            mockSpecialAbilityDataSelector.Setup(s => s.IsCollection(Config.Name, TableNameConstants.Collections.SpecialAbilityData, "ability 1")).Returns(true);
            mockSpecialAbilityDataSelector.Setup(s => s.IsCollection(Config.Name, TableNameConstants.Collections.SpecialAbilityData, "ability 2")).Returns(false);
            mockSpecialAbilityDataSelector.Setup(s => s.IsCollection(Config.Name, TableNameConstants.Collections.SpecialAbilityData, "ability 3")).Returns(true);

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecialAbilityAttributeRequirements, "base 1"))
                .Returns(attributeRequirements[0]);
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecialAbilityAttributeRequirements, "base 2"))
                .Returns(attributeRequirements[1]);
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecialAbilityAttributeRequirements, "base 3"))
                .Returns(attributeRequirements[2]);

            mockDamageDataSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.AbilityDamages, "ability 2"))
                .Returns(
                [
                    new DamageDataSelection { Type = "my damage type", Roll = "my roll" },
                ]);

            mockDamageDataSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.AbilityDamages, "ability 3x90210"))
                .Returns(
                [
                    new DamageDataSelection { Type = "my crit damage type", Roll = "my crit roll" },
                ]);

            var abilityPrototypes = new[]
            {
                new SpecialAbility { Name = "ability 1", BaseName = "custom base 1", BonusEquivalent = 2345, Power = 9876 },
                new SpecialAbility { Name = "ability 2", BaseName = "custom base 2", BonusEquivalent = 3456, Power = 8765 },
                new SpecialAbility { Name = "ability 3", BaseName = "custom base 3", BonusEquivalent = 4567, Power = 7654 },
            };

            var abilities = specialAbilitiesGenerator.GenerateFor(abilityPrototypes, "x90210");
            Assert.That(abilities, Is.Not.Empty);

            var abilityArray = abilities.ToArray();
            Assert.That(abilityArray[0].Name, Is.EqualTo("ability 1"));
            Assert.That(abilityArray[0].BaseName, Is.EqualTo("base 1"));
            Assert.That(abilityArray[0].AttributeRequirements, Is.EqualTo(attributeRequirements[0]));
            Assert.That(abilityArray[0].Power, Is.EqualTo(90210));
            Assert.That(abilityArray[0].BonusEquivalent, Is.EqualTo(9266));
            Assert.That(abilityArray[0].Damages, Is.Empty);
            Assert.That(abilityArray[0].CriticalDamages, Is.Empty);
            Assert.That(abilityArray[1].Name, Is.EqualTo("ability 2"));
            Assert.That(abilityArray[1].BaseName, Is.EqualTo("custom base 2"));
            Assert.That(abilityArray[1].AttributeRequirements, Is.Empty);
            Assert.That(abilityArray[1].Power, Is.EqualTo(8765));
            Assert.That(abilityArray[1].BonusEquivalent, Is.EqualTo(3456));
            Assert.That(abilityArray[1].Damages, Is.Empty);
            Assert.That(abilityArray[1].CriticalDamages, Is.Empty);
            Assert.That(abilityArray[2].Name, Is.EqualTo("ability 3"));
            Assert.That(abilityArray[2].BaseName, Is.EqualTo("base 3"));
            Assert.That(abilityArray[2].AttributeRequirements, Is.EqualTo(attributeRequirements[2]));
            Assert.That(abilityArray[2].Power, Is.EqualTo(1234));
            Assert.That(abilityArray[2].BonusEquivalent, Is.EqualTo(1337));
            Assert.That(abilityArray[2].Damages, Is.Empty);
            Assert.That(abilityArray[2].CriticalDamages, Has.Count.EqualTo(1));
            Assert.That(abilityArray[2].CriticalDamages[0].Roll, Is.EqualTo("my crit roll"));
            Assert.That(abilityArray[2].CriticalDamages[0].Type, Is.EqualTo("my crit damage type"));
            Assert.That(abilityArray.Length, Is.EqualTo(3));
        }

        [Test]
        public void GenerateFor_Prototypes_CreateSetAndCustomSpecialAbilities_GetMostPowerful()
        {
            CreateSpecialAbility("ability 1", "base 1", 9266, 90210);
            CreateSpecialAbility("ability 2", "base 2", 42, 600);
            CreateSpecialAbility("ability 3.1", "base 3", 1337, 1234);
            CreateSpecialAbility("ability 3", "base 3", 1336, 96);

            var attributeRequirements = new List<IEnumerable<string>>
            {
                (["other type 1", "type 1"]),
                (["other type 2", "type 2"]),
                (["other type 3", "type 3"])
            };

            mockSpecialAbilityDataSelector.Setup(s => s.IsCollection(Config.Name, TableNameConstants.Collections.SpecialAbilityData, "ability 1")).Returns(true);
            mockSpecialAbilityDataSelector.Setup(s => s.IsCollection(Config.Name, TableNameConstants.Collections.SpecialAbilityData, "ability 2")).Returns(false);
            mockSpecialAbilityDataSelector.Setup(s => s.IsCollection(Config.Name, TableNameConstants.Collections.SpecialAbilityData, "ability 3")).Returns(true);
            mockSpecialAbilityDataSelector.Setup(s => s.IsCollection(Config.Name, TableNameConstants.Collections.SpecialAbilityData, "ability 3.1")).Returns(true);

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecialAbilityAttributeRequirements, "base 1"))
                .Returns(attributeRequirements[0]);
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecialAbilityAttributeRequirements, "base 2"))
                .Returns(attributeRequirements[1]);
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecialAbilityAttributeRequirements, "base 3"))
                .Returns(attributeRequirements[2]);

            mockDamageDataSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.AbilityDamages, "ability 2"))
                .Returns(
                [
                    new DamageDataSelection { Type = "my damage type", Roll = "my roll" },
                ]);

            mockDamageDataSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.AbilityDamages, "ability 3x90210"))
                .Returns(
                [
                    new DamageDataSelection { Type = "my crit damage type", Roll = "my crit roll" },
                ]);

            mockDamageDataSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.AbilityDamages, "ability 3.1x90210"))
                .Returns(
                [
                    new DamageDataSelection { Type = "my higher crit damage type", Roll = "my higher crit roll" },
                ]);

            var abilityPrototypes = new[]
            {
                new SpecialAbility { Name = "ability 1", BaseName = "custom base 1", BonusEquivalent = 2345, Power = 9876 },
                new SpecialAbility { Name = "ability 2", BaseName = "custom base 2", BonusEquivalent = 3456, Power = 8765 },
                new SpecialAbility { Name = "ability 3", BaseName = "custom base 3", BonusEquivalent = 4567, Power = 7654 },
                new SpecialAbility { Name = "ability 3.1", BaseName = "custom base 3", BonusEquivalent = 5678, Power = 6543 },
            };

            var abilities = specialAbilitiesGenerator.GenerateFor(abilityPrototypes, "x90210");
            Assert.That(abilities, Is.Not.Empty);

            var abilityArray = abilities.ToArray();
            Assert.That(abilityArray[0].Name, Is.EqualTo("ability 1"));
            Assert.That(abilityArray[0].BaseName, Is.EqualTo("base 1"));
            Assert.That(abilityArray[0].AttributeRequirements, Is.EqualTo(attributeRequirements[0]));
            Assert.That(abilityArray[0].Power, Is.EqualTo(90210));
            Assert.That(abilityArray[0].BonusEquivalent, Is.EqualTo(9266));
            Assert.That(abilityArray[0].Damages, Is.Empty);
            Assert.That(abilityArray[0].CriticalDamages, Is.Empty);
            Assert.That(abilityArray[1].Name, Is.EqualTo("ability 2"));
            Assert.That(abilityArray[1].BaseName, Is.EqualTo("custom base 2"));
            Assert.That(abilityArray[1].AttributeRequirements, Is.Empty);
            Assert.That(abilityArray[1].Power, Is.EqualTo(8765));
            Assert.That(abilityArray[1].BonusEquivalent, Is.EqualTo(3456));
            Assert.That(abilityArray[1].Damages, Is.Empty);
            Assert.That(abilityArray[1].CriticalDamages, Is.Empty);
            Assert.That(abilityArray[2].Name, Is.EqualTo("ability 3.1"));
            Assert.That(abilityArray[2].BaseName, Is.EqualTo("base 3"));
            Assert.That(abilityArray[2].AttributeRequirements, Is.EqualTo(attributeRequirements[2]));
            Assert.That(abilityArray[2].Power, Is.EqualTo(1234));
            Assert.That(abilityArray[2].BonusEquivalent, Is.EqualTo(1337));
            Assert.That(abilityArray[2].Damages, Is.Empty);
            Assert.That(abilityArray[2].CriticalDamages, Has.Count.EqualTo(1));
            Assert.That(abilityArray[2].CriticalDamages[0].Roll, Is.EqualTo("my higher crit roll"));
            Assert.That(abilityArray[2].CriticalDamages[0].Type, Is.EqualTo("my higher crit damage type"));
            Assert.That(abilityArray.Length, Is.EqualTo(3));
        }

        [Test]
        public void GenerateFor_Prototypes_CreateSetAndCustomSpecialAbilities_RemoveDuplicates()
        {
            CreateSpecialAbility("ability 1", "base 1", 9266, 90210);
            CreateSpecialAbility("ability 2", "base 2", 42, 600);
            CreateSpecialAbility("ability 3", "base 3", 1337, 1234);

            var attributeRequirements = new List<IEnumerable<string>>
            {
                (["other type 1", "type 1"]),
                (["other type 2", "type 2"]),
                (["other type 3", "type 3"])
            };

            mockSpecialAbilityDataSelector.Setup(s => s.IsCollection(Config.Name, TableNameConstants.Collections.SpecialAbilityData, "ability 1")).Returns(true);
            mockSpecialAbilityDataSelector.Setup(s => s.IsCollection(Config.Name, TableNameConstants.Collections.SpecialAbilityData, "ability 2")).Returns(false);
            mockSpecialAbilityDataSelector.Setup(s => s.IsCollection(Config.Name, TableNameConstants.Collections.SpecialAbilityData, "ability 3")).Returns(true);

            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecialAbilityAttributeRequirements, "base 1"))
                .Returns(attributeRequirements[0]);
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecialAbilityAttributeRequirements, "base 2"))
                .Returns(attributeRequirements[1]);
            mockCollectionsSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.SpecialAbilityAttributeRequirements, "base 3"))
                .Returns(attributeRequirements[2]);

            mockDamageDataSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.AbilityDamages, "ability 2"))
                .Returns(
                [
                    new DamageDataSelection { Type = "my damage type", Roll = "my roll" },
                ]);

            mockDamageDataSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.AbilityDamages, "ability 3x90210"))
                .Returns(
                [
                    new DamageDataSelection { Type = "my crit damage type", Roll = "my crit roll" },
                ]);

            var abilityPrototypes = new[]
            {
                new SpecialAbility { Name = "ability 1", BaseName = "custom base 1", BonusEquivalent = 2345, Power = 9876 },
                new SpecialAbility { Name = "ability 2", BaseName = "custom base 2", BonusEquivalent = 3456, Power = 8765 },
                new SpecialAbility { Name = "ability 3", BaseName = "custom base 3", BonusEquivalent = 4567, Power = 7654 },
                new SpecialAbility { Name = "ability 3", BaseName = "custom base 3", BonusEquivalent = 4567, Power = 7654 },
            };

            var abilities = specialAbilitiesGenerator.GenerateFor(abilityPrototypes, "x90210");
            Assert.That(abilities, Is.Not.Empty);

            var abilityArray = abilities.ToArray();
            Assert.That(abilityArray[0].Name, Is.EqualTo("ability 1"));
            Assert.That(abilityArray[0].BaseName, Is.EqualTo("base 1"));
            Assert.That(abilityArray[0].AttributeRequirements, Is.EqualTo(attributeRequirements[0]));
            Assert.That(abilityArray[0].Power, Is.EqualTo(90210));
            Assert.That(abilityArray[0].BonusEquivalent, Is.EqualTo(9266));
            Assert.That(abilityArray[0].Damages, Is.Empty);
            Assert.That(abilityArray[0].CriticalDamages, Is.Empty);
            Assert.That(abilityArray[1].Name, Is.EqualTo("ability 2"));
            Assert.That(abilityArray[1].BaseName, Is.EqualTo("custom base 2"));
            Assert.That(abilityArray[1].AttributeRequirements, Is.Empty);
            Assert.That(abilityArray[1].Power, Is.EqualTo(8765));
            Assert.That(abilityArray[1].BonusEquivalent, Is.EqualTo(3456));
            Assert.That(abilityArray[1].Damages, Is.Empty);
            Assert.That(abilityArray[1].CriticalDamages, Is.Empty);
            Assert.That(abilityArray[2].Name, Is.EqualTo("ability 3"));
            Assert.That(abilityArray[2].BaseName, Is.EqualTo("base 3"));
            Assert.That(abilityArray[2].AttributeRequirements, Is.EqualTo(attributeRequirements[2]));
            Assert.That(abilityArray[2].Power, Is.EqualTo(1234));
            Assert.That(abilityArray[2].BonusEquivalent, Is.EqualTo(1337));
            Assert.That(abilityArray[2].Damages, Is.Empty);
            Assert.That(abilityArray[2].CriticalDamages, Has.Count.EqualTo(1));
            Assert.That(abilityArray[2].CriticalDamages[0].Roll, Is.EqualTo("my crit roll"));
            Assert.That(abilityArray[2].CriticalDamages[0].Type, Is.EqualTo("my crit damage type"));
            Assert.That(abilityArray.Length, Is.EqualTo(3));
        }

        [Test]
        public void ApplyAbilitiesToWeapon_NoAbilities()
        {
            var weapon = CreateWeapon();

            var updatedWeapon = specialAbilitiesGenerator.ApplyAbilitiesToWeapon(weapon);
            Assert.That(updatedWeapon, Is.EqualTo(weapon));
            Assert.That(updatedWeapon.Magic.SpecialAbilities, Is.Empty);
            Assert.That(updatedWeapon.Damages, Has.Count.EqualTo(1));
            Assert.That(updatedWeapon.Damages[0].Roll, Is.EqualTo("my roll"));
            Assert.That(updatedWeapon.Damages[0].Type, Is.EqualTo(WeaponDamageType));
            Assert.That(updatedWeapon.Damages[0].Condition, Is.Empty);
            Assert.That(updatedWeapon.CriticalDamages, Has.Count.EqualTo(1));
            Assert.That(updatedWeapon.CriticalDamages[0].Roll, Is.EqualTo("my crit roll"));
            Assert.That(updatedWeapon.CriticalDamages[0].Type, Is.EqualTo(WeaponDamageType_Crit));
            Assert.That(updatedWeapon.CriticalDamages[0].Condition, Is.Empty);
        }

        private Weapon CreateWeapon(bool isDouble = false)
        {
            var weapon = new Weapon
            {
                Name = "my weapon",
                Damages = [new Damage { Type = WeaponDamageType, Roll = "my roll" }],
                CriticalDamages = [new Damage { Type = WeaponDamageType_Crit, Roll = "my crit roll" }],
                CriticalMultiplier = "x9266",
                ThreatRange = 1
            };

            weapon.Magic.Bonus = 90210;

            if (isDouble)
            {
                weapon.Attributes = ["attribute 1", AttributeConstants.DoubleWeapon, "attribute 3"];
                weapon.SecondaryCriticalMultiplier = "other crit";
                weapon.SecondaryDamages.Add(new Damage { Roll = "jab", Type = "punch" });
                weapon.SecondaryCriticalDamages.Add(new Damage { Roll = "cross", Type = "punch" });
            }
            else
            {
                weapon.Attributes = ["attribute 1", "attribute 2", "attribute 3"];
            }

            return weapon;
        }

        private const string WeaponDamageType = "my weapon damage type";
        private const string WeaponDamageType_Crit = "my weapon crit damage type";

        [Test]
        public void ApplyAbilitiesToWeapon_NoAbilityDamages()
        {
            var weapon = CreateWeapon();
            weapon.Magic.SpecialAbilities = [new SpecialAbility { Name = "my special ability" }];

            var updatedWeapon = specialAbilitiesGenerator.ApplyAbilitiesToWeapon(weapon);
            Assert.That(updatedWeapon, Is.EqualTo(weapon));
            Assert.That(updatedWeapon.Magic.SpecialAbilities.Count(), Is.EqualTo(1));
            Assert.That(updatedWeapon.Damages, Has.Count.EqualTo(1));
            Assert.That(updatedWeapon.Damages[0].Roll, Is.EqualTo("my roll"));
            Assert.That(updatedWeapon.Damages[0].Type, Is.EqualTo(WeaponDamageType));
            Assert.That(updatedWeapon.Damages[0].Condition, Is.Empty);
            Assert.That(updatedWeapon.CriticalDamages, Has.Count.EqualTo(1));
            Assert.That(updatedWeapon.CriticalDamages[0].Roll, Is.EqualTo("my crit roll"));
            Assert.That(updatedWeapon.CriticalDamages[0].Type, Is.EqualTo(WeaponDamageType_Crit));
            Assert.That(updatedWeapon.CriticalDamages[0].Condition, Is.Empty);
        }

        [Test]
        public void ApplyAbilitiesToWeapon_Damages()
        {
            var ability = new SpecialAbility();
            ability.Damages.Add(new Damage { Roll = "some", Type = "plasma", Condition = "my condition" });
            ability.Damages.Add(new Damage { Roll = "a bit", Type = "ether", Condition = "my other condition" });

            var weapon = CreateWeapon();
            weapon.Magic.SpecialAbilities = [new SpecialAbility { Name = "my special ability" }, ability];

            var updatedWeapon = specialAbilitiesGenerator.ApplyAbilitiesToWeapon(weapon);
            Assert.That(updatedWeapon, Is.EqualTo(weapon));
            Assert.That(weapon.Magic.SpecialAbilities.Count(), Is.EqualTo(2));
            Assert.That(weapon.Damages, Has.Count.EqualTo(3));
            Assert.That(weapon.Damages[0].Roll, Is.EqualTo("my roll"));
            Assert.That(weapon.Damages[0].Type, Is.EqualTo(WeaponDamageType));
            Assert.That(weapon.Damages[0].Condition, Is.Empty);
            Assert.That(weapon.Damages[1].Roll, Is.EqualTo("some"));
            Assert.That(weapon.Damages[1].Type, Is.EqualTo("plasma"));
            Assert.That(weapon.Damages[1].Condition, Is.EqualTo("my condition"));
            Assert.That(weapon.Damages[2].Roll, Is.EqualTo("a bit"));
            Assert.That(weapon.Damages[2].Type, Is.EqualTo("ether"));
            Assert.That(weapon.Damages[2].Condition, Is.EqualTo("my other condition"));
            Assert.That(weapon.CriticalDamages, Has.Count.EqualTo(1));
            Assert.That(weapon.CriticalDamages[0].Roll, Is.EqualTo("my crit roll"));
            Assert.That(weapon.CriticalDamages[0].Type, Is.EqualTo(WeaponDamageType_Crit));
            Assert.That(weapon.CriticalDamages[0].Condition, Is.Empty);
            Assert.That(weapon.SecondaryDamages, Is.Empty);
            Assert.That(weapon.SecondaryCriticalDamages, Is.Empty);
        }

        [Test]
        public void ApplyAbilitiesToWeapon_Damages_EmptyDamageType()
        {
            var ability = new SpecialAbility();
            ability.Damages.Add(new Damage { Roll = "some", Type = string.Empty, Condition = "my condition" });
            ability.Damages.Add(new Damage { Roll = "a bit", Type = string.Empty, Condition = "my other condition" });

            var weapon = CreateWeapon();
            weapon.Magic.SpecialAbilities = [new SpecialAbility { Name = "my special ability" }, ability];

            var updatedWeapon = specialAbilitiesGenerator.ApplyAbilitiesToWeapon(weapon);
            Assert.That(updatedWeapon, Is.EqualTo(weapon));
            Assert.That(weapon.Magic.SpecialAbilities.Count(), Is.EqualTo(2));
            Assert.That(weapon.Damages, Has.Count.EqualTo(3));
            Assert.That(weapon.Damages[0].Roll, Is.EqualTo("my roll"));
            Assert.That(weapon.Damages[0].Type, Is.EqualTo(WeaponDamageType));
            Assert.That(weapon.Damages[0].Condition, Is.Empty);
            Assert.That(weapon.Damages[1].Roll, Is.EqualTo("some"));
            Assert.That(weapon.Damages[1].Type, Is.EqualTo(WeaponDamageType));
            Assert.That(weapon.Damages[1].Condition, Is.EqualTo("my condition"));
            Assert.That(weapon.Damages[2].Roll, Is.EqualTo("a bit"));
            Assert.That(weapon.Damages[2].Type, Is.EqualTo(WeaponDamageType));
            Assert.That(weapon.Damages[2].Condition, Is.EqualTo("my other condition"));
            Assert.That(weapon.CriticalDamages, Has.Count.EqualTo(1));
            Assert.That(weapon.CriticalDamages[0].Roll, Is.EqualTo("my crit roll"));
            Assert.That(weapon.CriticalDamages[0].Type, Is.EqualTo(WeaponDamageType_Crit));
            Assert.That(weapon.CriticalDamages[0].Condition, Is.Empty);
            Assert.That(weapon.SecondaryDamages, Is.Empty);
            Assert.That(weapon.SecondaryCriticalDamages, Is.Empty);
        }

        [Test]
        public void ApplyAbilitiesToWeapon_CriticalDamages()
        {
            var ability = new SpecialAbility
            {
                CriticalDamages =
                [
                    new Damage { Roll = "more", Type = "plasma", Condition = "my crit condition" },
                    new Damage { Roll = "a lot", Type = "ether" },
                ]
            };

            var weapon = CreateWeapon();
            weapon.Magic.SpecialAbilities = [new SpecialAbility(), ability];

            var updatedWeapon = specialAbilitiesGenerator.ApplyAbilitiesToWeapon(weapon);
            Assert.That(updatedWeapon, Is.EqualTo(weapon));
            Assert.That(weapon.Magic.SpecialAbilities.Count(), Is.EqualTo(2));
            Assert.That(weapon.Damages, Has.Count.EqualTo(1));
            Assert.That(weapon.Damages[0].Roll, Is.EqualTo("my roll"));
            Assert.That(weapon.Damages[0].Type, Is.EqualTo(WeaponDamageType));
            Assert.That(weapon.Damages[0].Condition, Is.Empty);
            Assert.That(weapon.CriticalDamages, Has.Count.EqualTo(3));
            Assert.That(weapon.CriticalDamages[0].Roll, Is.EqualTo("my crit roll"));
            Assert.That(weapon.CriticalDamages[0].Type, Is.EqualTo(WeaponDamageType_Crit));
            Assert.That(weapon.CriticalDamages[0].Condition, Is.Empty);
            Assert.That(weapon.CriticalDamages[1].Roll, Is.EqualTo("more"));
            Assert.That(weapon.CriticalDamages[1].Type, Is.EqualTo("plasma"));
            Assert.That(weapon.CriticalDamages[1].Condition, Is.EqualTo("my crit condition"));
            Assert.That(weapon.CriticalDamages[2].Roll, Is.EqualTo("a lot"));
            Assert.That(weapon.CriticalDamages[2].Type, Is.EqualTo("ether"));
            Assert.That(weapon.CriticalDamages[2].Condition, Is.Empty);
            Assert.That(weapon.SecondaryDamages, Is.Empty);
            Assert.That(weapon.SecondaryCriticalDamages, Is.Empty);
        }

        [Test]
        public void ApplyAbilitiesToWeapon_CriticalDamages_EmptyDamageType()
        {
            var ability = new SpecialAbility
            {
                CriticalDamages =
                [
                    new Damage { Roll = "more", Type = string.Empty, Condition = "my crit condition" },
                    new Damage { Roll = "a lot", Type = string.Empty },
                ]
            };

            var weapon = CreateWeapon();
            weapon.Magic.SpecialAbilities = [new SpecialAbility(), ability];

            var updatedWeapon = specialAbilitiesGenerator.ApplyAbilitiesToWeapon(weapon);
            Assert.That(updatedWeapon, Is.EqualTo(weapon));
            Assert.That(weapon.Magic.SpecialAbilities.Count(), Is.EqualTo(2));
            Assert.That(weapon.Damages, Has.Count.EqualTo(1));
            Assert.That(weapon.Damages[0].Roll, Is.EqualTo("my roll"));
            Assert.That(weapon.Damages[0].Type, Is.EqualTo(WeaponDamageType));
            Assert.That(weapon.Damages[0].Condition, Is.Empty);
            Assert.That(weapon.CriticalDamages, Has.Count.EqualTo(3));
            Assert.That(weapon.CriticalDamages[0].Roll, Is.EqualTo("my crit roll"));
            Assert.That(weapon.CriticalDamages[0].Type, Is.EqualTo(WeaponDamageType_Crit));
            Assert.That(weapon.CriticalDamages[0].Condition, Is.Empty);
            Assert.That(weapon.CriticalDamages[1].Roll, Is.EqualTo("more"));
            Assert.That(weapon.CriticalDamages[1].Type, Is.EqualTo(WeaponDamageType_Crit));
            Assert.That(weapon.CriticalDamages[1].Condition, Is.EqualTo("my crit condition"));
            Assert.That(weapon.CriticalDamages[2].Roll, Is.EqualTo("a lot"));
            Assert.That(weapon.CriticalDamages[2].Type, Is.EqualTo(WeaponDamageType_Crit));
            Assert.That(weapon.CriticalDamages[2].Condition, Is.Empty);
            Assert.That(weapon.SecondaryDamages, Is.Empty);
            Assert.That(weapon.SecondaryCriticalDamages, Is.Empty);
        }

        [Test]
        public void ApplyAbilitiesToWeapon_DamagesAndCriticalDamages()
        {
            var ability = new SpecialAbility();
            ability.Damages.Add(new Damage { Roll = "some", Type = "plasma", Condition = "my condition" });
            ability.Damages.Add(new Damage { Roll = "a bit", Type = "ether", Condition = "my other condition" });

            ability.CriticalDamages =
            [
                new Damage { Roll = "more", Type = "plasma", Condition = "my crit condition" },
                new Damage { Roll = "a lot", Type = "ether" },
            ];

            var weapon = CreateWeapon();
            weapon.Magic.SpecialAbilities = [new SpecialAbility(), ability];

            var updatedWeapon = specialAbilitiesGenerator.ApplyAbilitiesToWeapon(weapon);
            Assert.That(updatedWeapon, Is.EqualTo(weapon));
            Assert.That(weapon.Magic.SpecialAbilities.Count(), Is.EqualTo(2));
            Assert.That(weapon.Damages, Has.Count.EqualTo(3));
            Assert.That(weapon.Damages[0].Roll, Is.EqualTo("my roll"));
            Assert.That(weapon.Damages[0].Type, Is.EqualTo(WeaponDamageType));
            Assert.That(weapon.Damages[0].Condition, Is.Empty);
            Assert.That(weapon.Damages[1].Roll, Is.EqualTo("some"));
            Assert.That(weapon.Damages[1].Type, Is.EqualTo("plasma"));
            Assert.That(weapon.Damages[1].Condition, Is.EqualTo("my condition"));
            Assert.That(weapon.Damages[2].Roll, Is.EqualTo("a bit"));
            Assert.That(weapon.Damages[2].Type, Is.EqualTo("ether"));
            Assert.That(weapon.Damages[2].Condition, Is.EqualTo("my other condition"));
            Assert.That(weapon.CriticalDamages, Has.Count.EqualTo(3));
            Assert.That(weapon.CriticalDamages[0].Roll, Is.EqualTo("my crit roll"));
            Assert.That(weapon.CriticalDamages[0].Type, Is.EqualTo(WeaponDamageType_Crit));
            Assert.That(weapon.CriticalDamages[0].Condition, Is.Empty);
            Assert.That(weapon.CriticalDamages[1].Roll, Is.EqualTo("more"));
            Assert.That(weapon.CriticalDamages[1].Type, Is.EqualTo("plasma"));
            Assert.That(weapon.CriticalDamages[1].Condition, Is.EqualTo("my crit condition"));
            Assert.That(weapon.CriticalDamages[2].Roll, Is.EqualTo("a lot"));
            Assert.That(weapon.CriticalDamages[2].Type, Is.EqualTo("ether"));
            Assert.That(weapon.CriticalDamages[2].Condition, Is.Empty);
            Assert.That(weapon.SecondaryDamages, Is.Empty);
            Assert.That(weapon.SecondaryCriticalDamages, Is.Empty);
        }

        [Test]
        public void ApplyAbilitiesToWeapon_DamagesAndCriticalDamages_EmptyDamageTypes()
        {
            var ability = new SpecialAbility();
            ability.Damages.Add(new Damage { Roll = "some", Type = string.Empty, Condition = "my condition" });
            ability.Damages.Add(new Damage { Roll = "a bit", Type = "ether", Condition = "my other condition" });

            ability.CriticalDamages =
            [
                new Damage { Roll = "more", Type = string.Empty, Condition = "my crit condition" },
                new Damage { Roll = "a lot", Type = "ether" },
            ];

            var weapon = CreateWeapon();
            weapon.Magic.SpecialAbilities = [new SpecialAbility(), ability];

            var updatedWeapon = specialAbilitiesGenerator.ApplyAbilitiesToWeapon(weapon);
            Assert.That(updatedWeapon, Is.EqualTo(weapon));
            Assert.That(weapon.Magic.SpecialAbilities.Count(), Is.EqualTo(2));
            Assert.That(weapon.Damages, Has.Count.EqualTo(3));
            Assert.That(weapon.Damages[0].Roll, Is.EqualTo("my roll"));
            Assert.That(weapon.Damages[0].Type, Is.EqualTo(WeaponDamageType));
            Assert.That(weapon.Damages[0].Condition, Is.Empty);
            Assert.That(weapon.Damages[1].Roll, Is.EqualTo("some"));
            Assert.That(weapon.Damages[1].Type, Is.EqualTo(WeaponDamageType));
            Assert.That(weapon.Damages[1].Condition, Is.EqualTo("my condition"));
            Assert.That(weapon.Damages[2].Roll, Is.EqualTo("a bit"));
            Assert.That(weapon.Damages[2].Type, Is.EqualTo("ether"));
            Assert.That(weapon.Damages[2].Condition, Is.EqualTo("my other condition"));
            Assert.That(weapon.CriticalDamages, Has.Count.EqualTo(3));
            Assert.That(weapon.CriticalDamages[0].Roll, Is.EqualTo("my crit roll"));
            Assert.That(weapon.CriticalDamages[0].Type, Is.EqualTo(WeaponDamageType_Crit));
            Assert.That(weapon.CriticalDamages[0].Condition, Is.Empty);
            Assert.That(weapon.CriticalDamages[1].Roll, Is.EqualTo("more"));
            Assert.That(weapon.CriticalDamages[1].Type, Is.EqualTo(WeaponDamageType_Crit));
            Assert.That(weapon.CriticalDamages[1].Condition, Is.EqualTo("my crit condition"));
            Assert.That(weapon.CriticalDamages[2].Roll, Is.EqualTo("a lot"));
            Assert.That(weapon.CriticalDamages[2].Type, Is.EqualTo("ether"));
            Assert.That(weapon.CriticalDamages[2].Condition, Is.Empty);
            Assert.That(weapon.SecondaryDamages, Is.Empty);
            Assert.That(weapon.SecondaryCriticalDamages, Is.Empty);
        }

        [Test]
        public void ApplyAbilitiesToWeapon_Damages_DoubleWeapon_SameEnhancement()
        {
            mockPercentileSelector.Setup(s => s.SelectFrom(.5)).Returns(true);

            var ability = new SpecialAbility();
            ability.Name = "my name";
            ability.Damages.Add(new Damage { Roll = "some", Type = "plasma", Condition = "my condition" });
            ability.Damages.Add(new Damage { Roll = "a bit", Type = "ether", Condition = "my other condition" });

            var weapon = CreateWeapon(true);
            weapon.Magic.SpecialAbilities = [new SpecialAbility(), ability];
            weapon.SecondaryMagicBonus = weapon.Magic.Bonus;
            weapon.SecondaryHasAbilities = true;

            var updatedWeapon = specialAbilitiesGenerator.ApplyAbilitiesToWeapon(weapon);
            Assert.That(updatedWeapon, Is.EqualTo(weapon));
            Assert.That(weapon.Magic.SpecialAbilities.Count(), Is.EqualTo(2));
            Assert.That(weapon.Damages, Has.Count.EqualTo(3));
            Assert.That(weapon.Damages[0].Roll, Is.EqualTo("my roll"));
            Assert.That(weapon.Damages[0].Type, Is.EqualTo(WeaponDamageType));
            Assert.That(weapon.Damages[0].Condition, Is.Empty);
            Assert.That(weapon.Damages[1].Roll, Is.EqualTo("some"));
            Assert.That(weapon.Damages[1].Type, Is.EqualTo("plasma"));
            Assert.That(weapon.Damages[1].Condition, Is.EqualTo("my condition"));
            Assert.That(weapon.Damages[2].Roll, Is.EqualTo("a bit"));
            Assert.That(weapon.Damages[2].Type, Is.EqualTo("ether"));
            Assert.That(weapon.Damages[2].Condition, Is.EqualTo("my other condition"));
            Assert.That(weapon.CriticalDamages, Has.Count.EqualTo(1));
            Assert.That(weapon.CriticalDamages[0].Roll, Is.EqualTo("my crit roll"));
            Assert.That(weapon.CriticalDamages[0].Type, Is.EqualTo(WeaponDamageType_Crit));
            Assert.That(weapon.CriticalDamages[0].Condition, Is.Empty);
            Assert.That(weapon.SecondaryDamages, Has.Count.EqualTo(3));
            Assert.That(weapon.SecondaryDamages[0].Roll, Is.EqualTo("jab"));
            Assert.That(weapon.SecondaryDamages[0].Type, Is.EqualTo("punch"));
            Assert.That(weapon.SecondaryDamages[0].Condition, Is.Empty);
            Assert.That(weapon.SecondaryDamages[1].Roll, Is.EqualTo("some"));
            Assert.That(weapon.SecondaryDamages[1].Type, Is.EqualTo("plasma"));
            Assert.That(weapon.SecondaryDamages[1].Condition, Is.EqualTo("my condition"));
            Assert.That(weapon.SecondaryDamages[2].Roll, Is.EqualTo("a bit"));
            Assert.That(weapon.SecondaryDamages[2].Type, Is.EqualTo("ether"));
            Assert.That(weapon.SecondaryDamages[2].Condition, Is.EqualTo("my other condition"));
            Assert.That(weapon.SecondaryCriticalDamages, Has.Count.EqualTo(1));
            Assert.That(weapon.SecondaryCriticalDamages[0].Roll, Is.EqualTo("cross"));
            Assert.That(weapon.SecondaryCriticalDamages[0].Type, Is.EqualTo("punch"));
            Assert.That(weapon.SecondaryCriticalDamages[0].Condition, Is.Empty);
        }

        [Test]
        public void ApplyAbilitiesToWeapon_Damages_DoubleWeapon_SameEnhancement_EmptyDamageTypes()
        {
            var ability = new SpecialAbility
            {
                Name = "my name"
            };
            ability.Damages.Add(new Damage { Roll = "some", Type = string.Empty, Condition = "my condition" });
            ability.Damages.Add(new Damage { Roll = "a bit", Type = "ether", Condition = "my other condition" });

            var weapon = CreateWeapon(true);
            weapon.Magic.SpecialAbilities = [new SpecialAbility(), ability];
            weapon.SecondaryMagicBonus = weapon.Magic.Bonus;
            weapon.SecondaryHasAbilities = true;

            var updatedWeapon = specialAbilitiesGenerator.ApplyAbilitiesToWeapon(weapon);
            Assert.That(updatedWeapon, Is.EqualTo(weapon));
            Assert.That(weapon.Magic.SpecialAbilities.Count(), Is.EqualTo(2));
            Assert.That(weapon.Damages, Has.Count.EqualTo(3));
            Assert.That(weapon.Damages[0].Roll, Is.EqualTo("my roll"));
            Assert.That(weapon.Damages[0].Type, Is.EqualTo(WeaponDamageType));
            Assert.That(weapon.Damages[0].Condition, Is.Empty);
            Assert.That(weapon.Damages[1].Roll, Is.EqualTo("some"));
            Assert.That(weapon.Damages[1].Type, Is.EqualTo(WeaponDamageType));
            Assert.That(weapon.Damages[1].Condition, Is.EqualTo("my condition"));
            Assert.That(weapon.Damages[2].Roll, Is.EqualTo("a bit"));
            Assert.That(weapon.Damages[2].Type, Is.EqualTo("ether"));
            Assert.That(weapon.Damages[2].Condition, Is.EqualTo("my other condition"));
            Assert.That(weapon.CriticalDamages, Has.Count.EqualTo(1));
            Assert.That(weapon.CriticalDamages[0].Roll, Is.EqualTo("my crit roll"));
            Assert.That(weapon.CriticalDamages[0].Type, Is.EqualTo(WeaponDamageType_Crit));
            Assert.That(weapon.CriticalDamages[0].Condition, Is.Empty);
            Assert.That(weapon.SecondaryDamages, Has.Count.EqualTo(3));
            Assert.That(weapon.SecondaryDamages[0].Roll, Is.EqualTo("jab"));
            Assert.That(weapon.SecondaryDamages[0].Type, Is.EqualTo("punch"));
            Assert.That(weapon.SecondaryDamages[0].Condition, Is.Empty);
            Assert.That(weapon.SecondaryDamages[1].Roll, Is.EqualTo("some"));
            Assert.That(weapon.SecondaryDamages[1].Type, Is.EqualTo("punch"));
            Assert.That(weapon.SecondaryDamages[1].Condition, Is.EqualTo("my condition"));
            Assert.That(weapon.SecondaryDamages[2].Roll, Is.EqualTo("a bit"));
            Assert.That(weapon.SecondaryDamages[2].Type, Is.EqualTo("ether"));
            Assert.That(weapon.SecondaryDamages[2].Condition, Is.EqualTo("my other condition"));
            Assert.That(weapon.SecondaryCriticalDamages, Has.Count.EqualTo(1));
            Assert.That(weapon.SecondaryCriticalDamages[0].Roll, Is.EqualTo("cross"));
            Assert.That(weapon.SecondaryCriticalDamages[0].Type, Is.EqualTo("punch"));
            Assert.That(weapon.SecondaryCriticalDamages[0].Condition, Is.Empty);
        }

        [Test]
        public void ApplyAbilitiesToWeapon_CriticalDamages_DoubleWeapon_SameEnhancement()
        {
            mockDamageDataSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.AbilityDamages, "my nameother crit"))
                .Returns(
                [
                    new DamageDataSelection { Type = "slush", Roll = "additional" },
                    new DamageDataSelection { Type = "ice", Roll = "too much" },
                ]);

            var ability = new SpecialAbility
            {
                Name = "my name",
                CriticalDamages =
                [
                    new Damage { Roll = "more", Type = "plasma", Condition = "my crit condition" },
                    new Damage { Roll = "a lot", Type = "ether" },
                ]
            };

            var weapon = CreateWeapon(true);
            weapon.Magic.SpecialAbilities = [new SpecialAbility(), ability];
            weapon.SecondaryMagicBonus = weapon.Magic.Bonus;
            weapon.SecondaryHasAbilities = true;

            var updatedWeapon = specialAbilitiesGenerator.ApplyAbilitiesToWeapon(weapon);
            Assert.That(updatedWeapon, Is.EqualTo(weapon));
            Assert.That(weapon.Magic.SpecialAbilities.Count(), Is.EqualTo(2));
            Assert.That(weapon.Damages, Has.Count.EqualTo(1));
            Assert.That(weapon.Damages[0].Roll, Is.EqualTo("my roll"));
            Assert.That(weapon.Damages[0].Type, Is.EqualTo(WeaponDamageType));
            Assert.That(weapon.Damages[0].Condition, Is.Empty);
            Assert.That(weapon.CriticalDamages, Has.Count.EqualTo(3));
            Assert.That(weapon.CriticalDamages[0].Roll, Is.EqualTo("my crit roll"));
            Assert.That(weapon.CriticalDamages[0].Type, Is.EqualTo(WeaponDamageType_Crit));
            Assert.That(weapon.CriticalDamages[0].Condition, Is.Empty);
            Assert.That(weapon.CriticalDamages[1].Roll, Is.EqualTo("more"));
            Assert.That(weapon.CriticalDamages[1].Type, Is.EqualTo("plasma"));
            Assert.That(weapon.CriticalDamages[1].Condition, Is.EqualTo("my crit condition"));
            Assert.That(weapon.CriticalDamages[2].Roll, Is.EqualTo("a lot"));
            Assert.That(weapon.CriticalDamages[2].Type, Is.EqualTo("ether"));
            Assert.That(weapon.CriticalDamages[2].Condition, Is.Empty);
            Assert.That(weapon.SecondaryDamages, Has.Count.EqualTo(1));
            Assert.That(weapon.SecondaryDamages[0].Roll, Is.EqualTo("jab"));
            Assert.That(weapon.SecondaryDamages[0].Type, Is.EqualTo("punch"));
            Assert.That(weapon.SecondaryDamages[0].Condition, Is.Empty);
            Assert.That(weapon.SecondaryCriticalDamages, Has.Count.EqualTo(3));
            Assert.That(weapon.SecondaryCriticalDamages[0].Roll, Is.EqualTo("cross"));
            Assert.That(weapon.SecondaryCriticalDamages[0].Type, Is.EqualTo("punch"));
            Assert.That(weapon.SecondaryCriticalDamages[0].Condition, Is.Empty);
            Assert.That(weapon.SecondaryCriticalDamages[1].Roll, Is.EqualTo("more"));
            Assert.That(weapon.SecondaryCriticalDamages[1].Type, Is.EqualTo("plasma"));
            Assert.That(weapon.SecondaryCriticalDamages[1].Condition, Is.EqualTo("my crit condition"));
            Assert.That(weapon.SecondaryCriticalDamages[2].Roll, Is.EqualTo("a lot"));
            Assert.That(weapon.SecondaryCriticalDamages[2].Type, Is.EqualTo("ether"));
            Assert.That(weapon.SecondaryCriticalDamages[2].Condition, Is.Empty);
        }

        [Test]
        public void ApplyAbilitiesToWeapon_DamagesAndCriticalDamages_DoubleWeapon_SameEnhancement()
        {
            mockDamageDataSelector
                .Setup(s => s.SelectFrom(Config.Name, TableNameConstants.Collections.AbilityDamages, "my nameother crit"))
                .Returns(
                [
                    new DamageDataSelection { Type = "slush", Roll = "additional" },
                    new DamageDataSelection { Type = "ice", Roll = "too much" },
                ]);

            var ability = new SpecialAbility
            {
                Name = "my name",
                Damages =
                [
                    new Damage { Roll = "some", Type = "plasma", Condition = "my condition" },
                    new Damage { Roll = "a bit", Type = "ether", Condition = "my other condition" },
                ],
                CriticalDamages =
                [
                    new Damage { Roll = "more", Type = "plasma", Condition = "my crit condition" },
                    new Damage { Roll = "a lot", Type = "ether", Condition = "my other crit condition" },
                ],
            };

            var weapon = CreateWeapon(true);
            weapon.Magic.SpecialAbilities = [new SpecialAbility(), ability];
            weapon.SecondaryMagicBonus = weapon.Magic.Bonus;
            weapon.SecondaryHasAbilities = true;

            var updatedWeapon = specialAbilitiesGenerator.ApplyAbilitiesToWeapon(weapon);
            Assert.That(updatedWeapon, Is.EqualTo(weapon));
            Assert.That(weapon.Magic.SpecialAbilities.Count(), Is.EqualTo(2));
            Assert.That(weapon.Damages, Has.Count.EqualTo(3));
            Assert.That(weapon.Damages[0].Roll, Is.EqualTo("my roll"));
            Assert.That(weapon.Damages[0].Type, Is.EqualTo(WeaponDamageType));
            Assert.That(weapon.Damages[0].Condition, Is.Empty);
            Assert.That(weapon.Damages[1].Roll, Is.EqualTo("some"));
            Assert.That(weapon.Damages[1].Type, Is.EqualTo("plasma"));
            Assert.That(weapon.Damages[1].Condition, Is.EqualTo("my condition"));
            Assert.That(weapon.Damages[2].Roll, Is.EqualTo("a bit"));
            Assert.That(weapon.Damages[2].Type, Is.EqualTo("ether"));
            Assert.That(weapon.Damages[2].Condition, Is.EqualTo("my other condition"));
            Assert.That(weapon.CriticalDamages, Has.Count.EqualTo(3));
            Assert.That(weapon.CriticalDamages[0].Roll, Is.EqualTo("my crit roll"));
            Assert.That(weapon.CriticalDamages[0].Type, Is.EqualTo(WeaponDamageType_Crit));
            Assert.That(weapon.CriticalDamages[0].Condition, Is.Empty);
            Assert.That(weapon.CriticalDamages[1].Roll, Is.EqualTo("more"));
            Assert.That(weapon.CriticalDamages[1].Type, Is.EqualTo("plasma"));
            Assert.That(weapon.CriticalDamages[1].Condition, Is.EqualTo("my crit condition"));
            Assert.That(weapon.CriticalDamages[2].Roll, Is.EqualTo("a lot"));
            Assert.That(weapon.CriticalDamages[2].Type, Is.EqualTo("ether"));
            Assert.That(weapon.CriticalDamages[2].Condition, Is.EqualTo("my other crit condition"));
            Assert.That(weapon.SecondaryDamages, Has.Count.EqualTo(3));
            Assert.That(weapon.SecondaryDamages[0].Roll, Is.EqualTo("jab"));
            Assert.That(weapon.SecondaryDamages[0].Type, Is.EqualTo("punch"));
            Assert.That(weapon.SecondaryDamages[0].Condition, Is.Empty);
            Assert.That(weapon.SecondaryDamages[1].Roll, Is.EqualTo("some"));
            Assert.That(weapon.SecondaryDamages[1].Type, Is.EqualTo("plasma"));
            Assert.That(weapon.SecondaryDamages[1].Condition, Is.EqualTo("my condition"));
            Assert.That(weapon.SecondaryDamages[2].Roll, Is.EqualTo("a bit"));
            Assert.That(weapon.SecondaryDamages[2].Type, Is.EqualTo("ether"));
            Assert.That(weapon.SecondaryDamages[2].Condition, Is.EqualTo("my other condition"));
            Assert.That(weapon.SecondaryCriticalDamages, Has.Count.EqualTo(3));
            Assert.That(weapon.SecondaryCriticalDamages[0].Roll, Is.EqualTo("cross"));
            Assert.That(weapon.SecondaryCriticalDamages[0].Type, Is.EqualTo("punch"));
            Assert.That(weapon.SecondaryCriticalDamages[0].Condition, Is.Empty);
            Assert.That(weapon.SecondaryCriticalDamages[1].Roll, Is.EqualTo("more"));
            Assert.That(weapon.SecondaryCriticalDamages[1].Type, Is.EqualTo("plasma"));
            Assert.That(weapon.SecondaryCriticalDamages[1].Condition, Is.EqualTo("my crit condition"));
            Assert.That(weapon.SecondaryCriticalDamages[2].Roll, Is.EqualTo("a lot"));
            Assert.That(weapon.SecondaryCriticalDamages[2].Type, Is.EqualTo("ether"));
            Assert.That(weapon.SecondaryCriticalDamages[2].Condition, Is.EqualTo("my other crit condition"));
        }

        [Test]
        public void ApplyAbilitiesToWeapon_Damages_DoubleWeapon_LesserEnhancement()
        {
            var ability = new SpecialAbility
            {
                Name = "my name"
            };
            ability.Damages.Add(new Damage { Roll = "some", Type = "plasma", Condition = "my condition" });
            ability.Damages.Add(new Damage { Roll = "a bit", Type = "ether", Condition = "my other condition" });

            var weapon = CreateWeapon(true);
            weapon.Magic.SpecialAbilities = [new SpecialAbility(), ability];
            weapon.SecondaryMagicBonus = 42;
            weapon.SecondaryHasAbilities = false;

            var updatedWeapon = specialAbilitiesGenerator.ApplyAbilitiesToWeapon(weapon);
            Assert.That(updatedWeapon, Is.EqualTo(weapon));
            Assert.That(weapon.Magic.SpecialAbilities.Count(), Is.EqualTo(2));
            Assert.That(weapon.Damages, Has.Count.EqualTo(3));
            Assert.That(weapon.Damages[0].Roll, Is.EqualTo("my roll"));
            Assert.That(weapon.Damages[0].Type, Is.EqualTo(WeaponDamageType));
            Assert.That(weapon.Damages[0].Condition, Is.Empty);
            Assert.That(weapon.Damages[1].Roll, Is.EqualTo("some"));
            Assert.That(weapon.Damages[1].Type, Is.EqualTo("plasma"));
            Assert.That(weapon.Damages[1].Condition, Is.EqualTo("my condition"));
            Assert.That(weapon.Damages[2].Roll, Is.EqualTo("a bit"));
            Assert.That(weapon.Damages[2].Type, Is.EqualTo("ether"));
            Assert.That(weapon.Damages[2].Condition, Is.EqualTo("my other condition"));
            Assert.That(weapon.CriticalDamages, Has.Count.EqualTo(1));
            Assert.That(weapon.CriticalDamages[0].Roll, Is.EqualTo("my crit roll"));
            Assert.That(weapon.CriticalDamages[0].Type, Is.EqualTo(WeaponDamageType_Crit));
            Assert.That(weapon.CriticalDamages[0].Condition, Is.Empty);
            Assert.That(weapon.SecondaryDamages, Has.Count.EqualTo(1));
            Assert.That(weapon.SecondaryDamages[0].Roll, Is.EqualTo("jab"));
            Assert.That(weapon.SecondaryDamages[0].Type, Is.EqualTo("punch"));
            Assert.That(weapon.SecondaryDamages[0].Condition, Is.Empty);
            Assert.That(weapon.SecondaryCriticalDamages, Has.Count.EqualTo(1));
            Assert.That(weapon.SecondaryCriticalDamages[0].Roll, Is.EqualTo("cross"));
            Assert.That(weapon.SecondaryCriticalDamages[0].Type, Is.EqualTo("punch"));
            Assert.That(weapon.SecondaryCriticalDamages[0].Condition, Is.Empty);
        }

        [Test]
        public void ApplyAbilitiesToWeapon_CriticalDamages_DoubleWeapon_LesserEnhancement()
        {
            var ability = new SpecialAbility
            {
                Name = "my name",
                CriticalDamages =
                [
                    new Damage { Roll = "more", Type = "plasma", Condition = "my crit condition" },
                    new Damage { Roll = "a lot", Type = "ether", Condition = "my other crit condition" },
                ]
            };

            var weapon = CreateWeapon(true);
            weapon.Magic.SpecialAbilities = [new SpecialAbility(), ability];
            weapon.SecondaryMagicBonus = 42;
            weapon.SecondaryHasAbilities = false;

            var updatedWeapon = specialAbilitiesGenerator.ApplyAbilitiesToWeapon(weapon);
            Assert.That(updatedWeapon, Is.EqualTo(weapon));
            Assert.That(weapon.Magic.SpecialAbilities.Count(), Is.EqualTo(2));
            Assert.That(weapon.Damages, Has.Count.EqualTo(1));
            Assert.That(weapon.Damages[0].Roll, Is.EqualTo("my roll"));
            Assert.That(weapon.Damages[0].Type, Is.EqualTo(WeaponDamageType));
            Assert.That(weapon.Damages[0].Condition, Is.Empty);
            Assert.That(weapon.CriticalDamages, Has.Count.EqualTo(3));
            Assert.That(weapon.CriticalDamages[0].Roll, Is.EqualTo("my crit roll"));
            Assert.That(weapon.CriticalDamages[0].Type, Is.EqualTo(WeaponDamageType_Crit));
            Assert.That(weapon.CriticalDamages[0].Condition, Is.Empty);
            Assert.That(weapon.CriticalDamages[1].Roll, Is.EqualTo("more"));
            Assert.That(weapon.CriticalDamages[1].Type, Is.EqualTo("plasma"));
            Assert.That(weapon.CriticalDamages[1].Condition, Is.EqualTo("my crit condition"));
            Assert.That(weapon.CriticalDamages[2].Roll, Is.EqualTo("a lot"));
            Assert.That(weapon.CriticalDamages[2].Type, Is.EqualTo("ether"));
            Assert.That(weapon.CriticalDamages[2].Condition, Is.EqualTo("my other crit condition"));
            Assert.That(weapon.SecondaryDamages, Has.Count.EqualTo(1));
            Assert.That(weapon.SecondaryDamages[0].Roll, Is.EqualTo("jab"));
            Assert.That(weapon.SecondaryDamages[0].Type, Is.EqualTo("punch"));
            Assert.That(weapon.SecondaryDamages[0].Condition, Is.Empty);
            Assert.That(weapon.SecondaryCriticalDamages, Has.Count.EqualTo(1));
            Assert.That(weapon.SecondaryCriticalDamages[0].Roll, Is.EqualTo("cross"));
            Assert.That(weapon.SecondaryCriticalDamages[0].Type, Is.EqualTo("punch"));
            Assert.That(weapon.SecondaryCriticalDamages[0].Condition, Is.Empty);
        }

        [Test]
        public void ApplyAbilitiesToWeapon_DamagesAndCriticalDamages_DoubleWeapon_LesserEnhancement()
        {
            var ability = new SpecialAbility
            {
                Name = "my name",
                Damages =
                [
                    new Damage { Roll = "some", Type = "plasma", Condition = "my condition" },
                    new Damage { Roll = "a bit", Type = "ether", Condition = "my other condition" },
                ],
                CriticalDamages =
                [
                    new Damage { Roll = "more", Type = "plasma", Condition = "my crit condition" },
                    new Damage { Roll = "a lot", Type = "ether", Condition = "my other crit condition" },
                ],
            };

            var weapon = CreateWeapon(true);
            weapon.Magic.SpecialAbilities = [new SpecialAbility(), ability];
            weapon.SecondaryMagicBonus = 42;
            weapon.SecondaryHasAbilities = false;

            var updatedWeapon = specialAbilitiesGenerator.ApplyAbilitiesToWeapon(weapon);
            Assert.That(updatedWeapon, Is.EqualTo(weapon));
            Assert.That(weapon.Magic.SpecialAbilities.Count(), Is.EqualTo(2));
            Assert.That(weapon.Damages, Has.Count.EqualTo(3));
            Assert.That(weapon.Damages[0].Roll, Is.EqualTo("my roll"));
            Assert.That(weapon.Damages[0].Type, Is.EqualTo(WeaponDamageType));
            Assert.That(weapon.Damages[0].Condition, Is.Empty);
            Assert.That(weapon.Damages[1].Roll, Is.EqualTo("some"));
            Assert.That(weapon.Damages[1].Type, Is.EqualTo("plasma"));
            Assert.That(weapon.Damages[1].Condition, Is.EqualTo("my condition"));
            Assert.That(weapon.Damages[2].Roll, Is.EqualTo("a bit"));
            Assert.That(weapon.Damages[2].Type, Is.EqualTo("ether"));
            Assert.That(weapon.Damages[2].Condition, Is.EqualTo("my other condition"));
            Assert.That(weapon.CriticalDamages, Has.Count.EqualTo(3));
            Assert.That(weapon.CriticalDamages[0].Roll, Is.EqualTo("my crit roll"));
            Assert.That(weapon.CriticalDamages[0].Type, Is.EqualTo(WeaponDamageType_Crit));
            Assert.That(weapon.CriticalDamages[0].Condition, Is.Empty);
            Assert.That(weapon.CriticalDamages[1].Roll, Is.EqualTo("more"));
            Assert.That(weapon.CriticalDamages[1].Type, Is.EqualTo("plasma"));
            Assert.That(weapon.CriticalDamages[1].Condition, Is.EqualTo("my crit condition"));
            Assert.That(weapon.CriticalDamages[2].Roll, Is.EqualTo("a lot"));
            Assert.That(weapon.CriticalDamages[2].Type, Is.EqualTo("ether"));
            Assert.That(weapon.CriticalDamages[2].Condition, Is.EqualTo("my other crit condition"));
            Assert.That(weapon.SecondaryDamages, Has.Count.EqualTo(1));
            Assert.That(weapon.SecondaryDamages[0].Roll, Is.EqualTo("jab"));
            Assert.That(weapon.SecondaryDamages[0].Type, Is.EqualTo("punch"));
            Assert.That(weapon.SecondaryDamages[0].Condition, Is.Empty);
            Assert.That(weapon.SecondaryCriticalDamages, Has.Count.EqualTo(1));
            Assert.That(weapon.SecondaryCriticalDamages[0].Roll, Is.EqualTo("cross"));
            Assert.That(weapon.SecondaryCriticalDamages[0].Type, Is.EqualTo("punch"));
            Assert.That(weapon.SecondaryCriticalDamages[0].Condition, Is.Empty);
        }

        [Test]
        public void ApplyAbilitiesToWeapon_Damages_MultipleAbilities()
        {
            var ability1 = new SpecialAbility();
            ability1.Damages.Add(new Damage { Roll = "some", Type = "plasma", Condition = "my condition" });
            ability1.Damages.Add(new Damage { Roll = "a bit", Type = "ether", Condition = "my other condition" });

            ability1.CriticalDamages =
            [
                new Damage { Roll = "more", Type = "plasma", Condition = "my crit condition" },
                new Damage { Roll = "a lot", Type = "ether", Condition = "my other crit condition" },
            ];

            var ability2 = new SpecialAbility();
            ability2.Damages.Add(new Damage { Roll = "a touch", Type = "here", Condition = "my condition 2" });
            ability2.Damages.Add(new Damage { Roll = "another touch", Type = "there", Condition = "my other condition 2" });

            ability2.CriticalDamages =
            [
                new Damage { Roll = "MORE", Type = "here", Condition = "my crit condition 2" },
                new Damage { Roll = "a ton", Type = "there", Condition = "my other crit condition 2" },
            ];

            var weapon = CreateWeapon();
            weapon.Magic.SpecialAbilities = [new SpecialAbility(), ability1, ability2];

            var updatedWeapon = specialAbilitiesGenerator.ApplyAbilitiesToWeapon(weapon);
            Assert.That(updatedWeapon, Is.EqualTo(weapon));
            Assert.That(weapon.Magic.SpecialAbilities.Count(), Is.EqualTo(3));
            Assert.That(weapon.Damages, Has.Count.EqualTo(5));
            Assert.That(weapon.Damages[0].Roll, Is.EqualTo("my roll"));
            Assert.That(weapon.Damages[0].Type, Is.EqualTo(WeaponDamageType));
            Assert.That(weapon.Damages[0].Condition, Is.Empty);
            Assert.That(weapon.Damages[1].Roll, Is.EqualTo("some"));
            Assert.That(weapon.Damages[1].Type, Is.EqualTo("plasma"));
            Assert.That(weapon.Damages[1].Condition, Is.EqualTo("my condition"));
            Assert.That(weapon.Damages[2].Roll, Is.EqualTo("a bit"));
            Assert.That(weapon.Damages[2].Type, Is.EqualTo("ether"));
            Assert.That(weapon.Damages[2].Condition, Is.EqualTo("my other condition"));
            Assert.That(weapon.Damages[3].Roll, Is.EqualTo("a touch"));
            Assert.That(weapon.Damages[3].Type, Is.EqualTo("here"));
            Assert.That(weapon.Damages[3].Condition, Is.EqualTo("my condition 2"));
            Assert.That(weapon.Damages[4].Roll, Is.EqualTo("another touch"));
            Assert.That(weapon.Damages[4].Type, Is.EqualTo("there"));
            Assert.That(weapon.Damages[4].Condition, Is.EqualTo("my other condition 2"));
            Assert.That(weapon.CriticalDamages, Has.Count.EqualTo(5));
            Assert.That(weapon.CriticalDamages[0].Roll, Is.EqualTo("my crit roll"));
            Assert.That(weapon.CriticalDamages[0].Type, Is.EqualTo(WeaponDamageType_Crit));
            Assert.That(weapon.CriticalDamages[0].Condition, Is.Empty);
            Assert.That(weapon.CriticalDamages[1].Roll, Is.EqualTo("more"));
            Assert.That(weapon.CriticalDamages[1].Type, Is.EqualTo("plasma"));
            Assert.That(weapon.CriticalDamages[1].Condition, Is.EqualTo("my crit condition"));
            Assert.That(weapon.CriticalDamages[2].Roll, Is.EqualTo("a lot"));
            Assert.That(weapon.CriticalDamages[2].Type, Is.EqualTo("ether"));
            Assert.That(weapon.CriticalDamages[2].Condition, Is.EqualTo("my other crit condition"));
            Assert.That(weapon.CriticalDamages[3].Roll, Is.EqualTo("MORE"));
            Assert.That(weapon.CriticalDamages[3].Type, Is.EqualTo("here"));
            Assert.That(weapon.CriticalDamages[3].Condition, Is.EqualTo("my crit condition 2"));
            Assert.That(weapon.CriticalDamages[4].Roll, Is.EqualTo("a ton"));
            Assert.That(weapon.CriticalDamages[4].Type, Is.EqualTo("there"));
            Assert.That(weapon.CriticalDamages[4].Condition, Is.EqualTo("my other crit condition 2"));
            Assert.That(weapon.SecondaryDamages, Is.Empty);
            Assert.That(weapon.SecondaryCriticalDamages, Is.Empty);
        }

        [Test]
        public void ApplyAbilitiesToWeapon_Damages_MultipleAbilities_DoubleWeapon()
        {
            var ability1 = new SpecialAbility();
            ability1.Damages.Add(new Damage { Roll = "some", Type = "plasma", Condition = "my condition" });
            ability1.Damages.Add(new Damage { Roll = "a bit", Type = "ether", Condition = "my other condition" });

            ability1.CriticalDamages =
            [
                new Damage { Roll = "more", Type = "plasma", Condition = "my crit condition" },
                new Damage { Roll = "a lot", Type = "ether", Condition = "my other crit condition" },
            ];

            var ability2 = new SpecialAbility();
            ability2.Damages.Add(new Damage { Roll = "a touch", Type = "here", Condition = "my condition 2" });
            ability2.Damages.Add(new Damage { Roll = "another touch", Type = "there", Condition = "my other condition 2" });

            ability2.CriticalDamages =
            [
                new Damage { Roll = "MORE", Type = "here", Condition = "my crit condition 2" },
                new Damage { Roll = "a ton", Type = "there", Condition = "my other crit condition 2" },
            ];

            var weapon = CreateWeapon(true);
            weapon.Magic.SpecialAbilities = [new SpecialAbility(), ability1, ability2];
            weapon.SecondaryMagicBonus = weapon.Magic.Bonus;
            weapon.SecondaryHasAbilities = true;

            var updatedWeapon = specialAbilitiesGenerator.ApplyAbilitiesToWeapon(weapon);
            Assert.That(updatedWeapon, Is.EqualTo(weapon));
            Assert.That(weapon.Magic.SpecialAbilities.Count(), Is.EqualTo(3));
            Assert.That(weapon.Damages, Has.Count.EqualTo(5));
            Assert.That(weapon.Damages[0].Roll, Is.EqualTo("my roll"));
            Assert.That(weapon.Damages[0].Type, Is.EqualTo(WeaponDamageType));
            Assert.That(weapon.Damages[0].Condition, Is.Empty);
            Assert.That(weapon.Damages[1].Roll, Is.EqualTo("some"));
            Assert.That(weapon.Damages[1].Type, Is.EqualTo("plasma"));
            Assert.That(weapon.Damages[1].Condition, Is.EqualTo("my condition"));
            Assert.That(weapon.Damages[2].Roll, Is.EqualTo("a bit"));
            Assert.That(weapon.Damages[2].Type, Is.EqualTo("ether"));
            Assert.That(weapon.Damages[2].Condition, Is.EqualTo("my other condition"));
            Assert.That(weapon.Damages[3].Roll, Is.EqualTo("a touch"));
            Assert.That(weapon.Damages[3].Type, Is.EqualTo("here"));
            Assert.That(weapon.Damages[3].Condition, Is.EqualTo("my condition 2"));
            Assert.That(weapon.Damages[4].Roll, Is.EqualTo("another touch"));
            Assert.That(weapon.Damages[4].Type, Is.EqualTo("there"));
            Assert.That(weapon.Damages[4].Condition, Is.EqualTo("my other condition 2"));
            Assert.That(weapon.CriticalDamages, Has.Count.EqualTo(5));
            Assert.That(weapon.CriticalDamages[0].Roll, Is.EqualTo("my crit roll"));
            Assert.That(weapon.CriticalDamages[0].Type, Is.EqualTo(WeaponDamageType_Crit));
            Assert.That(weapon.CriticalDamages[0].Condition, Is.Empty);
            Assert.That(weapon.CriticalDamages[1].Roll, Is.EqualTo("more"));
            Assert.That(weapon.CriticalDamages[1].Type, Is.EqualTo("plasma"));
            Assert.That(weapon.CriticalDamages[1].Condition, Is.EqualTo("my crit condition"));
            Assert.That(weapon.CriticalDamages[2].Roll, Is.EqualTo("a lot"));
            Assert.That(weapon.CriticalDamages[2].Type, Is.EqualTo("ether"));
            Assert.That(weapon.CriticalDamages[2].Condition, Is.EqualTo("my other crit condition"));
            Assert.That(weapon.CriticalDamages[3].Roll, Is.EqualTo("MORE"));
            Assert.That(weapon.CriticalDamages[3].Type, Is.EqualTo("here"));
            Assert.That(weapon.CriticalDamages[3].Condition, Is.EqualTo("my crit condition 2"));
            Assert.That(weapon.CriticalDamages[4].Roll, Is.EqualTo("a ton"));
            Assert.That(weapon.CriticalDamages[4].Type, Is.EqualTo("there"));
            Assert.That(weapon.CriticalDamages[4].Condition, Is.EqualTo("my other crit condition 2"));
            Assert.That(weapon.SecondaryDamages, Has.Count.EqualTo(5));
            Assert.That(weapon.SecondaryDamages[0].Roll, Is.EqualTo("jab"));
            Assert.That(weapon.SecondaryDamages[0].Type, Is.EqualTo("punch"));
            Assert.That(weapon.SecondaryDamages[0].Condition, Is.Empty);
            Assert.That(weapon.SecondaryDamages[1].Roll, Is.EqualTo("some"));
            Assert.That(weapon.SecondaryDamages[1].Type, Is.EqualTo("plasma"));
            Assert.That(weapon.SecondaryDamages[1].Condition, Is.EqualTo("my condition"));
            Assert.That(weapon.SecondaryDamages[2].Roll, Is.EqualTo("a bit"));
            Assert.That(weapon.SecondaryDamages[2].Type, Is.EqualTo("ether"));
            Assert.That(weapon.SecondaryDamages[2].Condition, Is.EqualTo("my other condition"));
            Assert.That(weapon.SecondaryDamages[3].Roll, Is.EqualTo("a touch"));
            Assert.That(weapon.SecondaryDamages[3].Type, Is.EqualTo("here"));
            Assert.That(weapon.SecondaryDamages[3].Condition, Is.EqualTo("my condition 2"));
            Assert.That(weapon.SecondaryDamages[4].Roll, Is.EqualTo("another touch"));
            Assert.That(weapon.SecondaryDamages[4].Type, Is.EqualTo("there"));
            Assert.That(weapon.SecondaryDamages[4].Condition, Is.EqualTo("my other condition 2"));
            Assert.That(weapon.SecondaryCriticalDamages, Has.Count.EqualTo(5));
            Assert.That(weapon.SecondaryCriticalDamages[0].Roll, Is.EqualTo("cross"));
            Assert.That(weapon.SecondaryCriticalDamages[0].Type, Is.EqualTo("punch"));
            Assert.That(weapon.SecondaryCriticalDamages[0].Condition, Is.Empty);
            Assert.That(weapon.SecondaryCriticalDamages[1].Roll, Is.EqualTo("more"));
            Assert.That(weapon.SecondaryCriticalDamages[1].Type, Is.EqualTo("plasma"));
            Assert.That(weapon.SecondaryCriticalDamages[1].Condition, Is.EqualTo("my crit condition"));
            Assert.That(weapon.SecondaryCriticalDamages[2].Roll, Is.EqualTo("a lot"));
            Assert.That(weapon.SecondaryCriticalDamages[2].Type, Is.EqualTo("ether"));
            Assert.That(weapon.SecondaryCriticalDamages[2].Condition, Is.EqualTo("my other crit condition"));
            Assert.That(weapon.SecondaryCriticalDamages[3].Roll, Is.EqualTo("MORE"));
            Assert.That(weapon.SecondaryCriticalDamages[3].Type, Is.EqualTo("here"));
            Assert.That(weapon.SecondaryCriticalDamages[3].Condition, Is.EqualTo("my crit condition 2"));
            Assert.That(weapon.SecondaryCriticalDamages[4].Roll, Is.EqualTo("a ton"));
            Assert.That(weapon.SecondaryCriticalDamages[4].Type, Is.EqualTo("there"));
            Assert.That(weapon.SecondaryCriticalDamages[4].Condition, Is.EqualTo("my other crit condition 2"));
        }

        [TestCase(1, "19-20")]
        [TestCase(2, "17-20")]
        [TestCase(3, "15-20")]
        public void ApplyAbilitiesToWeapon_Keen(int originalThreatRange, string keenSummary)
        {
            var keen = new SpecialAbility { Name = SpecialAbilityConstants.Keen };
            var weapon = CreateWeapon();
            weapon.Magic.SpecialAbilities = [new SpecialAbility(), keen];
            weapon.ThreatRange = originalThreatRange;

            var updatedWeapon = specialAbilitiesGenerator.ApplyAbilitiesToWeapon(weapon);
            Assert.That(updatedWeapon, Is.EqualTo(weapon));
            Assert.That(weapon.Magic.SpecialAbilities.Count(), Is.EqualTo(2));
            Assert.That(weapon.ThreatRange, Is.EqualTo(originalThreatRange * 2));
            Assert.That(weapon.ThreatRangeSummary, Is.EqualTo(keenSummary));
        }

        [Test]
        public void ApplyAbilitiesToWeapon_SpellStoringWeaponHasSpellIfSelectorSaysSo()
        {
            mockPercentileSelector.Setup(s => s.SelectFrom<bool>(Config.Name, TableNameConstants.Percentiles.SpellStoringContainsSpell)).Returns(true);

            var weapon = CreateWeapon();
            weapon.Magic.SpecialAbilities = [new SpecialAbility { Name = SpecialAbilityConstants.SpellStoring }];

            mockSpellGenerator.Setup(g => g.GenerateType()).Returns("spell type");
            mockSpellGenerator.Setup(g => g.GenerateLevel(PowerConstants.Minor)).Returns(1337);
            mockSpellGenerator.Setup(g => g.Generate("spell type", 1337)).Returns("spell");

            var updatedWeapon = specialAbilitiesGenerator.ApplyAbilitiesToWeapon(weapon);
            Assert.That(updatedWeapon, Is.EqualTo(weapon));
            Assert.That(weapon.Contents, Contains.Item("spell"));
        }

        [Test]
        public void ApplyAbilitiesToWeapon_SpellStoringWeaponDoesNotHaveSpellIfSelectorSaysSo()
        {
            mockPercentileSelector.Setup(s => s.SelectFrom<bool>(Config.Name, TableNameConstants.Percentiles.SpellStoringContainsSpell)).Returns(false);

            var weapon = CreateWeapon();
            weapon.Magic.SpecialAbilities = [new SpecialAbility { Name = SpecialAbilityConstants.SpellStoring }];

            mockSpellGenerator.Setup(g => g.GenerateType()).Returns("spell type");
            mockSpellGenerator.Setup(g => g.GenerateLevel(PowerConstants.Minor)).Returns(1337);
            mockSpellGenerator.Setup(g => g.Generate("spell type", 1337)).Returns("spell");

            var updatedWeapon = specialAbilitiesGenerator.ApplyAbilitiesToWeapon(weapon);
            Assert.That(updatedWeapon, Is.EqualTo(weapon));
            Assert.That(weapon.Contents, Is.Empty);
        }

        [Test]
        public void ApplyAbilitiesToWeapon_AllAbilities()
        {
            mockPercentileSelector.Setup(s => s.SelectFrom<bool>(Config.Name, TableNameConstants.Percentiles.SpellStoringContainsSpell)).Returns(true);

            var ability1 = new SpecialAbility();
            ability1.Damages.Add(new Damage { Roll = "some", Type = "plasma", Condition = "my condition" });
            ability1.Damages.Add(new Damage { Roll = "a bit", Type = "ether", Condition = "my other condition" });

            ability1.CriticalDamages =
            [
                new Damage { Roll = "more", Type = "plasma", Condition = "my crit condition" },
                new Damage { Roll = "a lot", Type = "ether", Condition = "my other crit condition" },
            ];

            var ability2 = new SpecialAbility();
            ability2.Damages.Add(new Damage { Roll = "a touch", Type = "here", Condition = "my condition 2" });
            ability2.Damages.Add(new Damage { Roll = "another touch", Type = "there", Condition = "my other condition 2" });

            ability2.CriticalDamages =
            [
                new Damage { Roll = "MORE", Type = "here", Condition = "my crit condition 2" },
                new Damage { Roll = "a ton", Type = "there", Condition = "my other crit condition 2" },
            ];

            mockSpellGenerator.Setup(g => g.GenerateType()).Returns("spell type");
            mockSpellGenerator.Setup(g => g.GenerateLevel(PowerConstants.Minor)).Returns(1337);
            mockSpellGenerator.Setup(g => g.Generate("spell type", 1337)).Returns("spell");

            var weapon = CreateWeapon();
            weapon.Magic.SpecialAbilities =
            [
                new SpecialAbility(),
                ability1,
                ability2,
                new SpecialAbility { Name = SpecialAbilityConstants.SpellStoring },
                new SpecialAbility { Name = SpecialAbilityConstants.Keen },
            ];

            var updatedWeapon = specialAbilitiesGenerator.ApplyAbilitiesToWeapon(weapon);
            Assert.That(updatedWeapon, Is.EqualTo(weapon));
            Assert.That(weapon.Magic.SpecialAbilities.Count(), Is.EqualTo(5));
            Assert.That(weapon.Damages, Has.Count.EqualTo(5));
            Assert.That(weapon.Damages[0].Roll, Is.EqualTo("my roll"));
            Assert.That(weapon.Damages[0].Type, Is.EqualTo(WeaponDamageType));
            Assert.That(weapon.Damages[0].Condition, Is.Empty);
            Assert.That(weapon.Damages[1].Roll, Is.EqualTo("some"));
            Assert.That(weapon.Damages[1].Type, Is.EqualTo("plasma"));
            Assert.That(weapon.Damages[1].Condition, Is.EqualTo("my condition"));
            Assert.That(weapon.Damages[2].Roll, Is.EqualTo("a bit"));
            Assert.That(weapon.Damages[2].Type, Is.EqualTo("ether"));
            Assert.That(weapon.Damages[2].Condition, Is.EqualTo("my other condition"));
            Assert.That(weapon.Damages[3].Roll, Is.EqualTo("a touch"));
            Assert.That(weapon.Damages[3].Type, Is.EqualTo("here"));
            Assert.That(weapon.Damages[3].Condition, Is.EqualTo("my condition 2"));
            Assert.That(weapon.Damages[4].Roll, Is.EqualTo("another touch"));
            Assert.That(weapon.Damages[4].Type, Is.EqualTo("there"));
            Assert.That(weapon.Damages[4].Condition, Is.EqualTo("my other condition 2"));
            Assert.That(weapon.CriticalDamages, Has.Count.EqualTo(5));
            Assert.That(weapon.CriticalDamages[0].Roll, Is.EqualTo("my crit roll"));
            Assert.That(weapon.CriticalDamages[0].Type, Is.EqualTo(WeaponDamageType_Crit));
            Assert.That(weapon.CriticalDamages[0].Condition, Is.Empty);
            Assert.That(weapon.CriticalDamages[1].Roll, Is.EqualTo("more"));
            Assert.That(weapon.CriticalDamages[1].Type, Is.EqualTo("plasma"));
            Assert.That(weapon.CriticalDamages[1].Condition, Is.EqualTo("my crit condition"));
            Assert.That(weapon.CriticalDamages[2].Roll, Is.EqualTo("a lot"));
            Assert.That(weapon.CriticalDamages[2].Type, Is.EqualTo("ether"));
            Assert.That(weapon.CriticalDamages[2].Condition, Is.EqualTo("my other crit condition"));
            Assert.That(weapon.CriticalDamages[3].Roll, Is.EqualTo("MORE"));
            Assert.That(weapon.CriticalDamages[3].Type, Is.EqualTo("here"));
            Assert.That(weapon.CriticalDamages[3].Condition, Is.EqualTo("my crit condition 2"));
            Assert.That(weapon.CriticalDamages[4].Roll, Is.EqualTo("a ton"));
            Assert.That(weapon.CriticalDamages[4].Type, Is.EqualTo("there"));
            Assert.That(weapon.CriticalDamages[4].Condition, Is.EqualTo("my other crit condition 2"));
            Assert.That(weapon.SecondaryDamages, Is.Empty);
            Assert.That(weapon.SecondaryCriticalDamages, Is.Empty);
            Assert.That(weapon.Contents, Contains.Item("spell"));
            Assert.That(weapon.ThreatRange, Is.EqualTo(2));
            Assert.That(weapon.ThreatRangeSummary, Is.EqualTo("19-20"));
        }

        [Test]
        public void ApplyAbilitiesToWeapon_AllAbilities_DoubleWeapon()
        {
            mockPercentileSelector.Setup(s => s.SelectFrom<bool>(Config.Name, TableNameConstants.Percentiles.SpellStoringContainsSpell)).Returns(true);

            var ability1 = new SpecialAbility();
            ability1.Damages.Add(new Damage { Roll = "some", Type = "plasma", Condition = "my condition" });
            ability1.Damages.Add(new Damage { Roll = "a bit", Type = "ether", Condition = "my other condition" });

            ability1.CriticalDamages =
            [
                new Damage { Roll = "more", Type = "plasma", Condition = "my crit condition" },
                new Damage { Roll = "a lot", Type = "ether", Condition = "my other crit condition" },
            ];

            var ability2 = new SpecialAbility();
            ability2.Damages.Add(new Damage { Roll = "a touch", Type = "here", Condition = "my condition 2" });
            ability2.Damages.Add(new Damage { Roll = "another touch", Type = "there", Condition = "my other condition 2" });

            ability2.CriticalDamages =
            [
                new Damage { Roll = "MORE", Type = "here", Condition = "my crit condition 2" },
                new Damage { Roll = "a ton", Type = "there", Condition = "my other crit condition 2" },
            ];

            mockSpellGenerator.Setup(g => g.GenerateType()).Returns("spell type");
            mockSpellGenerator.Setup(g => g.GenerateLevel(PowerConstants.Minor)).Returns(1337);
            mockSpellGenerator.Setup(g => g.Generate("spell type", 1337)).Returns("spell");

            var weapon = CreateWeapon(true);
            weapon.Magic.SpecialAbilities =
            [
                new SpecialAbility(),
                ability1,
                ability2,
                new SpecialAbility { Name = SpecialAbilityConstants.SpellStoring },
                new SpecialAbility { Name = SpecialAbilityConstants.Keen },
            ];
            weapon.SecondaryMagicBonus = weapon.Magic.Bonus;
            weapon.SecondaryHasAbilities = true;

            var updatedWeapon = specialAbilitiesGenerator.ApplyAbilitiesToWeapon(weapon);
            Assert.That(updatedWeapon, Is.EqualTo(weapon));
            Assert.That(weapon.Magic.SpecialAbilities.Count(), Is.EqualTo(5));
            Assert.That(weapon.Damages, Has.Count.EqualTo(5));
            Assert.That(weapon.Damages[0].Roll, Is.EqualTo("my roll"));
            Assert.That(weapon.Damages[0].Type, Is.EqualTo(WeaponDamageType));
            Assert.That(weapon.Damages[0].Condition, Is.Empty);
            Assert.That(weapon.Damages[1].Roll, Is.EqualTo("some"));
            Assert.That(weapon.Damages[1].Type, Is.EqualTo("plasma"));
            Assert.That(weapon.Damages[1].Condition, Is.EqualTo("my condition"));
            Assert.That(weapon.Damages[2].Roll, Is.EqualTo("a bit"));
            Assert.That(weapon.Damages[2].Type, Is.EqualTo("ether"));
            Assert.That(weapon.Damages[2].Condition, Is.EqualTo("my other condition"));
            Assert.That(weapon.Damages[3].Roll, Is.EqualTo("a touch"));
            Assert.That(weapon.Damages[3].Type, Is.EqualTo("here"));
            Assert.That(weapon.Damages[3].Condition, Is.EqualTo("my condition 2"));
            Assert.That(weapon.Damages[4].Roll, Is.EqualTo("another touch"));
            Assert.That(weapon.Damages[4].Type, Is.EqualTo("there"));
            Assert.That(weapon.Damages[4].Condition, Is.EqualTo("my other condition 2"));
            Assert.That(weapon.CriticalDamages, Has.Count.EqualTo(5));
            Assert.That(weapon.CriticalDamages[0].Roll, Is.EqualTo("my crit roll"));
            Assert.That(weapon.CriticalDamages[0].Type, Is.EqualTo(WeaponDamageType_Crit));
            Assert.That(weapon.CriticalDamages[0].Condition, Is.Empty);
            Assert.That(weapon.CriticalDamages[1].Roll, Is.EqualTo("more"));
            Assert.That(weapon.CriticalDamages[1].Type, Is.EqualTo("plasma"));
            Assert.That(weapon.CriticalDamages[1].Condition, Is.EqualTo("my crit condition"));
            Assert.That(weapon.CriticalDamages[2].Roll, Is.EqualTo("a lot"));
            Assert.That(weapon.CriticalDamages[2].Type, Is.EqualTo("ether"));
            Assert.That(weapon.CriticalDamages[2].Condition, Is.EqualTo("my other crit condition"));
            Assert.That(weapon.CriticalDamages[3].Roll, Is.EqualTo("MORE"));
            Assert.That(weapon.CriticalDamages[3].Type, Is.EqualTo("here"));
            Assert.That(weapon.CriticalDamages[3].Condition, Is.EqualTo("my crit condition 2"));
            Assert.That(weapon.CriticalDamages[4].Roll, Is.EqualTo("a ton"));
            Assert.That(weapon.CriticalDamages[4].Type, Is.EqualTo("there"));
            Assert.That(weapon.CriticalDamages[4].Condition, Is.EqualTo("my other crit condition 2"));
            Assert.That(weapon.SecondaryDamages, Has.Count.EqualTo(5));
            Assert.That(weapon.SecondaryDamages[0].Roll, Is.EqualTo("jab"));
            Assert.That(weapon.SecondaryDamages[0].Type, Is.EqualTo("punch"));
            Assert.That(weapon.SecondaryDamages[0].Condition, Is.Empty);
            Assert.That(weapon.SecondaryDamages[1].Roll, Is.EqualTo("some"));
            Assert.That(weapon.SecondaryDamages[1].Type, Is.EqualTo("plasma"));
            Assert.That(weapon.SecondaryDamages[1].Condition, Is.EqualTo("my condition"));
            Assert.That(weapon.SecondaryDamages[2].Roll, Is.EqualTo("a bit"));
            Assert.That(weapon.SecondaryDamages[2].Type, Is.EqualTo("ether"));
            Assert.That(weapon.SecondaryDamages[2].Condition, Is.EqualTo("my other condition"));
            Assert.That(weapon.SecondaryDamages[3].Roll, Is.EqualTo("a touch"));
            Assert.That(weapon.SecondaryDamages[3].Type, Is.EqualTo("here"));
            Assert.That(weapon.SecondaryDamages[3].Condition, Is.EqualTo("my condition 2"));
            Assert.That(weapon.SecondaryDamages[4].Roll, Is.EqualTo("another touch"));
            Assert.That(weapon.SecondaryDamages[4].Type, Is.EqualTo("there"));
            Assert.That(weapon.SecondaryDamages[4].Condition, Is.EqualTo("my other condition 2"));
            Assert.That(weapon.SecondaryCriticalDamages, Has.Count.EqualTo(5));
            Assert.That(weapon.SecondaryCriticalDamages[0].Roll, Is.EqualTo("cross"));
            Assert.That(weapon.SecondaryCriticalDamages[0].Type, Is.EqualTo("punch"));
            Assert.That(weapon.SecondaryCriticalDamages[0].Condition, Is.Empty);
            Assert.That(weapon.SecondaryCriticalDamages[1].Roll, Is.EqualTo("more"));
            Assert.That(weapon.SecondaryCriticalDamages[1].Type, Is.EqualTo("plasma"));
            Assert.That(weapon.SecondaryCriticalDamages[1].Condition, Is.EqualTo("my crit condition"));
            Assert.That(weapon.SecondaryCriticalDamages[2].Roll, Is.EqualTo("a lot"));
            Assert.That(weapon.SecondaryCriticalDamages[2].Type, Is.EqualTo("ether"));
            Assert.That(weapon.SecondaryCriticalDamages[2].Condition, Is.EqualTo("my other crit condition"));
            Assert.That(weapon.SecondaryCriticalDamages[3].Roll, Is.EqualTo("MORE"));
            Assert.That(weapon.SecondaryCriticalDamages[3].Type, Is.EqualTo("here"));
            Assert.That(weapon.SecondaryCriticalDamages[3].Condition, Is.EqualTo("my crit condition 2"));
            Assert.That(weapon.SecondaryCriticalDamages[4].Roll, Is.EqualTo("a ton"));
            Assert.That(weapon.SecondaryCriticalDamages[4].Type, Is.EqualTo("there"));
            Assert.That(weapon.SecondaryCriticalDamages[4].Condition, Is.EqualTo("my other crit condition 2"));
            Assert.That(weapon.Contents, Contains.Item("spell"));
            Assert.That(weapon.ThreatRange, Is.EqualTo(2));
            Assert.That(weapon.ThreatRangeSummary, Is.EqualTo("19-20"));
        }
    }
}