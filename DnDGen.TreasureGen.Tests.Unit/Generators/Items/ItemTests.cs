using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Items.Mundane;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections;
using System.Linq;

namespace DnDGen.TreasureGen.Tests.Unit.Generators.Items
{
    [TestFixture]
    public class ItemTests
    {
        private Item item;
        private ItemVerifier itemVerifier;

        [SetUp]
        public void Setup()
        {
            item = new Item();
            itemVerifier = new ItemVerifier();
        }

        [Test]
        public void ItemInitialized()
        {
            Assert.That(item.Name, Is.Empty);
            Assert.That(item.BaseNames, Is.Empty);
            Assert.That(item.Attributes, Is.Empty);
            Assert.That(item.Magic, Is.Not.Null);
            Assert.That(item.Quantity, Is.EqualTo(1));
            Assert.That(item.Traits, Is.Empty);
            Assert.That(item.IsMagical, Is.False);
            Assert.That(item.CanBeUsedAsWeaponOrArmor, Is.False);
            Assert.That(item.Contents, Is.Empty);
            Assert.That(item.ItemType, Is.Empty);
        }

        [Test]
        public void IsMagicalTrueIfSetToTrue()
        {
            item.IsMagical = true;
            Assert.That(item.IsMagical, Is.True);
        }

        [Test]
        public void IsMagicalFalseIfSetToFalse()
        {
            item.IsMagical = true;
            item.IsMagical = false;
            Assert.That(item.IsMagical, Is.False);
        }

        [Test]
        public void IsMagicalTrueIfThereIsABonus()
        {
            item.Magic.Bonus = 1;
            Assert.That(item.IsMagical, Is.True);
        }

        [Test]
        public void IsMagicalTrueIfThereAreCharges()
        {
            item.Magic.Charges = 1;
            Assert.That(item.IsMagical, Is.True);
        }

        [Test]
        public void IsMagicalTrueIfThereIsACurse()
        {
            item.Magic.Curse = "curse";
            Assert.That(item.IsMagical, Is.True);
        }

        [Test]
        public void IsMagicalTrueIfThereIsIntelligenceWithEgo()
        {
            item.Magic.Intelligence.Ego = 1;
            Assert.That(item.IsMagical, Is.True);
        }

        [Test]
        public void IsMagicalTrueIfThereAreSpecialAbilities()
        {
            item.Magic.SpecialAbilities = new[] { new SpecialAbility() };
            Assert.That(item.IsMagical, Is.True);
        }

        [Test]
        public void SmartCloneItem()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);
            template.ItemType = Guid.NewGuid().ToString();

            template.Attributes = new[] { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() };

