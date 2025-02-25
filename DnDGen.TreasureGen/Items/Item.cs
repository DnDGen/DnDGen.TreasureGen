using DnDGen.TreasureGen.Items.Magical;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.TreasureGen.Items
{
    public class Item
    {
        public string Name { get; set; }
        public IEnumerable<string> BaseNames { get; set; }
        public string ItemType { get; set; }
        public HashSet<string> Traits { get; set; }
        public IEnumerable<string> Attributes { get; set; }
        public Magic Magic { get; set; }
        public int Quantity { get; set; }
        public List<string> Contents { get; set; }

        public string Summary
        {
            get
            {
                var summary = Name;

                var specificNames = WeaponConstants.GetAllSpecific()
                    .Union(ArmorConstants.GetAllSpecificArmorsAndShields());

                if (specificNames.Contains(Name))
                    return summary;

                var specialMaterials = TraitConstants.SpecialMaterials.GetAll();
                var materialTraits = Traits.Intersect(specialMaterials);
                if (materialTraits.Any())
                {
                    summary = $"{string.Join(", ", materialTraits)} {summary}";
                }

                if (Magic.Bonus > 0)
                {
                    summary = $"+{Magic.Bonus} {summary}";
                }

                if (Magic.SpecialAbilities.Any())
                {
                    var abilities = string.Join(", ", Magic.SpecialAbilities.Select(a => a.Name));
                    summary = $"{summary} of {abilities}";
                }

                return summary;
            }
        }

        public bool IsMagical
        {
            get
            {
                return isMagical
                       || Magic.Bonus > 0
                       || Magic.Charges > 0
                       || !string.IsNullOrEmpty(Magic.Curse)
                       || Magic.Intelligence.Ego > 0
                       || Magic.SpecialAbilities.Any();
            }
            set
            {
                isMagical = value;
            }
        }

        public virtual bool CanBeUsedAsWeaponOrArmor
        {
            get
            {
                var allWeapons = WeaponConstants.GetAllWeapons(true, true);
                var allArmors = ArmorConstants.GetAllArmorsAndShields(true);

                return ItemType == ItemTypeConstants.Weapon
                    || ItemType == ItemTypeConstants.Armor
                    || allWeapons.Contains(Name)
                    || allWeapons.Intersect(BaseNames).Any()
                    || allArmors.Contains(Name)
                    || allArmors.Intersect(BaseNames).Any();
            }
        }

        private bool isMagical;

        public Item()
        {
            Traits = [];
            Attributes = [];
            Magic = new Magic();
            Quantity = 1;
            Name = string.Empty;
            BaseNames = [];
            Contents = [];
            ItemType = string.Empty;
        }

        public virtual Item CloneInto(Item target)
        {
            var clone = MundaneCloneInto(target);
            clone.Magic.Bonus = Magic.Bonus;
            clone.Magic.Charges = Magic.Charges;
            clone.Magic.Curse = Magic.Curse;
            clone.Magic.Intelligence = Magic.Intelligence.Clone();
            clone.Magic.SpecialAbilities = Magic.SpecialAbilities.ToArray();
            clone.IsMagical = isMagical;

            return clone;
        }

        public virtual Item Clone()
        {
            var clone = new Item();
            clone = CloneInto(clone);

            return clone;
        }

        public virtual Item SmartClone()
        {
            var clone = new Item();
            clone = SmartCloneInto(clone);

            return clone;
        }

        public virtual Item SmartCloneInto(Item target)
        {
            var clone = MundaneCloneInto(target);

            clone.Magic.Curse = Magic.Curse;
            clone.IsMagical = isMagical;

            if (clone.CanBeUsedAsWeaponOrArmor)
                clone.Magic.SpecialAbilities = Magic.SpecialAbilities.ToArray();

            if (ItemType == ItemTypeConstants.Wand)
                clone.Contents.Clear();

            if (ItemType != ItemTypeConstants.Potion && ItemType != ItemTypeConstants.Scroll)
                clone.Magic.Charges = Magic.Charges;

            if (ItemType != ItemTypeConstants.Wand && ItemType != ItemTypeConstants.Scroll)
                clone.Magic.Bonus = Magic.Bonus;

            var nonIntelligenceItems = new[] { AttributeConstants.OneTimeUse, AttributeConstants.Ammunition };
            if (Attributes.Intersect(nonIntelligenceItems).Any() == false)
                clone.Magic.Intelligence = Magic.Intelligence.Clone();

            return clone;
        }

        public virtual Item MundaneClone()
        {
            var clone = new Item();
            clone = MundaneCloneInto(clone);

            return clone;
        }

        public virtual Item MundaneCloneInto(Item target)
        {
            if (target == null)
                throw new ArgumentException("Clone target cannot be null");

            target.Attributes = Attributes.ToArray();
            target.Contents.AddRange(Contents);
            target.ItemType = ItemType;
            target.Name = Name;
            target.BaseNames = BaseNames.ToArray();

            if (target is Weapon)
            {
                target.Quantity = Quantity != 1 ? Quantity : target.Quantity;
            }
            else
            {
                target.Quantity = Quantity;
            }

            foreach (var trait in Traits)
            {
                target.Traits.Add(trait);
            }

            return target;
        }

        public bool NameMatches(string name)
        {
            return Name == name || BaseNames.Contains(name);
        }
    }
}