namespace DnDGen.TreasureGen.Tables
{
    internal static class TableNameConstants
    {
        public static object Percentileset { get; internal set; }

        public static class Percentiles
        {
            public const string AlchemicalItems = "AlchemicalItems";
            public const string BalorOrPitFiend = "BalorOrPitFiend";
            public const string CastersShieldContainsSpell = "CastersShieldContainsSpell";
            public const string CastersShieldSpellTypes = "CastersShieldSpellTypes";
            public const string CurseDrawbacks = "CurseDrawbacks";
            public const string CursedDependentSituations = "CursedDependentSituations";
            public const string Curses = "Curses";
            public const string DarkwoodShields = "DarkwoodShields";
            public const string Gender = "Gender";
            public const string HasSpecialMaterial = "HasSpecialMaterial";
            public const string HornOfValhallaTypes = "HornOfValhallaTypes";
            public const string IntelligenceAlignments = "IntelligenceAlignments";
            public const string IntelligenceDedicatedPowers = "IntelligenceDedicatedPowers";
            public const string IntelligenceLesserPowers = "IntelligenceLesserPowers";
            public const string IntelligenceGreaterPowers = "IntelligenceGreaterPowers";
            public const string IntelligenceSpecialPurposes = "IntelligenceSpecialPurposes";
            public const string IntelligenceStrongStats = "IntelligenceStrongStats";
            public const string IronFlaskContents = "IronFlaskContents";
            public const string IsDeckOfIllusionsFullyCharged = "IsDeckOfIllusionsFullyCharged";
            public const string IsItemCursed = "IsItemCursed";
            public const string IsMasterwork = "IsMasterwork";
            public const string Languages = "Languages";
            public const string MagicalWeaponTypes = "MagicalWeaponTypes";
            public const string MeleeWeaponTraits = "MeleeWeaponTraits";
            public const string MundaneArmors = "MundaneArmors";
            public const string MundaneGearSizes = "MundaneGearSizes";
            public const string MundaneShields = "MundaneShields";
            public const string MundaneWeaponTypes = "MundaneWeaponTypes";
            public const string PersonalityTraits = "PersonalityTraits";
            public const string Planes = "Planes";
            public const string RangedWeaponTraits = "RangedWeaponTraits";
            public const string RobeOfTheArchmagiColors = "RobeOfTheArchmagiColors";
            public const string RobeOfUsefulItemsExtraItems = "RobeOfUsefulItemsExtraItems";
            public const string RodOfAbsorptionContainsSpellLevels = "RodOfAbsorptionContainsSpellLevels";
            public const string SpecificCursedItems = "SpecificCursedItems";
            public const string SpellStoringContainsSpell = "SpellStoringContainsSpell";
            public const string SpellTypes = "SpellTypes";
            public const string Tools = "Tools";

            public static string LevelXCoins(int level) => $"Level{level}Coins";
            public static string LevelXGoods(int level) => $"Level{level}Goods";
            public static string LevelXItems(int level) => $"Level{level}Items";
            public static string GOODTYPEValues(string goodType) => $"{goodType}Values";
            public static string POWERItems(string power) => $"{power}Items";
            public static string POWERITEMTYPEs(string power, string itemType) => $"{power}{itemType}s";
            public static string ITEMTYPETraits(string itemType) => $"{itemType}Traits";
            public static string WEAPONTYPEWeapons(string weaponType) => $"{weaponType}Weapons";
            public static string POWERATTRIBUTESpecialAbilities(string power, string attribute) => $"{power}{attribute}SpecialAbilities";
            public static string POWERSpecificITEMTYPEs(string power, string itemType) => $"{power}Specific{itemType}s";
            public static string ARMORTYPETypes(string armorType) => $"{armorType}Types";
            public static string LevelXSPELLTYPESpells(int level, string spellType) => $"Level{level}{spellType}Spells";
            public static string POWERSpellLevels(string power) => $"{power}SpellLevels";
        }

        public static class Collections
        {
            public const string ArmorData = "ArmorData";
            public const string ChargeLimits = "ChargeLimits";
            public const string ExtraItems = "ExtraItems";
            public const string IntelligenceData = "IntelligenceData";
            public const string IntelligenceCommunication = "IntelligenceCommunication";
            public const string IsIntelligent = "IsIntelligent";
            public const string ItemAlignmentRequirements = "ItemAlignmentRequirements";
            public const string ItemGroups = "ItemGroups";
            public const string PowerGroups = "PowerGroups";
            public const string ReplacementStrings = "ReplacementStrings";
            public const string SpecialAbilityData = "SpecialAbilityData";
            public const string SpecialAbilityAttributeRequirements = "SpecialAbilityAttributeRequirements";
            public const string SpecialMaterials = "SpecialMaterials";
            public const string SpecificCursedItemItemTypes = "SpecificCursedItemItemTypes";
            public const string SpecificCursedItemAttributes = "SpecificCursedItemAttributes";
            public const string AbilityDamages = "AbilityDamages";
            public const string WeaponDamages = "WeaponDamages";
            public const string WeaponCriticalDamages = "WeaponCriticalDamages";
            public const string WeaponData = "WeaponData";
            public const string WondrousItemContents = "WondrousItemContents";

            public static string GOODTYPEDescriptions(string goodType) => $"{goodType}Descriptions";
            public static string ITEMTYPEAttributes(string itemType) => $"{itemType}Attributes";
            public static string SpecificITEMTYPEAttributes(string itemType) => $"Specific{itemType}Attributes";
            public static string SpecificITEMTYPESpecialAbilities(string itemType) => $"Specific{itemType}SpecialAbilities";
            public static string SpecificITEMTYPETraits(string itemType) => $"Specific{itemType}Traits";
        }
    }
}