using DnDGen.Infrastructure.Models;
using DnDGen.TreasureGen.Tables;
using System;

namespace DnDGen.TreasureGen.Selectors.Selections
{
    internal class SpecialAbilityDataSelection : DataSelection<SpecialAbilityDataSelection>
    {
        public int BonusEquivalent { get; set; }
        public int Power { get; set; }
        public string BaseName { get; set; }

        public override Func<string[], SpecialAbilityDataSelection> MapTo => Map;
        public override Func<SpecialAbilityDataSelection, string[]> MapFrom => Map;

        public override int SectionCount => 3;

        public static SpecialAbilityDataSelection Map(string[] splitData)
        {
            return new SpecialAbilityDataSelection
            {
                BonusEquivalent = Convert.ToInt32(splitData[DataIndexConstants.SpecialAbility.BonusEquivalent]),
                Power = Convert.ToInt32(splitData[DataIndexConstants.SpecialAbility.Power]),
                BaseName = splitData[DataIndexConstants.SpecialAbility.BaseName],
            };
        }

        public static string[] Map(SpecialAbilityDataSelection selection)
        {
            var data = new string[selection.SectionCount];
            data[DataIndexConstants.SpecialAbility.BonusEquivalent] = selection.BonusEquivalent.ToString();
            data[DataIndexConstants.SpecialAbility.Power] = selection.Power.ToString();
            data[DataIndexConstants.SpecialAbility.BaseName] = selection.BaseName;

            return data;
        }
    }
}