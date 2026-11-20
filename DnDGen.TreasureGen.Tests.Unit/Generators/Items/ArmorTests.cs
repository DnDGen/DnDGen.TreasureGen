using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using NUnit.Framework;
using System;
using System.Text.Json;

namespace DnDGen.TreasureGen.Tests.Unit.Generators.Items
{
    [TestFixture]
    public class ArmorTests
    {
        private Armor armor;
        private ItemVerifier itemVerifier;

        [SetUp]
        public void Setup()
        {
            armor = new Armor();
            itemVerifier = new ItemVerifier();
        }

        [Test]
        public void ArmorInitialized()
        {
            Assert.That(armor.ArmorBonus, Is.EqualTo(0));
            Assert.That(armor.Size, Is.Empty);
            Assert.That(armor.MaxDexterityBonus, Is.EqualTo(0));
            Assert.That(armor.ArmorCheckPenalty, Is.EqualTo(0));
            Assert.That(armor.ItemType, Is.EqualTo(ItemTypeConstants.Armor));
        }

        [Test]
        public void ArmorIsItem()
        {
            Assert.That(armor, Is.InstanceOf<Item>());
        }

        [Test]
        public void ArmorCanBeUsedAsArmorOrWeapon()
        {
            Assert.That(armor.CanBeUsedAsWeaponOrArmor, Is.True);
        }

        [Test]
        public void TotalArmorBonusIncludesArmorBonus()
        {
            armor.ArmorBonus = 9266;
            Assert.That(armor.TotalArmorBonus, Is.EqualTo(9266));
        }

        [Test]
        public void TotalArmorBonusIncludesMagicBonus()
        {
            armor.ArmorBonus = 9266;
            armor.Magic.Bonus = 90210;
            Assert.That(armor.TotalArmorBonus, Is.EqualTo(9266 + 90210));
        }

        [Test]
        public void TotalArmorBonusButCursedWithDelusionIncludesMagicBonus()
        {
            armor.ArmorBonus = 9266;
            armor.Magic.Bonus = 90210;
            armor.Magic.Curse = CurseConstants.Delusion;

            Assert.That(armor.TotalArmorBonus, Is.EqualTo(9266));
        }

        [Test]
        public void TotalArmorBonusButCursedWithOppositeEffectIncludesMagicBonus()
        {
            armor.ArmorBonus = 9266;
            armor.Magic.Bonus = 90210;
            armor.Magic.Curse = CurseConstants.OppositeEffect;

            Assert.That(armor.TotalArmorBonus, Is.EqualTo(9266 - 90210));
        }

        [Test]
        public void TotalArmorCheckPenaltyIncludesArmorCheckPenalty()
        {
            armor.ArmorCheckPenalty = -9266;
            Assert.That(armor.TotalArmorCheckPenalty, Is.EqualTo(-9266));
        }

        [Test]
        public void TotalArmorCheckPenaltyWithMasterworkIs1Less()
        {
            armor.ArmorCheckPenalty = -9266;
            armor.Traits.Add(TraitConstants.Masterwork);
            Assert.That(armor.TotalArmorCheckPenalty, Is.EqualTo(-9265));
        }

        [TestCase(TraitConstants.SpecialMaterials.Adamantine, 0)]
        [TestCase(TraitConstants.SpecialMaterials.AlchemicalSilver, 0)]
        [TestCase(TraitConstants.SpecialMaterials.ColdIron, 0)]
        [TestCase(TraitConstants.SpecialMaterials.Darkwood, 1)]
        [TestCase(TraitConstants.SpecialMaterials.Dragonhide, 0)]
        [TestCase(TraitConstants.SpecialMaterials.Mithral, 2)]
        public void TotalArmorCheckPenaltyWithSpecialMaterial(string material, int decrease)
        {
            armor.ArmorCheckPenalty = -9266;
            armor.Traits.Add(material);
            Assert.That(armor.TotalArmorCheckPenalty, Is.EqualTo(-9266 + decrease));
        }

        [Test]
        public void TotalArmorCheckPenaltyIsCumulative()
        {
            armor.ArmorCheckPenalty = -9266;
            armor.Traits.Add(TraitConstants.Masterwork);
            armor.Traits.Add(TraitConstants.SpecialMaterials.Darkwood);

            Assert.That(armor.TotalArmorCheckPenalty, Is.EqualTo(-9264));
        }

        [Test]
        public void TotalArmorCheckPenaltyCannotBePositive()
        {
            armor.ArmorCheckPenalty = -2;
            armor.Traits.Add(TraitConstants.Masterwork);
            armor.Traits.Add(TraitConstants.SpecialMaterials.Mithral);

            Assert.That(armor.TotalArmorCheckPenalty, Is.EqualTo(0));
        }

        [Test]
        public void TotalMaxDexterityBonusIncludesMaxDexterityBonus()
        {
            armor.MaxDexterityBonus = 9266;
            Assert.That(armor.TotalMaxDexterityBonus, Is.EqualTo(9266));
        }

        [TestCase(TraitConstants.SpecialMaterials.Adamantine, 0)]
        [TestCase(TraitConstants.SpecialMaterials.AlchemicalSilver, 0)]
        [TestCase(TraitConstants.SpecialMaterials.ColdIron, 0)]
        [TestCase(TraitConstants.SpecialMaterials.Darkwood, 0)]
        [TestCase(TraitConstants.SpecialMaterials.Dragonhide, 0)]
        [TestCase(TraitConstants.SpecialMaterials.Mithral, 2)]
        public void TotalMaxDexterityBonusWithSpecialMaterial(string material, int increase)
        {
            armor.MaxDexterityBonus = 9266;
            armor.Traits.Add(material);
            Assert.That(armor.TotalMaxDexterityBonus, Is.EqualTo(9266 + increase));
        }

