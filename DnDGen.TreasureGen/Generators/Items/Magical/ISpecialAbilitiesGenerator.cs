using DnDGen.TreasureGen.Items;
using DnDGen.TreasureGen.Items.Magical;
using System.Collections.Generic;

namespace DnDGen.TreasureGen.Generators.Items.Magical
{
    internal interface ISpecialAbilitiesGenerator
    {
        IEnumerable<SpecialAbility> GenerateFor(Item targetItem, string power, int quantity);
        IEnumerable<SpecialAbility> GenerateFor(IEnumerable<SpecialAbility> abilityNames, string criticalMultiplier);
        Weapon ApplyAbilitiesToWeapon(Weapon weapon);
    }
}