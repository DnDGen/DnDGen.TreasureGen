using DnDGen.Infrastructure.Models;
using DnDGen.TreasureGen.Tables;
using System;

namespace DnDGen.TreasureGen.Selectors.Selections
{
    internal class IntelligenceDataSelection : DataSelection<IntelligenceDataSelection>
    {
        public string Senses { get; set; }
        public int LesserPowersCount { get; set; }
        public int GreaterPowersCount { get; set; }

        public override Func<string[], IntelligenceDataSelection> MapTo => Map;
        public override Func<IntelligenceDataSelection, string[]> MapFrom => Map;

        public override int SectionCount => 3;

        public static IntelligenceDataSelection Map(string[] splitData)
        {
            return new IntelligenceDataSelection
            {
                Senses = splitData[DataIndexConstants.Intelligence.Senses],
                LesserPowersCount = Convert.ToInt32(splitData[DataIndexConstants.Intelligence.LesserPowersCount]),
                GreaterPowersCount = Convert.ToInt32(splitData[DataIndexConstants.Intelligence.GreaterPowersCount]),
            };
        }

        public static string[] Map(IntelligenceDataSelection selection)
        {
            var data = new string[selection.SectionCount];
            data[DataIndexConstants.Intelligence.Senses] = selection.Senses;
            data[DataIndexConstants.Intelligence.LesserPowersCount] = selection.LesserPowersCount.ToString();
            data[DataIndexConstants.Intelligence.GreaterPowersCount] = selection.GreaterPowersCount.ToString();

            return data;
        }
    }
}