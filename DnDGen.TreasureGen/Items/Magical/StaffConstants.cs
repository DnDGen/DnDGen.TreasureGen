using System.Collections.Generic;

namespace DnDGen.TreasureGen.Items.Magical
{
    public static class StaffConstants
    {
        public const string Charming = "Staff of Charming";
        public const string Fire = "Staff of Fire";
        public const string SwarmingInsects = "Staff of Swarming Insects";
        public const string Healing = "Staff of Healing";
        public const string SizeAlteration = "Staff of Size Alteration";
        public const string Illumination = "Staff of Illumination";
        public const string Frost = "Staff of Frost";
        public const string Defense = "Staff of Defense";
        public const string Abjuration = "Staff of Abjuration";
        public const string Conjuration = "Staff of Conjuration";
        public const string Enchantment = "Staff of Enchantment";
        public const string Evocation = "Staff of Evocation";
        public const string Illusion = "Staff of Illusion";
        public const string Necromancy = "Staff of Necromancy";
        public const string Transmutation = "Staff of Transmutation";
        public const string Divination = "Staff of Divination";
        public const string EarthAndStone = "Staff of Earth and Stone";
        public const string Woodlands = "Staff of Woodlands";
        public const string Life = "Staff of Life";
        public const string Passage = "Staff of Passage";
        public const string Power = "Staff of Power";

        public static IEnumerable<string> GetAllStaffs() =>
            [
                Charming,
                Fire,
                SwarmingInsects,
                Healing,
                SizeAlteration,
                Illumination,
                Frost,
                Defense,
                Abjuration,
                Conjuration,
                Enchantment,
                Evocation,
                Illusion,
                Necromancy,
                Transmutation,
                Divination,
                EarthAndStone,
                Woodlands,
                Life,
                Passage,
                Power
            ];
    }
}