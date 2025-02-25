using DnDGen.RollGen;
using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using DnDGen.TreasureGen.Selectors.Percentiles;
using NUnit.Framework;
using System;
using System.Linq;

namespace DnDGen.TreasureGen.Tests.Unit.Generators.Items
{
    public class ItemVerifier
    {
        private readonly Random random;

        public ItemVerifier()
        {
            random = new Random();
        }

        public void AssertItem(Item item)
        {
            Assert.That(item.Name, Is.Not.Empty);
            Assert.That(item.Quantity, Is.Positive, item.Summary);
            Assert.That(item.ItemType, Is.Not.Empty, $"{item.Summary} item type");
            Assert.That(item.BaseNames, Is.Not.Empty, item.Summary);
            Assert.That(item.BaseNames, Is.All.Not.Empty, item.Summary);
            Assert.That(item.BaseNames, Is.Unique, item.Summary);
            Assert.That(item.Attributes, Is.Not.Null, item.Summary);
            Assert.That(item.Attributes, Is.All.Not.Empty, item.Summary);
            Assert.That(item.Attributes, Is.Unique, item.Summary);
            Assert.That(item.Magic, Is.Not.Null, item.Summary);
            Assert.That(item.Traits, Is.Not.Null, item.Summary);
            Assert.That(item.Traits, Is.All.Not.Empty, item.Summary);
            Assert.That(item.Traits, Is.Unique, item.Summary);
            Assert.That(item.Contents, Is.Not.Null, item.Summary);
            Assert.That(item.Contents, Is.All.Not.Empty, item.Summary);

            if (!item.CanBeUsedAsWeaponOrArmor)
                Assert.That(item.Magic.SpecialAbilities, Is.Empty, item.Summary);

            foreach (var ability in item.Magic.SpecialAbilities)
            {
                Assert.That(ability.Name, Is.Not.Empty, $"{item.Summary} ability name");
                Assert.That(ability.BonusEquivalent, Is.Not.Negative, ability.Name);
                Assert.That(ability.AttributeRequirements, Is.Not.Null, ability.Name);
                Assert.That(ability.AttributeRequirements, Is.All.Not.Empty, ability.Name);
                Assert.That(ability.BaseName, Is.Not.Empty, $"{item.Summary} with {ability.Name}");
                Assert.That(ability.Power, Is.Not.Negative, ability.Name);

                if (ability.BaseName == SpecialAbilityConstants.Bane)
                {
                    Assert.That(ability.Damages.Select(d => d.Condition), Is.All.Not.Empty, $"{item.Summary} with {ability.Name}");
                    Assert.That(ability.CriticalDamages.Select(d => d.Condition), Is.All.Not.Empty, $"{item.Summary} with {ability.Name}");
                }
            }

            AssertArmor(item);
            AssertWeapon(item);
            AssertSpecialMaterials(item);
        }

        private void AssertArmor(Item item)
        {
            if (!(item is Armor))
                return;

            var armor = item as Armor;
            Assert.That(armor.ArmorBonus, Is.Positive, armor.Summary);
            Assert.That(armor.CanBeUsedAsWeaponOrArmor, Is.True, armor.Summary);
            Assert.That(armor.Size, Is.Not.Empty, armor.Summary);
            Assert.That(armor.MaxDexterityBonus, Is.Not.Negative, armor.Summary);
            Assert.That(armor.ArmorCheckPenalty, Is.Not.Positive, armor.Summary);

            if (armor.IsMagical)
                Assert.That(armor.Traits, Contains.Item(TraitConstants.Masterwork), armor.Summary);
        }

