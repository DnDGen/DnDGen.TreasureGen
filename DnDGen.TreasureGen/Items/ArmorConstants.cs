using System.Collections.Generic;
using System.Linq;

namespace DnDGen.TreasureGen.Items
{
    public static class ArmorConstants
    {
        public const string PaddedArmor = "Padded armor";
        public const string LeatherArmor = "Leather armor";
        public const string StuddedLeatherArmor = "Studded leather armor";
        public const string ChainShirt = "Chain shirt";
        public const string HideArmor = "Hide armor";
        public const string ScaleMail = "Scale mail";
        public const string Chainmail = "Chainmail";
        public const string Breastplate = "Breastplate";
        public const string SplintMail = "Splint mail";
        public const string BandedMail = "Banded mail";
        public const string HalfPlate = "Half-plate";
        public const string FullPlate = "Full plate";
        public const string Buckler = "Buckler";
        public const string LightWoodenShield = "Light wooden shield";
        public const string LightSteelShield = "Light steel shield";
        public const string HeavyWoodenShield = "Heavy wooden shield";
        public const string HeavySteelShield = "Heavy steel shield";
        public const string TowerShield = "Tower shield";
        public const string ElvenChain = "Elven Chain";
        public const string RhinoHide = "Rhino Hide";
        public const string DwarvenPlate = "Dwarven Plate";
        public const string BandedMailOfLuck = "Banded Mail of Luck";
        public const string CelestialArmor = "Celestial Armor";
        public const string PlateArmorOfTheDeep = "Plate Armor of the Deep";
        public const string BreastplateOfCommand = "Breastplate of Command";
        public const string FullPlateOfSpeed = "Full Plate of Speed";
        public const string DemonArmor = "Demon Armor";
        public const string CastersShield = "Caster's Shield";
        public const string SpinedShield = "Spined Shield";
        public const string LionsShield = "Lion's Shield";
        public const string WingedShield = "Winged Shield";
        public const string AbsorbingShield = "Absorbing Shield";
        public const string ArmorOfRage = "Armor of Rage";
        public const string ArmorOfArrowAttraction = "Armor of Arrow Attraction";

        public static IEnumerable<string> GetAllArmorsAndShields(bool includeSpecific)
        {
            var armors = GetAllArmors(includeSpecific);
            var shields = GetAllShields(includeSpecific);

            return armors.Union(shields);
        }

        public static IEnumerable<string> GetAllSpecificArmorsAndShields()
        {
            var armors = GetAllSpecificArmors();
            var shields = GetAllSpecificShields();

            return armors.Union(shields);
        }

        public static IEnumerable<string> GetAllSpecificArmors() =>
            [
                ElvenChain,
                CelestialArmor,
                RhinoHide,
                BreastplateOfCommand,
                DwarvenPlate,
                BandedMailOfLuck,
                PlateArmorOfTheDeep,
                FullPlateOfSpeed,
                DemonArmor,
                ArmorOfArrowAttraction,
                ArmorOfRage
            ];

        public static IEnumerable<string> GetAllSpecificShields() =>
            [
                CastersShield,
                SpinedShield,
                LionsShield,
                WingedShield,
                AbsorbingShield,
            ];

        public static IEnumerable<string> GetAllShields(bool includeSpecific)
        {
            IEnumerable<string> shields =
            [
                Buckler,
                LightWoodenShield,
                LightSteelShield,
                HeavyWoodenShield,
                HeavySteelShield,
                TowerShield,
                CastersShield,
                SpinedShield,
                LionsShield,
                WingedShield,
                AbsorbingShield,
            ];

            if (!includeSpecific)
            {
                var specific = GetAllSpecificShields();
                shields = shields.Except(specific);
            }

            return shields;
        }

        public static IEnumerable<string> GetAllArmors(bool includeSpecific)
        {
            var light = GetAllLightArmors(includeSpecific);
            var medium = GetAllMediumArmors(includeSpecific);
            var heavy = GetAllHeavyArmors(includeSpecific);

            return light.Union(medium).Union(heavy);
        }

        public static IEnumerable<string> GetAllLightArmors(bool includeSpecific)
        {
            IEnumerable<string> armors =
            [
                PaddedArmor,
                LeatherArmor,
                StuddedLeatherArmor,
                ChainShirt,
                ElvenChain,
                CelestialArmor,
            ];

            if (!includeSpecific)
            {
                var specific = GetAllSpecificArmors();
                armors = armors.Except(specific);
            }

            return armors;
        }

        public static IEnumerable<string> GetAllMediumArmors(bool includeSpecific)
        {
            IEnumerable<string> armors =
            [
                HideArmor,
                ScaleMail,
                Chainmail,
                Breastplate,
                RhinoHide,
                BreastplateOfCommand,
            ];

            if (!includeSpecific)
            {
                var specific = GetAllSpecificArmors();
                armors = armors.Except(specific);
            }

            return armors;
        }

        public static IEnumerable<string> GetAllHeavyArmors(bool includeSpecific)
        {
            IEnumerable<string> armors =
            [
                SplintMail,
                BandedMail,
                HalfPlate,
                FullPlate,
                DwarvenPlate,
                BandedMailOfLuck,
                PlateArmorOfTheDeep,
                FullPlateOfSpeed,
                DemonArmor,
                ArmorOfArrowAttraction,
                ArmorOfRage
            ];

            if (!includeSpecific)
            {
                var specific = GetAllSpecificArmors();
                armors = armors.Except(specific);
            }

            return armors;
        }
    }
}