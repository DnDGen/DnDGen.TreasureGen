using System.Collections.Generic;
using System.Linq;

namespace DnDGen.TreasureGen.Items.Magical
{
    public class SpecialAbility
    {
        public string Name { get; set; }
        public string BaseName { get; set; }
        public int Power { get; set; }
        public IEnumerable<string> AttributeRequirements { get; set; }
        public int BonusEquivalent { get; set; }
        public List<Damage> Damages { get; set; }
        public List<Damage> CriticalDamages { get; set; }

        public SpecialAbility()
        {
            AttributeRequirements = [];
            Name = string.Empty;
            BaseName = string.Empty;
            Damages = [];
            CriticalDamages = [];
        }

        public bool RequirementsMet(Item targetItem)
        {
            if (targetItem.Magic.Bonus + BonusEquivalent > 10)
                return false;

            if (!AttributeRequirements.Any())
                return true;

            var met = AllAttributeRequirementsMet(targetItem);

            return met;
        }

        private bool AllAttributeRequirementsMet(Item targetItem)
        {
            var allAttributes = GetAttributes(targetItem);
            return AndRequirementsMet(allAttributes) && OrRequirementsMet(allAttributes) && NotRequirementsMet(allAttributes);
        }

        private IEnumerable<string> GetAttributes(Item targetItem)
        {
            var attributes = targetItem.Attributes;

            if (targetItem is Armor)
            {
                var armor = targetItem as Armor;
                attributes = attributes.Union([armor.Size]);
            }
            else if (targetItem is Weapon)
            {
                var weapon = targetItem as Weapon;
                var damageTypes = weapon.Damages.Select(d => d.Type);

                attributes = attributes
                    .Union(damageTypes)
                    .Union(
                    [
                        weapon.Size,
                        weapon.ThreatRangeSummary,
                    ]);
            }

            return attributes;
        }

        private bool AndRequirementsMet(IEnumerable<string> itemAttributes)
        {
            var andRequirements = AttributeRequirements.Where(r => !r.Contains('/') && !r.Contains('!'));
            return !andRequirements.Any() || andRequirements.All(r => itemAttributes.Any(a => a.Contains(r)));
        }

        private bool OrRequirementsMet(IEnumerable<string> itemAttributes)
        {
            var orRequirements = AttributeRequirements.Where(r => r.Contains('/'));

            foreach (var orRequirement in orRequirements)
            {
                var options = orRequirement.Split('/');
                if (itemAttributes.Any(a => options.Any(o => a.Contains(o))) == false)
                    return false;
            }

            return true;
        }

        private bool NotRequirementsMet(IEnumerable<string> itemAttributes)
        {
            var notRequirements = AttributeRequirements.Where(r => r.StartsWith('!'));

            foreach (var notRequirement in notRequirements)
            {
                var requirement = notRequirement.Substring(1);
                if (itemAttributes.Any(a => a.Contains(requirement)))
                    return false;
            }

            return true;
        }
    }
}