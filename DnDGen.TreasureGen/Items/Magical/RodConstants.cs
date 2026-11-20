using System.Collections.Generic;

namespace DnDGen.TreasureGen.Items.Magical
{
    public static class RodConstants
    {
        public const string Absorption = "Rod of Absorption";
        internal const string Absorption_Full = "Rod of Absorption (max)";
        public const string Alertness = "Rod of Alertness";
        public const string Cancellation = "Rod of Cancellation";
        public const string EnemyDetection = "Rod of Enemy Detection";
        public const string Flailing = "Rod of Flailing";
        public const string FlameExtinguishing = "Rod of Flame Extinguishing";
        public const string ImmovableRod = "Immovable Rod";
        public const string LordlyMight = "Rod of Lordly Might";
        public const string MetalAndMineralDetection = "Rod of Metal and Mineral Detection";
        public const string Metamagic_Empower = "Rod of Metamagic: Empower";
        public const string Metamagic_Empower_Greater = "Rod of Greater Metamagic: Empower";
        public const string Metamagic_Empower_Lesser = "Rod of Lesser Metamagic: Empower";
        public const string Metamagic_Enlarge = "Rod of Metamagic: Enlarge";
        public const string Metamagic_Enlarge_Greater = "Rod of Greater Metamagic: Enlarge";
        public const string Metamagic_Enlarge_Lesser = "Rod of Lesser Metamagic: Enlarge";
        public const string Metamagic_Extend = "Rod of Metamagic: Extend";
        public const string Metamagic_Extend_Greater = "Rod of Greater Metamagic: Extend";
        public const string Metamagic_Extend_Lesser = "Rod of Lesser Metamagic: Extend";
        public const string Metamagic_Maximize = "Rod of Metamagic: Maximize";
        public const string Metamagic_Maximize_Greater = "Rod of Greater Metamagic: Maximize";
        public const string Metamagic_Maximize_Lesser = "Rod of Lesser Metamagic: Maximize";
        public const string Metamagic_Quicken = "Rod of Metamagic: Quicken";
        public const string Metamagic_Quicken_Greater = "Rod of Greater Metamagic: Quicken";
        public const string Metamagic_Quicken_Lesser = "Rod of Lesser Metamagic: Quicken";
        public const string Metamagic_Silent = "Rod of Metamagic: Silent";
        public const string Metamagic_Silent_Greater = "Rod of Greater Metamagic: Silent";
        public const string Metamagic_Silent_Lesser = "Rod of Lesser Metamagic: Silent";
        public const string Negation = "Rod of Negation";
        public const string Python = "Rod of the Python";
        public const string Rulership = "Rod of Rulership";
        public const string Security = "Rod of Security";
        public const string Splendor = "Rod of Splendor";
        public const string ThunderAndLightning = "Rod of Thunder and Lightning";
        public const string Viper = "Rod of the Viper";
        public const string Withering = "Rod of Withering";
        public const string Wonder = "Rod of Wonder";

        public static IEnumerable<string> GetAllRods() =>
            [
                Absorption,
                Alertness,
                Cancellation,
                EnemyDetection,
                Flailing,
                FlameExtinguishing,
                ImmovableRod,
                LordlyMight,
                MetalAndMineralDetection,
                Metamagic_Empower,
                Metamagic_Empower_Greater,
                Metamagic_Empower_Lesser,
                Metamagic_Enlarge,
                Metamagic_Enlarge_Greater,
                Metamagic_Enlarge_Lesser,
                Metamagic_Extend,
                Metamagic_Extend_Greater,
                Metamagic_Extend_Lesser,
                Metamagic_Maximize,
                Metamagic_Maximize_Greater,
                Metamagic_Maximize_Lesser,
                Metamagic_Quicken,
                Metamagic_Quicken_Greater,
                Metamagic_Quicken_Lesser,
                Metamagic_Silent,
                Metamagic_Silent_Greater,
                Metamagic_Silent_Lesser,
                Negation,
                Python,
                Rulership,
                Security,
                Splendor,
                ThunderAndLightning,
                Viper,
                Withering,
                Wonder
            ];
    }
}