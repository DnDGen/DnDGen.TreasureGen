using System.Collections.Generic;

namespace DnDGen.TreasureGen.Items.Magical
{
    public class Intelligence
    {
        public int IntelligenceStat { get; set; }
        public int WisdomStat { get; set; }
        public int CharismaStat { get; set; }
        public List<string> Powers { get; set; }
        public string SpecialPurpose { get; set; }
        public string DedicatedPower { get; set; }
        public int Ego { get; set; }
        public IEnumerable<string> Communication { get; set; }
        public List<string> Languages { get; set; }
        public string Senses { get; set; }
        public string Alignment { get; set; }
        public string Personality { get; set; }

        public Intelligence()
        {
            Powers = [];
            SpecialPurpose = string.Empty;
            DedicatedPower = string.Empty;
            Communication = [];
            Senses = string.Empty;
            Alignment = string.Empty;
            Languages = [];
            Personality = string.Empty;
        }

        public Intelligence Clone()
        {
            var clone = new Intelligence
            {
                Alignment = Alignment,
                CharismaStat = CharismaStat,
                Communication = [.. Communication],
                DedicatedPower = DedicatedPower,
                Ego = Ego,
                IntelligenceStat = IntelligenceStat,
                Languages = [.. Languages],
                Personality = Personality,
                Powers = [.. Powers],
                Senses = Senses,
                SpecialPurpose = SpecialPurpose,
                WisdomStat = WisdomStat
            };

            return clone;
        }
    }
}