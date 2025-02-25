using DnDGen.Infrastructure.Models;
using DnDGen.TreasureGen.Tables;
using System;

namespace DnDGen.TreasureGen.Selectors.Selections
{
    internal class DamageDataSelection : DataSelection<DamageDataSelection>
    {
        public string Roll { get; set; }
        public string Type { get; set; }
        public string Condition { get; set; }

        public override Func<string[], DamageDataSelection> MapTo => Map;
        public override Func<DamageDataSelection, string[]> MapFrom => Map;

        public override int SectionCount => 3;

        public DamageDataSelection()
        {
            Roll = string.Empty;
            Type = string.Empty;
            Condition = string.Empty;
        }

        public static DamageDataSelection Map(string[] splitData)
        {
            return new DamageDataSelection
            {
                Roll = splitData[DataIndexConstants.Weapon.DamageData.RollIndex],
                Type = splitData[DataIndexConstants.Weapon.DamageData.TypeIndex],
                Condition = splitData[DataIndexConstants.Weapon.DamageData.ConditionIndex],
            };
        }

        public static string[] Map(DamageDataSelection selection)
        {
            var data = new string[selection.SectionCount];
            data[DataIndexConstants.Weapon.DamageData.RollIndex] = selection.Roll;
            data[DataIndexConstants.Weapon.DamageData.TypeIndex] = selection.Type;
            data[DataIndexConstants.Weapon.DamageData.ConditionIndex] = selection.Condition;

            return data;
        }
    }
}