            var copy = template.SmartClone();
            itemVerifier.AssertMagicalItemFromTemplate(copy, template);
            Assert.That(copy.ItemType, Is.EqualTo(template.ItemType));
            Assert.That(copy.Attributes, Is.EquivalentTo(template.Attributes));
        }

        [Test]
        public void SmartCloneItemIntoSpecifiedItem()
        {
            var clone = new Item();
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);
            template.ItemType = Guid.NewGuid().ToString();

            template.Attributes = new[] { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() };

            var newClone = template.SmartCloneInto(clone);
            Assert.That(newClone, Is.EqualTo(clone));
            itemVerifier.AssertMagicalItemFromTemplate(clone, template);
            Assert.That(clone.ItemType, Is.EqualTo(template.ItemType));
            Assert.That(clone.Attributes, Is.EquivalentTo(template.Attributes));
        }

        [Test]
        public void SmartCloneScroll()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);
            template.ItemType = ItemTypeConstants.Scroll;
            template.Magic.SpecialAbilities = new[]
            {
                new SpecialAbility { Name = Guid.NewGuid().ToString() },
                new SpecialAbility { Name = Guid.NewGuid().ToString() }
            };

            //INFO: All scrolls have a one-time use attribute, so this is appropriate for the use case
            template.Attributes = new[] { Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), AttributeConstants.OneTimeUse };

            var copy = template.SmartClone();
            itemVerifier.AssertMagicalItemFromTemplate(copy, template);
            Assert.That(copy.ItemType, Is.EqualTo(ItemTypeConstants.Scroll));
            Assert.That(copy.Attributes, Is.EquivalentTo(template.Attributes));
            Assert.That(copy.Magic.Bonus, Is.EqualTo(0));
            Assert.That(copy.Magic.Charges, Is.EqualTo(0));
            Assert.That(copy.Magic.Intelligence.Alignment, Is.Empty);
            Assert.That(copy.Magic.Intelligence.CharismaStat, Is.EqualTo(0));
            Assert.That(copy.Magic.Intelligence.Communication, Is.Empty);
            Assert.That(copy.Magic.Intelligence.DedicatedPower, Is.Empty);
            Assert.That(copy.Magic.Intelligence.Ego, Is.EqualTo(0));
            Assert.That(copy.Magic.Intelligence.IntelligenceStat, Is.EqualTo(0));
            Assert.That(copy.Magic.Intelligence.Languages, Is.Empty);
            Assert.That(copy.Magic.Intelligence.Personality, Is.Empty);
            Assert.That(copy.Magic.Intelligence.Powers, Is.Empty);
            Assert.That(copy.Magic.Intelligence.Senses, Is.Empty);
            Assert.That(copy.Magic.Intelligence.SpecialPurpose, Is.Empty);
            Assert.That(copy.Magic.Intelligence.WisdomStat, Is.EqualTo(0));
            Assert.That(copy.Magic.SpecialAbilities, Is.Empty);
        }

        [Test]
        public void SmartCloneOneTimeUseItem()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);
            template.ItemType = Guid.NewGuid().ToString();
            template.Magic.SpecialAbilities = new[]
            {
                new SpecialAbility { Name = Guid.NewGuid().ToString() },
                new SpecialAbility { Name = Guid.NewGuid().ToString() }
            };

            template.Attributes = new[] { Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), AttributeConstants.OneTimeUse };

            var copy = template.SmartClone();
            itemVerifier.AssertMagicalItemFromTemplate(copy, template);
            Assert.That(copy.ItemType, Is.EqualTo(template.ItemType));
            Assert.That(copy.Attributes, Is.EquivalentTo(template.Attributes));
            Assert.That(copy.Magic.Intelligence.Alignment, Is.Empty);
            Assert.That(copy.Magic.Intelligence.CharismaStat, Is.EqualTo(0));
            Assert.That(copy.Magic.Intelligence.Communication, Is.Empty);
            Assert.That(copy.Magic.Intelligence.DedicatedPower, Is.Empty);
            Assert.That(copy.Magic.Intelligence.Ego, Is.EqualTo(0));
            Assert.That(copy.Magic.Intelligence.IntelligenceStat, Is.EqualTo(0));
            Assert.That(copy.Magic.Intelligence.Languages, Is.Empty);
            Assert.That(copy.Magic.Intelligence.Personality, Is.Empty);
            Assert.That(copy.Magic.Intelligence.Powers, Is.Empty);
            Assert.That(copy.Magic.Intelligence.Senses, Is.Empty);
            Assert.That(copy.Magic.Intelligence.SpecialPurpose, Is.Empty);
            Assert.That(copy.Magic.Intelligence.WisdomStat, Is.EqualTo(0));
        }

        [Test]
        public void SmartCloneAmmunition()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);
            template.ItemType = ItemTypeConstants.Weapon;
            template.Magic.SpecialAbilities = new[]
            {
                new SpecialAbility { Name = Guid.NewGuid().ToString() },
                new SpecialAbility { Name = Guid.NewGuid().ToString() }
            };

            template.Attributes = new[] { Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), AttributeConstants.Ammunition };

            var clone = template.SmartClone();
            itemVerifier.AssertMagicalItemFromTemplate(clone, template);
            Assert.That(clone.ItemType, Is.EqualTo(template.ItemType));
            Assert.That(clone.Attributes, Is.EquivalentTo(template.Attributes));
            Assert.That(clone.Magic.Intelligence.Alignment, Is.Empty);
            Assert.That(clone.Magic.Intelligence.CharismaStat, Is.EqualTo(0));
            Assert.That(clone.Magic.Intelligence.Communication, Is.Empty);
            Assert.That(clone.Magic.Intelligence.DedicatedPower, Is.Empty);
            Assert.That(clone.Magic.Intelligence.Ego, Is.EqualTo(0));
            Assert.That(clone.Magic.Intelligence.IntelligenceStat, Is.EqualTo(0));
            Assert.That(clone.Magic.Intelligence.Languages, Is.Empty);
            Assert.That(clone.Magic.Intelligence.Personality, Is.Empty);
            Assert.That(clone.Magic.Intelligence.Powers, Is.Empty);
            Assert.That(clone.Magic.Intelligence.Senses, Is.Empty);
            Assert.That(clone.Magic.Intelligence.SpecialPurpose, Is.Empty);
            Assert.That(clone.Magic.Intelligence.WisdomStat, Is.EqualTo(0));
            Assert.That(clone.Magic.SpecialAbilities, Is.EquivalentTo(template.Magic.SpecialAbilities));
        }

        [Test]
        public void SmartClonePotion()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);
            template.ItemType = ItemTypeConstants.Potion;
            template.Magic.SpecialAbilities = new[]
            {
                new SpecialAbility { Name = Guid.NewGuid().ToString() },
                new SpecialAbility { Name = Guid.NewGuid().ToString() }
            };

            //INFO: All potions have a one-time use attribute, so this is appropriate for the use case
            template.Attributes = new[] { Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), AttributeConstants.OneTimeUse };

            var copy = template.SmartClone();
            itemVerifier.AssertMagicalItemFromTemplate(copy, template);
            Assert.That(copy.ItemType, Is.EqualTo(ItemTypeConstants.Potion));
            Assert.That(copy.Attributes, Is.EquivalentTo(template.Attributes));
            Assert.That(copy.Magic.Intelligence.Alignment, Is.Empty);
            Assert.That(copy.Magic.Intelligence.CharismaStat, Is.EqualTo(0));
            Assert.That(copy.Magic.Intelligence.Communication, Is.Empty);
            Assert.That(copy.Magic.Intelligence.DedicatedPower, Is.Empty);
            Assert.That(copy.Magic.Intelligence.Ego, Is.EqualTo(0));
            Assert.That(copy.Magic.Intelligence.IntelligenceStat, Is.EqualTo(0));
            Assert.That(copy.Magic.Intelligence.Languages, Is.Empty);
            Assert.That(copy.Magic.Intelligence.Personality, Is.Empty);
            Assert.That(copy.Magic.Intelligence.Powers, Is.Empty);
            Assert.That(copy.Magic.Intelligence.Senses, Is.Empty);
            Assert.That(copy.Magic.Intelligence.SpecialPurpose, Is.Empty);
            Assert.That(copy.Magic.Intelligence.WisdomStat, Is.EqualTo(0));
        }

        [Test]
        public void SmartCloneWand()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);
            template.ItemType = ItemTypeConstants.Wand;
            template.Magic.SpecialAbilities = new[]
            {
                new SpecialAbility { Name = Guid.NewGuid().ToString() },
                new SpecialAbility { Name = Guid.NewGuid().ToString() }
            };

            //INFO: All potions have a one-time use attribute, so this is appropriate for the use case
            template.Attributes = new[] { Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), AttributeConstants.OneTimeUse };

            var copy = template.SmartClone();
            itemVerifier.AssertMagicalItemFromTemplate(copy, template);
            Assert.That(copy.ItemType, Is.EqualTo(ItemTypeConstants.Wand));
            Assert.That(copy.Attributes, Is.EquivalentTo(template.Attributes));
            Assert.That(copy.Contents, Is.Empty);
            Assert.That(copy.Magic.Bonus, Is.EqualTo(0));
            Assert.That(copy.Magic.Intelligence.Alignment, Is.Empty);
            Assert.That(copy.Magic.Intelligence.CharismaStat, Is.EqualTo(0));
            Assert.That(copy.Magic.Intelligence.Communication, Is.Empty);
            Assert.That(copy.Magic.Intelligence.DedicatedPower, Is.Empty);
            Assert.That(copy.Magic.Intelligence.Ego, Is.EqualTo(0));
            Assert.That(copy.Magic.Intelligence.IntelligenceStat, Is.EqualTo(0));
            Assert.That(copy.Magic.Intelligence.Languages, Is.Empty);
            Assert.That(copy.Magic.Intelligence.Personality, Is.Empty);
            Assert.That(copy.Magic.Intelligence.Powers, Is.Empty);
            Assert.That(copy.Magic.Intelligence.Senses, Is.Empty);
            Assert.That(copy.Magic.Intelligence.SpecialPurpose, Is.Empty);
            Assert.That(copy.Magic.Intelligence.WisdomStat, Is.EqualTo(0));
            Assert.That(copy.Magic.SpecialAbilities, Is.Empty);
        }

        [Test]
        public void MundaneCloneItem()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);
            template.ItemType = Guid.NewGuid().ToString();
            template.Magic.SpecialAbilities = new[]
            {
                new SpecialAbility { Name = Guid.NewGuid().ToString() },
                new SpecialAbility { Name = Guid.NewGuid().ToString() }
            };

            template.Attributes = new[] { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() };

            var copy = template.MundaneClone();
            itemVerifier.AssertMundaneItemFromTemplate(copy, template);
            Assert.That(copy.ItemType, Is.EqualTo(template.ItemType));
            Assert.That(copy.Attributes, Is.EquivalentTo(template.Attributes));
            Assert.That(copy.Magic.SpecialAbilities, Is.Empty);
        }

        [Test]
        public void MundaneCloneItemIntoSpecifiedItem()
        {
            var clone = new Item();
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);
            template.ItemType = Guid.NewGuid().ToString();
            template.Magic.SpecialAbilities = new[]
            {
                new SpecialAbility { Name = Guid.NewGuid().ToString() },
                new SpecialAbility { Name = Guid.NewGuid().ToString() }
            };

            template.Attributes = new[] { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() };

            var newClone = template.MundaneCloneInto(clone);

            Assert.That(newClone, Is.EqualTo(clone));
            itemVerifier.AssertMundaneItemFromTemplate(clone, template);
            Assert.That(clone.ItemType, Is.EqualTo(template.ItemType));
            Assert.That(clone.Attributes, Is.EquivalentTo(template.Attributes));
            Assert.That(clone.Magic.SpecialAbilities, Is.Empty);
        }

        [Test]
        public void MundaneCloneItemIntoNullItem()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);
            template.ItemType = Guid.NewGuid().ToString();
            template.Magic.SpecialAbilities = new[]
            {
                new SpecialAbility { Name = Guid.NewGuid().ToString() },
                new SpecialAbility { Name = Guid.NewGuid().ToString() }
            };

            template.Attributes = new[] { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() };

            Assert.That(() => template.MundaneCloneInto(null),
                Throws.ArgumentException.With.Message.EqualTo("Clone target cannot be null"));
        }

        [TestCase(ItemTypeConstants.Armor)]
        [TestCase(ItemTypeConstants.Weapon)]
        public void ItemTypeCanBeUsedAsWeaponOrArmor(string itemType)
        {
            item.ItemType = itemType;
            Assert.That(item.CanBeUsedAsWeaponOrArmor, Is.True);
        }

        [TestCase(ItemTypeConstants.AlchemicalItem)]
        [TestCase(ItemTypeConstants.Potion)]
        [TestCase(ItemTypeConstants.Ring)]
        [TestCase(ItemTypeConstants.Rod)]
        [TestCase(ItemTypeConstants.Scroll)]
        [TestCase(ItemTypeConstants.Staff)]
        [TestCase(ItemTypeConstants.Tool)]
        [TestCase(ItemTypeConstants.Wand)]
        [TestCase(ItemTypeConstants.WondrousItem)]
        public void ItemTypeCannotBeUsedAsWeaponOrArmor(string itemType)
        {
            item.ItemType = itemType;
            Assert.That(item.CanBeUsedAsWeaponOrArmor, Is.False);
        }

        [Test]
        public void NamedWeaponCanBeUsedAsWeaponOrArmor()
        {
            var allWeapons = WeaponConstants.GetAllWeapons(true, true);
            foreach (var weapon in allWeapons)
            {
                item.Name = weapon;
                item.BaseNames = new[] { "base name", "other base name" };
                Assert.That(item.CanBeUsedAsWeaponOrArmor, Is.True, weapon);
            }
        }

        [Test]
        public void NamedArmorCanBeUsedAsWeaponOrArmor()
        {
            var allArmors = ArmorConstants.GetAllArmorsAndShields(true);
            foreach (var armor in allArmors)
            {
                item.Name = armor;
                item.BaseNames = new[] { "base name", "other base name" };
                Assert.That(item.CanBeUsedAsWeaponOrArmor, Is.True, armor);
            }
        }

        [Test]
        public void RandomNameCannotBeUsedAsWeaponOrArmor()
        {
            item.Name = "random name";
            Assert.That(item.CanBeUsedAsWeaponOrArmor, Is.False);
        }

        [Test]
        public void BaseNamedWeaponCanBeUsedAsWeaponOrArmor()
        {
            var allWeapons = WeaponConstants.GetAllWeapons(true, true);
            foreach (var weapon in allWeapons)
            {
                item.Name = "random name";
                item.BaseNames = new[] { "base name", weapon, "other base name" };
                Assert.That(item.CanBeUsedAsWeaponOrArmor, Is.True, weapon);
            }
        }

        [Test]
        public void BaseNamedArmorCanBeUsedAsWeaponOrArmor()
        {
            var allArmors = ArmorConstants.GetAllArmorsAndShields(true);
            foreach (var armor in allArmors)
            {
                item.Name = "random name";
                item.BaseNames = new[] { "base name", armor, "other base name" };
                Assert.That(item.CanBeUsedAsWeaponOrArmor, Is.True, armor);
            }
        }

        [Test]
        public void RandomBaseNameCannotBeUsedAsWeaponOrArmor()
        {
            item.Name = "random name";
            item.BaseNames = new[] { "base name", "other base name" };
            Assert.That(item.CanBeUsedAsWeaponOrArmor, Is.False);
        }

        [TestCase(ItemTypeConstants.Armor)]
        [TestCase(ItemTypeConstants.Weapon)]
        public void SmartCloneWithItemTypeCanBeUsedAsWeaponOrArmor(string itemType)
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);
            template.ItemType = itemType;

            var clone = template.SmartClone();
            Assert.That(clone, Is.Not.SameAs(template));
            Assert.That(JsonConvert.SerializeObject(clone), Is.EqualTo(JsonConvert.SerializeObject(template)));
            Assert.That(clone.Name, Is.EqualTo(template.Name));
            Assert.That(clone.BaseNames, Is.EquivalentTo(template.BaseNames));

            Assert.That(clone.Attributes, Is.EquivalentTo(template.Attributes));
            Assert.That(clone.Contents, Is.EquivalentTo(template.Contents));
            Assert.That(clone.Magic.Bonus, Is.EqualTo(template.Magic.Bonus), item.Name);
            Assert.That(clone.Magic.Charges, Is.EqualTo(template.Magic.Charges), item.Name);
            Assert.That(clone.Magic.Curse, Is.EqualTo(template.Magic.Curse).Or.EqualTo(CurseConstants.SpecificCursedItem));
            Assert.That(clone.Magic.Intelligence.Alignment, Is.EqualTo(template.Magic.Intelligence.Alignment));
            Assert.That(clone.Magic.Intelligence.CharismaStat, Is.EqualTo(template.Magic.Intelligence.CharismaStat));
            Assert.That(clone.Magic.Intelligence.Communication, Is.EquivalentTo(template.Magic.Intelligence.Communication));
            Assert.That(clone.Magic.Intelligence.DedicatedPower, Is.EqualTo(template.Magic.Intelligence.DedicatedPower));
            Assert.That(clone.Magic.Intelligence.Ego, Is.EqualTo(template.Magic.Intelligence.Ego));
            Assert.That(clone.Magic.Intelligence.IntelligenceStat, Is.EqualTo(template.Magic.Intelligence.IntelligenceStat));
            Assert.That(clone.Magic.Intelligence.Languages, Is.EquivalentTo(template.Magic.Intelligence.Languages));
            Assert.That(clone.Magic.Intelligence.Personality, Is.EqualTo(template.Magic.Intelligence.Personality));
            Assert.That(clone.Magic.Intelligence.Powers, Is.EquivalentTo(template.Magic.Intelligence.Powers));
            Assert.That(clone.Magic.Intelligence.Senses, Is.EqualTo(template.Magic.Intelligence.Senses));
            Assert.That(clone.Magic.Intelligence.SpecialPurpose, Is.EqualTo(template.Magic.Intelligence.SpecialPurpose));
            Assert.That(clone.Magic.Intelligence.WisdomStat, Is.EqualTo(template.Magic.Intelligence.WisdomStat));
            Assert.That(clone.Magic.SpecialAbilities, Is.EquivalentTo(template.Magic.SpecialAbilities));
            Assert.That(clone.Traits, Is.SupersetOf(template.Traits));
        }

        [TestCase(ItemTypeConstants.AlchemicalItem)]
        [TestCase(ItemTypeConstants.Potion)]
        [TestCase(ItemTypeConstants.Ring)]
        [TestCase(ItemTypeConstants.Rod)]
        [TestCase(ItemTypeConstants.Scroll)]
        [TestCase(ItemTypeConstants.Staff)]
        [TestCase(ItemTypeConstants.Tool)]
        [TestCase(ItemTypeConstants.Wand)]
        [TestCase(ItemTypeConstants.WondrousItem)]
        public void SmartCloneWithItemTypeCannotBeUsedAsWeaponOrArmor(string itemType)
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);
            template.ItemType = itemType;

            var clone = template.SmartClone();
            Assert.That(clone, Is.Not.EqualTo(template));
            Assert.That(clone.Name, Is.EqualTo(template.Name));
            Assert.That(clone.BaseNames, Is.EquivalentTo(template.BaseNames));

            Assert.That(clone.Attributes, Is.EquivalentTo(template.Attributes));
            Assert.That(clone.Magic.Curse, Is.EqualTo(template.Magic.Curse));
            Assert.That(clone.Magic.SpecialAbilities, Is.Empty);
            Assert.That(clone.Traits, Is.SupersetOf(template.Traits));
        }

        [Test]
        public void SmartCloneWithNamedWeaponCanBeUsedAsWeaponOrArmor()
        {
            var allWeapons = WeaponConstants.GetAllWeapons(true, true);
            foreach (var weapon in allWeapons)
            {
                var template = itemVerifier.CreateRandomTemplate(weapon);
                template.BaseNames = new[] { "base name", "other base name" };

                var clone = template.SmartClone();
                Assert.That(clone, Is.Not.SameAs(template));
                Assert.That(JsonConvert.SerializeObject(clone), Is.EqualTo(JsonConvert.SerializeObject(template)));
                Assert.That(clone.Name, Is.EqualTo(template.Name));
                Assert.That(clone.BaseNames, Is.EquivalentTo(template.BaseNames));

                Assert.That(clone.Attributes, Is.EquivalentTo(template.Attributes));
                Assert.That(clone.Contents, Is.EquivalentTo(template.Contents));
                Assert.That(clone.Magic.Bonus, Is.EqualTo(template.Magic.Bonus), item.Name);
                Assert.That(clone.Magic.Charges, Is.EqualTo(template.Magic.Charges), item.Name);
                Assert.That(clone.Magic.Curse, Is.EqualTo(template.Magic.Curse));
                Assert.That(clone.Magic.Intelligence.Alignment, Is.EqualTo(template.Magic.Intelligence.Alignment));
                Assert.That(clone.Magic.Intelligence.CharismaStat, Is.EqualTo(template.Magic.Intelligence.CharismaStat));
                Assert.That(clone.Magic.Intelligence.Communication, Is.EquivalentTo(template.Magic.Intelligence.Communication));
                Assert.That(clone.Magic.Intelligence.DedicatedPower, Is.EqualTo(template.Magic.Intelligence.DedicatedPower));
                Assert.That(clone.Magic.Intelligence.Ego, Is.EqualTo(template.Magic.Intelligence.Ego));
                Assert.That(clone.Magic.Intelligence.IntelligenceStat, Is.EqualTo(template.Magic.Intelligence.IntelligenceStat));
                Assert.That(clone.Magic.Intelligence.Languages, Is.EquivalentTo(template.Magic.Intelligence.Languages));
                Assert.That(clone.Magic.Intelligence.Personality, Is.EqualTo(template.Magic.Intelligence.Personality));
                Assert.That(clone.Magic.Intelligence.Powers, Is.EquivalentTo(template.Magic.Intelligence.Powers));
                Assert.That(clone.Magic.Intelligence.Senses, Is.EqualTo(template.Magic.Intelligence.Senses));
                Assert.That(clone.Magic.Intelligence.SpecialPurpose, Is.EqualTo(template.Magic.Intelligence.SpecialPurpose));
                Assert.That(clone.Magic.Intelligence.WisdomStat, Is.EqualTo(template.Magic.Intelligence.WisdomStat));
                Assert.That(clone.Magic.SpecialAbilities, Is.EquivalentTo(template.Magic.SpecialAbilities));
                Assert.That(clone.Traits, Is.SupersetOf(template.Traits));
            }
        }

        [Test]
        public void SmartCloneWithNamedArmorCanBeUsedAsWeaponOrArmor()
        {
            var allArmors = ArmorConstants.GetAllArmorsAndShields(true);
            foreach (var armor in allArmors)
            {
                var template = itemVerifier.CreateRandomTemplate(armor);
                template.BaseNames = new[] { "base name", "other base name" };

                var clone = template.SmartClone();
                Assert.That(clone, Is.Not.SameAs(template));
                Assert.That(JsonConvert.SerializeObject(clone), Is.EqualTo(JsonConvert.SerializeObject(template)));
                Assert.That(clone.Name, Is.EqualTo(template.Name));
                Assert.That(clone.BaseNames, Is.EquivalentTo(template.BaseNames));

                Assert.That(clone.Attributes, Is.EquivalentTo(template.Attributes));
                Assert.That(clone.Contents, Is.EquivalentTo(template.Contents));
                Assert.That(clone.Magic.Bonus, Is.EqualTo(template.Magic.Bonus), item.Name);
                Assert.That(clone.Magic.Charges, Is.EqualTo(template.Magic.Charges), item.Name);
                Assert.That(clone.Magic.Curse, Is.EqualTo(template.Magic.Curse));
                Assert.That(clone.Magic.Intelligence.Alignment, Is.EqualTo(template.Magic.Intelligence.Alignment));
                Assert.That(clone.Magic.Intelligence.CharismaStat, Is.EqualTo(template.Magic.Intelligence.CharismaStat));
                Assert.That(clone.Magic.Intelligence.Communication, Is.EquivalentTo(template.Magic.Intelligence.Communication));
                Assert.That(clone.Magic.Intelligence.DedicatedPower, Is.EqualTo(template.Magic.Intelligence.DedicatedPower));
                Assert.That(clone.Magic.Intelligence.Ego, Is.EqualTo(template.Magic.Intelligence.Ego));
                Assert.That(clone.Magic.Intelligence.IntelligenceStat, Is.EqualTo(template.Magic.Intelligence.IntelligenceStat));
                Assert.That(clone.Magic.Intelligence.Languages, Is.EquivalentTo(template.Magic.Intelligence.Languages));
                Assert.That(clone.Magic.Intelligence.Personality, Is.EqualTo(template.Magic.Intelligence.Personality));
                Assert.That(clone.Magic.Intelligence.Powers, Is.EquivalentTo(template.Magic.Intelligence.Powers));
                Assert.That(clone.Magic.Intelligence.Senses, Is.EqualTo(template.Magic.Intelligence.Senses));
                Assert.That(clone.Magic.Intelligence.SpecialPurpose, Is.EqualTo(template.Magic.Intelligence.SpecialPurpose));
                Assert.That(clone.Magic.Intelligence.WisdomStat, Is.EqualTo(template.Magic.Intelligence.WisdomStat));
                Assert.That(clone.Magic.SpecialAbilities, Is.EquivalentTo(template.Magic.SpecialAbilities));
                Assert.That(clone.Traits, Is.SupersetOf(template.Traits));
            }
        }

        [Test]
        public void SmartCloneWithRandomNameCannotBeUsedAsWeaponOrArmor()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);

            var clone = template.SmartClone();
            Assert.That(clone, Is.Not.EqualTo(template));
            Assert.That(clone.Name, Is.EqualTo(template.Name));
            Assert.That(clone.BaseNames, Is.EquivalentTo(template.BaseNames));

            Assert.That(clone.Attributes, Is.EquivalentTo(template.Attributes));
            Assert.That(clone.Contents, Is.EquivalentTo(template.Contents));
            Assert.That(clone.Magic.Bonus, Is.EqualTo(template.Magic.Bonus), item.Name);
            Assert.That(clone.Magic.Charges, Is.EqualTo(template.Magic.Charges), item.Name);
            Assert.That(clone.Magic.Curse, Is.EqualTo(template.Magic.Curse));
            Assert.That(clone.Magic.Intelligence.Alignment, Is.EqualTo(template.Magic.Intelligence.Alignment));
            Assert.That(clone.Magic.Intelligence.CharismaStat, Is.EqualTo(template.Magic.Intelligence.CharismaStat));
            Assert.That(clone.Magic.Intelligence.Communication, Is.EquivalentTo(template.Magic.Intelligence.Communication));
            Assert.That(clone.Magic.Intelligence.DedicatedPower, Is.EqualTo(template.Magic.Intelligence.DedicatedPower));
            Assert.That(clone.Magic.Intelligence.Ego, Is.EqualTo(template.Magic.Intelligence.Ego));
            Assert.That(clone.Magic.Intelligence.IntelligenceStat, Is.EqualTo(template.Magic.Intelligence.IntelligenceStat));
            Assert.That(clone.Magic.Intelligence.Languages, Is.EquivalentTo(template.Magic.Intelligence.Languages));
            Assert.That(clone.Magic.Intelligence.Personality, Is.EqualTo(template.Magic.Intelligence.Personality));
            Assert.That(clone.Magic.Intelligence.Powers, Is.EquivalentTo(template.Magic.Intelligence.Powers));
            Assert.That(clone.Magic.Intelligence.Senses, Is.EqualTo(template.Magic.Intelligence.Senses));
            Assert.That(clone.Magic.Intelligence.SpecialPurpose, Is.EqualTo(template.Magic.Intelligence.SpecialPurpose));
            Assert.That(clone.Magic.Intelligence.WisdomStat, Is.EqualTo(template.Magic.Intelligence.WisdomStat));
            Assert.That(clone.Magic.SpecialAbilities, Is.Empty);
            Assert.That(clone.Traits, Is.SupersetOf(template.Traits));
        }

        [Test]
        public void SmartCloneWithBaseNamedWeaponCanBeUsedAsWeaponOrArmor()
        {
            var allWeapons = WeaponConstants.GetAllWeapons(true, true);
            foreach (var weapon in allWeapons)
            {
                var name = Guid.NewGuid().ToString();
                var template = itemVerifier.CreateRandomTemplate(name);
                template.BaseNames = new[] { "base name", weapon, "other base name" };

                var clone = template.SmartClone();
                Assert.That(clone, Is.Not.SameAs(template));
                Assert.That(JsonConvert.SerializeObject(clone), Is.EqualTo(JsonConvert.SerializeObject(template)));
                Assert.That(clone.Name, Is.EqualTo(template.Name));
                Assert.That(clone.BaseNames, Is.EquivalentTo(template.BaseNames));

                Assert.That(clone.Attributes, Is.EquivalentTo(template.Attributes));
                Assert.That(clone.Contents, Is.EquivalentTo(template.Contents));
                Assert.That(clone.Magic.Bonus, Is.EqualTo(template.Magic.Bonus), item.Name);
                Assert.That(clone.Magic.Charges, Is.EqualTo(template.Magic.Charges), item.Name);
                Assert.That(clone.Magic.Curse, Is.EqualTo(template.Magic.Curse));
                Assert.That(clone.Magic.Intelligence.Alignment, Is.EqualTo(template.Magic.Intelligence.Alignment));
                Assert.That(clone.Magic.Intelligence.CharismaStat, Is.EqualTo(template.Magic.Intelligence.CharismaStat));
                Assert.That(clone.Magic.Intelligence.Communication, Is.EquivalentTo(template.Magic.Intelligence.Communication));
                Assert.That(clone.Magic.Intelligence.DedicatedPower, Is.EqualTo(template.Magic.Intelligence.DedicatedPower));
                Assert.That(clone.Magic.Intelligence.Ego, Is.EqualTo(template.Magic.Intelligence.Ego));
                Assert.That(clone.Magic.Intelligence.IntelligenceStat, Is.EqualTo(template.Magic.Intelligence.IntelligenceStat));
                Assert.That(clone.Magic.Intelligence.Languages, Is.EquivalentTo(template.Magic.Intelligence.Languages));
                Assert.That(clone.Magic.Intelligence.Personality, Is.EqualTo(template.Magic.Intelligence.Personality));
                Assert.That(clone.Magic.Intelligence.Powers, Is.EquivalentTo(template.Magic.Intelligence.Powers));
                Assert.That(clone.Magic.Intelligence.Senses, Is.EqualTo(template.Magic.Intelligence.Senses));
                Assert.That(clone.Magic.Intelligence.SpecialPurpose, Is.EqualTo(template.Magic.Intelligence.SpecialPurpose));
                Assert.That(clone.Magic.Intelligence.WisdomStat, Is.EqualTo(template.Magic.Intelligence.WisdomStat));
                Assert.That(clone.Magic.SpecialAbilities, Is.EquivalentTo(template.Magic.SpecialAbilities));
                Assert.That(clone.Traits, Is.SupersetOf(template.Traits));
            }
        }

        [Test]
        public void SmartCloneWithBaseNamedArmorCanBeUsedAsWeaponOrArmor()
        {
            var allArmors = ArmorConstants.GetAllArmorsAndShields(true);
            foreach (var armor in allArmors)
            {
                var name = Guid.NewGuid().ToString();
                var template = itemVerifier.CreateRandomTemplate(name);
                template.BaseNames = new[] { "base name", armor, "other base name" };

                var clone = template.SmartClone();
                Assert.That(clone, Is.Not.SameAs(template));
                Assert.That(JsonConvert.SerializeObject(clone), Is.EqualTo(JsonConvert.SerializeObject(template)));
                Assert.That(clone.Name, Is.EqualTo(template.Name));
                Assert.That(clone.BaseNames, Is.EquivalentTo(template.BaseNames));

                Assert.That(clone.Attributes, Is.EquivalentTo(template.Attributes));
                Assert.That(clone.Contents, Is.EquivalentTo(template.Contents));
                Assert.That(clone.Magic.Bonus, Is.EqualTo(template.Magic.Bonus), item.Name);
                Assert.That(clone.Magic.Charges, Is.EqualTo(template.Magic.Charges), item.Name);
                Assert.That(clone.Magic.Curse, Is.EqualTo(template.Magic.Curse));
                Assert.That(clone.Magic.Intelligence.Alignment, Is.EqualTo(template.Magic.Intelligence.Alignment));
                Assert.That(clone.Magic.Intelligence.CharismaStat, Is.EqualTo(template.Magic.Intelligence.CharismaStat));
                Assert.That(clone.Magic.Intelligence.Communication, Is.EquivalentTo(template.Magic.Intelligence.Communication));
                Assert.That(clone.Magic.Intelligence.DedicatedPower, Is.EqualTo(template.Magic.Intelligence.DedicatedPower));
                Assert.That(clone.Magic.Intelligence.Ego, Is.EqualTo(template.Magic.Intelligence.Ego));
                Assert.That(clone.Magic.Intelligence.IntelligenceStat, Is.EqualTo(template.Magic.Intelligence.IntelligenceStat));
                Assert.That(clone.Magic.Intelligence.Languages, Is.EquivalentTo(template.Magic.Intelligence.Languages));
                Assert.That(clone.Magic.Intelligence.Personality, Is.EqualTo(template.Magic.Intelligence.Personality));
                Assert.That(clone.Magic.Intelligence.Powers, Is.EquivalentTo(template.Magic.Intelligence.Powers));
                Assert.That(clone.Magic.Intelligence.Senses, Is.EqualTo(template.Magic.Intelligence.Senses));
                Assert.That(clone.Magic.Intelligence.SpecialPurpose, Is.EqualTo(template.Magic.Intelligence.SpecialPurpose));
                Assert.That(clone.Magic.Intelligence.WisdomStat, Is.EqualTo(template.Magic.Intelligence.WisdomStat));
                Assert.That(clone.Magic.SpecialAbilities, Is.EquivalentTo(template.Magic.SpecialAbilities));
                Assert.That(clone.Traits, Is.SupersetOf(template.Traits));
            }
        }

        [Test]
        public void SmartCloneWithRandomBaseNameCannotBeUsedAsWeaponOrArmor()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);

            var clone = template.SmartClone();
            Assert.That(clone, Is.Not.EqualTo(template));
            Assert.That(clone.Name, Is.EqualTo(template.Name));
            Assert.That(clone.BaseNames, Is.EquivalentTo(template.BaseNames));

            Assert.That(clone.Attributes, Is.EquivalentTo(template.Attributes));
            Assert.That(clone.Contents, Is.EquivalentTo(template.Contents));
            Assert.That(clone.Magic.Bonus, Is.EqualTo(template.Magic.Bonus), item.Name);
            Assert.That(clone.Magic.Charges, Is.EqualTo(template.Magic.Charges), item.Name);
            Assert.That(clone.Magic.Curse, Is.EqualTo(template.Magic.Curse));
            Assert.That(clone.Magic.Intelligence.Alignment, Is.EqualTo(template.Magic.Intelligence.Alignment));
            Assert.That(clone.Magic.Intelligence.CharismaStat, Is.EqualTo(template.Magic.Intelligence.CharismaStat));
            Assert.That(clone.Magic.Intelligence.Communication, Is.EquivalentTo(template.Magic.Intelligence.Communication));
            Assert.That(clone.Magic.Intelligence.DedicatedPower, Is.EqualTo(template.Magic.Intelligence.DedicatedPower));
            Assert.That(clone.Magic.Intelligence.Ego, Is.EqualTo(template.Magic.Intelligence.Ego));
            Assert.That(clone.Magic.Intelligence.IntelligenceStat, Is.EqualTo(template.Magic.Intelligence.IntelligenceStat));
            Assert.That(clone.Magic.Intelligence.Languages, Is.EquivalentTo(template.Magic.Intelligence.Languages));
            Assert.That(clone.Magic.Intelligence.Personality, Is.EqualTo(template.Magic.Intelligence.Personality));
            Assert.That(clone.Magic.Intelligence.Powers, Is.EquivalentTo(template.Magic.Intelligence.Powers));
            Assert.That(clone.Magic.Intelligence.Senses, Is.EqualTo(template.Magic.Intelligence.Senses));
            Assert.That(clone.Magic.Intelligence.SpecialPurpose, Is.EqualTo(template.Magic.Intelligence.SpecialPurpose));
            Assert.That(clone.Magic.Intelligence.WisdomStat, Is.EqualTo(template.Magic.Intelligence.WisdomStat));
            Assert.That(clone.Magic.SpecialAbilities, Is.Empty);
            Assert.That(clone.Traits, Is.SupersetOf(template.Traits));
        }

        [Test]
        public void CloneCopiesAll()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);

            var clone = template.Clone();
            Assert.That(clone, Is.Not.SameAs(template));
            Assert.That(JsonConvert.SerializeObject(clone), Is.EqualTo(JsonConvert.SerializeObject(template)));
            Assert.That(clone.Name, Is.EqualTo(template.Name));

            Assert.That(clone.Attributes, Is.EquivalentTo(template.Attributes));
            Assert.That(clone.BaseNames, Is.EquivalentTo(template.BaseNames));
            Assert.That(clone.Contents, Is.EquivalentTo(template.Contents));
            Assert.That(clone.Magic.Bonus, Is.EqualTo(template.Magic.Bonus));
            Assert.That(clone.Magic.Charges, Is.EqualTo(template.Magic.Charges));
            Assert.That(clone.Magic.Curse, Is.EqualTo(template.Magic.Curse));
            Assert.That(clone.Magic.Intelligence.Alignment, Is.EqualTo(template.Magic.Intelligence.Alignment));
            Assert.That(clone.Magic.Intelligence.CharismaStat, Is.EqualTo(template.Magic.Intelligence.CharismaStat));
            Assert.That(clone.Magic.Intelligence.Communication, Is.EquivalentTo(template.Magic.Intelligence.Communication));
            Assert.That(clone.Magic.Intelligence.DedicatedPower, Is.EqualTo(template.Magic.Intelligence.DedicatedPower));
            Assert.That(clone.Magic.Intelligence.Ego, Is.EqualTo(template.Magic.Intelligence.Ego));
            Assert.That(clone.Magic.Intelligence.IntelligenceStat, Is.EqualTo(template.Magic.Intelligence.IntelligenceStat));
            Assert.That(clone.Magic.Intelligence.Languages, Is.EquivalentTo(template.Magic.Intelligence.Languages));
            Assert.That(clone.Magic.Intelligence.Personality, Is.EqualTo(template.Magic.Intelligence.Personality));
            Assert.That(clone.Magic.Intelligence.Powers, Is.EquivalentTo(template.Magic.Intelligence.Powers));
            Assert.That(clone.Magic.Intelligence.Senses, Is.EqualTo(template.Magic.Intelligence.Senses));
            Assert.That(clone.Magic.Intelligence.SpecialPurpose, Is.EqualTo(template.Magic.Intelligence.SpecialPurpose));
            Assert.That(clone.Magic.Intelligence.WisdomStat, Is.EqualTo(template.Magic.Intelligence.WisdomStat));
            Assert.That(clone.Magic.SpecialAbilities, Is.EquivalentTo(template.Magic.SpecialAbilities));
            Assert.That(clone.Traits, Is.SupersetOf(template.Traits));
        }

        [Test]
        public void CloneCopiesAllIntoSpecifiedItem()
        {
            var clone = new Item();
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);

            var newClone = template.CloneInto(clone);
            Assert.That(newClone, Is.SameAs(clone).And.EqualTo(clone));
            Assert.That(clone, Is.Not.SameAs(template));
            Assert.That(JsonConvert.SerializeObject(clone), Is.EqualTo(JsonConvert.SerializeObject(template)));
            Assert.That(clone.Name, Is.EqualTo(template.Name));

            Assert.That(clone.Attributes, Is.EquivalentTo(template.Attributes));
            Assert.That(clone.BaseNames, Is.EquivalentTo(template.BaseNames));
            Assert.That(clone.Contents, Is.EquivalentTo(template.Contents));
            Assert.That(clone.Magic.Bonus, Is.EqualTo(template.Magic.Bonus));
            Assert.That(clone.Magic.Charges, Is.EqualTo(template.Magic.Charges));
            Assert.That(clone.Magic.Curse, Is.EqualTo(template.Magic.Curse));
            Assert.That(clone.Magic.Intelligence.Alignment, Is.EqualTo(template.Magic.Intelligence.Alignment));
            Assert.That(clone.Magic.Intelligence.CharismaStat, Is.EqualTo(template.Magic.Intelligence.CharismaStat));
            Assert.That(clone.Magic.Intelligence.Communication, Is.EquivalentTo(template.Magic.Intelligence.Communication));
            Assert.That(clone.Magic.Intelligence.DedicatedPower, Is.EqualTo(template.Magic.Intelligence.DedicatedPower));
            Assert.That(clone.Magic.Intelligence.Ego, Is.EqualTo(template.Magic.Intelligence.Ego));
            Assert.That(clone.Magic.Intelligence.IntelligenceStat, Is.EqualTo(template.Magic.Intelligence.IntelligenceStat));
            Assert.That(clone.Magic.Intelligence.Languages, Is.EquivalentTo(template.Magic.Intelligence.Languages));
            Assert.That(clone.Magic.Intelligence.Personality, Is.EqualTo(template.Magic.Intelligence.Personality));
            Assert.That(clone.Magic.Intelligence.Powers, Is.EquivalentTo(template.Magic.Intelligence.Powers));
            Assert.That(clone.Magic.Intelligence.Senses, Is.EqualTo(template.Magic.Intelligence.Senses));
            Assert.That(clone.Magic.Intelligence.SpecialPurpose, Is.EqualTo(template.Magic.Intelligence.SpecialPurpose));
            Assert.That(clone.Magic.Intelligence.WisdomStat, Is.EqualTo(template.Magic.Intelligence.WisdomStat));
            Assert.That(clone.Magic.SpecialAbilities, Is.EquivalentTo(template.Magic.SpecialAbilities));
            Assert.That(clone.Traits, Is.SupersetOf(template.Traits));
        }

        [Test]
        public void BUG_CloneIntoDoesNotOverwriteWeaponProperties()
        {
            var weapon = new Weapon();
            weapon.Ammunition = "my ammo";
            weapon.CriticalMultiplier = "x9266";
            weapon.Damages.Add(new Damage { Roll = "90210d42", Type = "emotional" });
            weapon.CriticalDamages.Add(new Damage { Roll = "600d1337", Type = "spiritual" });
            weapon.ThreatRange = 1336;
            weapon.Size = "my size";

            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);

            var newClone = template.CloneInto(weapon);
            Assert.That(newClone, Is.EqualTo(weapon));
            Assert.That(weapon, Is.Not.EqualTo(template));
            Assert.That(weapon.Name, Is.EqualTo(template.Name));

            Assert.That(weapon.Attributes, Is.EquivalentTo(template.Attributes));
            Assert.That(weapon.BaseNames, Is.EquivalentTo(template.BaseNames));
            Assert.That(weapon.Contents, Is.EquivalentTo(template.Contents));
            Assert.That(weapon.Magic.Bonus, Is.EqualTo(template.Magic.Bonus));
            Assert.That(weapon.Magic.Charges, Is.EqualTo(template.Magic.Charges));
            Assert.That(weapon.Magic.Curse, Is.EqualTo(template.Magic.Curse));
            Assert.That(weapon.Magic.Intelligence.Alignment, Is.EqualTo(template.Magic.Intelligence.Alignment));
            Assert.That(weapon.Magic.Intelligence.CharismaStat, Is.EqualTo(template.Magic.Intelligence.CharismaStat));
            Assert.That(weapon.Magic.Intelligence.Communication, Is.EquivalentTo(template.Magic.Intelligence.Communication));
            Assert.That(weapon.Magic.Intelligence.DedicatedPower, Is.EqualTo(template.Magic.Intelligence.DedicatedPower));
            Assert.That(weapon.Magic.Intelligence.Ego, Is.EqualTo(template.Magic.Intelligence.Ego));
            Assert.That(weapon.Magic.Intelligence.IntelligenceStat, Is.EqualTo(template.Magic.Intelligence.IntelligenceStat));
            Assert.That(weapon.Magic.Intelligence.Languages, Is.EquivalentTo(template.Magic.Intelligence.Languages));
            Assert.That(weapon.Magic.Intelligence.Personality, Is.EqualTo(template.Magic.Intelligence.Personality));
            Assert.That(weapon.Magic.Intelligence.Powers, Is.EquivalentTo(template.Magic.Intelligence.Powers));
            Assert.That(weapon.Magic.Intelligence.Senses, Is.EqualTo(template.Magic.Intelligence.Senses));
            Assert.That(weapon.Magic.Intelligence.SpecialPurpose, Is.EqualTo(template.Magic.Intelligence.SpecialPurpose));
            Assert.That(weapon.Magic.Intelligence.WisdomStat, Is.EqualTo(template.Magic.Intelligence.WisdomStat));
            Assert.That(weapon.Magic.SpecialAbilities, Is.EquivalentTo(template.Magic.SpecialAbilities));
            Assert.That(weapon.Traits, Is.SupersetOf(template.Traits));

            Assert.That(weapon.Ammunition, Is.EqualTo("my ammo"));
            Assert.That(weapon.CriticalMultiplier, Is.EqualTo("x9266"));
            Assert.That(weapon.Damages, Has.Count.EqualTo(1));
            Assert.That(weapon.Damages[0].Roll, Is.EqualTo("90210d42"));
            Assert.That(weapon.Damages[0].Type, Is.EqualTo("emotional"));
            Assert.That(weapon.CriticalDamages, Has.Count.EqualTo(1));
            Assert.That(weapon.CriticalDamages[0].Roll, Is.EqualTo("600d1337"));
            Assert.That(weapon.CriticalDamages[0].Type, Is.EqualTo("spiritual"));
            Assert.That(weapon.ThreatRange, Is.EqualTo(1336));
            Assert.That(weapon.Size, Is.EqualTo("my size"));
        }

        [Test]
        public void BUG_CloneIntoDoesNotOverwriteArmorProperties()
        {
            var armor = new Armor();
            armor.ArmorBonus = 9266;
            armor.ArmorCheckPenalty = -90210;
            armor.MaxDexterityBonus = 42;
            armor.Size = "my size";

            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomTemplate(name);

            var newClone = template.CloneInto(armor);
            Assert.That(newClone, Is.EqualTo(armor));
            Assert.That(armor, Is.Not.EqualTo(template));
            Assert.That(armor.Name, Is.EqualTo(template.Name));

            Assert.That(armor.Attributes, Is.EquivalentTo(template.Attributes));
            Assert.That(armor.BaseNames, Is.EquivalentTo(template.BaseNames));
            Assert.That(armor.Contents, Is.EquivalentTo(template.Contents));
            Assert.That(armor.Magic.Bonus, Is.EqualTo(template.Magic.Bonus));
            Assert.That(armor.Magic.Charges, Is.EqualTo(template.Magic.Charges));
            Assert.That(armor.Magic.Curse, Is.EqualTo(template.Magic.Curse));
            Assert.That(armor.Magic.Intelligence.Alignment, Is.EqualTo(template.Magic.Intelligence.Alignment));
            Assert.That(armor.Magic.Intelligence.CharismaStat, Is.EqualTo(template.Magic.Intelligence.CharismaStat));
            Assert.That(armor.Magic.Intelligence.Communication, Is.EquivalentTo(template.Magic.Intelligence.Communication));
            Assert.That(armor.Magic.Intelligence.DedicatedPower, Is.EqualTo(template.Magic.Intelligence.DedicatedPower));
            Assert.That(armor.Magic.Intelligence.Ego, Is.EqualTo(template.Magic.Intelligence.Ego));
            Assert.That(armor.Magic.Intelligence.IntelligenceStat, Is.EqualTo(template.Magic.Intelligence.IntelligenceStat));
            Assert.That(armor.Magic.Intelligence.Languages, Is.EquivalentTo(template.Magic.Intelligence.Languages));
            Assert.That(armor.Magic.Intelligence.Personality, Is.EqualTo(template.Magic.Intelligence.Personality));
            Assert.That(armor.Magic.Intelligence.Powers, Is.EquivalentTo(template.Magic.Intelligence.Powers));
            Assert.That(armor.Magic.Intelligence.Senses, Is.EqualTo(template.Magic.Intelligence.Senses));
            Assert.That(armor.Magic.Intelligence.SpecialPurpose, Is.EqualTo(template.Magic.Intelligence.SpecialPurpose));
            Assert.That(armor.Magic.Intelligence.WisdomStat, Is.EqualTo(template.Magic.Intelligence.WisdomStat));
            Assert.That(armor.Magic.SpecialAbilities, Is.EquivalentTo(template.Magic.SpecialAbilities));
            Assert.That(armor.Traits, Is.SupersetOf(template.Traits));

            Assert.That(armor.ArmorBonus, Is.EqualTo(9266));
            Assert.That(armor.ArmorCheckPenalty, Is.EqualTo(-90210));
            Assert.That(armor.MaxDexterityBonus, Is.EqualTo(42));
            Assert.That(armor.Size, Is.EqualTo("my size"));
        }

        [Test]
        public void NameMatchesIfName()
        {
            item.Name = "name";
            item.BaseNames = new[] { "base name", "other base name" };

            Assert.That(item.NameMatches("name"), Is.True);
        }

        [Test]
        public void NameMatchesIfAnyBaseName()
        {
            item.Name = "name";
            item.BaseNames = new[] { "base name", "other base name" };

            Assert.That(item.NameMatches("base name"), Is.True);
            Assert.That(item.NameMatches("other base name"), Is.True);
        }

        [Test]
        public void NameDoesNotMatch()
        {
            item.Name = "name";
            item.BaseNames = new[] { "base name", "other base name" };

            Assert.That(item.NameMatches("wrong name"), Is.False);
        }

        [Test]
        public void CloneCopiesIsMagical()
        {
            item.IsMagical = true;
            var clone = item.Clone();
            Assert.That(clone.IsMagical, Is.True);
        }

        [Test]
        public void SmartCloneCopiesIsMagical()
        {
            item.IsMagical = true;
            var clone = item.SmartClone();
            Assert.That(clone.IsMagical, Is.True);
        }

        [Test]
        public void Summary_MundaneItem()
        {
            item.Name = "my name";
            item.Magic.Bonus = 0;
            item.Magic.SpecialAbilities = Enumerable.Empty<SpecialAbility>();

            Assert.That(item.Summary, Is.EqualTo("my name"));
        }

        public static IEnumerable Weapons => WeaponConstants.GetAllWeapons(false, false).Select(w => new TestCaseData(w));
        public static IEnumerable Armors => ArmorConstants.GetAllArmorsAndShields(false).Select(a => new TestCaseData(a));
        public static IEnumerable AlchemicalItems => AlchemicalItemConstants.GetAllAlchemicalItems().Select(i => new TestCaseData(i));
        public static IEnumerable Potions => PotionConstants.GetAllPotions(true).Select(i => new TestCaseData(i));
        public static IEnumerable Rings => RingConstants.GetAllRings().Select(i => new TestCaseData(i));
        public static IEnumerable Rods => RodConstants.GetAllRods().Select(i => new TestCaseData(i));
        public static IEnumerable Staffs => StaffConstants.GetAllStaffs().Select(i => new TestCaseData(i));
        public static IEnumerable Tools => ToolConstants.GetAllTools().Select(i => new TestCaseData(i));
        public static IEnumerable WondrousItems => WondrousItemConstants.GetAllWondrousItems().Select(i => new TestCaseData(i));

        [TestCaseSource(nameof(Weapons))]
        [TestCaseSource(nameof(Armors))]
        [TestCaseSource(nameof(AlchemicalItems))]
        [TestCaseSource(nameof(Potions))]
        [TestCaseSource(nameof(Rings))]
        [TestCaseSource(nameof(Rods))]
        [TestCaseSource(nameof(Staffs))]
        [TestCaseSource(nameof(Tools))]
        [TestCaseSource(nameof(WondrousItems))]
        [TestCase(ItemTypeConstants.Scroll)]
        [TestCase("my name")]
        public void Summary_NonSpecificItem_Custom(string name)
        {
            item.Name = name;
            item.Magic.Bonus = 9266;
            item.Magic.SpecialAbilities =
            [
                new SpecialAbility { Name = "ability 1" },
                new SpecialAbility { Name = "ability 2" },
            ];
            item.Traits.Add(TraitConstants.SpecialMaterials.Dragonhide);
            item.Attributes = ["attribute 1", AttributeConstants.Specific, "attribute 2"];

            Assert.That(item.Summary, Is.EqualTo($"+9266 Dragonhide {name} of ability 1, ability 2"));
        }

        public static IEnumerable SpecificWeapons => WeaponConstants.GetAllSpecific().Select(w => new TestCaseData(w));
        public static IEnumerable SpecificArmors => ArmorConstants.GetAllSpecificArmorsAndShields().Select(a => new TestCaseData(a));

        [TestCaseSource(nameof(SpecificWeapons))]
        [TestCaseSource(nameof(SpecificArmors))]
        public void Summary_SpecificItem_Custom(string name)
        {
            item.Name = name;
            item.Magic.Bonus = 666;
            item.Magic.SpecialAbilities = new[]
            {
                new SpecialAbility { Name = "ability 1" },
                new SpecialAbility { Name = "ability 2" },
            };
            item.Traits.Add(TraitConstants.SpecialMaterials.Dragonhide);
            item.Attributes = new[] { "attribute 1", AttributeConstants.Specific, "attribute 2" };

            Assert.That(item.Summary, Is.EqualTo(name));
        }

        public static IEnumerable Materials => TraitConstants.SpecialMaterials.GetAll().Select(a => new TestCaseData(a));

        [TestCaseSource(nameof(Materials))]
        public void Summary_MundaneItem_SpecialMaterial(string material)
        {
            item.Name = "my name";
            item.Magic.Bonus = 0;
            item.Magic.SpecialAbilities = Enumerable.Empty<SpecialAbility>();
            item.Traits.Add(TraitConstants.Markings);
            item.Traits.Add(material);

            Assert.That(item.Summary, Is.EqualTo($"{material} my name"));
        }

        [Test]
        public void Summary_MundaneItem_SpecialMaterials()
        {
            item.Name = "my name";
            item.Magic.Bonus = 0;
            item.Magic.SpecialAbilities = Enumerable.Empty<SpecialAbility>();
            item.Traits.Add(TraitConstants.Markings);
            item.Traits.Add(TraitConstants.SpecialMaterials.Adamantine);
            item.Traits.Add(TraitConstants.SpecialMaterials.ColdIron);

            Assert.That(item.Summary, Is.EqualTo($"{TraitConstants.SpecialMaterials.Adamantine}, {TraitConstants.SpecialMaterials.ColdIron} my name"));
        }

        [Test]
        public void Summary_MagicalItem()
        {
            item.Name = "my name";
            item.Magic.Bonus = 9266;
            item.Magic.SpecialAbilities = Enumerable.Empty<SpecialAbility>();

            Assert.That(item.Summary, Is.EqualTo("+9266 my name"));
        }

        [Test]
        public void Summary_MagicalItem_NoBonus()
        {
            item.Name = "my name";
            item.Magic.Bonus = 0;
            item.Magic.SpecialAbilities = Enumerable.Empty<SpecialAbility>();
            item.IsMagical = true;

            Assert.That(item.Summary, Is.EqualTo("my name"));
        }

        [TestCaseSource(nameof(Materials))]
        public void Summary_MagicalItem_SpecialMaterial(string material)
        {
            item.Name = "my name";
            item.Magic.Bonus = 9266;
            item.Magic.SpecialAbilities = Enumerable.Empty<SpecialAbility>();
            item.Traits.Add(material);

            Assert.That(item.Summary, Is.EqualTo($"+9266 {material} my name"));
        }

        [Test]
        public void Summary_MagicalItem_SpecialMaterials()
        {
            item.Name = "my name";
            item.Magic.Bonus = 9266;
            item.Magic.SpecialAbilities = Enumerable.Empty<SpecialAbility>();
            item.Traits.Add(TraitConstants.SpecialMaterials.Dragonhide);
            item.Traits.Add(TraitConstants.SpecialMaterials.AlchemicalSilver);

            Assert.That(item.Summary, Is.EqualTo("+9266 Dragonhide, Alchemical silver my name"));
        }

        [Test]
        public void Summary_MagicItem_WithAbility()
        {
            item.Name = "my name";
            item.Magic.Bonus = 9266;
            item.Magic.SpecialAbilities = new[]
            {
                new SpecialAbility { Name = "ability 1" },
            };

            Assert.That(item.Summary, Is.EqualTo("+9266 my name of ability 1"));
        }

        [Test]
        public void Summary_MagicItem_WithAbilities()
        {
            item.Name = "my name";
            item.Magic.Bonus = 9266;
            item.Magic.SpecialAbilities = new[]
            {
                new SpecialAbility { Name = "ability 1" },
                new SpecialAbility { Name = "ability 2" },
            };

            Assert.That(item.Summary, Is.EqualTo("+9266 my name of ability 1, ability 2"));
        }

        [Test]
        public void Summary_MagicItem_WithSpecialMaterials_AndAbilities()
        {
            item.Name = "my name";
            item.Magic.Bonus = 9266;
            item.Magic.SpecialAbilities = new[]
            {
                new SpecialAbility { Name = "ability 1" },
                new SpecialAbility { Name = "ability 2" },
            };
            item.Traits.Add(TraitConstants.SpecialMaterials.Dragonhide);
            item.Traits.Add(TraitConstants.SpecialMaterials.AlchemicalSilver);

            Assert.That(item.Summary, Is.EqualTo("+9266 Dragonhide, Alchemical silver my name of ability 1, ability 2"));
        }
    }
}