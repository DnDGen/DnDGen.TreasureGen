using System.Collections.Generic;

namespace DnDGen.TreasureGen.Items.Magical
{
    public static class AlignmentConstants
    {
        public const string Lawful = "Lawful";
        public const string Chaotic = "Chaotic";
        public const string Evil = "Evil";
        public const string Good = "Good";
        public const string Neutral = "Neutral";

        public static string LawfulGood => $"{Lawful} {Good}";
        public static string NeutralGood => $"{Neutral} {Good}";
        public static string ChaoticGood => $"{Chaotic} {Good}";
        public static string LawfulNeutral => $"{Lawful} {Neutral}";
        public static string TrueNeutral => $"True {Neutral}";
        public static string ChaoticNeutral => $"{Chaotic} {Neutral}";
        public static string LawfulEvil => $"{Lawful} {Evil}";
        public static string NeutralEvil => $"{Neutral} {Evil}";
        public static string ChaoticEvil => $"{Chaotic} {Evil}";

        public static IEnumerable<string> GetAllAlignments() =>
            [
                LawfulGood,
                NeutralGood,
                ChaoticGood,
                LawfulNeutral,
                TrueNeutral,
                ChaoticNeutral,
                LawfulEvil,
                NeutralEvil,
                ChaoticEvil
            ];

        public static IEnumerable<string> GetAllPartialAlignments() =>
            [
                Lawful,
                Chaotic,
                Evil,
                Good,
                Neutral
            ];
    }
}