        private void AssertWeapon(Item item)
        {
            if (!(item is Weapon))
                return;

            var weapon = item as Weapon;
            Assert.That(weapon.CanBeUsedAsWeaponOrArmor, Is.True, weapon.Summary);
            Assert.That(weapon.CriticalMultiplier, Is.EqualTo("x2").Or.EqualTo("x3").Or.EqualTo("x4"), $"{weapon.Summary} critical multiplier");
            Assert.That(weapon.Size, Is.Not.Empty, $"{weapon.Summary} size");
            Assert.That(weapon.Damages, Is.Not.Empty
                .And.Count.EqualTo(1 + weapon.Magic.SpecialAbilities.SelectMany(a => a.Damages).Count()), $"{weapon.Summary} damages: {weapon.DamageSummary}");
            Assert.That(weapon.CriticalDamages, Is.Not.Empty
                .And.Count.EqualTo(1 + weapon.Magic.SpecialAbilities
                    .Where(a => a.CriticalDamages.Any())
                    .SelectMany(a => a.CriticalDamages)
                    .Count()), $"{weapon.Summary} critical damages: {weapon.CriticalDamageSummary}");

            foreach (var damage in weapon.Damages)
            {
                Assert.That(damage.Roll, Is.Not.Empty, weapon.Summary);
                Assert.That(damage.Type, Is.Not.Empty, weapon.Summary);
                Assert.That(damage.Condition, Does.Not.Contain(ReplacementStringConstants.DesignatedFoe), weapon.Summary);
            }

            foreach (var damage in weapon.CriticalDamages)
            {
                Assert.That(damage.Roll, Is.Not.Empty, weapon.Summary);
                Assert.That(damage.Type, Is.Not.Empty, weapon.Summary);
                Assert.That(damage.Condition, Does.Not.Contain(ReplacementStringConstants.DesignatedFoe), weapon.Summary);
            }

            Assert.That(weapon.Damages[0].Type, Contains.Substring(AttributeConstants.DamageTypes.Bludgeoning)
                .Or.Contains(AttributeConstants.DamageTypes.Piercing)
                .Or.Contains(AttributeConstants.DamageTypes.Slashing), weapon.Summary);
            Assert.That(weapon.CriticalDamages[0].Type, Contains.Substring(AttributeConstants.DamageTypes.Bludgeoning)
                .Or.Contains(AttributeConstants.DamageTypes.Piercing)
                .Or.Contains(AttributeConstants.DamageTypes.Slashing), weapon.Summary);

            Assert.That(weapon.ThreatRange, Is.Positive.And.InRange(1, 6), $"{weapon.Summary} threat range");
            Assert.That(weapon.ThreatRangeSummary, Is.Not.Empty, $"{weapon.Summary} threat range summary");
            Assert.That(weapon.Ammunition, Is.Not.Null, weapon.Summary);

            if (weapon.IsMagical)
                Assert.That(weapon.Traits, Contains.Item(TraitConstants.Masterwork), weapon.Summary);

            Assert.That(weapon.Attributes, Is.Not.Empty
                .And.Not.Contains(AttributeConstants.DamageTypes.Bludgeoning)
                .And.Not.Contains(AttributeConstants.DamageTypes.Piercing)
                .And.Not.Contains(AttributeConstants.DamageTypes.Slashing), $"{weapon.Summary} attributes");

            Assert.That(weapon.CombatTypes, Is.Not.Empty, $"{weapon.Summary} combat types");
            Assert.That(weapon.CombatTypes, Contains.Item(AttributeConstants.Melee).Or.Contains(AttributeConstants.Ranged), weapon.Summary);
            Assert.That(weapon.CombatTypes.Count(), Is.InRange(1, 2), weapon.Summary);

            if (weapon.Attributes.Contains(AttributeConstants.DoubleWeapon))
            {
                Assert.That(weapon.SecondaryCriticalMultiplier, Is.EqualTo("x2").Or.EqualTo("x3").Or.EqualTo("x4"), $"{weapon.Summary} secondary critical multiplier");
                Assert.That(weapon.SecondaryDamages, Is.Not.Empty, $"{weapon.Summary} secondary damages: {weapon.SecondaryDamageSummary}");
                Assert.That(weapon.SecondaryCriticalDamages, Is.Not.Empty, $"{weapon.Summary} secondary critical damages: {weapon.SecondaryCriticalDamageSummary}");

                foreach (var damage in weapon.SecondaryDamages)
                {
                    Assert.That(damage.Roll, Is.Not.Empty, weapon.Summary);
                    Assert.That(damage.Type, Is.Not.Empty, weapon.Summary);
                    Assert.That(damage.Condition, Does.Not.Contain(ReplacementStringConstants.DesignatedFoe), weapon.Summary);
                }

                foreach (var damage in weapon.SecondaryCriticalDamages)
                {
                    Assert.That(damage.Roll, Is.Not.Empty, weapon.Summary);
                    Assert.That(damage.Type, Is.Not.Empty, weapon.Summary);
                    Assert.That(damage.Condition, Does.Not.Contain(ReplacementStringConstants.DesignatedFoe), weapon.Summary);
                }

                Assert.That(weapon.SecondaryDamages[0].Type, Contains.Substring(AttributeConstants.DamageTypes.Bludgeoning)
                    .Or.Contains(AttributeConstants.DamageTypes.Piercing)
                    .Or.Contains(AttributeConstants.DamageTypes.Slashing), weapon.Summary);
                Assert.That(weapon.SecondaryCriticalDamages[0].Type, Contains.Substring(AttributeConstants.DamageTypes.Bludgeoning)
                    .Or.Contains(AttributeConstants.DamageTypes.Piercing)
                    .Or.Contains(AttributeConstants.DamageTypes.Slashing), weapon.Summary);

                if (weapon.SecondaryHasAbilities)
                {
                    Assert.That(weapon.SecondaryMagicBonus, Is.EqualTo(weapon.Magic.Bonus), weapon.Summary);
                    Assert.That(weapon.SecondaryDamages, Is.Not.Empty
                        .And.Count.EqualTo(1 + weapon.Magic.SpecialAbilities.SelectMany(a => a.Damages).Count()), $"{weapon.Summary} secondary damages: {weapon.SecondaryDamageSummary}");
                    Assert.That(weapon.SecondaryCriticalDamages, Is.Not.Empty
                        .And.Count.EqualTo(1 + weapon.Magic.SpecialAbilities
                            .Where(a => a.CriticalDamages.Any())
                            .SelectMany(a => a.CriticalDamages)
                            .Count()), $"{weapon.Summary} secondary critical damages: {weapon.SecondaryCriticalDamageSummary}");
                }
                else if (weapon.Magic.Bonus > 0)
                {
                    Assert.That(weapon.SecondaryMagicBonus, Is.EqualTo(weapon.Magic.Bonus - 1), weapon.Summary);
                }
                else
                {
                    Assert.That(weapon.SecondaryMagicBonus, Is.Zero, weapon.Summary);
                    Assert.That(weapon.SecondaryDamages, Is.Not.Empty
                        .And.Count.EqualTo(1), $"{weapon.Summary} secondary damages: {weapon.SecondaryDamageSummary}");
                    Assert.That(weapon.SecondaryCriticalDamages, Is.Not.Empty
                        .And.Count.EqualTo(1), $"{weapon.Summary} secondary critical damages: {weapon.SecondaryCriticalDamageSummary}");
                }
            }
            else
            {
                Assert.That(weapon.SecondaryDamages, Is.Empty, weapon.Summary);
                Assert.That(weapon.SecondaryCriticalDamages, Is.Empty, weapon.Summary);
                Assert.That(weapon.SecondaryCriticalMultiplier, Is.Empty, weapon.Summary);
                Assert.That(weapon.SecondaryMagicBonus, Is.Zero, weapon.Summary);
            }
        }