        [Test]
        public void CloneIsSuccessful()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomArmorTemplate(name);

            template.ArmorBonus = 9266;
            template.ArmorCheckPenalty = -90210;
            template.MaxDexterityBonus = 42;
            template.Size = "massive";

            var clone = template.Clone();
            Assert.That(clone, Is.Not.SameAs(template));
            Assert.That(JsonSerializer.Serialize(clone as Armor), Is.EqualTo(JsonSerializer.Serialize(template)));
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

            var cloneArmor = clone as Armor;
            Assert.That(cloneArmor.ArmorBonus, Is.EqualTo(template.ArmorBonus));
            Assert.That(cloneArmor.ArmorCheckPenalty, Is.EqualTo(template.ArmorCheckPenalty));
            Assert.That(cloneArmor.MaxDexterityBonus, Is.EqualTo(template.MaxDexterityBonus));
            Assert.That(cloneArmor.Size, Is.EqualTo(template.Size));
        }

        [Test]
        public void MundaneCloneIsSuccessful()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomArmorTemplate(name);
            template.ItemType = Guid.NewGuid().ToString();
            template.Magic.SpecialAbilities =
            [
                new SpecialAbility { Name = Guid.NewGuid().ToString() },
                new SpecialAbility { Name = Guid.NewGuid().ToString() }
            ];

            template.Attributes = [Guid.NewGuid().ToString(), Guid.NewGuid().ToString()];
            template.ArmorBonus = 9266;
            template.ArmorCheckPenalty = -90210;
            template.MaxDexterityBonus = 42;
            template.Size = "massive";

            var clone = template.MundaneClone();
            ItemVerifier.AssertMundaneItemFromTemplate(clone, template);
            Assert.That(clone.ItemType, Is.EqualTo(template.ItemType));
            Assert.That(clone.Attributes, Is.EquivalentTo(template.Attributes));
            Assert.That(clone.Magic.SpecialAbilities, Is.Empty);

            var cloneArmor = clone as Armor;
            Assert.That(cloneArmor.ArmorBonus, Is.EqualTo(template.ArmorBonus));
            Assert.That(cloneArmor.ArmorCheckPenalty, Is.EqualTo(template.ArmorCheckPenalty));
            Assert.That(cloneArmor.MaxDexterityBonus, Is.EqualTo(template.MaxDexterityBonus));
            Assert.That(cloneArmor.Size, Is.EqualTo(template.Size));
        }

        [Test]
        public void SmartCloneIsSuccessful()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomArmorTemplate(name);
            template.ItemType = Guid.NewGuid().ToString();

            template.Attributes = [Guid.NewGuid().ToString(), Guid.NewGuid().ToString()];
            template.ArmorBonus = 9266;
            template.ArmorCheckPenalty = -90210;
            template.MaxDexterityBonus = 42;
            template.Size = "massive";

            var clone = template.SmartClone();
            ItemVerifier.AssertMagicalItemFromTemplate(clone, template);
            Assert.That(clone.ItemType, Is.EqualTo(template.ItemType));
            Assert.That(clone.Attributes, Is.EquivalentTo(template.Attributes));

            var cloneArmor = clone as Armor;
            Assert.That(cloneArmor.ArmorBonus, Is.EqualTo(template.ArmorBonus));
            Assert.That(cloneArmor.ArmorCheckPenalty, Is.EqualTo(template.ArmorCheckPenalty));
            Assert.That(cloneArmor.MaxDexterityBonus, Is.EqualTo(template.MaxDexterityBonus));
            Assert.That(cloneArmor.Size, Is.EqualTo(template.Size));
        }

        [Test]
        public void CloneIntoCopiesProperties()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomArmorTemplate(name);

            template.ArmorBonus = 9266;
            template.ArmorCheckPenalty = -90210;
            template.MaxDexterityBonus = 42;
            template.Size = "massive";

            var clone = new Armor();
            var newClone = template.CloneInto(clone);
            Assert.That(newClone, Is.EqualTo(clone));
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

            Assert.That(clone.ArmorBonus, Is.EqualTo(template.ArmorBonus));
            Assert.That(clone.ArmorCheckPenalty, Is.EqualTo(template.ArmorCheckPenalty));
            Assert.That(clone.MaxDexterityBonus, Is.EqualTo(template.MaxDexterityBonus));
            Assert.That(clone.Size, Is.EqualTo(template.Size));
        }

        [Test]
        public void BUG_CloneIntoPrefersNonemptyProperties()
        {
            var name = Guid.NewGuid().ToString();
            var template = itemVerifier.CreateRandomArmorTemplate(name);

            template.ArmorBonus = 0;
            template.ArmorCheckPenalty = 0;
            template.MaxDexterityBonus = 0;
            template.Size = string.Empty;

            var clone = new Armor
            {
                ArmorBonus = 9266,
                ArmorCheckPenalty = -90210,
                MaxDexterityBonus = 42,
                Size = "massive"
            };

            var newClone = template.CloneInto(clone);
            Assert.That(newClone, Is.EqualTo(clone));
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

            var cloneWeapon = clone;
            Assert.That(cloneWeapon.ArmorBonus, Is.EqualTo(9266));
            Assert.That(cloneWeapon.ArmorCheckPenalty, Is.EqualTo(-90210));
            Assert.That(cloneWeapon.MaxDexterityBonus, Is.EqualTo(42));
            Assert.That(cloneWeapon.Size, Is.EqualTo("massive"));
        }
    }
}
