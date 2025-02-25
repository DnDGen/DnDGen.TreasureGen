using System.Collections.Generic;

namespace DnDGen.TreasureGen.Items
{
    public static class TraitConstants
    {
        public const string Masterwork = "Masterwork";
        public const string Markings = "Markings provide a clue to its function";
        public const string ShedsLight = "Sheds light";

        public static class SpecialMaterials
        {
            public const string Darkwood = "Darkwood";
            public const string Adamantine = "Adamantine";
            public const string Dragonhide = "Dragonhide";
            public const string ColdIron = "Cold iron";
            public const string Mithral = "Mithral";
            public const string AlchemicalSilver = "Alchemical silver";

            public static IEnumerable<string> GetAll() => [
                Darkwood,
                Adamantine,
                Dragonhide,
                ColdIron,
                Mithral,
                AlchemicalSilver
            ];
        }

        public static class Sizes
        {
            public const string Tiny = "Tiny";
            public const string Small = "Small";
            public const string Medium = "Medium";
            public const string Large = "Large";
            public const string Huge = "Huge";
            public const string Gargantuan = "Gargantuan";
            public const string Colossal = "Colossal";

            public static IEnumerable<string> GetAll() => [
                Tiny,
                Small,
                Medium,
                Large,
                Huge,
                Gargantuan,
                Colossal
            ];
        }
    }
}