        private void AssertSpecialMaterials(Item item)
        {
            if (item.Traits.Contains(TraitConstants.SpecialMaterials.Adamantine))
            {
                Assert.That(item.Attributes, Contains.Item(AttributeConstants.Metal), item.Summary);
                Assert.That(item.Traits, Contains.Item(TraitConstants.Masterwork), item.Summary);
            }

            if (item.Traits.Contains(TraitConstants.SpecialMaterials.Darkwood))
            {
                Assert.That(item.Attributes, Contains.Item(AttributeConstants.Wood), item.Summary);
                Assert.That(item.Traits, Contains.Item(TraitConstants.Masterwork), item.Summary);
            }

            if (item.Traits.Contains(TraitConstants.SpecialMaterials.Dragonhide))
            {
                Assert.That(item.Attributes, Does.Not.Contain(AttributeConstants.Metal)
                    .And.Not.Contains(AttributeConstants.Wood), $"{item.Name} of Dragonhide");
                Assert.That(item.Traits, Contains.Item(TraitConstants.Masterwork), $"{item.Summary} of Dragonhide");
            }

            if (item.Traits.Contains(TraitConstants.SpecialMaterials.ColdIron))
            {
                Assert.That(item.Attributes, Contains.Item(AttributeConstants.Metal), item.Summary);
            }

            if (item.Traits.Contains(TraitConstants.SpecialMaterials.Mithral))
            {
                Assert.That(item.Attributes, Contains.Item(AttributeConstants.Metal), item.Summary);
                Assert.That(item.Traits, Contains.Item(TraitConstants.Masterwork), item.Summary);
            }

            if (item.Traits.Contains(TraitConstants.SpecialMaterials.AlchemicalSilver))
            {
                Assert.That(item.Attributes, Contains.Item(AttributeConstants.Metal));
            }
        }

