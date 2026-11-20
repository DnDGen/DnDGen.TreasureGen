using System.Collections.Generic;

namespace DnDGen.TreasureGen.Items.Mundane
{
    public static class ToolConstants
    {
        public const string Backpack_Empty = "Empty backpack";
        public const string Crowbar = "Crowbar";
        public const string Lantern_Bullseye = "Bullseye lantern";
        public const string Lock_Simple = "Simple lock";
        public const string Lock_Average = "Average lock";
        public const string Lock_Good = "Good lock";
        public const string Lock_Superior = "Superior lock";
        public const string Manacles_Masterwork = "Masterwork manacles";
        public const string Mirror_SmallSteel = "Small steel mirror";
        public const string Rope_Silk = "Silk rope (50')";
        public const string Spyglass = "Spyglass";
        public const string ArtisansTools_Masterwork = "Masterwork artisan's tools";
        public const string ClimbersKit = "Climber's kit";
        public const string DisguiseKit = "Disguise kit";
        public const string HealersKit = "Healer's kit";
        public const string HolySymbol_Silver = "Silver holy symbol";
        public const string Hourglass = "Hourglass";
        public const string MagnifyingGlass = "Magnifying glass";
        public const string MusicalInstrument_Masterwork = "Masterwork musical instrument";
        public const string ThievesTools_Masterwork = "Masterwork thieves' tools";

        public static IEnumerable<string> GetAllTools() =>
            [
                Backpack_Empty,
                Crowbar,
                Lantern_Bullseye,
                Lock_Simple,
                Lock_Average,
                Lock_Good,
                Lock_Superior,
                Manacles_Masterwork,
                Mirror_SmallSteel,
                Rope_Silk,
                Spyglass,
                ArtisansTools_Masterwork,
                ClimbersKit,
                DisguiseKit,
                HealersKit,
                HolySymbol_Silver,
                Hourglass,
                MagnifyingGlass,
                MusicalInstrument_Masterwork,
                ThievesTools_Masterwork
            ];
    }
}
