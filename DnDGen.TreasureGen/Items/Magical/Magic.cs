using System.Collections.Generic;

namespace DnDGen.TreasureGen.Items.Magical
{
    public class Magic
    {
        public int Bonus { get; set; }
        public int Charges { get; set; }
        public IEnumerable<SpecialAbility> SpecialAbilities { get; set; }
        public string Curse { get; set; }
        public Intelligence Intelligence { get; set; }

        public Magic()
        {
            SpecialAbilities = [];
            Curse = string.Empty;
            Intelligence = new Intelligence();
        }
    }
}