        public void AssertIntelligence(Intelligence intelligence)
        {
            Assert.That(intelligence.Ego, Is.Positive);
            Assert.That(intelligence.Alignment, Is.Not.Empty);
            Assert.That(intelligence.CharismaStat, Is.AtLeast(10));
            Assert.That(intelligence.WisdomStat, Is.AtLeast(10));
            Assert.That(intelligence.IntelligenceStat, Is.AtLeast(10));
            Assert.That(intelligence.Communication, Is.Not.Empty
                .And.All.Not.Empty
                .And.Unique);
            Assert.That(intelligence.Personality, Is.Not.Null);
            Assert.That(intelligence.Powers, Is.Not.Empty
                .And.All.Not.Empty
                .And.Unique);
            Assert.That(intelligence.Senses, Is.Not.Empty);
        }

        public void AssertSpecificCursedItem(Item item)
        {
            var materials = TraitConstants.SpecialMaterials.GetAll();

            AssertItem(item);
            Assert.That(item.Magic.Curse, Is.Not.Empty.And.EqualTo(CurseConstants.SpecificCursedItem), item.Summary);
            Assert.That(item.Attributes, Contains.Item(AttributeConstants.Specific), item.Summary);
            Assert.That(item.Traits.Intersect(materials), Is.Empty, item.Summary);
        }

        public Armor CreateRandomArmorTemplate(string name)
        {
            var template = new Armor
            {
                Name = name
            };
            template = PopulateItem(template) as Armor;

            template.ArmorBonus = random.Next();
            template.ArmorCheckPenalty = -random.Next();
            template.MaxDexterityBonus = random.Next();

            return template;
        }

        public Weapon CreateRandomWeaponTemplate(string name)
        {
            var template = new Weapon
            {
                Name = name
            };
            template = PopulateItem(template) as Weapon;

            var damageTypes = new[] { AttributeConstants.DamageTypes.Bludgeoning, AttributeConstants.DamageTypes.Piercing, AttributeConstants.DamageTypes.Slashing };
            template.CriticalMultiplier = $"x{random.Next(3) + 2}";
            template.Damages.Add(new Damage
            {
                Roll = $"{random.Next(Limits.Quantity) + 1}d{random.Next(Limits.Die) + 1}",
                Type = $"{damageTypes[random.Next(3)]} {Guid.NewGuid()}"
            });
            template.CriticalDamages.Add(new Damage
            {
                Roll = $"{random.Next(Limits.Quantity) + 1}d{random.Next(Limits.Die) + 1}",
                Type = $"{damageTypes[random.Next(3)]} {Guid.NewGuid()}"
            });
            template.ThreatRange = random.Next(3) + 1;
            template.Size = $"size {Guid.NewGuid()}";

            return template;
        }

        public Item CreateRandomTemplate(string name)
        {
            var template = new Item
            {
                Name = name
            };
            template = PopulateItem(template);

            return template;
        }

        private Item PopulateItem(Item item)
        {
            item.BaseNames = [Guid.NewGuid().ToString(), Guid.NewGuid().ToString()];
            item.Quantity = random.Next();
            item.Contents.Add(Guid.NewGuid().ToString());
            item.Contents.Add(Guid.NewGuid().ToString());
            item.IsMagical = true;
            item.ItemType = Guid.NewGuid().ToString();
            item.Magic.Bonus = random.Next();
            item.Magic.Charges = random.Next();
            item.Magic.Curse = Guid.NewGuid().ToString();
            item.Magic.Intelligence.Alignment = Guid.NewGuid().ToString();
            item.Magic.Intelligence.CharismaStat = random.Next();
            item.Magic.Intelligence.Communication = new[] { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() };
            item.Magic.Intelligence.DedicatedPower = Guid.NewGuid().ToString();
            item.Magic.Intelligence.Ego = random.Next();
            item.Magic.Intelligence.IntelligenceStat = random.Next();
            item.Magic.Intelligence.Languages.Add(Guid.NewGuid().ToString());
            item.Magic.Intelligence.Languages.Add(Guid.NewGuid().ToString());
            item.Magic.Intelligence.Personality = Guid.NewGuid().ToString();
            item.Magic.Intelligence.Powers.Add(Guid.NewGuid().ToString());
            item.Magic.Intelligence.Powers.Add(Guid.NewGuid().ToString());
            item.Magic.Intelligence.Senses = Guid.NewGuid().ToString();
            item.Magic.Intelligence.SpecialPurpose = Guid.NewGuid().ToString();
            item.Magic.Intelligence.WisdomStat = random.Next();
            item.Magic.SpecialAbilities = new[]
            {
                new SpecialAbility { Name = Guid.NewGuid().ToString() },
                new SpecialAbility { Name = Guid.NewGuid().ToString() }
            };

            item.Traits.Add(Guid.NewGuid().ToString());
            item.Traits.Add(Guid.NewGuid().ToString());
            item.Attributes = new[] { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() };

            return item;
        }

