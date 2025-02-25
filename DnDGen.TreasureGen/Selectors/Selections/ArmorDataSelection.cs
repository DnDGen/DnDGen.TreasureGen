using DnDGen.Infrastructure.Models;
using DnDGen.TreasureGen.Tables;
using System;

namespace DnDGen.TreasureGen.Selectors.Selections
{
    internal class ArmorDataSelection : DataSelection<ArmorDataSelection>
    {
        public int ArmorBonus { get; set; }
        public int ArmorCheckPenalty { get; set; }
        public int MaxDexterityBonus { get; set; }

        public override Func<string[], ArmorDataSelection> MapTo => Map;
        public override Func<ArmorDataSelection, string[]> MapFrom => Map;

        public override int SectionCount => 3;

        public static ArmorDataSelection Map(string[] splitData)
        {
            var selection = new ArmorDataSelection
            {
                ArmorBonus = Convert.ToInt32(splitData[DataIndexConstants.Armor.ArmorBonus]),
                ArmorCheckPenalty = Convert.ToInt32(splitData[DataIndexConstants.Armor.ArmorCheckPenalty]),
                MaxDexterityBonus = Convert.ToInt32(splitData[DataIndexConstants.Armor.MaxDexterityBonus]),
            };

            return selection;
        }

        public static string[] Map(ArmorDataSelection selection)
        {
            var data = new string[selection.SectionCount];
            data[DataIndexConstants.Armor.ArmorCheckPenalty] = selection.ArmorCheckPenalty.ToString();
            data[DataIndexConstants.Armor.ArmorBonus] = selection.ArmorBonus.ToString();
            data[DataIndexConstants.Armor.MaxDexterityBonus] = selection.MaxDexterityBonus.ToString();

            return data;
        }
    }
}