        public void AssertMagicalItemFromTemplate(Item item, Item template)
        {
            Assert.That(item, Is.Not.EqualTo(template), item.Summary);
            Assert.That(item.IsMagical, Is.True, item.Summary);
            Assert.That(item.Magic.Curse, Is.EqualTo(template.Magic.Curse).Or.EqualTo(CurseConstants.SpecificCursedItem), item.Summary);

            var sizes = TraitConstants.Sizes.GetAll();
            Assert.That(item.Traits, Is.SupersetOf(template.Traits.Except(sizes)), item.Summary);

            if (item.Attributes.Contains(AttributeConstants.OneTimeUse) || item.Attributes.Contains(AttributeConstants.Ammunition))
            {
                AssertNoIntelligence(item.Magic.Intelligence, item.Summary);
            }
            else
            {
                Assert.That(item.Magic.Intelligence.Alignment, Is.EqualTo(template.Magic.Intelligence.Alignment), item.Summary);
                Assert.That(item.Magic.Intelligence.CharismaStat, Is.EqualTo(template.Magic.Intelligence.CharismaStat), item.Summary);
                Assert.That(item.Magic.Intelligence.Communication, Is.EquivalentTo(template.Magic.Intelligence.Communication), item.Summary);
                Assert.That(item.Magic.Intelligence.DedicatedPower, Is.EqualTo(template.Magic.Intelligence.DedicatedPower), item.Summary);
                Assert.That(item.Magic.Intelligence.Ego, Is.EqualTo(template.Magic.Intelligence.Ego), item.Summary);
                Assert.That(item.Magic.Intelligence.IntelligenceStat, Is.EqualTo(template.Magic.Intelligence.IntelligenceStat), item.Summary);
                Assert.That(item.Magic.Intelligence.Languages, Is.EquivalentTo(template.Magic.Intelligence.Languages), item.Summary);
                Assert.That(item.Magic.Intelligence.Personality, Is.EqualTo(template.Magic.Intelligence.Personality), item.Summary);
                Assert.That(item.Magic.Intelligence.Powers, Is.EquivalentTo(template.Magic.Intelligence.Powers), item.Summary);
                Assert.That(item.Magic.Intelligence.Senses, Is.EqualTo(template.Magic.Intelligence.Senses), item.Summary);
                Assert.That(item.Magic.Intelligence.SpecialPurpose, Is.EqualTo(template.Magic.Intelligence.SpecialPurpose), item.Summary);
                Assert.That(item.Magic.Intelligence.WisdomStat, Is.EqualTo(template.Magic.Intelligence.WisdomStat), item.Summary);
            }
        }

        private void AssertNoIntelligence(Intelligence intelligence, string name)
        {
            Assert.That(intelligence.Alignment, Is.Empty, name);
            Assert.That(intelligence.CharismaStat, Is.EqualTo(0), name);
            Assert.That(intelligence.Communication, Is.Empty, name);
            Assert.That(intelligence.DedicatedPower, Is.Empty, name);
            Assert.That(intelligence.Ego, Is.EqualTo(0), name);
            Assert.That(intelligence.IntelligenceStat, Is.EqualTo(0), name);
            Assert.That(intelligence.Languages, Is.Empty, name);
            Assert.That(intelligence.Personality, Is.Empty, name);
            Assert.That(intelligence.Powers, Is.Empty, name);
            Assert.That(intelligence.Senses, Is.Empty, name);
            Assert.That(intelligence.SpecialPurpose, Is.Empty, name);
            Assert.That(intelligence.WisdomStat, Is.EqualTo(0), name);
        }

        public void AssertMundaneItemFromTemplate(Item item, Item template)
        {
            Assert.That(item, Is.Not.EqualTo(template), item.Summary);
            Assert.That(item.Name, Is.EqualTo(template.Name));
            Assert.That(item.Contents, Is.EquivalentTo(template.Contents), item.Summary);
            Assert.That(item.IsMagical, Is.False, item.Summary);
            Assert.That(item.Magic.Bonus, Is.EqualTo(0), item.Summary);
            Assert.That(item.Magic.Charges, Is.EqualTo(0), item.Summary);
            Assert.That(item.Magic.Curse, Is.Empty, item.Summary);
            AssertNoIntelligence(item.Magic.Intelligence, item.Summary);
            Assert.That(item.Magic.SpecialAbilities, Is.Empty, item.Summary);

            if (!(item is Armor))
                Assert.That(template.Traits, Is.SubsetOf(item.Traits), item.Summary);
        }
    }
}
